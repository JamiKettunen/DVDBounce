using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DVDBounce.Properties;
using Microsoft.Win32;

namespace DVDBounce
{
    internal static class Config
    {
        internal static IntPtr WorkerW = IntPtr.Zero;

        internal static MainForm MainForm = null;

        #region Images

        // TODO Add support for animated images (switch baseImg & do UpdateDVDImg(x) when switched, timer triggered, custom interval, List<Image> ... )

        private static Bitmap _dvdBaseImg = null;
        internal static Bitmap DVDBaseImg
        {
            get
            {
                if (_dvdBaseImg == null)
                {
                    if (!string.IsNullOrEmpty(Settings.Default.LastDVDImage))
                        try { _dvdBaseImg = Base64ToBmp(Settings.Default.LastDVDImage); } catch { }
                        
                    if(_dvdBaseImg == null)
                        _dvdBaseImg = Resources.dvd;
                }
                return _dvdBaseImg;
            }
            set
            {
                _dvdBaseImg = value;
                
                // Update config
                Settings.Default.LastDVDImage = (Resources.dvd.GetHashCode() != _dvdBaseImg.GetHashCode() ? BmpToBase64(_dvdBaseImg) : "");
                Settings.Default.Save();
            }
        }

        private static Bitmap _dvdImg;
        internal static Bitmap DVDImg
        {
            get => _dvdImg;
            set
            {
                DVDBaseImg = value;
                UpdateDVDColor(_dvdColor);
                MainForm.UpdateMaxSizes();
            }
        }

        #region Background

        private static bool _useDesktopWallpaper = false;
        internal static bool UseDesktopWallpaper
        {
            get => _useDesktopWallpaper;
            set
            {
                _useDesktopWallpaper = value;

                /*if(_useDesktopWallpaper)
                    BackImage = GetWallpaper();*/

                // Update config
                Settings.Default.UseDesktopWallpaper = _useDesktopWallpaper;
                Settings.Default.Save();
            }
        }   

        internal static Image BackImage
        {
            get
            {
                if (MainForm.BackgroundImage == null)
                {
                    /*if (_useDesktopWallpaper)
                        BackImage = GetWallpaper();*/
                    
                    /*if (!string.IsNullOrEmpty(Settings.Default.LastBgImage))
                        try { MainForm.BackgroundImage = Base64ToBmp(Settings.Default.LastBgImage); } catch { }*/

                }
                return MainForm.BackgroundImage;
            }
            set
            {
                MainForm.BackgroundImage = value;

                // Update config
                Settings.Default.LastBgImagePath = (value != null ? BmpToBase64((Bitmap)value) : "");
                Settings.Default.Save();
            }
        }

        internal static ImageLayout BackImageLayout
        {
            get => MainForm.BackgroundImageLayout;
            set => MainForm.BackgroundImageLayout = value;
        }

        #endregion

        #endregion

        #region Colors

        private static Color _dvdColor = Color.WhiteSmoke;
        /// <summary>
        /// Update color of DVD image (must be White by default)
        /// </summary>
        internal static Color DVDColor
        {
            get => _dvdColor;
            set
            {
                _dvdColor = value;

                // Update displayed color
                UpdateDVDColor(_dvdColor);
            }
        }

        internal static Color BackColor
        {
            get => MainForm.BackColor;
            set
            {
                MainForm.BackColor = value;

                // Update config
                Settings.Default.BackColor = value;
                Settings.Default.Save();
            }
        }
        
        internal static void UpdateDVDColor(Color newColor)
        {
            using (Bitmap bmp = DVDBaseImg.ColorShade(newColor))
            {
                if (bmp != null)
                    _dvdImg = new Bitmap(bmp);
            }
        }

        #endregion

        #region BASE64 Image encoding & decoding

        private static Bitmap Base64ToBmp(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imgBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imgBytes, 0, imgBytes.Length))
            {
                imgBytes = null;
                return (Bitmap)Image.FromStream(ms, true);
            }
        }

        private static string BmpToBase64(Bitmap bmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                bmp.Save(ms, bmp.RawFormat);
                // Convert byte[] to base 64 string
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        #endregion

        #region Other

        private static Image ReadImage(string path)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
                    return Image.FromStream(ms);
            }
            catch { return null; }
        }

        private static string GetWallpaperPath()
        {
            byte[] path = (byte[])Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop").GetValue("TranscodedImageCache");
            Buffer.BlockCopy(path, 24, path, 0, path.Length - 24); // Offset by 24 bytes
            return Encoding.Unicode.GetString(path).TrimEnd('\0');
        }

        #endregion
    }
}
