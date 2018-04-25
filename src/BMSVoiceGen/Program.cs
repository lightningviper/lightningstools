using BMSVoiceGen.IO.Yaml;
using BMSVoiceGen.TextToSpeechProviders;
using log4net;
using log4net.Config;
using System.IO;

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
            var appSettings = _settingsReader.Read(args[0]);
            var textToSpeechProviderType = appSettings.TextToSpeechProviderType;
            var textToSpeechProvider = _textToSpeechProviderFactory.CreateTextToSpeechProvider(textToSpeechProviderType, args[1]);
            _textToSpeechBatchConverter.GenerateWAVs(appSettings.FragFile, appSettings.CsvFile, appSettings.OutputFolder, textToSpeechProvider, -1);
        }
        private static void ValidateArguments(string[] args)
        {
            if (args == null || args.Length !=2 || !File.Exists(args[0]) || !File.Exists(args[1]))
            {
                Log.Error("invalid command line arguments.");
            }
        }
    }
}

