using YamlDotNet.Serialization;

namespace BMSVoiceGen.TextToSpeechProviders.Google
{
    internal class Settings
    {
        [YamlMember(Alias = "api-key")]
        public string ApiKey { get; set; }

        [YamlMember(Alias = "voice-ids")]
        public string[] VoiceIds { get; set; } = DefaultVoiceSet;

        [YamlMember(Alias = "language-code")]
        public string LanguageCode { get; set; } = "en-us";

        [YamlMember(Alias = "rate-of-speech")]
        public float RateOfSpeech { get; set; } = 1.0f;

        [YamlMember(Alias = "volume-gain-db")]
        public float VolumeGainDb { get; set; } = +10.0f;

        [YamlMember(Alias = "sample-rate-hz")]
        public uint SampleRateHz { get; set; } = 8000;

        [YamlIgnore]
        private static string[] DefaultVoiceSet
        {
            get
            {
                return new[]
                {
                    "en-US-Wavenet-A", //speaker 0, USA male
                    "en-US-Wavenet-B", //speaker 1, USA male
                    "en-AU-Standard-B", //speaker 2, Australian male
                    "en-GB-Standard-B", //speaker 3, British male
                    "en-US-Wavenet-C", //speaker 4,  USA female
                    "en-US-Standard-B", //speaker 5, USA male
                    "en-US-Wavenet-E", //speaker 6, USA female
                    "en-US-Wavenet-D", //speaker 7, USA male
                    "en-US-Standard-D", //speaker 8, USA male
                    "en-GB-Standard-D", //speaker 9, British male
                    "en-GB-Standard-D", //speaker 10, British male
                    "en-GB-Standard-B", //speaker 11, British male
                    "en-US-Wavenet-E", //speaker 12, USA female
                    "en-GB-Standard-B", //speaker 13, British male
                    "en-AU-Standard-D", //speaker 14, Australian male
                };
            }
        }
    }
}
