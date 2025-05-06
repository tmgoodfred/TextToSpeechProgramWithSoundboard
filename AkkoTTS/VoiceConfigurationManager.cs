namespace AkkoTTS
{
    public class VoiceConfigurationManager
    {
        private readonly string configFilePath;
        private Dictionary<int, string> voiceFilePaths = new Dictionary<int, string>();

        public Dictionary<int, string> VoiceFilePaths => voiceFilePaths;

        public VoiceConfigurationManager()
        {
            configFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "sound_locations.txt");

            LoadVoiceConfigurations();
        }

        public void LoadVoiceConfigurations()
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
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error loading voice configurations: {ex.Message}", ex);
                }
            }
        }

        public string GetFormattedButtonText(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName.Length > 15)
            {
                fileName = fileName.Substring(0, 12) + "...";
            }
            return fileName;
        }

        public bool TryGetVoiceFilePath(int voiceNumber, out string filePath)
        {
            return voiceFilePaths.TryGetValue(voiceNumber, out filePath);
        }
    }
}