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
            Mode1.Enabled = Mode2.Enabled = Mode3.Enabled = Mode4.Enabled = false;
            lblMode.Enabled = false;
        }

        private void enableCbMode()
        {
            Mode1.Enabled = Mode2.Enabled = Mode3.Enabled = Mode4.Enabled = true;
            lblMode.Enabled = true;
        }

        private void initCbFusing()
        {
            disableCbFusing();
        }

        private void disableCbFusing()
        {
            Fusing1.Enabled = Fusing2.Enabled = Fusing3.Enabled = Fusing4.Enabled = false;
            lblFusing.Enabled = false;
        }

        private void enableCbFusing()
        {
            Fusing1.Enabled = Fusing2.Enabled = Fusing3.Enabled = Fusing4.Enabled = true;
            lblFusing.Enabled = true;
        }

        private void initCbSGLPAIR()
        {
            disableCbSGLPAIR();
        }

        private void disableCbSGLPAIR()
        {
            SGLPAIR1.Enabled = SGLPAIR2.Enabled = SGLPAIR3.Enabled = SGLPAIR4.Enabled = false;
            lblSGLPAIR.Enabled = false;
        }

        private void enableCbSGLPAIR()
        {
            SGLPAIR1.Enabled = SGLPAIR2.Enabled = SGLPAIR3.Enabled = SGLPAIR4.Enabled = true;
            lblSGLPAIR.Enabled = true;
        }

        private void initNumRipple()
        {
            RPL1.Text = RPL2.Text = RPL3.Text = RPL4.Text = "-";
            disableRipple();
        }

        private void initTxtHOF()
        {
            disableTxtHOF();
        }

        private void disableTxtHOF()
        {
            HOF1.Enabled = HOF2.Enabled = HOF3.Enabled = HOF4.Enabled = false;
            lblHOF.Enabled = false;
        }

        private void enableTxtHOF()
        {
            HOF1.Enabled = HOF2.Enabled = HOF3.Enabled = HOF4.Enabled = true;
            lblHOF.Enabled = true;
        }

        private void initNumSpacing()
        {
            Spacing1.Text = Spacing2.Text = Spacing3.Text = Spacing4.Text = "-";
            disableSpacing();
        }

        private void disableSpacing()
        {
            Spacing1.Enabled = Spacing2.Enabled = Spacing3.Enabled = Spacing4.Enabled = false;
            lblSpacing.Enabled = false;
        }

        private void enableSpacing()
        {
            Spacing1.Enabled = Spacing2.Enabled = Spacing3.Enabled = Spacing4.Enabled = true;
            lblSpacing.Enabled = true;
        }

        private void initNumRPM()
        {
            RPM1.Text = RPM2.Text = RPM3.Text = RPM4.Text = "-";
            disableRPM();
        }

        private void disableRPM()
        {
            RPM1.Enabled = RPM2.Enabled = RPM3.Enabled = RPM4.Enabled = false;
            lblRPM.Enabled = false;
        }

        private void enableRPM()
        {
            RPM1.Enabled = RPM2.Enabled = RPM3.Enabled = RPM4.Enabled = true;
            lblRPM.Enabled = true;
        }

        private void disableRipple()
        {
            RPL1.Enabled = RPL2.Enabled = RPL3.Enabled = RPL4.Enabled = false;
            lblRPL.Enabled = false;
        }

        private void enableRipple()
        {
            RPL1.Enabled = RPL2.Enabled = RPL3.Enabled = RPL4.Enabled = true;
            lblRPL.Enabled = true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            /* Todo: add back functionality */
        }

        private void cbMunitions_SelectedIndexChanged(object sender, EventArgs e)
        {
            char number = getNumber(sender);

            updateRowActivationMunition(number, sender);
        }

        private void updateRowActivationSGLPAIR(char rowNr, object sender)
        {
            ComboBox sglpair = (ComboBox)sender;
            if (sglpair.Text.Contains("RIP"))
            {
                enable("RPL", rowNr);
                if (!isRocket(findObj("munitions", rowNr).Text))
                {
                    enable("Spacing", rowNr);
                }
            }
        }

        private void updateRowActivationMunition(char rowNr, object sender)
        {
            ComboBox munitions = (ComboBox)sender;

            string m = munitions.Text;

            if (m.Contains("GBU") || m.Contains("MK-") || m.Contains("BDU"))
            {
                enableBomb(rowNr);
            }

            if (m.Contains("CBU"))
            {
                enableCBU(rowNr);
            }

            if (isRocket(m))
            {
                enableRocket(rowNr);
            }

            if (isMav(m))
            {
                enableMav(rowNr);
            }

            if (m.Equals("Guns"))
            {
                enableGun(rowNr);
            }

            setDefaults(rowNr, munitions);
        }

        private void setDefaults(char rowNr, ComboBox munitions)
        {
            string m = munitions.Text;
            if (m.Equals("GBU-10") || m.Equals("GBU-12"))
            {
                Control obj = findObj("mode", rowNr);
                obj.Text = "CCIP";
                obj = findObj("fusing", rowNr);
                obj.Text = "Nose";
                obj = findObj("sglpair", rowNr);
                obj.Text = "SGL";
            }

            if (m.Contains("MK-8"))
            {
                Control obj = findObj("mode", rowNr);
                obj.Text = "CCIP";
                obj = findObj("fusing", rowNr);
                obj.Text = "N/T";
                obj = findObj("sglpair", rowNr);
                obj.Text = "SGL";
            }

            if (m.Equals("GBU-31") || m.Equals("GBU-38"))
            {
                Control obj = findObj("mode", rowNr);
                obj.Text = "CCRP";
                obj = findObj("fusing", rowNr);
                obj.Text = "N/T";
                obj = findObj("sglpair", rowNr);
                obj.Text = "SGL";
            }

            if (isRocket(m))
            {
                Control obj = findObj("mode", rowNr);
                obj.Text = m.Contains("LUU") || m.Contains("M257") ? obj.Text = "CCRP" : obj.Text = "CCIP";

                obj = findObj("sglpair", rowNr);
                if (m.Contains("LUU") || m.Equals("M257") || m.Equals("M156"))
                {
                    obj.Text = "SGL";
                } else
                {
                    obj = findObj("sglpair", rowNr);
                    obj.Text = "RIP SGL";
                    
                    obj = findObj("RPL", rowNr);
                    obj.Text = "7";
                }
            }

            if (m.Contains("CBU"))
            {
                Control obj = findObj("mode", rowNr);
                obj.Text = "CCRP";
                obj = findObj("fusing", rowNr);
                obj.Text = "N/T";
                obj = findObj("sglpair", rowNr);
                obj.Text = "SGL";
                obj = findObj("HOF", rowNr);
                obj.Text = "1.500 ft";
                obj = findObj("RPM", rowNr);
                obj.Text = "900";
            }
        }

        private Control findObj(string obj, char rowNr)
        {
            Control[] list = this.Controls.Find(obj + rowNr, true);

            if (list[0] != null)
            {
                return list[0];
            }
            return null;
        }

        private void enableRocket(char rowNr)
        {
            enable("mode", rowNr);
            disable("fusing", rowNr);
            enable("SGLPAIR", rowNr);
            disable("RPL", rowNr);
            disable("Spacing", rowNr);
            disable("HOF", rowNr);
            disable("RPM", rowNr);
        }

        private bool isRocket(string m)
        {
            if (m.Equals("M151") || m.Equals("MK5") || m.Equals("M156") || m.Equals("M257") || m.Contains("LUU") || m.Equals("MK1") || m.Equals("MK61") || m.Equals("WTU1B"))
            {
                return true;
            }
            return false;
        }

        private bool isMav(string m)
        {
            if (m.Contains("-65"))
            {
                return true;
            }
            return false;
        }

        private void enableGun(char rowNr)
        {
            disableAll(rowNr);
        }

        private void enableMav(char rowNr)
        {
            disableAll(rowNr);
        }

        private void disableAll(char rowNr)
        {
            disable("mode", rowNr);
            disable("fusing", rowNr);
            disable("SGLPAIR", rowNr);
            disable("RPL", rowNr);
            disable("Spacing", rowNr);
            disable("HOF", rowNr);
            disable("RPM", rowNr);
        }

        private void enableBomb(char rowNr)
        {
            enable("mode", rowNr);
            enable("fusing", rowNr);
            enable("SGLPAIR", rowNr);
            disable("RPL", rowNr);
            disable("Spacing", rowNr);
            disable("HOF", rowNr);
            disable("RPM", rowNr);
        }

        private void enableCBU(char rowNr)
        {
            enableBomb(rowNr);
            enable("HOF", rowNr);
            enable("RPM", rowNr);
        }

        private void enable(string obj, char rowNr)
        {
            Control[] list = this.Controls.Find(obj + rowNr, true);
            list[0].Enabled = true;
            Control[] label = this.Controls.Find("lbl" + obj, true);
            label[0].Enabled = true;
        }

        private void disable(string obj, char rowNr)
        {
            Control[] list = this.Controls.Find(obj + rowNr, true);
            list[0].Text = "";
            list[0].Enabled = false;
            Control[] label = this.Controls.Find("lbl" + obj, true);
            label[0].Enabled = false;

            // TODO: if mode1-4.Enabled == false, label[0].Enabled = false;
        }

        private char getNumber(object sender)
        {
            Control ctrl = (Control)sender;
            return ctrl.Name[ctrl.Name.Length - 1];
        }

        private void SGLPAIR_SelectedIndexChanged(object sender, EventArgs e)
        {
            char number = getNumber(sender);

            updateRowActivationSGLPAIR(number, sender);
        }
    }
}
