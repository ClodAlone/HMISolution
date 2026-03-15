#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.DataVisualization.Models.Controls
{
    public partial class Group : NodeBase ,ICloneable
    {
        public Group()
        {
            this._mChildren = new Collection();
        }

        public Group(Group src)
        {
            this.CanUngroup = src.CanUngroup;
            this.Children = src.Children;
            this.Constraints = src.Constraints; 
            this.Height = src.Height;
            this.Labels = src.Labels; 
            this.Name = src.Name; 
            this.OffsetX = src.OffsetX;
            this.OffsetY = src.OffsetY;
            this.Pivot = src.Pivot;
            this.Ports = src.Ports;
            this.RotateAngle = src.RotateAngle; 
            this.Tag = src.Tag;
            this.Visible = src.Visible;
            this.Width = src.Width;
        }

        public void AppendChild(object node)
        {
            this._mChildren.Add(node);
        }

        public object Clone()
        {
            return new Group(this);
        }
    }
}
