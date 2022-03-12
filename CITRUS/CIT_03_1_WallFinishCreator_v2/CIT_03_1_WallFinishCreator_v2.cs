using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_03_1_WallFinishCreator_v2
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_03_1_WallFinishCreator_v2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            List<Room> roomList = new List<Room>();
            roomList = GetRoomsFromCurrentSelection(doc, sel);
            if (roomList.Count == 0)
            {
                RoomSelectionFilter selFilter = new RoomSelectionFilter();
                IList<Reference> selRooms = sel.PickObjects(ObjectType.Element, selFilter, "Выберите помещения!");

                foreach (Reference roomRef in selRooms)
                {
                    roomList.Add(doc.GetElement(roomRef) as Room);
                }
            }

            List<Material> materialsList = GetMaterialsListFromBoundarySegments(doc, roomList);
            List<WallType> wallTypesList = new FilteredElementCollector(doc)
                .OfClass(typeof(WallType))
                .Where(wt => wt.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Walls))
                .Cast<WallType>()
                .ToList();
            CIT_03_1_WallFinishCreatorForm_v2 wallFinishCreatorForm_v2 = new CIT_03_1_WallFinishCreatorForm_v2(doc, materialsList, wallTypesList);
            wallFinishCreatorForm_v2.ShowDialog();
            if (wallFinishCreatorForm_v2.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }
            List<List<Element>> finishCreatorSelectionResult = wallFinishCreatorForm_v2.FinishCreatorSelectionResult;

            //Транзакция
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Создание отделки");
                foreach (Room room in roomList)
                {
                    IList<IList<BoundarySegment>> roomBoundarySegmentsListsList = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                    foreach (IList<BoundarySegment> roomBoundarySegmentsList in roomBoundarySegmentsListsList)
                    {
                        XYZ resultingStartPoint = null;
                        XYZ resultingEndPoint = null;
                        XYZ resultingCurveOfBoundarySegmentDirection = null;
                        Material resultingElementOfBoundarySegmentMaterial = null;
                        foreach (BoundarySegment roomBoundarySegment in roomBoundarySegmentsList)
                        {
                            Curve curveOfBoundarySegment = roomBoundarySegment.GetCurve();
                            XYZ startPoint = curveOfBoundarySegment.GetEndPoint(0);
                            XYZ endPoint = curveOfBoundarySegment.GetEndPoint(1);
                            XYZ curveOfBoundarySegmentDirection = (curveOfBoundarySegment as Line).Direction;
                            Element elementOfBoundarySegment = doc.GetElement(roomBoundarySegment.ElementId);
                            Material elementOfBoundarySegmentMaterial = null;
                            if (elementOfBoundarySegment != null && elementOfBoundarySegment.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Walls))
                            {
                                elementOfBoundarySegmentMaterial = doc.GetElement((elementOfBoundarySegment as Wall)
                                    .WallType
                                    .GetCompoundStructure()
                                    .GetMaterialId(0)) as Material;
                            }
                            else if (elementOfBoundarySegment != null && elementOfBoundarySegment.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralColumns))
                            {
                                elementOfBoundarySegmentMaterial = doc.GetElement((elementOfBoundarySegment as FamilyInstance)
                                    .Symbol
                                    .get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM)
                                    .AsElementId()) as Material;
                            }
                            else if (elementOfBoundarySegment != null && elementOfBoundarySegment.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors))
                            {
                                elementOfBoundarySegmentMaterial = doc.GetElement((elementOfBoundarySegment as Floor)
                                    .FloorType
                                    .GetCompoundStructure()
                                    .GetMaterialId(0)) as Material;
                            }
                            else
                            {
                                continue;
                            }

                            if (resultingStartPoint == null
                                && resultingEndPoint == null
                                && resultingCurveOfBoundarySegmentDirection == null
                                && resultingElementOfBoundarySegmentMaterial == null)
                            {
                                resultingStartPoint = startPoint;
                                resultingEndPoint = endPoint;
                                resultingCurveOfBoundarySegmentDirection = curveOfBoundarySegmentDirection;
                                resultingElementOfBoundarySegmentMaterial = elementOfBoundarySegmentMaterial;
                                continue;
                            }
                            else if (resultingStartPoint != null
                                && resultingEndPoint != null
                                && resultingCurveOfBoundarySegmentDirection != null
                                && resultingElementOfBoundarySegmentMaterial != null)
                            {
                                if (resultingCurveOfBoundarySegmentDirection.IsAlmostEqualTo(curveOfBoundarySegmentDirection)
                                    && resultingElementOfBoundarySegmentMaterial.Id == elementOfBoundarySegmentMaterial.Id
                                    && (roomBoundarySegmentsList.Count - 1) != roomBoundarySegmentsList.IndexOf(roomBoundarySegment))
                                {
                                    resultingEndPoint = endPoint;
                                    continue;
                                }
                                else if ((!resultingCurveOfBoundarySegmentDirection.IsAlmostEqualTo(curveOfBoundarySegmentDirection)
                                    || resultingElementOfBoundarySegmentMaterial.Id != elementOfBoundarySegmentMaterial.Id)
                                    && (roomBoundarySegmentsList.Count -1) != roomBoundarySegmentsList.IndexOf(roomBoundarySegment))
                                {
                                    Curve resultingWallFinishCurve = Line.CreateBound(resultingStartPoint, resultingEndPoint) as Curve;
                                    WallType wt = finishCreatorSelectionResult.FirstOrDefault(l => (l[0] as Material).Id == resultingElementOfBoundarySegmentMaterial.Id)[1] as WallType;
                                    Wall w = Wall.Create(doc, resultingWallFinishCurve, wt.Id, room.LevelId, 10, 0, true, false) as Wall;
                                    w.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).Set(2);
                                    XYZ wallOrientationVector = (resultingWallFinishCurve as Line).Direction.CrossProduct(XYZ.BasisZ).Negate();
                                    ElementTransformUtils.MoveElement(doc, w.Id, wallOrientationVector * (w.WallType.Width / 2));
                                    resultingStartPoint = startPoint;
                                    resultingEndPoint = endPoint;
                                    resultingCurveOfBoundarySegmentDirection = curveOfBoundarySegmentDirection;
                                    resultingElementOfBoundarySegmentMaterial = elementOfBoundarySegmentMaterial;
                                    continue;
                                }
                                else
                                {
                                    if (resultingCurveOfBoundarySegmentDirection.IsAlmostEqualTo(curveOfBoundarySegmentDirection)
                                    && resultingElementOfBoundarySegmentMaterial.Id == elementOfBoundarySegmentMaterial.Id)
                                    {
                                        Curve resultingWallFinishCurve = Line.CreateBound(resultingStartPoint, endPoint) as Curve;
                                        WallType wt = finishCreatorSelectionResult.FirstOrDefault(l => (l[0] as Material).Id == elementOfBoundarySegmentMaterial.Id)[1] as WallType;
                                        Wall w = Wall.Create(doc, resultingWallFinishCurve, wt.Id, room.LevelId, 10, 0, true, false) as Wall;
                                        w.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).Set(2);
                                        XYZ wallOrientationVector = (resultingWallFinishCurve as Line).Direction.CrossProduct(XYZ.BasisZ).Negate();
                                        ElementTransformUtils.MoveElement(doc, w.Id, wallOrientationVector * (w.WallType.Width / 2));
                                    }
                                    else
                                    {
                                        Curve resultingWallFinishCurve = Line.CreateBound(resultingStartPoint, resultingEndPoint) as Curve;
                                        WallType wt = finishCreatorSelectionResult.FirstOrDefault(l => (l[0] as Material).Id == resultingElementOfBoundarySegmentMaterial.Id)[1] as WallType;
                                        Wall w = Wall.Create(doc, resultingWallFinishCurve, wt.Id, room.LevelId, 10, 0, true, false) as Wall;
                                        w.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).Set(2);
                                        XYZ wallOrientationVector = (resultingWallFinishCurve as Line).Direction.CrossProduct(XYZ.BasisZ).Negate();
                                        ElementTransformUtils.MoveElement(doc, w.Id, wallOrientationVector * (w.WallType.Width / 2));

                                        resultingWallFinishCurve = Line.CreateBound(startPoint, endPoint) as Curve;
                                        wt = finishCreatorSelectionResult.FirstOrDefault(l => (l[0] as Material).Id == elementOfBoundarySegmentMaterial.Id)[1] as WallType;
                                        w = Wall.Create(doc, resultingWallFinishCurve, wt.Id, room.LevelId, 10, 0, true, false) as Wall;
                                        w.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).Set(2);
                                        wallOrientationVector = (resultingWallFinishCurve as Line).Direction.CrossProduct(XYZ.BasisZ).Negate();
                                        ElementTransformUtils.MoveElement(doc, w.Id, wallOrientationVector * (w.WallType.Width / 2));
                                        continue;
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

        private static List<Material> GetMaterialsListFromBoundarySegments(Document doc, List<Room> roomList)
        {
            List<Material> tempMaterialsList = new List<Material>();
            List<ElementId> tempMaterialIdList = new List<ElementId>();
            foreach (Room room in roomList)
            {
                IList<IList<BoundarySegment>> roomBoundarySegmentsListsList = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                foreach (IList<BoundarySegment> roomBoundarySegmentsList in roomBoundarySegmentsListsList)
                {
                    foreach (BoundarySegment roomBoundarySegment in roomBoundarySegmentsList)
                    {
                        Element elementOfBoundarySegment = doc.GetElement(roomBoundarySegment.ElementId);
                        if (elementOfBoundarySegment != null && elementOfBoundarySegment.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Walls))
                        {
                            Material elementOfBoundarySegmentMaterial = doc.GetElement((elementOfBoundarySegment as Wall)
                                .WallType
                                .GetCompoundStructure()
                                .GetMaterialId(0)) as Material;
                            if (tempMaterialIdList.IndexOf(elementOfBoundarySegmentMaterial.Id) == -1)
                            {
                                tempMaterialIdList.Add(elementOfBoundarySegmentMaterial.Id);
                            }
                        }
                        else if (elementOfBoundarySegment != null && elementOfBoundarySegment.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralColumns))
                        {
                            Material elementOfBoundarySegmentMaterial = doc.GetElement((elementOfBoundarySegment as FamilyInstance)
                                .Symbol
                                .get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM)
                                .AsElementId()) as Material;
                            if (tempMaterialIdList.IndexOf(elementOfBoundarySegmentMaterial.Id) == -1)
                            {
                                tempMaterialIdList.Add(elementOfBoundarySegmentMaterial.Id);
                            }
                        }
                        else if (elementOfBoundarySegment != null && elementOfBoundarySegment.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors))
                        {
                            Material elementOfBoundarySegmentMaterial = doc.GetElement((elementOfBoundarySegment as Floor)
                                .FloorType
                                .GetCompoundStructure()
                                .GetMaterialId(0)) as Material;
                            if (tempMaterialIdList.IndexOf(elementOfBoundarySegmentMaterial.Id) == -1)
                            {
                                tempMaterialIdList.Add(elementOfBoundarySegmentMaterial.Id);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            foreach (ElementId elemId in tempMaterialIdList)
            {
                tempMaterialsList.Add(doc.GetElement(elemId) as Material);
            }
            return tempMaterialsList;
        }
        private static List<Room> GetRoomsFromCurrentSelection(Document doc, Selection sel)
        {
            ICollection<ElementId> selectedIds = sel.GetElementIds();
            List<Room> tempRoomsList = new List<Room>();
            foreach (ElementId roomId in selectedIds)
            {
                if (doc.GetElement(roomId) is Room
                    && null != doc.GetElement(roomId).Category
                    && doc.GetElement(roomId).Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Rooms))
                {
                    tempRoomsList.Add(doc.GetElement(roomId) as Room);
                }
            }
            return tempRoomsList;
        }
    }
}
