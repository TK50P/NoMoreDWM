# NoMoreDWM – A Simple Program to Disable Desktop Window Manager

<p align="center">
  <img src="https://github.com/user-attachments/assets/c0599679-848c-485b-9e84-7294d4720a73" width="45%" />
  <img src="https://github.com/user-attachments/assets/cbccfc6e-6b19-4db6-b756-f5ec6271ab5e" width="45%" />
</p>

> [!CAUTION]
> **DO NOT RUN** this on your main PC or mission-critical systems.  
This tool directly interacts with DWM behavior and may cause instability.
> 
> Please **do not open GitHub issues** — this is a proof-of-concept and was not developed with full knowledge of DWM internals (no reverse engineering involved).

**Story:**  
Microsoft introduced Windows Aero starting with Windows Vista.  
Windows Aero is a visual style with an emphasis on animation, glass effects, and translucency.  
Up until Windows 7, users could freely toggle Aero on and off (by switching to Windows Basic or Windows Classic themes).  
However, since Windows 8, Microsoft removed the Windows Basic and Windows Classic themes, making it impossible to disable DWM. (If you forcefully kill DWM, it restarts automatically.)  
Unfortunately, DWM has the downside of "forcefully" enabling vertical sync for each window, fixing the frame rate at 60 FPS when playing in windowed mode.  
Even in full-screen mode, this behavior is not ideal for users who play games while multitasking.

> [!IMPORTANT]  
> This tool also addresses the issue of the Blue Screen of Death (BSoD) not appearing in VirtualBox.  
You MUST enable 3D acceleration, and the Video Controller must be set to VBoxVGA.  
This means you need to use VirtualBox 6.0 or earlier.  
Since VirtualBox 6.1, 3D acceleration no longer works with VBoxVGA, and VBoxSVGA doesn't display the BSoD.  
**TL;DR:** VirtualBox 6.1 or later, or if 3D acceleration is disabled and the Video Controller is set to VBoxSVGA, the BSoD won't appear, even if DWM is disabled.  
![image](https://github.com/TK50P/NoMoreDWM/assets/127497974/d9ed49b8-fc8d-44a3-8209-0451d9160311)

> [!WARNING]  
> On Windows 10 and later, hardware acceleration MUST BE DISABLED ON EACH PROGRAM.  
Open-Shell or StartIsBack (in Windows 11, Start**All**Back), ExplorerPatcher must be installed to fix the non-functioning Start menu.
**Taking it a step further, In Windows 11, It's Mandatory.**

**Known Issues:**  
- Settings (including UWP apps) will not work, so you need to use the Control Panel instead. However, the Control Panel lacks many items that have been moved to Settings.  
- In Windows 8/8.1, if you click the network icon in the taskbar, the screen is cut off by 1/3 and does not disappear.  
- Additionally, the Ctrl+Alt+Del menu is black on Windows 8/8.1. (On Windows 10, the system uses a console-based LogonUI.)  
- On Windows 10 versions 1809+ through 22H2, the system may be slightly unstable, and permanently deleting files might not work.  
- There is also a bug where the system often becomes unresponsive once an error occurs. Some apps/programs may not work, even if they are not UWP apps.  
- Processes may remain active even after the window is completely closed.  
- **Note:** These issues do not affect Windows 8 or Windows 11.

![VirtualBox_Windows 8 1_14_06_2024_18_58_45](https://github.com/TK50P/NoMoreDWM/assets/127497974/ff8fbed4-5a32-467f-af3c-1cf40d754004)
