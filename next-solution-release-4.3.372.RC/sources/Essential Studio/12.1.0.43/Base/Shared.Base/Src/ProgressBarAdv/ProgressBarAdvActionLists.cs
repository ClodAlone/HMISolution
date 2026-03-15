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
    /// CheckBoxAdvActionList class.
    /// </summary>
    public class ProgressBarAdvActionLists : SyncActionListBase<ProgressBarAdv>
    {
        /// <summary>
        /// Initializes a new instance of the ClockActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public ProgressBarAdvActionLists(IComponent component)
            : base(component)
        {
        }
        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - ProgressBarAdv");
            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");
            //Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("Style", "ProgressBarStyle", "Appearance", "The progressbar style of the control.");
            this.AddDesignerActionPropertyItem("Textstyle", "TextStyle", "Appearance", "Text style for the control.");
            this.AddDesignerActionPropertyItem("Textalignment", "Text Alignment", "Appearance", "Text alignment for the control.");
            this.AddDesignerActionPropertyItem("Textorientation", "Text Orientation", "Appearance", "Text orientation for the control.");
        }
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name
        {
            get
            {
                string name = " ";
                if (this.Control != null)
                {
                    ProgressBarAdv control = this.Control as ProgressBarAdv;
                    name = control.Name;
                }

                return name;
            }

            set
            {
                SetValue("Name", value);
            }
        }
        /// <summary>
        /// Gets or sets the Progressbar style.
        /// </summary>
        /// <value>The style.</value>
        public  ProgressBarStyles Style
        {
            get
            {
                ProgressBarStyles style = ProgressBarStyles.Gradient;
                if (this.Control != null)
                {
                    ProgressBarAdv control = this.Control as ProgressBarAdv;
                    style = control.ProgressStyle;
                }

                return style;
            }
            set
            {
                SetValue("ProgressStyle", value);
            }
        }
        /// <summary>
        /// Gets or sets the Text style.
        /// </summary>
        /// <value>The style.</value>
        public  ProgressBarTextStyles Textstyle
        {
            get
            {
                ProgressBarTextStyles style = ProgressBarTextStyles.Percentage;
                if (this.Control != null)
                {
                    ProgressBarAdv control = this.Control as ProgressBarAdv;
                    style = control.TextStyle;
                }

                return style;
            }
            set
            {
                SetValue("TextStyle", value);
            }
        }
        /// <summary>
        /// Gets or sets the Text style.
        /// </summary>
        /// <value>The style.</value>
        public  TextAlignment Textalignment
        {
            get
            {
                TextAlignment style = TextAlignment.Center;
                if (this.Control != null)
                {
                    ProgressBarAdv control = this.Control as ProgressBarAdv;
                    style = control.TextAlignment;
                }

                return style;
            }
            set
            {
                SetValue("TextAlignment", value);
            }
        }
        /// <summary>
        /// Gets or sets the Text orientation.
        /// </summary>
        /// <value>The style.</value>
        public  Orientation Textorientation
        {
            get
            {
                Orientation style = Orientation.Horizontal;
                if (this.Control != null)
                {
                    ProgressBarAdv control = this.Control as ProgressBarAdv;
                    style = control.TextOrientation;
                }

                return style;
            }
            set
            {
                SetValue("TextOrientation", value);
            }
        }
    }
}
#endif