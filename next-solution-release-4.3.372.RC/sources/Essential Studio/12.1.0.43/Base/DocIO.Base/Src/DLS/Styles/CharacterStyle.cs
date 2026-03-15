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

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represent a style of character range.
    /// </summary>
    internal class CharacterStyle
      : Style
    {
        #region Properties
        /// <summary>
        /// Gets the base style.
        /// </summary>
        new public CharacterStyle BaseStyle
        {
            get
            {
                return base.BaseStyle as CharacterStyle;
            }
        }
        /// <summary>
        /// Gets the type of the style.
        /// </summary>
        /// <value>The type of the style.</value>
        public override StyleType StyleType
        {
            get
            {
                return StyleType.CharacterStyle;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterStyle"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal CharacterStyle(WordDocument doc)
            : base(doc)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Clones itself
        /// </summary>
        /// <returns></returns>
        public override IStyle Clone()
        {
            return (IStyle)CloneImpl();
        }
        #endregion
    }
}
