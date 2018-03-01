namespace DcsDataImporter
{
    partial class Form2
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvFlightplan = new System.Windows.Forms.DataGridView();
            this.colWpt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSpd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFormation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRemark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblKillbox = new System.Windows.Forms.Label();
            this.txtKillbox = new System.Windows.Forms.TextBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.dgvTma = new System.Windows.Forms.DataGridView();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAirbase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTMA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnImportMissionFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFlightplan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTma)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvFlightplan
            // 
            this.dgvFlightplan.AllowUserToAddRows = false;
            this.dgvFlightplan.AllowUserToDeleteRows = false;
            this.dgvFlightplan.AllowUserToResizeColumns = false;
            this.dgvFlightplan.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFlightplan.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFlightplan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFlightplan.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colWpt,
            this.colName,
            this.colAction,
            this.colPos,
            this.colAlt,
            this.colSpd,
            this.colFormation,
            this.colRemark});
            this.dgvFlightplan.Location = new System.Drawing.Point(12, 141);
            this.dgvFlightplan.Name = "dgvFlightplan";
            this.dgvFlightplan.RowHeadersVisible = false;
            this.dgvFlightplan.Size = new System.Drawing.Size(595, 617);
            this.dgvFlightplan.TabIndex = 2;
            // 
            // colWpt
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colWpt.DefaultCellStyle = dataGridViewCellStyle2;
            this.colWpt.HeaderText = "WPT";
            this.colWpt.Name = "colWpt";
            this.colWpt.ToolTipText = "Waypoint number";
            this.colWpt.Width = 33;
            // 
            // colName
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colName.DefaultCellStyle = dataGridViewCellStyle3;
            this.colName.HeaderText = "NAME";
            this.colName.Name = "colName";
            this.colName.ToolTipText = "Name of waypoint";
            this.colName.Width = 94;
            // 
            // colAction
            // 
            this.colAction.HeaderText = "Action";
            this.colAction.Name = "colAction";
            this.colAction.Width = 69;
            // 
            // colPos
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colPos.DefaultCellStyle = dataGridViewCellStyle4;
            this.colPos.HeaderText = "POS";
            this.colPos.Name = "colPos";
            this.colPos.ToolTipText = "Coordinates of waypoint or name in database";
            this.colPos.Width = 128;
            // 
            // colAlt
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colAlt.DefaultCellStyle = dataGridViewCellStyle5;
            this.colAlt.HeaderText = "ALT";
            this.colAlt.Name = "colAlt";
            this.colAlt.ToolTipText = "Altitude / elevation of waypoint";
            this.colAlt.Width = 38;
            // 
            // colSpd
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSpd.DefaultCellStyle = dataGridViewCellStyle6;
            this.colSpd.HeaderText = "TOT";
            this.colSpd.Name = "colSpd";
            this.colSpd.ToolTipText = "Time on Target";
            this.colSpd.Width = 50;
            // 
            // colFormation
            // 
            this.colFormation.HeaderText = "Formation";
            this.colFormation.Name = "colFormation";
            this.colFormation.Width = 75;
            // 
            // colRemark
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.colRemark.DefaultCellStyle = dataGridViewCellStyle7;
            this.colRemark.HeaderText = "Remark";
            this.colRemark.Name = "colRemark";
            this.colRemark.ToolTipText = "Remarks";
            this.colRemark.Width = 105;
            // 
            // lblKillbox
            // 
            this.lblKillbox.AutoSize = true;
            this.lblKillbox.Location = new System.Drawing.Point(12, 777);
            this.lblKillbox.Name = "lblKillbox";
            this.lblKillbox.Size = new System.Drawing.Size(40, 13);
            this.lblKillbox.TabIndex = 127;
            this.lblKillbox.Text = "Killbox:";
            // 
            // txtKillbox
            // 
            this.txtKillbox.Location = new System.Drawing.Point(58, 774);
            this.txtKillbox.Name = "txtKillbox";
            this.txtKillbox.Size = new System.Drawing.Size(549, 20);
            this.txtKillbox.TabIndex = 3;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(532, 866);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 6;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(451, 866);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 5;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // dgvTma
            // 
            this.dgvTma.AllowUserToAddRows = false;
            this.dgvTma.AllowUserToDeleteRows = false;
            this.dgvTma.AllowUserToResizeColumns = false;
            this.dgvTma.AllowUserToResizeRows = false;
            this.dgvTma.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTma.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colType,
            this.colAirbase,
            this.colTMA});
            this.dgvTma.Location = new System.Drawing.Point(12, 27);
            this.dgvTma.Name = "dgvTma";
            this.dgvTma.RowHeadersVisible = false;
            this.dgvTma.Size = new System.Drawing.Size(595, 89);
            this.dgvTma.TabIndex = 1;
            // 
            // colType
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colType.DefaultCellStyle = dataGridViewCellStyle8;
            this.colType.HeaderText = "";
            this.colType.Name = "colType";
            this.colType.Width = 35;
            // 
            // colAirbase
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colAirbase.DefaultCellStyle = dataGridViewCellStyle9;
            this.colAirbase.HeaderText = "Airbase";
            this.colAirbase.Name = "colAirbase";
            this.colAirbase.Width = 117;
            // 
            // colTMA
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colTMA.DefaultCellStyle = dataGridViewCellStyle10;
            this.colTMA.HeaderText = "TMA";
            this.colTMA.Name = "colTMA";
            this.colTMA.Width = 440;
            // 
            // btnImportMissionFile
            // 
            this.btnImportMissionFile.Location = new System.Drawing.Point(12, 866);
            this.btnImportMissionFile.Name = "btnImportMissionFile";
            this.btnImportMissionFile.Size = new System.Drawing.Size(113, 23);
            this.btnImportMissionFile.TabIndex = 4;
            this.btnImportMissionFile.Text = "Import mission file";
            this.btnImportMissionFile.UseVisualStyleBackColor = true;
            this.btnImportMissionFile.Click += new System.EventHandler(this.btnImportMissionFile_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 901);
            this.Controls.Add(this.btnImportMissionFile);
            this.Controls.Add(this.dgvTma);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.txtKillbox);
            this.Controls.Add(this.lblKillbox);
            this.Controls.Add(this.dgvFlightplan);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form2";
            this.Text = "Flightplan and Data";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFlightplan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTma)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvFlightplan;
        private System.Windows.Forms.Label lblKillbox;
        private System.Windows.Forms.TextBox txtKillbox;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.DataGridView dgvTma;
        private System.Windows.Forms.Button btnImportMissionFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAirbase;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTMA;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWpt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSpd;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFormation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRemark;
    }
}