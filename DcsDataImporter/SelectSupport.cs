using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DcsDataImporter
{
    public partial class SelectSupport : Form
    {
        string AmsndatMsnNumber;
        string airbaseDep;
        string airbaseArr;
        string NrAc;
        string Callsign;
        string Awacs;
        string AwacsChn;
        string AwacsBackupChn;
        string AwacsCp;
        string Tacp;
        string TacpType;
        string TacpChn;
        string TacpBackupChn;
        string TacpCp;
        string location;
        string tasking;
        string InternalChn;
        string InternalBackupChn;
        string internalFreq;
        string internalBackupFreq;
        string amplification;
        bool standardTraining;
        string AmsndatTakeoffTime;
        string timeFrom;
        string timeTo;

        public SelectSupport()
        {
            InitializeComponent();
        }

        public SelectSupport(string AmsndatMsnNumber, string airbaseDep, string airbaseArr, string NrAc, string Callsign, string Awacs, string AwacsChn, string AwacsBackupChn, string AwacsCp, string Tacp, string TacpType, string TacpChn, string TacpBackupChn, string TacpCp, string location, string tasking, string InternalChn, string InternalBackupChn, string amplification, bool chkTraining, string AmsndatTakeoffTime, string timeFrom, string timeTo)
        {
            InitializeComponent();

            this.AmsndatMsnNumber = AmsndatMsnNumber;
            this.airbaseDep = airbaseDep;
            this.airbaseArr = airbaseArr;
            this.NrAc = NrAc;
            this.Callsign = Callsign;
            this.Awacs = Awacs;
            this.AwacsChn = AwacsChn;
            this.AwacsBackupChn = AwacsBackupChn;
            this.AwacsCp = AwacsCp;
            this.Tacp = Tacp;
            this.TacpType = TacpType;
            this.TacpChn = TacpChn;
            this.TacpBackupChn = TacpBackupChn;
            this.TacpCp = TacpCp;
            this.location = location;
            this.tasking = tasking;
            this.InternalChn = InternalChn;
            this.InternalBackupChn = InternalBackupChn;
            this.amplification = amplification;
            this.standardTraining = chkTraining;
            this.AmsndatTakeoffTime = AmsndatTakeoffTime;
            this.timeFrom = timeFrom;
            this.timeTo = timeTo;

            if (standardTraining)
            {
                chkCsar.Checked = false;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (calcRows() > 9)
            {
                MessageBox.Show("Too many frequencies selected. Try deselecting a few and try again.");
            } else
            {
                Hide();
                Form1 form1 = new Form1(AmsndatMsnNumber, airbaseDep, airbaseArr, NrAc, Callsign, Awacs, AwacsChn, AwacsBackupChn, AwacsCp, Tacp, TacpType, TacpChn, TacpBackupChn, TacpCp, location, tasking, InternalChn, InternalBackupChn, amplification, standardTraining, AmsndatTakeoffTime, chkTma.Checked, chkAwacsAG.Checked, chkAwacsAA.Checked, chkExtraAwacsAG.Checked, chkExtraAwacsAA.Checked, chkFaca.Checked, chkCsar.Checked, chkJstar.Checked, chkScramble.Checked, chkExtraJtac.Checked, chkExtraPackage.Checked, numTankers.Text, this, timeFrom, timeTo);
                form1.Show();
            }
        }

        private int calcRows()
        {
            int sum = 0;

            if (chkAwacsAG.Checked)
            {
                sum++;
            }

            if (chkExtraAwacsAG.Checked)
            {
                sum++;
            }

            if (chkAwacsAA.Checked)
            {
                sum++;
            }

            if (chkExtraAwacsAA.Checked)
            {
                sum++;
            }

            if (chkFaca.Checked)
            {
                sum++;
            }

            if (chkCsar.Checked)
            {
                sum++;
            }

            if (chkJstar.Checked)
            {
                sum++;
            }

            if (chkScramble.Checked)
            {
                sum++;
            }

            if (chkExtraJtac.Checked)
            {
                sum++;
            }

            if (chkExtraPackage.Checked)
            {
                sum++;
            }

            sum += Int32.Parse(numTankers.Value.ToString());

            return sum;
        }
    }
}
