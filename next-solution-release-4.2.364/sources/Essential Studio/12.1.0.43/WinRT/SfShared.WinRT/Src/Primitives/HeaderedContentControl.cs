// <copyright file="HeaderedContentControl.cs" company="Syncfusion">
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
using System.Windows.Controls;
using System.Windows;

namespace Syncfusion.WP.Primitives
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
namespace Syncfusion.Tools.Primitives
#else
#if WPF
using System.Windows;
using System.Windows.Controls;
namespace Syncfusion.Windows.Primitives
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace Syncfusion.UI.Xaml.Primitives
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a control with headered content
    /// </summary>
    public class HeaderedContentControl : ContentControl
    {
        #region Constructor

        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Primitives.HeaderedContentControl"/> class.
        /// </summary>
        public HeaderedContentControl()
        {
            DefaultStyleKey = typeof(HeaderedContentControl);
        }

        #endregion

        #region Dependency Properties
        /// <summary>
        /// Gets or sets the data used as header
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(HeaderedContentControl), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the style for the data used as header
        /// </summary>
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(HeaderedContentControl), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the template for the data used as header
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HeaderedContentControl), new PropertyMetadata(null));

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7|| SILVERLIGHT)
        /// <summary>
        /// Gets or sets the TemplateSelector for the data used as header
        /// </summary>
        public DataTemplateSelector HeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
            set { SetValue(HeaderTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateSelectorProperty =
            DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(HeaderedContentControl), new PropertyMetadata(null));
#endif
        #endregion
    }
}
