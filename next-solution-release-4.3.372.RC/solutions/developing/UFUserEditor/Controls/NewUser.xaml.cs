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
using WPFUtilities.PropertyDataTemplate;
using System.ComponentModel;
using Utilities.WPF;
using UFUserEditor.ComponentService;
using DocumentManager.ComponentService;
using TranslationHelpers;
using System.Windows.Controls.Primitives;

namespace UFUserEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewUser.xaml
    /// </summary>
    public partial class NewUser : UserControl
    {
        #region DP

        #region IsRuntime
        public static readonly DependencyProperty IsRuntimeProperty = DependencyProperty.Register("IsRuntime", typeof(bool), typeof(NewUser), new UIPropertyMetadata(false));
        public bool IsRuntime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsRuntimeProperty);
            }
            set
            {
                SetValue(IsRuntimeProperty, value);
            }
        }
        #endregion

        #endregion
        #region Declarations
        IDictionary<string, string> stringlist;
        IDocument Document;
        #endregion

        #region Ctor
        public NewUser(Document.UFUserDocument document, bool isRuntime = false)
        {
            InitializeComponent();
            Document = document;
            IsRuntime = isRuntime;
            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, document, isRuntime);
            if (IsRuntime)
            {
                FontSize = WPFUtilities.Properties.Settings.Default.DialogControlsFontSize;
                runtimePasswordInDays.Visibility = Visibility.Visible;
                runtimeContentPresenterPasswordInDays.Visibility = Visibility.Visible;
                textPasswordExpiresInDays.Visibility = Visibility.Collapsed;
                contentPresenterPasswordExpiresInDays.Visibility = Visibility.Collapsed;
            }

            if (UFUserEditorManagerComponent.userEditorManagerComponent != null &&
                UFUserEditorManagerComponent.userEditorManagerComponent.StringEditorManager != null)
            {
                var stringManager = UFUserEditorManagerComponent.userEditorManagerComponent.StringEditorManager;
                var itemSource = new List<string>() { string.Empty };
                var cultures = stringManager.GetListAvailableCultures(document);
                if (cultures != null)
                    itemSource.AddRange(cultures);
                cmbCultures.ItemsSource = itemSource;

                //if (document.ActiveView as UFUserEditorControl != null)
                if (stringManager != null)
                    stringlist = stringManager.GetListStringForCulture(document, stringManager.GetActiveCulture(document));

                //userProperties.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserProperties", stringlist, Properties.Resources.UserProperties);
                userName.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserName", stringlist, Properties.Resources.UserName);
                userPassword.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserPassword", stringlist, Properties.Resources.Password);
                userPasswordConfirm.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserPasswordConfirm", stringlist, Properties.Resources.PasswordConfirm);
                electronicSignature.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserElectronicSignature", stringlist, Properties.Resources.ElectronicSignature);
                textBlockArrayDimension.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserPasswordExpiresInDays", stringlist, Properties.Resources.PasswordExpiresInDays);
                textForcePasswordChangeFirstLogin.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserForcePasswordChangeFirstLogin", stringlist, Properties.Resources.ForcePasswordChangeFirstLogin);
                textUserLoginDiabled.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserLoginDisabled", stringlist, Properties.Resources.UserLoginDisabled);
                userEmail.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserEmail", stringlist, Properties.Resources.Email);
                userPhoneNumber.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserPhoneNumber", stringlist, Properties.Resources.PhoneNumber);
                userMobile.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserMobile", stringlist, Properties.Resources.Mobile);
                userTelegram.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserTelegramChatID", stringlist, Properties.Resources.TelegramChatID);
                userAccessLevel.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserAccessLevel", stringlist, Properties.Resources.AccessLevel);
                userAccessMask.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserAccessMask", stringlist, Properties.Resources.AccessMask);
                userCultureName.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserCultureName", stringlist, Properties.Resources.CultureName);
                userFirebase.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_UserFCMTokenPath", stringlist, Properties.Resources.FCMTokenPath);
            }
            if (UFUserEditorManagerComponent.userEditorManagerComponent != null &&
                UFUserEditorManagerComponent.userEditorManagerComponent.DispatcherEditorManager == null)
            {
                userEmail.Visibility = Visibility.Collapsed;
                EmailMaskedTextbox.Visibility = Visibility.Collapsed;
                userPhoneNumber.Visibility = Visibility.Collapsed;
                textEditPhone.Visibility = Visibility.Collapsed;
                userMobile.Visibility = Visibility.Collapsed;
                textEditMobile.Visibility = Visibility.Collapsed;
                userTelegram.Visibility = Visibility.Collapsed;
                textTelegram.Visibility = Visibility.Collapsed;
                userFirebase.Visibility = Visibility.Collapsed;
                textFirebase.Visibility = Visibility.Collapsed;
            }
            //textPasswordExpiresInDays.NumberFormatInfo = new System.Globalization.NumberFormatInfo() { NumberDecimalDigits = 0 };

            //FontSize = Properties.Settings.Default.FontSize;
            //Foreground = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;
        }
        #endregion

        #region Methods
        private void AMButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int? value = 0;
            if (button.Tag is int)
            {
                try
                {
                    value = (int)button.Tag;
                }
                catch (Exception ex)
                {

                }
            }

            if (value == -1)
                value = null;

            var mdl = new model() { Value = value };
            var editor = new BitMaskEditor(IsRuntime) { DataContext = mdl, ShowInheritedButton = true, Document = Document };
            var ColorDialog = new GeneralDialogContent(editor)
            {
                Owner = button.FindParent<Window>(),
                Title = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_AccessMaskEditor", stringlist, Properties.Resources.AccessMaskEditor),
                HelpLink = "AccessMaskEditor"
            };
            if (ColorDialog.ShowDialog() != true)
            {
                return;
            }

            if (mdl.Value.HasValue)
                button.Tag = mdl.Value;
            else
                button.Tag = -1;
        }
        #endregion

        private void TouchButton_Click(object sender, RoutedEventArgs e)
        {
            Button touchButton = sender as Button;
            TextBox textBox = touchButton.Tag as TextBox;
            CommonControls.BindablePasswordBox passwordBox = touchButton.Tag as CommonControls.BindablePasswordBox;
            bool bNotMandatory = false;
            bool bIsNumeric = false;
            if(textBox != null)
            {
                if (textBox == textEditElectronicSignature || textBox == EmailMaskedTextbox ||
                    textBox == textEditPhone || textBox == textEditMobile || textBox == textTelegram || textBox == textFirebase)
                    bNotMandatory = true;
                else if (textBox == textEditAccessLevel || textBox == textRuntimePasswordExpiresInDays)
                    bIsNumeric = true;

                string touchTxt = bIsNumeric ? Pads.Pads.ShowNumericPad(textBox.Text, this.FindParent<Window>()) :
                                              Pads.Pads.ShowAlphaNumericPad(textBox.Text, this.FindParent<Window>());
                if (!String.IsNullOrEmpty(touchTxt) || bNotMandatory)
                {
                    textBox.Text = touchTxt;
                    textBox.SelectAll();
                }
                textBox.Focus();
            }
            else if(passwordBox != null)
            {
                string touchTxt = Pads.Pads.ShowPasswordPad(passwordBox.Password, this.FindParent<Window>());
                if (!String.IsNullOrEmpty(touchTxt))
                {
                    passwordBox.Password = touchTxt;
                }
                passwordBox.Focus();
            }
        }
    }
}
