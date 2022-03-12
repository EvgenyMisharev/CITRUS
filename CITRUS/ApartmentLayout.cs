using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class ApartmentLayout : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Level> levelsList = new FilteredElementCollector(doc).OfClass(typeof(Level))
                .WhereElementIsNotElementType()
                .OrderBy(lv => lv.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble())
                .Cast<Level>()
                .ToList();
            List<Room> roomList = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement))
                .WhereElementIsNotElementType()
                .Where(r => r.GetType() == typeof(Room))
                .Cast<Room>()
                .ToList();
            List<string> sectionNumberList = GetSectionNumberList(roomList);

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Квартирография");
                foreach(Level lv in levelsList)
                {
                    if (sectionNumberList.Count > 1)
                    {
                        foreach(String sn in sectionNumberList)
                        {
                            List<Room> roomListAtLevelAndSection = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement))
                                .WhereElementIsNotElementType()
                                .Where(r => r.GetType() == typeof(Room))
                                .Where(r => r.LevelId == lv.Id)
                                .Where(r =>r.get_Parameter(new Guid("b59a3474-a5f4-430a-b087-a20f1a4eb57e")).AsString() == sn)
                                .Cast<Room>()
                                .ToList();
                            List<string> apartmentNumberList = GetApartmentNumberList(roomListAtLevelAndSection);

                            SetRoomTypeParam(roomListAtLevelAndSection);
                            SetApartmentAreas(roomListAtLevelAndSection, apartmentNumberList);
                        }
                    }
                    else
                    {
                        List<Room> roomListAtLevel = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement))
                                .WhereElementIsNotElementType()
                                .Where(r => r.GetType() == typeof(Room))
                                .Where(r => r.LevelId == lv.Id)
                                .Cast<Room>()
                                .ToList();
                        List<string> apartmentNumberList = GetApartmentNumberList(roomListAtLevel);

                        SetRoomTypeParam(roomListAtLevel);
                        SetApartmentAreas(roomListAtLevel, apartmentNumberList);
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
        private static List<string> GetSectionNumberList(List<Room> roomList)
        {
            Guid sectionNumberParamGuid = new Guid("b59a3474-a5f4-430a-b087-a20f1a4eb57e");
            List<string> tempSectionNumberList = new List<string>();
            foreach (Room room in roomList)
            {
                string sectionNumber = room.get_Parameter(sectionNumberParamGuid).AsString();
                if (!tempSectionNumberList.Contains(sectionNumber) && sectionNumber != null)
                {
                    tempSectionNumberList.Add(sectionNumber);
                }
            }
            tempSectionNumberList = tempSectionNumberList.OrderBy(n => n).ToList();
            return tempSectionNumberList;
        }
        private static List<string> GetApartmentNumberList(List<Room> roomList)
        {
            Guid apartmentNumberParamGuid = new Guid("10fb72de-237e-4b9c-915b-8849b8907695");
            List<string> tempApartmentNumberList = new List<string>();
            foreach (Room room in roomList)
            {
                string apartmentNumber = room.get_Parameter(apartmentNumberParamGuid).AsString();
                if (!tempApartmentNumberList.Contains(apartmentNumber) && apartmentNumber != null)
                {
                    tempApartmentNumberList.Add(apartmentNumber);
                }
            }
            tempApartmentNumberList = tempApartmentNumberList.OrderBy(n => n).ToList();
            return tempApartmentNumberList;
        }

        private static void SetRoomTypeParam(List<Room> roomList)
        {
            Guid roomTypeParamGuid = new Guid("7743e986-fcd9-4029-b960-71e522adccab");
            Guid areaCoefficientParamGuid = new Guid("066eab6d-c348-4093-b0ca-1dfe7e78cb6e");
            foreach (Room room in roomList)
            {
                double roomTypeParamAsDouble = room.get_Parameter(roomTypeParamGuid).AsDouble();
                if (roomTypeParamAsDouble == 1 || roomTypeParamAsDouble == 2 || roomTypeParamAsDouble == 5)
                {
                    room.get_Parameter(areaCoefficientParamGuid).Set(1);
                }
                else if (roomTypeParamAsDouble == 3)
                {
                    room.get_Parameter(areaCoefficientParamGuid).Set(0.5);
                }
                else if (roomTypeParamAsDouble == 4)
                {
                    room.get_Parameter(areaCoefficientParamGuid).Set(0.3);
                }
                else
                {
                    TaskDialog.Show("Revit", $"У помещения с Id - {room.Id} не заполнен параметр АР_ТипПомещения");
                }
            }
        }
        private static void SetApartmentAreas(List<Room> roomList, List<string> apartmentNumberList)
        {
            Guid apartmentNumberParamGuid = new Guid("10fb72de-237e-4b9c-915b-8849b8907695");
            foreach (string apartmentNumber in apartmentNumberList)
            {
                List<Room> apartmentRoomList = new List<Room>();
                foreach (Room room in roomList)
                {
                    if (apartmentNumber == room.get_Parameter(apartmentNumberParamGuid).AsString())
                    {
                        apartmentRoomList.Add(room);
                    }
                }
                //АР_ПлощКвЖилая - сумма площадей жилых комнаят
                Guid apartmentAreaResidentialParamGuid = new Guid("178e222b-903b-48f5-8bfc-b624cd67d13c");
                //АР_ПлощКвартиры - площадь квартиры без 3 и 4
                Guid apartmentAreaParamGuid = new Guid("d3035d0f-b738-4407-a0e5-30787b92fa49");
                //АР_ПлощКвОбщая - общая площадь квартиры с учетом коэффициентов
                Guid apartmentAreaTotalParamGuid = new Guid("af973552-3d15-48e3-aad8-121fe0dda34e");

                double apartmentAreaResidential = 0;
                double apartmentArea = 0;
                double apartmentAreaTotal = 0;

                foreach (Room room in apartmentRoomList)
                {
                    Guid roomTypeParamGuid = new Guid("7743e986-fcd9-4029-b960-71e522adccab");
                    double roomTypeParamAsDouble = room.get_Parameter(roomTypeParamGuid).AsDouble();
                    if (roomTypeParamAsDouble == 1)
                    {
                        apartmentAreaResidential += (Math.Round(room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble()/10.764,2) * 10.764);
                    }
                    if (roomTypeParamAsDouble == 1 || roomTypeParamAsDouble == 2)
                    {
                        apartmentArea += (Math.Round(room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble() / 10.764, 2) * 10.764);
                    }
                    if (roomTypeParamAsDouble != 5)
                    {
                        if (roomTypeParamAsDouble == 1 || roomTypeParamAsDouble == 2)
                        {
                            apartmentAreaTotal += (Math.Round(room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble() / 10.764, 2) * 10.764);
                        }
                        else if (roomTypeParamAsDouble == 3)
                        {
                            apartmentAreaTotal += (Math.Round((room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble() / 10.764) * 0.5, 2) * 10.764);
                        }
                        else if (roomTypeParamAsDouble == 4)
                        {
                            apartmentAreaTotal += (Math.Round((room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble() / 10.764) * 0.3,2) * 10.764);
                        }
                    }
                }
                foreach (Room room in apartmentRoomList)
                {
                    room.get_Parameter(apartmentAreaResidentialParamGuid).Set(apartmentAreaResidential);
                    room.get_Parameter(apartmentAreaParamGuid).Set(apartmentArea);
                    room.get_Parameter(apartmentAreaTotalParamGuid).Set(apartmentAreaTotal);
                }
            }
        }
    }
}
