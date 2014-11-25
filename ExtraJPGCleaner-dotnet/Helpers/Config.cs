using System;
using System.Configuration;

namespace ExtraJPGCleaner.Helpers
{
    public static class Config
    {
        public static void Update(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Minimal);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        public static string Read(string key)
        {
            string result;

            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "";       
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                result = "";
            }

            return result;
        }
    }
}
