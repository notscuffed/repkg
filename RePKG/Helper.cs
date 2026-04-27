using System.Text.Json.Nodes;

namespace RePKG
{
    public static class Helper
    {
        public static IEnumerable<string> GetPropertyKeysForDynamic(JsonObject jsonObject)
        {
            return jsonObject.Select(x => x.Key);
        }
    }
}