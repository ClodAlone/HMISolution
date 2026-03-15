using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBase.UI;

namespace LacbusPC.UI
{
    public class ImportDataTreeItemControlLacbus : ImportDataTreeItemControl
    {
        #region DP        
        #region TagCategory
        public static readonly DependencyProperty DynAddressProperty = DependencyProperty.Register("TagCategory", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagCategoryChanged), new CoerceValueCallback(OnCoerceTagCategory)));

        private static object OnCoerceTagCategory(DependencyObject o, object value)
        {
            ImportDataTreeItemControlLacbus importDataTreeItemControl = o as ImportDataTreeItemControlLacbus;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagCategory((String)value);
            else
                return value;
        }

        private static void OnTagCategoryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlLacbus importDataTreeItemControl = o as ImportDataTreeItemControlLacbus;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagCategoryChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagCategory(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagCategoryChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagCategory");
        }

        public String TagCategory
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(DynAddressProperty);
            }
            set
            {
                SetValue(DynAddressProperty, value);
            }
        }
        #endregion
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlLacbus(object header, ImageSource icon = null) 
            : base(header, icon)
        {
            
            if ((header.GetType() == typeof(ImportDataLacbus)) || (header.GetType().BaseType == typeof(ImportDataLacbus)))
            {
                var tag = header as ImportDataLacbus;
                TagCategory = tag.Category;                
            }
        }
        #endregion Ctor
    }
}
