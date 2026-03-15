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

namespace Syncfusion.Windows.Forms.Chart.PostScript
{
    /// <summary>
    /// Represents the post script shading pattern dictionary.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class ShadingPatternDictionary : PostScriptDictionary
    {
        #region Members
        private const string BaseName = "ShdngPttrn";
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ShadingPatternDictionary"/> class.
        /// </summary>
        /// <param name="shading">The shading.</param>
        public ShadingPatternDictionary(string shading)
        {
            InternalTable.Add("/PatternType", 2);
            InternalTable.Add("/Shading", shading);
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
