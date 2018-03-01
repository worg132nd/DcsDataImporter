namespace DcsDataImporter
{
    partial class SelectSupport
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
            this.chkFaca = new System.Windows.Forms.CheckBox();
            this.chkJstar = new System.Windows.Forms.CheckBox();
            this.chkCsar = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkExtraAwacsAA = new System.Windows.Forms.CheckBox();
            this.chkExtraAwacsAG = new System.Windows.Forms.CheckBox();
            this.chkAwacsAG = new System.Windows.Forms.CheckBox();
            this.chkAwacsAA = new System.Windows.Forms.CheckBox();
            this.chkTma = new System.Windows.Forms.CheckBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.chkExtraPackage = new System.Windows.Forms.CheckBox();
            this.chkExtraJtac = new System.Windows.Forms.CheckBox();
            this.chkScramble = new System.Windows.Forms.CheckBox();
            this.numTankers = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTankers)).BeginInit();
            this.SuspendLayout();
            // 
            // chkFaca
            // 
            this.chkFaca.AutoSize = true;
            this.chkFaca.Location = new System.Drawing.Point(35, 144);
            this.chkFaca.Name = "chkFaca";
            this.chkFaca.Size = new System.Drawing.Size(59, 17);
            this.chkFaca.TabIndex = 3;
            this.chkFaca.Text = "FAC(A)";
            this.chkFaca.UseVisualStyleBackColor = true;
            // 
            // chkJstar
            // 
            this.chkJstar.AutoSize = true;
            this.chkJstar.Location = new System.Drawing.Point(35, 190);
            this.chkJstar.Name = "chkJstar";
            this.chkJstar.Size = new System.Drawing.Size(60, 17);
            this.chkJstar.TabIndex = 5;
            this.chkJstar.Text = "JSTAR";
            this.chkJstar.UseVisualStyleBackColor = true;
            // 
            // chkCsar
            // 
            this.chkCsar.AutoSize = true;
            this.chkCsar.Checked = true;
            this.chkCsar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCsar.Location = new System.Drawing.Point(35, 167);
            this.chkCsar.Name = "chkCsar";
            this.chkCsar.Size = new System.Drawing.Size(55, 17);
            this.chkCsar.TabIndex = 6;
            this.chkCsar.Text = "CSAR";
            this.chkCsar.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(99, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 303;
            this.label8.Text = "AWACS";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.chkExtraAwacsAA);
            this.panel1.Controls.Add(this.chkExtraAwacsAG);
            this.panel1.Controls.Add(this.chkAwacsAG);
            this.panel1.Controls.Add(this.chkAwacsAA);
            this.panel1.Location = new System.Drawing.Point(21, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(210, 103);
            this.panel1.TabIndex = 302;
            // 
            // chkExtraAwacsAA
            // 
            this.chkExtraAwacsAA.AutoSize = true;
            this.chkExtraAwacsAA.Checked = true;
            this.chkExtraAwacsAA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExtraAwacsAA.Location = new System.Drawing.Point(13, 80);
            this.chkExtraAwacsAA.Name = "chkExtraAwacsAA";
            this.chkExtraAwacsAA.Size = new System.Drawing.Size(134, 17);
            this.chkExtraAwacsAA.TabIndex = 307;
            this.chkExtraAwacsAA.Text = "Additional AWACS A-A";
            this.chkExtraAwacsAA.UseVisualStyleBackColor = true;
            // 
            // chkExtraAwacsAG
            // 
            this.chkExtraAwacsAG.AutoSize = true;
            this.chkExtraAwacsAG.Location = new System.Drawing.Point(13, 57);
            this.chkExtraAwacsAG.Name = "chkExtraAwacsAG";
            this.chkExtraAwacsAG.Size = new System.Drawing.Size(135, 17);
            this.chkExtraAwacsAG.TabIndex = 306;
            this.chkExtraAwacsAG.Text = "Additional AWACS A-G";
            this.chkExtraAwacsAG.UseVisualStyleBackColor = true;
            // 
            // chkAwacsAG
            // 
            this.chkAwacsAG.AutoSize = true;
            this.chkAwacsAG.Checked = true;
            this.chkAwacsAG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAwacsAG.Location = new System.Drawing.Point(13, 11);
            this.chkAwacsAG.Name = "chkAwacsAG";
            this.chkAwacsAG.Size = new System.Drawing.Size(86, 17);
            this.chkAwacsAG.TabIndex = 304;
            this.chkAwacsAG.Text = "AWACS A-G";
            this.chkAwacsAG.UseVisualStyleBackColor = true;
            // 
            // chkAwacsAA
            // 
            this.chkAwacsAA.AutoSize = true;
            this.chkAwacsAA.Checked = true;
            this.chkAwacsAA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAwacsAA.Location = new System.Drawing.Point(13, 34);
            this.chkAwacsAA.Name = "chkAwacsAA";
            this.chkAwacsAA.Size = new System.Drawing.Size(85, 17);
            this.chkAwacsAA.TabIndex = 305;
            this.chkAwacsAA.Text = "AWACS A-A";
            this.chkAwacsAA.UseVisualStyleBackColor = true;
            // 
            // chkTma
            // 
            this.chkTma.AutoSize = true;
            this.chkTma.Checked = true;
            this.chkTma.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTma.Location = new System.Drawing.Point(12, 267);
            this.chkTma.Name = "chkTma";
            this.chkTma.Size = new System.Drawing.Size(49, 17);
            this.chkTma.TabIndex = 304;
            this.chkTma.Text = "TMA";
            this.chkTma.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(165, 263);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 305;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // chkExtraPackage
            // 
            this.chkExtraPackage.AutoSize = true;
            this.chkExtraPackage.Location = new System.Drawing.Point(130, 167);
            this.chkExtraPackage.Name = "chkExtraPackage";
            this.chkExtraPackage.Size = new System.Drawing.Size(90, 17);
            this.chkExtraPackage.TabIndex = 306;
            this.chkExtraPackage.Text = "Add package";
            this.chkExtraPackage.UseVisualStyleBackColor = true;
            // 
            // chkExtraJtac
            // 
            this.chkExtraJtac.AutoSize = true;
            this.chkExtraJtac.Location = new System.Drawing.Point(130, 144);
            this.chkExtraJtac.Name = "chkExtraJtac";
            this.chkExtraJtac.Size = new System.Drawing.Size(74, 17);
            this.chkExtraJtac.TabIndex = 307;
            this.chkExtraJtac.Text = "Add JTAC";
            this.chkExtraJtac.UseVisualStyleBackColor = true;
            // 
            // chkScramble
            // 
            this.chkScramble.AutoSize = true;
            this.chkScramble.Checked = true;
            this.chkScramble.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkScramble.Location = new System.Drawing.Point(35, 213);
            this.chkScramble.Name = "chkScramble";
            this.chkScramble.Size = new System.Drawing.Size(70, 17);
            this.chkScramble.TabIndex = 308;
            this.chkScramble.Text = "Scramble";
            this.chkScramble.UseVisualStyleBackColor = true;
            // 
            // numTankers
            // 
            this.numTankers.Location = new System.Drawing.Point(130, 189);
            this.numTankers.Name = "numTankers";
            this.numTankers.Size = new System.Drawing.Size(26, 20);
            this.numTankers.TabIndex = 309;
            this.numTankers.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(162, 191);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 310;
            this.label1.Text = "Tankers";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(12, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(229, 230);
            this.panel2.TabIndex = 311;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(89, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 312;
            this.label2.Text = "Frequencies";
            // 
            // SelectSupport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 298);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numTankers);
            this.Controls.Add(this.chkScramble);
            this.Controls.Add(this.chkExtraJtac);
            this.Controls.Add(this.chkExtraPackage);
            this.Controls.Add(this.chkTma);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chkCsar);
            this.Controls.Add(this.chkJstar);
            this.Controls.Add(this.chkFaca);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SelectSupport";
            this.Text = "SelectSupport";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTankers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkFaca;
        private System.Windows.Forms.CheckBox chkJstar;
        private System.Windows.Forms.CheckBox chkCsar;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkExtraAwacsAG;
        private System.Windows.Forms.CheckBox chkAwacsAG;
        private System.Windows.Forms.CheckBox chkAwacsAA;
        private System.Windows.Forms.CheckBox chkTma;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.CheckBox chkExtraPackage;
        private System.Windows.Forms.CheckBox chkExtraJtac;
        private System.Windows.Forms.CheckBox chkExtraAwacsAA;
        private System.Windows.Forms.CheckBox chkScramble;
        private System.Windows.Forms.NumericUpDown numTankers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
    }
}