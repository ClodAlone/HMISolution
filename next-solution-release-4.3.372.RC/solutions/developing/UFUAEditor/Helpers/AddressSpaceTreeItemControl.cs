using UFUAModel;
using WPFUtilities;
using System.Windows.Media;
using System.Windows;

namespace UFUAEditor.Helpers
{
    public class AddressSpaceTreeItemControl : TreeItemControl
    {
        #region DP
        #region DataType
        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(string), typeof(AddressSpaceTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDataTypeChanged), new CoerceValueCallback(OnCoerceDataType)));

        private static object OnCoerceDataType(DependencyObject o, object value)
        {
            AddressSpaceTreeItemControl AddressSpaceTreeItemControl = o as AddressSpaceTreeItemControl;
            if (AddressSpaceTreeItemControl != null)
                return AddressSpaceTreeItemControl.OnCoerceDataType((string)value);
            else
                return value;
        }

        private static void OnDataTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AddressSpaceTreeItemControl AddressSpaceTreeItemControl = o as AddressSpaceTreeItemControl;
            if (AddressSpaceTreeItemControl != null)
                AddressSpaceTreeItemControl.OnDataTypeChanged((string)e.OldValue, (string)e.NewValue);
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
        #region PrototypeName
        public static readonly DependencyProperty PrototypeNameProperty = DependencyProperty.Register("PrototypeName", typeof(string), typeof(AddressSpaceTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPrototypeNameChanged), new CoerceValueCallback(OnCoercePrototypeName)));

        private static object OnCoercePrototypeName(DependencyObject o, object value)
        {
            AddressSpaceTreeItemControl AddressSpaceTreeItemControl = o as AddressSpaceTreeItemControl;
            if (AddressSpaceTreeItemControl != null)
                return AddressSpaceTreeItemControl.OnCoercePrototypeName((string)value);
            else
                return value;
        }

        private static void OnPrototypeNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AddressSpaceTreeItemControl AddressSpaceTreeItemControl = o as AddressSpaceTreeItemControl;
            if (AddressSpaceTreeItemControl != null)
                AddressSpaceTreeItemControl.OnPrototypeNameChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoercePrototypeName(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPrototypeNameChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("PrototypeName");
        }

        public string PrototypeName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(PrototypeNameProperty);
            }
            set
            {
                SetValue(PrototypeNameProperty, value);
            }
        }
        #endregion
        #region HistorianSettings
        public static readonly DependencyProperty HistorianSettingsProperty = DependencyProperty.Register("HistorianSettings", typeof(string), typeof(AddressSpaceTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnHistorianSettingsChanged), new CoerceValueCallback(OnCoerceHistorianSettings)));

        private static object OnCoerceHistorianSettings(DependencyObject o, object value)
        {
            AddressSpaceTreeItemControl AddressSpaceTreeItemControl = o as AddressSpaceTreeItemControl;
            if (AddressSpaceTreeItemControl != null)
                return AddressSpaceTreeItemControl.OnCoerceHistorianSettings((string)value);
            else
                return value;
        }

        private static void OnHistorianSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AddressSpaceTreeItemControl AddressSpaceTreeItemControl = o as AddressSpaceTreeItemControl;
            if (AddressSpaceTreeItemControl != null)
                AddressSpaceTreeItemControl.OnHistorianSettingsChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceHistorianSettings(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnHistorianSettingsChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("HistorianSettings");
        }

        public string HistorianSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(HistorianSettingsProperty);
            }
            set
            {
                SetValue(HistorianSettingsProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public AddressSpaceTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(UFUATag))
            {
                var tag = header as UFUATag;
                DataType = tag.DataType.ToString();
                PrototypeName = tag.PrototypeName;
                HistorianSettings = tag.HistorianSettings;
            }
        }
        #endregion
    }
}
