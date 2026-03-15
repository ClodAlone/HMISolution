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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.DataVisualization.Models.Controls
{
    public partial class Connector : ICloneable
    {
        private String _mName = string.Empty;
        [JsonProperty("name")]
        [DefaultValue("")]
        public String Name
        {
            get { return this._mName; }
            set
            {
                if (this._mName != value)
                {
                    this._mName = value;
                }
            }
        }

        public Connector()
        {
        }

        public Connector(Connector src)
        {
            this.Constraints = src.Constraints;
            this.TargetDecorator = src.TargetDecorator;
            this.TargetNode = src.TargetNode;
            this.TargetPort = src.TargetPort;
            this.Labels = src.Labels;
            this.Line = src.Line;
            this.LineColor = src.LineColor;
            this.LineDashArray = src.LineDashArray;
            this.LineWidth = src.LineWidth;
            this.Name = src.Name;
            this.Opacity = src.Opacity;
            this.SourceDecorator = src.SourceDecorator;
            this.SourceNode = src.SourceNode;
            this.SourcePort = src.SourcePort;
        }

        public object Clone()
        {
            return new Connector(this);
        }
    }
}
