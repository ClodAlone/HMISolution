// <copyright file="AutoTemplatedContentControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Class used to create control that uses some other type as it's default style key. 
    /// Useful when element can not have template itself and it's internal classes should not be visible to user.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class AutoTemplatedContentControl
        : ContentControl
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTemplatedContentControl"/> class.
        /// </summary>
        /// <param name="keyType">Specifies type to be set as a style key.</param>
        public AutoTemplatedContentControl(Type keyType)
        {
            if (keyType == null)
            {
                throw new ArgumentNullException("keyType");
            }

            this.SetValue(DefaultStyleKeyProperty, keyType);
        }
        #endregion
    }
}
