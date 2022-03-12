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

namespace CITRUS.CIT_04_2_RectangularColumnsReinforcement
{
    public partial class CIT_04_2_RectangularColumnsReinforcementForm : Form
    {
        public RebarBarType mySelectionMainBarTapeOne;
        public RebarBarType mySelectionMainBarTapeTwo;
        public RebarBarType mySelectionMainBarTapeThree;
        public RebarBarType mySelectionStirrupBarTape;
        public RebarBarType mySelectionPinBarTape;
        public RebarCoverType mySelectionRebarCoverType;

        public string CheckedRebarOutletsButtonName = "";
        public string CheckedRebarStrappingTypeButtonName = "";

        public int NumberOfBarsLRFaces = 0;
        public int NumberOfBarsTBFaces = 0;
        public double RebarOutletsLengthLong = 0;
        public double RebarOutletsLengthShort = 0;
        public double FloorThicknessAboveColumn = 0;
        public double StandardStirrupStep = 0;
        public double IncreasedStirrupStep = 0;
        public double FirstStirrupOffset = 0;
        public double StirrupIncreasedPlacementHeight = 0;
        public double ColumnSectionOffset;
        public double DeepeningBarsSize;

        public bool TransitionToOverlap;
        public bool СhangeColumnSection;
        public bool DeepeningBars;
        public bool BendIntoASlab;

        RCRF_Settings rcrf_Settings = null;

        public CIT_04_2_RectangularColumnsReinforcementForm(List<RebarBarType> mainBarTapesOne
            , List<RebarBarType> mainBarTapesTwo
            , List<RebarBarType> mainBarTapesThree
            , List<RebarBarType> stirrupBarTapes
            , List<RebarBarType> pinBarTapes
            , List<RebarCoverType> rebarCoverTypes)
        {
            InitializeComponent();
            rcrf_Settings = RCRF_Settings.GetSettings();
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "RCRF_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                textBox_NumberOfBarsLRFaces.Text = rcrf_Settings.NumberOfBarsLRFacesSettings;
                textBox_NumberOfBarsTBFaces.Text = rcrf_Settings.NumberOfBarsTBFacesSettings;
                textBox_RebarOutletsLengthLong.Text = rcrf_Settings.RebarOutletsLengthLongSettings;
                textBox_RebarOutletsLengthShort.Text = rcrf_Settings.RebarOutletsLengthShortSettings;
                textBox_FloorThicknessAboveColumn.Text = rcrf_Settings.FloorThicknessAboveColumnSettings;
                textBox_StandardStirrupStep.Text = rcrf_Settings.StandardStirrupStepSettings;
                textBox_IncreasedStirrupStep.Text = rcrf_Settings.IncreasedStirrupStepSettings;
                textBox_FirstStirrupOffset.Text = rcrf_Settings.FirstStirrupOffsetSettings;
                textBox_StirrupIncreasedPlacementHeight.Text = rcrf_Settings.StirrupIncreasedPlacementHeightSettings;
                textBox_ColumnSectionOffset.Text = rcrf_Settings.ColumnSectionOffsetSettings;
                textBox_DeepeningBars.Text = rcrf_Settings.DeepeningBarsSizeSettings;
            }

            List<RebarBarType> mainBarTapesOneListForComboBox = mainBarTapesOne;
            comboBox_MainBarTapesOne.DataSource = mainBarTapesOneListForComboBox;
            comboBox_MainBarTapesOne.DisplayMember = "Name";
            comboBox_MainBarTapesOne.SelectedItem = mainBarTapesOneListForComboBox.FirstOrDefault(rbt => rbt.Name == rcrf_Settings.mySelectionMainBarTapeOneSettings);

            List<RebarBarType> mainBarTapesTwoListForComboBox = mainBarTapesTwo;
            comboBox_MainBarTapesTwo.DataSource = mainBarTapesTwoListForComboBox;
            comboBox_MainBarTapesTwo.DisplayMember = "Name";
            comboBox_MainBarTapesTwo.SelectedItem = mainBarTapesTwoListForComboBox.FirstOrDefault(rbt => rbt.Name == rcrf_Settings.mySelectionMainBarTapeTwoSettings);

            List<RebarBarType> mainBarTapesThreeListForComboBox = mainBarTapesThree;
            comboBox_MainBarTapesThree.DataSource = mainBarTapesThreeListForComboBox;
            comboBox_MainBarTapesThree.DisplayMember = "Name";
            comboBox_MainBarTapesThree.SelectedItem = mainBarTapesThreeListForComboBox.FirstOrDefault(rbt => rbt.Name == rcrf_Settings.mySelectionMainBarTapeThreeSettings);

            List<RebarBarType> stirrupBarTapesForComboBox = stirrupBarTapes;
            comboBox_StirrupBarTapes.DataSource = stirrupBarTapesForComboBox;
            comboBox_StirrupBarTapes.DisplayMember = "Name";
            comboBox_StirrupBarTapes.SelectedItem = stirrupBarTapesForComboBox.FirstOrDefault(rbt => rbt.Name == rcrf_Settings.mySelectionStirrupBarTapeSettings);

            List<RebarBarType> pinBarTapesForComboBox = pinBarTapes;
            comboBox_PinBarTapes.DataSource = pinBarTapesForComboBox;
            comboBox_PinBarTapes.DisplayMember = "Name";
            comboBox_PinBarTapes.SelectedItem = pinBarTapesForComboBox.FirstOrDefault(rbt => rbt.Name == rcrf_Settings.mySelectionPinBarTapeSettings);

            List<RebarCoverType> rebarCoverTypesListForComboBox = rebarCoverTypes;
            comboBox_RebarCoverTypes.DataSource = rebarCoverTypesListForComboBox;
            comboBox_RebarCoverTypes.DisplayMember = "Name";
            comboBox_RebarCoverTypes.SelectedItem = rebarCoverTypesListForComboBox.FirstOrDefault(rbt => rbt.Name == rcrf_Settings.mySelectionRebarCoverTypeSettings);
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            mySelectionMainBarTapeOne = comboBox_MainBarTapesOne.SelectedItem as RebarBarType;
            mySelectionMainBarTapeTwo = comboBox_MainBarTapesTwo.SelectedItem as RebarBarType;
            mySelectionMainBarTapeThree = comboBox_MainBarTapesThree.SelectedItem as RebarBarType;
            mySelectionStirrupBarTape = comboBox_StirrupBarTapes.SelectedItem as RebarBarType;
            mySelectionPinBarTape = comboBox_PinBarTapes.SelectedItem as RebarBarType;
            mySelectionRebarCoverType = comboBox_RebarCoverTypes.SelectedItem as RebarCoverType;

            CheckedRebarOutletsButtonName = groupBox_RebarOutlets.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            CheckedRebarStrappingTypeButtonName = groupBox_StrappingType.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            TransitionToOverlap = checkBox_TransitionToOverlap.Checked;
            СhangeColumnSection = checkBox_СhangeSection.Checked;
            DeepeningBars = checkBox_DeepeningBars.Checked;
            BendIntoASlab = checkBox_BendIntoASlab.Checked;

            rcrf_Settings.NumberOfBarsLRFacesSettings = textBox_NumberOfBarsLRFaces.Text;
            rcrf_Settings.NumberOfBarsTBFacesSettings = textBox_NumberOfBarsTBFaces.Text;
            rcrf_Settings.RebarOutletsLengthLongSettings = textBox_RebarOutletsLengthLong.Text;
            rcrf_Settings.RebarOutletsLengthShortSettings = textBox_RebarOutletsLengthShort.Text;
            rcrf_Settings.FloorThicknessAboveColumnSettings = textBox_FloorThicknessAboveColumn.Text;
            rcrf_Settings.StandardStirrupStepSettings = textBox_StandardStirrupStep.Text;
            rcrf_Settings.IncreasedStirrupStepSettings = textBox_IncreasedStirrupStep.Text;
            rcrf_Settings.FirstStirrupOffsetSettings = textBox_FirstStirrupOffset.Text;
            rcrf_Settings.StirrupIncreasedPlacementHeightSettings = textBox_StirrupIncreasedPlacementHeight.Text;
            rcrf_Settings.ColumnSectionOffsetSettings = textBox_ColumnSectionOffset.Text;
            rcrf_Settings.DeepeningBarsSizeSettings = textBox_DeepeningBars.Text;


            rcrf_Settings.mySelectionMainBarTapeOneSettings = mySelectionMainBarTapeOne.Name;
            rcrf_Settings.mySelectionMainBarTapeTwoSettings = mySelectionMainBarTapeTwo.Name;
            rcrf_Settings.mySelectionMainBarTapeThreeSettings = mySelectionMainBarTapeThree.Name;
            rcrf_Settings.mySelectionStirrupBarTapeSettings = mySelectionStirrupBarTape.Name;
            rcrf_Settings.mySelectionPinBarTapeSettings = mySelectionPinBarTape.Name;
            rcrf_Settings.mySelectionRebarCoverTypeSettings = mySelectionRebarCoverType.Name;

            rcrf_Settings.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox_NumberOfBarsLRFaces_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(textBox_NumberOfBarsLRFaces.Text, out NumberOfBarsLRFaces);
        }
        private void textBox_NumberOfBarsTBFaces_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(textBox_NumberOfBarsTBFaces.Text, out NumberOfBarsTBFaces);
        }
        private void textBox_RebarOutletsLengthLong_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_RebarOutletsLengthLong.Text, out RebarOutletsLengthLong);
        }
        private void textBox_RebarOutletsLengthShort_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_RebarOutletsLengthShort.Text, out RebarOutletsLengthShort);
        }
        private void textBox_FloorThicknessAboveColumn_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_FloorThicknessAboveColumn.Text, out FloorThicknessAboveColumn);
        }
        private void textBox_StandardStirrupStep_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StandardStirrupStep.Text, out StandardStirrupStep);
        }
        private void textBox_IncreasedStirrupStep_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_IncreasedStirrupStep.Text, out IncreasedStirrupStep);
        }
        private void textBox_FirstStirrupOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_FirstStirrupOffset.Text, out FirstStirrupOffset);
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
    }
}
