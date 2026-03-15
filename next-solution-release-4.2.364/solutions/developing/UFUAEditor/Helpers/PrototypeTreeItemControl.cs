using UFUAModel;
using WPFUtilities;
using System.Windows.Media;
using System.Windows;

namespace UFUAEditor.Helpers
{
    public class PrototypeTreeItemControl : TreeItemControl
    {
        #region DP
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
        #region MemberOrderId
        public static readonly DependencyProperty MemberOrderIdProperty = DependencyProperty.Register("MemberOrderId", typeof(string), typeof(PrototypeTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnMemberOrderIdChanged), new CoerceValueCallback(OnCoerceMemberOrderId)));

        private static object OnCoerceMemberOrderId(DependencyObject o, object value)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                return PrototypeTreeItemControl.OnCoerceMemberOrderId((string)value);
            else
                return value;
        }

        private static void OnMemberOrderIdChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                PrototypeTreeItemControl.OnMemberOrderIdChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceMemberOrderId(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMemberOrderIdChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("MemberOrderId");
        }

        public string MemberOrderId
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(MemberOrderIdProperty);
            }
            set
            {
                SetValue(MemberOrderIdProperty, value);
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
        #region PrototypeName
        public static readonly DependencyProperty PrototypeNameProperty = DependencyProperty.Register("PrototypeName", typeof(string), typeof(PrototypeTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPrototypeNameChanged), new CoerceValueCallback(OnCoercePrototypeName)));

        private static object OnCoercePrototypeName(DependencyObject o, object value)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                return PrototypeTreeItemControl.OnCoercePrototypeName((string)value);
            else
                return value;
        }

        private static void OnPrototypeNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                PrototypeTreeItemControl.OnPrototypeNameChanged((string)e.OldValue, (string)e.NewValue);
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
        #region Alarms
        public static readonly DependencyProperty AlarmsProperty = DependencyProperty.Register("Alarms", typeof(string), typeof(PrototypeTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAlarmsChanged), new CoerceValueCallback(OnCoerceAlarms)));

        private static object OnCoerceAlarms(DependencyObject o, object value)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                return PrototypeTreeItemControl.OnCoerceAlarms((string)value);
            else
                return value;
        }

        private static void OnAlarmsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                PrototypeTreeItemControl.OnAlarmsChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceAlarms(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAlarmsChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("Alarms");
        }

        public string Alarms
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(AlarmsProperty);
            }
            set
            {
                SetValue(AlarmsProperty, value);
            }
        }
        #endregion
        #region HistorianSettings
        public static readonly DependencyProperty HistorianSettingsProperty = DependencyProperty.Register("HistorianSettings", typeof(string), typeof(PrototypeTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnHistorianSettingsChanged), new CoerceValueCallback(OnCoerceHistorianSettings)));

        private static object OnCoerceHistorianSettings(DependencyObject o, object value)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                return PrototypeTreeItemControl.OnCoerceHistorianSettings((string)value);
            else
                return value;
        }

        private static void OnHistorianSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PrototypeTreeItemControl PrototypeTreeItemControl = o as PrototypeTreeItemControl;
            if (PrototypeTreeItemControl != null)
                PrototypeTreeItemControl.OnHistorianSettingsChanged((string)e.OldValue, (string)e.NewValue);
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
        public PrototypeTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(UFUATag))
            {
                var tag = header as UFUATag;
                DataType = tag.DataType.ToString();
                MemberOrderId = tag.MemberOrderId.ToString();
                ModelType = tag.ModelType.ToString();
                PrototypeName = tag.PrototypeName;
                Alarms = tag.Alarms;
                HistorianSettings = tag.HistorianSettings;
            }
        }
        #endregion
    }
}
