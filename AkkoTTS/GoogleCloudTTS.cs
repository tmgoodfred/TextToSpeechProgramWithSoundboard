using Google.Cloud.TextToSpeech.V1;
using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;

namespace AkkoTTS
{
    internal class GoogleCloudTTS
    {
        private readonly IConfigurationRoot _configuration;
        private readonly TextToSpeechClient _client;
        private readonly string _tempDirectory;

        public GoogleCloudTTS(IConfigurationRoot configuration)
        {
            _configuration = configuration;
            _tempDirectory = Path.Combine(Path.GetTempPath(), "AkkoTTS");

            if (!Directory.Exists(_tempDirectory))
            {
                Directory.CreateDirectory(_tempDirectory);
            }

            var credentials = GetCredentials();

            _client = new TextToSpeechClientBuilder
            {
                ChannelCredentials = credentials.ToChannelCredentials()
            }.Build();
        }

        private GoogleCredential GetCredentials()
        {
            var credentialsSection = _configuration.GetSection("GoogleCloud:Credentials");

            string? credentialsJson = null;

            try
            {
                var credentialsDict = new Dictionary<string, object>();
                credentialsSection.Bind(credentialsDict);

                if (credentialsDict.Count > 0)
                {
                    credentialsJson = Newtonsoft.Json.JsonConvert.SerializeObject(credentialsDict);
                }
            }
            catch
            {
                // If binding fails, try another approach
            }

            if (string.IsNullOrEmpty(credentialsJson))
            {
                credentialsJson = credentialsSection.Value;
            }

            if (string.IsNullOrEmpty(credentialsJson))
            {
                throw new InvalidOperationException("Google Cloud credentials not found in configuration.");
            }

            return GoogleCredential.FromJson(credentialsJson)
                .CreateScoped(TextToSpeechClient.DefaultScopes);
        }

        public async Task<List<Voice>> GetChirp3EnUsVoicesAsync()
        {
            var response = await _client.ListVoicesAsync(new ListVoicesRequest { LanguageCode = "en-US" });

            return response.Voices
                .Where(v => v.Name.Contains("chirp") && v.LanguageCodes.Contains("en-US"))
                .ToList();
        }

        public async Task<string> SynthesizeSpeechAsync(string text, string voiceName)
        {

            var input = new SynthesisInput { Text = text };

            var voice = new VoiceSelectionParams
            {
                LanguageCode = "en-US",
                Name = voiceName
            };

            var audioConfig = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };

            var response = await _client.SynthesizeSpeechAsync(
                new SynthesizeSpeechRequest
                {
                    Input = input,
                    Voice = voice,
                    AudioConfig = audioConfig
                });

            string tempFilePath = Path.Combine(_tempDirectory, $"{Guid.NewGuid()}.mp3");
            File.WriteAllBytes(tempFilePath, response.AudioContent.ToByteArray());

            return tempFilePath;
        }

        public void PlayAudio(string audioFilePath)
        {
            using (var audioFile = new AudioFileReader(audioFilePath))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }

            try
            {
                File.Delete(audioFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting temporary file: {ex.Message}");
            }
        }

        public async Task SpeakTextAsync(string text, string voiceName)
        {
            try
            {
                string audioFilePath = await SynthesizeSpeechAsync(text, voiceName);
                PlayAudio(audioFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in text-to-speech: {ex.Message}");
                throw;
            }
        }

        public void Cleanup()
        {
            try
            {
                if (Directory.Exists(_tempDirectory))
                {
                    foreach (var file in Directory.GetFiles(_tempDirectory))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning up temporary files: {ex.Message}");
            }
        }
    }
}