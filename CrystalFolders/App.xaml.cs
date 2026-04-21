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

        public void UpdateTheme(bool isDark)
        {
            try
            {
                Config.isDarkMode = isDark;
                Config.ApplyTheme();

                string skinName = isDark ? "SkinDark.xaml" : "SkinDefault.xaml";
                var newSkinUri = new Uri($"pack://application:,,,/HandyControl;component/Themes/{skinName}");

                var dictionaries = Current.Resources.MergedDictionaries;
                for (int i = 0; i < dictionaries.Count; i++)
                {
                    if (dictionaries[i].Source != null && dictionaries[i].Source.OriginalString.Contains("HandyControl") && dictionaries[i].Source.OriginalString.Contains("Skin"))
                    {
                        dictionaries[i] = new ResourceDictionary { Source = newSkinUri };
                        return;
                    }
                }
                dictionaries.Add(new ResourceDictionary { Source = newSkinUri });
            }
            catch { }
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
                var newDict = new ResourceDictionary { Source = new Uri($"/Languages/StringResources.{langCode}.xaml", UriKind.RelativeOrAbsolute) };
                var dictionaries = Current.Resources.MergedDictionaries;
                var oldDict = dictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("StringResources."));

                if (oldDict != null) dictionaries[dictionaries.IndexOf(oldDict)] = newDict;
                else dictionaries.Add(newDict);

                if (!isInitialLoad)
                {
                    foreach (Window window in Current.Windows)
                        window.FlowDirection = (langCode == "ar") ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                }
                Config.UpdateIniFile("Language", langCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Language Error: {ex.Message}");
            }
        }
    }
}