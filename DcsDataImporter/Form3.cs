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

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void cbMunitions2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbMunitions3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbMunitions4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            /* Todo: add back functionality */
        }
    }
}
