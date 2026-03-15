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

#if !SILVERLIGHT
namespace Syncfusion.Windows.Controls.PivotGrid
#else
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    internal class PivotGridException : Exception
    {
        public PivotGridException()
            : base()
        {

        }

        public PivotGridException(string message)
            : base(message)
        {

        }

        public PivotGridException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
