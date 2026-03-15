#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.XPS
{
    internal class XPSDataReader
    {
        #region Constants
        private const string ST_Name = @"(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lo}|\p{Nl}|_)(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lo}|\p{Nl}|\p{Mn}|\p{Mc}|\p{Nd}|_)*";
        private const string ST_Boolean = @"true|false";
        private const string ST_Double = @"((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)";
        private const string ST_Point = @"((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)";
        private const string ST_Matrix = @"((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)( ?, ?)((\-|\+)?(([0-9]+(\.[0-9]+)?)|(\.[0-9]+))((e|E)(\-|\+)?[0-9]+)?)";
        private char[] m_commaSeparator = { ',' };
        #endregion

        #region Implementation
        /// <summary>
        /// Reads the Name of the element
        /// </summary>
        /// <param name="data">XPS data</param>
        /// <param name="position">Reader position</param>
        /// <returns>Name</returns>
        public string ReadName(string data, ref int position)
        {
            string result = "";

            Match match = new Regex(ST_Name).Match(data, position);
            position++;

            if (!match.Success)
                return result;

            result = match.Captures[0].Value;

            return result;
        }

        /// <summary>
        /// Reads the boolean value from the Data
        /// </summary>
        /// <param name="data">XPS data</param>
        /// <param name="position">Reader position</param>
        /// <returns>True if the next value is boolean</returns>
        public bool ReadBoolean(string data, ref int position)
        {
            bool result = false;

            Match match = new Regex(ST_Boolean).Match(data, position);
            position++;

            if (match.Success)
                return bool.TryParse(match.Captures[0].Value, out result);

            return result;
        }

        /// <summary>
        /// Reads the float from the data.
        /// </summary>
        /// <param name="data">XPS data</param>
        /// <param name="position">Reader position</param>
        /// <returns>float value</returns>
        public float ReadDouble(string data, ref int position)
        {
            float result = 0;

            Match match = new Regex(ST_Double).Match(data, position);
            position++;

            if (!match.Success)
                return result;

            float.TryParse(match.Captures[0].Value, out result);

            return result;
        }

        /// <summary>
        /// Reads the point from the data
        /// </summary>
        /// <param name="data">XPS data</param>
        /// <param name="position">Reader position</param>
        /// <returns>point</returns>
        public PointF ReadPoint(string data, ref int position)
        {
            PointF result = PointF.Empty;

            Match match = new Regex(ST_Point).Match(data, position);
            position++;

            if (!match.Success)
                return result;

            string[] points = match.Captures[0].Value.Split(m_commaSeparator);
            result = new PointF(float.Parse(points[0]), float.Parse(points[1]));

            return result;
        }

        /// <summary>
        /// Reads the matrix from the data
        /// </summary>
        /// <param name="data">XPS data</param>
        /// <param name="position">Reader position</param>
        /// <returns>Matrix</returns>
        public Matrix ReadMatrix(string data, ref int position)
        {
            Match match = new Regex(ST_Matrix).Match(data, position);
            position++;

            return null;
        }
        #endregion

    }
}
