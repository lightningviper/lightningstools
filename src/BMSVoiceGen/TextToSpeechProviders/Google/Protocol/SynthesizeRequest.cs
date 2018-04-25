namespace BMSVoiceGen.TextToSpeechProviders.Google.Protocol
{
    internal class SynthesizeRequest
    {
        public SynthesisInput input { get; set; }
        public VoiceSelectionParams voice { get; set; }
        public AudioConfig audioConfig { get; set; }
    }
}
