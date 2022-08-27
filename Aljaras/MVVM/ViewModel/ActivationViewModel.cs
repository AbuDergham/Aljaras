using Aljaras.Core;
using Aljaras.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aljaras.MVVM.ViewModel
{
    partial class ActivationViewModel : ObservableRecipient
    {
        public GlobalViewModel Global { get; set; } = GlobalViewModel.Instance;

        [ObservableProperty]
        private string generatedKey = LicenseKeyGenerator.GenerateLicenseKey(Environment.MachineName);

        [ObservableProperty]
        private string activationKey = "";

        [ObservableProperty]
        private bool isActivationEnabled = !LicenseKeyGenerator.isProductActivated();

        [RelayCommand]
        private void SaveKey()
        {
            if(ActivationKey == LicenseKeyGenerator.GenerateLicenseKey(GeneratedKey))
            {
                using (StreamWriter writetext = new("Aljaras.key"))
                {
                    writetext.WriteLine(GeneratedKey);
                    writetext.WriteLine(ActivationKey);
                }
                Global.NotificationMessage = new()
                {
                    BackgroundColor = MessageBackground.MediumSeaGreen.ToString(),
                    MessageText = Global.AppLang.Done
                };
                Global.NotificationList.Add(Global.NotificationMessage);
            }
            else
            {
                Global.NotificationMessage = new()
                {
                    BackgroundColor = MessageBackground.IndianRed.ToString(),
                    MessageText = "Activation Failed"
                };
                Global.NotificationList.Add(Global.NotificationMessage);
            }
        }

        public ActivationViewModel()
        {
            string? keyFile = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.key").FirstOrDefault();
            if (keyFile != null)
            {
                ActivationKey = File.ReadLines(keyFile).ElementAtOrDefault(1);
                if (ActivationKey == LicenseKeyGenerator.GenerateLicenseKey(GeneratedKey))
                { 
                    IsActivationEnabled = false;
                    Global.ProductActivated = GetVisibility.Hidden.ToString(); 
                }
                else
                {
                    Global.NotificationMessage = new()
                    {
                        BackgroundColor = MessageBackground.IndianRed.ToString(),
                        MessageText = "Activation File Corrupted"
                    };
                    Global.NotificationList.Add(Global.NotificationMessage);
                }
            }
                
        }
    }
}
