using System.IO;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using Aljaras.MVVM.Model;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using LiteDB;
using System.Linq;

namespace Aljaras.MVVM.ViewModel
{
    internal partial class SettingsViewModel : ObservableRecipient
    {

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        [ObservableProperty]
        private List<string> _lang = new();

        [ObservableProperty]
        private UserSettings userSet = new();

        #region RelayCommands
        [RelayCommand]
        private void SaveSettings()
        {
            using (var db = new LiteDatabase(@"Filename=Aljaras.jrsdb;connection=shared"))
            {
                var col = db.GetCollection<UserSettings>("UserSettings");
                var results = col.Find(x => x.Id == 1).FirstOrDefault();
                if (results != null)
                    col.Update(UserSet);
                else col.Insert(UserSet);
                MessageBox.Show("Done");
            }
            Global.GetUserSettings = UserSet;
            Global.SetAppLang();
        }

        [RelayCommand]
        private void PlayEmergencyAudioFile()
        {
            if (!Global.AudioPlayer.IsEmergency)
                Global.AudioPlayer.PlayPauseAudioFile(userSet.EmergencyAudioFileLocation, false);
        }

        [RelayCommand]
        private void SelectEmergencyAudioFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            string path = AppDomain.CurrentDomain.BaseDirectory + "Audio"; // this is the path that you are checking.
            if (Directory.Exists(path))
            {
                openFileDialog.InitialDirectory = path;
            }
            openFileDialog.Filter = "Audio File (*.mp3;*.wav)|*.mp3;*.wav;";
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            userSet.EmergencyAudioFileLocation = openFileDialog.FileName;
            if (Global.AudioPlayer.Output != null && Global.AudioPlayer.Output.PlaybackState != NAudio.Wave.PlaybackState.Stopped)
            {
                Global.AudioPlayer.Output.Stop();
                Global.AudioPlayer.Output = null;
            }
            if (!Global.AudioPlayer.IsEmergency)
                Global.AudioPlayer.PlayPauseAudioFile(openFileDialog.FileName, false);
        }
        #endregion



        public SettingsViewModel()
        {
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Languages"))
            {
                FileInfo[] files = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Languages").GetFiles("*.xml");
                foreach (FileInfo file in files)
                    _lang.Add(Path.GetFileNameWithoutExtension(file.Name));
            }
            using (var db = new LiteDatabase(@"Filename=Aljaras.jrsdb;connection=shared"))
            {
                var col = db.GetCollection<UserSettings>("UserSettings");
                var results = col.Find(x => x.Id == 1).FirstOrDefault();
                if (results != null)
                    UserSet = results;
            }
        }




    }
}
