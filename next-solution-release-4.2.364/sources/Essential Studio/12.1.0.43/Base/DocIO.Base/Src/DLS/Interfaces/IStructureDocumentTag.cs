#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;


namespace Syncfusion.DocIO.DLS
{
    internal interface IStructureDocumentTag: ICompositeEntity 
    {
        SDTContent SDTContent
        {
            get;
        }

        SDTProperties SDTProperties
        {
            get;
        }
        WCharacterFormat BreakCharacterFormat
        {
            get;
        }
    }

    internal interface IStructureDocumentTagBlock : ICompositeEntity
    {
        SDTBlockContent SDTContent
        {
            get;
        }
        SDTProperties SDTProperties
        {
            get;
        }
        WCharacterFormat BreakCharacterFormat
        {
            get;
        }
    }

    internal interface IStructureDocumentTagInline : IParagraphItem
    {
        SDTInlineContent SDTContent
        {
            get;
        }
        SDTProperties SDTProperties
        {
            get;
        }
        WCharacterFormat BreakCharacterFormat
        {
            get;
        }
    }

    internal interface IStructureDocumentTagRow
    {
        SDTRowContent SDTContent
        {
            get;
        }
        SDTProperties SDTProperties
        {
            get;
        }
        WCharacterFormat BreakCharacterFormat
        {
            get;
        }
    }
    internal interface IStructureDocumentTagCell
    {
        SDTCellContent SDTContent
        {
            get;
        }
        SDTProperties SDTProperties
        {
            get;
        }
        WCharacterFormat BreakCharacterFormat
        {
            get;
        }
    }
}
