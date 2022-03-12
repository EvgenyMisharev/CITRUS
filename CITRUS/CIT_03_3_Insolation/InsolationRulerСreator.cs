using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
    class InsolationRulerСreator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение активного вида
            ElementId viewId = doc.ActiveView.Id;

            //Список исходных лучей
            List<FamilyInstance> initialRayUnsortedList = new FilteredElementCollector(doc, viewId)
                .WhereElementIsNotElementType()
                .OfClass(typeof(FamilyInstance))
                .Where(l => l.Name == "CIT_03_Луч")
                .Cast<FamilyInstance>()
                .ToList();

            //Сортировка лучей по направлению относительно X
            List<FamilyInstance> initialRayList = initialRayUnsortedList.OrderBy(l => (l.HandOrientation).AngleTo(XYZ.BasisX)).ToList(); ;

            //Ось вращения при создании промежуточных лучей
            XYZ a = new XYZ(0, 0, 0);
            XYZ b = new XYZ(a.X, a.Y, a.Z + 10);
            Line axis = Line.CreateBound(a, b);

            for (int i=0; i< initialRayList.Count - 1; i++)
            {
                //Колличество минут между лучами
                int minutes = 0;
                //Время первого луча
                string firstRayTime = initialRayList[i].LookupParameter("Время").AsString();
                Int32.TryParse(firstRayTime.Split(':')[0], out int firstRayTimeHours);
                Int32.TryParse(firstRayTime.Split(':')[1], out int firstRayTimeMinutes);

                //Время второго луча
                string secondRayTime = initialRayList[i+1].LookupParameter("Время").AsString();
                Int32.TryParse(secondRayTime.Split(':')[0], out int secondRayTimeHours);
                Int32.TryParse(secondRayTime.Split(':')[1], out int secondRayTimeMinutes);

                //Вычисление колличества минут между лучами
                if (secondRayTimeMinutes == 0 & firstRayTimeMinutes == 0)
                {
                    minutes += (secondRayTimeHours - firstRayTimeHours) * 60;
                }
                else if (secondRayTimeMinutes == 0 & firstRayTimeMinutes != 0)
                {
                    minutes += 60 - firstRayTimeMinutes;
                }
                else
                {
                    minutes += secondRayTimeMinutes;
                }

                //Угол между первым и вторым лучем
                double rayAngle = initialRayList[i].HandOrientation.AngleTo(initialRayList[i+1].HandOrientation);
                //Шаг угла
                double rayAngleStep = rayAngle / minutes;
                //Угол поворота для последующих лучей
                double rotateAngle = 0;

                //Угловая высота первого луча
                double firstRayHeightAngle = initialRayList[i].LookupParameter("Высота").AsDouble();
                //Угловая высота второго луча
                double secondRayHeightAngle = initialRayList[i+1].LookupParameter("Высота").AsDouble();
                //Разница между угловой высотой
                double rayHeightAngleDelta = secondRayHeightAngle - firstRayHeightAngle;
                //Шаг угловой высоты
                double rayHeightAngleStep = rayHeightAngleDelta / minutes;
                //Угловая высота для промежуточных лучей
                double newRayHeightAngle = firstRayHeightAngle;

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Разбиение по минутам");
                    for (int j = 0; j < minutes-1; j++)
                    {
                        rotateAngle += rayAngleStep;
                        newRayHeightAngle += rayHeightAngleStep;

                        //Создание промежуточных лучей
                        ElementId newRayId = ElementTransformUtils.CopyElement(doc, initialRayList[i].Id, new XYZ(0, 0, 0)).First();
                        ElementTransformUtils.RotateElement(doc, newRayId, axis, - rotateAngle);
                        doc.GetElement(newRayId).LookupParameter("Высота").Set(newRayHeightAngle);
                    }
                    t.Commit();
                }
            }

            //Список лучей для создания Линий модели с учетом угловой высоты
            List<FamilyInstance> rayUnsortedListForRotate = new FilteredElementCollector(doc, viewId)
                .WhereElementIsNotElementType()
                .OfClass(typeof(FamilyInstance))
                .Where(l => l.Name == "CIT_03_Луч")
                .Cast<FamilyInstance>()
                .ToList();

            //Сортировка лучей по направлению относительно X
            List<FamilyInstance> rayListForRotate = rayUnsortedListForRotate.OrderBy(l => (l.HandOrientation).AngleTo(XYZ.BasisX)).ToList();

            ProgressBarForm pbf = null;
            Thread m_Thread = new Thread(() => Application.Run(pbf = new ProgressBarForm(rayListForRotate.Count)));
            m_Thread.IsBackground = true;
            m_Thread.Start();
            int step = 0;
            Thread.Sleep(100);

            if (rayListForRotate.Count == 0)
            {
                pbf.BeginInvoke(new Action(() => { pbf.Close(); }));
            }

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Построение лучей");
                foreach (FamilyInstance fi in rayListForRotate)
                {
                    step += 1;
                    pbf.BeginInvoke(new Action(() => { pbf.progressBar_pb.Value = step; }));

                    //Создание Линий Модели с учетом угловой высоты
                    XYZ p = new XYZ(0, 0, 0) + (20000 / 304.8) * fi.HandOrientation;
                    XYZ q = p + (100000 / 304.8) * fi.HandOrientation;
                    Plane plane = Plane.CreateByThreePoints(p, q, new XYZ(p.X, p.Y, p.Z + (20000 / 304.8)));
                    SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

                    Line rotationAxis = Line.CreateBound(new XYZ(0,0,0), new XYZ(0, 0, 0) + (500/304.8)*fi.FacingOrientation);

                    Line line = Line.CreateBound(p, q);
                    ModelCurve newRayWithoutAngle = doc.Create.NewModelCurve(line, sketchPlane);
                    ElementTransformUtils.RotateElement(doc, newRayWithoutAngle.Id, rotationAxis, - fi.LookupParameter("Высота").AsDouble());
                }
                pbf.BeginInvoke(new Action(() => { pbf.Close(); }));
                t.Commit();
            }
            TaskDialog.Show("Revit", "Обработка завершена!");
            return Result.Succeeded;
        }
    }
}
