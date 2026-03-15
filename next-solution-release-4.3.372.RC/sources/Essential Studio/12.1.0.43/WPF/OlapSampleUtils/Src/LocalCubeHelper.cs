#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace SampleUtils
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// SQL Server Versions
    /// </summary>
    public enum LocalCubeServerVersion
    {
        /// <summary>
        /// SQL Server Not Installed
        /// </summary>
        Invalid,

        /// <summary>
        /// SQL Server 2005 Installed
        /// </summary>
        SQLServer2005,

        /// <summary>
        /// SQL Server 2008 Installed
        /// </summary>
        SQLServer2008
    }

    /// <summary>
    /// Helps to identify the SQL Server Analysis service installed in the machine and returns
    /// the available version
    /// </summary>
    public class LocalCubeHelper
    {
        /// <summary>
        /// Gets the SQL Server Version for loading the offline cube
        /// </summary>
        /// <returns>
        /// Returns the SQL Server Version Installed in the Machine
        /// </returns>
        public static LocalCubeServerVersion GetAssemblyVersion()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Common Files\System\Ole DB\msmdlocal.dll";
            FileInfo finfo = new FileInfo(filePath);
            if (finfo.Exists)
            {
                FileVersionInfo fversioninfo = FileVersionInfo.GetVersionInfo(filePath);
                string versionNumber = string.Format("{0}", fversioninfo.FileVersion.Split(new string[] { "." }, StringSplitOptions.None));
                if (versionNumber == "2005")
                {
                    if (fversioninfo.FileBuildPart > 1399)
                    {
                        return LocalCubeServerVersion.SQLServer2005;
                    }
                    else
                    {
                        return LocalCubeServerVersion.Invalid;
                    }
                }
                else if (versionNumber == "2008")
                {
                    return LocalCubeServerVersion.SQLServer2008;
                }
            }

            return LocalCubeServerVersion.Invalid;
        }
    }
}