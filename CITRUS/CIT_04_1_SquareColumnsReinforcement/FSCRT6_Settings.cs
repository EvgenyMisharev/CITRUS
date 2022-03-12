using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CITRUS.CIT_04_1_SquareColumnsReinforcement
{
    public class FSCRT6_Settings
    {
        public static FSCRT6_Settings GetSettings()
        {
            FSCRT6_Settings fscrt6_Settings = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "FSCRT6_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(FSCRT6_Settings));
                    fscrt6_Settings = xSer.Deserialize(fs) as FSCRT6_Settings;
                    fs.Close();
                }
            }
            else
            {
                fscrt6_Settings = new FSCRT6_Settings();
            }

            return fscrt6_Settings;
        }

        public void Save ()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "FSCRT6_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(FSCRT6_Settings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }

        public string mySelectionFirstMainBarTapeSettings { get; set; }
        public string mySelectionSecondMainBarTapeSettings { get; set; }
        public string mySelectionFirstStirrupBarTapeSettings { get; set; }
        public string mySelectionSecondStirrupBarTapeSettings { get; set; }
        public string mySelectionRebarCoverTypeSettings { get; set; }

        public string FloorThicknessSettings { get; set; }
        public string RebarOutletsSettings { get; set; }
        public string RebarSecondOutletsSettings { get; set; }
        public string FirstStirrupOffsetSettings { get; set; }
        public string IncreasedStirrupSpacingSettings { get; set; }
        public string StandardStirrupSpacingSettings { get; set; }
        public string StirrupIncreasedPlacementHeightSettings { get; set; }
        public string ColumnSectionOffsetSettings { get; set; }
        public string DeepeningBarsSizeSettings { get; set; }

        public string SecondLowerRebarOffset1Settings { get; set; }
        public string SecondTopRebarOffset1Settings { get; set; }
        public string SecondLeftRebarOffset1Settings { get; set; }
        public string SecondRightRebarOffset1Settings { get; set; }

        public string SecondLowerRebarOffset2Settings { get; set; }
        public string SecondTopRebarOffset2Settings { get; set; }
        public string SecondLeftRebarOffset2Settings { get; set; }
        public string SecondRightRebarOffset2Settings { get; set; }
    }
}
