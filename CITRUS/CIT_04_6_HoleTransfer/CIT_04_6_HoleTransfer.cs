using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_6_HoleTransfer
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_6_HoleTransfer : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Список типов для поиска семейства отверстия
            List<FamilySymbol> familySymbolList = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Where(f => f.Name == "231_Проем прямоуг (Окно_Стена)")
                .Cast<FamilySymbol>()
                .ToList();
            if(familySymbolList.Count == 0)
            {
                TaskDialog.Show("Revit", "Семейство \"231_Проем прямоуг (Окно_Стена)\" не найдено");
                return Result.Cancelled;
            }

            //Получение типа 231_Проем прямоуг (Окно_Стена)
            FamilySymbol doorwayFamilySymbol = familySymbolList.First();

            //Если выбрана замена всех проемов

            //Получение всех дверей в проекте
            List<FamilyInstance> doorsList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(d => d.Host != null)
                .Where(d => d.Host.Category.Id.ToString() == "-2000011")
                .Where(d => d.Host.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT) != null)
                .Where(d => d.Host.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger() == 1)
                .ToList();

            List<FamilyInstance> windowsList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(d => d.Host != null)
                .Where(d => d.Host.Category.Id.ToString() == "-2000011")
                .Where(d => d.Host.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT) != null)
                .Where(d => d.Host.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger() == 1)
                .ToList();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("замена проемов");
                //Перенос дверных проемов
                foreach (FamilyInstance door in doorsList)
                {
                    //Примерная высота и ширина двери
                    double windowHeight = door.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble();
                    double furnitureWidth = door.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();

                    //Точка размещения двери
                    LocationPoint doorLocationPoint = door.Location as LocationPoint;
                    XYZ doorPoint = doorLocationPoint.Point;

                    //Получить уровень для размещения проема
                    Level lv = doc.GetElement(door.Host.LevelId) as Level;

                    //Вставка проема
                    FamilyInstance newDoorway = doc.Create.NewFamilyInstance(doorPoint, doorwayFamilySymbol, door.Host, lv, StructuralType.NonStructural);
                    newDoorway.LookupParameter("Рзм.Высота").Set(windowHeight);
                    newDoorway.LookupParameter("Рзм.Ширина").Set(furnitureWidth);
                    doc.Delete(door.Id);
                }

                //Перенос оконных проемов
                foreach (FamilyInstance window in windowsList)
                {
                    //Примерная высота и ширина двери
                    double windowHeight = window.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble();
                    double furnitureWidth = window.Symbol.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM).AsDouble();

                    //Точка размещения двери
                    LocationPoint doorLocationPoint = window.Location as LocationPoint;
                    XYZ doorPoint = doorLocationPoint.Point;

                    //Получить уровень для размещения проема
                    Level lv = doc.GetElement(window.Host.LevelId) as Level;

                    //Вставка проема
                    FamilyInstance newDoorway = doc.Create.NewFamilyInstance(doorPoint, doorwayFamilySymbol, window.Host, lv, StructuralType.NonStructural);
                    newDoorway.LookupParameter("Рзм.Высота").Set(windowHeight);
                    newDoorway.LookupParameter("Рзм.Ширина").Set(furnitureWidth);
                    doc.Delete(window.Id);
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
