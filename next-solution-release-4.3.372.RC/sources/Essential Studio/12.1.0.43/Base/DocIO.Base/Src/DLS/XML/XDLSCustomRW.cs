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

#region file using directives
using System;
using System.Xml;
using Syncfusion.DocIO.DLS.Entities;
#if (SILVERLIGHT || WP) && !SkipSilverlightNamespaces
#if WINRT
using Syncfusion.DocIO.WinrtHelper;
using System.Windows;
using Windows.Media;
using Matrix = Windows.UI.Xaml.Media.Matrix;
#else
#if !WP
using System.Drawing;
#endif
using Matrix = System.Windows.Media.Matrix;
#endif
#else
using System.Drawing.Drawing2D;
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS.XML
{
    /// <summary>
    /// Summary description for XDLSCustomRW.
    /// </summary>
    public class XDLSCustomRW
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private XmlReader m_reader = null;
        private XmlWriter m_writer = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="XDLSCustomRW"/> class.
        /// </summary>
        public XDLSCustomRW()
        {
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool Write(XmlWriter writer, string tagName, object value)
        {
            m_writer = writer;
            
#if !SILVERLIGHT && !WP
            if( value is Matrix )
            {
              WriteMatrix( tagName, value as Matrix );
            }
#endif
            if (value is Color)
            {
                WriteColor(tagName, (Color)value);
            }
            else if (value is Font)
            {
                WriteFont(tagName, (Font)value);
            }
            else
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public object Read(XmlReader reader, Type type)
        {
            m_reader = reader;

            if (type.Equals(typeof(Matrix)))
            {
                return ReadMatrix();
            }
            if (type.Equals(typeof(Color)))
            {
                return ReadColor();
            }
            if (type.Equals(typeof(Font)))
            {
                return ReadFont();
            }

            return null;
        }
        #endregion

        #region Class helper methods / custom writes
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="matrix"></param>
        private void WriteMatrix(string name, Matrix matrix)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            m_writer.WriteStartElement(name);

            float[] elements = matrix.Elements;
            m_writer.WriteAttributeString(PropertyNames.M11, XmlConvert.ToString(elements[0]));
            m_writer.WriteAttributeString(PropertyNames.M12, XmlConvert.ToString(elements[1]));
            m_writer.WriteAttributeString(PropertyNames.M21, XmlConvert.ToString(elements[2]));
            m_writer.WriteAttributeString(PropertyNames.M22, XmlConvert.ToString(elements[3]));
            m_writer.WriteAttributeString(PropertyNames.D1, XmlConvert.ToString(elements[4]));
            m_writer.WriteAttributeString(PropertyNames.D2, XmlConvert.ToString(elements[5]));

            m_writer.WriteEndElement();
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        private void WriteColor(string name, Color color)
        {
            m_writer.WriteStartElement(name);
            m_writer.WriteAttributeString(XDLSConstants.TypeTag, "Color");
            m_writer.WriteAttributeString("argb", XmlConvert.ToString((int)color.ToArgb()));
            m_writer.WriteEndElement();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="font"></param>
        private void WriteFont(string name, Font font)
        {
            m_writer.WriteStartElement(name);
            m_writer.WriteAttributeString(XDLSConstants.TypeTag, "Font");
#if SILVERLIGHT || WP
      m_writer.WriteAttributeString( "fontName", font.FontFamilyName );
#else
            m_writer.WriteAttributeString("fontName", font.Name);
#endif
            m_writer.WriteAttributeString("size", font.SizeInPoints.ToString());
            //m_writer.WriteAttributeString( XDLSConstants.StyleItemTag, font.Style.ToString() );
            m_writer.WriteEndElement();
        }
        #endregion

        #region Class helper methods / custom readers
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Font ReadFont()
        {
            string fontName = m_reader.GetAttribute("fontName");
            string size = m_reader.GetAttribute("size");
            string style = m_reader.GetAttribute(XDLSConstants.StyleItemTag);
            m_reader.Read();

            return new Font(fontName, int.Parse(size));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Color ReadColor()
        {
            string argb = m_reader.GetAttribute("argb");
            m_reader.Read();

            return Color.FromArgb(int.Parse(argb));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Matrix ReadMatrix()
        {
            string val = m_reader.GetAttribute(PropertyNames.M11);
            float m11 = XmlConvert.ToSingle(val);

            val = m_reader.GetAttribute(PropertyNames.M12);
            float m12 = XmlConvert.ToSingle(val);

            val = m_reader.GetAttribute(PropertyNames.M21);
            float m21 = XmlConvert.ToSingle(val);

            val = m_reader.GetAttribute(PropertyNames.M22);
            float m22 = XmlConvert.ToSingle(val);

            val = m_reader.GetAttribute(PropertyNames.D1);
            float d1 = XmlConvert.ToSingle(val);

            val = m_reader.GetAttribute(PropertyNames.D2);
            float d2 = XmlConvert.ToSingle(val);

            Matrix matrix;
#if WINRT
            Action action = new Action(
                delegate
                {
                    matrix = new Matrix(m11, m12, m21, m22, d1, d2);
                });
            UIDispatcher.Execute(action);
#else
            matrix = new Matrix(m11, m12, m21, m22, d1, d2);
#endif
            m_reader.Read();

            return matrix;
        }
        #endregion
    }
}
