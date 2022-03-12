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
    class WallsReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение достпа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Выбор плит для армирования
            WallSelectionFilter selFilter = new WallSelectionFilter();
            IList<Reference> selWalls = sel.PickObjects(ObjectType.Element, selFilter, "Выберите плиту!");
            if (selWalls.Count == 0)
            {
                return Result.Succeeded;
            }
            List<Wall> wallList = new List<Wall>();

            foreach (Reference wallRef in selWalls)
            {
                wallList.Add(doc.GetElement(wallRef) as Wall);
            }
            if (wallList.Count == 0)
            {
                TaskDialog.Show("Revit", "Плита не выбрана!");
                return Result.Cancelled;
            }

            //Выбор типа армирования по площади
            List<AreaReinforcementType> areaReinforcementTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(AreaReinforcementType))
                .Cast<AreaReinforcementType>()
                .ToList();
            if (areaReinforcementTypeList.Count == 0)
            {
                TaskDialog.Show("Revit", "Тип армирования по площади не найден!");
                return Result.Cancelled;
            }
            AreaReinforcementType areaReinforcementType = areaReinforcementTypeList.First();

            //Список для низ X
            RebarBarType rebarBarType = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList()
                .First();

            //Старт транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Армирование стены");

                foreach (Wall wall in wallList)
                {
                    XYZ direction = ((wall.Location as LocationCurve).Curve as Line).Direction;

                    //Получение верхней грани плиты перекрытия
                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    GeometryElement geomFloorElement = wall.get_Geometry(opt);
                    Solid floorSolid = null;
                    foreach (GeometryObject geomObj in geomFloorElement)
                    {
                        floorSolid = geomObj as Solid;
                        if (floorSolid != null) break;
                    }
                    FaceArray faceArray = floorSolid.Faces;
                    PlanarFace myFace = null;
                    foreach (PlanarFace pf in faceArray)
                    {
                        if (Math.Round(pf.FaceNormal.Normalize().X) == Math.Round(wall.Orientation.Normalize().X) 
                            && Math.Round(pf.FaceNormal.Normalize().Y) == Math.Round(wall.Orientation.Normalize().Y) 
                            && Math.Round(pf.FaceNormal.Normalize().Z) == Math.Round(wall.Orientation.Normalize().Z))
                        {
                            myFace = pf;
                            break;
                        }
                    }

                    //Получение контуров верхней грани плиты перекрытия
                    IList<CurveLoop> curveLoopList = myFace.GetEdgesAsCurveLoops();
                    IList<Curve> curveList = new List<Curve>();
                    foreach (Curve c in curveLoopList.First())
                    {
                        curveList.Add(c);
                    }

                    //Создание армирования по площади для низ X
                    AreaReinforcement areaReinforcement = AreaReinforcement.Create(doc
                        , wall
                        , curveList
                        , direction
                        , areaReinforcementType.Id
                        , rebarBarType.Id
                        , ElementId.InvalidElementId);
                }
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
