// <copyright file="TreeViewItemAdvAutomationPeer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System.Runtime.CompilerServices;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Exposes TreeViewItemAdv types to UI Automation.
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
    /// <example><code>public class TreeViewItemAdvAutomationPeer : <see cref="FrameworkElementAutomationPeer"/>, <see cref="IExpandCollapseProvider"/>, <see cref="ISelectionItemProvider"/>, <see cref="IScrollItemProvider"/></code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example>
    /// <code language="XAML">
    /// You cannot use this managed class in XAML.
    /// </code>
    /// </example>
    /// </list>
    /// </example>
    /// </list>
    /// <seealso cref="TreeViewAdv"/>
    /// <seealso cref="TreeViewAdvAutomationPeer"/>
    /// <seealso cref="System.Windows.Automation.Peers"/>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeViewItemAdvAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, ISelectionItemProvider, IScrollItemProvider
    {
        #region Constants

        /// <summary>
        /// Name of the class for automation peer.
        /// </summary>
        private const string C_sAutomationPeerClassName = "TreeViewItemAdv";

        #endregion Constants

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItemAdvAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public TreeViewItemAdvAutomationPeer(TreeViewItemAdv owner)
            : base(owner)
        {
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Gets the control type for the UIElement
        /// that is associated with this UIElementAutomationPeer.
        /// This method is called by GetAutomationControlType.
        /// </summary>
        /// <returns>The Custom enumeration value.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TreeItem;
        }

        /// <summary>
        /// Gets the name of the UIElement
        /// that is associated with this UIElementAutomationPeer.
        /// This method is called by GetClassName.
        /// </summary>
        /// <returns>An Empty string.</returns>
        protected override string GetClassNameCore()
        {
            return C_sAutomationPeerClassName;
        }

        /// <summary>
        /// Gets the control pattern for the UIElement
        /// that is associated with this UIElementAutomationPeer.
        /// </summary>
        /// <param name="patternInterface">A value from the enumeration.</param>
        /// <returns>A null reference.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            object pattern = null;

            if (patternInterface == PatternInterface.ExpandCollapse
                || patternInterface == PatternInterface.SelectionItem
                || patternInterface == PatternInterface.ScrollItem)
            {
                pattern = this;
            }

            return pattern;
        }

        /// <summary>
        /// Raises the automation is selected changed.
        /// </summary>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseAutomationIsSelectedChanged(bool isSelected)
        {
            base.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, !isSelected, isSelected);
        }

        /// <summary>
        /// Raises the expand collapse automation event.
        /// </summary>
        /// <param name="oldValue">if set to <c>true</c> [old value].</param>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            base.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed, newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }

        #endregion Implementation

        #region Supprot IExpandCollapseProvider

        /// <summary>
        /// Hides all nodes, controls, or content that are descendants of the control.
        /// </summary>
        void IExpandCollapseProvider.Collapse()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            TreeViewItemAdv owner = (TreeViewItemAdv)base.Owner;

            if (owner.HasItems)
            {
                owner.IsExpanded = false;
            }
        }

        /// <summary>
        /// Displays all child nodes, controls, or content of the control.
        /// </summary>
        void IExpandCollapseProvider.Expand()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            TreeViewItemAdv owner = (TreeViewItemAdv)base.Owner;

            if (owner.HasItems)
            {
                owner.IsExpanded = true;
            }
        }

        /// <summary>
        /// Gets the state, expanded or collapsed, of the control.
        /// </summary>
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                TreeViewItemAdv owner = (TreeViewItemAdv)base.Owner;
                ExpandCollapseState state = ExpandCollapseState.Expanded;

                if (!owner.HasItems)
                {
                    state = ExpandCollapseState.LeafNode;
                }
                else if (!owner.IsExpanded)
                {
                    state = ExpandCollapseState.Collapsed;
                }

                return state;
            }
        }

        #endregion Supprot IExpandCollapseProvider

        #region Support IScrollItemProvider

        /// <summary>
        /// Scrolls the content area of a container object in order
        /// to display the control within the visible region (viewport)
        /// of the container.
        /// </summary>
        void IScrollItemProvider.ScrollIntoView()
        {
            ((TreeViewItemAdv)base.Owner).BringIntoView();
        }

        #endregion Support IScrollItemProvider

        #region Support ISelectionItemProvider

        /// <summary>
        /// Adds the current element to the collection of selected items.
        /// </summary>
        void ISelectionItemProvider.AddToSelection()
        {
            TreeViewAdv parentTreeViewAdv = ((TreeViewItemAdv)base.Owner).ParentTreeView;

            if (parentTreeViewAdv != null
                || (parentTreeViewAdv.SelectedItem == null && parentTreeViewAdv.SelectedContainer == base.Owner))
            {
                ((TreeViewItemAdv)base.Owner).IsSelected = true;
            }
        }

        /// <summary>
        /// Removes the current element from the collection of selected items.
        /// </summary>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            ((TreeViewItemAdv)base.Owner).IsSelected = false;
        }

        /// <summary>
        /// Deselects any selected items and then selects the current element.
        /// </summary>
        void ISelectionItemProvider.Select()
        {
            ((TreeViewItemAdv)base.Owner).IsSelected = true;
        }

        /// <summary>
        /// Gets the UI Automation provider that implements ISelectionProvider
        /// and acts as the container for the calling object.
        /// </summary>
        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                ItemsControl element = ((TreeViewItemAdv)base.Owner).ParentItemsControl;
                IRawElementProviderSimple provider = null;

                if (element != null)
                {
                    AutomationPeer peer = UIElementAutomationPeer.FromElement(element);

                    if (peer != null)
                    {
                        provider = base.ProviderFromPeer(peer);
                    }
                }

                return provider;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an item is selected.
        /// </summary>
        /// <value></value>
        /// <returns>true if the element is selected; otherwise false.</returns>
        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                return ((TreeViewItemAdv)base.Owner).IsSelected;
            }
        }

        #endregion Support ISelectionItemProvider
    }
}