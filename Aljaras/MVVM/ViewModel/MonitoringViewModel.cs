using Aljaras.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiteDB;

namespace Aljaras.MVVM.ViewModel
{
    internal partial class MonitoringViewModel : ObservableRecipient
    {
        public GlobalViewModel Global { get; set; } = GlobalViewModel.Instance;
    }
}
