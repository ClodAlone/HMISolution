// <copyright file="TreeViewRowPresenterBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for the TreeView Row Presenter
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class TreeViewRowPresenterBase : FrameworkElement, IWeakEventListener
    {
        #region Members

        /// <summary>
        /// Presents Visual tree state
        /// </summary>
        private bool m_needUpdateVisualTree = true;

        /// <summary>
        /// Presents UIElement Collection
        /// </summary>
        private UIElementCollection m_uiElementCollection;

        #endregion Members

        #region Dependency property

        /// <summary>
        /// Represents the ColumnsProperty of TreeViewRowPreseneter
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(TreeViewColumnCollection), typeof(TreeViewRowPresenterBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(TreeViewRowPresenterBase.ColumnsPropertyChanged)));

        #endregion Dependency property

        #region Properties

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public TreeViewColumnCollection Columns
        {
            get
            {
                return (TreeViewColumnCollection)base.GetValue(ColumnsProperty);
            }

            set
            {
                base.SetValue(ColumnsProperty, value);
            }
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An enumerator for logical child elements of this element.
        /// </returns>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                return InternalChildren.GetEnumerator();
            }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                if (m_uiElementCollection == null)
                {
                    return 0;
                }

                return m_uiElementCollection.Count;
            }
        }

        /// <summary>
        /// Gets the internal children.
        /// </summary>
        /// <value>The internal children.</value>
        internal UIElementCollection InternalChildren
        {
            get
            {
                if (m_uiElementCollection == null)
                {
                    m_uiElementCollection = new UIElementCollection(this, this);
                }

                return m_uiElementCollection;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [need update visual tree].
        /// </summary>
        /// <value>
        /// <c>true</c> if [need update visual tree]; otherwise, <c>false</c>.
        /// </value>
        internal bool NeedUpdateVisualTree
        {
            get
            {
                return m_needUpdateVisualTree;
            }

            set
            {
                m_needUpdateVisualTree = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is presenter visual ready.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is presenter visual ready; otherwise, <c>false</c>.
        /// </value>
        private bool IsPresenterVisualReady
        {
            get
            {
                return IsInitialized && !NeedUpdateVisualTree;
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewRowPresenterBase"/> class.
        /// </summary>
        protected TreeViewRowPresenterBase()
        {
            m_needUpdateVisualTree = true;
            
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            if (m_uiElementCollection == null)
            {
                throw new ArgumentOutOfRangeException("index", index, "Visual_ArgumentOutOfRange");
            }

            return m_uiElementCollection[index];
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Called when [column property changed].
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="propertyName">Name of the property.</param>
        internal abstract void OnColumnPropertyChanged(TreeViewColumn column, string propertyName);

        /// <summary>
        /// Raises the <see cref="E:ColumnCollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.TreeViewColumnCollectionChangedEventArgs"/> instance containing the event data.</param>
        internal virtual void OnColumnCollectionChanged(TreeViewColumnCollectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the stable ancestor
        /// </summary>
        /// <returns>Framework Element</returns>
        private FrameworkElement GetStableAncester()
        {
            ItemsControl control = ItemsControl.ItemsControlFromItemContainer(TemplatedParent);

            if (control == null)
            {
                return this;
            }

            return control;
        }

        /// <summary>
        /// Columns the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="arg">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void ColumnCollectionChanged(object sender, NotifyCollectionChangedEventArgs arg)
        {
            TreeViewColumnCollectionChangedEventArgs e = arg as TreeViewColumnCollectionChangedEventArgs;

            if ((e != null) && this.IsPresenterVisualReady)
            {
                if (e.Column != null)
                {
                    OnColumnPropertyChanged(e.Column, e.PropertyName);
                }
                else
                {
                    OnColumnCollectionChanged(e);
                }
            }
        }

        /// <summary>
        /// Columns the property changed.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewRowPresenterBase listener = (TreeViewRowPresenterBase)d;
            TreeViewColumnCollection oldValue = (TreeViewColumnCollection)e.OldValue;

            if (oldValue != null)
            {
                InternalCollectionChangedEventManager.RemoveListener(oldValue, listener);

                if (!oldValue.InViewMode && (oldValue.Owner == listener.GetStableAncester()))
                {
                    oldValue.Owner = null;
                }
            }

            TreeViewColumnCollection newValue = (TreeViewColumnCollection)e.NewValue;

            if (newValue != null)
            {
                InternalCollectionChangedEventManager.AddListener(newValue, listener);

                if (!newValue.InViewMode && (newValue.Owner == null))
                {
                    newValue.Owner = listener.GetStableAncester();
                }
            }

            listener.NeedUpdateVisualTree = true;
            listener.Initialize();
            listener.InvalidateMeasure();
        }

        #endregion Implementation

        #region IWeakEventListener

        /// <summary>
        /// Receives the weak event.
        /// </summary>
        /// <param name="managerType">Type of the manager.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns>bool value type </returns>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs args)
        {
            if (managerType == typeof(InternalCollectionChangedEventManager))
            {
                ColumnCollectionChanged(sender, (NotifyCollectionChangedEventArgs)args);
                return true;
            }

            return false;
        }

        #endregion IWeakEventListener
    }
}