using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class MEPViewScheduleCreator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            
            List<MechanicalSystem> mechanicalSystemList = new FilteredElementCollector(doc).OfClass(typeof(MechanicalSystem)).Cast<MechanicalSystem>().ToList();
            ViewSchedule viewScheduleMEPEquipment = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>().Where(vs => vs.Name == "MEPViewScheduleCreator_Оборудование").First();
            ViewSchedule viewScheduleMEPRectangularDucts = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>().Where(vs => vs.Name == "MEPViewScheduleCreator_Воздуховоды_Прямоугольные").First();
            ViewSchedule viewScheduleMEPRoundDucts = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>().Where(vs => vs.Name == "MEPViewScheduleCreator_Воздуховоды_Круглые").First();
            ViewSchedule viewScheduleMEPInsulation = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>().Where(vs => vs.Name == "MEPViewScheduleCreator_Изоляция").First();

            //Придумать проверку на наличие спецификации
            List<ViewSchedule> viewSchedules = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSchedule))
                    .Cast<ViewSchedule>()
                    .Where(vs => vs.Name.ToString().Split('_').Length > 1)
                    .Where(vs => vs.Name.ToString().Split('_')[1] == "5.4.1"
                    || vs.Name.ToString().Split('_')[1] == "5.4.2"
                    || vs.Name.ToString().Split('_')[1] == "5.4.3"
                    || vs.Name.ToString().Split('_')[1] == "5.4.4")
                    .OrderBy(vs => vs.Name)
                    .ToList();
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Создание спецификаций");

                foreach (MechanicalSystem mechanicalSystem in mechanicalSystemList)
                {
                    if (viewSchedules.Any(i => i.Name.Split('_')[0] == mechanicalSystem.Name))
                    {
                        continue;
                    }
                    else
                    {
                        //Спецификация оборудования
                        ElementId newViewScheduleEquipmentMEPId = viewScheduleMEPEquipment.Duplicate(new ViewDuplicateOption());
                        ViewSchedule newViewScheduleEquipmentMEP = doc.GetElement(newViewScheduleEquipmentMEPId) as ViewSchedule;
                        newViewScheduleEquipmentMEP.Name = mechanicalSystem.Name + "_5.4.1_Оборудование";
                        var scheduleEquipmentDefinition = newViewScheduleEquipmentMEP.Definition;
                        ScheduleFilter scheduleEquipmentFilter = scheduleEquipmentDefinition.GetFilter(6);
                        scheduleEquipmentFilter.FilterType = ScheduleFilterType.Equal;
                        scheduleEquipmentFilter.SetValue(mechanicalSystem.Name);
                        scheduleEquipmentDefinition.SetFilter(6, scheduleEquipmentFilter);
                        string rowNamEquipmentStr = newViewScheduleEquipmentMEP.GetTableData().GetSectionData(SectionType.Body).NumberOfRows.ToString();
                        Int32.TryParse(rowNamEquipmentStr, out int rowNamEquipment);
                        if (rowNamEquipment == 0)
                        {
                            doc.Delete(newViewScheduleEquipmentMEPId);
                        }

                        //Спецификация прямоугольных воздуховодов
                        ElementId newViewScheduleRectangularDuctsMEPId = viewScheduleMEPRectangularDucts.Duplicate(new ViewDuplicateOption());
                        ViewSchedule newViewScheduleRectangularDuctsMEP = doc.GetElement(newViewScheduleRectangularDuctsMEPId) as ViewSchedule;
                        newViewScheduleRectangularDuctsMEP.Name = mechanicalSystem.Name + "_5.4.2_ВоздуховодыПрямоугольные";
                        var scheduleRectangularDuctsDefinition = newViewScheduleRectangularDuctsMEP.Definition;
                        ScheduleFilter scheduleRectangularDuctsFilter = scheduleRectangularDuctsDefinition.GetFilter(5);
                        scheduleRectangularDuctsFilter.FilterType = ScheduleFilterType.Equal;
                        scheduleRectangularDuctsFilter.SetValue(mechanicalSystem.Name);
                        scheduleRectangularDuctsDefinition.SetFilter(5, scheduleRectangularDuctsFilter);

                        string rowNamRectangularDuctsStr = newViewScheduleRectangularDuctsMEP.GetTableData().GetSectionData(SectionType.Body).NumberOfRows.ToString();
                        Int32.TryParse(rowNamRectangularDuctsStr, out int rowNamRectangularDucts);
                        if (rowNamRectangularDucts == 0)
                        {
                            doc.Delete(newViewScheduleRectangularDuctsMEPId);
                        }

                        //Спецификация круглых воздуховодов
                        ElementId newViewScheduleRoundDuctsMEPId = viewScheduleMEPRoundDucts.Duplicate(new ViewDuplicateOption());
                        ViewSchedule newViewScheduleRoundDuctsMEP = doc.GetElement(newViewScheduleRoundDuctsMEPId) as ViewSchedule;
                        newViewScheduleRoundDuctsMEP.Name = mechanicalSystem.Name + "_5.4.3_ВоздуховодыКруглые";
                        var scheduleRoundDuctsDefinition = newViewScheduleRoundDuctsMEP.Definition;
                        ScheduleFilter scheduleRoundDuctsFilter = scheduleRoundDuctsDefinition.GetFilter(5);
                        scheduleRoundDuctsFilter.FilterType = ScheduleFilterType.Equal;
                        scheduleRoundDuctsFilter.SetValue(mechanicalSystem.Name);
                        scheduleRoundDuctsDefinition.SetFilter(5, scheduleRoundDuctsFilter);

                        string rowNamRoundDuctsStr = newViewScheduleRoundDuctsMEP.GetTableData().GetSectionData(SectionType.Body).NumberOfRows.ToString();
                        Int32.TryParse(rowNamRoundDuctsStr, out int rowNamRoundDucts);
                        if (rowNamRoundDucts == 0)
                        {
                            doc.Delete(newViewScheduleRoundDuctsMEPId);
                        }

                        //Спецификация изоляции
                        ElementId newViewScheduleInsulationMEPId = viewScheduleMEPInsulation.Duplicate(new ViewDuplicateOption());
                        ViewSchedule newViewScheduleInsulationMEP = doc.GetElement(newViewScheduleInsulationMEPId) as ViewSchedule;
                        newViewScheduleInsulationMEP.Name = mechanicalSystem.Name + "_5.4.4_Изоляция";
                        var scheduleInsulationDefinition = newViewScheduleInsulationMEP.Definition;
                        ScheduleFilter scheduleInsulationFilter = scheduleInsulationDefinition.GetFilter(2);
                        scheduleInsulationFilter.FilterType = ScheduleFilterType.Equal;
                        scheduleInsulationFilter.SetValue(mechanicalSystem.Name);
                        scheduleInsulationDefinition.SetFilter(2, scheduleInsulationFilter);

                        string rowNamInsulationStr = newViewScheduleInsulationMEP.GetTableData().GetSectionData(SectionType.Body).NumberOfRows.ToString();
                        Int32.TryParse(rowNamInsulationStr, out int rowNamInsulation);
                        if (rowNamInsulation == 0)
                        {
                            doc.Delete(newViewScheduleInsulationMEPId);
                        }
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
