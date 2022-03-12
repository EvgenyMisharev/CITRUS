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
    public partial class CIT_04_1_1FormSquareColumnsReinforcementType3 : Form
    {
        public RebarBarType mySelectionFirstMainBarTape;
        public RebarBarType mySelectionSecondMainBarTape;
        public RebarBarType mySelectionFirstStirrupBarTape;
        public RebarBarType mySelectionSecondStirrupBarTape;
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

        FSCRT3_Settings fsсrt3_Settings = null;

        public CIT_04_1_1FormSquareColumnsReinforcementType3(List<RebarBarType> firstMainBarTapes
            , List<RebarBarType> secondMainBarTapes
            , List<RebarBarType> firstStirrupBarTapes
            , List<RebarBarType> secondStirrupBarTapes
            , List<RebarCoverType> rebarCoverTypes)
        {
            InitializeComponent();
            fsсrt3_Settings = FSCRT3_Settings.GetSettings();
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "FSCRT3_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                textBox_FloorThicknessAboveColumn.Text = fsсrt3_Settings.FloorThicknessSettings;
                textBox_RebarOutletsLength.Text = fsсrt3_Settings.RebarOutletsSettings;
                textBox_RebarSecondOutletsLength.Text = fsсrt3_Settings.RebarSecondOutletsSettings;
                textBox_FirstStirrupOffset.Text = fsсrt3_Settings.FirstStirrupOffsetSettings;
                textBox_IncreasedStirrupSpacing.Text = fsсrt3_Settings.IncreasedStirrupSpacingSettings;
                textBox_StandardStirrupSpacing.Text = fsсrt3_Settings.StandardStirrupSpacingSettings;
                textBox_StirrupIncreasedPlacementHeight.Text = fsсrt3_Settings.StirrupIncreasedPlacementHeightSettings;
                textBox_ColumnSectionOffset.Text = fsсrt3_Settings.ColumnSectionOffsetSettings;
                textBox_DeepeningBars.Text = fsсrt3_Settings.DeepeningBarsSizeSettings;
            }

            List<RebarBarType> firstMainBarTapesListForComboBox = firstMainBarTapes;
            comboBox_FirstMainBarTapes.DataSource = firstMainBarTapesListForComboBox;
            comboBox_FirstMainBarTapes.DisplayMember = "Name";
            comboBox_FirstMainBarTapes.SelectedItem = firstMainBarTapesListForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt3_Settings.mySelectionFirstMainBarTapeSettings);

            List<RebarBarType> secondMainBarTapesListForComboBox = secondMainBarTapes;
            comboBox_SecondMainBarTapes.DataSource = secondMainBarTapesListForComboBox;
            comboBox_SecondMainBarTapes.DisplayMember = "Name";
            comboBox_SecondMainBarTapes.SelectedItem = secondMainBarTapesListForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt3_Settings.mySelectionSecondMainBarTapeSettings);

            List<RebarBarType> firstStirrupBarTapesForComboBox = firstStirrupBarTapes;
            comboBox_StirrupBarTapes.DataSource = firstStirrupBarTapesForComboBox;
            comboBox_StirrupBarTapes.DisplayMember = "Name";
            comboBox_StirrupBarTapes.SelectedItem = firstStirrupBarTapesForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt3_Settings.mySelectionFirstStirrupBarTapeSettings);

            List<RebarBarType> secondStirrupBarTapesForComboBox = secondStirrupBarTapes;
            comboBox_SecondStirrupBarTapes.DataSource = secondStirrupBarTapesForComboBox;
            comboBox_SecondStirrupBarTapes.DisplayMember = "Name";
            comboBox_SecondStirrupBarTapes.SelectedItem = secondStirrupBarTapesForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt3_Settings.mySelectionSecondStirrupBarTapeSettings);

            List<RebarCoverType> rebarCoverTypesListForComboBox = rebarCoverTypes;
            comboBox_RebarCoverTypes.DataSource = rebarCoverTypesListForComboBox;
            comboBox_RebarCoverTypes.DisplayMember = "Name";
            comboBox_RebarCoverTypes.SelectedItem = rebarCoverTypesListForComboBox.FirstOrDefault(rbt => rbt.Name == fsсrt3_Settings.mySelectionRebarCoverTypeSettings);
        }

        private void button1_Ok_Click(object sender, EventArgs e)
        {
            mySelectionFirstMainBarTape = comboBox_FirstMainBarTapes.SelectedItem as RebarBarType;
            mySelectionSecondMainBarTape = comboBox_SecondMainBarTapes.SelectedItem as RebarBarType;
            mySelectionFirstStirrupBarTape = comboBox_StirrupBarTapes.SelectedItem as RebarBarType;
            mySelectionSecondStirrupBarTape = comboBox_SecondStirrupBarTapes.SelectedItem as RebarBarType;
            mySelectionRebarCoverType = comboBox_RebarCoverTypes.SelectedItem as RebarCoverType;

            CheckedRebarOutletsButtonName = groupBox_RebarOutlets.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            TransitionToOverlap = checkBox_TransitionToOverlap.Checked;
            DeepeningBars = checkBox_DeepeningBars.Checked;
            BendIntoASlab = checkBox_BendIntoASlab.Checked;

            fsсrt3_Settings.FloorThicknessSettings = textBox_FloorThicknessAboveColumn.Text;
            fsсrt3_Settings.RebarOutletsSettings = textBox_RebarOutletsLength.Text;
            fsсrt3_Settings.RebarSecondOutletsSettings = textBox_RebarSecondOutletsLength.Text;
            fsсrt3_Settings.FirstStirrupOffsetSettings = textBox_FirstStirrupOffset.Text;
            fsсrt3_Settings.IncreasedStirrupSpacingSettings = textBox_IncreasedStirrupSpacing.Text;
            fsсrt3_Settings.StandardStirrupSpacingSettings = textBox_StandardStirrupSpacing.Text;
            fsсrt3_Settings.StirrupIncreasedPlacementHeightSettings = textBox_StirrupIncreasedPlacementHeight.Text;
            fsсrt3_Settings.ColumnSectionOffsetSettings = textBox_ColumnSectionOffset.Text;
            fsсrt3_Settings.DeepeningBarsSizeSettings = textBox_DeepeningBars.Text;

            fsсrt3_Settings.mySelectionFirstMainBarTapeSettings = mySelectionFirstMainBarTape.Name;
            fsсrt3_Settings.mySelectionSecondMainBarTapeSettings = mySelectionSecondMainBarTape.Name;
            fsсrt3_Settings.mySelectionFirstStirrupBarTapeSettings = mySelectionFirstStirrupBarTape.Name;
            fsсrt3_Settings.mySelectionSecondStirrupBarTapeSettings = mySelectionSecondStirrupBarTape.Name;
            fsсrt3_Settings.mySelectionRebarCoverTypeSettings = mySelectionRebarCoverType.Name;
            fsсrt3_Settings.Save();

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
