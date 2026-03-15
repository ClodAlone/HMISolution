#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;

using System.Globalization;
using System.Collections.Generic;

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// Utility class to store information about Image transaction or scale matrix.
    /// </summary>
#if NETFX_CORE || WP
    public class PdfMatrix
#else
    internal class PdfMatrix
#endif
    {
        #region Fields
        /// <summary>
        /// Local variable to store content stream.
        /// </summary>
        private PdfReader m_contentStream = null;

        /// <summary>
        /// Local variable to store the key value.
        /// </summary>
        private string m_key = null;

        /// <summary>
        /// Local variable to store the width.
        /// </summary>
        private float m_width;

        /// <summary>
        /// Local variable to store the height.
        /// </summary>
        private float m_height;

        /// <summary>
        /// Local variable to store the X co-ordinate value.
        /// </summary>
        private float m_x;

        /// <summary>
        /// Local variable to store the Y co-ordinate value.
        /// </summary>
        private float m_y;

        /// <summary>
        /// Local variable to store the transaction matrix value.
        /// </summary>
        private float[] m_translationMatrix = null; //new float[6];

        /// <summary>
        /// Local variable to store the scale matrix value.
        /// </summary>
        private float[] m_scaleMatrix = null;// new float[6];

        /// <summary>
        /// Local variable to store the page size.
        /// </summary>
        private SizeF m_pageSize;

        /// <summary>
        /// Local variable to store the cm entries.
        /// </summary>
        internal List<string> m_token = new List<string>();

        /// <summary>
        /// Local variable to store the page margin cm.
        /// </summary>
        string m_marginToken = string.Empty;

        /// <summary>
        /// Local variable to store current Rectangle cm..
        /// </summary>
        string m_rectToken = string.Empty;

        /// <summary>
        /// Local variable to store Previous Rectangle cm.
        /// </summary>
        string m_prevRectToken = string.Empty;

        /// <summary>
        /// Local variable to store eftMargin.
        /// </summary>
        float m_leftMargin = 0.0f;

        /// <summary>
        /// Local variable to store topMargin.
        /// </summary>
        float m_topMargin = 0.0f;
        internal RectangleF m_scaledBounds = new RectangleF();
        #endregion

        #region constructor
        /// <summary>
        ///  Initializes a new instance of the <see cref="T:PdfMatrix"/> class.
        /// </summary>
        public PdfMatrix()
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="T:PdfMatrix"/> class.
        /// </summary>
        /// <param name="ContentStream">Content Stream.</param>
        /// <param name="key">Key Value.</param>
        /// <param name="pageSize">Page Size.</param>
        public PdfMatrix(PdfReader ContentStream, string key, SizeF pageSize)
        {
            m_contentStream = ContentStream;
            m_key = key;
            m_pageSize = pageSize;
            List<string> matrixString = MatrixCalculation();

            ConvertToArray(matrixString);

            if (m_token.Count == 2)
            {
                if (m_translationMatrix != null)
                    SetTranslationMatrix();
                else 
                    SetScaleMatrix(pageSize);
            }

        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the X co-ordinate value.
        /// </summary>
        public float GetScaleX
        {
            get
            {
                return m_x;
            }
        }

        /// <summary>
        /// Gets the Y co-ordinate value.
        /// </summary>
        public float GetScaleY
        {
            get
            {
                return m_y;
            }
        }

        /// <summary>
        /// Gets the image height
        /// </summary>
        public float GetHeight
        {
            get
            {
                return m_height;
            }
        }

        /// <summary>
        /// Gets the image width value.
        /// </summary>
        public float GetWidth
        {
            get
            {
                return m_width;
            }
        }
        /// <summary>
        /// gets the image leftmargin
        /// </summary>
        public float LeftMargin
        {
            get
            {
                return m_leftMargin;
            }
        }
        /// <summary>
        /// Gets the top margin
        /// </summary>
        public float TopMargin
        {
            get
            {
                return m_topMargin;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calculate the matrix value from the content stream.
        /// </summary>
        /// <returns>The matrix value in the form of array list.</returns>
        private List<string> MatrixCalculation()
        {
            m_contentStream.Position = 0;
            string currentToken = m_contentStream.ReadLine();// m_contentStream.ReadStream();
            string nullToken = string.Empty;
            bool checkMargin = true;
            bool start = false;
            bool end = false;
            bool check = true;

            while (currentToken != string.Empty || m_contentStream.Stream.Position < m_contentStream.Stream.Length)
            {
                if (currentToken.Contains(" re"))
                {
                    if (m_rectToken != string.Empty)
                    {
                        m_prevRectToken = m_rectToken;
                    }
                    m_rectToken = currentToken;
                }

                if (currentToken.Contains("q"))
                {
                    if (check == false)
                    {
                        start = true;
                    }
                }

                if (currentToken.Contains("Q"))
                {
                    end = true;
                    m_token.Clear();
                }

                if (start == true)
                {
                    if (currentToken.Contains(" cm"))
                    {
                        if (m_token.Count == 0)
                        {
                            string[] element = (currentToken.Split(new string[] { " ", "q" }, StringSplitOptions.RemoveEmptyEntries));
                            float a, b, c, d, e, f;
                            if (element.Length >= 6)
                            {
                                float.TryParse(element[0], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out a);
                                float.TryParse(element[1], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out b);
                                float.TryParse(element[2], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out c);
                                float.TryParse(element[3], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out d);
                                float.TryParse(element[4], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out e);
                                float.TryParse(element[5], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out f);
                                if (a != 1 || d != 1)
                                {
                                    if (nullToken != null)
                                    {
                                        string[] previousElement = (nullToken.Split(new string[] { " ", "q" }, StringSplitOptions.RemoveEmptyEntries));
                                        float a1, b1, c1, d1, e1, f1;
                                        if (previousElement.Length >= 6)
                                        {
                                            float.TryParse(previousElement[0], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out a1);
                                            float.TryParse(previousElement[1], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out b1);
                                            float.TryParse(previousElement[2], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out c1);
                                            float.TryParse(previousElement[3], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out d1);
                                            float.TryParse(previousElement[4], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out e1);
                                            float.TryParse(previousElement[5], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out f1);
                                            if (a1 != 1 || d1 != 1)
                                            {
                                                float width = a1 * a;
                                                float height = d1 * d;
                                                m_scaledBounds = new RectangleF(e, f, width, height);
                                            }
                                        }
                                    }
                                }
                            }
                            m_token.Add(currentToken);
                        }
                        else
                        {
                            m_token.Add(currentToken);
                        }
                    }
                }

                check = false;
                if (currentToken.Contains(" cm"))
                {
                    string[] element = (currentToken.Split(new string[] { " ", "q" }, StringSplitOptions.RemoveEmptyEntries));
                    float a;
                    float.TryParse(element[0], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out a);
                    if (a != 1.0)
                    {
                        if (!start)
                            m_token.Add(currentToken);

                    }
                    else if (m_token.Count == 0)
                    {
                        nullToken = currentToken;
                    }
                }

                if (checkMargin == false)
                {
                    m_marginToken = currentToken;
                }

                if (currentToken.Contains("Translate co-ordinate system"))
                {
                    checkMargin = false;
                }
                else
                {
                    checkMargin = true;
                }
                currentToken = m_contentStream.ReadLine();// m_contentStream.ReadStream();
                if (currentToken == string.Empty)
                {
                    if (m_token.Count == 0)
                    {
                        currentToken = m_contentStream.ReadLine();// m_contentStream.ReadStream();
                    }
                }
                if (currentToken.Contains(m_key))
                {
                    break;
                }
            }

            if (m_token.Count == 0)
            {
                if (nullToken.Length > 0)
                {
                    m_token.Add(nullToken);
                }
            }
            return m_token;
        }
           
        /// <summary>
        /// Calculate the scale matrix value.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        private void SetScaleMatrix(SizeF pageSize)
        {
            float tempV = m_scaleMatrix[0];
            m_x = m_scaleMatrix[4];
            if (pageSize.Width > pageSize.Height)
            {
                m_y = float.Parse(m_scaleMatrix[5].ToString());
            }
            else
            {
                m_y = (tempV + float.Parse(m_scaleMatrix[5].ToString()) > pageSize.Height ? tempV + float.Parse(m_scaleMatrix[5].ToString()) - pageSize.Height : float.Parse(m_scaleMatrix[5].ToString()));
            }
            m_width = m_scaleMatrix[0];
            if (!(m_scaleMatrix[1] == 0.0f && m_scaleMatrix[3] == 0.0f))
            {
                m_width = (float)Math.Sqrt(Math.Pow((double)m_scaleMatrix[0], 2) +
                                          Math.Pow((double)m_scaleMatrix[1], 2));
            }

            m_height = m_scaleMatrix[4];
            if (!(m_scaleMatrix[1] == 0.0f && m_scaleMatrix[3] == 0.0f))
            {
                m_height = (float)Math.Sqrt(Math.Pow((double)m_scaleMatrix[3], 2) +
                                          Math.Pow((double)m_scaleMatrix[4], 2));
            }

            m_x += m_leftMargin;
            m_y += m_topMargin;
        }

        /// <summary>
        /// Calculate the translation matrix value.
        /// </summary>
        private void SetTranslationMatrix()
        {
            m_x = float.Parse(m_translationMatrix[4].ToString());
            m_y = float.Parse(m_translationMatrix[5].ToString());

            if (m_scaleMatrix != null)
            {
                m_width = m_scaleMatrix[0];
                if (!(m_scaleMatrix[1] == 0.0f && m_scaleMatrix[3] == 0.0f))
                {
                    m_width = (float)Math.Sqrt(Math.Pow(float.Parse(m_scaleMatrix[0].ToString()), 2) +
                                              Math.Pow(float.Parse(m_scaleMatrix[1].ToString()), 2));
                }
              
                m_height = m_scaleMatrix[4];
                if (!(m_scaleMatrix[1] == 0.0f && m_scaleMatrix[3] == 0.0f))
                {
                    m_height = (float)Math.Sqrt(Math.Pow(m_scaleMatrix[3], 2) +
                                              Math.Pow(m_scaleMatrix[4], 2));
                }
            }
            else
            {
                m_width = -1;
                m_height = -1;
            }
            m_x += m_leftMargin;
            m_y += m_topMargin;
            if (m_y >= m_height)
            {
                m_y = m_y - m_height;
            }
        }

        /// <summary>
        /// Convert the matrix value as array.
        /// </summary>
        /// <param name="matrixString">Matrix value.</param>
        private void ConvertToArray(List<string> matrixString)
        {
            if (m_token.Count > 0)
            {
               string tokenBounds = m_token[0].ToString();
                float X, Y;
                float width, height;
                if (m_marginToken.Length > 0)
                {
                    string[] margintoken = m_marginToken.Split(new string[] { "cm" }, StringSplitOptions.None);
                    string[] marginRectangle = margintoken[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


                    m_leftMargin = float.Parse(marginRectangle[4].ToString(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
                    m_topMargin = float.Parse(marginRectangle[5].ToString(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture); 
                    m_leftMargin = Math.Abs(m_leftMargin);
                    m_topMargin = Math.Abs(m_topMargin);
                }

                if (m_token.Count == 2)
                {
                    foreach (string strValue in matrixString)
                    {
                        // Convert the Co-ordinate value as PointF
                        string[] token = strValue.Split(new string[] { "cm" }, StringSplitOptions.None);
                        string[] rectangle = token[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (rectangle[0] == "q")
                        {
                            for (int j = 0; j < rectangle.Length - 1; j++)
                            {
                                rectangle[j] = rectangle[j + 1];
                            }
                        }
                        switch (rectangle[0])
                        {
                            case "1.00":
                                // Translation Matrix
                                m_translationMatrix = new float[rectangle.Length];
                                for (int i = 0; i < rectangle.Length; i++)
                                    m_translationMatrix[i] = float.Parse(rectangle[i], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
                                break;
                           
                            case "1":
                                // Translation Matrix
                                m_translationMatrix = new float[rectangle.Length];
                                for (int i = 0; i < rectangle.Length; i++)
                                    m_translationMatrix[i] = float.Parse(rectangle[i], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
                                break;

                            default:
                                // Scaling Matrix.
                                if (m_scaleMatrix==null)
                                {
                                    m_scaleMatrix = new float[rectangle.Length];
                                    for (int i = 0; i < rectangle.Length; i++)
                                        m_scaleMatrix[i] = float.Parse(rectangle[i], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    if (m_scaleMatrix[0] != 1)
                                    {
                                        for (int i = 0; i < rectangle.Length; i++)
                                            m_scaleMatrix[i] = m_scaleMatrix[i] * float.Parse(rectangle[i], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        for (int i = 0; i < rectangle.Length; i++)
                                            m_scaleMatrix[i] = float.Parse(rectangle[i], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
                        
                                    }
                                }
                                break;
                        }
                    }
                }

                else if (m_token.Count == 1)
                {
                    string[] token = tokenBounds.Split(new string[] { "cm" }, StringSplitOptions.None);
                    string[] rectangle = token[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (rectangle.Length > 0)
                    {
                        if (rectangle[0] == "q")
                        {
                            for (int j = 0; j < rectangle.Length - 1; j++)
                            {
                                rectangle[j] = rectangle[j + 1];
                            }
                        }

                        float.TryParse(rectangle[4].ToString(),System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out X);
                        float.TryParse(rectangle[5].ToString(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out Y);
                        float.TryParse(rectangle[0].ToString(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out width);
                        float.TryParse(rectangle[3].ToString(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out height);

                        if (Y > 0)
                        {
                            Y = m_pageSize.Height - Y;
                        }
                        RectangleF bounds = new RectangleF(new PointF(Math.Abs(X), Math.Abs(Y)), new SizeF(Math.Abs(width), Math.Abs(height)));
                        if (bounds.Y >= bounds.Height)
                        {
                            bounds.Y = bounds.Y - bounds.Height;
                        }
                        bounds.X += m_leftMargin;
                        bounds.Y += m_topMargin;
                        if (m_rectToken.Length > 0)
                        {
                            string[] retoken = m_rectToken.Split(new string[] { "cm" }, StringSplitOptions.None);
                            string[] reRectangle = retoken[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            // bounds.Y += float.Parse(reRectangle[3].ToString());
                        }
                        m_x = bounds.X;
                        m_y = bounds.Y;
                        m_height = bounds.Height;
                        m_width = bounds.Width;
                    }
                }
            }
        }
        #endregion   
    }
}
