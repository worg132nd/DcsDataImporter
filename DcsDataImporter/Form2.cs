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
        }

        public Form2(string dep, string arr, string alt, string loc, string tac)
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

                    // hent ut til man får treff i mission fila: samle alle data

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
            btnNext.Hide();
            btnBack.Hide();
            btnImportMissionFile.Hide();

            string pathA10c = @"\Kneeboard Groups\A-10C";
            captureScreen(Properties.Settings.Default.pathKneeboardBuilder + pathA10c);

            Form3 form3 = new Form3(tacticalAirControl);
            form3.Show(); // Show next flight form
            Hide(); // Hide form1
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
