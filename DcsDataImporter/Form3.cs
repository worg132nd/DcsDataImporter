using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/* TODO:
 * 
 * If for instance selecting a maverick, e.g. AGM-65D, set boxes to inactive (enabled = false) for non-relevant boxes.
 * The maverick has no need to select between SGL and PAIR, has no HOF, and does not have to select between Nose and Tail fusing.
 * 
 */

namespace DcsDataImporter
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            initTarget();
        }

        private void initTarget()
        {
            initTgtDgv(dgvTgtLead);
            initTgtDgv(dgvTgtElem);
            initProfiles();
        }

        private void initTgtDgv(DataGridView dgv)
        {
            dgv.RowCount = 3;
            dgv.DefaultCellStyle.SelectionBackColor = dgv.DefaultCellStyle.BackColor;
            dgv.DefaultCellStyle.SelectionForeColor = dgv.DefaultCellStyle.ForeColor;

            var row = dgv.Rows[0];
            row.Cells[0].Value = "Primary";
            row.Cells[2].Value = "Secondary";
            row = dgv.Rows[1];
            row.Cells[0].Value = row.Cells[2].Value = "DMPI";
            row = dgv.Rows[2];
            row.Cells[0].Value = row.Cells[2].Value = "Coordinates";
        }

        private void initProfiles()
        {
            initNumRipple();
            initNumSpacing();
            initNumRPM();
            initTxtHOF();
            initCbSGLPAIR();
            initCbFusing();
            initCbMode();
        }

        private void initCbMode()
        {
            disableCbMode();
        }

        private void disableCbMode()
        {
            cbMode1.Enabled = cbMode2.Enabled = cbMode3.Enabled = cbMode4.Enabled = false;
            lblMode.Enabled = false;
        }

        private void enableCbMode()
        {
            cbMode1.Enabled = cbMode2.Enabled = cbMode3.Enabled = cbMode4.Enabled = true;
            lblMode.Enabled = true;
        }

        private void initCbFusing()
        {
            disableCbFusing();
        }

        private void disableCbFusing()
        {
            cbFusing1.Enabled = cbFusing2.Enabled = cbFusing3.Enabled = cbFusing4.Enabled = false;
            lblFusing.Enabled = false;
        }

        private void enableCbFusing()
        {
            cbFusing1.Enabled = cbFusing2.Enabled = cbFusing3.Enabled = cbFusing4.Enabled = true;
            lblFusing.Enabled = true;
        }

        private void initCbSGLPAIR()
        {
            disableCbSGLPAIR();
        }

        private void disableCbSGLPAIR()
        {
            cbSGLPAIR1.Enabled = cbSGLPAIR2.Enabled = cbSGLPAIR3.Enabled = cbSGLPAIR4.Enabled = false;
            lblSGLPAIR.Enabled = false;
        }

        private void enableCbSGLPAIR()
        {
            cbSGLPAIR1.Enabled = cbSGLPAIR2.Enabled = cbSGLPAIR3.Enabled = cbSGLPAIR4.Enabled = true;
            lblSGLPAIR.Enabled = true;
        }

        private void initNumRipple()
        {
            numRipple1.Text = numRipple2.Text = numRipple3.Text = numRipple4.Text = "-";
            disableRipple();
        }

        private void initTxtHOF()
        {
            disableTxtHOF();
        }

        private void disableTxtHOF()
        {
            txtHOF1.Enabled = txtHOF2.Enabled = txtHOF3.Enabled = txtHOF4.Enabled = false;
            lblHOF.Enabled = false;
        }

        private void enableTxtHOF()
        {
            txtHOF1.Enabled = txtHOF2.Enabled = txtHOF3.Enabled = txtHOF4.Enabled = true;
            lblHOF.Enabled = true;
        }

        private void initNumSpacing()
        {
            numSpacing1.Text = numSpacing2.Text = numSpacing3.Text = numSpacing4.Text = "-";
            disableSpacing();
        }

        private void disableSpacing()
        {
            numSpacing1.Enabled = numSpacing2.Enabled = numSpacing3.Enabled = numSpacing4.Enabled = false;
            lblSpacing.Enabled = false;
        }

        private void enableSpacing()
        {
            numSpacing1.Enabled = numSpacing2.Enabled = numSpacing3.Enabled = numSpacing4.Enabled = true;
            lblSpacing.Enabled = true;
        }

        private void initNumRPM()
        {
            numRPM1.Text = numRPM2.Text = numRPM3.Text = numRPM4.Text = "-";
            disableRPM();
        }

        private void disableRPM()
        {
            numRPM1.Enabled = numRPM2.Enabled = numRPM3.Enabled = numRPM4.Enabled = false;
            lblRPM.Enabled = false;
        }

        private void enableRPM()
        {
            numRPM1.Enabled = numRPM2.Enabled = numRPM3.Enabled = numRPM4.Enabled = true;
            lblRPM.Enabled = true;
        }

        private void disableRipple()
        {
            numRipple1.Enabled = numRipple2.Enabled = numRipple3.Enabled = numRipple4.Enabled = false;
            lblRPL.Enabled = false;
        }

        private void enableRipple()
        {
            numRipple1.Enabled = numRipple2.Enabled = numRipple3.Enabled = numRipple4.Enabled = true;
            lblRPL.Enabled = true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void cbMunitions2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbMunitions3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbMunitions4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            /* Todo: add back functionality */
        }
    }
}
