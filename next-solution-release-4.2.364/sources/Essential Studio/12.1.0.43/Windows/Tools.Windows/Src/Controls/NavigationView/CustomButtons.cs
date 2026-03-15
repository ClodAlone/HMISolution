#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Syncfusion.Windows.Forms.Tools.Navigation
{
    using Design;
    using System;

    /// <summary>
    /// This button can be hosted in <see cref="NavigationView"/>.
    /// </summary>
    [Designer(typeof(CustomButtonsDesigner))]
    [ToolboxItem(false)]
    public partial class CustomButton :
        ButtonAdv
    {
        #region Constants

        /// <summary>
        /// Default width of <see cref="CustomButton"/> control.
        /// </summary>
        public const int DefaultWidth = 26;

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        /// Default font style of the control
        /// </summary>
        private static Font FONTSTYLE = default(Font);

        /// <summary>
        /// Font which stored after changed in design
        /// </summary>

        private static Font USERFONTSTYLE = default(Font);

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomButton"/> class.
        /// </summary>
        public CustomButton()
        {
            InitializeComponent();

            this.UseVisualStyle = true;
            CTRLSIZE = this.Size;
        }

        #region For Touch

        bool isScaling = false;
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }

        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }
        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }

        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        /// <summary>
        ///Size changed
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
             base.OnSizeChanged(e);
             if (!EnableTouchMode && this.DesignMode)
             {
                 CTRLSIZE = this.Size;
             }
        }

        #endregion
        /// <summary>
        /// Gets the default size of the control.
        /// </summary>
        /// <value></value>
        /// <returns>The default <see cref="T:System.Drawing.Size"/> of the control.</returns>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(DefaultWidth, NavigationView.DefaultHeight);
            }
        }
    }

    /// <summary>
    /// Stores collection of <see cref="CustomButton"/> instances.
    /// </summary>
    [Editor(typeof(CustomButtonCollectionEditor), typeof(UITypeEditor))]
    public class CustomButtonCollection :
        ObservableCollectionBase<CustomButton>
    {
    }
}
