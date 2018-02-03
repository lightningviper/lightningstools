using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.VirtualJoystick.Finalize():System.Void")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.NativeMethods.DeviceIoControl(Microsoft.Win32.SafeHandles.SafeFileHandle,System.UInt32,System.Object,System.UInt32,System.Object,System.UInt32,System.UInt32&,System.Threading.NativeOverlapped&):System.Boolean"
        )]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.NativeMethods.DeviceIoControl(Microsoft.Win32.SafeHandles.SafeFileHandle,PPJoy.NativeMethods+EIOControlCode,System.Object,System.UInt32,System.Object,System.UInt32,System.UInt32&,System.Threading.NativeOverlapped&):System.Boolean"
        )]
[assembly: SuppressMessage("Microsoft.Design", "CA1017:MarkAssembliesWithComVisible")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.GetMappings(PPJoy.Device,PPJoy.JoystickMapScope):PPJoy.MappingCollection")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member",
        Target =
            "PPJoy.DeviceManager.GetMappings(PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope):PPJoy.MappingCollection"
        )]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Scope = "member",
        Target =
            "PPJoy.DeviceManager.GetMappings(PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope):PPJoy.MappingCollection"
        , MessageId = "unitNum+1")]
[assembly: SuppressMessage("Microsoft.Usage", "CA2209:AssembliesShouldDeclareMinimumSecurity")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.DeviceManager.GetMappings(PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope):PPJoy.MappingCollection"
        )]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.RemoveDevice(PPJoy.JoystickTypes,System.Int32):System.Void")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.RemoveMapping(PPJoy.Device,PPJoy.JoystickMapScope):System.Void")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member",
        Target =
            "PPJoy.DeviceManager.RemoveMapping(PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope):System.Void")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.DeviceManager.RemoveMapping(PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope):System.Void")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.DeviceManager.SetMapping(PPJoy.Device,PPJoy.JoystickMapScope,PPJoy.MappingCollection):System.Void")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Scope = "member",
        Target =
            "PPJoy.DeviceManager.SetMapping(PPJoy.Device,PPJoy.JoystickMapScope,PPJoy.MappingCollection):System.Void")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.SetMapping(PPJoy.Device,PPJoy.MappingCollection):System.Void")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member",
        Target =
            "PPJoy.DeviceManager.SetMapping(PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope,PPJoy.MappingCollection):System.Void"
        )]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Scope = "member",
        Target =
            "PPJoy.DeviceManager.SetMapping(PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope,PPJoy.MappingCollection):System.Void"
        , MessageId = "unitNum+1")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.DeviceManager.SetMapping(PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope,PPJoy.MappingCollection):System.Void"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Scope = "member",
        Target = "PPJoy.MappingCollection.CopyTo(System.Array,System.Int32):System.Void",
        MessageId = "System.ArgumentException.#ctor(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Scope = "member",
        Target = "PPJoy.MappingCollection.Insert(System.Int32,System.Object):System.Void",
        MessageId = "System.ArgumentException.#ctor(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member",
        Target = "PPJoy.MappingCollection.System.Collections.IList.Item[System.Int32]")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Scope = "member",
        Target = "PPJoy.MappingCollection.System.Collections.IList.Item[System.Int32]",
        MessageId = "System.ArgumentException.#ctor(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.AxisMapping")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.ButtonMapping")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.ContinuousPovMapping")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pov", Scope = "type",
        Target = "PPJoy.ContinuousPovMapping")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.Device")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "type",
        Target = "PPJoy.Device")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#Delete(System.Boolean,System.Boolean)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#DeviceType")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#GetMappings()")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#GetMappings(PPJoy.JoystickMapScope)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lpt",
        Scope = "member", Target = "PPJoy.Device.#LptNum")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Num",
        Scope = "member", Target = "PPJoy.Device.#LptNum")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#LptNum")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#ProductId")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#RemoveMappings()")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#RemoveMappings(PPJoy.JoystickMapScope)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#SetMappings(PPJoy.JoystickMapScope,PPJoy.MappingCollection)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#SetMappings(PPJoy.MappingCollection)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SubType",
        Scope = "member", Target = "PPJoy.Device.#SubType")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#SubType")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Num",
        Scope = "member", Target = "PPJoy.Device.#UnitNum")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#UnitNum")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.Device.#VendorId")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.DeviceAlreadyExistsException")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.DeviceManager")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "type",
        Target = "PPJoy.DeviceManager")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "subType",
        Scope = "member",
        Target =
            "PPJoy.DeviceManager.#CreateDevice(System.Int32,PPJoy.JoystickTypes,PPJoy.JoystickSubTypes,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Num",
        Scope = "member",
        Target =
            "PPJoy.DeviceManager.#CreateDevice(System.Int32,PPJoy.JoystickTypes,PPJoy.JoystickSubTypes,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "lpt",
        Scope = "member",
        Target =
            "PPJoy.DeviceManager.#CreateDevice(System.Int32,PPJoy.JoystickTypes,PPJoy.JoystickSubTypes,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.DeviceManager.#CreateDevice(System.Int32,PPJoy.JoystickTypes,PPJoy.JoystickSubTypes,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.#DeleteAllDevices(System.Boolean,System.Boolean)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.#GetAllDevices()")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Num",
        Scope = "member", Target = "PPJoy.DeviceManager.#GetDevice(System.Int32,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "lpt",
        Scope = "member", Target = "PPJoy.DeviceManager.#GetDevice(System.Int32,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.#GetDevice(System.Int32,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByProduct",
        Scope = "member", Target = "PPJoy.DeviceManager.#GetDeviceByProductId(System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member",
        Target = "PPJoy.DeviceManager.#GetDeviceByProductId(System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.#GetDeviceByProductId(System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Num",
        Scope = "member",
        Target =
            "PPJoy.DeviceManager.#GetDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope)"
        )]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "lpt",
        Scope = "member",
        Target =
            "PPJoy.DeviceManager.#GetDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope)"
        )]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.DeviceManager.#GetDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope)"
        )]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.#IdealMappings")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.#IsVirtualDevice(System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member",
        Target = "PPJoy.DeviceManager.#MaxValidUnitNumber(PPJoy.JoystickTypes)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.DeviceManager.#MaxValidUnitNumber(PPJoy.JoystickTypes)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Num",
        Scope = "member",
        Target =
            "PPJoy.DeviceManager.#RemoveDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope)"
        )]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "lpt",
        Scope = "member",
        Target =
            "PPJoy.DeviceManager.#RemoveDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope)"
        )]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.DeviceManager.#RemoveDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope)"
        )]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Num",
        Scope = "member",
        Target =
            "PPJoy.DeviceManager.#SetDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope,PPJoy.MappingCollection)"
        )]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "lpt",
        Scope = "member",
        Target =
            "PPJoy.DeviceManager.#SetDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope,PPJoy.MappingCollection)"
        )]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.DeviceManager.#SetDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope,PPJoy.MappingCollection)"
        )]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.DirectionalPovMapping")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.DeviceNotFoundException")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Scope = "type",
        Target = "PPJoy.AxisDataSources")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Scope = "type",
        Target = "PPJoy.AxisTypes")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pov",
        Scope = "member", Target = "PPJoy.AxisTypes.#Pov")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "VBRX", Scope = "member"
        , Target = "PPJoy.AxisTypes.#VBRX")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "VBRY", Scope = "member"
        , Target = "PPJoy.AxisTypes.#VBRY")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "VBRZ", Scope = "member"
        , Target = "PPJoy.AxisTypes.#VBRZ")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X", Scope = "member",
        Target = "PPJoy.AxisTypes.#X")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y", Scope = "member",
        Target = "PPJoy.AxisTypes.#Y")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Z", Scope = "member",
        Target = "PPJoy.AxisTypes.#Z")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Scope = "type",
        Target = "PPJoy.ButtonDataSources")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Scope = "type",
        Target = "PPJoy.ContinuousPovDataSources")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pov", Scope = "type",
        Target = "PPJoy.ContinuousPovDataSources")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Scope = "type",
        Target = "PPJoy.ContinuousPovDataSources")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pov", Scope = "type",
        Target = "PPJoy.DirectionalPovDataSources")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Scope = "type",
        Target = "PPJoy.DirectionalPovDataSources")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Scope = "type",
        Target = "PPJoy.JoystickMapScope")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SubTypes",
        Scope = "type", Target = "PPJoy.JoystickSubTypes")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Scope = "type",
        Target = "PPJoy.JoystickSubTypes")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickSubTypes.#Genesis_Pad_A_B_C_Start")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickSubTypes.#Genesis_Pad_A_B_C_X_Y_Z_Start_Mode")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "NES", Scope = "member",
        Target = "PPJoy.JoystickSubTypes.#NES")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SNES", Scope = "member"
        , Target = "PPJoy.JoystickSubTypes.#SNES_or_Virtual_Gameboy")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "or", Scope = "member",
        Target = "PPJoy.JoystickSubTypes.#SNES_or_Virtual_Gameboy")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gameboy",
        Scope = "member", Target = "PPJoy.JoystickSubTypes.#SNES_or_Virtual_Gameboy")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickSubTypes.#SNES_or_Virtual_Gameboy")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Scope = "type",
        Target = "PPJoy.JoystickTypes")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Scope = "type",
        Target = "PPJoy.JoystickTypes")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Genesis_Pad_ConsoleCable")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Genesis_Pad_DirectPad_Pro")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Genesis_Pad_DirectPad_Pro_V6")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Genesis_Pad_Linux")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Genesis_Pad_NTPad_XP")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SNES", Scope = "member"
        , Target = "PPJoy.JoystickTypes.#Genesis_Pad_SNESKey")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Genesis_Pad_SNESKey")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_Amiga_4_Player")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CHAM", Scope = "member"
        , Target = "PPJoy.JoystickTypes.#Joystick_CHAMPgames")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pgames",
        Scope = "member", Target = "PPJoy.JoystickTypes.#Joystick_CHAMPgames")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_CHAMPgames")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_DirectPad_Pro")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Herries",
        Scope = "member", Target = "PPJoy.JoystickTypes.#Joystick_IanHerries")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_IanHerries")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "c", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_Linux_DB9c")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_Linux_DB9c")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "gamecon",
        Scope = "member", Target = "PPJoy.JoystickTypes.#Joystick_Linux_gamecon")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "gamecon",
        Scope = "member", Target = "PPJoy.JoystickTypes.#Joystick_Linux_gamecon")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_Linux_gamecon")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "v", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_Linux_v0802")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_Linux_v0802")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "JoyStick",
        Scope = "member", Target = "PPJoy.JoystickTypes.#Joystick_LPT_JoyStick")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "LPT", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_LPT_JoyStick")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_LPT_JoyStick")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Tswitch",
        Scope = "member", Target = "PPJoy.JoystickTypes.#Joystick_LPTswitch")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_LPTswitch")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PCAE", Scope = "member"
        , Target = "PPJoy.JoystickTypes.#Joystick_PCAE")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_PCAE")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SNES", Scope = "member"
        , Target = "PPJoy.JoystickTypes.#Joystick_SNESKey2600")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_SNESKey2600")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_STFormat")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_TheMaze")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_TorMod")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gra",
        Scope = "member", Target = "PPJoy.JoystickTypes.#Joystick_TurboGraFX")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_TurboGraFX")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gra",
        Scope = "member", Target = "PPJoy.JoystickTypes.#Joystick_TurboGraFX_SwappedButtons")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Joystick_TurboGraFX_SwappedButtons")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Playstation_Pad_DirectPad_Pro")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Playstation_Pad_Linux")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Megatap",
        Scope = "member", Target = "PPJoy.JoystickTypes.#Playstation_Pad_Megatap")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Playstation_Pad_Megatap")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Playstation_Pad_NTPad_XP")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PSXPB",
        Scope = "member", Target = "PPJoy.JoystickTypes.#Playstation_Pad_PSXPBLib")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Playstation_Pad_PSXPBLib")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Radio_Control_TX")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "NES", Scope = "member",
        Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_DirectPadPro_Or_SNESKey")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SNES", Scope = "member"
        , Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_DirectPadPro_Or_SNESKey")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "or", Scope = "member",
        Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_DirectPadPro_Or_SNESKey")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_DirectPadPro_Or_SNESKey")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "NES", Scope = "member",
        Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_Linux")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SNES", Scope = "member"
        , Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_Linux")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "or", Scope = "member",
        Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_Linux")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_Linux")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "NES", Scope = "member",
        Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_PowerPad")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SNES", Scope = "member"
        , Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_PowerPad")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "or", Scope = "member",
        Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_PowerPad")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#SNES_or_NESPad_PowerPad")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member",
        Target = "PPJoy.JoystickTypes.#Virtual_Joystick")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.Mapping")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.MappingCollection")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pov",
        Scope = "member", Target = "PPJoy.MappingCollection.#PovMappings")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target =
            "PPJoy.NativeMethods.#CreateFile(System.String,PPJoy.NativeMethods+EFileAccess,PPJoy.NativeMethods+EFileShare,System.IntPtr,PPJoy.NativeMethods+ECreationDisposition,PPJoy.NativeMethods+EFileAttributes,Microsoft.Win32.SafeHandles.SafeFileHandle)"
        )]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.OperationFailedException")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.PovMapping")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pov", Scope = "type",
        Target = "PPJoy.PovMapping")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.PPJoyException")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1058:TypesShouldNotExtendCertainBaseTypes", Scope = "type",
        Target = "PPJoy.PPJoyException")]
[assembly:
    SuppressMessage("Microsoft.Interoperability", "CA1408:DoNotUseAutoDualClassInterfaceType", Scope = "type",
        Target = "PPJoy.VirtualJoystick")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.VirtualJoystick.#Dispose()")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Val",
        Scope = "member", Target = "PPJoy.VirtualJoystick.#MaxAnalogDataSourceVal")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Povs",
        Scope = "member", Target = "PPJoy.VirtualJoystick.#MaxVisiblePovs")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Val",
        Scope = "member", Target = "PPJoy.VirtualJoystick.#MinAnalogDataSourceVal")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pov",
        Scope = "member", Target = "PPJoy.VirtualJoystick.#PovCentered")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.VirtualJoystick.#SendUpdates()")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Num",
        Scope = "member", Target = "PPJoy.VirtualJoystick.#SetAnalogDataSourceValue(System.Int32,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.VirtualJoystick.#SetAnalogDataSourceValue(System.Int32,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Num",
        Scope = "member", Target = "PPJoy.VirtualJoystick.#SetDigitalDataSourceState(System.Int32,System.Boolean)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.VirtualJoystick.#SetDigitalDataSourceState(System.Int32,System.Boolean)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Scope = "member",
        Target = "PPJoy.VirtualJoystick.#VirtualStickNumber")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison",
        MessageId = "System.String.StartsWith(System.String)", Scope = "member",
        Target = "PPJoy.DeviceManager.#ReadMapData(PPJoy.JoystickMapPayload)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pov", Scope = "type",
        Target = "PPJoy.DirectionalPovMapping")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "PPJoy.NativeMethods.#CloseHandle(Microsoft.Win32.SafeHandles.SafeFileHandle)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Scope = "member", Target = "PPJoy.MappingComparer.#Compare(PPJoy.Mapping,PPJoy.Mapping)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "PPJoy.MappingComparer.#Compare(PPJoy.Mapping,PPJoy.Mapping)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0", Scope = "member", Target = "PPJoy.NativeMethods.#CreateFile(System.String,PPJoy.NativeMethods+EFileAccess,PPJoy.NativeMethods+EFileShare,System.IntPtr,PPJoy.NativeMethods+ECreationDisposition,PPJoy.NativeMethods+EFileAttributes,Microsoft.Win32.SafeHandles.SafeFileHandle)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1404:CallGetLastErrorImmediatelyAfterPInvoke", Scope = "member", Target = "PPJoy.NativeMethods.#DeviceIoControlSynchronous(Microsoft.Win32.SafeHandles.SafeFileHandle,System.UInt32,System.IntPtr,System.UInt32,System.IntPtr,System.UInt32,System.UInt32&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "PPJoy.VirtualJoystick.#GetFileHandle()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Playstation", Scope = "member", Target = "PPJoy.JoystickTypes.#Playstation_Pad_Linux")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Playstation", Scope = "member", Target = "PPJoy.JoystickTypes.#Playstation_Pad_NTPad_XP")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Playstation", Scope = "member", Target = "PPJoy.JoystickTypes.#Playstation_Pad_Megatap")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UnitNumber", Scope = "member", Target = "PPJoy.DeviceManager.#CreateDevice(System.Int32,PPJoy.JoystickTypes,PPJoy.JoystickSubTypes,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LPTNum", Scope = "member", Target = "PPJoy.DeviceManager.#CreateDevice(System.Int32,PPJoy.JoystickTypes,PPJoy.JoystickSubTypes,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ProductIDs", Scope = "member", Target = "PPJoy.DeviceManager.#CreateDevice(System.Int32,PPJoy.JoystickTypes,PPJoy.JoystickSubTypes,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ProductID", Scope = "member", Target = "PPJoy.DeviceManager.#IsVirtualDevice(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PPJoy", Scope = "member", Target = "PPJoy.DeviceManager.#IsVirtualDevice(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "PPJoy.DeviceManager.#GetFileHandle(System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "PPJoy.DeviceManager.#CloseFileHandle(Microsoft.Win32.SafeHandles.SafeFileHandle)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "PPJoy.DeviceManager.#IsSubTypeValidGivenJoystickType(PPJoy.JoystickTypes,PPJoy.JoystickSubTypes)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "4", Scope = "member", Target = "PPJoy.DeviceManager.#SetDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope,PPJoy.MappingCollection)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "PPJoy.DeviceManager.#BuildMapData(PPJoy.MappingCollection)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "PPJoy.DeviceManager.#ReadMapData(PPJoy.JoystickMapPayload)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "PPJoy.DeviceManager.#ReadMapData(PPJoy.JoystickMapPayload)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "PPJoy.DeviceManager.#DeleteDevice(PPJoy.Device,System.Boolean,System.Boolean)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Playstation", Scope = "member", Target = "PPJoy.JoystickTypes.#Playstation_Pad_PSXPBLib")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Playstation", Scope = "member", Target = "PPJoy.JoystickTypes.#Playstation_Pad_DirectPad_Pro")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.StartsWith(System.String)", Scope = "member", Target = "PPJoy.DeviceManager.#GetEnumSubtype(PPJoy.JoystickTypes,System.Byte)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface", Scope = "type", Target = "PPJoy.MappingCollection")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "thisDeviceMappings", Scope = "member", Target = "PPJoy.DeviceManager.#SetDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapScope,PPJoy.MappingCollection)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "joystickType", Scope = "member", Target = "PPJoy.DeviceManager.#SetDeviceMappings(System.Int32,PPJoy.JoystickTypes,System.Int32,PPJoy.JoystickMapHeader)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "PPJoy.DeviceManager.#DeleteDevice(PPJoy.Device,System.Boolean,System.Boolean)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "PPJoy.DeviceManager.#GetAllDevices()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "PPJoy.DeviceManager.#MaxValidUnitNumber(PPJoy.JoystickTypes)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "PPJoy.DeviceManager.#IdealMappings")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "mapping", Scope = "member", Target = "PPJoy.DeviceManager.#BuildMapData(PPJoy.MappingCollection)")]

