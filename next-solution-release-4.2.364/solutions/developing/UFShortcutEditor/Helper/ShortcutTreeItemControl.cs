using WPFUtilities;
using System.Windows.Media;
using System.Windows;
using System;
using UFShortcutSettings.ShortcutModel;

namespace UFShortcutEditor
{
    public class ShortcutTreeItemControl : TreeItemControl
    {
        #region DP
        #region ShortcutKey
        public static readonly DependencyProperty ShortcutKeyProperty = DependencyProperty.Register("ShortcutKey", typeof(string), typeof(ShortcutTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnShortcutKeyChanged), new CoerceValueCallback(OnCoerceShortcutKey)));

        private static object OnCoerceShortcutKey(DependencyObject o, object value)
        {
            ShortcutTreeItemControl ShortcutTreeItemControl = o as ShortcutTreeItemControl;
            if (ShortcutTreeItemControl != null)
                return ShortcutTreeItemControl.OnCoerceShortcutKey((string)value);
            else
                return value;
        }

        private static void OnShortcutKeyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ShortcutTreeItemControl ShortcutTreeItemControl = o as ShortcutTreeItemControl;
            if (ShortcutTreeItemControl != null)
                ShortcutTreeItemControl.OnShortcutKeyChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceShortcutKey(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShortcutKeyChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string ShortcutKey
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ShortcutKeyProperty);
            }
            set
            {
                SetValue(ShortcutKeyProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public ShortcutTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(UFKeyCommandEntity))
            {
                var tag = header as UFKeyCommandEntity;
                ShortcutKey = tag.ShortcutKey;
            }
        }
        #endregion
    }
}
