#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    #region Enum
    /// <summary>
    /// Specifies a rounding value for arc-ing page border corners. 
    /// </summary>
    [Documentation.DocumentationExclude()]
    public enum BorderStyleCornerRounding
    {
        /// <summary>
        /// No rounding.
        /// </summary>
        None = 0,

        /// <summary>
        /// Smallest rounding.
        /// </summary>
        Smallest = 1,

        /// <summary>
        /// Smaller rounding.
        /// </summary>
        Smaller = 2,

        /// <summary>
        /// Small rounding.
        /// </summary>
        Small = 3,

        /// <summary>
        /// Medium rounding.
        /// </summary>
        Medium = 5,

        /// <summary>
        /// Big rounding.
        /// </summary>
        Big = 7,

        /// <summary>
        /// Bigger rounding.
        /// </summary>
        Bigger = 9,

        /// <summary>
        /// Biggest rounding.
        /// </summary>
        Biggest = 11
    }

    /// <summary>
    /// Specifies the weight of the line used for drawing the border. This enum is used by the 
    /// <see cref="Syncfusion.Windows.Forms.Diagram.PageBorderStyle"/> and <see cref="Syncfusion.Windows.Forms.Diagram.HeaderFooterBorder"/> 
    /// classes.
    /// </summary>
    public enum BorderWeight
    {
        /// <summary>
        /// A thin line that is 1 pixel wide.
        /// </summary>
        Thin = 1,

        /// <summary>
        /// A thin line that is 2 pixels wide.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// A thick line that is 4 pixels wide.
        /// </summary>
        Thick = 4,
    }
    #endregion

    /// <summary>
    /// Encapsulates the properties of a page border.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contains properties needed to create a pen for drawing the border
    /// of an page. The
    /// <see cref="Syncfusion.Windows.Forms.Diagram.PageBorderStyle.CreatePen"/>
    /// method returns a pen to draw the border.
    /// </para>
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof(PageBorderStyleConverter))]
    public class PageBorderStyle
        : PropertyContainer
    {
        #region Class members
        private bool m_bVisible;
        private Color m_clrColor;
        private BorderWeight m_weight;
        private int m_nAlphaFactor;
        private DashStyle m_borderDashStyle;
        private BorderStyleCornerRounding m_cornerRounding;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PageBorderStyle"/> class.
        /// </summary>
        public PageBorderStyle()
            : base()
        {
            m_nAlphaFactor = 128;
            m_clrColor = Color.Black;
            m_weight = Diagram.BorderWeight.Thin;
            m_borderDashStyle = DashStyle.Dash;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageBorderStyle"/> class.
        /// </summary>
        /// <param name="src">The page border style.</param>
        public PageBorderStyle(PageBorderStyle src)
            : base()
        {
            m_bVisible = src.m_bVisible;
            m_clrColor = src.m_clrColor;
            m_weight = src.m_weight;
            m_nAlphaFactor = src.m_nAlphaFactor;
            m_borderDashStyle = src.m_borderDashStyle;
            m_cornerRounding = src.m_cornerRounding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageBorderStyle"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected PageBorderStyle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_bVisible = info.GetBoolean("visible");
            m_clrColor = (Color)info.GetValue("color", typeof(Color));
            m_weight = (BorderWeight)info.GetValue("weight", typeof(BorderWeight));
            m_nAlphaFactor = info.GetInt32("alphaFactor");
            m_borderDashStyle = (DashStyle)info.GetValue("borderDashStyle", typeof(DashStyle));
            m_cornerRounding = (BorderStyleCornerRounding)info.GetValue("cornerRounding", typeof(BorderStyleCornerRounding));
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether the border is visible or not.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Indicates if the border is visible.")]
        public bool ShowBorder
        {
            get 
            { 
                return m_bVisible; 
            }
            set
            {
                if (m_bVisible != value && OnPropertyChanging(DPN.ShowBorder, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.ShowBorder);
                    //// assign new value
                    m_bVisible = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.ShowBorder);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the pen used for drawing the border.
        /// </summary>
        [Browsable(true)]
        [Description("The pen color used for drawing the border.")]
        public Color BorderColor
        {
            get 
            { 
                return m_clrColor; 
            }
            set
            {
                if (m_clrColor != value && OnPropertyChanging(DPN.BorderColor, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.BorderColor);
                    //// assign new value
                    m_clrColor = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.BorderColor);
                }
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the line used for drawing the border.
        /// </summary>
        /// <value>A <see cref="Syncfusion.Windows.Forms.Diagram.BorderWeight"/> value.</value>
        [Browsable(true)]
        [DefaultValue(BorderWeight.Thin)]
        [Description("The thickness of the border line.")]
        public BorderWeight BorderWeight
        {
            get 
            { 
                return m_weight; 
            }
            set
            {
                if (m_weight != value && OnPropertyChanging(DPN.BorderWeight, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.BorderWeight);
                    //// assign new value
                    m_weight = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.BorderWeight);
                }
            }
        }

        /// <summary>
        /// Gets the width in pixels for the border line.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderWidth
        {
            get { return (int)this.BorderWeight; }
        }

        /// <summary>
        /// Gets or sets the type of line used for drawing the border. 
        /// </summary>
        /// <value>
        /// A <see cref="DashStyle"/> value.
        /// </value>
        [Browsable(true)]
        [DefaultValue(DashStyle.Dash)]
        [Description("The type of line used for drawing the border.")]
        public DashStyle BorderDashStyle
        {
            get 
            { 
                return m_borderDashStyle; 
            }
            set
            {
                if (m_borderDashStyle != value && OnPropertyChanging(DPN.BorderDashStyle, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.BorderDashStyle);
                    //// assign new value
                    m_borderDashStyle = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.BorderDashStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets Transparency of the border.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(128)]
        [Description("The transparency value of the border.")]
        public int AlphaFactor
        {
            get 
            { 
                return m_nAlphaFactor; 
            }
            set
            {
                if (m_nAlphaFactor != value && OnPropertyChanging(DPN.AlphaFactor, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.AlphaFactor);
                    //// assign new value
                    m_nAlphaFactor = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.AlphaFactor);
                }
            }
        }

        /// <summary>
        /// Gets or sets Rounding of the border corners.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(0)]
        [Description("Specifies a rounding for the border corners.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BorderStyleCornerRounding BorderCornerRounding
        {
            get 
            { 
                return m_cornerRounding; 
            }
            set
            {
                if (m_cornerRounding != value && OnPropertyChanging(DPN.BorderCornerRounding, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.BorderCornerRounding);
                    //// assign new value
                    m_cornerRounding = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.BorderCornerRounding);
                }
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Creates a pen to draw the border.
        /// </summary>
        /// <returns>System.Drawing.Pen object matching the border style.</returns>
        public Pen CreatePen()
        {
            Pen penToReturn = new Pen(Color.FromArgb(this.AlphaFactor, this.BorderColor), this.BorderWidth);
            penToReturn.DashStyle = this.BorderDashStyle;

            return penToReturn;
        }
        #endregion

        #region Class helper methods
        [Documentation.DocumentationExclude()]
        private bool ShouldSerializeBorderColor()
        {
            return (this.BorderColor != Color.Black);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>Property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return DPN.PageBorderStyle;
        }

        /// <summary>
        /// Creates a shallow copy of the PageBorderStyle object.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new PageBorderStyle(this);
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("visible", m_bVisible);
            info.AddValue("color", m_clrColor);
            info.AddValue("weight", m_weight);
            info.AddValue("alphaFactor", m_nAlphaFactor);
            info.AddValue("borderDashStyle", m_borderDashStyle);
            info.AddValue("cornerRounding", m_cornerRounding);
        }
        #endregion
    }
}
