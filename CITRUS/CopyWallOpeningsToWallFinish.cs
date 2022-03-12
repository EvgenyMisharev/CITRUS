using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CopyWallOpeningsToWallFinish : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            Wall wall1 = doc.GetElement(sel.PickObject(ObjectType.Element)) as Wall;
            Wall wall2 = doc.GetElement(sel.PickObject(ObjectType.Element)) as Wall;

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Вырезание");

                Options opt = new Options();
                opt.ComputeReferences = true;
                opt.DetailLevel = ViewDetailLevel.Fine;
                GeometryElement geomElem = wall1.get_Geometry(opt);
                foreach (GeometryObject geomObj in geomElem)
                {
                    Solid geomSolid = geomObj as Solid;
                    if (null != geomSolid)
                    {
                        PlanarFace targetFace = null;
                        FaceArray faces = geomSolid.Faces;
                        foreach (PlanarFace planarFace in faces)
                        {
                            if (wall2.Orientation.IsAlmostEqualTo(planarFace.FaceNormal))
                            {
                                targetFace = planarFace;
                                break;
                            }
                        }
                        XYZ p1 = null;
                        XYZ p2 = null;
                        var loops = targetFace.GetEdgesAsCurveLoops();

                        doc.Create.NewOpening(wall2, p1, p2);
                    }
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
