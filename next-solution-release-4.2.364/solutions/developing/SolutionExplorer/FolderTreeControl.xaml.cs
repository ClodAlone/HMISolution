using System.Windows;
using System.Windows.Controls;
using UFProjectManager.ComponentService;

namespace UFProjectManager
{
    /// <summary>
    /// Interaction logic for FolderTreeControl.xaml
    /// </summary>
    public partial class FolderTreeControl : UserControl
    {
        #region DP
        #region IsTitleVisible
        public static readonly DependencyProperty IsTitleVisibleProperty = DependencyProperty.Register("IsTitleVisible", typeof(bool), typeof(FolderTreeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsTitleVisibleChanged), new CoerceValueCallback(OnCoerceIsTitleVisible)));

        private static object OnCoerceIsTitleVisible(DependencyObject o, object value)
        {
            FolderTreeControl FolderTreeControl = o as FolderTreeControl;
            if (FolderTreeControl != null)
                return FolderTreeControl.OnCoerceIsTitleVisible((bool)value);
            else
                return value;
        }

        private static void OnIsTitleVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FolderTreeControl FolderTreeControl = o as FolderTreeControl;
            if (FolderTreeControl != null)
                FolderTreeControl.OnIsTitleVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceIsTitleVisible(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsTitleVisibleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool IsTitleVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsTitleVisibleProperty);
            }
            set
            {
                SetValue(IsTitleVisibleProperty, value);
            }
        }
        #endregion

        public System.Windows.Controls.Primitives.Popup TypeContextMenu { get { return contextMenu; } }

        #endregion
        public FolderTreeControl()
        {
            InitializeComponent();
        }
    }
}
