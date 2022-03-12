using System;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;

namespace CITRUS
{
    class PipeDuctSelectionFilter : ISelectionFilter
    {
		public bool AllowElement(Element elem)
		{

			if (elem is Pipe || elem is Duct)
			{
				return true;
			}
			return false;
		}

		public bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}
}
