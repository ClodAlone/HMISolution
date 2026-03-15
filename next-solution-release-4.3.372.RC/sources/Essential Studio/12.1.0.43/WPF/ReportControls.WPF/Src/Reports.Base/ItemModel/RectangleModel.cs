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
using System.Text.RegularExpressions;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.DOM;

#if !SILVERLIGHT
using System.Data;
using System.Threading;
#endif


namespace Syncfusion.RDL.ItemModel
{
    /// <summary>
    /// A model for TextboxModel. Contains information about size, position, data and style for a text box.
    /// </summary>
    internal class RectangleModel
        : ReportItemModeler
    {
        private RectangleItemExp rectItemExpPro;

        #region  properties

        public RectangleItemExpVal RectItemExpPro
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

        internal Dictionary<int, PageInfo> FlowPageSizes
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        internal RectangleModel()
        {
        }

        internal RectangleModel(ReportItem reportItem, ReportModel pageModel, bool isTablixChild, string dataSetName)
        {
            this.DataSetName = dataSetName;
            this.IsTablixChild = isTablixChild;
            this.ModelType = ModelType.RectangleModel;
            this.Model = pageModel;
            this.ReportItem = reportItem;
            this.Name = reportItem.Name;

            if (this.IsTablixChild)
            {
                this.DataSetFields = new List<DataField>();
            }

            if (this.ReportItem.Top != null)
            {
                this.Top = this.ReportItem.Top.PixelValue;
            }

            if (this.ReportItem.Left != null)
            {
                this.Left = this.ReportItem.Left.PixelValue;
            }

            if (this.ReportItem.Width != null)
            {
                this.Width = this.ReportItem.Width.PixelValue;
            }

            if (this.ReportItem.Height != null)
            {
                this.Height = this.ReportItem.Height.PixelValue;
            }

            this.ReportItemModelers = new ReportModelContentCollection();

            Rectangle rectangle = this.ReportItem as Rectangle;
            this.KeepTogether = rectangle.KeepTogether;

            if (rectangle.PageBreak != null)
            {
                this.PageBreak = rectangle.PageBreak.BreakLocation;
            }

            if (rectangle.ReportItems != null)
            {
                foreach (var item in rectangle.ReportItems)
                {
                    var itemModel = this.Model.GetModel(item, isTablixChild,this.DataSetName);

                    if (IsTablixChild)
                    {
                        itemModel.IsTablixInnerChild = true;
                    }

                    if (itemModel.DataSetFields != null)
                    {
                        foreach (DataField field in itemModel.DataSetFields)
                        {
                            this.AddFields(field);
                        }
                    }

                    this.ReportItemModelers.Add(itemModel);

                    if ( !itemModel.IsTablixChild && (itemModel.ModelType == ModelType.RectangleModel))
                    {
                        foreach (IReportItemModeler childModel in itemModel.ReportItemModelers)
                        {
                            this.ReportItemModelers.Add(childModel);

                            if (childModel.DataSetFields != null)
                            {
                                foreach (DataField field in childModel.DataSetFields)
                                {
                                    this.AddFields(field);
                                }
                            }
                        }
                    }
                }

                foreach (var model in this.ReportItemModelers)
                {
                    if (model.ContainerModel == null)
                    {
                        model.ContainerModel = this;
                    }

                    if (this.KeepTogether)
                    {
                        model.KeepTogether = true;
                    }
                }
            }

            this.ParseRectangle();
        }

        #endregion

        #region Parse
        
        void ParseRectangle()
        {
            Rectangle rect = this.ReportItem as Rectangle;
            this.rectItemExpPro = new RectangleItemExp();

            if (rect.Style != null)
            {
                this.rectItemExpPro.BackgroundColor = this.GetExpressionKey(rect.Style.BackgroundColor);
                this.rectItemExpPro.Border = this.GetEdgesBorder(rect.Style);

                if (rect.Style.BackgroundImage != null)
                {
                    this.rectItemExpPro.BackgroundImage = this.GetImage(rect.Style);
                }
            }

            if (rect.Visibility != null)
            {
                this.rectItemExpPro.Hidden = this.GetExpressionKey(rect.Visibility.Hidden);
                this.ToggleItem = rect.Visibility.ToggleItem;
            }

            this.rectItemExpPro.Zindex = this.GetExpressionKey(rect.ZIndex.ToString());

            if (rect.Bookmark != null)
            {
                this.rectItemExpPro.BookMark = this.GetExpressionKey(rect.Bookmark);
            }

            if (rect.DocumentMapLabel != null)
            {
                this.rectItemExpPro.DocumentMapLabel = this.GetExpressionKey(rect.DocumentMapLabel);
                this.DocumentMapLable = this.rectItemExpPro.DocumentMapLabel;
            }
        }

        BackGroundImageExp GetImage(DOM.Style style)
        {
            BackGroundImageExp imageExp = new BackGroundImageExp();
            imageExp.BackgroundRepeat = this.GetExpressionKey(style.BackgroundImage.BackgroundRepeat);            
            imageExp.MimeType = this.GetExpressionKey(style.BackgroundImage.MIMEType);            
            imageExp.Position = this.GetExpressionKey(style.BackgroundImage.Position.ToString());            
            imageExp.Source = style.BackgroundImage.Source.ToString();
            imageExp.Value = style.BackgroundImage.Value;
            imageExp.TransparentColor = this.GetExpressionKey(style.BackgroundImage.TransparentColor);            
            return imageExp;
        }

        BorderExp GetEdgesBorder(DOM.Style style)
        {
            if (style.Border == null)
            {
                return null;
            }

            BorderExp borderEdg = new BorderExp();
            BorderExpProperties defaultBorder = null;

            if (style.Border != null)
            {
                borderEdg.Default = new BorderExpProperties();
                defaultBorder = borderEdg.Default;

                if (style.Border.Color != null)
                {
                    borderEdg.Default.BorderBrush = this.GetExpressionKey(style.Border.Color);
                }
                if (style.Border.Style != null)
                {
                    borderEdg.Default.BorderStyle = this.GetExpressionKey(style.Border.Style);
                }
                if (style.Border.Width != null)
                {
                    borderEdg.Default.Thickness = this.GetExpressionKey(style.Border.Width.size);
                }
            }

            if (style.BottomBorder != null)
            {
                borderEdg.BottomBorder = new BorderExpProperties();
                var borderExp = borderEdg.BottomBorder;

                if (defaultBorder != null)
                {
                    borderExp.BorderBrush = defaultBorder.BorderBrush;
                    borderExp.BorderStyle = defaultBorder.BorderStyle;
                    borderExp.Thickness = defaultBorder.Thickness;
                }

                if (style.BottomBorder.Color != null)
                {
                    borderEdg.BottomBorder.BorderBrush = this.GetExpressionKey(style.BottomBorder.Color);
                }
                if (style.BottomBorder.Style != null)
                {
                    borderEdg.BottomBorder.BorderStyle = this.GetExpressionKey(style.BottomBorder.Style);
                }
                if (style.BottomBorder.Width != null)
                {
                    borderEdg.BottomBorder.Thickness = this.GetExpressionKey(style.BottomBorder.Width.size);
                }
            }
            if (style.TopBorder != null)
            {
                borderEdg.TopBorder = new BorderExpProperties();

                var borderExp = borderEdg.TopBorder;

                if (defaultBorder != null)
                {
                    borderExp.BorderBrush = defaultBorder.BorderBrush;
                    borderExp.BorderStyle = defaultBorder.BorderStyle;
                    borderExp.Thickness = defaultBorder.Thickness;
                }

                if (style.TopBorder.Color != null)
                {
                    borderEdg.TopBorder.BorderBrush = this.GetExpressionKey(style.TopBorder.Color);
                }
                if (style.TopBorder.Style != null)
                {
                    borderEdg.TopBorder.BorderStyle = this.GetExpressionKey(style.TopBorder.Style);
                }
                if (style.TopBorder.Width != null)
                {
                    borderEdg.TopBorder.Thickness = this.GetExpressionKey(style.TopBorder.Width.size);
                }
            }
            if (style.LeftBorder != null)
            {
                borderEdg.LeftBorder = new BorderExpProperties();

                var borderExp = borderEdg.LeftBorder;

                if (defaultBorder != null)
                {
                    borderExp.BorderBrush = defaultBorder.BorderBrush;
                    borderExp.BorderStyle = defaultBorder.BorderStyle;
                    borderExp.Thickness = defaultBorder.Thickness;
                }

                if (style.LeftBorder.Color != null)
                {
                    borderEdg.LeftBorder.BorderBrush = this.GetExpressionKey(style.LeftBorder.Color);
                }
                if (style.LeftBorder.Style != null)
                {
                    borderEdg.LeftBorder.BorderStyle = this.GetExpressionKey(style.LeftBorder.Style);
                }
                if (style.LeftBorder.Width != null)
                {
                    borderEdg.LeftBorder.Thickness = this.GetExpressionKey(style.LeftBorder.Width.size);
                }
            }
            if (style.RightBorder != null)
            {
                borderEdg.RightBorder = new BorderExpProperties();

                var borderExp = borderEdg.RightBorder;

                if (defaultBorder != null)
                {
                    borderExp.BorderBrush = defaultBorder.BorderBrush;
                    borderExp.BorderStyle = defaultBorder.BorderStyle;
                    borderExp.Thickness = defaultBorder.Thickness;
                }

                if (style.RightBorder.Color != null)
                {
                    borderEdg.RightBorder.BorderBrush = this.GetExpressionKey(style.RightBorder.Color);
                }
                if (style.RightBorder.Style != null)
                {
                    borderEdg.RightBorder.BorderStyle = this.GetExpressionKey(style.RightBorder.Style);
                }
                if (style.RightBorder.Width != null)
                {
                    borderEdg.RightBorder.Thickness = this.GetExpressionKey(style.RightBorder.Width.size);
                }
            }

            return borderEdg;
        }

        #endregion

        private string GetExpressionKey(string value)
        {
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

        private void AddFields(DataField field)
        {
            var fieldsCount = (from expField in this.DataSetFields
                               where (field.FunctionName.Equals(expField.FunctionName)
                               && field.Name.Equals(expField.Name) && !field.IsDataSetField)
                               select expField).Count();

            if (fieldsCount == 0)
            {
                this.DataSetFields.Add(field);
            }
        }

        void IntializeEngineValues()
        {
            if (this.FieldValues != null)
            {
                foreach (var value in this.FieldValues)
                {
                    this.Model.ExpressionEngine.FieldValues.Add(value.Name, value.Value.GetResult());
                }
            }
            else if (this.DataSetFields != null)
            {
                ReportingAggEngine engine = new ReportingAggEngine();
                engine.Fields = this.DataSetFields.Where(field => !(field.IsRecursiveField || field.IsDataSetField)).ToList();

                if (engine.Fields.Count > 0)
                {
                    List<object> datas = new List<object>();
                    datas.Add(this.DataSource);
                    engine.DataSource = datas;
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


        #region overridden methods

        public override IReportItemModeler GetModel()
        {
            RectangleModel itemModel = new RectangleModel();
            itemModel.IsTablixChild = this.IsTablixChild;
            itemModel.IsTablixInnerChild = this.IsTablixInnerChild;
            itemModel.ModelType = this.ModelType;
            itemModel.Model = this.Model;
            itemModel.ReportItem = this.ReportItem;
            itemModel.Name = this.Name;
            itemModel.Top = this.Top;
            itemModel.Left = this.Left;
            itemModel.Width = this.Width;
            itemModel.Height = this.Height;
            itemModel.rectItemExpPro = this.rectItemExpPro;
            itemModel.Hidden = this.Hidden;
            itemModel.KeepTogether = this.KeepTogether;

            if (this.IsTablixChild)
            {
                if (this.ReportItemModelers != null)
                {
                    itemModel.ReportItemModelers = new ReportModelContentCollection();

                    foreach (var itModel in this.ReportItemModelers)
                    {
                        itemModel.ReportItemModelers.Add(itModel.GetModel());
                    }
                }
            }
           
            return itemModel;
        }


        public override void DisposeReportItemObj()
        {
            this.Model = null;
            this.ReportItem = null;            
            this.DataSetFields = null;
            this.DataSource = null;
            this.FieldValues = null;

            if (this.ReportItemModelers != null)
            {
                foreach (var reportItem in this.ReportItemModelers)
                {
                    reportItem.DisposeReportItemObj();
                }
            }

            this.ReportItemModelers = null;
            base.DisposeReportItemObj();
        }

        public override void DisposeEvalObjects()
        {
            if (this.RectItemExpPro != null)
            {
                this.RectItemExpPro.BackgroundImage = null;
                this.RectItemExpPro.Border = null;
            }

            this.RectItemExpPro = null;

            if (this.ReportItemModelers != null)
            {
                foreach (var reportItem in this.ReportItemModelers)
                {
                    reportItem.DisposeEvalObjects();
                }
            }

            base.DisposeEvalObjects();
        }

        public override void Evaluate()
        {
            this.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();

            try
            {
                if (this.Model.EnableVirtualEvaluation)
                {
                    this.IntializeEngineValues();
                }

                this.RectItemExpPro = new RectangleItemExpVal();

                this.RectItemExpPro.ToolTip = this.Model.ExpressionEngine.GetEvalExpressionString(this.rectItemExpPro.ToolTip);
                if (this.rectItemExpPro.BackgroundColor != null)
                {
                    this.RectItemExpPro.BackgroundColor = this.Model.ExpressionEngine.GetEvalExpressionString(this.rectItemExpPro.BackgroundColor);
                }
                if (this.rectItemExpPro.BackgroundImage != null)
                {
                    this.RectItemExpPro.BackgroundImage = this.GetImage(this.rectItemExpPro.BackgroundImage);
                }
                if (this.rectItemExpPro.Border != null)
                {
                    this.RectItemExpPro.Border = this.GetEdgesBorder(this.rectItemExpPro.Border);
                }

                if (!string.IsNullOrEmpty(this.ToggleItem))
                {
                    this.GetTextBoxModel(this.ToggleItem);
                }

                this.RectItemExpPro.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(this.rectItemExpPro.Hidden));
                this.Hidden = this.RectItemExpPro.Hidden;
                this.RectItemExpPro.Zindex = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(this.rectItemExpPro.Zindex));
                this.RectItemExpPro.BookMark = this.Model.ExpressionEngine.GetEvalExpressionString(this.rectItemExpPro.BookMark);
                this.RectItemExpPro.DocumentMapLabel = this.Model.ExpressionEngine.GetEvalExpressionString(this.rectItemExpPro.DocumentMapLabel);
                this.DocumentMapLable=this.RectItemExpPro.DocumentMapLabel;
                if (!string.IsNullOrEmpty(this.DocumentMapLable))
                {
                    this.SetTreeModel();
                }
                if (this.ReportItemModelers != null && this.IsTablixChild)
                {
                    foreach (var itemModel in this.ReportItemModelers)
                    {
                        itemModel.DataSource = this.DataSource;
                        itemModel.FieldValues = this.FieldValues;
                        itemModel.Evaluate();
                    }
                }

                base.Evaluate();
            }
            catch (Exception e)
            {
                this.Model.ExceptionDetails.Add("Getting following exception while evaluate the expression in " + this.ReportItem.Name + " " + e.Message);
            }

            if (this.Model.ExpressionEngine.FieldValues != null)
            {
                this.Model.ExpressionEngine.FieldValues.Clear();
                this.Model.ExpressionEngine.FieldValues = null;
            }
        }     

        BorderExpval GetEdgesBorder(BorderExp style)
        {
            BorderExpval borderEdg = new BorderExpval();

            if (style.Default != null)
            {
                borderEdg.Default = new BorderExpvalProperties();

                if (style.Default.BorderBrush != null)
                {
                    borderEdg.Default.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(style.Default.BorderBrush);
                }
                if (style.Default.BorderStyle != null)
                {
                    borderEdg.Default.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(style.Default.BorderStyle), true);
                }
                if (style.Default.Thickness != null)
                {
                    borderEdg.Default.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(style.Default.Thickness)).FloatValue;
                }
            }
            if (style.BottomBorder != null)
            {
                borderEdg.BottomBorder = new BorderExpvalProperties();

                if (style.BottomBorder.BorderBrush != null)
                {
                    borderEdg.BottomBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(style.BottomBorder.BorderBrush);
                }
                if (style.BottomBorder.BorderStyle != null)
                {
                    borderEdg.BottomBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(style.BottomBorder.BorderStyle), true);
                }
                if (style.BottomBorder.Thickness != null)
                {
                    borderEdg.BottomBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(style.BottomBorder.Thickness)).FloatValue;
                }
            }
            if (style.TopBorder != null)
            {
                borderEdg.TopBorder = new BorderExpvalProperties();

                if (style.TopBorder.BorderBrush != null)
                {
                    borderEdg.TopBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(style.TopBorder.BorderBrush);
                }
                if (style.TopBorder.BorderStyle != null)
                {
                    borderEdg.TopBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(style.TopBorder.BorderStyle), true);
                }
                if (style.TopBorder.Thickness != null)
                {
                    borderEdg.TopBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(style.TopBorder.Thickness)).FloatValue;
                }
            }
            if (style.LeftBorder != null)
            {
                borderEdg.LeftBorder = new BorderExpvalProperties();

                if (style.LeftBorder.BorderBrush != null)
                {
                    borderEdg.LeftBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(style.LeftBorder.BorderBrush);
                }
                if (style.LeftBorder.BorderStyle != null)
                {
                    borderEdg.LeftBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(style.LeftBorder.BorderStyle), true);
                }
                if (style.LeftBorder.Thickness != null)
                {
                    borderEdg.LeftBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(style.LeftBorder.Thickness)).FloatValue;
                }
            }
            if (style.RightBorder != null)
            {
                borderEdg.RightBorder = new BorderExpvalProperties();

                if (style.RightBorder.BorderBrush != null)
                {
                    borderEdg.RightBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(style.RightBorder.BorderBrush);
                }
                if (style.RightBorder.BorderStyle != null)
                {
                    borderEdg.RightBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(style.RightBorder.BorderStyle), true);
                }
                if (style.RightBorder.Thickness != null)
                {
                    borderEdg.RightBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(style.RightBorder.Thickness)).FloatValue;
                }
            }

            return borderEdg;
        }

        BackGroundImageExpVal GetImage(BackGroundImageExp style)
        {
            BackGroundImageExpVal imageExp = new BackGroundImageExpVal();
            imageExp.BackgroundRepeat = this.Model.ExpressionEngine.GetEvalExpressionString(style.BackgroundRepeat);
            imageExp.MimeType = this.Model.ExpressionEngine.GetEvalExpressionString(style.MimeType);
            imageExp.Position = TryEnum<DOM.Position>(this.Model.ExpressionEngine.GetEvalExpressionString((style.Position.ToString())));
            imageExp.Source = TryEnum<DOM.Source>(this.Model.ExpressionEngine.GetEvalExpressionString((style.Source.ToString())));
            imageExp.Value = this.Model.ExpressionEngine.GetEvalExpressionString(style.Value);
            imageExp.TransparentColor = this.Model.ExpressionEngine.GetEvalExpressionString(style.TransparentColor);
            return imageExp;
        }

        private void SetTreeModel()
        {
            if (this.Model.MapModel == null)
            {
                this.Model.MapModel = new DocumentMapModel();
            }
            DocumentData node = new DocumentData();
            node.DocumentLable = this.DocumentMapLable;
            node.ModelType = ModelType.RectangleModel;
            node.ReportItemName = this.Name;
            node.IsTablixChild = this.IsTablixChild;
            this.DocumentNodeRefer = node;
            this.Model.MapModel.NodeData.Add(node);
        }

        public override void UpdateSize()
        {
            if (this.ReportItemModelers != null && this.IsTablixChild)
            {
                foreach (var model in this.ReportItemModelers)
                {
                    if (model != null && model.CanGrow && model.ModelType == Internal.ModelType.TextBoxModel)
                    {
                        model.UpdateSize();
                        var textbox = (model as TextboxModel);
                        double rectHeight = textbox.ActualHeight;
                        rectHeight = rectHeight >= this.Height ? rectHeight : this.Height;
                        this.Height = rectHeight;
                        double rectWidth = textbox.Width;
                        rectWidth = rectWidth >= this.Width ? rectWidth : this.Width;
                        this.Width = rectWidth;
                    }
                    else if (model != null && model.ModelType == Internal.ModelType.RectangleModel)
                    {
                        model.UpdateSize();
                        double rectHeight = (model as RectangleModel).Height;
                        rectHeight = rectHeight >= this.Height ? rectHeight : this.Height;
                        this.Height = rectHeight;
                        double rectWidth = (model as RectangleModel).Width;
                        rectWidth = rectWidth >= this.Width ? rectWidth : this.Width;
                        this.Width = rectWidth;
                    }
                    else if (model != null && model.ModelType == Internal.ModelType.TablixModel)
                    {
                        model.UpdateSize();
                        double rowHeight = 0, colWidth = 0;
                        var itemmodel = (model as TablixModel);
                        for (int pos = 0; pos < itemmodel.RowCount; pos++)
                        {
                            rowHeight += itemmodel.RowHeights[pos];
                        }
                        if (itemmodel.Height < rowHeight)
                        {
                            this.Height = model.Height = this.Height + (rowHeight - itemmodel.Height);
                        }

                        for (int pos = 0; pos < itemmodel.ColumnCount; pos++)
                        {
                            colWidth += itemmodel.ColumnWights[pos];
                        }

                        if (itemmodel.Width < colWidth)
                        {
                            this.Width = model.Width = this.Width + (colWidth - itemmodel.Width);
                        }
                    }
                    else if (model != null && model.ModelType == Internal.ModelType.SubReportModel)
                    {
                        model.UpdateSize();
                    }
                }
            }
        }

        public override void UpdateHeight(double firstPageHeight, LayoutReportItemModel locationInfo, double preferredHeight, ReportItemViewMode itemViewMode)
        {
            if (this.Hidden)
            {
                locationInfo.ActualHeight = 0;
                return;
            }
            double maxBottomValue = locationInfo.ActualTop + this.Height;
            double reportBottomValue = 0;
            bool hasPageBreak = false;

            if (this.ReportItemModelers != null)
            {
                if (!this.IsTablixChild)
                {
                    foreach (var model in this.ReportItemModelers)
                    {
                        if (itemViewMode == ReportItemViewMode.Normal)
                        {
                            reportBottomValue = model.PageInfo.ActualTop + model.PageInfo.ActualHeight;
                            if (model.PageBreak != BreakLocation.None)
                            {
                                hasPageBreak = true;
                            }
                        }
                        else if (itemViewMode == ReportItemViewMode.Print)
                        {
                            reportBottomValue = model.PrintPageInfo.ActualTop + model.PrintPageInfo.ActualHeight;
                        }
                        else
                        {
                            reportBottomValue = model.FlowLayoutInfo.ActualTop + model.FlowLayoutInfo.ActualHeight;
                        }

                        maxBottomValue = reportBottomValue > maxBottomValue ? reportBottomValue : maxBottomValue;
                    }
                }
                else
                {
                    this.UpdateHeightInnerReportItems(itemViewMode, firstPageHeight, preferredHeight);
                }
            }

            if (!this.IsTablixChild && (maxBottomValue - locationInfo.ActualTop) > this.Height)
            {
                locationInfo.ActualHeight += maxBottomValue - locationInfo.ActualTop;
            }
            else
            {
                locationInfo.ActualHeight = maxBottomValue - locationInfo.ActualTop;
            }
            locationInfo.ActualTop = this.IsTablixChild ? this.Top : locationInfo.ActualTop;

            Dictionary<int, PageInfo> pageSizes = new Dictionary<int, PageInfo>();
            List<double> pageWidths = itemViewMode == ReportItemViewMode.Normal ? this.PageWidths : this.PrintPageWidths;

            if (itemViewMode != ReportItemViewMode.None)
            {
                double abortHeight = preferredHeight;
                var hasHeightInfo = !double.IsNaN(firstPageHeight);
                var hasFirstPreferredHeight = !double.IsNaN(firstPageHeight);
                int pageCount = 0;
                double totalHeight = locationInfo.ActualHeight;

                if ((itemViewMode != ReportItemViewMode.Print && (!hasHeightInfo || (this.KeepTogether && !hasPageBreak))) || (hasHeightInfo && totalHeight <= firstPageHeight))
                {
                    foreach (var column in pageWidths)
                    {
                        pageSizes.Add(pageCount++, new PageInfo() { Height = locationInfo.ActualHeight, Width = column });
                    }
                }
                else
                {
                    var hightReachedEnd = false;
                    var isFirstPageSet = false;
                    double innerItemHeight = 0;
                    bool innerItemCalculated = false;
                    int maxPageCount = 0;

                    while (!hightReachedEnd)
                    {
                        if (!innerItemCalculated && !this.IsTablixChild)
                        {
                            innerItemHeight = this.GetInnerItemPageSize(pageCount, itemViewMode, out maxPageCount);
                            innerItemCalculated = true;
                        }

                        if (!isFirstPageSet)
                        {
                            firstPageHeight = innerItemHeight != 0 && innerItemHeight > firstPageHeight ? innerItemHeight + 1 : firstPageHeight;
                            if (maxPageCount != 0 && maxPageCount == pageCount + 1)
                            {
                                firstPageHeight = totalHeight;
                            }
                            if (totalHeight <= firstPageHeight)
                            {
                                hightReachedEnd = true;
                            }

                            foreach (var column in pageWidths)
                            {
                                pageSizes.Add(pageCount++, new PageInfo() { Height = firstPageHeight, Width = column });
                            }

                            totalHeight -= firstPageHeight;
                            isFirstPageSet = true;
                            innerItemCalculated = false;
                            innerItemHeight = 0;
                        }
                        else
                        {
                            double calHeight = innerItemHeight != 0 && innerItemHeight > abortHeight ? innerItemHeight + 1 : abortHeight;

                            if (totalHeight <= abortHeight)
                            {
                                hightReachedEnd = true;
                                calHeight = totalHeight;
                                calHeight = innerItemHeight > calHeight ? innerItemHeight + 1 : calHeight;
                            }

                            foreach (var column in pageWidths)
                            {
                                pageSizes.Add(pageCount++, new PageInfo() { Height = calHeight, Width = column });
                            }

                            totalHeight -= abortHeight;
                            innerItemCalculated = false;
                            innerItemHeight = 0;
                        }
                    }
                }
            }
            else
            {
                pageSizes.Add(0, new PageInfo() { Height = locationInfo.ActualHeight });
            }

            if (itemViewMode == ReportItemViewMode.Print)
            {
                this.PrintPageColumnCount = pageWidths.Count;
                this.PrintPageSizes = pageSizes;
            }
            else if (itemViewMode == ReportItemViewMode.Normal)
            {
                this.PageSizes = pageSizes;
            }
            else
            {
                this.FlowPageSizes = pageSizes;
            }

            locationInfo.TotalPages = pageSizes.Count;
        }

        private double GetInnerItemPageSize(int pageCount, ReportItemViewMode itemViewMode, out int maxPage)
        {
            maxPage = 0;
            if (itemViewMode == ReportItemViewMode.Normal && this.ReportItemModelers != null && this.ReportItemModelers.Count > 0 )
            {
                double pageHeight = this.Height;
                foreach (var model in this.ReportItemModelers)
                {
                    if (model != null && model.ModelType == Internal.ModelType.TextBoxModel)
                    {
                        var item = (model as TextboxModel);
                        if (itemViewMode == ReportItemViewMode.Normal && item.PageSizes != null && item.PageSizes.Count > pageCount)
                        {
                            if (pageCount == 0)
                            {
                                var height = model.Top + item.PageSizes[pageCount].Height;
                                pageHeight = height > pageHeight ? height : pageHeight;
                            }
                            else
                            {
                                pageHeight = item.PageSizes[pageCount].Height > pageHeight ? item.PageSizes[pageCount].Height : pageHeight;
                            }
                            maxPage = maxPage < item.PageSizes.Count ? item.PageSizes.Count : maxPage;
                        }
                        else if (item.PrintPageSizes != null && item.PrintPageSizes.Count > pageCount)
                        {
                            if (pageCount == 0)
                            {
                                var height = model.Top + item.PrintPageSizes[pageCount].Height;
                                pageHeight = height > pageHeight ? height : pageHeight;
                            }
                            else
                            {
                                pageHeight = item.PrintPageSizes[pageCount].Height > pageHeight ? item.PrintPageSizes[pageCount].Height : pageHeight;
                            }
                            maxPage = maxPage < item.PrintPageSizes.Count ? item.PrintPageSizes.Count : maxPage;
                        }
                    }
                    else if (model != null && model.ModelType == Internal.ModelType.RectangleModel)
                    {
                        var item = (model as RectangleModel);
                        if (itemViewMode == ReportItemViewMode.Normal && item.PageSizes != null && item.PageSizes.Count > pageCount)
                        {
                            if (pageCount == 0)
                            {
                                var height = model.Top + item.PageSizes[pageCount].Height;
                                pageHeight = height > pageHeight ? height : pageHeight;
                            }
                            else
                            {
                                pageHeight = item.PageSizes[pageCount].Height > pageHeight ? item.PageSizes[pageCount].Height : pageHeight;
                            }
                            maxPage = maxPage < item.PageSizes.Count ? item.PageSizes.Count : maxPage;
                        }
                        else if (item.PrintPageSizes != null && item.PrintPageSizes.Count > pageCount)
                        {
                            if (pageCount == 0)
                            {
                                var height = model.Top + item.PrintPageSizes[pageCount].Height;
                                pageHeight = height > pageHeight ? height : pageHeight;
                            }
                            else
                            {
                                pageHeight = item.PrintPageSizes[pageCount].Height > pageHeight ? item.PrintPageSizes[pageCount].Height : pageHeight;
                            }
                            maxPage = maxPage < item.PrintPageSizes.Count ? item.PrintPageSizes.Count : maxPage;
                        }
                    }
                    else if (model != null && model.ModelType == Internal.ModelType.TablixModel)
                    {
                        var item = (model as TablixModel);
                        if (itemViewMode == ReportItemViewMode.Normal && item.PageSizes != null && item.PageSizes.Count > pageCount)
                        {
                            if (pageCount == 0)
                            {
                                var height = model.Top + item.PageSizes[pageCount].Height;
                                pageHeight = height > pageHeight ? height : pageHeight;
                            }
                            else
                            {
                                pageHeight = item.PageSizes[pageCount].Height > pageHeight ? item.PageSizes[pageCount].Height : pageHeight;
                            }
                            maxPage = maxPage < item.PageSizes.Count ? item.PageSizes.Count : maxPage;
                        }
                        else if (item.PrintPageSizes != null && item.PrintPageSizes.Count > pageCount)
                        {
                            if (pageCount == 0)
                            {
                                var height = model.Top + item.PrintPageSizes[pageCount].Height;
                                pageHeight = height > pageHeight ? height : pageHeight;
                            }
                            else
                            {
                                pageHeight = item.PrintPageSizes[pageCount].Height > pageHeight ? item.PrintPageSizes[pageCount].Height : pageHeight;
                            }
                            maxPage = maxPage < item.PrintPageSizes.Count ? item.PrintPageSizes.Count : maxPage;
                        }
                    }
                    if (model.PageBreak != BreakLocation.None)
                    {
                        return 0;
                    }
                }
                return pageHeight;
            }
            return 0;
        }

        public override void UpdateWidth(double firstPageWidth, LayoutReportItemModel locationInfo, double preferredWidth, ReportItemViewMode itemViewMode)
        {
            if (this.Hidden)
            {
                locationInfo.ActualWidth = 0;
                return;
            }

            double maxRightValue = locationInfo.ActualLeft + this.Width;

            if (this.ReportItemModelers != null)
            {
                if (!this.IsTablixChild)
                {
                    foreach (var model in this.ReportItemModelers)
                    {
                        double reportRightValue = 0;

                        if (itemViewMode == ReportItemViewMode.Print)
                        {
                            reportRightValue = model.PrintPageInfo.ActualLeft + model.PrintPageInfo.ActualWidth;
                        }
                        else if (itemViewMode == ReportItemViewMode.Normal)
                        {
                            reportRightValue = model.PageInfo.ActualLeft + model.PageInfo.ActualWidth;
                        }
                        else
                        {
                            reportRightValue = model.FlowLayoutInfo.ActualLeft + model.FlowLayoutInfo.ActualWidth;
                        }

                        maxRightValue = reportRightValue > maxRightValue ? reportRightValue : maxRightValue;
                    }
                }
                else
                {
                    this.UpdateWithInnerReportItems(itemViewMode, firstPageWidth, preferredWidth);
                }
            }

            locationInfo.ActualWidth = maxRightValue - locationInfo.ActualLeft;
            locationInfo.ActualLeft = this.IsTablixChild ? this.Left : locationInfo.ActualLeft;

            if (itemViewMode != ReportItemViewMode.None)
            {
                double abortWidth = preferredWidth;
                var hasWidthInfo = !double.IsNaN(firstPageWidth);
                List<double> pageWidths = new List<double>();

                if (itemViewMode == ReportItemViewMode.Normal)
                {
                    this.PageWidths = pageWidths;
                }
                else
                {
                    this.PrintPageWidths = pageWidths;
                }

                double columnPageWidth = locationInfo.ActualWidth;

                if (!hasWidthInfo || (hasWidthInfo && columnPageWidth <= firstPageWidth))
                {
                    pageWidths.Add(columnPageWidth);
                }
                else
                {
                    var isFirstPageSet = false;
                    var columnReachedEnd = false;

                    while (!columnReachedEnd)
                    {
                        var colWidth = abortWidth;

                        if (!isFirstPageSet)
                        {
                            colWidth = firstPageWidth;
                            columnPageWidth -= firstPageWidth;
                            isFirstPageSet = true;
                        }
                        else
                        {
                            if (columnPageWidth <= abortWidth)
                            {
                                columnReachedEnd = true;
                                colWidth = columnPageWidth;
                            }

                            columnPageWidth -= abortWidth;
                        }

                        pageWidths.Add(colWidth);
                    }
                }
            }
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

        void UpdateWithInnerReportItems(ReportItemViewMode itemViewMode, double firstPageWidth, double preferredWidth)
        {
            if (itemViewMode != ReportItemViewMode.Print)
            {
                if (this.ReportItemModelers != null)
                {
                    foreach (var model in this.ReportItemModelers)
                    {
                        if (model.ModelType == ModelType.TablixModel || model.ModelType == ModelType.RectangleModel)
                        {
                            if (model.PageInfo == null)
                            {
                                model.PageInfo = model.FlowLayoutInfo = new LayoutReportItemModel();
                            }
                            model.UpdateWidth(firstPageWidth, model.PageInfo, preferredWidth, itemViewMode);
                        }
                    }
                }
            }
            else
            {
                if (this.ReportItemModelers != null)
                {
                    foreach (var model in this.ReportItemModelers)
                    {
                        if (model.ModelType == ModelType.TablixModel || model.ModelType == ModelType.RectangleModel)
                        {
                            if (model.PrintPageInfo == null)
                            {
                                model.PrintPageInfo = new LayoutReportItemModel();
                            }
                            else
                            {
                                model.PrintPageInfo.BelongsTo = null;
                            }
                            model.UpdateWidth(firstPageWidth, model.PrintPageInfo, preferredWidth, itemViewMode);
                        }
                    }
                }
            }
        }

        void UpdateHeightInnerReportItems(ReportItemViewMode itemViewMode, double firstPageHeight, double preferredHeight)
        {
            if (itemViewMode != ReportItemViewMode.Print)
            {
                if (this.ReportItemModelers != null)
                {
                    foreach (var model in this.ReportItemModelers)
                    {
                        if (this.Model.IsToggleState && model.PageInfo != null)
                        {
                            model.PageInfo.BelongsTo = null;
                        }

                        if (this.Model.IsToggleState && model.PrintPageInfo != null)
                        {
                            model.PrintPageInfo.BelongsTo = null;
                        }

                        if (model.ModelType == ModelType.TablixModel || model.ModelType == ModelType.RectangleModel)
                        {
                            if (model.PageInfo == null)
                            {
                                model.PageInfo = model.FlowLayoutInfo = new LayoutReportItemModel();
                            }
                            model.UpdateHeight(firstPageHeight, model.PageInfo, preferredHeight, itemViewMode);
                        }
                    }
                }
            }
            else
            {
                if (this.ReportItemModelers != null)
                {
                    foreach (var model in this.ReportItemModelers)
                    {
                        if (model.ModelType == ModelType.TablixModel || model.ModelType == ModelType.RectangleModel)
                        {
                            if (model.PrintPageInfo == null)
                            {
                                model.PrintPageInfo = new LayoutReportItemModel();
                            }
                            model.UpdateHeight(firstPageHeight, model.PrintPageInfo, preferredHeight, itemViewMode);
                        }
                    }
                }
            }

        }

        #region DataType Convert Wapper Method

        // clr generic datatype converter 
        internal T TryParse<T>(object value)
        {
            try
            {
                if (value==null)
                {
                    return default(T);
                }
                T retValue = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                return retValue;
            }
            catch
            {
                return default(T);
            }
        }

        // common enum type converter
        internal T TryEnum<T>(string enumMember)
        {
            try
            {
                T value = (T)Enum.Parse(typeof(T), enumMember, true);
                return value;
            }
            catch
            {
                return default(T);
            }
        }

        #endregion
  
    }
}