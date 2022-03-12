using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.RebarGroupCopier
{
    public partial class RebarGroupCopierForm : Form
    {
        public string CheckedButtonName;
        public string ColumnArrangementСheckedButtonName;

        public RebarGroupCopierForm()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            CheckedButtonName = groupBox_GroupTypes.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            ColumnArrangementСheckedButtonName = groupBox_ColumnArrangement.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
