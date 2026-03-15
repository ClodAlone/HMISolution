// <copyright file="RibbonGalleryGroup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a RibbonGalleryGroup control.
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
    /// <example><code>public class RibbonGalleryGroup : Control</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:RibbonGalleryGroup Name="group" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// RibbonGalleryGroup class represents a control that can display collection of UIElements in multiple rows inside RibbonGallery control.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a ButtonPanel in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:RibbonGalleryGroup Label="First group">
    /// <ribbon:RibbonGalleryItem>
    /// <Image Source="SampleImages/Apex.png"/>
    /// </ribbon:RibbonGalleryItem>
    /// <Image Source="SampleImages/Apex.png"/>
    /// <Image Source="SampleImages/Aspect.png"/>
    /// </ribbon:RibbonGalleryGroup>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a RibbonGalleryGroup in C#.
    /// <code>    
    /// RibbonGalleryGroup galleryGroup = new RibbonGalleryGroup();
    /// TextBlock text = new TextBlock();
    /// text.Text = "Item";
    /// galleryGroup.Items.Add(text);
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ContentProperty("Items")]
    [DefaultProperty("Items")]
    public class RibbonGalleryGroup : Control
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RibbonGalleryGroup"/> class.
        /// </summary>
        static RibbonGalleryGroup()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonGalleryGroup));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGalleryGroup), new FrameworkPropertyMetadata(typeof(RibbonGalleryGroup)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonGalleryGroup"/> class.
        /// </summary>
        public RibbonGalleryGroup()
        {
            Items = new ObservableCollection<object>();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public ObservableCollection<object> Items
        {
            get
            {
                return (ObservableCollection<object>)GetValue(ItemsProperty);
            }

            set
            {
                SetValue(ItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label of the gallery group.
        /// </summary>
        /// Type: <see cref="string"/>
        /// Text that names the gallery group.
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines the collection of gallery group items.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<object>), typeof(RibbonGalleryGroup), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines the label of the gallery group.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(RibbonGalleryGroup), new UIPropertyMetadata(string.Empty));

        #endregion
    }
}
