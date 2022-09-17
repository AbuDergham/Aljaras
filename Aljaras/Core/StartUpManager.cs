using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace Aljaras.Core
{
    public class StartUpManager
    {
        static readonly RegistryKey? CurrentUserKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        static readonly RegistryKey? LocalMachineKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        static readonly RegistryKey? WOW6432NodeKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        
        static readonly string AppLocationWithEXEExtension = !string.IsNullOrEmpty(App.AppName) ? Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".exe") : string.Empty;

        public static void AddApplicationToCurrentUserStartup()
        {
            if (CurrentUserKey != null && !string.IsNullOrEmpty(App.AppName))
            {
                CurrentUserKey.SetValue(App.AppName, AppLocationWithEXEExtension);
                CurrentUserKey.Close();
            }
        }

        public static void AddApplicationToAllUserStartup()
        {
            if (LocalMachineKey != null && !string.IsNullOrEmpty(App.AppName))
            {
                LocalMachineKey.SetValue(App.AppName, AppLocationWithEXEExtension);
                LocalMachineKey.Close();
            }
            if (Environment.Is64BitOperatingSystem && !string.IsNullOrEmpty(App.AppName))
            {
                if (WOW6432NodeKey != null && !string.IsNullOrEmpty(App.AppName))
                {
                    WOW6432NodeKey.SetValue(App.AppName, AppLocationWithEXEExtension);
                    WOW6432NodeKey.Close();
                }
            }
        }

        public static void RemoveApplicationFromCurrentUserStartup()
        {
            if (CurrentUserKey != null && !string.IsNullOrEmpty(App.AppName))
            {
                CurrentUserKey.DeleteValue(App.AppName, false);
                CurrentUserKey.Close();
            }
        }

        public static void RemoveApplicationFromAllUserStartup()
        {
            if (LocalMachineKey != null && !string.IsNullOrEmpty(App.AppName))
            {
                LocalMachineKey.DeleteValue(App.AppName, false);
                LocalMachineKey.Close();
            }
            if (Environment.Is64BitOperatingSystem && !string.IsNullOrEmpty(App.AppName))
            {
                if (WOW6432NodeKey != null && !string.IsNullOrEmpty(App.AppName))
                {
                    WOW6432NodeKey.DeleteValue(App.AppName, false);
                    WOW6432NodeKey.Close();
                }
            }
        }

        public static bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
            }
            catch (Exception)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
    }
}
