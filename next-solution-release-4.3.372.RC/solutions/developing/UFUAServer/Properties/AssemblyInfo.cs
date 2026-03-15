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
#if !CONNEXT
[assembly: Guid("7cfb8b5b-41a8-4366-a73b-e89b24479a5e")]
#else
[assembly: Guid("36CE1890-94EC-4430-9198-DD79D3113134")]
#endif

// Default namesapce of the application 
[assembly: Utilities.DefaultNamespaceAttribute("UFUAServerApp")]
