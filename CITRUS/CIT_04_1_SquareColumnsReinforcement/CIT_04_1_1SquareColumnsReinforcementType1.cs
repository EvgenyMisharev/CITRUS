using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CITRUS.CIT_04_1_SquareColumnsReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_1_1SquareColumnsReinforcementType1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Execute(commandData.Application);
        }

        public Result Execute(UIApplication uiapp)
        {
            string versString = uiapp.Application.VersionNumber;
            int versNumber = 0;
            Int32.TryParse(versString, out versNumber);
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
            //Завершение блока Получение списка колонн
#endregion

#region Старт блока выбора форм арматурных стержней
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

            //Выбор формы хомута Х_51
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
            //Завершение блока выбора форм арматурных стержней
#endregion

# region Старт блока создания списков типов для 
            //Список типов для выбора основной арматуры
            List<RebarBarType> mainRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов для выбора арматуры хомутов
            List<RebarBarType> stirrupRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов защитных слоев арматуры
            List<RebarCoverType> rebarCoverTypesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();

            //Завершение блока создания списков типов для 
#endregion

#region Старт блока использования формы

            

            //Вызов формы
            CIT_04_1_1FormSquareColumnsReinforcementType1 formSquareColumnsReinforcementType1 = new CIT_04_1_1FormSquareColumnsReinforcementType1(mainRebarTapesList, stirrupRebarTapesList, rebarCoverTypesList);
            formSquareColumnsReinforcementType1.ShowDialog();
            if (formSquareColumnsReinforcementType1.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

#region  Получение данных из формы

            //Выбор типа основной арматуры
            RebarBarType myMainRebarType = formSquareColumnsReinforcementType1.mySelectionMainBarTape;
            //Выбор типа арматуры хомутов
            RebarBarType myStirrupBarTape = formSquareColumnsReinforcementType1.mySelectionStirrupBarTape;
            //Выбор типа защитного слоя основной арматуры
            RebarCoverType myRebarCoverType = formSquareColumnsReinforcementType1.mySelectionRebarCoverType;

            //Диаметр стержня основной арматуры
            Parameter mainRebarTypeDiamParam = myMainRebarType.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double mainRebarDiam = mainRebarTypeDiamParam.AsDouble();
            //Диаметр хомута
            Parameter stirrupRebarTypeDiamParam = myStirrupBarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double stirrupRebarDiam = stirrupRebarTypeDiamParam.AsDouble();
            //Защитный слой арматуры как dooble
            double mainRebarCoverLayer = myRebarCoverType.CoverDistance;


            //Толщина перекрытия над колонной
            double floorThicknessAboveColumn = formSquareColumnsReinforcementType1.FloorThickness / 304.8;
            if(floorThicknessAboveColumn==0)
            {
                TaskDialog.Show("Revit", "Кожаный мешок, не забывай задавать толщину перекрытия!");
                return Result.Cancelled;
            }

            //Длина выпусков
            double rebarOutletsLength = formSquareColumnsReinforcementType1.RebarOutlets / 304.8;

            // Смещение первого хомута от низа колонны
            double firstStirrupOffset = formSquareColumnsReinforcementType1.FirstStirrupOffset / 304.8;

            // Учащенный шаг хомутов
            double increasedStirrupSpacing = formSquareColumnsReinforcementType1.IncreasedStirrupSpacing / 304.8;

            // Стандартный шаг хомутов
            double standardStirrupSpacing = formSquareColumnsReinforcementType1.StandardStirrupSpacing / 304.8;

            //Высота размещения хомутов с учащенным шагом
            double stirrupIncreasedPlacementHeight = formSquareColumnsReinforcementType1.StirrupIncreasedPlacementHeight / 304.8;
            int StirrupBarElemFrequentQuantity = (int)(stirrupIncreasedPlacementHeight / increasedStirrupSpacing) + 1;


            string checkedRebarOutletsButtonName = formSquareColumnsReinforcementType1.CheckedRebarOutletsButtonName;

            //Изменение сечения колонны
            bool changeColumnSection =formSquareColumnsReinforcementType1.СhangeColumnSection;

            //Переход со сварки на нахлест
            bool transitionToOverlap = formSquareColumnsReinforcementType1.TransitionToOverlap;

            //Заглубление стержней
            double deepeningBarsSize = 0;
            bool deepeningBars = formSquareColumnsReinforcementType1.DeepeningBars;
            if (deepeningBars == true)
            {
                deepeningBarsSize = formSquareColumnsReinforcementType1.DeepeningBarsSize / 304.8;
            }
            else
            {
                deepeningBarsSize = 0;
            }

            //Загнуть в плиту
            bool bendIntoASlab = formSquareColumnsReinforcementType1.BendIntoASlab;

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

            double sectionOffset = formSquareColumnsReinforcementType1 .ColumnSectionOffset/ 304.8;
            double deltaXOverlapping = Math.Sqrt(Math.Pow((sectionOffset + mainRebarDiam), 2) + Math.Pow(sectionOffset, 2));
            double alphaOverlapping = Math.Asin(sectionOffset / deltaXOverlapping);
            double deltaXWelding = Math.Sqrt(Math.Pow(sectionOffset, 2) + Math.Pow(sectionOffset, 2));
            double alphaWelding = Math.Asin(sectionOffset / deltaXWelding);

#region Старт блока Размещение арматуры в проекте
            //Открытие транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение арматуры колонн");

                foreach (FamilyInstance myColumn in columnsList)
                {
#region Старт блока сбора параметров колонны

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
                    if  (myColumn.Symbol.LookupParameter("Рзм.Высота") != null)
                    {
                        columnSectionHeight = myColumn.Symbol.LookupParameter("Рзм.Высота").AsDouble();
                    }
                    else
                    {
                        columnSectionHeight = myColumn.Symbol.LookupParameter("ADSK_Размер_Высота").AsDouble();
                    }

                    if (columnSectionWidth != columnSectionHeight)
                    {
                        continue;
                    }

                    if (columnSectionWidth > 400 / 304.8)
                    {
                        TaskDialog.Show("Revit", "Выбранный тип армирования применим для колонн сечением не более 400 мм");
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
                    //Завершение блока сбора параметров колонны
                    #endregion
 
                    //Универсальная коллекция для формирования группы выпусков
                    ICollection<ElementId> rebarIdCollection = new List<ElementId>();

                    #region Старт блока создания арматуры колонны
                    //Нормаль для построения стержней основной арматуры
                    XYZ mainRebarNormal = new XYZ(0, 1, 0);

                    if (checkedRebarOutletsButtonName == "radioButton_MainOverlappingRods" & changeColumnSection == false & bendIntoASlab == false)
                    {
                        //Если стыковка стержней в нахлест без изменения сечения колонны выше
                        //Точки для построения кривфх стержня
                        XYZ rebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6), Math.Round(rebar_p1.Y, 6), Math.Round(rebar_p1.Z + deepeningBarsSize + columnLength, 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + mainRebarDiam, 6), Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ rebar_p4 = new XYZ(Math.Round(rebar_p3.X, 6), Math.Round(rebar_p3.Y, 6), Math.Round(rebar_p3.Z + rebarOutletsLength, 6));

                        //Кривые стержня
                        List<Curve> myMainRebarCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        myMainRebarCurves.Add(line1);
                        Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        myMainRebarCurves.Add(line2);
                        Curve line3 = Line.CreateBound(rebar_p3, rebar_p4) as Curve;
                        myMainRebarCurves.Add(line3);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainOverlappingRods" & changeColumnSection == false & bendIntoASlab == true)
                    {
                        //Если стыковка стержней в нахлест загиб в плиту
                        //Точки для построения кривфх стержня
                        XYZ rebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6), Math.Round(rebar_p1.Y, 6), Math.Round(rebar_p1.Z + deepeningBarsSize + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + rebarOutletsLength - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z, 6));

                        //Кривые стержня
                        List<Curve> myMainRebarCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        myMainRebarCurves.Add(line1);
                        Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        myMainRebarCurves.Add(line2);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, 180 * (Math.PI / 180));

                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & changeColumnSection == false & transitionToOverlap == false & bendIntoASlab == false)
                    {
                        //Если стыковка стержней на сварке без изменения сечения колонны выше
                        //Точки для построения кривфх стержня
                        XYZ rebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLength, 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6), Math.Round(rebar_p1.Y, 6), Math.Round(rebar_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                        XYZ tubWelding_p0 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLength + baseLevelOffset);
                        //Кривые стержня
                        List<Curve> myMainRebarCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        myMainRebarCurves.Add(line1);


                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        FamilyInstance tubWelding_1 = doc.Create.NewFamilyInstance(tubWelding_p0, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_1.LookupParameter("Диаметр стержня").Set(mainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(tubWelding_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnMainRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        FamilyInstance tubWelding_2 = doc.Create.NewFamilyInstance(tubWelding_p0, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_2.LookupParameter("Диаметр стержня").Set(mainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_2.Id, newPlaсeСolumnMainRebar_2);
                        rebarIdCollection.Add(tubWelding_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnMainRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        FamilyInstance tubWelding_3 = doc.Create.NewFamilyInstance(tubWelding_p0, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_3.LookupParameter("Диаметр стержня").Set(mainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_3.Id, newPlaсeСolumnMainRebar_3);
                        rebarIdCollection.Add(tubWelding_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeWeldingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnMainRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);

                        FamilyInstance tubWelding_4 = doc.Create.NewFamilyInstance(tubWelding_p0, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_4.LookupParameter("Диаметр стержня").Set(mainRebarDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_4.Id, newPlaсeСolumnMainRebar_4);
                        rebarIdCollection.Add(tubWelding_4.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & changeColumnSection == false & transitionToOverlap == false & bendIntoASlab == true)
                    {
                        //Если стыковка стержней на сварке загиб в плиту
                        //Точки для построения кривфх стержня
                        XYZ rebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLength, 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6), Math.Round(rebar_p1.Y, 6), Math.Round(rebar_p1.Z - rebarOutletsLength + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + rebarOutletsLength - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z, 6));

                        //Кривые стержня
                        List<Curve> myMainRebarCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        myMainRebarCurves.Add(line1);
                        Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        myMainRebarCurves.Add(line2);


                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, 180 * (Math.PI / 180));

                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarType
                            , null
                            , null
                            , myColumn
                            , mainRebarNormal
                            , myMainRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & changeColumnSection == false & transitionToOverlap == true)
                    {
                        //Если стыковка стержней в нахлест без изменения сечения колонны выше
                        //Точки для построения кривфх стержня
                        XYZ rebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLength, 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6), Math.Round(rebar_p1.Y, 6), Math.Round(rebar_p1.Z + columnLength - +rebarOutletsLength, 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + mainRebarDiam, 6), Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ rebar_p4 = new XYZ(Math.Round(rebar_p3.X, 6), Math.Round(rebar_p3.Y, 6), Math.Round(rebar_p3.Z + rebarOutletsLength, 6));

                        //Кривые стержня
                        List<Curve> myMainRebarCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        myMainRebarCurves.Add(line1);
                        Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        myMainRebarCurves.Add(line2);
                        Curve line3 = Line.CreateBound(rebar_p3, rebar_p4) as Curve;
                        myMainRebarCurves.Add(line3);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainOverlappingRods" & changeColumnSection == true & sectionOffset <= 50 / 304.8)
                    {
                        //Если стыковка стержней в нахлест c изменением сечения колонны выше
                        //Точки для построения кривфх стержня

                        XYZ rebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6), Math.Round(rebar_p1.Y, 6), Math.Round(rebar_p1.Z + deepeningBarsSize + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + deltaXOverlapping, 6), Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ rebar_p4 = new XYZ(Math.Round(rebar_p3.X, 6), Math.Round(rebar_p3.Y, 6), Math.Round(rebar_p3.Z + rebarOutletsLength, 6));

                        //Кривые стержня
                        List<Curve> myMainRebarCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        myMainRebarCurves.Add(line1);
                        Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        myMainRebarCurves.Add(line2);
                        Curve line3 = Line.CreateBound(rebar_p3, rebar_p4) as Curve;
                        myMainRebarCurves.Add(line3);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, alphaOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, -alphaOverlapping);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, alphaOverlapping);
                        XYZ rotate2_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate2_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine2 = Line.CreateBound(rotate2_p1, rotate2_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, -alphaOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & changeColumnSection == true & transitionToOverlap == false & sectionOffset <= 50 / 304.8)
                    {
                        //Если стыковка стержней на сварке с изменением сечения колонны выше

                        XYZ tubWelding_p0 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLength + baseLevelOffset);

                        //Точки для построения кривфх стержня
                        XYZ rebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLength, 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6), Math.Round(rebar_p1.Y, 6), Math.Round(rebar_p1.Z + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn) - rebarOutletsLength, 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + deltaXWelding, 6), Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ rebar_p4 = new XYZ(Math.Round(rebar_p3.X, 6), Math.Round(rebar_p3.Y, 6), Math.Round(rebar_p3.Z + rebarOutletsLength, 6));

                        //Кривые стержня
                        List<Curve> myMainRebarCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        myMainRebarCurves.Add(line1);
                        Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        myMainRebarCurves.Add(line2);
                        Curve line3 = Line.CreateBound(rebar_p3, rebar_p4) as Curve;
                        myMainRebarCurves.Add(line3);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, alphaWelding);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        FamilyInstance tubWelding_1 = doc.Create.NewFamilyInstance(tubWelding_p0, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_1.LookupParameter("Диаметр стержня").Set(mainRebarDiam);
                        XYZ newPlaсetubWelding_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2 + sectionOffset, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2 + sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_1.Id, newPlaсetubWelding_1);
                        rebarIdCollection.Add(tubWelding_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, -alphaWelding);
                        XYZ newPlaсeСolumnMainRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnMainRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        FamilyInstance tubWelding_2 = doc.Create.NewFamilyInstance(tubWelding_p0, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_2.LookupParameter("Диаметр стержня").Set(mainRebarDiam);
                        XYZ newPlaсetubWelding_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2 + sectionOffset, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2 - sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_2.Id, newPlaсetubWelding_2);
                        rebarIdCollection.Add(tubWelding_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, alphaWelding);
                        XYZ rotate2_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate2_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine2 = Line.CreateBound(rotate2_p1, rotate2_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnMainRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);

                        FamilyInstance tubWelding_3 = doc.Create.NewFamilyInstance(tubWelding_p0, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_3.LookupParameter("Диаметр стержня").Set(mainRebarDiam);
                        XYZ newPlaсetubWelding_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2 - sectionOffset, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2 - sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_3.Id, newPlaсetubWelding_3);
                        rebarIdCollection.Add(tubWelding_3.Id);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, -alphaWelding);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnMainRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);

                        FamilyInstance tubWelding_4 = doc.Create.NewFamilyInstance(tubWelding_p0, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWelding_4.LookupParameter("Диаметр стержня").Set(mainRebarDiam);
                        XYZ newPlaсetubWelding_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2 - sectionOffset, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2 + sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, tubWelding_4.Id, newPlaсetubWelding_4);
                        rebarIdCollection.Add(tubWelding_4.Id);
                    }

                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & changeColumnSection == true & transitionToOverlap == true & sectionOffset <= 50 / 304.8)
                    {
                        //Если стыковка стержней в нахлест c изменением сечения колонны выше
                        //Точки для построения кривфх стержня

                        XYZ rebar_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLength, 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6), Math.Round(rebar_p1.Y, 6), Math.Round(rebar_p1.Z + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn) - rebarOutletsLength, 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + deltaXOverlapping, 6), Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ rebar_p4 = new XYZ(Math.Round(rebar_p3.X, 6), Math.Round(rebar_p3.Y, 6), Math.Round(rebar_p3.Z + rebarOutletsLength, 6));

                        //Кривые стержня
                        List<Curve> myMainRebarCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        myMainRebarCurves.Add(line1);
                        Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        myMainRebarCurves.Add(line2);
                        Curve line3 = Line.CreateBound(rebar_p3, rebar_p4) as Curve;
                        myMainRebarCurves.Add(line3);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, alphaOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        rebarIdCollection.Add(columnMainRebar_1.Id);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, -alphaOverlapping);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        rebarIdCollection.Add(columnMainRebar_2.Id);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, alphaOverlapping);
                        XYZ rotate2_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate2_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine2 = Line.CreateBound(rotate2_p1, rotate2_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        rebarIdCollection.Add(columnMainRebar_3.Id);
                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc, myMainRebarShapeOverlappingRods, myMainRebarType, null, null, myColumn, mainRebarNormal, myMainRebarCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, -alphaOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiam / 2, -columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        rebarIdCollection.Add(columnMainRebar_4.Id);
                    }

                    //Хомут
                    //Нормаль для построения хомута
                    XYZ narmalStirrup = new XYZ(0, 0, 1);

                    //Точки для построения кривых стержня хомута
                    XYZ rebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionHeight / 2 + mainRebarCoverLayer - stirrupRebarDiam, 6)
                        , Math.Round(columnOrigin.Y + columnSectionWidth / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2, 6)
                        , Math.Round(columnOrigin.Z + firstStirrupOffset, 6));

                    XYZ rebarStirrup_p2 = new XYZ(Math.Round(rebarStirrup_p1.X + columnSectionHeight - mainRebarCoverLayer * 2 + stirrupRebarDiam, 6)
                        , Math.Round(rebarStirrup_p1.Y, 6)
                        , Math.Round(rebarStirrup_p1.Z, 6));

                    XYZ rebarStirrup_p3 = new XYZ(Math.Round(rebarStirrup_p2.X, 6)
                        , Math.Round(rebarStirrup_p2.Y - columnSectionWidth + mainRebarCoverLayer * 2 - stirrupRebarDiam - stirrupRebarDiam/2, 6)
                        , Math.Round(rebarStirrup_p2.Z, 6));

                    XYZ rebarStirrup_p4 = new XYZ(Math.Round(rebarStirrup_p3.X - columnSectionHeight + mainRebarCoverLayer * 2 - stirrupRebarDiam, 6)
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

                    //Построение нижнего хомута
                    Rebar columnRebarDownStirrup = Rebar.CreateFromCurvesAndShape(doc, myStirrupRebarShape, myStirrupBarTape, myRebarHookType, myRebarHookType, myColumn, narmalStirrup, myStirrupCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);

                    columnRebarDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    columnRebarDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                    columnRebarDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupSpacing);
                    rebarIdCollection.Add(columnRebarDownStirrup.Id);

                    //Копирование хомута
                    XYZ pointTopStirrupInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupSpacing);
                    List<ElementId> columnRebarTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownStirrup.Id, pointTopStirrupInstallation) as List<ElementId>;
                    Element columnRebarTopStirrup = doc.GetElement(columnRebarTopStirrupIdList.First());

                    //Высота размещения хомутов со стандартным шагом
                    double StirrupStandardInstallationHeigh = columnLength - stirrupIncreasedPlacementHeight - firstStirrupOffset - 50 / 304.8;
                    int StirrupBarElemStandardQuantity = (int)(StirrupStandardInstallationHeigh / standardStirrupSpacing);
                    //Высота установки последнего хомута
                    double lastStirrupInstallationHeigh = columnLength - firstStirrupOffset - 50 / 304.8;

                    columnRebarTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    columnRebarTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemStandardQuantity);
                    columnRebarTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupSpacing);
                    rebarIdCollection.Add(columnRebarTopStirrup.Id);

                    //Копирование хомута последний
                    XYZ pointLastTopStirrupInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                    List<ElementId> columnRebarLastTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownStirrup.Id, pointLastTopStirrupInstallation) as List<ElementId>;
                    Element columnRebarLastTopStirrup = doc.GetElement(columnRebarLastTopStirrupIdList.First());
                    columnRebarLastTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                    rebarIdCollection.Add(columnRebarLastTopStirrup.Id);

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
#endregion
                t.Commit();
            }
#endregion
            return Result.Succeeded;
        }
    }
}
