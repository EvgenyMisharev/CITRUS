using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_3_BeamReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_3_BeamReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

#region Формы стержней
            //Выбор формы основной арматуры прямой стержень
            List<RebarShape> straightBarShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "01")
                .Cast<RebarShape>()
                .ToList();
            if (straightBarShapeList.Count == 0)
            {
                straightBarShapeList = new FilteredElementCollector(doc)
               .OfClass(typeof(RebarShape))
               .Where(rs => rs.Name.ToString() == "О_1")
               .Cast<RebarShape>()
               .ToList();
                if (straightBarShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 01 или О_1 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape straightBarShape = straightBarShapeList.First();

            //Выбор формы основной арматуры Г-образный стержень
            List<RebarShape> LBarShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "11")
                .Cast<RebarShape>()
                .ToList();
            if (LBarShapeList.Count == 0)
            {
                LBarShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_11")
                .Cast<RebarShape>()
                .ToList();
                if (LBarShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 11 или О_11 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape LBarShape = LBarShapeList.First();

            //Выбор формы основной арматуры Г-образный стержень под углом 
            List<RebarShape> LBarShapeAngleList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "15")
                .Cast<RebarShape>()
                .ToList();
            if (LBarShapeAngleList.Count == 0)
            {
                LBarShapeAngleList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_15")
                .Cast<RebarShape>()
                .ToList();
                if (LBarShapeAngleList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 15 или О_15 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape LBarShapeAngle = LBarShapeAngleList.First();

            //Выбор формы основной арматуры Г-образный стержень под острым углом
            List<RebarShape> LBarShapeSharpAngleList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "15a")
                .Cast<RebarShape>()
                .ToList();
            if (LBarShapeSharpAngleList.Count == 0)
            {
                LBarShapeSharpAngleList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_14")
                .Cast<RebarShape>()
                .ToList();
                if (LBarShapeSharpAngleList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 15a или О_14 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape LBarShapeSharpAngle = LBarShapeSharpAngleList.First();


            //Выбор формы основной арматуры П-образный стержень 
            List<RebarShape> UBarShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "21 отгибы разной длины")
                .Cast<RebarShape>()
                .ToList();
            if (UBarShapeList.Count == 0)
            {
                UBarShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_21")
                .Cast<RebarShape>()
                .ToList();
                if (UBarShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 21 отгибы разной длины или О_21 не найдена");
                    return Result.Failed;
                }
            }
            RebarShape UBarShape = UBarShapeList.First();

            //Выбор формы основной арматуры П-образный стержень под углом
            List<RebarShape> UBarShapeAngleList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "25a")
                .Cast<RebarShape>()
                .ToList();
            if (UBarShapeAngleList.Count == 0)
            {
                TaskDialog.Show("Revit", "Форма 25a не найдена");
                return Result.Failed;
            }
            RebarShape UBarShapeAngle = UBarShapeAngleList.First();

            //Выбор формы основной арматуры П-образный стержень под острым углом
            List<RebarShape> UBarShapeSharpAngleList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "25b")
                .Cast<RebarShape>()
                .ToList();
            if (UBarShapeSharpAngleList.Count == 0)
            {
                TaskDialog.Show("Revit", "Форма 25b не найдена");
                return Result.Failed;
            }
            RebarShape UBarShapeSharpAngle = UBarShapeSharpAngleList.First();

            //Выбор формы основной арматуры П-образный стержень под острым углом
            List<RebarShape> UBarShapeAngle2List = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "25c")
                .Cast<RebarShape>()
                .ToList();
            if (UBarShapeAngle2List.Count == 0)
            {
                TaskDialog.Show("Revit", "Форма 25c не найдена");
                return Result.Failed;
            }
            RebarShape UBarShape2Angle = UBarShapeAngle2List.First();

            //Выбор формы основной арматуры Z-образный стержень 
            List<RebarShape> ZBarShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "26")
                .Cast<RebarShape>()
                .ToList();
            if (ZBarShapeList.Count == 0)
            {
                ZBarShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Where(rs => rs.Name.ToString() == "О_26(α»90)")
                .Cast<RebarShape>()
                .ToList();
                if (ZBarShapeList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Форма 26 или О_26(α»90) не найдена");
                    return Result.Failed;
                }
            }
            RebarShape ZBarShape = ZBarShapeList.First();

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

            //Список типов для выбора основной арматуры Тип 1
            List<RebarBarType> mainRebarT1List = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

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

#endregion Формы стержней

            //Список типов для выбора основной арматуры Тип 2
            List<RebarBarType> mainRebarT2List = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов для выбора основной арматуры Тип 3
            List<RebarBarType> mainRebarT3List = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов для выбора арматуры хомута Тип 1
            List<RebarBarType> stirrupRebarT1List = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов для выбора арматуры хомута Тип 2
            List<RebarBarType> stirrupRebarT2List = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Список типов защитных слоев арматуры
            List<RebarCoverType> rebarTopCoverLayerList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();

            List<RebarCoverType> rebarBottomCoverLayerList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();

            List<RebarCoverType> rebarLRCoverLayerList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();

            //Выбор балок
            StructuralFramingSelectionFilter structuralFramingSelFilter = new StructuralFramingSelectionFilter(); //Вызов фильтра выбора
            IList<Reference> selBeams = sel.PickObjects(ObjectType.Element, structuralFramingSelFilter, "Выберите балки!");//Получение списка ссылок на выбранные балки
            //Получение списка выбранных балок
            List<FamilyInstance> beamsList = new List<FamilyInstance>();
            foreach (Reference beamRef in selBeams)
            {
                beamsList.Add(doc.GetElement(beamRef) as FamilyInstance);
            }
            //Завершение блока Получение списка балок


            //Вызов формы
            CIT_04_3_BeamReinforcementForm beamReinforcementForm
                = new CIT_04_3_BeamReinforcementForm(mainRebarT1List
                , mainRebarT2List
                , mainRebarT3List
                , stirrupRebarT1List
                , stirrupRebarT2List
                , rebarTopCoverLayerList
                , rebarBottomCoverLayerList
                , rebarLRCoverLayerList);

            beamReinforcementForm.ShowDialog();
            if (beamReinforcementForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            //Выбор типа основной арматуры
            //Диаметр стержня T1
            RebarBarType myMainRebarT1 = beamReinforcementForm.mySelectionMainBarT1;
            double myMainRebarT1Diam = myMainRebarT1.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
            //Диаметр стержня T2
            RebarBarType myMainRebarT2 = beamReinforcementForm.mySelectionMainBarT2;
            double myMainRebarT2Diam = myMainRebarT2.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
            //Диаметр стержня T3
            RebarBarType myMainRebarT3 = beamReinforcementForm.mySelectionMainBarT3;
            double myMainRebarT3Diam = myMainRebarT3.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
            //Диаметр хомута T1
            RebarBarType myStirrupT1 = beamReinforcementForm.mySelectionStirrupT1;
            double myStirrupT1Diam = myStirrupT1.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
            //Диаметр хомута T2
            RebarBarType myStirrupC1 = beamReinforcementForm.mySelectionStirrupC1;
            double myStirrupC1Diam = myStirrupC1.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();

            //Удлиннения основных стержней слева и справа
            double extensionLeftLenghtL1 = 0;
            double extensionLeftLenghtL2 = 0;
            double extensionRightLenghtR1 = 0;
            double extensionRightLenghtR2 = 0;

            extensionLeftLenghtL1 = beamReinforcementForm.ExtensionLeftLenghtL1 / 304.8;
            extensionLeftLenghtL2 = beamReinforcementForm.ExtensionLeftLenghtL2 / 304.8;
            extensionRightLenghtR1 = beamReinforcementForm.ExtensionRightLenghtR1 / 304.8;
            extensionRightLenghtR2 = beamReinforcementForm.ExtensionRightLenghtR2 / 304.8;

            double deepeningIntoTheStructureL1 = 0;
            double deepeningIntoTheStructureL2 = 0;
            double deepeningIntoTheStructureR1 = 0;
            double deepeningIntoTheStructureR2 = 0;

            deepeningIntoTheStructureL1 = beamReinforcementForm.DeepeningIntoTheStructureL1 / 304.8;
            deepeningIntoTheStructureL2 = beamReinforcementForm.DeepeningIntoTheStructureL2 / 304.8;
            deepeningIntoTheStructureR1 = beamReinforcementForm.DeepeningIntoTheStructureR1 / 304.8;
            deepeningIntoTheStructureR2 = beamReinforcementForm.DeepeningIntoTheStructureR2 / 304.8;
                      
            //Защитные слои арматуры
            RebarCoverType rebarTopCoverLayer = beamReinforcementForm.RebarTopCoverLayer;
            double rebarTopCoverLayerAsDouble = rebarTopCoverLayer.CoverDistance;

            RebarCoverType rebarBottomCoverLayer = beamReinforcementForm.RebarBottomCoverLayer;
            double rebarBottomCoverLayerAsDouble = rebarBottomCoverLayer.CoverDistance;

            RebarCoverType rebarLRCoverLayer = beamReinforcementForm.RebarLRCoverLayer;
            double rebarLRCoverLayerAsDouble = rebarLRCoverLayer.CoverDistance;

            //Кол-во стержней по граням
            int numberOfBarsTopFaces = beamReinforcementForm.NumberOfBarsTopFaces;
            int numberOfBarsBottomFaces = beamReinforcementForm.NumberOfBarsBottomFaces;

            // Шаг хомутов и длинф размещения
            double stirrupIndentL1 = beamReinforcementForm.StirrupIndentL1;
            double stirrupStepL1 = beamReinforcementForm.StirrupStepL1;
            int stirrupQuantityL1 = (int)(stirrupIndentL1 / stirrupStepL1) + 1;
            

            double stirrupIndentR1 = beamReinforcementForm.StirrupIndentR1;
            double stirrupStepR1 = beamReinforcementForm.StirrupStepR1;
            int stirrupQuantityR1 = (int)(stirrupIndentR1 / stirrupStepR1) + 1;
            
            double stirrupStepC1 = beamReinforcementForm.StirrupStepC1;

            stirrupStepL1 = stirrupStepL1 / 304.8;
            stirrupStepR1 = stirrupStepR1 / 304.8;
            stirrupStepC1 = stirrupStepC1 / 304.8;

            bool addBarL2 = beamReinforcementForm.AddBarL2;
            bool addBarR2 = beamReinforcementForm.AddBarR2;

            double extensionAddBarL2 = 0;
            double extensionAddBarR2 = 0;

            extensionAddBarL2 = beamReinforcementForm.ExtensionAddBarL2 / 304.8;
            extensionAddBarR2 = beamReinforcementForm.ExtensionAddBarR2 / 304.8;

            //Открытие транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение арматуры балок");
                foreach (FamilyInstance beam in beamsList)
                {
                    //Защитный слой арматуры верхней грани
                    Parameter rebarTopCoverLayerParam = beam.get_Parameter(BuiltInParameter.CLEAR_COVER_TOP);
                    if (rebarTopCoverLayerParam.IsReadOnly == false)
                    {
                        rebarTopCoverLayerParam.Set(rebarTopCoverLayer.Id);
                    }
                    //Защитный слой арматуры нижней грани
                    Parameter rebarBottomCoverLayerParam = beam.get_Parameter(BuiltInParameter.CLEAR_COVER_BOTTOM);
                    if (rebarBottomCoverLayerParam.IsReadOnly == false)
                    {
                        rebarBottomCoverLayerParam.Set(rebarBottomCoverLayer.Id);
                    }
                    //Защитный слой арматуры другие грани
                    Parameter rebarLRCoverLayerParam = beam.get_Parameter(BuiltInParameter.CLEAR_COVER_OTHER);
                    if (rebarLRCoverLayerParam.IsReadOnly == false)
                    {
                        rebarLRCoverLayerParam.Set(rebarLRCoverLayer.Id);
                    }

                    //Ширина балки
                    double beamWidth = 0;
                    if (beam.Symbol.LookupParameter("Рзм.Ширина") != null)
                    {
                        beamWidth = beam.Symbol.LookupParameter("Рзм.Ширина").AsDouble();
                    }
                    else
                    {
                        beamWidth = beam.Symbol.LookupParameter("ADSK_Размер_Ширина").AsDouble();
                    }

                    //Высота балки
                    double beamHeight = 0;
                    if (beam.Symbol.LookupParameter("Рзм.Высота") != null)
                    {
                        beamHeight = beam.Symbol.LookupParameter("Рзм.Высота").AsDouble();
                    }
                    else
                    {
                        beamHeight = beam.Symbol.LookupParameter("ADSK_Размер_Высота").AsDouble();
                    }


                    //Получаем основную кривую балки
                    LocationCurve myMainBeamLocationCurves = beam.Location as LocationCurve;
                    Curve myMainBeamCurve = myMainBeamLocationCurves.Curve;
                    //Начальная и конечная точки кривой
                    XYZ beamStartPoint = myMainBeamCurve.GetEndPoint(0);
                    XYZ beamEndPoint = myMainBeamCurve.GetEndPoint(1);

                    //Получение вектора основной кривой балки
                    Line beamMainLine = myMainBeamLocationCurves.Curve as Line;
                    XYZ beamMainLineDirectionVector = beamMainLine.Direction;

                    //Нормаль для построения основной арматуры
                    XYZ normal = beam.FacingOrientation;

                    //Вектор перпендикулярный основному направлению балки по вертикали
                    XYZ verticalVectorFromBeamMainLine = 1 * normal.CrossProduct(beamMainLineDirectionVector).Normalize();
                    //Горизонтальный вектор по основному направлению балки
                    XYZ horizontalVectorFromBeamMainLine = new XYZ(beamMainLineDirectionVector.X, beamMainLineDirectionVector.Y, 0).Normalize();

                    //Угол наклона балки к Z
                    double beamAngle = beamMainLineDirectionVector.AngleTo(XYZ.BasisZ) * (180 / Math.PI);
                    double beamAngleToX = beamAngle - 90;
                    double beamRoundAngle = Math.Round(beamAngle);

                    double extensionLeftLenghtL1Сalculated = Math.Abs(deepeningIntoTheStructureL1 / (Math.Cos(beamAngleToX * (Math.PI / 180))));
                    double extensionLeftLenghtL2Сalculated = Math.Abs(deepeningIntoTheStructureL2 / (Math.Cos(beamAngleToX * (Math.PI / 180))));
                    double extensionRightLenghtR1Сalculated = Math.Abs(deepeningIntoTheStructureR1 / (Math.Cos(beamAngleToX * (Math.PI / 180))));
                    double extensionRightLenghtR2Сalculated = Math.Abs(deepeningIntoTheStructureR2 / (Math.Cos(beamAngleToX * (Math.PI / 180))));

                    double extensionLeftBendLenghtL1 = extensionLeftLenghtL1 - extensionLeftLenghtL1Сalculated;
                    double extensionLeftBendLenghtL2 = extensionLeftLenghtL2 - extensionLeftLenghtL2Сalculated;
                    double extensionRightBendLenghtR1 = extensionRightLenghtR1 - extensionRightLenghtR1Сalculated;
                    double extensionRightBendLenghtR2 = extensionRightLenghtR2 - extensionRightLenghtR2Сalculated;

                    //Реальная длина балки
                    List<Solid> beamSolidList = new List<Solid>();
                    GeometryElement beamGeomElement = beam.get_Geometry(new Options());
                    foreach (GeometryObject geoObject in beamGeomElement)
                    {
                        Solid beamSolidForList = geoObject as Solid;
                        if (beamSolidForList != null)
                        {
                            if (beamSolidForList.Volume != 0)
                            {
                                beamSolidList.Add(beamSolidForList);
                            }
                        }
                    }

                    if (beamSolidList.Count == 0)
                    {
                        
                        foreach (GeometryObject geoObject in beamGeomElement)
                        {
                            GeometryInstance instance = geoObject as GeometryInstance;
                            foreach (GeometryObject instObj in instance.GetInstanceGeometry())
                            {
                                Solid beamSolidForList = instObj as Solid;
                                if (beamSolidForList != null)
                                {
                                    if (beamSolidForList.Volume != 0)
                                    {
                                        beamSolidList.Add(beamSolidForList);
                                    }
                                }
                            }
                        }
                    }
                    

                    List<Line> linesInMainDirectionList = new List<Line>();
                    Solid beamSolid = beamSolidList.First();
                    EdgeArray beamEdgesArray = beamSolid.Edges;
                    foreach (Edge edge in beamEdgesArray)
                    {
                        Line edgeLine = edge.AsCurve() as Line;
                        XYZ edgeLineDirectionVector = edgeLine.Direction;

                        if (Math.Round(edgeLineDirectionVector.AngleTo(beamMainLineDirectionVector) * (180 / Math.PI)) == 0 || edgeLineDirectionVector.AngleTo(beamMainLineDirectionVector) * (180 / Math.PI) == 180)
                        {
                            if (edgeLineDirectionVector.AngleTo(beamMainLineDirectionVector) * (180 / Math.PI) == 180)
                            {
                                edgeLine = edgeLine.CreateReversed() as Line;
                                linesInMainDirectionList.Add(edgeLine);
                            }
                            else
                            {
                                linesInMainDirectionList.Add(edgeLine);
                            }
                        }
                    }

                    //Грань с максимальной отметкой Z
                    double lineMaxZ = -10000;
                    Line requiredTopLine = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1, 0, 0)) as Line;
                    foreach (Line line in linesInMainDirectionList)
                    {
                        //Начальная и конечная точки грани
                        XYZ lineStartPoint = line.GetEndPoint(0);
                        XYZ lineEndPoint = line.GetEndPoint(1);

                        XYZ sideVector = new XYZ(lineStartPoint.X - beamStartPoint.X, lineStartPoint.Y - beamStartPoint.Y, lineStartPoint.Z - beamStartPoint.Z);
                        double side = beamMainLineDirectionVector.X * sideVector.Y - sideVector.X * beamMainLineDirectionVector.Y;

                        if ((lineMaxZ < lineStartPoint.Z || lineMaxZ < lineEndPoint.Z) & side < 0)
                        {
                            if (lineStartPoint.Z > lineEndPoint.Z)
                            {
                                lineMaxZ = lineStartPoint.Z;
                            }
                            else
                            {
                                lineMaxZ = lineEndPoint.Z;
                            }
                            requiredTopLine = line;
                        }
                    }

                    //Грань с минимальной отметкой Z
                    double lineMinZ = 10000;
                    Line requiredBottomLine = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1, 0, 0)) as Line;
                    foreach (Line line in linesInMainDirectionList)
                    {
                        //Начальная и конечная точки грани
                        XYZ lineStartPoint = line.GetEndPoint(0);
                        XYZ lineEndPoint = line.GetEndPoint(1);

                        XYZ sideVector = new XYZ(lineStartPoint.X - beamStartPoint.X, lineStartPoint.Y - beamStartPoint.Y, lineStartPoint.Z - beamStartPoint.Z);
                        double side = beamMainLineDirectionVector.X * sideVector.Y - sideVector.X * beamMainLineDirectionVector.Y;

                        if ((lineMinZ > lineStartPoint.Z || lineMinZ > lineEndPoint.Z) & side < 0)
                        {
                            if (lineStartPoint.Z < lineEndPoint.Z)
                            {
                                lineMinZ = lineStartPoint.Z;
                            }
                            else
                            {
                                lineMinZ = lineEndPoint.Z;
                            }
                            requiredBottomLine = line;
                        }
                    }

                    XYZ requiredTopLineStartPoint = new XYZ(0, 0, 0);
                    XYZ requiredTopLineEndPoint = new XYZ(0, 0, 0);
                    XYZ requiredBottomLineStartPoint = new XYZ(0, 0, 0);
                    XYZ requiredBottomLineEndPoint = new XYZ(0, 0, 0);

                    //Определение крайних точек геометрии балки
                    if ((Math.Round(requiredTopLine.GetEndPoint(0).Z, 6) != Math.Round(beamStartPoint.Z, 6) || Math.Round(requiredTopLine.GetEndPoint(1).Z, 6) != Math.Round(beamEndPoint.Z, 6)) & beamAngle != 90)
                    {
                        //Начальная и конечная точки верхней грани
                        requiredTopLineStartPoint = new XYZ(requiredTopLine.GetEndPoint(0).X, requiredTopLine.GetEndPoint(0).Y, beamStartPoint.Z)
                            + ((requiredTopLine.GetEndPoint(0).Z - beamStartPoint.Z) * XYZ.BasisZ)
                            + ((Math.Tan(beamAngleToX * (Math.PI / 180)) * (rebarTopCoverLayerAsDouble + myMainRebarT1Diam / 2)) * beamMainLineDirectionVector);
                        requiredTopLineEndPoint = new XYZ(requiredTopLine.GetEndPoint(1).X, requiredTopLine.GetEndPoint(1).Y, beamEndPoint.Z)
                            + ((requiredTopLine.GetEndPoint(1).Z - beamEndPoint.Z) * XYZ.BasisZ)
                            + ((Math.Tan(beamAngleToX * (Math.PI / 180)) * (rebarTopCoverLayerAsDouble + myMainRebarT1Diam / 2)) * beamMainLineDirectionVector);

                        //Начальная и конечная точки нижней грани
                        requiredBottomLineStartPoint = new XYZ(requiredBottomLine.GetEndPoint(0).X, requiredBottomLine.GetEndPoint(0).Y, beamStartPoint.Z)
                            + ((requiredBottomLine.GetEndPoint(0).Z - beamStartPoint.Z) * XYZ.BasisZ)
                            - ((Math.Tan(beamAngleToX * (Math.PI / 180)) * (rebarBottomCoverLayerAsDouble + myMainRebarT2Diam / 2)) * beamMainLineDirectionVector);
                        requiredBottomLineEndPoint = new XYZ(requiredBottomLine.GetEndPoint(1).X, requiredBottomLine.GetEndPoint(1).Y, beamEndPoint.Z)
                            + ((requiredBottomLine.GetEndPoint(1).Z - beamEndPoint.Z) * XYZ.BasisZ)
                            - ((Math.Tan(beamAngleToX * (Math.PI / 180)) * (rebarBottomCoverLayerAsDouble + myMainRebarT2Diam / 2)) * beamMainLineDirectionVector);
                    }

                    else if ((Math.Round(requiredTopLine.GetEndPoint(0).Z, 6) != Math.Round(beamStartPoint.Z, 6) || Math.Round(requiredTopLine.GetEndPoint(1).Z, 6) != Math.Round(beamEndPoint.Z, 6)) & beamAngle == 90)
                    {
                        //Начальная и конечная точки верхней грани
                        requiredTopLineStartPoint = new XYZ(requiredTopLine.GetEndPoint(0).X, requiredTopLine.GetEndPoint(0).Y, beamStartPoint.Z);
                        requiredTopLineEndPoint = new XYZ(requiredTopLine.GetEndPoint(1).X, requiredTopLine.GetEndPoint(1).Y, beamEndPoint.Z);

                        //Начальная и конечная точки нижней грани
                        requiredBottomLineStartPoint = new XYZ(requiredBottomLine.GetEndPoint(0).X, requiredBottomLine.GetEndPoint(0).Y, beamStartPoint.Z)
                            + ((requiredBottomLine.GetEndPoint(0).Z - beamStartPoint.Z) * XYZ.BasisZ);
                        requiredBottomLineEndPoint = new XYZ(requiredBottomLine.GetEndPoint(1).X, requiredBottomLine.GetEndPoint(1).Y, beamEndPoint.Z)
                            + ((requiredBottomLine.GetEndPoint(1).Z - beamEndPoint.Z) * XYZ.BasisZ);

                    }

                    else
                    {
                        //Начальная и конечная точки верхней грани
                        requiredTopLineStartPoint = new XYZ(requiredTopLine.GetEndPoint(0).X, requiredTopLine.GetEndPoint(0).Y, beamStartPoint.Z);
                        requiredTopLineEndPoint = new XYZ(requiredTopLine.GetEndPoint(1).X, requiredTopLine.GetEndPoint(1).Y, beamEndPoint.Z);

                        //Начальная и конечная точки нижней грани
                        requiredBottomLineStartPoint = new XYZ(requiredBottomLine.GetEndPoint(0).X, requiredBottomLine.GetEndPoint(0).Y, beamStartPoint.Z)
                            + ((requiredBottomLine.GetEndPoint(0).Z - beamStartPoint.Z) * XYZ.BasisZ);
                        requiredBottomLineEndPoint = new XYZ(requiredBottomLine.GetEndPoint(1).X, requiredBottomLine.GetEndPoint(1).Y, beamEndPoint.Z)
                            + ((requiredBottomLine.GetEndPoint(1).Z - beamEndPoint.Z) * XYZ.BasisZ);
                    }
                    //Длина балки по верхней грани
                    double beamLength = requiredTopLineStartPoint.DistanceTo(requiredTopLineEndPoint);

                    //Размещение основной арматуры
                    if (extensionLeftBendLenghtL1 <= 0 & extensionRightBendLenghtR1 <= 0 & beamRoundAngle == 90)
                    {
                        //Точки для построения стержней основной верхней арматуры
                        XYZ mainTopRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL1 * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR1 * beamMainLineDirectionVector);

                        //Кривые стержня основной верхней арматуры
                        List<Curve> myMainTopRebarCurves = new List<Curve>();
                        Curve topLine1 = Line.CreateBound(mainTopRebar_p1, mainTopRebar_p2) as Curve;
                        myMainTopRebarCurves.Add(topLine1);

                        //Стержни по верхней грани
                        Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT1
                            , null
                            , null
                            , beam
                            , normal
                            , myMainTopRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL1 <= 0 & extensionRightBendLenghtR1 <= 0 & beamRoundAngle != 90)
                    {
                        //Точки для построения стержней основной верхней арматуры
                        XYZ mainTopRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainTopRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainTopRebar_p3 = mainTopRebar_p1 - extensionLeftLenghtL1 * horizontalVectorFromBeamMainLine;
                        XYZ mainTopRebar_p4 = mainTopRebar_p2 + extensionRightLenghtR1 * horizontalVectorFromBeamMainLine;

                        //Кривые стержня основной верхней арматуры
                        List<Curve> myMainTopRebarCurves = new List<Curve>();
                        Curve topLine1 = Line.CreateBound(mainTopRebar_p3, mainTopRebar_p1) as Curve;
                        myMainTopRebarCurves.Add(topLine1);
                        Curve topLine2 = Line.CreateBound(mainTopRebar_p1, mainTopRebar_p2) as Curve;
                        myMainTopRebarCurves.Add(topLine2);
                        Curve topLine3 = Line.CreateBound(mainTopRebar_p2, mainTopRebar_p4) as Curve;
                        myMainTopRebarCurves.Add(topLine3);

                        //Стержни по верхней грани
                        Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                            , ZBarShape
                            , myMainRebarT1
                            , null
                            , null
                            , beam
                            , normal
                            , myMainTopRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL1 > 0 & extensionRightBendLenghtR1 <= 0 & beamRoundAngle == 90)
                    {
                        //Точки для построения стержней основной верхней арматуры
                        XYZ mainTopRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * beamMainLineDirectionVector)
                            - (deepeningIntoTheStructureL1 * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR1 * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p3 = mainTopRebar_p1 - (extensionLeftBendLenghtL1 * XYZ.BasisZ) + (myMainRebarT1Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной верхней арматуры
                        List<Curve> myMainTopRebarCurves = new List<Curve>();
                        Curve topLine1 = Line.CreateBound(mainTopRebar_p3, mainTopRebar_p1) as Curve;
                        myMainTopRebarCurves.Add(topLine1);
                        Curve topLine2 = Line.CreateBound(mainTopRebar_p1, mainTopRebar_p2) as Curve;
                        myMainTopRebarCurves.Add(topLine2);

                        //Стержни по верхней грани
                        Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT1
                            , null
                            , null
                            , beam
                            , normal
                            , myMainTopRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL1 > 0 & extensionRightBendLenghtR1 <= 0 & beamRoundAngle != 90)
                    {
                        //Точки для построения стержней основной верхней арматуры
                        XYZ mainTopRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            - ((extensionLeftLenghtL1Сalculated - ((myMainRebarT1Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainTopRebar_p3 = mainTopRebar_p1 - (extensionLeftBendLenghtL1 * XYZ.BasisZ);
                        XYZ mainTopRebar_p4 = mainTopRebar_p2 + (extensionRightLenghtR1 * horizontalVectorFromBeamMainLine);

                        //Кривые стержня основной верхней арматуры
                        List<Curve> myMainTopRebarCurves = new List<Curve>();
                        Curve topLine1 = Line.CreateBound(mainTopRebar_p3, mainTopRebar_p1) as Curve;
                        myMainTopRebarCurves.Add(topLine1);
                        Curve topLine2 = Line.CreateBound(mainTopRebar_p1, mainTopRebar_p2) as Curve;
                        myMainTopRebarCurves.Add(topLine2);
                        Curve topLine3 = Line.CreateBound(mainTopRebar_p2, mainTopRebar_p4) as Curve;
                        myMainTopRebarCurves.Add(topLine3);

                        if (beamRoundAngle < 90)
                        {
                            //Стержни по верхней грани
                            Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                                , UBarShapeAngle
                                , myMainRebarT1
                                , null
                                , null
                                , beam
                                , normal
                                , myMainTopRebarCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                        }

                        else if (beamRoundAngle > 90)
                        {
                            //Стержни по верхней грани
                            Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                                , UBarShapeSharpAngle
                                , myMainRebarT1
                                , null
                                , null
                                , beam
                                , normal
                                , myMainTopRebarCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                        }

                    } // почти Ok

                    if (extensionLeftBendLenghtL1 <= 0 & extensionRightBendLenghtR1 > 0 & beamRoundAngle == 90) // Ok
                    {
                        //Точки для построения стержней основной верхней арматуры
                        XYZ mainTopRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL1 * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            + (deepeningIntoTheStructureR1 * beamMainLineDirectionVector)
                            - (myMainRebarT1Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p3 = mainTopRebar_p2 - (extensionRightBendLenghtR1 * XYZ.BasisZ) + (myMainRebarT1Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной верхней арматуры
                        List<Curve> myMainTopRebarCurves = new List<Curve>();
                        Curve topLine1 = Line.CreateBound(mainTopRebar_p1, mainTopRebar_p2) as Curve;
                        myMainTopRebarCurves.Add(topLine1);
                        Curve topLine2 = Line.CreateBound(mainTopRebar_p2, mainTopRebar_p3) as Curve;
                        myMainTopRebarCurves.Add(topLine2);

                        //Стержни по верхней грани
                        Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT1
                            , null
                            , null
                            , beam
                            , normal
                            , myMainTopRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL1 <= 0 & extensionRightBendLenghtR1 > 0 & beamRoundAngle != 90) // Ok
                    {
                        //Точки для построения стержней основной верхней арматуры
                        XYZ mainTopRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainTopRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            + ((extensionRightLenghtR1Сalculated - ((myMainRebarT1Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p3 = mainTopRebar_p1 - (extensionLeftLenghtL1 * horizontalVectorFromBeamMainLine);
                        XYZ mainTopRebar_p4 = mainTopRebar_p2 - (extensionRightBendLenghtR1 * XYZ.BasisZ);

                        //Кривые стержня основной верхней арматуры
                        List<Curve> myMainTopRebarCurves = new List<Curve>();
                        Curve topLine1 = Line.CreateBound(mainTopRebar_p3, mainTopRebar_p1) as Curve;
                        myMainTopRebarCurves.Add(topLine1);
                        Curve topLine2 = Line.CreateBound(mainTopRebar_p1, mainTopRebar_p2) as Curve;
                        myMainTopRebarCurves.Add(topLine2);
                        Curve topLine3 = Line.CreateBound(mainTopRebar_p2, mainTopRebar_p4) as Curve;
                        myMainTopRebarCurves.Add(topLine3);

                        if (beamRoundAngle < 90)
                        {
                            //Стержни по верхней грани
                            Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                            , UBarShapeSharpAngle
                            , myMainRebarT1
                            , null
                            , null
                            , beam
                            , normal
                            , myMainTopRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                        }
                        if (beamRoundAngle > 90)
                        {
                            //Стержни по верхней грани
                            Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                            , UBarShapeAngle
                            , myMainRebarT1
                            , null
                            , null
                            , beam
                            , normal
                            , myMainTopRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                        }
                    } // почти Ok

                    if (extensionLeftBendLenghtL1 > 0 & extensionRightBendLenghtR1 > 0 & beamRoundAngle == 90) // Ok
                    {
                        //Точки для построения стержней основной верхней арматуры
                        XYZ mainTopRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL1Сalculated * beamMainLineDirectionVector)
                            + (myMainRebarT1Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR1Сalculated * beamMainLineDirectionVector)
                            - (myMainRebarT1Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p3 = mainTopRebar_p1
                            - (extensionLeftBendLenghtL1 * XYZ.BasisZ)
                            + (myMainRebarT1Diam / 2 * XYZ.BasisZ);
                        XYZ mainTopRebar_p4 = mainTopRebar_p2
                            - (extensionRightBendLenghtR1 * XYZ.BasisZ)
                            + (myMainRebarT1Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной верхней арматуры
                        List<Curve> myMainTopRebarCurves = new List<Curve>();
                        Curve topLine1 = Line.CreateBound(mainTopRebar_p3, mainTopRebar_p1) as Curve;
                        myMainTopRebarCurves.Add(topLine1);
                        Curve topLine2 = Line.CreateBound(mainTopRebar_p1, mainTopRebar_p2) as Curve;
                        myMainTopRebarCurves.Add(topLine2);
                        Curve topLine3 = Line.CreateBound(mainTopRebar_p2, mainTopRebar_p4) as Curve;
                        myMainTopRebarCurves.Add(topLine3);

                        //Стержни по верхней грани
                        Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                            , UBarShape
                            , myMainRebarT1
                            , null
                            , null
                            , beam
                            , normal
                            , myMainTopRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL1 > 0 & extensionRightBendLenghtR1 > 0 & beamRoundAngle != 90)
                    {
                        //Точки для построения стержней основной верхней арматуры
                        XYZ mainTopRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            - ((extensionLeftLenghtL1Сalculated - ((myMainRebarT1Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myMainRebarT1Diam / 2 * verticalVectorFromBeamMainLine)
                            + ((extensionRightLenghtR1Сalculated - ((myMainRebarT1Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainTopRebar_p3 = mainTopRebar_p1 - (extensionLeftBendLenghtL1 * XYZ.BasisZ);
                        XYZ mainTopRebar_p4 = mainTopRebar_p2 - (extensionRightBendLenghtR1 * XYZ.BasisZ);

                        //Кривые стержня основной верхней арматуры
                        List<Curve> myMainTopRebarCurves = new List<Curve>();
                        Curve topLine1 = Line.CreateBound(mainTopRebar_p3, mainTopRebar_p1) as Curve;
                        myMainTopRebarCurves.Add(topLine1);
                        Curve topLine2 = Line.CreateBound(mainTopRebar_p1, mainTopRebar_p2) as Curve;
                        myMainTopRebarCurves.Add(topLine2);
                        Curve topLine3 = Line.CreateBound(mainTopRebar_p2, mainTopRebar_p4) as Curve;
                        myMainTopRebarCurves.Add(topLine3);

                        if (beamRoundAngle < 90)
                        {
                            //Стержни по верхней грани
                            Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                                , UBarShape2Angle
                                , myMainRebarT1
                                , null
                                , null
                                , beam
                                , normal
                                , myMainTopRebarCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                        }

                        if (beamRoundAngle > 90)
                        {
                            //Стержни по верхней грани
                            Rebar mainTopRebar = Rebar.CreateFromCurvesAndShape(doc
                                , UBarShape2Angle
                                , myMainRebarT1
                                , null
                                , null
                                , beam
                                , normal
                                , myMainTopRebarCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainTopRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsTopFaces);
                        }
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle == 90 & addBarL2 == false & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2 * beamMainLineDirectionVector);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle == 90 & addBarL2 == true & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2 * beamMainLineDirectionVector);

                        //Точки для построения добавочных стержней нижней арматуры
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector);


                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры
                        List<Curve> myMainBottomAddRebarCurves = new List<Curve>();
                        Curve bottomAddLine1 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurves.Add(bottomAddLine1);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани
                        Rebar mainAddBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                    } //Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle == 90 & addBarL2 == false & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2 * beamMainLineDirectionVector);


                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры
                        List<Curve> myMainBottomAddRebarCurves = new List<Curve>();
                        Curve bottomAddLine1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurves.Add(bottomAddLine1);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани
                        Rebar mainAddBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                    } //Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle == 90 & addBarL2 == true & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);


                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector);


                        //Точки для построения добавочных стержней нижней арматуры
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2 * beamMainLineDirectionVector);


                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани справа
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                    } // Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle != 90 & addBarL2 == false & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1 - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);
                        XYZ mainBottomRebar_p4 = mainBottomRebar_p2 + (extensionRightLenghtR2 * horizontalVectorFromBeamMainLine);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);
                        Curve bottomLine3 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p4) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine3);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , ZBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } //Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle != 90 & addBarL2 == true & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p2 + (extensionRightLenghtR2 * horizontalVectorFromBeamMainLine);
                        
                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p3) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle != 90 & addBarL2 == false & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1 - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2* beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 + (extensionRightLenghtR2 * horizontalVectorFromBeamMainLine);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани справа
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle != 90 & addBarL2 == true & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector); ;

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector); ;

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector); ;

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector); ;

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 + (extensionRightLenghtR2 * horizontalVectorFromBeamMainLine);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани справа
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle == 90 & addBarL2 == false & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - (deepeningIntoTheStructureL2 * beamMainLineDirectionVector)
                            + (myMainRebarT2Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ) - (myMainRebarT2Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } //Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle == 90 & addBarL2 == true & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2 * beamMainLineDirectionVector);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (deepeningIntoTheStructureL2 * beamMainLineDirectionVector)
                            + (myMainRebarT3Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ) - (myMainRebarT3Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomStraightBarShapeine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomStraightBarShapeine1);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle == 90 & addBarL2 == false & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - (deepeningIntoTheStructureL2 * beamMainLineDirectionVector)
                            + (myMainRebarT2Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ) - (myMainRebarT2Diam / 2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2 * beamMainLineDirectionVector);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани справа
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle == 90 & addBarL2 == true & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (deepeningIntoTheStructureL2 * beamMainLineDirectionVector)
                            + (myMainRebarT3Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ) - (myMainRebarT3Diam / 2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2 * beamMainLineDirectionVector);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle != 90 & addBarL2 == false & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - ((extensionLeftLenghtL2Сalculated - ((myMainRebarT2Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ);
                        XYZ mainBottomRebar_p4 = mainBottomRebar_p2 + (extensionRightLenghtR2 * horizontalVectorFromBeamMainLine);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);
                        Curve bottomLine3 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p4) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine3);

                        if (beamRoundAngle < 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , UBarShapeSharpAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }
                        else if (beamRoundAngle > 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , UBarShapeAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle != 90 & addBarL2 == true & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p2 + (extensionRightLenghtR2 * horizontalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - ((extensionLeftLenghtL2Сalculated - ((myMainRebarT3Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p3) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        if (beamRoundAngle < 90)
                        {
                            //Дополнительные стержни по нижней грани слева
                            Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeSharpAngle
                                , myMainRebarT3
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomAddRebarCurvesL2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        if (beamRoundAngle > 90)
                        {
                            //Дополнительные стержни по нижней грани слева
                            Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeAngle
                                , myMainRebarT3
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomAddRebarCurvesL2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                    } // почти Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle != 90 & addBarL2 == false & addBarR2 == true) // почти Ok
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - ((extensionLeftLenghtL2Сalculated - ((myMainRebarT2Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 + (extensionRightLenghtR2 * horizontalVectorFromBeamMainLine);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                       

                        if (beamRoundAngle < 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeSharpAngle
                                , myMainRebarT2
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomRebarCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        if (beamRoundAngle > 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeAngle
                                , myMainRebarT2
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomRebarCurves
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        //Дополнительные стержни по нижней грани справа
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 <= 0 & beamRoundAngle != 90 & addBarL2 == true & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - ((extensionLeftLenghtL2Сalculated - ((myMainRebarT3Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector); ;

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 + (extensionRightLenghtR2 * horizontalVectorFromBeamMainLine);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        if (beamRoundAngle < 90)
                        {
                            //Дополнительные стержни по нижней грани слева
                            Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeSharpAngle
                                , myMainRebarT3
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomAddRebarCurvesL2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        if (beamRoundAngle > 90)
                        {
                            //Дополнительные стержни по нижней грани слева
                            Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeAngle
                                , myMainRebarT3
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomAddRebarCurvesL2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        //Дополнительные стержни по нижней грани справа
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle == 90 & addBarL2 == false & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + (deepeningIntoTheStructureR2 * beamMainLineDirectionVector)
                            - (myMainRebarT2Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ) - (myMainRebarT2Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p3) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle == 90 & addBarL2 == true & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + (deepeningIntoTheStructureR2 * beamMainLineDirectionVector)
                            - (myMainRebarT2Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ) - (myMainRebarT2Diam / 2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p3) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle == 90 & addBarL2 == false & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2* beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (deepeningIntoTheStructureR2 * beamMainLineDirectionVector)
                            - (myMainRebarT3Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ) - (myMainRebarT3Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани саправа
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle == 90 & addBarL2 == true & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredTopLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredTopLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (deepeningIntoTheStructureR2 * beamMainLineDirectionVector)
                            - (myMainRebarT3Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ) - (myMainRebarT3Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани саправа
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle != 90 & addBarL2 == false & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                             + (rebarLRCoverLayerAsDouble * normal)
                             + (myMainRebarT2Diam / 2 * normal)
                             - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                             - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + ((extensionRightLenghtR2Сalculated - ((myMainRebarT2Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1 - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);
                        XYZ mainBottomRebar_p4 = mainBottomRebar_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);
                        Curve bottomLine3 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p4) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine3);

                        if (beamRoundAngle < 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , UBarShapeAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        if (beamRoundAngle > 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , UBarShapeSharpAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle != 90 & addBarL2 == true & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                             + (rebarLRCoverLayerAsDouble * normal)
                             + (myMainRebarT2Diam / 2 * normal)
                             - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                             - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + ((extensionRightLenghtR2Сalculated - ((myMainRebarT2Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);
                        
                        XYZ mainBottomRebar_p3 = mainBottomRebar_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p3) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        if (beamRoundAngle < 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        if (beamRoundAngle > 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeSharpAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle != 90 & addBarL2 == false & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                             + (rebarLRCoverLayerAsDouble * normal)
                             + (myMainRebarT2Diam / 2 * normal)
                             - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                             - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1 - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + ((extensionRightLenghtR2Сalculated - ((myMainRebarT3Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector)
                            -(((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ);


                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);


                        if (beamRoundAngle > 90)
                        {
                            //Дополнительные стержни по нижней грани справа
                            Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeSharpAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }


                        if (beamRoundAngle < 90)
                        {
                            //Дополнительные стержни по нижней грани справа
                            Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 <= 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle != 90 & addBarL2 == true & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                             + (rebarLRCoverLayerAsDouble * normal)
                             + (myMainRebarT2Diam / 2 * normal)
                             - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                             - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 - (extensionLeftLenghtL2 * horizontalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + ((extensionRightLenghtR2Сalculated - ((myMainRebarT3Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ);


                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        if (beamRoundAngle > 90)
                        {
                            //Дополнительные стержни по нижней грани справа
                            Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeSharpAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        if (beamRoundAngle < 90)
                        {
                            //Дополнительные стержни по нижней грани справа
                            Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle == 90 & addBarL2 == false & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2Сalculated * beamMainLineDirectionVector)
                            + (myMainRebarT2Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2Сalculated * beamMainLineDirectionVector)
                            - (myMainRebarT2Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1
                            + (extensionLeftBendLenghtL2 * XYZ.BasisZ)
                            - (myMainRebarT2Diam / 2 * XYZ.BasisZ);

                        XYZ mainBottomRebar_p4 = mainBottomRebar_p2
                            + (extensionRightBendLenghtR2 * XYZ.BasisZ)
                            - (myMainRebarT2Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);
                        Curve bottomLine3 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p4) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine3);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , UBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle == 90 & addBarL2 == true & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2Сalculated * beamMainLineDirectionVector)
                            - (myMainRebarT2Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p2
                            + (extensionRightBendLenghtR2 * XYZ.BasisZ)
                            - (myMainRebarT2Diam / 2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2Сalculated * beamMainLineDirectionVector)
                            + (myMainRebarT3Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 
                            + (extensionLeftBendLenghtL2 * XYZ.BasisZ)
                            - (myMainRebarT3Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p3) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle == 90 & addBarL2 == false & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2Сalculated * beamMainLineDirectionVector)
                            + (myMainRebarT2Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1
                            + (extensionLeftBendLenghtL2 * XYZ.BasisZ)
                            - (myMainRebarT2Diam / 2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2Сalculated * beamMainLineDirectionVector)
                            - (myMainRebarT3Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 
                            + (extensionRightBendLenghtR2 * XYZ.BasisZ)
                            -(myMainRebarT3Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани справа
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle == 90 & addBarL2 == true & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionLeftLenghtL2Сalculated * beamMainLineDirectionVector)
                            + (myMainRebarT3Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1
                            + (extensionLeftBendLenghtL2 * XYZ.BasisZ)
                            - (myMainRebarT3Diam / 2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionRightLenghtR2Сalculated * beamMainLineDirectionVector)
                            - (myMainRebarT3Diam / 2 * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2
                            + (extensionRightBendLenghtR2 * XYZ.BasisZ)
                            - (myMainRebarT3Diam / 2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани слева
                        Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShape
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesL2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        //Дополнительные стержни по нижней грани справа
                        Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                        , LBarShape
                        , myMainRebarT3
                        , null
                        , null
                        , beam
                        , normal
                        , myMainBottomAddRebarCurvesR2
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                    } // Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle != 90 & addBarL2 == false & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - ((extensionLeftLenghtL2Сalculated - ((myMainRebarT2Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + ((extensionRightLenghtR2Сalculated - ((myMainRebarT2Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ);
                        XYZ mainBottomRebar_p4 = mainBottomRebar_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);
                        Curve bottomLine3 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p4) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine3);

                        if (beamRoundAngle < 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , UBarShape2Angle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        if (beamRoundAngle > 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , UBarShape2Angle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle != 90 & addBarL2 == true & addBarR2 == false)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            + ((extensionRightLenghtR2Сalculated - ((myMainRebarT2Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - ((extensionLeftLenghtL2Сalculated - ((myMainRebarT3Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p2, mainBottomRebar_p3) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        if (beamRoundAngle < 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                            //Дополнительные стержни по нижней грани слева
                            Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeSharpAngle
                                , myMainRebarT3
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomAddRebarCurvesL2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        if (beamRoundAngle > 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeSharpAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                            //Дополнительные стержни по нижней грани слева
                            Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeAngle
                                , myMainRebarT3
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomAddRebarCurvesL2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle != 90 & addBarL2 == false & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine)
                            - ((extensionLeftLenghtL2Сalculated - ((myMainRebarT2Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p3 = mainBottomRebar_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + ((extensionRightLenghtR2Сalculated - ((myMainRebarT3Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector)
                            -(((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p3, mainBottomRebar_p1) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);
                        Curve bottomLine2 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        if (beamRoundAngle < 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeSharpAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                            //Дополнительные стержни по нижней грани справа
                            Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        if (beamRoundAngle > 90)
                        {
                            //Стержни по нижней грани
                            Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeAngle
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                            //Дополнительные стержни по нижней грани справа
                            Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                            , LBarShapeSharpAngle
                            , myMainRebarT3
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomAddRebarCurvesR2
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }
                    } // почти Ok

                    if (extensionLeftBendLenghtL2 > 0 & extensionRightBendLenghtR2 > 0 & beamRoundAngle != 90 & addBarL2 == true & addBarR2 == true)
                    {
                        //Точки для построения стержней основной нижней арматуры
                        XYZ mainBottomRebar_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        XYZ mainBottomRebar_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam / 2 * verticalVectorFromBeamMainLine);

                        //Точки для построения добавочных стержней нижней арматуры слева
                        XYZ mainAddBottomRebarL2_p1 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - ((extensionLeftLenghtL2Сalculated - ((myMainRebarT3Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p2 = requiredBottomLineStartPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + (extensionAddBarL2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarL2_p3 = mainAddBottomRebarL2_p1 + (extensionLeftBendLenghtL2 * XYZ.BasisZ);

                        //Точки для построения добавочных стержней нижней арматуры справа
                        XYZ mainAddBottomRebarR2_p1 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            - (extensionAddBarR2 * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p2 = requiredBottomLineEndPoint
                            + (rebarLRCoverLayerAsDouble * normal)
                            + (myMainRebarT2Diam / 2 * normal)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myMainRebarT2Diam * verticalVectorFromBeamMainLine)
                            - (myMainRebarT3Diam / 2 * verticalVectorFromBeamMainLine)
                            + ((extensionRightLenghtR2Сalculated - ((myMainRebarT3Diam / 2) / (Math.Cos(beamAngleToX * (Math.PI / 180))))) * beamMainLineDirectionVector)
                            - (((myMainRebarT3Diam / 2 + myMainRebarT2Diam / 2) * (Math.Tan(beamAngleToX * (Math.PI / 180)))) * beamMainLineDirectionVector);

                        XYZ mainAddBottomRebarR2_p3 = mainAddBottomRebarR2_p2 + (extensionRightBendLenghtR2 * XYZ.BasisZ);

                        //Кривые стержня основной нижней арматуры
                        List<Curve> myMainBottomRebarCurves = new List<Curve>();
                        Curve bottomLine1 = Line.CreateBound(mainBottomRebar_p1, mainBottomRebar_p2) as Curve;
                        myMainBottomRebarCurves.Add(bottomLine1);

                        //Кривые дополнительных стержней нижней арматуры слева
                        List<Curve> myMainBottomAddRebarCurvesL2 = new List<Curve>();
                        Curve bottomAddLineL2_1 = Line.CreateBound(mainAddBottomRebarL2_p3, mainAddBottomRebarL2_p1) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_1);
                        Curve bottomAddLineL2_2 = Line.CreateBound(mainAddBottomRebarL2_p1, mainAddBottomRebarL2_p2) as Curve;
                        myMainBottomAddRebarCurvesL2.Add(bottomAddLineL2_2);

                        //Кривые дополнительных стержней нижней арматуры справа
                        List<Curve> myMainBottomAddRebarCurvesR2 = new List<Curve>();
                        Curve bottomAddLineR2_1 = Line.CreateBound(mainAddBottomRebarR2_p1, mainAddBottomRebarR2_p2) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_1);
                        Curve bottomAddLineR2_2 = Line.CreateBound(mainAddBottomRebarR2_p2, mainAddBottomRebarR2_p3) as Curve;
                        myMainBottomAddRebarCurvesR2.Add(bottomAddLineR2_2);

                        //Стержни по нижней грани
                        Rebar mainBottomRebar = Rebar.CreateFromCurvesAndShape(doc
                            , straightBarShape
                            , myMainRebarT2
                            , null
                            , null
                            , beam
                            , normal
                            , myMainBottomRebarCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                        mainBottomRebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                        if (beamRoundAngle < 90)
                        {
                            //Дополнительные стержни по нижней грани слева
                            Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeSharpAngle
                                , myMainRebarT3
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomAddRebarCurvesL2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                            //Дополнительные стержни по нижней грани справа
                            Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeAngle
                                , myMainRebarT3
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomAddRebarCurvesR2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }

                        if (beamRoundAngle > 90)
                        {
                            //Дополнительные стержни по нижней грани слева
                            Rebar mainAddBottomRebarL2 = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeAngle
                                , myMainRebarT3
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomAddRebarCurvesL2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarL2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);

                            //Дополнительные стержни по нижней грани справа
                            Rebar mainAddBottomRebarR2 = Rebar.CreateFromCurvesAndShape(doc
                                , LBarShapeSharpAngle
                                , myMainRebarT3
                                , null
                                , null
                                , beam
                                , normal
                                , myMainBottomAddRebarCurvesR2
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);

                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(1);
                            mainAddBottomRebarR2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(numberOfBarsBottomFaces);
                        }
                    } // почти Ok

                    //Поперечный шаг основной арматуры для хомутов
                    double topBarsStepForStirrup = ((beamWidth - rebarLRCoverLayerAsDouble * 2 - myMainRebarT1Diam) / (numberOfBarsTopFaces - 1));

                    //Хомуты при ширине балки меньше 350
                    if (Math.Round(beamWidth * 304.8) < 350)
                    {
                        //Точки для построения хомута Тест1
                        XYZ stirrupT1_p1 = requiredTopLineStartPoint
                            + ((50 / 304.8) * beamMainLineDirectionVector)
                            + (rebarLRCoverLayerAsDouble * normal)
                            - (myStirrupT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myStirrupT1Diam / 2 * verticalVectorFromBeamMainLine);
                        XYZ stirrupT1_p2 = stirrupT1_p1
                            + (beamWidth * normal)
                            - ((rebarLRCoverLayerAsDouble * 2) * normal)
                            + (myStirrupT1Diam * normal);
                        XYZ stirrupT1_p3 = stirrupT1_p2
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myStirrupT1Diam * verticalVectorFromBeamMainLine);
                        XYZ stirrupT1_p4 = stirrupT1_p3
                            - (beamWidth * normal)
                            + ((rebarLRCoverLayerAsDouble * 2) * normal)
                            - (myStirrupT1Diam * normal);

                        //Кривые стержня хомута Тест1
                        List<Curve> myStirrupT1Curves = new List<Curve>();
                        Curve stirrupT1Line1 = Line.CreateBound(stirrupT1_p1, stirrupT1_p2) as Curve;
                        myStirrupT1Curves.Add(stirrupT1Line1);
                        Curve stirrupT1Line2 = Line.CreateBound(stirrupT1_p2, stirrupT1_p3) as Curve;
                        myStirrupT1Curves.Add(stirrupT1Line2);
                        Curve stirrupT1Line3 = Line.CreateBound(stirrupT1_p3, stirrupT1_p4) as Curve;
                        myStirrupT1Curves.Add(stirrupT1Line3);
                        Curve stirrupT1Line4 = Line.CreateBound(stirrupT1_p4, stirrupT1_p1) as Curve;
                        myStirrupT1Curves.Add(stirrupT1Line4);

                        //Хомут Тест1
                        Rebar stirrupT1 = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupT1
                        , myRebarHookType
                        , myRebarHookType
                        , beam
                        , beamMainLineDirectionVector
                        , myStirrupT1Curves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        stirrupT1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        stirrupT1.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                        stirrupT1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupQuantityL1);
                        stirrupT1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stirrupStepL1);

                        //Точки для построения хомута Тест2
                        XYZ stirrupT2_p1 = requiredTopLineEndPoint
                            - ((50 / 304.8) * beamMainLineDirectionVector)
                            + (rebarLRCoverLayerAsDouble * normal)
                            - (myStirrupT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myStirrupT1Diam / 2 * verticalVectorFromBeamMainLine);
                        XYZ stirrupT2_p2 = stirrupT2_p1
                            + (beamWidth * normal)
                            - ((rebarLRCoverLayerAsDouble * 2) * normal)
                            + (myStirrupT1Diam * normal);
                        XYZ stirrupT2_p3 = stirrupT2_p2
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myStirrupT1Diam * verticalVectorFromBeamMainLine);
                        XYZ stirrupT2_p4 = stirrupT2_p3
                            - (beamWidth * normal)
                            + ((rebarLRCoverLayerAsDouble * 2) * normal)
                            - (myStirrupT1Diam * normal);

                        //Кривые стержня хомута Тест2
                        List<Curve> myStirrupT2Curves = new List<Curve>();
                        Curve stirrupT2Line1 = Line.CreateBound(stirrupT2_p1, stirrupT2_p2) as Curve;
                        myStirrupT2Curves.Add(stirrupT2Line1);
                        Curve stirrupT2Line2 = Line.CreateBound(stirrupT2_p2, stirrupT2_p3) as Curve;
                        myStirrupT2Curves.Add(stirrupT2Line2);
                        Curve stirrupT2Line3 = Line.CreateBound(stirrupT2_p3, stirrupT2_p4) as Curve;
                        myStirrupT2Curves.Add(stirrupT2Line3);
                        Curve stirrupT2Line4 = Line.CreateBound(stirrupT2_p4, stirrupT2_p1) as Curve;
                        myStirrupT2Curves.Add(stirrupT2Line4);

                        //Хомут Тест2
                        Rebar stirrupT2 = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupT1
                        , myRebarHookType
                        , myRebarHookType
                        , beam
                        , beamMainLineDirectionVector
                        , myStirrupT2Curves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        stirrupT2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        stirrupT2.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                        stirrupT2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupQuantityR1);
                        stirrupT2.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stirrupStepR1);


                        double stirrupPlacementLengthC1 = Math.Round((beamLength * 304.8) - 100 - stirrupIndentL1 - stirrupIndentR1);
                        int stirrupQuantityC1 = (int)(stirrupPlacementLengthC1 / (stirrupStepC1 * 304.8));
                        double x = stirrupPlacementLengthC1 - ((stirrupQuantityC1 - 1) * (stirrupStepC1 * 304.8));
                        double stirrupIndentC1 = stirrupIndentL1 / 304.8 + ((x / 2) / 304.8);

                        //Точки для построения хомута Тест3
                        XYZ stirrupC1_p1 = requiredTopLineStartPoint
                            + ((50 / 304.8) * beamMainLineDirectionVector)
                            + (stirrupIndentC1 * beamMainLineDirectionVector)
                            + (rebarLRCoverLayerAsDouble * normal)
                            - (myStirrupT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myStirrupT1Diam / 2 * verticalVectorFromBeamMainLine);
                        XYZ stirrupC1_p2 = stirrupC1_p1
                            + (beamWidth * normal)
                            - ((rebarLRCoverLayerAsDouble * 2) * normal)
                            + (myStirrupT1Diam * normal);
                        XYZ stirrupC1_p3 = stirrupC1_p2
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myStirrupT1Diam * verticalVectorFromBeamMainLine);
                        XYZ stirrupC1_p4 = stirrupC1_p3
                            - (beamWidth * normal)
                            + ((rebarLRCoverLayerAsDouble * 2) * normal)
                            - (myStirrupT1Diam * normal);

                        //Кривые стержня хомута Тест3
                        List<Curve> myStirrupC1Curves = new List<Curve>();
                        Curve stirrupC1Line1 = Line.CreateBound(stirrupC1_p1, stirrupC1_p2) as Curve;
                        myStirrupC1Curves.Add(stirrupC1Line1);
                        Curve stirrupC1Line2 = Line.CreateBound(stirrupC1_p2, stirrupC1_p3) as Curve;
                        myStirrupC1Curves.Add(stirrupC1Line2);
                        Curve stirrupC1Line3 = Line.CreateBound(stirrupC1_p3, stirrupC1_p4) as Curve;
                        myStirrupC1Curves.Add(stirrupC1Line3);
                        Curve stirrupC1Line4 = Line.CreateBound(stirrupC1_p4, stirrupC1_p1) as Curve;
                        myStirrupC1Curves.Add(stirrupC1Line4);

                        //Хомут Тест3
                        Rebar stirrupC1 = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupC1
                        , myRebarHookType
                        , myRebarHookType
                        , beam
                        , beamMainLineDirectionVector
                        , myStirrupC1Curves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        stirrupC1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        stirrupC1.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                        stirrupC1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupQuantityC1);
                        stirrupC1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stirrupStepC1);
                    }

                    //Хомуты при ширине балки больше или равной 350
                    else if (Math.Round(beamWidth * 304.8) >= 350)
                    {
                        //Точки для построения хомута Тест1
                        XYZ stirrupT1_p1 = requiredTopLineStartPoint
                            + ((50 / 304.8) * beamMainLineDirectionVector)
                            + (rebarLRCoverLayerAsDouble * normal)
                            - (myStirrupT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myStirrupT1Diam / 2 * verticalVectorFromBeamMainLine);
                        XYZ stirrupT1_p2 = stirrupT1_p1
                            + (beamWidth * normal)
                            - ((rebarLRCoverLayerAsDouble * 2) * normal)
                            + (myStirrupT1Diam * normal)
                            - (topBarsStepForStirrup * normal)
                            - (myStirrupT1Diam/2 * normal);
                        XYZ stirrupT1_p3 = stirrupT1_p2
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myStirrupT1Diam * verticalVectorFromBeamMainLine);
                        XYZ stirrupT1_p4 = stirrupT1_p3
                            - (beamWidth * normal)
                            + ((rebarLRCoverLayerAsDouble * 2) * normal)
                            - (myStirrupT1Diam * normal)
                            + (topBarsStepForStirrup * normal)
                            + (myStirrupT1Diam/2 * normal);

                        //Точки для построения хомута Тест1A
                        XYZ stirrupT1A_p1 = requiredTopLineStartPoint
                            + ((50 / 304.8) * beamMainLineDirectionVector)
                            + (myStirrupT1Diam * beamMainLineDirectionVector)
                            + (rebarLRCoverLayerAsDouble * normal)
                            - (myStirrupT1Diam * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myStirrupT1Diam / 2 * verticalVectorFromBeamMainLine)
                            + (topBarsStepForStirrup * normal);
                        XYZ stirrupT1A_p2 = stirrupT1A_p1
                            + (beamWidth * normal)
                            - ((rebarLRCoverLayerAsDouble * 2) * normal)
                            + (myStirrupT1Diam * normal)
                            - (topBarsStepForStirrup * normal)
                            + (myStirrupT1Diam /2 * normal);
                        XYZ stirrupT1A_p3 = stirrupT1A_p2
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myStirrupT1Diam * verticalVectorFromBeamMainLine);
                        XYZ stirrupT1A_p4 = stirrupT1A_p3
                            - (beamWidth * normal)
                            + ((rebarLRCoverLayerAsDouble * 2) * normal)
                            - (myStirrupT1Diam * normal)
                            + (topBarsStepForStirrup * normal)
                            - (myStirrupT1Diam / 2 * normal);


                        //Кривые стержня хомута Тест1
                        List<Curve> myStirrupT1Curves = new List<Curve>();
                        Curve stirrupT1Line1 = Line.CreateBound(stirrupT1_p1, stirrupT1_p2) as Curve;
                        myStirrupT1Curves.Add(stirrupT1Line1);
                        Curve stirrupT1Line2 = Line.CreateBound(stirrupT1_p2, stirrupT1_p3) as Curve;
                        myStirrupT1Curves.Add(stirrupT1Line2);
                        Curve stirrupT1Line3 = Line.CreateBound(stirrupT1_p3, stirrupT1_p4) as Curve;
                        myStirrupT1Curves.Add(stirrupT1Line3);
                        Curve stirrupT1Line4 = Line.CreateBound(stirrupT1_p4, stirrupT1_p1) as Curve;
                        myStirrupT1Curves.Add(stirrupT1Line4);

                        //Кривые стержня хомута Тест1A
                        List<Curve> myStirrupT1ACurves = new List<Curve>();
                        Curve stirrupT1ALine1 = Line.CreateBound(stirrupT1A_p1, stirrupT1A_p2) as Curve;
                        myStirrupT1ACurves.Add(stirrupT1ALine1);
                        Curve stirrupT1ALine2 = Line.CreateBound(stirrupT1A_p2, stirrupT1A_p3) as Curve;
                        myStirrupT1ACurves.Add(stirrupT1ALine2);
                        Curve stirrupT1ALine3 = Line.CreateBound(stirrupT1A_p3, stirrupT1A_p4) as Curve;
                        myStirrupT1ACurves.Add(stirrupT1ALine3);
                        Curve stirrupT1ALine4 = Line.CreateBound(stirrupT1A_p4, stirrupT1A_p1) as Curve;
                        myStirrupT1ACurves.Add(stirrupT1ALine4);

                        //Хомут Тест1
                        Rebar stirrupT1 = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupT1
                        , myRebarHookType
                        , myRebarHookType
                        , beam
                        , beamMainLineDirectionVector
                        , myStirrupT1Curves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        stirrupT1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        stirrupT1.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                        stirrupT1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupQuantityL1);
                        stirrupT1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stirrupStepL1);

                        //Хомут Тест1A
                        Rebar stirrupT1A = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupT1
                        , myRebarHookType
                        , myRebarHookType
                        , beam
                        , beamMainLineDirectionVector
                        , myStirrupT1ACurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        stirrupT1A.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        stirrupT1A.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                        stirrupT1A.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupQuantityL1);
                        stirrupT1A.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stirrupStepL1);


                        // Точки для построения хомута Тест2
                        XYZ stirrupT2_p1 = requiredTopLineEndPoint
                            - ((50 / 304.8) * beamMainLineDirectionVector)
                            + (rebarLRCoverLayerAsDouble * normal)
                            - (myStirrupT1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myStirrupT1Diam / 2 * verticalVectorFromBeamMainLine);
                        XYZ stirrupT2_p2 = stirrupT2_p1
                            + (beamWidth * normal)
                            - ((rebarLRCoverLayerAsDouble * 2) * normal)
                            + (myStirrupT1Diam * normal)
                            - (topBarsStepForStirrup * normal)
                            - (myStirrupT1Diam / 2 * normal);
                        XYZ stirrupT2_p3 = stirrupT2_p2
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myStirrupT1Diam * verticalVectorFromBeamMainLine);
                        XYZ stirrupT2_p4 = stirrupT2_p3
                            - (beamWidth * normal)
                            + ((rebarLRCoverLayerAsDouble * 2) * normal)
                            - (myStirrupT1Diam * normal)
                            + (topBarsStepForStirrup * normal)
                            + (myStirrupT1Diam / 2 * normal);

                        // Точки для построения хомута Тест2A
                        XYZ stirrupT2A_p1 = requiredTopLineEndPoint
                            - ((50 / 304.8) * beamMainLineDirectionVector)
                            - (myStirrupT1Diam * beamMainLineDirectionVector)
                            + (rebarLRCoverLayerAsDouble * normal)
                            - (myStirrupT1Diam * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myStirrupT1Diam / 2 * verticalVectorFromBeamMainLine)
                            + (topBarsStepForStirrup * normal);
                        XYZ stirrupT2A_p2 = stirrupT2A_p1
                            + (beamWidth * normal)
                            - ((rebarLRCoverLayerAsDouble * 2) * normal)
                            + (myStirrupT1Diam * normal)
                            - (topBarsStepForStirrup * normal)
                            + (myStirrupT1Diam / 2 * normal);
                        XYZ stirrupT2A_p3 = stirrupT2A_p2
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myStirrupT1Diam * verticalVectorFromBeamMainLine);
                        XYZ stirrupT2A_p4 = stirrupT2A_p3
                            - (beamWidth * normal)
                            + ((rebarLRCoverLayerAsDouble * 2) * normal)
                            - (myStirrupT1Diam * normal)
                            + (topBarsStepForStirrup * normal)
                            - (myStirrupT1Diam / 2 * normal);

                        //Кривые стержня хомута Тест2
                        List<Curve> myStirrupT2Curves = new List<Curve>();
                        Curve stirrupT2Line1 = Line.CreateBound(stirrupT2_p1, stirrupT2_p2) as Curve;
                        myStirrupT2Curves.Add(stirrupT2Line1);
                        Curve stirrupT2Line2 = Line.CreateBound(stirrupT2_p2, stirrupT2_p3) as Curve;
                        myStirrupT2Curves.Add(stirrupT2Line2);
                        Curve stirrupT2Line3 = Line.CreateBound(stirrupT2_p3, stirrupT2_p4) as Curve;
                        myStirrupT2Curves.Add(stirrupT2Line3);
                        Curve stirrupT2Line4 = Line.CreateBound(stirrupT2_p4, stirrupT2_p1) as Curve;
                        myStirrupT2Curves.Add(stirrupT2Line4);

                        //Кривые стержня хомута Тест2A
                        List<Curve> myStirrupT2ACurves = new List<Curve>();
                        Curve stirrupT2ALine1 = Line.CreateBound(stirrupT2A_p1, stirrupT2A_p2) as Curve;
                        myStirrupT2ACurves.Add(stirrupT2ALine1);
                        Curve stirrupT2ALine2 = Line.CreateBound(stirrupT2A_p2, stirrupT2A_p3) as Curve;
                        myStirrupT2ACurves.Add(stirrupT2ALine2);
                        Curve stirrupT2ALine3 = Line.CreateBound(stirrupT2A_p3, stirrupT2A_p4) as Curve;
                        myStirrupT2ACurves.Add(stirrupT2ALine3);
                        Curve stirrupT2ALine4 = Line.CreateBound(stirrupT2A_p4, stirrupT2A_p1) as Curve;
                        myStirrupT2ACurves.Add(stirrupT2ALine4);

                        //Хомут Тест2
                        Rebar stirrupT2 = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupT1
                        , myRebarHookType
                        , myRebarHookType
                        , beam
                        , beamMainLineDirectionVector
                        , myStirrupT2Curves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        stirrupT2.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        stirrupT2.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                        stirrupT2.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupQuantityR1);
                        stirrupT2.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stirrupStepR1);

                        //Хомут Тест2A
                        Rebar stirrupT2A = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupT1
                        , myRebarHookType
                        , myRebarHookType
                        , beam
                        , beamMainLineDirectionVector
                        , myStirrupT2ACurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        stirrupT2A.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        stirrupT2A.GetShapeDrivenAccessor().BarsOnNormalSide = false;
                        stirrupT2A.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupQuantityR1);
                        stirrupT2A.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stirrupStepR1);

                        double stirrupPlacementLengthC1 = Math.Round((beamLength * 304.8) - 100 - stirrupIndentL1 - stirrupIndentR1);
                        int stirrupQuantityC1 = (int)(stirrupPlacementLengthC1 / (stirrupStepC1 * 304.8));
                        double x = stirrupPlacementLengthC1 - ((stirrupQuantityC1 - 1) * (stirrupStepC1 * 304.8));
                        double stirrupIndentC1 = stirrupIndentL1 / 304.8 + ((x / 2) / 304.8);

                        //Точки для построения хомута Тест3
                        XYZ stirrupC1_p1 = requiredTopLineStartPoint
                            + ((50 / 304.8) * beamMainLineDirectionVector)
                            + (stirrupIndentC1 * beamMainLineDirectionVector)
                            + (rebarLRCoverLayerAsDouble * normal)
                            - (myStirrupC1Diam / 2 * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myStirrupC1Diam / 2 * verticalVectorFromBeamMainLine);
                        XYZ stirrupC1_p2 = stirrupC1_p1
                            + (beamWidth * normal)
                            - ((rebarLRCoverLayerAsDouble * 2) * normal)
                            + (myStirrupC1Diam * normal)
                            - (topBarsStepForStirrup * normal)
                            - (myStirrupC1Diam / 2 * normal);
                        XYZ stirrupC1_p3 = stirrupC1_p2
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myStirrupC1Diam * verticalVectorFromBeamMainLine);
                        XYZ stirrupC1_p4 = stirrupC1_p3
                            - (beamWidth * normal)
                            + ((rebarLRCoverLayerAsDouble * 2) * normal)
                            - (myStirrupC1Diam * normal)
                            + (topBarsStepForStirrup * normal)
                            + (myStirrupC1Diam / 2 * normal);

                        //Точки для построения хомута Тест3A
                        XYZ stirrupC1A_p1 = requiredTopLineStartPoint
                            + ((50 / 304.8) * beamMainLineDirectionVector)
                            + (stirrupIndentC1 * beamMainLineDirectionVector)
                            + (myStirrupC1Diam * beamMainLineDirectionVector)
                            + (rebarLRCoverLayerAsDouble * normal)
                            - (myStirrupC1Diam * normal)
                            + (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (myStirrupC1Diam / 2 * verticalVectorFromBeamMainLine)
                            + (topBarsStepForStirrup * normal); ;
                        XYZ stirrupC1A_p2 = stirrupC1A_p1
                            + (beamWidth * normal)
                            - ((rebarLRCoverLayerAsDouble * 2) * normal)
                            + (myStirrupC1Diam * normal)
                            - (topBarsStepForStirrup * normal)
                            + (myStirrupC1Diam / 2 * normal); ;
                        XYZ stirrupC1A_p3 = stirrupC1A_p2
                            + (beamHeight * verticalVectorFromBeamMainLine)
                            - (rebarTopCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            - (rebarBottomCoverLayerAsDouble * verticalVectorFromBeamMainLine)
                            + (myStirrupC1Diam * verticalVectorFromBeamMainLine);
                        XYZ stirrupC1A_p4 = stirrupC1A_p3
                            - (beamWidth * normal)
                            + ((rebarLRCoverLayerAsDouble * 2) * normal)
                            - (myStirrupC1Diam * normal)
                            + (topBarsStepForStirrup * normal)
                            - (myStirrupC1Diam / 2 * normal); ;

                        //Кривые стержня хомута Тест3
                        List<Curve> myStirrupC1Curves = new List<Curve>();
                        Curve stirrupC1Line1 = Line.CreateBound(stirrupC1_p1, stirrupC1_p2) as Curve;
                        myStirrupC1Curves.Add(stirrupC1Line1);
                        Curve stirrupC1Line2 = Line.CreateBound(stirrupC1_p2, stirrupC1_p3) as Curve;
                        myStirrupC1Curves.Add(stirrupC1Line2);
                        Curve stirrupC1Line3 = Line.CreateBound(stirrupC1_p3, stirrupC1_p4) as Curve;
                        myStirrupC1Curves.Add(stirrupC1Line3);
                        Curve stirrupC1Line4 = Line.CreateBound(stirrupC1_p4, stirrupC1_p1) as Curve;
                        myStirrupC1Curves.Add(stirrupC1Line4);

                        //Кривые стержня хомута Тест3A
                        List<Curve> myStirrupC1ACurves = new List<Curve>();
                        Curve stirrupC1ALine1 = Line.CreateBound(stirrupC1A_p1, stirrupC1A_p2) as Curve;
                        myStirrupC1ACurves.Add(stirrupC1ALine1);
                        Curve stirrupC1ALine2 = Line.CreateBound(stirrupC1A_p2, stirrupC1A_p3) as Curve;
                        myStirrupC1ACurves.Add(stirrupC1ALine2);
                        Curve stirrupC1ALine3 = Line.CreateBound(stirrupC1A_p3, stirrupC1A_p4) as Curve;
                        myStirrupC1ACurves.Add(stirrupC1ALine3);
                        Curve stirrupC1ALine4 = Line.CreateBound(stirrupC1A_p4, stirrupC1A_p1) as Curve;
                        myStirrupC1ACurves.Add(stirrupC1ALine4);

                        //Хомут Тест3
                        Rebar stirrupC1 = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupC1
                        , myRebarHookType
                        , myRebarHookType
                        , beam
                        , beamMainLineDirectionVector
                        , myStirrupC1Curves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        stirrupC1.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        stirrupC1.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                        stirrupC1.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupQuantityC1);
                        stirrupC1.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stirrupStepC1);

                        //Хомут Тест3
                        Rebar stirrupC1A = Rebar.CreateFromCurvesAndShape(doc
                        , myStirrupRebarShape
                        , myStirrupC1
                        , myRebarHookType
                        , myRebarHookType
                        , beam
                        , beamMainLineDirectionVector
                        , myStirrupC1ACurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);

                        stirrupC1A.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                        stirrupC1A.GetShapeDrivenAccessor().BarsOnNormalSide = true;
                        stirrupC1A.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(stirrupQuantityC1);
                        stirrupC1A.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(stirrupStepC1);
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
