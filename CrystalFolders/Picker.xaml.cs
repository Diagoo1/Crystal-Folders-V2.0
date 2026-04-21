using HandyControl.Data;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CrystalFolders
{
    public partial class Picker : Window
    {
        public Picker()
        {
            InitializeComponent();
            this.FlowDirection = (Config.currentLan == "ar") ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // ضبط الـ Picker ليظهر اللون الحالي بشفافيته المخزنة
            PickerControl.SelectedBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Config.HEX));
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            // اللون الافتراضي معتم تماماً
            PickerControl.SelectedBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0984E3"));
            Apply_Click(sender, e);
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (PickerControl.SelectedBrush != null)
            {
                // هنا يتم حفظ اللون بالكامل مع الـ Alpha (مثال: #800984E3)
                Config.HEX = PickerControl.SelectedBrush.Color.ToString();
                Config.ApplyTheme();
            }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void PickerControl_SelectedColorChanged(object sender, FunctionEventArgs<Color> e)
        {
            // تركنا هذه الدالة فارغة للسماح للمستخدم باختيار أي درجة شفافية يريدها من شريط Opacity
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}