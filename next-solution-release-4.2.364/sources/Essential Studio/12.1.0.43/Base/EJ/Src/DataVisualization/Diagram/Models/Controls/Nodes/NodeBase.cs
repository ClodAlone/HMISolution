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
using Syncfusion.JavaScript.DataVisualization.DiagramEnums;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.DataVisualization.Models.Controls
{
    public abstract class NodeBase 
    {

        private string _strName = "";
        [JsonProperty("name")]
        [DefaultValue("")]
        public string Name
        {
            get { return this._strName; }
            set
            {
                if (this._strName != value)
                    this._strName = value;
            }
        }

        private double _dWidth = 50;
        [JsonProperty("width")]
        [DefaultValue("50")]
        public double Width
        {
            get { return this._dWidth; }
            set
            {
                if (this._dWidth != value)
                    this._dWidth = value;
            }
        }

        private double _dHeight = 50;
        [JsonProperty("height")]
        [DefaultValue("50")]
        public double Height
        {
            get { return this._dHeight; }
            set
            {
                if (this._dHeight != value)
                    this._dHeight = value;
            }
        }

        private double _dPinX = 0;
        [JsonProperty("offsetX")]
        [DefaultValue("0")]
        public double OffsetX
        {
            get { return this._dPinX; }
            set
            {
                if (this._dPinX != value)
                    this._dPinX = value;
            }
        }

        private double _dPinY = 0;
        [JsonProperty("offsetY")]
        [DefaultValue("0")]
        public double OffsetY
        {
            get { return this._dPinY; }
            set
            {
                if (this._dPinY != value)
                    this._dPinY = value;
            }
        }

        private bool _bVisible = true;
        [JsonProperty("visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get { return this._bVisible; }
            set
            {
                if (this._bVisible != value)
                    this._bVisible = value;
            }
        }

        private NodeConstraints _constraints = NodeConstraints.Default;
        [JsonProperty("constraints")]
        [DefaultValue(NodeConstraints.Default)]
        [JsonConverter((typeof(StringEnumConverter)))]
        public NodeConstraints Constraints
        {
            get { return this._constraints; }
            set
            {
                if (this._constraints != value)
                    this._constraints = value;
            }
        }

        private object _tag;
        [JsonProperty("tag")]
        public object Tag
        {
            get { return this._tag; }
            set
            {
                if (this._tag != value)
                    this._tag = value;
            }
        }

        private Collection _labels = new Collection();
        [JsonProperty("labels")]
        public Collection Labels
        {
            get { return this._labels; }
            set
            {
                if (this._labels != value)
                    this._labels = value;
            }
        }

        private Collection _ports = new Collection();
        [JsonProperty("ports")]
        public Collection Ports
        {
            get { return this._ports; }
            set
            {
                if (this._ports != value)
                    this._ports = value;
            }
        }

        private double _rotateAngle;
        [JsonProperty("rotateAngle")]
        [DefaultValue(0)]
        public double RotateAngle
        {
            get { return this._rotateAngle; }
            set
            {
                if (this._rotateAngle != value)
                    this._rotateAngle = value;
            }
        }

        private DiagramPoint _pivot = new DiagramPoint();
        [JsonProperty("pivot")]
        public DiagramPoint Pivot
        {
            get { return this._pivot; }
            set
            {
                if (this._pivot != value)
                    this._pivot = value;
            }
        }
        private Dictionary<string, object> _addInfo = new Dictionary<string, object>();
        [JsonProperty("addInfo")]
        public Dictionary<string, object> AddInfo
        {
            get { return _addInfo; }
            set { _addInfo = value; }
        }
    }
}
