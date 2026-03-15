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
using UIMsgBoxAlertService.ComponentService;
using UFUAEditor.Document;
using UFInterfaces.Editors;
using HelpProvider.ComponentService;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewHistoricalSettings.xaml
    /// </summary>
    public partial class NewHistoricalSettings : UserControl
    {
        #region Declarations
        readonly UFUAServerDocument ufuaDocument;
        readonly IUIMsgBoxAlertService UIMsgBoxAlertService;
        readonly IHelpProvider HelpProvider;
        #endregion

        #region Constructors
        public NewHistoricalSettings(UFUAServerDocument doc)
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                var settings = DataContext as UFUAModel.UFUAHistorianSettings;
                if(settings != null)
                {
                    TagEntityReferenceModel EnableTag = new TagEntityReferenceModel() { Value = settings.EnableTag };
                    textHisEnabledTag.DataContext = EnableTag;
                    EnableTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (EnableTag.Value != null)
                                settings.EnableTag = EnableTag.Value;
                            else
                                settings.EnableTag = null;
                        }
                    };
                }
            };

            ufuaDocument = doc;
            UIMsgBoxAlertService = doc.EditorManagerComponent != null ? doc.EditorManagerComponent.UIInterface : null;
            HelpProvider = doc.EditorManagerComponent != null ? doc.EditorManagerComponent.HelpProvider : null;
            textEditMaxAge.Mask = String.Format("d '({0})' hh:mm:ss", Properties.Resources.TimeSpanFormatDaysPart);
            textEditMinInterval.Mask =
            textEditMaxInterval.Mask = String.Format("d '({0})' hh:mm:ss.fff", Properties.Resources.TimeSpanFormatDaysPart);
            comboDeviationType.ItemsSource = Enum.GetValues(typeof(UFUAModel.DeviationType));
        }
        #endregion

        #region Event Handlers
        private void textEditConnection_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            var connectionstring = XpoHelpers.XpoHelper.NormalizeConnectionString(textEditConnection.Text, ufuaDocument.rootBase);
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connectionstring, "ConnectionSourcePropertyEditor", this.FindParent<Window>(), UIMsgBoxAlertService, HelpProvider))
            {
                var settings = DataContext as UFUAModel.UFUAHistorianSettings;
                settings.ConnectionSettings = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(connectionstring, ufuaDocument.rootBase);
            }
        }

        #endregion
    }
}
