using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace Aljaras.MVVM.Model
{
    internal partial class TimePicker : ObservableRecipient
    {
        [ObservableProperty]
        private List<string> hour = new();

        [ObservableProperty]
        private List<string> minute = new();

        [ObservableProperty]
        private List<string> dayTime = new() { "AM", "PM" };
        
        public TimePicker()
        {
            for (int i = 1; i < 13; ++i) {Hour.Add(i.ToString().PadLeft(2, '0'));}
            for (int i = 0; i < 60; ++i) {Minute.Add(i.ToString().PadLeft(2, '0'));}
        }
    }
}
