#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.ReportWriter
{

    /// <summary>
    /// Defines the support for RDL and RDLC in Reportviewer
    /// </summary>
    /// <remarks></remarks>
    public enum ProcessingMode
    {
        /// <summary>
        /// ProcessingMode If the Report is RDLC
        /// </summary>
        /// <remarks></remarks>
        Remote,
        /// <summary>
        /// ProcessingMode If the Report is RDL
        /// </summary>
        /// <remarks></remarks>
        Local
    }

    /// <summary>
    /// Defines supported excel version types.
    /// </summary>
    /// <remarks></remarks>
    public enum ExcelVersion
    {
        /// <summary>
        /// Represents excel version 97-2003
        /// </summary>
        /// <remarks></remarks>
        Excel97to2003 = 0,
        /// <summary>
        /// Represents excel version Excel2007
        /// </summary>
        /// <remarks></remarks>
        Excel2007 = 1,
        /// <summary>
        /// Represents excel version Excel2010
        /// </summary>
        /// <remarks></remarks>
        Excel2010 = 2,
        /// <summary>
        /// Represents excel version Excel2013
        /// </summary>
        /// <remarks></remarks>
        Excel2013 = 3,
    }

    /// <summary>
    /// Defines supported WordFormatType
    /// </summary>
    /// <remarks></remarks>
    public enum WordFormatType
    {
        /// <summary>
        /// Microsoft Word file format.
        /// </summary>
        /// <remarks></remarks>
        Doc = 0,

        /// <summary>
        /// Microsoft Word document template.
        /// </summary>
        /// <remarks></remarks>
        Dot = 1,
        /// <summary>
        /// Microsoft Word 2007 file format.
        /// </summary>
        /// <remarks></remarks>
        Docx = 2,
        /// <summary>
        /// Microsoft Word 2007 file format.
        /// </summary>
        /// <remarks></remarks>
        Word2007 = 3,
        /// <summary>
        /// Microsoft Word 2010 file format.
        /// </summary>
        /// <remarks></remarks>
        Word2010 = 4,
        /// <summary>
        /// Microsoft Word 2013 file format.
        /// </summary>
        /// <remarks></remarks>
        Word2013 = 5,
        /// <summary>
        /// Microsoft Word 2007 Template format.
        /// </summary>
        /// <remarks></remarks>
        Word2007Dotx = 6,
        /// <summary>
        /// Microsoft Word 2010 Template format.
        /// </summary>
        /// <remarks></remarks>
        Word2010Dotx = 7,
        /// <summary>
        /// Microsoft Word 2013 Template format.
        /// </summary>
        /// <remarks></remarks>
        Word2013Dotx = 8,
        /// <summary>
        /// Microsoft Word 2007 macro enabled file format.
        /// </summary>
        /// <remarks></remarks>
        Word2007Docm = 9,
        /// <summary>
        /// Microsoft Word 2010 macro enabled file format.
        /// </summary>
        /// <remarks></remarks>
        Word2010Docm = 10,
        /// <summary>
        /// Microsoft Word 2013 macro enabled file format.
        /// </summary>
        /// <remarks></remarks>
        Word2013Docm = 11,
        /// <summary>
        /// Microsoft Word 2007 macro enabled template format.
        /// </summary>
        /// <remarks></remarks>
        Word2007Dotm = 12,
        /// <summary>
        /// Microsoft Word 2010 macro enabled template format.
        /// </summary>
        /// <remarks></remarks>
        Word2010Dotm = 13,
        /// <summary>
        /// Microsoft Word 2013 macro enabled template format.
        /// </summary>
        /// <remarks></remarks>
        Word2013Dotm = 14,
        /// <summary>
        /// Rtf format
        /// </summary>
        /// <remarks></remarks>
        Rtf = 15,
        /// <summary>
        /// Text file format.
        /// </summary>
        /// <remarks></remarks>
        Txt = 16,
        /// <summary>
        /// E-book format.
        /// </summary>
        /// <remarks></remarks>
        EPub = 17,
        /// <summary>
        /// Html format.
        /// </summary>
        /// <remarks></remarks>
        Html = 18,

        /// <summary>
        /// Xml file format.
        /// </summary>
        /// <remarks></remarks>
        Xml = 19,
        /// <summary>
        /// Support all Format Types.
        /// </summary>
        /// <remarks></remarks>
        Automatic = 20,
    }
#if SILVERLIGHT
    /// <summary>
    /// Defines the Export mode for the Report
    /// </summary>
    /// <remarks></remarks>
    public enum ExportMode
    {
        /// <summary>
        /// If export from Local 
        /// </summary>
        /// <remarks></remarks>
        Local,
        /// <summary>
        /// If export from Server
        /// </summary>
        /// <remarks></remarks>
        Server
    }
#endif

}
