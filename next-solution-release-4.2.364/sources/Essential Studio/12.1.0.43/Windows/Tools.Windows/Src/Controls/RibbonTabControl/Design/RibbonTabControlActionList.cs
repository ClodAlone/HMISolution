#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// Action list for RibbonTabControlDesigner.
    /// </summary>
   public class RibbonTabControlActionList
        : DesignerActionList
    {
        #region Fields
        /// <summary>
        /// RibbonTabControlDesigner instance.
        /// </summary>
        private RibbonTabControlDesigner m_designer;

        /// <summary>
        /// List of action items.
        /// </summary>
        private DesignerActionItemCollection m_itemsList = new DesignerActionItemCollection();

        /// <summary>
        /// Design time RibbonTabControl instance.
        /// </summary>
        private RibbonTabControl m_control;
        #endregion

        #region Class Initialization
      
        /// <summary>
        /// Initializes a new instance of the RibbonTabControlActionList class.
        /// </summary>
        /// <param name="control">RibbonTab Control</param>
        /// <param name="designer">RibbonTabControl Designer</param>
        public RibbonTabControlActionList(RibbonTabControl control, RibbonTabControlDesigner designer)
            : base(control)
        {
            m_designer = designer;
            m_control = control;

            m_itemsList.Add(new DesignerActionHeaderItem("Essential Tools - RibbonTabControl"));

            m_itemsList.Add(new DesignerActionPropertyItem("Name", "Name"));
            m_itemsList.Add(new DesignerActionPropertyItem("Dock", "Dock"));
            m_itemsList.Add(new DesignerActionPropertyItem("FillWidthWithItems", "Fill Width With Items"));

            m_itemsList.Add(new DesignerActionHeaderItem("New items", "New Items"));
            m_itemsList.Add(new DesignerActionMethodItem(this, "AddGroup", "Add group", "New Items"));
            m_itemsList.Add(new DesignerActionMethodItem(this, "AddTabItem", "Add tab item", "New Items"));
            m_itemsList.Add(new DesignerActionMethodItem(this, "AddButton", "Add button", "New Items"));
            m_itemsList.Add(new DesignerActionMethodItem(this, "AddLabel", "Add label", "New Items"));
            m_itemsList.Add(new DesignerActionMethodItem(this, "AddSeparator", "Add separator", "New Items"));
            m_itemsList.Add(new DesignerActionMethodItem(this, "AddCombobox", "Add combobox", "New Items"));
            m_itemsList.Add(new DesignerActionMethodItem(this, "AddTextbox", "Add textbox", "New Items"));
            m_itemsList.Add(new DesignerActionMethodItem(this, "AddProgressBar", "Add progress bar", "New Items"));
            m_itemsList.Add(new DesignerActionMethodItem(this, "AddSplitButton", "Add split button", "New Items"));
            m_itemsList.Add(new DesignerActionMethodItem(this, "AddDropdownButton", "Add dropdown button", "New Items"));
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Returns collection of action list items.
        /// </summary>
        /// <returns>Returns item collection</returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            return m_itemsList;
        }
        #endregion

        #region List Items
        /// <summary>
        /// Gets or sets Name of the control.
        /// </summary>
        public string Name
        {
            get
            {
                return m_control.Name;
            }
            set
            {
                m_control.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets Dock of the control.
        /// </summary>
        public DockStyle Dock
        {
            get
            {
                return m_control.Dock;
            }
            set
            {
                m_control.Dock = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether with of control should be filled with RibbonTabItems.
        /// </summary>
        public bool FillWidthWithItems
        {
            get
            {
                return m_control.FillWidthWithItems;
            }
            set
            {
                m_control.FillWidthWithItems = value;
            }
        }

        /// <summary>
        /// Adds new ribbon tab group.
        /// </summary>
        public void AddGroup()
        {
            m_designer.AddNewGroup();
        }

        /// <summary>
        /// Adds new ribbon tab item.
        /// </summary>
        public void AddTabItem()
        {
            m_designer.AddItem(typeof(RibbonTabItem));
        }

        /// <summary>
        /// Adds new toolstrip button.
        /// </summary>
        public void AddButton()
        {
            m_designer.AddItem(typeof(ToolStripButton));
        }

        /// <summary>
        /// Adds new toolstrip label.
        /// </summary>
        public void AddLabel()
        {
            m_designer.AddItem(typeof(ToolStripLabel));
        }

        /// <summary>
        /// Adds new toolstrip separator.
        /// </summary>
        public void AddSeparator()
        {
            m_designer.AddItem(typeof(ToolStripSeparator));
        }

        /// <summary>
        /// Adds new combobox.
        /// </summary>
        public void AddCombobox()
        {
            m_designer.AddItem(typeof(ToolStripComboBox));
        }

        /// <summary>
        /// Adds new textbox.
        /// </summary>
        public void AddTextbox()
        {
            m_designer.AddItem(typeof(ToolStripTextBox));
        }

        /// <summary>
        /// Adds new progress bar.
        /// </summary>
        public void AddProgressBar()
        {
            m_designer.AddItem(typeof(ToolStripProgressBar));
        }
      
        /// <summary>
        /// Adds new split button.
        /// </summary>
        public void AddSplitButton()
        {
            m_designer.AddItem(typeof(ToolStripSplitButton));
        }

        /// <summary>
        /// Adds new dropdown button.
        /// </summary>
        public void AddDropdownButton()
        {
            m_designer.AddItem(typeof(ToolStripDropDownButton));
        }
        #endregion
    }
}
#endif
