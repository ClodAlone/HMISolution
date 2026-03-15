#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;

using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics.Fonts
{
    /// <summary>
    /// Metrics of the font.
    /// </summary>
    internal class PdfFontMetrics : ICloneable
    {
        #region Fields
        /// <summary>
        /// Gets ascent of the font.
        /// </summary>
        public float Ascent;

        /// <summary>
        /// Gets descent of the font.
        /// </summary>
        public float Descent;

        /// <summary>
        /// Name of the font.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets PostScript Name of the  font.
        /// </summary>
        public string PostScriptName;

        /// <summary>
        /// Gets size of the font.
        /// </summary>
        public float Size;

        /// <summary>
        /// Gets height of the font.
        /// </summary>
        public float Height;

        /// <summary>
        /// First char of the font.
        /// </summary>
        public int FirstChar;

        /// <summary>
        /// Last char of the font.
        /// </summary>
        public int LastChar;

        /// <summary>
        /// Line gap.
        /// </summary>
        public int LineGap;

        /// <summary>
        /// Subscript size factor.
        /// </summary>
        public float SubScriptSizeFactor;

        /// <summary>
        /// Superscript size factor.
        /// </summary>
        public float SuperscriptSizeFactor;

        /// <summary>
        /// Gets table of glyphs' width.
        /// </summary>
        private WidthTable m_widthTable;
        #endregion

        #region Public methods
        /// <summary>
        /// Returns ascent taking into consideration font's size.
        /// </summary>
        /// <param name="format">Text format settings.</param>
        /// <returns>Returns ascent taking into consideration font's size.</returns>
        public float GetAscent(PdfStringFormat format)
        {
            return (Ascent * PdfFont.CharSizeMultiplier * GetSize(format));
        }

        /// <summary>
        /// Returns descent taking into consideration font's size.
        /// </summary>
        /// <param name="format">Text format settings.</param>
        /// <returns>Returns descent taking into consideration font's size.</returns>
        public float GetDescent(PdfStringFormat format)
        {
            return (Descent * PdfFont.CharSizeMultiplier * GetSize(format));
        }

        /// <summary>
        /// Returns Line gap taking into consideration font's size.
        /// </summary>
        /// <param name="format">Text format settings.</param>
        /// <returns>Returns line gap taking into consideration font's size.</returns>
        public float GetLineGap(PdfStringFormat format)
        {
            return (LineGap * PdfFont.CharSizeMultiplier * GetSize(format));
        }

        /// <summary>
        /// Returns height taking into consideration font's size.
        /// </summary>
        /// <param name="format">Text format settings.</param>
        /// <returns>Returns height taking into consideration font's size.</returns>
        public float GetHeight(PdfStringFormat format)
        {
            float height;
            if (GetDescent(format) < 0)
            {
                height = (GetAscent(format) - GetDescent(format) + GetLineGap(format));
            }
            else
            {
                height = (GetAscent(format) + GetDescent(format) + GetLineGap(format));
            }

            return height;
        }

        /// <summary>
        /// Calculates size of the font depending on the subscript/superscript value.
        /// </summary>
        /// <param name="format">Text format settings.</param>
        /// <returns>Size of the font depending on the subscript/superscript value.</returns>
        public float GetSize(PdfStringFormat format)
        {
            float size = Size;

            if (format != null)
            {
                switch (format.SubSuperScript)
                {
                    case PdfSubSuperScript.SubScript:
                        size /= SubScriptSizeFactor;
                        break;

                    case PdfSubSuperScript.SuperScript:
                        size /= SuperscriptSizeFactor;
                        break;
                }
            }

            return size;
        }

        /// <summary>
        /// Clones the metrics.
        /// </summary>
        /// <returns>Cloned metrics.</returns>
        public object Clone()
        {
            PdfFontMetrics metrics = (PdfFontMetrics)MemberwiseClone();
            metrics.WidthTable = WidthTable.Clone() as WidthTable;

            return metrics;
        }
        #endregion

        #region Properies
        /// <summary>
        /// Gets or sets the width table.
        /// </summary>
        public WidthTable WidthTable
        {
            get
            {
                return m_widthTable;
            }

            set
            {
                m_widthTable = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// The base class for a width table.
    /// </summary>
    internal abstract class WidthTable : ICloneable
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="System.Int32"/> at the specified index.
        /// </summary>
        /// <value>index</value>
        public abstract int this[int index] { get; }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones this instance of the WidthTable class.
        /// </summary>
        /// <returns>A copy of this WidthTable instance.</returns>
        public abstract WidthTable Clone();

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Toes the array.
        /// </summary>
        /// <returns></returns>
        internal abstract PdfArray ToArray();
        #endregion
    }

    /// <summary>
    /// Implements a width table for standard fonts.
    /// </summary>
    internal class StandardWidthTable : WidthTable
    {
        #region Fields
        /// <summary>
        /// The widths of the supported characters.
        /// </summary>
        private int[] m_widths;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="System.Int32"/> at the specified index.
        /// </summary>
        /// <value>index</value>
        public override int this[int index]
        {
            get
            {
                if (index < 0 || index >= m_widths.Length)
                {
                    throw new ArgumentOutOfRangeException("index", "The character is not supported by the font.");
                }

                int result = m_widths[index];

                return result;
            }
        }

        /// <summary>
        /// Gets the length of the internal array.
        /// </summary>
        public int Length
        {
            get
            {
                return m_widths.Length;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:StandardWidthTable"/> class.
        /// </summary>
        /// <param name="widths">The widths table.</param>
        internal StandardWidthTable(int[] widths)
        {
            if (widths == null)
            {
                throw new ArgumentNullException("widths");
            }

            m_widths = widths;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Clones this instance of the WidthTable class.
        /// </summary>
        /// <returns>A copy of this WidthTable instance.</returns>
        public override WidthTable Clone()
        {
            StandardWidthTable swt = MemberwiseClone() as StandardWidthTable;

            swt.m_widths = (int[])m_widths.Clone();

            return swt;
        }

        /// <summary>
        /// Converts width table to a PDF array.
        /// </summary>
        /// <returns>The properly formed pdf array.</returns>
        internal override PdfArray ToArray()
        {
            PdfArray arr = new PdfArray(m_widths);

            return arr;
        }
        #endregion
    }

    /// <summary>
    /// Implements CJK width table, which is quite complex.
    /// </summary>
    internal class CjkWidthTable : WidthTable
    {
        #region Fields
        /// <summary>
        /// Local variable to store the width.
        /// </summary>
        private List<CjkWidth> m_width;

        /// <summary>
        /// Local variable to store the default width.
        /// </summary>
        private int m_defaultWidth;
        #endregion

        #region Constuctors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CjkWidthTable"/> class.
        /// </summary>
        /// <param name="defaultWidth">The default width of the CJK characters.
        /// This value will be returned if there is no width information for a character.</param>
        public CjkWidthTable(int defaultWidth)
        {
            m_width = new List<CjkWidth>();
            m_defaultWidth = defaultWidth;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default character width.
        /// </summary>
        public int DefaultWidth
        {
            get
            {
                return m_defaultWidth;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Int32"/> at the specified index.
        /// </summary>
        /// <value>index</value>
        public override int this[int index]
        {
            get
            {
                int width = DefaultWidth;

                foreach (CjkWidth widths in m_width)
                {
                    if (index >= widths.From && index <= widths.To)
                    {
                        width = widths[index];
                    }
                }

                return width;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified widths.
        /// </summary>
        /// <param name="widths">The CJK widths.</param>
        public void Add(CjkWidth widths)
        {
            if (widths == null)
            {
                throw new ArgumentNullException("widths");
            }

            m_width.Add(widths);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Clones this instance of the WidthTable class.
        /// </summary>
        /// <returns>A copy of this WidthTable instance.</returns>
        public override WidthTable Clone()
        {
            CjkWidthTable wt = MemberwiseClone() as CjkWidthTable;

            wt.m_width = new List<CjkWidth>(m_width.Count);

            foreach (CjkWidth width in m_width)
            {
                wt.m_width.Add(width.Clone());
            }

            return wt;
        }

        /// <summary>
        /// Converts width table to a PDF array.
        /// </summary>
        /// <returns>A well formed PDF array.</returns>
        internal override PdfArray ToArray()
        {
            PdfArray arr = new PdfArray();

            foreach (CjkWidth width in m_width)
            {
                width.AppendToArray(arr);
            }

            return arr;
        }
        #endregion
    }

    /// <summary>
    /// The base class of CJK widths types.
    /// </summary>
    internal abstract class CjkWidth : ICloneable
    {
        #region Properties
        /// <summary>
        /// Gets the starting character.
        /// </summary>
        internal abstract int From { get; }

        /// <summary>
        /// Gets the ending character.
        /// </summary>
        internal abstract int To { get; }

        /// <summary>
        /// Gets the width of the specified character.
        /// </summary>
        internal abstract int this[int index] { get; }
        #endregion

        /// <summary>
        /// Appends internal data to a PDF array.
        /// </summary>
        /// <param name="arr">The pdf array.</param>
        internal abstract void AppendToArray(PdfArray arr);

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The proper copy of this instance.</returns>
        internal abstract CjkWidth Clone();
        #endregion
    }

    /// <summary>
    /// Implements capabilities to control a range of character with the same width.
    /// </summary>
    internal class CjkSameWidth : CjkWidth
    {
        #region Fields
        /// <summary>
        /// The Form
        /// </summary>
        private int m_from;

        /// <summary>
        /// The to
        /// </summary>
        private int m_to;

        /// <summary>
        /// The Width
        /// </summary>
        private int m_width;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the starting character.
        /// </summary>
        internal override int From
        {
            get
            {
                return m_from;
            }
        }

        /// <summary>
        /// Gets the ending character.
        /// </summary>
        internal override int To
        {
            get
            {
                return m_to;
            }
        }

        /// <summary>
        /// Gets the width of the specified character.
        /// </summary>
        internal override int this[int index]
        {
            get
            {
                if (index < From || index > To)
                {
                    throw new ArgumentOutOfRangeException("index", "Index is out of range.");
                }

                return m_width;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CjkSameWidth"/> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="width">The width.</param>
        public CjkSameWidth(int from, int to, int width)
        {
            if (from > to)
            {
                throw new ArgumentException("'From' can't be grater than 'to'.");
            }

            m_from = from;
            m_to = to;
            m_width = width;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Appends internal data to a PDF array.
        /// </summary>
        /// <param name="arr">The pdf array.</param>
        internal override void AppendToArray(PdfArray arr)
        {
            arr.Add(new PdfNumber(From));
            arr.Add(new PdfNumber(To));
            arr.Add(new PdfNumber(m_width));
        }
        #endregion

        #region IClonable interface
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The proper copy of this instance.</returns>
        internal override CjkWidth Clone()
        {
            CjkWidth w = MemberwiseClone() as CjkWidth;

            return w;
        }
        #endregion
    }

    /// <summary>
    /// Implements capabilities to control a sequent range of characters with different width.
    /// </summary>
    internal class CjkDifferentWidth : CjkWidth
    {
        #region Fields
        /// <summary>
        /// The form
        /// </summary>
        private int m_from;

        /// <summary>
        /// The width
        /// </summary>
        private int[] m_width;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the starting character.
        /// </summary>
        internal override int From
        {
            get
            {
                return m_from;
            }
        }

        /// <summary>
        /// Gets the ending character.
        /// </summary>
        internal override int To
        {
            get
            {
                int to = From + m_width.Length - 1;

                return to;
            }
        }

        /// <summary>
        /// Gets the width of the specified character.
        /// </summary>
        internal override int this[int index]
        {
            get
            {
                if (index < From || index > To)
                {
                    throw new ArgumentOutOfRangeException("index", "Index is out of range.");
                }

                int width = m_width[index - From];

                return width;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CjkDifferentWidth"/> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="widths">The widths.</param>
        public CjkDifferentWidth(int from, int[] widths)
        {
            if (widths == null)
            {
                throw new ArgumentNullException("widths");
            }

            m_from = from;
            m_width = widths;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Appends internal data to a PDF array.
        /// </summary>
        /// <param name="arr">The pdf array.</param>
        internal override void AppendToArray(PdfArray arr)
        {
            arr.Add(new PdfNumber(From));

            PdfArray widths = new PdfArray(m_width);

            arr.Add(widths);
        }
        #endregion

        #region IClonable interface
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The proper copy of this instance.</returns>
        internal override CjkWidth Clone()
        {
            CjkDifferentWidth dw = MemberwiseClone() as CjkDifferentWidth;

            dw.m_width = (int[])m_width.Clone();

            return dw;
        }
        #endregion
    }
}
