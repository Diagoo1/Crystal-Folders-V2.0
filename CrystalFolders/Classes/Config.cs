using HandyControl.Themes;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace CrystalFolders
{
    internal static class Config
    {
        private static readonly string AppFolderName = "CrystalFolders";
        private static readonly string IniFileName = "Config.ini";
        private static readonly string iniPath = GetConfigPath();

        internal static string[] iniLines;
        internal static string currentLan = "en";
        internal static string HEX = "#FF0984E3";
        internal static bool isDarkMode = false;
        internal static SolidColorBrush colorBrush =
            new SolidColorBrush((Color)ColorConverter.ConvertFromString(HEX));

        static Config()
        {
            if (!File.Exists(iniPath))
                CreateDefaultConfig();
        }

        private static string GetConfigPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fullPath = Path.Combine(appDataPath, AppFolderName);

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            return Path.Combine(fullPath, IniFileName);
        }

        private static void CreateDefaultConfig()
        {
            string[] defaultConfig =
            {
                "[Settings]",
                "Language = en",
                "DarkMode = False",
                "[Theme]",
                "AccentColor = #FF0984E3"
            };

            File.WriteAllLines(iniPath, defaultConfig);
        }

        public static void LoadSettings()
        {
            iniLines = File.ReadAllLines(iniPath);

            var langLine = iniLines?.FirstOrDefault(l => l.Trim().StartsWith("Language"));
            if (langLine != null && langLine.Contains("="))
                currentLan = langLine.Split('=')[1].Trim();

            var darkLine = iniLines?.FirstOrDefault(l => l.Trim().StartsWith("DarkMode"));
            if (darkLine != null && darkLine.Contains("="))
                bool.TryParse(darkLine.Split('=')[1].Trim(), out isDarkMode);

            var colorLine = iniLines?.FirstOrDefault(l => l.Trim().StartsWith("AccentColor"));
            if (colorLine != null && colorLine.Contains("="))
                HEX = colorLine.Split('=')[1].Trim();

            ApplyTheme();
        }

        public static void UpdateIniFile(string key, string value)
        {
            if (iniLines == null)
                iniLines = File.ReadAllLines(iniPath);

            List<string> lines = iniLines.ToList();
            bool found = false;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Trim().StartsWith(key))
                {
                    lines[i] = $"{key} = {value}";
                    found = true;
                    break;
                }
            }

            if (!found)
                lines.Add($"{key} = {value}");

            iniLines = lines.ToArray();
            File.WriteAllLines(iniPath, iniLines);
        }

        public static void ApplyTheme()
        {
            try
            {
                ThemeManager.Current.ApplicationTheme =
                    isDarkMode ? ApplicationTheme.Dark : ApplicationTheme.Light;

                // ✅ ناخد قيمة الشفافية من لون الـ Accent نفسه
                Color accentColor =
                    (Color)ColorConverter.ConvertFromString(HEX);

                byte alpha = accentColor.A;

                if (isDarkMode)
                {
                    // ===== الدارك مود الأصلي زي ما كان =====

                    Color darkBack =
                        (Color)ColorConverter.ConvertFromString("#1A1A1A");
                    darkBack.A = alpha;

                    Color cardBack =
                        (Color)ColorConverter.ConvertFromString("#242424");
                    cardBack.A = alpha;

                    Application.Current.Resources["AppBackground"] =
                        new SolidColorBrush(darkBack);

                    Application.Current.Resources["CardBackground"] =
                        new SolidColorBrush(cardBack);

                    Application.Current.Resources["SecondaryRegionBrush"] =
                        new SolidColorBrush(cardBack);

                    Application.Current.Resources["RegionBrush"] =
                        new SolidColorBrush(darkBack);

                    // ✅ الهيدر يتأثر بالشفافية
                    Application.Current.Resources["HeaderBackground"] =
                        new SolidColorBrush(darkBack);

                    Application.Current.Resources["TextPrimary"] =
                        Brushes.White;

                    Application.Current.Resources["BorderColor"] =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#333333"));

                    Application.Current.Resources["HoverColor"] =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#3D3D3D"));
                }
                else
                {
                    // ===== اللايت مود الأصلي زي ما كان =====

                    Color lightBack =
                        (Color)ColorConverter.ConvertFromString("#F3F3F3");
                    lightBack.A = alpha;

                    Color whiteCard = Colors.White;
                    whiteCard.A = alpha;

                    Application.Current.Resources["AppBackground"] =
                        new SolidColorBrush(lightBack);

                    Application.Current.Resources["CardBackground"] =
                        new SolidColorBrush(whiteCard);

                    Application.Current.Resources["SecondaryRegionBrush"] =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#F1F2F6"));

                    Application.Current.Resources["RegionBrush"] =
                        new SolidColorBrush(whiteCard);

                    // ✅ الهيدر يتأثر بالشفافية
                    Application.Current.Resources["HeaderBackground"] =
                        new SolidColorBrush(lightBack);

                    Application.Current.Resources["TextPrimary"] =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#202020"));

                    Application.Current.Resources["BorderColor"] =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#E5E5E5"));

                    Application.Current.Resources["HoverColor"] =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#F9F9F9"));
                }

                // ===== Accent =====
                colorBrush = new SolidColorBrush(accentColor);
                colorBrush.Freeze();

                Application.Current.Resources["AccentColor"] = colorBrush;
                Application.Current.Resources["PrimaryBrush"] = colorBrush;
                Application.Current.Resources["PrimaryColor"] = accentColor;

                UpdateIniFile("AccentColor", HEX);
                UpdateIniFile("DarkMode", isDarkMode.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme error: {ex.Message}");
            }
        }
    }
}