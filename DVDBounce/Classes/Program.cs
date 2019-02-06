using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

// Project inspiration: https://www.codeproject.com/Articles/856020/Draw-behind-Desktop-Icons-in-Windows

namespace DVDBounce
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region Preparations to place a Form below desktop icons

            // Fetch the Progman window
            IntPtr progman = W32.FindWindow("Progman", null);
            
            // Send 0x052C to Progman. This message directs Progman to spawn a 
            // WorkerW behind the desktop icons. If it is already there, nothing 
            // happens.
            W32.SendMessageTimeout(progman, 0x052C, new IntPtr(0), IntPtr.Zero, W32.SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out IntPtr result);

            // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
            // as a child. 
            // If we found that window, we take its next sibling and assign it to workerw.
            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                // Gets the WorkerW Window after the current one.
                if (W32.FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", IntPtr.Zero) != IntPtr.Zero)
                    Config.WorkerW = W32.FindWindowEx(IntPtr.Zero, tophandle, "WorkerW", IntPtr.Zero);
                return true;
            }), IntPtr.Zero);

            #endregion

            #region Start drawing...

            // Get the Device Context of the WorkerW
            /*IntPtr dc = W32.GetDCEx(Config.WorkerW, IntPtr.Zero, (W32.DeviceContextValues)0x403);
            if (dc == IntPtr.Zero) Environment.Exit(1);

            Font f = new Font("Segoe UI Semilight", 12f, FontStyle.Regular);
            SolidBrush sb = new SolidBrush(Color.FromArgb(255, 255, 255));
            Color bgColor = Color.FromArgb(8, 8, 8);
            System.Timers.Timer paintTimer = new System.Timers.Timer();
            Graphics g;
            g = Graphics.FromHdc(dc);
            paintTimer.Elapsed += (s, e) =>
            {
                // Draw graphics between icons and wallpaper
                // TODO On battery saving adjust g.CompositingQuality & g.CompositingMode

                //g.CompositingMode = CompositingMode.SourceOver; // Allows for alpha in Colors
                //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.
                //g.Clear(bgColor);

                g.FillRectangle(sb, 8, 8, 200, 150);
                g.DrawString("Drawn under icons!", f, Brushes.Black, 16, 16);
            };
            paintTimer.Interval = 1000 / 30;
            paintTimer.Start();*/

            // make sure to release the device context after use.
            //W32.ReleaseDC(Config.WorkerW, dc);

            //Environment.Exit(0);

            #endregion

            //Application.Run(new Form());
            Application.Run(new MainForm());

        }
    }
}
