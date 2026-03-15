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

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Enumerates types of the xmp structure.
    /// </summary>
    public enum XmpStructureType
    {
        /// <summary>
        /// A structure containing dimensions for a drawn object.
        /// </summary>
        Dimensions,

        /// <summary>
        /// A structure containing the characteristics of a font used in a document.
        /// </summary>
        Font,

        /// <summary>
        /// A structure containing the characteristics of a Coloring (swatch) used in a document.
        /// </summary>
        Colorant,

        /// <summary>
        /// A thumbnail image for a file.
        /// </summary>
        Thumbnail,

        /// <summary>
        /// Job structure.
        /// </summary>
        Job
    }

    /// <summary>
    /// Enumerates types of the xmp schema.
    /// </summary>
    public enum XmpSchemaType
    {
        /// <summary>
        /// Dublin Core Schema.
        /// </summary>
        DublinCoreSchema,

        /// <summary>
        /// Basic Schema.
        /// </summary>
        BasicSchema,

        /// <summary>
        /// Rights Management Schema.
        /// </summary>
        RightsManagementSchema,

        /// <summary>
        /// Basic Job Ticket Schema.
        /// </summary>
        BasicJobTicketSchema,

        /// <summary>
        /// Paged Text Schema.
        /// </summary>
        PagedTextSchema,

        /// <summary>
        /// Adobe PDF Schema.
        /// </summary>
        PDFSchema,

        /// <summary>
        /// Custom schema.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Types of the xmp arrays.
    /// </summary>
    public enum XmpArrayType
    {
        /// <summary>
        /// Unknown array type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Unordered array.
        /// </summary>
        Bag,

        /// <summary>
        /// Ordered array.
        /// </summary>
        Seq,

        /// <summary>
        /// Alternative array.
        /// </summary>
        Alt
    }
}
