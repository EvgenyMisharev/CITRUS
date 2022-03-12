using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_03_2_FinishNumerator
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_03_2_FinishNumerator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            CIT_03_2_FinishNumeratorForm finishNumeratorForm = new CIT_03_2_FinishNumeratorForm();
            finishNumeratorForm.ShowDialog();
            if (finishNumeratorForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }
            bool divideByFloors = finishNumeratorForm.DivideByFloors;

            List<Room> roomList = new FilteredElementCollector(doc)
                .OfClass(typeof(SpatialElement))
                .WhereElementIsNotElementType()
                .Where(r => r.GetType() == typeof(Room))
                .Cast<Room>()
                .Where(r => r.Area > 0)
                .ToList();

            if (divideByFloors == false)
            {
                List<Floor> floorList = new FilteredElementCollector(doc)
                       .OfClass(typeof(Floor))
                       .Cast<Floor>()
                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол" 
                       || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                       .ToList();
                if (floorList.Count == 0)
                {
                    TaskDialog.Show("Revit", "Пол в проекте не найден.\nПроверьте параметр \"Группа модели\" у перекрытий\nДля пола должно быть указано значение - \"Пол\" или \"Полы\" без ковычек");
                    return Result.Cancelled;
                }

                ProgressBarForm pbf = null;
                Thread m_Thread = new Thread(() => Application.Run(pbf = new ProgressBarForm(floorList.Count)));
                m_Thread.IsBackground = true;
                m_Thread.Start();
                int step = 0;
                Thread.Sleep(100);

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Заполнение номеров помещений");
                    //Очистка параметра "Помещение_Список номеров"
                    if (floorList.First().LookupParameter("Помещение_Список номеров") == null)
                    {
                        TaskDialog.Show("Revit", "У пола отсутствует параметр \"Помещение_Список номеров\"");
                        pbf.BeginInvoke(new Action(() => { pbf.Close(); }));
                        return Result.Cancelled;
                    }

                    foreach (Floor floor in floorList)
                    {
                        floor.LookupParameter("Помещение_Список номеров").Set("");
                    }
                    
                    foreach (Floor floor in floorList)
                    {
                        step += 1;
                        pbf.BeginInvoke(new Action(() => { pbf.progressBar_pb.Value = step; }));

                        Room room = null;
                        Floor floorForSolid = doc.GetElement(ElementTransformUtils.CopyElement(doc, floor.Id, (100 / 304.8) * XYZ.BasisZ).First()) as Floor;
                        GeometryElement geomFloorElement = floorForSolid.get_Geometry(new Options());
                        Solid floorSolid = null;
                        foreach (GeometryObject geomObj in geomFloorElement)
                        {
                            floorSolid = geomObj as Solid;
                            if (floorSolid != null) break;
                        }

                        foreach (Room r in roomList)
                        {
                            GeometryElement geomRoomElement = r.get_Geometry(new Options());
                            Solid roomSolid = null;
                            foreach (GeometryObject geomObj in geomRoomElement)
                            {
                                roomSolid = geomObj as Solid;
                                if (roomSolid != null) break;
                            }
                            Solid intersection = null;
                            try
                            {
                                intersection = BooleanOperationsUtils.ExecuteBooleanOperation(floorSolid, roomSolid, BooleanOperationsType.Intersect);
                            }
                            catch
                            {
                                TaskDialog.Show("Revit", "Не удалось обработать "
                                    + floor.FloorType.Name + "\nи помещение №" + r.Number.ToString()
                                    + " из за ошибок геометрии");
                            }
                            double volumeOfIntersection = 0;
                            if(intersection != null)
                            {
                                volumeOfIntersection = intersection.Volume;
                            }
                            if (volumeOfIntersection != 0)
                            {
                                room = r;
                                break;
                            }
                        }
                        doc.Delete(floorForSolid.Id);

                        if (room != null)
                        {
                            string floorDescription = floor.LookupParameter("Помещение_Список номеров").AsString();
                            string roomNumber = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
                            if (floorDescription != null & floorDescription != "")
                            {
                                if (floorDescription.Split(',').ToList().Contains(roomNumber))
                                {
                                    continue;
                                }
                                else
                                {
                                    List<Floor> floorListForFilling = new FilteredElementCollector(doc)
                                       .OfClass(typeof(Floor))
                                       .Cast<Floor>()
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол" 
                                       || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                                       .Where(f => f.FloorType.Id == floor.FloorType.Id)
                                       .ToList();
                                    foreach (Floor f in floorListForFilling)
                                    {
                                        f.LookupParameter("Помещение_Список номеров").Set(floorDescription + "," + roomNumber);
                                    }
                                }
                            }
                            else
                            {
                                List<Floor> floorListForFilling = new FilteredElementCollector(doc)
                                       .OfClass(typeof(Floor))
                                       .Cast<Floor>()
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                                       || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                                       .Where(f => f.FloorType.Id == floor.FloorType.Id)
                                       .ToList();
                                foreach (Floor f in floorListForFilling)
                                {
                                    f.LookupParameter("Помещение_Список номеров").Set(roomNumber);
                                }
                            }
                        }
                    }
                    pbf.BeginInvoke(new Action(() => { pbf.Close(); }));

                    List<Floor> floorListForSortedFilling = new FilteredElementCollector(doc)
                   .OfClass(typeof(Floor))
                   .Cast<Floor>()
                   .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                   .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                   || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                   .ToList();

                    List<ElementId> FloorTypeIdList = new List<ElementId>();

                    foreach (Floor floor in floorListForSortedFilling)
                    {
                        if (FloorTypeIdList.Contains(floor.FloorType.Id))
                        {
                            continue;
                        }
                        else
                        {
                            FloorTypeIdList.Add(floor.FloorType.Id);
                            string newfloorRoomsNumbersSorted = "";
                            string floorRoomsNumbers = floor.LookupParameter("Помещение_Список номеров").AsString();
                            if (floorRoomsNumbers != null & floorRoomsNumbers != "")
                            {
                                List<string> floorRoomsNumbersList = floorRoomsNumbers.Split(',').ToList();
                                floorRoomsNumbersList.Sort(new AlphanumComparatorFastString());
                                List<string> floorRoomsNumbersSortedList = floorRoomsNumbersList;
                                foreach (string st in floorRoomsNumbersSortedList)
                                {
                                    if (newfloorRoomsNumbersSorted == "")
                                    {
                                        newfloorRoomsNumbersSorted = st;
                                    }
                                    else
                                    {
                                        newfloorRoomsNumbersSorted += ", " + st;
                                    }
                                }


                            }
                            List<Floor> floorListForAddNewSortedFilling = new FilteredElementCollector(doc)
                               .OfClass(typeof(Floor))
                               .Cast<Floor>()
                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                               || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                               .Where(f => f.FloorType.Id == floor.FloorType.Id)
                               .ToList();
                            foreach (Floor f in floorListForAddNewSortedFilling)
                            {
                                f.LookupParameter("Помещение_Список номеров").Set(newfloorRoomsNumbersSorted);
                            }
                        }
                    }
                    t.Commit();
                }
            }

            if (divideByFloors == true)
            {
                List<Level> levelList = new FilteredElementCollector(doc)
                       .OfClass(typeof(Level))
                       .Cast<Level>()
                       .ToList();

                ProgressBarForm pbf = null;
                Thread m_Thread = new Thread(() => Application.Run(pbf = new ProgressBarForm(levelList.Count)));
                m_Thread.IsBackground = true;
                m_Thread.Start();
                int step = 0;
                Thread.Sleep(100);

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Заполнение номеров помещений");
                    foreach (Level lv in levelList)
                    {
                        step += 1;
                        pbf.BeginInvoke(new Action(() => { pbf.progressBar_pb.Value = step; }));

                        List<Floor> floorList = new FilteredElementCollector(doc)
                           .OfClass(typeof(Floor))
                           .Cast<Floor>()
                           .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                           .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                           || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                           .Where(f => f.LevelId == lv.Id)
                           .ToList();

                        if (floorList.Count != 0)
                        {
                            if (floorList.First().LookupParameter("Помещение_Список номеров") == null)
                            {
                                TaskDialog.Show("Revit", "У пола отсутствует параметр \"Помещение_Список номеров\"");
                                pbf.BeginInvoke(new Action(() => { pbf.Close(); }));
                                return Result.Cancelled;
                            }

                            //Очистка параметра "Помещение_Список номеров"
                            foreach (Floor floor in floorList)
                            {
                                floor.LookupParameter("Помещение_Список номеров").Set("");
                            }

                            foreach (Floor floor in floorList)
                            {
                                Room room = null;
                                Floor floorForSolid = doc.GetElement(ElementTransformUtils.CopyElement(doc, floor.Id, (100 / 304.8) * XYZ.BasisZ).First()) as Floor;
                                GeometryElement geomFloorElement = floorForSolid.get_Geometry(new Options());
                                Solid floorSolid = null;
                                foreach (GeometryObject geomObj in geomFloorElement)
                                {
                                    floorSolid = geomObj as Solid;
                                    if (floorSolid != null) break;
                                }

                                foreach (Room r in roomList)
                                {
                                    GeometryElement geomRoomElement = r.get_Geometry(new Options());
                                    Solid roomSolid = null;
                                    foreach (GeometryObject geomObj in geomRoomElement)
                                    {
                                        roomSolid = geomObj as Solid;
                                        if (roomSolid != null) break;
                                    }
                                    Solid intersection = null;
                                    try
                                    {
                                        intersection = BooleanOperationsUtils.ExecuteBooleanOperation(floorSolid, roomSolid, BooleanOperationsType.Intersect);
                                    }
                                    catch
                                    {
                                        TaskDialog.Show("Revit", "Не удалось обработать "
                                            + floor.FloorType.Name + "\nи помещение №" + r.Number.ToString()
                                            + " из за ошибок геометрии");
                                    }
                                    double volumeOfIntersection = 0;
                                    if (intersection != null)
                                    {
                                        volumeOfIntersection = intersection.Volume;
                                    }
                                    if (volumeOfIntersection != 0)
                                    {
                                        room = r;
                                        break;
                                    }
                                }
                                doc.Delete(floorForSolid.Id);

                                if (room != null)
                                {
                                    string floorDescription = floor.LookupParameter("Помещение_Список номеров").AsString();
                                    string roomNumber = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
                                    if (floorDescription != null & floorDescription != "")
                                    {
                                        if (floorDescription.Split(',').ToList().Contains(roomNumber))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            List<Floor> floorListForFilling = new FilteredElementCollector(doc)
                                               .OfClass(typeof(Floor))
                                               .Cast<Floor>()
                                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                                               || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                                               .Where(f => f.FloorType.Id == floor.FloorType.Id)
                                               .Where(f => f.LevelId == lv.Id)
                                               .ToList();
                                            foreach (Floor f in floorListForFilling)
                                            {
                                                f.LookupParameter("Помещение_Список номеров").Set(floorDescription + "," + roomNumber);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        List<Floor> floorListForFilling = new FilteredElementCollector(doc)
                                               .OfClass(typeof(Floor))
                                               .Cast<Floor>()
                                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                                               || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                                               .Where(f => f.FloorType.Id == floor.FloorType.Id)
                                               .Where(f => f.LevelId == lv.Id)
                                               .ToList();
                                        foreach (Floor f in floorListForFilling)
                                        {
                                            f.LookupParameter("Помещение_Список номеров").Set(roomNumber);
                                        }
                                    }
                                }
                            }

                            List<Floor> floorListForSortedFilling = new FilteredElementCollector(doc)
                               .OfClass(typeof(Floor))
                               .Cast<Floor>()
                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                               .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                               || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                               .Where(f => f.LevelId == lv.Id)
                               .ToList();

                            List<ElementId> FloorTypeIdList = new List<ElementId>();

                            foreach (Floor floor in floorListForSortedFilling)
                            {
                                if (FloorTypeIdList.Contains(floor.FloorType.Id))
                                {
                                    continue;
                                }
                                else
                                {
                                    FloorTypeIdList.Add(floor.FloorType.Id);
                                    string newfloorRoomsNumbersSorted = "";
                                    string floorRoomsNumbers = floor.LookupParameter("Помещение_Список номеров").AsString();
                                    if (floorRoomsNumbers != null & floorRoomsNumbers != "")
                                    {
                                        List<string> floorRoomsNumbersList = floorRoomsNumbers.Split(',').ToList();
                                        floorRoomsNumbersList.Sort(new AlphanumComparatorFastString());
                                        List<string> floorRoomsNumbersSortedList = floorRoomsNumbersList;
                                        foreach (string st in floorRoomsNumbersSortedList)
                                        {
                                            if (newfloorRoomsNumbersSorted == "")
                                            {
                                                newfloorRoomsNumbersSorted = st;
                                            }
                                            else
                                            {
                                                newfloorRoomsNumbersSorted += ", " + st;
                                            }
                                        }
                                    }
                                    List<Floor> floorListForAddNewSortedFilling = new FilteredElementCollector(doc)
                                       .OfClass(typeof(Floor))
                                       .Cast<Floor>()
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                                       .Where(f => f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Пол"
                                       || f.FloorType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Полы")
                                       .Where(f => f.FloorType.Id == floor.FloorType.Id)
                                       .Where(f => f.LevelId == lv.Id)
                                       .ToList();
                                    foreach (Floor f in floorListForAddNewSortedFilling)
                                    {
                                        f.LookupParameter("Помещение_Список номеров").Set(newfloorRoomsNumbersSorted);
                                    }
                                }
                            }
                        }
                    }
                    pbf.BeginInvoke(new Action(() => { pbf.Close(); }));
                    t.Commit();
                }
            }
            return Result.Succeeded;
        }
    }    
}
