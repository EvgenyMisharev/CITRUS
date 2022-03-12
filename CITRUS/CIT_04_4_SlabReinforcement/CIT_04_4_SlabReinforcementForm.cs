using Autodesk.Revit.DB.Structure;
using CITRUS.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_04_4_SlabReinforcement
{
    public partial class CIT_04_4_SlabReinforcementForm : Form
    {
        public RebarBarType mySelectionBottomXDirectionRebarTape;
        public RebarBarType mySelectionBottomYDirectionRebarTape;
        public RebarBarType mySelectionTopXDirectionRebarTape;
        public RebarBarType mySelectionTopYDirectionRebarTape;

        public double BottomXDirectionRebarSpacing;
        public double BottomYDirectionRebarSpacing;
        public double TopXDirectionRebarSpacing;
        public double TopYDirectionRebarSpacing;

        public RebarCoverType mySelectionRebarCoverTypeForTop;
        public RebarCoverType mySelectionRebarCoverTypeForBottom;

        public double PerimeterFramingDiam;
        public double PerimeterFramingOverlaping;
        public double PerimeterFramingEndCoverLayer;
        public double PerimeterFramingStep;

        public bool PerimeterFraming;
        public bool BottomXDirection;
        public bool BottomYDirection;
        public bool TopXDirection;
        public bool TopYDirection;

        SR_Settings sr_Settings = null;

        public CIT_04_4_SlabReinforcementForm(List<RebarBarType> BottomXDirectionRebarTapesList,
            List<RebarBarType> BottomYDirectionRebarTapesList
            , List<RebarBarType> TopXDirectionRebarTapesList
            , List<RebarBarType> TopYDirectionRebarTapesList
            , List<RebarCoverType> rebarCoverTypesForTop
            , List<RebarCoverType> rebarCoverTypesForBottom)
        {
            InitializeComponent();
            sr_Settings = SR_Settings.GetSettings();
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SR_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                textBox_BottomXDirectionRebarSpacing.Text = sr_Settings.BottomXDirectionRebarSpacingSettings;
                textBox_BottomYDirectionRebarSpacing.Text = sr_Settings.BottomYDirectionRebarSpacingSettings;
                textBox_TopXDirectionRebarSpacing.Text = sr_Settings.TopXDirectionRebarSpacingSettings;
                textBox_TopYDirectionRebarSpacing.Text = sr_Settings.TopYDirectionRebarSpacingSettings;

                textBox_PerimeterFramingDiam.Text = sr_Settings.PerimeterFramingDiamSettings;
                textBox_PerimeterFramingOverlaping.Text = sr_Settings.PerimeterFramingOverlapingSettings;
                textBox_PerimeterFramingEndCoverLayer.Text = sr_Settings.PerimeterFramingEndCoverLayerSettings;
                textBox_PerimeterFramingStep.Text = sr_Settings.PerimeterFramingStepSettings;
            }

            List<RebarBarType> bottomXDirectionRebarTapesListForComboBox = BottomXDirectionRebarTapesList;
            comboBox_BottomXDirectionRebarTapes.DataSource = bottomXDirectionRebarTapesListForComboBox;
            comboBox_BottomXDirectionRebarTapes.DisplayMember = "Name";
            comboBox_BottomXDirectionRebarTapes.SelectedItem = bottomXDirectionRebarTapesListForComboBox.FirstOrDefault(rbt => rbt.Name == sr_Settings.mySelectionBottomXDirectionRebarTapeSettings);

            List<RebarBarType> bottomYDirectionRebarTapesListForComboBox = BottomYDirectionRebarTapesList;
            comboBox_BottomYDirectionRebarTapes.DataSource = bottomYDirectionRebarTapesListForComboBox;
            comboBox_BottomYDirectionRebarTapes.DisplayMember = "Name";
            comboBox_BottomYDirectionRebarTapes.SelectedItem = bottomYDirectionRebarTapesListForComboBox.FirstOrDefault(rbt => rbt.Name == sr_Settings.mySelectionBottomYDirectionRebarTapeSettings);

            List<RebarBarType> topXDirectionRebarTapesListForComboBox = TopXDirectionRebarTapesList;
            comboBox_TopXDirectionRebarTapes.DataSource = topXDirectionRebarTapesListForComboBox;
            comboBox_TopXDirectionRebarTapes.DisplayMember = "Name";
            comboBox_TopXDirectionRebarTapes.SelectedItem = topXDirectionRebarTapesListForComboBox.FirstOrDefault(rbt => rbt.Name == sr_Settings.mySelectionTopXDirectionRebarTapeSettings);

            List<RebarBarType> topYDirectionRebarTapesListForComboBox = TopYDirectionRebarTapesList;
            comboBox_TopYDirectionRebarTapes.DataSource = topYDirectionRebarTapesListForComboBox;
            comboBox_TopYDirectionRebarTapes.DisplayMember = "Name";
            comboBox_TopYDirectionRebarTapes.SelectedItem = topYDirectionRebarTapesListForComboBox.FirstOrDefault(rbt => rbt.Name == sr_Settings.mySelectionTopYDirectionRebarTapeSettings);

            List<RebarCoverType> rebarCoverTypesForTopForComboBox = rebarCoverTypesForTop;
            comboBox_RebarCoverTypeForTop.DataSource = rebarCoverTypesForTopForComboBox;
            comboBox_RebarCoverTypeForTop.DisplayMember = "Name";
            comboBox_RebarCoverTypeForTop.SelectedItem = rebarCoverTypesForTopForComboBox.FirstOrDefault(rbt => rbt.Name == sr_Settings.mySelectionRebarCoverTypeForTopSettings);

            List<RebarCoverType> rebarCoverTypesForBottomForComboBox = rebarCoverTypesForBottom;
            comboBox_RebarCoverTypeForBottom.DataSource = rebarCoverTypesForBottomForComboBox;
            comboBox_RebarCoverTypeForBottom.DisplayMember = "Name";
            comboBox_RebarCoverTypeForBottom.SelectedItem = rebarCoverTypesForBottomForComboBox.FirstOrDefault(rbt => rbt.Name == sr_Settings.mySelectionRebarCoverTypeForBottomSettings);
        }

        private void button1_Ok_Click(object sender, EventArgs e)
        {
            mySelectionBottomXDirectionRebarTape = comboBox_BottomXDirectionRebarTapes.SelectedItem as RebarBarType;
            mySelectionBottomYDirectionRebarTape = comboBox_BottomYDirectionRebarTapes.SelectedItem as RebarBarType;
            mySelectionTopXDirectionRebarTape = comboBox_TopXDirectionRebarTapes.SelectedItem as RebarBarType;
            mySelectionTopYDirectionRebarTape = comboBox_TopYDirectionRebarTapes.SelectedItem as RebarBarType;
            mySelectionRebarCoverTypeForTop = comboBox_RebarCoverTypeForTop.SelectedItem as RebarCoverType;
            mySelectionRebarCoverTypeForBottom = comboBox_RebarCoverTypeForBottom.SelectedItem as RebarCoverType;

            PerimeterFraming = checkBox_PerimeterFraming.Checked;
            BottomXDirection = checkBox_BottomXDirection.Checked;
            BottomYDirection = checkBox_BottomYDirection.Checked;
            TopXDirection = checkBox_TopXDirection.Checked;
            TopYDirection = checkBox_TopYDirection.Checked;

            sr_Settings.BottomXDirectionRebarSpacingSettings = textBox_BottomXDirectionRebarSpacing.Text;
            sr_Settings.BottomYDirectionRebarSpacingSettings = textBox_BottomYDirectionRebarSpacing.Text;
            sr_Settings.TopXDirectionRebarSpacingSettings = textBox_TopXDirectionRebarSpacing.Text;
            sr_Settings.TopYDirectionRebarSpacingSettings = textBox_TopYDirectionRebarSpacing.Text;

            sr_Settings.PerimeterFramingDiamSettings = textBox_PerimeterFramingDiam.Text;
            sr_Settings.PerimeterFramingOverlapingSettings = textBox_PerimeterFramingOverlaping.Text;
            sr_Settings.PerimeterFramingEndCoverLayerSettings = textBox_PerimeterFramingEndCoverLayer.Text;
            sr_Settings.PerimeterFramingStepSettings = textBox_PerimeterFramingStep.Text;

            sr_Settings.mySelectionBottomXDirectionRebarTapeSettings = mySelectionBottomXDirectionRebarTape.Name;
            sr_Settings.mySelectionBottomYDirectionRebarTapeSettings = mySelectionBottomYDirectionRebarTape.Name;
            sr_Settings.mySelectionTopXDirectionRebarTapeSettings = mySelectionTopXDirectionRebarTape.Name;
            sr_Settings.mySelectionTopYDirectionRebarTapeSettings = mySelectionTopYDirectionRebarTape.Name;
            sr_Settings.mySelectionRebarCoverTypeForTopSettings = mySelectionRebarCoverTypeForTop.Name;
            sr_Settings.mySelectionRebarCoverTypeForBottomSettings = mySelectionRebarCoverTypeForBottom.Name;
            sr_Settings.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox_BottomXDirectionRebarSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_BottomXDirectionRebarSpacing.Text, out BottomXDirectionRebarSpacing);
        }

        private void textBox_BottomYDirectionRebarSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_BottomYDirectionRebarSpacing.Text, out BottomYDirectionRebarSpacing);
        }

        private void textBox_TopXDirectionRebarSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_TopXDirectionRebarSpacing.Text, out TopXDirectionRebarSpacing);
        }

        private void textBox_TopYDirectionRebarSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_TopYDirectionRebarSpacing.Text, out TopYDirectionRebarSpacing);
        }

        private void textBox_PerimeterFramingDiam_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_PerimeterFramingDiam.Text, out PerimeterFramingDiam);
        }

        private void textBox_PerimeterFramingOverlaping_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_PerimeterFramingOverlaping.Text, out PerimeterFramingOverlaping);
        }

        private void textBox_PerimeterFramingEndCoverLayer_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_PerimeterFramingEndCoverLayer.Text, out PerimeterFramingEndCoverLayer);
        }

        private void textBox_PerimeterFramingStep_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_PerimeterFramingStep.Text, out PerimeterFramingStep);
        }
    }
}
