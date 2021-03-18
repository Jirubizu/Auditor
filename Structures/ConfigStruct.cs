using Newtonsoft.Json;

namespace Auditor.Structures
{
    public class ConfigStruct
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        
        [JsonProperty("database_ip")]
        public string DatabaseIp { get; set; }
        
        [JsonProperty("database_port")]
        public int DatabasePort { get; set; }
    }
}