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

#region File using directives
using System;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for BackgroundGradient.
    /// </summary>
    public class BackgroundGradient : XDLSSerializableBase
    {
        #region Constants
        /// <summary>
        /// Class constants.
        /// </summary>
        internal const uint DEF_VERTICAL_ANGLE = 4289069056;
        internal const uint DEF_DIAGONALUP_ANGLE = 4286119936;
        internal const uint DEF_DIAGONALDOWN_ANGLE = 4292018176;
        internal const uint DEF_SHADEUP_VARIANT = 100;
        internal const uint DEF_SHADEOUT_VARIANT = 4294967246;
        internal const uint DEF_SHADEMIDDLE_VARIANT = 50;
        #endregion

        #region Fields
        /// <summary>
        /// Class fields.
        /// </summary>
        private BackgroundFillType m_fillType;
        private Color m_fillColor = Color.White;
        private Color m_fillBackColor = Color.White;
        private GradientShadingStyle m_shadingStyle;
        private GradientShadingVariant m_shadingVariant;
        private EscherClass m_escher;
        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets first color for gradient.
        /// </summary>
        public Color Color1
        {
            get
            {
                return m_fillColor;
            }
            set
            {
                m_fillColor = value;
            }
        }
        /// <summary>
        /// Gets/sets second color for gradient
        /// (used when TwoColors set to true).
        /// </summary>
        public Color Color2
        {
            get
            {
                return m_fillBackColor;
            }
            set
            {
                m_fillBackColor = value;
            }
        }
        /// <summary>
        /// Gets/sets shading style for gradient.
        /// </summary>
        public GradientShadingStyle ShadingStyle
        {
            get
            {
                return m_shadingStyle;
            }
            set
            {
                m_shadingStyle = value;
            }
        }
        /// <summary>
        /// Gets/sets shading variants.
        /// </summary>
        public GradientShadingVariant ShadingVariant
        {
            get
            {
                return m_shadingVariant;
            }
            set
            {
                m_shadingVariant = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundGradient"/> class.
        /// </summary>
        public BackgroundGradient()
            : base(null, null)
        {
            m_fillColor = Color.White;
            m_fillBackColor = Color.Black;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundGradient"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="container">The container.</param>
        internal BackgroundGradient(WordDocument doc, MsofbtSpContainer container)
            : base(doc, null)
        {
            m_escher = doc.Escher;
            GetGradientData(container);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Clone current Gradient object.
        /// </summary>
        /// <returns>Exact cope of current Gradient</returns>
        public BackgroundGradient Clone()
        {
            return (BackgroundGradient)base.CloneImpl();
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the gradient data.
        /// </summary>
        private void GetGradientData(MsofbtSpContainer container)
        {
            m_fillType = container.GetBackgroundFillType();
            //Get color 1.
            m_fillColor = container.GetBackgroundColor(false);
            //Get color 2.
            m_fillBackColor = container.GetBackgroundColor(true);
            //Get gradient shading style
            m_shadingStyle = container.GetGradientShadingStyle(m_fillType);
            //Get gradient shading variant
            m_shadingVariant = container.GetGradientShadingVariant(m_shadingStyle);
        }
        #endregion
//#if !SILVERLIGHT
        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (m_fillColor != Color.White)
            {
                writer.WriteValue(XDLSConstants.BackgroundColorAttr, m_fillColor);
            }
            if (m_fillBackColor != Color.White)
            {
                writer.WriteValue(XDLSConstants.BackgroundBackColorAttr, m_fillBackColor);
            }
            if (m_shadingStyle != GradientShadingStyle.Horizontal)
            {
                writer.WriteValue(XDLSConstants.BackgroundGradientStyle, m_shadingStyle);
            }
            if (m_shadingVariant != GradientShadingVariant.ShadingUp)
            {
                writer.WriteValue(XDLSConstants.BackgroundGradientVariant, m_shadingVariant);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);
            if (reader.HasAttribute(XDLSConstants.BackgroundColorAttr))
            {
                m_fillColor = reader.ReadColor(XDLSConstants.BackgroundColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.BackgroundBackColorAttr))
            {
                m_fillBackColor = reader.ReadColor(XDLSConstants.BackgroundBackColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.BackgroundGradientStyle))
            {
                m_shadingStyle = (GradientShadingStyle)reader.ReadEnum(XDLSConstants.BackgroundGradientStyle,
                  typeof(GradientShadingStyle));
            }
            if (reader.HasAttribute(XDLSConstants.BackgroundGradientVariant))
            {
                m_shadingVariant = (GradientShadingVariant)reader.ReadEnum(XDLSConstants.BackgroundGradientVariant,
                  typeof(GradientShadingVariant));
            }
        }
        #endregion
//#endif
    }
}
