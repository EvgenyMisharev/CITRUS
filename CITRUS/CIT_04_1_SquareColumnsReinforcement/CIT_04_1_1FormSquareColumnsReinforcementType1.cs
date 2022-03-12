using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CITRUS.Properties;
using System.IO;

namespace CITRUS.CIT_04_1_SquareColumnsReinforcement
{
    public partial class CIT_04_1_1FormSquareColumnsReinforcementType1 : Form
    {
        public RebarBarType mySelectionMainBarTape;
        public RebarBarType mySelectionStirrupBarTape;
        public RebarCoverType mySelectionRebarCoverType;

        public double FloorThickness;
        public double RebarOutlets;
        public double FirstStirrupOffset;
        public double IncreasedStirrupSpacing;
        public double StandardStirrupSpacing;
        public double StirrupIncreasedPlacementHeight;
        public double ColumnSectionOffset;
        public double DeepeningBarsSize;

        public string CheckedRebarOutletsButtonName;

        public bool СhangeColumnSection;
        public bool TransitionToOverlap;
        public bool DeepeningBars;
        public bool BendIntoASlab;

        FSCRT1_Settings fsсrt1_Settings = null;

        public CIT_04_1_1FormSquareColumnsReinforcementType1(List<RebarBarType> mainBarTapes, List<RebarBarType> stirrupBarTapes, List<RebarCoverType> rebarCoverTypes)
        {
            InitializeComponent();
            fsсrt1_Settings = FSCRT1_Settings.GetSettings();
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "FSCRT1_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                textBox_FloorThicknessAboveColumn.Text = fsсrt1_Settings.FloorThicknessSettings.ToString();
                textBox_RebarOutletsLength.Text = fsсrt1_Settings.RebarOutletsSettings.ToString();
                textBox_FirstStirrupOffset.Text = fsсrt1_Settings.FirstStirrupOffsetSettings.ToString();
                textBox_IncreasedStirrupSpacing.Text = fsсrt1_Settings.IncreasedStirrupSpacingSettings.ToString();
                textBox_StandardStirrupSpacing.Text = fsсrt1_Settings.StandardStirrupSpacingSettings.ToString();
                textBox_StirrupIncreasedPlacementHeight.Text = fsсrt1_Settings.StirrupIncreasedPlacementHeightSettings.ToString();
                textBox_ColumnSectionOffset.Text = fsсrt1_Settings.ColumnSectionOffsetSettings.ToString();
                textBox_DeepeningBars.Text = fsсrt1_Settings.DeepeningBarsSizeSettings.ToString();
            }

            List<RebarBarType> mainBarTapesListForComboBox = mainBarTapes;
            comboBox_MainBarTapes.DataSource = mainBarTapesListForComboBox;
            comboBox_MainBarTapes.DisplayMember = "Name";
            comboBox_MainBarTapes.SelectedItem = mainBarTapesListForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt1_Settings.mySelectionMainBarTapeSettings);

            List<RebarBarType> stirrupBarTapesForComboBox = stirrupBarTapes;
            comboBox_StirrupBarTapes.DataSource = stirrupBarTapesForComboBox;
            comboBox_StirrupBarTapes.DisplayMember = "Name";
            comboBox_StirrupBarTapes.SelectedItem = stirrupBarTapesForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt1_Settings.mySelectionStirrupBarTapeSettings);

            List<RebarCoverType> rebarCoverTypesListForComboBox = rebarCoverTypes;
            comboBox_RebarCoverTypes.DataSource = rebarCoverTypesListForComboBox;
            comboBox_RebarCoverTypes.DisplayMember = "Name";
            comboBox_RebarCoverTypes.SelectedItem = rebarCoverTypesListForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt1_Settings.mySelectionRebarCoverTypeSettings);
        }

        private void button1_Ok_Click(object sender, EventArgs e)
        {
            mySelectionMainBarTape = comboBox_MainBarTapes.SelectedItem as RebarBarType;
            mySelectionStirrupBarTape = comboBox_StirrupBarTapes.SelectedItem as RebarBarType;
            mySelectionRebarCoverType = comboBox_RebarCoverTypes.SelectedItem as RebarCoverType;

            CheckedRebarOutletsButtonName = groupBox_RebarOutlets.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            TransitionToOverlap = checkBox_TransitionToOverlap.Checked;
            DeepeningBars = checkBox_DeepeningBars.Checked;
            BendIntoASlab = checkBox_BendIntoASlab.Checked;

            fsсrt1_Settings.FloorThicknessSettings = textBox_FloorThicknessAboveColumn.Text;
            fsсrt1_Settings.RebarOutletsSettings = textBox_RebarOutletsLength.Text;
            fsсrt1_Settings.FirstStirrupOffsetSettings = textBox_FirstStirrupOffset.Text;
            fsсrt1_Settings.IncreasedStirrupSpacingSettings = textBox_IncreasedStirrupSpacing.Text;
            fsсrt1_Settings.StandardStirrupSpacingSettings = textBox_StandardStirrupSpacing.Text;
            fsсrt1_Settings.StirrupIncreasedPlacementHeightSettings = textBox_StirrupIncreasedPlacementHeight.Text;
            fsсrt1_Settings.ColumnSectionOffsetSettings = textBox_ColumnSectionOffset.Text;
            fsсrt1_Settings.DeepeningBarsSizeSettings = textBox_DeepeningBars.Text;

            fsсrt1_Settings.mySelectionMainBarTapeSettings = mySelectionMainBarTape.Name;
            fsсrt1_Settings.mySelectionStirrupBarTapeSettings = mySelectionStirrupBarTape.Name;
            fsсrt1_Settings.mySelectionRebarCoverTypeSettings = mySelectionRebarCoverType.Name;
            fsсrt1_Settings.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox_FloorThicknessAboveColumn_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_FloorThicknessAboveColumn.Text, out FloorThickness);
        }
        private void textBox_RebarOutletsLength_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_RebarOutletsLength.Text, out RebarOutlets);
        }
        private void textBox_FirstStirrupOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_FirstStirrupOffset.Text, out FirstStirrupOffset);
        }
        private void textBox_IncreasedStirrupSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_IncreasedStirrupSpacing.Text, out IncreasedStirrupSpacing);
        }
        private void textBox_StandardStirrupSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StandardStirrupSpacing.Text, out StandardStirrupSpacing);
        }
        private void textBox_StirrupIncreasedPlacementHeight_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StirrupIncreasedPlacementHeight.Text, out StirrupIncreasedPlacementHeight);
        }
        private void textBox_ColumnSectionOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ColumnSectionOffset.Text, out ColumnSectionOffset);
        }
        private void textBox_DeepeningBars_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_DeepeningBars.Text, out DeepeningBarsSize);
        }
        private void checkBox_СhangeSection_CheckedChanged(object sender, EventArgs e)
        {
            СhangeColumnSection = checkBox_СhangeSection.Checked;
        }
    }
}
