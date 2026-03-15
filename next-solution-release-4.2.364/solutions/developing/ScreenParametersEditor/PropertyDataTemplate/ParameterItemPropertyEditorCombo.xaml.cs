using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OPCUAViewModel;
using Utilities;
using DevExpress.Xpf.Editors;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using OPCUAViewModelService.ComponentService;
using System.ComponentModel;
using WPFUtilities;
using System.Windows.Data;

namespace ScreenParametersEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ParameterItemPropertyEditor.xaml
    /// </summary>
    public partial class ParameterItemPropertyEditorCombo : UserControl, INotifyPropertyChanged
    {
        #region DP
        #region ButtonTagBindingProp
        public static readonly DependencyProperty ButtonTagBindingPropProperty = DependencyProperty.Register("ButtonTagBindingProp", typeof(String), typeof(ParameterItemPropertyEditorCombo), new UIPropertyMetadata(null, new PropertyChangedCallback(OnButtonTagBindingPropChanged), new CoerceValueCallback(OnCoerceButtonTagBindingProp)));

        private static object OnCoerceButtonTagBindingProp(DependencyObject o, object value)
        {
            ParameterItemPropertyEditorCombo ParameterItemPropertyEditorCombo = o as ParameterItemPropertyEditorCombo;
            if (ParameterItemPropertyEditorCombo != null)
                return ParameterItemPropertyEditorCombo.OnCoerceButtonTagBindingProp((String)value);
            else
                return value;
        }

        private static void OnButtonTagBindingPropChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ParameterItemPropertyEditorCombo ParameterItemPropertyEditorCombo = o as ParameterItemPropertyEditorCombo;
            if (ParameterItemPropertyEditorCombo != null)
                ParameterItemPropertyEditorCombo.OnButtonTagBindingPropChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceButtonTagBindingProp(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnButtonTagBindingPropChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public String ButtonTagBindingProp
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(ButtonTagBindingPropProperty);
            }
            set
            {
                SetValue(ButtonTagBindingPropProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declarations
        bool bLoaded, bFilled, bEditing, bFlashing;
        #endregion

        #region Constructors
        public ParameterItemPropertyEditorCombo()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
                    if (!string.IsNullOrEmpty(currentStyle))
                        ThemeHelper.SetTheme(uriLabel, currentStyle);
                }
            };

            DataContextChanged += (o, e) =>
            {
                if (ButtonTagBindingProp != null)
                {
                    var b = new Binding()
                    {
                        Path = new PropertyPath(ButtonTagBindingProp),
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    };
                    uriButton.SetBinding(Button.TagProperty, b);
                }
            };
        }
        #endregion

        #region Commands
        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            var value = GetTagEntityReference(uriButton.Tag as String);
            if (value == null)
                value = new OPCUAEntityReference(null);

            if (OPCUAViewModelComponent.workspaceService.ContextDocument != null &&
                OPCUAViewModelComponent.workspaceService.ContextDocument is IDocument)
            {
                var doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
                value.Editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                value.Document = OPCUAViewModelComponent.workspaceService.ContextDocument;
            }

            bEditing = false;
            if (value.Edit(sync: true, noDataSinks: true, localserver: true))
            {
                if (value.HasValidValue)
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        bEditing = true;
                        uriButton.Tag = value.RelativePath;
                        bEditing = false;
                    });
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriButton.Tag = null;
        }
        #endregion

        #region Tag Combobox
        private void uriLabel_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillComboBox();
        }

        void FillComboBox(bool bForceRefresh = false)
        {
            if ((bFilled && !bForceRefresh) || !OPCUAViewModelComponent.workspaceServiceAvailable)
                return;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return;
            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;
            bFilled = true;
            progressBar.Visibility = Visibility.Visible;
            var task = Task.Factory.StartNew(() =>
            {
                return editor.GetFlatFullTagNameCollectionOrderByName(doc, false, bForceRefresh);
            });
            task.ContinueWith(ret =>
            {
                uriLabel.ItemsSource = ret.Result;
                progressBar.Visibility = Visibility.Collapsed;
                uriLabel.IsPopupOpen = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            uriLabel.SelectAll();
        }

        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!bEditing || bFlashing || !OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                bEditing = false;
                return;
            }
            bEditing = false;

            var name = uriLabel.Text;
            var tag = GetTagEntityReference(name);
            if (tag == null)
            {
                if (name == null || !name.EndsWith(NamespaceTableConverter.WholeFolderWildChar))
                {
                    bFlashing = true;

                    try
                    {
                        var oldColor = uriLabel.Foreground;
                        uriLabel.Foreground = Brushes.Red;
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                        uriLabel.Foreground = oldColor;
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                        uriLabel.Foreground = Brushes.Red;
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                        uriLabel.Foreground = oldColor;
                    }
                    finally
                    {
                        bFlashing = false;
                    }

                    uriLabel.GetBindingExpression(ComboBoxEdit.TextProperty).UpdateTarget();
                }
                else
                    uriButton.Tag = uriLabel.Text;
            }
            else
            {
                uriButton.Tag = tag.RelativePath;
            }
        }

        internal static OPCUAEntityReference GetTagEntityReference(String text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return null;

            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return null;

            var split = text.Split(':');
            var instance = split[0];
            var name = split[0];
            if (split.Length > 1)
                name = split[1];
            else
                instance = null;

            if (name != null && !name.EndsWith(NamespaceTableConverter.WholeFolderWildChar))
            {
                var xml = editor.GetTagEntityReference(doc, name, instance);
                if (!String.IsNullOrEmpty(xml))
                    return xml.FromXml<OPCUAEntityReference>();
            }

            return null;
        }

        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            bEditing = true;

            FillComboBox(e.Key == Key.F5);
        }

        private void uriLabel_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (bEditing || bFlashing)
                return;

            var name = uriLabel.Text;
            var tag = GetTagEntityReference(name);
            if (tag != null)
                uriButton.Tag = tag.RelativePath;
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }
}
