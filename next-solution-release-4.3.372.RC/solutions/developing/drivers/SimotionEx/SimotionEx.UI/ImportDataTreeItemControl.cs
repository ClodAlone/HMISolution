using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBaseEx.UI;

namespace Simotion.UI
{
    public class ImportDataTreeItemControlSimotion : ImportDataTreeItemControl
    {
        #region DP
        #region szTypeView
        public static readonly DependencyProperty TagszTypeViewProperty = DependencyProperty.Register("szTypeView", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagszTypeViewChanged), new CoerceValueCallback(OnCoerceTagszTypeView)));

        private static object OnCoerceTagszTypeView(DependencyObject o, object value)
        {
            ImportDataTreeItemControlSimotion importDataTreeItemControl = o as ImportDataTreeItemControlSimotion;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagszTypeView((String)value);
            else
                return value;
        }
        private static void OnTagszTypeViewChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlSimotion importDataTreeItemControl = o as ImportDataTreeItemControlSimotion;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagszTypeViewChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagszTypeView(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagszTypeViewChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("szTypeView");
        }

        public String szTypeView
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TagszTypeViewProperty);
            }
            set
            {
                SetValue(TagszTypeViewProperty, value);
            }
        }
        #endregion
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlSimotion(object header, ImageSource icon = null) 
            : base(header, icon)
        {
            if ((header.GetType() == typeof(ImportDataSimotion)) || (header.GetType().BaseType == typeof(ImportDataSimotion)))
            {
                var tag = header as ImportDataSimotion;                
                szTypeView = tag.szTypeView;
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
            if (Tag as ImportData != null)
            {
                szTypeView = ((ImportDataSimotion)Tag).szTypeView;
            }
        }
        #endregion     
    }
}
