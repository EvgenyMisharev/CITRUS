using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class MEPViewScheduleHost : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            MEPViewScheduleHostStartForm mepViewScheduleHostStartForm = new MEPViewScheduleHostStartForm();
            mepViewScheduleHostStartForm.ShowDialog();
            if (mepViewScheduleHostStartForm.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                return Result.Cancelled;
            }

            string sheetNumber = mepViewScheduleHostStartForm.FirstSeetNumber;

            List<FamilySymbol> firstTitleBlockList = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .Cast<FamilySymbol>()
                .Where(tb => tb.FamilyName == "Штамп-ГОСТ_v.2.0")
                .Where(tb => tb.Name == "Стандарт")
                .ToList();
            FamilySymbol firstTitleBlock = firstTitleBlockList.First();

            List<FamilySymbol> secondTitleBlockList = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .Cast<FamilySymbol>()
                .Where(tb => tb.FamilyName == "РОВЕН_Штамп-ГОСТ-Ф6-СО")
                .Where(tb => tb.Name == "А3А")
                .ToList();
            FamilySymbol secondTitleBlock = secondTitleBlockList.First();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение спецификаций на листы");
                ViewSheet firstViewSheet = ViewSheet.Create(doc, firstTitleBlock.Id);
                FamilyInstance firstViewSheetFrame = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .Where(fi => fi.OwnerViewId == firstViewSheet.Id)
                    .ToList()
                    .First();

                firstViewSheetFrame.LookupParameter("A").Set(3);
                firstViewSheetFrame.LookupParameter("x").Set(1);
                firstViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(sheetNumber);
                firstViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NAME).Set("Спецификация оборудования"); 
                ViewSheet myViewSheet = firstViewSheet;
                FamilyInstance myViewSheetFrame = firstViewSheetFrame;

                List <ViewSchedule> viewSchedules = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSchedule))
                    .Cast<ViewSchedule>()
                    .Where(vs => vs.Name.ToString().Split('_').Length > 1)
                    .Where(vs => vs.Name.ToString().Split('_')[1] == "5.4.1"
                    || vs.Name.ToString().Split('_')[1] == "5.4.2"
                    || vs.Name.ToString().Split('_')[1] == "5.4.3"
                    || vs.Name.ToString().Split('_')[1] == "5.4.4")
                    .OrderBy(vs => regexForNameLetters(vs.Name))
                    .ThenBy(vs => regexForNameNumbers(vs.Name))
                    .ToList();

                XYZ viewLocation = new XYZ(20 / 304.8, 260 / 304.8, 0);
                foreach (ViewSchedule viewSchedule in viewSchedules)
                {

                    string scheduleName = "Система " + viewSchedule.Name.Split('_')[0];
                    TableData td = viewSchedule.GetTableData();
                    TableSectionData tsd = td.GetSectionData(SectionType.Header);
                    tsd.SetCellText(0, 0, scheduleName);

                    ScheduleSheetInstance newViewSchedule = ScheduleSheetInstance.Create(doc, myViewSheet.Id, viewSchedule.Id, viewLocation);

                    BoundingBoxXYZ bb = newViewSchedule.get_BoundingBox(myViewSheet);
                    //XYZ bbMaxXYZ = bb.Max;
                    XYZ bbMinXYZ = bb.Min;

                    viewLocation = new XYZ(viewLocation.X, bbMinXYZ.Y + 0.00694444444444445, viewLocation.Z);

                    if (myViewSheetFrame.Symbol.Family.Name == "Штамп-ГОСТ_v.2.0" & viewLocation.Y < 0.196850393700787)
                    {
                        doc.Delete(newViewSchedule.Id);
                        ViewSheet secondViewSheet = ViewSheet.Create(doc, secondTitleBlock.Id);
                        FamilyInstance secondViewSheetFrame = new FilteredElementCollector(doc)
                            .OfClass(typeof(FamilyInstance))
                            .Cast<FamilyInstance>()
                            .Where(fi => fi.OwnerViewId == secondViewSheet.Id)
                            .ToList()
                            .First();

                        Int32.TryParse(sheetNumber, out int intSheetNumber);
                        intSheetNumber += 1;
                        sheetNumber = intSheetNumber.ToString();
                        secondViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(sheetNumber);
                        secondViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NAME).Set("Спецификация оборудования");

                        viewLocation = new XYZ(20 / 304.8, 260 / 304.8, 0);
                        myViewSheet = secondViewSheet;
                        myViewSheetFrame = secondViewSheetFrame;
                        newViewSchedule = ScheduleSheetInstance.Create(doc, myViewSheet.Id, viewSchedule.Id, viewLocation);
                        bb = newViewSchedule.get_BoundingBox(myViewSheet);
                        bbMinXYZ = bb.Min;
                        viewLocation = new XYZ(viewLocation.X, bbMinXYZ.Y + 0.00694444444444445, viewLocation.Z);
                    }
                    else if (myViewSheetFrame.Symbol.Family.Name == "РОВЕН_Штамп-ГОСТ-Ф6-СО" & viewLocation.Y < 0.0656167979002617)
                    {
                        doc.Delete(newViewSchedule.Id);
                        ViewSheet secondViewSheet = ViewSheet.Create(doc, secondTitleBlock.Id);
                        FamilyInstance secondViewSheetFrame = new FilteredElementCollector(doc)
                            .OfClass(typeof(FamilyInstance))
                            .Cast<FamilyInstance>()
                            .Where(fi => fi.OwnerViewId == secondViewSheet.Id)
                            .ToList()
                            .First();

                        Int32.TryParse(sheetNumber, out int intSheetNumber);
                        intSheetNumber += 1;
                        sheetNumber = intSheetNumber.ToString();
                        secondViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(sheetNumber);
                        secondViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NAME).Set("Спецификация оборудования");

                        viewLocation = new XYZ(20 / 304.8, 260 / 304.8, 0);
                        myViewSheet = secondViewSheet;
                        myViewSheetFrame = secondViewSheetFrame;
                        newViewSchedule = ScheduleSheetInstance.Create(doc, myViewSheet.Id, viewSchedule.Id, viewLocation);
                        bb = newViewSchedule.get_BoundingBox(myViewSheet);
                        bbMinXYZ = bb.Min;
                        viewLocation = new XYZ(viewLocation.X, bbMinXYZ.Y + 0.00694444444444445, viewLocation.Z);
                    }

                }
                t.Commit();
            }
            return Result.Succeeded;
        }
        static int regexForNameNumbers(string scheduleName)
        {
            string resultStringScheduleName = Regex.Match(scheduleName.Split('_')[0], @"\d+").Value;
            Int32.TryParse(resultStringScheduleName, out int resultIntSistemNumber);
            return resultIntSistemNumber;
        }
        static string regexForNameLetters(string scheduleName)
        {
            string resultStringSistemName = Regex.Match(scheduleName.Split('_')[0], @"\D+").Value;
            return resultStringSistemName;
        }
    }
}
