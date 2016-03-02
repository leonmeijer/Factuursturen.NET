using Newtonsoft.Json.Serialization;

namespace LVMS.FactuurSturen
{
    public class LowerCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}