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
using Utilities.WPF;
using StringManager.ComponentService;
using UFUserEditor.ComponentService;
using DocumentManager.ComponentService;
using TranslationHelpers;
using Pads;

namespace UFUserEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewRole.xaml
    /// </summary>
    public partial class NewRole : UserControl
    {
        #region DP

        #region IsRuntime
        public static readonly DependencyProperty IsRuntimeProperty = DependencyProperty.Register("IsRuntime", typeof(bool), typeof(NewRole), new UIPropertyMetadata(false));
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
        bool bLoaded = false;
        #endregion

        #region Ctor
        public NewRole(Document.UFUserDocument document, bool isRuntime = false)
        {
            InitializeComponent();
            Document = document;
            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, document, isRuntime);
            Loaded += (O, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                var tag = DataContext as UFUserModel.UFRole;
                if (string.IsNullOrEmpty(tag.TelegramGroupChatID))
                {
                    roleTelegramID.Visibility = Visibility.Collapsed;
                    textTelegramGroupChatID.Visibility = Visibility.Collapsed;
                    roleTelegramIDeditBtn.Visibility = Visibility.Collapsed;
                }
                else
                {
                    roleTelegramID.Visibility = Visibility.Visible;
                    textTelegramGroupChatID.Visibility = Visibility.Visible;
                    roleTelegramIDeditBtn.Visibility = Visibility.Visible;
                }
            };
            IsRuntime = isRuntime;
            if (IsRuntime)
                FontSize = WPFUtilities.Properties.Settings.Default.DialogControlsFontSize;
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

                roleName.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_RoleName", stringlist, Properties.Resources.RoleName);
                roleAccessLevel.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_RoleAccessLevel", stringlist, Properties.Resources.AccessLevel);
                roleAccessMask.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_RoleAccessMask", stringlist, Properties.Resources.AccessMask);
                roleCultureName.Text = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_RoleCultureName", stringlist, Properties.Resources.CultureName);
                
            }

            //FontSize = Properties.Settings.Default.FontSize;
            //roleProperties.MinHeight = roleName.MinHeight = roleAccessLevel.MinHeight = Properties.Settings.Default.LoginControlsHeight;
            //roleProperties.MinWidth = roleName.MinWidth = roleAccessLevel.MinWidth = Properties.Settings.Default.LoginControlsWidth;
            //bitmask.MinWidth = Properties.Settings.Default.LoginSelectButtonWidth;
            //bitmask.MinHeight = Properties.Settings.Default.LoginSelectButtonHeight;
        }
        #endregion

        #region Methods
        private void AMButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int value = 0;
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

            var mdl = new model() { Value = value };
            var editor = new BitMaskEditor(IsRuntime) { DataContext = mdl, Document = Document };
            var ColorDialog = new GeneralDialogContent(editor)
            {
                Owner = button.FindParent<Window>(),
                Title = TranslationHelper.TranlslateText($"_{UFUserEditorManagerComponent.stringPlaceolder}_AccessMaskEditor", stringlist, Properties.Resources.AccessMaskEditor),
                HelpLink = "RoleEditor"
            };
            if (ColorDialog.ShowDialog() != true)
            {
                return;
            }
            button.Tag = mdl.Value;
        }
        #endregion

        private void TouchButton_Click(object sender, RoutedEventArgs e)
        {
            Button touchButton = sender as Button;
            TextBox textBox = touchButton.Tag as TextBox;
            bool bNotMandatory = false;
            bool bIsNumeric = false;

            if (textBox == textEditAccessLevel)
                bIsNumeric = true;

            if (textBox != null)
            {
                string touchTxt = bIsNumeric ? Pads.Pads.ShowNumericPad(textBox.Text, this.FindParent<Window>()) : 
                                               Pads.Pads.ShowAlphaNumericPad(textBox.Text, this.FindParent<Window>());
                if (!String.IsNullOrEmpty(touchTxt) || bNotMandatory)
                {
                    textBox.Text = touchTxt;
                    textBox.SelectAll();
                }
                textBox.Focus();
            }
        }
    }
}
