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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <summary>
    /// Designer for RibbonTabGroup.
    /// </summary>
    public class RibbonTabGroupDesigner
        : ControlDesigner
    {
        #region Fields
        
        /// <summary>
        /// Design time RibbonTabGroup instance.
        /// </summary>
        private RibbonTabGroup m_control;

        /// <summary>
        /// Collection of verbs.
        /// </summary>
        private DesignerVerbCollection m_verbs = new DesignerVerbCollection();

        /// <summary>
        /// Glyph for tab group.
        /// </summary>
        private TabGroupGlyph m_groupGlyph;

        /// <summary>
        /// Behavior for tab group.
        /// </summary>
        private TabGroupBehavior m_groupBehavior;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of RibbonTabGroupDesigner.
        /// </summary>
        /// <param name="component">Component Value</param>
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            m_control = (RibbonTabGroup)component;

            m_verbs.Add(new DesignerVerb("Add tab item", new EventHandler(AddTabItem)));
            m_verbs.Add(new DesignerVerb("Add toolstrip button", new EventHandler(AddButton)));
            m_verbs.Add(new DesignerVerb("Add toolstrip label", new EventHandler(AddLabel)));
            m_verbs.Add(new DesignerVerb("Add toolstrip separator", new EventHandler(AddSeparator)));
            m_verbs.Add(new DesignerVerb("Add toolstrip combobox", new EventHandler(AddCombobox)));
            m_verbs.Add(new DesignerVerb("Add toolstrip textbox", new EventHandler(AddTextbox)));
            m_verbs.Add(new DesignerVerb("Add toolstrip progressbar", new EventHandler(AddProgressBar)));

            // m_verbs.Add( new DesignerVerb( "Add split button", new EventHandler( AddSplitButton ) ) );
            // m_verbs.Add( new DesignerVerb( "Add dropdown button", new EventHandler( AddDropdownButton ) ) );
            m_groupBehavior = new TabGroupBehavior(m_control, BehaviorService);
            m_groupGlyph = new TabGroupGlyph(m_control, BehaviorService, m_groupBehavior);

            ToolStripExService.Get(component.Site);
        }
        #endregion

        #region Overrides
      
        protected override bool GetHitTest(Point point)
        {
            return true;
        }

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
        /// Adds new glyphs to collection.
        /// </summary>
        /// <param name="selectiontype">Glyph SelectionType</param>
        /// <returns>Returns Glyph Collection </returns>
        public override GlyphCollection GetGlyphs(GlyphSelectionType selectiontype)
        {
            GlyphCollection glyphs = base.GetGlyphs(selectiontype);

            glyphs.Add(m_groupGlyph);

            return glyphs;
        }
        #endregion

        #region Verbs
        /// <summary>
        /// Adds new tab item.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddTabItem(object sender, EventArgs e)
        {
            if (AddNewToolStripItem != null) AddNewToolStripItem(m_control, new AddNewToolStripItemEventArgs(typeof(RibbonTabItem)));
        }

        /// <summary>
        /// Adds new ToolStrip button.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddButton(object sender, EventArgs e)
        {
            if (AddNewToolStripItem != null) AddNewToolStripItem(m_control, new AddNewToolStripItemEventArgs(typeof(ToolStripButton)));
        }

        /// <summary>
        /// Adds new ToolStrip label.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddLabel(object sender, EventArgs e)
        {
            if (AddNewToolStripItem != null) AddNewToolStripItem(m_control, new AddNewToolStripItemEventArgs(typeof(ToolStripLabel)));
        }

        /// <summary>
        /// Adds new ToolStrip separator.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddSeparator(object sender, EventArgs e)
        {
            if (AddNewToolStripItem != null) AddNewToolStripItem(m_control, new AddNewToolStripItemEventArgs(typeof(ToolStripSeparator)));
        }

        /// <summary>
        /// Adds new combobox.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddCombobox(object sender, EventArgs e)
        {
            if (AddNewToolStripItem != null) AddNewToolStripItem(m_control, new AddNewToolStripItemEventArgs(typeof(ToolStripComboBox)));
        }

        /// <summary>
        /// Adds new textbox.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddTextbox(object sender, EventArgs e)
        {
            if (AddNewToolStripItem != null) AddNewToolStripItem(m_control, new AddNewToolStripItemEventArgs(typeof(ToolStripTextBox)));
        }

        /// <summary>
        /// Adds new progress bar.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void AddProgressBar(object sender, EventArgs e)
        {
            if (AddNewToolStripItem != null) AddNewToolStripItem(m_control, new AddNewToolStripItemEventArgs(typeof(ToolStripProgressBar)));
        }
       
        #endregion

        #region Events
        /// <summary>
        /// Raised when new toolstrip item should be added to the tab group.
        /// </summary>
        public event AddNewToolStripItemEventHandler AddNewToolStripItem;

        /// <summary>
        /// Raised when ribbon tab group is clicked.
        /// </summary>
        public event ToolStripItemEventHandler GroupClicked
        {
            add
            {
                m_groupBehavior.GroupClicked += value;
            }
            remove
            {
                m_groupBehavior.GroupClicked -= value;
            }
        }

        /// <summary>
        /// Raised when new single item has to be added.
        /// </summary>
        public event NewItemDroppedAtSingleItemGroupEventHandler AddNewSingleItem
        {
            add
            {
                m_groupBehavior.NewItemDroppedAtSingleItemGroup += value;
            }
            remove
            {
                m_groupBehavior.NewItemDroppedAtSingleItemGroup -= value;
            }
        }
        #endregion
    }
}
#endif