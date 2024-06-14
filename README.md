# NoMoreDWM – A simple program to disable Desktop Window Manager

![image](https://github.com/TK50P/NoMoreDWM/assets/127497974/2239eb9e-95a0-457b-86ca-ccdc7c4d9059)

Story: <br>
Microsoft introduced Windows Aero and starting with Windows Vista. <br>
Windows Aero is a visual style with an emphasis on animation, glass, and translucency. <br>
Until Windows 7, you could freely turn Aero on and off (Windows basic or Windows classic). <br>
But since Windows 8, they removed Windows Basic and Windows Classic and made it impossible to disable DWM. (if you force kill DWM, it restarts soon) <br>
However, DWM has the problem of "forcefully" activating vertical sync for each window, so the frame rate is fixed at 60 when playing in windowed mode. <br>
Even in full screen mode, it is not suitable for people who play games while doing other tasks.

This tool also fixes Blue Screen of Death (BSoD) is not showing on VirtualBox. <br>
You MUST Enabled 3D acceleration and Video Controller is using VBoxVGA. <br>
That means you need to use VirtualBox 6.0 and earlier. <br>
Since VirtualBox 6.1 and later, 3D acceleration no longer works with VBoxVGA. But VBoxSVGA doesn't shows BSoD. <br>
TL;DR: VirtualBox 6.1 or 3D acceleration has been disabled and if Video Controller is VBoxSVGA, it won't shows BSoD even it's disabled. <br>
This is example of BSoD with VirtualBox 5.2.38 (Uses VBoxVGA by default until 5.2), 3D acceleration has been enabled.
![image](https://github.com/TK50P/NoMoreDWM/assets/127497974/d9ed49b8-fc8d-44a3-8209-0451d9160311)



**TBD**

Note Windows 10 and later hardware accleration MUST BE DISABLED ON EACH PROGRAMS. <br>
Open shell or startisback must be installed to fix start menu not working.

**In Windows 11, ExplorerPatcher or StartAllBack MUST BE INSTALLED.**

This script works up to Windows 11.
Mouse pointer not showing and broken Ribbon fixed. <br>

Known Issues: <br>
Settings (includes UWP) not work. so, you need to use Control Panel instead. but there's have LACK control panel items. many items are moved to settings. <br>
In Windows 8/8.1, if you click network icon in the taskbar, the screen is cut off by 1/3 and does not disappear again. <br>
Also, Ctrl+Alt+Del menu is black on Windows 8/8.1. (in Windows 10 will use console-based logonUI) <br>

in Windows 10 1809+ to Windows 10 22H2 can be slightly unstable and unable to permanentely delete files. <br>
And, There is a bug where the system often becomes unresponsive once an error occurs. some apps/programs not working even not UWP. <br>
Processes may also remain even the window is completely closed. <br>
This issue won't affect in Windows 8 and Windows 11.
![VirtualBox_Windows 8 1_14_06_2024_18_58_45](https://github.com/TK50P/NoMoreDWM/assets/127497974/ff8fbed4-5a32-467f-af3c-1cf40d754004)

