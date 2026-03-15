// <copyright file="DecimalSeparatorCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Input
#else
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{


    /// <summary>
    ///  Contains collection of symbols which can be used as NumberDecimalSeparator or
    /// NegativeSign
    /// </summary>
    [ClassReference(IsReviewed = false,ShouldInclude=false)]
    public class DecimalSeparatorCollection
    {
        #region Variables

        internal Dictionary<String, String> shiftseparatorList = new Dictionary<string, string>();
        internal Dictionary<String, String> separatorList = new Dictionary<string, string>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DecimalSeparatorCollection"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DecimalSeparatorCollection(bool isazerty)
        {
            if (isazerty)
            {
                #region Shift Separator Numbers

                shiftseparatorList.Add("Number1", "!");
                shiftseparatorList.Add("Number2", "@");
                shiftseparatorList.Add("Number3", "#");
                shiftseparatorList.Add("Number4", "$");
                shiftseparatorList.Add("Number5", "%");
                shiftseparatorList.Add("Number6", "^");
                shiftseparatorList.Add("Number7", "&");
                shiftseparatorList.Add("Number8", "*");
                shiftseparatorList.Add("Number9", "(");
                shiftseparatorList.Add("Number0", ")");

                #endregion

                #region Shift Separator

                shiftseparatorList.Add("186", ":");
                shiftseparatorList.Add("187", "+");
                shiftseparatorList.Add("188", "<");
                shiftseparatorList.Add("189", "_");
                shiftseparatorList.Add("190", ">");
                shiftseparatorList.Add("191", "?");
                shiftseparatorList.Add("192", "~");
                shiftseparatorList.Add("220", "|");

                #endregion

                #region Separator

                separatorList.Add("186", ";");
                separatorList.Add("187", "=");
                separatorList.Add("188", ",");
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                separatorList.Add("189", "-");
#else
            separatorList.Add("142", "-");
#endif
                separatorList.Add("191", "/");
                separatorList.Add("192", "'");
                separatorList.Add("190", ".");

                #endregion
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            else
            {
                #region Shift Separator

                shiftseparatorList.Add("187", "+");
                shiftseparatorList.Add("188", "?");
                shiftseparatorList.Add("190", ".");
                shiftseparatorList.Add("191", "/");
                shiftseparatorList.Add("192", "%");

                #endregion

                #region Separator

                separatorList.Add("186", "$");
                separatorList.Add("187", "=");
                separatorList.Add("188", ",");
                separatorList.Add("190", ";");
                separatorList.Add("191", ":");
                separatorList.Add("219", ")");
                separatorList.Add("220", "*");
                separatorList.Add("221", "^");
                separatorList.Add("223", "!");
                separatorList.Add("Number1", "&");
                separatorList.Add("Number5", "(");
                separatorList.Add("Number6", "-");
                separatorList.Add("Number8", "_");

                #endregion
            }
#endif
        }
        #endregion
    }
}
