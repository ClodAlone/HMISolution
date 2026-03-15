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
using System.Windows.Controls;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.ComponentModel;
using Syncfusion.RDL.DOM;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Documents;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using System.Windows.Data;
using System.IO;
using System.Xml.Serialization;
using Syncfusion.RDL.Internal;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;


namespace Syncfusion.Windows.Reports.Designer.Controls
{

    internal class TextBoxControl : RichTextBox, IReportItemControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Constructors

        public TextBoxControl()
        {
            this.Foreground = Brushes.Black;
            this.Properties = new Editors.TextBoxProperties();
            this.Properties.PropertyChanged += new PropertyChangedEventHandler(Properties_PropertyChanged);
            this.Properties.PropertyChanging += new PropertyChangingEventHandler(Properties_PropertyChanging);
            this.propertyValueConvertor = new ReportingConvertorUtil();
        }        

        #endregion
        
        #region private variables

        private object propertyOldValue = null;
        private Run currentRun;
        private Brush currentRunBackgroundColor;
        private CommandBinding copyBinding;
        private CommandBinding pasteBinding;
        private CommandBinding cutBinding;
        private List<Run> listRun = new List<Run>(); // Used to maintain the run elements which selected for copy
        private bool isFocusedItem;
        private bool isItemSelected;

        internal ReportingConvertorUtil propertyValueConvertor;
        
        public System.Windows.Documents.FlowDocument document;
        internal Syncfusion.Windows.Reports.Designer.Editors.TextBoxProperties Properties;

        XmlSerializer textSerializer = new XmlSerializer(typeof(RDL.DOM.TextBox));
        string oldItemString = null;
        RDL.DOM.ReportItem oldItem = null;

        #endregion

        #region private properties

        internal MenuItem TextBoxProperties { get; set; }
        private MenuItem TextProperties { get; set; }
        private MenuItem CreatePlaceHolder { get; set; }
        private MenuItem Expression { get; set; }
        private new MenuItem Cut { get; set; }
        private new MenuItem Copy { get; set; }
        private new MenuItem Paste { get; set; }

        #endregion

        #region public properties

        public static int TextBoxCount 
        { 
            get; 
            set; 
        }

        public int TextLength
        {
            get
            {
                return getTextLength();
            }
        }

        public RichTextBox InnerTextBox
        {
            get;
            set;
        }

        public System.Windows.Documents.Paragraph paragraph
        {
            get;
            set;
        }

        public DesignerDashStyleBorder InnerBorder
        {
            get;
            set;
        }

        public DataSets ReportDataSets
        {
            get;
            set;
        }

        public DataSet DataSet
        {
            get;
            set;
        }

        public bool TextBoxVisibility
        {
            get;
            set;
        }

        public RDL.DOM.Action Action { get; set; }

        public ContextMenu TextBoxControlContextMenu { get; set; }

        #endregion                 
        
        #region ReportItemControl Interface

        public ReportItem ReportItem
        {
            get;
            set;
        }

        public DesignPanel Panel
        {
            get;
            set;
        }

        public string ItemName
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        }

        public double ItemHeight
        {
            get
            {
                return this.ActualHeight;
            }
            set
            {
                if (this.ItemHeight != value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Height = propertyValueConvertor.GetSizeValue(value, this.Properties.Height);
                    this.Properties.IsInternalPropertyChange = false;
                }
                this.Height = value;
            }
        }

        public double ItemWidth
        {
            get
            {
                return this.ActualWidth;
            }
            set
            {
                if (this.ItemWidth != value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Width = propertyValueConvertor.GetSizeValue(value, this.Properties.Width);
                    this.Properties.IsInternalPropertyChange = false;
                }
                this.Width = value;
            }
        }

        public double ItemTop
        {
            get
            {
                return Canvas.GetTop(this);
            }
            set
            {
                if (this.ItemTop != value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Top = propertyValueConvertor.GetSizeValue(value, this.Properties.Top);
                    this.Properties.IsInternalPropertyChange = false;
                }
                Canvas.SetTop(this, value);
            }
        }

        public double ItemLeft
        {
            get
            {
                return Canvas.GetLeft(this);
            }
            set
            {
                if (this.ItemLeft != value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Left = propertyValueConvertor.GetSizeValue(value, this.Properties.Left);
                    this.Properties.IsInternalPropertyChange = false;
                }
                Canvas.SetLeft(this, value);
            }
        }

        public bool IsTablixItem
        {
            get;
            set;
        }

        public bool IsItemSelected
        {
            get
            {
                return this.isItemSelected;
            }
            set
            {
                if (this.isItemSelected != value)
                {
                    isItemSelected = value;
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.Properties, IsSelected = value });
                    OnPropertyChanged("IsItemSelected");
                }
            }
        }

        public bool IsFocusedItem
        {
            get
            {
                return isFocusedItem;
            }
            set
            {
                if (isFocusedItem != value)
                {
                    this.isFocusedItem = value;
                    OnPropertyChanged("IsFocusedItem");
                }
            }
        }


        public new Canvas Parent { get; set; }

        public DrawingReportItem ItemType
        {
            get
            {
                return DrawingReportItem.TextBox;
            }
        }

        public event ReportItemControlSizeHandler ReportItemSizeChanged;

        public void RaiseReportItemSizeChangedEvent()
        {
            if (this.ReportItemSizeChanged != null)
            {
                this.ReportItemSizeChanged(this, new EventArgs());
            }
        }

        public event ReportItemSelectedEvent ReportItemSelected;

        public void RaiseReportItemSelectedEvent(SelectedItemEventArgs selectedObjectArg)
        {
            if (this.ReportItemSelected != null)
            {
                this.ReportItemSelected(this, selectedObjectArg);
                
            }
        }
        
        public ReportItem GetReportItem()
        {
            if (this.ReportItem != null)
            {
                return this.ReportItem;
            }

            Syncfusion.RDL.DOM.TextBox textboxBase = new RDL.DOM.TextBox(); 
            textboxBase.Name = this.ItemName;
            textboxBase.KeepTogether = Convert.ToBoolean(this.Properties.KeepTogether);

            if (this.Properties.CanGrow == "True")
            {
                textboxBase.CanGrow = Convert.ToBoolean(this.Properties.CanGrow);
            }
            if (this.Properties.CanShrink == "True")
            {
                textboxBase.CanShrink = Convert.ToBoolean(this.Properties.CanShrink);
            }

            textboxBase.Height = new RDL.DOM.Size(this.ItemHeight / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Height));
            textboxBase.Width = new RDL.DOM.Size(this.ItemWidth / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Width));

            textboxBase.Left = new RDL.DOM.Size(this.ItemLeft / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Left));
            textboxBase.Top = new RDL.DOM.Size(this.ItemTop / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Top));

            if (!string.IsNullOrEmpty(this.Properties.DocumentMapLabel))
            {
                textboxBase.DocumentMapLabel = this.Properties.DocumentMapLabel;
            }
            if (this.ToolTip != null)
            {
                //textboxBase.ToolTip = this.ToolTip.ToString();
                textboxBase.ToolTip = this.Properties.ToolTip;
            }
            if (this.Action != null && (this.Action.BookmarkLink != null || this.Action.Drillthrough != null || this.Action.Hyperlink != null))
            {
                textboxBase.ActionInfo = new RDL.DOM.ActionInfo();
                textboxBase.ActionInfo.Actions = new RDL.DOM.Actions();
                textboxBase.ActionInfo.Actions.Clear();
                textboxBase.ActionInfo.Actions.Add(this.Action);
            }
 
            textboxBase.Style = new Syncfusion.RDL.DOM.Style();
            textboxBase.Visibility = new Syncfusion.RDL.DOM.Visibility();

            if (this.Properties.ToggleItem != null)
            {
                textboxBase.Visibility.ToggleItem = this.Properties.ToggleItem;
            }
            if (this.Properties.Hidden == "True" || this.Properties.Hidden.StartsWith("="))
            {
                textboxBase.Visibility.Hidden = this.Properties.Hidden;
            }
            if (this.Properties.InitialToggleState != "False")
            {
                textboxBase.ToggleImage = new RDL.DOM.ToggleImage();
                textboxBase.ToggleImage.InitialState = this.Properties.InitialToggleState;
            }
            if (this.Properties.WritingMode != "Default")
            {
                textboxBase.Style.WritingMode = this.Properties.WritingMode;
            }
            if (this.Properties.HideDuplicates != "None" && this.Properties.HideDuplicates != null)
            {
                textboxBase.HideDuplicates = this.Properties.HideDuplicates;
            }

            Syncfusion.RDL.DOM.Paragraphs paragraphsBase = new Paragraphs();

            // initial
            int paraCount=0;

            if (this.InnerTextBox == null)
            {
                this.InnerTextBox = new RichTextBox();
                this.InnerTextBox.Document = new FlowDocument();
            }
            paraCount = this.InnerTextBox.Document.Blocks.Count;


            if (this.TextLength == 0)
            {
                Run run = new Run("") { Foreground = Brushes.Black };
                            
                if (paraCount > 0)
                {
                    System.Windows.Documents.Paragraph para = this.InnerTextBox.Document.Blocks.First() as System.Windows.Documents.Paragraph;
                    if (para != null)
                    {
                        para.Inlines.Clear();
                        para.Inlines.Add(run);
                    }
                }
                else
                {
                    System.Windows.Documents.Paragraph para = new System.Windows.Documents.Paragraph();
                    para.Inlines.Add(run);
                    this.InnerTextBox.Document.Blocks.Add(para);
                }
            }

            foreach (System.Windows.Documents.Paragraph paragraph in this.InnerTextBox.Document.Blocks)
            {
                if (paragraph != null)
                {
                    Syncfusion.RDL.DOM.Paragraph paragraphBase = new Syncfusion.RDL.DOM.Paragraph();
                    paragraphBase.TextRuns = new TextRuns();
                    foreach (Run run in paragraph.Inlines)
                    {
                        if (run != null)
                        {
                            TextRun txtRun = getTextRun(run);
                            if (this.Properties.Format != null)
                            {
                                txtRun.Style.Format = this.Properties.Format;
                            }
                            paragraphBase.TextRuns.Add(txtRun);
                        }
                    }

                    paragraphBase.Style = new Syncfusion.RDL.DOM.Style();

                    if (this.Properties.ListLevel != 0)
                    {
                        paragraphBase.ListLevel = this.Properties.ListLevel;
                    }
                    if (this.Properties.ListStyle != RDL.DOM.ListStyle.None)
                    {
                        paragraphBase.ListStyle = this.Properties.ListStyle;
                    }
                    if (this.Properties.Indents.HangingIndent != null)
                        paragraphBase.HangingIndent = this.Properties.Indents.HangingIndent;

                    if(this.Properties.Indents.LeftIndent != null)
                        paragraphBase.LeftIndent = this.Properties.Indents.LeftIndent;

                    if (this.Properties.Indents.RightIndent != null)
                        paragraphBase.RightIndent = this.Properties.Indents.RightIndent;

                    if (this.Properties.SpaceAfter != null)
                        paragraphBase.SpaceAfter = this.Properties.SpaceAfter;

                    if (this.Properties.SpaceBefore != null)
                        paragraphBase.SpaceBefore = this.Properties.SpaceBefore;

                    // set the Text Align
                    if (this.Properties.HorizontalAlignment == "Right" || this.Properties.HorizontalAlignment == "Center")
                    {
                        paragraphBase.Style.TextAlign = this.Properties.HorizontalAlignment;
                    }

                    // set the Line Height                        
                    if (double.IsNaN(paragraph.LineHeight))
                    {
                        paragraphBase.Style.LineHeight = new Syncfusion.RDL.DOM.Size(10 + "pt");
                    }
                    else
                    {
                        paragraphBase.Style.LineHeight = new Syncfusion.RDL.DOM.Size(paragraph.LineHeight + "pt");
                    }

                    paragraphsBase.Add(paragraphBase);
                }
            }

            textboxBase.Paragraphs = paragraphsBase;

            // set the textbox style

            if (!string.IsNullOrEmpty(this.Properties.BackgroundImage.ImageValue))
            {
                textboxBase.Style.BackgroundImage = new RDL.DOM.BackgroundImage();
                textboxBase.Style.BackgroundImage.Value = this.Properties.BackgroundImage.ImageValue;
                textboxBase.Style.BackgroundImage.Source = this.Properties.BackgroundImage.Source;
                if (this.Properties.BackgroundImage.MIMEType != null)
                    textboxBase.Style.BackgroundImage.MIMEType = this.Properties.BackgroundImage.MIMEType;
            }
            if (this.Properties.VerticalAlignment == "Top" || this.Properties.VerticalAlignment == "Middle"
                || this.Properties.VerticalAlignment == "Bottom")
            {
                textboxBase.Style.VerticalAlign = this.Properties.VerticalAlignment;
            }

            textboxBase.Style.BackgroundColor = this.Properties.FillColor;

            if (this.Properties.Padding != null)
            {
                textboxBase.Style.PaddingLeft = new RDL.DOM.Size(this.Properties.Padding.PaddingLeft);
                textboxBase.Style.PaddingRight = new RDL.DOM.Size(this.Properties.Padding.PaddingRight);
                textboxBase.Style.PaddingTop = new RDL.DOM.Size(this.Properties.Padding.PaddingTop);
                textboxBase.Style.PaddingBottom = new RDL.DOM.Size(this.Properties.Padding.PaddingBottom);
            }
            
            textboxBase.Style.Border = new RDL.DOM.Border();
            textboxBase.Style.Border.Style = this.Properties.BorderStyles.DefaultBorderStyle;
            textboxBase.Style.Border.Width = this.Properties.BorderWidths.DefaultBorderWidth;
            textboxBase.Style.Border.Color = this.Properties.BorderColors.DefaultBorderColor;

            if (this.Properties.BorderWidths.LeftBorderWidth != null || this.Properties.BorderStyles.LeftBorderStyle != null
                || this.Properties.BorderColors.LeftBorderColor != null)
            {
                textboxBase.Style.LeftBorder = new RDL.DOM.LeftBorder();
                textboxBase.Style.LeftBorder.Color = this.Properties.BorderColors.LeftBorderColor;
                textboxBase.Style.LeftBorder.Width = this.Properties.BorderWidths.LeftBorderWidth;
                textboxBase.Style.LeftBorder.Style = this.Properties.BorderStyles.LeftBorderStyle;
            }
            if (this.Properties.BorderWidths.RightBorderWidth != null || this.Properties.BorderStyles.RightBorderStyle != null
                || this.Properties.BorderColors.RightBorderColor != null)
            {
                textboxBase.Style.RightBorder = new RDL.DOM.RightBorder();
                textboxBase.Style.RightBorder.Color = this.Properties.BorderColors.RightBorderColor;
                textboxBase.Style.RightBorder.Width = this.Properties.BorderWidths.RightBorderWidth;
                textboxBase.Style.RightBorder.Style = this.Properties.BorderStyles.RightBorderStyle;
            }
            if (this.Properties.BorderWidths.TopBorderWidth != null || this.Properties.BorderStyles.TopBorderStyle != null
                || this.Properties.BorderColors.TopBorderColor != null)
            {
                textboxBase.Style.TopBorder = new RDL.DOM.TopBorder();
                textboxBase.Style.TopBorder.Color = this.Properties.BorderColors.TopBorderColor;
                textboxBase.Style.TopBorder.Width = this.Properties.BorderWidths.TopBorderWidth;
                textboxBase.Style.TopBorder.Style = this.Properties.BorderStyles.TopBorderStyle;
            }
            if (this.Properties.BorderWidths.BottomBorderWidth != null || this.Properties.BorderStyles.BottomBorderStyle != null
                || this.Properties.BorderColors.BottomBorderColor != null)
            {
                textboxBase.Style.BottomBorder = new RDL.DOM.BottomBorder();
                textboxBase.Style.BottomBorder.Color = this.Properties.BorderColors.BottomBorderColor;
                textboxBase.Style.BottomBorder.Width = this.Properties.BorderWidths.BottomBorderWidth;
                textboxBase.Style.BottomBorder.Style = this.Properties.BorderStyles.BottomBorderStyle;
            }
            if (!string.IsNullOrEmpty(this.Properties.DataElementName))
            {
                textboxBase.DataElementName = this.Properties.DataElementName;
            }
            if (this.Properties.DataElementOutput != "Auto")
            {
                textboxBase.DataElementOutput = (RDL.DOM.DataElementOutputs)Enum.Parse(typeof(RDL.DOM.DataElementOutputs),this.Properties.DataElementOutput);
            }
            if (this.Properties.DataElementStyle != "Auto")
            {
                textboxBase.DataElementStyle = (RDL.DOM.DataElementStyle)Enum.Parse(typeof(RDL.DOM.DataElementStyle), this.Properties.DataElementStyle);
            }

            return textboxBase;
        }

        public void RestoreReportItem(RDL.DOM.ReportItem reportItem)
        {
            this.Properties.IsInternalPropertyChange = true;
            this.ReportItem = reportItem;

            if (this.ReportItem != null)
            {
                this.PopulateReportItem();
                MemoryStream stream = new MemoryStream();
                this.oldItem = this.GetReportItem();
                this.textSerializer.Serialize(stream, this.oldItem as RDL.DOM.TextBox);
                this.oldItemString = Encoding.UTF8.GetString(stream.GetBuffer());
                this.ReportItem = null;
            }

            this.Properties.IsInternalPropertyChange = false;
        }

        public void UpdateItemSizeProperties()
        {
            this.Properties.IsInternalPropertyChange = true;
            this.Properties.Width = propertyValueConvertor.GetSizeValue(this.ItemWidth, this.Properties.Width);
            this.Properties.Top = propertyValueConvertor.GetSizeValue(this.ItemTop, this.Properties.Top);
            this.Properties.Left = propertyValueConvertor.GetSizeValue(this.ItemLeft, this.Properties.Left);
            this.Properties.Height = propertyValueConvertor.GetSizeValue(this.ItemHeight, this.Properties.Height); 
            this.Properties.IsInternalPropertyChange = false;
        }

        #endregion        

        #region Helper methods

        public ImageSource GetImageSource()
        {
            this.Arrange(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            this.UpdateLayout();
            System.Windows.Media.Imaging.RenderTargetBitmap rtb = new System.Windows.Media.Imaging.RenderTargetBitmap((int)this.ActualWidth,(int) this.ActualHeight, 96, 96, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(this);
                ctx.DrawRectangle(vb, null, new Rect(new Point(),new System.Windows.Size(this.ActualWidth,this.ActualHeight)));
            }

            rtb.Render(dv);
            return rtb;
        }

        private int getTextLength()
        {
            if (this.InnerTextBox != null)
            {
                TextPointer start = this.InnerTextBox.Document.ContentStart;
                TextPointer end = this.InnerTextBox.Document.ContentEnd;
                TextRange t = new TextRange(start, end);
                if (t != null)
                    return (t.Text.Trim()).Length;
            }
            return 0;
        }

        public void PopulateReportItem()
        {
            Syncfusion.RDL.DOM.TextBox textBox = this.ReportItem as RDL.DOM.TextBox;

            if (textBox != null)
            {
                if (this.InnerTextBox == null)
                {
                    this.InnerTextBox = new RichTextBox();
                }

                if (this.document == null)
                {
                    document = new FlowDocument();
                    this.InnerTextBox.Document = document;
                }

                this.InnerTextBox.Document.Blocks.Clear();

                if (textBox.Paragraphs != null)
                {
                    foreach (Syncfusion.RDL.DOM.Paragraph paragraphBase in textBox.Paragraphs)
                    {
                        System.Windows.Documents.Paragraph paragraphRichTextbox = new System.Windows.Documents.Paragraph();
                        paragraphRichTextbox.Foreground = Brushes.Black;

                        if (paragraphBase.TextRuns != null)
                        {
                            foreach (Syncfusion.RDL.DOM.TextRun textRun in paragraphBase.TextRuns)
                            {
                                if (textRun.Value != null)
                                {
                                    paragraphRichTextbox.Inlines.Add(getRunFromTextRun(textRun));
                                }
                            }
                        }
                        if (paragraphBase.HangingIndent != null)
                            this.Properties.Indents.HangingIndent = paragraphBase.HangingIndent;

                        if (paragraphBase.LeftIndent != null)
                            this.Properties.Indents.LeftIndent = paragraphBase.LeftIndent;

                        if (paragraphBase.RightIndent != null)
                            this.Properties.Indents.RightIndent = paragraphBase.RightIndent;

                        if (paragraphBase.SpaceAfter != null)
                            this.Properties.SpaceAfter = paragraphBase.SpaceAfter.size;

                        if (paragraphBase.SpaceBefore != null)
                            this.Properties.SpaceBefore = paragraphBase.SpaceBefore.size;

                        if (paragraphBase.Style != null)
                        {
                            switch (paragraphBase.Style.TextAlign)
                            {
                                case "Left":
                                    this.Properties.HorizontalAlignment = "Left";
                                    break;
                                case "Right":
                                    this.Properties.HorizontalAlignment = "Right";
                                    break;
                                case "Center":
                                    this.Properties.HorizontalAlignment = "Center";
                                    break;
                                default:
                                    this.Properties.HorizontalAlignment = "Left";
                                    break;
                            }

                            paragraphRichTextbox.TextAlignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), this.Properties.HorizontalAlignment);
                            this.Properties.ListLevel = paragraphBase.ListLevel;
                            this.Properties.ListStyle = paragraphBase.ListStyle;

                            if (paragraphBase.Style.LineHeight != null && paragraphBase.Style.LineHeight.size != null)
                            {
                                this.Properties.LineSpacing = paragraphBase.Style.LineHeight.size;
                            }
                        }

                        this.document.Blocks.Add(paragraphRichTextbox);
                    }

                    if (textBox.Style != null)
                    {
                        if (textBox.Style.BackgroundColor != null)
                        {
                            this.Properties.FillColor = textBox.Style.BackgroundColor;
                        }

                        string leftPadding = "1pt";
                        string topPadding = "1pt";
                        string rightPadding = "1pt";
                        string bottomPadding = "1pt";

                        if (textBox.Style.PaddingLeft != null && textBox.Style.PaddingLeft.size != null)
                        {
                            leftPadding = textBox.Style.PaddingLeft.size;
                        }
                        if (textBox.Style.PaddingRight != null && textBox.Style.PaddingRight.size != null)
                        {
                            rightPadding = textBox.Style.PaddingRight.size;
                        }
                        if (textBox.Style.PaddingTop != null && textBox.Style.PaddingTop.size != null)
                        {
                            topPadding = textBox.Style.PaddingTop.size;
                        }
                        if (textBox.Style.PaddingBottom != null && textBox.Style.PaddingBottom.size != null)
                        {
                            bottomPadding = textBox.Style.PaddingBottom.size;
                        }

                        this.Properties.Padding.PaddingLeft = leftPadding;
                        this.Properties.Padding.PaddingRight = rightPadding;
                        this.Properties.Padding.PaddingTop = topPadding;
                        this.Properties.Padding.PaddingBottom = bottomPadding;

                        if (textBox.Style.BackgroundImage != null)
                        {
                            this.Properties.BackgroundImage.Source = textBox.Style.BackgroundImage.Source;
                            this.Properties.BackgroundImage.ImageValue = textBox.Style.BackgroundImage.Value;
                            if(textBox.Style.BackgroundImage.MIMEType != null)
                                this.Properties.BackgroundImage.MIMEType = textBox.Style.BackgroundImage.MIMEType;
                        }
                        if (textBox.Style.Border != null)
                        {
                            if (textBox.Style.Border.Style != null)
                                this.Properties.BorderStyles.DefaultBorderStyle = textBox.Style.Border.Style;
                            if (textBox.Style.Border.Color != null)
                                this.Properties.BorderColors.DefaultBorderColor = textBox.Style.Border.Color;
                            if (textBox.Style.Border.Width != null)
                                this.Properties.BorderWidths.DefaultBorderWidth = textBox.Style.Border.Width.size;
                        }
                        if (textBox.Style.LeftBorder != null)
                        {
                            if (textBox.Style.LeftBorder.Color != null)
                                this.Properties.BorderColors.LeftBorderColor = textBox.Style.LeftBorder.Color;
                            if (textBox.Style.LeftBorder.Width != null)
                                this.Properties.BorderWidths.LeftBorderWidth = textBox.Style.LeftBorder.Width.size;
                            if (textBox.Style.LeftBorder.Style != null)
                                this.Properties.BorderStyles.LeftBorderStyle = textBox.Style.LeftBorder.Style;
                        }
                        if (textBox.Style.RightBorder != null)
                        {
                            if (textBox.Style.RightBorder.Color != null)
                                this.Properties.BorderColors.RightBorderColor = textBox.Style.RightBorder.Color;
                            if (textBox.Style.RightBorder.Width != null)
                                this.Properties.BorderWidths.RightBorderWidth = textBox.Style.RightBorder.Width.size;
                            if (textBox.Style.RightBorder.Style != null)
                                this.Properties.BorderStyles.RightBorderStyle = textBox.Style.RightBorder.Style;
                        }
                        if (textBox.Style.TopBorder != null)
                        {
                            if (textBox.Style.TopBorder.Color != null)
                                this.Properties.BorderColors.TopBorderColor = textBox.Style.TopBorder.Color;
                            if (textBox.Style.TopBorder.Width != null)
                                this.Properties.BorderWidths.TopBorderWidth = textBox.Style.TopBorder.Width.size;
                            if (textBox.Style.TopBorder.Style != null)
                                this.Properties.BorderStyles.TopBorderStyle = textBox.Style.TopBorder.Style;
                        }
                        if (textBox.Style.BottomBorder != null)
                        {
                            if (textBox.Style.BottomBorder.Color != null)
                                this.Properties.BorderColors.BottomBorderColor = textBox.Style.BottomBorder.Color;
                            if (textBox.Style.BottomBorder.Width != null)
                                this.Properties.BorderWidths.BottomBorderWidth = textBox.Style.BottomBorder.Width.size;
                            if (textBox.Style.BottomBorder.Style != null)
                                this.Properties.BorderStyles.BottomBorderStyle = textBox.Style.BottomBorder.Style;
                        }
                        if (textBox.Style.WritingMode != null && textBox.Style.WritingMode != "Default")
                        {
                            this.Properties.WritingMode = textBox.Style.WritingMode;
                        }
                        if (textBox.Style.VerticalAlign != null && textBox.Style.VerticalAlign != "Default")
                        {
                            this.Properties.VerticalAlignment = textBox.Style.VerticalAlign;
                        }
                    }

                    if (textBox.Height != null)
                    {
                        this.Properties.Height = textBox.Height.size;
                    }
                    if (textBox.Width != null)
                    {
                        this.Properties.Width = textBox.Width.size;
                    } 
                    if (textBox.Top != null)
                    {
                        this.Properties.Top = textBox.Top.size;
                    }
                    if (textBox.Left != null)
                    {
                        this.Properties.Left = textBox.Left.size;
                    }
                    if (textBox.Visibility != null)
                    {
                        if (textBox.Visibility.Hidden != null)
                        {
                            this.Properties.Hidden = textBox.Visibility.Hidden;
                        }
                        if (textBox.Visibility.ToggleItem != null)
                        {
                            this.Properties.ToggleItem = textBox.Visibility.ToggleItem;
                        }
                    }
                    if (textBox.ActionInfo != null)
                    {
                        this.Action = textBox.ActionInfo.Actions.FirstOrDefault();
                    }
                    if (textBox.DataElementOutput != RDL.DOM.DataElementOutputs.Auto)
                    {
                        this.Properties.DataElementOutput = textBox.DataElementOutput.ToString();
                    }
                    if (textBox.DataElementStyle != RDL.DOM.DataElementStyle.Auto)
                    {
                        this.Properties.DataElementStyle = textBox.DataElementStyle.ToString();
                    }
                    if (textBox.ToggleImage != null)
                    {
                        this.Properties.InitialToggleState = textBox.ToggleImage.InitialState;
                    }
                    if (textBox.HideDuplicates != null && textBox.HideDuplicates != "None")
                    {
                        textBox.HideDuplicates = this.Properties.HideDuplicates;
                    }

                    this.Properties.DataElementName = textBox.DataElementName;
                    this.Properties.KeepTogether = textBox.KeepTogether.ToString();
                    this.Properties.CanGrow = textBox.CanGrow.ToString();
                    this.Properties.CanShrink = textBox.CanShrink.ToString();
                    this.Properties.DocumentMapLabel = textBox.DocumentMapLabel;
                }
            }
        }

        internal TextRun getTextRun(Run run)
        {
            string runText = run.Text;
            TextRun textRun = new TextRun();

            // save the place holder Label and field values.
            if (runText.StartsWith("[") && runText.EndsWith("]"))
            {
                if (runText.StartsWith("[&") && runText.EndsWith("]"))
                {
                    textRun.InternalLabel = runText;
                    string value = GetBuiltInFunctionsToRDL(runText);
                    if (value != null && value != string.Empty)
                    {
                        run.Tag = value;
                        textRun.Value = value;
                    }
                }
                else if (runText.StartsWith("[@") && runText.EndsWith("]"))
                {
                    textRun.InternalLabel = runText;
                    String modifiedText = runText.Replace("[@", "").Replace("]", "");
                    textRun.Value = "=Parameters!" + modifiedText + ".Value";
                }
                else
                {
                    textRun.InternalLabel = runText;
                    if (run.Tag != null && run.Tag is string)
                    {
                        textRun.Value = run.Tag as string;
                    }
                }
            }
            else if (runText.Equals("<<Expr>>"))
            {
                textRun.InternalLabel = runText;
                if (run.Tag != null && run.Tag is string)
                {
                    textRun.Value = run.Tag as string;
                }
            }
            else // save the normal text in the field "Value"
            {
                textRun.Value = runText;
            }

            textRun.Style = new Syncfusion.RDL.DOM.Style();

            // set the Font Name
            if (this.Properties.FontFamily.StartsWith("="))
            {
                textRun.Style.FontFamily = this.Properties.FontFamily;
            }
            else
            {
                textRun.Style.FontFamily = run.FontFamily.ToString();
            }

            // set the Font size
            if (this.Properties.FontSize.StartsWith("="))
            {
                textRun.Style.FontSize = this.Properties.FontSize;
            }
            else
            {
                const double PointValue = 1.333333333;
                textRun.Style.FontSize = new Syncfusion.RDL.DOM.Size((run.FontSize / PointValue) + "pt");
            }

            // set the Font weight
            if (this.Properties.FontWeight.StartsWith("="))
            {
                textRun.Style.FontWeight = this.Properties.FontWeight;
            }
            else
            {
                textRun.Style.FontWeight = run.FontWeight.ToString();
            }

            // set the Font style
            if (this.Properties.FontStyle.StartsWith("="))
            {
                textRun.Style.FontStyle = this.Properties.FontStyle;
            }
            else
            {
                textRun.Style.FontStyle = run.FontStyle.ToString();
            }

            // set the TextDecoration
            if (run.TextDecorations != null && run.TextDecorations.FirstOrDefault() != null)
            {
                if (run.TextDecorations.First().Location ==  TextDecorationLocation.Strikethrough)
                {
                    textRun.Style.TextDecoration = "LineThrough";
                }
                else
                {
                    textRun.Style.TextDecoration = Enum.Parse(typeof(Syncfusion.RDL.DOM.TextDecoration), run.TextDecorations.First().Location.ToString(), true).ToString();
                }
            }
            else
            {
                if (this.Properties.FontEffects != null && this.Properties.FontEffects.ToLower() == "strikethrough")
                {
                    textRun.Style.TextDecoration = "LineThrough";
                }
                else
                {
                    textRun.Style.TextDecoration = Enum.Parse(typeof(Syncfusion.RDL.DOM.TextDecoration), this.Properties.FontEffects, true).ToString();
                }
            }
            // set the Foreground color
            if (this.Properties.FontColor.StartsWith("="))
            {
                textRun.Style.Color = this.Properties.FontColor;
            }
            else
            {
                textRun.Style.Color = run.Foreground.ToString();
            }

            //set the text format
            textRun.Style.Format = this.Properties.Format;

            // set the background color
            if (run.Background != null && run.Background != Brushes.Transparent)
                textRun.Style.BackgroundColor = run.Background.ToString();

            return textRun;
        }

        internal static string GetBuiltInFunctionsToRDL(string richTextBoxContent)
        {
            string modifiedText = "";

            if (richTextBoxContent.StartsWith("[&") && richTextBoxContent.EndsWith("]"))
            {
                modifiedText = richTextBoxContent;
                modifiedText = modifiedText.Replace("[&", "").Replace("]", "");

                if (modifiedText == "ExecutionTime" || modifiedText == "ReportFolder"
                    || modifiedText == "ReportName" || modifiedText == "ReportServerUrl"
                    || modifiedText == "PageNumber" || modifiedText == "TotalPages")
                {
                    modifiedText = "=Globals!" + modifiedText;
                }
                else if (modifiedText == "UserID" || modifiedText == "Language")
                {
                    modifiedText = "=User!" + modifiedText;
                }
                else
                {
                    modifiedText = richTextBoxContent;
                }
            }
            else
            {
                modifiedText = richTextBoxContent;
            }

            return modifiedText;
        }

        #endregion        

        #region OverrridenMethods

        public override void OnApplyTemplate()
        {
            this.IsHitTestVisible = true;

            this.InnerTextBox = GetTemplateChild("PART_InnerTextBox") as RichTextBox; // TextBox;
            this.InnerBorder = GetTemplateChild("PART_InnerBorder") as DesignerDashStyleBorder;
            document = GetTemplateChild("TextBox_FlowDocument") as System.Windows.Documents.FlowDocument;

            this.InnerTextBox.IsHitTestVisible = true;
            paragraph = GetTemplateChild("Part_Paragraph") as System.Windows.Documents.Paragraph;
            this.InnerTextBox.IsHitTestVisible = true;
            this.InnerTextBox.Foreground = Brushes.Black;
            this.InnerTextBox.BorderBrush = Brushes.LightGray;
            this.InnerTextBox.BorderThickness = new Thickness(1);
            //this.InnerTextBox.AllowDrop = true;
            this.InnerTextBox.PreviewMouseRightButtonDown += new MouseButtonEventHandler(InnerTextBox_PreviewMouseRightButtonDown);
            this.InnerTextBox.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(InnerTextBox_PreviewKeyDown);
            this.InnerTextBox.PreviewGiveFeedback += new GiveFeedbackEventHandler(InnerTextBox_PreviewGiveFeedback);
            this.PreviewGiveFeedback += new GiveFeedbackEventHandler(TextBoxControl_PreviewGiveFeedback);
            this.InnerTextBox.PreviewDrop += new DragEventHandler(InnerTextBox_PreviewDrop);
            //this.PreviewDrop += new DragEventHandler(TextBoxControl_PreviewDrop);
            this.InnerTextBox.PreviewDragOver += new DragEventHandler(InnerTextBox_PreviewDragOver);
            this.InnerTextBox.PreviewDragEnter += new DragEventHandler(InnerTextBox_PreviewDragEnter);
            document.PreviewMouseDown += new MouseButtonEventHandler(document_PreviewMouseDown);
            document.PreviewGiveFeedback += new GiveFeedbackEventHandler(document_PreviewGiveFeedback);
            this.InnerTextBox.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(InnerTextBox_PreviewLostKeyboardFocus);
            this.InnerTextBox.GotFocus += new RoutedEventHandler(InnerTextBox_GotFocus);
            this.InnerTextBox.LostFocus += new RoutedEventHandler(InnerTextBox_LostFocus);
            // Binding the commands copy,paste and cut 
            copyBinding = new CommandBinding(ApplicationCommands.Copy);
            pasteBinding = new CommandBinding(ApplicationCommands.Paste);
            cutBinding = new CommandBinding(ApplicationCommands.Cut);
            copyBinding.Executed += new ExecutedRoutedEventHandler(copyBinding_Executed);
            pasteBinding.Executed += new ExecutedRoutedEventHandler(pasteBinding_Executed);
            cutBinding.Executed += new ExecutedRoutedEventHandler(cutBinding_Executed);
            
            InnerTextBox.CommandBindings.Add(copyBinding);
            InnerTextBox.CommandBindings.Add(pasteBinding);
            InnerTextBox.CommandBindings.Add(cutBinding);

            this.TextBoxProperties = GetTemplateChild("PART_TextBoxProperty") as MenuItem;
            this.TextProperties = GetTemplateChild("PART_TextProperty") as MenuItem;
            this.CreatePlaceHolder = GetTemplateChild("PART_CreatePlaceHolder") as MenuItem;
            this.Expression = GetTemplateChild("PART_Expression") as MenuItem;
            this.Cut = GetTemplateChild("PART_Cut") as MenuItem;
            this.Copy = GetTemplateChild("PART_Copy") as MenuItem;
            this.Paste = GetTemplateChild("PART_Paste") as MenuItem;
            Cut.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelCut"); ;
            Copy.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelCopy"); ;
            Paste.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelPaste"); ;
            TextBoxProperties.Header = SR.GetString(CultureInfo.CurrentUICulture, "titleTextBoxProperties");
            TextProperties.Header = SR.GetString(CultureInfo.CurrentUICulture, "titleTextProperties");
            CreatePlaceHolder.Header = SR.GetString(CultureInfo.CurrentUICulture, "titlePlaceHolderProperties");


            if (this.TextBoxProperties != null)
            {
                this.TextBoxProperties.Click += new System.Windows.RoutedEventHandler(TextBoxProperties_Click);
            }
            
            if (this.TextProperties != null)
            {
                this.TextProperties.Click += new RoutedEventHandler(TextProperties_Click);
            }

            if (this.Expression != null)
            {
                this.Expression.Click += new RoutedEventHandler(Expression_Click);
            }

            if (this.Cut != null)
            {
                this.Cut.Click += new RoutedEventHandler(Cut_Click);
            }
            
            if (this.Copy != null)
            {
                this.Copy.Click += new RoutedEventHandler(Copy_Click);
            }
            
            if (this.Paste != null)
            {
                this.Paste.Click += new RoutedEventHandler(Paste_Click);
            }

            base.OnApplyTemplate();

            this.Properties.IsInternalPropertyChange = true;

            this.Properties.Name = this.ItemName;
            this.Properties.UpdatePropertyValue();

            if (this.ReportItem != null)
            {
                this.PopulateReportItem();
                this.ReportItem = null;
            }

            this.Properties.IsInternalPropertyChange = false;
        }
        
        void InnerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Properties.BackgroundImage.ImageValue))
            {
                UpdateBackgroundImage();
            }

            MemoryStream stream = new MemoryStream();
            textSerializer.Serialize(stream, this.GetReportItem() as RDL.DOM.TextBox);
            string textValue = Encoding.UTF8.GetString(stream.GetBuffer());

            if (!oldItemString.Equals(textValue) && this.Panel!=null && !this.Panel.IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.oldItem;
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);
                change.NewValue = this.GetReportItem();
            }
        }

        void InnerTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Properties.BackgroundImage.ImageValue))
            {
                this.InnerTextBox.Background = this.propertyValueConvertor.GetBackGroundColor(this.Properties.FillColor);
            }

            MemoryStream stream = new MemoryStream();
            this.oldItem = this.GetReportItem();
            textSerializer.Serialize(stream, this.oldItem as RDL.DOM.TextBox);
            oldItemString = Encoding.UTF8.GetString(stream.GetBuffer());
        }

        #endregion

        #region Events

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        void Properties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "HORIZONTALALIGNMENT":
                    {
                        propertyValue = this.Properties.HorizontalAlignment;
                        break;
                    }
                case "FONTFAMILY":
                    {
                        propertyValue = this.Properties.FontFamily;
                        break;
                    }
                case "FONTSIZE":
                    {
                        propertyValue = this.Properties.FontSize;
                        break;
                    }
                case "FONTSTYLE":
                    {
                        propertyValue = this.Properties.FontStyle;
                        break;
                    }
                case "FONTWEIGHT":
                    {
                        propertyValue = this.Properties.FontWeight;
                        break;
                    }
                case "FONTEFFECTS":
                    {
                        propertyValue = this.Properties.FontEffects;
                        break;
                    }
                case "FILLCOLOR":
                    {
                        propertyValue = this.Properties.FillColor;
                        break;
                    }
                case "FONTCOLOR":
                    {
                        propertyValue = this.Properties.FontColor;
                        break;
                    }
                case "PADDINGLEFT":
                    {
                        propertyValue = this.Properties.Padding.PaddingLeft;
                        break;
                    }
                case "PADDINGRIGHT":
                    {
                        propertyValue = this.Properties.Padding.PaddingRight;
                        break;
                    }
                case "PADDINGTOP":
                    {
                        propertyValue = this.Properties.Padding.PaddingTop;
                        break;
                    }
                case "PADDINGBOTTOM":
                    {
                        propertyValue = this.Properties.Padding.PaddingBottom;
                        break;
                    }
                case "DEFAULTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LEFTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RIGHTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TOPBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BOTTOMBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DEFAULTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LEFTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RIGHTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TOPBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BOTTOMBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DEFAULTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LEFTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RIGHTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TOPBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BOTTOMBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "NAME":
                    {
                        propertyValue = this.Properties.Name;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.Properties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.Properties.ToggleItem;
                        break;
                    }
                case "IMAGEVALUE":
                    {
                        propertyValue = this.Properties.BackgroundImage.ImageValue;
                        break;
                    }
                case "SOURCE":
                    {
                        propertyValue = this.Properties.BackgroundImage.Source;
                        break;
                    }
                case "MIMETYPE":
                    {
                        propertyValue = this.Properties.BackgroundImage.MIMEType;
                        break;
                    }
                case "BACKGROUNDREPEAT":
                    {
                        propertyValue = this.Properties.BackgroundImage.MIMEType;
                        break;
                    }
                case "KEEPTOGETHER":
                    {
                        propertyValue = this.Properties.KeepTogether;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.Properties.DocumentMapLabel;
                        break;
                    }
                case "LISTLEVEL":
                    {
                        propertyValue = this.Properties.ListLevel;
                        break;
                    }
                case "LISTSTYLE":
                    {
                        propertyValue = this.Properties.ListStyle;
                        break;
                    }
                case "CANGROW":
                    {
                        propertyValue = this.Properties.CanGrow;
                        break;
                    }
                case "CANSHRINK":
                    {
                        propertyValue = this.Properties.CanShrink;
                        break;
                    }
                case "FORMAT":
                    {
                        propertyValue = this.Properties.Format;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.Properties.Left;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.Properties.Top;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.Properties.Height;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.Properties.Width;
                        break;
                    }
                case "SPACEBEFORE":
                    {
                        propertyValue = this.Properties.SpaceBefore;
                        break;
                    }
                case "SPACEAFTER":
                    {
                        propertyValue = this.Properties.SpaceAfter;
                        break;
                    }
                case "HANGINGINDENT":
                    {
                        propertyValue = this.Properties.Indents.HangingIndent;
                        break;
                    }
                case "LEFTINDENT":
                    {
                        propertyValue = this.Properties.Indents.LeftIndent;
                        break;
                    }
                case "RIGHTINDENT":
                    {
                        propertyValue = this.Properties.Indents.RightIndent;
                        break;
                    }
                case "DATAELEMENTNAME":
                    {
                        propertyValue = this.Properties.DataElementName;
                        break;
                    }
                case "DATAELEMENTOUTPUT":
                    {
                        propertyValue = this.Properties.DataElementOutput;
                        break;
                    }
                case "DATAELEMENTSTYLE":
                    {
                        propertyValue = this.Properties.DataElementStyle;
                        break;
                    }
                case "INITIALTOGGLESTATE":
                    {
                        propertyValue = this.Properties.InitialToggleState;
                        break;
                    }
                case "HIDEDUPLICATES":
                    {
                        propertyValue = this.Properties.HideDuplicates;
                        break;
                    }
                case "WRITINGMODE":
                    {
                        propertyValue = this.Properties.WritingMode;
                        break;
                    }
            }

            this.propertyOldValue = propertyValue;
        }

        void Properties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName.ToUpper();
            object propertyValue = null;

            switch (propertyName)
            {
                case "HORIZONTALALIGNMENT":
                    {
                        propertyValue = this.Properties.HorizontalAlignment;
                        this.InnerTextBox.Document.TextAlignment = this.propertyValueConvertor.GetTextAlign(this.Properties.HorizontalAlignment);
                        break;
                    }
                case "VERTICALALIGNMENT":
                    {
                        propertyValue = this.Properties.VerticalAlignment;
                        break;
                    }
                case "FONTFAMILY":
                    {
                        propertyValue = this.Properties.FontFamily;
                        this.InnerTextBox.SelectAll();
                        this.InnerTextBox.Selection.ApplyPropertyValue(System.Windows.Controls.TextBox.FontFamilyProperty, new FontFamily(this.propertyValueConvertor.GetFontFamily(this.Properties.FontFamily)));
                        //this.InnerTextBox.FontFamily = new FontFamily(this.propertyValueConvertor.GetFontFamily(this.Properties.FontFamily));
                        break;
                    }
                case "FONTSIZE":
                    {
                        propertyValue = this.Properties.FontSize;
                        this.InnerTextBox.SelectAll();
                        this.InnerTextBox.Selection.ApplyPropertyValue(System.Windows.Controls.TextBox.FontSizeProperty, this.propertyValueConvertor.GetFontWeight(this.Properties.FontSize));
                        //this.InnerTextBox.FontSize = this.propertyValueConvertor.GetFontWeight(this.Properties.FontSize);
                        break;
                    }
                case "FONTSTYLE":
                    {
                        propertyValue = this.Properties.FontStyle;
                        this.InnerTextBox.SelectAll();
                        this.InnerTextBox.Selection.ApplyPropertyValue(System.Windows.Controls.TextBox.FontStyleProperty, this.propertyValueConvertor.GetFontStyle(this.Properties.FontStyle));
                        //this.InnerTextBox.Document.FontStyle = this.propertyValueConvertor.GetFontStyle(this.Properties.FontStyle);
                        break;
                    }
                case "FONTEFFECTS":
                    {
                        propertyValue = this.Properties.FontEffects;
                        this.paragraph.TextDecorations = this.propertyValueConvertor.GetTextDecoration(this.Properties.FontEffects);

                        if (this.Properties.FontEffects != null)
                        {
                            this.InnerTextBox.SelectAll();
                            this.InnerTextBox.Selection.ApplyPropertyValue(System.Windows.Controls.TextBox.TextDecorationsProperty, this.propertyValueConvertor.GetTextDecoration(this.Properties.FontEffects));
                        }
                        break;
                    }
                case "FILLCOLOR":
                    {
                        propertyValue = this.Properties.FillColor;
                        if (string.IsNullOrEmpty(this.Properties.BackgroundImage.ImageValue))
                        {
                            this.InnerTextBox.Background = this.propertyValueConvertor.GetBackGroundColor(this.Properties.FillColor);
                        }
                        break;
                    }
                case "FONTCOLOR":
                    {
                        propertyValue = this.Properties.FontColor;
                        this.InnerTextBox.Foreground = this.propertyValueConvertor.GetColor(this.Properties.FontColor);

                        if (this.Properties.FontColor != null)
                        {
                            this.InnerTextBox.SelectAll();
                            this.InnerTextBox.Selection.ApplyPropertyValue(TextBoxControl.ForegroundProperty, this.propertyValueConvertor.GetColor(this.Properties.FontColor));
                        }
                        break;
                    }
                case "FONTWEIGHT":
                    {
                        propertyValue = this.Properties.FontWeight;
                        if (!this.Properties.FontWeight.Equals("Default") && !this.Properties.FontWeight.StartsWith("="))
                            this.InnerTextBox.FontWeight = (System.Windows.FontWeight)new FontWeightConverter().ConvertFromInvariantString(this.Properties.FontWeight);
                        break;
                    }
                case "PADDINGLEFT":
                    {
                        propertyValue = this.Properties.Padding.PaddingLeft;
                        this.UpdatePaddingValue();
                        break;
                    }
                case "PADDINGRIGHT":
                    {
                        propertyValue = this.Properties.Padding.PaddingRight;
                        this.UpdatePaddingValue();
                        break;
                    }
                case "PADDINGTOP":
                    {
                        propertyValue = this.Properties.Padding.PaddingTop;
                        this.UpdatePaddingValue();
                        break;
                    }
                case "PADDINGBOTTOM":
                    {
                        propertyValue = this.Properties.Padding.PaddingBottom;
                        this.UpdatePaddingValue();
                        break;
                    }
                case "DEFAULTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.DefaultBorderStyle;
                        this.InnerBorder.DashStyle = this.propertyValueConvertor.GetBorderStyle(this.Properties.BorderStyles.DefaultBorderStyle);
                        break;
                    }
                case "LEFTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RIGHTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TOPBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BOTTOMBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DEFAULTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.DefaultBorderWidth;
                        this.InnerBorder.BorderThickness = this.propertyValueConvertor.GetBorderThickness(this.Properties.BorderWidths.DefaultBorderWidth);
                        break;
                    }
                case "LEFTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RIGHTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TOPBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BOTTOMBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DEFAULTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.DefaultBorderColor;
                        this.InnerBorder.BorderBrush = this.propertyValueConvertor.GetBackGroundColor(this.Properties.BorderColors.DefaultBorderColor);
                        break;
                    }
                case "LEFTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RIGHTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TOPBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BOTTOMBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "NAME":
                    {
                        propertyValue = this.Properties.Name;
                        this.ItemName = this.Properties.Name;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.Properties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.Properties.ToggleItem;
                        break;
                    }
                case "IMAGEVALUE":
                    {
                        this.UpdateBackgroundImage();
                        propertyValue = this.Properties.BackgroundImage.ImageValue;
                        break;
                    }
                case "SOURCE":
                    {
                        this.UpdateBackgroundImage();
                        propertyValue = this.Properties.BackgroundImage.Source;
                        break;
                    }
                case "MIMETYPE":
                    {
                        propertyValue = this.Properties.BackgroundImage.MIMEType;
                        break;
                    }
                case "BACKGROUNDREPEAT":
                    {
                        propertyValue = this.Properties.BackgroundImage.MIMEType;
                        break;
                    }
                case "KEEPTOGETHER":
                    {
                        propertyValue = this.Properties.KeepTogether;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.Properties.DocumentMapLabel;
                        break;
                    }
                case "LISTLEVEL":
                    {
                        propertyValue = this.Properties.ListLevel;
                        break;
                    }
                case "LISTSTYLE":
                    {
                        propertyValue = this.Properties.ListStyle;
                        break;
                    }
                case "CANGROW":
                    {
                        propertyValue = this.Properties.CanGrow;
                        break;
                    }
                case "CANSHRINK":
                    {
                        propertyValue = this.Properties.CanShrink;
                        break;
                    }
                case "FORMAT":
                    {
                        propertyValue = this.Properties.Format;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.Properties.Left;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Left))
                            this.ItemLeft = new RDL.DOM.Size(this.Properties.Left).PixelValue;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.Properties.Top;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Top))
                            this.ItemTop = new RDL.DOM.Size(this.Properties.Top).PixelValue;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.Properties.Height;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Height))
                            this.ItemHeight = new RDL.DOM.Size(this.Properties.Height).PixelValue;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.Properties.Width;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Width))
                            this.ItemWidth = new RDL.DOM.Size(this.Properties.Width).PixelValue;
                        break;
                    }
                case "SPACEBEFORE":
                    {
                        propertyValue = this.Properties.SpaceBefore;
                        break;
                    }
                case "SPACEAFTER":
                    {
                        propertyValue = this.Properties.SpaceAfter;
                        break;
                    }
                case "HANGINGINDENT":
                    {
                        propertyValue = this.Properties.Indents.HangingIndent;
                        break;
                    }
                case "LEFTINDENT":
                    {
                        propertyValue = this.Properties.Indents.LeftIndent;
                        break;
                    }
                case "RIGHTINDENT":
                    {
                        propertyValue = this.Properties.Indents.RightIndent;
                        break;
                    }
                case "DATAELEMENTNAME":
                    {
                        propertyValue = this.Properties.DataElementName;
                        break;
                    }
                case "DATAELEMENTOUTPUT":
                    {
                        propertyValue = this.Properties.DataElementOutput;
                        break;
                    }
                case "DATAELEMENTSTYLE":
                    {
                        propertyValue = this.Properties.DataElementStyle;
                        break;
                    }
                case "INITIALTOGGLESTATE":
                    {
                        propertyValue = this.Properties.InitialToggleState;
                        break;
                    }
                case "HIDEDUPLICATES":
                    {
                        propertyValue = this.Properties.HideDuplicates;
                        break;
                    }
                case "WRITINGMODE":
                    {
                        propertyValue = this.Properties.WritingMode;
                        break;
                    }
            }

            if (!this.Properties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();

                switch (propertyName)
                {
                    case "PADDINGLEFT":
                    case "PADDINGRIGHT":
                    case "PADDINGTOP":
                    case "PADDINGBOTTOM":
                        change.PropertyObject = this.Properties.Padding;
                        break;

                    case "DEFAULTBORDERSTYLE":
                    case "LEFTBORDERSTYLE":
                    case "RIGHTBORDERSTYLE":
                    case "TOPBORDERSTYLE":
                    case "BOTTOMBORDERSTYLE":
                        change.PropertyObject = this.Properties.BorderStyles;
                        break;

                    case "DEFAULTBORDERCOLOR":
                    case "LEFTBORDERCOLOR":
                    case "RIGHTBORDERCOLOR":
                    case "TOPBORDERCOLOR":
                    case "BOTTOMBORDERCOLOR":
                        change.PropertyObject = this.Properties.BorderColors;
                        break;

                    case "DEFAULTBORDERWIDTH":
                    case "LEFTBORDERWIDTH":
                    case "RIGHTBORDERWIDTH":
                    case "TOPBORDERWIDTH":
                    case "BOTTOMBORDERWIDTH":
                        change.PropertyObject = this.Properties.BorderWidths;
                        break;

                    case "HANGINGINDENT":
                    case "LEFTINDENT":
                    case "RIGHTINDENT":
                        change.PropertyObject = this.Properties.Indents;
                        break;

                    case "SOURCE":
                    case "MIMETYPE":
                    case "IMAGEVALUE":
                        change.PropertyObject = this.Properties.BackgroundImage;
                        break;

                    default:
                        change.PropertyObject = this.Properties;
                        break;
                }

                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                this.InnerTextBox.Selection.ApplyPropertyValue(System.Windows.Documents.Paragraph.TextAlignmentProperty, this.propertyValueConvertor.GetTextAlign(this.Properties.HorizontalAlignment));
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
                if (propertyName == "TOP" || propertyName == "LEFT" || propertyName == "HEIGHT" || propertyName == "WIDTH")
                {
                    this.Panel.EditingManager.IsMergeAction = true;
                    this.RaiseReportItemSizeChangedEvent();
                    this.Panel.EditingManager.IsMergeAction = false;
                }
            }
        }

        private void UpdatePaddingValue()
        {
            double left = new RDL.DOM.Size(this.Properties.Padding.PaddingLeft).PixelValue;
            double right = new RDL.DOM.Size(this.Properties.Padding.PaddingLeft).PixelValue;
            double top = new RDL.DOM.Size(this.Properties.Padding.PaddingLeft).PixelValue;
            double bottom = new RDL.DOM.Size(this.Properties.Padding.PaddingLeft).PixelValue;
            this.InnerTextBox.Padding = new Thickness(left, top, right, bottom);
        }

        internal void UpdateBackgroundImage()
        {
            if (this.Properties.BackgroundImage.Source == RDL.DOM.Source.Embedded && this.Panel != null
                 && !string.IsNullOrEmpty(this.Properties.BackgroundImage.ImageValue))
            {
                RDL.DOM.EmbeddedImage embededimage = (from image in this.Panel.EmbeddedImages
                                                  where image.Name.Equals(this.Properties.BackgroundImage.ImageValue)
                                                  select image).FirstOrDefault();

                if (embededimage != null && embededimage.MIMEType != "image/emf")
                {
                    Base64ImageConverter converter = new Base64ImageConverter();
                    System.Windows.Media.Imaging.BitmapImage bitmapImage = converter.ConvertToImage(embededimage.ImageData);
                    ImageBrush brush = new ImageBrush(bitmapImage);
                    this.InnerTextBox.Background = brush;
                }
                else
                {
                    this.InnerTextBox.Background = this.propertyValueConvertor.GetBackGroundColor(this.Properties.FillColor);
                }
            }
            else
            {
                this.InnerTextBox.Background = this.propertyValueConvertor.GetBackGroundColor(this.Properties.FillColor);
            }
        }

        public void ResetSelection()
        {
            if (currentRunBackgroundColor == null) currentRunBackgroundColor = Brushes.White;
            if (this.currentRun != null)
                currentRun.SetValue(Inline.BackgroundProperty, currentRunBackgroundColor);

            Copy.IsEnabled = true;
            Cut.IsEnabled = true;

            TextProperties.IsEnabled = false;
            Expression.IsEnabled = true;
            CreatePlaceHolder.IsEnabled = false;
            TextBoxProperties.IsEnabled = true;
        }

        void InnerTextBox_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.InnerTextBox.Selection.IsEmpty == true)
            {
                TextProperties.IsEnabled = false;
                TextBoxProperties.IsEnabled = true;
                Expression.IsEnabled = true;
                CreatePlaceHolder.IsEnabled = true;

                Copy.IsEnabled = false;
                Cut.IsEnabled = false;
            }
            else
            {
                Copy.IsEnabled = true;
                Cut.IsEnabled = true;

                TextProperties.IsEnabled = true;
                Expression.IsEnabled = false;
                CreatePlaceHolder.IsEnabled = false;
                TextBoxProperties.IsEnabled = false;
            }

            if (listRun == null || (listRun != null && listRun.Count == 0))
            {
                Paste.IsEnabled = false;
            }
            else
            {
                Paste.IsEnabled = true;
            }
        }

        private void document_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void InnerTextBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (currentRunBackgroundColor == null) currentRunBackgroundColor = Brushes.Transparent;
            if (this.currentRun != null)
                currentRun.SetValue(Inline.BackgroundProperty, currentRunBackgroundColor);
        }

        private void TextBoxControl_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        internal void document_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (currentRunBackgroundColor == null) currentRunBackgroundColor = Brushes.Transparent;
                if (this.currentRun != null)
                    currentRun.SetValue(Inline.BackgroundProperty, currentRunBackgroundColor);
                TextPointer t = InnerTextBox.GetPositionFromPoint(e.GetPosition(InnerTextBox), true);
                if (t != null && t.Parent is Run)
                {
                    Run run = t.Parent as Run;
                    if (run != null)
                    {
                        string s = run.Text.Trim();
                        if ((s.StartsWith("[") && s.EndsWith("]")) || (string.Equals(s,"<<Expr>>")
                            && run.Tag != null && run.Tag.ToString().StartsWith("=")))
                        {
                            this.currentRun = run;
                            this.currentRunBackgroundColor = currentRun.Background;
                            if (InnerTextBox.Selection.IsEmpty)
                                currentRun.SetValue(Inline.BackgroundProperty, Brushes.CornflowerBlue);
                            run.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(runPlaceHolder_MouseRightButtonDown);
                            run.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(run_GotKeyboardFocus);
                        }
                        else
                        {
                            this.currentRun = run;
                            this.currentRunBackgroundColor = currentRun.Background;
                        }
                    }
                }
                else
                {
                    var block = InnerTextBox.Document.Blocks.FirstOrDefault();

                    if (block != null)
                    {
                        var run = (block as System.Windows.Documents.Paragraph).Inlines.FirstOrDefault();

                        if (run != null)
                        {
                            this.currentRun = run as Run;
                            this.currentRunBackgroundColor = currentRun.Background;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void run_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Run run = sender as Run;
            if (run != null)
            {
                InnerTextBox.CaretPosition = run.ElementEnd;
                InnerTextBox.CaretPosition = InnerTextBox.CaretPosition.GetNextContextPosition(LogicalDirection.Forward);
            }
        }


        private void InnerTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            TextPointer t = richTextBox.CaretPosition;
            if (t.Parent is Run)
            {
                Run run = t.Parent as Run;
                string s = t.GetTextInRun(LogicalDirection.Backward);
                s = s + t.GetTextInRun(LogicalDirection.Forward);
                s = s.Trim();
                if ((s.StartsWith("[") && s.EndsWith("]")) || (s.StartsWith("<<") && s.EndsWith(">>")))
                {
                    if (Keyboard.Modifiers != ModifierKeys.Control)
                    {
                        if (t.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
                        {
                            Run postRun = new Run(" ");
                            run.SiblingInlines.InsertAfter(run, postRun);
                            InnerTextBox.CaretPosition = postRun.ElementEnd;
                        }
                        else if (t.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                        {
                            Run preRun = new Run(" ");
                            run.SiblingInlines.InsertBefore(run, preRun);
                            InnerTextBox.CaretPosition = preRun.ElementStart;
                        }
                        else
                        {
                            run.Text = "";
                            run.MouseRightButtonDown -= new System.Windows.Input.MouseButtonEventHandler(runPlaceHolder_MouseRightButtonDown);
                            //richTextBox.CaretPosition = run.ElementEnd.GetNextContextPosition(LogicalDirection.Forward);
                            Run postRun = new Run(" ");
                            run.SiblingInlines.InsertAfter(run, postRun);
                            InnerTextBox.CaretPosition = postRun.ElementEnd.GetNextContextPosition(LogicalDirection.Forward);
                        }
                    }
                }
            }
        }

        static void InnerTextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Deselect if selection occured within place holder or part of the place holder
            RichTextBox InnerTextBox = sender as RichTextBox;
            if (InnerTextBox != null)
            {
                TextPointer t = InnerTextBox.Selection.Start;
                int i = 0;
                do
                {
                    if (t.Parent is Run)
                    {
                        Run run = t.Parent as Run;
                        if (run != null)
                        {
                            string s = t.GetTextInRun(LogicalDirection.Backward);
                            s = s + t.GetTextInRun(LogicalDirection.Forward);
                            s = s.Trim();
                            if (s.StartsWith("[") && s.EndsWith("]"))
                            {
                                if (!InnerTextBox.Selection.IsEmpty)
                                {
                                    InnerTextBox.Selection.Select(InnerTextBox.Selection.Start, InnerTextBox.Selection.Start);
                                }
                            }
                        }
                    }
                    t = InnerTextBox.Selection.End;
                    i++;
                } while (i < 2);
            }
        }



        private void InnerTextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            //e.Handled = true;
        }

        private void InnerTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            TextPointer t = InnerTextBox.GetPositionFromPoint(e.GetPosition(InnerTextBox), true);
            if (t.Parent is Run)
            {
                Run run = t.Parent as Run;
                if (run != null)
                {
                    string s = run.Text;
                    if (s.StartsWith("[") && s.EndsWith("]"))
                    {
                        InnerTextBox.CaretPosition = run.ElementEnd.GetNextContextPosition(LogicalDirection.Forward);
                    }
                    else
                    {
                        InnerTextBox.CaretPosition = InnerTextBox.GetPositionFromPoint(e.GetPosition(InnerTextBox), true);
                    }
                }
            }
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        private void InnerTextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TreeObjectCollection)) is TreeObjectCollection)
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);

                TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                if (itemCollection != null && itemCollection.Count > 0)
                {
                    TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;
                    if (treeViewItem != null && treeViewItem.Header != null&&treeViewItem.Tag!=null&&treeViewItem.Tag.GetType().Name=="String")
                    {
                        string header = treeViewItem.Header.ToString();
                        string content = treeViewItem.Tag.ToString().Replace("#BuiltIn#", "");//treeViewItem.Tag.ToString();

                        // One identification RESERVED FOR BUILD IN FUNCTIONS TAG                         
                        if (content == "ExecutionTime" || content == "ReportFolder"
                        || content == "ReportName" || content == "ReportServerUrl"
                        || content == "PageNumber" || content == "TotalPages"
                        || content == "UserID" || content == "Language")
                        {
                            Run runPlaceHolder = new Run(("[&" + content + "]"));
                            runPlaceHolder.Tag = GetBuiltInFunctionsToRDL("[&" + content + "]");
                            insertRun(runPlaceHolder);
                        }
                        else if (content == "Parameters")
                        {
                            Run runParam = new Run("[@" + header + "]");
                            runParam.Tag = "[@" + header + "]";
                            insertRun(runParam);
                        }
                        else
                        {
                            // Normal Place Holders
                            Run run = new Run(("[" + treeViewItem.Header.ToString() + "]"));
                            run.Tag = FieldConverter(treeViewItem.Header.ToString(), PlaceHolderType.Field, null);
                            insertRun(run);
                        }
                    }
                }

                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }

            e.Handled = true;
        }

        internal void TextBoxControl_PlaceHolder_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TreeObjectCollection)) is TreeObjectCollection)
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);
                this.Panel.EditingManager.IsMergeAction = true;

                TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                if (itemCollection != null && itemCollection.Count > 0)
                {
                    TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;
                    if (treeViewItem != null && treeViewItem.Header != null && treeViewItem.Tag != null)
                    {
                        string header = treeViewItem.Header.ToString();
                        string content = treeViewItem.Tag.ToString().Replace("#BuiltIn#", "");

                        // One identification RESERVED FOR BUILD IN FUNCTIONS TAG                         
                        if (content == "ExecutionTime" || content == "ReportFolder"
                        || content == "ReportName" || content == "ReportServerUrl"
                        || content == "PageNumber" || content == "TotalPages"
                        || content == "UserID" || content == "Language")
                        {
                            Run runPlaceHolder = new Run(("[&" + content + "]"));
                            runPlaceHolder.Tag = GetBuiltInFunctionsToRDL("[&" + content + "]");
                            insertRun(runPlaceHolder);
                            return;
                        }
                        else if (content == "Parameters")
                        {
                            Run runParam = new Run("[@" + header + "]");
                            runParam.Tag = "[@" + header + "]";
                            insertRun(runParam);
                            return;
                        }

                        // Normal Place Holders
                        Run run = new Run(("[" + treeViewItem.Header.ToString() + "]"));
                        run.Tag = FieldConverter(treeViewItem.Header.ToString(), PlaceHolderType.Field, null);
                        insertRun(run);
                    }
                }

                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }

            e.Handled = true;
        }

        internal void TextBoxControl_Text_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TreeObjectCollection)) is TreeObjectCollection)
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);
                this.Panel.EditingManager.IsMergeAction = false;

                TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                if (itemCollection != null && itemCollection.Count > 0)
                {
                    TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;
                    if (treeViewItem != null && treeViewItem.Header != null)
                    {
                        Run runPlaceHolder = new Run(treeViewItem.Header.ToString());
                        insertRun(runPlaceHolder);
                    }
                }

                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }

            e.Handled = true;
        }

        internal void TextBoxControl_Text_Drop(string field)
        {
            Run runPlaceHolder = new Run(field);
            insertRun(runPlaceHolder);
        }


        internal static string FieldConverter(string fieldName, PlaceHolderType placeHolderType, string dataSetName)
        {
            if (placeHolderType == PlaceHolderType.Field)
            {
                fieldName = "=Fields!" + fieldName + ".Value";
            }
            else
            {
                if (placeHolderType == PlaceHolderType.First)
                {
                    fieldName = "=First(Fields!" + fieldName + ".Value";
                }
                else if (placeHolderType == PlaceHolderType.Sum)
                {
                    fieldName = "=Sum(Fields!" + fieldName + ".Value";
                }
                else if (placeHolderType == PlaceHolderType.Count)
                {
                    fieldName = "=Count(Fields!" + fieldName + ".Value";
                }
                else if (placeHolderType == PlaceHolderType.Avg)
                {
                    fieldName = "=Avg(Fields!" + fieldName + ".Value";
                }

                if (dataSetName != null)
                {
                    fieldName = fieldName + ", \"" + dataSetName + "\")";
                }
                else
                {
                    fieldName = fieldName + ")";
                }
            }
            return fieldName;
        }


        private void InnerTextBox_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void runPlaceHolder_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ContextMenu contextMenuSpan = new ContextMenu();
            MenuItem placeHolderProperyMenu = new MenuItem();
            MenuItem cutMenu = new MenuItem();
            MenuItem copyMenu = new MenuItem();
            MenuItem pasteMenu = new MenuItem();
            MenuItem expressionMenu = new MenuItem();
            cutMenu.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelCut"); ;
            copyMenu.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelCopy"); ;
            pasteMenu.Header = SR.GetString(CultureInfo.CurrentUICulture, "labelPaste"); ;
            expressionMenu.Header = SR.GetString(CultureInfo.CurrentUICulture, "titleTextBoxExpression"); ;
            placeHolderProperyMenu.Header = SR.GetString(CultureInfo.CurrentUICulture, "titlePlaceHolderProperties");
            cutMenu.Click += new RoutedEventHandler(Cut_Click);
            copyMenu.Click += new RoutedEventHandler(Copy_Click);
            pasteMenu.Click += new RoutedEventHandler(Paste_Click);
            expressionMenu.Click += new RoutedEventHandler(Expression_Click);
           // placeHolderProperyMenu.Click += new RoutedEventHandler(placeHolderProperyMenu_Click);
            contextMenuSpan.Items.Add(cutMenu);
            contextMenuSpan.Items.Add(copyMenu);
            contextMenuSpan.Items.Add(pasteMenu);
            contextMenuSpan.Items.Add(expressionMenu);
            contextMenuSpan.Items.Add(placeHolderProperyMenu);
            currentRun.ContextMenu = contextMenuSpan;
        }


        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            innerTextBox_Cut();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            innerTextBox_Copy();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            innerTextBox_Paste();
        }

        private void cutBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            innerTextBox_Cut();
        }

        private void copyBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            innerTextBox_Copy();
        }

        private void pasteBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            innerTextBox_Paste();
        }


        private void innerTextBox_Cut()
        {
            if (listRun != null)
            {
                //clear the Previous cut
                if (listRun.Count > 0)
                {
                    listRun.Clear();
                }

                // If selection is empty, cut the current Place Holder
                if (InnerTextBox.Selection.IsEmpty)
                {
                    TextPointer t = InnerTextBox.CaretPosition;
                    Run run = t.Parent as Run;
                    if (run != null)
                    {
                        string runText = run.Text.Trim();
                        if (runText.StartsWith("[") && runText.EndsWith("]"))
                        {
                            Run runCopy = createCloneRun(run);
                            if (currentRunBackgroundColor != null) runCopy.Background = currentRunBackgroundColor;
                            listRun.Add(runCopy);
                            run.Text = "";
                            run.MouseRightButtonDown -= new System.Windows.Input.MouseButtonEventHandler(runPlaceHolder_MouseRightButtonDown);
                            InnerTextBox.CaretPosition = run.ElementEnd;
                        }
                    }
                }
                else
                // If selection is available in Rich Text Box, add the all Run elements( in the selection range)into the ListRun 
                {

                    TextPointer endSelection = InnerTextBox.Selection.End;
                    TextPointer startSelection = InnerTextBox.Selection.Start;
                    TextPointer t = InnerTextBox.Selection.Start;
                    Run fullRun = t.Parent as Run;
                    if (fullRun != null)
                    {
                        // If selection within the same run
                        if (endSelection.CompareTo(fullRun.ElementEnd) <= 0 && startSelection.CompareTo(fullRun.ElementStart) >= 0)
                        {
                            string pre = t.GetTextInRun(LogicalDirection.Backward);
                            string select = InnerTextBox.Selection.Text;
                            string post = endSelection.GetTextInRun(LogicalDirection.Forward);
                            fullRun.Text = pre;
                            Run selectTag = createCloneRun(fullRun);
                            selectTag.Text = select;
                            Run runCopy = createCloneRun(selectTag);
                            listRun.Add(runCopy);
                            selectTag.Text = "";
                            fullRun.SiblingInlines.InsertAfter(fullRun, selectTag);
                            Run postTag = createCloneRun(fullRun);
                            postTag.Text = post;
                            selectTag.SiblingInlines.InsertAfter(selectTag, postTag);
                            InnerTextBox.CaretPosition = selectTag.ElementEnd;
                        }
                        else
                        {
                            for (TextPointer position = t.GetNextContextPosition(LogicalDirection.Forward);
                                (position != null && position.CompareTo(endSelection) <= 0 && position.CompareTo(startSelection) >= 0);
                                position = position.GetNextContextPosition(LogicalDirection.Forward))
                            {
                                //MessageBox.Show(++j + "p");
                                Run run = position.Parent as Run;
                                if (run != null)
                                {
                                    TextPointer currentElementEnd = run.ElementEnd;
                                    //If part of selection is made in first run
                                    if (startSelection.CompareTo(run.ElementStart) > 0)
                                    {
                                        string pre = startSelection.GetTextInRun(LogicalDirection.Backward);
                                        string post = startSelection.GetTextInRun(LogicalDirection.Forward);
                                        //MessageBox.Show(" !Start " + pre + "-" + post);
                                        run.Text = pre;
                                        Run postTag = createCloneRun(run);
                                        postTag.Text = post;
                                        Run runCopy = createCloneRun(postTag);
                                        listRun.Add(runCopy);
                                        postTag.Text = "";
                                        run.SiblingInlines.InsertAfter(run, postTag);
                                        position = postTag.ElementEnd;
                                    }

                                    // Traverse through the selection
                                    if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
                                    {
                                        //MessageBox.Show(run.Text);
                                        Run runCopy = createCloneRun(run);
                                        listRun.Add(runCopy);
                                        run.Text = "";
                                        t = run.ElementEnd;
                                    }

                                    // If part of selection is made in last run
                                    if (endSelection.CompareTo(currentElementEnd) < 0)
                                    {
                                        string pre = endSelection.GetTextInRun(LogicalDirection.Backward);
                                        string post = endSelection.GetTextInRun(LogicalDirection.Forward);
                                        //MessageBox.Show(" !end " + pre + "-" + post);
                                        run.Text = pre;
                                        Run runCopy = createCloneRun(run);
                                        listRun.Add(runCopy);
                                        run.Text = "";
                                        Run postTag = createCloneRun(run);
                                        postTag.Text = post;
                                        run.SiblingInlines.InsertAfter(run, postTag);
                                        position = postTag.ElementEnd;
                                        t = run.ElementEnd;
                                    }
                                }
                            }
                            InnerTextBox.CaretPosition = t;
                        }
                    }
                }
            }
        }


        private void innerTextBox_Copy()
        {
            if (listRun != null)
            {
                //clear the Previous copy
                if (listRun.Count > 0)
                {
                    listRun.Clear();
                }

                // If selection is empty, copy the current Place Holder
                if (InnerTextBox.Selection.IsEmpty)
                {
                    TextPointer t = InnerTextBox.CaretPosition;
                    Run run = t.Parent as Run;
                    if (run != null)
                    {
                        string runText = run.Text.Trim();
                        if (runText.StartsWith("[") && runText.EndsWith("]"))
                        {
                            listRun.Add(run);
                        }
                    }
                }
                else
                // If selection is available in Rich Text Box, add the all Run elements( in the selection range)into the ListRun 
                {

                    //int j = 0;
                    TextPointer endSelection = InnerTextBox.Selection.End;
                    TextPointer startSelection = InnerTextBox.Selection.Start;
                    TextPointer t = InnerTextBox.Selection.Start;
                    Run fullRun = t.Parent as Run;
                    if (fullRun != null)
                    {
                        // If selection within the same run
                        if (endSelection.CompareTo(fullRun.ElementEnd) <= 0 && startSelection.CompareTo(fullRun.ElementStart) >= 0)
                        {
                            string pre = t.GetTextInRun(LogicalDirection.Backward);
                            string select = InnerTextBox.Selection.Text;
                            string post = endSelection.GetTextInRun(LogicalDirection.Forward);
                            fullRun.Text = pre;
                            Run selectTag = createCloneRun(fullRun);
                            selectTag.Text = select;
                            listRun.Add(selectTag);
                            fullRun.SiblingInlines.InsertAfter(fullRun, selectTag);
                            Run postTag = createCloneRun(fullRun);
                            postTag.Text = post;
                            selectTag.SiblingInlines.InsertAfter(selectTag, postTag);
                            InnerTextBox.CaretPosition = selectTag.ElementEnd;
                        }
                        else
                        {
                            for (TextPointer position = t.GetNextContextPosition(LogicalDirection.Forward);
                                (position != null && position.CompareTo(endSelection) <= 0 && position.CompareTo(startSelection) >= 0);
                                position = position.GetNextContextPosition(LogicalDirection.Forward))
                            {

                                //MessageBox.Show(++j + "p");                                
                                Run run = position.Parent as Run;
                                if (run != null)
                                {
                                    TextPointer currentElementEnd = run.ElementEnd;
                                    ///If part of selection is made in first run
                                    if (startSelection.CompareTo(run.ElementStart) > 0)
                                    {
                                        string pre = startSelection.GetTextInRun(LogicalDirection.Backward);
                                        string post = startSelection.GetTextInRun(LogicalDirection.Forward);
                                        run.Text = pre;
                                        Run postTag = createCloneRun(run);
                                        postTag.Text = post;
                                        run.SiblingInlines.InsertAfter(run, postTag);
                                        listRun.Add(postTag);
                                        position = postTag.ElementEnd;
                                    }

                                    // Traverse through the selection
                                    if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
                                    {
                                        listRun.Add(run);
                                        t = run.ElementEnd;
                                    }

                                    // If part of selection is made in last run
                                    if (endSelection.CompareTo(currentElementEnd) < 0)
                                    {
                                        string pre = endSelection.GetTextInRun(LogicalDirection.Backward);
                                        string post = endSelection.GetTextInRun(LogicalDirection.Forward);
                                        run.Text = pre;
                                        listRun.Add(run);
                                        Run postTag = createCloneRun(run);
                                        postTag.Text = post;
                                        run.SiblingInlines.InsertAfter(run, postTag);
                                        position = postTag.ElementEnd;
                                        t = run.ElementEnd;
                                    }
                                }
                            }
                            InnerTextBox.CaretPosition = t;
                        }
                    }
                }
            }
        }

        private void innerTextBox_Paste()
        {
            TextPointer t = InnerTextBox.CaretPosition;
            if (listRun != null)
            {
                // Create a clone for all ListRun elements
                int listRunCount = listRun.Count;
                if (listRunCount > 0)
                {
                    Run run = t.Parent as Run;
                    Run runCopy = null;
                    if (run != null)
                    {
                        TextPointer position = run.ElementEnd;
                        string post = t.GetTextInRun(LogicalDirection.Forward);
                        string pre = t.GetTextInRun(LogicalDirection.Backward);
                        string s = pre + post;
                        s = s.Trim();
                        if (s.StartsWith("[") && s.EndsWith("]"))
                        {
                            return;
                        }

                        run.Text = pre;
                        Run postTag = createCloneRun(run);
                        postTag.Text = post;
                        if (listRunCount == 1)
                            postTag.Text += " ";
                        for (int i = 0; i < listRunCount; i++)
                        {
                            Run r = listRun.ElementAt(i) as Run;
                            if (r != null)
                            {
                                runCopy = createCloneRun(r);
                                run.SiblingInlines.InsertAfter(run, runCopy);
                                run = runCopy;
                                position = run.ElementEnd;
                            }
                        }
                        if (run != null)
                        {
                            run.SiblingInlines.InsertAfter(run, postTag);
                        }
                        InnerTextBox.CaretPosition = postTag.ElementEnd;
                    }

                }
            }
        }

        private Run createCloneRun(Run run)
        {
            Run runCopy = new Run(run.Text);
            runCopy.Tag = run.Tag;
            runCopy.FontFamily = run.FontFamily;
            runCopy.FontSize = run.FontSize;
            runCopy.Foreground = run.Foreground;
            runCopy.Background = run.Background;
            runCopy.FontStyle = run.FontStyle;
            runCopy.FontWeight = run.FontWeight;
            runCopy.TextDecorations = run.TextDecorations;
            return runCopy;
        }

        private void insertRun(Run runPlaceHolder)
        {
            runPlaceHolder.Foreground = Brushes.Black;
            //Add the Place Holder at the specified Position
            TextPointer t = InnerTextBox.CaretPosition;
            if (t != null && t.Parent is Run)
            {
                Run run = t.Parent as Run;
                if (run != null && this.TextLength > 0)
                {
                    int length = InnerTextBox.CaretPosition.GetTextRunLength(LogicalDirection.Forward);
                    string pre = run.Text.Substring(0, run.Text.Length - length);
                    string post = " " + run.Text.Substring(run.Text.Length - length, length) + " ";
                    run.Text = pre;
                    Run postTag = createCloneRun(run);
                    postTag.Text = post;
                    run.SiblingInlines.InsertAfter(run, runPlaceHolder);
                    var preTag = new Run(" ");
                    runPlaceHolder.SiblingInlines.InsertBefore(runPlaceHolder, preTag);
                    runPlaceHolder.SiblingInlines.InsertAfter(runPlaceHolder, postTag);
                    InnerTextBox.CaretPosition = postTag.ElementEnd;
                }
                else
                {
                    addRunInEmptyTextbox(runPlaceHolder);
                }
            }
            else
            {
                addRunInEmptyTextbox(runPlaceHolder);
            }
        }

        private void addRunInEmptyTextbox(Run runPlaceHolder)
        {
            System.Windows.Documents.Paragraph paragraph;
            if (this.InnerTextBox.Document.Blocks.Count == 1 && this.InnerTextBox.Document.Blocks.First() is System.Windows.Documents.Paragraph)
            {
                paragraph = this.InnerTextBox.Document.Blocks.First() as System.Windows.Documents.Paragraph;
                if (this.TextLength == 0) paragraph.Inlines.Clear();
            }
            else
            {
                paragraph = new System.Windows.Documents.Paragraph();
            }
            if (paragraph != null)
            {
                paragraph.Inlines.Add(runPlaceHolder);
                document.Blocks.Add(paragraph);
            }
        }

        private bool isBuiltInFuntion(string content)
        {
            if (content != null && content != string.Empty && content.StartsWith("[&") && content.EndsWith("]"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Expression_Click(object sender, RoutedEventArgs e)
        {
            Syncfusion.Windows.Reports.Designer.Editors.ExpressionDialog dialog = new Syncfusion.Windows.Reports.Designer.Editors.ExpressionDialog(Editors.ValueType.StringValue, "Value");
            this.Panel.UpdateOwnerWindow(dialog);

            if (this.currentRun != null)
            {
                if (this.currentRun.Text.Equals("<<Expr>>"))
                {
                    dialog.expressionTextBox.Text = this.currentRun.Tag.ToString();
                }
                else
                {
                    var run = getTextRun(this.currentRun);

                    if ((run.Value != null && run.Value.Equals(this.currentRun.Text.Trim()))||
                        (run.InternalLabel != null && run.InternalLabel.Equals(this.currentRun.Text.Trim())))
                    {
                        if (run.Value != null)
                        {
                            dialog.expressionTextBox.Text = run.Value;
                        }
                        else
                        {
                            string field = this.currentRun.Text;
                            field = field.Replace('[', ' ');
                            field = field.Replace(']', ' ').Trim();
                            dialog.expressionTextBox.Text = "=Fields!" + field + ".Value";
                        }
                    }
                    else
                    {
                        dialog.expressionTextBox.Text = run.Value;
                    }
                }
            }
            else
            {
                try
                {
                    dialog.expressionTextBox.Text = this.InnerTextBox.Selection.Text;
                }
                catch { }
            }

            if (dialog.ShowDialog() == true)
            {
                bool replaceRun = true;
                string expression = dialog.expressionTextBox.Text.Trim();

                if (this.currentRun != null)
                {
                    string runText = this.currentRun.Text.Trim();

                    if (runText.Equals(expression))
                    {
                        replaceRun = false;
                    }
                    else if (runText.StartsWith("[") && runText.EndsWith("]")
                        && this.currentRun.Tag != null && this.currentRun.Tag.ToString().Equals(expression))
                    {
                        replaceRun = false;
                    }
                    else if (string.Equals(runText, "<<Expr>>") 
                        && this.currentRun.Tag != null && this.currentRun.Tag.ToString().Equals(expression))
                    {
                        replaceRun = false;
                    }
                }

                if (replaceRun)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.ItemChanged;
                    ItemChange change = new ItemChange();
                    change.ReportItem = this;
                    change.OldValue = this.GetReportItem();
                    action.ItemChange = change;
                    this.Panel.EditingManager.AddAction(action);

                    SetEpressionValue(expression);

                    change.NewValue = this.GetReportItem();
                    this.Properties.IsInternalPropertyChange = false;
                }
            }
        }

        private void SetEpressionValue(string expression)
        {
            TextRun textrun = new TextRun();
            textrun.Value = expression;
            TextPointer t = InnerTextBox.CaretPosition;

            if (t != null && t.Parent is Run)
            {
                Run run = t.Parent as Run;

                if (run != null)
                {
                    Run expRun = new Run();
                    expRun = createCloneRun(run);

                    if (expression.Contains("\""))
                    {
                        expRun.Text = "<<Expr>>";
                    }
                    else
                    {
                        expRun.Text = GetTextRunLabel(expression);
                    }

                    expRun.Tag = expression;

                    run.SiblingInlines.InsertAfter(run, expRun);
                    run.SiblingInlines.Remove(run);
                }
            }
        }

        private void TextProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = new ControlProperties(this, this.ReportDataSets, TextBoxPropertyType.TextProperty);
            this.Panel.UpdateOwnerWindow(controlProperties);

            if (!this.InnerTextBox.Selection.IsEmpty)
            {
                object objFontName = this.InnerTextBox.Selection.GetPropertyValue(TextBoxControl.FontFamilyProperty);
                
                if (objFontName != DependencyProperty.UnsetValue)
                {
                    if (this.Properties.FontFamily.StartsWith("="))
                        controlProperties.TextBoxFont.cmb_FontName.TextValue = this.Properties.FontFamily;
                    else
                        controlProperties.TextBoxFont.cmb_FontName.TextValue = objFontName.ToString();
                }
                else
                {
                    controlProperties.TextBoxFont.cmb_FontName.SelectedItem = new FontFamilyConverter().ConvertFromInvariantString("Arial");
                }

                double fontSize = 10;
                object objFontSize = this.InnerTextBox.Selection.GetPropertyValue(TextBoxControl.FontSizeProperty);
                if (objFontSize != DependencyProperty.UnsetValue)
                {
                    fontSize = (double)objFontSize;
                    const double PointValue = 1.333333333;
                    fontSize = Math.Floor(new RDL.DOM.Size((fontSize / PointValue) + "pt").FloatValue);
                }
                if (this.Properties.FontSize.StartsWith("="))
                {
                    controlProperties.TextBoxFont.cmb_FontSize.TextValue = this.Properties.FontSize;
                }
                else
                {
                    controlProperties.TextBoxFont.cmb_FontSize.TextValue = fontSize + "pt";
                }

                object objFontWeight = this.InnerTextBox.Selection.GetPropertyValue(TextBoxControl.FontWeightProperty);
                if (objFontWeight != DependencyProperty.UnsetValue)
                {
                    if ((System.Windows.FontWeight)objFontWeight == System.Windows.FontWeights.Bold)
                    {
                        controlProperties.TextBoxFont.chk_Bold.IsChecked = true;
                    }
                    else
                    {
                        controlProperties.TextBoxFont.chk_Bold.IsChecked = false;
                    }
                }
                else
                {
                    controlProperties.TextBoxFont.chk_Bold.IsChecked = false;
                }

                object objFontStyle = this.InnerTextBox.Selection.GetPropertyValue(TextBoxControl.FontStyleProperty);
                if (objFontStyle != DependencyProperty.UnsetValue)
                {
                    if ((System.Windows.FontStyle)objFontStyle == System.Windows.FontStyles.Italic)
                    {
                        controlProperties.TextBoxFont.chk_Italic.IsChecked = true;
                    }
                    else
                    {
                        controlProperties.TextBoxFont.chk_Italic.IsChecked = false;
                    }
                }
                else
                {
                    controlProperties.TextBoxFont.chk_Italic.IsChecked = false;
                }

                object objTextDecoration = this.InnerTextBox.Selection.GetPropertyValue(AccessText.TextDecorationsProperty);
                if (objTextDecoration != DependencyProperty.UnsetValue)
                {
                    if (this.Properties.FontEffects != null && this.Properties.FontEffects.StartsWith("="))
                    {
                        controlProperties.TextBoxFont.cmb_Effects.TextValue = this.Properties.FontEffects;
                    }
                    else
                    {
                        try
                        {
                            var decoration = (objTextDecoration as TextDecorationCollection).FirstOrDefault() != null ? (objTextDecoration as TextDecorationCollection).First().Location.ToString() : "None";
                            controlProperties.TextBoxFont.cmb_Effects.TextValue = Enum.Parse(typeof(Syncfusion.RDL.DOM.TextDecoration), decoration, true).ToString();
                        }
                        catch { }
                    }
                }
                else
                {
                    controlProperties.TextBoxFont.cmb_Effects.TextValue = Syncfusion.RDL.DOM.TextDecoration.None.ToString();
                }


                object objLineSpace = this.InnerTextBox.Selection.GetPropertyValue(AccessText.LineHeightProperty);
                if (objLineSpace != DependencyProperty.UnsetValue)
                {
                    double d = (double)objLineSpace;
                    if (double.IsNaN(d) == true) d = 10.0;
                    controlProperties.TextBoxFont.LineSpacingCustomValue.Value = d;
                }
                else
                {
                    controlProperties.TextBoxFont.LineSpacingCustomValue.Value = 10.0;
                }
                controlProperties.TextBoxFont.LineSpacingFontSize.IsChecked = controlProperties.TextBoxFont.LineSpacingCustomValue.Value != fontSize ? false : true;

                if (controlProperties.TextBoxFont.LineSpacingFontSize.IsChecked == true)
                {
                    controlProperties.TextBoxFont.LineSpacingCustom.IsChecked = false;
                    controlProperties.TextBoxFont.LineSpacingCustomValue.IsEnabled = false;
                }
                else
                {
                    controlProperties.TextBoxFont.LineSpacingCustom.IsChecked = true;
                    controlProperties.TextBoxFont.LineSpacingCustomValue.IsEnabled = true;
                }
                object objForeGroundColor = this.InnerTextBox.Selection.GetPropertyValue(TextBoxControl.ForegroundProperty);
                if (objForeGroundColor != DependencyProperty.UnsetValue)
                {
                    if (this.Properties.FontColor.StartsWith("="))
                        controlProperties.TextBoxFont.cpkr_Font.Text = this.Properties.FontColor;
                    else
                        controlProperties.TextBoxFont.cpkr_Font.Text = objForeGroundColor.ToString();
                }
                else
                {
                    controlProperties.TextBoxFont.cpkr_Font.Text = "Black";
                }

                object objTextBackground = this.InnerTextBox.Selection.GetPropertyValue(Inline.BackgroundProperty);
                if (objTextBackground == null) objTextBackground = Brushes.White;
                if (objTextBackground != DependencyProperty.UnsetValue)
                {
                    if (this.Properties.FillColor.StartsWith("="))
                        controlProperties.TextBoxFill.FillColor.Text = this.Properties.FillColor;
                    else
                        controlProperties.TextBoxFill.FillColor.Text = objTextBackground.ToString();
                }
                else
                {
                    controlProperties.TextBoxFill.FillColor.Text = "White";
                }

                object objTextAlign = this.InnerTextBox.Selection.GetPropertyValue(System.Windows.Documents.Paragraph.TextAlignmentProperty);
                if (objTextAlign != DependencyProperty.UnsetValue)
                {
                    TextAlignment tAlign = (TextAlignment)objTextAlign;
                    if (tAlign == TextAlignment.Left)
                    {
                        controlProperties.TextBoxAlignment.HorizontalValue.Text = "Left";
                    }
                    else if (tAlign == TextAlignment.Right)
                    {
                        controlProperties.TextBoxAlignment.HorizontalValue.Text = "Right";
                    }
                    else if (tAlign == TextAlignment.Center)
                    {
                        controlProperties.TextBoxAlignment.HorizontalValue.Text = "Center";
                    }
                    else
                    {
                        controlProperties.TextBoxAlignment.HorizontalValue.Text = "Default";
                    }
                }
                else
                {
                    controlProperties.TextBoxAlignment.HorizontalValue.SelectedIndex = 0;
                }
                if (this.Action != null)
                {
                    if (this.Action.Drillthrough != null)
                    {
                        if (!string.IsNullOrEmpty(this.Action.Drillthrough.ReportName))
                        {
                            controlProperties.ControlAction.rbtn_goToReport.IsChecked = true;
                            controlProperties.ControlAction.txt_reportName.Text = this.Action.Drillthrough.ReportName;
                        }
                    }
                    else if (!string.IsNullOrEmpty(this.Action.Hyperlink) || this.Panel.DataSets != null)
                    {
                        if (!string.IsNullOrEmpty(this.Action.Hyperlink))
                        {
                            controlProperties.ControlAction.cmbx_UrlPath.Items.Add(this.Action.Hyperlink);
                            controlProperties.ControlAction.cmbx_UrlPath.Text = this.Action.Hyperlink;
                            controlProperties.ControlAction.rbtn_goToUrl.IsChecked = true;
                        }
                    }
                }

                controlProperties.TextBoxAlignment.HorizontalValue.Text = this.Properties.HorizontalAlignment;

                if (controlProperties.ShowDialog() == true)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.ItemChanged;
                    ItemChange change = new ItemChange();
                    change.ReportItem = this;
                    change.OldValue = this.GetReportItem();
                    action.ItemChange =change;
                    this.Panel.EditingManager.AddAction(action);

                    Run fullRun = InnerTextBox.Selection.Start.Parent as Run;
                    if (fullRun != null)
                    {
                        setRunProperty(controlProperties);
                    }
                    change.NewValue = this.GetReportItem();
                    this.Properties.IsInternalPropertyChange = false;
                }

                InnerTextBox.Selection.Select(InnerTextBox.Selection.Start, InnerTextBox.Selection.Start);
            }
        }

        private void TextBoxProperties_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RDL.DOM.ReportItem oldTexboxValue = this.GetReportItem();

            ControlProperties controlProperties = new ControlProperties(this, this.Panel.DataSets, TextBoxPropertyType.TextBoxProperty);
            this.Panel.UpdateOwnerWindow(controlProperties);

            // Get the Properties from selected Text and assign into the wizard
            if (this.InnerTextBox.Selection.IsEmpty)
            {
                controlProperties.TextBoxGeneral.Name.Text = this.Properties.Name;

                controlProperties.TextBoxFont.cmb_FontName.TextValue = this.Properties.FontFamily;

                double fontSize = 10;
                object objFontSize = this.InnerTextBox.FontSize;
                if (objFontSize != DependencyProperty.UnsetValue)
                {
                    fontSize = (double)objFontSize;
                    const double PointValue = 1.333333333;
                    fontSize = Math.Floor(new RDL.DOM.Size((fontSize / PointValue) + "pt").FloatValue);
                }
                controlProperties.TextBoxFont.cmb_FontSize.TextValue = this.Properties.FontSize;


                //System.Windows.FontWeight fontweight = this.InnerTextBox.FontWeight;
                //this.Properties.FontStyle = this.InnerTextBox.FontSize.ToString();
                if (this.Properties.FontWeight == System.Windows.FontWeights.Bold.ToString())
                {
                    controlProperties.TextBoxFont.chk_Bold.IsChecked = true;
                }
                else
                {
                    controlProperties.TextBoxFont.chk_Bold.IsChecked = false;
                }
                //System.Windows.FontStyle fontstyle = this.InnerTextBox.FontStyle;
                //this.Properties.FontStyle = this.InnerTextBox.FontStyle.ToString();
                if (this.Properties.FontStyle == System.Windows.FontStyles.Italic.ToString())
                {
                    controlProperties.TextBoxFont.chk_Italic.IsChecked = true;
                }
                else
                {
                    controlProperties.TextBoxFont.chk_Italic.IsChecked = false;
                }
                //Brush brush = this.InnerTextBox.Foreground;
                //this.Properties.FontColor = this.InnerTextBox.Foreground.ToString();
                controlProperties.TextBoxFont.cpkr_Font.Text = this.Properties.FontColor;

                //controlProperties.TextBoxFont.cmb_Effects.Text = textDecoration;
                if (!string.IsNullOrEmpty(this.Properties.FontEffects))
                {
                    controlProperties.TextBoxFont.cmb_Effects.TextValue = this.Properties.FontEffects;
                }
                else
                {
                    controlProperties.TextBoxFont.cmb_Effects.TextValue = "Default";
                }

                double lineSpace = (double)InnerTextBox.GetValue(AccessText.LineHeightProperty);
                if (double.IsNaN(lineSpace) == true) lineSpace = 10.0;
                controlProperties.TextBoxFont.LineSpacingCustomValue.Value = lineSpace;
                controlProperties.TextBoxFont.LineSpacingFontSize.IsChecked = controlProperties.TextBoxFont.LineSpacingCustomValue.Value != fontSize ? false : true;
                if (controlProperties.TextBoxFont.LineSpacingFontSize.IsChecked == true)
                {
                    controlProperties.TextBoxFont.LineSpacingCustom.IsChecked = false;
                    controlProperties.TextBoxFont.LineSpacingCustomValue.IsEnabled = false;
                }
                else
                {
                    controlProperties.TextBoxFont.LineSpacingCustom.IsChecked = true;
                    controlProperties.TextBoxFont.LineSpacingCustomValue.IsEnabled = true;
                }
                //Brush background = this.InnerTextBox.Background;
                //this.Properties.FillColor = this.InnerTextBox.Background.ToString();

                controlProperties.TextBoxFill.FillColor.Text = this.Properties.FillColor;

                if (this.Properties.HorizontalAlignment == "Left")
                {
                    controlProperties.TextBoxAlignment.HorizontalValue.Text = "Left";
                }
                else if (this.Properties.HorizontalAlignment == "Right")
                {
                    controlProperties.TextBoxAlignment.HorizontalValue.Text = "Right";
                }
                else if (this.Properties.HorizontalAlignment == "Center")
                {
                    controlProperties.TextBoxAlignment.HorizontalValue.Text = "Center";
                }
                else
                {
                    controlProperties.TextBoxAlignment.HorizontalValue.Text = "Default";
                }

                if (this.Properties.VerticalAlignment == "Top")
                {
                    controlProperties.TextBoxAlignment.VerticalValue.Text = "Top";
                }
                else if (this.Properties.VerticalAlignment =="Center")
                {
                    controlProperties.TextBoxAlignment.VerticalValue.Text = "Middle";
                }
                else if (this.Properties.VerticalAlignment == "Bottom")
                {
                    controlProperties.TextBoxAlignment.VerticalValue.Text = "Bottom";
                }
                else
                {
                    controlProperties.TextBoxAlignment.VerticalValue.Text = "Default";
                }
                if (this.Properties.Padding != null)
                {
                    controlProperties.TextBoxAlignment.Left.Text = this.Properties.Padding.PaddingLeft;
                    controlProperties.TextBoxAlignment.Right.Text = this.Properties.Padding.PaddingRight;
                    controlProperties.TextBoxAlignment.Top.Text = this.Properties.Padding.PaddingTop;
                    controlProperties.TextBoxAlignment.Bottom.Text = this.Properties.Padding.PaddingBottom;
                }
            }
            controlProperties.TextBoxBorder.cmb_style.Text = this.Properties.BorderStyles.DefaultBorderStyle.ToString();
            controlProperties.TextBoxBorder.updwn_Custom.Text = this.Properties.BorderWidths.DefaultBorderWidth.ToString();
            controlProperties.TextBoxBorder.borderColor.Text = this.Properties.BorderColors.DefaultBorderColor.ToString();
            if (this.Properties.Hidden == "True")
            {
                controlProperties.TextBoxVisibility.rbtn_Show.IsChecked = false;
                controlProperties.TextBoxVisibility.rbtn_Hide.IsChecked = true;
            }
            else
            {
                controlProperties.TextBoxVisibility.rbtn_Show.IsChecked = true;
                controlProperties.TextBoxVisibility.rbtn_Hide.IsChecked = false;
            }
            if (this.Action != null)
            {
                if (this.Action.Drillthrough != null)
                {
                    if (!string.IsNullOrEmpty(this.Action.Drillthrough.ReportName))
                    {
                        controlProperties.ControlAction.rbtn_goToReport.IsChecked = true;
                        controlProperties.ControlAction.txt_reportName.Text = this.Action.Drillthrough.ReportName;
                    }
                }
                else if (!string.IsNullOrEmpty(this.Action.Hyperlink) || this.Panel.DataSets != null)
                {
                    if (!string.IsNullOrEmpty(this.Action.Hyperlink))
                    {
                        controlProperties.ControlAction.cmbx_UrlPath.Items.Add(this.Action.Hyperlink);
                        controlProperties.ControlAction.cmbx_UrlPath.Text = this.Action.Hyperlink;
                        controlProperties.ControlAction.rbtn_goToUrl.IsChecked = true;
                    }
                }
            }

            if (controlProperties.ShowDialog() == true)
            {
                if (this.InnerTextBox.Selection.IsEmpty)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.ItemChanged;
                    ItemChange change = new ItemChange();
                    change.ReportItem = this;
                    change.OldValue = oldTexboxValue;
                    action.ItemChange =change;
                    this.Panel.EditingManager.AddAction(action);
                    setTextBoxProperty(controlProperties);
                    change.NewValue = this.GetReportItem();
                    this.Properties.IsInternalPropertyChange = false;

                    MemoryStream stream = new MemoryStream();
                    textSerializer.Serialize(stream, change.NewValue as RDL.DOM.TextBox);
                    this.oldItem = change.NewValue;
                    this.oldItemString = Encoding.UTF8.GetString(stream.GetBuffer());
                }
            }
        }

        private void setTextBoxProperty(ControlProperties controlProperties)
        {
            if (controlProperties != null && controlProperties.grd_PlaceHolder != null)
            {
                foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is TextBoxGeneral)
                    {
                        TextBoxGeneral textBoxGeneral = (TextBoxGeneral)uiElement;

                        // if it is textbox properties
                        if ((textBoxGeneral.stpnl_Value.Visibility == System.Windows.Visibility.Hidden))
                        {
                            this.Properties.Name = controlProperties.TextBoxGeneral.Name.Text;
                            this.Properties.ToolTip = controlProperties.TextBoxGeneral.ToolTip.Text;
                        }
                    }
                    else if (uiElement is TextBoxAlignment)
                    {
                        TextBoxAlignment textBoxAlignment = (TextBoxAlignment)uiElement;
                        switch (textBoxAlignment.HorizontalValue.SelectedItem.ToString())
                        {
                            case "Left":
                                {
                                    this.Properties.HorizontalAlignment = "Left";
                                    break;
                                }
                            case "Right":
                                {
                                    this.Properties.HorizontalAlignment = "Right";
                                    break;
                                }
                            case "Center":
                                {
                                    this.Properties.HorizontalAlignment = "Center";
                                    break;
                                }
                            default:
                                {
                                    this.Properties.HorizontalAlignment = "Default";
                                    break;
                                }
                        }
                        switch (textBoxAlignment.VerticalValue.SelectedItem.ToString())
                        {
                            case "Top":
                                {
                                    this.Properties.VerticalAlignment = "Top";
                                    break;
                                }
                            case "Bottom":
                                {
                                    this.Properties.VerticalAlignment = "Bottom";
                                    break;
                                }
                            case "Middle":
                                {
                                    this.Properties.VerticalAlignment = "Middle";
                                    break;
                                }
                            default:
                                {
                                    this.Properties.VerticalAlignment = "Default";
                                    break;
                                }
                        }

                        this.Properties.Padding.PaddingLeft = controlProperties.TextBoxAlignment.Left.Text;
                        this.Properties.Padding.PaddingRight = controlProperties.TextBoxAlignment.Right.Text;
                        this.Properties.Padding.PaddingTop = controlProperties.TextBoxAlignment.Top.Text;
                        this.Properties.Padding.PaddingBottom = controlProperties.TextBoxAlignment.Bottom.Text;
                        //double leftPadding = new RDL.DOM.Size(controlProperties.TextBoxAlignment.Left.Text).PixelValue;
                        //double topPadding = (double)controlProperties.TextBoxAlignment.Top.Value;
                        //double rightPadding = (double)controlProperties.TextBoxAlignment.Right.Value;
                        //double bottomPadding = (double)controlProperties.TextBoxAlignment.Bottom.Value;
                        if (this.Properties.Padding != null)
                        {
                            double left = Convert.ToDouble(new RDL.DOM.Size(this.Properties.Padding.PaddingLeft).PixelValue);
                            double right = Convert.ToDouble(new RDL.DOM.Size(this.Properties.Padding.PaddingRight).PixelValue);
                            double top = Convert.ToDouble(new RDL.DOM.Size(this.Properties.Padding.PaddingTop).PixelValue);
                            double bottom = Convert.ToDouble(new RDL.DOM.Size(this.Properties.Padding.PaddingBottom).PixelValue);
                            this.InnerTextBox.Padding = new Thickness(left, top, right, bottom);
                        }
                    }
                    else if (uiElement is TextBoxFill)
                    {
                        TextBoxFill textBoxFill = (TextBoxFill)uiElement;
                        this.Properties.FillColor = textBoxFill.FillColor.Text;
                    }

                    else if (uiElement is TextBoxBorder)
                    {
                        TextBoxBorder textboxborder = (TextBoxBorder)uiElement;
                        this.Properties.BorderColors.DefaultBorderColor = textboxborder.borderColor.Text;
                        this.Properties.BorderStyles.DefaultBorderStyle = textboxborder.cmb_style.Text;
                        this.Properties.BorderWidths.DefaultBorderWidth = textboxborder.updwn_Custom.Text;
                    }
                    else if (uiElement is TextBoxFont)
                    {
                        this.InnerTextBox.Selection.Select(InnerTextBox.Document.ContentStart, InnerTextBox.Document.ContentEnd);

                        TextBoxFont textBoxFont = (TextBoxFont)uiElement;
                        string fSize = removePointFromString(textBoxFont.cmb_FontSize.TextValue);
                        this.Properties.FontSize = textBoxFont.cmb_FontSize.Text;
                        this.Properties.FontFamily = textBoxFont.cmb_FontName.TextValue;
                        this.Properties.FontColor = textBoxFont.cpkr_Font.Text;
                        this.Properties.FontStyle = (bool)textBoxFont.chk_Italic.IsChecked ? FontStyles.Italic.ToString() : this.Properties.FontStyle;
                        this.Properties.FontWeight = ((bool)textBoxFont.chk_Bold.IsChecked) ? FontWeights.Bold.ToString() : this.Properties.FontWeight;
                        this.Properties.FontEffects = textBoxFont.cmb_Effects.Text;
                        double lineHeight = 1;
                        if (!fSize.StartsWith("="))
                            lineHeight = (bool)textBoxFont.LineSpacingFontSize.IsChecked
                                                                    ? (double)new DoubleConverter().ConvertFromInvariantString(fSize)
                                                                    : (double)textBoxFont.LineSpacingCustomValue.Value;
                        this.Properties.LineSpacing = lineHeight.ToString() + "pt";
                    }
                    else if (uiElement is TextBoxVisibility)
                    {
                        TextBoxVisibility textBoxVisibility = (TextBoxVisibility)uiElement;

                        if ((bool)textBoxVisibility.Hide.IsChecked)
                        {
                            this.Properties.Hidden = "True";
                        }
                        else
                        {
                            this.Properties.Hidden = "False";
                        }
                            
                    }
                    else if (uiElement is ControlAction)
                    {
                        ControlAction textBoxAction = (ControlAction)uiElement;

                        if (this.Action == null)
                        {
                            this.Action = new RDL.DOM.Action();
                        }
                        if (textBoxAction.rbtn_goToReport.IsChecked == true)
                        {
                            if (this.Action.Drillthrough == null)
                            {
                                this.Action.Drillthrough = new RDL.DOM.Drillthrough();
                            }

                            this.Action.Drillthrough.Parameters = controlProperties.Parameters;
                            this.Action.Drillthrough.ReportName = textBoxAction.txt_reportName.Text;
                            this.Action.Hyperlink = null;
                        }
                        else if (textBoxAction.rbtn_goToUrl.IsChecked == true)
                        {
                            this.Action.Hyperlink = textBoxAction.cmbx_UrlPath.Text;
                            this.Action.Drillthrough = null;
                        }
                    }
                }
                InnerTextBox.Selection.Select(InnerTextBox.Selection.End, InnerTextBox.Selection.End);
            }

        }

        private void setRunProperty(ControlProperties controlProperties)
        {
            if (controlProperties != null && controlProperties.grd_PlaceHolder != null)
            {
                try
                {
                    foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                    {
                        //if (uiElement is TextBoxGeneral)
                        //{
                        //    TextBoxGeneral textBoxGeneral = (TextBoxGeneral)uiElement;

                        //    // if it is PlaceHolder
                        //    if (!(textBoxGeneral.stpnl_Value.Visibility == System.Windows.Visibility.Hidden))
                        //    {
                        //        string Name = controlProperties.TextBoxGeneral.Name.Text;

                        //        if (textBoxGeneral.Value.SelectedItem != null && textBoxGeneral.Value.SelectedItem.ToString() != string.Empty)
                        //        {
                        //            string value = controlProperties.TextBoxGeneral.Value.SelectedItem.ToString();
                        //            if (Name != null && Name.Trim() != string.Empty)
                        //            {
                        //                run.Text = "[" + Name + "]";
                        //            }
                        //            else
                        //            {
                        //                if (value.Contains("\""))
                        //                {
                        //                    run.Text = "<<Expr>>";
                        //                }
                        //                else
                        //                {
                        //                    run.Text = GetTextRunLabel(value);
                        //                }
                        //            }
                        //            run.Tag = value;
                        //        }
                        //    }
                        //}
                        //else if (uiElement is TextBoxAlignment)
                        if (uiElement is TextBoxAlignment)
                        {
                            TextBoxAlignment textBoxAlignment = (TextBoxAlignment)uiElement;

                            switch (textBoxAlignment.HorizontalValue.SelectedItem.ToString())
                            {
                                case "Left":
                                    {
                                        this.InnerTextBox.Selection.ApplyPropertyValue(System.Windows.Documents.Paragraph.TextAlignmentProperty, System.Windows.TextAlignment.Left);
                                        break;
                                    }
                                case "Right":
                                    {
                                        this.InnerTextBox.Selection.ApplyPropertyValue(System.Windows.Documents.Paragraph.TextAlignmentProperty, System.Windows.TextAlignment.Right);
                                        break;
                                    }
                                case "Center":
                                    {
                                        this.InnerTextBox.Selection.ApplyPropertyValue(System.Windows.Documents.Paragraph.TextAlignmentProperty, System.Windows.TextAlignment.Center);
                                        break;
                                    }
                                default:
                                    {
                                        this.InnerTextBox.Selection.ApplyPropertyValue(System.Windows.Documents.Paragraph.TextAlignmentProperty, System.Windows.TextAlignment.Left);
                                        break;
                                    }
                            }
                        }
                        else if (uiElement is TextBoxFill)
                        {
                            TextBoxFill textBoxFill = (TextBoxFill)uiElement;
                            if (!textBoxFill.FillColor.Text.StartsWith("="))
                                this.InnerTextBox.Selection.ApplyPropertyValue(Inline.BackgroundProperty, new BrushConverter().ConvertFromInvariantString(textBoxFill.FillColor.Text));
                        }
                        else if (uiElement is TextBoxFont)
                        {
                            TextBoxFont textBoxFont = (TextBoxFont)uiElement;
                            string fSize = "1";

                            if (!textBoxFont.cmb_FontSize.TextValue.StartsWith("="))
                            {
                                fSize = new RDL.DOM.Size(textBoxFont.cmb_FontSize.TextValue).PixelValue.ToString();
                                this.InnerTextBox.Selection.ApplyPropertyValue(TextBoxControl.FontSizeProperty,
                                    new FontSizeConverter().ConvertFromInvariantString(fSize));
                                fSize = removePointFromString(textBoxFont.cmb_FontSize.TextValue);
                            }
                            else
                            {
                                this.Properties.FontSize = textBoxFont.cmb_FontSize.TextValue;
                            }
                            if (!textBoxFont.cmb_FontName.Text.StartsWith("="))
                            {
                                this.InnerTextBox.Selection.ApplyPropertyValue(TextBoxControl.FontFamilyProperty,
                                    new FontFamilyConverter().ConvertFromInvariantString(textBoxFont.cmb_FontName.Text));
                            }
                            else
                            {
                                this.Properties.FontFamily = textBoxFont.cmb_FontName.Text;
                            }
                            if (!textBoxFont.cpkr_Font.Text.StartsWith("="))
                            {
                                this.InnerTextBox.Selection.ApplyPropertyValue(TextBoxControl.ForegroundProperty,
                                    new BrushConverter().ConvertFromInvariantString(textBoxFont.cpkr_Font.Text));
                            }
                            else
                            {
                                this.Properties.FontColor = textBoxFont.cpkr_Font.Text;
                            }
                            this.InnerTextBox.Selection.ApplyPropertyValue(TextBoxControl.FontStyleProperty,
                                new FontStyleConverter().ConvertFromInvariantString((bool)textBoxFont.chk_Italic.IsChecked ? FontStyles.Italic.ToString() : FontStyles.Normal.ToString()));
                            this.InnerTextBox.Selection.ApplyPropertyValue(TextBoxControl.FontWeightProperty,
                                new FontWeightConverter().ConvertFromInvariantString((bool)textBoxFont.chk_Bold.IsChecked ? FontWeights.Bold.ToString() : FontWeights.Normal.ToString()));
                            string textDecoration = EffectsType.None.ToString();
                            if (!textBoxFont.cmb_Effects.Text.StartsWith("=") && textBoxFont.cmb_Effects.SelectedItem != null)
                            {
                                textDecoration = textBoxFont.cmb_Effects.SelectedItem.ToString() == EffectsType.Default.ToString()
                                                                                                ? EffectsType.None.ToString() : textBoxFont.cmb_Effects.SelectedItem.ToString();
                                this.InnerTextBox.Selection.ApplyPropertyValue(System.Windows.Controls.TextBox.TextDecorationsProperty, new TextDecorationCollectionConverter().ConvertFromInvariantString(textDecoration));
                            }
                            else
                            {
                                this.Properties.FontEffects = textBoxFont.cmb_Effects.Text;
                            }

                            double lineHeight = 0;

                            if (textBoxFont.LineSpacingFontSize.IsChecked == true && !string.IsNullOrEmpty(fSize))
                            {
                                lineHeight = (double)new DoubleConverter().ConvertFromInvariantString(fSize);
                            }
                            else
                            {
                                lineHeight = (double)textBoxFont.LineSpacingCustomValue.Value;
                            }
                            if (!this.InnerTextBox.Selection.IsEmpty)
                            {
                                this.InnerTextBox.Selection.ApplyPropertyValue(AccessText.LineHeightProperty, lineHeight);
                            }
                        }
                        else if (uiElement is ControlAction)
                        {
                            ControlAction textBoxAction = (ControlAction)uiElement;

                            if (this.Action == null)
                            {
                                this.Action = new RDL.DOM.Action();
                            }
                            if (textBoxAction.rbtn_goToReport.IsChecked == true)
                            {
                                if (this.Action.Drillthrough == null)
                                {
                                    this.Action.Drillthrough = new RDL.DOM.Drillthrough();
                                }

                                this.Action.Drillthrough.Parameters = controlProperties.Parameters;
                                this.Action.Drillthrough.ReportName = textBoxAction.txt_reportName.Text;
                                this.Action.Hyperlink = null;
                            }
                            else if (textBoxAction.rbtn_goToUrl.IsChecked == true)
                            {
                                this.Action.Hyperlink = textBoxAction.cmbx_UrlPath.Text;
                                this.Action.Drillthrough = null;
                            }
                        }
                    }
                }
                catch { }
            }
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            if (this.InnerTextBox != null)
            {
                if (!double.IsInfinity(constraint.Width))
                    this.InnerTextBox.Width = constraint.Width;
                if (!double.IsInfinity(constraint.Height))
                    this.InnerTextBox.Height = constraint.Height;
            }
            return base.MeasureOverride(constraint);
        }


        private string removePointFromString(string fontSize)
        {
            if (fontSize != null)
            {
                fontSize = fontSize.Trim();
                if (fontSize.EndsWith("pt"))
                    fontSize = fontSize.Substring(0, fontSize.Length - 2);
            }
            else
                fontSize = "12";
            return fontSize;
        }

        public Run getRunFromTextRun(TextRun textRun)
        {

            Run run = new Run();
            string setText = textRun.Value;

            if (setText.StartsWith("="))
            {
                if (textRun.InternalLabel == null || (textRun.InternalLabel != null && textRun.InternalLabel == string.Empty))
                {
                    textRun.InternalLabel = GetTextRunLabel(setText);
                    run.Text = textRun.InternalLabel;
                }
                else if (!textRun.InternalLabel.StartsWith("[") && !textRun.InternalLabel.EndsWith("]"))
                {
                    run.Text = "[" + textRun.InternalLabel + "]";
                }
                else
                {
                    run.Text = textRun.InternalLabel;
                }

                run.Tag = textRun.Value;
            }
            else // if it is normal text
            {
                run.Text = textRun.Value;
            }

            if (textRun.Style != null)
            {
                if (textRun.Style.FontFamily != null && textRun.Style.FontFamily != "Default")
                {
                    this.Properties.FontFamily = textRun.Style.FontFamily;
                    if (!textRun.Style.FontFamily.StartsWith("="))
                    {
                        run.FontFamily = new FontFamily(this.propertyValueConvertor.GetFontFamily(textRun.Style.FontFamily));
                    }
                }
                if (textRun.Style.FontSize != null && textRun.Style.FontSize.size != null)
                {
                    this.Properties.FontSize = textRun.Style.FontSize.size;
                    if (!textRun.Style.FontSize.size.StartsWith("="))
                    {
                        run.FontSize = textRun.Style.FontSize.PixelValue;
                    }
                }
                if (textRun.Style.FontWeight != null && textRun.Style.FontWeight != RDL.DOM.FontWeight.Default.ToString())
                {
                    this.Properties.FontWeight = textRun.Style.FontWeight;
                    if (!textRun.Style.FontWeight.StartsWith("="))
                    {
                        run.FontWeight = (System.Windows.FontWeight)new FontWeightConverter().ConvertFromInvariantString(textRun.Style.FontWeight);
                    }
                }
                if (textRun.Style.FontStyle != null && textRun.Style.FontStyle != RDL.DOM.FontStyle.Default.ToString())
                {
                    this.Properties.FontStyle = textRun.Style.FontStyle;
                    run.FontStyle = this.propertyValueConvertor.GetFontStyle(textRun.Style.FontStyle);
                }
                if (textRun.Style.TextDecoration != null)
                {
                    switch (textRun.Style.TextDecoration)
                    {
                        case "Default":
                            {
                                run.TextDecorations = (TextDecorationCollection)new TextDecorationCollectionConverter()
                                    .ConvertFromInvariantString(Syncfusion.RDL.DOM.TextDecoration.None.ToString());
                                break;
                            }
                        case "LineThrough":
                            {
                                run.TextDecorations = (TextDecorationCollection)new TextDecorationCollectionConverter()
                                    .ConvertFromInvariantString("StrikeThrough");
                                break;
                            }
                        case "None":
                            {
                                run.TextDecorations = (TextDecorationCollection)new TextDecorationCollectionConverter()
                                .ConvertFromInvariantString(Syncfusion.RDL.DOM.TextDecoration.None.ToString());
                                break;
                            }
                        case "Overline":
                            {
                                run.TextDecorations = (TextDecorationCollection)new TextDecorationCollectionConverter()
                                .ConvertFromInvariantString(Syncfusion.RDL.DOM.TextDecoration.Overline.ToString());
                                break;
                            }
                        case "Underline":
                            {
                                run.TextDecorations = (TextDecorationCollection)new TextDecorationCollectionConverter()
                                    .ConvertFromInvariantString(Syncfusion.RDL.DOM.TextDecoration.Underline.ToString());
                                break;
                            }
                        default:
                            {
                                run.TextDecorations = (TextDecorationCollection)new TextDecorationCollectionConverter()
                                .ConvertFromInvariantString(Syncfusion.RDL.DOM.TextDecoration.None.ToString());
                                break;
                            }
                    }
                }

                if (textRun.Style.TextDecoration != null)
                {
                    this.Properties.FontEffects = textRun.Style.TextDecoration;
                    if (!textRun.Style.TextDecoration.StartsWith("="))
                    {
                        if (textRun.Style.TextDecoration != null && textRun.Style.TextDecoration == "LineThrough")
                        {
                            this.Properties.FontEffects = "StrikeThrough";
                        }
                        else
                        {
                            this.Properties.FontEffects = Enum.Parse(typeof(Syncfusion.RDL.DOM.TextDecoration), textRun.Style.TextDecoration, true).ToString();
                        }
                    }
                }
                if (textRun.Style.Format != null)
                {
                    this.Properties.Format = textRun.Style.Format;
                }
                if (textRun.Style.Color != null)
                {
                    this.Properties.FontColor = textRun.Style.Color;
                    if (!textRun.Style.Color.StartsWith("="))
                    {
                        run.Foreground = (Brush)new BrushConverter().ConvertFromInvariantString(textRun.Style.Color);
                    }
                }
                if (textRun.Style.BackgroundColor != null)
                {
                    if (!textRun.Style.BackgroundColor.StartsWith("="))
                    {
                        run.Background = (Brush)new BrushConverter().ConvertFromInvariantString(textRun.Style.BackgroundColor);
                    }
                }
            }

            return run;
        }

        public string GetTextRunLabel(string setText)
        {
            if (setText.StartsWith("="))
            {
                if (setText.StartsWith("=Globals!", true, null) || setText.StartsWith("=User!", true, null))
                {
                    setText = setText.Replace("=Globals!", "").Replace("=User!", "").Replace("\n", "");

                    if (setText == "ExecutionTime" || setText == "ReportFolder"
                        || setText == "ReportName" || setText == "ReportServerUrl"
                        || setText == "PageNumber" || setText == "TotalPages")
                    {
                        setText = "[&" + setText + "]";
                    }
                    else if (setText == "UserID" || setText == "Language")
                    {
                        setText = "[&" + setText + "]";
                    }
                }
                else if (setText.StartsWith("=Fields!", true, null) || setText.StartsWith("=Sum", true, null)
                    || setText.StartsWith("=Count", true, null) || setText.StartsWith("=First", true, null)
                    || setText.StartsWith("=Avg", true, null))
                {
                    if (setText.Contains('!') && setText.Contains('.') && !setText.Contains("\""))
                    {
                        int indexOfExclamatory = setText.IndexOf('!');
                        int indexOfDot = setText.IndexOf('.');
                        int stringLength = (indexOfDot - indexOfExclamatory) - 1;
                        int startingIndex = indexOfExclamatory + 1;

                        // if it is normal fields 
                        if (setText.StartsWith("=Fields!", true, null))
                        {
                            setText = setText.Substring(startingIndex, stringLength);
                            setText = "[" + setText + "]";
                        }
                        // if it is sum
                        else if (setText.StartsWith("=Sum", true, null))
                        {
                            setText = setText.Substring(startingIndex, stringLength);
                            setText = "[Sum(" + setText + ")]";
                        }
                        // if it is count
                        else if (setText.StartsWith("=Count", true, null))
                        {
                            setText = setText.Substring(startingIndex, stringLength);
                            setText = "[Count(" + setText + ")]";
                        }
                        // if it is First
                        else if (setText.StartsWith("=First", true, null))
                        {
                            setText = setText.Substring(startingIndex, stringLength);
                            setText = "[First(" + setText + ")]";
                        }
                        else if (setText.StartsWith("=Avg", true, null))
                        {
                            setText = setText.Substring(startingIndex, stringLength);
                            setText = "[Avg(" + setText + ")]";
                        }
                    }
                    else
                    {
                        setText = "<<Expr>>";
                    }
                }
                else // The expressions which we are not currently supported
                {
                    setText = "<<Expr>>";
                }
            }

            return setText;
        }

        #endregion
  
    }
}