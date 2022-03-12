using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CITRUS
{
    public class GloryHoleSettings
    {
        public double PipeSideClearance { get; set; }
        public double PipeTopBottomClearance { get; set; }
        public double DuctSideClearance { get; set; }
        public double DuctTopBottomClearance { get; set; }
        public double RoundUpIncrement{ get; set; }

    public static GloryHoleSettings GetSettings()
        {
            GloryHoleSettings gloryHoleSettings = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "GloryHoleSettings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(GloryHoleSettings));
                    gloryHoleSettings = xSer.Deserialize(fs) as GloryHoleSettings;
                    fs.Close();
                }
            }
            else
            {
                gloryHoleSettings = new GloryHoleSettings();
            }

            return gloryHoleSettings;
        }
        public void SaveSettings()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "GloryHoleSettings.xml";
            string assemblyPath = assemblyPathAll.Replace("CITRUS.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(GloryHoleSettings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
    }
}
