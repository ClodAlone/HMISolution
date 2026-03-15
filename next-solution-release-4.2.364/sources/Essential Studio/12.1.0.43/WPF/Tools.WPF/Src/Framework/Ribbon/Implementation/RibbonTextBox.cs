// <copyright file="RibbonTextBox.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// Represents a textbox Ribbon control. 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonTextBox : TextBox
    {
        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="RibbonTextBox"/> class.
        /// </summary>
        static RibbonTextBox()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonTextBox));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTextBox), new FrameworkPropertyMetadata(typeof(RibbonTextBox)));
        }
        #endregion
    }
}
