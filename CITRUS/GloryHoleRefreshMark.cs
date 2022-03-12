using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class GloryHoleRefreshMark : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Guid adskLevel = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
            Guid assignmentVersion = new Guid("c61dd38f-fc8c-4239-9056-dc5baa3c7304");

            List<Level> levelsList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .OrderBy(lv => lv.Elevation)
                .ToList();

            if (levelsList.Count != 0)
            {
                if (levelsList.First().get_Parameter(adskLevel) == null) 
                {
                    TaskDialog.Show("Revit", "Уровни не содержат параметр \"ADSK_Этаж\" или \"О_Этаж\"!\nВы можете добавить параметр из ФОП шаблонов ADSK или Weandrevit в качестве параметра экземпляра для уровней.");
                    return Result.Cancelled;
                }
            }

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Обновление марок");

                foreach (Level level in levelsList)
                {
                    List<FamilyInstance> intersectionPointWallForMarkRefreshWithVersionList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Windows)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(ip => ip.Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Стена")
                        .Where(ip => ip.LevelId == level.Id)
                        .Where(ip => !string.IsNullOrEmpty(ip.get_Parameter(assignmentVersion).AsString()))
                        .OrderBy(ip => (ip.Location as LocationPoint).Point.X)
                        .OrderByDescending(ip => (ip.Location as LocationPoint).Point.Y)
                        .ToList();

                    List<FamilyInstance> intersectionPointWallForMarkRefreshWithoutVersionList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Windows)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(ip => ip.Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Стена")
                        .Where(ip => ip.LevelId == level.Id)
                        .Where(ip => string.IsNullOrEmpty(ip.get_Parameter(assignmentVersion).AsString()))
                        .OrderBy(ip => (ip.Location as LocationPoint).Point.X)
                        .OrderByDescending(ip => (ip.Location as LocationPoint).Point.Y)
                        .ToList();

                    List<FamilyInstance> intersectionPointFloorForMarkRefreshWithVersionList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Windows)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(ip => ip.Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Плита")
                        .Where(ip => ip.LevelId == level.Id)
                        .Where(ip => !string.IsNullOrEmpty(ip.get_Parameter(assignmentVersion).AsString()))
                        .OrderBy(ip => (ip.Location as LocationPoint).Point.X)
                        .OrderByDescending(ip => (ip.Location as LocationPoint).Point.Y)
                        .ToList();

                    List<FamilyInstance> intersectionPointFloorForMarkRefreshWithoutVersionList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Windows)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(ip => ip.Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Плита")
                        .Where(ip => ip.LevelId == level.Id)
                        .Where(ip => string.IsNullOrEmpty(ip.get_Parameter(assignmentVersion).AsString()))
                        .OrderBy(ip => (ip.Location as LocationPoint).Point.X)
                        .OrderByDescending(ip => (ip.Location as LocationPoint).Point.Y)
                        .ToList();

                    int i = 1;
                    if(intersectionPointWallForMarkRefreshWithVersionList.Count == 0)
                    {
                        foreach (FamilyInstance intersectionPoint in intersectionPointWallForMarkRefreshWithoutVersionList)
                        {
                            intersectionPoint.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)
                                .Set(level.get_Parameter(adskLevel)
                                .AsString() + "c-" + i);
                            i++;
                        }
                    }
                    else
                    {
                        string x = intersectionPointWallForMarkRefreshWithVersionList[intersectionPointWallForMarkRefreshWithVersionList.Count - 1]
                            .get_Parameter(BuiltInParameter.ALL_MODEL_MARK)
                            .AsString()
                            .Split('-')
                            .Last();
                        Int32.TryParse(x, out i);
                        i++;
                        foreach (FamilyInstance intersectionPoint in intersectionPointWallForMarkRefreshWithoutVersionList)
                        {
                            intersectionPoint.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)
                                .Set(level.get_Parameter(adskLevel)
                                .AsString() + "c-" + i);
                            i++;
                        }
                    }


                    i = 1;
                    if(intersectionPointFloorForMarkRefreshWithVersionList.Count == 0)
                    {
                        foreach (FamilyInstance intersectionPoint in intersectionPointFloorForMarkRefreshWithoutVersionList)
                        {
                            intersectionPoint.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)
                                .Set(level.get_Parameter(adskLevel)
                                .AsString() + "п-" + i);
                            i++;
                        }
                    }
                    else
                    {
                        string x = intersectionPointFloorForMarkRefreshWithVersionList[intersectionPointFloorForMarkRefreshWithVersionList.Count - 1]
                            .get_Parameter(BuiltInParameter.ALL_MODEL_MARK)
                            .AsString()
                            .Split('-')
                            .Last();
                        Int32.TryParse(x, out i);
                        i++;
                        foreach (FamilyInstance intersectionPoint in intersectionPointFloorForMarkRefreshWithoutVersionList)
                        {
                            intersectionPoint.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)
                                .Set(level.get_Parameter(adskLevel)
                                .AsString() + "п-" + i);
                            i++;
                        }
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
