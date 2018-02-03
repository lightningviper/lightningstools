// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project. 
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc. 
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File". 
// You do not need to add suppressions to this file manually. 

using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Scope = "type",
        Target = "MFDExtractor.ImageRemoting.Server")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.Quantizer+Color32.#Red")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.Quantizer+Color32.#Green")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.Quantizer+Color32.#Blue")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.Quantizer+Color32.#ARGB")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.Quantizer+Color32.#Alpha")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Scope = "type",
        Target = "MFDExtractor.ImageRemoting.Quantizer+Color32")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type",
        Target = "MFDExtractor.ImageRemoting.Quantizer+Color32")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Quantizer",
        Scope = "type", Target = "MFDExtractor.ImageRemoting.Quantizer")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors", Scope = "type",
        Target = "MFDExtractor.ImageRemoting.Quantizer")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.OctreeQuantizer+Octree+OctreeNode.#NextReducible")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Scope = "member",
        Target =
            "MFDExtractor.ImageRemoting.OctreeQuantizer+Octree+OctreeNode.#GetPaletteIndex(MFDExtractor.ImageRemoting.Quantizer+Color32*,System.Int32)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.OctreeQuantizer+Octree+OctreeNode.#Children")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Scope = "member",
        Target =
            "MFDExtractor.ImageRemoting.OctreeQuantizer+Octree+OctreeNode.#.ctor(System.Int32,System.Int32,MFDExtractor.ImageRemoting.OctreeQuantizer+Octree)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.OctreeQuantizer+Octree.#.ctor(System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Quantizer",
        Scope = "type", Target = "MFDExtractor.ImageRemoting.OctreeQuantizer")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Octree",
        Scope = "type", Target = "MFDExtractor.ImageRemoting.OctreeQuantizer")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.IExtractorBitmapClient.#GetHudBitmap()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.IExtractorBitmapClient.#GetLeftMfdBitmap()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.IExtractorBitmapClient.#GetRightMfdBitmap()")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults",
        MessageId = "MFDExtractor.NativeMethods.SendMessage(System.IntPtr,System.Int32,System.Int32,System.Int32)",
        Scope = "member",
        Target =
            "MFDExtractor.ResizeHelper.#System.Windows.Forms.IMessageFilter.PreFilterMessage(System.Windows.Forms.Message&)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "MFDExtractor.ResizeHelper.#TheWindow")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "k", Scope = "member",
        Target = "MFDExtractor.ShortcutInput.#CharCodeFromKeys(System.Windows.Forms.Keys)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member",
        Target = "MFDExtractor.ShortcutInput.#MinModifiers")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "k", Scope = "member",
        Target = "MFDExtractor.ShortcutInput.#Win32ModifiersFromKeys(System.Windows.Forms.Keys)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member",
        Target = "MFDExtractor.Program.#PriorProcess()")]
[assembly:
    SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return",
        Scope = "member", Target = "MFDExtractor.NativeMethods.#ReleaseDC(System.IntPtr,System.IntPtr)")]
[assembly:
    SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "2",
        Scope = "member",
        Target = "MFDExtractor.NativeMethods.#SendMessage(System.IntPtr,System.Int32,System.Int32,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "3",
        Scope = "member",
        Target = "MFDExtractor.NativeMethods.#SendMessage(System.IntPtr,System.Int32,System.Int32,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return",
        Scope = "member",
        Target = "MFDExtractor.NativeMethods.#SendMessage(System.IntPtr,System.Int32,System.Int32,System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member",
        Target = "MFDExtractor.Program.#Main()")]
[assembly:
    SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.Client.#.ctor(System.Net.IPEndPoint,System.String)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#DoWork()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#Get3DHud()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#Get3DLeftMFD()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#Get3DRightMFD()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#HudImageUpdaterThreadWork()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#LeftMfdImageUpdaterThreadWork()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#ReadHudFromNetwork()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#ReadLeftMfdFromNetwork()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#ReadRightMfdFromNetwork()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#RightMfdImageUpdaterThreadWork()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#SetupImageClient()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#SetupImageServer()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#Stop()")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mfd",
        Scope = "member", Target = "MFDExtractor.ImageRemoting.IExtractorBitmapClient.#GetLeftMfdBitmap()")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mfd",
        Scope = "member", Target = "MFDExtractor.ImageRemoting.IExtractorBitmapClient.#GetRightMfdBitmap()")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mfd",
        Scope = "member", Target = "MFDExtractor.ImageRemoting.IExtractorBitmapServer.#GetLeftMfdBitmapBytes()")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mfd",
        Scope = "member", Target = "MFDExtractor.ImageRemoting.IExtractorBitmapServer.#GetRightMfdBitmapBytes()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.Server.#SetHudBitmap(System.Drawing.Bitmap)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.Server.#SetLeftMfdBitmap(System.Drawing.Bitmap)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.Server.#SetRightMfdBitmap(System.Drawing.Bitmap)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.ImageRemoting.Server.#TearDownService(System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member",
        Target = "MFDExtractor.InstrumentForm.#_resizeHelper")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.frmOptions.#SaveSettings(System.Boolean)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.frmOptions.#cmdCancel_Click(System.Object,System.EventArgs)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.frmOptions.#cmdOk_Click(System.Object,System.EventArgs)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.Extractor.#TearDownImageServer()")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes",
        Scope = "member", Target = "MFDExtractor.IPAddressControl.#SetAddressBytes(System.Byte[])")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.frmMain.#ReadKeyboard()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.frmMain.#KeyboardWatcherThread()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "MFDExtractor.frmMain.#InitializeKeyboard()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member",
        Target = "MFDExtractor.Extractor.#GetInstance()")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Draggable",
        Scope = "member", Target = "MFDExtractor.DraggableForm.#Draggable")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Draggable",
        Scope = "type", Target = "MFDExtractor.DraggableForm")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "frm", Scope = "type",
        Target = "MFDExtractor.frmMain")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "frm", Scope = "type",
        Target = "MFDExtractor.frmOptions")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "frm", Scope = "type",
        Target = "MFDExtractor.frmMain")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "frm", Scope = "type",
        Target = "MFDExtractor.frmOptions")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MFD")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MFD",
        Scope = "namespace", Target = "MFDExtractor")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MFD",
        Scope = "namespace", Target = "MFDExtractor.ImageRemoting")]