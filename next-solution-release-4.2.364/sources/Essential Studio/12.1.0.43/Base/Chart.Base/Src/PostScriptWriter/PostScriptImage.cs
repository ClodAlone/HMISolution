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
using System.IO;
using System.Text;
using Syncfusion.Documentation;
using Syncfusion.Windows.Forms.Chart.PostScript;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the post script document.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class PostScriptImage
    {
        #region Constants
        private const string c_espHeader = "%!PS-Adobe-3.0 EPSF-3.0";
        private const string c_boundingBoxFormat = "%%BoundingBox: {0} {1} {2} {3}";
        #endregion

        #region Members
        private PostScriptDictionaryCollection m_dictionaries;
        private TextWriter m_writer;
        private RectangleF m_bounds;
        private MemoryStream m_bodyStream;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PostScriptImage"/> class.
        /// </summary>
        /// <param name="width">The width of image.</param>
        /// <param name="height">The height of image.</param>
        public PostScriptImage(float width, float height)
        {
            m_dictionaries = new PostScriptDictionaryCollection();
            m_bounds = new RectangleF(0, 0, width, height);
            m_bodyStream = new MemoryStream();
            m_writer = new StreamWriter(m_bodyStream);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the dictionaries.
        /// </summary>
        /// <value>The dictionaries.</value>
        public PostScriptDictionaryCollection Dictionaries
        {
            get
            {
                return m_dictionaries;
            }
        }

        /// <summary>
        /// Gets the size of image.
        /// </summary>
        /// <value>The size.</value>
        public SizeF Size
        {
            get
            {
                return m_bounds.Size;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <returns>Returns new PostScriptGraphics instance.</returns>
        public PostScriptGraphics GetGraphics()
        {
            return new PostScriptGraphics(m_writer, this);
        }

        /// <summary>
        /// Saves image to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Save(Stream stream)
        {
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine(c_espHeader);
            writer.WriteLine(c_boundingBoxFormat, m_bounds.X, m_bounds.Y, m_bounds.Width, m_bounds.Height);

            writer.WriteLine("%%BeginDocument");
            ////writer.WriteLine("%%BeginDocument");
            writer.Flush();

            foreach (PostScriptDictionary dict in m_dictionaries)
            {
                string s = "/" + dict.Name + " " + dict.ToPostScriptString() + " def";
                writer.Flush();

                for (int i = 0; i < s.Length; i++)
                {
                    writer.BaseStream.WriteByte((byte)s[i]);
                }

                writer.Flush();
                writer.WriteLine();
            }

            m_writer.Flush();
            writer.WriteLine();
            writer.Flush();
            m_bodyStream.WriteTo(stream);
            stream.Flush();
            writer.WriteLine("%%EndDocument");
            writer.Flush();
        }

        /// <summary>
        /// Saves image to the file.
        /// </summary>
        /// <param name="filename">The name of file.</param>
        public void Save(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                this.Save(fs);
            }
        }
        #endregion
    }
}
