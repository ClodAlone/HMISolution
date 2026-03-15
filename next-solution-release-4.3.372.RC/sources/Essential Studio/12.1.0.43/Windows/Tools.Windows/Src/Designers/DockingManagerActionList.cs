#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Text;
using System.Reflection;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;
using System.Globalization;

namespace Syncfusion.Windows.Forms.Tools
{
    public class DockingManagerActionList : SyncActionListBase<DockingManager>
    {

        public DockingManagerActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools for Windows Forms 2.0");
            
            //Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("DockLabelAlignment", "Dock Label Alignment Style", "Appearance", "Determines the alignment of dock labels.");
            this.AddDesignerActionPropertyItem("DockTabAlignment", "Dock Tab Alignment Style", "Appearance", "Determines the alignment of tabs in dock tab groups.");
            this.AddDesignerActionPropertyItem("PaintBorders", "Paint Borders", "Appearance", "Determines whether to paint docked controls borders");
           
            //Behavior Category.
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("DockToFill", "Dock with Fill Style", "Behavior", "Gets/Sets a value indicating whether docked controls will occupy the form's full client region.");
            this.AddDesignerActionPropertyItem("DisallowFloating", "Disallow Floating", "Behavior", "Gets/Sets a value indicating whether controls are allowed to be floated.");
            this.AddDesignerActionPropertyItem("DragProviderStyle", "Drag Provider Style", "Behavior", "Styles of helper frames which are used to dock a control easily into different edges.");
            this.AddDesignerActionPropertyItem("VisualStyle", "Docking Windows Visual Style", "Behavior", "Gets/Sets the different visual styles for docking windows.");
            
            //Serialization.
            this.AddDesignerActionHeaderItem("Serialization");
            this.AddDesignerActionPropertyItem("PersistState", "Docking Windows State Persist", "Serialization", "Gets/Sets the state of docking windows should be persist during application closing or not.");
         

        }
        public DockLabelAlignmentStyle DockLabelAlignment
        {
            get
            {
                DockLabelAlignmentStyle dockLabelAlignment = DockLabelAlignmentStyle.Default;
                if (this.Component != null)
                {
                    DockingManager dockingComponent = this.Component as DockingManager;

                    dockLabelAlignment = dockingComponent.DockLabelAlignment;
                }
                return dockLabelAlignment;
            }
            set
            {
                SetValue("DockLabelAlignment", value);
            }
        }

        public DockTabAlignmentStyle DockTabAlignment
        {
            get
            {
                DockTabAlignmentStyle dockTabAlignment = DockTabAlignmentStyle.Bottom;
                if (this.Component != null)
                {
                    DockingManager dockingComponent = this.Component as DockingManager;

                    dockTabAlignment = dockingComponent.DockTabAlignment;
                }
                return dockTabAlignment;
            }
            set
            {
                SetValue("DockTabAlignment", value);
            }
        }
        public bool PaintBorders
        {
            get
            {
                bool paintBorders = true;
                if (this.Component != null)
                {
                    DockingManager dockingComponent = this.Component as DockingManager;
                    paintBorders = dockingComponent.PaintBorders;
                }
                return paintBorders;
            }
            set
            {
                SetValue("PaintBorders", value);
            }
        }
        public bool ThemesEnabled
        {
            get
            {
                bool themesEnabled = false;
                if (this.Component != null)
                {
                    DockingManager dockingComponent = this.Component as DockingManager;
                    themesEnabled = dockingComponent.ThemesEnabled;
                }
                return themesEnabled;
            }
            set
            {
                SetValue("ThemesEnabled", value);
            }
        }
        public bool DockToFill
        {
            get
            {
                bool dockToFill = false;
                if (this.Component != null)
                {
                    DockingManager dockingComponent = this.Component as DockingManager;
                    dockToFill = dockingComponent.DockToFill;
                }
                return dockToFill;
            }
            set
            {
                SetValue("DockToFill", value);
            }
        }
        public bool DisallowFloating
        {
            get
            {
                bool disallowFloating = false;
                if (this.Component != null)
                {
                    DockingManager dockingComponent = this.Component as DockingManager;
                    disallowFloating = dockingComponent.DisallowFloating;
                }
                return disallowFloating;
            }
            set
            {
                SetValue("DisallowFloating", value);
            }
        }
        public Syncfusion.Windows.Forms.Tools.DragProviderStyle DragProviderStyle
        {
            get
            {
                Syncfusion.Windows.Forms.Tools.DragProviderStyle dragProviderStyle = Syncfusion.Windows.Forms.Tools.DragProviderStyle.VS2005;
                if (this.Component != null)
                {
                    DockingManager dockingComponent = this.Component as DockingManager;
                    dragProviderStyle = dockingComponent.DragProviderStyle;
                }
                return dragProviderStyle;
            }
            set
            {
                SetValue("DragProviderStyle", value);
            }
        }

        public Syncfusion.Windows.Forms.VisualStyle VisualStyle
        {
            get
            {
                Syncfusion.Windows.Forms.VisualStyle visualStyle = Syncfusion.Windows.Forms.VisualStyle.Default;
                if (this.Component != null)
                {
                    DockingManager dockingComponent = this.Component as DockingManager;
                    visualStyle = dockingComponent.VisualStyle;
                }
                return visualStyle;
            }
            set
            {
                SetValue("VisualStyle", value);
            }
        }

        public System.Windows.Forms.ImageList ImageList
        {
            get
            {
                System.Windows.Forms.ImageList imageList = null;
                if (this.Component != null)
                {
                    DockingManager dockingComponent = this.Component as DockingManager;
                    imageList = dockingComponent.ImageList;
                }
                return imageList;
            }
            set
            {
                SetValue("ImageList", value);
            }
        }

        public bool PersistState
        {
            get
            {
                bool persistState = false;
                if (this.Component != null)
                {
                    DockingManager dockingComponent = this.Component as DockingManager;
                    persistState = dockingComponent.PersistState;
                }
                return persistState;
            }
            set
            {
                SetValue("PersistState", value);
            }
        }
    }
}
#endif