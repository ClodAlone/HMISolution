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
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart.PostScript
{
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class LinearShadingDictionary : PostScriptDictionary
    {
        #region Constants
        private const string BaseName = "LinearShdng";
        #endregion

        #region properties

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
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearShadingDictionary"/> class.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="funcName">Name of the func.</param>
        public LinearShadingDictionary(PointF point1, PointF point2, string funcName)
        {
            InternalTable.Add("/ShadingType", 2);
            InternalTable.Add("/ColorSpace", "/DeviceRGB");
            InternalTable.Add("/Coords", "[ " + point1.X.ToString().Replace(",", ".") + " " +
                                                                                    point1.Y.ToString().Replace(",", ".") + " " +
                                                                                    point2.X.ToString().Replace(",", ".") + " " +
                                                                                    point2.Y.ToString().Replace(",", ".") + " ]");
            InternalTable.Add("/Function", funcName);
            InternalTable.Add("/Extend", "[ true true ]");
        }
        #endregion
    }
}
