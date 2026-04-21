using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows; // ✅ === الإضافة الأهم لحل كل الأخطاء ===
using System.Windows.Input;
using System.Windows.Media.Imaging;
using HandyControl.Controls;

namespace CrystalFolders
{
    public partial class IconConverterWindow : System.Windows.Window
    {
        private string selectedImagePath;

        public IconConverterWindow()
        {
            InitializeComponent();
            // ✅ تصحيح: استخدام النوع مباشرةً بدلاً من this
            this.FlowDirection = (Config.currentLan == "ar") ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp" };
            if (dlg.ShowDialog() == true)
            {
                selectedImagePath = dlg.FileName;
                ImgPreview.Source = new BitmapImage(new Uri(selectedImagePath));
                PlaceholderText.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveLocation_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog { Filter = "Icon File|*.ico", FileName = "icon.ico" };
            if (dlg.ShowDialog() == true)
            {
                SavePathTxt.Text = dlg.FileName;
            }
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedImagePath) || string.IsNullOrEmpty(SavePathTxt.Text))
            {
                Growl.Warning(Application.Current.TryFindResource("PleaseSelectPaths")?.ToString() ?? "Select paths first");
                return;
            }

            try
            {
                using (Bitmap bmp = new Bitmap(selectedImagePath))
                {
                    using (FileStream fs = new FileStream(SavePathTxt.Text, FileMode.Create))
                    {
                        int[] sizes = { 16, 32, 48, 64, 128, 256 };

                        fs.WriteByte(0); fs.WriteByte(0);
                        fs.WriteByte(1); fs.WriteByte(0);
                        fs.WriteByte((byte)sizes.Length); fs.WriteByte(0);

                        long iconDataOffset = 6 + (16 * sizes.Length);
                        byte[][] iconImages = new byte[sizes.Length][];

                        for (int i = 0; i < sizes.Length; i++)
                        {
                            using (Bitmap resizedBmp = ResizeBitmap(bmp, sizes[i], sizes[i]))
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    resizedBmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                    iconImages[i] = ms.ToArray();
                                }
                            }

                            fs.WriteByte((byte)sizes[i]);
                            fs.WriteByte((byte)sizes[i]);
                            fs.WriteByte(0);
                            fs.WriteByte(0);
                            fs.WriteByte(1); fs.WriteByte(0);
                            fs.WriteByte(32); fs.WriteByte(0);

                            byte[] sizeBytes = BitConverter.GetBytes(iconImages[i].Length);
                            fs.Write(sizeBytes, 0, 4);

                            byte[] offsetBytes = BitConverter.GetBytes((int)iconDataOffset);
                            fs.Write(offsetBytes, 0, 4);

                            iconDataOffset += iconImages[i].Length;
                        }

                        for (int i = 0; i < sizes.Length; i++)
                        {
                            fs.Write(iconImages[i], 0, iconImages[i].Length);
                        }
                    }
                }
                Growl.Success(Application.Current.TryFindResource("IconSaved")?.ToString() ?? "Icon Saved!");
                this.Close();
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message);
            }
        }

        private Bitmap ResizeBitmap(Bitmap source, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(source, 0, 0, width, height);
            }
            return result;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}