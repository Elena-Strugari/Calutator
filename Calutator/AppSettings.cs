using System;
using System.IO;

namespace Calutator
{
    public class AppSettings
    {
        private const string SettingsFile = "settings.txt";

        public bool DigitGroupingEnabled { get; set; }
        public string LastUsedMode { get; set; } = "Standard";

        public static AppSettings LoadSettings()
        {
            if (!File.Exists(SettingsFile))
                return new AppSettings();

            string[] lines = File.ReadAllLines(SettingsFile);
            return new AppSettings
            {
                DigitGroupingEnabled = lines.Length > 0 && lines[0] == "1",
                LastUsedMode = lines.Length > 1 ? lines[1] : "Standard"
            };
        }

        public void SaveSettings()
        {
            File.WriteAllLines(SettingsFile, new string[]
            {
                DigitGroupingEnabled ? "1" : "0",
                LastUsedMode
            });
        }
    }
}
