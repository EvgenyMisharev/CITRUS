using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_03_1_WallFinishCreator
{
    public partial class CIT_03_1_WallFinishCreatorForm : System.Windows.Forms.Form
    {
        public WallType mySelectionWallTypeFirst;
        public double MainWallFinishHeight = 0;

        public CIT_03_1_WallFinishCreatorForm(List<WallType> wallTypeFirstList)
        {
            InitializeComponent();

            List<WallType> wallTypeFirstListForComboBox = wallTypeFirstList;
            comboBox_WallTypeFirst.DataSource = wallTypeFirstListForComboBox;
            comboBox_WallTypeFirst.DisplayMember = "Name";
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

        private void comboBox_WallTypeFirst_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionWallTypeFirst = comboBox_WallTypeFirst.SelectedItem as WallType;
        }

        private void textBox_MainWallFinishHeight_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_MainWallFinishHeight.Text, out MainWallFinishHeight);
        }
    }
}
