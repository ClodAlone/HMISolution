#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.IO;
using System.Net;
using Microsoft.Win32;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Microsoft.SqlServer.ReportingServices2005;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for SelectReport.xaml
    /// </summary>
    internal partial class SelectReport : ChromelessWindow
    {
        public SelectReport(SubReportGeneral reportGeneral)
        {
            try
            {
                InitializeComponent();
                this.Owner = Window.GetWindow(reportGeneral);
                this.Ltbx_Contents.SelectionChanged += new SelectionChangedEventHandler(Ltbx_Contents_SelectionChanged);
                this.Ltbx_Contents.MouseDoubleClick += new MouseButtonEventHandler(Ltbx_Contents_MouseDoubleClick);
                this.cmb_TypeItems.SelectionChanged += new SelectionChangedEventHandler(cmb_TypeItems_SelectionChanged);
                this.cmb_SitesAndServer.SelectionChanged += new SelectionChangedEventHandler(cmb_SitesAndServer_SelectionChanged);
                this.KeyDown += new KeyEventHandler(SelectReport_KeyUp);
                if (this.cmb_SitesAndServer != null)
                {
                    if (this.cmb_SitesAndServer.Items.Count == 0)
                    {
                        AddToSitesCmb("Recent Sites and Servers");
                        this.cmb_SitesAndServer.SelectedIndex = 0;
                    }
                }
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        void SelectReport_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (this.Ltbx_Contents.SelectedItem != null || this.Ltbx_Contents.IsEnabled == false)
                {
                    if (this.btn_Back.IsEnabled == true)
                    {
                        GoBackClicked();
                    }
                }
            }

            if (this.Ltbx_Contents.SelectedItem != null)
            {
                int i = this.Ltbx_Contents.SelectedIndex + 1;

                do
                {
                    if (i >= this.Ltbx_Contents.Items.Count)
                    {
                        i = 0;
                    }

                    Grid grid = this.Ltbx_Contents.Items[i] as Grid;
                    Label label = grid.Children[1] as Label;

                    if (((e.Key.ToString().Substring(0, 1)) == "D") && (e.Key.ToString().Length == 2))
                    {
                        if (e.Key.ToString().Equals( "D" + label.Content.ToString().Substring(0, 1), StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.Ltbx_Contents.SelectedIndex = i;
                            this.Ltbx_Contents.ScrollIntoView(this.Ltbx_Contents.SelectedItem);
                            e.Handled = true;
                            break;
                        }
                    }

                    if (e.Key.ToString().Equals(label.Content.ToString().Substring(0, 1), StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.Ltbx_Contents.SelectedIndex = i;
                        this.Ltbx_Contents.ScrollIntoView(this.Ltbx_Contents.SelectedItem);
                        e.Handled = true;
                        break;
                    }
                    i++;
                }
                while (this.Ltbx_Contents.SelectedIndex != i - 1);
            }
        }

        private void AddToSitesCmb(string newAddress)
        {
            newAddress = newAddress.Replace("_", "__");
            Grid gridSample = new Grid();
            RowDefinition r1 = new RowDefinition();
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            c1.Width = new GridLength();
            c2.Width = new GridLength();
            gridSample.RowDefinitions.Add(r1);
            gridSample.ColumnDefinitions.Add(c1);
            gridSample.ColumnDefinitions.Add(c2);
            Image img = new Image();
            Label label = new Label();
            label.Content = newAddress;

            if (cmb_SitesAndServer.Items.Count == 0)
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/OpenFolder.png", UriKind.RelativeOrAbsolute));
                img.Name = "Folder";
                Grid.SetColumn(img, 0);
                gridSample.Children.Add(img);
            }
            else
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/SiteOpened.png", UriKind.RelativeOrAbsolute));
                img.Name = "Folder";
                Grid.SetColumn(img, 0);
                gridSample.Children.Add(img);
            }

            Grid.SetColumn(label, 1);
            gridSample.Children.Add(label);
            this.cmb_SitesAndServer.Items.Add(gridSample);
        }

        private void UpdateRecentSites()
        {
            this.Ltbx_Contents.Items.Clear();
            RegistryKey HKLM = Registry.LocalMachine;
            try
            {
                HKLM = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Syncfusion\\Essential Suite\\InstalledVersions\\Reports\\ServerName", RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
                if (HKLM != null)
                {
                    string[] serverNameArray = HKLM.GetValueNames();
                    int count = HKLM.ValueCount;
                    foreach (string sKey in serverNameArray)
                    {
                        if (this.Ltbx_Contents != null && this.Ltbx_Contents.Items.Count != 0)
                        {
                            int j = this.Ltbx_Contents.Items.Count;
                            for (int i = 0; i < j; i++)
                            {
                                if (HKLM.GetValue(sKey).ToString() != this.Ltbx_Contents.Items[i].ToString())
                                {
                                    AddToListBox(HKLM.GetValue(sKey).ToString(), "Link");
                                }
                            }
                        }
                        else
                        {
                            AddToListBox(HKLM.GetValue(sKey).ToString(), "Link");
                        }
                    }
                }
                else
                {
                    if (this.givenFileName != null && this.givenFileName != string.Empty)
                    {
                        AddToListBox(this.givenFileName, "Link");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string selectedFileName { get; set; }

        public string givenFileName { get; set; }

        public string fileName = string.Empty;

        public string siteChanged = string.Empty;

        HttpWebRequest request;
        HttpWebResponse response;
        Uri uriObj;
        CatalogItem[] items;

        void Ltbx_Contents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileSelected();
        }

        void cmb_SitesAndServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            if ((this.cmb_SitesAndServer.SelectedItem != null) && (this.cmb_SitesAndServer.SelectedIndex != 0) && (siteChanged != string.Empty))
            {
                this.btn_Back.IsEnabled = true;
                readFiles("File type changed");
            }
            else
            {
                this.btn_Back.IsEnabled = false;
            }

            if (this.cmb_SitesAndServer.SelectedItem != null)
            {
                int i = this.cmb_SitesAndServer.Items.Count;
                int j = this.cmb_SitesAndServer.SelectedIndex;

                if (j != (i - 1))
                {
                    do
                    {
                        this.cmb_SitesAndServer.Items.RemoveAt(i-1);
                        i--;
                    } while (i != j+1);

                    if (this.cmb_SitesAndServer.SelectedIndex !=0)
                    {
                        readFiles("File type changed");
                    }
                    else
                    {
                        this.Ltbx_Contents.Items.Clear();
                        AddToListBox(this.givenFileName, "Link");
                    }
                }
            }

            if (this.cmb_SitesAndServer.SelectedIndex == 0)
            {
                this.Ltbx_Contents.Items.Clear();
                this.Ltbx_Contents.IsEnabled = true;
                this.Ltbx_Contents.HorizontalContentAlignment = HorizontalAlignment.Left;
                UpdateRecentSites();
            }
        }

        void cmb_TypeItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmb_TypeItems.SelectedItem != null && this.cmb_SitesAndServer.SelectedIndex != 0)
            {
                if (this.cmb_TypeItems.SelectedIndex == 0)
                {
                    this.cmb_TypeItems.Text = "Reports(*.rdl)";
                }
                else
                {
                    this.cmb_TypeItems.Text = "All Items(*.*)";
                }
                string fileType = "File type changed";
                readFiles(fileType);
            }
        }

        private void FileSelected()
        {
            if (this.Ltbx_Contents.SelectedItem != null)
            {
                if (this.cmb_SitesAndServer.SelectedIndex != 0)
                {
                    Grid grid = this.Ltbx_Contents.SelectedItem as Grid;
                    Label lab = grid.Children[1] as Label;
                    Image imag = grid.Children[0] as Image;

                    if (this.givenFileName != null)
                    {
                        if (lab.Content.ToString().Replace("__", "_") != this.givenFileName)
                        {
                            string selectedItem = lab.Content.ToString().Replace("__", "_");
                            fileName = lab.Content.ToString().Replace("__", "_");

                            if (items != null)
                            {
                                if (imag != null)
                                {
                                    if (imag.Name == "Folder")
                                    {
                                        string newLink = ((this.cmb_SitesAndServer.SelectedItem as Grid).Children[1] as Label).Content.ToString().Replace("__", "_") + "/" + fileName;
                                        AddToSitesCmb(newLink);
                                        siteChanged = "Link changed";
                                        this.cmb_SitesAndServer.SelectedIndex = this.cmb_SitesAndServer.Items.Count - 1;
                                    }
                                    else
                                    {
                                        string s = ((this.cmb_SitesAndServer.SelectedItem as Grid).Children[1] as Label).Content.ToString().Replace("__", "_");
                                        if (this.givenFileName == s)
                                        {
                                            selectedFileName = "/" + fileName;
                                        }
                                        else
                                        {
                                            int p = s.Length - this.givenFileName.Length;
                                            selectedFileName = s.Substring(this.givenFileName.Length, p) + "/" + fileName;
                                        }
                                        this.DialogResult = true;
                                        this.Close();
                                    }
                                }
                                else
                                {
                                    string s = ((this.cmb_SitesAndServer.SelectedItem as Grid).Children[1] as Label).Content.ToString().Replace("__", "_");
                                    if (this.givenFileName == s)
                                    {
                                        selectedFileName = "/" + fileName;
                                    }
                                    else
                                    {
                                        selectedFileName = s.Substring(this.givenFileName.Length, (s.Length - this.givenFileName.Length)) + "/" + fileName;
                                    }
                                    this.DialogResult = true;
                                    this.Close();
                                }
                            }
                            else
                            {
                                this.Ltbx_Contents.SelectedItem = null;
                                SelectedOpen();
                            }
                        }
                        else
                        {
                            AddToSitesCmb(this.givenFileName);
                            siteChanged = "yes";
                            this.cmb_SitesAndServer.SelectedIndex = this.cmb_SitesAndServer.Items.Count - 1;
                        }
                    }
                    else
                    {
                        this.txt_FileName.Text = lab.Content.ToString().Replace("__", "_");
                        this.Ltbx_Contents.SelectedItem = null;
                        SelectedOpen();
                    }
                }
                else
                {
                    this.txt_FileName.Text = ((this.Ltbx_Contents.SelectedItem as Grid).Children[1] as Label).Content.ToString().Replace("__", "_");
                    this.Ltbx_Contents.SelectedItem = null;
                    SelectedOpen();
                }
            }
        }

        void Ltbx_Contents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Ltbx_Contents.SelectedItem != null)
            {
                Grid grid = this.Ltbx_Contents.SelectedItem as Grid;
                Label lab = grid.Children[1] as Label;
                Image image = grid.Children[0] as Image;
                string selectedItem = lab.Content.ToString().Replace("__", "_");

                if (items != null)
                {
                    if (image != null)
                    {
                        if (image.Name == "Folder")
                        {
                            this.txt_FileName.Text = string.Empty;
                        }
                        else
                        {
                            this.txt_FileName.Text = lab.Content.ToString().Replace("__", "_");
                        }
                    }
                    else
                    {
                        this.txt_FileName.Text = lab.Content.ToString().Replace("__", "_");
                    }
                }
                else
                {
                    this.txt_FileName.Text = lab.Content.ToString().Replace("__", "_");
                }
            }
        }

        private void btn_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedOpen();
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        private void SelectedOpen()
        {
            string fileOpen = string.Empty;
            string check = string.Empty;
            string sb = string.Empty;
            if (this.cmb_SitesAndServer.SelectedIndex == 0)
            {
                this.Ltbx_Contents.SelectedItem = null;
            }
            if (this.Ltbx_Contents.SelectedItem == null)
            {
                if (this.txt_FileName.Text != string.Empty)
                {
                    if (this.givenFileName != null)
                    {
                        if (this.txt_FileName.Text[this.txt_FileName.Text.Length - 1] == '/')
                        {
                            this.txt_FileName.Text = this.txt_FileName.Text.Substring(0, this.txt_FileName.Text.Length - 1);
                        }

                        if (this.txt_FileName.Text.Length >= this.givenFileName.Length)
                        {
                            if (this.givenFileName != this.txt_FileName.Text)
                            {
                                string sr = ((this.cmb_SitesAndServer.SelectedItem as Grid).Children[1] as Label).Content.ToString().Replace("__", "_");
                                if (this.cmb_SitesAndServer.SelectedIndex != 0)
                                {
                                    if (this.txt_FileName.Text.Length < sr.Length)
                                    {
                                        sb = sr.Substring(0, this.txt_FileName.Text.Length);
                                    }
                                    else
                                    {
                                        string s = ((this.cmb_SitesAndServer.Items[1] as Grid).Children[1] as Label).Content.ToString().Replace("__", "_");
                                        string srt = this.txt_FileName.Text.Substring(0, s.Length);
                                        if (s == srt)
                                        {
                                            string s1 = this.txt_FileName.Text.Substring(s.Length + 1, (this.txt_FileName.Text.Length - s.Length - 1));
                                            do
                                            {
                                                int a = s1.IndexOf("/");
                                                if (a > 0)
                                                {
                                                    string s2 = s1.Substring(0, a);
                                                    s = s + "/" + s2;
                                                    AddToSitesCmb(s);
                                                    this.cmb_SitesAndServer.SelectedIndex = this.cmb_SitesAndServer.Items.Count - 1;
                                                    s1 = s1.Substring(s2.Length + 1, (s1.Length - a - 1));
                                                }
                                            } while (s1.IndexOf("/") >= 0);

                                            if (items != null)
                                            {
                                                string s3 = s.Substring(srt.Length, (s.Length - srt.Length)) + "/" + s1;
                                                foreach (CatalogItem c in items)
                                                {
                                                    if ((c.Path == s3) && (c.Type == ItemTypeEnum.Folder))
                                                    {
                                                        AddToSitesCmb(srt + c.Path);
                                                        this.cmb_SitesAndServer.SelectedIndex = this.cmb_SitesAndServer.Items.Count - 1;
                                                        sb = srt + c.Path;
                                                    }
                                                    else if ((c.Path == s3) && (c.Type != ItemTypeEnum.Folder))
                                                    {
                                                        sb = srt + c.Path;
                                                        selectedFileName = c.Path;
                                                        this.DialogResult = true;
                                                        this.Close();
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                                else
                                {
                                    sb = sr;
                                }
                            }
                            else
                            {
                                sb = this.txt_FileName.Text;
                                if (this.cmb_SitesAndServer.Items.Count == 1)
                                {
                                    AddToSitesCmb(this.givenFileName);
                                    this.cmb_SitesAndServer.SelectedIndex = this.cmb_SitesAndServer.Items.Count - 1;
                                }
                            }
                        }
                    }
                    
                    
                    if (sb != this.txt_FileName.Text)
                    {
                        if (items != null)
                        {
                            foreach (CatalogItem c in items)
                            {
                                if (this.txt_FileName.Text == c.Name)
                                {
                                    fileOpen = "It is a file";
                                    check = this.givenFileName + "/" + c.Path;
                                }
                            }
                        }

                        if (fileOpen == string.Empty)
                        {
                            int e = this.txt_FileName.Text.Length;

                            if (this.txt_FileName.Text[e - 1] == '/')
                            {
                                this.txt_FileName.Text = this.txt_FileName.Text.Substring(0, e - 1);
                            }
                            string str = this.txt_FileName.Text;
                            this.Ltbx_Contents.Items.Clear();
                            this.Ltbx_Contents.HorizontalContentAlignment = HorizontalAlignment.Center;
                            this.Ltbx_Contents.Items.Add("Loading...");
                            this.IsEnabled = false;

                            if (str != string.Empty)
                            {
                                try
                                {
                                    uriObj = new Uri(str);
                                    request = (HttpWebRequest)WebRequest.CreateDefault(uriObj);
                                    response = (HttpWebResponse)request.GetResponse();
                                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxUnableToConnect") + str + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectingServerInfo"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
                                    ConditionFalse();
                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message == "The remote server returned an error: (401) Unauthorized.")
                                    {
                                        string str1 = string.Empty;
                                        if (str.Contains('<') || str.Contains('>') || str.Contains('|') || str.Contains('?') || str.Contains('*') || str.Contains('\\'))
                                        {
                                            str1 = "Invalid";
                                        }

                                        if (str1 == string.Empty)
                                        {
                                            ServerAuthentication authentication = new ServerAuthentication(str, this);

                                            if (authentication.ShowDialog() == true)
                                            {
                                                givenFileName = str;
                                                this.IsEnabled = true;
                                                RegistryKey HKLMS = Registry.LocalMachine;
                                                HKLMS = HKLMS.OpenSubKey("SOFTWARE\\Syncfusion\\Essential Suite\\InstalledVersions\\Reports\\ServerName");

                                                if (HKLMS == null)
                                                {
                                                    try
                                                    {
                                                        HKLMS = HKLMS.CreateSubKey("SOFTWARE\\Syncfusion\\Essential Suite\\InstalledVersions\\Reports\\ServerName");
                                                    }
                                                    catch
                                                    {
                                                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxUnableToSave"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Information);
                                                    }

                                                }

                                                if (HKLMS != null)
                                                {
                                                    string[] serverNameArray = HKLMS.GetValueNames();
                                                    int count = HKLMS.ValueCount;
                                                    string alreadyRegistered = string.Empty;
                                                    foreach (string sKey in serverNameArray)
                                                    {
                                                        if (HKLMS.GetValue(sKey).ToString() == givenFileName)
                                                        {
                                                            alreadyRegistered = "Yes";
                                                        }
                                                    }

                                                    if (alreadyRegistered == string.Empty)
                                                    {
                                                        HKLMS.SetValue((HKLMS.ValueCount + 1).ToString(), givenFileName);
                                                    }
                                                    HKLMS.Close();
                                                }

                                                
                                                items = authentication.Items;
                                                AddToSitesCmb(this.txt_FileName.Text);
                                                this.cmb_SitesAndServer.SelectedIndex = this.cmb_SitesAndServer.Items.Count - 1;
                                                this.txt_FileName.Text = authentication.ServerAddress;
                                                this.btn_Back.IsEnabled = true;
                                                readFiles(string.Empty);
                                            }
                                            else
                                            {
                                                ConditionFalse();
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxInvalidLink"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
                                            ConditionFalse();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxUnableToConnect") + str + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectingServerInfo"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
                                        ConditionFalse();
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxMentionUrl"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
                                ConditionFalse();
                            }
                        }
                        else
                        {
                            MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxUnableToOpen") + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxUnableToConnect") + this.txt_FileName.Text + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectingServerInfo"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
                            ConditionFalse();
                        }
                    }
                    else
                    {
                        int y = this.cmb_SitesAndServer.Items.Count;
                        for (int z = 0; z < y; z++)
                        {
                            if (sb == ((this.cmb_SitesAndServer.Items[z] as Grid).Children[1] as Label).Content.ToString().Replace("__", "_"))
                            {
                                this.cmb_SitesAndServer.SelectedIndex = z;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEmptyRequestInfo"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
                    ConditionFalse();
                }
            }
            else
            {
                FileSelected();
            }
        }

        private void ConditionFalse()
        {
            this.IsEnabled = true;
            this.Ltbx_Contents.HorizontalContentAlignment = HorizontalAlignment.Left;
            if (this.cmb_SitesAndServer.SelectedIndex == 0)
            {
                UpdateRecentSites();
            }
            else
            {
                readFiles("File type changed");
            }
        }

        private void readFiles(string recd)
        {
            this.Ltbx_Contents.IsEnabled = true;
            if (recd != string.Empty)
            {
                if (recd == "File type changed")
                {
                    string cmbTxt = ((this.cmb_SitesAndServer.SelectedItem as Grid).Children[1] as Label).Content.ToString().Replace("__", "_");
                    if (this.givenFileName != cmbTxt)
                    {
                        int k = cmbTxt.Length - this.givenFileName.Length;
                        recd = cmbTxt.Substring((this.givenFileName.Length + 1), (k - 1));
                    }
                    else
                    {
                        recd = string.Empty;
                    }
                }
            }

            this.Ltbx_Contents.HorizontalContentAlignment = HorizontalAlignment.Left;
            string ciName = string.Empty;
            this.Ltbx_Contents.Items.Clear();

            if (items != null)
            {
                foreach (CatalogItem ci in items)
                {
                    if (recd == string.Empty)
                    {
                        ciName = "/" + ci.Name;
                    }
                    else
                    {
                        ciName = "/" + recd + "/" + ci.Name;
                    }

                    if ((ci.Type == ItemTypeEnum.Folder) && (ciName == ci.Path))
                    {
                        AddToListBox(ci.Name, "Folder");
                    }
                }

                foreach (CatalogItem ci in items)
                {
                    if (recd == string.Empty)
                    {
                        ciName = "/" + ci.Name;
                    }
                    else
                    {
                        ciName = "/" + recd + "/" + ci.Name;
                    }

                    if (this.cmb_TypeItems.Text == "Reports(*.rdl)")
                    {
                        if ((ci.Type == ItemTypeEnum.Report) && (ciName == ci.Path))
                        {
                            AddToListBox(ci.Name, "Report");
                        }
                    }
                    else if ((ci.Type != ItemTypeEnum.Folder) && (ciName == ci.Path))
                    {
                        if (ci.Type == ItemTypeEnum.Report)
                        {
                            AddToListBox(ci.Name, "Report");
                        }
                        else if (ci.Type == ItemTypeEnum.DataSource)
                        {
                            AddToListBox(ci.Name, "DataSource");
                        }
                        else
                        {
                            AddToListBox(ci.Name, "Others");
                        }
                    }
                }
            }

            if (this.Ltbx_Contents.Items.Count == 0)
            {
                this.Ltbx_Contents.HorizontalContentAlignment = HorizontalAlignment.Center;
                string str = "There are no items in this lists";
                this.Ltbx_Contents.IsEnabled = false;
                this.Ltbx_Contents.Items.Add(str);
            }
        }

        private void AddToListBox(string name, string type)
        {
            string obj = name.Replace("_", "__");
            Grid gridSample = new Grid();
            RowDefinition r1 = new RowDefinition();
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            c1.Width = new GridLength();
            c2.Width = new GridLength();
            gridSample.RowDefinitions.Add(r1);
            gridSample.ColumnDefinitions.Add(c1);
            gridSample.ColumnDefinitions.Add(c2);
            Image img = new Image();
            Label label = new Label();
            label.Content = obj;
            if(type == "Folder")
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/ClosedFolder.png", UriKind.RelativeOrAbsolute));
                img.Name = "Folder";
                Grid.SetColumn(img, 0);
                gridSample.Children.Add(img);
            }
            else if(type == "Report")
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/Report.png", UriKind.RelativeOrAbsolute));
                img.Name = "Report";
                Grid.SetColumn(img, 0);
                gridSample.Children.Add(img);
            }
            else if(type == "DataSource")
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/DataSource.png", UriKind.RelativeOrAbsolute));
                img.Name = "DataSource";
                Grid.SetColumn(img, 0);
                gridSample.Children.Add(img);
            }
            else if (type == "Link")
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/SiteAddress.png", UriKind.RelativeOrAbsolute));
                img.Name = "Link"; 
                Grid.SetColumn(img, 0);
                gridSample.Children.Add(img);
            }
            else if (type == "Others")
            {
                Label label1 = new Label();
                img.Name = "Others"; 
                label1.Content = "  ";
                Grid.SetColumn(label1, 0);
                gridSample.Children.Add(label1);
            }
            
            Grid.SetColumn(label, 1);
            gridSample.Children.Add(label);
            this.Ltbx_Contents.Items.Add(gridSample);
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            GoBackClicked();
        }

        private void GoBackClicked()
        {
            if (((this.cmb_SitesAndServer.SelectedItem as Grid).Children[1] as Label).Content.ToString().Replace("__", "_") != this.givenFileName && this.cmb_SitesAndServer.SelectedIndex != 0)
            {
                int i = this.cmb_SitesAndServer.Items.Count;
                this.cmb_SitesAndServer.Items.RemoveAt(i - 1);
                this.cmb_SitesAndServer.SelectedIndex = i - 2;
            }
            else
            {
                this.Ltbx_Contents.Items.Clear();
                this.Ltbx_Contents.IsEnabled = true;
                this.Ltbx_Contents.HorizontalContentAlignment = HorizontalAlignment.Left;
                this.cmb_SitesAndServer.Items.Remove(this.cmb_SitesAndServer.Items.Count - 1);
                this.cmb_SitesAndServer.SelectedIndex = 0;
            }
        }
    }
}