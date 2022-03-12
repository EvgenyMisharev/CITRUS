using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS
{
    public partial class FloorTypeSelector : System.Windows.Forms.Form
    {
        public FloorType mySelectionFloorType;
        public FloorTypeSelector (List<FloorType> floors)
        {   
            InitializeComponent();
            List<FloorType> floorTypListForComboBox = floors;
            comboBox_FloorTypes.DataSource = floorTypListForComboBox;
            comboBox_FloorTypes.DisplayMember = "Name";
        }


        private void comboBox_FloorTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionFloorType = comboBox_FloorTypes.SelectedItem as FloorType;
        }

        private void button1_Ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
