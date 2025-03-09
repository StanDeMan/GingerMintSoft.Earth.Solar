using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace DataBase.Contract
{
    public class DynamicContractResolver(params string[] prop) : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) 
        {
            // return all the properties which are not in the ignore list
            return base
                .CreateProperties(type, memberSerialization)
                .Where(p => !prop.Contains(p.PropertyName))
                .ToList();
        }
    }
}
