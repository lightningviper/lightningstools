using BMSVoiceGen.IO.Yaml;
using BMSVoiceGen.TextToSpeechProviders.AmazonPolly;
using BMSVoiceGen.TextToSpeechProviders.Google;
using System;
using System.IO;
using System.Reflection;

namespace BMSVoiceGen.TextToSpeechProviders
{
    internal interface ITextToSpeechProviderFactory
    {
        ITextToSpeechProvider CreateTextToSpeechProvider(TextToSpeechProviderType textToSpeechProviderType);
    }
    internal class TextToSpeechProviderFactory:ITextToSpeechProviderFactory
    {
        private readonly IYamlFileReader<AmazonPolly.Settings> _amazonPollySettingsFileReader;
        private readonly IYamlFileReader<Google.Settings> _googleSettingsFileReader;

        public TextToSpeechProviderFactory(
            IYamlFileReader<AmazonPolly.Settings> amazonPollySettingsFileReader = null, 
            IYamlFileReader<Google.Settings> googleSettingsFileReader = null)
        {
            _amazonPollySettingsFileReader = amazonPollySettingsFileReader ?? new YamlFileReader<AmazonPolly.Settings>();
            _googleSettingsFileReader = googleSettingsFileReader ?? new YamlFileReader<Google.Settings>();
        }

        public ITextToSpeechProvider CreateTextToSpeechProvider(TextToSpeechProviderType textToSpeechProviderType)
        {
            switch (textToSpeechProviderType)
            {
                case TextToSpeechProviderType.AmazonPolly:
                    {
                        var settings = _amazonPollySettingsFileReader.Read(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "conf"), "amazon-polly-settings.yml"));
                        return new AmazonPollyTextToSpeechProvider(settings);
                    }
                case TextToSpeechProviderType.GoogleCloudTextToSpeech:
                    {
                        var settings = _googleSettingsFileReader.Read(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "conf"), "google-tts-settings.yml"));
                        return new GoogleTextToSpeechProvider(settings);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(textToSpeechProviderType));
            }
        }
    }
}
