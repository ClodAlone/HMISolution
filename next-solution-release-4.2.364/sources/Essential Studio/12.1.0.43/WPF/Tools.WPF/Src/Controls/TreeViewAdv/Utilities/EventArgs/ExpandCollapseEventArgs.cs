// <copyright file="SortModeChangeEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System.Windows;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Contains arguments relevant to Expanding/Collapsing of the TreeViewItemAdv.
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
    /// <example><code>public class ExpandingCollapsingEventArgs : <see cref="RoutedEventArgs"/></code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example>
    /// <code language="XAML">
    /// This managed class is not typically used in XAML.
    /// </code>
    /// </example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// This class is responsible for packaging the event data for TreeViewItemAdv
    /// </remarks>
    /// <seealso cref="TreeViewAdv"/>
    /// <seealso cref="TreeViewItemAdv"/>
    /// <seealso cref="RoutedEventArgs"/>
    public class ExpandingCollapsingEventArgs : RoutedEventArgs
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandCollapseEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The source.</param>
        /// <param name="oldValue">The parent treeview.</param>
        public ExpandingCollapsingEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandCollapseEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        public ExpandingCollapsingEventArgs(RoutedEvent routedEvent)
            : this(routedEvent, null)
        {
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets the cancel.
        /// </summary>
        /// <value>The cancel.</value>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the IsExpandingOnDoubleClick will true when treeview has expand or collapsed by double click on treeview item,
        /// otherwise the IsExpandingOnDouble click will return false, if we expand the treeview item using gylph.
        /// </summary>
        /// <value>The IsExpandOnDoubleClick.</value>
        public bool IsExpandingOnDoubleClick
        {
            get;
            internal set;
        }

        #endregion Properties
    }

    /// <summary>
    /// Contains arguments relevant to Expanded/Collapsed of the TreeViewItemAdv.
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
    /// <example><code>public class ExpandedCollapsedEventArgs : <see cref="RoutedEventArgs"/></code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example>
    /// <code language="XAML">
    /// This managed class is not typically used in XAML.
    /// </code>
    /// </example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// This class is responsible for packaging the event data for TreeViewItemAdv
    /// </remarks>
    /// <seealso cref="TreeViewAdv"/>
    /// <seealso cref="TreeViewItemAdv"/>
    /// <seealso cref="RoutedEventArgs"/>
    public class ExpandedCollapsedEventArgs : RoutedEventArgs
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandCollapseEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The source.</param>
        /// <param name="oldValue">The parent treeview.</param>
        public ExpandedCollapsedEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandCollapseEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        public ExpandedCollapsedEventArgs(RoutedEvent routedEvent)
            : this(routedEvent, null)
        {
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets the IsExpandOnDoubleClick will true when treeview has expand or collapsed by double click on treeview item,
        /// otherwise the IsExpandOnDouble click will return false, if we expand the treeview item using gylph.
        /// </summary>
        /// <value>The IsExpandOnDoubleClick.</value>
        public bool IsExpandOnDoubleClick
        {
            get;
            internal set;
        }

        #endregion Properties
    }
}