#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
#if WINRT || WP
#else
using System.Drawing;
#endif
using System.Text;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;

namespace Syncfusion.DocIO.DLS
{
    public class FillFormat
    {
        private Color m_BackColor;
        private Color m_ForeColor;
        //private float m_GradientAngle;
        //private GradientColorType m_GradientColorType;
        //private float m_GradientDegree;
        //private List<GradientStops> m_GradientStops;
        //private GradientStyle m_GradientStyle;
        //Returns the gradient variant for the specified fill as an integer value from 1 to 4 for most gradient fills. Read-only Long.
        //private long m_GradientVariant;
        //private PresetGradientType m_PresetGradientType;
        //private PictureEffects m_PictureEffects;

        private PatternType m_Pattern = PatternType.Mixed;
        //private PresetTexture m_PresetTexture;
        private bool m_RotateWithObject;
        private TextureAlignment m_TextureAlignment;
        private double m_TextureHorizontalScale;
        //private string m_TextureName;
        private double m_TextureOffsetX;
        private double m_TextureOffsetY;
        private bool m_TextureTile;
        //private TextureType m_TextureType;
        private double m_TextureVerticalScale;
        private float m_Transparency;
        private FillType m_FillType;
        //private bool m_Visible;
        private bool m_Fill;
        private ImageRecord m_ImageRecord;
        private FlipOrientation m_FlipOrientation;
        private TileRectangle m_SourceRectangle;
        private TileRectangle m_FillRectangle;
        private GradientFill m_GradientFill;
        internal GradientFill GradientFill
        {
            get
            {
                if (m_GradientFill == null)
                    m_GradientFill = new GradientFill();
                return m_GradientFill;
            }
            set
            {
                m_GradientFill = value;
            }
        }
        
        internal TileRectangle FillRectangle
        {
            get
            {
                if (m_FillRectangle == null)
                    m_FillRectangle = new TileRectangle();
                return m_FillRectangle;
            }
            set { m_FillRectangle = value; }
        }
        internal TileRectangle SourceRectangle
        {
            get
            {
                if (m_SourceRectangle == null)
                    m_SourceRectangle = new TileRectangle();
                return m_SourceRectangle;
            }
            set { m_SourceRectangle = value; }
        }
        internal FlipOrientation FlipOrientation
        {
            get { return m_FlipOrientation; }
            set { m_FlipOrientation = value; }
        }
        internal ImageRecord ImageRecord
        {
            get { return m_ImageRecord; }
            set { m_ImageRecord = value; }
        }
        public bool Fill
        {
            get { return m_Fill; }
            set { m_Fill = value; FillFormatChanged(); }
        }
        /// <summary>
        ///
        /// </summary>
        public Color Color
        {
            // Represents Back color
            get { return m_BackColor; }
            set { m_BackColor = value; FillFormatChanged(); }
        }
        
        internal Color ForeColor
        {
            get { return m_ForeColor; }
            set { m_ForeColor = value; }
        }
        //internal float GradientAngle
        //{
        //    get { return m_GradientAngle; }
        //    set { m_GradientAngle = value; }
        //}
        //internal GradientColorType GradientColorType
        //{
        //    get { return m_GradientColorType; }
        //    set { m_GradientColorType = value; }
        //}
        //internal float GradientDegree
        //{
        //    get { return m_GradientDegree; }
        //    set { m_GradientDegree = value; }
        //}
        //internal List<GradientStops> GradientStops
        //{
        //    get { return m_GradientStops; }
        //    set { m_GradientStops = value; }
        //}
        //internal GradientStyle GradientStyle
        //{
        //    get { return m_GradientStyle; }
        //    set { m_GradientStyle = value; }
        //}
        ////Returns the gradient variant for the specified fill as an integer value from 1 to 4 for most gradient fills. Read-only Long.
        //internal long GradientVariant
        //{
        //    get { return m_GradientVariant; }
        //    set { m_GradientVariant = value; }
        //}
        //internal PresetGradientType PresetGradientType
        //{
        //    get { return m_PresetGradientType; }
        //    set { m_PresetGradientType = value; }
        //}
        //internal PictureEffects PictureEffects
        //{
        //    get { return m_PictureEffects; }
        //    set { m_PictureEffects = value; }
        //}
        internal PatternType Pattern
        {
            get { return m_Pattern; }
            set { m_Pattern = value; }
        }
        //internal PresetTexture PresetTexture
        //{
        //    get { return m_PresetTexture; }
        //    set { m_PresetTexture = value; }
        //}
        internal bool RotateWithObject
        {
            get { return m_RotateWithObject; }
            set { m_RotateWithObject = value; }
        }
        internal TextureAlignment TextureAlignment
        {
            get { return m_TextureAlignment; }
            set { m_TextureAlignment = value; }
        }
        internal double TextureHorizontalScale
        {
            get { return m_TextureHorizontalScale; }
            set { m_TextureHorizontalScale = value; }
        }
        //internal string TextureName
        //{
        //    get { return m_TextureName; }
        //    set { m_TextureName = value; }
        //}
        internal double TextureOffsetX
        {
            get { return m_TextureOffsetX; }
            set { m_TextureOffsetX = value; }
        }
        internal double TextureOffsetY
        {
            get { return m_TextureOffsetY; }
            set { m_TextureOffsetY = value; }
        }
        internal bool TextureTile
        {
            get { return m_TextureTile; }
            set { m_TextureTile = value; }
        }
        //internal TextureType TextureType
        //{
        //    get { return m_TextureType; }
        //    set { m_TextureType = value; }
        //}
        internal double TextureVerticalScale
        {
            get { return m_TextureVerticalScale; }
            set { m_TextureVerticalScale = value; }
        }
        public float Transparency
        {
            get { return m_Transparency; }
            set { m_Transparency = value; FillFormatChanged(); }
        }
        internal FillType FillType
        {
            get { return m_FillType; }
            set { m_FillType = value; }
        }
        //internal bool Visible
        //{
        //    get { return m_Visible; }
        //    set { m_Visible = value; }
        //}
        private Shape m_shape;
        public FillFormat(Shape shape)
        {
            m_shape = shape;
            m_Fill = true;
            m_FillType = FillType.FillSolid;
            m_BackColor = Color.White;
            FillFormatChanged();
        }
        private void FillFormatChanged()
        {
            if (m_shape.DocxProps.ContainsKey("gradFill"))
                m_shape.DocxProps.Remove("gradFill");
            if (m_shape.DocxProps.ContainsKey("blipFill"))
                m_shape.DocxProps.Remove("blipFill");
            if (m_shape.DocxProps.ContainsKey("pattFill"))
                m_shape.DocxProps.Remove("pattFill");
            if (m_shape.Docx2007Props.ContainsKey("fill"))
                m_shape.Docx2007Props.Remove("fill");
        }
    }
}
