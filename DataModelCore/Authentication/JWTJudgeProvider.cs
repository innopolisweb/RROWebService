using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace DataModelCore.Authentication
{
    public static class JWTJudgeProvider
    {
        public static string CreateToken(JudgePayload payload)
        {
            var key = Encoding.UTF8.GetBytes("roo-web-service-secure-Hrjkld-423y7842.f");
            var header = new JWTHeader { Algorithm = "RS256" };

            var headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header));
            var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));

            var encodedHeader = Base64UrlEncode(headerBytes);
            var encodedPayload = Base64UrlEncode(payloadBytes);

            var stringsToSign = encodedHeader + '.' + encodedPayload;
            var bytesToSign = Encoding.UTF8.GetBytes(stringsToSign);

            var signatureBytes = new HMACSHA256(key).ComputeHash(bytesToSign);
            var signature = Base64UrlEncode(signatureBytes);

            return stringsToSign + '.' + signature;

        }

        public static JudgePayload DecodeToken(string token)
        {
            var parts = token?.Split('.');
            if (parts?.Length != 3) return null;

            var header = parts[0];
            var payload = parts[1];
            var crypto = Base64UrlDecode(parts[2]);
            if (crypto == null) return null;

            var decodedHeader = Base64UrlDecode(header);
            if (decodedHeader == null) return null;
            var decodedPayload = Base64UrlDecode(payload);
            if (decodedPayload == null) return null;

            var headerJson = Encoding.UTF8.GetString(decodedHeader);
            var payloadJson = Encoding.UTF8.GetString(decodedPayload);

            var headerData = JsonConvert.DeserializeObject<JWTHeader>(headerJson);
            var payloadData = JsonConvert.DeserializeObject<JudgePayload>(payloadJson);

            if (headerData.Algorithm != "RS256") return null;

            var bytesToSign = Encoding.UTF8.GetBytes(header + '.' + payload);
            var key = Encoding.UTF8.GetBytes("roo-web-service-secure-Hrjkld-423y7842.f");

            var signature = new HMACSHA256(key).ComputeHash(bytesToSign);

            var decodedCrypto = Convert.ToBase64String(crypto);
            var decodedSignature = Convert.ToBase64String(signature);

            return decodedCrypto == decodedSignature ? payloadData : null;
        }

        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }

        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: return null;
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
    }
}