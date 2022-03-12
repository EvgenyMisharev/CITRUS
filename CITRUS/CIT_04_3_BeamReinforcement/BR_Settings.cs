using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CITRUS.CIT_04_3_BeamReinforcement
{
    public class BR_Settings
    {
        public static BR_Settings GetSettings()
        {
            BR_Settings br_Settings = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "BR_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(BR_Settings));
                    br_Settings = xSer.Deserialize(fs) as BR_Settings;
                    fs.Close();
                }
            }
            else
            {
                br_Settings = new BR_Settings();
            }

            return br_Settings;
        }

        public void Save ()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "BR_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(BR_Settings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
        public string mySelectionMainBarT1Settings { get; set; }
        public string mySelectionMainBarT2Settings { get; set; }
        public string mySelectionMainBarT3Settings { get; set; }
        public string mySelectionStirrupT1Settings { get; set; }
        public string mySelectionStirrupC1Settings { get; set; }

        public string RebarTopCoverLayerSettings { get; set; }
        public string RebarBottomCoverLayerSettings { get; set; }
        public string RebarLRCoverLayerSettings { get; set; }

        public string ExtensionLeftLenghtL1Settings { get; set; }
        public string ExtensionLeftLenghtL2Settings { get; set; }
        public string ExtensionRightLenghtR1Settings { get; set; }
        public string ExtensionRightLenghtR2Settings { get; set; }

        public string DeepeningIntoTheStructureL1Settings { get; set; }
        public string DeepeningIntoTheStructureL2Settings { get; set; }
        public string DeepeningIntoTheStructureR1Settings { get; set; }
        public string DeepeningIntoTheStructureR2Settings { get; set; }

        public string StirrupIndentL1Settings { get; set; }
        public string StirrupStepL1Settings { get; set; }
        public string StirrupIndentR1Settings { get; set; }
        public string StirrupStepR1Settings { get; set; }
        public string StirrupStepC1Settings { get; set; }

        public string ExtensionAddBarL2Settings { get; set; }
        public string ExtensionAddBarR2Settings { get; set; }

        public string NumberOfBarsTopFacesSettings { get; set; }
        public string NumberOfBarsBottomFacesSettings { get; set; }
    }
}
