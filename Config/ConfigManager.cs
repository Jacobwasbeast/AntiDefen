using System;
using System.IO;
using Newtonsoft.Json;

namespace AntiDefen.Config
{
    public class ConfigManager
    {
        private static readonly string JsonFile = "config.json";
        private readonly string _jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFile);
        private readonly ConfigFile _configFile;

        public ConfigManager()
        {
            try
            {
                if (File.Exists(_jsonPath))
                {
                    string json = File.ReadAllText(_jsonPath);
                    _configFile = JsonConvert.DeserializeObject<ConfigFile>(json) ?? new ConfigFile();
                }
                else
                {
                    _configFile = new ConfigFile();
                    SaveConfig();
                }

                ValidateConfig();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Retrieves the current configuration.
        /// </summary>
        public ConfigFile GetConfig() => _configFile;

        /// <summary>
        /// Saves the current configuration to disk.
        /// </summary>
        public void SaveConfig()
        {
            try
            {
                string configContent = JsonConvert.SerializeObject(_configFile, Formatting.Indented);
                File.WriteAllText(_jsonPath, configContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving config: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates that essential configuration properties are set.
        /// </summary>
        private void ValidateConfig()
        {
            if (string.IsNullOrWhiteSpace(_configFile.discordToken))
            {
                Console.WriteLine($"Please enter your Discord Bot token into the {JsonFile} file located at {_jsonPath}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Returns the singleton instance of ConfigManager.
        /// </summary>
        public static ConfigManager GetInstance() => Program.ConfigManager;
    }
}
