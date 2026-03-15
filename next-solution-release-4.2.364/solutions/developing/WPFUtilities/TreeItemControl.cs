using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFUtilities
{
    public class TreeItemControl : HeaderedItemsControl, INotifyPropertyChanged
    {
        #region DP
        #region ItemHeader
        public static readonly DependencyProperty ItemHeaderProperty = DependencyProperty.Register("ItemHeader", typeof(object), typeof(TreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnItemHeaderChanged), new CoerceValueCallback(OnCoerceItemHeader)));

        private static object OnCoerceItemHeader(DependencyObject o, object value)
        {
            TreeItemControl TreeItemControl = o as TreeItemControl;
            if (TreeItemControl != null)
                return TreeItemControl.OnCoerceItemHeader((object)value);
            else
                return value;
        }

        private static void OnItemHeaderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TreeItemControl TreeItemControl = o as TreeItemControl;
            if (TreeItemControl != null)
                TreeItemControl.OnItemHeaderChanged((object)e.OldValue, (object)e.NewValue);
        }

        protected virtual object OnCoerceItemHeader(object value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnItemHeaderChanged(object oldValue, object newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (Header != newValue)
            {
                Header = newValue;
                OnPropertyChanged("ItemHeader");
            }
        }

        public object ItemHeader
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return GetValue(ItemHeaderProperty);
            }
            set
            {
                SetValue(ItemHeaderProperty, value);
            }
        }
        #endregion
        #endregion
        public bool IsNodeExpanding { get; set; }
        public bool IsOpenable { get; set; }
        //public bool IsNodeRefreshing { get; set; }
        public ImageSource resourceIcon;
        public ImageSource ResourceIcon
        {
            get
            {
                return resourceIcon;
            }

            set
            {
                if (value != resourceIcon)
                {
                    resourceIcon = value;
                    OnPropertyChanged("ResourceIcon");
                }
            }
        }

        UserControl innerControl;
        public UserControl InnerControl {
            get
            {
                return innerControl;
            }
            set 
            {
                if (value != innerControl)
                {
                    innerControl = value;
                    OnPropertyChanged("InnerControl");
                }
            } 
        }
        
        public object TreeItemInnerObject { get; private set; }

        public TreeItemControl(object header, ImageSource icon = null)
        {
            TreeItemInnerObject = header;
            ItemHeader = header as string;
            ResourceIcon = icon;
        }

        public TreeItemControl(object header, string name, ImageSource icon = null)
        {
            TreeItemInnerObject = header;
            ItemHeader = name;
            ResourceIcon = icon;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return ItemHeader?.ToString();
        }
        #endregion
    }
}
