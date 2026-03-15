#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Controller;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.UI.Xaml; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    internal abstract class WrapperBase : IWrapper
    {
        private SharedData _mSharedData;
        public SharedData SharedData
        {
            get { return _mSharedData; }
            protected set
            { _mSharedData = value; }
        }

        bool _mCanVirtualize = true;
        public bool CanVirtualize
        {
            get
            {
                return _mCanVirtualize;
            }
            set
            {
                if (_mCanVirtualize != value)
                {
                    _mCanVirtualize = value;
                    //OnPropertyChanged("CanVirtualize");
                }
            }
        }

        protected WrapperBase(SharedData shared)
        {
            SharedData = shared;
            SharedDataInitialized();
        }

        protected bool SetValue(string property, object value)
        {
            if (Source == null)
            {
                return false;
            }
            bool success = false;
            if (SharedData.PropertyMappingDictionary != null)
            {
                Dictionary<string, PropertyInfo> infoDict;
                if (SharedData.PropertyMappingDictionary.TryGetValue(Source.GetType(), out infoDict))
                {
                    PropertyInfo info;
                    if (infoDict.TryGetValue(property, out info))
                    {
                        info.SetValue(Source, value);
                        success = true;
                    }
                }
            }
            return success;
        }

        protected bool TryGetValue<TValue>(string property, out TValue value)
        {
            value = default(TValue);
            if (Source == null)
            {
                return false;
            }
            bool success = false;
            if (SharedData.PropertyMappingDictionary != null)
            {
                Dictionary<string, PropertyInfo> infoDict;
                if (SharedData.PropertyMappingDictionary.TryGetValue(Source.GetType(), out infoDict))
                {
                    PropertyInfo info;
                    if (infoDict.TryGetValue(property, out info))
                    {
                        value = (TValue)info.GetValue(Source);
                        success = true;
                    }
                }
            }
            return success;
        }

        protected object _mSource;

        public object Source
        {
            get
            {
                return _mSource;
            }
            protected set
            {
                if (_mSource is INotifyPropertyChanged)
                {
                    (_mSource as INotifyPropertyChanged).PropertyChanged -= KnownSource_PropertyChanged;
                    _mSource = null;
                }
                _mSource = value;
                if (_mSource is UIElement)
                {
                    CanVirtualize = false;
                }
                else
                {
                    CanVirtualize = true;
                }
                if(_mSource is INotifyPropertyChanged)
                {
                    (_mSource as INotifyPropertyChanged).PropertyChanged += KnownSource_PropertyChanged;
                }
                if (_mSource is IDiagramElement)
                {
                    SourceChanged();
                }
                else
                {
                    if (_mSource != null)
                    {
                        CheckMapping(value);
                    }
                    SourceChanged();
                }
            }
        }

        protected void CheckMapping(object source)
        {
            bool alreadyAvailable = false;
            if (SharedData.PropertyMappingDictionary != null)
            {
                if (SharedData.PropertyMappingDictionary.ContainsKey(source.GetType()))
                {
                    alreadyAvailable = true;
                }
            }
            if (!alreadyAvailable)
            {
                Type sourceType = source.GetType();
                Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
                foreach (PropertyInfo property in sourceType.GetRuntimeProperties())
                {
                    foreach (Attribute attribute in property.GetCustomAttributes())
                    {
                        if (attribute is PropertyMapping)
                        {
                            PropertyMapping att = attribute as PropertyMapping;
                            properties.Add(att.Property, property);
                        }
                    }
                }
                SharedData.PropertyMappingDictionary.Add(sourceType, properties);
            }
        }

        protected abstract void SharedDataInitialized();
        protected abstract void SourceChanged();

        void KnownSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        protected abstract void OnPropertyChanged(string propertyName);

        public virtual void Dispose()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(null, null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    internal abstract partial class DiagramElementWrapper : WrapperBase, IDiagramElement
    {
        protected DiagramElementWrapper(SharedData shared) : base(shared)
        {
        }
    }
}
