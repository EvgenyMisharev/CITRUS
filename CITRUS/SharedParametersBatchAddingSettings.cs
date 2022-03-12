using Autodesk.Revit.DB;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace CITRUS
{
    public class SharedParametersBatchAddingSettings
    {
        public void Save(ObservableCollection<SharedParametersBatchAddingItem> sharedParametersBatchAddingItems, string jsonFilePath)
        {
            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
            }

            using (FileStream fs = new FileStream(jsonFilePath, FileMode.Create))
            {
                fs.Close();
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(jsonFilePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, sharedParametersBatchAddingItems);
            }
        }

        public ObservableCollection<SharedParametersBatchAddingItem> GetSettings(DefinitionGroups sharedParametersGroups, string jsonFilePath)
        {
            ObservableCollection<SharedParametersBatchAddingItem> sharedParametersBatchAddingItemsTmp = new ObservableCollection<SharedParametersBatchAddingItem>();
            if (File.Exists(jsonFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader sr = new StreamReader(jsonFilePath))
                using (JsonTextReader reader = new JsonTextReader(sr))
                {
                    ObservableCollection<SharedParametersBatchAddingItem> tmp = (ObservableCollection<SharedParametersBatchAddingItem>)serializer
                        .Deserialize(reader, typeof(ObservableCollection<SharedParametersBatchAddingItem>));
                    if(tmp != null)
                    {
                        foreach (SharedParametersBatchAddingItem itm in tmp)
                        {
                            foreach (DefinitionGroup spg in sharedParametersGroups)
                            {
                                foreach (ExternalDefinition d in spg.Definitions)
                                {
                                    if (itm.ExternalDefinitionParamGuid == d.GUID)
                                    {
                                        itm.ExternalDefinitionParam = d;
                                        break;
                                    }
                                }
                                if (itm.ExternalDefinitionParam != null) break;
                            }
                        }
                        sharedParametersBatchAddingItemsTmp = tmp;
                    }
                }
            }
            else
            {
                sharedParametersBatchAddingItemsTmp = new ObservableCollection<SharedParametersBatchAddingItem>();
            }
            return sharedParametersBatchAddingItemsTmp;
        }
    }
}
