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
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    public class MultiColumnTreeViewActionList : SyncActionListBase<MultiColumnTreeView>
    {
        public MultiColumnTreeViewActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - MultiColumnTreeView");

            this.AddDesignerActionPropertyItem("Name", "Name", "Appearance_1", "Indicates the Name used in code to identify the Object.");
            this.AddDesignerActionMethodItem("EditNodes", "Edit Nodes Collection", "Appearance_1", "Indicates Top Level nodes of the treeview.");

            // Show/Hide Category
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("ShowCheckBoxes", "Show Check Boxes", "Appearance", "Indicates if check boxes will be shown for the nodes.");
            this.AddDesignerActionPropertyItem("ShowOptionButtons", "Show Option Buttons", "Appearance", "Indicates if the nodes will have an option button");
            this.AddDesignerActionPropertyItem("ShowLines", "Show Lines", "Appearance", "Indicates if the tree lines are visible");
            this.AddDesignerActionPropertyItem("ShowPlusMinus", "Show Plus Minus", "Appearance", "Indicates if the plus minus controls are visible");

            // ImageList Category
            this.AddDesignerActionHeaderItem("ImageList");
            this.AddDesignerActionPropertyItem("LeftImageList", "Left ImageList", "ImageList", "Indicates the image list that holds images to be drawn on the left of the node.");
            this.AddDesignerActionPropertyItem("RightImageList", "Right ImageList", "ImageList", "Indicates the image list that holds images to be drawn on the right of the node.");
            this.AddDesignerActionPropertyItem("StateImageList", "State ImageList", "ImageList", "Indicates the image list that holds images to be drawn on the state of the node.");

            // Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("LabelEdit", "Edit Label", "Behavior", "Gets/Sets a value indicating whether the label text of the tree nodes can be edited.");
            this.AddDesignerActionPropertyItem("LoadOnDemand", "Load Child nodes OnDemand", "Behavior", "Specifies if the tree should follow the load-on-demand paradigm.");
            this.AddDesignerActionPropertyItem("OwnerDrawNodes", "Owner Draw Nodes", "Behavior", "Indicates if the BeforeNodePaint event will be fired before drawing a node.");
            this.AddDesignerActionPropertyItem("HotTracking", "HotTracking", "Behavior", "Indicates if the nodes will have a hot tracked appearance when the mouse cursor is hovering over them");

            // Layout Category
            this.AddDesignerActionHeaderItem("Layout");
            this.AddDesignerActionPropertyItem("Anchor", "Anchor", "Layout", "Specifies the edges of the container to which a certain control is to be bound.");
            this.AddDesignerActionPropertyItem("Dock", "Dock", "Layout", "Defines which borders of the control are to be bound to the container.");
        }

        public string Name
        {
            get
            {
                string name = " ";
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    name = control.Name;
                }
                return name;
            }
            set
            {
                SetValue("Name", value);
            }
        }

        public AnchorStyles Anchor
        {
            get
            {
                AnchorStyles anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    anchor = control.Anchor;
                }
                return anchor;
            }
            set
            {
                SetValue("Anchor", value);
            }
        }
        public DockStyle Dock
        {
            get
            {
                DockStyle dock = DockStyle.None;
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    dock = control.Dock;
                }
                return dock;
            }
            set
            {
                SetValue("Dock", value);
            }
        }

        public void EditNodes()
        {
            if (this.Control != null)
            {
                MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                TreeViewAdvEditorForm editor = new TreeViewAdvEditorForm(control, this.GetService(typeof(IServiceProvider)) as IServiceProvider);                       
                editor.ShowDialog();
            }
        }

        public bool HotTracking
        {
            get
            {
                bool hotTracking = false;
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    hotTracking = control.HotTracking;
                }
                return hotTracking;
            }
            set
            {
                SetValue("HotTracking", value);
            }
        }
        public bool ShowCheckBoxes
        {
            get
            {
                bool showCheckBoxes = false;
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    showCheckBoxes = control.ShowCheckBoxes;
                }
                return showCheckBoxes;
            }
            set
            {
                SetValue("ShowCheckBoxes", value);
            }
        }
        public bool ShowOptionButtons
        {
            get
            {
                bool showOptionButtons = false;
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    showOptionButtons = control.ShowOptionButtons;
                }
                return showOptionButtons;
            }
            set
            {
                SetValue("ShowOptionButtons", value);
            }
        }
        public bool ShowLines
        {
            get
            {
                bool showLines = true;
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    showLines = control.ShowLines;
                }
                return showLines;
            }
            set
            {
                SetValue("ShowLines", value);
            }
        }
        public bool ShowPlusMinus
        {
            get
            {
                bool showPlusMinus = true;
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    showPlusMinus = control.ShowPlusMinus;
                }
                return showPlusMinus;
            }
            set
            {
                SetValue("ShowPlusMinus", value);
            }
        }
        public ImageList LeftImageList
        {
            get
            {
                ImageList leftImageList = new ImageList();
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    leftImageList = control.LeftImageList;
                }
                return leftImageList;
            }
            set
            {
                SetValue("LeftImageList", value);
            }
        }
        public ImageList RightImageList
        {
            get
            {
                ImageList rightImageList = new ImageList();
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    rightImageList = control.RightImageList;
                }
                return rightImageList;
            }
            set
            {
                SetValue("RightImageList", value);
            }
        }
        public ImageList StateImageList
        {
            get
            {
                ImageList stateImageList = new ImageList();
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    stateImageList = control.StateImageList;
                }
                return stateImageList;
            }
            set
            {
                SetValue("StateImageList", value);
            }
        }

        public bool LabelEdit
        {
            get
            {
                bool labelEdit = false;
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    labelEdit = control.LabelEdit;
                }
                return labelEdit;
            }
            set
            {
                SetValue("LabelEdit", value);
            }
        }
        public bool LoadOnDemand
        {
            get
            {
                bool loadOnDemand = false;
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    loadOnDemand = control.LoadOnDemand;
                }
                return loadOnDemand;
            }
            set
            {
                SetValue("LoadOnDemand", value);
            }
        }
        public bool OwnerDrawNodes
        {
            get
            {
                bool ownerDrawNodes = false;
                if (this.Control != null)
                {
                    MultiColumnTreeView control = this.Control as MultiColumnTreeView;
                    ownerDrawNodes = control.OwnerDrawNodes;
                }
                return ownerDrawNodes;
            }
            set
            {
                SetValue("OwnerDrawNodes", value);
            }
        }
    }
}
#endif
