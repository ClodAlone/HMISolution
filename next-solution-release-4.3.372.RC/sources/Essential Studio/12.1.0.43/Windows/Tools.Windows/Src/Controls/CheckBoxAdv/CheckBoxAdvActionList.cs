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
    public class CheckBoxAdvActionList : SyncActionListBase<CheckBoxAdv>
    {
        /// <summary>
        /// Initializes a new instance of the CheckBoxAdvActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public CheckBoxAdvActionList(IComponent component)
            : base(component)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether Image check box is used.
        /// </summary>
        public bool ImageCheckBox
        {
            get
            {
                bool imageCheckBox = false;
                if (this.Control != null)
                {
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
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
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
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
        /// Gets or sets a value indicating whether Tristate of the  CheckBoxAdv is true or false.
        /// </summary>
        public bool Tristate
        {
            get
            {
                bool triState = false;
                if (this.Control != null)
                {
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
                    triState = control.Tristate;
                }

                return triState;
            }

            set
            {
                SetValue("Tristate", value);
            }
        }

        /// <summary>
        /// Gets or sets  CheckedImage.
        /// </summary>
        public Image CheckedImage
        {
            get
            {
                Image bcheckedImage = null;
                if (this.Control != null)
                {
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
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
        /// Gets or sets UncheckedImage.
        /// </summary>
        public Image UncheckedImage
        {
            get
            {
                Image buncheckedImage = null;
                if (this.Control != null)
                {
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
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
        /// Gets or sets IndeterminateImage.
        /// </summary>
        public Image IndeterminateImage
        {
            get
            {
                Image bindeterminateImage = null;
                if (this.Control != null)
                {
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
                    bindeterminateImage = control.IndeterminateImage;
                }

                return bindeterminateImage;
            }

            set
            {
                SetValue("IndeterminateImage", value);
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
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
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
        /// Gets or sets the Text
        /// </summary>
        public string Text
        {
            get
            {
                string text = " ";
                if (this.Control != null)
                {
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
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
        public CheckBoxAdvStyle Style
        {
            get
            {
                CheckBoxAdvStyle style = CheckBoxAdvStyle.Default;
                if(this.Control != null)
                {
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
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
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
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
                    CheckBoxAdv control = this.Control as CheckBoxAdv;
                    officeTheme = control.Office2010ColorScheme;
                }

                return officeTheme;
            }
            set
            {
                SetValue("Office2010ColorScheme", value);
            }
        }
        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - Advanced CheckBox");

            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("ImageCheckBox", "Image CheckBox", "Appearance", "Determines if the checkbox will be drawn using the images provided.");
            this.AddDesignerActionPropertyItem("Text", "Text", "Appearance", "The text associated with the control.");
            this.AddDesignerActionPropertyItem("Style", "Visual Style", "Appearance", "Visual Style for the checkbox.");
            
            if(this.Style == CheckBoxAdvStyle.Office2007)
                this.AddDesignerActionPropertyItem("Office2007ColorScheme", "Color Scheme", "Appearance", "Color Scheme for Office 2007 visual style.");
            if (this.Style == CheckBoxAdvStyle.Office2010)
                this.AddDesignerActionPropertyItem("Office2010ColorScheme", "Color Scheme", "Appearance", "Color Scheme for Office 2010 visual style.");
            // Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("Checked", "Checked", "Behavior", "Gets or sets the checked state of the CheckBox.");
            this.AddDesignerActionPropertyItem("Tristate", "Tristate", "Behavior", "Indicates whether undetermined state can be accessed through clicking.");

            if (this.ImageCheckBox)
            {
                // StateImages Category
                this.AddDesignerActionHeaderItem("StateImages");
                this.AddDesignerActionPropertyItem("CheckedImage", "Checked Image", "StateImages", "Gets or sets the image used to draw the checkbox when checked and mouse not over.");
                this.AddDesignerActionPropertyItem("UncheckedImage", "Unchecked Image", "StateImages", "Gets or sets the image used to draw the checkbox when Unchecked and mouse not over.");
                this.AddDesignerActionPropertyItem("IndeterminateImage", "Indeterminate Image", "StateImages", "Gets or sets the image used to draw the checkbox when Indeterminate and mouse not over.");
            }
        }
    }
}
#endif
