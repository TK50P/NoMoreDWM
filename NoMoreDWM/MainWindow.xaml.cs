using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows;

namespace NoMoreDWM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Check if the application is running as administrator
            if (!IsAdministrator())
            {
                MessageBox.Show("You must run this application as an administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            // Check OS version on startup
            if (IsWindows7OrEarlier())
            {
                MessageBox.Show("This program cannot be run on Windows 7 or earlier.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void DisableDWM_Click(object sender, RoutedEventArgs e)
        {
            if (IsWindows8())
            {
                // Show the warning message
                MessageBoxResult result = MessageBox.Show("Disabling DWM will cause these problems:\n" +
                                                          "- Network Flyout no longer works.\n" +
                                                          "- Start Menu no longer works. You MUST use Open Shell to fix the start menu.\n" +
                                                          "- Metro Apps won't work.\n" +
                                                          "- Login Screen won't work. (You MUST remove the user account password.)\n" +
                                                          "- Use Supermium to fix broken titlebars.",
                                                          "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                // If the user chooses 'Cancel', abort the operation
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }

                // Check if the user account has a password
                if (UserHasPassword())
                {
                    MessageBox.Show("The user account has a password. Please remove the password before disabling DWM.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ask for confirmation
                MessageBoxResult confirmationResult = MessageBox.Show("Are you sure you want to proceed?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirmationResult == MessageBoxResult.No)
                {
                    return;
                }
            }

            if (IsWindows10())
            {
                // Show the warning message for Windows 10 (1809+) and later
                MessageBoxResult result = MessageBox.Show("Disabling DWM will cause these problems:\n" +
                                                          "- Network Flyout and sound flyout no longer works. you need to revert windows 7's sound flyout.\n" +
                                                          "- Start Menu no longer works. You MUST use Open Shell to fix the start menu.\n" +
                                                          "- Metro/UWP Apps won't work.\n" +
                                                          "- It will uses Console-based Login screen.\n" +
                                                          "- Use Supermium to fix broken titlebars.\n" +
                                                          "- You need to use classic control panel applets to customize background.\n" +
                                                          "- (1809+) The process may persist even after the window is closed.\n" +
                                                          "- (1809+) Permanently delete files won't work.\n" +
                                                          "- (1809+) Once error occurs, the system may freeze. you need to log off then log in again.\n" +
                                                          "Are you sure to processed?",
                                                          "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                // If the user chooses 'Cancel', abort the operation
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }

                // Ask for confirmation
                MessageBoxResult confirmationResult = MessageBox.Show("Are you sure you want to proceed?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirmationResult == MessageBoxResult.No)
                {
                    return;
                }
            }

            if (IsWindows10())
            {
                // Show the warning message for Windows 10 (1809+) and later
                MessageBoxResult result = MessageBox.Show("Disabling DWM will cause these problems:\n" +
                                                          "- Network Flyout and sound flyout no longer works. you need to revert windows 7's sound flyout.\n" +
                                                          "- Start Menu no longer works. You MUST use Open Shell to fix the start menu.\n" +
                                                          "- Metro/UWP Apps won't work.\n" +
                                                          "- It will uses Console-based Login screen.\n" +
                                                          "- Use Supermium to fix broken titlebars.\n" +
                                                          "- You need to use classic control panel applets to customize background.\n" +
                                                          "Are you sure to processed?",
                                                          "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                // If the user chooses 'Cancel', abort the operation
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }

                // Ask for confirmation
                MessageBoxResult confirmationResult = MessageBox.Show("Are you sure you want to proceed?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirmationResult == MessageBoxResult.No)
                {
                    return;
                }
            }

            if (IsDwmRunning())
            {
                try
                {
                    string script = GetDisableScript();

                    string batFilePath = Path.Combine(Path.GetTempPath(), "DisableDWM.bat");
                    File.WriteAllText(batFilePath, script);

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = batFilePath,
                        UseShellExecute = true,
                        Verb = "runas",
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    Process.Start(psi);

                    MessageBox.Show("DWM disable script executed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Prompt for restart
                    PromptRestart();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("DWM is already disabled.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EnableDWM_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDwmRunning())
            {
                try
                {
                    string script = GetEnableScript();

                    string batFilePath = Path.Combine(Path.GetTempPath(), "EnableDWM.bat");
                    File.WriteAllText(batFilePath, script);

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = batFilePath,
                        UseShellExecute = true,
                        Verb = "runas",
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    Process.Start(psi);

                    MessageBox.Show("DWM enable script executed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Prompt for restart
                    PromptRestart();
                }

                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("DWM is already running.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        private bool UserHasPassword()
        {
            using (var context = new PrincipalContext(ContextType.Machine))
            {
                UserPrincipal user = UserPrincipal.Current;
                return user.LastPasswordSet.HasValue;
            }
        }

        private void PromptRestart()
        {
            MessageBoxResult result = MessageBox.Show("You must restart system to apply changes. Do you want to restart your system now?", "Restart Required", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Process.Start(new ProcessStartInfo("shutdown", "/r /t 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
        }

        private bool IsDwmRunning()
        {
            return Process.GetProcessesByName("dwm").Any();
        }

        private bool IsWindows7OrEarlier()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && (os.Version.Major < 6 || (os.Version.Major == 6 && os.Version.Minor <= 1));
        }

        private bool IsWindows8()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && os.Version.Major == 6 && os.Version.Minor == 2;
        }

        private bool IsWindows81()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && os.Version.Major == 6 && os.Version.Minor == 3;
        }

        private bool IsWindows10()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && os.Version.Major == 10 && os.Version.Minor == 0;
        }

        private bool IsWindows11()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && os.Version.Major == 10 && os.Version.Build >= 22000;
        }

        private string GetDisableScript()
        {
            if (IsWindows8())
            {
                return GetDisableScriptForWindows8();
            }
            else if (IsWindows81())
            {
                return GetDisableScriptForWindows81();
            }
            else if (IsWindows10())
            {
                return GetDisableScriptForWindows10();
            }
            else if (IsWindows11())
            {
                return GetDisableScriptForWindows11();
            }
            return string.Empty;
        }

        private string GetEnableScript()
        {
            if (IsWindows8())
            {
                return GetEnableScriptForWindows8();
            }
            else if (IsWindows81())
            {
                return GetEnableScriptForWindows81();
            }
            else if (IsWindows10())
            {
                return GetEnableScriptForWindows10();
            }
            else if (IsWindows11())
            {
                return GetEnableScriptForWindows11();
            }
            return string.Empty;
        }

        private string GetDisableScriptForWindows8()
        {
            // Add the specific script for Windows 8
            return @"
if exist %systemroot%\ImmersiveControlPanel takeown /F %systemroot%\ImmersiveControlPanel /R /A & icacls %systemroot%\ImmersiveControlPanel /grant Administrators:(F) /T
if exist %systemroot%\System32\UIRibbon.dll takeown /F %systemroot%\System32\UIRibbon.dll /A & icacls %systemroot%\System32\UIRibbon.dll /grant Administrators:(F)
if exist %systemroot%\System32\UIRibbonRes.dll takeown /F %systemroot%\System32\UIRibbonRes.dll /A & icacls %systemroot%\System32\UIRibbonRes.dll /grant Administrators:(F)
if exist %systemroot%\System32\dwm.exe takeown /F %systemroot%\System32\dwm.exe /A & icacls %systemroot%\System32\dwm.exe /grant Administrators:(F)
if exist %systemroot%\System32\UiRibbon.dll takeown /F %systemroot%\System32\UiRibbon.dll /A & icacls %systemroot%\System32\UiRibbon.dll /grant Administrators:(F)
if exist %systemroot%\System32\UiRibbonRes.dll takeown /F %systemroot%\System32\UiRibbonRes.dll /A & icacls %systemroot%\System32\UiRibbonRes.dll /grant Administrators:(F)
if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll takeown /F %systemroot%\System32\windows.immersiveshell.serviceprovider.dll /A & icacls %systemroot%\System32\windows.immersiveshell.serviceprovider.dll /grant Administrators:(F)
if exist %systemroot%\SystemResources takeown /F %systemroot%\SystemResources /R /A & icacls %systemroot%\SystemResources /grant Administrators:(F) /T
taskkill /F /IM ApplicationFrameHost.exe
taskkill /F /IM RuntimeBroker.exe
taskkill /F /IM ShellExperienceHost.exe
taskkill /F /IM SystemSettings.exe
if not exist %systemroot%\System32\UiRibbon.dll.old if exist %systemroot%\System32\UiRibbon.dll ren %systemroot%\System32\UiRibbon.dll UiRibbon.dll.old
if exist %systemroot%\System32\UiRibbon.dll.old if exist %systemroot%\System32\UiRibbon.dll del /q %systemroot%\System32\UiRibbon.dll
if not exist %systemroot%\System32\UiRibbonRes.dll.old if exist %systemroot%\System32\UiRibbonRes.dll ren %systemroot%\System32\UiRibbonRes.dll UiRibbonRes.dll.old
if exist %systemroot%\System32\UiRibbonRes.dll.old if exist %systemroot%\System32\UiRibbonRes.dll del /q %systemroot%\System32\UiRibbonRes.dll
if not exist %systemroot%\SystemResources.old if exist %systemroot%\SystemResources ren %systemroot%\SystemResources SystemResources.old
if exist %systemroot%\SystemResources.old if exist %systemroot%\SystemResources rmdir /S /Q %systemroot%\SystemResources
:: Tweaks for classic UI
reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ""AltTabSettings"" /t REG_DWORD /d ""0"" /f
reg add ""HKCU\Software\Microsoft\Windows\DWM"" /v ""CompositionPolicy"" /t REG_DWORD /d ""0"" /f
reg add ""HKLM\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""ConsoleMode"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""XamlCredUIAvailable"" /t REG_DWORD /d ""0"" /f
reg add ""HKLM\System\CurrentControlSet\Services\Themes"" /v ""Start"" /t REG_DWORD /d ""4"" /f
:: Confuse Windows with a fake dwm.exe
if exist %systemroot%\System32\dwm.exe takeown /F %systemroot%\System32\dwm.exe /A & icacls %systemroot%\System32\dwm.exe /grant Administrators:(F)
if not exist %systemroot%\System32\dwm.exe.old if exist %systemroot%\System32\dwm.exe ren %systemroot%\System32\dwm.exe dwm.exe.old
if exist %systemroot%\System32\dwm.exe.old if exist %systemroot%\System32\dwm.exe del /q %systemroot%\System32\dwm.exe
echo N| copy/-Y ""%systemroot%\System32\rundll32.exe"" ""%systemroot%\System32\dwm.exe""
            ";
        }

        private string GetDisableScriptForWindows81()
        {
            // Add the specific script for Windows 8.1
            return @"
if exist %systemroot%\ImmersiveControlPanel takeown /F %systemroot%\ImmersiveControlPanel /R /A & icacls %systemroot%\ImmersiveControlPanel /grant Administrators:(F) /T
if exist %systemroot%\System32\UIRibbon.dll takeown /F %systemroot%\System32\UIRibbon.dll /A & icacls %systemroot%\System32\UIRibbon.dll /grant Administrators:(F)
if exist %systemroot%\System32\UIRibbonRes.dll takeown /F %systemroot%\System32\UIRibbonRes.dll /A & icacls %systemroot%\System32\UIRibbonRes.dll /grant Administrators:(F)
if exist %systemroot%\System32\dwm.exe takeown /F %systemroot%\System32\dwm.exe /A & icacls %systemroot%\System32\dwm.exe /grant Administrators:(F)
if exist %systemroot%\System32\UiRibbon.dll takeown /F %systemroot%\System32\UiRibbon.dll /A & icacls %systemroot%\System32\UiRibbon.dll /grant Administrators:(F)
if exist %systemroot%\System32\UiRibbonRes.dll takeown /F %systemroot%\System32\UiRibbonRes.dll /A & icacls %systemroot%\System32\UiRibbonRes.dll /grant Administrators:(F)
if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll takeown /F %systemroot%\System32\windows.immersiveshell.serviceprovider.dll /A & icacls %systemroot%\System32\windows.immersiveshell.serviceprovider.dll /grant Administrators:(F)
if exist %systemroot%\SystemResources takeown /F %systemroot%\SystemResources /R /A & icacls %systemroot%\SystemResources /grant Administrators:(F) /T
taskkill /F /IM ApplicationFrameHost.exe
taskkill /F /IM RuntimeBroker.exe
taskkill /F /IM ShellExperienceHost.exe
taskkill /F /IM SystemSettings.exe
if not exist %systemroot%\System32\UiRibbon.dll.old if exist %systemroot%\System32\UiRibbon.dll ren %systemroot%\System32\UiRibbon.dll UiRibbon.dll.old
if exist %systemroot%\System32\UiRibbon.dll.old if exist %systemroot%\System32\UiRibbon.dll del /q %systemroot%\System32\UiRibbon.dll
if not exist %systemroot%\System32\UiRibbonRes.dll.old if exist %systemroot%\System32\UiRibbonRes.dll ren %systemroot%\System32\UiRibbonRes.dll UiRibbonRes.dll.old
if exist %systemroot%\System32\UiRibbonRes.dll.old if exist %systemroot%\System32\UiRibbonRes.dll del /q %systemroot%\System32\UiRibbonRes.dll
if not exist %systemroot%\SystemResources.old if exist %systemroot%\SystemResources ren %systemroot%\SystemResources SystemResources.old
if exist %systemroot%\SystemResources.old if exist %systemroot%\SystemResources rmdir /S /Q %systemroot%\SystemResources
:: Tweaks for classic UI
reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ""AltTabSettings"" /t REG_DWORD /d ""0"" /f
reg add ""HKCU\Software\Microsoft\Windows\DWM"" /v ""CompositionPolicy"" /t REG_DWORD /d ""0"" /f
reg add ""HKLM\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""ConsoleMode"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""XamlCredUIAvailable"" /t REG_DWORD /d ""0"" /f
reg add ""HKLM\System\CurrentControlSet\Services\Themes"" /v ""Start"" /t REG_DWORD /d ""4"" /f
:: Confuse Windows with a fake dwm.exe
if exist %systemroot%\System32\dwm.exe takeown /F %systemroot%\System32\dwm.exe /A & icacls %systemroot%\System32\dwm.exe /grant Administrators:(F)
if not exist %systemroot%\System32\dwm.exe.old if exist %systemroot%\System32\dwm.exe ren %systemroot%\System32\dwm.exe dwm.exe.old
if exist %systemroot%\System32\dwm.exe.old if exist %systemroot%\System32\dwm.exe del /q %systemroot%\System32\dwm.exe
echo N| copy/-Y ""%systemroot%\System32\rundll32.exe"" ""%systemroot%\System32\dwm.exe""
            ";
        }

        private string GetDisableScriptForWindows10()
        {
            // Add the specific script for Windows 10
            return @"
                if exist %systemroot%\ImmersiveControlPanel takeown /F %systemroot%\ImmersiveControlPanel /R /A & icacls %systemroot%\ImmersiveControlPanel /grant Administrators:(F) /T
                if exist %systemroot%\System32\UIRibbon.dll takeown /F %systemroot%\System32\UIRibbon.dll /A & icacls %systemroot%\System32\UIRibbon.dll /grant Administrators:(F)
                if exist %systemroot%\System32\UIRibbonRes.dll takeown /F %systemroot%\System32\UIRibbonRes.dll /A & icacls %systemroot%\System32\UIRibbonRes.dll /grant Administrators:(F)
                if exist %systemroot%\System32\dwm.exe takeown /F %systemroot%\System32\dwm.exe /A & icacls %systemroot%\System32\dwm.exe /grant Administrators:(F)
                if exist %systemroot%\System32\UiRibbon.dll takeown /F %systemroot%\System32\UiRibbon.dll /A & icacls %systemroot%\System32\UiRibbon.dll /grant Administrators:(F)
                if exist %systemroot%\System32\UiRibbonRes.dll takeown /F %systemroot%\System32\UiRibbonRes.dll /A & icacls %systemroot%\System32\UiRibbonRes.dll /grant Administrators:(F)
                if exist %systemroot%\System32\Windows.UI.Logon.dll takeown /F %systemroot%\System32\Windows.UI.Logon.dll /A & icacls %systemroot%\System32\Windows.UI.Logon.dll /grant Administrators:(F)
                if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll takeown /F %systemroot%\System32\windows.immersiveshell.serviceprovider.dll /A & icacls %systemroot%\System32\windows.immersiveshell.serviceprovider.dll /grant Administrators:(F)
                if exist %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy takeown /F %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy /R /A & icacls %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy /grant Administrators:(F) /T
                taskkill /F /IM ApplicationFrameHost.exe
                taskkill /F /IM RuntimeBroker.exe
                taskkill /F /IM ShellExperienceHost.exe
                taskkill /F /IM SystemSettings.exe
                if not exist %systemroot%\ImmersiveControlPanel.old if exist %systemroot%\ImmersiveControlPanel ren %systemroot%\ImmersiveControlPanel ImmersiveControlPanel.old
                if exist %systemroot%\ImmersiveControlPanel.old if exist %systemroot%\ImmersiveControlPanel rmdir /S /Q %systemroot%\ImmersiveControlPanel
                if not exist %systemroot%\System32\UiRibbon.dll.old if exist %systemroot%\System32\UiRibbon.dll ren %systemroot%\System32\UiRibbon.dll UiRibbon.dll.old
                if exist %systemroot%\System32\UiRibbon.dll.old if exist %systemroot%\System32\UiRibbon.dll del /q %systemroot%\System32\UiRibbon.dll
                if not exist %systemroot%\System32\UiRibbonRes.dll.old if exist %systemroot%\System32\UiRibbonRes.dll ren %systemroot%\System32\UiRibbonRes.dll UiRibbonRes.dll.old
                if exist %systemroot%\System32\UiRibbonRes.dll.old if exist %systemroot%\System32\UiRibbonRes.dll del /q %systemroot%\System32\UiRibbonRes.dll
                if not exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll ren %systemroot%\System32\windows.immersiveshell.serviceprovider.dll windows.immersiveshell.serviceprovider.dll.old
                if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll del /q %systemroot%\System32\windows.immersiveshell.serviceprovider.dll
                if not exist %systemroot%\System32\Windows.UI.Logon.dll.old if exist %systemroot%\System32\Windows.UI.Logon.dll ren %systemroot%\System32\Windows.UI.Logon.dll Windows.UI.Logon.dll.old
                if exist %systemroot%\System32\Windows.UI.Logon.dll.old if exist %systemroot%\System32\Windows.UI.Logon.dll del /q %systemroot%\System32\Windows.UI.Logon.dll
                if not exist %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy.old if exist %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy ren %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy ShellExperienceHost_cw5n1h2txyewy.old
                if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy.old if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy rmdir /S /Q %systemroot%\ShellExperienceHost_cw5n1h2txyewy
                :: Tweaks for classic UI
                reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ""AltTabSettings"" /t REG_DWORD /d ""0"" /f
                reg add ""HKCU\Software\Microsoft\Windows\DWM"" /v ""CompositionPolicy"" /t REG_DWORD /d ""0"" /f
                reg add ""HKLM\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""ConsoleMode"" /t REG_DWORD /d ""1"" /f
                reg add ""HKLM\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""XamlCredUIAvailable"" /t REG_DWORD /d ""0"" /f
                :: Confuse Windows with a fake dwm.exe
                if exist %systemroot%\System32\dwm.exe takeown /F %systemroot%\System32\dwm.exe /A & icacls %systemroot%\System32\dwm.exe /grant Administrators:(F)
                if not exist %systemroot%\System32\dwm.exe.old if exist %systemroot%\System32\dwm.exe ren %systemroot%\System32\dwm.exe dwm.exe.old
                if exist %systemroot%\System32\dwm.exe.old if exist %systemroot%\System32\dwm.exe del /q %systemroot%\System32\dwm.exe
                echo N| copy/-Y ""%systemroot%\System32\rundll32.exe"" ""%systemroot%\System32\dwm.exe""
            ";
        }

        private string GetDisableScriptForWindows11()
        {
            // Add the specific script for Windows 11
            return @"
if exist %systemroot%\ImmersiveControlPanel takeown /F %systemroot%\ImmersiveControlPanel /R /A & icacls %systemroot%\ImmersiveControlPanel /grant Administrators:(F) /T
if exist %systemroot%\System32\UIRibbon.dll takeown /F %systemroot%\System32\UIRibbon.dll /A & icacls %systemroot%\System32\UIRibbon.dll /grant Administrators:(F)
if exist %systemroot%\System32\UIRibbonRes.dll takeown /F %systemroot%\System32\UIRibbonRes.dll /A & icacls %systemroot%\System32\UIRibbonRes.dll /grant Administrators:(F)
if exist %systemroot%\System32\dwm.exe takeown /F %systemroot%\System32\dwm.exe /A & icacls %systemroot%\System32\dwm.exe /grant Administrators:(F)
if exist %systemroot%\System32\dwminit.dll takeown /F %systemroot%\System32\dwminit.dll /A & icacls %systemroot%\System32\dwminit.dll /grant Administrators:(F)
if exist %systemroot%\System32\Windows.UI.Search.dll takeown /F %systemroot%\System32\Windows.UI.Search.dll /A & icacls %systemroot%\System32\Windows.UI.Search.dll /grant Administrators:(F)
if exist %systemroot%\System32\Windows.Shell.Search.UriHandler.dll takeown /F %systemroot%\System32\Windows.Shell.Search.UriHandler.dll /A & icacls %systemroot%\System32\Windows.Shell.Search.UriHandler.dll /grant Administrators:(F)
if exist %systemroot%\System32\Windows.UI.Logon.dll takeown /F %systemroot%\System32\Windows.UI.Logon.dll /A & icacls %systemroot%\System32\Windows.UI.Logon.dll /grant Administrators:(F)
if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll takeown /F %systemroot%\System32\windows.immersiveshell.serviceprovider.dll /A & icacls %systemroot%\System32\windows.immersiveshell.serviceprovider.dll /grant Administrators:(F)
if exist %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy takeown /F %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy /R /A & icacls %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy /grant Administrators:(F) /T
taskkill /F /IM ApplicationFrameHost.exe
taskkill /F /IM RuntimeBroker.exe
taskkill /F /IM ShellExperienceHost.exe
taskkill /F /IM SystemSettings.exe
if not exist %systemroot%\ImmersiveControlPanel.old if exist %systemroot%\ImmersiveControlPanel ren %systemroot%\ImmersiveControlPanel ImmersiveControlPanel.old
if exist %systemroot%\ImmersiveControlPanel.old if exist %systemroot%\ImmersiveControlPanel rmdir /S /Q %systemroot%\ImmersiveControlPanel
if not exist %systemroot%\System32\dwminit.dll.old if exist %systemroot%\System32\dwminit.dll ren %systemroot%\System32\dwminit.dll dwminit.dll.old
if exist %systemroot%\System32\dwminit.dll.old if exist %systemroot%\System32\dwminit.dll del /q %systemroot%\System32\dwminit.dll
if not exist %systemroot%\System32\UiRibbon.dll.old if exist %systemroot%\System32\UiRibbon.dll ren %systemroot%\System32\UiRibbon.dll UiRibbon.dll.old
if exist %systemroot%\System32\UiRibbon.dll.old if exist %systemroot%\System32\UiRibbon.dll del /q %systemroot%\System32\UiRibbon.dll
if not exist %systemroot%\System32\UiRibbonRes.dll.old if exist %systemroot%\System32\UiRibbonRes.dll ren %systemroot%\System32\UiRibbonRes.dll UiRibbonRes.dll.old
if exist %systemroot%\System32\UiRibbonRes.dll.old if exist %systemroot%\System32\UiRibbonRes.dll del /q %systemroot%\System32\UiRibbonRes.dll
if not exist %systemroot%\System32\Windows.UI.Search.dll.old if exist %systemroot%\System32\dwminit.dll ren %systemroot%\System32\Windows.UI.Search.dll Windows.UI.Search.dll.old
if exist %systemroot%\System32\Windows.UI.Search.dll.old if exist %systemroot%\System32\Windows.UI.Search.dll del /q %systemroot%\System32\Windows.UI.Search.dll
if not exist %systemroot%\System32\Windows.Shell.Search.UriHandler.dll.old if exist %systemroot%\System32\Windows.Shell.Search.UriHandler.dll ren %systemroot%\System32\Windows.Shell.Search.UriHandler.dll Windows.Shell.Search.UriHandler.dll.old
if exist %systemroot%\System32\Windows.Shell.Search.UriHandler.dll.old if exist %systemroot%\System32\Windows.Shell.Search.UriHandler.dll del /q %systemroot%\System32\Windows.Shell.Search.UriHandler.dll
if not exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll ren %systemroot%\System32\windows.immersiveshell.serviceprovider.dll windows.immersiveshell.serviceprovider.dll.old
if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll del /q %systemroot%\System32\windows.immersiveshell.serviceprovider.dll
if not exist %systemroot%\System32\Windows.UI.Logon.dll.old if exist %systemroot%\System32\Windows.UI.Logon.dll ren %systemroot%\System32\Windows.UI.Logon.dll Windows.UI.Logon.dll.old
if exist %systemroot%\System32\Windows.UI.Logon.dll.old if exist %systemroot%\System32\Windows.UI.Logon.dll del /q %systemroot%\System32\Windows.UI.Logon.dll
if not exist %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy.old if exist %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy ren %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy ShellExperienceHost_cw5n1h2txyewy.old
if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy.old if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy rmdir /S /Q %systemroot%\ShellExperienceHost_cw5n1h2txyewy
:: Tweaks for classic UI
reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ""AltTabSettings"" /t REG_DWORD /d ""0"" /f
reg add ""HKCU\Software\Microsoft\Windows\DWM"" /v ""CompositionPolicy"" /t REG_DWORD /d ""0"" /f
reg add ""HKLM\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""ConsoleMode"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""XamlCredUIAvailable"" /t REG_DWORD /d ""0"" /f
reg add ""HKLM\System\CurrentControlSet\Services\Themes"" /v ""Start"" /t REG_DWORD /d ""4"" /f
:: Confuse Windows with a fake dwm.exe
if exist %systemroot%\System32\dwm.exe takeown /F %systemroot%\System32\dwm.exe /A & icacls %systemroot%\System32\dwm.exe /grant Administrators:(F)
if not exist %systemroot%\System32\dwm.exe.old if exist %systemroot%\System32\dwm.exe ren %systemroot%\System32\dwm.exe dwm.exe.old
if exist %systemroot%\System32\dwm.exe.old if exist %systemroot%\System32\dwm.exe del /q %systemroot%\System32\dwm.exe
echo N| copy/-Y ""%systemroot%\System32\rundll32.exe"" ""%systemroot%\System32\dwm.exe""
            ";
        }

        private string GetEnableScriptForWindows8()
        {
            // Add the specific script for Windows 8
            return @"
if not exist %systemroot%\System32\UiRibbon.dll if exist %systemroot%\System32\UiRibbon.dll.old ren %systemroot%\System32\UiRibbon.dll.old UiRibbon.dll
if exist %systemroot%\System32\UiRibbon.dll if exist %systemroot%\System32\UiRibbon.dll.old del /q %systemroot%\System32\UiRibbon.dll.old
if not exist %systemroot%\System32\UiRibbonRes.dll if exist %systemroot%\System32\UiRibbonRes.dll.old ren %systemroot%\System32\UiRibbonRes.dll.old UiRibbonRes.dll
if exist %systemroot%\System32\UiRibbonRes.dll if exist %systemroot%\System32\UiRibbonRes.dll.old del /q %systemroot%\System32\UiRibbonRes.dll.old
if not exist %systemroot%\System32\Windows.UI.Logon.dll if exist %systemroot%\System32\Windows.UI.Logon.dll.old ren %systemroot%\System32\Windows.UI.Logon.dll.old Windows.UI.Logon.dll
if exist %systemroot%\System32\Windows.UI.Logon.dll if exist %systemroot%\System32\Windows.UI.Logon.dll.old del /q %systemroot%\System32\Windows.UI.Logon.dll.old
if not exist %systemroot%\SystemResources if exist %systemroot%\SystemResources.old ren %systemroot%\SystemResources.old SystemResources
if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy if exist %systemroot%\SystemResources.old rmdir /S /Q %systemroot%\SystemResources.old
:: Revert classic UI tweaks
reg delete ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ""AltTabSettings"" /f
reg delete ""HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""ConsoleMode"" /f
reg delete ""HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""XamlCredUIAvailable"" /f
reg add ""HKCU\Software\Microsoft\Windows\DWM"" /v ""CompositionPolicy"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\SYSTEM\CurrentControlSet\Services\Themes"" /v ""Start"" /t REG_DWORD /d ""2"" /f
:: Revert DWM.exe changes
del %systemroot%\System32\dwm.exe
if exist %systemroot%\System32\dwm.exe.old takeown /F %systemroot%\System32\dwm.exe.old /A & icacls %systemroot%\System32\dwm.exe.old /grant Administrators:(F)
if not exist %systemroot%\System32\dwm.exe if exist %systemroot%\System32\dwm.exe.old ren %systemroot%\System32\dwm.exe.old dwm.exe
            ";
        }

        private string GetEnableScriptForWindows81()
        {
            // Add the specific script for Windows 8.1
            return @"
if not exist %systemroot%\System32\UiRibbon.dll if exist %systemroot%\System32\UiRibbon.dll.old ren %systemroot%\System32\UiRibbon.dll.old UiRibbon.dll
if exist %systemroot%\System32\UiRibbon.dll if exist %systemroot%\System32\UiRibbon.dll.old del /q %systemroot%\System32\UiRibbon.dll.old
if not exist %systemroot%\System32\UiRibbonRes.dll if exist %systemroot%\System32\UiRibbonRes.dll.old ren %systemroot%\System32\UiRibbonRes.dll.old UiRibbonRes.dll
if exist %systemroot%\System32\UiRibbonRes.dll if exist %systemroot%\System32\UiRibbonRes.dll.old del /q %systemroot%\System32\UiRibbonRes.dll.old
if not exist %systemroot%\System32\Windows.UI.Logon.dll if exist %systemroot%\System32\Windows.UI.Logon.dll.old ren %systemroot%\System32\Windows.UI.Logon.dll.old Windows.UI.Logon.dll
if exist %systemroot%\System32\Windows.UI.Logon.dll if exist %systemroot%\System32\Windows.UI.Logon.dll.old del /q %systemroot%\System32\Windows.UI.Logon.dll.old
if not exist %systemroot%\SystemResources if exist %systemroot%\SystemResources.old ren %systemroot%\SystemResources.old SystemResources
if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy if exist %systemroot%\SystemResources.old rmdir /S /Q %systemroot%\SystemResources.old
:: Revert classic UI tweaks
reg delete ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ""AltTabSettings"" /f
reg delete ""HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""ConsoleMode"" /f
reg delete ""HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""XamlCredUIAvailable"" /f
reg add ""HKCU\Software\Microsoft\Windows\DWM"" /v ""CompositionPolicy"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\SYSTEM\CurrentControlSet\Services\Themes"" /v ""Start"" /t REG_DWORD /d ""2"" /f
:: Revert DWM.exe changes
del %systemroot%\System32\dwm.exe
if exist %systemroot%\System32\dwm.exe.old takeown /F %systemroot%\System32\dwm.exe.old /A & icacls %systemroot%\System32\dwm.exe.old /grant Administrators:(F)
if not exist %systemroot%\System32\dwm.exe if exist %systemroot%\System32\dwm.exe.old ren %systemroot%\System32\dwm.exe.old dwm.exe

            ";
        }

        private string GetEnableScriptForWindows10()
        {
            // Add the specific script for Windows 10
            return @"
                if not exist %systemroot%\ImmersiveControlPanel if exist %systemroot%\ImmersiveControlPanel.old ren %systemroot%\ImmersiveControlPanel.old ImmersiveControlPanel
                if exist %systemroot%\ImmersiveControlPanel if exist %systemroot%\ImmersiveControlPanel.old rmdir /S /Q %systemroot%\ImmersiveControlPanel.old
                if not exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old ren %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old windows.immersiveshell.serviceprovider.dll
                if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old del /q %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old
                if not exist %systemroot%\System32\UiRibbon.dll if exist %systemroot%\System32\UiRibbon.dll.old ren %systemroot%\System32\UiRibbon.dll.old UiRibbon.dll
                if exist %systemroot%\System32\UiRibbon.dll if exist %systemroot%\System32\UiRibbon.dll.old del /q %systemroot%\System32\UiRibbon.dll.old
                if not exist %systemroot%\System32\UiRibbonRes.dll if exist %systemroot%\System32\UiRibbonRes.dll.old ren %systemroot%\System32\UiRibbonRes.dll.old UiRibbonRes.dll
                if exist %systemroot%\System32\UiRibbonRes.dll if exist %systemroot%\System32\UiRibbonRes.dll.old del /q %systemroot%\System32\UiRibbonRes.dll.old
                if not exist %systemroot%\System32\Windows.UI.Logon.dll if exist %systemroot%\System32\Windows.UI.Logon.dll.old ren %systemroot%\System32\Windows.UI.Logon.dll.old Windows.UI.Logon.dll
                if exist %systemroot%\System32\Windows.UI.Logon.dll if exist %systemroot%\System32\Windows.UI.Logon.dll.old del /q %systemroot%\System32\Windows.UI.Logon.dll.old
                if not exist %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy if exist %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy.old ren %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy.old ShellExperienceHost_cw5n1h2txyewy
                if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy.old rmdir /S /Q %systemroot%\ShellExperienceHost_cw5n1h2txyewy.old
                :: Revert classic UI tweaks
                reg delete ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ""AltTabSettings"" /f
                reg delete ""HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""ConsoleMode"" /f
                reg delete ""HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""XamlCredUIAvailable"" /f
                reg add ""HKCU\Software\Microsoft\Windows\DWM"" /v ""CompositionPolicy"" /t REG_DWORD /d ""1"" /f
                :: Revert DWM.exe changes
                del %systemroot%\System32\dwm.exe
                if exist %systemroot%\System32\dwm.exe.old takeown /F %systemroot%\System32\dwm.exe.old /A & icacls %systemroot%\System32\dwm.exe.old /grant Administrators:(F)
                if not exist %systemroot%\System32\dwm.exe if exist %systemroot%\System32\dwm.exe.old ren %systemroot%\System32\dwm.exe.old dwm.exe
            ";
        }

        private string GetEnableScriptForWindows11()
        {
            // Add the specific script for Windows 11
            return @"
if not exist %systemroot%\ImmersiveControlPanel if exist %systemroot%\ImmersiveControlPanel.old ren %systemroot%\ImmersiveControlPanel.old ImmersiveControlPanel
if exist %systemroot%\ImmersiveControlPanel if exist %systemroot%\ImmersiveControlPanel.old rmdir /S /Q %systemroot%\ImmersiveControlPanel.old
if not exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old ren %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old windows.immersiveshell.serviceprovider.dll
if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll if exist %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old del /q %systemroot%\System32\windows.immersiveshell.serviceprovider.dll.old
if not exist %systemroot%\System32\dwminit.dll if exist %systemroot%\System32\dwminit.dll.old ren %systemroot%\System32\dwminit.dll.old dwminit.dll
if exist %systemroot%\System32\dwminit.dll if exist %systemroot%\System32\dwminit.dll.old del /q %systemroot%\System32\dwminit.dll.old
if not exist %systemroot%\System32\UiRibbon.dll if exist %systemroot%\System32\UiRibbon.dll.old ren %systemroot%\System32\UiRibbon.dll.old UiRibbon.dll
if exist %systemroot%\System32\UiRibbon.dll if exist %systemroot%\System32\UiRibbon.dll.old del /q %systemroot%\System32\UiRibbon.dll.old
if not exist %systemroot%\System32\UiRibbonRes.dll if exist %systemroot%\System32\UiRibbonRes.dll.old ren %systemroot%\System32\UiRibbonRes.dll.old UiRibbonRes.dll
if exist %systemroot%\System32\UiRibbonRes.dll if exist %systemroot%\System32\UiRibbonRes.dll.old del /q %systemroot%\System32\UiRibbonRes.dll.old
if not exist %systemroot%\System32\Windows.UI.Search.dll if exist %systemroot%\System32\Windows.UI.Search.dll.old ren %systemroot%\System32\Windows.UI.Search.dll.old Windows.UI.Search.dll
if exist %systemroot%\System32\Windows.UI.Search.dll if exist %systemroot%\System32\Windows.UI.Search.dll.old del /q %systemroot%\System32\Windows.UI.Search.dll.old
if not exist %systemroot%\System32\Windows.Shell.Search.UriHandler.dll if exist %systemroot%\System32\Windows.Shell.Search.UriHandler.dll.old ren %systemroot%\System32\Windows.Shell.Search.UriHandler.dll.old Windows.Shell.Search.UriHandler.dll
if exist %systemroot%\System32\Windows.Shell.Search.UriHandler.dll if exist %systemroot%\System32\Windows.Shell.Search.UriHandler.dll.old del /q %systemroot%\System32\Windows.Shell.Search.UriHandler.dll.old
if not exist %systemroot%\System32\Windows.UI.Logon.dll if exist %systemroot%\System32\Windows.UI.Logon.dll.old ren %systemroot%\System32\Windows.UI.Logon.dll.old Windows.UI.Logon.dll
if exist %systemroot%\System32\Windows.UI.Logon.dll if exist %systemroot%\System32\Windows.UI.Logon.dll.old del /q %systemroot%\System32\Windows.UI.Logon.dll.old
if not exist %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy if exist %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy.old ren %systemroot%\SystemApps\ShellExperienceHost_cw5n1h2txyewy.old ShellExperienceHost_cw5n1h2txyewy
if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy.old rmdir /S /Q %systemroot%\ShellExperienceHost_cw5n1h2txyewy.old
:: Revert classic UI tweaks
reg delete ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ""AltTabSettings"" /f
reg delete ""HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""ConsoleMode"" /f
reg delete ""HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""XamlCredUIAvailable"" /f
reg add ""HKCU\Software\Microsoft\Windows\DWM"" /v ""CompositionPolicy"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\SYSTEM\CurrentControlSet\Services\Themes"" /v ""Start"" /t REG_DWORD /d ""2"" /f
:: Revert DWM.exe changes
del %systemroot%\System32\dwm.exe
if exist %systemroot%\System32\dwm.exe.old takeown /F %systemroot%\System32\dwm.exe.old /A & icacls %systemroot%\System32\dwm.exe.old /grant Administrators:(F)
if not exist %systemroot%\System32\dwm.exe if exist %systemroot%\System32\dwm.exe.old ren %systemroot%\System32\dwm.exe.old dwm.exe
            ";
        }
    }
}
