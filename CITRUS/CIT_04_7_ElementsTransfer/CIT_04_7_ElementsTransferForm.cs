using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_04_7_ElementsTransfer
{
    public partial class CIT_04_7_ElementsTransferForm : Form
    {
        public bool FloorTransferCheck;
        public bool СolumnTransferCheck;
        public bool WallTransferCheck;
        public bool BeamTransferCheck;
        public bool FoundatioTransferCheck;

        public bool ReplaceFloorType;
        public bool ReplaceСolumnType;
        public bool ReplaceWallType;
        public bool ReplaceBeamType;

        public CIT_04_7_ElementsTransferForm()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            FloorTransferCheck = checkBox_FloorTransfer.Checked;
            СolumnTransferCheck = checkBox_СolumnTransfer.Checked;
            WallTransferCheck = checkBox_WallTransfer.Checked;
            BeamTransferCheck = checkBox_BeamTransfer.Checked;
            FoundatioTransferCheck = checkBox_FoundatioTransfer.Checked;

            ReplaceFloorType = checkBox_ReplaceFloorType.Checked;
            ReplaceСolumnType = checkBox_ReplaceСolumnType.Checked;
            ReplaceWallType = checkBox_ReplaceWallType.Checked;
            ReplaceBeamType = checkBox_ReplaceBeamType.Checked;

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
