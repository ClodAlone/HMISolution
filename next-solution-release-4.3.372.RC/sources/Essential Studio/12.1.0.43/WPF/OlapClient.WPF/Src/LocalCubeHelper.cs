#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Client.Olap
{
    using System;
    using System.Diagnostics;
    using System.IO;

    #region Enumerated Data
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
        SQLServer2008,

        /// <summary>
        /// SQL Server 2008 R2 Installed
        /// </summary>
        SQLServer2008R2
    }
    #endregion

    #region LocalCubeHelper Definition
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
            bool fileExist = false;
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Common Files\System\Ole DB\msmdlocal.dll";
            FileInfo finfo = new FileInfo(filePath);
            if (finfo.Exists)
            {
                fileExist = true;
            }
            else
            {
                filePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Microsoft Analysis Services\AS OLEDB\90\msmdlocal.dll";
                finfo = new FileInfo(filePath);
                if (finfo.Exists)
                {
                    fileExist = true;
                }
                else
                {
                    filePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Microsoft Analysis Services\AS OLEDB\10\msmdlocal.dll";
                    finfo = new FileInfo(filePath);
                    if (finfo.Exists)
                    {
                        fileExist = true;
                    }
                }
            }

            if (fileExist)
            {
                FileVersionInfo fversioninfo = FileVersionInfo.GetVersionInfo(filePath);
                string versionNumber = string.Format("{0}", fversioninfo.FileVersion.Split(new[] { "." }, StringSplitOptions.None));
                if (versionNumber == "2005")
                {
                    if (fversioninfo.FileBuildPart > 1399)
                    {
                        return LocalCubeServerVersion.SQLServer2005;
                    }
                   
                    return LocalCubeServerVersion.Invalid;
                }

                if (versionNumber == "2008")
                {
                    return LocalCubeServerVersion.SQLServer2008;
                }

                if (versionNumber == "2009")
                {
                    return LocalCubeServerVersion.SQLServer2008R2;
                }
            }

            return LocalCubeServerVersion.Invalid;
        }
    }
    #endregion
}