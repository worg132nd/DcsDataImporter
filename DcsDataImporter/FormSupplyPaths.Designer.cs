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
            this.btnBrowseCommunicationNoAwacs = new System.Windows.Forms.Button();
            this.txtCommunicationNoAwacsPath = new System.Windows.Forms.TextBox();
            this.lblDocmPath = new System.Windows.Forms.Label();
            this.btnBrowseKneeboardB = new System.Windows.Forms.Button();
            this.txtKneeboardPath = new System.Windows.Forms.TextBox();
            this.lblKneeboardPath = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCommunicationHelp = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCommunicationPath = new System.Windows.Forms.TextBox();
            this.btnBrowseCommunication = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCommunicationNoTmaPath = new System.Windows.Forms.TextBox();
            this.btnBrowseCommunicationNoTma = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBrowseCommunicationNoAwacs
            // 
            this.btnBrowseCommunicationNoAwacs.Location = new System.Drawing.Point(260, 83);
            this.btnBrowseCommunicationNoAwacs.Name = "btnBrowseCommunicationNoAwacs";
            this.btnBrowseCommunicationNoAwacs.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseCommunicationNoAwacs.TabIndex = 0;
            this.btnBrowseCommunicationNoAwacs.Text = "Browse";
            this.btnBrowseCommunicationNoAwacs.UseVisualStyleBackColor = true;
            this.btnBrowseCommunicationNoAwacs.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtCommunicationNoAwacsPath
            // 
            this.txtCommunicationNoAwacsPath.Enabled = false;
            this.txtCommunicationNoAwacsPath.Location = new System.Drawing.Point(12, 83);
            this.txtCommunicationNoAwacsPath.Name = "txtCommunicationNoAwacsPath";
            this.txtCommunicationNoAwacsPath.Size = new System.Drawing.Size(242, 20);
            this.txtCommunicationNoAwacsPath.TabIndex = 87;
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
            this.btnApply.Location = new System.Drawing.Point(180, 270);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 91;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(260, 270);
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
            this.chkCommunicationHelp.Location = new System.Drawing.Point(12, 233);
            this.chkCommunicationHelp.Name = "chkCommunicationHelp";
            this.chkCommunicationHelp.Size = new System.Drawing.Size(302, 17);
            this.chkCommunicationHelp.TabIndex = 93;
            this.chkCommunicationHelp.Text = "Create kneeboard pages for DCS with communication help";
            this.chkCommunicationHelp.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(267, 13);
            this.label1.TabIndex = 96;
            this.label1.Text = "Select path to supplied Word file Communication.docm:";
            // 
            // txtCommunicationPath
            // 
            this.txtCommunicationPath.Enabled = false;
            this.txtCommunicationPath.Location = new System.Drawing.Point(12, 138);
            this.txtCommunicationPath.Name = "txtCommunicationPath";
            this.txtCommunicationPath.Size = new System.Drawing.Size(242, 20);
            this.txtCommunicationPath.TabIndex = 95;
            // 
            // btnBrowseCommunication
            // 
            this.btnBrowseCommunication.Location = new System.Drawing.Point(260, 138);
            this.btnBrowseCommunication.Name = "btnBrowseCommunication";
            this.btnBrowseCommunication.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseCommunication.TabIndex = 94;
            this.btnBrowseCommunication.Text = "Browse";
            this.btnBrowseCommunication.UseVisualStyleBackColor = true;
            this.btnBrowseCommunication.Click += new System.EventHandler(this.btnBrowseCommunication_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(302, 13);
            this.label2.TabIndex = 99;
            this.label2.Text = "Select path to supplied Word file CommunicationNoTma.docm:";
            // 
            // txtCommunicationNoTmaPath
            // 
            this.txtCommunicationNoTmaPath.Enabled = false;
            this.txtCommunicationNoTmaPath.Location = new System.Drawing.Point(12, 189);
            this.txtCommunicationNoTmaPath.Name = "txtCommunicationNoTmaPath";
            this.txtCommunicationNoTmaPath.Size = new System.Drawing.Size(242, 20);
            this.txtCommunicationNoTmaPath.TabIndex = 98;
            // 
            // btnBrowseCommunicationNoTma
            // 
            this.btnBrowseCommunicationNoTma.Location = new System.Drawing.Point(260, 189);
            this.btnBrowseCommunicationNoTma.Name = "btnBrowseCommunicationNoTma";
            this.btnBrowseCommunicationNoTma.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseCommunicationNoTma.TabIndex = 97;
            this.btnBrowseCommunicationNoTma.Text = "Browse";
            this.btnBrowseCommunicationNoTma.UseVisualStyleBackColor = true;
            this.btnBrowseCommunicationNoTma.Click += new System.EventHandler(this.btnBrowseCommunicationNoTma_Click);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 305);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCommunicationNoTmaPath);
            this.Controls.Add(this.btnBrowseCommunicationNoTma);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCommunicationPath);
            this.Controls.Add(this.btnBrowseCommunication);
            this.Controls.Add(this.chkCommunicationHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.txtKneeboardPath);
            this.Controls.Add(this.btnBrowseKneeboardB);
            this.Controls.Add(this.lblKneeboardPath);
            this.Controls.Add(this.lblDocmPath);
            this.Controls.Add(this.txtCommunicationNoAwacsPath);
            this.Controls.Add(this.btnBrowseCommunicationNoAwacs);
            this.Name = "FormSettings";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowseCommunicationNoAwacs;
        private System.Windows.Forms.TextBox txtCommunicationNoAwacsPath;
        private System.Windows.Forms.Label lblDocmPath;
        private System.Windows.Forms.Button btnBrowseKneeboardB;
        private System.Windows.Forms.TextBox txtKneeboardPath;
        private System.Windows.Forms.Label lblKneeboardPath;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCommunicationHelp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCommunicationPath;
        private System.Windows.Forms.Button btnBrowseCommunication;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCommunicationNoTmaPath;
        private System.Windows.Forms.Button btnBrowseCommunicationNoTma;
    }
}