//-------------------------------------------------------------------------------------------------
// <copyright file="GridStylesParser.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System.Xml;
using System.Xml.Serialization;
using System;
using System.Collections;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid.Design
{
    /// <summary>
    /// Description for GridStylesParser.
    /// </summary>
    internal class GridStylesParser
    {
        public GridStylesParser()
        {
        }

        /// <summary>
        ///     This method will parse the supplied StyleInfoStore for high cost properties, grouping them together for serialization purposes.
        ///     Currently, this is limited to the BackroundImage property.
        /// </summary>
        /// <param name="store" type="Syncfusion.Styles.StyleInfoStore">
        ///     <para>
        ///            The StyleInfoStore object to parse related properties.   
        ///     </para>
        /// </param>
        public static void ParsePropertyStore(StyleInfoStore store)
        {
            ////this implementation goes to individual properties directly
            StyleInfoProperty sip = store.FindStyleInfoProperty("BackgroundImage");
            object val = store.GetValue(sip);
            if (val != null)
            {
                byte[] bytes = GridSyncProperties.GetImageBytes(val as System.Drawing.Bitmap);
                string base64 = Convert.ToBase64String(bytes);
                string id = GridDesignerMain.syncProps.StoredImages.Add(base64);
                sip = store.FindStyleInfoProperty("BackgroundImageID");

                if (sip != null)
                {
                    store.SetValue(sip, id);
                }
            }
        }
    }
}
