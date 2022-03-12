using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITRUS.VoiceRecognition
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class VoiceRecognition : IExternalCommand
    {
        public UIApplication uiAppVR;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            uiAppVR = uiApp;
            VoiceRecognitionForm voiceRecognitionForm = new VoiceRecognitionForm();
            voiceRecognitionForm.ShowDialog();

            return Result.Succeeded;
        }

    }
}
