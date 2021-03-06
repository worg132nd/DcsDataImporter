﻿using System;
using System.IO.Compression;
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
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
//using Microsoft.Office.Interop.Word;

/* storeAsPdf-method contains code that is commented out temporarily */

/* References
 * 
 * The following references must be added for this project to work:
 * 
 * Add reference to C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IO.Compression.FileSystem.dll
 * and C:\Users\<username>\Downloads\itextsharp-all-5.5.9\itextsharp-dll-core\itextsharp.dll
 * 
 */

/* TODO
 * 
 * Bug: Remove focus from txtTacpCallsign and other Tacp and Awacs textboxes when unchecking TACP or AWACS (disableAwacs/disableTacp)
 * 
 * Add tanker ATO import functionality (ARINFO)
 * 
 * --------------------------------------------------------------------------------------------------------------------------------------
 * Must build up a group of images for standard training and another for mission (don't need multiple groups in the kneeboard builder!)
 * This will make integration with the kneeboard builder much better.
 * Need save the images to the correct group (either standard training or mission)
 * Mission can possibly have multiple versions: noTma, noAwacs etc.
 * MDC-000, MDC-001, MDC-002, MDC-003 etc.
 * Fordelen med å gjøre det på denne måten er at jeg ikke får blanke ark i kneeboard, eller trenger ikke modifisere ting i kneeboard før
 * en flight. Ulmepa er at det muuuligens blir vanskeligere å organisere ting i riktig rekkefølge.
 * --------------------------------------------------------------------------------------------------------------------------------------
 * 
 * Legg til hva slags fly man flyr på det første input formet
 * 
 * Test programmet på en annen maskin for å se hva som feiler. Bare kopier exe fila og se hvordan det går.
 * 
 * Add a line with TMA/airport: <TMA1>, <TMA2>, <TMA3>, <TMA4>, <TMA5>, <TMA1>, <ENTRY/EXIT1>, <ENTRY/EXIT2>, <ENTRY/EXIT3>, <NAV1>, <NAV2>, <NAV3> etc.
 * 
 * Lag import funksjonalitet for å laste ned ATO fra websiden vår
 * 
 * Lag installer for programmet (medium prioritet)
 * 
 * If a line breaks before //, need to set lines together
 * 
 * If singleship, replace "one by ALPHA 10 CHARLIEs" with "singleship ALPHA 10 CHARLIE"
 * 
 * Lag også en måte å parse metar på og anbefal loadout basert på været, velg aktiv rullebane, om lead skal ta right eller left på runway etc.) Tenk igjennom og gå igjennom gamle notater hva jeg pleier å skrive ned av items. Interne frekvenser, backup frekvenser etc.
 * 
 * Velg plassering av pdf også, og husk fra gang til gang, akkurat som med kneeboard
 * 
 * Kan ha checkbox bak runways som krysses av hvis aktiv runway. Så dersom den krysses av så velges automatisk riktig CP eller flightplan basert på retning.
 * 
 * Mulighet til å resette paths for både ato og for preset-pdf (hvis man f.eks. bytter installdirectory på kneeboardbuilder)
 * 
 * Ha en mulighet til å IKKE automatisk fylle ut, eller overskrive det som har blitt fylt ut
 * 
 * Hvis directory med COM1 STA etc. ikke finns, opprett mapper
 * 
 * Use System.Net and System.Text.RegularExpressions to use WebClient web = new WebClient() and String html = web.DownloadString("<URL>") to get website directly here
 * 
 * Legg til en file open dialog når man klikker submit for å velge hvor pdf-filene skal plasseres
 * 
 * Navn på PNG er nå COM2 TO-000, men hva om man har flere PNG i samme mappe? Lagre som 001, 002 etc? (lav prioritet)
 * 
 * * Velger automatisk autentisering for hver flight basert på pdf (lavt prioritert)
 * 
 * Parse også operative frekvenser pdf (men dette er lav prioritet)
 * 
 * Check if it's called Tbilisi or Tblisi (and fix so that either works with my program)
 * If unable to find frequency in the preset-pdf, fill inn automatically from the "operational frequency chart"-pdf
 * Label labels and change them to a red asterics (*) to mark that it has been filled out with a default value
 * Velg mellom IFR og VFR departure
 * Erstatt alle makroer og alle script slik at alt kan kjøres fra dette programmet (programmet gjør alt)
 * 
 * Readme.txt
 * - When using Word 2010 on my computer I need to replace all /TAGS/ with itself by running a makro.
 *   This should solve any problems with tags not being replaced. Could also help using a compatibility
 *   version of the word-document.
 * 
 */

namespace DcsDataImporter
{
    public partial class Form1 : Form
    {

        /* Fonetic alphabet */
        public const string A = "ALPHA";
        public const string B = "BRAVO";
        public const string C = "CHARLIE";
        public const string D = "DELTA";
        public const string E = "ECHO";
        public const string F = "FOXTROT";
        public const string G = "GOLF";
        public const string H = "HOTEL";
        public const string I = "INDIA";
        public const string J = "JULIETT";
        public const string K = "KILO";
        public const string L = "LIMA";
        public const string M = "MIKE";
        public const string N = "NOVEMBER";
        public const string O = "OSCAR";
        public const string P = "PAPA";
        public const string Q = "QUEBEC";
        public const string R = "ROMEO";
        public const string S = "SIERRA";
        public const string T = "TANGO";
        public const string U = "UNIFORM";
        public const string V = "VICTOR";
        public const string X = "XRAY";
        public const string Y = "YANKEE";
        public const string Z = "ZULU";

        public List<Tuple> list;
        public List<Airbase> airbases;
        public List<Range> ranges;
        public bool standardTrainingSet = false;

        private bool hasTma;
        private bool loadPrev = false;
        private bool fromBack = false;
        private string oldCallsign;

        private int lblJtacPosLeft = 0;
        private int lblJtacPosTop = 0;

        private int chkTacpPosLeft = 0;
        private int chkTacpPosTop = 0;

        /* Constructor for empty form
         * used when pressing Back on next form
         */
        public Form1()
        {
            init();
            loadPrevMission();
            loadPrev = true;
            // store old callsign to make sure that when the user goes back, the txtCallsign_leave event is triggered and updateFlight activated (used to be a bug)
            oldCallsign = txtCallsign.Text;
            fromBack = true;
        }

        /* Constructor with no ATO: Used when pressing the <Load prev.> button */
        public Form1(bool standardTraining)
        {
            if (standardTraining)
            {
                standardTrainingSet = true;
                MyGlobals.global_training = true;
            }
            init();
            loadPrevMission();
            loadPrev = true;
        }    

        /* Constructor when ATO is filled out */
        public Form1(string AmsndatMsnNumber, string airbaseDep, string airbaseArr, string NrAc, string Callsign, string Awacs, string AwacsChn, string AwacsBackupChn, string AwacsCp, string Tacp, string TacpType, string TacpChn, string TacpBackupChn, string TacpCp, string location, string tasking, string internalFreq, string internalBackupFreq, string amplification, bool standardTraining, string takeoffTime, bool tma, bool chkAwacsAG, bool chkAwacsAA, bool chkExtraAwacsAG, bool chkExtraAwacsAA, bool chkFaca, bool chkCsar, bool chkJstar, bool chkScramble, bool chkExtraJtac, bool chkExtraPackage, string numTankers, Form selectSupport, string timeFrom, string timeTo)
        {
            init();

            // Clear out default zeroes
            txtAwacsPreset.Text = txtTacpPreset.Text = txtInternalPreset.Text = txtInternalBackupPreset.Text = txtAwacsBackupPreset.Text = txtTacpBackupPreset.Text = "";

            if (Tacp == null) disableTacp();
            if (Awacs == null) disableAwacs();

            genAirbaseObj();

            /* Initialize form based on ATO */
            txtMsnNr.Text = AmsndatMsnNumber;
            setAirport(airbaseDep);
            setAirport(airbaseArr);
            txtCallsign.Text = Callsign;
            cmbNrOfAc.Text = convertNumberToDigit(NrAc);
            txtAwacsCallsign.Text = Awacs;
            txtAwacsCp.Text = AwacsCp;
            txtTacpCallsign.Text = Tacp;
            setJtacLbl(TacpType);
            initSupport(chkAwacsAG, chkAwacsAA, chkExtraAwacsAG, chkExtraAwacsAA, chkFaca, chkCsar, chkJstar, chkScramble, chkExtraJtac, chkExtraPackage, numTankers, selectSupport);
            setSupportCell("JTAC", "callsign", Tacp);
            txtTacpCp.Text = TacpCp;
            setSupportCell("JTAC", "notes", "Contact point " + TacpCp);
            txtLocation.Text = location;
            Properties.Settings.Default.prevAmpn = amplification;
            initFuel();
            hasTma = tma;
            txtVulEnd.Text = timeTo;

            // Set correct tasking
            if (tasking.Equals("TR"))
            {
                txtTasking.Text = "Training";
            }
            else
            {
                txtTasking.Text = tasking;
            }

            // Set times (takeoff time, vul start, vul end)
            formatAndSetTime(takeoffTime, txtTakeoffTime);
            formatAndSetTime(timeFrom, txtVulStart);
            formatAndSetTime(timeTo, txtVulEnd);

            buildList();

            string airportIdentifier = stripArrlocAndDeplocFromAirportIdentifier(airbaseDep);
            string airportName = ConvertAirfield(airportIdentifier);
            string airportTma = getAirportTma(airportIdentifier);

            string searchTerm = (airportName + " GND");

            var row = dgvAirbase.Rows[0];
            string groundFreq = row.Cells["colGnd"].Value as string;
            string towerFreq = row.Cells["colTwr"].Value as string;
            string tmaFreq = row.Cells["colTma"].Value as string;

            Tuple tuple = null;

            /* Set values in form based on airport (can be done initially and afterwards if Text is changed */
            tuple = list.Find(x => x.getName().ToLower().Contains(searchTerm.ToLower()));
            if (tuple != null) groundFreq = tuple.getFreq();

            searchTerm = (airportName + " TWR");
            tuple = list.Find(x => x.getName().ToLower().Contains(searchTerm.ToLower()));
            if (tuple != null) towerFreq = tuple.getFreq();

            searchTerm = (airportTma + " TMA");
            tuple = list.Find(x => x.getName().ToLower().Contains(searchTerm.ToLower()));
            if (tuple != null) tmaFreq = tuple.getFreq();

            tuple = list.Find(x => x.getName().ToLower().Contains("flight report".ToLower()));
            if (tuple != null)
            {
                setSupportCell("in-flight report", "preset", tuple.getPreset());
                setSupportCell("in-flight report", "freq", tuple.getFreq());
                setSupportCell("in-flight report", "channel", tuple.getChannel());
            }

            if (standardTraining)
            {
                standardTrainingSet = true;
                MyGlobals.global_training = true;
                initStandardTraining();
            }

            setRadio("AWACS", formatChannel(AwacsChn), formatChannel(AwacsBackupChn));
            setRadio("TACP", formatChannel(TacpChn), formatChannel(TacpBackupChn));
            setRadio("internal", formatChannel(internalFreq), formatChannel(internalBackupFreq));
        }

        // Convert from format 0730 to 07:30 and set the correct text box
        private void formatAndSetTime(string time, Control textBox) 
        {
            if (time != null && !time.Equals("-") && !time.Equals(""))
            {
                textBox.Text = time[0].ToString() + time[1].ToString() + ":" + time[2].ToString() + time[3].ToString();
            }
        }

        public static class MyGlobals
        {
            public static bool global_training = false;
        }

        private void initStandardTraining()
        {
            setSupportCell("AWACS A-A #1", "channel", "ORANGE 10");
            setSupportCell("AWACS A-A #1", "freq", "228.000");
            setSupportCell("AWACS A-A #1", "preset", "3");

            setSupportCell("AWACS A-A #2", "channel", "YELLOW 1");
            setSupportCell("AWACS A-A #2", "freq", "227.500");
            setSupportCell("AWACS A-A #2", "preset", "19");

            setSupportCell("AWACS A-G #1", "channel", "GREEN 4");
            setSupportCell("AWACS A-G #1", "freq", "229.000");
            setSupportCell("AWACS A-G #1", "preset", "2");

            setSupportCell("AWACS A-G #2", "channel", "YELLOW 1");
            setSupportCell("AWACS A-G #2", "freq", "227.500");
            setSupportCell("AWACS A-G #2", "preset", "19");

            setSupportCell("AWACS A-A #1", "backup", "136.250");
            setSupportCell("AWACS A-A #2", "backup", "136.250");
            setSupportCell("AWACS A-G #1", "backup", "136.250");
            setSupportCell("AWACS A-G #2", "backup", "136.250");

            setSupportCell("SCRAMBLE", "channel", "VIOLET 10");
            setSupportCell("SCRAMBLE", "freq", "228.250");
            setSupportCell("SCRAMBLE", "preset", "18");

            setSupportCell("CSAR", "channel", "BROWN 10");
            setSupportCell("CSAR", "freq", "228.500");
            setSupportCell("CSAR", "preset", "5");

            setSupportCell("In-flight Report", "channel", "OCHRE 9");
            setSupportCell("In-flight Report", "freq", "234.000");
            setSupportCell("In-flight Report", "preset", "4");

            txtAwacsBackupChannel.Text = "INDIGO 6";
            txtAwacsBackupFreq.Text = "136.250";

            setSupportCell("Tanker 1", "callsign", "TEXACO");
            setSupportCell("Tanker 1", "freq", "151.000");
            setSupportCell("Tanker 1", "channel", "OLIVE 10");
            setSupportCell("Tanker 1", "preset", "5");

            setSupportCell("Tanker 2", "callsign", "SHELL");
            setSupportCell("Tanker 2", "freq", "149.000");
            setSupportCell("Tanker 2", "channel", "OLIVE 8");
            setSupportCell("Tanker 2", "preset", "6");

            setSupportCell("Tanker 1", "notes", "TCN5Y FL115 270KTS");
            setSupportCell("Tanker 2", "notes", "TCN6Y FL12 270KTS");

            //txtTacpCp.Text = "MUKHRANI"; //default
            //setSupportCell("JTAC", "notes", "MUKHRANI");
            //txtParking.Text = "Apron 1, parking space 13";

            //cmbAirbaseAlt.Text = "UGKO";

            enableAwacs();
            txtAwacsFreq.Text = "237.000";
            txtAwacsChannel.Text = "BLUE 3";
            txtAwacsPreset.Text = "1";

        }

        private void init()
        {
            InitializeComponent();
            initDataGridViews();
            genRangeObj();
        }

        private void initDataGridViews()
        {
            initDataGridView(dgvAirbase, 4);
            initDataGridView(dgvFlight, 4);
            initDataGridView(dgvSupport, 10);
        }

        public void initDataGridView(DataGridView dgv, int rowCount)
        {
            dgv.RowCount = rowCount;
            dgv.DefaultCellStyle.SelectionBackColor = dgv.DefaultCellStyle.BackColor;
            dgv.DefaultCellStyle.SelectionForeColor = dgv.DefaultCellStyle.ForeColor;
        }

        /* Overloaded method with checkbox as arguments */
        private void initSupport(bool chkAwacsAG, bool chkAwacsAA, bool chkExtraAwacsAG, bool chkExtraAwacsAA, bool chkFaca, bool chkCsar, bool chkJstar, bool chkScramble, bool chkExtraJtac, bool chkExtraPackage, string numTankers, Form selectSupport)
        {
            fillSupportWithDash();
            int i = 0;

            var row = dgvSupport.Rows[0];
            if (chkAwacsAG && i < dgvSupport.Rows.Count)
            {
                if (chkExtraAwacsAG)
                {
                    row.Cells["colTypeSupport"].Value = "AWACS A-G #1";
                }
                else
                {
                    row.Cells["colTypeSupport"].Value = "AWACS A-G";
                }
                row.Cells["colNotesSupport"].Value = "WD";
                i++;
            }

            row = dgvSupport.Rows[i];
            if (chkExtraAwacsAG)
            {
                row.Cells["colTypeSupport"].Value = "AWACS A-G #2";
                row.Cells["colNotesSupport"].Value = "WD";
                i++;
            }

            row = dgvSupport.Rows[i];
            if (chkAwacsAA)
            {
                if (chkExtraAwacsAA)
                {
                    row.Cells["colTypeSupport"].Value = "AWACS A-A #1";
                }
                else
                {
                    row.Cells["colTypeSupport"].Value = "AWACS A-A";
                }

                row.Cells["colNotesSupport"].Value = "WD";
                i++;
            }

            row = dgvSupport.Rows[i];
            if (chkExtraAwacsAA)
            {
                row.Cells["colTypeSupport"].Value = "AWACS A-A #2";
                row.Cells["colNotesSupport"].Value = "WD";
                i++;
            }

            row = dgvSupport.Rows[i];
            if (chkFaca)
            {
                row.Cells["colTypeSupport"].Value = "FAC(A)";
                i++;
            }

            row = dgvSupport.Rows[i];
            if (chkCsar)
            {
                row.Cells["colTypeSupport"].Value = "CSAR";
                i++;
            }

            row = dgvSupport.Rows[i];
            if (chkJstar)
            {
                row.Cells["colTypeSupport"].Value = "JSTAR";
                i++;
            }

            row = dgvSupport.Rows[i];
            if (chkScramble)
            {
                row.Cells["colTypeSupport"].Value = "Scramble";
                i++;
            }

            row = dgvSupport.Rows[i];
            if (chkExtraJtac)
            {
                string value = "JTAC";
                if (lblJTAC.Text == "JTAC")
                {
                    value += " #2";
                }

                row.Cells["colTypeSupport"].Value = value;
                i++;
            }

            row = dgvSupport.Rows[i];
            if (chkExtraPackage)
            {
                row.Cells["colTypeSupport"].Value = "Package";
                i++;
            }
            for (int x = 0; x < Int32.Parse(numTankers); x++)
            {
                row = dgvSupport.Rows[i];
                if (numTankers.Equals("1"))
                {
                    row.Cells["colTypeSupport"].Value = "Tanker";
                } else
                {
                    row.Cells["colTypeSupport"].Value = "Tanker " + (x + 1).ToString();
                }
                i++;
            }

            row = dgvSupport.Rows[i];
            row.Cells["colTypeSupport"].Value = "In-Flight Report";
        }

        private void fillSupportWithDash()
        {
            int i = 0;
            while (i < dgvSupport.Rows.Count)
            {
                initSupportRow(i);
                i++;
            }
        }

        private void loadPrevMission()
        {
            buildList();
            genAirbaseObj();

            loadFirstLineOfMDC();
            loadDGVFlight();
            loadSelectedAirbases();
            loadTimes();
            loadAltAlerts();
            loadTaxiAndRejoin();
            loadDGVAirbases();
            loadAwacs();
            loadTacp();
            loadTextBoxes();
            loadInternalFrequencies();
            loadDGVSupport();

            if (Properties.Settings.Default.prevChkTma == "true") hasTma = true;
            else hasTma = false;
        }

        private void loadTaxiAndRejoin()
        {
            txtParking.Text = Properties.Settings.Default.prevTxtParking;
            cbRejoin.Text = Properties.Settings.Default.prevCbRejoin;
            cbTaxi.Text = Properties.Settings.Default.prevCbTaxi;
        }

        /* Loads textbox data like Metar and Amplification from last form */
        private void loadTextBoxes()
        {
            txtMetar.Text = Properties.Settings.Default.prevTxtMetar;
        }

        /* Loads takeoff and landing time data from last form */
        private void loadTimes()
        {
            txtStepTime.Text = Properties.Settings.Default.prevTxtStepTime;
            txtTaxiTime.Text = Properties.Settings.Default.prevTxtTaxiTime;
            txtTakeoffTime.Text = Properties.Settings.Default.prevTxtTakeoffTime;
            txtVulStart.Text = Properties.Settings.Default.prevTxtVulStart;
            txtVulEnd.Text = Properties.Settings.Default.prevTxtVulEnd;
            txtLandingTime.Text = Properties.Settings.Default.prevTxtLandingTime;
        }

        /* Loads selected airbases from last form */
        private void loadSelectedAirbases()
        {
            /* AIRPORT */
            cmbAirbaseDep.Text = Properties.Settings.Default.prevCmbAirbaseDep;
            cmbAirbaseArr.Text = Properties.Settings.Default.prevCmbAirbaseArr;
            cmbAirbaseAlt.Text = Properties.Settings.Default.prevCmbAirbaseAlt;
            cmbAirbaseBck.Text = Properties.Settings.Default.prevCmbAirbaseBck;
        }

        /* Loads first line of the MDC data from last form */
        private void loadFirstLineOfMDC()
        {
            txtCallsign.Text = Properties.Settings.Default.prevTxtCallsign;
            oldCallsign = txtCallsign.Text;
            cmbNrOfAc.Text = Properties.Settings.Default.prevTxtNrOfAc;
            txtMsnNr.Text = Properties.Settings.Default.prevTxtMsnNr;
            txtTasking.Text = Properties.Settings.Default.prevTxtTasking;
            txtLocation.Text = Properties.Settings.Default.prevTxtLocation;
        }

        /* Loads TACP (meaning JTAC and FAC(A)) data from last form */
        private void loadTacp()
        {
            if (Properties.Settings.Default.prevChkTacp == "true") chkTacp.Checked = true;
            else chkTacp.Checked = false;

            txtTacpCallsign.Text = Properties.Settings.Default.prevTxtTacpCallsign;
            txtTacpFreq.Text = Properties.Settings.Default.prevTxtTacpFreq;
            txtTacpChannel.Text = Properties.Settings.Default.prevTxtTacpChannel;
            txtTacpPreset.Text = Properties.Settings.Default.prevTxtTacpPreset;
            txtTacpCp.Text = Properties.Settings.Default.prevTxtTacpCp;

            txtTacpBackupFreq.Text = Properties.Settings.Default.prevTxtTacpBackupFreq;
            txtTacpBackupChannel.Text = Properties.Settings.Default.prevTxtTacpBackupChannel;
            txtTacpBackupPreset.Text = Properties.Settings.Default.prevTxtTacpBackupPreset;
        }

        private void setJtacLbl(string type)
        {
            if (type != null && !type.Equals(""))
            {
                lblJTAC.Text = type;
            }
        }

        /* Loads AWACS data from last form */
        private void loadAwacs()
        {
            if (Properties.Settings.Default.prevChkAwacs == "true") chkAwacs.Checked = true;
            else chkAwacs.Checked = false;

            txtAwacsCallsign.Text = Properties.Settings.Default.prevTxtAwacsCallsign;
            txtAwacsFreq.Text = Properties.Settings.Default.prevTxtAwacsFreq;
            txtAwacsChannel.Text = Properties.Settings.Default.prevTxtAwacsChannel;
            txtAwacsPreset.Text = Properties.Settings.Default.prevTxtAwacsPreset;
            txtAwacsCp.Text = Properties.Settings.Default.prevTxtAwacsCp;

            txtAwacsBackupFreq.Text = Properties.Settings.Default.prevTxtAwacsBackupFreq;
            txtAwacsBackupChannel.Text = Properties.Settings.Default.prevTxtAwacsBackupChannel;
            txtAwacsBackupPreset.Text = Properties.Settings.Default.prevTxtAwacsBackupPreset;
        }

        /* Loads internal frequency data from last form */
        private void loadInternalFrequencies()
        {
            txtInternalFreq.Text = Properties.Settings.Default.prevTxtInternalFreq;
            txtInternalChannel.Text = Properties.Settings.Default.prevTxtInternalChannel;
            txtInternalPreset.Text = Properties.Settings.Default.prevTxtInternalPreset;
            txtInternalBackupFreq.Text = Properties.Settings.Default.prevTxtInternalBackupFreq;
            txtInternalBackupChannel.Text = Properties.Settings.Default.prevTxtInternalBackupChannel;
            txtInternalBackupPreset.Text = Properties.Settings.Default.prevTxtInternalBackupPreset;
        }

        private void buildList()
        {
            string presetsTextCleaned = CleanPresets(ExtractTextFromPdf(Properties.Settings.Default.frequencyPresetFilename));

            bool lastWordWasChannel = false;
            Tuple tuple = null;

            list = new List<Tuple>();

            foreach (string word in presetsTextCleaned.Split(' '))
            {
                //MessageBox.Show(word);

                if (lastWordWasChannel && isNumber(word))
                {
                    // Number following a channel
                    lastWordWasChannel = false;

                    // Add color to item already in list
                    list.Find(x => x.getFreq().Equals(tuple.getFreq())).setChannel(word);
                }
                else
                {

                    if (isNumber(word))
                    {
                        // Preset
                        tuple = new Tuple(); // Create new item when encountering new preset
                        tuple.setPreset(word);
                    }

                    else if (isFreq(word))
                    {
                        // Freq
                        tuple.setFreq(word);
                        list.Add(tuple); // Add item to list here because all items has a frequency
                    }

                    else if (isColor(word))
                    {
                        // new: If a word is like YELLOW9, without space between color and digit
                        if (word.Any(char.IsDigit))
                        {
                            string color = Regex.Replace(word, @"[\d-]", string.Empty);
                            string nr = Int32.Parse(Regex.Match(word, @"\d").Value).ToString();
                            tuple.setChannel(color);
                            list.Find(x => x.getFreq().Equals(tuple.getFreq())).setChannel(nr);
                        } else
                        {
                            // Channel color
                            lastWordWasChannel = true;
                            tuple.setChannel(word);

                            // Name
                        }
                    }
                    else
                    {
                        if (tuple != null)
                        {
                            // Everything not identified is the name
                            tuple.setName(word);
                        }
                    }
                }
            }
            parseOperationalFreq();
        }

        private string convertNumberToDigit(string number)
        {
            string digit = "";
            if (number.Equals("one", StringComparison.InvariantCultureIgnoreCase))
            {
                digit = "1";
            }
            if (number.Equals("two", StringComparison.InvariantCultureIgnoreCase))
            {
                digit = "2";
            }
            if (number.Equals("three", StringComparison.InvariantCultureIgnoreCase))
            {
                digit = "3";
            }
            if (number.Equals("four", StringComparison.InvariantCultureIgnoreCase))
            {
                digit = "4";
            }
            return digit;
        }

        /* col is a string containing either "callsign", "freq", "channel", "preset", "backup" or "notes" */
        private void setSupportCell(string searchField, string col, string value)
        {
            if (value != null)
            {
                if (findRow(searchField) != -1)
                {
                    var row = dgvSupport.Rows[findRow(searchField)];
                    string colName = "col" + col.First().ToString().ToUpper() + col.Substring(1).ToLower() + "Support";
                    row.Cells[colName].Value = value;
                }
            }
        }

        /* MAKE A GENERIC METHOD FOR SETTING ANY colTypeSupport types, not just IFRN. Should be easy using findRow */

        private int findRow(string searchTerm)
        {
            int i = 0;
            while (i < dgvSupport.Rows.Count)
            {
                var row = dgvSupport.Rows[i];
                string colTypeSupport = row.Cells["colTypeSupport"].Value as string;
                if (colTypeSupport != null && colTypeSupport.ToLower().Contains(searchTerm.ToLower()))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        private string convertDigitToNumber(string digit)
        {
            string nr = "";
            if (digit.Equals("1", StringComparison.InvariantCultureIgnoreCase))
            {
                nr = "one";
            }
            if (digit.Equals("2", StringComparison.InvariantCultureIgnoreCase))
            {
                nr = "two";
            }
            if (digit.Equals("3", StringComparison.InvariantCultureIgnoreCase))
            {
                nr = "three";
            }
            if (digit.Equals("4", StringComparison.InvariantCultureIgnoreCase))
            {
                nr = "four";
            }
            return nr;
        }

        private string getAirportTma(string x)
        {
            /* Georgian airports */
            if (x == "UG5X") {
                x = "Kobuleti";
            } else if (x == "UG23") {
                x = "Gudauta";
            } else if (x == "UG24") {
                x = "Tbilisi";
            } else if (x == "UG27") {
                x = "Vaziani";
            } else if (x == "UGKO") {
                x = "Kutaisi";
            } else if (x == "UGKS") {
                x = "Senaki";
            } else if (x == "UGSB") {
                x = "Batumi";
            } else if (x == "UGSS") {
                x = "Sukhumi";
            } else if (x == "UGTB") {
                x = "Tbilisi";
            }
            return x;

            /* Russian airports */
            // TBD
        }

        /* TODO: Implement. Simplification of the 3 methods.
         * This method is not actually used YET
         */
        private void setRadio(string type, string mainCh, string backupCh)
        {
            const bool BACKUP = true;
            const bool MAIN = false;

            if (mainCh != null && mainCh != "")
            {
                Tuple tuple = getTuple(mainCh);
                if (tuple != null)
                {
                    setRadioAccordingToType(type, tuple, MAIN);
                }
                else
                {
                    setOnlyChannel(type, mainCh, MAIN);
                }
            }

            if (backupCh != null && backupCh != "")
            {
                Tuple tuple = getTuple(backupCh);
                if (tuple != null)
                {
                    setRadioAccordingToType(type, tuple, BACKUP);
                }
                else
                {
                    // set radio channel if no hit in list
                    setOnlyChannel(type, backupCh, BACKUP); // e.g. GREEN 1
                }
            }
        }

        private void setOnlyChannel(string type, string channel, bool backup)
        {
            if (type.ToUpper().Equals("AWACS"))
            {
                if (backup)
                {
                    txtAwacsBackupChannel.Text = channel;
                } else
                {
                    txtAwacsChannel.Text = channel;
                }
            }

            if (type.ToUpper().Equals("TACP"))
            {
                if (backup)
                {
                    txtTacpBackupChannel.Text = channel;
                    setSupportCell("JTAC", "backup", channel);
                }
                else
                {
                    txtTacpChannel.Text = channel;
                    setSupportCell("JTAC", "channel", channel);
                }
            }

            if (type.ToLower().Equals("internal"))
            {
                if (backup)
                {
                    txtInternalBackupChannel.Text = channel;
                } else
                {
                    txtInternalChannel.Text = channel;
                }
                
            }
        }

        /* Set AWACS main radio frequency values in the form */
        private void setAwacsMain(Tuple tuple)
        {
            txtAwacsChannel.Text = tuple.getChannel();
            txtAwacsFreq.Text = tuple.getFreq();
            txtAwacsPreset.Text = tuple.getPreset();
        }

        /* Set AWACS backup radio frequency values in the form */
        private void setAwacsBackup(Tuple tuple)
        {
            txtAwacsBackupChannel.Text = tuple.getChannel();
            txtAwacsBackupFreq.Text = tuple.getFreq();
            txtAwacsBackupPreset.Text = tuple.getPreset();
        }

        /* Set TACP main radio frequency values in the form, both textboxes and data grid views */
        private void setTacpMain(Tuple tuple)
        {
            txtTacpChannel.Text = tuple.getChannel();
            txtTacpFreq.Text = tuple.getFreq();
            txtTacpPreset.Text = tuple.getPreset();
        }

        /* Set TACP backup radio frequency values in the form */
        private void setTacpBackup(Tuple tuple)
        {
            txtTacpBackupChannel.Text = tuple.getChannel();
            txtTacpBackupFreq.Text = tuple.getFreq();
            txtTacpBackupPreset.Text = tuple.getPreset();

            setSupportCell("JTAC", "backup", tuple.getFreq()); //TODO: Consider removing (can be removed if you don't want TAC-data in dgvSupport)
        }

        /* Set internal main radio frequency values in the form */
        private void setInternalMain(Tuple tuple)
        {
            txtInternalChannel.Text = tuple.getChannel();
            txtInternalFreq.Text = tuple.getFreq();
            txtInternalPreset.Text = tuple.getPreset();
        }

        /* Set internal backup radio frequency values in the form */
        private void setInternalBackup(Tuple tuple)
        {
            txtInternalBackupChannel.Text = tuple.getChannel();
            txtInternalBackupFreq.Text = tuple.getFreq();
            txtInternalBackupPreset.Text = tuple.getPreset();
        }

        /* Sets radio according to specified type for AWACS, TACP and internal radio frequency values.
         * Sets either main or backup radio values based on parameter backup
         */
        private void setRadioAccordingToType(string type, Tuple tuple, bool backup)
        {
            /* AWACS frequencies */
            if (type.ToUpper().Equals("AWACS"))
            {
                if (backup) // Set backup frequencies
                {
                    setAwacsBackup(tuple);
                } else // Set main frequencies
                {
                    setAwacsMain(tuple);
                }
            }

            /* TAC frequencies */
            if (type.ToUpper().Equals("TACP"))
            {

                if (backup) // Setting backup frequencies
                {
                    setTacpBackup(tuple);
                } else // Setting main frequencies
                {
                    setTacpMain(tuple);
                }
            }

            /* INTERNAL FREQUENCIES */
            if (type.ToLower().Equals("internal"))
            {
                if (backup) // Setting backup frequencies
                {
                    setInternalBackup(tuple);
                } else // Setting main frequencies
                {
                    setInternalMain(tuple);
                }
            }
        }

        private string formatChannel(string channel)
        {
            if (channel == null || channel.Length == 0 || channel.Equals("-"))
            {
                return "";
            }
            if (!channel.Contains(" ") && !channel.Equals("-") && channel != null)
            {
                if (!channel.Any(char.IsDigit))
                {
                    return null;
                }
                /*try
                {*/
                channel = channel.Insert(channel.IndexOfAny("0123456789".ToCharArray()), " ");
                    //channel = channel.ToUpper();
                /*}*/

                /*catch (ArgumentOutOfRangeException ex) {
                    //throw ex;
                }*/

                
            }
            return channel;
        }

        private Tuple getTuple(string channel)
        {
            channel = formatChannel(channel);
            return list.Find(x => x.getChannel().ToLower().Equals(channel.ToLower()));
        }

        private static void SearchAndReplace(string search, string replacement)
        {
            
            if (replacement != null && replacement != "")
            {
                string path = Environment.CurrentDirectory + @"\" + "temp.docm"; // TODO: use currently active directory or select using a browse function instead. Save this in a document so it remembers it for each run of the application, so you don't have to do it more than once
                try
                {
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(path, true))
                    {
                        string docText = null;
                        using (StreamReader stream = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                        {
                            docText = stream.ReadToEnd();
                        }

                        Regex regexText = new Regex(search); // &lt og &gt escaper henholdsvis < og >
                        docText = regexText.Replace(docText, replacement);

                        using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                        {
                            sw.Write(docText);
                        }
                    }
                }
                catch (FileFormatException ffe)
                {
                    MessageBox.Show("The file is corrupt or not in the expected format. Closing application.");
                    Environment.Exit(1); // ToDo: Best practice to use Application.Exit() when using a Windows form
                }
                catch (System.IO.IOException)
                {
                    System.Threading.Thread.Sleep(50); // sleep to make sure focus is moved
                    SearchAndReplace(search, replacement);
                    return;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error");
                    Environment.Exit(1); // ToDo: Best practice to use Application.Exit() when using a Windows form
                }
            }
        }

        private void textboxLeaveEventHandler(object sender, EventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            string s = textbox.Text;

            if (textbox.Text != "")
            {

                Tuple tuple = null;

                if (textbox.Name.Contains("Channel"))
                {
                    //textbox.Text = formatChannel(textbox.Text);
                    tuple = (Tuple)list.Find(x => x.getChannel().ToLower().Equals(formatChannel(s.ToLower())));
                    if (tuple == null && isColor(textbox.Text))
                    {
                        if (formatChannel(textbox.Text) == null)
                        {
                            // valid color without preset but lacks number, e.g. brown
                            errorInvalidFormat();
                        }

                        // valid color with valid number, e.g. brown 1 (but nr not found in preset list)

                        textbox.Text = formatChannel(textbox.Text);
                        textbox.Text = textbox.Text.ToUpper();
                    } else if (!isColor(textbox.Text))
                    {
                        // completely invalid format like asdfasdf
                        errorInvalidFormat();
                        textbox.Text = "";
                    } else if (isColor(textbox.Text))
                    {
                        textbox.Text = formatChannel(textbox.Text);
                        textbox.Text = textbox.Text.ToUpper();
                    }
                }
            
                if (textbox.Name.Contains("Freq"))
                {
                    // maa haandtere 23, 237, 237., 237.0, 237.00 og 237.000
                    // håndterer ved å konvertere alle frekvenser til følgende
                    // 23.000, 237.000, 237.000, 237.000, 237.000 og 237.000
                    // append forskjellige antall 000

                    // xxx
                    if (s.Length == 3 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]))
                    {
                        s += ".000";
                    // xxx.
                    } else if (s.Length == 4 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]) && s[3] == '.')
                    {
                        s += "000";
                    // xxx.x
                    } else if (s.Length == 5 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]) && s[3] == '.' && Char.IsDigit(s[4]))
                    {
                        s += "00";
                    // xxx.xx
                    } else if (s.Length == 6 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]) && s[3] == '.' && Char.IsDigit(s[4]) && Char.IsDigit(s[5]))
                    {
                        s += "0";
                    // xx
                    } else if (s.Length == 2 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]))
                    {
                        s += ".00";
                    // xx.
                    } else if (s.Length == 3 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '.')
                    {
                        s += "000";
                    // xx.x
                    } else if (s.Length == 4 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '.' && Char.IsDigit(s[3]))
                    {
                        s += "0";
                    // xx.xx
                    } else if (s.Length == 5 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '.' && Char.IsDigit(s[3]) && Char.IsDigit(s[4]))
                    {
                        /* Can be both length 5 and 6 in the list, so has to find out which it is
                         * If it is 6, meaning search will yield no match, add a zero */
                        tuple = (Tuple)list.Find(x => x.getFreq().ToLower().Equals(s.ToLower()));
                        if (tuple == null) s += "0";
                    // xx.xxx
                    } else if (s.Length == 6 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '.' && Char.IsDigit(s[3]) && Char.IsDigit(s[4]))
                    {
                        s = s.Substring(0, 5);
                    }
                    tuple = (Tuple)list.Find(x => x.getFreq().ToLower().Equals(s.ToLower()));
                }


                string freq = "", preset = "", channel = "";
                
                if (tuple != null)
                {
                    freq = tuple.getFreq();
                    preset = tuple.getPreset();
                    channel = tuple.getChannel();

                    if (textbox.Name.Equals("txtAwacsChannel") && txtAwacsFreq.Text == "" && txtAwacsPreset.Text == "")
                    {
                        txtAwacsFreq.Text = freq;
                        txtAwacsPreset.Text = preset;

                        textbox.Text = channel; //replaced textbox.Text = textbox.Text.ToUpper(); 04/07-2017
                    }

                    if (textbox.Name.Equals("txtAwacsBackupChannel") && txtAwacsBackupFreq.Text == "" && txtAwacsBackupPreset.Text == "")
                    {
                        txtAwacsBackupFreq.Text = freq;
                        txtAwacsBackupPreset.Text = preset;

                        textbox.Text = channel; //replaced textbox.Text = textbox.Text.ToUpper(); 04/07-2017
                    }

                    if (textbox.Name.Equals("txtTacpChannel") && txtTacpFreq.Text == "" && txtTacpPreset.Text == "")
                    {
                        txtTacpFreq.Text = freq;
                        txtTacpPreset.Text = preset;

                        textbox.Text = channel; //replaced textbox.Text = textbox.Text.ToUpper(); 04/07-2017
                    }

                    if (textbox.Name.Equals("txtTacpBackupChannel") && txtTacpBackupFreq.Text == "" && txtTacpBackupPreset.Text == "")
                    {
                        txtTacpBackupFreq.Text = freq;
                        txtTacpBackupPreset.Text = preset;

                        textbox.Text = channel; //replaced textbox.Text = textbox.Text.ToUpper(); 04/07-2017
                    }

                    if (textbox.Name.Equals("txtInternalChannel") && txtInternalFreq.Text == "" && txtInternalPreset.Text == "")
                    {
                        txtInternalFreq.Text = freq;
                        txtInternalPreset.Text = preset;

                        textbox.Text = channel; // added 04/07-2017. Nothing here before
                    }

                    if (textbox.Name.Equals("txtInternalBackupChannel") && txtInternalBackupFreq.Text == "" && txtInternalBackupPreset.Text == "")
                    {
                        txtInternalBackupFreq.Text = freq;
                        txtInternalBackupPreset.Text = preset;

                        textbox.Text = channel; // added 04/07-2017. Nnothing here before
                    }

                    if (textbox.Name.Equals("txtAwacsFreq") && txtAwacsChannel.Text == "" && txtAwacsPreset.Text == "")
                    {
                        txtAwacsChannel.Text = channel;
                        txtAwacsPreset.Text = preset;
                        txtAwacsFreq.Text = freq;
                    }

                    if (textbox.Name.Equals("txtAwacsBackupFreq") && txtAwacsBackupChannel.Text == "" && txtAwacsBackupPreset.Text == "")
                    {
                        txtAwacsBackupChannel.Text = channel;
                        txtAwacsBackupPreset.Text = preset;
                        txtAwacsBackupFreq.Text = freq;
                    }

                    if (textbox.Name.Equals("txtTacpFreq") && txtTacpChannel.Text == "" && txtTacpPreset.Text == "")
                    {
                        txtTacpChannel.Text = channel;
                        txtTacpPreset.Text = preset;
                        txtTacpFreq.Text = freq;
                    }

                    if (textbox.Name.Equals("txtTacpBackupFreq") && txtTacpBackupChannel.Text == "" && txtTacpBackupPreset.Text == "")
                    {
                        txtTacpBackupChannel.Text = channel;
                        txtTacpBackupPreset.Text = preset;
                        txtTacpBackupFreq.Text = freq;
                    }

                    if (textbox.Name.Equals("txtInternalFreq") && txtInternalChannel.Text == "" && txtInternalPreset.Text == "")
                    {
                        txtInternalChannel.Text = channel;
                        txtInternalPreset.Text = preset;
                        txtInternalFreq.Text = freq;
                    }

                    if (textbox.Name.Equals("txtInternalBackupFreq") && txtInternalBackupChannel.Text == "" && txtInternalBackupPreset.Text == "")
                    {
                        txtInternalBackupChannel.Text = channel;
                        txtInternalBackupPreset.Text = preset;
                        txtInternalBackupFreq.Text = freq;
                    }
                } else
                {
                    // Add formating to frequency that does is not in the preset-file (pdf)
                    if (textbox.Name.Equals("txtAwacsFreq") && txtAwacsChannel.Text == "" && txtAwacsPreset.Text == "")
                    {
                        txtAwacsFreq.Text = s;
                        txtAwacsChannel.Text = "-";
                        txtAwacsPreset.Text = "-";
                    }

                    if (textbox.Name.Equals("txtAwacsBackupFreq") && txtAwacsBackupChannel.Text == "" && txtAwacsBackupPreset.Text == "")
                    {
                        txtAwacsBackupFreq.Text = s;
                        txtAwacsBackupChannel.Text = "-";
                        txtAwacsBackupPreset.Text = "-";
                    }

                    if (textbox.Name.Equals("txtTacpFreq") && txtTacpChannel.Text == "" && txtTacpPreset.Text == "")
                    {
                        txtTacpFreq.Text = s;
                        txtTacpChannel.Text = "-";
                        txtTacpPreset.Text = "-";
                    }

                    if (textbox.Name.Equals("txtTacpBackupFreq") && txtTacpBackupChannel.Text == "" && txtTacpBackupPreset.Text == "")
                    {
                        txtTacpBackupFreq.Text = s;
                        txtTacpBackupChannel.Text = "-";
                        txtTacpBackupPreset.Text = "-";
                    }

                    if (textbox.Name.Equals("txtInternalFreq") && txtInternalChannel.Text == "" && txtInternalPreset.Text == "")
                    {
                        txtInternalFreq.Text = s;
                        txtInternalChannel.Text = "-";
                        txtInternalPreset.Text = "-";
                    }

                    if (textbox.Name.Equals("txtInternalBackupFreq") && txtInternalBackupChannel.Text == "" && txtInternalBackupPreset.Text == "")
                    {
                        txtInternalBackupFreq.Text = s;
                        txtInternalBackupChannel.Text = "-";
                        txtInternalBackupPreset.Text = "-";
                    }
                }
            }
        }

        private void errorInvalidFormat()
        {
            MessageBox.Show("Format: COLOR # e.g. YELLOW 9", "INVALID FORMAT");
        }

        private string stripArrlocAndDeplocFromAirportIdentifier(string x)
        {
            string deploc = "DEPLOC:";
            string arrloc = "ARRLOC:";
            string altloc = "ALTLOC:";
            string bckloc = "BCKLOC:";
            if (x.StartsWith(deploc) || x.StartsWith(arrloc) || x.StartsWith(altloc) || x.StartsWith(bckloc)) {
                x = x.Remove(0,7);
            }
            return x;
        }

        private void setAirport(string identifier)
        {
            DataGridViewRow row = null;

            if (identifier.StartsWith("DEPLOC:"))
            {
                row = dgvAirbase.Rows[0];
                cmbAirbaseDep.Text = stripArrlocAndDeplocFromAirportIdentifier(identifier);
            } else if (identifier.StartsWith("ARRLOC:"))
            {
                row = dgvAirbase.Rows[1];
                cmbAirbaseArr.Text = stripArrlocAndDeplocFromAirportIdentifier(identifier);
            } else if (identifier.StartsWith("ALTLOC:"))
            {
                row = dgvAirbase.Rows[2];
                cmbAirbaseAlt.Text = stripArrlocAndDeplocFromAirportIdentifier(identifier);
            } else if (identifier.StartsWith("BCKLOC:"))
            {
                row = dgvAirbase.Rows[3];
                cmbAirbaseBck.Text = stripArrlocAndDeplocFromAirportIdentifier(identifier);
            }

            /* GEORGIA MAP: Using configuration files for all airbases, one for each theatre */
            setAirbase(row, stripArrlocAndDeplocFromAirportIdentifier(identifier));
        }

        private void clearRow(int i)
        {
            var row = dgvAirbase.Rows[i];
            row.Cells["colAirbase"].Value = "";
            row.Cells["colTcn"].Value = "";
            row.Cells["colGnd"].Value = "";
            row.Cells["colTwr"].Value = "";
            row.Cells["colTma"].Value = "";
            row.Cells["colElev"].Value = "";
            row.Cells["colRwy"].Value = "";
            row.Cells["colIls"].Value = "";
        }

        private void initSupportRow(int i)
        {
            var row = dgvSupport.Rows[i];
            row.Cells["colCallsignSupport"].Value = "-";
            row.Cells["colFreqSupport"].Value = "-";
            row.Cells["colChannelSupport"].Value = "-";
            row.Cells["colPresetSupport"].Value = "-";
            row.Cells["colBackupSupport"].Value = "-";
        }

        private void comboboxAirportSelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;

            string identifier = null;

            if (sender != null)
            {
                ComboBox cb = (ComboBox)sender;
                if (cb.Name == "cmbAirbaseDep")
                {
                    identifier = "DEPLOC:" + combobox.Text;
                }
                else if (cb.Name == "cmbAirbaseArr")
                {
                    identifier = "ARRLOC:" + combobox.Text;
                }
                else if (cb.Name == "cmbAirbaseAlt")
                {
                    identifier = "ALTLOC:" + combobox.Text;
                } else if (cb.Name == "cmbAirbaseBck")
                {
                    identifier = "BCKLOC:" + combobox.Text;
                }
            }

            setAirport(identifier);
        }

        private void genAirbaseObj()
        {
            airbases = new List<Airbase>();

            string path = Environment.CurrentDirectory + "\\" + "airbases-caucasus.csv";   

            using (StreamReader reader = new StreamReader(path))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();

                    string id, name, tcn, gnd, twr, tma, elev, rws, ils;
                    id = name = tcn = gnd = twr = tma = elev = rws = ils = null;

                    int valueNr = 0;
                    foreach (string word in line.Split(','))
                    {
                        if (valueNr == 0) id = word.Trim();
                        if (valueNr == 1) name = word.Trim();
                        if (valueNr == 2) tcn = word.Trim();
                        if (valueNr == 3) gnd = word.Trim();
                        if (valueNr == 4) twr = word.Trim();
                        if (valueNr == 5) tma = word.Trim();
                        if (valueNr == 6) elev = word.Trim();
                        if (valueNr == 7) rws = word.Trim();
                        if (valueNr == 8) ils = word.Trim();

                        valueNr++;
                    }
                    Airbase ab = new Airbase(id, name, tcn, gnd, twr, tma, elev, rws, ils);
                    airbases.Add(ab);
                }
            }
        }

        /* Parses 132nd operational frequencies, but remember to open, view source, and mark and copy code from the newest operational frequencies table and putting it in /bin/debug manually before running. */
        private void parseOperationalFreq()
        {
            Tuple t;

            string path = Environment.CurrentDirectory + "\\" + "view-source_www.132virtualwing.org_index.php_page_freqlist.html";

            string table = null;

            using (StreamReader reader = new StreamReader(path))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    if (line.StartsWith("<table id=\"freqlist\""))
                    {
                        // found the correct line that contains the whole frequency table
                        table = line;
                        break;
                    }
                }
            }

            string color = null;
            string freq = null;
            int nr = 1;
            Tuple tuple = null;

            // need to split up the table
            foreach (string word in table.Split('<', '>')) {
                if (isColor(word))
                {
                    color = word;
                }
                else if (isFreq(word))
                {
                    freq = word;

                    // does frequency exist from before, from the preset list?
                    try
                    {
                        tuple = (Tuple)list.Find(x => x.getChannel().ToLower().Equals(formatChannel((color + ' ' + nr.ToString()).ToLower())));
                    } catch (NullReferenceException)
                    {
                        tuple = null;
                    }

                    if (tuple == null)
                    {
                        // if not, add the frequency
                        tuple = new Tuple(); // Create new item when encountering new preset
                        tuple.setChannel(color.ToUpper() + ' ' + nr);
                        tuple.setFreq(word);
                        list.Add(tuple); // Add item to list here because all items has a frequency
                    }

                    if (nr == 11)
                    {
                        // reset: important for blank frequency slots being filtered out
                        nr = 1;
                        color = freq = null;
                        tuple = null;
                    } else
                    {
                        nr++;
                    }
                // removes blank frequency slots in table
                } else if (word.Trim().Equals("") && color != null)
                {
                    nr++;
                }
            }
        }

        /* Populates range objects based on the ranges-caucasus.csv file */
        private void genRangeObj()
        {
            ranges = new List<Range>();

            string path = Environment.CurrentDirectory + "\\" + "ranges-caucasus.csv";

            using (StreamReader reader = new StreamReader(path))
            {
                /* initialize variables */
                string range, cp, abbrev, joker, bingo, freq, backup, fp, amp;
                range = cp = abbrev = joker = bingo = freq = backup = fp = amp = null;

                /* read lines */
                int lineNr = 0;
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();

                    /* reading line 1 */
                    if (lineNr % 3 == 0)
                    {
                        range = cp = abbrev = joker = bingo = freq = backup = fp = amp = null;

                        int wordNr = 0;
                        foreach (string word in line.Split(','))
                        {
                            if (wordNr == 0) range = word.Trim();
                            if (wordNr == 1) cp = word.Trim();
                            if (wordNr == 2) abbrev = word.Trim();
                            if (wordNr == 3) joker = word.Trim();
                            if (wordNr == 4) bingo = word.Trim();
                            if (wordNr == 5) freq = word.Trim();
                            if (wordNr == 6) backup = word.Trim();

                            wordNr++;
                        }

                    /* reading line 2 */
                    } else if (lineNr % 3 == 1)
                    {
                        fp = line.Trim();
                    
                    /* reading line 3 */
                    } else if (lineNr % 3 == 2)
                    {
                        amp = line.Trim();

                        /* Done with reading one range object: Save */
                        Range rng = new Range(range, cp, abbrev, joker, bingo, freq, backup, fp, amp);
                        ranges.Add(rng);
                    }

                    lineNr++;
                }
            }
        }

        private void setAirbase(DataGridViewRow r, string id)
        {
            Airbase ab = airbases.Find(x => x.getIdentifier(id).Equals(id));
            var row = r;

            if (ab == null)
            {
                MessageBox.Show("Airbase was not found in the .csv file");
            } else
            {
                row.Cells[0].Value = ab.getName();
                row.Cells[1].Value = ab.getTacan();
                row.Cells[2].Value = ab.getGround();
                row.Cells[3].Value = ab.getTower();
                row.Cells[4].Value = ab.getTma();
                row.Cells[5].Value = ab.getElevation();
                row.Cells[6].Value = ab.getRunways();
                row.Cells[7].Value = ab.getIls();
            }
        }

        private void setTbilisi(DataGridViewRow r, string identifier)
        {

            if (identifier.StartsWith("DEPLOC:"))
            {
                txtParking.Text = "APRON 1";
            }

            enableTma();

            string fpTma = "UGTB3," // TMA          // UGTB3-8                              tma
                         + "UGTB4,"
                         + "UGTB5,"
                         + "UGTB6,"
                         + "UGTB7,"
                         + "UGTB8,"
                         + "UGTB1,"                 // UGTB1-3
                         + "UGTB2,"
                         + "UGTB3,"
                         + "INIT POSIT,"            // INIT POSIT, TBILISILOCH              starting position
                         + "TBILISILOCH,"           // TBILISILOCH, INIT POSIT
                         + "MUKHRANI/NDB"           // NEED STRAIGHT LINE TO DO MUK2DEP
                         + "DEDON,"
                         + "UG24 SOUTH,"            // UG24 SOUTH, UG24 LAKE, UG24 NORTH    possible exits
                         + "UG24 LAKE,"
                         + "UG24 NORTH,"
                         + "GLDANI,"                // GLDANI, MUKHRANI/NDB, OBORA          possible navigation points
                         + "OBORA,"
                         + "GIMUR/NDB";             // GIMUR/NDB                            endpoint

            /* LONG flightplan commented out for now: Keep if needed later
                string fpTma = "UGTB3," // TMA          // UGTB3-8                              tma
                             + "UGTB4,"
                             + "UGTB5,"
                             + "UGTB6,"
                             + "UGTB7,"
                             + "UGTB8,"
                             + "UGTB1,"                 // UGTB1-3
                             + "UGTB2,"
                             + "UGTB3,"
                             + "INIT POSIT,"            // INIT POSIT, TBILISILOCH              starting position
                             + "MUKHRANI/NDB"           // NEED STRAIGHT LINE TO DO MUK2DEP
                             + "TBILISILOCH,"
                             + "UG24 SOUTH,"            // UG24 SOUTH, UG24 LAKE, UG24 NORTH    possible exits
                             + "UG24 LAKE,"
                             + "UG24 NORTH,"
                             + "GLDANI,"                // GLDANI, MUKHRANI/NDB, OBORA          possible navigation points
                             + "OBORA,"
                             + "GIMUR/NDB,"             // GIMUR/NDB                            endpoint

                             // Consider removing these
                             + "OBORA,"                 // OBORA, MUKHRANI/NDB, GLDANI          rtb
                             + "MUKHRANI/NDB,"
                             + "GLDANI,"
                             + "UG24 NORTH,"            // UG24 NORTH, UG24 LAKE, UG24 SOUTH
                             + "UG24 LAKE,"
                             + "UG24 SOUTH,"
                             + "DEDON,"                 // DEDON (reconsider)
                             + "TBILISILOCH,"           // TBILISILOCH, INIT POSIT
                             + "INIT POSIT";            // NOTE: Also considered LAMUS, DEDON, SAMGORI and UG27 FOXTROTT also, but MAYBE overkill.Can always insert waypoint live if needed.Only take the most likely ones.The rest can be inserted live if needed. */
        }

        private void disableTma()
        {
            hasTma = false;
            // txtTma.Enabled = false;
            // txtTmaFreq.Enabled = false;
            // txtTma.Text = "";
        }

        private void enableTma()
        {
            hasTma = true;
            // txtTma.Enabled = true;
            // txtTmaFreq.Enabled = true;
        }

        private void setSenaki(DataGridViewRow r, string identifier)
        {

            /* Disabling TMA since this airport has no approach */
            
            string fpTma = "SENAKI-KOLKH,"
                         + "UGKS KETILAR,"
                         + "UKGS NOSIRI,"
                         + "UGKS RIVER W,"
                         + "UGKS ZENI";

            var row = r;

            row.Cells["colAirbase"].Value = "Senaki"; // Remember to add only Lochini to the kneeboard
            row.Cells["colTcn"].Value = "31X";
            row.Cells["colGnd"].Value = "132.1";
            row.Cells["colTwr"].Value = "132.2";
            row.Cells["colElev"].Value = "43";
            row.Cells["colRwy"].Value = "09/27";
            row.Cells["colIls"].Value = "108.90/-";

            if (identifier.StartsWith("DEPLOC:"))
            {
                txtParking.Text = "RAMP 1 SHELTER 23";
                disableTma();
            }
        }

        private void txtParking_Enter(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int visibleTime = 10000;

            //ToolTip tt = new ToolTip();
            //tt.Show("APRON#/RAMP#/PARKING SPACE#/SHELTER#", tb,visibleTime);
        }

        private void textboxLeaveLocationEventHandler(object sender, EventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            string s = textbox.Text;
            restore();

            string tianeti = "Tianeti range";
            string dusheti = "Dusheti range";
            string tetra = "Tetra range";
            string marnueli = "Marnueli range";
            string kutaisi = "Kutaisi range";
            string tkibuli = "Tkibuli range";
            string moas = "MOA South";
            string moan = "MOA North";

            string fpTet = "MUKHRANI/NDB," // TIA   // MUKHRANI/NDB, OBORA, GIMUR/NDB
                         + "OBORA,"
                         + "GIMUR/NDB,"
                         + "J05,"                   // J05-01
                         + "J04,"
                         + "J03,"
                         + "J02,"
                         + "J01,"
                         + "TE1,"                   // TE1-5, TE1
                         + "TE2,"
                         + "TE3,"
                         + "TE4,"
                         + "TE5,"
                         + "TE1";

            string fpDushex = "DR1,"
                            + "DR6,"
                            + "DR7,"
                            + "DR8,"
                            + "DR9,"
                            + "DR4,"
                            + "DR5,"
                            + "DR1,"
                            + "G01,"
                            + "G02,"
                            + "G03,"
                            + "G04";
            // Range freq (if range has JTAC (JTAC is enabled): set JTACs frequency and range frequency, else set only range frequency)

            string fpDush = "DR1,"
                          + "DR2,"
                          + "DR3,"
                          + "DR4,"
                          + "DR5,"
                          + "G01,"
                          + "G02,"
                          + "G03,"
                          + "G04";

            string fpTia = "TR1,"
                         + "TR2,"
                         + "TR3,"
                         + "TR4,"
                         + "TR5,"
                         + "TR6,"
                         + "G05,"
                         + "G06,"
                         + "H01,"
                         + "H02,"
                         + "H03,"
                         + "H04";

            string fpMar = "MR1,"
                         + "MR2,"
                         + "MR3,"
                         + "MR4";

            if (s != "")
            {

                /* TIANETI */
                if (tianeti.ToLower().Contains(s.ToLower()))
                {
                    setRange("TIA");

                    /* DUSHETI */
                }
                else if (dusheti.ToLower().Contains(s.ToLower()))
                {
                    setRange("DUSHEX");
                    /* string amp = "FAH 022 +-30. All conventional ordnance authorized. On the strafe panels, only guns is authorized.";
                    setRangeInfo(dusheti, "GIMUR", fpDushex, "DUSHEX", amp, 2900, 1900);
                    enableTacp();
                    lblJTAC.Text = "Range";
                    setTacpFreq("247.500", "LIME 11", 6, "140.750", "INDIGO 9", 0);
                    if (txtTacpCallsign.Text == "") txtTacpCallsign.Text = "Dusheti";*/

                    /* TETRA */
                }
                else if (tetra.ToLower().Contains(s.ToLower()))
                {
                    /* BAGEM is in the middle of TETRA.
                     * LAGAS is in the far west of TETRA, at the westernmost border of the range
                     */

                    setRange("TET");

                    /* MARNUELI */
                }
                else if (marnueli.ToLower().Contains(s.ToLower()))
                {
                    /* Obora is in the Tbilisi TMA, and very close to MUKHRANI.
                     * TISOT is in MARNUELI
                     */
                    setRange("MAR");

                    /*string amp = "FAH 225 +-30. All conventional ordnance authorized. On the strafe panels, only guns is authorized.";
                    setRangeInfo(marnueli, "OBORA", fpMar, "MAR", amp, 2900, 1900);
                    enableTacp();
                    lblJTAC.Text = "Range";
                    setTacpFreq("248.500", "PURPLE 1", 7, "124.750", "AMBER 1", 0);
                    if (txtTacpCallsign.Text == "") txtTacpCallsign.Text = "Marnueli";*/

                    /* KUTAISI */
                }
                else if (kutaisi.ToLower().Contains(s.ToLower()))
                {
                    setRange("KUT");
                    /*string amp = "PLACEHOLDER AMPLIFICATION KUTAISI";
                    setRangeInfo(kutaisi, "GIMUR", null, "KUT", amp, 4500, 3500);
                    enableTacp();
                    lblJTAC.Text = "Range";
                    setTacpFreq("233.000", "PINK 1", 0, "", "", 0);
                    if (txtTacpCallsign.Text == "") txtTacpCallsign.Text = "Kutaisi";
                    */

                    /* TKIBULI */
                }
                else if (tkibuli.ToLower().Contains(s.ToLower()))
                {
                    setRange("TKI");
                    /*
                    string amp = "PLACEHOLDER AMPLIFICATION TKIBULI";
                    setRangeInfo(tkibuli, "GIMUR", null, "TKI", amp, 4000, 3000);
                    enableTacp();
                    lblJTAC.Text = "Range";
                    setTacpFreq("230.000", "YELLOW 6", 0, "", "", 0);
                    if (txtTacpCallsign.Text == "") txtTacpCallsign.Text = "Tkibuli";
                    */

                    /* MOA SOUTH */
                }
                else if (moas.ToLower().Contains(s.ToLower()))
                {
                    setRange("MOAS");

                    /* MOA NORTH */
                }
                else if (moan.ToLower().Contains(s.ToLower()))
                {
                    setRange("MOAN");

                }
            }
        }

        private void setRange(string abbrev)
        {
            /* find correct range */
            Range rng = ranges.Find(x => x.getAbbrev().Equals(abbrev));

            if (rng != null)
            {
                setRangeInfo(rng.getRange(), rng.getCp(), rng.getFp(), rng.getAbbrev(), rng.getAmp(), Int32.Parse(rng.getJoker()), Int32.Parse(rng.getBingo()));
                enableTacp();
                // lblJTAC.Text = "Range";
                setRadio("TACP", formatChannel(rng.getFreq()), formatChannel(rng.getBackup()));
                setRangeCallsign(txtTacpCallsign.Text, rng.getRange());
            }
        }

        private void setRangeCallsign(string callsign, string range)
        {
            if (callsign == "")
            {
                if (range.ToLower().Contains("range"))
                {
                    txtTacpCallsign.Text = range.Split(' ')[0].ToString();
                } else
                {
                    txtTacpCallsign.Text = range;
                }
            }
        }

        private void setRangeInfo(string loc, string cp, string fp, string lbl, string amp, int joker, int bingo)
        {
            txtLocation.Text = loc;
            txtTacpCp.Text = cp;
            // txtFlightplan2.Text = fp.Replace(",", Environment.NewLine);
            // lblFp2.Text = lbl;
            Properties.Settings.Default.prevAmpn = amp;
            setFuel(joker, bingo);
        }

        private void setFuel(int joker, int bingo)
        {
            Properties.Settings.Default.prevTxtJoker = joker.ToString();
            Properties.Settings.Default.prevTxtBingo = bingo.ToString();
        }

        private void initFuel()
        {
            Properties.Settings.Default.prevTxtJoker = "2000";
            Properties.Settings.Default.prevTxtBingo = "1500";
        }

        private void setTacpFreq(string freq, string channel, int preset, string bkpFreq, string bkpChannel, int bkpPreset)
        {
            txtTacpFreq.Text = freq;
            txtTacpChannel.Text = channel;

            if (preset == 0)
            {
                txtTacpPreset.Value = 0;
                txtTacpPreset.Text = "";
            } else
            {
                txtTacpPreset.Value = preset;
            }

            txtTacpBackupFreq.Text = bkpFreq;
            txtTacpBackupChannel.Text = bkpChannel;

            if (bkpPreset == 0)
            {
                txtTacpBackupPreset.Value = 0;
                txtTacpBackupPreset.Text = "";
            } else
            {
                txtTacpBackupPreset.Value = bkpPreset;
            }
        }

        private void setAwacsFreq(string freq, string channel, int preset, string bkpFreq, string bkpChannel, int bkpPreset)
        {
            txtAwacsFreq.Text = freq;
            txtAwacsChannel.Text = channel;

            if (preset == 0)
            {
                txtAwacsPreset.Value = 0;
                txtAwacsPreset.Text = "";
            }
            else
            {
                txtAwacsPreset.Value = preset;
            }

            txtAwacsBackupFreq.Text = bkpFreq;
            txtAwacsBackupChannel.Text = bkpChannel;

            if (bkpPreset == 0)
            {
                txtAwacsBackupPreset.Value = 0;
                txtAwacsBackupPreset.Text = "";
            }
            else
            {
                txtAwacsBackupPreset.Value = bkpPreset;
            }
        }

        private void CheckCheck(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox cb = sender as System.Windows.Forms.CheckBox;
            if (cb.CheckState == CheckState.Checked)
            {
                HandleCheck(sender, e);
            } else
            {
                HandleUncheck(sender, e);
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

        private void disableAwacs()
        {
            setFreq("awacs", false);
        }

        private void enableAwacs()
        {
            setFreq("awacs", true);
        }

        private void disableTacp()
        {
            this.ActiveControl = txtMetar;
            setFreq("tacp", false);
            this.ActiveControl = txtMetar;
        }

        private void enableTacp()
        {
            setFreq("tacp", true);
        }

        void disableFocus()
        {
            //lblJTAC.Focus();
            
        }

        /* 
         * name has to be either awacs, tacp, (or internal)
         */
        private void setFreq(string name, Boolean value)
        {
            setControl("txt" + name.First().ToString().ToUpper() + name.Substring(1), value);
            setControl("txt" + name.First().ToString().ToUpper() + name.Substring(1) + "Backup", value);
            setControl("lbl" + name.First().ToString().ToUpper() + name.Substring(1), value);
            setControl("lbl" + name.First().ToString().ToUpper() + name.Substring(1) + "Backup", value);
            setCheckBox(name, value);
            setCallsignAndCpIfExists(name, value);
        }

        private void setCallsignAndCpIfExists(string name, Boolean value)
        {
            setControl(this.Controls.Find("txt" + name + "Callsign", true), value);
            setControl(this.Controls.Find("txt" + name + "Cp", true), value);
            setControl(this.Controls.Find("lbl" + name + "Callsign", true), value);
            setControl(this.Controls.Find("lbl" + name + "Cp", true), value);
        }

        private void setControl(Control[]c, Boolean value)
        {
            if (c != null && c.Length > 0)
            {
                if (c[0].Name.Contains("txt") && value == false)
                {
                    c[0].Text = "";
                }
                c[0].Enabled = value;
            }
        }

        private void setCheckBox(string name, Boolean value)
        {
            System.Windows.Forms.CheckBox cb = this.Controls.Find("chk" + name, true)[0] as System.Windows.Forms.CheckBox;
            cb.Checked = value;
        }

        private void setControl(string name, Boolean value)
        {
            setFreqItem(name + "Freq", value);
            setFreqItem(name + "Channel", value);
            setFreqItem(name + "Preset", value);
        }

        private void setFreqItem(string name, Boolean value)
        {
            Control c = this.Controls.Find(name, true)[0];
            if (value == false && name.Contains("txt"))
            {
                // clears frequency/channel,preset when disabled
                c.Text = "";
            }
            c.Enabled = value;
        }

        private void HandleCheck(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox cb = sender as System.Windows.Forms.CheckBox;
            if (cb.Name == "chkAwacs")
            {
                enableAwacs();
            } else if (cb.Name == "chkTacp")
            {
                enableTacp();
            }
        }

        private void HandleUncheck(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox cb = sender as System.Windows.Forms.CheckBox;
            if (cb.Name == "chkAwacs")
            {
                disableAwacs();
            } else if (cb.Name == "chkTacp")
            {
                disableTacp();
            }
        }

        private void clearTacpFreq()
        {
            setTacpFreq("", "", 0, "", "", 0);
            txtTacpCallsign.Text = "";
            txtTacpCp.Text = "";
        }

        private void createCommunicationHelp()
        {
            /* Only run SearchAndReplace if CommunicationHelp checkbox is checked in Settings */
            if (Properties.Settings.Default.CommunicationHelp)
            {
                string inputPath = Properties.Settings.Default.filePathCommunication;
                string fnIn = chooseTemplate();
                string fnOut = "temp.docm";
                moveFile(fnIn, fnOut, inputPath);
                initSearchAndReplace();
                storeAsPdf(Environment.CurrentDirectory);
                splitPdf(Environment.CurrentDirectory);
                setKneeboardPath();
                storePngsToKneeboardBuilder();
                delete(Environment.CurrentDirectory + @"\" + "temp.docm");
            }
        }

        private void initSearchAndReplace()
        {
            // DEPARTURE AND ARRIVAL
            SearchAndReplace("/CALLSIGN/", txtCallsign.Text); // TODO: Parse to get just callsign
            SearchAndReplace("/FLIGHT/", GetLettersOnly(txtCallsign.Text)); // TODO: Parse to get just letters (no digits) (split på 1, 2, 3, 4, 5, 6, 7, 8 og 9? og bare lagre words[0] bør funke)
            SearchAndReplace("/#AC/", convertDigitToNumber(cmbNrOfAc.Text));

            var row = dgvAirbase.Rows[0];

            SearchAndReplace("/AIRPORT/", row.Cells["colAirbase"].Value as string); // TODO: Transform to human-readable name TODO2: Separate departure and arrival airfield
            SearchAndReplace("/TMA/", row.Cells["colTma"].Value.ToString().TrimEnd("0".ToCharArray())); // TODO: Transform to human-readable name and select correct TMA for each airfield TODO: Change it to AIRPORTA for arrival and AIRPORTD for departure in the word document
            SearchAndReplace("/PARKING/", txtParking.Text);

            searchAndReplaceRunwayHeadings();

            // Need to make separate codewords for DEP and ARR, e.g. /GNDFRQDEP/ and /GNDFRQARR/
            SearchAndReplace("/GNDFRQ/", row.Cells["colGnd"].Value.ToString().TrimEnd("0".ToCharArray()));

            // Need to make separate codewords for DEP and ARR, e.g. /TWRFRQDEP/ and /TWRFRQARR/
            SearchAndReplace("/TWRFRQ/", row.Cells["colTwr"].Value.ToString().TrimEnd("0".ToCharArray()));

            // Need to make separate codewords for DEP and ARR, e.g. /TMAFRQDEP/ and /TMAFRQARR/
            SearchAndReplace("/TMAFRQ/", row.Cells["colTma"].Value.ToString().TrimEnd("0".ToCharArray()));


            // Remember to add only Lochini to the kneeboard

            // AWACS
            SearchAndReplace("/AWACS/", txtAwacsCallsign.Text);
            SearchAndReplace("/AWACSFRQ/", txtAwacsFreq.Text.TrimEnd("0".ToCharArray()));
            SearchAndReplace("/AWACSCNL/", txtAwacsChannel.Text);
            SearchAndReplace("/AWACSPST/", "P" + txtAwacsPreset.Text);

            SearchAndReplace("/CP/", txtTacpCp.Text);

            // TACP
            SearchAndReplace("/TACP/", txtTacpCallsign.Text);
            SearchAndReplace("/TACPFRQ/", txtTacpFreq.Text.TrimEnd("0".ToCharArray()));
            SearchAndReplace("/TACPCNL/", txtTacpChannel.Text);
            SearchAndReplace("/TACPPST/", "P" + txtTacpPreset.Text);

            SearchAndReplace("/LOC/", txtLocation.Text);
            SearchAndReplace("/MSN#/", txtMsnNr.Text);
            SearchAndReplace("/TASKING/", txtTasking.Text);

            row = dgvSupport.Rows[5];

            // IFRN
            SearchAndReplace("/IFRNFRQ/", row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray()));
            SearchAndReplace("/IFRNCNL/", row.Cells["colChannelSupport"].Value as string);
            SearchAndReplace("/IFRNPST/", "P" + row.Cells["colPresetSupport"].Value as string);

            SearchAndReplace("one by ALPHA 10 CHARLIEs", "singleship ALPHA 10 CHARLIE"); // TODO: Does not work, maybe because one is still /#AC/? Then I would have to wait before searching and replacing.
        }

        private void searchAndReplaceRunwayHeadings()
        {
            var row = dgvAirbase.Rows[0];

            // Need to make separate codewords for DEP and ARR, e.g. /#RD/ and /#RA/ (for departure and arrival airbases)
            string runways = (string)row.Cells["colRwy"].Value;
            foreach (string runway in runways.Split('/'))
            {
                bool second = false;

                if (runway.Contains('L'))
                {
                    SearchAndReplace("/#L/", runway);
                }
                else if (runway.Contains('R'))
                {
                    SearchAndReplace("/#R/", runway);
                }
                else
                {
                    if (second)
                    {
                        SearchAndReplace("/#R", runway);
                    }
                    else
                    {
                        SearchAndReplace("/#L/", runway);
                    }
                }
                second = true;
            }
        }

        /* Save file from one location to another */
        private void moveFile(string fnIn, string fnOut, string inputPath)
        {
            if (File.Exists(Environment.CurrentDirectory + @"\" + fnOut))
            {
                delete(Environment.CurrentDirectory + @"\" + fnOut);
            }

            try
            {
                File.Copy(inputPath + @"\" + fnIn, Environment.CurrentDirectory + @"\" + fnOut);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Cannot find the directory:\n" + inputPath + "\n\nClosing application", "Error: Directory not found");
                Environment.Exit(1); // ToDo: Best practice to use Application.Exit() when using a Windows form
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Cannot find the file:\n" + inputPath + @"\" + fnIn + "\n\nClosing application", "Error: File not found");
                Environment.Exit(1); // ToDo: Best practice to use Application.Exit() when using a Windows form
            }
        }

        /* Selects template based on settings in form */
        private string chooseTemplate()
        {
            string fnIn = "Communications.docm";
            if (hasTma == false && chkAwacs.Checked)
            {
                fnIn = "CommunicationsNoTma.docm";
            }

            if (hasTma && chkAwacs.Checked == false)
            {
                fnIn = "CommunicationsNoAwacs.docm";
            }
            return fnIn;
        }

        private void showMDC2()
        {
            // sending airbases to form2
            var row = dgvAirbase.Rows[0];

            row = dgvAirbase.Rows[0];
            string dep = (string)row.Cells["colAirbase"].Value;
            row = dgvAirbase.Rows[1];
            string arr = (string)row.Cells["colAirbase"].Value;
            row = dgvAirbase.Rows[2];
            string alt = (string)row.Cells["colAirbase"].Value;

            Form2 form2 = new Form2(blank(dep), blank(arr), blank(alt), txtLocation.Text, selectTac(), loadPrev);
            form2.Show(); // Show next flight form
            Hide(); // Hide form1
        }

        private string selectTac()
        {
            if (chkTacp.Checked)
            {
                return txtTacpCallsign.Text;
            }
            else
            {
                if (isAwacsGround())
                {
                    return getAwacsGround();
                }
                else
                {
                    return txtAwacsCallsign.Text;
                }
            }
            return "";
        }

        private bool isAwacsGround()
        {
            int rowNr = 0;
            while (rowNr < dgvSupport.RowCount)
            {
                var row = dgvSupport.Rows[rowNr];
                string value = row.Cells["colTypeSupport"].Value as string;
                if (value.Contains("AWACS A-G"))
                {
                    if (isFreq(row.Cells["colFreqSupport"].Value.ToString()))
                    {
                        return true;
                    }
                    break;
                }
                rowNr++;
            }
            return false;
        }

        private string getAwacsGround()
        {
            int rowNr = 0;
            while (rowNr < dgvSupport.RowCount)
            {
                var row = dgvSupport.Rows[rowNr];
                if (row.Cells["colTypeSupport"].Value.ToString().Contains("AWACS A-G"))
                {
                    if (!row.Cells["colCallsignSupport"].Value.Equals("") && !row.Cells["colCallsignSupport"].Value.Equals("-"))
                    {
                        return row.Cells["colCallsignSupport"].Value.ToString();
                    } else
                    {
                        return row.Cells["colTypeSupport"].Value.ToString();
                    }
                    break;
                }
                rowNr++;
            }
            return null;
        }

        private void txtTime_Leave(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb.Text.Length == 4)
            {
                tb.Text = tb.Text.Substring(0, 2) + ":" + tb.Text.Substring(2, 2);
            }

            if (tb.Text.Length == 0)
            {
                tb.Text += ":";
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            lblAltRes.Focus(); // Move focus away so no blue markings appear on screenshot
            System.Threading.Thread.Sleep(50); // sleep to make sure focus is moved
            createCommunicationHelp();

            btnSubmit.Hide();

            string pathA10c = @"\Kneeboard Groups\A-10C";
            
            captureScreen(Properties.Settings.Default.pathKneeboardBuilder + pathA10c);

            saveForm();
            showMDC2();
        }

        private void storePngsToKneeboardBuilder()
        {
            string pathA10c = @"\Kneeboard Groups\A-10C";
            string pathStandardTraining = @"\StandardTraining";
            string noAwacs = @"\NoAwacs";
            string noTacp = @"\NoTacp";

            if (@"C:\Users\blue\Programs\KneeboardBuilder\Kneeboard Groups\A-10C".Equals(Properties.Settings.Default.pathKneeboardBuilder + pathA10c))
            {
                /* Create directory if it does not exist */
                Directory.CreateDirectory(Properties.Settings.Default.pathKneeboardBuilder + pathA10c);

                toKneeboardBuilder(Properties.Settings.Default.pathKneeboardBuilder + pathA10c);
            }
        }

        private string blank(string airport)
        {
            if (airport == null)
            {
                return "";
            }
            return airport;
        }

        private void saveForm()
        {
            Properties.Settings.Default.prevTxtCallsign = txtCallsign.Text;
            Properties.Settings.Default.prevTxtNrOfAc = cmbNrOfAc.Text;
            Properties.Settings.Default.prevTxtMsnNr = txtMsnNr.Text;
            Properties.Settings.Default.prevTxtTasking = txtTasking.Text;
            Properties.Settings.Default.prevTxtLocation = txtLocation.Text;

            /* AIRPORT */
            Properties.Settings.Default.prevCmbAirbaseDep = cmbAirbaseDep.Text;
            Properties.Settings.Default.prevCmbAirbaseArr = cmbAirbaseArr.Text;
            Properties.Settings.Default.prevCmbAirbaseAlt = cmbAirbaseAlt.Text;
            Properties.Settings.Default.prevCmbAirbaseBck = cmbAirbaseBck.Text;

            Properties.Settings.Default.prevTxtStepTime = txtStepTime.Text;
            Properties.Settings.Default.prevTxtTaxiTime = txtTaxiTime.Text;
            Properties.Settings.Default.prevTxtTakeoffTime = txtTakeoffTime.Text;
            Properties.Settings.Default.prevTxtVulStart = txtVulStart.Text;
            Properties.Settings.Default.prevTxtVulEnd = txtVulEnd.Text;
            Properties.Settings.Default.prevTxtLandingTime = txtLandingTime.Text;

            Properties.Settings.Default.prevTxtParking = txtParking.Text;
            
            saveDGVAirbases();

            /* AWACS */
            if (chkAwacs.Checked == true) Properties.Settings.Default.prevChkAwacs = "true";
            else Properties.Settings.Default.prevChkAwacs = "false";

            Properties.Settings.Default.prevTxtAwacsCallsign = txtAwacsCallsign.Text;
            Properties.Settings.Default.prevTxtAwacsFreq = txtAwacsFreq.Text;
            Properties.Settings.Default.prevTxtAwacsChannel = txtAwacsChannel.Text;
            Properties.Settings.Default.prevTxtAwacsPreset = txtAwacsPreset.Text;
            Properties.Settings.Default.prevTxtAwacsCp = txtAwacsCp.Text;

            Properties.Settings.Default.prevTxtAwacsBackupFreq = txtAwacsBackupFreq.Text;
            Properties.Settings.Default.prevTxtAwacsBackupChannel = txtAwacsBackupChannel.Text;
            Properties.Settings.Default.prevTxtAwacsBackupPreset = txtAwacsBackupPreset.Text;

            /* TACP */
            if (chkTacp.Checked == true) Properties.Settings.Default.prevChkTacp = "true";
            else Properties.Settings.Default.prevChkTacp = "false";

            Properties.Settings.Default.prevTxtTacpCallsign = txtTacpCallsign.Text;
            Properties.Settings.Default.prevTxtTacpFreq = txtTacpFreq.Text;
            Properties.Settings.Default.prevTxtTacpChannel = txtTacpChannel.Text;
            Properties.Settings.Default.prevTxtTacpPreset = txtTacpPreset.Text;
            Properties.Settings.Default.prevTxtTacpCp = txtTacpCp.Text;

            Properties.Settings.Default.prevTxtTacpBackupFreq = txtTacpBackupFreq.Text;
            Properties.Settings.Default.prevTxtTacpBackupChannel = txtTacpBackupChannel.Text;
            Properties.Settings.Default.prevTxtTacpBackupPreset = txtTacpBackupPreset.Text;

            /* INTERNAL */
            Properties.Settings.Default.prevTxtInternalFreq = txtInternalFreq.Text;
            Properties.Settings.Default.prevTxtInternalChannel = txtInternalChannel.Text;
            Properties.Settings.Default.prevTxtInternalPreset = txtInternalPreset.Text;
            Properties.Settings.Default.prevTxtInternalBackupFreq = txtInternalBackupFreq.Text;
            Properties.Settings.Default.prevTxtInternalBackupChannel = txtInternalBackupChannel.Text;
            Properties.Settings.Default.prevTxtInternalBackupPreset = txtInternalBackupPreset.Text;

            Properties.Settings.Default.prevTxtMetar = txtMetar.Text;

            /* TAXI and REJOIN */
            Properties.Settings.Default.prevCbRejoin = cbRejoin.Text;
            Properties.Settings.Default.prevCbTaxi = cbTaxi.Text;

            saveDGVSupport();
            saveDGVFlight();

            if (hasTma == true) Properties.Settings.Default.prevChkTma = "true";
            else Properties.Settings.Default.prevChkTma = "false";

            saveAltAlerts();
        }

        private void saveAltAlerts()
        {
            Properties.Settings.Default.prevTxtHard = txtHard.Text;
            Properties.Settings.Default.prevTxtMslFlr = txtMslFlr.Text;
            Properties.Settings.Default.prevTxtCeil = txtCeil.Text;
        }

        private void loadAltAlerts()
        {
            txtHard.Text = Properties.Settings.Default.prevTxtHard;
            txtMslFlr.Text = Properties.Settings.Default.prevTxtMslFlr;
            txtCeil.Text = Properties.Settings.Default.prevTxtCeil;
        }

        private void saveSupportRow(int rowNr)
        {
            if (rowNr == 0)
            {
                var row = dgvSupport.Rows[0];
                Properties.Settings.Default.supportTypeRow0 = row.Cells["colTypeSupport"].Value as string;
                Properties.Settings.Default.supportCallsignRow0 = row.Cells["colCallsignSupport"].Value as string;
                if (row.Cells["colFreqSupport"].Value == null) Properties.Settings.Default.supportFreqRow0 = "";
                else Properties.Settings.Default.supportFreqRow0 = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportChannelRow0 = row.Cells["colChannelSupport"].Value as string;
                Properties.Settings.Default.supportPresetRow0 = row.Cells["colPresetSupport"].Value as string;
                if (row.Cells["colBackupSupport"].Value == null) Properties.Settings.Default.supportBackupRow0 = "";
                else Properties.Settings.Default.supportBackupRow0 = row.Cells["colBackupSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportNotesRow0 = row.Cells["colNotesSupport"].Value as string;
            }

            if (rowNr == 1)
            {
                var row = dgvSupport.Rows[1];
                Properties.Settings.Default.supportTypeRow1 = row.Cells["colTypeSupport"].Value as string;
                Properties.Settings.Default.supportCallsignRow1 = row.Cells["colCallsignSupport"].Value as string;
                if (row.Cells["colFreqSupport"].Value == null) Properties.Settings.Default.supportFreqRow1 = "";
                else Properties.Settings.Default.supportFreqRow1 = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportChannelRow1 = row.Cells["colChannelSupport"].Value as string;
                Properties.Settings.Default.supportPresetRow1 = row.Cells["colPresetSupport"].Value as string;
                if (row.Cells["colBackupSupport"].Value == null) Properties.Settings.Default.supportBackupRow1 = "";
                else Properties.Settings.Default.supportBackupRow1 = row.Cells["colBackupSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportNotesRow1 = row.Cells["colNotesSupport"].Value as string;
            }

            if (rowNr == 2)
            {
                var row = dgvSupport.Rows[2];
                Properties.Settings.Default.supportTypeRow2 = row.Cells["colTypeSupport"].Value as string;
                Properties.Settings.Default.supportCallsignRow2 = row.Cells["colCallsignSupport"].Value as string;
                if (row.Cells["colFreqSupport"].Value == null) Properties.Settings.Default.supportFreqRow2 = "";
                else Properties.Settings.Default.supportFreqRow2 = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportChannelRow2 = row.Cells["colChannelSupport"].Value as string;
                Properties.Settings.Default.supportPresetRow2 = row.Cells["colPresetSupport"].Value as string;
                if (row.Cells["colBackupSupport"].Value == null) Properties.Settings.Default.supportBackupRow2 = "";
                else Properties.Settings.Default.supportBackupRow2 = row.Cells["colBackupSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportNotesRow2 = row.Cells["colNotesSupport"].Value as string;
            }

            if (rowNr == 3)
            {
                var row = dgvSupport.Rows[3];
                Properties.Settings.Default.supportTypeRow3 = row.Cells["colTypeSupport"].Value as string;
                Properties.Settings.Default.supportCallsignRow3 = row.Cells["colCallsignSupport"].Value as string;
                if (row.Cells["colFreqSupport"].Value == null) Properties.Settings.Default.supportFreqRow3 = "";
                else Properties.Settings.Default.supportFreqRow3 = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportChannelRow3 = row.Cells["colChannelSupport"].Value as string;
                Properties.Settings.Default.supportPresetRow3 = row.Cells["colPresetSupport"].Value as string;
                if (row.Cells["colBackupSupport"].Value == null) Properties.Settings.Default.supportBackupRow3 = "";
                else Properties.Settings.Default.supportBackupRow3 = row.Cells["colBackupSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportNotesRow3 = row.Cells["colNotesSupport"].Value as string;
            }

            if (rowNr == 4)
            {
                var row = dgvSupport.Rows[4];
                Properties.Settings.Default.supportTypeRow4 = row.Cells["colTypeSupport"].Value as string;
                Properties.Settings.Default.supportCallsignRow4 = row.Cells["colCallsignSupport"].Value as string;
                if (row.Cells["colFreqSupport"].Value == null) Properties.Settings.Default.supportFreqRow4 = "";
                else Properties.Settings.Default.supportFreqRow4 = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportChannelRow4 = row.Cells["colChannelSupport"].Value as string;
                Properties.Settings.Default.supportPresetRow4 = row.Cells["colPresetSupport"].Value as string;
                if (row.Cells["colBackupSupport"].Value == null) Properties.Settings.Default.supportBackupRow4 = "";
                else Properties.Settings.Default.supportBackupRow4 = row.Cells["colBackupSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportNotesRow4 = row.Cells["colNotesSupport"].Value as string;
            }

            if (rowNr == 5)
            {
                var row = dgvSupport.Rows[5];
                Properties.Settings.Default.supportTypeRow5 = row.Cells["colTypeSupport"].Value as string;
                Properties.Settings.Default.supportCallsignRow5 = row.Cells["colCallsignSupport"].Value as string;
                if (row.Cells["colFreqSupport"].Value == null) Properties.Settings.Default.supportFreqRow5 = "";
                else Properties.Settings.Default.supportFreqRow5 = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportChannelRow5 = row.Cells["colChannelSupport"].Value as string;
                Properties.Settings.Default.supportPresetRow5 = row.Cells["colPresetSupport"].Value as string;
                if (row.Cells["colBackupSupport"].Value == null) Properties.Settings.Default.supportBackupRow5 = "";
                else Properties.Settings.Default.supportBackupRow5 = row.Cells["colBackupSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportNotesRow5 = row.Cells["colNotesSupport"].Value as string;
            }

            if (rowNr == 6)
            {
                var row = dgvSupport.Rows[6];
                Properties.Settings.Default.supportTypeRow6 = row.Cells["colTypeSupport"].Value as string;
                Properties.Settings.Default.supportCallsignRow6 = row.Cells["colCallsignSupport"].Value as string;
                if (row.Cells["colFreqSupport"].Value == null) Properties.Settings.Default.supportFreqRow6 = "";
                else Properties.Settings.Default.supportFreqRow6 = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportChannelRow6 = row.Cells["colChannelSupport"].Value as string;
                Properties.Settings.Default.supportPresetRow6 = row.Cells["colPresetSupport"].Value as string;
                if (row.Cells["colBackupSupport"].Value == null) Properties.Settings.Default.supportBackupRow6 = "";
                else Properties.Settings.Default.supportBackupRow6 = row.Cells["colBackupSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportNotesRow6 = row.Cells["colNotesSupport"].Value as string;
            }

            if (rowNr == 7)
            {
                var row = dgvSupport.Rows[7];
                Properties.Settings.Default.supportTypeRow7 = row.Cells["colTypeSupport"].Value as string;
                Properties.Settings.Default.supportCallsignRow7 = row.Cells["colCallsignSupport"].Value as string;
                if (row.Cells["colFreqSupport"].Value == null) Properties.Settings.Default.supportFreqRow7 = "";
                else Properties.Settings.Default.supportFreqRow7 = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportChannelRow7 = row.Cells["colChannelSupport"].Value as string;
                Properties.Settings.Default.supportPresetRow7 = row.Cells["colPresetSupport"].Value as string;
                if (row.Cells["colBackupSupport"].Value == null) Properties.Settings.Default.supportBackupRow7 = "";
                else Properties.Settings.Default.supportBackupRow7 = row.Cells["colBackupSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportNotesRow7 = row.Cells["colNotesSupport"].Value as string;
            }

            if (rowNr == 8)
            {
                var row = dgvSupport.Rows[8];
                Properties.Settings.Default.supportTypeRow8 = row.Cells["colTypeSupport"].Value as string;
                Properties.Settings.Default.supportCallsignRow8 = row.Cells["colCallsignSupport"].Value as string;
                if (row.Cells["colFreqSupport"].Value == null) Properties.Settings.Default.supportFreqRow8 = "";
                else Properties.Settings.Default.supportFreqRow8 = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportChannelRow8 = row.Cells["colChannelSupport"].Value as string;
                Properties.Settings.Default.supportPresetRow8 = row.Cells["colPresetSupport"].Value as string;
                if (row.Cells["colBackupSupport"].Value == null) Properties.Settings.Default.supportBackupRow8 = "";
                else Properties.Settings.Default.supportBackupRow8 = row.Cells["colBackupSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportNotesRow8 = row.Cells["colNotesSupport"].Value as string;
            }

            if (rowNr == 9)
            {
                var row = dgvSupport.Rows[9];
                Properties.Settings.Default.supportTypeRow9 = row.Cells["colTypeSupport"].Value as string;
                Properties.Settings.Default.supportCallsignRow9 = row.Cells["colCallsignSupport"].Value as string;
                if (row.Cells["colFreqSupport"].Value == null) Properties.Settings.Default.supportFreqRow9 = "";
                else Properties.Settings.Default.supportFreqRow9 = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportChannelRow9 = row.Cells["colChannelSupport"].Value as string;
                Properties.Settings.Default.supportPresetRow9 = row.Cells["colPresetSupport"].Value as string;
                if (row.Cells["colBackupSupport"].Value == null) Properties.Settings.Default.supportBackupRow9 = "";
                else Properties.Settings.Default.supportBackupRow9 = row.Cells["colBackupSupport"].Value.ToString().TrimEnd("0".ToCharArray());
                Properties.Settings.Default.supportNotesRow9 = row.Cells["colNotesSupport"].Value as string;
            }
        }

        private void loadSupportRow(int rowNr)
        {
            if (rowNr == 0)
            {
                var row = dgvSupport.Rows[0];
                row.Cells["colTypeSupport"].Value = Properties.Settings.Default.supportTypeRow0;
                row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.supportCallsignRow0;
                row.Cells["colFreqSupport"].Value = Properties.Settings.Default.supportFreqRow0;
                row.Cells["colChannelSupport"].Value = Properties.Settings.Default.supportChannelRow0;
                row.Cells["colPresetSupport"].Value = Properties.Settings.Default.supportPresetRow0;
                row.Cells["colBackupSupport"].Value = Properties.Settings.Default.supportBackupRow0;
                row.Cells["colNotesSupport"].Value = Properties.Settings.Default.supportNotesRow0;
            }

            if (rowNr == 1)
            {
                var row = dgvSupport.Rows[1];
                row.Cells["colTypeSupport"].Value = Properties.Settings.Default.supportTypeRow1;
                row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.supportCallsignRow1;
                row.Cells["colFreqSupport"].Value = Properties.Settings.Default.supportFreqRow1;
                row.Cells["colChannelSupport"].Value = Properties.Settings.Default.supportChannelRow1;
                row.Cells["colPresetSupport"].Value = Properties.Settings.Default.supportPresetRow1;
                row.Cells["colBackupSupport"].Value = Properties.Settings.Default.supportBackupRow1;
                row.Cells["colNotesSupport"].Value = Properties.Settings.Default.supportNotesRow1;
            }

            if (rowNr == 2)
            {
                var row = dgvSupport.Rows[2];
                row.Cells["colTypeSupport"].Value = Properties.Settings.Default.supportTypeRow2;
                row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.supportCallsignRow2;
                row.Cells["colFreqSupport"].Value = Properties.Settings.Default.supportFreqRow2;
                row.Cells["colChannelSupport"].Value = Properties.Settings.Default.supportChannelRow2;
                row.Cells["colPresetSupport"].Value = Properties.Settings.Default.supportPresetRow2;
                row.Cells["colBackupSupport"].Value = Properties.Settings.Default.supportBackupRow2;
                row.Cells["colNotesSupport"].Value = Properties.Settings.Default.supportNotesRow2;
            }

            if (rowNr == 3)
            {
                var row = dgvSupport.Rows[3];
                row.Cells["colTypeSupport"].Value = Properties.Settings.Default.supportTypeRow3;
                row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.supportCallsignRow3;
                row.Cells["colFreqSupport"].Value = Properties.Settings.Default.supportFreqRow3;
                row.Cells["colChannelSupport"].Value = Properties.Settings.Default.supportChannelRow3;
                row.Cells["colPresetSupport"].Value = Properties.Settings.Default.supportPresetRow3;
                row.Cells["colBackupSupport"].Value = Properties.Settings.Default.supportBackupRow3;
                row.Cells["colNotesSupport"].Value = Properties.Settings.Default.supportNotesRow3;
            }

            if (rowNr == 4)
            {
                var row = dgvSupport.Rows[4];
                row.Cells["colTypeSupport"].Value = Properties.Settings.Default.supportTypeRow4;
                row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.supportCallsignRow4;
                row.Cells["colFreqSupport"].Value = Properties.Settings.Default.supportFreqRow4;
                row.Cells["colChannelSupport"].Value = Properties.Settings.Default.supportChannelRow4;
                row.Cells["colPresetSupport"].Value = Properties.Settings.Default.supportPresetRow4;
                row.Cells["colBackupSupport"].Value = Properties.Settings.Default.supportBackupRow4;
                row.Cells["colNotesSupport"].Value = Properties.Settings.Default.supportNotesRow4;
            }

            if (rowNr == 5)
            {
                var row = dgvSupport.Rows[5];
                row.Cells["colTypeSupport"].Value = Properties.Settings.Default.supportTypeRow5;
                row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.supportCallsignRow5;
                row.Cells["colFreqSupport"].Value = Properties.Settings.Default.supportFreqRow5;
                row.Cells["colChannelSupport"].Value = Properties.Settings.Default.supportChannelRow5;
                row.Cells["colPresetSupport"].Value = Properties.Settings.Default.supportPresetRow5;
                row.Cells["colBackupSupport"].Value = Properties.Settings.Default.supportBackupRow5;
                row.Cells["colNotesSupport"].Value = Properties.Settings.Default.supportNotesRow5;
            }

            if (rowNr == 6)
            {
                var row = dgvSupport.Rows[6];
                row.Cells["colTypeSupport"].Value = Properties.Settings.Default.supportTypeRow6;
                row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.supportCallsignRow6;
                row.Cells["colFreqSupport"].Value = Properties.Settings.Default.supportFreqRow6;
                row.Cells["colChannelSupport"].Value = Properties.Settings.Default.supportChannelRow6;
                row.Cells["colPresetSupport"].Value = Properties.Settings.Default.supportPresetRow6;
                row.Cells["colBackupSupport"].Value = Properties.Settings.Default.supportBackupRow6;
                row.Cells["colNotesSupport"].Value = Properties.Settings.Default.supportNotesRow6;
            }

            if (rowNr == 7)
            {
                var row = dgvSupport.Rows[7];
                row.Cells["colTypeSupport"].Value = Properties.Settings.Default.supportTypeRow7;
                row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.supportCallsignRow7;
                row.Cells["colFreqSupport"].Value = Properties.Settings.Default.supportFreqRow7;
                row.Cells["colChannelSupport"].Value = Properties.Settings.Default.supportChannelRow7;
                row.Cells["colPresetSupport"].Value = Properties.Settings.Default.supportPresetRow7;
                row.Cells["colBackupSupport"].Value = Properties.Settings.Default.supportBackupRow7;
                row.Cells["colNotesSupport"].Value = Properties.Settings.Default.supportNotesRow7;
            }

            if (rowNr == 8)
            {
                var row = dgvSupport.Rows[8];
                row.Cells["colTypeSupport"].Value = Properties.Settings.Default.supportTypeRow8;
                row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.supportCallsignRow8;
                row.Cells["colFreqSupport"].Value = Properties.Settings.Default.supportFreqRow8;
                row.Cells["colChannelSupport"].Value = Properties.Settings.Default.supportChannelRow8;
                row.Cells["colPresetSupport"].Value = Properties.Settings.Default.supportPresetRow8;
                row.Cells["colBackupSupport"].Value = Properties.Settings.Default.supportBackupRow8;
                row.Cells["colNotesSupport"].Value = Properties.Settings.Default.supportNotesRow8;
            }

            if (rowNr == 9)
            {
                var row = dgvSupport.Rows[9];
                row.Cells["colTypeSupport"].Value = Properties.Settings.Default.supportTypeRow9;
                row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.supportCallsignRow9;
                row.Cells["colFreqSupport"].Value = Properties.Settings.Default.supportFreqRow9;
                row.Cells["colChannelSupport"].Value = Properties.Settings.Default.supportChannelRow9;
                row.Cells["colPresetSupport"].Value = Properties.Settings.Default.supportPresetRow9;
                row.Cells["colBackupSupport"].Value = Properties.Settings.Default.supportBackupRow9;
                row.Cells["colNotesSupport"].Value = Properties.Settings.Default.supportNotesRow9;
            }
        }

        private void saveDGVAirbases()
        {
            /* Departure */
            var row = dgvAirbase.Rows[0];
            
            Properties.Settings.Default.prevColAirbaseDep = row.Cells["colAirbase"].Value as string;
            Properties.Settings.Default.prevColTcnDep = row.Cells["colTcn"].Value as string;
            Properties.Settings.Default.prevColGndFreqDep = row.Cells["colGnd"].Value as string;
            Properties.Settings.Default.prevColTwrFreqDep = row.Cells["colTwr"].Value as string;
            Properties.Settings.Default.prevColTmaFreqDep = row.Cells["colTma"].Value as string;
            Properties.Settings.Default.prevColElevDep = row.Cells["colElev"].Value as string;
            Properties.Settings.Default.prevColRwyDep = row.Cells["colRwy"].Value as string;
            Properties.Settings.Default.prevColIlsDep = row.Cells["colIls"].Value as string;
            
            /* Arrival */
            row = dgvAirbase.Rows[1];

            Properties.Settings.Default.prevColAirbaseArr = row.Cells["colAirbase"].Value as string;
            Properties.Settings.Default.prevColTcnArr = row.Cells["colTcn"].Value as string;
            Properties.Settings.Default.prevColGndFreqArr = row.Cells["colGnd"].Value as string;
            Properties.Settings.Default.prevColTwrFreqArr = row.Cells["colTwr"].Value as string;
            Properties.Settings.Default.prevColTmaFreqArr = row.Cells["colTma"].Value as string;
            Properties.Settings.Default.prevColElevArr = row.Cells["colElev"].Value as string;
            Properties.Settings.Default.prevColRwyArr = row.Cells["colRwy"].Value as string;
            Properties.Settings.Default.prevColIlsArr = row.Cells["colIls"].Value as string;

            /* Alternate */
            row = dgvAirbase.Rows[2];

            Properties.Settings.Default.prevColAirbaseAlt = row.Cells["colAirbase"].Value as string;
            Properties.Settings.Default.prevColTcnAlt = row.Cells["colTcn"].Value as string;
            Properties.Settings.Default.prevColGndFreqAlt = row.Cells["colGnd"].Value as string;
            Properties.Settings.Default.prevColTwrFreqAlt = row.Cells["colTwr"].Value as string;
            Properties.Settings.Default.prevColTmaFreqAlt = row.Cells["colTma"].Value as string;
            Properties.Settings.Default.prevColElevAlt = row.Cells["colElev"].Value as string;
            Properties.Settings.Default.prevColRwyAlt = row.Cells["colRwy"].Value as string;
            Properties.Settings.Default.prevColIlsAlt = row.Cells["colIls"].Value as string;

            /* Backup */
            row = dgvAirbase.Rows[3];

            Properties.Settings.Default.prevColAirbaseBck = row.Cells["colAirbase"].Value as string;
            Properties.Settings.Default.prevColTcnBck = row.Cells["colTcn"].Value as string;
            Properties.Settings.Default.prevColGndFreqBck = row.Cells["colGnd"].Value as string;
            Properties.Settings.Default.prevColTwrFreqBck = row.Cells["colTwr"].Value as string;
            Properties.Settings.Default.prevColTmaFreqBck = row.Cells["colTma"].Value as string;
            Properties.Settings.Default.prevColElevBck = row.Cells["colElev"].Value as string;
            Properties.Settings.Default.prevColRwyBck = row.Cells["colRwy"].Value as string;
            Properties.Settings.Default.prevColIlsBck = row.Cells["colIls"].Value as string;
        }

        private void loadDGVSupport()
        {
            for (int i = 0; i < dgvSupport.Rows.Count; i++)
            {
                loadSupportRow(i);
            }
        }

        private void loadDGVFlight()
        {
           /* Lead */
           var row = dgvFlight.Rows[0];

            row.Cells["colPos"].Value = Properties.Settings.Default.prevColPosLead;
            row.Cells["colCallsign"].Value = Properties.Settings.Default.prevColCallsignLead;
            row.Cells["colPilot"].Value = Properties.Settings.Default.prevColPilotLead;
            row.Cells["colGidOid"].Value = Properties.Settings.Default.prevColGidOidLead;
            row.Cells["colYardstick"].Value = Properties.Settings.Default.prevColYardstickLead;
            row.Cells["colLsr"].Value = Properties.Settings.Default.prevColLsrLead;
            row.Cells["colNotes"].Value = Properties.Settings.Default.prevColNotesLead;

            /* Wing */
            row = dgvFlight.Rows[1];

            row.Cells["colPos"].Value = Properties.Settings.Default.prevColPosWing;
            row.Cells["colCallsign"].Value = Properties.Settings.Default.prevColCallsignWing;
            row.Cells["colPilot"].Value = Properties.Settings.Default.prevColPilotWing;
            row.Cells["colGidOid"].Value = Properties.Settings.Default.prevColGidOidWing;
            row.Cells["colYardstick"].Value = Properties.Settings.Default.prevColYardstickWing;
            row.Cells["colLsr"].Value = Properties.Settings.Default.prevColLsrWing;
            row.Cells["colNotes"].Value = Properties.Settings.Default.prevColNotesWing;

            /* Element */
            row = dgvFlight.Rows[2];

            row.Cells["colPos"].Value = Properties.Settings.Default.prevColPosElement;
            row.Cells["colCallsign"].Value = Properties.Settings.Default.prevColCallsignElement;
            row.Cells["colPilot"].Value = Properties.Settings.Default.prevColPilotElement;
            row.Cells["colGidOid"].Value = Properties.Settings.Default.prevColGidOidElement;
            row.Cells["colYardstick"].Value = Properties.Settings.Default.prevColYardstickElement;
            row.Cells["colLsr"].Value = Properties.Settings.Default.prevColLsrElement;
            row.Cells["colNotes"].Value = Properties.Settings.Default.prevColNotesElement;

            /* Trail */
            row = dgvFlight.Rows[3];

            row.Cells["colPos"].Value = Properties.Settings.Default.prevColPosTrail;
            row.Cells["colCallsign"].Value = Properties.Settings.Default.prevColCallsignTrail;
            row.Cells["colPilot"].Value = Properties.Settings.Default.prevColPilotTrail;
            row.Cells["colGidOid"].Value = Properties.Settings.Default.prevColGidOidTrail;
            row.Cells["colYardstick"].Value = Properties.Settings.Default.prevColYardstickTrail;
            row.Cells["colLsr"].Value = Properties.Settings.Default.prevColLsrTrail;
            row.Cells["colNotes"].Value = Properties.Settings.Default.prevColNotesTrail;
        }

        private void loadDGVAirbases()
        {
            /* Departure */
            var row = dgvAirbase.Rows[0];

            row.Cells["colAirbase"].Value = Properties.Settings.Default.prevColAirbaseDep;
            row.Cells["colTcn"].Value = Properties.Settings.Default.prevColTcnDep;
            row.Cells["colGnd"].Value = Properties.Settings.Default.prevColGndFreqDep;
            row.Cells["colTwr"].Value = Properties.Settings.Default.prevColTwrFreqDep;
            row.Cells["colTma"].Value = Properties.Settings.Default.prevColTmaFreqDep;
            row.Cells["colElev"].Value = Properties.Settings.Default.prevColElevDep;
            row.Cells["colRwy"].Value = Properties.Settings.Default.prevColRwyDep;
            row.Cells["colIls"].Value = Properties.Settings.Default.prevColIlsDep;

            /* Arrival */
            row = dgvAirbase.Rows[1];

            row.Cells["colAirbase"].Value = Properties.Settings.Default.prevColAirbaseArr;
            row.Cells["colTcn"].Value = Properties.Settings.Default.prevColTcnArr;
            row.Cells["colGnd"].Value = Properties.Settings.Default.prevColGndFreqArr;
            row.Cells["colTwr"].Value = Properties.Settings.Default.prevColTwrFreqArr;
            row.Cells["colTma"].Value = Properties.Settings.Default.prevColTmaFreqArr;
            row.Cells["colElev"].Value = Properties.Settings.Default.prevColElevArr;
            row.Cells["colRwy"].Value = Properties.Settings.Default.prevColRwyArr;
            row.Cells["colIls"].Value = Properties.Settings.Default.prevColIlsArr;

            /* Alternate */
            row = dgvAirbase.Rows[2];

            row.Cells["colAirbase"].Value = Properties.Settings.Default.prevColAirbaseAlt;
            row.Cells["colTcn"].Value = Properties.Settings.Default.prevColTcnAlt;
            row.Cells["colGnd"].Value = Properties.Settings.Default.prevColGndFreqAlt;
            row.Cells["colTwr"].Value = Properties.Settings.Default.prevColTwrFreqAlt;
            row.Cells["colTma"].Value = Properties.Settings.Default.prevColTmaFreqAlt;
            row.Cells["colElev"].Value = Properties.Settings.Default.prevColElevAlt;
            row.Cells["colRwy"].Value = Properties.Settings.Default.prevColRwyAlt;
            row.Cells["colIls"].Value = Properties.Settings.Default.prevColIlsAlt;

            /* Backup */
            row = dgvAirbase.Rows[3];

            row.Cells["colAirbase"].Value = Properties.Settings.Default.prevColAirbaseBck;
            row.Cells["colTcn"].Value = Properties.Settings.Default.prevColTcnBck;
            row.Cells["colGnd"].Value = Properties.Settings.Default.prevColGndFreqBck;
            row.Cells["colTwr"].Value = Properties.Settings.Default.prevColTwrFreqBck;
            row.Cells["colTma"].Value = Properties.Settings.Default.prevColTmaFreqBck;
            row.Cells["colElev"].Value = Properties.Settings.Default.prevColElevBck;
            row.Cells["colRwy"].Value = Properties.Settings.Default.prevColRwyBck;
            row.Cells["colIls"].Value = Properties.Settings.Default.prevColIlsBck;
        }

        private void saveDGVFlight()
        {
            /* Lead */
            var row = dgvFlight.Rows[0];

            Properties.Settings.Default.prevColPosLead = row.Cells["colPos"].Value as string;
            Properties.Settings.Default.prevColCallsignLead = row.Cells["colCallsign"].Value as string;
            Properties.Settings.Default.prevColPilotLead = row.Cells["colPilot"].Value as string;
            Properties.Settings.Default.prevColGidOidLead = row.Cells["colGidOid"].Value as string;
            Properties.Settings.Default.prevColYardstickLead = row.Cells["colYardstick"].Value as string;
            if (row.Cells["colLsr"].Value != null)
            {
                Properties.Settings.Default.prevColLsrLead = row.Cells["colLsr"].Value.ToString();
            }
            Properties.Settings.Default.prevColNotesLead = row.Cells["colNotes"].Value as string;

            /* Wing */
            row = dgvFlight.Rows[1];

            Properties.Settings.Default.prevColPosWing = row.Cells["colPos"].Value as string;
            Properties.Settings.Default.prevColCallsignWing = row.Cells["colCallsign"].Value as string;
            Properties.Settings.Default.prevColPilotWing = row.Cells["colPilot"].Value as string;
            Properties.Settings.Default.prevColGidOidWing = row.Cells["colGidOid"].Value as string;
            Properties.Settings.Default.prevColYardstickWing = row.Cells["colYardstick"].Value as string;
            if (row.Cells["colLsr"].Value != null)
            {
                Properties.Settings.Default.prevColLsrWing = row.Cells["colLsr"].Value.ToString();
            }
            Properties.Settings.Default.prevColNotesWing = row.Cells["colNotes"].Value as string;

            /* Element */
            row = dgvFlight.Rows[2];

            Properties.Settings.Default.prevColPosElement = row.Cells["colPos"].Value as string;
            Properties.Settings.Default.prevColCallsignElement = row.Cells["colCallsign"].Value as string;
            Properties.Settings.Default.prevColPilotElement = row.Cells["colPilot"].Value as string;
            Properties.Settings.Default.prevColGidOidElement = row.Cells["colGidOid"].Value as string;
            Properties.Settings.Default.prevColYardstickElement = row.Cells["colYardstick"].Value as string;
            if (row.Cells["colLsr"].Value != null)
            {
                Properties.Settings.Default.prevColLsrElement = row.Cells["colLsr"].Value.ToString();
            }
            Properties.Settings.Default.prevColNotesElement = row.Cells["colNotes"].Value as string;

            /* Wing 2: trail */
            row = dgvFlight.Rows[3];

            Properties.Settings.Default.prevColPosTrail = row.Cells["colPos"].Value as string;
            Properties.Settings.Default.prevColCallsignTrail = row.Cells["colCallsign"].Value as string;
            Properties.Settings.Default.prevColPilotTrail = row.Cells["colPilot"].Value as string;
            Properties.Settings.Default.prevColGidOidTrail = row.Cells["colGidOid"].Value as string;
            Properties.Settings.Default.prevColYardstickTrail = row.Cells["colYardstick"].Value as string;
            if (row.Cells["colLsr"].Value != null)
            {
                Properties.Settings.Default.prevColLsrTrail = row.Cells["colLsr"].Value.ToString() as string;
            }
            Properties.Settings.Default.prevColNotesTrail = row.Cells["colNotes"].Value as string;
        }

        private void saveDGVSupport()
        {
            for (int i = 0; i < dgvSupport.Rows.Count; i++)
            {
                saveSupportRow(i);
            }
        }

        /* Select kneeboardbuilder file and save path */
        public static void setKneeboardPath()
        {
            string fileDialogText = "Select the kneeboardbuilder application file";
            string filter = ".exe|*.exe";

            if (Properties.Settings.Default.pathKneeboardBuilder.Equals("") && Properties.Settings.Default.pathKneeboardBuilder != null)
            {
                Properties.Settings.Default.pathKneeboardBuilder = getPath(fileDialogText, filter);
                //Properties.Settings.Default.Save(); // RECENT CHANGE! ERROR WARNING DEBUG GO BACK HERE TO DOUBLE CHECK
            }
            else
            {
                Console.WriteLine("SYSTEM: Properties.Settings.Default.pathKneeboardBuilder is loaded from previous session: " + Properties.Settings.Default.pathKneeboardBuilder);
            }
        }


        // Todo: Throw exception when path is not retrieved
        public static string getPath(string fileDialogText, string filter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = fileDialogText;
            ofd.Filter = filter;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return System.IO.Path.GetDirectoryName(ofd.FileName);
            }
            return "";
        }

        private static string ConvertNumber(string number)
        {
            if (number == "1")
            {
                number = "one";
            } else if (number == "2")
            {
                number = "two";
            } else if (number == "3")
            {
                number = "three";
            } else if (number == "4")
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
                tma = "Tbilisi";
            } else
            {
                tma = "N/A";
            }
            return tma;
        }

        private static string ConvertAirfield(string identifier)
        {
            // Default return value
            string airfield = "N/A";

            /* Georgian airports */
            if ((identifier == "DEPLOC:UG5X") || (identifier == "ARRLOC:UG5X")) {
                airfield = "Kobuleti";
            } else if ((identifier == "DEPLOC:UG23") || (identifier == "ARRLOC:UG23")) {
                airfield = "Gudauta";
            } else if ((identifier == "DEPLOC:UG24") || (identifier == "ARRLOC:UG24")) {
                airfield = "Soganlug";
            } else if ((identifier == "DEPLOC:UG27") || (identifier == "ARRLOC:UG27")) {
                airfield = "Vaziani";
            } else if ((identifier == "DEPLOC:UGKO") || (identifier == "ARRLOC:UGKO")) {
                airfield = "Kutaisi";
            } else if ((identifier == "DEPLOC:UGKS") || (identifier == "ARRLOC:UGKS")) {
                airfield = "Senaki"; // Consider Senaki - Kolkhi as stated in the Aerodome charts
            } else if ((identifier == "DEPLOC:UGSB") || (identifier == "ARRLOC:UGSB")) {
                airfield = "Batumi";
            } else if ((identifier == "DEPLOC:UGSS") || (identifier == "ARRLOC:UGSS")) {
                airfield = "Sukhumi"; // Consider Sukhumi - Babushara as stated in the Aerodome charts

            /* Russian airports */
            } else if ((identifier == "DEPLOC:URKA") || (identifier == "ARRLOC:URKA")) {
                airfield = "Anapa - Vityazevo";
            } else if ((identifier == "DEPLOC:URKG") || (identifier == "ARRLOC:URKG")) {
                airfield = "Gelendzhik";
            } else if ((identifier == "DEPLOC:URKH") || (identifier == "ARRLOC:URKH")) {
                airfield = "Maykop - Khanskaya";
            } else if ((identifier == "DEPLOC:URKK") || (identifier == "ARRLOC:URKK")) {
                airfield = "Krasnodar - Pashkovsky";
            } else if ((identifier == "DEPLOC:URKL") || (identifier == "ARRLOC:URKL")) {
                airfield = "Krasnodar - Center";
            } else if ((identifier == "DEPLOC:URKN") || (identifier == "ARRLOC:URKN")) {
                airfield = "Novorossiysk";
            } else if ((identifier == "DEPLOC:URKW") || (identifier == "ARRLOC:URKW")) {
                airfield = "Krymsk";
            } else if ((identifier == "DEPLOC:URMM") || (identifier == "ARRLOC:URMM")) {
                airfield = "Mineralnye Vody";
            } else if ((identifier == "DEPLOC:URMN") || (identifier == "ARRLOC:URMN")) {
                airfield = "Nalchik";
            } else if ((identifier == "DEPLOC:URMO") || (identifier == "ARRLOC:URMO")) {
                airfield = "Beslan";
            } else if ((identifier == "DEPLOC:URSS") || (identifier == "ARRLOC:URSS")) {
                airfield = "Sochi - Adler";
            } else if ((identifier == "DEPLOC:XRMF") || (identifier == "ARRLOC:XRMF")) {
                airfield = "Mozdok";
            }

            return airfield;
        }

        private static string GetLettersOnly(string word)
        {
            return new String(word.Where(Char.IsLetter).ToArray());
        }

        public string GetFirstDigit(string word)
        {
            return new String(word.Where(Char.IsDigit).ToArray())[0].ToString();
        }

        private string getGroupId()
        {
            string gid = "";
            if (txtCallsign.Text != null && txtCallsign.Text != "")
            {
                if (Char.IsDigit(GetFirstDigit(txtCallsign.Text).ToCharArray()[0]))
                {
                    gid += "3" + GetFirstDigit(txtCallsign.Text).ToCharArray()[0];
                }
            }
            
            return gid;
        }

        private string getTwoDigitsFromCallsign()
        {
            string digits = "";
            if (txtCallsign.Text != null && txtCallsign.Text != "")
            {
                if (Char.IsDigit(GetFirstDigit(txtCallsign.Text).ToCharArray()[0]) && Char.IsDigit(txtCallsign.Text[txtCallsign.Text.Length - 1]))
                {
                    digits += GetFirstDigit(txtCallsign.Text); // Add first digit in callsign
                    digits += txtCallsign.Text[txtCallsign.Text.Length - 1]; // Add last digit in callsign
                }
            }
            return digits;
        }

        public static string ExtractTextFromPdf(string path)
        {
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
                return text.ToString();
            }
        }

        /* Removes lines that does not start with a digit from the preset file */
        public static string CleanPresets(string raw)
        {
            string clean = "";
            string line = null;
            StringReader reader = new StringReader(raw);
            while (true) {
                line = reader.ReadLine();
                if (line != null) {
                    if (Char.IsDigit(line.FirstOrDefault())) {
                        clean = clean + line + ' ';
                        // clean += Environment.NewLine;
                        // consider removing last '/n'
                    }
                } else {
                    break;
                }
            }
            return clean;
        }

        public bool isNumber(string word)
        {
            if (word.Length == 1 && Char.IsDigit(word[0]))
            {
                return true;
            }

            if (word.Length == 2 && (Char.IsDigit(word[0]) && Char.IsDigit(word[1]))) {
                return true;
            }
            return false;
        }

        bool isColor(string word)
        {
            word = word.ToUpper();
            if (word.Equals("BLUE")
                || word.Equals("GREEN")
                || word.Equals("RED")
                || word.Equals("YELLOW")
                || word.Equals("ORANGE")
                || word.Equals("PURPLE")
                || word.Equals("WHITE")
                || word.Equals("GRAY")
                || word.Equals("PINK")
                || word.Equals("BROWN")
                || word.Equals("VIOLET")
                || word.Equals("AMBER")
                || word.Equals("AQUA")
                || word.Equals("CHERRY")
                || word.Equals("GOLD")
                || word.Equals("CORAL")
                || word.Equals("INDIGO")
                || word.Equals("LEMON")
                || word.Equals("LIME")
                || word.Equals("MAROON")
                || word.Equals("OCHRE")
                || word.Equals("OLIVE")
                || word.Equals("BLACK")
                )
            {
                return true;
            // if color also has a number following it
            } else if((word.Contains("BLUE")
                || word.Contains("GREEN")
                || word.Contains("RED")
                || word.Contains("YELLOW")
                || word.Contains("ORANGE")
                || word.Contains("PURPLE")
                || word.Contains("WHITE")
                || word.Contains("GRAY")
                || word.Contains("PINK")
                || word.Contains("BROWN")
                || word.Contains("VIOLET")
                || word.Contains("AMBER")
                || word.Contains("AQUA")
                || word.Contains("CHERRY")
                || word.Contains("GOLD")
                || word.Contains("CORAL")
                || word.Contains("INDIGO")
                || word.Contains("LEMON")
                || word.Contains("LIME")
                || word.Contains("MAROON")
                || word.Contains("OCHRE")
                || word.Contains("OLIVE")
                || word.Contains("BLACK")
                ) && (char.IsDigit(word[word.Length-1]) || (char.IsDigit(word[word.Length-2]) && char.IsDigit(word[word.Length-1]))))
            {
                return true;
            } else return false;
        }

        bool isFreq(string word)
        {
            // ###.###
            if (word.Length == 7 && Char.IsDigit(word[0]) && Char.IsDigit(word[1]) && Char.IsDigit(word[2]) && word[3] == '.' && Char.IsDigit(word[4]) && Char.IsDigit(word[5]) && Char.IsDigit(word[6]))
            {
                return true;
            }

            // ##.##
            if (word.Length == 5 && Char.IsDigit(word[0]) && Char.IsDigit(word[1]) && word[2] == '.' && Char.IsDigit(word[3]) && Char.IsDigit(word[4]))
            {
                return true;
            }

            // ##.###
            if (word.Length == 6 && Char.IsDigit(word[0]) && Char.IsDigit(word[1]) && word[2] == '.' && Char.IsDigit(word[3]) && Char.IsDigit(word[4]) && Char.IsDigit(word[5]))
            {
                return true;
            }

            // ### important for 132nd operational frequencies table to work. Do not remove!
            if (word.Length == 6 && Char.IsDigit(word[0]) && Char.IsDigit(word[1]) && Char.IsDigit(word[2]) && word[3] == '.' && Char.IsDigit(word[4]) && Char.IsDigit(word[5]))
            {
                return true;
            }

            // ### important for 132nd operational frequencies table to work. Do not remove!
            if (word.Length == 3 && Char.IsDigit(word[0]) && Char.IsDigit(word[1]) && Char.IsDigit(word[2]))
            {
                return true;
            }

            return false;
        }

        void storeAsPdf(string path)
        {
            string wordDoc = "temp.docm";
            string pdfDoc = "temp.pdf";

            if (!path.EndsWith(@"\"))
            {
                path += @"\";
            }

            /*Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
            Document wordDocument = appWord.Documents.Open(path + wordDoc);
            wordDocument.ExportAsFixedFormat(path + pdfDoc, WdExportFormat.wdExportFormatPDF);

            wordDocument.Close();*/
        }

        void splitPdf(string path)
        {
            if (!path.EndsWith(@"\")) path += @"\";
            
            string filename = "cpdf.exe";
            string arguments = "-split temp.pdf -o page%%.pdf";

            runCommand(filename, arguments);

            // Must wait for pages to be generated
            while (!File.Exists(path + "page01.pdf")
                || !File.Exists(path + "page02.pdf")
                || !File.Exists(path + "page03.pdf")
                || !File.Exists(path + "page04.pdf")
                || !File.Exists(path + "page05.pdf")
                || !File.Exists(path + "page06.pdf")
                || !File.Exists(path + "page07.pdf")
                || !File.Exists(path + "page08.pdf")
                || !File.Exists(path + "page09.pdf")
                || !File.Exists(path + "page10.pdf"))
            {
                System.Threading.Thread.Sleep(50);
            }

            path = Environment.CurrentDirectory + @"\";

            save(path + "page01.pdf", path + "com1-sta.pdf");
            save(path + "page02.pdf", path + "com2-to.pdf");
            save(path + "page03.pdf", path + "com3-dep.pdf");
            save(path + "page04.pdf", path + "com4-tma.pdf");
            save(path + "page05.pdf", path + "com5-aci.pdf");
            save(path + "page06.pdf", path + "com6-cas.pdf");
            save(path + "page07.pdf", path + "com7-aco.pdf");
            save(path + "page08.pdf", path + "com8-arr.pdf");
            save(path + "page09.pdf", path + "com9-lan.pdf");
            save(path + "page10.pdf", path + "com10-tax.pdf");

            delete(path + "temp.pdf");
        }

        void toKneeboardBuilder(string path)
        {
            if (standardTrainingSet)
            {
                // save in /TR/
            } else
            {
                // save in /MSN/
            }

            saveAsPng("com1-sta", path);
            saveAsPng("com2-to", path);
            saveAsPng("com3-dep", path);
            saveAsPng("com4-tma", path);
            saveAsPng("com5-aci", path);
            saveAsPng("com6-cas", path);
            saveAsPng("com7-aco", path);
            saveAsPng("com8-arr", path);
            saveAsPng("com9-lan", path);
            saveAsPng("com10-tax", path);
        }

        private void captureScreen(string path)
        {
            System.Drawing.Rectangle bounds = this.Bounds;
            //using (Bitmap bitmap = new Bitmap(bounds.Width - 20, bounds.Height - 40))
            using (Bitmap bitmap = new Bitmap(bounds.Width-6, bounds.Height-30))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {   // Left + is to the right, - is to the left
                    // Top + is down, - is up
                    // g.CopyFromScreen(new System.Drawing.Point(bounds.Left+10, bounds.Top+30), System.Drawing.Point.Empty, bounds.Size);
                    g.CopyFromScreen(new System.Drawing.Point(bounds.Left+3, bounds.Top+30), System.Drawing.Point.Empty, bounds.Size);
                }
                /* Save it to kneeboard */
                if (MyGlobals.global_training == true)
                {
                    Directory.CreateDirectory(path + @"\TR");
                    bitmap.Save(path + @"\TR\TR-004.png");
                } else
                {
                    Directory.CreateDirectory(path + @"\MSN");
                    bitmap.Save(path + @"\MSN\MSN-004.png");
                }
                
            }
        }

        void saveAsPng(string pdfFileName, string path)
        {
            System.Threading.Thread.Sleep(50);
            while (!File.Exists(Environment.CurrentDirectory + "\\" + pdfFileName + ".pdf"))
            {
                System.Threading.Thread.Sleep(50);
                Console.WriteLine("Sleeping: cannot find file " + Environment.CurrentDirectory + "\\" + pdfFileName + ".pdf");
            }

            // for each pdf-file:
            if (File.Exists(Environment.CurrentDirectory + "\\" + pdfFileName + ".pdf"))
            {
                if (!path.EndsWith("\\"))
                {
                    path += "\\";
                }

                string pdfPath = Environment.CurrentDirectory + "\\" + pdfFileName + ".pdf";
                string dir = pdfFileName.ToUpper().Replace('-', ' ');
                string outFileName = "";

                if (standardTrainingSet)
                {
                    dir = "TR\\";
                    outFileName = "TR-";
                } else
                {
                    dir = "MSN\\";
                    outFileName = "MSN-";
                }

                outFileName += setOutFile(pdfFileName, path + dir);
                outFileName += ".png";

                // TODO: Check that directory exists, and if not, create it before saving

                string output = path + dir + outFileName;

                // Generate GhostscriptSettings
                GhostscriptSharp.GhostscriptSettings settingsForConvert = new GhostscriptSharp.GhostscriptSettings(); // set device

                // GhostscriptSettings.GhostscriptDevices;
                // settingsForConvert.Device = new GhostscriptSharp.Settings.GhostscriptDevices();
                settingsForConvert.Device = GhostscriptSharp.Settings.GhostscriptDevices.png256; // 3 sets output format as png256

                // GhostscriptSettings.Size
                GhostscriptSharp.Settings.GhostscriptPageSize pageSize = new GhostscriptSharp.Settings.GhostscriptPageSize();
                pageSize.Native = new GhostscriptSharp.Settings.GhostscriptPageSizes();
                pageSize.Native = GhostscriptSharp.Settings.GhostscriptPageSizes.a4;
                settingsForConvert.Size = pageSize;

                settingsForConvert.Resolution = new System.Drawing.Size(150, 150); // sets 150 PPI/DPI

                GhostscriptSharp.GhostscriptWrapper.GenerateOutput(pdfPath, output, settingsForConvert);
            } else
            {
                Console.WriteLine("File does not exist");
            }

        }

        string setOutFile(string pdfFileName, string path)
        {
            // count nr of documents in folder
            DirectoryInfo myDir = new DirectoryInfo(path);
            int count = myDir.GetFiles().Length;

            string outFileName = "";
            if (pdfFileName.Contains("com1-sta")) outFileName += "007";
            else if (pdfFileName.Contains("com2-to")) outFileName += "009";
            else if (pdfFileName.Contains("com3-dep")) outFileName += "011";
            else if (pdfFileName.Contains("com4-tma")) outFileName += "014";
            else if (pdfFileName.Contains("com5-aci")) outFileName += "015";
            else if (pdfFileName.Contains("com6-cas")) outFileName += "018";
            else if (pdfFileName.Contains("com7-aco")) outFileName += "0" + (count - 7).ToString();
            else if (pdfFileName.Contains("com8-arr")) outFileName += "0" + (count - 5).ToString();
            else if (pdfFileName.Contains("com9-lan")) outFileName += "0" + (count - 4).ToString();
            else if (pdfFileName.Contains("com10-tax")) outFileName += "0" + (count - 2).ToString();
            return outFileName;
        }

        void runCommand(string filename, string arguments)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();

            // hiding the commandline window
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            startInfo.FileName = filename;
            startInfo.Arguments = arguments;

            process.StartInfo = startInfo;
            process.Start();
        }

        void save(string replaceFp, string replacementFp)
        {
            if (replaceFp != null && replacementFp != null)
            {
                if (File.Exists(replaceFp))
                {
                    if (File.Exists(replacementFp)) {
                        try {
                            File.Delete(replacementFp);
                        }
                        catch (System.IO.IOException)
                        {
                            System.Threading.Thread.Sleep(50);
                            save(replaceFp, replacementFp);
                            return;
                        }
                    }
                    File.Move(replaceFp, replacementFp);
                }
            }
        }

        void delete(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            while (isFileLocked(info))
            {
                System.Threading.Thread.Sleep(50);
            }
            File.Delete(filePath);
        }

        protected virtual bool isFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return false;
        }

        private void cmbNrOfAc_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            string callsign = txtCallsign.Text;
            string old_callsign = oldCallsign;

            // changed (added the if clause) 27.06.2017 DEBUG: Could be that the if clause should be put around the whole content of this method
            if (txtCallsign.Text != oldCallsign && !fromBack)
            {
                setFlightTable(cb.Text);
            }

            if (cb.Text.Equals("1")) {
                lblTaxi.Enabled = false;
                cbTaxi.Enabled = false;
                lblSpacing.Enabled = false;
                txtSpacing.Enabled = false;
                lblRejoin.Enabled = false;
                cbRejoin.Enabled = false;
            } else
            {
                lblTaxi.Enabled = true;
                cbTaxi.Enabled = true;
                lblSpacing.Enabled = true;
                txtSpacing.Enabled = true;
                lblRejoin.Enabled = true;
                cbRejoin.Enabled = true;
            }
        }

        private void setFlightTable(string nr)
        {
            //string firstDigit = getTwoDigitsFromCallsign().Substring(0, 1);
            string flightNr = GetFirstDigit(txtCallsign.Text);
            string gid = getGroupId();
            string pos;
            int nrOfAc = Int32.Parse(nr);
            int i = 0;

            // Position 1:
            var row = dgvFlight.Rows[i];
            row.Cells["colPos"].Value = "Lead";
            row.Cells["colYardstick"].Value = flightNr + "Y";

            // Position 2, 3 and 4
            while (i < nrOfAc)
            {
                pos = (i + 1).ToString();
                row = dgvFlight.Rows[i];

                if (i == 2)
                {
                    row.Cells["colPos"].Value = "Element";
                } else 
                {
                    if (i != 0) row.Cells["colPos"].Value = "Wing";
                }

                row.Cells["colCallsign"].Value = txtCallsign.Text.Substring(0, txtCallsign.Text.Length - 1) + pos;
                string oid = flightNr + pos;
                row.Cells["colGidOid"].Value = gid + "/" + oid;

                if (i != 0) row.Cells["colYardstick"].Value = (Int32.Parse(flightNr) + 63).ToString() + "Y";

                row.Cells["colLsr"].Value = "15" + flightNr + pos;
                i++;
            }

            // Clear the remaining rows
            while (i < 4)
            {
                dgvFlight.Rows[i].SetValues("", "", "", "", "", "", "");
                i++;
            }
        }

        private void txtLocation_Enter(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int visibleTime = 10000;

            ToolTip tt = new ToolTip();
            tt.Show("Enter killbox or range", tb, visibleTime);
        }

        private void txtCallsign_Leave(object sender, EventArgs e)
        {
            // <CALLSIGN> <FLIGHTNR>-<POS>
            // JEDI 4-1

            txtCallsign.Text = txtCallsign.Text.ToUpper();

            string callsign_without_nr = getCharactersFromCallsign();

            var row = dgvFlight.Rows[0];
            string callsign_from_dgv = getCharactersFromCallsign(row.Cells[1].Value.ToString());
            string callsign_from_txt = getCharactersFromCallsign(txtCallsign.Text);
            
            // strip bort så bare callsign_without_nr står igjen
            if (callsign_from_txt != callsign_from_dgv)
            {
                string flightNr = GetFirstDigit(txtCallsign.Text);
                //string pos = getPosFromCallsign();

                updateFlight(callsign_without_nr, flightNr);
            }
        }

        private void updateFlight(string callsign, string flightNr)
        {
            int nrAc = Int32.Parse(cmbNrOfAc.Text);
            int i = 0;
            int pos = 1;

            while (i < nrAc)
            {
                var row = dgvFlight.Rows[i];
                row.Cells["colCallsign"].Value = callsign + " " + flightNr + "-" + pos;
                row.Cells["colGidOid"].Value = "3" + flightNr + "/" + flightNr + pos;

                if (pos == 1) row.Cells["colYardstick"].Value = flightNr + "Y";
                else
                {
                    row.Cells["colYardstick"].Value = (Int32.Parse(flightNr) + 63).ToString() + "Y";
                }
                row.Cells["colLsr"].Value = "15" + flightNr + pos;

                i++; pos++;
            }
        }

        private string getCharactersFromCallsign()
        {
            return txtCallsign.Text.Trim().Split(' ')[0].ToString();
        }

        private string getCharactersFromCallsign(string callsign)
        {
            return callsign.Trim().Split(' ')[0].ToString() as string;
        }

        private string getPosFromCallsign()
        {
            return txtCallsign.Text.Trim().Substring(txtCallsign.Text.Length - 1);
        }

        private void dgvSupport_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (dgv.CurrentCell != null)
            {
                string currentCellContent = (string)dgv.CurrentCell.Value;

                if (currentCellContent != null && currentCellContent != "-")
                {
                    Tuple tuple = null;
                    var row = dgv.CurrentRow;

                    if (dgv.CurrentCell.OwningColumn.Name == "colChannelSupport")
                    {
                        tuple = (Tuple)list.Find(x => x.getChannel().ToLower().Equals(formatChannel(currentCellContent.ToLower())));

                        if (tuple != null)
                        {
                            // only set cells when the two other cells are empty
                            if ((row.Cells["colFreqSupport"].Value.ToString() == "-" && row.Cells["colPresetSupport"].Value.ToString() == "-") || (row.Cells["colFreqSupport"].Value.ToString() == "" && row.Cells["colPresetSupport"].Value.ToString() == ""))
                            {
                                setRadio(dgv, tuple);
                            } else
                            {
                                row.Cells["colChannelSupport"].Value = formatChannel(row.Cells["colChannelSupport"].Value.ToString().ToUpper());
                            }
                        } else
                        {
                            if (formatChannel(row.Cells["colChannelSupport"].Value.ToString()) != null) {
                                row.Cells["colChannelSupport"].Value = formatChannel(row.Cells["colChannelSupport"].Value.ToString()).ToUpper();
                            } else
                            {
                                errorInvalidFormat();
                                row.Cells["colChannelSupport"].Value = "-";
                            }
                        }
                    }

                    else if (dgv.CurrentCell.OwningColumn.Name == "colFreqSupport")
                    {
                        string s = currentCellContent;
                        if (s.Length == 3 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]))
                        {
                            s += ".000";
                        }
                        else if (s.Length == 4 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]) && s[3] == '.')
                        {
                            s += "000";
                        }
                        else if (s.Length == 5 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]) && s[3] == '.' && Char.IsDigit(s[4]))
                        {
                            s += "00";
                        }
                        else if (s.Length == 6 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]) && s[3] == '.' && Char.IsDigit(s[4]) && Char.IsDigit(s[5]))
                        {
                            s += "0";
                        }
                        else if (s.Length == 2 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]))
                        {
                            s += ".00";
                        }
                        else if (s.Length == 3 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '.')
                        {
                            s += "00";
                        }
                        else if (s.Length == 4 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '.' && Char.IsDigit(s[3]))
                        {
                            s += "0";
                        }
                        else if (s.Length == 5 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '.' && Char.IsDigit(s[3]) && Char.IsDigit(s[4]))
                        {
                            /* Can be both length 5 and 6 in the list, so has to find out which it is
                             * If it is 6, meaning search will yield no match, add a zero */
                    tuple = (Tuple)list.Find(x => x.getFreq().ToLower().Equals(s.ToLower()));
                            if (tuple == null) s += "0";
                        }
                        tuple = (Tuple)list.Find(x => x.getFreq().ToLower().Equals(s.ToLower()));

                        if (tuple != null)
                        {
                            // only set cells when the two other cells are empty
                            if ((row.Cells["colChannelSupport"].Value.ToString() == "-" && row.Cells["colPresetSupport"].Value.ToString() == "-") || (row.Cells["colChannelSupport"].Value.ToString() == "" && row.Cells["colPresetSupport"].Value.ToString() == ""))
                            {
                                setRadio(dgv, tuple);
                            } else
                            {
                                row.Cells["colFreqSupport"].Value = formatChannel(row.Cells["colFreqSupport"].Value.ToString().ToUpper());
                            }
                        }
                    }
                }
            }
        }

        private void setRadio(DataGridView dgv, Tuple tuple)
        {
            if (tuple != null && dgv != null)
            {
                string freq = "", preset = "", channel = "";

                freq = tuple.getFreq();
                preset = tuple.getPreset();
                channel = tuple.getChannel();

                var row = dgv.CurrentRow;
                row.Cells["colChannelSupport"].Value = channel;
                row.Cells["colFreqSupport"].Value = freq;
                row.Cells["colPresetSupport"].Value = preset;
            }
        }

        private void txtTasking_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.Text.Equals("AR") || cb.Text.Equals("XAR") || cb.Text.Equals("GAR") || cb.Text.Equals("SCAR"))
            {
                lblJTAC.Text = "SCAR";
                restore();
            }
            else if (cb.Text.Contains("CAS") && cb.Text.Length <= 4)
            {
                lblJTAC.Text = "JTAC";
                restore();
                setSupportCell("JTAC", "type", "JTAC #2");
            }
            else if (cb.Text.Equals("FAC(A)"))
            {
                lblJTAC.Text = "FAC(A)";
                restore();
            }
            else if (cb.Text.Equals("CSAR") || cb.Text.Contains("EVAC") || cb.Text.Contains("SANDY"))
            {
                lblJTAC.Text = "CSAR";
                restore();
            }
            else if (cb.Text.Contains("AI") || cb.Text.Equals("OCA") || cb.Text.Equals("SEAD") || cb.Text.Equals("SWP") || cb.Text.Equals("ATK") || cb.Text.Equals("Escort") || cb.Text.Contains("CAP") || cb.Text.Equals("Aerial Delivery") || cb.Text.Equals("Air Assault"))
            {
                lblJTAC.Text = "Package";
                moveTacpCallsign();
                hideTacpCallsign();
            }
        }

        private void restore()
        {
            restoreTacpCallsign();
            showTacpCallsign();
        }

        private void moveTacpCallsign()
        {
            // store old position if position is different!
            if (!hasBeenMoved())
            {
                lblJtacPosLeft = lblJTAC.Left;
                lblJtacPosTop = lblJTAC.Top;
                chkTacpPosLeft = chkTacp.Left;
                chkTacpPosTop = chkTacp.Top;
            }

            // move to new position
            lblJTAC.Location = lblTacpCallsign.Location;
            chkTacp.Location = txtTacpCallsign.Location;
            chkTacp.Top += 3;
        }

        private Boolean hasBeenMoved()
        {
            if (lblJtacPosLeft != 0 || lblJtacPosTop != 0 || chkTacpPosLeft != 0 || chkTacpPosTop != 0)
            {
                return true;
            }
            return false;
        }

        private void restoreTacpCallsign()
        {
            if (lblJtacPosLeft != 0 && lblJtacPosTop != 0 && chkTacpPosLeft != 0 && chkTacpPosTop != 0)
            {
                lblJTAC.Left = lblJtacPosLeft;
                lblJTAC.Top = lblJtacPosTop;
                chkTacp.Left = chkTacpPosLeft;
                chkTacp.Top = chkTacpPosTop;
            }
        }

        private void hideTacpCallsign()
        {
            lblTacpCallsign.Hide();
            txtTacpCallsign.Hide();
        }

        private void showTacpCallsign()
        {
            lblTacpCallsign.Show();
            txtTacpCallsign.Show();
        }

        /* If leads lasercode is changed, the rest of the flight gets the appropriate lasercode as well */
        private void dgvFlight_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv.CurrentCell != null)
            {
                string currentCellContent = dgv.CurrentCell.Value as string;

                // LASERCODE
                if (currentCellContent != null && dgv.CurrentRow.Index == 0 && dgv.CurrentCell.OwningColumn.HeaderText == "LSR")
                {
                    if (validLaser(currentCellContent)) {
                        for (int i = 1; i < Int32.Parse(cmbNrOfAc.Text); i++)
                        {
                            dgv.Rows[dgv.CurrentRow.Index + i].Cells[dgv.CurrentCell.ColumnIndex].Value = Int32.Parse(currentCellContent) + i;
                        }
                    }
                }

                // TACAN
                if (currentCellContent != null && dgv.CurrentRow.Index == 0 && dgv.CurrentCell.OwningColumn.HeaderText == "TCN")
                {
                    if (validTacan(currentCellContent))
                    {
                        for (int i = 1; i < Int32.Parse(cmbNrOfAc.Text); i++)
                        {
                            if (currentCellContent.Length == 2)
                            {
                                dgv.Rows[dgv.CurrentRow.Index + i].Cells[dgv.CurrentCell.ColumnIndex].Value = Int32.Parse(currentCellContent.Substring(0, 1)) + 63 + currentCellContent.Substring(1,1);
                            }

                            if (currentCellContent.Length == 3)
                            {
                                dgv.Rows[dgv.CurrentRow.Index + i].Cells[dgv.CurrentCell.ColumnIndex].Value = Int32.Parse(currentCellContent.Substring(0, 2)) + 63 + currentCellContent.Substring(2,1);
                            }
                        }
                    }
                }

                // GID/OID
                if (currentCellContent != null && dgv.CurrentRow.Index == 0 && dgv.CurrentCell.OwningColumn.HeaderText == "GID/OID")
                {
                    if (validGidOid(currentCellContent))
                    {
                        for (int i = 1; i < Int32.Parse(cmbNrOfAc.Text); i++)
                        {
                            dgv.Rows[dgv.CurrentRow.Index + i].Cells[dgv.CurrentCell.ColumnIndex].Value = Int32.Parse(currentCellContent.Substring(0, 2)) + "/" + (Int32.Parse(currentCellContent.Substring(3, 2)) + i);
                        }
                    }
                }
            }
        }

        private bool validTacan(string tacan)
        {
            string s = tacan;

            if (s.Length == 2 && Char.IsDigit(s[0]) && Char.IsLetter(s[1]) && (s[1] == 'X' || s[1] == 'Y'))
            {
                return true;
            } else if (s.Length == 3 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsLetter(s[2]) && (s[2] == 'X' || s[2] == 'Y'))
            {
                return true;
            }
            return false;
        }

        private bool validLaser(string lasercode)
        {
            string s = lasercode;
            if (s.Length == 4 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]) && Char.IsDigit(s[3]))
            {
                return true;
            }
            return false;
        }

        private bool validGidOid(string gidOid)
        {
            string s = gidOid;
            if (s.Length == 5 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '/' && Char.IsDigit(s[3]) && Char.IsDigit(s[4]))
            {
                return true;
            }
            return false;
        }

        private void txtStepTime_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb.Text.Length == 2)
            {
                if (Char.IsDigit(tb.Text[0]) && Char.IsDigit(tb.Text[1]))
                {
                    tb.AppendText(":");
                }
            }
        }

        private void txtStepTime_Enter(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Length == 1 && tb.Text.Equals(":"))
            {
                tb.Text = "";
            }
        }
    }

    public class Range
    {
        string range;
        string cp;
        string abbrev; //abbreviated version of the range
        string joker;
        string bingo;
        string freq;
        string backup;
        string fp;
        string amp;

        public Range(string range, string cp, string abbrev, string joker, string bingo, string freq, string backup, string fp, string amp)
        {
            this.range = range;
            this.cp = cp;
            this.abbrev = abbrev;
            this.joker = joker;
            this.bingo = bingo;
            this.freq = freq;
            this.backup = backup;
            this.fp = fp;
            this.amp = amp;
        }

        public string getRange()
        {
            return this.range;
        }

        public string getCp()
        {
            return this.cp;
        }

        public string getAbbrev()
        {
            return this.abbrev;
        }

        public string getJoker()
        {
            return this.joker;
        }

        public string getBingo()
        {
            return this.bingo;
        }

        public string getFreq()
        {
            return this.freq;
        }

        public string getBackup()
        {
            return this.backup;
        }

        public string getFp()
        {
            return this.fp;
        }

        public string getAmp()
        {
            return this.amp;
        }
    }

    public class Airbase
    {
        string identifier;
        string name;
        string tacan;
        string ground;
        string tower;
        string tma;
        string elevation;
        string runways;
        string ils;

        public Airbase(string identifier, string name, string tacan, string ground, string tower, string tma, string elevation, string runways, string ils)
        {
            this.identifier = identifier;
            this.name = name;
            this.tacan = tacan;
            this.ground = ground;
            this.tower = tower;
            this.tma = tma;
            this.elevation = elevation;
            this.runways = runways;
            this.ils = ils;
        }

        public string getIdentifier(string id)
        {
            return this.identifier;
        }

        public string getName()
        {
            return this.name;
        }

        public string getTacan()
        {
            return this.tacan;
        }

        public string getGround()
        {
            return this.ground;
        }

        public string getTower()
        {
            return this.tower;
        }

        public string getTma()
        {
            return this.tma;
        }

        public string getElevation()
        {
            return this.elevation;
        }

        public string getRunways()
        {
            return this.runways;
        }

        public string getIls()
        {
            return this.ils;
        }
    }

    public class Tuple
    {
        string preset;
        string name;
        string freq;
        string channel;

        public Tuple ()
        {
            name = "";
        }

        public void print()
        {
            Console.WriteLine(preset);

            if (name != "")
            {
                Console.WriteLine(name);
            }

            Console.WriteLine(freq);

            if (channel != null)
            {
                Console.WriteLine(channel);
            }

            Console.WriteLine();
        }

        public void setPreset(string word)
        {
            this.preset = word.Trim();
        }

        public void setName(string word)
        {
            if (this.name == "")
            {
                this.name = word.Trim();
            } else
            {
                this.name += " " + word.Trim();
            }
        }

        public void setFreq(string word)
        {
            this.freq = word.Trim();
        }

        public void setChannel(string word)
        {
            if (this.channel == null)
            {
                this.channel = word.Trim();
            } else
            {
                this.channel += " " + word.Trim();
            }
        }

        public string getPreset()
        {
            return this.preset;
        }

        public string getName()
        {
            return this.name;
        }

        public string getFreq()
        {
            return this.freq;
        }

        public string getChannel()
        {
            if (this.channel == null) return "";
            return this.channel;
        }
    }
}
