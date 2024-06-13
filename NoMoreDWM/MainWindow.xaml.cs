using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace NoMoreDWM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Check OS version on startup
            if (IsWindows7())
            {
                MessageBox.Show("This program cannot be run on Windows 7 and earlier. This program requires Windows 8 and later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void DisableDWM_Click(object sender, RoutedEventArgs e)
        {
            if (IsDwmRunning())
            {
                try
                {
                    string script = @"
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
reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ""AltTabSettings"" /t REG_DWORD /d ""0"" /f
reg add ""HKCU\Software\Microsoft\Windows\DWM"" /v ""CompositionPolicy"" /t REG_DWORD /d ""0"" /f
reg add ""HKLM\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""ConsoleMode"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""XamlCredUIAvailable"" /t REG_DWORD /d ""0"" /f
reg add ""HKLM\System\CurrentControlSet\Services\Themes"" /v ""Start"" /t REG_DWORD /d ""4"" /f
if exist %systemroot%\System32\dwm.exe takeown /F %systemroot%\System32\dwm.exe /A & icacls %systemroot%\System32\dwm.exe /grant Administrators:(F)
if not exist %systemroot%\System32\dwm.exe.old if exist %systemroot%\System32\dwm.exe ren %systemroot%\System32\dwm.exe dwm.exe.old
if exist %systemroot%\System32\dwm.exe.old if exist %systemroot%\System32\dwm.exe del /q %systemroot%\System32\dwm.exe
echo N| copy/-Y ""%systemroot%\System32\rundll32.exe"" ""%systemroot%\System32\dwm.exe""
";

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
                    string script = @"
if not exist %systemroot%\System32\UiRibbon.dll if exist %systemroot%\System32\UiRibbon.dll.old ren %systemroot%\System32\UiRibbon.dll.old UiRibbon.dll
if exist %systemroot%\System32\UiRibbon.dll if exist %systemroot%\System32\UiRibbon.dll.old del /q %systemroot%\System32\UiRibbon.dll.old
if not exist %systemroot%\System32\UiRibbonRes.dll if exist %systemroot%\System32\UiRibbonRes.dll.old ren %systemroot%\System32\UiRibbonRes.dll.old UiRibbonRes.dll
if exist %systemroot%\System32\UiRibbonRes.dll if exist %systemroot%\System32\UiRibbonRes.dll.old del /q %systemroot%\System32\UiRibbonRes.dll.old
if not exist %systemroot%\System32\Windows.UI.Logon.dll if exist %systemroot%\System32\Windows.UI.Logon.dll.old ren %systemroot%\System32\Windows.UI.Logon.dll.old Windows.UI.Logon.dll
if exist %systemroot%\System32\Windows.UI.Logon.dll if exist %systemroot%\System32\Windows.UI.Logon.dll.old del /q %systemroot%\System32\Windows.UI.Logon.dll.old
if not exist %systemroot%\SystemResources if exist %systemroot%\SystemResources.old ren %systemroot%\SystemResources.old SystemResources
if exist %systemroot%\ShellExperienceHost_cw5n1h2txyewy if exist %systemroot%\SystemResources.old rmdir /S /Q %systemroot%\SystemResources.old
reg delete ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer"" /v ""AltTabSettings"" /f
reg delete ""HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""ConsoleMode"" /f
reg delete ""HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\TestHooks"" /v ""XamlCredUIAvailable"" /f
reg add ""HKCU\Software\Microsoft\Windows\DWM"" /v ""CompositionPolicy"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\SYSTEM\CurrentControlSet\Services\Themes"" /v ""Start"" /t REG_DWORD /d ""2"" /f
del %systemroot%\System32\dwm.exe
if exist %systemroot%\System32\dwm.exe.old takeown /F %systemroot%\System32\dwm.exe.old /A & icacls %systemroot%\System32\dwm.exe.old /grant Administrators:(F)
if not exist %systemroot%\System32\dwm.exe if exist %systemroot%\System32\dwm.exe.old ren %systemroot%\System32\dwm.exe.old dwm.exe
";

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

        private bool IsDwmRunning()
        {
            return Process.GetProcessesByName("dwm").Any();
        }

        private bool IsWindows7()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && os.Version.Major <= 6 && os.Version.Minor <= 1;
        }
    }
}
