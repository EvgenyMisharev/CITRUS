using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SystemsToSpace : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Guid adsk_SupplySystemNameGuid = new Guid("5162f6a4-55c5-43e6-95f4-c06ace52faa0");
            Guid adsk_ExhaustSystemNameGuid = new Guid("f11ed0d9-3b91-4d35-bc44-01a579e45ce7");
            Guid adsk_AirConsumptionGuid = new Guid("9b7d541b-cc04-4e41-949d-a0d6ed778a25");
            Guid adsk_EstimatedSupplyGuid = new Guid("ff939149-328d-421c-93c3-3348a7e55697");
            Guid adsk_EstimatedExhaustGuid = new Guid("550f0463-71d7-4856-879c-11f9004d5789");

            List<Space> spaceList = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_MEPSpaces)
                .Cast<Space>()
                .ToList();
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Системы в пространства");
                foreach (Space space in spaceList)
                {
                    List<FamilyInstance> ductTerminalList = new FilteredElementCollector(doc)
                        .WhereElementIsNotElementType()
                        .OfCategory(BuiltInCategory.OST_DuctTerminal)
                        .Cast<FamilyInstance>()
                        .Where(dt => dt.Space != null)
                        .Where(dt => dt.Space.Id == space.Id)
                        .Where(dt => dt.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM).AsString() != null)
                        .ToList();

                    List<string> supplySystemNameList = new List<string>();
                    List<string> exhaustSystemNameList = new List<string>();
                    double adsk_EstimatedSupply = 0;
                    double adsk_EstimatedExhaust = 0;
                    foreach (FamilyInstance ductTerminal in ductTerminalList)
                    {
                        if (ductTerminal.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM) != null)
                        {
                            string systemName = ductTerminal.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM).AsString();
                            if (systemName.StartsWith("П"))
                            {
                                if(!supplySystemNameList.Contains(systemName))
                                {
                                    supplySystemNameList.Add(systemName);
                                }
                                adsk_EstimatedSupply += ductTerminal.get_Parameter(adsk_AirConsumptionGuid).AsDouble();
                            }
                            else if (systemName.StartsWith("В"))
                            {
                                if (!exhaustSystemNameList.Contains(systemName))
                                {
                                    exhaustSystemNameList.Add(systemName);
                                }
                                adsk_EstimatedExhaust += ductTerminal.get_Parameter(adsk_AirConsumptionGuid).AsDouble();
                            }
                        }
                    }
                    supplySystemNameList.Sort(new AlphanumComparatorFastString());
                    exhaustSystemNameList.Sort(new AlphanumComparatorFastString());

                    string supplySystemNames = "";
                    foreach (string sn in supplySystemNameList)
                    {
                        if (supplySystemNames == "")
                        {
                            supplySystemNames = sn;
                        }
                        else
                        {
                            supplySystemNames += ", " + sn;
                        }
                    }
                    space.get_Parameter(adsk_SupplySystemNameGuid).Set(supplySystemNames);
                    
                    string exhaustSystemNames = "";
                    foreach (string sn in exhaustSystemNameList)
                    {
                        if (exhaustSystemNames == "")
                        {
                            exhaustSystemNames = sn;
                        }
                        else
                        {
                            exhaustSystemNames += ", " + sn;
                        }
                    }
                    space.get_Parameter(adsk_ExhaustSystemNameGuid).Set(exhaustSystemNames);
                    space.get_Parameter(adsk_EstimatedSupplyGuid).Set(adsk_EstimatedSupply);
                    space.get_Parameter(adsk_EstimatedExhaustGuid).Set(adsk_EstimatedExhaust);
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
