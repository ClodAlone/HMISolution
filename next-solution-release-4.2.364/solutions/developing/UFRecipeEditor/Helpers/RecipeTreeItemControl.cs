using WPFUtilities;
using System.Windows.Media;
using System.Windows;
using UFRecipeSettings.UFRecipeModel;

namespace UFRecipeEditor
{
    public class RecipeTreeItemControl : TreeItemControl
    {
        #region DP
        #region OID
        public static readonly DependencyProperty OIDProperty = DependencyProperty.Register("OID", typeof(string), typeof(RecipeTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnOIDChanged), new CoerceValueCallback(OnCoerceOID)));

        private static object OnCoerceOID(DependencyObject o, object value)
        {
            RecipeTreeItemControl RecipeTreeItemControl = o as RecipeTreeItemControl;
            if (RecipeTreeItemControl != null)
                return RecipeTreeItemControl.OnCoerceOID((string)value);
            else
                return value;
        }

        private static void OnOIDChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeTreeItemControl RecipeTreeItemControl = o as RecipeTreeItemControl;
            if (RecipeTreeItemControl != null)
                RecipeTreeItemControl.OnOIDChanged((string)e.OldValue, (string)e.NewValue);
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
        #region Type
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(RecipeTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTypeChanged), new CoerceValueCallback(OnCoerceType)));

        private static object OnCoerceType(DependencyObject o, object value)
        {
            RecipeTreeItemControl RecipeTreeItemControl = o as RecipeTreeItemControl;
            if (RecipeTreeItemControl != null)
                return RecipeTreeItemControl.OnCoerceType((string)value);
            else
                return value;
        }

        private static void OnTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeTreeItemControl RecipeTreeItemControl = o as RecipeTreeItemControl;
            if (RecipeTreeItemControl != null)
                RecipeTreeItemControl.OnTypeChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceType(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTypeChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string Type
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public RecipeTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            var type = header.GetType();
            Type = type.ToString();
            if (type == typeof(UFGroupEntity))
            {
                var tag = header as UFGroupEntity;
                OID = tag.OID.ToString();
            }
            else if (type == typeof(UFDataValueEntity))
            {
                var tag = header as UFDataValueEntity;
                OID = tag.OID.ToString();
            }
        }
        #endregion
    }
}
