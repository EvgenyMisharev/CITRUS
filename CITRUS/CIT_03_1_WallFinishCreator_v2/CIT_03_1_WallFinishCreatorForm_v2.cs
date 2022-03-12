using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_03_1_WallFinishCreator_v2
{
    public partial class CIT_03_1_WallFinishCreatorForm_v2 : System.Windows.Forms.Form
    {
        Document Doc;
        public List<List<Element>> FinishCreatorSelectionResult = new List<List<Element>>();
        public CIT_03_1_WallFinishCreatorForm_v2(Document doc, List<Material> materials, List<WallType> wallTypes)
        {
            InitializeComponent();
            Doc = doc;
            DataGridViewTextBoxColumn dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn.Name = "Материал основы";
            dataGridView.Columns.Add(dataGridViewTextBoxColumn);

            DataGridViewComboBoxColumn dataGridViewComboBoxColumn = new DataGridViewComboBoxColumn();
            dataGridViewComboBoxColumn.Name = "Тип отделки";
            dataGridViewComboBoxColumn.DataSource = wallTypes;
            dataGridViewComboBoxColumn.DisplayMember = "Name";
            dataGridView.Columns.Add(dataGridViewComboBoxColumn);

            foreach (Material material in materials)
            {
                dataGridView.Rows.Add(material.Name);
            }
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            for (int i = 0; i< dataGridView.Rows.Count; i++)
            {
                string materialStringValue = dataGridView.Rows[i].Cells[0].Value as string;
                string wallTypeStringValue = dataGridView.Rows[i].Cells[1].Value as string;

                Material material = new FilteredElementCollector(Doc)
                    .OfCategory(BuiltInCategory.OST_Materials)
                    .Where(m => m.Name == materialStringValue)
                    .Cast<Material>()
                    .ToList()
                    .First();

                WallType wallType = new FilteredElementCollector(Doc)
                    .OfClass(typeof(WallType))
                    .Where(wt=>wt.Name == wallTypeStringValue)
                    .Cast<WallType>()
                    .ToList()
                    .First();

                List<Element> elements = new List<Element>();
                elements.Add(material);
                elements.Add(wallType);
                FinishCreatorSelectionResult.Add(elements);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
