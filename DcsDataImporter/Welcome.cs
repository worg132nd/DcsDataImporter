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
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            var form = new frmUserInput();
            this.Hide();
            form.StartPosition = this.StartPosition;
            form.Show();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var form = new FormSettings();
            this.Hide();
            form.StartPosition = this.StartPosition;
            form.ShowDialog();
            this.Show();
        }
    }
}
