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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization.DiagramEnums;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.DataVisualization.Models.Controls
{

    public partial class Node
    {
       
        private string _strFillColor = "white";
        private string _strBorderColor = "black";
        private int _iBorderWidth = 1;
        private string _strBorderDashArray = "";
        private int _iOpacity = 1;
        private LinearGradient _linearGradient;
        private Shapes _shape = new Rectangle();

        [JsonProperty("fillColor")]
        [DefaultValue("white")]
        public string FillColor
        {
            get { return this._strFillColor; }
            set
            {
                if (this._strFillColor != value)
                    this._strFillColor = value;
            }
        }

        [JsonProperty("borderColor")]
        [DefaultValue("black")]
        public string BorderColor
        {
            get { return this._strBorderColor; }
            set
            {
                if (this._strBorderColor != value)
                    this._strBorderColor = value;
            }
        }

        [JsonProperty("borderWidth")]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get { return this._iBorderWidth; }
            set
            {
                if (this._iBorderWidth != value)
                    this._iBorderWidth = value;
            }
        }

        [JsonProperty("borderDashArray")]
        [DefaultValue("")]
        public string BorderDashArray
        {
            get { return this._strBorderDashArray; }
            set
            {
                if (this._strBorderDashArray != value)
                    this._strBorderDashArray = value;
            }
        }

        [JsonProperty("opacity")]
        [DefaultValue(1)]
        public int Opacity
        {
            get { return this._iOpacity; }
            set
            {
                if (this._iOpacity != value)
                    this._iOpacity = value;
            }
        }

        [JsonProperty("gradient")]
        public LinearGradient LinearGradient
        {
            get { return this._linearGradient; }
            set
            {
                if (this._linearGradient != value)
                    this._linearGradient = value;
            }
        }

        [JsonProperty("shape")]   
        [DefaultValue(null)]
        public Shapes Shape
        {
            get { return this._shape; }
            set
            {
                if (this._shape != value)
                {
                    this._shape = value;
                }
            }
        }
    }

    public partial class Group
    {
        private bool _mCanUngroup = true;
        private Collection _mChildren;
        [JsonProperty("children")]
        public Collection Children
        {
            get { return this._mChildren; }
            set
            {
                if (this._mChildren != value)
                    this._mChildren = value;
            }
        }

        [JsonProperty("canUngroup")]
        [DefaultValue(true)]
        public bool CanUngroup
        {
            get { return this._mCanUngroup; }
            set
            {
                if (this._mCanUngroup != value)
                    this._mCanUngroup = value;
            }
        }
    }

    public partial class Port
    {
        #region Name

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

        #endregion

        #region Visibility

        private PortVisibility _mVisibility = PortVisibility.Hover;

        [JsonProperty("visibility")]
        [DefaultValue(PortVisibility.Hover)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PortVisibility Visibility
        {
            get { return this._mVisibility; }
            set
            {
                if (this._mVisibility != value)
                {
                    this._mVisibility = value;
                }
            }
        }

        #endregion

        #region AllowConnections

        private Boolean _mAllowConnections = true;

        [JsonProperty("allowConnections")]
        [DefaultValue(true)]
        public Boolean AllowConnections
        {
            get { return this._mAllowConnections; }
            set
            {
                if (this._mAllowConnections != value)
                {
                    this._mAllowConnections = value;
                }
            }
        }

        #endregion

        #region Size

        private int _mSize = 10;

        [JsonProperty("size")]
        [DefaultValue(10)]
        public int Size
        {
            get { return this._mSize; }
            set
            {
                if (this._mSize != value)
                {
                    this._mSize = value;
                }
            }
        }

        #endregion

        #region Offset

        private DiagramPoint _mOffset = new DiagramPoint();

        [JsonProperty("offset")]
        
        public DiagramPoint Offset
        {
            get { return this._mOffset; }
            set
            {
                if (this._mOffset != value)
                {
                    this._mOffset = value;
                }
            }
        }

        #endregion

        #region BorderColor

        private String _mBorderColor = "#007bc6";

        [JsonProperty("borderColor")]
        [DefaultValue("#007bc6")]
        public String BorderColor
        {
            get { return this._mBorderColor; }
            set
            {
                if (this._mBorderColor != value)
                {
                    this._mBorderColor = value;
                }
            }
        }

        #endregion

        #region BorderWidth

        private int _mBorderWidth = 1;

        [JsonProperty("borderWidth")]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get { return this._mBorderWidth; }
            set
            {
                if (this._mBorderWidth != value)
                {
                    this._mBorderWidth = value;
                }
            }
        }

        #endregion

        #region FillColor

        private string _mFillColor = "white";

        [JsonProperty("fillColor")]
        [DefaultValue("white")]
        public string FillColor
        {
            get { return this._mFillColor; }
            set
            {
                if (this._mFillColor != value)
                {
                    this._mFillColor = value;
                }
            }
        }

        #endregion

        #region Shape

        private PortShapes _mShape = PortShapes.X;

        [JsonProperty("shape")]
        [DefaultValue(PortShapes.X)]
        public PortShapes Shape
        {
            get { return this._mShape; }
            set
            {
                if (this._mShape != value)
                {
                    this._mShape = value;
                }
            }
        }

        #endregion

        #region PathData

        private String _mPathData = string.Empty;

        [JsonProperty("pathData")]
        [DefaultValue("")]
        public String PathData
        {
            get { return this._mPathData; }
            set
            {
                if (this._mPathData != value)
                {
                    this._mPathData = value;
                }
            }
        }

        #endregion

        #region Constraints

        private PortConstraints _mConstraints = PortConstraints.Connect;

        [JsonProperty("constraints")]
        [DefaultValue(PortConstraints.Connect)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PortConstraints Constraints
        {
            get { return this._mConstraints; }
            set
            {
                if (this._mConstraints != value)
                {
                    this._mConstraints = value;
                }
            }
        }

        #endregion

    }

    public partial class Connector
    {
        

        #region Visible

        private Boolean _mVisible = true;

        [JsonProperty("visible")]
        [DefaultValue(true)]
        public Boolean Visible
        {
            get { return this._mVisible; }
            set
            {
                if (this._mVisible != value)
                {
                    this._mVisible = value;
                }
            }
        }

        #endregion

        #region LineDashArray

        private String _mLineDashArray = null;

        [JsonProperty("lineDashArray")]
        [DefaultValue("")]
        public String LineDashArray
        {
            get { return this._mLineDashArray; }
            set
            {
                if (this._mLineDashArray != value)
                {
                    this._mLineDashArray = value;
                }
            }
        }

        #endregion

        #region TargetPort

        private Port _mTargetPort = null;
        [JsonIgnore()]
        public Port TargetPort
        {
            get { return this._mTargetPort; }
            set
            {
                if (this._mTargetPort != value)
                {
                    this._mTargetPort = value;
                }
            }
        }

        #endregion

        #region SourcePort

        private Port _mSourcePort = null;
        [JsonIgnore()]
        public Port SourcePort
        {
            get { return this._mSourcePort; }
            set
            {
                if (this._mSourcePort != value)
                {
                    this._mSourcePort = value;
                }
            }
        }

        #endregion 

        #region HeadDecorator

        private Decorator _mTargetDecorator = new Decorator();

        [JsonProperty("targetDecorator")]
        public Decorator TargetDecorator
        {
            get { return this._mTargetDecorator; }
            set
            {
                if (this._mTargetDecorator != value)
                {
                    this._mTargetDecorator = value;
                }
            }
        }

        #endregion

        #region TailDecorator

        private Decorator _mSourceDecorator = new Decorator();

        [JsonProperty("sourceDecorator")]
        [DefaultValue(null)]
        public Decorator SourceDecorator
        {
            get { return this._mSourceDecorator; }
            set
            {
                if (this._mSourceDecorator != value)
                {
                    this._mSourceDecorator = value;
                }
            }
        }

        #endregion

        #region Line

        private Lines _mLine = new Lines();
        [JsonProperty("line")]
        [DefaultValue(null)]
        public Lines Line
        {
            get { return this._mLine; }
            set
            {
                if (this._mLine != value)
                {
                    this._mLine = value;
                }
            }
        }        

        #endregion

        #region LineColor

        private String _mLineColor = "black";

        [JsonProperty("lineColor")]
        [DefaultValue("black")]
        public String LineColor
        {
            get { return this._mLineColor; }
            set
            {
                if (this._mLineColor != value)
                {
                    this._mLineColor = value;
                }
            }
        }

        #endregion

        #region LineWidth

        private int _mLineWidth = 1;

        [JsonProperty("lineWidth")]
        [DefaultValue(1)]
        public int LineWidth
        {
            get { return this._mLineWidth; }
            set
            {
                if (this._mLineWidth != value)
                {
                    this._mLineWidth = value;
                }
            }
        }

        #endregion

        #region Constraints

        private ConnectorConstraints _mConstraints = ConnectorConstraints.Default;

        [JsonProperty("constraints")]
        [DefaultValue(ConnectorConstraints.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ConnectorConstraints Constraints
        {
            get { return this._mConstraints; }
            set
            {
                if (this._mConstraints != value)
                {
                    this._mConstraints = value;
                }
            }
        }

        #endregion

        #region Opacity

        private int _mOpacity = 1;

        [JsonProperty("opacity")]
        [DefaultValue(1)]
        public int Opacity
        {
            get { return this._mOpacity; }
            set
            {
                if (this._mOpacity != value)
                {
                    this._mOpacity = value;
                }
            }
        }

        #endregion

        #region Labels

        private Collection _mLabels = new Collection();
        [JsonProperty("labels")]
        public Collection Labels
        {
            get { return this._mLabels; }
            set
            {
                if (this._mLabels != value)
                {
                    this._mLabels = value;
                }
            }
        }

        #endregion

        #region TargetNode

        private Node _mTargetNode = null;
        [JsonIgnore()]
        public Node TargetNode
        {
            get { return this._mTargetNode; }
            set
            {
                if (this._mTargetNode != value)
                {
                    this._mTargetNode = value;
                }
            }
        }

        #endregion

        #region SourceNode

        private Node _mSourceNode = null;
        [JsonIgnore()]
        public Node SourceNode
        {
            get { return this._mSourceNode; }
            set
            {
                if (this._mSourceNode != value)
                {
                    this._mSourceNode = value;
                }
            }
        }

        #endregion

        #region ReadOnly Properties

        [JsonProperty("targetPortName")]
        [DefaultValue("")]
        public string TargetPortName
        {
            get { return this._mTargetPort != null ? this._mTargetPort.Name : ""; }
        }

        [JsonProperty("sourcePortName")]
        [DefaultValue("")]
        public string SourcePortName
        {
            get { return this._mSourcePort != null ? this._mSourcePort.Name : ""; }
        }

        [JsonProperty("targetNodeName")]
        [DefaultValue("")]
        public string TargetNodeName
        {
            get { return this._mTargetNode != null ? this._mTargetNode.Name : ""; }
        }

        [JsonProperty("sourceNodeName")]
        [DefaultValue("")]
        public string SourceNodeName
        {
            get { return this._mSourceNode != null ? this._mSourceNode.Name : ""; }
        }

        #endregion
    }

    public partial class Decorator
    {
        #region Shape

        private DecoratorShapes _mShape = DecoratorShapes.None;

        [JsonProperty("shape")]
        [DefaultValue(DecoratorShapes.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public DecoratorShapes Shape
        {
            get { return this._mShape; }
            set
            {
                if (this._mShape != value)
                {
                    this._mShape = value;
                }
            }
        }

        #endregion

        #region Width

        private int _mWidth = 8;

        [JsonProperty("width")]
        [DefaultValue(8)]
        public int Width
        {
            get { return this._mWidth; }
            set
            {
                if (this._mWidth != value)
                {
                    this._mWidth = value;
                }
            }
        }

        #endregion

        #region Height

        private int _mHeight = 8;

        [JsonProperty("height")]
        [DefaultValue(8)]
        public int Height
        {
            get { return this._mHeight; }
            set
            {
                if (this._mHeight != value)
                {
                    this._mHeight = value;
                }
            }
        }

        #endregion

        #region BorderColor

        private String _mBorderColor = "black";

        [JsonProperty("borderColor")]
        [DefaultValue("black")]
        public String BorderColor
        {
            get { return this._mBorderColor; }
            set
            {
                if (this._mBorderColor != value)
                {
                    this._mBorderColor = value;
                }
            }
        }

        #endregion

        #region FillColor

        private String _mFillColor = "black";

        [JsonProperty("fillColor")]
        [DefaultValue("black")]
        public String FillColor
        {
            get { return this._mFillColor; }
            set
            {
                if (this._mFillColor != value)
                {
                    this._mFillColor = value;
                }
            }
        }

        #endregion

        #region PathData

        private String _mPathData = string.Empty;

        [JsonProperty("pathData")]
        [DefaultValue("")]
        public String PathData
        {
            get { return this._mPathData; }
            set
            {
                if (this._mPathData != value)
                {
                    this._mPathData = value;
                }
            }
        }

        #endregion

    }

    public partial class TextBlock
    {
        #region ReadOnly

        private Boolean _mReadOnly = false;

        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public Boolean ReadOnly
        {
            get { return this._mReadOnly; }
            set
            {
                if (this._mReadOnly != value)
                {
                    this._mReadOnly = value;
                }
            }
        }

        #endregion

        #region Bold

        private Boolean _mBold = false;

        [JsonProperty("bold")]
        [DefaultValue(false)]
        public Boolean Bold
        {
            get { return this._mBold; }
            set
            {
                if (this._mBold != value)
                {
                    this._mBold = value;
                }
            }
        }

        #endregion

        #region Italic

        private Boolean _mItalic = false;

        [JsonProperty("italic")]
        [DefaultValue(false)]
        public Boolean Italic
        {
            get { return this._mItalic; }
            set
            {
                if (this._mItalic != value)
                {
                    this._mItalic = value;
                }
            }
        }

        #endregion

        #region Text

        private String _mText = string.Empty;

        [JsonProperty("text")]
        [DefaultValue("")]
        public String Text
        {
            get { return this._mText; }
            set
            {
                if (this._mText != value)
                {
                    this._mText = value;
                }
            }
        }

        #endregion

        #region TextDecoration

        private TextDecorations _mTextDecoration = TextDecorations.None;

        [JsonProperty("textDecoration")]
        [DefaultValue(TextDecorations.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TextDecorations TextDecoration
        {
            get { return this._mTextDecoration; }
            set
            {
                if (this._mTextDecoration != value)
                {
                    this._mTextDecoration = value;
                }
            }
        }

        #endregion

        #region FontSize

        private int _mFontSize = 12;

        [JsonProperty("fontSize")]
        [DefaultValue(12)]
        public int FontSize
        {
            get { return this._mFontSize; }
            set
            {
                if (this._mFontSize != value)
                {
                    this._mFontSize = value;
                }
            }
        }

        #endregion

        #region FontFamily

        private String _mFontFamily = "Arial";

        [JsonProperty("fontFamily")]
        [DefaultValue("Arial")]
        public String FontFamily
        {
            get { return this._mFontFamily; }
            set
            {
                if (this._mFontFamily != value)
                {
                    this._mFontFamily = value;
                }
            }
        }

        #endregion

        #region FontColor

        private String _mFontColor = "black";

        [JsonProperty("fontColor")]
        [DefaultValue("black")]
        public String FontColor
        {
            get { return this._mFontColor; }
            set
            {
                if (this._mFontColor != value)
                {
                    this._mFontColor = value;
                }
            }
        }

        #endregion

        #region Offset

        private DiagramPoint _mOffset = new DiagramPoint(0.5f, 0.5f);

        [JsonProperty("offset")]
        public DiagramPoint Offset
        {
            get { return this._mOffset; }
            set
            {
                if (this._mOffset != value)
                {
                    this._mOffset = value;
                }
            }
        }

        #endregion

        #region Align

        private TextAlign _mAlign = TextAlign.Center;

        [JsonProperty("align")]
        [DefaultValue(TextAlign.Center)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TextAlign Align
        {
            get { return this._mAlign; }
            set
            {
                if (this._mAlign != value)
                {
                    this._mAlign = value;
                }
            }
        }

        #endregion

        #region WrapText

        private bool _mWrapText = true;

        [JsonProperty("wrapText")]
        [DefaultValue(true)]
        public bool WrapText
        {
            get { return this._mWrapText; }
            set
            {
                if (this._mWrapText != value)
                {
                    this._mWrapText = value;
                }
            }
        }

        #endregion

        #region Margin

        private LabelMargin _mMargin = new LabelMargin();

        [JsonProperty("margin")]
        public LabelMargin Margin
        {
            get { return this._mMargin; }
            set
            {
                if (this._mMargin != value)
                {
                    this._mMargin = value;
                }
            }
        }

        #endregion

        #region HorizontalAlignment

        private HorizontalAlignment _mHorizontalAlignment =  HorizontalAlignment.Center;

        [JsonProperty("horizontalAlignment")]
        [DefaultValue(HorizontalAlignment.Center)]
        [JsonConverter(typeof(StringEnumConverter))]
        public HorizontalAlignment HorizontalAlignment
        {
            get { return this._mHorizontalAlignment; }
            set
            {
                if (this._mHorizontalAlignment != value)
                {
                    this._mHorizontalAlignment = value;
                }
            }
        }

        #endregion

        #region VerticalAlignment

        private VerticalAlignment _mVerticalAlignment = VerticalAlignment.Center;

        [JsonProperty("verticalAlignment")]
        [DefaultValue(VerticalAlignment.Center)]
        [JsonConverter(typeof(StringEnumConverter))]
        public VerticalAlignment VerticalAlignment
        {
            get { return this._mVerticalAlignment; }
            set
            {
                if (this._mVerticalAlignment != value)
                {
                    this._mVerticalAlignment = value;
                }
            }
        }

        #endregion

        #region LabelEditMode

        private LabelEditMode _mLabelEditMode = LabelEditMode.Edit;

        [JsonProperty("mode")]
        [DefaultValue(LabelEditMode.Edit)]
        [JsonConverter(typeof(StringEnumConverter))]
        public LabelEditMode Mode
        {
            get { return this._mLabelEditMode; }
            set
            {
                if (this._mLabelEditMode != value)
                {
                    this._mLabelEditMode = value;
                }
            }
        }

        #endregion
    }

    public partial class LabelMargin
    {
        public LabelMargin()
        {
        }

        public LabelMargin(int top, int left, int right, int bottom)
        {
            this._mBottom = bottom;
            this._mLeft = left;
            this._mRight = right;
            this._mTop = top;
        }
        #region Top

        private int _mTop = 5;

        [JsonProperty("top")]
        [DefaultValue(5)]
        public int Top
        {
            get { return this._mTop; }
            set
            {
                if (this._mTop != value)
                {
                    this._mTop = value;
                }
            }
        }

        #endregion

        #region Left

        private int _mLeft = 5;

        [JsonProperty("left")]
        [DefaultValue(5)]
        public int Left
        {
            get { return this._mLeft; }
            set
            {
                if (this._mLeft != value)
                {
                    this._mLeft = value;
                }
            }
        }

        #endregion

        #region Top

        private int _mRight = 5;

        [JsonProperty("right")]
        [DefaultValue(5)]
        public int Right
        {
            get { return this._mRight; }
            set
            {
                if (this._mRight != value)
                {
                    this._mRight = value;
                }
            }
        }

        #endregion

        #region Bottom

        private int _mBottom = 5;

        [JsonProperty("bottom")]
        [DefaultValue(5)]
        public int Bottom
        {
            get { return this._mBottom; }
            set
            {
                if (this._mBottom != value)
                {
                    this._mBottom = value;
                }
            }
        }

        #endregion
    }

    public partial class Label : TextBlock
    {
        #region Name

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

        #endregion

        #region Visible

        private Boolean _mVisible = true;

        [JsonProperty("visible")]
        [DefaultValue(true)]
        public Boolean Visible
        {
            get { return this._mVisible; }
            set
            {
                if (this._mVisible != value)
                {
                    this._mVisible = value;
                }
            }
        }

        #endregion

        #region BorderColor

        private String _mBorderColor = "transparent";

        [JsonProperty("borderColor")]
        [DefaultValue("transparent")]
        public String BorderColor
        {
            get { return this._mBorderColor; }
            set
            {
                if (this._mBorderColor != value)
                {
                    this._mBorderColor = value;
                }
            }
        }

        #endregion

        #region BorderWidth

        private int _mBorderWidth = 0;

        [JsonProperty("borderWidth")]
        [DefaultValue(0)]
        public int BorderWidth
        {
            get { return this._mBorderWidth; }
            set
            {
                if (this._mBorderWidth != value)
                {
                    this._mBorderWidth = value;
                }
            }
        }

        #endregion

        #region FillColor

        private String _mFillColor = "transparent";

        [JsonProperty("fillColor")]
        [DefaultValue("transparent")]
        public String FillColor
        {
            get { return this._mFillColor; }
            set
            {
                if (this._mFillColor != value)
                {
                    this._mFillColor = value;
                }
            }
        }

        #endregion

    }

    public partial class PageSettings
    {

        #region PageWidth

        private int _mPageWidth = 0;

        [JsonProperty("pageWidth")]
        [DefaultValue(0)]
        public int PageWidth
        {
            get { return this._mPageWidth; }
            set
            {
                if (this._mPageWidth != value)
                {
                    this._mPageWidth = value;
                }
            }
        }

        #endregion

        #region PageHeight

        private int _mPageHeight = 0;

        [JsonProperty("pageHeight")]
        [DefaultValue(0)]
        public int PageHeight
        {
            get { return this._mPageHeight; }
            set
            {
                if (this._mPageHeight != value)
                {
                    this._mPageHeight = value;
                }
            }
        }

        #endregion

        #region MultiplePage

        private Boolean _mMultiplePage = true;

        [JsonProperty("multiplePage")]
        [DefaultValue(true)]
        public Boolean MultiplePage
        {
            get { return this._mMultiplePage; }
            set
            {
                if (this._mMultiplePage != value)
                {
                    this._mMultiplePage = value;
                }
            }
        }

        #endregion

        #region PageBorderWidth

        private int _mPageBorderWidth = 1;

        [JsonProperty("pageBorderWidth")]
        [DefaultValue(1)]
        public int PageBorderWidth
        {
            get { return this._mPageBorderWidth; }
            set
            {
                if (this._mPageBorderWidth != value)
                {
                    this._mPageBorderWidth = value;
                }
            }
        }

        #endregion

        #region Pagebackground

        private String _mPageBackgroundColor = "#ffffff";

        [JsonProperty("pageBackgroundColor")]
        [DefaultValue("#ffff")]
        public String PageBackgroundColor
        {
            get { return this._mPageBackgroundColor; }
            set
            {
                if (this._mPageBackgroundColor != value)
                {
                    this._mPageBackgroundColor = value;
                }
            }
        }

        #endregion

        #region Pageborder

        private String _mPageBorderColor = "#565656";

        [JsonProperty("pageBorderColor")]
        [DefaultValue("#565656")]
        public String PageBorderColor
        {
            get { return this._mPageBorderColor; }
            set
            {
                if (this._mPageBorderColor != value)
                {
                    this._mPageBorderColor = value;
                }
            }
        }

        #endregion

        #region PageMargin

        private int _mPageMargin = 24;

        [JsonProperty("pageMargin")]
        [DefaultValue(24)]
        public int PageMargin
        {
            get { return this._mPageMargin; }
            set
            {
                if (this._mPageMargin != value)
                {
                    this._mPageMargin = value;
                }
            }
        }

        #endregion

        #region ShowPageBreaks

        private Boolean _mShowPageBreaks = true;

        [JsonProperty("showPageBreaks")]
        [DefaultValue(true)]
        public Boolean ShowPageBreaks
        {
            get { return this._mShowPageBreaks; }
            set
            {
                if (this._mShowPageBreaks != value)
                {
                    this._mShowPageBreaks = value;
                }
            }
        }

        #endregion

        #region PageOrientation

        private Syncfusion.JavaScript.DataVisualization.DiagramEnums.Orientation _mPageOrientation = Syncfusion.JavaScript.DataVisualization.DiagramEnums.Orientation.Portrait;

        [JsonProperty("pageOrientation")]
        [DefaultValue(Syncfusion.JavaScript.DataVisualization.DiagramEnums.Orientation.Portrait)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Syncfusion.JavaScript.DataVisualization.DiagramEnums.Orientation PageOrientation
        {
            get { return this._mPageOrientation; }
            set
            {
                if (this._mPageOrientation != value)
                {
                    this._mPageOrientation = value;
                }
            }
        }

        #endregion

    }

    public partial class SnapSettings
    {
        #region HorizontalGridlines

        private GridLines _mHorizontalGridlines;

        [JsonProperty("horizontalGridlines")]
        [DefaultValue(null)]
        public GridLines HorizontalGridlines
        {
            get
            {
                return this._mHorizontalGridlines;
            }
            set
            {
                if (this._mHorizontalGridlines != value)
                {
                    this._mHorizontalGridlines = value;
                }
            }
        }

        #endregion

        #region VerticalGridlines

        private GridLines _mVerticalGridlines;

        [JsonProperty("verticalGridlines")]
        public GridLines VerticalGridlines
        {
            get { return this._mVerticalGridlines; }
            set
            {
                if (this._mVerticalGridlines != value)
                {
                    this._mVerticalGridlines = value;
                }
            }
        }

        #endregion

        #region SnapConstraints

        private SnapConstraints _mSnapConstraints = SnapConstraints.All;

        [JsonProperty("snapConstraints")]
        [DefaultValue(SnapConstraints.All)]
        [JsonConverter((typeof(StringEnumConverter)))]
        public SnapConstraints SnapConstraints
        {
            get { return this._mSnapConstraints; }
            set
            {
                if (this._mSnapConstraints != value)
                {
                    this._mSnapConstraints = value;
                }
            }
        }

        #endregion

        #region SnapToObject
        private bool _mSnapToObject = true;
        [JsonProperty("snapToObject")]
        [DefaultValue(true)]
        public bool SnapToObject
        {
            get { return _mSnapToObject; }
            set
            {
                if (this._mSnapToObject != value)
                    this._mSnapToObject = value;
            }
        }
        #endregion

        #region SnapAngle
        private int _mSnapAngle = 5;
        [JsonProperty("snapAngle")]
        [DefaultValue(5)]
        public int SnapAngle
        {
            get { return _mSnapAngle; }
            set
            {
                if (this._mSnapAngle != value)
                    this._mSnapAngle = value;
            }
        }
        #endregion
    }

    public partial class GridLines
    {
        #region linesInterval

        private List<Decimal> _mlinesInterval = new List<Decimal>(); 
        [JsonProperty("linesInterval")]
        public List<Decimal> LinesInterval
        {
            get { return this._mlinesInterval; }
            set
            {
                if (this._mlinesInterval != value)
                {
                    this._mlinesInterval = value;
                }
            }
        }

        #endregion

        #region snapInterval

        private List<Decimal> _msnapInterval = new List<Decimal>();

        [JsonProperty("snapInterval")]
        public List<Decimal> SnapInterval
        {
            get { return this._msnapInterval; }
            set
            {
                if (this._msnapInterval != value)
                {
                    this._msnapInterval = value;
                }
            }
        }

        #endregion

        #region strokes

        private Strokes _mstrokes = new Strokes();

        [JsonProperty("strokes")]
        public Strokes Strokes
        {
            get { return this._mstrokes; }
            set
            {
                if (this._mstrokes != value)
                {
                    this._mstrokes = value;
                }
            }
        }

        #endregion

    }

    public partial class Strokes
    {
        private string _mstrokeDashArray = "0";
        [JsonProperty("strokeDashArray")]
        [DefaultValue("0")]
        public string StrokeDashArray
        {
            get { return this._mstrokeDashArray; }
            set
            {
                if (this._mstrokeDashArray != value)
                {
                    this._mstrokeDashArray = value;
                }
            }
        }

        private string _mstroke = "lightgray";
        [JsonProperty("stroke")]
        [DefaultValue("lightgray")]
        public string Stroke
        {
            get { return this._mstroke; }
            set
            {
                if (this._mstroke != value)
                {
                    this._mstroke = value;
                }
            }
        }
    }

    public partial class DiagramPoint
    {
        public DiagramPoint()
        {
            this._mx = 0;
            this._my = 0;
        }
        public DiagramPoint(float x, float y)
        {
            this._mx = x;
            this._my = y;
        }

        private float _mx = 0;
        [JsonProperty("x")]
        [DefaultValue(0)]
        public float X
        {
            get { return this._mx; }
            set
            {
                if (this._mx != value)
                {
                    this._mx = value;
                }
            }
        }

        private float _my = 0;
        [JsonProperty("y")]
        [DefaultValue(0)]
        public float Y
        {
            get { return this._my; }
            set
            {
                if (this._my != value)
                {
                    this._my = value;
                }
            }
        }

    }

    public partial class Layout
    {
        private string _mOrientation = "topToBottom";
        private string _mType = "";
        private int _mHorizontalSpacing = 30;
        private int _mVerticalSpacing = 30;
        private int _mMarginX = 0;
        private int _mMarginY = 0;
        private NodeBase _mFixedNode = null;


        [JsonProperty("orientation")]
        [DefaultValue("topToBottom")]
        public string Orientation
        {
            get { return this._mOrientation; }
            set
            {
                if (this._mOrientation != value)
                    this._mOrientation = value;
            }
        }

        [JsonProperty("horizontalSpacing")]
        [DefaultValue(30)]
        public int HorizontalSpacing
        {
            get { return this._mHorizontalSpacing; }
            set
            {
                if (this._mHorizontalSpacing != value)
                    this._mHorizontalSpacing = value;
            }
        }

        [JsonProperty("verticalSpacing")]
        [DefaultValue(30)]
        public int VerticalSpacing
        {
            get { return this._mVerticalSpacing; }
            set
            {
                if (this._mVerticalSpacing != value)
                    this._mVerticalSpacing = value;
            }
        }

        [JsonProperty("marginX")]
        [DefaultValue(0)]
        public int MarginX
        {
            get { return this._mMarginX; }
            set
            {
                if (this._mMarginX != value)
                    this._mMarginX = value;
            }
        }


        [JsonProperty("marginY")]
        [DefaultValue(0)]
        public int MarginY
        {
            get { return this._mMarginY; }
            set
            {
                if (this._mMarginY != value)
                    this._mMarginY = value;
            }
        }

        [JsonProperty("fixedNode")]
        [DefaultValue(null)]
        public NodeBase FixedNode
        {
            get { return this._mFixedNode; }
            set
            {
                if (this._mFixedNode != value)
                    this._mFixedNode = value;
            }
        }

        [JsonProperty("type")]
        [DefaultValue("")]
        public string Type
        {
            get { return this._mType; }
            set
            {
                if (this._mType != value)
                    this._mType = value;
            }
        }
    }

}
