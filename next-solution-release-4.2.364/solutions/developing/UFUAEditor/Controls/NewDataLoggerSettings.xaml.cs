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
using Utilities;
using Utilities.WPF;
using CommonControls;
using UFUAEditor.Document;
using ViewModelLib;
using Ookii.Dialogs.Wpf;
using UFInterfaces.Editors;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewDataLoggerSettings.xaml
    /// </summary>
    public partial class NewDataLoggerSettings : UserControl
    {
        #region Declarations
        UFUAServerDocument ufuaDocument;
        #endregion

        #region Construtctors
        public NewDataLoggerSettings(UFUAServerDocument doc)
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                var dls = DataContext as DataLoggerModel.DataLoggerSettings;
                if(dls != null)
                {
                    TagEntityReferenceModel EnTag = new TagEntityReferenceModel() { Value = dls.EnableRecordingTag };
                    TagEntityReferenceModel RecTag = new TagEntityReferenceModel() { Value = dls.RecordingTag };
                    TagEntityReferenceModel ResetTag = new TagEntityReferenceModel() { Value = dls.ResettingTag };

                    textEnableRecordingTag.DataContext = EnTag;
                    textRecordingTag.DataContext = RecTag;
                    textResettingTag.DataContext = ResetTag;

                    EnTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            try
                            {
                                if (EnTag.Value != null)
                                    dls.EnableRecordingTag = EnTag.Value;
                                else
                                    dls.EnableRecordingTag = null;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    };

                    RecTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            try
                            {
                                if (RecTag.Value != null)
                                    dls.RecordingTag = RecTag.Value;
                                else
                                    dls.RecordingTag = null;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    };

                    ResetTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            try
                            {
                                if (ResetTag.Value != null)
                                    dls.ResettingTag = ResetTag.Value;
                                else
                                    dls.ResettingTag = null;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    };
                }

                




            };

            ufuaDocument = doc;
            textEditMaxAge.Mask = String.Format("d '({0})' hh:mm:ss", Properties.Resources.TimeSpanFormatDaysPart);
            textEditRecordingTimeInterval.Mask = String.Format("d '({0})' hh:mm:ss.fff", Properties.Resources.TimeSpanFormatDaysPart);
            textEditHysteresisTimeInterval.Mask = "hh:mm:ss";
        }
        #endregion

        #region Event Handlers
        private void textEditConnection_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var doc = DataContext as DataLoggerModel.DataLoggerSettings;
            var _dataReaderModel = doc.ConnectionSettings;
            //if (dataReaderModel == null)
            //    dataReaderModel = new DataReader.DataReaderModel();
            var dataReaderModel = new DataReader.DataReaderModel();
            if(_dataReaderModel != null)
            {
                dataReaderModel.Connection = _dataReaderModel.Connection;
                dataReaderModel.DataProvider = _dataReaderModel.DataProvider;
                dataReaderModel.DataProviderDisplayName = _dataReaderModel.DataProviderDisplayName;
                dataReaderModel.DataProviderDescription = _dataReaderModel.DataProviderDescription;
                dataReaderModel.DataProviderShortDisplayName = _dataReaderModel.DataProviderShortDisplayName;
                dataReaderModel.DataSourceName = _dataReaderModel.DataSourceName;
                dataReaderModel.DataSourceDisplayName = _dataReaderModel.DataSourceDisplayName;
            };

            var viewModel = new DataReaderEditor.DataReaderModelView(dataReaderModel, ufuaDocument?.rootBase);
            viewModel.EditConnection.Execute(null);

            if (_dataReaderModel == null)
                doc.ConnectionSettings = dataReaderModel;
            else if (_dataReaderModel.Connection != dataReaderModel.Connection ||
                     _dataReaderModel.DataProvider != dataReaderModel.DataProvider ||
                     _dataReaderModel.DataProviderDisplayName != dataReaderModel.DataProviderDisplayName ||
                     _dataReaderModel.DataProviderDescription != dataReaderModel.DataProviderDescription ||
                     _dataReaderModel.DataProviderShortDisplayName != dataReaderModel.DataProviderShortDisplayName ||
                     _dataReaderModel.DataSourceName != dataReaderModel.DataSourceName ||
                     _dataReaderModel.DataSourceDisplayName != dataReaderModel.DataSourceDisplayName)
                doc.ConnectionSettings = dataReaderModel;

            textEditConnection.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        private void textEditConnection_ClearButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var doc = DataContext as DataLoggerModel.DataLoggerSettings;
            doc.ConnectionSettings = null;
            textEditConnection.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        #endregion
    }
}
