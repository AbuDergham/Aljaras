using Aljaras.Core;
using Aljaras.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aljaras.MVVM.ViewModel
{
    internal partial class AlarmViewModel : ObservableRecipient
    {
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        #region Variables
        System.Windows.Forms.OpenFileDialog _filePath;
        private NAudio.Wave.BlockAlignReductionStream? stream = null;
        private NAudio.Wave.DirectSoundOut? output = null;
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private List<Schedule> scheduleList = new();

        [ObservableProperty]
        private List<Alarm> alarmList = new();

        [ObservableProperty]
        private Alarm currentAlarm = new();

        [ObservableProperty]
        private TimePicker timePicker = new();

        [ObservableProperty]
        private string audioFileName;

        [ObservableProperty]
        private string isNOScheduleMessageVisible;

        [ObservableProperty]
        private string isNOAlarmMessageVisible;

        [ObservableProperty]
        private bool enableScheduleTitleTB = false;

        [ObservableProperty]
        private bool enableCheckBox = false;

        [ObservableProperty]
        private long playPauseAlarmButton;

        [ObservableProperty]
        private Schedule currentSchedule = new();
        #endregion

        #region RelayCommands
        [RelayCommand]
        private void ReloadAlarm()
        {
            if (CurrentSchedule != null && CurrentSchedule.ScheduleId > 0)
                LoadAlarmCollectionData(CurrentSchedule.ScheduleId);
        }

        [RelayCommand]
        private void EnableAddNewSchedule()
        {
            CurrentSchedule = new Schedule();
            EnableScheduleTitleTB = true;
            EnableCheckBox = true;
        }

        [RelayCommand]
        private void NewAlarm(){CurrentAlarm = new Alarm();}
        
        [RelayCommand]
        private void TimeNow()
        {
            DateTime _dt = DateTime.Now;
           
            CurrentAlarm.Hour = _dt.ToString("hh"); 
            CurrentAlarm.Minute = _dt.ToString("mm");
            CurrentAlarm.DayTime = _dt.ToString("tt");
        }

        [RelayCommand]
        private void SaveAlarm()
        {
            if (CurrentSchedule != null && CurrentSchedule.ScheduleId > 0)
            {
                if (CurrentAlarm != null && !string.IsNullOrEmpty(CurrentAlarm.AlarmTitle) && !string.IsNullOrWhiteSpace(CurrentAlarm.AlarmTitle))
                { 
                    string _audioFileLocation;
                    if (CurrentAlarm.AudioFileLocation != null && File.Exists(CurrentAlarm.AudioFileLocation))
                    _audioFileLocation = CurrentAlarm.AudioFileLocation;
                    else if (_filePath == null) _audioFileLocation = AppDomain.CurrentDomain.BaseDirectory + "Audio\\School.mp3";
                    else _audioFileLocation = _filePath.FileName;
                
                    // Open database (or create if doesn't exist)
                    using (var db = new LiteDatabase(@"Filename=Aljaras.jrsdb;connection=shared"))
                    {
                        Alarm tmpAlarm = CurrentAlarm;
                        //tmpAlarm.FullTime = DateTime.Now;
                        tmpAlarm.FullTime = DateTime.Parse(CurrentAlarm.Hour + ":" + CurrentAlarm.Minute + " " + CurrentAlarm.DayTime);


                            ;
                        tmpAlarm.AudioFileLocation = _audioFileLocation;
                    
                        var col = db.GetCollection<Alarm>("Alarms");
                        if (CurrentAlarm.AlarmId > 0)
                            col.Update(tmpAlarm);
                        else
                        {
                            tmpAlarm.AlarmId = DateTime.Now.Ticks;
                            tmpAlarm.ScheduleId = CurrentSchedule.ScheduleId;
                            col.Insert(tmpAlarm);
                        }
                    }
                    MessageBox.Show("Done");
                    LoadAlarmCollectionData(CurrentSchedule.ScheduleId);
                }else MessageBox.Show("invalid title value.");
            }
            else
            {
                MessageBox.Show("Select a Schedule First.");
                return;
            }
            CurrentAlarm = new();
            CallGlobal();
        }

        private void CallGlobal()
        {
            GlobalViewModel.Instance.LoadMonitoringAlarmCollectionData();
            GlobalViewModel.Instance.NextAlarm();
        }

        [RelayCommand]
        private void SelectAudioFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            string path = AppDomain.CurrentDomain.BaseDirectory + "Audio"; // this is the path that you are checking.
            if (Directory.Exists(path))
            {
                openFileDialog.InitialDirectory = path;
            }
            openFileDialog.Filter = "Audio File (*.mp3;*.wav)|*.mp3;*.wav;";
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            DisposeWave();
            _filePath = openFileDialog;
            StartAudio();
        }

        [RelayCommand]
        void PlayPauseAudioFile()
        {
            if (output != null && PlayPauseAlarmButton == CurrentAlarm.AlarmId)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing) output.Pause();
                else if (output.PlaybackState == NAudio.Wave.PlaybackState.Paused) output.Play();
                else if (output.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
                {
                    StartAudio();
                }
            }else
            {
                if (CurrentAlarm != null && !string.IsNullOrEmpty(CurrentAlarm.AudioFileLocation))
                {
                    if (output != null && output.PlaybackState == NAudio.Wave.PlaybackState.Playing) output.Stop();
                    PlayPauseAlarmButton = CurrentAlarm.AlarmId;
                    _filePath = new();
                    _filePath.FileName = CurrentAlarm.AudioFileLocation;
                    StartAudio();
                } 
            }
        }

        [RelayCommand]
        private void SaveSchedule()
        {
            if (CurrentSchedule != null && !string.IsNullOrEmpty(CurrentSchedule.ScheduleTitle) && !string.IsNullOrWhiteSpace(CurrentSchedule.ScheduleTitle))
            {
                // Open database (or create if doesn't exist)
                // LiteDatabase(@"Filename=Aljaras.jrsdb;Password='mustafa';connection=shared");
                using (var db = new LiteDatabase(@"Filename=Aljaras.jrsdb;connection=shared"))
                {
                    Schedule _sch = CurrentSchedule;
                    // Get a collection (or create, if doesn't exist)
                    var col = db.GetCollection<Schedule>("Schedules");
                    if (CurrentSchedule.ScheduleId.ToString() == "0")
                    {
                        _sch.ScheduleId = DateTime.Now.Ticks;
                        col.Insert(_sch);
                    }
                    else col.Update(_sch);
                }
            }
            else
            {
                MessageBox.Show("Please Enter a valid value");
            }
            LoadScheduleCollectionData();
            EnableScheduleTitleTB = false;
            EnableCheckBox = false;
            CurrentSchedule = new();
            CallGlobal();
        }

        [RelayCommand]
        private void EditSchedule()
        {
            EnableScheduleTitleTB = true;
            EnableCheckBox = true;
        }

        [RelayCommand]
        private void DeleteAlarm()
        {
            if (CurrentAlarm != null && CurrentAlarm.AlarmId > 0)
            {
                // Open database (or create if doesn't exist)
                using (var db = new LiteDatabase(@"Filename=Aljaras.jrsdb;connection=shared"))
                {
                    var col = db.GetCollection<Alarm>("Alarms");
                    col.Delete(CurrentAlarm.AlarmId);
                    MessageBox.Show("Done");
                    //((MainWindow)System.Windows.Application.Current.MainWindow).RefreshPage();
                }
            }
            else MessageBox.Show("Select alarm to delete");
            LoadAlarmCollectionData(CurrentSchedule.ScheduleId);
            CurrentAlarm = new();
            CallGlobal();
        }

        [RelayCommand]
        private void DeleteSchedule()
        {
            if (CurrentSchedule != null && CurrentSchedule.ScheduleId > 0)
            {
                List<Alarm> ScheduleAlarmList = new();
                using (var db = new LiteDatabase(@"Filename=Aljaras.jrsdb;connection=shared"))
                {
                    // Get a collection (or create, if doesn't exist)
                    var col = db.GetCollection<Alarm>("Alarms");

                    var results = col.Find(x => x.ScheduleId.ToString().Contains(CurrentSchedule.ScheduleId.ToString()));
                    List<Alarm> list = new();
                    list = results.ToList();
                    foreach (var _item in list)
                        col.Delete(_item.AlarmId);
                }

                // Open database (or create if doesn't exist)
                using (var db = new LiteDatabase(@"Filename=Aljaras.jrsdb;connection=shared"))
                {
                    // Get a collection (or create, if doesn't exist)
                    var col = db.GetCollection<Schedule>("Schedules");
                    var results = col.Find(x => x.ScheduleId.ToString().Contains(CurrentSchedule.ScheduleId.ToString()));
                    var rs = results.FirstOrDefault();
                    col.Delete(rs.ScheduleId);
                    MessageBox.Show("Done");
                }
                LoadScheduleCollectionData();
                AlarmList = new();
                IsNOAlarmMessageVisible = "Visible";
            }
            else MessageBox.Show("Select Schedule To Delete");
            CurrentSchedule = new();
            CallGlobal();
        }
        #endregion

        #region Functions
        public AlarmViewModel()
        {
            LoadScheduleCollectionData();
            LoadAlarmCollectionData(CurrentSchedule.ScheduleId);
        }

        partial void OnCurrentScheduleChanged(Schedule value)
        {
            if (CurrentSchedule != null && CurrentSchedule.ScheduleId > 0)
                LoadAlarmCollectionData(CurrentSchedule.ScheduleId);
        }

        private void DisposeWave()
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing) output.Stop();
                output.Dispose();
                output = null;
            }
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }

        void StartAudio()
        {
            if (_filePath.FileName.ToLower().EndsWith(".mp3"))
            {
                NAudio.Wave.WaveStream pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(new NAudio.Wave.Mp3FileReader(_filePath.FileName));
                stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
            }
            else if (_filePath.FileName.ToLower().EndsWith(".wav"))
            {
                NAudio.Wave.WaveStream pcm = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(_filePath.FileName));
                stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
            }
            else MessageBox.Show("Not a correct audio file type.");
            AudioFileName = _filePath.SafeFileName;
            CurrentAlarm.AudioFileLocation = _filePath.FileName;
            output = new NAudio.Wave.DirectSoundOut();
            output.Init(stream);
            output.Play();
        }

        private void LoadScheduleCollectionData()
        {
            ScheduleList = new List<Schedule>();
            using (var db = new LiteDatabase(@"Filename=Aljaras.jrsdb;connection=shared"))
            {
                var col = db.GetCollection<Schedule>("Schedules");
                List<Schedule> list = new List<Schedule>();
                ScheduleList = col.Query().ToList();
            }
            if (ScheduleList != null & ScheduleList?.Count > 0)
            {
                IsNOScheduleMessageVisible = "Hidden";
                //CurrentSchedule = new Schedule();
            }
            else
            {
                IsNOScheduleMessageVisible = "Visible";
                EnableCheckBox = true;
                EnableScheduleTitleTB = true;
            }
        }

        private void LoadAlarmCollectionData(long _SId)
        {
            AlarmList = new();

            using (var db = new LiteDatabase(@"Filename=Aljaras.jrsdb;connection=shared"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Alarm>("Alarms");

                var results = col.Find(x => x.ScheduleId.ToString().Contains(CurrentSchedule.ScheduleId.ToString()));
                List<Alarm> list = new();
                AlarmList = results.ToList().OrderBy(x => x.FullTime).ToList();
            }
            if (AlarmList != null && AlarmList.Count > 0)
                IsNOAlarmMessageVisible = "Hidden";
            else IsNOAlarmMessageVisible = "Visible";
        }
        #endregion

    }
}
