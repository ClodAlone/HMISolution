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
using Syncfusion.JavaScript.DataVisualization.Models.Collections;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.DataVisualization.Models.Controls
{
    public class Shapes
    {
        public virtual string Type
        {
            get
            {
                return "";
            }
        }
    }

    public class Rectangle : Shapes, ICloneable
    {
        private int _cornerRadius;
        public Rectangle()
        {

        }

        public Rectangle(int cornerRadius)
        {
            this._cornerRadius = cornerRadius;
        }

        public Rectangle(Rectangle src)
        {
            this._cornerRadius = src._cornerRadius;
        }


        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "rectangle";
            }
        }

        [JsonProperty("cornerRadius")]
        [DefaultValue(0)]
        public int cornerRadius
        {
            get
            {
                return _cornerRadius;
            }
            set
            {
                if (_cornerRadius != value)
                    _cornerRadius = value;
            }
        }

        public object Clone()
        {
            return new Rectangle(this);
        }
    }

    public class Ellipse : Shapes, ICloneable
    {

        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "ellipse";
            }
        }

        public object Clone()
        {
            return new Ellipse();
        }
    }

    public class Image : Shapes, ICloneable
    {
        private string _mSrc = string.Empty;
        public Image(string src)
        {
            this._mSrc = src;
        }

        public Image(Image src)
        {
            this._mSrc = src._mSrc;
        }

        [JsonProperty("src")]
        [DefaultValue("")]
        public string Src 
        {
            get { return this._mSrc; }
            set { this._mSrc = value; } 
        }

        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "image";
            }
        }

        public object Clone()
        {
            return new Image(this);
        }
    }

    public class Path : Shapes, ICloneable
    {
        private string _mPathData = string.Empty;
        public Path(string src)
        {
            this._mPathData = src;
        }

        public Path(Path src)
        {
            this._mPathData = src._mPathData;
        }

        [JsonProperty("pathData")]
        [DefaultValue("")]
        public string PathData
        {
            get { return this._mPathData; }
            set { this._mPathData = value; }
        }

        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "path";
            }
        }

        public object Clone()
        {
            return new Path(this);
        }
    }

    public class Text : Shapes, ICloneable
    {
        public TextBlock _mText = new TextBlock();
        public Text(TextBlock text)
        {
            this._mText = text;
        }

        public Text(Text src)
        {
            this.TextBlock = src.TextBlock;
        }

        [JsonProperty("textBlock")]
        [DefaultValue("")]
        public TextBlock TextBlock
        {
            get { return this._mText; }
            set { _mText = value; }
        }

        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "text";
            }
        }
        public object Clone()
        {
            return new Text(this);
        }
    }

    public class Polygon : Shapes, ICloneable
    {
        private Collection _mPoints;
        public Polygon()
        {
            this._mPoints = new Collection();
        }

        public Polygon(Collection points)
        {
            this._mPoints = points;
        }

        public Polygon(Polygon src)
        {
            this.Points = src.Points;
        }

        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "polygon";
            }
        }

        [JsonProperty("points")]
        public Collection Points
        {
            get { return _mPoints; }
            set { _mPoints = value; }
        }

        public object Clone()
        {
            return new Polygon(this);
        }
    }

    public class Html : Shapes, ICloneable
    {
        private string _mHtmlData = string.Empty;
        public Html(string htmlData)
        {
            this._mHtmlData = htmlData;
        }
        public Html(Html src)
        {
            this._mHtmlData = src._mHtmlData;
        }

        [JsonProperty("html")]
        [DefaultValue("")]
        public string HtmlData
        {
            get { return this._mHtmlData; }
            set { this._mHtmlData = value; }
        }
        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "html";
            }
        }
        public object Clone()
        {
            return new Html(this);
        }
    }

    public class NativeNode : Shapes, ICloneable
    {
        private string _contentId = string.Empty;
        public NativeNode(string contentId)
        {
            this._contentId = contentId;
        }
        public NativeNode(NativeNode src)
        {
            this._contentId = src._contentId;
        }

        [JsonProperty("content")]
        [DefaultValue("")]
        public string ContentId
        {
            get { return this._contentId; }
            set { this._contentId = value; }
        }
        [JsonProperty("type")]
        [DefaultValue("")]
        public override string Type
        {
            get
            {
                return "native";
            }
        }
        public object Clone()
        {
            return new NativeNode(this);
        }
    }
}
