using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class TXTExport : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<ViewSchedule> viewSchedules = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>()
                .OrderBy(vs => vs.Name)
                .ToList();
            List<ViewSchedule> selectedViewSchedulesList;

            TXTExportForm txtExportForm = new TXTExportForm(viewSchedules);
            txtExportForm.ShowDialog();
            if (txtExportForm.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                return Result.Cancelled;
            }
            selectedViewSchedulesList = txtExportForm.selectedViewSchedules;
            string filePath = txtExportForm.filePath;

        ViewScheduleExportOptions opt = new ViewScheduleExportOptions();
            foreach (ViewSchedule vs in selectedViewSchedulesList)
            {
                vs.Export(filePath, vs.Name + ".txt", opt);
            }

            return Result.Succeeded;
        }
    }
}
