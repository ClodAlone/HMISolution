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

namespace Syncfusion.Windows.Forms.Tools
{
    public class StatusBarAdvActionList : SyncActionListBase<StatusBarAdv>
    {
        public StatusBarAdvActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - StatusBarAdv");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("ThemesEnabled", "ThemesEnabled", "Appearance", "Indicates whether the Statusbar will draw a themed background. Indicated settings: BorderStyle = None.");
            this.AddDesignerActionPropertyItem("Alignment", "Alignment", "Appearance", "Gets or sets the alignment of the panels.");
            this.AddDesignerActionPropertyItem("AutoHeightControls", "AutoHeight Controls", "Appearance", "Indicates whether the StatusBar will resize the height of the panels according to it`s height.");
            this.AddDesignerActionPropertyItem("SizingGrip", "Sizing Grip", "Appearance", "Indicates if the Sizing grip is visible.");
            this.AddDesignerActionPropertyItem("Panels", "Panels", "Appearance", "Gets or sets the StatusBarAdvPanel controls contained in the StatusBarAdv.");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Layout");
            this.AddDesignerActionPropertyItem("AutoSize", "AutoSize", "Layout", "Indicates whether the control will size automatically to fit its contents.");

            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the Name used in code to identify the Object.");
        }

        public StatusBarAdvPanel[] Panels
        {
            get
            {
                ArrayList panels = new ArrayList();
                if (this.Control != null)
                {
                    StatusBarAdv control = this.Control as StatusBarAdv;
                    for (int i = 0; i < control.Panels.Length; i++)
                    {
                        if (!panels.Contains(control.Panels[i]))
                            panels.Add(control.Panels[i]);
                    }
                }
                return (StatusBarAdvPanel[])panels.ToArray(typeof(StatusBarAdvPanel));
            }
            set
            {
                SetValue("Panels", value);
            }
        }
        public FlowAlignment Alignment
        {
            get
            {
                FlowAlignment alignment = FlowAlignment.ChildConstraints;
                if (this.Control != null)
                {
                    StatusBarAdv control = this.Control as StatusBarAdv;
                    alignment = control.Alignment;
                }
                return alignment;
            }
            set
            {
                SetValue("Alignment", value);
            }
        }
        public bool SizingGrip
        {
            get
            {
                bool sizingGrip = true;
                if (this.Control != null)
                {
                    StatusBarAdv control = this.Control as StatusBarAdv;
                    sizingGrip = control.SizingGrip;
                }
                return sizingGrip;
            }
            set
            {
                SetValue("SizingGrip", value);
            }
        }
        public bool AutoHeightControls
        {
            get
            {
                bool autoHeightControls = true;
                if (this.Control != null)
                {
                    StatusBarAdv control = this.Control as StatusBarAdv;
                    autoHeightControls = control.AutoHeightControls;
                }
                return autoHeightControls;
            }
            set
            {
                SetValue("AutoHeightControls", value);
            }
        }
        public bool AutoSize
        {
            get
            {
                bool autoSize = false;
                if (this.Control != null)
                {
                    StatusBarAdv control = this.Control as StatusBarAdv;
                    autoSize = control.AutoSize;
                }
                return autoSize;
            }
            set
            {
                SetValue("AutoSize", value);
            }
        }
        public string Name
        {
            get
            {
                string name = " ";
                if (this.Control != null)
                {
                    StatusBarAdv control = this.Control as StatusBarAdv;
                    name = control.Name;
                }
                return name;
            }
            set
            {
                SetValue("Name", value);
            }
        }
        public bool ThemesEnabled
        {
            get
            {
                bool themesEnabled = true;
                if (this.Control != null)
                {
                    StatusBarAdv control = this.Control as StatusBarAdv;
                    themesEnabled = control.ThemesEnabled;
                }
                return themesEnabled;
            }
            set
            {
                SetValue("ThemesEnabled", value);
            }
        }
    }
}
#endif
