using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using NAudio.Wave;

namespace AkkoTTS
{
    public partial class AkkoTTSForm : Form
    {
        private SpeechSynthesizer synth;
        private Dictionary<int, string> voiceFilePaths = new Dictionary<int, string>();
        private string configFilePath;
        private WaveOutEvent currentOutputDevice;
        private bool isPlaying = false;
        private WaveOutEvent secondaryOutputDevice;
        private bool useSecondaryOutput = false;
        private Label statusLabel;
        private Button lastPlayedButton;
        private Color originalButtonColor;
        private System.Windows.Forms.Timer animationTimer;
        private int animationFrame = 0;
        private string[] animationFrames = new string[] { "▮▯▯▯", "▮▮▯▯", "▮▮▮▯", "▮▮▮▮", "▮▮▮▯", "▮▮▯▯", "▮▯▯▯" };

        public AkkoTTSForm()
        {
            InitializeComponent();
            synth = new SpeechSynthesizer();

            statusLabel = new Label
            {
                Text = "Ready",
                AutoSize = true,
                Location = new Point(10, this.ClientSize.Height - 30),
                BackColor = Color.LightGreen,
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(statusLabel);

            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 150;
            animationTimer.Tick += AnimationTimer_Tick;

            foreach (var voice in synth.GetInstalledVoices().Select(v => v.VoiceInfo.Name))
                voiceComboBox.Items.Add(voice);

            if (voiceComboBox.Items.Count > 0)
                voiceComboBox.SelectedIndex = 0;

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var caps = WaveOut.GetCapabilities(i);
                outputDeviceComboBox.Items.Add($"{i}: {caps.ProductName}");
                headphoneComboBox.Items.Add($"{i}: {caps.ProductName}");
            }

            if (outputDeviceComboBox.Items.Count > 0)
            {
                outputDeviceComboBox.SelectedIndex = 0;
                headphoneComboBox.SelectedIndex = -1;
            }

            useHeadphonesCheckBox.CheckedChanged += (s, e) =>
            {
                useSecondaryOutput = useHeadphonesCheckBox.Checked;
                headphoneComboBox.Enabled = useSecondaryOutput;
            };

            speakBtn.Click += speakBtn_Click;
            clearBtn.Click += clearBtn_Click;
            speachTxt.KeyDown += speachTxt_KeyDown;

            configFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "sound_locations.txt");

            LoadVoiceConfigurations();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationFrame = (animationFrame + 1) % animationFrames.Length;

            if (isPlaying)
            {
                string baseText = statusLabel.Text;
                if (baseText.EndsWith(" " + animationFrames[0]) ||
                    baseText.EndsWith(" " + animationFrames[1]) ||
                    baseText.EndsWith(" " + animationFrames[2]) ||
                    baseText.EndsWith(" " + animationFrames[3]) ||
                    baseText.EndsWith(" " + animationFrames[4]) ||
                    baseText.EndsWith(" " + animationFrames[5]) ||
                    baseText.EndsWith(" " + animationFrames[6]))
                {
                    baseText = baseText.Substring(0, baseText.LastIndexOf(" "));
                }

                statusLabel.Text = baseText + " " + animationFrames[animationFrame];
            }
        }

        private void UpdatePlayingStatus(bool isPlaying, string message = null)
        {
            if (isPlaying)
            {
                statusLabel.Text = message ?? "Playing audio...";
                statusLabel.BackColor = Color.LightSalmon;

                animationFrame = 0;
                animationTimer.Start();
            }
            else
            {
                animationTimer.Stop();

                statusLabel.Text = "Ready";
                statusLabel.BackColor = Color.LightGreen;

                if (lastPlayedButton != null)
                {
                    lastPlayedButton.BackColor = originalButtonColor;
                    lastPlayedButton = null;
                }
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            speachTxt.Clear();
        }

        private void speakBtn_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(speachTxt.Text))
                return;

            StopAudio();

            UpdatePlayingStatus(true, "Speaking text...");

            if (voiceComboBox.SelectedItem != null)
                synth.SelectVoice(voiceComboBox.SelectedItem.ToString()!);

            try
            {
                using (var stream = new MemoryStream())
                {
                    var format = new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono);
                    synth.SetOutputToAudioStream(stream, format);
                    synth.Speak(speachTxt.Text);
                    synth.SetOutputToNull();

                    stream.Position = 0;
                    byte[] audioData = stream.ToArray();

                    var primaryStream = new MemoryStream(audioData);
                    var waveProvider = new RawSourceWaveStream(primaryStream, new WaveFormat(44100, 16, 1));
                    currentOutputDevice = new WaveOutEvent();
                    currentOutputDevice.DeviceNumber = outputDeviceComboBox.SelectedIndex;
                    currentOutputDevice.Init(waveProvider);

                    currentOutputDevice.PlaybackStopped += (s, e) =>
                    {
                        isPlaying = false;
                        UpdatePlayingStatus(false);
                        CleanupPlayback();
                        primaryStream.Dispose();
                    };

                    if (useSecondaryOutput && headphoneComboBox.SelectedIndex >= 0)
                    {
                        var secondaryStream = new MemoryStream(audioData);
                        var secondaryWaveProvider = new RawSourceWaveStream(secondaryStream, new WaveFormat(44100, 16, 1));
                        secondaryOutputDevice = new WaveOutEvent();
                        secondaryOutputDevice.DeviceNumber = headphoneComboBox.SelectedIndex;
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

                    currentOutputDevice.Play();
                    isPlaying = true;

                }

                speachTxt.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing speech: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdatePlayingStatus(false);
                CleanupPlayback();
            }
        }

        private void speachTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                speakBtn.PerformClick();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            StopAudio();
            synth.Dispose();
            base.OnFormClosed(e);
        }

        private void LoadVoiceConfigurations()
        {
            if (File.Exists(configFilePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(configFilePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            if (int.TryParse(parts[0], out int voiceNumber) && voiceNumber >= 1 && voiceNumber <= 8)
                            {
                                string filePath = parts[1];
                                voiceFilePaths[voiceNumber] = filePath;
                                UpdateVoiceButtonText(voiceNumber, filePath);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading voice configurations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateVoiceButtonText(int voiceNumber, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            Button button = null;
            switch (voiceNumber)
            {
                case 1: button = voice1; break;
                case 2: button = voice2; break;
                case 3: button = voice3; break;
                case 4: button = voice4; break;
                case 5: button = voice5; break;
                case 6: button = voice6; break;
                case 7: button = voice7; break;
                case 8: button = voice8; break;
            }

            if (button != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName.Length > 15)
                {
                    fileName = fileName.Substring(0, 12) + "...";
                }
                button.Text = fileName;
            }
        }

        private void PlayAudioFile(string filePath, Button sourceButton = null)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("Audio file not found or not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            StopAudio();

            try
            {
                if (sourceButton != null)
                {
                    lastPlayedButton = sourceButton;
                    originalButtonColor = sourceButton.BackColor;
                    sourceButton.BackColor = Color.LightBlue;
                }

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                UpdatePlayingStatus(true, $"Playing: {fileName}");

                var audioFile = new AudioFileReader(filePath);
                currentOutputDevice = new WaveOutEvent();
                currentOutputDevice.DeviceNumber = outputDeviceComboBox.SelectedIndex;
                currentOutputDevice.Init(audioFile);

                currentOutputDevice.PlaybackStopped += (s, e) =>
                {
                    isPlaying = false;
                    UpdatePlayingStatus(false);
                    CleanupPlayback();
                };

                if (useSecondaryOutput && headphoneComboBox.SelectedIndex >= 0)
                {
                    var secondaryAudioFile = new AudioFileReader(filePath);
                    secondaryOutputDevice = new WaveOutEvent();
                    secondaryOutputDevice.DeviceNumber = headphoneComboBox.SelectedIndex;
                    secondaryOutputDevice.Init(secondaryAudioFile);

                    secondaryOutputDevice.PlaybackStopped += (s, e) =>
                    {
                        if (secondaryOutputDevice != null)
                        {
                            secondaryOutputDevice.Dispose();
                            secondaryOutputDevice = null;
                        }
                    };
                }

                currentOutputDevice.Play();
                if (secondaryOutputDevice != null)
                    secondaryOutputDevice.Play();

                isPlaying = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing audio file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdatePlayingStatus(false);
                CleanupPlayback();
            }
        }

        private void CleanupPlayback()
        {
            if (currentOutputDevice != null)
            {
                currentOutputDevice.Dispose();
                currentOutputDevice = null;
            }

            if (secondaryOutputDevice != null)
            {
                secondaryOutputDevice.Dispose();
                secondaryOutputDevice = null;
            }
        }

        private void StopAudio()
        {
            if (currentOutputDevice != null && isPlaying)
            {
                currentOutputDevice.Stop();
                if (secondaryOutputDevice != null)
                    secondaryOutputDevice.Stop();

                isPlaying = false;
                UpdatePlayingStatus(false);
                CleanupPlayback();
            }
        }

        private void voice1_Click(object sender, EventArgs e)
        {
            if (voiceFilePaths.TryGetValue(1, out string filePath))
            {
                PlayAudioFile(filePath);
            }
            else
            {
                MessageBox.Show("No audio file set for Voice 1", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void voice2_Click(object sender, EventArgs e)
        {
            if (voiceFilePaths.TryGetValue(2, out string filePath))
            {
                PlayAudioFile(filePath);
            }
            else
            {
                MessageBox.Show("No audio file set for Voice 2", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void voice3_Click(object sender, EventArgs e)
        {
            if (voiceFilePaths.TryGetValue(3, out string filePath))
            {
                PlayAudioFile(filePath);
            }
            else
            {
                MessageBox.Show("No audio file set for Voice 3", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void voice4_Click(object sender, EventArgs e)
        {
            if (voiceFilePaths.TryGetValue(4, out string filePath))
            {
                PlayAudioFile(filePath);
            }
            else
            {
                MessageBox.Show("No audio file set for Voice 4", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void voice5_Click(object sender, EventArgs e)
        {
            if (voiceFilePaths.TryGetValue(5, out string filePath))
            {
                PlayAudioFile(filePath);
            }
            else
            {
                MessageBox.Show("No audio file set for Voice 5", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void voice6_Click(object sender, EventArgs e)
        {
            if (voiceFilePaths.TryGetValue(6, out string filePath))
            {
                PlayAudioFile(filePath);
            }
            else
            {
                MessageBox.Show("No audio file set for Voice 6", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void voice7_Click(object sender, EventArgs e)
        {
            if (voiceFilePaths.TryGetValue(7, out string filePath))
            {
                PlayAudioFile(filePath);
            }
            else
            {
                MessageBox.Show("No audio file set for Voice 7", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void voice8_Click(object sender, EventArgs e)
        {
            if (voiceFilePaths.TryGetValue(8, out string filePath))
            {
                PlayAudioFile(filePath);
            }
            else
            {
                MessageBox.Show("No audio file set for Voice 8", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void editVoiceLinesBtn_Click(object sender, EventArgs e)
        {
            EditVoiceLines editVoiceLinesForm = new EditVoiceLines();
            editVoiceLinesForm.ShowDialog();

            LoadVoiceConfigurations();
        }

        private void stopAudioBtn_Click(object sender, EventArgs e)
        {
            StopAudio();
        }
    }
}
