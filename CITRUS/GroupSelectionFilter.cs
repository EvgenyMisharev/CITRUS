using System;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;

namespace CITRUS
{
    class GroupSelectionFilter : ISelectionFilter
	{

		public bool AllowElement(Autodesk.Revit.DB.Element elem)
		{
			if (elem is Group)
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
