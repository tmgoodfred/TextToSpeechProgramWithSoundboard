namespace AkkoTTS
{
    partial class AkkoTTSForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            speachTxt = new TextBox();
            clearBtn = new Button();
            speakBtn = new Button();
            voiceComboBox = new ComboBox();
            outputDeviceComboBox = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            voice1 = new Button();
            editVoiceLinesBtn = new Button();
            voice2 = new Button();
            voice3 = new Button();
            voice4 = new Button();
            voice5 = new Button();
            voice6 = new Button();
            voice7 = new Button();
            voice8 = new Button();
            stopAudioBtn = new Button();
            label3 = new Label();
            headphoneComboBox = new ComboBox();
            useHeadphonesCheckBox = new CheckBox();
            SuspendLayout();
            // 
            // speachTxt
            // 
            speachTxt.Location = new Point(134, 49);
            speachTxt.Multiline = true;
            speachTxt.Name = "speachTxt";
            speachTxt.Size = new Size(585, 104);
            speachTxt.TabIndex = 0;
            speachTxt.KeyDown += speachTxt_KeyDown;
            // 
            // clearBtn
            // 
            clearBtn.Location = new Point(53, 69);
            clearBtn.Name = "clearBtn";
            clearBtn.Size = new Size(75, 23);
            clearBtn.TabIndex = 1;
            clearBtn.Text = "Clear";
            clearBtn.UseVisualStyleBackColor = true;
            clearBtn.Click += clearBtn_Click;
            // 
            // speakBtn
            // 
            speakBtn.Location = new Point(725, 69);
            speakBtn.Name = "speakBtn";
            speakBtn.Size = new Size(75, 23);
            speakBtn.TabIndex = 2;
            speakBtn.Text = "Speak";
            speakBtn.UseVisualStyleBackColor = true;
            speakBtn.Click += speakBtn_Click;
            // 
            // voiceComboBox
            // 
            voiceComboBox.FormattingEnabled = true;
            voiceComboBox.Location = new Point(180, 183);
            voiceComboBox.Name = "voiceComboBox";
            voiceComboBox.Size = new Size(180, 23);
            voiceComboBox.TabIndex = 3;
            // 
            // outputDeviceComboBox
            // 
            outputDeviceComboBox.FormattingEnabled = true;
            outputDeviceComboBox.Location = new Point(482, 183);
            outputDeviceComboBox.Name = "outputDeviceComboBox";
            outputDeviceComboBox.Size = new Size(228, 23);
            outputDeviceComboBox.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.WhiteSmoke;
            label1.Location = new Point(136, 188);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 5;
            label1.Text = "Voice:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.WhiteSmoke;
            label2.Location = new Point(390, 188);
            label2.Name = "label2";
            label2.Size = new Size(86, 15);
            label2.TabIndex = 6;
            label2.Text = "Output Device:";
            // 
            // voice1
            // 
            voice1.Location = new Point(136, 259);
            voice1.Name = "voice1";
            voice1.Size = new Size(88, 23);
            voice1.TabIndex = 7;
            voice1.Text = "Voice Line 1";
            voice1.UseVisualStyleBackColor = true;
            voice1.Click += voice1_Click;
            // 
            // editVoiceLinesBtn
            // 
            editVoiceLinesBtn.Location = new Point(292, 364);
            editVoiceLinesBtn.Name = "editVoiceLinesBtn";
            editVoiceLinesBtn.Size = new Size(101, 23);
            editVoiceLinesBtn.TabIndex = 8;
            editVoiceLinesBtn.Text = "Edit Voice Lines";
            editVoiceLinesBtn.UseVisualStyleBackColor = true;
            editVoiceLinesBtn.Click += editVoiceLinesBtn_Click;
            // 
            // voice2
            // 
            voice2.Location = new Point(272, 259);
            voice2.Name = "voice2";
            voice2.Size = new Size(88, 23);
            voice2.TabIndex = 9;
            voice2.Text = "Voice Line 2";
            voice2.UseVisualStyleBackColor = true;
            voice2.Click += voice2_Click;
            // 
            // voice3
            // 
            voice3.Location = new Point(439, 259);
            voice3.Name = "voice3";
            voice3.Size = new Size(88, 23);
            voice3.TabIndex = 10;
            voice3.Text = "Voice Line 3";
            voice3.UseVisualStyleBackColor = true;
            voice3.Click += voice3_Click;
            // 
            // voice4
            // 
            voice4.Location = new Point(609, 259);
            voice4.Name = "voice4";
            voice4.Size = new Size(88, 23);
            voice4.TabIndex = 11;
            voice4.Text = "Voice Line 4";
            voice4.UseVisualStyleBackColor = true;
            voice4.Click += voice4_Click;
            // 
            // voice5
            // 
            voice5.Location = new Point(136, 313);
            voice5.Name = "voice5";
            voice5.Size = new Size(88, 23);
            voice5.TabIndex = 12;
            voice5.Text = "Voice Line 5";
            voice5.UseVisualStyleBackColor = true;
            voice5.Click += voice5_Click;
            // 
            // voice6
            // 
            voice6.Location = new Point(272, 313);
            voice6.Name = "voice6";
            voice6.Size = new Size(88, 23);
            voice6.TabIndex = 13;
            voice6.Text = "Voice Line 6";
            voice6.UseVisualStyleBackColor = true;
            voice6.Click += voice6_Click;
            // 
            // voice7
            // 
            voice7.Location = new Point(439, 313);
            voice7.Name = "voice7";
            voice7.Size = new Size(88, 23);
            voice7.TabIndex = 14;
            voice7.Text = "Voice Line 7";
            voice7.UseVisualStyleBackColor = true;
            voice7.Click += voice7_Click;
            // 
            // voice8
            // 
            voice8.Location = new Point(609, 313);
            voice8.Name = "voice8";
            voice8.Size = new Size(88, 23);
            voice8.TabIndex = 15;
            voice8.Text = "Voice Line 8";
            voice8.UseVisualStyleBackColor = true;
            voice8.Click += voice8_Click;
            // 
            // stopAudioBtn
            // 
            stopAudioBtn.Location = new Point(433, 364);
            stopAudioBtn.Name = "stopAudioBtn";
            stopAudioBtn.Size = new Size(94, 23);
            stopAudioBtn.TabIndex = 16;
            stopAudioBtn.Text = "STOP AUDIO";
            stopAudioBtn.UseVisualStyleBackColor = true;
            stopAudioBtn.Click += stopAudioBtn_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.WhiteSmoke;
            label3.Location = new Point(374, 217);
            label3.Name = "label3";
            label3.Size = new Size(102, 15);
            label3.TabIndex = 18;
            label3.Text = "Your headphones:";
            // 
            // headphoneComboBox
            // 
            headphoneComboBox.FormattingEnabled = true;
            headphoneComboBox.Location = new Point(482, 214);
            headphoneComboBox.Name = "headphoneComboBox";
            headphoneComboBox.Size = new Size(228, 23);
            headphoneComboBox.TabIndex = 17;
            // 
            // useHeadphonesCheckBox
            // 
            useHeadphonesCheckBox.AutoSize = true;
            useHeadphonesCheckBox.BackColor = Color.WhiteSmoke;
            useHeadphonesCheckBox.Location = new Point(717, 217);
            useHeadphonesCheckBox.Name = "useHeadphonesCheckBox";
            useHeadphonesCheckBox.Size = new Size(97, 19);
            useHeadphonesCheckBox.TabIndex = 19;
            useHeadphonesCheckBox.Text = "Hear Output?";
            useHeadphonesCheckBox.UseVisualStyleBackColor = false;
            // 
            // AkkoTTSForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            BackgroundImage = Properties.Resources.akko_bg;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(837, 413);
            Controls.Add(useHeadphonesCheckBox);
            Controls.Add(label3);
            Controls.Add(headphoneComboBox);
            Controls.Add(stopAudioBtn);
            Controls.Add(voice8);
            Controls.Add(voice7);
            Controls.Add(voice6);
            Controls.Add(voice5);
            Controls.Add(voice4);
            Controls.Add(voice3);
            Controls.Add(voice2);
            Controls.Add(editVoiceLinesBtn);
            Controls.Add(voice1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(outputDeviceComboBox);
            Controls.Add(voiceComboBox);
            Controls.Add(speakBtn);
            Controls.Add(clearBtn);
            Controls.Add(speachTxt);
            Name = "AkkoTTSForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Akko Text To Speech";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox speachTxt;
        private Button clearBtn;
        private Button speakBtn;
        private ComboBox voiceComboBox;
        private ComboBox outputDeviceComboBox;
        private Label label1;
        private Label label2;
        private Button voice1;
        private Button editVoiceLinesBtn;
        private Button voice2;
        private Button voice3;
        private Button voice4;
        private Button voice5;
        private Button voice6;
        private Button voice7;
        private Button voice8;
        private Button stopAudioBtn;
        private Label label3;
        private ComboBox headphoneComboBox;
        private CheckBox useHeadphonesCheckBox;
    }
}
