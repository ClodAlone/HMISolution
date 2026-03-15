using DataLoggerColumnListControl;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
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
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using WPFUtilities;
using WPFPenHelpers;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using System.ComponentModel;
using Converters;
using StringManager.ComponentService;

namespace SmartControlUtilities.Controls
{
    /// <summary>
    /// Interaction logic for TagSelector.xaml
    /// </summary>
    public partial class PenNameSelector : UserControl, IDisposable
    {
        #region DP
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(PenNameSelector), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDocumentChanged)));
        private static void OnDocumentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PenNameSelector control = o as PenNameSelector;
            if (control != null)
                control.OnDocumentChanged((IDocument)e.OldValue, (IDocument)e.NewValue);
        }
        protected virtual void OnDocumentChanged(IDocument oldValue, IDocument newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != null)
            {
                stringEditorManager = newValue.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            }
        }
        public IDocument Document
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDocument)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }
        #endregion
        #region PropName
        public static readonly DependencyProperty PropNameProperty = DependencyProperty.Register("PropName", typeof(string), typeof(PenNameSelector), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPropNameChanged)));
        private static void OnPropNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PenNameSelector control = o as PenNameSelector;
            if (control != null)
                control.OnPropNameChanged((string)e.OldValue, (string)e.NewValue);
        }
        protected virtual void OnPropNameChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlBinding();
        }
        public string PropName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(PropNameProperty);
            }
            set
            {
                SetValue(PropNameProperty, value);
            }
        }
        #endregion
        #endregion

        #region declarations
        IStringEditorManager stringEditorManager;
        bool bLoaded;
        bool bDisposed;
        bool bEditing;
        public event EventHandler OnApplyChanges;
        bool bInit;
        #endregion

        #region Events
        virtual public void OnApplyChangesEvent(object sender, EventArgs e = null)
        {
            OnApplyChanges?.Invoke(sender, e ?? EventArgs.Empty);
        }
        #endregion

        #region ctor
        public PenNameSelector()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    UpdateControlBinding();
                    bInit = true;
                }
            };
        }

        #endregion

        #region methods
        private void UpdateControlBinding()
        {
            if (PropName != null)
            {
                var bindingValue = new Binding()
                {
                    Path = new PropertyPath(PropName),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                };
                uriLabel.SetBinding(TextBox.TextProperty, bindingValue);
            }
        }
        private void BtnClear(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
                return;
            Helpers.SetProperty(string.Empty, PropName, DataContext);
            OnApplyChangesEvent(this);
        }

        public class ApplyChangeCallerEventArgs : EventArgs
        {
            public bool dataContextResetRequired = true;
        }

        private void textChanged(object sender, TextChangedEventArgs e)
        {
            if(bInit)
                OnApplyChangesEvent(this, new ApplyChangeCallerEventArgs { dataContextResetRequired = false });
        }

        private void BtnEdit(object sender, RoutedEventArgs e)
        {
            if (DataContext == null || Document == null || PropName == null || stringEditorManager == null)
                return;
            var stringEditor = stringEditorManager.GetStringEditor(Document);
            if (stringEditor == null)
                return;

            string value = (string)Helpers.GetPropertyValue(PropName, DataContext);
            var Dialog = new GeneralDialogContent(stringEditor)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                Title = Properties.Resources.SelectStringEditor,
                HelpLink = "StringEditor"
            };
            if (Dialog.ShowDialog() != true)
                return;

            if (stringEditor.DataContext != null && (stringEditor.DataContext as String) != null)
            {
                Helpers.SetProperty(new String((stringEditor.DataContext as String).ToArray()), PropName, DataContext);
                uriLabel.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                OnApplyChangesEvent(this);
            }
        }

        private void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
                return;
            
            if (!bEditing || Document == null || stringEditorManager == null)
            {
                bEditing = false;
                return;
            }

            bEditing = false;
            OnApplyChangesEvent(this);
        }

      
        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            bEditing = true;
        }

      
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
      
            if (uriLabel != null)
            {
                uriLabel.LostFocus -= uriLabel_LostFocus;
                uriLabel.PreviewKeyDown -= uriLabel_KeyDown;
                BindingOperations.ClearAllBindings(uriLabel);
            }
        }

        #endregion
    }
}

