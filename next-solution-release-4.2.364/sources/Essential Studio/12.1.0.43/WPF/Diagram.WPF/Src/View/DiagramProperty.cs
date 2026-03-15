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

namespace Syncfusion.Windows.Diagram
{
    public class DiagramProperty : IDiagramProperty
    {
        public Type ObjectType
        {
            get;
            set;
        }

        public string PropertyName
        {
            get;
            set;
        }
    }
}
