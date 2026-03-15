using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DevExpress.Xpo.DB;
using DocumentManager.ComponentService;
using DriverSettingsInterfaces;
using NewDriverWizard.ComponentService;
using UFUAEditor.Controls;
using System.Windows;
using Utilities.WPF;
using DevExpress.Xpf.CodeView.Documents;

namespace NewDriverWizard
{
    /// <summary>
    /// Interaction logic for Step2.xaml
    /// </summary>
    public partial class Step2 : UserControl, IWizardElement
    {
        string prevDriverName = string.Empty;
        internal bool EnNext { get { return (stepContent.Children.Count > 0 && stepContent.Children[0] is UserControl && (stepContent.Children[0] as UserControl).DataContext != null); } }
        public Step2()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (String.IsNullOrEmpty(NewDriverWizardComponent.driver))
                    return;

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

                    if (driverWpfEditing == null /*|| driverWpfEditing.GeneralSettingsEditor == null*/)
                        return;
                                        
                    String connString = null;
                    //String xmlfile = null;

                    //xmlfile = String.Format("{0}\\{1}\\{2}\\{3}",
                    //    System.IO.Path.GetDirectoryName(NewDriverWizardComponent.ProjectUri.LocalPath),
                    //    System.IO.Path.GetFileNameWithoutExtension(NewDriverWizardComponent.ProjectUri.LocalPath),
                    //    (NewDriverWizardComponent.ProjectView.UFUAEditorManager as IDocumentManager).TypeLabel,
                    //    "Server.UFUAServer");

                    connString = NewDriverWizardComponent.currentConn;//InMemoryDataStore.GetConnectionString(xmlfile);

                    var control = driverWpfEditing.WzrdGeneralSettingsEditor()/*GeneralSettingsEditor*/;
                    if (control == null) return;
                    var settingsContext = new ComunicationSettingsContext2() {
                        ConnectionString = connString,
                        bProtected = ((UFUAEditor.Document.UFUAServerDocument)(this.DataContext)).Protected,
                        protectionCode = ((UFUAEditor.Document.UFUAServerDocument)(this.DataContext)).Id,
                    };

                    control.DataContext = settingsContext;
                    control.Margin = new System.Windows.Thickness(5);
                    control.ClearValue(FrameworkElement.WidthProperty);
                    control.ClearValue(FrameworkElement.HeightProperty);
                    control.Loaded += (obj, ea) =>
                    {
                        NewDriverWizardComponent.wizardPluginComponent.UpdateHelpLink(this, control?.DataContext?.GetType().FullName);
                    };

                    stepContent.Dispose();
                    stepContent.Children.Clear();
                    stepContent.Children.Add(control);
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
                NewDriverWizardComponent.genSett = null;
                if (stepContent.Children.Count > 0 && stepContent.Children[0] is UserControl)
                    NewDriverWizardComponent.genSett = (stepContent.Children[0] as UserControl).DataContext;
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
