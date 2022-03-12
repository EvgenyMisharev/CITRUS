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
    class GloryHoleSaveAssignmentVersion : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Guid intersectionPointWidthGuid = new Guid("8f2e4f93-9472-4941-a65d-0ac468fd6a5d");
            Guid intersectionPointHeightGuid = new Guid("da753fe3-ecfa-465b-9a2c-02f55d0c2ff1");
            Guid intersectionPointThicknessGuid = new Guid("293f055d-6939-4611-87b7-9a50d0c1f50e");

            Guid adskLevelGuid = new Guid("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
            Guid assignmentVersionGuid = new Guid("c61dd38f-fc8c-4239-9056-dc5baa3c7304");

            List<FamilyInstance> intersectionPointFamilyInstanceList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .Where(ip => ip.Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Стена" || ip.Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Плита")
                .ToList();

            //Вызов формы
            GloryHoleSaveAssignmentVersionWPF gloryHoleSaveAssignmentVersionWPF = new GloryHoleSaveAssignmentVersionWPF();
            gloryHoleSaveAssignmentVersionWPF.ShowDialog();
            if (gloryHoleSaveAssignmentVersionWPF.DialogResult != true)
            {
                return Result.Cancelled;
            }
            string actionSelectionButtonName = gloryHoleSaveAssignmentVersionWPF.ActionSelectionButtonName;
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Сохранение версии задания");
                if (actionSelectionButtonName == "radioButton_SaveAssignmentVersion")
                {
                    foreach (FamilyInstance ip in intersectionPointFamilyInstanceList)
                    {
                        string intersectionPointMarkString = ip.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                        string intersectionPointWidthString = Math.Round(ip.get_Parameter(intersectionPointWidthGuid).AsDouble(), 6).ToString();
                        string intersectionPointHeightString = Math.Round(ip.get_Parameter(intersectionPointHeightGuid).AsDouble(), 6).ToString();
                        string intersectionPointThicknessString = Math.Round(ip.get_Parameter(intersectionPointThicknessGuid).AsDouble(), 6).ToString();
                        string intersectionPointXString = Math.Round(((ip.Location as LocationPoint).Point.X), 6).ToString();
                        string intersectionPointYString = Math.Round(((ip.Location as LocationPoint).Point.Y), 6).ToString();
                        string intersectionPointZString = Math.Round(((ip.Location as LocationPoint).Point.Z), 6).ToString();

                        if (!ip.get_Parameter(assignmentVersionGuid).AsString().StartsWith("в"))
                        {
                            ip.get_Parameter(assignmentVersionGuid).Set($"в1" +
                                $"|{intersectionPointMarkString}" +
                                $"|{intersectionPointWidthString}" +
                                $"|{intersectionPointHeightString}" +
                                $"|{intersectionPointThicknessString}" +
                                $"|{intersectionPointXString}" +
                                $"|{intersectionPointYString}" +
                                $"|{intersectionPointZString}");
                        }
                        else
                        {
                            string[] assignmentVersionString = ip.get_Parameter(assignmentVersionGuid).AsString().Split('|');
                            if (assignmentVersionString[1] != intersectionPointMarkString 
                                || assignmentVersionString[2] != intersectionPointWidthString
                                || assignmentVersionString[3] != intersectionPointHeightString
                                || assignmentVersionString[4] != intersectionPointThicknessString
                                || assignmentVersionString[5] != intersectionPointXString
                                || assignmentVersionString[6] != intersectionPointYString
                                || assignmentVersionString[7] != intersectionPointZString)
                            {
                                char[] versionNameList = ip.get_Parameter(assignmentVersionGuid).AsString().Split('|')[0].ToCharArray();
                                Int32.TryParse(versionNameList[1].ToString(), out int versionNumber);
                                versionNumber++;
                                ip.get_Parameter(assignmentVersionGuid).Set($"в{versionNumber}"+
                                $"|{intersectionPointMarkString}" +
                                $"|{intersectionPointWidthString}" +
                                $"|{intersectionPointHeightString}" +
                                $"|{intersectionPointThicknessString}" +
                                $"|{intersectionPointXString}" +
                                $"|{intersectionPointYString}" +
                                $"|{intersectionPointZString}");
                            }
                        }
                    }
                }
                else if(actionSelectionButtonName == "radioButton_ResetAssignmentVersion")
                {
                    foreach (FamilyInstance ip in intersectionPointFamilyInstanceList)
                    {
                        ip.get_Parameter(assignmentVersionGuid).Set("");
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
