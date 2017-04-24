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
            initPackageDgv();
            initForm();
        }

        private void initForm()
        {
            txtCallsign.Text = Properties.Settings.Default.prevTxtCallsign;
            txtMsnNr.Text = Properties.Settings.Default.prevTxtMsnNr;
            txtJoker.Text = Properties.Settings.Default.prevTxtJoker;
            txtBingo.Text = Properties.Settings.Default.prevTxtBingo;
            cmbNrOfAc.Text = Properties.Settings.Default.prevTxtNrOfAc;

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

        private void initPackageDgv()
        {
            dgvPackage.RowCount = 6;
            dgvPackage.DefaultCellStyle.SelectionBackColor = dgvPackage.DefaultCellStyle.BackColor;
            dgvPackage.DefaultCellStyle.SelectionForeColor = dgvPackage.DefaultCellStyle.ForeColor;

            int i = 0;
            while (i < dgvPackage.Rows.Count)
            {
                initSupportRow(i);
                i++;
            }
        }

        private void initSupportRow(int i)
        {
            var row = dgvPackage.Rows[i];
            row.Cells["colCallsign"].Value = "-";
            row.Cells["colAircraftType"].Value = "-";
            row.Cells["colGid"].Value = "-";
            row.Cells["colTcn"].Value = "-";
            row.Cells["colTask"].Value = "-";
            row.Cells["colNotes"].Value = "-";

            row = dgvPackage.Rows[0];
            row.Cells["colTask"].Value = "SWEEP";
            row = dgvPackage.Rows[1];
            row.Cells["colTask"].Value = "SEAD";
            row = dgvPackage.Rows[2];
            row.Cells["colTask"].Value = "SEAD";
            row = dgvPackage.Rows[3];
            row.Cells["colTask"].Value = "AI";
            row = dgvPackage.Rows[4];
            row.Cells["colTask"].Value = "CAP";
            row = dgvPackage.Rows[5];
            row.Cells["colTask"].Value = "CAP";
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
            string pathA10c = @"\Kneeboard Groups\A-10C";
            captureScreen(Properties.Settings.Default.pathKneeboardBuilder + pathA10c);

            System.Windows.Forms.Application.Exit();
        }

        private void captureScreen(string path)
        {
            System.Drawing.Rectangle bounds = this.Bounds;
            using (Bitmap bitmap = new Bitmap(bounds.Width - 6, bounds.Height - 30))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new System.Drawing.Point(bounds.Left + 3, bounds.Top + 30), System.Drawing.Point.Empty, bounds.Size);
                }
                System.IO.Directory.CreateDirectory(path + @"\MDC");
                bitmap.Save(path + @"\MDC\MDC-002.png");
            }
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

        private bool isBomb(string m)
        {
            if (m.Contains("GBU") || m.Contains("CBU") || m.Contains("BDU") || m.Contains("MK-8"))
            {
                return true;
            }
            return false;
        }

        private bool isHighDrag(string m)
        {
            if (m.Equals("MK-82AIR") || m.Equals("BDU-50HD"))
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

            if (allDisabled(obj))
            {
                Control[] label = this.Controls.Find("lbl" + obj, true);
                label[0].Enabled = false;
            }
            // TODO: if mode1-4.Enabled == false, label[0].Enabled = false;
        }

        private bool allDisabled(string obj)
        {
            int rowNr = 1;
            while (rowNr < 5)
            {
                Control[] list = this.Controls.Find(obj + rowNr, true);
                if (list[0].Enabled)
                {
                    return false;
                }
                rowNr++;
            }
            return true;
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

        private void cbEgressCardinal_SelectedIndexChanged(object sender, EventArgs e)
        {
            const string N = "360";
            const string NE = "45";
            const string E = "90";
            const string SE = "135";
            const string S = "180";
            const string SW = "225";
            const string W = "270";
            const string NW = "315";

            char rowNr = getNumber(sender);
            ComboBox cardinal = (ComboBox)sender;
            if (cardinal.Text.Equals("N")) findObj("numEgressHeading", rowNr).Text = N;
            if (cardinal.Text.Equals("NE")) findObj("numEgressHeading", rowNr).Text = NE;
            if (cardinal.Text.Equals("E")) findObj("numEgressHeading", rowNr).Text = E;
            if (cardinal.Text.Equals("SE")) findObj("numEgressHeading", rowNr).Text = SE;
            if (cardinal.Text.Equals("S")) findObj("numEgressHeading", rowNr).Text = S;
            if (cardinal.Text.Equals("SW")) findObj("numEgressHeading", rowNr).Text = SW;
            if (cardinal.Text.Equals("W")) findObj("numEgressHeading", rowNr).Text = W;
            if (cardinal.Text.Equals("NW")) findObj("numEgressHeading", rowNr).Text = NW;
        }

        private void cbDelivery_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox delivery = sender as ComboBox;
            
            if (delivery.SelectedItem.Equals("Low Angle Strafe (LAS)")) delivery.Items[delivery.FindStringExact("Low Angle Strafe (LAS)")] = "LAS";
            if (delivery.SelectedItem.Equals("High Angle Strafe (HAS)")) delivery.Items[delivery.FindStringExact("High Angle Strafe (HAS)")] = "HAS";
            if (delivery.SelectedItem.Equals("Long Range Strafe (LRS)")) delivery.Items[delivery.FindStringExact("Long Range Strafe (LRS)")] = "LRS";
            if (delivery.SelectedItem.Equals("Two-Target Strafe (TTS)")) delivery.Items[delivery.FindStringExact("Two-Target Strafe (TTS)")] = "TTS";
            if (delivery.SelectedItem.Equals("Low Angle Rocket (LAR)")) delivery.Items[delivery.FindStringExact("Low Angle Rocket (LAR)")] = "LAR";
            if (delivery.SelectedItem.Equals("High Angle Rocket (HAR)")) delivery.Items[delivery.FindStringExact("High Angle Rocket (HAR)")] = "HAR";
            if (delivery.SelectedItem.Equals("Low Altitude Tactical Rocket (LATR)")) delivery.Items[delivery.FindStringExact("Low Altitude Tactical Rocket (LATR)")] = "LATR";
            if (delivery.SelectedItem.Equals("High Altitude Tactical Rocket (HATR)")) delivery.Items[delivery.FindStringExact("High Altitude Tactical Rocket (HATR)")] = "HATR";
            if (delivery.SelectedItem.Equals("High Altitude Release Rocket (HARR)")) delivery.Items[delivery.FindStringExact("High Altitude Release Rocket (HARR)")] = "HARR";
            if (delivery.SelectedItem.Equals("Loft Rocket (LR)")) delivery.Items[delivery.FindStringExact("Loft Rocket (LR)")] = "LR";
            if (delivery.SelectedItem.Equals("Visual Level Delivery (VLD)")) delivery.Items[delivery.FindStringExact("Visual Level Delivery (VLD)")] = "VLD";
            if (delivery.SelectedItem.Equals("Low Angle High Drag (LAHD)")) delivery.Items[delivery.FindStringExact("Low Angle High Drag (LAHD)")] = "LAHD";
            if (delivery.SelectedItem.Equals("Low Angle Low Drag (LALD)")) delivery.Items[delivery.FindStringExact("Low Angle Low Drag (LALD)")] = "LALD";
            if (delivery.SelectedItem.Equals("Dive Bomb (DB)")) delivery.Items[delivery.FindStringExact("Dive Bomb (DB)")] = "DB";
            if (delivery.SelectedItem.Equals("High Altitude Dive Bomb (HADB)")) delivery.Items[delivery.FindStringExact("High Altitude Dive Bomb (HADB)")] = "HADB";
            if (delivery.SelectedItem.Equals("High Altitude Release Bomb (HARB)")) delivery.Items[delivery.FindStringExact("High Altitude Release Bomb (HARB)")] = "HARB";
            if (delivery.SelectedItem.Equals("Low Altitude Toss (LAT)")) delivery.Items[delivery.FindStringExact("Low Altitude Toss (LAT)")] = "LAT";
            if (delivery.SelectedItem.Equals("Medium Altitude Toss (MAT)")) delivery.Items[delivery.FindStringExact("Medium Altitude Toss (MAT)")] = "MAT";
        }

        private void cbDelivery_DropDown(object sender, EventArgs e)
        {
            char rowNr = getNumber(sender);
            ComboBox d = sender as ComboBox;

            Control[] attack_profile = this.Controls.Find("txtProfileAttack" + rowNr, true);
            if (profileMatched(attack_profile))
            {
                Control[] control_Delivery = this.Controls.Find("cbDelivery" + rowNr, true);
                ComboBox munitions = getMunitions(attack_profile);
                setCollection(munitions.Text, control_Delivery[0] as ComboBox);
            }
        }

        private void setCollection(string munitions, ComboBox delivery)
        {
            const string LAS = "Low Angle Strafe (LAS)";
            const string HAS = "High Angle Strafe (HAS)";
            const string LRS = "Long Range Strafe (LRS)";
            const string TTS = "Two-Target Strafe (TTS)";

            const string LAR = "Low Angle Rocket(LAR)";
            const string HAR = "High Angle Rocket (HAR)";
            const string LATR = "Low Altitude Tactical Rocket (LATR)";
            const string HATR = "High Altitude Tactical Rocket (HATR)";
            const string HARR = "High Altitude Release Rocket(HARR)";
            const string LR = "Loft Rocket (LR)";

            const string VLD = "Visual Level Delivery (VLD)";
            const string LAHD = "Low Angle High Drag (LAHD)";
            const string LALD = "Low Angle Low Drag (LALD)";
            const string DB = "Dive Bomb (DB)";
            const string HADB = "High Altitude Dive Bomb (HADB)";
            const string HARB = "High Altitude Release Bomb (HARB)";
            const string LAT = "Low Altitude Toss (LAT)";
            const string MAT = "Medium Altitude Toss (MAT)";
            const string popup = "Popup";

            if (isRocket(munitions))
            {
                delivery.Items.Clear();

                delivery.Items.Add(LAR);
                delivery.Items.Add(HAR);
                delivery.Items.Add(LATR);
                delivery.Items.Add(HATR);
                delivery.Items.Add(HARR);
                delivery.Items.Add(LR);
                delivery.Items.Add(popup);
            }

            if (munitions.Contains("Gun"))
            {
                delivery.Items.Clear();

                delivery.Items.Add(LAS);
                delivery.Items.Add(HAS);
                delivery.Items.Add(LRS);
                delivery.Items.Add(TTS);
                delivery.Items.Add(popup);
            }

            if (isBomb(munitions))
            {
                delivery.Items.Clear();

                if (isHighDrag(munitions))
                {
                    delivery.Items.Add(LAHD);
                }
                
                delivery.Items.Add(VLD);
                delivery.Items.Add(LALD);
                delivery.Items.Add(DB);
                delivery.Items.Add(HADB);
                delivery.Items.Add(HARB);
                delivery.Items.Add(LAT);
                delivery.Items.Add(MAT);
                delivery.Items.Add(popup);
            }
        }

        private bool profileMatched(Control[] profile_a)
        {
            int rowNr = 1;
            while (rowNr < 5)
            {
                Control[] profile_b = this.Controls.Find("txtProfile" + rowNr.ToString(), true);
                if (profile_a[0].Text.Equals(profile_b[0].Text) && profile_a[0].Text != "")
                {
                    return true;
                }
                rowNr++;
            }
            return false;
        }

        private ComboBox getMunitions(Control[] profile_a)
        {
            int rowNr = 1;
            while (rowNr < 5)
            {
                Control[] profile_b = this.Controls.Find("txtProfile" + rowNr.ToString(), true);
                if (profile_a[0].Text.Equals(profile_b[0].Text) && profile_a[0].Text != "")
                {
                    return this.Controls.Find("Munitions" + rowNr, true)[0] as ComboBox;
                }
                rowNr++;
            }
            return null;
        }
    }
}
