using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace CITRUS
{
    class DuctFittingSelectionFilter : ISelectionFilter
    {
		public bool AllowElement(Autodesk.Revit.DB.Element elem)
		{
			if (elem is FamilyInstance
				&& elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_DuctFitting)
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
