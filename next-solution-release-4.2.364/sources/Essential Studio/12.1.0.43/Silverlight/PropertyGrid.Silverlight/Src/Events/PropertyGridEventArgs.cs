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

namespace Syncfusion.Windows.PropertyGrid
{
    public class ValueChangedEventArgs : EventArgs
    {
        public ValueChangedEventArgs(Property item)
        {
            Property = item;
        }
        public ValueChangedEventArgs(Property item,object newValue,object oldValue)
        {
            Property = item;
            NewValue = newValue;
            OldValue = oldValue;
        }
        public Property Property
        {
            get;
            set;
        }

        public object NewValue
        {
            get;
            internal set;
    }
        public object OldValue
        {
            get;
            internal set;
        }

    }

    public class Property
    {
        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public object SelectedObject
        {
            get;
            set;
        }

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
            get { return PropertyInformation.Name; }
        }

        internal string _DisplayName;
        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get
            {
                return _DisplayName;
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
                return _isReadOnly;
            }
        }

        internal object _Value;
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return _Value; }
        }
    }
}
