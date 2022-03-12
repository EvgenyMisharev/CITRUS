using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS
{
    public partial class MEPViewScheduleHostStartForm : Form
    {
        public string FirstSeetNumber = "";
        public MEPViewScheduleHostStartForm()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox_FirstSeetNumber_TextChanged(object sender, EventArgs e)
        {
            FirstSeetNumber = textBox_FirstSeetNumber.Text;
        }
    }
}
