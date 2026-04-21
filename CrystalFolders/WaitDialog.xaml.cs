using System.Windows;

namespace CrystalFolders
{
    public partial class WaitDialog : Window
    {
        public WaitDialog()
        {
            InitializeComponent();

            // دعم اتجاه اللغة (عربي / إنجليزي) بناءً على الإعدادات
            this.FlowDirection = (Config.currentLan == "ar")
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;
        }
    }
}