using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_03_3_Insolation
{
    public partial class InsolationAtPointForm : Form
    {
        public string VerificationOption;
        public bool CheckSelectedPanels;
        public bool CheckSelectedPoints;
        public bool WallsAndFloorsGeometry;
        public InsolationAtPointForm()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            VerificationOption = groupBox_VerificationOptions.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            CheckSelectedPanels = checkBox_CheckSelectedPanels.Checked;
            CheckSelectedPoints = checkBox_CheckSelectedPoints.Checked;
            WallsAndFloorsGeometry = checkBox_WallsAndFloorsGeometry.Checked;
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
