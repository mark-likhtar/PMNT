using System;
using System.Collections.Generic;
using System.Configuration;

namespace PSLNLExportUtility.Insfrastructure.Settings
{
    public static class SettingsLoader
    {
        private static readonly Dictionary<string, dynamic> _cache = new Dictionary<string, dynamic>();

        public static T GetValue<T>(string key)
        {
            if (_cache.TryGetValue(key, out dynamic result))
            {
                return (T)result;
            }

            T value = ReadValueFromSettings<T>(key);
            _cache.Add(key, value);

            return value;
        }

        private static T ReadValueFromSettings<T>(string key)
        {
            try
            {
                var sourceValue = ConfigurationManager.AppSettings[key];
                var targetType = typeof(T);

                if (
                    targetType.IsGenericType &&
                    targetType
                        .GetGenericTypeDefinition()
                        .Equals(typeof(Nullable<>))
                )
                {
                    if (sourceValue == null)
                    {
                        return default(T);
                    }

                    targetType = Nullable.GetUnderlyingType(targetType);
                }

                return (T)Convert.ChangeType(sourceValue, targetType);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Couldn't get value for key '{key}'. Exception: {ex}.");
            }
        }
    }
}
