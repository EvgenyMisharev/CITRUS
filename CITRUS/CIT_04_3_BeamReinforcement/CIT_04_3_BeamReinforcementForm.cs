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

namespace CITRUS.CIT_04_3_BeamReinforcement
{
    public partial class CIT_04_3_BeamReinforcementForm : Form
    {
        public RebarBarType mySelectionMainBarT1;
        public RebarBarType mySelectionMainBarT2;
        public RebarBarType mySelectionMainBarT3;
        public RebarBarType mySelectionStirrupT1;
        public RebarBarType mySelectionStirrupC1;

        public RebarCoverType RebarTopCoverLayer;
        public RebarCoverType RebarBottomCoverLayer;
        public RebarCoverType RebarLRCoverLayer;

        public double ExtensionLeftLenghtL1;
        public double ExtensionLeftLenghtL2;
        public double ExtensionRightLenghtR1;
        public double ExtensionRightLenghtR2;

        public double DeepeningIntoTheStructureL1;
        public double DeepeningIntoTheStructureL2;
        public double DeepeningIntoTheStructureR1;
        public double DeepeningIntoTheStructureR2;

        public double StirrupIndentL1;
        public double StirrupStepL1;
        public double StirrupIndentR1;
        public double StirrupStepR1;
        public double StirrupStepC1;

        public double ExtensionAddBarL2;
        public double ExtensionAddBarR2;

        public int NumberOfBarsTopFaces = 0;
        public int NumberOfBarsBottomFaces = 0;

        public bool AddBarL2;
        public bool AddBarR2;

        BR_Settings br_Settings = null;

        public CIT_04_3_BeamReinforcementForm(List<RebarBarType> mainBarT1
            , List<RebarBarType> mainBarT2
            , List<RebarBarType> mainBarT3
            , List<RebarBarType> stirrupT1
            , List<RebarBarType> stirrupT2
            , List<RebarCoverType> topCoverLayerList
            , List<RebarCoverType> bottomCoverLayerList
            , List<RebarCoverType> LRCoverLayerList)
        {
            InitializeComponent();
            br_Settings = BR_Settings.GetSettings();
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "BR_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                textBox_ExtensionLeftLenghtL1.Text = br_Settings.ExtensionLeftLenghtL1Settings;
                textBox_ExtensionLeftLenghtL2.Text = br_Settings.ExtensionLeftLenghtL2Settings;
                textBox_ExtensionRightLenghtR1.Text = br_Settings.ExtensionRightLenghtR1Settings;
                textBox_ExtensionRightLenghtR2.Text = br_Settings.ExtensionRightLenghtR2Settings;

                textBox_DeepeningIntoTheStructureL1.Text = br_Settings.DeepeningIntoTheStructureL1Settings;
                textBox_DeepeningIntoTheStructureL2.Text = br_Settings.DeepeningIntoTheStructureL2Settings;
                textBox_DeepeningIntoTheStructureR1.Text = br_Settings.DeepeningIntoTheStructureR1Settings;
                textBox_DeepeningIntoTheStructureR2.Text = br_Settings.DeepeningIntoTheStructureR2Settings;

                textBox_StirrupIndentL1.Text = br_Settings.StirrupIndentL1Settings;
                textBox_StirrupStepL1.Text = br_Settings.StirrupStepL1Settings;
                textBox_StirrupIndentR1.Text = br_Settings.StirrupIndentR1Settings;
                textBox_StirrupStepR1.Text = br_Settings.StirrupStepR1Settings;
                textBox_StirrupStepC1.Text = br_Settings.StirrupStepC1Settings;

                textBox_ExtensionAddBarL2.Text = br_Settings.ExtensionAddBarL2Settings;
                textBox_ExtensionAddBarR2.Text = br_Settings.ExtensionAddBarR2Settings;

                textBox_NumberOfBarsTopFaces.Text = br_Settings.NumberOfBarsTopFacesSettings;
                textBox_NumberOfBarsBottomFaces.Text = br_Settings.NumberOfBarsBottomFacesSettings;
            }

            List<RebarBarType> mainBarT1ListForComboBox = mainBarT1;
            comboBox_MainBarT1.DataSource = mainBarT1ListForComboBox;
            comboBox_MainBarT1.DisplayMember = "Name";
            comboBox_MainBarT1.SelectedItem = mainBarT1ListForComboBox.FirstOrDefault(rbt => rbt.Name == br_Settings.mySelectionMainBarT1Settings);

            List<RebarBarType> mainBarT2ListForComboBox = mainBarT2;
            comboBox_MainBarT2.DataSource = mainBarT2ListForComboBox;
            comboBox_MainBarT2.DisplayMember = "Name";
            comboBox_MainBarT2.SelectedItem = mainBarT2ListForComboBox.FirstOrDefault(rbt => rbt.Name == br_Settings.mySelectionMainBarT2Settings);

            List<RebarBarType> mainBarT3ListForComboBox = mainBarT3;
            comboBox_MainBarT3.DataSource = mainBarT3ListForComboBox;
            comboBox_MainBarT3.DisplayMember = "Name";
            comboBox_MainBarT3.SelectedItem = mainBarT3ListForComboBox.FirstOrDefault(rbt => rbt.Name == br_Settings.mySelectionMainBarT3Settings);

            List<RebarBarType> stirrupT1ListForComboBox = stirrupT1;
            comboBox_StirrupT1.DataSource = stirrupT1ListForComboBox;
            comboBox_StirrupT1.DisplayMember = "Name";
            comboBox_StirrupT1.SelectedItem = stirrupT1ListForComboBox.FirstOrDefault(rbt => rbt.Name == br_Settings.mySelectionStirrupT1Settings);

            List<RebarBarType> stirrupT2ListForComboBox = stirrupT2;
            comboBox_StirrupC1.DataSource = stirrupT2ListForComboBox;
            comboBox_StirrupC1.DisplayMember = "Name";
            comboBox_StirrupC1.SelectedItem = stirrupT2ListForComboBox.FirstOrDefault(rbt => rbt.Name == br_Settings.mySelectionStirrupC1Settings);

            List<RebarCoverType> topCoverLayerListForComboBox = topCoverLayerList;
            comboBox_RebarTopCoverLayer.DataSource = topCoverLayerListForComboBox;
            comboBox_RebarTopCoverLayer.DisplayMember = "Name";
            comboBox_RebarTopCoverLayer.SelectedItem = topCoverLayerListForComboBox.FirstOrDefault(rbt => rbt.Name == br_Settings.RebarTopCoverLayerSettings);

            List<RebarCoverType> bottomCoverLayerListForComboBox = bottomCoverLayerList;
            comboBox_RebarBottomCoverLayer.DataSource = bottomCoverLayerListForComboBox;
            comboBox_RebarBottomCoverLayer.DisplayMember = "Name";
            comboBox_RebarBottomCoverLayer.SelectedItem = bottomCoverLayerListForComboBox.FirstOrDefault(rbt => rbt.Name == br_Settings.RebarBottomCoverLayerSettings);

            List<RebarCoverType> LRCoverLayerListForComboBox = LRCoverLayerList;
            comboBox_RebarLRCoverLayer.DataSource = LRCoverLayerListForComboBox;
            comboBox_RebarLRCoverLayer.DisplayMember = "Name";
            comboBox_RebarLRCoverLayer.SelectedItem = LRCoverLayerListForComboBox.FirstOrDefault(rbt => rbt.Name == br_Settings.RebarLRCoverLayerSettings);
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            mySelectionMainBarT1 = comboBox_MainBarT1.SelectedItem as RebarBarType;
            mySelectionMainBarT2 = comboBox_MainBarT2.SelectedItem as RebarBarType;
            mySelectionMainBarT3 = comboBox_MainBarT3.SelectedItem as RebarBarType;
            mySelectionStirrupT1 = comboBox_StirrupT1.SelectedItem as RebarBarType;
            mySelectionStirrupC1 = comboBox_StirrupC1.SelectedItem as RebarBarType;
            RebarTopCoverLayer = comboBox_RebarTopCoverLayer.SelectedItem as RebarCoverType;
            RebarBottomCoverLayer = comboBox_RebarBottomCoverLayer.SelectedItem as RebarCoverType;
            RebarLRCoverLayer = comboBox_RebarLRCoverLayer.SelectedItem as RebarCoverType;

            AddBarL2 = checkBox_AddBarL2.Checked;
            AddBarR2 = checkBox_AddBarR2.Checked;

            br_Settings.ExtensionLeftLenghtL1Settings = textBox_ExtensionLeftLenghtL1.Text;
            br_Settings.ExtensionLeftLenghtL2Settings = textBox_ExtensionLeftLenghtL2.Text;
            br_Settings.ExtensionRightLenghtR1Settings = textBox_ExtensionRightLenghtR1.Text;
            br_Settings.ExtensionRightLenghtR2Settings = textBox_ExtensionRightLenghtR2.Text;

            br_Settings.DeepeningIntoTheStructureL1Settings = textBox_DeepeningIntoTheStructureL1.Text;
            br_Settings.DeepeningIntoTheStructureL2Settings = textBox_DeepeningIntoTheStructureL2.Text;
            br_Settings.DeepeningIntoTheStructureR1Settings = textBox_DeepeningIntoTheStructureR1.Text;
            br_Settings.DeepeningIntoTheStructureR2Settings = textBox_DeepeningIntoTheStructureR2.Text;

            br_Settings.StirrupIndentL1Settings = textBox_StirrupIndentL1.Text;
            br_Settings.StirrupStepL1Settings = textBox_StirrupStepL1.Text;
            br_Settings.StirrupIndentR1Settings = textBox_StirrupIndentR1.Text;
            br_Settings.StirrupStepR1Settings = textBox_StirrupStepR1.Text;
            br_Settings.StirrupStepC1Settings = textBox_StirrupStepC1.Text;

            br_Settings.ExtensionAddBarL2Settings = textBox_ExtensionAddBarL2.Text;
            br_Settings.ExtensionAddBarR2Settings = textBox_ExtensionAddBarR2.Text;

            br_Settings.NumberOfBarsTopFacesSettings = textBox_NumberOfBarsTopFaces.Text;
            br_Settings.NumberOfBarsBottomFacesSettings = textBox_NumberOfBarsBottomFaces.Text;

            br_Settings.mySelectionMainBarT1Settings = mySelectionMainBarT1.Name;
            br_Settings.mySelectionMainBarT2Settings = mySelectionMainBarT2.Name;
            br_Settings.mySelectionMainBarT3Settings = mySelectionMainBarT3.Name;
            br_Settings.mySelectionStirrupT1Settings = mySelectionStirrupT1.Name;
            br_Settings.mySelectionStirrupC1Settings = mySelectionStirrupC1.Name;

            br_Settings.RebarTopCoverLayerSettings = RebarTopCoverLayer.Name;
            br_Settings.RebarBottomCoverLayerSettings = RebarBottomCoverLayer.Name;
            br_Settings.RebarLRCoverLayerSettings = RebarLRCoverLayer.Name;
            br_Settings.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox_ExtensionLeftLenghtL1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ExtensionLeftLenghtL1.Text, out ExtensionLeftLenghtL1);
        }
        private void textBox_ExtensionLeftLenghtL2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ExtensionLeftLenghtL2.Text, out ExtensionLeftLenghtL2);
        }
        private void textBox_ExtensionRightLenghtR1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ExtensionRightLenghtR1.Text, out ExtensionRightLenghtR1);
        }
        private void textBox_ExtensionRightLenghtR2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ExtensionRightLenghtR2.Text, out ExtensionRightLenghtR2);
        }
        private void textBox_NumberOfBarsTopFaces_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(textBox_NumberOfBarsTopFaces.Text, out NumberOfBarsTopFaces);
        }
        private void textBox_NumberOfBarsBottomFaces_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(textBox_NumberOfBarsBottomFaces.Text, out NumberOfBarsBottomFaces);
        }
        private void textBox_DeepeningIntoTheStructureL1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_DeepeningIntoTheStructureL1.Text, out DeepeningIntoTheStructureL1);
        }
        private void textBox_DeepeningIntoTheStructureL2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_DeepeningIntoTheStructureL2.Text, out DeepeningIntoTheStructureL2);
        }
        private void textBox_DeepeningIntoTheStructureR1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_DeepeningIntoTheStructureR1.Text, out DeepeningIntoTheStructureR1);
        }
        private void textBox_DeepeningIntoTheStructureR2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_DeepeningIntoTheStructureR2.Text, out DeepeningIntoTheStructureR2);
        }
        private void textBox_StirrupIndentL1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StirrupIndentL1.Text, out StirrupIndentL1);
        }
        private void textBox_StirrupStepL1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StirrupStepL1.Text, out StirrupStepL1);
        }
        private void textBox_StirrupIndentR1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StirrupIndentR1.Text, out StirrupIndentR1);
        }
        private void textBox_StirrupStepR1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StirrupStepR1.Text, out StirrupStepR1);
        }
        private void textBox_StirrupStepC1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StirrupStepC1.Text, out StirrupStepC1);
        }
        private void textBox_ExtensionAddBarL2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ExtensionAddBarL2.Text, out ExtensionAddBarL2);
        }
        private void textBox_ExtensionAddBarR2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ExtensionAddBarR2.Text, out ExtensionAddBarR2);
        }
    }
}
