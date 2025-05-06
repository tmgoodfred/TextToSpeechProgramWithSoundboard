using Microsoft.Extensions.Configuration;

namespace AkkoTTS
{
    public partial class AkkoTTSForm : Form
    {
        private SpeechSynthesizerManager speechSynthesizerManager;
        private AudioManager audioManager;
        private VoiceConfigurationManager voiceConfigManager;
        private StatusManager statusManager;
        private GoogleCloudTTS googleCloudTTS;
        private IConfigurationRoot configuration;
        private Label statusLabel;
        private string lastSpokenText = "";
        private string lastSelectedVoice = "";
        private bool wasLastSpeechCloud = false;
        private string lastCloudAudioFilePath = "";

        public AkkoTTSForm()
        {
            InitializeComponent();

            // Initialize configuration
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            

            configuration = builder.Build();

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

            // Initialize managers
            statusManager = new StatusManager(statusLabel);
            speechSynthesizerManager = new SpeechSynthesizerManager();
            audioManager = new AudioManager((isPlaying, message) =>
                statusManager.UpdateStatus(isPlaying, message));
            voiceConfigManager = new VoiceConfigurationManager();
            googleCloudTTS = new GoogleCloudTTS(configuration);

            ConfigureButtonAppearance(voice1);
            ConfigureButtonAppearance(voice2);
            ConfigureButtonAppearance(voice3);
            ConfigureButtonAppearance(voice4);
            ConfigureButtonAppearance(voice5);
            ConfigureButtonAppearance(voice6);
            ConfigureButtonAppearance(voice7);
            ConfigureButtonAppearance(voice8);

            useHeadphonesCheckBox.CheckedChanged += UseHeadphonesCheckBox_CheckedChanged;


            // Load voices into combobox
            LoadVoices();

            // Load audio devices
            LoadAudioDevices();

            // Set up voice buttons
            SetupVoiceButtons();
        }

        private void LoadVoices()
        {
            // Add local voices first
            foreach (var voice in speechSynthesizerManager.GetInstalledVoices())
            {
                voiceComboBox.Items.Add(voice);
            }

            // Then add Google Cloud voices
            googleCloudTTS.ListAvailableVoices(voiceName => voiceComboBox.Items.Add(voiceName));

            if (voiceComboBox.Items.Count > 0)
                voiceComboBox.SelectedIndex = 0;
        }

        private void LoadAudioDevices()
        {
            var devices = AudioManager.GetAvailableDevices();

            foreach (var device in devices)
            {
                outputDeviceComboBox.Items.Add(device);
                headphoneComboBox.Items.Add(device);
            }

            if (outputDeviceComboBox.Items.Count > 0)
            {
                outputDeviceComboBox.SelectedIndex = 0;
                headphoneComboBox.SelectedIndex = -1;
                headphoneComboBox.Enabled = false;
            }
        }

        private void SetupVoiceButtons()
        {
            // Update button text based on configured voice lines
            foreach (var kvp in voiceConfigManager.VoiceFilePaths)
            {
                int voiceNumber = kvp.Key;
                string filePath = kvp.Value;
                UpdateVoiceButtonText(voiceNumber, filePath);
            }

            // Set up click handlers for voice buttons
            voice1.Click += (s, e) => PlayVoiceLine(1, voice1);
            voice2.Click += (s, e) => PlayVoiceLine(2, voice2);
            voice3.Click += (s, e) => PlayVoiceLine(3, voice3);
            voice4.Click += (s, e) => PlayVoiceLine(4, voice4);
            voice5.Click += (s, e) => PlayVoiceLine(5, voice5);
            voice6.Click += (s, e) => PlayVoiceLine(6, voice6);
            voice7.Click += (s, e) => PlayVoiceLine(7, voice7);
            voice8.Click += (s, e) => PlayVoiceLine(8, voice8);
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
                button.Text = voiceConfigManager.GetFormattedButtonText(filePath);
            }
        }

        private void PlayVoiceLine(int voiceNumber, Button sourceButton)
        {
            if (voiceConfigManager.TryGetVoiceFilePath(voiceNumber, out string filePath))
            {
                try
                {
                    int primaryDeviceIndex = outputDeviceComboBox.SelectedIndex;
                    int? secondaryDeviceIndex = useHeadphonesCheckBox.Checked && headphoneComboBox.SelectedIndex >= 0
                        ? headphoneComboBox.SelectedIndex
                        : null;

                    audioManager.PlayAudioFile(filePath, primaryDeviceIndex, secondaryDeviceIndex);
                    statusManager.UpdateStatus(true, null, sourceButton);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"No audio file set for Voice {voiceNumber}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void speakBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(speachTxt.Text))
                return;

            try
            {
                string selectedVoice = voiceComboBox.SelectedItem?.ToString();

                // Store the last spoken text and voice
                lastSpokenText = speachTxt.Text;
                lastSelectedVoice = selectedVoice;

                // Check if it's a Google Cloud voice
                if (selectedVoice != null && selectedVoice.StartsWith("Google:"))
                {
                    wasLastSpeechCloud = true;
                    PlayGoogleCloudSpeech(speachTxt.Text, selectedVoice);
                }
                else
                {
                    wasLastSpeechCloud = false;
                    PlayLocalSpeech(speachTxt.Text, selectedVoice);
                }

                speachTxt.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing speech: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void speachTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                speakBtn_Click(sender, e);
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            speachTxt.Clear();
        }

        private async void PlayGoogleCloudSpeech(string text, string selectedVoice, bool isRepeat = false)
        {
            try
            {
                string audioFilePath;

                // If this is a repeat and we have a saved file path, use it
                if (isRepeat && !string.IsNullOrEmpty(lastCloudAudioFilePath) && File.Exists(lastCloudAudioFilePath))
                {
                    audioFilePath = lastCloudAudioFilePath;
                    statusManager.UpdateStatus(true, "Replaying Cloud TTS...");
                }
                else
                {
                    // Generate new audio
                    statusManager.UpdateStatus(true, "Generating speech with Google Cloud...");
                    audioFilePath = await googleCloudTTS.SynthesizeSpeechAsync(text, selectedVoice);
                    lastCloudAudioFilePath = audioFilePath; // Save the file path for future repeats
                }

                // Play the audio file
                int primaryDeviceIndex = outputDeviceComboBox.SelectedIndex;
                int? secondaryDeviceIndex = useHeadphonesCheckBox.Checked && headphoneComboBox.SelectedIndex >= 0
                    ? headphoneComboBox.SelectedIndex
                    : null;

                // Use a direct call to play the audio file without updating the status
                try
                {
                    var audioFile = new NAudio.Wave.AudioFileReader(audioFilePath);
                    audioManager.StopAudio(); // Stop any currently playing audio

                    // Update status manually before playing
                    statusManager.UpdateStatus(true, isRepeat ? "Replaying Cloud TTS..." : "Playing Cloud TTS...");

                    // Play the audio file
                    audioManager.PlaySpeechFromReader(audioFile, primaryDeviceIndex, secondaryDeviceIndex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error playing audio file: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                statusManager.UpdateStatus(false);
                MessageBox.Show($"Error with Google Cloud TTS: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PlayLocalSpeech(string text, string selectedVoice)
        {
            try
            {
                using (var stream = speechSynthesizerManager.SynthesizeSpeech(text, selectedVoice))
                {
                    // Play the audio
                    int primaryDeviceIndex = outputDeviceComboBox.SelectedIndex;
                    int? secondaryDeviceIndex = useHeadphonesCheckBox.Checked && headphoneComboBox.SelectedIndex >= 0
                        ? headphoneComboBox.SelectedIndex
                        : null;

                    audioManager.PlaySpeechFromStream(stream, primaryDeviceIndex, secondaryDeviceIndex);
                }
            }
            catch (Exception ex)
            {
                statusManager.UpdateStatus(false);
                MessageBox.Show($"Error with local speech synthesis: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UseHeadphonesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            headphoneComboBox.Enabled = useHeadphonesCheckBox.Checked;
        }

        private void stopAudioBtn_Click(object sender, EventArgs e)
        {
            audioManager.StopAudio();
        }

        private void editVoiceLinesBtn_Click(object sender, EventArgs e)
        {
            using (var editVoiceLinesForm = new EditVoiceLines())
            {
                editVoiceLinesForm.ShowDialog();
            }

            voiceConfigManager.LoadVoiceConfigurations();

            foreach (var kvp in voiceConfigManager.VoiceFilePaths)
            {
                UpdateVoiceButtonText(kvp.Key, kvp.Value);
            }
        }

        private void ConfigureButtonAppearance(Button button)
        {
            button.BackColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.MouseDownBackColor = Color.White;
            button.FlatAppearance.MouseOverBackColor = Color.White;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            audioManager.StopAudio();
            speechSynthesizerManager.Dispose();
            googleCloudTTS.Dispose();
            statusManager.Dispose();
            base.OnFormClosed(e);
        }

        private void repeatBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lastSpokenText) || string.IsNullOrEmpty(lastSelectedVoice))
            {
                MessageBox.Show("No previous speech to repeat.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (wasLastSpeechCloud)
                {
                    PlayGoogleCloudSpeech(lastSpokenText, lastSelectedVoice, true);
                }
                else
                {
                    PlayLocalSpeech(lastSpokenText, lastSelectedVoice);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error repeating speech: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}