using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFUtilities;
using DriverCodeBaseEx.UI;

namespace BACnet.UI
{
    public class ImportDataTreeItemControlBACnet : ImportDataTreeItemControl
    {
        #region DP        
        #region TagDynAddress
        public static readonly DependencyProperty DynAddressProperty = DependencyProperty.Register("TagDynAddress", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagDynAddressChanged), new CoerceValueCallback(OnCoerceTagDynAddress)));

        private static object OnCoerceTagDynAddress(DependencyObject o, object value)
        {
            ImportDataTreeItemControlBACnet importDataTreeItemControl = o as ImportDataTreeItemControlBACnet;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagDynAddress((String)value);
            else
                return value;
        }

        private static void OnTagDynAddressChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlBACnet importDataTreeItemControl = o as ImportDataTreeItemControlBACnet;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagDynAddressChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagDynAddress(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagDynAddressChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagDynAddress");
        }

        public String TagDynAddress
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
        public ImportDataTreeItemControlBACnet(object header, ImageSource icon = null) 
            : base(header, icon)
        {
            
            if ((header.GetType() == typeof(ImportDataBACnet)) || (header.GetType().BaseType == typeof(ImportDataBACnet)))
            {
                var tag = header as ImportDataBACnet;
                TagDynAddress = tag.DynAddress;                
            }
        }
        #endregion Ctor
    }
}
