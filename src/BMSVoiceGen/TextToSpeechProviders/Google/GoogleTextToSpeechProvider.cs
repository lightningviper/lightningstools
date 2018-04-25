using BMSVoiceGen.TextToSpeechProviders.Google.Protocol;
using Google.Apis.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace BMSVoiceGen.TextToSpeechProviders.Google
{
    internal class GoogleTextToSpeechProvider : ITextToSpeechProvider, IDisposable
    {
        internal const ushort GOOGLE_SYNTHESIZE_SPEECH_RATE_LIMIT_MAX_REQUESTS_PER_SECOND = 80;
        private const string SERVICE_URL_TEMPLATE = "https://texttospeech.googleapis.com/v1beta1/text:synthesize?fields=audioContent&key={0}";
        private const string OAUTH_SCOPE = "https://www.googleapis.com/auth/cloud-platform";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ConfigurableHttpClient _httpClient;
        public Settings Settings { get; set; } = new Settings();

        public GoogleTextToSpeechProvider(Settings settings, IHttpClientFactory httpClientFactory = null)
        {
            Settings = settings;
            _httpClientFactory = httpClientFactory ?? new HttpClientFactory();
            _httpClient = _httpClientFactory.CreateHttpClient(new CreateHttpClientArgs() { GZipEnabled = true });

        }
        public void GenerateWAV(string speakerText, ushort voice, string outputFileName)
        {
            var request = new SynthesizeRequest()
            {
                input = new SynthesisInput()
                {
                    ssml = $"<speak>{speakerText}</speak>"
                },
                voice = new VoiceSelectionParams()
                {
                    name = Settings.VoiceIds[voice],
                    languageCode=Settings.LanguageCode
                },
                audioConfig = new AudioConfig()
                {
                    audioEncoding = AudioEncoding.LINEAR16,
                    speakingRate = Settings.RateOfSpeech,
                    volumeGainDb = Settings.VolumeGainDb,
                    sampleRateHertz = Settings.SampleRateHz
                }
            };
            var requestJson = JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var requestPayload = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var serviceUrl = string.Format(SERVICE_URL_TEMPLATE, Settings.ApiKey);
            var response = _httpClient.PostAsync(serviceUrl, requestPayload).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseJson = response.Content.ReadAsStringAsync().Result;
                var responseObj = JsonConvert.DeserializeObject<SynthesizeResponse>(responseJson);
                using (var fs = new FileStream(outputFileName, FileMode.Create))
                {
                    var audioBytes = Convert.FromBase64String(responseObj.audioContent);
                    fs.Write(audioBytes, 0, audioBytes.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
            else
            {
                throw new Exception($"HTTP response status code:{response.StatusCode}; reason: {response.ReasonPhrase}");
            }
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
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
