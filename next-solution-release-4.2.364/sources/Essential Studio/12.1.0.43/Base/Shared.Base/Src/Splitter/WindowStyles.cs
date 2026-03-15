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

namespace Syncfusion.Windows.Forms
{
	[Syncfusion.Documentation.DocumentationExclude()]
    internal enum WS: long
    {
        OVERLAPPED       = 0x00000000,
        POPUP            = 0x80000000,
        CHILD            = 0x40000000,
        MINIMIZE         = 0x20000000,
        VISIBLE          = 0x10000000,
        DISABLED         = 0x08000000,
        CLIPSIBLINGS     = 0x04000000,
        CLIPCHILDREN     = 0x02000000,
        MAXIMIZE         = 0x01000000,
        CAPTION          = 0x00C00000,     
        BORDER           = 0x00800000,
        DLGFRAME         = 0x00400000,
        VSCROLL          = 0x00200000,
        HSCROLL          = 0x00100000,
        SYSMENU          = 0x00080000,
        THICKFRAME       = 0x00040000,
        GROUP            = 0x00020000,
        TABSTOP          = 0x00010000,
        EX_DLGMODALFRAME     = 0x00000001,
        EX_NOPARENTNOTIFY    = 0x00000004,
        EX_TOPMOST           = 0x00000008,
        EX_ACCEPTFILES       = 0x00000010,
        EX_TRANSPARENT       = 0x00000020,
        EX_MDICHILD          = 0x00000040,
        EX_TOOLWINDOW        = 0x00000080,
        EX_WINDOWEDGE        = 0x00000100,
        EX_CLIENTEDGE        = 0x00000200,
        EX_CONTEXTHELP       = 0x00000400,
        EX_RIGHT             = 0x00001000,
        EX_LEFT              = 0x00000000,
        EX_RTLREADING        = 0x00002000,
        EX_LTRREADING        = 0x00000000,
        EX_LEFTSCROLLBAR     = 0x00004000,
        EX_RIGHTSCROLLBAR    = 0x00000000,
        EX_CONTROLPARENT     = 0x00010000,
        EX_STATICEDGE        = 0x00020000,
        EX_APPWINDOW         = 0x00040000,
        EX_LAYERED           = 0x00080000
    };

    
}
