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

namespace Syncfusion.XlsIO.Implementation.SizeProperties
{
    class Size
    {
        private SizeProperties SizeProperties;
        private ClientAnchor shape;

        public Size(ClientAnchor shape)
        {
            // TODO: Complete member initialization
            this.shape = shape;
        }
        internal SizeProperties method_7()
        {
            if (this.SizeProperties == null)
            {
                this.SizeProperties = new SizeProperties(this);
            }
            return this.SizeProperties;
        }
    }
}
