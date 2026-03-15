using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("WpfElements")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Mindscape")]
[assembly: AssemblyProduct("Mindscape WPF Elements")]
[assembly: AssemblyCopyright("Copyright © Mindscape 2008-2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

//[assembly: InternalsVisibleTo("Mindscape.WpfElements.UnitTests, PublicKey=00240000048000009400000006020000002400005253413100040000010001002B1943A3E3EFF73BFDF672D9F5CB60E70BE0E9561D83E92C342310083A83CB4C0DA73D3A6B1FD0670857820F45F8D33CFAA9C94D84CE601F56ABA4764C7CB36A41E63BE8D212916FC9A60F1C4A1E751A30687EBCC886D3D1F0192F6BF7E3CCEDA226737F50768EC9859699494FF7AA57A19E844AD13D5BB2E55462643E7534CB")]
//[assembly: InternalsVisibleTo("wpfertl, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c55701fad18ce0de69b3d6b3ea0453d5977fa8962d1e6b48d439ebd7fa740ac2ffbd356da99ec84bc965de896a856a7c714b2a237e814ec31be7598d2cbeaaecd26534d0a69df44020d22c63ae3ccf0f6988a123950f4f9915154143c5a1dd678ef9aeb86a9f653098525a2f5d509d023b1ea0a79cf306cf603ab8d0d7b0a3b8")]
[assembly: InternalsVisibleTo("Mindscape.WpfElements.UnitTests")]
[assembly: InternalsVisibleTo("wpfertl")]

[assembly: CLSCompliant(true)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.SourceAssembly, //where theme specific resource dictionaries are located
  //(used if a resource is not found in the page, 
  // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
  //(used if a resource is not found in the page, 
  // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsDefinition("http://namespaces.mindscape.co.nz/wpf", "Mindscape.WpfElements")]
[assembly: XmlnsDefinition("http://namespaces.mindscape.co.nz/wpf", "Mindscape.WpfElements.Themes")]
[assembly: XmlnsDefinition("http://namespaces.mindscape.co.nz/wpf", "Mindscape.WpfElements.Charting")]
[assembly: XmlnsDefinition("http://namespaces.mindscape.co.nz/wpf", "Mindscape.WpfElements.PropertyEditing")]
[assembly: XmlnsDefinition("http://namespaces.mindscape.co.nz/wpf", "Mindscape.WpfElements.WpfPropertyGrid")]
[assembly: XmlnsDefinition("http://namespaces.mindscape.co.nz/wpf", "Mindscape.WpfElements.WpfDataGrid")]
[assembly: XmlnsDefinition("http://namespaces.mindscape.co.nz/wpf", "Mindscape.WpfElements.WpfPropertyGrid.Themes")]
[assembly: XmlnsPrefix("http://namespaces.mindscape.co.nz/wpf", "ms")]

// [assembly: AllowPartiallyTrustedCallers]
