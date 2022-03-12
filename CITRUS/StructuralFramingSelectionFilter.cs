using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS
{
    class StructuralFramingSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is FamilyInstance && null != elem.Category && elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralFraming);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
