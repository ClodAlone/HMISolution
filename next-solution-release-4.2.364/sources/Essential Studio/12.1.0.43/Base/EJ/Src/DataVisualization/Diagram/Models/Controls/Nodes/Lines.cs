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
    public class Lines
    {
        public virtual string Type
        {
            get
            {
                return "";
            }
        }
    }

    public class Straight : Lines, ICloneable
    {

        private DiagramPoint _mStartPoint;
        private DiagramPoint _mEndPoint;

        public Straight()
        {
            this._mStartPoint = new DiagramPoint();
            this._mEndPoint = new DiagramPoint();
        }
        public Straight(DiagramPoint startPoint, DiagramPoint endPoint)
        {
            this._mStartPoint = startPoint;
            this._mEndPoint = endPoint;
        }
        public Straight(Straight src)
        {
            this._mStartPoint = src._mStartPoint;
            this._mEndPoint = src._mEndPoint;
        }
        [JsonProperty("startPoint")]
        public DiagramPoint StartPoint
        {
            get { return this._mStartPoint; }
            set { this._mStartPoint = value; }
        }
        [JsonProperty("endPoint")]
        public DiagramPoint EndPoint
        {
            get { return this._mEndPoint; }
            set { this._mEndPoint = value; }
        }
        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "straight";
            }
        }

        public object Clone()
        {
            return new Straight(this);
        }
    }

    public class Orthogonal : Lines, ICloneable
    {

        private DiagramPoint _mStartPoint;
        private DiagramPoint _mEndPoint;

        public Orthogonal()
        {
            this._mStartPoint = new DiagramPoint();
            this._mEndPoint = new DiagramPoint();
        }
        public Orthogonal(DiagramPoint startPoint, DiagramPoint endPoint)
        {
            this._mStartPoint = startPoint;
            this._mEndPoint = endPoint;
        }
        public Orthogonal(Orthogonal src)
        {
            this._mStartPoint = src._mStartPoint;
            this._mEndPoint = src._mEndPoint;
        }
        [JsonProperty("startPoint")]
        public DiagramPoint StartPoint
        {
            get { return this._mStartPoint; }
            set { this._mStartPoint = value; }
        }
        [JsonProperty("endPoint")]
        public DiagramPoint EndPoint
        {
            get { return this._mEndPoint; }
            set { this._mEndPoint = value; }
        }
        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "orthogonal";
            }
        }

        public object Clone()
        {
            return new Orthogonal(this);
        }
    }

    public class Bezier : Lines, ICloneable
    {

        private DiagramPoint _mStartPoint;
        private DiagramPoint _mEndPoint;
        private DiagramPoint _point1;
        private DiagramPoint _point2;

        public Bezier()
        {
            this._mStartPoint = new DiagramPoint();
            this._mEndPoint = new DiagramPoint();
            this._point1 = new DiagramPoint();
            this._point2 = new DiagramPoint();
        }
        public Bezier(DiagramPoint startPoint, DiagramPoint endPoint)
        {
            this._mStartPoint = startPoint;
            this._mEndPoint = endPoint;
            this._point1 = new DiagramPoint();
            this._point2 = new DiagramPoint();
        }
        public Bezier(DiagramPoint startPoint, DiagramPoint endPoint, DiagramPoint point1, DiagramPoint point2)
        {
            this._mStartPoint = startPoint;
            this._mEndPoint = endPoint;
            this._point1 = point1;
            this._point2 = point2;
        }

        public Bezier(Bezier src)
        {
            this._mStartPoint = src._mStartPoint;
            this._mEndPoint = src._mEndPoint;
            this._point1 = src._point1;
            this._point2 = src._point2;
        }
        [JsonProperty("startPoint")]
        public DiagramPoint StartPoint
        {
            get { return this._mStartPoint; }
            set { this._mStartPoint = value; }
        }
        [JsonProperty("endPoint")]
        public DiagramPoint EndPoint
        {
            get { return this._mEndPoint; }
            set { this._mEndPoint = value; }
        }
        [JsonProperty("point1")]
        public DiagramPoint Point1
        {
            get { return this._point1; }
            set { this._point1 = value; }
        }
        [JsonProperty("point2")]
        public DiagramPoint Point2
        {
            get { return this._point2; }
            set { this._point2 = value; }
        }
        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "bezier";
            }
        }

        public object Clone()
        {
            return new Bezier(this);
        }
    }
}
