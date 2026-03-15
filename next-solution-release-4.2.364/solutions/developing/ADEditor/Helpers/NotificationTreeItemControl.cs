using UFUAModel;
using WPFUtilities;
using System.Windows.Media;
using System.Windows;
using ADModel;
using System;

namespace ADEditor.Controls
{
    public class NotificationTreeItemControl : TreeItemControl
    {
        #region DP
        #region Message
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(String), typeof(NotificationTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnMessageChanged), new CoerceValueCallback(OnCoerceMessage)));

        private static object OnCoerceMessage(DependencyObject o, object value)
        {
            NotificationTreeItemControl NotificationTreeItemControl = o as NotificationTreeItemControl;
            if (NotificationTreeItemControl != null)
                return NotificationTreeItemControl.OnCoerceMessage((String)value);
            else
                return value;
        }

        private static void OnMessageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NotificationTreeItemControl NotificationTreeItemControl = o as NotificationTreeItemControl;
            if (NotificationTreeItemControl != null)
                NotificationTreeItemControl.OnMessageChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceMessage(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMessageChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("Message");
        }

        public String Message
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }
        #endregion
        #region Priority
        public static readonly DependencyProperty PriorityProperty = DependencyProperty.Register("Priority", typeof(String), typeof(NotificationTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPriorityChanged), new CoerceValueCallback(OnCoercePriority)));

        private static object OnCoercePriority(DependencyObject o, object value)
        {
            NotificationTreeItemControl NotificationTreeItemControl = o as NotificationTreeItemControl;
            if (NotificationTreeItemControl != null)
                return NotificationTreeItemControl.OnCoercePriority((String)value);
            else
                return value;
        }

        private static void OnPriorityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NotificationTreeItemControl NotificationTreeItemControl = o as NotificationTreeItemControl;
            if (NotificationTreeItemControl != null)
                NotificationTreeItemControl.OnPriorityChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoercePriority(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPriorityChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("Priority");
        }

        public String Priority
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(PriorityProperty);
            }
            set
            {
                SetValue(PriorityProperty, value);
            }
        }
        #endregion
        #region PluginName
        public static readonly DependencyProperty PluginNameProperty = DependencyProperty.Register("PluginName", typeof(String), typeof(NotificationTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPluginNameChanged), new CoerceValueCallback(OnCoercePluginName)));

        private static object OnCoercePluginName(DependencyObject o, object value)
        {
            NotificationTreeItemControl NotificationTreeItemControl = o as NotificationTreeItemControl;
            if (NotificationTreeItemControl != null)
                return NotificationTreeItemControl.OnCoercePluginName((String)value);
            else
                return value;
        }

        private static void OnPluginNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NotificationTreeItemControl NotificationTreeItemControl = o as NotificationTreeItemControl;
            if (NotificationTreeItemControl != null)
                NotificationTreeItemControl.OnPluginNameChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoercePluginName(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPluginNameChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("PluginName");
        }

        public String PluginName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(PluginNameProperty);
            }
            set
            {
                SetValue(PluginNameProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public NotificationTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(ADNotification))
            {
                var tag = header as ADNotification;
                Message = tag.Message;
                Priority = tag.Priority.ToString();
                PluginName = tag.PluginName;
            }
        }
        #endregion
    }
}
