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
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            this.CenterToParent();
            InitializeComponent();
            txtKneeboardPath.Text = Properties.Settings.Default.pathKneeboardBuilder;
            txtWordFile.Text = Properties.Settings.Default.filePathCommunicationNoAwacs;

            chkCommunicationHelp.Enabled = false;

            if (Properties.Settings.Default.CommunicationHelp)
            {
                chkCommunicationHelp.Checked = true;
            }

            if (txtKneeboardPath.Text != "" && txtWordFile.Text != "")
            {
                chkCommunicationHelp.Enabled = true;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ".docm|*.docm";
            ofd.Title = "Select the file path for the supplied file CommunicationNoAwacs.docm";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtWordFile.Text = ofd.FileName; // only file name

                if (txtKneeboardPath.Text != "")
                {
                    chkCommunicationHelp.Enabled = true;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ".exe|*.exe";
            ofd.Title = "Select path to the install directory of the Kneeboard Builder application by selecting the exe file";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtKneeboardPath.Text = ofd.FileName; // path

                if (txtWordFile.Text != "")
                {
                    chkCommunicationHelp.Enabled = true;
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (txtKneeboardPath.Text != "")
            {
                Properties.Settings.Default.pathKneeboardBuilder = txtKneeboardPath.Text; // path
            }

            if (txtWordFile.Text != "")
            {
                Properties.Settings.Default.filePathCommunicationNoAwacs = txtWordFile.Text;
            }

            if (chkCommunicationHelp.Checked)
            {
                var form = new FormPopup();
                this.Hide();
                form.ShowDialog();
                this.Close();
            }

            if (!chkCommunicationHelp.Checked)
            {
                Properties.Settings.Default.CommunicationHelp = false;
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
