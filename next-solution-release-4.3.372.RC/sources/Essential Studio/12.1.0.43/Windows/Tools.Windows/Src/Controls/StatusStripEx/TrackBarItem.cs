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
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    [Designer(typeof(Design.TrackBarItemDesigner))]
    [System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip)]
    public class TrackBarItem : ToolStripControlHost
    {
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the TrackBarItem class.
        /// </summary>
        public TrackBarItem()
            : base(CreateControlInstance())
        {
            CTRLSIZE = this.Size;
        }
        #endregion

        #region Properties
  
        public TrackBarEx TrackBarExControl
        {
            get
            {
                return Control as TrackBarEx;
            }
        }

        /// <summary> Gets or sets a value indicating whether control is transparent. </summary>
        [DefaultValue(true)]
        public bool Transparent
        {
            get
            {
                return TrackBarExControl.Transparent;
            }
            set
            {
                TrackBarExControl.Transparent = value;
            }
        }

        /// <summary> Gets or sets value of TrackBar position. </summary>
        [Category("Behavior")]
        [Description("Value of TrackBar position.")]
        public int Value
        {
            get
            {
                return TrackBarExControl.Value;
            }
            set
            {
                TrackBarExControl.Value = value;
            }
        }

        /// <summary> Gets or sets minimum value of TrackBar. </summary>
        [Category("Behavior")]
        [Description("Minimum value of TrackBar.")]
        [DefaultValue(0)]
        public int Minimum
        {
            get
            {
                return TrackBarExControl.Minimum;
            }
            set
            {
                TrackBarExControl.Minimum = value;
            }
        }

        /// <summary> Gets or sets maximum value of TrackBar. </summary>
        [Category("Behavior")]
        [Description("Maximum value of TrackBar.")]
        public int Maximum
        {
            get
            {
                return TrackBarExControl.Maximum;
            }
            set
            {
                TrackBarExControl.Maximum = value;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of TrackBarEx control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(typeof(Orientation), "Horizontal")]
        [Description("Specify the orientation of TrackBarEx control.")]
        public Orientation Orientation
        {
            get
            {
                return TrackBarExControl.Orientation;
            }
            set
            {
                TrackBarExControl.Orientation = value;
            }
        }
        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        /// gets or sets the touchmode
        /// </summary>
		[DefaultValue(false)]
        public virtual bool EnableTouchMode
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
        /// applies the scaling
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float sf)
        {
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * sf), (int)(CTRLSIZE.Height * sf));
            isScaling = false;
            this.Invalidate();
        }
        /// <summary>
        /// Font changed event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        #endregion
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the Value property of a track bar changes, either by movement of the scroll box or by manipulation in code.
        /// </summary>
        [Description("Occurs when the Value property of a track bar changes, either by movement of the scroll box or by manipulation in code.")]
        public event EventHandler ValueChanged
        {
            add
            {
                this.TrackBarExControl.ValueChanged += value;
            }
            remove
            {
                this.TrackBarExControl.ValueChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when either a mouse or keyboard action moves the scroll box.
        /// </summary>
        [Description("Occurs when either a mouse or keyboard action moves the scroll box.")]
        public event EventHandler Scroll
        {
            add
            {
                this.TrackBarExControl.Scroll += value;
            }
            remove
            {
                this.TrackBarExControl.Scroll -= value;
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Create the actual control, note this is static so it can be called from the
        /// constructor.
        /// </summary>
        /// <returns>Returns control</returns>
        private static Control CreateControlInstance()
        {
            TrackBarEx trackBar = new TrackBarEx();
            trackBar.Transparent = true;

            return trackBar;
        }
    
        private bool ShouldSerializeValue()
        {
            return this.Value > this.Minimum;
        }
        #endregion

        #region Overrides
     
        protected override void SetBounds(Rectangle bounds)
        {
            base.SetBounds(bounds);

            if (Control != null)
            {
                this.Control.Invalidate();
            }
        }
        #endregion
    }
}

#endif
