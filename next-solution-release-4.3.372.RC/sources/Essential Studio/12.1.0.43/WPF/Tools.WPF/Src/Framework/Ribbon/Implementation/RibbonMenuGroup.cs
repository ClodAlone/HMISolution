// <copyright file="RibbonMenuGroup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Represents a RibbonMenuGroup control.
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
    /// <example><code>public class RibbonMenuGroup : HeaderedItemsControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:RibbonMenuGroup Name="group" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// RibbonMenuGroup class represents a group control that can display child UIElements in border with header and icon bar.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a RibbonMenuGroup in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:RibbonMenuGroup Header="Header1">
    /// <ribbon:RibbonButton SizeForm = "Small" Label="Item1" SmallIcon="SampleImages/Document32.png"/><ribbon:RibbonButton SizeForm = "Small" Label="Item2" SmallIcon="SampleImages/Document32.png"/>
    /// <ribbon:RibbonButton SizeForm = "Small" Label="Item3" SmallIcon="SampleImages/Save32.png"/>
    /// <ribbon:RibbonButton SizeForm = "Small" Label="Item4" SmallIcon="SampleImages/Close32.png"/>
    /// </ribbon:RibbonMenuGroup>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a RibbonMenuGroup in C#.
    /// <code>
    /// RibbonButton button1;
    /// RibbonButton button2;
    /// RibbonMenuGroup group = new RibbonMenuGroup();
    /// group.Items.Add(button1);
    /// group.Items.Add(button2);
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonMenuGroup : HeaderedItemsControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether menu icon bar is enabled.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True if icon bar is enabled, false if disabled.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonMenuGroup group;
        /// // ....
        /// group.IconBarEnabled = true;
        /// </code>
        /// </example>
        public bool IconBarEnabled
        {
            get
            {
                return (bool)GetValue(IconBarEnabledProperty);
            }

            set
            {
                SetValue(IconBarEnabledProperty, value);
            }
        }
        #endregion

        #region DP Properties
        /// <summary>
        /// Specifies whether menu icon bar is enabled. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IconBarEnabledProperty =
            DependencyProperty.Register("IconBarEnabled", typeof(bool), typeof(RibbonMenuGroup), new UIPropertyMetadata(false));
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RibbonMenuGroup"/> class.
        /// </summary>
        static RibbonMenuGroup()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonMenuGroup));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonMenuGroup), new FrameworkPropertyMetadata(typeof(RibbonMenuGroup)));
        }
        #endregion

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            IInputElement element = Keyboard.FocusedElement;
            if (element != null)
            {
                if (e.Key == Key.Down)
                {
                    (element as UIElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
                else if (e.Key == Key.Up)
                {                  
                    (element as UIElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    if (element != this.Items[0])
                        e.Handled = true;
                }
            }
            else
                (this.Items[0] as UIElement).Focus();
        }
    }
}
