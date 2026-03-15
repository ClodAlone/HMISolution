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
using UriResolver.ComponentService;
using System.IO;
using CustomWizardPlugin.ComponentService;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using DocumentManager.ComponentService;

namespace CustomWizardPlugin
{
    /// <summary>
    /// Interaction logic for AlarmStep.xaml
    /// </summary>
    public partial class AlarmStep : UserControl, IWizardElement
    {
        public AlarmStep()
        {
            InitializeComponent();
            InitLabels();
        }

        private void InitLabels()
        {
            var cultInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
            labelHisTitle.Content = cultInfo.ToTitleCase(Properties.Resources.AddAlarms);
            labelDlrTitle.Content = cultInfo.ToTitleCase(Properties.Resources.AddDatalogger);

            checkboxAnalogAlarm.Content = cultInfo.ToTitleCase(Properties.Resources.AddAnalogPrototype);
            checkboxDigitalAlarm.Content = cultInfo.ToTitleCase(Properties.Resources.AddDigitalPrototype);

            checkboxDLROnChange.Content = cultInfo.ToTitleCase(Properties.Resources.AddDataloggerOnChange);
            checkboxDLR5Sec.Content = cultInfo.ToTitleCase(Properties.Resources.AddDatalogger5Sec);
            checkboxDLR30Sec.Content = cultInfo.ToTitleCase(Properties.Resources.AddDatalogger30Sec);
            checkboxDLR1Min.Content = cultInfo.ToTitleCase(Properties.Resources.AddDatalogger1Min);
        }

        public bool Execute()
        {
            //if ((bool)WithAlarmRadio.IsChecked &&
            //    ((bool)checkboxAnalogAlarm.IsChecked ||
            //    (bool)checkboxDigitalAlarm.IsChecked))
            //{
            //    AddAlarms();
            //}

            //if ((bool)WithDlrRadio.IsChecked &&
            //    ((bool)checkboxDLROnChange.IsChecked ||
            //    (bool)checkboxDLR5Sec.IsChecked ||
            //    (bool)checkboxDLR30Sec.IsChecked ||
            //    (bool)checkboxDLR1Min.IsChecked))
            //{
            //    AddHistaricals();
            //}
            //if ((bool)checkboxAnalogAlarm.IsChecked ||
            //    (bool)checkboxDigitalAlarm.IsChecked)
            {
                AddAlarms();
            }

            if ((bool)checkboxDLROnChange.IsChecked ||
                (bool)checkboxDLR5Sec.IsChecked ||
                (bool)checkboxDLR30Sec.IsChecked ||
                (bool)checkboxDLR1Min.IsChecked)
            {
                AddHistaricals();
            }

            return false;
        }

        void AddHistaricals()
        {
            UpdateDLRDocument();
        }

        void UpdateDLRDocument()
        {
            var connString = CustomWizardPluginComponent.ProjectView.GetServerIOConnectionStringFromUri(CustomWizardPluginComponent.ProjectUri);
            if (connString != null)
            {
                using (var dl = XpoDefault.GetDataLayer(connString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (var uow = new UnitOfWork(dl))
                    {

                        if ((bool)checkboxDLROnChange.IsChecked)
                        {
                            //checkboxDLROnChange
                            var datalogger = new UFUAModel.UFUAHistorianSettings(uow) { Name = Properties.Resources.DLROnChange };
                        }
                        if ((bool)checkboxDLR5Sec.IsChecked)
                        {
                            //checkboxDLR5Sec
                            var datalogger = new UFUAModel.UFUAHistorianSettings(uow) { Name = Properties.Resources.DLR5Sec };
                            datalogger.MinTimeInterval = new TimeSpan(0, 0, 5);
                            datalogger.MaxTimeInterval = new TimeSpan(0, 0, 5);
                        }
                        if ((bool)checkboxDLR30Sec.IsChecked)
                        {
                            //checkboxDLR30Sec
                            var datalogger = new UFUAModel.UFUAHistorianSettings(uow) { Name = Properties.Resources.DLR30Sec };
                            datalogger.MinTimeInterval = new TimeSpan(0, 0, 30);
                            datalogger.MaxTimeInterval = new TimeSpan(0, 0, 30);
                        }
                        if ((bool)checkboxDLR1Min.IsChecked)
                        {
                            //checkboxDLR1Min
                            var datalogger = new UFUAModel.UFUAHistorianSettings(uow) { Name = Properties.Resources.DLR1Min };
                            datalogger.MinTimeInterval = new TimeSpan(0, 1, 0);
                            datalogger.MaxTimeInterval = new TimeSpan(0, 1, 0);
                        }

                        uow.CommitChanges();
                    }
                }
            }
        }

        void AddAlarms()
        {
            UpdateDocument();
        }

        void UpdateDocument()
        {
            var connString = CustomWizardPluginComponent.ProjectView.GetServerIOConnectionStringFromUri(CustomWizardPluginComponent.ProjectUri);
            if (connString != null)
            {
                using (var dl = XpoDefault.GetDataLayer(connString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (var uow = new UnitOfWork(dl))
                    {
                        var aa = new UFUAModel.UFUAArea(uow) { Name = Properties.Resources.AlarmAreaName, NodeId = Guid.NewGuid() };
                        var aSource = new UFUAModel.UFUAAlarmSource(uow) { Name = Properties.Resources.AlarmSourceName, NodeId = Guid.NewGuid() };
                        if (aa != null)
                            aa.UFUAAlarmSources.Add(aSource);

                        if ((bool)checkboxAnalogAlarm.IsChecked)
                        {
                            //Analog Prototipe
                            var aAnalogDefinition = new UFUAModel.UFUAAlarmDefinition(uow) { Name = Properties.Resources.AlarmAnalogPrototipeName, NodeId = Guid.NewGuid() };
                            aAnalogDefinition.AlarmType = UFUAModel.AlarmType.ExclusiveLevel;
                            if (aSource != null)
                                aSource.UFUAAlarmDefinitions.Add(aAnalogDefinition);
                        }

                        if ((bool)checkboxDigitalAlarm.IsChecked)
                        {
                            //Digital Prototipe
                            var aDigitalDefinition = new UFUAModel.UFUAAlarmDefinition(uow) { Name = Properties.Resources.AlarmDigitalPrototipeName, NodeId = Guid.NewGuid() };
                            if (aSource != null)
                                aSource.UFUAAlarmDefinitions.Add(aDigitalDefinition);
                            var sSourceDefinitions = aSource.UFUAAlarmDefinitions;
                        }

                        uow.CommitChanges();
                    }
                }
            }
        }
    }
}
