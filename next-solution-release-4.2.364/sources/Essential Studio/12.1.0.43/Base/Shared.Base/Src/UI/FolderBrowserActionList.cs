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
    public class FolderBrowserActionList : SyncActionListBase<FolderBrowser>
    {
        /// <summary>
        /// Initializes a new instance of the CheckBoxAdvActionList class.
        /// </summary>
        /// <param name="component"> Represents component</param>
        public FolderBrowserActionList(IComponent component)
            : base(component)
        {
        }
        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - Folder Browser");

            // Appearance category.
            this.AddDesignerActionHeaderItem("Appearance");
            this.AddDesignerActionPropertyItem("StartLocation", "Start Location", "Appearance", "Gets the start location todisplay the dialog.");
            this.AddDesignerActionPropertyItem("Style", "Style", "Appearance", "Gets the options for folder dialog.");
        }
        /// <summary>
        /// Gets or sets the options style.
        /// </summary>
        /// <value>The style.</value>
        public FolderBrowserStyles Style
        {
            get
            {
                FolderBrowserStyles style = FolderBrowserStyles.RestrictToFilesystem;
                if (this.Control != null)
                {
                    FolderBrowser control = this.Control as FolderBrowser;
                    style = control.Style;
                }

                return style;
            }
            set
            {
                SetValue("Style", value);
            }
        }
        /// <summary>
        /// Gets or sets the start location
        /// </summary>
        /// <value>The style.</value>
        public FolderBrowserFolder StartLocation
        {
            get
            {
                FolderBrowserFolder startlocation = FolderBrowserFolder.Desktop;
                if (this.Control != null)
                {
                    FolderBrowser control = this.Control as FolderBrowser;
                    startlocation = control.StartLocation;
                }

                return startlocation;
            }
            set
            {
                SetValue("StartLocation", value);
            }
        }
    }
}
#endif