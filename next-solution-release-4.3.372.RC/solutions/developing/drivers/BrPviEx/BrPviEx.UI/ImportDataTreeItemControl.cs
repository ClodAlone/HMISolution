using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBaseEx.UI;

namespace BrPvi.UI
{
    public class ImportDataTreeItemControlBrPvi : ImportDataTreeItemControl
    {
        #region DP        
        #region TagTask
        public static readonly DependencyProperty TagTaskProperty = DependencyProperty.Register("TagTask", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagTaskChanged), new CoerceValueCallback(OnCoerceTagTask)));

        private static object OnCoerceTagTask(DependencyObject o, object value)
        {
            ImportDataTreeItemControlBrPvi importDataTreeItemControl = o as ImportDataTreeItemControlBrPvi;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagTask((String)value);
            else
                return value;
        }
        private static void OnTagTaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlBrPvi importDataTreeItemControl = o as ImportDataTreeItemControlBrPvi;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagTaskChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagTask(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagTaskChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagTask");
        }

        public String TagTask
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TagTaskProperty);
            }
            set
            {
                SetValue(TagTaskProperty, value);
            }
        }
        #endregion
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlBrPvi(object header, ImageSource icon = null) 
            : base(header, icon)
        {
            if ((header.GetType() == typeof(ImportDataBrPvi)) || (header.GetType().BaseType == typeof(ImportDataBrPvi)))
            {
                var tag = header as ImportDataBrPvi;
                TagName = tag.TreeName;
                TagTask = tag.Task;
                TagType = tag.szTypeView;
            }
        }
        #endregion Ctor

        #region Methods
        /// <summary>
        /// Force control's properties to be updated with new values (based on user's inteface parameters (Add Station Name, ecc))
        /// </summary>
        public override void UpdateAllColumns() 
        {
            base.UpdateAllColumns();
            if (Tag as ImportDataBrPvi != null)
            {
                TagName = ((ImportDataBrPvi)Tag).TreeName;
                TagTask = ((ImportDataBrPvi)Tag).Task;
                TagType = ((ImportDataBrPvi)Tag).szTypeView;
            }
        }
        #endregion     
    }
}
