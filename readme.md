# Lightning's Tools

***
## Releases

### [End User Applications](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications)

#### [ADI Test Tool](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/ADI%20Test%20Tool/Version%200.1.0.0/ADITestTool_v0_1_0_0__x86.zip)
Desktop application for testing Henkie's F-16 ADI Support Board with a real ARU-42/A Attitude Director Indicator instrument.

#### [Analog Devices AD536x-AD537x Eval Board Test Tool](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/Analog%20Devices%20AD536x-AD537x%20Eval%20Board%20Test%20Tool/Version%200.1.0.0/AnalogDevicesAD536xAD537xEvalBoardTestTool_v0_1_0_0.zip)
Desktop application for testing Analog Devices AD536x and AD537x digital-to-analog converter evaluation boards [providing +/-10VDC analog outputs in order to drive military-grade simulated flight instruments]. Demonstrates the use of the `AnalogDevices` class library.

#### [F16 Center Pedestal Display](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/F16%20Center%20Pedestal%20Display/Version%200.5.8.1/F16CPD_v0_5_8_1__x86.zip)
Desktop application providing a semi-realistic simulation of the Raytheon F-16 Center Pedestal Display for use with Falcon BMS.  

#### [Falcon BMS Keyfile Viewer](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/Falcon%20BMS%20Keyfile%20Viewer/Version%200.1.0.0/Falcon_BMS_Keyfile_Viewer_v0_1_0_0__x86.zip)
Desktop application for viewing Falcon .key files.  Demonstrates the use of the `F4KeyFile` library.

#### [Falcon BMS Shared Memory Mirror](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/Falcon%20BMS%20Shared%20Memory%20Mirror/for%20Falcon%20BMS%204.34/F4SharedMemMirror_for_Falcon_BMS_4_34__x86.zip)
Desktop client-server application that mirrors the contents of Falcon's shared memory areas to one or more remote networked PCs.

#### [Falcon BMS Shared Memory Recorder](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/Falcon%20BMS%20Shared%20Memory%20Recorder/for%20Falcon%20BMS%204.34/Shared_Memory_Recorder_for_Falcon_BMS_4_34__x86.zip)
Desktop application that enables recording and playback of Falcon shared memory data.

#### [Falcon BMS Shared Memory Viewer](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/Falcon%20BMS%20Shared%20Memory%20Viewer/for%20Falcon%20BMS%204.34/Shared_Memory_Viewer_for_Falcon_BMS_4_34__x86.zip)
Desktop application that displays the contents of Falcon's shared memory areas.

#### [Falcon BMS Textures Shared Memory Tester](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/Falcon%20BMS%20Textures%20Shared%20Memory%20Tester/Version%200.1.4.0/Falcon_BMS_Textures_Shared_Memory_Tester_v0_1_4_0__x86.zip)
Desktop application for displaying the contents of Falcon BMS' "Textures Shared Memory" area.   Demonstrates the use of the `F4TexSharedMem` library.

#### [MFDExtractor](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/Falcon%20MFD%20Extractor/Version%200.6.3.0/Falcon_MFD_Extractor_v0_6_3_0__x86.zip)
End user client/server application that allows extracting various flight instruments from Falcon.    

#### [JoyMapper](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/JoyMapper/Version%200.4.1.0/JoyMapper_v0_4_1_0__x86_Setup.zip)
Desktop application for remapping analog and digital inputs from DirectInput devices, BetaInnovations non-Joystick-class HID devices, and PHCC devices using PPJoy virtual joystick drivers.  

#### [PHCC Test Tool](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/PHCC%20Test%20Tool/v0.1.2.0/PHCC_Test_Tool_v0_1_2_0__x86.zip)
End-user desktop application providing basic testing capabilities for the PHCC motherboard and attached peripherals.  Demonstrates the use of the `PHCC` class library.

#### [SDI Test Tool](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/SDI%20Test%20Tool/Version%200.1.0.0/SDITestTool_v0_1_0_0__x86.zip)
End-user desktop application for testing Henkie's Digital-to-Synchro (SDI) interface card. Demonstrates the use of the `SDI` class library.

#### [TlkTool](https://github.com/lightningviper/lightningstools/raw/master/releases/End%20User%20Applications/TlkTool/TlkTool_v0_2_4_0__x86.zip)
End-user command-line (console) application which provides the ability to edit Falcon's AI comms databases (falcon.TLK, commFile.bin, fragFile.bin, and evalFile.bin files).  
- Supports decompressing and extracting any/all of the individual compressed audio files from within the .TLK file, to ordinary .WAV files or compressed .LH or .SPX files.  
- Supports exporting the commFile.bin, fragFile.bin, and evalFile.bin files to plain colon-delimited text files, which can then be edited in any text editor or spreadsheet.  
- Supports regenerating the commFile.bin, fragFile.bin, and evalFile.bin files from these plain text files.  
- Supports creating or regenerating a .TLK file from a set of .WAV, .LH, or .SPX files and updating individual entries in an existing .TLK file

***


# Building the Code
##### Installing Prerequisites and Checking out the Code
To clone the `lightningstools` repository and install all required prerequisites in a single command, run the following command from an **administrative** command prompt:
```
bitsadmin /transfer LightningsToolsBootstrap /dynamic /download /priority HIGH "https://raw.githubusercontent.com/lightningviper/lightningstools/master/src/MasterBuild/up.bat" "%temp%\up.bat" & "%temp%\up.bat"
```

The above command will clone the repository to a local folder, `\lightningstools`, located in the root of the current drive where the above command was run from (typically, this folder will be located at `c:\lightningstools`).  
  
The following prerequisites will be installed:  
- Git for Windows
- Visual Studio 2019 Community Edition  
- Visual Studio Installer Project Add-In  
- .NET Framework 4.8 (required by all projects)  
- Command-line Build tools
- Windows 10 SDK version 10.0.10586.0 
- TortoiseGit
  
The above command will also restore all required NuGet packages for all projects and solutions in the repository.
  

##### How to Build Specific Projects/Solutions:
From an **administrative command prompt**, run the following command:  
```
\lightningstools\src\MasterBuild\Build.bat <path to solution>
```

Replace `<path to solution>` with the path to the particular `.sln` of the solution you want to build.   

For example, to build MFD Extractor, execute:  
```
\lightningstools\src\MasterBuild\Build.bat \lightningstools\src\MFDExtractor\MFDExtractor.sln
```

##### How to Build All Projects/Solutions:
From an **administrative command prompt**, run the following command:  
```
\lightningstools\src\MasterBuild\Build.bat
```


##### Where to Find Build Outputs
Build outputs will typically be located underneath the solution folder for the specific solution(s) you are building, in 
`<solution folder>\bin\<processor architecture>\Debug`  
and 
`<solution folder>\bin\<processor architecture>\Release`  

***
# Projects 
[AnalogDevices](https://github.com/lightningviper/lightningstools/tree/master/src/AnalogDevices/)  
C# class library that provides programmatic control over the Analog Devices AD536x and AD537x digital-to-analog converter evaluation boards [providing +/-10VDC analog outputs in order to drive military-grade simulated flight instruments]

[BIUSBWrapper](https://github.com/lightningviper/lightningstools/tree/master/src/BIUSBWrapper/)  
C# class library which wraps the native (C++) BetaInnovations USB SDK, allowing the Beta Innovations USB SDK to be called easily from within managed (.NET) code.

[Common](https://github.com/lightningviper/lightningstools/tree/master/src/Common/)  
C# class libraries containing functionality which is utilized across various other libraries and applications in this repository.  
_Namespaces_:  
    - [Common.Application](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Application/)  
    - [Common.Collections](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Collections/)  
    - [Common.Compression](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Compression/)  
    - [Common.Drawing](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Drawing/)  
    - [Common.Exceptions](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Exceptions/)  
    - [Common.Generic](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Generic/)  
    - [Common.HardwareSupport](https://github.com/lightningviper/lightningstools/tree/master/src/Common/HardwareSupport/)  
    - [Common.IO](https://github.com/lightningviper/lightningstools/tree/master/src/Common/IO/)  
    - [Common.Imaging](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Imaging/)  
    - [Common.InputSupport](https://github.com/lightningviper/lightningstools/tree/master/src/Common/InputSupport/)  
    - [Common.InputSupport.BetaInnovations](https://github.com/lightningviper/lightningstools/tree/master/src/Common/InputSupport/BetaInnovations/)  
    - [Common.InputSupport.DirectInput](https://github.com/lightningviper/lightningstools/tree/master/src/Common/InputSupport/DirectInput/)  
    - [Common.InputSupport.Phcc](https://github.com/lightningviper/lightningstools/tree/master/src/Common/InputSupport/Phcc/)  
    - [Common.MacroProgramming](https://github.com/lightningviper/lightningstools/tree/master/src/Common/MacroProgramming/)  
    - [Common.Math](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Math/)  
    - [Common.Networking](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Networking/)  
    - [Common.Reflection](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Reflection/)  
    - [Common.Serialization](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Serialization/)  
    - [Common.SimSupport](https://github.com/lightningviper/lightningstools/tree/master/src/Common/SimSupport/)  
    - [Common.Statistics](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Statistics/)  
    - [Common.Strings](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Strings/)  
    - [Common.ThirdParty](https://github.com/lightningviper/lightningstools/tree/master/src/Common/ThirdParty/)  
    - [Common.Threading](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Threading/)  
    - [Common.UI](https://github.com/lightningviper/lightningstools/tree/master/src/Common/UI/)  
    - [Common.Win32](https://github.com/lightningviper/lightningstools/tree/master/src/Common/Win32/)  

[F16CPD](https://github.com/lightningviper/lightningstools/tree/master/src/F16CPD/)  
Desktop application providing a semi-realistic simulation of the Raytheon F-16 Center Pedestal Display for use with Falcon BMS.  

[F4KeyFile](https://github.com/lightningviper/lightningstools/tree/master/src/F4KeyFile/)  
C# class library which supports loading, parsing, modifying, saving, and generating Falcon .key files.  

[F4KeyFileViewer](https://github.com/lightningviper/lightningstools/tree/master/src/F4KeyFileViewer/)  
Desktop application for viewing Falcon .key files.  Demonstrates the use of the [F4KeyFile](https://github.com/lightningviper/lightningstools/tree/master/src/F4KeyFile/) library.

[F4ResourceFileEditor](https://github.com/lightningviper/lightningstools/tree/master/src/F4ResourceFileEditor/)  
Desktop application for viewing the contents of Falcon resource files.  Originally intended as an editor, it is a currently a read-only tool used to validate the corresponding functionality in the underlying F4Utils libraries.

[F4SharedMem](https://github.com/lightningviper/lightningstools/tree/master/src/F4SharedMem/)  
C# class library for reading values from Falcon BMS shared memory area. 

[F4SharedMemMirror](https://github.com/lightningviper/lightningstools/tree/master/src/F4SharedMemMirror/)  
Desktop client-server application that mirrors the contents of Falcon's shared memory areas to one or more remote networked PCs.

[F4SharedMemViewer](https://github.com/lightningviper/lightningstools/tree/master/src/F4SharedMemViewer/)  
Desktop application that displays the contents of Falcon's shared memory areas.

[F4SharedMemoryRecorder](https://github.com/lightningviper/lightningstools/tree/master/src/F4SharedMemoryRecorder/)  
Minimum-viable concept demonstrator showing ability to record and playback a stream of Falcon shared memory data.   Useful for certain software testing scenarios requiring repeatability that would otherwise be difficult to obtain.

[F4TexSharedMem](https://github.com/lightningviper/lightningstools/tree/master/src/F4TexSharedMem/)  
C# class library for reading images from Falcon BMS' "textures shared memory" area.  

[F4TexSharedMemTester](https://github.com/lightningviper/lightningstools/tree/master/src/F4TexSharedMemTester/)  
Desktop application for displaying the contents of Falcon BMS' "Textures Shared Memory" area.   Demonstrates the use of the [F4TexSharedMem](https://github.com/lightningviper/lightningstools/tree/master/src/F4TexSharedMem/) library.

[F4Utils](https://github.com/lightningviper/lightningstools/tree/master/src/F4Utils/)  
C# class libraries which provide support for detecting Falcon operating characteristics,  interacting with the simulation programmatically, and reading from and writing to various low-level Falcon file formats.  
_Namespaces_:  
    - [F4Utils.Campaign](https://github.com/lightningviper/lightningstools/tree/master/src/F4Utils/Campaign/)  
    Read F4 Campaign files (.CAM/.TRN/.TAC) and the Falcon class table  
    - [F4Utils.Process](https://github.com/lightningviper/lightningstools/tree/master/src/F4Utils/Process/)  
    Detect whether Falcon is running, where it is running from, and send keyboard callbacks to Falcon  
    - [F4Utils.Resources](https://github.com/lightningviper/lightningstools/tree/master/src/F4Utils/Resources/)  
    Read F4's Resource Bundle files (.RSC/.IDX)  
    - [F4Utils.SimSupport](https://github.com/lightningviper/lightningstools/tree/master/src/F4Utils/SimSupport/)  
    Abstraction over Falcon's shared memory areas that exposes values as generic "signals" - used by [SimLinkup](https://github.com/lightningviper/lightningstools/tree/master/src/SimLinkup/)  
    - [F4Utils.Speech](https://github.com/lightningviper/lightningstools/tree/master/src/F4Utils/Speech/)  
    Support for Falcon's AI Speech/Comms database (FALCON.TLK, commFile.bin, evalFile.bin, & fragFile.bin) (reading, writing, generating, exporting, and file format conversion)  
    - [F4Utils.Terrain](https://github.com/lightningviper/lightningstools/tree/master/src/F4Utils/Terrain/)  
    Basic support for Falcon's terrain database (THEATER.MAP, THEATER.Lx/THEATER.Ox, THEATER.TDF, TEXTURE.BIN, etc) which includes support for accessing the terrain tile and mipmap images, the terrain height information at any grid coordinate, the terrain extent themselves, the UI theater map at various resolutions, etc.    

[Henkie.ADI.TestTool](https://github.com/lightningviper/lightningstools/tree/master/src/Henkie/Henkie.ADI.TestTool/)  
Desktop application for testing Henkie's F-16 ADI Support Board with a real ARU-42/A Attitude Director Indicator instrument.

[Henkie.Altimeter](https://github.com/lightningviper/lightningstools/tree/master/src/Henkie/Henkie.Altimeter/)  
C# class library for interfacing with Henkie's Altimeter interface card.  

[Henkie.Common](https://github.com/lightningviper/lightningstools/tree/master/src/Henkie/Henkie.Common/)  
Shared C# code for Henkie.* projects.

[Henkie.FuelFlow](https://github.com/lightningviper/lightningstools/tree/master/src/Henkie/Henkie.FuelFlow/)  
C# class library for interfacing with Henkie's Fuel Flow Indicator interface card.  

[Henkie.HSI](https://github.com/lightningviper/lightningstools/tree/master/src/Henkie/Henkie.HSI/)  
C# class library for interfacing with Henkie's HSI Board #1 interface card.  

[Henkie.QuadSinCos](https://github.com/lightningviper/lightningstools/tree/master/src/Henkie/Henkie.QuadSinCos/)  
C# class library for interfacing with Henkie's Standby ADI / Quad SIN/COS interface card.

[Henkie.SDI](https://github.com/lightningviper/lightningstools/tree/master/src/Henkie/Henkie.SDI/)  
C# class library for interfacing with Henkie's Digital-to-Synchro (SDI) interface card.  

[Henkie.SDI.TestTool](https://github.com/lightningviper/lightningstools/tree/master/src/Henkie/Henkie.SDI.TestTool/)  
End-user desktop application for testing Henkie's Digital-to-Synchro (SDI) interface card. Demonstrates the use of the [Henkie.SDI](https://github.com/lightningviper/lightningstools/tree/master/src/Henkie/Henkie.SDI/) class library.  

[JoyMapper](https://github.com/lightningviper/lightningstools/tree/master/src/JoyMapper/)  
Desktop application for remapping analog and digital inputs from DirectInput devices, BetaInnovations non-Joystick-class HID devices, and PHCC devices using PPJoy virtual joystick drivers.  
   
[LightningGauges](https://github.com/lightningviper/lightningstools/tree/master/src/LightningGauges/)  
C# class library which provides GDI+ renderers for various flight instruments and gauges as found in the F-16.  

[Lzss](https://github.com/lightningviper/lightningstools/tree/master/src/Lzss/)  
Class libraries that support compressing and decompressing binary content in the LZSS format used in the binary file formats within Hasbro & Microprose games such as Falcon.  
- [LzssNative](https://github.com/lightningviper/lightningstools/tree/master/src/Lzss/LzssNative/)  
    C++ (native) class library  
- [LzssManaged](https://github.com/lightningviper/lightningstools/tree/master/src/Lzss/LzssManaged/)  
    C# managed .NET P/Invoke wrapper  

[MFDExtractor](https://github.com/lightningviper/lightningstools/tree/master/src/MFDExtractor/)  
End user client/server application that allows extracting various flight instruments from Falcon.  

[MasterBuild](https://github.com/lightningviper/lightningstools/tree/master/src/MasterBuild/)  
Automation scripts for running command-line builds, setting up developer and build machines, building all projects in the repository in a single pass, etc.  

[PHCC](https://github.com/lightningviper/lightningstools/tree/master/src/PHCC/)  
C# class library for interfacing with the PIC Home Cockpit Controller I/O hardware and various attached peripherals.  

[PPJoyWrapper](https://github.com/lightningviper/lightningstools/tree/master/src/PPJoyWrapper/)  
C# class library for interacting with the PPJoy virtual joystick drivers from Deon Van Der Westhuysen.  

[PDF2Img](https://github.com/lightningviper/lightningstools/tree/master/src/PDF2Img/)  
C# class library for converting PDFs to images.  Uses embedded GhostScript library to perform the conversions.   

[PhccDeviceManager](https://github.com/lightningviper/lightningstools/tree/master/src/PhccDeviceManager/)  
End-user GUI application for configuring PHCC motherboard and peripherals.  

[PhccTestTool](https://github.com/lightningviper/lightningstools/tree/master/src/PhccTestTool/)  
End-user desktop application providing basic testing capabilities for the PHCC motherboard and attached peripherals.  Demonstrates the use of the [PHCC](https://github.com/lightningviper/lightningstools/tree/master/src/PHCC/) class library.  

[SimLinkup](https://github.com/lightningviper/lightningstools/tree/master/src/SimLinkup/)  
End-user desktop application for controlling various simulator-related I/O hardware and physical instruments using data from simulation software, and for controlling the simulation by reading inputs from their I/O hardware.  Provides out-of-the-box support for Falcon BMS, PHCC devices and popular peripherals, BetaInnovations devices, DirectInput devices, Analog Devices AD536x and AD537x evaluation boards, and a variety of simulated instrumentation. 

[SpeexInvoke](https://github.com/lightningviper/lightningstools/tree/master/src/SpeexInvoke/)  
C# wrapper for the open-source Speex audio compression library.  

[TlkTool](https://github.com/lightningviper/lightningstools/tree/master/src/TlkTool/)  
End-user command-line (console) application which provides the ability to edit Falcon's AI comms databases (falcon.TLK, commFile.bin, fragFile.bin, and evalFile.bin files).  
_Functionality supported:_
- Decompressing and extracting any/all of the individual compressed audio files from within the .TLK file, to ordinary .WAV files or compressed .LH or .SPX files.  
- Exporting the commFile.bin, fragFile.bin, and evalFile.bin files to plain colon-delimited text files, which can then be edited in any text editor or spreadsheet.   
- Regenerating the commFile.bin, fragFile.bin, and evalFile.bin files from these plain text files.  
- Creating or regenerating a .TLK file from a set of .WAV, .LH, or .SPX files and updating individual entries in an existing .TLK file