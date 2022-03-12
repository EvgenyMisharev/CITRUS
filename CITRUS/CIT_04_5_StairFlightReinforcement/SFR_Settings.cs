using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CITRUS.CIT_04_5_StairFlightReinforcement
{
    public class SFR_Settings
    {
        public static SFR_Settings GetSettings()
        {
            SFR_Settings sfr_Settings = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SFR_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(SFR_Settings));
                    sfr_Settings = xSer.Deserialize(fs) as SFR_Settings;
                    fs.Close();
                }
            }
            else
            {
                sfr_Settings = new SFR_Settings();
            }

            return sfr_Settings;
        }

        public void Save ()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SFR_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(SFR_Settings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
        public string mySelectionStepRebarTypeSettings { get; set; }
        public string mySelectionStaircaseRebarTypeSettings { get; set; }

        public string StepRebarCoverLayerSettings { get; set; }
        public string StepLengthSettings { get; set; }
        public string StepHeightSettings { get; set; }
        public string StaircaseSlabThicknessSettings { get; set; }
        public string StairCoverLayerSettings { get; set; }
        public string StepRebarStepSettings { get; set; }
        public string StaircaseRebarStepSettings { get; set; }
        public string TopExtensionStaircaseSettings { get; set; }
        public string TopExtensionHeightStaircaseSettings { get; set; }
        public string BottomExtensionHeightStaircaseSettings { get; set; }
        public string BottomExtensionHeightStaircaseNodeA2Settings { get; set; }
        public string FirstBarMeshNameSettings { get; set; }
        public string AdditionalBarMeshName_1Settings { get; set; }
        public string AdditionalBarMeshName_2Settings { get; set; }
    }
}
