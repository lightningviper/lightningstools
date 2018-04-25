using BMSVoiceGen.IO.Yaml;
using BMSVoiceGen.TextToSpeechProviders.AmazonPolly;
using System;

namespace BMSVoiceGen.TextToSpeechProviders
{
    internal interface ITextToSpeechProviderFactory
    {
        ITextToSpeechProvider CreateTextToSpeechProvider(TextToSpeechProviderType textToSpeechProviderType, string settingsFilePath);
    }
    internal class TextToSpeechProviderFactory:ITextToSpeechProviderFactory
    {
        private readonly IYamlFileReader<AmazonPolly.Settings> _amazonPollySettingsFileReader;
        public TextToSpeechProviderFactory(IYamlFileReader<AmazonPolly.Settings> amazonPollySettingsFileReader = null)
        {
            _amazonPollySettingsFileReader = amazonPollySettingsFileReader ?? new YamlFileReader<AmazonPolly.Settings>();
        }

        public ITextToSpeechProvider CreateTextToSpeechProvider(TextToSpeechProviderType textToSpeechProviderType, string settingsFilePath)
        {
            switch (textToSpeechProviderType)
            {
                case TextToSpeechProviderType.AmazonPolly:
                    var settings = _amazonPollySettingsFileReader.Read(settingsFilePath);
                    return new AmazonPollyTextToSpeechProvider(settings);

                default:
                    throw new ArgumentOutOfRangeException(nameof(textToSpeechProviderType));
            }
        }
    }
}
