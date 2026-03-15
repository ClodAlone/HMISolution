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

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// This class contains the data of geomerty path.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class Data
    {
        #region Members
        private PathData m_data;
        private GraphicsPath m_path;
        private static Regex m_dataRegex;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the path data.
        /// </summary>
        /// <value>The path data.</value>
        public PathData PathData
        {
            get
            {
                return m_data;
            }
        }

        /// <summary>
        /// Gets the <see cref="GraphicsPath"/>.
        /// </summary>
        /// <value>The path.</value>
        public GraphicsPath Path
        {
            get
            {
                return m_path;
            }
        }

        /// <summary>
        /// Gets the data regex.
        /// </summary>
        /// <value>The data regex.</value>
        private static Regex DataRegex
        {
            get
            {
                if (m_dataRegex == null)
                {
                    m_dataRegex = new Regex(@"(?<type>[mzlhvcsqta])\s*(?<numbers>([-]?[0-9]+([.][0-9]+)?(e[+-][0-9]+)?[, ]*)*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }

                return m_dataRegex;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Data(PathData data)
        {
            m_data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Data(string data)
        {
            ParseString(data);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            string data = "";
            bool closed = false;

            for (int i = 0, length = m_data.Types.Length; i < length; i++)
            {                
                byte type = m_data.Types[i];
                if ((type & (byte)PathPointType.Bezier) == (byte)PathPointType.Bezier)
                {
                    data += SVG.VALUE_C;
                    for (int j = 0; (j < 3) && (i + j) < length; j++)
                    {
                        data += " " + Utility.GetFloat(m_data.Points[i + j].X) + " " +
                            Utility.GetFloat(m_data.Points[i + j].Y) + " ";
                    }

                    i += 2;
                    type = m_data.Types[i];
                    closed = false;
                }
                else

                    if ((type & (byte)PathPointType.Line) == (byte)PathPointType.Line)
                    {
                        data += SVG.VALUE_L;
                        data += " " + Utility.GetFloat(m_data.Points[i].X) + " " +
                            Utility.GetFloat(m_data.Points[i].Y) + " ";
                        closed = false;
                    }
                    else

                        if (type == (byte)PathPointType.Start)
                        {
                            if ((i > 0) && !closed)
                            {
                                data += SVG.VALUE_Z;
                                closed = true;
                            }

                            data += SVG.VALUE_M;
                            data += " " + Utility.GetFloat(m_data.Points[i].X) + " " +
                                Utility.GetFloat(m_data.Points[i].Y) + " ";
                            closed = false;
                        }

                if ((type & (byte)PathPointType.CloseSubpath) != 0)
                {
                    if (!closed)
                    {
                        data += SVG.VALUE_Z;
                        closed = true;
                    }
                }
            }

            return data;
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Parses the string.
        /// </summary>
        /// <param name="value">The value.</param>
        private void ParseString(string value)
        {
            ArrayList points = new ArrayList();
            ArrayList types = new ArrayList();

            MatchCollection mchdt = DataRegex.Matches(value);

            for (int i = 0, c = mchdt.Count; i < c; i++)
            {
                string type = mchdt[i].Groups["type"].Value;

                switch (type.ToUpper())
                {
                    case SVG.VALUE_M:
                        #region Case VALUE_M
                        {
                            PointsArray mpa = new PointsArray(mchdt[i].Groups["numbers"].Value);
                            bool first = true;

                            for (int j = 0, cj = mpa.Points.Length; j < cj; j++)
                            {
                                points.Add(mpa.Points[j]);
                                types.Add(first ? (byte)0 : (byte)1);
                                first = false;
                            }

                            break;
                        }
                        #endregion

                    case SVG.VALUE_L:
                        #region Case VALUE_L
                        {
                            PointsArray lpa = new PointsArray(mchdt[i].Groups["numbers"].Value);

                            for (int j = 0, cj = lpa.Points.Length; j < cj; j++)
                            {
                                points.Add(lpa.Points[j]);
                                types.Add((byte)1);
                            }

                            break;
                        }
                        #endregion

                    case SVG.VALUE_H:
                        #region case VALUE_H
                        {
                            FloatArray hfa = new FloatArray(mchdt[i].Groups["numbers"].Value);

                            for (int j = 0, cj = hfa.Array.Length; j < cj; j++)
                            {
                                points.Add(new PointF(hfa.Array[j], ((PointF)points[points.Count - 1]).Y));
                                types.Add((byte)1);
                            }

                            break;
                        }
                        #endregion

                    case SVG.VALUE_V:
                        #region Case VALUE_V
                        {
                            FloatArray vfa = new FloatArray(mchdt[i].Groups["numbers"].Value);

                            for (int j = 0, cj = vfa.Array.Length; j < cj; j++)
                            {
                                points.Add(new PointF(((PointF)points[points.Count - 1]).X,
                                    vfa.Array[j]));
                                types.Add((byte)1);
                            }

                            break;
                        }
                        #endregion

                    case SVG.VALUE_C:
                        #region Case VALUE_C
                        {
                            PointsArray lpa = new PointsArray(mchdt[i].Groups["numbers"].Value);

                            for (int j = 0, cj = lpa.Points.Length; j < cj; j++)
                            {
                                points.Add(lpa.Points[j]);
                                types.Add((byte)3);
                            }

                            break;
                        }
                        #endregion

                    case SVG.VALUE_Z:
                        #region Case VALUE_Z
                        {
                            byte last = (byte)types[types.Count - 1];
                            types.RemoveAt(types.Count - 1);
                            types.Add((byte)(128 | last));
                            break;
                        }
                        #endregion
                }
            }

            m_data = new PathData();
            m_data.Points = (PointF[])points.ToArray(typeof(PointF));
            m_data.Types = (byte[])types.ToArray(typeof(byte));
            m_path = new GraphicsPath(m_data.Points, m_data.Types);
        }
        #endregion
    }
}
