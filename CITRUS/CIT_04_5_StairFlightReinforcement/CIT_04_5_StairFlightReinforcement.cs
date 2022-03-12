using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_5_StairFlightReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_5_StairFlightReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение доступа к UI
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

#region Старт блока выбора форм арматурных стержней
            //Выбор формы прямых стержней
            List<RebarShape> rebarStraightShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "01")
                .Cast<RebarShape>()
                .ToList();
            if (rebarStraightShapeList.Count == 0)
            {
                rebarStraightShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_1")
                .Cast<RebarShape>()
                .ToList();
                if (rebarStraightShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 01 или О_1 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape rebarStraightShape = rebarStraightShapeList.First();

            //Выбор формы Г-образных стержней
            List<RebarShape> rebarLShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "11")
                .Cast<RebarShape>()
                .ToList();
            if (rebarLShapeList.Count == 0)
            {
                rebarLShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_11")
                .Cast<RebarShape>()
                .ToList();
                if (rebarLShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 11 или О_11 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape rebarLShape = rebarLShapeList.First();

            //Выбор формы основной арматуры П-образный стержень под углом
            List<RebarShape> rebarUShapeAngleList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "25a")
                .Cast<RebarShape>()
                .ToList();
            if (rebarUShapeAngleList.Count == 0)
            {
                TaskDialog.Show("Revit", "Форма 25a не найдена");
                return Result.Failed;
            }
            RebarShape rebarUShapeAngle = rebarUShapeAngleList.First();

            //Выбор формы Z-образных стержней
            List<RebarShape> rebarZShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "26")
                .Cast<RebarShape>()
                .ToList();
            if (rebarZShapeList.Count == 0)
            {
                rebarZShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_26(α»90)")
                .Cast<RebarShape>()
                .ToList();

                if (rebarZShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 26 или О_26(α»90) не найдена");
                    return Result.Failed;
                }
            }
            RebarShape rebarZShape = rebarZShapeList.First();
#endregion

            //Выбор лестничного марша
            Reference selStairFlightRef = sel.PickObject(ObjectType.Element, "Выберите лестничный марш");
            Element stairFlight = doc.GetElement(selStairFlightRef);

            //Выбор грани для первых двух точек
            Reference selFaceRef = sel.PickObject(ObjectType.Face, "Выберите грань");
            GeometryObject faceAsGeoObject = doc.GetElement(selFaceRef).GetGeometryObjectFromReference(selFaceRef);
            PlanarFace planarFace = faceAsGeoObject as PlanarFace;

            //Задание рабочей плоскости для выбора точек 1 и 2
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Задать рабочую плоскость для точек 1 и 2");

                Plane plane = Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                uiDoc.ActiveView.SketchPlane = sketchPlane;
                uiDoc.ActiveView.ShowActiveWorkPlane();

                t.Commit();
            }

            //Выбор точки 1
            ObjectSnapTypes snapTypes = ObjectSnapTypes.Endpoints | ObjectSnapTypes.Intersections;
            XYZ firstPoint = sel.PickPoint(snapTypes, "Выберите точку 1");
            //Выбор точки 2
            XYZ secondPoint = sel.PickPoint(snapTypes, "Выберите точку 2");

            //Выбор грани для третьей точки
            selFaceRef = sel.PickObject(ObjectType.Face, "Выберите грань");
            faceAsGeoObject = doc.GetElement(selFaceRef).GetGeometryObjectFromReference(selFaceRef);
            planarFace = faceAsGeoObject as PlanarFace;
            //Задание рабочей плоскости для выбора точки 3
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Задать рабочую плоскость для точки 3");

                Plane plane = Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                uiDoc.ActiveView.SketchPlane = sketchPlane;
                uiDoc.ActiveView.ShowActiveWorkPlane();

                t.Commit();
            }
            //Выбор точки 3
            XYZ thirdPoint = sel.PickPoint(snapTypes, "Выберите точку 3");
            
            //Получение основных векторов
            XYZ horisontalСrossVector = (secondPoint - firstPoint).Normalize();
            XYZ horisontalLengthwiseVector = (new XYZ(thirdPoint.X, thirdPoint.Y, 0) - new XYZ(firstPoint.X, firstPoint.Y, 0)).Normalize();
            XYZ mainLengthwiseVector = (thirdPoint - firstPoint).Normalize();

            //Список типов для выбора арматуры ступеней
            List<RebarBarType> stepRebarTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов для выбора арматуры плиты марша
            List<RebarBarType> staircaseRebarTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Вызов формы
            CIT_04_5_StairFlightReinforcementForm stairFlightReinforcementForm = new CIT_04_5_StairFlightReinforcementForm(stepRebarTypeList, staircaseRebarTypeList);
            stairFlightReinforcementForm.ShowDialog();
            if (stairFlightReinforcementForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            //Имена выбранных узлов примыкания
            string checkedBottomConnectionNodeName = stairFlightReinforcementForm.CheckedBottomConnectionNodeName;
            string checkedTopConnectionNodeName = stairFlightReinforcementForm.CheckedTopConnectionNodeName;

            //Имя арматурной сетки
            string firstBarMeshName = stairFlightReinforcementForm.FirstBarMeshName;
            //Имя дополнительной арматурной сетки. Узел Б1
            string additionalBarMeshName_1 = stairFlightReinforcementForm.AdditionalBarMeshName_1;
            //Имя дополнительной арматурной сетки. Узел А2
            string additionalBarMeshName_2 = stairFlightReinforcementForm.AdditionalBarMeshName_2;

            //Получение типа арматуры ступеней
            RebarBarType stepRebarType = stairFlightReinforcementForm.mySelectionStepRebarType;
            //Диаметр стержня арматуры ступеней
            Parameter stepRebarTypeDiamParam = stepRebarType.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double stepRebarDiam = stepRebarTypeDiamParam.AsDouble();

            //Получение типа арматуры плиты марша
            RebarBarType staircaseRebarType = stairFlightReinforcementForm.mySelectionStaircaseRebarType;
            //Диаметр стержня арматуры плиты марша
            Parameter staircaseRebarTypeDiamParam = staircaseRebarType.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double staircaseRebarDiam = staircaseRebarTypeDiamParam.AsDouble();

            //Длина ступени
            double stepLength = stairFlightReinforcementForm.StepLength / 304.8;
            //Высота ступени
            double stepHeight = stairFlightReinforcementForm.StepHeight / 304.8;
            //Гипотенуза длинны и высоты ступени
            double stepsHypotenuse = Math.Sqrt(Math.Pow(stepLength, 2) + Math.Pow(stepHeight, 2));
            //Защитный слой арматуры ступеней
            double stepRebarCoverLayer = stairFlightReinforcementForm.StepRebarCoverLayer/304.8;
            //Шаг стержней ступени
            double stepRebarStep = stairFlightReinforcementForm.StepRebarStep / 304.8;
            //Шаг стержней марша
            double staircaseRebarStep = stairFlightReinforcementForm.StaircaseRebarStep / 304.8;

            //Толщина плиты марша
            double staircaseSlabThickness = stairFlightReinforcementForm.StaircaseSlabThickness / 304.8;
            //Защитный слой плиты марша
            double staircaseCoverLayer = stairFlightReinforcementForm.StairCoverLayer / 304.8;
            //Длина марша
            double staircaseLength = firstPoint.DistanceTo(thirdPoint);
            //Кол-во ступеней
            int stepQuantity = (int)(staircaseLength / stepsHypotenuse);

            //Удлиннение марша сверху
            double topExtensionStaircase = stairFlightReinforcementForm.TopExtensionStaircase / 304.8;
            // Высота удлиннения марша сверху
            double topExtensionHeightStaircase = stairFlightReinforcementForm.TopExtensionHeightStaircase / 304.8;
            // Высота последней ступени марша снизу
            double bottomExtensionHeightStaircaseNodeA1 = stairFlightReinforcementForm.BottomExtensionHeightStaircase / 304.8;
            double bottomExtensionHeightStaircaseNodeA2 = stairFlightReinforcementForm.BottomExtensionHeightStaircaseNodeA2 / 304.8;

            //Расчет длины вертикального участка стержня ступени
            //Угол наклона лестницы к вертикальной оси
            double angle = mainLengthwiseVector.AngleTo(XYZ.BasisZ.Negate()) * (180 / Math.PI);
            //Толщина плиты лестничного марша по вертикали
            double verticalStairThickness = (staircaseSlabThickness / Math.Cos((90 - angle) * (Math.PI / 180)));
            //Толщина защитного слоя плиты лестничного марша по вертикали
            double verticalStairCoverLayer = (staircaseCoverLayer / Math.Cos((90 - angle) * (Math.PI / 180)));
            //Дополнительное смещение
            double verticalAdditionalStepOffset = (stepRebarCoverLayer + stepRebarDiam / 2) * Math.Tan((90 - angle) * (Math.PI / 180));
            //Длина вертикального участка арматуры ступени
            double stepRebarVerticalLength = RoundUpToFive(((stepHeight - stepRebarCoverLayer - stepRebarDiam) + (verticalStairThickness - verticalAdditionalStepOffset - verticalStairCoverLayer))*304.8)/304.8 - stepRebarDiam/2;

            //Расчет длины горизонтального участка стержня ступени
            //Толщина плиты лестничного марша по горизонтали
            double horizontalStairThickness = (staircaseSlabThickness / Math.Sin((90 - angle) * (Math.PI / 180)));
            //Толщина защитного слоя плиты лестничного марша по горизонтали
            double horizontalStairCoverLayer = (staircaseCoverLayer / Math.Sin((90 - angle) * (Math.PI / 180)));
            //Дополнительное смещение
            double horizontalAdditionalStepOffset = (stepRebarCoverLayer + stepRebarDiam / 2) / Math.Tan((90 - angle) * (Math.PI / 180));
            //Длина горизонтального участка арматуры ступени
            double stepRebarHorizontalLength = RoundUpToFive(((stepLength - stepRebarCoverLayer - stepRebarDiam) + (horizontalStairThickness - horizontalStairCoverLayer - horizontalAdditionalStepOffset))*304.8)/304.8 - stepRebarDiam/2;

            // Ширина лестницы
            double staircaseWidth = firstPoint.DistanceTo(secondPoint);
            //Ширина установки арматуры ступеней за вычетом защитных слоев и диаметров арматуры ступеней
            double stepRebarPlacementWidth = (staircaseWidth - stepRebarCoverLayer*2 - stepRebarDiam) * 304.8;
            //Колличество стержней для ступени
            int stepBarsQuantity = (int)(stepRebarPlacementWidth / (stepRebarStep*304.8));
            //Остаток ширины при размещении стержней для ступени
            double remainderStaircaseWidth = (stepRebarPlacementWidth - stepBarsQuantity * (stepRebarStep*304.8)) / 304.8;
         
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Основное армирование ступеней");
                uiDoc.ActiveView.HideActiveWorkPlane();

                //Универсальная коллекция для формирования группы ступени
                ICollection<ElementId> mainStepRebarIdCollection = new List<ElementId>();

                //Армирование ступеней стартовый стержень
                //Точки для построения кривых Г-стержня
                XYZ startMainStepLRebar_p1 = firstPoint
                    + stepsHypotenuse * mainLengthwiseVector
                    - (stepRebarCoverLayer + stepRebarDiam / 2) * XYZ.BasisZ
                    - (stepRebarCoverLayer + stepRebarDiam / 2) * horisontalLengthwiseVector
                    + (stepRebarCoverLayer + stepRebarDiam / 2) * horisontalСrossVector;

                XYZ startMainStepLRebar_p2 = startMainStepLRebar_p1 - stepRebarHorizontalLength * horisontalLengthwiseVector;
                XYZ startMainStepLRebar_p3 = startMainStepLRebar_p1 - stepRebarVerticalLength * XYZ.BasisZ;

                //Точки для построения кривых  прямого стержня
                XYZ startMainStepStraightRebar_p1 = firstPoint
                    + stepsHypotenuse * mainLengthwiseVector
                    - (stepRebarCoverLayer + stepRebarDiam) * XYZ.BasisZ
                    - (stepRebarCoverLayer + stepRebarDiam / 2 + stepRebarDiam) * horisontalLengthwiseVector
                    + (10 / 304.8) * horisontalСrossVector;

                XYZ startMainStepStraightRebar_p2 = startMainStepStraightRebar_p1 + ((staircaseWidth - (20 / 304.8)) * horisontalСrossVector);

                //Кривые Г-стержня
                List<Curve> startMainStepLRebarCurves = new List<Curve>();

                Curve startMainStepLRebar_Line1 = Line.CreateBound(startMainStepLRebar_p2, startMainStepLRebar_p1) as Curve;
                startMainStepLRebarCurves.Add(startMainStepLRebar_Line1);
                Curve startMainStepLRebar_Line2 = Line.CreateBound(startMainStepLRebar_p1, startMainStepLRebar_p3) as Curve;
                startMainStepLRebarCurves.Add(startMainStepLRebar_Line2);

                //Кривые прямого стержня
                List<Curve> startMainStepStraightRebarCurves = new List<Curve>();

                Curve startMainStepStraightRebar_Line1 = Line.CreateBound(startMainStepStraightRebar_p1, startMainStepStraightRebar_p2) as Curve;
                startMainStepStraightRebarCurves.Add(startMainStepStraightRebar_Line1);

                //Г-стержень ступени
                Rebar startMainStepLRebar = Rebar.CreateFromCurvesAndShape(doc,
                rebarLShape,
                stepRebarType,
                null,
                null,
                stairFlight,
                horisontalСrossVector,
                startMainStepLRebarCurves,
                RebarHookOrientation.Right,
                RebarHookOrientation.Right);
                if (startMainStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                {
                    startMainStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(1);
                }
                mainStepRebarIdCollection.Add(startMainStepLRebar.Id);

                List<ElementId> endMainStepLRebarIdList = ElementTransformUtils.CopyElement(doc, startMainStepLRebar.Id, (staircaseWidth - (stepRebarCoverLayer*2 + stepRebarDiam)) * horisontalСrossVector) as List<ElementId>;
                Element endMainStepLRebar = doc.GetElement(endMainStepLRebarIdList.First());
                if (endMainStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                {
                    endMainStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                }
                mainStepRebarIdCollection.Add(endMainStepLRebar.Id);

                List<ElementId> middleMainStepLRebarIdList = ElementTransformUtils.CopyElement(doc, startMainStepLRebar.Id, (remainderStaircaseWidth / 2 + stepRebarStep) * horisontalСrossVector) as List<ElementId>;
                Element middleMainStepLRebar = doc.GetElement(middleMainStepLRebarIdList.First());
                
                middleMainStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                middleMainStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stepBarsQuantity - 1);
                middleMainStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepRebarStep);
                if (middleMainStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                {
                    middleMainStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                }
                mainStepRebarIdCollection.Add(middleMainStepLRebar.Id);

                //Прямой стержень ступени по вертикали
                Rebar startMainStepStraightRebar_1 = Rebar.CreateFromCurvesAndShape(doc,
                rebarStraightShape,
                stepRebarType,
                null,
                null,
                stairFlight,
                XYZ.BasisZ,
                startMainStepStraightRebarCurves,
                RebarHookOrientation.Right,
                RebarHookOrientation.Right);

                ElementTransformUtils.MoveElement(doc, startMainStepStraightRebar_1.Id, (-10 / 304.8) * XYZ.BasisZ);
                startMainStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                startMainStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                startMainStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                if (startMainStepStraightRebar_1.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                {
                    startMainStepStraightRebar_1.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                }
                mainStepRebarIdCollection.Add(startMainStepStraightRebar_1.Id);

                //Прямой стержень ступени по горизонтали
                Rebar startMainStepStraightRebar_2 = Rebar.CreateFromCurvesAndShape(doc,
                rebarStraightShape,
                stepRebarType,
                null,
                null,
                stairFlight,
                horisontalLengthwiseVector,
                startMainStepStraightRebarCurves,
                RebarHookOrientation.Right,
                RebarHookOrientation.Right);

                ElementTransformUtils.MoveElement(doc, startMainStepStraightRebar_2.Id, (-90 / 304.8) * horisontalLengthwiseVector - (stepRebarDiam/2) * XYZ.BasisZ);
                startMainStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                startMainStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                startMainStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                if (startMainStepStraightRebar_2.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                {
                    startMainStepStraightRebar_2.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                }
                mainStepRebarIdCollection.Add(startMainStepStraightRebar_2.Id);

                if (doc.GetElement(mainStepRebarIdCollection.First()).LookupParameter("Мрк.МаркаИзделия") != null)
                {
                    foreach (ElementId barId in mainStepRebarIdCollection)
                    {
                        doc.GetElement(barId).LookupParameter("Мрк.МаркаИзделия").Set(firstBarMeshName);
                    }
                }

                t.Commit();

                t.Start("Копирование групп армирования ступеней");
                ElementId categoryId = doc.GetElement(mainStepRebarIdCollection.First()).Category.Id;

                List<Group> projectGroupList = new FilteredElementCollector(doc).OfClass(typeof(Group)).Cast<Group>().ToList();
                if (projectGroupList.Any(g => g.GroupType.Name == "Сетка " + firstBarMeshName))
                {
                    TaskDialog.Show("Revit", "Группа с имененм Сетка " + firstBarMeshName
                        + " уже существует!\nБыла создана группа с именем Сетка " + firstBarMeshName + "A");
                    firstBarMeshName = firstBarMeshName + "A";
                }

                Group firstStepBarMeshGroup = doc.Create.NewGroup(mainStepRebarIdCollection);
                firstStepBarMeshGroup.GroupType.Name = "Сетка " + firstBarMeshName;
                AssemblyInstance firstStepBarMeshAssemblyInstance = AssemblyInstance.Create(doc, mainStepRebarIdCollection, categoryId) as AssemblyInstance;

                for (int i = 1; i < stepQuantity - 1; i++)
                {
                    ElementTransformUtils.CopyElement(doc, firstStepBarMeshGroup.Id, (stepsHypotenuse * i) * mainLengthwiseVector);
                }
                t.Commit();

                t.Start("Назначение имени сборки");
                firstStepBarMeshAssemblyInstance.AssemblyTypeName = "Сетка " + firstBarMeshName;
                t.Commit();

                if (checkedBottomConnectionNodeName == "radioButton_NodeA1" & checkedTopConnectionNodeName == "radioButton_NodeB1")
                {
                    //Универсальная коллекция для формирования группы дополнительной ступени
                    ICollection<ElementId> additionalStepRebarIdCollection_1 = new List<ElementId>();

                    //Предворительные вычисления
                    //Толщина защитного слоя плиты марша плюс 1,5 диаметра арматуры плиты по вертикали
                    double verticalStairCoverLayerOneAndHalfStaircaseRebarDiam = ((staircaseCoverLayer + staircaseRebarDiam + staircaseRebarDiam/2) / Math.Cos((90 - angle) * (Math.PI / 180)));
                    //Толщина защитного слоя плиты марша плюс 0,5 диаметра арматуры плиты по вертикали
                    double verticalStairCoverLayerAndHalfStaircaseRebarDiam = ((staircaseCoverLayer + staircaseRebarDiam / 2) / Math.Cos((90 - angle) * (Math.PI / 180)));
                    //Рассчет дополнительных смещений для построения основной продольной арматуры
                    double deltaTop_1 = (stepHeight - staircaseCoverLayer - staircaseRebarDiam - staircaseRebarDiam / 2) + verticalStairCoverLayerOneAndHalfStaircaseRebarDiam;
                    double deltaTop_2 = deltaTop_1 / Math.Sin((90 - angle) * (Math.PI / 180));
                    double deltaTop_3 = deltaTop_1 / Math.Tan((90 - angle) * (Math.PI / 180));
                    double deltaTop_4 = topExtensionStaircase + stepLength - deltaTop_3;

                    double deltaTop_5 = (verticalStairThickness - verticalStairCoverLayerAndHalfStaircaseRebarDiam + stepHeight - (topExtensionHeightStaircase - staircaseCoverLayer - staircaseRebarDiam / 2)) / Math.Sin((90 - angle) * (Math.PI / 180));
                    double deltaTop_6 = (verticalStairThickness - verticalStairCoverLayerAndHalfStaircaseRebarDiam + stepHeight - (topExtensionHeightStaircase - staircaseCoverLayer - staircaseRebarDiam / 2)) / Math.Tan((90 - angle) * (Math.PI / 180));
                    double deltaTop_7 = topExtensionStaircase + stepLength - deltaTop_6;

                    double deltaBottom_1 = verticalStairThickness - verticalStairCoverLayerAndHalfStaircaseRebarDiam - (bottomExtensionHeightStaircaseNodeA1 - stepHeight - staircaseCoverLayer - staircaseRebarDiam / 2);
                    double deltaBottom_2 = deltaBottom_1 / Math.Sin((90 - angle) * (Math.PI / 180));
                    double deltaBottom_3 = deltaBottom_1 / Math.Tan((90 - angle) * (Math.PI / 180));

                    //Ширина установки арматуры плиты марша за вычетом защитных слоев и диаметров арматуры ступеней
                    double staircaseRebarPlacementWidth = (staircaseWidth - staircaseCoverLayer * 2 - staircaseRebarDiam) * 304.8;
                    //Колличество стержней в поперечном направлении
                    int staircaseCrossBarsQuantity = (int)(staircaseRebarPlacementWidth / (staircaseRebarStep * 304.8));
                    //Остаток ширины при размещении стержней для ступени
                    double staircaseRemainderWidth = (staircaseRebarPlacementWidth - staircaseCrossBarsQuantity * (staircaseRebarStep * 304.8)) / 304.8;
                    //Колличество стержней в продольном направлении
                    int staircaseLengthwiseBarsQuantity = (int)(staircaseLength / staircaseRebarStep);
                    
                    t.Start("Основное армирование марша");
                    //Армирование плиты лестницы
                    //Точки для построения кривых верхнего продольного стержня
                    XYZ staircaseTopZRebar_p1 = firstPoint
                        - stepHeight * XYZ.BasisZ
                        - verticalStairCoverLayerOneAndHalfStaircaseRebarDiam * XYZ.BasisZ
                        - deltaTop_2 * mainLengthwiseVector
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) *horisontalСrossVector;

                    XYZ staircaseTopZRebar_p2 = staircaseTopZRebar_p1 - deltaTop_4 * horisontalLengthwiseVector;

                    XYZ staircaseTopZRebar_p3 = thirdPoint
                        - staircaseCoverLayer * XYZ.BasisZ
                        - (staircaseRebarDiam + staircaseRebarDiam / 2) * XYZ.BasisZ
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) * horisontalСrossVector
                        - deltaTop_3 * horisontalLengthwiseVector;

                    XYZ staircaseTopZRebar_p4 = thirdPoint
                        - staircaseCoverLayer * XYZ.BasisZ
                        - (staircaseRebarDiam + staircaseRebarDiam / 2) * XYZ.BasisZ
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) * horisontalСrossVector;

                    //Точки для построения кривых нижнего продольного стержня
                    XYZ staircaseBottomZRebar_p1 = firstPoint
                        - stepHeight * XYZ.BasisZ
                        - verticalStairThickness * XYZ.BasisZ
                        + verticalStairCoverLayerAndHalfStaircaseRebarDiam * XYZ.BasisZ
                        - deltaTop_5 * mainLengthwiseVector
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) * horisontalСrossVector;

                    XYZ staircaseBottomZRebar_p2 = staircaseBottomZRebar_p1 - deltaTop_7 * horisontalLengthwiseVector;

                    XYZ staircaseBottomZRebar_p3 = thirdPoint
                        - (bottomExtensionHeightStaircaseNodeA1 - staircaseCoverLayer - staircaseRebarDiam / 2) * XYZ.BasisZ
                        - deltaBottom_3 * horisontalLengthwiseVector
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) * horisontalСrossVector;

                    XYZ staircaseBottomZRebar_p4 = staircaseBottomZRebar_p3 + deltaBottom_3 * horisontalLengthwiseVector;

                    //Кривые верхнего продольного стержня
                    List<Curve> staircaseTopZRebarCurves = new List<Curve>();

                    Curve staircaseTopZRebar_Line1 = Line.CreateBound(staircaseTopZRebar_p4, staircaseTopZRebar_p3) as Curve;
                    staircaseTopZRebarCurves.Add(staircaseTopZRebar_Line1);
                    Curve staircaseTopZRebar_Line2 = Line.CreateBound(staircaseTopZRebar_p3, staircaseTopZRebar_p1) as Curve;
                    staircaseTopZRebarCurves.Add(staircaseTopZRebar_Line2);
                    Curve staircaseTopZRebar_Line3 = Line.CreateBound(staircaseTopZRebar_p1, staircaseTopZRebar_p2) as Curve;
                    staircaseTopZRebarCurves.Add(staircaseTopZRebar_Line3);

                    //Кривые нижнего продольного стержня
                    List<Curve> staircaseBottomZRebarCurves = new List<Curve>();

                    Curve staircaseBottomZRebar_Line1 = Line.CreateBound(staircaseBottomZRebar_p4, staircaseBottomZRebar_p3) as Curve;
                    staircaseBottomZRebarCurves.Add(staircaseBottomZRebar_Line1);
                    Curve staircaseBottomZRebar_Line2 = Line.CreateBound(staircaseBottomZRebar_p3, staircaseBottomZRebar_p1) as Curve;
                    staircaseBottomZRebarCurves.Add(staircaseBottomZRebar_Line2);
                    Curve staircaseBottomZRebar_Line3 = Line.CreateBound(staircaseBottomZRebar_p1, staircaseBottomZRebar_p2) as Curve;
                    staircaseBottomZRebarCurves.Add(staircaseBottomZRebar_Line3);

                    Rebar startStaircaseTopZRebar = Rebar.CreateFromCurvesAndShape(doc,
                        rebarZShape,
                        staircaseRebarType,
                        null,
                        null,
                        stairFlight,
                        horisontalСrossVector,
                        staircaseTopZRebarCurves,
                        RebarHookOrientation.Right,
                        RebarHookOrientation.Right);

                    List<ElementId> endStaircaseTopZRebarIdList = ElementTransformUtils.CopyElement(doc, startStaircaseTopZRebar.Id, (staircaseWidth - (staircaseCoverLayer * 2 + staircaseRebarDiam)) * horisontalСrossVector) as List<ElementId>;
                    Element endStaircaseTopZRebar = doc.GetElement(endStaircaseTopZRebarIdList.First());

                    List<ElementId> middleStaircaseTopZRebarIdList = ElementTransformUtils.CopyElement(doc, startStaircaseTopZRebar.Id, (staircaseRemainderWidth / 2 + staircaseRebarStep) * horisontalСrossVector) as List<ElementId>;
                    Element middleStaircaseTopZRebar = doc.GetElement(middleStaircaseTopZRebarIdList.First());

                    middleStaircaseTopZRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    middleStaircaseTopZRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(staircaseCrossBarsQuantity - 1);
                    middleStaircaseTopZRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(staircaseRebarStep);

                    Rebar startStaircaseBottomZRebar = Rebar.CreateFromCurvesAndShape(doc,
                        rebarZShape,
                        staircaseRebarType,
                        null,
                        null,
                        stairFlight,
                        horisontalСrossVector,
                        staircaseBottomZRebarCurves,
                        RebarHookOrientation.Right,
                        RebarHookOrientation.Right);

                    List<ElementId> endStaircaseBottomZRebarIdList = ElementTransformUtils.CopyElement(doc, startStaircaseBottomZRebar.Id, (staircaseWidth - (staircaseCoverLayer * 2 + staircaseRebarDiam)) * horisontalСrossVector) as List<ElementId>;
                    Element endStaircaseBottomZRebar = doc.GetElement(endStaircaseBottomZRebarIdList.First());

                    List<ElementId> middleStaircaseBottomZRebarIdList = ElementTransformUtils.CopyElement(doc, startStaircaseBottomZRebar.Id, (staircaseRemainderWidth / 2 + staircaseRebarStep) * horisontalСrossVector) as List<ElementId>;
                    Element middleStaircaseBottomZRebar = doc.GetElement(middleStaircaseBottomZRebarIdList.First());

                    middleStaircaseBottomZRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    middleStaircaseBottomZRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(staircaseCrossBarsQuantity - 1);
                    middleStaircaseBottomZRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(staircaseRebarStep);

                    //Точки для построения кривых верхнего поперечного стержня
                    XYZ staircaseTopStraightRebar_p1 = firstPoint
                        - stepHeight * XYZ.BasisZ
                        - verticalStairCoverLayerAndHalfStaircaseRebarDiam * XYZ.BasisZ
                        - (deltaTop_2 - 20/304.8) * mainLengthwiseVector
                        + (10 / 304.8) * horisontalСrossVector;

                    XYZ staircaseTopStraightRebar_p2 = staircaseTopStraightRebar_p1 + ((staircaseWidth - (20 / 304.8)) * horisontalСrossVector);

                    //Кривые верхнего поперечного стержня
                    List<Curve> staircaseTopStraightRebarCurves = new List<Curve>();

                    Curve staircaseTopStraightRebar_Line1 = Line.CreateBound(staircaseTopStraightRebar_p1, staircaseTopStraightRebar_p2) as Curve;
                    staircaseTopStraightRebarCurves.Add(staircaseTopStraightRebar_Line1);

                    //Поперечный стержень
                    Rebar staircaseTopStraightRebar = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    staircaseRebarType,
                    null,
                    null,
                    stairFlight,
                    mainLengthwiseVector,
                    staircaseTopStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    staircaseTopStraightRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    staircaseTopStraightRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(staircaseLengthwiseBarsQuantity + 1);
                    staircaseTopStraightRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(staircaseRebarStep);

                    ElementTransformUtils.CopyElement(doc, staircaseTopStraightRebar.Id, (mainLengthwiseVector.CrossProduct(horisontalСrossVector)).Normalize().Negate() * (staircaseSlabThickness - staircaseCoverLayer * 2 - staircaseRebarDiam * 2));

                    //Дополнительные поперечные стержни в примыканиях
                    //Точки для построения кривых верхнего дополнительного поперечного стержня в примыкании
                    XYZ staircaseAdditionalTopStraightRebar_p1 = firstPoint
                        - (stepLength + topExtensionStaircase - staircaseCoverLayer - staircaseRebarDiam / 2) * horisontalLengthwiseVector 
                        - (staircaseCoverLayer + staircaseRebarDiam / 2) * XYZ.BasisZ
                        + (10 / 304.8) * horisontalСrossVector;

                    XYZ staircaseAdditionalTopStraightRebar_p2 = staircaseAdditionalTopStraightRebar_p1 + ((staircaseWidth - (20 / 304.8)) * horisontalСrossVector);

                    //Точки для построения кривых нижнего дополнительного поперечного стержня в примыкании
                    XYZ staircaseAdditionalBottomStraightRebar_p1 = thirdPoint
                        - (staircaseCoverLayer + staircaseRebarDiam / 2) * horisontalLengthwiseVector
                        - (staircaseCoverLayer + staircaseRebarDiam / 2) * XYZ.BasisZ
                        + (10 / 304.8) * horisontalСrossVector;

                    XYZ staircaseAdditionalBottomStraightRebar_p2 = staircaseAdditionalBottomStraightRebar_p1 + ((staircaseWidth - (20 / 304.8)) * horisontalСrossVector);


                    //Кривые верхнего дополнительного поперечного стержня
                    List<Curve> staircaseAdditionalTopStraightRebarCurves = new List<Curve>();

                    Curve staircaseAdditionalTopStraightRebar_Line1 = Line.CreateBound(staircaseAdditionalTopStraightRebar_p1, staircaseAdditionalTopStraightRebar_p2) as Curve;
                    staircaseAdditionalTopStraightRebarCurves.Add(staircaseAdditionalTopStraightRebar_Line1);

                    //Кривые нижнего дополнительного поперечного стержня
                    List<Curve> staircaseAdditionalBottomStraightRebarCurves = new List<Curve>();

                    Curve staircaseAdditionalBottomStraightRebar_Line1 = Line.CreateBound(staircaseAdditionalBottomStraightRebar_p1, staircaseAdditionalBottomStraightRebar_p2) as Curve;
                    staircaseAdditionalBottomStraightRebarCurves.Add(staircaseAdditionalBottomStraightRebar_Line1);

                    //Дополнительный поперечный стержень в верхнем примыкании
                    Rebar staircaseAdditionalTopStraightRebar_1 = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    staircaseRebarType,
                    null,
                    null,
                    stairFlight,
                    horisontalLengthwiseVector,
                    staircaseAdditionalTopStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    staircaseAdditionalTopStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    staircaseAdditionalTopStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    staircaseAdditionalTopStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(topExtensionStaircase - (staircaseCoverLayer*2 + staircaseRebarDiam));

                    List<ElementId> staircaseAdditionalTopStraightRebar_2IdList = ElementTransformUtils.CopyElement(doc, staircaseAdditionalTopStraightRebar_1.Id, (topExtensionHeightStaircase - staircaseCoverLayer*2 - staircaseRebarDiam *2) * XYZ.BasisZ.Negate()) as List<ElementId>;
                    Element staircaseAdditionalTopStraightRebar_2 = doc.GetElement(staircaseAdditionalTopStraightRebar_2IdList.First());

                    staircaseAdditionalTopStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    staircaseAdditionalTopStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    staircaseAdditionalTopStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(topExtensionStaircase - (staircaseCoverLayer * 2 + staircaseRebarDiam + 50/304.8));

                    //Дополнительный поперечный стержень в нижнем примыкании
                    Rebar staircaseAdditionalBottomStraightRebar_1 = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    staircaseRebarType,
                    null,
                    null,
                    stairFlight,
                    horisontalLengthwiseVector.Negate(),
                    staircaseAdditionalBottomStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    staircaseAdditionalBottomStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    staircaseAdditionalBottomStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    staircaseAdditionalBottomStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepLength - (staircaseCoverLayer * 2 + staircaseRebarDiam));

                    List<ElementId> staircaseAdditionalBottomStraightRebar_2IdList = ElementTransformUtils.CopyElement(doc, staircaseAdditionalBottomStraightRebar_1.Id, (bottomExtensionHeightStaircaseNodeA1 - staircaseCoverLayer*2 - staircaseRebarDiam * 2) * XYZ.BasisZ.Negate()) as List<ElementId>;
                    Element staircaseAdditionalBottomStraightRebar_2 = doc.GetElement(staircaseAdditionalBottomStraightRebar_2IdList.First());

                    //Дополнительное армирование верхней ступени лестницы
                    //Точки для построения кривых Г-стержня дополнительного армирования
                    XYZ additionalStepLRebar_p1 = firstPoint
                        - (stepRebarCoverLayer + stepRebarDiam / 2) * XYZ.BasisZ
                        - (stepRebarCoverLayer + stepRebarDiam / 2) * horisontalLengthwiseVector
                        + (stepRebarCoverLayer + stepRebarDiam / 2) * horisontalСrossVector;

                    XYZ additionalStepLRebar_p2 = additionalStepLRebar_p1 
                        - (stepLength + topExtensionStaircase - staircaseCoverLayer - staircaseRebarDiam / 2) * horisontalLengthwiseVector;

                    XYZ additionalStepLRebar_p3 = additionalStepLRebar_p1 - stepRebarVerticalLength * XYZ.BasisZ;

                    //Точки для построения кривых  прямого стержня дополнительного армирования
                    XYZ additionalStepStraightRebar_p1 = firstPoint
                        - (stepRebarCoverLayer + stepRebarDiam) * XYZ.BasisZ
                        - (stepRebarCoverLayer + stepRebarDiam / 2 + stepRebarDiam) * horisontalLengthwiseVector
                        + (10 / 304.8) * horisontalСrossVector;

                    XYZ additionalStepStraightRebar_p2 = additionalStepStraightRebar_p1 + ((staircaseWidth - (20 / 304.8)) * horisontalСrossVector);

                    //Кривые Г-стержня дополнительного армирования
                    List<Curve> additionalStepLRebarCurves = new List<Curve>();

                    Curve additionalStepLRebar_Line1 = Line.CreateBound(additionalStepLRebar_p2, additionalStepLRebar_p1) as Curve;
                    additionalStepLRebarCurves.Add(additionalStepLRebar_Line1);
                    Curve additionalStepLRebar_Line2 = Line.CreateBound(additionalStepLRebar_p1, additionalStepLRebar_p3) as Curve;
                    additionalStepLRebarCurves.Add(additionalStepLRebar_Line2);

                    //Кривые прямого стержня дополнительного армирования
                    List<Curve> additionalStepStraightRebarCurves = new List<Curve>();

                    Curve additionalStepStraightRebar_Line1 = Line.CreateBound(additionalStepStraightRebar_p1, additionalStepStraightRebar_p2) as Curve;
                    additionalStepStraightRebarCurves.Add(additionalStepStraightRebar_Line1);

                    //Г-стержень ступени дополнительного армирования
                    Rebar startAdditionalStepLRebar = Rebar.CreateFromCurvesAndShape(doc,
                    rebarLShape,
                    stepRebarType,
                    null,
                    null,
                    stairFlight,
                    horisontalСrossVector,
                    additionalStepLRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);
                    if (startAdditionalStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        startAdditionalStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(1);
                    }
                    additionalStepRebarIdCollection_1.Add(startAdditionalStepLRebar.Id);

                    List<ElementId> endAdditionalStepLRebarIdList = ElementTransformUtils.CopyElement(doc, startAdditionalStepLRebar.Id, (staircaseWidth - (stepRebarCoverLayer * 2 + stepRebarDiam)) * horisontalСrossVector) as List<ElementId>;
                    Element endAdditionalStepLRebar = doc.GetElement(endAdditionalStepLRebarIdList.First());
                    if (endAdditionalStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        endAdditionalStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalStepRebarIdCollection_1.Add(endAdditionalStepLRebar.Id);

                    List<ElementId> middleAdditionalStepLRebarIdList = ElementTransformUtils.CopyElement(doc, startAdditionalStepLRebar.Id, (remainderStaircaseWidth / 2 + stepRebarStep) * horisontalСrossVector) as List<ElementId>;
                    Element middleAdditionalStepLRebar = doc.GetElement(middleAdditionalStepLRebarIdList.First());

                    middleAdditionalStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    middleAdditionalStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stepBarsQuantity - 1);
                    middleAdditionalStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepRebarStep);
                    if (middleAdditionalStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        middleAdditionalStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalStepRebarIdCollection_1.Add(middleAdditionalStepLRebar.Id);

                    //Прямой стержень ступени по вертикали
                    Rebar additionalStepStraightRebar_1 = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    stepRebarType,
                    null,
                    null,
                    stairFlight,
                    XYZ.BasisZ,
                    additionalStepStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    ElementTransformUtils.MoveElement(doc, additionalStepStraightRebar_1.Id, (-10 / 304.8) * XYZ.BasisZ);
                    additionalStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    additionalStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    additionalStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                    if (additionalStepStraightRebar_1.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        additionalStepStraightRebar_1.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalStepRebarIdCollection_1.Add(additionalStepStraightRebar_1.Id);

                    //Прямой стержень ступени по горизонтали
                    Rebar additionalStepStraightRebar_2 = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    stepRebarType,
                    null,
                    null,
                    stairFlight,
                    horisontalLengthwiseVector,
                    additionalStepStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    ElementTransformUtils.MoveElement(doc, additionalStepStraightRebar_2.Id, (-90 / 304.8) * horisontalLengthwiseVector - (stepRebarDiam / 2) * XYZ.BasisZ);
                    additionalStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    additionalStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    additionalStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                    if (additionalStepStraightRebar_2.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        additionalStepStraightRebar_2.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalStepRebarIdCollection_1.Add(additionalStepStraightRebar_2.Id);

                    if (doc.GetElement(additionalStepRebarIdCollection_1.First()).LookupParameter("Мрк.МаркаИзделия") != null)
                    {
                        foreach (ElementId barId in additionalStepRebarIdCollection_1)
                        {
                            doc.GetElement(barId).LookupParameter("Мрк.МаркаИзделия").Set(additionalBarMeshName_1);
                        }
                    }

                    ElementId additionalCategoryId_1 = doc.GetElement(additionalStepRebarIdCollection_1.First()).Category.Id;

                    projectGroupList = new FilteredElementCollector(doc).OfClass(typeof(Group)).Cast<Group>().ToList();
                    if (projectGroupList.Any(g => g.GroupType.Name == "Сетка " + additionalBarMeshName_1))
                    {
                        TaskDialog.Show("Revit", "Группа с имененм Сетка " + additionalBarMeshName_1
                            + " уже существует!\nБыла создана группа с именем Сетка " + additionalBarMeshName_1 + "B");
                        additionalBarMeshName_1 = additionalBarMeshName_1 + "B";
                    }
                    Group additionalStepBarMeshGroup_1 = doc.Create.NewGroup(additionalStepRebarIdCollection_1);
                    additionalStepBarMeshGroup_1.GroupType.Name = "Сетка " + additionalBarMeshName_1;
                    AssemblyInstance additionalStepBarMeshAssemblyInstance = AssemblyInstance.Create(doc, additionalStepRebarIdCollection_1, additionalCategoryId_1) as AssemblyInstance;
                    t.Commit();

                    t.Start("Назначение имени сборки");
                    additionalStepBarMeshAssemblyInstance.AssemblyTypeName = "Сетка " + additionalBarMeshName_1;
                    t.Commit();
                }

                if (checkedBottomConnectionNodeName == "radioButton_NodeA2" & checkedTopConnectionNodeName == "radioButton_NodeB1")
                {
                    //Универсальная коллекция для формирования группы дополнительной ступени
                    ICollection<ElementId> additionalTopStepRebarIdCollection_1 = new List<ElementId>();
                    ICollection<ElementId> additionalBottomStepRebarIdCollection_1 = new List<ElementId>();

                    //Предворительные вычисления
                    //Толщина защитного слоя плиты марша плюс 1,5 диаметра арматуры плиты по вертикали
                    double verticalStairCoverLayerOneAndHalfStaircaseRebarDiam = ((staircaseCoverLayer + staircaseRebarDiam + staircaseRebarDiam / 2) / Math.Cos((90 - angle) * (Math.PI / 180)));
                    //Толщина защитного слоя плиты марша плюс 0,5 диаметра арматуры плиты по вертикали
                    double verticalStairCoverLayerAndHalfStaircaseRebarDiam = ((staircaseCoverLayer + staircaseRebarDiam / 2) / Math.Cos((90 - angle) * (Math.PI / 180)));
                    //Рассчет дополнительных смещений для построения основной продольной арматуры
                    double deltaTop_1 = (stepHeight - staircaseCoverLayer - staircaseRebarDiam - staircaseRebarDiam / 2) + verticalStairCoverLayerOneAndHalfStaircaseRebarDiam;
                    double deltaTop_2 = deltaTop_1 / Math.Sin((90 - angle) * (Math.PI / 180));
                    double deltaTop_3 = deltaTop_1 / Math.Tan((90 - angle) * (Math.PI / 180));
                    double deltaTop_4 = topExtensionStaircase + stepLength - deltaTop_3;

                    double deltaTop_5 = (verticalStairThickness - verticalStairCoverLayerAndHalfStaircaseRebarDiam + stepHeight - (topExtensionHeightStaircase - staircaseCoverLayer - staircaseRebarDiam / 2)) / Math.Sin((90 - angle) * (Math.PI / 180));
                    double deltaTop_6 = (verticalStairThickness - verticalStairCoverLayerAndHalfStaircaseRebarDiam + stepHeight - (topExtensionHeightStaircase - staircaseCoverLayer - staircaseRebarDiam / 2)) / Math.Tan((90 - angle) * (Math.PI / 180));
                    double deltaTop_7 = topExtensionStaircase + stepLength - deltaTop_6;

                    double deltaBottom_1 = (staircaseCoverLayer + staircaseRebarDiam + staircaseRebarDiam / 2) * Math.Tan((90 - angle) * (Math.PI / 180));
                    double deltaBottom_2 = (stepLength - staircaseCoverLayer - staircaseRebarDiam / 2) / Math.Cos((90 - angle) * (Math.PI / 180));

                    //Ширина установки арматуры плиты марша за вычетом защитных слоев и диаметров арматуры ступеней
                    double staircaseRebarPlacementWidth = (staircaseWidth - staircaseCoverLayer * 2 - staircaseRebarDiam) * 304.8;
                    //Колличество стержней в поперечном направлении
                    int staircaseCrossBarsQuantity = (int)(staircaseRebarPlacementWidth / (staircaseRebarStep * 304.8));
                    //Остаток ширины при размещении стержней для ступени
                    double staircaseRemainderWidth = (staircaseRebarPlacementWidth - staircaseCrossBarsQuantity * (staircaseRebarStep * 304.8)) / 304.8;
                    //Колличество стержней в продольном направлении
                    int staircaseLengthwiseBarsQuantity = (int)(staircaseLength / staircaseRebarStep);

                    t.Start("Основное армирование марша");
                    //Армирование плиты лестницы
                    //Точки для построения кривых верхнего продольного стержня
                    XYZ staircaseTopURebar_p1 = firstPoint
                        - stepHeight * XYZ.BasisZ
                        - verticalStairCoverLayerOneAndHalfStaircaseRebarDiam * XYZ.BasisZ
                        - deltaTop_2 * mainLengthwiseVector
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) * horisontalСrossVector;

                    XYZ staircaseTopURebar_p2 = staircaseTopURebar_p1 - deltaTop_4 * horisontalLengthwiseVector;

                    XYZ staircaseTopURebar_p3 = thirdPoint
                        - (stepHeight + verticalStairCoverLayerOneAndHalfStaircaseRebarDiam - deltaBottom_1) * XYZ.BasisZ
                        - (staircaseCoverLayer + staircaseRebarDiam + staircaseRebarDiam / 2) * horisontalLengthwiseVector
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) * horisontalСrossVector;

                    XYZ staircaseTopURebar_p4 = thirdPoint
                        - bottomExtensionHeightStaircaseNodeA2 * XYZ.BasisZ
                        - (staircaseCoverLayer + staircaseRebarDiam + staircaseRebarDiam / 2) * horisontalLengthwiseVector
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) * horisontalСrossVector;

                    //Точки для построения кривых нижнего продольного стержня
                    XYZ staircaseBottomURebar_p1 = firstPoint
                        - stepHeight * XYZ.BasisZ
                        - verticalStairThickness * XYZ.BasisZ
                        + verticalStairCoverLayerAndHalfStaircaseRebarDiam * XYZ.BasisZ
                        - deltaTop_5 * mainLengthwiseVector
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) * horisontalСrossVector;

                    XYZ staircaseBottomURebar_p2 = staircaseBottomURebar_p1 - deltaTop_7 * horisontalLengthwiseVector;

                    XYZ staircaseBottomURebar_p3 = thirdPoint
                        - (stepHeight + verticalStairThickness - verticalStairCoverLayerAndHalfStaircaseRebarDiam) * XYZ.BasisZ
                        - deltaBottom_2 * mainLengthwiseVector
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) * horisontalСrossVector;

                    XYZ staircaseBottomURebar_p4 = thirdPoint
                        - bottomExtensionHeightStaircaseNodeA2 * XYZ.BasisZ
                        - (stepLength - (staircaseCoverLayer + staircaseRebarDiam / 2)) * horisontalLengthwiseVector
                        + staircaseCoverLayer * horisontalСrossVector
                        + (staircaseRebarDiam / 2) * horisontalСrossVector;

                    //Кривые верхнего продольного стержня
                    List<Curve> staircaseTopURebarCurves = new List<Curve>();

                    Curve staircaseTopURebar_Line1 = Line.CreateBound(staircaseTopURebar_p4, staircaseTopURebar_p3) as Curve;
                    staircaseTopURebarCurves.Add(staircaseTopURebar_Line1);
                    Curve staircaseTopURebar_Line2 = Line.CreateBound(staircaseTopURebar_p3, staircaseTopURebar_p1) as Curve;
                    staircaseTopURebarCurves.Add(staircaseTopURebar_Line2);
                    Curve staircaseTopURebar_Line3 = Line.CreateBound(staircaseTopURebar_p1, staircaseTopURebar_p2) as Curve;
                    staircaseTopURebarCurves.Add(staircaseTopURebar_Line3);

                    //Кривые нижнего продольного стержня
                    List<Curve> staircaseBottomURebarCurves = new List<Curve>();

                    Curve staircaseBottomURebar_Line1 = Line.CreateBound(staircaseBottomURebar_p4, staircaseBottomURebar_p3) as Curve;
                    staircaseBottomURebarCurves.Add(staircaseBottomURebar_Line1);
                    Curve staircaseBottomURebar_Line2 = Line.CreateBound(staircaseBottomURebar_p3, staircaseBottomURebar_p1) as Curve;
                    staircaseBottomURebarCurves.Add(staircaseBottomURebar_Line2);
                    Curve staircaseBottomURebar_Line3 = Line.CreateBound(staircaseBottomURebar_p1, staircaseBottomURebar_p2) as Curve;
                    staircaseBottomURebarCurves.Add(staircaseBottomURebar_Line3);

                    Rebar startStaircaseTopURebar = Rebar.CreateFromCurvesAndShape(doc,
                        rebarUShapeAngle,
                        staircaseRebarType,
                        null,
                        null,
                        stairFlight,
                        horisontalСrossVector,
                        staircaseTopURebarCurves,
                        RebarHookOrientation.Right,
                        RebarHookOrientation.Right);

                    List<ElementId> endStaircaseTopURebarIdList = ElementTransformUtils.CopyElement(doc, startStaircaseTopURebar.Id, (staircaseWidth - (staircaseCoverLayer * 2 + staircaseRebarDiam)) * horisontalСrossVector) as List<ElementId>;
                    Element endStaircaseTopURebar = doc.GetElement(endStaircaseTopURebarIdList.First());

                    List<ElementId> middleStaircaseTopURebarIdList = ElementTransformUtils.CopyElement(doc, startStaircaseTopURebar.Id, (staircaseRemainderWidth / 2 + staircaseRebarStep) * horisontalСrossVector) as List<ElementId>;
                    Element middleStaircaseTopURebar = doc.GetElement(middleStaircaseTopURebarIdList.First());

                    middleStaircaseTopURebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    middleStaircaseTopURebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(staircaseCrossBarsQuantity - 1);
                    middleStaircaseTopURebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(staircaseRebarStep);

                    Rebar startStaircaseBottomURebar = Rebar.CreateFromCurvesAndShape(doc,
                        rebarUShapeAngle,
                        staircaseRebarType,
                        null,
                        null,
                        stairFlight,
                        horisontalСrossVector,
                        staircaseBottomURebarCurves,
                        RebarHookOrientation.Right,
                        RebarHookOrientation.Right);

                    List<ElementId> endStaircaseBottomURebarIdList = ElementTransformUtils.CopyElement(doc, startStaircaseBottomURebar.Id, (staircaseWidth - (staircaseCoverLayer * 2 + staircaseRebarDiam)) * horisontalСrossVector) as List<ElementId>;
                    Element endStaircaseBottomURebar = doc.GetElement(endStaircaseBottomURebarIdList.First());

                    List<ElementId> middleStaircaseBottomURebarIdList = ElementTransformUtils.CopyElement(doc, startStaircaseBottomURebar.Id, (staircaseRemainderWidth / 2 + staircaseRebarStep) * horisontalСrossVector) as List<ElementId>;
                    Element middleStaircaseBottomURebar = doc.GetElement(middleStaircaseBottomURebarIdList.First());

                    middleStaircaseBottomURebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    middleStaircaseBottomURebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(staircaseCrossBarsQuantity - 1);
                    middleStaircaseBottomURebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(staircaseRebarStep);

                    //Точки для построения кривых верхнего поперечного стержня
                    XYZ staircaseTopStraightRebar_p1 = firstPoint
                        - stepHeight * XYZ.BasisZ
                        - verticalStairCoverLayerAndHalfStaircaseRebarDiam * XYZ.BasisZ
                        - (deltaTop_2 - 20 / 304.8) * mainLengthwiseVector
                        + (10 / 304.8) * horisontalСrossVector;

                    XYZ staircaseTopStraightRebar_p2 = staircaseTopStraightRebar_p1 + ((staircaseWidth - (20 / 304.8)) * horisontalСrossVector);

                    //Кривые верхнего поперечного стержня
                    List<Curve> staircaseTopStraightRebarCurves = new List<Curve>();

                    Curve staircaseTopStraightRebar_Line1 = Line.CreateBound(staircaseTopStraightRebar_p1, staircaseTopStraightRebar_p2) as Curve;
                    staircaseTopStraightRebarCurves.Add(staircaseTopStraightRebar_Line1);

                    //Поперечный стержень
                    Rebar staircaseTopStraightRebar = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    staircaseRebarType,
                    null,
                    null,
                    stairFlight,
                    mainLengthwiseVector,
                    staircaseTopStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    staircaseTopStraightRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    staircaseTopStraightRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(staircaseLengthwiseBarsQuantity + 3);
                    staircaseTopStraightRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(staircaseRebarStep);

                    List<ElementId> staircaseBottomStraightRebarIdList = ElementTransformUtils.CopyElement(doc, staircaseTopStraightRebar.Id, (mainLengthwiseVector.CrossProduct(horisontalСrossVector)).Normalize().Negate() * (staircaseSlabThickness - staircaseCoverLayer * 2 - staircaseRebarDiam * 2)) as List<ElementId>;
                    Element staircaseBottomStraightRebar = doc.GetElement(staircaseBottomStraightRebarIdList.First());

                    staircaseBottomStraightRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    staircaseBottomStraightRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(staircaseLengthwiseBarsQuantity + 2);
                    staircaseBottomStraightRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(staircaseRebarStep);

                    //Дополнительные поперечные стержни в примыканиях
                    //Точки для построения кривых верхнего дополнительного поперечного стержня в примыкании
                    XYZ staircaseAdditionalTopStraightRebar_p1 = firstPoint
                        - (stepLength + topExtensionStaircase - staircaseCoverLayer - staircaseRebarDiam / 2) * horisontalLengthwiseVector
                        - (staircaseCoverLayer + staircaseRebarDiam / 2) * XYZ.BasisZ
                        + (10 / 304.8) * horisontalСrossVector;

                    XYZ staircaseAdditionalTopStraightRebar_p2 = staircaseAdditionalTopStraightRebar_p1 + ((staircaseWidth - (20 / 304.8)) * horisontalСrossVector);

                    //Кривые верхнего дополнительного поперечного стержня
                    List<Curve> staircaseAdditionalTopStraightRebarCurves = new List<Curve>();

                    Curve staircaseAdditionalTopStraightRebar_Line1 = Line.CreateBound(staircaseAdditionalTopStraightRebar_p1, staircaseAdditionalTopStraightRebar_p2) as Curve;
                    staircaseAdditionalTopStraightRebarCurves.Add(staircaseAdditionalTopStraightRebar_Line1);

                    //Дополнительный поперечный стержень в верхнем примыкании
                    Rebar staircaseAdditionalTopStraightRebar_1 = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    staircaseRebarType,
                    null,
                    null,
                    stairFlight,
                    horisontalLengthwiseVector,
                    staircaseAdditionalTopStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    staircaseAdditionalTopStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    staircaseAdditionalTopStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    staircaseAdditionalTopStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(topExtensionStaircase - (staircaseCoverLayer * 2 + staircaseRebarDiam));

                    List<ElementId> staircaseAdditionalTopStraightRebar_2IdList = ElementTransformUtils.CopyElement(doc, staircaseAdditionalTopStraightRebar_1.Id, (topExtensionHeightStaircase - staircaseCoverLayer * 2 - staircaseRebarDiam * 2) * XYZ.BasisZ.Negate()) as List<ElementId>;
                    Element staircaseAdditionalTopStraightRebar_2 = doc.GetElement(staircaseAdditionalTopStraightRebar_2IdList.First());

                    staircaseAdditionalTopStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    staircaseAdditionalTopStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    staircaseAdditionalTopStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(topExtensionStaircase - (staircaseCoverLayer * 2 + staircaseRebarDiam + 50 / 304.8));

                    //Дополнительное армирование верхней ступени лестницы
                    //Точки для построения кривых Г-стержня дополнительного армирования
                    XYZ additionalTopStepLRebar_p1 = firstPoint
                        - (stepRebarCoverLayer + stepRebarDiam / 2) * XYZ.BasisZ
                        - (stepRebarCoverLayer + stepRebarDiam / 2) * horisontalLengthwiseVector
                        + (stepRebarCoverLayer + stepRebarDiam / 2) * horisontalСrossVector;

                    XYZ additionalTopStepLRebar_p2 = additionalTopStepLRebar_p1
                        - (stepLength + topExtensionStaircase - staircaseCoverLayer - staircaseRebarDiam / 2) * horisontalLengthwiseVector;

                    XYZ additionalTopStepLRebar_p3 = additionalTopStepLRebar_p1 - stepRebarVerticalLength * XYZ.BasisZ;

                    //Точки для построения кривых  прямого стержня дополнительного армирования
                    XYZ additionalTopStepStraightRebar_p1 = firstPoint
                        - (stepRebarCoverLayer + stepRebarDiam) * XYZ.BasisZ
                        - (stepRebarCoverLayer + stepRebarDiam / 2 + stepRebarDiam) * horisontalLengthwiseVector
                        + (10 / 304.8) * horisontalСrossVector;

                    XYZ additionalTopStepStraightRebar_p2 = additionalTopStepStraightRebar_p1 + ((staircaseWidth - (20 / 304.8)) * horisontalСrossVector);

                    //Кривые Г-стержня дополнительного армирования
                    List<Curve> additionalTopStepLRebarCurves = new List<Curve>();

                    Curve additionalTopStepLRebar_Line1 = Line.CreateBound(additionalTopStepLRebar_p2, additionalTopStepLRebar_p1) as Curve;
                    additionalTopStepLRebarCurves.Add(additionalTopStepLRebar_Line1);
                    Curve additionalTopStepLRebar_Line2 = Line.CreateBound(additionalTopStepLRebar_p1, additionalTopStepLRebar_p3) as Curve;
                    additionalTopStepLRebarCurves.Add(additionalTopStepLRebar_Line2);

                    //Кривые прямого стержня дополнительного армирования
                    List<Curve> additionalTopStepStraightRebarCurves = new List<Curve>();

                    Curve additionalTopStepStraightRebar_Line1 = Line.CreateBound(additionalTopStepStraightRebar_p1, additionalTopStepStraightRebar_p2) as Curve;
                    additionalTopStepStraightRebarCurves.Add(additionalTopStepStraightRebar_Line1);

                    //Г-стержень ступени дополнительного армирования
                    Rebar startAdditionalTopStepLRebar = Rebar.CreateFromCurvesAndShape(doc,
                    rebarLShape,
                    stepRebarType,
                    null,
                    null,
                    stairFlight,
                    horisontalСrossVector,
                    additionalTopStepLRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);
                    if (startAdditionalTopStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        startAdditionalTopStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(1);
                    }
                    additionalTopStepRebarIdCollection_1.Add(startAdditionalTopStepLRebar.Id);

                    List<ElementId> endAdditionalTopStepLRebarIdList = ElementTransformUtils.CopyElement(doc, startAdditionalTopStepLRebar.Id, (staircaseWidth - (stepRebarCoverLayer * 2 + stepRebarDiam)) * horisontalСrossVector) as List<ElementId>;
                    Element endAdditionalTopStepLRebar = doc.GetElement(endAdditionalTopStepLRebarIdList.First());
                    if (endAdditionalTopStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        endAdditionalTopStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalTopStepRebarIdCollection_1.Add(endAdditionalTopStepLRebar.Id);

                    List<ElementId> middleAdditionalTopStepLRebarIdList = ElementTransformUtils.CopyElement(doc, startAdditionalTopStepLRebar.Id, (remainderStaircaseWidth / 2 + stepRebarStep) * horisontalСrossVector) as List<ElementId>;
                    Element middleAdditionalTopStepLRebar = doc.GetElement(middleAdditionalTopStepLRebarIdList.First());

                    middleAdditionalTopStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    middleAdditionalTopStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stepBarsQuantity - 1);
                    middleAdditionalTopStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepRebarStep);
                    if (middleAdditionalTopStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        middleAdditionalTopStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalTopStepRebarIdCollection_1.Add(middleAdditionalTopStepLRebar.Id);

                    //Прямой стержень ступени по вертикали
                    Rebar additionalTopStepStraightRebar_1 = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    stepRebarType,
                    null,
                    null,
                    stairFlight,
                    XYZ.BasisZ,
                    additionalTopStepStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    ElementTransformUtils.MoveElement(doc, additionalTopStepStraightRebar_1.Id, (-10 / 304.8) * XYZ.BasisZ);
                    additionalTopStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    additionalTopStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    additionalTopStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                    if (additionalTopStepStraightRebar_1.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        additionalTopStepStraightRebar_1.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalTopStepRebarIdCollection_1.Add(additionalTopStepStraightRebar_1.Id);

                    //Прямой стержень ступени по горизонтали
                    Rebar additionalTopStepStraightRebar_2 = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    stepRebarType,
                    null,
                    null,
                    stairFlight,
                    horisontalLengthwiseVector,
                    additionalTopStepStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    ElementTransformUtils.MoveElement(doc, additionalTopStepStraightRebar_2.Id, (-90 / 304.8) * horisontalLengthwiseVector - (stepRebarDiam / 2) * XYZ.BasisZ);
                    additionalTopStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    additionalTopStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    additionalTopStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                    if (additionalTopStepStraightRebar_2.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        additionalTopStepStraightRebar_2.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalTopStepRebarIdCollection_1.Add(additionalTopStepStraightRebar_2.Id);

                    if (doc.GetElement(additionalTopStepRebarIdCollection_1.First()).LookupParameter("Мрк.МаркаИзделия") != null)
                    {
                        foreach (ElementId barId in additionalTopStepRebarIdCollection_1)
                        {
                            doc.GetElement(barId).LookupParameter("Мрк.МаркаИзделия").Set(additionalBarMeshName_1);
                        }
                    }

                    ElementId additionalCategoryId_1 = doc.GetElement(additionalTopStepRebarIdCollection_1.First()).Category.Id;

                    projectGroupList = new FilteredElementCollector(doc).OfClass(typeof(Group)).Cast<Group>().ToList();
                    if (projectGroupList.Any(g => g.GroupType.Name == "Сетка " + additionalBarMeshName_1))
                    {
                        TaskDialog.Show("Revit", "Группа с имененм Сетка " + additionalBarMeshName_1
                            + " уже существует!\nБыла создана группа с именем Сетка " + additionalBarMeshName_1 + "B");
                        additionalBarMeshName_1 = additionalBarMeshName_1 + "B";
                    }
                    Group additionalTopStepBarMeshGroup_1 = doc.Create.NewGroup(additionalTopStepRebarIdCollection_1);
                    additionalTopStepBarMeshGroup_1.GroupType.Name = "Сетка " + additionalBarMeshName_1;
                    AssemblyInstance additionalTopStepBarMeshAssemblyInstance = AssemblyInstance.Create(doc, additionalTopStepRebarIdCollection_1, additionalCategoryId_1) as AssemblyInstance;
                    t.Commit();

                    t.Start("Назначение имени сборки");
                    additionalTopStepBarMeshAssemblyInstance.AssemblyTypeName = "Сетка " + additionalBarMeshName_1;
                    t.Commit();

                    t.Start("Армирование нижней ступени");
                    //Точки для построения кривых нижнего дополнительного поперечного стержня в примыкании
                    XYZ staircaseAdditionalBottomStraightRebar_p1 = thirdPoint
                        - (staircaseCoverLayer + staircaseRebarDiam / 2) * horisontalLengthwiseVector
                        - (bottomExtensionHeightStaircaseNodeA2 - (staircaseCoverLayer + staircaseRebarDiam / 2)) * XYZ.BasisZ
                        + (10 / 304.8) * horisontalСrossVector;

                    XYZ staircaseAdditionalBottomStraightRebar_p2 = staircaseAdditionalBottomStraightRebar_p1 + ((staircaseWidth - (20 / 304.8)) * horisontalСrossVector);

                    //Кривые нижнего дополнительного поперечного стержня
                    List<Curve> staircaseAdditionalBottomStraightRebarCurves = new List<Curve>();

                    Curve staircaseAdditionalBottomStraightRebar_Line1 = Line.CreateBound(staircaseAdditionalBottomStraightRebar_p1, staircaseAdditionalBottomStraightRebar_p2) as Curve;
                    staircaseAdditionalBottomStraightRebarCurves.Add(staircaseAdditionalBottomStraightRebar_Line1);


                    //Дополнительный поперечный стержень в нижнем примыкании
                    Rebar staircaseAdditionalBottomStraightRebar_1 = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    staircaseRebarType,
                    null,
                    null,
                    stairFlight,
                    XYZ.BasisZ,
                    staircaseAdditionalBottomStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    staircaseAdditionalBottomStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    staircaseAdditionalBottomStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    staircaseAdditionalBottomStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(bottomExtensionHeightStaircaseNodeA2 - (stepHeight + staircaseCoverLayer * 2 + staircaseRebarDiam));

                    List<ElementId> staircaseAdditionalBottomStraightRebar_2IdList = ElementTransformUtils.CopyElement(doc, staircaseAdditionalBottomStraightRebar_1.Id,  (stepLength - (staircaseCoverLayer * 2 + staircaseRebarDiam *2)) * horisontalLengthwiseVector.Negate()) as List<ElementId>;
                    Element staircaseAdditionalBottomStraightRebar_2 = doc.GetElement(staircaseAdditionalBottomStraightRebar_2IdList.First());


                    //Дополнительное армирование нижней ступени лестницы
                    //Точки для построения кривых Г-стержня дополнительного армирования
                    XYZ additionalBottomStepLRebar_p1 = thirdPoint
                        - (stepRebarCoverLayer + stepRebarDiam / 2) * XYZ.BasisZ
                        - (stepRebarCoverLayer + stepRebarDiam / 2) * horisontalLengthwiseVector
                        + (stepRebarCoverLayer + stepRebarDiam / 2) * horisontalСrossVector;

                    XYZ additionalBottomStepLRebar_p2 = additionalBottomStepLRebar_p1
                        - stepRebarHorizontalLength * horisontalLengthwiseVector;

                    XYZ additionalBottomStepLRebar_p3 = additionalBottomStepLRebar_p1 - (stepHeight - stepRebarDiam / 2) * XYZ.BasisZ;

                    //Точки для построения кривых  прямого стержня дополнительного армирования
                    XYZ additionalBottomStepStraightRebar_p1 = thirdPoint
                        - (stepRebarCoverLayer + stepRebarDiam) * XYZ.BasisZ
                        - (stepRebarCoverLayer + stepRebarDiam / 2 + stepRebarDiam) * horisontalLengthwiseVector
                        + (10 / 304.8) * horisontalСrossVector;

                    XYZ additionalBottomStepStraightRebar_p2 = additionalBottomStepStraightRebar_p1 + ((staircaseWidth - (20 / 304.8)) * horisontalСrossVector);

                    //Кривые Г-стержня дополнительного армирования
                    List<Curve> additionalBottomStepLRebarCurves = new List<Curve>();

                    Curve additionalBottomStepLRebar_Line1 = Line.CreateBound(additionalBottomStepLRebar_p2, additionalBottomStepLRebar_p1) as Curve;
                    additionalBottomStepLRebarCurves.Add(additionalBottomStepLRebar_Line1);
                    Curve additionalBottomStepLRebar_Line2 = Line.CreateBound(additionalBottomStepLRebar_p1, additionalBottomStepLRebar_p3) as Curve;
                    additionalBottomStepLRebarCurves.Add(additionalBottomStepLRebar_Line2);

                    //Кривые прямого стержня дополнительного армирования
                    List<Curve> additionalBottomStepStraightRebarCurves = new List<Curve>();

                    Curve additionalBottomStepStraightRebar_Line1 = Line.CreateBound(additionalBottomStepStraightRebar_p1, additionalBottomStepStraightRebar_p2) as Curve;
                    additionalBottomStepStraightRebarCurves.Add(additionalBottomStepStraightRebar_Line1);

                    //Г-стержень ступени дополнительного армирования
                    Rebar startAdditionalBottomStepLRebar = Rebar.CreateFromCurvesAndShape(doc,
                    rebarLShape,
                    stepRebarType,
                    null,
                    null,
                    stairFlight,
                    horisontalСrossVector,
                    additionalBottomStepLRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);
                    if (startAdditionalBottomStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        startAdditionalBottomStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(1);
                    }
                    additionalBottomStepRebarIdCollection_1.Add(startAdditionalBottomStepLRebar.Id);

                    List<ElementId> endAdditionalBottomStepLRebarIdList = ElementTransformUtils.CopyElement(doc, startAdditionalBottomStepLRebar.Id, (staircaseWidth - (stepRebarCoverLayer * 2 + stepRebarDiam)) * horisontalСrossVector) as List<ElementId>;
                    Element endAdditionalBottomStepLRebar = doc.GetElement(endAdditionalBottomStepLRebarIdList.First());
                    if (endAdditionalBottomStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        endAdditionalBottomStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalBottomStepRebarIdCollection_1.Add(endAdditionalBottomStepLRebar.Id);

                    List<ElementId> middleAdditionalBottomStepLRebarIdList = ElementTransformUtils.CopyElement(doc, startAdditionalBottomStepLRebar.Id, (remainderStaircaseWidth / 2 + stepRebarStep) * horisontalСrossVector) as List<ElementId>;
                    Element middleAdditionalBottomStepLRebar = doc.GetElement(middleAdditionalBottomStepLRebarIdList.First());

                    middleAdditionalBottomStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    middleAdditionalBottomStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stepBarsQuantity - 1);
                    middleAdditionalBottomStepLRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stepRebarStep);
                    if (middleAdditionalBottomStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        middleAdditionalBottomStepLRebar.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalBottomStepRebarIdCollection_1.Add(middleAdditionalBottomStepLRebar.Id);

                    //Прямой стержень ступени по вертикали
                    Rebar additionalBottomStepStraightRebar_1 = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    stepRebarType,
                    null,
                    null,
                    stairFlight,
                    XYZ.BasisZ,
                    additionalBottomStepStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    ElementTransformUtils.MoveElement(doc, additionalBottomStepStraightRebar_1.Id, (-10 / 304.8) * XYZ.BasisZ);
                    additionalBottomStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    additionalBottomStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    additionalBottomStepStraightRebar_1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                    if (additionalBottomStepStraightRebar_1.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        additionalBottomStepStraightRebar_1.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalBottomStepRebarIdCollection_1.Add(additionalBottomStepStraightRebar_1.Id);

                    //Прямой стержень ступени по горизонтали
                    Rebar additionalBottomStepStraightRebar_2 = Rebar.CreateFromCurvesAndShape(doc,
                    rebarStraightShape,
                    stepRebarType,
                    null,
                    null,
                    stairFlight,
                    horisontalLengthwiseVector,
                    additionalBottomStepStraightRebarCurves,
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right);

                    ElementTransformUtils.MoveElement(doc, additionalBottomStepStraightRebar_2.Id, (-90 / 304.8) * horisontalLengthwiseVector - (stepRebarDiam / 2) * XYZ.BasisZ);
                    additionalBottomStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    additionalBottomStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(2);
                    additionalBottomStepStraightRebar_2.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(100 / 304.8);
                    if (additionalBottomStepStraightRebar_2.LookupParameter("Орг.ГлавнаяДетальСборки") != null)
                    {
                        additionalBottomStepStraightRebar_2.LookupParameter("Орг.ГлавнаяДетальСборки").Set(0);
                    }
                    additionalBottomStepRebarIdCollection_1.Add(additionalBottomStepStraightRebar_2.Id);


                    if (doc.GetElement(additionalBottomStepRebarIdCollection_1.First()).LookupParameter("Мрк.МаркаИзделия") != null)
                    {
                        foreach (ElementId barId in additionalBottomStepRebarIdCollection_1)
                        {
                            doc.GetElement(barId).LookupParameter("Мрк.МаркаИзделия").Set(additionalBarMeshName_2);
                        }
                    }

                    ElementId additionalCategoryId_2 = doc.GetElement(additionalBottomStepRebarIdCollection_1.First()).Category.Id;

                    projectGroupList = new FilteredElementCollector(doc).OfClass(typeof(Group)).Cast<Group>().ToList();
                    if (projectGroupList.Any(g => g.GroupType.Name == "Сетка " + additionalBarMeshName_2))
                    {
                        TaskDialog.Show("Revit", "Группа с имененм Сетка " + additionalBarMeshName_2
                            + " уже существует!\nБыла создана группа с именем Сетка " + additionalBarMeshName_2 + "C");
                        additionalBarMeshName_2 = additionalBarMeshName_2 + "C";
                    }
                    Group additionalBottomStepBarMeshGroup_1 = doc.Create.NewGroup(additionalBottomStepRebarIdCollection_1);
                    additionalBottomStepBarMeshGroup_1.GroupType.Name = "Сетка " + additionalBarMeshName_2;
                    AssemblyInstance additionalBottomStepBarMeshAssemblyInstance = AssemblyInstance.Create(doc, additionalBottomStepRebarIdCollection_1, additionalCategoryId_2) as AssemblyInstance;

                    t.Commit();

                    t.Start("Назначение имени сборки");
                    additionalBottomStepBarMeshAssemblyInstance.AssemblyTypeName = "Сетка " + additionalBarMeshName_2;
                    t.Commit();
                }

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
