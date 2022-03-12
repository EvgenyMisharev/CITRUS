using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    
    public partial class VoiceRecognitionForm : System.Windows.Forms.Form
    {
        SpeechRecognitionEngine spchEngine = new SpeechRecognitionEngine();
        SpeechSynthesizer speechSent = new SpeechSynthesizer();
        public VoiceRecognitionForm()
        {
            InitializeComponent();
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            spchEngine.RecognizeAsync(RecognizeMode.Multiple);
            speechSent.GetInstalledVoices();
            btn_Stop.Enabled = true;
            btn_Start.Enabled = false;
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            spchEngine.RecognizeAsyncStop();
            btn_Start.Enabled = true;
            btn_Stop.Enabled = false;
        }

        private void VoiceRecognition_Load(object sender, EventArgs e)
        {
            Choices commands = new Choices();
            commands.Add(new string[] { "привет ревит", "один", "у семена александровича день рождения", "тест" });
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(commands);
            Grammar grammar = new Grammar(grammarBuilder);
            spchEngine.LoadGrammarAsync(grammar);
            spchEngine.SetInputToDefaultAudioDevice();
            speechSent.SetOutputToDefaultAudioDevice();
            spchEngine.SpeechRecognized += SpchEngine_SpeechRecognized;
        }
        
        private void SpchEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            
            string spichResult = "";
            if (e.Result.Confidence > 0.7) spichResult = e.Result.Text;

            switch (spichResult)
            {
                case "один":
                    speechSent.SpeakAsync("один");
                    break;
                //case "привет ревит":
                //    speechSent.SpeakAsync("Привет! Чем могу помочь?");
                //    break;
                case "привет ревит":
                    speechSent.SpeakAsync("Блин! Что тебе опять нужно, кожаный ублюдок?");
                    break;
                case "у семена александровича день рождения":
                    speechSent.SpeakAsync("Оооооо! Это меняет дело! Дорогой, Семен Александровичь, поздравляю тебя с днем рождения!" +
                        "Желаю тебе здоровья, отличного настроения каждый день, профессиональных и творческих успехов, самореализации, всегда улыбающихся родных и близких!");
                    
                    DR dr1 = new DR();
                    UIApplication uiapp = sender as Autodesk.Revit.UI.UIApplication;
                    dr1.Execute(uiapp.Application);

                    speechSent.SpeakAsync("С Днем Рождения!");
                    break;
            }

        }
    }

    public partial class DR : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Result.Succeeded;
        }

        public Result Execute(UIApplication uiapp)
        {
            //Получение текущего документа
            Document doc = uiapp.ActiveUIDocument.Document;

            //Список семейств с именем IntersectionPoint
            List<Family> families = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().Where(f => f.Name == "210_Прямоугольного сечения (НесКол_2ур)").ToList();
            if (families.Count != 1) return Result.Failed;
            Family mainFam = families.First();

            //ID элемента IntersectionPoint
            List<ElementId> symbolsIds = mainFam.GetFamilySymbolIds().ToList();
            ElementId firstSymbolId = symbolsIds.First();

            //Тип элемента(FamilySymbol) IntersectionPoint
            FamilySymbol mySymbol = doc.GetElement(firstSymbolId) as FamilySymbol;
            if (mySymbol == null) return Result.Failed;

            //Уровень на активном виде
            Autodesk.Revit.DB.View active = doc.ActiveView;
            Level myLevel = active.GenLevel;
            if (myLevel == null) return Result.Failed;

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Размещение семейств");
                FamilyInstance fi = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), mySymbol, myLevel, StructuralType.NonStructural);






                t.Commit();
            }

            return Result.Succeeded;
        }

        internal void Execute(object application)
        {
            throw new NotImplementedException();
        }
    }
}
