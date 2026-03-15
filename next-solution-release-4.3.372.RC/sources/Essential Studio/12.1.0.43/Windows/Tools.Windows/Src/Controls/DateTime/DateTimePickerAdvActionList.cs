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
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    public class DateTimePickerAdvActionList : SyncActionListBase<DateTimePickerAdv>
    {
        public DateTimePickerAdvActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - DateTimePickerAdv");

            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the Name used in code to identify the Object.");

            this.AddDesignerActionPropertyItem("Culture", "Culture", "Appearance", "Indicates the Current culture of the DateTimePickerAdv.");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("ShowCheckBox", "Show CheckBox", "Appearance", "Indicates if the CheckBox will be visible.");
            this.AddDesignerActionPropertyItem("ShowDropButton", "Show Drop Down Button", "Appearance", "Indicates if the Drop down button is visible.");
            this.AddDesignerActionPropertyItem("ShowUpDown", "Show UpDown Button", "Appearance", "Indicates if the Updown Buttons are visible.");
            this.AddDesignerActionPropertyItem("ShowUpDownOnFocus", "Show UpDown Button on Focus", "Appearance", "Indicates if the Updown will be visible when got a focus.");
            this.AddDesignerActionPropertyItem("Style", "Visual Style", "Appearance", "Indicates Office Style of the DateTimePickerAdv.");            

            // Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("Checked", "Checked State", "Behavior", "Indicates the Checked state of the DateTimePickerAdv.");
            this.AddDesignerActionPropertyItem("IsNullDate", "Is Null Date", "Behavior", "Indicates if there is no date selected.");
        }

        public string Name
        {
            get
            {
                string name = " ";
                if (this.Control != null)
                {
                    DateTimePickerAdv control = this.Control as DateTimePickerAdv;
                    name = control.Name;
                }
                return name;
            }
            set
            {
                SetValue("Name", value);
            }
        }

        [TypeConverter(typeof(SpecificCultureInfoTypeConverter))]
        public CultureInfo Culture
        {
            get
            {
                CultureInfo bCulture = CultureInfo.CurrentCulture;
                if (this.Control != null)
                {
                    DateTimePickerAdv control = this.Control as DateTimePickerAdv;
                    bCulture = control.Culture;
                }
                return bCulture;
            }
            set
            {
                SetValue("Culture", value);
            }
        }               

        public bool ShowCheckBox
        {
            get
            {
                bool showCheckBox = false;
                if (this.Control != null)
                {
                    DateTimePickerAdv control = this.Control as DateTimePickerAdv;
                    showCheckBox = control.ShowCheckBox;
                }
                return showCheckBox;
            }
            set
            {
                SetValue("ShowCheckBox", value);
            }
        }

        public bool ShowDropButton
        {
            get
            {
                bool showDropButton = false;
                if (this.Control != null)
                {
                    DateTimePickerAdv control = this.Control as DateTimePickerAdv;
                    showDropButton = control.ShowDropButton;
                }
                return showDropButton;
            }
            set
            {
                SetValue("ShowDropButton", value);
            }
        }

        public bool ShowUpDown
        {
            get
            {
                bool showUpDown = false;
                if (this.Control != null)
                {
                    DateTimePickerAdv control = this.Control as DateTimePickerAdv;
                    showUpDown = control.ShowUpDown;
                }
                return showUpDown;
            }
            set
            {
                SetValue("ShowUpDown", value);
            }
        }

        public bool ShowUpDownOnFocus
        {
            get
            {
                bool showUpDownOnFocus = false;
                if (this.Control != null)
                {
                    DateTimePickerAdv control = this.Control as DateTimePickerAdv;
                    showUpDownOnFocus = control.ShowUpDownOnFocus;
                }
                return showUpDownOnFocus;
            }
            set
            {
                SetValue("ShowUpDownOnFocus", value);
            }
        }

        public Syncfusion.Windows.Forms.VisualStyle Style
        {
            get
            {
                Syncfusion.Windows.Forms.VisualStyle style = Syncfusion.Windows.Forms.VisualStyle.Default;
                if (this.Control != null)
                {
                    DateTimePickerAdv control = this.Control as DateTimePickerAdv;
                    style = control.Style;
                }
                return style;
            }
            set
            {
                SetValue("Style", value);
            }
        }

        public bool Checked
        {
            get
            {
                bool bChecked = false;
                if (this.Control != null)
                {
                    DateTimePickerAdv control = this.Control as DateTimePickerAdv;
                    bChecked = control.Checked;
                }
                return bChecked;
            }
            set
            {
                SetValue("Checked", value);
            }
        }

        public bool IsNullDate
        {
            get
            {
                bool isNullDate = false;
                if (this.Control != null)
                {
                    DateTimePickerAdv control = this.Control as DateTimePickerAdv;
                    isNullDate = control.IsNullDate;
                }
                return isNullDate;
            }
            set
            {
                SetValue("IsNullDate", value);
            }
        }               
    }
}
#endif
