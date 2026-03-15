#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

using Syncfusion.Pdf.IO;

namespace Syncfusion.Pdf.Primitives
{
    /// <summary>
    /// Implementation of the name object.
    /// </summary>
# if NETFX_CORE || WP
    public class PdfName : IPdfPrimitive
#else
    internal class PdfName : IPdfPrimitive
#endif
    {
        #region Constants
        /// <summary>
        /// Start symbol of the name object.
        /// </summary>
        internal const string StringStartMark = "/";

        /// <summary>
        /// PDF special characters.
        /// </summary>
        public static string Delimiters = "()<>[]{}/%}";
        #endregion

        #region Static fields
        /// <summary>
        /// The symbols that are not allowed in PDF names and should be replaced.
        /// </summary>
        private static readonly char[] m_replacements =
		{
			' ',
			'\t',
			'\n',
			'\r'
		};
        #endregion

        #region Fields
        /// <summary>
        /// Value of the element.
        /// </summary>
        private string m_value = string.Empty;
        /// <summary>
        /// Shows the type of object status whether it is object registered or other status;
        /// </summary>
        private ObjectStatus m_status;
        /// <summary>
        /// Indicates if the object is currently in saving state or not.
        /// </summary>
        private bool m_isSaving;
        /// <summary>
        /// Holds the index number of the object.
        /// </summary>
        private int m_index;

        /// <summary>
        /// Internal variable to store the position.
        /// </summary>
        private int m_position = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the object.
        /// </summary>
        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if (value != m_value)
                {
                    string val = value;

                    if (value != null && value.Length > 0)
                    {
                        // Check first symbol and trim from left '/' symbol.
                        val = (value.Substring(0, 1) == StringStartMark) ?
                            value.Substring(1) : value;

                        m_value = NormalizeValue(val);
                    }
                    else
                    {
                        m_value = val;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Status of the specified object.
        /// </summary>
        public ObjectStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this document is saving or not.
        /// </summary>
        public bool IsSaving
        {
            get
            {
                return m_isSaving;
            }
            set
            {
                m_isSaving = value;
            }
        }

        /// <summary>
        /// Gets or sets the integer value of the specified object.
        /// </summary>
        public int ObjectCollectionIndex
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the object.
        /// </summary>
        public int Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        /// <summary>
        /// Returns cloned object.
        /// </summary>
        public IPdfPrimitive ClonedObject
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfName"/> class.
        /// </summary>
        public PdfName()
        {
        }

        /// <summary>
        /// Creates object with defined string value.
        /// </summary>
        /// <param name="value">Value of the string.</param>
        public PdfName(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            m_value = NormalizeValue(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfName"/> class.
        /// </summary>
        /// <param name="value">The enum value.</param>
        public PdfName(Enum value)
            : this(value.ToString())
        {
        }
        #endregion

        #region Class static methods
        /// <summary>
        /// Makes the string more correct from the PDF's point of view.
        /// </summary>
        /// <param name="value">The string to normalize as PDF name.</param>
        /// <returns>The normalized string.</returns>
        private static string NormalizeValue(string value)
        {
            string str = value;

            foreach (char c in m_replacements)
            {
                str = NormalizeValue(str, c);
            }

            return str;
        }

        /// <summary>
        /// Replace a symbol with its code with the precedence of the sharp sign.
        /// </summary>
        /// <param name="value">The string which the symbol should be replaced in.</param>
        /// <param name="symbol">The symbol to replace.</param>
        /// <returns></returns>
        private static string NormalizeValue(string value, char symbol)
        {
            const string strFormat = "#{0:X}";

            return value.Replace(symbol.ToString(), string.Format(strFormat, ((int)symbol)));
        }

        /// <summary>
        /// Replace some characters with its escape sequences.
        /// </summary>
        /// <param name="str">The string value.</param>
        /// <returns>Modified string.</returns>
        public static string EscapeString(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str == string.Empty) return str;

            StringBuilder result = new StringBuilder();

            for (int i = 0, len = str.Length; i < len; i++)
            {
                char ch = str[i];

                int index = Delimiters.IndexOf(ch);

                int code = (int)ch;
                switch (code)
                {
                    case '\r':
                        result.Append("\\r");
                        break;
                    case '\n':
                        result.Append("\n");
                        break;
                    case '(':
                    case ')':
                    case '\\':
                        //result.Append( '\\' ).Append( ch );
                        result.Append(ch);
                        break;
                    default:
                        result.Append(ch);
                        break;
                }
            }

            return result.ToString();
        }
        #endregion

        #region Operators
        /// <summary>
        /// Explicit operator. Converts a string to a PdfName.
        /// </summary>
        /// <param name="str">The string representation of the name.</param>
        /// <returns>Properly initialized PdfName instance.</returns>
        public static explicit operator PdfName(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            PdfName name = new PdfName(str);

            return name;
        }

        #endregion

        #region Overrides
        /// <summary>
        /// Gets string representation of the primitive.
        /// </summary>
        public override string ToString()
        {
            return (StringStartMark + EscapeString(Value));
        }

        /// <summary>
        /// Compares two PDF names.
        /// </summary>
        /// <param name="obj">PDFName to compare</param>
        /// <returns>The result of comparison.</returns>
        public override bool Equals(object obj)
        {
            PdfName name = obj as PdfName;

            if (name == (object)null) return false;
            else return (name.Value == Value);
        }

        /// <summary>
        /// Returns a hash code for the name.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Compares two names.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="name2">The name2.</param>
        /// <returns></returns>
        public static bool operator ==(PdfName name1, object name2)
        {
            bool result;
            object name1Object = name1 as object;

            if (name1Object == name2)
            {
                result = true;
            }
            else if (name1Object == null || name2 == null)
            {
                result = false;
            }
            else
            {
                PdfName name2Instance = name2 as PdfName;

                if (name2Instance == null)
                {
                    result = false;
                }
                else
                {
                    result = (name1.Value == name2Instance.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines if two names aren't equal.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="name2">The name2.</param>
        /// <returns></returns>
        public static bool operator !=(PdfName name1, object name2)
        {
            return !(name1 == name2);
        }

        /// <summary>
        /// Determines if two names are equal.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="name2">The name2.</param>
        /// <returns></returns>
        public static bool operator ==(PdfName name1, PdfName name2)
        {
            bool result = false;

            if ((object)name1 == (object)name2)
            {
                result = true;
            }
            else if ((object)name1 == null || (object)name2 == null)
            {
                result = false;
            }
            else
            {
                result = (name1.Value == name2.Value);
            }

            return result;
        }

        /// <summary>
        /// Determines if two names aren't equal.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="name2">The name2.</param>
        /// <returns></returns>
        public static bool operator !=(PdfName name1, PdfName name2)
        {
            return !(name1 == name2);
        }
        #endregion

        #region IPdfSavable Members
        /// <summary>
        /// Saves the name using the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Save(IPdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            //if( Value == null || Value == string.Empty )
            //{
            //  Value = writer.Document.GenerateName();
            //}

            writer.Write(ToString());
        }

        /// <summary>
        /// Creates a copy of PdfName.
        /// </summary>
        public IPdfPrimitive Clone(PdfCrossTable crossTable)
        {
            PdfName newName = new PdfName();
            newName.Value = m_value;

            return newName;
        }
        #endregion
    }
}
