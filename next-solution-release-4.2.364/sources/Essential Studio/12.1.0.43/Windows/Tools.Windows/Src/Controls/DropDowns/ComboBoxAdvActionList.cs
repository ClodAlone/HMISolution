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
    public class ComboBoxAdvActionList : SyncActionListBase<ComboBoxAdv>
    {
        public ComboBoxAdvActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - ComboBoxAdv");
            this.AddDesignerActionPropertyItem("Name", "Name", "Appearance", "Specifies the name for the control.");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("Border3DStyle", "Border3D Style", "Appearance", "Indicates the style of the 3D Border.");
            this.AddDesignerActionPropertyItem("DropDownStyle", "DropDown Style", "Appearance", "Indicates the style of the comboBox.");
            this.AddDesignerActionPropertyItem("Style", "Visual Style", "Appearance", "Specifies advanced appearance and behavior.");

            // Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            this.AddDesignerActionPropertyItem("DropDownWidth", "DropDown Width", "Behavior", "Specifies the width of the dropdown.");
            this.AddDesignerActionPropertyItem("MaxDropDownItems", "MaxDropDownItems", "Behavior", "Specfies the maximum number of items to be displayed in the combobox dropdown.");
            this.AddDesignerActionPropertyItem("ReadOnly", "ReadOnly", "Behavior", "Specifies whether the text in the edit portion can be changed or not.");
            this.AddDesignerActionPropertyItem("Sorted", "Sort Items", "Behavior", "Specifies whether items in the dropdown should be sorted.");

            // Misc Category
            this.AddDesignerActionHeaderItem("Images");
            this.AddDesignerActionPropertyItem("ShowImageInTextBox", "ShowImageInTextBox", "Misc", "Specifies whether selected image should be shown in the textbox.");
            this.AddDesignerActionPropertyItem("ImageList", "ImageList", "Misc", "Specfies the imagelist for the combobox.");
        }

        public string Name
        {
            get
            {
                string name = String.Empty;
                if (this.Control != null)
                {
                    ComboBoxAdv control = this.Control as ComboBoxAdv;
                    name = control.Name;
                }
                return name;
            }
            set
            {
                SetValue("Name", value);
            }
        }

        public Border3DStyle Border3DStyle
        {
            get
            {
                Border3DStyle border3DStyle = Border3DStyle.Sunken;
                if (this.Control != null)
                {
                    ComboBoxAdv control = this.Control as ComboBoxAdv;
                    border3DStyle = control.Border3DStyle;
                }
                return border3DStyle;
            }
            set
            {
                SetValue("Border3DStyle", value);
            }
        }

        public ComboBoxStyle DropDownStyle
        {
            get
            {
                ComboBoxStyle dropDownStyle = ComboBoxStyle.DropDown;
                if (this.Control != null)
                {
                    ComboBoxAdv control = this.Control as ComboBoxAdv;
                    dropDownStyle = control.DropDownStyle;
                }
                return dropDownStyle;
            }
            set
            {
                SetValue("DropDownStyle", value);
            }
        }

        public VisualStyle Style
        {
            get
            {
                VisualStyle visualStyle = VisualStyle.Default;
                if (this.Control != null)
                {
                    ComboBoxAdv control = this.Control as ComboBoxAdv;
                    visualStyle = control.Style;
                }
                return visualStyle;
            }
            set
            {
                SetValue("Style", value);
            }
        }

        public int DropDownWidth
        {
            get
            {
                int dropDownWidth = this.Control.Width;
                if (this.Control != null)
                {
                    ComboBoxAdv control = this.Control as ComboBoxAdv;
                    dropDownWidth = control.Width;
                }
                return dropDownWidth;
            }
            set
            {
                SetValue("DropDownWidth", value);
            }
        }

        public int MaxDropDownItems
        {
            get
            {
                int maxDropDownItems = 8;
                if (this.Control != null)
                {
                    ComboBoxAdv control = this.Control as ComboBoxAdv;
                    maxDropDownItems = control.MaxDropDownItems;
                }
                return maxDropDownItems;
            }
            set
            {
                SetValue("MaxDropDownItems", value);
            }
        }

        public bool ReadOnly
        {
            get
            {
                bool readOnly = false;
                if (this.Control != null)
                {
                    ComboBoxAdv control = this.Control as ComboBoxAdv;
                    readOnly = control.ReadOnly;
                }
                return readOnly;
            }
            set
            {
                SetValue("ReadOnly", value);
            }
        }

        public bool Sorted
        {
            get
            {
                bool sorted = false;
                if (this.Control != null)
                {
                    ComboBoxAdv control = this.Control as ComboBoxAdv;
                    sorted = control.Sorted;
                }
                return sorted;
            }
            set
            {
                SetValue("Sorted", value);
            }
        }

        public bool ShowImageInTextBox
        {
            get
            {
                bool showImageInTextBox = false;
                if (this.Control != null)
                {
                    ComboBoxAdv control = this.Control as ComboBoxAdv;
                    showImageInTextBox = control.ShowImageInTextBox;
                }
                return showImageInTextBox;
            }
            set
            {
                SetValue("ShowImageInTextBox", value);
            }
        }

        public ImageList ImageList
        {
            get
            {
                ImageList imageList = null;
                if (this.Control != null)
                {
                    ComboBoxAdv control = this.Control as ComboBoxAdv;
                    imageList = control.ImageList;
                }
                return imageList;
            }
            set
            {
                SetValue("ImageList", value);
            }
        }
    }
}
#endif