using DocumentManager.ComponentService;
using StringManager.ComponentService;
using System;
using System.Web.ClientServices.Providers;
using System.Windows;
using System.Windows.Controls;
using TranslationHelpers;
using Utilities;
using Utilities.WPF;

namespace AuthenticationCredentialsProvider
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl, IClientFormsAuthenticationCredentialsProvider
    {
        readonly LoginInfo loginInfo = new LoginInfo();

        string stringPlaceolder = "UserManagement";
        public LoginControl(IDocument document)
        {
            InitializeComponent();
            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, document);
            Loaded += (o, e) =>
                {
                    DataContext = loginInfo;

                    if (!String.IsNullOrEmpty(txtUser.Text))
                        loginInfo.UserName = txtUser.Text;

                    if (document != null)
                    {
                        IStringEditorManager stringManager = document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringManager != null)
                        {
                            var stringlist = stringManager.GetListStringForCulture(document, stringManager.GetActiveCulture(document));

                            requestedRole.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RequestedRole", stringlist, Properties.Resources.RequestedRole);
                            requestedLevel.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RequestedLevel", stringlist, Properties.Resources.RequestedLevel);
                            userName.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_UserNameLabel", stringlist, Properties.Resources.UserNameLabel);
                            password.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PasswordLabel", stringlist, Properties.Resources.PasswordLabel);
                        }
                    }
                };
        }

        public ClientFormsAuthenticationCredentials GetCredentials()
        {
            loginInfo.Password = passwordBox.Password;
            loginInfo.UserName = txtUser.Text;
            return loginInfo.ToLoginParameters();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var user = Pads.Pads.ShowAlphaNumericPad(txtUser.Text, this.FindParent<Window>());
            if (!String.IsNullOrEmpty(user))
            {
                txtUser.Text = user;
                txtUser.SelectAll();
            }
            txtUser.Focus();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var psw = Pads.Pads.ShowPasswordPad(passwordBox.Password, this.FindParent<Window>());
            if (!String.IsNullOrEmpty(psw))
            {
                passwordBox.Password = psw;
                passwordBox.SelectAll();
            }
            passwordBox.Focus();
        }
    }
}
