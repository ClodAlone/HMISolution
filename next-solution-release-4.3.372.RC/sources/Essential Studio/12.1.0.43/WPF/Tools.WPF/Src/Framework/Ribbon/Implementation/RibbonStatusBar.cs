// <copyright file="RibbonStatusBar.cs" company="Syncfusion">
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
    /// <summary>
    /// Represents a RibbonStatusBar control.
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class RibbonStatusBar : ItemsControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:RibbonStatusBar Name="bar" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// A RibbonStatusBar is a bar that typically displays a horizontal row of images and status information. 
    /// You can divide the items in a RibbonStatusBar into groups that contain related items, by 
    /// using Separator controls.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a RibbonStatusBar in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:RibbonStatusBar>
    /// <TextBlock VerticalAlignment="Center" Margin="7,0,0,0" ">Ready</TextBlock>
    /// </ribbon:RibbonStatusBar>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a RibbonStatusBar in C#.
    /// <code>    
    /// TextBlock text = new TextBlock();
    /// text.Text = "Ready";
    /// RibbonStatusBar bar = new RibbonStatusBar();
    /// bar.Items.Add(text);
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonStatusBar : ItemsControl
    {
        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="RibbonStatusBar"/> class.
        /// </summary>
        static RibbonStatusBar()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonStatusBar));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonStatusBar), new FrameworkPropertyMetadata(typeof(RibbonStatusBar)));
        }
        #endregion
    }
}
