#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;

namespace Syncfusion.Windows.ComponentModel
{
    /// <summary>
    /// Provides support for the <see cref="GetItemProperties"/> method that returns a <see cref="PropertyDescriptorCollection"/>.
    /// </summary>
    public interface IItemPropertiesSource
    {
        /// <summary>
        /// Returns a collection of property descriptors.
        /// </summary>
        /// <returns></returns>
        PropertyDescriptorCollection GetItemProperties();
    }

}
