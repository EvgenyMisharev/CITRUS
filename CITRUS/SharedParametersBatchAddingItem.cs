using Autodesk.Revit.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CITRUS
{
    public class SharedParametersBatchAddingItem
    {
        [JsonIgnore]
        public ExternalDefinition ExternalDefinitionParam { get; set; }
        public Guid ExternalDefinitionParamGuid { get; set; }
        public bool AddParameterSelectedOptionParam { get; set; }
        public KeyValuePair<string, BuiltInParameterGroup> BuiltInParameterGroupParam { get; set; }
        public string FormulaParam { get; set; }

    }
}
