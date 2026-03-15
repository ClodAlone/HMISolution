// <copyright file="RibbonPage.cs" company="Syncfusion">
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a RibbonPage control.
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
    /// <example><code>public class RibbonPage : Page</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:RibbonPage x:Class="RibbonSample.Page1" x:Name="RibbonPage"/>]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// RibbonPage class represents main Ribbon UI element - Page control.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a RibbonPage in XAML.
    /// <code><![CDATA[<ribbon:RibbonPage x:Class="TestXBAP.Page1" Name="ribbonPage"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:ribbon="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
    /// xmlns:shared="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.WPF"
    /// xmlns:sample="clr-namespace:TestXBAP"
    /// Title="Page1"/>]]></code>
    /// <para/>This example shows how to create a RibbonPage in C#.
    /// <code>    
    /// using System;
    /// using System.Windows;
    /// using Syncfusion.Windows.Tools.Controls;
    /// namespace CSharp
    /// {
    /// public partial class CodeOnlyPage : RibbonPage
    /// {
    /// public CodeOnlyPage()
    /// {
    /// this.Title = "Main Page";
    /// }
    /// }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonPage : Page
    {
        #region Properties
        /// <summary>
        /// Gets or sets the Ribbon control of the page.
        /// </summary>
        /// <value>
        /// Type: <see cref="Ribbon"/>
        /// Instance of ribbon control used in page.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonPage page;
        /// Ribbon ribbon;
        /// // ....
        /// page.Ribbon = ribbon;
        /// </code>
        /// </example>
        /// <seealso cref="Ribbon"/>
        /// <seealso cref="RibbonPage"/>
        public Ribbon Ribbon
        {
            get
            {
                return (Ribbon)GetValue(RibbonProperty);
            }

            set
            {
                SetValue(RibbonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the StatusBar control of the page.
        /// </summary>        
        /// <value>
        /// Type: <see cref="RibbonStatusBar"/>
        /// Instance of RibbonStatusBar control used in page.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonPage page;
        /// RibbonStatusBar statusBar;
        /// // ....
        /// page.StatusBar = statusBar;
        /// </code>
        /// </example>
        /// <seealso cref="Ribbon"/>
        /// <seealso cref="RibbonPage"/>
        public RibbonStatusBar StatusBar
        {
            get
            {
                return (RibbonStatusBar)GetValue(StatusBarProperty);
            }

            set
            {
                SetValue(StatusBarProperty, value);
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Gets or sets the Ribbon control of the page. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty RibbonProperty =
            DependencyProperty.Register("Ribbon", typeof(Ribbon), typeof(RibbonPage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the StatusBar control of the page. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty StatusBarProperty =
            DependencyProperty.Register("StatusBar", typeof(RibbonStatusBar), typeof(RibbonPage), new UIPropertyMetadata(null));

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RibbonPage"/> class.
        /// </summary>
        static RibbonPage()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonPage));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonPage), new FrameworkPropertyMetadata(typeof(RibbonPage)));
        }
        #endregion

       
    }
}
