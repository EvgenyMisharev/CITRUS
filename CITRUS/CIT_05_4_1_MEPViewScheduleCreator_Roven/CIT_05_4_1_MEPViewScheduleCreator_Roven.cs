using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_05_4_1_MEPViewScheduleCreator_Roven
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_05_4_1_MEPViewScheduleCreator_Roven : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Получение всех вентиляционных систем в проекте (возможно не пригодится)
            List<MechanicalSystem> mechanicalSystemList = new FilteredElementCollector(doc)
                .OfClass(typeof(MechanicalSystem))
                .Cast<MechanicalSystem>()
                .ToList();

            //Пустой список спецификаций
            List<ViewSchedule> viewScheduleList = new List<ViewSchedule>();

            //Получение шаблонной спецификации для оборудования
            List<ViewSchedule> viewScheduleMEPVSCEquipmentList = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>()
                .Where(vs => vs.Name == "MEPVSC_Оборудование")
                .ToList();
            if (viewScheduleMEPVSCEquipmentList.Count == 0)
            {
                TaskDialog.Show("Revit", "Спецификация \"MEPVSC_Оборудование\" не найдена!");
                return Result.Cancelled;
            }
            ViewSchedule viewScheduleMEPVSCEquipment = viewScheduleMEPVSCEquipmentList.First();
            //Добавляем ее в список спецификаций
            viewScheduleList.Add(viewScheduleMEPVSCEquipment);

            //Получение шиблонной спецификации Общеобменных круглых воздуховодов
            List<ViewSchedule> viewScheduleMEPVSCGeneralRoundDuctsList = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>()
                .Where(vs => vs.Name == "MEPVSC_Общеобменная_Воздуховоды круглые")
                .ToList();
            if (viewScheduleMEPVSCGeneralRoundDuctsList.Count == 0)
            {
                TaskDialog.Show("Revit", "Спецификация \"MEPVSC_Общеобменная_Воздуховоды круглые\" не найдена!");
                return Result.Cancelled;
            }
            ViewSchedule viewScheduleMEPVSCGeneralRoundDucts = viewScheduleMEPVSCGeneralRoundDuctsList.First();
            //Добавляем ее в список спецификаций
            viewScheduleList.Add(viewScheduleMEPVSCGeneralRoundDucts);

            //Получение шаблонной спецификации Общеобменных прямоугольных воздуховодов
            List<ViewSchedule> viewScheduleMEPVSCGeneralRectangularDuctsList = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>().Where(vs => vs.Name == "MEPVSC_Общеобменная_Воздуховоды прямоугольные")
                .ToList();
            if (viewScheduleMEPVSCGeneralRectangularDuctsList.Count == 0)
            {
                TaskDialog.Show("Revit", "Спецификация \"MEPVSC_Общеобменная_Воздуховоды прямоугольные\" не найдена!");
                return Result.Cancelled;
            }
            ViewSchedule viewScheduleMEPVSCGeneralRectangularDucts = viewScheduleMEPVSCGeneralRectangularDuctsList.First();
            //Добавляем ее в список спецификаций
            viewScheduleList.Add(viewScheduleMEPVSCGeneralRectangularDucts);

            //Получение шаблонной спецификации круглых воздуховодов Дымоудаления
            List<ViewSchedule> viewScheduleMEPVSCSmokeRoundDuctsList = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>()
                .Where(vs => vs.Name == "MEPVSC_Дымоудаление_Воздуховоды круглые")
                .ToList();
            if (viewScheduleMEPVSCSmokeRoundDuctsList.Count == 0)
            {
                TaskDialog.Show("Revit", "Спецификация \"MEPVSC_Дымоудаление_Воздуховоды круглые\" не найдена!");
                return Result.Cancelled;
            }
            ViewSchedule viewScheduleMEPVSCSmokeRoundDucts = viewScheduleMEPVSCSmokeRoundDuctsList.First();
            //Добавляем ее в список спецификаций
            viewScheduleList.Add(viewScheduleMEPVSCSmokeRoundDucts);

            //Получени шаблонной спецификации прямоугольных воздуховодов Дымоудаления
            List<ViewSchedule> viewScheduleMEPVSCSmokeRectangularDuctsList = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>()
                .Where(vs => vs.Name == "MEPVSC_Дымоудаление_Воздуховоды прямоугольные")
                .ToList();
            if (viewScheduleMEPVSCSmokeRectangularDuctsList.Count == 0)
            {
                TaskDialog.Show("Revit", "Спецификация \"MEPVSC_Дымоудаление_Воздуховоды прямоугольные\" не найдена!");
                return Result.Cancelled;
            }
            ViewSchedule viewScheduleMEPVSCSmokeRectangularDucts = viewScheduleMEPVSCSmokeRectangularDuctsList.First();
            //Добавляем ее в список спецификаций
            viewScheduleList.Add(viewScheduleMEPVSCSmokeRectangularDucts);

            //Получение шаблонной спецификации Изоляции воздуховодов
            List<ViewSchedule> viewScheduleMEPVSCInsulationList = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>().Where(vs => vs.Name == "MEPVSC_Изоляция воздуховодов")
                .ToList();
            if (viewScheduleMEPVSCInsulationList.Count == 0)
            {
                TaskDialog.Show("Revit", "Спецификация \"MEPVSC_Изоляция воздуховодов\" не найдена!");
                return Result.Cancelled;
            }
            ViewSchedule viewScheduleMEPVSCInsulation = viewScheduleMEPVSCInsulationList.First();
            //Добавляем ее в список спецификаций
            viewScheduleList.Add(viewScheduleMEPVSCInsulation);

            //Получение штампа для 1-го листа
            List<FamilySymbol> firstTitleBlockList = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .Cast<FamilySymbol>()
                .Where(tb => tb.FamilyName == "РОВЕН_Штамп-ГОСТ-Ф3")
                .Where(tb => tb.Name == "А3А")
                .ToList();
            if (firstTitleBlockList.Count == 0)
            {
                TaskDialog.Show("Revit", "Семейство рамки \"РОВЕН_Штамп-ГОСТ-Ф3\" тип \"А3А\" не найдено!");
                return Result.Cancelled;
            }
            FamilySymbol firstTitleBlock = firstTitleBlockList.First();

            //Получение штампа для последующих листов
            List<FamilySymbol> secondTitleBlockList = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .Cast<FamilySymbol>()
                .Where(tb => tb.FamilyName == "РОВЕН_Штамп-ГОСТ-Ф6-СО")
                .Where(tb => tb.Name == "А3А")
                .ToList();
            if (secondTitleBlockList.Count == 0)
            {
                TaskDialog.Show("Revit", "Семейство рамки \"РОВЕН_Штамп-ГОСТ-Ф6-СО\" тип \"А3А\" не найдено!");
                return Result.Cancelled;
            }
            FamilySymbol secondTitleBlock = secondTitleBlockList.First();

            //Номер листа (запилить форму с указанием номера)
            int sheetNumber = 101;
            int startSheetNumber = sheetNumber;

            //Транзакция
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Очистка параметров");
                //Активировать поле ADSK_Примечание
                Guid adskNoteGuidForActivation = new Guid("a85b7661-26b0-412f-979c-66af80b4b2c3");
                List<FamilyInstance> adskNoteElementsForActivation = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .Where(fi => fi.get_Parameter(adskNoteGuidForActivation) != null)
                    .Where(fi => fi.get_Parameter(adskNoteGuidForActivation).AsString() == null)
                    .Cast<FamilyInstance>()
                    .ToList();
                foreach (FamilyInstance element in adskNoteElementsForActivation)
                {
                    element.get_Parameter(adskNoteGuidForActivation).Set(" ");
                    element.get_Parameter(adskNoteGuidForActivation).Set("");
                }

                List<Duct> adskNoteDuctsForActivation = new FilteredElementCollector(doc)
                    .OfClass(typeof(Duct))
                    .Where(fi => fi.get_Parameter(adskNoteGuidForActivation) != null)
                    .Where(fi => fi.get_Parameter(adskNoteGuidForActivation).AsString() == null)
                    .Cast<Duct>()
                    .ToList();
                foreach (Duct duct in adskNoteDuctsForActivation)
                {
                    duct.get_Parameter(adskNoteGuidForActivation).Set(" ");
                    duct.get_Parameter(adskNoteGuidForActivation).Set("");
                }

                //Активировать поле ADSK_Группирование
                Guid adskGroupGuidForActivation = new Guid("3de5f1a4-d560-4fa8-a74f-25d250fb3401");
                List<FamilyInstance> adskGroupElementsForActivation = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .Where(fi => fi.get_Parameter(adskGroupGuidForActivation) != null)
                    .Where(fi => fi.get_Parameter(adskGroupGuidForActivation).AsString() == null)
                    .Cast<FamilyInstance>()
                    .ToList();
                foreach (FamilyInstance element in adskGroupElementsForActivation)
                {
                    element.get_Parameter(adskGroupGuidForActivation).Set(" ");
                    element.get_Parameter(adskGroupGuidForActivation).Set("");
                }

                //Активировать поле Воздуховод толщина металла Ч.М.
                List<Duct> metalThicknessDuctFerrousMetalForActivation = new FilteredElementCollector(doc)
                    .OfClass(typeof(Duct))
                    .Where(fi => fi.LookupParameter("Воздуховод толщина металла Ч.М.") != null)
                    .Where(fi => fi.LookupParameter("Воздуховод толщина металла Ч.М.").AsInteger() == 0)
                    .Cast<Duct>()
                    .ToList();
                foreach (Duct duct in metalThicknessDuctFerrousMetalForActivation)
                {
                    duct.LookupParameter("Воздуховод толщина металла Ч.М.").Set(0);
                }

                //Очистить значение РОВЕН_Пустая строка
                Guid rovenEmptyLineGuidForCleaning = new Guid("9fcdaf8a-4560-4b30-9648-f2ca2f333d11");
                List<FamilyInstance> elementsForCleaning = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .Where(fi => fi.get_Parameter(rovenEmptyLineGuidForCleaning) != null)
                    .Cast<FamilyInstance>()
                    .ToList();
                foreach (FamilyInstance element in elementsForCleaning)
                {
                    element.get_Parameter(rovenEmptyLineGuidForCleaning).Set(0);
                }
                t.Commit();

                //Старт блока создания и размещения спецификаций
                t.Start("Создание спецификаций");
                //Сщздание 1-го листа
                ViewSheet firstViewSheet = ViewSheet.Create(doc, firstTitleBlock.Id);
                //Выбор рамки на 1-ом листе
                FamilyInstance firstViewSheetFrame = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .Where(fi => fi.OwnerViewId == firstViewSheet.Id)
                    .ToList()
                    .First();
                //Заполнение параметров Номера и Имени листа
                firstViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(sheetNumber.ToString());
                firstViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NAME).Set("Спецификация оборудования");

                //Назначение 1-го листа текущим листом для размещения спецификаций
                ViewSheet myViewSheet = firstViewSheet;
                //Назначение рамки 1-го листа текущей рамкой для размещения спецификаций
                FamilyInstance myViewSheetFrame = firstViewSheetFrame;
                //Точка для размещения спецификации на листе
                XYZ viewScheduleLocation = new XYZ(20 / 304.8, 260 / 304.8, 0);
                //Параметр высоты спецификации на листе
                double viewScheduleInstanceHight = 0;

                //Перебор списка шаблонных спецификаций
                foreach (ViewSchedule viewSchedule in viewScheduleList)
                {
                    TableData tableData = viewSchedule.GetTableData();
                    TableSectionData sectionData = tableData.GetSectionData(SectionType.Body);
                    int nRows = sectionData.NumberOfRows;
                    int nColumns = sectionData.NumberOfColumns;

                    if (viewSchedule.Name == "MEPVSC_Оборудование")
                    {
                        for (int i = 0; i < nRows; i++)
                        {
                            if (startSheetNumber == sheetNumber & viewScheduleInstanceHight <= 200 / 304.8)
                            {
                                string adskGroup = "";
                                string adskName = "";
                                string adskNote = "";
                                List<FamilyInstance> rowElements = new List<FamilyInstance>();

                                if (sectionData.CanRemoveRow(i) == true)
                                {
                                    adskGroup = viewSchedule.GetCellText(SectionType.Body, i, 9);
                                    adskName = viewSchedule.GetCellText(SectionType.Body, i, 1);
                                    adskNote = viewSchedule.GetCellText(SectionType.Body, i, 8);
                                    Guid adskGroupGuid = new Guid("3de5f1a4-d560-4fa8-a74f-25d250fb3401");
                                    Guid adskNameGuid = new Guid("e6e0f5cd-3e26-485b-9342-23882b20eb43");
                                    Guid adskNoteGuid = new Guid("a85b7661-26b0-412f-979c-66af80b4b2c3");
                                    Guid rovenEmptyLineGuid = new Guid("9fcdaf8a-4560-4b30-9648-f2ca2f333d11");

                                    rowElements = new FilteredElementCollector(doc, viewSchedule.Id)
                                        .Cast<FamilyInstance>()
                                        .Where(fi => fi.get_Parameter(adskNameGuid) != null)
                                        .Where(fi => fi.get_Parameter(adskGroupGuid).AsString() == adskGroup)
                                        .Where(fi => fi.get_Parameter(adskNameGuid).AsString() == adskName)
                                        .Where(fi => fi.get_Parameter(adskNoteGuid).AsString() == adskNote)
                                        .ToList();
                                    foreach (FamilyInstance fi in rowElements)
                                    {
                                        fi.get_Parameter(rovenEmptyLineGuid).Set(sheetNumber);
                                    }

                                    rowElements = new FilteredElementCollector(doc, viewSchedule.Id)
                                        .Cast<FamilyInstance>()
                                        .Where(fi => fi.Symbol.get_Parameter(adskNameGuid) != null)
                                        .Where(fi => fi.get_Parameter(adskGroupGuid).AsString() == adskGroup)
                                        .Where(fi => fi.Symbol.get_Parameter(adskNameGuid).AsString() == adskName)
                                        .Where(fi => fi.get_Parameter(adskNoteGuid).AsString() == adskNote)
                                        .ToList();
                                    foreach (FamilyInstance fi in rowElements)
                                    {
                                        fi.get_Parameter(rovenEmptyLineGuid).Set(sheetNumber);
                                    }

                                    ElementId newViewScheduleMEPVSCEquipmentId = viewScheduleMEPVSCEquipment.Duplicate(new ViewDuplicateOption());
                                    ViewSchedule newViewScheduleMEPVSCEquipment = doc.GetElement(newViewScheduleMEPVSCEquipmentId) as ViewSchedule;
                                    newViewScheduleMEPVSCEquipment.Name = viewScheduleMEPVSCEquipment.Name + "_лист" + sheetNumber.ToString();

                                    ScheduleDefinition scheduleDefinition = newViewScheduleMEPVSCEquipment.Definition;
                                    ScheduleFilter scheduleEquipmentFilter = scheduleDefinition.GetFilter(5);
                                    scheduleEquipmentFilter.FilterType = ScheduleFilterType.Equal;
                                    scheduleEquipmentFilter.SetValue(sheetNumber);
                                    scheduleDefinition.SetFilter(5, scheduleEquipmentFilter);
                                    scheduleDefinition.GetField(25).IsHidden = true;

                                    ScheduleSheetInstance newViewScheduleInstance = ScheduleSheetInstance
                                        .Create(doc
                                        , myViewSheet.Id
                                        , newViewScheduleMEPVSCEquipment.Id
                                        , viewScheduleLocation);
                                    BoundingBoxXYZ bbox = newViewScheduleInstance.get_BoundingBox(myViewSheet);
                                    viewScheduleInstanceHight = bbox.Max.Y - bbox.Min.Y;
                                    if (viewScheduleInstanceHight <= 200 / 304.8)
                                    {
                                        doc.Delete(newViewScheduleMEPVSCEquipment.Id);
                                    }
                                    else
                                    {
                                        sheetNumber += 1;
                                        ViewSheet secondViewSheet = ViewSheet.Create(doc, secondTitleBlock.Id);
                                        FamilyInstance secondViewSheetFrame = new FilteredElementCollector(doc)
                                            .OfClass(typeof(FamilyInstance))
                                            .Cast<FamilyInstance>()
                                            .Where(fi => fi.OwnerViewId == secondViewSheet.Id)
                                            .ToList()
                                            .First();

                                        secondViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(sheetNumber.ToString());
                                        secondViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NAME).Set("Спецификация оборудования");
                                        myViewSheet = secondViewSheet;
                                        myViewSheetFrame = secondViewSheetFrame;
                                    }

                                }
                            }

                            else
                            {
                                string adskGroup = "";
                                string adskName = "";
                                string adskNote = "";
                                List<FamilyInstance> rowElements = new List<FamilyInstance>();

                                if (sectionData.CanRemoveRow(i) == true)
                                {

                                    adskGroup = viewSchedule.GetCellText(SectionType.Body, i, 9);
                                    adskName = viewSchedule.GetCellText(SectionType.Body, i, 1);
                                    adskNote = viewSchedule.GetCellText(SectionType.Body, i, 8);
                                    Guid adskGroupGuid = new Guid("3de5f1a4-d560-4fa8-a74f-25d250fb3401");
                                    Guid adskNameGuid = new Guid("e6e0f5cd-3e26-485b-9342-23882b20eb43");
                                    Guid adskNoteGuid = new Guid("a85b7661-26b0-412f-979c-66af80b4b2c3");
                                    Guid rovenEmptyLineGuid = new Guid("9fcdaf8a-4560-4b30-9648-f2ca2f333d11");

                                    rowElements = new FilteredElementCollector(doc, viewSchedule.Id)
                                        .Cast<FamilyInstance>()
                                        .Where(fi => fi.get_Parameter(adskNameGuid) != null)
                                        .Where(fi => fi.get_Parameter(adskGroupGuid).AsString() == adskGroup)
                                        .Where(fi => fi.get_Parameter(adskNameGuid).AsString() == adskName)
                                        .Where(fi => fi.get_Parameter(adskNoteGuid).AsString() == adskNote)
                                        .ToList();

                                    foreach (FamilyInstance fi in rowElements)
                                    {
                                        fi.get_Parameter(rovenEmptyLineGuid).Set(sheetNumber);
                                    }

                                    rowElements = new FilteredElementCollector(doc, viewSchedule.Id)
                                        .Cast<FamilyInstance>()
                                        .Where(fi => fi.Symbol.get_Parameter(adskNameGuid) != null)
                                        .Where(fi => fi.get_Parameter(adskGroupGuid).AsString() == adskGroup)
                                        .Where(fi => fi.Symbol.get_Parameter(adskNameGuid).AsString() == adskName)
                                        .Where(fi => fi.get_Parameter(adskNoteGuid).AsString() == adskNote)
                                        .ToList();
                                    foreach (FamilyInstance fi in rowElements)
                                    {
                                        fi.get_Parameter(rovenEmptyLineGuid).Set(sheetNumber);
                                    }

                                    ElementId newViewScheduleMEPVSCEquipmentId = viewScheduleMEPVSCEquipment.Duplicate(new ViewDuplicateOption());
                                    ViewSchedule newViewScheduleMEPVSCEquipment = doc.GetElement(newViewScheduleMEPVSCEquipmentId) as ViewSchedule;
                                    newViewScheduleMEPVSCEquipment.Name = viewScheduleMEPVSCEquipment.Name + "_лист" + sheetNumber.ToString();

                                    ScheduleDefinition scheduleDefinition = newViewScheduleMEPVSCEquipment.Definition;
                                    ScheduleFilter scheduleEquipmentFilter = scheduleDefinition.GetFilter(5);
                                    scheduleEquipmentFilter.FilterType = ScheduleFilterType.Equal;
                                    scheduleEquipmentFilter.SetValue(sheetNumber);
                                    scheduleDefinition.SetFilter(5, scheduleEquipmentFilter);
                                    scheduleDefinition.GetField(25).IsHidden = true;

                                    ScheduleSheetInstance newViewScheduleInstance = ScheduleSheetInstance.Create(doc, myViewSheet.Id, newViewScheduleMEPVSCEquipment.Id, viewScheduleLocation);
                                    BoundingBoxXYZ bbox = newViewScheduleInstance.get_BoundingBox(myViewSheet);
                                    viewScheduleInstanceHight = bbox.Max.Y - bbox.Min.Y;
                                    if (viewScheduleInstanceHight <= 240 / 304.8 & i < nRows - 1)
                                    {
                                        doc.Delete(newViewScheduleMEPVSCEquipment.Id);
                                    }
                                    else if (viewScheduleInstanceHight > 240 / 304.8 & i < nRows - 1)
                                    {
                                        sheetNumber += 1;
                                        ViewSheet secondViewSheet = ViewSheet.Create(doc, secondTitleBlock.Id);
                                        FamilyInstance secondViewSheetFrame = new FilteredElementCollector(doc)
                                            .OfClass(typeof(FamilyInstance))
                                            .Cast<FamilyInstance>()
                                            .Where(fi => fi.OwnerViewId == secondViewSheet.Id)
                                            .ToList()
                                            .First();

                                        secondViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(sheetNumber.ToString());
                                        secondViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NAME).Set("Спецификация оборудования");
                                        myViewSheet = secondViewSheet;
                                        myViewSheetFrame = secondViewSheetFrame;
                                    }

                                }

                            }
                        }
                    }

                    if (viewSchedule.Name == "MEPVSC_Общеобменная_Воздуховоды круглые")
                    {
                        viewScheduleLocation = new XYZ(viewScheduleLocation.X, viewScheduleLocation.Y - viewScheduleInstanceHight, 0);
                        double deltaHight = viewScheduleInstanceHight;
                        for (int i = 0; i < nRows; i++)
                        {
                            if (startSheetNumber != sheetNumber & viewScheduleInstanceHight <= 240 / 304.8)
                            {
                                string systemName = "";
                                string adskShortName = "";
                                string rovenProductType = "";
                                string rovenAirDuctRAL = "";
                                string rovenFittingsMaterial = "";
                                string diameter = "";
                                string airDuctConnectionCircle = "";
                                string metalThicknessDuctFerrousMetal = "";
                                string adskNote = "";
                                List<Duct> rowElements = new List<Duct>();
                                if (sectionData.CanRemoveRow(i) == true)
                                {
                                    systemName = viewSchedule.GetCellText(SectionType.Body, i, 0); //В1
                                    adskShortName = viewSchedule.GetCellText(SectionType.Body, i, 2); //Возд.
                                    rovenProductType = viewSchedule.GetCellText(SectionType.Body, i, 19).ToString(); //4 
                                    rovenAirDuctRAL = viewSchedule.GetCellText(SectionType.Body, i, 21).ToString(); //0
                                    rovenFittingsMaterial = viewSchedule.GetCellText(SectionType.Body, i, 20); //1
                                    diameter = viewSchedule.GetCellText(SectionType.Body, i, 8); //315
                                    airDuctConnectionCircle = viewSchedule.GetCellText(SectionType.Body, i, 24).ToString();//6
                                    metalThicknessDuctFerrousMetal = viewSchedule.GetCellText(SectionType.Body, i, 23).ToString();//
                                    adskNote = viewSchedule.GetCellText(SectionType.Body, i, 18); //""

                                    Guid adskShortNameGuid = new Guid("f194bf60-b880-4217-b793-1e0c30dda5e9");
                                    Guid rovenProductTypeGuid = new Guid("2efb8907-3150-4c1f-9965-6ee321ff378b");
                                    Guid rovenAirDuctRALGuid = new Guid("69f7cda5-d4c3-4077-8198-8a7d7b305803");
                                    Guid rovenFittingsMaterialGuid = new Guid("15cf4725-f4db-45d0-95e8-cab206d27f1f");
                                    Guid rovenEmptyLineGuid = new Guid("9fcdaf8a-4560-4b30-9648-f2ca2f333d11");
                                    Guid adskNoteGuid = new Guid("a85b7661-26b0-412f-979c-66af80b4b2c3");

                                    rowElements = new FilteredElementCollector(doc, viewSchedule.Id)
                                        .Cast<Duct>()
                                        .Where(fi => fi.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM).AsString() == systemName)
                                        .Where(fi => fi.DuctType.get_Parameter(adskShortNameGuid).AsString() == adskShortName)
                                        .Where(fi => fi.DuctType.get_Parameter(rovenProductTypeGuid).AsInteger().ToString() == rovenProductType)
                                        .Where(fi => fi.get_Parameter(rovenAirDuctRALGuid).AsInteger().ToString() == rovenAirDuctRAL)
                                        .Where(fi => fi.DuctType.get_Parameter(rovenFittingsMaterialGuid).AsInteger().ToString() == rovenFittingsMaterial)
                                        .Where(fi => (fi.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM).AsDouble() * 304.8).ToString() == diameter)
                                        .Where(fi => fi.LookupParameter("Воздуховод соединение круг. Возд").AsInteger().ToString() == airDuctConnectionCircle)
                                        .Where(fi => fi.LookupParameter("Воздуховод толщина металла Ч.М.").AsInteger().ToString() == metalThicknessDuctFerrousMetal)
                                        .Where(fi => fi.get_Parameter(adskNoteGuid).AsString() == adskNote)
                                        .ToList();
                                    foreach (Duct fi in rowElements)
                                    {
                                        fi.get_Parameter(rovenEmptyLineGuid).Set(sheetNumber);
                                    }

                                    ElementId newViewScheduleMEPVSCGeneralRoundDuctsId = viewScheduleMEPVSCGeneralRoundDucts.Duplicate(new ViewDuplicateOption());
                                    ViewSchedule newViewScheduleMEPVSCGeneralRoundDucts = doc.GetElement(newViewScheduleMEPVSCGeneralRoundDuctsId) as ViewSchedule;
                                    newViewScheduleMEPVSCGeneralRoundDucts.Name = viewScheduleMEPVSCGeneralRoundDucts.Name + "_лист" + sheetNumber.ToString();

                                    ScheduleDefinition scheduleDefinition = newViewScheduleMEPVSCGeneralRoundDucts.Definition;
                                    ScheduleFilter scheduleGeneralRoundDuctsFilter = scheduleDefinition.GetFilter(7);
                                    scheduleGeneralRoundDuctsFilter.FilterType = ScheduleFilterType.Equal;
                                    scheduleGeneralRoundDuctsFilter.SetValue(sheetNumber);
                                    scheduleDefinition.SetFilter(7, scheduleGeneralRoundDuctsFilter);
                                    scheduleDefinition.GetField(1).IsHidden = true;
                                    scheduleDefinition.GetField(25).IsHidden = true;
                                    scheduleDefinition.GetField(26).IsHidden = true;
                                    scheduleDefinition.GetField(30).IsHidden = true;
                                    scheduleDefinition.GetField(32).IsHidden = true;
                                    scheduleDefinition.GetField(33).IsHidden = true;

                                    ScheduleSheetInstance newViewScheduleInstance = ScheduleSheetInstance.Create(doc, myViewSheet.Id, newViewScheduleMEPVSCGeneralRoundDucts.Id, viewScheduleLocation);
                                    BoundingBoxXYZ bbox = newViewScheduleInstance.get_BoundingBox(myViewSheet);
                                    viewScheduleInstanceHight = bbox.Max.Y - bbox.Min.Y + deltaHight;
                                    if (viewScheduleInstanceHight <= 240 / 304.8 & i < nRows - 1)
                                    {
                                        doc.Delete(newViewScheduleMEPVSCGeneralRoundDucts.Id);
                                    }
                                    else
                                    {
                                        sheetNumber += 1;
                                        ViewSheet secondViewSheet = ViewSheet.Create(doc, secondTitleBlock.Id);
                                        FamilyInstance secondViewSheetFrame = new FilteredElementCollector(doc)
                                            .OfClass(typeof(FamilyInstance))
                                            .Cast<FamilyInstance>()
                                            .Where(fi => fi.OwnerViewId == secondViewSheet.Id)
                                            .ToList()
                                            .First();

                                        secondViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(sheetNumber.ToString());
                                        secondViewSheetFrame.get_Parameter(BuiltInParameter.SHEET_NAME).Set("Спецификация оборудования");
                                        myViewSheet = secondViewSheet;
                                        myViewSheetFrame = secondViewSheetFrame;
                                        viewScheduleLocation = new XYZ(20 / 304.8, 260 / 304.8, 0);
                                        viewScheduleInstanceHight = 0;
                                        deltaHight = 0;
                                    }
                                }

                            }
                        }
                    }

                    if (viewSchedule.Name == "MEPVSC_Общеобменная_Воздуховоды прямоугольные")
                    {
                        for (int i = 0; i < nRows; i++)
                        {
                            if (startSheetNumber != sheetNumber & viewScheduleInstanceHight <= 240 / 304.8)
                            {
                                string systemName = "";
                                string adskShortName = "";
                                string rovenProductType = "";
                                string rovenAirDuctRAL = "";
                                string rovenFittingsMaterial = "";
                                string width = "";
                                string height = "";
                                string airDuctConnectionRectangular = "";
                                string metalThicknessDuctFerrousMetal = "";
                                string adskNote = "";
                                List<Duct> rowElements = new List<Duct>();

                                if (sectionData.CanRemoveRow(i) == true)
                                {
                                    systemName = viewSchedule.GetCellText(SectionType.Body, i, 0); //В1
                                    adskShortName = viewSchedule.GetCellText(SectionType.Body, i, 2); //Возд.
                                    rovenProductType = viewSchedule.GetCellText(SectionType.Body, i, 20).ToString(); //1
                                    rovenAirDuctRAL = viewSchedule.GetCellText(SectionType.Body, i, 22).ToString(); //0
                                    rovenFittingsMaterial = viewSchedule.GetCellText(SectionType.Body, i, 21); //1
                                    width = viewSchedule.GetCellText(SectionType.Body, i, 7); //200
                                    height = viewSchedule.GetCellText(SectionType.Body, i, 9); //150
                                    airDuctConnectionRectangular = viewSchedule.GetCellText(SectionType.Body, i, 18).ToString();//5
                                    metalThicknessDuctFerrousMetal = viewSchedule.GetCellText(SectionType.Body, i, 24).ToString();//0
                                    adskNote = viewSchedule.GetCellText(SectionType.Body, i, 19); //""
                                    Guid adskShortNameGuid = new Guid("f194bf60-b880-4217-b793-1e0c30dda5e9");
                                    Guid rovenProductTypeGuid = new Guid("2efb8907-3150-4c1f-9965-6ee321ff378b");
                                    Guid rovenAirDuctRALGuid = new Guid("69f7cda5-d4c3-4077-8198-8a7d7b305803");
                                    Guid rovenFittingsMaterialGuid = new Guid("15cf4725-f4db-45d0-95e8-cab206d27f1f");
                                    Guid rovenEmptyLineGuid = new Guid("9fcdaf8a-4560-4b30-9648-f2ca2f333d11");
                                    Guid adskNoteGuid = new Guid("a85b7661-26b0-412f-979c-66af80b4b2c3");

                                    rowElements = new FilteredElementCollector(doc, viewSchedule.Id)
                                        .Cast<Duct>()
                                        .Where(fi => fi.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM).AsString() == systemName)
                                        .Where(fi => fi.DuctType.get_Parameter(adskShortNameGuid).AsString() == adskShortName)
                                        .Where(fi => fi.DuctType.get_Parameter(rovenProductTypeGuid).AsInteger().ToString() == rovenProductType)
                                        .Where(fi => fi.get_Parameter(rovenAirDuctRALGuid).AsInteger().ToString() == rovenAirDuctRAL)
                                        .Where(fi => fi.DuctType.get_Parameter(rovenFittingsMaterialGuid).AsInteger().ToString() == rovenFittingsMaterial)
                                        .Where(fi => (fi.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM).AsDouble() * 304.8).ToString() == width)
                                        .Where(fi => (fi.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM).AsDouble() * 304.8).ToString() == height)
                                        .Where(fi => fi.LookupParameter("Воздуховод соединение прямоуг. Возд").AsInteger().ToString() == airDuctConnectionRectangular)
                                        .Where(fi => fi.LookupParameter("Воздуховод толщина металла Ч.М.").AsInteger().ToString() == metalThicknessDuctFerrousMetal)
                                        .Where(fi => fi.get_Parameter(adskNoteGuid).AsString() == adskNote)
                                        .ToList();

                                    foreach (Duct fi in rowElements)
                                    {
                                        fi.get_Parameter(rovenEmptyLineGuid).Set(sheetNumber);
                                    }
                                }
                            }
                        }
                    }
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
