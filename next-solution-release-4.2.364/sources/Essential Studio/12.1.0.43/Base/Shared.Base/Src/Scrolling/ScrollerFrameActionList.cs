#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws.
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
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

    /// <summary>
    /// RadioButtonAdvActionList Class.
    /// </summary>
    public class ScrollerFrameActionList : SyncActionListBase<ScrollersFrame>
    {
        /// <summary>
        /// Initializes a new instance of the RadioButtonAdvActionList class.
        /// </summary>
        /// <param name="component">Represents component</param>
        public ScrollerFrameActionList(IComponent component)
            : base(component)
        {
        }

        /// <summary>
        /// Gets or sets thumb color.
        /// </summary>
        public SizeGripperVisibility Sizegripper
        {
            get
            {
                SizeGripperVisibility size = SizeGripperVisibility.Visible;
                if (this.Control != null)
                {
                    ScrollersFrame control = this.Control as ScrollersFrame;
                    size = control.SizeGripperVisibility;
                }

                return size;
            }

            set
            {
                SetValue("SizeGripperVisibility", value);
            }
        }

        public  ScrollBarCustomDrawStyles Style
        {
            get
            {
                ScrollBarCustomDrawStyles style = ScrollBarCustomDrawStyles.WindowsXP;
                if (this.Control != null)
                {
                    ScrollersFrame control = this.Control as ScrollersFrame;
                    style = control.VisualStyle;
                }

                return style;
            }
            set
            {
                SetValue("VisualStyle", value);
            }
        }


        /// <summary>
        /// InitializeActionList method
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - ScrollerFrame");

            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Visualstyle", "Visual Style", "Design", "gets or sets the visual style");
            this.AddDesignerActionPropertyItem("Sizegripper", "Size Gripper Visibility", "Design", "Sizegripper visibility of the control");
        }
    }
}
#endif
