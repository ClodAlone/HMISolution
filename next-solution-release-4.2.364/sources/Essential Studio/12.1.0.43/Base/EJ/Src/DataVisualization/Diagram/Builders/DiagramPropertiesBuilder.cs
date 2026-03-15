#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;
using Syncfusion.JavaScript.DataVisualization.Models.Controls;
using Syncfusion.JavaScript.DataVisualization.Builders;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class DiagramPropertiesBuilder
    {
        public Diagram diagram;

        public DiagramPropertiesBuilder(Diagram diagram)
        { 
            this.diagram = new Diagram(diagram.ID, diagram.DiagramModel); 
        }

        public DiagramPropertiesBuilder()
        {

        }

        public DiagramPropertiesBuilder Height(String height)
        {
            diagram.DiagramModel.Height = height;
            return this;
        }
        public DiagramPropertiesBuilder Width(String width)
        {
            diagram.DiagramModel.Width = width;
            return this;
        }

        public DiagramPropertiesBuilder BorderColor(String borderColor)
        {
            diagram.DiagramModel.BorderColor = borderColor;
            return this;
        }

        public DiagramPropertiesBuilder BorderWidth(int borderWidth)
        {
            diagram.DiagramModel.BorderWidth = borderWidth;
            return this;
        }

        public DiagramPropertiesBuilder EnableContextMenu(bool enableContextMenu)
        {
            diagram.DiagramModel.EnableContextMenu = enableContextMenu;
            return this;
        }

        public DiagramPropertiesBuilder AutoScroll(bool autoScroll)
        {
            diagram.DiagramModel.AutoScroll = autoScroll;
            return this;
        }

        public DiagramPropertiesBuilder AutoSize(bool autoSize)
        {
            diagram.DiagramModel.AutoSize = autoSize;
            return this;
        }

        public DiagramPropertiesBuilder EnableVisualGuide(bool enableVisualGuide)
        {
            diagram.DiagramModel.EnableVisualGuide = enableVisualGuide;
            return this;
        }

        public virtual DiagramPropertiesBuilder Nodes(Action<IDiagramNodesAdder> nodes)
        {
            diagram.DiagramModel.Nodes = new Collection();
            DiagramNodesAdder addNodes = new DiagramNodesAdder(diagram.DiagramModel);
            nodes.Invoke(addNodes); 
            return this;
        }

        public virtual DiagramPropertiesBuilder Connectors(Action<IDiagramConnectorsAdder> connectors)
        {
            diagram.DiagramModel.Connectors = new Collection();
            DiagramConnectorsAdder addConnectors = new DiagramConnectorsAdder(diagram.DiagramModel);
            connectors.Invoke(addConnectors);
            return this;
        }

        public DiagramPropertiesBuilder PageSettings(Action<PageSettingsBuilder> pageSettings)
        {
            var builder = new PageSettingsBuilder(diagram.DiagramModel);
            if (pageSettings != null)
                pageSettings.Invoke(builder);
            return this;
        }

        public DiagramPropertiesBuilder SnapSettings(Action<SnapSettingsBuilder> snapSettings)
        {
            var builder = new SnapSettingsBuilder(diagram.DiagramModel);
            if (snapSettings != null)
                snapSettings.Invoke(builder);
            return this;
        }

        public DiagramPropertiesBuilder Layout(Action<LayoutBuilder> layout)
        {
            var builder = new LayoutBuilder(diagram.DiagramModel);
            if (layout != null)
                layout.Invoke(builder);
            return this;
        }

        //Render
        public HtmlString Render()
        {
            return new HtmlString(diagram.Render().ToString());
        }
        public override String ToString()
        {
            return Render().ToString();
        }

         
    }
}
