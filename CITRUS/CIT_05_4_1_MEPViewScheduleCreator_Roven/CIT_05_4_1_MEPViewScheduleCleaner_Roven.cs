using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_05_4_1_MEPViewScheduleCreator_Roven
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_05_4_1_MEPViewScheduleCleaner_Roven : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Получение всех листов со спецификациями
            List<ViewSheet> viewSheetsList = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet))
                .Where(vs => vs.get_Parameter(BuiltInParameter.SHEET_NAME).AsString().Contains("Спецификация оборудования"))
                .Cast<ViewSheet>()
                .ToList();

            //Получение всех спецификаций Оборудования
            List<ViewSchedule> viewScheduleMEPVSCEquipmentList = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule))
                .Where(vs => vs.Name.Contains("MEPVSC_Оборудование") & vs.Name != "MEPVSC_Оборудование")
                .Cast<ViewSchedule>()
                .ToList();

            //Получение всех спецификаций круглых воздуховодов общеобменной вентиляции
            List<ViewSchedule> viewScheduleMEPVSCGeneralRoundDuctsList = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule))
                .Where(vs => vs.Name.Contains("MEPVSC_Общеобменная_Воздуховоды круглые") & vs.Name != "MEPVSC_Общеобменная_Воздуховоды круглые")
                .Cast<ViewSchedule>()
                .ToList();


            //Транзакция
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Удаление спецификаций");
                foreach (ViewSheet vs in viewSheetsList)
                {
                    doc.Delete(vs.Id);
                }

                foreach (ViewSchedule vs in viewScheduleMEPVSCEquipmentList)
                {
                    doc.Delete(vs.Id);
                }

                foreach (ViewSchedule vs in viewScheduleMEPVSCGeneralRoundDuctsList)
                {
                    doc.Delete(vs.Id);
                }
                t.Commit();
            }
                return Result.Succeeded;
        }
    }
}
