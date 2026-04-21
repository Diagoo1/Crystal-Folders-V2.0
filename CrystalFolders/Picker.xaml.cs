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
            PickerControl.SelectedBrush = new SolidColorBrush(Config.colorBrush.Color);
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            PickerControl.SelectedBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0984E3"));
            Apply_Click(sender, e);
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (PickerControl.SelectedBrush != null)
            {
                Config.HEX = PickerControl.SelectedBrush.Color.ToString();
                Config.ApplyTheme();
            }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void PickerControl_SelectedColorChanged(object sender, FunctionEventArgs<Color> e)
        {
            if (PickerControl.SelectedBrush != null && PickerControl.SelectedBrush.Color.A < 255)
            {
                Color c = PickerControl.SelectedBrush.Color;
                PickerControl.SelectedBrush = new SolidColorBrush(Color.FromRgb(c.R, c.G, c.B));
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}