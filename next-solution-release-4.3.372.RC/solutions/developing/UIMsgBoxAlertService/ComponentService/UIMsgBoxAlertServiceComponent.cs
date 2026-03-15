using System;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using System.Windows;
#if !WINDOWS_UWP
using Ookii.Dialogs.Wpf;
using System.Windows.Input;
using DevExpress.Xpf.WindowsUI;
using System.Reflection;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
#else
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using System.Collections.Generic;
#endif

namespace UIMsgBoxAlertService.ComponentService
{
    public class UIMsgBoxAlertServiceComponent : ComponentBase<IUIMsgBoxAlertService>, IUIMsgBoxAlertService, IDisposable
    {
#region Declaration
        Object lockObject = new Object();
#if !WINDOWS_UWP
        IBusyComponent busyComponent;
        List<string> dontShowAgainKeys;
#endif
#endregion
#region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            GetComponentInterfaces();
#if !WINDOWS_UWP
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                dontShowAgainKeys = new List<string>();
                SaveDontShowAgainKeys();
            }
#endif
        }
#endregion

        private void GetComponentInterfaces()
        {
#if !WINDOWS_UWP
            if (busyComponent == null)
                busyComponent = GetService(typeof(IBusyComponent)) as IBusyComponent;
#endif
        }

#region IUIMsgBoxAlertService Members

        /// <summary>
        /// Displays an error dialog with a given message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowError(string message)
        {
            ShowMessage(message, Properties.Resources.ErrorTitle, CustomDialogIcons.Stop);
        }

        /// <summary>
        /// Displays an error dialog with a given message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowInformation(string message)
        {
            ShowMessage(message, Properties.Resources.InformationTitle, CustomDialogIcons.Information);
        }

#if !WINDOWS_UWP
        /// <summary>
        /// Shows an information message with the "Don’t show this message again" option
        /// </summary>
        /// <param name="message">The information message</param>
        /// <param name="messageId">The key string to use for storing option</param>
        public bool ShowHidingInformation(string message, string messageId)
        { 
            if (dontShowAgainKeys == null)
                LoadDontShowAgainKeys();
            if (dontShowAgainKeys != null && dontShowAgainKeys.Contains(messageId))
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    dontShowAgainKeys.Remove(messageId);
                    SaveDontShowAgainKeys();
                }
                else
                    return false;
            }

            var messageBox = new UIMsgBoxAlertService.ComponentService.Controls.DontShowAgainMessageBox();
            messageBox.txtMessage.Text = message;
            var wnd = new DevExpress.Xpf.Core.ThemedWindow()
            {
                Content = messageBox,
                ShowTitle = true,
                ShowIcon = true,
                //ShowInTaskbar = true,
                ShowActivated = true,
                Title = Properties.Resources.InformationTitle,
                Icon = GetImageSource(System.Drawing.SystemIcons.Information),
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            var currentSkin = Utilities.ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
            if (!String.IsNullOrEmpty(currentSkin))
                WPFUtilities.ThemeHelper.SetTheme(wnd, currentSkin);
            wnd.ShowDialog(MessageBoxButton.OK);
            var bDontShowAgain = messageBox.checkDontShowAgain.IsChecked == true;
            if (bDontShowAgain)
            {
                if (dontShowAgainKeys == null)
                    dontShowAgainKeys = new List<string>();
                dontShowAgainKeys.Add(messageId);
                SaveDontShowAgainKeys();
            }

            return true;
        }
#endif

        /// <summary>
        /// Displays an error dialog with a given message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void ShowWarning(string message)
        {
            ShowMessage(message, Properties.Resources.WarningTitle, CustomDialogIcons.Warning);
        }

        /// <summary>
        /// Displays a Yes/No dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowYesNo(string message, CustomDialogIcons icon, string title = null)
        {
            return ShowQuestionWithButton(message, icon, CustomDialogButtons.YesNo, title);
        }

        /// <summary>
        /// Displays a Yes/No/Cancel dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowYesNoCancel(string message, CustomDialogIcons icon, string title = null)
        {
            return ShowQuestionWithButton(message, icon, CustomDialogButtons.YesNoCancel, title);
        }

        /// <summary>
        /// Displays a OK/Cancel dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowOkCancel(string message, CustomDialogIcons icon, string title = null)
        {
            return ShowQuestionWithButton(message, icon, CustomDialogButtons.OKCancel, title);
        }

#if !WINDOWS_UWP
        /// <summary>
        /// Displays a Yes/Yes to All/No/No to All/Cancel dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        public CustomDialogResults ShowYesNoAllCancel(string message, CustomDialogIcons icon, string title = null)
        {
            var messageBox = new Controls.YesNoAllCancelControl();
            messageBox.txtMessage.Text = message;

            var wnd = new DevExpress.Xpf.Core.ThemedWindow()
            {
                Content = messageBox,
                ShowTitle = true,
                ShowIcon = icon != CustomDialogIcons.None ? true : false,
                //ShowInTaskbar = true,
                ShowActivated = true,
                Title = title ?? Properties.Resources.PleaseConfirmTitle,
                Icon = GetImageSource(icon),
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            var currentSkin = Utilities.ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
            if (!String.IsNullOrEmpty(currentSkin))
                WPFUtilities.ThemeHelper.SetTheme(wnd, currentSkin);
            if (busyComponent != null)
                busyComponent.ResetBusy();
            wnd.ShowDialog();
            if (busyComponent != null)
                busyComponent.RestoreBusy();

            return messageBox.ClickedButton;
        }

        public ShowCredentialResults ShowCredentialDialog(ShowCredentialOptions option)
        {
            ShowCredentialResults ret = new ShowCredentialResults();
            ret.result = CustomDialogResults.Cancel;
            if (!Environment.UserInteractive)
                return ret;

            using (CredentialDialog dialog = new CredentialDialog())
            {
                // The window title will not be used on Vista and later; there the title will always be "Windows Security".
                dialog.WindowTitle = option.WindowTitle;
                dialog.MainInstruction = option.MainInstruction;
                dialog.Content = option.Content;
                dialog.ShowSaveCheckBox = option.ShowSaveCheckBox;
                dialog.ShowUIForSavedCredentials = option.ShowUIForSavedCredentials;
                // The target is the key under which the credentials will be stored.
                // It is recommended to set the target to something following the "Company_Application_Server" pattern.
                // Targets are per user, not per application, so using such a pattern will ensure uniqueness.
                dialog.Target = option.SavedCredentialsBucket;
                if (dialog.ShowDialog())
                {
                    ret.UserName = dialog.Credentials.UserName;
                    ret.Password = dialog.Credentials.Password;
                    // Normally, you should verify if the credentials are correct before calling ConfirmCredentials.
                    // ConfirmCredentials will save the credentials if and only if the user checked the save checkbox.
                    // dialog.ConfirmCredentials(true);

                    ret.result = CustomDialogResults.OK;
                }
            }

            return ret;
        }

        public String ShowBrowseFolderDialog(String selectedpath)
        {
            if (!Environment.UserInteractive)
                return String.Empty;
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.SelectedPath = selectedpath;
            if (dialog.ShowDialog() != true)
                return String.Empty;
            return dialog.SelectedPath;
        }
#endif

#if WINDOWS_UWP
        async Task<ShowCredentialResults> ShowCredential()
        {
            ShowCredentialResults ret = new ShowCredentialResults();
            ret.result = CustomDialogResults.Cancel;

            SignInContentDialog signInDialog = new SignInContentDialog();
            await signInDialog.ShowAsync();

            if (signInDialog.Result == SignInResult.SignInOK)
            {
                // Sign in was successful.
                ret.result = CustomDialogResults.OK;
                ret.UserName = signInDialog.UserName;
                ret.Password = signInDialog.Password;
            }
            /*
            else if (signInDialog.Result == SignInResult.SignInFail)
            {
                // Sign in failed.
            }
            else if (signInDialog.Result == SignInResult.SignInCancel)
            {
                // Sign in was cancelled by the user.
            }
            */
            return ret;
        }
#endif

#if !WINDOWS_UWP
        public String ShowOpenFileDialog(String filter, FileOpenOptions? options = null)
        {
            if (!Environment.UserInteractive)
                return String.Empty;
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Filter = filter;
            if (options != null && options.HasValue)
            {
                dialog.CheckFileExists = options.Value.CheckFileExists;
                dialog.AddExtension = options.Value.AddExtension;
                dialog.DefaultExt = options.Value.DefaultExt;
                if (String.IsNullOrEmpty(dialog.DefaultExt) && !String.IsNullOrEmpty(filter))
                    dialog.DefaultExt = GetDefaultExtension(filter);
            }
            if (dialog.ShowDialog() != true)
                return String.Empty;
            return dialog.FileName;
        }

        public String ShowSaveFileDialog(String filter, FileSaveOptions? options = null)
        {
            if (!Environment.UserInteractive)
                return String.Empty;

            VistaSaveFileDialog dialog = new VistaSaveFileDialog();
            dialog.Filter = filter;
            if (options != null && options.HasValue)
            {
                dialog.OverwritePrompt = options.Value.OverwritePrompt;
                dialog.AddExtension = options.Value.AddExtension;
                dialog.DefaultExt = options.Value.DefaultExt;
                if (String.IsNullOrEmpty(dialog.DefaultExt) && !String.IsNullOrEmpty(filter))
                    dialog.DefaultExt = GetDefaultExtension(filter);
            }
            if (dialog.ShowDialog() != true)
                return String.Empty;
            return dialog.FileName;
        }

        public String[] ShowSelectFileDialog(String filter)
        {
            if (!Environment.UserInteractive)
                return null;
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Filter = filter;
            dialog.Multiselect = true;
            if (dialog.ShowDialog() != true)
                return null;
            return dialog.FileNames;
        }
#endif
#endregion

#region Private Methods
        /// <summary>
        /// Shows a standard System.Windows.MessageBox using the parameters requested
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="heading">The heading to be displayed</param>
        /// <param name="icon">The icon to be displayed.</param>
        private void ShowMessage(string message, string heading, CustomDialogIcons icon)
        {
#if !WINDOWS_UWP
           if (Environment.UserInteractive)
            {
                //if (System.Threading.Thread.CurrentThread.GetApartmentState() == System.Threading.ApartmentState.STA)
                //{
                //    WinUIMessageBox.Show(
                //                    Keyboard.FocusedElement != null ? Window.GetWindow(Keyboard.FocusedElement as DependencyObject) : null,
                //                    message,
                //                    heading,
                //                    MessageBoxButton.OK,
                //                    GetImage(icon),
                //                    MessageBoxResult.None, MessageBoxOptions.None,
                //                    DevExpress.Xpf.Core.FloatingMode.Window
                //                    );
                //}
                //else
                {
                    if (busyComponent != null)
                        busyComponent.ResetBusy();
                    try
                    {
                        MessageBox.Show(message, heading, MessageBoxButton.OK, GetImage(icon));
                    }
                    catch
                    {
                        MessageBox.Show(message, heading, MessageBoxButton.OK, GetImage(icon), MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                    }
                    if (busyComponent != null)
                        busyComponent.RestoreBusy();
                }
            }
#else
            showMsgBox(message, heading);
#endif
        }

#if WINDOWS_UWP
        private async void showMsgBox(string message, string heading)
        {
            var service = new DevExpress.Mvvm.UI.MessageBoxService();
            var list = new List<DevExpress.Mvvm.UICommand>();
            await service.ShowAsync(message, heading, list);

            //var msgDialog = new MessageDialog(message, heading);
            //await msgDialog.ShowAsync();
            /*
            var dialog = new ContentDialog()
            {
                Title = heading
            };
            var panel = new StackPanel();
            panel.Children.Add(new TextBlock() { Text = message, TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap,
                                                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center });
            var btn = new Button() { Content = "Ok", HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center };
            panel.Children.Add(btn);
            btn.Click += (o, e) => { dialog.Hide(); };
            dialog.Content = panel;

            await dialog.ShowAsync();
            */
        }

        private async Task<CustomDialogResults> showQuestion(string message, CustomDialogButtons buttons)
        {
            var service = new DevExpress.Mvvm.UI.MessageBoxService();
            var list = new List<DevExpress.Mvvm.UICommand>();

            switch (buttons)
            {
                case CustomDialogButtons.OK:
                    list.Add(new DevExpress.Mvvm.UICommand { Caption = "Ok", Id = 0, IsDefault = true });
                    break;
                case CustomDialogButtons.OKCancel:
                    list.Add(new DevExpress.Mvvm.UICommand { Caption = "Ok", Id = 0, IsDefault = true });
                    list.Add(new DevExpress.Mvvm.UICommand { Caption = "Cancel", Id = 1, IsCancel = true });
                    break;
                case CustomDialogButtons.YesNo:
                    list.Add(new DevExpress.Mvvm.UICommand { Caption = "Yes", Id = 0, IsDefault = true });
                    list.Add(new DevExpress.Mvvm.UICommand { Caption = "No", Id = 1, IsCancel = true });
                    break;
                case CustomDialogButtons.YesNoCancel:
                    list.Add(new DevExpress.Mvvm.UICommand { Caption = "Yes", Id = 0, IsDefault = true });
                    list.Add(new DevExpress.Mvvm.UICommand { Caption = "No", Id = 1 });
                    list.Add(new DevExpress.Mvvm.UICommand { Caption = "Cancel", Id = 2, IsCancel = true });
                    break;
            }
            var res = await service.ShowAsync(message, "", list);
            switch (buttons)
            {
                case CustomDialogButtons.OK:
                    return CustomDialogResults.OK;
                case CustomDialogButtons.OKCancel:
                    return (int)res.Id == 0 ? CustomDialogResults.OK : CustomDialogResults.Cancel;
                case CustomDialogButtons.YesNo:
                    return (int)res.Id == 0 ? CustomDialogResults.Yes : CustomDialogResults.No;
                case CustomDialogButtons.YesNoCancel:
                    var id = (int)res.Id;
                    switch (id)
                    {
                        case 0: return CustomDialogResults.Yes;
                        case 1: return CustomDialogResults.No;
                        default: return CustomDialogResults.Cancel;
                    }
            }

            return CustomDialogResults.Cancel;

            /*
            var msgDialog = new MessageDialog(message);
            msgDialog.Commands.Clear();
            switch (buttons)
            {
                case CustomDialogButtons.OK:
                    msgDialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                    break;
                case CustomDialogButtons.OKCancel:
                    msgDialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                    msgDialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });
                    break;
                case CustomDialogButtons.YesNo:
                    msgDialog.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
                    msgDialog.Commands.Add(new UICommand { Label = "No", Id = 1 });
                    break;
                case CustomDialogButtons.YesNoCancel:
                    msgDialog.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
                    msgDialog.Commands.Add(new UICommand { Label = "No", Id = 1 });
                    msgDialog.Commands.Add(new UICommand { Label = "Cancel", Id = 2 });
                    break;
            }

            var res = await msgDialog.ShowAsync();

            switch (buttons)
            {
                case CustomDialogButtons.OK:
                    return CustomDialogResults.OK;
                case CustomDialogButtons.OKCancel:
                    return (int)res.Id == 0 ? CustomDialogResults.OK : CustomDialogResults.Cancel; 
                case CustomDialogButtons.YesNo:
                    return (int)res.Id == 0 ? CustomDialogResults.Yes : CustomDialogResults.No;
                case CustomDialogButtons.YesNoCancel:
                    var id = (int)res.Id;
                    switch(id)
                    {
                        case 0: return CustomDialogResults.Yes;
                        case 1: return CustomDialogResults.No;
                        default: return CustomDialogResults.Cancel;
                    }
            }

            return CustomDialogResults.Cancel;
            */
        }
#endif

        /// <summary>
        /// Shows a standard System.Windows.MessageBox using the parameters requested
        /// but will return a translated result to enable adhere to the IMessageBoxService
        /// implementation required. 
        /// 
        /// This abstraction allows for different frameworks to use the same ViewModels but supply
        /// alternative implementations of core service interfaces
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <param name="button"></param>
        /// <param name="title">The title to be displayed.</param>
        /// <returns>CustomDialogResults results to use</returns>
        private CustomDialogResults ShowQuestionWithButton(string message,
            CustomDialogIcons icon, CustomDialogButtons button, string title = null)
        {
#if !WINDOWS_UWP
            if (!Environment.UserInteractive)
                return CustomDialogResults.Cancel;

            MessageBoxResult result;
            //if (System.Threading.Thread.CurrentThread.GetApartmentState() == System.Threading.ApartmentState.STA)
            //{
            //    result = WinUIMessageBox.Show(
            //                    Keyboard.FocusedElement != null ? Window.GetWindow(Keyboard.FocusedElement as DependencyObject) : null,
            //                    message,
            //                    Properties.Resources.PleaseConfirmTitle,
            //                    GetButton(button),
            //                    GetImage(icon),
            //                    MessageBoxResult.None, MessageBoxOptions.None,
            //                    DevExpress.Xpf.Core.FloatingMode.Window
            //                    );
            //}
            //else
            {
                if (busyComponent != null)
                    busyComponent.ResetBusy();
                try
                {
                    result = MessageBox.Show(message, title ?? Properties.Resources.PleaseConfirmTitle,
                        GetButton(button), GetImage(icon));
                }
                catch
                {
                    result = MessageBox.Show(message, title ?? Properties.Resources.PleaseConfirmTitle,
                        GetButton(button), GetImage(icon), MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                }
                if (busyComponent != null)
                    busyComponent.RestoreBusy();
            }

            return GetResult(result);
#else
            return showQuestion(message, button).Result;
#endif
        }

        /// <summary>
        /// Translates a CustomDialogIcons into a standard WPF System.Windows.MessageBox MessageBoxImage.
        /// This abstraction allows for different frameworks to use the same ViewModels but supply
        /// alternative implementations of core service interfaces
        /// </summary>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>A standard WPF System.Windows.MessageBox MessageBoxImage</returns>
#if !WINDOWS_UWP
        private MessageBoxImage GetImage(CustomDialogIcons icon)
        {
            MessageBoxImage image = MessageBoxImage.None;

            switch (icon)
            {
                case CustomDialogIcons.Information:
                    image = MessageBoxImage.Information;
                    break;
                case CustomDialogIcons.Question:
                    image = MessageBoxImage.Question;
                    break;
                case CustomDialogIcons.Exclamation:
                    image = MessageBoxImage.Exclamation;
                    break;
                case CustomDialogIcons.Stop:
                    image = MessageBoxImage.Stop;
                    break;
                case CustomDialogIcons.Warning:
                    image = MessageBoxImage.Warning;
                    break;
            }
            return image;
        }

        ImageSource GetImageSource(CustomDialogIcons icon)
        {
            ImageSource iconSource = null;
            switch (icon)
            {
                case CustomDialogIcons.Information:
                    iconSource = GetImageSource(System.Drawing.SystemIcons.Information);
                    break;
                case CustomDialogIcons.Question:
                    iconSource = GetImageSource(System.Drawing.SystemIcons.Question);
                    break;
                case CustomDialogIcons.Exclamation:
                    iconSource = GetImageSource(System.Drawing.SystemIcons.Exclamation);
                    break;
                case CustomDialogIcons.Stop:
                    iconSource = GetImageSource(System.Drawing.SystemIcons.Error);
                    break;
                case CustomDialogIcons.Warning:
                    iconSource = GetImageSource(System.Drawing.SystemIcons.Warning);
                    break;
            }
            
            return iconSource;
        }

        ImageSource GetImageSource(System.Drawing.Icon icon)
        {
            try
            {
                using (var bmp = icon.ToBitmap())
                {
                    var stream = new MemoryStream();
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    return System.Windows.Media.Imaging.BitmapFrame.Create(stream);
                }
            }
            catch
            {
                return null;
            }
        }
#endif

        /// <summary>
        /// Translates a CustomDialogButtons into a standard WPF System.Windows.MessageBox MessageBoxButton.
        /// This abstraction allows for different frameworks to use the same ViewModels but supply
        /// alternative implementations of core service interfaces
        /// </summary>
        /// <param name="btn">The button type to be displayed.</param>
        /// <returns>A standard WPF System.Windows.MessageBox MessageBoxButton</returns>
#if !WINDOWS_UWP
        private MessageBoxButton GetButton(CustomDialogButtons btn)
        {
            MessageBoxButton button = MessageBoxButton.OK;

            switch (btn)
            {
                case CustomDialogButtons.OK:
                    button = MessageBoxButton.OK;
                    break;
                case CustomDialogButtons.OKCancel:
                    button = MessageBoxButton.OKCancel;
                    break;
                case CustomDialogButtons.YesNo:
                    button = MessageBoxButton.YesNo;
                    break;
                case CustomDialogButtons.YesNoCancel:
                    button = MessageBoxButton.YesNoCancel;
                    break;
            }
            return button;
        }
#endif

        /// <summary>
        /// Translates a standard WPF System.Windows.MessageBox MessageBoxResult into a
        /// CustomDialogIcons.
        /// This abstraction allows for different frameworks to use the same ViewModels but supply
        /// alternative implementations of core service interfaces
        /// </summary>
        /// <param name="result">The standard WPF System.Windows.MessageBox MessageBoxResult</param>
        /// <returns>CustomDialogResults results to use</returns>
#if !WINDOWS_UWP
        private CustomDialogResults GetResult(MessageBoxResult result)
        {
            CustomDialogResults customDialogResults = CustomDialogResults.None;

            switch (result)
            {
                case MessageBoxResult.Cancel:
                    customDialogResults = CustomDialogResults.Cancel;
                    break;
                case MessageBoxResult.No:
                    customDialogResults = CustomDialogResults.No;
                    break;
                case MessageBoxResult.None:
                    customDialogResults = CustomDialogResults.None;
                    break;
                case MessageBoxResult.OK:
                    customDialogResults = CustomDialogResults.OK;
                    break;
                case MessageBoxResult.Yes:
                    customDialogResults = CustomDialogResults.Yes;
                    break;
            }
            return customDialogResults;
        }

        string GetDefaultExtension(String filter)
        {
            string[] filterElements = filter.Split(new char[] { '|' });
            if (filterElements == null || filterElements.Length > 1)
            {
                var index = filterElements[1].IndexOf('.');
                if (index != -1)
                    return filterElements[1].Substring(index + 1);
            }

            return null;
        }

#region Isolated Storage

        static String GetStoreFileName()
        {
            return String.Format("{0}.DontShowAgain.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        void SaveDontShowAgainKeys()
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || dontShowAgainKeys == null)
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(List<string>));
                        serializer.WriteObject(writer, dontShowAgainKeys);
                    }
                }
            }
            catch
            { }
        }

        void LoadDontShowAgainKeys()
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage)
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(), FileMode.OpenOrCreate, isoStorage))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (XmlReader reader = XmlReader.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(List<string>));
                        dontShowAgainKeys = serializer.ReadObject(reader) as List<string>;
                    }
                }
            }
            catch
            { }
        }

        #endregion
#endif
#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            lockObject = null;
        }

#endregion
    }
}
