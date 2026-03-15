using UFUAModel;
using WPFUtilities;
using System.Windows.Media;
using System.Windows;
using TempVariablesModel;

namespace TempVariablesManager.Helpers
{
    public class TempVarTreeItemControl : TreeItemControl
    {
        #region DP
        #region DataType
        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(string), typeof(TempVarTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDataTypeChanged), new CoerceValueCallback(OnCoerceDataType)));

        private static object OnCoerceDataType(DependencyObject o, object value)
        {
            TempVarTreeItemControl TempVarTreeItemControl = o as TempVarTreeItemControl;
            if (TempVarTreeItemControl != null)
                return TempVarTreeItemControl.OnCoerceDataType((string)value);
            else
                return value;
        }

        private static void OnDataTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TempVarTreeItemControl TempVarTreeItemControl = o as TempVarTreeItemControl;
            if (TempVarTreeItemControl != null)
                TempVarTreeItemControl.OnDataTypeChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceDataType(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDataTypeChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("DataType");
        }
        
        public string DataType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(DataTypeProperty);
            }
            set
            {
                SetValue(DataTypeProperty, value);
            }
        }
        #endregion
        #region Description
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(TempVarTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDescriptionChanged), new CoerceValueCallback(OnCoerceDescription)));

        private static object OnCoerceDescription(DependencyObject o, object value)
        {
            TempVarTreeItemControl TempVarTreeItemControl = o as TempVarTreeItemControl;
            if (TempVarTreeItemControl != null)
                return TempVarTreeItemControl.OnCoerceDescription((string)value);
            else
                return value;
        }

        private static void OnDescriptionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TempVarTreeItemControl TempVarTreeItemControl = o as TempVarTreeItemControl;
            if (TempVarTreeItemControl != null)
                TempVarTreeItemControl.OnDescriptionChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceDescription(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDescriptionChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("Description");
        }

        public string Description
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public TempVarTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(UFUATag))
            {
                var tag = header as Variable;
                DataType = tag.DataType.ToString();
                Description = tag.Description;
            }
        }
        #endregion
    }
}
