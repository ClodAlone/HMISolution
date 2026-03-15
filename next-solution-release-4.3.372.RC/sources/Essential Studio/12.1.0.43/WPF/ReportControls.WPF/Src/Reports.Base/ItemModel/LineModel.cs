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
using Syncfusion.RDL.Controls;
#endif


namespace Syncfusion.RDL.ItemModel
{
    /// <summary>
    /// A model for TextboxModel. Contains information about size, position, data and style for a text box.
    /// </summary>
    internal class LineModel
        : ReportItemModeler
    {
        #region members

        LinePropertiesExp LineExpProp;

        #endregion

        #region  properties

        public LinePropertiesExpVal LineProperties 
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

        public double X1
        {
            get;
            set;
        }

        public double Y1
        {
            get;
            set;
        }

        public double X2
        {
            get;
            set;
        }

        public double Y2
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        internal LineModel()
        {
        }

        internal LineModel(ReportItem reportItem, ReportModel pageModel, bool isTablixChild,string dataSetName)
        {
            this.DataSetName = dataSetName;
            this.Name = reportItem.Name;
            this.IsTablixChild = isTablixChild;
            this.Model = pageModel;
            this.ReportItem = reportItem;
            this.ModelType = ModelType.LineModel;
            
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

            this.X1 = 0;
            this.Y1 = 0;
            this.X2 = this.Width;
            this.Y2 = this.Height;

            this.ParseLine();
        }

        void ParseLine()
        {
            Line line = this.ReportItem as Line;
            this.LineExpProp = new LinePropertiesExp();
            RDL.DOM.Style style = line.Style;
            this.LineExpProp.BookMark = this.Model.ExpressionEngine.GetExpressionKey(line.Bookmark);
            this.AddDataFields(line.Bookmark);
            this.LineExpProp.DocumentMapLabel = this.Model.ExpressionEngine.GetExpressionKey(line.DocumentMapLabel);
            this.DocumentMapLable = this.LineExpProp.DocumentMapLabel;
            this.AddDataFields(line.DocumentMapLabel);
            if (line.Visibility != null)
            {
                this.LineExpProp.Hidden = this.Model.ExpressionEngine.GetExpressionKey(line.Visibility.Hidden);
                this.AddDataFields(line.Visibility.Hidden);
                this.ToggleItem = line.Visibility.ToggleItem;
            }
            if (style.Border != null)
            {
                if (style.Border.Color!=null)
                {
                    this.LineExpProp.LineColor = this.Model.ExpressionEngine.GetExpressionKey(style.Border.Color);
                    this.AddDataFields(style.Border.Color);
                }
                if (style.Border.Style != null)
                {
                    this.LineExpProp.LineStyle = this.Model.ExpressionEngine.GetExpressionKey(style.Border.Style);
                    this.AddDataFields(style.Border.Style);
                }
                if (style.Border.Width != null)
                {
                    this.LineExpProp.LineWidth = this.Model.ExpressionEngine.GetExpressionKey(style.Border.Width.size);
                    this.AddDataFields(style.Border.Width.FloatValue.ToString());
                }
            }
            this.LineExpProp.ZIndex = this.Model.ExpressionEngine.GetExpressionKey(line.ZIndex.ToString());
        }

        #endregion

        private void AddDataFields(string key)
        {
            if (this.IsTablixChild)
            {
                if (!string.IsNullOrEmpty(key) && this.Model.ExpressionEngine.FieldInformations.ContainsKey(key))
                {
                    var fields = this.Model.ExpressionEngine.FieldInformations[key];

                    foreach (var field in fields)
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
                }
            }
        }

#if !SILVERLIGHT

        internal Stream GetImageStream()
        {
            Stream source = new ImageConversion().CovertToImage(new ReportingLine(this));            
            return source;
        }
#endif

        #region overridden methods

        public override void Evaluate()
        {
            try
            {
                if (this.Width < 0)
                {
                    double left = this.Left + this.Width;
                    this.Width = Math.Abs(left - this.Left);
                    this.X2 = 0;
                    this.X1 = this.Width;
                    this.Left = left;
                }
                if (this.Height < 0)
                {
                    double top = this.Top + this.Height;
                    this.Height = Math.Abs((this.Top + this.Height) - this.Top);
                    this.Y2 = 0;
                    this.Y1 = this.Height;
                    this.Top = top;
                }

                this.LineProperties = new LinePropertiesExpVal();
                this.LineProperties.BookMark = this.Model.ExpressionEngine.GetEvalExpressionString(this.LineExpProp.BookMark);
                this.LineProperties.DocumentMapLabel = this.Model.ExpressionEngine.GetEvalExpressionString(this.LineExpProp.DocumentMapLabel);
                this.DocumentMapLable = this.LineProperties.DocumentMapLabel;
                if (!string.IsNullOrEmpty(this.DocumentMapLable) && !this.IsTablixChild)
                {
                    this.SetTreeModel();
                }
                if (!string.IsNullOrEmpty(this.ToggleItem))
                {
                    this.GetTextBoxModel(this.ToggleItem);
                }
                this.LineProperties.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(this.LineExpProp.Hidden));
                this.LineProperties.LineColor = this.Model.ExpressionEngine.GetEvalExpressionString(this.LineExpProp.LineColor);
                this.Hidden = this.LineProperties.Hidden;
                this.LineProperties.ZIndex = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(this.LineExpProp.ZIndex));

                if (this.LineExpProp.LineStyle != null)
                {
                    this.LineProperties.LineStyle = TryEnum<LineStyle>(this.Model.ExpressionEngine.GetExpressionKey(this.LineExpProp.LineStyle));
                }
                if (!string.IsNullOrEmpty(this.LineExpProp.LineWidth))
                {
                    this.LineProperties.LineWidth = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.LineExpProp.LineWidth)).PixelValue;
                }
                else
                {
                    this.LineProperties.LineWidth = 1;
                }

                base.Evaluate();
            }
            catch (Exception e)
            {
                this.Model.ExceptionDetails.Add("Getting following exception while evaluate the expression in " + this.ReportItem.Name + " " + e.Message);
            }
        }

        private void SetTreeModel()
        {
            if (this.Model.MapModel == null)
            {
                this.Model.MapModel = new DocumentMapModel();
            }
            DocumentData node = new DocumentData();
            node.DocumentLable = this.DocumentMapLable;
            node.ModelType = ModelType.LineModel;
            node.ReportItemName = this.Name;
            node.IsTablixChild = this.IsTablixChild;
            this.DocumentNodeRefer = node;
            this.Model.MapModel.NodeData.Add(node);
        }

        public override IReportItemModeler GetModel()
        {
            LineModel lineModel = new LineModel();
            lineModel.IsTablixChild = this.IsTablixChild;
            lineModel.ModelType = this.ModelType;
            lineModel.Model = this.Model;
            lineModel.ReportItem = this.ReportItem;
            lineModel.Name = this.Name;
            lineModel.Top = this.Top;
            lineModel.Left = this.Left;
            lineModel.Width = this.Width;
            lineModel.Height = this.Height;
            lineModel.LineExpProp = this.LineExpProp;
            lineModel.Hidden = this.Hidden;
            lineModel.X1 = this.X1;
            lineModel.X2 = this.X2;
            lineModel.Y1 = this.Y1;
            lineModel.Y2 = this.Y2;

            return lineModel;
        }

        public override void DisposeEvalObjects()
        {
            this.LineProperties = null;
            base.DisposeEvalObjects();
        }

        public override void DisposeReportItemObj()
        {
            this.Model = null;
            this.ReportItem = null;
            this.DataSetFields = null;
            this.DataSource = null;
            this.FieldValues = null;

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
                this.DocumentNodeRefer.PageNo = pageNo + 1;
                this.DocumentNodeRefer.TopPos = this.Top;
                this.DocumentNodeRefer.LeftPos = this.Left;
                this.DocumentNodeRefer = null;
            }
        }

        #endregion

        #region DataType Convert Wapper Method

        // clr generic datatype converter 
        public T TryParse<T>(object value)
        {
            try
            {
                if (value != null)
                {
                    T retValue = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                    return retValue;
                }
                  return default(T);
            }   
            catch
            {
                return default(T);
            }
        }

        // common enum type converter
        public T TryEnum<T>(string enumMember)
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