﻿using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для GloryHoleSaveAssignmentVersionWPF.xaml
    /// </summary>
    public partial class GloryHoleSaveAssignmentVersionWPF : Window
    {
        public string ActionSelectionButtonName;
        public GloryHoleSaveAssignmentVersionWPF()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            ActionSelectionButtonName = (this.groupBox_ActionSelection.Content as Grid).Children.OfType<RadioButton>().FirstOrDefault(rb => rb.IsChecked.Value == true).Name;
            this.DialogResult = true;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void GloryHoleSaveAssignmentVersionWPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                ActionSelectionButtonName = (this.groupBox_ActionSelection.Content as Grid).Children.OfType<RadioButton>().FirstOrDefault(rb => rb.IsChecked.Value == true).Name;
                this.DialogResult = true;
                this.Close();
            }

            else if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

    }
}
