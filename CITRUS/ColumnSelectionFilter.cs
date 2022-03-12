using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;


namespace CITRUS
{
    class ColumnSelectionFilter : ISelectionFilter
    {
		
		public bool AllowElement(Autodesk.Revit.DB.Element elem)
		{
			return elem is FamilyInstance && null != elem.Category && elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralColumns);
		}

		public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
		{
			return false;
		}
	}
}
