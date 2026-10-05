using System;
using System.IO;
using System.Linq;

namespace VpnGuardForGames
{
    enum PromptMode
    {
        Ask,
        Warn
    }

    sealed class Settings
    {
        public string[] Adapters = { "NordLayer" };
        public string Client = "NordLayer";
        public PromptMode Mode = PromptMode.Ask;

        public static string FilePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"); }
        }

        public static Settings Load()
        {
            var settings = new Settings();
            if (!File.Exists(FilePath))
            {
                return settings;
            }

            foreach (string line in File.ReadAllLines(FilePath))
            {
                string trimmed = line.Trim();
                int separator = trimmed.IndexOf('=');
                if (trimmed.StartsWith(";") || trimmed.StartsWith("#") || separator < 0)
                {
                    continue;
                }

                string key = trimmed.Substring(0, separator).Trim().ToLowerInvariant();
                string value = trimmed.Substring(separator + 1).Trim();

                switch (key)
                {
                    case "adapters":
                        settings.Adapters = value.Split(',').Select(part => part.Trim()).Where(part => part.Length > 0).ToArray();
                        break;
                    case "client":
                        settings.Client = value;
                        break;
                    case "mode":
                        settings.Mode = value.Equals("warn", StringComparison.OrdinalIgnoreCase) ? PromptMode.Warn : PromptMode.Ask;
                        break;
                }
            }

            return settings;
        }
    }
}
