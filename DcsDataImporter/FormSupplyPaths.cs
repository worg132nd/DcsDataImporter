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
            txtCommunicationNoAwacsPath.Text = Properties.Settings.Default.filePathCommunicationNoAwacs;
            txtCommunicationPath.Text = Properties.Settings.Default.filePathCommunication;
            txtCommunicationNoTmaPath.Text = Properties.Settings.Default.filePathCommunicationNoTma;

            chkCommunicationHelp.Enabled = false;

            if (Properties.Settings.Default.CommunicationHelp)
            {
                chkCommunicationHelp.Checked = true;
            }

            if (txtKneeboardPath.Text != "" && txtCommunicationNoAwacsPath.Text != "" && txtCommunicationPath.Text != "" && txtCommunicationNoTmaPath.Text != "")
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
                txtCommunicationNoAwacsPath.Text = ofd.FileName; // only file name

                if (txtKneeboardPath.Text != "" && txtCommunicationPath.Text != "" && txtCommunicationNoTmaPath.Text != "")
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

                if (txtCommunicationNoAwacsPath.Text != "" && txtCommunicationPath.Text != "" && txtCommunicationNoTmaPath.Text != "")
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

            if (txtCommunicationNoAwacsPath.Text != "")
            {
                Properties.Settings.Default.filePathCommunicationNoAwacs = txtCommunicationNoAwacsPath.Text;
            }

            if (txtCommunicationNoTmaPath.Text != "")
            {
                Properties.Settings.Default.filePathCommunicationNoTma = txtCommunicationNoTmaPath.Text;
            }

            if (txtCommunicationPath.Text != "")
            {
                Properties.Settings.Default.filePathCommunication = txtCommunicationPath.Text;
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

        private void btnBrowseCommunication_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ".docm|*.docm";
            ofd.Title = "Select the file path for the supplied file Communication.docm";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtCommunicationPath.Text = ofd.FileName; // only file name

                if (txtKneeboardPath.Text != "" && txtCommunicationNoAwacsPath.Text != "" && txtCommunicationNoTmaPath.Text != "")
                {
                    chkCommunicationHelp.Enabled = true;
                }
            }
        }

        private void btnBrowseCommunicationNoTma_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ".docm|*.docm";
            ofd.Title = "Select the file path for the supplied file CommunicationNoTma.docm";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtCommunicationNoTmaPath.Text = ofd.FileName; // only file name

                if (txtKneeboardPath.Text != "" && txtCommunicationNoAwacsPath.Text != "" && txtCommunicationPath.Text != "")
                {
                    chkCommunicationHelp.Enabled = true;
                }
            }
        }
    }
}
