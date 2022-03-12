using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace CITRUS
{
    public partial class TXTExportForm : System.Windows.Forms.Form
    {
        public List<ViewSchedule> selectedViewSchedules;
        public string filePath = "";
        public TXTExportForm(List<ViewSchedule> viewSchedules)
        {
            InitializeComponent();
            List<ViewSchedule> viewSchedulesListForComboBox = viewSchedules;
            listBox_ViewSchedules.DataSource = viewSchedulesListForComboBox;
            listBox_ViewSchedules.DisplayMember = "Name";
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            selectedViewSchedules = listBox_ViewSchedules.SelectedItems.Cast<ViewSchedule>().ToList();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btn_FolderBrowserDialog_Click(object sender, EventArgs e)
        {
            //folderBrowserDialog.ShowDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = folderBrowserDialog.SelectedPath;
                richTextBox_FilePach.Text = filePath;
            }
        }
    }
}
