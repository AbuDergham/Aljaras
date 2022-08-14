using CommunityToolkit.Mvvm.ComponentModel;

namespace Aljaras.MVVM.Model
{
    internal partial class UserNotificationMessage : ObservableRecipient
    {
        [ObservableProperty]
        private bool _activateMessage = false;
        
        [ObservableProperty]
        private string _backgroundColor = "Transparent";

        [ObservableProperty]
        private string _text = "";
    }
}
