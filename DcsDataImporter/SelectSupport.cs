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

        public SelectSupport()
        {
            InitializeComponent();
        }

        public SelectSupport(string AmsndatMsnNumber, string airbaseDep, string airbaseArr, string NrAc, string Callsign, string Awacs, string AwacsChn, string AwacsBackupChn, string AwacsCp, string Tacp, string TacpType, string TacpChn, string TacpBackupChn, string TacpCp, string location, string tasking, string InternalChn, string InternalBackupChn, string amplification, bool chkTraining, string AmsndatTakeoffTime)
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
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Hide();
            Form1 form1 = new Form1(AmsndatMsnNumber, airbaseDep, airbaseArr, NrAc, Callsign, Awacs, AwacsChn, AwacsBackupChn, AwacsCp, Tacp, TacpType, TacpChn, TacpBackupChn, TacpCp, location, tasking, InternalChn, InternalBackupChn, amplification, standardTraining, AmsndatTakeoffTime);
            form1.Show();
        }
    }
}
