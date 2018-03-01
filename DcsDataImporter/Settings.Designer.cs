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
            this.btnBrowseWordFiles = new System.Windows.Forms.Button();
            this.txtCommunicationPath = new System.Windows.Forms.TextBox();
            this.btnBrowseKneeboard = new System.Windows.Forms.Button();
            this.txtKneeboardPath = new System.Windows.Forms.TextBox();
            this.lblKneeboardPath = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCommunicationHelp = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBrowseWordFiles
            // 
            this.btnBrowseWordFiles.Location = new System.Drawing.Point(260, 93);
            this.btnBrowseWordFiles.Name = "btnBrowseWordFiles";
            this.btnBrowseWordFiles.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseWordFiles.TabIndex = 0;
            this.btnBrowseWordFiles.Text = "Browse";
            this.btnBrowseWordFiles.UseVisualStyleBackColor = true;
            this.btnBrowseWordFiles.Click += new System.EventHandler(this.btnBrowseWordFiles_Click);
            // 
            // txtCommunicationPath
            // 
            this.txtCommunicationPath.Enabled = false;
            this.txtCommunicationPath.Location = new System.Drawing.Point(12, 95);
            this.txtCommunicationPath.Name = "txtCommunicationPath";
            this.txtCommunicationPath.Size = new System.Drawing.Size(242, 20);
            this.txtCommunicationPath.TabIndex = 87;
            // 
            // btnBrowseKneeboard
            // 
            this.btnBrowseKneeboard.Location = new System.Drawing.Point(260, 28);
            this.btnBrowseKneeboard.Name = "btnBrowseKneeboard";
            this.btnBrowseKneeboard.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseKneeboard.TabIndex = 89;
            this.btnBrowseKneeboard.Text = "Browse";
            this.btnBrowseKneeboard.UseVisualStyleBackColor = true;
            this.btnBrowseKneeboard.Click += new System.EventHandler(this.btnBrowseKneeboard_Click);
            // 
            // txtKneeboardPath
            // 
            this.txtKneeboardPath.Enabled = false;
            this.txtKneeboardPath.Location = new System.Drawing.Point(12, 28);
            this.txtKneeboardPath.Name = "txtKneeboardPath";
            this.txtKneeboardPath.Size = new System.Drawing.Size(242, 20);
            this.txtKneeboardPath.TabIndex = 90;
            // 
            // lblKneeboardPath
            // 
            this.lblKneeboardPath.AutoSize = true;
            this.lblKneeboardPath.Location = new System.Drawing.Point(12, 12);
            this.lblKneeboardPath.Name = "lblKneeboardPath";
            this.lblKneeboardPath.Size = new System.Drawing.Size(218, 13);
            this.lblKneeboardPath.TabIndex = 88;
            this.lblKneeboardPath.Text = "Select installation path to Kneeboard Builder:";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(179, 178);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 91;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(259, 178);
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
            this.chkCommunicationHelp.Location = new System.Drawing.Point(12, 155);
            this.chkCommunicationHelp.Name = "chkCommunicationHelp";
            this.chkCommunicationHelp.Size = new System.Drawing.Size(302, 17);
            this.chkCommunicationHelp.TabIndex = 93;
            this.chkCommunicationHelp.Text = "Create kneeboard pages for DCS with communication help";
            this.chkCommunicationHelp.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 13);
            this.label3.TabIndex = 101;
            this.label3.Text = "Select paths to supplied Word files:";
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 212);
            this.Controls.Add(this.btnBrowseWordFiles);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCommunicationPath);
            this.Controls.Add(this.chkCommunicationHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.txtKneeboardPath);
            this.Controls.Add(this.btnBrowseKneeboard);
            this.Controls.Add(this.lblKneeboardPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormSettings";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowseWordFiles;
        private System.Windows.Forms.TextBox txtCommunicationPath;
        private System.Windows.Forms.Button btnBrowseKneeboard;
        private System.Windows.Forms.TextBox txtKneeboardPath;
        private System.Windows.Forms.Label lblKneeboardPath;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCommunicationHelp;
        private System.Windows.Forms.Label label3;
    }
}