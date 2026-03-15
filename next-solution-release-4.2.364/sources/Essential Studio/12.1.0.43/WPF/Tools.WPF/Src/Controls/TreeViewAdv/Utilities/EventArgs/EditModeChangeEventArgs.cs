// <copyright file="EditModeChangeEventArgs.cs" company="Syncfusion">
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
    /// Contains arguments relevant to edit mode of the TreeViewItemAdv.
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
    /// <example><code>public class EditModeChangeEventArgs : <see cref="RoutedEventArgs"/></code></example>
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
    /// when changes IsInEditmMode property of the node.
    /// </remarks>
    /// <seealso cref="TreeViewAdv"/>
    /// <seealso cref="TreeViewItemAdv"/>
    /// <seealso cref="RoutedEventArgs"/>
    public class EditModeChangeEventArgs : RoutedEventArgs
    {
        #region Member

        /// <summary>
        /// Old value.
        /// </summary>
        private object m_oldValue = null;

        /// <summary>
        /// New value.
        /// </summary>
        private object m_newValue = null;

        #endregion Member

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="EditModeChangeEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The source.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public EditModeChangeEventArgs(RoutedEvent routedEvent, object source, object oldValue, object newValue)
            : base(routedEvent, source)
        {
            m_oldValue = oldValue;
            m_newValue = newValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditModeChangeEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        public EditModeChangeEventArgs(RoutedEvent routedEvent)
            : this(routedEvent, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditModeChangeEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public EditModeChangeEventArgs(RoutedEvent routedEvent, object source)
            : this(routedEvent, source, null, null)
        {
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets old value.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// A object that represent old value of the node.
        /// </value>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="object"/>
        public object OldValue
        {
            get
            {
                return m_oldValue;
            }
        }

        /// <summary>
        /// Gets new value.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// A object that represent new value of the node.
        /// </value>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="object"/>
        public object NewValue
        {
            get
            {
                return m_newValue;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        /// <value></value>
        /// <returns>true if the event should be canceled; otherwise, false.
        /// </returns>
        public bool Cancel
        {
            get;
            set;
        }

        #endregion Properties
    }
}