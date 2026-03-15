// <copyright file="Transition.cs" company="Syncfusion">
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
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
namespace Syncfusion.WP.Controls
#else
#if SILVERLIGHT
using System.Windows;
namespace Syncfusion.Tools.Controls
#else
#if WPF
using System.Windows;
namespace Syncfusion.Windows.Controls
#else
using Windows.UI.Xaml;
namespace Syncfusion.UI.Xaml.Controls
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a class for the content transition
    /// </summary>
    public class ContentTransition : DependencyObject
    {
        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.ContentTransition"/> class.
        /// </summary>
       [ClassReference(IsReviewed = false, ShouldInclude = false)]
        public ContentTransition()
        {
            
        }
    }
}
