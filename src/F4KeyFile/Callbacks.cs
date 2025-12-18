namespace F4KeyFile
{
    public enum Callbacks
    {
        [Category("NOOP")]
        [SubCategory("NOOP")]
        [Description("Do nothing")]
        [ShortDescription("Do Nothing")]
        SimDoNothing,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("EPU / GEN Switch - Hold")]
        [ShortDescription("EPU GEN Switch")]
        SimEpuGenTest,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("FLCS PWR TEST Switch - Hold")]
        [ShortDescription("FLCS PWR Test")]
        SimFlcsPowerTest,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("MAL & IND LTS Button - Hold")]
        [ShortDescription("MalIndLts Hold")]
        SimMalIndLights,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("MAL & IND LTS Button - Release")]
        [ShortDescription("MalIndLts Rel")]
        SimMalIndLightsOFF,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("OXY QTY Switch - Hold")]
        [ShortDescription("Oxy Qty Switch")]
        SimOBOGSBit,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("FIRE & OHEAT DETECT Button - Hold")]
        [ShortDescription("Fire & Oheat Detect")]
        SimOverHeat,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("PROBE HEAT Switch - Step Down")]
        [ShortDescription("Probe Heat Dn")]
        SimProbeHeatMoveDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("PROBE HEAT Switch - Step Up")]
        [ShortDescription("Probe Heat Up")]
        SimProbeHeatMoveUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("PROBE HEAT Switch - OFF")]
        [ShortDescription("Probe Heat Off")]
        SimProbeHeatOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("PROBE HEAT Switch - ON")]
        [ShortDescription("Probe Heat On")]
        SimProbeHeatOn,

        [Category("LEFT CONSOLE")]
        [SubCategory("TEST PANEL")]
        [Description("PROBE HEAT Switch - TEST")]
        [ShortDescription("Probe Heat Test")]
        SimProbeHeatTest,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("ALT FLAPS Switch - Toggle")]
        [ShortDescription("Alt Flaps Tog")]
        SimAltFlaps,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("ALT FLAPS Switch - EXTEND")]
        [ShortDescription("Alt Flaps Extend")]
        SimAltFlapsExtend,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("ALT FLAPS Switch - NORM")]
        [ShortDescription("Alt Flaps Norm")]
        SimAltFlapsNorm,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("DIGITAL Switch - Toggle")]
        [ShortDescription("Digital Sw Toggle")]
        SimDigitalBUP,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("DIGITAL Switch - BACKUP")]
        [ShortDescription("Digital Sw Backup")]
        SimDigitalBUPBackup,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("DIGITAL Switch - OFF")]
        [ShortDescription("Digital Switch Off")]
        SimDigitalBUPOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("FLCS Switch - Hold")]
        [ShortDescription("FLCS Switch")]
        SimFLCSReset,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("BIT Switch - Push")]
        [ShortDescription("Bit Switch")]
        SimFLTBIT,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("LE FLAPS Switch - AUTO")]
        [ShortDescription("LE Flaps Auto")]
        SimLEFAuto,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("LE FLAPS Switch - LOCK")]
        [ShortDescription("LE Flaps Lock")]
        SimLEFLock,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("LE FLAPS Switch - Toggle")]
        [ShortDescription("LE Flaps Tog")]
        SimLEFLockSwitch,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("MANUAL TF FLYUP Switch - Toggle")]
        [ShortDescription("Man TF Flyup Tog")]
        SimManualFlyup,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("MANUAL TF FLYUP Switch - DISABLE")]
        [ShortDescription("Man TF Flyup Dis")]
        SimManualFlyupDisable,

        [Category("LEFT CONSOLE")]
        [SubCategory("FLT CONTROL PANEL")]
        [Description("MANUAL TF FLYUP Switch - ENABLE")]
        [ShortDescription("Man TF Flyup En")]
        SimManualFlyupEnable,

        [Category("LEFT CONSOLE")]
        [SubCategory("MANUAL TRIM PANEL")]
        [Description("TRIM / AP DISC Switch - DISC")]
        [ShortDescription("Trim / AP Disc")]
        SimTrimAPDISC,

        [Category("LEFT CONSOLE")]
        [SubCategory("MANUAL TRIM PANEL")]
        [Description("TRIM / AP DISC Switch - NORM")]
        [ShortDescription("Trim / AP Norm")]
        SimTrimAPNORM,

        [Category("LEFT CONSOLE")]
        [SubCategory("MANUAL TRIM PANEL")]
        [Description("PITCH TRIM Wheel - NOSE DN")]
        [ShortDescription("Trim Nose Dn")]
        SimTrimNoseDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("MANUAL TRIM PANEL")]
        [Description("PITCH TRIM Wheel - NOSE UP")]
        [ShortDescription("Trim Nose Up")]
        SimTrimNoseUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("MANUAL TRIM PANEL")]
        [Description("ROLL TRIM Wheel - L WING DN")]
        [ShortDescription("Trim L Wing Dn")]
        SimTrimRollLeft,

        [Category("LEFT CONSOLE")]
        [SubCategory("MANUAL TRIM PANEL")]
        [Description("ROLL TRIM Wheel - R WING DN")]
        [ShortDescription("Trim R Wing Dn")]
        SimTrimRollRight,

        [Category("LEFT CONSOLE")]
        [SubCategory("MANUAL TRIM PANEL")]
        [Description("YAW TRIM Knob - L")]
        [ShortDescription("Trim Yaw Left")]
        SimTrimYawLeft,

        [Category("LEFT CONSOLE")]
        [SubCategory("MANUAL TRIM PANEL")]
        [Description("YAW TRIM Knob - R")]
        [ShortDescription("Trim Yaw Right")]
        SimTrimYawRight,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("ENG FEED Knob - Step Down")]
        [ShortDescription("Eng Feed Dn")]
        SimDecFuelPump,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("AIR REFUEL Switch - CLOSE")]
        [ShortDescription("Air Refuel Close")]
        SimFuelDoorClose,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("AIR REFUEL Switch - OPEN")]
        [ShortDescription("Air Refuel Open")]
        SimFuelDoorOpen,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("AIR REFUEL Switch - Toggle")]
        [ShortDescription("Air Refuel Tog")]
        SimFuelDoorToggle,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("ENG FEED Knob - AFT")]
        [ShortDescription("Eng Feed Aft")]
        SimFuelPumpAft,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("ENG FEED Knob - FWD")]
        [ShortDescription("Eng Feed Fwd")]
        SimFuelPumpFwd,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("ENG FEED Knob - NORM")]
        [ShortDescription("Eng Feed Norm")]
        SimFuelPumpNorm,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("ENG FEED Knob - OFF")]
        [ShortDescription("Eng Feed Off")]
        SimFuelPumpOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("ENG FEED Knob - Step Up")]
        [ShortDescription("Eng Feed Up")]
        SimIncFuelPump,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("MASTER Switch - OFF")]
        [ShortDescription("Fuel Master Off")]
        SimMasterFuelOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("MASTER Switch - ON")]
        [ShortDescription("Fuel Master On")]
        SimMasterFuelOn,

        [Category("LEFT CONSOLE")]
        [SubCategory("FUEL PANEL")]
        [Description("MASTER Switch - Toggle")]
        [ShortDescription("Fuel Master Tog")]
        SimToggleMasterFuel,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CNI Knob Switch - BACKUP")]
        [ShortDescription("CNI Knob Bckup")]
        SimAuxComBackup,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CNI Knob Switch - UFC")]
        [ShortDescription("CNI Knob UFC")]
        SimAuxComUFC,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CHANNEL - Toggle Band X / Y")]
        [ShortDescription("Channel Tog X / Y")]
        SimCycleBandAuxComDigit,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CHANNEL - Cycle Up Center Digit")]
        [ShortDescription("Centr Channel Up")]
        SimCycleCenterAuxComDigit,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CHANNEL - Cycle Up Left Digit")]
        [ShortDescription("Left Channel Up")]
        SimCycleLeftAuxComDigit,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CHANNEL - Cycle Up Right Digit")]
        [ShortDescription("Right Channel Up")]
        SimCycleRightAuxComDigit,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CHANNEL - Cycle Down Center Dig.")]
        [ShortDescription("Centr Channel Dn")]
        SimDecCenterAuxComDigit,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CHANNEL - Cycle Down Left Digit")]
        [ShortDescription("Left Channel Dn ")]
        SimDecLeftAuxComDigit,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CHANNEL - Cycle Down Right Digit")]
        [ShortDescription("Right Channel Dn")]
        SimDecRightAuxComDigit,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - 0 * **")]
        [ShortDescription("IFF M1 - 0 * **")]
        SimIFFBackupM1Digit1_0,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - 1 * **")]
        [ShortDescription("IFF M1 - 1 * **")]
        SimIFFBackupM1Digit1_1,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - 2 * **")]
        [ShortDescription("IFF M1 - 2 * **")]
        SimIFFBackupM1Digit1_2,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - 3 * **")]
        [ShortDescription("IFF M1 - 3 * **")]
        SimIFFBackupM1Digit1_3,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - 4 * **")]
        [ShortDescription("IFF M1 - 4 * **")]
        SimIFFBackupM1Digit1_4,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - 5 * **")]
        [ShortDescription("IFF M1 - 5 * **")]
        SimIFFBackupM1Digit1_5,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - 6 * **")]
        [ShortDescription("IFF M1 - 6 * **")]
        SimIFFBackupM1Digit1_6,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - 7 * **")]
        [ShortDescription("IFF M1 - 7 * **")]
        SimIFFBackupM1Digit1_7,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - X * **-Cycle Down")]
        [ShortDescription("IFF M1 X * **Dn")]
        SimIFFBackupM1Digit1Dec,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - X * **-Cycle Up")]
        [ShortDescription("IFF M1 X * **Up")]
        SimIFFBackupM1Digit1Inc,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - *0 * *")]
        [ShortDescription("IFF M1 - *0 * *")]
        SimIFFBackupM1Digit2_0,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - *1 * *")]
        [ShortDescription("IFF M1 - *1 * *")]
        SimIFFBackupM1Digit2_1,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - *2 * *")]
        [ShortDescription("IFF M1 - *2 * *")]
        SimIFFBackupM1Digit2_2,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - *3 * *")]
        [ShortDescription("IFF M1 - *3 * *")]
        SimIFFBackupM1Digit2_3,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - *X * *-Cycle Down")]
        [ShortDescription("IFF M1 * X * *Dn")]
        SimIFFBackupM1Digit2Dec,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE I - *X * *-Cycle Up")]
        [ShortDescription("IFF M1 * X * *Up")]
        SimIFFBackupM1Digit2Inc,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - **0 * ")]
        [ShortDescription("IFF M3 - **0 * ")]
        SimIFFBackupM3Digit1_0,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - **1 * ")]
        [ShortDescription("IFF M3 - **1 * ")]
        SimIFFBackupM3Digit1_1,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - **2 * ")]
        [ShortDescription("IFF M3 - **2 * ")]
        SimIFFBackupM3Digit1_2,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - **3 * ")]
        [ShortDescription("IFF M3 - **3 * ")]
        SimIFFBackupM3Digit1_3,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - **4 * ")]
        [ShortDescription("IFF M3 - **4 * ")]
        SimIFFBackupM3Digit1_4,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - **5 * ")]
        [ShortDescription("IFF M3 - **5 * ")]
        SimIFFBackupM3Digit1_5,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - **6 * ")]
        [ShortDescription("IFF M3 - **6 * ")]
        SimIFFBackupM3Digit1_6,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - **7 * ")]
        [ShortDescription("IFF M3 - **7 * ")]
        SimIFFBackupM3Digit1_7,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - **X * -Cycle Down")]
        [ShortDescription("IFF M3 * *X * Dn")]
        SimIFFBackupM3Digit1Dec,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - **X * -Cycle Up")]
        [ShortDescription("IFF M3 * *X * Up")]
        SimIFFBackupM3Digit1Inc,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - ***0")]
        [ShortDescription("IFF M3 - ***0")]
        SimIFFBackupM3Digit2_0,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - ***1")]
        [ShortDescription("IFF M3 - ***1")]
        SimIFFBackupM3Digit2_1,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - ***2")]
        [ShortDescription("IFF M3 - ***2")]
        SimIFFBackupM3Digit2_2,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - ***3")]
        [ShortDescription("IFF M3 - ***3")]
        SimIFFBackupM3Digit2_3,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - ***4")]
        [ShortDescription("IFF M3 - ***4")]
        SimIFFBackupM3Digit2_4,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - ***5")]
        [ShortDescription("IFF M3 - ***5")]
        SimIFFBackupM3Digit2_5,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - ***6")]
        [ShortDescription("IFF M3 - ***6")]
        SimIFFBackupM3Digit2_6,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - ***7")]
        [ShortDescription("IFF M3 - ***7")]
        SimIFFBackupM3Digit2_7,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - ***X - Cycle Down")]
        [ShortDescription("IFF M3 * **X Dn")]
        SimIFFBackupM3Digit2Dec,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF MODE 3 - ***X - Cycle Up")]
        [ShortDescription("IFF M3 * **X Up")]
        SimIFFBackupM3Digit2Inc,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("M - 4 CODE Switch - HOLD")]
        [ShortDescription("M - 4 Code Hold")]
        SimIFFCodeSwitchHold,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("M - 4 CODE Switch - ZERO")]
        [ShortDescription("M - 4 Code Zero")]
        SimIFFCodeSwitchZero,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF ENABLE Switch - Cycle")]
        [ShortDescription("Enable Cycle")]
        SimIFFEnableCycle,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF ENABLE Switch - Step down")]
        [ShortDescription("Enable Dec")]
        SimIFFEnableDec,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF ENABLE Switch - Step Up")]
        [ShortDescription("Enable Inc")]
        SimIFFEnableInc,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF ENABLE Switch - M1 / M3")]
        [ShortDescription("Enable M1M3")]
        SimIFFEnableM1M3,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF ENABLE Switch - M3 / MS")]
        [ShortDescription("Enable M3MS")]
        SimIFFEnableM3MS,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("IFF ENABLE Switch - OFF")]
        [ShortDescription("Enable Off")]
        SimIFFEnableOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("MASTER Knob - Step Down")]
        [ShortDescription("Master Down")]
        SimIFFMasterDec,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("MASTER Knob - EMER")]
        [ShortDescription("Master Emer")]
        SimIFFMasterEmerg,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("MASTER Knob - Step Up")]
        [ShortDescription("Master Up")]
        SimIFFMasterInc,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("MASTER Knob - LOW")]
        [ShortDescription("Master Low")]
        SimIFFMasterLow,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("MASTER Knob - NORM")]
        [ShortDescription("Master Norm")]
        SimIFFMasterNorm,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("MASTER Knob - OFF")]
        [ShortDescription("Master Off")]
        SimIFFMasterOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("MASTER Knob - STBY")]
        [ShortDescription("Master Stby")]
        SimIFFMasterStby,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("MONITOR Switch - AUDIO")]
        [ShortDescription("Monitor Audio")]
        SimIFFMode4MonitorAud,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("MONITOR Switch - OUT")]
        [ShortDescription("Monitor Out")]
        SimIFFMode4MonitorOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("MONITOR Switch - Toggle")]
        [ShortDescription("Monitor Toggle")]
        SimIFFMode4MonitorToggle,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("REPLY Switch - A")]
        [ShortDescription("Reply A")]
        SimIFFMode4ReplyAlpha,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("REPLY Switch - B")]
        [ShortDescription("Reply B")]
        SimIFFMode4ReplyBravo,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("REPLY Switch - Cycle")]
        [ShortDescription("Reply Cycle")]
        SimIFFMode4ReplyCycle,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("REPLY Switch - Step Down")]
        [ShortDescription("Reply Down")]
        SimIFFMode4ReplyDec,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("REPLY Switch - Step Up")]
        [ShortDescription("Reply Up")]
        SimIFFMode4ReplyInc,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("REPLY Switch - OUT")]
        [ShortDescription("Reply Out")]
        SimIFFMode4ReplyOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("STATION SELECTOR Switch - A / A TR")]
        [ShortDescription("Station Sel A / A TR")]
        SimTACANAATR,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("STATION SELECTOR Switch - T / R")]
        [ShortDescription("Station Sel T / R")]
        SimTACANTR,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("STATION SELECTOR Switch - Toggle")]
        [ShortDescription("Station Sel Tog")]
        SimToggleAuxComAATR,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CNI Knob Switch - Toggle")]
        [ShortDescription("CNI Knob Tog")]
        SimToggleAuxComMaster,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CHANNEL - Toggle Band X")]
        [ShortDescription("Channel X")]
        SimXBandAuxComDigit,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUX COMM PANEL")]
        [Description("CHANNEL - Toggle Band Y")]
        [ShortDescription("Channel Y")]
        SimYBandAuxComDigit,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("ANTI COLLISION Switch - OFF")]
        [ShortDescription("Anti Coll Lts Off")]
        SimAntiCollOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("ANTI COLLISION Switch - ON")]
        [ShortDescription("Anti Coll Lts On")]
        SimAntiCollOn,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("ANTI COLLISION Switch - Toggle")]
        [ShortDescription("Anti Coll Lts Tog")]
        SimExtlAntiColl,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("MASTER Switch - NORM")]
        [ShortDescription("Master Lts Norm")]
        SimExtlMasterNorm,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("MASTER Switch - OFF")]
        [ShortDescription("Master Lts Off")]
        SimExtlMasterOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("MASTER Switch - Toggle")]
        [ShortDescription("Master Lts Tog")]
        SimExtlPower,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("POSITION Switch - Toggle")]
        [ShortDescription("Position Lts Tog")]
        SimExtlSteady,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("WING / TAIL Switch - Toggle")]
        [ShortDescription("Wing / Fus Lts Tog")]
        SimExtlWing,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("POSITION Switch - FLASH")]
        [ShortDescription("Position Lts Flash")]
        SimLightsFlash,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("POSITION Switch - STEADY")]
        [ShortDescription("Position Lts Stdy")]
        SimLightsSteady,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("WING / TAIL Switch - BRT")]
        [ShortDescription("Wing / Fus Lts Brt")]
        SimWingLightBrt,

        [Category("LEFT CONSOLE")]
        [SubCategory("EXT LIGHTING PANEL")]
        [Description("WING / TAIL Switch - OFF")]
        [ShortDescription("Wing / Fus Lts Off")]
        SimWingLightOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("EPU PANEL")]
        [Description("EPU Switch - NORM")]
        [ShortDescription("EPU Sw Norm")]
        SimEpuAuto,

        [Category("LEFT CONSOLE")]
        [SubCategory("EPU PANEL")]
        [Description("EPU Switch - Step Down")]
        [ShortDescription("EPU Sw Step Dn")]
        SimEpuDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("EPU PANEL")]
        [Description("EPU Switch - OFF")]
        [ShortDescription("EPU Sw Off")]
        SimEpuOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("EPU PANEL")]
        [Description("EPU Switch - ON")]
        [ShortDescription("EPU Sw On")]
        SimEpuOn,

        [Category("LEFT CONSOLE")]
        [SubCategory("EPU PANEL")]
        [Description("EPU Switch - Cycle")]
        [ShortDescription("EPU Sw Cycl")]
        SimEpuToggle,

        [Category("LEFT CONSOLE")]
        [SubCategory("EPU PANEL")]
        [Description("EPU Switch - Step Up")]
        [ShortDescription("EPU Sw Step Up")]
        SimEpuUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("ELEC PANEL")]
        [Description("CAUTION RESET Button - Push")]
        [ShortDescription("Caution Reset")]
        SimElecReset,

        [Category("LEFT CONSOLE")]
        [SubCategory("ELEC PANEL")]
        [Description("MAIN PWR Switch - BATT")]
        [ShortDescription("Main Pwr Batt")]
        SimMainPowerBatt,

        [Category("LEFT CONSOLE")]
        [SubCategory("ELEC PANEL")]
        [Description("MAIN PWR Switch - Step Down")]
        [ShortDescription("Main Pwr Step Dn")]
        SimMainPowerDec,

        [Category("LEFT CONSOLE")]
        [SubCategory("ELEC PANEL")]
        [Description("MAIN PWR Switch - Step Up")]
        [ShortDescription("Main Pwr Step Up")]
        SimMainPowerInc,

        [Category("LEFT CONSOLE")]
        [SubCategory("ELEC PANEL")]
        [Description("MAIN PWR Switch - MAIN")]
        [ShortDescription("Main Pwr Main")]
        SimMainPowerMain,

        [Category("LEFT CONSOLE")]
        [SubCategory("ELEC PANEL")]
        [Description("MAIN PWR Switch - OFF")]
        [ShortDescription("Main Pwr Off")]
        SimMainPowerOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("AVTR PANEL")]
        [Description("AVTR Switch - Cycle")]
        [ShortDescription("AVTR Sw Cycle")]
        SimAVTRSwitch,

        [Category("LEFT CONSOLE")]
        [SubCategory("AVTR PANEL")]
        [Description("AVTR Switch - AUTO")]
        [ShortDescription("AVTR Sw Auto")]
        SimAVTRSwitchAuto,

        [Category("LEFT CONSOLE")]
        [SubCategory("AVTR PANEL")]
        [Description("AVTR Switch - Step Down")]
        [ShortDescription("AVTR Sw Dn")]
        SimAVTRSwitchDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("AVTR PANEL")]
        [Description("AVTR Switch - OFF")]
        [ShortDescription("AVTR Sw Off")]
        SimAVTRSwitchOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("AVTR PANEL")]
        [Description("AVTR Switch - ON")]
        [ShortDescription("AVTR Sw On")]
        SimAVTRSwitchOn,

        [Category("LEFT CONSOLE")]
        [SubCategory("AVTR PANEL")]
        [Description("AVTR Switch - Step Up")]
        [ShortDescription("AVTR Sw Up")]
        SimAVTRSwitchUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("AVTR PANEL")]
        [Description("AVTR Switch - Toggle ON / OFF")]
        [ShortDescription("AVTR Sw Tog ")]
        SimAVTRToggle,

        [Category("LEFT CONSOLE")]
        [SubCategory("ECM PANEL")]
        [Description("OPR Switch - Toggle")]
        [ShortDescription("ECM Opr Tog")]
        SimEcmPower,

        [Category("LEFT CONSOLE")]
        [SubCategory("ECM PANEL")]
        [Description("OPR Switch - OFF")]
        [ShortDescription("ECM Opr Off")]
        SimEcmPowerOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("ECM PANEL")]
        [Description("OPR Switch - OPR")]
        [ShortDescription("ECM Opr On")]
        SimEcmPowerOn,

        [Category("LEFT CONSOLE")]
        [SubCategory("ENG & JET START PANEL")]
        [Description("ENG CONT Switch - Toggle")]
        [ShortDescription("Eng Cont Tog")]
        SimEngCont,

        [Category("LEFT CONSOLE")]
        [SubCategory("ENG & JET START PANEL")]
        [Description("ENG CONT Switch - PRI")]
        [ShortDescription("Eng Cont Pri")]
        SimEngContPri,

        [Category("LEFT CONSOLE")]
        [SubCategory("ENG & JET START PANEL")]
        [Description("ENG CONT Switch - SEC")]
        [ShortDescription("Eng Cont Sec")]
        SimEngContSec,

        [Category("LEFT CONSOLE")]
        [SubCategory("ENG & JET START PANEL")]
        [Description("JFS Switch - Cycle  1 / OFF / 2")]
        [ShortDescription("JFS Cyc Start")]
        SimJfsStartCycle,

        [Category("LEFT CONSOLE")]
        [SubCategory("ENG & JET START PANEL")]
        [Description("JFS Switch - Step Down")]
        [ShortDescription("JFS Dn")]
        SimJfsStartDec,

        [Category("LEFT CONSOLE")]
        [SubCategory("ENG & JET START PANEL")]
        [Description("JFS Switch - START 2")]
        [ShortDescription("JFS Start2")]
        SimJfsStartDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("ENG & JET START PANEL")]
        [Description("JFS Switch - Step Up")]
        [ShortDescription("JFS Up")]
        SimJfsStartInc,

        [Category("LEFT CONSOLE")]
        [SubCategory("ENG & JET START PANEL")]
        [Description("JFS Switch - OFF")]
        [ShortDescription("JFS Off")]
        SimJfsStartMid,

        [Category("LEFT CONSOLE")]
        [SubCategory("ENG & JET START PANEL")]
        [Description("JFS Switch - START 1")]
        [ShortDescription("JFS Start1")]
        SimJfsStartUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 2 PANEL")]
        [Description("ILS Knob - Volume Decr.")]
        [ShortDescription("ILS Vol Dec")]
        SimILSDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 2 PANEL")]
        [Description("ILS Knob - Volume Incr.")]
        [ShortDescription("ILS Vol Inc")]
        SimILSUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 2 PANEL")]
        [Description("INTERCOM Knob - Volume Decr.")]
        [ShortDescription("Intercom Vol Dec")]
        SimStepIntercomVolumeDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 2 PANEL")]
        [Description("INTERCOM Knob - Volume Incr.")]
        [ShortDescription("Intercom Vol Inc")]
        SimStepIntercomVolumeUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 1 Mode Knob - Toggle")]
        [ShortDescription("Com1 Mode Tog")]
        SimAud1Com1,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 1 Mode Knob - GD")]
        [ShortDescription("Comm1 Mode Gd")]
        SimAud1Com1Gd,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 1 Mode Knob - SQL")]
        [ShortDescription("Comm1 Mode Sql")]
        SimAud1Com1Sql,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 2 Mode Knob - Toggle")]
        [ShortDescription("Com2 Mode Tog")]
        SimAud1Com2,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 2 Mode Knob - GD")]
        [ShortDescription("Comm2 Mode Gd")]
        SimAud1Com2Gd,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 2 Mode Knob - SQL")]
        [ShortDescription("Comm2 Mode Sql")]
        SimAud1Com2Sql,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 1 Knob - Power Off")]
        [ShortDescription("Comm1 Pwr Off")]
        SimComm1PowerOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 1 Knob - Power On")]
        [ShortDescription("Comm1 Pwr On")]
        SimComm1PowerOn,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 2 Knob - Power Off")]
        [ShortDescription("Comm2 Pwr Off")]
        SimComm2PowerOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 2 Knob - Power On")]
        [ShortDescription("Comm2 Pwr On")]
        SimComm2PowerOn,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 1 Knob - Volume Decr.")]
        [ShortDescription("Comm1 Vol Dec")]
        SimStepComm1VolumeDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 1 Knob - Volume Incr.")]
        [ShortDescription("Comm1 Vol Inc")]
        SimStepComm1VolumeUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 2 Knob - Volume Decr.")]
        [ShortDescription("Comm2 Vol Dec")]
        SimStepComm2VolumeDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("COMM 2 Knob - Volume Incr.")]
        [ShortDescription("Comm2 Vol Inc")]
        SimStepComm2VolumeUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("MSL Knob - Volume Decr.")]
        [ShortDescription("MSL Vol Dec")]
        SimStepMissileVolumeDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("MSL Knob - Volume Incr.")]
        [ShortDescription("MSL Vol Inc")]
        SimStepMissileVolumeUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("THREAT Knob - Volume Decr.")]
        [ShortDescription("Threat Vol Dec")]
        SimStepThreatVolumeDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("AUDIO 1 PANEL")]
        [Description("THREAT Knob - Volume Incr.")]
        [ShortDescription("Threat Vol Inc")]
        SimStepThreatVolumeUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("MPO PANEL")]
        [Description("MANUAL PITCH Switch - Hold")]
        [ShortDescription("MPO Hold")]
        SimMPO,

        [Category("LEFT CONSOLE")]
        [SubCategory("MPO PANEL")]
        [Description("MANUAL PITCH Switch - Toggle")]
        [ShortDescription("MPO Tog")]
        SimMPOToggle,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("VOL Knob - AI vs IVC Volume Decr")]
        [ShortDescription("Vol Knob Decr")]
        OTWBalanceIVCvsAIDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("VOL Knob - AI vs IVC Volume Incr")]
        [ShortDescription("Vol Knob Incr")]
        OTWBalanceIVCvsAIUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("FUNCTION Knob - BOTH")]
        [ShortDescription("Function Both")]
        SimBupUhfBoth,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("A - 3 - 2 - T Rotary 2 * *.* **")]
        [ShortDescription("UHF 2 * *.* **")]
        SimBupUhfFreq1_2,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("A - 3 - 2 - T Rotary 3 * *.* **")]
        [ShortDescription("UHF 3 * *.* **")]
        SimBupUhfFreq1_3,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("A - 3 - 2 - T Rotary X * *.* **-Step Down")]
        [ShortDescription("UHF X * *.* **Dn")]
        SimBupUhfFreq1Dec,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("A - 3 - 2 - T Rotary X * *.* **-Step Up")]
        [ShortDescription("UHF X * *.* **Up")]
        SimBupUhfFreq1Inc,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * 0 *.* **")]
        [ShortDescription("UHF * 0 *.* **")]
        SimBupUhfFreq2_0,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * 1 *.* **")]
        [ShortDescription("UHF * 1 *.* **")]
        SimBupUhfFreq2_1,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * 2 *.* **")]
        [ShortDescription("UHF * 2 *.* **")]
        SimBupUhfFreq2_2,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * 3 *.* **")]
        [ShortDescription("UHF * 3 *.* **")]
        SimBupUhfFreq2_3,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * 4 *.* **")]
        [ShortDescription("UHF * 4 *.* **")]
        SimBupUhfFreq2_4,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * 5 *.* **")]
        [ShortDescription("UHF * 5 *.* **")]
        SimBupUhfFreq2_5,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * 6 *.* **")]
        [ShortDescription("UHF * 6 *.* **")]
        SimBupUhfFreq2_6,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * 7 *.* **")]
        [ShortDescription("UHF * 7 *.* **")]
        SimBupUhfFreq2_7,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * 8 *.* **")]
        [ShortDescription("UHF * 8 *.* **")]
        SimBupUhfFreq2_8,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * 9 *.* **")]
        [ShortDescription("UHF * 9 *.* **")]
        SimBupUhfFreq2_9,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * X *.* **-Cycle Down")]
        [ShortDescription("UHF * X *.* **Dn")]
        SimBupUhfFreq2Dec,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * X *.* **-Cycle Up")]
        [ShortDescription("UHF * X *.* **Up")]
        SimBupUhfFreq2Inc,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *0.* **")]
        [ShortDescription("UHF * *0.* **")]
        SimBupUhfFreq3_0,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *1.* **")]
        [ShortDescription("UHF * *1.* **")]
        SimBupUhfFreq3_1,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *2.* **")]
        [ShortDescription("UHF * *2.* **")]
        SimBupUhfFreq3_2,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *3.* **")]
        [ShortDescription("UHF * *3.* **")]
        SimBupUhfFreq3_3,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *4.* **")]
        [ShortDescription("UHF * *4.* **")]
        SimBupUhfFreq3_4,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *5.* **")]
        [ShortDescription("UHF * *5.* **")]
        SimBupUhfFreq3_5,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *6.* **")]
        [ShortDescription("UHF * *6.* **")]
        SimBupUhfFreq3_6,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *7.* **")]
        [ShortDescription("UHF * *7.* **")]
        SimBupUhfFreq3_7,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *8.* **")]
        [ShortDescription("UHF * *8.* **")]
        SimBupUhfFreq3_8,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *9.* **")]
        [ShortDescription("UHF * *9.* **")]
        SimBupUhfFreq3_9,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *X.* **-Cycle Down")]
        [ShortDescription("UHF * *X.* **Dn")]
        SimBupUhfFreq3Dec,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * *X.* **-Cycle Up")]
        [ShortDescription("UHF * *X.* **Up")]
        SimBupUhfFreq3Inc,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.0 * *")]
        [ShortDescription("UHF * **.0 * *")]
        SimBupUhfFreq4_0,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.1 * *")]
        [ShortDescription("UHF * **.1 * *")]
        SimBupUhfFreq4_1,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.2 * *")]
        [ShortDescription("UHF * **.2 * *")]
        SimBupUhfFreq4_2,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.3 * *")]
        [ShortDescription("UHF * **.3 * *")]
        SimBupUhfFreq4_3,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.4 * *")]
        [ShortDescription("UHF * **.4 * *")]
        SimBupUhfFreq4_4,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.5 * *")]
        [ShortDescription("UHF * **.5 * *")]
        SimBupUhfFreq4_5,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.6 * *")]
        [ShortDescription("UHF * **.6 * *")]
        SimBupUhfFreq4_6,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.7 * *")]
        [ShortDescription("UHF * **.7 * *")]
        SimBupUhfFreq4_7,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.8 * *")]
        [ShortDescription("UHF * **.8 * *")]
        SimBupUhfFreq4_8,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.9 * *")]
        [ShortDescription("UHF * **.9 * *")]
        SimBupUhfFreq4_9,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.X * *-Cycle Down")]
        [ShortDescription("UHF * **.X * *Dn")]
        SimBupUhfFreq4Dec,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.X * *-Cycle Up")]
        [ShortDescription("UHF * **.X * *Up")]
        SimBupUhfFreq4Inc,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.* 00")]
        [ShortDescription("UHF * **.* 00")]
        SimBupUhfFreq5_00,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.* 25")]
        [ShortDescription("UHF * **.* 25")]
        SimBupUhfFreq5_25,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.* 50")]
        [ShortDescription("UHF * **.* 50")]
        SimBupUhfFreq5_50,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.* 75")]
        [ShortDescription("UHF * **.* 75")]
        SimBupUhfFreq5_75,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.* XX - Cycle Down")]
        [ShortDescription("UHF * **.* XX Dn")]
        SimBupUhfFreq5Dec,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("Manual Frequency * **.* XX - Cycle Up")]
        [ShortDescription("UHF * **.* XX Up")]
        SimBupUhfFreq5Inc,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("FUNCTION Knob - Step Down")]
        [ShortDescription("Function Step Dn")]
        SimBupUhfFuncDec,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("FUNCTION Knob - Step Up")]
        [ShortDescription("Function Step Up")]
        SimBupUhfFuncInc,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("MODE Knob - GRD")]
        [ShortDescription("Mode Guard")]
        SimBupUhfGuard,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("FUNCTION Knob - MAIN")]
        [ShortDescription("Function Main")]
        SimBupUhfMain,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("MODE Knob - MNL")]
        [ShortDescription("Mode Manual")]
        SimBupUhfManual,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("MODE Knob - Step Down")]
        [ShortDescription("Mode Step Dn")]
        SimBupUhfModeDec,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("MODE Knob - Step Up")]
        [ShortDescription("Mode Step Up")]
        SimBupUhfModeInc,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("FUNCTION Knob - OFF")]
        [ShortDescription("Function Off")]
        SimBupUhfOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("MODE Knob - PRESET")]
        [ShortDescription("Mode Preset")]
        SimBupUhfPreset,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("PRESET CHANNEL Knob - Cycle Up")]
        [ShortDescription("Channel Cycle Up")]
        SimCycleRadioChannel,

        [Category("LEFT CONSOLE")]
        [SubCategory("UHF PANEL")]
        [Description("PRESET CHANNEL Knob - Cycle Down")]
        [ShortDescription("Channel Cycle Dn")]
        SimDecRadioChannel,

        [Category("LEFT CONSOLE")]
        [SubCategory("LEFT SIDE WALL")]
        [Description("LEFT CANOPY - Close")]
        [ShortDescription("Canopy Close")]
        AFCanopyDec,

        [Category("LEFT CONSOLE")]
        [SubCategory("LEFT SIDE WALL")]
        [Description("LEFT CANOPY - Open")]
        [ShortDescription("Canopy Open")]
        AFCanopyInc,

        [Category("LEFT CONSOLE")]
        [SubCategory("LEFT SIDE WALL")]
        [Description("LEFT SPIDER - Lock")]
        [ShortDescription("Spider Lock")]
        AFCanopyLock,

        [Category("LEFT CONSOLE")]
        [SubCategory("LEFT SIDE WALL")]
        [Description("LEFT SPIDER - Toggle Open / Close")]
        [ShortDescription("Spider Tog")]
        AFCanopyLockToggle,

        [Category("LEFT CONSOLE")]
        [SubCategory("LEFT SIDE WALL")]
        [Description("LEFT CANOPY - Stop")]
        [ShortDescription("Canopy Stop")]
        AFCanopyStop,

        [Category("LEFT CONSOLE")]
        [SubCategory("LEFT SIDE WALL")]
        [Description("LEFT SPIDER - Unlock")]
        [ShortDescription("Spider Unlock")]
        AFCanopyUnlock,

        [Category("LEFT CONSOLE")]
        [SubCategory("LEFT SIDE WALL")]
        [Description("LEFT SLAP Switch(ECM - PGRM # 5)")]
        [ShortDescription("Slap Switch")]
        SimSlapSwitch,

        [Category("LEFT CONSOLE")]
        [SubCategory("SEAT")]
        [Description("EJECT Handle - Hold For Eject")]
        [ShortDescription("EJECT")]
        SimEject,

        [Category("LEFT CONSOLE")]
        [SubCategory("SEAT")]
        [Description("Safety Lever - Toggle")]
        [ShortDescription("Safety Lever Tog")]
        SimSeatArm,

        [Category("LEFT CONSOLE")]
        [SubCategory("SEAT")]
        [Description("Move Down")]
        [ShortDescription("Seat Down")]
        SimSeatDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("SEAT")]
        [Description("Safety Lever - Locked")]
        [ShortDescription("Safety Lever Lock")]
        SimSeatOff,

        [Category("LEFT CONSOLE")]
        [SubCategory("SEAT")]
        [Description("Safety Lever - Armed")]
        [ShortDescription("Safety Lever Arm")]
        SimSeatOn,

        [Category("LEFT CONSOLE")]
        [SubCategory("SEAT")]
        [Description("Move Up")]
        [ShortDescription("Seat Up")]
        SimSeatUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("SPD BRAKE Switch - Close")]
        [ShortDescription("SpdBrk Close")]
        AFBrakesIn,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("SPD BRAKE Switch - Open")]
        [ShortDescription("SpdBrk Open")]
        AFBrakesOut,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("SPD BRAKE Switch - Toggle")]
        [ShortDescription("SpdBrk Tog")]
        AFBrakesToggle,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("COMMS Switch Left - IFF OUT")]
        [ShortDescription("Comms IFF Out")]
        SimCommsSwitchLeft,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("COMMS Switch Right - IFF IN")]
        [ShortDescription("Comms Sw IFF In")]
        SimCommsSwitchRight,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("RDR CURSOR - Down")]
        [ShortDescription("Rdr Cursor Dn")]
        SimCursorDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("RDR CURSOR - Cursor Enable")]
        [ShortDescription("Rdr Cursor Enabl")]
        SimCursorEnable,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("RDR CURSOR - Left")]
        [ShortDescription("Rdr Cursor Left")]
        SimCursorLeft,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("RDR CURSOR - Right")]
        [ShortDescription("Rdr Cursor Right")]
        SimCursorRight,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("RDR CURSOR - Toggle Stop Movement")]
        [ShortDescription("Rdr Cursor Stop")]
        SimCursorStopMovement,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("RDR CURSOR - Up")]
        [ShortDescription("Rdr Cursor Up")]
        SimCursorUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("DOGFIGHT Switch - MRM / DF Cancel")]
        [ShortDescription("MRM / DF Cancel")]
        SimDeselectOverride,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("RDR CURSOR - Cursor Zero")]
        [ShortDescription("Rdr Cursor Zero")]
        SimRadarCursorZero,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("ANT ELEV Knob - Center")]
        [ShortDescription("AntElev Center")]
        SimRadarElevationCenter,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("ANT ELEV Knob - Tilt Down")]
        [ShortDescription("AntElev Dn")]
        SimRadarElevationDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("ANT ELEV Knob - Tilt Up")]
        [ShortDescription("AntElev Up")]
        SimRadarElevationUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("MAN RANGE Knob - Down")]
        [ShortDescription("Man Range Dn")]
        SimRangeKnobDown,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("MAN RANGE Knob - Up")]
        [ShortDescription("Man Range Up")]
        SimRangeKnobUp,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("DOGFIGHT Switch - MRM Override")]
        [ShortDescription("MRM Override")]
        SimSelectMRMOverride,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("DOGFIGHT Switch - DF Override")]
        [ShortDescription("DF Override")]
        SimSelectSRMOverride,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("CUTOFF RELEASE - Idle Detent - Toggle")]
        [ShortDescription("Idle Detent - Tog.")]
        SimThrottleIdleDetent,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("CUTOFF RELEASE - Idle Detent - Off")]
        [ShortDescription("Idle Detent - Off")]
        SimThrottleIdleDetentBack,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("CUTOFF RELEASE - Idle Detent - Idle")]
        [ShortDescription("Idle Detent - Idle")]
        SimThrottleIdleDetentForward,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("CUTOFF RELEASE - Left Engine")]
        [ShortDescription("Idle Detent Left")]
        SimThrottleIdleDetentLeft,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("CUTOFF RELEASE - Right Engine")]
        [ShortDescription("Idle Detent Right")]
        SimThrottleIdleDetentRight,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("MAN RANGE Knob - UNCAGE")]
        [ShortDescription("ManRng Uncage")]
        SimToggleMissileCage,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("COMMS Switch Up - UHF")]
        [ShortDescription("Comms Sw Up")]
        SimTransmitCom1,

        [Category("LEFT CONSOLE")]
        [SubCategory("THROTTLE QUADRANT SYSTEM")]
        [Description("COMMS Switch Down - VHF")]
        [ShortDescription("Comms Sw Dn")]
        SimTransmitCom2,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("ALT GEAR CONTROL")]
        [Description("ALT Extend Gear Handle - Push")]
        [ShortDescription("Alt Gear Extend")]
        AFAlternateGear,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("ALT GEAR CONTROL")]
        [Description("ALT Reset Button - Push")]
        [ShortDescription("Alt Gear Reset")]
        AFAlternateGearReset,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("TWA PANEL")]
        [Description("POWER Button - Toggle")]
        [ShortDescription("TWA Power Tog.")]
        SimRwrPower,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("TWA PANEL")]
        [Description("POWER Button - Off")]
        [ShortDescription("TWA Power Off")]
        SimRwrPowerOff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("TWA PANEL")]
        [Description("POWER Button - On")]
        [ShortDescription("TWA Power On")]
        SimRwrPowerOn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("TWA PANEL")]
        [Description("LOW Button - Toggle")]
        [ShortDescription("TWA Low Tog.")]
        SimRWRSetGroundPriority,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("TWA PANEL")]
        [Description("SEARCH Button - Toggle")]
        [ShortDescription("TWA Search Tog.")]
        SimRWRSetSearch,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("HMCS PANEL")]
        [Description("HMSC Knob - OFF")]
        [ShortDescription("HMCS Knob Off")]
        SimHmsOff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("HMCS PANEL")]
        [Description("HMSC Knob - ON")]
        [ShortDescription("HMCS Knob On")]
        SimHmsOn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("HMCS PANEL")]
        [Description("HMSC Knob - Brightness Decr.")]
        [ShortDescription("HMCS Bright Dec")]
        SimHmsSymWheelDn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("HMCS PANEL")]
        [Description("HMSC Knob - Brightness Incr.")]
        [ShortDescription("HMCS Bright Inc")]
        SimHmsSymWheelUp,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("CH Switch - Power OFF")]
        [ShortDescription("CH Pwr Off")]
        SimEWSChaffOff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("CH Switch - Power ON")]
        [ShortDescription("CH Pwr On")]
        SimEWSChaffOn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("CH Switch - Toggle Power")]
        [ShortDescription("CH Pwr Tog")]
        SimEWSChaffPower,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("DISP Switch - Power Off(MLU EW Panel)")]
        [ShortDescription("DISP Pwr Off")]
        SimEWSDispOff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("DISP Switch - Power On(MLU EW Panel)")]
        [ShortDescription("DISP Pwr On")]
        SimEWSDispOn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("DISP Switch - Toggle(MLU EW Panel)")]
        [ShortDescription("DISP Pwr Toggle")]
        SimEWSDispPower,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("FL Switch - Power OFF")]
        [ShortDescription("FL Pwr Off")]
        SimEWSFlareOff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("FL Switch - Power ON")]
        [ShortDescription("FL Pwr On")]
        SimEWSFlareOn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("FL Switch - Toggle Power")]
        [ShortDescription("FL Pwr Tog")]
        SimEWSFlarePower,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("JMR Switch - Power OFF")]
        [ShortDescription("JMR Pwr Off")]
        SimEWSJammerOff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("JMR Switch - Power ON")]
        [ShortDescription("JMR Pwr On")]
        SimEWSJammerOn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("JMR Switch - Toggle Power")]
        [ShortDescription("JMR Pwr Tog")]
        SimEWSJammerPower,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("JETT Switch - Toggle")]
        [ShortDescription("Jett Sw Toggle")]
        SimEwsJett,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("JETT Switch - OFF")]
        [ShortDescription("Jett Sw Off")]
        SimEwsJettOff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("JETT Switch - ON")]
        [ShortDescription("Jett Sw On")]
        SimEwsJettOn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MODE Knob - AUTO")]
        [ShortDescription("CMDS Mode Auto")]
        SimEWSModeAuto,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MODE Knob - BYP")]
        [ShortDescription("CMDS Mode Byp")]
        SimEWSModeByp,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MODE Knob - MAN")]
        [ShortDescription("CMDS Mode Man")]
        SimEWSModeMan,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MODE Knob - OFF")]
        [ShortDescription("CMDS Mode Off")]
        SimEWSModeOff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MODE Knob - SEMI")]
        [ShortDescription("CMDS Mode Semi")]
        SimEWSModeSemi,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MODE Knob - STBY")]
        [ShortDescription("CMDS Mode Stby")]
        SimEWSModeStby,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MWS Switch - Power OFF")]
        [ShortDescription("MWS Pwr Off")]
        SimEWSMwsOff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MWS Switch - Power ON")]
        [ShortDescription("MWS Pwr On")]
        SimEWSMwsOn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MWS Switch - Toggle Power")]
        [ShortDescription("MWS Pwr Tog")]
        SimEWSMwsPower,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("O1 Switch - Power OFF")]
        [ShortDescription("O1 Pwr Off")]
        SimEWSO1Off,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("O1 Switch - Power ON")]
        [ShortDescription("O1 Pwr On")]
        SimEWSO1On,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("O1 Switch - Toggle Power")]
        [ShortDescription("O1 Pwr Tog")]
        SimEWSO1Power,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("O2 Switch - Power OFF")]
        [ShortDescription("O2 Pwr Off")]
        SimEWSO2Off,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("O2 Switch - Power ON")]
        [ShortDescription("O2 Pwr On")]
        SimEWSO2On,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("O2 Switch - Toggle Power")]
        [ShortDescription("O2 Pwr Tog")]
        SimEWSO2Power,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MODE Knob - Step Down")]
        [ShortDescription("CMDS Mode Dn")]
        SimEWSPGMDec,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("MODE Knob - Step Up")]
        [ShortDescription("CMDS Mode Up")]
        SimEWSPGMInc,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("PRGM Knob - Step Down")]
        [ShortDescription("PRGM Knob Dn")]
        SimEWSProgDec,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("PRGM Knob - 4")]
        [ShortDescription("PRGM Knob 4")]
        SimEWSProgFour,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("PRGM Knob - Step Up")]
        [ShortDescription("PRGM Knob Up")]
        SimEWSProgInc,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("PRGM Knob - 1")]
        [ShortDescription("PRGM Knob 1")]
        SimEWSProgOne,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("PRGM Knob - 3")]
        [ShortDescription("PRGM Knob 3")]
        SimEWSProgThree,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("PRGM Knob - 2")]
        [ShortDescription("PRGM Knob 2")]
        SimEWSProgTwo,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("RWR Switch - Power OFF")]
        [ShortDescription("RWR Pwr Off")]
        SimEWSRWROff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("RWR Switch - Power ON")]
        [ShortDescription("RWR Pwr On")]
        SimEWSRWROn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("CMDS PANEL")]
        [Description("RWR Switch - Toggle Power")]
        [ShortDescription("RWR Pwr Tog")]
        SimEWSRWRPower,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("DN LOCK REL - Push")]
        [ShortDescription("Gear Dn Lck Rel")]
        AFEmergencyGearHandleUnlock,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("LG Handle - DN")]
        [ShortDescription("Gear Down")]
        AFGearDown,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("LG Handle - Toggle")]
        [ShortDescription("Gear Toggle")]
        AFGearToggle,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("LG Handle - UP")]
        [ShortDescription("Gear Up")]
        AFGearUp,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("BRAKES - Channel 2")]
        [ShortDescription("Brake Chnl 2")]
        SimBrakeChannelDown,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("BRAKES - Toggle")]
        [ShortDescription("Brake Chnl Tog")]
        SimBrakeChannelToggle,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("BRAKES - Channel 1")]
        [ShortDescription("Brake Chnl 1")]
        SimBrakeChannelUp,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("STORES CONFIG Switch - CAT I")]
        [ShortDescription("CAT Sw I")]
        SimCATI,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("STORES CONFIG Switch - CAT III")]
        [ShortDescription("CAT Sw III")]
        SimCATIII,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("STORES CONFIG Switch - Toggle")]
        [ShortDescription("CAT Sw Tog")]
        SimCATSwitch,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("EMER STORES JETTISON Button - Hold")]
        [ShortDescription("Emergency Jett")]
        SimEmergencyJettison,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("GND JETT Switch - Toggle")]
        [ShortDescription("GND Jett Tog")]
        SimGndJettEnable,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("GND JETT Switch - OFF")]
        [ShortDescription("GND Jett Off")]
        SimGndJettOff,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("GND JETT Switch - ENABLE")]
        [ShortDescription("GND Jett Enable")]
        SimGndJettOn,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("HOOK Switch - DN")]
        [ShortDescription("Hook Dn")]
        SimHookDown,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("HOOK Switch - Toggle")]
        [ShortDescription("Hook Tog")]
        SimHookToggle,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("HOOK Switch - UP")]
        [ShortDescription("Hook Up")]
        SimHookUp,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("LIGHTS Switch - Cycle")]
        [ShortDescription("Lndg Lights Tog")]
        SimLandingLightCycle,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("LIGHTS Switch - Step Down")]
        [ShortDescription("Lndg Lights Down")]
        SimLandingLightDec,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("LIGHTS Switch - TAXI")]
        [ShortDescription("Light Sw Taxi")]
        SimLandingLightDown,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("LIGHTS Switch - Step Up")]
        [ShortDescription("Lndg Lights Up")]
        SimLandingLightInc,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("LIGHTS Switch - OFF")]
        [ShortDescription("Light Sw Off")]
        SimLandingLightMid,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("LIGHTS Switch - LANDING")]
        [ShortDescription("Light Sw Land")]
        SimLandingLightUp,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("PARKING BRAKE Switch - Cycle")]
        [ShortDescription("Parking Brk Cyc")]
        SimParkingBrakeCycle,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("PARKING BRAKE Switch - Step Down")]
        [ShortDescription("Parking Brk Dn")]
        SimParkingBrakeDec,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("PARKING BRAKE Switch - OFF")]
        [ShortDescription("Parking Brk Off")]
        SimParkingBrakeDown,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("PARKING BRAKE Switch - Step Up")]
        [ShortDescription("Parking Brk Up")]
        SimParkingBrakeInc,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("PARKING BRAKE Switch - ANTI SKID")]
        [ShortDescription("ParkBrk AntiSkid")]
        SimParkingBrakeMid,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("PARKING BRAKE Switch - ON")]
        [ShortDescription("Parking Brk On")]
        SimParkingBrakeUp,

        [Category("LEFT AUX CONSOLE")]
        [SubCategory("GEAR PANEL")]
        [Description("HORN SILENCER Button - Push")]
        [ShortDescription("Horn Silencer")]
        SimSilenceHorn,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("MASTER ARM Switch - ON")]
        [ShortDescription("Master Arm On")]
        SimArmMasterArm,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("LASER Switch - OFF")]
        [ShortDescription("Laser Sw Off")]
        SimLaserArmOff,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("LASER Switch - ARM")]
        [ShortDescription("Laser Sw Arm")]
        SimLaserArmOn,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("LASER Switch - Toggle")]
        [ShortDescription("Laser Sw Tog")]
        SimLaserArmToggle,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("ROLL Switch - Step Down")]
        [ShortDescription("AP Roll Step Dn")]
        SimLeftAPDec,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("ROLL Switch - STRG SEL")]
        [ShortDescription("AP Roll Strg Sel")]
        SimLeftAPDown,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("ROLL Switch - Step Up")]
        [ShortDescription("AP Roll Step Up")]
        SimLeftAPInc,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("ROLL Switch - ATT HOLD")]
        [ShortDescription("AP Roll Att Hold")]
        SimLeftAPMid,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("ROLL Switch - Cycle")]
        [ShortDescription("AP Roll Cyc")]
        SimLeftAPSwitch,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("ROLL Switch - HDG SEL")]
        [ShortDescription("AP Roll Hdg Sel")]
        SimLeftAPUp,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("MASTER ARM Switch - Step Down")]
        [ShortDescription("Master Arm Dn")]
        SimMasterArmDown,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("MASTER ARM Switch - Step Up")]
        [ShortDescription("Master Arm Up")]
        SimMasterArmUp,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("RF Switch - NORM")]
        [ShortDescription("RF Sw Norm")]
        SimRFNorm,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("RF Switch - QUIET")]
        [ShortDescription("RF Sw Quiet")]
        SimRFQuiet,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("RF Switch - SILENT")]
        [ShortDescription("RF Sw Silent")]
        SimRFSilent,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("RF Switch - Cycle")]
        [ShortDescription("RF Sw Cycle")]
        SimRFSwitch,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("RF Switch - Step Down")]
        [ShortDescription("RF Sw Dn")]
        SimRFSwitchDown,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("RF Switch - Step Up")]
        [ShortDescription("RF Sw Up")]
        SimRFSwitchUp,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("PITCH Switch - Step Down")]
        [ShortDescription("AP Pitch Step Dn")]
        SimRightAPDec,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("PITCH Switch - ATT HOLD")]
        [ShortDescription("AP Pitch Att Hold")]
        SimRightAPDown,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("PITCH Switch - Step Up")]
        [ShortDescription("AP Pitch Step Up")]
        SimRightAPInc,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("PITCH Switch - A / P OFF")]
        [ShortDescription("AP Pitch A / P Off")]
        SimRightAPMid,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("PITCH Switch - Cycle(also Combat AP)")]
        [ShortDescription("AP Pitch Cyc")]
        SimRightAPSwitch,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("PITCH Switch - ALT HOLD")]
        [ShortDescription("AP Pitch Alt Hold")]
        SimRightAPUp,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("MASTER ARM Switch - OFF")]
        [ShortDescription("Master Arm Off")]
        SimSafeMasterArm,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("MASTER ARM Switch - SIM")]
        [ShortDescription("Master Arm Sim")]
        SimSimMasterArm,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("MASTER ARM Switch - Cycle")]
        [ShortDescription("Master Arm Cyc")]
        SimStepMasterArm,

        [Category("CENTER CONSOLE")]
        [SubCategory("MISC PANEL")]
        [Description("ADV MODE - Toggle TFR On / Off")]
        [ShortDescription("TFR Toggle")]
        SimToggleTFR,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT EYEBROW")]
        [Description("MASTER CAUTION Button - Push")]
        [ShortDescription("Master Caution")]
        ExtinguishMasterCaution,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT EYEBROW")]
        [Description("F ACK Button - Push")]
        [ShortDescription("F ACK")]
        SimICPFAck,

        [Category("CENTER CONSOLE")]
        [SubCategory("TWP")]
        [Description("HANDOFF - Push")]
        [ShortDescription("TWP Handoff")]
        SimRWRHandoff,

        [Category("CENTER CONSOLE")]
        [SubCategory("TWP")]
        [Description("MISSILE LAUNCH - Push")]
        [ShortDescription("TWP Msl Launch")]
        SimRWRLaunch,

        [Category("CENTER CONSOLE")]
        [SubCategory("TWP")]
        [Description("PRIORITY MODE - Toggle")]
        [ShortDescription("TWP Priority")]
        SimRWRSetPriority,

        [Category("CENTER CONSOLE")]
        [SubCategory("TWP")]
        [Description("TGT SEP - Push")]
        [ShortDescription("TWP Tgt Sep")]
        SimRWRSetTargetSep,

        [Category("CENTER CONSOLE")]
        [SubCategory("TWP")]
        [Description("UNKNOWN - Toggle")]
        [ShortDescription("TWP Unkown")]
        SimRWRSetUnknowns,

        [Category("CENTER CONSOLE")]
        [SubCategory("TWP")]
        [Description("SYS TEST - Push")]
        [ShortDescription("TWP Sys Test")]
        SimRWRSysTest,

        [Category("CENTER CONSOLE")]
        [SubCategory("RWR")]
        [Description("Brightness Knob - Decrease")]
        [ShortDescription("RWR Brightn Dec")]
        SimRWRBrightnessDown,

        [Category("CENTER CONSOLE")]
        [SubCategory("RWR")]
        [Description("Brightness Knob - Increase")]
        [ShortDescription("RWR Brightn Inc")]
        SimRWRBrightnessUp,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 10 Button - Push")]
        [ShortDescription("LMFD OSB 10")]
        SimCBEOSB_10L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 11 Button - Push")]
        [ShortDescription("LMFD OSB 11")]
        SimCBEOSB_11L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 12 Button - Push")]
        [ShortDescription("LMFD OSB 12")]
        SimCBEOSB_12L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 13 Button - Push")]
        [ShortDescription("LMFD OSB 13")]
        SimCBEOSB_13L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 14 Button - Push")]
        [ShortDescription("LMFD OSB 14")]
        SimCBEOSB_14L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 15 Button - Push")]
        [ShortDescription("LMFD OSB 15")]
        SimCBEOSB_15L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 16 Button - Push")]
        [ShortDescription("LMFD OSB 16")]
        SimCBEOSB_16L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 17 Button - Push")]
        [ShortDescription("LMFD OSB 17")]
        SimCBEOSB_17L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 18 Button - Push")]
        [ShortDescription("LMFD OSB 18")]
        SimCBEOSB_18L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 19 Button - Push")]
        [ShortDescription("LMFD OSB 19")]
        SimCBEOSB_19L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 01 Button - Push")]
        [ShortDescription("LMFD OSB 01")]
        SimCBEOSB_1L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 20 Button - Push")]
        [ShortDescription("LMFD OSB 20")]
        SimCBEOSB_20L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 02 Button - Push")]
        [ShortDescription("LMFD OSB 02")]
        SimCBEOSB_2L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 03 Button - Push")]
        [ShortDescription("LMFD OSB 03")]
        SimCBEOSB_3L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 04 Button - Push")]
        [ShortDescription("LMFD OSB 04")]
        SimCBEOSB_4L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 05 Button - Push")]
        [ShortDescription("LMFD OSB 05")]
        SimCBEOSB_5L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 06 Button - Push")]
        [ShortDescription("LMFD OSB 06")]
        SimCBEOSB_6L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 07 Button - Push")]
        [ShortDescription("LMFD OSB 07")]
        SimCBEOSB_7L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 08 Button - Push")]
        [ShortDescription("LMFD OSB 08")]
        SimCBEOSB_8L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("OSB - 09 Button - Push")]
        [ShortDescription("LMFD OSB 09")]
        SimCBEOSB_9L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("BRT Button - Decrease Brightness")]
        [ShortDescription("LMFD Brt Dec")]
        SimCBEOSB_BRTDOWN_L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("BRT Button - Increase Brightness")]
        [ShortDescription("LMFD Brt Inc")]
        SimCBEOSB_BRTUP_L,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("GAIN Button - Decrease Sensor Gain")]
        [ShortDescription("Radar Gain Dec")]
        SimRadarGainDown,

        [Category("CENTER CONSOLE")]
        [SubCategory("LEFT MFD")]
        [Description("GAIN Button - Increase Sensor Gain")]
        [ShortDescription("Radar Gain Inc")]
        SimRadarGainUp,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("BRT Wheel - Decrease FLIR Intensity")]
        [ShortDescription("ICP FLIR BRT Dn")]
        SimBrtWheelDn,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("BRT Wheel - Increase FLIR Intensity")]
        [ShortDescription("ICP FLIR BRT Up")]
        SimBrtWheelUp,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("DRIFT C / O Switch - Tog.ON / NORM!")]
        [ShortDescription("ICP Drift c / o Tog")]
        SimDriftCO,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("DRIFT C / O Switch - NORM")]
        [ShortDescription("ICP Drift c / o Norm")]
        SimDriftCOOff,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("DRIFT C / O Switch - ON")]
        [ShortDescription("ICP Drift c / o On")]
        SimDriftCOOn,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("FLIR Rocker - Level Down")]
        [ShortDescription("ICP FLIR Lvl Dn")]
        SimFlirLevelDown,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("FLIR Rocker - Level Up")]
        [ShortDescription("ICP FLIR Lvl Up")]
        SimFlirLevelUp,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("SYM Wheel - HUD Power - OFF")]
        [ShortDescription("ICP Hud Pwr Off")]
        SimHUDOff,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("SYM Wheel - HUD Power - On")]
        [ShortDescription("ICP Hud Pwr On")]
        SimHUDOn,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("SYM Wheel - HUD Power - Toggle")]
        [ShortDescription("ICP Hud Pwr Tog")]
        SimHUDPower,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("A - A Button - Push")]
        [ShortDescription("ICP A - A")]
        SimICPAA,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("A - G Button - Push")]
        [ShortDescription("ICP A - G")]
        SimICPAG,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("2 - ALOW Button - Push")]
        [ShortDescription("ICP 2 - ALOW")]
        SimICPALOW,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("RCL Button - Push")]
        [ShortDescription("ICP RCL")]
        SimICPCLEAR,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("COM1 Button - Push")]
        [ShortDescription("ICP COM1")]
        SimICPCom1,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("COM2 Button - Push")]
        [ShortDescription("ICP COM2")]
        SimICPCom2,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("5 - CRUS Button - Push")]
        [ShortDescription("ICP 5 - CRUS")]
        SimICPCrus,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("DCS DOWN - Push")]
        [ShortDescription("ICP DCS Dn")]
        SimICPDEDDOWN,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("DCS SEQ(Right) - Push")]
        [ShortDescription("ICP DCS SEQ")]
        SimICPDEDSEQ,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("DCS UP - Push")]
        [ShortDescription("ICP DCS Up")]
        SimICPDEDUP,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("8 - FIX Button - Push")]
        [ShortDescription("ICP 8 - FIX")]
        SimICPEIGHT,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("ENTER Button - Push")]
        [ShortDescription("ICP ENTR")]
        SimICPEnter,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("IFF Button - Push")]
        [ShortDescription("ICP IFF")]
        SimICPIFF,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("LIST Button - Push")]
        [ShortDescription("ICP LIST")]
        SimICPLIST,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("7 - MARK Button - Push")]
        [ShortDescription("ICP 7 - MARK")]
        SimICPMark,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("NAV Mode(no such button In Pit)")]
        [ShortDescription("ICP NAV Mode")]
        SimICPNav,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("NEXT Button - Push")]
        [ShortDescription("ICP Next")]
        SimICPNext,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("9 - A - CAL Button - Push")]
        [ShortDescription("ICP 9 - A - CAL")]
        SimICPNINE,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("PREVIOUS Button - Push")]
        [ShortDescription("ICP Previous")]
        SimICPPrevious,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("DCS RTN(Left) - Push")]
        [ShortDescription("ICP DCS RTN")]
        SimICPResetDED,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("6 - TIME Button - Push")]
        [ShortDescription("ICP 6 - TIME")]
        SimICPSIX,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("4 - STPT Button - Push")]
        [ShortDescription("ICP 4 - STPT")]
        SimICPStpt,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("3 Button - Push")]
        [ShortDescription("ICP 3")]
        SimICPTHREE,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("1 - ILS Button - Push")]
        [ShortDescription("ICP 1 - ILS")]
        SimICPTILS,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("0 - M - SEL Button - Push")]
        [ShortDescription("ICP 0 - M - SEL")]
        SimICPZERO,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("DEPR RET Wheel - Step Down")]
        [ShortDescription("ICP Depr Ret Dn")]
        SimRetDn,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("DEPR RET Wheel - Step Up")]
        [ShortDescription("ICP Depr Ret Up")]
        SimRetUp,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("FLIR - WX Mode")]
        [ShortDescription("ICP FLIR WX")]
        SimSetWX,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("SYM Wheel - Decrease HUD Brightness")]
        [ShortDescription("ICP HudBrghtDec")]
        SimSymWheelDn,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("SYM Wheel - Increase HUD Brightness")]
        [ShortDescription("ICP HudBrght Inc")]
        SimSymWheelUp,

        [Category("CENTER CONSOLE")]
        [SubCategory("ICP")]
        [Description("DRIFT C / O Switch - WARN RESET")]
        [ShortDescription("ICP Drift c / o Warn")]
        SimWarnReset,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("Altimeter Pressure Knob - Decr. (5°)")]
        [ShortDescription("Alt Press - 5")]
        SimAltPressDec,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("Altimeter Pressure Knob - Decr. (1°)")]
        [ShortDescription("Alt Press - 1")]
        SimAltPressDecBy1,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("Altimeter Pressure Knob - Incr. (5°)")]
        [ShortDescription("Alt Press + 5")]
        SimAltPressInc,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("Altimeter Pressure Knob - Incr. (1°)")]
        [ShortDescription("Alt Press + 1")]
        SimAltPressIncBy1,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("HSI CRS Knob - Decrease(5°)")]
        [ShortDescription("HSI Crs Dec 5°")]
        SimHsiCourseDec,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("HSI CRS Knob - Increase(5°)")]
        [ShortDescription("HSI Crs Inc 5°")]
        SimHsiCourseInc,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("HSI CRS Knob - Decrease(1°)")]
        [ShortDescription("HSI Crs Dec 1°")]
        SimHsiCrsDecBy1,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("HSI CRS Knob - Increase(1°)")]
        [ShortDescription("HSI Crs Inc 1°")]
        SimHsiCrsIncBy1,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("HSI HDG Knob - Decrease(1°)")]
        [ShortDescription("HSI Hdg Dec 1°")]
        SimHsiHdgDecBy1,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("HSI HDG Knob - Increase(1°)")]
        [ShortDescription("HSI Hdg Inc 1°")]
        SimHsiHdgIncBy1,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("HSI HDG Knob - Decrease(5°)")]
        [ShortDescription("HSI Hdg Dec 5°")]
        SimHsiHeadingDec,

        [Category("CENTER CONSOLE")]
        [SubCategory("MAIN INSTRUMENT")]
        [Description("HSI HDG Knob - Increase(5°)")]
        [ShortDescription("HSI Hdg Inc 5°")]
        SimHsiHeadingInc,

        [Category("CENTER CONSOLE")]
        [SubCategory("INSTR MODE PANEL")]
        [Description("MODE Knob - ILS / NAV")]
        [ShortDescription("Instr Mode Ils / Nav")]
        SimHSIIlsNav,

        [Category("CENTER CONSOLE")]
        [SubCategory("INSTR MODE PANEL")]
        [Description("MODE Knob - ILS / TCN")]
        [ShortDescription("Instr Mode Ils / Tcn")]
        SimHSIIlsTcn,

        [Category("CENTER CONSOLE")]
        [SubCategory("INSTR MODE PANEL")]
        [Description("MODE Knob - Step Down")]
        [ShortDescription("Instr Mode Dn")]
        SimHSIModeDec,

        [Category("CENTER CONSOLE")]
        [SubCategory("INSTR MODE PANEL")]
        [Description("MODE Knob - Step Up")]
        [ShortDescription("Instr Mode Up")]
        SimHSIModeInc,

        [Category("CENTER CONSOLE")]
        [SubCategory("INSTR MODE PANEL")]
        [Description("MODE Knob - NAV")]
        [ShortDescription("Instr Mode Nav")]
        SimHSINav,

        [Category("CENTER CONSOLE")]
        [SubCategory("INSTR MODE PANEL")]
        [Description("MODE Knob - TCN")]
        [ShortDescription("Instr Mode Tcn")]
        SimHSITcn,

        [Category("CENTER CONSOLE")]
        [SubCategory("INSTR MODE PANEL")]
        [Description("MODE Knob - Cycle")]
        [ShortDescription("Instr Mode Cyc")]
        SimStepHSIMode,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("FUEL QTY SEL Knob - Step Down")]
        [ShortDescription("Fuel Qty Dn")]
        SimDecFuelSwitch,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("EXT FUEL TRANS Switch - Toggle")]
        [ShortDescription("Fuel Trans Tog")]
        SimExtFuelTrans,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("FUEL QTY SEL Knob - EXT CTR")]
        [ShortDescription("Fuel Trans Ext Ctr")]
        SimFuelSwitchCenterExt,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("FUEL QTY SEL Knob - NORM")]
        [ShortDescription("Fuel Trans Norm")]
        SimFuelSwitchNorm,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("FUEL QTY SEL Knob - RSVR")]
        [ShortDescription("Fuel Trans Rsvr")]
        SimFuelSwitchResv,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("FUEL QTY SEL Knob - TEST")]
        [ShortDescription("Fuel Trans Test")]
        SimFuelSwitchTest,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("FUEL QTY SEL Knob - EXT WING")]
        [ShortDescription("Fuel Trans Ext Wg")]
        SimFuelSwitchWingExt,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("FUEL QTY SEL Knob - INT WING")]
        [ShortDescription("Fuel Trans Int Wg")]
        SimFuelSwitchWingInt,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("EXT FUEL TRANS Switch - NORM")]
        [ShortDescription("Fuel Trans Norm")]
        SimFuelTransNorm,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("EXT FUEL TRANS Switch - WING FIRST")]
        [ShortDescription("Fuel Trans Wing")]
        SimFuelTransWing,

        [Category("CENTER CONSOLE")]
        [SubCategory("FUEL QTY PANEL")]
        [Description("FUEL QTY SEL Knob - Step Up")]
        [ShortDescription("Fuel Qty Up")]
        SimIncFuelSwitch,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 10 Button - Push")]
        [ShortDescription("RMFD OSB 10")]
        SimCBEOSB_10R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 11 Button - Push")]
        [ShortDescription("RMFD OSB 11")]
        SimCBEOSB_11R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 12 Button - Push")]
        [ShortDescription("RMFD OSB 12")]
        SimCBEOSB_12R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 13 Button - Push")]
        [ShortDescription("RMFD OSB 13")]
        SimCBEOSB_13R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 14 Button - Push")]
        [ShortDescription("RMFD OSB 14")]
        SimCBEOSB_14R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 15 Button - Push")]
        [ShortDescription("RMFD OSB 15")]
        SimCBEOSB_15R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 16 Button - Push")]
        [ShortDescription("RMFD OSB 16")]
        SimCBEOSB_16R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 17 Button - Push")]
        [ShortDescription("RMFD OSB 17")]
        SimCBEOSB_17R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 18 Button - Push")]
        [ShortDescription("RMFD OSB 18")]
        SimCBEOSB_18R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 19 Button - Push")]
        [ShortDescription("RMFD OSB 19")]
        SimCBEOSB_19R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 01 Button - Push")]
        [ShortDescription("RMFD OSB 01")]
        SimCBEOSB_1R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 20 Button - Push")]
        [ShortDescription("RMFD OSB 20")]
        SimCBEOSB_20R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 02 Button - Push")]
        [ShortDescription("RMFD OSB 02")]
        SimCBEOSB_2R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 03 Button - Push")]
        [ShortDescription("RMFD OSB 03")]
        SimCBEOSB_3R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 04 Button - Push")]
        [ShortDescription("RMFD OSB 04")]
        SimCBEOSB_4R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 05 Button - Push")]
        [ShortDescription("RMFD OSB 05")]
        SimCBEOSB_5R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 06 Button - Push")]
        [ShortDescription("RMFD OSB 06")]
        SimCBEOSB_6R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 07 Button - Push")]
        [ShortDescription("RMFD OSB 07")]
        SimCBEOSB_7R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 08 Button - Push")]
        [ShortDescription("RMFD OSB 08")]
        SimCBEOSB_8R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("OSB - 09 Button - Push")]
        [ShortDescription("RMFD OSB 09")]
        SimCBEOSB_9R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("BRT Button - Decrease Brightness")]
        [ShortDescription("RMFD Brt Dec")]
        SimCBEOSB_BRTDOWN_R,

        [Category("CENTER CONSOLE")]
        [SubCategory("RIGHT MFD")]
        [Description("BRT Button - Increase Brightness")]
        [ShortDescription("RMFD Brt Inc")]
        SimCBEOSB_BRTUP_R,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("FCR Switch - OFF")]
        [ShortDescription("FCR Sw Off")]
        SimFCROff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("FCR Switch - ON")]
        [ShortDescription("FCR Sw On")]
        SimFCROn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("FCR Switch - Toggle")]
        [ShortDescription("FCR Sw Tog")]
        SimFCRPower,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("LEFT HDPT Switch - OFF")]
        [ShortDescription("Left Hdpt Off")]
        SimLeftHptOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("LEFT HDPT Switch - ON")]
        [ShortDescription("Left Hdpt On")]
        SimLeftHptOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("LEFT HDPT Switch - Toggle")]
        [ShortDescription("Left Hdpt Tog")]
        SimLeftHptPower,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("RDR ALT Switch - Step Down")]
        [ShortDescription("RDR Alt Dn")]
        SimRALTDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("RDR ALT Switch - OFF")]
        [ShortDescription("RDR Alt Off")]
        SimRALTOFF,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("RDR ALT Switch - ON")]
        [ShortDescription("RDR Alt On")]
        SimRALTON,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("RDR ALT Switch - STDBY")]
        [ShortDescription("RDR Alt Stdby")]
        SimRALTSTDBY,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("RDR ALT Switch - Step Up")]
        [ShortDescription("RDR Alt Up")]
        SimRALTUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("RIGHT HDPT Switch - OFF")]
        [ShortDescription("Right Hdpt Off")]
        SimRightHptOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("RIGHT HDPT Switch - ON")]
        [ShortDescription("Right Hdpt On")]
        SimRightHptOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("SNSR PWR PANEL")]
        [Description("RIGHT HDPT Switch - Toggle")]
        [ShortDescription("Right Hdpt Tog")]
        SimRightHptPower,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Altitude Switch - AUTO")]
        [ShortDescription("HUDAlt Auto")]
        SimHUDAltAuto,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Altitude Switch - BARO")]
        [ShortDescription("HUD Alt Baro")]
        SimHUDAltBaro,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Altitude Switch - Step Dn")]
        [ShortDescription("HUD Alt Dn")]
        SimHUDAltDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Altitude Switch - RADAR")]
        [ShortDescription("HUD Alt Radar")]
        SimHUDAltRadar,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Altitude Switch - Step Up")]
        [ShortDescription("HUD Alt Up")]
        SimHUDAltUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Brightness Switch - Cycle")]
        [ShortDescription("HUD Brightn Cyc")]
        SimHUDBrightness,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Brightness Switch - Step Dn")]
        [ShortDescription("HUD Brightn Dn")]
        SimHUDBrightnessDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Brightness Switch - Step Up")]
        [ShortDescription("HUD Brightn Up")]
        SimHUDBrightnessUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Brightness Switch - AUTO BRT")]
        [ShortDescription("HUD Brightn Auto")]
        SimHUDBrtAuto,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Brightness Switch - DAY")]
        [ShortDescription("HUD Brightn Day")]
        SimHUDBrtDay,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Brightness Switch - NIG")]
        [ShortDescription("HUD Brightn Nig")]
        SimHUDBrtNight,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DED Data Switch - Cycle")]
        [ShortDescription("HUD DED Cyc")]
        SimHUDDED,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DED Data Switch - DED")]
        [ShortDescription("HUD DED Data")]
        SimHUDDEDDED,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DED Data Switch - Step Dn")]
        [ShortDescription("HUD DED Dn")]
        SimHUDDEDDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DED Data Switch - OFF")]
        [ShortDescription("HUD DED Off")]
        SimHUDDEDOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DED Data Switch - PFL")]
        [ShortDescription("HUD DED Pfl")]
        SimHUDDEDPFL,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DED Data Switch - Step Up")]
        [ShortDescription("HUD DED Up")]
        SimHUDDEDUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("FPM Switch - Cycle")]
        [ShortDescription("HUD FPM Cyc")]
        SimHUDFPM,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Altitude Switch - Cycle")]
        [ShortDescription("HUD Alt Cyc")]
        SimHUDRadar,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Scales Switch - Cycle")]
        [ShortDescription("HUD Scales Cyc")]
        SimHUDScales,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Scales Switch - Step Down")]
        [ShortDescription("HUD Scales Dn")]
        SimHUDScalesDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Scales Switch - Step Up")]
        [ShortDescription("HUD Scales Up")]
        SimHUDScalesUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Velocity Switch - Cycle")]
        [ShortDescription("HUD Velocity Cyc")]
        SimHUDVelocity,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Velocity Switch - CAS")]
        [ShortDescription("HUD Velocity Cas")]
        SimHUDVelocityCAS,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Velocity Switch - Step Dn")]
        [ShortDescription("HUD Velocity Dn")]
        SimHUDVelocityDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Velocity Switch - GND SPD")]
        [ShortDescription("HUD Velocity Gnd")]
        SimHUDVelocityGND,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Velocity Switch - TAS")]
        [ShortDescription("HUD Velocity Tas")]
        SimHUDVelocityTAS,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Velocity Switch - Step Up")]
        [ShortDescription("HUD Velocity Up")]
        SimHUDVelocityUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("FPM Switch - ATT / FPM")]
        [ShortDescription("HUD FPM Att / Hud")]
        SimPitchLadderATTFPM,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("FPM Switch - Step Down")]
        [ShortDescription("HUD FPM Dn")]
        SimPitchLadderDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("FPM Switch - FPM")]
        [ShortDescription("HUD FPM Fpm")]
        SimPitchLadderFPM,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("FPM Switch - OFF")]
        [ShortDescription("HUD FPM Off")]
        SimPitchLadderOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("FPM Switch - Step Up")]
        [ShortDescription("HUD FPM Up")]
        SimPitchLadderUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DEPR RET Switch - OFF")]
        [ShortDescription("HUD Depr Ret Off")]
        SimReticleOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DEPR RET Switch - PRI")]
        [ShortDescription("HUD Depr Ret Pri")]
        SimReticlePri,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DEPR RET Switch - STBY")]
        [ShortDescription("HUD DeprRet Sby")]
        SimReticleStby,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DEPR RET Switch - Cycle")]
        [ShortDescription("HUD DeprRet Cyc")]
        SimReticleSwitch,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DEPR RET Switch - Step Down")]
        [ShortDescription("HUD DeprRet Dn")]
        SimReticleSwitchDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("DEPR RET Switch - Step Up")]
        [ShortDescription("HUD DeprRet Up")]
        SimReticleSwitchUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Scales Switch - OFF")]
        [ShortDescription("HUD Scales Off")]
        SimScalesOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Scales Switch - VAH")]
        [ShortDescription("HUD Scales Vah")]
        SimScalesVAH,

        [Category("RIGHT CONSOLE")]
        [SubCategory("HUD PANEL")]
        [Description("Scales Switch - VV / VAH")]
        [ShortDescription("HUD Scls Vv / Vah")]
        SimScalesVVVAH,

        [Category("RIGHT CONSOLE")]
        [SubCategory("LIGHTING PANEL")]
        [Description("DED Knob(Primary) - Cycle")]
        [ShortDescription("Pri DED Cyc")]
        SimDedBrightness,

        [Category("RIGHT CONSOLE")]
        [SubCategory("LIGHTING PANEL")]
        [Description("DED Knob(Primary) - Step Down")]
        [ShortDescription("Pri DED Dn")]
        SimDedBrightnessCCW,

        [Category("RIGHT CONSOLE")]
        [SubCategory("LIGHTING PANEL")]
        [Description("DED Knob(Primary) - Step Up")]
        [ShortDescription("Pri DED Up")]
        SimDedBrightnessCW,

        [Category("RIGHT CONSOLE")]
        [SubCategory("LIGHTING PANEL")]
        [Description("INST PNL Knob(Primary) - Cycle")]
        [ShortDescription("Pri Inst Pnl Cyc")]
        SimInstrumentLight,

        [Category("RIGHT CONSOLE")]
        [SubCategory("LIGHTING PANEL")]
        [Description("INST PNL Knob(Primary) - Step Down")]
        [ShortDescription("Pri Inst Pnl Dn")]
        SimInstrumentLightCCW,

        [Category("RIGHT CONSOLE")]
        [SubCategory("LIGHTING PANEL")]
        [Description("INST PNL Knob(Primary) - Step Up")]
        [ShortDescription("Pri Inst Pnl Up")]
        SimInstrumentLightCW,

        [Category("RIGHT CONSOLE")]
        [SubCategory("LIGHTING PANEL")]
        [Description("CONSOLES Knob(Flood) - Cycle")]
        [ShortDescription("Fld Consoles Cyc")]
        SimInteriorLight,

        [Category("RIGHT CONSOLE")]
        [SubCategory("LIGHTING PANEL")]
        [Description("CONSOLES Knob(Flood) - Step Down")]
        [ShortDescription("Fld Consoles Dn")]
        SimInteriorLightCCW,

        [Category("RIGHT CONSOLE")]
        [SubCategory("LIGHTING PANEL")]
        [Description("CONSOLES Knob(Flood) - Step Up")]
        [ShortDescription("Fld Consoles Up")]
        SimInteriorLightCW,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AIR COND PANEL")]
        [Description("AIR SOURCE Knob - DUMP")]
        [ShortDescription("Air Source Dump")]
        SimAirSourceDump,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AIR COND PANEL")]
        [Description("AIR SOURCE Knob - NORM")]
        [ShortDescription("Air Source Norm")]
        SimAirSourceNorm,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AIR COND PANEL")]
        [Description("AIR SOURCE Knob - OFF")]
        [ShortDescription("Air Source Off")]
        SimAirSourceOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AIR COND PANEL")]
        [Description("AIR SOURCE Knob - RAM")]
        [ShortDescription("Air Source Ram")]
        SimAirSourceRam,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AIR COND PANEL")]
        [Description("AIR SOURCE Knob - Step Down")]
        [ShortDescription("Air Source Dn")]
        SimDecAirSource,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AIR COND PANEL")]
        [Description("AIR SOURCE Knob - Step Up")]
        [ShortDescription("Air Source Up")]
        SimIncAirSource,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ZEROIZE PANEL")]
        [Description("VMS Switch - Toggle")]
        [ShortDescription("VMS Sw Tog")]
        SimInhibitVMS,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ZEROIZE PANEL")]
        [Description("VMS Switch - INHIBIT")]
        [ShortDescription("VMS Sw Inhibit")]
        SimVMSOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ZEROIZE PANEL")]
        [Description("VMS Switch - ON")]
        [ShortDescription("VMS Sw On")]
        SimVMSOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("IFF UHF Switch - Cycle")]
        [ShortDescription("ANT SEL Cyc")]
        SimAntennaSelectCycle,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("IFF UHF Switch - Step Down")]
        [ShortDescription("ANT SEL DN")]
        SimAntennaSelectDec,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("IFF UHF Switch - LOWER")]
        [ShortDescription("ANT SEL Lower")]
        SimAntennaSelectDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("IFF UHF Switch - Step Up")]
        [ShortDescription("ANT SEL Up")]
        SimAntennaSelectInc,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("IFF UHF Switch - NORM")]
        [ShortDescription("ANT SEL Norm")]
        SimAntennaSelectMid,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("IFF UHF Switch - UPPER")]
        [ShortDescription("ANT SEL Upper")]
        SimAntennaSelectUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("ENGINE Switch - Cycle")]
        [ShortDescription("ICE Eng Cyc")]
        SimAntiIceCycle,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("ENGINE Switch - Step Down")]
        [ShortDescription("ICE Eng Dn")]
        SimAntiIceDec,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("ENGINE Switch - OFF")]
        [ShortDescription("ICE Eng Off")]
        SimAntiIceDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("ENGINE Switch - Step up")]
        [ShortDescription("ICE Eng Up")]
        SimAntiIceInc,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("ENGINE Switch - AUTO")]
        [ShortDescription("ICE Eng Auto")]
        SimAntiIceMid,

        [Category("RIGHT CONSOLE")]
        [SubCategory("ANTI ICE / ANT SEL PANEL")]
        [Description("ENGINE Switch - ON")]
        [ShortDescription("ICE Eng On")]
        SimAntiIceUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("DL Switch - OFF")]
        [ShortDescription("DL Sw Off")]
        SimDLOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("DL Switch - ON")]
        [ShortDescription("DL Sw On")]
        SimDLOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("DL Switch - Toggle")]
        [ShortDescription("DL Sw Tog")]
        SimDLPower,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("FCC Switch - OFF")]
        [ShortDescription("FCC Sw Off")]
        SimFCCOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("FCC Switch - ON")]
        [ShortDescription("FCC Sw On")]
        SimFCCOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("FCC Switch - Toggle")]
        [ShortDescription("FCC Sw Tog")]
        SimFCCPower,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("GPS Switch - OFF")]
        [ShortDescription("GPS Sw Off")]
        SimGPSOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("GPS Switch - ON")]
        [ShortDescription("GPS Sw On")]
        SimGPSOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("GPS Switch - Toggle")]
        [ShortDescription("GPS Sw Tog")]
        SimGPSPower,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("INS Knob - Step Down")]
        [ShortDescription("INS Knob Dn")]
        SimINSDec,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("INS Knob - Step Up")]
        [ShortDescription("INS Knob Up")]
        SimINSInc,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("INS Knob - IN FLT ALIGN")]
        [ShortDescription("INS Knob In Flt Ali")]
        SimINSInFlt,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("INS Knob - NAV")]
        [ShortDescription("INS Knob Nav")]
        SimINSNav,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("INS Knob - NORM")]
        [ShortDescription("INS Knob Norm")]
        SimINSNorm,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("INS Knob - OFF")]
        [ShortDescription("INS Knob Off")]
        SimINSOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("MAP Switch - OFF")]
        [ShortDescription("MAP Sw Off")]
        SimMAPOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("MAP Switch - ON")]
        [ShortDescription("MAP Sw On")]
        SimMAPOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("MAP Switch - Toggle")]
        [ShortDescription("MAP Sw Tog")]
        SimMAPPower,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("MFD Switch - OFF")]
        [ShortDescription("MFD Sw Off")]
        SimMFDOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("MFD Switch - ON")]
        [ShortDescription("MFD Sw On")]
        SimMFDOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("MFD Switch - Toggle")]
        [ShortDescription("MFD Sw Tog")]
        SimMFDPower,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("SMS Switch - OFF")]
        [ShortDescription("SMS Sw Off")]
        SimSMSOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("SMS Switch - ON")]
        [ShortDescription("SMS Sw On")]
        SimSMSOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("SMS Switch - Toggle")]
        [ShortDescription("SMS Sw Tog")]
        SimSMSPower,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("UFC Switch - OFF")]
        [ShortDescription("UFC Sw Off")]
        SimUFCOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("UFC Switch - ON")]
        [ShortDescription("UFC Sw On")]
        SimUFCOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("AVIONIC POWER PANEL")]
        [Description("UFC Switch - Toggle")]
        [ShortDescription("UFC Sw Tog")]
        SimUFCPower,

        [Category("RIGHT CONSOLE")]
        [SubCategory("OXYGEN PANEL")]
        [Description("Setting 2 - OFF(Pilot breathing)")]
        [ShortDescription("Oxy Set2 OFF")]
        SimOxySupplyOff,

        [Category("RIGHT CONSOLE")]
        [SubCategory("OXYGEN PANEL")]
        [Description("Setting 2 - ON(Pilot breathing)")]
        [ShortDescription("Oxy Set2 ON")]
        SimOxySupplyOn,

        [Category("RIGHT CONSOLE")]
        [SubCategory("OXYGEN PANEL")]
        [Description("Setting 2 - Toggle(Pilot breathing)")]
        [ShortDescription("Oxy Set2 Cyc")]
        SimOxySupplyToggle,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("TRIM Left - Roll Left")]
        [ShortDescription("Stick Trim Left")]
        AFAileronTrimLeft,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("TRIM Right - Roll Right")]
        [ShortDescription("Stick Trim Right")]
        AFAileronTrimRight,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("TRIM Down - Nose Up")]
        [ShortDescription("Stick Trim Ns Up")]
        AFElevatorTrimDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("TRIM Up - Nose Down")]
        [ShortDescription("Stick Trim Ns Dn")]
        AFElevatorTrimUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("PADDLE SWITCH")]
        [ShortDescription("Paddle Switch")]
        SimAPOverride,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("CMS Down")]
        [ShortDescription("CMS Down")]
        SimCMSDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("CMS Left")]
        [ShortDescription("CMS Left")]
        SimCMSLeft,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("CMS Right")]
        [ShortDescription("CMS Right")]
        SimCMSRight,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("CMS Up")]
        [ShortDescription("CMS Up")]
        SimCMSUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("DMS Down")]
        [ShortDescription("DMS Down")]
        SimDMSDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("DMS Left")]
        [ShortDescription("DMS Left")]
        SimDMSLeft,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("DMS Right")]
        [ShortDescription("DMS Right")]
        SimDMSRight,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("DMS Up")]
        [ShortDescription("DMS Up")]
        SimDMSUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("PINKY SWITCH(DX SHIFT)")]
        [ShortDescription("Pinky(DX Shift)")]
        SimHotasPinkyShift,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("NWS A / R DISC MSL STEP SWITCH")]
        [ShortDescription("NWS A / R MSL")]
        SimMissileStep,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("WEAPON RELEASE(Pickle)")]
        [ShortDescription("WPN Release")]
        SimPickle,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("PINKY SWITCH")]
        [ShortDescription("Pinky Switch")]
        SimPinkySwitch,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("TMS Down")]
        [ShortDescription("TMS Down")]
        SimTMSDown,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("TMS Left")]
        [ShortDescription("TMS Left")]
        SimTMSLeft,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("TMS Right")]
        [ShortDescription("TMS Right")]
        SimTMSRight,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("TMS Up")]
        [ShortDescription("TMS Up")]
        SimTMSUp,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("FIRST TRIGGER DETENT")]
        [ShortDescription("1st Trigger Det")]
        SimTriggerFirstDetent,

        [Category("RIGHT CONSOLE")]
        [SubCategory("FLIGHT STICK")]
        [Description("SECOND TRIGGER DETENT")]
        [ShortDescription("2nd Trigger Det")]
        SimTriggerSecondDetent,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("FLAPS - Decrease")]
        [ShortDescription("Flaps Dec")]
        AFDecFlap,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("LEFS - Decrease")]
        [ShortDescription("Lefs Dec")]
        AFDecLEF,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Drag Chute Deploy")]
        [ShortDescription("Drag Chute Depl")]
        AFDragChute,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("FLAPS - Set To Full")]
        [ShortDescription("Flaps Full")]
        AFFullFlap,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("LEFS - Set To Full")]
        [ShortDescription("Lefs Full")]
        AFFullLEF,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("FLAPS - Increase")]
        [ShortDescription("Flaps Inc")]
        AFIncFlap,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("LEFS - Increase")]
        [ShortDescription("Lefs Inc")]
        AFIncLEF,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("FLAPS - Set To Null")]
        [ShortDescription("Flaps Null")]
        AFNoFlap,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("LEFS - Set To Null")]
        [ShortDescription("Lefs Null")]
        AFNoLEF,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Trim - Reset(Change here)")]
        [ShortDescription("Trim Reset")]
        AFResetTrim,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("NAVOPS - Release Catapult Trigger")]
        [ShortDescription("Release Catapult")]
        AFTriggerCatapult,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Wing Fold - Down")]
        [ShortDescription("Wing Fold Dn")]
        AFWingFoldDown,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Wing Fold - Toggle")]
        [ShortDescription("Wing Fold Tog")]
        AFWingFoldToggle,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Wing Fold - Up")]
        [ShortDescription("Wing Fold Up")]
        AFWingFoldUp,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Cockpit Defaults - Load")]
        [ShortDescription("Load Ckpit Deflts")]
        LoadCockpitDefaults,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Disable Mouse Btns in 3D")]
        [ShortDescription("Mouse Btns dis")]
        OTWMouseButtonsIn3dDisable,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Enable Mouse Btns in 3D")]
        [ShortDescription("Mouse Btns en")]
        OTWMouseButtonsIn3dEnable,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Toggle Mouse Btns in 3D")]
        [ShortDescription("Mouse Btns Tog")]
        OTWMouseButtonsIn3dToggle,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("HUD Color - Cycle")]
        [ShortDescription("HUD Color Cyc")]
        OTWStepHudColor,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Cockpit Defaults - Save")]
        [ShortDescription("Save Ckpit Deflts")]
        SaveCockpitDefaults,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Drop Chaff(non EWS AC)")]
        [ShortDescription("Drop Chaff")]
        SimDropChaff,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Drop Flare(non EWS AC)")]
        [ShortDescription("Drop Flare")]
        SimDropFlare,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 FCS GAIN Switch - NORM")]
        [ShortDescription("F - 18 FCS Norm")]
        SimF18FCSGainNORM,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 FCS GAIN Switch - ORIDE")]
        [ShortDescription("F - 18 FCS Oride")]
        SimF18FCSGainORIDE,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 FCS GAIN Switch - Toggle")]
        [ShortDescription("F - 18 FCS Tog")]
        SimF18FCSGainToggle,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 FCS T / O TRIM Button")]
        [ShortDescription("F - 18 T / O Trim")]
        SimF18FCSTOTrim,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 Throttle - ATC Button")]
        [ShortDescription("F - 18 Throttle ATC")]
        SimF18ThrottleATC,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Dump Fuel")]
        [ShortDescription("Fuel Dump")]
        SimFuelDump,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 LAUNCH BAR Switch - EXTEND")]
        [ShortDescription("F - 18 LBar Extend")]
        SimLaunchBarEXTEND,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 LAUNCH BAR Switch - RETRACT")]
        [ShortDescription("F - 18 LBar Retract")]
        SimLaunchBarRETRACT,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 LAUNCH BAR Switch - Toggle")]
        [ShortDescription("F - 18 LBar Tog")]
        SimLaunchBarToggle,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Left Kneeboard - Dec")]
        [ShortDescription("Lt Kneeboard Dec")]
        SimLeftKneePadDec,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Left Kneeboard - Inc")]
        [ShortDescription("Lt Kneeboard Inc")]
        SimLeftKneePadInc,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Nightvision - Off")]
        [ShortDescription("NVG Off")]
        SimNVGModeOff,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Nightvision - On")]
        [ShortDescription("NVG On")]
        SimNVGModeOn,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Right Kneeboard - Dec")]
        [ShortDescription("Rt Kneeboard Dec")]
        SimRightKneePadDec,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Right Kneeboard - Inc")]
        [ShortDescription("Rt Kneeboard Inc")]
        SimRightKneePadInc,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Smoke - Off")]
        [ShortDescription("Smoke Off")]
        SimSmokeOff,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Smoke - On")]
        [ShortDescription("Smoke On")]
        SimSmokeOn,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Spotlight - Toggle")]
        [ShortDescription("Spotlight")]
        SimSpotLight,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Formation Lights - Step Down")]
        [ShortDescription("Form Lights Dn")]
        SimStepFormationLightsDown,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Formation Lights - Step Up")]
        [ShortDescription("Form Lights Up")]
        SimStepFormationLightsUp,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 FLAP Switch - AUTO")]
        [ShortDescription("F - 18 Flap Auto")]
        SimTEFCMDAuto,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 FLAP Switch - Step Down")]
        [ShortDescription("F - 18 Flap Down")]
        SimTEFCMDDec,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 FLAP Switch - FULL")]
        [ShortDescription("F - 18 Flap Full")]
        SimTEFCMDFull,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 FLAP Switch - HALF")]
        [ShortDescription("F - 18 Flap Half")]
        SimTEFCMDHalf,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("F - 18 FLAP Switch - Step Up")]
        [ShortDescription("F - 18 Flap Up")]
        SimTEFCMDInc,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Visor - Toggle")]
        [ShortDescription("Visor Toggle")]
        SimVisorToggle,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Wheel Brakes - Hold")]
        [ShortDescription("Wheel Brakes")]
        SimWheelBrakes,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Mouselook / Clickable Pit - Toggle ")]
        [ShortDescription("Clickabl Pit Mode")]
        ToggleClickablePitMode,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Nightvision - Toggle")]
        [ShortDescription("NVG Toggle")]
        ToggleNVGMode,

        [Category("MISCELLANEOUS")]
        [SubCategory("OTHER COCKPIT CALLBACKS")]
        [Description("Smoke - Toggle")]
        [ShortDescription("Smoke Toggle")]
        ToggleSmoke,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Bomb Burst Altitude Decrease")]
        [ShortDescription("Burst Alt Decr")]
        BombBurstDecrement,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Bomb Burst Altitude Increase")]
        [ShortDescription("Burst Alt Incr")]
        BombBurstIncrement,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Bomb Interval Decrement")]
        [ShortDescription("Bomb Interval Dec")]
        BombIntervalDecrement,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Bomb Interval Increment")]
        [ShortDescription("Bomb Interval Inc")]
        BombIntervalIncrement,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Bomb Pair Release")]
        [ShortDescription("Bomb Pair Rel")]
        BombPairRelease,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Bomb Ripple Dencrement")]
        [ShortDescription("Bomb Ripple Dec")]
        BombRippleDecrement,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Bomb Ripple Increment")]
        [ShortDescription("Bomb Ripple Inc")]
        BombRippleIncrement,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Bomb Single Release")]
        [ShortDescription("Bomb Single Rel")]
        BombSGLRelease,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Decrease ALOW")]
        [ShortDescription("ALOW Dec.")]
        DecreaseAlow,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Increase ALOW")]
        [ShortDescription("ALOW Inc.")]
        IncreaseAlow,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Step 3rd MFD(like DMS l / r)")]
        [ShortDescription("Step 3rd MFD")]
        OTWStepMFD3,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Step 4th MFD(like DMS l / r)")]
        [ShortDescription("Step 4th MFD")]
        OTWStepMFD4,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Swap MFDs")]
        [ShortDescription("Swap MFDs")]
        OTWSwapMFDS,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Toggle Jammer")]
        [ShortDescription("Toggle Jammer")]
        SimECMOn,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("HSD Range Decrease")]
        [ShortDescription("HSD Range Decr")]
        SimHSDRangeStepDown,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("HSD Range Increase")]
        [ShortDescription("HSD Range Incr")]
        SimHSDRangeStepUp,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Next AG Weapon")]
        [ShortDescription("Next AG Weapon")]
        SimNextAGWeapon,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Next Waypoint")]
        [ShortDescription("Next Waypoint")]
        SimNextWaypoint,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Previous Waypoint")]
        [ShortDescription("Prev.Waypoint")]
        SimPrevWaypoint,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Radar AA Mode Step")]
        [ShortDescription("AA Mode Step")]
        SimRadarAAModeStep,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Radar AG Mode Step")]
        [ShortDescription("AG Mode Step")]
        SimRadarAGModeStep,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Radar Azimuth Scan Change")]
        [ShortDescription("Azimuth Scan")]
        SimRadarAzimuthScanChange,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Radar Bar Scan Change")]
        [ShortDescription("Bar Scan")]
        SimRadarBarScanChange,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Radar Freeze")]
        [ShortDescription("Radar Freeze")]
        SimRadarFreeze,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Radar Range Down")]
        [ShortDescription("Radar Range Dn")]
        SimRadarRangeStepDown,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Radar Range Up")]
        [ShortDescription("Radar Range Up")]
        SimRadarRangeStepUp,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Radar Snowplow")]
        [ShortDescription("Radar Snowplow")]
        SimRadarSnowplow,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Toggle Missile Bore / Slave")]
        [ShortDescription("Missile Bore / Slave")]
        SimToggleMissileBoreSlave,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Toggle Missile Spot / Scan")]
        [ShortDescription("Missile Spot / Scan")]
        SimToggleMissileSpotScan,

        [Category("MISCELLANEOUS")]
        [SubCategory("SHORTCUTS")]
        [Description("Toggle Missile TD / BP")]
        [ShortDescription("Missile TD / BP")]
        SimToggleMissileTDBPUncage,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Throttle Full Afterburner")]
        [ShortDescription("Throttle Max AB")]
        AFABFull,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Throttle Min.Afterburner")]
        [ShortDescription("Throttle Min AB")]
        AFABOn,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Roll Left")]
        [ShortDescription("Roll Left")]
        AFAileronLeft,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Roll Right")]
        [ShortDescription("Roll Right")]
        AFAileronRight,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Throttle Step Down")]
        [ShortDescription("Throttle Step Dn")]
        AFCoarseThrottleDown,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Throttle Step Up")]
        [ShortDescription("Throttle Step Up")]
        AFCoarseThrottleUp,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("VTOL - EXHAUST - Decrease Angle")]
        [ShortDescription("Vtol Exhaust Dec")]
        AFDecExhaust,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Nose Down")]
        [ShortDescription("Nose Down")]
        AFElevatorDown,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Nose Up")]
        [ShortDescription("Nose Up")]
        AFElevatorUp,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Throttle Idle")]
        [ShortDescription("Throttle Idle")]
        AFIdle,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("VTOL - EXHAUST - Increase Angle")]
        [ShortDescription("Vtol Exhaust Inc")]
        AFIncExhaust,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Rudder Left")]
        [ShortDescription("Rudder Left")]
        AFRudderLeft,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Rudder Right")]
        [ShortDescription("Rudder Right")]
        AFRudderRight,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Throttle Back")]
        [ShortDescription("Throttle Back")]
        AFThrottleDown,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("Throttle Forward")]
        [ShortDescription("Throttle Fwd")]
        AFThrottleUp,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("ENGINE - Togg.Thrust Reverser")]
        [ShortDescription("Thrust Reverser")]
        AFTriggerReverseThrust,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("ENGINE - Cycle Engines")]
        [ShortDescription("Cycle Engines")]
        CycleEngine,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("ENGINE - Select Both Engines")]
        [ShortDescription("Both Engines")]
        selectBothEngines,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("ENGINE - Select Left Engine")]
        [ShortDescription("Left Engine")]
        selectLeftEngine,

        [Category("MISCELLANEOUS")]
        [SubCategory("KEYBOARD FLIGHT CONTROLS")]
        [Description("ENGINE - Select Right Engine")]
        [ShortDescription("Right Engine")]
        selectRightEngine,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 10 Button - Push")]
        [ShortDescription("TMFD OSB 10")]
        SimCBEOSB_10T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 11 Button - Push")]
        [ShortDescription("TMFD OSB 11")]
        SimCBEOSB_11T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 12 Button - Push")]
        [ShortDescription("TMFD OSB 12")]
        SimCBEOSB_12T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 13 Button - Push")]
        [ShortDescription("TMFD OSB 13")]
        SimCBEOSB_13T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 14 Button - Push")]
        [ShortDescription("TMFD OSB 14")]
        SimCBEOSB_14T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 15 Button - Push")]
        [ShortDescription("TMFD OSB 15")]
        SimCBEOSB_15T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 16 Button - Push")]
        [ShortDescription("TMFD OSB 16")]
        SimCBEOSB_16T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 17 Button - Push")]
        [ShortDescription("TMFD OSB 17")]
        SimCBEOSB_17T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 18 Button - Push")]
        [ShortDescription("TMFD OSB 18")]
        SimCBEOSB_18T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 19 Button - Push")]
        [ShortDescription("TMFD OSB 19")]
        SimCBEOSB_19T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 01 Button - Push")]
        [ShortDescription("TMFD OSB 01")]
        SimCBEOSB_1T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 20 Button - Push")]
        [ShortDescription("TMFD OSB 20")]
        SimCBEOSB_20T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 02 Button - Push")]
        [ShortDescription("TMFD OSB 02")]
        SimCBEOSB_2T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 03 Button - Push")]
        [ShortDescription("TMFD OSB 03")]
        SimCBEOSB_3T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 04 Button - Push")]
        [ShortDescription("TMFD OSB 04")]
        SimCBEOSB_4T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 05 Button - Push")]
        [ShortDescription("TMFD OSB 05")]
        SimCBEOSB_5T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 06 Button - Push")]
        [ShortDescription("TMFD OSB 06")]
        SimCBEOSB_6T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 07 Button - Push")]
        [ShortDescription("TMFD OSB 07")]
        SimCBEOSB_7T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 08 Button - Push")]
        [ShortDescription("TMFD OSB 08")]
        SimCBEOSB_8T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("OSB - 09 Button - Push")]
        [ShortDescription("TMFD OSB 09")]
        SimCBEOSB_9T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("BRT Button - Decrease Brightness")]
        [ShortDescription("TMFD Brt Dec")]
        SimCBEOSB_BRTDOWN_T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(THIRD)")]
        [Description("BRT Button - Increase Brightness")]
        [ShortDescription("TMFD Brt Inc")]
        SimCBEOSB_BRTUP_T,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 10 Button - Push")]
        [ShortDescription("FMFD OSB 10")]
        SimCBEOSB_10F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 11 Button - Push")]
        [ShortDescription("FMFD OSB 11")]
        SimCBEOSB_11F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 12 Button - Push")]
        [ShortDescription("FMFD OSB 12")]
        SimCBEOSB_12F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 13 Button - Push")]
        [ShortDescription("FMFD OSB 13")]
        SimCBEOSB_13F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 14 Button - Push")]
        [ShortDescription("FMFD OSB 14")]
        SimCBEOSB_14F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 15 Button - Push")]
        [ShortDescription("FMFD OSB 15")]
        SimCBEOSB_15F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 16 Button - Push")]
        [ShortDescription("FMFD OSB 16")]
        SimCBEOSB_16F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 17 Button - Push")]
        [ShortDescription("FMFD OSB 17")]
        SimCBEOSB_17F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 18 Button - Push")]
        [ShortDescription("FMFD OSB 18")]
        SimCBEOSB_18F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 19 Button - Push")]
        [ShortDescription("FMFD OSB 19")]
        SimCBEOSB_19F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 01 Button - Push")]
        [ShortDescription("FMFD OSB 01")]
        SimCBEOSB_1F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 20 Button - Push")]
        [ShortDescription("FMFD OSB 20")]
        SimCBEOSB_20F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 02 Button - Push")]
        [ShortDescription("FMFD OSB 02")]
        SimCBEOSB_2F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 03 Button - Push")]
        [ShortDescription("FMFD OSB 03")]
        SimCBEOSB_3F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 04 Button - Push")]
        [ShortDescription("FMFD OSB 04")]
        SimCBEOSB_4F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 05 Button - Push")]
        [ShortDescription("FMFD OSB 05")]
        SimCBEOSB_5F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 06 Button - Push")]
        [ShortDescription("FMFD OSB 06")]
        SimCBEOSB_6F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 07 Button - Push")]
        [ShortDescription("FMFD OSB 07")]
        SimCBEOSB_7F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 08 Button - Push")]
        [ShortDescription("FMFD OSB 08")]
        SimCBEOSB_8F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("OSB - 09 Button - Push")]
        [ShortDescription("FMFD OSB 09")]
        SimCBEOSB_9F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("BRT Button - Decrease Brightness")]
        [ShortDescription("FMFD Brt Dec")]
        SimCBEOSB_BRTDOWN_F,

        [Category("MISCELLANEOUS")]
        [SubCategory("EXTRA MFD(FOURTH)")]
        [Description("BRT Button - Increase Brightness")]
        [ShortDescription("FMFD Brt Inc")]
        SimCBEOSB_BRTUP_F,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Campaign - QuickSave(Host only)")]
        [ShortDescription("Camp Quick Save")]
        CampaignQuickSave,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Key Combination Keys(KeyCombo)")]
        [ShortDescription("KeyCombo")]
        CommandsSetKeyCombo,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Labels Far - Toggle")]
        [ShortDescription("Labels Far")]
        OTWToggleCampNames,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Engine Display Toggle")]
        [ShortDescription("Engine Display")]
        OTWToggleEngineDisplay,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Flap Display Toggle")]
        [ShortDescription("Flap Display")]
        OTWToggleFlapDisplay,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Display Frame Rate - Toggle")]
        [ShortDescription("Frame Rate")]
        OTWToggleFrameRate,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Toggle HUD Rendering")]
        [ShortDescription("HUD Rendering")]
        OTWToggleHUDRendering,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Labels Near - Toggle")]
        [ShortDescription("Labels Near")]
        OTWToggleNames,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Show Online Status - Toggle")]
        [ShortDescription("Online Status")]
        OTWToggleOnlinePlayersDisplay,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Show Score Display - Toggle")]
        [ShortDescription("Score Display")]
        OTWToggleScoreDisplay,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Pretty Filming(Hide Overlays)")]
        [ShortDescription("PrettyFilming")]
        PrettyFilm,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Pretty Screenshot(additional)")]
        [ShortDescription("PrettyScreenshot")]
        PrettyScreenShot,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Joystick Recenter")]
        [ShortDescription("Recenter Joystick")]
        RecenterJoystick,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("TrackIR Recenter(additional)")]
        [ShortDescription("Recenter TrackIR")]
        RecenterTrackIR,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("TrackIR Reload")]
        [ShortDescription("Reload TrackIR")]
        ReloadTrackIR,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Screenshot(additional)")]
        [ShortDescription("Add.Screenshot ")]
        ScreenShot,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Toggle Exit Sim Menu")]
        [ShortDescription("Exit Sim")]
        SimEndFlight,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("DX Shift")]
        [ShortDescription("DX Shift")]
        SimHotasShift,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Sim - Freeze - Toggle")]
        [ShortDescription("Toggle Freeze")]
        SimMotionFreeze,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Chat")]
        [ShortDescription("Open Chat Box")]
        SimOpenChatBox,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Sim - Pause")]
        [ShortDescription("Sim Pause")]
        SimPause,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Toggle Pilot Model")]
        [ShortDescription("Pilot Model")]
        SimPilotToggle,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Random Error")]
        [ShortDescription("Random Error")]
        SimRandomError,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Sim - Resume")]
        [ShortDescription("Sim Resume")]
        SimResume,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Sim - Pause - Toggle")]
        [ShortDescription("Toggle Pause")]
        SimTogglePaused,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Time Acceleration - Toggle 2x")]
        [ShortDescription("Time Accel 2x")]
        TimeAccelerate,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Time Acceleration - Step Down")]
        [ShortDescription("Time Accel Dn")]
        TimeAccelerateDec,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Time Acceleration - Step Up")]
        [ShortDescription("Time Accel Up")]
        TimeAccelerateInc,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Time Acceleration - Toggle 4x")]
        [ShortDescription("Time Accel 4x")]
        TimeAccelerateMaxToggle,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Toggle Infobar")]
        [ShortDescription("Infobar")]
        ToggleInfoBar,

        [Category("MISCELLANEOUS")]
        [SubCategory("SIMULATION & HARDWARE")]
        [Description("Toggle Radio Subtitles")]
        [ShortDescription("Subtitles")]
        ToggleSubTitles,

        [Category("MISCELLANEOUS")]
        [SubCategory("WINAMP")]
        [Description("Next Track")]
        [ShortDescription("WinAmp Next")]
        WinAmpNextTrack,

        [Category("MISCELLANEOUS")]
        [SubCategory("WINAMP")]
        [Description("Previous Track")]
        [ShortDescription("WinAmp Prev")]
        WinAmpPreviousTrack,

        [Category("MISCELLANEOUS")]
        [SubCategory("WINAMP")]
        [Description("Start Playback")]
        [ShortDescription("WinAmp Start")]
        WinAmpStartPlayback,

        [Category("MISCELLANEOUS")]
        [SubCategory("WINAMP")]
        [Description("Stop Playback")]
        [ShortDescription("WinAmp Stop")]
        WinAmpStopPlayback,

        [Category("MISCELLANEOUS")]
        [SubCategory("WINAMP")]
        [Description("Toggle Pause")]
        [ShortDescription("WinAmp Pause")]
        WinAmpTogglePause,

        [Category("MISCELLANEOUS")]
        [SubCategory("WINAMP")]
        [Description("Toggle Playback")]
        [ShortDescription("WinAmp Play")]
        WinAmpTogglePlayback,

        [Category("MISCELLANEOUS")]
        [SubCategory("WINAMP")]
        [Description("Volume Down")]
        [ShortDescription("WinAmp Vol Up")]
        WinAmpVolumeDown,

        [Category("MISCELLANEOUS")]
        [SubCategory("WINAMP")]
        [Description("Volume Up")]
        [ShortDescription("WinAmp Vol Dn")]
        WinAmpVolumeUp,

        [Category("MISCELLANEOUS")]
        [SubCategory("DEVELOPMENT")]
        [Description("Enter Position(EyeFly only)")]
        [ShortDescription("Dev Position")]
        DEV_OTWEnterPosition,

        [Category("MISCELLANEOUS")]
        [SubCategory("DEVELOPMENT")]
        [Description("Scale Down")]
        [ShortDescription("Dev Scale Down")]
        DEV_OTWScaleDown,

        [Category("MISCELLANEOUS")]
        [SubCategory("DEVELOPMENT")]
        [Description("Scale Up")]
        [ShortDescription("Dev Scale Up")]
        DEV_OTWScaleUp,

        [Category("MISCELLANEOUS")]
        [SubCategory("DEVELOPMENT")]
        [Description("Set Scale")]
        [ShortDescription("Dev Set Scale")]
        DEV_OTWSetScale,

        [Category("MISCELLANEOUS")]
        [SubCategory("DEVELOPMENT")]
        [Description("Location Display - Toggle")]
        [ShortDescription("Dev Location")]
        DEV_OTWToggleLocationDisplay,

        [Category("MISCELLANEOUS")]
        [SubCategory("DEVELOPMENT")]
        [Description("Debug Labels - Cycle")]
        [ShortDescription("Dev Debug")]
        DEV_SimCycleDebugLabels,

        [Category("MISCELLANEOUS")]
        [SubCategory("DEVELOPMENT")]
        [Description("Regenerate Mission(Dogfight only)")]
        [ShortDescription("Dev Regen")]
        DEV_SimRegen,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Decrease FOV – Or Mousewheel")]
        [ShortDescription("Decr FOV")]
        FOVDecrease,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Default FOV")]
        [ShortDescription("Default FOV")]
        FOVDefault,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Increase FOV – Or Mousewheel")]
        [ShortDescription("Incr FOV")]
        FOVIncrease,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Look Closer - Toggle")]
        [ShortDescription("Look Closer")]
        FOVToggle,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Rotate View Down")]
        [ShortDescription("View Down")]
        OTWViewDown,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Rotate View Down - Left")]
        [ShortDescription("View Down - Left")]
        OTWViewDownLeft,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Rotate View Down - Right")]
        [ShortDescription("View Down - Right")]
        OTWViewDownRight,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Rotate View Left")]
        [ShortDescription("View Left")]
        OTWViewLeft,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Rotate View Right")]
        [ShortDescription("View Right")]
        OTWViewRight,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Rotate View Up")]
        [ShortDescription("View Up")]
        OTWViewUp,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Rotate View Up - Left")]
        [ShortDescription("View Up - Left")]
        OTWViewUpLeft,

        [Category("VIEWS")]
        [SubCategory("VIEW GENERAL CONTROL")]
        [Description("Rotate View Up - Right")]
        [ShortDescription("View Up - Right")]
        OTWViewUpRight,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Glance Backward")]
        [ShortDescription("Glace Bckwd")]
        OTWCheckSix,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Glance Forward")]
        [ShortDescription("Glance Fwd")]
        OTWGlanceForward,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Next Custom 3dPit View")]
        [ShortDescription("Nxt Custom View")]
        OTWNextCustom3dPitView,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Previous Custom 3dPit View")]
        [ShortDescription("Prev Custom View")]
        OTWPrevCustom3dPitView,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Snap(3D) Cockpit")]
        [ShortDescription("Snap Pit(3D)")]
        OTWSelect2DCockpitMode,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Pan(3D) Cockpit")]
        [ShortDescription("Pan Pit(3D)")]
        OTWSelect3DCockpitMode,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Extended FOV")]
        [ShortDescription("Extended FOV")]
        OTWSelectEFOVPadlockMode,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock EFOV Mode = AA")]
        [ShortDescription("EFOV Mode AA")]
        OTWSelectEFOVPadlockModeAA,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock EFOV Mode = AG")]
        [ShortDescription("EFOV Mode AG")]
        OTWSelectEFOVPadlockModeAG,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock")]
        [ShortDescription("Padlock")]
        OTWSelectF3PadlockMode,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock Mode = AA")]
        [ShortDescription("Padlock Mode AA")]
        OTWSelectF3PadlockModeAA,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock Mode = AG")]
        [ShortDescription("Padlock Mode AG")]
        OTWSelectF3PadlockModeAG,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("HUD Only")]
        [ShortDescription("Hud Only")]
        OTWSelectHUDMode,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock next")]
        [ShortDescription("Padlock Next")]
        OTWStepNextPadlock,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock next AA")]
        [ShortDescription("Padlock Nxt AA")]
        OTWStepNextPadlockAA,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock next AG")]
        [ShortDescription("Padlock Nxt AG")]
        OTWStepNextPadlockAG,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock previous")]
        [ShortDescription("Padlock Prev")]
        OTWStepPrevPadlock,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock prev AA")]
        [ShortDescription("Padlock Prev AA")]
        OTWStepPrevPadlockAA,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Padlock prev AG")]
        [ShortDescription("Padlock Prev AG")]
        OTWStepPrevPadlockAG,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Toggle Empty Cockpit Shell")]
        [ShortDescription("Empty Ckpit Shell")]
        OTWToggle3DEmptyShell,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Toggle Custom 3dPit View")]
        [ShortDescription("Tog CustomView")]
        OTWToggleCustom3dPitView,

        [Category("VIEWS")]
        [SubCategory("VIEW INTERNAL")]
        [Description("Toggle SA bar")]
        [ShortDescription("SA - Bar")]
        OTWToggleSidebar,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Enemy Aircraft Camera")]
        [ShortDescription("Enemy AC Cam")]
        OTWSelectAirEnemyMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Friendly Aircraft Camera")]
        [ShortDescription("Friendly AC Cam")]
        OTWSelectAirFriendlyMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Chase Camera")]
        [ShortDescription("Chase Cam")]
        OTWSelectChaseMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Flyby Camera")]
        [ShortDescription("Flyby Cam")]
        OTWSelectFlybyMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Enemy Ground Unit Camera")]
        [ShortDescription("Enemy GU Cam")]
        OTWSelectGroundEnemyMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Friendly Ground Unit Camera")]
        [ShortDescription("Friendly GU Cam")]
        OTWSelectGroundFriendlyMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Incoming Camera")]
        [ShortDescription("Incoming Cam")]
        OTWSelectIncomingMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Next TopGun View")]
        [ShortDescription("TopGun Nxt")]
        OTWSelectNextTopGunView,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Orbit Camera")]
        [ShortDescription("Orbit Cam")]
        OTWSelectOrbitMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Prev TopGun View")]
        [ShortDescription("TopGun Prev")]
        OTWSelectPrevTopGunView,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Satellite Camera")]
        [ShortDescription("Satellite Cam")]
        OTWSelectSatelliteMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Target Camera")]
        [ShortDescription("Target Cam")]
        OTWSelectTargetMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("TopGun Camera")]
        [ShortDescription("TopGun Cam")]
        OTWSelectTopGunView,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Weapon Camera")]
        [ShortDescription("Weapon Cam")]
        OTWSelectWeaponMode,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Next Aircraft")]
        [ShortDescription("Next Aircraft")]
        OTWStepNextAC,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Previous Aircraft")]
        [ShortDescription("Prev Aircraft")]
        OTWStepPrevAC,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Action Camera")]
        [ShortDescription("Action Cam")]
        OTWToggleActionCamera,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Toggle EyeFly(Free Cam)")]
        [ShortDescription("EyFly Free Cam")]
        OTWToggleEyeFly,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Target - To - Self Camera")]
        [ShortDescription("Tgt to Self Cam")]
        OTWTrackExternal,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Target - to - Weapon Camera")]
        [ShortDescription("Tgt to Wpn Cam")]
        OTWTrackTargetToWeapon,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Reset View")]
        [ShortDescription("View Reset")]
        OTWViewReset,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Zoom In")]
        [ShortDescription("Zoom In")]
        OTWViewZoomIn,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Zoom Out")]
        [ShortDescription("Zoom Out")]
        OTWViewZoomOut,

        [Category("VIEWS")]
        [SubCategory("VIEW EXTERNAL")]
        [Description("Toggle Displacement Camera")]
        [ShortDescription("Displcmnt Cam")]
        ToggleDisplacementCam,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Menu Clear")]
        [ShortDescription("Radio Menu Clear")]
        OTWRadioMenuClear,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Radio - next menu")]
        [ShortDescription("Radio - next menu")]
        OTWRadioMenuStep,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Radio - previous menu")]
        [ShortDescription("Radio - previous menu")]
        OTWRadioMenuStepBack,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("AWACS Menu")]
        [ShortDescription("Awacs Menu")]
        RadioAWACSCommand,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Element Menu")]
        [ShortDescription("Element Menu")]
        RadioElementCommand,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Flight Menu")]
        [ShortDescription("Flight Menu")]
        RadioFlightCommand,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Menu Eight")]
        [ShortDescription("Radio Menu 8")]
        RadioMenuEight,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Menu Five")]
        [ShortDescription("Radio Menu 5")]
        RadioMenuFive,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Menu Four")]
        [ShortDescription("Radio Menu 4")]
        RadioMenuFour,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Menu Nine")]
        [ShortDescription("Radio Menu 9")]
        RadioMenuNine,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Menu One")]
        [ShortDescription("Radio Menu 1")]
        RadioMenuOne,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Menu Seven")]
        [ShortDescription("Radio Menu 7")]
        RadioMenuSeven,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Menu Six")]
        [ShortDescription("Radio Menu 6")]
        RadioMenuSix,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Menu Three")]
        [ShortDescription("Radio Menu 3")]
        RadioMenuThree,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Menu Two")]
        [ShortDescription("Radio Menu 2")]
        RadioMenuTwo,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Radio - send message")]
        [ShortDescription("Radio - send message")]
        RadioMessageSend,


        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Tanker Menu")]
        [ShortDescription("Tanker Menu")]
        RadioTankerCommand,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("ATC Menu")]
        [ShortDescription("ATC Menu")]
        RadioTowerCommand,

        [Category("RADIO COMMS")]
        [SubCategory("GENERAL RADIO OPTIONS")]
        [Description("Wingman Menu")]
        [ShortDescription("Wingman Menu")]
        RadioWingCommand,

        [Category("RADIO COMMS")]
        [SubCategory("AWACS COMMS")]
        [Description("Declare")]
        [ShortDescription("Awacs Declare")]
        AWACSDeclare,

        [Category("RADIO COMMS")]
        [SubCategory("AWACS COMMS")]
        [Description("Vector To Carrier Group")]
        [ShortDescription("Awacs Vect Carrie")]
        AWACSRequestCarrier,

        [Category("RADIO COMMS")]
        [SubCategory("AWACS COMMS")]
        [Description("Request Help")]
        [ShortDescription("Awacs Req Help")]
        AWACSRequestHelp,

        [Category("RADIO COMMS")]
        [SubCategory("AWACS COMMS")]
        [Description("Request Picture")]
        [ShortDescription("Awacs Picture")]
        AWACSRequestPicture,

        [Category("RADIO COMMS")]
        [SubCategory("AWACS COMMS")]
        [Description("Request Relief")]
        [ShortDescription("Awacs Req Relief")]
        AWACSRequestRelief,

        [Category("RADIO COMMS")]
        [SubCategory("AWACS COMMS")]
        [Description("Vector To Tanker")]
        [ShortDescription("Awacs Vect Tank")]
        AWACSRequestTanker,

        [Category("RADIO COMMS")]
        [SubCategory("AWACS COMMS")]
        [Description("Unable")]
        [ShortDescription("Awacs Unable")]
        AWACSUnable,

        [Category("RADIO COMMS")]
        [SubCategory("AWACS COMMS")]
        [Description("Vector To Nearest Threat")]
        [ShortDescription("Awacs Vector Tgt")]
        AWACSVectorToThreat,

        [Category("RADIO COMMS")]
        [SubCategory("AWACS COMMS")]
        [Description("Wilco")]
        [ShortDescription("Awacs Wilco")]
        AWACSWilco,

        [Category("RADIO COMMS")]
        [SubCategory("ATC COMMS")]
        [Description("Abort Approach")]
        [ShortDescription("Atc Abort")]
        ATCAbortApproach,

        [Category("RADIO COMMS")]
        [SubCategory("ATC COMMS")]
        [Description("Inbound For Landing")]
        [ShortDescription("Atc Inbound")]
        ATCRequestClearance,

        [Category("RADIO COMMS")]
        [SubCategory("ATC COMMS")]
        [Description("Declaring An Emergency")]
        [ShortDescription("Atc Emergency")]
        ATCRequestEmergencyClearance,

        [Category("RADIO COMMS")]
        [SubCategory("ATC COMMS")]
        [Description("Request Takeoff")]
        [ShortDescription("Atc Req TakeOff")]
        ATCRequestTakeoff,

        [Category("RADIO COMMS")]
        [SubCategory("ATC COMMS")]
        [Description("Request Taxi")]
        [ShortDescription("Atc Req Taxi")]
        ATCRequestTaxi,

        [Category("RADIO COMMS")]
        [SubCategory("TANKER COMMS")]
        [Description("Breakaway")]
        [ShortDescription("Tanker Break")]
        TankerBreakaway,

        [Category("RADIO COMMS")]
        [SubCategory("TANKER COMMS")]
        [Description("Done Refueling")]
        [ShortDescription("Tanker Done")]
        TankerDoneRefueling,

        [Category("RADIO COMMS")]
        [SubCategory("TANKER COMMS")]
        [Description("Ready For Gas")]
        [ShortDescription("Tanker Ready")]
        TankerReadyForGas,

        [Category("RADIO COMMS")]
        [SubCategory("TANKER COMMS")]
        [Description("Request Fuel")]
        [ShortDescription("Tanker Req Fuel")]
        TankerRequestFuel,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Arrowhead")]
        [ShortDescription("Wing Go Arrowh")]
        WingmanArrow,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Break Left")]
        [ShortDescription("Wing Break Left")]
        WingmanBreakLeft,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Break Right")]
        [ShortDescription("Wing Break Right")]
        WingmanBreakRight,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Chainsaw")]
        [ShortDescription("Wing Chainsaw")]
        WingmanChainsaw,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Check Your Six")]
        [ShortDescription("Wing Check Six")]
        WingmanCheckSix,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Clear My Six")]
        [ShortDescription("Wing Clear Six")]
        WingmanClearSix,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Close Up")]
        [ShortDescription("Wing Close Up")]
        WingmanCloseup,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Lower")]
        [ShortDescription("Wing Go Lower")]
        WingmanDecreaseRelAlt,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Attack Targets")]
        [ShortDescription("Wing Attck Tgts")]
        WingmanDesignateGroup,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Attack My Target")]
        [ShortDescription("Wing Attck my Tgt")]
        WingmanDesignateTarget,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Diamond")]
        [ShortDescription("Wing Go Diamond")]
        WingmanDiamond,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Drop Stores")]
        [ShortDescription("Wing Drop Stores")]
        WingmanDropStores,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Echelon Left")]
        [ShortDescription("Wing Go Echel.Lt")]
        WingmanEchelonLeft,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Echelon Right")]
        [ShortDescription("Wing Go Echel.Rt")]
        WingmanEchelonRight,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Flex")]
        [ShortDescription("Wing Flex")]
        WingmanFlex,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Fluid")]
        [ShortDescription("Wing Go Fluid")]
        WingmanFluid,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Say Position")]
        [ShortDescription("Wing Say Posit")]
        WingmanGiveBra,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Say Damage")]
        [ShortDescription("Wing Say Damge")]
        WingmanGiveDamageReport,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Say Fuel")]
        [ShortDescription("Wing Say Fuel")]
        WingmanGiveFuelState,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Say Status")]
        [ShortDescription("Wing Say Status")]
        WingmanGiveStatus,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Say Weapons")]
        [ShortDescription("Wing Say Wpns")]
        WingmanGiveWeaponsCheck,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Cover")]
        [ShortDescription("Wing Go Cover")]
        WingmanGoCoverMode,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Shooter")]
        [ShortDescription("Wing Go Shooter")]
        WingmanGoShooterMode,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Higher")]
        [ShortDescription("Wing Go Higher")]
        WingmanIncreaseRelAlt,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Kickout")]
        [ShortDescription("Wing Kickout")]
        WingmanKickout,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Ladder")]
        [ShortDescription("Wing Go Ladder")]
        WingmanLadder,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Line")]
        [ShortDescription("Wing Go Line")]
        WingmanLine,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Pince")]
        [ShortDescription("Wing Pince")]
        WingmanPince,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Posthole")]
        [ShortDescription("Wing Posthole")]
        WingmanPosthole,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Rejoin")]
        [ShortDescription("Wing Rejoin")]
        WingmanRejoin,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Resume Mission")]
        [ShortDescription("Wing Resume ")]
        WingmanResumeNormal,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Return to Base")]
        [ShortDescription("Wing RTB")]
        WingmanRTB,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Datalink Ground Target")]
        [ShortDescription("Wing Datalnk Gnd")]
        WingmanSendGrdDL,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Spread")]
        [ShortDescription("Wing Go Spread")]
        WingmanSpread,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Stack")]
        [ShortDescription("Wing Go Stack")]
        WingmanStack,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Switch Side")]
        [ShortDescription("Wing Switch Side")]
        WingmanToggleSide,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Trail")]
        [ShortDescription("Wing Go Trail")]
        WingmanTrail,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Weapons Free")]
        [ShortDescription("Wing Wpns Free")]
        WingmanWeaponsFree,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Weapons Hold")]
        [ShortDescription("Wing Wpns Hold")]
        WingmanWeaponsHold,

        [Category("RADIO COMMS")]
        [SubCategory("WINGMAN COMMANDS")]
        [Description("Go Wedge")]
        [ShortDescription("Wing Go Wedge")]
        WingmanWedge,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Arrowhead")]
        [ShortDescription("Elem Go Arrowh")]
        ElementArrow,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Break Left")]
        [ShortDescription("Elem Break Left")]
        ElementBreakLeft,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Break Right")]
        [ShortDescription("Elem Break Right")]
        ElementBreakRight,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Chainsaw")]
        [ShortDescription("Elem Chainsaw")]
        ElementChainsaw,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Check Your Six")]
        [ShortDescription("Elem Check Six")]
        ElementCheckSix,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Clear My Six")]
        [ShortDescription("Elem Clear Six")]
        ElementClearSix,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Close Up")]
        [ShortDescription("Elem Close Up")]
        ElementCloseup,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Lower")]
        [ShortDescription("Elem Go Lower")]
        ElementDecreaseRelAlt,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Attack Targets")]
        [ShortDescription("Elem Attck Tgts")]
        ElementDesignateGroup,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Attack My Target")]
        [ShortDescription("Elem Attck my Tgt")]
        ElementDesignateTarget,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Diamond")]
        [ShortDescription("Elem Go Diamnd")]
        ElementDiamond,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Drop Stores")]
        [ShortDescription("Elem Drop Stores")]
        ElementDropStores,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Echelon Left")]
        [ShortDescription("Elem Go Eche Lt")]
        ElementEchelonLeft,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Echelon Right")]
        [ShortDescription("Elem Go Eche Rt")]
        ElementEchelonRight,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Flex")]
        [ShortDescription("Elem Flex")]
        ElementFlex,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Fluid")]
        [ShortDescription("Elem Go Fluid")]
        ElementFluid,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Say Position")]
        [ShortDescription("Elem Say Posit")]
        ElementGiveBra,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Say Damage")]
        [ShortDescription("Elem Say Damge")]
        ElementGiveDamageReport,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Say Fuel")]
        [ShortDescription("Elem Say Fuel")]
        ElementGiveFuelState,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Say Status")]
        [ShortDescription("Elem Say Status")]
        ElementGiveStatus,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Say Weapons")]
        [ShortDescription("Elem Say Wpns")]
        ElementGiveWeaponsCheck,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Higher")]
        [ShortDescription("Elem Go Higher")]
        ElementIncreaseRelAlt,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Kickout")]
        [ShortDescription("Elem Kickout")]
        ElementKickout,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Ladder")]
        [ShortDescription("Elem Go Ladder")]
        ElementLadder,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Line")]
        [ShortDescription("Elem Go Line")]
        ElementLine,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Pince")]
        [ShortDescription("Elem Pince")]
        ElementPince,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Posthole")]
        [ShortDescription("Elem Posthole")]
        ElementPosthole,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Rejoin")]
        [ShortDescription("Elem Rejoin")]
        ElementRejoin,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Resume Mission")]
        [ShortDescription("Elem Resume ")]
        ElementResumeNormal,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Return to Base")]
        [ShortDescription("Elem RTB")]
        ElementRTB,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Datalink Ground Target")]
        [ShortDescription("Elem DatalnkGnd")]
        ElementSendGrnDL,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Spread")]
        [ShortDescription("Elem Go Spread")]
        ElementSpread,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Stack")]
        [ShortDescription("Elem Go Stack")]
        ElementStack,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Switch Side")]
        [ShortDescription("Elem Switch Side")]
        ElementToggleSide,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Trail")]
        [ShortDescription("Elem Go Trail")]
        ElementTrail,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Weapons Free")]
        [ShortDescription("Elem Wpns Free")]
        ElementWeaponsFree,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Weapons Hold")]
        [ShortDescription("Elem Wpns Hold")]
        ElementWeaponsHold,

        [Category("RADIO COMMS")]
        [SubCategory("ELEMENT COMMANDS")]
        [Description("Go Wedge")]
        [ShortDescription("Elem Go Wedge")]
        ElementWedge,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Arrowhead")]
        [ShortDescription("Flt Go Arrowh")]
        FlightArrow,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Box")]
        [ShortDescription("Flt Go Box")]
        FlightBox,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Break Left")]
        [ShortDescription("Flt Break Left")]
        FlightBreakLeft,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Break Right")]
        [ShortDescription("Flt Break Right")]
        FlightBreakRight,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Chainsaw")]
        [ShortDescription("Flt Chainsaw")]
        FlightChainsaw,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Check Your Six")]
        [ShortDescription("Flt Check Six")]
        FlightCheckSix,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Clear My Six")]
        [ShortDescription("Flt Clear Six")]
        FlightClearSix,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Close Up")]
        [ShortDescription("Flt Close Up")]
        FlightCloseup,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Lower")]
        [ShortDescription("Flt Go Lower")]
        FlightDecreaseRelAlt,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Attack Target")]
        [ShortDescription("Flt Attck Tgts")]
        FlightDesignateGroup,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Attack My Target")]
        [ShortDescription("Flt Attck my Tgt")]
        FlightDesignateTarget,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Diamond")]
        [ShortDescription("Flt Go Diamond")]
        FlightDiamond,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Drop Store")]
        [ShortDescription("Flt Drop Stores")]
        FlightDropStores,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Echolon Right")]
        [ShortDescription("Flt Go Echolon Lt")]
        FlightEchelonLeft,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Echolon Left")]
        [ShortDescription("Flt Go Echolon Rt")]
        FlightEchelonRight,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Finger Four")]
        [ShortDescription("Flt Go Finger 4")]
        FlightFinger4,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Flex")]
        [ShortDescription("Flt Flex")]
        FlightFlex,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Fluid")]
        [ShortDescription("Flt Go Fluid")]
        FlightFluid,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Say Position")]
        [ShortDescription("Flt Say Posit")]
        FlightGiveBra,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Say Damage")]
        [ShortDescription("Flt Say Damge")]
        FlightGiveDamageReport,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Say Fuel")]
        [ShortDescription("Flt Say Fuel")]
        FlightGiveFuelState,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Say Status")]
        [ShortDescription("Flt Say Status")]
        FlightGiveStatus,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Say Weapon")]
        [ShortDescription("Flt Say Wpns")]
        FlightGiveWeaponsCheck,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Higher")]
        [ShortDescription("Flt Go Higher")]
        FlightIncreaseRelAlt,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Kickout")]
        [ShortDescription("Flt Kickout")]
        FlightKickout,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Ladder")]
        [ShortDescription("Flt Go Ladder")]
        FlightLadder,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Line")]
        [ShortDescription("Flt Go Line")]
        FlightLine,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Pince")]
        [ShortDescription("Flt Pince")]
        FlightPince,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Posthole")]
        [ShortDescription("Flt Posthole")]
        FlightPosthole,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Rejoin")]
        [ShortDescription("Flt Rejoin")]
        FlightRejoin,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Res Cell")]
        [ShortDescription("Flt Go Res Cell")]
        FlightResCell,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Resume Mission")]
        [ShortDescription("Flt Resume ")]
        FlightResumeNormal,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Return to Base")]
        [ShortDescription("Flt RTB")]
        FlightRTB,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Datalink Ground Target")]
        [ShortDescription("Flt DatalnkGnd")]
        FlightSendGrnDL,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Spread")]
        [ShortDescription("Flt Go Spread")]
        FlightSpread,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Stack")]
        [ShortDescription("Flt Go Stack")]
        FlightStack,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Switch Side")]
        [ShortDescription("Flt Switch Side")]
        FlightToggleSide,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go Trail")]
        [ShortDescription("Flt Go Trail")]
        FlightTrail,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Go VIC")]
        [ShortDescription("Flt Go Vic")]
        FlightVic,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Weapons Free")]
        [ShortDescription("Flt Wpns Free")]
        FlightWeaponsFree,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Weapons Hold")]
        [ShortDescription("Flt Wpns Hold")]
        FlightWeaponsHold,

        [Category("RADIO COMMS")]
        [SubCategory("FLIGHT COMMANDS")]
        [Description("Wedge")]
        [ShortDescription("Flt Go Wedge")]
        FlightWedge,
    }
}
