// ZAudio for HHSAdvSDL

using Plugin.Maui.Audio;
using System;
using System.Dynamic;
using System.IO;
using System.Text;

namespace HHSAdvMAUI
{
    public class ZAudio
    {
        private string[] soundFiles = {
            "highschool", "charumera", "explosion", string.Empty,  "in_toilet", "acid",
        };
        public string DataFolder { get; private set; }
        private readonly IAudioManager audioManager;
        private IAudioPlayer? audioPlayer;
        public ZAudio(IAudioManager audioManager)
        {
            this.audioManager = audioManager;
        }
        public async Task Play(int id)
        {
            if (id < 0 || id >= soundFiles.Length || string.IsNullOrEmpty(soundFiles[id])) return;
            StringBuilder mp3 = new StringBuilder(soundFiles[id]);
            mp3.Append(".mp3");
            string af = mp3.ToString();
            using (var stream = await FileSystem.OpenAppPackageFileAsync(af))
            {
                audioPlayer = audioManager.CreatePlayer(stream);
                audioPlayer.Play();
            }
        }
        public void Stop() => audioPlayer?.Stop();
    }
}
