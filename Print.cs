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
    public partial class Print : Form
    {
        public Print()
        {
            InitializeComponent();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            Bill rpt = new Bill();
            crystalReportViewer1.ReportSource = rpt;
        }
    }
}
