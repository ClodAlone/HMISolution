#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINDOWS_PHONE ||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Syncfusion.WP.Primitives
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Markup;
using System.Windows.Data;
namespace Syncfusion.Tools.Primitives
#else
#if WPF
using System.Windows;
using System.Windows.Markup;
using System.Windows.Data;
namespace Syncfusion.Windows.Primitives
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Primitives
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a <see cref="T:System.Windows.DataTemplate" /> that supports
    /// <see cref="T:System.Windows.Controls.HeaderedItemsControl" /> objects,
    /// such as <see cref="T:System.Windows.Controls.TreeViewItem" />.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
#if !WINRT
    [ContentProperty("Template")]
#endif
    public partial class HierarchicalDataTemplate : DataTemplate
    {
       
        /// <summary>
        /// Gets or sets the values for the item source
        /// </summary>
        public Binding ItemsSource { get; set; }

        #region public DataTemplate ItemTemplate

        /// <summary>
        /// The DataTemplate to apply to the ItemTemplate property on a
        /// generated HeaderedItemsControl (such as a MenuItem or a
        /// TreeViewItem), to indicate how to display items from the next level
        /// in the data hierarchy.
        /// </summary>
        private DataTemplate _itemTemplate;

        /// <summary>
        /// Gets a value indicating whether the ItemTemplate property was set on
        /// the template.
        /// </summary>
        internal bool IsItemTemplateSet { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.DataTemplate" /> to
        /// apply to the
        /// <see cref="P:System.Windows.Controls.ItemsControl.ItemTemplate" />
        /// property on a generated
        /// <see cref="T:System.Windows.Controls.HeaderedItemsControl" />, such
        /// as a <see cref="T:System.Windows.Controls.TreeViewItem" />, to
        /// indicate how to display items from the next sublevel in the data
        /// hierarchy.
        /// </summary>
        /// <value>
        /// The <see cref="T:System.Windows.DataTemplate" /> to apply to the
        /// <see cref="P:System.Windows.Controls.ItemsControl.ItemTemplate" />
        /// property on a generated
        /// <see cref="T:System.Windows.Controls.HeaderedItemsControl" />, such
        /// as a <see cref="T:System.Windows.Controls.TreeViewItem" />, to
        /// indicate how to display items from the next sublevel in the data
        /// hierarchy.
        /// </value>
        public DataTemplate ItemTemplate
        {
            get { return _itemTemplate; }
            set
            {
                IsItemTemplateSet = true;
                _itemTemplate = value;
            }
        }

        private DataTemplate _template;
        /// <summary>
        /// Gets or sets the value for the template
        /// </summary>
#if WPF
        public new DataTemplate Template
#else
        public DataTemplate Template
#endif
        {
            get { return _template; }
            set { _template = value; }
        }

        #endregion public DataTemplate ItemTemplate

        #region public Style ItemContainerStyle

        /// <summary>
        /// The Style to apply to the ItemContainerStyle property on a generated
        /// HeaderedItemsControl (such as a MenuItem or a TreeViewItem), to
        /// indicate how to style items from the next level in the data
        /// hierarchy.
        /// </summary>
        private Style _itemContainerStyle;

        /// <summary>
        /// Gets a value indicating whether the ItemContainerStyle property was
        /// set on the template.
        /// </summary>
        internal bool IsItemContainerStyleSet { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Style" /> that is
        /// applied to the item container for each child item.
        /// </summary>
        /// <value>
        /// The style that is applied to the item container for each child item.
        /// </value>
        public Style ItemContainerStyle
        {
            get { return _itemContainerStyle; }
            set
            {
                IsItemContainerStyleSet = true;
                _itemContainerStyle = value;
            }
        }

        #endregion public Style ItemContainerStyle

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:WinRTXamlToolkit.Controls.Data.HierarchicalDataTemplate" /> class.
        /// </summary>
        public HierarchicalDataTemplate()
        {
        }
    }
}
