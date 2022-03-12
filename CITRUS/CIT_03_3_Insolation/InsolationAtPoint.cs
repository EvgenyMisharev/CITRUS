using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_03_3_Insolation
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class InsolationAtPoint : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Доступ к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            //Вызов формы
            InsolationAtPointForm insolationAtPointForm = new InsolationAtPointForm();
            insolationAtPointForm.ShowDialog();
            if (insolationAtPointForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            string verificationOption = insolationAtPointForm.VerificationOption;
            bool checkSelectedPanels = insolationAtPointForm.CheckSelectedPanels;
            bool checkSelectedPoints = insolationAtPointForm.CheckSelectedPoints;
            bool wallsAndFloorsGeometry = insolationAtPointForm.WallsAndFloorsGeometry;

            //Получение связанного файла "Инсоляционная линейка"
            IEnumerable<RevitLinkInstance> revitLinkInstanceInsolationRuler = new FilteredElementCollector(doc)
                        .OfClass(typeof(RevitLinkInstance))
                        .Where(li => li.Name.Contains("Инсоляционная линейка"))
                        .Cast<RevitLinkInstance>();

            if (revitLinkInstanceInsolationRuler.Count() == 0)
            {
                TaskDialog.Show("Revit", "Инсоляционная линейка не найдена!");
                return Result.Cancelled;
            }
            Document revitLinkInsolationRuler = revitLinkInstanceInsolationRuler.First().GetLinkDocument();

            //Получение лучей из файла "Инсоляционная линейка"
            List<ModelCurve> rayList = new FilteredElementCollector(revitLinkInsolationRuler)
                .OfCategory(BuiltInCategory.OST_Lines)
                .Cast<ModelCurve>()
                .ToList();
            if (rayList.Count() == 0)
            {
                TaskDialog.Show("Revit", "Инсоляционная линейка не содержит лучей");
                return Result.Cancelled;
            }

            //Получение векторов из лучей
            List<XYZ> rayVectorsUnsortedList = new List<XYZ>();
            foreach (ModelCurve mc in rayList)
            {
                rayVectorsUnsortedList.Add(((mc.Location as LocationCurve).Curve as Line).Direction.Normalize());
            }
            List<XYZ> rayVectorsList = rayVectorsUnsortedList.OrderBy(v => v.AngleTo(XYZ.BasisX)).ToList();

            //Получение списка твёрдых тел из Форм для проверки пересечений
            List<FamilyInstance> massList = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Mass)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            List<Solid> solidList = new List<Solid>();
            foreach (FamilyInstance mass in massList)
            {
                GeometryElement geomElement = mass.get_Geometry(new Options());
                Solid solid = null;
                foreach (GeometryObject geomObj in geomElement)
                {
                    solid = geomObj as Solid;
                    if (solid != null) break;
                }
                solidList.Add(solid);
            }
            if (verificationOption == "radioButton_PointInsolation" && wallsAndFloorsGeometry)
            {
                //Получение списка связанных файлов без "Инсоляционной линейки"
                List<RevitLinkInstance> revitLinkInstance = new FilteredElementCollector(doc)
                    .OfClass(typeof(RevitLinkInstance))
                    .Where(li => !li.Name.Contains("Инсоляционная линейка"))
                    .Cast<RevitLinkInstance>()
                    .Where(li => li.GetLinkDocument() != null)
                    .ToList();

                if (revitLinkInstance.Count() == 0)
                {
                    TaskDialog.Show("Revit", "Связанные файлы не найдены");
                }

                //Получение solid объектов из стен и перекрытий связанных файлов
                foreach (RevitLinkInstance rl in revitLinkInstance)
                {
                    Transform transform = rl.GetTotalTransform();

                    Document docWallFloor = rl.GetLinkDocument();
                    //Получение солидов перекрытий из связанного файла
                    List<Floor> floorList = new FilteredElementCollector(docWallFloor)
                        .WhereElementIsNotElementType()
                        .OfCategory(BuiltInCategory.OST_Floors)
                        .OfClass(typeof(Floor))
                        .Cast<Floor>()
                        .ToList();
                    foreach (Floor floor in floorList)
                    {
                        GeometryElement geomElement = floor.get_Geometry(new Options());
                        Solid solid = null;
                        foreach (GeometryObject geomObj in geomElement)
                        {
                            solid = geomObj as Solid;
                            if (solid != null) break;
                        }
                        Solid transformSolid = SolidUtils.CreateTransformed(solid, transform);
                        solidList.Add(transformSolid);
                    }
                    //Получение солидов стен из связанного файла
                    List<Wall> wallList = new FilteredElementCollector(docWallFloor)
                        .WhereElementIsNotElementType()
                        .OfCategory(BuiltInCategory.OST_Walls)
                        .OfClass(typeof(Wall))
                        .Cast<Wall>()
                        .Where(w => w.CurtainGrid == null)
                        .ToList();
                    foreach (Wall wall in wallList)
                    {
                        GeometryElement geomElement = wall.get_Geometry(new Options());
                        Solid solid = null;
                        foreach (GeometryObject geomObj in geomElement)
                        {
                            solid = geomObj as Solid;
                            if (solid != null) break;
                        }
                        Solid transformSolid = SolidUtils.CreateTransformed(solid, transform);
                        solidList.Add(transformSolid);
                    }
                }
            }

            if (verificationOption == "radioButton_PanelInsolation")
            {
                List<Autodesk.Revit.DB.Panel> panelList = new List<Autodesk.Revit.DB.Panel>();
                if(checkSelectedPanels)
                {
                    PanelSelectionFilter panelSelFilter = new PanelSelectionFilter();
                    IList<Reference> selPanelsRefList = sel.PickObjects(ObjectType.Element, panelSelFilter, "Выберите панели!");

                    foreach (Reference selPanelRef in selPanelsRefList)
                    {
                        panelList.Add(doc.GetElement(selPanelRef) as Autodesk.Revit.DB.Panel);
                    }
                }
                else
                {
                    panelList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_CurtainWallPanels)
                        .WhereElementIsNotElementType()
                        .Cast<Autodesk.Revit.DB.Panel>()
                        .ToList();
                }

                ProgressBarForm pbf = null;
                Thread m_Thread = new Thread(() => Application.Run(pbf = new ProgressBarForm(panelList.Count)));
                m_Thread.IsBackground = true;
                m_Thread.Start();
                int step = 0;
                Thread.Sleep(100);

                if (panelList.Count == 0)
                {
                    pbf.BeginInvoke(new Action(() => { pbf.Close(); }));
                }

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Расчет времени инсоляции панелей");
                    SolidCurveIntersectionOptions intersectOptions = new SolidCurveIntersectionOptions();
                    foreach (Autodesk.Revit.DB.Panel p in panelList)
                    {
                        step += 1;
                        pbf.BeginInvoke(new Action(() => { pbf.progressBar_pb.Value = step; }));

                        Solid panelSolid = null;
                        GeometryElement geomElement = p.get_Geometry(new Options());
                        foreach (GeometryObject geomObj in geomElement)
                        {
                            GeometryInstance geomInstance = geomObj as GeometryInstance;
                            if (null != geomInstance)
                            {
                                GeometryElement instanceGeometryElement = geomInstance.GetInstanceGeometry();
                                foreach (GeometryObject instObj in instanceGeometryElement)
                                {
                                    panelSolid = instObj as Solid;
                                    if (panelSolid != null) break;
                                }
                            }
                        }

                        int numberOfNonintersectingRays = 0;
                        int continuousInsolation = 0;
                        List<int> continuousInsolationList = new List<int>();

                        XYZ insolationPointLocation = panelSolid.ComputeCentroid();
                        List<Curve> rayCurveUnsortedListFromPanelCentralPoints = new List<Curve>();
                        foreach (XYZ v in rayVectorsList)
                        {
                            Curve curve = Line.CreateBound(insolationPointLocation, insolationPointLocation + (1000000 / 304.8) * v) as Curve;
                            rayCurveUnsortedListFromPanelCentralPoints.Add(curve);
                        }

                        List<Curve> rayCurveListFromPanelCentralPoints = rayCurveUnsortedListFromPanelCentralPoints.OrderBy(c => (c as Line).Direction.AngleTo(XYZ.BasisX)).ToList();

                        for (int i = 0; i < rayCurveListFromPanelCentralPoints.Count; i++)
                        {
                            int intersectionCount = 0;
                            foreach (Solid solid in solidList)
                            {
                                SolidCurveIntersection intersection = solid.IntersectWithCurve(rayCurveListFromPanelCentralPoints[i], intersectOptions);
                                var sc = intersection.SegmentCount;
                                if (sc > 0)
                                {
                                    intersectionCount += 1;
                                }
                            }
                            if (intersectionCount == 0 & i != rayCurveListFromPanelCentralPoints.Count - 1)
                            {
                                numberOfNonintersectingRays += 1;
                                continuousInsolation += 1;
                            }
                            else if (intersectionCount == 0 & i == rayCurveListFromPanelCentralPoints.Count - 1)
                            {
                                numberOfNonintersectingRays += 1;
                                continuousInsolation += 1;
                                continuousInsolationList.Add(continuousInsolation);
                            }

                            else if (intersectionCount != 0 & i != rayCurveListFromPanelCentralPoints.Count - 1)
                            {
                                continuousInsolationList.Add(continuousInsolation);
                                continuousInsolation = 0;
                            }
                            else
                            {
                                continuousInsolationList.Add(continuousInsolation);
                            }
                        }

                        p.LookupParameter("Инсоляция (мин.)").Set(numberOfNonintersectingRays);
                        if (continuousInsolationList.Count != 0)
                        {
                            p.LookupParameter("Непрерывная инсоляция (мин.)").Set(continuousInsolationList.Max());
                            p.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Прерывистая");
                        }
                        else
                        {
                            p.LookupParameter("Непрерывная инсоляция (мин.)").Set(numberOfNonintersectingRays);
                            p.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Непрерывная");
                        }

                    }
                    pbf.BeginInvoke(new Action(() => { pbf.Close(); }));
                    t.Commit();
                }
            }
            else if (verificationOption == "radioButton_PointInsolation")
            {
                // Получение списка точек для проверки инсоляции
                List<FamilyInstance> insolationPointsList = new List<FamilyInstance>();
                if (checkSelectedPoints == false)
                {
                    insolationPointsList = new FilteredElementCollector(doc)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(fi => fi.Name == "InsolationPoint")
                        .ToList();
                    if (insolationPointsList.Count == 0)
                    {
                        TaskDialog.Show("Revit", "Семейство \"InsolationPoint\" не размещено в проекте");
                        return Result.Cancelled;
                    }
                }

                else
                {
                    FamilyInstanceSelectionFilter selFilter = new FamilyInstanceSelectionFilter();
                    IList<Reference> selFamilyInstances = sel.PickObjects(ObjectType.Element, selFilter, "Выберите точки!");

                    foreach (Reference genericModelsRef in selFamilyInstances)
                    {
                        if ((doc.GetElement(genericModelsRef) as FamilyInstance).Name == "InsolationPoint")
                        {
                            insolationPointsList.Add(doc.GetElement(genericModelsRef) as FamilyInstance);
                        }
                    }
                }

                ProgressBarForm pbf = null;
                Thread m_Thread = new Thread(() => Application.Run(pbf = new ProgressBarForm(insolationPointsList.Count)));
                m_Thread.IsBackground = true;
                m_Thread.Start();
                int step = 0;
                Thread.Sleep(100);

                if (insolationPointsList.Count == 0)
                {
                    pbf.BeginInvoke(new Action(() => { pbf.Close(); }));
                }


                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Расчет времени инсоляции точек");
                    SolidCurveIntersectionOptions intersectOptions = new SolidCurveIntersectionOptions();
                    foreach (FamilyInstance p in insolationPointsList)
                    {
                        step += 1;
                        pbf.BeginInvoke(new Action(() => { pbf.progressBar_pb.Value = step; }));

                        int numberOfNonintersectingRays = 0;
                        int continuousInsolation = 0;
                        List<int> continuousInsolationList = new List<int>();

                        XYZ insolationPointLocation = (p.Location as LocationPoint).Point;
                        List<Curve> rayCurveUnsortedListFromInsolationPoint = new List<Curve>();
                        foreach (XYZ v in rayVectorsList)
                        {
                            Curve curve = Line.CreateBound(insolationPointLocation, insolationPointLocation + (1000000 / 304.8) * v) as Curve;
                            rayCurveUnsortedListFromInsolationPoint.Add(curve);
                        }

                        List<Curve> rayCurveListFromInsolationPoint = rayCurveUnsortedListFromInsolationPoint.OrderBy(c => (c as Line).Direction.AngleTo(XYZ.BasisX)).ToList();

                        for (int i = 0; i < rayCurveListFromInsolationPoint.Count; i++)
                        {
                            int intersectionCount = 0;
                            foreach (Solid solid in solidList)
                            {
                                SolidCurveIntersection intersection = solid.IntersectWithCurve(rayCurveListFromInsolationPoint[i], intersectOptions);
                                var sc = intersection.SegmentCount;
                                if (sc > 0)
                                {
                                    intersectionCount += 1;
                                }
                            }
                            if (intersectionCount == 0 & i != rayCurveListFromInsolationPoint.Count - 1)
                            {
                                numberOfNonintersectingRays += 1;
                                continuousInsolation += 1;
                            }
                            else if (intersectionCount == 0 & i == rayCurveListFromInsolationPoint.Count - 1)
                            {
                                numberOfNonintersectingRays += 1;
                                continuousInsolation += 1;
                                continuousInsolationList.Add(continuousInsolation);
                            }

                            else if (intersectionCount != 0 & i != rayCurveListFromInsolationPoint.Count - 1)
                            {
                                continuousInsolationList.Add(continuousInsolation);
                                continuousInsolation = 0;
                            }
                            else
                            {
                                continuousInsolationList.Add(continuousInsolation);
                            }
                        }

                        p.LookupParameter("Инсоляция (мин.)").Set(numberOfNonintersectingRays);
                        if (continuousInsolationList.Count != 0)
                        {
                            p.LookupParameter("Непрерывная инсоляция (мин.)").Set(continuousInsolationList.Max());
                            p.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Прерывистая");
                        }
                        else
                        {
                            p.LookupParameter("Непрерывная инсоляция (мин.)").Set(numberOfNonintersectingRays);
                            p.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Непрерывная");
                        }
                    }
                    pbf.BeginInvoke(new Action(() => { pbf.Close(); }));
                    t.Commit();
                }
            }
            else
            {
                TaskDialog.Show("Revit", "Что-то пошло не так!");
                return Result.Cancelled;
            }

            TaskDialog.Show("Revit", "Обработка завершена успешно!");
            return Result.Succeeded;
        }
    }
}
