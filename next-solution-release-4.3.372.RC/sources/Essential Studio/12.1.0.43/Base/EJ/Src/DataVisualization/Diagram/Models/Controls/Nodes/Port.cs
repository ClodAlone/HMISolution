#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.DataVisualization.Models.Controls
{
    public partial class Port :ICloneable
    {
        public Port()
        {
        }
        public Port(Port src)
        {
            this.AllowConnections = src.AllowConnections;
            this.BorderColor = src.BorderColor;
            this.BorderWidth = src.BorderWidth;
            this.FillColor = src.FillColor;
            this.Name = src.Name;
            this.Offset = src.Offset;
            this.PathData = src.PathData;
            this.Shape = src.Shape;
            this.Size = src.Size;
            this.Visibility = src.Visibility;
        }
        public object Clone()
        {
            return new Port(this);
        }
    }

    
}
