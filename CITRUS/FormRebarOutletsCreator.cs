using Autodesk.Revit.DB.Structure;
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
    public partial class FormRebarOutletsCreator : Form
    {
        public string OutletSizesCheckedButtonName;
        public string ForceTypeCheckedButtonName;
        public string ColumnArrangementCheckedButtonName;

        public double ManualOverlapLength;
        public double ManualAnchorageLength;
        public double OffsetFromSlabBottom;

        public RebarBarType mySelectionStirrupBarTape;

        public FormRebarOutletsCreator(List<RebarBarType> stirrupBarTapes)
        {
            InitializeComponent();

            List<RebarBarType> stirrupBarTapesForComboBox = stirrupBarTapes;
            comboBox_StirrupBarTapes.DataSource = stirrupBarTapesForComboBox;
            comboBox_StirrupBarTapes.DisplayMember = "Name";
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            OutletSizesCheckedButtonName = group_OutletSizes.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            ForceTypeCheckedButtonName = groupBox_ForceType.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            ColumnArrangementCheckedButtonName = groupBox_ColumnArrangement.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox_ManualOverlapLength_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ManualOverlapLength.Text, out ManualOverlapLength);
        }

        private void textBox_ManualAnchorageLength_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ManualAnchorageLength.Text, out ManualAnchorageLength);
        }

        private void textBox_OffsetFromSlabBottom_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_OffsetFromSlabBottom.Text, out OffsetFromSlabBottom);

            if (textBox_OffsetFromSlabBottom.Text == string.Empty)
            {
                label_RequiredField.Text = "Укажите смещение для растяжения";
                label_RequiredField.ForeColor = Color.Red;
            }

            else if (textBox_OffsetFromSlabBottom.Text != string.Empty)
            {
                label_RequiredField.Text = "Отлично";
                label_RequiredField.ForeColor = Color.Green;
            }
        }

        private void comboBox_StirrupBarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionStirrupBarTape = comboBox_StirrupBarTapes.SelectedItem as RebarBarType;
        }

    }
}
