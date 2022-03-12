using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CITRUS
{
    /// <summary>
    /// Логика взаимодействия для GloryHoleWPF.xaml
    /// </summary>
    public partial class GloryHoleWPF : Window
    {
        public double PipeSideClearance;
        public double PipeTopBottomClearance;
        public double DuctSideClearance;
        public double DuctTopBottomClearance;
        public double RoundUpIncrement;
        public bool MergeHoles;

        GloryHoleSettings gloryHoleSettings = null;
        public GloryHoleWPF()
        {
            InitializeComponent();
            GetSettingsFromXML();
            FillOutFormFieldsFromXML();
        }
        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            MergeHoles = checkBox_MergeHoles.IsChecked.Value;
            SaveSettings();
            this.DialogResult = true;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void GloryHoleWPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                MergeHoles = checkBox_MergeHoles.IsChecked.Value;
                SaveSettings();
                this.DialogResult = true;
                this.Close();
            }

            else if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
        private void GetSettingsFromXML()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;

            gloryHoleSettings = GloryHoleSettings.GetSettings();
        }
        private void SaveSettings()
        {
            double.TryParse(textBox_PipeSideClearance.Text, out PipeSideClearance);
            double.TryParse(textBox_PipeTopBottomClearance.Text, out PipeTopBottomClearance);
            double.TryParse(textBox_DuctSideClearance.Text, out DuctSideClearance);
            double.TryParse(textBox_DuctTopBottomClearance.Text, out DuctTopBottomClearance);
            double.TryParse(textBox_RoundUpIncrement.Text, out RoundUpIncrement);

            gloryHoleSettings.PipeSideClearance = PipeSideClearance;
            gloryHoleSettings.PipeTopBottomClearance = PipeTopBottomClearance;
            gloryHoleSettings.DuctSideClearance = DuctSideClearance;
            gloryHoleSettings.DuctTopBottomClearance = DuctTopBottomClearance;
            gloryHoleSettings.RoundUpIncrement = RoundUpIncrement;

            gloryHoleSettings.SaveSettings();
        }
        private void FillOutFormFieldsFromXML()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "GloryHoleSettings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                textBox_PipeSideClearance.Text = Math.Round(gloryHoleSettings.PipeSideClearance).ToString();
                textBox_PipeTopBottomClearance.Text = Math.Round(gloryHoleSettings.PipeTopBottomClearance).ToString();
                textBox_DuctSideClearance.Text = Math.Round(gloryHoleSettings.DuctSideClearance).ToString();
                textBox_DuctTopBottomClearance.Text = Math.Round(gloryHoleSettings.DuctTopBottomClearance).ToString();
                textBox_RoundUpIncrement.Text = Math.Round(gloryHoleSettings.RoundUpIncrement).ToString();
            }
            else
            {
                textBox_PipeSideClearance.Text = "50";
                textBox_PipeTopBottomClearance.Text = "50";
                textBox_DuctSideClearance.Text = "75";
                textBox_DuctTopBottomClearance.Text = "75";
                textBox_RoundUpIncrement.Text = "50";
            }
        }
    }
}
