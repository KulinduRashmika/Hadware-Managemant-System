using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_CW_2
{
    public partial class Stockprint : Form
    {
        public Stockprint()
        {
            InitializeComponent();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            stockreport rpt = new stockreport();
            crystalReportViewer1.ReportSource = rpt;
        }
    }
}
