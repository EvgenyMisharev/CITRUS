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

namespace CITRUS.CIT_04_5_StairFlightReinforcement
{
    public partial class CIT_04_5_StairFlightReinforcementForm : Form
    {
        public RebarBarType mySelectionStepRebarType;
        public RebarBarType mySelectionStaircaseRebarType;

        public double StepRebarCoverLayer;
        public double StepLength;
        public double StepHeight;
        public double StaircaseSlabThickness;
        public double StairCoverLayer;
        public double StepRebarStep;
        public double StaircaseRebarStep;
        public double TopExtensionStaircase;
        public double TopExtensionHeightStaircase;
        public double BottomExtensionHeightStaircase;
        public double BottomExtensionHeightStaircaseNodeA2;
        public string FirstBarMeshName = "";
        public string AdditionalBarMeshName_1 = "";
        public string AdditionalBarMeshName_2 = "";

        public string CheckedBottomConnectionNodeName;
        public string CheckedTopConnectionNodeName;

        SFR_Settings sfr_Settings = null;

        public CIT_04_5_StairFlightReinforcementForm(List<RebarBarType> stepRebarType, List<RebarBarType> staircaseRebarType)
        {
            InitializeComponent();
            sfr_Settings = SFR_Settings.GetSettings();
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SFR_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                textBox_StepRebarCoverLayer.Text = sfr_Settings.StepRebarCoverLayerSettings;
                textBox_StepLength.Text = sfr_Settings.StepLengthSettings;
                textBox_StepHeight.Text = sfr_Settings.StepHeightSettings;
                textBox_StaircaseSlabThickness.Text = sfr_Settings.StaircaseSlabThicknessSettings;
                textBox_StairCoverLayer.Text = sfr_Settings.StairCoverLayerSettings;
                textBox_StepRebarStep.Text = sfr_Settings.StepRebarStepSettings;
                textBox_StaircaseRebarStep.Text = sfr_Settings.StaircaseRebarStepSettings;
                textBox_TopExtensionStaircase.Text = sfr_Settings.TopExtensionStaircaseSettings;
                textBox_TopExtensionHeightStaircase.Text = sfr_Settings.TopExtensionHeightStaircaseSettings;
                textBox_BottomExtensionHeightStaircase.Text = sfr_Settings.BottomExtensionHeightStaircaseSettings;
                textBox_BottomExtensionHeightStaircaseNodeA2.Text = sfr_Settings.BottomExtensionHeightStaircaseNodeA2Settings;
                textBox_FirstBarMeshName.Text = sfr_Settings.FirstBarMeshNameSettings;
                textBox_AdditionalBarMeshName_1.Text = sfr_Settings.AdditionalBarMeshName_1Settings;
                textBox_AdditionalBarMeshName_2.Text = sfr_Settings.AdditionalBarMeshName_2Settings;

            }

            List<RebarBarType> stepRebarTypeListForComboBox = stepRebarType;
            comboBox_stepRebarType.DataSource = stepRebarTypeListForComboBox;
            comboBox_stepRebarType.DisplayMember = "Name";
            comboBox_stepRebarType.SelectedItem = stepRebarTypeListForComboBox.FirstOrDefault(rbt => rbt.Name == sfr_Settings.mySelectionStepRebarTypeSettings);

            List<RebarBarType> staircaseRebarTypeListForComboBox = staircaseRebarType;
            comboBox_staircaseRebarType.DataSource = staircaseRebarTypeListForComboBox;
            comboBox_staircaseRebarType.DisplayMember = "Name";
            comboBox_staircaseRebarType.SelectedItem = staircaseRebarTypeListForComboBox.FirstOrDefault(rbt => rbt.Name == sfr_Settings.mySelectionStaircaseRebarTypeSettings);
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            mySelectionStepRebarType = comboBox_stepRebarType.SelectedItem as RebarBarType;
            mySelectionStaircaseRebarType = comboBox_staircaseRebarType.SelectedItem as RebarBarType;

            CheckedBottomConnectionNodeName = groupBox_BottomConnectionNode.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            CheckedTopConnectionNodeName = groupBox_TopConnectionNode.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;

            sfr_Settings.StepRebarCoverLayerSettings = textBox_StepRebarCoverLayer.Text;
            sfr_Settings.StepLengthSettings = textBox_StepLength.Text;
            sfr_Settings.StepHeightSettings = textBox_StepHeight.Text;
            sfr_Settings.StaircaseSlabThicknessSettings = textBox_StaircaseSlabThickness.Text;
            sfr_Settings.StairCoverLayerSettings = textBox_StairCoverLayer.Text;
            sfr_Settings.StepRebarStepSettings = textBox_StepRebarStep.Text;
            sfr_Settings.StaircaseRebarStepSettings = textBox_StaircaseRebarStep.Text;
            sfr_Settings.TopExtensionStaircaseSettings = textBox_TopExtensionStaircase.Text;
            sfr_Settings.TopExtensionHeightStaircaseSettings = textBox_TopExtensionHeightStaircase.Text;
            sfr_Settings.BottomExtensionHeightStaircaseSettings = textBox_BottomExtensionHeightStaircase.Text;
            sfr_Settings.BottomExtensionHeightStaircaseNodeA2Settings = textBox_BottomExtensionHeightStaircaseNodeA2.Text;
            sfr_Settings.FirstBarMeshNameSettings = textBox_FirstBarMeshName.Text;
            sfr_Settings.AdditionalBarMeshName_1Settings = textBox_AdditionalBarMeshName_1.Text;
            sfr_Settings.AdditionalBarMeshName_2Settings = textBox_AdditionalBarMeshName_2.Text;

            sfr_Settings.mySelectionStepRebarTypeSettings = mySelectionStepRebarType.Name;
            sfr_Settings.mySelectionStaircaseRebarTypeSettings = mySelectionStaircaseRebarType.Name;
            sfr_Settings.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox_StepRebarCoverLayer_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StepRebarCoverLayer.Text, out StepRebarCoverLayer);
        }
        private void textBox_StepLength_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StepLength.Text, out StepLength);
        }
        private void textBox_StepHeight_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StepHeight.Text, out StepHeight);
        }
        private void textBox_StaircaseSlabThickness_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StaircaseSlabThickness.Text, out StaircaseSlabThickness);
        }
        private void textBox_StairCoverLayer_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StairCoverLayer.Text, out StairCoverLayer);
        }
        private void textBox_StepRebarStep_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StepRebarStep.Text, out StepRebarStep);
        }
        private void textBox_StaircaseRebarStep_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StaircaseRebarStep.Text, out StaircaseRebarStep);
        }
        private void textBox_TopExtensionStaircase_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_TopExtensionStaircase.Text, out TopExtensionStaircase);
        }
        private void textBox_TopExtensionHeightStaircase_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_TopExtensionHeightStaircase.Text, out TopExtensionHeightStaircase);
        }
        private void textBox_BottomExtensionHeightStaircase_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_BottomExtensionHeightStaircase.Text, out BottomExtensionHeightStaircase);
        }
        private void textBox_BottomExtensionHeightStaircaseNodeA2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_BottomExtensionHeightStaircaseNodeA2.Text, out BottomExtensionHeightStaircaseNodeA2);
        }
        private void textBox_FirstBarMeshName_TextChanged(object sender, EventArgs e)
        {
            FirstBarMeshName = textBox_FirstBarMeshName.Text;
        }
        private void textBox_AdditionalBarMeshName_1_TextChanged(object sender, EventArgs e)
        {
            AdditionalBarMeshName_1 = textBox_AdditionalBarMeshName_1.Text;
        }
        private void textBox_AdditionalBarMeshName_2_TextChanged(object sender, EventArgs e)
        {
            AdditionalBarMeshName_2 = textBox_AdditionalBarMeshName_2.Text;
        }
    }
}
