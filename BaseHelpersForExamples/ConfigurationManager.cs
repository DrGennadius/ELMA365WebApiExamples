using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseHelpersForExamples
{
    public class ConfigurationManager
    {
        public static string ConfigFilePath { get; set; }

        public static Configuration Load(string configFilePath)
        {
            ConfigFilePath = configFilePath;
            return Load();
        }

        public static Configuration Load()
        {
            Configuration configuration = null;
            if (string.IsNullOrEmpty(ConfigFilePath))
            {
                ConfigFilePath = Path.Combine(Environment.CurrentDirectory, "config.json");
            }
            if (File.Exists(ConfigFilePath))
            {
                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
                configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigFilePath), new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                });
            }
            else
            {
                configuration = new Configuration();
                Save(configuration);
            }
            return configuration;
        }

        public static void Save(Configuration configuration, string configFilePath)
        {
            ConfigFilePath = configFilePath;
            Save(configuration);
        }

        public static void Save(Configuration configuration)
        {
            if (configuration == null)
            {
                configuration = new Configuration();
            }
            if (string.IsNullOrEmpty(ConfigFilePath))
            {
                ConfigFilePath = Path.Combine(Environment.CurrentDirectory, "config.json");
            }
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            string json = JsonConvert.SerializeObject(configuration, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
            File.WriteAllText(ConfigFilePath, json);
        }
    }
}
