namespace DVDBounce
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.trayConfigure = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuSeperator = new System.Windows.Forms.ToolStripSeparator();
            this.trayQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 17;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.trayMenu;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "DVD Bounce";
            this.trayIcon.Visible = true;
            // 
            // trayMenu
            // 
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayConfigure,
            this.trayMenuSeperator,
            this.trayQuit});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.Size = new System.Drawing.Size(128, 54);
            // 
            // trayConfigure
            // 
            this.trayConfigure.Name = "trayConfigure";
            this.trayConfigure.Size = new System.Drawing.Size(127, 22);
            this.trayConfigure.Text = "&Configure";
            // 
            // trayMenuSeperator
            // 
            this.trayMenuSeperator.Name = "trayMenuSeperator";
            this.trayMenuSeperator.Size = new System.Drawing.Size(124, 6);
            // 
            // trayQuit
            // 
            this.trayQuit.Name = "trayQuit";
            this.trayQuit.Size = new System.Drawing.Size(127, 22);
            this.trayQuit.Text = "&Quit";
            this.trayQuit.Click += new System.EventHandler(this.trayQuit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(8)))), ((int)(((byte)(8)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(960, 540);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "DVD Bounce | Click for options";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.trayMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem trayConfigure;
        private System.Windows.Forms.ToolStripSeparator trayMenuSeperator;
        private System.Windows.Forms.ToolStripMenuItem trayQuit;
        internal System.Windows.Forms.Timer timerUpdate;
    }
}

