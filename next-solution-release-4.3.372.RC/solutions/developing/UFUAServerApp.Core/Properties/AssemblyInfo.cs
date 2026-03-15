using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if !CONNEXT
[assembly: AssemblyTitle("Movicon.NExT I/O Data Server")]
#else
[assembly: AssemblyTitle("Connext I/O Data Server")]
#endif
[assembly: AssemblyCulture("")]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4C6D183F-B783-4606-9F56-7DCAB707AD81")]

// Default namesapce of the application 
[assembly: Utilities.DefaultNamespaceAttribute("UFUAServerApp.Core")]
