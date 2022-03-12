using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_00_1_FillingParameterLevel
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_00_1_FillingParameterLevel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Level> levelsList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .ToList();

            //Несортированные списки уровней выше и ниже ноля
            List<Level> unsortedLevelsListAboveZero = new List<Level>();
            List<Level> unsortedLevelsListBelowZero = new List<Level>();

            foreach(Level lv in levelsList)
            {
                if (Math.Round(lv.Elevation, 6) >= 0)
                {
                    unsortedLevelsListAboveZero.Add(lv);
                }
                else
                {
                    unsortedLevelsListBelowZero.Add(lv);
                }
            }

            //Сортированные списки уровней выше и ниже ноля
            List<Level> sortedLevelsListAboveZero = unsortedLevelsListAboveZero.OrderBy(lv => Math.Round(lv.Elevation, 6)).ToList();
            List<Level> sortedLevelsListBelowZero = unsortedLevelsListBelowZero.OrderByDescending(lv => Math.Round(lv.Elevation, 6)).ToList();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Заполнение параметра \"О_Этаж\"");

                int sortedLevelsListAboveZeroCnt = 1;
                CIT_00_1_FillingParameterLevelProgressBarForm pbf = null;
                Thread m_Thread = new Thread(() => Application.Run(pbf = new CIT_00_1_FillingParameterLevelProgressBarForm(sortedLevelsListAboveZero.Count)));
                m_Thread.IsBackground = true;
                m_Thread.Start();
                int step = 0;
                Thread.Sleep(100);

                //Обработка элементов выше ноля
                foreach (Level lv in sortedLevelsListAboveZero)
                {
                    step += 1;
                    pbf.BeginInvoke(new Action(() => { pbf.label_LevelName.Text = lv.Name; }));
                    pbf.BeginInvoke(new Action(() => { pbf.progressBar_pb.Value = step; }));

                    List<Floor> floorsList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Floors)
                        .OfClass(typeof(Floor))
                        .WhereElementIsNotElementType()
                        .Cast<Floor>()
                        .Where(fl => fl.get_Parameter(BuiltInParameter.LEVEL_PARAM) != null)
                        .Where(fl => fl.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsElementId() == lv.Id)
                        .ToList();
                    foreach (Floor fl in floorsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        fl.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListAboveZeroCnt.ToString());
                    }

                    List<Wall> wallsList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Walls)
                        .OfClass(typeof(Wall))
                        .WhereElementIsNotElementType()
                        .Cast<Wall>()
                        .Where(wl => wl.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT) != null)
                        .Where(wl => wl.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId() == lv.Id)
                        .ToList();
                    foreach (Wall wl in wallsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        if (wl.get_Parameter(paramGUID).IsReadOnly == false)
                        {
                            wl.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListAboveZeroCnt.ToString());
                        }
                        Parameter floorParam = wl.LookupParameter(sortedLevelsListAboveZeroCnt.ToString());
                        if (floorParam != null & wl.CurtainGrid != null)
                        {
                            if(floorParam.IsReadOnly == false)
                            {
                                floorParam.Set(10.764);
                            }
                        }
                        for (int i = -10; i <= 50; i++)
                        {
                            if (i != sortedLevelsListAboveZeroCnt)
                            {
                                Parameter floorParamForNull = wl.LookupParameter(i.ToString());
                                if (floorParamForNull != null & wl.CurtainGrid != null)
                                {
                                    if (floorParamForNull.IsReadOnly == false)
                                    {
                                        floorParamForNull.Set(0);
                                    }
                                }
                            }
                        }
                    }

                    List<FamilyInstance> colunsList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralColumns)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(cl => cl.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM) != null)
                        .Where(cl => cl.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId() == lv.Id)
                        .ToList();
                    foreach (FamilyInstance cl in colunsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        cl.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListAboveZeroCnt.ToString());
                    }

                    List<FamilyInstance> beamsList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralFraming)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(bm => bm.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM) != null)
                        .Where(bm => bm.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId() == lv.Id)
                        .ToList();
                    foreach (FamilyInstance bm in beamsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        bm.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListAboveZeroCnt.ToString());
                    }

                    List<Floor> floorFoundationList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                        .OfClass(typeof(Floor))
                        .WhereElementIsNotElementType()
                        .Cast<Floor>()
                        .Where(pfd => pfd.get_Parameter(BuiltInParameter.LEVEL_PARAM) != null)
                        .Where(pfd => pfd.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsElementId() == lv.Id)
                        .ToList();
                    foreach (Floor pfd in floorFoundationList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        pfd.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListAboveZeroCnt.ToString());
                    }

                    List<FamilyInstance> foundationList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(fd => fd.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null)
                        .Where(fd => fd.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId() == lv.Id)
                        .ToList();
                    foreach (FamilyInstance fd in foundationList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        fd.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListAboveZeroCnt.ToString());
                    }

                    //Архитектурные элементы
                    //Окна
                    List<FamilyInstance> windowsList = new FilteredElementCollector(doc)
                       .OfCategory(BuiltInCategory.OST_Windows)
                       .OfClass(typeof(FamilyInstance))
                       .WhereElementIsNotElementType()
                       .Cast<FamilyInstance>()
                       .Where(w => w.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null)
                       .Where(w => w.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId() == lv.Id)
                       .ToList();
                    foreach (FamilyInstance w in windowsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        w.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListAboveZeroCnt.ToString());
                        Parameter floorParam = w.LookupParameter(sortedLevelsListAboveZeroCnt.ToString());
                        if (floorParam != null)
                        {
                            floorParam.Set(10.764);
                        }
                        for (int i = -10; i <= 50; i++)
                        {
                            if (i != sortedLevelsListAboveZeroCnt)
                            {
                                Parameter floorParamForNull = w.LookupParameter(i.ToString());
                                if (floorParamForNull != null)
                                {
                                    if (floorParamForNull.IsReadOnly == false)
                                    {
                                        floorParamForNull.Set(0);
                                    }
                                }
                            }
                        }
                    }
                    //Двери
                    List<FamilyInstance> doorsList = new FilteredElementCollector(doc)
                       .OfCategory(BuiltInCategory.OST_Doors)
                       .OfClass(typeof(FamilyInstance))
                       .WhereElementIsNotElementType()
                       .Cast<FamilyInstance>()
                       .Where(d => d.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null)
                       .Where(d => d.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId() == lv.Id)
                       .ToList();
                    foreach (FamilyInstance d in doorsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        d.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListAboveZeroCnt.ToString());
                        Parameter floorParam = d.LookupParameter(sortedLevelsListAboveZeroCnt.ToString());
                        if (floorParam != null)
                        {
                            floorParam.Set(10.764);
                        }
                        for (int i = -10; i <= 50; i++)
                        {
                            if (i != sortedLevelsListAboveZeroCnt)
                            {
                                Parameter floorParamForNull = d.LookupParameter(i.ToString());
                                if (floorParamForNull != null)
                                {
                                    if (floorParamForNull.IsReadOnly == false)
                                    {
                                        floorParamForNull.Set(0);
                                    }
                                }
                            }
                        }
                    }
                    //Ограждения
                    List<Railing> railingsList = new FilteredElementCollector(doc)
                       .OfCategory(BuiltInCategory.OST_StairsRailing)
                       .OfClass(typeof(Railing))
                       .WhereElementIsNotElementType()
                       .Cast<Railing>()
                       .Where(r => r.get_Parameter(BuiltInParameter.STAIRS_RAILING_BASE_LEVEL_PARAM) != null)
                       .Where(r => r.get_Parameter(BuiltInParameter.STAIRS_RAILING_BASE_LEVEL_PARAM).AsElementId() == lv.Id)
                       .ToList();
                    foreach (Railing r in railingsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        r.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListAboveZeroCnt.ToString());
                    }
                    //Перемычки. Обобщенные модели
                    List<FamilyInstance> breastplatesList = new FilteredElementCollector(doc)
                       .OfCategory(BuiltInCategory.OST_GenericModel)
                       .OfClass(typeof(FamilyInstance))
                       .WhereElementIsNotElementType()
                       .Cast<FamilyInstance>()
                       .Where(bp => bp.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null)
                       .Where(bp => bp.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId() == lv.Id)
                       .ToList();
                    foreach (FamilyInstance bp in breastplatesList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        bp.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListAboveZeroCnt.ToString());
                        Parameter floorParam = bp.LookupParameter(sortedLevelsListAboveZeroCnt.ToString());
                        if (floorParam != null)
                        {
                            floorParam.Set(10.764);
                        }
                        for (int i = -10; i <= 50; i++)
                        {
                            if (i != sortedLevelsListAboveZeroCnt)
                            {
                                Parameter floorParamForNull = bp.LookupParameter(i.ToString());
                                if (floorParamForNull != null)
                                {
                                    if (floorParamForNull.IsReadOnly == false)
                                    {
                                        floorParamForNull.Set(0);
                                    }
                                }
                            }
                        }
                    }

                    sortedLevelsListAboveZeroCnt++;
                }
                pbf.BeginInvoke(new Action(() => { pbf.Close(); }));

                m_Thread = new Thread(() => Application.Run(pbf = new CIT_00_1_FillingParameterLevelProgressBarForm(sortedLevelsListBelowZero.Count)));
                m_Thread.IsBackground = true;
                m_Thread.Start();
                step = 0;
                Thread.Sleep(100);

                int sortedLevelsListBelowZeroCnt = -1;
                //Обработка элементов ниже ноля
                foreach (Level lv in sortedLevelsListBelowZero)
                {
                    step += 1;
                    pbf.BeginInvoke(new Action(() => { pbf.label_LevelName.Text = lv.Name; }));
                    pbf.BeginInvoke(new Action(() => { pbf.progressBar_pb.Value = step; }));

                    List<Floor> floorsList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Floors)
                        .OfClass(typeof(Floor))
                        .WhereElementIsNotElementType()
                        .Cast<Floor>()
                        .Where(fl => fl.get_Parameter(BuiltInParameter.LEVEL_PARAM) != null)
                        .Where(fl => fl.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsElementId() == lv.Id)
                        .ToList();
                    foreach (Floor fl in floorsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        fl.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListBelowZeroCnt.ToString());
                    }

                    List<Wall> wallsList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Walls)
                        .OfClass(typeof(Wall))
                        .WhereElementIsNotElementType()
                        .Cast<Wall>()
                        .Where(wl => wl.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT) != null)
                        .Where(wl => wl.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId() == lv.Id)
                        .ToList();
                    foreach (Wall wl in wallsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        if (wl.get_Parameter(paramGUID).IsReadOnly == false)
                        {
                            wl.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListBelowZeroCnt.ToString());
                        }
                        Parameter floorParam = wl.LookupParameter(sortedLevelsListBelowZeroCnt.ToString());
                        if (floorParam != null & wl.CurtainGrid != null)
                        {
                            if (floorParam.IsReadOnly == false)
                            {
                                floorParam.Set(10.764);
                            }
                        }
                        for (int i = -10; i <= 50; i++)
                        {
                            if (i != sortedLevelsListBelowZeroCnt)
                            {
                                Parameter floorParamForNull = wl.LookupParameter(i.ToString());
                                if (floorParamForNull != null & wl.CurtainGrid != null)
                                {
                                    if (floorParamForNull.IsReadOnly == false)
                                    {
                                        floorParamForNull.Set(0);
                                    }
                                }
                            }
                        }
                    }

                    List<FamilyInstance> colunsList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralColumns)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(cl => cl.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM) != null)
                        .Where(cl => cl.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId() == lv.Id)
                        .ToList();
                    foreach (FamilyInstance cl in colunsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        cl.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListBelowZeroCnt.ToString());
                    }

                    List<FamilyInstance> beamsList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralFraming)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(bm => bm.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM) != null)
                        .Where(bm => bm.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId() == lv.Id)
                        .ToList();
                    foreach (FamilyInstance bm in beamsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        bm.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListBelowZeroCnt.ToString());
                    }

                    List<Floor> floorFoundationList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                        .OfClass(typeof(Floor))
                        .WhereElementIsNotElementType()
                        .Cast<Floor>()
                        .Where(pfd => pfd.get_Parameter(BuiltInParameter.LEVEL_PARAM) != null)
                        .Where(pfd => pfd.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsElementId() == lv.Id)
                        .ToList();
                    foreach (Floor pfd in floorFoundationList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        pfd.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListBelowZeroCnt.ToString());
                    }

                    List<FamilyInstance> foundationList = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                        .OfClass(typeof(FamilyInstance))
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(fd => fd.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null)
                        .Where(fd => fd.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId() == lv.Id)
                        .ToList();
                    foreach (FamilyInstance fd in foundationList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        fd.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListBelowZeroCnt.ToString());
                    }
                    //Архитектурные элементы
                    //Окна
                    List<FamilyInstance> windowsList = new FilteredElementCollector(doc)
                       .OfCategory(BuiltInCategory.OST_Windows)
                       .OfClass(typeof(FamilyInstance))
                       .WhereElementIsNotElementType()
                       .Cast<FamilyInstance>()
                       .Where(w => w.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null)
                       .Where(w => w.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId() == lv.Id)
                       .ToList();
                    foreach (FamilyInstance w in windowsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        w.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListBelowZeroCnt.ToString());
                        Parameter floorParam = w.LookupParameter(sortedLevelsListBelowZeroCnt.ToString());
                        if (floorParam != null)
                        {
                            floorParam.Set(10.764);
                        }
                        for (int i = -10; i <= 50; i++)
                        {
                            if (i != sortedLevelsListBelowZeroCnt)
                            {
                                Parameter floorParamForNull = w.LookupParameter(i.ToString());
                                if (floorParamForNull != null)
                                {
                                    if (floorParamForNull.IsReadOnly == false)
                                    {
                                        floorParamForNull.Set(0);
                                    }
                                }
                            }
                        }
                    }
                    //Двери
                    List<FamilyInstance> doorsList = new FilteredElementCollector(doc)
                       .OfCategory(BuiltInCategory.OST_Doors)
                       .OfClass(typeof(FamilyInstance))
                       .WhereElementIsNotElementType()
                       .Cast<FamilyInstance>()
                       .Where(d => d.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null)
                       .Where(d => d.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId() == lv.Id)
                       .ToList();
                    foreach (FamilyInstance d in doorsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        d.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListBelowZeroCnt.ToString());
                        Parameter floorParam = d.LookupParameter(sortedLevelsListBelowZeroCnt.ToString());
                        if (floorParam != null)
                        {
                            floorParam.Set(10.764);
                        }
                        for (int i = -10; i <= 50; i++)
                        {
                            if (i != sortedLevelsListBelowZeroCnt)
                            {
                                Parameter floorParamForNull = d.LookupParameter(i.ToString());
                                if (floorParamForNull != null)
                                {
                                    if (floorParamForNull.IsReadOnly == false)
                                    {
                                        floorParamForNull.Set(0);
                                    }
                                }
                            }
                        }
                    }
                    //Ограждения
                    List<Railing> railingsList = new FilteredElementCollector(doc)
                       .OfCategory(BuiltInCategory.OST_StairsRailing)
                       .OfClass(typeof(Railing))
                       .WhereElementIsNotElementType()
                       .Cast<Railing>()
                       .Where(r => r.get_Parameter(BuiltInParameter.STAIRS_RAILING_BASE_LEVEL_PARAM) != null)
                       .Where(r => r.get_Parameter(BuiltInParameter.STAIRS_RAILING_BASE_LEVEL_PARAM).AsElementId() == lv.Id)
                       .ToList();
                    foreach (Railing r in railingsList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        r.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListBelowZeroCnt.ToString());
                    }

                    //Перемычки. Обобщенные модели
                    List<FamilyInstance> breastplatesList = new FilteredElementCollector(doc)
                       .OfCategory(BuiltInCategory.OST_GenericModel)
                       .OfClass(typeof(FamilyInstance))
                       .WhereElementIsNotElementType()
                       .Cast<FamilyInstance>()
                       .Where(bp => bp.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null)
                       .Where(bp => bp.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId() == lv.Id)
                       .ToList();
                    foreach (FamilyInstance bp in breastplatesList)
                    {
                        Guid paramGUID = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
                        bp.get_Parameter(paramGUID).Set("Этаж " + sortedLevelsListBelowZeroCnt.ToString());
                        Parameter floorParam = bp.LookupParameter(sortedLevelsListBelowZeroCnt.ToString());
                        if (floorParam != null)
                        {
                            floorParam.Set(10.764);
                        }
                        for (int i = -10; i <= 50; i++)
                        {
                            if (i != sortedLevelsListBelowZeroCnt)
                            {
                                Parameter floorParamForNull = bp.LookupParameter(i.ToString());
                                if (floorParamForNull != null)
                                {
                                    if (floorParamForNull.IsReadOnly == false)
                                    {
                                        floorParamForNull.Set(0);
                                    }
                                }
                            }
                        }
                    }

                    sortedLevelsListBelowZeroCnt--;
                }
                pbf.BeginInvoke(new Action(() => { pbf.Close(); }));
                t.Commit();
            }

            TaskDialog.Show("Revit", "Обработка завершена!");
            return Result.Succeeded;
        }
    }
}
