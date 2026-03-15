using DocumentManager.ComponentService;
using Pads;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TranslationHelpers;
using Utilities;
using Utilities.WPF;

namespace UFUserEditor.Controls
{
    /// <summary>
    /// Interaction logic for PaswordExpired.xaml
    /// </summary>
    public partial class PaswordExpired : UserControl
    {
        string stringPlaceolder = "UserManagement";
        public PaswordExpired(IDocument document)
        {
            InitializeComponent();
            if (document != null)
            {
                ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, document, !DesignerProperties.GetIsInDesignMode(this));

                IStringEditorManager stringManager = document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                if (stringManager != null)
                {
                    var stringlist = stringManager.GetListStringForCulture(document, stringManager.GetActiveCulture(document));
                    oldPassword.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_OldPassword", stringlist, Properties.Resources.OldPassword);
                    password.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Password", stringlist, Properties.Resources.Password);
                    confirmPassword.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PasswordConfirm", stringlist, Properties.Resources.PasswordConfirm);
                }
                //FontSize = Properties.Settings.Default.FontSize;
                //textOldPassword.MinCustomHeight = pswEditPwd.MinCustomHeight = pswEditPwdConfirm.MinCustomHeight = pswEditPwdConfirm.MinHeight = password.MinHeight = confirmPassword.MinHeight = Properties.Settings.Default.LoginControlsHeight;
                //oldPassword.MinWidth = password.MinWidth = confirmPassword.MinWidth = Properties.Settings.Default.LoginControlsWidth;
                //selectButton1.MinWidth = selectButton2.MinWidth = selectButton3.MinWidth = Properties.Settings.Default.LoginSelectButtonWidth;
                //selectButton1.MinHeight = selectButton2.MinHeight = selectButton3.MinHeight = Properties.Settings.Default.LoginSelectButtonHeight;
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var psw = Pads.Pads.ShowPasswordPad(textOldPassword.Password, this.FindParent<Window>());
            if (!String.IsNullOrEmpty(psw))
            {
                textOldPassword.Password = psw;
            }
            textOldPassword.Focus();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            var psw = Pads.Pads.ShowPasswordPad(pswEditPwd.Password, this.FindParent<Window>());
            if (!String.IsNullOrEmpty(psw))
            {
                pswEditPwd.Password = psw;
            }
            pswEditPwd.Focus();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            var psw = Pads.Pads.ShowPasswordPad(pswEditPwdConfirm.Password, this.FindParent<Window>());
            if (!String.IsNullOrEmpty(psw))
            {
                pswEditPwdConfirm.Password = psw;
            }
            pswEditPwdConfirm.Focus();
        }
    }
}
