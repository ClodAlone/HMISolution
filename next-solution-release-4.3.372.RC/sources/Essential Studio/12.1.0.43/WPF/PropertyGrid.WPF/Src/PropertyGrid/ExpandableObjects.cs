#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace Syncfusion.Windows.PropertyGrid
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpandableObjects : ExpandableObjectConverter
    {
        /// <summary>
        /// Gets a value indicating whether this object supports properties using the specified context.
        /// </summary>
        /// <returns>
        /// true because <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object. This method never returns false.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
