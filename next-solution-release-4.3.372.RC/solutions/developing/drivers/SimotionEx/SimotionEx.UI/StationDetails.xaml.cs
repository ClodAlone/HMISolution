using DevExpress.Xpo;
using Ookii.Dialogs.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Simotion.UI
{
    /// <summary>
    /// Interaction logic for StationDetails.xaml
    /// </summary>
    public partial class StationDetails : UserControl
    {
        string _strConnString;
        UnitOfWork _ufw;
        bool bLoaded = false;
        SimotionStationSettings stationSetting;

        public StationDetails(string strConnString, UnitOfWork ufw)
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                stationSetting = DataContext as SimotionStationSettings;                
                var baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                //insert Channel in ComboBox CmbChannel
                baseStation.CmbChannel.ItemsSource = stationSetting.DriverSettings.ChannelSettings;
                MainStack.Children.Insert(0, baseStation);

                _strConnString = strConnString;
                _ufw = ufw;

                // from wizard --> disable file selection
                if (string.IsNullOrEmpty(_strConnString))
                {
                    btnSelecFile.IsEnabled = false;
                }
                else
                {
                    // Symbolic file not set --> verify is old driver version
                    if (String.IsNullOrEmpty(stationSetting.SymbolicFile))
                        stationSetting.SymbolicFile = SimotionUISymbolicFileManagement.GetSymbolicFileName(_strConnString, stationSetting, SimotionUISymbolicFileManagement.FilePath.SettingFileName);
                }
            };
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            string sourceFile = null;
            string resultFile = null;

            if (Environment.UserInteractive)
            {
                VistaOpenFileDialog dialog = new VistaOpenFileDialog();
                dialog.Filter = Properties.Resources.SYMBOLIC_FILE_FILTER;
                // if symbolic file was defined, open dialog directly to this file                                
                dialog.FileName = SimotionUISymbolicFileManagement.GetSymbolicFileName(_strConnString, stationSetting, SimotionUISymbolicFileManagement.FilePath.FileWithFullPath);

                if (dialog.ShowDialog() != true)
                    return;

                sourceFile = dialog.FileName;
            }

            if (!SimotionUISymbolicFileManagement.IsValidSymbolicFile(sourceFile))
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorSelectedSymbolicFileInvalid, sourceFile));
                return;
            }

            if (SimotionProtocol.IsImportedSymbolicFile(sourceFile))
            {
                resultFile = SimotionProtocol.GetSymbolicFileImportName(sourceFile);
            }
            else
            {
                SimotionImportFile import = new SimotionImportFile(_strConnString, _ufw);
                resultFile = import.ImportRequest(sourceFile, stationSetting);
                if (string.IsNullOrEmpty(resultFile))
                    resultFile = txtSymbolicFile.Text;
            }

            txtSymbolicFile.Text = resultFile.ToString();
        }
    }
}

