using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Structure;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class RebarOutletsCreator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Список типов для выбора арматуры хомутов
            List<RebarBarType> stirrupRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Доступ к форме
            FormRebarOutletsCreator formRebarOutletsCreator = new FormRebarOutletsCreator(stirrupRebarTapesList);
            formRebarOutletsCreator.ShowDialog();
            if (formRebarOutletsCreator.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                return Result.Cancelled;
            }
            string outletSizesCheckedButtonName = formRebarOutletsCreator.OutletSizesCheckedButtonName;
            string forceTypeCheckedButtonName = formRebarOutletsCreator.ForceTypeCheckedButtonName;
            string columnArrangementCheckedButtonName = formRebarOutletsCreator.ColumnArrangementCheckedButtonName;
            double manualOverlapLength = formRebarOutletsCreator.ManualOverlapLength / 304.8;
            double manualAnchorageLength = formRebarOutletsCreator.ManualAnchorageLength / 304.8;
            double offsetFromSlabBottom = formRebarOutletsCreator.OffsetFromSlabBottom / 304.8;

            //Выбор типа арматуры хомутов
            RebarBarType myStirrupBarTape = formRebarOutletsCreator.mySelectionStirrupBarTape;

            //Выбор фундаментной плиты
            FloorSelectionFilter selFilterFloor = new FloorSelectionFilter(); //Вызов фильтра выбора
            Reference selFloor = sel.PickObject(ObjectType.Element, selFilterFloor, "Выберите фундаментную плиту!");//Получение списка ссылок на выбранную плиту
            Floor mySelFloor = doc.GetElement(selFloor) as Floor;
            if (mySelFloor.Category.Id.IntegerValue != -2001300 & mySelFloor.Category.Id.IntegerValue != -2000032) //Для фундаментной плиты перекрытием
            {
                TaskDialog.Show("Revit", "Выберите фундаментную плиту!");
                return Result.Cancelled;
            }

            //Имя материала плиты
            CompoundStructure floorStructure = mySelFloor.FloorType.GetCompoundStructure();
            Material floorMaterial = doc.GetElement(floorStructure.GetMaterialId(0)) as Material;
            Guid floorMaterialCodeGuid = new Guid("b5675d33-fade-46b1-921b-0cab8eec101e");
            Parameter floorMaterialCode = floorMaterial.get_Parameter(floorMaterialCodeGuid);
            string floorMaterialName = "";
            if (floorMaterialCode != null)
            {
                floorMaterialName = floorMaterial.get_Parameter(floorMaterialCodeGuid).AsValueString().Substring(2, 1).ToString()
                    + floorMaterial.get_Parameter(floorMaterialCodeGuid).AsValueString().Substring(3, 1).ToString();
            }
            else
            {
                floorMaterialName = floorMaterial.Name.Split(' ').ToArray()[0].ToString();
            }

            //Толщина плиты
            double mySelFloorThickness = mySelFloor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble();
            //Координаты верха плиты
            ElementId mySelFloorLevelId = mySelFloor.LevelId;
            Level mySelFloorLevel = doc.GetElement(mySelFloorLevelId) as Level;
            double mySelFloorElevation = mySelFloorLevel.Elevation;
            double mySelFloorTopOffset = mySelFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble();
            double mySelFloorTopElevation = Math.Round((mySelFloorElevation + mySelFloorTopOffset), 6);
            
            Document doc2 = null;
            XYZ linkOrigin = new XYZ();
            List<FamilyInstance> columnsList = new List<FamilyInstance>();
            if (columnArrangementCheckedButtonName == "radioButton_Link")
            {
                //Выбор связанного файла
                RevitLinkInstanceSelectionFilter selFilterRevitLinkInstance = new RevitLinkInstanceSelectionFilter(); //Вызов фильтра выбора
                Reference selRevitLinkInstance = sel.PickObject(ObjectType.Element, selFilterRevitLinkInstance, "Выберите связанный файл!");//Получение ссылки на выбранную группу
                IEnumerable<RevitLinkInstance> myRevitLinkInstance = new FilteredElementCollector(doc)
                    .OfClass(typeof(RevitLinkInstance))
                    .Where(li => li.Id == selRevitLinkInstance.ElementId)
                    .Cast<RevitLinkInstance>();
                linkOrigin = myRevitLinkInstance.First().GetTransform().Origin;
                doc2 = myRevitLinkInstance.First().GetLinkDocument();

                //Выбор колонн в связанном файле
                IList<Reference> selElementsList = sel.PickObjects(ObjectType.LinkedElement, "Выберите колонны!");//Получение списка ссылок на выбранные колонны
                foreach (Reference refElem in selElementsList)
                {
                    if (doc2.GetElement(refElem.LinkedElementId).Category.Id.ToString() == "-2001330")
                    {
                        columnsList.Add((doc2.GetElement(refElem.LinkedElementId)) as FamilyInstance);
                    }
                }
            }
            else
            {
                //Выбор колонн
                ColumnSelectionFilter columnSelFilter = new ColumnSelectionFilter(); //Вызов фильтра выбора
                IList<Reference> selColumns = sel.PickObjects(ObjectType.Element, columnSelFilter, "Выберите колонны!");//Получение списка ссылок на выбранные колонны

                foreach (Reference columnRef in selColumns)
                {
                    columnsList.Add(doc.GetElement(columnRef) as FamilyInstance);
                }
                //Завершение блока Получение списка колонн
            }

#region Формы арматуры
            //Выбор формы основной арматуры если требуемая длина анкеровки меньше толщины плиты
            List<RebarShape> rebarShapeAnchorageOutletsLessFloorThicknessList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "01")
                .Cast<RebarShape>()
                .ToList();
            if (rebarShapeAnchorageOutletsLessFloorThicknessList.Count == 0)
            {
                rebarShapeAnchorageOutletsLessFloorThicknessList = new FilteredElementCollector(doc)
               .OfClass(typeof(RebarShape))
               .Where(rs => rs.Name.ToString() == "О_1")
               .Cast<RebarShape>()
               .ToList();
                if (rebarShapeAnchorageOutletsLessFloorThicknessList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 01 или О_1 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape myRebarShapeAnchorageOutletsLessFloorThickness = rebarShapeAnchorageOutletsLessFloorThicknessList.First();

            //Выбор формы основной арматуры если требуемая длина анкеровки больше толщины плиты
            List<RebarShape> rebarShapeAnchorageOutletsGreaterFloorThicknessList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "11")
                .Cast<RebarShape>()
                .ToList();
            if (rebarShapeAnchorageOutletsGreaterFloorThicknessList.Count == 0)
            {
                rebarShapeAnchorageOutletsGreaterFloorThicknessList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_11")
                .Cast<RebarShape>()
                .ToList();
                if (rebarShapeAnchorageOutletsGreaterFloorThicknessList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 11 или О_11 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape myrebarShapeAnchorageOutletsGreaterFloorThickness = rebarShapeAnchorageOutletsGreaterFloorThicknessList.First();

            //Выбор формы хомута
            List<RebarShape> rebarStirrupShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "51")
                .Cast<RebarShape>()
                .ToList();
            if (rebarStirrupShapeList.Count == 0)
            {
                rebarStirrupShapeList = new FilteredElementCollector(doc)
               .OfClass(typeof(RebarShape))
               .Where(rs => rs.Name.ToString() == "Х_51")
               .Cast<RebarShape>()
               .ToList();
                if (rebarStirrupShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 51 или Х_51 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape myStirrupRebarShape = rebarStirrupShapeList.First();

            //Выбор формы загиба хомута
            List<RebarHookType> rebarHookTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarHookType))
                .Where(rs => rs.Name.ToString() == "Сейсмическая поперечная арматура - 135 градусов")
                .Cast<RebarHookType>()
                .ToList();
            if (rebarHookTypeList.Count == 0)
            {
                rebarHookTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarHookType))
                .Where(rs => rs.Name.ToString() == "Хомут/стяжка_135°")
                .Cast<RebarHookType>()
                .ToList();
                if (rebarHookTypeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма загиба Сейсмическая поперечная арматура - 135 градусов или Хомут/стяжка_135° не найдена");
                    return Result.Failed;
                }
            }
            RebarHookType myRebarHookType = rebarHookTypeList.First();

            //Диаметр хомута
            Parameter stirrupRebarTypeDiamParam = myStirrupBarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double stirrupRebarDiam = stirrupRebarTypeDiamParam.AsDouble();
#endregion

            //Список семейств с именем CIT_04_ВаннаДляСварки
            List<Family> familiesTubWelding = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().Where(f => f.Name == "CIT_04_ВаннаДляСварки").ToList();
            if (familiesTubWelding.Count == 0)
            {
                TaskDialog.Show("Revit", "Семейство CIT_04_ВаннаДляСварки не найдено");
                return Result.Failed;
            }
            Family familieTubWelding = familiesTubWelding.First();

            //CIT_04_ВаннаДляСварки
            List<ElementId> symbolsTubWeldingIds = familieTubWelding.GetFamilySymbolIds().ToList();
            ElementId firstSymbolTubWeldingId = symbolsTubWeldingIds.First();

            //Тип элемента(FamilySymbol) CIT_04_ВаннаДляСварки
            FamilySymbol myTubWeldingSymbol = doc.GetElement(firstSymbolTubWeldingId) as FamilySymbol;
            if (myTubWeldingSymbol == null)
            {
                TaskDialog.Show("Revit", "Семейство CIT_04_ВаннаДляСварки не найдено");
                return Result.Failed;
            }
            //Завершение блока Получение типа элемента CIT_04_ВаннаДляСварки


            //Нормаль для построения стержней основной арматуры
            XYZ mainRebarNormal = new XYZ(0, 1, 0);

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение выпусков");
                foreach (FamilyInstance column in columnsList)
                {
                    //Материал колонны
                    ElementId columnMaterialId = column.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsElementId();
                    Material columnMaterial = null;
                    if (columnArrangementCheckedButtonName == "radioButton_Link")
                    { 
                        columnMaterial = doc2.GetElement(columnMaterialId) as Material; 
                    }
                    else
                    {
                        columnMaterial = doc.GetElement(columnMaterialId) as Material;
                    }
                    Guid columnMaterialCodeGuid = new Guid("b5675d33-fade-46b1-921b-0cab8eec101e");
                    string columnMaterialName = "";
                    Parameter columnMaterialCode = columnMaterial.get_Parameter(columnMaterialCodeGuid);
                    if (columnMaterialCode != null)
                    {
                        columnMaterialName = columnMaterial.get_Parameter(columnMaterialCodeGuid).AsValueString().Substring(2,1).ToString()
                            + columnMaterial.get_Parameter(columnMaterialCodeGuid).AsValueString().Substring(3, 1).ToString();
                    }
                    else
                    {
                        string columnMaterialNameAsString = columnMaterial.Name + " AddSimbol";
                        columnMaterialName = columnMaterialNameAsString.Split(' ').ToArray()[0].ToString();
                    }
                    LocationPoint columnLocationPoint = column.Location as LocationPoint;
                    XYZ columnLocationPointXYZ = new XYZ(columnLocationPoint.Point.X, columnLocationPoint.Point.Y, columnLocationPoint.Point.Z);
                    //Защитный слой колонны
                    ElementId myRebarCoverTypeID = column.get_Parameter(BuiltInParameter.CLEAR_COVER_OTHER).AsElementId();
                    RebarCoverType myRebarCoverType = null;
                    if (columnArrangementCheckedButtonName == "radioButton_Link")
                    {
                        myRebarCoverType = doc2.GetElement(myRebarCoverTypeID) as RebarCoverType;
                    }
                    else
                    {
                        myRebarCoverType = doc.GetElement(myRebarCoverTypeID) as RebarCoverType;
                    }
                    double mainRebarCoverLayer = myRebarCoverType.CoverDistance;

                    //Габариты колонны
                    //Ширина сечения колонны
                    double columnSectionWidth = 0;
                    if (column.Symbol.LookupParameter("Рзм.Ширина") != null)
                    {
                        columnSectionWidth = column.Symbol.LookupParameter("Рзм.Ширина").AsDouble();
                    }
                    else
                    {
                        columnSectionWidth = column.Symbol.LookupParameter("ADSK_Размер_Ширина").AsDouble();
                    }

                    //Высота сечения колонны
                    double columnSectionHeight = 0;
                    if (column.Symbol.LookupParameter("Рзм.Высота") != null)
                    {
                        columnSectionHeight = column.Symbol.LookupParameter("Рзм.Высота").AsDouble();
                    }
                    else
                    {
                        columnSectionHeight = column.Symbol.LookupParameter("ADSK_Размер_Высота").AsDouble();
                    }

                    //Точка вставки колонны
                    LocationPoint columnOriginLocationPoint = column.Location as LocationPoint;
                    XYZ columnLinkOrigin = columnOriginLocationPoint.Point;
                    XYZ columnOrigin = new XYZ(columnLinkOrigin.X + linkOrigin.X, columnLinkOrigin.Y + linkOrigin.Y, columnLinkOrigin.Z + linkOrigin.Z);

                    BoundingBoxXYZ bbox = column.get_BoundingBox(null);
                    Outline myColumnOutLn = new Outline(bbox.Min, bbox.Max);

                    List<Rebar> columnRebarList = new List<Rebar>();
                    //Список стержней в колонне
                    Guid rebarClassNumberGuid = new Guid("32a47c7f-e91d-4a8e-bf24-927cb679b4d1");
                    if (columnArrangementCheckedButtonName == "radioButton_Link")
                    {
                        Parameter rebarClassNumber = null;
                        List<Rebar> columnRebarListTmp = new FilteredElementCollector(doc2)
                        .OfClass(typeof(Rebar))
                        .WherePasses(new BoundingBoxIntersectsFilter(myColumnOutLn))
                        .Cast<Rebar>()
                        .ToList();
                        if (columnRebarListTmp.Count != 0)
                        {
                            rebarClassNumber = doc2.GetElement(columnRebarListTmp.First().GetTypeId()).get_Parameter(rebarClassNumberGuid);
                        }
                        if(rebarClassNumber != null)
                        {
                            columnRebarList = new FilteredElementCollector(doc2)
                            .OfClass(typeof(Rebar))
                            .WherePasses(new BoundingBoxIntersectsFilter(myColumnOutLn))
                            .Cast<Rebar>()
                            .Where(r => doc2.GetElement(r.GetTypeId()).get_Parameter(rebarClassNumberGuid).AsDouble() == 500
                            || doc2.GetElement(r.GetTypeId()).get_Parameter(rebarClassNumberGuid).AsDouble() == 400)
                            .Where(r => r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "51" &
                            r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "Х_51")
                            .Where(r => r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "02" &
                            r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "Х_(22)")
                            .Where(r => doc2.GetElement(r.GetHostId()).Category.Id.ToString() == "-2001330")
                            .ToList();
                        }
                        else
                        {
                            columnRebarList = new FilteredElementCollector(doc2)
                            .OfClass(typeof(Rebar))
                            .WherePasses(new BoundingBoxIntersectsFilter(myColumnOutLn))
                            .Cast<Rebar>()
                            .Where(r => r.Name.Split(' ').ToArray().Contains("A500") || r.Name.Split(' ').ToArray().Contains("А500")
                            || r.Name.Split(' ').ToArray().Contains("A500C") || r.Name.Split(' ').ToArray().Contains("А500С")
                            || r.Name.Split(' ').ToArray().Contains("A500СП") || r.Name.Split(' ').ToArray().Contains("А500СП")
                            || r.Name.Split(' ').ToArray().Contains("A400") || r.Name.Split(' ').ToArray().Contains("А400")
                            || r.Name.Split(' ').ToArray().Contains("A400C") || r.Name.Split(' ').ToArray().Contains("А400С")
                            || r.Name.Split(' ').ToArray().Contains("A400СП") || r.Name.Split(' ').ToArray().Contains("А400СП"))                            .Where(r => r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "51" &
                            r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "Х_51")
                            .Where(r => r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "02" &
                            r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "Х_(22)")
                            .Where(r => doc2.GetElement(r.GetHostId()).Category.Id.ToString() == "-2001330")
                            .ToList();
                        }
                    }
                    else
                    {
                        Parameter rebarClassNumber = null;
                        List<Rebar> columnRebarListTmp = new FilteredElementCollector(doc)
                        .OfClass(typeof(Rebar))
                        .WherePasses(new BoundingBoxIntersectsFilter(myColumnOutLn))
                        .Cast<Rebar>()
                        .ToList();
                        if (columnRebarListTmp.Count != 0)
                        {
                            rebarClassNumber = doc.GetElement(columnRebarListTmp.First().GetTypeId()).get_Parameter(rebarClassNumberGuid);
                        }
                        if (rebarClassNumber != null)
                        {
                            columnRebarList = new FilteredElementCollector(doc)
                                .OfClass(typeof(Rebar))
                                .WherePasses(new BoundingBoxIntersectsFilter(myColumnOutLn))
                                .Cast<Rebar>()
                                .Where(r => doc.GetElement(r.GetTypeId()).get_Parameter(rebarClassNumberGuid).AsDouble() == 500
                                || doc.GetElement(r.GetTypeId()).get_Parameter(rebarClassNumberGuid).AsDouble() == 400)
                                .Where(r => r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "51" &
                                r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "Х_51")
                                .Where(r => r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "02" &
                                r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "Х_(22)")
                                .Where(r => doc.GetElement(r.GetHostId()).Category.Id.ToString() == "-2001330")
                                .ToList();
                        }
                        else
                        {
                            columnRebarList = new FilteredElementCollector(doc)
                                .OfClass(typeof(Rebar))
                                .WherePasses(new BoundingBoxIntersectsFilter(myColumnOutLn))
                                .Cast<Rebar>()
                                .Where(r => r.Name.Split(' ').ToArray().Contains("A500") || r.Name.Split(' ').ToArray().Contains("А500")
                                || r.Name.Split(' ').ToArray().Contains("A500C") || r.Name.Split(' ').ToArray().Contains("А500С")
                                || r.Name.Split(' ').ToArray().Contains("A500СП") || r.Name.Split(' ').ToArray().Contains("А500СП")
                                || r.Name.Split(' ').ToArray().Contains("A400") || r.Name.Split(' ').ToArray().Contains("А400")
                                || r.Name.Split(' ').ToArray().Contains("A400C") || r.Name.Split(' ').ToArray().Contains("А400С")
                                || r.Name.Split(' ').ToArray().Contains("A400СП") || r.Name.Split(' ').ToArray().Contains("А400СП"))
                                .Where(r => r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "51" &
                                r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "Х_51")
                                .Where(r => r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "02" &
                                r.get_Parameter(BuiltInParameter.REBAR_SHAPE).AsValueString() != "Х_(22)")
                                .Where(r => doc.GetElement(r.GetHostId()).Category.Id.ToString() == "-2001330")
                                .ToList();
                        }

                    }

                    if (columnRebarList.Count == 0)
                    {
                        TaskDialog.Show("Revit", "Арматура в колонне не найдена!");
                        return Result.Cancelled;
                    }

                    //Получение ID стержня с мин Х и мин Y из списка
                    ElementId minXminY = GetMinXMinYRebar(columnRebarList, columnSectionWidth, columnSectionHeight, mainRebarCoverLayer, columnOrigin, linkOrigin);
                    //Получение ID стержня с мин Х и макс Y из списка
                    ElementId minXmaxY = GetMinXMaxYRebar(columnRebarList, columnSectionWidth, columnSectionHeight, mainRebarCoverLayer, columnOrigin, linkOrigin);
                    //Получение ID стержня с мин Х и макс Y из списка
                    ElementId maxXmaxY = GetMaxXMaxYRebar(columnRebarList, columnSectionWidth, columnSectionHeight, mainRebarCoverLayer, columnOrigin, linkOrigin);
                    //Получение ID стержня с мин Х и макс Y из списка
                    ElementId maxXminY = GetMaxXMinYRebar(columnRebarList, columnSectionWidth, columnSectionHeight, mainRebarCoverLayer, columnOrigin, linkOrigin);

                    //Нахлест или сварка
                    string OverlapOrWelding = "";
                    //Максимальная длина анкеровки
                    double maxAnchoringLength = 0;

                    double maxColumnRebarDiamParamDouble = 0;

                    //Универсальная коллекция для формирования группы выпусков
                    ICollection<ElementId> rebarIdCollection = new List<ElementId>();

                    // Обработка списка стержней из колонны
                    foreach (Rebar columnRebar in columnRebarList)
                    {
                        //Диаметр стержня
                        RebarBarType columnRebarType = columnRebar.Document.GetElement(columnRebar.GetTypeId()) as RebarBarType;
                        double columnRebarDiamParamDouble = columnRebar.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
                        Parameter rebarClassNumber = null;
                        rebarClassNumber = columnRebar.get_Parameter(rebarClassNumberGuid);
                        string rebarClassNumberString = "";
                        if (rebarClassNumber != null)
                        {
                            rebarClassNumberString = doc.GetElement(columnRebar.GetTypeId())
                                .get_Parameter(rebarClassNumberGuid)
                                .AsDouble()
                                .ToString();
                        }
                        else
                        {
                            rebarClassNumberString = columnRebar.Name.Split(' ').ToArray()[0].Substring(1);
                        }

                        //Определение координат нижней точки арматурного стержня
                        IList<Curve> columnRebarCurveList = columnRebar.GetCenterlineCurves(false, false, false, new MultiplanarOption(), 0);
                        Curve firstCurveColumnRebar = columnRebarCurveList.First();
                        XYZ columnRebarFirstPoint = firstCurveColumnRebar.GetEndPoint(0);

                        //Длина нахлеста
                        double overlapLength = 0;
                        //Длина анкеровки
                        double anchorageLength = 0;

                        //Длина, если автоподбор и растяжение A500
                        if (outletSizesCheckedButtonName == "radioButton_Auto" 
                            & forceTypeCheckedButtonName == "radioButton_Stretching"
                            & rebarClassNumberString == "500")
                        {   //Определение длины нахлеста из диаметра арматуры и класса бетона

                            if (columnMaterialName == "B20")
                            {
                                overlapLength = StretchingOverlapLengthsB20A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B25")
                            {
                                overlapLength = StretchingOverlapLengthsB25A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B30")
                            {
                                overlapLength = StretchingOverlapLengthsB30A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B35")
                            {
                                overlapLength = StretchingOverlapLengthsB35A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B40")
                            {
                                overlapLength = StretchingOverlapLengthsB40A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "20")
                            {
                                overlapLength = StretchingOverlapLengthsB20A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "25")
                            {
                                overlapLength = StretchingOverlapLengthsB25A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "30")
                            {
                                overlapLength = StretchingOverlapLengthsB30A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "35")
                            {
                                overlapLength = StretchingOverlapLengthsB35A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "40")
                            {
                                overlapLength = StretchingOverlapLengthsB40A500(columnRebarDiamParamDouble);
                            }

                            if (overlapLength == 0)
                            {
                                TaskDialog.Show("Revit", "Материал колонны некорректен!");
                                return Result.Cancelled;
                            }

                            //Определение длины анкеровки из диаметра арматуры и класса бетона

                            if (floorMaterialName == "B20")
                            {
                                anchorageLength = StretchingAnchorageLengthsB20A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B25")
                            {
                                anchorageLength = StretchingAnchorageLengthsB25A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B30")
                            {
                                anchorageLength = StretchingAnchorageLengthsB30A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B35")
                            {
                                anchorageLength = StretchingAnchorageLengthsB35A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B40")
                            {
                                anchorageLength = StretchingAnchorageLengthsB40A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "20")
                            {
                                anchorageLength = StretchingAnchorageLengthsB20A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "25")
                            {
                                anchorageLength = StretchingAnchorageLengthsB25A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "30")
                            {
                                anchorageLength = StretchingAnchorageLengthsB30A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "35")
                            {
                                anchorageLength = StretchingAnchorageLengthsB35A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "40")
                            {
                                anchorageLength = StretchingAnchorageLengthsB40A500(columnRebarDiamParamDouble);
                            }

                            if (anchorageLength == 0)
                            {
                                TaskDialog.Show("Revit", "Материал фундаментной плиты некорректен!");
                                return Result.Cancelled;
                            }
                        }

                        //Длина, если автоподбор и сжатие A500
                        if (outletSizesCheckedButtonName == "radioButton_Auto" 
                            & forceTypeCheckedButtonName == "radioButton_Compression"
                            & rebarClassNumberString == "500")
                        {
                            //Определение длины нахлеста из диаметра арматуры и класса бетона

                            if (columnMaterialName == "B20")
                            {
                                overlapLength = CompressionOverlapLengthsB20A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B25")
                            {
                                overlapLength = CompressionOverlapLengthsB25A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B30")
                            {
                                overlapLength = CompressionOverlapLengthsB30A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B35")
                            {
                                overlapLength = CompressionOverlapLengthsB35A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B40")
                            {
                                overlapLength = CompressionOverlapLengthsB40A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "20")
                            {
                                overlapLength = CompressionOverlapLengthsB20A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "25")
                            {
                                overlapLength = CompressionOverlapLengthsB25A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "30")
                            {
                                overlapLength = CompressionOverlapLengthsB30A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "35")
                            {
                                overlapLength = CompressionOverlapLengthsB35A500(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "40")
                            {
                                overlapLength = CompressionOverlapLengthsB40A500(columnRebarDiamParamDouble);
                            }

                            if (overlapLength == 0)
                            {
                                TaskDialog.Show("Revit", "Материал колонны некорректен!");
                                return Result.Cancelled;
                            }

                            //Определение длины анкеровки из диаметра арматуры и класса бетона

                            if (floorMaterialName == "B20")
                            {
                                anchorageLength = CompressionAnchorageLengthsB20A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B25")
                            {
                                anchorageLength = CompressionAnchorageLengthsB25A500(columnRebarDiamParamDouble);
                            }

                            else if (floorMaterialName == "B30")
                            {
                                anchorageLength = CompressionAnchorageLengthsB30A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B35")
                            {
                                anchorageLength = CompressionAnchorageLengthsB35A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B40")
                            {
                                anchorageLength = CompressionAnchorageLengthsB40A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "20")
                            {
                                anchorageLength = CompressionAnchorageLengthsB20A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "25")
                            {
                                anchorageLength = CompressionAnchorageLengthsB25A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "30")
                            {
                                anchorageLength = CompressionAnchorageLengthsB30A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "35")
                            {
                                anchorageLength = CompressionAnchorageLengthsB35A500(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "40")
                            {
                                anchorageLength = CompressionAnchorageLengthsB40A500(columnRebarDiamParamDouble);
                            }

                            if (anchorageLength == 0)
                            {
                                TaskDialog.Show("Revit", "Материал фундаментной плиты некорректен!");
                                return Result.Cancelled;
                            }
                        }



                        //Длина, если автоподбор и растяжение A400
                        if (outletSizesCheckedButtonName == "radioButton_Auto"
                            & forceTypeCheckedButtonName == "radioButton_Stretching"
                            & rebarClassNumberString == "400")
                        {   //Определение длины нахлеста из диаметра арматуры и класса бетона

                            if (columnMaterialName == "B20")
                            {
                                overlapLength = StretchingOverlapLengthsB20A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B25")
                            {
                                overlapLength = StretchingOverlapLengthsB25A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B30")
                            {
                                overlapLength = StretchingOverlapLengthsB30A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B35")
                            {
                                overlapLength = StretchingOverlapLengthsB35A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B40")
                            {
                                overlapLength = StretchingOverlapLengthsB40A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "20")
                            {
                                overlapLength = StretchingOverlapLengthsB20A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "25")
                            {
                                overlapLength = StretchingOverlapLengthsB25A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "30")
                            {
                                overlapLength = StretchingOverlapLengthsB30A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "35")
                            {
                                overlapLength = StretchingOverlapLengthsB35A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "40")
                            {
                                overlapLength = StretchingOverlapLengthsB40A400(columnRebarDiamParamDouble);
                            }

                            if (overlapLength == 0)
                            {
                                TaskDialog.Show("Revit", "Материал колонны некорректен!");
                                return Result.Cancelled;
                            }

                            //Определение длины анкеровки из диаметра арматуры и класса бетона

                            if (floorMaterialName == "B20")
                            {
                                anchorageLength = StretchingAnchorageLengthsB20A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B25")
                            {
                                anchorageLength = StretchingAnchorageLengthsB25A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B30")
                            {
                                anchorageLength = StretchingAnchorageLengthsB30A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B35")
                            {
                                anchorageLength = StretchingAnchorageLengthsB35A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B40")
                            {
                                anchorageLength = StretchingAnchorageLengthsB40A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "20")
                            {
                                anchorageLength = StretchingAnchorageLengthsB20A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "25")
                            {
                                anchorageLength = StretchingAnchorageLengthsB25A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "30")
                            {
                                anchorageLength = StretchingAnchorageLengthsB30A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "35")
                            {
                                anchorageLength = StretchingAnchorageLengthsB35A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "40")
                            {
                                anchorageLength = StretchingAnchorageLengthsB40A400(columnRebarDiamParamDouble);
                            }

                            if (anchorageLength == 0)
                            {
                                TaskDialog.Show("Revit", "Материал фундаментной плиты некорректен!");
                                return Result.Cancelled;
                            }
                        }

                        //Длина, если автоподбор и сжатие A500
                        if (outletSizesCheckedButtonName == "radioButton_Auto"
                            & forceTypeCheckedButtonName == "radioButton_Compression"
                            & rebarClassNumberString == "400")
                        {
                            //Определение длины нахлеста из диаметра арматуры и класса бетона

                            if (columnMaterialName == "B20")
                            {
                                overlapLength = CompressionOverlapLengthsB20A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B25")
                            {
                                overlapLength = CompressionOverlapLengthsB25A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B30")
                            {
                                overlapLength = CompressionOverlapLengthsB30A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B35")
                            {
                                overlapLength = CompressionOverlapLengthsB35A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "B40")
                            {
                                overlapLength = CompressionOverlapLengthsB40A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "20")
                            {
                                overlapLength = CompressionOverlapLengthsB20A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "25")
                            {
                                overlapLength = CompressionOverlapLengthsB25A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "30")
                            {
                                overlapLength = CompressionOverlapLengthsB30A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "35")
                            {
                                overlapLength = CompressionOverlapLengthsB35A400(columnRebarDiamParamDouble);
                            }
                            else if (columnMaterialName == "40")
                            {
                                overlapLength = CompressionOverlapLengthsB40A400(columnRebarDiamParamDouble);
                            }

                            if (overlapLength == 0)
                            {
                                TaskDialog.Show("Revit", "Материал колонны некорректен!");
                                return Result.Cancelled;
                            }

                            //Определение длины анкеровки из диаметра арматуры и класса бетона

                            if (floorMaterialName == "B20")
                            {
                                anchorageLength = CompressionAnchorageLengthsB20A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B25")
                            {
                                anchorageLength = CompressionAnchorageLengthsB25A400(columnRebarDiamParamDouble);
                            }

                            else if (floorMaterialName == "B30")
                            {
                                anchorageLength = CompressionAnchorageLengthsB30A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B35")
                            {
                                anchorageLength = CompressionAnchorageLengthsB35A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "B40")
                            {
                                anchorageLength = CompressionAnchorageLengthsB40A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "20")
                            {
                                anchorageLength = CompressionAnchorageLengthsB20A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "25")
                            {
                                anchorageLength = CompressionAnchorageLengthsB25A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "30")
                            {
                                anchorageLength = CompressionAnchorageLengthsB30A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "35")
                            {
                                anchorageLength = CompressionAnchorageLengthsB35A400(columnRebarDiamParamDouble);
                            }
                            else if (floorMaterialName == "40")
                            {
                                anchorageLength = CompressionAnchorageLengthsB40A400(columnRebarDiamParamDouble);
                            }

                            if (anchorageLength == 0)
                            {
                                TaskDialog.Show("Revit", "Материал фундаментной плиты некорректен!");
                                return Result.Cancelled;
                            }
                        }


                        //Длина, если указывать вручную
                        else if (outletSizesCheckedButtonName == "radioButton_SetValue")
                        {
                            overlapLength = manualOverlapLength;
                            anchorageLength = manualAnchorageLength;
                        }

                        //Определение максимальной анкеровки
                        if (maxAnchoringLength < anchorageLength)
                        {
                            maxAnchoringLength = anchorageLength;
                        }

                        //Определение максимального диаметра
                        if (maxColumnRebarDiamParamDouble < columnRebarDiamParamDouble)
                        {
                            maxColumnRebarDiamParamDouble = columnRebarDiamParamDouble;
                        }

                        //Проверка длины анкеровки и толщины плиты
                        bool AnchoringGreaterFloorThickness = anchorageLength > mySelFloorThickness;

                        //Если нахлест растяжение. Анкеровка больше толщины плиты 
                        if (AnchoringGreaterFloorThickness == true & mySelFloorTopElevation == Math.Round(columnRebarFirstPoint.Z, 6) & forceTypeCheckedButtonName == "radioButton_Stretching")
                        {
                            //Точки для построения стержня
                            XYZ rebar_p1 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z + overlapLength, 6));
                            XYZ rebar_p2 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z - mySelFloorThickness + offsetFromSlabBottom, 6));
                            XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + (anchorageLength - (mySelFloorThickness - offsetFromSlabBottom)), 6), Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z, 6));
                            ////Кривые стержня
                            List<Curve> myFloorRebarOutletCurves = new List<Curve>();

                            Curve myFloorRebarOutletLine1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                            myFloorRebarOutletCurves.Add(myFloorRebarOutletLine1);
                            Curve myFloorRebarOutletLine2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                            myFloorRebarOutletCurves.Add(myFloorRebarOutletLine2);

                            Rebar floorRebarOutlet = Rebar.CreateFromCurvesAndShape(doc
                                , myrebarShapeAnchorageOutletsGreaterFloorThickness
                                , columnRebarType, null, null, mySelFloor
                                , mainRebarNormal
                                , myFloorRebarOutletCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            rebarIdCollection.Add(floorRebarOutlet.Id);

                            XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                            XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                            Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);

                            if (minXminY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(rebarOffset, rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -135 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (minXmaxY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(rebarOffset, -rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 135 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (maxXmaxY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(-rebarOffset, -rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 45 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (maxXminY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(-rebarOffset, rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -45 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (minXminY != columnRebar.Id & minXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X < columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ translation = new XYZ(columnRebarDiamParamDouble, 0, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            else if (maxXminY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X > columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {
                                XYZ translation = new XYZ(-columnRebarDiamParamDouble, 0, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            else if (minXminY != columnRebar.Id & maxXminY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y < columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -90 * (Math.PI / 180));
                                XYZ translation = new XYZ(0, columnRebarDiamParamDouble, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            else if (minXmaxY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y > columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 90 * (Math.PI / 180));
                                XYZ translation = new XYZ(0, -columnRebarDiamParamDouble, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            OverlapOrWelding = "StretchOverlap";
                        }

                        //Если нахлест растяжение. Анкеровка меньше или ровна толщине плиты 
                        else if (AnchoringGreaterFloorThickness == false & mySelFloorTopElevation == Math.Round(columnRebarFirstPoint.Z, 6) & forceTypeCheckedButtonName == "radioButton_Stretching")
                        {
                            //Точки для построения стержня
                            XYZ rebar_p1 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z + overlapLength, 6));
                            XYZ rebar_p2 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z - anchorageLength, 6));


                            ////Кривые стержня
                            List<Curve> myFloorRebarOutletCurves = new List<Curve>();

                            Curve myFloorRebarOutletLine1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                            myFloorRebarOutletCurves.Add(myFloorRebarOutletLine1);

                            Rebar floorRebarOutlet = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , columnRebarType, null, null, mySelFloor
                                , mainRebarNormal
                                , myFloorRebarOutletCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            rebarIdCollection.Add(floorRebarOutlet.Id);

                            XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                            XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                            Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);

                            if (minXminY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(rebarOffset, rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -135 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (minXmaxY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(rebarOffset, -rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 135 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (maxXmaxY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(-rebarOffset, -rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 45 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (maxXminY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(-rebarOffset, rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -45 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (minXminY != columnRebar.Id & minXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X < columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ translation = new XYZ(columnRebarDiamParamDouble, 0, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            else if (maxXminY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X > columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {
                                XYZ translation = new XYZ(-columnRebarDiamParamDouble, 0, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            else if (minXminY != columnRebar.Id & maxXminY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y < columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -90 * (Math.PI / 180));
                                XYZ translation = new XYZ(0, columnRebarDiamParamDouble, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            else if (minXmaxY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y > columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 90 * (Math.PI / 180));
                                XYZ translation = new XYZ(0, -columnRebarDiamParamDouble, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            OverlapOrWelding = "StretchOverlap";
                        }

                        //Если сварка растяжение. Анкеровка больше толщины плиты 
                        else if (AnchoringGreaterFloorThickness == true & mySelFloorTopElevation != Math.Round(columnRebarFirstPoint.Z, 6) & forceTypeCheckedButtonName == "radioButton_Stretching")
                        {
                            //Точки для построения стержня
                            XYZ rebar_p1 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z, 6));
                            XYZ rebar_p2 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(mySelFloorTopElevation - (mySelFloorThickness - offsetFromSlabBottom), 6));
                            XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + (anchorageLength - (mySelFloorThickness - offsetFromSlabBottom)), 6), Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z, 6));

                            ////Кривые стержня
                            List<Curve> myFloorRebarOutletCurves = new List<Curve>();

                            Curve myFloorRebarOutletLine1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                            myFloorRebarOutletCurves.Add(myFloorRebarOutletLine1);
                            Curve myFloorRebarOutletLine2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                            myFloorRebarOutletCurves.Add(myFloorRebarOutletLine2);

                            Rebar floorRebarOutlet = Rebar.CreateFromCurvesAndShape(doc
                                , myrebarShapeAnchorageOutletsGreaterFloorThickness
                                , columnRebarType, null, null, mySelFloor
                                , mainRebarNormal
                                , myFloorRebarOutletCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            rebarIdCollection.Add(floorRebarOutlet.Id);

                            XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                            XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                            Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);

                            XYZ tubWeldingInstalationPoint = new XYZ(0, 0, 0);
                            if (rebar_p1.Z < 0)
                            {
                                tubWeldingInstalationPoint = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + Math.Abs(mySelFloorTopElevation - mySelFloorTopOffset));
                            }
                            else
                            {
                                tubWeldingInstalationPoint = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z - Math.Abs(mySelFloorTopElevation - mySelFloorTopOffset));
                            }

                            //Установка ванночки для сварки
                            FamilyInstance tubWelding = doc.Create.NewFamilyInstance(tubWeldingInstalationPoint, myTubWeldingSymbol, mySelFloorLevel, StructuralType.NonStructural);
                            tubWelding.LookupParameter("Диаметр стержня").Set(columnRebarDiamParamDouble);

                            rebarIdCollection.Add(tubWelding.Id);

                            if (minXminY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -135 * (Math.PI / 180));
                            }

                            else if (minXmaxY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 135 * (Math.PI / 180));
                            }

                            else if (maxXmaxY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 45 * (Math.PI / 180));
                            }

                            else if (maxXminY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -45 * (Math.PI / 180));
                            }

                            else if (minXminY != columnRebar.Id & minXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X < columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 180 * (Math.PI / 180));

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, rebarElemBarSpacing * i, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            else if (maxXminY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X > columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, -rebarElemBarSpacing * i, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            else if (minXminY != columnRebar.Id & maxXminY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y < columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -90 * (Math.PI / 180));

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(rebarElemBarSpacing * i, 0, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            else if (minXmaxY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y > columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 90 * (Math.PI / 180));

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(-rebarElemBarSpacing * i, 0, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            OverlapOrWelding = "StretchWelding";
                        }

                        //Если сварка растяжение. Анкеровка меньше или равна толщине плиты 
                        else if (AnchoringGreaterFloorThickness == false & mySelFloorTopElevation != Math.Round(columnRebarFirstPoint.Z, 6) & forceTypeCheckedButtonName == "radioButton_Stretching")
                        {
                            //Точки для построения стержня
                            XYZ rebar_p1 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z, 6));
                            XYZ rebar_p2 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(mySelFloorTopElevation - anchorageLength, 6));


                            ////Кривые стержня
                            List<Curve> myFloorRebarOutletCurves = new List<Curve>();

                            Curve myFloorRebarOutletLine1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                            myFloorRebarOutletCurves.Add(myFloorRebarOutletLine1);

                            Rebar floorRebarOutlet = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , columnRebarType, null, null, mySelFloor
                                , mainRebarNormal
                                , myFloorRebarOutletCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            rebarIdCollection.Add(floorRebarOutlet.Id);

                            XYZ tubWeldingInstalationPoint = new XYZ(0, 0, 0);
                            if (rebar_p1.Z < 0)
                            {
                                tubWeldingInstalationPoint = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + Math.Abs(mySelFloorTopElevation - mySelFloorTopOffset));
                            }
                            else
                            {
                                tubWeldingInstalationPoint = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z - Math.Abs(mySelFloorTopElevation - mySelFloorTopOffset));
                            }

                            //Установка ванночки для сварки
                            FamilyInstance tubWelding = doc.Create.NewFamilyInstance(tubWeldingInstalationPoint, myTubWeldingSymbol, mySelFloorLevel, StructuralType.NonStructural);
                            tubWelding.LookupParameter("Диаметр стержня").Set(columnRebarDiamParamDouble);

                            rebarIdCollection.Add(tubWelding.Id);

                            XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                            XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                            Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);

                            if (minXminY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -135 * (Math.PI / 180));
                            }

                            else if (minXmaxY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 135 * (Math.PI / 180));
                            }

                            else if (maxXmaxY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 45 * (Math.PI / 180));
                            }

                            else if (maxXminY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -45 * (Math.PI / 180));
                            }

                            else if (minXminY != columnRebar.Id & minXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X < columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 180 * (Math.PI / 180));

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, rebarElemBarSpacing * i, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            else if (maxXminY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X > columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, -rebarElemBarSpacing * i, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            else if (minXminY != columnRebar.Id & maxXminY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y < columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -90 * (Math.PI / 180));

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(rebarElemBarSpacing * i, 0, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            else if (minXmaxY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y > columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 90 * (Math.PI / 180));

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(-rebarElemBarSpacing * i, 0, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            OverlapOrWelding = "StretchWelding";
                        }

                        //Если нахлест сжатие анкеровка больше толщины плиты.
                        else if (AnchoringGreaterFloorThickness == true & mySelFloorTopElevation == Math.Round(columnRebarFirstPoint.Z, 6) & forceTypeCheckedButtonName == "radioButton_Compression")
                        {
                            double spAnchorageLength = anchorageLength - 5 * columnRebarDiamParamDouble;
                            if (spAnchorageLength > mySelFloorThickness)
                            {
                                TaskDialog.Show("Revit", "Длина анкеровки - 5d стержня > Толщины плиты!" +
                                    "\nУвеличте толщину плиты или уменьшите диаметры основной арматуры колонн");
                                return Result.Cancelled;
                            }

                            else
                            {
                                //Точки для построения стержня
                                XYZ rebar_p1 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z + overlapLength, 6));
                                XYZ rebar_p2 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z - spAnchorageLength, 6));


                                ////Кривые стержня
                                List<Curve> myFloorRebarOutletCurves = new List<Curve>();

                                Curve myFloorRebarOutletLine1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                                myFloorRebarOutletCurves.Add(myFloorRebarOutletLine1);

                                Rebar floorRebarOutlet = Rebar.CreateFromCurvesAndShape(doc
                                    , myRebarShapeAnchorageOutletsLessFloorThickness
                                    , columnRebarType, null, null, mySelFloor
                                    , mainRebarNormal
                                    , myFloorRebarOutletCurves
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                rebarIdCollection.Add(floorRebarOutlet.Id);

                                XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                                XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                                Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);

                                if (minXminY == columnRebar.Id)
                                {
                                    double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                    XYZ translation = new XYZ(rebarOffset, rebarOffset, 0);
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -135 * (Math.PI / 180));
                                    ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                                }

                                else if (minXmaxY == columnRebar.Id)
                                {
                                    double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                    XYZ translation = new XYZ(rebarOffset, -rebarOffset, 0);
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 135 * (Math.PI / 180));
                                    ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                                }

                                else if (maxXmaxY == columnRebar.Id)
                                {
                                    double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                    XYZ translation = new XYZ(-rebarOffset, -rebarOffset, 0);
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 45 * (Math.PI / 180));
                                    ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                                }

                                else if (maxXminY == columnRebar.Id)
                                {
                                    double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                    XYZ translation = new XYZ(-rebarOffset, rebarOffset, 0);
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -45 * (Math.PI / 180));
                                    ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                                }

                                else if (minXminY != columnRebar.Id & minXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X < columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                                {
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 180 * (Math.PI / 180));
                                    XYZ translation = new XYZ(columnRebarDiamParamDouble, 0, 0);
                                    ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                    if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                    {
                                        int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                        double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                        floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                    }
                                }

                                else if (maxXminY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X > columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                                {
                                    XYZ translation = new XYZ(-columnRebarDiamParamDouble, 0, 0);
                                    ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                    if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                    {
                                        int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                        double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                        floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                    }
                                }

                                else if (minXminY != columnRebar.Id & maxXminY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y < columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                                {
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -90 * (Math.PI / 180));
                                    XYZ translation = new XYZ(0, columnRebarDiamParamDouble, 0);
                                    ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                    if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                    {
                                        int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                        double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                        floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                        Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisX, rebar_p1);
                                        List<ElementId> floorRebarOutletIdList = new List<ElementId>();
                                        floorRebarOutletIdList.Add(floorRebarOutlet.Id);
                                        ElementTransformUtils.MirrorElements(doc, floorRebarOutletIdList, plane, false);
                                    }
                                }

                                else if (minXmaxY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y > columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                                {
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 90 * (Math.PI / 180));
                                    XYZ translation = new XYZ(0, -columnRebarDiamParamDouble, 0);
                                    ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                    if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                    {
                                        int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                        double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                        floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                    }
                                }

                                OverlapOrWelding = "CompressionOverlap";
                            }
                        }

                        //Если нахлест сжатие анкеровка меньше или ровна толщины плиты.
                        else if (AnchoringGreaterFloorThickness == false & mySelFloorTopElevation == Math.Round(columnRebarFirstPoint.Z, 6) & forceTypeCheckedButtonName == "radioButton_Compression")
                        {
                            //Точки для построения стержня
                            XYZ rebar_p1 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z + overlapLength, 6));
                            XYZ rebar_p2 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z - anchorageLength, 6));


                            ////Кривые стержня
                            List<Curve> myFloorRebarOutletCurves = new List<Curve>();

                            Curve myFloorRebarOutletLine1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                            myFloorRebarOutletCurves.Add(myFloorRebarOutletLine1);

                            Rebar floorRebarOutlet = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , columnRebarType, null, null, mySelFloor
                                , mainRebarNormal
                                , myFloorRebarOutletCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            rebarIdCollection.Add(floorRebarOutlet.Id);

                            XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                            XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                            Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);

                            if (minXminY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(rebarOffset, rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -135 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (minXmaxY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(rebarOffset, -rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 135 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (maxXmaxY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(-rebarOffset, -rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 45 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (maxXminY == columnRebar.Id)
                            {
                                double rebarOffset = Math.Sin(45 * (Math.PI / 180)) * columnRebarDiamParamDouble;

                                XYZ translation = new XYZ(-rebarOffset, rebarOffset, 0);
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -45 * (Math.PI / 180));
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);
                            }

                            else if (minXminY != columnRebar.Id & minXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X < columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ translation = new XYZ(columnRebarDiamParamDouble, 0, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            else if (maxXminY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X > columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {
                                XYZ translation = new XYZ(-columnRebarDiamParamDouble, 0, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            else if (minXminY != columnRebar.Id & maxXminY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y < columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -90 * (Math.PI / 180));
                                XYZ translation = new XYZ(0, columnRebarDiamParamDouble, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            else if (minXmaxY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y > columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 90 * (Math.PI / 180));
                                XYZ translation = new XYZ(0, -columnRebarDiamParamDouble, 0);
                                ElementTransformUtils.MoveElement(doc, floorRebarOutlet.Id, translation);

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);
                                }
                            }

                            OverlapOrWelding = "CompressionOverlap";
                        }

                        //Если сварка сжатие анкеровка больше толщины плиты.
                        else if (AnchoringGreaterFloorThickness == true & mySelFloorTopElevation != Math.Round(columnRebarFirstPoint.Z, 6) & forceTypeCheckedButtonName == "radioButton_Compression")
                        {
                            double spAnchorageLength = anchorageLength - 5 * columnRebarDiamParamDouble;
                            if (spAnchorageLength > mySelFloorThickness)
                            {
                                TaskDialog.Show("Revit", "Длина анкеровки - 5d стержня > Толщины плиты!" +
                                    "\nУвеличте толщину плиты или уменьшите диаметры основной арматуры колонн");
                                return Result.Cancelled;
                            }

                            else
                            {
                                //Точки для построения стержня
                                XYZ rebar_p1 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z, 6));
                                XYZ rebar_p2 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(mySelFloorTopElevation - spAnchorageLength, 6));

                                ////Кривые стержня
                                List<Curve> myFloorRebarOutletCurves = new List<Curve>();

                                Curve myFloorRebarOutletLine1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                                myFloorRebarOutletCurves.Add(myFloorRebarOutletLine1);

                                Rebar floorRebarOutlet = Rebar.CreateFromCurvesAndShape(doc
                                    , myRebarShapeAnchorageOutletsLessFloorThickness
                                    , columnRebarType, null, null, mySelFloor
                                    , mainRebarNormal
                                    , myFloorRebarOutletCurves
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                rebarIdCollection.Add(floorRebarOutlet.Id);

                                XYZ tubWeldingInstalationPoint = new XYZ(0, 0, 0);
                                if (rebar_p1.Z < 0)
                                {
                                    tubWeldingInstalationPoint = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + Math.Abs(mySelFloorTopElevation - mySelFloorTopOffset));
                                }
                                else
                                {
                                    tubWeldingInstalationPoint = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z - Math.Abs(mySelFloorTopElevation - mySelFloorTopOffset));
                                }

                                //Установка ванночки для сварки
                                FamilyInstance tubWelding = doc.Create.NewFamilyInstance(tubWeldingInstalationPoint, myTubWeldingSymbol, mySelFloorLevel, StructuralType.NonStructural);
                                tubWelding.LookupParameter("Диаметр стержня").Set(columnRebarDiamParamDouble);

                                rebarIdCollection.Add(tubWelding.Id);

                                XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                                XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                                Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);

                                if (minXminY == columnRebar.Id)
                                {
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -135 * (Math.PI / 180));
                                }

                                else if (minXmaxY == columnRebar.Id)
                                {
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 135 * (Math.PI / 180));
                                }

                                else if (maxXmaxY == columnRebar.Id)
                                {
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 45 * (Math.PI / 180));
                                }

                                else if (maxXminY == columnRebar.Id)
                                {
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -45 * (Math.PI / 180));
                                }

                                else if (minXminY != columnRebar.Id & minXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X < columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                                {
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 180 * (Math.PI / 180));

                                    if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                    {
                                        int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                        double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                        floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                        for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                        {
                                            XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, rebarElemBarSpacing * i, 0);
                                            List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                            Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                            rebarIdCollection.Add(newtubWelding.Id);
                                        }
                                    }
                                }

                                else if (maxXminY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X > columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                                {

                                    if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                    {
                                        int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                        double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                        floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                        for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                        {
                                            XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, -rebarElemBarSpacing * i, 0);
                                            List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                            Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                            rebarIdCollection.Add(newtubWelding.Id);
                                        }
                                    }
                                }

                                else if (minXminY != columnRebar.Id & maxXminY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y < columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                                {
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -90 * (Math.PI / 180));

                                    if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                    {
                                        int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                        double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                        floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                        for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                        {
                                            XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(rebarElemBarSpacing * i, 0, 0);
                                            List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                            Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                            rebarIdCollection.Add(newtubWelding.Id);
                                        }
                                    }
                                }

                                else if (minXmaxY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y > columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                                {
                                    ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 90 * (Math.PI / 180));

                                    if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                    {
                                        int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                        double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                        floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                        floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                        for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                        {
                                            XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(-rebarElemBarSpacing * i, 0, 0);
                                            List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                            Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                            rebarIdCollection.Add(newtubWelding.Id);
                                        }
                                    }
                                }

                                OverlapOrWelding = "CompressionWelding";
                            }
                        }

                        //Если сварка сжатие анкеровка меньше толщины плиты.
                        else if (AnchoringGreaterFloorThickness == false & mySelFloorTopElevation != Math.Round(columnRebarFirstPoint.Z, 6) & forceTypeCheckedButtonName == "radioButton_Compression")
                        {
                            //Точки для построения стержня
                            XYZ rebar_p1 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(columnRebarFirstPoint.Z, 6));
                            XYZ rebar_p2 = new XYZ(Math.Round(columnRebarFirstPoint.X + linkOrigin.X, 6), Math.Round(columnRebarFirstPoint.Y + linkOrigin.Y, 6), Math.Round(mySelFloorTopElevation - anchorageLength, 6));


                            ////Кривые стержня
                            List<Curve> myFloorRebarOutletCurves = new List<Curve>();

                            Curve myFloorRebarOutletLine1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                            myFloorRebarOutletCurves.Add(myFloorRebarOutletLine1);

                            Rebar floorRebarOutlet = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , columnRebarType, null, null, mySelFloor
                                , mainRebarNormal
                                , myFloorRebarOutletCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            rebarIdCollection.Add(floorRebarOutlet.Id);

                            XYZ tubWeldingInstalationPoint = new XYZ(0, 0, 0);
                            if (rebar_p1.Z < 0)
                            {
                                tubWeldingInstalationPoint = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + Math.Abs(mySelFloorTopElevation - mySelFloorTopOffset));
                            }
                            else
                            {
                                tubWeldingInstalationPoint = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z - Math.Abs(mySelFloorTopElevation - mySelFloorTopOffset));
                            }

                            //Установка ванночки для сварки
                            FamilyInstance tubWelding = doc.Create.NewFamilyInstance(tubWeldingInstalationPoint, myTubWeldingSymbol, mySelFloorLevel, StructuralType.NonStructural);
                            tubWelding.LookupParameter("Диаметр стержня").Set(columnRebarDiamParamDouble);

                            rebarIdCollection.Add(tubWelding.Id);

                            XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                            XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                            Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);

                            if (minXminY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -135 * (Math.PI / 180));
                            }

                            else if (minXmaxY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 135 * (Math.PI / 180));
                            }

                            else if (maxXmaxY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 45 * (Math.PI / 180));
                            }

                            else if (maxXminY == columnRebar.Id)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -45 * (Math.PI / 180));
                            }

                            else if (minXminY != columnRebar.Id & minXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X < columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 180 * (Math.PI / 180));

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, rebarElemBarSpacing * i, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            else if (maxXminY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.X + linkOrigin.X > columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, -rebarElemBarSpacing * i, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            else if (minXminY != columnRebar.Id & maxXminY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y < columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, -90 * (Math.PI / 180));

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(rebarElemBarSpacing * i, 0, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            else if (minXmaxY != columnRebar.Id & maxXmaxY != columnRebar.Id & columnRebarFirstPoint.Y + linkOrigin.Y > columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer - columnRebarDiamParamDouble)
                            {
                                ElementTransformUtils.RotateElement(doc, floorRebarOutlet.Id, rotateLine, 90 * (Math.PI / 180));

                                if (columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).AsInteger() == 3)
                                {
                                    int rebarElemQuantityOfBars = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
                                    double rebarElemBarSpacing = columnRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).AsDouble();
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                    floorRebarOutlet.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarElemQuantityOfBars);
                                    floorRebarOutlet.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(rebarElemBarSpacing);

                                    for (int i = 1; i < rebarElemQuantityOfBars; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(-rebarElemBarSpacing * i, 0, 0);
                                        List<ElementId> newtubWeldingIdList = ElementTransformUtils.CopyElement(doc, tubWelding.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                        Element newtubWelding = doc.GetElement(newtubWeldingIdList.First());
                                        rebarIdCollection.Add(newtubWelding.Id);
                                    }
                                }
                            }

                            OverlapOrWelding = "CompressionWelding";
                        }
                    }

                    //Хомут
                    //Нормаль для построения хомута
                    XYZ narmalStirrup = new XYZ(0, 0, 1);

                    if (OverlapOrWelding == "StretchOverlap" & forceTypeCheckedButtonName == "radioButton_Stretching" & maxAnchoringLength > mySelFloorThickness)
                    {
                        double rebarAngleOffset = Math.Sin(45 * (Math.PI / 180)) * maxColumnRebarDiamParamDouble;

                        //Точки для построения кривых стержня хомута
                        XYZ rebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam + rebarAngleOffset, 6)
                            , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2 - rebarAngleOffset, 6)
                            , Math.Round(mySelFloorTopElevation - 100 / 304.8, 6));

                        XYZ rebarStirrup_p2 = new XYZ(Math.Round(rebarStirrup_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam - rebarAngleOffset * 2, 6)
                            , Math.Round(rebarStirrup_p1.Y, 6)
                            , Math.Round(rebarStirrup_p1.Z, 6));

                        XYZ rebarStirrup_p3 = new XYZ(Math.Round(rebarStirrup_p2.X, 6)
                            , Math.Round(rebarStirrup_p2.Y - columnSectionHeight + mainRebarCoverLayer * 2 - stirrupRebarDiam - stirrupRebarDiam / 2 + rebarAngleOffset * 2, 6)
                            , Math.Round(rebarStirrup_p2.Z, 6));

                        XYZ rebarStirrup_p4 = new XYZ(Math.Round(rebarStirrup_p3.X - columnSectionWidth + mainRebarCoverLayer * 2 - stirrupRebarDiam + rebarAngleOffset * 2, 6)
                            , Math.Round(rebarStirrup_p3.Y, 6)
                            , Math.Round(rebarStirrup_p3.Z, 6));

                        //Кривые хомута
                        List<Curve> myStirrupCurves = new List<Curve>();

                        Curve Stirrup_line1 = Line.CreateBound(rebarStirrup_p1, rebarStirrup_p2) as Curve;
                        myStirrupCurves.Add(Stirrup_line1);
                        Curve Stirrup_line2 = Line.CreateBound(rebarStirrup_p2, rebarStirrup_p3) as Curve;
                        myStirrupCurves.Add(Stirrup_line2);
                        Curve Stirrup_line3 = Line.CreateBound(rebarStirrup_p3, rebarStirrup_p4) as Curve;
                        myStirrupCurves.Add(Stirrup_line3);
                        Curve Stirrup_line4 = Line.CreateBound(rebarStirrup_p4, rebarStirrup_p1) as Curve;
                        myStirrupCurves.Add(Stirrup_line4);

                        //Построение хомута
                        Rebar outletsStirrup_1 = Rebar.CreateFromCurvesAndShape(doc
                            , myStirrupRebarShape
                            , myStirrupBarTape
                            , myRebarHookType
                            , myRebarHookType
                            , mySelFloor
                            , narmalStirrup
                            , myStirrupCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        //Копирование хомута
                        XYZ pointStirrupInstallation1 = new XYZ(0, 0, -(mySelFloorThickness - (offsetFromSlabBottom + 200 / 304.8)));
                        XYZ pointStirrupInstallation2 = new XYZ(0, 0, -((mySelFloorThickness - offsetFromSlabBottom - 200 / 304.8)) / 2);
                        List<ElementId> outletsStirrup_2 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation1) as List<ElementId>;
                        List<ElementId> outletsStirrup_3 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation2) as List<ElementId>;

                        rebarIdCollection.Add(outletsStirrup_1.Id);
                        rebarIdCollection.Add(doc.GetElement(outletsStirrup_2.First()).Id);
                        rebarIdCollection.Add(doc.GetElement(outletsStirrup_3.First()).Id);
                    }

                    else if (OverlapOrWelding == "StretchOverlap" & forceTypeCheckedButtonName == "radioButton_Stretching" & maxAnchoringLength < mySelFloorThickness)
                    {
                        double rebarAngleOffset = Math.Sin(45 * (Math.PI / 180)) * maxColumnRebarDiamParamDouble;

                        //Точки для построения кривых стержня хомута
                        XYZ rebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam + rebarAngleOffset, 6)
                            , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2 - rebarAngleOffset, 6)
                            , Math.Round(mySelFloorTopElevation - 100 / 304.8, 6));

                        XYZ rebarStirrup_p2 = new XYZ(Math.Round(rebarStirrup_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam - rebarAngleOffset * 2, 6)
                            , Math.Round(rebarStirrup_p1.Y, 6)
                            , Math.Round(rebarStirrup_p1.Z, 6));

                        XYZ rebarStirrup_p3 = new XYZ(Math.Round(rebarStirrup_p2.X, 6)
                            , Math.Round(rebarStirrup_p2.Y - columnSectionHeight + mainRebarCoverLayer * 2 - stirrupRebarDiam - stirrupRebarDiam / 2 + rebarAngleOffset * 2, 6)
                            , Math.Round(rebarStirrup_p2.Z, 6));

                        XYZ rebarStirrup_p4 = new XYZ(Math.Round(rebarStirrup_p3.X - columnSectionWidth + mainRebarCoverLayer * 2 - stirrupRebarDiam + rebarAngleOffset * 2, 6)
                            , Math.Round(rebarStirrup_p3.Y, 6)
                            , Math.Round(rebarStirrup_p3.Z, 6));

                        //Кривые хомута
                        List<Curve> myStirrupCurves = new List<Curve>();

                        Curve Stirrup_line1 = Line.CreateBound(rebarStirrup_p1, rebarStirrup_p2) as Curve;
                        myStirrupCurves.Add(Stirrup_line1);
                        Curve Stirrup_line2 = Line.CreateBound(rebarStirrup_p2, rebarStirrup_p3) as Curve;
                        myStirrupCurves.Add(Stirrup_line2);
                        Curve Stirrup_line3 = Line.CreateBound(rebarStirrup_p3, rebarStirrup_p4) as Curve;
                        myStirrupCurves.Add(Stirrup_line3);
                        Curve Stirrup_line4 = Line.CreateBound(rebarStirrup_p4, rebarStirrup_p1) as Curve;
                        myStirrupCurves.Add(Stirrup_line4);

                        //Построение хомута
                        Rebar outletsStirrup_1 = Rebar.CreateFromCurvesAndShape(doc
                            , myStirrupRebarShape
                            , myStirrupBarTape
                            , myRebarHookType
                            , myRebarHookType
                            , mySelFloor
                            , narmalStirrup
                            , myStirrupCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        //Копирование хомута
                        XYZ pointStirrupInstallation1 = new XYZ(0, 0, -(maxAnchoringLength - 150 / 304.8));
                        XYZ pointStirrupInstallation2 = new XYZ(0, 0, -((maxAnchoringLength - 150 / 304.8) / 2));
                        List<ElementId> outletsStirrup_2 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation1) as List<ElementId>;
                        List<ElementId> outletsStirrup_3 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation2) as List<ElementId>;

                        rebarIdCollection.Add(outletsStirrup_1.Id);
                        rebarIdCollection.Add(outletsStirrup_2.First());
                        rebarIdCollection.Add(outletsStirrup_3.First());
                    }

                    else if (OverlapOrWelding == "StretchWelding" & forceTypeCheckedButtonName == "radioButton_Stretching" & maxAnchoringLength > mySelFloorThickness)
                    {
                        //Точки для построения кривых стержня хомута
                        XYZ rebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam, 6)
                            , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2, 6)
                            , Math.Round(mySelFloorTopElevation - 100 / 304.8, 6));

                        XYZ rebarStirrup_p2 = new XYZ(Math.Round(rebarStirrup_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam, 6)
                            , Math.Round(rebarStirrup_p1.Y, 6)
                            , Math.Round(rebarStirrup_p1.Z, 6));

                        XYZ rebarStirrup_p3 = new XYZ(Math.Round(rebarStirrup_p2.X, 6)
                            , Math.Round(rebarStirrup_p2.Y - columnSectionHeight + mainRebarCoverLayer * 2 - stirrupRebarDiam - stirrupRebarDiam / 2, 6)
                            , Math.Round(rebarStirrup_p2.Z, 6));

                        XYZ rebarStirrup_p4 = new XYZ(Math.Round(rebarStirrup_p3.X - columnSectionWidth + mainRebarCoverLayer * 2 - stirrupRebarDiam, 6)
                            , Math.Round(rebarStirrup_p3.Y, 6)
                            , Math.Round(rebarStirrup_p3.Z, 6));

                        //Кривые хомута
                        List<Curve> myStirrupCurves = new List<Curve>();

                        Curve Stirrup_line1 = Line.CreateBound(rebarStirrup_p1, rebarStirrup_p2) as Curve;
                        myStirrupCurves.Add(Stirrup_line1);
                        Curve Stirrup_line2 = Line.CreateBound(rebarStirrup_p2, rebarStirrup_p3) as Curve;
                        myStirrupCurves.Add(Stirrup_line2);
                        Curve Stirrup_line3 = Line.CreateBound(rebarStirrup_p3, rebarStirrup_p4) as Curve;
                        myStirrupCurves.Add(Stirrup_line3);
                        Curve Stirrup_line4 = Line.CreateBound(rebarStirrup_p4, rebarStirrup_p1) as Curve;
                        myStirrupCurves.Add(Stirrup_line4);

                        //Построение хомута
                        Rebar outletsStirrup_1 = Rebar.CreateFromCurvesAndShape(doc
                            , myStirrupRebarShape
                            , myStirrupBarTape
                            , myRebarHookType
                            , myRebarHookType
                            , mySelFloor
                            , narmalStirrup
                            , myStirrupCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        //Копирование хомута
                        XYZ pointStirrupInstallation1 = new XYZ(0, 0, -(mySelFloorThickness - (offsetFromSlabBottom + 200 / 304.8)));
                        XYZ pointStirrupInstallation2 = new XYZ(0, 0, -((mySelFloorThickness - offsetFromSlabBottom - 200 / 304.8)) / 2);
                        List<ElementId> outletsStirrup_2 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation1) as List<ElementId>;
                        List<ElementId> outletsStirrup_3 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation2) as List<ElementId>;

                        rebarIdCollection.Add(outletsStirrup_1.Id);
                        rebarIdCollection.Add(outletsStirrup_2.First());
                        rebarIdCollection.Add(outletsStirrup_3.First());
                    }

                    else if (OverlapOrWelding == "StretchWelding" & forceTypeCheckedButtonName == "radioButton_Stretching" & maxAnchoringLength < mySelFloorThickness)
                    {
                        //Точки для построения кривых стержня хомута
                        XYZ rebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam, 6)
                            , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2, 6)
                            , Math.Round(columnOrigin.Z - 100 / 304.8, 6));

                        XYZ rebarStirrup_p2 = new XYZ(Math.Round(rebarStirrup_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam, 6)
                            , Math.Round(rebarStirrup_p1.Y, 6)
                            , Math.Round(rebarStirrup_p1.Z, 6));

                        XYZ rebarStirrup_p3 = new XYZ(Math.Round(rebarStirrup_p2.X, 6)
                            , Math.Round(rebarStirrup_p2.Y - columnSectionHeight + mainRebarCoverLayer * 2 - stirrupRebarDiam - stirrupRebarDiam / 2, 6)
                            , Math.Round(rebarStirrup_p2.Z, 6));

                        XYZ rebarStirrup_p4 = new XYZ(Math.Round(rebarStirrup_p3.X - columnSectionWidth + mainRebarCoverLayer * 2 - stirrupRebarDiam, 6)
                            , Math.Round(rebarStirrup_p3.Y, 6)
                            , Math.Round(rebarStirrup_p3.Z, 6));

                        //Кривые хомута
                        List<Curve> myStirrupCurves = new List<Curve>();

                        Curve Stirrup_line1 = Line.CreateBound(rebarStirrup_p1, rebarStirrup_p2) as Curve;
                        myStirrupCurves.Add(Stirrup_line1);
                        Curve Stirrup_line2 = Line.CreateBound(rebarStirrup_p2, rebarStirrup_p3) as Curve;
                        myStirrupCurves.Add(Stirrup_line2);
                        Curve Stirrup_line3 = Line.CreateBound(rebarStirrup_p3, rebarStirrup_p4) as Curve;
                        myStirrupCurves.Add(Stirrup_line3);
                        Curve Stirrup_line4 = Line.CreateBound(rebarStirrup_p4, rebarStirrup_p1) as Curve;
                        myStirrupCurves.Add(Stirrup_line4);

                        //Построение хомута
                        Rebar outletsStirrup_1 = Rebar.CreateFromCurvesAndShape(doc
                            , myStirrupRebarShape
                            , myStirrupBarTape
                            , myRebarHookType
                            , myRebarHookType
                            , mySelFloor
                            , narmalStirrup
                            , myStirrupCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        //Копирование хомута
                        XYZ pointStirrupInstallation1 = new XYZ(0, 0, -(maxAnchoringLength - 150 / 304.8));
                        XYZ pointStirrupInstallation2 = new XYZ(0, 0, -((maxAnchoringLength - 150 / 304.8) / 2));
                        List<ElementId> outletsStirrup_2 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation1) as List<ElementId>;
                        List<ElementId> outletsStirrup_3 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation2) as List<ElementId>;

                        rebarIdCollection.Add(outletsStirrup_1.Id);
                        rebarIdCollection.Add(outletsStirrup_2.First());
                        rebarIdCollection.Add(outletsStirrup_3.First());
                    }

                    else if (OverlapOrWelding == "CompressionOverlap" & forceTypeCheckedButtonName == "radioButton_Compression" & maxAnchoringLength > mySelFloorThickness)
                    {
                        double spMaxAnchorageLength = maxAnchoringLength - 5 * maxColumnRebarDiamParamDouble;
                        if (spMaxAnchorageLength > mySelFloorThickness)
                        {
                            TaskDialog.Show("Revit", "Длина анкеровки - 5d стержня > Толщины плиты!" +
                                "\nУвеличте толщину плиты или уменьшите диаметры основной арматуры колонн");
                            return Result.Cancelled;
                        }

                        else
                        {
                            //Исходные точки для построения обвязочных стержней
                            double rebarAngleOffset = Math.Sin(45 * (Math.PI / 180)) * maxColumnRebarDiamParamDouble;
                            double diameterСheck = maxColumnRebarDiamParamDouble / 2;

                            List<RebarBarType> rebarTypeForAdditionalBarList = new FilteredElementCollector(doc)
                                .OfClass(typeof(RebarBarType))
                                .Cast<RebarBarType>().Where(rbt => rbt.Name.Split(' ').ToArray().Contains("A500")).Where(rbt => rbt.Name.Split(' ').ToArray().Length == 2)
                                .ToList();
                            List<RebarBarType> newRebarTypeForAdditionalBarList = rebarTypeForAdditionalBarList
                                .OrderBy(rbt => rbt.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble())
                                .ToList();
                            RebarBarType rebarTypeForAdditionalBar = newRebarTypeForAdditionalBarList
                                .FirstOrDefault(rbt => rbt.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble() >= diameterСheck);

                            double additionalBarDiam = rebarTypeForAdditionalBar.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();

                            XYZ rebarStirrup_p1_Basic = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - additionalBarDiam / 2 + rebarAngleOffset, 6)
                                , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + additionalBarDiam / 2 - rebarAngleOffset, 6)
                                , Math.Round(mySelFloorTopElevation - 100 / 304.8, 6));

                            XYZ rebarStirrup_p2_Basic = new XYZ(Math.Round(rebarStirrup_p1_Basic.X + columnSectionWidth - mainRebarCoverLayer * 2 + additionalBarDiam - rebarAngleOffset * 2, 6)
                                , Math.Round(rebarStirrup_p1_Basic.Y, 6)
                                , Math.Round(rebarStirrup_p1_Basic.Z, 6));

                            XYZ rebarStirrup_p3_Basic = new XYZ(Math.Round(rebarStirrup_p2_Basic.X, 6)
                                , Math.Round(rebarStirrup_p2_Basic.Y - columnSectionHeight + mainRebarCoverLayer * 2 - additionalBarDiam + rebarAngleOffset * 2, 6)
                                , Math.Round(rebarStirrup_p2_Basic.Z, 6));

                            XYZ rebarStirrup_p4_Basic = new XYZ(Math.Round(rebarStirrup_p3_Basic.X - columnSectionWidth + mainRebarCoverLayer * 2 - additionalBarDiam + rebarAngleOffset * 2, 6)
                                , Math.Round(rebarStirrup_p3_Basic.Y, 6)
                                , Math.Round(rebarStirrup_p3_Basic.Z, 6));

                            //Точки стержня 1
                            XYZ firstBar_p1 = new XYZ(rebarStirrup_p1_Basic.X - (25 / 304.8), rebarStirrup_p1_Basic.Y, rebarStirrup_p1_Basic.Z);
                            XYZ firstBar_p2 = new XYZ(rebarStirrup_p2_Basic.X + (25 / 304.8), rebarStirrup_p2_Basic.Y, rebarStirrup_p1_Basic.Z);

                            //Кривые стержня1
                            List<Curve> myFloorRebarOutletCurves1 = new List<Curve>();

                            Curve myFloorRebarOutletLine1 = Line.CreateBound(firstBar_p1, firstBar_p2) as Curve;
                            myFloorRebarOutletCurves1.Add(myFloorRebarOutletLine1);

                            //Точки стержня 2
                            XYZ secondBar_p1 = new XYZ(rebarStirrup_p2_Basic.X, rebarStirrup_p2_Basic.Y + (25 / 304.8), rebarStirrup_p2_Basic.Z + additionalBarDiam);
                            XYZ secondBar_p2 = new XYZ(rebarStirrup_p3_Basic.X, rebarStirrup_p3_Basic.Y - (25 / 304.8), rebarStirrup_p3_Basic.Z + additionalBarDiam);

                            //Кривые стержня2
                            List<Curve> myFloorRebarOutletCurves2 = new List<Curve>();

                            Curve myFloorRebarOutletLine2 = Line.CreateBound(secondBar_p1, secondBar_p2) as Curve;
                            myFloorRebarOutletCurves2.Add(myFloorRebarOutletLine2);

                            //Точки стержня 3
                            XYZ thirdBar_p1 = new XYZ(rebarStirrup_p3_Basic.X + (25 / 304.8), rebarStirrup_p3_Basic.Y, rebarStirrup_p3_Basic.Z);
                            XYZ thirdBar_p2 = new XYZ(rebarStirrup_p4_Basic.X - (25 / 304.8), rebarStirrup_p4_Basic.Y, rebarStirrup_p4_Basic.Z);

                            //Кривые стержня3
                            List<Curve> myFloorRebarOutletCurves3 = new List<Curve>();

                            Curve myFloorRebarOutletLine3 = Line.CreateBound(thirdBar_p1, thirdBar_p2) as Curve;
                            myFloorRebarOutletCurves3.Add(myFloorRebarOutletLine3);

                            //Точки стержня 4
                            XYZ fourthBar_p1 = new XYZ(rebarStirrup_p4_Basic.X, rebarStirrup_p4_Basic.Y - (25 / 304.8), rebarStirrup_p4_Basic.Z + additionalBarDiam);
                            XYZ fourthBar_p2 = new XYZ(rebarStirrup_p1_Basic.X, rebarStirrup_p1_Basic.Y + (25 / 304.8), rebarStirrup_p1_Basic.Z + additionalBarDiam);

                            //Кривые стержня4
                            List<Curve> myFloorRebarOutletCurves4 = new List<Curve>();

                            Curve myFloorRebarOutletLine4 = Line.CreateBound(fourthBar_p1, fourthBar_p2) as Curve;
                            myFloorRebarOutletCurves4.Add(myFloorRebarOutletLine4);

                            Rebar floorRebarOutlet1 = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , rebarTypeForAdditionalBar //Заменить тип стержня диаметр осн/2
                                , null
                                , null
                                , mySelFloor
                                , narmalStirrup
                                , myFloorRebarOutletCurves1
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            Rebar floorRebarOutlet2 = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , rebarTypeForAdditionalBar //Заменить тип стержня диаметр осн/2
                                , null
                                , null
                                , mySelFloor
                                , narmalStirrup
                                , myFloorRebarOutletCurves2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            Rebar floorRebarOutlet3 = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , rebarTypeForAdditionalBar //Заменить тип стержня диаметр осн/2
                                , null
                                , null
                                , mySelFloor
                                , narmalStirrup
                                , myFloorRebarOutletCurves3
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            Rebar floorRebarOutlet4 = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , rebarTypeForAdditionalBar //Заменить тип стержня диаметр осн/2
                                , null
                                , null
                                , mySelFloor
                                , narmalStirrup
                                , myFloorRebarOutletCurves4
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            rebarIdCollection.Add(floorRebarOutlet1.Id);
                            rebarIdCollection.Add(floorRebarOutlet2.Id);
                            rebarIdCollection.Add(floorRebarOutlet3.Id);
                            rebarIdCollection.Add(floorRebarOutlet4.Id);

                            //Копирование стержней
                            XYZ pointStirrupInstallation1 = new XYZ(0, 0, -(mySelFloorThickness - (offsetFromSlabBottom + 200 / 304.8)));
                            XYZ pointStirrupInstallation2 = new XYZ(0, 0, -((mySelFloorThickness - offsetFromSlabBottom - 200 / 304.8)) / 2);

                            List<ElementId> rebarId_1 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet1.Id, pointStirrupInstallation1) as List<ElementId>;
                            List<ElementId> rebarId_2 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet1.Id, pointStirrupInstallation2) as List<ElementId>;

                            List<ElementId> rebarId_3 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet2.Id, pointStirrupInstallation1) as List<ElementId>;
                            List<ElementId> rebarId_4 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet2.Id, pointStirrupInstallation2) as List<ElementId>;

                            List<ElementId> rebarId_5 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet3.Id, pointStirrupInstallation1) as List<ElementId>;
                            List<ElementId> rebarId_6 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet3.Id, pointStirrupInstallation2) as List<ElementId>;

                            List<ElementId> rebarId_7 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet4.Id, pointStirrupInstallation1) as List<ElementId>;
                            List<ElementId> rebarId_8 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet4.Id, pointStirrupInstallation2) as List<ElementId>;

                            rebarIdCollection.Add(rebarId_1.First());
                            rebarIdCollection.Add(rebarId_2.First());
                            rebarIdCollection.Add(rebarId_3.First());
                            rebarIdCollection.Add(rebarId_4.First());
                            rebarIdCollection.Add(rebarId_5.First());
                            rebarIdCollection.Add(rebarId_6.First());
                            rebarIdCollection.Add(rebarId_7.First());
                            rebarIdCollection.Add(rebarId_8.First());
                        }
                    }

                    else if (OverlapOrWelding == "CompressionOverlap" & forceTypeCheckedButtonName == "radioButton_Compression" & maxAnchoringLength < mySelFloorThickness)
                    {
                        double rebarAngleOffset = Math.Sin(45 * (Math.PI / 180)) * maxColumnRebarDiamParamDouble;

                        //Точки для построения кривых стержня хомута
                        XYZ rebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam + rebarAngleOffset, 6)
                            , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2 - rebarAngleOffset, 6)
                            , Math.Round(mySelFloorTopElevation - 100 / 304.8, 6));

                        XYZ rebarStirrup_p2 = new XYZ(Math.Round(rebarStirrup_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam - rebarAngleOffset * 2, 6)
                            , Math.Round(rebarStirrup_p1.Y, 6)
                            , Math.Round(rebarStirrup_p1.Z, 6));

                        XYZ rebarStirrup_p3 = new XYZ(Math.Round(rebarStirrup_p2.X, 6)
                            , Math.Round(rebarStirrup_p2.Y - columnSectionHeight + mainRebarCoverLayer * 2 - stirrupRebarDiam - stirrupRebarDiam / 2 + rebarAngleOffset * 2, 6)
                            , Math.Round(rebarStirrup_p2.Z, 6));

                        XYZ rebarStirrup_p4 = new XYZ(Math.Round(rebarStirrup_p3.X - columnSectionWidth + mainRebarCoverLayer * 2 - stirrupRebarDiam + rebarAngleOffset * 2, 6)
                            , Math.Round(rebarStirrup_p3.Y, 6)
                            , Math.Round(rebarStirrup_p3.Z, 6));

                        //Кривые хомута
                        List<Curve> myStirrupCurves = new List<Curve>();

                        Curve Stirrup_line1 = Line.CreateBound(rebarStirrup_p1, rebarStirrup_p2) as Curve;
                        myStirrupCurves.Add(Stirrup_line1);
                        Curve Stirrup_line2 = Line.CreateBound(rebarStirrup_p2, rebarStirrup_p3) as Curve;
                        myStirrupCurves.Add(Stirrup_line2);
                        Curve Stirrup_line3 = Line.CreateBound(rebarStirrup_p3, rebarStirrup_p4) as Curve;
                        myStirrupCurves.Add(Stirrup_line3);
                        Curve Stirrup_line4 = Line.CreateBound(rebarStirrup_p4, rebarStirrup_p1) as Curve;
                        myStirrupCurves.Add(Stirrup_line4);

                        //Построение хомута
                        Rebar outletsStirrup_1 = Rebar.CreateFromCurvesAndShape(doc
                            , myStirrupRebarShape
                            , myStirrupBarTape
                            , myRebarHookType
                            , myRebarHookType
                            , mySelFloor
                            , narmalStirrup
                            , myStirrupCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        //Копирование хомута
                        XYZ pointStirrupInstallation1 = new XYZ(0, 0, -(maxAnchoringLength - 150 / 304.8));
                        XYZ pointStirrupInstallation2 = new XYZ(0, 0, -((maxAnchoringLength - 150 / 304.8) / 2));
                        List<ElementId> outletsStirrup_2 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation1) as List<ElementId>;
                        List<ElementId> outletsStirrup_3 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation2) as List<ElementId>;

                        rebarIdCollection.Add(outletsStirrup_1.Id);
                        rebarIdCollection.Add(outletsStirrup_2.First());
                        rebarIdCollection.Add(outletsStirrup_3.First());
                    }

                    else if (OverlapOrWelding == "CompressionWelding" & forceTypeCheckedButtonName == "radioButton_Compression" & maxAnchoringLength > mySelFloorThickness)
                    {
                        double spMaxAnchorageLength = maxAnchoringLength - 5 * maxColumnRebarDiamParamDouble;
                        if (spMaxAnchorageLength > mySelFloorThickness)
                        {
                            TaskDialog.Show("Revit", "Длина анкеровки - 5d стержня > Толщины плиты!" +
                                "\nУвеличте толщину плиты или уменьшите диаметры основной арматуры колонн");
                            return Result.Cancelled;
                        }

                        else
                        {
                            //Исходные точки для построения обвязочных стержней
                            double diameterСheck = maxColumnRebarDiamParamDouble / 2;

                            List<RebarBarType> rebarTypeForAdditionalBarList = new FilteredElementCollector(doc)
                                .OfClass(typeof(RebarBarType))
                                .Cast<RebarBarType>().Where(rbt => rbt.Name.Split(' ').ToArray().Contains("A500")).Where(rbt => rbt.Name.Split(' ').ToArray().Length == 2)
                                .ToList();
                            List<RebarBarType> newRebarTypeForAdditionalBarList = rebarTypeForAdditionalBarList
                                .OrderBy(rbt => rbt.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble())
                                .ToList();
                            RebarBarType rebarTypeForAdditionalBar = newRebarTypeForAdditionalBarList
                                .FirstOrDefault(rbt => rbt.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble() >= diameterСheck);

                            double additionalBarDiam = rebarTypeForAdditionalBar.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();

                            XYZ rebarStirrup_p1_Basic = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - additionalBarDiam / 2, 6)
                                , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + additionalBarDiam / 2, 6)
                                , Math.Round(mySelFloorTopElevation - 100 / 304.8, 6));

                            XYZ rebarStirrup_p2_Basic = new XYZ(Math.Round(rebarStirrup_p1_Basic.X + columnSectionWidth - mainRebarCoverLayer * 2 + additionalBarDiam, 6)
                                , Math.Round(rebarStirrup_p1_Basic.Y, 6)
                                , Math.Round(rebarStirrup_p1_Basic.Z, 6));

                            XYZ rebarStirrup_p3_Basic = new XYZ(Math.Round(rebarStirrup_p2_Basic.X, 6)
                                , Math.Round(rebarStirrup_p2_Basic.Y - columnSectionHeight + mainRebarCoverLayer * 2 - additionalBarDiam, 6)
                                , Math.Round(rebarStirrup_p2_Basic.Z, 6));

                            XYZ rebarStirrup_p4_Basic = new XYZ(Math.Round(rebarStirrup_p3_Basic.X - columnSectionWidth + mainRebarCoverLayer * 2 - additionalBarDiam, 6)
                                , Math.Round(rebarStirrup_p3_Basic.Y, 6)
                                , Math.Round(rebarStirrup_p3_Basic.Z, 6));

                            //Точки стержня 1
                            XYZ firstBar_p1 = new XYZ(rebarStirrup_p1_Basic.X - (25 / 304.8), rebarStirrup_p1_Basic.Y, rebarStirrup_p1_Basic.Z);
                            XYZ firstBar_p2 = new XYZ(rebarStirrup_p2_Basic.X + (25 / 304.8), rebarStirrup_p2_Basic.Y, rebarStirrup_p1_Basic.Z);

                            //Кривые стержня1
                            List<Curve> myFloorRebarOutletCurves1 = new List<Curve>();

                            Curve myFloorRebarOutletLine1 = Line.CreateBound(firstBar_p1, firstBar_p2) as Curve;
                            myFloorRebarOutletCurves1.Add(myFloorRebarOutletLine1);

                            //Точки стержня 2
                            XYZ secondBar_p1 = new XYZ(rebarStirrup_p2_Basic.X, rebarStirrup_p2_Basic.Y + (25 / 304.8), rebarStirrup_p2_Basic.Z + additionalBarDiam);
                            XYZ secondBar_p2 = new XYZ(rebarStirrup_p3_Basic.X, rebarStirrup_p3_Basic.Y - (25 / 304.8), rebarStirrup_p3_Basic.Z + additionalBarDiam);

                            //Кривые стержня2
                            List<Curve> myFloorRebarOutletCurves2 = new List<Curve>();

                            Curve myFloorRebarOutletLine2 = Line.CreateBound(secondBar_p1, secondBar_p2) as Curve;
                            myFloorRebarOutletCurves2.Add(myFloorRebarOutletLine2);

                            //Точки стержня 3
                            XYZ thirdBar_p1 = new XYZ(rebarStirrup_p3_Basic.X + (25 / 304.8), rebarStirrup_p3_Basic.Y, rebarStirrup_p3_Basic.Z);
                            XYZ thirdBar_p2 = new XYZ(rebarStirrup_p4_Basic.X - (25 / 304.8), rebarStirrup_p4_Basic.Y, rebarStirrup_p4_Basic.Z);

                            //Кривые стержня3
                            List<Curve> myFloorRebarOutletCurves3 = new List<Curve>();

                            Curve myFloorRebarOutletLine3 = Line.CreateBound(thirdBar_p1, thirdBar_p2) as Curve;
                            myFloorRebarOutletCurves3.Add(myFloorRebarOutletLine3);

                            //Точки стержня 4
                            XYZ fourthBar_p1 = new XYZ(rebarStirrup_p4_Basic.X, rebarStirrup_p4_Basic.Y - (25 / 304.8), rebarStirrup_p4_Basic.Z + additionalBarDiam);
                            XYZ fourthBar_p2 = new XYZ(rebarStirrup_p1_Basic.X, rebarStirrup_p1_Basic.Y + (25 / 304.8), rebarStirrup_p1_Basic.Z + additionalBarDiam);

                            //Кривые стержня4
                            List<Curve> myFloorRebarOutletCurves4 = new List<Curve>();

                            Curve myFloorRebarOutletLine4 = Line.CreateBound(fourthBar_p1, fourthBar_p2) as Curve;
                            myFloorRebarOutletCurves4.Add(myFloorRebarOutletLine4);

                            Rebar floorRebarOutlet1 = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , rebarTypeForAdditionalBar //Заменить тип стержня диаметр осн/2
                                , null
                                , null
                                , mySelFloor
                                , narmalStirrup
                                , myFloorRebarOutletCurves1
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            Rebar floorRebarOutlet2 = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , rebarTypeForAdditionalBar //Заменить тип стержня диаметр осн/2
                                , null
                                , null
                                , mySelFloor
                                , narmalStirrup
                                , myFloorRebarOutletCurves2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            Rebar floorRebarOutlet3 = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , rebarTypeForAdditionalBar //Заменить тип стержня диаметр осн/2
                                , null
                                , null
                                , mySelFloor
                                , narmalStirrup
                                , myFloorRebarOutletCurves3
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            Rebar floorRebarOutlet4 = Rebar.CreateFromCurvesAndShape(doc
                                , myRebarShapeAnchorageOutletsLessFloorThickness
                                , rebarTypeForAdditionalBar //Заменить тип стержня диаметр осн/2
                                , null
                                , null
                                , mySelFloor
                                , narmalStirrup
                                , myFloorRebarOutletCurves4
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            rebarIdCollection.Add(floorRebarOutlet1.Id);
                            rebarIdCollection.Add(floorRebarOutlet2.Id);
                            rebarIdCollection.Add(floorRebarOutlet3.Id);
                            rebarIdCollection.Add(floorRebarOutlet4.Id);

                            //Копирование стержней
                            XYZ pointStirrupInstallation1 = new XYZ(0, 0, -(mySelFloorThickness - (offsetFromSlabBottom + 200 / 304.8)));
                            XYZ pointStirrupInstallation2 = new XYZ(0, 0, -((mySelFloorThickness - offsetFromSlabBottom - 200 / 304.8)) / 2);

                            List<ElementId> rebarId_1 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet1.Id, pointStirrupInstallation1) as List<ElementId>;
                            List<ElementId> rebarId_2 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet1.Id, pointStirrupInstallation2) as List<ElementId>;

                            List<ElementId> rebarId_3 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet2.Id, pointStirrupInstallation1) as List<ElementId>;
                            List<ElementId> rebarId_4 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet2.Id, pointStirrupInstallation2) as List<ElementId>;

                            List<ElementId> rebarId_5 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet3.Id, pointStirrupInstallation1) as List<ElementId>;
                            List<ElementId> rebarId_6 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet3.Id, pointStirrupInstallation2) as List<ElementId>;

                            List<ElementId> rebarId_7 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet4.Id, pointStirrupInstallation1) as List<ElementId>;
                            List<ElementId> rebarId_8 = ElementTransformUtils.CopyElement(doc, floorRebarOutlet4.Id, pointStirrupInstallation2) as List<ElementId>;

                            rebarIdCollection.Add(rebarId_1.First());
                            rebarIdCollection.Add(rebarId_2.First());
                            rebarIdCollection.Add(rebarId_3.First());
                            rebarIdCollection.Add(rebarId_4.First());
                            rebarIdCollection.Add(rebarId_5.First());
                            rebarIdCollection.Add(rebarId_6.First());
                            rebarIdCollection.Add(rebarId_7.First());
                            rebarIdCollection.Add(rebarId_8.First());
                        }
                    }

                    else if (OverlapOrWelding == "CompressionWelding" & forceTypeCheckedButtonName == "radioButton_Compression" & maxAnchoringLength < mySelFloorThickness)
                    {
                        //Точки для построения кривых стержня хомута
                        XYZ rebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam, 6)
                            , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2, 6)
                            , Math.Round(mySelFloorTopElevation - 100 / 304.8, 6));

                        XYZ rebarStirrup_p2 = new XYZ(Math.Round(rebarStirrup_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam, 6)
                            , Math.Round(rebarStirrup_p1.Y, 6)
                            , Math.Round(rebarStirrup_p1.Z, 6));

                        XYZ rebarStirrup_p3 = new XYZ(Math.Round(rebarStirrup_p2.X, 6)
                            , Math.Round(rebarStirrup_p2.Y - columnSectionHeight + mainRebarCoverLayer * 2 - stirrupRebarDiam - stirrupRebarDiam / 2, 6)
                            , Math.Round(rebarStirrup_p2.Z, 6));

                        XYZ rebarStirrup_p4 = new XYZ(Math.Round(rebarStirrup_p3.X - columnSectionWidth + mainRebarCoverLayer * 2 - stirrupRebarDiam, 6)
                            , Math.Round(rebarStirrup_p3.Y, 6)
                            , Math.Round(rebarStirrup_p3.Z, 6));

                        //Кривые хомута
                        List<Curve> myStirrupCurves = new List<Curve>();

                        Curve Stirrup_line1 = Line.CreateBound(rebarStirrup_p1, rebarStirrup_p2) as Curve;
                        myStirrupCurves.Add(Stirrup_line1);
                        Curve Stirrup_line2 = Line.CreateBound(rebarStirrup_p2, rebarStirrup_p3) as Curve;
                        myStirrupCurves.Add(Stirrup_line2);
                        Curve Stirrup_line3 = Line.CreateBound(rebarStirrup_p3, rebarStirrup_p4) as Curve;
                        myStirrupCurves.Add(Stirrup_line3);
                        Curve Stirrup_line4 = Line.CreateBound(rebarStirrup_p4, rebarStirrup_p1) as Curve;
                        myStirrupCurves.Add(Stirrup_line4);

                        //Построение хомута
                        Rebar outletsStirrup_1 = Rebar.CreateFromCurvesAndShape(doc
                            , myStirrupRebarShape
                            , myStirrupBarTape
                            , myRebarHookType
                            , myRebarHookType
                            , mySelFloor
                            , narmalStirrup
                            , myStirrupCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);


                        //Копирование хомута
                        XYZ pointStirrupInstallation1 = new XYZ(0, 0, -(maxAnchoringLength - 150 / 304.8));
                        XYZ pointStirrupInstallation2 = new XYZ(0, 0, -((maxAnchoringLength - 150 / 304.8) / 2));
                        List<ElementId> outletsStirrup_2 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation1) as List<ElementId>;
                        List<ElementId> outletsStirrup_3 = ElementTransformUtils.CopyElement(doc, outletsStirrup_1.Id, pointStirrupInstallation2) as List<ElementId>;

                        rebarIdCollection.Add(outletsStirrup_1.Id);
                        rebarIdCollection.Add(outletsStirrup_2.First());
                        rebarIdCollection.Add(outletsStirrup_3.First());
                    }

                    if (rebarIdCollection.Count == 0)
                    {
                        TaskDialog.Show("Revit", "Группа выпусков не содержит ни одного стержня!");
                        return Result.Failed;
                    }

                    List<Group> projectGroupList = new FilteredElementCollector(doc).OfClass(typeof(Group)).Cast<Group>().ToList();
                    if (projectGroupList.Any(g => g.GroupType.Name == "Выпуски " + column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString()))
                    {
                        TaskDialog.Show("Revit", "Группа с имененм Выпуски "
                            + column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString()
                            + " уже существует!\nБыли созданы отдельные стержни выпусков без группировки!");
                        continue;
                    }
                    else
                    {
                        Group newOutletsGroup = doc.Create.NewGroup(rebarIdCollection);
                        newOutletsGroup.GroupType.Name = "Выпуски " + column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }

        private double StretchingOverlapLengthsB20A500(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 350 / 304.8;
                    break;

                case 8:
                    OverlapLength = 470 / 304.8;
                    break;

                case 10:
                    OverlapLength = 580 / 304.8;
                    break;

                case 12:
                    OverlapLength = 700 / 304.8;
                    break;

                case 14:
                    OverlapLength = 820 / 304.8;
                    break;

                case 16:
                    OverlapLength = 930 / 304.8;
                    break;

                case 18:
                    OverlapLength = 1050 / 304.8;
                    break;

                case 20:
                    OverlapLength = 1160 / 304.8;
                    break;

                case 22:
                    OverlapLength = 1275 / 304.8;
                    break;

                case 25:
                    OverlapLength = 1450 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1625 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1860 / 304.8;
                    break;

                case 36:
                    OverlapLength = 2320 / 304.8;
                    break;

                case 40:
                    OverlapLength = 2580 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double StretchingOverlapLengthsB25A500(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 300 / 304.8;
                    break;

                case 8:
                    OverlapLength = 400 / 304.8;
                    break;

                case 10:
                    OverlapLength = 500 / 304.8;
                    break;

                case 12:
                    OverlapLength = 600 / 304.8;
                    break;

                case 14:
                    OverlapLength = 700 / 304.8;
                    break;

                case 16:
                    OverlapLength = 800 / 304.8;

                    break;

                case 18:
                    OverlapLength = 900 / 304.8;
                    break;

                case 20:
                    OverlapLength = 1000 / 304.8;
                    break;

                case 22:
                    OverlapLength = 1100 / 304.8;
                    break;

                case 25:
                    OverlapLength = 1250 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1400 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1590 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1990 / 304.8;
                    break;

                case 40:
                    OverlapLength = 2210 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double StretchingOverlapLengthsB30A500(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 365 / 304.8;
                    break;

                case 10:
                    OverlapLength = 455 / 304.8;
                    break;

                case 12:
                    OverlapLength = 545 / 304.8;
                    break;

                case 14:
                    OverlapLength = 635 / 304.8;
                    break;

                case 16:
                    OverlapLength = 730 / 304.8;
                    break;

                case 18:
                    OverlapLength = 820 / 304.8;
                    break;

                case 20:
                    OverlapLength = 910 / 304.8;
                    break;

                case 22:
                    OverlapLength = 1000 / 304.8;
                    break;

                case 25:
                    OverlapLength = 1135 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1270 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1455 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1815 / 304.8;
                    break;

                case 40:
                    OverlapLength = 2020 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double StretchingOverlapLengthsB35A500(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 240 / 304.8;
                    break;

                case 8:
                    OverlapLength = 325 / 304.8;
                    break;

                case 10:
                    OverlapLength = 405 / 304.8;
                    break;

                case 12:
                    OverlapLength = 485 / 304.8;
                    break;

                case 14:
                    OverlapLength = 565 / 304.8;
                    break;

                case 16:
                    OverlapLength = 645 / 304.8;
                    break;

                case 18:
                    OverlapLength = 725 / 304.8;
                    break;

                case 20:
                    OverlapLength = 805 / 304.8;
                    break;

                case 22:
                    OverlapLength = 885 / 304.8;
                    break;

                case 25:
                    OverlapLength = 1005 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1125 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1285 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1605 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1785 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double StretchingOverlapLengthsB40A500(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 225 / 304.8;
                    break;

                case 8:
                    OverlapLength = 300 / 304.8;
                    break;

                case 10:
                    OverlapLength = 375 / 304.8;
                    break;

                case 12:
                    OverlapLength = 450 / 304.8;
                    break;

                case 14:
                    OverlapLength = 525 / 304.8;
                    break;

                case 16:
                    OverlapLength = 600 / 304.8;
                    break;

                case 18:
                    OverlapLength = 675 / 304.8;
                    break;

                case 20:
                    OverlapLength = 745 / 304.8;
                    break;

                case 22:
                    OverlapLength = 820 / 304.8;
                    break;

                case 25:
                    OverlapLength = 935 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1045 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1195 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1495 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1660 / 304.8;
                    break;
            }
            return OverlapLength;
        }


        private double CompressionOverlapLengthsB20A500(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 265 / 304.8;
                    break;

                case 8:
                    OverlapLength = 350 / 304.8;
                    break;

                case 10:
                    OverlapLength = 735 / 304.8;
                    break;

                case 12:
                    OverlapLength = 525 / 304.8;
                    break;

                case 14:
                    OverlapLength = 610 / 304.8;
                    break;

                case 16:
                    OverlapLength = 700 / 304.8;
                    break;

                case 18:
                    OverlapLength = 785 / 304.8;
                    break;

                case 20:
                    OverlapLength = 870 / 304.8;
                    break;

                case 22:
                    OverlapLength = 960 / 304.8;
                    break;

                case 25:
                    OverlapLength = 1090 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1220 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1395 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1740 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1935 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double CompressionOverlapLengthsB25A500(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 300 / 304.8;
                    break;

                case 10:
                    OverlapLength = 375 / 304.8;
                    break;

                case 12:
                    OverlapLength = 450 / 304.8;
                    break;

                case 14:
                    OverlapLength = 525 / 304.8;
                    break;

                case 16:
                    OverlapLength = 600 / 304.8;

                    break;

                case 18:
                    OverlapLength = 675 / 304.8;
                    break;

                case 20:
                    OverlapLength = 745 / 304.8;
                    break;

                case 22:
                    OverlapLength = 820 / 304.8;
                    break;

                case 25:
                    OverlapLength = 935 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1045 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1195 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1495 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1660 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double CompressionOverlapLengthsB30A500(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 275 / 304.8;
                    break;

                case 10:
                    OverlapLength = 340 / 304.8;
                    break;

                case 12:
                    OverlapLength = 410 / 304.8;
                    break;

                case 14:
                    OverlapLength = 480 / 304.8;
                    break;

                case 16:
                    OverlapLength = 575 / 304.8;
                    break;

                case 18:
                    OverlapLength = 615 / 304.8;
                    break;

                case 20:
                    OverlapLength = 680 / 304.8;
                    break;

                case 22:
                    OverlapLength = 750 / 304.8;
                    break;

                case 25:
                    OverlapLength = 855 / 304.8;
                    break;

                case 28:
                    OverlapLength = 955 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1090 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1365 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1515 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double CompressionOverlapLengthsB35A500(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 250 / 304.8;
                    break;

                case 10:
                    OverlapLength = 305 / 304.8;
                    break;

                case 12:
                    OverlapLength = 365 / 304.8;
                    break;

                case 14:
                    OverlapLength = 425 / 304.8;
                    break;

                case 16:
                    OverlapLength = 485 / 304.8;
                    break;

                case 18:
                    OverlapLength = 545 / 304.8;
                    break;

                case 20:
                    OverlapLength = 605 / 304.8;
                    break;

                case 22:
                    OverlapLength = 665 / 304.8;
                    break;

                case 25:
                    OverlapLength = 755 / 304.8;
                    break;

                case 28:
                    OverlapLength = 845 / 304.8;
                    break;

                case 32:
                    OverlapLength = 965 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1205 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1340 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double CompressionOverlapLengthsB40A500(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 250 / 304.8;
                    break;

                case 10:
                    OverlapLength = 280 / 304.8;
                    break;

                case 12:
                    OverlapLength = 335 / 304.8;
                    break;

                case 14:
                    OverlapLength = 395 / 304.8;
                    break;

                case 16:
                    OverlapLength = 450 / 304.8;
                    break;

                case 18:
                    OverlapLength = 505 / 304.8;
                    break;

                case 20:
                    OverlapLength = 560 / 304.8;
                    break;

                case 22:
                    OverlapLength = 615 / 304.8;
                    break;

                case 25:
                    OverlapLength = 700 / 304.8;
                    break;

                case 28:
                    OverlapLength = 785 / 304.8;
                    break;

                case 32:
                    OverlapLength = 895 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1120 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1245 / 304.8;
                    break;
            }
            return OverlapLength;
        }


        private double StretchingAnchorageLengthsB20A500(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 290 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 390 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 490 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 580 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 680 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 775 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 870 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 970 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 1065 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 1210 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 1355 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 1550 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1935 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 2150 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double StretchingAnchorageLengthsB25A500(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 250 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 335 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 415 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 500 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 580 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 665 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 745 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 830 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 915 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 1035 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 1160 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 1325 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1660 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1845 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double StretchingAnchorageLengthsB30A500(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 230 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 305 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 380 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 455 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 530 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 605 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 680 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 760 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 835 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 945 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 1060 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 1210 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1515 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1685 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double StretchingAnchorageLengthsB35A500(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 270 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 335 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 405 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 470 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 535 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 605 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 670 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 740 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 840 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 940 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 1070 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1340 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1490 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double StretchingAnchorageLengthsB40A500(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 190 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 250 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 310 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 375 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 435 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 500 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 560 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 625 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 685 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 780 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 870 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 995 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1245 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1380 / 304.8;
                    break;
            }
            return AnchorageLength;
        }


        private double CompressionAnchorageLengthsB20A500(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 220 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 290 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 365 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 435 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 510 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 580 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 655 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 725 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 800 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 910 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 1015 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 1160 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1450 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1615 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double CompressionAnchorageLengthsB25A500(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 250 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 310 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 375 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 435 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 500 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 560 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 625 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 685 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 780 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 870 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 995 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1245 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1380 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double CompressionAnchorageLengthsB30A500(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 230 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 285 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 340 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 400 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 455 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 510 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 570 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 625 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 710 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 795 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 910 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1135 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1260 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double CompressionAnchorageLengthsB35A500(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 250 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 305 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 355 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 405 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 455 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 505 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 555 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 630 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 705 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 805 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1005 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1115 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double CompressionAnchorageLengthsB40A500(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 235 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 280 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 330 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 375 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 420 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 470 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 515 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 585 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 655 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 745 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 935 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1035 / 304.8;
                    break;
            }
            return AnchorageLength;
        }


        private ElementId GetMinXMinYRebar(List<Rebar> rebarList, double sectionWidth, double columnHeight, double coverLayer, XYZ columnOrigin, XYZ linkOrigin)
        { 
            XYZ minmax = new XYZ(10000, 10000, 0);
            ElementId rebId = new ElementId(0);
            double sw = sectionWidth;
            double sh = columnHeight;
            double cl = coverLayer;
            XYZ p = columnOrigin;
            XYZ lo = linkOrigin;

            foreach (Rebar reb in rebarList)
            {
                double rebDiam = reb.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();

                IList<Curve> columnRebarCurveList = reb.GetCenterlineCurves(false, false, false, new MultiplanarOption(), 0);
                Curve firstCurveColumnRebar = columnRebarCurveList.First();
                XYZ columnRebarFirstPoint = firstCurveColumnRebar.GetEndPoint(0);

                if (columnRebarFirstPoint.X + lo.X < p.X - sw / 2 + cl + rebDiam)
                {

                    if (columnRebarFirstPoint.Y + lo.Y < p.Y - sh/2 + cl + rebDiam & columnRebarFirstPoint.Y + lo.Y < minmax.Y)
                    {
                        minmax = columnRebarFirstPoint;
                        rebId = reb.Id;
                    }
                }
            }

            return rebId;
        }

        private ElementId GetMinXMaxYRebar(List<Rebar> rebarList, double sectionWidth, double columnHeight, double coverLayer, XYZ columnOrigin, XYZ linkOrigin)
        {
            XYZ minmax = new XYZ(10000, -10000, 0);
            ElementId rebId = new ElementId(0);
            double sw = sectionWidth;
            double sh = columnHeight;
            double cl = coverLayer;
            XYZ p = columnOrigin;
            XYZ lo = linkOrigin;

            foreach (Rebar reb in rebarList)
            {
                double rebDiam = reb.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();

                IList<Curve> columnRebarCurveList = reb.GetCenterlineCurves(false, false, false, new MultiplanarOption(), 0);
                Curve firstCurveColumnRebar = columnRebarCurveList.First();
                XYZ columnRebarFirstPoint = firstCurveColumnRebar.GetEndPoint(0);

                if (columnRebarFirstPoint.X + lo.X < p.X - sw / 2 + cl + rebDiam)
                {

                    if (columnRebarFirstPoint.Y + lo.Y > p.Y + sh / 2 - cl - rebDiam & columnRebarFirstPoint.Y + lo.Y > minmax.Y)
                    {
                        minmax = columnRebarFirstPoint;
                        rebId = reb.Id;
                    }
                }
            }

            return rebId;
        }

        private ElementId GetMaxXMaxYRebar(List<Rebar> rebarList, double sectionWidth, double columnHeight, double coverLayer, XYZ columnOrigin, XYZ linkOrigin)
        {
            XYZ minmax = new XYZ(-10000, -10000, 0);
            ElementId rebId = new ElementId(0);
            double sw = sectionWidth;
            double sh = columnHeight;
            double cl = coverLayer;
            XYZ p = columnOrigin;
            XYZ lo = linkOrigin;

            foreach (Rebar reb in rebarList)
            {
                double rebDiam = reb.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();

                IList<Curve> columnRebarCurveList = reb.GetCenterlineCurves(false, false, false, new MultiplanarOption(), 0);
                Curve firstCurveColumnRebar = columnRebarCurveList.First();
                XYZ columnRebarFirstPoint = firstCurveColumnRebar.GetEndPoint(0);

                if (columnRebarFirstPoint.X + lo.X > p.X + sw / 2 - cl - rebDiam)
                {
                    if (columnRebarFirstPoint.Y + lo.Y > p.Y + sh / 2 - cl - rebDiam & columnRebarFirstPoint.Y + lo.Y > minmax.Y)
                    {
                        minmax = columnRebarFirstPoint;
                        rebId = reb.Id;
                    }
                }
            }

            return rebId;
        }

        private ElementId GetMaxXMinYRebar(List<Rebar> rebarList, double sectionWidth, double columnHeight, double coverLayer, XYZ columnOrigin, XYZ linkOrigin)
        {
            XYZ minmax = new XYZ(-10000, 10000, 0);
            ElementId rebId = new ElementId(0);
            double sw = sectionWidth;
            double sh = columnHeight;
            double cl = coverLayer;
            XYZ p = columnOrigin;
            XYZ lo = linkOrigin;

            foreach (Rebar reb in rebarList)
            {
                double rebDiam = reb.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();

                IList<Curve> columnRebarCurveList = reb.GetCenterlineCurves(false, false, false, new MultiplanarOption(), 0);
                Curve firstCurveColumnRebar = columnRebarCurveList.First();
                XYZ columnRebarFirstPoint = firstCurveColumnRebar.GetEndPoint(0);

                if (columnRebarFirstPoint.X + lo.X > p.X + sw / 2 - cl - rebDiam)
                {
                    if (columnRebarFirstPoint.Y + lo.Y < p.Y - sh / 2 + cl + rebDiam & columnRebarFirstPoint.Y + lo.Y < minmax.Y)
                    {
                        minmax = columnRebarFirstPoint;
                        rebId = reb.Id;
                    }
                }
            }

            return rebId;
        }


        private double StretchingOverlapLengthsB20A400(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 290 / 304.8;
                    break;

                case 8:
                    OverlapLength = 380 / 304.8;
                    break;

                case 10:
                    OverlapLength = 480 / 304.8;
                    break;

                case 12:
                    OverlapLength = 570 / 304.8;
                    break;

                case 14:
                    OverlapLength = 670 / 304.8;
                    break;

                case 16:
                    OverlapLength = 760 / 304.8;
                    break;

                case 18:
                    OverlapLength = 860 / 304.8;
                    break;

                case 20:
                    OverlapLength = 950 / 304.8;
                    break;

                case 22:
                    OverlapLength = 1050 / 304.8;
                    break;

                case 25:
                    OverlapLength = 1190 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1330 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1520 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1900 / 304.8;
                    break;

                case 40:
                    OverlapLength = 2110 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double StretchingOverlapLengthsB25A400(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 330 / 304.8;
                    break;

                case 10:
                    OverlapLength = 410 / 304.8;
                    break;

                case 12:
                    OverlapLength = 490 / 304.8;
                    break;

                case 14:
                    OverlapLength = 570 / 304.8;
                    break;

                case 16:
                    OverlapLength = 650 / 304.8;

                    break;

                case 18:
                    OverlapLength = 730 / 304.8;
                    break;

                case 20:
                    OverlapLength = 820 / 304.8;
                    break;

                case 22:
                    OverlapLength = 900 / 304.8;
                    break;

                case 25:
                    OverlapLength = 1020 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1140 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1300 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1630 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1810 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double StretchingOverlapLengthsB30A400(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 300 / 304.8;
                    break;

                case 10:
                    OverlapLength = 370 / 304.8;
                    break;

                case 12:
                    OverlapLength = 450 / 304.8;
                    break;

                case 14:
                    OverlapLength = 520 / 304.8;
                    break;

                case 16:
                    OverlapLength = 600 / 304.8;
                    break;

                case 18:
                    OverlapLength = 670 / 304.8;
                    break;

                case 20:
                    OverlapLength = 740 / 304.8;
                    break;

                case 22:
                    OverlapLength = 820 / 304.8;
                    break;

                case 25:
                    OverlapLength = 930 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1040 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1190 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1490 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1650 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double StretchingOverlapLengthsB35A400(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 270 / 304.8;
                    break;

                case 10:
                    OverlapLength = 330 / 304.8;
                    break;

                case 12:
                    OverlapLength = 400 / 304.8;
                    break;

                case 14:
                    OverlapLength = 460 / 304.8;
                    break;

                case 16:
                    OverlapLength = 530 / 304.8;
                    break;

                case 18:
                    OverlapLength = 590 / 304.8;
                    break;

                case 20:
                    OverlapLength = 660 / 304.8;
                    break;

                case 22:
                    OverlapLength = 720 / 304.8;
                    break;

                case 25:
                    OverlapLength = 820 / 304.8;
                    break;

                case 28:
                    OverlapLength = 920 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1050 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1310 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1460 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double StretchingOverlapLengthsB40A400(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 250 / 304.8;
                    break;

                case 10:
                    OverlapLength = 310 / 304.8;
                    break;

                case 12:
                    OverlapLength = 370 / 304.8;
                    break;

                case 14:
                    OverlapLength = 430 / 304.8;
                    break;

                case 16:
                    OverlapLength = 490 / 304.8;
                    break;

                case 18:
                    OverlapLength = 550 / 304.8;
                    break;

                case 20:
                    OverlapLength = 610 / 304.8;
                    break;

                case 22:
                    OverlapLength = 970 / 304.8;
                    break;

                case 25:
                    OverlapLength = 760 / 304.8;
                    break;

                case 28:
                    OverlapLength = 860 / 304.8;
                    break;

                case 32:
                    OverlapLength = 980 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1220 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1360 / 304.8;
                    break;
            }
            return OverlapLength;
        }


        private double CompressionOverlapLengthsB20A400(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 290 / 304.8;
                    break;

                case 10:
                    OverlapLength = 360 / 304.8;
                    break;

                case 12:
                    OverlapLength = 430 / 304.8;
                    break;

                case 14:
                    OverlapLength = 500 / 304.8;
                    break;

                case 16:
                    OverlapLength = 570 / 304.8;
                    break;

                case 18:
                    OverlapLength = 640 / 304.8;
                    break;

                case 20:
                    OverlapLength = 710 / 304.8;
                    break;

                case 22:
                    OverlapLength = 790 / 304.8;
                    break;

                case 25:
                    OverlapLength = 890 / 304.8;
                    break;

                case 28:
                    OverlapLength = 1000 / 304.8;
                    break;

                case 32:
                    OverlapLength = 1140 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1420 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1580 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double CompressionOverlapLengthsB25A400(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 250 / 304.8;
                    break;

                case 10:
                    OverlapLength = 310 / 304.8;
                    break;

                case 12:
                    OverlapLength = 370 / 304.8;
                    break;

                case 14:
                    OverlapLength = 430 / 304.8;
                    break;

                case 16:
                    OverlapLength = 490 / 304.8;

                    break;

                case 18:
                    OverlapLength = 550 / 304.8;
                    break;

                case 20:
                    OverlapLength = 610 / 304.8;
                    break;

                case 22:
                    OverlapLength = 670 / 304.8;
                    break;

                case 25:
                    OverlapLength = 760 / 304.8;
                    break;

                case 28:
                    OverlapLength = 860 / 304.8;
                    break;

                case 32:
                    OverlapLength = 980 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1220 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1360 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double CompressionOverlapLengthsB30A400(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 250 / 304.8;
                    break;

                case 10:
                    OverlapLength = 280 / 304.8;
                    break;

                case 12:
                    OverlapLength = 340 / 304.8;
                    break;

                case 14:
                    OverlapLength = 390 / 304.8;
                    break;

                case 16:
                    OverlapLength = 450 / 304.8;
                    break;

                case 18:
                    OverlapLength = 500 / 304.8;
                    break;

                case 20:
                    OverlapLength = 560 / 304.8;
                    break;

                case 22:
                    OverlapLength = 620 / 304.8;
                    break;

                case 25:
                    OverlapLength = 700 / 304.8;
                    break;

                case 28:
                    OverlapLength = 780 / 304.8;
                    break;

                case 32:
                    OverlapLength = 890 / 304.8;
                    break;

                case 36:
                    OverlapLength = 1120 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1240 / 304.8;
                    break;
            }
            return OverlapLength;
        }
        private double CompressionOverlapLengthsB35A400(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 250 / 304.8;
                    break;

                case 10:
                    OverlapLength = 250 / 304.8;
                    break;

                case 12:
                    OverlapLength = 300 / 304.8;
                    break;

                case 14:
                    OverlapLength = 350 / 304.8;
                    break;

                case 16:
                    OverlapLength = 400 / 304.8;
                    break;

                case 18:
                    OverlapLength = 450 / 304.8;
                    break;

                case 20:
                    OverlapLength = 500 / 304.8;
                    break;

                case 22:
                    OverlapLength = 540 / 304.8;
                    break;

                case 25:
                    OverlapLength = 620 / 304.8;
                    break;

                case 28:
                    OverlapLength = 690 / 304.8;
                    break;

                case 32:
                    OverlapLength = 790 / 304.8;
                    break;

                case 36:
                    OverlapLength = 990 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1100 / 304.8;
                    break;
            }
            return OverlapLength;
        }

        private double CompressionOverlapLengthsB40A400(double diam)
        {
            double OverlapLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    OverlapLength = 250 / 304.8;
                    break;

                case 8:
                    OverlapLength = 250 / 304.8;
                    break;

                case 10:
                    OverlapLength = 250 / 304.8;
                    break;

                case 12:
                    OverlapLength = 280 / 304.8;
                    break;

                case 14:
                    OverlapLength = 320 / 304.8;
                    break;

                case 16:
                    OverlapLength = 370 / 304.8;
                    break;

                case 18:
                    OverlapLength = 410 / 304.8;
                    break;

                case 20:
                    OverlapLength = 460 / 304.8;
                    break;

                case 22:
                    OverlapLength = 510 / 304.8;
                    break;

                case 25:
                    OverlapLength = 570 / 304.8;
                    break;

                case 28:
                    OverlapLength = 640 / 304.8;
                    break;

                case 32:
                    OverlapLength = 730 / 304.8;
                    break;

                case 36:
                    OverlapLength = 920 / 304.8;
                    break;

                case 40:
                    OverlapLength = 1020 / 304.8;
                    break;
            }
            return OverlapLength;
        }


        private double StretchingAnchorageLengthsB20A400(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 240 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 320 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 400 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 480 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 560 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 640 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 710 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 790 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 870 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 990 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 1110 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 1270 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1580 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1760 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double StretchingAnchorageLengthsB25A400(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 210 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 270 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 340 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 410 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 480 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 540 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 610 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 680 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 750 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 850 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 950 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 1100 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1360 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1510 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double StretchingAnchorageLengthsB30A400(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 250 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 310 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 370 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 440 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 500 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 560 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 620 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 680 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 780 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 870 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 990 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1240 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1380 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double StretchingAnchorageLengthsB35A400(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 220 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 280 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 330 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 390 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 440 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 500 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 550 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 600 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 690 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 770 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 880 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1100 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1220 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double StretchingAnchorageLengthsB40A400(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 210 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 260 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 310 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 360 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 410 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 460 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 510 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 560 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 640 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 710 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 820 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1020 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1130 / 304.8;
                    break;
            }
            return AnchorageLength;
        }


        private double CompressionAnchorageLengthsB20A400(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 240 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 300 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 360 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 420 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 480 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 540 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 600 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 650 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 740 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 830 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 950 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1190 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1320 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double CompressionAnchorageLengthsB25A400(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 210 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 260 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 310 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 360 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 410 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 460 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 510 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 560 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 640 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 710 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 820 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 1020 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1130 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double CompressionAnchorageLengthsB30A400(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 240 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 280 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 330 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 370 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 420 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 470 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 510 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 580 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 650 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 740 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 930 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 1030 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double CompressionAnchorageLengthsB35A400(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 210 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 250 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 290 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 330 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 370 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 410 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 450 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 520 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 580 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 660 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 820 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 910 / 304.8;
                    break;
            }
            return AnchorageLength;
        }

        private double CompressionAnchorageLengthsB40A400(double diam)
        {
            double AnchorageLength = 0;
            switch (Math.Round(diam * 304.8))
            {
                case 6:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 8:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 10:
                    AnchorageLength = 200 / 304.8;
                    break;

                case 12:
                    AnchorageLength = 230 / 304.8;
                    break;

                case 14:
                    AnchorageLength = 270 / 304.8;
                    break;

                case 16:
                    AnchorageLength = 310 / 304.8;
                    break;

                case 18:
                    AnchorageLength = 350 / 304.8;
                    break;

                case 20:
                    AnchorageLength = 380 / 304.8;
                    break;

                case 22:
                    AnchorageLength = 420 / 304.8;
                    break;

                case 25:
                    AnchorageLength = 480 / 304.8;
                    break;

                case 28:
                    AnchorageLength = 540 / 304.8;
                    break;

                case 32:
                    AnchorageLength = 610 / 304.8;
                    break;

                case 36:
                    AnchorageLength = 760 / 304.8;
                    break;

                case 40:
                    AnchorageLength = 850 / 304.8;
                    break;
            }
            return AnchorageLength;
        }
    }
}
