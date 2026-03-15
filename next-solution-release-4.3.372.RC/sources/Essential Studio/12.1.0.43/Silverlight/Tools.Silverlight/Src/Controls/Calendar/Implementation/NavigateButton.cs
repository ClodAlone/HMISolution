#region Copyright
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

namespace Syncfusion.Windows.Tools.Controls.Implementation
{
    public class NavigateButton : ContentControl
    {
        /// <summary>
        /// Identifies <see cref="Enabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register("Enabled", typeof(bool), typeof(NavigateButton), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether navigate button is enabled.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Navigate button becomes disabled if there is no more
        /// available dates.
        /// </remarks>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
        /// <seealso cref="bool"/>
        public bool Enabled
        {
            get
            {
                return (bool)GetValue(EnabledProperty);
            }

            set
            {
                SetValue(EnabledProperty, value);
            }
        }

        /// <summary>
        /// Updates data template of the NavigateButton.
        /// </summary>
        /// <param name="template">Data template to be set to the NavigateButton. If it is
        /// null the local value of data template would be cleared.</param>
        protected internal void UpdateCellTemplate(ControlTemplate template)
        {
            if (template != null)
            {
                Template = template;
            }
        }
        #region Initialization

        #endregion
    }
}
