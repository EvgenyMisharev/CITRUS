using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Architecture;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class FinishingSheet : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Room> roomsList = new FilteredElementCollector(doc)
                .OfClass(typeof(SpatialElement))
                .WhereElementIsNotElementType()
                .Where(room => room.GetType() == typeof(Room))
                .Cast<Room>()
                .ToList();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Отделка");

                foreach (Room room in roomsList)
                {
                    double doorWindowArea = 0;

                    List<FamilyInstance> windowsListFromRoom = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Windows)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .Where(w => w.FromRoom != null)
                    .Where(w => w.FromRoom.Id == room.Id)
                    .ToList();
                    if (windowsListFromRoom.Count != 0)
                    {
                        foreach (FamilyInstance w in windowsListFromRoom)
                        {
                            doorWindowArea += w.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble() * w.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                        }
                    }

                    List<FamilyInstance> windowsListToRoom = new FilteredElementCollector(doc)
                   .OfCategory(BuiltInCategory.OST_Windows)
                   .WhereElementIsNotElementType()
                   .Cast<FamilyInstance>()
                   .Where(w => w.ToRoom != null)
                   .Where(w => w.ToRoom.Id == room.Id)
                   .ToList();
                    if (windowsListToRoom.Count != 0)
                    {
                        foreach (FamilyInstance w in windowsListToRoom)
                        {
                            doorWindowArea += w.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble() * w.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                        }
                    }

                    List<FamilyInstance> doorsListFromRoom = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .Where(d => d.FromRoom != null)
                    .Where(d => d.FromRoom.Id == room.Id)
                    .ToList();
                    if (doorsListFromRoom.Count != 0)
                    {
                        foreach (FamilyInstance d in doorsListFromRoom)
                        {
                            doorWindowArea += d.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble() * d.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                        }
                    }

                    List<FamilyInstance> doorsListToRoom = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .Where(d => d.ToRoom != null)
                    .Where(d => d.ToRoom.Id == room.Id)
                    .ToList();
                    if (doorsListToRoom.Count != 0)
                    {
                        foreach (FamilyInstance d in doorsListToRoom)
                        {
                            doorWindowArea += d.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble() * d.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();
                        }
                    }

                    room.LookupParameter("ADSK_Площадь проемов").Set(doorWindowArea);

                }


                List<string> descriptionFinishList = new List<string>();
                foreach (Room room in roomsList)
                {
                    string descriptionWallFinish = room.LookupParameter("Описание Стен").AsString();
                    string descriptionCeilingFinish = room.LookupParameter("Описание Потолка").AsString();
                    string descriptionFinish = descriptionWallFinish + "|" + descriptionCeilingFinish;
                    if (descriptionFinishList.Contains(descriptionFinish))
                    {
                        continue;
                    }
                    else
                    {
                        descriptionFinishList.Add(descriptionFinish);
                    }
                }

                foreach (string str in descriptionFinishList)
                {
                    string[] w = str.Split('|');

                    List<Room> roomsListNumeratorList = new FilteredElementCollector(doc)
                        .OfClass(typeof(SpatialElement))
                        .WhereElementIsNotElementType()
                        .Where(room => room.GetType() == typeof(Room))
                        .Where(room => room.LookupParameter("Описание Стен").AsString() == w[0])
                        .Where(room => room.LookupParameter("Описание Потолка").AsString() == w[1])
                        .Cast<Room>()
                        .ToList();

                    List<string> numbers = new List<string>();
                    foreach (Room r in roomsListNumeratorList)
                    {
                        numbers.Add(r.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString());
                    }
                    numbers.Sort(new AlphanumComparatorFastString());

                    string numbersStr = "";
                    foreach (string strNum in numbers)
                    {
                        int index = numbers.IndexOf(strNum);
                        if (index != numbers.Count - 1)
                        {
                            numbersStr = numbersStr + strNum + ", ";
                        }
                        else
                        {
                            numbersStr = numbersStr + strNum;
                        }
                    }

                    foreach (Room r in roomsListNumeratorList)
                    {
                        r.LookupParameter("Групповой номер-Стены").Set(numbersStr);
                    }
                }


                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
