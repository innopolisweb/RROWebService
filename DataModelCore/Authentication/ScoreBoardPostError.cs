namespace DataModelCore.Authentication
{
    public enum ScoreBoardPostError
    {
        RecordPayloadMismatch,
        OverwritingForbidden,
        InvalidToken,
        TokenExpired,
        TokenNotPassed,
        InvalidModelContainsNulls,
        AccessDenied
    }
}
