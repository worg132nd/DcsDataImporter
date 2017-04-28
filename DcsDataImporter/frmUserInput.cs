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
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;

namespace DcsDataImporter
{

    public partial class frmUserInput : Form
    {
        /*
         * NATO ATO (Air Tasking Order) standard
         */

        /* VTASK */
        string VtaskTaskedUnitId;
        string VtaskMissionNumber;
        string VtaskBriefingTime;

        /* TASKUNIT */
        string TaskunitTaskedUnit;
        string TaskunitUnitLocation;

        /* AMSNDAT */
        string AmsndatMsnNumber;
        string AmsndatPrimaryMission;
        string AmsndatSecondaryMission;
        string AmsndatPackage;
        string AmsndatCommanderId;
        string AmsndatAlertState;
        string AmsndatTakeoffTime;
        string AmsndatDepartureAirfield;
        string AmsndatRecoveryAirfield;

        /* MSNACFT */
        string MsnacftNumberOfAircraftInFlight;
        string MsnacftTypeOfAircraft;
        string MsnacftCallsignAndFlightNumber;
        string MsnacftPrimaryConfig;
        string MsnacftSecondaryConfig;
        string MsnacftPrimaryFrequency;
        string MsnacftSecondaryFrequency;

        /* AMSNLOC */
        string AmsnlocTimeFrom;
        string AmsnlocTimeTo;
        string AmsnlocPosition;
        string AmsnlocAltitude;

        /* GTGLOC */
        string GtglocDesignator;
        string GtglocTimeOnTarget;
        string GtglocNotEarlierThan;
        string GtglocNotLaterThan;
        string GtglocTargetName;
        string GtglocTargetId;
        string GtglocTargetType;
        string GtglocDmpiDescription;
        string GtglocDesiredMeanPointOfImpact;
        string GtglocDmpiElevation;
        string GtglocTargetPriority;

        /* PKGCMD */
        string PkgcmdPackage;
        string PkgcmdTaskedUnit;
        string PkgcmdMsnNumber;
        string PkgcmdCallsign;

        /* ARINFO */
        string ArinfoCallsign;
        string ArinfoPosition;
        string ArinfoAltitude;
        string ArinfoFrequency;
        string ArinfoTACAN;

        /* CONTROLA */
        string ControlaType;
        string ControlaCallsign;
        string ControlaReportInPoint;
        string ControlaPrimaryFrequency;
        string ControlaSecondaryFrequency;

        /* JTAC */
        string JtacType;
        string JtacCallsign;
        string JtacPosition;
        string JtacPrimaryFrequency;
        string JtacSecondaryFrequency;

        /* AMPN */
        string AmpnAmplification;

        public frmUserInput()
        {
            this.CenterToParent();
            InitializeComponent();

            if (!Properties.Settings.Default.prevMsnNr.Equals("") && Properties.Settings.Default.prevMsnNr != null)
            {
                numMissionId.Value = Int32.Parse(Properties.Settings.Default.prevMsnNr);
            }

            if (!Properties.Settings.Default.frequencyPresetSafeFilename.Equals("") && Properties.Settings.Default.frequencyPresetSafeFilename != null)
            {
                txtFrequencyFile.Text = Properties.Settings.Default.frequencyPresetSafeFilename;
            }
        }

        /* Gets information from the ATO txt file and puts into the forms text boxes */
        private void button2_Click(object sender, EventArgs e)
        {
            if (txtFrequencyFile.Text == "")
            {
                MessageBox.Show("If you want to use the automatic filling in of frequencies, channels and presets, select a frequency file with presets like the 132nd file 132-617-A-10 Radio presets v1.0.pdf", "No frequency preset file selected");
            }

            bool foundTasking = false;
            MessageBox.Show("Browse to the ATO file which includes the air tasking orders in .txt format", "Select ATO file");

            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Filter = ".txt|*.txt";
            ofd1.Title = "Select the ATO file";
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                StreamReader file = new StreamReader(@ofd1.FileName);

                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] words = line.Split('/');

                    /* Find line that matches with tasking */

                    if (words[0] == "VTASK" && words[2] == numMissionId.Text)
                    {
                        /* After correct tasking is found */

                        foundTasking = true;

                        // Set VTASK data
                        VtaskTaskedUnitId = words[1];
                        VtaskMissionNumber = words[2];
                        Properties.Settings.Default.prevMsnNr = words[2];
                        Properties.Settings.Default.Save();
                        VtaskBriefingTime = words[3];

                        /* Read next line */
                        while ((line = file.ReadLine()) != null)
                        {
                            words = line.Split('/');

                            if (words[0] == "TASKUNIT")
                            {
                                // Set TASKUNIT data
                                TaskunitTaskedUnit = words[1];
                                TaskunitUnitLocation = words[2];
                            }
                            else if (words[0] == "AMSNDAT")
                            {
                                // Set AMSNDAT data
                                AmsndatMsnNumber = words[1];
                                AmsndatPrimaryMission = words[2];
                                AmsndatSecondaryMission = words[3];
                                AmsndatPackage = words[4];
                                AmsndatCommanderId = words[5];
                                AmsndatAlertState = words[6];
                                AmsndatTakeoffTime = words[7];
                                AmsndatDepartureAirfield = words[8];
                                AmsndatRecoveryAirfield = words[9];
                            }
                            else if (words[0] == "MSNACFT")
                            {
                                // Set MSNACFT data
                                MsnacftNumberOfAircraftInFlight = ConvertNumber(words[1]);
                                MsnacftTypeOfAircraft = words[2];
                                MsnacftCallsignAndFlightNumber = words[3];
                                MsnacftPrimaryConfig = words[4];
                                MsnacftSecondaryConfig = words[5];
                                MsnacftPrimaryFrequency = words[6];
                                MsnacftSecondaryFrequency = words[7];
                            }
                            else if (words[0] == "AMSNLOC")
                            {
                                // Set AMSNLOC data
                                AmsnlocTimeFrom = words[1];
                                AmsnlocTimeTo = words[2];
                                AmsnlocPosition = words[3];
                                AmsnlocAltitude = words[4];
                            }
                            else if (words[0] == "GTGLOC")
                            {
                                // Set GTGLOC data
                                GtglocDesignator = words[1];
                                GtglocTimeOnTarget = words[2];
                                GtglocNotEarlierThan = words[3];
                                GtglocNotLaterThan = words[4];
                                GtglocTargetName = words[5];
                                GtglocTargetId = words[6];
                                GtglocTargetType = words[7];
                                GtglocDmpiDescription = words[8];
                                GtglocDesiredMeanPointOfImpact = words[9];
                                GtglocDmpiElevation = words[10];
                                GtglocTargetPriority = words[11];
                            }
                            else if (words[0] == "PKGCMD")
                            {
                                // Set PKGCMD data
                                PkgcmdPackage = words[1];
                                PkgcmdTaskedUnit = words[2];
                                PkgcmdMsnNumber = words[3];
                                PkgcmdCallsign = words[4];
                            }
                            else if (words[0] == "ARINFO")
                            {
                                // Set ARINFO dato
                                ArinfoCallsign = words[1];
                                ArinfoPosition = words[2];
                                ArinfoAltitude = words[3];
                                ArinfoFrequency = words[4];
                                ArinfoTACAN = words[5];
                            }
                            else if (words[0] == "CONTROLA")
                            {
                                // Set CONTROLA data
                                ControlaType = words[1];
                                ControlaCallsign = words[2];
                                ControlaReportInPoint = words[3];
                                ControlaPrimaryFrequency = words[4];
                                ControlaSecondaryFrequency = words[5];
                            }
                            else if (words[0] == "TAC" || words[0] == "JTAC")
                            {
                                // Set JTAC data
                                JtacType = words[1];
                                JtacCallsign = words[2];
                                JtacPosition = words[3];
                                JtacPrimaryFrequency = words[4];
                                JtacSecondaryFrequency = words[5];
                            }
                            else if (words[0] == "AMPN")
                            {
                                // Set AMPN data
                                AmpnAmplification = words[1];
                                // read next line until //
                                while ((line = file.ReadLine()) != null)
                                {
                                    AmpnAmplification += line;
                                    // add line to AmpnAmplification
                                    if (line.EndsWith(@"//"))
                                    {

                                        AmpnAmplification = AmpnAmplification.Substring(0, AmpnAmplification.Length-2);
                                        // reached end of AMPN
                                        break;
                                    }
                                }
                            }
                            else if (words[0] == "VTASK")
                            {
                                /* Stop parsing ATO at next VTASK */
                                break;
                            }
                        }
                    }
                }
                file.Close();
            }

            if (!foundTasking)
            {
                MessageBox.Show("The mission ID is not found in the ATO. Select a new mission ID and try again.", "Invalid Mission ID");
            } else if (txtFlightPosition.Text == "")
            {
                MessageBox.Show("Select your position within the flight", "Select position");
            } else 
            {
                Hide();

                // AMSNDAT
                string MsnNr = AmsndatMsnNumber;
                string airbaseDep = AmsndatDepartureAirfield;
                string airbaseArr = AmsndatRecoveryAirfield;
                string tasking = AmsndatPrimaryMission;

                // MSNACFT
                string NrAc = MsnacftNumberOfAircraftInFlight;
                string Callsign = GetLettersOnly(MsnacftCallsignAndFlightNumber) + " " + GetFirstDigit(MsnacftCallsignAndFlightNumber) + "-" + txtFlightPosition.Text;
                string InternalChn = MsnacftPrimaryFrequency;
                string InternalBackupChn = MsnacftSecondaryFrequency;

                // CONTROLA
                string Awacs = ControlaCallsign;
                string AwacsChn = ControlaPrimaryFrequency; // TODO: Parse correct preset from the preset list pdf documents if possible
                string AwacsBackupChn = ControlaSecondaryFrequency;
                string AwacsCp = ControlaReportInPoint;

                // JTAC
                string Tacp = JtacCallsign;
                string TacpType = JtacType;
                string TacpChn = JtacPrimaryFrequency;
                string TacpBackupChn = JtacSecondaryFrequency;
                string TacpCp = JtacPosition;

                // AMSNLOC
                string location = AmsnlocPosition;

                // AMPN
                string amplification = AmpnAmplification;

                Form1 form1 = new Form1(AmsndatMsnNumber, airbaseDep, airbaseArr, NrAc, Callsign, Awacs, AwacsChn, AwacsBackupChn, AwacsCp, Tacp, TacpType, TacpChn, TacpBackupChn, TacpCp, location, tasking, InternalChn, InternalBackupChn, amplification, chkTraining.Checked, AmsndatTakeoffTime);
                form1.Show();
            }
        }

        private static string ConvertNumber(string number)
        {
            if (number == "1")
            {
                number = "one";
            }
            else if (number == "2")
            {
                number = "two";
            }
            else if (number == "3")
            {
                number = "three";
            }
            else if (number == "4")
            {
                number = "four";
            }
            return number;
        }

        private static string ConvertTma(string identifier)
        {
            string tma;
            if ((identifier == "DEPLOC:UGTB") || (identifier == "ARRLOC:UGTB"))
            {
                tma = "Tblisi";
            }
            else
            {
                tma = "N/A";
            }
            return tma;
        }

        private static string ConvertAirfield(string identifier)
        {
            // Default return value
            string airfield = "N/A";

            if ((identifier == "DEPLOC:UGTB") || (identifier == "ARRLOC:UGTB"))
            {
                airfield = "Lochini";
            }
            else if ((identifier == "DEPLOC:UGKS") || (identifier == "ARRLOC:UGKS"))
            {
                airfield = "Senaki"; // Consider Senaki - Kolkhi as stated in the Aerodome charts
            }
            return airfield;
        }

        private static string GetLettersOnly(string word)
        {
            return new String(word.Where(Char.IsLetter).ToArray()).ToString();
        }

        public static string GetFirstDigit(string word)
        {
            return new String(word.Where(Char.IsDigit).ToArray())[0].ToString();
        }

        private void btnFrequencyFileBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ".pdf|*.pdf";
            ofd.Title = "Select the frequency preset file";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtFrequencyFile.Text = ofd.SafeFileName; // only file name
                Properties.Settings.Default.frequencyPresetSafeFilename = ofd.SafeFileName; // only file name
                Properties.Settings.Default.frequencyPresetFilename = ofd.FileName; // path
            }
        }

        private void btnLoadPrev_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(chkTraining.Checked);
            Hide(); // Hide initial loading form
            form1.Show(); // Show second flight form
        }
    }

}
