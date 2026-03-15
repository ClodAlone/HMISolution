#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if !WINDOWS_PHONE
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif
namespace Syncfusion.Data
{
    public abstract class GroupDescription : INotifyPropertyChanged
    {
        private ObservableCollection<object> _explicitGroupNames = new ObservableCollection<object>();

        protected event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                this.PropertyChanged += value;
            }
            remove
            {
                this.PropertyChanged -= value;
            }
        }

        protected GroupDescription()
        {
            this._explicitGroupNames.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupNamesChanged);
        }

        private void OnGroupNamesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("GroupNames"));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        public bool ShouldSerializeGroupNames()
        {
            return (this._explicitGroupNames.Count > 0);
        }

        public ObservableCollection<object> GroupNames
        {
            get
            {
                return this._explicitGroupNames;
            }
        }
    }

    public class PropertyGroupDescription : GroupDescription
    {
        private IValueConverter _converter;
        private string _propertyName;
        private PropertyPath _propertyPath;
        private StringComparison _stringComparison;

        public PropertyGroupDescription()
        {
            this._stringComparison = StringComparison.Ordinal;
        }

        public PropertyGroupDescription(string propertyName)
        {
            this._stringComparison = StringComparison.Ordinal;
            this.UpdatePropertyName(propertyName);
        }

        public PropertyGroupDescription(string propertyName, IValueConverter converter)
        {
            this._stringComparison = StringComparison.Ordinal;
            this.UpdatePropertyName(propertyName);
            this._converter = converter;
        }

        public PropertyGroupDescription(string propertyName, IValueConverter converter, StringComparison stringComparison)
        {
            this._stringComparison = StringComparison.Ordinal;
            this.UpdatePropertyName(propertyName);
            this._converter = converter;
            this._stringComparison = stringComparison;
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void UpdatePropertyName(string propertyName)
        {
            this._propertyName = propertyName;
            this._propertyPath = !string.IsNullOrEmpty(propertyName) ? new PropertyPath(propertyName) : null;
        }

        public IValueConverter Converter
        {
            get
            {
                return this._converter;
            }
            set
            {
                this._converter = value;
                this.OnPropertyChanged("Converter");
            }
        }

        public string PropertyName
        {
            get
            {
                return this._propertyName;
            }
            set
            {
                this.UpdatePropertyName(value);
                this.OnPropertyChanged("PropertyName");
            }
        }

        public StringComparison StringComparison
        {
            get
            {
                return this._stringComparison;
            }
            set
            {
                this._stringComparison = value;
                this.OnPropertyChanged("StringComparison");
            }
        }
    }
}
