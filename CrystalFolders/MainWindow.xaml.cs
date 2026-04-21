using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CrystalFolders
{
    public partial class MainWindow : System.Windows.Window
    {
        // ================== FIELDS & PROPERTIES ==================
        public static string icoPath;
        public static bool isPortable = false, isRestore = false;
        public static ObservableCollection<string> folderList;
        public static List<string> subfolderList;

        // ================== WIN32 API CALLS ==================
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("shell32.dll", EntryPoint = "#727")]
        public static extern int SHGetImageList(int iImageList, ref Guid riid, out IImageList ppv);

        [ComImport]
        [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IImageList
        {
            [PreserveSig] int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);
            [PreserveSig] int ReplaceIcon(int i, IntPtr hicon, ref int pi);
            [PreserveSig] int SetOverlayImage(int iImage, int iOverlay);
            [PreserveSig] int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);
            [PreserveSig] int AddMasked(IntPtr hbmImage, int crMask, ref int pi);
            [PreserveSig] int Draw(ref IMAGELISTDRAWPARAMS pimldp);
            [PreserveSig] int Remove(int i);
            [PreserveSig] int GetIcon(int i, int flags, out IntPtr picon);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGELISTDRAWPARAMS
        {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x, y, cx, cy, xBitmap, yBitmap, rgbBk, rgbFg, fStyle, dwRop, fState, Frame, crEffect;
        }

        const int SHIL_JUMBO = 0x4;
        const int SHIL_EXTRALARGE = 0x2;
        const uint SHGFI_SYSICONINDEX = 0x4000;

        // ================== HELPERS ==================
        private string GetStr(string key)
        {
            return Application.Current.TryFindResource(key)?.ToString() ?? key;
        }

        // ================== INITIALIZATION ==================
        public MainWindow()
        {
            InitializeComponent();
            folderList = new ObservableCollection<string>();
            subfolderList = new List<string>();
            DropList.ItemsSource = folderList;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.FlowDirection = (Config.currentLan == "ar") ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            UpdateThemeIcon();
        }

        // ================== THEME MANAGEMENT ==================
        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            Config.isDarkMode = !Config.isDarkMode;
            Config.ApplyTheme();
            UpdateThemeIcon();
        }

        private void UpdateThemeIcon()
        {
            SunIcon.Visibility = Config.isDarkMode ? Visibility.Collapsed : Visibility.Visible;
            MoonIcon.Visibility = Config.isDarkMode ? Visibility.Visible : Visibility.Collapsed;
        }

        // ================== CORE UTILITIES ==================
        private void NCount()
        {
            Dot.Text = folderList.Count.ToString();
            Dotsub.Text = subfolderList.Count.ToString();
        }

        private void Clear()
        {
            folderList.Clear();
            subfolderList.Clear();
            icoPath = null;
            isRestore = false;
            Iconpic.Source = null;
            SlidePortable.IsChecked = false;
            SlideSub.IsChecked = false;

            ResetIconPreview();

            RestoreBtn.SetResourceReference(ContentProperty, "Restore");
            Customize.SetResourceReference(ContentProperty, "ApplyCustomization");

            // ≈⁄«œ…  ›⁄Ì· «·⁄‰«’— ⁄‰œ «· ’›Ì—
            ChooseBtn.IsEnabled = true;
            SlideSub.IsEnabled = true;
            SlidePortable.IsEnabled = true;

            IconPreviewArea.Cursor = Cursors.Hand;

            NCount();
        }

        private void AddSubFolders()
        {
            subfolderList.Clear();
            foreach (string path in folderList)
            {
                try
                {
                    if (Directory.Exists(path))
                        subfolderList.AddRange(Directory.GetDirectories(path));
                }
                catch { }
            }
            NCount();
        }

        // ================== UI ACTIONS ==================
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (folderList.Count == 0 && subfolderList.Count == 0 && Iconpic.Source == null && isRestore == false)
            {
                Growl.InfoGlobal(GetStr("AppAlreadyInitialState"));
                return;
            }
            Clear();
            Growl.SuccessGlobal(GetStr("AppRefreshed"));
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserEx.FolderBrowserDialog
            {
                Title = GetStr("SelectFolders"),
                AllowMultiSelect = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string path in dialog.SelectedFolders)
                {
                    if (!folderList.Contains(path)) folderList.Add(path);
                }
                if (SlideSub.IsChecked == true) AddSubFolders();
                NCount();
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (folderList.Count == 0)
            {
                Growl.WarningGlobal(GetStr("ListAlreadyEmpty"));
                return;
            }

            if (DropList.SelectedItems.Count > 0)
            {
                var selected = DropList.SelectedItems.Cast<string>().ToList();
                foreach (string s in selected) folderList.Remove(s);

                if (SlideSub.IsChecked == true) AddSubFolders();
                else NCount();

                if (DropList.SelectedItem == null) ResetIconPreview();
                Growl.SuccessGlobal(GetStr("ItemsRemoved"));
            }
            else Growl.WarningGlobal(GetStr("PleaseSelectItems"));
        }

        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            if (folderList.Count == 0)
            {
                Growl.WarningGlobal(GetStr("ListAlreadyEmpty"));
                return;
            }

            folderList.Clear();
            subfolderList.Clear();
            NCount();
            ResetIconPreview();
            Growl.InfoGlobal(GetStr("FoldersListCleared"));
        }

        // ================== ICON LOGIC ==================
        private void ChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isRestore) return;

            OpenFileDialog dlg = new OpenFileDialog { Filter = $"{GetStr("IconFiles")} (*.ico)|*.ico" };

            if (dlg.ShowDialog() == true)
            {
                icoPath = dlg.FileName;
                try
                {
                    using (FileStream fs = new FileStream(icoPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        IconBitmapDecoder decoder = new IconBitmapDecoder(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                        var bestFrame = decoder.Frames.OrderByDescending(f => f.PixelWidth * f.PixelHeight).FirstOrDefault();
                        if (bestFrame != null)
                        {
                            if (bestFrame.CanFreeze) bestFrame.Freeze();
                            Iconpic.Source = bestFrame;
                        }
                    }
                    Icon_border.Visibility = Icon_cross.Visibility = Visibility.Collapsed;
                }
                catch { Growl.ErrorGlobal(GetStr("InvalidIconFile")); }
            }
        }

        private void RestoreBtn_Click(object sender, RoutedEventArgs e)
        {
            isRestore = !isRestore;
            if (isRestore)
            {
                icoPath = null;
                ResetIconPreview();
                RestoreBtn.SetResourceReference(ContentProperty, "NormalMode");
                Customize.SetResourceReference(ContentProperty, "RestoreToDefault");
                IconPreviewArea.Cursor = Cursors.Arrow;

                //  ⁄ÿÌ· «·√“—«— ⁄‰œ «·œŒÊ· ›Ì Ê÷⁄ «·—Ì” Ê—
                ChooseBtn.IsEnabled = false;
                SlideSub.IsEnabled = false;
                SlidePortable.IsEnabled = false;

                UpdateRestorePreview();
                Growl.InfoGlobal(GetStr("RestoreModeActive"));
            }
            else
            {
                Growl.SuccessGlobal(GetStr("NormalModeActive"));
                RestoreBtn.SetResourceReference(ContentProperty, "Restore");
                Customize.SetResourceReference(ContentProperty, "ApplyCustomization");
                IconPreviewArea.Cursor = Cursors.Hand;

                //  ›⁄Ì· «·√“—«— ⁄‰œ «·⁄Êœ… ··Ê÷⁄ «·⁄«œÌ
                ChooseBtn.IsEnabled = true;
                SlideSub.IsEnabled = true;
                SlidePortable.IsEnabled = true;

                ResetIconPreview();
            }
        }

        // œ«·… ·› Õ „ÕÊ· «·√ÌﬁÊ‰« 
        private void OpenConverter_Click(object sender, RoutedEventArgs e)
        {
            new IconConverterWindow { Owner = this }.ShowDialog();
        }

        // ================== CUSTOMIZATION ENGINE ==================
        private async void Customize_Click(object sender, RoutedEventArgs e)
        {
            // «· Õﬁﬁ «·√Ê·Ì
            if (icoPath == null && !isRestore) { Growl.WarningGlobal(GetStr("ChooseAnIconFirst")); return; }
            if (folderList.Count == 0) { Growl.WarningGlobal(GetStr("ListAlreadyEmpty")); return; }

            // 1. ≈‰‘«¡ «·‰«›–… Ê ÕœÌœ «·„«·ﬂ (·Ã⁄·Â«  ŸÂ— ›Êﬁ «·‹ MainWindow)
            var waitDialog = new WaitDialog { Owner = this };

            try
            {
                // 2. ≈ŸÂ«— «·‰«›–… Ê œﬁÌﬁ  ›⁄Ì· «·“—
                Customize.IsEnabled = false;
                waitDialog.Show();

                List<string> allPaths = new List<string>(folderList);
                if (SlideSub.IsChecked == true) allPaths.AddRange(subfolderList);

                // 3. «·»œ¡ ›Ì «·„Â„… «·Œ·›Ì…
                await Task.Run(() =>
                {
                    foreach (string path in allPaths)
                    {
                        try
                        {
                            if (!Directory.Exists(path)) continue;

                            string iniPath = Path.Combine(path, "desktop.ini");
                            string portIco = Path.Combine(path, "folder_icon.ico");

                            if (File.Exists(iniPath))
                            {
                                File.SetAttributes(iniPath, FileAttributes.Normal);
                                File.Delete(iniPath);
                            }

                            if (isRestore)
                            {
                                if (File.Exists(portIco)) { File.SetAttributes(portIco, FileAttributes.Normal); File.Delete(portIco); }
                                File.SetAttributes(path, FileAttributes.Normal);
                            }
                            else
                            {
                                string finalIco = icoPath;
                                if (isPortable)
                                {
                                    File.Copy(icoPath, portIco, true);
                                    File.SetAttributes(portIco, FileAttributes.Hidden | FileAttributes.System);
                                    finalIco = "folder_icon.ico";
                                }

                                var ini = new StringBuilder();
                                ini.AppendLine("[.ShellClassInfo]");
                                ini.AppendLine($"IconResource={finalIco},0");
                                File.WriteAllText(iniPath, ini.ToString(), Encoding.Unicode);
                                File.SetAttributes(iniPath, FileAttributes.Hidden | FileAttributes.System);
                                File.SetAttributes(path, FileAttributes.ReadOnly);
                            }
                            // ≈‘⁄«— «·‰Ÿ«„ »«· €ÌÌ— ·ﬂ· „Ã·œ
                            SHChangeNotify(0x00002000, 0x0001, IntPtr.Zero, IntPtr.Zero);
                        }
                        catch { /*  Ã«Â· «·√Œÿ«¡ «·»”Ìÿ… ·ﬂ· „Ã·œ */ }
                    }
                });

                // 4. ≈‘⁄«— «·‰Ÿ«„ «·‰Â«∆Ì
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
                Growl.SuccessGlobal(isRestore ? GetStr("FoldersHaveBeenRestored") : GetStr("IconsApplied"));
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"{GetStr("Error")}: {ex.Message}");
            }
            finally
            {
                // 5. ≈€·«ﬁ ‰«›–… «·«‰ Ÿ«— ›Ê—« ⁄‰œ «·«‰ Â«¡ Ê ›⁄Ì· «·“— „Ãœœ«
                waitDialog.Close();
                Customize.IsEnabled = true;
                Clear();
            }
        }

        // ================== PREVIEW & RENDER ==================
        private ImageSource GetSafeFolderIcon(string path)
        {
            IntPtr hIcon = IntPtr.Zero;
            try
            {
                string iniPath = Path.Combine(path, "desktop.ini");
                if (File.Exists(iniPath))
                {
                    try
                    {
                        var lines = File.ReadAllLines(iniPath);
                        foreach (var line in lines)
                        {
                            if (line.StartsWith("IconResource=", StringComparison.OrdinalIgnoreCase))
                            {
                                string iconPath = line.Substring("IconResource=".Length).Split(',')[0].Trim();
                                if (!Path.IsPathRooted(iconPath)) iconPath = Path.Combine(path, iconPath);
                                if (File.Exists(iconPath))
                                {
                                    using (FileStream fs = new FileStream(iconPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        IconBitmapDecoder decoder = new IconBitmapDecoder(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                                        var bestFrame = decoder.Frames.OrderByDescending(f => f.PixelWidth * f.PixelHeight).FirstOrDefault();
                                        if (bestFrame != null) { if (bestFrame.CanFreeze) bestFrame.Freeze(); return bestFrame; }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }

                SHFILEINFO shinfo = new SHFILEINFO();
                IntPtr res = SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_SYSICONINDEX);
                if (res != IntPtr.Zero)
                {
                    IImageList iml;
                    Guid iid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
                    if (SHGetImageList(SHIL_JUMBO, ref iid, out iml) == 0)
                    {
                        if (iml.GetIcon(shinfo.iIcon, 0x0001, out hIcon) == 0 && hIcon != IntPtr.Zero) return ConvertIconToBitmap(hIcon);
                    }
                }
            }
            catch { }
            return null;
        }

        private BitmapSource ConvertIconToBitmap(IntPtr hIcon)
        {
            var img = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DestroyIcon(hIcon);
            if (img.CanFreeze) img.Freeze();
            return img;
        }

        private void UpdateRestorePreview()
        {
            if (DropList.SelectedItem == null) { ResetIconPreview(); return; }
            string path = DropList.SelectedItem.ToString();
            if (!Directory.Exists(path)) { ResetIconPreview(); return; }
            var icon = GetSafeFolderIcon(path);
            if (icon != null)
            {
                Iconpic.Source = icon;
                Icon_border.Visibility = Visibility.Collapsed;
                Icon_restore.Visibility = Visibility.Collapsed;
                Icon_cross.Visibility = Visibility.Collapsed;
            }
            else ResetIconPreview();
        }

        private void ResetIconPreview()
        {
            Iconpic.Source = null;
            Icon_border.Visibility = Visibility.Visible;
            Icon_restore.Visibility = isRestore ? Visibility.Visible : Visibility.Collapsed;
            Icon_cross.Visibility = isRestore ? Visibility.Collapsed : Visibility.Visible;
        }

        // ================== EVENT HANDLERS ==================
        private void DropList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (isRestore) UpdateRestorePreview();
        }

        private void SlideSub_Checked(object sender, RoutedEventArgs e) => AddSubFolders();
        private void SlideSub_Unchecked(object sender, RoutedEventArgs e) { subfolderList.Clear(); NCount(); }
        private void SlidePortable_Click(object sender, RoutedEventArgs e) => isPortable = SlidePortable.IsChecked == true;

        private void IconPreviewArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isRestore) ChooseBtn_Click(sender, e);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void Info_Click(object sender, RoutedEventArgs e) => new About() { Owner = this }.Show();
        private void Help_Click(object sender, RoutedEventArgs e) => new HelpDialog() { Owner = this }.ShowDialog();
        private void DropList_DragEnter(object sender, DragEventArgs e) => e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;

        private void DropList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string f in files)
                    if (Directory.Exists(f) && !folderList.Contains(f))
                        folderList.Add(f);

                if (SlideSub.IsChecked == true) AddSubFolders();
                NCount();
            }
        }
    }
}