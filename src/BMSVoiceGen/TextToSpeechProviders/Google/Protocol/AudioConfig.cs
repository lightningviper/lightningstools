namespace BMSVoiceGen.TextToSpeechProviders.Google.Protocol
{
    internal class AudioConfig
    {
        public AudioEncoding audioEncoding { get; set; }
        public float speakingRate { get; set; }
        public float pitch { get; set; }
        public float volumeGainDb { get; set; }
        public float sampleRateHertz { get; set; }
    }
}
