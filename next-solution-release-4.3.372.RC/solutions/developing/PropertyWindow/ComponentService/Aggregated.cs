using System;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using System.Windows.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Utilities;
using System.Windows.Controls;
using Mindscape.WpfElements.WpfPropertyGrid;
using PropertyControl.PropertyDataTemplate;
using Mindscape.WpfElements.PropertyEditing;
using UriResolver.ComponentService;
using DocumentManager.ComponentService;
using DataReader;
using Converters;
using UIMsgBoxAlertService.ComponentService;
using HelpProvider.ComponentService;
using WPFUtilities.PropertyDataTemplate;
using PropertyControl.Localization;
using UFInterfaces.PropertyControl;
using System.Threading.Tasks;

namespace PropertyControl.ComponentService
{
    public class Aggregated : ICustomTypeDescriptor, IDataErrorInfo, IDisposable
    {
        #region Declarations
        readonly Object _main;
        readonly List<Object> _extended;
        readonly List<Object> _parents;
        readonly PropertyControlUI propertyControlUI;

        readonly List<INotifyPropertyVisibilityChanged> visibilityChangedObjects;
        readonly List<INotifyPropertyReadOnlyChanged> readOnlyChangedObjects;

        Dictionary<PropertyDescriptor, Object> _mainPropsAttribute;
        Dictionary<PropertyDescriptor, Object> _mainProps;

        Dictionary<PropertyDescriptor, Object> _extendedPropsAttribute;
        Dictionary<PropertyDescriptor, Object> _extendedProps;

        Dictionary<PropertyDescriptor, Object> _parentsPropsAttribute;
        Dictionary<PropertyDescriptor, Object> _parentsProps;

        bool enableDependencyProperties;
        bool projHasChildren;
        public bool HasChildren
        {
            get { return projHasChildren; }
        }

        DispatcherOperation dpGridRefresh;
        #endregion

        #region Constructors
        Aggregated(Aggregated aggregated)
        {
            _main = aggregated._main;
            _extended = aggregated._extended;
            _parents = aggregated._parents;
        }

        public Aggregated(PropertyControlUI owner, Object main, bool projectHasChildren, IList<Object> extended = null, IList<Object> parents = null)
        {
            _main = main;
            if (extended != null)
                _extended = new List<Object>(extended);
            if (parents != null)
                _parents = new List<Object>(parents);
            propertyControlUI = owner;

            if (_main is IContainPropertyEditors)
            {
                PropertyControlComponent.propertyControlComponent.AddToolBoxPropertyEditors(_main as IContainPropertyEditors);
            }

            var notifylist = new List<Object>();
            notifylist.Add(_main);
            if (_extended != null)
                notifylist.AddRange(_extended);

            visibilityChangedObjects = (from c in notifylist.AsParallel()
                                        where c is INotifyPropertyVisibilityChanged
                                        select c as INotifyPropertyVisibilityChanged).ToList();

            readOnlyChangedObjects = (from c in notifylist.AsParallel()
                                      where c is INotifyPropertyReadOnlyChanged
                                      select c as INotifyPropertyReadOnlyChanged).ToList();

            Parallel.ForEach(visibilityChangedObjects, notifyObject =>
            {
                notifyObject.PropertyVisiblityChanged += Aggregated_PropertyInvalidateContent;
            });

            Parallel.ForEach(readOnlyChangedObjects, notifyObject =>
            {
                notifyObject.PropertyReadOnlyChanged += Aggregated_PropertyInvalidateContent;
            });
            projHasChildren = projectHasChildren;
        }

        private void Aggregated_PropertyInvalidateContent(object sender, PropertyChangedEventArgs e)
        {
            InvalidateTypeDescriptor();

            if (dpGridRefresh == null ||
                dpGridRefresh.Status == DispatcherOperationStatus.Completed ||
                dpGridRefresh.Status == DispatcherOperationStatus.Aborted)
            {
                dpGridRefresh = propertyControlUI.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    propertyControlUI.propertyGrid.Refresh();
                });
            }
        }
        #endregion

        public Object Main
        {
            get
            {
                return _main;
            }
        }

        internal void InvalidateTypeDescriptor()
        {
            if (retAttributes != null)
            {
                ClearTypeDescriptorCache(retAttributes);
                retAttributes.Clear();
                retAttributes = null;
            }

            if (ret != null)
            {
                ClearTypeDescriptorCache(ret);
                ret.Clear();
                ret = null;
            }
        }

        void ClearTypeDescriptorCache(PropertyDescriptorCollection properties)
        {
            foreach (PropertyDescriptor prop in properties)
            {
                TypeDescriptor.Refresh(prop.PropertyType);
            }
        }

        internal Aggregated CreateDuplicatedInstanceAndEnableDependencyProperties()
        {
            return new Aggregated(this) { enableDependencyProperties = true };
        }

        bool IsPropertyVisible(PropertyDescriptor prop, object source)
        {
            if (propertyControlUI != null)
                return propertyControlUI.IsPropertyVisible(prop, source);
            else
                return true;
        }

        bool IsBindingExpression(PropertyDescriptor prop, object source)
        {
            DependencyObject depObject = null;
            if (source is ICustomTypeDescriptor)
            {
                var typeDesc = source as ICustomTypeDescriptor;
                depObject = typeDesc.GetPropertyOwner(prop) as DependencyObject;
            }
            else if (source is DependencyObject)
                depObject = source as DependencyObject;

            if (depObject != null)
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(prop);
                if (dpd != null)
                {
                    var depValue = depObject.ReadLocalValue(dpd.DependencyProperty);
                    if (depValue is System.Windows.Data.BindingExpression)
                        return true;
                }
            }

            return false;
        }

        bool IsContentControlContent()
        {
            if (_parents != null && _main is UIElement)
            {
                foreach (var obj in _parents)
                {
                    if (obj is ContentControl && (obj as ContentControl).Content == _main)
                        return true;
                }
            }

            return false;
        }

        #region ICustomTypeDescriptor implementation

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        LocalizablePropertyDescriptorCollection retAttributes;
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            if (retAttributes != null)
                return retAttributes;

            retAttributes = new LocalizablePropertyDescriptorCollection(projHasChildren);

            var listPropName = new List<String>();

            bool isContentControlContent = IsContentControlContent();
            var properties = TypeDescriptor.GetProperties(_main, attributes, true);
            _mainPropsAttribute = new Dictionary<PropertyDescriptor, object>();
            foreach (PropertyDescriptor prop in properties)
            {
                if (_mainPropsAttribute.ContainsKey(prop) || !IsPropertyVisible(prop, _main) || IsBindingExpression(prop, _main))
                    continue;

                if (isContentControlContent && PropertyControlUI.IsValidAttachedProperty(prop.Name))
                    continue;

                listPropName.Add(prop.Name);
                _mainPropsAttribute.Add(prop, _main);
                if (enableDependencyProperties || !(_main is INotifyPropertyVisibilityChanged) || (_main as INotifyPropertyVisibilityChanged)[prop.Name])
                {
                    bool bReadOnly = (_main is INotifyPropertyReadOnlyChanged) && (_main as INotifyPropertyReadOnlyChanged)[prop.Name];
                    retAttributes.Insert(prop, bReadOnly);
                }
            }

            if (_extended != null)
            {
                _extendedPropsAttribute = new Dictionary<PropertyDescriptor, Object>();
                foreach (var objext in _extended)
                {
                    properties = TypeDescriptor.GetProperties(objext, attributes, true);
                    foreach (PropertyDescriptor prop in properties)
                    {
                        if (_extendedPropsAttribute.ContainsKey(prop) || !IsPropertyVisible(prop, objext) || IsBindingExpression(prop, objext))
                            continue;

                        listPropName.Add(prop.Name);
                        _extendedPropsAttribute.Add(prop, objext);
                        if (enableDependencyProperties || !(objext is INotifyPropertyVisibilityChanged) || (objext as INotifyPropertyVisibilityChanged)[prop.Name])
                        {
                            bool bReadOnly = (_main is INotifyPropertyReadOnlyChanged) && (_main as INotifyPropertyReadOnlyChanged)[prop.Name];
                            retAttributes.Insert(prop, bReadOnly);
                        }
                    }
                }
            }

            if (_parents != null)
            {
                _parentsPropsAttribute = new Dictionary<PropertyDescriptor, Object>();
                foreach (var objext in _parents)
                {
                    properties = TypeDescriptor.GetProperties(objext, attributes, true);
                    foreach (PropertyDescriptor prop in properties)
                    {
                        if (listPropName.Contains(prop.Name) || 
                            _parentsPropsAttribute.ContainsKey(prop) ||
                            !IsPropertyVisible(prop, objext) ||
                            IsBindingExpression(prop, objext))
                            continue;

                        _parentsPropsAttribute.Add(prop, objext);
                        if (enableDependencyProperties || !(objext is INotifyPropertyVisibilityChanged) || (objext as INotifyPropertyVisibilityChanged)[prop.Name])
                        {
                            bool bReadOnly = (_main is INotifyPropertyReadOnlyChanged) && (_main as INotifyPropertyReadOnlyChanged)[prop.Name];
                            retAttributes.Insert(prop, bReadOnly);
                        }
                    }
                }
            }

            // The following lines prevent a memory leak because the Dictionary<object, DependencyPropertyDescriptor> called 'DependencyPropertyDescriptor._cahce'
            // is never cleared and will be filled with a new 'LocalizablePropertyDescriptor' element.
            Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            {
                if (retAttributes != null)
                {
                    ClearTypeDescriptorCache(retAttributes);
                }
            });

            return retAttributes;
        }

        LocalizablePropertyDescriptorCollection ret;
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            if (ret != null)
                return ret;

            ret = new LocalizablePropertyDescriptorCollection(projHasChildren);

            var listPropName = new List<String>();

            bool isContentControlContent = IsContentControlContent();
            var properties = TypeDescriptor.GetProperties(_main, true);
            _mainProps = new Dictionary<PropertyDescriptor, object>();
            foreach (PropertyDescriptor prop in properties)
            {
                if (_mainProps.ContainsKey(prop) || !IsPropertyVisible(prop, _main) || IsBindingExpression(prop, _main))
                    continue;

                if (isContentControlContent && PropertyControlUI.IsValidAttachedProperty(prop.Name))
                    continue;

                listPropName.Add(prop.Name);
                _mainProps.Add(prop, _main);
                if (enableDependencyProperties || !(_main is INotifyPropertyVisibilityChanged) || (_main as INotifyPropertyVisibilityChanged)[prop.Name])
                {
                    bool bReadOnly = (_main is INotifyPropertyReadOnlyChanged) && (_main as INotifyPropertyReadOnlyChanged)[prop.Name];
                    ret.Insert(prop, bReadOnly);
                }
            }

            if (_extended != null)
            {
                _extendedProps = new Dictionary<PropertyDescriptor, object>();
                foreach (var objext in _extended)
                {
                    properties = TypeDescriptor.GetProperties(objext, true);
                    foreach (PropertyDescriptor prop in properties)
                    {
                        if (_extendedProps.ContainsKey(prop) || !IsPropertyVisible(prop, objext) || IsBindingExpression(prop, objext))
                            continue;

                        listPropName.Add(prop.Name);
                        _extendedProps.Add(prop, objext);
                        if (enableDependencyProperties || !(objext is INotifyPropertyVisibilityChanged) || (objext as INotifyPropertyVisibilityChanged)[prop.Name])
                        {
                            bool bReadOnly = (_main is INotifyPropertyReadOnlyChanged) && (_main as INotifyPropertyReadOnlyChanged)[prop.Name];
                            ret.Insert(prop, bReadOnly);
                        }
                    }
                }
            }

            if (_parents != null)
            {
                _parentsProps = new Dictionary<PropertyDescriptor, object>();
                foreach (var objext in _parents)
                {
                    properties = TypeDescriptor.GetProperties(objext, true);
                    foreach (PropertyDescriptor prop in properties)
                    {
                        if (listPropName.Contains(prop.Name) ||
                            _parentsProps.ContainsKey(prop) ||
                            !IsPropertyVisible(prop, objext) ||
                            IsBindingExpression(prop, objext))
                            continue;

                        _parentsProps.Add(prop, objext);
                        if (enableDependencyProperties || !(objext is INotifyPropertyVisibilityChanged) || (objext as INotifyPropertyVisibilityChanged)[prop.Name])
                        {
                            bool bReadOnly = (_main is INotifyPropertyReadOnlyChanged) && (_main as INotifyPropertyReadOnlyChanged)[prop.Name];
                            ret.Insert(prop, bReadOnly);
                        }
                    }
                }
            }

            // The following lines prevent a memory leak because the Dictionary<object, DependencyPropertyDescriptor> called 'DependencyPropertyDescriptor._cahce'
            // is never cleared and will be filled with a new 'LocalizablePropertyDescriptor' element.
            Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            {
                if (ret != null)
                {
                    ClearTypeDescriptorCache(ret);
                }
            });

            return ret;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            if (_mainProps != null && _mainProps.ContainsKey(pd))
                return _mainProps[pd];
            else if (_mainPropsAttribute != null && _mainPropsAttribute.ContainsKey(pd))
                return _mainPropsAttribute[pd];
            else if (_extendedProps != null && _extendedProps.ContainsKey(pd))
                return _extendedProps[pd];
            else if (_extendedPropsAttribute != null && _extendedPropsAttribute.ContainsKey(pd))
                return _extendedPropsAttribute[pd];
            else if (_parentsProps != null && _parentsProps.ContainsKey(pd))
                return _parentsProps[pd];
            else if (_parentsPropsAttribute != null && _parentsPropsAttribute.ContainsKey(pd))
                return _parentsPropsAttribute[pd];

            return this;
        }

        #endregion

        #region IDataErrorInfo Members

        public string Error
        {
            get
            {
                string ret = null;
                if (_main is IDataErrorInfo)
                {
                    var obj = _main as IDataErrorInfo;
                    ret = obj.Error;
                }

                if (_extended != null)
                {
                    foreach (var objext in _extended)
                    {
                        if (ret != null && objext is IDataErrorInfo)
                        {
                            var obj = objext as IDataErrorInfo;
                            ret = obj.Error;
                        }
                    }
                }

                if (_parents != null)
                {
                    foreach (var objext in _parents)
                    {
                        if (ret != null && objext is IDataErrorInfo)
                        {
                            var obj = objext as IDataErrorInfo;
                            ret = obj.Error;
                        }
                    }
                }

                return ret;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string ret = null;
                if (_main is IDataErrorInfo)
                {
                    var obj = _main as IDataErrorInfo;
                    ret = obj[columnName];
                }

                if (String.IsNullOrEmpty(ret) && _extended != null)
                {
                    foreach (var objext in _extended)
                    {
                        if (!String.IsNullOrEmpty(ret))
                            break;

                        if (objext is IDataErrorInfo)
                        {
                            var obj = objext as IDataErrorInfo;
                            ret = obj[columnName];
                        }
                    }
                }

                if (String.IsNullOrEmpty(ret) && _parents != null)
                {
                    foreach (var objext in _parents)
                    {
                        if (!String.IsNullOrEmpty(ret))
                            break;

                        if (objext is IDataErrorInfo)
                        {
                            var obj = objext as IDataErrorInfo;
                            ret = obj[columnName];
                        }
                    }
                }

                return ret;
            }
        }

        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            InvalidateTypeDescriptor();

            if (visibilityChangedObjects != null)
            {
                Parallel.ForEach(visibilityChangedObjects, notifyObject =>
                {
                    notifyObject.PropertyVisiblityChanged -= Aggregated_PropertyInvalidateContent;
                });
            }

            if (visibilityChangedObjects != null)
            {
                Parallel.ForEach(readOnlyChangedObjects, notifyObject =>
                {
                    notifyObject.PropertyReadOnlyChanged -= Aggregated_PropertyInvalidateContent;
                });
            }

            if (dpGridRefresh != null &&
                dpGridRefresh.Status != DispatcherOperationStatus.Aborted &&
                dpGridRefresh.Status != DispatcherOperationStatus.Completed)
                dpGridRefresh.Abort();
        }
        #endregion
    }
}
