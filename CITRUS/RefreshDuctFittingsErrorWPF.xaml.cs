using System;
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
    /// Логика взаимодействия для RefreshDuctFittingsErrorWPF.xaml
    /// </summary>
    public partial class RefreshDuctFittingsErrorWPF : Window
    {
        public RefreshDuctFittingsErrorWPF(string elementIdsListStr)
        {
            InitializeComponent();
            richTextBox_Errors.Document.Blocks.Clear();
            richTextBox_Errors.Document.Blocks.Add(new Paragraph(new Run(elementIdsListStr)));
        }
        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void RefreshDuctFittingsErrorWPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
