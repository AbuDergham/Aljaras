using AljarasActivation.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Windows;

namespace AljarasActivation.MVVM.ViewModel
{
    internal partial class MainViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private string generatedKey = LicenseKeyGenerator.GenerateLicenseKey(Environment.MachineName);

        [ObservableProperty]
        private string activationKey = "";

        #region Functions
        public MainViewModel()
        {
            ActivationKey = LicenseKeyGenerator.GenerateLicenseKey(GeneratedKey);
        }
        #endregion

    }
}
