using Newtonsoft.Json;

namespace Auditor.Structures
{
    public class ConfigStruct
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}