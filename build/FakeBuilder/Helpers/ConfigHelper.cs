using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBuilder.Helpers
{
    public static class ConfigHelper
    {
        private static ConfigParameters parameters;
        private static string path = "parameters.json";

        public static ConfigParameters Instance
        {
            get
            {
                if (parameters == null)
                {
                    if (!File.Exists(path))
                    {
                        parameters = new ConfigParameters();
                        JsonSerializer serializer = new JsonSerializer();
                        using (StreamWriter sw = new StreamWriter(path))
                        using (JsonWriter writer = new JsonTextWriter(sw))
                        {
                            serializer.Serialize(writer, parameters);
                        }

                        throw new Exception($"Empty config file, please fill: {path}, file");
                    }
                    else
                    {
                        using (StreamReader file = File.OpenText(path))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            parameters = (ConfigParameters)serializer.Deserialize(file, typeof(ConfigParameters));
                        }
                    }
                }

                return parameters;
            }
        }
    }
}
