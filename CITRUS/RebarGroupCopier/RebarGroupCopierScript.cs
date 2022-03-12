using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Structure;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class RebarGroupCopierScript : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            RebarGroupCopier.RebarGroupCopierForm rebarGroupCopierForm = new RebarGroupCopier.RebarGroupCopierForm();
            rebarGroupCopierForm.ShowDialog();
            string checkedButtonNameResult = "";
            string columnArrangementСheckedButtonName = "";
            if (rebarGroupCopierForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }
            else
            {
                checkedButtonNameResult = rebarGroupCopierForm.CheckedButtonName;
                columnArrangementСheckedButtonName = rebarGroupCopierForm.ColumnArrangementСheckedButtonName;
            }

			if (checkedButtonNameResult == "radioButton_ColumnGroups")
			{
				//Выбор группы арматуры
				GroupSelectionFilter selFilter = new GroupSelectionFilter(); //Вызов фильтра выбора
				List<Reference> selGroupList = sel.PickObjects(ObjectType.Element, selFilter, "Выберите группу!").ToList();//Получение ссылки на выбранную группу
				List<Group> myGroupList = new List<Group>();
				foreach (Reference refer in selGroupList)
				{
					myGroupList.Add(doc.GetElement(refer) as Group);
				}
				using (Transaction t = new Transaction(doc))
				{
					t.Start("Копирование групп арматуры");

					foreach (Group myGroup in myGroupList)
					{
						string rebarElemHostMarkParam = "";
						Rebar myFirstElementInGroupAsRebar = null;
						List <ElementId> myElementsInGroup = myGroup.GetDependentElements(null).ToList();
						foreach(ElementId elemId in myElementsInGroup)
                        {
							Element myElementInGroup = doc.GetElement(elemId) as Element;
							if(myElementInGroup.GetType().Name == "Rebar")
                            {
								rebarElemHostMarkParam = myElementInGroup.get_Parameter(BuiltInParameter.REBAR_ELEM_HOST_MARK).AsString();
								myFirstElementInGroupAsRebar = doc.GetElement(elemId) as Rebar;
								break;
							}
						}
						 
						ElementId myRebarHostElementId = myFirstElementInGroupAsRebar.GetHostId();
						FamilyInstance myRebarHostElement = doc.GetElement(myRebarHostElementId) as FamilyInstance;
						LocationPoint rebarHostElementLocation = myRebarHostElement.Location as LocationPoint;
						XYZ rebarHostElementLocationXYZ = rebarHostElementLocation.Point;

						List<FamilyInstance> columnsOfMyRebarHostMark = new FilteredElementCollector(doc)
							.OfClass(typeof(FamilyInstance))
							.OfCategory(BuiltInCategory.OST_StructuralColumns)
							.Where(fi => fi.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == rebarElemHostMarkParam & fi.Id != myRebarHostElementId)
							.Cast<FamilyInstance>()
							.ToList();
						foreach (FamilyInstance column in columnsOfMyRebarHostMark)
						{
							LocationPoint columnLocation = column.Location as LocationPoint;
							XYZ columnLocationXYZ = columnLocation.Point;
							XYZ vectorForGroupCopy = new XYZ(columnLocationXYZ.X - rebarHostElementLocationXYZ.X, columnLocationXYZ.Y - rebarHostElementLocationXYZ.Y, columnLocationXYZ.Z - rebarHostElementLocationXYZ.Z);

							List<ElementId> newGroupElementIdList = ElementTransformUtils.CopyElement(doc, myGroup.Id, vectorForGroupCopy) as List<ElementId>;
							ElementId newGroupElementId = newGroupElementIdList.First();

							//Ось вращения
							XYZ rotationPoint1 = new XYZ(columnLocationXYZ.X, columnLocationXYZ.Y, columnLocationXYZ.Z);
							XYZ rotationPoint2 = new XYZ(columnLocationXYZ.X, columnLocationXYZ.Y, columnLocationXYZ.Z + 1);
							Line rotationAxis = Line.CreateBound(rotationPoint1, rotationPoint2);

							if (columnLocation.Rotation != 0)
							{
								ElementTransformUtils.RotateElement(doc, newGroupElementId, rotationAxis, columnLocation.Rotation);
							}
						}
					}
					t.Commit();
				}
			}

			else if (checkedButtonNameResult == "radioButton_OutletsGroups")
			{
				//Выбор группы арматуры
				GroupSelectionFilter selFilter = new GroupSelectionFilter(); //Вызов фильтра выбора
				List<Reference> selGroupList = sel.PickObjects(ObjectType.Element, selFilter, "Выберите группу!").ToList();//Получение ссылки на выбранную группу
				List<Group> myGroupList = new List<Group>();
				foreach (Reference refer in selGroupList)
				{
					myGroupList.Add(doc.GetElement(refer) as Group);
				}
				Document doc2 = null;
				XYZ linkOrigin = new XYZ(0, 0, 0);
				if (columnArrangementСheckedButtonName == "radioButton_Link")
                {
					//Выбор связанного файла
					RevitLinkInstanceSelectionFilter selFilterRevitLinkInstance = new RevitLinkInstanceSelectionFilter(); //Вызов фильтра выбора
					Reference selRevitLinkInstance = sel.PickObject(ObjectType.Element, selFilterRevitLinkInstance, "Выберите связанный файл!");//Получение ссылки на выбранную группу
					IEnumerable<RevitLinkInstance> myRevitLinkInstance = new FilteredElementCollector(doc)
						.OfClass(typeof(RevitLinkInstance))
						.Where(li => li.Id == selRevitLinkInstance.ElementId)
						.Cast<RevitLinkInstance>();
					linkOrigin = myRevitLinkInstance.First().GetTransform().Origin;
					doc2 = myRevitLinkInstance.First().GetLinkDocument();
				}
				
				
				using (Transaction t = new Transaction(doc))
				{
					t.Start("Копирование групп арматуры");


					foreach (Group myGroup in myGroupList)
					{
						BoundingBoxXYZ bbox = myGroup.get_BoundingBox(null);
						Outline myGroupOutLn = new Outline(new XYZ (bbox.Min.X - linkOrigin.X, bbox.Min.Y - linkOrigin.Y, bbox.Min.Z), new XYZ (bbox.Max.X - linkOrigin.X, bbox.Max.Y - linkOrigin.Y, bbox.Max.Z));

						List<FamilyInstance> myColumnsList = new List<FamilyInstance>();
						if (myGroup.Name.Split(' ').Length > 1)
						{
							if (columnArrangementСheckedButtonName == "radioButton_Link")
							{
								//Список колонн не пересекающих группу
								myColumnsList = new FilteredElementCollector(doc2)
									.OfClass(typeof(FamilyInstance))
									.OfCategory(BuiltInCategory.OST_StructuralColumns)
									.WherePasses(new BoundingBoxIntersectsFilter(myGroupOutLn, true))
									.Where(fi => fi.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == myGroup.Name.Split(' ')[1])
									.Cast<FamilyInstance>()
									.ToList();
							}
                            else
							{
								myColumnsList = new FilteredElementCollector(doc)
									.OfClass(typeof(FamilyInstance))
									.OfCategory(BuiltInCategory.OST_StructuralColumns)
									.WherePasses(new BoundingBoxIntersectsFilter(myGroupOutLn, true))
									.Where(fi => fi.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == myGroup.Name.Split(' ')[1])
									.Cast<FamilyInstance>()
									.ToList();
							}
						}
						else
                        {
							TaskDialog.Show("Revit", "Группа " + "\"" + myGroup.Name+ "\"" + "\nНазвание некорректно!");
							continue;
                        }

						LocationPoint groupLocation = myGroup.Location as LocationPoint;
						XYZ groupLocationXYZ = groupLocation.Point;

						foreach (FamilyInstance column in myColumnsList)
						{
							LocationPoint columnLocation = column.Location as LocationPoint;
							XYZ columnLocationXYZLink = columnLocation.Point;
							XYZ columnLocationXYZ = new XYZ(columnLocationXYZLink.X + linkOrigin.X, columnLocationXYZLink.Y + linkOrigin.Y, columnLocationXYZLink.Z);
							XYZ vectorForGroupCopy = new XYZ(columnLocationXYZ.X - groupLocationXYZ.X, columnLocationXYZ.Y - groupLocationXYZ.Y, 0);

							List<ElementId> newGroupElementIdList = ElementTransformUtils.CopyElement(doc, myGroup.Id, vectorForGroupCopy) as List<ElementId>;
							ElementId newGroupElementId = newGroupElementIdList.First();

							//Ось вращения
							XYZ rotationPoint1 = new XYZ(columnLocationXYZ.X, columnLocationXYZ.Y, columnLocationXYZ.Z);
							XYZ rotationPoint2 = new XYZ(columnLocationXYZ.X, columnLocationXYZ.Y, columnLocationXYZ.Z + 1);
							Line rotationAxis = Line.CreateBound(rotationPoint1, rotationPoint2);

							if (columnLocation.Rotation != 0)
							{
								ElementTransformUtils.RotateElement(doc, newGroupElementId, rotationAxis, columnLocation.Rotation);
							}

						}
					}

					t.Commit();
				}
			}

			return Result.Succeeded;
        }
    }
}
