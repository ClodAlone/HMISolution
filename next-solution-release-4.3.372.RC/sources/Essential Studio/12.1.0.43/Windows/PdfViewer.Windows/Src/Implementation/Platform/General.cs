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

namespace Syncfusion.PdfViewer.Base
{
    class PIPath
    {
        public PIPath()
        {
        }
    }

    class PIPen
    {
        public PIPen()
        {
        }
    }

    class PIBrush
    {
        public PIBrush()
        {
        }
    }

    class PIGraphicsState
    {
        object _state;

        public PIGraphicsState(object state)
        {
            _state = state;
        }

        public object State
        {
            get
            {
                return _state;
            }
        }
    }
}
