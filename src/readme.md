
# Building the Code
##### Installing Prerequisites and Checking out the Code
To clone the `lightningstools` repository and install all required prerequisites in a single command, run the following command from an **administrative** command prompt:
```
@cd "%userprofile%\Downloads" & powershell Invoke-WebRequest -OutFile "up.bat" "https://raw.githubusercontent.com/lightningviper/lightningstools/master/src/MasterBuild/up.bat" & up.bat
```

The above command will clone the repository to a local folder, `\lightningstools`, located in the root of the current drive where the above command was run from (typically, this folder will be located at `c:\lightningstools`).  
  
The following prerequisites will be installed:  
- Visual Studio 2017 Community Edition  
- Visual Studio Installer Project add-in  
- DirectX SDK (June 2010)  
- .NET Framework 3.5 SP1 (required for Managed DirectX)  
- .NET Framework 4.6.1 (required by all projects)  
- C++ runtimes.  
- Command-line Build tools  
  
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
[ADITestTool](https://github.com/lightningviper/lightningstools/tree/master/src/ADITestTool/)  
Desktop application for testing Henkie's F-16 ADI Support Board with a real ARU-42/A Attitude Director Indicator instrument.

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
    - [Common.PDF](https://github.com/lightningviper/lightningstools/tree/master/src/Common/PDF/)  
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
C# class library for reading values from Falcon 4's shared memory area. 

[F4SharedMemMirror](https://github.com/lightningviper/lightningstools/tree/master/src/F4SharedMemMirror/)  
Desktop client-server application that mirrors the contents of Falcon's shared memory areas to one or more remote networked PCs.

[F4SharedMemViewer](https://github.com/lightningviper/lightningstools/tree/master/src/F4SharedMemViewer/)  
Desktop application that displays the contents of Falcon's shared memory areas.

[F4SharedMemoryRecorder](https://github.com/lightningviper/lightningstools/tree/master/src/F4SharedMemoryRecorder/)  
Minimum-viable concept demonstrator showing ability to record and playback a stream of Falcon shared memory data.   Useful for certain software testing scenarios requiring repeatability that would otherwise be difficult to obtain.

[F4TexSharedMem](https://github.com/lightningviper/lightningstools/tree/master/src/F4TexSharedMem/)  
C# class library for reading images from Falcon BMS' "textures shared memory" area.  

[F4TexSharedMemTester](https://github.com/lightningviper/lightningstools/tree/master/src/F4TexSharedMemTester/)  
Desktop application for displaying the contents of Falcon BMS' "Textures Shared Memory" area.   Demonstrates the use of the [F4TexSharedMem]((https://github.com/lightningviper/lightningstools/tree/master/src/F4TexSharedMem/) library.

[F4Utils](https://github.com/lightningviper/lightningstools/tree/master/src/F4Utils/)  
C# class libraries which provide support for detecting Falcon operating characteristics,  interacting with the simulation programmatically, and reading from and writing to various low-level Falcon 4 file formats.  
_Namespaces_:  
    - [F4Utils.Campaign](https://github.com/lightningviper/lightningstools/tree/master/src/F4Utils/Campaign/)  
    Read F4 Campaign files (.CAM/.TRN/.TAC) and the Falcon 4 class table  
    - [F4Utils.PlayerOp](https://github.com/lightningviper/lightningstools/tree/master/src/F4Utils/PlayerOp/)  
    Read the Player Options file (used for determining attributes like the current pilot callsign, etc.)  
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

[JoyMapper](https://github.com/lightningviper/lightningstools/tree/master/src/JoyMapper/)  
Desktop application for remapping analog and digital inputs from DirectInput devices, BetaInnovations non-Joystick-class HID devices, and PHCC devices using PPJoy virtual joystick drivers.  
   
[LightningGauges](https://github.com/lightningviper/lightningstools/tree/master/src/LightningGauges/)  
C# class library which provides GDI+ renderers for various flight instruments and gauges as found in the F-16.  

[Lzss](https://github.com/lightningviper/lightningstools/tree/master/src/Lzss/)  
Class libraries that support compressing and decompressing binary content in the LZSS format used in the binary file formats within Hasbro & Microprose games such as Falcon 4.  
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
C# class library for converting PDFs to images.  Uses embedded GhostScript library to perform the conversions.  Intended to replace the functionality in [Common.PDF](https://github.com/lightningviper/lightningstools/tree/master/src/Common/PDF/).  

[PhccDeviceManager](https://github.com/lightningviper/lightningstools/tree/master/src/PhccDeviceManager/)  
End-user GUI application for configuring PHCC motherboard and peripherals.  

[PhccTestTool](https://github.com/lightningviper/lightningstools/tree/master/src/PhccTestTool/)  
End-user desktop application providing basic testing capabilities for the PHCC motherboard and attached peripherals.  Demonstrates the use of the [PHCC](https://github.com/lightningviper/lightningstools/tree/master/src/PHCC/) class library.  

[SDI](https://github.com/lightningviper/lightningstools/tree/master/src/SDI/)  
C# class library for interfacing with Henkie's Digital-to-Synchro (SDI) interface card.  

[SDITestTool](https://github.com/lightningviper/lightningstools/tree/master/src/SDITestTool/)  
End-user desktop application for testing Henkie's Digital-to-Synchro (SDI) interface card. Demonstrates the use of the [SDI](https://github.com/lightningviper/lightningstools/tree/master/src/SDI/) class library.  

[SimLinkup](https://github.com/lightningviper/lightningstools/tree/master/src/SimLinkup/)  
End-user desktop application for controlling various simulator-related I/O hardware and physical instruments using data from simulation software, and for controlling the simulation by reading inputs from their I/O hardware.  Provides out-of-the-box support for Falcon 4, PHCC devices and popular peripherals, BetaInnovations devices, DirectInput devices, Analog Devices AD536x and AD537x evaluation boards, and a variety of simulated instrumentation.  

[SpeexInvoke](https://github.com/lightningviper/lightningstools/tree/master/src/SpeexInvoke/)  
C# wrapper for the open-source Speex audio compression library.  

[TlkTool](https://github.com/lightningviper/lightningstools/tree/master/src/TlkTool/)  
End-user command-line (console) application which provides the ability to edit Falcon's AI comms databases (falcon.TLK, commFile.bin, fragFile.bin, and evalFile.bin files).  
_Functionality supported:_
- Decompressing and extracting any/all of the individual compressed audio files from within the .TLK file, to ordinary .WAV files or compressed .LH or .SPX files.  
- Exporting the commFile.bin, fragFile.bin, and evalFile.bin files to plain colon-delimited text files, which can then be edited in any text editor or spreadsheet.   
- Regenerating the commFile.bin, fragFile.bin, and evalFile.bin files from these plain text files.  
- Creating or regenerating a .TLK file from a set of .WAV, .LH, or .SPX files and updating individual entries in an existing .TLK file