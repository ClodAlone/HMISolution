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
    /// <summary>
    /// Represents the IWindow Interface.
    /// </summary>
	public interface IWindow
	{
        /// <summary>
        /// Occurs when [closed].
        /// </summary>
		event EventHandler Closed;

		/// <summary>
		/// Closes the window.
		/// </summary>
		void Close();

		/// <summary>
		/// Gets or sets the caption of the window.
		/// </summary>
        /// <value>
        /// Type: <see cref="Caption"/>
        /// Provides Caption value for the <see cref="Window"/>.
        /// </value> 
		string Caption 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dragging enabled].
        /// </summary>
        /// <value><c>True</c> if [dragging enabled]; otherwise, <c>false</c>.</value>
        bool DraggingEnabled
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets a value indicating whether [resize enabled].
        /// </summary>
        /// <value><c>True</c> if [resize enabled]; otherwise, <c>false</c>.</value>
		bool ResizeEnabled 
        { 
            get;
            set;
        }

		/// <summary>
        /// Gets or sets HorizontalScrollBarVisibility
		/// enable or disable the automatic horizontal scrollbar
		/// it's set on auto by default.
		/// </summary>
        /// <value>
        /// Type: <see cref="HorizontalScrollBarVisibility"/>
        /// Provides HorizontalScrollBarVisibility value for the <see cref="Window"/>.
        /// </value> 
		ScrollBarVisibility HorizontalScrollBarVisibility 
        { 
            get; 
            set; 
        }

		/// <summary>
        /// Gets or sets VerticalScrollBarVisibility
		/// enable or disable the automatic vertical scrollbar
		/// it's set on auto by default.
		/// </summary>
        /// <value>
        /// Type: <see cref="VerticalScrollBarVisibility"/>
        /// Provides VerticalScrollBarVisibility value for the <see cref="Window"/>.
        /// </value>
		ScrollBarVisibility VerticalScrollBarVisibility
        {
            get;
            set; 
        }
	}
}