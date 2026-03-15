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
    public class RadioButtonAdvActionList : SyncActionListBase<RadioButtonAdv>
    {
        /// <summary>
        /// Initializes a new instance of the RadioButtonAdvActionList class.
        /// </summary>
        /// <param name="component">Represents component</param>
        public RadioButtonAdvActionList(IComponent component)
            : base(component)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether ImageCheckBox.
        /// </summary>
        public bool ImageCheckBox
        {
            get
            {
                bool imageCheckBox = false;
                if (this.Control != null)
                {
                    RadioButtonAdv control = this.Control as RadioButtonAdv;
                    imageCheckBox = control.ImageCheckBox;
                }

                return imageCheckBox;
            }

            set
            {
                SetValue("ImageCheckBox", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether checked property is true or false.
        /// </summary>
        public bool Checked
        {
            get
            {
                bool bchecked = true;
                if (this.Control != null)
                {
                    RadioButtonAdv control = this.Control as RadioButtonAdv;
                    bchecked = control.Checked;
                }

                return bchecked;
            }

            set
            {
                SetValue("Checked", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating checked Image.
        /// </summary>
        public Image CheckedImage
        {
            get
            {
                Image bcheckedImage = null;
                if (this.Control != null)
                {
                    RadioButtonAdv control = this.Control as RadioButtonAdv;
                    bcheckedImage = control.CheckedImage;
                }

                return bcheckedImage;
            }

            set
            {
                SetValue("CheckedImage", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicatingUncheckedImage
        /// </summary>
        public Image UncheckedImage
        {
            get
            {
                Image buncheckedImage = null;
                if (this.Control != null)
                {
                    RadioButtonAdv control = this.Control as RadioButtonAdv;
                    buncheckedImage = control.UncheckedImage;
                }

                return buncheckedImage;
            }

            set
            {
                SetValue("UncheckedImage", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating Name
        /// </summary>
        public string Name
        {
            get
            {
                string name = " ";
                if (this.Control != null)
                {
                    RadioButtonAdv control = this.Control as RadioButtonAdv;
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
        /// Gets or sets a value indicating Text.
        /// </summary>
        public string Text
        {
            get
            {
                string text = " ";
                if (this.Control != null)
                {
                    RadioButtonAdv control = this.Control as RadioButtonAdv;
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
        /// Gets or sets a value indicating whether ThemesEnabled property is true or false.
        /// </summary>
        public bool ThemesEnabled
        {
            get
            {
                bool themesEnabled = true;
                if (this.Control != null)
                {
                    RadioButtonAdv control = this.Control as RadioButtonAdv;
                    themesEnabled = control.ThemesEnabled;
                }

                return themesEnabled;
            }

            set
            {
                SetValue("ThemesEnabled", value);
            }
        }

        public RadioButtonAdvStyle Style
        {
            get
            {
                RadioButtonAdvStyle style = RadioButtonAdvStyle.Default;
                if (this.Control != null)
                {
                    RadioButtonAdv control = this.Control as RadioButtonAdv;
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
                    RadioButtonAdv control = this.Control as RadioButtonAdv;
                    officeTheme = control.Office2007ColorScheme;
                }

                return officeTheme;
            }
            set
            {
                SetValue("Office2007ColorScheme", value);
            }
        }

        /// <summary>
        /// InitializeActionList method
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - Advanced RadioButton");

            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("ImageCheckBox", "Image CheckBox", "Appearance", "Determines if the checkbox will be drawn using the images provided.");
            this.AddDesignerActionPropertyItem("ThemesEnabled", "Themes Enabled", "Appearance", "Indicates whether themes are enabled for this control.");
            this.AddDesignerActionPropertyItem("Text", "Text", "Appearance", "The text associated with the control.");
            this.AddDesignerActionPropertyItem("Style", "Visual Style", "Appearance", "Visual Style for the Radio button.");

            if (this.Style == RadioButtonAdvStyle.Office2007)
                this.AddDesignerActionPropertyItem("Office2007ColorScheme", "Color Scheme", "Appearance", "Color Scheme for Office 2007 visual style.");
            // Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("Checked", "Checked", "Behavior", "Gets or sets the checked state of the radio button.");

            if (this.ImageCheckBox)
            {
                // StateImages Category
                this.AddDesignerActionHeaderItem("StateImages");
                this.AddDesignerActionPropertyItem("CheckedImage", "Checked Image", "StateImages", "Gets or sets the image used to draw the radio button when checked and mouse not over.");
                this.AddDesignerActionPropertyItem("UncheckedImage", "Unchecked Image", "StateImages", "Gets or sets the image used to draw the radio button when unchecked and mouse not over.");
            }
        }
    }
}
#endif
