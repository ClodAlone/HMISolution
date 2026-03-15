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
    public class SplashPanelActionList : SyncActionListBase<SplashPanel>
    {
        public SplashPanelActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - SplashPanel");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("ShowAnimation", "Show Animation", "Appearance", "Specifies if the window display should be animated.");
            this.AddDesignerActionPropertyItem("ShowAsTopMost", "Show as TopMost", "Appearance", "Specifies if the SplashPanel is to be displayed as a TopMost window.");
            this.AddDesignerActionPropertyItem("AnimationSpeed", "Animation Speed", "Appearance", "The speed at which the animation unfolds on the screen and the SplashPanel becomes visible.");
            this.AddDesignerActionPropertyItem("DesktopAlignment", "Desktop Alignment", "Appearance", "Specifies how the splash screen has to be aligned when it appears initially with respect to the desktop.");

            // Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("SuspendAutoCloseWhenMouseOver", "Suspend AutoClose when MouseOver", "Behavior", "Specifies if the SplashPanel should not be closed when the mouse is over it.");
            this.AddDesignerActionPropertyItem("CloseOnClick", "Close on Click", "Behavior", "The SplashPanel will be closed if this set to true and the user clicks the SplashPanel.");
            this.AddDesignerActionPropertyItem("ShowInTaskbar", "Show in Taskbar", "Behavior", "Specifies if the SplashPanel is to be shown in the Taskbar");
            this.AddDesignerActionPropertyItem("TimerInterval", "TimerInterval", "Behavior", "Gets or sets the period of time the splash window should be visible for.");
            this.AddDesignerActionPropertyItem("FormIcon", "Form Icon", "Behavior", "Specifies the icon for the SplashPanel when displayed in the Taskbar.");

            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the Name used in code to identify the Object.");
        }

        public int AnimationSpeed
        {
            get
            {
                int animationSpeed = 10;
                if (this.Control != null)
                {
                    SplashPanel control = this.Control as SplashPanel;
                    animationSpeed = control.AnimationSpeed;
                }
                return animationSpeed;
            }
            set
            {
                SetValue("AnimationSpeed", value);
            }
        }
        public int TimerInterval
        {
            get
            {
                int timerInterval = 5000;
                if (this.Control != null)
                {
                    SplashPanel control = this.Control as SplashPanel;
                    timerInterval = control.TimerInterval;
                }
                return timerInterval;
            }
            set
            {
                SetValue("TimerInterval", value);
            }
        }
        public bool ShowAnimation
        {
            get
            {
                bool showAnimation = true;
                if (this.Control != null)
                {
                    SplashPanel control = this.Control as SplashPanel;
                    showAnimation = control.ShowAnimation;
                }
                return showAnimation;
            }
            set
            {
                SetValue("ShowAnimation", value);
            }
        }
        public string Name
        {
            get
            {
                string name = " ";
                if (this.Control != null)
                {
                    SplashPanel control = this.Control as SplashPanel;
                    name = control.Name;
                }
                return name;
            }
            set
            {
                SetValue("Name", value);
            }
        }
        public SplashAlignment DesktopAlignment
        {
            get
            {
                SplashAlignment desktopAlignment = SplashAlignment.Center;
                if (this.Control != null)
                {
                    SplashPanel control = this.Control as SplashPanel;
                    desktopAlignment = control.DesktopAlignment;
                }
                return desktopAlignment;
            }
            set
            {
                SetValue("DesktopAlignment", value);
            }
        }
        public bool ShowAsTopMost
        {
            get
            {
                bool showAsTopMost = true;
                if (this.Control != null)
                {
                    SplashPanel control = this.Control as SplashPanel;
                    showAsTopMost = control.ShowAsTopMost;
                }
                return showAsTopMost;
            }
            set
            {
                SetValue("ShowAsTopMost", value);
            }
        }
        public bool SuspendAutoCloseWhenMouseOver
        {
            get
            {
                bool suspendAutoCloseWhenMouseOver = false;
                if (this.Control != null)
                {
                    SplashPanel control = this.Control as SplashPanel;
                    suspendAutoCloseWhenMouseOver = control.SuspendAutoCloseWhenMouseOver;
                }
                return suspendAutoCloseWhenMouseOver;
            }
            set
            {
                SetValue("SuspendAutoCloseWhenMouseOver", value);
            }
        }
        public bool ShowInTaskbar
        {
            get
            {
                bool showInTaskbar = true;
                if (this.Control != null)
                {
                    SplashPanel control = this.Control as SplashPanel;
                    showInTaskbar = control.ShowInTaskbar;
                }
                return showInTaskbar;
            }
            set
            {
                SetValue("ShowInTaskbar", value);
            }
        }
        public bool CloseOnClick
        {
            get
            {
                bool closeOnClick = false;
                if (this.Control != null)
                {
                    SplashPanel control = this.Control as SplashPanel;
                    closeOnClick = control.CloseOnClick;
                }
                return closeOnClick;
            }
            set
            {
                SetValue("CloseOnClick", value);
            }
        }
        public Icon FormIcon
        {
            get
            {
                Icon formIcon = null;
                if (this.Control != null)
                {
                    SplashPanel control = this.Control as SplashPanel;
                    formIcon = control.FormIcon;
                }
                return formIcon;
            }
            set
            {
                SetValue("FormIcon", value);
            }
        }
    }
}
#endif
