using Aljaras.Core;
using Aljaras.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aljaras.MVVM.ViewModel
{
    internal partial class HolidaysViewModel : ObservableRecipient
    {
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        #region Observable 
        [ObservableProperty]
        private Holiday currentHoliday = new();

        [ObservableProperty]
        private List<Holiday> holidayList = new();

        [ObservableProperty]
        private string isNOHolidayMessageVisible = GetVisibility.Visible.ToString();

        [ObservableProperty]
        private TimePicker timePicker = new();
        #endregion

        #region RelayCommands
        [RelayCommand]
        private void NewHoliday() { CurrentHoliday = new(); }

        [RelayCommand]
        private void TimeNow()
        {
            DateTime _dt = DateTime.Now;
            CurrentHoliday.ReminderHour = _dt.ToString("hh");
            CurrentHoliday.ReminderMinute = _dt.ToString("mm");
            CurrentHoliday.ReminderDayTime = _dt.ToString("tt");
        }

        [RelayCommand]
        private void CloneHoliday(Holiday obj)
        {
            using (App.db)
            {
                var holidayCol = App.db.GetCollection<Holiday>("Holidays");
                Holiday tmp = obj;
                tmp.HolidayId = DateTime.Now.Ticks;
                tmp.HolidayTitle = obj.HolidayTitle + Global.AppLang.Clone;
                holidayCol.Insert(tmp);
            }
            LoadHolidayCollectionData();
            CallGlobal();
            Global.NotificationMessage = new()
            {
                BackgroundColor = MessageBackground.MediumSeaGreen.ToString(),
                MessageText = Global.AppLang.Done
            };
            GlobalViewModel.Instance.NotificationList.Add(Global.NotificationMessage);
        }

        [RelayCommand]
        void PlayPauseAudioFile()
        {
            if (!Global.AudioOperations.IsEmergency)
                _ = Global.AudioOperations.PlayPauseAudioFile(CurrentHoliday.ReminderAudioFileLocation, false);
        }

        [RelayCommand]
        private void SelectAudioFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            string path = AppDomain.CurrentDomain.BaseDirectory + "Audio";
            if (Directory.Exists(path))
                openFileDialog.InitialDirectory = path;
            openFileDialog.Filter = "Audio File (*.mp3;*.wav)|*.mp3;*.wav;";
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            string newPath = Global.AudioOperations.MoveAudioFileToLibrary(openFileDialog.FileName);
            if (!Global.AudioOperations.IsEmergency)
                _ = Global.AudioOperations.PlayPauseAudioFile(newPath, false);
            CurrentHoliday.ReminderAudioFileLocation = newPath;
        }

        [RelayCommand]
        private void De_ActivateHoliday(Holiday obj)
        {
            using (App.db)
            {
                var holidayCol = App.db.GetCollection<Holiday>("Holidays");
                holidayCol.Update(obj);
            }
            Global.NotificationMessage = new()
            {
                BackgroundColor = MessageBackground.IndianRed.ToString(),
                MessageText = Global.AppLang.Done
            };
            GlobalViewModel.Instance.NotificationList.Add(Global.NotificationMessage);
            CallGlobal();
        }

        [RelayCommand]
        private void DeleteHoliday(Holiday obj)
        {
            using (App.db)
            {
                var col = App.db.GetCollection<Holiday>("Holidays");
                col.Delete(obj.HolidayId);
                Global.NotificationMessage = new()
                {
                    BackgroundColor = MessageBackground.IndianRed.ToString(),
                    MessageText = Global.AppLang.Done
                };
                GlobalViewModel.Instance.NotificationList.Add(Global.NotificationMessage);
            }
            LoadHolidayCollectionData();
            CurrentHoliday = new();
            CallGlobal();
        }

        [RelayCommand]
        private void SaveHoliday()
        {
            if (CurrentHoliday != null && !string.IsNullOrEmpty(CurrentHoliday.HolidayTitle) && !string.IsNullOrWhiteSpace(CurrentHoliday.HolidayTitle))
            {
                var fileLocation = new string[] { CurrentHoliday.ReminderAudioFileLocation, AppDomain.CurrentDomain.BaseDirectory + "Audio\\Attention.mp3" }.FirstOrDefault(s => !string.IsNullOrEmpty(s) && File.Exists(s)) ?? "";
                if (string.IsNullOrEmpty(fileLocation) || string.IsNullOrWhiteSpace(fileLocation))
                {
                    Global.NotificationMessage = new()
                    {
                        BackgroundColor = MessageBackground.IndianRed.ToString(),
                        MessageText = Global.AppLang.NotCorrectAudio
                    };
                    GlobalViewModel.Instance.NotificationList.Add(Global.NotificationMessage);
                    return;
                }
                fileLocation = Global.AudioOperations.MoveAudioFileToLibrary(fileLocation);
                using (App.db)
                {
                    CurrentHoliday.FullTime = ChangeDateOnly(CurrentHoliday.ReminderDate, DateTime.Parse(CurrentHoliday.ReminderHour + ":" + CurrentHoliday.ReminderMinute + " " + CurrentHoliday.ReminderDayTime));
                    CurrentHoliday.ReminderAudioFileLocation = fileLocation;
                    var col = App.db.GetCollection<Holiday>("Holidays");
                    if (CurrentHoliday.HolidayId > 0)
                        col.Update(CurrentHoliday);
                    else
                    {
                        CurrentHoliday.HolidayId = DateTime.Now.Ticks;
                        CurrentHoliday.FullTime = ChangeDateOnly(CurrentHoliday.ReminderDate, DateTime.Parse(CurrentHoliday.ReminderHour + ":" + CurrentHoliday.ReminderMinute + " " + CurrentHoliday.ReminderDayTime));
                        col.Insert(CurrentHoliday);
                    }
                }
                LoadHolidayCollectionData();
                Global.NotificationMessage = new()
                {
                    BackgroundColor = MessageBackground.SeaGreen.ToString(),
                    MessageText = Global.AppLang.Done
                };
                GlobalViewModel.Instance.NotificationList.Add(Global.NotificationMessage);
            }
            else
            {
                Global.NotificationMessage = new()
                {
                    BackgroundColor = MessageBackground.IndianRed.ToString(),
                    MessageText = Global.AppLang.InvalidTitle
                };
                GlobalViewModel.Instance.NotificationList.Add(Global.NotificationMessage);
            }
            CurrentHoliday = new();
            CallGlobal();
        }
        #endregion

        #region Functions
        private void LoadHolidayCollectionData()
        {
            using (App.db)
            {
                var col = App.db.GetCollection<Holiday>("Holidays");
                HolidayList = col.Query().OrderBy(h => h.HolidayDate).ToList();
            }
            if (HolidayList != null && HolidayList.Count > 0)
                IsNOHolidayMessageVisible = GetVisibility.Hidden.ToString();
            else
            {
                IsNOHolidayMessageVisible = GetVisibility.Visible.ToString();
            }
        }

        private static void CallGlobal()
        {
            GlobalViewModel.Instance.LoadMonitoringAlarmCollectionData();
            GlobalViewModel.Instance.NextAlarm();
        }

        public static DateTime ChangeDateOnly(DateTime _date, DateTime _time) { return _date.Date + _time.TimeOfDay; }

        public HolidaysViewModel() { LoadHolidayCollectionData();}
        #endregion

    }
}
