using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class GloryHole : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Переменная для связанного файла
            Document linkDoc = null;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Выбор связанного файла
            RevitLinkInstanceSelectionFilter selFilterRevitLinkInstance = new RevitLinkInstanceSelectionFilter();
            Reference selRevitLinkInstance = null;
            try
            {
                selRevitLinkInstance = sel.PickObject(ObjectType.Element, selFilterRevitLinkInstance, "Выберите связанный файл!");
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
                        
            IEnumerable<RevitLinkInstance> revitLinkInstance = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance))
                .Where(li => li.Id == selRevitLinkInstance.ElementId)
                .Cast<RevitLinkInstance>();
            if(revitLinkInstance.Count() == 0)
            {
                TaskDialog.Show("Ravit", "Связанный файл не найден!");
                return Result.Cancelled;
            }
            linkDoc = revitLinkInstance.First().GetLinkDocument();
            Transform transform = revitLinkInstance.First().GetTotalTransform();

            //Получение стен из связанного файла
            List<Wall> wallsInLinkList = new FilteredElementCollector(linkDoc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .OfClass(typeof(Wall))
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .Where(w => w.CurtainGrid == null)
                .ToList();
            //Получение перекрытий из связанного файла
            List<Floor> floorsInLinkList = new FilteredElementCollector(linkDoc)
                .OfCategory(BuiltInCategory.OST_Floors)
                .OfClass(typeof(Floor))
                .WhereElementIsNotElementType()
                .Cast<Floor>()
                .Where(f => f.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).AsInteger() == 1)
                .ToList();

            //Получение трубопроводов и воздуховодов
            List<Pipe> pipesList = new List<Pipe>();
            List<Duct> ductsList = new List<Duct>();
            //Выбор трубы или воздуховода
            PipeDuctSelectionFilter pipeDuctSelectionFilter = new PipeDuctSelectionFilter();
            IList<Reference> pipeDuctRefList = null;
            try
            {
                pipeDuctRefList = sel.PickObjects(ObjectType.Element, pipeDuctSelectionFilter, "Выберите трубу или воздуховод!");
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            foreach (Reference refElem in pipeDuctRefList)
            {
                if (doc.GetElement(refElem).Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_PipeCurves))
                {
                    pipesList.Add((doc.GetElement(refElem) as Pipe));
                }
                else if (doc.GetElement(refElem).Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_DuctCurves))
                {
                    ductsList.Add((doc.GetElement(refElem)) as Duct);
                }
            }

            //Типы семейств для обозначения проемов
            FamilySymbol intersectionPointRectangularWallFamilySymbol = null;
            FamilySymbol intersectionPointRectangularFloorFamilySymbol = null;
            List<FamilySymbol> intersectionPointRectangularWallFamilySymbolList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Where(fs => fs.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Стена")
                .ToList();
            if(intersectionPointRectangularWallFamilySymbolList.Count != 0)
            {
                intersectionPointRectangularWallFamilySymbol = intersectionPointRectangularWallFamilySymbolList.First();
            }
            else
            {
                TaskDialog.Show("Revit", "Семейство \"CIT_00_Точка пересечения_Прямоугольная_Стена\" не найдено!");
                return Result.Cancelled;
            }
            List<FamilySymbol> intersectionPointRectangularFloorFamilySymbolList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Where(fs => fs.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Плита")
                .ToList();
            if (intersectionPointRectangularFloorFamilySymbolList.Count != 0)
            {
                intersectionPointRectangularFloorFamilySymbol = intersectionPointRectangularFloorFamilySymbolList.First();
            }
            else
            {
                TaskDialog.Show("Revit", "Семейство \"CIT_00_Точка пересечения_Прямоугольная_Плита\" не найдено!");
                return Result.Cancelled;
            }
            Guid intersectionPointWidthGuid = new Guid("8f2e4f93-9472-4941-a65d-0ac468fd6a5d");
            Guid intersectionPointHeightGuid = new Guid("da753fe3-ecfa-465b-9a2c-02f55d0c2ff1");
            Guid intersectionPointThicknessGuid = new Guid("293f055d-6939-4611-87b7-9a50d0c1f50e");

            Guid heightOfBaseLevelGuid = new Guid("9f5f7e49-616e-436f-9acc-5305f34b6933");
            Guid levelOffsetGuid = new Guid("515dc061-93ce-40e4-859a-e29224d80a10");

            //Вызов формы
            GloryHoleWPF gloryHoleWPF = new GloryHoleWPF();
            gloryHoleWPF.ShowDialog();
            if (gloryHoleWPF.DialogResult != true)
            {
                return Result.Cancelled;
            }
            double pipeSideClearance = (gloryHoleWPF.PipeSideClearance * 2) / 304.8;
            double pipeTopBottomClearance = (gloryHoleWPF.PipeTopBottomClearance * 2) / 304.8;
            double ductSideClearance = (gloryHoleWPF.DuctSideClearance * 2) / 304.8;
            double ductTopBottomClearance = (gloryHoleWPF.DuctTopBottomClearance * 2) / 304.8;
            double roundUpIncrement = gloryHoleWPF.RoundUpIncrement;
            bool mergeHoles = gloryHoleWPF.MergeHoles;

            List<FamilyInstance> pipeWallIntersectionPointList = new List<FamilyInstance>();
            List<FamilyInstance> pipeFloorIntersectionPointList = new List<FamilyInstance>();
            BasePoint basePoint = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_ProjectBasePoint)
                .WhereElementIsNotElementType()
                .Cast<BasePoint>()
                .First();
            double basePointZ = basePoint.SharedPosition.Z;
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение точек пересечения");
                intersectionPointRectangularWallFamilySymbol.Activate();
                intersectionPointRectangularFloorFamilySymbol.Activate();
                Options opt = new Options();
                opt.ComputeReferences = true;
                opt.DetailLevel = ViewDetailLevel.Fine;

                //Обработка стен
                foreach (Wall wall in wallsInLinkList)
                {
                    GeometryElement geomElem = wall.get_Geometry(opt);
                    foreach (GeometryObject geomObj in geomElem)
                    {
                        Solid geomSolid = geomObj as Solid;
                        if (null != geomSolid)
                        {
                            Solid transformGeomSolid = SolidUtils.CreateTransformed(geomSolid, transform);
                            foreach (Pipe pipe in pipesList)
                            {
                                Curve pipeCurve = (pipe.Location as LocationCurve).Curve;
                                SolidCurveIntersectionOptions scio = new SolidCurveIntersectionOptions();
                                SolidCurveIntersection intersection = transformGeomSolid.IntersectWithCurve(pipeCurve, scio);
                                if (intersection.SegmentCount > 0)
                                {
                                    XYZ wallOrientation = wall.Orientation;
                                    double pipeDiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble();
                                    double intersectionPointHeight = RoundUpToIncrement(pipeDiameter + pipeTopBottomClearance, roundUpIncrement);
                                    double intersectionPointThickness = RoundUpToIncrement(wall.Width, 1);

                                    double a = Math.Round((wallOrientation.AngleTo((pipeCurve as Line).Direction)) * (180 / Math.PI), 6);

                                    if (a > 90 && a < 180)
                                    {
                                        a = (180 - a) * (Math.PI / 180);
                                    }
                                    else
                                    {
                                        a = a * (Math.PI / 180);
                                    }
                                    double delta1 = Math.Abs((wall.Width / 2) * Math.Tan(a));
                                    double delta2 = Math.Abs((pipeDiameter / 2) / Math.Cos(a));
                                    if (delta1 >= 9.84251968504 || delta2 >= 9.84251968504) continue;

                                    Level lvl = null;
                                    lvl = GetClosestBottomWallLevel(doc, linkDoc, wall);

                                    XYZ intersectionCurveStartPoint = intersection.GetCurveSegment(0).GetEndPoint(0);
                                    XYZ intersectionCurveEndPoint = intersection.GetCurveSegment(0).GetEndPoint(1);
                                    XYZ originIntersectionCurve = ((intersectionCurveStartPoint + intersectionCurveEndPoint) / 2) - (intersectionPointHeight / 2) * XYZ.BasisZ;
                                    originIntersectionCurve = new XYZ(originIntersectionCurve.X, originIntersectionCurve.Y, RoundToIncrement(originIntersectionCurve.Z, 10) - (lvl.Elevation) - basePointZ);

                                   FamilyInstance intersectionPoint = doc.Create.NewFamilyInstance(originIntersectionCurve
                                        , intersectionPointRectangularWallFamilySymbol
                                        , lvl
                                        , StructuralType.NonStructural) as FamilyInstance;
                                    if (Math.Round(wallOrientation.AngleTo(intersectionPoint.FacingOrientation), 6) != 0)
                                    {
                                        Line rotationLine = Line.CreateBound(originIntersectionCurve, originIntersectionCurve + 1 * XYZ.BasisZ);
                                        ElementTransformUtils.RotateElement(doc, intersectionPoint.Id, rotationLine, wallOrientation.AngleTo(intersectionPoint.FacingOrientation));
                                    }
                                    intersectionPoint.get_Parameter(intersectionPointWidthGuid).Set(RoundUpToIncrement(delta1 * 2 + delta2 * 2 + pipeSideClearance, roundUpIncrement));
                                    intersectionPoint.get_Parameter(intersectionPointHeightGuid).Set(intersectionPointHeight);
                                    intersectionPoint.get_Parameter(intersectionPointThicknessGuid).Set(intersectionPointThickness);

                                    intersectionPoint.get_Parameter(heightOfBaseLevelGuid).Set((doc.GetElement(intersectionPoint.LevelId) as Level).Elevation);
                                    intersectionPoint.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(originIntersectionCurve.Z);
                                    intersectionPoint.get_Parameter(levelOffsetGuid).Set(originIntersectionCurve.Z);
                                    pipeWallIntersectionPointList.Add(intersectionPoint);
                                }
                            }

                            foreach (Duct duct in ductsList)
                            {
                                Curve ductCurve = (duct.Location as LocationCurve).Curve;
                                SolidCurveIntersectionOptions scio = new SolidCurveIntersectionOptions();
                                SolidCurveIntersection intersection = transformGeomSolid.IntersectWithCurve(ductCurve, scio);
                                if (intersection.SegmentCount > 0)
                                {
                                    XYZ wallOrientation = wall.Orientation;
                                    if (duct.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM) != null)
                                    {
                                        double ductDiameter = duct.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM).AsDouble();
                                        double intersectionPointHeight = RoundUpToIncrement(ductDiameter + ductTopBottomClearance, roundUpIncrement);
                                        double intersectionPointThickness = RoundUpToIncrement(wall.Width, 1);

                                        double a = Math.Round((wallOrientation.AngleTo((ductCurve as Line).Direction)) * (180 / Math.PI), 6);

                                        if (a > 90 && a < 180)
                                        {
                                            a = (180 - a) * (Math.PI / 180);
                                        }
                                        else
                                        {
                                            a = a * (Math.PI / 180);
                                        }
                                        double delta1 = Math.Abs((wall.Width / 2) * Math.Tan(a));
                                        double delta2 = Math.Abs((ductDiameter / 2) / Math.Cos(a));
                                        if (delta1 >= 9.84251968504 || delta2 >= 9.84251968504) continue;

                                        Level lvl = null;
                                        lvl = GetClosestBottomWallLevel(doc, linkDoc, wall);

                                        XYZ intersectionCurveStartPoint = intersection.GetCurveSegment(0).GetEndPoint(0);
                                        XYZ intersectionCurveEndPoint = intersection.GetCurveSegment(0).GetEndPoint(1);
                                        XYZ originIntersectionCurve = ((intersectionCurveStartPoint + intersectionCurveEndPoint) / 2) - (intersectionPointHeight / 2) * XYZ.BasisZ;
                                        originIntersectionCurve = new XYZ(originIntersectionCurve.X, originIntersectionCurve.Y, RoundToIncrement(originIntersectionCurve.Z, 10) - (lvl.Elevation) - basePointZ);

                                        FamilyInstance intersectionPoint = doc.Create.NewFamilyInstance(originIntersectionCurve
                                            , intersectionPointRectangularWallFamilySymbol
                                            , lvl
                                            , StructuralType.NonStructural) as FamilyInstance;
                                        if (Math.Round(wallOrientation.AngleTo(intersectionPoint.FacingOrientation), 6) != 0)
                                        {
                                            Line rotationLine = Line.CreateBound(originIntersectionCurve, originIntersectionCurve + 1 * XYZ.BasisZ);
                                            ElementTransformUtils.RotateElement(doc, intersectionPoint.Id, rotationLine, wallOrientation.AngleTo(intersectionPoint.FacingOrientation));
                                        }
                                        intersectionPoint.get_Parameter(intersectionPointWidthGuid).Set(RoundUpToIncrement(delta1 * 2 + delta2 * 2 + ductSideClearance, 50));
                                        intersectionPoint.get_Parameter(intersectionPointHeightGuid).Set(intersectionPointHeight);
                                        intersectionPoint.get_Parameter(intersectionPointThicknessGuid).Set(intersectionPointThickness);

                                        intersectionPoint.get_Parameter(heightOfBaseLevelGuid).Set((doc.GetElement(intersectionPoint.LevelId) as Level).Elevation);
                                        intersectionPoint.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(originIntersectionCurve.Z);
                                        intersectionPoint.get_Parameter(levelOffsetGuid).Set(originIntersectionCurve.Z);
                                    }
                                    else
                                    {
                                        double ductHeight = duct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM).AsDouble();
                                        double ductWidth = duct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM).AsDouble();
                                        double intersectionPointHeight = RoundUpToIncrement(ductHeight + ductTopBottomClearance, roundUpIncrement);
                                        double intersectionPointThickness = RoundUpToIncrement(wall.Width, 1);

                                        double a = Math.Round((wallOrientation.AngleTo((ductCurve as Line).Direction)) * (180 / Math.PI), 6);

                                        if (a > 90 && a < 180)
                                        {
                                            a = (180 - a) * (Math.PI / 180);
                                        }
                                        else
                                        {
                                            a = a * (Math.PI / 180);
                                        }

                                        double delta1 = Math.Abs((wall.Width / 2) * Math.Tan(a));
                                        double delta2 = Math.Abs((ductWidth / 2) / Math.Cos(a));
                                        if (delta1 >= 9.84251968504 || delta2 >= 9.84251968504) continue;

                                        Level lvl = null;
                                        lvl = GetClosestBottomWallLevel(doc, linkDoc, wall);

                                        XYZ intersectionCurveStartPoint = intersection.GetCurveSegment(0).GetEndPoint(0);
                                        XYZ intersectionCurveEndPoint = intersection.GetCurveSegment(0).GetEndPoint(1);
                                        XYZ originIntersectionCurve = ((intersectionCurveStartPoint + intersectionCurveEndPoint) / 2) - (intersectionPointHeight / 2) * XYZ.BasisZ;
                                        originIntersectionCurve = new XYZ(originIntersectionCurve.X, originIntersectionCurve.Y, RoundToIncrement(originIntersectionCurve.Z, 10) - (lvl.Elevation) - basePointZ);

                                        FamilyInstance intersectionPoint = doc.Create.NewFamilyInstance(originIntersectionCurve
                                            , intersectionPointRectangularWallFamilySymbol
                                            , lvl
                                            , StructuralType.NonStructural) as FamilyInstance;
                                        if (Math.Round(wallOrientation.AngleTo(intersectionPoint.FacingOrientation), 6) != 0)
                                        {
                                            Line rotationLine = Line.CreateBound(originIntersectionCurve, originIntersectionCurve + 1 * XYZ.BasisZ);
                                            ElementTransformUtils.RotateElement(doc, intersectionPoint.Id, rotationLine, wallOrientation.AngleTo(intersectionPoint.FacingOrientation));
                                        }
                                        intersectionPoint.get_Parameter(intersectionPointWidthGuid).Set(RoundUpToIncrement(delta1 * 2 + delta2 * 2 + ductSideClearance, roundUpIncrement));
                                        intersectionPoint.get_Parameter(intersectionPointHeightGuid).Set(intersectionPointHeight);
                                        intersectionPoint.get_Parameter(intersectionPointThicknessGuid).Set(intersectionPointThickness);

                                        intersectionPoint.get_Parameter(heightOfBaseLevelGuid).Set((doc.GetElement(intersectionPoint.LevelId) as Level).Elevation);
                                        intersectionPoint.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(originIntersectionCurve.Z);
                                        intersectionPoint.get_Parameter(levelOffsetGuid).Set(originIntersectionCurve.Z);
                                    }
                                }
                            }
                        }
                    }
                }

                //Обработка перекрытий
                foreach (Floor floor in floorsInLinkList)
                {
                    GeometryElement geomElem = floor.get_Geometry(opt);
                    foreach (GeometryObject geomObj in geomElem)
                    {
                        Solid geomSolid = geomObj as Solid;
                        if (null != geomSolid)
                        {
                            Solid transformGeomSolid = SolidUtils.CreateTransformed(geomSolid, transform);
                            foreach (Pipe pipe in pipesList)
                            {
                                Curve pipeCurve = (pipe.Location as LocationCurve).Curve;
                                SolidCurveIntersectionOptions scio = new SolidCurveIntersectionOptions();
                                SolidCurveIntersection intersection = transformGeomSolid.IntersectWithCurve(pipeCurve, scio);
                                if (intersection.SegmentCount > 0)
                                {
                                    double pipeDiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble();
                                    double intersectionPointHeight = RoundUpToIncrement(pipeDiameter + pipeTopBottomClearance, roundUpIncrement);
                                    double intersectionPointWidth = RoundUpToIncrement(pipeDiameter + pipeSideClearance, roundUpIncrement);
                                    double intersectionPointThickness = RoundToIncrement(floor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble() + 100 / 304.8, 10);

                                    XYZ intersectionCurveStartPoint = intersection.GetCurveSegment(0).GetEndPoint(0);
                                    XYZ intersectionCurveEndPoint = intersection.GetCurveSegment(0).GetEndPoint(1);
                                    XYZ originIntersectionCurve = null;

                                    if (intersectionCurveStartPoint.Z > intersectionCurveEndPoint.Z)
                                    {
                                        originIntersectionCurve = intersectionCurveStartPoint;
                                    }
                                    else
                                    {
                                        originIntersectionCurve = intersectionCurveEndPoint;
                                    }
                                    if(Math.Round(originIntersectionCurve.Z,6) >= 0)
                                    {
                                        originIntersectionCurve = new XYZ(originIntersectionCurve.X, originIntersectionCurve.Y, 50 / 304.8);
                                    }
                                    else
                                    {
                                        originIntersectionCurve = new XYZ(originIntersectionCurve.X, originIntersectionCurve.Y, - 50 / 304.8);
                                    }

                                    Level lvl = null;
                                    lvl = GetClosestBottomFloorLevel(doc, linkDoc, floor);

                                    FamilyInstance intersectionPoint = doc.Create.NewFamilyInstance(originIntersectionCurve
                                        , intersectionPointRectangularFloorFamilySymbol
                                        , lvl
                                        , StructuralType.NonStructural) as FamilyInstance;
                                    intersectionPoint.get_Parameter(intersectionPointWidthGuid).Set(intersectionPointWidth);
                                    intersectionPoint.get_Parameter(intersectionPointHeightGuid).Set(intersectionPointHeight);
                                    intersectionPoint.get_Parameter(intersectionPointThicknessGuid).Set(intersectionPointThickness);
                                    intersectionPoint.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM)
                                        .Set(intersectionPoint.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).AsDouble()
                                        + floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble());
                                    pipeFloorIntersectionPointList.Add(intersectionPoint);
                                }
                            }

                            foreach (Duct duct in ductsList)
                            {
                                Curve ductCurve = (duct.Location as LocationCurve).Curve;
                                SolidCurveIntersectionOptions scio = new SolidCurveIntersectionOptions();
                                SolidCurveIntersection intersection = transformGeomSolid.IntersectWithCurve(ductCurve, scio);
                                if (intersection.SegmentCount > 0)
                                {
                                    XYZ intersectionCurveStartPoint = intersection.GetCurveSegment(0).GetEndPoint(0);
                                    XYZ intersectionCurveEndPoint = intersection.GetCurveSegment(0).GetEndPoint(1);
                                    XYZ originIntersectionCurve = null;

                                    if (intersectionCurveStartPoint.Z > intersectionCurveEndPoint.Z)
                                    {
                                        originIntersectionCurve = intersectionCurveStartPoint;
                                    }
                                    else
                                    {
                                        originIntersectionCurve = intersectionCurveEndPoint;
                                    }
                                    if (Math.Round(originIntersectionCurve.Z, 6) >= 0)
                                    {
                                        originIntersectionCurve = new XYZ(originIntersectionCurve.X, originIntersectionCurve.Y, 50 / 304.8);
                                    }
                                    else
                                    {
                                        originIntersectionCurve = new XYZ(originIntersectionCurve.X, originIntersectionCurve.Y, -50 / 304.8);
                                    }

                                    if (duct.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM) != null)
                                    {
                                        double ductDiameter = duct.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM).AsDouble();
                                        double intersectionPointHeight = RoundUpToIncrement(ductDiameter + ductTopBottomClearance, roundUpIncrement);
                                        double intersectionPointWidth = RoundUpToIncrement(ductDiameter + ductSideClearance, roundUpIncrement);
                                        double intersectionPointThickness = RoundToIncrement(floor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble() + 100 / 304.8, 10);

                                        Level lvl = null;
                                        lvl = GetClosestBottomFloorLevel(doc, linkDoc, floor);

                                        FamilyInstance intersectionPoint = doc.Create.NewFamilyInstance(originIntersectionCurve
                                            , intersectionPointRectangularFloorFamilySymbol
                                            , lvl
                                            , StructuralType.NonStructural) as FamilyInstance;
                                        intersectionPoint.get_Parameter(intersectionPointWidthGuid).Set(intersectionPointWidth);
                                        intersectionPoint.get_Parameter(intersectionPointHeightGuid).Set(intersectionPointHeight);
                                        intersectionPoint.get_Parameter(intersectionPointThicknessGuid).Set(intersectionPointThickness);
                                        intersectionPoint.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM)
                                            .Set(intersectionPoint.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).AsDouble()
                                            + floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble());
                                    }
                                    else
                                    {
                                        double ductHeight = duct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM).AsDouble();
                                        double ductWidth = duct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM).AsDouble();
                                        double intersectionPointHeight = RoundUpToIncrement(ductHeight + ductTopBottomClearance, roundUpIncrement);
                                        double intersectionPointWidth = RoundUpToIncrement(ductWidth + ductSideClearance, roundUpIncrement);
                                        double intersectionPointThickness = RoundToIncrement(floor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble() + 100 / 304.8, 10);
                                        double ductRotationAngle = GetAngleFromMEPCurve(duct as MEPCurve);

                                        Level lvl = null;
                                        lvl = GetClosestBottomFloorLevel(doc, linkDoc, floor);

                                        FamilyInstance intersectionPoint = doc.Create.NewFamilyInstance(originIntersectionCurve
                                            , intersectionPointRectangularFloorFamilySymbol
                                            , lvl
                                            , StructuralType.NonStructural) as FamilyInstance;
                                        intersectionPoint.get_Parameter(intersectionPointWidthGuid).Set(intersectionPointWidth);
                                        intersectionPoint.get_Parameter(intersectionPointHeightGuid).Set(intersectionPointHeight);
                                        intersectionPoint.get_Parameter(intersectionPointThicknessGuid).Set(intersectionPointThickness);
                                        intersectionPoint.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM)
                                            .Set(intersectionPoint.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).AsDouble()
                                            + floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble());

                                        if (ductRotationAngle != 0)
                                        {
                                            Line rotationAxis = Line.CreateBound(originIntersectionCurve, originIntersectionCurve + 1 * XYZ.BasisZ);
                                            ElementTransformUtils.RotateElement(doc
                                                , intersectionPoint.Id
                                                , rotationAxis
                                                , ductRotationAngle);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                t.Commit();
            }
            if (pipeWallIntersectionPointList.Count != 0 && mergeHoles)
            {
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Объединение точек пересечения со стенами");
                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    opt.DetailLevel = ViewDetailLevel.Fine;

                    List<FamilyInstance> pipeIntersectionPointGroup = new List<FamilyInstance>();
                    pipeIntersectionPointGroup.Add(pipeWallIntersectionPointList[0]);
                    pipeWallIntersectionPointList.Remove(pipeWallIntersectionPointList[0]);
                    List<FamilyInstance> tempPipeIntersectionPointList = pipeWallIntersectionPointList.ToList();

                    while (pipeWallIntersectionPointList.Count != 0)
                    {
                        for (int i = 0; i < pipeIntersectionPointGroup.Count;)
                        {
                            Solid solidA = GetSolidFromIntersectionPoint(opt, pipeIntersectionPointGroup[i]);
                            for (int j = 0; j < tempPipeIntersectionPointList.Count;)
                            {
                                Solid solidB = GetSolidFromIntersectionPoint(opt, tempPipeIntersectionPointList[j]);
                                double intersectvolume = Math.Round(BooleanOperationsUtils.ExecuteBooleanOperation(solidA, solidB, BooleanOperationsType.Intersect).Volume, 6);
                                if (intersectvolume > 0)
                                {
                                    pipeIntersectionPointGroup.Add(tempPipeIntersectionPointList[j]);
                                    tempPipeIntersectionPointList.Remove(tempPipeIntersectionPointList[j]);
                                    i = -1;
                                    j++;
                                }
                                else
                                {
                                    j++;
                                }
                            }
                            i++;
                        }

                        if (pipeIntersectionPointGroup.Count > 1)
                        {
                            //Средняя точка
                            double pointsSquareSummX = 0;
                            double pointsSquareSummY = 0;
                            double pointsSquareSummZ = 0;
                            foreach (FamilyInstance p in pipeIntersectionPointGroup)
                            {
                                pointsSquareSummX = pointsSquareSummX + (p.Location as LocationPoint).Point.X;
                                pointsSquareSummY = pointsSquareSummY + (p.Location as LocationPoint).Point.Y;
                                pointsSquareSummZ = pointsSquareSummZ + (p.Location as LocationPoint).Point.Z;
                            }
                            double pointCenterX = pointsSquareSummX / pipeIntersectionPointGroup.Count;
                            double pointCenterY = pointsSquareSummY / pipeIntersectionPointGroup.Count;
                            double pointCenterZ = RoundToIncrement(pointsSquareSummZ / pipeIntersectionPointGroup.Count, 10);

                            //Размер проема
                            double pipeIntersectionRightFaceDistance = 0;
                            double pipeIntersectionLeftFaceDistance = 0;
                            double pipeIntersectionTopFaceDistance = 0;
                            double pipeIntersectionBottomFaceDistance = 0;
                            double intersectionPointThickness = 0;
                            XYZ intersectionPointOrientation = pipeIntersectionPointGroup.First().FacingOrientation;

                            foreach (FamilyInstance p in pipeIntersectionPointGroup)
                            {
                                XYZ pipeIntersectionLocationPoint = (p.Location as LocationPoint).Point;
                                double pipeIntersectionWidth = p.get_Parameter(intersectionPointWidthGuid).AsDouble();
                                double pipeIntersectionHeight = p.get_Parameter(intersectionPointHeightGuid).AsDouble();
                                XYZ pointGravityCenter = new XYZ(pointCenterX, pointCenterY, (p.Location as LocationPoint).Point.Z);

                                XYZ pipeIntersectionRightFaceDistancePoint = pipeIntersectionLocationPoint + (pipeIntersectionWidth / 2) * p.HandOrientation;
                                XYZ pipeIntersectionLeftFaceDistancePoint = pipeIntersectionLocationPoint + (pipeIntersectionWidth / 2) * p.HandOrientation.Negate();

                                XYZ pipeIntersectionTopFaceDistancePoint = pipeIntersectionLocationPoint + (pipeIntersectionHeight / 2) * XYZ.BasisZ;
                                XYZ pipeIntersectionBottomFaceDistancePoint = pipeIntersectionLocationPoint + (pipeIntersectionHeight / 2) * XYZ.BasisZ.Negate();


                                Curve pipeIntersectionRightFaceCurve = Line.CreateBound(pipeIntersectionRightFaceDistancePoint + 50 * XYZ.BasisZ, pipeIntersectionRightFaceDistancePoint + 50 * XYZ.BasisZ.Negate()) as Curve;
                                Curve pipeIntersectionLeftFaceCurve = Line.CreateBound(pipeIntersectionLeftFaceDistancePoint + 50 * XYZ.BasisZ, pipeIntersectionLeftFaceDistancePoint + 50 * XYZ.BasisZ.Negate()) as Curve;

                                Curve pipeIntersectionTopFaceCurve = Line.CreateBound(pipeIntersectionTopFaceDistancePoint + 50 * p.HandOrientation, pipeIntersectionTopFaceDistancePoint + 50 * p.HandOrientation.Negate()) as Curve;
                                Curve pipeIntersectionBottomFaceCurve = Line.CreateBound(pipeIntersectionBottomFaceDistancePoint + 50 * p.HandOrientation, pipeIntersectionBottomFaceDistancePoint + 50 * p.HandOrientation.Negate()) as Curve;

                                IntersectionResultArray ira = new IntersectionResultArray();
                                SetComparisonResult scr = new SetComparisonResult();

                                scr = (Line.CreateBound(pointGravityCenter, pointGravityCenter + 50 * p.HandOrientation) as Curve).Intersect(pipeIntersectionRightFaceCurve, out ira);
                                if (scr == SetComparisonResult.Overlap)
                                {
                                    double tmpPipeIntersectionRightFaceDistance = pointGravityCenter.DistanceTo(ira.get_Item(0).XYZPoint);
                                    if (tmpPipeIntersectionRightFaceDistance > pipeIntersectionRightFaceDistance)
                                    {
                                        pipeIntersectionRightFaceDistance = tmpPipeIntersectionRightFaceDistance;
                                    }
                                }

                                scr = (Line.CreateBound(pointGravityCenter, pointGravityCenter + 50 * p.HandOrientation.Negate()) as Curve).Intersect(pipeIntersectionLeftFaceCurve, out ira);
                                if (scr == SetComparisonResult.Overlap)
                                {
                                    double tmpPipeIntersectionLeftFaceDistance = pointGravityCenter.DistanceTo(ira.get_Item(0).XYZPoint);
                                    if (tmpPipeIntersectionLeftFaceDistance > pipeIntersectionLeftFaceDistance)
                                    {
                                        pipeIntersectionLeftFaceDistance = tmpPipeIntersectionLeftFaceDistance;
                                    }
                                }

                                scr = (Line.CreateBound(pointGravityCenter, pointGravityCenter + 50 * XYZ.BasisZ) as Curve).Intersect(pipeIntersectionTopFaceCurve, out ira);
                                if (scr == SetComparisonResult.Overlap)
                                {
                                    double tmpPipeIntersectionTopFaceDistance = pointGravityCenter.DistanceTo(ira.get_Item(0).XYZPoint);
                                    if (tmpPipeIntersectionTopFaceDistance > pipeIntersectionTopFaceDistance)
                                    {
                                        pipeIntersectionTopFaceDistance = tmpPipeIntersectionTopFaceDistance;
                                    }
                                }

                                scr = (Line.CreateBound(pointGravityCenter, pointGravityCenter + 50 * XYZ.BasisZ.Negate()) as Curve).Intersect(pipeIntersectionBottomFaceCurve, out ira);
                                if (scr == SetComparisonResult.Overlap)
                                {
                                    double tmpPipeIntersectionBottomFaceDistance = pointGravityCenter.DistanceTo(ira.get_Item(0).XYZPoint);
                                    if (tmpPipeIntersectionBottomFaceDistance > pipeIntersectionBottomFaceDistance)
                                    {
                                        pipeIntersectionBottomFaceDistance = tmpPipeIntersectionBottomFaceDistance;
                                    }
                                }

                                if (p.get_Parameter(intersectionPointThicknessGuid).AsDouble() > intersectionPointThickness)
                                {
                                    intersectionPointThickness = p.get_Parameter(intersectionPointThicknessGuid).AsDouble();
                                }
                            }
                            XYZ pointCenterResult = new XYZ(pointCenterX
                                , pointCenterY
                                , pointCenterZ - (doc.GetElement(pipeIntersectionPointGroup.First().LevelId) as Level).Elevation - basePointZ);

                            FamilyInstance intersectionPoint = doc.Create.NewFamilyInstance(pointCenterResult
                            , intersectionPointRectangularWallFamilySymbol
                            , doc.GetElement(pipeIntersectionPointGroup.First().LevelId) as Level
                            , StructuralType.NonStructural) as FamilyInstance;
                            if (Math.Round(intersectionPointOrientation.AngleTo(intersectionPoint.FacingOrientation), 6) != 0)
                            {
                                Line rotationLine = Line.CreateBound(pointCenterResult, pointCenterResult + 1 * XYZ.BasisZ);
                                ElementTransformUtils.RotateElement(doc, intersectionPoint.Id, rotationLine, intersectionPointOrientation.AngleTo(intersectionPoint.FacingOrientation));
                            }
                            intersectionPoint.get_Parameter(intersectionPointWidthGuid).Set(RoundUpToIncrement(pipeIntersectionRightFaceDistance + pipeIntersectionLeftFaceDistance, roundUpIncrement));
                            intersectionPoint.get_Parameter(intersectionPointHeightGuid).Set(RoundUpToIncrement(pipeIntersectionTopFaceDistance + pipeIntersectionBottomFaceDistance, roundUpIncrement));
                            intersectionPoint.get_Parameter(intersectionPointThicknessGuid).Set(intersectionPointThickness);

                            intersectionPoint.get_Parameter(heightOfBaseLevelGuid).Set((doc.GetElement(pipeIntersectionPointGroup.First().LevelId) as Level).Elevation);
                            intersectionPoint.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(pointCenterResult.Z);
                            intersectionPoint.get_Parameter(levelOffsetGuid).Set(pointCenterResult.Z);
                        }
                        foreach (FamilyInstance p in pipeIntersectionPointGroup)
                        {
                            if (pipeIntersectionPointGroup.Count > 1)
                            {
                                pipeWallIntersectionPointList.Remove(p);
                                doc.Delete(p.Id);
                            }
                            else
                            {
                                pipeWallIntersectionPointList.Remove(p);
                            }
                        }
                        pipeIntersectionPointGroup.Clear();
                        if (pipeWallIntersectionPointList.Count != 0)
                        {
                            pipeIntersectionPointGroup.Add(pipeWallIntersectionPointList[0]);
                            pipeWallIntersectionPointList.Remove(pipeWallIntersectionPointList[0]);
                            tempPipeIntersectionPointList = pipeWallIntersectionPointList.ToList();
                        }
                    }
                    t.Commit();
                }

            }
            if (pipeFloorIntersectionPointList.Count != 0 && mergeHoles)
            {
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Объединение точек пересечения с перекрытиями");
                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    opt.DetailLevel = ViewDetailLevel.Fine;

                    List<FamilyInstance> pipeIntersectionPointGroup = new List<FamilyInstance>();
                    pipeIntersectionPointGroup.Add(pipeFloorIntersectionPointList[0]);
                    pipeFloorIntersectionPointList.Remove(pipeFloorIntersectionPointList[0]);
                    List<FamilyInstance> tempPipeIntersectionPointList = pipeFloorIntersectionPointList.ToList();

                    while (pipeFloorIntersectionPointList.Count != 0)
                    {
                        for (int i = 0; i < pipeIntersectionPointGroup.Count;)
                        {
                            Solid solidA = GetSolidFromIntersectionPoint(opt, pipeIntersectionPointGroup[i]);
                            for (int j = 0; j < tempPipeIntersectionPointList.Count;)
                            {
                                Solid solidB = GetSolidFromIntersectionPoint(opt, tempPipeIntersectionPointList[j]);
                                double intersectvolume = Math.Round(BooleanOperationsUtils.ExecuteBooleanOperation(solidA, solidB, BooleanOperationsType.Intersect).Volume, 6);
                                if (intersectvolume > 0)
                                {
                                    pipeIntersectionPointGroup.Add(tempPipeIntersectionPointList[j]);
                                    tempPipeIntersectionPointList.Remove(tempPipeIntersectionPointList[j]);
                                    i = -1;
                                    j++;
                                }
                                else
                                {
                                    j++;
                                }
                            }
                            i++;
                        }

                        if (pipeIntersectionPointGroup.Count > 1)
                        {
                            //Средняя точка
                            double pointsSquareSummX = 0;
                            double pointsSquareSummY = 0;
                            foreach (FamilyInstance p in pipeIntersectionPointGroup)
                            {
                                pointsSquareSummX = pointsSquareSummX + (p.Location as LocationPoint).Point.X;
                                pointsSquareSummY = pointsSquareSummY + (p.Location as LocationPoint).Point.Y;
                            }
                            double pointCenterX = pointsSquareSummX / pipeIntersectionPointGroup.Count;
                            double pointCenterY = pointsSquareSummY / pipeIntersectionPointGroup.Count;

                            //Размер проема
                            double pipeIntersectionRightFaceDistance = 0;
                            double pipeIntersectionLeftFaceDistance = 0;
                            double pipeIntersectionTopFaceDistance = 0;
                            double pipeIntersectionBottomFaceDistance = 0;
                            double intersectionPointThickness = 0;
                            foreach (FamilyInstance p in pipeIntersectionPointGroup)
                            {
                                XYZ pipeIntersectionLocationPoint = (p.Location as LocationPoint).Point;
                                double pipeIntersectionWidth = p.get_Parameter(intersectionPointWidthGuid).AsDouble();
                                double pipeIntersectionHeight = p.get_Parameter(intersectionPointHeightGuid).AsDouble();
                                XYZ pointGravityCenter = new XYZ(pointCenterX, pointCenterY, (p.Location as LocationPoint).Point.Z);

                                XYZ pipeIntersectionRightFaceDistancePoint = pipeIntersectionLocationPoint + (pipeIntersectionWidth / 2) * p.HandOrientation;
                                XYZ pipeIntersectionLeftFaceDistancePoint = pipeIntersectionLocationPoint + (pipeIntersectionWidth / 2) * p.HandOrientation.Negate();

                                XYZ pipeIntersectionTopFaceDistancePoint = pipeIntersectionLocationPoint + (pipeIntersectionHeight / 2) * p.FacingOrientation;
                                XYZ pipeIntersectionBottomFaceDistancePoint = pipeIntersectionLocationPoint + (pipeIntersectionHeight / 2) * p.FacingOrientation.Negate();


                                Curve pipeIntersectionRightFaceCurve = Line.CreateBound(pipeIntersectionRightFaceDistancePoint + 50 * p.FacingOrientation, pipeIntersectionRightFaceDistancePoint + 50 * p.FacingOrientation.Negate()) as Curve;
                                Curve pipeIntersectionLeftFaceCurve = Line.CreateBound(pipeIntersectionLeftFaceDistancePoint + 50 * p.FacingOrientation, pipeIntersectionLeftFaceDistancePoint + 50 * p.FacingOrientation.Negate()) as Curve;

                                Curve pipeIntersectionTopFaceCurve = Line.CreateBound(pipeIntersectionTopFaceDistancePoint + 50 * p.HandOrientation, pipeIntersectionTopFaceDistancePoint + 50 * p.HandOrientation.Negate()) as Curve;
                                Curve pipeIntersectionBottomFaceCurve = Line.CreateBound(pipeIntersectionBottomFaceDistancePoint + 50 * p.HandOrientation, pipeIntersectionBottomFaceDistancePoint + 50 * p.HandOrientation.Negate()) as Curve;

                                IntersectionResultArray ira = new IntersectionResultArray();
                                SetComparisonResult scr = new SetComparisonResult();
                                
                                scr = (Line.CreateBound(pointGravityCenter, pointGravityCenter + 50 * p.HandOrientation) as Curve).Intersect(pipeIntersectionRightFaceCurve, out ira);
                                if(scr == SetComparisonResult.Overlap)
                                {
                                    double tmpPipeIntersectionRightFaceDistance = pointGravityCenter.DistanceTo(ira.get_Item(0).XYZPoint);
                                    if (tmpPipeIntersectionRightFaceDistance > pipeIntersectionRightFaceDistance)
                                    {
                                        pipeIntersectionRightFaceDistance = tmpPipeIntersectionRightFaceDistance;
                                    }
                                }

                                scr = (Line.CreateBound(pointGravityCenter, pointGravityCenter + 50 * p.HandOrientation.Negate()) as Curve).Intersect(pipeIntersectionLeftFaceCurve, out ira);
                                if (scr == SetComparisonResult.Overlap)
                                {
                                    double tmpPipeIntersectionLeftFaceDistance = pointGravityCenter.DistanceTo(ira.get_Item(0).XYZPoint);
                                    if (tmpPipeIntersectionLeftFaceDistance > pipeIntersectionLeftFaceDistance)
                                    {
                                        pipeIntersectionLeftFaceDistance = tmpPipeIntersectionLeftFaceDistance;
                                    }
                                }

                                scr = (Line.CreateBound(pointGravityCenter, pointGravityCenter + 50 * p.FacingOrientation) as Curve).Intersect(pipeIntersectionTopFaceCurve, out ira);
                                if (scr == SetComparisonResult.Overlap)
                                {
                                    double tmpPipeIntersectionTopFaceDistance = pointGravityCenter.DistanceTo(ira.get_Item(0).XYZPoint);
                                    if (tmpPipeIntersectionTopFaceDistance > pipeIntersectionTopFaceDistance)
                                    {
                                        pipeIntersectionTopFaceDistance = tmpPipeIntersectionTopFaceDistance;
                                    }
                                }

                                scr = (Line.CreateBound(pointGravityCenter, pointGravityCenter + 50 * p.FacingOrientation.Negate()) as Curve).Intersect(pipeIntersectionBottomFaceCurve, out ira);
                                if (scr == SetComparisonResult.Overlap)
                                {
                                    double tmpPipeIntersectionBottomFaceDistance = pointGravityCenter.DistanceTo(ira.get_Item(0).XYZPoint);
                                    if (tmpPipeIntersectionBottomFaceDistance > pipeIntersectionBottomFaceDistance)
                                    {
                                        pipeIntersectionBottomFaceDistance = tmpPipeIntersectionBottomFaceDistance;
                                    }
                                }

                                if (p.get_Parameter(intersectionPointThicknessGuid).AsDouble() > intersectionPointThickness)
                                {
                                    intersectionPointThickness = p.get_Parameter(intersectionPointThicknessGuid).AsDouble();
                                }
                            }
                            XYZ pointCenterResult = new XYZ(pointCenterX
                                , pointCenterY
                                , (pipeIntersectionPointGroup.First().Location as LocationPoint).Point.Z - (doc.GetElement(pipeIntersectionPointGroup.First().LevelId) as Level).Elevation - basePointZ);

                            FamilyInstance intersectionPoint = doc.Create.NewFamilyInstance(pointCenterResult
                            , intersectionPointRectangularFloorFamilySymbol
                            , doc.GetElement(pipeIntersectionPointGroup.First().LevelId) as Level
                            , StructuralType.NonStructural) as FamilyInstance;
                            intersectionPoint.get_Parameter(intersectionPointWidthGuid).Set(RoundUpToIncrement(pipeIntersectionRightFaceDistance + pipeIntersectionLeftFaceDistance, roundUpIncrement));
                            intersectionPoint.get_Parameter(intersectionPointHeightGuid).Set(RoundUpToIncrement(pipeIntersectionTopFaceDistance + pipeIntersectionBottomFaceDistance, roundUpIncrement));
                            intersectionPoint.get_Parameter(intersectionPointThicknessGuid).Set(intersectionPointThickness);
                        }
                        foreach (FamilyInstance p in pipeIntersectionPointGroup)
                        {
                            if (pipeIntersectionPointGroup.Count > 1)
                            {
                                pipeFloorIntersectionPointList.Remove(p);
                                doc.Delete(p.Id);
                            }
                            else
                            {
                                pipeFloorIntersectionPointList.Remove(p);
                            }
                        }
                        pipeIntersectionPointGroup.Clear();
                        if (pipeFloorIntersectionPointList.Count != 0)
                        {
                            pipeIntersectionPointGroup.Add(pipeFloorIntersectionPointList[0]);
                            pipeFloorIntersectionPointList.Remove(pipeFloorIntersectionPointList[0]);
                            tempPipeIntersectionPointList = pipeFloorIntersectionPointList.ToList();
                        }
                    }

                    t.Commit();
                }
            }
            return Result.Succeeded;
        }

        private static Level GetClosestBottomWallLevel(Document doc, Document linkDoc, Wall wall)
        {
            Level tmpLvl = null;
            List<Level> lvlList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .Where(l => Math.Round(l.Elevation, 6) == Math.Round((linkDoc.GetElement(wall.LevelId) as Level).Elevation, 6))
                .ToList();
            if (lvlList.Count != 0)
            {
                tmpLvl = lvlList.First() as Level;
            }
            else
            {
                double heightDifference = 10000000000;
                lvlList = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Levels)
                    .WhereElementIsNotElementType()
                    .Cast<Level>()
                    .ToList();
                foreach (Level l in lvlList)
                {
                    double tmpHeightDifference = Math.Round(l.Elevation, 6) - Math.Round((linkDoc.GetElement(wall.LevelId) as Level).Elevation);
                    if (Math.Round(l.Elevation, 6) > Math.Round((linkDoc.GetElement(wall.LevelId) as Level).Elevation)
                        && tmpHeightDifference < heightDifference)
                    {
                        tmpLvl = l as Level;
                        heightDifference = tmpHeightDifference;
                    }
                }
            }
            return tmpLvl;
        }
        private static Level GetClosestBottomFloorLevel(Document doc, Document linkDoc, Floor floor)
        {
            Level tmpLvl = null;
            List<Level> lvlList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .Where(l => Math.Round(l.Elevation, 6) == Math.Round((linkDoc.GetElement(floor.LevelId) as Level).Elevation, 6))
                .ToList();
            if (lvlList.Count != 0)
            {
                tmpLvl = lvlList.First() as Level;
            }
            else
            {
                double heightDifference = 10000000000;
                lvlList = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Levels)
                    .WhereElementIsNotElementType()
                    .Cast<Level>()
                    .ToList();
                foreach (Level l in lvlList)
                {
                    double tmpHeightDifference = Math.Round(l.Elevation, 6) - Math.Round((linkDoc.GetElement(floor.LevelId) as Level).Elevation);
                    if (Math.Round(l.Elevation, 6) > Math.Round((linkDoc.GetElement(floor.LevelId) as Level).Elevation)
                        && tmpHeightDifference < heightDifference)
                    {
                        tmpLvl = l as Level;
                        heightDifference = tmpHeightDifference;
                    }
                }
            }
            return tmpLvl;
        }
        private static Solid GetSolidFromIntersectionPoint(Options opt, FamilyInstance pipeIntersectionPoint)
        {
            Solid tmpSolid = null;
            GeometryElement geoElement = pipeIntersectionPoint.get_Geometry(opt);
            foreach (GeometryObject geoObject in geoElement)
            {
                GeometryInstance instance = geoObject as GeometryInstance;
                if (instance != null)
                {
                    GeometryElement instanceGeometryElement = instance.GetInstanceGeometry();
                    foreach (GeometryObject o in instanceGeometryElement)
                    {
                        tmpSolid = o as Solid;
                        if (tmpSolid != null && tmpSolid.Volume != 0) break;
                    }
                }
            }

            return tmpSolid;
        }
        private double RoundToIncrement(double x, double m)
        {
            return (Math.Round((x * 304.8) / m) * m) / 304.8;
        }
        private double RoundUpToIncrement(double x, double m)
        {
            double a = (x * 304.8) % m;
            if(Math.Round(a, 6) != 0)
            {
                return (Math.Round((x * 304.8) / m) * m + m) / 304.8;
            }
            else
            {
                return x;
            }
        }
        private double GetAngleFromMEPCurve(MEPCurve curve)
        {
            foreach (Connector c in curve.ConnectorManager.Connectors)
            {
                return Math.Asin(c.CoordinateSystem.BasisY.X);
            }
            return 0;
        }
    }
}
