using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Mechanical;
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
    class MEPViewScheduleCreatorADM : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            ViewSchedule pipeSpecificationFfloorByFloor = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>().Where(vs => vs.Name == "Спецификация труб (поэтажная)")
                .First();  //Спецификация систем трубопроводов
            TableData tableData = pipeSpecificationFfloorByFloor.GetTableData();
            TableSectionData sectionData = tableData.GetSectionData(SectionType.Body);
            int nRows = sectionData.NumberOfRows;
            int nColumns = sectionData.NumberOfColumns;

            double rowHightSumm = 0;
            int sheetNumber = 101;
            int startSheetNumber = sheetNumber;

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Разделение спецификаций");
                for (int i = 0; i < nRows; i++)
                {
                    double rowHeight = sectionData.GetRowHeight(i);
                    rowHightSumm += rowHeight;

                    if (startSheetNumber == sheetNumber & rowHightSumm <= 200 / 380.4)
                    {
                        string adskGroup = "";
                        string familyName = "";
                        string pipeDiameter = "";
                        string floorNumber = "";
                        List<Element> rowElements = new List<Element>();

                        if (sectionData.CanRemoveRow(i) == true)
                        {

                            adskGroup = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 0);
                            familyName = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 1).Split(':')[1].TrimStart(' ');
                            pipeDiameter = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 2);
                            floorNumber = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 4);


                            rowElements = new FilteredElementCollector(doc, pipeSpecificationFfloorByFloor.Id)
                            .Where(fn => fn.LookupParameter("ADSK_Группирование").AsString() == adskGroup)
                            .Where(fn => fn.Name == familyName)
                            .Where(pd => pd.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString() == pipeDiameter)
                            .Where(fn => fn.LookupParameter("ADSK_Групп_спец").AsString() == floorNumber)
                            .Cast<Element>()
                            .ToList();

                            foreach (Element elem in rowElements)
                            {
                                elem.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(sheetNumber.ToString());
                            }
                        }
                    }

                    else if (startSheetNumber != sheetNumber & rowHightSumm <= 240 / 380.4)
                    {
                        string adskGroup = "";
                        string familyName = "";
                        string pipeDiameter = "";
                        string floorNumber = "";
                        List<Element> rowElements = new List<Element>();

                        if (sectionData.CanRemoveRow(i) == true)
                        {

                            adskGroup = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 0);
                            familyName = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 1).Split(':')[1].TrimStart(' ');
                            pipeDiameter = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 2);
                            floorNumber = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 4);


                            rowElements = new FilteredElementCollector(doc, pipeSpecificationFfloorByFloor.Id)
                            .Where(fn => fn.LookupParameter("ADSK_Группирование").AsString() == adskGroup)
                            .Where(fn => fn.Name == familyName)
                            .Where(pd => pd.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString() == pipeDiameter)
                            .Where(fn => fn.LookupParameter("ADSK_Групп_спец").AsString() == floorNumber)
                            .Cast<Element>()
                            .ToList();

                            foreach (Element elem in rowElements)
                            {
                                elem.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(sheetNumber.ToString());
                            }
                        }
                    }

                    else
                    {
                        rowHightSumm = rowHeight;
                        sheetNumber += 1;

                        string adskGroup = "";
                        string familyName = "";
                        string pipeDiameter = "";
                        string floorNumber = "";
                        List<Element> rowElements = new List<Element>();

                        adskGroup = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 0);
                        familyName = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 1).Split(':')[1].TrimStart(' ');
                        pipeDiameter = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 2);
                        floorNumber = pipeSpecificationFfloorByFloor.GetCellText(SectionType.Body, i, 4);


                        rowElements = new FilteredElementCollector(doc, pipeSpecificationFfloorByFloor.Id)
                        .Where(fn => fn.LookupParameter("ADSK_Группирование").AsString() == adskGroup)
                        .Where(fn => fn.Name == familyName)
                        .Where(pd => pd.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString() == pipeDiameter)
                        .Where(fn => fn.LookupParameter("ADSK_Групп_спец").AsString() == floorNumber)
                        .Cast<Element>()
                        .ToList();

                        foreach (Element elem in rowElements)
                        {
                            elem.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(sheetNumber.ToString());
                        }
                    }
                }
                t.Commit();
            }

            List<FamilySymbol> firstTitleBlockList = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .Cast<FamilySymbol>()
                .Where(tb => tb.FamilyName == "ADSK_ОсновнаяНадписьФорма3_ГОСТ21-1101-2013")
                .Where(tb => tb.Name == "А3А")
                .ToList();
            FamilySymbol firstTitleBlock = firstTitleBlockList.First();

            List<FamilySymbol> secondTitleBlockList = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .Cast<FamilySymbol>()
                .Where(tb => tb.FamilyName == "ADSK_ОсновнаяНадписьФорма6_ГОСТ21-1101-2013")
                .Where(tb => tb.Name == "А3А")
                .ToList();
            FamilySymbol secondTitleBlock = secondTitleBlockList.First();

            using (Transaction t2 = new Transaction(doc))
            {
                t2.Start("Размещение на листы");
                
                for (int i = startSheetNumber; i <= sheetNumber; i++)
                {
                    //Спецификация оборудования
                    ElementId newViewScheduleEquipmentMEPId = pipeSpecificationFfloorByFloor.Duplicate(new ViewDuplicateOption());
                    ViewSchedule newViewScheduleMEP = doc.GetElement(newViewScheduleEquipmentMEPId) as ViewSchedule;
                    newViewScheduleMEP.Name = pipeSpecificationFfloorByFloor.Name + "_лист" + i.ToString();
                    var scheduleDefinition = newViewScheduleMEP.Definition;
                    ScheduleField myField = scheduleDefinition.GetField(scheduleDefinition.GetFieldId(6));
                    ScheduleFilterType scheduleFilterType = (ScheduleFilterType)Enum.Parse(typeof(ScheduleFilterType), "Equal");
                    ScheduleFilter scheduleFilter = new ScheduleFilter(myField.FieldId, scheduleFilterType, i.ToString());
                    scheduleDefinition.AddFilter(scheduleFilter);

                    if (i== startSheetNumber)
                    {
                        ViewSheet firstViewSheet = ViewSheet.Create(doc, firstTitleBlock.Id);
                        FamilyInstance firstViewSheetFrame = new FilteredElementCollector(doc)
                            .OfClass(typeof(FamilyInstance))
                            .Cast<FamilyInstance>()
                            .Where(fi => fi.OwnerViewId == firstViewSheet.Id)
                            .ToList()
                            .First();
                        firstViewSheet.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set("CO" + i.ToString());
                        firstViewSheet.get_Parameter(BuiltInParameter.SHEET_NAME).Set("Спецификация оборудования");

                        XYZ viewLocation = new XYZ(-400 / 304.8, 260 / 304.8, 0);
                        ScheduleSheetInstance newViewSchedule = ScheduleSheetInstance.Create(doc, firstViewSheet.Id, newViewScheduleMEP.Id, viewLocation);
                    }

                    else if (i!= startSheetNumber)
                    {
                        ViewSheet secondViewSheet = ViewSheet.Create(doc, secondTitleBlock.Id);
                        FamilyInstance secondViewSheetFrame = new FilteredElementCollector(doc)
                            .OfClass(typeof(FamilyInstance))
                            .Cast<FamilyInstance>()
                            .Where(fi => fi.OwnerViewId == secondViewSheet.Id)
                            .ToList()
                            .First();
                        secondViewSheet.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set("CO" + i.ToString());
                        secondViewSheet.get_Parameter(BuiltInParameter.SHEET_NAME).Set("Спецификация оборудования");

                        XYZ viewLocation = new XYZ(-400 / 304.8, 260 / 304.8, 0);
                        ScheduleSheetInstance newViewSchedule = ScheduleSheetInstance.Create(doc, secondViewSheet.Id, newViewScheduleMEP.Id, viewLocation);
                    }

                }

                t2.Commit();
            }
            return Result.Succeeded;
        }
    }
}