#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Text;
using System.Reflection;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;
using System.Globalization;

namespace Syncfusion.Windows.Forms.Tools
{
    public class ButtonEditActionList : SyncActionListBase<ButtonEdit>
    {

        public ButtonEditActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - ButtonEdit");

            //Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");

            //Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("ShowTextBox", "Show TextBox", "Appearance", "Indicates whether the embedded TextBox is visible in the ButtonEdit.");
            this.AddDesignerActionPropertyItem("UseVisualStyle", "UseVisualStyle", "Appearance", "Indicates whether to use visual styles.");
            
            if(this.UseVisualStyle)
                this.AddDesignerActionPropertyItem("ButtonStyle", "Button Style", "Appearance", "Gets or sets the button style for the control.");

            //Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("Buttons", "Buttons Collection", "Behavior", "Gets or sets the collection of Buttons that makes up this ButtonEdit control.");
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show text box.
        /// </summary>
        /// <value><c>true</c> if show text box; otherwise, <c>false</c>.</value>
        public bool ShowTextBox
        {
            get
            {
                bool showTextBox = true;
                if (this.Control != null)
                {
                    ButtonEdit control = this.Control as ButtonEdit;
                    showTextBox = control.ShowTextBox;
                }
                return showTextBox;
            }
            set
            {
                SetValue("ShowTextBox", value);
            }
        }

        /// <summary>
        /// Gets or sets the button style.
        /// </summary>
        /// <value>The button style.</value>
        public ButtonAppearance ButtonStyle
        {
            get
            {
                ButtonAppearance appearance = ButtonAppearance.WindowsXP;
                if (this.Control != null)
                {
                    ButtonEdit control = this.Control as ButtonEdit;
                    appearance = control.ButtonStyle;
                }
                return appearance;
            }
            set
            {
                SetValue( "ButtonStyle", value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use visual style.
        /// </summary>
        /// <value><c>true</c> if use visual style; otherwise, <c>false</c>.</value>
        public bool UseVisualStyle
        {
            get
            {
                bool useVisualStyle = false;
                if (this.Control != null)
                {
                    ButtonEdit control = this.Control as ButtonEdit;
                    useVisualStyle = control.UseVisualStyle;
                }
                return useVisualStyle;
            }
            set
            {
                SetValue("UseVisualStyle", value);
            }
        }

        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>The buttons.</value>
        public Syncfusion.Windows.Forms.Tools.ButtonEdit.ButtonEditChildButtonCollection Buttons
        {
            get
            {
                Syncfusion.Windows.Forms.Tools.ButtonEdit.ButtonEditChildButtonCollection buttons = null;
                if (this.Control != null)
                {
                    ButtonEdit control = this.Control as ButtonEdit;
                    buttons = control.Buttons;
                }
                return buttons;
            }
            set
            {
                SetValue("Buttons", value);
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                string name = string.Empty;
                if (this.Control != null)
                {
                    ButtonEdit control = this.Control as ButtonEdit;
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
