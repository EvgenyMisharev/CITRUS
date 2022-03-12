using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.CIT_04_1_SquareColumnsReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_04_1_SquareColumnsReinforcement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            SquareСolumnTypeSelectionForm squareСolumnTypeSelectionForm = new SquareСolumnTypeSelectionForm();
            squareСolumnTypeSelectionForm.ShowDialog();
            string checkedButtonNameResult= "";
            if (squareСolumnTypeSelectionForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }
            else if (squareСolumnTypeSelectionForm.DialogResult == System.Windows.Forms.DialogResult.OK )
            {
                checkedButtonNameResult = squareСolumnTypeSelectionForm.checkedButtonName;
            }

            if (checkedButtonNameResult == "radioButton_Type1")
            {
                CIT_04_1_1SquareColumnsReinforcementType1 type1 = new CIT_04_1_1SquareColumnsReinforcementType1();
                type1.Execute(commandData.Application);
            }

            else if (checkedButtonNameResult == "radioButton_Type2")
            {
                CIT_04_1_1SquareColumnsReinforcementType2 type2 = new CIT_04_1_1SquareColumnsReinforcementType2();
                type2.Execute(commandData.Application);
            }

            else if (checkedButtonNameResult == "radioButton_Type3")
            {
                CIT_04_1_1SquareColumnsReinforcementType3 type3 = new CIT_04_1_1SquareColumnsReinforcementType3();
                type3.Execute(commandData.Application);
            }

            else if (checkedButtonNameResult == "radioButton_Type4")
            {
                CIT_04_1_1SquareColumnsReinforcementType4 type4 = new CIT_04_1_1SquareColumnsReinforcementType4();
                type4.Execute(commandData.Application);
            }

            else if (checkedButtonNameResult == "radioButton_Type5")
            {
                CIT_04_1_1SquareColumnsReinforcementType5 type5 = new CIT_04_1_1SquareColumnsReinforcementType5();
                type5.Execute(commandData.Application);
            }

            else if (checkedButtonNameResult == "radioButton_Type6")
            {
                CIT_04_1_1SquareColumnsReinforcementType6 type6 = new CIT_04_1_1SquareColumnsReinforcementType6();
                type6.Execute(commandData.Application);
            }

            return Result.Succeeded;
        }

    }
}
