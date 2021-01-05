using System.IO;
using Auditor.Structures;
using Newtonsoft.Json;

namespace Auditor.Services
{
    public class ConfigService
    {
        public ConfigStruct Config { get; }

        public ConfigService(string path)
        {
            Config = JsonConvert.DeserializeObject<ConfigStruct>(File.ReadAllText(path));
        }
        
    }
}