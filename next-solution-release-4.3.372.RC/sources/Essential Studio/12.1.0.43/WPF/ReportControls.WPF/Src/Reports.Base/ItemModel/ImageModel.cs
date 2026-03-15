//-------------------------------------------------------------------------------------------------
// <copyright file="PageModelTextbox.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.DOM;

#if !SILVERLIGHT
using System.Data;
using Syncfusion.RDL.Controls;
#endif


namespace Syncfusion.RDL.ItemModel
{
    /// <summary>
    /// A model for TextboxModel. Contains information about size, position, data and style for a text box.
    /// </summary>
    internal class ImageModel
        : ReportItemModeler
    {
        #region members

        ImagePropertiesExp ImagePropExp;

        #endregion

        #region  properties

        public ImagePropertiesExpVal ImageProperties
        {
            get;
            set;
        }

        internal int PrintPageColumnCount
        {
            get;
            set;
        }

        internal Dictionary<int, PageInfo> PageSizes
        {
            get;
            set;
        }

        internal List<double> PageWidths
        {
            get;
            set;
        }

        internal Dictionary<int, PageInfo> PrintPageSizes
        {
            get;
            set;
        }

        internal List<double> PrintPageWidths
        {
            get;
            set;
        }

        internal object ImageData
        {
            get;
            set;
        }

        internal ImageFormats ImageFormat
        {
            get;
            set;
        }

#if WINRT
        internal Windows.UI.Xaml.Media.Stretch ImageSize
        {
            get;
            set;
        }
#else
        internal System.Windows.Media.Stretch ImageSize
        {
            get;
            set;
        }
#endif

        #endregion

        #region Constructors

        internal ImageModel()
        {
        }

        internal ImageModel(ReportItem reportItem, ReportModel pageModel, bool isTablixChild, string dataSetName)
        {
            this.DataSetName = dataSetName;
            this.Model = pageModel;
            this.ReportItem = reportItem;
            this.ModelType = ModelType.ImageModel;
            this.Name = reportItem.Name;
            this.IsTablixChild = isTablixChild;

            if (this.IsTablixChild)
            {
                this.DataSetFields = new List<DataField>();
            }
            if (reportItem.Top != null)
            {
                this.Top = reportItem.Top.PixelValue;
            }

            if (reportItem.Left != null)
            {
                this.Left = reportItem.Left.PixelValue;
            }

            if (reportItem.Width != null)
            {
                this.Width = reportItem.Width.PixelValue;
            }

            if (reportItem.Height != null)
            {
                this.Height = reportItem.Height.PixelValue;
            }

            this.ParseImage();
        }

        void ParseImage()
        {
            Image Image = this.ReportItem as Image;
            this.ImagePropExp = new ImagePropertiesExp();
            RDL.DOM.Style style = Image.Style;
            this.ImagePropExp.BookMark = this.GetExpressionKey(Image.Bookmark);

            this.ImagePropExp.DocumentMapLabel = this.GetExpressionKey(Image.DocumentMapLabel);
            this.DocumentMapLable = this.ImagePropExp.DocumentMapLabel;

            if (Image.Visibility != null && Image.Visibility.Hidden != null)
            {
                this.ImagePropExp.Hidden = this.GetExpressionKey(Image.Visibility.Hidden);
                this.ToggleItem = Image.Visibility.ToggleItem;
            }

            this.ImagePropExp.Border = new BorderExp();
            if (style.Border != null)
            {
                this.ImagePropExp.Border.Default = new BorderExpProperties();
                this.ImagePropExp.Border.Default.BorderBrush = this.GetExpressionKey(style.Border.Color);
                this.ImagePropExp.Border.Default.BorderStyle = this.GetExpressionKey(style.Border.Style);
                if (style.Border.Width != null)
                {
                    this.ImagePropExp.Border.Default.Thickness = this.GetExpressionKey(style.Border.Width.size);
                }
            }
            if (style.BottomBorder != null)
            {
                this.ImagePropExp.Border.BottomBorder = new BorderExpProperties();
                this.ImagePropExp.Border.BottomBorder.BorderBrush = this.GetExpressionKey(style.BottomBorder.Color);
                this.ImagePropExp.Border.BottomBorder.BorderStyle = this.GetExpressionKey(style.BottomBorder.Style);

                if (style.BottomBorder.Width != null)
                {
                    this.ImagePropExp.Border.BottomBorder.Thickness = this.GetExpressionKey(style.BottomBorder.Width.size);
                }
            }
            if (style.LeftBorder != null)
            {
                this.ImagePropExp.Border.LeftBorder = new BorderExpProperties();
                this.ImagePropExp.Border.LeftBorder.BorderBrush = this.GetExpressionKey(style.LeftBorder.Color);
                this.ImagePropExp.Border.LeftBorder.BorderStyle = this.GetExpressionKey(style.LeftBorder.Style);

                if (style.LeftBorder.Width != null)
                {
                    this.ImagePropExp.Border.LeftBorder.Thickness = this.GetExpressionKey(style.LeftBorder.Width.size);
                }
            }
            if (style.RightBorder != null)
            {
                this.ImagePropExp.Border.RightBorder = new BorderExpProperties();
                this.ImagePropExp.Border.RightBorder.BorderBrush = this.GetExpressionKey(style.RightBorder.Color);
                this.ImagePropExp.Border.RightBorder.BorderStyle = this.GetExpressionKey(style.RightBorder.Style);

                if (style.RightBorder.Width != null)
                {
                    this.ImagePropExp.Border.RightBorder.Thickness = this.GetExpressionKey(style.RightBorder.Width.size);
                }
            }
            if (style.TopBorder != null)
            {
                this.ImagePropExp.Border.TopBorder = new BorderExpProperties();
                this.ImagePropExp.Border.TopBorder.BorderBrush = this.GetExpressionKey(style.TopBorder.Color);
                this.ImagePropExp.Border.TopBorder.BorderStyle = this.GetExpressionKey(style.TopBorder.Style);

                if (style.TopBorder.Width != null)
                {
                    this.ImagePropExp.Border.TopBorder.Thickness = this.GetExpressionKey(style.TopBorder.Width.size);
                }
            }

            this.ImagePropExp.Padding = new ThicknessExp();
            this.ImagePropExp.Padding.Bottom = this.GetExpressionKey(style.PaddingBottom.size);
            this.ImagePropExp.Padding.Left = this.GetExpressionKey(style.PaddingLeft.size);
            this.ImagePropExp.Padding.Right = this.GetExpressionKey(style.PaddingRight.size);
            this.ImagePropExp.Padding.Top = this.GetExpressionKey(style.PaddingTop.size);
            this.ImagePropExp.MIMEType = this.GetExpressionKey(Image.MIMEType);

            this.ImagePropExp.ToolTip = this.GetExpressionKey(Image.ToolTip);
            this.ImagePropExp.Value = this.GetExpressionKey(Image.Value);
            this.ImagePropExp.ZIndex = this.GetExpressionKey(Image.ZIndex.ToString());

            if (Image.ActionInfo != null)
            {
                this.ImagePropExp.ImageActionInfo = new ImageActionInfoExp();
                foreach (RDL.DOM.Action Action in Image.ActionInfo.Actions)
                {
                    this.ImagePropExp.ImageActionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);
                    this.ImagePropExp.ImageActionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);

                    if (Action.Drillthrough != null)
                    {
                        this.ImagePropExp.ImageActionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);
                        this.ImagePropExp.ImageActionInfo.Parameters=new List<ImageParameterExp>();
                        foreach (Parameter parameters in Action.Drillthrough.Parameters)
                        {
                            ImageParameterExp Parameter = new ImageParameterExp();
                            Parameter.Name = this.GetExpressionKey(parameters.Name);
                            Parameter.Omit = this.GetExpressionKey(parameters.Omit);
                            Parameter.Value = this.GetExpressionKey(parameters.Value);
                            this.ImagePropExp.ImageActionInfo.Parameters.Add(Parameter);
                        }
                    }
                }
            }
        }

        private string GetExpressionKey(string value)
        {
            if (this.IsTablixChild && value != null && value.Contains("RowNumber("))
            {
                this.HasRowNumber = true;
            }

            string key = this.Model.ExpressionEngine.GetExpressionKey(value, this.DataSetName,this.IsTablixChild);
            this.AddDataFields(key);
            return key;
        }

        private void AddDataFields(string key)
        {
            if (this.IsTablixChild)
            {
                this.Model.ExpressionEngine.AddTablixDataFields(key, this);
            }
        }

        void IntializeEngineValues()
        {
            if (this.RowNumbers != null)
                this.Model.ExpressionEngine.RowNumbers = this.RowNumbers;
            if (this.GroupLevels != null)
                this.Model.ExpressionEngine.GroupLevels = this.GroupLevels;

            if (this.FieldValues != null)
            {
                foreach (var value in this.FieldValues)
                {
                    this.Model.ExpressionEngine.FieldValues.Add(value.Name, value.Value.GetResult());
                }
            }
            else if (this.DataSetFields != null && this.DataSource != null)
            {
                ReportingAggEngine engine = new ReportingAggEngine();
                engine.Fields = this.DataSetFields.Where(field => !(field.IsRecursiveField || field.IsDataSetField)).ToList();

                if (engine.Fields.Count > 0)
                {
                    List<object> datas = new List<object>();
                    engine.DataSource = this.DataSource;
                    engine.Model = this.Model;

                    engine.PopulateValue();

                    foreach (DataField field in engine.Fields)
                    {
                        this.Model.ExpressionEngine.FieldValues.Add(field.Name, field.Value.Value.GetResult());
                    }
                }

                engine.DisposeEngine();
            }
        }


        #endregion

#if !SILVERLIGHT

        internal Stream GetImageStream()
        {
            Stream source =  new ImageConversion().CovertToImage(new ReportingImage(this));
            return source;
        }
#endif

        #region overridden methods

        public override void Evaluate()
        {
            this.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();
            this.IntializeEngineValues();
            try
            {
                this.ImageProperties = new ImagePropertiesExpVal();
                this.ImageProperties.BookMark = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.BookMark);
                this.ImageProperties.DocumentMapLabel = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.DocumentMapLabel);
                this.DocumentMapLable = this.ImageProperties.DocumentMapLabel;
                if (!string.IsNullOrEmpty(this.DocumentMapLable) && !this.IsTablixChild)
                {
                    this.SetTreeModel();
                }
                if (!string.IsNullOrEmpty(this.ToggleItem))
                {
                    this.GetTextBoxModel(this.ToggleItem);
                }
                if (this.ImagePropExp.Hidden != null)
                {
                    this.ImageProperties.Hidden = bool.Parse(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Hidden));
                    this.Hidden = this.ImageProperties.Hidden;
                }
                else
                {
                    this.Hidden = this.ImageProperties.Hidden;
                }

                this.ImageProperties.Border = new BorderExpval();
                if (this.ImagePropExp.Border.Default != null)
                {
                    this.ImageProperties.Border.Default = new BorderExpvalProperties();
                    this.ImageProperties.Border.Default.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.Default.BorderBrush);

                    if (this.ImagePropExp.Border.Default.BorderStyle != null)
                    {
                        this.ImageProperties.Border.Default.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.Default.BorderStyle), true);
                    }
                    if (this.ImagePropExp.Border.Default.Thickness != null)
                    {
                        this.ImageProperties.Border.Default.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.Default.Thickness)).FloatValue;
                    }
                }
                if (this.ImagePropExp.Border.BottomBorder != null)
                {
                    this.ImageProperties.Border.BottomBorder = new BorderExpvalProperties();
                    this.ImageProperties.Border.BottomBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.BottomBorder.BorderBrush);

                    if (this.ImagePropExp.Border.BottomBorder.Thickness != null)
                    {
                        this.ImageProperties.Border.BottomBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.BottomBorder.Thickness)).FloatValue;
                    }
                    if (this.ImagePropExp.Border.BottomBorder.BorderStyle != null)
                    {
                        this.ImageProperties.Border.BottomBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.BottomBorder.BorderStyle), true);
                    }
                }
                if (this.ImagePropExp.Border.LeftBorder != null)
                {
                    this.ImageProperties.Border.LeftBorder = new BorderExpvalProperties();
                    this.ImageProperties.Border.LeftBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.LeftBorder.BorderBrush);

                    if (this.ImagePropExp.Border.LeftBorder.Thickness != null)
                    {
                        this.ImageProperties.Border.LeftBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.LeftBorder.Thickness)).FloatValue;
                    }
                    if (this.ImagePropExp.Border.LeftBorder.BorderStyle != null)
                    {
                        this.ImageProperties.Border.LeftBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.LeftBorder.BorderStyle), true);
                    }
                }
                if (this.ImagePropExp.Border.RightBorder != null)
                {
                    this.ImageProperties.Border.RightBorder = new BorderExpvalProperties();
                    this.ImageProperties.Border.RightBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.RightBorder.BorderBrush);

                    if (this.ImagePropExp.Border.RightBorder.Thickness != null)
                    {
                        this.ImageProperties.Border.RightBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.RightBorder.Thickness)).FloatValue;
                    }
                    if (this.ImagePropExp.Border.RightBorder.BorderStyle != null)
                    {
                        this.ImageProperties.Border.RightBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.RightBorder.BorderStyle), true);
                    }
                }
                if (this.ImagePropExp.Border.TopBorder != null)
                {
                    this.ImageProperties.Border.TopBorder = new BorderExpvalProperties();
                    this.ImageProperties.Border.TopBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.TopBorder.BorderBrush);

                    if (this.ImagePropExp.Border.TopBorder.Thickness != null)
                    {
                        this.ImageProperties.Border.TopBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.TopBorder.Thickness)).FloatValue;
                    }
                    if (this.ImagePropExp.Border.TopBorder.BorderStyle != null)
                    {
                        this.ImageProperties.Border.TopBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Border.TopBorder.BorderStyle), true);
                    }
                }

                this.ImageProperties.Padding = new ThicknessExpval();
                this.ImageProperties.Padding.Bottom = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Padding.Bottom)).FloatValue;
                this.ImageProperties.Padding.Left = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Padding.Left)).FloatValue;
                this.ImageProperties.Padding.Right = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Padding.Right)).FloatValue;
                this.ImageProperties.Padding.Top = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Padding.Top)).FloatValue;
                this.ImageProperties.MIMEType = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.MIMEType);
                this.ImageProperties.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.ToolTip);
                this.ImageProperties.Value = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Value);
                this.ImageProperties.ZIndex = int.Parse(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.ZIndex));

                Image image = this.ReportItem as Image;
                if (image.Source == Source.Embedded)
                {
                    foreach (EmbeddedImage embeddedImage in this.Model.Report.EmbeddedImages)
                    {
                        if (embeddedImage.Name.Contains(this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.Value)))
                        {
                            this.ImageData = System.Convert.FromBase64String(embeddedImage.ImageData);
                            this.ImageFormat = this.PopulateImageFormatFromString(embeddedImage.MIMEType);
                        }
                    }
                }
                else if ((this.ReportItem as Image).Source == Source.Database)
                {
                    ImageData = this.Model.ExpressionEngine.GetEvalExpression(this.ImagePropExp.Value);
                }
#if !SILVERLIGHT
                else if (image.Source == Source.External)
                {
                    string imagePath = this.Model.ExpressionEngine.GetEvalExpressionString(image.Value);
                    string reportPath = this.Model.ReportPath;

                    if (!System.IO.Path.IsPathRooted(imagePath) && !string.IsNullOrEmpty(reportPath))
                    {
                        imagePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(reportPath), imagePath);
                    }

                    byte[] binaryData = null;



                    try
                    {
                        if (image.Value.StartsWith("http:/"))
                        {
                            var webClient = new System.Net.WebClient();
                            binaryData = webClient.DownloadData(image.Value);
                        }

                        else
                        {
                            FileStream inFile = new System.IO.FileStream(imagePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                            binaryData = new Byte[inFile.Length];
                            long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);
                            inFile.Close();
                        }
                    }
                    catch { }

                    if (imagePath.ToLower().Trim().Contains(".emf"))
                    {
                        this.ImageFormat = ImageFormats.Emf;
                    }

                    ImageData = binaryData;
                }
#endif

#if WINRT
            switch (image.Sizing)
            {
                case DOM.Sizing.AutoSize:
                case DOM.Sizing.Clip:
                    this.ImageSize = Windows.UI.Xaml.Media.Stretch.None;
                    break;
                case DOM.Sizing.Fit:
                    this.ImageSize = Windows.UI.Xaml.Media.Stretch.Fill;
                    break;
                case DOM.Sizing.FitProportional:
                    this.ImageSize = Windows.UI.Xaml.Media.Stretch.Uniform;
                    break;
                default:
                    this.ImageSize = Windows.UI.Xaml.Media.Stretch.Fill;
                    break;
            }
#else
                switch (image.Sizing)
                {
                    case DOM.Sizing.AutoSize:
                    case DOM.Sizing.Clip:
                        this.ImageSize = System.Windows.Media.Stretch.None;
                        break;
                    case DOM.Sizing.Fit:
                        this.ImageSize = System.Windows.Media.Stretch.Fill;
                        break;
                    case DOM.Sizing.FitProportional:
                        this.ImageSize = System.Windows.Media.Stretch.Uniform;
                        break;
                    default:
                        this.ImageSize = System.Windows.Media.Stretch.Fill;
                        break;
                }
#endif

                if (this.ImagePropExp.ImageActionInfo != null)
                {
                    this.ImageProperties.ImageActionInfo = new ImageActionInfoExpVal();
                    this.ImageProperties.ImageActionInfo.BookmarkLink = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.ImageActionInfo.BookmarkLink);
                    this.ImageProperties.ImageActionInfo.Hyperlink = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.ImageActionInfo.Hyperlink);
                    this.ImageProperties.ImageActionInfo.ReportName = this.Model.ExpressionEngine.GetEvalExpressionString(this.ImagePropExp.ImageActionInfo.ReportName);
                    if (this.ImagePropExp.ImageActionInfo.Parameters != null)
                    {
                        this.ImageProperties.ImageActionInfo.Parameters = new List<ImageParameterExpVal>();
                        foreach (var parameters in this.ImagePropExp.ImageActionInfo.Parameters)
                        {
                            ImageParameterExpVal Parameter = new ImageParameterExpVal();
                            Parameter.Name = this.Model.ExpressionEngine.GetEvalExpressionString(parameters.Name);
                            Parameter.Omit = this.Model.ExpressionEngine.GetEvalExpressionString(parameters.Omit);
                            Parameter.Value = this.Model.ExpressionEngine.GetEvalExpressionString(parameters.Value);
                            this.ImageProperties.ImageActionInfo.Parameters.Add(Parameter);
                        }
                    }
                }

                base.Evaluate();
            }
            catch (Exception e)
            {
                this.Model.ExceptionDetails.Add("Getting following exception while evaluate the expression in " + this.ReportItem.Name + " " + e.Message);
            }
        }


        private ImageFormats PopulateImageFormatFromString(string extension)
        {
            switch (extension.ToLower())
            {
                case "image/png":
                    return ImageFormats.Png;
                case "image/jpeg":
                case "image/jpg":
                case "image/jpe":
                    return ImageFormats.Jpeg;
                case "image/bmp":
                    return ImageFormats.Bmp;
                case "image/gif":
                    return ImageFormats.Gif;
                case "image/emf":
                    return ImageFormats.Emf;
            }

            return ImageFormats.Bmp;
        }

        private void SetTreeModel()
        {
            if (this.Model.MapModel == null)
            {
                this.Model.MapModel = new DocumentMapModel();
            }
            DocumentData node = new DocumentData();
            node.DocumentLable = this.DocumentMapLable;
            node.ModelType = ModelType.ImageModel;
            node.ReportItemName = this.Name;
            node.IsTablixChild = this.IsTablixChild;
            this.DocumentNodeRefer = node;
            this.Model.MapModel.NodeData.Add(node);
        }

        public override IReportItemModeler GetModel()
        {
            ImageModel imageModel = new ImageModel();
            imageModel.IsTablixChild = this.IsTablixChild;
            imageModel.IsTablixInnerChild = this.IsTablixInnerChild;
            imageModel.ModelType = this.ModelType;
            imageModel.Model = this.Model;
            imageModel.ReportItem = this.ReportItem;
            imageModel.Name = this.Name;
            imageModel.Top = this.Top;
            imageModel.Left = this.Left;
            imageModel.Width = this.Width;
            imageModel.Height = this.Height;
            imageModel.ImagePropExp = this.ImagePropExp;
            imageModel.Hidden = this.Hidden;
            imageModel.DataSetFields = this.DataSetFields;

            return imageModel;
        }

        public override void DisposeEvalObjects()
        {
            this.ImageProperties = null;
            base.DisposeEvalObjects();
        }

        public override void DisposeReportItemObj()
        {
            this.Model = null;
            this.ReportItem = null;
            this.ImageData = null;
            this.DataSetFields = null;
            this.DataSource = null;
            this.FieldValues = null;
            this.RowNumbers = null;

            if (this.IsTablixInnerChild)
            {
                this.FlowLayoutInfo = null;
            }
            base.DisposeReportItemObj();
        }

        public override void UpdateHeight(double firstPageHeight, LayoutReportItemModel locationInfo, double preferredHeight, ReportItemViewMode itemViewMode)
        {
            locationInfo.ActualHeight = this.Hidden ? 0 : this.Height;
        }

        public override void UpdateWidth(double firstPageWidth, LayoutReportItemModel locationInfo, double preferredWidth, ReportItemViewMode itemViewMode)
        {
            locationInfo.ActualWidth = this.Hidden ? 0 : this.Width;
        }

        public override void UpdatePageNo(int pageNo)
        {
            if (this.DocumentNodeRefer != null)
            {
                this.DocumentNodeRefer.PageNo=pageNo + 1;
                this.DocumentNodeRefer.TopPos = this.Top;
                this.DocumentNodeRefer.LeftPos = this.Left;
                this.DocumentNodeRefer = null;
            }
        }
        #endregion
    }
}