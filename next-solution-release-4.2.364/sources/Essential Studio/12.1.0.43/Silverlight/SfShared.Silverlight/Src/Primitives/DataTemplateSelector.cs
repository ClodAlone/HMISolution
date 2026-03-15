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
#if !Silverlight4
using System.Threading.Tasks;
#endif
using System.Windows;

namespace Syncfusion.Tools.Primitives
{
    /// <summary>
    /// Represents a class for selecting the Data Template
    /// </summary>
    public class DataTemplateSelector
    {
        /// <summary>
        /// Used to select a <see cref="T:System.Windows.DataTemplate"/>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }
    }
}
