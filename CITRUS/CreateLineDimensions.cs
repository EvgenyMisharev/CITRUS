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
    class CreateLineDimensions : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;

            XYZ pt1 = uiDoc.Selection.PickPoint();
            XYZ pt2 = uiDoc.Selection.PickPoint();
            Line tempLine = Line.CreateBound(pt1, pt2);
            Line line = null;
            Line lineForGridIntersectionCheck = null;

            if (tempLine.Direction.AngleTo(XYZ.BasisX) * 180 / Math.PI >= -45 
                && tempLine.Direction.AngleTo(XYZ.BasisX) * 180 / Math.PI <= 45 ||
                tempLine.Direction.AngleTo(XYZ.BasisX.Negate()) * 180 / Math.PI >= -45
                && tempLine.Direction.AngleTo(XYZ.BasisX.Negate()) * 180 / Math.PI <= 45)
            {
                line = Line.CreateBound(pt1, new XYZ(pt2.X, pt1.Y, pt2.Z));
                lineForGridIntersectionCheck = Line.CreateBound(new XYZ(pt1.X, pt1.Y, 0), new XYZ(pt2.X, pt1.Y, 0));
            }
            else
            {
                line = Line.CreateBound(pt1, new XYZ(pt1.X, pt2.Y, pt2.Z));
                lineForGridIntersectionCheck = Line.CreateBound(new XYZ(pt1.X, pt1.Y, 0), new XYZ(pt1.X, pt2.Y, 0));
            }

            ReferenceArray references = new ReferenceArray();
            List<Grid> gridsList = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_Grids)
                .Cast<Grid>()
                .ToList();

            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.IncludeNonVisibleObjects = false;
            opt.View = doc.ActiveView;

            foreach (Grid grid in gridsList)
            {
                foreach (GeometryObject obj in grid.get_Geometry(opt))
                {
                    SetComparisonResult intersectionResult = grid.Curve.Intersect(lineForGridIntersectionCheck);
                    if (intersectionResult == SetComparisonResult.Overlap)
                    {
                        if (obj is Line)
                        {
                            Line gridLine = obj as Line;
                            references.Append(gridLine.Reference);
                            break;
                        }
                    }
                }
            }

            //List<FamilyInstance> columnsList = new FilteredElementCollector(doc, doc.ActiveView.Id)
            //   .OfCategory(BuiltInCategory.OST_StructuralColumns)
            //   .OfClass(typeof(FamilyInstance))
            //   .WhereElementIsNotElementType()
            //   .Cast<FamilyInstance>()
            //   .ToList();

            //foreach (FamilyInstance column in columnsList)
            //{
            //    GeometryElement geomElem = column.get_Geometry(opt);
            //    foreach (GeometryObject geoObject in geomElem)
            //    {
            //        GeometryInstance instance = geoObject as GeometryInstance;
            //        if (null != instance)
            //        {
            //            foreach (GeometryObject instObj in instance.SymbolGeometry)
            //            {
            //                Solid solid = instObj as Solid;
            //                if (null == solid || 0 == solid.Faces.Size || 0 == solid.Edges.Size)
            //                {
            //                    continue;
            //                }
            //                Transform instTransform = instance.Transform;
            //                foreach (Face face in solid.Faces)
            //                {
            //                    UV uV = new UV(0.5, 0.5);
            //                    if (face.ComputeNormal(uV).IsAlmostEqualTo(line.Direction) || face.ComputeNormal(uV).IsAlmostEqualTo(line.Direction.Negate()))
            //                    {
            //                        references.Append(face.Reference);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            List<Wall> wallsList = new FilteredElementCollector(doc, doc.ActiveView.Id)
               .OfCategory(BuiltInCategory.OST_Walls)
               .OfClass(typeof(Wall))
               .WhereElementIsNotElementType()
               .Cast<Wall>()
               .ToList();
            foreach (Wall wall in wallsList)
            {
                GeometryElement geomElem = wall.get_Geometry(opt);
                foreach (GeometryObject geoObject in geomElem)
                {
                    Solid solid = geoObject as Solid;
                    if (null == solid || 0 == solid.Faces.Size || 0 == solid.Edges.Size)
                    {
                        continue;
                    }
                    foreach (PlanarFace face in solid.Faces)
                    {
                        if (face.FaceNormal.IsAlmostEqualTo(line.Direction) || face.FaceNormal.IsAlmostEqualTo(line.Direction.Negate()))
                        {
                            SetComparisonResult intersectionResult = face.Intersect(line);
                            if (intersectionResult == SetComparisonResult.Overlap)
                            {
                                references.Append(face.Reference);
                            }
                            
                        }
                    }
                }
            }

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размер по линии");
                Dimension dimension = doc.Create.NewDimension(doc.ActiveView, line, references);
                DimensionSegmentArray dimensionSegments = dimension.Segments;
                bool status = false;
                foreach (DimensionSegment ds in dimensionSegments)
                {
                    if (ds.Value < 500 / 304.8)
                    {

                        if (dimensionSegments.get_Item(0) == ds && line.Direction.IsAlmostEqualTo(XYZ.BasisX))
                        {
                            ds.TextPosition = new XYZ(ds.TextPosition.X - 500 / 304.8, ds.TextPosition.Y, 0);
                        }
                        else if (dimensionSegments.get_Item(0) == ds && line.Direction.IsAlmostEqualTo(XYZ.BasisX.Negate()))
                        {
                            ds.TextPosition = new XYZ(ds.TextPosition.X + 500 / 304.8, ds.TextPosition.Y, 0);
                        }
                        else if(dimensionSegments.get_Item(dimensionSegments.Size - 1) == ds && line.Direction.IsAlmostEqualTo(XYZ.BasisX))
                        {
                            ds.TextPosition = new XYZ(ds.TextPosition.X + 500 / 304.8, ds.TextPosition.Y, 0);
                        }
                        else if (dimensionSegments.get_Item(dimensionSegments.Size - 1) == ds && line.Direction.IsAlmostEqualTo(XYZ.BasisX.Negate()))
                        {
                            ds.TextPosition = new XYZ(ds.TextPosition.X - 500 / 304.8, ds.TextPosition.Y, 0);
                        }
                        else if (dimensionSegments.get_Item(0) == ds && line.Direction.IsAlmostEqualTo(XYZ.BasisY))
                        {
                            ds.TextPosition = new XYZ(ds.TextPosition.X , ds.TextPosition.Y - 500 / 304.8, 0);
                        }
                        else if (dimensionSegments.get_Item(0) == ds && line.Direction.IsAlmostEqualTo(XYZ.BasisY.Negate()))
                        {
                            ds.TextPosition = new XYZ(ds.TextPosition.X, ds.TextPosition.Y + 500 / 304.8, 0);
                        }
                        else if (dimensionSegments.get_Item(dimensionSegments.Size - 1) == ds && line.Direction.IsAlmostEqualTo(XYZ.BasisY))
                        {
                            ds.TextPosition = new XYZ(ds.TextPosition.X, ds.TextPosition.Y + 500 / 304.8, 0);
                        }
                        else if (dimensionSegments.get_Item(dimensionSegments.Size - 1) == ds && line.Direction.IsAlmostEqualTo(XYZ.BasisY.Negate()))
                        {
                            ds.TextPosition = new XYZ(ds.TextPosition.X, ds.TextPosition.Y - 500 / 304.8, 0);
                        }
                        else
                        {
                            if (!status)
                            {
                                ds.TextPosition = new XYZ(ds.TextPosition.X - 500 / 304.8, ds.TextPosition.Y - 500 / 304.8, 0);
                                status = true;
                            }
                            else
                            {
                                ds.TextPosition = new XYZ(ds.TextPosition.X + 500 / 304.8, ds.TextPosition.Y - 500 / 304.8, 0);
                                status = false;
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
