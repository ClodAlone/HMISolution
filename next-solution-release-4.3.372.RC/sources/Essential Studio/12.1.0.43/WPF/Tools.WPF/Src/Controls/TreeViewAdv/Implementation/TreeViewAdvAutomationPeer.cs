// <copyright file="TreeViewAdvAutomationPeer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System.Collections.Generic;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Exposes TreeViewAdv types to UI Automation.
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
    /// <example><code>public class TreeViewAdvAutomationPeer : <see cref="FrameworkElementAutomationPeer"/>, <see cref="ISelectionProvider"/></code></example>
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
    public class TreeViewAdvAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
    {
        #region Constants

        /// <summary>
        /// Name of the class for automation peer.
        /// </summary>
        private const string C_sAutomationPeerClassName = "TreeViewAdv";

        #endregion Constants

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewAdvAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public TreeViewAdvAutomationPeer(TreeViewAdv owner)
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
            return AutomationControlType.Tree;
        }

        /// <summary>
        /// Gets the collection of child elements of the UIElement
        /// that is associated with this UIElementAutomationPeer.
        /// This method is called by GetChildren.
        /// </summary>
        /// <returns>A list of child AutomationPeer elements.</returns>
        protected override List<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> list = null;
            ItemsControl owner = (ItemsControl)base.Owner;
            ItemCollection items = owner.Items;

            if (items.Count <= 0)
            {
                list = new List<AutomationPeer>(items.Count);
                TreeViewItemAdv element = null;

                for (int i = 0; i < items.Count; i++)
                {
                    element = owner.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;

                    if (element != null)
                    {
                        AutomationPeer item = UIElementAutomationPeer.FromElement(element);

                        if (item == null)
                        {
                            item = UIElementAutomationPeer.CreatePeerForElement(element);
                        }

                        list.Add(item);
                    }
                }
            }

            return list;
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

            if (patternInterface == PatternInterface.Selection)
            {
                pattern = this;
            }
            else if (patternInterface == PatternInterface.Scroll)
            {
                TreeViewAdv owner = base.Owner as TreeViewAdv;

                if (owner != null && owner.ScrollHost != null)
                {
                    AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(owner.ScrollHost);

                    if ((peer != null) && (peer is IScrollProvider))
                    {
                        peer.EventsSource = this;
                        return (IScrollProvider)peer;
                    }
                }
            }

            return pattern;
        }

        #endregion Implementation

        #region Support ISelectionProvider

        /// <summary>
        /// Gets a value indicating whether the UI Automation provider allows more
        /// than one child element to be selected concurrently.
        /// </summary>
        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether whether the UI Automation provider requires
        /// at least one child element to be selected.
        /// </summary>
        bool ISelectionProvider.IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves a UI Automation provider for each child element that is selected.
        /// </summary>
        /// <returns>A collection of UI Automation providers.</returns>
        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            IRawElementProviderSimple[] simpleArray = null;
            TreeViewItemAdv element = ((TreeViewAdv)base.Owner).SelectedContainer;

            if (element != null)
            {
                AutomationPeer peer = UIElementAutomationPeer.FromElement(element);

                if (peer != null)
                {
                    simpleArray = new IRawElementProviderSimple[] { base.ProviderFromPeer(peer) };
                }
            }

            if (simpleArray == null)
            {
                simpleArray = new IRawElementProviderSimple[0];
            }

            return simpleArray;
        }

        #endregion Support ISelectionProvider
    }
}