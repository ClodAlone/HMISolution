#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.UI.Xaml.Collections.ComponentModel
{
    // Summary:
    //     Specifies that this object supports a simple, transacted notification for
    //     batch initialization.
    internal interface ISupportInitialize
    {
        // Summary:
        //     Signals the object that initialization is starting.
        void BeginInit();
        //
        // Summary:
        //     Signals the object that initialization is complete.
        void EndInit();
    }
}
