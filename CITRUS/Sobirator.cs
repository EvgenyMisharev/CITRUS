using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace CITRUS
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	class Sobirator : IExternalCommand
	{

		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			Document doc = commandData.Application.ActiveUIDocument.Document;
			List<Element> titleBlockList = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks).WhereElementIsNotElementType().ToList();

			List<View> sheetList = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).ToList().ConvertAll(e => { return doc.GetElement(e.Id) as View; });
			SortedList<string, ViewSet> viewSheetSetsList = new SortedList<string, ViewSet>(); //коллекция ключ (строка) / значение (ViewSet)



			foreach (Element e in titleBlockList)
			{
				Parameter s_number = e.get_Parameter(BuiltInParameter.SHEET_NUMBER);
				View sheet = sheetList.Where(v => v.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString() == s_number.AsString()).First();
				Parameter s_height = e.get_Parameter(BuiltInParameter.SHEET_HEIGHT);
				Parameter s_width = e.get_Parameter(BuiltInParameter.SHEET_WIDTH);

				string s_complect = "";

				//получаю "комплект" из листа с тем же номером, что у titleblock
				foreach (Element sh in sheetList)
				{
					Parameter sheet_number = sh.get_Parameter(BuiltInParameter.SHEET_NUMBER);
					if (sheet_number.AsString() == s_number.AsString())
					{
						if (sh.GetParameters("Раздел проекта").Count != 0)
						{
							s_complect = sh.GetParameters("Раздел проекта")[0].AsString();
						}

						else if (sh.GetParameters("РОВЕН_Раздел проекта").Count != 0)
						{
							s_complect = sh.GetParameters("РОВЕН_Раздел проекта")[0].AsString();
						}

						else if (sh.GetParameters("Орг.КомплектЧертежей").Count != 0)						{
							s_complect = sh.GetParameters("Орг.КомплектЧертежей")[0].AsString();
						}
						break;
					}
				}

				string s_format = s_complect + ": " + GetSheetFormat(s_height.AsValueString(), s_width.AsValueString());

				if (viewSheetSetsList.Keys.Contains(s_format))
				{
					ViewSet vs = viewSheetSetsList.Values.ElementAt(viewSheetSetsList.Keys.IndexOf(s_format));
					vs.Insert(sheet);
				}
				else
				{
					ViewSet vs = new ViewSet();
					vs.Insert(sheet);
					viewSheetSetsList.Add(s_format, vs);
				}
			}

			using (TransactionGroup tg = new TransactionGroup(doc))
			{
				tg.Start("Sorting ViewSheets by format");
				using (Transaction tx1 = new Transaction(doc))
				{
					tx1.Start("Deleting existing ViewSheet Sets");
					List<ElementId> allViewSheetSetsIds = new FilteredElementCollector(doc).OfClass(typeof(ViewSheetSet)).ToElementIds().ToList();
					doc.Delete(allViewSheetSetsIds);
					tx1.Commit();
				}

				using (Transaction tx2 = new Transaction(doc))
				{
					tx2.Start("Creating new ViewSheet Sets");
					PrintManager pm = doc.PrintManager;
					pm.PrintRange = PrintRange.Select;
					ViewSheetSetting vss = pm.ViewSheetSetting;
					foreach (KeyValuePair<string, ViewSet> kvp in viewSheetSetsList)
					{
						vss.CurrentViewSheetSet.Views = kvp.Value;
						vss.SaveAs(kvp.Key);
					}
					tx2.Commit();
				}
				tg.Assimilate();
			}

			TaskDialog.Show("Отчет", "Обработка завершена!");
			return Result.Succeeded;
		}

		private string GetSheetFormat(string height, string width)
		{
			string result = "";
			switch (width + "x" + height)
			{
				case "1682x1189": result = "A0x2А"; break;
				case "1188x841": result = "A0А"; break;
				case "2520x1188": result = "A0x3А"; break;
				case "841x594": result = "A1А"; break;
				case "1782x841": result = "A1x3А"; break;
				case "2376x841": result = "A1x4А"; break;
				case "2970x841": result = "A1x5А"; break;
				case "594x420": result = "A2А"; break;
				case "1260x594": result = "A2x3А"; break;
				case "1680x594": result = "A2x4А"; break;
				case "2100x594": result = "A2x5А"; break;
				case "420x297": result = "A3А"; break;
				case "891x420": result = "A3x3А"; break;
				case "1188x420": result = "A3x4А"; break;
				case "1485x420": result = "A3x5А"; break;
				case "1782x420": result = "A3x6А"; break;
				case "2079x420": result = "A3x7А"; break;
				case "297x210": result = "A4А"; break;
				case "630x297": result = "A4x3А"; break;
				case "840x297": result = "A4x4А"; break;
				case "1050x297": result = "A4x5А"; break;
				case "1260x297": result = "A4x6А"; break;
				case "1188x1680": result = "A0x2K"; break;
				case "840x1188": result = "A0K"; break;
				case "1188x2520": result = "A0x3K"; break;
				case "594x841": result = "A1K"; break;
				case "840x1782": result = "A1x3K"; break;
				case "840x2376": result = "A1x4K"; break;
				case "840x2970": result = "A1x5K"; break;
				case "420x594": result = "A2K"; break;
				case "594x1260": result = "A2x3K"; break;
				case "594x1680": result = "A2x4K"; break;
				case "594x2100": result = "A2x5K"; break;
				case "297x420": result = "A3K"; break;
				case "420x891": result = "A3x3K"; break;
				case "420x1188": result = "A3x4K"; break;
				case "420x1485": result = "A3x5K"; break;
				case "420x1782": result = "A3x6K"; break;
				case "420x2079": result = "A3x7K"; break;
				case "210x297": result = "A4K"; break;
				case "297x630": result = "A4x3K"; break;
				case "297x840": result = "A4x4K"; break;
				case "297x1050": result = "A4x5K"; break;
				case "297x1260": result = "A4x6K"; break;
				default:
					result = "Unknown format";
					break;
			}
			return result;
		}
	}

}


