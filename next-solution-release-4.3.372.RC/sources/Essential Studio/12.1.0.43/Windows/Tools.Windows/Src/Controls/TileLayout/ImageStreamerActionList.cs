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
    public class ImageStreamerActionList : SyncActionListBase<ImageStreamer>
    {
        /// <summary>
        /// Initializes a new instance of the ClockActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public ImageStreamerActionList(IComponent component)
            : base(component)
        {
        }
        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - ImageStreamer");
            // Design Category
            this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the name used in code to identify the object.");
            this.AddDesignerActionPropertyItem("Text", "Text", "Appearance", "The text associated with the control.");

            //Appearance category.
            this.AddDesignerActionHeaderItem("Animation");
            this.AddDesignerActionPropertyItem("Animationspeed", "Animation Speed", "Animation", "Gets or Sets the animation speed.");
            this.AddDesignerActionPropertyItem("Sliderspeed", "Slider Speed", "Animation", "Gets or Sets the slider speed.");
            this.AddDesignerActionPropertyItem("Textanimation", "Text Animation", "Animation", "Gets or Sets the text animation.");
            if(this.Textanimation)
                this.AddDesignerActionPropertyItem("Textdirection", "Text Direction", "Animation", "Gets or Sets the text animation direction.");

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
                    ImageStreamer control = this.Control as ImageStreamer;
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
                    ImageStreamer control = this.Control as ImageStreamer;
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
        /// Gets the animation speed
        /// </summary>
        public  int Animationspeed
        {
            get
            {
                int speed = 10;
                if (this.Control != null)
                {
                    ImageStreamer control = this.Control as ImageStreamer;
                    speed = control.AnimationSpeed;
                }

                return speed;
            }
            set
            {
                SetValue("AnimationSpeed", value);
            }
        }
        /// <summary>
        /// Gets the slider speed
        /// </summary>
        public int Sliderspeed
        {
            get
            {
                int sliderspeed = 1000;
                if (this.Control != null)
                {
                    ImageStreamer control = this.Control as ImageStreamer;
                    sliderspeed = control.SliderSpeed;
                }

                return sliderspeed;
            }
            set
            {
                SetValue("SliderSpeed", value);
            }
        }
        /// <summary>
        /// Gets the text animation
        /// </summary>
        public bool Textanimation
        {
            get
            {
                bool textanimate = false;
                if (this.Control != null)
                {
                    ImageStreamer control = this.Control as ImageStreamer;
                    textanimate = control.TextAnimation;
                }

                return textanimate;
            }
            set
            {
                SetValue("TextAnimation", value);
            }
        }
        /// <summary>
        /// Gets the text animation direction
        /// </summary>
        public  Syncfusion.Windows.Forms.Tools.ImageStreamer.TextStreamDirection Textdirection
        {
            get
            {
                Syncfusion.Windows.Forms.Tools.ImageStreamer.TextStreamDirection direction = ImageStreamer.TextStreamDirection.RightToLeft;
                if (this.Control != null)
                {
                    ImageStreamer control = this.Control as ImageStreamer;
                    direction = control.TextAnimationDirection;
                }

                return direction;
            }
            set
            {
                SetValue("TextAnimationDirection", value);
            }
        }
    }
}
#endif