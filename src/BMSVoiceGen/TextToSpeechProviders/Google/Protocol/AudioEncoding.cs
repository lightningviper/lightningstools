using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BMSVoiceGen.TextToSpeechProviders.Google.Protocol
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum AudioEncoding
    {
        AUDIO_ENCODING_UNSPECIFIED,
        LINEAR16,
        MP3,
        OGG_OPUS
    }
}
