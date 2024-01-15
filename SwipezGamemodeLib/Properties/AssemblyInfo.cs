using System;
using MelonLoader;
using SwipezGamemodeLib; // The namespace of your mod class
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LabFusion.SDK.Modules;
using SwipezGamemodeLib.Module;
using Main = SwipezGamemodeLib.Main;

// ...
[assembly: MelonInfo(typeof(Main), "SwipezGamemodeLib", "1.3.0", "notnotnotswipez")]
[assembly: ModuleInfo(typeof(SwipezLibModule), "SwipezGamemodeLibModule", "1.3.0", "notnotnotswipez", "swlibmodule", true, ConsoleColor.Yellow)]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SwipezGamemodeLib")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("SwipezGamemodeLib")]
[assembly: AssemblyCopyright("Copyright ©  2023")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("08e3c3dc-6940-4e56-ac3e-1861c34c0595")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
