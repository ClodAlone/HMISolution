using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using Utilities;
using DocumentManager.ComponentService;

namespace SQLDriver.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        
        public ChannelDetails()
        {
            bool bLoaded = false;

            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                SQLDriverChannelSettings settings = DataContext as SQLDriverChannelSettings;
                DriverCodeBase.UI.Controls.BaseChannelSettings baseDyn = new DriverCodeBase.UI.Controls.BaseChannelSettings() { DataContext = DataContext };
                DataReader.DataReaderModel drModel = null;
                if (!string.IsNullOrEmpty(settings.Provider) || !string.IsNullOrEmpty(settings.ConnectionString))
                    drModel = new DataReader.DataReaderModel() { Connection = "DataProvider=" + settings.Provider +";" + settings.ConnectionString};
                ConnectionStringValue cnValue = new ConnectionStringValue() { Value = drModel };
                editConnectionString.DataContext = cnValue;
                cnValue.PropertyChanged += (obj, ea) =>
                {
                    if (ea.PropertyName == "Value")
                    {
                        try
                        {
                            settings.ConnectionString = (cnValue?.Value as DataReader.DataReaderModel)?.Connection;
                            settings.Provider = (cnValue?.Value as DataReader.DataReaderModel)?.DataProvider;
                        }
                        catch (Exception)
                        {
                        }
                    }
                };
                DataReader.DataReaderModel bDrModel = null;
                if (!string.IsNullOrEmpty(settings.BackupProvider) || !string.IsNullOrEmpty(settings.BackupConnectionString))
                 bDrModel = new DataReader.DataReaderModel() { Connection = "DataProvider=" + settings.BackupProvider + ";" + settings.BackupConnectionString };
                ConnectionStringValue bCnValue = new ConnectionStringValue() { Value = bDrModel };
                editBackupConnectionString.DataContext = bCnValue;
                bCnValue.PropertyChanged += (obj, ea) =>
                {
                    if (ea.PropertyName == "Value")
                    {
                        try
                        {
                            settings.BackupConnectionString = (bCnValue?.Value as DataReader.DataReaderModel)?.Connection;
                            settings.BackupProvider = (bCnValue?.Value as DataReader.DataReaderModel)?.DataProvider;
                        }
                        catch (Exception)
                        {
                        }
                    }
                };
                //baseDyn.ScheduleTime.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.ScheduleTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Visible;
                baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Visible;
                baseDyn.PollError.Visibility = System.Windows.Visibility.Visible;
                baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Visible;
                MainStack.Children.Insert(0, baseDyn);
            };

        }
        
        private string _ConnectionString;
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set
            {
                _ConnectionString = value;
            }
        }

    }
}
