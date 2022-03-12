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
    public class FSCRT4_Settings
    {
        public static FSCRT4_Settings GetSettings()
        {
            FSCRT4_Settings fscrt4_Settings = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "FSCRT4_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(FSCRT4_Settings));
                    fscrt4_Settings = xSer.Deserialize(fs) as FSCRT4_Settings;
                    fs.Close();
                }
            }
            else
            {
                fscrt4_Settings = new FSCRT4_Settings();
            }

            return fscrt4_Settings;
        }

        public void Save ()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "FSCRT4_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(FSCRT4_Settings));
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

        public string SecondLowerRebarOffsetSettings { get; set; }
        public string SecondTopRebarOffsetSettings { get; set; }
        public string SecondLeftRebarOffsetSettings { get; set; }
        public string SecondRightRebarOffsetSettings { get; set; }
    }
}
