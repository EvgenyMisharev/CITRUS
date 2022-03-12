using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CapitalMaker : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			//Получение текущего документа
			Document doc = commandData.Application.ActiveUIDocument.Document;

			//Получение доступа к Selection
			Selection sel = commandData.Application.ActiveUIDocument.Selection;

			//Выбор колонн
			ColumnSelectionFilter selFilter = new ColumnSelectionFilter(); //Вызов фильтра выбора
			IList<Reference> selColumns = sel.PickObjects(ObjectType.Element, selFilter, "Выберите колонны!");//Получение списка ссылок на выбранные колонны

			List<FamilyInstance> columnsList = new List<FamilyInstance>();//Получение списка выбранных колонн
			foreach (Reference columnRef in selColumns)
			{
				columnsList.Add(doc.GetElement(columnRef) as FamilyInstance);
			}

			//Выбор перекрытия
			FloorSelectionFilter selFloorFilter = new FloorSelectionFilter();//Вызов фильтра выбора
			IList<Reference> selFloors = sel.PickObjects(ObjectType.Element, selFloorFilter, "Выберите перекрытие!");//Получение списка ссылок на выбранные перекрытия

			List<Floor> floorList = new List<Floor>();//Получение списка выбранных перекрытий
			foreach (Reference floorRef in selFloors)
			{
				floorList.Add(doc.GetElement(floorRef) as Floor);
			}

			Floor targetFloor = floorList.First();//Выбор первого перекрытия из списка перекрытий

			double levelOffset = targetFloor.LookupParameter("Смещение от уровня").AsDouble();//Получение смещения перекрытия от уровня
			double floorThickness = targetFloor.LookupParameter("Толщина").AsDouble();//Получение толщины перекрытия

			FilteredElementCollector levels = new FilteredElementCollector(doc)
				.WhereElementIsNotElementType()
				.OfCategory(BuiltInCategory.OST_Levels)
				.OfClass(typeof(Level));

			int levelIdInt = targetFloor.LevelId.IntegerValue;

			Level myLevel = null;

			foreach (Level e in levels)
			{
				if (e.Id.IntegerValue.Equals(levelIdInt))
				{
					myLevel = e;
				}
			}

			double targetOffset = (myLevel.Elevation+(levelOffset - floorThickness));//Получение смещения по Z для создания нового элемента
			
			List<FloorType> myFloorTypeList = new FilteredElementCollector(doc).OfClass(typeof(FloorType)).Cast<FloorType>().ToList();


			FloorTypeSelectorForCapitalMaker formFloorTypeSelectorForCapitalMaker = new FloorTypeSelectorForCapitalMaker(myFloorTypeList);
			formFloorTypeSelectorForCapitalMaker.ShowDialog();
			FloorType myFloorType = formFloorTypeSelectorForCapitalMaker.mySelectionFloorTypeForCapitalMaker;
			double targetOffsetForCurves_X1 = formFloorTypeSelectorForCapitalMaker.myOffsetForCapitalMaker_X1/304.8;
			double targetOffsetForCurves_Y1 = formFloorTypeSelectorForCapitalMaker.myOffsetForCapitalMaker_Y1 / 304.8;
			double targetOffsetForCurves_X2 = formFloorTypeSelectorForCapitalMaker.myOffsetForCapitalMaker_X2 / 304.8;
			double targetOffsetForCurves_Y2 = formFloorTypeSelectorForCapitalMaker.myOffsetForCapitalMaker_Y2 / 304.8;

			using (Transaction t = new Transaction(doc))
			{
				t.Start("Размещение семейств");
				foreach (FamilyInstance column in columnsList)
				{
					LocationPoint location = column.Location as LocationPoint;
					XYZ targetPoint = location.Point;
					//AnalyticalModel modelColumn = column.GetAnalyticalModel();
					//Curve columnCurve = modelColumn.GetCurve();
					XYZ targetPointOffset = new XYZ(targetPoint.X, targetPoint.Y, targetOffset);

					CurveArray capitalesCurves = new CurveArray();

					List<XYZ> points1 = new List<XYZ>();
					points1.Add(new XYZ(targetPointOffset.X - targetOffsetForCurves_X1, targetPointOffset.Y + targetOffsetForCurves_Y1, targetPointOffset.Z));
					points1.Add(new XYZ(targetPointOffset.X + targetOffsetForCurves_X2, targetPointOffset.Y + targetOffsetForCurves_Y1, targetPointOffset.Z));

					capitalesCurves.Append(HermiteSpline.Create(points1, false));

					List<XYZ> points2 = new List<XYZ>();
					points2.Add(new XYZ(targetPointOffset.X + targetOffsetForCurves_X2, targetPointOffset.Y + targetOffsetForCurves_Y1, targetPointOffset.Z));
					points2.Add(new XYZ(targetPointOffset.X + targetOffsetForCurves_X2, targetPointOffset.Y - targetOffsetForCurves_Y2, targetPointOffset.Z));


					capitalesCurves.Append(HermiteSpline.Create(points2, false));

					List<XYZ> points3 = new List<XYZ>();
					points3.Add(new XYZ(targetPointOffset.X + targetOffsetForCurves_X2, targetPointOffset.Y - targetOffsetForCurves_Y2, targetPointOffset.Z));
					points3.Add(new XYZ(targetPointOffset.X - targetOffsetForCurves_X1, targetPointOffset.Y - targetOffsetForCurves_Y2, targetPointOffset.Z));


					capitalesCurves.Append(HermiteSpline.Create(points3, false));

					List<XYZ> points4 = new List<XYZ>();
					points4.Add(new XYZ(targetPointOffset.X - targetOffsetForCurves_X1, targetPointOffset.Y - targetOffsetForCurves_Y2, targetPointOffset.Z));
					points4.Add(new XYZ(targetPointOffset.X - targetOffsetForCurves_X1, targetPointOffset.Y + targetOffsetForCurves_Y1, targetPointOffset.Z));


					capitalesCurves.Append(HermiteSpline.Create(points4, false));

					Floor myCapital = doc.Create.NewFloor(capitalesCurves, myFloorType, myLevel, false);

					
					//Проверяем повернута ли колонна. Если да, то поворачиваем капитель вдоль оси колонны.
					if (location.Rotation != 0)
					{
						string capitalRotation = location.Rotation.ToString();
						double capitalRotationD = 0;
						double.TryParse(capitalRotation, out capitalRotationD);

						XYZ point1 = new XYZ(targetPointOffset.X, targetPointOffset.Y, targetPointOffset.Z);
						XYZ point2 = new XYZ(targetPointOffset.X, targetPointOffset.Y, targetPointOffset.Z + 5);
						Line axis = Line.CreateBound(point1, point2);

						ElementTransformUtils.RotateElement(doc, myCapital.Id, axis, capitalRotationD);

					}

					JoinGeometryUtils.JoinGeometry(doc, myCapital,column);

				}
				t.Commit();

			}

			return Result.Succeeded;
        }
    }
}
