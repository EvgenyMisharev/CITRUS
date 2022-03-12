using System;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;

namespace CITRUS
{
    class SpaceSelectionFilter : ISelectionFilter
	{

		public bool AllowElement(Autodesk.Revit.DB.Element elem)
		{
			if (elem is Space)
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
