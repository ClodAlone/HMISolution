using System;
using System.Windows;
using System.Windows.Media;
using DriverCodeBase.UI;

namespace OpcClientDriver.UI
{
    public class ImportDataTreeItemControlOpcClientDriver : ImportDataTreeItemControl
    {
        #region DP        
        #region TagArrayDim
        public static readonly DependencyProperty TagArrayProperty = DependencyProperty.Register("TagArrayDim", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagArrayDimChanged), new CoerceValueCallback(OnCoerceTagArrayDim)));

        private static object OnCoerceTagArrayDim(DependencyObject o, object value)
        {
            ImportDataTreeItemControlOpcClientDriver importDataTreeItemControl = o as ImportDataTreeItemControlOpcClientDriver;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagArrayDim((String)value);
            else
                return value;
        }

        private static void OnTagArrayDimChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlOpcClientDriver importDataTreeItemControl = o as ImportDataTreeItemControlOpcClientDriver;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagArrayDimChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagArrayDim(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagArrayDimChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagArrayDim");
        }

        public String TagArrayDim
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TagArrayProperty);
            }
            set
            {
                SetValue(TagArrayProperty, value);
            }
        }
        #endregion
        #region TagEndPoint
        public static readonly DependencyProperty TagEndPointProperty = DependencyProperty.Register("TagEndPoint", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagEndPointChanged), new CoerceValueCallback(OnCoerceTagEndPoint)));

        private static object OnCoerceTagEndPoint(DependencyObject o, object value)
        {
            ImportDataTreeItemControlOpcClientDriver importDataTreeItemControl = o as ImportDataTreeItemControlOpcClientDriver;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagEndPoint((String)value);
            else
                return value;
        }

        private static void OnTagEndPointChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlOpcClientDriver importDataTreeItemControl = o as ImportDataTreeItemControlOpcClientDriver;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagEndPointChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagEndPoint(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagEndPointChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagEndPoint");
        }

        public String TagEndPoint
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TagEndPointProperty);
            }
            set
            {
                SetValue(TagEndPointProperty, value);
            }
        }
        #endregion
        #region TagRelativePath
        public static readonly DependencyProperty TagRelativePathProperty = DependencyProperty.Register("TagRelativePath", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagRelativePathChanged), new CoerceValueCallback(OnCoerceTagRelativePath)));

        private static object OnCoerceTagRelativePath(DependencyObject o, object value)
        {
            ImportDataTreeItemControlOpcClientDriver importDataTreeItemControl = o as ImportDataTreeItemControlOpcClientDriver;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagRelativePath((String)value);
            else
                return value;
        }

        private static void OnTagRelativePathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlOpcClientDriver importDataTreeItemControl = o as ImportDataTreeItemControlOpcClientDriver;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagRelativePathChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagRelativePath(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagRelativePathChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagRelativePath");
        }

        public String TagRelativePath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TagRelativePathProperty);
            }
            set
            {
                SetValue(TagRelativePathProperty, value);
            }
        }
        #endregion
        #region TagNodeId
        public static readonly DependencyProperty NodeIdProperty = DependencyProperty.Register("TagNodeId", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagNodeIdChanged), new CoerceValueCallback(OnCoerceTagNodeId)));

        private static object OnCoerceTagNodeId(DependencyObject o, object value)
        {
            ImportDataTreeItemControlOpcClientDriver importDataTreeItemControl = o as ImportDataTreeItemControlOpcClientDriver;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagNodeId((String)value);
            else
                return value;
        }

        private static void OnTagNodeIdChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlOpcClientDriver importDataTreeItemControl = o as ImportDataTreeItemControlOpcClientDriver;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagNodeIdChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagNodeId(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagNodeIdChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagNodeId");
        }

        public String TagNodeId
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(NodeIdProperty);
            }
            set
            {
                SetValue(NodeIdProperty, value);
            }
        }
        #endregion
        #region TagAppName
        public static readonly DependencyProperty TagAppNameProperty = DependencyProperty.Register("TagAppName", typeof(String), typeof(ImportDataTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagAppNameChanged), new CoerceValueCallback(OnCoerceTagAppName)));

        private static object OnCoerceTagAppName(DependencyObject o, object value)
        {
            ImportDataTreeItemControlOpcClientDriver importDataTreeItemControl = o as ImportDataTreeItemControlOpcClientDriver;
            if (importDataTreeItemControl != null)
                return importDataTreeItemControl.OnCoerceTagAppName((String)value);
            else
                return value;
        }

        private static void OnTagAppNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportDataTreeItemControlOpcClientDriver importDataTreeItemControl = o as ImportDataTreeItemControlOpcClientDriver;
            if (importDataTreeItemControl != null)
                importDataTreeItemControl.OnTagAppNameChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTagAppName(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagAppNameChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("TagAppName");
        }

        public String TagAppName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TagAppNameProperty);
            }
            set
            {
                SetValue(TagAppNameProperty, value);
            }
        }
        #endregion
        #endregion DP

        #region Ctor
        public ImportDataTreeItemControlOpcClientDriver(object header, ImageSource icon = null) 
            : base(header, icon)
        {            
            if ((header.GetType() == typeof(ImportDataOpcClientDriver)) || (header.GetType().BaseType == typeof(ImportDataOpcClientDriver)))
            {
                var tag = header as ImportDataOpcClientDriver;
                TagArrayDim = tag.ArrayDimension.ToString();
                TagEndPoint = tag.Endpointurl;
                TagRelativePath = tag.RelativePath;
                TagNodeId = tag.NodeId;
                TagAppName = tag.AppName;

            }
        }
        #endregion Ctor
    }
}
