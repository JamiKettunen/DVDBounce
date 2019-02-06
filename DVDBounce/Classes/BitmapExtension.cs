using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DVDBounce
{
    internal static class ExtBitmap
    {
        private static float redShade;
        private static float greenShade;
        private static float blueShade;

        private static float red;
        private static float green;
        private static float blue;

        internal static Bitmap ColorShade(this Bitmap sourceBitmap, Color c)
        {
            try
            {
                redShade = c.R / 255f;
                greenShade = c.G / 255f;
                blueShade = c.B / 255f;

                BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                        sourceBitmap.Width, sourceBitmap.Height),
                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

                Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

                sourceBitmap.UnlockBits(sourceData);

                blue = 0;
                green = 0;
                red = 0;

                for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
                {
                    blue = pixelBuffer[k] * blueShade;
                    green = pixelBuffer[k + 1] * greenShade;
                    red = pixelBuffer[k + 2] * redShade;

                    if (blue < 0)
                    { blue = 0; }

                    if (green < 0)
                    { green = 0; }

                    if (red < 0)
                    { red = 0; }

                    pixelBuffer[k] = (byte)blue;
                    pixelBuffer[k + 1] = (byte)green;
                    pixelBuffer[k + 2] = (byte)red;

                }

                Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

                BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                        resultBitmap.Width, resultBitmap.Height),
                                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
                resultBitmap.UnlockBits(resultData);

                return resultBitmap;
            }
            catch { return null; }
        }
    }
}
