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

namespace Syncfusion.HtmlConverter.Natives
{
    internal enum OLECMDID
    {
        OLECMDID_ALLOWUILESSSAVEAS = 0x2e,
        OLECMDID_CLEARSELECTION = 0x12,
        OLECMDID_CLOSE = 0x2d,
        OLECMDID_COPY = 12,
        OLECMDID_CUT = 11,
        OLECMDID_DELETE = 0x21,
        OLECMDID_DONTDOWNLOADCSS = 0x2f,
        OLECMDID_ENABLE_INTERACTION = 0x24,
        OLECMDID_FIND = 0x20,
        OLECMDID_FOCUSVIEWCONTROLS = 0x39,
        OLECMDID_FOCUSVIEWCONTROLSQUERY = 0x3a,
        OLECMDID_GETPRINTTEMPLATE = 0x34,
        OLECMDID_GETZOOMRANGE = 20,
        OLECMDID_HIDETOOLBARS = 0x18,
        OLECMDID_HTTPEQUIV = 0x22,
        OLECMDID_HTTPEQUIV_DONE = 0x23,
        OLECMDID_NEW = 2,
        OLECMDID_ONTOOLBARACTIVATED = 0x1f,
        OLECMDID_ONUNLOAD = 0x25,
        OLECMDID_OPEN = 1,
        OLECMDID_PAGEACTIONBLOCKED = 0x37,
        OLECMDID_PAGEACTIONUIQUERY = 0x38,
        OLECMDID_PAGESETUP = 8,
        OLECMDID_PASTE = 13,
        OLECMDID_PASTESPECIAL = 14,
        OLECMDID_PREREFRESH = 0x27,
        OLECMDID_PRINT = 6,
        OLECMDID_PRINT2 = 0x31,
        OLECMDID_PRINTPREVIEW = 7,
        OLECMDID_PRINTPREVIEW2 = 50,
        OLECMDID_PROPERTIES = 10,
        OLECMDID_PROPERTYBAG2 = 0x26,
        OLECMDID_REDO = 0x10,
        OLECMDID_REFRESH = 0x16,
        OLECMDID_SAVE = 3,
        OLECMDID_SAVEAS = 4,
        OLECMDID_SAVECOPYAS = 5,
        OLECMDID_SELECTALL = 0x11,
        OLECMDID_SETDOWNLOADSTATE = 0x1d,
        OLECMDID_SETPRINTTEMPLATE = 0x33,
        OLECMDID_SETPROGRESSMAX = 0x19,
        OLECMDID_SETPROGRESSPOS = 0x1a,
        OLECMDID_SETPROGRESSTEXT = 0x1b,
        OLECMDID_SETTITLE = 0x1c,
        OLECMDID_SHOWFIND = 0x2a,
        OLECMDID_SHOWMESSAGE = 0x29,
        OLECMDID_SHOWPAGEACTIONMENU = 0x3b,
        OLECMDID_SHOWPAGESETUP = 0x2b,
        OLECMDID_SHOWPRINT = 0x2c,
        OLECMDID_SHOWSCRIPTERROR = 40,
        OLECMDID_SPELL = 9,
        OLECMDID_STOP = 0x17,
        OLECMDID_STOPDOWNLOAD = 30,
        OLECMDID_UNDO = 15,
        OLECMDID_UPDATECOMMANDS = 0x15,
        OLECMDID_UPDATEPAGESTATUS = 0x30,
        OLECMDID_ZOOM = 0x13
    }

    internal enum OLECMDF
    {
        OLECMDF_DEFHIDEONCTXTMENU = 0x20,
        OLECMDF_ENABLED = 2,
        OLECMDF_INVISIBLE = 0x10,
        OLECMDF_LATCHED = 4,
        OLECMDF_NINCHED = 8,
        OLECMDF_SUPPORTED = 1
    }

    internal enum OLECMDEXECOPT
    {
        OLECMDEXECOPT_DODEFAULT,
        OLECMDEXECOPT_PROMPTUSER,
        OLECMDEXECOPT_DONTPROMPTUSER,
        OLECMDEXECOPT_SHOWHELP
    }

    internal enum tagREADYSTATE
    {
        READYSTATE_UNINITIALIZED,
        READYSTATE_LOADING,
        READYSTATE_LOADED,
        READYSTATE_INTERACTIVE,
        READYSTATE_COMPLETE
    }
}
