#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Syncfusion.XPS
{
    /// <summary>
    /// Represents the path data reader.
    /// </summary>
    internal class PathDataReader
    {
        #region Constants
        private const string ST_Double = @"((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)";
        private const string ST_Point = @"((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)";
        private const string ST_EventArrayPos = @"(\+?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-859 9]+)?) (\+?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( (\+?(([0-9]+(\.[0-860 9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?) (\+?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-861 |\+)?[0-9]+)?))*";
        private const string ST_Points = @"((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)(((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?))*";
        private const string ST_PointsM3 = @"((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)){2}(( ((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)){3})*";
        private char[] m_commaSeparator = { ',' };
        #endregion

        #region Fields
        private char[] m_seperator = new char[] { ' ' };
        private string m_text;
        private int m_position;
        private char[] m_symbols = new char[] { 'F', 'f', 'm', 'M', 'l', 'L', 'h', 'H', 'v', 'V', 'c', 'C', 'q', 'Q', 's', 'S', 'a', 'A', 'z', 'Z' };
        private char m_currentSymbol;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this <see cref="StringTokenizer"/> is EOF.
        /// </summary>
        /// <value><c>true</c> if EOF; otherwise, <c>false</c>.</value>
        public bool EOF
        {
            get
            {
                return (m_position == m_text.Length ||
                    m_text.IndexOfAny(m_symbols, m_position) == -1);
            }
        }

        /// <summary>
        /// Gets text length.
        /// </summary>
        public int Length
        {
            get
            {
                return m_text.Length;
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
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
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the PathDataReader class.
        /// </summary>
        /// <param name="text"></param>
        public PathDataReader(string text)
        {
            m_text = text;
            m_position = 0;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Reads the symbols
        /// </summary>
        /// <returns>Symbol</returns>
        public char ReadSymbol()
        {
            int pos = m_text.IndexOfAny(m_symbols, m_position);

            if (pos == -1)
                return '\0';

            m_currentSymbol = m_text[pos];
            m_position = pos + 1;
            return m_currentSymbol;
        }

        /// <summary>
        /// Gets the next symbol
        /// </summary>
        /// <returns>Symbol</returns>
        public char GetNextSymbol()
        {
            int pos = m_text.IndexOfAny(m_symbols, m_position);

            if (pos == -1)
                return '\0';

            return m_text[pos];
        }

        /// <summary>
        /// Updates the current position of the reader
        /// </summary>
        /// <param name="length">Length of the path data</param>
        public void UpdateCurrentPosition(int length)
        {
            m_position += length;
            //int nextSpace = m_text.IndexOf(' ', m_position);
            //if (nextSpace != -1)
            //    m_position = nextSpace;
        }

        /// <summary>
        /// Reads the float value from the path data
        /// </summary>
        /// <param name="value">float value</param>
        /// <returns>True if the next value is float</returns>
        public bool TryReadFloat(out float value)
        {
            value = 0;

            Match result = Regex.Match(m_text.Substring(m_position), ST_Double);

            if (result.Success)
            {
                UpdateCurrentPosition(result.Index + result.Captures[0].Value.Length);
                value = XPSRenderer.ParseFloat(result.Captures[0].Value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the pint form the path data
        /// </summary>
        /// <param name="val">Point value</param>
        /// <returns>True if the next parameter is point</returns>
        public bool TryReadPoint(out PointF val)
        {
            val = PointF.Empty;

            Match result = Regex.Match(m_text.Substring(m_position), ST_Points);

            if (result.Success)
            {
                if (char.IsLetter(m_text[m_position]) && m_text[m_position] != 'L')
                    return false;

                // If next char is not a letter or an empty space, then proceed.
                if (!char.Equals(' ', m_text[m_position + 1]) && char.IsLetter(m_text, m_position + 1) && (m_text[m_position + 1] != 'L'))
                    return false;

                string res = result.Captures[0].Value;

                UpdateCurrentPosition(result.Index + res.Length);
                string[] points = res.Split(m_commaSeparator);
                val = new PointF(XPSRenderer.ParseFloat(points[0]), XPSRenderer.ParseFloat(points[1]));
                return true;
            }
            return false;
        }

        public bool TryReadPointM3(out PointF[] val)
        {
            val = null;
            Match result = Regex.Match(m_text.Substring(m_position), ST_PointsM3);
            List<PointF> val1 = new List<PointF>();

            if (result.Success)
            {
                if (char.IsLetter(m_text[m_position]) && m_text[m_position] != 'C')
                    return false;

                // If next char is not a letter or an empty space, then proceed.
                if (!char.Equals(' ', m_text[m_position + 1]) && char.IsLetter(m_text, m_position + 1))
                    return false;

                string res = result.Captures[0].Value;

                UpdateCurrentPosition(result.Index + res.Length);
                res = res.Replace(", ", ",");
                string[] points = res.Split(m_seperator);
                for (int i = 0; i < points.Length; i++)
                    val1.Add(new PointF(XPSRenderer.ParseFloat(points[i].Split(m_commaSeparator)[0]), XPSRenderer.ParseFloat(points[i].Split(m_commaSeparator)[1])));

                val = val1.ToArray();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the position array from the path data
        /// </summary>
        /// <param name="val">position array</param>
        /// <returns>True if the next parameter is position array</returns>
        public bool TryReadPositionArray(out string[] val)
        {
            val = null;
            Match result = Regex.Match(m_text.Substring(m_position), ST_EventArrayPos);

            if (result.Success)
            {
                val = (result.Captures[0].Value).Split(m_seperator);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the points from the path data
        /// </summary>
        /// <returns>Points</returns>
        public PointF[] ReadPoints()
        {
            List<PointF> points = new List<PointF>();

            PointF point;

            while (!CheckIfCurrentCharIsSymbol() && TryReadPoint(out point))
            {
                points.Add(point);
            }

            return points.ToArray();
        }

        /// <summary>
        /// Checks if the current character is symbol
        /// </summary>
        /// <returns>True if the character is a symbol</returns>
        private bool CheckIfCurrentCharIsSymbol()
        {
            return m_text.IndexOfAny(m_symbols, m_position) == m_position + 1 ? true : false;
        }
        #endregion
    }
}
