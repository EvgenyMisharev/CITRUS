using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_2_RectangularColumnsReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_2_RectangularColumnsReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
#region Выбор колонн
            ColumnSelectionFilter columnSelFilter = new ColumnSelectionFilter(); //Вызов фильтра выбора
            IList<Reference> selColumns = sel.PickObjects(ObjectType.Element, columnSelFilter, "Выберите колонны!");//Получение списка ссылок на выбранные колонны

            List<FamilyInstance> columnsList = new List<FamilyInstance>();//Получение списка выбранных колонн
            foreach (Reference columnRef in selColumns)
            {
                columnsList.Add(doc.GetElement(columnRef) as FamilyInstance);
            }
            //Завершение блока Получение списка колонн
#endregion

#region Выбор форм арматурных стержней и загибов
            // Выбор формы основной арматуры если стыковка стержней в нахлест
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

            //Выбор формы шпильки 
            List<RebarShape> rebarPinShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "02")
                .Cast<RebarShape>()
                .ToList();
            if (rebarPinShapeList.Count == 0)
            {
                rebarPinShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "Х_(22)")
                .Cast<RebarShape>()
                .ToList();
                if (rebarPinShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 02 или Х_(22) не найдена");
                    return Result.Failed;
                }
            }
            RebarShape myPinRebarShape = rebarPinShapeList.First();

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

            //Выбор формы загиба шпильки
            List<RebarHookType> rebarPinHookTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarHookType))
                .Where(rs => rs.Name.ToString() == "Стандартный - 180 градусов")
                .Cast<RebarHookType>()
                .ToList();
            if (rebarPinHookTypeList.Count == 0)
            {
                rebarPinHookTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarHookType))
                .Where(rs => rs.Name.ToString() == "Хомут/стяжка_180°")
                .Cast<RebarHookType>()
                .ToList();
                if (rebarPinHookTypeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма загиба Стандартный - 180 градусов или Хомут/стяжка_180° не найдена");
                    return Result.Failed;
                }
            }
            RebarHookType myRebarPinHookType = rebarPinHookTypeList.First();
            //Завершение блока выбора форм арматурных стержней
#endregion

#region Создание списков типов для формы
            //Список типов для выбора основной арматуры
            List<RebarBarType> mainRebarOneTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            List<RebarBarType> mainRebarTwoTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            List<RebarBarType> mainRebarThreeTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов для выбора арматуры хомутов
            List<RebarBarType> stirrupRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов для выбора арматуры хомутов
            List<RebarBarType> pinRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов защитных слоев арматуры
            List<RebarCoverType> rebarCoverTypesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();
            //Завершение блока создания списков типов для формы
            #endregion
            
#region Вызов и обработка результатов формы
            //Вызов формы
            CIT_04_2_RectangularColumnsReinforcementForm rectangularColumnsReinforcementForm 
                = new CIT_04_2_RectangularColumnsReinforcementForm(mainRebarOneTapesList
                , mainRebarTwoTapesList
                , mainRebarThreeTapesList
                , stirrupRebarTapesList
                , pinRebarTapesList
                , rebarCoverTypesList);
            rectangularColumnsReinforcementForm.ShowDialog();
            if (rectangularColumnsReinforcementForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            //Выбор типа основной арматуры
            RebarBarType myMainRebarTypeOne = rectangularColumnsReinforcementForm.mySelectionMainBarTapeOne;
            RebarBarType myMainRebarTypeTwo = rectangularColumnsReinforcementForm.mySelectionMainBarTapeTwo;
            RebarBarType myMainRebarTypeThree = rectangularColumnsReinforcementForm.mySelectionMainBarTapeThree;
            //Выбор типа арматуры хомутов
            RebarBarType myStirrupBarTape = rectangularColumnsReinforcementForm.mySelectionStirrupBarTape;
            RebarBarType myPinBarTape = rectangularColumnsReinforcementForm.mySelectionPinBarTape;
            //Выбор типа защитного слоя основной арматуры
            RebarCoverType myRebarCoverType = rectangularColumnsReinforcementForm.mySelectionRebarCoverType;

            //Диаметр стержня основной арматуры
            Parameter mainRebarTypeOneDiamParam = myMainRebarTypeOne.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double mainRebarDiamTypeOne = mainRebarTypeOneDiamParam.AsDouble();
            Parameter mainRebarTypeTwoDiamParam = myMainRebarTypeTwo.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double mainRebarDiamTypeTwo = mainRebarTypeTwoDiamParam.AsDouble();
            Parameter mainRebarTypeThreeDiamParam = myMainRebarTypeThree.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double mainRebarDiamTypeThree = mainRebarTypeThreeDiamParam.AsDouble();

            //Диаметр хомута
            Parameter stirrupRebarTypeDiamParam = myStirrupBarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double stirrupRebarDiam = stirrupRebarTypeDiamParam.AsDouble();
            //Диаметр шпильки
            Parameter pinRebarTypeDiamParam = myPinBarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double pinRebarDiam = pinRebarTypeDiamParam.AsDouble();
            //Защитный слой арматуры как dooble
            double mainRebarCoverLayer = myRebarCoverType.CoverDistance;

            //Кол-во стержней по левой и правой граням
            int numberOfBarsLRFaces = rectangularColumnsReinforcementForm.NumberOfBarsLRFaces;
            int numberOfBarsTBFaces = rectangularColumnsReinforcementForm.NumberOfBarsTBFaces;

            //Длины выпусков
            double rebarOutletsLengthLong = rectangularColumnsReinforcementForm.RebarOutletsLengthLong / 304.8;
            double rebarOutletsLengthShort = rectangularColumnsReinforcementForm.RebarOutletsLengthShort / 304.8;

            double floorThicknessAboveColumn = rectangularColumnsReinforcementForm.FloorThicknessAboveColumn / 304.8;
            double standardStirrupStep = rectangularColumnsReinforcementForm.StandardStirrupStep / 304.8;
            double increasedStirrupStep = rectangularColumnsReinforcementForm.IncreasedStirrupStep / 304.8;
            double firstStirrupOffset = rectangularColumnsReinforcementForm.FirstStirrupOffset / 304.8;
            double stirrupIncreasedPlacementHeight = rectangularColumnsReinforcementForm.StirrupIncreasedPlacementHeight / 304.8;
            int StirrupBarElemFrequentQuantity = (int)(stirrupIncreasedPlacementHeight / increasedStirrupStep) + 1;

            string checkedRebarOutletsButtonName = rectangularColumnsReinforcementForm.CheckedRebarOutletsButtonName;
            string checkedRebarStrappingTypeButtonName = rectangularColumnsReinforcementForm.CheckedRebarStrappingTypeButtonName;

            //Переход со сварки на нахлест
            bool transitionToOverlap = rectangularColumnsReinforcementForm.TransitionToOverlap;

            //Изменение сечения колонны
            bool changeColumnSection = rectangularColumnsReinforcementForm.СhangeColumnSection;
            double sectionOffset = rectangularColumnsReinforcementForm.ColumnSectionOffset / 304.8;

            //Заглубление стержней
            double deepeningBarsSize = 0;
            bool deepeningBars = rectangularColumnsReinforcementForm.DeepeningBars;
            if (deepeningBars == true)
            {
                deepeningBarsSize = rectangularColumnsReinforcementForm.DeepeningBarsSize / 304.8;
            }
            else
            {
                deepeningBarsSize = 0;
            }

            //Загнуть в плиту
            bool bendIntoASlab = rectangularColumnsReinforcementForm.BendIntoASlab;

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

            //Нормаль для построения стержней основной арматуры
            XYZ mainRebarNormalMain = new XYZ(0, 1, 0);
            XYZ mainRebarNormalAdditional = new XYZ(1, 0, 0);

            //Открытие транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение арматуры колонн");
                foreach (FamilyInstance myColumn in columnsList)
                {
#region Сбор параметров колонны
                    // Базовый уровень
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

                    if (columnSectionWidth == columnSectionHeight)
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
#endregion
                    //Старт блока задания параметра защитного слоя боковых граней колонны
                    //Защитный слой арматуры боковых граней
                    Parameter clearCoverOther = myColumn.get_Parameter(BuiltInParameter.CLEAR_COVER_OTHER);
                    clearCoverOther.Set(myRebarCoverType.Id);
                    //Завершение блока сбора параметров колонны

                    int numberOfSpacesLRFacesForStirrup = numberOfBarsLRFaces - 3;
                    double stepBarsLRFacesForStirrup = 0;
                    double stepBarsTBFacesForStirrup = 0;
                    double residueForOffsetForStirrupLR = 0;
                    double residueForOffsetForStirrupTB = 0;
                    //Универсальная коллекция для формирования группы выпусков
                    ICollection<ElementId> rebarIdCollection = new List<ElementId>();

                    //Если стыковка стержней в нахлест без изменения сечения колонны выше
                    if (checkedRebarOutletsButtonName == "radioButton_MainOverlappingRods" & changeColumnSection == false & bendIntoASlab == false)
                    {
#region Угловые стержни
                        //Точки для построения кривых стержня один длинного
                        XYZ mainRebarTypeOneLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                        , Math.Round(columnOrigin.Y, 6)
                        , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ mainRebarTypeOneLong_p2 = new XYZ(Math.Round(mainRebarTypeOneLong_p1.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Z + deepeningBarsSize + columnLength, 6));
                        XYZ mainRebarTypeOneLong_p3 = new XYZ(Math.Round(mainRebarTypeOneLong_p2.X + mainRebarDiamTypeOne, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ mainRebarTypeOneLong_p4 = new XYZ(Math.Round(mainRebarTypeOneLong_p3.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Z + rebarOutletsLengthLong, 6));

                        //Точки для построения кривых стержня один короткого
                        XYZ mainRebarTypeOneShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                            , Math.Round(columnOrigin.Y, 6)
                            , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ mainRebarTypeOneShort_p2 = new XYZ(Math.Round(mainRebarTypeOneShort_p1.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Z + deepeningBarsSize + columnLength, 6));
                        XYZ mainRebarTypeOneShort_p3 = new XYZ(Math.Round(mainRebarTypeOneShort_p2.X + mainRebarDiamTypeOne, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ mainRebarTypeOneShort_p4 = new XYZ(Math.Round(mainRebarTypeOneShort_p3.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Z + rebarOutletsLengthShort, 6));

                        //Кривые стержня один длинного
                        List<Curve> myMainRebarTypeOneCurvesLong = new List<Curve>();

                        Curve myMainRebarTypeOneLong_line1 = Line.CreateBound(mainRebarTypeOneLong_p1, mainRebarTypeOneLong_p2) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line1);
                        Curve myMainRebarTypeOneLong_line2 = Line.CreateBound(mainRebarTypeOneLong_p2, mainRebarTypeOneLong_p3) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line2);
                        Curve myMainRebarTypeOneLong_line3 = Line.CreateBound(mainRebarTypeOneLong_p3, mainRebarTypeOneLong_p4) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line3);

                        //Кривые стержня один короткого
                        List<Curve> myMainRebarTypeOneCurvesShort = new List<Curve>();

                        Curve myMainRebarTypeOneShort_line1 = Line.CreateBound(mainRebarTypeOneShort_p1, mainRebarTypeOneShort_p2) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line1);
                        Curve myMainRebarTypeOneShort_line2 = Line.CreateBound(mainRebarTypeOneShort_p2, mainRebarTypeOneShort_p3) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line2);
                        Curve myMainRebarTypeOneShort_line3 = Line.CreateBound(mainRebarTypeOneShort_p3, mainRebarTypeOneShort_p4) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line3);

                        //Нижний левый угол
                        Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);

                        rebarIdCollection.Add(columnMainRebarLowerLeftСorner.Id);

                        //Верхний левый угол
                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }
                        else if (numberOfBarsLRFaces % 2 == 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }

                        //Верхний правый угол
                        Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ rotate_p1 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z);
                        XYZ rotate_p2 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebarUpperRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);

                        rebarIdCollection.Add(columnMainRebarUpperRightСorner.Id);

                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }

                        if (numberOfBarsLRFaces % 2 == 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesShort
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }
#endregion

#region Стержни по левой и правой граням
                        if (numberOfBarsLRFaces >= 3)
                        {
                            //Точки для построения кривфх стержня два удлиненного
                            XYZ mainRebarTypeTwoLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                            XYZ mainRebarTypeTwoLong_p2 = new XYZ(Math.Round(mainRebarTypeTwoLong_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Z + deepeningBarsSize + columnLength, 6));
                            XYZ mainRebarTypeTwoLong_p3 = new XYZ(Math.Round(mainRebarTypeTwoLong_p2.X + mainRebarDiamTypeTwo, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeTwoLong_p4 = new XYZ(Math.Round(mainRebarTypeTwoLong_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня два укороченного
                            XYZ mainRebarTypeTwoShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                            XYZ mainRebarTypeTwoShort_p2 = new XYZ(Math.Round(mainRebarTypeTwoShort_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoShort_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p1.Z + deepeningBarsSize + columnLength, 6));
                            XYZ mainRebarTypeTwoShort_p3 = new XYZ(Math.Round(mainRebarTypeTwoShort_p2.X + mainRebarDiamTypeTwo, 6)
                                , Math.Round(mainRebarTypeTwoShort_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeTwoShort_p4 = new XYZ(Math.Round(mainRebarTypeTwoShort_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoShort_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p3.Z + rebarOutletsLengthShort, 6));

                            //Кривые стержня удлиненного
                            List<Curve> myMainRebarTypeTwoCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeTwoLong_line1 = Line.CreateBound(mainRebarTypeTwoLong_p1, mainRebarTypeTwoLong_p2) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line1);
                            Curve myMainRebarTypeTwoLong_line2 = Line.CreateBound(mainRebarTypeTwoLong_p2, mainRebarTypeTwoLong_p3) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line2);
                            Curve myMainRebarTypeTwoLong_line3 = Line.CreateBound(mainRebarTypeTwoLong_p3, mainRebarTypeTwoLong_p4) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line3);

                            //Кривые стержня укороченного
                            List<Curve> myMainRebarTypeTwoCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeTwoShort_line1 = Line.CreateBound(mainRebarTypeTwoShort_p1, mainRebarTypeTwoShort_p2) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line1);
                            Curve myMainRebarTypeTwoShort_line2 = Line.CreateBound(mainRebarTypeTwoShort_p2, mainRebarTypeTwoShort_p3) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line2);
                            Curve myMainRebarTypeTwoShort_line3 = Line.CreateBound(mainRebarTypeTwoShort_p3, mainRebarTypeTwoShort_p4) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line3);

                            //Левая грань короткие
                            Rebar columnMainRebarLeftFaceLong = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Расчеты для размещения стержней
                            int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                            double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                            stepBarsLRFacesForStirrup = stepBarsLRFaces;
                            double residueForOffsetLR = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                            residueForOffsetForStirrupLR = residueForOffsetLR;

                            XYZ newPlaсeСolumnMainRebarLeftFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            rebarIdCollection.Add(columnMainRebarLeftFaceLong.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Левая грань длинные
                                Rebar columnMainRebarLeftFaceShort = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeTwo
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeTwoCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarLeftFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces * 2 + residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);

                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                                rebarIdCollection.Add(columnMainRebarLeftFaceShort.Id);
                            }

                            //Правая грань короткий
                            Rebar columnMainRebarRightFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarRightFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            rebarIdCollection.Add(columnMainRebarRightFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Правая грань длинный
                                Rebar columnMainRebarRightFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeTwo
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalMain
                                    , myMainRebarTypeTwoCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeColumnMainRebarRightFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces * 2 - residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                                rebarIdCollection.Add(columnMainRebarRightFaceLong.Id);
                            }
                        }
#endregion

#region Стержни по нижней и верхней граням
                        if (numberOfBarsTBFaces >= 3)
                        {
                            //Точки для построения кривых стержня три длинного
                            XYZ mainRebarTypeThreeLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                            XYZ mainRebarTypeThreeLong_p2 = new XYZ(Math.Round(mainRebarTypeThreeLong_p1.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeThreeLong_p1.Z + deepeningBarsSize + columnLength, 6));
                            XYZ mainRebarTypeThreeLong_p3 = new XYZ(Math.Round(mainRebarTypeThreeLong_p2.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p2.Y + mainRebarDiamTypeThree, 6)
                                , Math.Round(mainRebarTypeThreeLong_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeThreeLong_p4 = new XYZ(Math.Round(mainRebarTypeThreeLong_p3.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeThreeLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня три короткого
                            XYZ mainRebarTypeThreeShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                            XYZ mainRebarTypeThreeShort_p2 = new XYZ(Math.Round(mainRebarTypeThreeShort_p1.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p1.Y, 6)
                                , Math.Round(mainRebarTypeThreeShort_p1.Z + deepeningBarsSize + columnLength, 6));
                            XYZ mainRebarTypeThreeShort_p3 = new XYZ(Math.Round(mainRebarTypeThreeShort_p2.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p2.Y + mainRebarDiamTypeThree, 6)
                                , Math.Round(mainRebarTypeThreeShort_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeThreeShort_p4 = new XYZ(Math.Round(mainRebarTypeThreeShort_p3.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p3.Y, 6)
                                , Math.Round(mainRebarTypeThreeShort_p3.Z + rebarOutletsLengthShort, 6));

                            //Кривые стержня длинного
                            List<Curve> myMainRebarTypeThreeCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeThreeLong_line1 = Line.CreateBound(mainRebarTypeThreeLong_p1, mainRebarTypeThreeLong_p2) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line1);
                            Curve myMainRebarTypeThreeLong_line2 = Line.CreateBound(mainRebarTypeThreeLong_p2, mainRebarTypeThreeLong_p3) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line2);
                            Curve myMainRebarTypeThreeLong_line3 = Line.CreateBound(mainRebarTypeThreeLong_p3, mainRebarTypeThreeLong_p4) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line3);

                            //Кривые стержня короткого
                            List<Curve> myMainRebarTypeThreeCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeThreeShort_line1 = Line.CreateBound(mainRebarTypeThreeShort_p1, mainRebarTypeThreeShort_p2) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line1);
                            Curve myMainRebarTypeThreeShort_line2 = Line.CreateBound(mainRebarTypeThreeShort_p2, mainRebarTypeThreeShort_p3) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line2);
                            Curve myMainRebarTypeThreeShort_line3 = Line.CreateBound(mainRebarTypeThreeShort_p3, mainRebarTypeThreeShort_p4) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line3);

                            //Нижняя грань короткие
                            Rebar columnMainRebarBottomFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Cтержни нижняя и верхняя грани
                            int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                            double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                            stepBarsTBFacesForStirrup = stepBarsTBFaces;
                            double residueForOffsetTB = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;
                            residueForOffsetForStirrupTB = residueForOffsetTB;

                            XYZ newPlaсeСolumnMainRebarBottomFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffsetTB
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            rebarIdCollection.Add(columnMainRebarBottomFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Нижняя грань длинные
                                Rebar columnMainRebarBottomFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarBottomFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces * 2 + residueForOffsetTB
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                                rebarIdCollection.Add(columnMainRebarBottomFaceLong.Id);
                            }

                            //Верхняя грань короткие
                            Rebar columnMainRebarTopFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarTopFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffsetTB
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            rebarIdCollection.Add(columnMainRebarTopFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Верхняя грань длинные
                                Rebar columnMainRebarTopFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarTopFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces * 2 - residueForOffsetTB
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                                rebarIdCollection.Add(columnMainRebarTopFaceLong.Id);
                            }
                        }

#endregion
                    }
                    //Если стыковка стержней в нахлест загиб в плиту
                    else if (checkedRebarOutletsButtonName == "radioButton_MainOverlappingRods" & changeColumnSection == false & bendIntoASlab == true)
                    {
#region Угловые стержни
                        //Точки для построения кривфх основных угловых стержней
                        XYZ mainRebarTypeOneLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ mainRebarTypeOneLong_p2 = new XYZ(Math.Round(mainRebarTypeOneLong_p1.X, 6), Math.Round(mainRebarTypeOneLong_p1.Y, 6), Math.Round(mainRebarTypeOneLong_p1.Z + deepeningBarsSize + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                        XYZ mainRebarTypeOneLong_p3 = new XYZ(Math.Round(mainRebarTypeOneLong_p2.X + rebarOutletsLengthLong - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(mainRebarTypeOneLong_p2.Y, 6), Math.Round(mainRebarTypeOneLong_p2.Z, 6));

                        //Кривые стержня один длинного
                        List<Curve> myMainRebarTypeOneCurvesLong = new List<Curve>();

                        Curve myMainRebarTypeOneLong_line1 = Line.CreateBound(mainRebarTypeOneLong_p1, mainRebarTypeOneLong_p2) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line1);
                        Curve myMainRebarTypeOneLong_line2 = Line.CreateBound(mainRebarTypeOneLong_p2, mainRebarTypeOneLong_p3) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line2);

                        //Нижний левый угол
                        Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeRodsBendIntoASlab
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ rotate_p1 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z);
                        XYZ rotate_p2 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebarLowerLeftСorner.Id, rotateLine, 180 * (Math.PI / 180));

                        XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);

                        rebarIdCollection.Add(columnMainRebarLowerLeftСorner.Id);

                        //Верхний левый угол
                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarUpperLeftСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }
                        else if (numberOfBarsLRFaces % 2 == 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarUpperLeftСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }

                        //Верхний правый угол
                        Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeRodsBendIntoASlab
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);

                        rebarIdCollection.Add(columnMainRebarUpperRightСorner.Id);

                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeRodsBendIntoASlab
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }

                        if (numberOfBarsLRFaces % 2 == 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeRodsBendIntoASlab
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }
                        #endregion

 #region Стержни по левой и правой граням
                        if (numberOfBarsLRFaces >= 3)
                        {
                            //Точки для построения кривфх основных угловых стержней
                            XYZ mainRebarTypeTwoShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                            XYZ mainRebarTypeTwoShort_p2 = new XYZ(Math.Round(mainRebarTypeTwoShort_p1.X, 6), Math.Round(mainRebarTypeTwoShort_p1.Y, 6), Math.Round(mainRebarTypeTwoShort_p1.Z + deepeningBarsSize + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                            XYZ mainRebarTypeTwoShort_p3 = new XYZ(Math.Round(mainRebarTypeTwoShort_p2.X + rebarOutletsLengthShort - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(mainRebarTypeTwoShort_p2.Y, 6), Math.Round(mainRebarTypeTwoShort_p2.Z, 6));

                            //Кривые стержня один короткого
                            List<Curve> myMainRebarTypeTwoCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeTwoShort_line1 = Line.CreateBound(mainRebarTypeTwoShort_p1, mainRebarTypeTwoShort_p2) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line1);
                            Curve myMainRebarTypeTwoShort_line2 = Line.CreateBound(mainRebarTypeTwoShort_p2, mainRebarTypeTwoShort_p3) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line2);

                            //Левая грань короткие
                            Rebar columnMainRebarLeftFaceLong = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Расчеты для размещения стержней
                            int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                            double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                            stepBarsLRFacesForStirrup = stepBarsLRFaces;
                            double residueForOffsetLR = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                            residueForOffsetForStirrupLR = residueForOffsetLR;

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLeftFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarLeftFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            rebarIdCollection.Add(columnMainRebarLeftFaceLong.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Левая грань длинные
                                Rebar columnMainRebarLeftFaceShort = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeRodsBendIntoASlab
                                , myMainRebarTypeTwo
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeTwoCurvesShort
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarLeftFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarLeftFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces * 2 + residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);

                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                                rebarIdCollection.Add(columnMainRebarLeftFaceShort.Id);
                            }

                            //Правая грань короткий
                            Rebar columnMainRebarRightFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                            
                            XYZ newPlaсeColumnMainRebarRightFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            rebarIdCollection.Add(columnMainRebarRightFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Правая грань длинный
                                Rebar columnMainRebarRightFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeRodsBendIntoASlab
                                    , myMainRebarTypeTwo
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalMain
                                    , myMainRebarTypeTwoCurvesShort
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                
                                XYZ newPlaсeColumnMainRebarRightFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces * 2 - residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                                rebarIdCollection.Add(columnMainRebarRightFaceLong.Id);
                            }
                        }
#endregion

#region Стержни по нижней и верхней граням
                        if (numberOfBarsTBFaces >= 3)
                        {
                            //Точки для построения кривфх основных угловых стержней
                            XYZ mainRebarTypeThreeShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                            XYZ mainRebarTypeThreeShort_p2 = new XYZ(Math.Round(mainRebarTypeThreeShort_p1.X, 6), Math.Round(mainRebarTypeThreeShort_p1.Y, 6), Math.Round(mainRebarTypeThreeShort_p1.Z + deepeningBarsSize + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                            XYZ mainRebarTypeThreeShort_p3 = new XYZ(Math.Round(mainRebarTypeThreeShort_p2.X , 6), Math.Round(mainRebarTypeThreeShort_p2.Y + rebarOutletsLengthShort - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(mainRebarTypeThreeShort_p2.Z, 6));

                            //Кривые стержня один короткого
                            List<Curve> myMainRebarTypeThreeCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeThreeShort_line1 = Line.CreateBound(mainRebarTypeThreeShort_p1, mainRebarTypeThreeShort_p2) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line1);
                            Curve myMainRebarTypeThreeShort_line2 = Line.CreateBound(mainRebarTypeThreeShort_p2, mainRebarTypeThreeShort_p3) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line2);

                            //Нижняя грань короткие
                            Rebar columnMainRebarBottomFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Cтержни нижняя и верхняя грани
                            int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                            double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                            stepBarsTBFacesForStirrup = stepBarsTBFaces;
                            double residueForOffsetTB = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;
                            residueForOffsetForStirrupTB = residueForOffsetTB;

                            ElementTransformUtils.RotateElement(doc, columnMainRebarBottomFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarBottomFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffsetTB
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            rebarIdCollection.Add(columnMainRebarBottomFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Нижняя грань длинные
                                Rebar columnMainRebarBottomFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeRodsBendIntoASlab
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesShort
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarBottomFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarBottomFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces * 2 + residueForOffsetTB
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                                rebarIdCollection.Add(columnMainRebarBottomFaceLong.Id);
                            }

                            //Верхняя грань короткие
                            Rebar columnMainRebarTopFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            
                            XYZ newPlaсeСolumnMainRebarTopFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffsetTB
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            rebarIdCollection.Add(columnMainRebarTopFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Верхняя грань длинные
                                Rebar columnMainRebarTopFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeRodsBendIntoASlab
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesShort
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                
                                XYZ newPlaсeСolumnMainRebarTopFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces * 2 - residueForOffsetTB
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                                rebarIdCollection.Add(columnMainRebarTopFaceLong.Id);
                            }
                        }

                        #endregion
                    }
                    //Если стыковка стержней в нахлест с изменением сечения колонны выше
                    else if (checkedRebarOutletsButtonName == "radioButton_MainOverlappingRods" & changeColumnSection == true & sectionOffset <= 50 / 304.8)
                    {
#region Угловые стержни
                        //Точки для построения кривых стержня один длинного
                        XYZ mainRebarTypeOneLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                        , Math.Round(columnOrigin.Y, 6)
                        , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ mainRebarTypeOneLong_p2 = new XYZ(Math.Round(mainRebarTypeOneLong_p1.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Z + deepeningBarsSize + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ mainRebarTypeOneLong_p3 = new XYZ(Math.Round(mainRebarTypeOneLong_p2.X + mainRebarDiamTypeOne + sectionOffset, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ mainRebarTypeOneLong_p4 = new XYZ(Math.Round(mainRebarTypeOneLong_p3.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Z + rebarOutletsLengthLong, 6));

                        //Точки для построения кривых стержня один короткого
                        XYZ mainRebarTypeOneShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                            , Math.Round(columnOrigin.Y, 6)
                            , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                        XYZ mainRebarTypeOneShort_p2 = new XYZ(Math.Round(mainRebarTypeOneShort_p1.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Z + deepeningBarsSize + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ mainRebarTypeOneShort_p3 = new XYZ(Math.Round(mainRebarTypeOneShort_p2.X + mainRebarDiamTypeOne + sectionOffset, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ mainRebarTypeOneShort_p4 = new XYZ(Math.Round(mainRebarTypeOneShort_p3.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Z + rebarOutletsLengthShort, 6));

                        //Кривые стержня один длинного
                        List<Curve> myMainRebarTypeOneCurvesLong = new List<Curve>();

                        Curve myMainRebarTypeOneLong_line1 = Line.CreateBound(mainRebarTypeOneLong_p1, mainRebarTypeOneLong_p2) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line1);
                        Curve myMainRebarTypeOneLong_line2 = Line.CreateBound(mainRebarTypeOneLong_p2, mainRebarTypeOneLong_p3) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line2);
                        Curve myMainRebarTypeOneLong_line3 = Line.CreateBound(mainRebarTypeOneLong_p3, mainRebarTypeOneLong_p4) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line3);

                        //Кривые стержня один короткого
                        List<Curve> myMainRebarTypeOneCurvesShort = new List<Curve>();

                        Curve myMainRebarTypeOneShort_line1 = Line.CreateBound(mainRebarTypeOneShort_p1, mainRebarTypeOneShort_p2) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line1);
                        Curve myMainRebarTypeOneShort_line2 = Line.CreateBound(mainRebarTypeOneShort_p2, mainRebarTypeOneShort_p3) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line2);
                        Curve myMainRebarTypeOneShort_line3 = Line.CreateBound(mainRebarTypeOneShort_p3, mainRebarTypeOneShort_p4) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line3);

                        //Нижний левый угол
                        Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);

                        rebarIdCollection.Add(columnMainRebarLowerLeftСorner.Id);

                        //Верхний левый угол
                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }
                        else if (numberOfBarsLRFaces % 2 == 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }

                        //Верхний правый угол
                        Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ rotate_p1 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z);
                        XYZ rotate_p2 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebarUpperRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);

                        rebarIdCollection.Add(columnMainRebarUpperRightСorner.Id);

                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }

                        if (numberOfBarsLRFaces % 2 == 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesShort
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }
#endregion

#region Стержни по левой и правой граням
                        if (numberOfBarsLRFaces >= 3)
                        {
                            //Точки для построения кривфх стержня два удлиненного
                            XYZ mainRebarTypeTwoLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                            XYZ mainRebarTypeTwoLong_p2 = new XYZ(Math.Round(mainRebarTypeTwoLong_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Z + deepeningBarsSize + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                            XYZ mainRebarTypeTwoLong_p3 = new XYZ(Math.Round(mainRebarTypeTwoLong_p2.X + mainRebarDiamTypeTwo + sectionOffset, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                            XYZ mainRebarTypeTwoLong_p4 = new XYZ(Math.Round(mainRebarTypeTwoLong_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня два укороченного
                            XYZ mainRebarTypeTwoShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                            XYZ mainRebarTypeTwoShort_p2 = new XYZ(Math.Round(mainRebarTypeTwoShort_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoShort_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p1.Z + deepeningBarsSize + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                            XYZ mainRebarTypeTwoShort_p3 = new XYZ(Math.Round(mainRebarTypeTwoShort_p2.X + mainRebarDiamTypeTwo + sectionOffset, 6)
                                , Math.Round(mainRebarTypeTwoShort_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                            XYZ mainRebarTypeTwoShort_p4 = new XYZ(Math.Round(mainRebarTypeTwoShort_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoShort_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p3.Z + rebarOutletsLengthShort, 6));

                            //Кривые стержня удлиненного
                            List<Curve> myMainRebarTypeTwoCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeTwoLong_line1 = Line.CreateBound(mainRebarTypeTwoLong_p1, mainRebarTypeTwoLong_p2) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line1);
                            Curve myMainRebarTypeTwoLong_line2 = Line.CreateBound(mainRebarTypeTwoLong_p2, mainRebarTypeTwoLong_p3) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line2);
                            Curve myMainRebarTypeTwoLong_line3 = Line.CreateBound(mainRebarTypeTwoLong_p3, mainRebarTypeTwoLong_p4) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line3);

                            //Кривые стержня укороченного
                            List<Curve> myMainRebarTypeTwoCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeTwoShort_line1 = Line.CreateBound(mainRebarTypeTwoShort_p1, mainRebarTypeTwoShort_p2) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line1);
                            Curve myMainRebarTypeTwoShort_line2 = Line.CreateBound(mainRebarTypeTwoShort_p2, mainRebarTypeTwoShort_p3) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line2);
                            Curve myMainRebarTypeTwoShort_line3 = Line.CreateBound(mainRebarTypeTwoShort_p3, mainRebarTypeTwoShort_p4) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line3);

                            //Левая грань короткие
                            Rebar columnMainRebarLeftFaceLong = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Расчеты для размещения стержней
                            int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                            double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                            stepBarsLRFacesForStirrup = stepBarsLRFaces;
                            double residueForOffsetLR = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                            residueForOffsetForStirrupLR = residueForOffsetLR;

                            XYZ newPlaсeСolumnMainRebarLeftFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            rebarIdCollection.Add(columnMainRebarLeftFaceLong.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Левая грань длинные
                                Rebar columnMainRebarLeftFaceShort = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeTwo
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeTwoCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarLeftFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces * 2 + residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);

                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                                rebarIdCollection.Add(columnMainRebarLeftFaceShort.Id);
                            }

                            //Правая грань короткий
                            Rebar columnMainRebarRightFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarRightFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            rebarIdCollection.Add(columnMainRebarRightFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Правая грань длинный
                                Rebar columnMainRebarRightFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeTwo
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalMain
                                    , myMainRebarTypeTwoCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeColumnMainRebarRightFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces * 2 - residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                                rebarIdCollection.Add(columnMainRebarRightFaceLong.Id);
                            }
                        }
#endregion

#region Стержни по нижней и верхней граням
                        if (numberOfBarsTBFaces >= 3)
                        {
                            //Точки для построения кривых стержня три длинного
                            XYZ mainRebarTypeThreeLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                            XYZ mainRebarTypeThreeLong_p2 = new XYZ(Math.Round(mainRebarTypeThreeLong_p1.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeThreeLong_p1.Z + deepeningBarsSize + columnLength, 6));
                            XYZ mainRebarTypeThreeLong_p3 = new XYZ(Math.Round(mainRebarTypeThreeLong_p2.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p2.Y + mainRebarDiamTypeThree, 6)
                                , Math.Round(mainRebarTypeThreeLong_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeThreeLong_p4 = new XYZ(Math.Round(mainRebarTypeThreeLong_p3.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeThreeLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня три короткого
                            XYZ mainRebarTypeThreeShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z - deepeningBarsSize, 6));
                            XYZ mainRebarTypeThreeShort_p2 = new XYZ(Math.Round(mainRebarTypeThreeShort_p1.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p1.Y, 6)
                                , Math.Round(mainRebarTypeThreeShort_p1.Z + deepeningBarsSize + columnLength, 6));
                            XYZ mainRebarTypeThreeShort_p3 = new XYZ(Math.Round(mainRebarTypeThreeShort_p2.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p2.Y + mainRebarDiamTypeThree, 6)
                                , Math.Round(mainRebarTypeThreeShort_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeThreeShort_p4 = new XYZ(Math.Round(mainRebarTypeThreeShort_p3.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p3.Y, 6)
                                , Math.Round(mainRebarTypeThreeShort_p3.Z + rebarOutletsLengthShort, 6));

                            //Кривые стержня длинного
                            List<Curve> myMainRebarTypeThreeCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeThreeLong_line1 = Line.CreateBound(mainRebarTypeThreeLong_p1, mainRebarTypeThreeLong_p2) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line1);
                            Curve myMainRebarTypeThreeLong_line2 = Line.CreateBound(mainRebarTypeThreeLong_p2, mainRebarTypeThreeLong_p3) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line2);
                            Curve myMainRebarTypeThreeLong_line3 = Line.CreateBound(mainRebarTypeThreeLong_p3, mainRebarTypeThreeLong_p4) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line3);

                            //Кривые стержня короткого
                            List<Curve> myMainRebarTypeThreeCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeThreeShort_line1 = Line.CreateBound(mainRebarTypeThreeShort_p1, mainRebarTypeThreeShort_p2) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line1);
                            Curve myMainRebarTypeThreeShort_line2 = Line.CreateBound(mainRebarTypeThreeShort_p2, mainRebarTypeThreeShort_p3) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line2);
                            Curve myMainRebarTypeThreeShort_line3 = Line.CreateBound(mainRebarTypeThreeShort_p3, mainRebarTypeThreeShort_p4) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line3);

                            //Нижняя грань короткие
                            Rebar columnMainRebarBottomFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Cтержни нижняя и верхняя грани
                            int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                            double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                            stepBarsTBFacesForStirrup = stepBarsTBFaces;
                            double residueForOffsetTB = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;
                            residueForOffsetForStirrupTB = residueForOffsetTB;

                            XYZ newPlaсeСolumnMainRebarBottomFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffsetTB
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            rebarIdCollection.Add(columnMainRebarBottomFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Нижняя грань длинные
                                Rebar columnMainRebarBottomFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarBottomFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces * 2 + residueForOffsetTB
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                                rebarIdCollection.Add(columnMainRebarBottomFaceLong.Id);
                            }

                            //Верхняя грань короткие
                            Rebar columnMainRebarTopFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarTopFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffsetTB
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            rebarIdCollection.Add(columnMainRebarTopFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Верхняя грань длинные
                                Rebar columnMainRebarTopFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarTopFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces * 2 - residueForOffsetTB
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                                rebarIdCollection.Add(columnMainRebarTopFaceLong.Id);
                            }
                        }

#endregion
                    }
                    //Если стыковка стержней на сварке без изменения сечения колонны выше
                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & transitionToOverlap == false & changeColumnSection == false & bendIntoASlab == false)
                    {
#region Угловые стержни
                        //Точки для построения кривых стержня один длинного
                        XYZ mainRebarTypeOneLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                        XYZ mainRebarTypeOneLong_p2 = new XYZ(Math.Round(mainRebarTypeOneLong_p1.X, 6), Math.Round(mainRebarTypeOneLong_p1.Y, 6), Math.Round(mainRebarTypeOneLong_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                        // Точки для построения кривых стержня один короткого
                        XYZ mainRebarTypeOneShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                        XYZ mainRebarTypeOneShort_p2 = new XYZ(Math.Round(mainRebarTypeOneShort_p1.X, 6), Math.Round(mainRebarTypeOneShort_p1.Y, 6), Math.Round(mainRebarTypeOneShort_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                        //Точки для установки ванночки
                        XYZ longTubWeldingOne_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthLong + baseLevelOffset);
                        XYZ shortTubWeldingOne_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthShort + baseLevelOffset);

                        //Кривые стержня один длинного
                        List<Curve> myMainRebarTypeOneCurvesLong = new List<Curve>();
                        Curve myMainRebarTypeOneLong_line1 = Line.CreateBound(mainRebarTypeOneLong_p1, mainRebarTypeOneLong_p2) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line1);
                       
                        //Кривые стержня один короткого
                        List<Curve> myMainRebarTypeOneCurvesShort = new List<Curve>();
                        Curve myMainRebarTypeOneShort_line1 = Line.CreateBound(mainRebarTypeOneShort_p1, mainRebarTypeOneShort_p2) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line1);

                        //Нижний левый угол
                        Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeWeldingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);
                        rebarIdCollection.Add(columnMainRebarLowerLeftСorner.Id);

                        FamilyInstance tubWeldingLowerLeftСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWeldingLowerLeftСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                        ElementTransformUtils.MoveElement(doc, tubWeldingLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);
                        rebarIdCollection.Add(tubWeldingLowerLeftСorner.Id);

                        //Верхний левый угол
                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);

                            FamilyInstance tubWeldingUpperLeftСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingUpperLeftСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                            rebarIdCollection.Add(tubWeldingUpperLeftСorner.Id);
                        }
                        else if (numberOfBarsLRFaces % 2 == 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);

                            FamilyInstance tubWeldingUpperLeftСorner = doc.Create.NewFamilyInstance(shortTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingUpperLeftСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                            rebarIdCollection.Add(tubWeldingUpperLeftСorner.Id);
                        }

                        //Верхний правый угол
                        Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeWeldingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ rotate_p1 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z);
                        XYZ rotate_p2 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebarUpperRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);
                        rebarIdCollection.Add(columnMainRebarUpperRightСorner.Id);

                        FamilyInstance tubWeldingUpperRightСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWeldingUpperRightСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                        ElementTransformUtils.MoveElement(doc, tubWeldingUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);
                        rebarIdCollection.Add(tubWeldingUpperRightСorner.Id);

                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeWeldingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);

                            FamilyInstance tubWeldingLowerRightСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingLowerRightСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                            rebarIdCollection.Add(tubWeldingLowerRightСorner.Id);
                        }

                        if (numberOfBarsLRFaces % 2 == 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeWeldingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesShort
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);

                            FamilyInstance tubWeldingLowerRightСorner = doc.Create.NewFamilyInstance(shortTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingLowerRightСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                            rebarIdCollection.Add(tubWeldingLowerRightСorner.Id);
                        }
#endregion

#region Стержни по левой и правой граням
                        if (numberOfBarsLRFaces >= 3)
                        {
                            //Точки для построения кривых стержня один длинного
                            XYZ mainRebarTypeTwoLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeTwoLong_p2 = new XYZ(Math.Round(mainRebarTypeTwoLong_p1.X, 6), Math.Round(mainRebarTypeTwoLong_p1.Y, 6), Math.Round(mainRebarTypeTwoLong_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                            // Точки для построения кривых стержня один короткого
                            XYZ mainRebarTypeTwoShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeTwoShort_p2 = new XYZ(Math.Round(mainRebarTypeTwoShort_p1.X, 6), Math.Round(mainRebarTypeTwoShort_p1.Y, 6), Math.Round(mainRebarTypeTwoShort_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                            //Точки для установки ванночки
                            XYZ longTubWeldingTwo_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthLong + baseLevelOffset);
                            XYZ shortTubWeldingTwo_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthShort + baseLevelOffset);

                            //Кривые стержня один длинного
                            List<Curve> myMainRebarTypeTwoCurvesLong = new List<Curve>();
                            Curve myMainRebarTypeTwoLong_line1 = Line.CreateBound(mainRebarTypeTwoLong_p1, mainRebarTypeTwoLong_p2) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line1);

                            //Кривые стержня один короткого
                            List<Curve> myMainRebarTypeTwoCurvesShort = new List<Curve>();
                            Curve myMainRebarTypeTwoShort_line1 = Line.CreateBound(mainRebarTypeTwoShort_p1, mainRebarTypeTwoShort_p2) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line1);

                            //Левая грань короткие
                            Rebar columnMainRebarLeftFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Расчеты для размещения стержней
                            int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                            double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                            stepBarsLRFacesForStirrup = stepBarsLRFaces;
                            double residueForOffsetLR = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                            residueForOffsetForStirrupLR = residueForOffsetLR;

                            XYZ newPlaсeСolumnMainRebarLeftFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);
                            columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                FamilyInstance tubWeldingLeftFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingLeftFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);
                                rebarIdCollection.Add(tubWeldingLeftFaceShort.Id);

                                for (int i =1; i< (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingLeftFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceShort.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                    Element newTubWeldingLeftFaceShort = doc.GetElement(newTubWeldingLeftFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingLeftFaceShort.Id);
                                }
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingLeftFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingLeftFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);
                                rebarIdCollection.Add(tubWeldingLeftFaceShort.Id);

                                for (int i = 1; i <= (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingLeftFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceShort.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                    Element newTubWeldingLeftFaceShort = doc.GetElement(newTubWeldingLeftFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingLeftFaceShort.Id);
                                }
                            }
                            if (columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarLeftFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Левая грань длинные
                                Rebar columnMainRebarLeftFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeWeldingRods
                                , myMainRebarTypeTwo
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeTwoCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarLeftFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces * 2 + residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);

                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                    FamilyInstance tubWeldingLeftFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingLeftFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);
                                    rebarIdCollection.Add(tubWeldingLeftFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceLong = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingLeftFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceLong.Id, pointTubWeldingInstallationLeftFaceLong) as List<ElementId>;
                                        Element newTubWeldingLeftFaceLong = doc.GetElement(newTubWeldingLeftFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingLeftFaceLong.Id);
                                    }

                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));

                                    FamilyInstance tubWeldingLeftFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingLeftFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);
                                    rebarIdCollection.Add(tubWeldingLeftFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceLong = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingLeftFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceLong.Id, pointTubWeldingInstallationLeftFaceLong) as List<ElementId>;
                                        Element newTubWeldingLeftFaceLong = doc.GetElement(newTubWeldingLeftFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingLeftFaceLong.Id);
                                    }
                                }
                                if (columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarLeftFaceLong.Id);
                            }

                            //Правая грань короткий
                            Rebar columnMainRebarRightFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarRightFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                FamilyInstance tubWeldingRightFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingRightFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                                rebarIdCollection.Add(tubWeldingRightFaceShort.Id);

                                for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationRightFaceShort = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingRightFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceShort.Id, pointTubWeldingInstallationRightFaceShort) as List<ElementId>;
                                    Element newTubWeldingRightFaceShort = doc.GetElement(newTubWeldingRightFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingRightFaceShort.Id);
                                }
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingRightFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingRightFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                                rebarIdCollection.Add(tubWeldingRightFaceShort.Id);

                                for (int i = 1; i <= (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationRightFaceShort = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingRightFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceShort.Id, pointTubWeldingInstallationRightFaceShort) as List<ElementId>;
                                    Element newTubWeldingRightFaceShort = doc.GetElement(newTubWeldingRightFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingRightFaceShort.Id);
                                }
                            }
                            if (columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarRightFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Правая грань длинный
                                Rebar columnMainRebarRightFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeWeldingRods
                                    , myMainRebarTypeTwo
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalMain
                                    , myMainRebarTypeTwoCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeColumnMainRebarRightFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces * 2 - residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                    FamilyInstance tubWeldingRightFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingRightFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                    rebarIdCollection.Add(tubWeldingRightFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationRightFaceLong = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingRightFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceLong.Id, pointTubWeldingInstallationRightFaceLong) as List<ElementId>;
                                        Element newTubWeldingRightFaceLong = doc.GetElement(newTubWeldingRightFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingRightFaceLong.Id);
                                    }
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));

                                    FamilyInstance tubWeldingRightFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingRightFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                    rebarIdCollection.Add(tubWeldingRightFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationRightFaceLong = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingRightFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceLong.Id, pointTubWeldingInstallationRightFaceLong) as List<ElementId>;
                                        Element newTubWeldingRightFaceLong = doc.GetElement(newTubWeldingRightFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingRightFaceLong.Id);
                                    }
                                }
                                if (columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarRightFaceLong.Id);
                            }
                        }
#endregion

#region Стержни по нижней и верхней граням
                        if (numberOfBarsTBFaces >= 3)
                        {
                            //Точки для построения кривых стержня один длинного
                            XYZ mainRebarTypeThreeLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeThreeLong_p2 = new XYZ(Math.Round(mainRebarTypeThreeLong_p1.X, 6), Math.Round(mainRebarTypeThreeLong_p1.Y, 6), Math.Round(mainRebarTypeThreeLong_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                            // Точки для построения кривых стержня один короткого
                            XYZ mainRebarTypeThreeShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeThreeShort_p2 = new XYZ(Math.Round(mainRebarTypeThreeShort_p1.X, 6), Math.Round(mainRebarTypeThreeShort_p1.Y, 6), Math.Round(mainRebarTypeThreeShort_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                            //Точки для установки ванночки
                            XYZ longTubWeldingThree_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthLong + baseLevelOffset);
                            XYZ shortTubWeldingThree_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthShort + baseLevelOffset);

                            //Кривые стержня один длинного
                            List<Curve> myMainRebarTypeThreeCurvesLong = new List<Curve>();
                            Curve myMainRebarTypeThreeLong_line1 = Line.CreateBound(mainRebarTypeThreeLong_p1, mainRebarTypeThreeLong_p2) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line1);

                            //Кривые стержня один короткого
                            List<Curve> myMainRebarTypeThreeCurvesShort = new List<Curve>();
                            Curve myMainRebarTypeThreeShort_line1 = Line.CreateBound(mainRebarTypeThreeShort_p1, mainRebarTypeThreeShort_p2) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line1);

                            //Нижняя грань короткие
                            Rebar columnMainRebarBottomFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Cтержни нижняя и верхняя грани
                            int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                            double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                            stepBarsTBFacesForStirrup = stepBarsTBFaces;
                            double residueForOffsetTB = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;
                            residueForOffsetForStirrupTB = residueForOffsetTB;

                            XYZ newPlaсeСolumnMainRebarBottomFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffsetTB
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                FamilyInstance tubWeldingBottomFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingBottomFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                                rebarIdCollection.Add(tubWeldingBottomFaceShort.Id);

                                for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationBottomFaceShort = new XYZ((stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingBottomFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceShort.Id, pointTubWeldingInstallationBottomFaceShort) as List<ElementId>;
                                    Element newTubWeldingBottomFaceShort = doc.GetElement(newTubWeldingBottomFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingBottomFaceShort.Id);
                                }
                            }

                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingBottomFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingBottomFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                                rebarIdCollection.Add(tubWeldingBottomFaceShort.Id);

                                for (int i = 1; i <= (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationBottomFaceShort = new XYZ((stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingBottomFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceShort.Id, pointTubWeldingInstallationBottomFaceShort) as List<ElementId>;
                                    Element newTubWeldingBottomFaceShort = doc.GetElement(newTubWeldingBottomFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingBottomFaceShort.Id);
                                }
                            }
                            if (columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarBottomFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Нижняя грань длинные
                                Rebar columnMainRebarBottomFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeWeldingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarBottomFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces * 2 + residueForOffsetTB
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                    FamilyInstance tubWeldingBottomFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingBottomFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                    rebarIdCollection.Add(tubWeldingBottomFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationBottomFaceLong = new XYZ((stepBarsTBFaces * 2) * i, 0, 0);
                                        List<ElementId> newTubWeldingBottomFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceLong.Id, pointTubWeldingInstallationBottomFaceLong) as List<ElementId>;
                                        Element newTubWeldingBottomFaceLong = doc.GetElement(newTubWeldingBottomFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingBottomFaceLong.Id);
                                    }
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));

                                    FamilyInstance tubWeldingBottomFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingBottomFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                    rebarIdCollection.Add(tubWeldingBottomFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationBottomFaceLong = new XYZ((stepBarsTBFaces * 2) * i,0 , 0);
                                        List<ElementId> newTubWeldingBottomFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceLong.Id, pointTubWeldingInstallationBottomFaceLong) as List<ElementId>;
                                        Element newTubWeldingBottomFaceLong = doc.GetElement(newTubWeldingBottomFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingBottomFaceLong.Id);
                                    }
                                }
                                if (columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarBottomFaceLong.Id);
                            }

                            //Верхняя грань короткие
                            Rebar columnMainRebarTopFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarTopFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffsetTB
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                FamilyInstance tubWeldingTopFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingTopFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                                rebarIdCollection.Add(tubWeldingTopFaceShort.Id);

                                for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationTopFaceShort = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingTopFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceShort.Id, pointTubWeldingInstallationTopFaceShort) as List<ElementId>;
                                    Element newTubWeldingTopFaceShort = doc.GetElement(newTubWeldingTopFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingTopFaceShort.Id);
                                }
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingTopFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingTopFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                                rebarIdCollection.Add(tubWeldingTopFaceShort.Id);

                                for (int i = 1; i <= (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationTopFaceShort = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingTopFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceShort.Id, pointTubWeldingInstallationTopFaceShort) as List<ElementId>;
                                    Element newTubWeldingTopFaceShort = doc.GetElement(newTubWeldingTopFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingTopFaceShort.Id);
                                }
                            }
                            if (columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarTopFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Верхняя грань длинные
                                Rebar columnMainRebarTopFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeWeldingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarTopFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces * 2 - residueForOffsetTB
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                    FamilyInstance tubWeldingTopFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingTopFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                    rebarIdCollection.Add(tubWeldingTopFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationTopFaceLong = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                        List<ElementId> newTubWeldingTopFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceLong.Id, pointTubWeldingInstallationTopFaceLong) as List<ElementId>;
                                        Element newTubWeldingTopFaceLong = doc.GetElement(newTubWeldingTopFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingTopFaceLong.Id);
                                    }
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                    
                                    FamilyInstance tubWeldingTopFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingTopFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                    rebarIdCollection.Add(tubWeldingTopFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationTopFaceLong = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                        List<ElementId> newTubWeldingTopFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceLong.Id, pointTubWeldingInstallationTopFaceLong) as List<ElementId>;
                                        Element newTubWeldingTopFaceLong = doc.GetElement(newTubWeldingTopFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingTopFaceLong.Id);
                                    }
                                }
                                if (columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarTopFaceLong.Id);
                            }
                        }

#endregion
                    }
                    //Если стыковка стержней на сварке без изменения сечения колонны выше
                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & transitionToOverlap == false & changeColumnSection == false & bendIntoASlab == true)
                    {
#region Угловые стержни

                        //Точки для построения кривых удлиненных стержней (начало и конец удлиненные)
                        XYZ mainRebarTypeOneLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                        XYZ mainRebarTypeOneLong_p2 = new XYZ(Math.Round(mainRebarTypeOneLong_p1.X, 6), Math.Round(mainRebarTypeOneLong_p1.Y, 6), Math.Round(mainRebarTypeOneLong_p1.Z - rebarOutletsLengthLong + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                        XYZ mainRebarTypeOneLong_p3 = new XYZ(Math.Round(mainRebarTypeOneLong_p2.X + rebarOutletsLengthLong - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(mainRebarTypeOneLong_p2.Y, 6), Math.Round(mainRebarTypeOneLong_p2.Z, 6));

                        //Точки для построения кривых удлиненных стержней (начало укороченное и конец удлиненный)
                        XYZ mainRebarTypeOneShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                        XYZ mainRebarTypeOneShort_p2 = new XYZ(Math.Round(mainRebarTypeOneShort_p1.X, 6), Math.Round(mainRebarTypeOneShort_p1.Y, 6), Math.Round(mainRebarTypeOneShort_p1.Z - rebarOutletsLengthShort + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                        XYZ mainRebarTypeOneShort_p3 = new XYZ(Math.Round(mainRebarTypeOneLong_p2.X + rebarOutletsLengthLong - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(mainRebarTypeOneShort_p2.Y, 6), Math.Round(mainRebarTypeOneShort_p2.Z, 6));

                        //Кривые основных угловых стержней (начало и конец удлиненные)
                        List<Curve> myMainRebarTypeOneCurvesLong = new List<Curve>();

                        Curve firstMainLineLong1 = Line.CreateBound(mainRebarTypeOneLong_p1, mainRebarTypeOneLong_p2) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(firstMainLineLong1);
                        Curve firstMainLineLong2 = Line.CreateBound(mainRebarTypeOneLong_p2, mainRebarTypeOneLong_p3) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(firstMainLineLong2);

                        //Кривые основных угловых стержней (начало укороченное и конец удлиненный)
                        List<Curve> myMainRebarTypeOneCurvesShort = new List<Curve>();

                        Curve firstMainLineShort1 = Line.CreateBound(mainRebarTypeOneShort_p1, mainRebarTypeOneShort_p2) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(firstMainLineShort1);
                        Curve firstMainLineShort2 = Line.CreateBound(mainRebarTypeOneShort_p2, mainRebarTypeOneShort_p3) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(firstMainLineShort2);

                        //Нижний левый угол
                        Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeRodsBendIntoASlab
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ rotate_p1 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z);
                        XYZ rotate_p2 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebarLowerLeftСorner.Id, rotateLine, 180 * (Math.PI / 180));

                        XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);
                        rebarIdCollection.Add(columnMainRebarLowerLeftСorner.Id);

                        //Верхний левый угол
                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarUpperLeftСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }
                        else if (numberOfBarsLRFaces % 2 == 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarUpperLeftСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }

                        //Верхний правый угол
                        Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeRodsBendIntoASlab
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);
                        rebarIdCollection.Add(columnMainRebarUpperRightСorner.Id);

                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeRodsBendIntoASlab
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }

                        if (numberOfBarsLRFaces % 2 == 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeRodsBendIntoASlab
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesShort
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }
                        #endregion

#region Стержни по левой и правой граням
                        if (numberOfBarsLRFaces >= 3)
                        {
                            //Точки для построения кривых удлиненных стержней (начало удлиненное и конец укороченный)
                            XYZ mainRebarTypeTwoLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeTwoLong_p2 = new XYZ(Math.Round(mainRebarTypeTwoLong_p1.X, 6), Math.Round(mainRebarTypeTwoLong_p1.Y, 6), Math.Round(mainRebarTypeTwoLong_p1.Z - rebarOutletsLengthLong + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                            XYZ mainRebarTypeTwoLong_p3 = new XYZ(Math.Round(mainRebarTypeTwoLong_p2.X + rebarOutletsLengthShort - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(mainRebarTypeTwoLong_p2.Y, 6), Math.Round(mainRebarTypeTwoLong_p2.Z, 6));

                            //Точки для построения кривых удлиненных стержней (начало и конец укороченные)
                            XYZ mainRebarTypeTwoShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeTwoShort_p2 = new XYZ(Math.Round(mainRebarTypeTwoShort_p1.X, 6), Math.Round(mainRebarTypeTwoShort_p1.Y, 6), Math.Round(mainRebarTypeTwoShort_p1.Z - rebarOutletsLengthShort + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                            XYZ mainRebarTypeTwoShort_p3 = new XYZ(Math.Round(mainRebarTypeTwoLong_p2.X + rebarOutletsLengthShort - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(mainRebarTypeTwoShort_p2.Y, 6), Math.Round(mainRebarTypeTwoShort_p2.Z, 6));

                            //Кривые основных угловых стержней (начало удлиненное и конец укороченный)
                            List<Curve> myMainRebarTypeTwoCurvesLong = new List<Curve>();

                            Curve secondMainLineLong1 = Line.CreateBound(mainRebarTypeTwoLong_p1, mainRebarTypeTwoLong_p2) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(secondMainLineLong1);
                            Curve secondMainLineLong2 = Line.CreateBound(mainRebarTypeTwoLong_p2, mainRebarTypeTwoLong_p3) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(secondMainLineLong2);

                            //Кривые основных угловых стержней (начало и конец укороченные)
                            List<Curve> myMainRebarTypeTwoCurvesShort = new List<Curve>();

                            Curve secondMainLineShort1 = Line.CreateBound(mainRebarTypeTwoShort_p1, mainRebarTypeTwoShort_p2) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(secondMainLineShort1);
                            Curve secondMainLineShort2 = Line.CreateBound(mainRebarTypeTwoShort_p2, mainRebarTypeTwoShort_p3) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(secondMainLineShort2);

                            //Левая грань короткие
                            Rebar columnMainRebarLeftFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Расчеты для размещения стержней
                            int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                            double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                            stepBarsLRFacesForStirrup = stepBarsLRFaces;
                            double residueForOffsetLR = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                            residueForOffsetForStirrupLR = residueForOffsetLR;

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLeftFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarLeftFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);
                            columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            if (columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarLeftFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Левая грань длинные
                                Rebar columnMainRebarLeftFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeRodsBendIntoASlab
                                , myMainRebarTypeTwo
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeTwoCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarLeftFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarLeftFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces * 2 + residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);

                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                
                                }
                                if (columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarLeftFaceLong.Id);
                            }

                            //Правая грань короткий
                            Rebar columnMainRebarRightFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeColumnMainRebarRightFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            if (columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarRightFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Правая грань длинный
                                Rebar columnMainRebarRightFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeRodsBendIntoASlab
                                    , myMainRebarTypeTwo
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalMain
                                    , myMainRebarTypeTwoCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                XYZ newPlaсeColumnMainRebarRightFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces * 2 - residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                if (columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarRightFaceLong.Id);
                            }
                        }
#endregion

#region Стержни по нижней и верхней граням
                        if (numberOfBarsTBFaces >= 3)
                        {
                            //Точки для построения кривых удлиненных стержней (начало удлиненное и конец укороченный)
                            XYZ mainRebarTypeThreeLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeThreeLong_p2 = new XYZ(Math.Round(mainRebarTypeThreeLong_p1.X, 6), Math.Round(mainRebarTypeThreeLong_p1.Y, 6), Math.Round(mainRebarTypeThreeLong_p1.Z - rebarOutletsLengthLong + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                            XYZ mainRebarTypeThreeLong_p3 = new XYZ(Math.Round(mainRebarTypeThreeLong_p2.X , 6), Math.Round(mainRebarTypeThreeLong_p2.Y + rebarOutletsLengthShort - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(mainRebarTypeThreeLong_p2.Z, 6));

                            //Точки для построения кривых удлиненных стержней (начало и конец укороченные)
                            XYZ mainRebarTypeThreeShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeThreeShort_p2 = new XYZ(Math.Round(mainRebarTypeThreeShort_p1.X, 6), Math.Round(mainRebarTypeThreeShort_p1.Y, 6), Math.Round(mainRebarTypeThreeShort_p1.Z - rebarOutletsLengthShort + columnLength + floorThicknessAboveColumn - 60 / 304.8, 6));
                            XYZ mainRebarTypeThreeShort_p3 = new XYZ(Math.Round(mainRebarTypeThreeLong_p2.X , 6), Math.Round(mainRebarTypeThreeShort_p2.Y + rebarOutletsLengthShort - (floorThicknessAboveColumn - 60 / 304.8), 6), Math.Round(mainRebarTypeThreeShort_p2.Z, 6));

                            //Кривые основных угловых стержней (начало удлиненное и конец укороченный)
                            List<Curve> myMainRebarTypeThreeCurvesLong = new List<Curve>();

                            Curve secondMainLineLong1 = Line.CreateBound(mainRebarTypeThreeLong_p1, mainRebarTypeThreeLong_p2) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(secondMainLineLong1);
                            Curve secondMainLineLong2 = Line.CreateBound(mainRebarTypeThreeLong_p2, mainRebarTypeThreeLong_p3) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(secondMainLineLong2);

                            //Кривые основных угловых стержней (начало и конец укороченные)
                            List<Curve> myMainRebarTypeThreeCurvesShort = new List<Curve>();

                            Curve secondMainLineShort1 = Line.CreateBound(mainRebarTypeThreeShort_p1, mainRebarTypeThreeShort_p2) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(secondMainLineShort1);
                            Curve secondMainLineShort2 = Line.CreateBound(mainRebarTypeThreeShort_p2, mainRebarTypeThreeShort_p3) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(secondMainLineShort2);

                            //Нижняя грань короткие
                            Rebar columnMainRebarBottomFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Cтержни нижняя и верхняя грани
                            int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                            double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                            stepBarsTBFacesForStirrup = stepBarsTBFaces;
                            double residueForOffsetTB = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;
                            residueForOffsetForStirrupTB = residueForOffsetTB;

                            ElementTransformUtils.RotateElement(doc, columnMainRebarBottomFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarBottomFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffsetTB
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }

                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            if (columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarBottomFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Нижняя грань длинные
                                Rebar columnMainRebarBottomFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeRodsBendIntoASlab
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarBottomFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarBottomFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces * 2 + residueForOffsetTB
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                if (columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarBottomFaceLong.Id);
                            }

                            //Верхняя грань короткие
                            Rebar columnMainRebarTopFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeRodsBendIntoASlab
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarTopFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffsetTB
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            if (columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarTopFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Верхняя грань длинные
                                Rebar columnMainRebarTopFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeRodsBendIntoASlab
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarTopFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces * 2 - residueForOffsetTB
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                if (columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarTopFaceLong.Id);
                            }
                        }

#endregion
                    }
                    //Если стыковка стержней на сварке с изменением сечения колонны выше
                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & transitionToOverlap == false & changeColumnSection == true & sectionOffset <= 50 / 304.8)
                    {
#region Угловые стержни
                        //Точки для построения кривых стержня один длинного
                        XYZ mainRebarTypeOneLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                        , Math.Round(columnOrigin.Y, 6)
                        , Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                        XYZ mainRebarTypeOneLong_p2 = new XYZ(Math.Round(mainRebarTypeOneLong_p1.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Z + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn) - rebarOutletsLengthLong, 6));
                        XYZ mainRebarTypeOneLong_p3 = new XYZ(Math.Round(mainRebarTypeOneLong_p2.X + sectionOffset, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ mainRebarTypeOneLong_p4 = new XYZ(Math.Round(mainRebarTypeOneLong_p3.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Z + rebarOutletsLengthLong, 6));

                        //Точки для построения кривых стержня один короткого
                        XYZ mainRebarTypeOneShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                            , Math.Round(columnOrigin.Y, 6)
                            , Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                        XYZ mainRebarTypeOneShort_p2 = new XYZ(Math.Round(mainRebarTypeOneShort_p1.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Z + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn) - rebarOutletsLengthShort, 6));
                        XYZ mainRebarTypeOneShort_p3 = new XYZ(Math.Round(mainRebarTypeOneShort_p2.X + sectionOffset, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ mainRebarTypeOneShort_p4 = new XYZ(Math.Round(mainRebarTypeOneShort_p3.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Z + rebarOutletsLengthShort, 6));

                        //Точки для установки ванночки
                        XYZ longTubWeldingOne_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthLong + baseLevelOffset);
                        XYZ shortTubWeldingOne_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthShort + baseLevelOffset);

                        //Кривые стержня один длинного
                        List<Curve> myMainRebarTypeOneCurvesLong = new List<Curve>();

                        Curve myMainRebarTypeOneLong_line1 = Line.CreateBound(mainRebarTypeOneLong_p1, mainRebarTypeOneLong_p2) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line1);
                        Curve myMainRebarTypeOneLong_line2 = Line.CreateBound(mainRebarTypeOneLong_p2, mainRebarTypeOneLong_p3) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line2);
                        Curve myMainRebarTypeOneLong_line3 = Line.CreateBound(mainRebarTypeOneLong_p3, mainRebarTypeOneLong_p4) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line3);

                        //Кривые стержня один короткого
                        List<Curve> myMainRebarTypeOneCurvesShort = new List<Curve>();

                        Curve myMainRebarTypeOneShort_line1 = Line.CreateBound(mainRebarTypeOneShort_p1, mainRebarTypeOneShort_p2) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line1);
                        Curve myMainRebarTypeOneShort_line2 = Line.CreateBound(mainRebarTypeOneShort_p2, mainRebarTypeOneShort_p3) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line2);
                        Curve myMainRebarTypeOneShort_line3 = Line.CreateBound(mainRebarTypeOneShort_p3, mainRebarTypeOneShort_p4) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line3);

                        //Нижний левый угол
                        Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);
                        rebarIdCollection.Add(columnMainRebarLowerLeftСorner.Id);

                        FamilyInstance tubWeldingLowerLeftСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWeldingLowerLeftСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                        ElementTransformUtils.MoveElement(doc, tubWeldingLowerLeftСorner.Id, new XYZ (newPlaсeСolumnMainRebarLowerLeftСorner.X + sectionOffset, newPlaсeСolumnMainRebarLowerLeftСorner.Y, newPlaсeСolumnMainRebarLowerLeftСorner.Z));
                        rebarIdCollection.Add(tubWeldingLowerLeftСorner.Id);

                        //Верхний левый угол
                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);

                            FamilyInstance tubWeldingUpperLeftСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingUpperLeftСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingUpperLeftСorner.Id, new XYZ(newPlaсeСolumnMainRebarUpperLeftСorner.X + sectionOffset, newPlaсeСolumnMainRebarUpperLeftСorner.Y, newPlaсeСolumnMainRebarUpperLeftСorner.Z));
                            rebarIdCollection.Add(tubWeldingUpperLeftСorner.Id);
                        }
                        else if (numberOfBarsLRFaces % 2 == 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);
                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);

                            FamilyInstance tubWeldingUpperLeftСorner = doc.Create.NewFamilyInstance(shortTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingUpperLeftСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingUpperLeftСorner.Id, new XYZ(newPlaсeСolumnMainRebarUpperLeftСorner.X + sectionOffset, newPlaсeСolumnMainRebarUpperLeftСorner.Y, newPlaсeСolumnMainRebarUpperLeftСorner.Z));
                            rebarIdCollection.Add(tubWeldingUpperLeftСorner.Id);
                        }

                        //Верхний правый угол
                        Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ rotate_p1 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z);
                        XYZ rotate_p2 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebarUpperRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);
                        rebarIdCollection.Add(columnMainRebarUpperRightСorner.Id);

                        FamilyInstance tubWeldingUpperRightСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                        tubWeldingUpperRightСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                        ElementTransformUtils.MoveElement(doc, tubWeldingUpperRightСorner.Id, new XYZ(newPlaсeColumnMainRebarUpperRightСorner.X - sectionOffset, newPlaсeColumnMainRebarUpperRightСorner.Y, newPlaсeColumnMainRebarUpperRightСorner.Z));
                        rebarIdCollection.Add(tubWeldingUpperRightСorner.Id);

                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);

                            FamilyInstance tubWeldingLowerRightСorner = doc.Create.NewFamilyInstance(longTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingLowerRightСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingLowerRightСorner.Id, new XYZ(newPlaсeColumnMainRebarLowerRightСorner.X - sectionOffset, newPlaсeColumnMainRebarLowerRightСorner.Y, newPlaсeColumnMainRebarLowerRightСorner.Z));
                            rebarIdCollection.Add(tubWeldingLowerRightСorner.Id);
                        }

                        if (numberOfBarsLRFaces % 2 == 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesShort
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);
                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);

                            FamilyInstance tubWeldingLowerRightСorner = doc.Create.NewFamilyInstance(shortTubWeldingOne_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                            tubWeldingLowerRightСorner.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeOne);
                            ElementTransformUtils.MoveElement(doc, tubWeldingLowerRightСorner.Id, new XYZ(newPlaсeColumnMainRebarLowerRightСorner.X - sectionOffset, newPlaсeColumnMainRebarLowerRightСorner.Y, newPlaсeColumnMainRebarLowerRightСorner.Z));
                            rebarIdCollection.Add(tubWeldingLowerRightСorner.Id);
                        }
                        #endregion

#region Стержни по левой и правой граням
                        if (numberOfBarsLRFaces >= 3)
                        {
                            //Точки для построения кривфх стержня два удлиненного
                            XYZ mainRebarTypeTwoLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeTwoLong_p2 = new XYZ(Math.Round(mainRebarTypeTwoLong_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Z + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn) - rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeTwoLong_p3 = new XYZ(Math.Round(mainRebarTypeTwoLong_p2.X + sectionOffset, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                            XYZ mainRebarTypeTwoLong_p4 = new XYZ(Math.Round(mainRebarTypeTwoLong_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня два укороченного
                            XYZ mainRebarTypeTwoShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeTwoShort_p2 = new XYZ(Math.Round(mainRebarTypeTwoShort_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoShort_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p1.Z + columnLength - (sectionOffset * 6 - floorThicknessAboveColumn) - rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeTwoShort_p3 = new XYZ(Math.Round(mainRebarTypeTwoShort_p2.X + sectionOffset, 6)
                                , Math.Round(mainRebarTypeTwoShort_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                            XYZ mainRebarTypeTwoShort_p4 = new XYZ(Math.Round(mainRebarTypeTwoShort_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoShort_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p3.Z + rebarOutletsLengthShort, 6));

                            //Точки для установки ванночки
                            XYZ longTubWeldingTwo_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthLong + baseLevelOffset);
                            XYZ shortTubWeldingTwo_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthShort + baseLevelOffset);

                            //Кривые стержня удлиненного
                            List<Curve> myMainRebarTypeTwoCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeTwoLong_line1 = Line.CreateBound(mainRebarTypeTwoLong_p1, mainRebarTypeTwoLong_p2) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line1);
                            Curve myMainRebarTypeTwoLong_line2 = Line.CreateBound(mainRebarTypeTwoLong_p2, mainRebarTypeTwoLong_p3) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line2);
                            Curve myMainRebarTypeTwoLong_line3 = Line.CreateBound(mainRebarTypeTwoLong_p3, mainRebarTypeTwoLong_p4) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line3);

                            //Кривые стержня укороченного
                            List<Curve> myMainRebarTypeTwoCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeTwoShort_line1 = Line.CreateBound(mainRebarTypeTwoShort_p1, mainRebarTypeTwoShort_p2) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line1);
                            Curve myMainRebarTypeTwoShort_line2 = Line.CreateBound(mainRebarTypeTwoShort_p2, mainRebarTypeTwoShort_p3) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line2);
                            Curve myMainRebarTypeTwoShort_line3 = Line.CreateBound(mainRebarTypeTwoShort_p3, mainRebarTypeTwoShort_p4) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line3);

                            //Левая грань короткие
                            Rebar columnMainRebarLeftFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Расчеты для размещения стержней
                            int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                            double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                            stepBarsLRFacesForStirrup = stepBarsLRFaces;
                            double residueForOffsetLR = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                            residueForOffsetForStirrupLR = residueForOffsetLR;

                            XYZ newPlaсeСolumnMainRebarLeftFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);
                            columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                FamilyInstance tubWeldingLeftFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingLeftFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceShort.Id, new XYZ(newPlaсeСolumnMainRebarLeftFaceShort.X + sectionOffset, newPlaсeСolumnMainRebarLeftFaceShort.Y, newPlaсeСolumnMainRebarLeftFaceShort.Z));
                                rebarIdCollection.Add(tubWeldingLeftFaceShort.Id);

                                for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingLeftFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceShort.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                    Element newTubWeldingLeftFaceShort = doc.GetElement(newTubWeldingLeftFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingLeftFaceShort.Id);
                                }
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingLeftFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingLeftFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceShort.Id, new XYZ(newPlaсeСolumnMainRebarLeftFaceShort.X + sectionOffset, newPlaсeСolumnMainRebarLeftFaceShort.Y, newPlaсeСolumnMainRebarLeftFaceShort.Z));
                                rebarIdCollection.Add(tubWeldingLeftFaceShort.Id);

                                for (int i = 1; i <= (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationLeftFaceShort = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingLeftFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceShort.Id, pointTubWeldingInstallationLeftFaceShort) as List<ElementId>;
                                    Element newTubWeldingLeftFaceShort = doc.GetElement(newTubWeldingLeftFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingLeftFaceShort.Id);
                                }
                            }
                            if (columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarLeftFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Левая грань длинные
                                Rebar columnMainRebarLeftFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeTwo
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeTwoCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarLeftFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces * 2 + residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);

                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                    FamilyInstance tubWeldingLeftFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingLeftFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceLong.Id, new XYZ(newPlaсeСolumnMainRebarLeftFaceLong.X + sectionOffset, newPlaсeСolumnMainRebarLeftFaceLong.Y, newPlaсeСolumnMainRebarLeftFaceLong.Z));
                                    rebarIdCollection.Add(tubWeldingLeftFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceLong = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingLeftFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceLong.Id, pointTubWeldingInstallationLeftFaceLong) as List<ElementId>;
                                        Element newTubWeldingLeftFaceLong = doc.GetElement(newTubWeldingLeftFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingLeftFaceLong.Id);
                                    }

                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));

                                    FamilyInstance tubWeldingLeftFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingLeftFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingLeftFaceLong.Id, new XYZ(newPlaсeСolumnMainRebarLeftFaceLong.X + sectionOffset, newPlaсeСolumnMainRebarLeftFaceLong.Y, newPlaсeСolumnMainRebarLeftFaceLong.Z));
                                    rebarIdCollection.Add(tubWeldingLeftFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationLeftFaceLong = new XYZ(0, (stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingLeftFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingLeftFaceLong.Id, pointTubWeldingInstallationLeftFaceLong) as List<ElementId>;
                                        Element newTubWeldingLeftFaceLong = doc.GetElement(newTubWeldingLeftFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingLeftFaceLong.Id);
                                    }
                                }
                                if (columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarLeftFaceLong.Id);
                            }

                            //Правая грань короткий
                            Rebar columnMainRebarRightFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarRightFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                FamilyInstance tubWeldingRightFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingRightFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceShort.Id, new XYZ(newPlaсeColumnMainRebarRightFaceShort.X - sectionOffset, newPlaсeColumnMainRebarRightFaceShort.Y, newPlaсeColumnMainRebarRightFaceShort.Z));
                                rebarIdCollection.Add(tubWeldingRightFaceShort.Id);

                                for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationRightFaceShort = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingRightFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceShort.Id, pointTubWeldingInstallationRightFaceShort) as List<ElementId>;
                                    Element newTubWeldingRightFaceShort = doc.GetElement(newTubWeldingRightFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingRightFaceShort.Id);
                                }
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingRightFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingRightFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceShort.Id, new XYZ(newPlaсeColumnMainRebarRightFaceShort.X - sectionOffset, newPlaсeColumnMainRebarRightFaceShort.Y, newPlaсeColumnMainRebarRightFaceShort.Z));
                                rebarIdCollection.Add(tubWeldingRightFaceShort.Id);

                                for (int i = 1; i <= (numberOfBarsLRFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationRightFaceShort = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                    List<ElementId> newTubWeldingRightFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceShort.Id, pointTubWeldingInstallationRightFaceShort) as List<ElementId>;
                                    Element newTubWeldingRightFaceShort = doc.GetElement(newTubWeldingRightFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingRightFaceShort.Id);
                                }
                            }
                            if (columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarRightFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Правая грань длинный
                                Rebar columnMainRebarRightFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeTwo
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalMain
                                    , myMainRebarTypeTwoCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeColumnMainRebarRightFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces * 2 - residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);

                                    FamilyInstance tubWeldingRightFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingRightFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceLong.Id, new XYZ(newPlaсeColumnMainRebarRightFaceLong.X - sectionOffset, newPlaсeColumnMainRebarRightFaceLong.Y, newPlaсeColumnMainRebarRightFaceLong.Z));
                                    rebarIdCollection.Add(tubWeldingRightFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationRightFaceLong = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingRightFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceLong.Id, pointTubWeldingInstallationRightFaceLong) as List<ElementId>;
                                        Element newTubWeldingRightFaceLong = doc.GetElement(newTubWeldingRightFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingRightFaceLong.Id);
                                    }
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));

                                    FamilyInstance tubWeldingRightFaceLong = doc.Create.NewFamilyInstance(longTubWeldingTwo_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingRightFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeTwo);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingRightFaceLong.Id, new XYZ(newPlaсeColumnMainRebarRightFaceLong.X - sectionOffset, newPlaсeColumnMainRebarRightFaceLong.Y, newPlaсeColumnMainRebarRightFaceLong.Z));
                                    rebarIdCollection.Add(tubWeldingRightFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsLRFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationRightFaceLong = new XYZ(0, (-stepBarsLRFaces * 2) * i, 0);
                                        List<ElementId> newTubWeldingRightFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingRightFaceLong.Id, pointTubWeldingInstallationRightFaceLong) as List<ElementId>;
                                        Element newTubWeldingRightFaceLong = doc.GetElement(newTubWeldingRightFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingRightFaceLong.Id);
                                    }
                                }
                                if (columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarRightFaceLong.Id);
                            }
                        }
#endregion

#region Стержни по нижней и верхней граням
                        if (numberOfBarsTBFaces >= 3)
                        {
                            //Точки для построения кривых стержня один длинного
                            XYZ mainRebarTypeThreeLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeThreeLong_p2 = new XYZ(Math.Round(mainRebarTypeThreeLong_p1.X, 6), Math.Round(mainRebarTypeThreeLong_p1.Y, 6), Math.Round(mainRebarTypeThreeLong_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                            // Точки для построения кривых стержня один короткого
                            XYZ mainRebarTypeThreeShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeThreeShort_p2 = new XYZ(Math.Round(mainRebarTypeThreeShort_p1.X, 6), Math.Round(mainRebarTypeThreeShort_p1.Y, 6), Math.Round(mainRebarTypeThreeShort_p1.Z + columnLength + floorThicknessAboveColumn, 6));

                            //Точки для установки ванночки
                            XYZ longTubWeldingThree_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthLong + baseLevelOffset);
                            XYZ shortTubWeldingThree_p1 = new XYZ(Math.Round(columnOrigin.X, 6), Math.Round(columnOrigin.Y, 6), columnLength + floorThicknessAboveColumn + rebarOutletsLengthShort + baseLevelOffset);

                            //Кривые стержня один длинного
                            List<Curve> myMainRebarTypeThreeCurvesLong = new List<Curve>();
                            Curve myMainRebarTypeThreeLong_line1 = Line.CreateBound(mainRebarTypeThreeLong_p1, mainRebarTypeThreeLong_p2) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line1);

                            //Кривые стержня один короткого
                            List<Curve> myMainRebarTypeThreeCurvesShort = new List<Curve>();
                            Curve myMainRebarTypeThreeShort_line1 = Line.CreateBound(mainRebarTypeThreeShort_p1, mainRebarTypeThreeShort_p2) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line1);

                            //Нижняя грань короткие
                            Rebar columnMainRebarBottomFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Cтержни нижняя и верхняя грани
                            int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                            double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                            stepBarsTBFacesForStirrup = stepBarsTBFaces;
                            double residueForOffsetTB = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;
                            residueForOffsetForStirrupTB = residueForOffsetTB;

                            XYZ newPlaсeСolumnMainRebarBottomFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffsetTB
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                FamilyInstance tubWeldingBottomFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingBottomFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                                rebarIdCollection.Add(tubWeldingBottomFaceShort.Id);

                                for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationBottomFaceShort = new XYZ((stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingBottomFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceShort.Id, pointTubWeldingInstallationBottomFaceShort) as List<ElementId>;
                                    Element newTubWeldingBottomFaceShort = doc.GetElement(newTubWeldingBottomFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingBottomFaceShort.Id);
                                }
                            }

                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingBottomFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingBottomFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                                rebarIdCollection.Add(tubWeldingBottomFaceShort.Id);

                                for (int i = 1; i <= (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationBottomFaceShort = new XYZ((stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingBottomFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceShort.Id, pointTubWeldingInstallationBottomFaceShort) as List<ElementId>;
                                    Element newTubWeldingBottomFaceShort = doc.GetElement(newTubWeldingBottomFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingBottomFaceShort.Id);
                                }
                            }
                            if (columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarBottomFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Нижняя грань длинные
                                Rebar columnMainRebarBottomFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeWeldingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarBottomFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces * 2 + residueForOffsetTB
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                    FamilyInstance tubWeldingBottomFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingBottomFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                    rebarIdCollection.Add(tubWeldingBottomFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationBottomFaceLong = new XYZ((stepBarsTBFaces * 2) * i, 0, 0);
                                        List<ElementId> newTubWeldingBottomFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceLong.Id, pointTubWeldingInstallationBottomFaceLong) as List<ElementId>;
                                        Element newTubWeldingBottomFaceLong = doc.GetElement(newTubWeldingBottomFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingBottomFaceLong.Id);
                                    }
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));

                                    FamilyInstance tubWeldingBottomFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingBottomFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                    rebarIdCollection.Add(tubWeldingBottomFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationBottomFaceLong = new XYZ((stepBarsTBFaces * 2) * i, 0, 0);
                                        List<ElementId> newTubWeldingBottomFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingBottomFaceLong.Id, pointTubWeldingInstallationBottomFaceLong) as List<ElementId>;
                                        Element newTubWeldingBottomFaceLong = doc.GetElement(newTubWeldingBottomFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingBottomFaceLong.Id);
                                    }
                                }
                                if (columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarBottomFaceLong.Id);
                            }

                            //Верхняя грань короткие
                            Rebar columnMainRebarTopFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeWeldingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarTopFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffsetTB
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                FamilyInstance tubWeldingTopFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingTopFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                                rebarIdCollection.Add(tubWeldingTopFaceShort.Id);

                                for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationTopFaceShort = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingTopFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceShort.Id, pointTubWeldingInstallationTopFaceShort) as List<ElementId>;
                                    Element newTubWeldingTopFaceShort = doc.GetElement(newTubWeldingTopFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingTopFaceShort.Id);
                                }
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);

                                FamilyInstance tubWeldingTopFaceShort = doc.Create.NewFamilyInstance(shortTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                tubWeldingTopFaceShort.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                                rebarIdCollection.Add(tubWeldingTopFaceShort.Id);

                                for (int i = 1; i <= (numberOfBarsTBFaces - 2) / 2; i++)
                                {
                                    XYZ pointTubWeldingInstallationTopFaceShort = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                    List<ElementId> newTubWeldingTopFaceShortIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceShort.Id, pointTubWeldingInstallationTopFaceShort) as List<ElementId>;
                                    Element newTubWeldingTopFaceShort = doc.GetElement(newTubWeldingTopFaceShortIdList.First());
                                    rebarIdCollection.Add(newTubWeldingTopFaceShort.Id);
                                }
                            }
                            if (columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                            }
                            rebarIdCollection.Add(columnMainRebarTopFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Верхняя грань длинные
                                Rebar columnMainRebarTopFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeWeldingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarTopFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces * 2 - residueForOffsetTB
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);

                                    FamilyInstance tubWeldingTopFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingTopFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                    rebarIdCollection.Add(tubWeldingTopFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationTopFaceLong = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                        List<ElementId> newTubWeldingTopFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceLong.Id, pointTubWeldingInstallationTopFaceLong) as List<ElementId>;
                                        Element newTubWeldingTopFaceLong = doc.GetElement(newTubWeldingTopFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingTopFaceLong.Id);
                                    }
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));

                                    FamilyInstance tubWeldingTopFaceLong = doc.Create.NewFamilyInstance(longTubWeldingThree_p1, myTubWeldingSymbol, baseLevel, StructuralType.NonStructural);
                                    tubWeldingTopFaceLong.LookupParameter("Диаметр стержня").Set(mainRebarDiamTypeThree);
                                    ElementTransformUtils.MoveElement(doc, tubWeldingTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                    rebarIdCollection.Add(tubWeldingTopFaceLong.Id);

                                    for (int i = 1; i < (numberOfBarsTBFaces - 2) / 2; i++)
                                    {
                                        XYZ pointTubWeldingInstallationTopFaceLong = new XYZ((-stepBarsTBFaces * 2) * i, 0, 0);
                                        List<ElementId> newTubWeldingTopFaceLongIdList = ElementTransformUtils.CopyElement(doc, tubWeldingTopFaceLong.Id, pointTubWeldingInstallationTopFaceLong) as List<ElementId>;
                                        Element newTubWeldingTopFaceLong = doc.GetElement(newTubWeldingTopFaceLongIdList.First());
                                        rebarIdCollection.Add(newTubWeldingTopFaceLong.Id);
                                    }
                                }
                                if (columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).IsReadOnly == false)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);
                                }
                                rebarIdCollection.Add(columnMainRebarTopFaceLong.Id);
                            }
                        }

#endregion
                    }
                    //Если переход со стыковки на сварке в нахлест без изменения сечения колонны выше
                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & transitionToOverlap == true & changeColumnSection == false)
                    {
#region Угловые стержни
                        //Точки для построения кривых стержня один длинного
                        XYZ mainRebarTypeOneLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                        , Math.Round(columnOrigin.Y, 6)
                        , Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                        XYZ mainRebarTypeOneLong_p2 = new XYZ(Math.Round(mainRebarTypeOneLong_p1.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Z + columnLength - rebarOutletsLengthLong, 6));
                        XYZ mainRebarTypeOneLong_p3 = new XYZ(Math.Round(mainRebarTypeOneLong_p2.X + mainRebarDiamTypeOne, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ mainRebarTypeOneLong_p4 = new XYZ(Math.Round(mainRebarTypeOneLong_p3.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Z + rebarOutletsLengthLong, 6));

                        //Точки для построения кривых стержня один короткого
                        XYZ mainRebarTypeOneShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                            , Math.Round(columnOrigin.Y, 6)
                            , Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                        XYZ mainRebarTypeOneShort_p2 = new XYZ(Math.Round(mainRebarTypeOneShort_p1.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Z + columnLength - rebarOutletsLengthShort, 6));
                        XYZ mainRebarTypeOneShort_p3 = new XYZ(Math.Round(mainRebarTypeOneShort_p2.X + mainRebarDiamTypeOne, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Z + floorThicknessAboveColumn, 6));
                        XYZ mainRebarTypeOneShort_p4 = new XYZ(Math.Round(mainRebarTypeOneShort_p3.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Z + rebarOutletsLengthShort, 6));

                        //Кривые стержня один длинного
                        List<Curve> myMainRebarTypeOneCurvesLong = new List<Curve>();

                        Curve myMainRebarTypeOneLong_line1 = Line.CreateBound(mainRebarTypeOneLong_p1, mainRebarTypeOneLong_p2) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line1);
                        Curve myMainRebarTypeOneLong_line2 = Line.CreateBound(mainRebarTypeOneLong_p2, mainRebarTypeOneLong_p3) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line2);
                        Curve myMainRebarTypeOneLong_line3 = Line.CreateBound(mainRebarTypeOneLong_p3, mainRebarTypeOneLong_p4) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line3);

                        //Кривые стержня один короткого
                        List<Curve> myMainRebarTypeOneCurvesShort = new List<Curve>();

                        Curve myMainRebarTypeOneShort_line1 = Line.CreateBound(mainRebarTypeOneShort_p1, mainRebarTypeOneShort_p2) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line1);
                        Curve myMainRebarTypeOneShort_line2 = Line.CreateBound(mainRebarTypeOneShort_p2, mainRebarTypeOneShort_p3) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line2);
                        Curve myMainRebarTypeOneShort_line3 = Line.CreateBound(mainRebarTypeOneShort_p3, mainRebarTypeOneShort_p4) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line3);

                        //Нижний левый угол
                        Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);

                        rebarIdCollection.Add(columnMainRebarLowerLeftСorner.Id);

                        //Верхний левый угол
                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }
                        else if (numberOfBarsLRFaces % 2 == 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }

                        //Верхний правый угол
                        Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ rotate_p1 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z);
                        XYZ rotate_p2 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebarUpperRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);

                        rebarIdCollection.Add(columnMainRebarUpperRightСorner.Id);

                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }

                        if (numberOfBarsLRFaces % 2 == 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesShort
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }
                        #endregion

#region Стержни по левой и правой граням
                        if (numberOfBarsLRFaces >= 3)
                        {
                            //Точки для построения кривфх стержня два удлиненного
                            XYZ mainRebarTypeTwoLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeTwoLong_p2 = new XYZ(Math.Round(mainRebarTypeTwoLong_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Z + columnLength - rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeTwoLong_p3 = new XYZ(Math.Round(mainRebarTypeTwoLong_p2.X + mainRebarDiamTypeTwo, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeTwoLong_p4 = new XYZ(Math.Round(mainRebarTypeTwoLong_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня два укороченного
                            XYZ mainRebarTypeTwoShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeTwoShort_p2 = new XYZ(Math.Round(mainRebarTypeTwoShort_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoShort_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p1.Z + columnLength - rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeTwoShort_p3 = new XYZ(Math.Round(mainRebarTypeTwoShort_p2.X + mainRebarDiamTypeTwo, 6)
                                , Math.Round(mainRebarTypeTwoShort_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeTwoShort_p4 = new XYZ(Math.Round(mainRebarTypeTwoShort_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoShort_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p3.Z + rebarOutletsLengthShort, 6));

                            //Кривые стержня удлиненного
                            List<Curve> myMainRebarTypeTwoCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeTwoLong_line1 = Line.CreateBound(mainRebarTypeTwoLong_p1, mainRebarTypeTwoLong_p2) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line1);
                            Curve myMainRebarTypeTwoLong_line2 = Line.CreateBound(mainRebarTypeTwoLong_p2, mainRebarTypeTwoLong_p3) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line2);
                            Curve myMainRebarTypeTwoLong_line3 = Line.CreateBound(mainRebarTypeTwoLong_p3, mainRebarTypeTwoLong_p4) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line3);

                            //Кривые стержня укороченного
                            List<Curve> myMainRebarTypeTwoCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeTwoShort_line1 = Line.CreateBound(mainRebarTypeTwoShort_p1, mainRebarTypeTwoShort_p2) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line1);
                            Curve myMainRebarTypeTwoShort_line2 = Line.CreateBound(mainRebarTypeTwoShort_p2, mainRebarTypeTwoShort_p3) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line2);
                            Curve myMainRebarTypeTwoShort_line3 = Line.CreateBound(mainRebarTypeTwoShort_p3, mainRebarTypeTwoShort_p4) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line3);

                            //Левая грань короткие
                            Rebar columnMainRebarLeftFaceLong = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Расчеты для размещения стержней
                            int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                            double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                            stepBarsLRFacesForStirrup = stepBarsLRFaces;
                            double residueForOffsetLR = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                            residueForOffsetForStirrupLR = residueForOffsetLR;

                            XYZ newPlaсeСolumnMainRebarLeftFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            rebarIdCollection.Add(columnMainRebarLeftFaceLong.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Левая грань длинные
                                Rebar columnMainRebarLeftFaceShort = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeTwo
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeTwoCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarLeftFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces * 2 + residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);

                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                                rebarIdCollection.Add(columnMainRebarLeftFaceShort.Id);
                            }

                            //Правая грань короткий
                            Rebar columnMainRebarRightFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarRightFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            rebarIdCollection.Add(columnMainRebarRightFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Правая грань длинный
                                Rebar columnMainRebarRightFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeTwo
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalMain
                                    , myMainRebarTypeTwoCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeColumnMainRebarRightFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces * 2 - residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                                rebarIdCollection.Add(columnMainRebarRightFaceLong.Id);
                            }
                        }
                        #endregion

#region Стержни по нижней и верхней граням
                        if (numberOfBarsTBFaces >= 3)
                        {
                            //Точки для построения кривых стержня три длинного
                            XYZ mainRebarTypeThreeLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeThreeLong_p2 = new XYZ(Math.Round(mainRebarTypeThreeLong_p1.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeThreeLong_p1.Z + columnLength - rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeThreeLong_p3 = new XYZ(Math.Round(mainRebarTypeThreeLong_p2.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p2.Y + mainRebarDiamTypeThree, 6)
                                , Math.Round(mainRebarTypeThreeLong_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeThreeLong_p4 = new XYZ(Math.Round(mainRebarTypeThreeLong_p3.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeThreeLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня три короткого
                            XYZ mainRebarTypeThreeShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeThreeShort_p2 = new XYZ(Math.Round(mainRebarTypeThreeShort_p1.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p1.Y, 6)
                                , Math.Round(mainRebarTypeThreeShort_p1.Z + columnLength - rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeThreeShort_p3 = new XYZ(Math.Round(mainRebarTypeThreeShort_p2.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p2.Y + mainRebarDiamTypeThree, 6)
                                , Math.Round(mainRebarTypeThreeShort_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeThreeShort_p4 = new XYZ(Math.Round(mainRebarTypeThreeShort_p3.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p3.Y, 6)
                                , Math.Round(mainRebarTypeThreeShort_p3.Z + rebarOutletsLengthShort, 6));

                            //Кривые стержня длинного
                            List<Curve> myMainRebarTypeThreeCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeThreeLong_line1 = Line.CreateBound(mainRebarTypeThreeLong_p1, mainRebarTypeThreeLong_p2) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line1);
                            Curve myMainRebarTypeThreeLong_line2 = Line.CreateBound(mainRebarTypeThreeLong_p2, mainRebarTypeThreeLong_p3) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line2);
                            Curve myMainRebarTypeThreeLong_line3 = Line.CreateBound(mainRebarTypeThreeLong_p3, mainRebarTypeThreeLong_p4) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line3);

                            //Кривые стержня короткого
                            List<Curve> myMainRebarTypeThreeCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeThreeShort_line1 = Line.CreateBound(mainRebarTypeThreeShort_p1, mainRebarTypeThreeShort_p2) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line1);
                            Curve myMainRebarTypeThreeShort_line2 = Line.CreateBound(mainRebarTypeThreeShort_p2, mainRebarTypeThreeShort_p3) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line2);
                            Curve myMainRebarTypeThreeShort_line3 = Line.CreateBound(mainRebarTypeThreeShort_p3, mainRebarTypeThreeShort_p4) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line3);

                            //Нижняя грань короткие
                            Rebar columnMainRebarBottomFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Cтержни нижняя и верхняя грани
                            int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                            double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                            stepBarsTBFacesForStirrup = stepBarsTBFaces;
                            double residueForOffsetTB = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;
                            residueForOffsetForStirrupTB = residueForOffsetTB;

                            XYZ newPlaсeСolumnMainRebarBottomFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffsetTB
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            rebarIdCollection.Add(columnMainRebarBottomFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Нижняя грань длинные
                                Rebar columnMainRebarBottomFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarBottomFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces * 2 + residueForOffsetTB
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                                rebarIdCollection.Add(columnMainRebarBottomFaceLong.Id);
                            }

                            //Верхняя грань короткие
                            Rebar columnMainRebarTopFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarTopFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffsetTB
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            rebarIdCollection.Add(columnMainRebarTopFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Верхняя грань длинные
                                Rebar columnMainRebarTopFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarTopFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces * 2 - residueForOffsetTB
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                                rebarIdCollection.Add(columnMainRebarTopFaceLong.Id);
                            }
                        }

                        #endregion
                    }
                    //Если переход со стыковки на сварке в нахлест c изменением сечения колонны выше
                    else if (checkedRebarOutletsButtonName == "radioButton_MainWeldingRods" & transitionToOverlap == true & changeColumnSection == true & sectionOffset <= 50 / 304.8)
                    {
#region Угловые стержни
                        //Точки для построения кривых стержня один длинного
                        XYZ mainRebarTypeOneLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                        , Math.Round(columnOrigin.Y, 6)
                        , Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                        XYZ mainRebarTypeOneLong_p2 = new XYZ(Math.Round(mainRebarTypeOneLong_p1.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p1.Z + columnLength - rebarOutletsLengthLong - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ mainRebarTypeOneLong_p3 = new XYZ(Math.Round(mainRebarTypeOneLong_p2.X + mainRebarDiamTypeOne + sectionOffset, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ mainRebarTypeOneLong_p4 = new XYZ(Math.Round(mainRebarTypeOneLong_p3.X, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneLong_p3.Z + rebarOutletsLengthLong, 6));

                        //Точки для построения кривых стержня один короткого
                        XYZ mainRebarTypeOneShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                            , Math.Round(columnOrigin.Y, 6)
                            , Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                        XYZ mainRebarTypeOneShort_p2 = new XYZ(Math.Round(mainRebarTypeOneShort_p1.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p1.Z + columnLength - rebarOutletsLengthShort - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ mainRebarTypeOneShort_p3 = new XYZ(Math.Round(mainRebarTypeOneShort_p2.X + mainRebarDiamTypeOne + sectionOffset, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                        XYZ mainRebarTypeOneShort_p4 = new XYZ(Math.Round(mainRebarTypeOneShort_p3.X, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Y, 6)
                            , Math.Round(mainRebarTypeOneShort_p3.Z + rebarOutletsLengthShort, 6));

                        //Кривые стержня один длинного
                        List<Curve> myMainRebarTypeOneCurvesLong = new List<Curve>();

                        Curve myMainRebarTypeOneLong_line1 = Line.CreateBound(mainRebarTypeOneLong_p1, mainRebarTypeOneLong_p2) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line1);
                        Curve myMainRebarTypeOneLong_line2 = Line.CreateBound(mainRebarTypeOneLong_p2, mainRebarTypeOneLong_p3) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line2);
                        Curve myMainRebarTypeOneLong_line3 = Line.CreateBound(mainRebarTypeOneLong_p3, mainRebarTypeOneLong_p4) as Curve;
                        myMainRebarTypeOneCurvesLong.Add(myMainRebarTypeOneLong_line3);

                        //Кривые стержня один короткого
                        List<Curve> myMainRebarTypeOneCurvesShort = new List<Curve>();

                        Curve myMainRebarTypeOneShort_line1 = Line.CreateBound(mainRebarTypeOneShort_p1, mainRebarTypeOneShort_p2) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line1);
                        Curve myMainRebarTypeOneShort_line2 = Line.CreateBound(mainRebarTypeOneShort_p2, mainRebarTypeOneShort_p3) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line2);
                        Curve myMainRebarTypeOneShort_line3 = Line.CreateBound(mainRebarTypeOneShort_p3, mainRebarTypeOneShort_p4) as Curve;
                        myMainRebarTypeOneCurvesShort.Add(myMainRebarTypeOneShort_line3);

                        //Нижний левый угол
                        Rebar columnMainRebarLowerLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebarLowerLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarLowerLeftСorner.Id, newPlaсeСolumnMainRebarLowerLeftСorner);

                        rebarIdCollection.Add(columnMainRebarLowerLeftСorner.Id);

                        //Верхний левый угол
                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesLong
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }
                        else if (numberOfBarsLRFaces % 2 == 0)
                        {
                            Rebar columnMainRebarUpperLeftСorner = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeOne
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeOneCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            XYZ newPlaсeСolumnMainRebarUpperLeftСorner = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarUpperLeftСorner.Id, newPlaсeСolumnMainRebarUpperLeftСorner);

                            rebarIdCollection.Add(columnMainRebarUpperLeftСorner.Id);
                        }

                        //Верхний правый угол
                        Rebar columnMainRebarUpperRightСorner = Rebar.CreateFromCurvesAndShape(doc
                        , myMainRebarShapeOverlappingRods
                        , myMainRebarTypeOne
                        , null
                        , null
                        , myColumn
                        , mainRebarNormalMain
                        , myMainRebarTypeOneCurvesLong
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        XYZ rotate_p1 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z);
                        XYZ rotate_p2 = new XYZ(mainRebarTypeOneLong_p1.X, mainRebarTypeOneLong_p1.Y, mainRebarTypeOneLong_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebarUpperRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeColumnMainRebarUpperRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebarUpperRightСorner.Id, newPlaсeColumnMainRebarUpperRightСorner);

                        rebarIdCollection.Add(columnMainRebarUpperRightСorner.Id);

                        if (numberOfBarsLRFaces % 2 != 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }

                        if (numberOfBarsLRFaces % 2 == 0)
                        {
                            //Нижний правый угол
                            Rebar columnMainRebarLowerRightСorner = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeOne
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeOneCurvesShort
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarLowerRightСorner.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarLowerRightСorner = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2, -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2, 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLowerRightСorner.Id, newPlaсeColumnMainRebarLowerRightСorner);

                            rebarIdCollection.Add(columnMainRebarLowerRightСorner.Id);
                        }
                        #endregion

#region Стержни по левой и правой граням
                        if (numberOfBarsLRFaces >= 3)
                        {
                            //Точки для построения кривфх стержня два удлиненного
                            XYZ mainRebarTypeTwoLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeTwoLong_p2 = new XYZ(Math.Round(mainRebarTypeTwoLong_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p1.Z + columnLength - rebarOutletsLengthLong - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                            XYZ mainRebarTypeTwoLong_p3 = new XYZ(Math.Round(mainRebarTypeTwoLong_p2.X + mainRebarDiamTypeTwo + sectionOffset, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                            XYZ mainRebarTypeTwoLong_p4 = new XYZ(Math.Round(mainRebarTypeTwoLong_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня два укороченного
                            XYZ mainRebarTypeTwoShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeTwoShort_p2 = new XYZ(Math.Round(mainRebarTypeTwoShort_p1.X, 6)
                                , Math.Round(mainRebarTypeTwoShort_p1.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p1.Z + columnLength - rebarOutletsLengthShort - (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                            XYZ mainRebarTypeTwoShort_p3 = new XYZ(Math.Round(mainRebarTypeTwoShort_p2.X + mainRebarDiamTypeTwo + sectionOffset, 6)
                                , Math.Round(mainRebarTypeTwoShort_p2.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p2.Z + floorThicknessAboveColumn + (sectionOffset * 6 - floorThicknessAboveColumn), 6));
                            XYZ mainRebarTypeTwoShort_p4 = new XYZ(Math.Round(mainRebarTypeTwoShort_p3.X, 6)
                                , Math.Round(mainRebarTypeTwoShort_p3.Y, 6)
                                , Math.Round(mainRebarTypeTwoShort_p3.Z + rebarOutletsLengthShort, 6));

                            //Кривые стержня удлиненного
                            List<Curve> myMainRebarTypeTwoCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeTwoLong_line1 = Line.CreateBound(mainRebarTypeTwoLong_p1, mainRebarTypeTwoLong_p2) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line1);
                            Curve myMainRebarTypeTwoLong_line2 = Line.CreateBound(mainRebarTypeTwoLong_p2, mainRebarTypeTwoLong_p3) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line2);
                            Curve myMainRebarTypeTwoLong_line3 = Line.CreateBound(mainRebarTypeTwoLong_p3, mainRebarTypeTwoLong_p4) as Curve;
                            myMainRebarTypeTwoCurvesLong.Add(myMainRebarTypeTwoLong_line3);

                            //Кривые стержня укороченного
                            List<Curve> myMainRebarTypeTwoCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeTwoShort_line1 = Line.CreateBound(mainRebarTypeTwoShort_p1, mainRebarTypeTwoShort_p2) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line1);
                            Curve myMainRebarTypeTwoShort_line2 = Line.CreateBound(mainRebarTypeTwoShort_p2, mainRebarTypeTwoShort_p3) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line2);
                            Curve myMainRebarTypeTwoShort_line3 = Line.CreateBound(mainRebarTypeTwoShort_p3, mainRebarTypeTwoShort_p4) as Curve;
                            myMainRebarTypeTwoCurvesShort.Add(myMainRebarTypeTwoShort_line3);

                            //Левая грань короткие
                            Rebar columnMainRebarLeftFaceLong = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Расчеты для размещения стержней
                            int numberOfSpacesLRFaces = numberOfBarsLRFaces - 1;
                            double residualSizeLRFaces = columnSectionHeight - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsLRFaces = RoundUpToFive(Math.Round((residualSizeLRFaces / numberOfSpacesLRFaces) * 304.8)) / 304.8;
                            stepBarsLRFacesForStirrup = stepBarsLRFaces;
                            double residueForOffsetLR = (residualSizeLRFaces - (stepBarsLRFaces * numberOfSpacesLRFaces)) / 2;
                            residueForOffsetForStirrupLR = residueForOffsetLR;

                            XYZ newPlaсeСolumnMainRebarLeftFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces + residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceLong.Id, newPlaсeСolumnMainRebarLeftFaceLong);
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarLeftFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            rebarIdCollection.Add(columnMainRebarLeftFaceLong.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Левая грань длинные
                                Rebar columnMainRebarLeftFaceShort = Rebar.CreateFromCurvesAndShape(doc
                                , myMainRebarShapeOverlappingRods
                                , myMainRebarTypeTwo
                                , null
                                , null
                                , myColumn
                                , mainRebarNormalMain
                                , myMainRebarTypeTwoCurvesLong
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarLeftFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeTwo / 2
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFaces * 2 + residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarLeftFaceShort.Id, newPlaсeСolumnMainRebarLeftFaceShort);

                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarLeftFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                                rebarIdCollection.Add(columnMainRebarLeftFaceShort.Id);
                            }

                            //Правая грань короткий
                            Rebar columnMainRebarRightFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeTwo
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalMain
                            , myMainRebarTypeTwoCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeColumnMainRebarRightFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces - residueForOffsetLR
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceShort.Id, newPlaсeColumnMainRebarRightFaceShort);
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsLRFaces % 2 == 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                            }
                            if (numberOfBarsLRFaces % 2 != 0)
                            {
                                columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarRightFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                            rebarIdCollection.Add(columnMainRebarRightFaceShort.Id);

                            if (numberOfBarsLRFaces > 3)
                            {
                                //Правая грань длинный
                                Rebar columnMainRebarRightFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeTwo
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalMain
                                    , myMainRebarTypeTwoCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarRightFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeColumnMainRebarRightFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeTwo / 2
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsLRFaces * 2 - residueForOffsetLR
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarRightFaceLong.Id, newPlaсeColumnMainRebarRightFaceLong);
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsLRFaces % 2 == 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsLRFaces - 2) / 2);
                                }
                                if (numberOfBarsLRFaces % 2 != 0)
                                {
                                    columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsLRFaces - 2) / 2)));
                                }
                                columnMainRebarRightFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsLRFaces * 2);

                                rebarIdCollection.Add(columnMainRebarRightFaceLong.Id);
                            }
                        }
#endregion

#region Стержни по нижней и верхней граням
                        if (numberOfBarsTBFaces >= 3)
                        {
                            //Точки для построения кривых стержня три длинного
                            XYZ mainRebarTypeThreeLong_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z + rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeThreeLong_p2 = new XYZ(Math.Round(mainRebarTypeThreeLong_p1.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p1.Y, 6)
                                , Math.Round(mainRebarTypeThreeLong_p1.Z + columnLength - rebarOutletsLengthLong, 6));
                            XYZ mainRebarTypeThreeLong_p3 = new XYZ(Math.Round(mainRebarTypeThreeLong_p2.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p2.Y + mainRebarDiamTypeThree, 6)
                                , Math.Round(mainRebarTypeThreeLong_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeThreeLong_p4 = new XYZ(Math.Round(mainRebarTypeThreeLong_p3.X, 6)
                                , Math.Round(mainRebarTypeThreeLong_p3.Y, 6)
                                , Math.Round(mainRebarTypeThreeLong_p3.Z + rebarOutletsLengthLong, 6));

                            //Точки для построения кривфх стержня три короткого
                            XYZ mainRebarTypeThreeShort_p1 = new XYZ(Math.Round(columnOrigin.X, 6)
                                , Math.Round(columnOrigin.Y, 6)
                                , Math.Round(columnOrigin.Z + rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeThreeShort_p2 = new XYZ(Math.Round(mainRebarTypeThreeShort_p1.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p1.Y, 6)
                                , Math.Round(mainRebarTypeThreeShort_p1.Z + columnLength - rebarOutletsLengthShort, 6));
                            XYZ mainRebarTypeThreeShort_p3 = new XYZ(Math.Round(mainRebarTypeThreeShort_p2.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p2.Y + mainRebarDiamTypeThree, 6)
                                , Math.Round(mainRebarTypeThreeShort_p2.Z + floorThicknessAboveColumn, 6));
                            XYZ mainRebarTypeThreeShort_p4 = new XYZ(Math.Round(mainRebarTypeThreeShort_p3.X, 6)
                                , Math.Round(mainRebarTypeThreeShort_p3.Y, 6)
                                , Math.Round(mainRebarTypeThreeShort_p3.Z + rebarOutletsLengthShort, 6));

                            //Кривые стержня длинного
                            List<Curve> myMainRebarTypeThreeCurvesLong = new List<Curve>();

                            Curve myMainRebarTypeThreeLong_line1 = Line.CreateBound(mainRebarTypeThreeLong_p1, mainRebarTypeThreeLong_p2) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line1);
                            Curve myMainRebarTypeThreeLong_line2 = Line.CreateBound(mainRebarTypeThreeLong_p2, mainRebarTypeThreeLong_p3) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line2);
                            Curve myMainRebarTypeThreeLong_line3 = Line.CreateBound(mainRebarTypeThreeLong_p3, mainRebarTypeThreeLong_p4) as Curve;
                            myMainRebarTypeThreeCurvesLong.Add(myMainRebarTypeThreeLong_line3);

                            //Кривые стержня короткого
                            List<Curve> myMainRebarTypeThreeCurvesShort = new List<Curve>();

                            Curve myMainRebarTypeThreeShort_line1 = Line.CreateBound(mainRebarTypeThreeShort_p1, mainRebarTypeThreeShort_p2) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line1);
                            Curve myMainRebarTypeThreeShort_line2 = Line.CreateBound(mainRebarTypeThreeShort_p2, mainRebarTypeThreeShort_p3) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line2);
                            Curve myMainRebarTypeThreeShort_line3 = Line.CreateBound(mainRebarTypeThreeShort_p3, mainRebarTypeThreeShort_p4) as Curve;
                            myMainRebarTypeThreeCurvesShort.Add(myMainRebarTypeThreeShort_line3);

                            //Нижняя грань короткие
                            Rebar columnMainRebarBottomFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            //Cтержни нижняя и верхняя грани
                            int numberOfSpacesTBFaces = numberOfBarsTBFaces - 1;
                            double residualSizeTBFaces = columnSectionWidth - mainRebarCoverLayer * 2 - mainRebarDiamTypeOne;
                            double stepBarsTBFaces = RoundUpToFive(Math.Round((residualSizeTBFaces / numberOfSpacesTBFaces) * 304.8)) / 304.8;
                            stepBarsTBFacesForStirrup = stepBarsTBFaces;
                            double residueForOffsetTB = (residualSizeTBFaces - (stepBarsTBFaces * numberOfSpacesTBFaces)) / 2;
                            residueForOffsetForStirrupTB = residueForOffsetTB;

                            XYZ newPlaсeСolumnMainRebarBottomFaceShort = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces + residueForOffsetTB
                                , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceShort.Id, newPlaсeСolumnMainRebarBottomFaceShort);
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarBottomFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            rebarIdCollection.Add(columnMainRebarBottomFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Нижняя грань длинные
                                Rebar columnMainRebarBottomFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                XYZ newPlaсeСolumnMainRebarBottomFaceLong = new XYZ(-columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFaces * 2 + residueForOffsetTB
                                    , -columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarBottomFaceLong.Id, newPlaсeСolumnMainRebarBottomFaceLong);
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarBottomFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                                rebarIdCollection.Add(columnMainRebarBottomFaceLong.Id);
                            }

                            //Верхняя грань короткие
                            Rebar columnMainRebarTopFaceShort = Rebar.CreateFromCurvesAndShape(doc
                            , myMainRebarShapeOverlappingRods
                            , myMainRebarTypeThree
                            , null
                            , null
                            , myColumn
                            , mainRebarNormalAdditional
                            , myMainRebarTypeThreeCurvesShort
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceShort.Id, rotateLine, 180 * (Math.PI / 180));
                            XYZ newPlaсeСolumnMainRebarTopFaceShort = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces - residueForOffsetTB
                                , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                , 0);
                            ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceShort.Id, newPlaсeСolumnMainRebarTopFaceShort);
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            if (numberOfBarsTBFaces % 2 == 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                            }
                            if (numberOfBarsTBFaces % 2 != 0)
                            {
                                columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)) + 1);
                            }
                            columnMainRebarTopFaceShort.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                            rebarIdCollection.Add(columnMainRebarTopFaceShort.Id);

                            if (numberOfBarsTBFaces > 3)
                            {
                                //Верхняя грань длинные
                                Rebar columnMainRebarTopFaceLong = Rebar.CreateFromCurvesAndShape(doc
                                    , myMainRebarShapeOverlappingRods
                                    , myMainRebarTypeThree
                                    , null
                                    , null
                                    , myColumn
                                    , mainRebarNormalAdditional
                                    , myMainRebarTypeThreeCurvesLong
                                    , RebarHookOrientation.Right
                                    , RebarHookOrientation.Right);

                                ElementTransformUtils.RotateElement(doc, columnMainRebarTopFaceLong.Id, rotateLine, 180 * (Math.PI / 180));
                                XYZ newPlaсeСolumnMainRebarTopFaceLong = new XYZ(columnSectionWidth / 2 - mainRebarCoverLayer - mainRebarDiamTypeOne / 2 - stepBarsTBFaces * 2 - residueForOffsetTB
                                    , columnSectionHeight / 2 - mainRebarCoverLayer - mainRebarDiamTypeThree / 2
                                    , 0);
                                ElementTransformUtils.MoveElement(doc, columnMainRebarTopFaceLong.Id, newPlaсeСolumnMainRebarTopFaceLong);
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                                if (numberOfBarsTBFaces % 2 == 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set((numberOfBarsTBFaces - 2) / 2);
                                }
                                if (numberOfBarsTBFaces % 2 != 0)
                                {
                                    columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(Math.Round(Convert.ToDouble((numberOfBarsTBFaces - 2) / 2)));
                                }
                                columnMainRebarTopFaceLong.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepBarsTBFaces * 2);

                                rebarIdCollection.Add(columnMainRebarTopFaceLong.Id);
                            }
                        }

#endregion
                    }

#region Хомуты и стяжки
                    if (checkedRebarStrappingTypeButtonName == "radioButton_StrappingTypePylon")
                    {
                        //Хомут
                        //Нормаль для построения хомута
                        XYZ normalStirrup = new XYZ(0, 0, 1);

                        if (numberOfBarsLRFaces > 5)
                        {
                            //Точки для построения кривых стержня хомута 1
                            XYZ rebarStirrupFirst_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam, 6)
                            , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2, 6)
                            , Math.Round(columnOrigin.Z + firstStirrupOffset, 6));

                            XYZ rebarStirrupFirst_p2 = new XYZ(Math.Round(rebarStirrupFirst_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam, 6)
                                , Math.Round(rebarStirrupFirst_p1.Y, 6)
                                , Math.Round(rebarStirrupFirst_p1.Z, 6));

                            XYZ rebarStirrupFirst_p3 = new XYZ(Math.Round(rebarStirrupFirst_p2.X, 6)
                                , Math.Round(rebarStirrupFirst_p2.Y
                                - stepBarsLRFacesForStirrup * numberOfSpacesLRFacesForStirrup
                                - residueForOffsetForStirrupLR
                                - mainRebarDiamTypeOne / 2
                                - mainRebarDiamTypeTwo / 2
                                - stirrupRebarDiam
                                - stirrupRebarDiam / 2, 6)
                                , Math.Round(rebarStirrupFirst_p2.Z, 6));

                            XYZ rebarStirrupFirst_p4 = new XYZ(Math.Round(rebarStirrupFirst_p3.X - columnSectionWidth + mainRebarCoverLayer * 2 - stirrupRebarDiam, 6)
                                , Math.Round(rebarStirrupFirst_p3.Y, 6)
                                , Math.Round(rebarStirrupFirst_p3.Z, 6));

                            //Кривые хомута 1
                            List<Curve> myStirrupFirstCurves = new List<Curve>();

                            Curve firstStirrup_line1 = Line.CreateBound(rebarStirrupFirst_p1, rebarStirrupFirst_p2) as Curve;
                            myStirrupFirstCurves.Add(firstStirrup_line1);
                            Curve firstStirrup_line2 = Line.CreateBound(rebarStirrupFirst_p2, rebarStirrupFirst_p3) as Curve;
                            myStirrupFirstCurves.Add(firstStirrup_line2);
                            Curve firstStirrup_line3 = Line.CreateBound(rebarStirrupFirst_p3, rebarStirrupFirst_p4) as Curve;
                            myStirrupFirstCurves.Add(firstStirrup_line3);
                            Curve firstStirrup_line4 = Line.CreateBound(rebarStirrupFirst_p4, rebarStirrupFirst_p1) as Curve;
                            myStirrupFirstCurves.Add(firstStirrup_line4);

                            //Построение нижнего хомута 1
                            Rebar columnRebarFirstDownStirrup = Rebar.CreateFromCurvesAndShape(doc
                                , myStirrupRebarShape
                                , myStirrupBarTape
                                , myRebarHookType
                                , myRebarHookType
                                , myColumn
                                , normalStirrup
                                , myStirrupFirstCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            columnRebarFirstDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarFirstDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                            columnRebarFirstDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);

                            rebarIdCollection.Add(columnRebarFirstDownStirrup.Id);

                            //Копирование хомута 1
                            XYZ pointFirstTopStirrupInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                            List<ElementId> columnRebarFirstTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarFirstDownStirrup.Id, pointFirstTopStirrupInstallation) as List<ElementId>;
                            Element columnRebarFirstTopStirrup = doc.GetElement(columnRebarFirstTopStirrupIdList.First());

                            //Высота размещения хомутов со стандартным шагом
                            double StirrupStandardInstallationHeigh = columnLength - stirrupIncreasedPlacementHeight - firstStirrupOffset - 50 / 304.8;
                            int stirrupBarElemStandardQuantity = (int)(StirrupStandardInstallationHeigh / standardStirrupStep);
                            //Высота установки последнего хомута
                            double lastStirrupInstallationHeigh = columnLength - firstStirrupOffset - 50 / 304.8;

                            columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                            columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                            rebarIdCollection.Add(columnRebarFirstTopStirrup.Id);

                            //Копирование хомута 1 последний
                            XYZ pointLastTopStirrupInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                            List<ElementId> columnRebarLastTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarFirstDownStirrup.Id, pointLastTopStirrupInstallation) as List<ElementId>;
                            Element columnRebarLastTopStirrup = doc.GetElement(columnRebarLastTopStirrupIdList.First());
                            columnRebarLastTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                            rebarIdCollection.Add(columnRebarLastTopStirrup.Id);

                            //Точки для построения кривых стержня хомута 2
                            XYZ rebarStirrupSecond_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam / 2, 6)
                                , Math.Round(columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer - stirrupRebarDiam, 6)
                                , Math.Round(columnOrigin.Z + firstStirrupOffset + stirrupRebarDiam, 6));

                            XYZ rebarStirrupSecond_p2 = new XYZ(Math.Round(rebarStirrupSecond_p1.X, 6)
                                , Math.Round(rebarStirrupSecond_p1.Y
                                + stepBarsLRFacesForStirrup * numberOfSpacesLRFacesForStirrup
                                + residueForOffsetForStirrupLR
                                + mainRebarDiamTypeOne / 2
                                + mainRebarDiamTypeTwo / 2
                                + stirrupRebarDiam, 6)
                                , Math.Round(rebarStirrupSecond_p1.Z, 6));

                            XYZ rebarStirrupSecond_p3 = new XYZ(Math.Round(rebarStirrupSecond_p2.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam + stirrupRebarDiam/2, 6)
                                , Math.Round(rebarStirrupSecond_p2.Y, 6)
                                , Math.Round(rebarStirrupSecond_p2.Z, 6));

                            XYZ rebarStirrupSecond_p4 = new XYZ(Math.Round(rebarStirrupSecond_p3.X, 6)
                                , Math.Round(rebarStirrupSecond_p3.Y
                                - stepBarsLRFacesForStirrup * numberOfSpacesLRFacesForStirrup
                                - residueForOffsetForStirrupLR
                                - mainRebarDiamTypeOne / 2
                                - mainRebarDiamTypeTwo / 2
                                - stirrupRebarDiam, 6)
                                , Math.Round(rebarStirrupSecond_p3.Z, 6));

                            //Кривые хомута 2
                            List<Curve> myStirrupSecondCurves = new List<Curve>();

                            Curve secondStirrup_line1 = Line.CreateBound(rebarStirrupSecond_p1, rebarStirrupSecond_p2) as Curve;
                            myStirrupSecondCurves.Add(secondStirrup_line1);
                            Curve secondStirrup_line2 = Line.CreateBound(rebarStirrupSecond_p2, rebarStirrupSecond_p3) as Curve;
                            myStirrupSecondCurves.Add(secondStirrup_line2);
                            Curve secondStirrup_line3 = Line.CreateBound(rebarStirrupSecond_p3, rebarStirrupSecond_p4) as Curve;
                            myStirrupSecondCurves.Add(secondStirrup_line3);
                            Curve secondStirrup_line4 = Line.CreateBound(rebarStirrupSecond_p4, rebarStirrupSecond_p1) as Curve;
                            myStirrupSecondCurves.Add(secondStirrup_line4);

                            //Построение нижнего хомута 2
                            Rebar columnRebarSecondDownStirrup = Rebar.CreateFromCurvesAndShape(doc
                                , myStirrupRebarShape
                                , myStirrupBarTape
                                , myRebarHookType
                                , myRebarHookType
                                , myColumn
                                , normalStirrup
                                , myStirrupSecondCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            columnRebarSecondDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarSecondDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                            columnRebarSecondDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);

                            rebarIdCollection.Add(columnRebarSecondDownStirrup.Id);

                            //Копирование хомута 2
                            XYZ pointSecondTopStirrupInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                            List<ElementId> columnRebarSecondTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarSecondDownStirrup.Id, pointSecondTopStirrupInstallation) as List<ElementId>;
                            Element columnRebarSecondTopStirrup = doc.GetElement(columnRebarSecondTopStirrupIdList.First());

                            columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                            columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                            rebarIdCollection.Add(columnRebarSecondTopStirrup.Id);

                            // Копирование хомута 2 последний
                            XYZ pointLastSecondTopStirrupInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                            List<ElementId> columnRebarLastSecondTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarSecondDownStirrup.Id, pointLastSecondTopStirrupInstallation) as List<ElementId>;
                            Element columnRebarLastSecondTopStirrup = doc.GetElement(columnRebarLastSecondTopStirrupIdList.First());
                            columnRebarLastSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                            rebarIdCollection.Add(columnRebarLastSecondTopStirrup.Id);
                        }

                        if (numberOfBarsLRFaces <= 5)
                        {
                            //Точки для построения кривых стержня хомута
                            XYZ rebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam, 6)
                                , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2, 6)
                                , Math.Round(columnOrigin.Z + firstStirrupOffset, 6));

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

                            //Построение нижнего хомута
                            Rebar columnRebarDownStirrup = Rebar.CreateFromCurvesAndShape(doc, myStirrupRebarShape, myStirrupBarTape, myRebarHookType, myRebarHookType, myColumn, normalStirrup, myStirrupCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);

                            columnRebarDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                            columnRebarDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);
                            rebarIdCollection.Add(columnRebarDownStirrup.Id);

                            //Копирование хомута
                            XYZ pointTopStirrupInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                            List<ElementId> columnRebarTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownStirrup.Id, pointTopStirrupInstallation) as List<ElementId>;
                            Element columnRebarTopStirrup = doc.GetElement(columnRebarTopStirrupIdList.First());

                            //Высота размещения хомутов со стандартным шагом
                            double StirrupStandardInstallationHeigh = columnLength - stirrupIncreasedPlacementHeight - firstStirrupOffset - 50 / 304.8;
                            int StirrupBarElemStandardQuantity = (int)(StirrupStandardInstallationHeigh / standardStirrupStep);
                            //Высота установки последнего хомута
                            double lastStirrupInstallationHeigh = columnLength - firstStirrupOffset - 50 / 304.8;

                            columnRebarTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemStandardQuantity);
                            columnRebarTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                            rebarIdCollection.Add(columnRebarTopStirrup.Id);

                            //Копирование хомута последний
                            XYZ pointLastTopStirrupInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                            List<ElementId> columnRebarLastTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownStirrup.Id, pointLastTopStirrupInstallation) as List<ElementId>;
                            Element columnRebarLastTopStirrup = doc.GetElement(columnRebarLastTopStirrupIdList.First());
                            columnRebarLastTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                            rebarIdCollection.Add(columnRebarLastTopStirrup.Id);
                        }

                        if (numberOfBarsLRFaces > 7)
                        {
                            //Шпилька
                            //Точки для построения кривых стержня шпильки
                            double rebarStandardHookBendDiameter = myPinBarTape.get_Parameter(BuiltInParameter.REBAR_STANDARD_HOOK_BEND_DIAMETER).AsDouble();

                            XYZ rebarDownPin_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - pinRebarDiam, 6)
                                , Math.Round(columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFacesForStirrup * 4 + residueForOffsetForStirrupLR + rebarStandardHookBendDiameter / 2 + pinRebarDiam / 2, 6)
                                , Math.Round(columnOrigin.Z + firstStirrupOffset + stirrupRebarDiam + pinRebarDiam, 6));

                            XYZ rebarDownPin_p2 = new XYZ(Math.Round(columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer + pinRebarDiam, 6)
                                , Math.Round(rebarDownPin_p1.Y, 6)
                                , Math.Round(rebarDownPin_p1.Z, 6));

                            //Кривые шпильки
                            List<Curve> myDownPinCurves = new List<Curve>();
                            Curve downPin_line1 = Line.CreateBound(rebarDownPin_p1, rebarDownPin_p2) as Curve;
                            myDownPinCurves.Add(downPin_line1);

                            Rebar columnRebarDownPin = Rebar.CreateFromCurvesAndShape(doc
                                , myPinRebarShape
                                , myPinBarTape
                                , myRebarPinHookType
                                , myRebarPinHookType
                                , myColumn
                                , normalStirrup
                                , myDownPinCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);
                            rebarIdCollection.Add(columnRebarDownPin.Id);

                            //Высота размещения хомутов со стандартным шагом
                            double StirrupStandardInstallationHeigh = columnLength - stirrupIncreasedPlacementHeight - firstStirrupOffset - 50 / 304.8;
                            int stirrupBarElemStandardQuantity = (int)(StirrupStandardInstallationHeigh / standardStirrupStep);
                            //Высота установки последней шпильки 
                            double lastStirrupInstallationHeigh = columnLength - firstStirrupOffset - 50 / 304.8;

                            //Копирование шпильки вверх
                            XYZ pointTopPinInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                            List<ElementId> columnRebarTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointTopPinInstallation) as List<ElementId>;
                            Element columnRebarTopPin = doc.GetElement(columnRebarTopPinIdList.First());

                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                            rebarIdCollection.Add(columnRebarTopPin.Id);

                            //Копирование шпильки последний
                            XYZ pointLastTopPinInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                            List<ElementId> columnRebarLastTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointLastTopPinInstallation) as List<ElementId>;
                            Element columnRebarLastTopPin = doc.GetElement(columnRebarLastTopPinIdList.First());
                            columnRebarLastTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                            rebarIdCollection.Add(columnRebarLastTopPin.Id);

                            if (numberOfBarsLRFaces > 9)
                            {
                                int n = (numberOfBarsLRFaces - 8) / 2; // Необходимое кол-во копий шпильки
                                for (int i = 1; i <= n; i++)
                                {
                                    XYZ pointPinInstallation = new XYZ(0, (stepBarsLRFacesForStirrup * 2) * i, 0);
                                    List<ElementId> newColumnRebarDownPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointPinInstallation) as List<ElementId>;
                                    Element newColumnRebarDownPin = doc.GetElement(newColumnRebarDownPinIdList.First());
                                    rebarIdCollection.Add(newColumnRebarDownPin.Id);

                                    List<ElementId> newColumnRebarTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarTopPin.Id, pointPinInstallation) as List<ElementId>;
                                    Element newColumnRebarTopPin = doc.GetElement(newColumnRebarTopPinIdList.First());
                                    rebarIdCollection.Add(newColumnRebarTopPin.Id);

                                    List<ElementId> newColumnRebarLastTopPinList = ElementTransformUtils.CopyElement(doc, columnRebarLastTopPin.Id, pointPinInstallation) as List<ElementId>;
                                    Element newColumnRebarLastTopPin = doc.GetElement(newColumnRebarLastTopPinList.First());
                                    rebarIdCollection.Add(newColumnRebarLastTopPin.Id);
                                }
                            }
                        }
                    }

                    if (checkedRebarStrappingTypeButtonName == "radioButton_StrappingTypeColumn")
                    {
                        //Хомут
                        //Нормаль для построения хомута
                        XYZ normalStirrup = new XYZ(0, 0, 1);

                        //Точки для построения кривых стержня хомута
                        XYZ rebarStirrup_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - stirrupRebarDiam, 6)
                            , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + stirrupRebarDiam / 2, 6)
                            , Math.Round(columnOrigin.Z + firstStirrupOffset, 6));

                        XYZ rebarStirrup_p2 = new XYZ(Math.Round(rebarStirrup_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam, 6)
                            , Math.Round(rebarStirrup_p1.Y, 6)
                            , Math.Round(rebarStirrup_p1.Z, 6));

                        XYZ rebarStirrup_p3 = new XYZ(Math.Round(rebarStirrup_p2.X, 6)
                            , Math.Round(rebarStirrup_p2.Y - columnSectionHeight + mainRebarCoverLayer * 2 - stirrupRebarDiam - stirrupRebarDiam/2, 6)
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

                        //Построение нижнего хомута
                        Rebar columnRebarDownStirrup = Rebar.CreateFromCurvesAndShape(doc, myStirrupRebarShape, myStirrupBarTape, myRebarHookType, myRebarHookType, myColumn, normalStirrup, myStirrupCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);

                        columnRebarDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnRebarDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                        columnRebarDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);
                        rebarIdCollection.Add(columnRebarDownStirrup.Id);

                        //Копирование хомута
                        XYZ pointTopStirrupInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                        List<ElementId> columnRebarTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownStirrup.Id, pointTopStirrupInstallation) as List<ElementId>;
                        Element columnRebarTopStirrup = doc.GetElement(columnRebarTopStirrupIdList.First());

                        //Высота размещения хомутов со стандартным шагом
                        double StirrupStandardInstallationHeigh = columnLength - stirrupIncreasedPlacementHeight - firstStirrupOffset - 50 / 304.8;
                        int stirrupBarElemStandardQuantity = (int)(StirrupStandardInstallationHeigh / standardStirrupStep);
                        //Высота установки последнего хомута
                        double lastStirrupInstallationHeigh = columnLength - firstStirrupOffset - 50 / 304.8;

                        columnRebarTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        columnRebarTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                        columnRebarTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                        rebarIdCollection.Add(columnRebarTopStirrup.Id);

                        //Копирование хомута последний
                        XYZ pointLastTopStirrupInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                        List<ElementId> columnRebarLastTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownStirrup.Id, pointLastTopStirrupInstallation) as List<ElementId>;
                        Element columnRebarLastTopStirrup = doc.GetElement(columnRebarLastTopStirrupIdList.First());
                        columnRebarLastTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                        rebarIdCollection.Add(columnRebarLastTopStirrup.Id);

                        if (numberOfBarsLRFaces == 3 || numberOfBarsLRFaces == 5)
                        {
                            //Шпилька
                            //Точки для построения кривых стержня шпильки
                            double rebarStandardHookBendDiameter = myPinBarTape.get_Parameter(BuiltInParameter.REBAR_STANDARD_HOOK_BEND_DIAMETER).AsDouble();

                            XYZ rebarDownPin_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - pinRebarDiam, 6)
                                , Math.Round(columnOrigin.Y + rebarStandardHookBendDiameter / 2 + pinRebarDiam / 2, 6)
                                , Math.Round(columnOrigin.Z + firstStirrupOffset + stirrupRebarDiam * 2 + pinRebarDiam, 6));

                            XYZ rebarDownPin_p2 = new XYZ(Math.Round(columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer + pinRebarDiam, 6)
                                , Math.Round(rebarDownPin_p1.Y, 6)
                                , Math.Round(rebarDownPin_p1.Z, 6));

                            //Кривые шпильки
                            List<Curve> myDownPinCurves = new List<Curve>();
                            Curve downPin_line1 = Line.CreateBound(rebarDownPin_p1, rebarDownPin_p2) as Curve;
                            myDownPinCurves.Add(downPin_line1);

                            Rebar columnRebarDownPin = Rebar.CreateFromCurvesAndShape(doc
                                , myPinRebarShape
                                , myPinBarTape
                                , myRebarPinHookType
                                , myRebarPinHookType
                                , myColumn
                                , normalStirrup
                                , myDownPinCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);
                            rebarIdCollection.Add(columnRebarDownPin.Id);

                            //Копирование шпильки вверх
                            XYZ pointTopPinInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                            List<ElementId> columnRebarTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointTopPinInstallation) as List<ElementId>;
                            Element columnRebarTopPin = doc.GetElement(columnRebarTopPinIdList.First());

                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                            rebarIdCollection.Add(columnRebarTopPin.Id);

                            //Копирование шпильки последний
                            XYZ pointLastTopPinInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                            List<ElementId> columnRebarLastTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointLastTopPinInstallation) as List<ElementId>;
                            Element columnRebarLastTopPin = doc.GetElement(columnRebarLastTopPinIdList.First());
                            columnRebarLastTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                            rebarIdCollection.Add(columnRebarLastTopPin.Id);
                        }

                        if (numberOfBarsLRFaces == 4)
                        {
                            //Шпилька
                            //Точки для построения кривых стержня шпильки
                            double rebarStandardHookBendDiameter = myPinBarTape.get_Parameter(BuiltInParameter.REBAR_STANDARD_HOOK_BEND_DIAMETER).AsDouble();

                            XYZ rebarDownPin_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - pinRebarDiam, 6)
                                , Math.Round(columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFacesForStirrup * 2 + residueForOffsetForStirrupLR + rebarStandardHookBendDiameter / 2 + pinRebarDiam / 2, 6)
                                , Math.Round(columnOrigin.Z + firstStirrupOffset + stirrupRebarDiam + pinRebarDiam, 6));

                            XYZ rebarDownPin_p2 = new XYZ(Math.Round(columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer + pinRebarDiam, 6)
                                , Math.Round(rebarDownPin_p1.Y, 6)
                                , Math.Round(rebarDownPin_p1.Z, 6));

                            //Кривые шпильки
                            List<Curve> myDownPinCurves = new List<Curve>();
                            Curve downPin_line1 = Line.CreateBound(rebarDownPin_p1, rebarDownPin_p2) as Curve;
                            myDownPinCurves.Add(downPin_line1);

                            Rebar columnRebarDownPin = Rebar.CreateFromCurvesAndShape(doc
                                , myPinRebarShape
                                , myPinBarTape
                                , myRebarPinHookType
                                , myRebarPinHookType
                                , myColumn
                                , normalStirrup
                                , myDownPinCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);
                            rebarIdCollection.Add(columnRebarDownPin.Id);

                            //Копирование шпильки вверх
                            XYZ pointTopPinInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                            List<ElementId> columnRebarTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointTopPinInstallation) as List<ElementId>;
                            Element columnRebarTopPin = doc.GetElement(columnRebarTopPinIdList.First());

                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                            rebarIdCollection.Add(columnRebarTopPin.Id);

                            //Копирование шпильки последний
                            XYZ pointLastTopPinInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                            List<ElementId> columnRebarLastTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointLastTopPinInstallation) as List<ElementId>;
                            Element columnRebarLastTopPin = doc.GetElement(columnRebarLastTopPinIdList.First());
                            columnRebarLastTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                            rebarIdCollection.Add(columnRebarLastTopPin.Id);
                        }

                        if (numberOfBarsLRFaces >=6)
                        {
                            //Точки для построения кривых стержня хомута 1
                            XYZ rebarStirrupFirst_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth/2 + mainRebarCoverLayer - stirrupRebarDiam, 6)
                            , Math.Round(columnOrigin.Y
                            + columnSectionHeight/2
                            - mainRebarCoverLayer
                            - mainRebarDiamTypeOne/2
                            - residueForOffsetForStirrupLR
                            - stepBarsLRFacesForStirrup*2
                            + mainRebarDiamTypeTwo / 2
                            + stirrupRebarDiam / 2, 6)
                            , Math.Round(columnOrigin.Z + firstStirrupOffset + stirrupRebarDiam, 6));

                            XYZ rebarStirrupFirst_p2 = new XYZ(Math.Round(rebarStirrupFirst_p1.X + columnSectionWidth - mainRebarCoverLayer * 2 + stirrupRebarDiam, 6)
                                , Math.Round(rebarStirrupFirst_p1.Y, 6)
                                , Math.Round(rebarStirrupFirst_p1.Z, 6));

                            XYZ rebarStirrupFirst_p3 = new XYZ(Math.Round(rebarStirrupFirst_p2.X, 6)
                                , Math.Round(columnOrigin.Y
                                - columnSectionHeight / 2
                                + mainRebarCoverLayer
                                + mainRebarDiamTypeOne / 2
                                + residueForOffsetForStirrupLR
                                + stepBarsLRFacesForStirrup * 2
                                - mainRebarDiamTypeTwo / 2
                                - stirrupRebarDiam, 6)
                                , Math.Round(rebarStirrupFirst_p2.Z, 6));

                            XYZ rebarStirrupFirst_p4 = new XYZ(Math.Round(rebarStirrupFirst_p3.X - columnSectionWidth + mainRebarCoverLayer * 2 - stirrupRebarDiam, 6)
                                , Math.Round(rebarStirrupFirst_p3.Y, 6)
                                , Math.Round(rebarStirrupFirst_p3.Z, 6));

                            //Кривые хомута 1
                            List<Curve> myStirrupFirstCurves = new List<Curve>();

                            Curve firstStirrup_line1 = Line.CreateBound(rebarStirrupFirst_p1, rebarStirrupFirst_p2) as Curve;
                            myStirrupFirstCurves.Add(firstStirrup_line1);
                            Curve firstStirrup_line2 = Line.CreateBound(rebarStirrupFirst_p2, rebarStirrupFirst_p3) as Curve;
                            myStirrupFirstCurves.Add(firstStirrup_line2);
                            Curve firstStirrup_line3 = Line.CreateBound(rebarStirrupFirst_p3, rebarStirrupFirst_p4) as Curve;
                            myStirrupFirstCurves.Add(firstStirrup_line3);
                            Curve firstStirrup_line4 = Line.CreateBound(rebarStirrupFirst_p4, rebarStirrupFirst_p1) as Curve;
                            myStirrupFirstCurves.Add(firstStirrup_line4);

                            //Построение нижнего хомута 1
                            Rebar columnRebarFirstDownStirrup = Rebar.CreateFromCurvesAndShape(doc
                                , myStirrupRebarShape
                                , myStirrupBarTape
                                , myRebarHookType
                                , myRebarHookType
                                , myColumn
                                , normalStirrup
                                , myStirrupFirstCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            columnRebarFirstDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarFirstDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                            columnRebarFirstDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);

                            rebarIdCollection.Add(columnRebarFirstDownStirrup.Id);

                            //Копирование хомута 1
                            XYZ pointFirstTopStirrupInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                            List<ElementId> columnRebarFirstTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarFirstDownStirrup.Id, pointFirstTopStirrupInstallation) as List<ElementId>;
                            Element columnRebarFirstTopStirrup = doc.GetElement(columnRebarFirstTopStirrupIdList.First());

                            columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                            columnRebarFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                            rebarIdCollection.Add(columnRebarFirstTopStirrup.Id);

                            //Копирование хомута 1 последний
                            XYZ pointLastFirstTopStirrupInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                            List<ElementId> columnRebarLastFirstTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarFirstDownStirrup.Id, pointLastFirstTopStirrupInstallation) as List<ElementId>;
                            Element columnRebarLastFirstTopStirrup = doc.GetElement(columnRebarLastFirstTopStirrupIdList.First());
                            columnRebarLastFirstTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                            rebarIdCollection.Add(columnRebarLastFirstTopStirrup.Id);
                        }

                        if (numberOfBarsLRFaces > 7)
                        {
                            //Шпилька
                            //Точки для построения кривых стержня шпильки
                            double rebarStandardHookBendDiameter = myPinBarTape.get_Parameter(BuiltInParameter.REBAR_STANDARD_HOOK_BEND_DIAMETER).AsDouble();

                            XYZ rebarDownPin_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer - pinRebarDiam, 6)
                                , Math.Round(columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsLRFacesForStirrup * 4 + residueForOffsetForStirrupLR + rebarStandardHookBendDiameter / 2 + pinRebarDiam / 2, 6)
                                , Math.Round(columnOrigin.Z + firstStirrupOffset + stirrupRebarDiam*2 + pinRebarDiam, 6));

                            XYZ rebarDownPin_p2 = new XYZ(Math.Round(columnOrigin.X + columnSectionWidth / 2 - mainRebarCoverLayer + pinRebarDiam, 6)
                                , Math.Round(rebarDownPin_p1.Y, 6)
                                , Math.Round(rebarDownPin_p1.Z, 6));

                            //Кривые шпильки
                            List<Curve> myDownPinCurves = new List<Curve>();
                            Curve downPin_line1 = Line.CreateBound(rebarDownPin_p1, rebarDownPin_p2) as Curve;
                            myDownPinCurves.Add(downPin_line1);

                            Rebar columnRebarDownPin = Rebar.CreateFromCurvesAndShape(doc
                                , myPinRebarShape
                                , myPinBarTape
                                , myRebarPinHookType
                                , myRebarPinHookType
                                , myColumn
                                , normalStirrup
                                , myDownPinCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);
                            rebarIdCollection.Add(columnRebarDownPin.Id);

                            //Копирование шпильки вверх
                            XYZ pointTopPinInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                            List<ElementId> columnRebarTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointTopPinInstallation) as List<ElementId>;
                            Element columnRebarTopPin = doc.GetElement(columnRebarTopPinIdList.First());

                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                            rebarIdCollection.Add(columnRebarTopPin.Id);

                            //Копирование шпильки последний
                            XYZ pointLastTopPinInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                            List<ElementId> columnRebarLastTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointLastTopPinInstallation) as List<ElementId>;
                            Element columnRebarLastTopPin = doc.GetElement(columnRebarLastTopPinIdList.First());
                            columnRebarLastTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                            rebarIdCollection.Add(columnRebarLastTopPin.Id);

                            if (numberOfBarsLRFaces > 9)
                            {
                                int n = (numberOfBarsLRFaces - 8) / 2; // Необходимое кол-во копий шпильки
                                for (int i = 1; i <= n; i++)
                                {
                                    XYZ pointPinInstallation = new XYZ(0, (stepBarsLRFacesForStirrup * 2) * i, 0);
                                    List<ElementId> newColumnRebarDownPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointPinInstallation) as List<ElementId>;
                                    Element newColumnRebarDownPin = doc.GetElement(newColumnRebarDownPinIdList.First());
                                    rebarIdCollection.Add(newColumnRebarDownPin.Id);

                                    List<ElementId> newColumnRebarTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarTopPin.Id, pointPinInstallation) as List<ElementId>;
                                    Element newColumnRebarTopPin = doc.GetElement(newColumnRebarTopPinIdList.First());
                                    rebarIdCollection.Add(newColumnRebarTopPin.Id);

                                    List<ElementId> newColumnRebarLastTopPinList = ElementTransformUtils.CopyElement(doc, columnRebarLastTopPin.Id, pointPinInstallation) as List<ElementId>;
                                    Element newColumnRebarLastTopPin = doc.GetElement(newColumnRebarLastTopPinList.First());
                                    rebarIdCollection.Add(newColumnRebarLastTopPin.Id);
                                }
                            }
                        }

                        if (numberOfBarsTBFaces == 4)
                        {
                            //Шпилька
                            //Точки для построения кривых стержня шпильки
                            double rebarStandardHookBendDiameter = myPinBarTape.get_Parameter(BuiltInParameter.REBAR_STANDARD_HOOK_BEND_DIAMETER).AsDouble();

                            XYZ rebarDownPin_p1 = new XYZ(Math.Round(columnOrigin.X - columnSectionWidth / 2 + mainRebarCoverLayer + mainRebarDiamTypeOne / 2 + stepBarsTBFacesForStirrup * 2 + residueForOffsetForStirrupTB + rebarStandardHookBendDiameter / 2 + pinRebarDiam / 2, 6)
                                , Math.Round(columnOrigin.Y + columnSectionHeight / 2 - mainRebarCoverLayer + pinRebarDiam, 6)
                                , Math.Round(columnOrigin.Z + firstStirrupOffset + stirrupRebarDiam * 2 + pinRebarDiam, 6));

                            XYZ rebarDownPin_p2 = new XYZ(Math.Round(rebarDownPin_p1.X, 6)
                                , Math.Round(columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer - pinRebarDiam, 6)
                                , Math.Round(rebarDownPin_p1.Z, 6));

                            //Кривые шпильки
                            List<Curve> myDownPinCurves = new List<Curve>();
                            Curve downPin_line1 = Line.CreateBound(rebarDownPin_p1, rebarDownPin_p2) as Curve;
                            myDownPinCurves.Add(downPin_line1);

                            Rebar columnRebarDownPin = Rebar.CreateFromCurvesAndShape(doc
                                , myPinRebarShape
                                , myPinBarTape
                                , myRebarPinHookType
                                , myRebarPinHookType
                                , myColumn
                                , normalStirrup
                                , myDownPinCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                            columnRebarDownPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);
                            rebarIdCollection.Add(columnRebarDownPin.Id);

                            //Копирование шпильки вверх
                            XYZ pointTopPinInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                            List<ElementId> columnRebarTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointTopPinInstallation) as List<ElementId>;
                            Element columnRebarTopPin = doc.GetElement(columnRebarTopPinIdList.First());

                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                            columnRebarTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                            rebarIdCollection.Add(columnRebarTopPin.Id);

                            //Копирование шпильки последний
                            XYZ pointLastTopPinInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                            List<ElementId> columnRebarLastTopPinIdList = ElementTransformUtils.CopyElement(doc, columnRebarDownPin.Id, pointLastTopPinInstallation) as List<ElementId>;
                            Element columnRebarLastTopPin = doc.GetElement(columnRebarLastTopPinIdList.First());
                            columnRebarLastTopPin.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                            rebarIdCollection.Add(columnRebarLastTopPin.Id);

                        }

                        if (numberOfBarsTBFaces>=5 & numberOfBarsTBFaces % 2 != 0)
                        {
                            //Точки для построения кривых стержня хомута 2
                            XYZ rebarStirrupSecond_p1 = new XYZ(Math.Round(columnOrigin.X - stepBarsTBFacesForStirrup - mainRebarDiamTypeThree/2 - stirrupRebarDiam, 6)
                                , Math.Round(columnOrigin.Y + columnSectionHeight/2 - mainRebarCoverLayer + stirrupRebarDiam/2, 6)
                                , Math.Round(columnOrigin.Z + firstStirrupOffset + stirrupRebarDiam*2, 6));

                            XYZ rebarStirrupSecond_p2 = new XYZ(Math.Round(columnOrigin.X + stepBarsTBFacesForStirrup + mainRebarDiamTypeThree / 2, 6)
                                , Math.Round(rebarStirrupSecond_p1.Y, 6)
                                , Math.Round(rebarStirrupSecond_p1.Z, 6));

                            XYZ rebarStirrupSecond_p3 = new XYZ(Math.Round(rebarStirrupSecond_p2.X, 6)
                                , Math.Round(columnOrigin.Y - columnSectionHeight / 2 + mainRebarCoverLayer - stirrupRebarDiam, 6)
                                , Math.Round(rebarStirrupSecond_p2.Z, 6));

                            XYZ rebarStirrupSecond_p4 = new XYZ(Math.Round(columnOrigin.X - stepBarsTBFacesForStirrup - mainRebarDiamTypeThree / 2 - stirrupRebarDiam, 6)
                                , Math.Round(rebarStirrupSecond_p3.Y, 6)
                                , Math.Round(rebarStirrupSecond_p3.Z, 6));

                            //Кривые хомута 2
                            List<Curve> myStirrupSecondCurves = new List<Curve>();

                            Curve secondStirrup_line1 = Line.CreateBound(rebarStirrupSecond_p1, rebarStirrupSecond_p2) as Curve;
                            myStirrupSecondCurves.Add(secondStirrup_line1);
                            Curve secondStirrup_line2 = Line.CreateBound(rebarStirrupSecond_p2, rebarStirrupSecond_p3) as Curve;
                            myStirrupSecondCurves.Add(secondStirrup_line2);
                            Curve secondStirrup_line3 = Line.CreateBound(rebarStirrupSecond_p3, rebarStirrupSecond_p4) as Curve;
                            myStirrupSecondCurves.Add(secondStirrup_line3);
                            Curve secondStirrup_line4 = Line.CreateBound(rebarStirrupSecond_p4, rebarStirrupSecond_p1) as Curve;
                            myStirrupSecondCurves.Add(secondStirrup_line4);

                            //Построение нижнего хомута 2
                            Rebar columnRebarSecondDownStirrup = Rebar.CreateFromCurvesAndShape(doc
                                , myStirrupRebarShape
                                , myStirrupBarTape
                                , myRebarHookType
                                , myRebarHookType
                                , myColumn
                                , normalStirrup
                                , myStirrupSecondCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            columnRebarSecondDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarSecondDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(StirrupBarElemFrequentQuantity + 1);
                            columnRebarSecondDownStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(increasedStirrupStep);

                            rebarIdCollection.Add(columnRebarSecondDownStirrup.Id);

                            //Копирование хомута 2
                            XYZ pointSecondTopStirrupInstallation = new XYZ(0, 0, stirrupIncreasedPlacementHeight + standardStirrupStep);
                            List<ElementId> columnRebarSecondTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarSecondDownStirrup.Id, pointSecondTopStirrupInstallation) as List<ElementId>;
                            Element columnRebarSecondTopStirrup = doc.GetElement(columnRebarSecondTopStirrupIdList.First());

                            columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                            columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupBarElemStandardQuantity);
                            columnRebarSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);
                            rebarIdCollection.Add(columnRebarSecondTopStirrup.Id);

                            // Копирование хомута 2 последний
                            XYZ pointLastSecondTopStirrupInstallation = new XYZ(0, 0, lastStirrupInstallationHeigh);
                            List<ElementId> columnRebarLastSecondTopStirrupIdList = ElementTransformUtils.CopyElement(doc, columnRebarSecondDownStirrup.Id, pointLastSecondTopStirrupInstallation) as List<ElementId>;
                            Element columnRebarLastSecondTopStirrup = doc.GetElement(columnRebarLastSecondTopStirrupIdList.First());
                            columnRebarLastSecondTopStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                            rebarIdCollection.Add(columnRebarLastSecondTopStirrup.Id);

                        }
                    }
#endregion
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
                return Result.Succeeded;
        }
        private double RoundUpToFive(double toRound)
        {
            if (toRound % 5 == 0) return toRound;
            return (5 - toRound % 5) + toRound;
        }
    }
}
