using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Aljaras.MVVM.Model
{
    internal partial class Alarm : ObservableRecipient
    {
        [ObservableProperty]
        private long alarmId = 0;

        [ObservableProperty]
        private string alarmTitle = "";

        [ObservableProperty]
        private long scheduleId = 0;

        [ObservableProperty]
        private bool sun = true;

        [ObservableProperty]
        private bool mon = true;

        [ObservableProperty]
        private bool tue = true;

        [ObservableProperty]
        private bool wed = true;

        [ObservableProperty]
        private bool thu = true;

        [ObservableProperty]
        private bool fri = true;

        [ObservableProperty]
        private bool sat = true;

        [ObservableProperty]
        private string hour = "01";

        [ObservableProperty]
        private string minute = "00";

        [ObservableProperty]
        private string dayTime = "AM";

        [ObservableProperty]
        private DateTime fullTime;

        [ObservableProperty]
        private string audioFileLocation = "";

        [ObservableProperty]
        private bool isAlarmActive = true;
    }
}
