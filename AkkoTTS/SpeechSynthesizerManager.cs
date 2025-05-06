using System.Speech.AudioFormat;
using System.Speech.Synthesis;

namespace AkkoTTS
{
    public class SpeechSynthesizerManager : IDisposable
    {
        private SpeechSynthesizer synth;

        public SpeechSynthesizerManager()
        {
            synth = new SpeechSynthesizer();
        }

        public List<string> GetInstalledVoices()
        {
            return synth.GetInstalledVoices()
                .Select(v => v.VoiceInfo.Name)
                .ToList();
        }

        public MemoryStream SynthesizeSpeech(string text, string voiceName)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty", nameof(text));

            if (!string.IsNullOrEmpty(voiceName))
                synth.SelectVoice(voiceName);

            var stream = new MemoryStream();
            var format = new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono);
            synth.SetOutputToAudioStream(stream, format);
            synth.Speak(text);
            synth.SetOutputToNull();

            stream.Position = 0;
            return stream;
        }

        public void Dispose()
        {
            synth?.Dispose();
        }
    }
}