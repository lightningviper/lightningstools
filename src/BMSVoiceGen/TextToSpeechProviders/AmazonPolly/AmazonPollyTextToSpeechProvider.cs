using Amazon.Polly;
using Amazon.Polly.Model;
using BMSVoiceGen.Audio.IO;
using System;

namespace BMSVoiceGen.TextToSpeechProviders.AmazonPolly
{
    internal class AmazonPollyTextToSpeechProvider : ITextToSpeechProvider, IDisposable
    {
        internal const ushort AWS_POLLY_SYNTHESIZE_SPEECH_RATE_LIMIT_MAX_REQUESTS_PER_SECOND = 80;
        private readonly IAmazonPolly _awsPollyClient;
        public Settings Settings { get; set; } = new Settings();

        public AmazonPollyTextToSpeechProvider(Settings settings, IAmazonPolly awsPollyClient= null)
        {
            Settings = settings;
            _awsPollyClient =  awsPollyClient ?? 
                new AmazonPollyClient(
                    settings.AwsAccessKeyId, 
                    settings.AwsSecretAccessKey,
                    new AmazonPollyConfig()
                    {
                        RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(settings.AwsRegion),
                        ThrottleRetries = settings.ThrottleRetries,
                        MaxErrorRetry = (int)settings.MaxErrorRetries, 
                        UseNagleAlgorithm = settings.UseNagleAlgorithm
                    }
            );
        }
        public void GenerateWAV(string speakerText, ushort voice, string outputFileName)
        {
            var request = new SynthesizeSpeechRequest()
            {
                OutputFormat = OutputFormat.Pcm,
                SampleRate = Settings.SampleRate.ToString(),
                Text = GetSSMLForSpeakerText(speakerText),
                TextType = TextType.Ssml,
                VoiceId = Settings.VoiceIds[voice]
            };
            var response = _awsPollyClient.SynthesizeSpeech(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                new PCMAudioSerializer().Serialize(response.AudioStream, outputFileName);
            }
        }

        private string GetSSMLForSpeakerText(string speakerText)
        {
            return   $"<speak>" +
                (Settings.UseDynamicRangeCompression ? "<amazon:effect name=\"drc\">" : "") +
                    (Settings.UseAutoBreathing ? $"<amazon:auto-breaths " +
                                            $"volume=\"{Enum.GetName(typeof(Volume), Settings.BreathVolume).ToLowerInvariant().Replace('_', '-')}\" " +
                                            $"frequency=\"{Enum.GetName(typeof(Frequency), Settings.BreathFrequency).ToLowerInvariant().Replace('_', '-')}\" " +
                                            $"duration=\"{Enum.GetName(typeof(Duration), Settings.BreathDuration).ToLowerInvariant().Replace('_', '-')}" 
                                        + $"\">" : "") +
                                            $"<prosody " +
                                                $"volume=\"{Enum.GetName(typeof(Volume), Settings.SpeechVolume).ToLowerInvariant().Replace('_', '-')}\" " +
                                                $"rate=\"{Enum.GetName(typeof(Rate), Settings.RateOfSpeech).ToLowerInvariant().Replace('_', '-')}" + 
                                            $"\">" +
                                                 $"{speakerText}" +
                                         $"</prosody>" +
                    (Settings.UseAutoBreathing ? $"</amazon:auto-breaths>" : "") +
            (Settings.UseDynamicRangeCompression ? "</amazon:effect>" : "") +
                $"</speak>";
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _awsPollyClient.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
