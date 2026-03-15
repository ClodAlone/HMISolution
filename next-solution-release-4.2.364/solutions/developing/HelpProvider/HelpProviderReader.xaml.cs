using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HelpProvider
{
    /// <summary>
    /// Interaction logic for HelpProviderReader.xaml
    /// </summary>
    public partial class HelpProviderReader : UserControl
    {
        public HelpProviderReader()
        {
            InitializeComponent();
        }

        private string _LocalFilePath;
        public string LocalFilePath
        {
            get { return _LocalFilePath; }
            set
            {
                _LocalFilePath = value;
            }
        }
        private string _WebFilePath;
        public string WebFilePath
        {
            get { return _WebFilePath; }
            set
            {
                _WebFilePath = value;
            }
        }
        private string _LocalInvariantFilePath;
        public string LocalInvariantFilePath
        {
            get { return _LocalInvariantFilePath; }
            set
            {
                _LocalInvariantFilePath = value;
            }
        }
        private string _DefaultPage;
        public string DefaultPage
        {
            get { return _DefaultPage; }
            set
            {
                _DefaultPage = value;
            }
        }

        internal void ShowTopicName(string prop)
        {
            webBr.Visibility = System.Windows.Visibility.Collapsed;
            flowDocView.Visibility = System.Windows.Visibility.Collapsed;
            frameView.Visibility = System.Windows.Visibility.Collapsed;
            textTopicName.Visibility = System.Windows.Visibility.Visible;
            textTopicName.Text = prop;

        }

        int fails;
        internal void NavigateUri(Uri uri)
        {
            {
                fails = 0;
                flowDocView.Visibility = System.Windows.Visibility.Collapsed;
                frameView.Visibility = System.Windows.Visibility.Collapsed/*Visible*/;

                //frameView.Source = uri;
                webBr.Visibility = System.Windows.Visibility.Visible;
                webBr.Navigate(uri);
            }
        }

        private void frameView_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            if (++fails > 1)
            {
                if (fails > 2)
                {
                    frameView.Source = null;
                    return;
                }
                if (e.Uri.IsFile)
                {
                    //show web
                    frameView.Source = new Uri(string.Format("{0}\\{1}", WebFilePath, DefaultPage));
                }
                else
                {
                    //show local
                    frameView.Source = new Uri(string.Format("{0}.{3}\\{2}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), DefaultPage, LocalInvariantFilePath,mainversion));
                }
                return;
            }
            try
            {
                string s = e.Uri.AbsoluteUri;
                if (e.Uri.IsFile)
                {
                    //search on line
                    string folder = (new Uri(string.Format("{0}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), LocalInvariantFilePath))).AbsolutePath;
                    var pos = s.IndexOf(folder);
                    if (pos != -1)
                    {
                        s = s.Substring(pos + folder.Length);
                        frameView.Source = new Uri(string.Format("{0}\\{1}", WebFilePath, s));
                    }
                }
                else
                {
                    //search locally
                    string folder = (new Uri(WebFilePath)).AbsolutePath;
                    var pos = s.IndexOf(folder);
                    if (pos != -1)
                    {
                        s = s.Substring(pos + folder.Length);
                        frameView.Source = new Uri(string.Format("{0}.{3}\\{2}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), s, LocalFilePath,mainversion));
                    }
                }
            }
            catch (Exception f)
            { }
            
        }
    }
}
