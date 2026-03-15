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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Syncfusion.Windows.PropertyGrid;
using System.Reflection;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Reports.Designer.Editors
{
    internal interface IDesignerProperties : INotifyPropertyChanged, INotifyPropertyChanging
    {
        string Name { get; set; }

        bool IsInternalPropertyChange { get; set; }
    }

    internal interface IReportItemProperties : IDesignerProperties
    {
        bool IsTablixCell { get; set; }
    }

    #region NestedProperties

    internal class Padding
    {
        string left = null;
        string right = null;
        string top = null;
        string bottom = null;

        [CategoryAttribute("Padding")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the padding between the left edge of the report item and its content.")]
        public string PaddingLeft
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("PaddingLeft");
                    this.left = value;
                    OnPropertyChanged("PaddingLeft");
                }
            }
        }

        [CategoryAttribute("Padding")]
        [DisplayNameAttribute("Right")]
        [DescriptionAttribute("Specifies the padding between the right edge of the report item and its content.")]
        public string PaddingRight
        {
            get
            {
                return this.right;
            }
            set
            {
                if (this.right != value)
                {
                    OnPropertyChanging("PaddingRight");
                    this.right = value;
                    OnPropertyChanged("PaddingRight");
                }
            }
        }

        [CategoryAttribute("Padding")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the padding between the top edge of the report item and its content.")]
        public string PaddingTop
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("PaddingTop");
                    this.top = value;
                    OnPropertyChanged("PaddingTop");
                }
            }
        }

        [CategoryAttribute("Padding")]
        [DisplayNameAttribute("Bottom")]
        [DescriptionAttribute("Specifies the padding between the bottom edge of the report item and its content.")]
        public string PaddingBottom
        {
            get
            {
                return this.bottom;
            }
            set
            {
                if (this.bottom != value)
                {
                    OnPropertyChanging("PaddingBottom");
                    this.bottom = value;
                    OnPropertyChanged("PaddingBottom");
                }
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #region  Members

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    internal class Margins
    {
        string left = null;
        string right = null;
        string top = null;
        string bottom = null;

        [CategoryAttribute("Margins")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Left.")]
        public string LeftMargin
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("LeftMargin");
                    this.left = value;
                    OnPropertyChanged("LeftMargin");
                }
            }
        }

        [CategoryAttribute("Margins")]
        [DisplayNameAttribute("Right")]
        [DescriptionAttribute("Right.")]
        public string RightMargin
        {
            get
            {
                return this.right;
            }
            set
            {
                if (this.right != value)
                {
                    OnPropertyChanging("RightMargin");
                    this.right = value;
                    OnPropertyChanged("RightMargin");
                }
            }
        }

        [CategoryAttribute("Margins")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Top.")]
        public string TopMargin
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("TopMargin");
                    this.top = value;
                    OnPropertyChanged("TopMargin");
                }
            }
        }

        [CategoryAttribute("Margins")]
        [DisplayNameAttribute("Bottom")]
        [DescriptionAttribute("Bottom.")]
        public string BottomMargin
        {
            get
            {
                return this.bottom;
            }
            set
            {
                if (this.bottom != value)
                {
                    OnPropertyChanging("BottomMargin");
                    this.bottom = value;
                    OnPropertyChanged("BottomMargin");
                }
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        public void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    internal class BorderWidths
    {
        string _default=null;
        string left = null;
        string right = null;
        string top = null;
        string bottom = null;

        [CategoryAttribute("BorderWidth")]
        [DisplayNameAttribute("Default")]
        [DescriptionAttribute("Specifies the type of the image source.For example,the image source is external to the report.")]
        public string DefaultBorderWidth
        {
            get
            {
                return this._default;
            }
            set
            {
                if (this._default != value)
                {
                    OnPropertyChanging("DefaultBorderWidth");
                    this._default = value;
                    OnPropertyChanged("DefaultBorderWidth");
                }
            }
        }

        [CategoryAttribute("BorderWidth")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the type of the image source.For example,the image source is external to the report.")]
        public string LeftBorderWidth
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("LeftBorderWidth");
                    this.left = value;
                    OnPropertyChanged("LeftBorderWidth");
                }
            }
        }

        [CategoryAttribute("BorderWidth")]
        [DisplayNameAttribute("Right")]
        [DescriptionAttribute("Specifies the type of the image source.For example,the image source is external to the report.")]
        public string RightBorderWidth
        {
            get
            {
                return this.right;
            }
            set
            {
                if (this.right != value)
                {
                    OnPropertyChanging("RightBorderWidth");
                    this.right = value;
                    OnPropertyChanged("RightBorderWidth");
                }
            }
        }

        [CategoryAttribute("BorderWidth")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the type of the image source.For example,the image source is external to the report.")]
        public string TopBorderWidth
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("TopBorderWidth");
                    this.top = value;
                    OnPropertyChanged("TopBorderWidth");
                }
            }
        }

        [CategoryAttribute("BorderWidth")]
        [DisplayNameAttribute("Bottom")]
        [DescriptionAttribute("Specifies the type of the image source.For example,the image source is external to the report.")]
        public string BottomBorderWidth
        {
            get
            {
                return this.bottom;
            }
            set
            {
                if (this.bottom != value)
                {
                    OnPropertyChanging("BottomBorderWidth");
                    this.bottom = value;
                    OnPropertyChanged("BottomBorderWidth");
                }
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        public void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    internal class BorderColors
    {
        string _default = null;
        string left = null;
        string right = null;
        string top = null;
        string bottom = null;

        [CategoryAttribute("BorderColor")]
        [DisplayNameAttribute("Default")]
        [DescriptionAttribute("Specifies the default color of the border.")]
        public string DefaultBorderColor
        {
            get
            {
                return this._default;
            }
            set
            {
                if (this._default != value && value != "#00FFFFFF")
                {
                    OnPropertyChanging("DefaultBorderColor");
                    this._default = value;
                    OnPropertyChanged("DefaultBorderColor");
                }
            }
        }

        [CategoryAttribute("BorderColor")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the color of left border.")]
        public string LeftBorderColor
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value && value != "#00FFFFFF")
                {
                    OnPropertyChanging("LeftBorderColor");
                    this.left = value;
                    OnPropertyChanged("LeftBorderColor");
                }
            }
        }

        [CategoryAttribute("BorderColor")]
        [DisplayNameAttribute("Right")]
        [DescriptionAttribute("Specifies the color of right border.")]
        public string RightBorderColor
        {
            get
            {
                return this.right;
            }
            set
            {
                if (this.right != value && value != "#00FFFFFF")
                {
                    OnPropertyChanging("RightBorderColor");
                    this.right = value;
                    OnPropertyChanged("RightBorderColor");
                }
            }
        }

        [CategoryAttribute("BorderColor")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the color of top border.")]
        public string TopBorderColor
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value && value != "#00FFFFFF")
                {
                    OnPropertyChanging("TopBorderColor");
                    this.top = value;
                    OnPropertyChanged("TopBorderColor");
                }
            }
        }

        [CategoryAttribute("BorderColor")]
        [DisplayNameAttribute("Bottom")]
        [DescriptionAttribute("Specifies the color of bottom border.")]
        public string BottomBorderColor
        {
            get
            {
                return this.bottom;
            }
            set
            {
                if (this.bottom != value && value != "#00FFFFFF")
                {
                    OnPropertyChanging("BottomBorderColor");
                    this.bottom = value;
                    OnPropertyChanged("BottomBorderColor");
                }
            }
        }

        public override string ToString()
        {
            return (string.Empty);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        public void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    internal class BorderStyles
    {
        string _default = null;
        string left = null;
        string right = null;
        string top = null;
        string bottom = null;

        [CategoryAttribute("BorderStyle")]
        [DisplayNameAttribute("Default")]
        [DescriptionAttribute("Specifies the default style of the border.")]
        public string DefaultBorderStyle
        {
            get
            {
                return this._default;
            }
            set
            {
                if (this._default != value)
                {
                    OnPropertyChanging("DefaultBorderStyle");
                    this._default = value;
                    OnPropertyChanged("DefaultBorderStyle");
                }
            }
        }

        [CategoryAttribute("BorderStyle")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the style of left border.")]
        public string LeftBorderStyle
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("LeftBorderStyle");
                    this.left = value;
                    OnPropertyChanged("LeftBorderStyle");
                }
            }
        }

        [CategoryAttribute("BorderStyle")]
        [DisplayNameAttribute("Right")]
        [DescriptionAttribute("Specifies the style of right border.")]
        public string RightBorderStyle
        {
            get
            {
                return this.right;
            }
            set
            {
                if (this.right != value)
                {
                    OnPropertyChanging("RightBorderStyle");
                    this.right = value;
                    OnPropertyChanged("RightBorderStyle");
                }
            }
        }

        [CategoryAttribute("BorderStyle")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the style of top border.")]
        public string TopBorderStyle
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("TopBorderStyle");
                    this.top = value;
                    OnPropertyChanged("TopBorderStyle");
                }
            }
        }

        [CategoryAttribute("BorderStyle")]
        [DisplayNameAttribute("Bottom")]
        [DescriptionAttribute("Specifies the style of bottom border.")]
        public string BottomBorderStyle
        {
            get
            {
                return this.bottom;
            }
            set
            {
                if (this.bottom != value)
                {
                    OnPropertyChanging("BottomBorderStyle");
                    this.bottom = value;
                    OnPropertyChanged("BottomBorderStyle");
                }
            }
        }

        public override string ToString()
        {
            return (string.Empty);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        public void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    internal class Locations
    {
        string left = null;
        string top = null;

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the distance of the item from the left of the containing object.")]
        public string Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("Left");
                    this.left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the distance of the item from the top of the containing object.")]
        public string Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("Top");
                    this.top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        public override string ToString()
        {
            return (string.Empty);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        public void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    internal class Indents
    {
        string hanging = null;
        string left = null;
        string right = null;

        [CategoryAttribute("Indent")]
        [DisplayNameAttribute("HangingIndent")]
        [DescriptionAttribute("Hanging Indentation.")]
        public string HangingIndent
        {
            get
            {
                return this.hanging;
            }
            set
            {
                if (this.hanging != value)
                {
                    OnPropertyChanging("HangingIndent");
                    this.hanging = value;
                    OnPropertyChanged("HangingIndent");
                }
            }
        }

        [CategoryAttribute("Indent")]
        [DisplayNameAttribute("LeftIndent")]
        [DescriptionAttribute("Left Indentation.")]
        public string LeftIndent
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("LeftIndent");
                    this.left = value;
                    OnPropertyChanged("LeftIndent");
                }
            }
        }

        [CategoryAttribute("Indent")]
        [DisplayNameAttribute("RightIndent")]
        [DescriptionAttribute("Right Indentation.")]
        public string RightIndent
        {
            get
            {
                return this.right;
            }
            set
            {
                if (this.right != value)
                {
                    OnPropertyChanging("RightIndent");
                    this.right = value;
                    OnPropertyChanged("RightIndent");
                }
            }
        }

        public override string ToString()
        {
            return (string.Empty);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        public void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    internal class BackgroundImage
    {
        RDL.DOM.Source source = RDL.DOM.Source.Embedded;
        string imageValue = null;
        string mimeType = null;
        //string backgroundRepeat = null;

        [CategoryAttribute("BackgroundImage")]
        [DisplayNameAttribute("Source")]
        [DescriptionAttribute("Specifies the type of the image source.For example,the image source is external to the report.")]
        public RDL.DOM.Source Source
        {
            get
            {
                return this.source;
            }
            set
            {
                if (this.source != value)
                {
                    this.OnPropertyChanging("Source");
                    this.source = value;
                    this.OnPropertyChanged("Source");
                }
            }
        }

        [CategoryAttribute("BackgroundImage")]
        [DisplayNameAttribute("Value")]
        [DescriptionAttribute("Specifies the value of background image and depends on the source of the image.")]
        public string ImageValue
        {
            get
            {
                return this.imageValue;
            }
            set
            {
                if (value != this.imageValue)
                {
                    this.OnPropertyChanging("ImageValue");
                    this.imageValue = value;
                    this.OnPropertyChanged("ImageValue");
                }
            }
        }

        [CategoryAttribute("BackgroundImage")]
        [DisplayNameAttribute("MIMEType")]
        [DescriptionAttribute("The MIMEtype for the image.Required if the source is a database and ignored otherwise.")]
        public string MIMEType
        {
            get
            {
                return this.mimeType;
            }
            set
            {
                if (this.mimeType != value)
                {
                    this.OnPropertyChanging("MIMEType");
                    this.mimeType = value;
                    this.OnPropertyChanged("MIMEType");
                }
            }
        }

        public override string ToString()
        {
            return (string.Empty);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        public void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    internal class Sizes
    {
        string width = null;
        string height = null;

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specifies the width of the report item")]
        public string Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value != this.width)
                {
                    this.OnPropertyChanging("Width");
                    this.width = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specifies the height of the report item")]
        public string Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value != this.height)
                {
                    this.OnPropertyChanging("Height");
                    this.height = value;
                    this.OnPropertyChanged("Height");
                }
            }
        }

        public override string ToString()
        {
            return (string.Empty);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        public void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    #endregion

    #region TextBox Properties

    internal class TextRunProperties : IDesignerProperties
    {
        public TextRunProperties()
        {
            this.HorizontalAlignment = "Default";
            this.FontFamily = "Arial";
            this.FontSize = "10pt";
            this.FontStyle = "Default";
            this.FontWeight = "Default";
            this.FontEffects = "Default";
        }

        [CategoryAttribute("Alignment")]
        [DisplayNameAttribute("Horizontal Alignment")]
        [DescriptionAttribute("Specifies the horizontal alignment of text within the report item.")]
        public string HorizontalAlignment
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Family")]
        [DescriptionAttribute("Specifies the name of the font family.")]
        public string FontFamily
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Size")]
        [DescriptionAttribute("Specifies the point size of the font.")]
        public string FontSize
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Style")]
        [DescriptionAttribute("Specifies the style of the font.")]
        public string FontStyle
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Weight")]
        [DescriptionAttribute("Specifies thickness of the font.")]
        public string FontWeight
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Color")]
        [DescriptionAttribute("Specifies the color of the text.")]
        public string FontColor
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Effects")]
        [DescriptionAttribute("Specifies special text formatting such as underlining.")]
        public string FontEffects
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Line Spacing")]
        [DescriptionAttribute("Specifies the distance between the text lines.")]
        public int LineSpacing
        {
            get;
            set;
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Fill Color")]
        [DescriptionAttribute("Specifies the background color of the item.")]
        public string FillColor
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    internal class TextBoxProperties : IReportItemProperties
    {
        int listLevel = 0;
        string name = null;
        string tooltip = null;
        string horizontalalignment = null;
        string verticalalignment = null;
        string toggleState = null;
        string fontfamily = null;
        string fontsize = null;
        string fontstyle = null;
        string fontweight = null;
        string fontcolor = null;
        string fonteffects = null;
        string linespacing = null;
        string fillcolor = null;
        string hidden = null;
        string writingMode = null;
        string toggleItem = null;
        string keeptogether = null;
        string spaceBefore = null;
        string spaceAfter = null;
        string format = null;
        string canGrow = null;
        string canShrink = null;
        string hideDuplicate = null;
        string dataElementName = null;
        string dataElementOutput = null;
        string dataElementStyle = null;
        string documentMapLabel = null;
        RDL.DOM.ListStyle listStyle = RDL.DOM.ListStyle.None;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        public TextBoxProperties()
        {
            this.Size = new Sizes();
            this.Indents = new Indents();
            this.Padding = new Padding();
            this.Location = new Locations();
            this.BorderStyles = new BorderStyles();
            this.BorderWidths = new BorderWidths();
            this.BorderColors = new BorderColors();
            this.BackgroundImage = new BackgroundImage();

            this.Size.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Size.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);   
            this.Indents.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Indents.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);         
            this.Padding.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Padding.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.Location.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Location.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);            
            this.BorderStyles.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderStyles.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderWidths.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderWidths.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderColors.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderColors.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BackgroundImage.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BackgroundImage.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
        }

        public void UpdatePropertyValue()
        {
            this.HorizontalAlignment = "Default";
            this.VerticalAlignment = "Default";
            this.FontFamily = "Arial";
            this.FontSize = "10pt";
            this.FontStyle = "Default";
            this.FontWeight = "Default";
            this.FontEffects = "Default";
            this.LineSpacing = "10pt";
            this.Hidden = "False";
            this.FillColor = "Transparent";
            this.FontColor = "Black";
            this.KeepTogether = "True";
            this.CanShrink = "False";
            this.CanGrow = "True";
            this.ListLevel = 0;
            this.ListStyle = RDL.DOM.ListStyle.None;
            this.WritingMode = "Default";
            this.InitialToggleState = "False";
            this.DataElementOutput = "Auto";
            this.DataElementStyle = "Auto";
            this.BorderStyles.DefaultBorderStyle = "None";
            this.BorderWidths.DefaultBorderWidth = "1pt";
            this.BorderColors.DefaultBorderColor = "Black";
            this.Padding.PaddingLeft = this.Padding.PaddingRight = this.Padding.PaddingTop = this.Padding.PaddingBottom = "2pt";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("CanGrow")]
        [DescriptionAttribute("Indicates whether the report item automatically increases in size to accommodate a long value.")]
        public string CanGrow
        {
            get
            {
                return this.canGrow;
            }
            set
            {
                if (value != this.canGrow)
                {
                    this.OnPropertyChanging("CanGrow");
                    this.canGrow = value;
                    this.OnPropertyChanged("CanGrow");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("CanShrink")]
        [DescriptionAttribute("Indicates whether the report item automatically decreases in size to accommodate a short value.")]
        public string CanShrink
        {
            get
            {
                return this.canShrink;
            }
            set
            {
                if (value != this.canShrink)
                {
                    this.OnPropertyChanging("CanShrink");
                    this.canShrink = value;
                    this.OnPropertyChanged("CanShrink");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DocumentMapLable")]
        [DescriptionAttribute("Identifeis an instance of a report item within the document map.")]
        public string DocumentMapLabel
        {
            get
            {
                return this.documentMapLabel;
            }
            set
            {
                if (value != this.documentMapLabel)
                {
                    this.OnPropertyChanging("DocumentMapLable");
                    this.documentMapLabel = value;
                    this.OnPropertyChanged("DocumentMapLable");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("HideDuplicates")]
        [DescriptionAttribute("Idicates whether an item is displayed when the current value of the item is the same as its value in preceding row.")]
        public string HideDuplicates
        {
            get
            {
                return this.hideDuplicate;
            }
            set
            {
                if (value != this.hideDuplicate)
                {
                    this.OnPropertyChanging("HideDuplicates");
                    this.hideDuplicate = value;
                    this.OnPropertyChanged("HideDuplicates");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("KeepTogether")]
        [DescriptionAttribute("Indicates whether to keep all sections of the data region together on one page .")]
        public string KeepTogether
        {
            get
            {
                return this.keeptogether;
            }
            set
            {
                if (value != this.keeptogether)
                {
                    this.OnPropertyChanging("KeepTogether");
                    this.keeptogether = value;
                    this.OnPropertyChanged("KeepTogether");
                }
            }
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("ToolTip")]
        [DescriptionAttribute("Specifies the textual label for the report item. For example, it can be used to render TITLE and ALT attributes in HTML reports.")]
        public string ToolTip
        {
            get
            {
                return this.tooltip;
            }
            set
            {
                if (value != this.tooltip)
                {
                    this.OnPropertyChanging("ToolTip");
                    this.tooltip = value;
                    this.OnPropertyChanged("ToolTip");
                }
            }
        }

        [CategoryAttribute("Alignment")]
        [DisplayNameAttribute("Text Alignment")]
        [DescriptionAttribute("Specifies the horizontal alignment of text within the report item.")]
        public string HorizontalAlignment
        {
            get
            {
                return this.horizontalalignment;
            }
            set
            {
                if (value != this.horizontalalignment)
                {
                    this.OnPropertyChanging("HorizontalAlignment");
                    this.horizontalalignment = value;
                    this.OnPropertyChanged("HorizontalAlignment");
                }
            }
        }

        [CategoryAttribute("Alignment")]
        [DisplayNameAttribute("Vertical Alignment")]
        [DescriptionAttribute("Specifies the vertical alignment of text within the report item.")]
        public string VerticalAlignment
        {
            get
            {
                return this.verticalalignment;
            }
            set
            {
                if (value != this.verticalalignment)
                {
                    this.OnPropertyChanging("VerticalAlignment");
                    this.verticalalignment = value;
                    this.OnPropertyChanged("VerticalAlignment");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Alignment")]
        [DisplayNameAttribute("Indent")]
        [DescriptionAttribute("Paragraph indentation.")]
        public Indents Indents
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Alignment")]
        [DisplayNameAttribute("Padding")]
        [DescriptionAttribute("Specifies the amount of padding between the report item boundary and its contents.")]
        public Padding Padding
        {
            get;
            set;
        }

        [CategoryAttribute("Alignment")]
        [DisplayNameAttribute("SpaceAfter")]
        [DescriptionAttribute("Space after Paragraph.")]
        public string SpaceAfter
        {
            get
            {
                return this.spaceAfter;
            }
            set
            {
                if (value != this.spaceAfter)
                {
                    OnPropertyChanging("SpaceAfter");
                    this.spaceAfter = value;
                    OnPropertyChanged("SpaceAfter");
                }
            }
        }

        [CategoryAttribute("Alignment")]
        [DisplayNameAttribute("SpaceBefore")]
        [DescriptionAttribute("Space before Paragraph.")]
        public string SpaceBefore
        {
            get
            {
                return this.spaceBefore;
            }
            set
            {
                if (value != this.spaceBefore)
                {
                    OnPropertyChanging("SpaceBefore");
                    this.spaceBefore = value;
                    OnPropertyChanged("SpaceBefore");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementName")]
        [DescriptionAttribute("Specify the name of the data element or attribute of report item.")]
        public string DataElementName
        {
            get
            {
                return this.dataElementName;
            }
            set
            {
                if (value != this.dataElementName)
                {
                    OnPropertyChanging("DataElementName");
                    this.dataElementName = value;
                    OnPropertyChanged("DataElementName");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementOutput")]
        [DescriptionAttribute("Indicates whether and how data element should appear when rendered.")]
        public string DataElementOutput
        {
            get
            {
                return this.dataElementOutput;
            }
            set
            {
                if (value != this.dataElementOutput)
                {
                    OnPropertyChanging("DataElementOutput");
                    this.dataElementOutput = value;
                    OnPropertyChanged("DataElementOutput");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementStyle")]
        [DescriptionAttribute("Indicates whether a text box within the report renders as an element or an attribute when the report is rendered to XML.")]
        public string DataElementStyle
        {
            get
            {
                return this.dataElementStyle;
            }
            set
            {
                if (value != this.dataElementStyle)
                {
                    OnPropertyChanging("DataElementStyle");
                    this.dataElementStyle = value;
                    OnPropertyChanged("DataElementStyle");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Family")]
        [DescriptionAttribute("Specifies the name of the font family.")]
        public string FontFamily
        {
            get
            {
                return this.fontfamily;
            }
            set
            {
                if (value != this.fontfamily)
                {
                    this.OnPropertyChanging("FontFamily");
                    this.fontfamily = value;
                    this.OnPropertyChanged("FontFamily");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Size")]
        [DescriptionAttribute("Specifies the point size of the font.")]
        public string FontSize
        {
            get
            {
                return this.fontsize;
            }
            set
            {
                if (value != this.fontsize)
                {
                    this.OnPropertyChanging("FontSize");
                    this.fontsize = value;
                    this.OnPropertyChanged("FontSize");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Style")]
        [DescriptionAttribute("Specifies the style of the font.")]
        public string FontStyle
        {
            get
            {
                return this.fontstyle;
            }
            set
            {
                if (value != this.fontstyle)
                {
                    this.OnPropertyChanging("FontStyle");
                    this.fontstyle = value;
                    this.OnPropertyChanged("FontStyle");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Weight")]
        [DescriptionAttribute("Specifies thickness of the font.")]
        public string FontWeight
        {
            get
            {
                return this.fontweight;
            }
            set
            {
                if (value != this.fontweight)
                {
                    this.OnPropertyChanging("FontWeight");
                    this.fontweight = value;
                    this.OnPropertyChanged("FontWeight");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Color")]
        [DescriptionAttribute("Specifies the color of the text.")]
        public string FontColor
        {
            get
            {
                return this.fontcolor;
            }
            set
            {
                if (value != this.fontcolor)
                {
                    this.OnPropertyChanging("FontColor");

                    if (value == "#00FFFFFF")
                    {
                        value = this.fontcolor;
                    }

                    this.fontcolor = value;
                    this.OnPropertyChanged("FontColor");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("TextDecoration")]
        [DescriptionAttribute("Specifies special text formatting such as underlining.")]
        public string FontEffects
        {
            get
            {
                return this.fonteffects;
            }
            set
            {
                if (value != this.fonteffects)
                {
                    this.OnPropertyChanging("FontEffects");
                    this.fonteffects = value;
                    this.OnPropertyChanged("FontEffects");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Line Height")]
        [DescriptionAttribute("Specifies the distance between the text lines.")]
        public string LineSpacing
        {
            get
            {
                return this.linespacing;
            }
            set
            {
                if (value != this.linespacing)
                {
                    this.OnPropertyChanging("LineSpacing");
                    this.linespacing = value;
                    this.OnPropertyChanged("LineSpacing");
                }
            }
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("BackgroundColor")]
        [DescriptionAttribute("Specifies the background color of the item.")]
        public string FillColor
        {
            get
            {
                return this.fillcolor;
            }
            set
            {
                if (value != this.fillcolor)
                {
                    this.OnPropertyChanging("FillColor");

                    if (value == "#00FFFFFF")
                    {
                        value = this.fillcolor;
                    }

                    this.fillcolor = value;
                    this.OnPropertyChanged("FillColor");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("BackgroundImage")]
        [DescriptionAttribute("Specifies the background image of the report item.")]
        public BackgroundImage BackgroundImage
        {
            get;
            set;
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Hidden")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the report item is initially hidden.")]
        public string Hidden
        {
            get
            {
                return this.hidden;
            }
            set
            {
                if (value != this.hidden)
                {
                    this.OnPropertyChanging("Hidden");
                    this.hidden = value;
                    this.OnPropertyChanged("Hidden");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("InitialToggleState")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates initial state,expanded(+) or collapsed(-),of the toggle image.The state also can be set by expression that evaluates to Boolean.")]
        public string InitialToggleState
        {
            get
            {
                return this.toggleState;
            }
            set
            {
                if (value != this.toggleState)
                {
                    this.OnPropertyChanging("InitialToggleState");
                    this.toggleState = value;
                    this.OnPropertyChanged("InitialToggleState");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("ToggleItem")]
        [DescriptionAttribute("Specifies name of the report item to click to show or hide another report item.")]
        public string ToggleItem
        {
            get
            {
                return this.toggleItem;
            }
            set
            {
                if (value != this.toggleItem)
                {
                    this.OnPropertyChanging("ToggleItem");
                    this.toggleItem = value;
                    this.OnPropertyChanged("ToggleItem");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public BorderStyles BorderStyles
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the size of the border.")]
        public BorderWidths BorderWidths
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("BorderColor")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public BorderColors BorderColors
        {
            get;
            set;
        }

        [CategoryAttribute("Lists")]
        [DisplayNameAttribute("ListLevel")]
        [DescriptionAttribute("The bullet or numbering style for the list paragraph,and its indentation level.")]
        public int ListLevel
        {
            get
            {
                return this.listLevel;
            }
            set
            {
                if (value != this.listLevel)
                {
                    this.OnPropertyChanging("ListLevel");
                    this.listLevel = value;
                    this.OnPropertyChanged("ListLevel");
                }
            }
        }

        [CategoryAttribute("Lists")]
        [DisplayNameAttribute("ListStyle")]
        [DescriptionAttribute("The style of list.")]
        public RDL.DOM.ListStyle ListStyle
        {
            get
            {
                return this.listStyle;
            }
            set
            {
                if (value != this.listStyle)
                {
                    this.OnPropertyChanging("ListStyle");
                    this.listStyle = value;
                    this.OnPropertyChanged("ListStyle");
                }
            }
        }

        [CategoryAttribute("Number")]
        [DisplayNameAttribute("Format")]
        [DescriptionAttribute("Specifies the expression that formats the string in the report item.")]
        public string Format
        {
            get
            {
                return this.format;
            }
            set
            {
                if (value != this.format)
                {
                    this.OnPropertyChanging("Format");
                    this.format = value;
                    this.OnPropertyChanged("Format");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Location")]
        [DescriptionAttribute("Specifies the position of the top-left corner of a report item in relation to its container.")]
        public Locations Location
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Size")]
        [DescriptionAttribute("Specifies the Size of the report item")]
        public Sizes Size
        {
            get;
            set;
        }

        string width = null;
        string height = null;

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specifies the width of the report item")]
        public string Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value != this.width)
                {
                    this.OnPropertyChanging("Width");
                    this.width = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specifies the height of the report item")]
        public string Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value != this.height)
                {
                    this.OnPropertyChanging("Height");
                    this.height = value;
                    this.OnPropertyChanged("Height");
                }
            }
        }

        string left = null;
        string top = null;

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the distance of the item from the left of the containing object.")]
        public string Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("Left");
                    this.left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the distance of the item from the top of the containing object.")]
        public string Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("Top");
                    this.top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        [CategoryAttribute("Localization")]
        [DisplayNameAttribute("WritingMode")]
        [DescriptionAttribute("Specifies whether the text displayed vertically or horizontally.")]
        public string WritingMode
        {
            get
            {
                return this.writingMode;
            }
            set
            {
                if (this.writingMode != value)
                {
                    OnPropertyChanging("WritingMode");
                    this.writingMode = value;
                    OnPropertyChanged("WritingMode");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    #endregion

    #region Sub report properties

    internal class SubReportProperties : IReportItemProperties
    {
        string name = null;
        string tooltip = null;
        string hidden = null;
        string format = null;
        string reportName = null;
        string toggleItem = null;
        string omitOnBreak = null;
        string keeptogether = null;
        string dataElementName = null;
        string dataElementOutput = null;
        string documentMapLabel = null;
        string verticalalignment = null;
        string horizontalalignment = null;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        public SubReportProperties()
        {
            this.Size = new Sizes();
            this.Padding = new Padding();
            this.Location = new Locations();
            this.BorderColors = new BorderColors();
            this.BorderWidths = new BorderWidths();
            this.BorderStyles = new BorderStyles();

            this.Size.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Size.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.Padding.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Padding.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderStyles.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderStyles.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderWidths.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderWidths.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderColors.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderColors.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.Location.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Location.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
        }

        public void UpdatePropertyValue()
        {
            this.KeepTogether = "True";
            this.Hidden = "False";
            this.DataElementOutput = "Auto";
            this.OmitBorderOnPageBreak = "False";
            this.BorderStyles.DefaultBorderStyle = "None";
            this.BorderWidths.DefaultBorderWidth = "1pt";
            this.BorderColors.DefaultBorderColor = "Black";
            this.Padding.PaddingLeft = this.Padding.PaddingRight = this.Padding.PaddingTop = this.Padding.PaddingBottom = "0pt";
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("KeepTogether")]
        [DescriptionAttribute("Indicates whether to keep all sections of the data region together on one page .")]
        public string KeepTogether
        {
            get
            {
                return this.keeptogether;
            }
            set
            {
                if (value != this.keeptogether)
                {
                    this.OnPropertyChanging("KeepTogether");
                    this.keeptogether = value;
                    this.OnPropertyChanged("KeepTogether");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("General")]
        [DisplayNameAttribute("ToolTip")]
        [DescriptionAttribute("Specifies the textual label for the report item. For example, it can be used to render TITLE and ALT attributes in HTML reports.")]
        public string ToolTip
        {
            get
            {
                return tooltip;
            }
            set
            {
                this.OnPropertyChanging("ToolTip");
                this.tooltip = value;
                this.OnPropertyChanged("ToolTip");
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("OmitBorderOnPageBreak")]
        [DescriptionAttribute("Indicates whether border should appear around report items that span multiple pages.")]
        public string OmitBorderOnPageBreak
        {
            get
            {
                return this.omitOnBreak;
            }
            set
            {
                this.OnPropertyChanging("OmitBorderOnPageBreak");
                this.omitOnBreak = value;
                this.OnPropertyChanged("OmitBorderOnPageBreak");
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("ReportName")]
        [DescriptionAttribute("Specifies path to the subreport.")]
        public string ReportName
        {
            get
            {
                return this.reportName;
            }
            set
            {
                this.OnPropertyChanging("ReportName");
                this.reportName = value;
                this.OnPropertyChanged("ReportName");
            }
        }

        [CategoryAttribute("Data")]
        [DisplayNameAttribute("DatasetName")]
        [DescriptionAttribute("Specifies the name of the Dataset.")]
        public string Dataset
        {
            get;
            set;
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementName")]
        [DescriptionAttribute("Specify the name of the data element or attribute of report item.")]
        public string DataElementName
        {
            get
            {
                return this.dataElementName;
            }
            set
            {
                if (value != this.dataElementName)
                {
                    OnPropertyChanging("DataElementName");
                    this.dataElementName = value;
                    OnPropertyChanged("DataElementName");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementOutput")]
        [DescriptionAttribute("Indicates whether and how data element should appear when rendered.")]
        public string DataElementOutput
        {
            get
            {
                return this.dataElementOutput;
            }
            set
            {
                if (value != this.dataElementOutput)
                {
                    OnPropertyChanging("DataElementOutput");
                    this.dataElementOutput = value;
                    OnPropertyChanged("DataElementOutput");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DocumentMapLabel")]
        [DescriptionAttribute("Identifeis an instance of a report item within the document map.")]
        public string DocumentMapLabel
        {
            get
            {
                return this.documentMapLabel;
            }
            set
            {
                if (value != this.documentMapLabel)
                {
                    this.OnPropertyChanging("DocumentMapLabel");
                    this.documentMapLabel = value;
                    this.OnPropertyChanged("DocumentMapLabel");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("BorderStyle")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public BorderStyles BorderStyles
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("BorderWidth")]
        [DescriptionAttribute("Specifies the size of the border.")]
        public BorderWidths BorderWidths
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("BorderColor")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public BorderColors BorderColors
        {
            get;
            set;
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Hidden")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the report item is initially hidden.")]
        public string Hidden
        {
            get
            {
                return this.hidden;
            }
            set
            {
                if (value != this.hidden)
                {
                    this.OnPropertyChanging("Hidden");
                    this.hidden = value;
                    this.OnPropertyChanged("Hidden");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("ToggleItem")]
        [DescriptionAttribute("Specifies name of the report item to click to show or hide another report item.")]
        public string ToggleItem
        {
            get
            {
                return this.toggleItem;
            }
            set
            {
                if (value != this.toggleItem)
                {
                    this.OnPropertyChanging("ToggleItem");
                    this.toggleItem = value;
                    this.OnPropertyChanged("ToggleItem");
                }
            }
        }

        [CategoryAttribute("No Rows")]
        [DisplayNameAttribute("Format")]
        [DescriptionAttribute("Specifies the expression that formats the string in the report item.")]
        public string Format
        {
            get
            {
                return this.format;
            }
            set
            {
                if (value != this.format)
                {
                    this.OnPropertyChanging("Format");
                    this.format = value;
                    this.OnPropertyChanged("Format");
                }
            }
        }

        [CategoryAttribute("No Rows")]
        [DisplayNameAttribute("Text Alignment")]
        [DescriptionAttribute("Specifies the horizontal alignment of text within the report item.")]
        public string HorizontalAlignment
        {
            get
            {
                return this.horizontalalignment;
            }
            set
            {
                if (value != this.horizontalalignment)
                {
                    this.OnPropertyChanging("HorizontalAlignment");
                    this.horizontalalignment = value;
                    this.OnPropertyChanged("HorizontalAlignment");
                }
            }
        }

        [CategoryAttribute("No Rows")]
        [DisplayNameAttribute("Vertical Alignment")]
        [DescriptionAttribute("Specifies the vertical alignment of text within the report item.")]
        public string VerticalAlignment
        {
            get
            {
                return this.verticalalignment;
            }
            set
            {
                if (value != this.verticalalignment)
                {
                    this.OnPropertyChanging("VerticalAlignment");
                    this.verticalalignment = value;
                    this.OnPropertyChanged("VerticalAlignment");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("No Rows")]
        [DisplayNameAttribute("Padding")]
        [DescriptionAttribute("Specifies the amount of padding between the report item boundary and its contents.")]
        public Padding Padding
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Location")]
        [DescriptionAttribute("Specifies the position of the top-left corner of a report item in relation to its container.")]
        public Locations Location
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Size")]
        [DescriptionAttribute("Specifies the Size of the report item")]
        public Sizes Size
        {
            get;
            set;
        }


        string width = null;
        string height = null;

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specifies the width of the report item")]
        public string Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value != this.width)
                {
                    this.OnPropertyChanging("Width");
                    this.width = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specifies the height of the report item")]
        public string Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value != this.height)
                {
                    this.OnPropertyChanging("Height");
                    this.height = value;
                    this.OnPropertyChanged("Height");
                }
            }
        }

        string left = null;
        string top = null;

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the distance of the item from the left of the containing object.")]
        public string Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("Left");
                    this.left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the distance of the item from the top of the containing object.")]
        public string Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("Top");
                    this.top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    #endregion

    #region Map Properties

    internal class MapProperties : IReportItemProperties
    {
        string name = null;
        string borderColor = null;
        string borderWidth = null;
        string borderStyle = null;
        string toolTip = null;
        string pageName = null;
        string direction = null;
        string language = null;
        string numeralLanguage = null;
        string tileLanguage = null;
        string documentMapLabel = null;
        string textAntiAliasingQuality = null;
        string antiAliasing = null;
        string dataElementName = null;
        string dataElementOutput = null;
        string hidden = null;
        string toggleItem = null;
        string breakLocation = null;
        string disabled = null;
        string resetPageNmber = null;
        string numeralVariant = null;
        string maximumSpatialElementCount = null;
        string maximumTotalPointCount = null;
        string calender = null;
        string backgroundcolor = null;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        public MapProperties()
        {

        }

        public void UpdatePropertyValue()
        {
            this.antiAliasing = "All";
            this.textAntiAliasingQuality = "High";
            this.dataElementOutput = "Auto";
            this.breakLocation = "None";
            this.disabled = "False";
            this.resetPageNmber = "False";
            this.direction = "Default";
            this.hidden = "false";
            this.numeralVariant = "1";
            this.calender = "Default";
            this.maximumSpatialElementCount = "20000";
            this.maximumTotalPointCount = "1000000";
            this.backgroundcolor = "Transparent";
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Size")]
        [DescriptionAttribute("Specifies the Size of the report item")]
        public Sizes Size
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Location")]
        [DescriptionAttribute("Specifies the Size of the report item")]
        public Locations Location
        {
            get;
            set;
        }

        [CategoryAttribute("Appearance")]
        [DisplayNameAttribute("TextAntiAliasingQuality")]
        [DescriptionAttribute("The quality of all text anti-aliasing")]
        public string TextAntiAliasingQuality
        {
            get
            {
                return this.textAntiAliasingQuality;
            }
            set
            {
                if (value != this.textAntiAliasingQuality)
                {
                    this.OnPropertyChanging("TextAntiAliasingQuality");
                    this.textAntiAliasingQuality = value;
                    this.OnPropertyChanged("TextAntiAliasingQuality");
                }
            }
        }

        [CategoryAttribute("Appearance")]
        [DisplayNameAttribute("AntiAliasing")]
        [DescriptionAttribute("Specifies the smoothing(anti-aliasing) mode.Applied to all map elements")]
        public string AntiAliasing
        {
            get
            {
                return this.antiAliasing;
            }
            set
            {
                if (value != this.antiAliasing)
                {
                    this.OnPropertyChanging("AntiAliasing");
                    this.antiAliasing = value;
                    this.OnPropertyChanged("AntiAliasing");
                }
            }
        }
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the border width of the report item.")]
        public string BorderWidth
        {
            get
            {
                return this.borderWidth;
            }
            set
            {
                if (value != this.borderWidth)
                {
                    this.OnPropertyChanging("BorderWidth");
                    this.borderWidth = value;
                    this.OnPropertyChanged("BorderWidth");
                }
            }
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Color")]
        [DescriptionAttribute("Specifies the border color of the report item.")]
        public string BorderColor
        {
            get
            {
                return this.borderColor;
            }
            set
            {
                if (value != this.borderColor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("BorderColor");
                    this.borderColor = value;
                    this.OnPropertyChanged("BorderColor");
                }
            }
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public string BorderStyle
        {
            get
            {
                return this.borderStyle;
            }
            set
            {
                if (value != this.borderStyle)
                {
                    this.OnPropertyChanging("BorderStyle");
                    this.borderStyle = value;
                    this.OnPropertyChanged("BorderStyle");
                }
            }
        }
        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Background Color")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public string BackgroundColor
        {
            get
            {
                return this.backgroundcolor;
            }
            set
            {
                if (value != this.backgroundcolor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("BackgroundColor");
                    this.backgroundcolor = value;
                    this.OnPropertyChanged("BackgroundColor");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementName")]
        [DescriptionAttribute("Specifies the name of the data element or attribute of the report item.")]
        public string DataElementName
        {
            get
            {
                return this.dataElementName;
            }
            set
            {
                if (value != this.dataElementName)
                {
                    this.OnPropertyChanging("DataElementName");
                    this.dataElementName = value;
                    this.OnPropertyChanged("DataElementName");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementOutput")]
        [DescriptionAttribute("Indicates whether and how the data element should appear when rendered.")]
        public string DataElementOutput
        {
            get
            {
                return this.dataElementOutput;
            }
            set
            {
                if (value != this.dataElementOutput)
                {
                    this.OnPropertyChanging("DataElementOutput");
                    this.dataElementOutput = value;
                    this.OnPropertyChanged("DataElementOutput");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [CategoryAttribute("PageBreak")]
        [DisplayNameAttribute("BreakLocation")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public string BreakLocation
        {
            get
            {
                return this.breakLocation;
            }
            set
            {
                if (value != this.breakLocation)
                {
                    this.OnPropertyChanging("BreakLocation");
                    this.breakLocation = value;
                    this.OnPropertyChanged("BreakLocation");
                }
            }
        }

        [CategoryAttribute("PageBreak")]
        [DisplayNameAttribute("Disabled")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the rendering extension ignores the page break properties.")]
        public string Disabled
        {
            get
            {
                return this.disabled;
            }
            set
            {
                if (value != this.disabled)
                {
                    this.OnPropertyChanging("Disabled");
                    this.disabled = value;
                    this.OnPropertyChanged("Disabled");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("ToolTip")]
        [DescriptionAttribute("Specifies the textual label for the report item. For example, it can be used to render TITLE and ALT attributes in HTML reports.")]
        public string ToolTip
        {
            get
            {
                return this.toolTip;
            }
            set
            {
                if (value != this.toolTip)
                {
                    this.OnPropertyChanging("ToolTip");
                    this.toolTip = value;
                    this.OnPropertyChanged("ToolTip");
                }
            }
        }

        [CategoryAttribute("PageBreak")]
        [DisplayNameAttribute("ResetPageNumber")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the rendering extension resets the page number to 1 on a page break.")]
        public string ResetPageNumber
        {
            get
            {
                return this.resetPageNmber;
            }
            set
            {
                if (value != this.disabled)
                {
                    this.OnPropertyChanging("ResetPageNumber");
                    this.resetPageNmber = value;
                    this.OnPropertyChanged("ResetPageNumber");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("PageName")]
        [DescriptionAttribute("Specifies the string that the rendering extension uses as the page name.")]
        public string PageName
        {
            get
            {
                return this.pageName;
            }
            set
            {
                if (value != this.pageName)
                {
                    this.OnPropertyChanging("PageName");
                    this.pageName = value;
                    this.OnPropertyChanged("PageName");
                }
            }
        }

        [CategoryAttribute("International")]
        [DisplayNameAttribute("Direction")]
        [DescriptionAttribute("Indicates whether the control should draw right-to-left for RTL languages.")]
        public string Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                if (value != this.direction)
                {
                    this.OnPropertyChanging("Direction");
                    this.direction = value;
                    this.OnPropertyChanged("Direction");
                }
            }
        }

        [CategoryAttribute("International")]
        [DisplayNameAttribute("TileLanguage")]
        [DescriptionAttribute("Specifies the preferred language to use when loading title from the tile server.")]
        public string TileLanguage
        {
            get
            {
                return this.tileLanguage;
            }
            set
            {
                if (value != this.tileLanguage)
                {
                    this.OnPropertyChanging("TileLanguage");
                    this.tileLanguage = value;
                    this.OnPropertyChanged("TileLanguage");
                }
            }
        }

        [CategoryAttribute("International")]
        [DisplayNameAttribute("NumeralLanguage")]
        [DescriptionAttribute("Specifies the language to use to format numbers.")]
        public string NumeralLanguage
        {
            get
            {
                return this.numeralLanguage;
            }
            set
            {
                if (value != this.numeralLanguage)
                {
                    this.OnPropertyChanging("NumeralLanguage");
                    this.numeralLanguage = value;
                    this.OnPropertyChanged("NumeralLanguage");
                }
            }
        }

        [CategoryAttribute("International")]
        [DisplayNameAttribute("NumeralVariant")]
        [DescriptionAttribute("Specifies the langauge variant to use to format numbers.")]
        public string NumeralVariant
        {
            get
            {
                return this.numeralVariant;
            }
            set
            {
                if (value != this.numeralVariant)
                {
                    this.OnPropertyChanging("NumeralVariant");
                    this.numeralVariant = value;
                    this.OnPropertyChanged("NumeralVariant");
                }
            }
        }

        [CategoryAttribute("Localization")]
        [DisplayNameAttribute("Language")]
        [DescriptionAttribute("Indicates the primary language of the text.")]
        public string Language
        {
            get
            {
                return this.language;
            }
            set
            {
                if (value != this.language)
                {
                    this.OnPropertyChanging("Language");
                    this.language = value;
                    this.OnPropertyChanged("Language");
                }
            }
        }

        [CategoryAttribute("Localization")]
        [DisplayNameAttribute("Calender")]
        [DescriptionAttribute("Specifies the calender to use to format dates.")]
        public string Calender
        {
            get
            {
                return this.calender;
            }
            set
            {
                if (value != this.calender)
                {
                    this.OnPropertyChanging("Calender");
                    this.calender = value;
                    this.OnPropertyChanged("Calender");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DocumentMapLabel")]
        [DescriptionAttribute("Identifeis an instance of a report item within the document map.")]
        public string DocumentMapLabel
        {
            get
            {
                return this.documentMapLabel;
            }
            set
            {
                if (value != this.documentMapLabel)
                {
                    this.OnPropertyChanging("DocumentMapLabel");
                    this.documentMapLabel = value;
                    this.OnPropertyChanged("DocumentMapLabel");
                }
            }
        }

        [CategoryAttribute("Spatial")]
        [DisplayNameAttribute("MaximumSpatialElementCount")]
        [DescriptionAttribute("Specifies maximum number of spatial elements to load in the map during report processing")]
        public string MaximumSpatialElementCount
        {
            get
            {
                return this.maximumSpatialElementCount;
            }
            set
            {
                if (value != this.maximumSpatialElementCount)
                {
                    this.OnPropertyChanging("MaximumSpatialElementCount");
                    this.maximumSpatialElementCount = value;
                    this.OnPropertyChanged("MaximumSpatialElementCount");
                }
            }
        }

        [CategoryAttribute("Spatial")]
        [DisplayNameAttribute("MaximumTotalPointCount")]
        [DescriptionAttribute("Specifies maximum number of spatial elements to load in the map during report processing")]
        public string MaximumTotalPointCount
        {
            get
            {
                return this.maximumTotalPointCount;
            }
            set
            {
                if (value != this.maximumTotalPointCount)
                {
                    this.OnPropertyChanging("MaximumTotalPointCount");
                    this.maximumTotalPointCount = value;
                    this.OnPropertyChanged("MaximumTotalPointCount");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Hidden")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the report item is initially hidden.")]
        public string Hidden
        {
            get
            {
                return this.hidden;
            }
            set
            {
                if (value != this.hidden)
                {
                    this.OnPropertyChanging("Hidden");
                    this.hidden = value;
                    this.OnPropertyChanged("Hidden");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("ToggleItem")]
        [DescriptionAttribute("Specifies name of the report item to click to show or hide another report item.")]
        public string ToggleItem
        {
            get
            {
                return this.toggleItem;
            }
            set
            {
                if (value != this.toggleItem)
                {
                    this.OnPropertyChanging("ToggleItem");
                    this.toggleItem = value;
                    this.OnPropertyChanged("ToggleItem");
                }
            }
        }


        string width = null;
        string height = null;

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specifies the width of the report item")]
        public string Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value != this.width)
                {
                    this.OnPropertyChanging("Width");
                    this.width = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specifies the height of the report item")]
        public string Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value != this.height)
                {
                    this.OnPropertyChanging("Height");
                    this.height = value;
                    this.OnPropertyChanged("Height");
                }
            }
        }

        string left = null;
        string top = null;

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the distance of the item from the left of the containing object.")]
        public string Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("Left");
                    this.left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the distance of the item from the top of the containing object.")]
        public string Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("Top");
                    this.top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

    }

    #endregion

    #region Image Properties

    internal class ImageProperties : IReportItemProperties
    {
        string name = null;
        string hidden = null;
        string tooltip = null;
        string mimeType = null;
        string toggleItem = null;
        string imageValue = null;
        string documentMapLabel = null;
        RDL.DOM.Source source = RDL.DOM.Source.Embedded;
        RDL.DOM.Sizing sizing = RDL.DOM.Sizing.FitProportional;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        public ImageProperties()
        {
            this.Size = new Sizes();
            this.Padding = new Padding();
            this.Location = new Locations();
            this.BorderWidths = new BorderWidths();
            this.BorderColors = new BorderColors();
            this.BorderStyles = new BorderStyles();

            this.Size.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Size.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.Padding.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Padding.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderStyles.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderStyles.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderWidths.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderWidths.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderColors.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderColors.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.Location.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Location.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
        }

        public void UpdatePropertyValue()
        {            
            this.ImageValue = null;
            this.Source = RDL.DOM.Source.Embedded;
            this.Hidden = "False";
            this.BorderStyles.DefaultBorderStyle = "None";
            this.BorderWidths.DefaultBorderWidth = "1pt";
            this.BorderColors.DefaultBorderColor = "Black";
            this.Padding.PaddingLeft = this.Padding.PaddingRight = this.Padding.PaddingTop = this.Padding.PaddingBottom = "0pt";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [Browsable(false)]
        [DisplayNameAttribute("ToolTip")]
        [DescriptionAttribute("Specifies the textual label for the report item. For example, it can be used to render TITLE and ALT attributes in HTML reports.")]
        public string ToolTip
        {
            get
            {
                return tooltip;
            }
            set
            {
                this.OnPropertyChanging("ToolTip");
                this.tooltip = value;
                this.OnPropertyChanged("ToolTip");
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Source")]
        [DescriptionAttribute("Specifies the type of the image source.For example,the image source is external to the report.")]
        public RDL.DOM.Source Source
        {
            get
            {
                return source;
            }
            set
            {
                this.OnPropertyChanging("Source");
                this.source = value;
                this.OnPropertyChanged("Source");
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("ImageValue")]
        [DescriptionAttribute("Specifies the imported image value.")]
        public string ImageValue
        {
            get
            {
                return this.imageValue;
            }
            set
            {
                if (value != this.imageValue)
                {
                    this.OnPropertyChanging("ImageValue");
                    this.imageValue = value;
                    this.OnPropertyChanged("ImageValue");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("MIMEType")]
        [DescriptionAttribute("specifies the MIMEtype of the image.")]
        public string MIMEType
        {
            get
            {
                return this.mimeType;
            }
            set
            {
                if (this.mimeType != value)
                {
                    this.OnPropertyChanging("MIMEType");
                    this.mimeType = value;
                    this.OnPropertyChanged("MIMEType");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Color")]
        [DescriptionAttribute("Specifies the color of the entire border or the individual border lines of the item.")]
        public BorderColors BorderColors
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("BorderWidth")]
        [DescriptionAttribute("Specifies the thickness of the border of the item.")]
        public BorderWidths BorderWidths
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public BorderStyles BorderStyles
        {
            get;
            set;
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Sizing")]
        [DescriptionAttribute("Specifies the appearance of the image if it does not fit within the height.")]
        public RDL.DOM.Sizing Sizing
        {
            get
            {
                return this.sizing;
            }
            set
            {
                if (value != this.sizing)
                {
                    this.OnPropertyChanging("Sizing");
                    this.sizing = value;
                    this.OnPropertyChanged("Sizing");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Padding")]
        [DescriptionAttribute("Specifies the Padding of the report item.")]
        public Padding Padding
        {
            get;
            set;
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Hidden")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the report item is initially hidden.")]
        public string Hidden
        {
            get
            {
                return this.hidden;
            }
            set
            {
                if (value != this.hidden)
                {
                    this.OnPropertyChanging("Hidden");
                    this.hidden = value;
                    this.OnPropertyChanged("Hidden");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("ToggleItem")]
        [DescriptionAttribute("Specifies name of the report item to click to show or hide another report item.")]
        public string ToggleItem
        {
            get
            {
                return this.toggleItem;
            }
            set
            {
                if (value != this.toggleItem)
                {
                    this.OnPropertyChanging("ToggleItem");
                    this.toggleItem = value;
                    this.OnPropertyChanged("ToggleItem");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DocumentMapLabel")]
        [DescriptionAttribute("Identifeis an instance of a report item within the document map.")]
        public string DocumentMapLabel
        {
            get
            {
                return this.documentMapLabel;
            }
            set
            {
                if (value != this.documentMapLabel)
                {
                    this.OnPropertyChanging("DocumentMapLabel");
                    this.documentMapLabel = value;
                    this.OnPropertyChanged("DocumentMapLabel");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Location")]
        [DescriptionAttribute("Specifies the position of the top-left corner of a report item in relation to its container.")]
        public Locations Location
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Size")]
        [DescriptionAttribute("Specifies the Size of the report item")]
        public Sizes Size
        {
            get;
            set;
        }


        string width = null;
        string height = null;

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specifies the width of the report item")]
        public string Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value != this.width)
                {
                    this.OnPropertyChanging("Width");
                    this.width = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specifies the height of the report item")]
        public string Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value != this.height)
                {
                    this.OnPropertyChanging("Height");
                    this.height = value;
                    this.OnPropertyChanged("Height");
                }
            }
        }

        string left = null;
        string top = null;

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the distance of the item from the left of the containing object.")]
        public string Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("Left");
                    this.left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the distance of the item from the top of the containing object.")]
        public string Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("Top");
                    this.top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    #endregion

    #region Line Properties

    internal class LineProperties : IReportItemProperties
    {
        string name = null;
        string horizontal = null;
        string vertical = null;
        string height = null;
        string width = null;

        string hidden = null;
        string linestyle = null;
        string linecolor = null;
        string toggleItem = null;
        string linethickness = null;
        string documentMapLabel = null;

        public LineProperties()
        {
            this.Location = new Locations();
            this.Location.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Location.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
        }

        public void UpdatePropertyValue()
        {
            this.LineStyle = "Solid";
            this.LineColor = "Black";
            this.LineWidth = "1pt";
            this.Hidden = "False";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specifies the width of the report item")]
        public string Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value != this.width)
                {
                    this.OnPropertyChanging("Width");
                    this.width = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specifies the height of the report item")]
        public string Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value != this.height)
                {
                    this.OnPropertyChanging("Height");
                    this.height = value;
                    this.OnPropertyChanged("Height");
                }
            }
        }

        [CategoryAttribute("EndPoint")]
        [DisplayNameAttribute("Horizontal")]
        [DescriptionAttribute("Specifies the distance between lower-right end of the line and the left of the containing object.")]
        public string Horizontal
        {
            get
            {
                return this.horizontal;
            }
            set
            {
                if (value != this.horizontal)
                {
                    this.OnPropertyChanging("Horizontal");
                    this.horizontal = value;
                    this.OnPropertyChanged("Horizontal");
                }
            }
        }

        [CategoryAttribute("EndPoint")]
        [DisplayNameAttribute("Vertical")]
        [DescriptionAttribute("Specifies the distance between lower-right end of the line and the top of the containing object.")]
        public string Vertical
        {
            get
            {
                return this.vertical;
            }
            set
            {
                if (value != this.vertical)
                {
                    this.OnPropertyChanging("Vertical");
                    this.vertical = value;
                    this.OnPropertyChanged("Vertical");
                }
            }
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("Style")]
        [DisplayNameAttribute("Line Style")]
        [DescriptionAttribute("Specifies the style of the report item.")]
        public string LineStyle
        {
            get
            {
                return this.linestyle;
            }
            set
            {
                if (value != this.linestyle)
                {
                    this.OnPropertyChanging("LineStyle");
                    this.linestyle = value;
                    this.OnPropertyChanged("LineStyle");
                }
            }
        }

        [CategoryAttribute("Style")]
        [DisplayNameAttribute("Line Width")]
        [DescriptionAttribute("Specifies the width of the report item.")]
        public string LineWidth
        {
            get
            {
                return this.linethickness;
            }
            set
            {
                if (value != this.linethickness)
                {
                    this.OnPropertyChanging("LineWidth");
                    this.linethickness = value;
                    this.OnPropertyChanged("LineWidth");
                }
            }
        }

        [CategoryAttribute("Style")]
        [DisplayNameAttribute("Line Color")]
        [DescriptionAttribute("Specifies the color of the report item.")]
        public string LineColor
        {
            get
            {
                return this.linecolor;
            }
            set
            {
                if (value != this.linecolor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("LineColor");
                    this.linecolor = value;
                    this.OnPropertyChanged("LineColor");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Hidden")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the report item is initially hidden.")]
        public string Hidden
        {
            get
            {
                return this.hidden;
            }
            set
            {
                if (value != this.hidden)
                {
                    this.OnPropertyChanging("Hidden");
                    this.hidden = value;
                    this.OnPropertyChanged("Hidden");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("ToggleItem")]
        [DescriptionAttribute("Specifies name of the report item to click to show or hide another report item.")]
        public string ToggleItem
        {
            get
            {
                return this.toggleItem;
            }
            set
            {
                if (value != this.toggleItem)
                {
                    this.OnPropertyChanging("ToggleItem");
                    this.toggleItem = value;
                    this.OnPropertyChanged("ToggleItem");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DocumentMapLabel")]
        [DescriptionAttribute("Identifeis an instance of a report item within the document map.")]
        public string DocumentMapLabel
        {
            get
            {
                return this.documentMapLabel;
            }
            set
            {
                if (value != this.documentMapLabel)
                {
                    this.OnPropertyChanging("DocumentMapLabel");
                    this.documentMapLabel = value;
                    this.OnPropertyChanged("DocumentMapLabel");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Location")]
        [DescriptionAttribute("Specifies the position of the top-left corner of a report item in relation to its container.")]
        public Locations Location
        {
            get;
            set;
        }

        string left = null;
        string top = null;

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the distance of the item from the left of the containing object.")]
        public string Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("Left");
                    this.left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the distance of the item from the top of the containing object.")]
        public string Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("Top");
                    this.top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    #endregion

    #region Chart Properties

    internal class ChartProperties : IReportItemProperties
    {
        string name = null;
        string charttype = null;
        string tooltip = "Chart Tooltip";
        string borderwidth = null;
        string bordercolor = null;
        string borderStyle = null;
        string background = null;
        string hidden = null;
        string fillstyle = null;
        string toggleItem = null;
        string primarycolor = null;
        string gradientstyle = null;
        string secondarycolor = null;
        string adornmenttype = null;
        string documentMapLabel = null;
        string dataElementName = null;
        string dataElementOutput = null;
        string dataElementStyle = null;
        RDL.DOM.BreakLocation pageBreak = RDL.DOM.BreakLocation.None;

        public ChartProperties()
        {
            this.Size = new Sizes();
            this.Location = new Locations();
            this.BackgroundImage = new BackgroundImage();

            this.Size.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Size.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.Location.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Location.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BackgroundImage.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BackgroundImage.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);

            this.ChartType = "Column";
            this.BorderWidth = "1pt";
            this.FillStyle = "Solid";
            this.GradientStyle = "LeftRight";
            this.PrimaryColor = "White";
            this.SecondaryColor = "White";
            this.ChartBackground = "White";
            this.BorderStyle = "None";
            this.BorderColor = "LightGray";
            this.AdornmentType = "None";
            this.Hidden = "False";
            this.DataElementOutput = "Auto";
            this.DataElementStyle = "Auto";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [Browsable(false)]
        public string DesignerMode
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Chart Type")]
        [DescriptionAttribute("Specifies the type of the chart.")]
        public string ChartType
        {
            get
            {
                return this.charttype;
            }
            set
            {
                if (value != this.charttype)
                {
                    this.OnPropertyChanging("ChartType");
                    this.charttype = value;
                    this.OnPropertyChanged("ChartType");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("ToolTip")]
        [DescriptionAttribute("Specifies the type of the chart.")]
        public string ToolTip
        {
            get
            {
                return this.tooltip;
            }
            set
            {
                if (value != this.tooltip)
                {
                    this.OnPropertyChanging("ToolTip");
                    this.tooltip = value;
                    this.OnPropertyChanged("ToolTip");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("PageBreak")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public RDL.DOM.BreakLocation PageBreak
        {
            get
            {
                return this.pageBreak;
            }
            set
            {
                if (value != this.pageBreak)
                {
                    this.OnPropertyChanging("PageBreak");
                    this.pageBreak = value;
                    this.OnPropertyChanged("PageBreak");
                }
            }
        }

        [CategoryAttribute("Data")]
        [DisplayNameAttribute("DatasetName")]
        [DescriptionAttribute("Specifies the name of the Dataset.")]
        public string Dataset
        {
            get;
            set;
        }
        
        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementName")]
        [DescriptionAttribute("Specify the name of the data element or attribute of report item.")]
        public string DataElementName
        {
            get
            {
                return this.dataElementName;
            }
            set
            {
                if (value != this.dataElementName)
                {
                    OnPropertyChanging("DataElementName");
                    this.dataElementName = value;
                    OnPropertyChanged("DataElementName");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementOutput")]
        [DescriptionAttribute("Indicates whether and how data element should appear when rendered.")]
        public string DataElementOutput
        {
            get
            {
                return this.dataElementOutput;
            }
            set
            {
                if (value != this.dataElementOutput)
                {
                    OnPropertyChanging("DataElementOutput");
                    this.dataElementOutput = value;
                    OnPropertyChanged("DataElementOutput");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementStyle")]
        [DescriptionAttribute("Indicates whether a text box within the report renders as an element or an attribute when the report is rendered to XML.")]
        public string DataElementStyle
        {
            get
            {
                return this.dataElementStyle;
            }
            set
            {
                if (value != this.dataElementStyle)
                {
                    OnPropertyChanging("DataElementStyle");
                    this.dataElementStyle = value;
                    OnPropertyChanged("DataElementStyle");
                }
            }
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the border width of the report item.")]
        public string BorderWidth
        {
            get
            {
                return this.borderwidth;
            }
            set
            {
                if (value != this.borderwidth)
                {
                    this.OnPropertyChanging("BorderWidth");
                    this.borderwidth = value;
                    this.OnPropertyChanged("BorderWidth");
                }
            }
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Color")]
        [DescriptionAttribute("Specifies the border color of the report item.")]
        public string BorderColor
        {
            get
            {
                return this.bordercolor;
            }
            set
            {
                if (value != this.bordercolor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("BorderColor");
                    this.bordercolor = value;
                    this.OnPropertyChanged("BorderColor");
                }
            }
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public string BorderStyle
        {
            get
            {
                return this.borderStyle;
            }
            set
            {
                if (value != this.borderStyle)
                {
                    this.OnPropertyChanging("BorderStyle");
                    this.borderStyle = value;
                    this.OnPropertyChanged("BorderStyle");
                }
            }
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Chart Background")]
        [DescriptionAttribute("Specifies the background color of the report item.")]
        public string ChartBackground
        {
            get
            {
                return this.background;
            }
            set
            {
                if (value != this.background)
                {
                    this.OnPropertyChanging("ChartBackground");
                    if (value == "#00FFFFFF")
                    {
                        value = this.background;
                    }

                    this.background = value;
                    this.OnPropertyChanged("ChartBackground");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("BackgroundImage")]
        [DescriptionAttribute("Specifies background image of the report item.")]
        public BackgroundImage BackgroundImage
        {
            get;
            set;
        }

        [CategoryAttribute("AreaColor")]
        [DisplayNameAttribute("Fill Style")]
        [DescriptionAttribute("Specifies the style of the report item.")]
        public string FillStyle
        {
            get
            {
                return this.fillstyle;
            }
            set
            {
                if (value != this.fillstyle)
                {
                    this.OnPropertyChanging("FillStyle");
                    this.fillstyle = value;
                    this.OnPropertyChanged("FillStyle");
                }
            }
        }

        [CategoryAttribute("AreaColor")]
        [DisplayNameAttribute("Gradient Style")]
        [DescriptionAttribute("Specifies the gradient style of the report item.")]
        public string GradientStyle
        {
            get
            {
                return this.gradientstyle;
            }
            set
            {
                if (value != this.gradientstyle)
                {
                    this.OnPropertyChanging("GradientStyle");
                    this.gradientstyle = value;
                    this.OnPropertyChanged("GradientStyle");
                }
            }
        }

        [CategoryAttribute("AreaColor")]
        [DisplayNameAttribute("Primary Color")]
        [DescriptionAttribute("Specifies the primary color of the report item.")]
        public string PrimaryColor
        {
            get
            {
                return this.primarycolor;
            }
            set
            {
                if (value != this.primarycolor)
                {
                    this.OnPropertyChanging("PrimaryColor");
                    if (value == "#00FFFFFF")
                    {
                        value = this.primarycolor;
                    }

                    this.primarycolor = value;
                    this.OnPropertyChanged("PrimaryColor");
                }
            }
        }

        [CategoryAttribute("AreaColor")]
        [DisplayNameAttribute("Secondary Color")]
        [DescriptionAttribute("Specifies the secondary color of the report item.")]
        public string SecondaryColor
        {
            get
            {
                return this.secondarycolor;
            }
            set
            {
                if (value != this.secondarycolor)
                {
                    this.OnPropertyChanging("SecondaryColor");
                    if (value == "#00FFFFFF")
                    {
                        value = this.secondarycolor;
                    }

                    this.secondarycolor = value;
                    this.OnPropertyChanged("SecondaryColor");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Adornment")]
        [DescriptionAttribute("Specifies the Adornment of the chart series")]
        public string AdornmentType
        {
            get
            {
                return this.adornmenttype;
            }
            set
            {
                if (value != this.adornmenttype)
                {
                    this.OnPropertyChanging("AdornmentType");
                    this.adornmenttype = value;
                    this.OnPropertyChanged("AdornmentType");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Hidden")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the report item is initially hidden.")]
        public string Hidden
        {
            get
            {
                return this.hidden;
            }
            set
            {
                if (value != this.hidden)
                {
                    this.OnPropertyChanging("Hidden");
                    this.hidden = value;
                    this.OnPropertyChanged("Hidden");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("ToggleItem")]
        [DescriptionAttribute("Specifies name of the report item to click to show or hide another report item.")]
        public string ToggleItem
        {
            get
            {
                return this.toggleItem;
            }
            set
            {
                if (value != this.toggleItem)
                {
                    this.OnPropertyChanging("ToggleItem");
                    this.toggleItem = value;
                    this.OnPropertyChanged("ToggleItem");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DocumentMapLabel")]
        [DescriptionAttribute("Identifeis an instance of a report item within the document map.")]
        public string DocumentMapLabel
        {
            get
            {
                return this.documentMapLabel;
            }
            set
            {
                if (value != this.documentMapLabel)
                {
                    this.OnPropertyChanging("DocumentMapLabel");
                    this.documentMapLabel = value;
                    this.OnPropertyChanged("DocumentMapLabel");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Location")]
        [DescriptionAttribute("Specifies the position of the top-left corner of a report item in relation to its container.")]
        public Locations Location
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Size")]
        [DescriptionAttribute("Specifies the Size of the report item")]
        public Sizes Size
        {
            get;
            set;
        }


        string width = null;
        string height = null;

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specifies the width of the report item")]
        public string Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value != this.width)
                {
                    this.OnPropertyChanging("Width");
                    this.width = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specifies the height of the report item")]
        public string Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value != this.height)
                {
                    this.OnPropertyChanging("Height");
                    this.height = value;
                    this.OnPropertyChanged("Height");
                }
            }
        }

        string left = null;
        string top = null;

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the distance of the item from the left of the containing object.")]
        public string Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("Left");
                    this.left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the distance of the item from the top of the containing object.")]
        public string Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("Top");
                    this.top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    internal class DefaultChartProperties : IDesignerProperties
    {
        string name = null;
        string fillstyle = null;
        string gradientstyle = null;
        string primarycolor = null;
        string secondarycolor = null;
        //string chartType = null;

        public DefaultChartProperties()
        {
            this.FillStyle = "Gradient";
            this.GradientStyle = "LeftRight";
            this.PrimaryColor = "White";
            this.SecondaryColor = "White";
            this.Name = "Default";
            this.CategoryAxisMajorGridLines = new MajorGridLines()
            {
                EnableMajorGridLines = "False",
                MajorGridLinesStyle = "Solid",
                MajorGridLinesColor = "Silver",
                MajorGridLinesWidth = "1pt"
            };
            this.CategoryAxisMinorGridLines = new MinorGridLines()
            {
                EnableMinorGridLines = "False",
                MinorGridLinesStyle = "Solid",
                MinorGridLinesColor = "Silver",
                MinorGridLinesWidth = "1pt"
            };
            this.ValueAxisMajorGridLines = new MajorGridLines()
            {
                EnableMajorGridLines = "True",
                MajorGridLinesStyle = "Solid",
                MajorGridLinesColor = "Silver",
                MajorGridLinesWidth = "1pt"
            };
            this.ValueAxisMinorGridLines = new MinorGridLines()
            {
                EnableMinorGridLines = "False",
                MinorGridLinesStyle = "Solid",
                MinorGridLinesColor = "Silver",
                MinorGridLinesWidth = "1pt"
            };
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [CategoryAttribute("AreaColor")]
        [DisplayNameAttribute("Fill Style")]
        [DescriptionAttribute("Specifies the style of the report item.")]
        public string FillStyle
        {
            get
            {
                return this.fillstyle;
            }
            set
            {
                if (value != this.fillstyle)
                {
                    this.OnPropertyChanging("FillStyle");
                    this.fillstyle = value;
                    this.OnPropertyChanged("FillStyle");
                }
            }
        }

        [CategoryAttribute("AreaColor")]
        [DisplayNameAttribute("Gradient Style")]
        [DescriptionAttribute("Specifies the gradient style of the report item.")]
        public string GradientStyle
        {
            get
            {
                return this.gradientstyle;
            }
            set
            {
                if (value != this.gradientstyle)
                {
                    this.OnPropertyChanging("GradientStyle");
                    this.gradientstyle = value;
                    this.OnPropertyChanged("GradientStyle");
                }
            }
        }

        [CategoryAttribute("AreaColor")]
        [DisplayNameAttribute("Primary Color")]
        [DescriptionAttribute("Specifies the primary color of the report item.")]
        public string PrimaryColor
        {
            get
            {
                return this.primarycolor;
            }
            set
            {
                if (value != this.primarycolor)
                {
                    this.OnPropertyChanging("PrimaryColor");
                    if (value == "#00FFFFFF")
                    {
                        value = this.primarycolor;
                    }
                    this.primarycolor = value;
                    this.OnPropertyChanged("PrimaryColor");
                }
            }
        }

        [CategoryAttribute("AreaColor")]
        [DisplayNameAttribute("Secondary Color")]
        [DescriptionAttribute("Specifies the secondary color of the report item.")]
        public string SecondaryColor
        {
            get
            {
                return this.secondarycolor;
            }
            set
            {
                if (value != this.secondarycolor)
                {
                    this.OnPropertyChanging("SecondaryColor");
                    if (value == "#00FFFFFF")
                    {
                        value = this.secondarycolor;
                    }

                    this.secondarycolor = value;
                    this.OnPropertyChanged("SecondaryColor");
                }
            }
        }

        [Browsable(false)]
        public string ChartType
        {
            get;
            set;
        }

        [CategoryAttribute("Category Axis")]
        [DisplayName("MajorGridLines")]
        [DescriptionAttribute("Category Axis Major Grid Lines Style.")]
        public MajorGridLines CategoryAxisMajorGridLines 
        { 
            get; 
            set; 
        }

        [CategoryAttribute("Category Axis")]
        [DisplayName("MinorGridLines")]
        [DescriptionAttribute("Category Axis Minor Grid Lines Style.")]
        public MinorGridLines CategoryAxisMinorGridLines 
        {
            get; 
            set; 
        }

        [CategoryAttribute("Value Axis")]
        [DisplayName("MajorGridLines")]
        [DescriptionAttribute("Value Axis Major Grid Lines Style.")]
        public MajorGridLines ValueAxisMajorGridLines 
        { 
            get; 
            set; 
        }

        [CategoryAttribute("Value Axis")]
        [DisplayName("MinorGridLines")]
        [DescriptionAttribute("Value Axis Minor Grid Lines Style.")]
        public MinorGridLines ValueAxisMinorGridLines 
        { 
            get; 
            set; 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    internal class MajorGridLines : IDesignerProperties
    {
        string enableMajorGridLines = null;
        string gridLineStyle = null;
        string gridLineWidth = null;
        string gridlineColor = null;

        [CategoryAttribute("MajorGridLines")]
        [DisplayNameAttribute("Enabled")]
        [DescriptionAttribute("Indicates whether the major grid lines are enabled.")]
        public string EnableMajorGridLines
        {
            get
            {
                return this.enableMajorGridLines;
            }
            set
            {
                if (value != this.enableMajorGridLines)
                {
                    this.OnPropertyChanging("EnableMajorGridLines");
                    this.enableMajorGridLines = value;
                    this.OnPropertyChanged("EnableMajorGridLines");
                }
            }
        }

        [CategoryAttribute("MajorGridLines")]
        [DisplayNameAttribute("LineStyle")]
        [DescriptionAttribute("Specifies the major grid lines style.")]
        public string MajorGridLinesStyle
        {
            get
            {
                return this.gridLineStyle;
            }
            set
            {
                if (value != this.gridLineStyle)
                {
                    this.OnPropertyChanging("MajorGridLinesStyle");
                    this.gridLineStyle = value;
                    this.OnPropertyChanged("MajorGridLinesStyle");
                }
            }
        }

        [CategoryAttribute("MajorGridLines")]
        [DisplayNameAttribute("LineWidth")]
        [DescriptionAttribute("Specifies the width of the major grid lines.")]
        public string MajorGridLinesWidth
        {
            get
            {
                return this.gridLineWidth;
            }
            set
            {
                if (value != this.gridLineWidth)
                {
                    this.OnPropertyChanging("MajorGridLinesWidth");
                    this.gridLineWidth = value;
                    this.OnPropertyChanged("MajorGridLinesWidth");
                }
            }
        }

        [CategoryAttribute("MajorGridLines")]
        [DisplayNameAttribute("LineColor")]
        [DescriptionAttribute("Specifies the color of the major grid lines.")]
        public string MajorGridLinesColor
        {
            get
            {
                return this.gridlineColor;
            }
            set
            {
                if (value != this.gridlineColor)
                {
                    this.OnPropertyChanging("MajorGridLinesColor");
                    this.gridlineColor = value;
                    this.OnPropertyChanged("MajorGridLinesColor");
                }
            }
        }

        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        public override string ToString()
        {
            return this.EnableMajorGridLines + ", " + this.MajorGridLinesWidth + ", " + this.MajorGridLinesStyle + ", " + this.MajorGridLinesColor;
        }
    }

    internal class MinorGridLines : IDesignerProperties
    {
        string enableMinorGridLines = null;
        string minorgridLineStyle = null;
        string minorgridLineWidth = null;
        string minorgridlineColor = null;

        [CategoryAttribute("MinorGridLines")]
        [DisplayNameAttribute("Enabled")]
        [DescriptionAttribute("Indicates whether the minor grid lines are enabled.")]
        public string EnableMinorGridLines
        {
            get
            {
                return this.enableMinorGridLines;
            }
            set
            {
                if (value != this.enableMinorGridLines)
                {
                    this.OnPropertyChanging("EnableMinorGridLines");
                    this.enableMinorGridLines = value;
                    this.OnPropertyChanged("EnableMinorGridLines");
                }
            }
        }

        [CategoryAttribute("MinorGridLines")]
        [DisplayNameAttribute("LineStyle")]
        [DescriptionAttribute("Specifies the minor grid lines style.")]
        public string MinorGridLinesStyle
        {
            get
            {
                return this.minorgridLineStyle;
            }
            set
            {
                if (value != this.minorgridLineStyle)
                {
                    this.OnPropertyChanging("MinorGridLinesStyle");
                    this.minorgridLineStyle = value;
                    this.OnPropertyChanged("MinorGridLinesStyle");
                }
            }
        }

        [CategoryAttribute("MinorGridLines")]
        [DisplayNameAttribute("LineWidth")]
        [DescriptionAttribute("Specifies the width of the minor grid lines.")]
        public string MinorGridLinesWidth
        {
            get
            {
                return this.minorgridLineWidth;
            }
            set
            {
                if (value != this.minorgridLineWidth)
                {
                    this.OnPropertyChanging("MinorGridLinesWidth");
                    this.minorgridLineWidth = value;
                    this.OnPropertyChanged("MinorGridLinesWidth");
                }
            }
        }

        [CategoryAttribute("MinorGridLines")]
        [DisplayNameAttribute("LineColor")]
        [DescriptionAttribute("Specifies the color of the minor grid lines.")]
        public string MinorGridLinesColor
        {
            get
            {
                return this.minorgridlineColor;
            }
            set
            {
                if (value != this.minorgridlineColor)
                {
                    this.OnPropertyChanging("MinorGridLinesColor");
                    this.minorgridlineColor = value;
                    this.OnPropertyChanged("MinorGridLinesColor");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        public override string ToString()
        {
            return this.EnableMinorGridLines + ", " + this.MinorGridLinesWidth + ", " + this.MinorGridLinesStyle + ", " + this.MinorGridLinesColor;
        }
    }

    internal class ChartLegendProperties : IDesignerProperties
    {
        string legendname = "ChartLegend";
        string showlegend = "True";
        string position = null;
        string layout = null;
        string legendbackground = "White";
        string fontfamily = null;
        string fontsize = "8pt";
        string fontstyle = null;
        string fontcolor = "Black";
        string legendborderwidth = "1pt";
        string legendbordercolor = "Black";

        public ChartLegendProperties()
        {
            this.Position = "TopCenter";
            this.Layout = "Row";
            this.FontFamily = "Arial";
            this.FontSize = "8pt";
            this.FontStyle = "Default";
            this.BorderThickness = "1pt";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.legendname;
            }
            set
            {
                if (value != this.legendname)
                {
                    this.OnPropertyChanging("Name");
                    this.legendname = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Show Legend")]
        [DescriptionAttribute("Specifies whether the legend will show or not.")]
        public string Hidden
        {
            get
            {
                return this.showlegend;
            }
            set
            {
                if (value != this.showlegend)
                {
                    this.OnPropertyChanging("ShowLegend");
                    this.showlegend = value;
                    this.OnPropertyChanged("ShowLegend");
                }
            }
        }

        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Position")]
        [DescriptionAttribute("Specifies the position of the legend.")]
        public string Position
        {
            get
            {
                return this.position;
            }
            set
            {
                if (value != this.position)
                {
                    this.OnPropertyChanging("Position");
                    this.position = value;
                    this.OnPropertyChanged("Position");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Layout")]
        [DescriptionAttribute("Specifies the layout of the legend.")]
        public string Layout
        {
            get
            {
                return this.layout;
            }
            set
            {
                if (value != this.layout)
                {
                    this.OnPropertyChanging("Layout");
                    this.layout = value;
                    this.OnPropertyChanged("Layout");
                }
            }
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Background Color")]
        [DescriptionAttribute("Specifies the background color of the legend.")]
        public string BackFill
        {
            get
            {
                return this.legendbackground;
            }
            set
            {
                if (value != this.legendbackground && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("BackFill");
                    this.legendbackground = value;
                    this.OnPropertyChanged("BackFill");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Family")]
        [DescriptionAttribute("Specifies the name of the font family.")]
        public string FontFamily
        {
            get
            {
                return this.fontfamily;
            }
            set
            {
                if (value != this.fontfamily)
                {
                    this.OnPropertyChanging("FonFamily");
                    this.fontfamily = value;
                    this.OnPropertyChanged("FontFamily");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Size")]
        [DescriptionAttribute("Specifies the point size of the font.")]
        public string FontSize
        {
            get
            {
                return this.fontsize;
            }
            set
            {
                if (value != this.fontsize)
                {
                    this.OnPropertyChanging("FontSize");
                    this.fontsize = value;
                    this.OnPropertyChanged("FontSize");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Style")]
        [DescriptionAttribute("Specifies the style of the font.")]
        public string FontStyle
        {
            get
            {
                return this.fontstyle;
            }
            set
            {
                if (value != this.fontstyle)
                {
                    this.OnPropertyChanging("FontStyle");
                    this.fontstyle = value;
                    this.OnPropertyChanged("FontStyle");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Color")]
        [DescriptionAttribute("Specifies the color of the text.")]
        public string FontColor
        {
            get
            {
                return this.fontcolor;
            }
            set
            {
                if (value != this.fontcolor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("FontColor");
                    this.fontcolor = value;
                    this.OnPropertyChanged("FontColor");
                }
            }
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the border width of the entire border or the individual border widths of the item.")]
        public string BorderThickness
        {
            get
            {
                return this.legendborderwidth;
            }
            set
            {
                if (value != this.legendborderwidth)
                {
                    this.OnPropertyChanging("BorderThickness");
                    this.legendborderwidth = value;
                    this.OnPropertyChanged("BorderThickness");
                }
            }
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Color")]
        [DescriptionAttribute("Specifies the color of the entire border or the individual border lines of the item.")]
        public string LegendBorderColor
        {
            get
            {
                return this.legendbordercolor;
            }
            set
            {
                if (value != this.legendbordercolor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("LegendBorderColor");
                    this.legendbordercolor = value;
                    this.OnPropertyChanged("LegendBorderColor");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    internal class ChartSeriesPropertiesCollection : List<ChartSeriesProperties>
    {
    }

    internal class ChartSeriesProperties : IDesignerProperties
    {
        string name = null;
        string charttype = null;
        string seriescolor = null;
        string borderwidth = null;
        string bordercolor = null;
        string adornmenttype = null;
        string adornmentcolor = null;
        string size = "10pt";

        public ChartSeriesProperties()
        {
            this.ChartType = "Column";
            this.SeriesColor = "Green";
            this.BorderWidth = "1pt";
            this.BorderColor = "Black";
            this.AdornmentColor = "Red";
            this.AdornmentType = "None";
            this.Size = "20pt";
            this.DataLabelsPosition = "Default";
            this.ShowDataLabels = "False";
            this.CategoryAxisName = "Primary";
            this.ValueAxisName = "Primary";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the series.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Chart Type")]
        [DescriptionAttribute("Specifies the chart type of the series.")]
        public string ChartType
        {
            get
            {
                return this.charttype;
            }
            set
            {
                if (value != this.charttype)
                {
                    this.OnPropertyChanging("ChartType");
                    this.charttype = value;
                    this.OnPropertyChanged("ChartType");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Series Color")]
        [DescriptionAttribute("Specifies the color of the chart series.")]
        public string SeriesColor
        {
            get
            {
                return this.seriescolor;
            }
            set
            {
                if (value != this.seriescolor)
                {
                    this.OnPropertyChanging("SeriesColor");
                    this.seriescolor = value;
                    this.OnPropertyChanged("SeriesColor");
                }
            }
        }


        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the border width of the report item.")]
        public string BorderWidth
        {
            get
            {
                return this.borderwidth;
            }
            set
            {
                if (value != this.borderwidth)
                {
                    this.OnPropertyChanging("BorderWidth");
                    this.borderwidth = value;
                    this.OnPropertyChanged("BorderWidth");
                }
            }
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Color")]
        [DescriptionAttribute("Specifies the border color of the report item.")]
        public string BorderColor
        {
            get
            {
                return this.bordercolor;
            }
            set
            {
                if (value != this.bordercolor)
                {
                    this.OnPropertyChanging("BorderColor");
                    this.bordercolor = value;
                    this.OnPropertyChanged("BorderColor");
                }
            }
        }

        [CategoryAttribute("Adornment")]
        [DisplayNameAttribute("Adornment Type")]
        [DescriptionAttribute("Specifies the Adornment type of the chart series.")]
        public string AdornmentType
        {
            get
            {
                return this.adornmenttype;
            }
            set
            {
                if (value != this.adornmenttype)
                {
                    this.OnPropertyChanging("AdornmentType");
                    this.adornmenttype = value;
                    this.OnPropertyChanged("AdornmentType");
                }
            }
        }

        [CategoryAttribute("Adornment")]
        [DisplayNameAttribute("Adornment Color")]
        [DescriptionAttribute("Specifies the Adornment color of the chart series.")]
        public string AdornmentColor
        {
            get
            {
                return this.adornmentcolor;
            }
            set
            {
                if (value != this.adornmentcolor)
                {
                    this.OnPropertyChanging("AdornmentColor");
                    this.adornmentcolor = value;
                    this.OnPropertyChanged("AdornmentColor");
                }
            }
        }

        [CategoryAttribute("Adornment")]
        [DisplayNameAttribute("Size")]
        [DescriptionAttribute("Specifies the size of the Adornment.")]
        public string Size
        {
            get
            {
                return this.size;
            }
            set
            {
                if (value != this.size)
                {
                    this.OnPropertyChanging("Size");
                    this.size = value;
                    this.OnPropertyChanged("Size");
                }
            }
        }

        string showDataLabels = null;
        [CategoryAttribute("Data Labels")]
        [DisplayNameAttribute("Show Data Labels")]
        [DescriptionAttribute("Indicates whether the datalabels are hidden are not.")]
        public string ShowDataLabels
        {
            get
            {
                return this.showDataLabels;
            }
            set
            {
                if (value != this.showDataLabels)
                {
                    this.OnPropertyChanging("ShowDataLabels");
                    this.showDataLabels = value;
                    this.OnPropertyChanged("ShowDataLabels");
                }
            }
        }

        string position = null;
        [CategoryAttribute("Data Labels")]
        [DisplayNameAttribute("Position")]
        [DescriptionAttribute("The Position of the data labels of chart series.")]
        public string DataLabelsPosition
        {
            get
            {
                return this.position;
            }
            set
            {
                if (value != this.position)
                {
                    this.OnPropertyChanging("Position");
                    this.position = value;
                    this.OnPropertyChanged("Position");
                }
            }
        }

        string categoryAxisName = null;
        [CategoryAttribute("Axes")]
        [DisplayNameAttribute("CategoryAxisName")]
        [DescriptionAttribute("Category axis name used to plot the series.")]
        public string CategoryAxisName
        {
            get
            {
                return this.categoryAxisName;
            }
            set
            {
                if (value != this.categoryAxisName)
                {
                    this.OnPropertyChanging("CategoryAxisName");
                    this.categoryAxisName = value;
                    this.OnPropertyChanged("CategoryAxisName");
                }
            }
        }

        string valueAxisName = null;
        [CategoryAttribute("Axes")]
        [DisplayNameAttribute("ValueAxisName")]
        [DescriptionAttribute("Value axis name used to plot the series.")]
        public string ValueAxisName
        {
            get
            {
                return this.valueAxisName;
            }
            set
            {
                if (value != this.valueAxisName)
                {
                    this.OnPropertyChanging("ValueAxisName");
                    this.valueAxisName = value;
                    this.OnPropertyChanged("ValueAxisName");
                }
            }
        }

        [Browsable(false)]
        public string SeriesType
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    internal class ChartTitleProperties : IDesignerProperties
    {
        string title = "ChartTitle";
        string titlefontfamily = "Segoe UI";
        string titlefontsize = "16pt";
        string titlefontstyle = "Default";
        string titlefontcolor = "Black";
        string titlebackcolor = "White";
        string visibility = "True";

        public ChartTitleProperties()
        {
            this.TitleFontFamily = "Arial";
            this.TitleFontSize = "10pt";
            this.TitleFontStyle = "Default";
            this.Name = "ChartTitle";
            this.TitleFontColor = "Black";
            this.TitleBackFill = "White";
            this.Visibility = "True";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Title")]
        [DescriptionAttribute("Specifies the title of the chart.")]
        public string Name
        {
            get
            {
                return this.title;
            }
            set
            {
                if (value != this.title)
                {
                    this.OnPropertyChanging("Name");
                    this.title = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Family")]
        [DescriptionAttribute("Specifies the name of the font family.")]
        public string TitleFontFamily
        {
            get
            {
                return this.titlefontfamily;
            }
            set
            {
                if (value != this.titlefontfamily)
                {
                    this.OnPropertyChanging("TitleFontFamily");
                    this.titlefontfamily = value;
                    this.OnPropertyChanged("TitleFontFamily");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Size")]
        [DescriptionAttribute("Specifies the point size of the label.")]
        public string TitleFontSize
        {
            get
            {
                return this.titlefontsize;
            }
            set
            {
                if (value != this.titlefontsize)
                {
                    this.OnPropertyChanging("TitleFontSize");
                    this.titlefontsize = value;
                    this.OnPropertyChanged("TitleFontSize");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Style")]
        [DescriptionAttribute("Specifies the style of the label.")]
        public string TitleFontStyle
        {
            get
            {
                return this.titlefontstyle;
            }
            set
            {
                if (value != this.titlefontstyle)
                {
                    this.OnPropertyChanging("TitleFontStyle");
                    this.titlefontstyle = value;
                    this.OnPropertyChanged("TitleFontStyle");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Color")]
        [DescriptionAttribute("Specifies the color of the label.")]
        public string TitleFontColor
        {
            get
            {
                return this.titlefontcolor;
            }
            set
            {
                if (value != this.titlefontcolor)
                {
                    this.OnPropertyChanging("TitleFontColor");
                    this.titlefontcolor = value;
                    this.OnPropertyChanged("TitleFontColor");
                }
            }
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Back Fill")]
        [DescriptionAttribute("Specifies the background color of the chart title.")]
        public string TitleBackFill
        {
            get
            {
                return this.titlebackcolor;
            }
            set
            {
                if (value != this.titlebackcolor)
                {
                    this.OnPropertyChanging("TitleBackFill");
                    this.titlebackcolor = value;
                    this.OnPropertyChanged("TitleBackFill");
                }
            }
        }

        [Browsable(false)]
        public string Visibility
        {
            get
            {
                return this.visibility;
            }
            set
            {
                if (value != this.visibility)
                {
                    this.OnPropertyChanging("Visibility");
                    this.visibility = value;
                    this.OnPropertyChanged("Visibility");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    internal class ValueAxisProperties : IDesignerProperties
    {
        string reverseDirection = null;
        string lineStyle = null;
        string lineWidth = null;
        string lineColor = null;
        string hideAxislabels = null;
        string fontFamily = null;
        string fontSize = null;
        string fontWeight = null;
        string fontColor = null;
        string fontAngle = null;
        string enableMajorTickMarks = null;
        string enableMinorTickMarks = null;
        string tickStyle = null;
        string tickWidth = null;
        string tickColor = null;
        string tickLength = null;
        string minortickStyle = null;
        string minortickColor = null;
        //string smallTicks = null;
        string visibility = null;

        public ValueAxisProperties()
        {
            this.ReverseDirection = "False";
            this.LineStyle = "Solid";
            this.LineWidth = "1pt";
            this.HideAxisLabels = "False";
            this.FontFamily = "Segoe UI";
            this.FontSize = "10pt";
            this.FontWeight = "Default";
            this.LineStyle = "Solid";
            this.EnableMajorTickMarks = "True";
            this.EnableMinorTickMarks = "False";
            this.TickStyle = "Solid";
            this.Name = "Chart Axis";
            this.TickColor = "Black";
            this.FontColor = "Black";
            this.LineColor = "Black";
            this.TickLength = "1pt";
            this.FontAngle = "0";
            this.TickWidth = "1pt";
            this.MinorTickColor = "Black";
            this.MinorTickStyle = "Solid";
            this.Visibility = "True";
        }

        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Reverse Direction")]
        [DescriptionAttribute("The values on the axis are in reverse order.")]
        public string ReverseDirection
        {
            get
            {
                return this.reverseDirection;
            }
            set
            {
                if (value != this.reverseDirection)
                {
                    this.OnPropertyChanging("ReverseDirection");
                    this.reverseDirection = value;
                    this.OnPropertyChanged("ReverseDirection");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Line Style")]
        [DescriptionAttribute("Specifies the style of the report item.")]
        public string LineStyle
        {
            get
            {
                return this.lineStyle;
            }
            set
            {
                if (value != this.lineStyle)
                {
                    this.OnPropertyChanging("LineStyle");
                    this.lineStyle = value;
                    this.OnPropertyChanged("LineStyle");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Line Width")]
        [DescriptionAttribute("Specifies the width of the report item.")]
        public string LineWidth
        {
            get
            {
                return this.lineWidth;
            }
            set
            {
                if (value != this.lineWidth)
                {
                    this.OnPropertyChanging("LineWidth");
                    this.lineWidth = value;
                    this.OnPropertyChanged("LineWidth");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Line Color")]
        [DescriptionAttribute("Specifies the color of the report item.")]
        public string LineColor
        {
            get
            {
                return this.lineColor;
            }
            set
            {
                if (value != this.lineColor)
                {
                    this.OnPropertyChanging("LineColor");
                    this.lineColor = value;
                    this.OnPropertyChanged("LineColor");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Hide Axis Labels")]
        [DescriptionAttribute("Indicates whether the axis labes are hidden.")]
        public string HideAxisLabels
        {
            get
            {
                return this.hideAxislabels;
            }
            set
            {
                if (value != this.hideAxislabels)
                {
                    this.OnPropertyChanging("HideAxisLabels");
                    this.hideAxislabels = value;
                    this.OnPropertyChanged("HideAxisLabels");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Family")]
        [DescriptionAttribute("Specifies the name of the font family.")]
        public string FontFamily
        {
            get
            {
                return this.fontFamily;
            }
            set
            {
                if (value != this.fontFamily)
                {
                    this.OnPropertyChanging("FontFamily");
                    this.fontFamily = value;
                    this.OnPropertyChanged("FontFamily");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Size")]
        [DescriptionAttribute("Specifies the point size of the label.")]
        public string FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                if (value != this.fontSize)
                {
                    this.OnPropertyChanging("FontSize");
                    this.fontSize = value;
                    this.OnPropertyChanged("FontSize");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Weight")]
        [DescriptionAttribute("Specifies the font weight of the label.")]
        public string FontWeight
        {
            get
            {
                return this.fontWeight;
            }
            set
            {
                if (value != this.fontWeight)
                {
                    this.OnPropertyChanging("FontWeight");
                    this.fontWeight = value;
                    this.OnPropertyChanged("FontWeight");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Color")]
        [DescriptionAttribute("Specifies the color of the label.")]
        public string FontColor
        {
            get
            {
                return this.fontColor;
            }
            set
            {
                if (value != this.fontColor)
                {
                    this.OnPropertyChanging("FontColor");
                    this.fontColor = value;
                    this.OnPropertyChanged("FontColor");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Angle")]
        [DescriptionAttribute("Specifies the angle of the label.")]
        public string FontAngle
        {
            get
            {
                return this.fontAngle;
            }
            set
            {
                if (value != this.fontAngle)
                {
                    this.OnPropertyChanging("FontAngle");
                    this.fontAngle = value;
                    this.OnPropertyChanged("FontAngle");
                }
            }
        }

        [CategoryAttribute("MajorTick")]
        [DisplayNameAttribute("Enable Major Tick Marks")]
        [DescriptionAttribute("Indicates whether the major tick marks are Enabled.")]
        public string EnableMajorTickMarks
        {
            get
            {
                return this.enableMajorTickMarks;
            }
            set
            {
                if (value != this.enableMajorTickMarks)
                {
                    this.OnPropertyChanging("EnableMajorTickMarks");
                    this.enableMajorTickMarks = value;
                    this.OnPropertyChanged("EnableMajorTickMarks");
                }
            }
        }

        [CategoryAttribute("MinorTick")]
        [DisplayNameAttribute("Enable Minor Tick Marks")]
        [DescriptionAttribute("Indicates whether the minor tick marks are Enabled.")]
        public string EnableMinorTickMarks
        {
            get
            {
                return this.enableMinorTickMarks;
            }
            set
            {
                if (value != this.enableMinorTickMarks)
                {
                    this.OnPropertyChanging("EnableMinorTickMarks");
                    this.enableMinorTickMarks = value;
                    this.OnPropertyChanged("EnableMinorTickMarks");
                }
            }
        }

        [CategoryAttribute("MajorTick")]
        [DisplayNameAttribute("Tick Style")]
        [DescriptionAttribute("Specifies the style of the major tick mark.")]
        public string TickStyle
        {
            get
            {
                return this.tickStyle;
            }
            set
            {
                if (value != this.tickStyle)
                {
                    this.OnPropertyChanging("TickStyle");
                    this.tickStyle = value;
                    this.OnPropertyChanged("TickStyle");
                }
            }
        }

        [CategoryAttribute("MinorTick")]
        [DisplayNameAttribute("Tick Width")]
        [DescriptionAttribute("Specifies the width of the major tick mark.")]
        public string TickWidth
        {
            get
            {
                return this.tickWidth;
            }
            set
            {
                if (value != this.tickWidth)
                {
                    this.OnPropertyChanging("TickWidth");
                    this.tickWidth = value;
                    this.OnPropertyChanged("TickWidth");
                }
            }
        }

        [CategoryAttribute("MajorTick")]
        [DisplayNameAttribute("Tick Color")]
        [DescriptionAttribute("Specifies the color of the major tick mark.")]
        public string TickColor
        {
            get
            {
                return this.tickColor;
            }
            set
            {
                if (value != this.tickColor)
                {
                    this.OnPropertyChanging("TickColor");
                    this.tickColor = value;
                    this.OnPropertyChanged("TickColor");
                }
            }
        }

        [CategoryAttribute("MinorTick")]
        [DisplayNameAttribute("Minor Tick Style")]
        [DescriptionAttribute("Specifies the style of the minor tick mark.")]
        public string MinorTickStyle
        {
            get
            {
                return this.minortickStyle;
            }
            set
            {
                if (value != this.minortickStyle)
                {
                    this.OnPropertyChanging("MinorTickStyle");
                    this.minortickStyle = value;
                    this.OnPropertyChanged("MinorTickStyle");
                }
            }
        }

        [CategoryAttribute("MinorTick")]
        [DisplayNameAttribute("Minor Tick Color")]
        [DescriptionAttribute("Specifies the color of the minor tick mark.")]
        public string MinorTickColor
        {
            get
            {
                return this.minortickColor;
            }
            set
            {
                if (value != this.minortickColor)
                {
                    this.OnPropertyChanging("MinorTickColor");
                    this.minortickColor = value;
                    this.OnPropertyChanged("MinorTickColor");
                }
            }
        }

        [CategoryAttribute("MajorTick")]
        [DisplayNameAttribute("Tick Length")]
        [DescriptionAttribute("Specifies the length of the minor tick mark.")]
        public string TickLength
        {
            get
            {
                return this.tickLength;
            }
            set
            {
                if (value != this.tickLength)
                {
                    this.OnPropertyChanging("TickLength");
                    this.tickLength = value;
                    this.OnPropertyChanged("TickLength");
                }
            }
        }
       
        [Browsable(false)]
        public string Visibility
        {
            get
            {
                return this.visibility;
            }
            set
            {
                if (value != this.visibility)
                {
                    this.OnPropertyChanging("Visibility");
                    this.visibility = value;
                    this.OnPropertyChanged("Visibility");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    internal class CategoryAxisProperties : IDesignerProperties
    {
        string reverseDirection = null;
        string lineStyle = null;
        string lineWidth = null;
        string lineColor = null;
        string hideAxislabels = null;
        string fontFamily = null;
        string fontSize = null;
        string fontWeight = null;
        string fontColor = null;
        string fontAngle = null;
        string enableMajorTickMarks = null;
        string enableMinorTickMarks = null;
        string tickStyle = null;
        string tickWidth = null;
        string tickColor = null;
        string tickLength = null;
        string minortickStyle = null;
        string minortickColor = null;
        //string smallTicks = null;
        string visibility = null;

        public CategoryAxisProperties()
        {
            this.ReverseDirection = "False";
            this.LineStyle = "Solid";
            this.LineWidth = "1pt";
            this.HideAxisLabels = "False";
            this.FontFamily = "Segoe UI";
            this.FontSize = "10pt";
            this.FontWeight = "Default";
            this.LineStyle = "Solid";
            this.EnableMajorTickMarks = "True";
            this.EnableMinorTickMarks = "False";
            this.TickStyle = "Solid";
            this.Name = "Chart Axis";
            this.TickColor = "Black";
            this.FontColor = "Black";
            this.LineColor = "Black";
            this.TickLength = "1pt";
            this.FontAngle = "0";
            this.TickWidth = "1pt";
            this.MinorTickColor = "Black";
            this.MinorTickStyle = "Solid";
            this.Visibility = "True";
        }

        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Reverse Direction")]
        [DescriptionAttribute("The values on the axis are in reverse order.")]
        public string ReverseDirection
        {
            get
            {
                return this.reverseDirection;
            }
            set
            {
                if (value != this.reverseDirection)
                {
                    this.OnPropertyChanging("ReverseDirection");
                    this.reverseDirection = value;
                    this.OnPropertyChanged("ReverseDirection");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Line Style")]
        [DescriptionAttribute("Specifies the style of the report item.")]
        public string LineStyle
        {
            get
            {
                return this.lineStyle;
            }
            set
            {
                if (value != this.lineStyle)
                {
                    this.OnPropertyChanging("LineStyle");
                    this.lineStyle = value;
                    this.OnPropertyChanged("LineStyle");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Line Width")]
        [DescriptionAttribute("Specifies the width of the report item.")]
        public string LineWidth
        {
            get
            {
                return this.lineWidth;
            }
            set
            {
                if (value != this.lineWidth)
                {
                    this.OnPropertyChanging("LineWidth");
                    this.lineWidth = value;
                    this.OnPropertyChanged("LineWidth");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Line Color")]
        [DescriptionAttribute("Specifies the color of the report item.")]
        public string LineColor
        {
            get
            {
                return this.lineColor;
            }
            set
            {
                if (value != this.lineColor)
                {
                    this.OnPropertyChanging("LineColor");
                    this.lineColor = value;
                    this.OnPropertyChanged("LineColor");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Hide Axis Labels")]
        [DescriptionAttribute("Indicates whether the axis labes are hidden.")]
        public string HideAxisLabels
        {
            get
            {
                return this.hideAxislabels;
            }
            set
            {
                if (value != this.hideAxislabels)
                {
                    this.OnPropertyChanging("HideAxisLabels");
                    this.hideAxislabels = value;
                    this.OnPropertyChanged("HideAxisLabels");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Family")]
        [DescriptionAttribute("Specifies the name of the font family.")]
        public string FontFamily
        {
            get
            {
                return this.fontFamily;
            }
            set
            {
                if (value != this.fontFamily)
                {
                    this.OnPropertyChanging("FontFamily");
                    this.fontFamily = value;
                    this.OnPropertyChanged("FontFamily");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Size")]
        [DescriptionAttribute("Specifies the point size of the label.")]
        public string FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                if (value != this.fontSize)
                {
                    this.OnPropertyChanging("FontSize");
                    this.fontSize = value;
                    this.OnPropertyChanged("FontSize");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Weight")]
        [DescriptionAttribute("Specifies the font weight of the label.")]
        public string FontWeight
        {
            get
            {
                return this.fontWeight;
            }
            set
            {
                if (value != this.fontWeight)
                {
                    this.OnPropertyChanging("FontWeight");
                    this.fontWeight = value;
                    this.OnPropertyChanged("FontWeight");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Color")]
        [DescriptionAttribute("Specifies the color of the label.")]
        public string FontColor
        {
            get
            {
                return this.fontColor;
            }
            set
            {
                if (value != this.fontColor)
                {
                    this.OnPropertyChanging("FontColor");
                    this.fontColor = value;
                    this.OnPropertyChanged("FontColor");
                }
            }
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Angle")]
        [DescriptionAttribute("Specifies the angle of the label.")]
        public string FontAngle
        {
            get
            {
                return this.fontAngle;
            }
            set
            {
                if (value != this.fontAngle)
                {
                    this.OnPropertyChanging("FontAngle");
                    this.fontAngle = value;
                    this.OnPropertyChanged("FontAngle");
                }
            }
        }

        [CategoryAttribute("MajorTick")]
        [DisplayNameAttribute("Enable Major Tick Marks")]
        [DescriptionAttribute("Indicates whether the major tick marks are Enabled.")]
        public string EnableMajorTickMarks
        {
            get
            {
                return this.enableMajorTickMarks;
            }
            set
            {
                if (value != this.enableMajorTickMarks)
                {
                    this.OnPropertyChanging("EnableMajorTickMarks");
                    this.enableMajorTickMarks = value;
                    this.OnPropertyChanged("EnableMajorTickMarks");
                }
            }
        }

        [CategoryAttribute("MinorTick")]
        [DisplayNameAttribute("Enable Minor Tick Marks")]
        [DescriptionAttribute("Indicates whether the minor tick marks are Enabled.")]
        public string EnableMinorTickMarks
        {
            get
            {
                return this.enableMinorTickMarks;
            }
            set
            {
                if (value != this.enableMinorTickMarks)
                {
                    this.OnPropertyChanging("EnableMinorTickMarks");
                    this.enableMinorTickMarks = value;
                    this.OnPropertyChanged("EnableMinorTickMarks");
                }
            }
        }

        [CategoryAttribute("MajorTick")]
        [DisplayNameAttribute("Tick Style")]
        [DescriptionAttribute("Specifies the style of the major tick mark.")]
        public string TickStyle
        {
            get
            {
                return this.tickStyle;
            }
            set
            {
                if (value != this.tickStyle)
                {
                    this.OnPropertyChanging("TickStyle");
                    this.tickStyle = value;
                    this.OnPropertyChanged("TickStyle");
                }
            }
        }

        [CategoryAttribute("MinorTick")]
        [DisplayNameAttribute("Tick Width")]
        [DescriptionAttribute("Specifies the width of the major tick mark.")]
        public string TickWidth
        {
            get
            {
                return this.tickWidth;
            }
            set
            {
                if (value != this.tickWidth)
                {
                    this.OnPropertyChanging("TickWidth");
                    this.tickWidth = value;
                    this.OnPropertyChanged("TickWidth");
                }
            }
        }

        [CategoryAttribute("MajorTick")]
        [DisplayNameAttribute("Tick Color")]
        [DescriptionAttribute("Specifies the color of the major tick mark.")]
        public string TickColor
        {
            get
            {
                return this.tickColor;
            }
            set
            {
                if (value != this.tickColor)
                {
                    this.OnPropertyChanging("TickColor");
                    this.tickColor = value;
                    this.OnPropertyChanged("TickColor");
                }
            }
        }

        [CategoryAttribute("MinorTick")]
        [DisplayNameAttribute("Minor Tick Style")]
        [DescriptionAttribute("Specifies the style of the minor tick mark.")]
        public string MinorTickStyle
        {
            get
            {
                return this.minortickStyle;
            }
            set
            {
                if (value != this.minortickStyle)
                {
                    this.OnPropertyChanging("MinorTickStyle");
                    this.minortickStyle = value;
                    this.OnPropertyChanged("MinorTickStyle");
                }
            }
        }

        [CategoryAttribute("MinorTick")]
        [DisplayNameAttribute("Minor Tick Color")]
        [DescriptionAttribute("Specifies the color of the minor tick mark.")]
        public string MinorTickColor
        {
            get
            {
                return this.minortickColor;
            }
            set
            {
                if (value != this.minortickColor)
                {
                    this.OnPropertyChanging("MinorTickColor");
                    this.minortickColor = value;
                    this.OnPropertyChanged("MinorTickColor");
                }
            }
        }

        [CategoryAttribute("MajorTick")]
        [DisplayNameAttribute("Tick Length")]
        [DescriptionAttribute("Specifies the length of the minor tick mark.")]
        public string TickLength
        {
            get
            {
                return this.tickLength;
            }
            set
            {
                if (value != this.tickLength)
                {
                    this.OnPropertyChanging("TickLength");
                    this.tickLength = value;
                    this.OnPropertyChanged("TickLength");
                }
            }
        }       

        [Browsable(false)]
        public string Visibility
        {
            get
            {
                return this.visibility;
            }
            set
            {
                if (value != this.visibility)
                {
                    this.OnPropertyChanging("Visibility");
                    this.visibility = value;
                    this.OnPropertyChanged("Visibility");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    internal class CategoryAxisTitleProperties : IDesignerProperties
    {
        string title = null;
        string fontFamily = null;
        string fontSize = null;
        string fontStyle = null;
        string fontColor = null;
        string alignment = null;

        public CategoryAxisTitleProperties()
        {
            this.FontFamily = "Arial";
            this.FontSize = "8pt";
            this.TitleAlignment = "Center";
            this.FontStyle = "Default";
            this.Name = "Axis Title";
            this.FontColor = "Black";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Caption")]
        [DescriptionAttribute("Specifies the title of the chart.")]
        public string Name
        {
            get
            {
                return this.title;
            }
            set
            {
                if (value != this.title)
                {
                    this.OnPropertyChanging("Name");
                    this.title = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Family")]
        [DescriptionAttribute("Specifies the name of the font family.")]
        public string FontFamily
        {
            get
            {
                return this.fontFamily;
            }
            set
            {
                if (value != this.fontFamily)
                {
                    this.OnPropertyChanging("FontFamily");
                    this.fontFamily = value;
                    this.OnPropertyChanged("FontFamily");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Size")]
        [DescriptionAttribute("Specifies the point size of the label.")]
        public string FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                if (value != this.fontSize)
                {
                    this.OnPropertyChanging("FontSize");
                    this.fontSize = value;
                    this.OnPropertyChanged("FontSize");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Style")]
        [DescriptionAttribute("Specifies the style of the label.")]
        public string FontStyle
        {
            get
            {
                return this.fontStyle;
            }
            set
            {
                if (value != this.fontStyle)
                {
                    this.OnPropertyChanging("FontStyle");
                    this.fontStyle = value;
                    this.OnPropertyChanged("FontStyle");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Color")]
        [DescriptionAttribute("Specifies the color of the label.")]
        public string FontColor
        {
            get
            {
                return this.fontColor;
            }
            set
            {
                if (value != this.fontColor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("FontColor");
                    this.fontColor = value;
                    this.OnPropertyChanged("FontColor");
                }
            }
        }

        [CategoryAttribute("Alignment")]
        [DisplayNameAttribute("TextAlign")]
        [DescriptionAttribute("Specifies the alignment of the title.")]
        public string TitleAlignment
        {
            get
            {
                return this.alignment;
            }
            set
            {
                if (value != this.alignment)
                {
                    this.OnPropertyChanging("TitleAlignment");
                    this.alignment = value;
                    this.OnPropertyChanged("TitleAlignment");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    internal class ValueAxisTitleProperties : IDesignerProperties
    {
        string title = null;
        string fontFamily = null;
        string fontSize = null;
        string fontStyle = null;
        string fontColor = null;
        string alignment = null;
        
        public ValueAxisTitleProperties()
        {
            this.FontFamily = "Arial";
            this.FontSize = "8pt";
            this.TitleAlignment = "Center";
            this.FontStyle = "Default";
            this.Name = "Axis Title";
            this.FontColor = "Black";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Caption")]
        [DescriptionAttribute("Specifies the title of the chart.")]
        public string Name
        {
            get
            {
                return this.title;
            }
            set
            {
                if (value != this.title)
                {
                    this.OnPropertyChanging("Name");
                    this.title = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Family")]
        [DescriptionAttribute("Specifies the name of the font family.")]
        public string FontFamily
        {
            get
            {
                return this.fontFamily;
            }
            set
            {
                if (value != this.fontFamily)
                {
                    this.OnPropertyChanging("FontFamily");
                    this.fontFamily = value;
                    this.OnPropertyChanged("FontFamily");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Size")]
        [DescriptionAttribute("Specifies the point size of the label.")]
        public string FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                if (value != this.fontSize)
                {
                    this.OnPropertyChanging("FontSize");
                    this.fontSize = value;
                    this.OnPropertyChanged("FontSize");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Style")]
        [DescriptionAttribute("Specifies the style of the label.")]
        public string FontStyle
        {
            get
            {
                return this.fontStyle;
            }
            set
            {
                if (value != this.fontStyle)
                {
                    this.OnPropertyChanging("FontStyle");
                    this.fontStyle = value;
                    this.OnPropertyChanged("FontStyle");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Color")]
        [DescriptionAttribute("Specifies the color of the label.")]
        public string FontColor
        {
            get
            {
                return this.fontColor;
            }
            set
            {
                if (value != this.fontColor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("FontColor");
                    this.fontColor = value;
                    this.OnPropertyChanged("FontColor");
                }
            }
        }

        [CategoryAttribute("Alignment")]
        [DisplayNameAttribute("TextAlign")]
        [DescriptionAttribute("Specifies the alignment of the title.")]
        public string TitleAlignment
        {
            get
            {
                return this.alignment;
            }
            set
            {
                if (value != this.alignment)
                {
                    this.OnPropertyChanging("TitleAlignment");
                    this.alignment = value;
                    this.OnPropertyChanged("TitleAlignment");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    internal class DataLabelspropertiesCollection : List<DataLabelsproperties>
    {
    }

    internal class DataLabelsproperties : IDesignerProperties
    {
        string showDataLabels = null;
        string position = null;

        public DataLabelsproperties()
        {
            this.Name = "Chart Series Labels";
            this.ShowDataLabels = "False";
            this.DataLabelsPosition = "TopCenter";
        }

        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Show Data Labels")]
        [DescriptionAttribute("Indicates whether the datalabels are hidden are not.")]
        public string ShowDataLabels
        {
            get
            {
                return this.showDataLabels;
            }
            set
            {
                if (value != this.showDataLabels)
                {
                    this.OnPropertyChanging("ShowDataLabels");
                    this.showDataLabels = value;
                    this.OnPropertyChanged("ShowDataLabels");
                }
            }
        }

        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Position")]
        [DescriptionAttribute("The Position of the data labels of chart series.")]
        public string DataLabelsPosition
        {
            get
            {
                return this.position;
            }
            set
            {
                if (value != this.position)
                {
                    this.OnPropertyChanging("Position");
                    this.position = value;
                    this.OnPropertyChanged("Position");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    internal class LegendTitleProperties : IDesignerProperties
    {
        string caption = "LegendTitle";
        string fontfamily = "Arial";
        string fontsize = "8pt";
        string fontstyle = "Bold";
        string fontcolor = "Black";
        string backcolor = "White";

        public LegendTitleProperties()
        {
            this.FontFamily = "Arial";
            this.FontSize = "8pt";
            this.FontStyle = "Default";
            this.Name = "LegendTitle";
            this.FontColor = "Black";
            this.BackFill = "White";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Title")]
        [DescriptionAttribute("Specifies the title of the Legend.")]
        public string Caption
        {
            get
            {
                return this.caption;
            }
            set
            {
                if (value != this.caption)
                {
                    this.OnPropertyChanging("Caption");
                    this.caption = value;
                    this.OnPropertyChanged("Caption");
                }
            }
        }

        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Family")]
        [DescriptionAttribute("Specifies the name of the font family.")]
        public string FontFamily
        {
            get
            {
                return this.fontfamily;
            }
            set
            {
                if (value != this.fontfamily)
                {
                    this.OnPropertyChanging("FontFamily");
                    this.fontfamily = value;
                    this.OnPropertyChanged("FontFamily");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Size")]
        [DescriptionAttribute("Specifies the point size of the label.")]
        public string FontSize
        {
            get
            {
                return this.fontsize;
            }
            set
            {
                if (value != this.fontsize)
                {
                    this.OnPropertyChanging("FontSize");
                    this.fontsize = value;
                    this.OnPropertyChanged("FontSize");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Style")]
        [DescriptionAttribute("Specifies the style of the label.")]
        public string FontStyle
        {
            get
            {
                return this.fontstyle;
            }
            set
            {
                if (value != this.fontstyle)
                {
                    this.OnPropertyChanging("FontStyle");
                    this.fontstyle = value;
                    this.OnPropertyChanged("FontStyle");
                }
            }
        }

        [CategoryAttribute("Font")]
        [DisplayNameAttribute("Font Color")]
        [DescriptionAttribute("Specifies the color of the label.")]
        public string FontColor
        {
            get
            {
                return this.fontcolor;
            }
            set
            {
                if (value != this.fontcolor)
                {
                    this.OnPropertyChanging("FontColor");
                    this.fontcolor = value;
                    this.OnPropertyChanged("FontColor");
                }
            }
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Back Fill")]
        [DescriptionAttribute("Specifies the background color of the legend title.")]
        public string BackFill
        {
            get
            {
                return this.backcolor;
            }
            set
            {
                if (value != this.backcolor)
                {
                    this.OnPropertyChanging("BackFill");
                    this.backcolor = value;
                    this.OnPropertyChanged("BackFill");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
    }

    #endregion

    #region Tablix properties

    internal class TablixProperties : IReportItemProperties
    {
        string name = null;
        string hidden = null;
        string format = null;
        string tooltip = null;
        string toggleItem = null;
        string datasetName = null;
        string omitOnBreak = null;
        string keeptogether = null;
        string fixrowHeader = null;
        string fixcolHeader = null;
        string repeatcolHeader = null;
        string repeatrowHeader = null;
        string dataElementName = null;
        string dataElementOutput = null;
        string documentMapLabel = null;
        string horizontalalignment = null;
        string verticalalignment = null;
        RDL.DOM.BreakLocation pageBreak = RDL.DOM.BreakLocation.None;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        public TablixProperties()
        {
            this.Size = new Sizes();
            this.Padding = new Padding();
            this.Location = new Locations();
            this.BorderStyles = new BorderStyles();
            this.BorderWidths = new BorderWidths();
            this.BorderColors = new BorderColors();

            this.Size.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Size.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.Padding.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Padding.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderStyles.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderStyles.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderWidths.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderWidths.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderColors.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderColors.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.Location.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Location.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);

            this.KeepTogether = "False";
            this.Hidden = "False";
            this.HorizontalAlignment = "Default";
            this.VerticalAlignment = "Default";
            this.DataElementOutput = "Auto";
            this.OmitBorderOnPageBreak = "False";
            this.BorderStyles.DefaultBorderStyle = "None";
            this.BorderWidths.DefaultBorderWidth = "1pt";
            this.BorderColors.DefaultBorderColor = "Black";
            this.Padding.PaddingLeft = this.Padding.PaddingRight = this.Padding.PaddingTop = this.Padding.PaddingBottom = "0pt";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specfies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public BorderStyles BorderStyles
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the size of the border.")]
        public BorderWidths BorderWidths
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("BorderColor")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public BorderColors BorderColors
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("General")]
        [DisplayNameAttribute("ToolTip")]
        [DescriptionAttribute("Specifies the textual label for the report item. For example, it can be used to render TITLE and ALT attributes in HTML reports.")]
        public string ToolTip
        {
            get
            {
                return this.tooltip;
            }
            set
            {
                if (value != this.tooltip)
                {
                    this.OnPropertyChanging("ToolTip");
                    this.tooltip = value;
                    this.OnPropertyChanged("ToolTip");
                }
            }
        }


        [CategoryAttribute("General")]
        [DisplayNameAttribute("KeepTogether")]
        [DescriptionAttribute("Indicates whether to keep all sections of the data region together on one page .")]
        public string KeepTogether
        {
            get
            {
                return this.keeptogether;
            }
            set
            {
                if (value != this.keeptogether)
                {
                    this.OnPropertyChanging("KeepTogether");
                    this.keeptogether = value;
                    this.OnPropertyChanged("KeepTogether");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("PageBreak")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public RDL.DOM.BreakLocation PageBreak
        {
            get
            {
                return this.pageBreak;
            }
            set
            {
                if (value != this.pageBreak)
                {
                    this.OnPropertyChanging("PageBreak");
                    this.pageBreak = value;
                    this.OnPropertyChanged("PageBreak");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("RepeatRowHeader")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public string RepeatRowHeaders
        {
            get
            {
                return this.repeatrowHeader;
            }
            set
            {
                if (value != this.repeatrowHeader)
                {
                    this.OnPropertyChanging("RepeatRowHeader");
                    this.repeatrowHeader = value;
                    this.OnPropertyChanged("RepeatRowHeader");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("RepeatColumnHeaders")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public string RepeatColumnHeaders
        {
            get
            {
                return this.repeatcolHeader;
            }
            set
            {
                if (value != this.repeatcolHeader)
                {
                    this.OnPropertyChanging("RepeatColumnHeaders");
                    this.repeatcolHeader = value;
                    this.OnPropertyChanged("RepeatColumnHeaders");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("FixedRowHeaders")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public string FixedRowHeaders
        {
            get
            {
                return this.fixrowHeader;
            }
            set
            {
                if (value != this.fixrowHeader)
                {
                    this.OnPropertyChanging("FixedRowHeaders");
                    this.fixrowHeader = value;
                    this.OnPropertyChanged("FixedRowHeaders");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("FixedColumnHeaders")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public string FixedColumnHeaders
        {
            get
            {
                return this.fixcolHeader;
            }
            set
            {
                if (value != this.fixcolHeader)
                {
                    this.OnPropertyChanging("FixedColumnHeaders");
                    this.fixcolHeader = value;
                    this.OnPropertyChanged("FixedColumnHeaders");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementName")]
        [DescriptionAttribute("Specify the name of the data element or attribute of report item.")]
        public string DataElementName
        {
            get
            {
                return this.dataElementName;
            }
            set
            {
                if (value != this.dataElementName)
                {
                    OnPropertyChanging("DataElementName");
                    this.dataElementName = value;
                    OnPropertyChanged("DataElementName");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementOutput")]
        [DescriptionAttribute("Indicates whether and how data element should appear when rendered.")]
        public string DataElementOutput
        {
            get
            {
                return this.dataElementOutput;
            }
            set
            {
                if (value != this.dataElementOutput)
                {
                    OnPropertyChanging("DataElementOutput");
                    this.dataElementOutput = value;
                    OnPropertyChanged("DataElementOutput");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DocumentMapLabel")]
        [DescriptionAttribute("Identifeis an instance of a report item within the document map.")]
        public string DocumentMapLabel
        {
            get
            {
                return this.documentMapLabel;
            }
            set
            {
                if (value != this.documentMapLabel)
                {
                    this.OnPropertyChanging("DocumentMapLabel");
                    this.documentMapLabel = value;
                    this.OnPropertyChanged("DocumentMapLabel");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("Other")]
        [DisplayNameAttribute("OmitBorderOnPageBreak")]
        [DescriptionAttribute("Indicates whether border should appear around report items that span multiple pages.")]
        public string OmitBorderOnPageBreak
        {
            get
            {
                return this.omitOnBreak;
            }
            set
            {
                this.OnPropertyChanging("OmitBorderOnPageBreak");
                this.omitOnBreak = value;
                this.OnPropertyChanged("OmitBorderOnPageBreak");
            }
        }

        [CategoryAttribute("Data")]
        [DisplayNameAttribute("DatasetName")]
        [DescriptionAttribute("Specifies the name of the Dataset.")]
        public string Dataset
        {
            get
            {
                return this.datasetName;
            }
            set
            {
                if (value != this.datasetName)
                {
                    this.OnPropertyChanging("Dataset");
                    this.datasetName = value;
                    this.OnPropertyChanged("Dataset");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Hidden")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the report item is initially hidden.")]
        public string Hidden
        {
            get
            {
                return this.hidden;
            }
            set
            {
                if (value != this.hidden)
                {
                    this.OnPropertyChanging("Hidden");
                    this.hidden = value;
                    this.OnPropertyChanged("Hidden");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("ToggleItem")]
        [DescriptionAttribute("Specifies name of the report item to click to show or hide another report item.")]
        public string ToggleItem
        {
            get
            {
                return this.toggleItem;
            }
            set
            {
                if (value != this.toggleItem)
                {
                    this.OnPropertyChanging("ToggleItem");
                    this.toggleItem = value;
                    this.OnPropertyChanged("ToggleItem");
                }
            }
        }

        [CategoryAttribute("No Rows")]
        [DisplayNameAttribute("Text Alignment")]
        [DescriptionAttribute("Specifies the horizontal alignment of text within the report item.")]
        public string HorizontalAlignment
        {
            get
            {
                return this.horizontalalignment;
            }
            set
            {
                if (value != this.horizontalalignment)
                {
                    this.OnPropertyChanging("HorizontalAlignment");
                    this.horizontalalignment = value;
                    this.OnPropertyChanged("HorizontalAlignment");
                }
            }
        }

        [CategoryAttribute("No Rows")]
        [DisplayNameAttribute("Vertical Alignment")]
        [DescriptionAttribute("Specifies the vertical alignment of text within the report item.")]
        public string VerticalAlignment
        {
            get
            {
                return this.verticalalignment;
            }
            set
            {
                if (value != this.verticalalignment)
                {
                    this.OnPropertyChanging("VerticalAlignment");
                    this.verticalalignment = value;
                    this.OnPropertyChanged("VerticalAlignment");
                }
            }
        }

        [CategoryAttribute("No Rows")]
        [DisplayNameAttribute("Format")]
        [DescriptionAttribute("Specifies the expression that formats the string in the report item.")]
        public String Format
        {
            get
            {
                return this.format;
            }
            set
            {
                if (value != this.format)
                {
                    this.OnPropertyChanging("Format");
                    this.format = value;
                    this.OnPropertyChanged("Format");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("No Rows")]
        [DisplayNameAttribute("Padding")]
        [DescriptionAttribute("Specifies the amount of padding between the report item boundary and its contents.")]
        public Padding Padding
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Location")]
        [DescriptionAttribute("Specifies the position of the top-left corner of a report item in relation to its container.")]
        public Locations Location
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Size")]
        [DescriptionAttribute("Specifies the Size of the report item")]
        public Sizes Size
        {
            get;
            set;
        }


        string width = null;
        string height = null;

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specifies the width of the report item")]
        public string Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value != this.width)
                {
                    this.OnPropertyChanging("Width");
                    this.width = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specifies the height of the report item")]
        public string Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value != this.height)
                {
                    this.OnPropertyChanging("Height");
                    this.height = value;
                    this.OnPropertyChanged("Height");
                }
            }
        }

        string left = null;
        string top = null;

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the distance of the item from the left of the containing object.")]
        public string Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("Left");
                    this.left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the distance of the item from the top of the containing object.")]
        public string Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("Top");
                    this.top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    internal class TablixMemberProperties : IDesignerProperties
    {
        bool fixedData;
        bool hideIfNoRows;
        bool repeatonNewPage;
        string keepwithGroup = null;

        string name = null;
        string hidden = null;
        string parent = null;
        string toggleItem = null;
        string domainScope = null;
        string keeptogether = null;
        string dataElementName = null;
        string dataElementOutput = null;
        string documentMapLabel = null;
        RDL.DOM.BreakLocation pageBreak =  RDL.DOM.BreakLocation.None;
        string groupdataElementName = null;
        string groupdataElementOutput = null;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        public TablixMemberProperties()
        {
            this.KeepTogether = "False";
            this.KeepWithGroup = "None";
            this.PageBreak = RDL.DOM.BreakLocation.None;
            this.Hidden = "False";
            this.DataElementOutput = "Auto";
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool HasGroup
        {
            get;
            set;
        }

        [Browsable(false)]
        public object TablixMember
        {
            get;
            set;
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("FixedData")]
        [DescriptionAttribute("Indicates whether the entire member,including its body cells display on the page when the user scorlls part of the Tablix off page.")]
        public bool FixedData
        {
            get
            {
                return this.fixedData;
            }
            set
            {
                if (value != this.fixedData)
                {
                    this.OnPropertyChanging("FixedData");
                    this.fixedData = value;
                    this.OnPropertyChanged("FixedData");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("HideIfNoRows")]
        [DescriptionAttribute("Indicates whether a static member is hidden when the data region contains no row of data.The property oveerides other visibility properties that apply to the data regions.")]
        public bool HideIfNoRows
        {
            get
            {
                return this.hideIfNoRows;
            }
            set
            {
                if (value != this.hideIfNoRows)
                {
                    this.OnPropertyChanging("HideIfNoRows");
                    this.hideIfNoRows = value;
                    this.OnPropertyChanged("HideIfNoRows");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("KeepTogether")]
        [DescriptionAttribute("Indicates whether to keep all sections of the data region together on one page .")]
        public string KeepTogether
        {
            get
            {
                return this.keeptogether;
            }
            set
            {
                if (value != this.keeptogether)
                {
                    this.OnPropertyChanging("KeepTogether");
                    this.keeptogether = value;
                    this.OnPropertyChanged("KeepTogether");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("KeepWithGroup")]
        [DescriptionAttribute("Indicates whether a static member appears on the same page with the closest non-hidden instance of the previous or following sibling dynamic member whenever possible.")]
        public string KeepWithGroup
        {
            get
            {
                return this.keepwithGroup;
            }
            set
            {
                if (value != this.keepwithGroup)
                {
                    this.OnPropertyChanging("KeepWithGroup");
                    this.keepwithGroup = value;
                    this.OnPropertyChanged("KeepWithGroup");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("RepeatOnNewPage")]
        [DescriptionAttribute("Indicates whether a static member is repeatedd on every page on which at least one complete instance of the dynamic member referred to in the KeepWithGroup property or the decendends of the member appears.")]
        public bool RepeatOnNewPage
        {
            get
            {
                return this.repeatonNewPage;
            }
            set
            {
                if (value != this.repeatonNewPage)
                {
                    this.OnPropertyChanging("RepeatOnNewPage");
                    this.repeatonNewPage = value;
                    this.OnPropertyChanged("RepeatOnNewPage");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementName")]
        [DescriptionAttribute("Specify the name of the data element or attribute of report item.")]
        public string DataElementName
        {
            get
            {
                return this.dataElementName;
            }
            set
            {
                if (value != this.dataElementName)
                {
                    OnPropertyChanging("DataElementName");
                    this.dataElementName = value;
                    OnPropertyChanged("DataElementName");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementOutput")]
        [DescriptionAttribute("Indicates whether and how data element should appear when rendered.")]
        public string DataElementOutput
        {
            get
            {
                return this.dataElementOutput;
            }
            set
            {
                if (value != this.dataElementOutput)
                {
                    OnPropertyChanging("DataElementOutput");
                    this.dataElementOutput = value;
                    OnPropertyChanged("DataElementOutput");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Hidden")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the report item is initially hidden.")]
        public string Hidden
        {
            get
            {
                return this.hidden;
            }
            set
            {
                if (value != this.hidden)
                {
                    this.OnPropertyChanging("Hidden");
                    this.hidden = value;
                    this.OnPropertyChanged("Hidden");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("ToggleItem")]
        [DescriptionAttribute("Specifies name of the report item to click to show or hide another report item.")]
        public string ToggleItem
        {
            get
            {
                return this.toggleItem;
            }
            set
            {
                if (value != this.toggleItem)
                {
                    this.OnPropertyChanging("ToggleItem");
                    this.toggleItem = value;
                    this.OnPropertyChanged("ToggleItem");
                }
            }
        }

        //TablixMember Group Properties
        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DataElementName")]
        [DescriptionAttribute("Specify the name of the data element or attribute of report item.")]
        public string GroupDataElementName
        {
            get
            {
                return this.groupdataElementName;
            }
            set
            {
                if (value != this.groupdataElementName)
                {
                    OnPropertyChanging("GroupDataElementName");
                    this.groupdataElementName = value;
                    OnPropertyChanged("GroupDataElementName");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DataElementOutput")]
        [DescriptionAttribute("Indicates whether and how data element should appear when rendered.")]
        public string GroupDataElementOutput
        {
            get
            {
                return this.groupdataElementOutput;
            }
            set
            {
                if (value != this.groupdataElementOutput)
                {
                    OnPropertyChanging("GroupDataElementOutput");
                    this.groupdataElementOutput = value;
                    OnPropertyChanged("GroupDataElementOutput");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DocumentMapLabel")]
        [DescriptionAttribute("Identifeis an instance of a report item within the document map.")]
        public string DocumentMapLabel
        {
            get
            {
                return this.documentMapLabel;
            }
            set
            {
                if (value != this.documentMapLabel)
                {
                    this.OnPropertyChanging("DocumentMapLabel");
                    this.documentMapLabel = value;
                    this.OnPropertyChanged("DocumentMapLabel");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("PageBreak")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public RDL.DOM.BreakLocation PageBreak
        {
            get
            {
                return this.pageBreak;
            }
            set
            {
                if (value != this.pageBreak)
                {
                    this.OnPropertyChanging("PageBreak");
                    this.pageBreak = value;
                    this.OnPropertyChanged("PageBreak");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specfies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("Other")]
        [DisplayNameAttribute("Parent")]
        [DescriptionAttribute("Specifies an expression that identifies the parent group in a recursive hierarchy.This is allowed only if the group has exactly one group expression.")]
        public string Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (value != this.parent)
                {
                    this.OnPropertyChanging("Parent");
                    this.parent = value;
                    this.OnPropertyChanged("Parent");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DomainScope")]
        [DescriptionAttribute("Specifies the scope within which instance of this group are synchronized.")]
        public string DomainScope
        {
            get
            {
                return this.domainScope;
            }
            set
            {
                if (value != this.domainScope)
                {
                    this.OnPropertyChanging("DomainScope");
                    this.domainScope = value;
                    this.OnPropertyChanged("DomainScope");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    #endregion

    #region Rectangle properties

    internal class RectangleProperties : IReportItemProperties
    {
        string name = null;
        string hidden = null;
        string tooltip = null;
        string toggleItem = null;
        string omitOnBreak = null;
        string keeptogether = null;
        string backgroundColor = null;
        string dataElementName = null;
        string documentMapLabel = null;
        string dataElementOutput = null;
        RDL.DOM.BreakLocation pageBreak = RDL.DOM.BreakLocation.None;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        public RectangleProperties()
        {
            this.Size = new Sizes();
            this.Location = new Locations();
            this.BorderStyles = new BorderStyles();
            this.BorderWidths = new BorderWidths();
            this.BorderColors = new BorderColors();
            this.BackgroundImage = new BackgroundImage();

            this.Size.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Size.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.Location.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Location.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);            
            this.BorderStyles.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderStyles.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderWidths.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderWidths.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderColors.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderColors.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BackgroundImage.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BackgroundImage.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
        }

        public void UpdatePropertyValue()
        {
            this.BackgroundColor = "Transparent";
            this.KeepTogether = "True";
            this.Hidden = "False";
            this.DataElementOutput = "Auto";
            this.OmitBorderOnPageBreak = "False";
            this.BorderStyles.DefaultBorderStyle = "None";
            this.BorderWidths.DefaultBorderWidth = "1pt";
            this.BorderColors.DefaultBorderColor = "Black";
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specfies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("KeepTogether")]
        [DescriptionAttribute("Indicates whether to keep all sections of the data region together on one page .")]
        public string KeepTogether
        {
            get
            {
                return this.keeptogether;
            }
            set
            {
                if (value != this.keeptogether)
                {
                    this.OnPropertyChanging("KeepTogether");
                    this.keeptogether = value;
                    this.OnPropertyChanged("KeepTogether");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("PageBreak")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public RDL.DOM.BreakLocation PageBreak
        {
            get
            {
                return this.pageBreak;
            }
            set
            {
                if (value != this.pageBreak)
                {
                    this.OnPropertyChanging("PageBreak");
                    this.pageBreak = value;
                    this.OnPropertyChanged("PageBreak");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("OmitBorderOnPageBreak")]
        [DescriptionAttribute("Indicates whether border should appear around report items that span multiple pages.")]
        public string OmitBorderOnPageBreak
        {
            get
            {
                return this.omitOnBreak;
            }
            set
            {
                this.OnPropertyChanging("OmitBorderOnPageBreak");
                this.omitOnBreak = value;
                this.OnPropertyChanged("OmitBorderOnPageBreak");
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public BorderStyles BorderStyles
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the size of the border.")]
        public BorderWidths BorderWidths
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("BorderColor")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public BorderColors BorderColors
        {
            get;
            set;
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Background Color")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public string BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                if (value != this.backgroundColor)
                {
                    this.OnPropertyChanging("BackgroundColor");
                    this.backgroundColor = value;
                    this.OnPropertyChanged("BackgroundColor");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("BackgroundImage")]
        [DescriptionAttribute("Specfies the background image of the report item.")]
        public BackgroundImage BackgroundImage
        {
            get;
            set;
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementName")]
        [DescriptionAttribute("Specify the name of the data element or attribute of report item.")]
        public string DataElementName
        {
            get
            {
                return this.dataElementName;
            }
            set
            {
                if (value != this.dataElementName)
                {
                    OnPropertyChanging("DataElementName");
                    this.dataElementName = value;
                    OnPropertyChanged("DataElementName");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementOutput")]
        [DescriptionAttribute("Indicates whether and how data element should appear when rendered.")]
        public string DataElementOutput
        {
            get
            {
                return this.dataElementOutput;
            }
            set
            {
                if (value != this.dataElementOutput)
                {
                    OnPropertyChanging("DataElementOutput");
                    this.dataElementOutput = value;
                    OnPropertyChanged("DataElementOutput");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Hidden")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the report item is initially hidden.")]
        public string Hidden
        {
            get
            {
                return this.hidden;
            }
            set
            {
                if (value != this.hidden)
                {
                    this.OnPropertyChanging("Hidden");
                    this.hidden = value;
                    this.OnPropertyChanged("Hidden");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("ToggleItem")]
        [DescriptionAttribute("Specifies name of the report item to click to show or hide another report item.")]
        public string ToggleItem
        {
            get
            {
                return this.toggleItem;
            }
            set
            {
                if (value != this.toggleItem)
                {
                    this.OnPropertyChanging("ToggleItem");
                    this.toggleItem = value;
                    this.OnPropertyChanged("ToggleItem");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DocumentMapLabel")]
        [DescriptionAttribute("Identifeis an instance of a report item within the document map.")]
        public string DocumentMapLabel
        {
            get
            {
                return this.documentMapLabel;
            }
            set
            {
                if (value != this.documentMapLabel)
                {
                    this.OnPropertyChanging("DocumentMapLabel");
                    this.documentMapLabel = value;
                    this.OnPropertyChanged("DocumentMapLabel");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("ToolTip")]
        [DescriptionAttribute("Specifies the textual label for the report item. For example, it can be used to render TITLE and ALT attributes in HTML reports.")]
        public string ToolTip
        {
            get
            {
                return this.tooltip;
            }
            set
            {
                if (value != this.tooltip)
                {
                    this.OnPropertyChanging("ToolTip");
                    this.tooltip = value;
                    this.OnPropertyChanged("ToolTip");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Location")]
        [DescriptionAttribute("Specifies the position of the top-left corner of a report item in relation to its container.")]
        public Locations Location
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Size")]
        [DescriptionAttribute("Specifies the Size of the report item")]
        public Sizes Size
        {
            get;
            set;
        }


        string width = null;
        string height = null;

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specifies the width of the report item")]
        public string Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value != this.width)
                {
                    this.OnPropertyChanging("Width");
                    this.width = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specifies the height of the report item")]
        public string Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value != this.height)
                {
                    this.OnPropertyChanging("Height");
                    this.height = value;
                    this.OnPropertyChanged("Height");
                }
            }
        }

        string left = null;
        string top = null;

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the distance of the item from the left of the containing object.")]
        public string Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("Left");
                    this.left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the distance of the item from the top of the containing object.")]
        public string Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("Top");
                    this.top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    #endregion

    #region Gauge properties

    internal class GaugeProperties : IReportItemProperties
    {
        string borderwidth = null;
        string bordercolor = null;
        string borderStyle = null;
        string name = null;
        string thickness = null;
        string type = null;
        string backfill = null;
        string framefill = null;
        string range = null;
        string pointer = null;
        string scale = null;
        string hidden = null;
        string toggleItem = null;
        string dataElementName = null;
        string documentMapLabel = null;
        string dataElementOutput = null;
        RDL.DOM.BreakLocation pageBreak = RDL.DOM.BreakLocation.None;

        public GaugeProperties()
        {
            this.Size = new Sizes();
            this.Location = new Locations();

            this.Size.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Size.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.Location.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Location.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
        }

        public void UpdatePropertyValue()
        {
            this.Thickness = "8pt";
            this.Type = "Circular1";
            this.FrameFill = "LightGray";
            this.BackFill = "SkyBlue";
            this.BorderColor = "Black";
            this.BorderStyle = "None";
            this.BorderWidth = "1pt";
            this.Hidden = "False";
            this.DataElementOutput = "Auto";
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the border width of the report item.")]
        public string BorderWidth
        {
            get
            {
                return this.borderwidth;
            }
            set
            {
                if (value != this.borderwidth)
                {
                    this.OnPropertyChanging("BorderWidth");
                    this.borderwidth = value;
                    this.OnPropertyChanged("BorderWidth");
                }
            }
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Color")]
        [DescriptionAttribute("Specifies the border color of the report item.")]
        public string BorderColor
        {
            get
            {
                return this.bordercolor;
            }
            set
            {
                if (value != this.bordercolor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("BorderColor");
                    this.bordercolor = value;
                    this.OnPropertyChanged("BorderColor");
                }
            }
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public string BorderStyle
        {
            get
            {
                return this.borderStyle;
            }
            set
            {
                if (value != this.borderStyle)
                {
                    this.OnPropertyChanging("BorderStyle");
                    this.borderStyle = value;
                    this.OnPropertyChanged("BorderStyle");
                }
            }
        }


        [CategoryAttribute("Data")]
        [DisplayNameAttribute("DatasetName")]
        [DescriptionAttribute("Specifies the name of the Dataset.")]
        public string Dataset
        {
            get;
            set;
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementName")]
        [DescriptionAttribute("Specify the name of the data element or attribute of report item.")]
        public string DataElementName
        {
            get
            {
                return this.dataElementName;
            }
            set
            {
                if (value != this.dataElementName)
                {
                    OnPropertyChanging("DataElementName");
                    this.dataElementName = value;
                    OnPropertyChanged("DataElementName");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementOutput")]
        [DescriptionAttribute("Indicates whether and how data element should appear when rendered.")]
        public string DataElementOutput
        {
            get
            {
                return this.dataElementOutput;
            }
            set
            {
                if (value != this.dataElementOutput)
                {
                    OnPropertyChanging("DataElementOutput");
                    this.dataElementOutput = value;
                    OnPropertyChanged("DataElementOutput");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.OnPropertyChanging("Name");
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Thickness")]
        [DescriptionAttribute("Specifies the thickness of the report item.")]
        public string Thickness
        {
            get
            {
                return this.thickness;
            }
            set
            {
                if (value != this.thickness)
                {
                    this.OnPropertyChanging("Thickness");
                    this.thickness = value;
                    this.OnPropertyChanged("Thickness");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Type")]
        [DescriptionAttribute("Specifies the Type of the report item.")]
        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                if (value != this.type)
                {
                    this.OnPropertyChanging("Type");
                    this.type = value;
                    this.OnPropertyChanged("Type");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Back Fill")]
        [DescriptionAttribute("Specifies the background color of the gauge.")]
        public string BackFill
        {
            get
            {
                return this.backfill;
            }
            set
            {
                if (value != this.backfill && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("BackFill");
                    this.backfill = value;
                    this.OnPropertyChanged("BackFill");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Frame Fill")]
        [DescriptionAttribute("Specifies the background color of the gauge.")]
        public string FrameFill
        {
            get
            {
                return this.framefill;
            }
            set
            {
                if (value != this.framefill && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("FrameFill");
                    this.framefill = value;
                    this.OnPropertyChanged("FrameFill");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("PageBreak")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public RDL.DOM.BreakLocation PageBreak
        {
            get
            {
                return this.pageBreak;
            }
            set
            {
                if (value != this.pageBreak)
                {
                    this.OnPropertyChanging("PageBreak");
                    this.pageBreak = value;
                    this.OnPropertyChanged("PageBreak");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Hidden")]
        [DefaultValue("False")]
        [DescriptionAttribute("Indicates whether the report item is initially hidden.")]
        public string Hidden
        {
            get
            {
                return this.hidden;
            }
            set
            {
                if (value != this.hidden)
                {
                    this.OnPropertyChanging("Hidden");
                    this.hidden = value;
                    this.OnPropertyChanged("Hidden");
                }
            }
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("ToggleItem")]
        [DescriptionAttribute("Specifies name of the report item to click to show or hide another report item.")]
        public string ToggleItem
        {
            get
            {
                return this.toggleItem;
            }
            set
            {
                if (value != this.toggleItem)
                {
                    this.OnPropertyChanging("ToggleItem");
                    this.toggleItem = value;
                    this.OnPropertyChanged("ToggleItem");
                }
            }
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("DocumentMapLabel")]
        [DescriptionAttribute("Identifeis an instance of a report item within the document map.")]
        public string DocumentMapLabel
        {
            get
            {
                return this.documentMapLabel;
            }
            set
            {
                if (value != this.documentMapLabel)
                {
                    this.OnPropertyChanging("DocumentMapLabel");
                    this.documentMapLabel = value;
                    this.OnPropertyChanged("DocumentMapLabel");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Location")]
        [DescriptionAttribute("Specifies the position of the top-left corner of a report item in relation to its container.")]
        public Locations Location
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Position")]
        [DisplayNameAttribute("Size")]
        [DescriptionAttribute("Specifies the Size of the report item")]
        public Sizes Size
        {
            get;
            set;
        }


        string width = null;
        string height = null;

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specifies the width of the report item")]
        public string Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value != this.width)
                {
                    this.OnPropertyChanging("Width");
                    this.width = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specifies the height of the report item")]
        public string Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value != this.height)
                {
                    this.OnPropertyChanging("Height");
                    this.height = value;
                    this.OnPropertyChanged("Height");
                }
            }
        }

        string left = null;
        string top = null;

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Left")]
        [DescriptionAttribute("Specifies the distance of the item from the left of the containing object.")]
        public string Left
        {
            get
            {
                return this.left;
            }
            set
            {
                if (this.left != value)
                {
                    OnPropertyChanging("Left");
                    this.left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        [CategoryAttribute("Location")]
        [DisplayNameAttribute("Top")]
        [DescriptionAttribute("Specifies the distance of the item from the top of the containing object.")]
        public string Top
        {
            get
            {
                return this.top;
            }
            set
            {
                if (this.top != value)
                {
                    OnPropertyChanging("Top");
                    this.top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        [Browsable(false)]
        public string Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                if (value != this.scale)
                {
                    this.OnPropertyChanging("Scale");
                    this.scale = value;
                    this.OnPropertyChanged("Scale");
                }
            }
        }

        [Browsable(false)]
        public string Pointer
        {
            get
            {
                return this.pointer;
            }
            set
            {
                if (value != this.pointer)
                {
                    this.OnPropertyChanging("Pointer");
                    this.pointer = value;
                    this.OnPropertyChanged("Pointer");
                }
            }
        }

        [Browsable(false)]
        public string Range
        {
            get
            {
                return this.range;
            }
            set
            {
                if (value != this.range)
                {
                    this.OnPropertyChanging("Range");
                    this.range = value;
                    this.OnPropertyChanged("Range");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion
    }

    internal class GaugePanelProperties
    {
        public GaugePanelProperties()
        {
            this.Hidden = "False";
            this.Style = "Solid";
            this.PageBreak = "None";
            this.BorderWidth = "1pt";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Name
        {
            get;
            set;
        }


        [CategoryAttribute("General")]
        [DisplayNameAttribute("PageBreak")]
        [DescriptionAttribute("Indicates how the rendering extension inserts a page break in relation to the group.")]
        public string PageBreak
        {
            get;
            set;
        }

        [CategoryAttribute("Visibility")]
        [DisplayNameAttribute("Visibility")]
        [DescriptionAttribute("Specifies the name of the report item.")]
        public string Hidden
        {
            get;
            set;
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Back Fill")]
        [DescriptionAttribute("Specifies the background color of the gaugePanel.")]
        public string BackFill
        {
            get;
            set;
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the border width of the entire border or the individual border widths of the item.")]
        public string BorderWidth
        {
            get;
            set;
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Color")]
        [DescriptionAttribute("Specifies the color of the entire border or the individual border lines of the item.")]
        public string BorderColor
        {
            get;
            set;
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Style")]
        [DescriptionAttribute("Specifies the border style of the report item.")]
        public string Style
        {
            get;
            set;
        }
    }

    internal class RangeProperties
    {
        public RangeProperties()
        {

            this.DistanceFromScale = 30;
            this.Placement = "Inside";
            this.BorderWidth = "3pt";
            this.StartWidth = 30;
            this.EndWidth = 30;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Start Range")]
        [DescriptionAttribute("Specifies starting range of the report item")]
        public int StartRange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("End Range")]
        [DescriptionAttribute("Specifies the end range of the item.")]
        public int EndRange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Start Width")]
        [DescriptionAttribute("Specifies the starting width of the item.")]
        public int StartWidth
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("End Width")]
        [DescriptionAttribute("Specifies the end width of the item.")]
        public int EndWidth
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Distance From Scale")]
        [DescriptionAttribute("Specifies the distance from the scale value of the item.")]
        public int DistanceFromScale
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Placement")]
        [DescriptionAttribute("Specifies placment of the report item.")]
        public string Placement
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Back Fill")]
        [DescriptionAttribute("Specifies the background color of the gaugePanel.")]
        public string BackFill
        {
            get;
            set;
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the border width of the entire border or the individual border widths of the item.")]
        public string BorderWidth
        {
            get;
            set;
        }

        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Color")]
        [DescriptionAttribute("Specifies the color of the entire border or the individual border lines of the item.")]
        public string BorderColor
        {
            get;
            set;
        }
    }

    internal class PointerProperties
    {
        public PointerProperties()
        {
            this.PointerType = "Needle";
            this.NeedleType = "Triangle";
            this.BorderWidth = "0.25pt";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Pointer Width")]
        [DescriptionAttribute("Specifies the pointer width of the Gauge item")]
        public int PointerWidth
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Pointer Type")]
        [DescriptionAttribute("Specifies the pointer type of the Gauge item")]
        public string PointerType
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Needle Type")]
        [DescriptionAttribute("Specifies the needle type of the Gauge item")]
        public string NeedleType
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Pointer Value")]
        [DescriptionAttribute("Specifies the pointer value of the Gauge item")]
        public int PointerValue
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Cap Radius")]
        [DescriptionAttribute("Specifies the cap radius of the Gauge item")]
        public int CapRadius
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Cap Color")]
        [DescriptionAttribute("Specifies the cap color of the Gauge item")]
        public string CapColor
        {
            get;
            set;
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Back Fill")]
        [DescriptionAttribute("Specifies the background color of the gaugePanel.")]
        public string BackFill
        {
            get;
            set;
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the border width of the entire border or the individual border widths of the item.")]
        public string BorderWidth
        {
            get;
            set;
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Border Color")]
        [DescriptionAttribute("Specifies the color of the entire border or the individual border lines of the item.")]
        public string BorderColor
        {
            get;
            set;
        }
    }

    internal class ScaleProperties
    {
        public ScaleProperties()
        {
            this.FontFamily = "Segoe UI";
            this.FontSize = "14pt";
            this.Placement = "Inside";
            this.FontStyle = "Default";
            this.MajorTickLength = 19;
            this.MajorTickWidth = 2;
            this.MajorTickShape = "Rectangle";
            this.MajorTickPlacement = "Cross";
            this.MinorTickLength = 10;
            this.MinorTickWidth = 1;
            this.MinorTickShape = "Rectangle";
            this.MinorTickPlacement = "Cross";
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Minimum Value")]
        [DescriptionAttribute("Specifies the minimum scale value of the Gauge item")]
        public int MinimumValue
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Maximum Value")]
        [DescriptionAttribute("Specifies the maximum scale value of the Gauge item")]
        public int MaximumValue
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Multiply Scale Labels Value")]
        [DescriptionAttribute("Specifies the multiply scale value of the Gauge item")]
        public int MultiplyScaleValue
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Scale Radius")]
        [DescriptionAttribute("Specifies the scale radius value of the Gauge item")]
        public int ScaleRadius
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Start Angle")]
        [DescriptionAttribute("Specifies the starting angle of the scale")]
        public int StartAngle
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Sweep Angle")]
        [DescriptionAttribute("Specifies the sweep angle of the scale")]
        public int SweepAngle
        {
            get;
            set;
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Placement")]
        [DescriptionAttribute("Specifies placment of the report item.")]
        public string Placement
        {
            get;
            set;
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Family")]
        [DescriptionAttribute("Specifies the name of the font family.")]
        public string FontFamily
        {
            get;
            set;
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Size")]
        [DescriptionAttribute("Specifies the point size of the label.")]
        public string FontSize
        {
            get;
            set;
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Style")]
        [DescriptionAttribute("Specifies the style of the label.")]
        public string FontStyle
        {
            get;
            set;
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Color")]
        [DescriptionAttribute("Specifies the color of the label.")]
        public string FontColor
        {
            get;
            set;
        }

        [CategoryAttribute("Label")]
        [DisplayNameAttribute("Font Angle")]
        [DescriptionAttribute("Specifies the angle of the label.")]
        public int FontAngle
        {
            get;
            set;
        }

        [CategoryAttribute("Tick")]
        [DisplayNameAttribute("Major Tick Shape")]
        [DescriptionAttribute("Specifies the style of the tick mark.")]
        public string MajorTickShape
        {
            get;
            set;
        }

        [CategoryAttribute("Tick")]
        [DisplayNameAttribute("Major Tick Width")]
        [DescriptionAttribute("Specifies the width of the tick mark.")]
        public int MajorTickWidth
        {
            get;
            set;
        }

        [CategoryAttribute("Tick")]
        [DisplayNameAttribute("Major Tick Placement")]
        [DescriptionAttribute("Specifies placment of the report item.")]
        public string MajorTickPlacement
        {
            get;
            set;
        }

        [CategoryAttribute("Tick")]
        [DisplayNameAttribute("Major Tick Color")]
        [DescriptionAttribute("Specifies the color of the tick mark.")]
        public string MajorTickColor
        {
            get;
            set;
        }

        [CategoryAttribute("Tick")]
        [DisplayNameAttribute("Major Tick Length")]
        [DescriptionAttribute("Specifies the lenhgt of the tick mark.")]
        public int MajorTickLength
        {
            get;
            set;
        }

        [CategoryAttribute("Tick")]
        [DisplayNameAttribute("Minor Tick Shape")]
        [DescriptionAttribute("Specifies the style of the tick mark.")]
        public string MinorTickShape
        {
            get;
            set;
        }

        [CategoryAttribute("Tick")]
        [DisplayNameAttribute("Minor Tick Width")]
        [DescriptionAttribute("Specifies the width of the tick mark.")]
        public int MinorTickWidth
        {
            get;
            set;
        }

        [CategoryAttribute("Tick")]
        [DisplayNameAttribute("Minor Tick Color")]
        [DescriptionAttribute("Specifies the color of the tick mark.")]
        public string MinorTickColor
        {
            get;
            set;
        }

        [CategoryAttribute("Tick")]
        [DisplayNameAttribute("Minor Tick Placement")]
        [DescriptionAttribute("Specifies placment of the report item.")]
        public string MinorTickPlacement
        {
            get;
            set;
        }

        [CategoryAttribute("Tick")]
        [DisplayNameAttribute("Minor Tick Length")]
        [DescriptionAttribute("Specifies the lenhgt of the tick mark.")]
        public int MinorTickLength
        {
            get;
            set;
        }
    }

    #endregion

    #region Report related properties

    internal class HeaderProperties : IDesignerProperties
    {
        string backgroundColor = null;
        string headerHeight = "1in";
        string printOnFirstPage = null;
        string printOnLastPage = null;

        public HeaderProperties()
        {
            this.BorderStyles = new BorderStyles();
            this.BorderWidths = new BorderWidths();
            this.BorderColors = new BorderColors();
            this.BackgroundImage = new BackgroundImage();
            this.BorderStyles.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderStyles.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderWidths.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderWidths.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderColors.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderColors.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BackgroundImage.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BackgroundImage.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);

            this.BackgroundColor = "White";
            this.PrintOnFirstPage = "True";
            this.PrintOnLastPage = "True";
            this.BorderStyles.DefaultBorderStyle = "None";
            this.BorderWidths.DefaultBorderWidth = "1pt";
            this.BorderColors.DefaultBorderColor = "Black";
        }

        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specfies the header height of the report.")]
        public string HeaderHeight
        {
            get
            {
                return headerHeight;
            }
            set
            {
                if (headerHeight != value)
                {
                    this.OnPropertyChanging("HeaderHeight");
                    headerHeight = value;
                    this.OnPropertyChanged("HeaderHeight");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("PrintOnFirstPage")]
        [DefaultValue("True")]
        [DescriptionAttribute("Indicates whether the page header is included on the first page of the report.")]
        public string PrintOnFirstPage
        {
            get
            {
                return this.printOnFirstPage;
            }
            set
            {
                if (value != this.printOnFirstPage)
                {
                    this.OnPropertyChanging("PrintOnFirstPage");
                    this.printOnFirstPage = value;
                    this.OnPropertyChanged("PrintOnFirstPage");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("PrintOnLastPage")]
        [DefaultValue("True")]
        [DescriptionAttribute("Indicates whether the page header is included on the last page of the report.")]
        public string PrintOnLastPage
        {
            get
            {
                return this.printOnLastPage;
            }
            set
            {
                if (value != this.printOnLastPage)
                {
                    this.OnPropertyChanging("PrintOnLastPage");
                    this.printOnLastPage = value;
                    this.OnPropertyChanged("PrintOnLastPage");
                }
            }
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Background Color")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public string BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                if (value != this.backgroundColor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("BackgroundColor");
                    this.backgroundColor = value;
                    this.OnPropertyChanged("BackgroundColor");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("BackgroundImage")]
        [DisplayNameAttribute("Source")]
        [DescriptionAttribute("Specfies the background image of the report item.")]
        public BackgroundImage BackgroundImage
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public BorderStyles BorderStyles
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the size of the border.")]
        public BorderWidths BorderWidths
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("BorderColor")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public BorderColors BorderColors
        {
            get;
            set;
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    internal class FooterProperties : IDesignerProperties
    {
        string backgroundColor = null;
        string footerHeight = "1in";
        string printOnFirstPage = null;
        string printOnLastPage = null;

        public FooterProperties()
        {            
            this.BorderStyles = new BorderStyles();
            this.BorderWidths = new BorderWidths();
            this.BorderColors = new BorderColors();
            this.BackgroundImage = new BackgroundImage();
            this.BorderStyles.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderStyles.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderWidths.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderWidths.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderColors.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderColors.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BackgroundImage.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BackgroundImage.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);

            this.BackgroundColor = "White";
            this.PrintOnFirstPage = "True";
            this.PrintOnLastPage = "True";
            this.BorderStyles.DefaultBorderStyle = "None";
            this.BorderWidths.DefaultBorderWidth = "1pt";
            this.BorderColors.DefaultBorderColor = "Black";
        }

        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }
        
        [CategoryAttribute("General")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specfies the footer height of the report.")]
        public string FooterHeight
        {
            get
            {
                return footerHeight;
            }
            set
            {
                if (footerHeight != value)
                {
                    this.OnPropertyChanging("FooterHeight");
                    footerHeight = value;
                    this.OnPropertyChanged("FooterHeight");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("PrintOnFirstPage")]
        [DefaultValue("True")]
        [DescriptionAttribute("Indicates whether the page footer is included on the first page of the report.")]
        public string PrintOnFirstPage
        {
            get
            {
                return this.printOnFirstPage;
            }
            set
            {
                if (value != this.printOnFirstPage)
                {
                    this.OnPropertyChanging("PrintOnFirstPage");
                    this.printOnFirstPage = value;
                    this.OnPropertyChanged("PrintOnFirstPage");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("PrintOnLastPage")]
        [DefaultValue("True")]
        [DescriptionAttribute("Indicates whether the page footer is included on the last page of the report.")]
        public string PrintOnLastPage
        {
            get
            {
                return this.printOnLastPage;
            }
            set
            {
                if (value != this.printOnLastPage)
                {
                    this.OnPropertyChanging("PrintOnLastPage");
                    this.printOnLastPage = value;
                    this.OnPropertyChanged("PrintOnLastPage");
                }
            }
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Background Color")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public string BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                if (value != this.backgroundColor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("BackgroundColor");
                    this.backgroundColor = value;
                    this.OnPropertyChanged("BackgroundColor");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("BackgroundImage")]
        [DescriptionAttribute("Specfies the background image of the report item.")]
        public BackgroundImage BackgroundImage
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public BorderStyles BorderStyles
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the size of the border.")]
        public BorderWidths BorderWidths
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("BorderColor")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public BorderColors BorderColors
        {
            get;
            set;
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    internal class BodyProperties : IDesignerProperties
    {
        string backgroundColor = null;
        string bodywidth = "6in";
        string bodyHeight = "2.5in";

        public BodyProperties()
        {
            this.BorderStyles = new BorderStyles();
            this.BorderWidths = new BorderWidths();
            this.BorderColors = new BorderColors();
            this.BackgroundImage = new BackgroundImage();
            this.BorderStyles.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderStyles.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderWidths.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderWidths.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderColors.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderColors.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BackgroundImage.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BackgroundImage.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);

            this.BackgroundColor = "White";
            this.BorderStyles.DefaultBorderStyle = "None";
            this.BorderWidths.DefaultBorderWidth = "1pt";
            this.BorderColors.DefaultBorderColor = "Black";
        }

        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("Background Color")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public string BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                if (value != this.backgroundColor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("BackgroundColor");
                    this.backgroundColor = value;
                    this.OnPropertyChanged("BackgroundColor");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Fill")]
        [DisplayNameAttribute("BackgroundImage")]
        [DescriptionAttribute("Specfies the background image of the report item.")]
        public BackgroundImage BackgroundImage
        {
            get;
            set;
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specfies the body width of the report.")]
        public string ReportWidth
        {
            get
            {
                return this.bodywidth;
            }
            set
            {
                if (this.bodywidth != value)
                {
                    this.OnPropertyChanging("ReportWidth");
                    this.bodywidth = value;
                    this.OnPropertyChanged("ReportWidth");
                }
            }
        }

        [CategoryAttribute("Size")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specfies the body height of the report.")]
        public string BodyHeight
        {
            get
            {
                return this.bodyHeight;
            }
            set
            {
                if (this.bodyHeight != value)
                {
                    this.OnPropertyChanging("BodyHeight");
                    this.bodyHeight = value;
                    this.OnPropertyChanged("BodyHeight");
                }
            }
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public BorderStyles BorderStyles
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the size of the border.")]
        public BorderWidths BorderWidths
        {
            get;
            set;
        }

        [Browsable(false)]
        [CategoryAttribute("Border")]
        [DisplayNameAttribute("BorderColor")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public BorderColors BorderColors
        {
            get;
            set;
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    internal class ReportProperties : IDesignerProperties
    {
        int autoRefresh = 0;
        string author = null;
        string pagewidth = "8.5in";
        string pageheight = "11in";
        string description = null;
        string dataSchema = null;
        string dataTransform = null;
        string backgroundColor = null;
        string dataElementName = null;
        string dataElementStyle = null;

        public ReportProperties()
        {
            this.Margins = new Margins();
            this.BorderStyles = new BorderStyles();
            this.BorderWidths = new BorderWidths();
            this.BorderColors = new BorderColors();
            this.BackgroundImage = new BackgroundImage();
            this.Margins.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.Margins.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderStyles.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderStyles.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderWidths.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderWidths.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BorderColors.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BorderColors.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);
            this.BackgroundImage.PropertyChanging += new PropertyChangingEventHandler(InnerProperty_PropertyChanging);
            this.BackgroundImage.PropertyChanged += new PropertyChangedEventHandler(InnerProperty_PropertyChanged);

            this.BackgroundColor = "White";
            this.DataElementStyle = "Auto";
            this.BorderStyles.DefaultBorderStyle = "None";
            this.BorderWidths.DefaultBorderWidth = "1pt";
            this.BorderColors.DefaultBorderColor = "Black";
            this.Margins.LeftMargin = this.Margins.RightMargin = this.Margins.TopMargin = this.Margins.BottomMargin = "1in";
        }

        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsTablixCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public bool IsInternalPropertyChange
        {
            get;
            set;
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Author")]
        [DescriptionAttribute("Specfies report author.")]
        public string Author
        {
            get
            {
                return this.author;
            }
            set
            {
                if (value != this.author)
                {
                    this.OnPropertyChanging("Author");
                    this.author = value;
                    this.OnPropertyChanged("Author");
                }
            }
        }

        [CategoryAttribute("General")]
        [DisplayNameAttribute("Description")]
        [DescriptionAttribute("Specfies report description.")]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (value != this.description)
                {
                    this.OnPropertyChanging("Description");
                    this.description = value;
                    this.OnPropertyChanged("Description");
                }
            }
        }


        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementName")]
        [DescriptionAttribute("Specify the name of the data element or attribute of report item.")]
        public string DataElementName
        {
            get
            {
                return this.dataElementName;
            }
            set
            {
                if (value != this.dataElementName)
                {
                    OnPropertyChanging("DataElementName");
                    this.dataElementName = value;
                    OnPropertyChanged("DataElementName");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataElementStyle")]
        [DescriptionAttribute("Indicates whether a text box within the report renders as an element or an attribute when the report is rendered to XML.")]
        public string DataElementStyle
        {
            get
            {
                return this.dataElementStyle;
            }
            set
            {
                if (value != this.dataElementStyle)
                {
                    OnPropertyChanging("DataElementStyle");
                    this.dataElementStyle = value;
                    OnPropertyChanged("DataElementStyle");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataSchema")]
        [DescriptionAttribute("Specifies schema or namespace to be used when rendering report data.")]
        public string DataSchema
        {
            get
            {
                return this.dataSchema;
            }
            set
            {
                if (value != this.dataElementStyle)
                {
                    OnPropertyChanging("DataSchema");
                    this.dataSchema = value;
                    OnPropertyChanged("DataSchema");
                }
            }
        }

        [CategoryAttribute("Data Only")]
        [DisplayNameAttribute("DataTransform")]
        [DescriptionAttribute("Specifies the location and file name of the XSLT transformation to apply when rendering to XML.")]
        public string DataTransform
        {
            get
            {
                return this.dataTransform;
            }
            set
            {
                if (value != this.dataTransform)
                {
                    OnPropertyChanging("DataTransform");
                    this.dataTransform = value;
                    OnPropertyChanged("DataTransform");
                }
            }
        }

        [CategoryAttribute("Page")]
        [DisplayNameAttribute("Width")]
        [DescriptionAttribute("Specfies the page width of the report.")]
        public string PageWidth
        {
            get
            {
                return this.pagewidth;
            }
            set
            {
                if (value != this.pagewidth)
                {
                    this.OnPropertyChanging("PageWidth");
                    this.pagewidth = value;
                    this.OnPropertyChanged("PageWidth");
                }
            }
        }

        [CategoryAttribute("Page")]
        [DisplayNameAttribute("Height")]
        [DescriptionAttribute("Specfies the page height of the report.")]
        public string PageHeight
        {
            get
            {
                return this.pageheight;
            }
            set
            {
                if (value != this.pageheight)
                {
                    this.OnPropertyChanging("PageHeight");
                    this.pageheight = value;
                    this.OnPropertyChanged("PageHeight");
                }
            }
        }

        [CategoryAttribute("Page")]
        [DisplayNameAttribute("Background Color")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public string BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                if (value != this.backgroundColor && value != "#00FFFFFF")
                {
                    this.OnPropertyChanging("BackgroundColor");
                    this.backgroundColor = value;
                    this.OnPropertyChanged("BackgroundColor");
                }
            }
        }

        [ReadOnly(true)]
        [CategoryAttribute("Page")]
        [DisplayNameAttribute("BackgroundImage")]
        [DescriptionAttribute("Specfies the background image of the report item.")]
        public BackgroundImage BackgroundImage
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Page")]
        [DisplayNameAttribute("Margins")]
        [DescriptionAttribute("Specfies the left,right,top or bottom margines of report.")]
        public Margins Margins
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Page")]
        [DisplayNameAttribute("Border Style")]
        [DescriptionAttribute("Specfies the border style of the report item.")]
        public BorderStyles BorderStyles
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Page")]
        [DisplayNameAttribute("Border Width")]
        [DescriptionAttribute("Specifies the size of the border.")]
        public BorderWidths BorderWidths
        {
            get;
            set;
        }

        [ReadOnly(true)]
        [CategoryAttribute("Page")]
        [DisplayNameAttribute("BorderColor")]
        [DescriptionAttribute("Specfies the background color of the report item.")]
        public BorderColors BorderColors
        {
            get;
            set;
        }

        [CategoryAttribute("Other")]
        [DisplayNameAttribute("AutoRefresh")]
        [DescriptionAttribute("Specfies the rate in second at which the report page automatically refreshes when rendered in HTML.")]
        public int AutoRefresh
        {
            get
            {
                return this.autoRefresh;
            }
            set
            {
                if (value != this.autoRefresh)
                {
                    this.OnPropertyChanging("AutoRefresh");
                    this.autoRefresh = value;
                    this.OnPropertyChanged("AutoRefresh");
                }
            }
        }

        #region InnerProperty Events

        void InnerProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void InnerProperty_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            this.OnPropertyChanging(e.PropertyName);
        }

        #endregion

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }

    #endregion
}