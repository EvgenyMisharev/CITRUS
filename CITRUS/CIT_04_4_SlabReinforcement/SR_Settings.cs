using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CITRUS.CIT_04_4_SlabReinforcement
{
    public class SR_Settings
    {
        public static SR_Settings GetSettings()
        {
            SR_Settings sr_Settings = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SR_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(SR_Settings));
                    sr_Settings = xSer.Deserialize(fs) as SR_Settings;
                    fs.Close();
                }
            }
            else
            {
                sr_Settings = new SR_Settings();
            }

            return sr_Settings;
        }

        public void Save ()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SR_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(SR_Settings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
        public string mySelectionBottomXDirectionRebarTapeSettings { get; set; }
        public string mySelectionBottomYDirectionRebarTapeSettings { get; set; }
        public string mySelectionTopXDirectionRebarTapeSettings { get; set; }
        public string mySelectionTopYDirectionRebarTapeSettings { get; set; }

        public string mySelectionRebarCoverTypeForTopSettings { get; set; }
        public string mySelectionRebarCoverTypeForBottomSettings { get; set; }

        public string BottomXDirectionRebarSpacingSettings { get; set; }
        public string BottomYDirectionRebarSpacingSettings { get; set; }
        public string TopXDirectionRebarSpacingSettings { get; set; }
        public string TopYDirectionRebarSpacingSettings { get; set; }

        public string PerimeterFramingDiamSettings { get; set; }
        public string PerimeterFramingOverlapingSettings { get; set; }
        public string PerimeterFramingEndCoverLayerSettings { get; set; }
        public string PerimeterFramingStepSettings { get; set; }
    }
}
