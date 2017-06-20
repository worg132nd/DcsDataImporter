using System;
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
// using Microsoft.Office.Interop.Word;

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
 * Samtidig som BINGO økes, økes også JOKER slik at JOKER aldri er mindre enn 500 over BINGO.
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
        public bool standardTrainingSet = false;

        private bool hasTma;

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
        }

        /* Constructor with no ATO */
        public Form1(bool standardTraining)
        {
            if (standardTraining) standardTrainingSet = true;
            init();
            loadPrevMission();
        }    

        /* Constructor when ATO is filled out */
        public Form1(string AmsndatMsnNumber, string airbaseDep, string airbaseArr, string NrAc, string Callsign, string Awacs, string AwacsChn, string AwacsBackupChn, string AwacsCp, string Tacp, string TacpType, string TacpChn, string TacpBackupChn, string TacpCp, string location, string tasking, string internalFreq, string internalBackupFreq, string amplification, bool standardTraining, string takeoffTime, bool tma, bool chkAwacsAG, bool chkAwacsAA, bool chkExtraAwacsAG, bool chkExtraAwacsAA, bool chkFaca, bool chkCsar, bool chkJstar, bool chkScramble, bool chkExtraJtac, bool chkExtraPackage, string numTankers, Form selectSupport)
        {
            init();

            // Clear out default zeroes
            txtAwacsPreset.Text = txtTacpPreset.Text = txtInternalPreset.Text = txtInternalBackupPreset.Text = txtAwacsBackupPreset.Text = txtTacpBackupPreset.Text = "";

            if (Tacp == null) disableTacp();
            if (Awacs == null) disableAwacs();

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

            // Set correct tasking
            if (tasking.Equals("TR"))
            {
                txtTasking.Text = "Training";
            }
            else
            {
                txtTasking.Text = tasking;
            }

            if (!takeoffTime.Equals("-") && !takeoffTime.Equals(""))
            {
                txtTakeoffTime.Text = takeoffTime[0].ToString() + takeoffTime[1].ToString() + ":" + takeoffTime[2].ToString() + takeoffTime[3].ToString();
            }

            if (standardTraining)
            {
                standardTrainingSet = true;
                txtTacpCp.Text = "MUKHRANI"; //default
                setSupportCell("JTAC", "notes", "MUKHRANI");
                disableAwacs();
            }

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

            setRadio("AWACS", formatChannel(AwacsChn), formatChannel(AwacsBackupChn));
            setRadio("TACP", formatChannel(TacpChn), formatChannel(TacpBackupChn));
            setRadio("internal", formatChannel(internalFreq), formatChannel(internalBackupFreq));
        }

        private void init()
        {
            InitializeComponent();
            initDataGridViews();
        }

        private void initDataGridViews()
        {
            initDataGridView(dgvAirbase, 3);
            initDataGridView(dgvFlight, 4);
            initDataGridView(dgvSupport, 10);
            // initSupport();
        }

        public void initDataGridView(DataGridView dgv, int rowCount)
        {
            dgv.RowCount = rowCount;
            dgv.DefaultCellStyle.SelectionBackColor = dgv.DefaultCellStyle.BackColor;
            dgv.DefaultCellStyle.SelectionForeColor = dgv.DefaultCellStyle.ForeColor;
        }

        private void initSupport()
        {
            /* fillSupportWithDash();

            var row = dgvSupport.Rows[0];
            row.Cells["colTypeSupport"].Value = "AWACS A-G";
            //row.Cells["colNotesSupport"].Value = "WD";
            row = dgvSupport.Rows[1];
            row.Cells["colTypeSupport"].Value = "AWACS A-A";
            //row.Cells["colNotesSupport"].Value = "WD";
            row = dgvSupport.Rows[2];
            row.Cells["colTypeSupport"].Value = "FAC(A)";
            row = dgvSupport.Rows[3];
            row.Cells["colTypeSupport"].Value = "Tanker 1";
            row = dgvSupport.Rows[4];
            row.Cells["colTypeSupport"].Value = "Tanker 2";
            row = dgvSupport.Rows[5];
            row.Cells["colTypeSupport"].Value = "JSTAR";
            row = dgvSupport.Rows[6];
            row.Cells["colTypeSupport"].Value = "In-Flight Report";
            row = dgvSupport.Rows[7];
            row.Cells["colTypeSupport"].Value = "CSAR";
            row = dgvSupport.Rows[8];
            row.Cells["colTypeSupport"].Value = "Package";
            */
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
                row.Cells["colTypeSupport"].Value = "Tanker " + (x + 1).ToString();
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

            loadFirstLineOfMDC();
            loadDGVFlight();
            loadSelectedAirbases();
            loadTimes();
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
        }

        /* Loads first line of the MDC data from last form */
        private void loadFirstLineOfMDC()
        {
            txtCallsign.Text = Properties.Settings.Default.prevTxtCallsign;
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
                        // Channel color
                        lastWordWasChannel = true;
                        tuple.setChannel(word);

                        // Name
                    }
                    else
                    {
                        // Everything not identified is the name
                        tuple.setName(word);
                    }
                }
            }
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
                channel = channel.Insert(channel.IndexOfAny("0123456789".ToCharArray()), " ");
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
                catch (Exception e)
                {
                    MessageBox.Show("Error: ", e.ToString());
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
                    tuple = (Tuple)list.Find(x => x.getChannel().ToLower().Equals(formatChannel(s.ToLower())));
                }
            
                if (textbox.Name.Contains("Freq"))
                {
                    // maa haandtere 23, 237, 237., 237.0, 237.00 og 237.000
                    // håndterer ved å konvertere alle frekvenser til følgende
                    // 23.000, 237.000, 237.000, 237.000, 237.000 og 237.000
                    // append forskjellige antall 000

                    if (s.Length == 3 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]))
                    {
                        s += ".000";
                    } else if (s.Length == 4 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]) && s[3] == '.')
                    {
                        s += "000";
                    } else if (s.Length == 5 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]) && s[3] == '.' && Char.IsDigit(s[4]))
                    {
                        s += "00";
                    } else if (s.Length == 6 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && Char.IsDigit(s[2]) && s[3] == '.' && Char.IsDigit(s[4]) && Char.IsDigit(s[5]))
                    {
                        s += "0";
                    } else if (s.Length == 2 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]))
                    {
                        s += ".00";
                    } else if (s.Length == 3 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '.')
                    {
                        s += "00";
                    } else if (s.Length == 4 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '.' && Char.IsDigit(s[3]))
                    {
                        s += "0";
                    } else if (s.Length == 5 && Char.IsDigit(s[0]) && Char.IsDigit(s[1]) && s[2] == '.' && Char.IsDigit(s[3]) && Char.IsDigit(s[4]))
                    {
                        /* Can be both length 5 and 6 in the list, so has to find out which it is
                         * If it is 6, meaning search will yield no match, add a zero */
                        tuple = (Tuple)list.Find(x => x.getFreq().ToLower().Equals(s.ToLower()));
                        if (tuple == null) s += "0";
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

                        textbox.Text = textbox.Text.ToUpper();
                    }

                    if (textbox.Name.Equals("txtAwacsBackupChannel") && txtAwacsBackupFreq.Text == "" && txtAwacsBackupPreset.Text == "")
                    {
                        txtAwacsBackupFreq.Text = freq;
                        txtAwacsBackupPreset.Text = preset;

                        textbox.Text = textbox.Text.ToUpper();
                    }

                    if (textbox.Name.Equals("txtTacpChannel") && txtTacpFreq.Text == "" && txtTacpPreset.Text == "")
                    {
                        txtTacpFreq.Text = freq;
                        txtTacpPreset.Text = preset;

                        textbox.Text = textbox.Text.ToUpper();
                    }

                    if (textbox.Name.Equals("txtTacpBackupChannel") && txtTacpBackupFreq.Text == "" && txtTacpBackupPreset.Text == "")
                    {
                        txtTacpBackupFreq.Text = freq;
                        txtTacpBackupPreset.Text = preset;

                        textbox.Text = textbox.Text.ToUpper();
                    }

                    if (textbox.Name.Equals("txtInternalChannel") && txtInternalFreq.Text == "" && txtInternalPreset.Text == "")
                    {
                        txtInternalFreq.Text = freq;
                        txtInternalPreset.Text = preset;
                    }

                    if (textbox.Name.Equals("txtInternalBackupChannel") && txtInternalBackupFreq.Text == "" && txtInternalBackupPreset.Text == "")
                    {
                        txtInternalBackupFreq.Text = freq;
                        txtInternalBackupPreset.Text = preset;
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
                }
            }
        }

        private string stripArrlocAndDeplocFromAirportIdentifier(string x)
        {
            string deploc = "DEPLOC:";
            string arrloc = "ARRLOC:";
            string altloc = "ALTLOC:";
            if (x.StartsWith(deploc) || x.StartsWith(arrloc) || x.StartsWith(altloc)) {
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
            }

            /* GEORGIA MAP */
            // Use configuration files for airbases instead. This makes the user capable of changing airbase data himself.

            /* Kobuleti */
            if (identifier.EndsWith("UG5X"))
            {
                /* txtTma.Text = "";
                txtAirportName.Text = "Kobuleti"; */
            }

            /* Gudauta */
            else if (identifier.EndsWith("UG23"))
            {
                /* txtTma.Text = "";
                txtAirportName.Text = "Gudauta"; */
            }

            /* Soganlug */
            else if (identifier.EndsWith("UG24"))
            {
                /* txtTma.Text = "";
                txtAirportName.Text = "Soganlug"; */
            }

            /* Vaziani */
            else if (identifier.EndsWith("UG27"))
            {
                /* txtTma.Text = "";
                txtAirportName.Text = "Vaziani"; */
                setVaziani(row, identifier);
            }

            /* Kutaisi - Kopitnari */
            else if (identifier.EndsWith("UGKO"))
            {
                /* txtTma.Text = "";
                txtAirportName.Text = "Kutaisi"; */
            }

            /* Senaki - Kolkhi */
            else if (identifier.EndsWith("UGKS"))
            {
                setSenaki(row, identifier);
            }

            /* Batumi */
            else if (identifier.EndsWith("UGSB"))
            {
                /* txtTma.Text = "";
                txtAirportName.Text = "Batumi"; */
            }

            /* Sukhumi - Babashara */
            else if (identifier.EndsWith("UGSS"))
            {
                /* txtTma.Text = "";
                txtAirportName.Text = "Sukhumi"; */
            }

            /* Tblisi Lochini */
            else if (identifier.EndsWith("UGTB"))
            {
                setTbilisi(row, identifier);
            }
            else
            {
                // clear current row
                if (identifier.StartsWith("DEPLOC:"))
                {
                    clearRow(0);
                } else if (identifier.StartsWith("ARRLOC:"))
                {
                    clearRow(1);
                } else if (identifier.StartsWith("ALTLOC:"))
                {
                    clearRow(2);
                }
            }
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
                }
            }

            setAirport(identifier);
        }

        private void setTbilisi(DataGridViewRow r, string identifier)
        {
            var row = r;

            row.Cells["colAirbase"].Value = "Tbilisi-Lochini"; // Remember to add only Lochini to the kneeboard
            row.Cells["colTcn"].Value = "25X";
            row.Cells["colGnd"].Value = "138.1";
            row.Cells["colTwr"].Value = "138.2";
            row.Cells["colTma"].Value = "127.2";
            row.Cells["colElev"].Value = "1473";
            row.Cells["colRwy"].Value = "31L/13R";
            row.Cells["colIls"].Value = "108.90/110.30";

            if (identifier.StartsWith("DEPLOC:"))
            {
                txtParking.Text = "APRON 1";
            }

            /*txtAirportName.Text = "Lochini";
            txtRunwayRight.Text = "13R";
            txtRunwayLeft.Text = "31L";
            lblRunwayRight.Text = "Right runway";
            lblRunwayLeft.Text = "Left runway";
            txtTmaFreq.Text = "127.2";
            txtTowerFreq.Text = "138.2";
            txtGroundFreq.Text = "138.1";
            txtTACAN.Text = "25X";
            txtILS.Text = "13R:110.30 31L:108.90";*/

            /* Enabling TMA since Tbilisi has an approach */
            enableTma();

            // txtTma.Text = "Tblisi";

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

        private void setVaziani(DataGridViewRow r, string identifier)
        {
            var row = r;

            row.Cells["colAirbase"].Value = "Vaziani"; // Remember to add only Lochini to the kneeboard
            row.Cells["colTcn"].Value = "22X";
            row.Cells["colGnd"].Value = "140.1";
            row.Cells["colTwr"].Value = "140.2";
            row.Cells["colTma"].Value = "127.2";
            row.Cells["colElev"].Value = "1492";
            row.Cells["colRwy"].Value = "13/31";
            row.Cells["colIls"].Value = "108.75/108.75";

            if (identifier.StartsWith("DEPLOC:"))
            {
                txtParking.Text = "Unknown";
            }

            enableTma();
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

            ToolTip tt = new ToolTip();
            tt.Show("APRON#/RAMP#/PARKING SPACE#/SHELTER#", tb,visibleTime);
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
                if (tianeti.ToLower().Contains(s.ToLower())) {
                    string amp = "FAH 360 +-30. All conventional ordnance authorized. On the strafe panels, only guns is authorized. CBU's only allowed on the artillery firing locations.";
                    setRangeInfo(tianeti, "MUKHRANI", fpTia, "TIA", amp, 3100, 2100);
                    enableTacp();
                    lblJTAC.Text = "Range";
                    setTacpFreq("225.750", "INDIGO 10", 8, "131.750", "CHERRY 10", 18);
                    if (txtTacpCallsign.Text == "") txtTacpCallsign.Text = "Tianeti";

                /* DUSHETI */
                } else if (dusheti.ToLower().Contains(s.ToLower()))
                {
                    string amp = "FAH 022 +-30. All conventional ordnance authorized. On the strafe panels, only guns is authorized.";
                    setRangeInfo(dusheti, "GIMUR", fpDushex, "DUSHEX", amp, 2900, 1900);
                    enableTacp();
                    lblJTAC.Text = "Range";
                    setTacpFreq("247.500", "LIME 11", 6, "140.750", "INDIGO 9", 0);
                    if (txtTacpCallsign.Text == "") txtTacpCallsign.Text = "Dusheti";

                /* TETRA */
                } else if (tetra.ToLower().Contains(s.ToLower()))
                {
                    /* BAGEM is in the middle of TETRA.
                     * LAGAS is in the far west of TETRA, at the westernmost border of the range
                     */
                    string amp = "No FAH restrictions. All conventional ordnance authorized. No use of CBU's in the villages.";
                    setRangeInfo(tetra, "GIMUR", fpTet, "TET", amp, 2800, 1800);
                    enableTacp();
                    lblJTAC.Text = "Range";
                    setTacpFreq("243.500", "RED 10", 9, "127.750", "PURPLE 11", 15); // Backup channel has conflicts with AWACS backup frequency
                    if (txtTacpCallsign.Text == "") txtTacpCallsign.Text = "Tetra";

                /* MARNUELI */
                } else if (marnueli.ToLower().Contains(s.ToLower()))
                {
                    /* Obora is in the Tbilisi TMA, and very close to MUKHRANI.
                     * TISOT is in MARNUELI
                     */
                    string amp = "FAH 225 +-30. All conventional ordnance authorized. On the strafe panels, only guns is authorized.";
                    setRangeInfo(marnueli, "OBORA", fpMar, "MAR", amp, 2900, 1900);
                    enableTacp();
                    lblJTAC.Text = "Range";
                    setTacpFreq("248.500", "PURPLE 1", 7, "124.750", "AMBER 1", 0);
                    if (txtTacpCallsign.Text == "") txtTacpCallsign.Text = "Marnueli";
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
            setFreq("tacp", false);
        }

        private void enableTacp()
        {
            setFreq("tacp", true);
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

            Form2 form2 = new Form2(blank(dep), blank(arr), blank(alt), txtLocation.Text, selectTac());
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
                if (row.Cells["colTypeSupport"].Value.Equals("AWACS A-G"))
                {
                    if (!row.Cells["colCallsignSupport"].Value.Equals("") && !row.Cells["colCallsignSupport"].Value.Equals("-"))
                    {
                        return true;
                    }
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
                if (row.Cells["colTypeSupport"].Value.Equals("AWACS A-G"))
                {
                    if (!row.Cells["colCallsignSupport"].Value.Equals("") && !row.Cells["colCallsignSupport"].Value.Equals("-"))
                    {
                        return row.Cells["colCallsignSupport"].Value.ToString();
                    }
                }
                rowNr++;
            }
            return null;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
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

            Properties.Settings.Default.prevTxtStepTime = txtStepTime.Text;
            Properties.Settings.Default.prevTxtTaxiTime = txtTaxiTime.Text;
            Properties.Settings.Default.prevTxtTakeoffTime = txtTakeoffTime.Text;
            Properties.Settings.Default.prevTxtVulStart = txtVulStart.Text;
            Properties.Settings.Default.prevTxtVulEnd = txtVulEnd.Text;
            Properties.Settings.Default.prevTxtLandingTime = txtLandingTime.Text;
            
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

            var row = dgvSupport.Rows[6];

            /* IFRN */
            Properties.Settings.Default.prevTxtIfrnFreq = row.Cells["colFreqSupport"].Value.ToString().TrimEnd("0".ToCharArray());
            Properties.Settings.Default.prevTxtIfrnChannel = row.Cells["colChannelSupport"].Value as string;
            Properties.Settings.Default.prevTxtIfrnPreset = row.Cells["colPresetSupport"].Value as string;

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

            Properties.Settings.Default.Save();
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

        }

        private void loadDGVSupport()
        {
            /* AWACS A-G */
            
            var row = dgvSupport.Rows[0];     

            row.Cells["colTypeSupport"].Value = Properties.Settings.Default.prevColTypeAwacsAGSupport;
            row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.prevColCallsignAwacsAGSupport;
            row.Cells["colFreqSupport"].Value = Properties.Settings.Default.prevColFreqAwacsAGSupport;
            row.Cells["colChannelSupport"].Value = Properties.Settings.Default.prevColChannelAwacsAGSupport;
            row.Cells["colPresetSupport"].Value = Properties.Settings.Default.prevColPresetAwacsAGSupport;
            row.Cells["colBackupSupport"].Value = Properties.Settings.Default.prevColBackupAwacsAGSupport;
            row.Cells["colNotesSupport"].Value = Properties.Settings.Default.prevColNotesAwacsAGSupport;

            /* AWACS A-A */
            row = dgvSupport.Rows[1];

            row.Cells["colTypeSupport"].Value = Properties.Settings.Default.prevColTypeAwacsAASupport;
            row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.prevColCallsignAwacsAASupport;
            row.Cells["colFreqSupport"].Value = Properties.Settings.Default.prevColFreqAwacsAASupport;
            row.Cells["colChannelSupport"].Value = Properties.Settings.Default.prevColChannelAwacsAASupport;
            row.Cells["colPresetSupport"].Value = Properties.Settings.Default.prevColPresetAwacsAASupport;
            row.Cells["colBackupSupport"].Value = Properties.Settings.Default.prevColBackupAwacsAASupport;
            row.Cells["colNotesSupport"].Value = Properties.Settings.Default.prevColNotesAwacsAASupport;

            /* FAC(A) */
            row = dgvSupport.Rows[2];

            row.Cells["colTypeSupport"].Value = Properties.Settings.Default.prevColTypeFACASupport;
            row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.prevColCallsignFACASupport;
            row.Cells["colFreqSupport"].Value = Properties.Settings.Default.prevColFreqFACASupport;
            row.Cells["colChannelSupport"].Value = Properties.Settings.Default.prevColChannelFACASupport;
            row.Cells["colPresetSupport"].Value = Properties.Settings.Default.prevColPresetFACASupport;
            row.Cells["colBackupSupport"].Value = Properties.Settings.Default.prevColBackupFACASupport;
            row.Cells["colNotesSupport"].Value = Properties.Settings.Default.prevColNotesFACASupport;

            /* Tanker 1 */
            row = dgvSupport.Rows[3];

            row.Cells["colTypeSupport"].Value = Properties.Settings.Default.prevColTypeTanker1Support;
            row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.prevColCallsignTanker1Support;
            row.Cells["colFreqSupport"].Value = Properties.Settings.Default.prevColFreqTanker1Support;
            row.Cells["colChannelSupport"].Value = Properties.Settings.Default.prevColChannelTanker1Support;
            row.Cells["colPresetSupport"].Value = Properties.Settings.Default.prevColPresetTanker1Support;
            row.Cells["colBackupSupport"].Value = Properties.Settings.Default.prevColBackupTanker1Support;
            row.Cells["colNotesSupport"].Value = Properties.Settings.Default.prevColNotesTanker1Support;

            /* Tanker 2 */
            row = dgvSupport.Rows[4];

            row.Cells["colTypeSupport"].Value = Properties.Settings.Default.prevColTypeTanker2Support;
            row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.prevColCallsignTanker2Support;
            row.Cells["colFreqSupport"].Value = Properties.Settings.Default.prevColFreqTanker2Support;
            row.Cells["colChannelSupport"].Value = Properties.Settings.Default.prevColChannelTanker2Support;
            row.Cells["colPresetSupport"].Value = Properties.Settings.Default.prevColPresetTanker2Support;
            row.Cells["colBackupSupport"].Value = Properties.Settings.Default.prevColBackupTanker2Support;
            row.Cells["colNotesSupport"].Value = Properties.Settings.Default.prevColNotesTanker2Support;

            /* JSTAR */
            row = dgvSupport.Rows[5];

            row.Cells["colTypeSupport"].Value = Properties.Settings.Default.prevColTypeJSTARSupport;
            row.Cells["colCallsignSupport"].Value = Properties.Settings.Default.prevColCallsignJSTARSupport;
            row.Cells["colFreqSupport"].Value = Properties.Settings.Default.prevColFreqJSTARSupport;
            row.Cells["colChannelSupport"].Value = Properties.Settings.Default.prevColChannelJSTARSupport;
            row.Cells["colPresetSupport"].Value = Properties.Settings.Default.prevColPresetJSTARSupport;
            row.Cells["colBackupSupport"].Value = Properties.Settings.Default.prevColBackupJSTARSupport;
            row.Cells["colNotesSupport"].Value = Properties.Settings.Default.prevColNotesJSTARSupport;
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
            Properties.Settings.Default.prevColLsrLead = row.Cells["colLsr"].Value as string;
            Properties.Settings.Default.prevColNotesLead = row.Cells["colNotes"].Value as string;

            /* Wing */
            row = dgvFlight.Rows[1];

            Properties.Settings.Default.prevColPosWing = row.Cells["colPos"].Value as string;
            Properties.Settings.Default.prevColCallsignWing = row.Cells["colCallsign"].Value as string;
            Properties.Settings.Default.prevColPilotWing = row.Cells["colPilot"].Value as string;
            Properties.Settings.Default.prevColGidOidWing = row.Cells["colGidOid"].Value as string;
            Properties.Settings.Default.prevColYardstickWing = row.Cells["colYardstick"].Value as string;
            Properties.Settings.Default.prevColLsrWing = row.Cells["colLsr"].Value as string;
            Properties.Settings.Default.prevColNotesWing = row.Cells["colNotes"].Value as string;

            /* Element */
            row = dgvFlight.Rows[2];

            Properties.Settings.Default.prevColPosElement = row.Cells["colPos"].Value as string;
            Properties.Settings.Default.prevColCallsignElement = row.Cells["colCallsign"].Value as string;
            Properties.Settings.Default.prevColPilotElement = row.Cells["colPilot"].Value as string;
            Properties.Settings.Default.prevColGidOidElement = row.Cells["colGidOid"].Value as string;
            Properties.Settings.Default.prevColYardstickElement = row.Cells["colYardstick"].Value as string;
            Properties.Settings.Default.prevColLsrElement = row.Cells["colLsr"].Value as string;
            Properties.Settings.Default.prevColNotesElement = row.Cells["colNotes"].Value as string;

            /* Wing 2: trail */
            row = dgvFlight.Rows[3];

            Properties.Settings.Default.prevColPosTrail = row.Cells["colPos"].Value as string;
            Properties.Settings.Default.prevColCallsignTrail = row.Cells["colCallsign"].Value as string;
            Properties.Settings.Default.prevColPilotTrail = row.Cells["colPilot"].Value as string;
            Properties.Settings.Default.prevColGidOidTrail = row.Cells["colGidOid"].Value as string;
            Properties.Settings.Default.prevColYardstickTrail = row.Cells["colYardstick"].Value as string;
            Properties.Settings.Default.prevColLsrTrail = row.Cells["colLsr"].Value as string;
            Properties.Settings.Default.prevColNotesTrail = row.Cells["colNotes"].Value as string;
        }

        private void saveDGVSupport()
        {
            /* AWACS A-G */
            var row = dgvSupport.Rows[0];

            Properties.Settings.Default.prevColTypeAwacsAGSupport = row.Cells["colTypeSupport"].Value as string;
            Properties.Settings.Default.prevColCallsignAwacsAGSupport = row.Cells["colCallsignSupport"].Value as string;
            Properties.Settings.Default.prevColFreqAwacsAGSupport = row.Cells["colFreqSupport"].Value as string;
            Properties.Settings.Default.prevColChannelAwacsAGSupport = row.Cells["colChannelSupport"].Value as string;
            Properties.Settings.Default.prevColPresetAwacsAGSupport = row.Cells["colPresetSupport"].Value as string;
            Properties.Settings.Default.prevColBackupAwacsAGSupport = row.Cells["colBackupSupport"].Value as string;
            Properties.Settings.Default.prevColNotesAwacsAGSupport = row.Cells["colNotesSupport"].Value as string;

            /* AWACS A-A */
            row = dgvSupport.Rows[1];

            Properties.Settings.Default.prevColTypeAwacsAASupport = row.Cells["colTypeSupport"].Value as string;
            Properties.Settings.Default.prevColCallsignAwacsAASupport = row.Cells["colCallsignSupport"].Value as string;
            Properties.Settings.Default.prevColFreqAwacsAASupport = row.Cells["colFreqSupport"].Value as string;
            Properties.Settings.Default.prevColChannelAwacsAASupport = row.Cells["colChannelSupport"].Value as string;
            Properties.Settings.Default.prevColPresetAwacsAASupport = row.Cells["colPresetSupport"].Value as string;
            Properties.Settings.Default.prevColBackupAwacsAASupport = row.Cells["colBackupSupport"].Value as string;
            Properties.Settings.Default.prevColNotesAwacsAASupport = row.Cells["colNotesSupport"].Value as string;

            /* FAC(A) */
            row = dgvSupport.Rows[2];

            Properties.Settings.Default.prevColTypeFACASupport = row.Cells["colTypeSupport"].Value as string;
            Properties.Settings.Default.prevColCallsignFACASupport = row.Cells["colCallsignSupport"].Value as string;
            Properties.Settings.Default.prevColFreqFACASupport = row.Cells["colFreqSupport"].Value as string;
            Properties.Settings.Default.prevColChannelFACASupport = row.Cells["colChannelSupport"].Value as string;
            Properties.Settings.Default.prevColPresetFACASupport = row.Cells["colPresetSupport"].Value as string;
            Properties.Settings.Default.prevColBackupFACASupport = row.Cells["colBackupSupport"].Value as string;
            Properties.Settings.Default.prevColNotesFACASupport = row.Cells["colNotesSupport"].Value as string;

            /* Tanker 1 */
            row = dgvSupport.Rows[3];

            Properties.Settings.Default.prevColTypeTanker1Support = row.Cells["colTypeSupport"].Value as string;
            Properties.Settings.Default.prevColCallsignTanker1Support = row.Cells["colCallsignSupport"].Value as string;
            Properties.Settings.Default.prevColFreqTanker1Support = row.Cells["colFreqSupport"].Value as string;
            Properties.Settings.Default.prevColChannelTanker1Support = row.Cells["colChannelSupport"].Value as string;
            Properties.Settings.Default.prevColPresetTanker1Support = row.Cells["colPresetSupport"].Value as string;
            Properties.Settings.Default.prevColBackupTanker1Support = row.Cells["colBackupSupport"].Value as string;
            Properties.Settings.Default.prevColNotesTanker1Support = row.Cells["colNotesSupport"].Value as string;

            /* Tanker 2 */
            row = dgvSupport.Rows[4];

            Properties.Settings.Default.prevColTypeTanker2Support = row.Cells["colTypeSupport"].Value as string;
            Properties.Settings.Default.prevColCallsignTanker2Support = row.Cells["colCallsignSupport"].Value as string;
            Properties.Settings.Default.prevColFreqTanker2Support = row.Cells["colFreqSupport"].Value as string;
            Properties.Settings.Default.prevColChannelTanker2Support = row.Cells["colChannelSupport"].Value as string;
            Properties.Settings.Default.prevColPresetTanker2Support = row.Cells["colPresetSupport"].Value as string;
            Properties.Settings.Default.prevColBackupTanker2Support = row.Cells["colBackupSupport"].Value as string;
            Properties.Settings.Default.prevColNotesTanker2Support = row.Cells["colNotesSupport"].Value as string;

            /* JSTAR */
            row = dgvSupport.Rows[5];

            Properties.Settings.Default.prevColTypeJSTARSupport = row.Cells["colTypeSupport"].Value as string;
            Properties.Settings.Default.prevColCallsignJSTARSupport = row.Cells["colCallsignSupport"].Value as string;
            Properties.Settings.Default.prevColFreqJSTARSupport = row.Cells["colFreqSupport"].Value as string;
            Properties.Settings.Default.prevColChannelJSTARSupport = row.Cells["colChannelSupport"].Value as string;
            Properties.Settings.Default.prevColPresetJSTARSupport = row.Cells["colPresetSupport"].Value as string;
            Properties.Settings.Default.prevColBackupJSTARSupport = row.Cells["colBackupSupport"].Value as string;
            Properties.Settings.Default.prevColNotesJSTARSupport = row.Cells["colNotesSupport"].Value as string;
        }

        /* Select kneeboardbuilder file and save path */
        public static void setKneeboardPath()
        {
            string fileDialogText = "Select the kneeboardbuilder application file";
            string filter = ".exe|*.exe";

            if (Properties.Settings.Default.pathKneeboardBuilder.Equals("") && Properties.Settings.Default.pathKneeboardBuilder != null)
            {
                Properties.Settings.Default.pathKneeboardBuilder = getPath(fileDialogText, filter);
                Properties.Settings.Default.Save();
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

            return false;
        }

        void storeAsPdf(string path)
        {
            /* string wordDoc = "temp.docm";
            string pdfDoc = "temp.pdf";

            if (!path.EndsWith(@"\"))
            {
                path += @"\";
            }

            Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
            Document wordDocument = appWord.Documents.Open(path + wordDoc);
            wordDocument.ExportAsFixedFormat(path + pdfDoc, WdExportFormat.wdExportFormatPDF);

            wordDocument.Close(); */
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
                Directory.CreateDirectory(path + @"\MDC");
                bitmap.Save(path + @"\MDC\MDC-000.png");
            }
        }

        /* void saveAsPngTwo(string pdfFileName, string savePath)
        {
            string pdfPath = Environment.CurrentDirectory + "\\" + pdfFileName + ".pdf";

            Console.WriteLine(pdfPath);

            while (!File.Exists(pdfPath))
            {
                System.Threading.Thread.Sleep(50);
            }

            // File exists when arriving here

            if (!savePath.EndsWith("\\"))
            {
                savePath += "\\";
            }

            Console.WriteLine(pdfFileName);
            
            // Do NOT save com5-aci and com7-aco
            if (pdfFileName == "com1-sta")
            {
                savePath += "StandardTraining-004";
            } else if (pdfFileName == "com2-to")
            {
                savePath += "StandardTraining-006";
            } else if (pdfFileName == "com3-dep")
            {
                savePath += "StandardTraining-008";
            } else if (pdfFileName == "com4-tma")
            {
                savePath += "StandardTraining-011";
            } else if (pdfFileName == "com6-cas")
            {
                savePath += "StandardTraining-013";
            } else if (pdfFileName == "com8-arr")
            {
                savePath += "StandardTraining-021";
            } else if (pdfFileName == "com9-lan")
            {
                savePath += "StandardTraining-022";
            } else if (pdfFileName == "com10-tax")
            {
                savePath += "StandardTraining-024";
            }

            Console.WriteLine("SavePath: " + savePath);

            convertPdfToPng(pdfPath, savePath);
        }*/

        /* private void convertPdfToPng(string pdfPath, string output)
        {
            if (File.Exists(pdfPath))
            {
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
        } */










        void saveAsPng(string pdfFileName, string path)
        {

            while (!File.Exists(Environment.CurrentDirectory + "\\" + pdfFileName + ".pdf"))
            {
                System.Threading.Thread.Sleep(50);
                Console.WriteLine("Sleeping: cannot find file " + Environment.CurrentDirectory + "\\" + pdfFileName + ".pdf");
            }

            if (File.Exists(Environment.CurrentDirectory + "\\" + pdfFileName + ".pdf"))
            {
                if (!path.EndsWith("\\"))
                {
                    path += "\\";
                }

                string pdfPath = Environment.CurrentDirectory + "\\" + pdfFileName + ".pdf";
                string dir = pdfFileName.ToUpper().Replace('-', ' ');
                string outFileName = dir + "-000.png";
                dir += "\\";

                // TODO: Check that directory exists, and if not, create it before saving
                // TODO: If the file has more than one page, need to add 001, 002 and so on, instead of always overwriting 000

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
                    if (File.Exists(replacementFp)) File.Delete(replacementFp);
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
            setFlightTable(cb.Text);

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

            string callsign = getCharactersFromCallsign();
            string flightNr = GetFirstDigit(txtCallsign.Text);
            string pos = getPosFromCallsign();

            updateFlight(callsign, flightNr);
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

                    if (dgv.CurrentCell.OwningColumn.Name == "colChannelSupport")
                    {
                        tuple = (Tuple)list.Find(x => x.getChannel().ToLower().Equals(formatChannel(currentCellContent.ToLower())));

                        if (tuple != null)
                        {
                            setRadio(dgv, tuple);
                        } else
                        {
                            var row = dgv.CurrentRow;
                            row.Cells["colChannelSupport"].Value = row.Cells["colChannelSupport"].Value.ToString().ToUpper();
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
                            setRadio(dgv, tuple);
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
