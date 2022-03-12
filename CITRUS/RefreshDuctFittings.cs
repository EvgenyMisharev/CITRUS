using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
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
    class RefreshDuctFittings : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            RefreshDuctFittingsStartWPF refreshDuctFittingsStartWPF = new RefreshDuctFittingsStartWPF();
            refreshDuctFittingsStartWPF.ShowDialog();
            if (refreshDuctFittingsStartWPF.DialogResult != true)
            {
                return Result.Cancelled;
            }
            string refreshOptionCheckedButtonName = refreshDuctFittingsStartWPF.RefreshOptionCheckedButtonName;
            
            List<FamilyInstance> ductFittingList = new List<FamilyInstance>();
            if (refreshOptionCheckedButtonName == "radioButton_Selected")
            {
                ICollection<ElementId> selectedIds = sel.GetElementIds();
                foreach (ElementId intersectionPointId in selectedIds)
                {
                    if (doc.GetElement(intersectionPointId) is FamilyInstance
                        && null != doc.GetElement(intersectionPointId).Category
                        && doc.GetElement(intersectionPointId).Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_DuctFitting))
                    {
                        ductFittingList.Add(doc.GetElement(intersectionPointId) as FamilyInstance);
                    }
                }

                if(ductFittingList.Count == 0)
                {
                    DuctFittingSelectionFilter selFilterDuctFitting = new DuctFittingSelectionFilter();
                    IList<Reference> selDuctFitting = null;
                    try
                    {
                        selDuctFitting = sel.PickObjects(ObjectType.Element, selFilterDuctFitting, "Выберите соединительные детали!");
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                    {
                        return Result.Cancelled;
                    }

                    foreach (Reference refElem in selDuctFitting)
                    {
                        ductFittingList.Add((doc.GetElement(refElem)) as FamilyInstance);
                    }
                }
            }
            else if (refreshOptionCheckedButtonName == "radioButton_VisibleInView")
            {
                ductFittingList = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .OfCategory(BuiltInCategory.OST_DuctFitting)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .ToList();
            }
            else if(refreshOptionCheckedButtonName == "radioButton_WholeProject")
            {
                ductFittingList = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_DuctFitting)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .ToList();
            }

            List<ElementId> errorElementsIdList = new List<ElementId>();
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Обновить фитинги");
                foreach (FamilyInstance ductFitting in ductFittingList)
                {
                    DuctType ductType = null;
                    bool ductHasChanged = false;
                    bool connectorShapeChangeFlag = false;
                    ConnectorProfileType connectorShape = ConnectorProfileType.Invalid;
                    ConnectorSet connectorSet = null;
                    try
                    {
                        connectorSet = ductFitting.MEPModel.ConnectorManager.Connectors;
                    }
                    catch
                    {
                        continue;
                    }

                    foreach (Connector pConn in connectorSet)
                    {
                        ConnectorSet connectorSetRefs = pConn.AllRefs;
                        if (connectorShape == ConnectorProfileType.Invalid)
                        {
                            connectorShape = pConn.Shape;
                        }
                        else
                        {
                            if (connectorShape != pConn.Shape)
                            {
                                connectorShapeChangeFlag = true;
                            }
                        }

                        foreach (Connector c in connectorSetRefs)
                        {
                            Element connectionElement = c.Owner;
                            if (connectionElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_DuctCurves)
                            {
                                if (ductType == null)
                                {
                                    ductType = (connectionElement as Duct).DuctType;
                                }
                                else
                                {
                                    if (ductType.Id != (connectionElement as Duct).DuctType.Id)
                                    {
                                        ductHasChanged = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (ductHasChanged) break;
                    }
                    if (ductType != null && !ductHasChanged)
                    {
                        if ((ductFitting.MEPModel as MechanicalFitting).PartType == PartType.Elbow)
                        {
                            RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                            int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Elbows);
                            if (initRuleCount != 0)
                            {
                                RoutingPreferenceRule elbowsRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Elbows, 0);
                                FamilySymbol elbowsFamilySymbol = doc.GetElement(elbowsRule.MEPPartId) as FamilySymbol;
                                if (elbowsFamilySymbol.Id != ductFitting.Symbol.Id)
                                {
                                    try
                                    {
                                        ductFitting.Symbol = elbowsFamilySymbol;
                                    }
                                    catch
                                    {
                                        if(!errorElementsIdList.Contains(ductFitting.Id))
                                        {
                                            errorElementsIdList.Add(ductFitting.Id);
                                        }
                                        continue;
                                    }
                                }
                            }
                        }
                        else if ((ductFitting.MEPModel as MechanicalFitting).PartType == PartType.TapAdjustable)
                        {
                            RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                            int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Junctions);
                            if (initRuleCount != 0)
                            {
                                RoutingPreferenceRule tapAdjustableRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Junctions, 0);
                                FamilySymbol tapAdjustableFamilySymbol = doc.GetElement(tapAdjustableRule.MEPPartId) as FamilySymbol;
                                if (tapAdjustableFamilySymbol.Id != ductFitting.Symbol.Id)
                                {
                                    try
                                    {
                                        ductFitting.Symbol = tapAdjustableFamilySymbol;
                                    }
                                    catch
                                    {
                                        if (!errorElementsIdList.Contains(ductFitting.Id))
                                        {
                                            errorElementsIdList.Add(ductFitting.Id);
                                        }
                                        continue;
                                    }
                                }
                            }
                        }
                        else if ((ductFitting.MEPModel as MechanicalFitting).PartType == PartType.Transition)
                        {
                            if (!connectorShapeChangeFlag)
                            {
                                RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                                int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Transitions);
                                if (initRuleCount != 0)
                                {
                                    RoutingPreferenceRule transitionsRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Transitions, 0);
                                    FamilySymbol transitionsFamilySymbol = doc.GetElement(transitionsRule.MEPPartId) as FamilySymbol;
                                    if (transitionsFamilySymbol.Id != ductFitting.Symbol.Id)
                                    {
                                        try
                                        {
                                            ductFitting.Symbol = transitionsFamilySymbol;
                                        }
                                        catch
                                        {
                                            if (!errorElementsIdList.Contains(ductFitting.Id))
                                            {
                                                errorElementsIdList.Add(ductFitting.Id);
                                            }
                                            continue;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                                int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.TransitionsRectangularToRound);
                                if (initRuleCount != 0)
                                {
                                    RoutingPreferenceRule transitionsRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.TransitionsRectangularToRound, 0);
                                    FamilySymbol transitionsFamilySymbol = doc.GetElement(transitionsRule.MEPPartId) as FamilySymbol;
                                    if (transitionsFamilySymbol.Id != ductFitting.Symbol.Id)
                                    {
                                        try
                                        {
                                            ductFitting.Symbol = transitionsFamilySymbol;
                                        }
                                        catch
                                        {
                                            if (!errorElementsIdList.Contains(ductFitting.Id))
                                            {
                                                errorElementsIdList.Add(ductFitting.Id);
                                            }
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                        else if ((ductFitting.MEPModel as MechanicalFitting).PartType == PartType.Cap)
                        {
                            RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                            int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Caps);
                            if (initRuleCount != 0)
                            {
                                RoutingPreferenceRule capsRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Caps, 0);
                                FamilySymbol capsRuleFamilySymbol = doc.GetElement(capsRule.MEPPartId) as FamilySymbol;
                                if (capsRuleFamilySymbol.Id != ductFitting.Symbol.Id)
                                {
                                    try
                                    {
                                        ductFitting.Symbol = capsRuleFamilySymbol;
                                    }
                                    catch
                                    {
                                        if (!errorElementsIdList.Contains(ductFitting.Id))
                                        {
                                            errorElementsIdList.Add(ductFitting.Id);
                                        }
                                        continue;
                                    }
                                }
                            }
                        }
                        else if ((ductFitting.MEPModel as MechanicalFitting).PartType == PartType.Tee)
                        {
                            RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                            int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Junctions);
                            if (initRuleCount != 0)
                            {
                                RoutingPreferenceRule junctionsRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Junctions, 0);
                                FamilySymbol junctionsRuleFamilySymbol = doc.GetElement(junctionsRule.MEPPartId) as FamilySymbol;
                                if (junctionsRuleFamilySymbol.Id != ductFitting.Symbol.Id)
                                {
                                    try
                                    {
                                        ductFitting.Symbol = junctionsRuleFamilySymbol;
                                    }
                                    catch
                                    {
                                        if (!errorElementsIdList.Contains(ductFitting.Id))
                                        {
                                            errorElementsIdList.Add(ductFitting.Id);
                                        }
                                        continue;
                                    }
                                }
                            }
                        }
                        else if ((ductFitting.MEPModel as MechanicalFitting).PartType == PartType.Cross)
                        {
                            RoutingPreferenceManager routePrefManager = ductType.RoutingPreferenceManager;
                            int initRuleCount = routePrefManager.GetNumberOfRules(RoutingPreferenceRuleGroupType.Crosses);
                            if (initRuleCount != 0)
                            {
                                RoutingPreferenceRule crossRule = routePrefManager.GetRule(RoutingPreferenceRuleGroupType.Crosses, 0);
                                FamilySymbol crossRuleFamilySymbol = doc.GetElement(crossRule.MEPPartId) as FamilySymbol;
                                if (crossRuleFamilySymbol.Id != ductFitting.Symbol.Id)
                                {
                                    try
                                    {
                                        ductFitting.Symbol = crossRuleFamilySymbol;
                                    }
                                    catch
                                    {
                                        if (!errorElementsIdList.Contains(ductFitting.Id))
                                        {
                                            errorElementsIdList.Add(ductFitting.Id);
                                        }
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
                t.Commit();
            }
            if(errorElementsIdList.Count == 0)
            {
                TaskDialog.Show("Revit", "Обработка завершена!");
            }
            else
            {
                string errorElementsIdStringList = "";
                foreach(ElementId id in errorElementsIdList)
                {
                    if(errorElementsIdStringList == "")
                    {
                        errorElementsIdStringList += id.ToString();
                    }
                    else
                    {
                        errorElementsIdStringList = errorElementsIdStringList + ";" + id.ToString();
                    }
                    
                }
                RefreshDuctFittingsErrorWPF refreshDuctFittingsErrorWPF = new RefreshDuctFittingsErrorWPF(errorElementsIdStringList);
                refreshDuctFittingsErrorWPF.ShowDialog();
            }
            return Result.Succeeded;
        }
    }
}
