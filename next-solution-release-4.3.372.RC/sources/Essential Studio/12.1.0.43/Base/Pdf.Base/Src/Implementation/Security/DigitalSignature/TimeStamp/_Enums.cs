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

namespace Syncfusion.Pdf.Security
{
    [Flags()]
    internal enum PKIStatus
    {
        Granted= 0,
        GrantedWithMods=1,
        Rejection=2,
        Waiting=3,
        RevocationWarning=4,
        RevocationNotification=5,

    }
}
