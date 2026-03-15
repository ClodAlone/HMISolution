#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Class that represents values of attributes in tag elements.
    /// </summary>
    internal sealed class AttributeValue
    {
        #region Class constants
        /// <summary>
        /// Value of the tag attribute.
        /// </summary>
        public const string JScript = "jscript";

        /// <summary>
        /// Value of the tag attribute.
        /// </summary>
        public const string JavaScript = "javascript";

        /// <summary>
        /// Value of the tag attribute.
        /// </summary>
        public const string Vbs = "vbs";

        /// <summary>
        /// Value of the tag attribute.
        /// </summary>
        public const string VBScript = "vbscript";

        /// <summary>
        /// Value of the tag attribute.
        /// </summary>
        public const string Csh = "c#";

        /// <summary>
        /// Value of the tag attribute.
        /// </summary>
        public const string Csharp = "csharp";
        #endregion

        /// <summary>
        /// Prevents a default instance of the AttributeValue class from being created
        /// </summary>
        private AttributeValue()
        {
            throw new NotImplementedException();
        }
    }
}
