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
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Syncfusion.Windows.PropertyGrid
{
    public class PropertyItem : DependencyObject, IDataErrorInfo, INotifyPropertyChanged
    {
        public PropertyItem()
        {
            CategoryValueProperties = new PropertyItemCollection();
        }

        /// <summary>
        /// Gets or sets the property grid.
        /// </summary>
        /// <value>The property grid.</value>
        public PropertyGrid PropertyGrid
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public object SelectedObject
        {
            get { return (object)GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyItem), new PropertyMetadata(null,OnSelectedObjectChanged));


        private static void OnSelectedObjectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PropertyItem pitem = obj as PropertyItem;
            pitem.OnSelectedObjectChanged(args);
        }

        protected void OnSelectedObjectChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Editor != null && PropertyGrid.isOldType)
            {
                if (this.CanWrite)
                {
                    try
                    {
                        Binding bind = new Binding(this.Name);
                        bind.Source = this.SelectedObject;
                        bind.ValidatesOnExceptions = true;
                        bind.ValidatesOnDataErrors = true;
                        bind.Mode = BindingMode.TwoWay;                       
                        BindingOperations.SetBinding(this, PropertyItem.ValueProperty, bind);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        Binding bind = new Binding(this.Name);
                        bind.Source = this.SelectedObject;
                        bind.ValidatesOnExceptions = true;
                        bind.ValidatesOnDataErrors = true;
                        bind.Mode = BindingMode.OneWay;                       
                        BindingOperations.SetBinding(this, PropertyItem.ValueProperty, bind);
                    }
                    catch { }
                }
               
            }
        }
               
        /// <summary>
        /// Gets or sets the property information.
        /// </summary>
        /// <value>The property information.</value>
        public PropertyExpandModes PropertyExpandMode
        {
            get { return (PropertyExpandModes)GetValue(PropertyExpandModeProperty); }
            set { SetValue(PropertyExpandModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertyVisiblityMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyExpandModeProperty =
            DependencyProperty.Register("PropertyExpandMode", typeof(PropertyExpandModes), typeof(PropertyItem), new PropertyMetadata(PropertyExpandModes.FlatMode));


        /// <summary>
        /// Gets or sets the property information.
        /// </summary>
        /// <value>The property information.</value>
        public PropertyInfo PropertyInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get 
            {
                if (PropertyInformation != null)
                {
                    return PropertyInformation.Name;
                }
                return "";
            }
        }


        internal string displayName;
        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get
            {
                if(string.IsNullOrEmpty(displayName))
                {
                    DisplayNameAttribute attr = GetAttribute<DisplayNameAttribute>(PropertyInformation);
                    if (attr != null && !string.IsNullOrEmpty(attr.DisplayName))
                        displayName = attr.DisplayName;
                    else
                        displayName =Name;
                }
                return displayName;
            }
            set
            {
                displayName = value;
            }
        }

        internal string _category;
        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        public string Category
        {
            get
            {
                return _category;
            }
        }


        internal string _description;
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    DescriptionAttribute attr = GetAttribute<DescriptionAttribute>(PropertyInformation);
                    if (attr != null && !string.IsNullOrEmpty(attr.Description))
                        _description = attr.Description;
                    else
                        _description = "";
                }
                return _description;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool EnableToolTip
        {
            get
            {
                if (PropertyGrid != null)
                {
                    return PropertyGrid.EnableToolTip;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public Type PropertyType
        {
         
            get { return PropertyInformation.PropertyType; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can write.
        /// </summary>
        /// <value><c>true</c> if this instance can write; otherwise, <c>false</c>.</value>
        public bool CanWrite
        {
            get { return (PropertyInformation!=null?PropertyInformation.CanWrite && !IsReadOnly:false); }
        }

        internal bool _browsable = true;
        /// <summary>
        /// Gets a value indicating whether this <see cref="PropertyItem"/> is browsable.
        /// </summary>
        /// <value><c>true</c> if browsable; otherwise, <c>false</c>.</value>
        public bool Browsable
        {
            get { return _browsable; }
        }

        internal bool _isReadOnly;
        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                //ReadOnlyAttribute readOnlyAttribute = PropertyItem.GetAttribute<ReadOnlyAttribute>(PropertyInformation);
                //if (readOnlyAttribute != null)
                //    _isReadOnly = readOnlyAttribute.IsReadOnly;
                //else
                //    _isReadOnly = false;
                return _isReadOnly;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(PropertyItem), new PropertyMetadata(null, OnValueChanged));

        /// <summary>
        /// Called when [value changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PropertyItem pitem = (PropertyItem)obj;
            pitem.OnValueChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:ValueChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.PropertyGrid != null)
                this.PropertyGrid.FireValueChanged(this,args);
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public static T GetAttribute<T>(PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes(typeof(T), true);
            return (attributes.Length > 0) ? attributes.OfType<T>().First() : default(T);
        }

        //public T GetAttribute<T>()
        //{
        //    return GetAttribute<T>(_propertyInfo);
        //}

        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        /// <value>The editor.</value>
        public object Editor
        {
            get;
            set;
        }

        public double PropertyLevel
        {
            get { return (double)GetValue(PropertyLevelProperty); }
            set { SetValue(PropertyLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertyLevel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyLevelProperty =
            DependencyProperty.Register("PropertyLevel", typeof(double), typeof(PropertyItem), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the property editor.
        /// </summary>
        /// <value>The property editor.</value>
        public object PropertyEditor
        {
            get;
            set;
        }

        private DataTemplate _template;
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        public DataTemplate Template
        {
            get { return _template; }
            set { _template = value; }
        }

        /// <summary>
        /// Gets or sets the category value properties.
        /// </summary>
        /// <value>The category value properties.</value>
        public PropertyItemCollection CategoryValueProperties
        {
            get { return (PropertyItemCollection)GetValue(CategoryValuePropertiesProperty); }
            set { SetValue(CategoryValuePropertiesProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CategoryValuePropertiesProperty =
            DependencyProperty.Register("CategoryValueProperties", typeof(PropertyItemCollection), typeof(PropertyItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the selected object properties.
        /// </summary>
        /// <value>The selected object properties.</value>
        internal PropertyItemCollection SelectedObjectProperties
        {
            get { return (PropertyItemCollection)GetValue(SelectedObjectPropertiesProperty); }
            set { SetValue(SelectedObjectPropertiesProperty, value); }
        }


        internal static readonly DependencyProperty SelectedObjectPropertiesProperty =
            DependencyProperty.Register("SelectedObjectProperties", typeof(PropertyItemCollection), typeof(PropertyItem), new PropertyMetadata(null));



        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <value></value>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified column name.
        /// </summary>
        /// <value></value>
        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (PropertyType == typeof(Image))
                {
                    result = "Please enter a Name";
                }
                return result;
            }
        }
      
        private bool isselected = false;
        public bool IsSelected
        {
            get
            {
                return isselected;
            }
            set
            {
                isselected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }
        
        internal bool IsCategoryEditorEnabled
        {
            set;
            get;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
