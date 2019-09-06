using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace RePKG
{
    public static class Helper
    {
        public static IEnumerable<string> GetPropertyKeysForDynamic(dynamic dynamicToGetPropertiesFor)
        {
            JObject attributesAsJObject = dynamicToGetPropertiesFor;
            var values = attributesAsJObject.ToObject<Dictionary<string, object>>();
            var toReturn = new List<string>();
            foreach (var key in values.Keys)
            {
                toReturn.Add(key);
            }

            return toReturn;
        }
    }
}