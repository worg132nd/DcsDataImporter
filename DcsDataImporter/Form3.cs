using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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
        public List<ATO> ATOs;

        public Form3(string tac)
        {
            InitializeComponent();
            initTarget(tac);
            commonInit();
        }

        /* From form2 after back from form3 */
        public Form3()
        {
            InitializeComponent();
            initDgvs();
            loadForm();
            commonInit();
        }

        private void commonInit()
        {
            loadLasercode();
            parseATO();
        }

        private void parseATO()
        {
            ATOs = new List<ATO>();

            string path = Properties.Settings.Default.pathAto;

            using (StreamReader reader = new StreamReader(path))
            {
                string nrAc, type, callsignAndNr, priConf, secConf, priFreq, secFreq, tasking, timeFrom, timeTo, pos;
                nrAc = type = callsignAndNr = priConf = secConf = priFreq = secFreq = tasking = timeFrom = timeTo = pos = null;

                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();

                    // Parse tasking from AMSNDAT
                    if (line.StartsWith("AMSNDAT"))
                    {
                        int valueNr = 0;
                        foreach (string word in line.Split('/'))
                        {
                            if (valueNr == 2) tasking = word.Trim();
                            valueNr++;
                        }
                    } else if (line.StartsWith("MSNACFT"))
                    {
                        int valueNr = 0;
                        foreach (string word in line.Split('/'))
                        {
                            if (valueNr == 1) nrAc = word.Trim();
                            if (valueNr == 2) type = word.Trim();
                            if (valueNr == 3) callsignAndNr = word.Trim();
                            if (valueNr == 4) priConf = word.Trim();
                            if (valueNr == 5) secConf = word.Trim();
                            if (valueNr == 6) priFreq = word.Trim();
                            if (valueNr == 7) secFreq = word.Trim();

                            valueNr++;
                        }
                        if (isFlight(type) && !isCallsignMe(callsignAndNr))
                        {
                            /*MessageBox.Show( //debug
                            "nrAc: " + nrAc + '\n'
                            + "type: " + type + '\n'
                            + "callsignAndNr: " + callsignAndNr + '\n'
                            + "priConf: " + priConf + '\n'
                            + "secConf: " + secConf + '\n'
                            + "priFreq: " + priFreq + '\n'
                            + "secFreq: " + secFreq + '\n'
                            + "tasking: " + tasking);*/

                            ATO ato = new ATO(nrAc, type, callsignAndNr, priConf, secConf, priFreq, secFreq, tasking);
                            ATOs.Add(ato);
                        }
                        tasking = null;
                        timeFrom = null;
                        timeTo = null;
                        pos = null;
                    } else if (line.StartsWith("AMSNLOC"))
                    {
                        int valueNr = 0;
                        foreach (string word in line.Split('/'))
                        {
                            if (valueNr == 1) timeFrom = word.Trim();
                            if (valueNr == 2) timeTo = word.Trim();
                            if (valueNr == 3) pos = word.Trim();

                            valueNr++;
                        }

                        ATO ato = ATOs.Last();
                        ato.setTimeFrom(timeFrom);
                        ato.setTimeTo(timeTo);
                        ato.setPos(pos);
                    }
                }
            }

            // after all flights have been added, add all A-10s to the datagrid
            foreach (ATO ato in ATOs)
            {
                if (ato.getTypeOfAc().ToUpper().Trim().Equals("A-10C"))
                {
                    setPkgFlight(ato);
                }
            }

            // add the rest
            foreach (ATO ato in ATOs)
            {
                if (!ato.getTypeOfAc().ToUpper().Trim().Equals("A-10C"))
                {
                    setPkgFlight(ato);
                }
            }

            if (ATOs.Count > dgvPackage.Rows.Count)
            {
                MessageBox.Show("Too many flights to show all!");
            }
        }

        private bool isCallsignMe(string callsignAndNr)
        {
            string justCallsignMe = new string(txtCallsign.Text.Where(Char.IsLetter).ToArray()).ToUpper();
            string justCallsignAto = new string(callsignAndNr.Where(Char.IsLetter).ToArray()).ToUpper();

            if (justCallsignMe.Equals(justCallsignAto))
            {
                return true;
            }
            return false;
        }

        // find first free row
        // write ato to datagrid
        private void setPkgFlight(ATO ato)
        {
            int rowNr = 0;

            while (rowNr < dgvPackage.Rows.Count)
            {
                var row = dgvPackage.Rows[rowNr];

                if (row.Cells["colCallsign"].Value.ToString() == "-")
                {
                    row.Cells["colCallsign"].Value = ato.getCallsignAndNr();
                    row.Cells["colAircraftType"].Value = ato.getTypeOfAc();
                    row.Cells["colTask"].Value = ato.getTasking();

                    // First, clear out comment
                    if (row.Cells["colNotes"].Value.ToString() == "-")
                    {
                        row.Cells["colNotes"].Value = "";
                    }


                    if (ato.getTimeFrom() != null)
                    {
                        row.Cells["colNotes"].Value += ato.getTimeFrom();
                    }

                    if (ato.getTimeTo() != null)
                    {
                        row.Cells["colNotes"].Value += " - " + ato.getTimeTo();
                    }

                    if (ato.getPos() != null)
                    {
                        row.Cells["colNotes"].Value += " " + ato.getPos();
                    }

                    break;
                }
                rowNr++;
            }
        }

        private bool isFlight(string type)
        {
            type = type.ToLower();

            if (!type.Equals("ground") && (!type.Equals("e-3")) && (!type.Equals("e-2"))) {
                return true;
            }
            return false;
        }

        private void loadLasercode()
        {
            int acNr = Int32.Parse(Properties.Settings.Default.prevTxtCallsign.Last().ToString());

            switch (acNr)
            {
                case 1:
                    lblLaser.Text = Properties.Settings.Default.prevColLsrLead;
                    break;
                case 2:
                    lblLaser.Text = Properties.Settings.Default.prevColLsrWing;
                    break;
                case 3:
                    lblLaser.Text = Properties.Settings.Default.prevColLsrElement;
                    break;
                case 4:
                    lblLaser.Text = Properties.Settings.Default.prevColLsrTrail;
                    break;
            }
        }

        private void initDgvs()
        {
            initTgtDgv(dgvTgtLead);
            initTgtDgv(dgvTgtElem);
            initPackageDgv();
        }

        private void initTarget(string tac)
        {
            initDgvs();
            
            // Disable second element if less than 3 in the flight
            if (Int32.Parse(Properties.Settings.Default.prevTxtNrOfAc) < 3){
                disableSecElem();
                moveSecElemUp(132);
                moveFirstElemUp(7);
                adjGuiSglElem();
            }

            initProfiles();
            initForm(tac);
        }

        private void adjGuiSglElem()
        {
            dgvPackage.Top -= 20;
            lblPackage.Top -= 20;
            moveProfilesUp(10);
        }

        private void moveProfilesUp(int h)
        {
            lblProfile.Top -= h;
            txtProfile1.Top -= h;
            txtProfile2.Top -= h;
            txtProfile3.Top -= h;
            txtProfile4.Top -= h;

            lblMunitions.Top -= h;
            Munitions1.Top -= h;
            Munitions2.Top -= h;
            Munitions3.Top -= h;
            Munitions4.Top -= h;

            lblMode.Top -= h;
            Mode1.Top -= h;
            Mode2.Top -= h;
            Mode3.Top -= h;
            Mode4.Top -= h;

            lblFusing.Top -= h;
            Fusing1.Top -= h;
            Fusing2.Top -= h;
            Fusing3.Top -= h;
            Fusing4.Top -= h;

            lblSGLPAIR.Top -= h;
            SGLPAIR1.Top -= h;
            SGLPAIR2.Top -= h;
            SGLPAIR3.Top -= h;
            SGLPAIR4.Top -= h;

            lblRPL.Top -= h;
            RPL1.Top -= h;
            RPL2.Top -= h;
            RPL3.Top -= h;
            RPL4.Top -= h;

            lblSpacing.Top -= h;
            Spacing1.Top -= h;
            Spacing2.Top -= h;
            Spacing3.Top -= h;
            Spacing4.Top -= h;

            lblHOF.Top -= h;
            HOF1.Top -= h;
            HOF2.Top -= h;
            HOF3.Top -= h;
            HOF4.Top -= h;

            lblRPM.Top -= h;
            RPM1.Top -= h;
            RPM2.Top -= h;
            RPM3.Top -= h;
            RPM4.Top -= h;
        }

        private void disableSecElem()
        {
            lblElement2.Hide();
            lblAttack2.Hide();
            lblRelease2.Hide();
            lblEgress2.Hide();
            lblProfile2.Hide();
            lblDelivery2.Hide();
            lblIpAttack2.Hide();
            lblScSs2.Hide();
            lblFormation2.Hide();
            lblFAH2.Hide();
            lblAltitude2.Hide();
            lblAbort2.Hide();
            lblSem2.Hide();
            lblLeftRight2.Hide();
            lblCardinal2.Hide();
            lblHeading2.Hide();
            lblIpEgress2.Hide();
            tgtPanel.Height = 233;
            dgvTgtElem.Hide();
            lblElement1.Hide();
        }

        private void moveSecElemUp(int h)
        {
            lblElement2.Top -= h;
            lblAttack2.Top -= h;
            lblRelease2.Top -= h;
            lblEgress2.Top -= h;
            lblProfile2.Top -= h;
            lblDelivery2.Top -= h;
            lblIpAttack2.Top -= h;
            lblScSs2.Top -= h;
            lblFormation2.Top -= h;
            lblFAH2.Top -= h;
            lblAltitude2.Top -= h;
            lblAbort2.Top -= h;
            lblSem2.Top -= h;
            lblLeftRight2.Top -= h;
            lblCardinal2.Top -= h;
            lblHeading2.Top -= h;
            lblIpEgress2.Top -= h;

            txtProfileAttack3.Top -= h;
            txtProfileAttack4.Top -= h;
            cbDelivery3.Top -= h;
            cbDelivery4.Top -= h;
            txtAttackIP3.Top -= h;
            txtAttackIP4.Top -= h;
            cbSCSS3.Top -= h;
            cbSCSS4.Top -= h;
            cbFormation3.Top -= h;
            cbFormation4.Top -= h;
            numFAH3.Top -= h;
            numFAH4.Top -= h;
            txtAltitude3.Top -= h;
            txtAltitude4.Top -= h;
            txtAbort3.Top -= h;
            txtAbort4.Top -= h;
            cbSem3.Top -= h;
            cbSem4.Top -= h;
            cbEgressDirection3.Top -= h;
            cbEgressDirection4.Top -= h;
            cbEgressCardinal3.Top -= h;
            cbEgressCardinal4.Top -= h;
            numEgressHeading3.Top -= h;
            numEgressHeading4.Top -= h;
            txtEgressIP3.Top -= h;
            txtEgressIP4.Top -= h;

            moveBottomTgtDelimiter();
        }

        private void moveBottomTgtDelimiter()
        {
            panel2.Top = tgtPanel.Height + tgtPanel.Location.Y;
            panel3.Top = tgtPanel.Height + tgtPanel.Location.Y + 1;
        }

        private void moveFirstElemUp(int h)
        {
            lblElement1.Top -= h;
            lblAttack1.Top -= h;
            lblRelease1.Top -= h;
            lblEgress1.Top -= h;
            lblProfile1.Top -= h;
            lblDelivery1.Top -= h;
            lblIpAttack1.Top -= h;
            lblScSs1.Top -= h;
            lblFormation1.Top -= h;
            lblFAH1.Top -= h;
            lblAltitude1.Top -= h;
            lblAbort1.Top -= h;
            lblSem1.Top -= h;
            lblLeftRight1.Top -= h;
            lblCardinal1.Top -= h;
            lblHeading1.Top -= h;
            lblIpEgress1.Top -= h;

            txtProfileAttack1.Top -= h;
            txtProfileAttack2.Top -= h;
            cbDelivery1.Top -= h;
            cbDelivery2.Top -= h;
            txtAttackIP1.Top -= h;
            txtAttackIP2.Top -= h;
            cbSCSS1.Top -= h;
            cbSCSS2.Top -= h;
            cbFormation1.Top -= h;
            cbFormation2.Top -= h;
            numFAH1.Top -= h;
            numFAH2.Top -= h;
            txtAltitude1.Top -= h;
            txtAltitude2.Top -= h;
            txtAbort1.Top -= h;
            txtAbort2.Top -= h;
            cbSem1.Top -= h;
            cbSem2.Top -= h;
            cbEgressDirection1.Top -= h;
            cbEgressDirection2.Top -= h;
            cbEgressCardinal1.Top -= h;
            cbEgressCardinal2.Top -= h;
            numEgressHeading1.Top -= h;
            numEgressHeading2.Top -= h;
            txtEgressIP1.Top -= h;
            txtEgressIP2.Top -= h;

            // moving top of datagrid view
            dgvTgtLead.Top -= h;

            // space between sets of rows (pairs of rows)
            h = h - 7;

            txtProfileAttack3.Top -= h;
            txtProfileAttack4.Top -= h;
            cbDelivery3.Top -= h;
            cbDelivery4.Top -= h;
            txtAttackIP3.Top -= h;
            txtAttackIP4.Top -= h;
            cbSCSS3.Top -= h;
            cbSCSS4.Top -= h;
            cbFormation3.Top -= h;
            cbFormation4.Top -= h;
            numFAH3.Top -= h;
            numFAH4.Top -= h;
            txtAltitude3.Top -= h;
            txtAltitude4.Top -= h;
            txtAbort3.Top -= h;
            txtAbort4.Top -= h;
            cbSem3.Top -= h;
            cbSem4.Top -= h;
            cbEgressDirection3.Top -= h;
            cbEgressDirection4.Top -= h;
            cbEgressCardinal3.Top -= h;
            cbEgressCardinal4.Top -= h;
            numEgressHeading3.Top -= h;
            numEgressHeading4.Top -= h;
            txtEgressIP3.Top -= h;
            txtEgressIP4.Top -= h;
        }

        private void initForm(string tac)
        {
            txtCallsign.Text = Properties.Settings.Default.prevTxtCallsign;
            txtMsnNr.Text = Properties.Settings.Default.prevTxtMsnNr;
            txtJoker.Text = Properties.Settings.Default.prevTxtJoker;
            txtBingo.Text = Properties.Settings.Default.prevTxtBingo;
            cmbNrOfAc.Text = Properties.Settings.Default.prevTxtNrOfAc;
            txtTac.Text = tac;
            txtNotes.Text = Properties.Settings.Default.prevAmpn;
            txtVulStart.Text = Properties.Settings.Default.prevTxtVulStart;
            txtVulEnd.Text = Properties.Settings.Default.prevTxtVulEnd;
        }

        private void initTgtDgv(DataGridView dgv)
        {
            dgv.RowCount = 3;
            dgv.DefaultCellStyle.SelectionBackColor = dgv.DefaultCellStyle.BackColor;
            dgv.DefaultCellStyle.SelectionForeColor = dgv.DefaultCellStyle.ForeColor;

            var row = dgv.Rows[0];
            row.Cells[0].Value = "Primary";
            row.Cells[0].Style.Font = new Font(dgvTgtLead.DefaultCellStyle.Font, FontStyle.Bold);
            row.Cells[2].Value = "Secondary";
            row.Cells[2].Style.Font = new Font(dgvTgtLead.DefaultCellStyle.Font, FontStyle.Bold);
            row = dgv.Rows[1];
            row.Cells[0].Value = row.Cells[2].Value = "DMPI";
            row = dgv.Rows[2];
            row.Cells[0].Value = row.Cells[2].Value = "Coordinates";
        }

        private void initPackageDgv()
        {
            dgvPackage.RowCount = 7;
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
            row.Cells["colTask"].Value = "-";
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
            lblVulEnd.Focus(); // Move focus away so no blue markings appear on screenshot
            System.Threading.Thread.Sleep(50); // sleep to make sure focus is moved

            btnSubmit.Hide();
            btnBack.Hide();

            string pathA10c = @"\Kneeboard Groups\A-10C";
            captureScreen(Properties.Settings.Default.pathKneeboardBuilder + pathA10c);

            Properties.Settings.Default.prev_training = Form1.MyGlobals.global_training;

            // save settings is critical to be able to save settings before application closes:
            // Solved bug with Lsr not being saved between sessions. DO NOT DELETE!
            Properties.Settings.Default.Save();

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

                if (Form1.MyGlobals.global_training == true)
                {
                    System.IO.Directory.CreateDirectory(path + @"\TR");
                    bitmap.Save(path + @"\TR\TR-006.png");
                }
                else
                {
                    System.IO.Directory.CreateDirectory(path + @"\MSN");
                    bitmap.Save(path + @"\MSN\MSN-006.png");
                }

                /*System.IO.Directory.CreateDirectory(path + @"\MDC");
                bitmap.Save(path + @"\MDC\MDC-002.png");*/
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            /* Todo: add back functionality */

            saveForm();

            Form2 form2 = new Form2();
            form2.Show();
            Hide();
        }

        private void saveForm()
        {
            Properties.Settings.Default.prevForm3TxtTac = txtTac.Text;
            Properties.Settings.Default.prevForm3TxtCallsign = txtCallsign.Text;
            Properties.Settings.Default.prevForm3TxtMsnNr = txtMsnNr.Text;
            Properties.Settings.Default.prevForm3CmbNrOfAc = cmbNrOfAc.Text;
            Properties.Settings.Default.prevForm3NumPlaytime = numPlaytime.Text;
            Properties.Settings.Default.prevForm3TxtLoadout = txtLoadout.Text;
            Properties.Settings.Default.prevForm3CbAbort = cbAbort.Text;

            Properties.Settings.Default.prevForm3LblLaser = lblLaser.Text;

            saveDgvElem1();

            Properties.Settings.Default.prevForm3TxtProfileAttack1 = txtProfileAttack1.Text;
            Properties.Settings.Default.prevForm3TxtProfileAttack2 = txtProfileAttack2.Text;
            Properties.Settings.Default.prevForm3TxtProfileAttack3 = txtProfileAttack3.Text;
            Properties.Settings.Default.prevForm3TxtProfileAttack4 = txtProfileAttack4.Text;
            Properties.Settings.Default.prevForm3CbDelivery1 = cbDelivery1.Text;
            Properties.Settings.Default.prevForm3CbDelivery2 = cbDelivery2.Text;
            Properties.Settings.Default.prevForm3CbDelivery3 = cbDelivery3.Text;
            Properties.Settings.Default.prevForm3CbDelivery4 = cbDelivery4.Text;
            Properties.Settings.Default.prevForm3TxtAttackIP1 = txtAttackIP1.Text;
            Properties.Settings.Default.prevForm3TxtAttackIP2 = txtAttackIP2.Text;
            Properties.Settings.Default.prevForm3TxtAttackIP3 = txtAttackIP3.Text;
            Properties.Settings.Default.prevForm3TxtAttackIP4 = txtAttackIP4.Text;
            Properties.Settings.Default.prevForm3CbSCSS1 = cbSCSS1.Text;
            Properties.Settings.Default.prevForm3CbSCSS1 = cbSCSS2.Text;
            Properties.Settings.Default.prevForm3CbSCSS3 = cbSCSS3.Text;
            Properties.Settings.Default.prevForm3CbSCSS4 = cbSCSS4.Text;
            Properties.Settings.Default.prevForm3CbFormation1 = cbFormation1.Text;
            Properties.Settings.Default.prevForm3CbFormation2 = cbFormation2.Text;
            Properties.Settings.Default.prevForm3CbFormation3 = cbFormation3.Text;
            Properties.Settings.Default.prevForm3CbFormation4 = cbFormation4.Text;
            Properties.Settings.Default.prevForm3NumFAH1 = numFAH1.Text;
            Properties.Settings.Default.prevForm3NumFAH2 = numFAH2.Text;
            Properties.Settings.Default.prevForm3NumFAH3 = numFAH3.Text;
            Properties.Settings.Default.prevForm3NumFAH4 = numFAH4.Text;
            Properties.Settings.Default.prevForm3TxtAltitude1 = txtAltitude1.Text;
            Properties.Settings.Default.prevForm3TxtAltitude2 = txtAltitude2.Text;
            Properties.Settings.Default.prevForm3TxtAltitude3 = txtAltitude3.Text;
            Properties.Settings.Default.prevForm3TxtAltitude4 = txtAltitude4.Text;
            Properties.Settings.Default.prevForm3TxtAbort1 = txtAbort1.Text;
            Properties.Settings.Default.prevForm3TxtAbort2 = txtAbort2.Text;
            Properties.Settings.Default.prevForm3TxtAbort3 = txtAbort3.Text;
            Properties.Settings.Default.prevForm3TxtAbort4 = txtAbort4.Text;
            Properties.Settings.Default.prevForm3CbSem1 = cbSem1.Text;
            Properties.Settings.Default.prevForm3CbSem2 = cbSem2.Text;
            Properties.Settings.Default.prevForm3CbSem3 = cbSem3.Text;
            Properties.Settings.Default.prevForm3CbSem4 = cbSem4.Text;
            Properties.Settings.Default.prevForm3CbEgressDirection1 = cbEgressDirection1.Text;
            Properties.Settings.Default.prevForm3CbEgressDirection2 = cbEgressDirection2.Text;
            Properties.Settings.Default.prevForm3CbEgressDirection3 = cbEgressDirection3.Text;
            Properties.Settings.Default.prevForm3CbEgressDirection4 = cbEgressDirection4.Text;
            Properties.Settings.Default.prevForm3CbEgressCardinal1 = cbEgressCardinal1.Text;
            Properties.Settings.Default.prevForm3CbEgressCardinal2 = cbEgressCardinal2.Text;
            Properties.Settings.Default.prevForm3CbEgressCardinal3 = cbEgressCardinal3.Text;
            Properties.Settings.Default.prevForm3CbEgressCardinal4 = cbEgressCardinal4.Text;
            Properties.Settings.Default.prevForm3NumEgressHeading1 = numEgressHeading1.Text;
            Properties.Settings.Default.prevForm3NumEgressHeading2 = numEgressHeading2.Text;
            Properties.Settings.Default.prevForm3NumEgressHeading3 = numEgressHeading3.Text;
            Properties.Settings.Default.prevForm3NumEgressHeading4 = numEgressHeading4.Text;
            Properties.Settings.Default.prevForm3TxtEgressIP1 = txtEgressIP1.Text;
            Properties.Settings.Default.prevForm3TxtEgressIP2 = txtEgressIP2.Text;
            Properties.Settings.Default.prevForm3TxtEgressIP3 = txtEgressIP3.Text;
            Properties.Settings.Default.prevForm3TxtEgressIP4 = txtEgressIP4.Text;
            Properties.Settings.Default.prevForm3TxtProfile1 = txtProfile1.Text;
            Properties.Settings.Default.prevForm3TxtProfile2 = txtProfile2.Text;
            Properties.Settings.Default.prevForm3TxtProfile3 = txtProfile3.Text;
            Properties.Settings.Default.prevForm3TxtProfile4 = txtProfile4.Text;
            Properties.Settings.Default.prevForm3Munitions1 = Munitions1.Text;
            Properties.Settings.Default.prevForm3Munitions2 = Munitions2.Text;
            Properties.Settings.Default.prevForm3Munitions3 = Munitions3.Text;
            Properties.Settings.Default.prevForm3Munitions4 = Munitions4.Text;
            Properties.Settings.Default.prevForm3Mode1 = Mode1.Text;
            Properties.Settings.Default.prevForm3Mode2 = Mode2.Text;
            Properties.Settings.Default.prevForm3Mode3 = Mode3.Text;
            Properties.Settings.Default.prevForm3Mode4 = Mode4.Text;
            Properties.Settings.Default.prevForm3Fusing1 = Fusing1.Text;
            Properties.Settings.Default.prevForm3Fusing2 = Fusing2.Text;
            Properties.Settings.Default.prevForm3Fusing3 = Fusing3.Text;
            Properties.Settings.Default.prevForm3Fusing4 = Fusing4.Text;
            Properties.Settings.Default.prevForm3SGLPAIR1 = SGLPAIR1.Text;
            Properties.Settings.Default.prevForm3SGLPAIR2 = SGLPAIR2.Text;
            Properties.Settings.Default.prevForm3SGLPAIR3 = SGLPAIR3.Text;
            Properties.Settings.Default.prevForm3SGLPAIR4 = SGLPAIR4.Text;
            Properties.Settings.Default.prevForm3RPL1 = RPL1.Text;
            Properties.Settings.Default.prevForm3RPL2 = RPL2.Text;
            Properties.Settings.Default.prevForm3RPL3 = RPL3.Text;
            Properties.Settings.Default.prevForm3RPL4 = RPL4.Text;
            Properties.Settings.Default.prevForm3Spacing1 = Spacing1.Text;
            Properties.Settings.Default.prevForm3Spacing2 = Spacing2.Text;
            Properties.Settings.Default.prevForm3Spacing3 = Spacing3.Text;
            Properties.Settings.Default.prevForm3Spacing4 = Spacing4.Text;
            Properties.Settings.Default.prevForm3HOF1 = HOF1.Text;
            Properties.Settings.Default.prevForm3HOF2 = HOF2.Text;
            Properties.Settings.Default.prevForm3HOF3 = HOF3.Text;
            Properties.Settings.Default.prevForm3HOF4 = HOF4.Text;
            Properties.Settings.Default.prevForm3RPM1 = RPM1.Text;
            Properties.Settings.Default.prevForm3RPM2 = RPM2.Text;
            Properties.Settings.Default.prevForm3RPM3 = RPM3.Text;
            Properties.Settings.Default.prevForm3RPM4 = RPM4.Text;
            Properties.Settings.Default.prevForm3TxtNotes = txtNotes.Text;
            Properties.Settings.Default.prevForm3TxtJoker = txtJoker.Text;
            Properties.Settings.Default.prevForm3TxtBingo = txtBingo.Text;
            Properties.Settings.Default.prevForm3TxtVulStart = txtVulStart.Text;
            Properties.Settings.Default.prevForm3TxtVulEnd = txtVulEnd.Text;

            saveDgvElem2();

            saveDgvPackage();
        }

        private void loadForm()
        {
            txtTac.Text = Properties.Settings.Default.prevForm3TxtTac;
            txtCallsign.Text = Properties.Settings.Default.prevForm3TxtCallsign;
            txtMsnNr.Text = Properties.Settings.Default.prevForm3TxtMsnNr;
            cmbNrOfAc.Text = Properties.Settings.Default.prevForm3CmbNrOfAc;
            numPlaytime.Text = Properties.Settings.Default.prevForm3NumPlaytime;
            txtLoadout.Text = Properties.Settings.Default.prevForm3TxtLoadout;
            cbAbort.Text = Properties.Settings.Default.prevForm3CbAbort;

            lblLaser.Text = Properties.Settings.Default.prevForm3LblLaser;

            loadDgvElem1();

            txtProfileAttack1.Text = Properties.Settings.Default.prevForm3TxtProfileAttack1;
            txtProfileAttack2.Text = Properties.Settings.Default.prevForm3TxtProfileAttack2;
            txtProfileAttack3.Text = Properties.Settings.Default.prevForm3TxtProfileAttack3;
            txtProfileAttack4.Text = Properties.Settings.Default.prevForm3TxtProfileAttack4;
            cbDelivery1.Text = Properties.Settings.Default.prevForm3CbDelivery1;
            cbDelivery2.Text = Properties.Settings.Default.prevForm3CbDelivery2;
            cbDelivery3.Text = Properties.Settings.Default.prevForm3CbDelivery3;
            cbDelivery4.Text = Properties.Settings.Default.prevForm3CbDelivery4;
            txtAttackIP1.Text = Properties.Settings.Default.prevForm3TxtAttackIP1;
            txtAttackIP2.Text = Properties.Settings.Default.prevForm3TxtAttackIP2;
            txtAttackIP3.Text = Properties.Settings.Default.prevForm3TxtAttackIP3;
            txtAttackIP4.Text = Properties.Settings.Default.prevForm3TxtAttackIP4;
            cbSCSS1.Text = Properties.Settings.Default.prevForm3CbSCSS1;
            cbSCSS2.Text = Properties.Settings.Default.prevForm3CbSCSS1;
            cbSCSS3.Text = Properties.Settings.Default.prevForm3CbSCSS3;
            cbSCSS4.Text = Properties.Settings.Default.prevForm3CbSCSS4;
            cbFormation1.Text = Properties.Settings.Default.prevForm3CbFormation1;
            cbFormation2.Text = Properties.Settings.Default.prevForm3CbFormation2;
            cbFormation3.Text = Properties.Settings.Default.prevForm3CbFormation3;
            cbFormation4.Text = Properties.Settings.Default.prevForm3CbFormation4;
            numFAH1.Text = Properties.Settings.Default.prevForm3NumFAH1;
            numFAH2.Text = Properties.Settings.Default.prevForm3NumFAH2;
            numFAH3.Text = Properties.Settings.Default.prevForm3NumFAH3;
            numFAH4.Text = Properties.Settings.Default.prevForm3NumFAH4;
            txtAltitude1.Text = Properties.Settings.Default.prevForm3TxtAltitude1;
            txtAltitude2.Text = Properties.Settings.Default.prevForm3TxtAltitude2;
            txtAltitude3.Text = Properties.Settings.Default.prevForm3TxtAltitude3;
            txtAltitude4.Text = Properties.Settings.Default.prevForm3TxtAltitude4;
            txtAbort1.Text = Properties.Settings.Default.prevForm3TxtAbort1;
            txtAbort2.Text = Properties.Settings.Default.prevForm3TxtAbort2;
            txtAbort3.Text = Properties.Settings.Default.prevForm3TxtAbort3;
            txtAbort4.Text = Properties.Settings.Default.prevForm3TxtAbort4;
            cbSem1.Text = Properties.Settings.Default.prevForm3CbSem1;
            cbSem2.Text = Properties.Settings.Default.prevForm3CbSem2;
            cbSem3.Text = Properties.Settings.Default.prevForm3CbSem3;
            cbSem4.Text = Properties.Settings.Default.prevForm3CbSem4;
            cbEgressDirection1.Text = Properties.Settings.Default.prevForm3CbEgressDirection1;
            cbEgressDirection2.Text = Properties.Settings.Default.prevForm3CbEgressDirection2;
            cbEgressDirection3.Text = Properties.Settings.Default.prevForm3CbEgressDirection3;
            cbEgressDirection4.Text = Properties.Settings.Default.prevForm3CbEgressDirection4;
            cbEgressCardinal1.Text = Properties.Settings.Default.prevForm3CbEgressCardinal1;
            cbEgressCardinal2.Text = Properties.Settings.Default.prevForm3CbEgressCardinal2;
            cbEgressCardinal3.Text = Properties.Settings.Default.prevForm3CbEgressCardinal3;
            cbEgressCardinal4.Text = Properties.Settings.Default.prevForm3CbEgressCardinal4;
            numEgressHeading1.Text = Properties.Settings.Default.prevForm3NumEgressHeading1;
            numEgressHeading2.Text = Properties.Settings.Default.prevForm3NumEgressHeading2;
            numEgressHeading3.Text = Properties.Settings.Default.prevForm3NumEgressHeading3;
            numEgressHeading4.Text = Properties.Settings.Default.prevForm3NumEgressHeading4;
            txtEgressIP1.Text = Properties.Settings.Default.prevForm3TxtEgressIP1;
            txtEgressIP2.Text = Properties.Settings.Default.prevForm3TxtEgressIP2;
            txtEgressIP3.Text = Properties.Settings.Default.prevForm3TxtEgressIP3;
            txtEgressIP4.Text = Properties.Settings.Default.prevForm3TxtEgressIP4;
            txtProfile1.Text = Properties.Settings.Default.prevForm3TxtProfile1;
            txtProfile2.Text = Properties.Settings.Default.prevForm3TxtProfile2;
            txtProfile3.Text = Properties.Settings.Default.prevForm3TxtProfile3;
            txtProfile4.Text = Properties.Settings.Default.prevForm3TxtProfile4;
            Munitions1.Text = Properties.Settings.Default.prevForm3Munitions1;
            Munitions2.Text = Properties.Settings.Default.prevForm3Munitions2;
            Munitions3.Text = Properties.Settings.Default.prevForm3Munitions3;
            Munitions4.Text = Properties.Settings.Default.prevForm3Munitions4;
            Mode1.Text = Properties.Settings.Default.prevForm3Mode1;
            Mode2.Text = Properties.Settings.Default.prevForm3Mode2;
            Mode3.Text = Properties.Settings.Default.prevForm3Mode3;
            Mode4.Text = Properties.Settings.Default.prevForm3Mode4;
            Fusing1.Text = Properties.Settings.Default.prevForm3Fusing1;
            Fusing2.Text = Properties.Settings.Default.prevForm3Fusing2;
            Fusing3.Text = Properties.Settings.Default.prevForm3Fusing3;
            Fusing4.Text = Properties.Settings.Default.prevForm3Fusing4;
            SGLPAIR1.Text = Properties.Settings.Default.prevForm3SGLPAIR1;
            SGLPAIR2.Text = Properties.Settings.Default.prevForm3SGLPAIR2;
            SGLPAIR3.Text = Properties.Settings.Default.prevForm3SGLPAIR3;
            SGLPAIR4.Text = Properties.Settings.Default.prevForm3SGLPAIR4;
            RPL1.Text = Properties.Settings.Default.prevForm3RPL1;
            RPL2.Text = Properties.Settings.Default.prevForm3RPL2;
            RPL3.Text = Properties.Settings.Default.prevForm3RPL3;
            RPL4.Text = Properties.Settings.Default.prevForm3RPL4;
            Spacing1.Text = Properties.Settings.Default.prevForm3Spacing1;
            Spacing2.Text = Properties.Settings.Default.prevForm3Spacing2;
            Spacing3.Text = Properties.Settings.Default.prevForm3Spacing3;
            Spacing4.Text = Properties.Settings.Default.prevForm3Spacing4;
            HOF1.Text = Properties.Settings.Default.prevForm3HOF1;
            HOF2.Text = Properties.Settings.Default.prevForm3HOF2;
            HOF3.Text = Properties.Settings.Default.prevForm3HOF3;
            HOF4.Text = Properties.Settings.Default.prevForm3HOF4;
            RPM1.Text = Properties.Settings.Default.prevForm3RPM1;
            RPM2.Text = Properties.Settings.Default.prevForm3RPM2;
            RPM3.Text = Properties.Settings.Default.prevForm3RPM3;
            RPM4.Text = Properties.Settings.Default.prevForm3RPM4;
            txtNotes.Text = Properties.Settings.Default.prevForm3TxtNotes;
            txtJoker.Text = Properties.Settings.Default.prevForm3TxtJoker;
            txtBingo.Text = Properties.Settings.Default.prevForm3TxtBingo;
            txtVulStart.Text = Properties.Settings.Default.prevForm3TxtVulStart;
            txtVulEnd.Text = Properties.Settings.Default.prevForm3TxtVulEnd;

            loadDgvElem2();

            loadDgvPackage();

        }

        private void saveDgvElem1()
        {
            var row = dgvTgtLead.Rows[0];

            Properties.Settings.Default.e1r0c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.e1r0c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.e1r0c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.e1r0c4 = row.Cells[3].Value as string;

            row = dgvTgtLead.Rows[1];

            Properties.Settings.Default.e1r1c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.e1r1c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.e1r1c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.e1r1c4 = row.Cells[3].Value as string;

            row = dgvTgtLead.Rows[2];

            Properties.Settings.Default.e1r2c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.e1r2c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.e1r2c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.e1r2c4 = row.Cells[3].Value as string;
        }

        private void saveDgvElem2()
        {
            var row = dgvTgtElem.Rows[0];

            Properties.Settings.Default.e2r0c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.e2r0c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.e2r0c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.e2r0c4 = row.Cells[3].Value as string;

            row = dgvTgtElem.Rows[1];

            Properties.Settings.Default.e2r1c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.e2r1c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.e2r1c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.e2r1c4 = row.Cells[3].Value as string;

            row = dgvTgtElem.Rows[2];

            Properties.Settings.Default.e2r2c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.e2r2c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.e2r2c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.e2r2c4 = row.Cells[3].Value as string;
        }

        private void loadDgvElem1()
        {
            var row = dgvTgtLead.Rows[0];

            row.Cells[0].Value = Properties.Settings.Default.e1r0c1;
            row.Cells[1].Value = Properties.Settings.Default.e1r0c2;
            row.Cells[2].Value = Properties.Settings.Default.e1r0c3;
            row.Cells[3].Value = Properties.Settings.Default.e1r0c4;

            row = dgvTgtLead.Rows[1];

            row.Cells[0].Value = Properties.Settings.Default.e1r1c1;
            row.Cells[1].Value = Properties.Settings.Default.e1r1c2;
            row.Cells[2].Value = Properties.Settings.Default.e1r1c3;
            row.Cells[3].Value = Properties.Settings.Default.e1r1c4;

            row = dgvTgtLead.Rows[2];

            row.Cells[0].Value = Properties.Settings.Default.e1r2c1;
            row.Cells[1].Value = Properties.Settings.Default.e1r2c2;
            row.Cells[2].Value = Properties.Settings.Default.e1r2c3;
            row.Cells[3].Value = Properties.Settings.Default.e1r2c4;
        }

        private void loadDgvElem2()
        {
            var row = dgvTgtElem.Rows[0];

            row.Cells[0].Value = Properties.Settings.Default.e2r0c1;
            row.Cells[1].Value = Properties.Settings.Default.e2r0c2;
            row.Cells[2].Value = Properties.Settings.Default.e2r0c3;
            row.Cells[3].Value = Properties.Settings.Default.e2r0c4;

            row = dgvTgtElem.Rows[1];

            row.Cells[0].Value = Properties.Settings.Default.e2r1c1;
            row.Cells[1].Value = Properties.Settings.Default.e2r1c2;
            row.Cells[2].Value = Properties.Settings.Default.e2r1c3;
            row.Cells[3].Value = Properties.Settings.Default.e2r1c4;

            row = dgvTgtElem.Rows[2];

            row.Cells[0].Value = Properties.Settings.Default.e2r2c1;
            row.Cells[1].Value = Properties.Settings.Default.e2r2c2;
            row.Cells[2].Value = Properties.Settings.Default.e2r2c3;
            row.Cells[3].Value = Properties.Settings.Default.e2r2c4;
        }

        private void saveDgvPackage()
        {
            var row = dgvPackage.Rows[0];

            Properties.Settings.Default.pr0c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.pr0c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.pr0c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.pr0c4 = row.Cells[3].Value as string;
            Properties.Settings.Default.pr0c5 = row.Cells[4].Value as string;
            Properties.Settings.Default.pr0c6 = row.Cells[5].Value as string;

            row = dgvPackage.Rows[1];

            Properties.Settings.Default.pr1c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.pr1c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.pr1c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.pr1c4 = row.Cells[3].Value as string;
            Properties.Settings.Default.pr1c5 = row.Cells[4].Value as string;
            Properties.Settings.Default.pr1c6 = row.Cells[5].Value as string;

            row = dgvPackage.Rows[2];

            Properties.Settings.Default.pr2c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.pr2c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.pr2c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.pr2c4 = row.Cells[3].Value as string;
            Properties.Settings.Default.pr2c5 = row.Cells[4].Value as string;
            Properties.Settings.Default.pr2c6 = row.Cells[5].Value as string;

            row = dgvPackage.Rows[3];

            Properties.Settings.Default.pr3c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.pr3c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.pr3c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.pr3c4 = row.Cells[3].Value as string;
            Properties.Settings.Default.pr3c5 = row.Cells[4].Value as string;
            Properties.Settings.Default.pr3c6 = row.Cells[5].Value as string;

            row = dgvPackage.Rows[4];

            Properties.Settings.Default.pr4c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.pr4c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.pr4c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.pr4c4 = row.Cells[3].Value as string;
            Properties.Settings.Default.pr4c5 = row.Cells[4].Value as string;
            Properties.Settings.Default.pr4c6 = row.Cells[5].Value as string;

            row = dgvPackage.Rows[5];

            Properties.Settings.Default.pr5c1 = row.Cells[0].Value as string;
            Properties.Settings.Default.pr5c2 = row.Cells[1].Value as string;
            Properties.Settings.Default.pr5c3 = row.Cells[2].Value as string;
            Properties.Settings.Default.pr5c4 = row.Cells[3].Value as string;
            Properties.Settings.Default.pr5c5 = row.Cells[4].Value as string;
            Properties.Settings.Default.pr5c6 = row.Cells[5].Value as string;
        }

        private void loadDgvPackage()
        {
            var row = dgvPackage.Rows[0];

            row.Cells[0].Value = Properties.Settings.Default.pr0c1;
            row.Cells[1].Value = Properties.Settings.Default.pr0c2;
            row.Cells[2].Value = Properties.Settings.Default.pr0c3;
            row.Cells[3].Value = Properties.Settings.Default.pr0c4;
            row.Cells[4].Value = Properties.Settings.Default.pr0c5;
            row.Cells[5].Value = Properties.Settings.Default.pr0c6;

            row = dgvPackage.Rows[1];

            row.Cells[0].Value = Properties.Settings.Default.pr1c1;
            row.Cells[1].Value = Properties.Settings.Default.pr1c2;
            row.Cells[2].Value = Properties.Settings.Default.pr1c3;
            row.Cells[3].Value = Properties.Settings.Default.pr1c4;
            row.Cells[4].Value = Properties.Settings.Default.pr1c5;
            row.Cells[5].Value = Properties.Settings.Default.pr1c6;

            row = dgvPackage.Rows[2];

            row.Cells[0].Value = Properties.Settings.Default.pr2c1;
            row.Cells[1].Value = Properties.Settings.Default.pr2c2;
            row.Cells[2].Value = Properties.Settings.Default.pr2c3;
            row.Cells[3].Value = Properties.Settings.Default.pr2c4;
            row.Cells[4].Value = Properties.Settings.Default.pr2c5;
            row.Cells[5].Value = Properties.Settings.Default.pr2c6;

            row = dgvPackage.Rows[3];

            row.Cells[0].Value = Properties.Settings.Default.pr3c1;
            row.Cells[1].Value = Properties.Settings.Default.pr3c2;
            row.Cells[2].Value = Properties.Settings.Default.pr3c3;
            row.Cells[3].Value = Properties.Settings.Default.pr3c4;
            row.Cells[4].Value = Properties.Settings.Default.pr3c5;
            row.Cells[5].Value = Properties.Settings.Default.pr3c6;

            row = dgvPackage.Rows[4];

            row.Cells[0].Value = Properties.Settings.Default.pr4c1;
            row.Cells[1].Value = Properties.Settings.Default.pr4c2;
            row.Cells[2].Value = Properties.Settings.Default.pr4c3;
            row.Cells[3].Value = Properties.Settings.Default.pr4c4;
            row.Cells[4].Value = Properties.Settings.Default.pr4c5;
            row.Cells[5].Value = Properties.Settings.Default.pr4c6;

            row = dgvPackage.Rows[5];

            row.Cells[0].Value = Properties.Settings.Default.pr5c1;
            row.Cells[1].Value = Properties.Settings.Default.pr5c2;
            row.Cells[2].Value = Properties.Settings.Default.pr5c3;
            row.Cells[3].Value = Properties.Settings.Default.pr5c4;
            row.Cells[4].Value = Properties.Settings.Default.pr5c5;
            row.Cells[5].Value = Properties.Settings.Default.pr5c6;
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

            const string LAR = "Low Angle Rocket (LAR)";
            const string HAR = "High Angle Rocket (HAR)";
            const string LATR = "Low Altitude Tactical Rocket (LATR)";
            const string HATR = "High Altitude Tactical Rocket (HATR)";
            const string HARR = "High Altitude Release Rocket (HARR)";
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

        private void txtBingo_ValueChanged(object sender, EventArgs e)
        {
            int diff = Int32.Parse(txtJoker.Value.ToString()) - Int32.Parse(txtBingo.Value.ToString());
            if (diff < 500)
            {
                int add = 500 - diff;
                txtJoker.Value += add;
            }
        }

        private void txtJoker_ValueChanged(object sender, EventArgs e)
        {
            int diff = Int32.Parse(txtJoker.Value.ToString()) - Int32.Parse(txtBingo.Value.ToString());
            if (diff < 500)
            {
                int sub = 500 - diff;
                txtBingo.Value -= sub;
            }
        }
    }

    public class ATO
    {
        string nrAc;
        string typeOfAc;
        string callsignAndNr;
        string priConf;
        string secConf;
        string priFreq;
        string secFreq;
        string tasking;
        string timeFrom;
        string timeTo;
        string pos;

        public ATO(string nrAc, string typeOfAc, string callsignAndNr, string priConf, string secConf, string priFreq, string secFreq, string tasking)
        {
            this.nrAc = nrAc;
            this.typeOfAc = typeOfAc;
            this.callsignAndNr = callsignAndNr;
            this.priConf = priConf;
            this.secConf = secConf;
            this.priFreq = priFreq;
            this.secFreq = secFreq;
            this.tasking = tasking;
        }

        public void setNrAc(string nrAc)
        {
            this.nrAc = nrAc;
        }

        public string getNrAc()
        {
            return this.nrAc;
        }

        public void setTypeOfAc(string typeOfAc)
        {
            this.typeOfAc = typeOfAc;
        }

        public string getTypeOfAc()
        {
            return this.typeOfAc;
        }

        public void setCallsignAndNr(string callsignAndNr)
        {
            this.callsignAndNr = callsignAndNr;
        }

        public string getCallsignAndNr()
        {
            return this.callsignAndNr;
        }

        public void setPriConf(string priConf)
        {
            this.priConf = priConf;
        }

        public string getPriConf()
        {
            return this.priConf;
        }

        public void setSecConf(string secConf)
        {
            this.secConf = secConf;
        }

        public string getSecConf()
        {
            return this.secConf;
        }

        public void setPriFreq(string priFreq)
        {
            this.priFreq = priFreq;
        }

        public string getPriFreq()
        {
            return this.priFreq;
        }

        public void setSecFreq(string secFreq)
        {
            this.secFreq = secFreq;
        }

        public string getSecFreq()
        {
            return this.secFreq;
        }

        public void setTasking(string tasking)
        {
            this.tasking = tasking;
        }

        public string getTasking()
        {
            return this.tasking;
        }

        public void setTimeFrom(string timeFrom)
        {
            this.timeFrom = timeFrom;
        }

        public string getTimeFrom()
        {
            return this.timeFrom;
        }

        public void setTimeTo(string timeTo)
        {
            this.timeTo = timeTo;
        }

        public string getTimeTo()
        {
            return this.timeTo;
        }

        public void setPos(string pos)
        {
            this.pos = pos;
        }

        public string getPos()
        {
            return this.pos;
        }
    }
}
