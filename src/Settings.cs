using System;
using System.IO;
using System.Linq;

namespace VpnGuardForGames
{
    enum ReminderMode
    {
        Off,
        Ask,
        Warn
    }

    sealed class Settings
    {
        public string[] Adapters = { "NordLayer" };
        public string Client = "NordLayer";
        public ReminderMode Mode = ReminderMode.Ask;
        public ReminderMode Reconnect = ReminderMode.Ask;

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
                        settings.Mode = ParseMode(value, settings.Mode);
                        break;
                    case "reconnect":
                        settings.Reconnect = ParseMode(value, settings.Reconnect);
                        break;
                }
            }

            return settings;
        }

        static ReminderMode ParseMode(string value, ReminderMode fallback)
        {
            switch (value.ToLowerInvariant())
            {
                case "off":
                    return ReminderMode.Off;
                case "ask":
                    return ReminderMode.Ask;
                case "warn":
                    return ReminderMode.Warn;
                default:
                    return fallback;
            }
        }
    }
}
