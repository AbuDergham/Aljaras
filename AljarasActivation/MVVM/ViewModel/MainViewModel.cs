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
        private string generatedKey = string.Empty;

        [RelayCommand]
        private void CopyKey() => Clipboard.SetText(ActivationKey);

        [RelayCommand]
        private void PasteKey() => GeneratedKey = Clipboard.GetText();

        partial void OnGeneratedKeyChanged(string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            { ActivationKey = string.Empty; GeneratedKey = string.Empty; }
            else ActivationKey = LicenseKeyGenerator.GenerateLicenseKey(GeneratedKey);
        }

        [ObservableProperty]
        private string activationKey = string.Empty;

        #region Functions
        public MainViewModel()
        {
            ActivationKey = string.Empty;
        }
        #endregion

    }
}
