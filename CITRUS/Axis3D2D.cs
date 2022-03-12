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
    class Axis3D2D : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Активный вид
            View view = doc.ActiveView;
            //Список осей на активном виде
            List<Grid> gridList = new FilteredElementCollector(doc, view.Id).OfCategory(BuiltInCategory.OST_Grids).Cast<Grid>().ToList();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Преобразование осей в 2D");
                foreach (Grid g in gridList)
                {
                    g.SetDatumExtentType(DatumEnds.End0, view, DatumExtentType.ViewSpecific);
                    g.SetDatumExtentType(DatumEnds.End1, view, DatumExtentType.ViewSpecific);
                }
                t.Commit();
            }

                return Result.Succeeded;
        }
    }
}
