using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace CrystalFolders
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Config.LoadSettings();
            SwitchLanguage(Config.currentLan, true);
        }

        // ✅ طريقة يدوية لتبديل الثيم تعمل مع جميع إصدارات HandyControl
        public void UpdateTheme(bool isDark)
        {
            try
            {
                // نحدد المسار الخاص بالثيم بناءً على حالة الدارك مود
                string skinName = isDark ? "SkinDark.xaml" : "SkinDefault.xaml";
                var newSkinUri = new Uri($"pack://application:,,,/HandyControl;component/Themes/{skinName}");

                var dictionaries = Current.Resources.MergedDictionaries;

                // نبحث عن القاموس القديم الخاص بـ HandyControl ونستبدله
                for (int i = 0; i < dictionaries.Count; i++)
                {
                    var dict = dictionaries[i];
                    if (dict.Source != null && dict.Source.OriginalString.Contains("HandyControl") && dict.Source.OriginalString.Contains("Skin"))
                    {
                        dictionaries[i] = new ResourceDictionary { Source = newSkinUri };
                        return;
                    }
                }

                // إذا لم نجده (في حال كانت المكتبة مدمجة بشكل مختلف)، نضيفه كقاموس جديد
                dictionaries.Add(new ResourceDictionary { Source = newSkinUri });
            }
            catch { /* في حال فشل الوصول للمكتبة لا يتوقف البرنامج */ }
        }

        public static void SwitchLanguage(string langCode, bool isInitialLoad = false)
        {
            if (string.IsNullOrEmpty(langCode)) langCode = "en";
            Config.currentLan = langCode;

            var culture = new CultureInfo(langCode);
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            try
            {
                var newDict = new ResourceDictionary
                {
                    Source = new Uri($"/Languages/StringResources.{langCode}.xaml", UriKind.RelativeOrAbsolute)
                };

                var dictionaries = Current.Resources.MergedDictionaries;
                var oldDict = dictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("StringResources."));

                if (oldDict != null)
                {
                    int index = dictionaries.IndexOf(oldDict);
                    dictionaries[index] = newDict;
                }
                else
                {
                    dictionaries.Add(newDict);
                }

                if (!isInitialLoad)
                {
                    foreach (Window window in Current.Windows)
                    {
                        window.FlowDirection = (langCode == "ar") ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                    }
                }

                Config.UpdateIniFile("Language", langCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Critical error loading language '{langCode}':\n{ex.Message}", "Localization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}