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

namespace Syncfusion.Windows.Forms.Chart.PostScript
{
    /// <summary>
    /// Represents the post script linear color function.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class LinearColorFunction : PostScriptDictionary
    {
        #region Members
        private const string BaseName = "LinearFunc";
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearColorFunction"/> class.
        /// </summary>
        /// <param name="c1">The color1.</param>
        /// <param name="c2">The color2.</param>
        public LinearColorFunction(Color c1, Color c2)
        {
            InternalTable.Add("/FunctionType", 2);
            InternalTable.Add("/Domain", "[ 0 1 ]");
            InternalTable.Add("/Range", "[ 0 1 0 1 0 1 ]");
            InternalTable.Add("/N", "1");

            StringBuilder sb = new StringBuilder(100, 200);
            sb.Append("[");
            sb.Append(" ");
            sb.Append((c1.R / 255.0f).ToString().Replace(",", "."));
            sb.Append(" ");
            sb.Append((c1.G / 255.0f).ToString().Replace(",", "."));
            sb.Append(" ");
            sb.Append((c1.B / 255.0f).ToString().Replace(",", "."));
            sb.Append(" ");
            sb.Append("]");

            InternalTable.Add("/C0", sb.ToString());

            sb = new StringBuilder(100, 200);

            sb.Append("[");
            sb.Append(" ");
            sb.Append((c2.R / 255.0f).ToString().Replace(",", "."));
            sb.Append(" ");
            sb.Append((c2.G / 255.0f).ToString().Replace(",", "."));
            sb.Append(" ");
            sb.Append((c2.B / 255.0f).ToString().Replace(",", "."));
            sb.Append(" ");
            sb.Append("]");

            InternalTable.Add("/C1", sb.ToString());
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get
            {
                return BaseName + base.Name;
            }
        }
    }

    /// <summary>
    /// Represents the post script sampled color function.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class SampledColorFunction : PostScriptDictionary // this function is not working proprly
    {
        #region Members

        private const string BaseName = "SampledFunc";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SampledColorFunction"/> class.
        /// </summary>
        /// <param name="crs">The color array..</param>
        public SampledColorFunction(Color[] crs)
        {
            InternalTable.Add("/FunctionType", 0);
            InternalTable.Add("/Domain", "[ 0 1 ]");
            InternalTable.Add("/Range", "[ 0 1 0 1 0 1 ]");

            InternalTable.Add("/BitsPerSample", "8");
            ////InternalTable.Add("/Decode", "[ 0 255 0 255 0 255 ]");

            ////number of samples per color
            StringBuilder sb = new StringBuilder(100, 200);
            sb.Append("[");
            sb.Append(" ");
            sb.Append(crs.Length.ToString().Replace(",", "."));
            sb.Append(" ");
            sb.Append(crs.Length.ToString().Replace(",", "."));
            sb.Append(" ");
            sb.Append(crs.Length.ToString().Replace(",", "."));
            sb.Append(" ");
            sb.Append("]");
            InternalTable.Add("/Size", sb.ToString());

            //// data source
            ////StringBuilder sb2 = new StringBuilder(100, 200);
            ////sb2.Append("(");
            ////for (int i = 0; i < clrs.Length; i++)
            ////{
            ////  sb2.Append(@"\" + clrs[i].R);
            ////  sb2.Append(@"\" + clrs[i].G);
            ////  sb2.Append(@"\" + clrs[i].B);
            ////}
            ////sb2.Append(")");
            ////InternalTable.Add("/DataSource", sb2.ToString());

            StringBuilder sb2 = new StringBuilder(100, 200);
            sb2.Append("<");
            for (int i = 0; i < crs.Length; i++)
            {
                sb2.Append(crs[i].R.ToString("x").ToUpper());
                sb2.Append(crs[i].G.ToString("x").ToUpper());
                sb2.Append(crs[i].B.ToString("x").ToUpper());
            }

            sb2.Append(">");
            InternalTable.Add("/DataSource", sb2.ToString());
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get
            {
                return BaseName + base.Name;
            }
        }
    }

    /// <summary>
    /// Represents the post script stitching color function.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class StitchingColorFunction : PostScriptDictionary
    {
        #region Members

        private const string BaseName = "StitchFunc";
        private ArrayList functionsArrayList = new ArrayList(10);

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="StitchingColorFunction"/> class.
        /// </summary>
        /// <param name="clrs">The color array.</param>
        public StitchingColorFunction(Color[] clrs)
        {
            InternalTable.Add("/FunctionType", 3);
            InternalTable.Add("/Domain", "[ 0 1 ]");
            InternalTable.Add("/Range", "[ 0 1 0 1 0 1 ]");
            int k = clrs.Length - 1;

            ////Encode
            StringBuilder sb = new StringBuilder(100, 200);
            sb.Append("[ ");
            for (int i = 0; i < k; i++)
            {
                sb.Append("0 1 ");
            }

            sb.Append("]");
            InternalTable.Add("/Encode", sb.ToString());

            ////bounds
            sb = new StringBuilder(100, 200);
            sb.Append("[ ");
            for (int i = 0; i < k - 1; i++)
            {
                sb.Append(((i + 1) / (double)k).ToString().Replace(",", ".") + " ");
            }

            sb.Append("]");
            InternalTable.Add("/Bounds", sb.ToString());

            PostScriptArray array = new PostScriptArray();
            for (int i = 0; i < clrs.Length - 1; i++)
            {
                array.List.Add(new LinearColorFunction(clrs[i], clrs[i + 1]));
            }

            InternalTable.Add("/Functions", array);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get
            {
                return BaseName + base.Name;
            }
        }
    }
}
