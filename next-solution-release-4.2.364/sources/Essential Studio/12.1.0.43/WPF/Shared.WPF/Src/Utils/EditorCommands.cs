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
using System.Windows.Input;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
    public class EditorCommands
    {
        /// <summary>
        /// Clear Command
        /// </summary>
        private static RoutedCommand clear = new RoutedCommand();

        /// <summary>
        /// Gets the Clear Command.
        /// </summary>
        /// <value>The Clear Command.</value>
        public static RoutedCommand Clear
        {
            get
            {
                return clear;
            }
        }
    }
}
