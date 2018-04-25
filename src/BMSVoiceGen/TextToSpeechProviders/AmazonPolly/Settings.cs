using YamlDotNet.Serialization;

namespace BMSVoiceGen.TextToSpeechProviders.AmazonPolly
{
    internal class Settings
    {
        private const int DEFAULT_SAMPLE_RATE = 8000;
        private const int DEFAULT_MAX_ERROR_RETRIES = -1;

        [YamlMember(Alias = "aws-access-key-id")]
        public string AwsAccessKeyId { get; set; }

        [YamlMember(Alias = "aws-secret-access-key")]
        public string AwsSecretAccessKey { get; set; }

        [YamlMember(Alias = "aws-region")]
        public string AwsRegion { get; set; }

        [YamlMember(Alias = "max-error-retries")]
        public int MaxErrorRetries { get; set; } = DEFAULT_MAX_ERROR_RETRIES;

        [YamlMember(Alias = "throttle-retries")]
        public bool ThrottleRetries { get; set; } = false;

        [YamlMember(Alias = "use-nagle-algorithm")]
        public bool UseNagleAlgorithm { get; set; } = false;

        [YamlMember(Alias = "voice-ids")]
        public string[] VoiceIds { get; set; } = DefaultVoiceIdSet;

        [YamlMember(Alias = "rate-of-speech")]
        public Rate RateOfSpeech { get; set; } = Rate.Medium;

        [YamlMember(Alias = "speech-volume")]
        public Volume SpeechVolume { get; set; } = Volume.X_Loud;

        [YamlMember(Alias = "use-dynamic-range-compression")]
        public bool UseDynamicRangeCompression { get; set; } = true;

        [YamlMember(Alias = "use-auto-breathing")]
        public bool UseAutoBreathing { get; set; } = false;

        [YamlMember(Alias = "breath-volume")]
        public Volume BreathVolume { get; set; } = Volume.Default;

        [YamlMember(Alias = "breath-frequency")]
        public Frequency BreathFrequency { get; set; } = Frequency.Default;

        [YamlMember(Alias = "breath-duration")]
        public Duration BreathDuration { get; set; } = Duration.Default;

        [YamlMember(Alias = "sample-rate")]
        public uint SampleRate { get; set; } = DEFAULT_SAMPLE_RATE;

        [YamlIgnore]
        private static string[] DefaultVoiceIdSet
        {
            get
            {
                return new[]
                {
                    "Matthew", //speaker 0, USA male
                    "Joey", //speaker 1, USA male
                    "Russell", //speaker 2, Australian male
                    "Brian", //speaker 3, British male
                    "Joanna", //speaker 4,  USA female
                    "Geraint", //speaker 5, Welsh male
                    "Nicole", //speaker 6, Australian female
                    "Matthew", //speaker 7, USA male
                    "Joey", //speaker 8, USA male
                    "Russell", //speaker 9, Australian male
                    "Brian", //speaker 10, British male
                    "Geraint", //speaker 11, Welsh male
                    "Amy", //speaker 12, British female
                    "Brian", //speaker 13, British male
                    "Russell", //speaker 14, Australian male
                };
            }
        }
    }
}
