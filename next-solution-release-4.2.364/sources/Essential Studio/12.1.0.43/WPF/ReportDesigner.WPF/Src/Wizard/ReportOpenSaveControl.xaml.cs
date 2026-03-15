#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Reports;
using System.Windows.Controls.Primitives;
using System.IO;
using Syncfusion.Windows.Reports.Designer.Wizard;
using Syncfusion.RDL.ServerProcessor;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Wizard
{
    /// <summary>
    /// Interaction logic for ReportServerOpenDialog.xaml
    /// </summary>
    /// 
    #region Control Property Enums

    public enum DialogMode
    {
        Default,
        Local,
        Server
    }

    public enum DialogFileType
    {
        RDL,
        RDLC,
        DataSource
    }

    internal enum DialogType
    {
        Open,
        Save
    }

    #endregion

    internal partial class ReportOpenSaveControl : UserControl
    {    
        #region Variables

        private Window window;

        private ServerReportProcessor reportService;

        //variable for add list of Dir or files
        private List<Listdir> listOfFolderInfo;

        //Recent Files
        private List<string> recentfiles;

        private string ssrspath = string.Empty;

        private bool isReportingServer = true;

        private bool isFile = false;

        private bool isLocalMode = true;

        private bool isBackProcess = true;

        private string servercurrenturl = string.Empty;

        //variable for return currten directory
        private static string currentDir = string.Empty;

        //constant variable check or add text in comobox
        private const string myComputer = "My Computer";

        private const string recentsites = "Recent Servers";

        private string filterFormat = string.Empty;

        int _searchIndex = 0;

        string _oldChar = string.Empty;

        #endregion

        #region Property

        //File Format Filtering
        internal static readonly DependencyProperty FileTypeproperty;

        internal DialogFileType FileType
        {
            get
            {
                return (DialogFileType)this.GetValue(FileTypeproperty);
            }
            set
            {
                this.SetValue(FileTypeproperty, value);
            }
        }

        //File Path
        internal static readonly DependencyProperty FilePathproperty;

        internal string FilePath
        {
            get
            {
                return (string)this.GetValue(FilePathproperty);
            }
            set
            {
                this.SetValue(FilePathproperty, value);
            }
        }

        //Server url
        internal static readonly DependencyProperty ReportServerURLProperty;

        internal string ReportServerURL
        {
            get
            {
                return (string)this.GetValue(ReportServerURLProperty);
            }
            private set
            {
                this.SetValue(ReportServerURLProperty, value);
            }
        }

        //Crentical
        internal ICredentials ReportServerCredential { get; private set; }

        //Dialogmodeproperty 
        internal static readonly DependencyProperty DialogModeProperty;

        internal DialogMode DialogMode
        {
            get
            {
                return (DialogMode)this.GetValue(DialogModeProperty);
            }
            set
            {
                this.SetValue(DialogModeProperty, value);
            }
        }

        //Dialog Type
        internal static readonly DependencyProperty DialogTypeProperty;

        internal DialogType DialogType
        {
            get
            {
                return (DialogType)this.GetValue(DialogTypeProperty);
            }
            set
            {
                this.SetValue(DialogTypeProperty, value);

            }
        }

        public string LoginUsername { get; set; }

        public string LoginPassword { get; set; }

        #endregion

        #region Device Identify enums

        //uniqully identify each logical drives
        enum LogicalDevice
        {
            Fixed,
            CDRom,
            Removable
        };

        enum TypeofFile
        {
            File = 1,
            Folder,
            ServerReport,
            ServerFolder,
            ServerUrl,
            ServerDataSource,
            ServerSite,
            Device
        };

        #endregion

        #region Constructor

        static ReportOpenSaveControl()
        {
            DialogModeProperty = DependencyProperty.Register("DialogMode", typeof(DialogMode), typeof(ReportOpenSaveControl), new System.Windows.PropertyMetadata(DialogMode.Local));
            FileTypeproperty = DependencyProperty.Register("FileType", typeof(DialogFileType), typeof(ReportOpenSaveControl), new PropertyMetadata(DialogFileType.RDL));
            FilePathproperty = DependencyProperty.Register("FilePath", typeof(string), typeof(ReportOpenSaveControl), new PropertyMetadata(string.Empty));
            DialogTypeProperty = DependencyProperty.Register("DialogType", typeof(DialogType), typeof(ReportOpenSaveControl), new PropertyMetadata(DialogType.Open));
            ReportServerURLProperty = DependencyProperty.Register("ReportServerURL", typeof(string), typeof(ReportOpenSaveControl), new PropertyMetadata(string.Empty));
        }

        public ReportOpenSaveControl()
        {
            InitializeComponent();

            recentfiles = new List<string>();
            listOfFolderInfo = new List<Listdir>();

          //  reportService = new ServerReportProcessor();           

            this.Loaded += (sen, arg) =>
                {
                    this.window = Window.GetWindow(this);
                };
        }

        #endregion

        #region recentserver methods

        private void RecentServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (reportService == null)
                    reportService = new ServerReportProcessor();

                addressBar.Items.Clear();
                AddRecentDirectory(recentsites);
                listOfFolderInfo = GetRecentUrl();
                folderArea.ItemsSource = listOfFolderInfo;
                isReportingServer = false;
                isFile = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private List<Listdir> GetRecentUrl()
        {
            List<Listdir> recentUrls = new List<Listdir>();
            List<string> recentTempUrl = recentfiles.Distinct().ToList();

            foreach (string str in recentTempUrl)
            {
                recentUrls.Add(new Listdir(@"/Syncfusion.ReportDesigner.WPF;component/Images/server.jpg", str, TypeofFile.ServerUrl.ToString()));
            }

            return recentUrls;
        }

        #endregion

        #region open reportingserver file method

        private void SSRSOpen(string serverrecenturl)
        {
            try
            {

                folderArea.IsEnabled = false;
                reportService.ReportServerUrl = serverrecenturl;
                reportService.ReportServerCredential = System.Net.CredentialCache.DefaultCredentials;
                GetRecentServerFiles(serverrecenturl, reportService.ReportServerCredential);
            }
            catch
            {
                if (!CheckCredential(serverrecenturl))
                {
                    isReportingServer = false;
                    btnBack.IsEnabled = true;
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxAccesDenied"), SR.GetString(CultureInfo.CurrentUICulture, "titleWarning"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                folderArea.IsEnabled = true;
            }
        }

        private void GetRecentServerFiles(string serverrecenturl, ICredentials servercredential)
        {
            string strtemppath = "/";
            this.ReportServerCredential = servercredential;
            listOfFolderInfo = GetServerFiles(strtemppath);
            folderArea.ItemsSource = listOfFolderInfo;
            addressBar.Items.Clear();
            AddRecentDirectory(searchName.Text);
            isReportingServer = true;
            btnBack.IsEnabled = false;
            servercurrenturl = serverrecenturl;
            recentfiles.Add(servercurrenturl);
            searchName.Text = string.Empty;
        }

        private bool CheckCredential(string serverrecenturl)
        {
            try
            {
                if (reportService == null)
                    reportService = new ServerReportProcessor();
                LoginCredentials login = new LoginCredentials(serverrecenturl);
                login.Owner = this.window;
                SkinStorage.SetVisualStyle(login, SkinStorage.GetVisualStyle(login.Owner));

                if (login.ShowDialog() == true)
                {
                    LoginUsername = login.Username;
                    LoginPassword = login.Password;
                    reportService.ReportServerCredential = new NetworkCredential(login.Username, login.Password);
                    reportService.ReportServerUrl = serverrecenturl;
                    GetRecentServerFiles(serverrecenturl, reportService.ReportServerCredential);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //get server folders
        private List<Listdir> GetServerFiles(string strpath)
        {
            ssrspath = strpath;
            btnBack.IsEnabled = true;
            List<Listdir> ltserfl = new List<Listdir>();

            List<CatalogItem> items;
            bool isExecuted = reportService.GetCatalogItem(strpath, out items);

            if (isExecuted)
            {
                foreach (CatalogItem ci in items)
                {
                    if (ci.Type == ItemTypeEnum.Folder)
                    {
                        ltserfl.Add(new Listdir(@"/Syncfusion.ReportDesigner.WPF;component/Images/Folder.jpg", ci.Name, TypeofFile.ServerFolder.ToString()));
                    }
                    if (ci.Type == ItemTypeEnum.Report && FileType == DialogFileType.RDL)
                    {
                        ltserfl.Add(new Listdir(@"/Syncfusion.ReportDesigner.WPF;component/Images/rdl.jpg", ci.Name, TypeofFile.ServerReport.ToString()));
                    }
                    if (ci.Type == ItemTypeEnum.DataSource && FileType == DialogFileType.DataSource)
                    {
                        ltserfl.Add(new Listdir(@"/Syncfusion.ReportDesigner.WPF;component/Images/Network Datasource.jpg", ci.Name, TypeofFile.ServerDataSource.ToString()));
                    }
                    if (ci.Type == ItemTypeEnum.Site)
                    {
                        ltserfl.Add(new Listdir(@"/Syncfusion.ReportDesigner.WPF;component/Images/WebSiteicon.png", ci.Name, TypeofFile.ServerSite.ToString()));
                    }
                }
                return ltserfl;
            }
            else
            {
                throw new Exception("Could not retrive folder from provided path/reportserver");
            }
        }

        //get serverfolder parent path
        private string GetParentFolder(string strpath)
        {
            string ssrpath = string.Empty;
            int len = strpath.Length - 1;
            for (int i = len; i >= 0; i--)
            {
                if (strpath[i] == '/')
                {
                    ssrpath = strpath;
                    break;
                }
                else
                {
                    strpath = strpath.Remove(strpath.Length - 1, 1);
                }
            }
            return ssrpath;
        }


        #endregion

        private void Desktop_Click(object sender, RoutedEventArgs e)
        {
            isReportingServer = false;
            isFile = false;
            addressBar.Items.Clear();
            string startPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            listOfFolderInfo = GetFolders(startPath);
            folderArea.ItemsSource = listOfFolderInfo;
            AddRecentDirectory(startPath);
        }

        private void MyDocuments_Click(object sender, RoutedEventArgs e)
        {
            isReportingServer = false;
            isFile = false;
            addressBar.Items.Clear();
            string startPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            listOfFolderInfo = GetFolders(startPath);
            folderArea.ItemsSource = listOfFolderInfo;
            AddRecentDirectory(startPath);
        }

        private void MyComputer_Click(object sender, RoutedEventArgs e)
        {
            isReportingServer = false;
            isFile = false;
            addressBar.Items.Clear();
            listOfFolderInfo = GetLogicalDrives();
            folderArea.ItemsSource = listOfFolderInfo;
            AddRecentDirectory(myComputer);
        }

        //Window Load
        internal void UpdateReportDialog()
        {
            bool isCheckServerCon = true;
            filterFormat = FileType.ToString();
            searchType.Items.Add("Report Files(*." + filterFormat + ")");

            if (this.DialogType == Wizard.DialogType.Open)
            {
                btntype.Content = "Open";
            }
            else if (this.DialogType == Wizard.DialogType.Save)
            {
                btntype.Content = "Save";
                searchName.Text = "Untitled." + FileType.ToString().ToLower();
            }
            if (FileType == DialogFileType.RDLC || DialogMode.Local == DialogMode)
            {
                clklblrecenturl.Visibility = Visibility.Collapsed;
                isLocalMode = false;
            }
            else if (DialogMode.Server == DialogMode)
            {
                isCheckServerCon = false;
                drivers.Visibility = Visibility.Collapsed;
                RecentServer_Click(null, null);
            }
            if (isCheckServerCon)
            {
                MyDocuments_Click(null, null);
            }
            searchType.SelectedIndex = searchType.Items.Count - 1;
        }

        //select the current item from list
        private void SelectCurrentItem(object sender, MouseButtonEventArgs me)
        {
            string strAccessDenied = string.Empty;
            isFile = false;

            try
            {
                string strCon = string.Empty;
                string temppath = listOfFolderInfo[folderArea.SelectedIndex].FileName;
                string tempType = listOfFolderInfo[folderArea.SelectedIndex].FolderType;

                if (tempType == TypeofFile.Folder.ToString())
                {
                    strAccessDenied = currentDir;
                    if (currentDir.Length == 3 && currentDir.Contains(":") && currentDir.EndsWith("\\"))
                    {
                        currentDir = currentDir.Replace("\\", "");
                    }
                    
                    strCon = currentDir + "\\" + temppath;
                    Inselected(strCon);
                }
                else if (tempType == TypeofFile.Device.ToString())
                {
                    strCon = temppath;
                    Inselected(strCon);
                }
                else if (tempType == TypeofFile.File.ToString())
                {
                    if (this.DialogType == Wizard.DialogType.Save)
                    {
                        searchName.Text = temppath;
                    }
                    else
                    {
                        searchName.Text = currentDir + "\\" + temppath;
                    }
                    OpenSaveClick(sender, me);
                }
                else if (tempType == TypeofFile.ServerReport.ToString() && isReportingServer)
                {
                    isFile = true;
                    if (ssrspath.Length == 1)
                    {
                        searchName.Text = ssrspath + temppath;
                    }
                    else
                    {
                        searchName.Text = ssrspath + "/" + temppath;
                    }
                    OpenSaveClick(sender, me);
                }
                else if (tempType == TypeofFile.ServerFolder.ToString() && isReportingServer)
                {
                    listOfFolderInfo = GetServerFiles(ssrspath + temppath);
                    folderArea.ItemsSource = listOfFolderInfo;
                    AddRecentDirectory(servercurrenturl + ssrspath);
                }
                else if (tempType == TypeofFile.ServerDataSource.ToString() && isReportingServer)
                {

                }
                else if (tempType == TypeofFile.ServerUrl.ToString())
                {
                    addressBar.Items.Clear();
                    SSRSOpen(temppath);
                    AddRecentDirectory(temppath);
                }
                else if (tempType == TypeofFile.ServerSite.ToString())
                {
                    SSRSOpen(temppath);
                    AddRecentDirectory(temppath);
                }

            }
            catch
            {
                currentDir = strAccessDenied;
                RemoveRecentDirectory();
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxAccesDenied"), SR.GetString(CultureInfo.CurrentUICulture, "titleWarning"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FolderArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isFile = false;
            try
            {
                string temptype = listOfFolderInfo[folderArea.SelectedIndex].FolderType;
                string temppath = listOfFolderInfo[folderArea.SelectedIndex].FileName;

                if (temptype == TypeofFile.File.ToString())
                {
                    if (this.DialogType == Wizard.DialogType.Save)
                    {
                        searchName.Text = temppath;
                    }
                    else
                    {
                        if (currentDir.Length == 3 && currentDir.Contains(":") && currentDir.EndsWith("\\"))
                        {
                            currentDir = currentDir.Replace("\\", "");
                        }

                        searchName.Text = currentDir + "\\" + temppath;
                    }
                }
                else if (temptype == TypeofFile.ServerReport.ToString() && isReportingServer)
                {
                    isFile = true;
                    if (ssrspath.Length == 1)
                    {
                        searchName.Text = ssrspath + temppath;
                    }
                    else
                    {
                        searchName.Text = ssrspath + "/" + temppath;
                    }
                }
                else
                {
                    searchName.Text = "";
                }
            }
            catch
            {
            }
        }

        //go to the parent Directory, when Click
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (isReportingServer == true)
            {
                string strpath = GetParentFolder(ssrspath);
                listOfFolderInfo = GetServerFiles(strpath);
                folderArea.ItemsSource = listOfFolderInfo;
                RemoveRecentDirectory();
                AddRecentDirectory(strpath);
            }
            else
            {
                if (currentDir.Length == 3 && currentDir[2] == '\\')
                {
                    MyComputer_Click(sender, e);
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentDir))
                    {
                        DirectoryInfo currinfo = Directory.GetParent(currentDir);

                        string str = currinfo.ToString();

                        listOfFolderInfo = GetFolders(str);
                        folderArea.ItemsSource = listOfFolderInfo;
                        RemoveRecentDirectory();
                    }
                }
            }
        }

        //Select the folder from recent going list
        private void AddressBar_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                string strtemppath = addressBar.SelectedValue.ToString();
                if (!IsLocalPath(strtemppath))
                {
                    addressBar.Items.Clear();
                    SSRSOpen(strtemppath);
                    AddRecentDirectory(strtemppath);
                }
                else if (isReportingServer == true)
                {
                    listOfFolderInfo = GetServerFiles(strtemppath);
                    folderArea.ItemsSource = listOfFolderInfo;
                }
                else
                {
                    if (strtemppath == myComputer)
                    {
                        listOfFolderInfo = GetLogicalDrives();
                        folderArea.ItemsSource = listOfFolderInfo;
                    }
                    else if (strtemppath == recentsites)
                    {
                    }
                    else
                    {
                        listOfFolderInfo = GetFolders(strtemppath);
                        folderArea.ItemsSource = listOfFolderInfo;
                    }
                }
            }
            catch
            {
                RemoveRecentDirectory();
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxAccesDenied"), SR.GetString(CultureInfo.CurrentUICulture, "titleWarning"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //check file or folder
        private void SSRSCheck(string temFolderType, string temppath)
        {
            if (temFolderType == TypeofFile.ServerReport.ToString() && isReportingServer)
            {
                isFile = true;
                if (ssrspath.Length == 1)
                {
                    searchName.Text = ssrspath + temppath;
                }
                else
                {
                    searchName.Text = ssrspath + "/" + temppath;
                }
            }
            else if (temFolderType == TypeofFile.ServerReport.ToString() && isReportingServer)
            {
                listOfFolderInfo = GetServerFiles(ssrspath + temppath);
                folderArea.ItemsSource = listOfFolderInfo;
                AddRecentDirectory(ssrspath);
            }
            else if (temFolderType == TypeofFile.ServerDataSource.ToString() && isReportingServer)
            {
                isFile = true;
                if (ssrspath.Length == 1)
                {
                    searchName.Text = ssrspath + temppath;
                }
                else
                {
                    searchName.Text =ssrspath + "/" + temppath;
                }
            }
        }

        #region GetFolders and Files method

        //Get list of files or folders
        private List<Listdir> GetFolders(string strpath)
        {
            currentDir = strpath;

            List<Listdir> ltfol = new List<Listdir>();

            string[] oDirectories = Directory.GetDirectories(strpath);

            btnBack.IsEnabled = true;

            foreach (string oCurrent in oDirectories)
            {
                ltfol.Add(new Listdir(@"/Syncfusion.ReportDesigner.WPF;component/Images/Folder.jpg", System.IO.Path.GetFileName(oCurrent), TypeofFile.Folder.ToString()));
            }

            string[] rdlfiles = Directory.GetFiles(strpath, "*." + filterFormat.ToLower());

            foreach (string rdlfil in rdlfiles)
            {
                ltfol.Add(new Listdir(@"/Syncfusion.ReportDesigner.WPF;component/Images/rdl.jpg", System.IO.Path.GetFileName(rdlfil), TypeofFile.File.ToString()));
            }

            return ltfol;
        }

        #endregion


        #region Get Logical Drives method

        //get Logical Drives
        private List<Listdir> GetLogicalDrives()
        {
            List<Listdir> ltdri = new List<Listdir>();

            string[] drives = System.IO.Directory.GetLogicalDrives();

            string temp = string.Empty;

            currentDir = string.Empty;
            btnBack.IsEnabled = false;
            DriveInfo drv;
            foreach (string str in drives)
            {
                drv = new DriveInfo(str);
                temp = drv.DriveType.ToString();
                if (temp == LogicalDevice.Fixed.ToString())
                {
                    ltdri.Add(new Listdir(@"/Syncfusion.ReportDesigner.WPF;component/Images/Hard_Disk.jpg", str, TypeofFile.Device.ToString()));
                }
                else if (temp == LogicalDevice.CDRom.ToString())
                {
                    ltdri.Add(new Listdir(@"/Syncfusion.ReportDesigner.WPF;component/Images/cdrom.jpg", str, TypeofFile.Device.ToString()));
                }
                else if (temp == LogicalDevice.Removable.ToString())
                {
                    ltdri.Add(new Listdir(@"/Syncfusion.ReportDesigner.WPF;component/Images/usb.jpg", str, TypeofFile.Device.ToString()));
                }
            }
            return ltdri;
        }

        #endregion

        //add recent directory path in comobox
        private void AddRecentDirectory(string Dir_name)
        {
            if (!addressBar.Items.Contains(Dir_name))
            {
                addressBar.Items.Add(Dir_name);
            }
            addressBar.SelectedIndex = addressBar.Items.Count - 1;
        }

        //remove recent directory path in comobox
        private void RemoveRecentDirectory()
        {
            addressBar.Items.Remove(addressBar.SelectedValue);
            AddRecentDirectory(currentDir);
        }

        private void OpenSaveClick(object sender, RoutedEventArgs e)
        {          
            if (searchName.Text.Trim().Equals(""))
            {
                if (folderArea.SelectedIndex >= 0 && (!isReportingServer))
                {
                    string temppath = currentDir + "\\" + listOfFolderInfo[folderArea.SelectedIndex].FileName;
                    if (IsFolder(temppath))
                    {
                        listOfFolderInfo = GetFolders(temppath);
                        folderArea.ItemsSource = listOfFolderInfo;
                        AddRecentDirectory(temppath);
                        isReportingServer = false;
                    }
                    else
                    {
                        FilePath = temppath;
                        this.window.DialogResult = true;
                        this.window.Close();
                    }
                }
                else if (isReportingServer)
                {
                    if (folderArea.SelectedIndex >= 0)
                    {
                        string temppath = listOfFolderInfo[folderArea.SelectedIndex].FileName;
                        string temFolderType = listOfFolderInfo[folderArea.SelectedIndex].FolderType;
                        SSRSCheck(temFolderType, temppath);
                    }
                }
            }
            else if (!IsLocalPath(searchName.Text))
            {
                if (isLocalMode)
                {
                    SSRSOpen(searchName.Text);
                }
            }
            else if (isFile && isLocalMode)
            {
                ReportServerURL = servercurrenturl;
                FilePath = searchName.Text;

                if (this.DialogType == Wizard.DialogType.Save)
                {
                    if (this.ValidateFileReplace(searchName.Text.Replace('/', ' ').Trim(), true))
                    {
                        this.window.DialogResult = true;
                        this.window.Close();
                    }
                }
                else
                {
                    this.window.DialogResult = true;
                    this.window.Close();
                }
            }
            else if (isReportingServer && this.DialogType == Wizard.DialogType.Save)
            {
                ReportServerURL = servercurrenturl;
                if (ssrspath.Length == 1)
                {
                    FilePath = ssrspath + searchName.Text;
                }
                else
                {
                    FilePath = ssrspath + "/" + searchName.Text;
                }

                if (this.DialogType == Wizard.DialogType.Save)
                {
                    if (this.ValidateFileReplace(searchName.Text.Replace('/', ' ').Trim(), true))
                    {
                        this.window.DialogResult = true;
                        this.window.Close();
                    }
                }
                else
                {
                    this.window.DialogResult = true;
                    this.window.Close();
                }
            }
            else if (File.Exists(searchName.Text) && (this.DialogMode != DialogMode.Server))
            {
                if (this.DialogType == Wizard.DialogType.Save)
                {
                    if (ValidateFileReplace(searchName.Text, false))
                    {
                        SetPath(searchName.Text, false);
                    }
                }
                else
                {
                    SetPath(searchName.Text, false);
                }
            }
            else if (this.DialogType == Wizard.DialogType.Save)
            {
                if (File.Exists(System.IO.Path.Combine(currentDir, searchName.Text)))
                {
                    if (ValidateFileReplace(searchName.Text, false))
                    {
                        SetPath(currentDir + "\\" + searchName.Text, true);
                    }
                }
                else
                {
                    SetPath(currentDir + "\\" + searchName.Text, true);
                }
            }
            else if (Directory.Exists(searchName.Text) && (this.DialogMode != DialogMode.Server))
            {
                listOfFolderInfo = GetFolders(searchName.Text);
                folderArea.ItemsSource = listOfFolderInfo;
                AddRecentDirectory(searchName.Text);
                searchName.Text = "";
                isReportingServer = false;
            }
        }

        private bool ValidateFileReplace(string fileName, bool isServer)
        {
            MessageBoxResult result;

            if (isServer)
            {
                if (listOfFolderInfo.SingleOrDefault(file => file.FileName.Equals(fileName)) != null)
                {
                    result = MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCtrName") + fileName + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxExist") + "\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxReplace"),
                              SR.GetString(CultureInfo.CurrentUICulture, "titleSaveAs"), MessageBoxButton.YesNo, MessageBoxImage.Question);

                    return result == MessageBoxResult.Yes;
                }
            }
            else
            {
                result = MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCtrName") + fileName + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxExist") + "\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxReplace"),
                              SR.GetString(CultureInfo.CurrentUICulture, "titleSaveAs"), MessageBoxButton.YesNo, MessageBoxImage.Question);

                return result == MessageBoxResult.Yes;
            }

            return true;
        }

        private void SetPath(string strpath, bool ischeck)
        {
            string ext = System.IO.Path.GetExtension(strpath);
            if (ext == ".rdl" || ext == ".rdlc")
            {
                FilePath = strpath;
                this.window.DialogResult = true;
                this.window.Close();
            }
            else if (ischeck)
            {
                FilePath = strpath + "." + filterFormat.ToLower();
                this.window.DialogResult = true;
                this.window.Close();
            }
        }

        private void Inselected(string strCon)
        {
            listOfFolderInfo = GetFolders(strCon);
            AddRecentDirectory(strCon);
            folderArea.ItemsSource = listOfFolderInfo;
        }

        private static bool IsLocalPath(string url)
        {
            try
            {
                return new Uri(url).IsFile;
            }
            catch
            {
                return true;
            }
        }

        public bool IsFolder(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.window.DialogResult = false;
            this.window.Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back && isBackProcess)
            {
                Back_Click(sender, e);
            }
            else if (e.Key == Key.Enter)
            {
                string isAccessDenied = string.Empty;
                try
                {
                    isAccessDenied = currentDir;
                    OpenSaveClick(sender, e);
                }
                catch
                {
                    currentDir = isAccessDenied;
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxAccesDenied"), SR.GetString(CultureInfo.CurrentUICulture, "titleWarning"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void searchName_GotFocus(object sender, RoutedEventArgs e)
        {
            isBackProcess = false;
        }

        private void searchName_LostFocus(object sender, RoutedEventArgs e)
        {
            isBackProcess = true;
        }

        private void folderArea_KeyDown(object sender, KeyEventArgs e)
        {
           // this.folderArea.Items
           string  _searchString = string.Empty;
            _searchString += e.Key.ToString().ToLower();

            if (_searchString.Length > 1)
            {
               _searchString = _searchString.Remove(0, 1);
            }

            var itemcount = (from items in folderArea.Items.OfType<Syncfusion.Windows.Reports.Designer.Wizard.Listdir>() where items.FileName.ToLower().StartsWith(_searchString) select items).ToList();

            if (itemcount.Count == _searchIndex || _oldChar!=_searchString)
            {
                _searchIndex = 0;
            }

            for (int i = _searchIndex; i < itemcount.Count; )
            {
                folderArea.SelectedItem = itemcount[i];
                folderArea.ScrollIntoView(itemcount[i]);
                _oldChar = _searchString;
                _searchIndex = i + 1;
                break;
            }
        }
    }

    internal class Listdir
    {
        public string Image { get; set; }

        public string FileName { get; set; }

        public string FolderType { get; set; }

        public Listdir()
        {
        }

        public Listdir(string Image, string FileName, string FolderType)
        {
            this.Image = Image;
            this.FileName = FileName;
            this.FolderType = FolderType;
        }
    }

    internal class ClickableLabel : Label
    {
        public static readonly RoutedEvent ClickEvent;

        static ClickableLabel()
        {
            ClickEvent = ButtonBase.ClickEvent.AddOwner(typeof(ClickableLabel));
        }

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            CaptureMouse();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
                if (IsMouseOver)
                {
                    RaiseEvent(new RoutedEventArgs(ClickEvent, this));
                }
            }
        }
    }
}

