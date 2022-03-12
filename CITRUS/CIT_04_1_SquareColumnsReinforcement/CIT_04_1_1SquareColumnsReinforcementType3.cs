﻿using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CITRUS.CIT_04_1_SquareColumnsReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_1_1SquareColumnsReinforcementType3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Execute(commandData.Application);
        }

        public Result Execute(UIApplication uiapp)
        {
            //Получение текущего документа
            Document doc = uiapp.ActiveUIDocument.Document;

            //Получение доступа к Selection
            Selection sel = uiapp.ActiveUIDocument.Selection;
#region Старт блока Получение списка колонн
            //Выбор колонн
            ColumnSelectionFilter columnSelFilter = new ColumnSelectionFilter(); //Вызов фильтра выбора
            IList<Reference> selColumns = sel.PickObjects(ObjectType.Element, columnSelFilter, "Выберите колонны!");//Получение списка ссылок на выбранные колонны

            List<FamilyInstance> columnsList = new List<FamilyInstance>();//Получение списка выбранных колонн
            foreach (Reference columnRef in selColumns)
            {
                columnsList.Add(doc.GetElement(columnRef) as FamilyInstance);
            }
#endregion

# region Старт блока выбора форм арматурных стержней
            //Выбор формы основной арматуры если стыковка стержней в нахлест
            List<RebarShape> rebarShapeMainOverlappingRodsList = new FilteredElementCollector(doc)
                            .OfClass(typeof(RebarShape))
                            .Where(rs => rs.Name.ToString() == "26")
                            .Cast<RebarShape>()
                            .ToList();
            if (rebarShapeMainOverlappingRodsList.Count == 0)
            {
                rebarShapeMainOverlappingRodsList = new FilteredElementCollector(doc)
               .OfClass(typeof(RebarShape))
               .Where(rs => rs.Name.ToString() == "О_26(α»90)")
               .Cast<RebarShape>()
               .ToList();
                if (rebarShapeMainOverlappingRodsList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 26 или О_26(α»90) не найдена");
                    return Result.Failed;
                }
            }
            RebarShape myMainRebarShapeOverlappingRods = rebarShapeMainOverlappingRodsList.First();

            //Выбор формы основной арматуры если стыковка на сварке
            List<RebarShape> rebarShapeMainWeldingRodsList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "01")
                .Cast<RebarShape>()
                .ToList();
            if (rebarShapeMainWeldingRodsList.Count == 0)
            {
                rebarShapeMainWeldingRodsList = new FilteredElementCollector(doc)
               .OfClass(typeof(RebarShape))
               .Where(rs => rs.Name.ToString() == "О_1")
               .Cast<RebarShape>()
               .ToList();
                if (rebarShapeMainWeldingRodsList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 01 или О_1 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape myMainRebarShapeWeldingRods = rebarShapeMainWeldingRodsList.First();

            //Выбор формы основной арматуры если загиб в плиту
            List<RebarShape> rebarShapeMainRodsListBendIntoASlab = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "11")
                .Cast<RebarShape>()
                .ToList();
            if (rebarShapeMainRodsListBendIntoASlab.Count == 0)
            {
                rebarShapeMainRodsListBendIntoASlab = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_11")
                .Cast<RebarShape>()
                .ToList();
                if (rebarShapeMainRodsListBendIntoASlab.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 11 или О_11 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape myMainRebarShapeRodsBendIntoASlab = rebarShapeMainRodsListBendIntoASlab.First();

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
#endregion

# region Старт блока создания списков типов для формы
            //Список типов для выбора основной арматуры
            List<RebarBarType> firstMainRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();
            //Список типов для выбора основной арматуры
            List<RebarBarType> secondMainRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов для выбора арматуры хомутов опоясывающих
            List<RebarBarType> firstStirrupRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов для выбора арматуры хомутов дополнительных
            List<RebarBarType> secondStirrupRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов защитных слоев арматуры
            List<RebarCoverType> rebarCoverTypesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();

            #endregion

#region  Старт блока использования формы

            //Вызов формы
            CIT_04_1_1FormSquareColumnsReinforcementType3 formSquareColumnsReinforcementType3 = new CIT_04_1_1FormSquareColumnsReinforcementType3(firstMainRebarTapesList, secondMainRebarTapesList, firstStirrupRebarTapesList, secondStirrupRebarTapesList, rebarCoverTypesList);
            formSquareColumnsReinforcementType3.ShowDialog();
            if (formSquareColumnsReinforcementType3.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            #region Старт блока Получение данных из формы

            //Выбор типа угловой основной арматуры
            RebarBarType myFirstMainRebarType = formSquareColumnsReinforcementType3.mySelectionFirstMainBarTape;
            //Выбор типа основной боковой арматуры
            RebarBarType mySecondMainRebarType = formSquareColumnsReinforcementType3.mySelectionSecondMainBarTape;
            //Выбор типа арматуры хомутов опоясывающих
            RebarBarType myFirstStirrupBarTape = formSquareColumnsReinforcementType3.mySelectionFirstStirrupBarTape;
            //Выбор типа арматуры хомутов дополнительных
            RebarBarType mySecondStirrupBarTape = formSquareColumnsReinforcementType3.mySelectionSecondStirrupBarTape;
            //Выбор типа защитного слоя основной арматуры
            RebarCoverType myRebarCoverType = formSquareColumnsReinforcementType3.mySelectionRebarCoverType;

            //Диаметр стержня угловой основной арматуры
            Parameter mainFirstRebarTypeDiamParam = myFirstMainRebarType.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double firstMainRebarDiam = mainFirstRebarTypeDiamParam.AsDouble();

            //Диаметр стержня основной боковой арматуры
            Parameter secondMainRebarTypeDiamParam = mySecondMainRebarType.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double secondMainRebarDiam = secondMainRebarTypeDiamParam.AsDouble();

            //Диаметр хомута опоясывающего
            Parameter firstStirrupRebarTypeDiamParam = myFirstStirrupBarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double firstStirrupRebarDiam = firstStirrupRebarTypeDiamParam.AsDouble();

            //Диаметр хомута дополнительного
            Parameter secondStirrupRebarTypeDiamParam = mySecondStirrupBarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double secondStirrupRebarDiam = secondStirrupRebarTypeDiamParam.AsDouble();
            //Диаметр загиба хомута дополнительного

            Parameter secondStirrupRebarTypeBendDiamParam = mySecondStirrupBarTape.get_Parameter(BuiltInParameter.REBAR_BAR_STIRRUP_BEND_DIAMETER);
            double secondStirrupRebarBendDiam = secondStirrupRebarTypeBendDiamParam.AsDouble();

            //Защитный слой арматуры как dooble
            double mainRebarCoverLayer = myRebarCoverType.CoverDistance;
            

            //Толщина перекрытия над колонной
            double floorThicknessAboveColumn = formSquareColumnsReinforcementType3.FloorThickness / 304.8;
            if (floorThicknessAboveColumn == 0)
            {
                TaskDialog.Show("Revit", "Кожаный мешок, не забывай задавать толщину перекрытия!");
                return Result.Cancelled;
            }
            //Длина выпусков
            double rebarOutletsLength = formSquareColumnsReinforcementType3.RebarOutlets / 304.8;

            //Длина выпусков боковых стержней
            double rebarSecondOutletsLength = formSquareColumnsReinforcementType3.RebarSecondOutlets / 304.8;

            // Смещение первого хомута от низа колонны
            double firstStirrupOffset = formSquareColumnsReinforcementType3.FirstStirrupOffset / 304.8;

            // Учащенный шаг хомутов
            double increasedStirrupSpacing = formSquareColumnsReinforcementType3.IncreasedStirrupSpacing / 304.8;

            // Стандартный шаг хомутов
            double standardStirrupSpacing = formSquareColumnsReinforcementType3.StandardStirrupSpacing / 304.8;

            //Высота размещения хомутов с учащенным шагом
            double stirrupIncreasedPlacementHeight = formSquareColumnsReinforcementType3.StirrupIncreasedPlacementHeight / 304.8;
            int StirrupBarElemFrequentQuantity = (int)(stirrupIncreasedPlacementHeight / increasedStirrupSpacing) + 1;

            string checkedRebarOutletsButtonName = formSquareColumnsReinforcementType3.CheckedRebarOutletsButtonName;

            //Изменение сечения колонны
            bool changeColumnSection = formSquareColumnsReinforcementType3.СhangeColumnSection;
            
            //Переход со сварки на нахлест
            bool transitionToOverlap = formSquareColumnsReinforcementType3.TransitionToOverlap;

            //Заглубление стержней
            double deepeningBarsSize = 0;
            bool deepeningBars = formSquareColumnsReinforcementType3.DeepeningBars;
            if (deepeningBars == true)
            {
                deepeningBarsSize = formSquareColumnsReinforcementType3.DeepeningBarsSize / 304.8;
            }
            else
            {
                deepeningBarsSize = 0;
            }

            //Загнуть в плиту
            bool bendIntoASlab = formSquareColumnsReinforcementType3.BendIntoASlab;

            //Завершение блока Получение данных из формы
            #endregion
            //Завершение блока использования формы
            #endregion

#region Старт блока Получение типа элемента CIT_04_ВаннаДляСварки
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
            #endregion

            double sectionOffset = formSquareColumnsReinforcementType3.ColumnSectionOffset / 304.8;
            double deltaXOverlapping = Math.Sqrt(Math.Pow((sectionOffset + firstMainRebarDiam), 2) + Math.Pow(sectionOffset, 2));
            double alphaOverlapping = Math.Asin(sectionOffset / deltaXOverlapping);

            double deltaXSecondOverlapping = Math.Sqrt(Math.Pow(sectionOffset, 2) + Math.Pow(secondMainRebarDiam, 2));
            double alphaSecondOverlapping = Math.Asin(secondMainRebarDiam / deltaXSecondOverlapping);

            double deltaXWelding = Math.Sqrt(Math.Pow(sectionOffset, 2) + Math.Pow(sectionOffset, 2));
            double alphaWelding = Math.Asin(sectionOffset / deltaXWelding);

#region Старт блока Размещение арматуры в проекте
            //Открытие транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение арматуры колонн");
                //Старт блока обработки списка колонн
                foreach (FamilyInstance myColumn in columnsList)
                {
                    //Старт блока сбора параметров колонны
                    //Базовый уровень
                    ElementId baseLevelId = myColumn.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId();
                    Level baseLevel = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Where(lv => lv.Id == baseLevelId)
                        .Cast<Level>()
                        .ToList()
                        .First();
                    //Отметка базового уровня
                    double baseLevelElevation = Math.Round(baseLevel.Elevation, 6);

                    //Верхний уровень
                    ElementId topLevelId = myColumn.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM).AsElementId();
                    Level topLevel = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Where(lv => lv.Id == topLevelId)
                        .Cast<Level>()
                        .ToList()
                        .First();
                    //Отметка верхнего уровня
                    double topLevelElevation = Math.Round(topLevel.Elevation, 6);

                    //Смещение снизу
                    Parameter baseLevelOffsetParam = myColumn.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM);
                    double baseLevelOffset = Math.Round(baseLevelOffsetParam.AsDouble(), 6);

                    //Смещение сверху
                    Parameter topLevelOffsetParam = myColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);
                    double topLevelOffset = Math.Round(topLevelOffsetParam.AsDouble(), 6);

                    //Длина колонны
                    double columnLength = ((topLevelElevation + topLevelOffset) - (baseLevelElevation + baseLevelOffset));

                    //Ширина сечения колонны
                    double columnSectionWidth = 0;
                    if (myColumn.Symbol.LookupParameter("Рзм.Ширина") != null)
                    {
                        columnSectionWidth = myColumn.Symbol.LookupParameter("Рзм.Ширина").AsDouble();
                    }
                    else
                    {
                        columnSectionWidth = myColumn.Symbol.LookupParameter("ADSK_Размер_Ширина").AsDouble();
                    }

                    //Высота сечения колонны
                    double columnSectionHeight = 0;
                    if (myColumn.Symbol.LookupParameter("Рзм.Высота") != null)
                    {
                        columnSectionHeight = myColumn.Symbol.LookupParameter("Рзм.Высота").AsDouble();
                    }
                    else
                    {
                        columnSectionHeight = myColumn.Symbol.LookupParameter("ADSK_Размер_Высота").AsDouble();
                    }

                    if (columnSectionWidth!= columnSectionHeight)
                    {
                        continue;
                    }

                    if (columnSectionWidth <= 300/304.8)
                    {
                        continue;
                    }

                    //Получение нижней точки геометрии колонны
                    LocationPoint columnOriginLocationPoint = myColumn.Location as LocationPoint;
                    XYZ columnOriginBase = columnOriginLocationPoint.Point;
                    XYZ columnOrigin = new XYZ(columnOriginBase.X, columnOriginBase.Y, baseLevelElevation + baseLevelOffset);

                    //Угол поворота колонны
                    double columnRotation = columnOriginLocationPoint.Rotation;
                    //Ось вращения
                    XYZ rotationPoint1 = new XYZ(columnOrigin.X, columnOrigin.Y, columnOrigin.Z);
                    XYZ rotationPoint2 = new XYZ(columnOrigin.X, columnOrigin.Y, columnOrigin.Z + 1);
                    Line rotationAxis = Line.CreateBound(rotationPoint1, rotationPoint2);

                    //Завершение блока сбора параметров колонны

                    //Старт блока задания параметра защитного слоя боковых граней колонны
                    //Защитный слой арматуры боковых граней
                    Parameter clearCoverOther = myColumn.get_Parameter(BuiltInParameter.CLEAR_COVER_OTHER);
                    clearCoverOther.Set(myRebarCoverType.Id);
                    //Завершение блока задания параметра защитного слоя боковых граней колонны

                    //Универсальная коллекция для формирования группы выпусков
                    ICollection<ElementId> rebarIdCollection = new List<ElementId>();

                    //Старт блока создания арматуры колонны
                    //Нормаль для построения стержней основной арматуры
                    XYZ mainRebarNormal = new XYZ(0, 1, 0);

                    if (checkedRebarOutletsButtonName == "radioButton_MainOverlappingRods" & changeColumnSection == false & bendIntoASlab == false)
                    {
                        //Если стыковка стержней в нахлест без изменения сечения колонны выше
                        //Точки для построения кривфх основных угловых стержней
                        XYZ firstMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ firstMainRebar_p2 = new XYZ(Math.Round(firstMainRebar_p1.X, 6), Math.Round(firstMainRebar_p1.Y, 6), Math.Round(firstMainRebar_p1.Z + deepeningBarsSize + columnLength, 6));
                        XYZ firstMainRebar_p3 = new XYZ(Math.Round(firstMainRebar_p2.X + firstMainRebarDiam, 6), Math.Round(firstMainRebar_p2.Y, 6), Math.Round(firstMainRebar_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ firstMainRebar_p4 = new XYZ(Math.Round(firstMainRebar_p3.X, 6), Math.Round(firstMainRebar_p3.Y, 6), Math.Round(firstMainRebar_p3.Z + rebarOutletsLength, 6));

                        //Точки для построения кривфх основных боковых стержней
                        XYZ secondMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ secondMainRebar_p2 = new XYZ(Math.Round(secondMainRebar_p1.X, 6), Math.Round(secondMainRebar_p1.Y, 6), Math.Round(secondMainRebar_p1.Z + deepeningBarsSize + columnLength, 6));
                        XYZ secondMainRebar_p3 = new XYZ(Math.Round(secondMainRebar_p2.X + secondMainRebarDiam, 6), Math.Round(secondMainRebar_p2.Y, 6), Math.Round(secondMainRebar_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ secondMainRebar_p4 = new XYZ(Math.Round(secondMainRebar_p3.X, 6), Math.Round(secondMainRebar_p3.Y, 6), Math.Round(secondMainRebar_p3.Z + rebarSecondOutletsLength, 6));

                        //Кривые основных угловых стержней
                        List<Curve> myFirstMainRebarCurves = new List<Curve>();

                        Curve firstMainLine1 = Line.CreateBound(firstMainRebar_p1, firstMainRebar_p2) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine1);
                        Curve firstMainLine2 = Line.CreateBound(firstMainRebar_p2, firstMainRebar_p3) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine2);
                        Curve firstMainLine3 = Line.CreateBound(firstMainRebar_p3, firstMainRebar_p4) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine3);

                        //Кривые основных боковых стержней
                        List<Curve> mySecondMainRebarCurves = new List<Curve>();

                        Curve secondMainLine1 = Line.CreateBound(secondMainRebar_p1, secondMainRebar_p2) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine1);
                        Curve secondMainLine2 = Line.CreateBound(secondMainRebar_p2, secondMainRebar_p3) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine2);
                        Curve secondMainLine3 = Line.CreateBound(secondMainRebar_p3, secondMainRebar_p4) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine3);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);

                        //Центральный левый стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        rebarIdCollection.Add(columnMainRebar_1A.Id);

                        //Центральный правый стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_2A = new XYZ(+columnSectionHeight / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnMainRebar_2A);
                        rebarIdCollection.Add(columnMainRebar_2A.Id);

                        //Центральный нижний стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3A = new XYZ(0, -columnSectionWidth / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnMainRebar_3A);
                        rebarIdCollection.Add(columnMainRebar_3A.Id);

                        //Центральный верхний стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4A = new XYZ(0, columnSectionWidth / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnMainRebar_4A);
                        rebarIdCollection.Add(columnMainRebar_4A.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainOverlappingRods" & changeColumnSection == false & bendIntoASlab == true)
                    {
                        //Если стыковка стержней в нахлест без изменения сечения колонны выше
                        //Точки для построения кривфх основных угловых стержней
                        XYZ firstMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ firstMainRebar_p2 = new XYZ(Math.Round(firstMainRebar_p1.X, 6), Math.Round(firstMainRebar_p1.Y, 6), Math.Round(firstMainRebar_p1.Z + deepeningBarsSize + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                        XYZ firstMainRebar_p3 = new XYZ(Math.Round(firstMainRebar_p2.X + rebarOutletsLength - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(firstMainRebar_p2.Y, 6), Math.Round(firstMainRebar_p2.Z, 6));

                        //Точки для построения кривфх основных боковых стержней
                        XYZ secondMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ secondMainRebar_p2 = new XYZ(Math.Round(secondMainRebar_p1.X, 6), Math.Round(secondMainRebar_p1.Y, 6), Math.Round(secondMainRebar_p1.Z + deepeningBarsSize + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                        XYZ secondMainRebar_p3 = new XYZ(Math.Round(secondMainRebar_p2.X + rebarSecondOutletsLength - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(secondMainRebar_p2.Y, 6), Math.Round(secondMainRebar_p2.Z, 6));

                        //Кривые основных угловых стержней
                        List<Curve> myFirstMainRebarCurves = new List<Curve>();

                        Curve firstMainLine1 = Line.CreateBound(firstMainRebar_p1, firstMainRebar_p2) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine1);
                        Curve firstMainLine2 = Line.CreateBound(firstMainRebar_p2, firstMainRebar_p3) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine2);

                        //Кривые основных боковых стержней
                        List<Curve> mySecondMainRebarCurves = new List<Curve>();

                        Curve secondMainLine1 = Line.CreateBound(secondMainRebar_p1, secondMainRebar_p2) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine1);
                        Curve secondMainLine2 = Line.CreateBound(secondMainRebar_p2, secondMainRebar_p3) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine2);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myFirstMainRebarType
                            , null, null, myColumn
                            , mainRebarNormal
                            , myFirstMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, 180 * (Math.PI / 180));

                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myFirstMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myFirstMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myFirstMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myFirstMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myFirstMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myFirstMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);

                        //Центральный левый стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , mySecondMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , mySecondMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        rebarIdCollection.Add(columnMainRebar_1A.Id);

                        //Центральный правый стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , mySecondMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , mySecondMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebar_2A = new XYZ(+columnSectionHeight / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnMainRebar_2A);
                        rebarIdCollection.Add(columnMainRebar_2A.Id);

                        //Центральный нижний стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , mySecondMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , mySecondMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_3A = new XYZ(0, -columnSectionWidth / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnMainRebar_3A);
                        rebarIdCollection.Add(columnMainRebar_3A.Id);

                        //Центральный верхний стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , mySecondMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , mySecondMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_4A = new XYZ(0, columnSectionWidth / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnMainRebar_4A);
                        rebarIdCollection.Add(columnMainRebar_4A.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & transitionToOverlap == false & changeColumnSection == false & bendIntoASlab == false)
                    {
                        //Если стыковка стержней на сварке без изменения сечения колонны выше
                        //Точки для построения кривфх основных угловых стержней
                        XYZ firstMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLength, 6));
                        XYZ firstMainRebar_p2 = new XYZ(Math.Round(firstMainRebar_p1.X, 6), Math.Round(firstMainRebar_p1.Y, 6), Math.Round(firstMainRebar_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                        //Точки для построения кривфх основных боковых стержней
                        XYZ secondMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarSecondOutletsLength, 6));
                        XYZ secondMainRebar_p2 = new XYZ(Math.Round(secondMainRebar_p1.X, 6), Math.Round(secondMainRebar_p1.Y, 6), Math.Round(secondMainRebar_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                        XYZ firstTubWelding_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLength + baseLevelOffset);
                        XYZ secondTubWelding_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarSecondOutletsLength + baseLevelOffset);
                        //Кривые основных угловых стержней
                        List<Curve> myFirstMainRebarCurves = new List<Curve>();
                        Curve firstMainLine1 = Line.CreateBound(firstMainRebar_p1, firstMainRebar_p2) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine1);

                        //Кривые основных боковых стержней
                        List<Curve> mySecondMainRebarCurves = new List<Curve>();
                        Curve secondMainLine1 = Line.CreateBound(secondMainRebar_p1, secondMainRebar_p2) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine1);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        FamilyInstance tubWelding_1 = doc.Create.NewFamilyInstance(firstTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_1.LookupParameter("Диаметр стержня").Set(firstMainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(tubWelding_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnMainRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        FamilyInstance tubWelding_2 = doc.Create.NewFamilyInstance(firstTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_2.LookupParameter("Диаметр стержня").Set(firstMainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_2.Id, newPlaсeСolumnMainRebar_2);
                        rebarIdCollection.Add(tubWelding_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnMainRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        FamilyInstance tubWelding_3 = doc.Create.NewFamilyInstance(firstTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_3.LookupParameter("Диаметр стержня").Set(firstMainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_3.Id, newPlaсeСolumnMainRebar_3);
                        rebarIdCollection.Add(tubWelding_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnMainRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);

                        FamilyInstance tubWelding_4 = doc.Create.NewFamilyInstance(firstTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_4.LookupParameter("Диаметр стержня").Set(firstMainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_4.Id, newPlaсeСolumnMainRebar_4);
                        rebarIdCollection.Add(tubWelding_4.Id);

                        //Центральный левый стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        rebarIdCollection.Add(columnMainRebar_1A.Id);

                        FamilyInstance tubWelding_1A = doc.Create.NewFamilyInstance(secondTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_1A.LookupParameter("Диаметр стержня").Set(secondMainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_1A.Id, newPlaсeСolumnMainRebar_1A);
                        rebarIdCollection.Add(tubWelding_1A.Id);

                        //Центральный правый стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_2A = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnMainRebar_2A);
                        rebarIdCollection.Add(columnMainRebar_2A.Id);

                        FamilyInstance tubWelding_2A = doc.Create.NewFamilyInstance(secondTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_2A.LookupParameter("Диаметр стержня").Set(secondMainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_2A.Id, newPlaсeСolumnMainRebar_2A);
                        rebarIdCollection.Add(tubWelding_2A.Id);

                        //Центральный нижний стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3A = new XYZ(0, -columnSectionWidth / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnMainRebar_3A);
                        rebarIdCollection.Add(columnMainRebar_3A.Id);

                        FamilyInstance tubWelding_3A = doc.Create.NewFamilyInstance(secondTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_3A.LookupParameter("Диаметр стержня").Set(secondMainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_3A.Id, newPlaсeСolumnMainRebar_3A);
                        rebarIdCollection.Add(tubWelding_3A.Id);

                        //Центральный верхний стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4A = new XYZ(0, columnSectionWidth / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnMainRebar_4A);
                        rebarIdCollection.Add(columnMainRebar_4A.Id);

                        FamilyInstance tubWelding_4A = doc.Create.NewFamilyInstance(secondTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_4A.LookupParameter("Диаметр стержня").Set(secondMainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_4A.Id, newPlaсeСolumnMainRebar_4A);
                        rebarIdCollection.Add(tubWelding_4A.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & transitionToOverlap == false & changeColumnSection == false & bendIntoASlab == true)
                    {
                        //Если стыковка стержней на сварке загнуть в плиту
                        //Точки для построения кривфх основных угловых стержней
                        XYZ firstMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize + rebarOutletsLength, 6));
                        XYZ firstMainRebar_p2 = new XYZ(Math.Round(firstMainRebar_p1.X, 6), Math.Round(firstMainRebar_p1.Y, 6), Math.Round(firstMainRebar_p1.Z + deepeningBarsSize - rebarOutletsLength + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                        XYZ firstMainRebar_p3 = new XYZ(Math.Round(firstMainRebar_p2.X + rebarOutletsLength - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(firstMainRebar_p2.Y, 6), Math.Round(firstMainRebar_p2.Z, 6));

                        //Точки для построения кривфх основных боковых стержней
                        XYZ secondMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize + rebarSecondOutletsLength, 6));
                        XYZ secondMainRebar_p2 = new XYZ(Math.Round(secondMainRebar_p1.X, 6), Math.Round(secondMainRebar_p1.Y, 6), Math.Round(secondMainRebar_p1.Z + deepeningBarsSize - rebarSecondOutletsLength + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                        XYZ secondMainRebar_p3 = new XYZ(Math.Round(secondMainRebar_p2.X + rebarSecondOutletsLength - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(secondMainRebar_p2.Y, 6), Math.Round(secondMainRebar_p2.Z, 6));

                        //Кривые основных угловых стержней
                        List<Curve> myFirstMainRebarCurves = new List<Curve>();

                        Curve firstMainLine1 = Line.CreateBound(firstMainRebar_p1, firstMainRebar_p2) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine1);
                        Curve firstMainLine2 = Line.CreateBound(firstMainRebar_p2, firstMainRebar_p3) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine2);

                        //Кривые основных боковых стержней
                        List<Curve> mySecondMainRebarCurves = new List<Curve>();

                        Curve secondMainLine1 = Line.CreateBound(secondMainRebar_p1, secondMainRebar_p2) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine1);
                        Curve secondMainLine2 = Line.CreateBound(secondMainRebar_p2, secondMainRebar_p3) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine2);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myFirstMainRebarType
                            , null, null, myColumn
                            , mainRebarNormal
                            , myFirstMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, 180 * (Math.PI / 180));

                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myFirstMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myFirstMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myFirstMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myFirstMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myFirstMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myFirstMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);

                        //Центральный левый стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , mySecondMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , mySecondMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        rebarIdCollection.Add(columnMainRebar_1A.Id);

                        //Центральный правый стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , mySecondMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , mySecondMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebar_2A = new XYZ(+columnSectionHeight / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnMainRebar_2A);
                        rebarIdCollection.Add(columnMainRebar_2A.Id);

                        //Центральный нижний стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , mySecondMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , mySecondMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_3A = new XYZ(0, -columnSectionWidth / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnMainRebar_3A);
                        rebarIdCollection.Add(columnMainRebar_3A.Id);

                        //Центральный верхний стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , mySecondMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , mySecondMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_4A = new XYZ(0, columnSectionWidth / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnMainRebar_4A);
                        rebarIdCollection.Add(columnMainRebar_4A.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & transitionToOverlap == true & changeColumnSection == false)
                    {
                        //Если переход стыковки со сварки на нахлест без изменения сечения колонны выше
                        //Точки для построения кривфх основных угловых стержней
                        XYZ firstMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLength, 6));
                        XYZ firstMainRebar_p2 = new XYZ(Math.Round(firstMainRebar_p1.X, 6), Math.Round(firstMainRebar_p1.Y, 6), Math.Round(firstMainRebar_p1.Z + columnLength - rebarOutletsLength, 6));
                        XYZ firstMainRebar_p3 = new XYZ(Math.Round(firstMainRebar_p2.X + firstMainRebarDiam, 6), Math.Round(firstMainRebar_p2.Y, 6), Math.Round(firstMainRebar_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ firstMainRebar_p4 = new XYZ(Math.Round(firstMainRebar_p3.X, 6), Math.Round(firstMainRebar_p3.Y, 6), Math.Round(firstMainRebar_p3.Z + rebarOutletsLength, 6));

                        //Точки для построения кривфх основных боковых стержней
                        XYZ secondMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarSecondOutletsLength, 6));
                        XYZ secondMainRebar_p2 = new XYZ(Math.Round(secondMainRebar_p1.X, 6), Math.Round(secondMainRebar_p1.Y, 6), Math.Round(secondMainRebar_p1.Z + columnLength - rebarSecondOutletsLength, 6));
                        XYZ secondMainRebar_p3 = new XYZ(Math.Round(secondMainRebar_p2.X + secondMainRebarDiam, 6), Math.Round(secondMainRebar_p2.Y, 6), Math.Round(secondMainRebar_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ secondMainRebar_p4 = new XYZ(Math.Round(secondMainRebar_p3.X, 6), Math.Round(secondMainRebar_p3.Y, 6), Math.Round(secondMainRebar_p3.Z + rebarSecondOutletsLength, 6));

                        //Кривые основных угловых стержней
                        List<Curve> myFirstMainRebarCurves = new List<Curve>();

                        Curve firstMainLine1 = Line.CreateBound(firstMainRebar_p1, firstMainRebar_p2) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine1);
                        Curve firstMainLine2 = Line.CreateBound(firstMainRebar_p2, firstMainRebar_p3) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine2);
                        Curve firstMainLine3 = Line.CreateBound(firstMainRebar_p3, firstMainRebar_p4) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine3);

                        //Кривые основных боковых стержней
                        List<Curve> mySecondMainRebarCurves = new List<Curve>();

                        Curve secondMainLine1 = Line.CreateBound(secondMainRebar_p1, secondMainRebar_p2) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine1);
                        Curve secondMainLine2 = Line.CreateBound(secondMainRebar_p2, secondMainRebar_p3) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine2);
                        Curve secondMainLine3 = Line.CreateBound(secondMainRebar_p3, secondMainRebar_p4) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine3);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);

                        //Центральный левый стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        rebarIdCollection.Add(columnMainRebar_1A.Id);

                        //Центральный правый стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_2A = new XYZ(+columnSectionHeight / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnMainRebar_2A);
                        rebarIdCollection.Add(columnMainRebar_2A.Id);

                        //Центральный нижний стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3A = new XYZ(0, -columnSectionWidth / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnMainRebar_3A);
                        rebarIdCollection.Add(columnMainRebar_3A.Id);

                        //Центральный верхний стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4A = new XYZ(0, columnSectionWidth / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnMainRebar_4A);
                        rebarIdCollection.Add(columnMainRebar_4A.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainOverlappingRods" & changeColumnSection == true & sectionOffset <= 50 / 304.8)
                    {
                        //Если стыковка стержней в нахлест c изменением сечения колонны выше
                        //Точки для построения кривых основных угловых стержней

                        XYZ firstMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ firstMainRebar_p2 = new XYZ(Math.Round(firstMainRebar_p1.X, 6), Math.Round(firstMainRebar_p1.Y, 6), Math.Round(firstMainRebar_p1.Z + deepeningBarsSize  + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ firstMainRebar_p3 = new XYZ(Math.Round(firstMainRebar_p2.X + deltaXOverlapping, 6), Math.Round(firstMainRebar_p2.Y, 6), Math.Round(firstMainRebar_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ firstMainRebar_p4 = new XYZ(Math.Round(firstMainRebar_p3.X, 6), Math.Round(firstMainRebar_p3.Y, 6), Math.Round(firstMainRebar_p3.Z + rebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых стержней

                        XYZ secondMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ secondMainRebar_p2 = new XYZ(Math.Round(secondMainRebar_p1.X, 6), Math.Round(secondMainRebar_p1.Y, 6), Math.Round(secondMainRebar_p1.Z + deepeningBarsSize + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ secondMainRebar_p3 = new XYZ(Math.Round(secondMainRebar_p2.X + deltaXSecondOverlapping, 6), Math.Round(secondMainRebar_p2.Y, 6), Math.Round(secondMainRebar_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ secondMainRebar_p4 = new XYZ(Math.Round(secondMainRebar_p3.X, 6), Math.Round(secondMainRebar_p3.Y, 6), Math.Round(secondMainRebar_p3.Z + rebarSecondOutletsLength, 6));

                        //Кривые основных угловых стержней
                        List<Curve> myFirstMainRebarCurves = new List<Curve>();

                        Curve firstMainLine1 = Line.CreateBound(firstMainRebar_p1, firstMainRebar_p2) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine1);
                        Curve firstMainLine2 = Line.CreateBound(firstMainRebar_p2, firstMainRebar_p3) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine2);
                        Curve firstMainLine3 = Line.CreateBound(firstMainRebar_p3, firstMainRebar_p4) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine3);

                        //Кривые основных боковых стержней
                        List<Curve> mySecondMainRebarCurves = new List<Curve>();

                        Curve secondMainLine1 = Line.CreateBound(secondMainRebar_p1, secondMainRebar_p2) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine1);
                        Curve secondMainLine2 = Line.CreateBound(secondMainRebar_p2, secondMainRebar_p3) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine2);
                        Curve secondMainLine3 = Line.CreateBound(secondMainRebar_p3, secondMainRebar_p4) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine3);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, alphaOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, -alphaOverlapping);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, alphaOverlapping);
                        XYZ rotate2_p1 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z);
                        XYZ rotate2_p2 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z + 1);
                        Line rotateLine2 = Line.CreateBound(rotate2_p1, rotate2_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, -alphaOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);

                        //Центральный левый стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, alphaSecondOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        rebarIdCollection.Add(columnMainRebar_1A.Id);

                        //Центральный правый стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2A = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnRebar_2A);
                        rebarIdCollection.Add(columnMainRebar_2A.Id);

                        //Центральный нижний стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine2, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3A = new XYZ(0, -columnSectionWidth / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnRebar_3A);
                        rebarIdCollection.Add(columnMainRebar_3A.Id);

                        //Центральный верхний стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine, alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine2, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4A = new XYZ(0, columnSectionWidth / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnRebar_4A);
                        rebarIdCollection.Add(columnMainRebar_4A.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & transitionToOverlap == false & changeColumnSection == true & sectionOffset <= 50 / 304.8)
                    {
                        //Если стыковка стержней на сварке с изменением сечения колонны выше

                        XYZ firstTubWelding_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLength + baseLevelOffset);
                        XYZ secondTubWelding_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarSecondOutletsLength + baseLevelOffset);

                        //Точки для построения кривых основных угловых стержней
                        XYZ firstMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLength, 6));
                        XYZ firstMainRebar_p2 = new XYZ(Math.Round(firstMainRebar_p1.X, 6), Math.Round(firstMainRebar_p1.Y, 6), Math.Round(firstMainRebar_p1.Z + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn) - rebarOutletsLength, 6));
                        XYZ firstMainRebar_p3 = new XYZ(Math.Round(firstMainRebar_p2.X + deltaXWelding, 6), Math.Round(firstMainRebar_p2.Y, 6), Math.Round(firstMainRebar_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ firstMainRebar_p4 = new XYZ(Math.Round(firstMainRebar_p3.X, 6), Math.Round(firstMainRebar_p3.Y, 6), Math.Round(firstMainRebar_p3.Z + rebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых стержней
                        XYZ secondMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z+ rebarSecondOutletsLength, 6));
                        XYZ secondMainRebar_p2 = new XYZ(Math.Round(secondMainRebar_p1.X, 6), Math.Round(secondMainRebar_p1.Y, 6), Math.Round(secondMainRebar_p1.Z + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn)-rebarSecondOutletsLength, 6));
                        XYZ secondMainRebar_p3 = new XYZ(Math.Round(secondMainRebar_p2.X + sectionOffset, 6), Math.Round(secondMainRebar_p2.Y, 6), Math.Round(secondMainRebar_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ secondMainRebar_p4 = new XYZ(Math.Round(secondMainRebar_p3.X, 6), Math.Round(secondMainRebar_p3.Y, 6), Math.Round(secondMainRebar_p3.Z + rebarSecondOutletsLength, 6));

                        //Кривые основных угловых стержней
                        List<Curve> myFirstMainRebarCurves = new List<Curve>();

                        Curve firstMainLine1 = Line.CreateBound(firstMainRebar_p1, firstMainRebar_p2) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine1);
                        Curve firstMainLine2 = Line.CreateBound(firstMainRebar_p2, firstMainRebar_p3) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine2);
                        Curve firstMainLine3 = Line.CreateBound(firstMainRebar_p3, firstMainRebar_p4) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine3);

                        //Кривые основных боковых стержней
                        List<Curve> mySecondMainRebarCurves = new List<Curve>();

                        Curve secondMainLine1 = Line.CreateBound(secondMainRebar_p1, secondMainRebar_p2) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine1);
                        Curve secondMainLine2 = Line.CreateBound(secondMainRebar_p2, secondMainRebar_p3) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine2);
                        Curve secondMainLine3 = Line.CreateBound(secondMainRebar_p3, secondMainRebar_p4) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine3);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, alphaWelding);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        FamilyInstance tubWelding_1 = doc.Create.NewFamilyInstance(firstTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_1.LookupParameter("Диаметр стержня").Set(firstMainRebarDiam);
                        XYZ newPlaсetubWelding_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2 + sectionOffset, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2 + sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_1.Id, newPlaсetubWelding_1);
                        rebarIdCollection.Add(tubWelding_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, -alphaWelding);
                        XYZ newPlaсeСolumnMainRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnMainRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        FamilyInstance tubWelding_2 = doc.Create.NewFamilyInstance(firstTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_2.LookupParameter("Диаметр стержня").Set(firstMainRebarDiam);
                        XYZ newPlaсetubWelding_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2 + sectionOffset, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2 - sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_2.Id, newPlaсetubWelding_2);
                        rebarIdCollection.Add(tubWelding_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, alphaWelding);
                        XYZ rotate2_p1 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z);
                        XYZ rotate2_p2 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z + 1);
                        Line rotateLine2 = Line.CreateBound(rotate2_p1, rotate2_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnMainRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        FamilyInstance tubWelding_3 = doc.Create.NewFamilyInstance(firstTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_3.LookupParameter("Диаметр стержня").Set(firstMainRebarDiam);
                        XYZ newPlaсetubWelding_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2 - sectionOffset, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2 - sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_3.Id, newPlaсetubWelding_3);
                        rebarIdCollection.Add(tubWelding_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, -alphaWelding);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnMainRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);

                        FamilyInstance tubWelding_4 = doc.Create.NewFamilyInstance(firstTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_4.LookupParameter("Диаметр стержня").Set(firstMainRebarDiam);
                        XYZ newPlaсetubWelding_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2 - sectionOffset, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2 + sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_4.Id, newPlaсetubWelding_4);
                        rebarIdCollection.Add(tubWelding_4.Id);

                        //Центральный левый стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        rebarIdCollection.Add(columnMainRebar_1A.Id);

                        FamilyInstance tubWelding_1A = doc.Create.NewFamilyInstance(secondTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_1A.LookupParameter("Диаметр стержня").Set(secondMainRebarDiam);
                        XYZ newPlaсetubWelding_1A = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2 + sectionOffset,0, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_1A.Id, newPlaсetubWelding_1A);
                        rebarIdCollection.Add(tubWelding_1A.Id);

                        //Центральный правый стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2A = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnRebar_2A);
                        rebarIdCollection.Add(columnMainRebar_2A.Id);

                        FamilyInstance tubWelding_2A = doc.Create.NewFamilyInstance(secondTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_2A.LookupParameter("Диаметр стержня").Set(secondMainRebarDiam);
                        XYZ newPlaсetubWelding_2A = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2 - sectionOffset, 0, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_2A.Id, newPlaсetubWelding_2A);
                        rebarIdCollection.Add(tubWelding_2A.Id);

                        //Центральный нижний стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine2, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3A = new XYZ(0, -columnSectionWidth / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnRebar_3A);
                        rebarIdCollection.Add(columnMainRebar_3A.Id);

                        FamilyInstance tubWelding_3A = doc.Create.NewFamilyInstance(secondTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_3A.LookupParameter("Диаметр стержня").Set(secondMainRebarDiam);
                        XYZ newPlaсetubWelding_3A = new XYZ(0, -columnSectionWidth / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2 + sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_3A.Id, newPlaсetubWelding_3A);
                        rebarIdCollection.Add(tubWelding_3A.Id);

                        //Центральный верхний стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine2, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4A = new XYZ(0, columnSectionWidth / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnRebar_4A);
                        rebarIdCollection.Add(columnMainRebar_4A.Id);

                        FamilyInstance tubWelding_4A = doc.Create.NewFamilyInstance(secondTubWelding_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_4A.LookupParameter("Диаметр стержня").Set(secondMainRebarDiam);
                        XYZ newPlaсetubWelding_4A = new XYZ(0, columnSectionWidth / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2 - sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_4A.Id, newPlaсetubWelding_4A);
                        rebarIdCollection.Add(tubWelding_4A.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & transitionToOverlap == true & changeColumnSection == true & sectionOffset <= 50 / 304.8)
                    {
                        //Если переход со сварки на нахлест c изменением сечения колонны выше
                        //Точки для построения кривых основных угловых стержней

                        XYZ firstMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLength, 6));
                        XYZ firstMainRebar_p2 = new XYZ(Math.Round(firstMainRebar_p1.X, 6), Math.Round(firstMainRebar_p1.Y, 6), Math.Round(firstMainRebar_p1.Z + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn) - rebarOutletsLength, 6));
                        XYZ firstMainRebar_p3 = new XYZ(Math.Round(firstMainRebar_p2.X + deltaXOverlapping, 6), Math.Round(firstMainRebar_p2.Y, 6), Math.Round(firstMainRebar_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ firstMainRebar_p4 = new XYZ(Math.Round(firstMainRebar_p3.X, 6), Math.Round(firstMainRebar_p3.Y, 6), Math.Round(firstMainRebar_p3.Z + rebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых стержней

                        XYZ secondMainRebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarSecondOutletsLength, 6));
                        XYZ secondMainRebar_p2 = new XYZ(Math.Round(secondMainRebar_p1.X, 6), Math.Round(secondMainRebar_p1.Y, 6), Math.Round(secondMainRebar_p1.Z + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn) - rebarSecondOutletsLength, 6));
                        XYZ secondMainRebar_p3 = new XYZ(Math.Round(secondMainRebar_p2.X + deltaXSecondOverlapping, 6), Math.Round(secondMainRebar_p2.Y, 6), Math.Round(secondMainRebar_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ secondMainRebar_p4 = new XYZ(Math.Round(secondMainRebar_p3.X, 6), Math.Round(secondMainRebar_p3.Y, 6), Math.Round(secondMainRebar_p3.Z + rebarSecondOutletsLength, 6));

                        //Кривые основных угловых стержней
                        List<Curve> myFirstMainRebarCurves = new List<Curve>();

                        Curve firstMainLine1 = Line.CreateBound(firstMainRebar_p1, firstMainRebar_p2) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine1);
                        Curve firstMainLine2 = Line.CreateBound(firstMainRebar_p2, firstMainRebar_p3) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine2);
                        Curve firstMainLine3 = Line.CreateBound(firstMainRebar_p3, firstMainRebar_p4) as Curve;
                        myFirstMainRebarCurves.Add(firstMainLine3);

                        //Кривые основных боковых стержней
                        List<Curve> mySecondMainRebarCurves = new List<Curve>();

                        Curve secondMainLine1 = Line.CreateBound(secondMainRebar_p1, secondMainRebar_p2) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine1);
                        Curve secondMainLine2 = Line.CreateBound(secondMainRebar_p2, secondMainRebar_p3) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine2);
                        Curve secondMainLine3 = Line.CreateBound(secondMainRebar_p3, secondMainRebar_p4) as Curve;
                        mySecondMainRebarCurves.Add(secondMainLine3);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, alphaOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, -alphaOverlapping);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, alphaOverlapping);
                        XYZ rotate2_p1 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z);
                        XYZ rotate2_p2 = new XYZ(firstMainRebar_p1.X, firstMainRebar_p1.Y, firstMainRebar_p1.Z + 1);
                        Line rotateLine2 = Line.CreateBound(rotate2_p1, rotate2_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myFirstMainRebarType, null, null, myColumn, mainRebarNormal, myFirstMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, -alphaOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - firstMainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + firstMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);

                        //Центральный левый стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, alphaSecondOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        rebarIdCollection.Add(columnMainRebar_1A.Id);

                        //Центральный правый стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2A = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnRebar_2A);
                        rebarIdCollection.Add(columnMainRebar_2A.Id);

                        //Центральный нижний стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine2, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3A = new XYZ(0, -columnSectionWidth / 2 + mainRebarCoverLayer + secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnRebar_3A);
                        rebarIdCollection.Add(columnMainRebar_3A.Id);

                        //Центральный верхний стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, mySecondMainRebarType, null, null, myColumn, mainRebarNormal, mySecondMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine, alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine2, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4A = new XYZ(0, columnSectionWidth / 2 - mainRebarCoverLayer - secondMainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnRebar_4A);
                        rebarIdCollection.Add(columnMainRebar_4A.Id);
                    }

                    //Хомут
                    //Нормаль для построения хомутов
                    XYZ narmalStirrup = new XYZ(0, 0, 1);

                    //Точки для построения кривых стержня хомута опоясывающего
                    XYZ firstRebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionHeight / 2 + mainRebarCoverLayer - firstStirrupRebarDiam, 6)
                        , Math.Round(columnOrigin.Y + columnSectionWidth / 2 - mainRebarCoverLayer + firstStirrupRebarDiam / 2, 6)
                        , Math.Round(columnOrigin.Z + firstStirrupOffset, 6));

                    XYZ firstRebarStirrup_p2 = new XYZ(Math.Round(firstRebarStirrup_p1.X + columnSectionHeight - mainRebarCoverLayer * 2 + firstStirrupRebarDiam, 6)
                        , Math.Round(firstRebarStirrup_p1.Y, 6)
                        , Math.Round(firstRebarStirrup_p1.Z, 6));

                    XYZ firstRebarStirrup_p3 = new XYZ(Math.Round(firstRebarStirrup_p2.X, 6)
                        , Math.Round(firstRebarStirrup_p2.Y - columnSectionWidth + mainRebarCoverLayer * 2 - firstStirrupRebarDiam - firstStirrupRebarDiam/2, 6)
                        , Math.Round(firstRebarStirrup_p2.Z, 6));

                    XYZ firstRebarStirrup_p4 = new XYZ(Math.Round(firstRebarStirrup_p3.X - columnSectionHeight + mainRebarCoverLayer * 2 - firstStirrupRebarDiam, 6)
                        , Math.Round(firstRebarStirrup_p3.Y, 6)
                        , Math.Round(firstRebarStirrup_p3.Z, 6));


                    //Точки для построения кривых стержня хомута дополнительного
                    double deltaXdeltaY1 = Math.Sqrt(Math.Pow(secondStirrupRebarDiam / 2, 2) / 2);
                    double deltaXdeltaY2 = Math.Sqrt(Math.Pow(secondStirrupRebarDiam, 2) / 2);
                    //Бесплодная попытка сделать нормальный размер хомута
                    //double G1 = Math.Sqrt(Math.Pow(secondStirrupRebarBendDiam / 2, 2) + Math.Pow(secondStirrupRebarBendDiam / 2,2))- secondStirrupRebarBendDiam;
                    //double G2 = Math.Sqrt(Math.Pow(secondStirrupRebarDiam / 2, 2) + Math.Pow(secondStirrupRebarDiam / 2, 2));


                    XYZ secondRebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionHeight/2+mainRebarCoverLayer-secondStirrupRebarDiam- deltaXdeltaY1, 6)
                        , Math.Round(columnOrigin.Y- deltaXdeltaY1, 6)
                        , Math.Round(columnOrigin.Z + firstStirrupOffset+ firstStirrupRebarDiam/2+ secondStirrupRebarDiam/2, 6));

                    XYZ secondRebarStirrup_p2 = new XYZ(Math.Round(columnOrigin.X- deltaXdeltaY1, 6)
                        , Math.Round(columnOrigin.Y+ columnSectionWidth/2 - mainRebarCoverLayer+ secondStirrupRebarDiam - deltaXdeltaY1, 6)
                        , Math.Round(secondRebarStirrup_p1.Z, 6));

                    XYZ secondRebarStirrup_p3 = new XYZ(Math.Round(columnOrigin.X + columnSectionHeight / 2 - mainRebarCoverLayer + secondStirrupRebarDiam - deltaXdeltaY1 + deltaXdeltaY2, 6)
                        , Math.Round(columnOrigin.Y- deltaXdeltaY1- deltaXdeltaY2, 6)
                        , Math.Round(secondRebarStirrup_p2.Z, 6));

                    XYZ secondRebarStirrup_p4 = new XYZ(Math.Round(columnOrigin.X- deltaXdeltaY1+ deltaXdeltaY2, 6)
                        , Math.Round(columnOrigin.Y - columnSectionWidth / 2 + mainRebarCoverLayer - secondStirrupRebarDiam - deltaXdeltaY1 - deltaXdeltaY2, 6)
                        , Math.Round(secondRebarStirrup_p3.Z, 6));

                    //Кривые хомута опоясывающего
                    List<Curve> myFirstStirrupCurves = new List<Curve>();

                    Curve firstStirrup_line1 = Line.CreateBound(firstRebarStirrup_p1, firstRebarStirrup_p2) as Curve;
                    myFirstStirrupCurves.Add(firstStirrup_line1);
                    Curve firstStirrup_line2 = Line.CreateBound(firstRebarStirrup_p2, firstRebarStirrup_p3) as Curve;
                    myFirstStirrupCurves.Add(firstStirrup_line2);
                    Curve firstStirrup_line3 = Line.CreateBound(firstRebarStirrup_p3, firstRebarStirrup_p4) as Curve;
                    myFirstStirrupCurves.Add(firstStirrup_line3);
                    Curve firstStirrup_line4 = Line.CreateBound(firstRebarStirrup_p4, firstRebarStirrup_p1) as Curve;
                    myFirstStirrupCurves.Add(firstStirrup_line4);

                    //Кривые хомута дополнительного
                    List<Curve> mySecondStirrupCurves = new List<Curve>();

                    Curve secondStirrup_line1 = Line.CreateBound(secondRebarStirrup_p1, secondRebarStirrup_p2) as Curve;
                    mySecondStirrupCurves.Add(secondStirrup_line1);
                    Curve secondStirrup_line2 = Line.CreateBound(secondRebarStirrup_p2, secondRebarStirrup_p3) as Curve;
                    mySecondStirrupCurves.Add(secondStirrup_line2);
                    Curve secondStirrup_line3 = Line.CreateBound(secondRebarStirrup_p3, secondRebarStirrup_p4) as Curve;
                    mySecondStirrupCurves.Add(secondStirrup_line3);
                    Curve secondStirrup_line4 = Line.CreateBound(secondRebarStirrup_p4, secondRebarStirrup_p1) as Curve;
                    mySecondStirrupCurves.Add(secondStirrup_line4);

                    //Построение нижнего хомута опоясывающего
                    Rebar columnRebarDownFirstStirrup = Rebar.CreateFromCurvesAndShape(doc, myStirrupRebarShape
                        , myFirstStirrupBarTape
                        , myRebarHookType
                        , myRebarHookType
                        , myColumn
                        , narmalStirrup
                        , myFirstStirrupCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                    columnRebarDownFirstStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    columnRebarDownFirstStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                    columnRebarDownFirstStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupSpacing);
                    rebarIdCollection.Add(columnRebarDownFirstStirrup.Id);

                    //Копирование хомута опоясывающего
                    XYZ pointTopStirrupInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupSpacing);
                    List<ElementId> columnRebarFirstTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownFirstStirrup.Id, pointTopStirrupInstallation) as List<ElementId>;
                    Element columnRebarFirstTopStirrup = doc.GetElement(columnRebarFirstTopStirrupIdList.First());

                    //Высота размещения хомутов опоясывающих со стандартным шагом
                    double StirrupStandardInstallationHeigh = columnLength - stirrupIncreasedPlacementHeight - firstStirrupOffset - 50 / 304.8;
                    int StirrupBarElemStandardQuantity = (int)(StirrupStandardInstallationHeigh / standardStirrupSpacing);
                    //Высота установки последнего хомута
                    double lastStirrupInstallationHeigh = columnLength - firstStirrupOffset - 50 / 304.8;

                    columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemStandardQuantity);
                    columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupSpacing);
                    rebarIdCollection.Add(columnRebarFirstTopStirrup.Id);

                    //Копирование хомута 1 последний
                    XYZ pointLastTopStirrupInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                    List<ElementId> columnRebarLastTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownFirstStirrup.Id, pointLastTopStirrupInstallation) as List<ElementId>;
                    Element columnRebarLastTopStirrup = doc.GetElement(columnRebarLastTopStirrupIdList.First());
                    columnRebarLastTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                    rebarIdCollection.Add(columnRebarLastTopStirrup.Id);

                    //Построение нижнего хомута дополнительного
                    Rebar columnRebarDownSecondStirrup = Rebar.CreateFromCurvesAndShape(doc, myStirrupRebarShape
                        , mySecondStirrupBarTape
                        , myRebarHookType
                        , myRebarHookType
                        , myColumn
                        , narmalStirrup
                        , mySecondStirrupCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                    columnRebarDownSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    columnRebarDownSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                    columnRebarDownSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupSpacing);
                    rebarIdCollection.Add(columnRebarDownSecondStirrup.Id);

                    //Копирование хомута дополнительного
                    List<ElementId> columnRebarSecondTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownSecondStirrup.Id, pointTopStirrupInstallation) as List<ElementId>;
                    Element columnRebarSecondTopStirrup = doc.GetElement(columnRebarSecondTopStirrupIdList.First());

                    columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemStandardQuantity);
                    columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupSpacing);
                    rebarIdCollection.Add(columnRebarSecondTopStirrup.Id);

                    // Копирование хомута 2 последний
                    XYZ pointLastSecondTopStirrupInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                    List<ElementId> columnRebarLastSecondTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownSecondStirrup.Id, pointLastSecondTopStirrupInstallation) as List<ElementId>;
                    Element columnRebarLastSecondTopStirrup = doc.GetElement(columnRebarLastSecondTopStirrupIdList.First());
                    columnRebarLastSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                    rebarIdCollection.Add(columnRebarLastSecondTopStirrup.Id);

                    List<Group> projectGroupList = new FilteredElementCollector(doc).OfClass(typeof(Group)).Cast<Group>().ToList();
                    if (projectGroupList.Any(g => g.GroupType.Name == myColumn.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString()))
                    {
                        TaskDialog.Show("Revit", "Группа с имененм "
                            + myColumn.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString()
                            + " уже существует!\nБыли созданы отдельные стержни колонны без группировки!"); ;
                        continue;
                    }
                    else
                    {
                        Group newOutletsGroup = doc.Create.NewGroup(rebarIdCollection);
                        if (myColumn.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == null || myColumn.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == "")
                        {
                            TaskDialog.Show("Revit", "У колонны отсутствует марка!"
                                + "\nИмя группы по умолчанию - " + newOutletsGroup.GroupType.Name);
                        }

                        else
                        {
                            newOutletsGroup.GroupType.Name = myColumn.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                        }

                        if (columnOriginLocationPoint.Rotation != 0)
                        {
                            ElementTransformUtils.RotateElement(doc, newOutletsGroup.Id, rotationAxis, columnRotation);
                        }
                    }
                }

                t.Commit();
            }
            #endregion
            return Result.Succeeded;
        }
    }
}
