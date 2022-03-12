using System;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;

namespace CITRUS
{
    class GloryHoleWindowsSelectionFilter : ISelectionFilter
	{

		public bool AllowElement(Autodesk.Revit.DB.Element elem)
		{
			if (elem is FamilyInstance 
				&& elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Windows
				&& ((elem as FamilyInstance).Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Стена" 
				|| (elem as FamilyInstance).Symbol.Family.Name == "CIT_00_Точка пересечения_Прямоугольная_Плита"))
			{
				return true;
			}
			return false;
		}

		public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
		{
			return false;
		}
	}
}
