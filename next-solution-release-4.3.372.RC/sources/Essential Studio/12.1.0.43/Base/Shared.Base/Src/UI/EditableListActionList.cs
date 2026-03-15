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
    public class EditableListActionList : SyncActionListBase<EditableList>
    {
        /// <summary>
        /// Initializes a new instance of the CheckBoxAdvActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public EditableListActionList(IComponent component)
            : base(component)
        {
        }
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - EditableList");

            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");
            this.AddDesignerActionPropertyItem("Text", "Text", "Design", "The text associated with the control.");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("Borderstyle", "Border Style", "Appearance", "Gets or Sets the border style.");
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("Autoscroll", "AutoScroll", "Behavior", "Sets the autoscroll property.");
            this.AddDesignerActionPropertyItem("Listbox", "ListBox Text Alignment", "Behavior", "Sets the Listbox text alignment property.");
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
                    EditableList control = this.Control as EditableList;
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
                    EditableList control = this.Control as EditableList;
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
        /// Gets or sets the border style.
        /// </summary>
        /// <value>The style.</value>
        public BorderStyle Borderstyle
        {
            get
            {
                BorderStyle bstyle = BorderStyle.None;
                if (this.Control != null)
                {
                    EditableList control = this.Control as EditableList;
                    bstyle = control.BorderStyle;
                }

                return bstyle;
            }
            set
            {
                SetValue("BorderStyle", value);
            }
        }

        /// <summary>
        /// Gets or sets the auoscroll.
        /// </summary>
        /// <value>The style.</value>
        public bool Autoscroll
        {
            get
            {
                bool autoscroll = false;
                if (this.Control != null)
                {
                    EditableList control = this.Control as EditableList;
                    autoscroll = control.AutoScroll;
                }

                return autoscroll;
            }
            set
            {
                SetValue("AutoScroll", value);
            }
        }
        /// <summary>
        /// Gets or sets listbox text alignment
        /// </summary>
        /// <value>The style.</value>
        public TextAlignment Listbox
        {
            get
            {
                TextAlignment listbox = TextAlignment.Left;
                if (this.Control != null)
                {
                    EditableList control = this.Control as EditableList;
                    listbox = control.ListBoxTextAlignment;
                }

                return listbox;
            }
            set
            {
                SetValue("ListBoxTextAlignment", value);
            }
        }
    }
}
#endif