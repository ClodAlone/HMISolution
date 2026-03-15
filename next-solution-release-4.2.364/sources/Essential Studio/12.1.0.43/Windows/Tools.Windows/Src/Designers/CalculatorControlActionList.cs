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
    public class CalculatorControlActionList : SyncActionListBase<CalculatorControl>
    {

        public CalculatorControlActionList(IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools for Windows Forms 2.0");


            this.AddDesignerActionPropertyItem("Culture", "Culture", "Appearance", "Specifies the Culture to be used for Calculator control.");


            this.AddDesignerActionPropertyItem("DisplayTextAlign", "Text Alignment", "Appearance", "Horizontal text alignment for the control.");

            this.AddDesignerActionPropertyItem("LayoutType", "Layout Type", "Appearance", "Specifies the Calculator Layout types.");



            this.AddDesignerActionPropertyItem("ShowDisplayArea", "Show/Hide Display Area", "Appearance", "Shows/Hides the text display area in the calculator.");

            this.AddDesignerActionPropertyItem("ThemesEnabled", "Themes Enabled", "Appearance", "Gets/Sets Themes Enabled property.");


            this.AddDesignerActionPropertyItem("RepeatAssignAction", "RepeatAssignAction", "Behavior", "Indicates whether the assignment action will repeat the previous action.");


            this.AddDesignerActionPropertyItem("Anchor", "Anchor", "Layout", "Specifies the edges of the container to which a certain control is to be bound.");


            this.AddDesignerActionPropertyItem("Dock", "Dock", "Layout", "Defines which borders of the control are to be bound to the container.");

            if(this.UseVisualStyle)
                this.AddDesignerActionPropertyItem("ButtonStyle", "Button Styles", "Miscellaneous", "Specifies the different styles of buttons");
            if(this.ThemesEnabled)
                this.AddDesignerActionPropertyItem("UseVisualStyle", "Enable Visual styles", "Miscellaneous", "Enables Visual Styles for buttons");


        }
        public CultureInfo Culture
        {
            get
            {
                CultureInfo culture = new CultureInfo("en-US");
                if (this.Control != null)
                {
                    CalculatorControl control = this.Control as CalculatorControl;
                    culture = control.Culture;
                }
                return culture;
            }
            set
            {
                SetValue("Culture", value);
            }
            
        }
        public HorizontalAlignment DisplayTextAlign
        {
            get
            {
                HorizontalAlignment displayTextAlign = HorizontalAlignment.Right;
                if (this.Control != null)
                {
                    CalculatorControl control = this.Control as CalculatorControl;
                    displayTextAlign = control.DisplayTextAlign;
                }
                return displayTextAlign;
            }
            set
            {
                SetValue("DisplayTextAlign", value);
            }

        }

        public CalculatorLayoutTypes LayoutType
        {

            get
            {
                CalculatorLayoutTypes layoutType = CalculatorLayoutTypes.WindowsStandard;
                if (this.Control != null)
                {
                    CalculatorControl control = this.Control as CalculatorControl;
                    layoutType = control.LayoutType;
                }
                return layoutType;
            }
            set
            {
                SetValue("LayoutType", value);
            }

        }

        public bool ShowDisplayArea
        {

            get
            {
                bool showDisplayArea = true;
                if (this.Control != null)
                {
                    CalculatorControl control = this.Control as CalculatorControl;
                    showDisplayArea = control.ShowDisplayArea;
                }
                return showDisplayArea;
            }
            set
            {
                SetValue("ShowDisplayArea", value);
            }

        }

        public bool ThemesEnabled
        {

            get
            {
                bool themesEnabled = true;
                if (this.Control != null)
                {
                    CalculatorControl control = this.Control as CalculatorControl;
                    themesEnabled = control.ThemesEnabled;
                }
                return themesEnabled;
            }
            set
            {
                SetValue("ThemesEnabled", value);
            }

        }

        public bool RepeatAssignAction
        {

            get
            {
                bool repeatAssignAction = true;
                if (this.Control != null)
                {
                    CalculatorControl control = this.Control as CalculatorControl;
                    repeatAssignAction = control.RepeatAssignAction;
                }
                return repeatAssignAction;
            }
            set
            {
                SetValue("RepeatAssignAction", value);
            }

        }
        public AnchorStyles Anchor
        {

            get
            {
                AnchorStyles anchor = ((System.Windows.Forms.AnchorStyles.Top)|(System.Windows.Forms.AnchorStyles.Left));
                if (this.Control != null)
                {
                    CalculatorControl control = this.Control as CalculatorControl;
                    anchor = control.Anchor;
                }
                return anchor;
            }
            set
            {
                SetValue("Anchor", value);
            }

        }
        public DockStyle Dock
        {

            get
            {
                DockStyle dock = DockStyle.None;
                if (this.Control != null)
                {
                    CalculatorControl control = this.Control as CalculatorControl;
                    dock = control.Dock;
                }
                return dock;
            }
            set
            {
                SetValue("Dock", value);
            }

        }
        public ButtonAppearance ButtonStyle
        {

            get
            {
                ButtonAppearance buttonStyle = ButtonAppearance.Classic;
                if (this.Control != null)
                {
                    CalculatorControl control = this.Control as CalculatorControl;
                    buttonStyle = control.ButtonStyle;
                }
                return buttonStyle;
            }
            set
            {
                SetValue("ButtonStyle", value);
            }

        }
        public bool UseVisualStyle
        {

            get
            {
                bool visualStyle = false;
                if (this.Control != null)
                {
                    CalculatorControl control = this.Control as CalculatorControl;
                    visualStyle = control.UseVisualStyle;
                }
                return visualStyle;
            }
            set
            {
                SetValue("UseVisualStyle", value);
            }

        }
    }
}
#endif