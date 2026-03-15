#region Copyright

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 

#endregion

using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Olap.Common;

namespace SampleUtils
{
    /// <summary>
    ///     Sample window.
    /// </summary>
    public class SampleWindow : Window
    {
        #region Initilize

        /// <summary>
        ///     Initializes the object <see cref="SampleWindow" />.
        /// </summary>
        public SampleWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the <see cref="WaitingDialog" /> object.
        /// </summary>
        public WaitingDialog WaitingDialog { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Shows the waiting dialog.
        /// </summary>
        public void ShowWaitingDialog()
        {
            WaitingDialog = new WaitingDialog {Owner = this};
            WaitingDialog.Display();
        }

        /// <summary>
        ///     Closes the waiting dialog.
        /// </summary>
        public void CloseWaitingDialog()
        {
            if (WaitingDialog != null)
            {
                WaitingDialog.Close();
            }
        }

        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        /// <returns>A connection string.</returns>
        public string GetConnectionString()
        {
            string configFilePath = FindConfigFile("OLAPSample.config");
            if (configFilePath == "")
                configFilePath = SampleSourceHelper.GetSamplePath() + @"Config\OLAPSample.config";
            string cubeFile = configFilePath.Substring(0,
                                                       configFilePath.ToLower()
                                                                     .LastIndexOf(@"\config\", StringComparison.Ordinal) +
                                                       1);
            if (configFilePath != string.Empty)
            {
                if (File.Exists(configFilePath))
                {
                    SampleUtils.SampleSource sampleSource = Common.XmlToFromFile(configFilePath, typeof (SampleSource)) as SampleSource;
                    if (sampleSource != null)
                    {
                        if (sampleSource.Source == Source.SyncfusionOfflineCube)
                        {
                            return new SampleSourceHelper(sampleSource, cubeFile, Platform.WPF).ConnectionString;
                        }
                        if (sampleSource.Source == Source.CustomServer)
                        {
                            return new SampleSourceHelper(sampleSource).ConnectionString;
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
        ///     Gets the connection string for a transactional view demo sample.
        /// </summary>
        /// <returns>A connection string.</returns>
        public string ConnectionStringforTransactionView()
        {
            string configFilePath = FindConfigFile("OLAPSample.config");
            if (configFilePath == "")
                configFilePath = SampleSourceHelper.GetSamplePath() + @"Config\OLAPSample.config";
            if (File.Exists(configFilePath))
            {
                string cubeFile =
                    configFilePath.Substring(0, configFilePath.ToLower().IndexOf("\\config", StringComparison.Ordinal)) +
                    "\\";
                return "Data Source=" + cubeFile + @"Common\Data\OfflineCube\Sales DB.cub;Provider=msolap;";
            }

            MessageBox.Show("Config file not found", "Error");

            return string.Empty;
        }

        /// <summary>
        ///     Shows the exception message.
        /// </summary>
        /// <param name="exceptionMessage">Exception object.</param>
        public void ShowExceptionMessage(Exception exceptionMessage)
        {
            MessageBox.Show(exceptionMessage.Message, "Error");
        }

        private string FindConfigFile(string fileName)
        {
            // Check both in parent folder and Parent\Data folders.
            string dataFileName = fileName;
            for (int n = 0; n < 12; n++)
            {
                if (File.Exists(fileName))
                {
                    return new FileInfo(fileName).FullName;
                }
                if (File.Exists(dataFileName))
                {
                    return new FileInfo(dataFileName).FullName;
                }
                fileName = @"..\" + fileName;
                dataFileName = @"..\" + dataFileName;
            }
            return "";
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name.Equals("VisualStyle"))
            {
                if (e.NewValue.Equals("Default"))
                {
                    Background = Brushes.WhiteSmoke;
                    Foreground = Brushes.Black;
                }
                else if (e.NewValue.Equals("Blend"))
                {
                    Background = (Brush) new BrushConverter().ConvertFromInvariantString("#FF333333");
                    Foreground = (Brush) new BrushConverter().ConvertFromInvariantString("#FFFFFFFF");
                }
                else if (e.NewValue.Equals("Office2007Blue"))
                {
                    Background = (Brush) new BrushConverter().ConvertFromInvariantString("#FFEAF2FB");
                    Foreground = Brushes.Black;
                }
                else if (e.NewValue.Equals("Office2007Silver"))
                {
                    Background = (Brush) new BrushConverter().ConvertFromInvariantString("#FFE8EAEC");
                    Foreground = Brushes.Black;
                }
                else if (e.NewValue.Equals("Office2007Black"))
                {
                    Background = (Brush) new BrushConverter().ConvertFromInvariantString("#FFCED3DA");
                    Foreground = Brushes.Black;
                }
                else if (e.NewValue.Equals("Office2003"))
                {
                    Background = (Brush) new BrushConverter().ConvertFromInvariantString("#FFEAF2FB");
                    Foreground = Brushes.Black;
                }
            }
        }

        #endregion
    }
}