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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// This class holds information on lines rendered as part of the chart's rendering system.
    /// </summary>
    /// <seealso cref="ChartAxis.GridLineType"/>
    /// <seealso cref="ChartAxis.LineType"/>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LineInfo
    {
        #region Members
        private Color backColor = Color.Black;
        private Brush brush;
        private DashStyle dashStyle = DashStyle.Solid;
        private Color foreColor = Color.Black;

        private Pen pen = new Pen(Color.Black);
        private PenType penType = PenType.SolidColor;
        private float width = 1F;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when properties is changed.
        /// </summary>
        /// <internalonly/>
        [DocumentationExclude()]
        public event EventHandler SettingsChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the backcolor that is to be associated with the line.
        /// </summary>
        [DefaultValue(typeof(Color), "Black"), ChartTemplate(ChartTemplateSet.Simple), NotifyParentProperty(true)]
        [Description("Specifies the backcolor that is to be associated with the line.")]
        public Color BackColor
        {
            get
            {
                return backColor;
            }

            set
            {
                if (backColor != value)
                {
                    backColor = value;
                    RefreshPen();
                }
            }
        }

        /// <summary>
        ///  Gets the brush information that is to be used with the line.
        /// </summary>
        [DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets the brush information that is to be used with the line.")]
        public Brush Brush
        {
            get
            {
                return brush;
            }
        }

        /// <summary>
        /// Gets or sets the style of the line.
        /// </summary>
        [DefaultValue(DashStyle.Solid), ChartTemplate(ChartTemplateSet.Simple), NotifyParentProperty(true)]
        [Description("Specifies the style of the line.")]
        public DashStyle DashStyle
        {
            get
            {
                return dashStyle;
            }

            set
            {
                if (dashStyle != value)
                {
                    if (value != DashStyle.Custom)
                    {
                        dashStyle = value;
                        RefreshPen();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the forecolor of the line.
        /// </summary>
        [DefaultValue(typeof(Color), "Black"), ChartTemplate(ChartTemplateSet.Simple), NotifyParentProperty(true)]
        [Description("Specifies the forecolor of the line.")]
        public Color ForeColor
        {
            get
            {
                return foreColor;
            }

            set
            {
                if (foreColor != value)
                {
                    foreColor = value;
                    RefreshPen();
                }
            }
        }

        /// <summary>
        /// Gets the pen used to render the line.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Returns the pen used to render the line.")]
        public Pen Pen
        {
            get
            {
                return pen;
            }
        }

        /// <summary>
        /// Gets or sets the type of pen that is to be used with the line.
        /// </summary>
        [DefaultValue(PenType.SolidColor) , NotifyParentProperty(true)]
        [Description("Specifies the type of pen that is to be used with the line.")]
        public PenType PenType
        {
            get
            {
                return penType;
            }

            set
            {
                if (penType != value)
                {
                    penType = value;
                    RefreshPen();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the line.
        /// </summary>
        [DefaultValue(1F), ChartTemplate(ChartTemplateSet.Simple), NotifyParentProperty(true)]
        [Description("Specifies the width of the line.")]
        public float Width
        {
            get
            {
                return width;
            }

            set
            {
                if (width != value)
                {
                    width = value;
                    RefreshPen();
                }
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LineInfo"/> class.
        /// </summary>
        public LineInfo()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the <see cref="E:SettingsChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <internalonly/>
        [DocumentationExclude()]
        protected virtual void OnSettingsChanged(EventArgs e)
        {
            if (this.SettingsChanged != null)
            {
                this.SettingsChanged(this, e);
            }
        }

        /// <summary>
        /// Resets the value of BackColor property.
        /// </summary>
        /// <internalonly/>
        [DocumentationExclude()]
        protected void ResetBackColor()
        {
            this.BackColor = Color.Black;
        }

        /// <summary>
        /// Resets the value of ForeColor property.
        /// </summary>
        /// <internalonly/>
        [DocumentationExclude()]
        protected void ResetForeColor()
        {
            this.ForeColor = Color.Black;
        }

        /// <summary>
        /// Indicates whether the BackColor should be serialized.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        /// <internalonly/>
        [DocumentationExclude()]
        protected bool ShouldSerializeBackColor()
        {
            if (this.BackColor == Color.Black)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Indicates whether the ForeColor property should be serialized
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        /// <internalonly/>
        [DocumentationExclude()]
        protected bool ShouldSerializeForeColor()
        {
            if (this.ForeColor == Color.Black)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Refreshes the pen.
        /// </summary>
        private void RefreshPen()
        {
            switch (penType)
            {
                case PenType.HatchFill:
                    brush = new HatchBrush(HatchStyle.SmallGrid, foreColor, backColor);
                    break;

                case PenType.LinearGradient:
                    brush = new LinearGradientBrush(new Point(0, 0), new Point(100, 0), foreColor, backColor);
                    break;

                default:
                    brush = new SolidBrush(foreColor);
                    break;
            }

            pen = new Pen(brush, width);
            pen.DashStyle = dashStyle;
            pen.LineJoin = LineJoin.Bevel;
            pen.Alignment = PenAlignment.Center;
            this.OnSettingsChanged(EventArgs.Empty);
        }
        #endregion
    }
}