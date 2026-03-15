using WPFUtilities;
using System.Windows.Media;
using System.Windows;
using MenuSettings.MenuModel;

namespace UFMenuEditor
{
    public class MenuTreeItemControl : TreeItemControl
    {
        #region DP
        #region OID
        public static readonly DependencyProperty OIDProperty = DependencyProperty.Register("OID", typeof(string), typeof(MenuTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnOIDChanged), new CoerceValueCallback(OnCoerceOID)));

        private static object OnCoerceOID(DependencyObject o, object value)
        {
            MenuTreeItemControl MenuTreeItemControl = o as MenuTreeItemControl;
            if (MenuTreeItemControl != null)
                return MenuTreeItemControl.OnCoerceOID((string)value);
            else
                return value;
        }

        private static void OnOIDChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MenuTreeItemControl MenuTreeItemControl = o as MenuTreeItemControl;
            if (MenuTreeItemControl != null)
                MenuTreeItemControl.OnOIDChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceOID(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnOIDChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string OID
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(OIDProperty);
            }
            set
            {
                SetValue(OIDProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public MenuTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(UFMenuItemEntity))
            {
                var tag = header as UFMenuItemEntity;
                OID = tag.OID.ToString();
            }
        }
        #endregion
    }
}
