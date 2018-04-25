namespace BMSVoiceGen.TextToSpeechProviders
{
    internal interface ITextToSpeechProvider
    {
        void GenerateWAV(string text, ushort voice, string outputFilePath);
    }
}
