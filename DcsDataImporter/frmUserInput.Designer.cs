namespace DcsDataImporter
{
    partial class frmUserInput
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
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFlightPosition = new System.Windows.Forms.ComboBox();
            this.numMissionId = new System.Windows.Forms.NumericUpDown();
            this.chkTraining = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFrequencyFile = new System.Windows.Forms.TextBox();
            this.btnFrequencyFileBrowse = new System.Windows.Forms.Button();
            this.btnLoadPrev = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numMissionId)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Enter Mission ID from ATO:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(263, 117);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 84;
            this.button2.Text = "Import Data";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 79;
            this.label1.Text = "Flight position:";
            // 
            // txtFlightPosition
            // 
            this.txtFlightPosition.FormattingEnabled = true;
            this.txtFlightPosition.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.txtFlightPosition.Location = new System.Drawing.Point(226, 33);
            this.txtFlightPosition.Name = "txtFlightPosition";
            this.txtFlightPosition.Size = new System.Drawing.Size(112, 21);
            this.txtFlightPosition.TabIndex = 82;
            this.txtFlightPosition.Text = "1";
            // 
            // numMissionId
            // 
            this.numMissionId.Location = new System.Drawing.Point(226, 7);
            this.numMissionId.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numMissionId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMissionId.Name = "numMissionId";
            this.numMissionId.Size = new System.Drawing.Size(112, 20);
            this.numMissionId.TabIndex = 81;
            this.numMissionId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkTraining
            // 
            this.chkTraining.AutoSize = true;
            this.chkTraining.Location = new System.Drawing.Point(15, 123);
            this.chkTraining.Name = "chkTraining";
            this.chkTraining.Size = new System.Drawing.Size(106, 17);
            this.chkTraining.TabIndex = 83;
            this.chkTraining.Text = "Standard training";
            this.chkTraining.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 13);
            this.label4.TabIndex = 85;
            this.label4.Text = "Frequency file selected:";
            // 
            // txtFrequencyFile
            // 
            this.txtFrequencyFile.Enabled = false;
            this.txtFrequencyFile.Location = new System.Drawing.Point(15, 80);
            this.txtFrequencyFile.Name = "txtFrequencyFile";
            this.txtFrequencyFile.Size = new System.Drawing.Size(242, 20);
            this.txtFrequencyFile.TabIndex = 86;
            // 
            // btnFrequencyFileBrowse
            // 
            this.btnFrequencyFileBrowse.Location = new System.Drawing.Point(263, 80);
            this.btnFrequencyFileBrowse.Name = "btnFrequencyFileBrowse";
            this.btnFrequencyFileBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnFrequencyFileBrowse.TabIndex = 87;
            this.btnFrequencyFileBrowse.Text = "Browse";
            this.btnFrequencyFileBrowse.UseVisualStyleBackColor = true;
            this.btnFrequencyFileBrowse.Click += new System.EventHandler(this.btnFrequencyFileBrowse_Click);
            // 
            // btnLoadPrev
            // 
            this.btnLoadPrev.Location = new System.Drawing.Point(171, 117);
            this.btnLoadPrev.Name = "btnLoadPrev";
            this.btnLoadPrev.Size = new System.Drawing.Size(75, 23);
            this.btnLoadPrev.TabIndex = 88;
            this.btnLoadPrev.Text = "Load prev.";
            this.btnLoadPrev.UseVisualStyleBackColor = true;
            this.btnLoadPrev.Click += new System.EventHandler(this.btnLoadPrev_Click);
            // 
            // frmUserInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 152);
            this.Controls.Add(this.btnLoadPrev);
            this.Controls.Add(this.btnFrequencyFileBrowse);
            this.Controls.Add(this.txtFrequencyFile);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkTraining);
            this.Controls.Add(this.numMissionId);
            this.Controls.Add(this.txtFlightPosition);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmUserInput";
            this.Text = "Import Data";
            ((System.ComponentModel.ISupportInitialize)(this.numMissionId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox txtFlightPosition;
        private System.Windows.Forms.NumericUpDown numMissionId;
        private System.Windows.Forms.CheckBox chkTraining;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtFrequencyFile;
        private System.Windows.Forms.Button btnFrequencyFileBrowse;
        private System.Windows.Forms.Button btnLoadPrev;
    }
}