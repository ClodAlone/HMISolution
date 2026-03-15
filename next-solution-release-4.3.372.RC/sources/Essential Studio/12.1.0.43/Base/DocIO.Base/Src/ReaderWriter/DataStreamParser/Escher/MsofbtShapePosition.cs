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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for MsofbtSecondaryFOPT - OfficeArtSecondaryFOPT.
    /// </summary>
    internal class MsofbtSecondaryFOPT : BaseEscherRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private msofbtRGFOPTE m_prop;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        internal msofbtRGFOPTE Properties
        {
            get
            {
                return m_prop;
            }
            set
            {
                m_prop = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MsofbtSecondaryFOPT"/> class.
        /// </summary>
        internal MsofbtSecondaryFOPT(WordDocument doc)
            : base(MSOFBT.msofbtSecondaryFOPT, 3, doc)
        {
            m_prop = new msofbtRGFOPTE();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            m_prop.Clear();
            m_prop.Read(stream, Header.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            Header.Instance = CountInstanceValue();
            m_prop.Write(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtSecondaryFOPT options = new MsofbtSecondaryFOPT(m_doc);
            FOPTEBase fopteBase = null;
            foreach (Object obj in m_prop.Values)
            {
                fopteBase = (FOPTEBase)obj;
                options.m_prop.Add(fopteBase);
            }
            options.m_doc = m_doc;
            return options;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();

            if (m_prop != null)
            {
                m_prop.Clear();
                m_prop = null;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Get uint property value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public uint GetPropertyValue(int key)
        {
            if (this.Properties.ContainsKey(key))
            {
                FOPTEBid fbValue = this.Properties[key] as FOPTEBid;
                if (fbValue != null)
                    return fbValue.Value;
            }
            return uint.MaxValue;
        }
        /// <summary>
        /// Get complex value by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] GetComplexPropValue(int key)
        {
            if (this.Properties.ContainsKey(key))
            {
                FOPTEComplex complexVal = (FOPTEComplex)this.Properties[key];
                return complexVal.Value;
            }
            return null;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Counts the instance value.
        /// </summary>
        /// <returns></returns>
        private int CountInstanceValue()
        {
            int retValue = 0;
            foreach (Object obj in m_prop.Values)
            {
                FOPTEBase fopte = obj as FOPTEBase;
                if (fopte.Id < 10000)
                {
                    retValue += 1;
                }
            }
            return retValue;
        }

        #endregion
    }
    /// <summary>
    /// Summary description for MsofbtTertiaryFOPT - OfficeArtTertiaryFOPT.
    /// </summary>
    internal class MsofbtTertiaryFOPT : BaseEscherRecord
    {
        #region Class constants
        private const int DEF_UNKNOWN2_PID = 0x053F;
        private const uint DEF_NOTALLOWINCELL = 2147483648;
        #endregion

        #region Class members
        private msofbtRGFOPTE m_prop;
        private LineStyleBooleanProperties m_lineProps;
        private WrapPolygonVertices m_wrapPolygonVetrices;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the line properties.
        /// </summary>
        /// <value>The line properties.</value>
        internal LineStyleBooleanProperties LineProperties
        {
            get
            {
                if (m_lineProps == null)
                    m_lineProps = new LineStyleBooleanProperties(m_prop, (int)FOPTELineStyle.lineStyleBooleanProperties);
                return m_lineProps;
            }
        }
        /// <summary>
        /// Gets the wrap polygon vertices.
        /// </summary>
        /// <value>
        /// The wrap polygon vertices.
        /// </value>
        internal WrapPolygonVertices WrapPolygonVertices
        {
            get
            {
                if (m_wrapPolygonVetrices == null)
                    m_wrapPolygonVetrices = new WrapPolygonVertices(m_prop, (int)FOPTEGroupShape.pWrapPolygonVertices);
                return m_wrapPolygonVetrices;

            }
        }
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        internal msofbtRGFOPTE Properties
        {
            get
            {
                return m_prop;
            }
            set
            {
                m_prop = value;
            }
        }
        /// <summary>
        /// Gets or sets the X align.
        /// </summary>
        /// <value>The X align.</value>
        public uint XAlign
        {
            get
            {
                return GetPropertyValue((int)FOPTEGroupShape.posh);
            }
            set
            {
                SetPropertyValue((int)FOPTEGroupShape.posh, value);
            }
        }
        /// <summary>
        /// Gets or sets the X rel to.
        /// </summary>
        /// <value>The X rel to.</value>
        public uint XRelTo
        {
            get
            {
                return GetPropertyValue((int)FOPTEGroupShape.posrelh);
            }
            set
            {
                SetPropertyValue((int)FOPTEGroupShape.posrelh, value);
            }
        }
        /// <summary>
        /// Gets or sets the Y align.
        /// </summary>
        /// <value>The Y align.</value>
        public uint YAlign
        {
            get
            {
                return GetPropertyValue((int)FOPTEGroupShape.posv);
            }
            set
            {
                SetPropertyValue((int)FOPTEGroupShape.posv, value);
            }
        }
        /// <summary>
        /// Gets or sets the Y rel to.
        /// </summary>
        /// <value>The Y rel to.</value>
        public uint YRelTo
        {
            get
            {
                return GetPropertyValue((int)FOPTEGroupShape.posrelv);
            }
            set
            {
                SetPropertyValue((int)FOPTEGroupShape.posrelv, value);
            }
        }
        /// <summary>
        /// Gets or sets the layout in table cell.
        /// </summary>
        /// <value>The layout in table cell.</value>
        public uint LayoutInTableCell
        {
            get
            {
                return GetPropertyValue((int)FOPTEGroupShape.fPrint);
            }
            set
            {
                SetPropertyValue((int)FOPTEGroupShape.fPrint, value);
            }
        }
        /// <summary>
        /// Gets or sets the unknown1.
        /// </summary>
        /// <value>The unknown1.</value>
        public uint Unknown1
        {
            get
            {
                return GetPropertyValue((int)FOPTEFillStyle.fNoFillHitTest);
            }
            set
            {
                SetPropertyValue((int)FOPTEFillStyle.fNoFillHitTest, value);
            }
        }
        /// <summary>
        /// Gets or sets the unknown2.
        /// </summary>
        /// <value>The unknown2.</value>
        public uint Unknown2
        {
            get
            {
                return GetPropertyValue((int)DEF_UNKNOWN2_PID);
            }
            set
            {
                SetPropertyValue((int)DEF_UNKNOWN2_PID, value);
            }
        }
        /// <summary>
        /// Gets/sets a value indicating whether allow in table cell.
        /// </summary>
        /// <value><c>true</c> if allow in table cell; otherwise, <c>false</c>.</value>
        internal bool AllowInTableCell
        {
            get
            {
                if (LayoutInTableCell != uint.MaxValue)
                {
                    bool useLayoutInCell = ((LayoutInTableCell & 0x80000000) >> 31) != 0;
                    if (useLayoutInCell)
                        return ((LayoutInTableCell & 0x8000) >> 15) != 0;
                }
                return true;
            }
            set
            {
                if (LayoutInTableCell == uint.MaxValue)
                    LayoutInTableCell = DEF_NOTALLOWINCELL;
                //Set fUsefLayoutInCell as true
                LayoutInTableCell = (uint)((LayoutInTableCell & 0x7FFFFFFF) | (1 << 31));
                //Set fLayoutInCell
                LayoutInTableCell = (uint)((LayoutInTableCell & 0xFFFF7FFF) | (value ? 1 : 0 << 15));
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MsofbtTertiaryFOPT"/> class.
        /// </summary>
        internal MsofbtTertiaryFOPT(WordDocument doc)
            : base(MSOFBT.msofbtTertiaryFOPT, 3, doc)
        {
            m_prop = new msofbtRGFOPTE();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            m_prop.Clear();
            m_prop.Read(stream, Header.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            Header.Instance = CountInstanceValue();
            m_prop.Write(stream);
        }
        /// <summary>
        /// Clone current record.
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtTertiaryFOPT options = new MsofbtTertiaryFOPT(m_doc);
            FOPTEBase fopteBase = null;
            foreach (Object obj in m_prop.Values)
            {
                fopteBase = (FOPTEBase)obj;
                options.m_prop.Add(fopteBase);
            }
            options.m_doc = m_doc;
            return options;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();

            if (m_prop != null)
            {
                m_prop.Clear();
                m_prop = null;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Get uint property value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal uint GetPropertyValue(int key)
        {
            if (m_prop.ContainsKey(key))
            {
                FOPTEBid fbValue = m_prop[key] as FOPTEBid;
                if (fbValue != null)
                    return fbValue.Value;
            }
            return uint.MaxValue;
        }
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        internal void SetPropertyValue(int key, uint value)
        {
            if (m_prop.ContainsKey(key))
                (m_prop[key] as FOPTEBid).Value = value;
            else
                m_prop.Add(key, new FOPTEBid(key, false, value));
        }
        /// <summary>
        /// Get complex value by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal byte[] GetComplexPropValue(int key)
        {
            if (this.Properties.ContainsKey(key))
            {
                FOPTEComplex complexVal = (FOPTEComplex)this.Properties[key];
                return complexVal.Value;
            }
            return null;
        }
        /// <summary>
        /// Counts the instance value.
        /// </summary>
        /// <returns></returns>
        private int CountInstanceValue()
        {
            int retValue = 0;
            foreach (Object obj in m_prop.Values)
            {
                FOPTEBase fopte = obj as FOPTEBase;
                //Handled specifically for duplicate properties.
                if (fopte.Id < 10000)
                {
                    retValue += 1;
                }
            }
            return retValue;
        }
        #endregion
    }
    /// <summary>
    /// Summary description for LineStyleBooleanProperties
    /// </summary>
    internal class LineStyleBooleanProperties
    {
        #region Constants
        internal const uint DefaultValue = 46;
        #endregion

        #region Class members
        private int m_key;
        private msofbtRGFOPTE m_prop;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets a value indicating whether this instance has defined.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has defined; otherwise, <c>false</c>.
        /// </value>
        internal bool HasDefined
        {
            get
            {
                return m_prop.ContainsKey(m_key);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [pen align inset].
        /// </summary>
        /// <value><c>true</c> if [pen align inset]; otherwise, <c>false</c>.</value>
        internal bool PenAlignInset
        {
            get
            {
                if (UsefInsetPenOK
                    && InsetPenOK
                    && UsefInsetPen)
                    return InsetPen;
                return false;
            }
            set
            {
                UsefInsetPenOK = InsetPenOK = UsefInsetPen = InsetPen = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [usef line opaque back color].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [usef line opaque back color]; otherwise, <c>false</c>.
        /// </value>
        internal bool UsefLineOpaqueBackColor
        { 
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x2000000) >> 25) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFDFFFFFF) | ((value ? 1 : 0) << 25));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [usef inset pen].
        /// </summary>
        /// <value><c>true</c> if [usef inset pen]; otherwise, <c>false</c>.</value>
        internal bool UsefInsetPen
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x400000) >> 22) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFBFFFFF) | ((value ? 1 : 0) << 22));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [usef inset pen OK].
        /// </summary>
        /// <value><c>true</c> if [usef inset pen OK]; otherwise, <c>false</c>.</value>
        internal bool UsefInsetPenOK
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x200000) >> 21) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFDFFFFF) | ((value ? 1 : 0) << 21));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [usef arrowheads OK].
        /// </summary>
        /// <value><c>true</c> if [usef arrowheads OK]; otherwise, <c>false</c>.</value>
        internal bool UsefArrowheadsOK
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x100000) >> 20) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFEFFFFF) | ((value ? 1 : 0) << 20));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [usef line].
        /// </summary>
        /// <value><c>true</c> if [usef line]; otherwise, <c>false</c>.</value>
        internal bool UsefLine
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x80000) >> 19) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFF7FFFF) | ((value ? 1 : 0) << 19));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [usef hit test line].
        /// </summary>
        /// <value><c>true</c> if [usef hit test line]; otherwise, <c>false</c>.</value>
        internal bool UsefHitTestLine
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x40000) >> 18) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFBFFFF) | ((value ? 1 : 0) << 18));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [usef line fill shape].
        /// </summary>
        /// <value><c>true</c> if [usef line fill shape]; otherwise, <c>false</c>.</value>
        internal bool UsefLineFillShape
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x20000) >> 17) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFDFFFF) | ((value ? 1 : 0) << 17));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [usef no line draw dash].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [usef no line draw dash]; otherwise, <c>false</c>.
        /// </value>
        internal bool UsefNoLineDrawDash
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x10000) >> 16) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFEFFFF) | ((value ? 1 : 0) << 16));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [line opaque back color].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [line opaque back color]; otherwise, <c>false</c>.
        /// </value>
        internal bool LineOpaqueBackColor
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x200) >> 9) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFFFDFF) | ((value ? 1 : 0) << 9));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [inset pen].
        /// </summary>
        /// <value><c>true</c> if [inset pen]; otherwise, <c>false</c>.</value>
        internal bool InsetPen
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x40) >> 6) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFFFFBF) | ((value ? 1 : 0) << 6));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [inset pen OK].
        /// </summary>
        /// <value><c>true</c> if [inset pen OK]; otherwise, <c>false</c>.</value>
        internal bool InsetPenOK
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x20) >> 5) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFFFFDF) | ((value ? 1 : 0) << 5));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [arrowheads OK].
        /// </summary>
        /// <value><c>true</c> if [arrowheads OK]; otherwise, <c>false</c>.</value>
        internal bool ArrowheadsOK
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x10) >> 4) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFFFFEF) | ((value ? 1 : 0) << 4));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LineStyleBooleanProperties"/> is line.
        /// </summary>
        /// <value><c>true</c> if line; otherwise, <c>false</c>.</value>
        internal bool Line
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x8) >> 3) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFFFFF7) | ((value ? 1 : 0) << 3));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [hit test line].
        /// </summary>
        /// <value><c>true</c> if [hit test line]; otherwise, <c>false</c>.</value>
        internal bool HitTestLine
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x4) >> 2) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFFFFFB) | ((value ? 1 : 0) << 2));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [line fill shape].
        /// </summary>
        /// <value><c>true</c> if [line fill shape]; otherwise, <c>false</c>.</value>
        internal bool LineFillShape
        {
            get
            {
                if (!HasDefined)
                    return false;
                return (((m_prop[m_key] as FOPTEBid).Value & 0x2) >> 1) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFFFFFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [no line draw dash].
        /// </summary>
        /// <value><c>true</c> if [no line draw dash]; otherwise, <c>false</c>.</value>
        internal bool NoLineDrawDash
        {
            get
            {
                if (!HasDefined)
                    return false;
                return ((m_prop[m_key] as FOPTEBid).Value & 0x1) != 0;
            }
            set
            {
                if (HasDefined)
                    (m_prop[m_key] as FOPTEBid).Value = (uint)(((m_prop[m_key] as FOPTEBid).Value & 0xFFFFFFFE) | (value ? 1 : 0));
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LineStyleBooleanProperties"/> class.
        /// </summary>
        /// <param name="prop">The prop.</param>
        internal LineStyleBooleanProperties(msofbtRGFOPTE prop, int key)
        {
            m_prop = prop;
            m_key = key;
        }
        #endregion
    }

    /// <summary>
    /// Wrap Polygon Vertices class
    /// </summary>
    internal class WrapPolygonVertices
    {
        #region Class members
        private int m_key;
        private msofbtRGFOPTE m_prop;
        private List<PointF> m_coords;
        private uint m_nelems;
        private uint m_nelemsalloc;
        private uint m_cbelem;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LineStyleBooleanProperties"/> class.
        /// </summary>
        /// <param name="prop">The prop.</param>
        internal WrapPolygonVertices(msofbtRGFOPTE prop, int key)
        {
            m_prop = prop;
            m_key = key;
            m_coords = new List<PointF>();
            readArrayData();
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets or sets the coords.
        /// </summary>
        /// <value>
        /// The coords.
        /// </value>
        internal List<PointF> Coords
        {
            get
            {
                return m_coords;
            }
            set
            {
                m_coords = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of array elements that are contained in this record.
        /// </summary>
        /// <value>
        /// The n elems.
        /// </value>
        internal uint nElems
        {
            get { return m_nelems; }
            set { m_nelems = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of array elements that this record can contain.
        /// </summary>
        /// <value>
        /// The n elems alloc.
        /// </value>
        internal uint nElemsAlloc
        {
            get { return m_nelemsalloc; }
            set { m_nelemsalloc = value; }
        }

        /// <summary>
        /// Gets or sets the size, in bytes, of each element in the data array.
        /// </summary>
        /// <value>
        /// The cb elem.
        /// </value>
        internal uint cbElem
        {
            get { return m_cbelem; }
            set { m_cbelem = value; }
        }
        #endregion

        /// <summary>
        /// Reads the array data.
        /// </summary>
        private void readArrayData()
        {
            MemoryStream stream = new MemoryStream((m_prop[m_key] as FOPTEComplex).Value);
            StreamReader dataReader = new StreamReader(stream);

            byte[] temp = new byte[2];
            stream.Read(temp, 0, 2);
            m_nelems = BitConverter.ToUInt16(temp, 0);

            stream.Read(temp, 0, 2);
            m_nelemsalloc = BitConverter.ToUInt16(temp, 0);

            stream.Read(temp, 0, 2);
            m_cbelem = BitConverter.ToUInt16(temp, 0);

            for (int i = 0; i < (int)m_nelems; i++)
            {
                temp = new byte[(int)m_cbelem];
                stream.Read(temp, 0, (int)m_cbelem);
                int x = BitConverter.ToInt32(temp, 0);
                int y = BitConverter.ToInt32(temp, 4);
                m_coords.Add(new PointF(x, y));
            }
        }
    }
}
