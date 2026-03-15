using UFUAModel;
using WPFUtilities;
using System.Windows.Media;
using System.Windows;

namespace DriverCodeBaseEx.UI.SettingsControls
{
    public class PrototypeTreeItemControl : TreeItemControl
    {
        #region DP
        #region Name
        public static readonly DependencyProperty NameCProperty = DependencyProperty.Register("Name", typeof(string), typeof(PrototypeTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnNameCChanged), new CoerceValueCallback(OnCoerceNameC)));

        private static object OnCoerceNameC(DependencyObject o, object value)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                return PrototypeTreeItemControl.OnCoerceNameC((string)value);
            else
                return value;
        }

        private static void OnNameCChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                PrototypeTreeItemControl.OnNameCChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceNameC(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNameCChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("NameC");
        }

        public string NameC
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(NameCProperty);
            }
            set
            {
                SetValue(NameCProperty, value);
            }
        }
        #endregion
        #region DataType
        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(string), typeof(PrototypeTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDataTypeChanged), new CoerceValueCallback(OnCoerceDataType)));

        private static object OnCoerceDataType(DependencyObject o, object value)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                return PrototypeTreeItemControl.OnCoerceDataType((string)value);
            else
                return value;
        }

        private static void OnDataTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                PrototypeTreeItemControl.OnDataTypeChanged((string)e.OldValue, (string)e.NewValue);
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
        #region ModelType
        public static readonly DependencyProperty ModelTypeProperty = DependencyProperty.Register("ModelType", typeof(string), typeof(PrototypeTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnModelTypeChanged), new CoerceValueCallback(OnCoerceModelType)));

        private static object OnCoerceModelType(DependencyObject o, object value)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                return PrototypeTreeItemControl.OnCoerceModelType((string)value);
            else
                return value;
        }

        private static void OnModelTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                PrototypeTreeItemControl.OnModelTypeChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceModelType(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnModelTypeChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("ModelType");
        }

        public string ModelType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ModelTypeProperty);
            }
            set
            {
                SetValue(ModelTypeProperty, value);
            }
        }
        #endregion        
        #region StringLength
        public static readonly DependencyProperty StringLengthProperty = DependencyProperty.Register("StringLength", typeof(string), typeof(PrototypeTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnMemberStringLength), new CoerceValueCallback(OnCoerceStringLength)));

        private static object OnCoerceStringLength(DependencyObject o, object value)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                return PrototypeTreeItemControl.OnCoerceStringLength((string)value);
            else
                return value;
        }

        private static void OnMemberStringLength(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                PrototypeTreeItemControl.OnMemberStringLength((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceStringLength(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMemberStringLength(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("StringLength");
        }

        public string StringLength
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(StringLengthProperty);
            }
            set
            {
                SetValue(StringLengthProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public PrototypeTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(StructStringLength.ProMemberView))
            {
                var tag = header as StructStringLength.ProMemberView;
                NameC = tag.Name.ToString();                
                ModelType = tag.ModelType.ToString();
                StringLength = tag.StringLength.ToString();
                DataType = tag.DataType.ToString();
            }
        }
        #endregion
    }
}
