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

#if WINRT
using Syncfusion.UI.Xaml.Reports;
#elif MVC
using Syncfusion.Reports.Mvc;
#else
using Syncfusion.Windows.Reports;
#endif

#if !SILVERLIGHT
using System.Data;
using System.Threading;
#endif


namespace Syncfusion.RDL.ItemModel
{
    /// <summary>
    /// A model for TextboxModel. Contains information about size, position, data and style for a text box.
    /// </summary>
    internal class SubReportModel
        : ReportItemModeler
    {
        #region members

        private SubReportItemExp subReportItemExpPro;

        //double bodyBottomGap = 0;
        //double bodyRightGap = 0;

        #endregion

        #region  properties

        public SubReportItemExpVal SubReportItemExpPro
        {
            get;
            set;
        }

        internal ReportModel SubreportReportModel
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

        internal SubReport SubReport
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        internal SubReportModel(ReportItem reportItem, ReportModel pageModel, bool isTablixChild,string dataSetName)
        {
            this.DataSetName = dataSetName;
            this.IsTablixChild = isTablixChild;
            this.Model = pageModel;
            this.ReportItem = reportItem;            
            this.SubReport = this.ReportItem as SubReport;
            this.KeepTogether = this.SubReport.KeepTogether;
            this.SubreportReportModel = new ReportModel();
            this.SubreportReportModel.IsRDLC = this.Model.IsRDLC;
            this.SubreportReportModel.ReportServerUrl = this.Model.ReportServerUrl;
            this.SubreportReportModel.ReportServerCredential = this.Model.ReportServerCredential;
            this.SubreportReportModel.ReportServerFormsCredential = this.Model.ReportServerFormsCredential;
            this.SubreportReportModel.SubReportStream = this.Model.SubReportStream;
            this.SubreportReportModel.EnableVirtualEvaluation = this.Model.EnableVirtualEvaluation;

#if SILVERLIGHT
            this.SubreportReportModel.ReportServiceURL = this.Model.ReportServiceURL;
#endif

            this.ModelType = ModelType.SubReportModel;
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

            foreach(var parameter in this.SubReport.Parameters)
            {
                parameter.Value = this.GetExpressionKey(parameter.Value);
            }

            this.ParseSubReport();
        }

        private string GetExpressionKey(string value)
        {
            string key = this.Model.ExpressionEngine.GetExpressionKey(value, this.DataSetName);
            this.AddDataFields(key);
            return key;
        }

        private void AddDataFields(string key)
        {
            if (this.IsTablixChild)
            {
                if (!string.IsNullOrEmpty(key) && this.Model.ExpressionEngine.FieldInformations.ContainsKey(key))
                {
                    var fields = this.Model.ExpressionEngine.FieldInformations[key];

                    foreach (var field in fields)
                    {
                        this.AddFields(field);
                    }
                }
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

        #endregion


        void ParseSubReport()
        {
            SubReport subReport = this.ReportItem as SubReport;
            this.subReportItemExpPro = new SubReportItemExp();

            if (subReport.Style != null)
            {
                this.subReportItemExpPro.BackgroundColor = this.GetExpressionKey(subReport.Style.BackgroundColor);
                this.subReportItemExpPro.Border = this.GetEdgesBorder(subReport.Style);

                if (subReport.Style.BackgroundImage != null)
                {
                    this.subReportItemExpPro.BackgroundImage = this.GetImage(subReport.Style);
                }
            }
            if (subReport.Visibility != null)
            {
                this.subReportItemExpPro.Hidden = this.GetExpressionKey(subReport.Visibility.Hidden);
                this.ToggleItem = subReport.Visibility.ToggleItem;
            }

            this.subReportItemExpPro.Zindex = this.GetExpressionKey(subReport.ZIndex.ToString());

            if (subReport.Bookmark != null)
            {
                this.subReportItemExpPro.BookMark = this.GetExpressionKey(subReport.Bookmark);
            }

            if (subReport.DocumentMapLabel != null)
            {
                this.subReportItemExpPro.DocumentMapLabel = this.GetExpressionKey(subReport.DocumentMapLabel);
                this.DocumentMapLable = this.subReportItemExpPro.DocumentMapLabel;
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

        #region overridden methods

        public override void Load()
        {
            this.SubreportReportModel.ReportLoaded += new ReportLoadedEventHandler(SubreportReportModel_ReportLoaded);
            string reportPath = this.SubReport.ReportName;

            if (!string.IsNullOrEmpty(this.Model.ReportServerUrl))
            {
                if(!(reportPath.Trim().StartsWith(@"/")))
                {
                    string parentpath = this.Model.ReportPath;
                    int lastIndex = parentpath.LastIndexOf("/");
                    reportPath = parentpath.Substring(0,lastIndex+1) + reportPath;  
                }

                this.SubreportReportModel.ReportPath = reportPath;
                this.SubreportReportModel.ProcessReport();
            }
            else
            {  
                if(!string.IsNullOrEmpty(this.SubReport.ReportName) && !string.IsNullOrEmpty(this.Model.ReportPath))
                {
                    if(this.Model.IsRDLC)
                    {
                        reportPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.Model.ReportPath), reportPath) + ".rdlc";
                    }
                    else 
                    {
                        reportPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.Model.ReportPath), reportPath) + ".rdl";
                    }
#if !WINRT
                    if (System.IO.File.Exists(reportPath))
#endif
                    {
                        this.SubreportReportModel.ReportPath = reportPath;
                        this.SubreportReportModel.ProcessReport();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(reportPath) && this.Model.SubReportStream.ContainsKey(reportPath))
                    {
                        this.SubreportReportModel.LoadReport(this.Model.SubReportStream[reportPath]);
                    }
                }
            }
        }

        void SubreportReportModel_ReportLoaded(object sender, EventArgs e)
        {
            this.IsLoaded = true;
            this.SubreportReportModel.ReportLoaded -= new ReportLoadedEventHandler(SubreportReportModel_ReportLoaded);

            this.ReportItemModelers = new ReportModelContentCollection();
            foreach (var model in this.SubreportReportModel.BodyReportItemModels)
            {
                model.IsSubReportChild = true;
                if (model.ContainerModel == null)
                {
                    model.ContainerModel = this;
                }

                if (this.KeepTogether)
                {
                    model.KeepTogether = true;
                }

                model.Name = this.SubReport.Name + "_" + model.Name;
        //        model.ReportItem.Name = this.SubReport.Name + "_" + model.Name;
                this.ReportItemModelers.Add(model);
            }

            this.RaiseReportItemLoaded(null);
        }

        public override void Evaluate()
        {
            try
            {
                this.SubreportReportModel.EnableVirtualEvaluation = this.Model.EnableVirtualEvaluation;

                this.SubReportItemExpPro = new SubReportItemExpVal();

                if (this.subReportItemExpPro.BackgroundColor != null)
                {
                    this.SubReportItemExpPro.BackgroundColor = this.Model.ExpressionEngine.GetEvalExpressionString(this.subReportItemExpPro.BackgroundColor);
                }
                if (this.subReportItemExpPro.BackgroundImage != null)
                {
                    this.SubReportItemExpPro.BackgroundImage = this.GetImage(this.subReportItemExpPro.BackgroundImage);
                }
                if (this.subReportItemExpPro.Border != null)
                {
                    this.SubReportItemExpPro.Border = this.GetEdgesBorder(this.subReportItemExpPro.Border);
                }
                if (!string.IsNullOrEmpty(this.ToggleItem))
                {
                    this.GetTextBoxModel(this.ToggleItem);
                }
                this.SubReportItemExpPro.Hidden = TryParse<bool>(this.Model.ExpressionEngine.GetEvalExpressionString(this.subReportItemExpPro.Hidden));
                this.Hidden = this.SubReportItemExpPro.Hidden;
                this.SubReportItemExpPro.Zindex = TryParse<int>(this.Model.ExpressionEngine.GetEvalExpressionString(this.subReportItemExpPro.Zindex));
                this.SubReportItemExpPro.BookMark = this.Model.ExpressionEngine.GetEvalExpressionString(this.subReportItemExpPro.BookMark);
                this.SubReportItemExpPro.DocumentMapLabel = this.Model.ExpressionEngine.GetEvalExpressionString(this.subReportItemExpPro.DocumentMapLabel);
                this.DocumentMapLable = this.SubReportItemExpPro.DocumentMapLabel;
                if (!string.IsNullOrEmpty(this.DocumentMapLable))
                {
                    this.SetTreeModel();
                }
#if WINRT
            List<Syncfusion.UI.Xaml.Reports.ReportParameter> reportParameters = new List<Syncfusion.UI.Xaml.Reports.ReportParameter>();

            foreach (var parameter in this.SubReport.Parameters)
            {
                Syncfusion.UI.Xaml.Reports.ReportParameter meter = new Syncfusion.UI.Xaml.Reports.ReportParameter();
                meter.Name = parameter.Name;
                string valuestring = this.Model.ExpressionEngine.GetEvalExpressionString(parameter.Value);
                meter.Values.Add(valuestring);
                meter.Labels.Add(valuestring);
                reportParameters.Add(meter);
            }

#elif MVC
            List<Syncfusion.Reports.Mvc.ReportParameter> reportParameters = new List<Syncfusion.Reports.Mvc.ReportParameter>();

            foreach (var parameter in this.SubReport.Parameters)
            {
                Syncfusion.Reports.Mvc.ReportParameter meter = new Syncfusion.Reports.Mvc.ReportParameter();
                meter.Name = parameter.Name;
                string valuestring = this.Model.ExpressionEngine.GetEvalExpressionString(parameter.Value);
                meter.Values.Add(valuestring);
                meter.Labels.Add(valuestring);
                reportParameters.Add(meter);
            }
#else
                List<Syncfusion.Windows.Reports.ReportParameter> reportParameters = new List<Syncfusion.Windows.Reports.ReportParameter>();

                foreach (var parameter in this.SubReport.Parameters)
                {
                    Syncfusion.Windows.Reports.ReportParameter meter = new Syncfusion.Windows.Reports.ReportParameter();
                    meter.Name = parameter.Name;
                    string valuestring = this.Model.ExpressionEngine.GetEvalExpressionString(parameter.Value);
                    meter.Values.Add(valuestring);
                    meter.Labels.Add(valuestring);
                    reportParameters.Add(meter);
                }
#endif
                this.SubreportReportModel.SetParameters(reportParameters);

                this.SubreportReportModel.ProcessedData.ResetProceesedData();

                if (this.SubreportReportModel.IsRDLC == true)
                {
                    SubreportProcessingEventArgs subReportEventArgs = new SubreportProcessingEventArgs();
                    subReportEventArgs.ReportPath = this.SubReport.ReportName;
                    subReportEventArgs.DataSourceName = this.SubreportReportModel.GetDataSetNames();
                    subReportEventArgs.DataSources = new ReportDataSourceCollection();
                    subReportEventArgs.Parameters = this.SubreportReportModel.GetParameters();
                    this.Model.RaiseSubReportProcessingEvent(subReportEventArgs);
                    this.SubreportReportModel.DataSources = subReportEventArgs.DataSources;
                    this.SubreportReportModel.InitilizeReport();
                    this.SubreportReportModel.SubreportProcessing += SubreportReportModel_SubreportProcessing;

                    if (this.ReportItemModelers != null)
                    {
                        foreach (var itemModel in this.ReportItemModelers.Where(item => item.ContainerModel == this))
                        {
                            itemModel.Evaluate();
                        }
                    }

                    this.SubreportReportModel.SubreportProcessing -= SubreportReportModel_SubreportProcessing;
                    base.Evaluate();
                }
                else
                {
                    this.SubreportReportModel.DataSourceUpdated += SubreportReportModel_DataSourceUpdated;
                    this.SubreportReportModel.InitilizeReport();
                }
            }
            catch (Exception e)
            {
                this.Model.ExceptionDetails.Add("Getting following exception while evaluate the expression in " + this.ReportItem.Name + " " + e.Message);
            }
        }

        void SubreportReportModel_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            this.Model.RaiseSubReportProcessingEvent(e);
        }

        void SubreportReportModel_DataSourceUpdated(object sender, EventArgs e)
        {
            this.SubreportReportModel.DataSourceUpdated -= SubreportReportModel_DataSourceUpdated;

            if (this.ReportItemModelers != null)
            {
                foreach (var itemModel in this.ReportItemModelers.Where(item=> item.ContainerModel == this))
                {
                    itemModel.Evaluate();
                }
            }

            base.Evaluate();
        }

        public override void DisposeEvalObjects()
        {
            this.SubReportItemExpPro = null;
            if (this.ReportItemModelers != null)
            {
                foreach (var reportItem in this.ReportItemModelers)
                {
                    reportItem.DisposeEvalObjects();
                }
            }
            base.DisposeEvalObjects();
        }

        private void SetTreeModel()
        {
            if (this.Model.MapModel == null)
            {
                this.Model.MapModel = new DocumentMapModel();
            }
            DocumentData node = new DocumentData();
            node.DocumentLable = this.DocumentMapLable;
            node.ModelType = ModelType.SubReportModel;
            node.ReportItemName = this.Name;
            node.IsTablixChild = this.IsTablixChild;
            this.DocumentNodeRefer = node;
            this.Model.MapModel.NodeData.Add(node);
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
            if (this.IsTablixInnerChild)
            {
                this.FlowLayoutInfo = null;
            }
            base.DisposeReportItemObj();
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

            if (this.ReportItemModelers != null)
            {
                foreach (var model in this.ReportItemModelers)
                {
                    if (itemViewMode == ReportItemViewMode.Normal && model.PageInfo != null)
                    {
                        reportBottomValue = model.PageInfo.ActualTop + model.PageInfo.ActualHeight;
                    }
                    else if (itemViewMode == ReportItemViewMode.Print && model.PrintPageInfo != null)
                    {
                        reportBottomValue = model.PrintPageInfo.ActualTop + model.PrintPageInfo.ActualHeight;
                    }
                    else if (model.FlowLayoutInfo != null)
                    {
                        reportBottomValue = model.FlowLayoutInfo.ActualTop + model.FlowLayoutInfo.ActualHeight;
                    }

                    maxBottomValue = reportBottomValue > maxBottomValue ? reportBottomValue : maxBottomValue;
                }
            }

            locationInfo.ActualHeight = maxBottomValue - locationInfo.ActualTop;

            if (itemViewMode != ReportItemViewMode.None)
            {
                Dictionary<int, PageInfo> pageSizes = new Dictionary<int, PageInfo>();

                List<double> pageWidths = itemViewMode == ReportItemViewMode.Normal ? this.PageWidths : this.PrintPageWidths;

                double abortHeight = preferredHeight;
                var hasHeightInfo = !double.IsNaN(firstPageHeight);
                var hasFirstPreferredHeight = !double.IsNaN(firstPageHeight);
                int pageCount = 0;
                double totalHeight = locationInfo.ActualHeight;

                if ((itemViewMode != ReportItemViewMode.Print && (!hasHeightInfo || this.KeepTogether)) || (hasHeightInfo && totalHeight <= firstPageHeight))
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

                    while (!hightReachedEnd)
                    {
                        if (!isFirstPageSet)
                        {
                            foreach (var column in pageWidths)
                            {
                                pageSizes.Add(pageCount++, new PageInfo() { Height = firstPageHeight, Width = column });
                            }

                            totalHeight -= firstPageHeight;
                            isFirstPageSet = true;
                        }
                        else
                        {
                            double calHeight = abortHeight;

                            if (totalHeight <= abortHeight)
                            {
                                hightReachedEnd = true;
                                calHeight = totalHeight;
                            }

                            foreach (var column in pageWidths)
                            {
                                pageSizes.Add(pageCount++, new PageInfo() { Height = calHeight, Width = column });
                            }

                            totalHeight -= abortHeight;
                        }
                    }
                }

                if (itemViewMode == ReportItemViewMode.Print)
                {
                    this.PrintPageColumnCount = pageWidths.Count;
                    this.PrintPageSizes = pageSizes;
                }
                else
                {
                    this.PageSizes = pageSizes;
                }

                locationInfo.TotalPages = pageSizes.Count;
            }
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
                foreach (var model in this.ReportItemModelers)
                {
                    double reportRightValue = 0;

                    if (itemViewMode == ReportItemViewMode.Print && model.PageInfo != null)
                    {
                        reportRightValue = model.PrintPageInfo.ActualLeft + model.PrintPageInfo.ActualWidth;
                    }
                    else if (itemViewMode == ReportItemViewMode.Normal && model.PageInfo != null)
                    {
                        reportRightValue = model.PageInfo.ActualLeft + model.PageInfo.ActualWidth;
                    }
                    else if (model.FlowLayoutInfo != null)
                    {
                        reportRightValue = model.FlowLayoutInfo.ActualLeft + model.FlowLayoutInfo.ActualWidth;
                    }

                    maxRightValue = reportRightValue > maxRightValue ? reportRightValue : maxRightValue;
                }
            }

            locationInfo.ActualWidth = maxRightValue - locationInfo.ActualLeft;

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

        #region DataType Convert Wapper Method

        // clr generic datatype converter 
        internal T TryParse<T>(object value)
        {
            try
            {
                if (value == null)
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