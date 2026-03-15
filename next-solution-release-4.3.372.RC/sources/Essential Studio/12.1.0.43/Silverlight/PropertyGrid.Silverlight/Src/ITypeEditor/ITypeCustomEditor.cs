#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.PropertyGrid
{
    public interface ITypeCustomEditor
    {
        ITypeEditor Editor
        {
            get;
            set;
        }

        ObservableCollection<string> Properties
        {
            get;
            set;
        }

        Type PropertyType { get; set; }

        bool HasPropertyType { get; set; }
    }

    public class CustomEditor : ITypeCustomEditor
    {
        public CustomEditor()
        {

        }

        private ITypeEditor _Editor;
        public ITypeEditor Editor
        {
            get
            {
                return _Editor;
            }
            set
            {
                _Editor = value;
            }
        }

        private ObservableCollection<string> _Properties = new ObservableCollection<string>();
        public ObservableCollection<string> Properties
        {
            get
            {
                return _Properties;
            }
            set
            {
                _Properties = value;
            }
        }

        private Type _PropertyType = null;
        public Type PropertyType
        {
            get { return _PropertyType; }
            set { _PropertyType = value; }
        }

        private bool _HasPropertyType = false;
        public bool HasPropertyType
        {
            get { return _HasPropertyType; }
            set { _HasPropertyType = value; }
        }
    }

    public class CustomEditorCollection : ObservableCollection<CustomEditor>
    {
        public ITypeCustomEditor this[Type PropertyType]
        {
            get
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    ITypeCustomEditor item = this.Items[i] as ITypeCustomEditor;

                 
                }
                return null;
            }
        }
        
        public ITypeCustomEditor this[string PropertyName, Type PropertyType]
        {
            get
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    ITypeCustomEditor item = this.Items[i] as ITypeCustomEditor;

                    if (item.HasPropertyType)
                    {
                        if (item.PropertyType.Equals(PropertyType))
                        {
                            return this.Items[i];
                        }
                    }
                    else
                    {
                        if (item.Properties.Contains(PropertyName))
                        {
                            return this.Items[i];
                        }
                    }
                }
                return null;
            }
        }
    }
}
