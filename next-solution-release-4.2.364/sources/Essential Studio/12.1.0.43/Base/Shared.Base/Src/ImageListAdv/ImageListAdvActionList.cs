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
    public class ImageListAdvActionList : SyncActionListBase<ImageListAdv>
    {
        /// <summary>
        /// Initializes a new instance of the ClockActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public ImageListAdvActionList(IComponent component)
            : base(component)
        {
        }
        /// <summary>
        /// Overrridden InitializeActionList.
        /// </summary>
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - ImageListAdv");
            this.AddDesignerActionHeaderItem("Misc");
            this.AddDesignerActionPropertyItem("Images", "Images", "Misc", "Collection of Images.");
        }
        /// <summary>
        /// Gets the image collection
        /// </summary>
        public ImageCollection Images
        {
            get
            {
                ImageCollection image = new ImageCollection();
                if (this.Control != null)
                {
                    ImageListAdv control = this.Control as ImageListAdv;
                    image = control.Images;
                }

                return image;
            }
            set
            {
                SetValue("Images", value);
            }
        }
    }
}
#endif