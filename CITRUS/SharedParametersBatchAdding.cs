using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SharedParametersBatchAdding : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            DefinitionFile defFile = uidoc.Application.Application.OpenSharedParameterFile();
            DefinitionGroups sharedParametersGroups = defFile.Groups;
            ObservableCollection<KeyValuePair<string, BuiltInParameterGroup>> builtInParameterGroupKeyValuePairs = new ObservableCollection<KeyValuePair<string, BuiltInParameterGroup>>();
            foreach (BuiltInParameterGroup item in (BuiltInParameterGroup[])Enum.GetValues(typeof(BuiltInParameterGroup)))
            {
                try
                {
                    builtInParameterGroupKeyValuePairs.Add(new KeyValuePair<string, BuiltInParameterGroup>(LabelUtils.GetLabelFor(item), item));
                }
                catch { }
            }
            builtInParameterGroupKeyValuePairs = new ObservableCollection<KeyValuePair<string, BuiltInParameterGroup>>(builtInParameterGroupKeyValuePairs.OrderBy(i => i.Key));
            SharedParametersBatchAddingFormWPF sharedParametersBatchAddingFormWPF = new SharedParametersBatchAddingFormWPF(sharedParametersGroups, builtInParameterGroupKeyValuePairs);
            sharedParametersBatchAddingFormWPF.ShowDialog();
            if (sharedParametersBatchAddingFormWPF.DialogResult != true)
            {
                return Result.Cancelled;
            }
            string addParametersSelectedOption = sharedParametersBatchAddingFormWPF.AddParametersSelectedOption;
            IList<SharedParametersBatchAddingItem> sharedParametersBatchAddingItemsList = sharedParametersBatchAddingFormWPF.SharedParametersBatchAddingItemsList;
            if (addParametersSelectedOption == "radioButton_ActiveFamily")
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;
                if (doc.IsFamilyDocument)
                {
                    using (Transaction t = new Transaction(doc))
                    {
                        t.Start("Добавление параметров в семейство");
                        IList<FamilyParameter> fPars = doc.FamilyManager.GetParameters().Where(p => p.IsShared).ToList();

                        foreach (SharedParametersBatchAddingItem sharedParameterItem in sharedParametersBatchAddingItemsList)
                        {
                            IList<FamilyParameter> inFamily = fPars.Where(p => p.GUID == sharedParameterItem.ExternalDefinitionParam.GUID).ToList();
                            if (inFamily.Count == 0)
                            {
                                FamilyParameter fp = doc.FamilyManager.AddParameter(sharedParameterItem.ExternalDefinitionParam, sharedParameterItem.BuiltInParameterGroupParam.Value, sharedParameterItem.AddParameterSelectedOptionParam) as FamilyParameter;
                                if (sharedParameterItem.FormulaParam != null && sharedParameterItem.FormulaParam != "")
                                {
                                    try
                                    {
                                        doc.FamilyManager.SetFormula(fp, sharedParameterItem.FormulaParam);
                                    }
                                    catch
                                    {
                                        TaskDialog.Show("Revit", "Проверьте формат формулы в параметре \"" + sharedParameterItem.ExternalDefinitionParam.Name + "\"!");
                                    }
                                }
                            }
                            else
                            {
                                if (sharedParameterItem.FormulaParam != null && sharedParameterItem.FormulaParam != "")
                                {
                                    try
                                    {
                                        doc.FamilyManager.SetFormula(fPars.Where(p => p.GUID == sharedParameterItem.ExternalDefinitionParam.GUID).ToList().First(), sharedParameterItem.FormulaParam);
                                    }
                                    catch
                                    {
                                        TaskDialog.Show("Revit", "Проверьте формат формулы в параметре \"" + sharedParameterItem.ExternalDefinitionParam.Name + "\"!");
                                    }
                                }
                            }

                        }
                        t.Commit();
                    }
                }
                else
                {
                    TaskDialog.Show("Revit","Текущий документ не является семейством!");
                    return Result.Cancelled;
                }
            }
            else if(addParametersSelectedOption == "radioButton_AllOpenFamilies")
            {
                DocumentSet documentSet = commandData.Application.Application.Documents;
                foreach(Document doc in documentSet)
                {
                    if (doc.IsFamilyDocument)
                    {
                        using (Transaction t = new Transaction(doc))
                        {
                            t.Start("Добавление параметров в семейство");
                            IList<FamilyParameter> fPars = doc.FamilyManager.GetParameters().Where(p => p.IsShared).ToList();

                            foreach (SharedParametersBatchAddingItem sharedParameterItem in sharedParametersBatchAddingItemsList)
                            {
                                IList<FamilyParameter> inFamily = fPars.Where(p => p.GUID == sharedParameterItem.ExternalDefinitionParam.GUID).ToList();
                                if (inFamily.Count == 0)
                                {
                                    FamilyParameter fp = doc.FamilyManager.AddParameter(sharedParameterItem.ExternalDefinitionParam, sharedParameterItem.BuiltInParameterGroupParam.Value, sharedParameterItem.AddParameterSelectedOptionParam) as FamilyParameter;
                                    if (sharedParameterItem.FormulaParam != null && sharedParameterItem.FormulaParam != "")
                                    {
                                        try
                                        {
                                            doc.FamilyManager.SetFormula(fp, sharedParameterItem.FormulaParam);
                                        }
                                        catch
                                        {
                                            TaskDialog.Show("Revit", "Проверьте формат формулы в параметре \"" + sharedParameterItem.ExternalDefinitionParam.Name + "\"!");
                                        }
                                    }
                                }
                                else
                                {
                                    if (sharedParameterItem.FormulaParam != null && sharedParameterItem.FormulaParam != "")
                                    {
                                        try
                                        {
                                            doc.FamilyManager.SetFormula(fPars.Where(p => p.GUID == sharedParameterItem.ExternalDefinitionParam.GUID).ToList().First(), sharedParameterItem.FormulaParam);
                                        }
                                        catch
                                        {
                                            TaskDialog.Show("Revit", "Проверьте формат формулы в параметре \"" + sharedParameterItem.ExternalDefinitionParam.Name + "\"!");
                                        }
                                    }
                                }
                            }
                            t.Commit();
                        }
                    }
                }
            }
            else if (addParametersSelectedOption == "radioButton_FamiliesInSelectedFolder")
            {
                if(sharedParametersBatchAddingFormWPF.FilePath == null)
                {
                    TaskDialog.Show("Revit", "Не выбран путь к папке с семействами!");
                    return Result.Cancelled;
                }
                string[] files = Directory.GetFiles(sharedParametersBatchAddingFormWPF.FilePath).Where(p=>p.Split('.').Last() == "rfa").ToArray();
                foreach (string file in files)
                {
                    Document familyDoc = commandData.Application.Application.OpenDocumentFile(file);
                    using (Transaction t = new Transaction(familyDoc))
                    {
                        t.Start("Добавление параметров в семейство");
                        IList<FamilyParameter> fPars = familyDoc.FamilyManager.GetParameters().Where(p => p.IsShared).ToList();

                        foreach (SharedParametersBatchAddingItem sharedParameterItem in sharedParametersBatchAddingItemsList)
                        {
                            IList<FamilyParameter> inFamily = fPars.Where(p => p.GUID == sharedParameterItem.ExternalDefinitionParam.GUID).ToList();
                            if (inFamily.Count == 0)
                            {
                                FamilyParameter fp = familyDoc.FamilyManager.AddParameter(sharedParameterItem.ExternalDefinitionParam, sharedParameterItem.BuiltInParameterGroupParam.Value, sharedParameterItem.AddParameterSelectedOptionParam) as FamilyParameter;
                                if (sharedParameterItem.FormulaParam != null && sharedParameterItem.FormulaParam != "")
                                {
                                    try
                                    {
                                        familyDoc.FamilyManager.SetFormula(fp, sharedParameterItem.FormulaParam);
                                    }
                                    catch
                                    {
                                        TaskDialog.Show("Revit", "Проверьте формат формулы в параметре \"" + sharedParameterItem.ExternalDefinitionParam.Name + "\"!");
                                    }
                                }
                            }
                            else
                            {
                                if (sharedParameterItem.FormulaParam != null && sharedParameterItem.FormulaParam != "")
                                {
                                    try
                                    {
                                        familyDoc.FamilyManager.SetFormula(fPars.Where(p => p.GUID == sharedParameterItem.ExternalDefinitionParam.GUID).ToList().First(), sharedParameterItem.FormulaParam);
                                    }
                                    catch
                                    {
                                        TaskDialog.Show("Revit", "Проверьте формат формулы в параметре \"" + sharedParameterItem.ExternalDefinitionParam.Name + "\"!");
                                    }
                                }
                            }
                        }
                        t.Commit();
                    }
                    familyDoc.Close();
                }
            }
            return Result.Succeeded;
        }
    }
}
