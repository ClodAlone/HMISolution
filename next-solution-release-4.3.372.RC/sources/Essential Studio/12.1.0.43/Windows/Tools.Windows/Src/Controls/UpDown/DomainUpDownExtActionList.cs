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
    public class DomainUpDownExtActionList : SyncActionListBase<DomainUpDownExt>
    {
        public DomainUpDownExtActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - DomainUpDownExt");

            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the Name used in code to identify the Object.");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("VisualStyle", "Visual Style", "Appearance", "Sets the visual style of the control.");
            this.AddDesignerActionPropertyItem("ThemedBorder", "Themed Border", "Appearance", "Specifies whether or not you want themed border around the control when themes are enabled.");
            this.AddDesignerActionPropertyItem("Text", "Text", "Appearance", "The text associated with the control.");
            this.AddDesignerActionPropertyItem("TextAlign", "Text Alignment", "Appearance", "Gets or sets the alignment of the text.");
            this.AddDesignerActionPropertyItem("UpDownAlign", "UpDown Align", "Appearance", "Gets or sets the alignment of the up and down buttons on the control.");

            // Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("MaxLength", "MaxLength", "Behavior", "Gets or Sets the maximum length of the text that can be entered into the editable portion of the control.");
        }

        public string Name
        {
            get
            {
                string name = " ";
                if (this.Control != null)
                {
                    DomainUpDownExt control = this.Control as DomainUpDownExt;
                    name = control.Name;
                }
                return name;
            }
            set
            {
                SetValue("Name", value);
            }
        }
        public string Text
        {
            get
            {
                string text = " ";
                if (this.Control != null)
                {
                    DomainUpDownExt control = this.Control as DomainUpDownExt;
                    text = control.Text;
                }
                return text;
            }
            set
            {
                SetValue("Text", value);
            }
        }
        public HorizontalAlignment TextAlign
        {
            get
            {
                HorizontalAlignment textAlignment = HorizontalAlignment.Left;
                if (this.Control != null)
                {
                    DomainUpDownExt control = this.Control as DomainUpDownExt;
                    textAlignment = control.TextAlign;
                }
                return textAlignment;
            }
            set
            {
                SetValue("TextAlign", value);
            }
        }
        public bool ThemedBorder
        {
            get
            {
                bool themedBorder = true;
                if (this.Control != null)
                {
                    DomainUpDownExt control = this.Control as DomainUpDownExt;
                    themedBorder = control.ThemedBorder;
                }
                return themedBorder;
            }
            set
            {
                SetValue("ThemedBorder", value);
            }
        }
        public LeftRightAlignment UpDownAlign
        {
            get
            {
                LeftRightAlignment upDownAlign = LeftRightAlignment.Right;
                if (this.Control != null)
                {
                    DomainUpDownExt control = this.Control as DomainUpDownExt;
                    upDownAlign = control.UpDownAlign;
                }
                return upDownAlign;
            }
            set
            {
                SetValue("UpDownAlign", value);
            }
        }
        public int MaxLength
        {
            get
            {
                int maxLength = 32767;
                if (this.Control != null)
                {
                    DomainUpDownExt control = this.Control as DomainUpDownExt;
                    maxLength = control.MaxLength;
                }
                return maxLength;
            }
            set
            {
                SetValue("MaxLength", value);
            }
        }
        public VisualStyle VisualStyle
        {
            get
            {
                VisualStyle vstyle = VisualStyle.Default;
                if (this.Control != null)
                {
                    DomainUpDownExt control = this.Control as DomainUpDownExt;
                    vstyle = control.VisualStyle;
                }
                return vstyle;
            }
            set
            {
                SetValue("VisualStyle", value);
            }
        }
    }
}
#endif
