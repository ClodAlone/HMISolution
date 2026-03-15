#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

// <copyright file="IView.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Interface to the view.
    /// </summary>
    public interface IView
    {
        #region Properties

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        IModel Model { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        Point Origin { get; set; }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        System.Windows.Thickness Bounds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show page].
        /// </summary>
        /// <value><c>true</c> if [show page]; otherwise, <c>false</c>.</value>
        bool ShowPage { get; set; }

        #endregion
    }
}
