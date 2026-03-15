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
using WPFUtilities;
using DevExpress.Xpf.Core;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using System.Web.ClientServices.Providers;
using MSZUtilsServiceHelper;

namespace MSZUtils.Controls
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : DXWindow
    {
        readonly LoginInfo loginInfo = new LoginInfo();

        public LoginControl(IDocument document, string currentStyle)
        {
            InitializeComponent();

            Loaded += (o, e) =>
                {
                    DataContext = loginInfo;
                    FontSize = Properties.Settings.Default.FontSize;

                    if (!String.IsNullOrEmpty(txtUser.Text))
                        loginInfo.UserName = txtUser.Text;

                    ThemeHelper.SetTheme(this, currentStyle);
                };
        }

        public ClientAutenticationCredentials GetCredentials()
        {
            //if (Application.Current.Dispatcher.CheckAccess())
            //{
            //    Owner = Application.Current.MainWindow;
            //    Background = Application.Current.MainWindow.Background;
            //}

            if (ShowDialog() == true)
            {
                return new ClientAutenticationCredentials(loginInfo.ToLoginParameters());
            }
            else
            {
                return null;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            loginInfo.Password = passwordBox.Password;
            DialogResult = true;
        }
    }
}
