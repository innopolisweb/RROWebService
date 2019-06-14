using System;
using System.Collections.Generic;

namespace RROScoreBoard.Services
{
    public static class ServiceProvider
    {
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

        public static void AddService<T>(T service)
        {
            Services[typeof(T)] = service;
        }

        public static T GetService<T>()
        {
            if (Services.ContainsKey(typeof(T))) return (T)Services[typeof(T)];
            throw new Exception($"Service {typeof(T)} is not registered");
        }
    }
}