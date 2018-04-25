using System.IO;
using YamlDotNet.Serialization;

namespace BMSVoiceGen
{
    internal class Settings 
    {
        [YamlMember(Alias = "bms-folder")]
        public string BmsFolder { get; set; }

        [YamlMember(Alias = "output-folder")]
        public string OutputFolder { get; set; }

        [YamlMember(Alias = "tts-provider")]
        public TextToSpeechProviderType TextToSpeechProviderType { get; set; }

        [YamlIgnore()]
        public string CsvFile { get { return Path.Combine(BmsFolder ?? "", @"Data\Sounds\F4Talk95v1-0-0.csv"); } }

        [YamlIgnore()]
        public string FragFile { get { return Path.Combine(BmsFolder ?? "", @"Data\Sounds\fragfile.bin");  } }

    }
}
