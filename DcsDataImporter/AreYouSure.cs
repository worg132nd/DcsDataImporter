using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DcsDataImporter
{
    public partial class AreYouSure : Form
    {
        public AreYouSure()
        {
            InitializeComponent();
            this.CenterToParent();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CommunicationHelp = false;
            this.Close();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CommunicationHelp = true;
            this.Close();
        }
    }
}
