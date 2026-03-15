using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DriverCodeBase;
using DriverSettingsInterfaces;
using NewDriverWizard.ComponentService;
using UFUAEditor.Controls;
using System.Windows;
using Utilities.WPF;

namespace NewDriverWizard
{
    /// <summary>
    /// Interaction logic for Step4.xaml
    /// </summary>
    public partial class Step4 : UserControl, IWizardElement
    {
        bool isLoaded;
        string prevDriverName = string.Empty;
        public Step4()
        {
            InitializeComponent();

            Loaded += (o, e) => 
            {
                if (NewDriverWizardComponent.genSett == null || String.IsNullOrEmpty(NewDriverWizardComponent.driver))
                    return;

                isLoaded = true;
                string rootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(
                    String.Format("{0}\\Drivers\\{1}", rootPath,
                    NewDriverWizardComponent.driver));

                if (!string.IsNullOrEmpty(prevDriverName))
                {
                    if (prevDriverName.CompareTo(NewDriverWizardComponent.driver) == 0)
                        return;
                }

                prevDriverName = NewDriverWizardComponent.driver;

                ICommunicationDriverWpfEditing driverWpfEditing = null;
                try
                {
                    var types = Assembly.LoadFile(uidll).GetTypes();
                    var list = (from t in types/*.AsParallel()*/
                                where !t.IsAbstract && typeof(ICommunicationDriverWpfEditing).IsAssignableFrom(t)
                                select (ICommunicationDriverWpfEditing)Activator.CreateInstance(t)).ToList();
                    driverWpfEditing = list[0];

                    if (driverWpfEditing == null)
                        return;

                    var station = driverWpfEditing.NewStationSettingsEditor(NewDriverWizardComponent.genSett);
                    if (station == null)
                        return;

                    station.Margin = new System.Windows.Thickness(5);
                    station.ClearValue(FrameworkElement.WidthProperty);
                    station.ClearValue(FrameworkElement.HeightProperty);
                    station.Loaded += (obj, ea) =>
                    {
                        NewDriverWizardComponent.wizardPluginComponent.UpdateHelpLink(this, station?.DataContext?.GetType().FullName);
                    };

                    stepContent.Dispose();
                    stepContent.Children.Clear();
                    stepContent.Children.Add(station);
                }
                catch (Exception ex)
                {
                    var errorText = new TextBlock() 
                    {
                        Margin = new Thickness(10),
                        Text = String.Format(Properties.Resources.ErrorOnLoadingStep, ex.Message),
                        FontSize = 24,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    stepContent.Dispose();
                    stepContent.Children.Clear();
                    stepContent.Children.Add(errorText);
                }
            };
        }

        public bool Execute()
        {
            try
            {
                if (stepContent.Children.Count > 0 && stepContent.Children[0] is UserControl)
                    NewDriverWizardComponent.StationName = ((stepContent.Children[0] as UserControl).DataContext as StationSettings).Name;
                return false;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        public bool CanShowed()
        {
            if (isLoaded)
                return true;

            if (NewDriverWizardComponent.genSett is DriverCodeBase.DriverSettings)
            {
                var settings = NewDriverWizardComponent.genSett as DriverCodeBase.DriverSettings;
                return settings.StationSettings.Count == 0;
            }
            if (NewDriverWizardComponent.genSett is DriverCodeBaseEx.DriverSettings)
            {
                var settings = NewDriverWizardComponent.genSett as DriverCodeBaseEx.DriverSettings;
                return settings.StationSettings.Count == 0;
            }
            return false;
        }
    }
}
