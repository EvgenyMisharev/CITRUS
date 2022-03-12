using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_04_1_SquareColumnsReinforcement
{
    public partial class SquareСolumnTypeSelectionForm : Form
    {
        public string checkedButtonName;
        public SquareСolumnTypeSelectionForm()
        {
            InitializeComponent();

        }

        private void button1_Ok_Click(object sender, EventArgs e)
        {
            checkedButtonName = groupBox_Types.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void groupBox_Types_Enter(object sender, EventArgs e)
        {
            
        }
    }
    
    
}
