using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Analysis;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class HeatLoss : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            SpaceSelectionFilter spaceSelFilter = new SpaceSelectionFilter();
            IList<Reference> selSpaces = sel.PickObjects(ObjectType.Element, spaceSelFilter, "Выберите пространства!");
            List<Space> spaceList = new List<Space>();

            foreach (Reference spaceRef in selSpaces)
            {
                spaceList.Add(doc.GetElement(spaceRef) as Space);
            }
            //List<Space> spaceList = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_MEPSpaces).Cast<Space>().Where(sp => sp.Volume > 0).ToList();
            ElementId linkFileId = spaceList.First().GetBoundarySegments(new SpatialElementBoundaryOptions()).First().First().ElementId;
            RevitLinkInstance linkFile = doc.GetElement(linkFileId) as RevitLinkInstance;
            Document doc2 = linkFile.GetLinkDocument();
            List<FamilyInstance> windowsList = new FilteredElementCollector(doc2)
                .OfClass(typeof(FamilyInstance))
                .OfCategory(BuiltInCategory.OST_Windows)
                .Cast<FamilyInstance>()
                .ToList();
            List<FamilyInstance> doorList = new FilteredElementCollector(doc2)
                .OfClass(typeof(FamilyInstance))
                .OfCategory(BuiltInCategory.OST_Doors)
                .Cast<FamilyInstance>()
                .ToList();

            //Открытие транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Тест Room");
                foreach (Space space in spaceList)
                {
                    if (space.get_Parameter(BuiltInParameter.SPACE_ASSOC_ROOM_NUMBER).AsString() != "Незанятое")
                    {
                        string assocRoomNumber = space.get_Parameter(BuiltInParameter.SPACE_ASSOC_ROOM_NUMBER).AsString();
                        string assocRoomName = space.get_Parameter(BuiltInParameter.SPACE_ASSOC_ROOM_NAME).AsString();

                        List<Room> roomsList = new FilteredElementCollector(doc2)
                            .OfClass(typeof(SpatialElement))
                            .OfCategory(BuiltInCategory.OST_Rooms)
                            .Cast<Room>()
                            .Where(rm => rm.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString() == assocRoomNumber)
                            .Where(rm => rm.get_Parameter(BuiltInParameter.ROOM_NAME).AsString() == assocRoomName)
                            .ToList();
                        Room room = roomsList.First();

                        double windowsAreaInRoom = 0;
                        double doorAreaInRoom = 0;
                        foreach (FamilyInstance window in windowsList)
                        {
                            if (window.Host != null & window.FromRoom != null && window.FromRoom.Id == room.Id 
                                & doc2.GetElement(window.Host.GetTypeId()).get_Parameter(BuiltInParameter.FUNCTION_PARAM).AsInteger() == 1)
                            {
                                windowsAreaInRoom += window.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble() *
                                        window.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                            }
                        }

                        foreach (FamilyInstance door in doorList)
                        {
                            if (door.Host != null & door.FromRoom != null && door.FromRoom.Id == room.Id
                                & doc2.GetElement(door.Host.GetTypeId()).get_Parameter(BuiltInParameter.FUNCTION_PARAM).AsInteger() == 1)
                            {
                                doorAreaInRoom += door.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble() *
                                        door.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                            }
                        }

                        space.LookupParameter("CIT_Площадь окон").Set(windowsAreaInRoom);
                        space.LookupParameter("CIT_Площадь наружных дверей").Set(doorAreaInRoom);
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
