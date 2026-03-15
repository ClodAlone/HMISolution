#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Edit.Enums;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Edit.Design
{
    /// <summary>
    /// Designer for EditControl and related controls.
    /// </summary>
    public class EditControlDesigner
    : ControlDesigner
    {
        #region Overrides
        /// <summary>
        /// Removes unnecessary properties from property grid.
        /// </summary>
        /// <param name="properties">The properties for EditControl.</param>
        protected override void PostFilterProperties(IDictionary properties)
        {
            base.PostFilterProperties(properties);

            properties.Remove("BackgroundImage");
            properties.Remove("Cursor");
            properties.Remove("Font");
            properties.Remove("ForeColor");
            properties.Remove("AccelerateScrolling");
            properties.Remove("AllowIncreaseSmallChange");
            properties.Remove("AllowSizeGrip");
            properties.Remove("EnableIntelliMouse");
            properties.Remove("FillSplitterPane");
            properties.Remove("ForceSizeBox");
            properties.Remove("HorizontalScrollTips");
            properties.Remove("HorizontalThumbTrack");
            properties.Remove("HScroll");
            properties.Remove("ScrollTipFormat");
            properties.Remove("SizeGripStyle");
            properties.Remove("SmartSizeBox");
            properties.Remove("VerticalScrollTips");
            properties.Remove("VerticalThumbTrack");
            properties.Remove("VScroll");
        }

        /// <summary>
        /// Gets selection rules for the EditControl. In some cases control sizing can be limited (SingleLine mode or AutoSize mode).
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                EditControl control = (EditControl)this.Control;
                SelectionRules rules = SelectionRules.Visible | SelectionRules.Moveable;
                if (!control.AutoSize)
                {
                    if (control.SingleLineMode)
                    {
                        rules |= SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
                    }
                    else
                    {
                        rules |= SelectionRules.AllSizeable;
                    }
                }
                else
                {
                    if (control.WordWrap && control.WordWrapMode == WordWrapMode.Control)
                    {
                        rules |= SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
                    }
                }

                return rules;
            }
        }

        /// <summary>
        /// Initializes EditControlDesigner.
        /// </summary>
        /// <param name="component">The EditControl.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            EditControl control = component as EditControl;
            if (control != null)
            {
                control.ParentChanged += new EventHandler(OnControlParentChanged);
            }

#if SyncfusionFramework2_0
            // Add ScrollersFrame component to designer host. For enabling ScrollersFrame's designer.
            INestedContainer nestedContainer = this.GetService(typeof(INestedContainer)) as INestedContainer;
            if (nestedContainer != null)
            {
                EditControl editor = component as EditControl;
                if (editor != null && editor.m_scrollersFrame != null)
                {
                    nestedContainer.Add(editor.m_scrollersFrame);
                }
            }
#endif
        }

        /// <summary>
        /// Disposes EditControlDesigner.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            EditControl control = this.Component as EditControl;

            if (control != null)
            {
                control.ParentChanged -= new EventHandler(OnControlParentChanged);
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Prevents EditControl child controls from being unhooked and receiving mouse messages.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The event argument</param>
        private void OnControlParentChanged(object sender, EventArgs e)
        {
            HookChildControls((Control)sender);
        }
        #endregion

        #region Action List Call
        private DesignerActionListCollection actionListCollection;

        /// <summary>
        /// Gets the design-time action lists supported by the component associated with the designer.
        /// </summary>
        /// <value>Design-time action lists</value>
        /// <returns>
        /// The design-time action lists supported by the component associated with the designer.
        /// </returns>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (actionListCollection == null)
                {
                    actionListCollection = new DesignerActionListCollection();
                    actionListCollection.Add(new EditControlActionList(this.Component));
                }

                return actionListCollection;
            }
        }

        #endregion
    }

    #region Action List Class
    internal class EditControlActionList : DesignerActionList
    {
        #region Initialization

        private EditControl editControl;
        private DesignerActionUIService designerService;

        public EditControlActionList(IComponent component)
            : base(component)
        {
            editControl = (EditControl)component;
            designerService = (DesignerActionUIService)this.GetService(typeof(DesignerActionUIService));
        }

        #endregion

        #region Properties

        //Appearance Properties
        public bool ShowLineNumbers
        {
            get { return editControl.ShowLineNumbers; }
            set { SetProperty("ShowLineNumbers", value); }
        }

        public bool ShowOutliningCollapsers
        {
            get { return editControl.ShowOutliningCollapsers; }
            set { SetProperty("ShowOutliningCollapsers",value);}
        }

        public bool SingleLineMode
        {
            get { return editControl.SingleLineMode; }
            set { SetProperty("SingleLineMode", value); 

            }
        }

        //Behavior Properties
        public bool ConvertOnLoad
        {
            get { return editControl.ConvertOnLoad; }
            set { SetProperty("ConvertOnLoad",value);}
        }

        public bool ReadOnly
        {
            get { return editControl.ReadOnly; }
            set { SetProperty("ReadOnly",value);}
        }

        public bool SaveOnClose
        {
            get { return editControl.SaveOnClose; }
            set { SetProperty("SaveOnClose",value);}
        }

        //Column Guidelines
        public bool ShowColumnGuides
        {
            get { return editControl.ShowColumnGuides; }
            set { SetProperty("ShowColumnGuides",value);}
        }

        //Data
        public String Text
        {
            get {  return editControl.Text; }
            set { SetProperty("Text",value);}
        }

        //Layout
        public DockStyle Dock
        {
            get { return editControl.Dock; }
            set { SetProperty("Dock",value);}
        }

        //ScrollBarButtons
        public bool AlwaysShowScrollers
        {
            get { return editControl.AlwaysShowScrollers; }
            set { SetProperty("AlwaysShowScrollers",value);}
        }

        //WordWrapping
        public bool WordWrap
        {
            get { return editControl.WordWrap; }
            set { SetProperty("WordWrap",value);}
        }

        public WordWrapMode WordWrapMode
        {
            get { return editControl.WordWrapMode; }
            set { SetProperty("WordWrapMode",value);}
        }

        /// <summary>
        /// To write or modify the property value into initilalizeCompoinent()
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="value">new value</param>
        private void SetProperty(string propertyName, object value)
        {

            //SD3966 Fixed: This function for resolve the Smart tag panel Issue
            // Get property
            PropertyDescriptor property =
              TypeDescriptor.GetProperties(this.editControl)[propertyName];
            // Set property value
            if (property != null)
            {
                property.SetValue(editControl, value);
            }
           
            //End
        }

        #endregion

        #region overrides
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();

            //Headers
            items.Add(new DesignerActionHeaderItem("Syncfusion Essential Edit"));

            //Properties

            //Appearance
            items.Add(new DesignerActionTextItem("Appearance", "Appearance"));
            items.Add(new DesignerActionPropertyItem("ShowLineNumbers", "ShowLineNumbers", "Appearance"));
            items.Add(new DesignerActionPropertyItem("ShowOutliningCollapsers", "ShowOutliningCollapsers", "Appearance"));
            items.Add(new DesignerActionPropertyItem("SingleLineMode", "SingleLineMode", "Appearance"));

            //Behavior
            items.Add(new DesignerActionTextItem("Behavior", "Behavior"));
            items.Add(new DesignerActionPropertyItem("ConvertOnLoad", "ConvertOnLoad", "Behavior"));
            items.Add(new DesignerActionPropertyItem("ReadOnly", "ReadOnly", "Behavior"));
            items.Add(new DesignerActionPropertyItem("SaveOnClose", "SaveOnClose", "Behavior"));

            //Column Guidelines
            items.Add(new DesignerActionTextItem("Column Guidelines", "Column Guidelines"));
            items.Add(new DesignerActionPropertyItem("ShowColumnGuides", "ShowColumnGuides", "Column Guidelines"));

            //Data
            items.Add(new DesignerActionTextItem("Data", "Data"));
            items.Add(new DesignerActionPropertyItem("Text", "Text", "Data"));

            //Layout
            items.Add(new DesignerActionTextItem("Layout", "Layout"));
            items.Add(new DesignerActionPropertyItem("Dock", "Dock", "Layout"));

            //ScrollbarButtons
            items.Add(new DesignerActionTextItem("ScrollbarButtons", "ScrollbarButtons"));
            items.Add(new DesignerActionPropertyItem("AlwaysShowScrollers", "AlwaysShowScrollers", "ScrollbarButtons"));

            //WordWrapping
            items.Add(new DesignerActionTextItem("WordWrapping", "WordWrapping"));
            items.Add(new DesignerActionPropertyItem("WordWrap", "WordWrap", "WordWrapping"));
            items.Add(new DesignerActionPropertyItem("WordWrapMode", "WordWrapMode", "WordWrapping"));

            return items;
        }
        #endregion
    }

    #endregion
}