#region Using directives

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using TickZoom.Api;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ExamplesPluginTests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ExamplesPluginTests")]
[assembly: AssemblyCopyright("Copyright 2009")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
#if POSTSHARPX
[assembly: Diagram( AttributePriority=1, AttributeTargetTypes = "TickZoom.TickUtil.*")] 
#endif
// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]

// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion("0.5.22.67")]
