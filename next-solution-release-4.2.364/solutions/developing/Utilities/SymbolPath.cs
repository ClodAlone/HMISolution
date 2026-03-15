using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

namespace Utilities
{
    public static class SymbolLibraryPath
    {
        public readonly static string ProjectSymbolTag = Properties.Settings.Default.ProjectSymbolTag;
        public readonly static string PatternXaml = Properties.Settings.Default.PatternXaml;
        public readonly static string RootSymbolFolder = Properties.Settings.Default.RootSymbolFolder;
        public readonly static string RootStyleFolder = Properties.Settings.Default.RootStyleFolder;
    }
}
