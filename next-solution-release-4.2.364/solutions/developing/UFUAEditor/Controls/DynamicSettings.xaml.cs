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
using UFUAEditor.Document;
using DriverSettingsInterfaces;
using System.Reflection;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpo;
using UFInterfaces.Editors;
using HelpProvider.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using DevExpress.Xpf.Core;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for DynamicSettings.xaml
    /// </summary>
    public partial class DynamicSettings : UserControl, IDisposable
    {
        #region Declarations
        readonly UFUAServerDocument Document;
        readonly IDynamicSettingsEditing DynamicTag;
        readonly List<UserControl> listControls = new List<UserControl>();
        readonly String filterDriverName = null;
        List<UFUAModel.UFUACommunicationDriver> comunicationDrivers;
        bool bDisposed;
        #endregion

        public DynamicSettings(UFUAServerDocument doc, IDynamicSettingsEditing dynTag, bool allowInstalldDriver = true, string filter = null)
        {
            InitializeComponent();
            Document = doc;
            DynamicTag = dynTag;
            filterDriverName = filter;

            comunicationDrivers = GetComunicationDrivers();
            if (comunicationDrivers.Count == 0)
            {
                stackPanel.Visibility = Visibility.Visible;
                tabControlExt.Visibility = Visibility.Collapsed;
                btnInstallDriver.Visibility = allowInstalldDriver ? Visibility.Visible : Visibility.Collapsed;
            }

            Document.GetConfiguration().ComunicationDrivers.CollectionChanged += CommunicationDriversChanged;

            InitTabs();
        }

        private void CommunicationDriversChanged(object sender, XPCollectionChangedEventArgs e)
        {
            if (!bDisposed && (e.CollectionChangedType == DevExpress.Xpo.XPCollectionChangedType.AfterAdd ||
                e.CollectionChangedType == DevExpress.Xpo.XPCollectionChangedType.AfterRemove))
            {
                Dispatcher.InvokeIfRequired(() =>
                {
                    comunicationDrivers = GetComunicationDrivers();
                    stackPanel.Visibility = comunicationDrivers.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                    tabControlExt.Visibility = comunicationDrivers.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

                    InitTabs();
                });
            }
        }

        List<UFUAModel.UFUACommunicationDriver> GetComunicationDrivers()
        {
            return (from c in Document.GetConfiguration().ComunicationDrivers.AsParallel()
                    where filterDriverName == null || c.Name == filterDriverName
                    select c).ToList();
        }

        private void InitTabs()
        {
            using (new WaitCursor())
            {
                tabControlExt.UpdateLayout();
                tabControlExt.BeginInit();

                foreach (var driver in comunicationDrivers)
                {
                    var drvdll = String.Format("{0}\\{1}", UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), driver.AssemblyName);
                    var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(drvdll);

                    if (!System.IO.File.Exists(drvdll) || !System.IO.File.Exists(uidll))
                        continue;

                    try
                    {
                        var types = Assembly.LoadFile(uidll).GetTypes();
                        var list = (from t in types/*.AsParallel()*/
                                    where !t.IsAbstract && typeof(ICommunicationDriverWpfEditing).IsAssignableFrom(t)
                                    select (ICommunicationDriverWpfEditing)Activator.CreateInstance(t)).ToList();

                        var control = list[0].DynamicSettingsEditor(Document.ConnectionString);
                        var driverHelpLink = list[0].GetType().FullName;
                        control.DataContext = DynamicTag;
                        var grid = new Grid();
                        control.ClearValue(FrameworkElement.WidthProperty);
                        control.ClearValue(FrameworkElement.HeightProperty);
                        listControls.Add(control);
                        var scroll = new ScrollViewer()
                        {
                            Content = control,
                            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                        };
                        Grid.SetRow(scroll, 0);

                        grid.RowDefinitions.Add(new RowDefinition());
                        grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        
                        var but = new Button() { Tag = driver, DataContext = list[0] , Height = 30};
                        but.Content = Properties.Resources.EditDriversSettings;
                        but.Click += but_Click;
                        Grid.SetRow(but, 1);

                        grid.Children.Add(scroll);
                        grid.Children.Add(but);
                        grid.Tag = driverHelpLink.Replace("UI.", "");

                        var tabItem = new DXTabItem()
                        {
                            Header = driver.FriendlyName,
                            Content = grid
                        };
                        tabItem.InitItemTemplate();

                        for (int i = 0; i < tabControlExt.Items.Count; i++)
                        {
                            var tb = tabControlExt.Items[i] as DXTabItem;
                            if ((string)tb.Header == driver.FriendlyName)
                            {
                                tabControlExt.Items.RemoveAt(i);
                                tabControlExt.UpdateLayout();
                                break;
                            }
                        }
                        

                        int j = tabControlExt.Items.Add(tabItem);
                        (tabControlExt.Items[j] as DXTabItem).Visibility = System.Windows.Visibility.Hidden;
                        (tabControlExt.Items[j] as DXTabItem).Visibility = System.Windows.Visibility.Visible;
                        (tabControlExt.Items[j] as DXTabItem).Refresh();
                    }
                    catch (Exception ex)
                    { 
                    }
                }

                tabControlExt.EndInit();
            }
        }

        void but_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if(button == null)
                return;

            var interfaccia = button.DataContext as ICommunicationDriverWpfEditing;
            if (interfaccia == null)
                return;

            var control = interfaccia.GeneralSettingsEditor as UserControl;
            if (control == null)
                return;
            listControls.Add(control);
            var driver = button.Tag as UFUAModel.UFUACommunicationDriver;
            if (driver == null)
                return;

            try
            {
                var settingsContext = new ComunicationSettingsContext2()
                {
                    ConnectionString = Document.ConnectionString,
                    bProtected = Document.Protected,
                    protectionCode = Document.Id,
                    listTag = Document.GetFlatFullTagNameNodeIdCollection()
                };
                control.DataContext = settingsContext;
                var driverName = System.IO.Path.GetFileNameWithoutExtension(driver.AssemblyName);
                GeneralDialogContent Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Title = driver.FriendlyName,
                    Owner = this.FindParent<Window>()
                };
                if (Dialog.ShowDialog() == true)
                {
                    interfaccia.SaveSettings(control);
                    Dispatcher.InvokeIfRequired(() =>
                    {
                        InitTabs();

                        for (int i = 0; i < tabControlExt.Items.Count; i++)
                        {
                            if ((tabControlExt.Items[i] as DXTabItem).Header.ToString() == driver.FriendlyName)
                                tabControlExt.SelectedItem = tabControlExt.Items[i];
                        }
                    });
                }
            }
            catch (DriverBaseInterfaces.ValidatingDocumentException ex)
            {
                var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, ex.FilePath));
            }
            /*
            if (control is IDisposable)
                (control as IDisposable).Dispose();
            */
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var driverList = new DriverList();
            GeneralDialogContent Dialog = new GeneralDialogContent(driverList)
            {
                DialogKeepContent = true,
                Owner = this.FindParent<Window>(),
                HelpLink = "DriverList"
            };
            if (Dialog.ShowDialog() == true)
            {
                var driverInfo = driverList.GetSelectedDriverInfo();
                if (driverInfo != null)
                {
                    var driver = Document.AddNewDriver();
                    driver.Factory = driverInfo.Factory;
                    driver.FriendlyName = driverInfo.FriendlyName;
                    driver.Name = driverInfo.AssemblyName.Substring(0, driverInfo.AssemblyName.Length - 4);
                    driver.AssemblyName = driverInfo.AssemblyName;

                    Document.GetConfiguration().ComunicationDrivers.Add(driver);
                }

                e.Handled = true;
            }

            if (driverList is IDisposable)
                (driverList as IDisposable).Dispose();
        }

        #region IDisposable

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            Document.GetConfiguration().ComunicationDrivers.CollectionChanged -= CommunicationDriversChanged;

            listControls.ForEach(control =>
                {
                    if (control is IDisposable)
                        (control as IDisposable).Dispose();
                });
            listControls.Clear();

            foreach (DXTabItem item in tabControlExt.Items)
            {
                if (item.Content is FrameworkElement)
                {
                    var fe = item.Content as FrameworkElement;
                    var list = (from p in fe.GetChildrenOfType<FrameworkElement>()
                                where p is IDisposable
                                select p as IDisposable).ToList();

                    list.ForEach((o) => o.Dispose());
                }

            }
            tabControlExt.Items.Clear();
            tabControlExt.Dispose();
        }

        #endregion

        private void tabControlExt_SelectionChanged(object sender, TabControlSelectionChangedEventArgs e)
        {
            GeneralDialog generalDialogContent = this.FindParent<GeneralDialog>();
            if (generalDialogContent != null)
            {
                generalDialogContent.HelpLink = GetHelpLink();
            }
        }
        string GetHelpLink()
        {
            string prop = string.Empty;

            prop = this.GetType().FullName;

            var driver = tabControlExt.SelectedItemContent;
            if (driver != null && (driver is Grid) && (driver as Grid).Children.Count > 0)
            {
                prop = (driver as Grid).Tag?.ToString();
            }

            return prop;
        }
    }
}
