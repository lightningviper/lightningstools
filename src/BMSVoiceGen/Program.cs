using BMSVoiceGen.IO.Yaml;
using BMSVoiceGen.TextToSpeechProviders;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;

namespace BMSVoiceGen
{
    public class Program
    {
        private static ILog Log = LogManager.GetLogger(typeof(Program));
        private static readonly IYamlFileReader<Settings> _settingsReader = new YamlFileReader<Settings>();
        private static readonly ITextToSpeechProviderFactory _textToSpeechProviderFactory = new TextToSpeechProviderFactory();
        private static readonly ITextToSpeechBatchConverter _textToSpeechBatchConverter = new TextToSpeechBatchConverter();

        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            var appSettings = _settingsReader.Read(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "conf"), "app-settings.yml"));
            var textToSpeechProviderType = appSettings.TextToSpeechProviderType;
            var textToSpeechProvider = _textToSpeechProviderFactory.CreateTextToSpeechProvider(textToSpeechProviderType);
            _textToSpeechBatchConverter.GenerateWAVs(appSettings.FragFile, appSettings.CsvFile, appSettings.OutputFolder, textToSpeechProvider, -1);
        }
    }
}

