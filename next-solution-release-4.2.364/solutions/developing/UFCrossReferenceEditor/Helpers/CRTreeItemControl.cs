using UFUAModel;
using WPFUtilities;
using System.Windows.Media;
using System.Windows;
using System;
using UFCrossReferenceModel;

namespace UFCrossReferenceEditor
{
    public class CRTagsTreeItemControl : TreeItemControl
    {
        #region DP
        #region ReadablePath
        public static readonly DependencyProperty ReadablePathProperty = DependencyProperty.Register("ReadablePath", typeof(string), typeof(CRTagsTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnReadablePathChanged), new CoerceValueCallback(OnCoerceReadablePath)));

        private static object OnCoerceReadablePath(DependencyObject o, object value)
        {
            CRTagsTreeItemControl CRTagsTreeItemControl = o as CRTagsTreeItemControl;
            if (CRTagsTreeItemControl != null)
                return CRTagsTreeItemControl.OnCoerceReadablePath((string)value);
            else
                return value;
        }

        private static void OnReadablePathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CRTagsTreeItemControl CRTagsTreeItemControl = o as CRTagsTreeItemControl;
            if (CRTagsTreeItemControl != null)
                CRTagsTreeItemControl.OnReadablePathChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceReadablePath(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnReadablePathChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string ReadablePath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ReadablePathProperty);
            }
            set
            {
                SetValue(ReadablePathProperty, value);
            }
        }
        #endregion

        #region EndpointUrl
        public static readonly DependencyProperty EndpointUrlProperty = DependencyProperty.Register("EndpointUrl", typeof(string), typeof(CRTagsTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnEndpointUrlChanged), new CoerceValueCallback(OnCoerceEndpointUrl)));

        private static object OnCoerceEndpointUrl(DependencyObject o, object value)
        {
            CRTagsTreeItemControl control = o as CRTagsTreeItemControl;
            if (control != null)
                return control.OnCoerceEndpointUrl((string)value);
            else
                return value;
        }

        private static void OnEndpointUrlChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CRTagsTreeItemControl control = o as CRTagsTreeItemControl;
            if (control != null)
                control.OnEndpointUrlChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceEndpointUrl(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEndpointUrlChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string EndpointUrl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(EndpointUrlProperty);
            }
            set
            {
                SetValue(EndpointUrlProperty, value);
            }
        }

        #endregion

        #region IsNotInUse
        public static readonly DependencyProperty IsNotInUseProperty = DependencyProperty.Register("IsNotInUse", typeof(bool), typeof(CRTagsTreeItemControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsNotInUseChanged), new CoerceValueCallback(OnCoerceIsNotInUse)));

        private static object OnCoerceIsNotInUse(DependencyObject o, object value)
        {
            CRTagsTreeItemControl control = o as CRTagsTreeItemControl;
            if (control != null)
                return control.OnCoerceIsNotInUse((bool)value);
            else
                return value;
        }

        private static void OnIsNotInUseChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CRTagsTreeItemControl control = o as CRTagsTreeItemControl;
            if (control != null)
                control.OnIsNotInUseChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceIsNotInUse(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsNotInUseChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool IsNotInUse
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsNotInUseProperty);
            }
            set
            {
                SetValue(IsNotInUseProperty, value);
            }
        }

        #endregion

        #region IsNotValid
        public static readonly DependencyProperty IsNotValidProperty = DependencyProperty.Register("IsNotValid", typeof(bool), typeof(CRTagsTreeItemControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsNotValidChanged), new CoerceValueCallback(OnCoerceIsNotValid)));

        private static object OnCoerceIsNotValid(DependencyObject o, object value)
        {
            CRTagsTreeItemControl CRTagsTreeItemControl = o as CRTagsTreeItemControl;
            if (CRTagsTreeItemControl != null)
                return CRTagsTreeItemControl.OnCoerceIsNotValid((bool)value);
            else
                return value;
        }

        private static void OnIsNotValidChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CRTagsTreeItemControl CRTagsTreeItemControl = o as CRTagsTreeItemControl;
            if (CRTagsTreeItemControl != null)
                CRTagsTreeItemControl.OnIsNotValidChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceIsNotValid(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsNotValidChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool IsNotValid
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsNotValidProperty);
            }
            set
            {
                SetValue(IsNotValidProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public CRTagsTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(UFCrossReferenceTag))
            {
                var tag = header as UFCrossReferenceTag;
                ReadablePath = tag.ReadablePath;
                IsNotValid = tag.IsNotValid;
                //Container = tag.Container;
            }
        }
        #endregion
    }

    public class CRScreenTreeItemControl : TreeItemControl
    {
        #region DP
        #region ReadablePath
        public static readonly DependencyProperty ReadablePathProperty = DependencyProperty.Register("ReadablePath", typeof(string), typeof(CRScreenTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnReadablePathChanged), new CoerceValueCallback(OnCoerceReadablePath)));

        private static object OnCoerceReadablePath(DependencyObject o, object value)
        {
            CRScreenTreeItemControl CRScreenTreeItemControl = o as CRScreenTreeItemControl;
            if (CRScreenTreeItemControl != null)
                return CRScreenTreeItemControl.OnCoerceReadablePath((string)value);
            else
                return value;
        }

        private static void OnReadablePathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CRScreenTreeItemControl CRScreenTreeItemControl = o as CRScreenTreeItemControl;
            if (CRScreenTreeItemControl != null)
                CRScreenTreeItemControl.OnReadablePathChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceReadablePath(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnReadablePathChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string ReadablePath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ReadablePathProperty);
            }
            set
            {
                SetValue(ReadablePathProperty, value);
            }
        }
        #endregion
        #region IsNotValid
        public static readonly DependencyProperty IsNotValidProperty = DependencyProperty.Register("IsNotValid", typeof(bool), typeof(CRScreenTreeItemControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsNotValidChanged), new CoerceValueCallback(OnCoerceIsNotValid)));

        private static object OnCoerceIsNotValid(DependencyObject o, object value)
        {
            CRScreenTreeItemControl CRScreenTreeItemControl = o as CRScreenTreeItemControl;
            if (CRScreenTreeItemControl != null)
                return CRScreenTreeItemControl.OnCoerceIsNotValid((bool)value);
            else
                return value;
        }

        private static void OnIsNotValidChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CRScreenTreeItemControl CRScreenTreeItemControl = o as CRScreenTreeItemControl;
            if (CRScreenTreeItemControl != null)
                CRScreenTreeItemControl.OnIsNotValidChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceIsNotValid(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsNotValidChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool IsNotValid
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsNotValidProperty);
            }
            set
            {
                SetValue(IsNotValidProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public CRScreenTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(UFCrossReferenceScreen))
            {
                var tag = header as UFCrossReferenceScreen;
                ReadablePath = tag.ReadablePath;
                IsNotValid = tag.IsNotValid;
                //Container = tag.Container;
            }
        }
        #endregion
    }

    public class CRConnectionTreeItemControl : TreeItemControl
    {
        #region DP

        #endregion

        #region Ctor
        public CRConnectionTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(UFCrossReferenceConnection))
            {
                var tag = header as UFCrossReferenceConnection;
                //EntityReadablePath = tag.EntityReadablePath;
                //Container = tag.Container;
            }
        }
        #endregion
    }

    public class CREntityTreeItemControl : TreeItemControl
    {
        #region DP
        #region Container
        public static readonly DependencyProperty ContainerProperty = DependencyProperty.Register("Container", typeof(string), typeof(CREntityTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnContainerChanged), new CoerceValueCallback(OnCoerceContainer)));

        private static object OnCoerceContainer(DependencyObject o, object value)
        {
            CREntityTreeItemControl CREntityTreeItemControl = o as CREntityTreeItemControl;
            if (CREntityTreeItemControl != null)
                return CREntityTreeItemControl.OnCoerceContainer((string)value);
            else
                return value;
        }

        private static void OnContainerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CREntityTreeItemControl CREntityTreeItemControl = o as CREntityTreeItemControl;
            if (CREntityTreeItemControl != null)
                CREntityTreeItemControl.OnContainerChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceContainer(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnContainerChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string Container
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ContainerProperty);
            }
            set
            {
                SetValue(ContainerProperty, value);
            }
        }
        #endregion

        #region EndpointUrl
        public static readonly DependencyProperty EndpointUrlProperty = DependencyProperty.Register("EndpointUrl", typeof(string), typeof(CREntityTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnEndpointUrlChanged), new CoerceValueCallback(OnCoerceEndpointUrl)));

        private static object OnCoerceEndpointUrl(DependencyObject o, object value)
        {
            CREntityTreeItemControl control = o as CREntityTreeItemControl;
            if (control != null)
                return control.OnCoerceEndpointUrl((string)value);
            else
                return value;
        }

        private static void OnEndpointUrlChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CREntityTreeItemControl control = o as CREntityTreeItemControl;
            if (control != null)
                control.OnEndpointUrlChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceEndpointUrl(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEndpointUrlChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string EndpointUrl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(EndpointUrlProperty);
            }
            set
            {
                SetValue(EndpointUrlProperty, value);
            }
        }

        #endregion

        #region EntityReadablePath
        public static readonly DependencyProperty EntityReadablePathProperty = DependencyProperty.Register("EntityReadablePath", typeof(string), typeof(CREntityTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnEntityReadablePathChanged), new CoerceValueCallback(OnCoerceEntityReadablePath)));

        private static object OnCoerceEntityReadablePath(DependencyObject o, object value)
        {
            CREntityTreeItemControl CREntityTreeItemControl = o as CREntityTreeItemControl;
            if (CREntityTreeItemControl != null)
                return CREntityTreeItemControl.OnCoerceEntityReadablePath((string)value);
            else
                return value;
        }

        private static void OnEntityReadablePathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CREntityTreeItemControl CREntityTreeItemControl = o as CREntityTreeItemControl;
            if (CREntityTreeItemControl != null)
                CREntityTreeItemControl.OnEntityReadablePathChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceEntityReadablePath(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEntityReadablePathChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string EntityReadablePath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(EntityReadablePathProperty);
            }
            set
            {
                SetValue(EntityReadablePathProperty, value);
            }
        }
        #endregion
        #region TagName
        public static readonly DependencyProperty TagNameProperty = DependencyProperty.Register("TagName", typeof(string), typeof(CREntityTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagNameChanged), new CoerceValueCallback(OnCoerceTagName)));

        private static object OnCoerceTagName(DependencyObject o, object value)
        {
            CREntityTreeItemControl CREntityTreeItemControl = o as CREntityTreeItemControl;
            if (CREntityTreeItemControl != null)
                return CREntityTreeItemControl.OnCoerceTagName((string)value);
            else
                return value;
        }

        private static void OnTagNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CREntityTreeItemControl CREntityTreeItemControl = o as CREntityTreeItemControl;
            if (CREntityTreeItemControl != null)
                CREntityTreeItemControl.OnTagNameChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceTagName(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagNameChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string TagName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TagNameProperty);
            }
            set
            {
                SetValue(TagNameProperty, value);
            }
        }
        #endregion

        #region IsNotRefUsed
        public static readonly DependencyProperty IsNotRefUsedProperty = DependencyProperty.Register("IsNotRefUsed", typeof(bool), typeof(CREntityTreeItemControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsNotRefUsedChanged), new CoerceValueCallback(OnCoerceIsNotRefUsed)));

        private static object OnCoerceIsNotRefUsed(DependencyObject o, object value)
        {
            CREntityTreeItemControl control = o as CREntityTreeItemControl;
            if (control != null)
                return control.OnCoerceIsNotRefUsed((bool)value);
            else
                return value;
        }

        private static void OnIsNotRefUsedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CREntityTreeItemControl control = o as CREntityTreeItemControl;
            if (control != null)
                control.OnIsNotRefUsedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceIsNotRefUsed(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsNotRefUsedChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool IsNotRefUsed
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsNotRefUsedProperty);
            }
            set
            {
                SetValue(IsNotRefUsedProperty, value);
            }
        }

        #endregion

        #region IsNotRefValid
        public static readonly DependencyProperty IsNotRefValidProperty = DependencyProperty.Register("IsNotRefValid", typeof(bool), typeof(CREntityTreeItemControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsNotRefValidChanged), new CoerceValueCallback(OnCoerceIsNotRefValid)));

        private static object OnCoerceIsNotRefValid(DependencyObject o, object value)
        {
            CREntityTreeItemControl CREntityTreeItemControl = o as CREntityTreeItemControl;
            if (CREntityTreeItemControl != null)
                return CREntityTreeItemControl.OnCoerceIsNotRefValid((bool)value);
            else
                return value;
        }

        private static void OnIsNotRefValidChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CREntityTreeItemControl CREntityTreeItemControl = o as CREntityTreeItemControl;
            if (CREntityTreeItemControl != null)
                CREntityTreeItemControl.OnIsNotRefValidChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceIsNotRefValid(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsNotRefValidChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool IsNotRefValid
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsNotRefValidProperty);
            }
            set
            {
                SetValue(IsNotRefValidProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public CREntityTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(UFCrossReferenceEntity))
            {
                var tag = header as UFCrossReferenceEntity;
                TagName = tag.TagName;
                IsNotRefValid = tag.IsNotRefValid;
                EntityReadablePath = tag.EntityReadablePath;
                Container = tag.Container;
            }
        }
        #endregion
    }
}
