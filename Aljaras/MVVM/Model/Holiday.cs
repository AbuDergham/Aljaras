using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aljaras.MVVM.Model
{
    internal partial class Holiday : ObservableRecipient
    {
        [ObservableProperty]
        private long holidayId = 0;

        [ObservableProperty]
        private string holidayTitle = "";

        [ObservableProperty]
        private DateTime holidayDate = DateTime.Now;

        [ObservableProperty]
        private bool isHolidayActive = true;

        [ObservableProperty]
        private bool isReminderActive = false;

        [ObservableProperty]
        private DateTime reminderDate = DateTime.Now;

        [ObservableProperty]
        private string reminderHour = "01";

        [ObservableProperty]
        private string reminderMinute = "00";

        [ObservableProperty]
        private string reminderDayTime = "AM";

        [ObservableProperty]
        private DateTime fullTime;

        [ObservableProperty]
        private string reminderAudioFileLocation = "";
    }
}
