#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A FillStyle is a collection of properties that define a brush used for
    /// fill operations during rendering.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(FillStyleConverter))]
    public class HandleStyle
        : PropertyContainer
    {
        #region Class members
        private Color m_clrFill;
        private Color m_clrOutline;
        private int m_nFillColorAlphaFactor;
        private int m_nOutlineColorAlphaFactor;
        private HandleSize m_handleSize;
        private HandleStyleType m_style;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HandleStyle"/> class.
        /// </summary>
        public HandleStyle()
        {
            m_clrFill = Color.YellowGreen;
            m_clrOutline = Color.Black;
            m_nFillColorAlphaFactor = 255;
            m_nOutlineColorAlphaFactor = 255;
            m_handleSize = HandleSize.Medium;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandleStyle"/> class.
        /// </summary>
        /// <param name="src">The handle style.</param>
        public HandleStyle(HandleStyle src)
            : base(src)
        {
            m_clrFill = src.m_clrFill;
            m_clrOutline = src.m_clrOutline;
            m_nFillColorAlphaFactor = src.m_nFillColorAlphaFactor;
            m_nOutlineColorAlphaFactor = src.m_nOutlineColorAlphaFactor;
            m_handleSize = src.m_handleSize;
            m_style = src.m_style;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandleStyle"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected HandleStyle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_clrFill = (Color)info.GetValue("color", typeof(Color));
            m_clrOutline = (Color)info.GetValue("outlineColor", typeof(Color));
            m_nFillColorAlphaFactor = info.GetInt32("fillColorAlphaFactor");
            m_nOutlineColorAlphaFactor = info.GetInt32("outlineColorAlphaFactor");
            m_handleSize = (HandleSize)info.GetValue("handleSize", typeof(HandleSize));
            m_style = (HandleStyleType)info.GetValue("handleStyleType", typeof(HandleStyleType));
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets Color to use for the brush.
        /// </summary>
        /// <remarks>
        /// NOTE: If <see cref="Syncfusion.Windows.Forms.Diagram.FillStyle.Type"/> is
        /// set to FillType.LinearGradient, then this is the ending color for the
        /// gradient.
        /// </remarks>
        [Browsable(true)]
        [Description("Color used for fill handle interior.")]
        public Color FillColor
        {
            get 
            { 
                return m_clrFill; 
            }
            set
            {
                if (m_clrFill != value && OnPropertyChanging(DPN.FillColor, value))
                {
                    // assign new value
                    m_clrFill = value;

                    // raise property changed event
                    OnPropertyChanged(DPN.FillColor);
                }
            }
        }

        /// <summary>
        /// Gets or sets Alpha blending factor.
        /// </summary>
        [Browsable(true)]
        [Description("Alpha blending factor ( 0 = transparent, 255 = opaque )")]
        [DefaultValue(255)]
        public int FillColorAlphaFactor
        {
            get 
            { 
                return m_nFillColorAlphaFactor; 
            }
            set
            {
                if (value < CommonUsedValues.TRANSPARENT && value > CommonUsedValues.OPAQUE)
                    throw new ArgumentOutOfRangeException("ColorAlphaFactor");

                if (m_nFillColorAlphaFactor != value && OnPropertyChanging(DPN.FillColorAlphaFactor, value))
                {
                    // assign new value
                    m_nFillColorAlphaFactor = value;

                    // raise property changed event
                    OnPropertyChanged(DPN.FillColorAlphaFactor);
                }
            }
        }

        /// <summary>
        /// Gets or sets Alpha blending factor for the ForeColor.
        /// </summary>
        [Browsable(true)]
        [Description("Alpha blending factor ( 0 = transparent, 255 = opaque )")]
        [DefaultValue(255)]
        public int OutlineColorAlphaFactor
        {
            get 
            {
                return m_nOutlineColorAlphaFactor; 
            }
            set
            {
                if (value < CommonUsedValues.TRANSPARENT && value > CommonUsedValues.OPAQUE)
                    throw new ArgumentOutOfRangeException("ForeColorAlphaFactor");

                if (m_nOutlineColorAlphaFactor != value && OnPropertyChanging(DPN.OutlineColorAlphaFactor, value))
                {
                    // assign new value
                    m_nOutlineColorAlphaFactor = value;
                    
                    // raise property changed event
                    OnPropertyChanged(DPN.OutlineColorAlphaFactor);
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreground color used for the fill style.
        /// </summary>
        /// <remarks>
        /// NOTE: If <see cref="Syncfusion.Windows.Forms.Diagram.FillStyle.Type"/> is
        /// set to FillType.LinearGradient, then this is the starting color for the
        /// gradient.
        /// </remarks>
        [Browsable(true)]
        [Description("The foreground color used for the fill.")]
        public Color OutlineColor
        {
            get 
            { 
                return m_clrOutline; 
            }
            set
            {
                if (m_clrOutline != value && OnPropertyChanging(DPN.OutlineColor, value))
                {
                    // assign new value
                    m_clrOutline = value;
                    
                    // raise property changed event
                    OnPropertyChanged(DPN.OutlineColor);
                }
            }
        }

        /// <summary>
        /// Gets or sets Handle Type.
        /// </summary>
        [Browsable(true)]
        [Description("Type of handle.")]
        [DefaultValue(HandleStyleType.Square)]
        public HandleStyleType Type
        {
            get 
            { 
                return m_style; 
            }
            set
            {
                if (m_style != value && OnPropertyChanging(DPN.Type, value))
                {
                    // assign new value
                    m_style = value;
                    
                    // raise property changed event
                    OnPropertyChanged(DPN.Type);
                }
            }
        }

        /// <summary>
        /// Gets or sets Hatch brush style to create for filled regions.
        /// </summary>
        [Browsable(true)]
        [Description("Handle size.")]
        [DefaultValue(HandleSize.Medium)]
        public HandleSize HandleSize
        {
            get 
            { 
                return m_handleSize; 
            }
            set
            {
                if (m_handleSize != value && OnPropertyChanging(DPN.HandleSize, value))
                {
                    // assign new value
                    m_handleSize = value;
                    
                    // raise property changed event
                    OnPropertyChanged(DPN.HandleSize);
                }
            }
        }
        #endregion

        #region Class helper Methods
        /// <summary>
        /// Shows whether OutlineColor will be serializable.
        /// </summary>
        /// <returns>true, if serialize color.</returns>
        protected bool ShouldSerializeColor()
        {
            return (Color.Black != this.OutlineColor);
        }

        /// <summary>
        /// Shows whether FillColor will be serializable.
        /// </summary>
        /// <returns>rue, if serialize forecolor.</returns>
        protected bool ShouldSerializeForeColor()
        {
            return (Color.Green != this.FillColor);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>Property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return "HandleStyle";
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new HandleStyle(this);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("color", m_clrFill);
            info.AddValue("outlineColor", m_clrOutline);
            info.AddValue("fillColorAlphaFactor", m_nFillColorAlphaFactor);
            info.AddValue("outlineColorAlphaFactor", m_nOutlineColorAlphaFactor);
            info.AddValue("handleSize", m_handleSize);
            info.AddValue("handleStyleType", m_style);
        }
        #endregion
    }

    /// <summary>
    /// Rotation handle style.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(FillStyleConverter))]
    public class RotationHandleStyle
        : HandleStyle
    {
        #region Class initialize/filnalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RotationHandleStyle"/> class.
        /// </summary>
        public RotationHandleStyle()
        {
            this.HandleSize = HandleSize.Giant;
        }
        #endregion
    }

    /// <summary>
    /// Resize handle style.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(FillStyleConverter))]
    public class ResizeHandlesStyle
        : HandleStyle
    {
    }

    /// <summary>
    /// Pin point style.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(FillStyleConverter))]
    public class PinPointStyle
        : HandleStyle
    {
    }

    /// <summary>
    /// Control point style.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(FillStyleConverter))]
    public class ControlPointStyle
        : HandleStyle
    {
    }

    /// <summary>
    /// Handle styles
    /// </summary>
    public class HandleStyles
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HandleStyles"/> class.
        /// </summary>
        public HandleStyles()
        {
            RotationHandleStyle = new RotationHandleStyle();
            ResizeHandlesStyle = new ResizeHandlesStyle();
            PinPointStyle = new PinPointStyle();
            ControlPointStyle = new ControlPointStyle();
        }
        #endregion

        /// <summary>
        /// Rotation handle styles.
        /// </summary>
        public RotationHandleStyle RotationHandleStyle;

        /// <summary>
        /// Resize handles styles.
        /// </summary>
        public ResizeHandlesStyle ResizeHandlesStyle;

        /// <summary>
        /// Pin point style.
        /// </summary>
        public PinPointStyle PinPointStyle;

        /// <summary>
        /// Control point style.
        /// </summary>
        public ControlPointStyle ControlPointStyle;

        #region IServiceReferenceHolder Members
        /// <summary>
        /// Updates the service references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            RotationHandleStyle.UpdateServiceReferences(provider);
            ResizeHandlesStyle.UpdateServiceReferences(provider);
            PinPointStyle.UpdateServiceReferences(provider);
            ControlPointStyle.UpdateServiceReferences(provider);
        }
        #endregion
    }

    /// <summary>
    /// Handle style type enumeration
    /// </summary>
    public enum HandleStyleType
    {
        /// <summary>
        /// Square style
        /// </summary>
        Square = 0,

        /// <summary>
        /// Circle style
        /// </summary>
        Circle,

        /// <summary>
        /// Rhombus style.
        /// </summary>
        Rhomb
    }

    /// <summary>
    /// Handle size enumeration
    /// </summary>
    public enum HandleSize
    {
        /// <summary>
        /// Small size.
        /// </summary>
        Small = 5,

        /// <summary>
        /// Medium style.
        /// </summary>
        Medium = 7,

        /// <summary>
        /// Bug style.
        /// </summary>
        Big = 9,

        /// <summary>
        /// Giant style.
        /// </summary>
        Giant = 11
    }
}
