namespace DcsDataImporter
{
    partial class FormSettings
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
            this.btnBrowseDocm = new System.Windows.Forms.Button();
            this.txtWordFile = new System.Windows.Forms.TextBox();
            this.lblDocmPath = new System.Windows.Forms.Label();
            this.btnBrowseKneeboardB = new System.Windows.Forms.Button();
            this.txtKneeboardPath = new System.Windows.Forms.TextBox();
            this.lblKneeboardPath = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCommunicationHelp = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnBrowseDocm
            // 
            this.btnBrowseDocm.Location = new System.Drawing.Point(260, 83);
            this.btnBrowseDocm.Name = "btnBrowseDocm";
            this.btnBrowseDocm.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseDocm.TabIndex = 0;
            this.btnBrowseDocm.Text = "Browse";
            this.btnBrowseDocm.UseVisualStyleBackColor = true;
            this.btnBrowseDocm.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtWordFile
            // 
            this.txtWordFile.Enabled = false;
            this.txtWordFile.Location = new System.Drawing.Point(12, 83);
            this.txtWordFile.Name = "txtWordFile";
            this.txtWordFile.Size = new System.Drawing.Size(242, 20);
            this.txtWordFile.TabIndex = 87;
            // 
            // lblDocmPath
            // 
            this.lblDocmPath.AutoSize = true;
            this.lblDocmPath.Location = new System.Drawing.Point(12, 67);
            this.lblDocmPath.Name = "lblDocmPath";
            this.lblDocmPath.Size = new System.Drawing.Size(313, 13);
            this.lblDocmPath.TabIndex = 88;
            this.lblDocmPath.Text = "Select path to supplied Word file CommunicationNoAwacs.docm:";
            // 
            // btnBrowseKneeboardB
            // 
            this.btnBrowseKneeboardB.Location = new System.Drawing.Point(260, 31);
            this.btnBrowseKneeboardB.Name = "btnBrowseKneeboardB";
            this.btnBrowseKneeboardB.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseKneeboardB.TabIndex = 89;
            this.btnBrowseKneeboardB.Text = "Browse";
            this.btnBrowseKneeboardB.UseVisualStyleBackColor = true;
            this.btnBrowseKneeboardB.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // txtKneeboardPath
            // 
            this.txtKneeboardPath.Enabled = false;
            this.txtKneeboardPath.Location = new System.Drawing.Point(12, 31);
            this.txtKneeboardPath.Name = "txtKneeboardPath";
            this.txtKneeboardPath.Size = new System.Drawing.Size(242, 20);
            this.txtKneeboardPath.TabIndex = 90;
            // 
            // lblKneeboardPath
            // 
            this.lblKneeboardPath.AutoSize = true;
            this.lblKneeboardPath.Location = new System.Drawing.Point(12, 15);
            this.lblKneeboardPath.Name = "lblKneeboardPath";
            this.lblKneeboardPath.Size = new System.Drawing.Size(218, 13);
            this.lblKneeboardPath.TabIndex = 88;
            this.lblKneeboardPath.Text = "Select installation path to Kneeboard Builder:";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(179, 165);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 91;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(259, 165);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 92;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkCommunicationHelp
            // 
            this.chkCommunicationHelp.AutoSize = true;
            this.chkCommunicationHelp.Location = new System.Drawing.Point(12, 128);
            this.chkCommunicationHelp.Name = "chkCommunicationHelp";
            this.chkCommunicationHelp.Size = new System.Drawing.Size(302, 17);
            this.chkCommunicationHelp.TabIndex = 93;
            this.chkCommunicationHelp.Text = "Create kneeboard pages for DCS with communication help";
            this.chkCommunicationHelp.UseVisualStyleBackColor = true;
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 199);
            this.Controls.Add(this.chkCommunicationHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.txtKneeboardPath);
            this.Controls.Add(this.btnBrowseKneeboardB);
            this.Controls.Add(this.lblKneeboardPath);
            this.Controls.Add(this.lblDocmPath);
            this.Controls.Add(this.txtWordFile);
            this.Controls.Add(this.btnBrowseDocm);
            this.Name = "FormSettings";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowseDocm;
        private System.Windows.Forms.TextBox txtWordFile;
        private System.Windows.Forms.Label lblDocmPath;
        private System.Windows.Forms.Button btnBrowseKneeboardB;
        private System.Windows.Forms.TextBox txtKneeboardPath;
        private System.Windows.Forms.Label lblKneeboardPath;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCommunicationHelp;
    }
}