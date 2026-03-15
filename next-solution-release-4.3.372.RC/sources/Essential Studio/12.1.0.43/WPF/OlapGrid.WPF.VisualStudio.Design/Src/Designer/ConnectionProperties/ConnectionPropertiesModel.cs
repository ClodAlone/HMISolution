#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Shared.Olap;

namespace Syncfusion.OlapGrid.WPF.VisualStudio.Design
{
    /// <summary>
    /// Model for creating and editing the connection properties.
    /// </summary>
    public class ConnectionPropertiesModel
        : DependencyObject,
        INotifyPropertyChanged, 
        IWizardNavigationButtonsStatus
    {
        #region Memebers

        /// <summary>
        /// Validates the designer settings.
        /// </summary>
        private bool? isDesignerSettingsValid = null;

        /// <summary>
        /// Toggles the visibility of the offline cube information grid.
        /// </summary>
        private bool canOfflineCubeGridEnabled;

        /// <summary>
        /// Toggles the visibility of the custom server information grid.
        /// </summary>
        private bool canCustomServerGridEnabled;

        /// <summary>
        /// Toggles the visibility of the raw connection string text box.
        /// </summary>
        private bool canRawStringEnabled;

        /// <summary>
        /// Toggles the visiblity of the back button.
        /// </summary>
        private bool canBackButtonEnabled;

        /// <summary>
        /// Toggles the visiblity of the next button.
        /// </summary>
        private bool canNextButtonEnabled;
        
        /// <summary>
        /// Toggles the visiblity of the finish button.
        /// </summary>
        private bool canFinishButtonEnabled;

        /// <summary>
        /// Represents the local file path of an offline cube.
        /// </summary>
        private string filePath;

        /// <summary>
        /// Server name.
        /// </summary>
        private string serverName;

        /// <summary>
        /// Data base name.
        /// </summary>
        private string databaseName;

        /// <summary>
        /// User name for accessing the data base.
        /// </summary>
        private string userName;

        /// <summary>
        /// Password for accessing the data base.
        /// </summary>
        private string password;

        /// <summary>
        /// User defined connection string.
        /// </summary>
        private string rawConnectionString;

        /// <summary>
        /// Instance of the selected grid.
        /// </summary>
        private ModelItem selectedItem;

        /// <summary>
        /// Instance of the wizard window.
        /// </summary>
        private WizardWindow winzardWindowInstance;

        /// <summary>
        /// Type of the connection mode.
        /// </summary>
        private ConnectionMode connectionMode;

        /// <summary>
        /// Checks whether the transaction altered the any data.
        /// </summary>
        private bool isDirty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPropertiesModel"/> class.
        /// </summary>
        /// <param name="selectedItem">The selected item.</param>
        /// <param name="winzardWindow">The winzard window.</param>
        public ConnectionPropertiesModel(ModelItem selectedItem, WizardWindow winzardWindow)
        {
            this.canBackButtonEnabled = false;
            this.canNextButtonEnabled = false;
            this.canFinishButtonEnabled = false;
            this.canOfflineCubeGridEnabled = false;
            this.canCustomServerGridEnabled = false;
            this.canRawStringEnabled = false;

            this.selectedItem = selectedItem; 
            this.winzardWindowInstance = winzardWindow;

            var v = this.selectedItem.Properties["DesignerSettings"].Value;

            if (v != null)
            {
                this.ConnectionMode = (ConnectionMode)v.Properties["ConnectionMode"].ComputedValue;
                string conStr = (string)v.Properties["ConnectionString"].ComputedValue;
                this.isDesignerSettingsValid = false;

                if (this.ConnectionMode == ConnectionMode.OfflineCube)
                {
                    this.canCustomServerGridEnabled = false;
                    this.canRawStringEnabled = false;
                    this.canOfflineCubeGridEnabled = true;
                    
                    //// Parsing the fields
                    this.filePath = conStr.Substring(conStr.IndexOf('=') + 1).Split(';')[0];

                    if (!string.IsNullOrEmpty(this.filePath))
                    {
                        this.isDesignerSettingsValid = true;
                    }
                    else
                    {
                        this.isDesignerSettingsValid = false;
                    }

                    this.ConfigureWizardForOfflineCubeSetting();
                }
                else if(this.ConnectionMode == ConnectionMode.CustomServer)
                {
                    this.canOfflineCubeGridEnabled = false;
                    this.canRawStringEnabled = false;
                    this.canCustomServerGridEnabled = true;

                    this.isDesignerSettingsValid = false;

                    //// Parsing of server connection string.
                    string tempConStr = conStr.Substring(conStr.IndexOf('=') + 1);
                    this.serverName = tempConStr.Split(';')[0];
                    this.databaseName = tempConStr.Substring(tempConStr.IndexOf('=') + 1).Split(';')[0];
                    this.userName = conStr.Substring(conStr.IndexOf('=') + 1).Split(';')[2].Split('=').Last();
                    this.password = conStr.Substring(conStr.IndexOf('=') + 1).Split(';')[3].Split('=').Last();

                    if (!string.IsNullOrEmpty(this.serverName) && !string.IsNullOrEmpty(this.databaseName))
                    {
                        this.isDesignerSettingsValid = true;
                    }

                    this.ConfigureWizardForServerSettings();
                }
                else if (this.ConnectionMode == ConnectionMode.RawString)
                {
                    this.canOfflineCubeGridEnabled = false;
                    this.canCustomServerGridEnabled = false;
                    this.canRawStringEnabled = true;

                    this.isDesignerSettingsValid = false;

                    //// Parsing of server connection string.
                    this.rawConnectionString = conStr;

                    if (!string.IsNullOrEmpty(this.rawConnectionString))
                    {
                        this.isDesignerSettingsValid = true;
                    }

                    this.ConfigureWizardForServerSettings();
                }
            }
            else
            {
                this.canOfflineCubeGridEnabled = false;
                this.canCustomServerGridEnabled = false;
                this.canRawStringEnabled = false;
                this.isDesignerSettingsValid = null;
                this.UpdateButtonStatus(false, false, false);
            }

            this.BrowseCommand = new DelegateCommand<object>((Action) (() =>
                {
                    this.Browse();
                }));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the connection mode.
        /// </summary>
        /// <value>The connection mode.</value>
        public ConnectionMode ConnectionMode
        {
            get
            {
                return this.connectionMode;
            }
            set
            {
                this.connectionMode = value;
                this.RaisePropertyChanged("ConnectionMode");

                if (value == ConnectionMode.OfflineCube)
                {
                    this.CanCustomServerGridEnabled = false;
                    this.CanRawStringEnabled = false;
                    this.CanOfflineCubeGridEnabled = true;
                    this.ConfigureWizardForOfflineCubeSetting();
                }
                else if(value == ConnectionMode.CustomServer)
                {
                    this.CanOfflineCubeGridEnabled = false;
                    this.CanRawStringEnabled = false;
                    this.CanCustomServerGridEnabled = true;
                    this.ConfigureWizardForServerSettings();
                }
                else if(value == ConnectionMode.RawString)
                {
                    this.CanOfflineCubeGridEnabled = false;
                    this.CanCustomServerGridEnabled = false;
                    this.CanRawStringEnabled = true;
                    this.ConfigureWizardForServerSettings();
                }
            }
        }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                this.IsDirty = true;
                this.filePath = value;
                this.ConfigureWizardForOfflineCubeSetting();
                this.RaisePropertyChanged("FilePath");
            }
        }        

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>The name of the server.</value>
        public string ServerName
        {
            get
            {
                return this.serverName;
            }
            set
            {
                this.IsDirty = true;
                this.serverName = value;
                this.ConfigureWizardForServerSettings();
            }
        }        

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName
        {
            get
            {
                return this.databaseName;
            }
            set
            {
                this.IsDirty = true;
                this.databaseName = value;
                this.ConfigureWizardForServerSettings();
            }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        /// <summary>
        /// Gets or sets the raw connection string.
        /// </summary>
        /// <value>The raw connection string.</value>
        public string RawConnectionString
        {
            get
            {
                return this.rawConnectionString;
            }
            set
            {
                this.rawConnectionString = value;
                this.ConfigureWizardForServerSettings();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can offline cube grid enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can offline cube grid enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CanOfflineCubeGridEnabled
        {
            get
            {
                return this.canOfflineCubeGridEnabled;
            }
            set
            {
                this.canOfflineCubeGridEnabled = value;
                this.RaisePropertyChanged("CanOfflineCubeGridEnabled");
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance can custom server grid enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can custom server grid enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CanCustomServerGridEnabled
        {
            get
            {
                return this.canCustomServerGridEnabled;
            }
            set
            {
                this.canCustomServerGridEnabled = value;
                this.RaisePropertyChanged("CanCustomServerGridEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can raw string enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can raw string enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CanRawStringEnabled
        {
            get
            {
                return this.canRawStringEnabled;
            }
            set
            {
                this.canRawStringEnabled = value;
                this.RaisePropertyChanged("CanRawStringEnabled");
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance can back button enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can back button enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CanBackButtonEnabled
        {
            get
            {
                return this.canBackButtonEnabled;
            }
            set
            {
                this.canBackButtonEnabled = value;
                this.winzardWindowInstance.buttonBack.IsEnabled = value;
                this.RaisePropertyChanged("CanBackButtonEnabled");
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance can next button enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can next button enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CanNextButtonEnabled
        {
            get
            {
                return this.canNextButtonEnabled;
            }
            set
            {
                this.canNextButtonEnabled = value;
                this.winzardWindowInstance.buttonNext.IsEnabled = value;
                this.winzardWindowInstance.buttonTestConnection.IsEnabled = value;
                this.RaisePropertyChanged("CanNextButtonEnabled");
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance can finish button enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can finish button enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CanFinishButtonEnabled
        {
            get
            {
                return this.canFinishButtonEnabled;
            }
            set
            {
                this.canFinishButtonEnabled = value;
                this.winzardWindowInstance.buttonFinish.IsEnabled = value;
                this.RaisePropertyChanged("CanFinishButtonEnabled");
            }
        }

        /// <summary>
        /// Gets or sets the browse command.
        /// </summary>
        /// <value>The browse command.</value>
        public DelegateCommand<object> BrowseCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }
            set
            {
                this.isDirty = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public void CommitChanges()
        {
            if (this.isDesignerSettingsValid == null || this.isDesignerSettingsValid == true)
            {
                if (isDesignerSettingsValid == null)
                {
                    this.selectedItem.Properties["DesignerSettings"].SetValue(new DesignerSettings());
                }

                this.selectedItem.Properties["DesignerSettings"].Value.Properties["ConnectionMode"].SetValue(this.connectionMode);

                if (this.ConnectionMode == ConnectionMode.OfflineCube)
                {
                    this.selectedItem.Properties["DesignerSettings"].Value.Properties["ConnectionString"].SetValue("Data Source=" + this.filePath + "; Provider=msolap;");
                }
                else if(this.ConnectionMode == ConnectionMode.CustomServer)
                {
                    if (!string.IsNullOrEmpty(this.userName))
                    {
                        this.selectedItem.Properties["DesignerSettings"].Value.Properties["ConnectionString"].SetValue("Data Source=" + this.serverName + "; Initial Catalog=" + this.databaseName + ";User Id=" + this.userName + ";Password=" + this.password + ";");
                    } 
                    else if (!string.IsNullOrEmpty(this.serverName) && !string.IsNullOrEmpty(this.databaseName))
                    {
                        this.selectedItem.Properties["DesignerSettings"].Value.Properties["ConnectionString"].SetValue("Data Source=" + this.serverName + "; Initial Catalog=" + this.databaseName + ";");
                    }
                }
                else if (this.ConnectionMode == ConnectionMode.RawString)
                {
                    if(!string.IsNullOrEmpty(this.rawConnectionString))
                    {
                        this.selectedItem.Properties["DesignerSettings"].Value.Properties["ConnectionString"].SetValue(this.rawConnectionString);
                    }
                }

                // MessageBox.Show("Connection Properties Model station commit completed.");
            }
        }

        /// <summary>
        /// Updates the button status.
        /// </summary>
        /// <param name="back">if set to <c>true</c> [back].</param>
        /// <param name="next">if set to <c>true</c> [next].</param>
        /// <param name="finish">if set to <c>true</c> [finish].</param>
        public void UpdateButtonStatus(bool back, bool next, bool finish)
        {
            this.CanBackButtonEnabled = back;
            this.CanNextButtonEnabled = next;
            this.CanFinishButtonEnabled = finish;
        }

        /// <summary>
        /// Configures the wizard for offline cube setting.
        /// </summary>
        private void ConfigureWizardForOfflineCubeSetting()
        {
            if (this.ConnectionMode == ConnectionMode.OfflineCube)
            {
                if (string.IsNullOrEmpty(this.filePath))
                {
                    //// Validated and update button status...
                    this.UpdateButtonStatus(false, false, false);
                }
                else
                {
                    //// Validated and update button status...
                    if (this.isDesignerSettingsValid != null)
                    {
                        this.UpdateButtonStatus(false, true, true);
                    }
                    else
                    {
                        this.UpdateButtonStatus(false, true, false);
                    }
                }
            }
        }

        /// <summary>
        /// Configures the wizard for server settings.
        /// </summary>
        private void ConfigureWizardForServerSettings()
        {
            if (this.ConnectionMode == ConnectionMode.CustomServer)
            {
                if (string.IsNullOrEmpty(this.serverName) || string.IsNullOrEmpty(this.databaseName))
                {
                    //// Validated and update button status...
                    this.UpdateButtonStatus(false, false, false);
                }
                else
                {
                    if (this.isDesignerSettingsValid != null)
                    {
                        this.UpdateButtonStatus(false, true, true);
                    }
                    else
                    {
                        this.UpdateButtonStatus(false, true, false);
                    }
                }
            }
            else if (this.ConnectionMode == ConnectionMode.RawString)
            {
                if (string.IsNullOrEmpty(this.rawConnectionString))
                {
                    //// Validated and update button status...
                    this.UpdateButtonStatus(false, false, false);
                }
                else
                {
                    if (this.isDesignerSettingsValid != null)
                    {
                        this.UpdateButtonStatus(false, true, true);
                    }
                    else
                    {
                        this.UpdateButtonStatus(false, true, false);
                    }
                }
            }
        }

        /// <summary>
        /// Browses this instance.
        /// </summary>
        private void Browse()
        {
            try
            {
                System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
				open.Filter = "Cube Files (*.cub)|*.cub";
                open.ShowDialog();
                open.RestoreDirectory = true;

                if(!string.IsNullOrEmpty(open.FileName))
                {
                    this.FilePath = open.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            if (this.ConnectionMode != ConnectionMode.None)
            {
                if (this.ConnectionMode == ConnectionMode.OfflineCube)
                {
                    return "Data Source=" + this.FilePath + "; Provider=msolap;";
                }
                else if (this.ConnectionMode == ConnectionMode.CustomServer)
                {
                    return "Data Source=" + this.ServerName + "; Initial Catalog=" + this.DatabaseName + ";";
                }
                else if (this.ConnectionMode == ConnectionMode.RawString)
                {
                    return this.RawConnectionString;
                }
            }
            else
            {
                throw new ArgumentException("Proper connection mode is not selected.");
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Tests the connection string.
        /// </summary>
        /// <returns></returns>
        public bool TestConnectionString()
        {
            string connStr = this.GetConnectionString();
            try
            {
                Syncfusion.Olap.Manager.OlapDataManager odp = new Syncfusion.Olap.Manager.OlapDataManager(connStr);

                if (odp.DataProvider.GetAllCubes.Count > 0)
                {
                    return true;
                }
                else
                {                 
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    #region Connection Mode to bool converter

    /// <summary>
    /// ConnectionMode enum to radio button state and vice-versa.
    /// </summary>
    public class ConnectionModeToBoolConverter 
        : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //// IP -> string
            //// OP -> bool

            string parameterString = parameter as string;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            return parameterValue.Equals(value);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //// IP -> bool
            //// OP -> string

            string parameterString = parameter as string;

            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }

        #endregion
    }

    #endregion
}
