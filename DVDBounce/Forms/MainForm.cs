using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using DVDBounce.Properties;

namespace DVDBounce
{
    public partial class MainForm : Form
    {
        // TODO Add multiple DVD support (instead of only 1)

        // TODO Add up to 5 colors to fade to (custom rainbow effect)

        #region DVD properties
        
        #region Internal
        
        private int left = 0;
        private int top = 0;
        private byte dir = 1; // 1 = TopLeft, 2 = TopRight, 3 = BottomLeft, 4 = BottomRight

        private bool _useRainbowColor = false;
        private double _rainbowProgress = 0;
        private int _rainbowSpeed = 25;

        private static double rainbowDiv;
        private static int rainbowAscending;
        private static int rainbowDescending;

        #endregion

        #region DVD Color related

        #region Rainbow color (Enable, Progress, Speed, ...)

        private System.Timers.Timer rainbowTimer = new System.Timers.Timer();
        private double RainbowCycleStep = 0.01;

        private void RainbowTimer_IncrementCycle(object sender, System.Timers.ElapsedEventArgs e)
        {
            RainbowProgress += RainbowCycleStep;
        }

        /// <summary>
        /// Update whether to use Rainbow DVD color
        /// </summary>
        private bool UseRainbowColor
        {
            get => _useRainbowColor;
            set
            {
                _useRainbowColor = value;

                rainbowTimer.Enabled = _useRainbowColor;
                if (!_useRainbowColor) _rainbowProgress = 0;
            }
        }
        
        private static Color Rainbow(double progress)
        {
            rainbowDiv = (Math.Abs(progress % 1) * 6);
            rainbowAscending = (int)((rainbowDiv % 1) * 255);
            rainbowDescending = 255 - rainbowAscending;

            switch ((int)rainbowDiv)
            {
                case 0:
                    return Color.FromArgb(255, 255, rainbowAscending, 0);
                case 1:
                    return Color.FromArgb(255, rainbowDescending, 255, 0);
                case 2:
                    return Color.FromArgb(255, 0, 255, rainbowAscending);
                case 3:
                    return Color.FromArgb(255, 0, rainbowDescending, 255);
                case 4:
                    return Color.FromArgb(255, rainbowAscending, 0, 255);
                default: // case 5:
                    return Color.FromArgb(255, 255, 0, rainbowDescending);
            }
        }

        /// <summary>
        /// Get or update the current Rainbow color cycle progress (0.0-1.0)
        /// </summary>
        private double RainbowProgress
        {
            get => _rainbowProgress;
            set
            {
                if (value < 0) return;
                else if (value > 1) _rainbowProgress = value - (int)value; // Remove full numbers (eg. 1.2 - 1 = 0.2) |Math.Round(x, 2)
                else _rainbowProgress = value; //Math.Round(value, 2);
                Config.DVDColor = Rainbow(_rainbowProgress);
            }
        }

        /// <summary>
        /// Update Rainbow color cycle speed
        /// </summary>
        private int RainbowSpeed
        {
            get => _rainbowSpeed;
            set
            {
                if (value < 1 || value > 1000) return; // hard-limit

                _rainbowSpeed = value;
                rainbowTimer.Interval = (1000 / _rainbowSpeed);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Should the DVD always start in the middle of the Window?
        /// </summary>
        private bool StartInMiddle = false;

        /// <summary>
        /// DVD pixel movement amount in X & Y-axis on each animation frame
        /// </summary>
        private int Speed = 8; // 4 | TODO Differenciate speeds to speedx & speedy (deltax & deltay calculation logic on side hits)

        #endregion

        #region DVD methods



        #endregion

        #region Other properties

        private Random rng = new Random();
        private Rectangle screenRect;
        private ulong sideHits = 0;
        private ulong cornerHits = 0;

        /// <summary>
        /// Imprecise corner threshold pixel count
        /// </summary>
        private byte ImpreciseCornerPixels = 2; // def. 1 for perfect corner hits only

        private int _fps = 144;
        /// <summary>
        /// Times / second to redraw the Form
        /// </summary>
        private int FPS
        {
            get => _fps;
            set
            {
                _fps = value;
                timerUpdate.Interval = (int)Math.Round(1000m / _fps, 0);
                //Debug.WriteLine("FPS: " + _fps + "; Interval: " + timerUpdate.Interval);
            }
        }

        #endregion

        #region MainForm & load init

        /// <summary>
        /// Paint child controls on top of background (no white rectangle placeholder for Controls, flickering reduction).
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~0x02000000; // Turn off WS_CLIPCHILDREN
                cp.ExStyle |= 0x80; // Turn on WS_EX_TOOLWINDOW (Removes program from ALT + TAB view & puts to "Background processes" list on TaskMgr)
                return cp;
            }
        }

        public MainForm()
        {
            // Increase painting speed etc
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            rainbowTimer.Elapsed += RainbowTimer_IncrementCycle;

            InitializeComponent();

            Config.MainForm = this;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Shared.MainForm = this;

            // Put the Form behind desktop icons & above the wallpaper
            W32.SetParent(this.Handle, Config.WorkerW);
        
            MoveToScreen(Cursor.Position);
            
            Config.DVDImg = Resources.dvd;
            Config.UseDesktopWallpaper = true;
            RainbowSpeed = _rainbowSpeed; // Use default speed (for now)
            UseRainbowColor = true;
            FPS = 60;

            Init();
            timerUpdate.Start();
        }

        #endregion

        #region DVD icon painting

        private StringFormat centerFormat;

        private void PreparePaint()
        {
            centerFormat = new StringFormat();
            centerFormat.Alignment = StringAlignment.Center;
            centerFormat.LineAlignment = StringAlignment.Center;

            //timeRect = new Rectangle(0, (int)(screenRect.Height / 100d * 20d), screenRect.Width, screenRect.Height);
        }

        //Rectangle timeRect;
        private string strHitCounts;

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // TODO Draw background / Image here
            //e.Graphics.Clear(Config.BackColor);

            // Draw hit counters
            strHitCounts = $"Corner Hits: {cornerHits}\nSide Hits: {sideHits}";
            e.Graphics.DrawString(strHitCounts, new Font("Segoe UI Semilight", 14f, FontStyle.Regular), Brushes.Gray, screenRect, centerFormat);

            /*if(timeRect != null)
                e.Graphics.DrawString(DateTime.Now.ToString("H:mm | dd.MM.yyyy"), new Font("Segoe UI Semilight", 24f, FontStyle.Regular), Brushes.WhiteSmoke, timeRect, centerFormat);*/

            // Draw collision box around DVD
            //e.Graphics.DrawRectangle(Pens.Blue, left, top, _dvdImg.Width, _dvdImg.Height);

            // Draw the DVD
            e.Graphics.DrawImage(Config.DVDImg, left, top);
        }

        #endregion

        #region Key bindings

        private bool fullscreen = false;
        private FormWindowState lastState;
        private Point lastPos;
        private Size lastSize;
        
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset
            if (e.KeyCode == Keys.F5)
            {
                Init(); // Re-initialize
            }
            // Toggle fullscreen
            /*else if (e.KeyCode == Keys.F11)
            {
                if (!fullscreen)
                {
                    if(this.WindowState == FormWindowState.Normal)
                    {
                        lastPos = this.Location;
                        lastSize = this.Size;
                    }

                    Cursor.Hide();
                    fullscreen = true;
                    lastState = this.WindowState;
                    this.Hide();
                    this.WindowState = FormWindowState.Normal;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.Location = Screen.GetWorkingArea(Cursor.Position).Location; // GetBounds ...
                    this.Size = Screen.GetWorkingArea(Cursor.Position).Size; // GetBounds ...
                    this.Show();

                    // TODO Relocate DVD image to match last position on scale 
                    //Debug.WriteLine("Before: " + left + "," + top);

                    // Fix location scale...
                    double tx = ((double)left / (double)this.Width) + 1;
                    left = (int)(left * tx);
                    double ty = ((double)top / (double)this.Height) + 1;
                    top = (int)(top * ty);

                    //Debug.WriteLine("After: " + left + "," + top);
                    //Debug.WriteLine("Scale: " + tx + "x" + ty);

                    // TODO Show bounce info on fullscreen
                }
                else
                {
                    this.Size = lastSize;

                    // TODO Relocate DVD image to match last position on scale 

                    this.Location = lastPos;
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.WindowState = lastState;
                    Cursor.Show();
                    fullscreen = false;
                    //this.Size = lastSize;
                    //this.Location = lastPos;
                }
            }*/
        }

        #endregion

        /// <summary>
        /// Moves the Form to the Screen closest to the specified point 
        /// </summary>
        /// <param name="pointInScreen"></param>
        private void MoveToScreen(Point pointInScreen)
        {
            screenRect = Screen.GetWorkingArea(Cursor.Position);
            this.Location = screenRect.Location;
            this.Size = screenRect.Size;
            UpdateMaxSizes();
            PreparePaint();
        }

        private void Init(Point startPos = new Point(), int startDir = -1)
        {
            // Reset hits
            sideHits = 0;
            cornerHits = 0;

            // Start going in a random direction
            dir = (byte)((startDir > 0 && startDir < 5) ? startDir : rng.Next(1, 5));

            // Initialize starting location
            if(startPos.IsEmpty)
            {
                if (!StartInMiddle)
                {
                    // Pick random starting location
                    left = rng.Next(0, (this.ClientSize.Width - Config.DVDImg.Width) + 1);
                    top = rng.Next(0, (this.ClientSize.Height - Config.DVDImg.Height) + 1);
                }
                else
                {
                    // Start in the middle of the Form
                    left = (this.ClientSize.Width / 2) - (Config.DVDImg.Width / 2);
                    top = (this.ClientSize.Height / 2) - (Config.DVDImg.Height / 2);
                }
            }
            else
            {
                left = startPos.X;
                top = startPos.Y;
            }

            // Other
            _rainbowProgress = 0;
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            // Move the DVD
            left += (dir % 2 != 0 ? -Speed : Speed);
            top += (dir < 3 ? -Speed : Speed);

            // Do some collision detection & bounce (if outside bounds)
            DoCollisions();

            // Show updated frame
            this.Refresh();
        }

        #region Collision detection

        private int maxW = 0;
        private int maxH = 0;

        internal void UpdateMaxSizes()
        {
            try
            {
                maxW = (this.ClientRectangle.Width - Config.DVDImg.Width - 1);
                maxH = (this.ClientRectangle.Height - Config.DVDImg.Height - 1);
            }
            catch { maxW = 0; maxH = 0; }
        }

        private bool cornerHit = false;
        private bool lastCornerImprecise = false;
        private byte lastDir = 0;

        private void DoCollisions()
        {
            // Outside some bounds => Bounce
            if (left < 1 || top < 1 || left > maxW || top > maxH)
            {
                //Debug.WriteLine("Outside bounds => Bounce");

                cornerHit = false;
                lastDir = dir;

                #region Perfect corner collisions

                // Top-Left corner hit
                if (left < 1 && top < 1)
                {
                    dir = 4;
                    cornerHit = true;
                }
                // Top-Right corner hit
                else if (left > maxW && top < 1)
                {
                    dir = 3;
                    cornerHit = true;
                }
                // Bottom-Left corner hit
                else if (left < 1 && top > maxH)
                {
                    dir = 2;
                    cornerHit = true;
                }
                // Bottom-Right corner hit
                else if (left > maxW && top > maxH)
                {
                    dir = 1;
                    cornerHit = true;
                }

                #endregion

                #region Side collisions

                // TODO Only update hits IF dir changed

                // Left side hit
                else if (left < 1)
                    dir = (byte)(dir == 1 ? 2 : dir == 3 ? 4 : dir);
                // Top side hit
                else if (top < 1)
                    dir = (byte)(dir == 2 ? 4 : dir == 1 ? 3 : dir);
                // Right side hit
                else if (left > maxW)
                    dir = (byte)(dir == 4 ? 3 : dir == 2 ? 1 : dir);
                // Bottom side hit
                else if (top > maxH)
                    dir = (byte)(dir == 4 ? 2 : dir == 3 ? 1 : dir);

                // Check for imprecise corner hits on SIDES
                if(!cornerHit && ImpreciseCornerPixels > 1)
                {
                    //Debug.WriteLine("Left: " + left + ", Top: " + top + ", Threshold: " + cornerThreshold + ", LastCornerImprecise: " + lastCornerImprecise);

                    if (!lastCornerImprecise) // Last side WAS NOT an imprecise corner hit
                    {
                        //Debug.WriteLine("Checking non-precise corner... (chiter knob)");

                        // Top-Left corner
                        if (left - ImpreciseCornerPixels < 1 && top - ImpreciseCornerPixels < 1)
                            cornerHit = true;
                        // Top-Right corner
                        else if (left + ImpreciseCornerPixels > maxW && top - ImpreciseCornerPixels < 1)
                            cornerHit = true;
                        // Bottom-Left corner
                        else if (left - ImpreciseCornerPixels < 1 && top + ImpreciseCornerPixels > maxH)
                            cornerHit = true;
                        // Bottom-Right corner
                        else if (left + ImpreciseCornerPixels > maxW && top + ImpreciseCornerPixels > maxH)
                            cornerHit = true;

                        lastCornerImprecise = cornerHit;
                    }
                    else lastCornerImprecise = false; // Last side WAS an imprecise corner hit
                }

                if(dir != lastDir)
                {
                    // Update hit amounts
                    if (!cornerHit) sideHits++;
                    else cornerHits++;

                    // Update values shown in UI
                    //UpdateHits();
                }
                //else { Debug.WriteLine("No collision? 🤔"); }

                #endregion
            }
        }

        #endregion

        #region Tray icon

        private void trayQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion
    }
}
