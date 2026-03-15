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
    public class ClockActionList : SyncActionListBase<Clock>
    {
        /// <summary>
        /// Initializes a new instance of the ClockActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public ClockActionList(IComponent component)
            : base(component)
        {
        }
        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - Clock");

            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");
            //Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("Text", "Text", "Appearance", "The text associated with the control.");
            this.AddDesignerActionPropertyItem("Type", "ClockType", "Appearance", "The type of the control.");
            // Behavior Category
            this.AddDesignerActionHeaderItem("Behavior");
            if (this.Control.ClockType == ClockTypes.Analog)
                this.AddDesignerActionPropertyItem("hourdesignator", "Show HourDesignator", "Behavior", "Shows the Hour Designator.");
            else
            {
                if(!this.Control.ShowClockFrame)
                    this.AddDesignerActionPropertyItem("Shapes", "Clock Shapes", "Behavior", "The shape of the control");
                this.AddDesignerActionPropertyItem("Frames", "Show Frames", "Behavior", "Shows the Frame of the control");
                if (this.Control.ShowClockFrame)
                    this.AddDesignerActionPropertyItem("Types", "Frame Types", "Behavior", "The Frame Types of the control");
                this.AddDesignerActionPropertyItem("Dates", "Display Dates", "Behavior", "Shows the date in the control");

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
                    Clock control = this.Control as Clock;
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
                    Clock control = this.Control as Clock;
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
        /// Gets or sets the Clock type.
        /// </summary>
        /// <value>The style.</value>
        public ClockTypes Type
        {
            get
            {
                ClockTypes Type = ClockTypes.Analog;
                if (this.Control != null)
                {
                    Clock control = this.Control as Clock;
                    Type = control.ClockType;
                }

                return Type;
            }
            set
            {
                SetValue("ClockType", value);
            }
        }
        /// <summary>
        /// Gets or sets the hourdesignator
        /// </summary>
        public bool hourdesignator
        {
            get
            {
                bool hourdesignator = false;
                if (this.Control != null)
                {
                    Clock control = this.Control as Clock;
                    hourdesignator = control.ShowHourDesignator;
                }

                return hourdesignator;
            }

            set
            {
                SetValue("ShowHourDesignator", value);
            }
        }
        /// <summary>
        /// Gets or sets the clock shapes
        /// </summary>
        public  ClockShapes Shapes
        {
            get
            {
                ClockShapes Shapes = ClockShapes.Circle;
                if (this.Control != null)
                {
                    Clock control = this.Control as Clock;
                    Shapes = control.ClockShape;
                }

                return Shapes;
            }

            set
            {
                SetValue("ClockShape", value);
            }
        }
        /// <summary>
        /// Gets or sets the frames
        /// </summary>
        public bool Frames
        {
            get
            {
                bool Frames = false;
                if (this.Control != null)
                {
                    Clock control = this.Control as Clock;
                    Frames = control.ShowClockFrame;
                }

                return Frames;
            }

            set
            {
                SetValue("ShowClockFrame", value);
            }
        }
        /// <summary>
        /// Gets or sets to show dates
        /// </summary>
        public bool Dates
        {
            get
            {
                bool Dates = false;
                if (this.Control != null)
                {
                    Clock control = this.Control as Clock;
                    Dates = control.DisplayDates;
                }

                return Dates;
            }

            set
            {
                SetValue("DisplayDates", value);
            }
        }
        /// <summary>
        /// Gets or sets the frame types
        /// </summary>
        public ClockFrames Types
        {
            get
            {
                ClockFrames Types = ClockFrames.CircularFrame;
                if (this.Control != null)
                {
                    Clock control = this.Control as Clock;
                    Types = control.ClockFrame;
                }

                return Types;
            }

            set
            {
                SetValue("ClockFrame", value);
            }
        }
    }
}
#endif