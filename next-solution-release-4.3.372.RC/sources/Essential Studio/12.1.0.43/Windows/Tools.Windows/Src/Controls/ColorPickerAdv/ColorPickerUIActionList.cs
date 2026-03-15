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
    public class ColorPickerUIActionList : SyncActionListBase<ColorPickerUIAdv>
    {
        /// <summary>
        /// Initializes a new instance of the ClockActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public ColorPickerUIActionList(IComponent component)
            : base(component)
        {
        }
        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - ColorPickerUIAdv");
            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");
            //Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("Text", "Text", "Appearance", "The text associated with the control.");
            this.AddDesignerActionPropertyItem("Style", "Visual Style", "Appearance", "Visual Style for the control.");
            if (this.Style == ColorPickerUIAdv.visualstyle.Office2007)
                this.AddDesignerActionPropertyItem("Office2007ColorScheme", "Color Scheme", "Appearance", "Color Scheme for Office 2007 visual style.");
            if (this.Style == ColorPickerUIAdv.visualstyle.Office2010)
                this.AddDesignerActionPropertyItem("Office2010ColorScheme", "Color Scheme", "Appearance", "Color Scheme for Office 2010 visual style.");
            this.AddDesignerActionPropertyItem("Horizontal", "Horizontal Spacing", "Appearance", "The horizontal spacing between items in the control.");
            this.AddDesignerActionPropertyItem("Vertical", "Vertical Spacing", "Appearance", "The vertical spacing between items in the control.");
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
                    ColorPickerUIAdv control = this.Control as ColorPickerUIAdv;
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
        public ColorPickerUIAdv.visualstyle Style
        {
            get
            {
                ColorPickerUIAdv.visualstyle style = ColorPickerUIAdv.visualstyle.Default;
                if (this.Control != null)
                {
                    ColorPickerUIAdv control = this.Control as ColorPickerUIAdv;
                    style = control.Style;
                }

                return style;
            }
            set
            {
                SetValue("Style", value);
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
                    ColorPickerUIAdv control = this.Control as ColorPickerUIAdv;
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
        /// Gets or sets the Office2007 color scheme.
        /// </summary>
        /// <value>The Office2007 color scheme.</value>
        public Office2007Theme Office2007ColorScheme
        {
            get
            {
                Office2007Theme officeTheme = Office2007Theme.Blue;
                if (this.Control != null)
                {
                    ColorPickerUIAdv control = this.Control as ColorPickerUIAdv;
                    officeTheme = control.Office2007Theme;
                }

                return officeTheme;
            }
            set
            {
                SetValue("Office2007Theme", value);
            }
        }

        /// <summary>
        /// Gets or sets the Office2010 color scheme.
        /// </summary>
        /// <value>The Office2010 color scheme.</value>
        public Office2010Theme Office2010ColorScheme
        {
            get
            {
                Office2010Theme officeTheme = Office2010Theme.Blue;
                if (this.Control != null)
                {
                    ColorPickerUIAdv control = this.Control as ColorPickerUIAdv;
                    officeTheme = control.Office2010Theme;
                }

                return officeTheme;
            }
            set
            {
                SetValue("Office2010Theme", value);
            }
        }
        /// <summary>
        /// Gets or sets the HorizontalItemsSpacing.
        /// </summary>
        /// <value>The style.</value>
        public int Horizontal
        {
            get
            {
                int Horizontal = 4;
                if (this.Control != null)
                {
                    ColorPickerUIAdv control = this.Control as ColorPickerUIAdv;
                    Horizontal = control.HorizontalItemsSpacing;
                }

                return Horizontal;
            }
            set
            {
                SetValue("HorizontalItemsSpacing", value);
            }
        }
        /// <summary>
        /// Gets or sets the VerticalItemsSpacing.
        /// </summary>
        /// <value>The style.</value>
        public int Vertical
        {
            get
            {
                int Vertical = 0;
                if (this.Control != null)
                {
                    ColorPickerUIAdv control = this.Control as ColorPickerUIAdv;
                    Vertical = control.VerticalItemsSpacing;
                }

                return Vertical;
            }
            set
            {
                SetValue("VerticalItemsSpacing", value);
            }
        }
    }
}
#endif