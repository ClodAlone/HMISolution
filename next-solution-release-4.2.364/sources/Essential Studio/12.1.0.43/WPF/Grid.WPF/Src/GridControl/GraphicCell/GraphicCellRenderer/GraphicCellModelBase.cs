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
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicCellModelBase : Disposable
    {
        GraphicModel graphicModel;

        public GraphicCellModelBase()
        {

        }

        public GraphicModel GraphicModel
        {
            get { return graphicModel; }
            set { graphicModel = value; }
        }

        public virtual IGraphicCellRenderer CreateRenderer()
        {
            throw new NotImplementedException("GraphicCellRenderer is not implemented in derived class");
            //return null;
        }
    }
}
