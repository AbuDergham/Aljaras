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
        private string generatedKey = "";

        partial void OnGeneratedKeyChanged(string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            { ActivationKey = ""; GeneratedKey = ""; }
            else ActivationKey = LicenseKeyGenerator.GenerateLicenseKey(GeneratedKey);
        }

        [ObservableProperty]
        private string activationKey = "";

        #region Functions
        public MainViewModel()
        {
            ActivationKey = "";
        }
        #endregion

    }
}
