#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Syncfusion.JavaScript.DataVisualization.Models;
using Syncfusion.JavaScript.DataVisualization.Models.Controls;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;

namespace Syncfusion.JavaScript.DataVisualization.Builders
{

    public interface IDiagramNodesAdder
    {
        IDiagramNodesAdder Add(Collection nodes);
        IDiagramNodesAdder Add(Node node);
    }

    /// <summary>
    /// Implementation of DiagramModel properties using view formatting. 
    /// </summary>
    /// <remarks>Get properties from viewpage and assign these properties to DiagramModel model</remarks>
    public class DiagramNodesAdder : IDiagramNodesAdder
    {
        private DiagramProperties model;

        public DiagramNodesAdder(DiagramProperties diagramModel)
        {
            this.model = diagramModel;
        }

        public IDiagramNodesAdder Add(Collection nodes)
        {
            foreach (NodeBase node in nodes)
                this.model.Nodes.Add(node);
            return this;
        }

        public IDiagramNodesAdder Add(Node node)
        {
            this.model.Nodes.Add(node);
            return this;
        }
    }
}
