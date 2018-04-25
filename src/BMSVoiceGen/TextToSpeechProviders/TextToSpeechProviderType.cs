using YamlDotNet.Serialization;

namespace BMSVoiceGen
{
    public enum TextToSpeechProviderType
    {
        AmazonPolly = 0,
        GoogleCloudTextToSpeech=1
    }
}
