using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_7_ElementsTransfer
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_7_ElementsTransfer : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Опции копирования
            CopyPasteOptions copyOptions = new CopyPasteOptions();
            copyOptions.SetDuplicateTypeNamesHandler(new CopyUseDestination());

            //Вызов формы с выбором типа копирования проемов
            CIT_04_7_ElementsTransferForm elementsTransferForm = new CIT_04_7_ElementsTransferForm();
            elementsTransferForm.ShowDialog();
            if (elementsTransferForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            bool floorTransferCheck = elementsTransferForm.FloorTransferCheck;
            bool columnTransferCheck = elementsTransferForm.СolumnTransferCheck;
            bool wallTransferCheck = elementsTransferForm.WallTransferCheck;
            bool beamTransferCheck = elementsTransferForm.BeamTransferCheck;
            bool foundatioTransferCheck = elementsTransferForm.FoundatioTransferCheck;

            bool replaceFloorType = elementsTransferForm.ReplaceFloorType;
            bool replaceСolumnType = elementsTransferForm.ReplaceСolumnType;
            bool replaceWallType = elementsTransferForm.ReplaceWallType;
            bool replaceBeamType = elementsTransferForm.ReplaceBeamType;

            //Переменная для связанных файлов
            Document doc2 = null;
            //Переменная для смещения связей
            Transform linkOrigin = null;
            //Список связанных файлов
            List <RevitLinkInstance> myRevitLinkInstanceList = new List<RevitLinkInstance>();

            //Вызов выбора связанных файлов, если выбрана какая-то из опций копирования элементов
            if (floorTransferCheck || columnTransferCheck || wallTransferCheck || beamTransferCheck || foundatioTransferCheck)
            {
                //Выбор связанного файла
                RevitLinkInstanceSelectionFilter selFilterRevitLinkInstance = new RevitLinkInstanceSelectionFilter(); //Вызов фильтра выбора
                List<Reference> selRevitLinkInstanceList = sel.PickObjects(ObjectType.Element, selFilterRevitLinkInstance, "Выберите связанные файлы!").ToList();//Получение ссылки на выбранную группу

                //Заполнение списка связанных файлов
                foreach (Reference refElem in selRevitLinkInstanceList)
                {
                    RevitLinkInstance myRevitLinkInstance = new FilteredElementCollector(doc)
                    .OfClass(typeof(RevitLinkInstance))
                    .Where(li => li.Id == refElem.ElementId)
                    .Cast<RevitLinkInstance>()
                    .First();
                    myRevitLinkInstanceList.Add(myRevitLinkInstance);
                }
            }

            //Обработка списка связанных файлов
            foreach (RevitLinkInstance rli in myRevitLinkInstanceList)
            {
                //Получение смещения связи
                linkOrigin = rli.GetTotalTransform();
                //Получение связанного файла
                doc2 = rli.GetLinkDocument();

                //Сбор перекрытий для копирования в файл
                ICollection<ElementId> floorIdList = new List<ElementId>();
                if (floorTransferCheck)
                {
                    List<Floor> floorList = new FilteredElementCollector(doc2)
                    .OfClass(typeof(Floor))
                    .OfCategory(BuiltInCategory.OST_Floors)
                    .Cast<Floor>()
                    .Where(f => f.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).AsInteger() == 1)
                    .ToList();

                    foreach (Floor fl in floorList)
                    {
                        floorIdList.Add(fl.Id);
                    }
                }

                //Сбор колонн для копирования в файл
                ICollection<ElementId> columnIdList = new List<ElementId>();
                if (columnTransferCheck)
                {
                    List<FamilyInstance> columnList = new FilteredElementCollector(doc2)
                    .OfClass(typeof(FamilyInstance))
                    .OfCategory(BuiltInCategory.OST_StructuralColumns)
                    .Cast<FamilyInstance>()
                    .ToList();

                    foreach (FamilyInstance c in columnList)
                    {
                        columnIdList.Add(c.Id);
                    }
                }

                //Сбор стен для копирования в файл
                ICollection<ElementId> wallIdList = new List<ElementId>();
                if (wallTransferCheck)
                {
                    List<Wall> wallList = new FilteredElementCollector(doc2)
                    .OfClass(typeof(Wall))
                    .Cast<Wall>()
                    .Where(w => w.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger() == 1)
                    .ToList();

                    foreach (Wall w in wallList)
                    {
                        wallIdList.Add(w.Id);
                    }
                }

                //Сбор балок для копирования в файл
                ICollection<ElementId> beamIdList = new List<ElementId>();
                if (beamTransferCheck)
                {
                    List<FamilyInstance> beamList = new FilteredElementCollector(doc2)
                    .OfClass(typeof(FamilyInstance))
                    .OfCategory(BuiltInCategory.OST_StructuralFraming)
                    .Cast<FamilyInstance>()
                    .Where(f => f.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM) != null)
                    .Where(f => f.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsValueString().Contains("Бетон")
                    || f.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsValueString().Contains("елезобетон"))
                    .ToList();

                    foreach (FamilyInstance fi in beamList)
                    {
                        beamIdList.Add(fi.Id);
                    }
                }

                //Сбор фундаментов для копирования в файл
                ICollection<ElementId> foundationIdList = new List<ElementId>();
                if (foundatioTransferCheck)
                {
                    List<FamilyInstance> foundationList = new FilteredElementCollector(doc2)
                    .OfClass(typeof(FamilyInstance))
                    .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                    .Cast<FamilyInstance>()
                    .ToList();

                    foreach (FamilyInstance f in foundationList)
                    {
                        foundationIdList.Add(f.Id);
                    }
                }

                //Транзакция для копирования элементов из связи
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Копирование элементов");
                    if (columnTransferCheck & columnIdList.Count != 0)
                    {
                        ElementTransformUtils.CopyElements(doc2, columnIdList, doc, linkOrigin, copyOptions);
                    }

                    if (wallTransferCheck & wallIdList.Count != 0)
                    {
                        foreach(ElementId wallId in wallIdList)
                        {
                            List<ElementId> tempWallIdList = new List<ElementId>();
                            tempWallIdList.Add(wallId);
                            ElementTransformUtils.CopyElements(doc2, tempWallIdList, doc, linkOrigin, copyOptions);
                        }
                    }

                    if (floorTransferCheck & floorIdList.Count != 0)
                    {
                        ElementTransformUtils.CopyElements(doc2, floorIdList, doc, linkOrigin, copyOptions);
                    }

                    if (beamTransferCheck & beamIdList.Count != 0)
                    {
                        ElementTransformUtils.CopyElements(doc2, beamIdList, doc, linkOrigin, copyOptions);
                    }

                    if (foundatioTransferCheck & foundationIdList.Count != 0)
                    {
                        ElementTransformUtils.CopyElements(doc2, foundationIdList, doc, linkOrigin, copyOptions);
                    }

                    doc.Regenerate();
                    t.Commit();
                }
            }

            //Транзакция для замены типов
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Замена элементов");
                //Замена типов перекрытий
                if (replaceFloorType)
                {
                    //Список всех перекрытий в проекте
                    List<Floor> floorListForReplacement = new FilteredElementCollector(doc)
                        .OfClass(typeof(Floor))
                        .OfCategory(BuiltInCategory.OST_Floors)
                        .Cast<Floor>()
                        .Where(f => f.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).AsInteger() == 1)
                        .ToList();

                    //Список типов перекрытий с именем "В25 200мм"
                    List<FloorType> floorTypesListB20200mm = new FilteredElementCollector(doc)
                        .OfClass(typeof(FloorType))
                        .OfCategory(BuiltInCategory.OST_Floors)
                        .Where(ft => ft.Name == "В25 200мм")
                        .Cast<FloorType>().ToList();

                    //Если тип перекрытия "В25 200мм" отсутствует в проекте
                    if (floorTypesListB20200mm.Count() == 0)
                    {
                        TaskDialog.Show("Revit", "Тип перекрытия \"В25 200мм\" не найден");
                        return Result.Cancelled;
                    }
                    //Если тип перекрытия "В25 200мм" есть в проекте добавляем его в переменную. Используем его для создания недостающих типов.
                    FloorType floorTypeB20200mm = floorTypesListB20200mm.First();

                    //Список толщин перекрытий в проекте
                    List<double> floorThicknessList = new List<double>();
                    foreach (Floor floor in floorListForReplacement)
                    {
                        double floorThickness = floor.FloorType.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM).AsDouble();
                        if (floorThicknessList.Contains(floorThickness)) continue;
                        else floorThicknessList.Add(floorThickness);
                    }

                    //Обработка списка толщин перекрытий
                    foreach (double thickness in floorThicknessList)
                    {
                        //Все перекрытия с толщиной thickness
                        List<Floor> floorListForReplacementTemp = new FilteredElementCollector(doc)
                        .OfClass(typeof(Floor))
                        .OfCategory(BuiltInCategory.OST_Floors)
                        .Cast<Floor>()
                        .Where(f => f.FloorType.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM).AsDouble() == thickness)
                        .ToList();

                        //Поиск типа перекрытия для замены
                        List<FloorType> floorTypesListTemp = new FilteredElementCollector(doc)
                            .OfClass(typeof(FloorType))
                            .OfCategory(BuiltInCategory.OST_Floors)
                            .Where(ft => ft.Name == "В25 " + thickness * 304.8 + "мм")
                            .Cast<FloorType>()
                            .ToList();

                        //Переменная для типа перекрытия
                        FloorType newFloorType = null;

                        //Если нужный тип перекрытия не найден в проекте, создаем его
                        if (floorTypesListTemp.Count() == 0)
                        {
                            newFloorType = floorTypeB20200mm.Duplicate("В25 " + thickness * 304.8 + "мм") as FloorType;

                            CompoundStructure compound = newFloorType.GetCompoundStructure();
                            compound.SetLayerWidth(0, thickness);
                            newFloorType.SetCompoundStructure(compound);
                            newFloorType.LookupParameter("О_Наименование").Set("Перекрытие t = " + thickness * 304.8 + "мм");
                        }

                        //Если нужный тип найден в проекте
                        else
                        {
                            newFloorType = floorTypesListTemp.First();
                        }

                        //Замена типа перекрытия толщиной thickness
                        foreach (Floor fl in floorListForReplacementTemp)
                        {
                            if (fl.FloorType != newFloorType)
                            {
                                fl.FloorType = newFloorType;
                            }
                        }
                    }
                }

                //Замена типов колонн
                if(replaceСolumnType)
                {
                    //Список всех колонн в проекте
                    List<FamilyInstance> columnListForReplacement = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .OfCategory(BuiltInCategory.OST_StructuralColumns)
                    .Cast<FamilyInstance>()
                    .Where(c => c.HasSweptProfile())
                    .ToList();

                    //Список типов колонн семейства "210_Прямоугольного сечения (НесКол_2ур)" с именем "400 x 400 мм"
                    List<FamilySymbol> columnTypesList400 = new FilteredElementCollector(doc)
                        .OfClass(typeof(FamilySymbol))
                        .OfCategory(BuiltInCategory.OST_StructuralColumns)
                        .Cast<FamilySymbol>()
                        .Where(fs => fs.Family.Name == "210_Прямоугольного сечения (НесКол_2ур)")
                        .Where(fs => fs.Name == "400 x 400 мм")
                        .ToList();

                    //Если тип "400 x 400 мм" семейства "210_Прямоугольного сечения (НесКол_2ур)" отсутствует в проекте
                    if (columnTypesList400.Count() == 0)
                    {
                        TaskDialog.Show("Revit", "Тип колонны \"400 x 400 мм\" семейства\n\"210_Прямоугольного сечения (НесКол_2ур)\"\nне найден");
                        return Result.Cancelled;
                    }
                    //Если тип "400 x 400 мм" семейства "210_Прямоугольного сечения (НесКол_2ур)" есть в проекте добавляем его в переменную. Используем его для создания недостающих типов.
                    FamilySymbol columnType400 = columnTypesList400.First();

                    //Обработка списка колонн
                    foreach (FamilyInstance column in columnListForReplacement)
                    {
                        GeometryElement geomRoomElement = column.Symbol.get_Geometry(new Options());
                        Solid columnSolid = null;
                        foreach (GeometryObject geomObj in geomRoomElement)
                        {
                            columnSolid = geomObj as Solid;
                            if (columnSolid != null) break;
                        }

                        FaceArray FacesList = columnSolid.Faces;
                        Face targetFace = null;
                        double faceArea = 9999;

                        foreach (PlanarFace face in FacesList)
                        {
                            if (face.Area < faceArea)
                            {
                                faceArea = face.Area;
                                targetFace = face;
                            }
                        }

                        CurveLoop curveLoop = targetFace.GetEdgesAsCurveLoops().First();
                        double columnHeightDouble = 0;
                        double columnWidthDouble = 9999;
                        foreach (Line ln in curveLoop)
                        {
                            if (columnHeightDouble < ln.Length)
                            {
                                columnHeightDouble = ln.Length;
                            }

                            if (columnWidthDouble > ln.Length)
                            {
                                columnWidthDouble = ln.Length;
                            }
                        }
                        string columnWidthStr = Math.Round(columnWidthDouble * 304.8).ToString();
                        string columnHeightStr = Math.Round(columnHeightDouble * 304.8).ToString();

                        //Поиск типа колонны для замены
                        List<FamilySymbol> columnTypesListTemp = new FilteredElementCollector(doc)
                            .OfClass(typeof(FamilySymbol))
                            .OfCategory(BuiltInCategory.OST_StructuralColumns)
                            .Cast<FamilySymbol>()
                            .Where(fs => fs.Family.Name == "210_Прямоугольного сечения (НесКол_2ур)")
                            .Where(fs => fs.Name == columnWidthStr + " x " + columnHeightStr + " мм")
                            .ToList();


                        //Переменная для типа колонны
                        FamilySymbol newFamilySymbol = null;
                        //Если нужный тип колонны не найден в проекте, создаем его
                        if (columnTypesListTemp.Count() == 0)
                        {
                            newFamilySymbol = columnType400.Duplicate(columnWidthStr + " x " + columnHeightStr + " мм") as FamilySymbol;

                            newFamilySymbol.LookupParameter("Рзм.Ширина").Set(columnWidthDouble);
                            newFamilySymbol.LookupParameter("Рзм.Высота").Set(columnHeightDouble);
                        }
                        //Если нужный тип найден в проекте
                        else
                        {
                            newFamilySymbol = columnTypesListTemp.First();
                        }

                        if (column.Symbol != newFamilySymbol)
                        {
                            column.Symbol = newFamilySymbol;
                        }

                    }
                }

                //Замена типов стен
                if (replaceWallType)
                {
                    List<Wall> wallListForReplacement = new FilteredElementCollector(doc)
                    .OfClass(typeof(Wall))
                    .Cast<Wall>()
                    .Where(w => w.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger() == 1)
                    .Where(w => doc.GetElement(w.WallType.GetCompoundStructure().GetLayers()[0].MaterialId).get_Parameter(BuiltInParameter.MATERIAL_NAME).AsString().Contains("Бетон")
                    || doc.GetElement(w.WallType.GetCompoundStructure().GetLayers()[0].MaterialId).get_Parameter(BuiltInParameter.MATERIAL_NAME).AsString().Contains("елезобетон"))
                    .ToList();

                    //Список типов стен с именем "В25 200мм"
                    List<WallType> wallTypesListB20200mm = new FilteredElementCollector(doc)
                    .OfClass(typeof(WallType))
                    .Cast<WallType>()
                    .Where(wt => wt.Name == "В25 200мм")
                    .ToList();

                    //Если тип стены "В25 200мм" отсутствует в проекте
                    if (wallTypesListB20200mm.Count() == 0)
                    {
                        TaskDialog.Show("Revit", "Тип стены \"В25 200мм\" не найден");
                        return Result.Cancelled;
                    }
                    //Если тип стены "В25 200мм" есть в проекте добавляем его в переменную. Используем его для создания недостающих типов.
                    WallType wallTypeB20200mm = wallTypesListB20200mm.First();

                    //Список толщин стен в проекте
                    List<double> wallThicknessList = new List<double>();
                    foreach (Wall wall in wallListForReplacement)
                    {
                        double wallThickness = wall.WallType.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble();
                        if (wallThicknessList.Contains(wallThickness)) continue;
                        else wallThicknessList.Add(wallThickness);
                    }

                    //Обработка списка толщин перекрытий
                    foreach (double thickness in wallThicknessList)
                    {
                        //Все стены с толщиной thickness
                        List<Wall> wallListForReplacementTemp = new FilteredElementCollector(doc)
                            .OfClass(typeof(Wall))
                            .Cast<Wall>()
                            .Where(w => w.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger() == 1)
                            .Where(w => doc.GetElement(w.WallType.GetCompoundStructure().GetLayers()[0].MaterialId).get_Parameter(BuiltInParameter.MATERIAL_NAME).AsString().Contains("Бетон")
                            || doc.GetElement(w.WallType.GetCompoundStructure().GetLayers()[0].MaterialId).get_Parameter(BuiltInParameter.MATERIAL_NAME).AsString().Contains("елезобетон"))
                            .Where(w => w.WallType.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble() == thickness)
                            .ToList();

                        //Поиск типа стены для замены
                        List<WallType> wallTypesListTemp = new FilteredElementCollector(doc)
                            .OfClass(typeof(WallType))
                            .Where(wt => wt.Name == "В25 " + thickness * 304.8 + "мм")
                            .Cast<WallType>()
                            .ToList();

                        //Переменная для типа перекрытия
                        WallType newWallType = null;

                        //Если нужный тип стены не найден в проекте, создаем его
                        if (wallTypesListTemp.Count() == 0)
                        {
                            newWallType = wallTypeB20200mm.Duplicate("В25 " + thickness * 304.8 + "мм") as WallType;

                            CompoundStructure compound = newWallType.GetCompoundStructure();
                            compound.SetLayerWidth(0, thickness);
                            newWallType.SetCompoundStructure(compound);
                            newWallType.LookupParameter("О_Наименование").Set("Стена монолитная t = " + thickness * 304.8 + "мм");
                        }

                        //Если нужный тип найден в проекте
                        else
                        {
                            newWallType = wallTypesListTemp.First();
                        }

                        //Замена типа стены толщиной thickness
                        foreach (Wall wl in wallListForReplacementTemp)
                        {
                            if (wl.WallType != newWallType)
                            {
                                wl.WallType = newWallType;
                            }
                        }
                    }
                }

                //Замена типов балок
                if (replaceBeamType)
                {
                    List<FamilyInstance> beamListForReplacement = new FilteredElementCollector(doc)
                        .OfClass(typeof(FamilyInstance))
                        .OfCategory(BuiltInCategory.OST_StructuralFraming)
                        .Cast<FamilyInstance>()
                        .Where(f => f.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM) != null)
                        .Where(f => f.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsValueString().Contains("Бетон")
                        || f.Symbol.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsValueString().Contains("елезобетон"))
                        .ToList();

                    //Список типов колонн семейства "210_Прямоугольного сечения (НесКаркас_Балка)" с именем "300 x 600 мм"
                    List<FamilySymbol> beamTypesList300x600 = new FilteredElementCollector(doc)
                        .OfClass(typeof(FamilySymbol))
                        .OfCategory(BuiltInCategory.OST_StructuralFraming)
                        .Cast<FamilySymbol>()
                        .Where(fs => fs.Family.Name == "210_Прямоугольного сечения (НесКаркас_Балка)")
                        .Where(fs => fs.Name == "300 x 600 мм")
                        .ToList();

                    //Если тип "300 x 600 мм" семейства "210_Прямоугольного сечения (НесКаркас_Балка)" отсутствует в проекте
                    if (beamTypesList300x600.Count() == 0)
                    {
                        TaskDialog.Show("Revit", "Тип колонны \"300 x 600 мм\" семейства\n\"210_Прямоугольного сечения (НесКаркас_Балка)\"\nне найден");
                        return Result.Cancelled;
                    }
                    //Если тип "300 x 600 мм" семейства "210_Прямоугольного сечения (НесКаркас_Балка)" есть в проекте добавляем его в переменную. Используем его для создания недостающих типов.
                    FamilySymbol beamType300x600 = beamTypesList300x600.First();

                    foreach (FamilyInstance beam in beamListForReplacement)
                    {

                        GeometryElement geomRoomElement = beam.Symbol.get_Geometry(new Options());
                        Solid beamSolid = null;
                        foreach (GeometryObject geomObj in geomRoomElement)
                        {
                            beamSolid = geomObj as Solid;
                            if (beamSolid != null) break;
                        }

                        FaceArray FacesList = beamSolid.Faces;
                        Face targetFace = null;
                        double faceArea = 9999;

                        foreach (PlanarFace face in FacesList)
                        {
                            if (face.Area < faceArea)
                            {
                                faceArea = face.Area;
                                targetFace = face;
                            }
                        }
                        CurveLoop curveLoop = targetFace.GetEdgesAsCurveLoops().First();
                        double beamHeightDouble = 0;
                        double beamWidthDouble = 9999;
                        foreach (Line ln in curveLoop)
                        {
                            if (beamHeightDouble < ln.Length)
                            {
                                beamHeightDouble = ln.Length;
                            }

                            if (beamWidthDouble > ln.Length)
                            {
                                beamWidthDouble = ln.Length;
                            }
                        }

                        string beamWidthStr = Math.Round(beamWidthDouble * 304.8).ToString();
                        string beamHeightStr = Math.Round(beamHeightDouble * 304.8).ToString();

                        //Поиск типа балки для замены
                        List<FamilySymbol> beamTypesListTemp = new FilteredElementCollector(doc)
                            .OfClass(typeof(FamilySymbol))
                            .OfCategory(BuiltInCategory.OST_StructuralFraming)
                            .Cast<FamilySymbol>()
                            .Where(fs => fs.Family.Name == "210_Прямоугольного сечения (НесКаркас_Балка)")
                            .Where(fs => fs.Name == beamWidthStr + " x " + beamHeightStr + " мм")
                            .ToList();


                        //Переменная для типа перекрытия
                        FamilySymbol newFamilySymbol = null;

                        //Если нужный тип колонны не найден в проекте, создаем его
                        if (beamTypesListTemp.Count() == 0)
                        {
                            newFamilySymbol = beamType300x600.Duplicate(beamWidthStr + " x " + beamHeightStr + " мм") as FamilySymbol;

                            newFamilySymbol.LookupParameter("Рзм.Ширина").Set(beamWidthDouble);
                            newFamilySymbol.LookupParameter("Рзм.Высота").Set(beamHeightDouble);
                        }
                        //Если нужный тип найден в проекте
                        else
                        {
                            newFamilySymbol = beamTypesListTemp.First();
                        }

                        if (beam.Symbol != newFamilySymbol)
                        {
                            beam.Symbol = newFamilySymbol;
                        }
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
    internal class CopyUseDestination : IDuplicateTypeNamesHandler
    {
        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }
}
