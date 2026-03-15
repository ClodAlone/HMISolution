#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Tools.Controls.RibbonTabControl.Interfaces;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// Designer for RibbonTabControl.
    /// </summary>
    public class RibbonTabControlDesigner
        : ControlDesigner
    {
        #region Fields
        /// <summary>
        /// Design time RibbonTabControl instance.
        /// </summary>
        private RibbonTabControl m_control;

        /// <summary>
        /// Collection of verbs.
        /// </summary>
        private DesignerVerbCollection m_verbs = new DesignerVerbCollection();

        /// <summary>
        /// Action list.
        /// </summary>
        private DesignerActionListCollection m_actionList = new DesignerActionListCollection();
        #endregion

        #region Static Fields
        /// <summary>
        /// Pen for drawing adornment border.
        /// </summary>
        private static Pen _adornmentsBorderPen = new Pen(Color.Gray);
        #endregion

        #region Initialization & Finalization

        static RibbonTabControlDesigner()
        {
            _adornmentsBorderPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }

        /// <summary>
        /// Initializes new instance of RibbonTabControlDesigner.
        /// </summary>
        /// <param name="component"> Component value</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            m_control = (RibbonTabControl)Component;

            m_control.Groups.GroupAdded += new RibbonTabGroupEventHandler(Groups_GroupAdded);

            m_verbs.Add(new DesignerVerb("Add group", new EventHandler(AddGroup)));
            m_verbs.Add(new DesignerVerb("Add tab item", new EventHandler(AddTabItem)));
            m_verbs.Add(new DesignerVerb("Add toolstrip button", new EventHandler(AddButton)));
            m_verbs.Add(new DesignerVerb("Add toolstrip label", new EventHandler(AddLabel)));
            m_verbs.Add(new DesignerVerb("Add separator", new EventHandler(AddSeparator)));
            m_verbs.Add(new DesignerVerb("Add toolstrip combobox", new EventHandler(AddCombobox)));
            m_verbs.Add(new DesignerVerb("Add toolstrip textbox", new EventHandler(AddTextbox)));
            m_verbs.Add(new DesignerVerb("Add toolstrip progressbar", new EventHandler(AddProgressBar)));

            // m_verbs.Add( new DesignerVerb( "Add split button", new EventHandler( AddSplitButton ) ) );
            // m_verbs.Add( new DesignerVerb( "Add dropdown button", new EventHandler( AddDropdownButton ) ) );
            RibbonTabControlActionList ribbonTabControlActionList = new RibbonTabControlActionList(m_control, this);

            m_actionList.Add(ribbonTabControlActionList);

            EnableDesignMode((Control)m_control.Header, "Header");
            IDesignerHost headerDesignerHost = GetDesignerHost((Control)m_control.Header);

            if (headerDesignerHost != null)
            {
                IHeaderDesigner headerDesigner = headerDesignerHost.GetDesigner((Control)m_control.Header) as IHeaderDesigner;

                if (headerDesigner != null)
                {
                    RibbonHeaderControlDesigner tabDesigner = headerDesigner.GetTabHeaderDesigner();

                    if (tabDesigner != null)
                    {
                        tabDesigner.Verbs.AddRange(this.Verbs);
                        tabDesigner.ActionLists.Add(ribbonTabControlActionList);
                    }
                }
            }

            ISelectionService selectionService = m_control.Site.GetService(typeof(ISelectionService)) as ISelectionService;

            if (selectionService != null)
            {
                selectionService.SelectionChanged += new EventHandler(SelectionService_SelectionChanged);
            }

            IComponentChangeService changeService = m_control.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            if (changeService != null)
            {
                changeService.ComponentRemoved += new ComponentEventHandler(ChangeService_ComponentRemoved);
            }
        }

        /// <summary>
        /// Releases resources.
        /// </summary>
        /// <param name="disposing">Bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            foreach (RibbonTabGroup group in m_control.Groups)
            {
                IDesignerHost groupDesignerHost = GetDesignerHost(group);

                if (groupDesignerHost != null)
                {
                    RibbonTabGroupDesigner groupDesigner = groupDesignerHost.GetDesigner(group) as RibbonTabGroupDesigner;

                    if (groupDesigner != null)
                    {
                        groupDesigner.AddNewToolStripItem -= new AddNewToolStripItemEventHandler(GroupDesigner_AddNewItem);
                        groupDesigner.GroupClicked -= new ToolStripItemEventHandler(GroupDesigner_GroupClicked);
                        groupDesigner.AddNewSingleItem -= new NewItemDroppedAtSingleItemGroupEventHandler(GroupDesigner_AddNewSingleItem);
                    }
                }
            }

            m_control.Groups.GroupAdded -= new RibbonTabGroupEventHandler(Groups_GroupAdded);

            base.Dispose(disposing);
        }
        #endregion

        #region Verbs
        /// <summary>
        /// Adds new ribbon tab group.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddGroup(object sender, EventArgs e)
        {
            AddNewGroup();
        }

        /// <summary>
        /// Adds new ribbon tab item.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddTabItem(object sender, EventArgs e)
        {
            AddItem(typeof(RibbonTabItem));
        }

        /// <summary>
        /// Adds new toolstrip button.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddButton(object sender, EventArgs e)
        {
            AddItem(typeof(ToolStripButton));
        }

        /// <summary>
        /// Adds new toolstrip label.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddLabel(object sender, EventArgs e)
        {
            AddItem(typeof(ToolStripLabel));
        }

        /// <summary>
        /// Adds new toolstrip separator.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddSeparator(object sender, EventArgs e)
        {
            AddItem(typeof(ToolStripSeparator));
        }

        /// <summary>
        /// Adds new combobox.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddCombobox(object sender, EventArgs e)
        {
            AddItem(typeof(ToolStripComboBox));
        }

        /// <summary>
        /// Adds new textbox.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddTextbox(object sender, EventArgs e)
        {
            AddItem(typeof(ToolStripTextBox));
        }

        /// <summary>
        /// Adds new progress bar.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddProgressBar(object sender, EventArgs e)
        {
            AddItem(typeof(ToolStripProgressBar));
        }
       
        #endregion

        #region Internal Methods
        /// <summary>
        /// Adds new toolstrip item to the control.
        /// </summary>
        /// <param name="itemType">ToolStripItem child type indicating type of component that has to be created.</param>
        /// <returns>Newly created component instance.</returns>
        internal ToolStripItem AddItem(Type itemType)
        {
            if (!itemType.IsSubclassOf(typeof(ToolStripItem))) throw new ArgumentOutOfRangeException("itemType");

            RibbonTabGroup newGroup = AddNewGroup();
            newGroup.SingleItem = true;
            return AddNewItem(itemType, newGroup);
        }

        /// <summary>
        /// Adds new ribbon tab group to the control.
        /// </summary>
        /// <returns>Newly created RibbonTabGroup instance.</returns>
        internal RibbonTabGroup AddNewGroup()
        {
            RibbonTabGroup newGroup = (RibbonTabGroup)CreateHostedComponent(typeof(RibbonTabGroup));

            return m_control.AddGroup(newGroup);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates hosted component of given type.
        /// </summary>
        /// <param name="componentType">Type of component to create.</param>
        /// <param name="bAssignText">Indicates whether text property has to be assigned.</param>
        /// <returns>Newly created hosted component.</returns>
        private IComponent CreateHostedComponent(Type componentType, bool bAssignText)
        {
            if (componentType == null) throw new ArgumentNullException("componentType");

            IDesignerHost designerHost = GetDesignerHost(m_control);

            if (designerHost != null)
            {
                IComponent comp = designerHost.CreateComponent(componentType);

                if (bAssignText)
                {
                    PropertyInfo textProp = componentType.GetProperty("Text", BindingFlags.Instance | BindingFlags.Public);
                    PropertyInfo nameProp = componentType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);

                    if (textProp != null)
                    {
                        textProp.SetValue(comp, nameProp.GetValue(comp, null), null);
                    }
                }

                return comp;
            }

            return null;
        }

        /// <summary>
        /// Creates hosted component of given type.
        /// </summary>
        /// <param name="componentType">Type of component to create.</param>
        /// <returns>Newly created hosted component.</returns>
        private IComponent CreateHostedComponent(Type componentType)
        {
            return CreateHostedComponent(componentType, true);
        }

        /// <summary>
        /// Gets designer host of the component.
        /// </summary>
        /// <param name="component">Component to get designer host from.</param>
        /// <returns>Designer host of the control.</returns>
        private IDesignerHost GetDesignerHost(Component component)
        {
            if (component != null && component.Site != null) return component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

            return null;
        }

        /// <summary>
        /// Adds new toolstrip item to the control.
        /// </summary>
        /// <param name="itemType">ToolStripItem child type indicating type of component that has to be created.</param>
        /// <param name="group">Group to add item to.</param>
        /// <returns>Newly created component instance.</returns>
        private ToolStripItem AddNewItem(Type itemType, RibbonTabGroup group)
        {
            if (group == null) throw new ArgumentNullException("group");
            if (!itemType.IsSubclassOf(typeof(ToolStripItem))) throw new ArgumentOutOfRangeException("itemType");

            ToolStripItem item = (ToolStripItem)CreateHostedComponent(itemType);

            if (item == null) return null;

            if (item is RibbonTabItem)
            {
                ((RibbonTabItem)item).Page = (RibbonTabPage)CreateHostedComponent(typeof(RibbonTabPage));
            }

            return m_control.AddItem(item, group);
        }

        /// <summary>
        /// Deactivates all groups so that they don't highlight selected items.
        /// </summary>
        private void DeactivateGroups()
        {
            foreach (RibbonTabGroup iterGroup in m_control.Groups)
            {
                iterGroup.Active = false;
                iterGroup.InvalidateWithInnerControl();
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Gets verbs collection.
        /// </summary>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                return m_verbs;
            }
        }

        /// <summary>
        /// Gets action list.
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                return m_actionList;
            }
        }

        /// <summary>
        /// Paints dash border.
        /// </summary>
        /// <param name="pe"> PaintEventArgs that contains the event data.</param>
        protected override void OnPaintAdornments(System.Windows.Forms.PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);

            pe.Graphics.DrawRectangle(_adornmentsBorderPen, 0, 0, m_control.Width - 1, m_control.Height - 1);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Adds new item to the group.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
       public void GroupDesigner_AddNewItem(object sender, AddNewToolStripItemEventArgs e)
        {
            AddNewItem(e.Type, (RibbonTabGroup)sender);
        }

        /// <summary>
        /// Initializes group designer.
        /// </summary>
       /// <param name="sender">Sender object</param>
       /// <param name="args"> RibbonTabGroupEventArgs that contains the event data.</param>
       public void Groups_GroupAdded(object sender, RibbonTabGroupEventArgs args)
        {
            RibbonTabGroup group = args.Group;

            IDesignerHost groupDesignerHost = GetDesignerHost(group);

            if (groupDesignerHost != null)
            {
                RibbonTabGroupDesigner groupDesigner = groupDesignerHost.GetDesigner(group) as RibbonTabGroupDesigner;

                if (groupDesigner != null)
                {
                    groupDesigner.AddNewToolStripItem += new AddNewToolStripItemEventHandler(GroupDesigner_AddNewItem);
                    groupDesigner.GroupClicked += new ToolStripItemEventHandler(GroupDesigner_GroupClicked);
                    groupDesigner.AddNewSingleItem += new NewItemDroppedAtSingleItemGroupEventHandler(GroupDesigner_AddNewSingleItem);
                }
            }
        }

        /// <summary>
        /// Activates corresponding group so that it highlights clicked item.
        /// </summary>
       /// <param name="sender">Sender object</param>
       /// <param name="e"> EventArgs that contains the event data.</param>
       public void GroupDesigner_GroupClicked(object sender, ToolStripItemEventArgs e)
        {
            RibbonTabGroup group = sender as RibbonTabGroup;

            if (group != null)
            {
                DeactivateGroups();

                group.Active = true;
                group.Invalidate();
            }
        }

        /// <summary>
        /// Adds new single item.
        /// </summary>
       /// <param name="sender">Sender object</param>
       /// <param name="args">NewItemDroppedAtSingleItemGroupEventArgs that contains the event data.</param>
       public void GroupDesigner_AddNewSingleItem(object sender, NewItemDroppedAtSingleItemGroupEventArgs args)
        {
            RibbonTabGroup newGroup = AddNewGroup();
            newGroup.SingleItem = true;
            m_control.AddItem(args.Item, newGroup);

            m_control.Header.Groups.Remove(newGroup);

            RibbonTabGroup groupAtInsertionPlace = (RibbonTabGroup)sender;
            int iInsertPlace = m_control.Groups.IndexOf(groupAtInsertionPlace);

            if (!args.CloserToLeft) iInsertPlace++;

            m_control.Groups.Insert(iInsertPlace, newGroup);
            ((Control)m_control.Header).PerformLayout();
        }

        /// <summary>
        /// Removes selected item highlighting if needed.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
       public void SelectionService_SelectionChanged(object sender, EventArgs e)
        {
            ISelectionService selectionService = (ISelectionService)sender;

            ICollection selectedComonents = selectionService.GetSelectedComponents();

            if (selectedComonents.Count != 1)
            {
                DeactivateGroups();
            }
            else
            {
                IEnumerator enumerator = selectedComonents.GetEnumerator();
                enumerator.MoveNext();
                IComponent selectedComp = (IComponent)enumerator.Current;

                if (!(selectedComp is ToolStripItem)) DeactivateGroups();

                // Don't allow to select "single item" group.
                if (selectedComp is RibbonTabGroup && ((RibbonTabGroup)selectedComp).SingleItem)
                {
                    selectionService.SetSelectedComponents(new Component[] { m_control.Header.GetTabsHeader() });
                }
            }
        }
        
        /// <summary>
        /// Removes single item group if single item was removed.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
       public void ChangeService_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            IDesignerHost designerHost = GetDesignerHost(m_control);

            if (designerHost != null)
            {
                ToolStripItem item = e.Component as ToolStripItem;

                if (item != null)
                {
                    RibbonTabGroup group = item.Owner as RibbonTabGroup;

                    if (group != null && group.SingleItem)
                    {
                        group.Items.Clear();
                        m_control.Groups.Remove(group);
                        designerHost.DestroyComponent(group);
                    }
                }

                RibbonTabItem tabItem = item as RibbonTabItem;

                if (tabItem != null)
                {
                    designerHost.DestroyComponent(tabItem.Page);
                }
            }
        }
        #endregion
    }
}
#endif