#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Install.Utils;

namespace SampleUtils
{
    /// <summary>
    /// A Helper class for the Sample.
    /// </summary>
    public class SampleSourceHelper
    {
        #region Constants

        private const string ASPCubeName = @"ASP\Adventure_Works_Ext.cub";
        private const string WPFCubeName = @"WPF\Adventure_Works_Ext.cub";
        private const string CubeName = "Adventure_Works_Ext.cub";
        private const string SLCubeName = @"SL\Adventure_Works_Ext.cub";
        private const string LSCubeName = @"LS\Adventure_Works_Ext.cub";
        private const string MVCCubeName = @"MVC\Adventure_Works_Ext.cub";
        private const string OfflineCubeConnectionString = @"Datasource='{0}'; Provider=msolap;";
        private const string OfflineCubeLocation = @"Common\Data\OfflineCube\";
        private const string ServerConnectionString = @"Data Source={0}; Initial Catalog={1};";
        private const string SyncfusionServerAddress = "http://bi.syncfusion.com/olap/msmdpump.dll";
        private const string SyncfusionServerDatabase = "Adventure Works DW 2008 SE";

        #endregion

        #region Initialize/Finalize

        /// <summary>
        ///     Initializes an object for <see cref="SampleSourceHelper" /> class.
        /// </summary>
        /// <param name="source">
        ///     An object of <see cref="SampleSource" />.
        /// </param>
        public SampleSourceHelper(SampleSource source)
        {
            SampleSource = source;
            SamplePath = GetSamplePath();
        }

        /// <summary>
        ///     Initializes an object for <see cref="SampleSourceHelper" /> class.
        /// </summary>
        /// <param name="source">
        ///     An object of <see cref="SampleSource" />.
        /// </param>
        /// <param name="samplePath">A string contains sample path.</param>
        public SampleSourceHelper(SampleSource source, string samplePath)
        {
            SampleSource = source;
            SamplePath = samplePath;
        }

        /// <summary>
        ///     Initializes an object for <see cref="SampleSourceHelper" /> class.
        /// </summary>
        /// <param name="source">
        ///     An object of <see cref="SampleSource" />.
        /// </param>
        /// <param name="samplePath">A string contains sample path.</param>
        /// <param name="platform">A platform whether its ASP or WPF or Silverlight or MVC.</param>
        public SampleSourceHelper(SampleSource source, string samplePath, Platform platform)
        {
            SampleSource = source;
            SamplePath = samplePath;
            Platform = platform;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        public string ConnectionString
        {
            get { return GetConnectionString(Platform); }
        }

        /// <summary>
        ///     Gets the connection string for Silverlight platform.
        /// </summary>
        public string SilverlightConnectionString
        {
            get { return GetConnectionString(Platform.SL); }
        }

        /// <summary>
        ///     Gets the connection string for LightSwitch platform.
        /// </summary>
        public string LSConnectionString
        {
            get { return GetConnectionString(Platform.LS); }
        }

        /// <summary>
        ///     Gets the connection string for MVC platform.
        /// </summary>
        public string MVCConnectionString
        {
            get { return GetConnectionString(Platform.MVC); }
        }

        /// <summary>
        ///     Gets or sets the sample path.
        /// </summary>
        public string SamplePath { get; set; }

        /// <summary>
        ///     Gets or sets <see cref="SampleSource" /> object.
        /// </summary>
        public SampleSource SampleSource { get; set; }

        /// <summary>
        ///     Gets or sets the platform.
        /// </summary>
        public Platform Platform { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets sample path.
        /// </summary>
        /// <returns>A string value contains sample installation path.</returns>
        public static string GetSamplePath()
        {
            string samplePath = EssentialStudio.CurrentVersion.SamplesPath;
            return samplePath;
        }

        //public static string GetSamplePath(Platform platform)
        //{
        //    string __samplePath = EssentialStudio.Samples(EssentialStudio.Platforms.ES);
        //    if (platform == Platform.ASP)
        //        __samplePath = EssentialStudio.Samples(EssentialStudio.Platforms.ASPNET);
        //    else if (platform == Platform.MVC)
        //        __samplePath = EssentialStudio.Samples(EssentialStudio.Platforms.MVC);
        //    else if (platform == Platform.SL)
        //        __samplePath = EssentialStudio.Samples(EssentialStudio.Platforms.Silverlight);
        //    else if (platform == Platform.WPF)
        //        __samplePath = EssentialStudio.Samples(EssentialStudio.Platforms.WPF);
        //    return __samplePath;
        //}

        /// <summary>
        ///     Gets installed version path.
        /// </summary>
        /// <returns>A string contains the the installation path.</returns>
        public static string GetInstalledVersionPath()
        {
            string installedPath = EssentialStudio.CurrentVersion.StudioPath;
            installedPath = installedPath.Remove(installedPath.Length - 1);
            installedPath = installedPath.Substring(0, installedPath.LastIndexOf('\\'));
            return installedPath;
        }

        //public static string GetInstalledVersionPath(Platform platform)
        //{
        //    string __installedPath = EssentialStudio.Samples(EssentialStudio.Platforms.ES);
        //    if (platform == Platform.ASP)
        //        __installedPath = EssentialStudio.Samples(EssentialStudio.Platforms.ASPNET);
        //    else if (platform == Platform.MVC)
        //        __installedPath = EssentialStudio.Samples(EssentialStudio.Platforms.MVC);
        //    else if (platform == Platform.SL)
        //        __installedPath = EssentialStudio.Samples(EssentialStudio.Platforms.Silverlight);
        //    else if (platform == Platform.WPF)
        //        __installedPath = EssentialStudio.Samples(EssentialStudio.Platforms.WPF);
        //    __installedPath = __installedPath.Remove(__installedPath.Length - 1);
        //    __installedPath = __installedPath.Substring(0, __installedPath.LastIndexOf('\\'));
        //    return __installedPath;
        //}

        /// <summary>
        ///     Gets assembly path.
        /// </summary>
        /// <returns>A string contains the assembly path.</returns>
        public static string GetAssemblyPath()
        {
            string installedVersionPath = GetInstalledVersionPath();
            if (!string.IsNullOrEmpty(installedVersionPath))
            {
                return installedVersionPath + @"Assemblies";
            }

            return string.Empty;
        }

        //public static string GetAssemblyPath(Platform platform)
        //{
        //    string installedVersionPath = GetInstalledVersionPath(platform);
        //    if (installedVersionPath != null && installedVersionPath != string.Empty)
        //    {
        //        return installedVersionPath + @"Assemblies";
        //    }

        //    return string.Empty;
        //}

        /// <summary>
        ///     Gets basic template path.
        /// </summary>
        /// <returns>A string contains the template path.</returns>
        public static string GetBasicTemplatePath()
        {
            string samplesPath = GetSamplePath();
            if (!string.IsNullOrEmpty(samplesPath))
            {
                return samplesPath + @"Common\Data\CubeModelTemplates\BasicTemplate.xml";
            }

            return string.Empty;
        }

        //public static string GetBasicTemplatePath(Platform platform)
        //{
        //    string samplesPath = GetSamplePath(platform);
        //    if (samplesPath != null && samplesPath != string.Empty)
        //    {
        //        return samplesPath + @"Common\Data\CubeModelTemplates\BasicTemplate.xml";
        //    }

        //    return string.Empty;
        //}

        /// <summary>
        ///     Gets pre-compiled assembly location.
        /// </summary>
        /// <returns>A string contains the pre-compiled assembly location.</returns>
        public static string GetPreCompiledAssemblyPath()
        {
            string installedVersionPath = GetInstalledVersionPath();
            if (!string.IsNullOrEmpty(installedVersionPath))
            {
                return installedVersionPath + @"precompiledassemblies\" + EssentialStudio.CurrentVersion.VersionNumber;
            }

            return string.Empty;
        }

        //public static string GetPreCompiledAssemblyPath(Platform platform)
        //{
        //    string installedVersionPath = GetInstalledVersionPath(platform);
        //    if (installedVersionPath != null && installedVersionPath != string.Empty)
        //    {
        //        return installedVersionPath + @"precompiledassemblies\" + EssentialStudio.CurrentVersion.VersionNumber;
        //    }

        //    return string.Empty;
        //}

        /// <summary>
        ///     Gets offline cube path.
        /// </summary>
        /// <returns>A string contains the offline cube path.</returns>
        public static string GetOfflineCubePath()
        {
            return GetSamplePath() + OfflineCubeLocation + WPFCubeName;
        }

        //public static string GetOfflineCubePath(Platform platform)
        //{
        //    string cubeName = WPFCubeName;
        //    if (platform == Platform.ASP)
        //        cubeName = ASPCubeName;
        //    else if (platform == Platform.WPF)
        //        cubeName = WPFCubeName;
        //    else if (platform == Platform.SL)
        //        cubeName = SLCubeName;
        //    else if (platform == Platform.MVC)
        //        cubeName = MVCCubeName;
        //    return SampleSourceHelper.GetSamplePath(platform) + OfflineCubeLocation + cubeName;
        //}

        /// <summary>
        ///     Gets offline cube location.
        /// </summary>
        /// <returns>A string contains the offline cube location.</returns>
        public static string GetOfflineCubeLocation()
        {
            return GetSamplePath() + OfflineCubeLocation;
        }

        private string GetConnectionString(Platform platform)
        {
            if (SampleSource.Source == Source.SyncfusionServer)
            {
                return string.Format(ServerConnectionString, SyncfusionServerAddress, SyncfusionServerDatabase);
            }
            if (SampleSource.Source == Source.SyncfusionOfflineCube)
            {
                if (SamplePath == string.Empty)
                {
                    throw new Exception("Please specify the file path");
                }
                if (platform == Platform.WPF)
                    return string.Format(OfflineCubeConnectionString, SamplePath + OfflineCubeLocation + WPFCubeName);
                if (platform == Platform.SL)
                    return string.Format(OfflineCubeConnectionString, SamplePath + OfflineCubeLocation + SLCubeName);
                if (platform == Platform.LS)
                    return string.Format(OfflineCubeConnectionString, SamplePath + OfflineCubeLocation + LSCubeName);
                if (platform == Platform.ASP)
                    return string.Format(OfflineCubeConnectionString, SamplePath + OfflineCubeLocation + ASPCubeName);
                if (platform == Platform.MVC)
                    return string.Format(OfflineCubeConnectionString, SamplePath + OfflineCubeLocation + MVCCubeName);
                return string.Format(OfflineCubeConnectionString, SamplePath + OfflineCubeLocation + CubeName);
            }
            if (SampleSource.Source == Source.CustomOfflineCube)
            {
                if (SampleSource.FilePath == string.Empty)
                {
                    throw new Exception("Please specify the file path");
                }
                return string.Format(OfflineCubeConnectionString, SampleSource.FilePath);
            }
            if (SampleSource.Source == Source.CustomServer)
            {
                if (SampleSource.ServerName == string.Empty)
                {
                    throw new Exception("Please specify the server name");
                }

                if (SampleSource.DatabaseName == string.Empty)
                {
                    throw new Exception("Please specify the Database name");
                }

                return string.Format(ServerConnectionString, SampleSource.ServerName, SampleSource.DatabaseName);
            }

            return string.Empty;
        }

        #endregion
    }
}