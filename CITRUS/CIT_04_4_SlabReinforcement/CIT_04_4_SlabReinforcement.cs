using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CITRUS.CIT_04_4_SlabReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_4_SlabReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение достпа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Выбор плит для армирования
            FloorSelectionFilter selFilter = new FloorSelectionFilter();
            IList<Reference> selSlabs = sel.PickObjects(ObjectType.Element, selFilter, "Выберите плиту!");
            if (selSlabs.Count == 0)
            {
                return Result.Succeeded;
            }
            List<Floor> floorList = new List<Floor>();

            foreach (Reference floorRef in selSlabs)
            {
                floorList.Add(doc.GetElement(floorRef) as Floor);
            }
            if (floorList.Count == 0)
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

            //Создание списков типов арматуры для формы
            //Список для низ X
            List<RebarBarType> bottomXDirectionRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();
            //Список для низ Y
            List<RebarBarType> bottomYDirectionRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();
            //Список для верх X
            List<RebarBarType> topXDirectionRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();
            //Список для верх Y
            List<RebarBarType> topYDirectionRebarTapesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            //Создание списков типов защитных слоёв для формы
            //Список типов защитных слоев арматуры верх
            List<RebarCoverType> rebarCoverTypesListForTop = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();
            //Список типов защитных слоев арматуры низ
            List<RebarCoverType> rebarCoverTypesListForBottom = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();
            
            //Вызов формы
            CIT_04_4_SlabReinforcementForm slabReinforcementForm = new CIT_04_4_SlabReinforcementForm(bottomXDirectionRebarTapesList
                , bottomYDirectionRebarTapesList
                , topXDirectionRebarTapesList
                , topYDirectionRebarTapesList
                , rebarCoverTypesListForTop
                , rebarCoverTypesListForBottom);

            slabReinforcementForm.ShowDialog();
            if (slabReinforcementForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            //Получение результатов формы
            //Тип арматуры низ X
            RebarBarType bottomXDirectionRebarTape = slabReinforcementForm.mySelectionBottomXDirectionRebarTape;
            Parameter bottomXDirectionRebarTapeDiamParam = bottomXDirectionRebarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double bottomXDirectionRebarDiam = bottomXDirectionRebarTapeDiamParam.AsDouble();

            //Тип арматуры низ Y
            RebarBarType bottomYDirectionRebarTape = slabReinforcementForm.mySelectionBottomYDirectionRebarTape;
            Parameter bottomYDirectionRebarTapeDiamParam = bottomYDirectionRebarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double bottomYDirectionRebarDiam = bottomYDirectionRebarTapeDiamParam.AsDouble();

            //Тип арматуры верх X
            RebarBarType topXDirectionRebarTape = slabReinforcementForm.mySelectionTopXDirectionRebarTape;
            Parameter topXDirectionRebarTapeDiamParam = topXDirectionRebarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double topXDirectionRebarDiam = topXDirectionRebarTapeDiamParam.AsDouble();

            //Тип арматуры верх Y
            RebarBarType topYDirectionRebarTape = slabReinforcementForm.mySelectionTopYDirectionRebarTape;
            Parameter topYDirectionRebarTapeDiamParam = topYDirectionRebarTape.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER);
            double topYDirectionRebarDiam = topYDirectionRebarTapeDiamParam.AsDouble();

            //Шаг арматуры низ X
            double bottomXDirectionRebarSpacing = slabReinforcementForm.BottomXDirectionRebarSpacing / 304.8;
            //Шаг арматуры низ Y 
            double bottomYDirectionRebarSpacing = slabReinforcementForm.BottomYDirectionRebarSpacing / 304.8;
            //Шаг арматуры верх X
            double topXDirectionRebarSpacing = slabReinforcementForm.TopXDirectionRebarSpacing / 304.8;
            //Шаг арматуры верх Y
            double topYDirectionRebarSpacing = slabReinforcementForm.TopYDirectionRebarSpacing / 304.8;

            //Диаметр П-шки обрамления
            double perimeterFramingDiam = slabReinforcementForm.PerimeterFramingDiam / 304.8;
            //Нахлёст П-шки обрамления
            double perimeterFramingOverlaping = slabReinforcementForm.PerimeterFramingOverlaping / 304.8;
            //Защитный слой торца П-шки
            double perimeterFramingEndCoverLayer = slabReinforcementForm.PerimeterFramingEndCoverLayer / 304.8 + perimeterFramingDiam;
            //Шаг П-шки
            double perimeterFramingStep = slabReinforcementForm.PerimeterFramingStep / 304.8;

            //Выбор типа защитного слоя сверху
            RebarCoverType rebarCoverTypeForTop = slabReinforcementForm.mySelectionRebarCoverTypeForTop;
            double rebarCoverTop = rebarCoverTypeForTop.CoverDistance;
            //Выбор типа защитного слоя снизу
            RebarCoverType rebarCoverTypeForBottom = slabReinforcementForm.mySelectionRebarCoverTypeForBottom;
            double rebarCoverBottom = rebarCoverTypeForBottom.CoverDistance;

            //Защитный слой П-шки сверху
            double perimeterFramingTopCoverLayer = rebarCoverTop + topYDirectionRebarDiam;
            //Защитный слой П-шки снизу
            double perimeterFramingBottomCoverLayer = rebarCoverBottom;

            bool perimeterFraming = slabReinforcementForm.PerimeterFraming;
            bool bottomXDirection = slabReinforcementForm.BottomXDirection;
            bool bottomYDirection = slabReinforcementForm.BottomYDirection;
            bool topXDirection = slabReinforcementForm.TopXDirection;
            bool topYDirection = slabReinforcementForm.TopYDirection;

            //Старт транзакции
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Армирование плиты");
                foreach (Floor floor in floorList)
                {
                    //Установка верхнего и нижнего защитных слоев плиты
                    floor.get_Parameter(BuiltInParameter.CLEAR_COVER_TOP).Set(rebarCoverTypeForTop.Id);
                    floor.get_Parameter(BuiltInParameter.CLEAR_COVER_BOTTOM).Set(rebarCoverTypeForBottom.Id);
                    //Толщина плиты
                    double floorThickness = floor.FloorType.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM).AsDouble();
                    //Объявляем переменную для семейства обрамления
                    FamilySymbol targetPerimeterFramingFamilySymbol = null;

                    if (perimeterFraming == true)
                    {

                        //Семейство обрамления проема
                        List<FamilySymbol> perimeterFramingFamilySymbolList = new FilteredElementCollector(doc)
                            .OfClass(typeof(FamilySymbol))
                            .Cast<FamilySymbol>()
                            .Where(fs => fs.FamilyName == "263_Обрамление периметра плиты (ОбщМод_Линия)")
                            .Where(fs => Math.Round(fs.LookupParameter("Диаметр П").AsDouble(), 6) == Math.Round(perimeterFramingDiam, 6))
                            .Where(fs => Math.Round(fs.LookupParameter("Анкеровка П").AsDouble(), 6) == Math.Round(perimeterFramingOverlaping, 6))
                            .ToList();

                        //Если семейство обрамления проема с заданными параметрами не найдено
                        if (perimeterFramingFamilySymbolList.Count == 0)
                        {
                            //Семейство обрамления проема проверить существует ли оно в проекте
                            perimeterFramingFamilySymbolList = new FilteredElementCollector(doc)
                                .OfClass(typeof(FamilySymbol))
                                .Cast<FamilySymbol>()
                                .Where(fs => fs.FamilyName == "263_Обрамление периметра плиты (ОбщМод_Линия)")
                                .ToList();
                            //Если не существует
                            if (perimeterFramingFamilySymbolList.Count == 0)
                            {
                                TaskDialog.Show("Revit", "Семейство \"263_Обрамление периметра плиты (ОбщМод_Линия)\" не найдено!");
                                return Result.Cancelled;
                            }
                            //Если существует взять любой тип семейства обрамления для создания нового
                            FamilySymbol typeForCopy = perimeterFramingFamilySymbolList.First();

                            //Создание нового типа семейства обрамления
                            targetPerimeterFramingFamilySymbol = typeForCopy.Duplicate("D=" + perimeterFramingDiam * 304.8 + "мм"
                                + ", Нахлест =" + perimeterFramingOverlaping * 304.8 + "мм") as FamilySymbol;

                            //Задание требуемых параметров
                            targetPerimeterFramingFamilySymbol.LookupParameter("Диаметр П").Set(perimeterFramingDiam);
                            targetPerimeterFramingFamilySymbol.LookupParameter("Анкеровка П").Set(perimeterFramingOverlaping);
                        }
                        //Если семейство обрамления проема с заданными параметрами найдено
                        else
                        {
                            targetPerimeterFramingFamilySymbol = perimeterFramingFamilySymbolList.First();
                        }
                    }

                    //Вектор направления пролёта перекрытия
                    double spanDirectionAngle = floor.SpanDirectionAngle;
                    XYZ zeroPoint = new XYZ(0, 0, 0);
                    XYZ directionPointStart = new XYZ(1, 0, 0);
                    Transform rot = Transform.CreateRotationAtPoint(XYZ.BasisZ, spanDirectionAngle, zeroPoint);
                    XYZ directionPoint = rot.OfPoint(directionPointStart);
                    XYZ directionVector = (directionPoint - zeroPoint).Normalize();

                    //Получение верхней грани плиты перекрытия
                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    GeometryElement geomFloorElement = floor.get_Geometry(opt);
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
                        if (Math.Round(pf.FaceNormal.Normalize().Z) == XYZ.BasisZ.Z)
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

                    if (bottomXDirection == true)
                    {


                        //Создание армирования по площади для низ X
                        AreaReinforcement areaReinforcementBottomXDirection = AreaReinforcement.Create(doc
                            , floor
                            , curveList
                            , directionVector
                            , areaReinforcementType.Id
                            , bottomXDirectionRebarTape.Id
                            , ElementId.InvalidElementId);

                        areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1).Set(1);
                        areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2).Set(0);
                        areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1).Set(0);
                        areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2).Set(0);
                        areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_BOTTOM_DIR_1).Set(bottomXDirectionRebarSpacing);
                        areaReinforcementBottomXDirection.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM).Set("низ X фон");
                    }

                    if (bottomYDirection == true)
                    {
                        //Создание армирования по площади для низ Y
                        AreaReinforcement areaReinforcementBottomYDirection = AreaReinforcement.Create(doc
                            , floor
                            , curveList
                            , directionVector
                            , areaReinforcementType.Id
                            , bottomYDirectionRebarTape.Id
                            , ElementId.InvalidElementId);

                        areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1).Set(0);
                        areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2).Set(1);
                        areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1).Set(0);
                        areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2).Set(0);
                        areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_BOTTOM_DIR_2).Set(bottomYDirectionRebarSpacing);
                        areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM).Set("низ Y фон");
                        areaReinforcementBottomYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ADDL_BOTTOM_OFFSET).Set(bottomXDirectionRebarDiam);
                    }

                    if (topXDirection == true)
                    {
                        //Создание армирования по площади для верх X
                        AreaReinforcement areaReinforcemenTopXDirection = AreaReinforcement.Create(doc
                            , floor
                            , curveList
                            , directionVector
                            , areaReinforcementType.Id
                            , topXDirectionRebarTape.Id
                            , ElementId.InvalidElementId);

                        areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1).Set(0);
                        areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2).Set(0);
                        areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1).Set(1);
                        areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2).Set(0);
                        areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_TOP_DIR_1).Set(topXDirectionRebarSpacing);
                        areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM).Set("верх X фон");
                        areaReinforcemenTopXDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ADDL_TOP_OFFSET).Set(topYDirectionRebarDiam);
                    }

                    if (topYDirection == true)
                    {


                        //Создание армирования по площади для верх Y
                        AreaReinforcement areaReinforcemenTopYDirection = AreaReinforcement.Create(doc
                            , floor
                            , curveList
                            , directionVector
                            , areaReinforcementType.Id
                            , topYDirectionRebarTape.Id
                            , ElementId.InvalidElementId);

                        areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_1).Set(0);
                        areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_BOTTOM_DIR_2).Set(0);
                        areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_1).Set(0);
                        areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ACTIVE_TOP_DIR_2).Set(1);
                        areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_SPACING_TOP_DIR_2).Set(topYDirectionRebarSpacing);
                        areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.NUMBER_PARTITION_PARAM).Set("верх Y фон");
                        areaReinforcemenTopYDirection.get_Parameter(BuiltInParameter.REBAR_SYSTEM_ADDL_TOP_OFFSET).Set(0);
                    }

                    if (perimeterFraming == true)
                    {
                        //Устройство обрамления по всем контурам перекрытия
                        foreach (CurveLoop cl in curveLoopList)
                        {
                            foreach (Line ln in cl)
                            {
                                FamilyInstance fi = doc.Create.NewFamilyInstance(myFace, ln, targetPerimeterFramingFamilySymbol);
                                fi.LookupParameter("Защитный слой верх").Set(perimeterFramingTopCoverLayer);
                                fi.LookupParameter("Защитный слой низ").Set(perimeterFramingBottomCoverLayer);
                                fi.LookupParameter("Защитный слой торец").Set(perimeterFramingEndCoverLayer);
                                fi.LookupParameter("Толщина плиты").Set(floorThickness);
                                fi.LookupParameter("Мсв.Шаг").Set(perimeterFramingStep);
                                fi.LookupParameter("Мрк.МаркаКонструкции").Set(floor.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString());
                            }
                        }
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
