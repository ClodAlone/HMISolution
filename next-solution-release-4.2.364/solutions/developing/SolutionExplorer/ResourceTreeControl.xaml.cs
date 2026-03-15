using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using UFProjectManager.ComponentService;

namespace UFProjectManager
{
    /// <summary>
    /// Interaction logic for ResourceTreeControl.xaml
    /// </summary>
    public partial class ResourceTreeControl : UserControl
    {
        #region DP
        #region IsTitleVisible
        public static readonly DependencyProperty IsTitleVisibleProperty = DependencyProperty.Register("IsTitleVisible", typeof(bool), typeof(ResourceTreeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsTitleVisibleChanged), new CoerceValueCallback(OnCoerceIsTitleVisible)));

        private static object OnCoerceIsTitleVisible(DependencyObject o, object value)
        {
            ResourceTreeControl ResourceTreeControl = o as ResourceTreeControl;
            if (ResourceTreeControl != null)
                return ResourceTreeControl.OnCoerceIsTitleVisible((bool)value);
            else
                return value;
        }

        private static void OnIsTitleVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ResourceTreeControl ResourceTreeControl = o as ResourceTreeControl;
            if (ResourceTreeControl != null)
                ResourceTreeControl.OnIsTitleVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
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

        public Popup TypeContextMenu { get { return contextMenu; } }

        #endregion
        #endregion

        public ResourceTreeControl()
        {
            InitializeComponent();
        }
    }
}
