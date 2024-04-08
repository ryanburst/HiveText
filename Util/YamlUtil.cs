using System.IO;
using System.Reflection;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace HiveText.Util
{
    public static class YamlUtil
    {
        internal static void WriteConfigFileFromResource(string configFilePath)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "HiveText.Example.yml";

            using Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream == null)
            {
                throw new FileNotFoundException($"Resource '{resourceName}' not found in the assembly.");
            }

            using StreamReader reader = new StreamReader(resourceStream);
            string contents = reader.ReadToEnd();

            File.WriteAllText(configFilePath, contents);
        }

        internal static void ReadYaml(string yamlInput)
        {
            IDeserializer? deserializer = new DeserializerBuilder().Build();
            HiveTextPlugin.yamlData = deserializer.Deserialize<Dictionary<string, List<string>>>(yamlInput);
        }

       
        public static void WriteYaml(string filePath)
        {
            ISerializer? serializer = new SerializerBuilder().Build();
            using StreamWriter? output = new StreamWriter(filePath);
            serializer.Serialize(output, HiveTextPlugin.yamlData);

            // Serialize the data again to YAML format
            string serializedData = serializer.Serialize(HiveTextPlugin.yamlData);

            // Append the serialized YAML data to the file
            File.AppendAllText(filePath, serializedData);
        }
    }
}

