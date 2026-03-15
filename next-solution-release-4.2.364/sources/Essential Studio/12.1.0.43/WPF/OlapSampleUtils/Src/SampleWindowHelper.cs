#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace SampleUtils
{
    using System;
    using System.IO;
    using System.Windows;

    /// <summary>
    /// Helper class of the samples startup page
    /// </summary>
    public class SampleWindowHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleWindowHelper"/> class.
        /// </summary>
        public SampleWindowHelper()
        {
        }

        /// <summary>
        /// Gets or sets the waiting dialog.
        /// </summary>
        /// <value>The waiting dialog.</value>
        public WaitingDialog WaitingDialog { get; set; }

        /// <summary>
        /// Closes the waiting dialog.
        /// </summary>
        public void CloseWaitingDialog()
        {
            if (WaitingDialog != null)
            {
                WaitingDialog.Close();
            }
        }
        string FindConfigFile(string fileName)
        {
            // Check both in parent folder and Parent\Data folders.
            string dataFileName = fileName;
            for (int n = 0; n < 12; n++)
            {
                if (System.IO.File.Exists(fileName))
                {
                    return new FileInfo(fileName).FullName;
                }
                if (System.IO.File.Exists(dataFileName))
                {
                    return new FileInfo(dataFileName).FullName;
                }
                fileName = @"..\" + fileName;
                dataFileName = @"..\" + dataFileName;
            }
            return "";
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>Returns the ConnectionString of type string</returns>
        public string GetConnectionString()
        {
            string configFilePath = FindConfigFile("Sample.config");
            if (configFilePath == "")
                configFilePath = SampleSourceHelper.GetSamplePath() + @"bi\Sample.config";
            if (configFilePath != string.Empty)
            {
                if (File.Exists(configFilePath))
                {
                    SampleUtils.SampleSource sampleSource = Syncfusion.Olap.Model.Common.XmlToFromFile(configFilePath, typeof(SampleSource)) as SampleSource;
                    if (sampleSource != null)
                    {
                        if (sampleSource.Source == Source.SyncfusionOfflineCube)
                        {
                            LocalCubeServerVersion localCubeServerVersion = LocalCubeHelper.GetAssemblyVersion();
                            if (localCubeServerVersion == LocalCubeServerVersion.SQLServer2005)
                            {
                                sampleSource.Version = Version.SQL2005;
                            }
                            else if (localCubeServerVersion == LocalCubeServerVersion.SQLServer2008)
                            {
                                sampleSource.Version = Version.SQL2008;
                            }
                            else
                            {
                                MessageBox.Show(@"Pre-built offline cubes installed with this product are not compatible with the version of Microsoft Office installed on your system. Please note that only offline cubes require Microsoft Office. You may connect to a SSAS Server using the “Configure Sample DataSource” utility in the Syncfusion BI Dashboard installed with Essential Studio.", "Loading error");
                                return string.Empty;
                            }
                            //// string samplePath = string.Format("{0}", configFilePath.ToLower().Split(new string[] { "\\bi" }, StringSplitOptions.None));
                            SampleUtils.SampleSourceHelper sampleSourceHelper = new SampleSourceHelper(sampleSource);
                            return sampleSourceHelper.ConnectionString;
                        }
                        else if (sampleSource.Source == Source.CustomServer)
                        {
                            SampleUtils.SampleSourceHelper sampleSourceHelper = new SampleSourceHelper(sampleSource);
                            return sampleSourceHelper.ConnectionString;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error occured while loading the sample configuration file", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Config file not found", "Error");
                }
            }
            else
            {
                MessageBox.Show("Config file path not supplied", "Error");
            }

            return string.Empty;
        }

        /// <summary>
        /// Shows the exception message.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        public void ShowExceptionMessage(Exception exceptionMessage)
        {
            if (exceptionMessage.Message == "Unable to open the connection")
            {
                if (exceptionMessage.InnerException != null)
                {
                    if (exceptionMessage.InnerException.InnerException != null)
                    {
                        if (exceptionMessage.InnerException.InnerException.Message.Contains("local cube file cannot be opened"))
                        {
                            if (this.WaitingDialog != null && this.WaitingDialog.IsActive)
                            {
                                this.CloseWaitingDialog();
                            }

                            MessageBox.Show(@"This offline cube has been locked for use by another application. Please check if any other application/web server instance is currently using this cube.", "Local cube locked");
                            return;
                        }
                    }
                }
            }

            if (this.WaitingDialog != null && this.WaitingDialog.IsActive)
            {
                this.CloseWaitingDialog();
            }

            MessageBox.Show(exceptionMessage.Message, "Error");
        }

        internal string ConnectionStringforTransactionView()
        {
            string configFilePath = FindConfigFile("Sample.config");
            if (configFilePath != "")
                configFilePath = "Data Source="+SampleSourceHelper.GetSamplePath() + @"Common\Data\OfflineCube\Sales DB.cub;Provider=msolap;";
            return configFilePath;
        }
    }
}
