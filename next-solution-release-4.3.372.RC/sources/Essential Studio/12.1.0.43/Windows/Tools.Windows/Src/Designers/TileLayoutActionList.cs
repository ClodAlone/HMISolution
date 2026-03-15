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
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Tools
{
    public class TileLayoutActionList : SyncActionListBase<TileLayout>
    {

        public TileLayoutActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
           this.AddDesignerActionHeaderItem("Essential Tools - TileLayout");

           this.AddDesignerActionPropertyItem("Name", "Name", "Appearance_1", "Indicates the Name used in code to identify the Object.");
           this.AddDesignerActionPropertyItem("Groups", "Groups Collection", "Appearance_1", "Contains the Group.");
           
           //Show/Hide Category
           this.AddDesignerActionHeaderItem("Appearance");
		    this.AddDesignerActionPropertyItem("BackColor", "Back Color", "Appearance", "Indicates the BackColor.");
           
           
           //Behavior Category
           this.AddDesignerActionHeaderItem("Behavior");
           this.AddDesignerActionPropertyItem("ShowItemPreview", "Show Item Preview", "Behavior", "Gets or sets a value indicating to show the preivew of the item");
           this.AddDesignerActionPropertyItem("AllowNewGroup", "Allow New Group", "Behavior", "Allow new group dynamically.");
           

            //Layout Category
            this.AddDesignerActionHeaderItem("Layout");
            this.AddDesignerActionPropertyItem("Dock", "Dock", "Layout", "Defines which borders of the control are to be bound to the container.");

        }

        public string Name
        {

            get
            {
                string name = " ";
                if (this.Control != null)
                {
                    TileLayout control = this.Control as TileLayout;
                    name = control.Name;
                }
                return name;
            }
            set
            {
                SetValue("Name", value);
            }
        }

        public TileGroupCollection Groups
        {
            get {
                TileGroupCollection group = new TileGroupCollection();

                if(this.Control != null)
                    group = this.Control.Groups ;
                return group ;
                }

            set { SetValue("Groups", value); }
        }


        public DockStyle Dock
        {

            get
            {
                DockStyle dock = DockStyle.None;
                if (this.Control != null)
                {
                    TileLayout control = this.Control as TileLayout;
                    dock = control.Dock;
                }
                return dock;
            }
            set
            {
                SetValue("Dock", value);
            }

        }


        public Color BackColor
        {

            get
            {
                Color backgroundColor = SystemColors.Control;
                if (this.Control != null)
                {
                    TileLayout control = this.Control as TileLayout;
                    backgroundColor = control.BackColor  ;
                }
                return backgroundColor;
            }
            set
            {
                SetValue("BackColor", value);
            }

        }


        public bool ShowItemPreview
        {

            get
            {
                bool showItemPreview = false;
                if (this.Control != null)
                {
                    TileLayout control = this.Control as TileLayout;
                    showItemPreview = control.ShowItemPreview;
                }
                return showItemPreview;
            }
            set
            {
                SetValue("ShowItemPreview", value);
            }

        }

        public bool AllowNewGroup
        {

            get
            {
                bool allowNewGroup = false;
                if (this.Control != null)
                {
                    TileLayout control = this.Control as TileLayout;
                    allowNewGroup = control.AllowNewGroup ;;
                }
                return allowNewGroup;
            }
            set
            {
                SetValue("AllowNewGroup", value);
            }

        }
    }
}
#endif
