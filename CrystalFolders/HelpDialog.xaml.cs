using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CrystalFolders
{
    public partial class HelpDialog : Window
    {
        public HelpDialog()
        {
            InitializeComponent();

            // ضبط الاتجاه بناءً على اللغة
            this.FlowDirection = (Config.currentLan == "ar") ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            UpdateHelpContent();
        }

        private string GetStr(string key) => Application.Current.TryFindResource(key)?.ToString() ?? key;

        private void UpdateHelpContent()
        {
            HelpText.Inlines.Clear();

            // 1. الجزء الخاص بـ Portable Config
            Bold b1 = new Bold(new Run(GetStr("HelpPortableHeader"))) { Foreground = (SolidColorBrush)Application.Current.FindResource("AccentColor") };
            HelpText.Inlines.Add(b1);
            HelpText.Inlines.Add(new LineBreak());

            // اختيار النص حسب الحالة (Restore أم Customize)
            string portableDesc = MainWindow.isRestore ? GetStr("WillRemoveAnyIconsCopied") : GetStr("WillMakeTheCustomIconVisible");
            HelpText.Inlines.Add(new Run(portableDesc));

            HelpText.Inlines.Add(new LineBreak());
            HelpText.Inlines.Add(new LineBreak());

            // 2. الجزء الخاص بـ Usage
            Bold b2 = new Bold(new Run(GetStr("HelpUsageHeader"))) { Foreground = (SolidColorBrush)Application.Current.FindResource("AccentColor") };
            HelpText.Inlines.Add(b2);
            HelpText.Inlines.Add(new LineBreak());
            HelpText.Inlines.Add(new Run(GetStr("ItWorksByDraggingFolders")));
            HelpText.Inlines.Add(new LineBreak());

            string limitDesc = MainWindow.isRestore ? GetStr("YouCanRestoreUpTo30Folders") : GetStr("YouCanCustomizeUpTo30Folders");
            HelpText.Inlines.Add(new Run(limitDesc));
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}