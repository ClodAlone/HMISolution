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
    public class ComboDropDownActionList : SyncActionListBase<ComboDropDown>
    {
        /// <summary>
        /// Initializes a new instance of the CheckBoxAdvActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public ComboDropDownActionList(IComponent component)
            : base(component)
        {
        }
        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - ComboDropDown");

            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");
            this.AddDesignerActionPropertyItem("Text", "Text", "Appearance", "The text associated with the control.");
            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("BorderStyle", "BorderStyle", "Appearance", "Determines the borderstyleof the control");
            this.AddDesignerActionPropertyItem("DropDownStyle", "DropDown Style", "Appearance", "The dropdown style with the control.");
            this.AddDesignerActionPropertyItem("Style", "Visual Style", "Appearance", "Visual Style for the control.");
            
            if(this.Style == VisualStyle.Office2007)
                this.AddDesignerActionPropertyItem("Office2007ColorScheme", "Color Scheme", "Appearance", "Color Scheme for Office 2007 visual style.");
            if (this.Style == VisualStyle.Office2010)
                this.AddDesignerActionPropertyItem("Office2010ColorScheme", "Color Scheme", "Appearance", "Color Scheme for Office 2010 visual style.");
            // Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("Readonly", "ReadOnly", "Behavior", "Gets or sets the ReadOnly state of the CheckBox.");
            this.AddDesignerActionPropertyItem("GrayonReadonly", "GrayOnReadOnly", "Behavior", "Indicates whether text should be grayedout on readonly.");
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
                    ComboDropDown control = this.Control as ComboDropDown;
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
        /// Gets or sets Readonly.
        /// </summary>
        public bool Readonly
        {
            get
            {
                bool read_only =false;
                if (this.Control != null)
                {
                    ComboDropDown control = this.Control as ComboDropDown;
                    read_only = control.ReadOnly;
                }

                return read_only;
            }

            set
            {
                SetValue("ReadOnly", value);
            }
        }
        /// <summary>
        /// Gets or sets Readonly.
        /// </summary>
        public bool GrayonReadonly
        {
            get
            {
                bool grayread_only = false;
                if (this.Control != null)
                {
                    ComboDropDown control = this.Control as ComboDropDown;
                    grayread_only = control.GrayOnReadOnly;
                }

                return grayread_only;
            }

            set
            {
                SetValue("GrayOnReadOnly", value);
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
                    ComboDropDown control = this.Control as ComboDropDown;
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
        public VisualStyle Style
        {
            get
            {
                VisualStyle style = VisualStyle.Default;
                if(this.Control != null)
                {
                    ComboDropDown control = this.Control as ComboDropDown;
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
                    ComboDropDown control = this.Control as ComboDropDown;
                    officeTheme = control.Office2007ColorTheme;
                }

                return officeTheme;
            }
            set
            {
                SetValue("Office2007ColorTheme", value);
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
                    ComboDropDown control = this.Control as ComboDropDown;
                    officeTheme = control.Office2010ColorTheme;
                }

                return officeTheme;
            }
            set
            {
                SetValue("Office2010ColorTheme", value);
            }
        }
        /// <summary>
        /// Gets or sets the Border style.
        /// </summary>
        /// <value>The style.</value>
        public Border3DStyle BorderStyle
        {
            get
            {
                Border3DStyle bstyle = Border3DStyle.Sunken;
                if (this.Control != null)
                {
                    ComboDropDown control = this.Control as ComboDropDown;
                    bstyle = control.Border3DStyle;
                }

                return bstyle;
            }
            set
            {
                SetValue("Border3DStyle", value);
            }
        }
        /// <summary>
        /// Gets or sets the Dropdown style.
        /// </summary>
        /// <value>The style.</value>
        public ComboBoxStyle DropDownStyle
        {
            get
            {
                ComboBoxStyle dstyle = ComboBoxStyle.Simple;
                if (this.Control != null)
                {
                    ComboDropDown control = this.Control as ComboDropDown;
                    dstyle = control.DropDownStyle;
                }

                return dstyle;
            }
            set
            {
                SetValue("DropDownStyle", value);
            }
        }
    }
}
#endif
