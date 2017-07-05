using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLua;

namespace DcsDataImporter
{
    public partial class Form2 : Form
    {

        Dictionary<string, Waypoint> waypoints = new Dictionary<string, Waypoint>();
        LinkedList<string> waypointIDs = new LinkedList<string>();
        string tacticalAirControl = "";


        public Form2()
        {
            InitializeComponent();

            initFp();
            initTma();

            loadWaypoints(); // load waypoints when coming from form3 using back
        }

        public Form2(string dep, string arr, string alt, string loc, string tac, bool loadPrev)
        {
            InitializeComponent();
            initFp();
            initTma();

            setAirbases(dep, arr, alt);

            if (isRange(loc))
            {
                lblKillbox.Text = "Range: ";
                setRange(loc);
            } else
            {
                setKillbox(loc);
            }
            tacticalAirControl = tac;

            if (loadPrev) // if loadPrev has been sent from form1, load the previous waypoints
            {
                loadWaypoints(); // load waypoints when coming from form1 using next
            }
        }

        private bool isRange(string loc)
        {
            bool answer = false;

            if (loc.Equals("dusheti range", StringComparison.InvariantCultureIgnoreCase))
            {
                answer = true;
            }

            if (loc.Equals("tianeti range", StringComparison.InvariantCultureIgnoreCase))
            {
                answer = true;
            }

            if (loc.Equals("marnueli range", StringComparison.InvariantCultureIgnoreCase))
            {
                answer = true;
            }

            if (loc.Equals("tetra range", StringComparison.InvariantCultureIgnoreCase))
            {
                answer = true;
            }

            return answer;
        }

        private void setAirbases(string dep, string arr, string alt)
        {
            var row = dgvTma.Rows[0];
            row.Cells["colAirbase"].Value = dep;
            row = dgvTma.Rows[1];
            row.Cells["colAirbase"].Value = arr;
            row = dgvTma.Rows[2];
            row.Cells["colAirbase"].Value = alt;

            setTma(dep, arr, alt);

        }

        private void setTma(string dep, string arr, string alt)
        {
            /* string fpTma = "UGTB3," // TMA          // UGTB3-8                              tma
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
                         + "GIMUR/NDB"; */

            string fpUGTB = "UGTB 3-8, 1-3, INIT, MUKH, DED, UG24 L, N, S, GLD, OBO, GOR";
            string fpUGKS = "UGKS KETILAR, UGKS NOSIRI, UGKS RIVER W, UGKS ZENI";
            string fpUG27 = fpUGTB;

            string fp = null;

            if (dep != null && dep != "")
            {
                var row = dgvTma.Rows[0];
                if (dep == "Tbilisi-Lochini") fp = fpUGTB;
                if (dep == "Senaki") fp = fpUGKS;
                if (dep == "Vaziani") fp = fpUG27;
                row.Cells["colTma"].Value = fp;
            }

            if (arr != null && arr != "")
            {
                var row = dgvTma.Rows[1];
                fp = null;

                // Only set TMA if DEP is not the same as ARR to avoid listing TMA twice
                if (arr == "Tbilisi-Lochini" && !fpUGTB.Equals(dgvTma.Rows[0].Cells["colTma"].Value)) fp = fpUGTB;
                if (arr == "Senaki" && !fpUGKS.Equals(dgvTma.Rows[0].Cells["colTma"].Value)) fp = fpUGKS;
                if (arr == "Vaziani" && !fpUG27.Equals(dgvTma.Rows[0].Cells["colTma"].Value)) fp = fpUG27;

                row.Cells["colTma"].Value = fp;
            }

            if (alt != null && alt != "")
            {
                var row = dgvTma.Rows[2];

                fp = null;

                if (alt == "Tbilisi-Lochini")
                {
                    if (!rowTwoIsEqualTo(fpUGTB) && !rowTwoIsBlank())
                    {
                        fp = fpUGTB;
                    }
                    else if (rowTwoIsBlank() && !rowOneIsEqualTo(fpUGTB))
                    {
                        fp = fpUGTB;
                    }
                }

                if (alt == "Senaki")
                {
                    if (!rowTwoIsEqualTo(fpUGKS) && !rowTwoIsBlank())
                    {
                        fp = fpUGKS;
                    }
                    else if (rowTwoIsBlank() && !rowOneIsEqualTo(fpUGKS))
                    {
                        fp = fpUGKS;
                    }
                }

                if (alt == "Vaziani")
                {
                    if (!rowTwoIsEqualTo(fpUG27) && !rowTwoIsBlank())
                    {
                        fp = fpUG27;
                    } else if (rowTwoIsBlank() && !rowOneIsEqualTo(fpUG27))
                    {
                        fp = fpUG27;
                    }
                }

                row.Cells["colTma"].Value = fp;
            }
        }

        private bool rowTwoIsBlank()
        {
            var row = dgvTma.Rows[1];
            bool answer = false;
            if (row.Cells["colTma"].Value == null)
            {
                answer = true;
            }
            return answer;
        }

        private bool rowTwoIsEqualTo(string value)
        {
            var row = dgvTma.Rows[1];
            bool answer = false;
            if (value.Equals(row.Cells["colTma"].Value)) {
                answer = true;
            }
            return answer;
        }

        private bool rowOneIsEqualTo(string value)
        {
            var row = dgvTma.Rows[0];
            bool answer = false;
            if (value.Equals(row.Cells["colTma"].Value)) {
                answer = true;
            }
            return answer;
        }

        private void setKillbox(string loc)
        {
            // strip killbox
            if (loc.StartsWith("killbox", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (string word in loc.Split(' '))
                {
                    loc = word;
                }
            }

            // after stripping verify two characters
            if (loc.Count() == 2)
            {
                loc = loc.ToUpper();
                txtKillbox.Text = loc + "SE, " + loc + "SW, " + loc + "NW, " + loc + "NE, " + loc + "SE";
            }
        }

        private void setRange(string loc)
        {
            string fp = "";

            if (loc.Equals("dusheti range", StringComparison.InvariantCultureIgnoreCase))
            {
                fp = "DR1, DR6-9, DR4-5, DR1, G01-04";
            }

            if (loc.Equals("tianeti range", StringComparison.InvariantCultureIgnoreCase))
            {
                fp = "TR1-6, 1, G05-06, H01-04"; 
            }

            if (loc.Equals("marnueli range", StringComparison.InvariantCultureIgnoreCase))
            {
                fp = "MR1-4";
            }

            if (loc.Equals("tetra range", StringComparison.InvariantCultureIgnoreCase))
            {
                fp = "MUKHRANI/NDB, OBORA, GIMUR/NDB, J05-01, TE1-5, TE1";
            }

            txtKillbox.Text = fp;
        }

        private void initFp()
        {
            int nrOfWpts = 27;

            dgvFlightplan.RowCount = nrOfWpts;

            int counter = 0;

            while (counter < nrOfWpts)
            {
                var row = dgvFlightplan.Rows[counter];
                row.Cells["colWpt"].Value = counter + 1;
                counter++;
            }
        }

        private void initTma()
        {
            dgvTma.RowCount = 3;

            var row = dgvTma.Rows[0];
            row.Cells["colType"].Value = "DEP";

            row = dgvTma.Rows[1];
            row.Cells["colType"].Value = "ARR";

            row = dgvTma.Rows[2];
            row.Cells["colType"].Value = "ALT";

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            saveForm();

            Form1 form1 = new Form1();
            form1.Show();
            Hide();
        }

        private void btnImportMissionFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = ".miz|*.miz";
            ofd.Title = "Select mission file";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string missionFileName = ofd.SafeFileName; // only file name
                string missionFileNameWithPath = ofd.FileName; // path
                // string extractPath = Environment.CurrentDirectory;
                Console.WriteLine("Current directory: " + Environment.CurrentDirectory);

                string startPath = @"c:\example\start";
                string zipPath = missionFileNameWithPath;

                /* Delete previous extract directory and build new directory */
            Directory.CreateDirectory(Environment.CurrentDirectory + @"\extract"); // Create dir for first time to avoid exception
                deleteDirectory(Environment.CurrentDirectory + @"\extract");
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\extract");

                string extractPath = Environment.CurrentDirectory + @"\extract";

                ZipFile.ExtractToDirectory(zipPath, extractPath);

                loadMissionFile();
            }
        }

        private void loadMissionFile()
        {
            string extractPath = Environment.CurrentDirectory + @"\extract";

            loadDictionaryFile();

            string missionFile = extractPath + @"\mission";

            loadMissionFileData();
        }

        private void loadDictionaryFile()
        {
            string extractPath = Environment.CurrentDirectory + @"\extract";

            string dictionaryFile = extractPath + @"\l10n\DEFAULT\dictionary";

            Dictionary<int, string> dic = new Dictionary<int, string>();

            /* read file line by line */
            int counter = 0;
            string line;

            StreamReader file = new StreamReader(dictionaryFile);

            while ((line = file.ReadLine()) != null)
            {
                // Need to save every line that contains DictKey_
                if (line.StartsWith("    [\"DictKey_"))
                {
                    dic.Add(extractIdFromLineInDictionaryFile(line), line);

                }
                counter++;
            }

            file.Close();

            Console.ReadLine();

            int nrIDs = counter;

            counter = 0;
            string callsign = Properties.Settings.Default.prevTxtAwacsCallsign;
            bool foundCallsign = false;
            string value;

            while (counter < nrIDs)
            {
                if (dic.TryGetValue(counter, out value))
                {
                    if (value.StartsWith("    [\"DictKey_UnitName") && value.Contains(callsign))
                    {
                        foundCallsign = true;
                    }
                    else
                    {
                        if (foundCallsign && value.StartsWith("    [\"DictKey_WptName"))
                        {
                            /* Jump out of while loop if a waypoint has no name: means that the end of the waypoints for the flight has been reached */
                            if (getWaypointFromLineInDictionaryFile(value).Equals(""))
                            {
                                break;
                            }

                            string dictName = getDictName(value);
                            string displayName = getWaypointFromLineInDictionaryFile(value);

                            setWaypoint(dictName, displayName);

                            /* A linked list of waypoint IDs */
                            waypointIDs.AddLast(dictName);
                        }
                    }
                }
                counter++;
            }
        }

        private string getWaypointFromLineInDictionaryFile(string line)
        {
            foreach (string word in line.Split('"'))
            {
                if (!word.Contains("    [") && !word.StartsWith("DictKey_") && !word.Equals("] = "))
                {
                    return word;
                }
            }
            return null;
        }

        private string getDictName(string line)
        {
            foreach (string word in line.Split('"'))
            {
                if (word.StartsWith("DictKey_WptName_"))
                {
                    /* Make sure the word contains a number, e.g. DictKey_WptName_19 */
                    foreach (string part in word.Split('_'))
                    {
                        if (isNumber(part))
                        {
                            return word;
                        }
                    }
                }
            }
            return "";
        }

        /* Creates waypoint and displays it in the datagridview */
        private void setWaypoint(string dictName, string displayName)
        {
            Waypoint wp = new Waypoint(dictName);
            wp.setDisplayName(displayName);

            /* A hashmap of waypoints */
            waypoints.Add(dictName, wp);

            /* Set name of last waypoint */
            var row = dgvFlightplan.Rows[waypoints.Count - 1];
            row.Cells["colName"].Value = displayName;
        }

        private int extractIdFromLineInDictionaryFile(string line)
        {
            int id = 0;

            foreach (string word in line.Split('_', '\"'))
            {
                if (isNumber(word))
                {
                    id = Int32.Parse(word);
                }
            }
            return id;
        }

        private void deleteDirectory(string path)
        {
            // Note: Cannot have the directory open while code is running or it will cause an exception
            // Can solve this if replacing this method with a better method
            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private void loadMissionFileData()
        {

            string extractPath = Environment.CurrentDirectory + @"\extract";
            string missionFile = extractPath + @"\mission";

            Waypoint wp;
            foreach (string dictName in waypointIDs)
            {
                if (waypoints.TryGetValue(dictName, out wp))
                {
                    //Console.WriteLine(wp.getDictName());
                    //Console.WriteLine(wp.getDisplayName());

                    string line;
                    string prevLine = null;
                    string twoLinesBack = null;

                    StreamReader file = new StreamReader(missionFile);

                    while ((line = file.ReadLine()) != null)
                    {
                        // Need to save every line that contains DictKey_
                        if (line.Contains(dictName))
                        {
                            wp.setX(stripXYCoordinates(prevLine.Trim()));
                            wp.setY(stripXYCoordinates(twoLinesBack.Trim()));
                            
                            //wp.setX(prevLine.Trim());
                            //wp.setY(twoLinesBack.Trim());

                        }

                        // Can have line1, line2, line3 etc and remember the last 100 lines to make sure we get every data
                        twoLinesBack = prevLine;
                        prevLine = line;
                    }
                }
            }

            showFlightPlan();
        }

        private void showFlightPlan()
        {
            Waypoint wp;
            int nrOfWpt = 0;
            foreach (string dictName in waypointIDs)
            {
                if (waypoints.TryGetValue(dictName, out wp))
                {
                    // debug
                    Console.WriteLine(wp.getDictName() + " : " + wp.getDisplayName() + " : " + wp.getX() + " : " + wp.getY());

                    /* add waypoint information to datagridview */
                    var row = dgvFlightplan.Rows[nrOfWpt];
                    row.Cells["colName"].Value = wp.getDisplayName();
                    row.Cells["colPos"].Value = wp.getX() + ", " + wp.getY();

                    nrOfWpt++;
                }
            }
        }

        private string stripXYCoordinates(string x)
        {
            string removeStringX = "[\"x\"] = ";
            string removeStringY = "[\"y\"] = ";
            x = x.Remove(0, removeStringX.Length); // same length for x and y
            x = x.Remove(x.Length - 1); // removes the last , which is same for x and y
            return x;
        }

        private string convertFromVecToLatLong(string x_string, string y_string, string z_string)
        {
            int x = Int32.Parse(x_string);
            int y = Int32.Parse(y_string);
            int z = Int32.Parse(z_string);


            // Lua l = new Lua();
            // l.DoFile("lua.log");
            // LuaFunction f = _convertMetersToLatLon["convertMetersToLatLon"] as LuaFunction;
            //if (f != null)
            //{
            //    f.Call("My log message");
            //}

            return "";
        }

        public bool isNumber(string word)
        {
            if (word.Length == 1 && Char.IsDigit(word[0]))
            {
                return true;
            }

            if (word.Length == 2 && (Char.IsDigit(word[0]) && Char.IsDigit(word[1])))
            {
                return true;
            }
            return false;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // ToDo: Deselect the blue squares on currentcell in dgv's (doesn't work due to bug)

            lblKillbox.Focus(); // Move focus away so no blue markings appear on screenshot
            System.Threading.Thread.Sleep(50); // sleep to make sure focus is moved

            btnNext.Hide();
            btnBack.Hide();
            btnImportMissionFile.Hide();

            string pathA10c = @"\Kneeboard Groups\A-10C";
            captureScreen(Properties.Settings.Default.pathKneeboardBuilder + pathA10c);

            saveForm();

            Form3 form3 = new Form3(tacticalAirControl);
            form3.Show(); // Show next flight form
            Hide(); // Hide form1
        }

        private void saveForm()
        {
            saveWaypoints();
            saveTma();
        }

        private void loadForm()
        {
            loadWaypoints();
            loadTma();
        }

        private void saveWaypoints()
        {
            var row = dgvFlightplan.Rows[0];

            Properties.Settings.Default.wpr0c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr0c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr0c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr0c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr0c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr0c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr0c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[1];

            Properties.Settings.Default.wpr1c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr1c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr1c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr1c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr1c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr1c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr1c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[2];

            Properties.Settings.Default.wpr2c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr2c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr2c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr2c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr2c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr2c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr2c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[3];

            Properties.Settings.Default.wpr3c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr3c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr3c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr3c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr3c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr3c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr3c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[4];

            Properties.Settings.Default.wpr4c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr4c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr4c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr4c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr4c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr4c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr4c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[5];

            Properties.Settings.Default.wpr5c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr5c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr5c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr5c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr5c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr5c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr5c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[6];

            Properties.Settings.Default.wpr6c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr6c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr6c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr6c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr6c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr6c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr6c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[7];

            Properties.Settings.Default.wpr7c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr7c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr7c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr7c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr7c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr7c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr7c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[8];

            Properties.Settings.Default.wpr8c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr8c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr8c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr8c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr8c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr8c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr8c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[9];

            Properties.Settings.Default.wpr9c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr9c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr9c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr9c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr9c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr9c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr9c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[10];

            Properties.Settings.Default.wpr10c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr10c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr10c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr10c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr10c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr10c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr10c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[11];

            Properties.Settings.Default.wpr11c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr11c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr11c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr11c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr11c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr11c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr11c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[12];

            Properties.Settings.Default.wpr12c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr12c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr12c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr12c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr12c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr12c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr12c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[13];

            Properties.Settings.Default.wpr13c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr13c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr13c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr13c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr13c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr13c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr13c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[14];

            Properties.Settings.Default.wpr14c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr14c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr14c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr14c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr14c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr14c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr14c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[15];

            Properties.Settings.Default.wpr15c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr15c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr15c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr15c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr15c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr15c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr15c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[16];

            Properties.Settings.Default.wpr16c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr16c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr16c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr16c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr16c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr16c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr16c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[17];

            Properties.Settings.Default.wpr17c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr17c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr17c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr17c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr17c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr17c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr17c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[18];

            Properties.Settings.Default.wpr18c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr18c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr18c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr18c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr18c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr18c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr18c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[19];

            Properties.Settings.Default.wpr19c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr19c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr19c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr19c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr19c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr19c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr19c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[20];

            Properties.Settings.Default.wpr20c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr20c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr20c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr20c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr20c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr20c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr20c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[21];

            Properties.Settings.Default.wpr21c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr21c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr21c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr21c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr21c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr21c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr21c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[22];

            Properties.Settings.Default.wpr22c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr22c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr22c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr22c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr22c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr22c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr22c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[23];

            Properties.Settings.Default.wpr23c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr23c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr23c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr23c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr23c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr23c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr23c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[24];

            Properties.Settings.Default.wpr24c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr24c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr24c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr24c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr24c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr24c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr24c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[25];

            Properties.Settings.Default.wpr25c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr25c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr25c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr25c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr25c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr25c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr25c7 = row.Cells[7].Value as string;

            row = dgvFlightplan.Rows[26];

            Properties.Settings.Default.wpr26c1 = row.Cells[1].Value as string;
            Properties.Settings.Default.wpr26c2 = row.Cells[2].Value as string;
            Properties.Settings.Default.wpr26c3 = row.Cells[3].Value as string;
            Properties.Settings.Default.wpr26c4 = row.Cells[4].Value as string;
            Properties.Settings.Default.wpr26c5 = row.Cells[5].Value as string;
            Properties.Settings.Default.wpr26c6 = row.Cells[6].Value as string;
            Properties.Settings.Default.wpr26c7 = row.Cells[7].Value as string;
        }

        private void loadWaypoints()
        {
            var row = dgvFlightplan.Rows[0];

            row.Cells[1].Value = Properties.Settings.Default.wpr0c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr0c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr0c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr0c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr0c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr0c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr0c7;

            row = dgvFlightplan.Rows[1];

            row.Cells[1].Value = Properties.Settings.Default.wpr1c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr1c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr1c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr1c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr1c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr1c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr1c7;

            row = dgvFlightplan.Rows[2];

            row.Cells[1].Value = Properties.Settings.Default.wpr2c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr2c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr2c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr2c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr2c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr2c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr2c7;

            row = dgvFlightplan.Rows[3];

            row.Cells[1].Value = Properties.Settings.Default.wpr3c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr3c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr3c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr3c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr3c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr3c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr3c7;

            row = dgvFlightplan.Rows[4];

            row.Cells[1].Value = Properties.Settings.Default.wpr4c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr4c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr4c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr4c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr4c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr4c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr4c7;

            row = dgvFlightplan.Rows[5];

            row.Cells[1].Value = Properties.Settings.Default.wpr5c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr5c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr5c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr5c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr5c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr5c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr5c7;

            row = dgvFlightplan.Rows[6];

            row.Cells[1].Value = Properties.Settings.Default.wpr6c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr6c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr6c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr6c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr6c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr6c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr6c7;

            row = dgvFlightplan.Rows[7];

            row.Cells[1].Value = Properties.Settings.Default.wpr7c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr7c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr7c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr7c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr7c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr7c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr7c7;

            row = dgvFlightplan.Rows[8];

            row.Cells[1].Value = Properties.Settings.Default.wpr8c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr8c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr8c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr8c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr8c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr8c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr8c7;

            row = dgvFlightplan.Rows[9];

            row.Cells[1].Value = Properties.Settings.Default.wpr9c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr9c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr9c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr9c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr9c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr9c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr9c7;

            row = dgvFlightplan.Rows[10];

            row.Cells[1].Value = Properties.Settings.Default.wpr10c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr10c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr10c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr10c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr10c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr10c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr10c7;

            row = dgvFlightplan.Rows[11];

            row.Cells[1].Value = Properties.Settings.Default.wpr11c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr11c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr11c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr11c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr11c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr11c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr11c7;

            row = dgvFlightplan.Rows[12];

            row.Cells[1].Value = Properties.Settings.Default.wpr12c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr12c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr12c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr12c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr12c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr12c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr12c7;

            row = dgvFlightplan.Rows[13];

            row.Cells[1].Value = Properties.Settings.Default.wpr13c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr13c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr13c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr13c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr13c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr13c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr13c7;

            row = dgvFlightplan.Rows[14];

            row.Cells[1].Value = Properties.Settings.Default.wpr14c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr14c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr14c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr14c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr14c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr14c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr14c7;

            row = dgvFlightplan.Rows[15];

            row.Cells[1].Value = Properties.Settings.Default.wpr15c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr15c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr15c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr15c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr15c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr15c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr15c7;

            row = dgvFlightplan.Rows[16];

            row.Cells[1].Value = Properties.Settings.Default.wpr16c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr16c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr16c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr16c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr16c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr16c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr16c7;

            row = dgvFlightplan.Rows[17];

            row.Cells[1].Value = Properties.Settings.Default.wpr17c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr17c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr17c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr17c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr17c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr17c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr17c7;

            row = dgvFlightplan.Rows[18];

            row.Cells[1].Value = Properties.Settings.Default.wpr18c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr18c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr18c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr18c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr18c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr18c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr18c7;

            row = dgvFlightplan.Rows[19];

            row.Cells[1].Value = Properties.Settings.Default.wpr19c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr19c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr19c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr19c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr19c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr19c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr19c7;

            row = dgvFlightplan.Rows[20];

            row.Cells[1].Value = Properties.Settings.Default.wpr20c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr20c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr20c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr20c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr20c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr20c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr20c7;

            row = dgvFlightplan.Rows[21];

            row.Cells[1].Value = Properties.Settings.Default.wpr21c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr21c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr21c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr21c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr21c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr21c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr21c7;

            row = dgvFlightplan.Rows[22];

            row.Cells[1].Value = Properties.Settings.Default.wpr22c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr22c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr22c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr22c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr22c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr22c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr22c7;

            row = dgvFlightplan.Rows[23];

            row.Cells[1].Value = Properties.Settings.Default.wpr23c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr23c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr23c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr23c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr23c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr23c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr23c7;

            row = dgvFlightplan.Rows[24];

            row.Cells[1].Value = Properties.Settings.Default.wpr24c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr24c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr24c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr24c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr24c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr24c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr24c7;

            row = dgvFlightplan.Rows[25];

            row.Cells[1].Value = Properties.Settings.Default.wpr25c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr25c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr25c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr25c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr25c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr25c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr25c7;

            row = dgvFlightplan.Rows[26];

            row.Cells[1].Value = Properties.Settings.Default.wpr26c1;
            row.Cells[2].Value = Properties.Settings.Default.wpr26c2;
            row.Cells[3].Value = Properties.Settings.Default.wpr26c3;
            row.Cells[4].Value = Properties.Settings.Default.wpr26c4;
            row.Cells[5].Value = Properties.Settings.Default.wpr26c5;
            row.Cells[6].Value = Properties.Settings.Default.wpr26c6;
            row.Cells[7].Value = Properties.Settings.Default.wpr26c7;
        }

        private void saveTma()
        {
            // TBD.
        }

        private void loadTma()
        {
            // TBD.
        }

        private void captureScreen(string path)
        {
            System.Drawing.Rectangle bounds = this.Bounds;
            //using (Bitmap bitmap = new Bitmap(bounds.Width - 20, bounds.Height - 40))
            using (Bitmap bitmap = new Bitmap(bounds.Width - 6, bounds.Height - 30))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {   // Left + is to the right, - is to the left
                    // Top + is down, - is up
                    // g.CopyFromScreen(new System.Drawing.Point(bounds.Left+10, bounds.Top+30), System.Drawing.Point.Empty, bounds.Size);
                    g.CopyFromScreen(new System.Drawing.Point(bounds.Left + 3, bounds.Top + 30), System.Drawing.Point.Empty, bounds.Size);
                }
                /* Save it to kneeboard */
                Directory.CreateDirectory(path + @"\MDC");
                bitmap.Save(path + @"\MDC\MDC-001.png");
            }
        }
    }

    public class Waypoint
    {
        string displayName;
        string dictName; // ID
        string number;
        string x;
        string y;
        string speed;
        string eta;
        string alt;

        public Waypoint(string dictName)
        {
            this.dictName = dictName;
        }

        public void setDisplayName(string name)
        {
            this.displayName = name;
        }

        public string getDisplayName()
        {
            return this.displayName;
        }

        public void setDictName(string name)
        {
            this.dictName = name;
        }

        public string getDictName()
        {
            return this.dictName;
        }

        public void setX(string x)
        {
            this.x = x;
        }

        public string getX()
        {
            return this.x;
        }

        public void setY(string y)
        {
            this.y = y;
        }

        public string getY()
        {
            return this.y;
        }
    }

}
