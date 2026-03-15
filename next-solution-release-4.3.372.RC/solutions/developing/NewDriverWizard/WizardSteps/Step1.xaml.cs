using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NewDriverWizard.ComponentService;
using UFInterfaces.Editors;
using Utilities.WPF;

namespace NewDriverWizard
{
    /// <summary>
    /// Interaction logic for ProjectWizardPathAndType.xaml
    /// </summary>
    public partial class Step1 : UserControl, IWizardElement
    {
        internal bool EnNext { get { return (driverList.DataContext as UFInterfaces.Editors.DriverXmlInfo) != null; } }
        UserControl driverList = null;
        
        public Step1()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                try
                {
                    driverList = NewDriverWizardComponent.ProjectView.UFUAEditorManager.GetDriverListControl(false);
                    driverList.ClearValue(FrameworkElement.WidthProperty);
                    driverList.ClearValue(FrameworkElement.HeightProperty);
                    driverList.Margin = new System.Windows.Thickness(5);
                    driverList.DataContextChanged += (obj, ea) =>
                    {
                        Execute();
                    };

                    stepContent.Dispose();
                    stepContent.Children.Clear();
                    stepContent.Children.Add(driverList);
                    //Execute();
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
                    errorText.Text = String.Format(Properties.Resources.ErrorOnLoadingStep, ex.Message);

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
                DriverXmlInfo drv = driverList.DataContext as UFInterfaces.Editors.DriverXmlInfo;
                NewDriverWizardComponent.xmlDriver = drv;
                NewDriverWizardComponent.driver = drv.AssemblyName;
                
                NewDriverWizardComponent.genSett = null;
                NewDriverWizardComponent.channelName = null;
                NewDriverWizardComponent.StationName = null;

                NewDriverWizardComponent.wizardPluginComponent.UpdateHelpLink(this, $"{System.IO.Path.GetFileNameWithoutExtension(drv.AssemblyName)}");
                return false;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        public bool CanShowed()
        {
            return true;
        }
    }
}
