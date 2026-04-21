using HandyControl.Themes;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CrystalFolders
{
    internal static class Config
    {
        // ======================= التغييرات الرئيسية هنا =======================

        // 1. تعريف المسارات كأساس للكلاس
        private static readonly string AppFolderName = "CrystalFolders";
        private static readonly string IniFileName = "Config.ini";

        // 2. تهيئة المسار مباشرة عند التعريف لضمان أنه لن يكون فارغاً أبداً
        //    استخدام private readonly يعتبر أفضل ممارسة هنا
        private static readonly string iniPath = GetConfigPath();

        // ====================================================================

        internal static string[] iniLines;
        internal static string currentLan = "en";
        internal static string HEX = "#FF0984E3";
        internal static bool isDarkMode = false;
        internal static SolidColorBrush colorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(HEX));


        // 3. الـ Static Constructor الآن وظيفته فقط هي إنشاء الملف عند الحاجة
        static Config()
        {
            // بما أن iniPath تم تعيينه بالفعل، نحن فقط نتأكد من وجود الملف
            if (!File.Exists(iniPath))
            {
                CreateDefaultConfig();
            }
        }

        // هذه الدالة الآن private لأنها تُستخدم داخليًا فقط عند تهيئة المسار
        private static string GetConfigPath()
        {
            // الحصول على مسار AppData/Roaming
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // دمج المسار: C:\Users\Username\AppData\Roaming\CrystalFolders
            string fullPath = Path.Combine(appDataPath, AppFolderName);

            // التأكد من أن المجلد موجود، وإذا لم يكن موجوداً يتم إنشاؤه
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // إرجاع المسار الكامل للملف
            return Path.Combine(fullPath, IniFileName);
        }

        private static void CreateDefaultConfig()
        {
            string[] defaultConfig = { "[Settings]", "Language = en", "DarkMode = False", "[Theme]", "AccentColor = #FF0984E3" };
            // سيتم الكتابة دائمًا في المسار الصحيح لأن iniPath مضمون القيمة
            File.WriteAllLines(iniPath, defaultConfig);
        }

        public static void LoadSettings()
        {
            // 4. لا داعي للتحقق من وجود الملف هنا مرة أخرى، لأن الـ constructor قد ضمن وجوده
            // if (!File.Exists(iniPath)) CreateDefaultConfig(); // <-- هذا السطر أصبح غير ضروري

            iniLines = File.ReadAllLines(iniPath);
            var langLine = iniLines?.FirstOrDefault(l => l.Trim().StartsWith("Language"));
            if (langLine != null && langLine.Contains("=")) currentLan = langLine.Split('=')[1].Trim();

            var darkLine = iniLines?.FirstOrDefault(l => l.Trim().StartsWith("DarkMode"));
            if (darkLine != null && darkLine.Contains("=")) bool.TryParse(darkLine.Split('=')[1].Trim(), out isDarkMode);

            var colorLine = iniLines?.FirstOrDefault(l => l.Trim().StartsWith("AccentColor"));
            if (colorLine != null && colorLine.Contains("=")) HEX = colorLine.Split('=')[1].Trim();

            ApplyTheme();
        }

        public static void UpdateIniFile(string key, string value)
        {
            // تأكد من تحميل الأسطر إذا لم يتم تحميلها من قبل
            if (iniLines == null)
            {
                iniLines = File.ReadAllLines(iniPath);
            }

            bool keyFound = false;
            for (int i = 0; i < iniLines.Length; i++)
            {
                if (iniLines[i].Trim().StartsWith(key))
                {
                    iniLines[i] = $"{key} = {value}";
                    keyFound = true;
                    break;
                }
            }
            if (!keyFound)
            {
                var newLines = iniLines.ToList();
                // إضافة منطق للتحقق من الأقسام لاحقاً إذا أردت
                newLines.Add($"{key} = {value}");
                iniLines = newLines.ToArray();
            }
            File.WriteAllLines(iniPath, iniLines);
        }

        // باقي الدوال كما هي بدون تغيير
        public static void ApplyTheme()
        {
            try
            {
                ThemeManager.Current.ApplicationTheme = isDarkMode ? ApplicationTheme.Dark : ApplicationTheme.Light;

                if (isDarkMode)
                {
                    var darkBack = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A"));
                    var cardBack = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#242424"));

                    Application.Current.Resources["AppBackground"] = darkBack;
                    Application.Current.Resources["CardBackground"] = cardBack;
                    Application.Current.Resources["TextPrimary"] = Brushes.White;
                    Application.Current.Resources["BorderColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
                    Application.Current.Resources["HoverColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3D3D3D"));

                    Application.Current.Resources["SecondaryRegionBrush"] = cardBack;
                    Application.Current.Resources["RegionBrush"] = darkBack;
                }
                else
                {
                    var lightBack = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F3F3"));
                    Application.Current.Resources["AppBackground"] = lightBack;
                    Application.Current.Resources["CardBackground"] = Brushes.White;
                    Application.Current.Resources["TextPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#202020"));
                    Application.Current.Resources["BorderColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E5E5"));
                    Application.Current.Resources["HoverColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F9F9F9"));

                    Application.Current.Resources["SecondaryRegionBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1F2F6"));
                    Application.Current.Resources["RegionBrush"] = Brushes.White;
                }

                colorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(HEX));
                colorBrush.Freeze();

                var hoverColor = new SolidColorBrush(GetHoverColor(colorBrush.Color));
                hoverColor.Freeze();

                Application.Current.Resources["AccentColor"] = colorBrush;
                Application.Current.Resources["AccentHoverColor"] = hoverColor;

                Application.Current.Resources["PrimaryBrush"] = colorBrush;
                Application.Current.Resources["PrimaryColor"] = colorBrush.Color;

                if (isDarkMode)
                {
                    var contrastingBrush = GetContrastingForeground(colorBrush.Color);
                    Application.Current.Resources["TextIconBrush"] = contrastingBrush;
                    Application.Current.Resources["PrimaryTextBrush"] = contrastingBrush;
                }
                else
                {
                    Application.Current.Resources["TextIconBrush"] = Brushes.Black;
                    Application.Current.Resources["PrimaryTextBrush"] = Brushes.Black;
                }

                UpdateIniFile("AccentColor", HEX);
                UpdateIniFile("DarkMode", isDarkMode.ToString());
            }
            catch (Exception ex)
            {
                // من الجيد تسجيل الأخطاء بدلاً من تجاهلها
                System.Diagnostics.Debug.WriteLine($"Error applying theme: {ex.Message}");
            }
        }

        private static Color GetHoverColor(Color baseColor)
        {
            float factor = 0.85f;
            return Color.FromRgb((byte)(baseColor.R * factor), (byte)(baseColor.G * factor), (byte)(baseColor.B * factor));
        }

        private static Brush GetContrastingForeground(Color backgroundColor)
        {
            double luminance = (0.299 * backgroundColor.R + 0.587 * backgroundColor.G + 0.114 * backgroundColor.B) / 255;
            return luminance > 0.5 ? Brushes.Black : Brushes.White;
        }
    }
}