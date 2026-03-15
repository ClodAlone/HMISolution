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

namespace Syncfusion.Windows.Tools.Controls
{
    public class CssPropertiesAttribute :Attribute
    {
        #region Private members
        
        private string _propertyname = string.Empty;

        #endregion 

        #region Properties

        /// <summary>
        /// Defines the property name 
        /// </summary>
        internal string PropertyName
        {
            get
            {
                return _propertyname;
            }
            set
            {
                _propertyname = value;
            }
        }

        #endregion

        #region Constructors

        public CssPropertiesAttribute()
        {
        }

        public CssPropertiesAttribute(string name)
        {
            _propertyname = name;
        }

        #endregion
    }

    public class HTMLNotInheritable : Attribute
    {
        public HTMLNotInheritable()
        {
        }
    }
}
