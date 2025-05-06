namespace AkkoTTS
{
    public class StatusManager
    {
        private Label statusLabel;
        private System.Windows.Forms.Timer animationTimer;
        private int animationFrame = 0;
        private string[] animationFrames = new string[] { "▮▯▯▯", "▮▮▯▯", "▮▮▮▯", "▮▮▮▮", "▮▮▮▯", "▮▮▯▯", "▮▯▯▯" };
        private Button? lastPlayedButton;
        private Color originalButtonColor;

        public StatusManager(Label statusLabel)
        {
            this.statusLabel = statusLabel;

            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 150;
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationFrame = (animationFrame + 1) % animationFrames.Length;

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

        public void UpdateStatus(bool isPlaying, string? message = null, Button? sourceButton = null)
        {
            if (isPlaying)
            {
                if (sourceButton != null)
                {
                    lastPlayedButton = sourceButton;
                    originalButtonColor = sourceButton.BackColor;
                    sourceButton.BackColor = Color.LightBlue;
                }

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

        public void Dispose()
        {
            animationTimer?.Stop();
            animationTimer?.Dispose();
        }
    }
}