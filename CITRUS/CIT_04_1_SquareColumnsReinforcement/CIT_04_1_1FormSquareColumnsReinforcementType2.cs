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
using CITRUS.Properties;
using System.IO;

namespace CITRUS.CIT_04_1_SquareColumnsReinforcement
{
    public partial class CIT_04_1_1FormSquareColumnsReinforcementType2 : Form
    {
        public RebarBarType mySelectionFirstMainBarTape;
        public RebarBarType mySelectionSecondMainBarTape;
        public RebarBarType mySelectionStirrupBarTape;
        public RebarCoverType mySelectionRebarCoverType;

        public double FloorThickness;
        public double RebarOutlets;
        public double RebarSecondOutlets;
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

        FSCRT2_Settings fsсrt2_Settings = null;

        public CIT_04_1_1FormSquareColumnsReinforcementType2(List<RebarBarType> firstMainBarTapes, List<RebarBarType> secondMainBarTapes, List<RebarBarType> stirrupBarTapes, List<RebarCoverType> rebarCoverTypes)
        {
            InitializeComponent();
            fsсrt2_Settings = FSCRT2_Settings.GetSettings();
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "FSCRT2_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                textBox_FloorThicknessAboveColumn.Text = fsсrt2_Settings.FloorThicknessSettings;
                textBox_RebarOutletsLength.Text = fsсrt2_Settings.RebarOutletsSettings;
                textBox_RebarSecondOutletsLength.Text = fsсrt2_Settings.RebarSecondOutletsSettings;
                textBox_FirstStirrupOffset.Text = fsсrt2_Settings.FirstStirrupOffsetSettings;
                textBox_IncreasedStirrupSpacing.Text = fsсrt2_Settings.IncreasedStirrupSpacingSettings;
                textBox_StandardStirrupSpacing.Text = fsсrt2_Settings.StandardStirrupSpacingSettings;
                textBox_StirrupIncreasedPlacementHeight.Text = fsсrt2_Settings.StirrupIncreasedPlacementHeightSettings;
                textBox_ColumnSectionOffset.Text = fsсrt2_Settings.ColumnSectionOffsetSettings;
                textBox_DeepeningBars.Text = fsсrt2_Settings.DeepeningBarsSizeSettings;
            }

            List<RebarBarType> firstMainBarTapesListForComboBox = firstMainBarTapes;
            comboBox_FirstMainBarTapes.DataSource = firstMainBarTapesListForComboBox;
            comboBox_FirstMainBarTapes.DisplayMember = "Name";
            comboBox_FirstMainBarTapes.SelectedItem = firstMainBarTapesListForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt2_Settings.mySelectionFirstMainBarTapeSettings);

            List<RebarBarType> secondMainBarTapesListForComboBox = secondMainBarTapes;
            comboBox_SecondMainBarTapes.DataSource = secondMainBarTapesListForComboBox;
            comboBox_SecondMainBarTapes.DisplayMember = "Name";
            comboBox_SecondMainBarTapes.SelectedItem = secondMainBarTapesListForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt2_Settings.mySelectionSecondMainBarTapeSettings);

            List<RebarBarType> stirrupBarTapesForComboBox = stirrupBarTapes;
            comboBox_StirrupBarTapes.DataSource = stirrupBarTapesForComboBox;
            comboBox_StirrupBarTapes.DisplayMember = "Name";
            comboBox_StirrupBarTapes.SelectedItem = stirrupBarTapesForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt2_Settings.mySelectionStirrupBarTapeSettings);

            List<RebarCoverType> rebarCoverTypesListForComboBox = rebarCoverTypes;
            comboBox_RebarCoverTypes.DataSource = rebarCoverTypesListForComboBox;
            comboBox_RebarCoverTypes.DisplayMember = "Name";
            comboBox_RebarCoverTypes.SelectedItem = rebarCoverTypesListForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt2_Settings.mySelectionRebarCoverTypeSettings);
        }

        private void button1_Ok_Click(object sender, EventArgs e)
        {
            mySelectionFirstMainBarTape = comboBox_FirstMainBarTapes.SelectedItem as RebarBarType;
            mySelectionSecondMainBarTape = comboBox_SecondMainBarTapes.SelectedItem as RebarBarType;
            mySelectionStirrupBarTape = comboBox_StirrupBarTapes.SelectedItem as RebarBarType;
            mySelectionRebarCoverType = comboBox_RebarCoverTypes.SelectedItem as RebarCoverType;

            CheckedRebarOutletsButtonName = groupBox_RebarOutlets.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            TransitionToOverlap = checkBox_TransitionToOverlap.Checked;
            DeepeningBars = checkBox_DeepeningBars.Checked;
            BendIntoASlab = checkBox_BendIntoASlab.Checked;

            fsсrt2_Settings.FloorThicknessSettings = textBox_FloorThicknessAboveColumn.Text;
            fsсrt2_Settings.RebarOutletsSettings = textBox_RebarOutletsLength.Text;
            fsсrt2_Settings.RebarSecondOutletsSettings = textBox_RebarSecondOutletsLength.Text;
            fsсrt2_Settings.FirstStirrupOffsetSettings = textBox_FirstStirrupOffset.Text;
            fsсrt2_Settings.IncreasedStirrupSpacingSettings = textBox_IncreasedStirrupSpacing.Text;
            fsсrt2_Settings.StandardStirrupSpacingSettings = textBox_StandardStirrupSpacing.Text;
            fsсrt2_Settings.StirrupIncreasedPlacementHeightSettings = textBox_StirrupIncreasedPlacementHeight.Text;
            fsсrt2_Settings.ColumnSectionOffsetSettings = textBox_ColumnSectionOffset.Text;
            fsсrt2_Settings.DeepeningBarsSizeSettings = textBox_DeepeningBars.Text;

            fsсrt2_Settings.mySelectionFirstMainBarTapeSettings = mySelectionFirstMainBarTape.Name;
            fsсrt2_Settings.mySelectionSecondMainBarTapeSettings = mySelectionSecondMainBarTape.Name;
            fsсrt2_Settings.mySelectionStirrupBarTapeSettings = mySelectionStirrupBarTape.Name;
            fsсrt2_Settings.mySelectionRebarCoverTypeSettings = mySelectionRebarCoverType.Name;
            fsсrt2_Settings.Save();

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
        private void textBox_RebarSecondOutletsLength_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_RebarSecondOutletsLength.Text, out RebarSecondOutlets);
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
