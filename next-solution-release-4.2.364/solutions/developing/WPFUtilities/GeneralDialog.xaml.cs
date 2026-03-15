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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using WPFUtilities;
using Utilities.WPF;
using log4net;
using HelpProvider.ComponentService;
using System.Reflection;

namespace Utilities
{
    public enum GeneralDialogButtons : int
    {
        None = 0x00000000,

        OkButton = 0x00000001,
        CloseButton = 0x00000002,
        CancelButton = 0x00000004,
        HelpButton = 0x00000008,

        OkCloseButtons = 0x00000003,
        OkCloseHelpButtons = 0x0000000B,

        OkCancelButtons = 0x00000005,
        OkCancelHelpButtons = 0x0000000D,

        CloseHelpButtons = 0x0000000A,
        CancelHelpButtons = 0x0000000C,

        CannotCancel= 0x10000000,

        AllButtons = 0x000000ff
    }

    public class GeneralDialogContent
    {
        public Window Owner 
        { 
            get
            {
                if (dialogWindow == null)
                    return null;
                return dialogWindow.Owner;
            }
            set
            {
                if (dialogWindow != null && dialogWindow != value)
                    dialogWindow.Owner = value;
            }
        }

        public void Activate()
        {
            if (dialogWindow == null)
                return;
            dialogWindow.Activate();
        }

        public String HelpLink
        {
            get
            {
                if (dialogWindow == null)
                    return String.Empty;
                return dialogWindow.HelpLink;
            }
            set
            {
                if (dialogWindow != null)
                    dialogWindow.HelpLink = value;
            }
        }
        public FrameworkElement DialogContent  { get; private set; }
        public bool DialogKeepContent { get; set; }

        public event CancelEventHandler Closing;

        public String Title 
        { 
            get
            {
                if (dialogWindow == null)
                    return String.Empty;
                return dialogWindow.Title;
            }
            set
            {
                if (dialogWindow != null)
                    dialogWindow.Title = value;
            }
        }

        public bool? DialogResult
        {
            get
            {
                if (dialogWindow == null)
                    return null;
                return dialogWindow.DialogResult;
            }
            set
            {
                if (dialogWindow != null)
                    dialogWindow.DialogResult = value;
            }
        }

        readonly GeneralDialog dialogWindow;
        public GeneralDialogContent(FrameworkElement dialogContent, int fontSize, double buttonWidth, double buttonHeight,
            GeneralDialogButtons buttons, IDictionary<GeneralDialogButtons, String> captions, bool bHandleEnterKey = true, bool bmaximizeContent = false)
            : this(dialogContent, buttons, captions, bHandleEnterKey: bHandleEnterKey)
        {
            dialogWindow.FontSize = fontSize;
            dialogWindow.ButtonWidth = buttonWidth;
            dialogWindow.ButtonHeight = buttonHeight;
            if (bmaximizeContent)
            {
                dialogWindow.Padding = new Thickness(0);
                dialogWindow.bMaximizeContent = bmaximizeContent;
            }
        }

        public GeneralDialogContent(FrameworkElement dialogContent, 
            GeneralDialogButtons buttons, 
            IDictionary<GeneralDialogButtons, String> captions, 
            bool bHandleEnterKey = true) : 
            this(dialogContent, buttons)
        {
            if (captions != null)
            {
                dialogWindow.OkContent = captions.ContainsKey(GeneralDialogButtons.OkButton) ? captions[GeneralDialogButtons.OkButton] : WPFUtilities.Properties.Resources.LabelOk;
                dialogWindow.CloseContent = captions.ContainsKey(GeneralDialogButtons.CloseButton) ? captions[GeneralDialogButtons.CloseButton] : WPFUtilities.Properties.Resources.LabelClose;
                dialogWindow.CancelContent = captions.ContainsKey(GeneralDialogButtons.CancelButton) ? captions[GeneralDialogButtons.CancelButton] : WPFUtilities.Properties.Resources.LabelCancel;
                dialogWindow.HelpContent = captions.ContainsKey(GeneralDialogButtons.HelpButton) ? captions[GeneralDialogButtons.HelpButton] : WPFUtilities.Properties.Resources.LabelHelp;
            }
        }

        public GeneralDialogContent(double x, double y, bool bRelative, bool bAlwaysCenterScreen, FrameworkElement dialogContent, GeneralDialogButtons buttons = GeneralDialogButtons.OkCancelHelpButtons, bool bhandleEnterKey = true)
            : this(dialogContent, buttons, bhandleEnterKey)
        {
            dialogWindow.bRelative = bRelative;
            dialogWindow.x = x;
            dialogWindow.y = y;
            dialogWindow.bAlwaysCenterScreen = bAlwaysCenterScreen;
        }

        public GeneralDialogContent(FrameworkElement dialogContent, GeneralDialogButtons buttons = GeneralDialogButtons.OkCancelHelpButtons, bool bhandleEnterKey = true)
        {
            DialogContent = dialogContent;
            dialogWindow = new GeneralDialog(DialogContent)
            {
                Title = this.Title,
                Owner = this.Owner,
                HelpLink = HelpLink,
                bHandleEnterKey = bhandleEnterKey,
                bShowOk = (buttons & GeneralDialogButtons.OkButton) == GeneralDialogButtons.OkButton,
                bShowClose = (buttons & GeneralDialogButtons.CloseButton) == GeneralDialogButtons.CloseButton,
                bShowCancel = (buttons & GeneralDialogButtons.CancelButton) == GeneralDialogButtons.CancelButton,
                bShowHelp = (buttons & GeneralDialogButtons.HelpButton) == GeneralDialogButtons.HelpButton,
                bHideAllButtons = (buttons & GeneralDialogButtons.AllButtons) == 0,
                bCannotCancel = (buttons & GeneralDialogButtons.CannotCancel) == GeneralDialogButtons.CannotCancel
            };
            if (FontStyleHelper.hasCustomFontFamily)
                dialogWindow.FontFamily = FontStyleHelper.customFontFamily;
            if (FontStyleHelper.hasCustomFontSize)
                dialogWindow.FontSize = FontStyleHelper.customFontSize;
        }

        public void DisableCancelButton()
        {
            dialogWindow.btnCancel.IsEnabled = false;
        }

        public void Hide()
        {
            dialogWindow.Hide();
        }

        public void Show()
        {
            if (DialogKeepContent)
            {
                if (!bSubscribedClosing)
                {
                    bSubscribedClosing = true;
                    dialogWindow.Closing += (o, e) =>
                    {
                        var temp = Closing;
                        if (temp != null)
                            temp(this, e);
                        if (!e.Cancel)
                            dialogWindow.Content = null;
                    };

                    dialogWindow.Closed += (o, e) =>
                    {
                        if (!DialogKeepContent)
                            DisposeDialogContent();
                    };
                }
            }
            else
            {
                if (!bSubscribedClosing)
                {
                    bSubscribedClosing = true;
                    dialogWindow.Closing += (o, e) =>
                    {
                        var temp = Closing;
                        if (temp != null)
                            temp(this, e);
                    };

                    dialogWindow.Closed += (o, e) =>
                    {
                        if (!DialogKeepContent)
                            DisposeDialogContent();
                    };
                }
            }

            EnsureDialogWindowOwner();
            dialogWindow.Show();
        }

        public void Close()
        {
            dialogWindow.Close(); // avoid memory leak
            if (!DialogKeepContent)
                DisposeDialogContent();
            dialogWindow.Content = null;
        }

        void EnsureDialogWindowOwner()
        {
            if (Owner == null)
            {
                if (Application.Current.Dispatcher.CheckAccess() &&
                    Application.Current.MainWindow != null &&
                    Application.Current.MainWindow.CheckAccess())
                    Owner = Application.Current.MainWindow;
                else
                {
                    var ie = Keyboard.FocusedElement as DependencyObject;
                    if (ie != null)
                        Owner = Window.GetWindow(ie);
                }
            }

            try
            {
                //run in the same thread
                Window mainWindow = Application.Current.MainWindow;
            }
            catch
            {
                //runtime window
                dialogWindow.FontSize = WPFUtilities.Properties.Settings.Default.DialogFontSize;
                dialogWindow.ButtonWidth = WPFUtilities.Properties.Settings.Default.ButtonsWidth;
                dialogWindow.ButtonHeight = WPFUtilities.Properties.Settings.Default.ButtonsHeight;
            }
        }

        void DisposeDialogContent()
        {
            if (DialogContent is IDisposable)
                (DialogContent as IDisposable).Dispose();
            else
                DisposeDialogChildren();
        }

        void DisposeDialogChildren()
        {
            var disposables = (from c in DialogContent.GetChildrenOfType<FrameworkElement>()
                                where c is IDisposable && c.GetType().GetMethod("System.IDisposable.Dispose", BindingFlags.NonPublic | BindingFlags.Instance) == null
                                select c).ToList();

            DependencyObjectExtensions.CleanChildrenOfTypeCache();

            disposables.ForEach(c =>
            {
                (c as IDisposable).Dispose();
            });
        }

        bool bSubscribedClosing;
        public bool? ShowDialog()
        {
            if (DialogKeepContent)
            {
                if (!bSubscribedClosing)
                {
                    bSubscribedClosing = true;
                    dialogWindow.Closing += (o, e) =>
                    {
                        var temp = Closing;
                        if (temp != null)
                            temp(this, e);
                        if (!e.Cancel)
                            dialogWindow.Content = null;
                    };
                }
            }
            else
            {
                if (!bSubscribedClosing)
                {
                    bSubscribedClosing = true;
                    dialogWindow.Closing += (o, e) =>
                    {
                        var temp = Closing;
                        if (temp != null)
                            temp(this, e);
                    };
                }
            }

            EnsureDialogWindowOwner();
            dialogWindow.ShowDialog();
            dialogWindow.Close(); // avoid memory leak
            if (Owner != null)
            {
                Owner.Focusable = true;
                Owner.Focus();
            }
            if (!DialogKeepContent)
                DisposeDialogContent();
            // dialogWindow.Content = null;

            return dialogWindow.DialogResult;
        }

        public static IDictionary<GeneralDialogButtons, String> GetDefaultButtonCaptions()
        {
            return new Dictionary<GeneralDialogButtons, String>()
            { 
                { GeneralDialogButtons.OkButton, WPFUtilities.Properties.Resources.LabelOk },
                { GeneralDialogButtons.CloseButton, WPFUtilities.Properties.Resources.LabelClose },
                { GeneralDialogButtons.CancelButton, WPFUtilities.Properties.Resources.LabelCancel },
                { GeneralDialogButtons.HelpButton, WPFUtilities.Properties.Resources.LabelHelp }
            };
        }
    }

    /// <summary>
    /// Interaction logic for GeneralDialog.xaml
    /// </summary>
    public partial class GeneralDialog : DXWindow
    {
        static Dictionary<String, Rect> mapPositions = new Dictionary<String, Rect>();
        static Dictionary<String, WindowState> mapStates = new Dictionary<String, WindowState>();
        static readonly ILog logGeneral = LogManager.GetLogger(WPFUtilities.Properties.Resources.GeneralLog);
        public bool bShowOk = true;
        public bool bShowClose = true;
        public bool bShowCancel = true;
        public bool bCannotCancel = false;
        public bool bShowHelp = true;
        public bool bHideAllButtons = false;
        public bool bMaximizeContent = false;
        public bool bHandleEnterKey = true;
        public bool bRelative = true;
        public bool bAlwaysCenterScreen = false;
        public double x = -1;
        public double y = -1;
        public string HelpLink = string.Empty;
        public string OkContent = WPFUtilities.Properties.Resources.LabelOk;
        public double ButtonWidth = double.NaN;
        public double ButtonHeight = double.NaN;
        public string CancelContent = WPFUtilities.Properties.Resources.LabelCancel;
        public string CloseContent = WPFUtilities.Properties.Resources.LabelClose;
        public string HelpContent = WPFUtilities.Properties.Resources.LabelHelp;
        bool bLoaded;
        static bool bDialogPositionsLoaded;
        static bool bExitEventHandlerRegisterd;
        Button closeButton;
        bool bCloseDispatched;
        public GeneralDialog(FrameworkElement dialogContent)
        {
            InitializeComponent();

            WindowStyle = System.Windows.WindowStyle.ToolWindow;

            //BorderEffect = BorderEffect.Default;

            DialogContent.Content = dialogContent;
            DataContext = this;
            var reference = String.Format("{0}-{1}", dialogContent.GetType(), dialogContent.Name);

            if (!bExitEventHandlerRegisterd)
            {
                bExitEventHandlerRegisterd = true;
                Application.Current.Dispatcher.InvokeIfRequired(() =>
                {
                    Application.Current.Exit += (o, e) =>
                        {
                            try
                            {
                                SaveDialogPositions();
                            }
                            catch (Exception ex)
                            {
                                logGeneral.Error(WPFUtilities.Properties.Resources.FailedToSaveWindowsPositions, ex);
                            }
                        };
                });
            }

            ThemeHelper.SetTheme(this);
            var currentStyle = ThemeHelper.GetTheme(this);
            ThemeHelper.SetTheme(dialogContent, currentStyle);

            Loaded += (o, e) =>
                {
                    if (bLoaded)
                        return;
                    bLoaded = true;

                    btnCancel.Content = CancelContent;
                    btnOK.Content = OkContent;
                    btnClose.Content = CloseContent;
                    btnHelp.Content = HelpContent;
                    if(!double.IsNaN(ButtonWidth))
                    {
                        btnCancel.MinWidth = ButtonWidth;
                        btnOK.MinWidth = ButtonWidth;
                        btnClose.MinWidth = ButtonWidth;
                        btnHelp.MinWidth = ButtonWidth;
                    }
                    if (!double.IsNaN(ButtonHeight))
                    {
                        btnCancel.MinHeight = ButtonHeight;
                        btnOK.MinHeight = ButtonHeight;
                        btnClose.MinHeight = ButtonHeight;
                        btnHelp.MinHeight = ButtonHeight;
                    }
                    if (bHideAllButtons)
                    {
                        btnOK.Visibility = Visibility.Collapsed;
                        btnCancel.Visibility = Visibility.Collapsed;
                        btnHelp.Visibility = Visibility.Collapsed;
                        btnClose.Visibility = Visibility.Collapsed;
                        Footer.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btnOK.Visibility = bShowOk ? Visibility.Visible : Visibility.Collapsed;
                        btnClose.Visibility = bShowClose && !bShowOk ? Visibility.Visible : Visibility.Collapsed;
                        btnCancel.Visibility = bShowCancel ? Visibility.Visible : Visibility.Collapsed;
                        btnHelp.Visibility = bShowHelp ? Visibility.Visible : Visibility.Collapsed;
                    }
                    if(bMaximizeContent)
                    {
                        mainRectangle.Margin = new Thickness(0);
                        mainBorder.Margin = new Thickness(0);
                    }

                    if (bShowClose && bCannotCancel)
                    {
                        Closing += (ob, ev) =>
                        {
                            if (DialogResult != true)
                                ev.Cancel = true;
                        };
                    }

                    if (!bDialogPositionsLoaded)
                    {
                        bDialogPositionsLoaded = true;
                        try
                        {
                            LoadDialogPositions();
                        }
                        catch (Exception ex)
                        {
                            logGeneral.Error(WPFUtilities.Properties.Resources.FailedToLoadWindowsPositions, ex);
                        }
                    }

                    if (x != -1 && y != -1)
                    {
                        WindowStartupLocation = WindowStartupLocation.Manual;
                        SizeToContent = SizeToContent.WidthAndHeight;
                        double dx = x;
                        double dy = y;
                        if (bRelative)
                        {
                            dx = Owner.Left + x;
                            dy = Owner.Top + y;
                        }
                        else
                        {
                            System.Drawing.Point p = System.Windows.Forms.Cursor.Position;
                            var screen = System.Windows.Forms.Screen.FromPoint(p);
                            if(screen != null)
                            {
                                dx += screen.WorkingArea.X;
                                dy += screen.WorkingArea.Y;
                            }
                        }

                        double dw = Width;
                        double dh = Height;
                        if (mapPositions.ContainsKey(reference))
                        {
                            dw = (mapPositions[reference].Width);
                            dh = (mapPositions[reference].Height);
                        }
                        if (dx + dw > SystemParameters.VirtualScreenWidth ||
                            dy + dh > SystemParameters.VirtualScreenHeight ||
                            dx < SystemParameters.VirtualScreenLeft ||
                            dy < SystemParameters.VirtualScreenTop)
                            WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        else
                        {
                            WindowStartupLocation = WindowStartupLocation.Manual;
                            SizeToContent = SizeToContent.Manual;
                            Top = dy;
                            Left = dx;
                            Width = dw;
                            Height = dh;
                        }
                    }
                    else if(bAlwaysCenterScreen)
                    {
                        if (mapPositions.ContainsKey(reference))
                        {
                            var screen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
                            var workingArea = screen.WorkingArea;
                            double dw = (mapPositions[reference].Width);
                            double dh = (mapPositions[reference].Height);
                            double dx = workingArea.Left + (workingArea.Width - dw) / 2;
                            double dy = workingArea.Top + (workingArea.Height - dh) / 2;
                            if (dx + dw > SystemParameters.VirtualScreenWidth ||
                                dy + dh > SystemParameters.VirtualScreenHeight ||
                                dx < SystemParameters.VirtualScreenLeft ||
                                dy < SystemParameters.VirtualScreenTop)
                                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            else
                            {
                                WindowStartupLocation = WindowStartupLocation.Manual;
                                SizeToContent = SizeToContent.Manual;
                                Top = dy;
                                Left = dx;
                                Width = dw;
                                Height = dh;
                            }
                        }
                        else 
                            WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }
                    else if (mapPositions.ContainsKey(reference))
                    {
                        SizeToContent = SizeToContent.Manual;
                        WindowStartupLocation = WindowStartupLocation.Manual;

                        try
                        {
                            // var rect = System.Windows.Forms.Screen.GetWorkingArea(new System.Drawing.Point((int)Top, (int)Left));
                            if (mapPositions[reference].Left + mapPositions[reference].Width > SystemParameters.VirtualScreenWidth ||
                                mapPositions[reference].Top + mapPositions[reference].Height > SystemParameters.VirtualScreenHeight ||
                                mapPositions[reference].Left < SystemParameters.VirtualScreenLeft ||
                                mapPositions[reference].Top < SystemParameters.VirtualScreenTop)
                                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                            else
                            {
                                Top = mapPositions[reference].Top;
                                Left = mapPositions[reference].Left;
                                Width = mapPositions[reference].Width;
                                Height = mapPositions[reference].Height;
                            }
                            //if (Left + Width > rect.Right)
                            //    Left -= (Left + Width - rect.Right);
                            //if (Top + Height > rect.Bottom)
                            //    Top -= (Top + Height - rect.Bottom);
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }

                    if (mapStates.ContainsKey(reference))
                        WindowState = mapStates[reference];

                    dialogContent.Focusable = true;
                    dialogContent.Focus();
                };

            Closed += (o, e) =>
                {
                    if (closeButton != null)
                        closeButton.TouchUp -= Cancel_TouchUp;
                    var rect = new Rect(Left, Top, Width, Height);
                    mapPositions.Remove(reference);
                    mapPositions.Add(reference, rect);
                    mapStates.Remove(reference);
                    mapStates.Add(reference, WindowState);

                    //if (Application.Current.Dispatcher.CheckAccess() && Application.Current.MainWindow != null)
                    //{
                    //    Application.Current.MainWindow.Dispatcher.InvokeIfRequired(() =>
                    //    {
                    //        Application.Current.MainWindow.Activate();
                    //    });
                    //}
                    if (Owner != null)
                        Owner.Activate();

                    ClearContent();
                };

            if (dialogContent.ReadLocalValue(FrameworkElement.WidthProperty) != DependencyProperty.UnsetValue &&
                dialogContent.ReadLocalValue(FrameworkElement.HeightProperty) != DependencyProperty.UnsetValue)
            {
                DialogContent.Width = dialogContent.Width;
                DialogContent.Height = dialogContent.Height;
                dialogContent.ClearValue(FrameworkElement.WidthProperty);
                dialogContent.ClearValue(FrameworkElement.HeightProperty);
                SizeToContent = SizeToContent.WidthAndHeight;
            }
            else if (dialogContent.ReadLocalValue(FrameworkElement.WidthProperty) != DependencyProperty.UnsetValue)
            {
                DialogContent.Width = dialogContent.Width;
                dialogContent.ClearValue(FrameworkElement.WidthProperty);
                SizeToContent = SizeToContent.Width;
            }
            else if (dialogContent.ReadLocalValue(FrameworkElement.HeightProperty) != DependencyProperty.UnsetValue)
            {
                DialogContent.Height = dialogContent.Height;
                dialogContent.ClearValue(FrameworkElement.HeightProperty);
                SizeToContent = SizeToContent.Height;
            }

            ContentRendered += (o, e) =>
                {
                    DialogContent.ClearValue(FrameworkElement.WidthProperty);
                    DialogContent.ClearValue(FrameworkElement.HeightProperty);
                    SizeToContent = SizeToContent.Manual;
                    if (closeButton == null)
                    {
                        closeButton = LayoutHelper.FindElement(this as FrameworkElement, (frameworkElement) =>
                        {
                            if (frameworkElement != null) return frameworkElement.Name == "PART_CloseButton";
                            return false;
                        }) as Button;

                        if (closeButton != null)
                        {
                            closeButton.TouchUp += Cancel_TouchUp;
                        }
                    }
                };
        }

        public void ClearContent()
        {
            // DialogContent.Content = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.ValidateBindings())
            {
                try
                {
                    RequestClose(true);
                }
                catch
                { }
            }
        }

        private void Cancel_TouchUp(object sender, TouchEventArgs e)
        {
            try
            {
                RequestClose();
            }
            catch
            { }
        }

        public void RequestClose(bool result = false)
        {
            if (!bCloseDispatched)
            {
                bCloseDispatched = true;
                Dispatcher.BeginInvoke(() =>
                {
                    bCloseDispatched = false;
                    if (result)
                        DialogResult = true;
                    // check again DialogResult because Closing event handler should be set to 'false' for avoing to close the window
                    if (DialogResult == true)
                        Close();
                }, DispatcherPriority.Input);
            }
        }

        #region Save Load Recents

        const String StoreFileName = "SavedDialogs";

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

        static void SaveDialogPositions()
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(StoreFileName))
                return;

            using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.Create, isoStorage))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    ConformanceLevel = System.Xml.ConformanceLevel.Auto,
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    try
                    {
                        DataContractSerializer serializer1 = new DataContractSerializer(mapPositions.GetType());
                        serializer1.WriteObject(writer, mapPositions);
                        DataContractSerializer serializer2 = new DataContractSerializer(mapStates.GetType());
                        serializer2.WriteObject(writer, mapStates);
                    }
                    catch (Exception ex)
                    {
                        writer.Close();
                    }
                }
            }
        }

        static bool bLoadedPosition;
        static void LoadDialogPositions()
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (bLoadedPosition || null == isoStorage || string.IsNullOrEmpty(StoreFileName) ||
                isoStorage.GetFileNames(StoreFileName).Length <= 0)
                return;
            bLoadedPosition = true;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.OpenOrCreate, isoStorage))
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Auto,
                    CloseInput = true
                };

                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    try
                    {
                        var serializer1 = new DataContractSerializer(mapPositions.GetType());
                        mapPositions = serializer1.ReadObject(reader) as Dictionary<String, Rect>;

                        var serializer2 = new DataContractSerializer(mapStates.GetType());
                        mapStates = serializer2.ReadObject(reader) as Dictionary<String, WindowState>;
                    }
                    catch (Exception ex)
                    {
                        mapStates.Clear();
                        mapPositions.Clear();
                        reader.Close();
                    }
                }
            }
        }

        #endregion

        public static List<T> FindChild<T>(DependencyObject parent/*, string childName*/)
   where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            List<T> foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null )
                {
                    // recursively drill down the tree
                    var tlist = FindChild<T>(child/*, childName*/);
                    if (tlist != null && tlist.Count > 0)
                    {
                        if (foundChild == null)
                            foundChild = new List<T>();
                        foundChild.AddRange(tlist);
                    }
                }
                //else if (!string.IsNullOrEmpty(childName))
                //{
                //    var frameworkElement = child as FrameworkElement;
                //    // If the child's name is set for search
                //    if (frameworkElement != null && frameworkElement.Name == childName)
                //    {
                //        // if the child's name is of the request name
                //        foundChild = (T)child;
                //        break;
                //    }
                //}
                else
                {
                    // child element found.
                    if (foundChild == null)
                        foundChild = new List<T>();
                    foundChild.Add((T)child);
                    if (VisualTreeHelper.GetChildrenCount(child) > 0)
                    {
                        var tlist = FindChild<T>(child/*, childName*/);
                        if (tlist != null && tlist.Count > 0)
                        {
                            if (foundChild == null)
                                foundChild = new List<T>();
                            foundChild.AddRange(tlist);
                        }
                    }
                }
            }

            return foundChild;
        }

        bool ShowHelp()
        {
            //show help...
            string prop = string.Empty;
            var main = this.DialogContent.Content as UserControl;
            var tabs = this.DialogContent.Content as TabControl;
            var scroll = this.DialogContent.Content as ScrollViewer;
            var helplink = this.HelpLink;

            if(!string.IsNullOrEmpty(helplink))
            {
                prop = helplink.ToString();
            }
            else if(scroll != null && scroll.Content != null)
            {
                var _main = scroll.Content as UserControl;
                if(_main != null)
                    prop = _main.GetType().FullName;
            }
            else if (main != null)
            {

                if (main.DataContext == this)
                    return false;

                if (main.DataContext != null)
                    prop = main.DataContext.GetType().FullName;

            }
            else if (tabs != null && tabs.SelectedContent != null)
            {
                prop = tabs.SelectedContent.GetType().FullName;
            }
            if (prop.Length > 0)
            {
                //MessageBox.Show(string.Format("Help for '{0}:{1}'", bind.ResolvedSource.GetType().FullName, bind.ResolvedSourcePropertyName));
                //string prop = string.Format("{0}:{1}", bind.ResolvedSource.GetType().FullName, bind.ResolvedSourcePropertyName);
                /*@"E:\PRIVATE\12-0-Drivers\UFSolution\bin\Debug\DesignPlugins", @"HelpProvider.dll"*/
                var list = FindAndLoadDLL.LoadDLLs<IHelpProvider>(string.Format("{0}DesignPlugins", AppDomain.CurrentDomain.BaseDirectory), "HelpProvider.dll");
                if (list.Count > 0)
                    list[0].OpenDialogHelpPage(prop, true, true);
                return true;
            }

            return false;
        }

        private void DXWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                if (ShowHelp())
                    e.Handled = true;
            }
            else if (e.Key == Key.Enter && !bHideAllButtons)
            {
                if (e.OriginalSource != null)
                {
                    if (e.OriginalSource.GetType().GetProperty("Code") != null)
                        return;
                    var textBox = e.OriginalSource as TextBox;
                    if (textBox != null && textBox.AcceptsReturn)
                        return;
                }

                if (this.ValidateBindings())
                {
                    try
                    {
                        RequestClose(true);
                    }
                    catch
                    { }
                    e.Handled = bHandleEnterKey;
                }
            }
        }

        public IList<DependencyProperty> GetAttachedProperties(DependencyObject obj)
        {
            List<DependencyProperty> result = new List<DependencyProperty>();

            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(obj,
                new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) }))
            {
                DependencyPropertyDescriptor dpd =
                    DependencyPropertyDescriptor.FromProperty(pd);

                if (dpd != null)
                {
                    result.Add(dpd.DependencyProperty);
                }
            }

            return result;
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            ShowHelp();
        }
    }
}
