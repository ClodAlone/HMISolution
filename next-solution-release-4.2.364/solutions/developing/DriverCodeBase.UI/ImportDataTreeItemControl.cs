using System;
using System.Windows;
using System.Windows.Media;
using WPFUtilities;

namespace DriverCodeBase.UI
{
    public class ImportDataTreeItemControl : TreeItemControl
    {
        #region DP
        #region TagName
        public static readonly DependencyProperty TagNameProperty = DependencyProperty.Register("TagName", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagNameChanged), new CoerceValueCallback(OnCoerceTagName)));

        private static object OnCoerceTagName(DependencyObject o, object value)
        {
            ImportDataTreeItemControl importDataTreeItemControl = o as ImportDataTreeItemControl;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagName((String)value);
            else
                return value;
        }

        private static void OnTagNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControl importDataTreeItemControl = o as ImportDataTreeItemControl;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagNameChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagName(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagNameChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagName");
        }

        public String TagName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TagNameProperty);
            }
            set
            {
                SetValue(TagNameProperty, value);
            }
        }
        #endregion
        #region TagAddress
        public static readonly DependencyProperty TagAddressProperty = DependencyProperty.Register("TagAddress", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagAddressChanged), new CoerceValueCallback(OnCoerceTagAddress)));

        private static object OnCoerceTagAddress(DependencyObject o, object value)
        {
            ImportDataTreeItemControl importDataTreeItemControl = o as ImportDataTreeItemControl;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagAddress((String)value);
            else
                return value;
        }

        private static void OnTagAddressChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControl importDataTreeItemControl = o as ImportDataTreeItemControl;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagAddressChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagAddress(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagAddressChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagAddress");
        }

        public String TagAddress
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TagAddressProperty);
            }
            set
            {
                SetValue(TagAddressProperty, value);
            }
        }
        #endregion        
        #region TagType
        public static readonly DependencyProperty TagTypeProperty = DependencyProperty.Register("TagType", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagTypeChanged), new CoerceValueCallback(OnCoerceTagType)));

        private static object OnCoerceTagType(DependencyObject o, object value)
        {
            ImportDataTreeItemControl importDataTreeItemControl = o as ImportDataTreeItemControl;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagType((String)value);
            else
                return value;
        }

        private static void OnTagTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControl importDataTreeItemControl = o as ImportDataTreeItemControl;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagTypeChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagType(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagTypeChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagType");
        }

        public String TagType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TagTypeProperty);
            }
            set
            {
                SetValue(TagTypeProperty, value);
            }
        }
        #endregion
        #region TagDescription
        public static readonly DependencyProperty TagDescriptionProperty = DependencyProperty.Register("TagDescription", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagDescriptionChanged), new CoerceValueCallback(OnCoerceTagDescription)));

        private static object OnCoerceTagDescription(DependencyObject o, object value)
        {
            ImportDataTreeItemControl importDataTreeItemControl = o as ImportDataTreeItemControl;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagDescription((String)value);
            else
                return value;
        }

        private static void OnTagDescriptionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControl importDataTreeItemControl = o as ImportDataTreeItemControl;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagDescriptionChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagDescription(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagDescriptionChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagDescription");
        }

        public String TagDescription
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TagDescriptionProperty);
            }
            set
            {
                SetValue(TagDescriptionProperty, value);
            }
        }
        #endregion
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControl(object header, ImageSource icon = null) 
            : base(header, icon)
        {
            
            if ((header.GetType() == typeof(ImportData)) || (header.GetType().BaseType == typeof(ImportData)))
            {
                var tag = header as ImportData;
                TagName = tag.Name;
                TagAddress = tag.Address;
                TagType = tag.szType;
                TagDescription = tag.Description;

                Tag = tag;
            }
        }
        #endregion Ctor

        #region Methods
        /// <summary>
        /// Force control's properties to be updated with new values (based on user's inteface parameters (Add Station Name, ecc))
        /// </summary>
        public virtual void UpdateAllColumns()            
        {
            if (Tag as ImportData != null)
            {                
                TagName = ((ImportData)Tag).Name;
                TagAddress = ((ImportData)Tag).Address;
                TagType = ((ImportData)Tag).szType;
                TagDescription = ((ImportData)Tag).Description;
            }
        }
        #endregion
    }
}
