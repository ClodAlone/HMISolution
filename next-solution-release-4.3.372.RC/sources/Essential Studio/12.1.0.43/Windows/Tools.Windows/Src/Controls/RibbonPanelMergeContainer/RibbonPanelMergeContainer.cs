#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.ComponentModel;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// RibbonPanelMerge Container 
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Tools.RibbonPanelMergeContainer), "ToolboxIcons.RibbonPanelMergeContainer.bmp")]
    public partial class RibbonPanelMergeContainer : RibbonPanel
    {
        #region Constructors
        public RibbonPanelMergeContainer()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(RibbonPanelMergeContainer));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.Size = new Size(100, 50);
            this.Visible = false;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the height and width of the control.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Size Size
        {
            get 
            { 
                return base.Size; 
            }
            set
            { 
                base.Size = value; 
            }
        }

        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of the control relative
        ///     to the upper-left corner of its container.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Point Location
        {
            get { return base.Location; }
            set { base.Location = value; }
        }

        /// <summary>
        /// Gets default color scheme
        /// </summary>
        [Description("Specifies the default color scheme (Silver. Blue).")]
        protected override ToolStripEx.ColorScheme DefaultOfficeColorScheme
        {
            get { return ToolStripEx.ColorScheme.Blue; }
        }

        /// <summary>
        /// Gets default border style of toolstrips
        /// </summary>
        [Description("Specifies the default border style.")]
        protected override ToolStripBorderStyle DefaultBorderStyle
        {
            get { return ToolStripBorderStyle.Etched; }
        }

        /// <summary>
        /// Gets default caption style.
        /// </summary>
        [Description("Specifies the default style (Top, Bottom) of caption")]
        protected override CaptionStyle DefaultCaptionStyle
        {
            get { return CaptionStyle.Bottom; }
        }
        #endregion
    }
}
#endif