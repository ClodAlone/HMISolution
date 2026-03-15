// <copyright file="RibbonAdorner.cs" company="Syncfusion">
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
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents adorner class.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonAdorner : TemplatedAdornerBase
    {
        #region Fields
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.
        /// This enables animation, styling, binding data etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RibbonAdorner), new UIPropertyMetadata(string.Empty));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Using a DependencyProperty as the backing store for Active.
        /// This enables animation, styling, binding data etc...
        /// </summary>
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register("Enabled", typeof(bool), typeof(RibbonAdorner), new UIPropertyMetadata(true));

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text for the Adorner.</value>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RibbonAdorner"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
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
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RibbonAdorner"/> class.
        /// </summary>
        static RibbonAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonAdornerInternalControl), new FrameworkPropertyMetadata(typeof(RibbonAdorner)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">adorner Element</param>
        public RibbonAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.Loaded += new RoutedEventHandler(RibbonAdorner_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the RibbonAdorner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void RibbonAdorner_Loaded(object sender, RoutedEventArgs e)
        {
            Border b_internalBorder = InnerControl.GetTemplateChildInternal("PART_InnerBorder") as Border;

            if (b_internalBorder != null)
            {
                if (MinWidth < b_internalBorder.ActualWidth)

#if SyncfusionFramework4_0
                    if (this.UseLayoutRounding)
                        Width = b_internalBorder.Child.RenderSize.Width + 6;
                    else
#endif

                        Width = b_internalBorder.Child.RenderSize.Width + 5;

                if (MinHeight < b_internalBorder.ActualHeight)
                {
                    Height = b_internalBorder.Child.RenderSize.Height + 3;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion
    }

    /// <summary>
    /// Represents RibbonAdornerTemplate Class
    /// </summary>
    public class RibbonAdornerInternalControl : TemplatedAdornerInternalControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonAdornerInternalControl"/> class.
        /// </summary>
        /// <param name="adorner">Given adorner.</param>
        public RibbonAdornerInternalControl(TemplatedAdornerBase adorner)
            : base(adorner)
        {
        }
    }
   
}