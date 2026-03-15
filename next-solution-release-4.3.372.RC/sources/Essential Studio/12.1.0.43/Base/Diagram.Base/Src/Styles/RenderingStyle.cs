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
using System.Drawing.Text;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Encapsulates the rendering properties of an object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This style is used to set rendering properties for System.Drawing.Graphics
    /// during rendering.
    /// </para>
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof(RenderingStyleConverter))]
    public class RenderingStyle
        : PropertyContainer
    {
        #region Class members
        /// <summary>
        /// Specifies whether smoothing (antialiasing) is applied to lines and curves
        /// and the edges of filled areas.
        /// </summary>
        private SmoothingMode m_smoothingMode;

        /// <summary>
        /// Specifies text rendering mode.
        /// </summary>
        private TextRenderingHint m_textRenderingHint;

        /// <summary>
        /// Specifies how data is interpolated between endpoints.
        /// </summary>
        private InterpolationMode m_interpolationMode;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingStyle"/> class.
        /// </summary>
        public RenderingStyle()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingStyle"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public RenderingStyle(RenderingStyle src)
            : base(src)
        {
            m_smoothingMode = src.m_smoothingMode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderingStyle"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected RenderingStyle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_smoothingMode = (SmoothingMode)info.GetValue("smoothingMode", typeof(SmoothingMode));
            m_interpolationMode = (InterpolationMode)info.GetValue("interpolationMode", typeof(InterpolationMode));
            m_textRenderingHint = (TextRenderingHint)info.GetValue("textRenderingHint", typeof(TextRenderingHint));
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets smoothing (antialiasing) to lines and curves
        /// and the edges of filled areas.
        /// </summary>
        [Description("Specifies whether smoothing (antialiasing) is applied.")]
        [DefaultValue(System.Drawing.Drawing2D.SmoothingMode.Default)]
        public SmoothingMode SmoothingMode
        {
            get 
            { 
                return m_smoothingMode; 
            }
            set
            {
                if (m_smoothingMode != value && OnPropertyChanging(DPN.SmoothingMode, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.SmoothingMode);
                    //// assign new value
                    m_smoothingMode = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.SmoothingMode);
                }
            }
        }

        /// <summary>
        /// Gets or sets text rendering mode.
        /// </summary>
        [Description("Specifies text rendering mode.")]
        [DefaultValue(TextRenderingHint.SystemDefault)]
        public TextRenderingHint TextRenderingHint
        {
            get 
            { 
                return m_textRenderingHint; 
            }
            set
            {
                if (m_textRenderingHint != value && OnPropertyChanging(DPN.TextRenderingHint, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.TextRenderingHint);
                    //// assign new value
                    m_textRenderingHint = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.TextRenderingHint);
                }
            }
        }

        /// <summary>
        /// Gets or sets how data is interpolated between endpoints.
        /// </summary>
        [Description("Specifies how data is interpolated between endpoints.")]
        [DefaultValue(InterpolationMode.Default)]
        public InterpolationMode InterpolationMode
        {
            get 
            { 
                return m_interpolationMode; 
            }
            set
            {
                if (m_interpolationMode != value && OnPropertyChanging(DPN.InterpolationMode, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.InterpolationMode);
                    //// assign new value
                    m_interpolationMode = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.InterpolationMode);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether current instance inherit container measure units.
        /// </summary>
        /// <value>
        /// <c>true</c> if current instance inherit container measure units; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool InheritContainerMeasureUnits
        {
            get { return base.InheritContainerMeasureUnits; }
            set { base.InheritContainerMeasureUnits = value; }
        }

        /// <summary>
        /// Gets or sets the measure unit.
        /// </summary>
        /// <value>The measure unit.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override MeasureUnits MeasureUnit
        {
            get { return base.MeasureUnit; }
            set { base.MeasureUnit = value; }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Applies the rendering style to a Graphics context object.
        /// </summary>
        /// <param name="grfx">Graphics context object to apply rendering style to.</param>
        public virtual void ApplySettings(Graphics grfx)
        {
            if (this.SmoothingMode == SmoothingMode.Invalid)
            {
                grfx.SmoothingMode = SmoothingMode.Default;
            }
            else
            {
                grfx.SmoothingMode = this.SmoothingMode;
            }

            if (this.InterpolationMode == InterpolationMode.Invalid)
            {
                grfx.InterpolationMode = InterpolationMode.Default;
            }
            else
            {
                grfx.InterpolationMode = this.InterpolationMode;
            }

            grfx.TextRenderingHint = this.TextRenderingHint;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>Property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return DPN.RenderingStyle;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new RenderingStyle(this);
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

            info.AddValue("smoothingMode", m_smoothingMode);
            info.AddValue("textRenderingHint", m_textRenderingHint);
            info.AddValue("interpolationMode", m_interpolationMode);
        }
        #endregion
    }
}