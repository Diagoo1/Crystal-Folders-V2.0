using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CrystalFolders
{
    public partial class About : Window
    {
        #region النصوص المدمجة (إنجليزي وعربي)

        // --- نص ترخيص Crystal Folders ---
        private const string CF_License_EN = @"Free License

Copyright (c) 2026 - 2027 Tarek Sadek & Génesis Toxical

Developed version by: Tarek Sadek

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.";

        private const string CF_License_AR = @"رخصة مجانية

حقوق الطبع والنشر (ج) 2026 - 2027 طارق صادق و جينيسيس توكسيكال

تم تطوير البرنامج بواسطة: طارق صادق

يُمنح الإذن بموجب هذا، مجانًا، لأي شخص يحصل على نسخة من هذا البرنامج وملفات التوثيق ذات الصلة، للتعامل في البرنامج دون قيود، بما في ذلك على سبيل المثال لا الحصر حقوق استخدام ونسخ وتعديل ودمج ونشر وتوزيع وترخيص و/أو بيع نسخ من البرنامج، وفقًا للشروط التالية:

يجب إدراج إشعار حقوق الطبع والنشر أعلاه وإشعار الإذن هذا في جميع النسخ أو الأجزاء الجوهرية من البرنامج.

يتم تقديم البرنامج ""كما هو""، دون أي ضمان من أي نوع، صريحًا كان أو ضمنيًا. لا يتحمل المؤلفون أو أصحاب حقوق الطبع والنشر في أي حال من الأحوال المسؤولية عن أي مطالبة أو أضرار أو مسؤولية أخرى تنشأ عن أو تتعلق بالبرنامج أو استخدامه.";

        // --- نص ترخيص Crystal Icons ---
        private const string CI_License_EN = @"Free License

Copyright (c) 2026 - 2027 Tarek Sadek

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.";

        private const string CI_License_AR = @"رخصة مجانية

حقوق الطبع والنشر (ج) 2026 - 2027 طارق صادق - برنامج مجاني

يُمنح الإذن بموجب هذا، مجانًا، لأي شخص يحصل على نسخة من هذا البرنامج وملفات التوثيق ذات الصلة، للتعامل في البرنامج دون قيود، بما في ذلك حقوق الاستخدام والنسخ والتعديل والنشر والتوزيع والترخيص، وفقًا للشروط التالية:

يجب إدراج إشعار حقوق الطبع والنشر أعلاه وإشعار الإذن هذا في جميع النسخ أو الأجزاء الجوهرية من البرنامج.

يتم تقديم البرنامج ""كما هو""، دون أي ضمان من أي نوع. هذا البرنامج مجاني بالكامل، ولا يتحمل المطور (طارق صادق) أي مسؤولية عن أي مطالبات أو أضرار ناتجة عن استخدامه.";

        #endregion

        public About()
        {
            InitializeComponent();
            if (LangBox != null)
            {
                LangBox.SelectionChanged += LangBox_SelectionChanged;
            }
        }

        private string GetStr(string key) => Application.Current.TryFindResource(key)?.ToString() ?? key;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (LangBox != null)
            {
                LangBox.SelectedIndex = (Config.currentLan == "ar") ? 1 : 0;
            }
            UpdateUI();
        }

        private void UpdateUI()
        {
            this.FlowDirection = (Config.currentLan == "ar") ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            if (Arrw_2 != null && Arrw_2.Visibility == Visibility.Visible)
                Btn_2_MouseDown(null, null);
            else
                Btn_1_MouseDown(null, null);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Owner != null) this.Owner.Activate();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void CF_info()
        {
            try
            {
                if (License != null)
                    License.Text = (Config.currentLan == "ar") ? CF_License_AR : CF_License_EN;

                if (Description != null)
                {
                    string desc = (Config.currentLan == "ar") ? "الموقع الرسمي لـ كريستال فولدرز" : "Crystal Folders Official Website";
                    Description.Text = $"{desc} - v{GetStr("Ver")}";
                }
            }
            catch { }
        }

        private void Arrows(FrameworkElement visibleArrw, FrameworkElement opacityBtn)
        {
            if (Arrw_1 != null) Arrw_1.Visibility = Visibility.Hidden;
            if (Arrw_2 != null) Arrw_2.Visibility = Visibility.Hidden;

            if (this.FindName("Btn_1") is TextBlock btn1Text) btn1Text.Opacity = 1;
            if (this.FindName("Btn_2") is TextBlock btn2Text) btn2Text.Opacity = 1;

            if (visibleArrw != null) visibleArrw.Visibility = Visibility.Visible;
            if (opacityBtn != null) opacityBtn.Opacity = 0.7;
        }

        private void Btn_1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CF_info();
            Arrows(Arrw_1, this.FindName("Btn_1") as FrameworkElement);
            if (HeaderTitle != null) HeaderTitle.SetResourceReference(TextBlock.TextProperty, "CrystalFoldersTitle");
        }

        private void Btn_2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (License != null)
                    License.Text = (Config.currentLan == "ar") ? CI_License_AR : CI_License_EN;

                if (Description != null)
                    Description.Text = (Config.currentLan == "ar") ? "كريستال أيكونز - محول الصور للأيقونات" : "Crystal Icons - Image to Icon Converter";

                Arrows(Arrw_2, this.FindName("Btn_2") as FrameworkElement);
                if (HeaderTitle != null) HeaderTitle.SetResourceReference(TextBlock.TextProperty, "CrystalIconsTitle");
            }
            catch { }
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            string url = (Arrw_2 != null && Arrw_2.Visibility == Visibility.Visible)
                ? "https://github.com/Diagoo1" // رابط بروفايلك أو المشروع
                : "https://github.com/Diagoo1";

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{GetStr("ErrorOpeningLink")}\n{ex.Message}");
            }
        }

        private void LangBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LangBox == null || !IsLoaded) return;
            string selectedLang = (LangBox.SelectedIndex == 1) ? "ar" : "en";
            if (Config.currentLan != selectedLang)
            {
                App.SwitchLanguage(selectedLang);
                UpdateUI();
            }
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Picker { Owner = this };
            picker.ShowDialog();
        }

        private void Back_Click(object sender, RoutedEventArgs e) => Close();
    }
}