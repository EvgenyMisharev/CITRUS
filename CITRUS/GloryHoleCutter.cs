using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class GloryHoleCutter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            ////Получение точек вырезания
            List<FamilyInstance> intersectionPointList = new List<FamilyInstance>();
            intersectionPointList = GetIntersectionPointCurrentSelection(doc, sel);
            if(intersectionPointList.Count == 0)
            {
                //Выбор точек вырезания
                GloryHoleWindowsSelectionFilter gloryHoleWindowsSelectionFilter = new GloryHoleWindowsSelectionFilter();
                IList<Reference> intersectionPointRefList = null;
                try
                {
                    intersectionPointRefList = sel.PickObjects(ObjectType.Element, gloryHoleWindowsSelectionFilter, "Выберите точки вырезания!");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Cancelled;
                }
                foreach (Reference refElem in intersectionPointRefList)
                {
                    intersectionPointList.Add((doc.GetElement(refElem) as FamilyInstance));
                }
            }

            //Типы семейств для вырезания проемов
            FamilySymbol intersectionPointRectangularWallFamilySymbol = null;
            FamilySymbol intersectionPointRectangularFloorFamilySymbol = null;
            List<FamilySymbol> intersectionPointRectangularWallFamilySymbolList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Where(fs => fs.Family.Name == "231_Отверстие прямоуг (Окно_Стена)")
                .ToList();
            if (intersectionPointRectangularWallFamilySymbolList.Count != 0)
            {
                intersectionPointRectangularWallFamilySymbol = intersectionPointRectangularWallFamilySymbolList.First();
            }
            else
            {
                TaskDialog.Show("Revit", "232_Отверстие в плите прямоугольное (Окно_Плита)\" не найдено!");
                return Result.Cancelled;
            }
            List<FamilySymbol> intersectionPointRectangularFloorFamilySymbolList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Where(fs => fs.Family.Name == "232_Отверстие в плите прямоугольное (Окно_Плита)")
                .ToList();
            if (intersectionPointRectangularFloorFamilySymbolList.Count != 0)
            {
                intersectionPointRectangularFloorFamilySymbol = intersectionPointRectangularFloorFamilySymbolList.First();
            }
            else
            {
                TaskDialog.Show("Revit", "Семейство \"232_Отверстие в плите прямоугольное (Окно_Плита)\" не найдено!");
                return Result.Cancelled;
            }

            //Получение стен
            List<Wall> wallsList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .OfClass(typeof(Wall))
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .ToList();
            //Получение перекрытий
            List<Floor> floorsList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Floors)
                .OfClass(typeof(Floor))
                .WhereElementIsNotElementType()
                .Cast<Floor>()
                .Where(f => f.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).AsInteger() == 1)
                .ToList();

            Guid intersectionPointWidthGuid = new Guid("8f2e4f93-9472-4941-a65d-0ac468fd6a5d");
            Guid intersectionPointHeightGuid = new Guid("da753fe3-ecfa-465b-9a2c-02f55d0c2ff1");
            Guid intersectionPointThicknessGuid = new Guid("293f055d-6939-4611-87b7-9a50d0c1f50e");

            Guid heightOfBaseLevelGuid = new Guid("9f5f7e49-616e-436f-9acc-5305f34b6933");
            Guid levelOffsetGuid = new Guid("515dc061-93ce-40e4-859a-e29224d80a10");

            List<ElementId> tmpIntersectionPointIdList = new List<ElementId>();
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Вырезание отверстий");
                intersectionPointRectangularWallFamilySymbol.Activate();
                intersectionPointRectangularFloorFamilySymbol.Activate();
                Options opt = new Options();
                opt.ComputeReferences = true;
                opt.DetailLevel = ViewDetailLevel.Fine;

                //Обработка стен
                foreach (Wall wall in wallsList)
                {
                    Solid wallSolidClone = null;
                    GeometryElement geomElem = wall.get_Geometry(opt);
                    foreach (GeometryObject geomObj in geomElem)
                    {
                        Solid solidA = geomObj as Solid;
                        if (null != solidA)
                        {
                            wallSolidClone = SolidUtils.Clone(solidA);
                            for (int i = 0; i < intersectionPointList.Count; i++)
                            {
                                if (tmpIntersectionPointIdList.IndexOf(intersectionPointList[i].Id).Equals(-1))
                                {
                                    Solid solidB = GetSolidFromIntersectionPoint(opt, intersectionPointList[i]);
                                    double intersectvolume = Math.Round(BooleanOperationsUtils.ExecuteBooleanOperation(wallSolidClone, solidB, BooleanOperationsType.Intersect).Volume, 6);
                                    if (intersectvolume > 0 && intersectionPointList[i].Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Стена")
                                    {
                                        FamilyInstance hole = doc.Create.NewFamilyInstance((intersectionPointList[i].Location as LocationPoint).Point
                                            , intersectionPointRectangularWallFamilySymbol
                                            , wall
                                            , doc.GetElement(intersectionPointList[i].LevelId) as Level
                                            , StructuralType.NonStructural) as FamilyInstance;

                                        hole.get_Parameter(intersectionPointWidthGuid).Set(intersectionPointList[i].get_Parameter(intersectionPointWidthGuid).AsDouble());
                                        hole.get_Parameter(intersectionPointHeightGuid).Set(intersectionPointList[i].get_Parameter(intersectionPointHeightGuid).AsDouble());
                                        hole.get_Parameter(heightOfBaseLevelGuid).Set(intersectionPointList[i].get_Parameter(heightOfBaseLevelGuid).AsDouble());
                                        hole.get_Parameter(levelOffsetGuid).Set(intersectionPointList[i].get_Parameter(levelOffsetGuid).AsDouble());

                                        tmpIntersectionPointIdList.Add(intersectionPointList[i].Id);
                                    }
                                }
                            }
                        }
                    }
                    if (intersectionPointList.Count == tmpIntersectionPointIdList.Count) break;
                }
                //Обработка перекрытий
                Transform transform = Transform.CreateTranslation(new XYZ(0, 0, 0));
                foreach (Floor floor in floorsList)
                {
                    Solid floorSolidClone = null;
                    GeometryElement geomElem = floor.get_Geometry(opt);
                    foreach (GeometryObject geomObj in geomElem)
                    {
                        Solid solidA = geomObj as Solid;
                        if (null != solidA)
                        {
                            floorSolidClone = SolidUtils.Clone(solidA);
                            for (int i = 0; i < intersectionPointList.Count; i++)
                            {
                                if (tmpIntersectionPointIdList.IndexOf(intersectionPointList[i].Id).Equals(-1))
                                {
                                    Solid solidB = GetSolidFromIntersectionPoint(opt, intersectionPointList[i]);
                                    double intersectvolume = Math.Round(BooleanOperationsUtils.ExecuteBooleanOperation(floorSolidClone, solidB, BooleanOperationsType.Intersect).Volume, 6);
                                    if (intersectvolume > 0 && intersectionPointList[i].Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Плита")
                                    {
                                        FamilyInstance hole = doc.Create.NewFamilyInstance((intersectionPointList[i].Location as LocationPoint).Point
                                            , intersectionPointRectangularFloorFamilySymbol
                                            , floor
                                            , doc.GetElement(intersectionPointList[i].LevelId) as Level
                                            , StructuralType.NonStructural) as FamilyInstance;

                                        hole.get_Parameter(BuiltInParameter.CASEWORK_WIDTH).Set(intersectionPointList[i].get_Parameter(intersectionPointWidthGuid).AsDouble());
                                        hole.get_Parameter(BuiltInParameter.CASEWORK_HEIGHT).Set(intersectionPointList[i].get_Parameter(intersectionPointHeightGuid).AsDouble());
                                        if (Math.Round((intersectionPointList[i].Location as LocationPoint).Rotation, 6) != 0)
                                        {
                                            Line axis = Line.CreateBound((intersectionPointList[i].Location as LocationPoint).Point, (intersectionPointList[i].Location as LocationPoint).Point + 1 * XYZ.BasisZ);
                                            ElementTransformUtils.RotateElement(doc, hole.Id, axis, (intersectionPointList[i].Location as LocationPoint).Rotation);
                                        }
                                        tmpIntersectionPointIdList.Add(intersectionPointList[i].Id);
                                    }
                                }
                            }
                        }
                    }
                    if (intersectionPointList.Count == tmpIntersectionPointIdList.Count) break;
                }
                foreach (ElementId intersectionPointId in tmpIntersectionPointIdList)
                {
                    doc.Delete(intersectionPointId);
                }

                t.Commit();
            }
                return Result.Succeeded;
        }
        private static Solid GetSolidFromIntersectionPoint(Options opt, FamilyInstance fi)
        {
            Solid tmpSolid = null;
            GeometryElement geoElement = fi.get_Geometry(opt);
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
        private static List<FamilyInstance> GetIntersectionPointCurrentSelection(Document doc, Selection sel)
        {
            ICollection<ElementId> selectedIds = sel.GetElementIds();
            List<FamilyInstance> tempIntersectionPointList = new List<FamilyInstance>();
            foreach (ElementId intersectionPointId in selectedIds)
            {
                if (doc.GetElement(intersectionPointId) is FamilyInstance
                    && null != doc.GetElement(intersectionPointId).Category
                    && doc.GetElement(intersectionPointId).Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Windows)
                    && ((doc.GetElement(intersectionPointId) as FamilyInstance).Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Стена" 
                    || (doc.GetElement(intersectionPointId) as FamilyInstance).Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Плита"))
                {
                    tempIntersectionPointList.Add(doc.GetElement(intersectionPointId) as FamilyInstance);
                }
            }
            return tempIntersectionPointList;
        }
    }
}
