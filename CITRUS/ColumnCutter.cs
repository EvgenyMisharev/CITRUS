using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class ColumnCutter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<FamilyInstance> columnsList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            List<Level> levelsList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .OrderBy(lv => lv.Elevation)
                .ToList();



            using (Transaction t = new Transaction(doc))
            {
                t.Start("Резчик колонн");
                foreach (FamilyInstance column in columnsList)
                {
                    FamilySymbol columnFamilySymbol = column.Symbol;
                    XYZ columnLocationPoint = (column.Location as LocationPoint).Point;
                    double columnRotation = (column.Location as LocationPoint).Rotation;
                    Line rotationAxisForNewColumn = Line.CreateBound(columnLocationPoint, columnLocationPoint + 100 * XYZ.BasisZ);
                    double columnBaseLevelOffset = column.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM).AsDouble();
                    double columnTopLevelOffset = column.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM).AsDouble();
                    string columnMark = column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();

                    ElementId columnBaseLevelId = column.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId();
                    ElementId columnTopLevelId = column.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId();

                    //Найти индекс базового уровня колонны
                    int columnBaseLevelIndex = levelsList.FindIndex(lv => lv.Id == columnBaseLevelId);
                    int columnTopLeveIndex = levelsList.FindIndex(lv => lv.Id == columnTopLevelId);
                    List<Level> shortLevelsList = new List<Level>();
                    if (columnTopLevelId != levelsList[columnBaseLevelIndex + 1].Id)
                    {
                        for (int i = 0; i<levelsList.Count(); i++)
                        {
                            if (i >= columnBaseLevelIndex && i <= columnTopLeveIndex)
                            {
                                shortLevelsList.Add(levelsList[i]);
                            }
                        }
                        if (columnBaseLevelIndex != columnTopLeveIndex)
                        {
                            doc.Delete(column.Id);
                        }
                        
                    }

                    for (int i = 0; i < shortLevelsList.Count() - 1; i++)
                    {
                        FamilyInstance newColumn = doc.Create.NewFamilyInstance(columnLocationPoint
                            , columnFamilySymbol
                            , shortLevelsList[i]
                            , StructuralType.Column);

                        newColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(shortLevelsList[i+1].Id);
                        if (i == 0)
                        {
                            newColumn.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM).Set(columnBaseLevelOffset);
                        }
                        else
                        {
                            newColumn.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM).Set(0);
                        }
                        if (i == shortLevelsList.Count() - 2)
                        {
                            newColumn.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM).Set(columnTopLevelOffset);
                        }
                        else
                        {
                            newColumn.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_OFFSET_PARAM).Set(0);
                        }

                        ElementTransformUtils.RotateElement(doc, newColumn.Id, rotationAxisForNewColumn, columnRotation);

                        newColumn.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(columnMark);
                    }
                }

                 t.Commit();
            }
                return Result.Succeeded;
        }
    }
}
