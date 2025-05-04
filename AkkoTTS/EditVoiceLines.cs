using System.IO;
using System.Text;

namespace AkkoTTS
{
    public partial class EditVoiceLines : Form
    {
        private Dictionary<int, string> voiceFilePaths = new Dictionary<int, string>();
        private string configFilePath;

        public EditVoiceLines()
        {
            InitializeComponent();

            configFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "sound_locations.txt");

            LoadVoiceConfigurations();
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
                                voiceFilePaths[voiceNumber] = parts[1];
                                UpdateVoiceLabel(voiceNumber, parts[1]);
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

        private void UpdateVoiceLabel(int voiceNumber, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            Label label = null;
            switch (voiceNumber)
            {
                case 1: label = voice1Lbl; break;
                case 2: label = voice2Lbl; break;
                case 3: label = voice3Lbl; break;
                case 4: label = voice4Lbl; break;
                case 5: label = voice5Lbl; break;
                case 6: label = voice6Lbl; break;
                case 7: label = voice7Lbl; break;
                case 8: label = voice8Lbl; break;
            }

            if (label != null)
            {
                label.Text = Path.GetFileName(filePath);
            }
        }

        private void SaveVoiceConfigurations()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var kvp in voiceFilePaths)
                {
                    sb.AppendLine($"{kvp.Key}={kvp.Value}");
                }
                File.WriteAllText(configFilePath, sb.ToString());
                MessageBox.Show("Voice configurations saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving voice configurations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectAudioFile(int voiceNumber)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Audio Files (*.mp3;*.wav)|*.mp3;*.wav";
                openFileDialog.Title = $"Select Audio File for Voice {voiceNumber}";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    voiceFilePaths[voiceNumber] = filePath;
                    UpdateVoiceLabel(voiceNumber, filePath);
                }
            }
        }

        private void editVoice1_Click(object sender, EventArgs e)
        {
            SelectAudioFile(1);
        }

        private void editVoice2_Click(object sender, EventArgs e)
        {
            SelectAudioFile(2);
        }

        private void editVoice3_Click(object sender, EventArgs e)
        {
            SelectAudioFile(3);
        }

        private void editVoice4_Click(object sender, EventArgs e)
        {
            SelectAudioFile(4);
        }

        private void editVoice5_Click(object sender, EventArgs e)
        {
            SelectAudioFile(5);
        }

        private void editVoice6_Click(object sender, EventArgs e)
        {
            SelectAudioFile(6);
        }

        private void editVoice7_Click(object sender, EventArgs e)
        {
            SelectAudioFile(7);
        }

        private void editVoice8_Click(object sender, EventArgs e)
        {
            SelectAudioFile(8);
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            SaveVoiceConfigurations();
            this.Close();
        }
    }
}