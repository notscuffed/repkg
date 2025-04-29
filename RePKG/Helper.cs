using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
namespace RePKG
{
    public static class Helper
    {
        public static IEnumerable<string> GetPropertyKeysForDynamic(dynamic dynamicToGetPropertiesFor)
        {
            var jsonString = JsonSerializer.Serialize(dynamicToGetPropertiesFor);
            using JsonDocument jsonDoc = JsonDocument.Parse(jsonString);
            return jsonDoc.RootElement.EnumerateObject().Select(p => p.Name).ToList();
        }
    }
}