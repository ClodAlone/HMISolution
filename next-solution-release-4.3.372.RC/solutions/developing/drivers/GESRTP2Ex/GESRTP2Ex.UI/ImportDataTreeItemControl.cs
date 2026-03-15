using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBaseEx.UI;

namespace GESRTP2.UI
{
    public class ImportDataTreeItemControlGESRTP2 : ImportDataTreeItemControl
    {
        #region DP        
        #region TagPublish
        public static readonly DependencyProperty PublishProperty = DependencyProperty.Register("TagPublish", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagPublishChanged), new CoerceValueCallback(OnCoerceTagPublish)));

        private static object OnCoerceTagPublish(DependencyObject o, object value)
        {
            ImportDataTreeItemControlGESRTP2 importDataTreeItemControl = o as ImportDataTreeItemControlGESRTP2;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagPublish((String)value);
            else
                return value;
        }

        private static void OnTagPublishChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlGESRTP2 importDataTreeItemControl = o as ImportDataTreeItemControlGESRTP2;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagPublishChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagPublish(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagPublishChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagPublish");
        }

        public String TagPublish
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(PublishProperty);
            }
            set
            {
                SetValue(PublishProperty, value);
            }
        }
        #endregion
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlGESRTP2(object header, ImageSource icon = null) 
            : base(header, icon)
        {

            if ((header.GetType() == typeof(ImportDataGESRTP2)) || (header.GetType().BaseType == typeof(ImportDataGESRTP2)))
            {
                var tag = header as ImportDataGESRTP2;
                TagPublish = tag.Publish;
            }
        }
        #endregion Ctor
    }
}
