// ---------------------------------------------------------------------------
// Campari Software
//
// GlobalSuppressions.cs
//
// Provides assembly level (global) CodeAnalysis suppressions for FxCop.
//
// While static code analysis with FxCop is excellent for catching many common
// and not so common code errors, there are some things that it flags that
// do not always apply to the project at hand. For those cases, FxCop allows
// you to exclude the message (and optionally give a justification reason for
// excluding it). However, those exclusions are stored only in the FxCop
// project file. In the 2.0 version of the .NET framework, Microsoft introduced
// SuppressMessageAttribute, which is used primarily by the version of FxCop
// that is built in to Visual Studio. As this built-in functionality is not
// included in all versions of Visual Studio, we have opted to continue
// using the standalone version of FxCop. 
//
// In order for this version to recognize SupressMessageAttribute, the
// CODE_ANALYSIS symbol must be defined.
//
// ---------------------------------------------------------------------------
// Copyright (C) 2006 Campari Software
// All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
// ---------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

// FxCop says that namespaces should generally have more than five types.
// Unfortunately, not all of these namespaces currently have more than five
// types but we still want the namespace so we can expand the library in the
// future without moving types around. 
#region CA1020:AvoidNamespacesWithFewTypes
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Campari.Software.Collections", Justification = "Ignoring this warning...we want these namespaces, but don't have enough classes to go in them to satisfy the rule.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Campari.Software.Reflection", Justification = "Ignoring this warning...we want these namespaces, but don't have enough classes to go in them to satisfy the rule.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Campari.Software.Remoting", Justification = "Ignoring this warning...we want these namespaces, but don't have enough classes to go in them to satisfy the rule.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Campari.Software.Serialization", Justification = "Ignoring this warning...we want these namespaces, but don't have enough classes to go in them to satisfy the rule.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Campari.Software.IO", Justification = "Ignoring this warning...we want these namespaces, but don't have enough classes to go in them to satisfy the rule.")]
#endregion

// We could use a CustomDictionary.xml file to handle the spelling and case
// rules, but VS2005 Code Analysis doesn't support them and the FxCop add-ins
// and custom external tools don't rely on a project file, so we can't specify
// the location of the CustomDictionary.xml file. We don't want to modify the
// default file that ships with the FxCop distribution either. This does make
// more work for us, but it is the safest solution.
#region CA1703:ResourceStringsShouldBeSpelledCorrectly
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "Campari.Software.Properties.Resources.resources", MessageId = "parsable")]
#endregion

#region CA1705:LongAcronymsShouldBePascalCased
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.IO.ExecutableType.DOS", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.Networking.PlatformId.OSF", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.Networking.PlatformId.VMS", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.Networking.PlatformId.DOS", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.Networking.ServerTypes.DFS", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.Networking.ServerTypes.VMS", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.Networking.ServerTypes.OSF", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.Networking.ServerTypes.DSS", MessageId = "Member")]

[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.InternetInformationServicesComponent.BitsISAPI", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.InternetInformationServicesComponent.NNTP", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.InternetInformationServicesComponent.ASP", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.InternetInformationServicesComponent.SMTP", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.InternetInformationServicesComponent.WWW", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.InternetInformationServicesComponent.FTP", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", Scope = "member", Target = "Campari.Software.InternetInformationServicesComponent.WebDAV", MessageId = "Member")]
#endregion

#region CA1704:IdentifiersShouldBeSpelledCorrectly
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Campari.Software.Diagnostics.SnapshotTasks", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Campari.Software.Diagnostics", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Campari.Software.Text", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "type", Target = "Campari.Software.Text.StringTokenizer", MessageId = "Tokenizer")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Campari.Software.Reflection", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Campari.Software.Collections", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Campari.Software.IO", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Campari.Software.Networking", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Campari.Software.Networking.ServerTypes.MicrosoftFilePrintForNetware", MessageId = "Netware")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "Campari.Software.Networking.ServerTypes.Unix", MessageId = "Unix")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Campari.Software", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "type", Target = "Campari.Software.ExtendedEnvironment", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Campari.Software.Serialization", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "namespace", Target = "Campari.Software.Remoting", MessageId = "Campari")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope="member", Target="Campari.Software.InternetInformationServicesComponent.InetMgr", MessageId="Inet")]
#endregion

