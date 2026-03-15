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
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization.DiagramEnums;
using Syncfusion.JavaScript.DataVisualization.Models;
using Syncfusion.JavaScript.DataVisualization.Models.Controls;

namespace Syncfusion.JavaScript.DataVisualization.Builders
{
    public class LayoutBuilder
    {
        public DiagramProperties diagramModel;
        public LayoutBuilder(DiagramProperties model)
        {
            this.diagramModel = model;
        }

        public LayoutBuilder HorizontalSpacing(int horizontalSpacing)
        {
            this.diagramModel.Layout.HorizontalSpacing = horizontalSpacing;
            return this;
        }

        public LayoutBuilder VerticalSpacing(int verticalSpacing)
        {
            this.diagramModel.Layout.VerticalSpacing = verticalSpacing;
            return this;
        }

        public LayoutBuilder MarginX(int marginX)
        {
            this.diagramModel.Layout.MarginX = marginX;
            return this;
        }

        public LayoutBuilder MarginY(int marginY)
        {
            this.diagramModel.Layout.MarginY = marginY;
            return this;
        }

        public LayoutBuilder Orientation(string orientation)
        {
            this.diagramModel.Layout.Orientation = orientation;
            return this;
        }

        public LayoutBuilder FixedNode(NodeBase fixedNode)
        {
            this.diagramModel.Layout.FixedNode = fixedNode;
            return this;
        } 
        
    }
}
