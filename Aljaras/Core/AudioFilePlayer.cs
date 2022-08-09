using Aljaras.MVVM.ViewModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aljaras.Core
{
    internal partial class AudioFilePlayer : ObservableRecipient
    {
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        [ObservableProperty]
        private BlockAlignReductionStream? stream = null;

        [ObservableProperty]
        private DirectSoundOut? output = null;

        [ObservableProperty]
        private bool isEmergency = false;

        [ObservableProperty]
        private bool repeat = false;

        public async Task PlayPauseAudioFile(string fileLocation, bool emergency)
        {
            if (emergency)
            {
                if (Output != null && Output.PlaybackState != PlaybackState.Stopped)
                    Output.Stop();
                Output = null;
            }
            
            if (Output != null)
            {
                if (Output.PlaybackState == PlaybackState.Playing) Output.Pause();
                else if (Output.PlaybackState == PlaybackState.Paused) Output.Play();
                else if (Output.PlaybackState == PlaybackState.Stopped)
                    StartAudio(fileLocation);
            } else if (fileLocation != null)
                {
                    if (Output != null && Output.PlaybackState == PlaybackState.Playing) Output.Stop();
                    StartAudio(fileLocation);
                    while (IsEmergency && Repeat)
                    {
                        if (Output != null && Output.PlaybackState == PlaybackState.Stopped)
                        StartAudio(fileLocation);
                        await Task.Delay(500);
                    }
                    if(!Repeat)
                    GlobalViewModel.Instance.AudioPlayer.isEmergency = false;
                }
        }

        private void StartAudio(string fileLocation)
        {
            WaveStream pcm;
            if (fileLocation.ToLower().EndsWith(".mp3"))
                pcm = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(fileLocation));
            else if (fileLocation.ToLower().EndsWith(".wav"))
                pcm = new WaveChannel32(new WaveFileReader(fileLocation));
            else
            {
                MessageBox.Show("Not a correct audio file type.");
                return;
            }
            stream = new BlockAlignReductionStream(pcm);
            Output = new DirectSoundOut();
            Output.Init(stream);
            Output.Play();
        }
    }
}
