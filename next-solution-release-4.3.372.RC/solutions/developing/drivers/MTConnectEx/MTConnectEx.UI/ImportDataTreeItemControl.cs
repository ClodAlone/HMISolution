using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBaseEx.UI;

namespace MTConnect.UI
{
    public class ImportDataTreeItemControlMTConnect : ImportDataTreeItemControl
    {
        #region DP        
        #region TagDynAddress
        public static readonly DependencyProperty DynPathProperty = DependencyProperty.Register("TagDynPath", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagDynPathChanged), new CoerceValueCallback(OnCoerceTagDynPath)));
        public static readonly DependencyProperty DynCategoryProperty = DependencyProperty.Register("TagDynCategory", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagDynCategoryChanged), new CoerceValueCallback(OnCoerceTagDynCategory)));

        #region TagDynPath
        private static object OnCoerceTagDynPath(DependencyObject o, object value)
        {
            ImportDataTreeItemControlMTConnect importDataTreeItemControl = o as ImportDataTreeItemControlMTConnect;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagDynPath((String)value);
            else
                return value;
        }

        private static void OnTagDynPathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlMTConnect importDataTreeItemControl = o as ImportDataTreeItemControlMTConnect;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagDynPathChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagDynPath(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagDynPathChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagDynPath");
        }

        public String TagDynPath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(DynPathProperty);
            }
            set
            {
                SetValue(DynPathProperty, value);
            }
        }
        #endregion TagDynPath

        #region TagDynCategory
        private static object OnCoerceTagDynCategory(DependencyObject o, object value)
        {
            ImportDataTreeItemControlMTConnect importDataTreeItemControl = o as ImportDataTreeItemControlMTConnect;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagDynCategory((String)value);
            else
                return value;
        }

        private static void OnTagDynCategoryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlMTConnect importDataTreeItemControl = o as ImportDataTreeItemControlMTConnect;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagDynCategoryChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagDynCategory(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagDynCategoryChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagDynCategory");
        }

        public String TagDynCategory
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(DynCategoryProperty);
            }
            set
            {
                SetValue(DynCategoryProperty, value);
            }
        }
        #endregion TagDynPath

        #endregion
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlMTConnect(object header, ImageSource icon = null)
            : base(header, icon)
        {

            if ((header.GetType() == typeof(ImportDataMTConnect)) || (header.GetType().BaseType == typeof(ImportDataMTConnect)))
            {
                var tag = header as ImportDataMTConnect;
                //TagDynAddress = tag.Address;
                TagDynPath = tag.DynAddress;
                TagDynCategory = tag.Description;
            }
        }
        #endregion Ctor
    }
}
