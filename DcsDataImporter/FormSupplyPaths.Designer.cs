namespace DcsDataImporter
{
    partial class FormSupplyPaths
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
            this.btnNext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBrowseDocm
            // 
            this.btnBrowseDocm.Location = new System.Drawing.Point(260, 25);
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
            this.txtWordFile.Location = new System.Drawing.Point(12, 25);
            this.txtWordFile.Name = "txtWordFile";
            this.txtWordFile.Size = new System.Drawing.Size(242, 20);
            this.txtWordFile.TabIndex = 87;
            // 
            // lblDocmPath
            // 
            this.lblDocmPath.AutoSize = true;
            this.lblDocmPath.Location = new System.Drawing.Point(12, 9);
            this.lblDocmPath.Name = "lblDocmPath";
            this.lblDocmPath.Size = new System.Drawing.Size(212, 13);
            this.lblDocmPath.TabIndex = 88;
            this.lblDocmPath.Text = "Select path to supplied Word (.docm) file(s):";
            // 
            // btnBrowseKneeboardB
            // 
            this.btnBrowseKneeboardB.Location = new System.Drawing.Point(260, 64);
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
            this.txtKneeboardPath.Location = new System.Drawing.Point(12, 64);
            this.txtKneeboardPath.Name = "txtKneeboardPath";
            this.txtKneeboardPath.Size = new System.Drawing.Size(242, 20);
            this.txtKneeboardPath.TabIndex = 90;
            // 
            // lblKneeboardPath
            // 
            this.lblKneeboardPath.AutoSize = true;
            this.lblKneeboardPath.Location = new System.Drawing.Point(12, 48);
            this.lblKneeboardPath.Name = "lblKneeboardPath";
            this.lblKneeboardPath.Size = new System.Drawing.Size(218, 13);
            this.lblKneeboardPath.TabIndex = 88;
            this.lblKneeboardPath.Text = "Select installation path to Kneeboard Builder:";
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(260, 105);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 91;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // FormSupplyPaths
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 140);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.txtKneeboardPath);
            this.Controls.Add(this.btnBrowseKneeboardB);
            this.Controls.Add(this.lblKneeboardPath);
            this.Controls.Add(this.lblDocmPath);
            this.Controls.Add(this.txtWordFile);
            this.Controls.Add(this.btnBrowseDocm);
            this.Name = "FormSupplyPaths";
            this.Text = "Supply paths";
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
        private System.Windows.Forms.Button btnNext;
    }
}