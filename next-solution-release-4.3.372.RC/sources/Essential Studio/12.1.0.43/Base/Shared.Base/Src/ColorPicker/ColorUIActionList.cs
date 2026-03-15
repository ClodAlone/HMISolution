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
    public class ColorUIActionList : SyncActionListBase<ColorUIControl>
    {
        /// <summary>
        /// Initializes a new instance of the ClockActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public ColorUIActionList(IComponent component)
            : base(component)
        {
        }
        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - ColorUIControl");
            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");
            //Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("Text", "Text", "Appearance", "The text associated with the control.");
            this.AddDesignerActionPropertyItem("BStyle", "Border Style", "Appearance", "BorderStyle for the control.");
            this.AddDesignerActionPropertyItem("Style", "Visual Style", "Appearance", "Visual Style for the control.");

        }
        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        public string Text
        {
            get
            {
                string text = " ";
                if (this.Control != null)
                {
                    ColorUIControl control = this.Control as ColorUIControl;
                    text = control.Text;
                }

                return text;
            }

            set
            {
                SetValue("Text", value);
            }
        }
        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        /// <value>The style.</value>
        public ColorUIStyle Style
        {
            get
            {
                ColorUIStyle style = ColorUIStyle.Default;
                if (this.Control != null)
                {
                    ColorUIControl control = this.Control as ColorUIControl;
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
        /// Gets or sets the visual style.
        /// </summary>
        /// <value>The style.</value>
        public BorderStyle BStyle
        {
            get
            {
                BorderStyle BStyle = BorderStyle.Fixed3D;
                if (this.Control != null)
                {
                    ColorUIControl control = this.Control as ColorUIControl;
                    BStyle = control.BorderStyle;
                }

                return BStyle;
            }
            set
            {
                SetValue("BorderStyle", value);
            }
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
                    ColorUIControl control = this.Control as ColorUIControl;
                    name = control.Name;
                }

                return name;
            }

            set
            {
                SetValue("Name", value);
            }
        }
    }
}
#endif