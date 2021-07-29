using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BMSVoiceGen.IO.Yaml
{
    internal interface IYamlFileReader<T>
    {
        T Read(string filePath);
    }
    internal class YamlFileReader<T>:IYamlFileReader<T>
    {
        public T Read(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            using (var input = new StreamReader(fs))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(HyphenatedNamingConvention.Instance)
                    .Build();
                return deserializer.Deserialize<T>(input);
            }
        }
    }
}
