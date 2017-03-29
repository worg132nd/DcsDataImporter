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
    public partial class FormSupplyPaths : Form
    {
        public FormSupplyPaths()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ".docm|*.docm";
            ofd.Title = "Select the file path for the supplied Word (.docm) file(s)";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtWordFile.Text = ofd.SafeFileName; // only file name
                Properties.Settings.Default.pathDocmFile = ofd.FileName; // path
                Properties.Settings.Default.pathDocmFile += ofd.SafeFileName; // only file name
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ".*|*.*";
            ofd.Title = "Select path to the install directory of the Kneeboard Builder application";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtKneeboardPath.Text = ofd.FileName; // path
                Properties.Settings.Default.pathKneeboardBuilder = ofd.FileName; // path
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
