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
    public partial class FloorTypeSelectorForCapitalMaker : System.Windows.Forms.Form
    {
        public FloorType mySelectionFloorTypeForCapitalMaker;
        public double myOffsetForCapitalMaker_X1;
        public double myOffsetForCapitalMaker_Y1;
        public double myOffsetForCapitalMaker_X2;
        public double myOffsetForCapitalMaker_Y2;
        public FloorTypeSelectorForCapitalMaker (List<FloorType> floors)
        {
            InitializeComponent();
            List<FloorType> floorTypListForComboBox = floors;
            comboBox_FloorTypesForCapitalMaker.DataSource = floorTypListForComboBox;
            comboBox_FloorTypesForCapitalMaker.DisplayMember = "Name";
        }

        private void FloorTypeSelectorForCapitalMaker_Load(object sender, EventArgs e)
        {

        }

        private void comboBox_FloorTypesForCapitalMaker_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionFloorTypeForCapitalMaker = comboBox_FloorTypesForCapitalMaker.SelectedItem as FloorType;
        }

        private void button1_Ok_Click(object sender, EventArgs e)
        {
            double.TryParse(textBox1_Offset.Text, out myOffsetForCapitalMaker_X1);
            double.TryParse(textBox2_Offset.Text, out myOffsetForCapitalMaker_Y1);
            double.TryParse(textBoxX2_Offset.Text, out myOffsetForCapitalMaker_X2);
            double.TryParse(textBoxY2_Offset.Text, out myOffsetForCapitalMaker_Y2);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox1_Offset_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_Offset_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxX2_Offset_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxY2_Offset_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
