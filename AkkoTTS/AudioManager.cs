using NAudio.Wave;

namespace AkkoTTS
{
    public class AudioManager : IDisposable
    {
        private WaveOutEvent? primaryOutputDevice;
        private WaveOutEvent? secondaryOutputDevice;
        private bool isPlaying = false;
        private Action<bool, string> updateStatusCallback;

        public bool IsPlaying => isPlaying;

        public AudioManager(Action<bool, string> statusUpdateCallback)
        {
            updateStatusCallback = statusUpdateCallback;
        }

        public void PlaySpeechFromStream(MemoryStream audioStream, int primaryDeviceIndex, int? secondaryDeviceIndex = null)
        {
            StopAudio();

            try
            {
                byte[] audioData = audioStream.ToArray();
                var primaryStream = new MemoryStream(audioData);
                var waveProvider = new RawSourceWaveStream(primaryStream, new WaveFormat(44100, 16, 1));

                primaryOutputDevice = new WaveOutEvent();
                primaryOutputDevice.DeviceNumber = primaryDeviceIndex;
                primaryOutputDevice.Init(waveProvider);

                primaryOutputDevice.PlaybackStopped += (s, e) =>
                {
                    isPlaying = false;
                    updateStatusCallback?.Invoke(false, null);
                    CleanupPlayback();
                    primaryStream.Dispose();
                };

                if (secondaryDeviceIndex.HasValue && secondaryDeviceIndex.Value >= 0)
                {
                    var secondaryStream = new MemoryStream(audioData);
                    var secondaryWaveProvider = new RawSourceWaveStream(secondaryStream, new WaveFormat(44100, 16, 1));
                    secondaryOutputDevice = new WaveOutEvent();
                    secondaryOutputDevice.DeviceNumber = secondaryDeviceIndex.Value;
                    secondaryOutputDevice.Init(secondaryWaveProvider);

                    secondaryOutputDevice.PlaybackStopped += (s, e) =>
                    {
                        if (secondaryOutputDevice != null)
                        {
                            secondaryOutputDevice.Dispose();
                            secondaryOutputDevice = null;
                        }
                        secondaryStream.Dispose();
                    };

                    secondaryOutputDevice.Play();
                }

                primaryOutputDevice.Play();
                isPlaying = true;
                updateStatusCallback?.Invoke(true, "Speaking text...");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error playing speech: {ex.Message}", ex);
            }
        }

        public void PlaySpeechFromReader(AudioFileReader audioFile, int primaryDeviceIndex, int? secondaryDeviceIndex = null)
        {
            StopAudio();

            try
            {
                primaryOutputDevice = new WaveOutEvent();
                primaryOutputDevice.DeviceNumber = primaryDeviceIndex;
                primaryOutputDevice.Init(audioFile);

                primaryOutputDevice.PlaybackStopped += (s, e) =>
                {
                    isPlaying = false;
                    updateStatusCallback?.Invoke(false, null);
                    CleanupPlayback();
                };

                if (secondaryDeviceIndex.HasValue && secondaryDeviceIndex.Value >= 0)
                {
                    var secondaryAudioFile = new AudioFileReader(audioFile.FileName);
                    secondaryOutputDevice = new WaveOutEvent();
                    secondaryOutputDevice.DeviceNumber = secondaryDeviceIndex.Value;
                    secondaryOutputDevice.Init(secondaryAudioFile);

                    secondaryOutputDevice.PlaybackStopped += (s, e) =>
                    {
                        if (secondaryOutputDevice != null)
                        {
                            secondaryOutputDevice.Dispose();
                            secondaryOutputDevice = null;
                        }
                    };

                    secondaryOutputDevice.Play();
                }

                primaryOutputDevice.Play();
                isPlaying = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error playing audio file: {ex.Message}", ex);
            }
        }

        public void PlayAudioFile(string filePath, int primaryDeviceIndex, int? secondaryDeviceIndex = null)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException("Audio file not found or not set.");
            }

            StopAudio();

            try
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                updateStatusCallback?.Invoke(true, $"Playing: {fileName}");

                var audioFile = new AudioFileReader(filePath);
                primaryOutputDevice = new WaveOutEvent();
                primaryOutputDevice.DeviceNumber = primaryDeviceIndex;
                primaryOutputDevice.Init(audioFile);

                primaryOutputDevice.PlaybackStopped += (s, e) =>
                {
                    isPlaying = false;
                    updateStatusCallback?.Invoke(false, null);
                    CleanupPlayback();
                };

                if (secondaryDeviceIndex.HasValue && secondaryDeviceIndex.Value >= 0)
                {
                    var secondaryAudioFile = new AudioFileReader(filePath);
                    secondaryOutputDevice = new WaveOutEvent();
                    secondaryOutputDevice.DeviceNumber = secondaryDeviceIndex.Value;
                    secondaryOutputDevice.Init(secondaryAudioFile);

                    secondaryOutputDevice.PlaybackStopped += (s, e) =>
                    {
                        if (secondaryOutputDevice != null)
                        {
                            secondaryOutputDevice.Dispose();
                            secondaryOutputDevice = null;
                        }
                    };

                    secondaryOutputDevice.Play();
                }

                primaryOutputDevice.Play();
                isPlaying = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error playing audio file: {ex.Message}", ex);
            }
        }

        public void StopAudio()
        {
            if (primaryOutputDevice != null && isPlaying)
            {
                primaryOutputDevice.Stop();
                if (secondaryOutputDevice != null)
                    secondaryOutputDevice.Stop();

                isPlaying = false;
                updateStatusCallback?.Invoke(false, null);
                CleanupPlayback();
            }
        }

        private void CleanupPlayback()
        {
            if (primaryOutputDevice != null)
            {
                primaryOutputDevice.Dispose();
                primaryOutputDevice = null;
            }

            if (secondaryOutputDevice != null)
            {
                secondaryOutputDevice.Dispose();
                secondaryOutputDevice = null;
            }
        }

        public void Dispose()
        {
            StopAudio();
        }

        public static List<string> GetAvailableDevices()
        {
            var devices = new List<string>();
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var caps = WaveOut.GetCapabilities(i);
                devices.Add($"{i}: {caps.ProductName}");
            }
            return devices;
        }
    }
}