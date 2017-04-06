using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DcsDataImporter
{
    public partial class FormSettings : Form
    {
        bool oldValue;
        public FormSettings()
        {
            this.CenterToParent();
            InitializeComponent();
            txtKneeboardPath.Text = Properties.Settings.Default.pathKneeboardBuilder;
            txtCommunicationPath.Text = Properties.Settings.Default.filePathCommunication;

            chkCommunicationHelp.Enabled = false;

            /* Load chkCommunicationHelp checkbox */
            chkCommunicationHelp.Checked = false;
            if (Properties.Settings.Default.CommunicationHelp)
            {
                chkCommunicationHelp.Checked = true;
            }

            if (!string.IsNullOrEmpty(txtCommunicationPath.Text))
            {
                chkCommunicationHelp.Enabled = true;
            }

            /* store the old value of chkCommunicationHelp to see if it changes */
            oldValue = false;
            if (chkCommunicationHelp.Checked)
            {
                oldValue = true;
            }
        }

        private void btnBrowseWordFiles_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the path for the supplied Word (.docm) files:";
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                    bool validFile = false;
                    int validFiles = 0;
                    
                    foreach (string file in files)
                    {
                        if (file.Contains("communication") && file.EndsWith(".docm")) {
                            validFile = true;
                            validFiles++;
                        }
                    }

                    System.Windows.Forms.MessageBox.Show(fbd.SelectedPath + "\n\nValid Word (.docm) files found: " + validFiles, "Message");

                    txtCommunicationPath.Text = fbd.SelectedPath;
                    if (validFile && !string.IsNullOrEmpty(txtCommunicationPath.Text) && !string.IsNullOrEmpty(txtKneeboardPath.Text))
                    {
                        chkCommunicationHelp.Enabled = true;
                    }
                }
            }
        }

        private void btnBrowseKneeboard_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ".exe|*.exe";
            ofd.Title = "Select path to the install directory of the Kneeboard Builder application by selecting the exe file";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtKneeboardPath.Text = ofd.FileName; // path

                if (!string.IsNullOrEmpty(txtCommunicationPath.Text))
                {
                    chkCommunicationHelp.Enabled = true;
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtKneeboardPath.Text))
            {
                Properties.Settings.Default.pathKneeboardBuilder = txtKneeboardPath.Text; // path
            }
            
            if (!string.IsNullOrEmpty(txtCommunicationPath.Text))
            {
                Properties.Settings.Default.filePathCommunication = txtCommunicationPath.Text;
            }

            if (chkCommunicationHelp.Checked && (chkCommunicationHelp.Checked != oldValue))
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

                if (!string.IsNullOrEmpty(txtKneeboardPath.Text) && !string.IsNullOrEmpty(txtCommunicationPath.Text))
                {
                    chkCommunicationHelp.Enabled = true;
                }
            }
        }
    }
}
