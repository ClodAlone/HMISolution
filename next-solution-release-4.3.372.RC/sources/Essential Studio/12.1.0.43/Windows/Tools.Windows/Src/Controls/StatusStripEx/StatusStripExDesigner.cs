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
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    public class StatusStripExDesigner : ControlDesigner
    {
        #region Constructors
    
        public StatusStripExDesigner()
        {
            m_statusStripGlyphs = new GlyphCollection();
        }
        #endregion

        #region Nested classes
        /// <summary>
        /// Action list for RibbonControlAdvHeaderDesigner.
        /// </summary>
        private class StatusStripExDesignerActionList
            : DesignerActionList
        {
            #region Initialization
            /// <summary>
            /// Initializes a new instance of the StatusStripExDesignerActionList class.
            /// </summary>
            /// <param name="control">Design time StatusStripEx instance.</param>
            /// <param name="designer">Underlying StatusStripExDesigner.</param>
            public StatusStripExDesignerActionList(StatusStripEx control, StatusStripExDesigner designer)
                : base(control)
            {
                m_designer = designer;
                m_control = control;

                m_actionItems = new DesignerActionItemCollection();

                m_actionItems.Add(new DesignerActionHeaderItem("Properties", "Properties"));
                m_actionItems.Add(new DesignerActionPropertyItem("Dock", "Dock", "Properties", "Gets or sets which control borders are docked to its parent control and determines how a control is resized with its parent."));

                m_actionItems.Add(new DesignerActionHeaderItem("StatusControls items", "StatusControls items"));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddStatusLabel", "Add StatusLabel", "StatusControls items", "Adds status label."));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddProgressBar", "Add ProgressBar", "StatusControls items", "Adds progress bar."));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddDropDownButton", "Add DropDownButton", "StatusControls items", "Adds dropdown button."));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddSplitButton", "Add SplitButton", "StatusControls items", "Adds split button."));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddPanelItem", "Add PanelItem", "StatusControls items", "Adds panel item."));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddTrackBarItem", "Add TrackBarItem", "StatusControls items", "Adds track bar item."));

                m_actionItems.Add(new DesignerActionHeaderItem("Notifications items", "Notifications items"));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddStatusStripButton", "Add StatusStripButton", "Notifications items", "Adds status strip button."));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddStatusStripLabel", "Add StatusStripLabel", "Notifications items", "Adds status strip label"));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddStatusProgressBar", "Add StatusStrip ProgressBar", "Notifications items", "Adds status  strip progress bar."));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddStatusDropDownButton", "Add StatusStrip DropDownButton", "Notifications items", "Adds status strip dropdown button."));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddStatusSplitButton", "Add StatusStrip SplitButton", "Notifications items", "Adds status strip split button."));
                m_actionItems.Add(new DesignerActionMethodItem(this, "AddStatusPanelItem", "Add StatusStrip PanelItem", "Notifications items", "Adds status strip panel item."));
            }
            #endregion

            #region Private Properties
            /// <summary> Gets or sets Dock of the control. </summary>
            public DockStyleEx Dock
            {
                get
                {
                    return m_control.Dock;
                }
                set
                {
                    PropertyDescriptor pdDock = TypeDescriptor.GetProperties(m_control)["Dock"];

                    if (pdDock != null)
                    {
                        pdDock.SetValue(m_control, value);
                    }
                }
            }
            #endregion

            #region Private Methods
            /// <summary>
            /// Adds new StatusLabel.
            /// </summary>
            private void AddStatusLabel()
            {
                m_designer.AddItem(typeof(ToolStripStatusLabel));
            }

            /// <summary>
            /// Adds new DropDownButton.
            /// </summary>
            private void AddDropDownButton()
            {
                m_designer.AddItem(typeof(ToolStripDropDownButton));
            }

            /// <summary>
            /// Adds new SplitButton.
            /// </summary>
            private void AddSplitButton()
            {
                m_designer.AddItem(typeof(ToolStripSplitButton));
            }

            /// <summary>
            /// Adds new PanelItem.
            /// </summary>
            private void AddPanelItem()
            {
                m_designer.AddItem(typeof(ToolStripPanelItem));
            }

            /// <summary>
            /// Adds new TrackBarItem.
            /// </summary>
            private void AddTrackBarItem()
            {
                m_designer.AddItem(typeof(TrackBarItem));
            }

            /// <summary>
            /// Adds new ProgressBar.
            /// </summary>
            private void AddProgressBar()
            {
                m_designer.AddItem(typeof(ToolStripProgressBar));
            }

            /// <summary>
            /// Adds new StatusStripButton.
            /// </summary>
            private void AddStatusStripButton()
            {
                m_designer.AddItem(typeof(StatusStripButton));
            }

            /// <summary>
            /// Adds new StatusStripLabel.
            /// </summary>
            private void AddStatusStripLabel()
            {
                m_designer.AddItem(typeof(StatusStripLabel));
            }

            /// <summary>
            /// Adds new ProgressBar to the status bar.
            /// </summary>
            private void AddStatusProgressBar()
            {
                m_designer.AddItem(typeof(StatusStripProgressBar));
            }

            /// <summary>
            /// Adds new DropDownButton to the status bar.
            /// </summary>
            private void AddStatusDropDownButton()
            {
                m_designer.AddItem(typeof(StatusStripDropDownButton));
            }

            /// <summary>
            /// Adds new SplitButton to the status bar.
            /// </summary>
            private void AddStatusSplitButton()
            {
                m_designer.AddItem(typeof(StatusStripSplitButton));
            }

            /// <summary>
            /// Adds new PanelItem to the status bar.
            /// </summary>
            private void AddStatusPanelItem()
            {
                m_designer.AddItem(typeof(StatusStripPanelItem));
            }

            #endregion

            #region Overrides
            /// <summary>
            /// Returns collection of action list items.
            /// </summary>
            /// <returns>Returns collection of action list items</returns>
            public override DesignerActionItemCollection GetSortedActionItems()
            {
                return m_actionItems;
            }
            #endregion

            #region Fields
            /// <summary>
            /// Underlying StatusStripExDesigner.
            /// </summary>
            private StatusStripExDesigner m_designer;

            /// <summary>
            /// Collection of action items.
            /// </summary>
            private DesignerActionItemCollection m_actionItems;

            /// <summary>
            /// Design time StatusStripEx instance.
            /// </summary>
            private StatusStripEx m_control;
            #endregion
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Initializes new instance of RibbonTabGroupDesigner.
        /// </summary>
        /// <param name="component">Component parameter</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            m_control = (StatusStripEx)component;

            if (m_control != null)
            {
                m_control.ItemAdded += new ToolStripItemEventHandler(OnItemAdded);

                m_actionLists = new DesignerActionListCollection();
                m_actionLists.Add(new StatusStripExDesignerActionList(m_control, this));

                m_ribbonAdornerSvc = RibbonAdornerService.Get(m_control.Site);
                m_toolStripSvc = ToolStripExService.Get(m_control.Site);
            }

            UpdateGlyphs();
        }

        private void OnItemAdded(object sender, ToolStripItemEventArgs e)
        {
            // As item is added glyphs for it must be updated.
            UpdateGlyphs();
        }

        /// <summary>
        /// Updates all glyphs for StatusStripEx's items.
        /// </summary>
        private void UpdateGlyphs()
        {
            if (m_ribbonAdornerSvc != null)
            {
                GlyphCollection glyphs = m_ribbonAdornerSvc.Adorner.Glyphs;

                foreach (Glyph g in m_statusStripGlyphs)
                {
                    glyphs.Remove(g);
                }

                m_statusStripGlyphs.Clear();

                for (int i = 0, count = m_control.Items.Count; i < count; i++)
                {
                    ToolStripItem item = m_control.Items[i];

                    ComponentGlyph glyph = DesignerUtils.GetToolStripItemGlyph(item);

                    if (glyph != null)
                    {
                        m_statusStripGlyphs.Insert(0, glyph);
                    }
                }

                glyphs.AddRange(m_statusStripGlyphs);
            }
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                return m_actionLists;
            }
        }
 
        public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
        {
            GlyphCollection gc = base.GetGlyphs(selectionType);

            foreach (ToolStripItem item in m_control.Items)
            {
                Glyph glyph = DesignerUtils.GetToolStripItemGlyph(item);

                if (glyph != null)
                {
                    gc.Add(glyph);
                }
            }

            return gc;
        }
        #endregion

        #region Implementation
        internal void AddItem(Type itemType)
        {
            IDesignerHost host = m_control.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

            if (host != null)
            {
                ToolStripItem item = (ToolStripItem)host.CreateComponent(itemType);

                if (item != null)
                {
                    item.Text = item.Name;

                    m_control.Items.Add(item);
                }
            }
        }
        #endregion

        #region Fields
        /// <summary> Design time StatusStripEx instance. </summary>
        private StatusStripEx m_control;

        /// <summary> Action lists. </summary>
        private DesignerActionListCollection m_actionLists;

        /// <summary>
        /// StatusStripEx items' glyphs.
        /// </summary>
        private GlyphCollection m_statusStripGlyphs;

        /// <summary>
        /// RibbonAdorner service to have access to StatusStripEx glyphs.
        /// </summary>
        private RibbonAdornerService m_ribbonAdornerSvc;
        
        private ToolStripExService m_toolStripSvc;
        #endregion
    }
}
#endif
