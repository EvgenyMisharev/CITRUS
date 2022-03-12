using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CITRUS.CIT_04_2_RectangularColumnsReinforcement
{
    public class RCRF_Settings
    {
        public static RCRF_Settings GetSettings()
        {
            RCRF_Settings rcrf_Settings = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "RCRF_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(RCRF_Settings));
                    rcrf_Settings = xSer.Deserialize(fs) as RCRF_Settings;
                    fs.Close();
                }
            }
            else
            {
                rcrf_Settings = new RCRF_Settings();
            }

            return rcrf_Settings;
        }

        public void Save ()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "RCRF_Settings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(RCRF_Settings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }

        public string mySelectionMainBarTapeOneSettings { get; set; }
        public string mySelectionMainBarTapeTwoSettings { get; set; }
        public string mySelectionMainBarTapeThreeSettings { get; set; }
        public string mySelectionStirrupBarTapeSettings { get; set; }
        public string mySelectionPinBarTapeSettings { get; set; }
        public string mySelectionRebarCoverTypeSettings { get; set; }

        public string NumberOfBarsLRFacesSettings { get; set; }
        public string NumberOfBarsTBFacesSettings { get; set; }
        public string RebarOutletsLengthLongSettings { get; set; }
        public string RebarOutletsLengthShortSettings { get; set; }
        public string FloorThicknessAboveColumnSettings { get; set; }
        public string StandardStirrupStepSettings { get; set; }
        public string IncreasedStirrupStepSettings { get; set; }
        public string FirstStirrupOffsetSettings { get; set; }
        public string StirrupIncreasedPlacementHeightSettings { get; set; }
        public string ColumnSectionOffsetSettings { get; set; }
        public string DeepeningBarsSizeSettings { get; set; }
    }
}
