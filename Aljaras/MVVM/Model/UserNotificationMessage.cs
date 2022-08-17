using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Aljaras.MVVM.Model
{
    public partial class UserNotificationMessage : ObservableRecipient
    {
        [ObservableProperty]
        private string _activeMessage = ((MessageVisibility)2).ToString();
        
        [ObservableProperty]
        private string _backgroundColor = ((MessageBackground)1).ToString();

        [ObservableProperty]
        private string _text = "";

        [ObservableProperty]
        private int secondsToShow;
    }
    public enum MessageBackground
    {
        DarkSlateBlue,
        Goldenrod,
        IndianRed,
        LightCoral,
        LightSeaGreen,
        MediumOrchid,
        MediumPurple,
        MediumSeaGreen,
        MediumSlateBlue,
        OliveDrab,
        RoyalBlue,
        SeaGreen,
        SlateBlue,
        SlateGray,
        SteelBlue,
        Teal
    }
    public enum MessageVisibility
    {
        Collapsed,
        Hidden,
        Visible
    }
}
