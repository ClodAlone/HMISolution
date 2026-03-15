#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if SyncfusionFramework2_0
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

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Contains the properties and its functionality of internal html behavior.
    /// </summary>
    public class HTMLUIActionList : SyncActionListBase<HTMLUIControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        public HTMLUIActionList(IComponent component)
            : base(component)
        {
        }
        /// <summary>
        /// overriding InitializeActionList
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential HTMLUI for Windows Forms");

            // Appearance category.
            this.AddDesignerActionPropertyItem("ShowTitle", "Show TitleBar", "Appearance", "Toggle the display of the Title Bar in the HTMLUI Control.");

            // Behavior Category
            this.AddDesignerActionPropertyItem("AutoScroll", "Scroll Automatically", "Behavior", "When the document exceeds the bounds scroll automatically.");
            this.AddDesignerActionPropertyItem("AutoRunScripts", "Run Embedded Scripts", "Behavior", "Run the scripts that are embedded in the document automatically.");
        }

        /// <summary>
        /// Gets or sets a bool property to make the title to show
        /// </summary>
        public bool ShowTitle
        {
            get
            {
                bool showTitle = true;
                if (this.Control != null)
                {
                    HTMLUIControl htmluiControl = this.Control as HTMLUIControl;
                    showTitle = htmluiControl.ShowTitle;
                }
                return showTitle;
            }
            set
            {
                SetValue("ShowTitle", value);
            }
        }

        /// <summary>
        /// Gets or sets a bool property to toggle betwen auto scrolling
        /// </summary>
        public bool AutoScroll
        {
            get
            {
                bool autoScroll = false;
                if (this.Control != null)
                {
                    HTMLUIControl htmluiControl = this.Control as HTMLUIControl;
                    autoScroll = htmluiControl.AutoScroll;
                }
                return autoScroll;
            }
            set
            {
                SetValue("AutoScroll", value);
            }
        }

        /// <summary>
        /// Gets or sets a bool value indicating whether to run the scripts.
        /// </summary>
        public bool AutoRunScripts
        {
            get
            {
                bool autoRunScripts = false;
                if (this.Control != null)
                {
                    HTMLUIControl htmluiControl = this.Control as HTMLUIControl;
                    autoRunScripts = htmluiControl.AutoRunScripts;
                }
                return autoRunScripts;
            }
            set
            {
                SetValue("AutoRunScripts", value);
            }
        }
    }
}
#endif