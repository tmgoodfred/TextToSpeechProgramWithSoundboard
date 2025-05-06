using Google.Cloud.TextToSpeech.V1;
using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AkkoTTS
{
    internal class GoogleCloudTTS : IDisposable
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

            try
            {
                var credentials = GetCredentials();

                _client = new TextToSpeechClientBuilder
                {
                    ChannelCredentials = credentials.ToChannelCredentials()
                }.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing Google Cloud TTS: {ex.Message}");
                // Create a null client - we'll check for this before using it
                _client = null;
            }
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

        public void ListAvailableVoices(Action<string> addVoiceToComboBox)
        {
            if (_client == null)
                return; // Skip if client initialization failed

            try
            {
                var response = _client.ListVoices(new ListVoicesRequest());

                foreach (var voice in response.Voices)
                {
                    // Only add Chirp3-HD voices
                    if (voice.LanguageCodes.Contains("en-US") && voice.Name.Contains("Chirp3-HD"))
                    {
                        string voiceName = $"Google: {voice.Name}";
                        addVoiceToComboBox(voiceName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Google Cloud voices: {ex.Message}");
                // Don't throw - just silently fail to add voices
            }
        }

        public async Task<string> SynthesizeSpeechAsync(string text, string voiceName)
        {
            if (_client == null)
                throw new InvalidOperationException("Google Cloud TTS client is not initialized.");

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty", nameof(text));

            // Extract the actual voice name from the format "Google: voice-name"
            string actualVoiceName = voiceName;
            if (voiceName.StartsWith("Google: "))
            {
                actualVoiceName = voiceName.Substring("Google: ".Length);
            }

            var input = new SynthesisInput { Text = text };

            var voice = new VoiceSelectionParams
            {
                LanguageCode = "en-US",
                Name = actualVoiceName
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

        public void Dispose()
        {
            Cleanup();
        }
    }
}