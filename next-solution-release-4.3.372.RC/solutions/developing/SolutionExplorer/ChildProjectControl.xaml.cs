using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFProjectManager.ComponentService;

namespace UFProjectManager
{
    /// <summary>
    /// Interaction logic for ChildProjectControl.xaml
    /// </summary>
    public partial class ChildProjectControl : UserControl
    {
        #region DP
        #region IsTitleVisible
        public static readonly DependencyProperty IsTitleVisibleProperty = DependencyProperty.Register("IsTitleVisible", typeof(bool), typeof(ChildProjectControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsTitleVisibleChanged), new CoerceValueCallback(OnCoerceIsTitleVisible)));

        private static object OnCoerceIsTitleVisible(DependencyObject o, object value)
        {
            ChildProjectControl ChildProjectControl = o as ChildProjectControl;
            if (ChildProjectControl != null)
                return ChildProjectControl.OnCoerceIsTitleVisible((bool) value);
            else
                return value;
        }

        private static void OnIsTitleVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChildProjectControl ChildProjectControl = o as ChildProjectControl;
            if (ChildProjectControl != null)
                ChildProjectControl.OnIsTitleVisibleChanged((bool) e.OldValue, (bool) e.NewValue);
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
                return (bool) GetValue(IsTitleVisibleProperty);
            }
            set
            {
                SetValue(IsTitleVisibleProperty, value);
            }
        }

        public Popup TypeContextMenu { get { return contextMenu; } }
        #endregion
        #endregion

        public ChildProjectControl()
        {
            InitializeComponent();
        }
    }
}
