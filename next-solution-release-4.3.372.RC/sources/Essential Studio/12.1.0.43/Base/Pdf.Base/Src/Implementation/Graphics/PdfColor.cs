#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

#if SILVERLIGHT
using System.Windows.Media;
using System.Collections.Generic;
#else

#endif

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Implements structures and routines working with color.
    /// </summary>
    public struct PdfColor
    {
        #region Static members
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Holds RGB colors converted into strings.
        /// </summary>
        private static Dictionary<int, object> s_rgbStrings = new Dictionary<int, object>();
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Holds Grayscale colors converted into strings for stroking.
        /// </summary>
        private static Dictionary<float, object> s_grayStringsSroke = new Dictionary<float, object>();
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Holds Grayscale colors converted into strings for filling.
        /// </summary>
        private static Dictionary<float, object> s_grayStringsFill = new Dictionary<float, object>();
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Holds the system-wide empty PDF color.
        /// </summary>
        private static PdfColor s_emptyColor = new PdfColor();
        #endregion

        #region Constants
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Max value of color channel.
        /// </summary>
        private const float MaxColourChannelValue = 255.0f;
        #endregion

        #region Fields
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value of Red channel.
        /// </summary>
        private byte m_red;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value of Cyan channel.
        /// </summary>
        private float m_cyan;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value of Green channel.
        /// </summary>
        private byte m_green;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value of Magenta channel.
        /// </summary>
        private float m_magenta;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value of Blue channel.
        /// </summary>
        private byte m_blue;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value of Yellow channel.
        /// </summary>
        private float m_yellow;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value of Black channel.
        /// </summary>
        private float m_black;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value of Gray channel.
        /// </summary>
        private float m_gray;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Alpha channel.
        /// </summary>
        private byte m_alpha;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Shows if the color is empty.
        /// </summary>
        private bool m_isFilled;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a null color.
        /// </summary>
        /// <value>The empty.</value>
        /// <property name="flag" value="Finished"/>
        public static PdfColor Empty
        {
            get
            {
                return s_emptyColor;
            }
        }

        /// <summary>
        /// Gets whether the PDFColor is Empty or not.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        /// <property name="flag" value="Finished"/>
        public bool IsEmpty
        {
            get
            {
                return !m_isFilled;
            }
        }

        /// <summary>
        /// Gets or sets Blue channel value.
        /// </summary>
        /// <value>The B.</value>
        /// <property name="flag" value="Finished"/>
        public byte B
        {
            get
            {
                return m_blue;
            }
            set
            {
                m_blue = value;
                AssignCMYK(m_red, m_green, m_blue);
                m_isFilled = true;
            }
        }

        /// <summary>
        /// Gets the blue.
        /// </summary>
        public float Blue
        {
            get
            {
                return B / MaxColourChannelValue;
            }
        }

        /// <summary>
        /// Gets or sets Cyan channel value.
        /// </summary>
        /// <value>The C.</value>
        /// <property name="flag" value="Finished"/>
        public float C
        {
            get
            {
                return m_cyan;
            }
            set
            {
                if (value < 0f)
                {
                    m_cyan = 0f;
                }
                else if (value > 1f)
                {
                    m_cyan = 1f;
                }
                else
                {
                    m_cyan = value;
                }
                AssignRGB(m_cyan, m_magenta, m_yellow, m_black);
                m_isFilled = true;
            }
        }

        /// <summary>
        /// Gets or sets Green channel value.
        /// </summary>
        /// <value>The G.</value>
        /// <property name="flag" value="Finished"/>
        public byte G
        {
            get
            {
                return m_green;
            }
            set
            {
                m_green = value;
                AssignCMYK(m_red, m_green, m_blue);
                m_isFilled = true;
            }
        }

        /// <summary>
        /// Gets the green.
        /// </summary>
        /// <value>The green.</value>
        public float Green
        {
            get
            {
                return G / MaxColourChannelValue;
            }
        }

        /// <summary>
        /// Gets or sets Gray channel value.
        /// </summary>
        /// <value>The gray.</value>
        /// <property name="flag" value="Finished"/>
        public float Gray
        {
            get
            {
                return (((float)((m_red + m_green) + m_blue)) / (MaxColourChannelValue * 3));
            }
            set
            {
                if (value < 0f)
                {
                    m_gray = 0f;
                }
                else if (value > 1f)
                {
                    m_gray = 1f;
                }
                else
                {
                    m_gray = value;
                }
                R = (byte)(m_gray * MaxColourChannelValue);
                G = (byte)(m_gray * MaxColourChannelValue);
                B = (byte)(m_gray * MaxColourChannelValue);
                AssignCMYK(m_red, m_green, m_blue);
                m_isFilled = true;
            }
        }

        /// <summary>
        /// Gets or sets Black channel value.
        /// </summary>
        /// <value>The K.</value>
        /// <property name="flag" value="Finished"/>
        public float K
        {
            get
            {
                return m_black;
            }
            set
            {
                if (value < 0f)
                {
                    m_black = 0f;
                }
                else if (value > 1f)
                {
                    m_black = 1f;
                }
                else
                {
                    m_black = value;
                }
                AssignRGB(m_cyan, m_magenta, m_yellow, m_black);
                m_isFilled = true;
            }
        }

        /// <summary>
        /// Gets or sets Magenta channel value.
        /// </summary>
        /// <value>The M.</value>
        /// <property name="flag" value="Finished"/>
        public float M
        {
            get
            {
                return m_magenta;
            }
            set
            {
                if (value < 0f)
                {
                    m_magenta = 0f;
                }
                else if (value > 1f)
                {
                    m_magenta = 1f;
                }
                else
                {
                    m_magenta = value;
                }
                AssignRGB(m_cyan, m_magenta, m_yellow, m_black);
                m_isFilled = true;
            }
        }

        /// <summary>
        /// Gets or sets Red channel value.
        /// </summary>
        /// <value>The R.</value>
        /// <property name="flag" value="Finished"/>
        public byte R
        {
            get
            {
                return m_red;
            }
            set
            {
                m_red = value;
                AssignCMYK(m_red, m_green, m_blue);
                m_isFilled = true;
            }
        }

        /// <summary>
        /// Gets the red.
        /// </summary>
        public float Red
        {
            get
            {
                return R / MaxColourChannelValue;
            }
        }

        /// <summary>
        /// Gets or sets Yellow channel value.
        /// </summary>
        /// <value>The Y.</value>
        /// <property name="flag" value="Finished"/>
        public float Y
        {
            get
            {
                return m_yellow;
            }
            set
            {
                if (value < 0f)
                {
                    m_yellow = 0f;
                }
                else
                {
                    if (value > 1f)
                    {
                        m_yellow = 1f;
                    }
                    else
                    {
                        m_yellow = value;
                    }
                    AssignRGB(m_cyan, m_magenta, m_yellow, m_black);
                    m_isFilled = true;
                }
            }
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Gets or sets Alpha channel value.
        /// </summary>
        internal byte A
        {
            get
            {
                return m_alpha;
            }
            set
            {
                if (value < 0)
                {
                    m_alpha = 0;
                }
                else
                {
                    if (m_alpha != value)
                    {
                        m_alpha = value;
                    }
                }
                m_isFilled = true;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColor"/> class.
        /// </summary>
        /// <param name="color">Source color object.</param>
        /// <property name="flag" value="Finished"/>
        public PdfColor(PdfColor color)
        {
            // Just initialization.
            m_red = color.R;
            m_cyan = color.C;
            m_green = color.G;
            m_magenta = color.M;
            m_blue = color.B;
            m_yellow = color.Y;
            m_black = color.K;
            m_gray = color.Gray;
            m_alpha = color.m_alpha;

            m_isFilled = (m_alpha != 0);
            //true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColor"/> class.
        /// </summary>
        /// <param name="color">Source color object.</param>
        /// <property name="flag" value="Finished"/>
#if SILVERLIGHT
        public PdfColor(System.Windows.Media.Color color)
            : this(color.A, color.R, color.G, color.B)
#else
         public PdfColor(Color color)
            : this(color.A, color.R, color.G, color.B)
#endif
        {
#if SILVERLIGHT
            if (color.IsEmpty())
            {
                m_isFilled = false;
            }
#else
            if (color.Equals(Color.Empty))
            {
                m_isFilled = false;
            }
            else if (CompareColours(color, Color.Empty))
            {
                m_isFilled = false;
            }
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColor"/> class.
        /// </summary>
        /// <param name="gray">Gray value.</param>
        /// <property name="flag" value="Finished"/>
        public PdfColor(float gray)
        {
            if (gray < 0f)
            {
                gray = 0f;
            }

            if (gray > 1f)
            {
                gray = 1f;
            }

            m_red = (byte)(gray * MaxColourChannelValue);
            m_green = (byte)(gray * MaxColourChannelValue);
            m_blue = (byte)(gray * MaxColourChannelValue);
            m_cyan = gray;
            m_magenta = gray;
            m_yellow = gray;
            m_black = gray;
            m_gray = gray;
            m_alpha = (byte)MaxColourChannelValue;
            m_isFilled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColor"/> class.
        /// </summary>
        /// <param name="red">Red channel value.</param>
        /// <param name="green">Green channel value.</param>
        /// <param name="blue">Blue channel value.</param>
        /// <property name="flag" value="Finished"/>
        public PdfColor(byte red, byte green, byte blue)
            : this((byte)MaxColourChannelValue, red, green, blue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColor"/> class.
        /// </summary>
        /// <param name="red">The red colour value in the range from 0.0 to 1.0.</param>
        /// <param name="green">The green value in the range from 0.0 to 1.0.</param>
        /// <param name="blue">The blue value in the range from 0.0 to 1.0.</param>
        internal PdfColor(float red, float green, float blue)
            : this((byte)(red * MaxColourChannelValue),
                            (byte)(green * MaxColourChannelValue),
                            (byte)(blue * MaxColourChannelValue))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColor"/> class.
        /// </summary>
        /// <param name="cyan">Cyan channel value.</param>
        /// <param name="magenta">Magenta channel value.</param>
        /// <param name="yellow">Yellow channel value.</param>
        /// <param name="black">Black channel value.</param>
        /// <property name="flag" value="Finished"/>
        public PdfColor(float cyan, float magenta, float yellow, float black)
        {
            m_red = 0;
            m_cyan = cyan;
            m_green = 0;
            m_magenta = magenta;
            m_blue = 0;
            m_yellow = yellow;
            m_black = black;
            m_gray = 0;
            m_alpha = (byte)MaxColourChannelValue;
            m_isFilled = true;

            AssignRGB(cyan, magenta, yellow, black);
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Creates new object.
        /// </summary>
        /// <param name="a">Alpha channel.</param>
        /// <param name="red">Red channel value.</param>
        /// <param name="green">Green channel value.</param>
        /// <param name="blue">Blue channel value.</param>
        internal PdfColor(byte a, byte red, byte green, byte blue)
        {
            m_black = 0;
            m_cyan = 0;
            m_magenta = 0;
            m_yellow = 0;
            m_gray = 0f;
            m_red = red;
            m_green = green;
            m_blue = blue;
            m_alpha = a;
            m_isFilled = (m_alpha != 0);

            AssignCMYK(red, green, blue);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates the Alpha ,Red ,Green, and Blue value of this PDFColor structure.
        /// </summary>
        /// <returns>ARGB value.</returns>
        /// <property name="flag" value="Finished"/>
        public int ToArgb()
        {
            Color color = FromRGBColor(R, G, B);
            return color.ToArgb();
        }

        private static Color FromRGBColor(int r, int g, int b)
        {
#if SILVERLIGHT || NETFX_CORE || WP
            Color rgb = new Color();
            
            rgb.R = (byte)r;
            rgb.G = (byte)g;
            rgb.B = (byte)b;

            return rgb;
#else
            return Color.FromArgb(r, g, b);
#endif
        }

        #endregion

        #region Operators
        /// <summary>
        /// Implicit operator.
        /// </summary>
        /// <param name="color">System.Drawing.Color.</param>
        /// <returns>PDFColor.</returns>
        /// <property name="flag" value="Finished"/>
        public static implicit operator PdfColor(Color color)
        {
            PdfColor pdfColor = new PdfColor(color);

            return pdfColor;
        }

        /// <summary>
        /// Implicit operator.
        /// </summary>
        /// <param name="color">System.Drawing.Color.</param>
        /// <returns>PDFColor.</returns>
        /// <property name="flag" value="Finished"/>
        public static implicit operator Color(PdfColor color)
        {
            Color result = Color.FromArgb(color.A, color.R, color.G, color.B);

            return result;
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="colour1">The color 1.</param>
        /// <param name="colour2">The color 2.</param>
        /// <returns>
        /// True if color 1 is equal to color 2; otherwise False.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        public static bool operator ==(PdfColor colour1, PdfColor colour2)
        {
            bool result;
            result = colour1.Equals(colour2);

            return result;
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="colour1">The color 1.</param>
        /// <param name="colour2">The color 2.</param>
        /// <returns>
        /// True if color 1 is not equal to color 2; otherwise False.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        public static bool operator !=(PdfColor colour1, PdfColor colour2)
        {
            return !(colour1 == colour2);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/>
        /// is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to
        /// compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// True if the specified <see cref="T:System.Object"/> is equal
        /// to the current <see cref="T:System.Object"/>; otherwise -
        /// False.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        public override bool Equals(object obj)
        {
            if (obj is PdfColor)
            {
                return Equals((PdfColor)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Determines if the specified color is equal to this one.
        /// </summary>
        /// <param name="colour">The color.</param>
        /// <returns>
        /// True if the color is equal; otherwise - False.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        public bool Equals(PdfColor colour)
        {
            bool result = false;

            if (IsEmpty && colour.IsEmpty)
            {
                result = true;
            }
            if (!IsEmpty || !colour.IsEmpty)
            {
                result |= (m_black != colour.m_black);
                result |= (m_cyan != colour.m_cyan);
                result |= (m_magenta != colour.m_magenta);
                result |= (m_yellow != colour.m_yellow);
                result |= (m_gray != colour.m_gray);
                result |= (m_red != colour.m_red);
                result |= (m_green != colour.m_green);
                result |= (m_blue != colour.m_blue);
                result |= (m_alpha != colour.m_alpha);
            }

            return !result;

        }

        /// <summary>
        /// Serves as a hash function for a particular type, suitable for
        /// use in hashing algorithms and data structures like a hash
        /// table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Implementation
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Sets RGB color.
        /// </summary>
        /// <param name="ifStroking">If color stroking.</param>
        /// <returns>
        /// Result string.
        /// </returns>
        private string RGBToString(bool ifStroking)
        {
            byte r = R;
            byte g = G;
            byte b = B;

            int key = ((int)r << 16) + ((int)g << 8) + b;
            if (ifStroking) key += 1 << 24;

            string colour;

            lock (s_rgbStrings)
            {
                object obj = null;

                if (s_rgbStrings.ContainsKey(key))
                    obj = s_rgbStrings[key];

                if (obj == null)
                {
                    float red = r / MaxColourChannelValue;
                    float green = g / MaxColourChannelValue;
                    float blue = b / MaxColourChannelValue;

                    colour = string.Format(CultureInfo.InvariantCulture,
                        "{0:#0.######} {1:#0.######} {2:#0.######} {3}{4}", red, green, blue,
                        (ifStroking) ? Operators.SetRGBColorForStroking : Operators.SetRGBColorForNonStroking,
                        Operators.NewLine);

                    s_rgbStrings[key] = colour;
                }
                else
                {
                    colour = obj.ToString();
                }
            }

            return colour;
        }

        /// <summary>
        /// Sets Calibrated RGB color.
        /// </summary>
        /// <param name="ifStroking"></param>
        /// <returns></returns>
        private string CalRGBToString(bool ifStroking)
        {
            if ((R > 1) || (R < 0) || (G > 1) || (G < 0) || (G > 1) || (G < 0))
            {
                string s = CalLabToString(ifStroking);
                return s;
            }
            else
            {
                byte r = Convert.ToByte(R * 255);
                byte g = Convert.ToByte(G * 255);
                byte b = Convert.ToByte(B * 255);

                int key = ((int)r << 16) + ((int)g << 8) + b;
                if (ifStroking) key += 1 << 24;

                string colour;

                lock (s_rgbStrings)
                {
                    if (!s_rgbStrings.ContainsKey(key))
                    {
                        float red = r / MaxColourChannelValue;
                        float green = g / MaxColourChannelValue;
                        float blue = b / MaxColourChannelValue;

                        colour = string.Format(CultureInfo.InvariantCulture,
                            "{0:#0.######} {1:#0.######} {2:#0.######} {3}{4}", red, green, blue,
                            (ifStroking) ? Operators.SetColorStroking : Operators.SetColorNonStroking,
                            Operators.NewLine);

                        s_rgbStrings[key] = colour;
                    }
                    else
                    {
                        colour = s_rgbStrings[key].ToString();
                    }
                }

                return colour;
            }
        }

        /// <summary>
        /// Sets Calibrated Lab color.
        /// </summary>
        /// <param name="ifStroking"></param>
        /// <returns></returns>
        private string CalLabToString(bool ifStroking)
        {
            byte r = R;
            byte g = G;
            byte b = B;

            int key = ((int)r << 16) + ((int)g << 8) + b;
            if (ifStroking) key += 1 << 24;

            string colour;

            lock (s_rgbStrings)
            {
                object obj = s_rgbStrings.ContainsKey(key) ? s_rgbStrings[key] : null;

                if (obj == null)
                {
                    float red = r;
                    float green = g;
                    float blue = b;

                    colour = string.Format(CultureInfo.InvariantCulture,
                        "{0:#0.######} {1:#0.######} {2:#0.######} {3}{4}", red, green, blue,
                        (ifStroking) ? Operators.SetColorStroking : Operators.SetColorNonStroking,
                        Operators.NewLine);

                    s_rgbStrings[key] = colour;
                }
                else
                {
                    colour = obj.ToString();
                }
            }

            return colour;
        }


        /// <summary>
        /// Sets Calibrated Gray color.
        /// </summary>
        /// <param name="ifStroking"></param>
        /// <returns></returns>
        private string CalGrayscaleToString(bool ifStroking)
        {
            float gray = Gray;
            string colour;

            lock (s_grayStringsSroke)
            {
                object obj = (ifStroking) ? 
                    s_grayStringsSroke.ContainsKey(gray) ? s_grayStringsSroke[gray] : null :
                    s_grayStringsFill.ContainsKey(gray) ? s_grayStringsFill[gray] : null;

                if (obj == null)
                {
                    colour = string.Format(CultureInfo.InvariantCulture, "{0} {1}{2}", gray,
                        (ifStroking) ? Operators.SetColorStroking : Operators.SetColorNonStroking,
                        Operators.NewLine);

                    if (ifStroking)
                    {
                        s_grayStringsSroke[gray] = colour;
                    }
                    else
                    {
                        s_grayStringsFill[gray] = colour;
                    }
                }
                else
                {
                    colour = obj.ToString();
                }
            }

            return colour;
        }

        /// <summary>
        /// Sets Calibrated RGB color.
        /// </summary>
        /// <param name="ifStroking"></param>
        /// <returns></returns>
        private string IccRGBToString(bool ifStroking)
        {
            if ((R > 1) || (R < 0) || (G > 1) || (G < 0) || (G > 1) || (G < 0))
            {
                string s = CalLabToString(ifStroking);
                return s;
            }
            else
            {
                byte r = Convert.ToByte(R * 255);
                byte g = Convert.ToByte(G * 255);
                byte b = Convert.ToByte(B * 255);

                int key = ((int)r << 16) + ((int)g << 8) + b;
                if (ifStroking) key += 1 << 24;

                string colour;

                lock (s_rgbStrings)
                {
                    object obj=null;
                    if(s_rgbStrings.ContainsKey(key))
                    obj = s_rgbStrings[key];

                    if (obj == null)
                    {
                        float red = r / MaxColourChannelValue;
                        float green = g / MaxColourChannelValue;
                        float blue = b / MaxColourChannelValue;

                        colour = string.Format(CultureInfo.InvariantCulture,
                            "{0:#0.######} {1:#0.######} {2:#0.######} {3}{4}", red, green, blue,
                            (ifStroking) ? Operators.SetColorAndPatternStroking : Operators.SetColorAndPattern,
                            Operators.NewLine);

                        s_rgbStrings[key] = colour;
                    }
                    else
                    {
                        colour = obj.ToString();
                    }
                }

                return colour;
            }
        }

        /// <summary>
        /// Sets Calibrated CMYK color.
        /// </summary>
        /// <param name="ifStroking"></param>
        /// <returns></returns>
        private string CalCMYKToString(bool ifStroking)
        {
            string colour = string.Format(CultureInfo.InvariantCulture,
                "{0:#0.######} {1:#0.######} {2:#0.######} {3:#0.######} {4}{5}",
                m_cyan, m_magenta, m_yellow, m_black,
                (ifStroking) ? Operators.SetColorAndPatternStroking : Operators.SetColorAndPattern,
                Operators.NewLine);

            return colour;
        }
        /// <summary>
        /// Sets Calibrated Lab color.
        /// </summary>
        /// <param name="ifStroking"></param>
        /// <returns></returns>
        private string IccLabToString(bool ifStroking)
        {
            byte r = R;
            byte g = G;
            byte b = B;

            int key = ((int)r << 16) + ((int)g << 8) + b;
            if (ifStroking) key += 1 << 24;

            string colour;

            lock (s_rgbStrings)
            {
                object obj = s_rgbStrings[key];

                if (obj == null)
                {
                    float red = r;
                    float green = g;
                    float blue = b;

                    colour = string.Format(CultureInfo.InvariantCulture,
                        "{0:#0.######} {1:#0.######} {2:#0.######} {3}{4}", red, green, blue,
                        (ifStroking) ? Operators.SetColorStroking : Operators.SetColorNonStroking,
                        Operators.NewLine);

                    s_rgbStrings[key] = colour;
                }
                else
                {
                    colour = obj.ToString();
                }
            }

            return colour;
        }


        /// <summary>
        /// Sets Calibrated Gray color.
        /// </summary>
        /// <param name="ifStroking"></param>
        /// <returns></returns>
        private string IccGrayscaleToString(bool ifStroking)
        {
            float gray = Gray;
            string colour;

            lock (s_grayStringsSroke)
            {
                object obj = (ifStroking) ?
                    s_grayStringsSroke.ContainsKey(gray) ? s_grayStringsSroke[gray] : null :
                    s_grayStringsSroke.ContainsKey(gray) ? s_grayStringsFill[gray] : null;

                if (obj == null)
                {
                    colour = string.Format(CultureInfo.InvariantCulture, "{0} {1}{2}", gray,
                        (ifStroking) ? Operators.SetColorAndPatternStroking : Operators.SetColorAndPattern,
                        Operators.NewLine);

                    if (ifStroking)
                    {
                        s_grayStringsSroke[gray] = colour;
                    }
                    else
                    {
                        s_grayStringsFill[gray] = colour;
                    }
                }
                else
                {
                    colour = obj.ToString();
                }
            }

            return colour;
        }

        internal string IndexedToString(bool ifStroking)
        {
            float gray = G;
            string colour;

            lock (s_grayStringsSroke)
            {
                object obj = (ifStroking) ?
                    s_grayStringsSroke.ContainsKey(gray) ? s_grayStringsSroke[gray] : null :
                    s_grayStringsFill.ContainsKey(gray) ? s_grayStringsFill[gray] : null;

                if (obj == null)
                {
                    colour = string.Format(CultureInfo.InvariantCulture, "{0} {1}{2}", gray,
                        (ifStroking) ? Operators.SetColorStroking : Operators.SetColorNonStroking,
                        Operators.NewLine);

                    if (ifStroking)
                    {
                        s_grayStringsSroke[gray] = colour;
                    }
                    else
                    {
                        s_grayStringsFill[gray] = colour;
                    }
                }
                else
                {
                    colour = obj.ToString();
                }
            }

            return colour;
        }


        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Sets gray color.
        /// </summary>
        /// <param name="ifStroking">If color stroking.</param>
        /// <returns>
        /// Result string.
        /// </returns>
        private string GrayscaleToString(bool ifStroking)
        {
            float gray = Gray;
            string colour;

            lock (s_grayStringsSroke)
            {
                object obj = (ifStroking) ? 
                    s_grayStringsSroke.ContainsKey(gray) ? s_grayStringsSroke[gray] : null
                    : s_grayStringsFill.ContainsKey(gray) ? s_grayStringsFill[gray] : null;

                if (obj == null)
                {
                    colour = string.Format(CultureInfo.InvariantCulture, "{0} {1}{2}", gray,
                        (ifStroking) ? Operators.SetGrayColorForStroking : Operators.SetGrayColorForNonstroking,
                        Operators.NewLine);

                    if (ifStroking)
                    {
                        s_grayStringsSroke[gray] = colour;
                    }
                    else
                    {
                        s_grayStringsFill[gray] = colour;
                    }
                }
                else
                {
                    colour = obj.ToString();
                }
            }

            return colour;
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Sets CMYK color.
        /// </summary>
        /// <param name="ifStroking">If color stroking.</param>
        /// <returns>
        /// Result string.
        /// </returns>
        private string CMYKToString(bool ifStroking)
        {
            string colour = string.Format(CultureInfo.InvariantCulture,
                "{0:#0.######} {1:#0.######} {2:#0.######} {3:#0.######} {4}{5}",
                m_cyan, m_magenta, m_yellow, m_black,
                (ifStroking) ? Operators.SetCMYKColorForStroking : Operators.SetCMYKColorForNonstroking,
                Operators.NewLine);

            return colour;
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Writes RGB colour to string builder.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="stroke">If set to True stroke.</param>
        private void RGBToStringBuilder(StringBuilder sb, bool stroke)
        {
            float red = R / MaxColourChannelValue;
            float green = G / MaxColourChannelValue;
            float blue = B / MaxColourChannelValue;

            sb.Append(PdfNumber.FloatToString(red));
            sb.Append(Operators.WhiteSpace);
            sb.Append(PdfNumber.FloatToString(green));
            sb.Append(Operators.WhiteSpace);
            sb.Append(PdfNumber.FloatToString(blue));
            sb.Append(Operators.WhiteSpace);

            if (stroke)
            {
                sb.Append(Operators.SetRGBColorForStroking);
            }
            else
            {
                sb.Append(Operators.SetRGBColorForNonStroking);
            }

            //sb.Append( Operators.NewLine );
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Writes CMYK color to string builder.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="stroke">If set to true stroke; otherwise, false.</param>
        private void CMYKToStringBuilder(StringBuilder sb, bool stroke)
        {
            sb.Append(PdfNumber.FloatToString(m_cyan));
            sb.Append(Operators.WhiteSpace);
            sb.Append(PdfNumber.FloatToString(m_magenta));
            sb.Append(Operators.WhiteSpace);
            sb.Append(PdfNumber.FloatToString(m_yellow));
            sb.Append(Operators.WhiteSpace);
            sb.Append(PdfNumber.FloatToString(m_black));
            sb.Append(Operators.WhiteSpace);

            if (stroke)
            {
                sb.Append(Operators.SetCMYKColorForStroking);
            }
            else
            {
                sb.Append(Operators.SetCMYKColorForNonstroking);
            }
            //sb.Append( Operators.NewLine );
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Writes grayscale color to string builder.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="stroke">If set to True stroke.</param>
        private void GrayscaleToStringBuilder(StringBuilder sb, bool stroke)
        {
            sb.Append(PdfNumber.FloatToString(Gray));
            sb.Append(Operators.WhiteSpace);

            if (stroke)
            {
                sb.Append(Operators.SetGrayColorForStroking);
            }
            else
            {
                sb.Append(Operators.SetGrayColorForNonstroking);
            }
            //sb.Append( Operators.NewLine );
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Converts PDFColor to PDF string representation.
        /// </summary>
        /// <param name="colorSpace">Color space.</param>
        /// <param name="stroke">If color stroking.</param>
        /// <returns>
        /// Result string.
        /// </returns>
        internal string ToString(PdfColorSpace colorSpace, bool stroke)
        {
            if (IsEmpty)
            {
                return string.Empty;
            }

            switch (colorSpace)
            {
                case PdfColorSpace.RGB:
                    {
                        return RGBToString(stroke);

                    }
                case PdfColorSpace.GrayScale:
                    {
                        return GrayscaleToString(stroke);
                    }
                case PdfColorSpace.CMYK:
                    {
                        return CMYKToString(stroke);
                    }

                default:
                    throw new ArgumentException("Unsupported colour space: " + colorSpace);
            }
        }

        /// <summary>
        /// Converts PDFColor to PDF string representation.
        /// </summary>
        /// <param name="colorSpace"></param>
        /// <param name="stroke"></param>
        /// <returns></returns>
        internal string CalToString(PdfColorSpace colorSpace, bool stroke)
        {
            if (IsEmpty)
            {
                return string.Empty;
            }

            switch (colorSpace)
            {
                case PdfColorSpace.RGB:
                    {
                        return CalRGBToString(stroke);

                    }
                case PdfColorSpace.GrayScale:
                    {
                        return CalGrayscaleToString(stroke);
                    }
                case PdfColorSpace.CMYK:
                    {
                        return CMYKToString(stroke);
                    }

                default:
                    throw new ArgumentException("Unsupported colour space: " + colorSpace);
            }
        }

        internal string IccColorToString(PdfColorSpace colorSpace, bool stroke)
        {
            if (IsEmpty)
            {
                return string.Empty;
            }

            switch (colorSpace)
            {
                case PdfColorSpace.RGB:
                    {
                        return IccRGBToString(stroke);

                    }
                case PdfColorSpace.GrayScale:
                    {
                        return IccGrayscaleToString(stroke);
                    }
                case PdfColorSpace.CMYK:
                    {
                        return CalCMYKToString(stroke);
                    }

                default:
                    throw new ArgumentException("Unsupported colour space: " + colorSpace);
            }
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Writes color value to a string builder.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="colorSpace">The color space.</param>
        /// <param name="stroke">If set to True stroke.</param>
        internal void WriteToStringBuilder(StringBuilder sb, PdfColorSpace colorSpace,
            bool stroke)
        {
            if (sb == null)
                throw new ArgumentNullException("sb");

            if (IsEmpty)
            {
                sb.Append(string.Empty);
            }
            else
            {
                switch (colorSpace)
                {
                    case PdfColorSpace.RGB:
                        RGBToStringBuilder(sb, stroke);
                        break;

                    case PdfColorSpace.GrayScale:
                        GrayscaleToStringBuilder(sb, stroke);
                        break;

                    case PdfColorSpace.CMYK:
                        CMYKToStringBuilder(sb, stroke);
                        break;
                }
            }
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Converts RGB to CMYK.
        /// </summary>
        /// <param name="r">Red channel value.</param>
        /// <param name="g">Green channel value.</param>
        /// <param name="b">Blue channel value.</param>
        private void AssignCMYK(byte r, byte g, byte b)
        {
            float red = (float)r / MaxColourChannelValue;
            float green = (float)g / MaxColourChannelValue;
            float blue = (float)b / MaxColourChannelValue;

            float black = PdfNumber.Min(1 - red, 1 - green, 1 - blue);
            float cyan = (black == 1.0f) ? 0 : (1 - red - black) / (1 - black);
            float magenta = (black == 1.0f) ? 0 : (1 - green - black) / (1 - black);
            float yellow = (black == 1.0f) ? 0 : (1 - blue - black) / (1 - black);

            m_black = black;
            m_cyan = cyan;
            m_magenta = magenta;
            m_yellow = yellow;
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Converts CMYK to RGB.
        /// </summary>
        /// <param name="cyan">Cyan channel value.</param>
        /// <param name="magenta">Magenta channel value.</param>
        /// <param name="yellow">Yellow channel value.</param>
        /// <param name="black">Black channel value.</param>
        private void AssignRGB(float cyan, float magenta, float yellow, float black)
        {
            float fBlack = black * MaxColourChannelValue;
            float redInit = (cyan * (MaxColourChannelValue - fBlack)) + fBlack;
            float greenInit = (magenta * (MaxColourChannelValue - fBlack)) + fBlack;
            float blueInit = (yellow * (MaxColourChannelValue - fBlack)) + fBlack;

            m_red = (byte)(MaxColourChannelValue - Math.Min(MaxColourChannelValue, redInit));
            m_green = (byte)(MaxColourChannelValue - Math.Min(MaxColourChannelValue, greenInit));
            m_blue = (byte)(MaxColourChannelValue - Math.Min(MaxColourChannelValue, blueInit));
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Compares colors.
        /// </summary>
        /// <param name="color1">The color 1.</param>
        /// <param name="color2">The color 2.</param>
        /// <returns>
        /// True if colors are identical; otherwise - False.
        /// </returns>
#if SILVERLIGHT
    private static bool CompareColours(System.Windows.Media.Color color1, System.Windows.Media.Color color2)
#else
    private static bool CompareColours(Color color1, Color color2)
#endif
        {
            bool result = true;

            result &= color1.A == color2.A;
            result &= color1.R == color2.R;
            result &= color1.G == color2.G;
            result &= color1.B == color2.B;

            return result;
        }

        /// <summary>
        /// Converts colour to a PDF array of R, G and B float values.
        /// </summary>
        /// <returns>Filled PdfArray object.</returns>
        internal PdfArray ToArray()
        {
            return ToArray(PdfColorSpace.RGB);
        }

        /// <summary>
        /// Converts colour to a PDF array.
        /// </summary>
        /// <param name="colorSpace">The color space.</param>
        /// <returns>The well filled PdfArray object.</returns>
        internal PdfArray ToArray(PdfColorSpace colorSpace)
        {
            PdfArray array = new PdfArray();

            switch (colorSpace)
            {
                case PdfColorSpace.CMYK:
                    array.Add(new PdfNumber(C));
                    array.Add(new PdfNumber(M));
                    array.Add(new PdfNumber(Y));
                    array.Add(new PdfNumber(K));
                    break;

                case PdfColorSpace.GrayScale:
                    array.Add(new PdfNumber(Gray));
                    break;

                case PdfColorSpace.RGB:
                    array.Add(new PdfNumber(Red));
                    array.Add(new PdfNumber(Green));
                    array.Add(new PdfNumber(Blue));
                    break;

                default:
                    throw new NotSupportedException("Unsupported colour space.");
            }
            return array;
        }
        #endregion

    }
}
