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

    public interface IDiagramConnectorsAdder
    {
        IDiagramConnectorsAdder Add(Collection connectors);
        IDiagramConnectorsAdder Add(Connector connector);
    }

    /// <summary>
    /// Implementation of DiagramModel properties using view formatting. 
    /// </summary>
    /// <remarks>Get properties from viewpage and assign these properties to DiagramModel model</remarks>
    public class DiagramConnectorsAdder : IDiagramConnectorsAdder
    {
        private DiagramProperties model;

        public DiagramConnectorsAdder(DiagramProperties diagramModel)
        {
            this.model = diagramModel;
        }

        public IDiagramConnectorsAdder Add(Collection connectors)
        {
            foreach (Connector connector in connectors)
                this.model.Connectors.Add(connector);
            return this;
        }

        public IDiagramConnectorsAdder Add(Connector connector)
        {
            this.model.Connectors.Add(connector);
            return this;
        }
    }
}
