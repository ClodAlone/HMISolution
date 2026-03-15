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
    /// Represents the IWindow Manager Interface.
    /// </summary>
	public interface IWindowsManager
	{
        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="location">The location.</param>
        /// <returns>The IWindow Object.</returns>
		IWindow ShowWindow(FrameworkElement content, string caption, Point location);

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="location">The location.</param>
		void ShowWindow(Window w, Point location);
	}
}
