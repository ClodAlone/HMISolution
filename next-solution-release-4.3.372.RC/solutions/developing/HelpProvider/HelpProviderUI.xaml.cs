using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HelpProvider.ComponentService;
using System.Xml.Linq;
using log4net;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using WPFUtilities;
using Utilities.WPF;
using System.Globalization;
using Utilities;

namespace HelpProvider
{
    /// <summary>
    /// Interaction logic for PluginUI.xaml
    /// </summary>
    public partial class HelpProviderUI : UserControl, INotifyPropertyChanged
    {
        HelpProviderComponent helpProviderComponent;
        HelpProviderReader helpProviderReader;

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.HelpManager);

        object lockfile = new object();
        
        private  bool _LocalHelp = false;
        public  bool LocalHelp
        {
            get { return _LocalHelp; }
            set
            {
                if (value != _LocalHelp)
                {
                    _LocalHelp = value;
                    writeLocalHelp();

                    OnPropertyChanged("LocalHelp");
                }
            }
        }
        void readLocalHelp()
        {
            lock (lockfile)
            {
                try
                {
                    string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                    var doc = XElement.Load(String.Format("{0}.{1}\\HelpOnLine\\WebHelpList.xml", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), mainversion));

                    var stlist = (from item in doc.Descendants("LocalHelp")
                                  select item).ToList();
                    if (stlist.Count > 0)
                    {
                        _LocalHelp = Convert.ToBoolean(stlist[0].Value);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(Properties.Resources.ErrorReadingLocalHelp, ex);
                }
            }
        }
        void writeLocalHelp()
        {
            lock (lockfile)
            {
                try
                {
                    string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                    var doc = XElement.Load(String.Format("{0}.{1}\\HelpOnLine\\WebHelpList.xml", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),mainversion));
                    var stlist = (from item in doc.Descendants("LocalHelp")
                                  select item).ToList();
                    if (stlist.Count > 0)
                    {
                        stlist[0].SetValue(LocalHelp);
                    }
                    else
                        doc.Add(new XElement("LocalHelp", LocalHelp));
                    doc.Save(String.Format("{0}.{1}\\HelpOnLine\\WebHelpList.xml", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),mainversion));
                }
                catch (Exception ex)
                {
                    log.Error(Properties.Resources.ErrorSettingLocalHelp, ex);
                }
            }
        }
        public HelpProviderUI(HelpProviderComponent hc)
        {
            InitializeComponent();
            
            readLocalHelp();
            helpProviderComponent = hc;

        }

        private string _DefaultPage;
        public string DefaultPage
        {
            get { return _DefaultPage; }
            set
            {
                if (value != _DefaultPage)
                {
                    _DefaultPage = value;

                    OnPropertyChanged("DefaultPage");
                }
            }
        }

        private string _LocalFilePath;
        public string LocalFilePath
        {
            get { return _LocalFilePath; }
            set
            {
                if (value != _LocalFilePath)
                {
                    _LocalFilePath = value;

                    OnPropertyChanged("LocalFilePath");
                }
            }
        }
        private string _LocalInvariantFilePath;
        public string LocalInvariantFilePath
        {
            get { return _LocalInvariantFilePath; }
            set
            {
                if (value != _LocalInvariantFilePath)
                {
                    _LocalInvariantFilePath = value;

                    OnPropertyChanged("LocalInvariantFilePath");
                }
            }
        }
        private string _WebFilePath;
        public string WebFilePath
        {
            get { return _WebFilePath; }
            set
            {
                if (value != _WebFilePath)
                {
                    _WebFilePath = value;

                    OnPropertyChanged("WebFilePath");
                }
            }
        }
        string lastCultureUsed;
        public void InitRootPath()
        {
            lock (lockfile)
            {
                var cultureName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var resources = HelpProvider.Properties.Resources.ResourceManager.GetResourceSet(new CultureInfo(""), false, false);
                if (resources != null)
                    cultureName = "en";

                if (lastCultureUsed == cultureName)
                        return;
                lastCultureUsed = cultureName;

                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                var doc = XElement.Load(String.Format("{0}.{1}\\HelpOnLine\\WebHelpList.xml", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),mainversion));
                var element = (from item in doc.Descendants("LocalPath")
                               select item).FirstOrDefault();
                if (element != null && !string.IsNullOrEmpty(element.Value))
                {
                    LocalFilePath = $"{element.Value}\\{cultureName}";
                    LocalInvariantFilePath = $"{element.Value}\\{cultureName}";
                }

                element = (from item in doc.Descendants("WebPath")
                           select item).FirstOrDefault();
                if (element != null && !string.IsNullOrEmpty(element.Value))
                    WebFilePath = $"{element.Value}\\{cultureName}";
            }
        }
        
        internal string GetPathToOpen(string selected, bool local, bool useInvariant = false)
        {
            string ret = string.Empty;
            try
            {
                string filepath;
                
                InitRootPath();
                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;

                lock (lockfile)
                {
                    var doc = XElement.Load(String.Format("{0}.{1}\\HelpOnLine\\WebHelpList.xml", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),mainversion));

                    var stlist = (from item in doc.Descendants("ShowTopicName")
                                  select item).ToList();
                    showTopicName = stlist.Count > 0;

                    var element = (from item in doc.Descendants("DefaultPage")
                               select item).FirstOrDefault();
                    if (element != null)
                        DefaultPage = element.Value;

                    
                    if (local)
                    {
                        filepath = useInvariant ? LocalInvariantFilePath : LocalFilePath;
                    }
                    else
                    {
                        filepath = WebFilePath;
                    }
                    
                    

                    int i = selected.IndexOf("(");
                    if (i != -1)
                        selected = selected.Substring(0, i);
                    
                    var singleitem = (from item in doc.Descendants("Topic")
                                      where item.Attribute("Class").Value == selected
                                      select item).ToList();
                    if (singleitem.Count > 0)
                    {
                        if (local)
                            ret = string.Format("{0}.{3}\\{2}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), singleitem[0].Attribute("Path").Value, filepath,mainversion);
                        else
                            ret = string.Format("{0}\\{1}", filepath, singleitem[0].Attribute("Path").Value);
                    }
                    else
                    {
                        int j = selected.IndexOf(":");
                        if(j != -1)
                        { 
                            selected = selected.Substring(0,j);
                        var secondtime = (from item in doc.Descendants("Topic")
                                      where item.Attribute("Class").Value == selected
                                      select item).ToList();
                            if(secondtime.Count > 0)
                            {
                                if (local)
                                    ret = string.Format("{0}.{3}\\{2}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), secondtime[0].Attribute("Path").Value, filepath,mainversion);
                                else
                                    ret = string.Format("{0}\\{1}", filepath, secondtime[0].Attribute("Path").Value);
                            }
                        }
                        else if (!showTopicName)
                        {
                            var defitem = (from item in doc.Descendants("Topic")
                                           where item.Attribute("Class").Value == "" && item.Attribute("Scope").Value == "" && item.Attribute("Parent").Value == ""
                                           select item).ToList();
                            if (defitem.Count > 0)
                            {
                                if (local)
                                    ret = string.Format("{0}.{3}\\{2}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), defitem[0].Attribute("Path").Value, filepath,mainversion);
                                else
                                    ret = string.Format("{0}\\{1}", filepath, defitem[0].Attribute("Path").Value);
                            }
                        }
                    }
                }
                //Ultima spe
                if (ret.Length == 0 && !showTopicName)
                {
                    if (DefaultPage.Length > 0)
                    {
                        if (local)
                            ret = string.Format("{0}.{3}\\{2}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), DefaultPage, filepath,mainversion);
                        else
                            ret = string.Format("{0}\\{1}", filepath, DefaultPage);
                    }
                    else
                    {
                        ret = filepath;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.ErrorFillingHelpLinks, ex);
            }

            return ret;
        }

        internal void OpenHelp(Uri uri)
        {
            if (uri.IsFile && !File.Exists(uri.LocalPath))
            {
                //default local
                uri = new Uri(string.Format("{0}\\{2}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), DefaultPage, LocalFilePath));
                if (uri.IsFile && !File.Exists(uri.LocalPath))
                {
                    //default web
                    uri = new Uri(string.Format("{0}\\{1}", WebFilePath, DefaultPage));
                }
            }
            /* Valutare...
            if (helpWindow == null)
                helpWindow = new SHDocVw.InternetExplorer();

            helpWindow.Visible = true;
            helpWindow.Navigate(uri.AbsoluteUri);
            return;
            */

            String iePath = null;
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\IEXPLORE.EXE");
                if (key != null)
                {
                    Object n = key.GetValue("");
                    if (n != null)
                        iePath = n.ToString();
                    else
                    {
                        Object o = key.GetValue("Path");
                        if (o != null)
                        {
                            iePath = string.Format("{0}\\iexplore.exe", o.ToString());
                        }
                    }

                    var pr = System.Diagnostics.Process.Start(string.Format("\"{0}\"", iePath), string.Format("\"{0}\"", uri.AbsoluteUri));
                }
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                string s = ex.Message;
            }
        }

        bool showTopicName = false;

        internal bool ShowDialogHelp(Uri dest, bool newwin = false)
        {
            try
            {
                try
                {
                    Process.Start(dest.AbsolutePath);
                }
                catch (Exception)
                {
                    try
                    {
                        Process.Start(dest.AbsoluteUri);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        internal bool OpenDialogUri(string prop, bool newwin = false)
        {
            try
            {
                string hfile = GetPathToOpen(prop, LocalHelp);
                if (hfile.Length > 0)
                {
                    if (!ShowDialogHelp(new Uri(hfile), newwin))
                    {
                        if (LocalHelp)
                        {
                            if (helpProviderComponent.UIInterface.ShowOkCancel(Properties.Resources.ErrorReadingLocalHelpTopic, UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Warning) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.OK)
                                ShoOnlineHelp(prop, newwin);
                        }
                        else
                            ShoOnlineHelp(prop, newwin);
                    }
                }
                else if (showTopicName)
                {
                    var hdt = new HelpProviderReader();
                    hdt.ClearValue(FrameworkElement.WidthProperty);
                    hdt.ClearValue(FrameworkElement.HeightProperty);

                    hdt.LocalFilePath = LocalFilePath;
                    hdt.WebFilePath = WebFilePath;
                    hdt.LocalInvariantFilePath = LocalInvariantFilePath;
                    hdt.DefaultPage = DefaultPage;

                    hdt.ShowTopicName(prop);
                    HelpWindow hw = new HelpWindow(hdt) { Title = Properties.Resources.PopUpTitle };
                    hw.ShowDialog();
                    hw.Close();
                }
                else
                    helpProviderComponent.UIInterface.ShowWarning(Properties.Resources.GenericErrorReadingHelp);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        void ShoOnlineHelp(string selectedObject, bool newwin)
        {
            string destination = GetPathToOpen(selectedObject, false);
            if (destination.Length > 0)
                ShowDialogHelp(new Uri(destination), newwin);
            else
                helpProviderComponent.UIInterface.ShowWarning(Properties.Resources.GenericErrorReadingHelp);
        }

        internal void ExecuteHelp()
        {
            var selectedObject = helpProviderComponent.GetSelectedObject();
            Trace.TraceInformation(string.Format("ExecuteHelp: {0}", selectedObject));

            if (selectedObject == null)
                selectedObject = string.Empty;

            OpenDialogUri(selectedObject, false);
        }

        private void CanExecuteHelp(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnUseLocalHelp(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            LocalHelp = !LocalHelp;
        }

        internal void ShowAboutBox()
        {
            var parent = this.FindParent<Window>();
            var box = new HelpProvider.Controls.AboutProduct();

            var title = ApplicationPropertiesHelper.GetProperty("MainWindowTitle") as String;

            var dialog = new Utilities.GeneralDialogContent(box,GeneralDialogButtons.OkButton)
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = string.Format(Properties.Resources.AboutTitle, title)
            };
            dialog.ShowDialog();
        }

        void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnExecuteHelp(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteHelp();
        }

        private void OnAboutBox(object sender, ExecutedRoutedEventArgs e)
        {
            ShowAboutBox();
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
    }
}
