using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class FloorGenerator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
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

			List<FloorType> myFloorTypeList = new FilteredElementCollector(doc)
				.OfClass(typeof(FloorType))
				.Where(f=>f.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors))
				.Cast<FloorType>()
				.ToList();

		    FloorTypeSelector formRoomTypeSelector = new FloorTypeSelector(myFloorTypeList);
            formRoomTypeSelector.ShowDialog();
			FloorType myFloorType = formRoomTypeSelector.mySelectionFloorType;


			using (Transaction t = new Transaction(doc))
			{
				foreach (Room myRoom in roomList)
				{
					Level myLevel = myRoom.Level;
					if (myLevel == null)
					{
						continue;
					}
					CurveArray roomCurves = new CurveArray();
					CurveArray secondCurves = new CurveArray();
					IList<IList<BoundarySegment>> loops = myRoom.GetBoundarySegments(new SpatialElementBoundaryOptions());
					for (int i = 0; i < loops.Count();i++)
                    {
						if (i == 0)
						{
							foreach (BoundarySegment seg in loops[i])
							{
								roomCurves.Append(seg.GetCurve());
							}
						}
						else
                        {
							foreach (BoundarySegment seg in loops[i])
							{
								secondCurves.Append(seg.GetCurve());
							}
						}
					}
					t.Start("Создание пола");
					Floor myFloor = doc.Create.NewFloor(roomCurves, myFloorType, myLevel, true);
					t.Commit();
					t.Start("Вырезание проема");
					if (secondCurves.Size != 0)
                    {
						doc.Create.NewOpening(myFloor, secondCurves, true);
					}
					t.Commit();
				}
			}
			return Result.Succeeded;
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
