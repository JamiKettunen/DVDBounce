using System.Windows.Forms;

namespace DVDBounce
{
    public partial class ConfigForm : Form
    {
        /// <summary>
        /// Paint child controls on top of background (no white rectangle placeholder for Controls, flickering reduction).
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~0x02000000; // Turn off WS_CLIPCHILDREN
                return cp;
            }
        }

        public ConfigForm()
        {
            // Increase painting speed etc
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, System.EventArgs e)
        {

        }
    }
}
