// <copyright file="ButtonPanel.cs" company="Syncfusion">
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a ButtonPanel control.
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
    /// <example><code>public class ButtonPanel : ItemsControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:ButtonPanel Name="buttonPanel" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// ButtonPanel class represents a panel control that can display horizontal row of UIElements in a single border.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a ButtonPanel in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:ButtonPanel>
    /// <ribbon:RibbonButton Name="btnIncFontSize" Command="EditingCommands.IncreaseFontSize" />
    /// <ribbon:RibbonButton Name="btnDecFontSize" Command="EditingCommands.DecreaseFontSize" />
    /// </ribbon:ButtonPanel>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a ButtonPanel in C#.
    /// <code>    
    /// StackPanel stackPanel;
    /// RibbonButton button1;
    /// RibbonButton button2;
    /// ButtonPanel panel = new ButtonPanel();
    /// panel.Items.Add(button1);
    /// panel.Items.Add(button2);
    /// stackPanel.Children.Add( panel );
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ButtonPanel : ItemsControl
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="ButtonPanel"/> class.
        /// </summary>
        /// <remarks> This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
        /// This style is defined in themes\generic.xaml</remarks>
        static ButtonPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonPanel), new FrameworkPropertyMetadata(typeof(ButtonPanel)));
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the separator visibility.
        /// </summary>
        /// <value>The separator visibility.</value>
        public Visibility SeparatorVisibility
        {
            get { return (Visibility)GetValue(SeparatorVisibilityProperty); }
            set { SetValue(SeparatorVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeparatorVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeparatorVisibilityProperty =
            DependencyProperty.Register("SeparatorVisibility", typeof(Visibility), typeof(ButtonPanel), new UIPropertyMetadata(Visibility.Visible));

        #endregion

    }
}
