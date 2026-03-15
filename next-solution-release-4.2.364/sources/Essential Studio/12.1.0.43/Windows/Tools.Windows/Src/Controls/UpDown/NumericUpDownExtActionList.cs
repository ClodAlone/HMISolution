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
    public class NumericUpDownExtActionList : SyncActionListBase<NumericUpDownExt>
    {
        public NumericUpDownExtActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - NumericUpDownExt");

            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the Name used in code to identify the Object.");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("Visualstyle", "Visual Style", "Appearance", "Gets or Sets the Visual Style should be used for this control when available.");
            this.AddDesignerActionPropertyItem("ThemedBorder", "Themed Border", "Appearance", "Specifies whether or not you want themed border around the control when themes are enabled.");
            this.AddDesignerActionPropertyItem("Hexadecimal", "HexaDecimal", "Appearance", " Gets or sets a value indicating whether the Numeric UpDown should display the value it contains in hexadecimal format.");
            this.AddDesignerActionPropertyItem("Value", "Value", "Appearance", "The current value of the Numeric UpDown control.");
            this.AddDesignerActionPropertyItem("TextAlign", "Text Alignment", "Appearance", "Gets or sets the alignment of the text.");
            this.AddDesignerActionPropertyItem("UpDownAlign", "UpDown Align", "Appearance", "Gets or sets the alignment of the up and down buttons on the control.");

            // Data Category
            this.AddDesignerActionHeaderItem("Data");
            this.AddDesignerActionPropertyItem("ThousandsSeparator", "Thousands Separator", "Data", "  Gets or sets a value indicating whether a thousands separator is displayed in the Numeric UpDown.");
            this.AddDesignerActionPropertyItem("DecimalPlaces", "Decimal Places", "Data", " Gets or sets the number of decimal places to display in the Numeric UpDown. ");
            this.AddDesignerActionPropertyItem("Increment", "Increment", "Data", " Gets or sets the value to increment or decrement the Numeric UpDown.");
            this.AddDesignerActionPropertyItem("Maximum", "Maximum", "Data", "  Gets or sets the maximum value for the Numeric UpDown");
            this.AddDesignerActionPropertyItem("Minimum", "Minimum", "Data", " Gets or sets the minimum allowed value for the Numeric UpDown.");
        }

        public string Name
        {
           get
            {
                string name = " ";
                if (this.Control != null)
                {
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
                    name = control.Name;
                }
                return name;
            }
            set
            {
                SetValue("Name", value);
            }
        }

        public decimal Value
        {
            get
            {
                decimal text = 0;
                if (this.Control != null)
                {
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
                    text = control.Value;
                }
                return text;
            }
            set
            {
                SetValue("Value", value);
            }
        }
        public HorizontalAlignment TextAlign
        {
            get
            {
                HorizontalAlignment textAlignment = HorizontalAlignment.Left;
                if (this.Control != null)
                {
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
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
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
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
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
                    upDownAlign = control.UpDownAlign;
                }
                return upDownAlign;
            }
            set
            {
                SetValue("UpDownAlign", value);
            }
        }
        public VisualStyle Visualstyle
        {
            get
            {
                VisualStyle visualstyle = VisualStyle.Default;
                if (this.Control != null)
                {
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
                    visualstyle = control.VisualStyle;
                }
                return visualstyle;
            }
            set
            {
                SetValue("VisualStyle", value);
            }
        }
        public bool Hexadecimal
        {
            get
            {
                bool hexadecimal = true;
                if (this.Control != null)
                {
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
                    hexadecimal = control.Hexadecimal;
                }
                return hexadecimal;
            }
            set
            {
                SetValue("Hexadecimal", value);
            }
        }

        public int DecimalPlaces
        {
            get
            {
                int decimalPlaces = 0;
                if (this.Control != null)
                {
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
                    decimalPlaces = control.DecimalPlaces;
                }
                return decimalPlaces;
            }
            set
            {
                SetValue("DecimalPlaces", value);
            }
        }
        public decimal Increment
        {
            get
            {
                decimal increment = 1;
                if (this.Control != null)
                {
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
                    increment = control.Increment;
                }
                return increment;
            }
            set
            {
                SetValue("Increment", value);
            }
        }
        public decimal Maximum
        {
            get
            {
                decimal maximum = 100;
                if (this.Control != null)
                {
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
                    maximum = control.Maximum;
                }
                return maximum;
            }
            set
            {
                SetValue("Maximum", value);
            }
        }
        public decimal Minimum
        {
            get
            {
                decimal minimum = 0;
                if (this.Control != null)
                {
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
                    minimum = control.Minimum;
                }
                return minimum;
            }
            set
            {
                SetValue("Minimum", value);
            }
        }
        public bool ThousandsSeparator
        {
            get
            {
                bool thousandsSeparator = true;
                if (this.Control != null)
                {
                    NumericUpDownExt control = this.Control as NumericUpDownExt;
                    thousandsSeparator = control.ThousandsSeparator;
                }
                return thousandsSeparator;
            }
            set
            {
                SetValue("ThousandsSeparator", value);
            }
        }
    }
}
#endif
