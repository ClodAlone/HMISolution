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
    internal class GaugeModel
        : ReportItemModeler
    {
        #region members

        GaugePanelExp GaugePanelExpProp;
        ReportingAggEngine Engine;

        //private double actualHeight = 0;
        //private double actualWidth = 0;

        #endregion

        #region  properties

        public GaugePanelExpVal GaugePanelProperties
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

        #endregion

        #region Constructors

        internal GaugeModel(ReportItem reportItem, ReportModel pageModel, bool isTablixChild, string dataSetName)
        {
            this.DataSetFields = new List<DataField>();
            this.Model = pageModel;
            this.ReportItem = reportItem;
            this.ModelType = ModelType.GaugeModel;
            this.IsTablixChild = isTablixChild;

            this.Name = reportItem.Name;

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

            GaugePanel gauge = (reportItem as GaugePanel);
            this.DataSetName = gauge.DataSetName;

            if (gauge.PageBreak != null)
            {
                this.PageBreak = gauge.PageBreak.BreakLocation;
            }

            if (this.DataSetName == null)
            {
                this.DataSetName = dataSetName;
            }

            this.ParseGauge();
        }

        public GaugeModel()
        {
            // TODO: Complete member initialization
        }

        void ParseGauge()
        {
            GaugePanel gaugepanel = this.ReportItem as GaugePanel;
            this.GaugePanelExpProp = new GaugePanelExp();
            RDL.DOM.Style style = gaugepanel.Style;
            this.GaugePanelExpProp.RadialGauges = new List<GaugePropertiesExp>();

            var dataRegion = this.ReportItem as DataRegion;
            if (dataRegion != null)
            {
                this.ExpFilters = this.Model.ParseFilters(dataRegion.Filters, this.DataSetName, true);
            }

            this.GaugePanelExpProp.AntiAliasing = this.GetExpressionKey(gaugepanel.AntiAliasing.ToString());

            this.GaugePanelExpProp.AutoLayout = gaugepanel.AutoLayout;

            this.GaugePanelExpProp.ShadowIntensity = this.GetExpressionKey(gaugepanel.ShadowIntensity.ToString());


            if (gaugepanel.Visibility != null)
            {
                if (gaugepanel.Visibility.Hidden != null)
                {
                    this.GaugePanelExpProp.Hidden = this.GetExpressionKey(gaugepanel.Visibility.Hidden);
                }
                if (gaugepanel.Visibility.ToggleItem != null)
                {
                    this.ToggleItem = gaugepanel.Visibility.ToggleItem;
                }
            }

            this.GaugePanelExpProp.Filters = new List<FilterExp>();
            if (gaugepanel.Filters != null)
            {
                foreach (Filter filter in gaugepanel.Filters)
                {
                    FilterExp Filter = new FilterExp();
                    Filter.FilterExpression = this.GetExpressionKey(filter.FilterExpression);

                    this.GaugePanelExpProp.Filters.Add(Filter);
                }
            }
            this.GaugePanelExpProp.GaugeFrame = new FramePropertiesExp();

            if (gaugepanel.BackFrame != null)
            {
                this.GaugePanelExpProp.GaugeFrame.FrameGlassEffect = this.GetExpressionKey(gaugepanel.BackFrame.GlassEffect.ToString());

                this.GaugePanelExpProp.GaugeFrame.FrameImage = new BaseImageExp();

                this.GaugePanelExpProp.GaugeFrame.FrameImage.HueColor = this.GetExpressionKey(gaugepanel.BackFrame.FrameImage.HueColor);

                this.GaugePanelExpProp.GaugeFrame.FrameImage.ClipImage = gaugepanel.BackFrame.FrameImage.ClipImage;

                this.GaugePanelExpProp.GaugeFrame.FrameImage.Transparency = this.GetExpressionKey(gaugepanel.BackFrame.FrameImage.Transparency.ToString());

                this.GaugePanelExpProp.GaugeFrame.FrameImage.MIMEType = this.GetExpressionKey(gaugepanel.BackFrame.FrameImage.MIMEType);

                this.GaugePanelExpProp.GaugeFrame.FrameImage.Source = this.GetExpressionKey(gaugepanel.BackFrame.FrameImage.Source.ToString());

                this.GaugePanelExpProp.GaugeFrame.FrameImage.TransparentColor = this.GetExpressionKey(gaugepanel.BackFrame.FrameImage.TransparentColor);

                this.GaugePanelExpProp.GaugeFrame.FrameImage.Value = this.GetExpressionKey(gaugepanel.BackFrame.FrameImage.Value.ToString());


                this.GaugePanelExpProp.GaugeFrame.BackFrameStyle = new GaugeStyleExp();
                this.GaugePanelExpProp.GaugeFrame.FrameShape = this.GetExpressionKey(gaugepanel.BackFrame.FrameShape.ToString());

                this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BorderColor = this.GetExpressionKey(gaugepanel.BackFrame.Style.Border.Color);

                this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BorderStyle = this.GetExpressionKey(gaugepanel.BackFrame.Style.Border.Style);
                if (gaugepanel.BackFrame.Style.Border.Width != null)
                {
                    this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BorderWidth = this.GetExpressionKey(gaugepanel.BackFrame.Style.Border.Width.size);

                }
                this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BackgroundColor = this.GetExpressionKey(gaugepanel.BackFrame.Style.BackgroundColor);

                this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BackgroundGradientEndcolor = this.GetExpressionKey(gaugepanel.BackFrame.Style.BackgroundGradientEndColor);

                this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BackgroundGradientType = this.GetExpressionKey(gaugepanel.BackFrame.Style.BackgroundGradientType.ToString());

                this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BackgroundHatchType = this.GetExpressionKey(gaugepanel.BackFrame.Style.BackgroundHatchType.ToString());

                this.GaugePanelExpProp.GaugeFrame.FrameStyle = this.GetExpressionKey(gaugepanel.BackFrame.FrameStyle.ToString());

                this.GaugePanelExpProp.GaugeFrame.FrameWidth = this.GetExpressionKey(gaugepanel.BackFrame.FrameWidth.ToString());
            }

            this.GaugePanelExpProp.TextAntiAliasingQuality = this.GetExpressionKey(gaugepanel.TextAntiAliasingQuality.ToString());


            this.GaugePanelExpProp.TopImage = new BaseImageExp();
            if (gaugepanel.TopImage != null)
            {
                this.GaugePanelExpProp.TopImage.HueColor = this.GetExpressionKey(gaugepanel.TopImage.HueColor);

                this.GaugePanelExpProp.TopImage.MIMEType = this.GetExpressionKey(gaugepanel.TopImage.MIMEType);

                this.GaugePanelExpProp.TopImage.Source = this.GetExpressionKey(gaugepanel.TopImage.Source.ToString());

                this.GaugePanelExpProp.TopImage.TransparentColor = this.GetExpressionKey(gaugepanel.TopImage.TransparentColor);

                this.GaugePanelExpProp.TopImage.Value = this.GetExpressionKey(gaugepanel.TopImage.Value.ToString());
            }

            this.GaugePanelExpProp.AutoLayout = gaugepanel.AutoLayout;

            this.GaugePanelExpProp.Border = new BorderExp();
            if (style.Border != null)
            {
                this.GaugePanelExpProp.Border.Default = new BorderExpProperties();
                this.GaugePanelExpProp.Border.Default.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(style.Border.Color);
                this.AddDataFields(style.Border.Color);
                this.GaugePanelExpProp.Border.Default.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(style.Border.Style);
                this.AddDataFields(style.Border.Style);
                if (style.Border.Width != null)
                {
                    this.GaugePanelExpProp.Border.Default.Thickness = this.Model.ExpressionEngine.GetExpressionKey(style.Border.Width.size);
                    this.AddDataFields(style.Border.Width.size);
                }
            }
            if (style.LeftBorder != null)
            {
                this.GaugePanelExpProp.Border.LeftBorder = new BorderExpProperties();
                this.GaugePanelExpProp.Border.LeftBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(style.LeftBorder.Color);
                this.AddDataFields(style.LeftBorder.Color);
                this.GaugePanelExpProp.Border.Default.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(style.LeftBorder.Style);
                this.AddDataFields(style.LeftBorder.Style);
                if (style.LeftBorder.Width != null)
                {
                    this.GaugePanelExpProp.Border.LeftBorder.Thickness = this.Model.ExpressionEngine.GetExpressionKey(style.LeftBorder.Width.size);
                    this.AddDataFields(style.LeftBorder.Width.size);
                }
            }
            if (style.TopBorder != null)
            {
                this.GaugePanelExpProp.Border.TopBorder = new BorderExpProperties();
                this.GaugePanelExpProp.Border.TopBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(style.TopBorder.Color);
                this.AddDataFields(style.TopBorder.Color);
                this.GaugePanelExpProp.Border.TopBorder.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(style.TopBorder.Style);
                this.AddDataFields(style.TopBorder.Style);
                if (style.TopBorder.Width != null)
                {
                    this.GaugePanelExpProp.Border.TopBorder.Thickness = this.Model.ExpressionEngine.GetExpressionKey(style.TopBorder.Width.size);
                    this.AddDataFields(style.TopBorder.Width.size);
                }
            }
            if (style.RightBorder != null)
            {
                this.GaugePanelExpProp.Border.RightBorder = new BorderExpProperties();
                this.GaugePanelExpProp.Border.RightBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(style.RightBorder.Color);
                this.AddDataFields(style.RightBorder.Color);
                this.GaugePanelExpProp.Border.Default.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(style.RightBorder.Style);
                this.AddDataFields(style.RightBorder.Style);
                if (style.RightBorder.Width != null)
                {
                    this.GaugePanelExpProp.Border.RightBorder.Thickness = this.Model.ExpressionEngine.GetExpressionKey(style.RightBorder.Width.size);
                    this.AddDataFields(style.RightBorder.Width.size);
                }
            }
            if (style.BottomBorder != null)
            {
                this.GaugePanelExpProp.Border.BottomBorder = new BorderExpProperties();
                this.GaugePanelExpProp.Border.BottomBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(style.BottomBorder.Color);
                this.AddDataFields(style.BottomBorder.Color);
                this.GaugePanelExpProp.Border.BottomBorder.BorderStyle = this.Model.ExpressionEngine.GetExpressionKey(style.BottomBorder.Style);
                this.AddDataFields(style.BottomBorder.Style);
                if (style.BottomBorder.Width != null)
                {
                    this.GaugePanelExpProp.Border.BottomBorder.Thickness = this.Model.ExpressionEngine.GetExpressionKey(style.BottomBorder.Width.size);
                    this.AddDataFields(style.BottomBorder.Width.size);
                }
            }

            this.GaugePanelExpProp.BackgroundColor = this.GetExpressionKey(style.BackgroundColor);

            this.GaugePanelExpProp.PageName = this.GetExpressionKey(gaugepanel.PageName);

            this.GaugePanelExpProp.ToolTip = this.GetExpressionKey(gaugepanel.ToolTip);

            this.GaugePanelExpProp.Direction = this.GetExpressionKey(gaugepanel.Style.Direction.ToString());

            this.GaugePanelExpProp.NumeralLanguage = this.GetExpressionKey(style.NumeralLanguage);

            this.GaugePanelExpProp.NumeralVariant = this.GetExpressionKey(style.NumeralVariant);

            this.GaugePanelExpProp.Language = this.GetExpressionKey(style.Language);

            this.GaugePanelExpProp.Calender = this.GetExpressionKey(style.Calendar.ToString());

            this.GaugePanelExpProp.BookMark = this.GetExpressionKey(gaugepanel.Bookmark);

            this.GaugePanelExpProp.DocumentMapLabel = this.GetExpressionKey(gaugepanel.DocumentMapLabel);
            this.DocumentMapLable = this.GaugePanelExpProp.DocumentMapLabel;

            this.GaugePanelExpProp.ZIndex = this.GetExpressionKey(gaugepanel.ZIndex.ToString());


            this.GaugePanelExpProp.GaugeLabels = new List<GaugeLabelExp>();
            if (gaugepanel.GaugeLabels != null)
            {
                foreach (GaugeLabel gaugelabel in gaugepanel.GaugeLabels)
                {
                    RDL.DOM.Style labelstyle = gaugelabel.Style;
                    var label = new GaugeLabelExp();
                    label.LabelStyle = new GaugeStyleExp();

                    label.LabelStyle.BackgroundColor = this.GetExpressionKey(labelstyle.BackgroundColor);

                    label.LabelStyle.BackgroundGradientEndcolor = this.GetExpressionKey(labelstyle.BackgroundGradientEndColor);

                    label.LabelStyle.BackgroundGradientType = this.GetExpressionKey(labelstyle.BackgroundGradientType.ToString());

                    label.LabelStyle.BackgroundHatchType = this.GetExpressionKey(labelstyle.BackgroundHatchType.ToString());

                    label.LabelStyle.OffSet = this.GetExpressionKey(labelstyle.ShadowOffset);

                    label.LabelStyle.BorderColor = this.GetExpressionKey(labelstyle.Border.Color);

                    label.LabelStyle.BorderStyle = this.GetExpressionKey(labelstyle.Border.Style);

                    if (labelstyle.Border.Width != null)
                    {
                        label.LabelStyle.BorderWidth = this.GetExpressionKey(labelstyle.Border.Width.size);

                    }
                    label.LabelStyle.FontFamily = this.GetExpressionKey(labelstyle.FontFamily);

                    label.LabelStyle.FontSize = this.GetExpressionKey(labelstyle.FontSize.size);

                    label.LabelStyle.FontStyle = this.GetExpressionKey(labelstyle.FontStyle);

                    label.LabelStyle.FontWeight = this.GetExpressionKey(labelstyle.FontWeight);

                    label.Hidden = gaugelabel.Hidden;

                    label.TextAlign = this.GetExpressionKey(labelstyle.TextAlign);

                    label.TextColor = this.GetExpressionKey(labelstyle.Color);

                    label.TextDecoration = this.GetExpressionKey(labelstyle.TextDecoration);

                    label.TextShadowOffset = this.GetExpressionKey(gaugelabel.TextShadowOffset.size);

                    label.VerticalAlign = this.GetExpressionKey(labelstyle.VerticalAlign);

                    label.UseFontPercent = gaugelabel.UseFontPercent;

                    label.ResizeMode = this.GetExpressionKey(gaugelabel.ResizeMode.ToString());

                    label.ToolTip = this.GetExpressionKey(gaugelabel.ToolTip);

                    label.Text = this.GetExpressionKey(gaugelabel.Text);

                    label.ZIndex = this.GetExpressionKey(gaugelabel.ZIndex.ToString());

                    label.LabelStyle.BackgroundColor = this.GetExpressionKey(labelstyle.BackgroundColor);

                    label.Top = this.GetExpressionKey(gaugelabel.Top.ToString());

                    label.Left = this.GetExpressionKey(gaugelabel.Left.ToString());

                    label.Height = this.GetExpressionKey(gaugelabel.Height.ToString());

                    label.Width = this.GetExpressionKey(gaugelabel.Width.ToString());

                    label.Angle = this.GetExpressionKey(gaugelabel.Angle.ToString());

                    if (gaugelabel.ActionInfo != null)
                    {
                        foreach (RDL.DOM.Action Action in gaugelabel.ActionInfo.Actions)
                        {
                            label.ActionInfo = new ActionInfoExp();
                            label.ActionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);

                            label.ActionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);


                            if (Action.Drillthrough != null)
                            {
                                label.ActionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);

                                label.ActionInfo.Parameters = new List<ParameterExp>();
                                foreach (Parameter parameters in Action.Drillthrough.Parameters)
                                {
                                    var Parameter = new ParameterExp();
                                    Parameter.Name = this.GetExpressionKey(parameters.Name);

                                    Parameter.Omit = this.GetExpressionKey(parameters.Omit);

                                    Parameter.Value = this.GetExpressionKey(parameters.Value);

                                    label.ActionInfo.Parameters.Add(Parameter);
                                }
                            }
                        }
                    }
                    this.GaugePanelExpProp.GaugeLabels.Add(label);
                }
            }

            if (gaugepanel.StateIndicators != null)
            {
                this.GaugePanelExpProp.Indicators = new List<IndicatorsExp>();

                foreach (var stateIndicator in gaugepanel.StateIndicators)
                {
                    IndicatorsExp indicators = new IndicatorsExp();
                    indicators.Hidden = this.GetExpressionKey(stateIndicator.Hidden);
                    indicators.Angle = this.GetExpressionKey(stateIndicator.Angle);
                    indicators.ToolTip = this.GetExpressionKey(stateIndicator.ToolTip);
                    indicators.Top = this.GetExpressionKey(stateIndicator.Top);
                    indicators.Left = this.GetExpressionKey(stateIndicator.Left);
                    indicators.Height = this.GetExpressionKey(stateIndicator.Height);
                    indicators.Width = this.GetExpressionKey(stateIndicator.Width);
                    indicators.ZIndex = this.GetExpressionKey(stateIndicator.ZIndex);
                    indicators.GaugeIndicatorStyle = this.GetExpressionKey(stateIndicator.IndicatorStyle);
                    indicators.ScaleFactor = this.GetExpressionKey(stateIndicator.ScaleFactor);
                    indicators.IconSet = this.GetExpressionKey(stateIndicator.IconsSet);
                    indicators.TransformationType = this.GetExpressionKey(stateIndicator.TransformationType);
                    indicators.FillColor = this.GetExpressionKey(stateIndicator.Style.BackgroundColor);

                    indicators.MaximumValue = new GaugeInputValueExp();
                    indicators.MaximumValue.AddConstant = this.GetExpressionKey(stateIndicator.MaximumValue.AddConstant.ToString());
                    indicators.MaximumValue.DataElementName = this.GetExpressionKey(stateIndicator.MaximumValue.DataElementName);
                    indicators.MaximumValue.DataElementOutput = this.GetExpressionKey(stateIndicator.MaximumValue.DataElementOutput.ToString());
                    indicators.MaximumValue.Formula = this.GetExpressionKey(stateIndicator.MaximumValue.Formula.ToString());
                    indicators.MaximumValue.MaxPercent = this.GetExpressionKey(stateIndicator.MaximumValue.MaxPercent.ToString());
                    indicators.MaximumValue.MinPercent = this.GetExpressionKey(stateIndicator.MaximumValue.MinPercent.ToString());
                    indicators.MaximumValue.Multiplier = this.GetExpressionKey(stateIndicator.MaximumValue.Multiplier.ToString());
                    indicators.MaximumValue.Value = this.GetExpressionKey(stateIndicator.MaximumValue.Value);

                    indicators.MinimumValue = new GaugeInputValueExp();
                    indicators.MinimumValue.AddConstant = this.GetExpressionKey(stateIndicator.MinimumValue.AddConstant.ToString());
                    indicators.MinimumValue.DataElementName = this.GetExpressionKey(stateIndicator.MinimumValue.DataElementName);
                    indicators.MinimumValue.DataElementOutput = this.GetExpressionKey(stateIndicator.MinimumValue.DataElementOutput.ToString());
                    indicators.MinimumValue.Formula = this.GetExpressionKey(stateIndicator.MinimumValue.Formula.ToString());
                    indicators.MinimumValue.MaxPercent = this.GetExpressionKey(stateIndicator.MinimumValue.MaxPercent.ToString());
                    indicators.MinimumValue.MinPercent = this.GetExpressionKey(stateIndicator.MinimumValue.MinPercent.ToString());
                    indicators.MinimumValue.Multiplier = this.GetExpressionKey(stateIndicator.MinimumValue.Multiplier.ToString());
                    indicators.MinimumValue.Value = this.GetExpressionKey(stateIndicator.MinimumValue.Value);

                    indicators.IndicatorData = new GaugeInputValueExp();
                    indicators.IndicatorData.AddConstant = this.GetExpressionKey(stateIndicator.GaugeInputValue.AddConstant.ToString());
                    indicators.IndicatorData.DataElementName = this.GetExpressionKey(stateIndicator.GaugeInputValue.DataElementName);
                    indicators.IndicatorData.DataElementOutput = this.GetExpressionKey(stateIndicator.GaugeInputValue.DataElementOutput.ToString());
                    indicators.IndicatorData.Formula = this.GetExpressionKey(stateIndicator.GaugeInputValue.Formula.ToString());
                    indicators.IndicatorData.MaxPercent = this.GetExpressionKey(stateIndicator.GaugeInputValue.MaxPercent.ToString());
                    indicators.IndicatorData.MinPercent = this.GetExpressionKey(stateIndicator.GaugeInputValue.MinPercent.ToString());
                    indicators.IndicatorData.Multiplier = this.GetExpressionKey(stateIndicator.GaugeInputValue.Multiplier.ToString());
                    indicators.IndicatorData.Value = this.GetExpressionKey(stateIndicator.GaugeInputValue.Value);

                    if (stateIndicator.ActionInfo != null)
                    {
                        indicators.ActionInfo = new List<ActionInfoExp>();
                        foreach (var Action in stateIndicator.ActionInfo.Actions)
                        {
                            ActionInfoExp actionInfo = new ActionInfoExp();
                            actionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);
                            actionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);

                            if (Action.Drillthrough != null)
                            {
                                actionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);
                                actionInfo.Parameters = new List<ParameterExp>();
                                foreach (Parameter parameters in Action.Drillthrough.Parameters)
                                {
                                    ParameterExp Parameter = new ParameterExp();
                                    Parameter.Name = this.GetExpressionKey(parameters.Name);
                                    Parameter.Omit = this.GetExpressionKey(parameters.Omit);
                                    Parameter.Value = this.GetExpressionKey(parameters.Value);
                                    actionInfo.Parameters.Add(Parameter);
                                }
                            }
                            indicators.ActionInfo.Add(actionInfo);
                        }
                    }

                    if (stateIndicator.IndicatorStates != null && stateIndicator.IndicatorStates.Count > 0)
                    {
                        indicators.IndicatorState = new List<IndicatorStateExp>();
                        foreach (var indicatorState in stateIndicator.IndicatorStates)
                        {
                            IndicatorStateExp indicatorStateExp = new IndicatorStateExp();
                            indicatorStateExp.ResizeMode = this.GetExpressionKey(indicatorState.ResizeMode);
                            indicatorStateExp.FillColor = this.GetExpressionKey(indicatorState.Color);
                            indicatorStateExp.ScaleFactor = this.GetExpressionKey(indicatorState.ScaleFactor);
                            indicatorStateExp.IndicatorStyle = this.GetExpressionKey(indicatorState.IndicatorStyle);


                            indicatorStateExp.StartValue = new GaugeInputValueExp();
                            indicatorStateExp.StartValue.AddConstant = this.GetExpressionKey(indicatorState.StartValue.AddConstant.ToString());
                            indicatorStateExp.StartValue.DataElementName = this.GetExpressionKey(indicatorState.StartValue.DataElementName);
                            indicatorStateExp.StartValue.DataElementOutput = this.GetExpressionKey(indicatorState.StartValue.DataElementOutput.ToString());
                            indicatorStateExp.StartValue.Formula = this.GetExpressionKey(indicatorState.StartValue.Formula.ToString());
                            indicatorStateExp.StartValue.MaxPercent = this.GetExpressionKey(indicatorState.StartValue.MaxPercent.ToString());
                            indicatorStateExp.StartValue.MinPercent = this.GetExpressionKey(indicatorState.StartValue.MinPercent.ToString());
                            indicatorStateExp.StartValue.Multiplier = this.GetExpressionKey(indicatorState.StartValue.Multiplier.ToString());
                            indicatorStateExp.StartValue.Value = this.GetExpressionKey(indicatorState.StartValue.Value);

                            indicatorStateExp.EndValue = new GaugeInputValueExp();
                            indicatorStateExp.EndValue.AddConstant = this.GetExpressionKey(indicatorState.EndValue.AddConstant.ToString());
                            indicatorStateExp.EndValue.DataElementName = this.GetExpressionKey(indicatorState.EndValue.DataElementName);
                            indicatorStateExp.EndValue.DataElementOutput = this.GetExpressionKey(indicatorState.EndValue.DataElementOutput.ToString());
                            indicatorStateExp.EndValue.Formula = this.GetExpressionKey(indicatorState.EndValue.Formula.ToString());
                            indicatorStateExp.EndValue.MaxPercent = this.GetExpressionKey(indicatorState.EndValue.MaxPercent.ToString());
                            indicatorStateExp.EndValue.MinPercent = this.GetExpressionKey(indicatorState.EndValue.MinPercent.ToString());
                            indicatorStateExp.EndValue.Multiplier = this.GetExpressionKey(indicatorState.EndValue.Multiplier.ToString());
                            indicatorStateExp.EndValue.Value = this.GetExpressionKey(indicatorState.EndValue.Value);

                            if (indicatorState.IndicatorImage != null)
                            {
                                indicatorStateExp.HugeColor = this.GetExpressionKey(indicatorState.IndicatorImage.HueColor);
                                indicatorStateExp.Transparency = this.GetExpressionKey(indicatorState.IndicatorImage.Transparency);

                                indicatorStateExp.StateImage = new BaseImageExp();
                                if (indicatorState.IndicatorImage != null)
                                {
                                    indicatorStateExp.StateImage.HueColor = this.GetExpressionKey(indicatorState.IndicatorImage.HueColor);
                                    indicatorStateExp.StateImage.MIMEType = this.GetExpressionKey(indicatorState.IndicatorImage.MIMEType);
                                    indicatorStateExp.StateImage.Source = this.GetExpressionKey(indicatorState.IndicatorImage.Source.ToString());
                                    indicatorStateExp.StateImage.TransparentColor = this.GetExpressionKey(indicatorState.IndicatorImage.TransparentColor);
                                    indicatorStateExp.StateImage.Value = this.GetExpressionKey(indicatorState.IndicatorImage.Value.ToString());
                                }
                            }

                            indicators.IndicatorState.Add(indicatorStateExp);
                        }
                    }

                    this.GaugePanelExpProp.Indicators.Add(indicators);
                }
            }

            if (gaugepanel.LinearGauges != null && gaugepanel.LinearGauges.Count > 0)
            {
                this.GaugePanelExpProp.LinearGauges = new List<GaugePropertiesExp>();
                foreach (LinearGauge lineargauge in gaugepanel.LinearGauges)
                {
                    var Lineargauge = new GaugePropertiesExp();
                    Lineargauge.GaugeFrame = new FramePropertiesExp();
                    Lineargauge.GaugeFrame.FrameImage = new BaseImageExp();
                    Lineargauge.GaugeFrame.BackFrameStyle = new GaugeStyleExp();
                    Lineargauge.GaugeStyle = new GaugeStyleExp();
                    Lineargauge.GaugeTopImage = new BaseImageExp();
                    Lineargauge.GaugeFrame.FrameProperty = new GaugeStyleExp();

                    Lineargauge.GaugeFrame.FrameShape = this.GetExpressionKey(lineargauge.BackFrame.FrameShape.ToString());

                    Lineargauge.GaugeFrame.FrameStyle = this.GetExpressionKey(lineargauge.BackFrame.FrameStyle.ToString());

                    Lineargauge.GaugeStyle.BackgroundColor = this.GetExpressionKey(lineargauge.BackFrame.Style.BackgroundColor);

                    Lineargauge.GaugeStyle.BackgroundGradientEndcolor = this.GetExpressionKey(lineargauge.BackFrame.Style.BackgroundGradientEndColor);

                    Lineargauge.GaugeStyle.BackgroundGradientType = this.GetExpressionKey(lineargauge.BackFrame.Style.BackgroundGradientType.ToString());

                    Lineargauge.GaugeStyle.BackgroundHatchType = this.GetExpressionKey(lineargauge.BackFrame.Style.BackgroundHatchType.ToString());

                    Lineargauge.GaugeStyle.BorderColor = this.GetExpressionKey(lineargauge.BackFrame.Style.Border.Color);

                    Lineargauge.GaugeStyle.BorderStyle = this.GetExpressionKey(lineargauge.BackFrame.Style.Border.Style);

                    if (lineargauge.BackFrame.Style.Border.Width != null)
                    {
                        Lineargauge.GaugeStyle.BorderWidth = this.GetExpressionKey(lineargauge.BackFrame.Style.Border.Width.size);

                    }
                    Lineargauge.GaugeFrame.BackFrameStyle.BackgroundColor = this.GetExpressionKey(lineargauge.BackFrame.FrameBackground.Style.BackgroundColor);

                    Lineargauge.GaugeFrame.BackFrameStyle.BackgroundGradientEndcolor = this.GetExpressionKey(lineargauge.BackFrame.FrameBackground.Style.BackgroundGradientEndColor);

                    Lineargauge.GaugeFrame.BackFrameStyle.BackgroundGradientType = this.GetExpressionKey(lineargauge.BackFrame.FrameBackground.Style.BackgroundGradientType.ToString());

                    Lineargauge.GaugeFrame.BackFrameStyle.BackgroundHatchType = this.GetExpressionKey(lineargauge.BackFrame.FrameBackground.Style.BackgroundHatchType.ToString());

                    Lineargauge.GaugeFrame.FrameImage.MIMEType = this.GetExpressionKey(lineargauge.BackFrame.FrameImage.MIMEType);

                    Lineargauge.GaugeFrame.FrameImage.HueColor = this.GetExpressionKey(lineargauge.BackFrame.FrameImage.HueColor);

                    Lineargauge.GaugeFrame.FrameImage.Source = this.GetExpressionKey(lineargauge.BackFrame.FrameImage.Source.ToString());

                    Lineargauge.GaugeFrame.FrameImage.Transparency = this.GetExpressionKey(lineargauge.BackFrame.FrameImage.Transparency.ToString());

                    Lineargauge.GaugeFrame.FrameImage.TransparentColor = this.GetExpressionKey(lineargauge.BackFrame.FrameImage.TransparentColor);

                    Lineargauge.GaugeFrame.FrameImage.Value = this.GetExpressionKey(lineargauge.BackFrame.FrameImage.Value.ToString());

                    Lineargauge.GaugeFrame.FrameImage.ClipImage = lineargauge.BackFrame.FrameImage.ClipImage;

                    Lineargauge.GaugeFrame.FrameWidth = this.GetExpressionKey(lineargauge.BackFrame.FrameWidth.ToString());

                    Lineargauge.GaugeFrame.FrameGlassEffect = this.GetExpressionKey(lineargauge.BackFrame.GlassEffect.ToString());

                    Lineargauge.GaugeFrame.BackFrameStyle.OffSet = this.GetExpressionKey(lineargauge.BackFrame.Style.ShadowOffset);

                    Lineargauge.GaugeFrame.FrameProperty.BackgroundColor = this.GetExpressionKey(lineargauge.BackFrame.Style.BackgroundColor);


                    Lineargauge.Hidden = lineargauge.Hidden;

                    Lineargauge.GaugeTopImage.HueColor = this.GetExpressionKey(lineargauge.TopImage.HueColor);

                    Lineargauge.GaugeTopImage.MIMEType = this.GetExpressionKey(lineargauge.TopImage.MIMEType);

                    Lineargauge.GaugeTopImage.Source = this.GetExpressionKey(lineargauge.TopImage.Source.ToString());

                    Lineargauge.GaugeTopImage.TransparentColor = this.GetExpressionKey(lineargauge.TopImage.TransparentColor);

                    Lineargauge.GaugeTopImage.Value = this.GetExpressionKey(lineargauge.TopImage.Value.ToString());

                    Lineargauge.ClipContent = lineargauge.ClipContent;

                    Lineargauge.GaugeTooltip = this.GetExpressionKey(lineargauge.ToolTip);

                    Lineargauge.AspectRatio = this.GetExpressionKey(lineargauge.AspectRatio.ToString());

                    Lineargauge.GaugeHeight = this.GetExpressionKey(lineargauge.Height.ToString());

                    Lineargauge.GaugeWidth = this.GetExpressionKey(lineargauge.Width.ToString());

                    Lineargauge.Left = this.GetExpressionKey(lineargauge.Left.ToString());

                    Lineargauge.Top = this.GetExpressionKey(lineargauge.Top.ToString());

                    Lineargauge.Orientation = this.GetExpressionKey(lineargauge.Orientation.ToString());

                    Lineargauge.GaugeWidth = this.GetExpressionKey(lineargauge.Width.ToString());

                    Lineargauge.GaugeHeight = this.GetExpressionKey(lineargauge.Height.ToString());

                    if (lineargauge.ActionInfo != null)
                    {
                        foreach (RDL.DOM.Action Action in lineargauge.ActionInfo.Actions)
                        {
                            Lineargauge.ActionInfo = new ActionInfoExp();
                            Lineargauge.ActionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);

                            Lineargauge.ActionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);
                            if (Action.Drillthrough != null)
                            {
                                Lineargauge.ActionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);


                                Lineargauge.ActionInfo.Parameters = new List<ParameterExp>();
                                foreach (Parameter parameters in Action.Drillthrough.Parameters)
                                {
                                    ParameterExp Parameter = new ParameterExp();
                                    Parameter.Name = this.GetExpressionKey(parameters.Name);

                                    Parameter.Omit = this.GetExpressionKey(parameters.Omit);

                                    Parameter.Value = this.GetExpressionKey(parameters.Value);

                                    Lineargauge.ActionInfo.Parameters.Add(Parameter);
                                }
                            }
                        }
                    }

                    Lineargauge.GaugeScales = new List<GaugeScalePropertiesExp>();
                    foreach (LinearScale gaugescale in lineargauge.GaugeScales)
                    {
                        RDL.DOM.Style gaugescalestyle = gaugescale.Style;
                        var Scale = new GaugeScalePropertiesExp();
                        Scale.ScaleStyle = new GaugeStyleExp();
                        Scale.ScaleLabel = new GaugeLabelExp();
                        Scale.ScaleLabel.LabelStyle = new GaugeStyleExp();

                        Scale.ScaleStyle.BorderColor = this.GetExpressionKey(gaugescalestyle.Border.Color);

                        Scale.ScaleStyle.BorderStyle = this.GetExpressionKey(gaugescalestyle.Border.Style);

                        if (gaugescalestyle.Border.Width != null)
                        {
                            Scale.ScaleStyle.BorderWidth = this.GetExpressionKey(gaugescalestyle.Border.Width.size);

                        }
                        Scale.ScaleStyle.BackgroundColor = this.GetExpressionKey(gaugescalestyle.BackgroundColor);

                        Scale.ScaleStyle.BackgroundGradientEndcolor = this.GetExpressionKey(gaugescalestyle.BackgroundGradientEndColor);

                        Scale.ScaleStyle.BackgroundGradientType = this.GetExpressionKey(gaugescalestyle.BackgroundGradientType.ToString());

                        Scale.ScaleStyle.BackgroundHatchType = this.GetExpressionKey(gaugescalestyle.BackgroundHatchType.ToString());

                        Scale.Hidden = gaugescale.Hidden;

                        Scale.ScaleStyle.OffSet = this.GetExpressionKey(gaugescalestyle.ShadowOffset);

                        Scale.LabelMultiplier = this.GetExpressionKey(gaugescale.Multiplier.ToString());

                        Scale.LogBase = this.GetExpressionKey(gaugescale.LogarithmicBase.ToString());

                        Scale.LogrithmicScale = gaugescale.Logarithmic;

                        Scale.ReverseDirection = gaugescale.Reversed;

                        Scale.StartMargin = this.GetExpressionKey(gaugescale.StartMargin.ToString());

                        Scale.EndMargin = this.GetExpressionKey(gaugescale.EndMargin.ToString());

                        Scale.Position = this.GetExpressionKey(gaugescale.Position.ToString());


                        Scale.ScaleLabel.ScaleDistance = this.GetExpressionKey(gaugescale.ScaleLabels.DistanceFromScale.ToString());

                        Scale.ScaleLabel.LabelStyle.FontFamily = this.GetExpressionKey(gaugescale.ScaleLabels.Style.FontFamily);

                        Scale.ScaleLabel.LabelStyle.FontSize = this.GetExpressionKey(gaugescale.ScaleLabels.Style.FontSize.size);

                        Scale.ScaleLabel.LabelStyle.FontStyle = this.GetExpressionKey(gaugescale.ScaleLabels.Style.FontStyle);

                        Scale.ScaleLabel.LabelStyle.FontWeight = this.GetExpressionKey(gaugescale.ScaleLabels.Style.FontWeight);

                        Scale.ScaleLabel.FontAngle = this.GetExpressionKey(gaugescale.ScaleLabels.FontAngle.ToString());

                        Scale.ScaleLabel.FormatString = this.GetExpressionKey(gaugescale.ScaleLabels.Style.Format);//formatstring

                        Scale.ScaleLabel.Hidden = gaugescale.ScaleLabels.Hidden;

                        Scale.ScaleLabel.LabelInterval = this.GetExpressionKey(gaugescale.ScaleLabels.Interval.ToString());

                        Scale.ScaleLabel.LabelIntervalOffset = this.GetExpressionKey(gaugescale.ScaleLabels.IntervalOffset.ToString());

                        Scale.ScaleLabel.ScalePlacment = this.GetExpressionKey(gaugescale.ScaleLabels.Placement.ToString());

                        Scale.ScaleLabel.RotateLabel = gaugescale.ScaleLabels.RotateLabels;

                        Scale.ScaleLabel.EndLabel = gaugescale.ScaleLabels.ShowEndLabels;

                        Scale.ScaleLabel.TextColor = this.GetExpressionKey(gaugescale.ScaleLabels.Style.Color);//textcolor

                        Scale.ScaleLabel.TextDecoration = this.GetExpressionKey(gaugescale.ScaleLabels.Style.TextDecoration);

                        Scale.ScaleLabel.UseFontPercent = gaugescale.ScaleLabels.UseFontPercent;


                        Scale.MajorTickMark = new TickMarksExp();
                        Scale.MajorTickMark.TickMarkStyle = new GaugeStyleExp();
                        Scale.MajorTickMark.TickMarkImage = new BaseImageExp();

                        Scale.MajorTickMark.TickMarkStyle.BorderColor = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Style.Border.Color);

                        Scale.MajorTickMark.TickMarkStyle.BorderStyle = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Style.Border.Style);

                        if (gaugescale.GaugeMajorTickMarks.Style.Border.Width != null)
                        {
                            Scale.MajorTickMark.TickMarkStyle.BorderWidth = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Style.Border.Width.size);

                        }
                        Scale.MajorTickMark.DistanceFromScale = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.DistanceFromScale.ToString());

                        Scale.MajorTickMark.EnableGradient = gaugescale.GaugeMajorTickMarks.EnableGradient;

                        Scale.MajorTickMark.FillColor = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Style.Color);//fillcolor

                        Scale.MajorTickMark.GradientDensity = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.GradientDensity.ToString());

                        Scale.MajorTickMark.HideTickMark = gaugescale.GaugeMajorTickMarks.Hidden;

                        Scale.MajorTickMark.Interval = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Interval.ToString());

                        Scale.MajorTickMark.IntervalOffset = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.IntervalOffset.ToString());

                        Scale.MajorTickMark.Length = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Length.ToString());

                        Scale.MajorTickMark.TickMarkPlacement = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Placement.ToString());

                        Scale.MajorTickMark.TickMarkShape = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Shape.ToString());

                        Scale.MajorTickMark.TickMarkImage.MIMEType = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.TickMarkImage.MIMEType);

                        Scale.MajorTickMark.TickMarkImage.Source = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.TickMarkImage.Source.ToString());

                        Scale.MajorTickMark.TickMarkImage.TransparentColor = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.TickMarkImage.TransparentColor);

                        Scale.MajorTickMark.TickMarkImage.Value = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.TickMarkImage.Value.ToString());

                        Scale.MajorTickMark.TickMarkImage.HueColor = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.TickMarkImage.HueColor);

                        Scale.MajorTickMark.Width = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Width.ToString());


                        Scale.MinorTickMark = new TickMarksExp();
                        Scale.MinorTickMark.TickMarkStyle = new GaugeStyleExp();
                        Scale.MinorTickMark.TickMarkImage = new BaseImageExp();

                        Scale.MinorTickMark.TickMarkStyle.BorderColor = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Style.Border.Color);

                        Scale.MinorTickMark.TickMarkStyle.BorderStyle = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Style.Border.Style);

                        if (gaugescale.GaugeMinorTickMarks.Style.Border.Width != null)
                        {
                            Scale.MinorTickMark.TickMarkStyle.BorderWidth = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Style.Border.Width.size);

                        }
                        Scale.MinorTickMark.DistanceFromScale = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.DistanceFromScale.ToString());

                        Scale.MinorTickMark.EnableGradient = gaugescale.GaugeMinorTickMarks.EnableGradient;

                        Scale.MinorTickMark.FillColor = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Style.Color);//fillcolor

                        Scale.MinorTickMark.GradientDensity = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.GradientDensity.ToString());

                        Scale.MinorTickMark.HideTickMark = gaugescale.GaugeMinorTickMarks.Hidden;

                        Scale.MinorTickMark.Interval = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Interval.ToString());

                        Scale.MinorTickMark.IntervalOffset = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.IntervalOffset.ToString());

                        Scale.MinorTickMark.Length = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Length.ToString());

                        Scale.MinorTickMark.TickMarkPlacement = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Placement.ToString());

                        Scale.MinorTickMark.TickMarkShape = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Shape.ToString());

                        Scale.MinorTickMark.TickMarkImage.MIMEType = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.TickMarkImage.MIMEType);

                        Scale.MinorTickMark.TickMarkImage.Source = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.TickMarkImage.Source.ToString());

                        Scale.MinorTickMark.TickMarkImage.TransparentColor = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.TickMarkImage.TransparentColor);

                        Scale.MinorTickMark.TickMarkImage.Value = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.TickMarkImage.Value.ToString());

                        Scale.MinorTickMark.TickMarkImage.HueColor = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.TickMarkImage.HueColor);

                        Scale.MinorTickMark.Width = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Width.ToString());


                        Scale.ScaleInterval = this.GetExpressionKey(gaugescale.Interval.ToString());

                        Scale.ScaleIntervaloffset = this.GetExpressionKey(gaugescale.IntervalOffset.ToString());


                        Scale.MaximumPin = new ScalePinExp();
                        Scale.MaximumPin.PinImage = new BaseImageExp();
                        Scale.MaximumPin.PinLabel = new GaugeLabelExp();
                        Scale.MaximumPin.PinLabel.LabelStyle = new GaugeStyleExp();

                        Scale.MaximumPin.Location = this.GetExpressionKey(gaugescale.MaximumPin.Location.ToString());

                        Scale.MaximumPin.Enable = gaugescale.MaximumPin.Enable;

                        Scale.MaximumPin.PinLabel.AllowUpsideDown = gaugescale.MaximumPin.PinLabel.AllowUpsideDown;

                        Scale.MaximumPin.PinLabel.ScaleDistance = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.DistanceFromScale.ToString());

                        Scale.MaximumPin.PinLabel.LabelStyle.FontSize = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.FontSize.size);

                        Scale.MaximumPin.PinLabel.LabelStyle.FontStyle = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.FontStyle);

                        Scale.MaximumPin.PinLabel.LabelStyle.FontWeight = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.FontWeight);

                        Scale.MaximumPin.PinLabel.LabelStyle.FontFamily = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.FontFamily);

                        Scale.MaximumPin.PinLabel.FontAngle = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.FontAngle.ToString());

                        Scale.MaximumPin.PinLabel.ScalePlacment = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Placement.ToString());

                        Scale.MaximumPin.PinLabel.UseFontPercent = gaugescale.MaximumPin.PinLabel.UseFontPercent;

                        Scale.MaximumPin.PinLabel.Text = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Text);

                        Scale.MaximumPin.PinLabel.RotateLabel = gaugescale.MaximumPin.PinLabel.RotateLabel;

                        Scale.MaximumPin.PinLabel.TextDecoration = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.TextDecoration);

                        Scale.MaximumPin.PinLabel.TextColor = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.Color);


                        Scale.MinimumPin = new ScalePinExp();
                        Scale.MinimumPin.PinImage = new BaseImageExp();
                        Scale.MinimumPin.PinLabel = new GaugeLabelExp();
                        Scale.MinimumPin.PinLabel.LabelStyle = new GaugeStyleExp();

                        Scale.MinimumPin.Location = this.GetExpressionKey(gaugescale.MinimumPin.Location.ToString());

                        Scale.MinimumPin.Enable = gaugescale.MinimumPin.Enable;

                        Scale.MinimumPin.PinLabel.AllowUpsideDown = gaugescale.MinimumPin.PinLabel.AllowUpsideDown;

                        Scale.MinimumPin.PinLabel.ScaleDistance = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.DistanceFromScale.ToString());

                        Scale.MinimumPin.PinLabel.LabelStyle.FontSize = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.FontSize.size);

                        Scale.MinimumPin.PinLabel.LabelStyle.FontStyle = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.FontStyle);

                        Scale.MinimumPin.PinLabel.LabelStyle.FontWeight = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.FontWeight);

                        Scale.MinimumPin.PinLabel.LabelStyle.FontFamily = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.FontFamily);

                        Scale.MinimumPin.PinLabel.FontAngle = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.FontAngle.ToString());

                        Scale.MinimumPin.PinLabel.ScalePlacment = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Placement.ToString());

                        Scale.MinimumPin.PinLabel.UseFontPercent = gaugescale.MinimumPin.PinLabel.UseFontPercent;

                        Scale.MinimumPin.PinLabel.Text = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Text);

                        Scale.MinimumPin.PinLabel.RotateLabel = gaugescale.MinimumPin.PinLabel.RotateLabel;

                        Scale.MinimumPin.PinLabel.TextDecoration = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.TextDecoration);

                        Scale.MinimumPin.PinLabel.TextColor = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.Color);


                        Scale.MaximumValue = new GaugeInputValueExp();

                        Scale.MaximumValue.AddConstant = this.GetExpressionKey(gaugescale.MaximumValue.AddConstant.ToString());

                        Scale.MaximumValue.DataElementName = this.GetExpressionKey(gaugescale.MaximumValue.DataElementName);

                        Scale.MaximumValue.DataElementOutput = this.GetExpressionKey(gaugescale.MaximumValue.DataElementOutput.ToString());

                        Scale.MaximumValue.Formula = this.GetExpressionKey(gaugescale.MaximumValue.Formula.ToString());

                        Scale.MaximumValue.MaxPercent = this.GetExpressionKey(gaugescale.MaximumValue.MaxPercent.ToString());

                        Scale.MaximumValue.MinPercent = this.GetExpressionKey(gaugescale.MaximumValue.MinPercent.ToString());

                        Scale.MaximumValue.Multiplier = this.GetExpressionKey(gaugescale.MaximumValue.Multiplier.ToString());

                        Scale.MaximumValue.Value = this.GetExpressionKey(gaugescale.MaximumValue.Value);


                        Scale.MinimumValue = new GaugeInputValueExp();

                        Scale.MinimumValue.AddConstant = this.GetExpressionKey(gaugescale.MinimumValue.AddConstant.ToString());

                        Scale.MinimumValue.DataElementName = this.GetExpressionKey(gaugescale.MinimumValue.DataElementName);

                        Scale.MinimumValue.DataElementOutput = this.GetExpressionKey(gaugescale.MinimumValue.DataElementOutput.ToString());

                        Scale.MinimumValue.Formula = this.GetExpressionKey(gaugescale.MinimumValue.Formula.ToString());

                        Scale.MinimumValue.MaxPercent = this.GetExpressionKey(gaugescale.MinimumValue.MaxPercent.ToString());

                        Scale.MinimumValue.MinPercent = this.GetExpressionKey(gaugescale.MinimumValue.MinPercent.ToString());

                        Scale.MinimumValue.Multiplier = this.GetExpressionKey(gaugescale.MinimumValue.Multiplier.ToString());

                        Scale.MinimumValue.Value = this.GetExpressionKey(gaugescale.MinimumValue.Value);


                        Scale.TickMarksonTop = gaugescale.TickMarksOnTop;

                        Scale.ToolTip = this.GetExpressionKey(gaugescale.ToolTip);

                        Scale.ScaleWidth = this.GetExpressionKey(gaugescale.Width.ToString());


                        if (gaugescale.ActionInfo != null)
                        {
                            foreach (RDL.DOM.Action Action in gaugescale.ActionInfo.Actions)
                            {
                                Scale.ActionInfo = new ActionInfoExp();
                                Scale.ActionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);

                                Scale.ActionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);

                                if (Action.Drillthrough != null)
                                {
                                    Scale.ActionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);


                                    Scale.ActionInfo.Parameters = new List<ParameterExp>();
                                    foreach (Parameter parameters in Action.Drillthrough.Parameters)
                                    {
                                        ParameterExp Parameter = new ParameterExp();
                                        Parameter.Name = this.GetExpressionKey(parameters.Name);

                                        Parameter.Omit = this.GetExpressionKey(parameters.Omit);

                                        Parameter.Value = this.GetExpressionKey(parameters.Value);

                                        Scale.ActionInfo.Parameters.Add(Parameter);
                                    }
                                }
                            }
                        }

                        Scale.ScalePointer = new List<PointerExp>();
                        foreach (LinearPointer gaugepointer in gaugescale.GaugePointers)
                        {
                            RDL.DOM.Style gaugepointerstyle = gaugepointer.Style;
                            var Pointer = new PointerExp();
                            Pointer.PointerImage = new PointerImageExp();
                            Pointer.PointerStyle = new GaugeStyleExp();
                            Pointer.PointerImage.PointerImage = new BaseImageExp();
                            Pointer.LinearPointer = new LinearPointerExp();
                            Pointer.LinearPointer.ThermometerProperty = new GaugeStyleExp();
                            Pointer.Value = new GaugeInputValueExp();

                            Pointer.PointerStyle.BackgroundColor = this.GetExpressionKey(gaugepointerstyle.BackgroundColor);

                            Pointer.PointerStyle.BackgroundGradientEndcolor = this.GetExpressionKey(gaugepointerstyle.BackgroundGradientEndColor);

                            Pointer.PointerStyle.BackgroundGradientType = this.GetExpressionKey(gaugepointerstyle.BackgroundGradientType.ToString());

                            Pointer.PointerStyle.BackgroundHatchType = this.GetExpressionKey(gaugepointerstyle.BackgroundHatchType.ToString());

                            Pointer.PointerStyle.BorderColor = this.GetExpressionKey(gaugepointerstyle.Border.Color);

                            Pointer.PointerStyle.BorderStyle = this.GetExpressionKey(gaugepointerstyle.Border.Style);

                            if (gaugepointerstyle.Border.Width != null)
                            {
                                Pointer.PointerStyle.BorderWidth = this.GetExpressionKey(gaugepointerstyle.Border.Width.size);

                            }
                            Pointer.PointerStyle.OffSet = this.GetExpressionKey(gaugepointer.Style.ShadowOffset);


                            Pointer.Hidden = gaugepointer.Hidden;

                            Pointer.PointerImage = new PointerImageExp();
                            Pointer.PointerImage.PivotX = this.GetExpressionKey(gaugepointer.PointerImage.OffsetX.size);

                            Pointer.PointerImage.PivotY = this.GetExpressionKey(gaugepointer.PointerImage.OffsetY.size);


                            Pointer.PointerImage.PointerImage = new BaseImageExp();
                            Pointer.PointerImage.PointerImage.HueColor = this.GetExpressionKey(gaugepointer.PointerImage.HueColor);

                            Pointer.PointerImage.PointerImage.MIMEType = this.GetExpressionKey(gaugepointer.PointerImage.MIMEType);

                            Pointer.PointerImage.PointerImage.Source = this.GetExpressionKey(gaugepointer.PointerImage.Source.ToString());

                            Pointer.PointerImage.PointerImage.Transparency = this.GetExpressionKey(gaugepointer.PointerImage.Transparency.ToString());

                            Pointer.PointerImage.PointerImage.TransparentColor = this.GetExpressionKey(gaugepointer.PointerImage.TransparentColor);

                            Pointer.PointerImage.PointerImage.Value = this.GetExpressionKey(gaugepointer.PointerImage.Value.ToString());


                            Pointer.SnappingInterval = this.GetExpressionKey(gaugepointer.SnappingInterval.ToString());

                            Pointer.SnappingEnabled = gaugepointer.SnappingEnabled;

                            Pointer.PointerTooltip = this.GetExpressionKey(gaugepointer.ToolTip);

                            Pointer.ScaleDistance = this.GetExpressionKey(gaugepointer.DistanceFromScale.ToString());

                            Pointer.Placement = this.GetExpressionKey(gaugepointer.Placement.ToString());

                            Pointer.PointerWidth = this.GetExpressionKey(gaugepointer.Width.ToString());

                            Pointer.LinearPointer.PointerType = this.GetExpressionKey(gaugepointer.Type.ToString());

                            Pointer.BarStart = this.GetExpressionKey(gaugepointer.BarStart.ToString());

                            Pointer.MarkerLength = this.GetExpressionKey(gaugepointer.MarkerLength.ToString());

                            Pointer.MarkerStyle = this.GetExpressionKey(gaugepointer.MarkerStyle.ToString());

                            Pointer.LinearPointer.ThermometerProperty.BackgroundColor = this.GetExpressionKey(gaugepointer.Thermometer.Style.BackgroundColor);

                            Pointer.LinearPointer.ThermometerProperty.BackgroundGradientEndcolor = this.GetExpressionKey(gaugepointer.Thermometer.Style.BackgroundGradientEndColor);

                            Pointer.LinearPointer.ThermometerProperty.BackgroundGradientType = this.GetExpressionKey(gaugepointer.Thermometer.Style.BackgroundGradientType.ToString());

                            Pointer.LinearPointer.ThermometerProperty.BackgroundHatchType = this.GetExpressionKey(gaugepointer.Thermometer.Style.BackgroundHatchType.ToString());

                            Pointer.LinearPointer.ThermometerStyle = this.GetExpressionKey(gaugepointer.Thermometer.ThermometerStyle.ToString());

                            Pointer.LinearPointer.BulbOffset = this.GetExpressionKey(gaugepointer.Thermometer.BulbOffset.ToString());

                            Pointer.LinearPointer.BulbSize = this.GetExpressionKey(gaugepointer.Thermometer.BulbSize.ToString());


                            Pointer.Value.AddConstant = this.GetExpressionKey(gaugepointer.GaugeInputValue.AddConstant.ToString());

                            Pointer.Value.DataElementName = this.GetExpressionKey(gaugepointer.GaugeInputValue.DataElementName);

                            Pointer.Value.DataElementOutput = this.GetExpressionKey(gaugepointer.GaugeInputValue.DataElementOutput.ToString());

                            Pointer.Value.Formula = this.GetExpressionKey(gaugepointer.GaugeInputValue.Formula.ToString());

                            Pointer.Value.MaxPercent = this.GetExpressionKey(gaugepointer.GaugeInputValue.MaxPercent.ToString());

                            Pointer.Value.MinPercent = this.GetExpressionKey(gaugepointer.GaugeInputValue.MinPercent.ToString());

                            Pointer.Value.Multiplier = this.GetExpressionKey(gaugepointer.GaugeInputValue.Multiplier.ToString());

                            if (gaugepointer.GaugeInputValue.Value != null)
                            {
                                Pointer.Value.Value = this.GetExpressionKey(gaugepointer.GaugeInputValue.Value);
                            }

                            if (gaugepointer.ActionInfo != null)
                            {
                                foreach (RDL.DOM.Action Action in gaugepointer.ActionInfo.Actions)
                                {
                                    Pointer.ActionInfo = new ActionInfoExp();
                                    Pointer.ActionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);

                                    Pointer.ActionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);

                                    Pointer.ActionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);


                                    Pointer.ActionInfo.Parameters = new List<ParameterExp>();
                                    foreach (Parameter parameters in Action.Drillthrough.Parameters)
                                    {
                                        var Parameter = new ParameterExp();
                                        Parameter.Name = this.GetExpressionKey(parameters.Name);

                                        Parameter.Omit = this.GetExpressionKey(parameters.Omit);

                                        Parameter.Value = this.GetExpressionKey(parameters.Value);

                                        Pointer.ActionInfo.Parameters.Add(Parameter);
                                    }
                                }
                            }
                            Scale.ScalePointer.Add(Pointer);
                        }

                        Scale.ScaleRange = new List<ScaleRangeExp>();
                        foreach (ScaleRange scalerange in gaugescale.ScaleRanges)
                        {
                            RDL.DOM.Style scalerangestyle = scalerange.Style;
                            var Range = new ScaleRangeExp();
                            Range.RangeStyle = new GaugeStyleExp();
                            Range.StartValue = new GaugeInputValueExp();
                            Range.EndValue = new GaugeInputValueExp();

                            Range.RangeStyle.BackgroundColor = this.GetExpressionKey(scalerangestyle.BackgroundColor);

                            Range.RangeStyle.BackgroundGradientEndcolor = this.GetExpressionKey(scalerangestyle.BackgroundGradientEndColor);

                            Range.RangeStyle.BackgroundGradientType = this.GetExpressionKey(scalerangestyle.BackgroundGradientType.ToString());

                            Range.RangeStyle.BackgroundHatchType = this.GetExpressionKey(scalerangestyle.BackgroundHatchType.ToString());

                            Range.RangeStyle.BorderColor = this.GetExpressionKey(scalerangestyle.Border.Color);

                            Range.RangeStyle.BorderStyle = this.GetExpressionKey(scalerangestyle.Border.Style);

                            if (scalerangestyle.Border.Width != null)
                            {
                                Range.RangeStyle.BorderWidth = this.GetExpressionKey(scalerangestyle.Border.Width.size);

                            }
                            Range.Hidden = scalerange.Hidden;

                            Range.RangeStyle.OffSet = this.GetExpressionKey(scalerangestyle.ShadowOffset);

                            Range.InRangeBarColor = this.GetExpressionKey(scalerange.InRangeBarPointerColor);

                            Range.InRangeLabelColor = this.GetExpressionKey(scalerange.InRangeLabelColor);

                            Range.InRangeTickmarkColor = this.GetExpressionKey(scalerange.InRangeTickMarksColor);

                            Range.ToolTip = this.GetExpressionKey(scalerange.ToolTip);

                            Range.EndWidth = this.GetExpressionKey(scalerange.EndWidth.ToString());

                            Range.RangePlacement = this.GetExpressionKey(scalerange.Placement.ToString());

                            Range.ScaleDistance = this.GetExpressionKey(scalerange.DistanceFromScale.ToString());

                            Range.StartWidth = this.GetExpressionKey(scalerange.StartWidth.ToString());


                            Range.EndValue.AddConstant = this.GetExpressionKey(scalerange.EndValue.AddConstant.ToString());

                            Range.EndValue.DataElementName = this.GetExpressionKey(scalerange.EndValue.DataElementName);

                            Range.EndValue.DataElementOutput = this.GetExpressionKey(scalerange.EndValue.DataElementOutput.ToString());

                            Range.EndValue.Formula = this.GetExpressionKey(scalerange.EndValue.Formula.ToString());

                            Range.EndValue.MaxPercent = this.GetExpressionKey(scalerange.EndValue.MaxPercent.ToString());

                            Range.EndValue.MinPercent = this.GetExpressionKey(scalerange.EndValue.MinPercent.ToString());

                            Range.EndValue.Multiplier = this.GetExpressionKey(scalerange.EndValue.Multiplier.ToString());

                            Range.EndValue.Value = this.GetExpressionKey(scalerange.EndValue.Value);


                            Range.StartValue.AddConstant = this.GetExpressionKey(scalerange.StartValue.AddConstant.ToString());

                            Range.StartValue.DataElementName = this.GetExpressionKey(scalerange.StartValue.DataElementName);

                            Range.StartValue.DataElementOutput = this.GetExpressionKey(scalerange.StartValue.DataElementOutput.ToString());

                            Range.StartValue.Formula = this.GetExpressionKey(scalerange.StartValue.Formula.ToString());

                            Range.StartValue.MaxPercent = this.GetExpressionKey(scalerange.StartValue.MaxPercent.ToString());

                            Range.StartValue.MinPercent = this.GetExpressionKey(scalerange.StartValue.MinPercent.ToString());

                            Range.StartValue.Multiplier = this.GetExpressionKey(scalerange.StartValue.Multiplier.ToString());

                            Range.StartValue.Value = this.GetExpressionKey(scalerange.StartValue.Value);


                            if (scalerange.ActionInfo != null)
                            {
                                foreach (RDL.DOM.Action Action in scalerange.ActionInfo.Actions)
                                {
                                    Range.ActionInfo = new ActionInfoExp();
                                    Range.ActionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);

                                    Range.ActionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);

                                    Range.ActionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);


                                    Range.ActionInfo.Parameters = new List<ParameterExp>();
                                    foreach (Parameter parameters in Action.Drillthrough.Parameters)
                                    {
                                        var Parameter = new ParameterExp();
                                        Parameter.Name = this.GetExpressionKey(parameters.Name);

                                        Parameter.Omit = this.GetExpressionKey(parameters.Omit);

                                        Parameter.Value = this.GetExpressionKey(parameters.Value);

                                        Range.ActionInfo.Parameters.Add(Parameter);
                                    }
                                }
                            }
                            Scale.ScaleRange.Add(Range);
                        }
                        Lineargauge.GaugeScales.Add(Scale);
                    }
                    this.GaugePanelExpProp.LinearGauges.Add(Lineargauge);
                }
            }

            if (gaugepanel.RadialGauges != null && gaugepanel.RadialGauges.Count > 0)
            {
                this.GaugePanelExpProp.RadialGauges = new List<GaugePropertiesExp>();
                foreach (RadialGauge radialgauge in gaugepanel.RadialGauges)
                {
                    var Radialgauge = new GaugePropertiesExp();
                    Radialgauge.GaugeFrame = new FramePropertiesExp();
                    Radialgauge.GaugeFrame.FrameImage = new BaseImageExp();
                    Radialgauge.GaugeFrame.BackFrameStyle = new GaugeStyleExp();
                    Radialgauge.GaugeStyle = new GaugeStyleExp();
                    Radialgauge.GaugeTopImage = new BaseImageExp();
                    Radialgauge.GaugeFrame.FrameProperty = new GaugeStyleExp();

                    Radialgauge.GaugeFrame.FrameShape = this.GetExpressionKey(radialgauge.BackFrame.FrameShape.ToString());

                    Radialgauge.GaugeFrame.FrameStyle = this.GetExpressionKey(radialgauge.BackFrame.FrameStyle.ToString());

                    Radialgauge.GaugeStyle.BackgroundColor = this.GetExpressionKey(radialgauge.BackFrame.Style.BackgroundColor);

                    Radialgauge.GaugeStyle.BackgroundGradientEndcolor = this.GetExpressionKey(radialgauge.BackFrame.Style.BackgroundGradientEndColor);

                    Radialgauge.GaugeStyle.BackgroundGradientType = this.GetExpressionKey(radialgauge.BackFrame.Style.BackgroundGradientType.ToString());

                    Radialgauge.GaugeStyle.BackgroundHatchType = this.GetExpressionKey(radialgauge.BackFrame.Style.BackgroundHatchType.ToString());

                    if (radialgauge.BackFrame.Style.Border != null)
                    {
                        if (radialgauge.BackFrame.Style.Border.Color != null)
                        {
                            Radialgauge.GaugeStyle.BorderColor = this.GetExpressionKey(radialgauge.BackFrame.Style.Border.Color);
                        }
                        Radialgauge.GaugeStyle.BorderStyle = this.GetExpressionKey(radialgauge.BackFrame.Style.Border.Style);

                        if (radialgauge.BackFrame.Style.Border.Width != null)
                        {
                            Radialgauge.GaugeStyle.BorderWidth = this.GetExpressionKey(radialgauge.BackFrame.Style.Border.Width.size);

                        }
                    }
                    Radialgauge.GaugeFrame.BackFrameStyle.BackgroundColor = this.GetExpressionKey(radialgauge.BackFrame.FrameBackground.Style.BackgroundColor);

                    Radialgauge.GaugeFrame.BackFrameStyle.BackgroundGradientEndcolor = this.GetExpressionKey(radialgauge.BackFrame.FrameBackground.Style.BackgroundGradientEndColor);

                    Radialgauge.GaugeFrame.BackFrameStyle.BackgroundGradientType = this.GetExpressionKey(radialgauge.BackFrame.FrameBackground.Style.BackgroundGradientType.ToString());

                    Radialgauge.GaugeFrame.BackFrameStyle.BackgroundHatchType = this.GetExpressionKey(radialgauge.BackFrame.FrameBackground.Style.BackgroundHatchType.ToString());

                    if (radialgauge.BackFrame.FrameImage != null)
                    {
                        Radialgauge.GaugeFrame.FrameImage.MIMEType = this.GetExpressionKey(radialgauge.BackFrame.FrameImage.MIMEType);

                        Radialgauge.GaugeFrame.FrameImage.HueColor = this.GetExpressionKey(radialgauge.BackFrame.FrameImage.HueColor);

                        Radialgauge.GaugeFrame.FrameImage.Source = this.GetExpressionKey(radialgauge.BackFrame.FrameImage.Source.ToString());

                        Radialgauge.GaugeFrame.FrameImage.Transparency = this.GetExpressionKey(radialgauge.BackFrame.FrameImage.Transparency.ToString());

                        Radialgauge.GaugeFrame.FrameImage.TransparentColor = this.GetExpressionKey(radialgauge.BackFrame.FrameImage.TransparentColor);

                        Radialgauge.GaugeFrame.FrameImage.Value = this.GetExpressionKey(radialgauge.BackFrame.FrameImage.Value.ToString());

                        Radialgauge.GaugeFrame.FrameImage.ClipImage = radialgauge.BackFrame.FrameImage.ClipImage;
                    }

                    Radialgauge.GaugeFrame.FrameWidth = this.GetExpressionKey(radialgauge.BackFrame.FrameWidth.ToString());

                    Radialgauge.GaugeFrame.FrameGlassEffect = this.GetExpressionKey(radialgauge.BackFrame.GlassEffect.ToString());

                    Radialgauge.GaugeFrame.BackFrameStyle.OffSet = this.GetExpressionKey(radialgauge.BackFrame.Style.ShadowOffset);

                    Radialgauge.GaugeFrame.FrameProperty.BackgroundColor = this.GetExpressionKey(radialgauge.BackFrame.Style.BackgroundColor);


                    Radialgauge.Hidden = radialgauge.Hidden;
                    if (radialgauge.TopImage != null)
                    {
                        Radialgauge.GaugeTopImage.HueColor = this.GetExpressionKey(radialgauge.TopImage.HueColor);

                        Radialgauge.GaugeTopImage.MIMEType = this.GetExpressionKey(radialgauge.TopImage.MIMEType);

                        Radialgauge.GaugeTopImage.Source = this.GetExpressionKey(radialgauge.TopImage.Source.ToString());

                        Radialgauge.GaugeTopImage.TransparentColor = this.GetExpressionKey(radialgauge.TopImage.TransparentColor);

                        Radialgauge.GaugeTopImage.Value = this.GetExpressionKey(radialgauge.TopImage.Value.ToString());
                    }

                    Radialgauge.ClipContent = radialgauge.ClipContent;

                    Radialgauge.GaugeTooltip = this.GetExpressionKey(radialgauge.ToolTip);

                    Radialgauge.AspectRatio = this.GetExpressionKey(radialgauge.AspectRatio.ToString());

                    Radialgauge.Xposition = this.GetExpressionKey(radialgauge.PivotX.ToString());

                    Radialgauge.Yposition = this.GetExpressionKey(radialgauge.PivotY.ToString());

                    Radialgauge.GaugeHeight = this.GetExpressionKey(radialgauge.Height.ToString());

                    Radialgauge.GaugeWidth = this.GetExpressionKey(radialgauge.Width.ToString());

                    Radialgauge.Left = this.GetExpressionKey(radialgauge.Left.ToString());

                    Radialgauge.Top = this.GetExpressionKey(radialgauge.Top.ToString());

                    if (radialgauge.ActionInfo != null)
                    {
                        foreach (RDL.DOM.Action Action in radialgauge.ActionInfo.Actions)
                        {
                            Radialgauge.ActionInfo = new ActionInfoExp();
                            Radialgauge.ActionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);

                            Radialgauge.ActionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);

                            if (Action.Drillthrough != null)
                            {
                                Radialgauge.ActionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);

                                Radialgauge.ActionInfo.Parameters = new List<ParameterExp>();
                                if (Action.Drillthrough.Parameters != null)
                                {
                                    foreach (Parameter parameters in Action.Drillthrough.Parameters)
                                    {
                                        var Parameter = new ParameterExp();
                                        Parameter.Name = this.GetExpressionKey(parameters.Name);

                                        Parameter.Omit = this.GetExpressionKey(parameters.Omit);

                                        Parameter.Value = this.GetExpressionKey(parameters.Value);

                                        Radialgauge.ActionInfo.Parameters.Add(Parameter);
                                    }
                                }
                            }
                        }
                    }

                    Radialgauge.GaugeScales = new List<GaugeScalePropertiesExp>();
                    foreach (RadialScale gaugescale in radialgauge.GaugeScales)
                    {
                        RDL.DOM.Style gaugescalestyle = gaugescale.Style;
                        var Scale = new GaugeScalePropertiesExp();
                        Scale.ScaleStyle = new GaugeStyleExp();
                        if (gaugescalestyle != null)
                        {
                            if (gaugescalestyle.Border != null)
                            {
                                if (gaugescalestyle.Border.Color != null)
                                {
                                    Scale.ScaleStyle.BorderColor = this.GetExpressionKey(gaugescalestyle.Border.Color);
                                }

                                Scale.ScaleStyle.BorderStyle = this.GetExpressionKey(gaugescalestyle.Border.Style);

                                if (gaugescalestyle.Border.Width != null)
                                {
                                    Scale.ScaleStyle.BorderWidth = this.GetExpressionKey(gaugescalestyle.Border.Width.size);

                                }
                            }
                            if (gaugescalestyle.BackgroundColor != null)
                            {
                                Scale.ScaleStyle.BackgroundColor = this.GetExpressionKey(gaugescalestyle.BackgroundColor);

                            }
                            Scale.ScaleStyle.BackgroundGradientEndcolor = this.GetExpressionKey(gaugescalestyle.BackgroundGradientEndColor);

                            Scale.ScaleStyle.BackgroundGradientType = this.GetExpressionKey(gaugescalestyle.BackgroundGradientType.ToString());

                            Scale.ScaleStyle.BackgroundHatchType = this.GetExpressionKey(gaugescalestyle.BackgroundHatchType.ToString());

                            Scale.Hidden = gaugescale.Hidden;

                            Scale.ScaleStyle.OffSet = this.GetExpressionKey(gaugescalestyle.ShadowOffset);
                        }
                        Scale.LabelMultiplier = this.GetExpressionKey(gaugescale.Multiplier.ToString());

                        Scale.LogBase = this.GetExpressionKey(gaugescale.LogarithmicBase.ToString());

                        Scale.LogrithmicScale = gaugescale.Logarithmic;

                        Scale.ReverseDirection = gaugescale.Reversed;


                        Scale.ScaleLabel = new GaugeLabelExp();
                        Scale.ScaleLabel.LabelStyle = new GaugeStyleExp();

                        Scale.ScaleLabel.AllowUpsideDown = gaugescale.ScaleLabels.AllowUpsideDown;

                        Scale.ScaleLabel.ScaleDistance = this.GetExpressionKey(gaugescale.ScaleLabels.DistanceFromScale.ToString());

                        Scale.ScaleLabel.LabelStyle.FontFamily = this.GetExpressionKey(gaugescale.ScaleLabels.Style.FontFamily);

                        Scale.ScaleLabel.LabelStyle.FontSize = this.GetExpressionKey(gaugescale.ScaleLabels.Style.FontSize.size);

                        Scale.ScaleLabel.LabelStyle.FontStyle = this.GetExpressionKey(gaugescale.ScaleLabels.Style.FontStyle);

                        Scale.ScaleLabel.LabelStyle.FontWeight = this.GetExpressionKey(gaugescale.ScaleLabels.Style.FontWeight);

                        Scale.ScaleLabel.FontAngle = this.GetExpressionKey(gaugescale.ScaleLabels.FontAngle.ToString());

                        Scale.ScaleLabel.FormatString = this.GetExpressionKey(gaugescale.ScaleLabels.Style.Format);//formatstring

                        Scale.ScaleLabel.Hidden = gaugescale.ScaleLabels.Hidden;

                        Scale.ScaleLabel.LabelInterval = this.GetExpressionKey(gaugescale.ScaleLabels.Interval.ToString());

                        Scale.ScaleLabel.LabelIntervalOffset = this.GetExpressionKey(gaugescale.ScaleLabels.IntervalOffset.ToString());

                        Scale.ScaleLabel.ScalePlacment = this.GetExpressionKey(gaugescale.ScaleLabels.Placement.ToString());

                        Scale.ScaleLabel.RotateLabel = gaugescale.ScaleLabels.RotateLabels;

                        Scale.ScaleLabel.EndLabel = gaugescale.ScaleLabels.ShowEndLabels;

                        Scale.ScaleLabel.TextColor = this.GetExpressionKey(gaugescale.ScaleLabels.Style.Color);//textcolor

                        Scale.ScaleLabel.TextDecoration = this.GetExpressionKey(gaugescale.ScaleLabels.Style.TextDecoration);

                        Scale.ScaleLabel.UseFontPercent = gaugescale.ScaleLabels.UseFontPercent;



                        Scale.MajorTickMark = new TickMarksExp();
                        Scale.MajorTickMark.TickMarkStyle = new GaugeStyleExp();
                        Scale.MajorTickMark.TickMarkImage = new BaseImageExp();

                        if (gaugescale.GaugeMajorTickMarks.Style != null)
                        {
                            if (gaugescale.GaugeMajorTickMarks.Style.Border != null)
                            {
                                Scale.MajorTickMark.TickMarkStyle.BorderColor = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Style.Border.Color);

                                Scale.MajorTickMark.TickMarkStyle.BorderStyle = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Style.Border.Style);

                                if (gaugescale.GaugeMajorTickMarks.Style.Border.Width != null)
                                {
                                    Scale.MajorTickMark.TickMarkStyle.BorderWidth = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Style.Border.Width.size);

                                }
                            }
                            Scale.MajorTickMark.FillColor = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Style.Color);//fillcolor
                        }
                        Scale.MajorTickMark.DistanceFromScale = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.DistanceFromScale.ToString());

                        Scale.MajorTickMark.EnableGradient = gaugescale.GaugeMajorTickMarks.EnableGradient;


                        Scale.MajorTickMark.GradientDensity = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.GradientDensity.ToString());

                        Scale.MajorTickMark.HideTickMark = gaugescale.GaugeMajorTickMarks.Hidden;

                        Scale.MajorTickMark.Interval = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Interval.ToString());

                        Scale.MajorTickMark.IntervalOffset = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.IntervalOffset.ToString());

                        Scale.MajorTickMark.Length = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Length.ToString());

                        Scale.MajorTickMark.TickMarkPlacement = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Placement.ToString());

                        Scale.MajorTickMark.TickMarkShape = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Shape.ToString());
                        if (gaugescale.GaugeMajorTickMarks.TickMarkImage != null)
                        {
                            Scale.MajorTickMark.TickMarkImage.MIMEType = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.TickMarkImage.MIMEType);

                            Scale.MajorTickMark.TickMarkImage.Source = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.TickMarkImage.Source.ToString());

                            Scale.MajorTickMark.TickMarkImage.TransparentColor = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.TickMarkImage.TransparentColor);

                            Scale.MajorTickMark.TickMarkImage.Value = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.TickMarkImage.Value.ToString());

                            Scale.MajorTickMark.TickMarkImage.HueColor = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.TickMarkImage.HueColor);
                        }

                        Scale.MajorTickMark.Width = this.GetExpressionKey(gaugescale.GaugeMajorTickMarks.Width.ToString());



                        Scale.MinorTickMark = new TickMarksExp();
                        Scale.MinorTickMark.TickMarkStyle = new GaugeStyleExp();
                        Scale.MinorTickMark.TickMarkImage = new BaseImageExp();
                        if (gaugescale.GaugeMinorTickMarks.Style != null)
                        {
                            if (gaugescale.GaugeMinorTickMarks.Style.Border != null)
                            {
                                Scale.MinorTickMark.TickMarkStyle.BorderColor = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Style.Border.Color);

                                Scale.MinorTickMark.TickMarkStyle.BorderStyle = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Style.Border.Style);

                                if (gaugescale.GaugeMinorTickMarks.Style.Border.Width != null)
                                {
                                    Scale.MinorTickMark.TickMarkStyle.BorderWidth = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Style.Border.Width.size);
                                }
                            }
                            Scale.MinorTickMark.DistanceFromScale = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.DistanceFromScale.ToString());

                            Scale.MinorTickMark.EnableGradient = gaugescale.GaugeMinorTickMarks.EnableGradient;

                            Scale.MinorTickMark.FillColor = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Style.Color);//fillcolor
                        }

                        Scale.MinorTickMark.GradientDensity = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.GradientDensity.ToString());

                        Scale.MinorTickMark.HideTickMark = gaugescale.GaugeMinorTickMarks.Hidden;

                        Scale.MinorTickMark.Interval = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Interval.ToString());

                        Scale.MinorTickMark.IntervalOffset = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.IntervalOffset.ToString());

                        Scale.MinorTickMark.Length = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Length.ToString());

                        Scale.MinorTickMark.TickMarkPlacement = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Placement.ToString());

                        Scale.MinorTickMark.TickMarkShape = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Shape.ToString());

                        if (gaugescale.GaugeMinorTickMarks.TickMarkImage != null)
                        {
                            Scale.MinorTickMark.TickMarkImage.MIMEType = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.TickMarkImage.MIMEType);

                            Scale.MinorTickMark.TickMarkImage.Source = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.TickMarkImage.Source.ToString());

                            Scale.MinorTickMark.TickMarkImage.TransparentColor = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.TickMarkImage.TransparentColor);

                            Scale.MinorTickMark.TickMarkImage.Value = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.TickMarkImage.Value.ToString());

                            Scale.MinorTickMark.TickMarkImage.HueColor = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.TickMarkImage.HueColor);

                        }
                        Scale.MinorTickMark.Width = this.GetExpressionKey(gaugescale.GaugeMinorTickMarks.Width.ToString());


                        Scale.ScaleInterval = this.GetExpressionKey(gaugescale.Interval.ToString());

                        Scale.ScaleIntervaloffset = this.GetExpressionKey(gaugescale.IntervalOffset.ToString());

                        Scale.StartAngle = this.GetExpressionKey(gaugescale.StartAngle.ToString());

                        Scale.SweepAngle = this.GetExpressionKey(gaugescale.SweepAngle.ToString());

                        Scale.Radius = this.GetExpressionKey(gaugescale.Radius.ToString());


                        Scale.MaximumPin = new ScalePinExp();
                        Scale.MaximumPin.PinImage = new BaseImageExp();
                        Scale.MaximumPin.PinLabel = new GaugeLabelExp();
                        Scale.MaximumPin.PinLabel.LabelStyle = new GaugeStyleExp();
                        if (gaugescale.MaximumPin != null)
                        {
                            Scale.MaximumPin.Location = this.GetExpressionKey(gaugescale.MaximumPin.Location.ToString());

                            Scale.MaximumPin.Enable = gaugescale.MaximumPin.Enable;

                            Scale.MaximumPin.PinLabel.AllowUpsideDown = gaugescale.MaximumPin.PinLabel.AllowUpsideDown;

                            Scale.MaximumPin.PinLabel.ScaleDistance = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.DistanceFromScale.ToString());
                            if (gaugescale.MaximumPin.PinLabel.Style != null)
                            {
                                Scale.MaximumPin.PinLabel.LabelStyle.FontSize = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.FontSize.size);

                                Scale.MaximumPin.PinLabel.LabelStyle.FontStyle = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.FontStyle);

                                Scale.MaximumPin.PinLabel.LabelStyle.FontWeight = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.FontWeight);

                                Scale.MaximumPin.PinLabel.LabelStyle.FontFamily = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.FontFamily);

                                Scale.MaximumPin.PinLabel.TextDecoration = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.TextDecoration);

                                Scale.MaximumPin.PinLabel.TextColor = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Style.Color);

                            }

                            Scale.MaximumPin.PinLabel.FontAngle = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.FontAngle.ToString());

                            Scale.MaximumPin.PinLabel.ScalePlacment = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Placement.ToString());

                            Scale.MaximumPin.PinLabel.UseFontPercent = gaugescale.MaximumPin.PinLabel.UseFontPercent;

                            Scale.MaximumPin.PinLabel.Text = this.GetExpressionKey(gaugescale.MaximumPin.PinLabel.Text);

                            Scale.MaximumPin.PinLabel.RotateLabel = gaugescale.MaximumPin.PinLabel.RotateLabel;
                        }

                        Scale.MinimumPin = new ScalePinExp();
                        Scale.MinimumPin.PinImage = new BaseImageExp();
                        Scale.MinimumPin.PinLabel = new GaugeLabelExp();
                        Scale.MinimumPin.PinLabel.LabelStyle = new GaugeStyleExp();
                        if (gaugescale.MinimumPin != null)
                        {
                            Scale.MinimumPin.Location = this.GetExpressionKey(gaugescale.MinimumPin.Location.ToString());

                            Scale.MinimumPin.Enable = gaugescale.MinimumPin.Enable;

                            Scale.MinimumPin.PinLabel.AllowUpsideDown = gaugescale.MinimumPin.PinLabel.AllowUpsideDown;

                            Scale.MinimumPin.PinLabel.ScaleDistance = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.DistanceFromScale.ToString());
                            if (gaugescale.MinimumPin.PinLabel.Style != null)
                            {
                                Scale.MinimumPin.PinLabel.LabelStyle.FontSize = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.FontSize.size);

                                Scale.MinimumPin.PinLabel.LabelStyle.FontStyle = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.FontStyle);

                                Scale.MinimumPin.PinLabel.LabelStyle.FontWeight = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.FontWeight);

                                Scale.MinimumPin.PinLabel.LabelStyle.FontFamily = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.FontFamily);

                                Scale.MinimumPin.PinLabel.TextDecoration = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.TextDecoration);

                                Scale.MinimumPin.PinLabel.TextColor = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Style.Color);//textcolor
                            }
                            Scale.MinimumPin.PinLabel.FontAngle = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.FontAngle.ToString());

                            Scale.MinimumPin.PinLabel.ScalePlacment = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Placement.ToString());

                            Scale.MinimumPin.PinLabel.UseFontPercent = gaugescale.MinimumPin.PinLabel.UseFontPercent;

                            Scale.MinimumPin.PinLabel.Text = this.GetExpressionKey(gaugescale.MinimumPin.PinLabel.Text);

                            Scale.MinimumPin.PinLabel.RotateLabel = gaugescale.MinimumPin.PinLabel.RotateLabel;

                        }

                        Scale.MaximumValue = new GaugeInputValueExp();

                        Scale.MaximumValue.AddConstant = this.GetExpressionKey(gaugescale.MaximumValue.AddConstant.ToString());

                        Scale.MaximumValue.DataElementName = this.GetExpressionKey(gaugescale.MaximumValue.DataElementName);

                        Scale.MaximumValue.DataElementOutput = this.GetExpressionKey(gaugescale.MaximumValue.DataElementOutput.ToString());

                        Scale.MaximumValue.Formula = this.GetExpressionKey(gaugescale.MaximumValue.Formula.ToString());

                        Scale.MaximumValue.MaxPercent = this.GetExpressionKey(gaugescale.MaximumValue.MaxPercent.ToString());

                        Scale.MaximumValue.MinPercent = this.GetExpressionKey(gaugescale.MaximumValue.MinPercent.ToString());

                        Scale.MaximumValue.Multiplier = this.GetExpressionKey(gaugescale.MaximumValue.Multiplier.ToString());

                        Scale.MaximumValue.Value = this.GetExpressionKey(gaugescale.MaximumValue.Value);


                        Scale.MinimumValue = new GaugeInputValueExp();
                        Scale.MinimumValue.AddConstant = this.GetExpressionKey(gaugescale.MinimumValue.AddConstant.ToString());

                        Scale.MinimumValue.DataElementName = this.GetExpressionKey(gaugescale.MinimumValue.DataElementName);

                        Scale.MinimumValue.DataElementOutput = this.GetExpressionKey(gaugescale.MinimumValue.DataElementOutput.ToString());

                        Scale.MinimumValue.Formula = this.GetExpressionKey(gaugescale.MinimumValue.Formula.ToString());

                        Scale.MinimumValue.MaxPercent = this.GetExpressionKey(gaugescale.MinimumValue.MaxPercent.ToString());

                        Scale.MinimumValue.MinPercent = this.GetExpressionKey(gaugescale.MinimumValue.MinPercent.ToString());

                        Scale.MinimumValue.Multiplier = this.GetExpressionKey(gaugescale.MinimumValue.Multiplier.ToString());

                        Scale.MinimumValue.Value = this.GetExpressionKey(gaugescale.MinimumValue.Value);

                        Scale.TickMarksonTop = gaugescale.TickMarksOnTop;

                        Scale.ToolTip = this.GetExpressionKey(gaugescale.ToolTip);

                        Scale.ScaleWidth = this.GetExpressionKey(gaugescale.Width.ToString());


                        if (gaugescale.ActionInfo != null)
                        {
                            foreach (RDL.DOM.Action Action in gaugescale.ActionInfo.Actions)
                            {
                                Scale.ActionInfo = new ActionInfoExp();
                                Scale.ActionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);

                                Scale.ActionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);


                                if (Action.Drillthrough != null)
                                {
                                    Scale.ActionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);

                                    Scale.ActionInfo.Parameters = new List<ParameterExp>();
                                    foreach (Parameter parameters in Action.Drillthrough.Parameters)
                                    {
                                        var Parameter = new ParameterExp();
                                        Parameter.Name = this.GetExpressionKey(parameters.Name);

                                        Parameter.Omit = this.GetExpressionKey(parameters.Omit);

                                        Parameter.Value = this.GetExpressionKey(parameters.Value);

                                        Scale.ActionInfo.Parameters.Add(Parameter);
                                    }
                                }
                            }
                        }

                        Scale.ScalePointer = new List<PointerExp>();
                        foreach (RadialPointer gaugepointer in gaugescale.GaugePointers)
                        {
                            RDL.DOM.Style pointerstyle = gaugepointer.Style;
                            var Pointer = new PointerExp();
                            Pointer.PointerImage = new PointerImageExp();
                            Pointer.PointerStyle = new GaugeStyleExp();
                            Pointer.PointerImage.PointerImage = new BaseImageExp();
                            Pointer.RadialPointer = new RadialPointerExp();
                            Pointer.Value = new GaugeInputValueExp();

                            Pointer.PointerStyle.BackgroundColor = this.GetExpressionKey(pointerstyle.BackgroundColor);

                            Pointer.PointerStyle.BackgroundGradientEndcolor = this.GetExpressionKey(pointerstyle.BackgroundGradientEndColor);

                            Pointer.PointerStyle.BackgroundGradientType = this.GetExpressionKey(pointerstyle.BackgroundGradientType.ToString());

                            Pointer.PointerStyle.BackgroundHatchType = this.GetExpressionKey(pointerstyle.BackgroundHatchType.ToString());

                            if (pointerstyle.Border != null)
                            {
                                Pointer.PointerStyle.BorderColor = this.GetExpressionKey(pointerstyle.Border.Color);

                                Pointer.PointerStyle.BorderStyle = this.GetExpressionKey(pointerstyle.Border.Style);

                                if (pointerstyle.Border.Width != null)
                                {
                                    Pointer.PointerStyle.BorderWidth = this.GetExpressionKey(pointerstyle.Border.Width.size);

                                }
                            }
                            Pointer.Hidden = gaugepointer.Hidden;

                            if (gaugepointer.PointerImage != null)
                            {
                                if (gaugepointer.PointerImage.OffsetX != null)
                                {
                                    Pointer.PointerImage.PivotX = this.GetExpressionKey(gaugepointer.PointerImage.OffsetX.size);

                                }
                                if (gaugepointer.PointerImage.OffsetY != null)
                                {
                                    Pointer.PointerImage.PivotY = this.GetExpressionKey(gaugepointer.PointerImage.OffsetY.size);

                                }
                                Pointer.PointerImage.PointerImage.HueColor = this.GetExpressionKey(gaugepointer.PointerImage.HueColor);

                                Pointer.PointerImage.PointerImage.MIMEType = this.GetExpressionKey(gaugepointer.PointerImage.MIMEType);

                                Pointer.PointerImage.PointerImage.Source = this.GetExpressionKey(gaugepointer.PointerImage.Source.ToString());

                                Pointer.PointerImage.PointerImage.Transparency = this.GetExpressionKey(gaugepointer.PointerImage.Transparency.ToString());

                                Pointer.PointerImage.PointerImage.TransparentColor = this.GetExpressionKey(gaugepointer.PointerImage.TransparentColor);

                                Pointer.PointerImage.PointerImage.Value = this.GetExpressionKey(gaugepointer.PointerImage.Value.ToString());
                            }

                            Pointer.PointerStyle.OffSet = this.GetExpressionKey(gaugepointer.Style.ShadowOffset);

                            Pointer.SnappingInterval = this.GetExpressionKey(gaugepointer.SnappingInterval.ToString());

                            Pointer.SnappingEnabled = gaugepointer.SnappingEnabled;

                            Pointer.PointerTooltip = this.GetExpressionKey(gaugepointer.ToolTip);

                            Pointer.ScaleDistance = this.GetExpressionKey(gaugepointer.DistanceFromScale.ToString());

                            Pointer.Placement = this.GetExpressionKey(gaugepointer.Placement.ToString());

                            Pointer.PointerWidth = this.GetExpressionKey(gaugepointer.Width.ToString());

                            Pointer.RadialPointer.PointerType = this.GetExpressionKey(gaugepointer.Type.ToString());

                            Pointer.BarStart = this.GetExpressionKey(gaugepointer.BarStart.ToString());

                            Pointer.MarkerLength = this.GetExpressionKey(gaugepointer.MarkerLength.ToString());

                            Pointer.RadialPointer.MarkerStyle = this.GetExpressionKey(gaugepointer.MarkerStyle.ToString());

                            Pointer.RadialPointer.NeedleStyle = this.GetExpressionKey(gaugepointer.NeedleStyle.ToString());
                            if (gaugepointer.GaugeInputValue != null)
                            {
                                Pointer.Value.AddConstant = this.GetExpressionKey(gaugepointer.GaugeInputValue.AddConstant.ToString());

                                Pointer.Value.DataElementName = this.GetExpressionKey(gaugepointer.GaugeInputValue.DataElementName);

                                Pointer.Value.DataElementOutput = this.GetExpressionKey(gaugepointer.GaugeInputValue.DataElementOutput.ToString());

                                Pointer.Value.Formula = this.GetExpressionKey(gaugepointer.GaugeInputValue.Formula.ToString());

                                Pointer.Value.MaxPercent = this.GetExpressionKey(gaugepointer.GaugeInputValue.MaxPercent.ToString());

                                Pointer.Value.MinPercent = this.GetExpressionKey(gaugepointer.GaugeInputValue.MinPercent.ToString());

                                Pointer.Value.Multiplier = this.GetExpressionKey(gaugepointer.GaugeInputValue.Multiplier.ToString());

                                Pointer.Value.Value = this.GetExpressionKey(gaugepointer.GaugeInputValue.Value);
                            }
                            Pointer.RadialPointer.CapProperties = new PointerCapExp();
                            Pointer.RadialPointer.CapProperties.CapImage = new BaseImageExp();
                            Pointer.RadialPointer.CapProperties.CapStyle = new GaugeStyleExp();

                            Pointer.RadialPointer.CapProperties.PointerCapStyle = this.GetExpressionKey(gaugepointer.PointerCap.CapStyle.ToString());
                            if (gaugepointer.PointerCap.CapImage != null)
                            {
                                Pointer.RadialPointer.CapProperties.CapImage.HueColor = this.GetExpressionKey(gaugepointer.PointerCap.CapImage.HueColor);

                                Pointer.RadialPointer.CapProperties.CapImage.MIMEType = this.GetExpressionKey(gaugepointer.PointerCap.CapImage.MIMEType);

                                Pointer.RadialPointer.CapProperties.CapImage.Source = this.GetExpressionKey(gaugepointer.PointerCap.CapImage.Source.ToString());

                                Pointer.RadialPointer.CapProperties.CapImage.TransparentColor = this.GetExpressionKey(gaugepointer.PointerCap.CapImage.TransparentColor);

                                Pointer.RadialPointer.CapProperties.CapImage.Value = this.GetExpressionKey(gaugepointer.PointerCap.CapImage.Value.ToString());

                                if (gaugepointer.PointerCap.CapImage.OffsetX != null)
                                {
                                    Pointer.RadialPointer.CapProperties.OffsetX = this.GetExpressionKey(gaugepointer.PointerCap.CapImage.OffsetX.size);

                                }
                                if (gaugepointer.PointerCap.CapImage.OffsetY != null)
                                {
                                    Pointer.RadialPointer.CapProperties.OffsetY = this.GetExpressionKey(gaugepointer.PointerCap.CapImage.OffsetY.size);
                                }
                            }
                            Pointer.RadialPointer.CapProperties.CapStyle.BackgroundColor = this.GetExpressionKey(gaugepointer.PointerCap.Style.BackgroundColor);

                            Pointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientEndcolor = this.GetExpressionKey(gaugepointer.PointerCap.Style.BackgroundGradientEndColor);

                            Pointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientType = this.GetExpressionKey(gaugepointer.PointerCap.Style.BackgroundGradientType.ToString());

                            Pointer.RadialPointer.CapProperties.CapStyle.BackgroundHatchType = this.GetExpressionKey(gaugepointer.PointerCap.Style.BackgroundHatchType.ToString());

                            Pointer.RadialPointer.CapProperties.Hidden = gaugepointer.PointerCap.Hidden;

                            Pointer.RadialPointer.CapProperties.OnTop = gaugepointer.PointerCap.OnTop;

                            Pointer.RadialPointer.CapProperties.Reflection = gaugepointer.PointerCap.Reflection;

                            Pointer.RadialPointer.CapProperties.PointerCapWidth = this.GetExpressionKey(gaugepointer.PointerCap.Width.ToString());


                            if (gaugepointer.ActionInfo != null)
                            {
                                foreach (RDL.DOM.Action Action in gaugepointer.ActionInfo.Actions)
                                {
                                    Pointer.ActionInfo = new ActionInfoExp();
                                    Pointer.ActionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);

                                    Pointer.ActionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);
                                    if (Action.Drillthrough != null)
                                    {

                                        Pointer.ActionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);

                                        Pointer.ActionInfo.Parameters = new List<ParameterExp>();
                                        foreach (Parameter parameters in Action.Drillthrough.Parameters)
                                        {
                                            var Parameter = new ParameterExp();
                                            Parameter.Name = this.GetExpressionKey(parameters.Name);

                                            Parameter.Omit = this.GetExpressionKey(parameters.Omit);

                                            Parameter.Value = this.GetExpressionKey(parameters.Value);

                                            Pointer.ActionInfo.Parameters.Add(Parameter);
                                        }
                                    }
                                }
                            }
                            Scale.ScalePointer.Add(Pointer);
                        }

                        if (gaugescale.ScaleRanges != null)
                        {
                            Scale.ScaleRange = new List<ScaleRangeExp>();
                            foreach (ScaleRange scalerange in gaugescale.ScaleRanges)
                            {
                                RDL.DOM.Style rangestyle = scalerange.Style;
                                var Range = new ScaleRangeExp();
                                Range.RangeStyle = new GaugeStyleExp();
                                Range.StartValue = new GaugeInputValueExp();
                                Range.EndValue = new GaugeInputValueExp();

                                Range.RangeStyle.BackgroundColor = this.GetExpressionKey(rangestyle.BackgroundColor);

                                Range.RangeStyle.BackgroundGradientEndcolor = this.GetExpressionKey(rangestyle.BackgroundGradientEndColor);

                                Range.RangeStyle.BackgroundGradientType = this.GetExpressionKey(rangestyle.BackgroundGradientType.ToString());

                                Range.RangeStyle.BackgroundHatchType = this.GetExpressionKey(rangestyle.BackgroundHatchType.ToString());

                                Range.RangeStyle.BorderColor = this.GetExpressionKey(rangestyle.Border.Color);

                                Range.RangeStyle.BorderStyle = this.GetExpressionKey(rangestyle.Border.Style);

                                if (rangestyle.Border.Width != null)
                                {
                                    Range.RangeStyle.BorderWidth = this.GetExpressionKey(rangestyle.Border.Width.size);

                                }
                                Range.Hidden = scalerange.Hidden;

                                Range.RangeStyle.OffSet = this.GetExpressionKey(rangestyle.ShadowOffset);

                                Range.InRangeBarColor = this.GetExpressionKey(scalerange.InRangeBarPointerColor);

                                Range.InRangeLabelColor = this.GetExpressionKey(scalerange.InRangeLabelColor);

                                Range.InRangeTickmarkColor = this.GetExpressionKey(scalerange.InRangeTickMarksColor);

                                Range.ToolTip = this.GetExpressionKey(scalerange.ToolTip);

                                Range.EndWidth = this.GetExpressionKey(scalerange.EndWidth.ToString());

                                Range.RangePlacement = this.GetExpressionKey(scalerange.Placement.ToString());

                                Range.ScaleDistance = this.GetExpressionKey(scalerange.DistanceFromScale.ToString());

                                Range.StartWidth = this.GetExpressionKey(scalerange.StartWidth.ToString());

                                Range.EndValue.AddConstant = this.GetExpressionKey(scalerange.EndValue.AddConstant.ToString());

                                Range.EndValue.DataElementName = this.GetExpressionKey(scalerange.EndValue.DataElementName);

                                Range.EndValue.DataElementOutput = this.GetExpressionKey(scalerange.EndValue.DataElementOutput.ToString());

                                Range.EndValue.Formula = this.GetExpressionKey(scalerange.EndValue.Formula.ToString());

                                Range.EndValue.MaxPercent = this.GetExpressionKey(scalerange.EndValue.MaxPercent.ToString());

                                Range.EndValue.MinPercent = this.GetExpressionKey(scalerange.EndValue.MinPercent.ToString());

                                Range.EndValue.Multiplier = this.GetExpressionKey(scalerange.EndValue.Multiplier.ToString());

                                Range.EndValue.Value = this.GetExpressionKey(scalerange.EndValue.Value);


                                Range.StartValue.AddConstant = this.GetExpressionKey(scalerange.StartValue.AddConstant.ToString());

                                Range.StartValue.DataElementName = this.GetExpressionKey(scalerange.StartValue.DataElementName);

                                Range.StartValue.DataElementOutput = this.GetExpressionKey(scalerange.StartValue.DataElementOutput.ToString());

                                Range.StartValue.Formula = this.GetExpressionKey(scalerange.StartValue.Formula.ToString());

                                Range.StartValue.MaxPercent = this.GetExpressionKey(scalerange.StartValue.MaxPercent.ToString());

                                Range.StartValue.MinPercent = this.GetExpressionKey(scalerange.StartValue.MinPercent.ToString());

                                Range.StartValue.Multiplier = this.GetExpressionKey(scalerange.StartValue.Multiplier.ToString());

                                Range.StartValue.Value = this.GetExpressionKey(scalerange.StartValue.Value);


                                if (scalerange.ActionInfo != null)
                                {
                                    foreach (RDL.DOM.Action Action in scalerange.ActionInfo.Actions)
                                    {
                                        Range.ActionInfo = new ActionInfoExp();
                                        Range.ActionInfo.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);

                                        Range.ActionInfo.Hyperlink = this.GetExpressionKey(Action.Hyperlink);

                                        Range.ActionInfo.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);


                                        Range.ActionInfo.Parameters = new List<ParameterExp>();
                                        foreach (Parameter parameters in Action.Drillthrough.Parameters)
                                        {
                                            var Parameter = new ParameterExp();
                                            Parameter.Name = this.GetExpressionKey(parameters.Name);

                                            Parameter.Omit = this.GetExpressionKey(parameters.Omit);

                                            Parameter.Value = this.GetExpressionKey(parameters.Value);

                                            Range.ActionInfo.Parameters.Add(Parameter);
                                        }
                                    }
                                }
                                Scale.ScaleRange.Add(Range);
                            }
                        }
                        Radialgauge.GaugeScales.Add(Scale);
                    }
                    this.GaugePanelExpProp.RadialGauges.Add(Radialgauge);
                }
            }
        }

        public string GetEvalString(string value)
        {
            var expressFields = this.Model.ExpressionEngine.FieldValues;

            if (this.Engine.Fields != null)
            {
                this.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();

                foreach (DataField field in this.Engine.Fields)
                {
                    this.Model.ExpressionEngine.FieldValues.Add(field.Name, field.Value.Value.GetResult());
                }
            }

            string outputVlaue = this.Model.ExpressionEngine.GetEvalExpressionString(value);

            this.Model.ExpressionEngine.FieldValues = expressFields;

            return outputVlaue;
        }

        private string GetExpressionKey(object value)
        {
            if (value != null)
            {
                string key = this.Model.ExpressionEngine.GetExpressionKey(value.ToString(), this.DataSetName, true);
                this.AddDataFields(key);
                return key;
            }

            return null;
        }

        private void AddDataFields(string key)
        {
            if (!string.IsNullOrEmpty(key) && this.Model.ExpressionEngine.FieldInformations.ContainsKey(key))
            {
                var fields = this.Model.ExpressionEngine.FieldInformations[key];

                foreach (var field in fields.Where(f => (f.DataSetName == null || f.DataSetName == this.DataSetName)))
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

        #endregion

#if !SILVERLIGHT

        internal Stream GetImageStream()
        {
            Stream source = new ImageConversion().CovertToImage(new ReportingGauge(this));
            return source;
        }
#else
        internal Stream GetImageStream()
        {
#if !WINRT
            UIElement gauge = ReportModel.UICollection[this.Name];
            System.Windows.Media.Imaging.WriteableBitmap _bitmap = new System.Windows.Media.Imaging.WriteableBitmap(gauge, gauge.RenderTransform);
            MemoryStream fs = new MemoryStream();
            int width = _bitmap.PixelWidth;
            int height = _bitmap.PixelHeight;

            Syncfusion.Windows.Chart.ChartImage ei = new Syncfusion.Windows.Chart.ChartImage(width, height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int pixel = _bitmap.Pixels[(i * width) + j];
                    ei.SetPixel(j, i,
                                (byte)((pixel >> 16) & 0xFF),
                                (byte)((pixel >> 8) & 0xFF),
                                (byte)(pixel & 0xFF),
                                (byte)((pixel >> 24) & 0xFF)
                    );
                }
            }
            Stream png = ei.GetStream();
            int len = (int)png.Length;
            byte[] bytes = new byte[len];
            png.Read(bytes, 0, len);
            fs.Write(bytes, 0, len);
            fs.Position = 0;
            return fs;
#else
            return null;
#endif
        }

#endif

        #region overridden methods

        public override IReportItemModeler GetModel()
        {
            GaugeModel itemModel = new GaugeModel();
            itemModel.ExpFilters = this.ExpFilters;
            itemModel.CanGrow = this.CanGrow;
            itemModel.KeepTogether = this.KeepTogether;
            itemModel.Height = this.Height;
            itemModel.Name = this.Name;
            itemModel.Top = this.Top;
            itemModel.Left = this.Left;
            itemModel.Width = this.Width;
            itemModel.Height = this.Height;
            itemModel.Width = this.Width;
            itemModel.Model = this.Model;
            itemModel.ModelType = this.ModelType;
            itemModel.FlowLayoutInfo = this.FlowLayoutInfo;
            itemModel.ReportItemModelers = this.ReportItemModelers;
            itemModel.IsSubReportChild = this.IsSubReportChild;
            itemModel.IsTablixChild = this.IsTablixChild;
            itemModel.IsTablixInnerChild = this.IsTablixInnerChild;
            itemModel.ContainerModel = this.ContainerModel;
            itemModel.GaugePanelExpProp = this.GaugePanelExpProp;
            itemModel.GaugePanelProperties = this.GaugePanelProperties;
            itemModel.Engine = this.Engine;
            itemModel.DataSetFields = this.DataSetFields;
            itemModel.DataSetName = this.DataSetName;
            itemModel.DataSource = this.DataSource;
            return itemModel;
        }

        public override void Evaluate()
        {
            try
            {
                this.Engine = new ReportingAggEngine();

                if (this.DataSource == null)
                {
                    var viewAdv = (from view in this.Model.ProcessedData.DataSourceObjects
                                   where view.Key == this.DataSetName
                                   select view.Value).SingleOrDefault();

                    this.Engine.DataSource = this.Model.ProcessedData.FilterItemSoruce(viewAdv, this.ExpFilters);

                }
                else
                {
                    this.Engine.DataSource = this.Model.ProcessedData.FilterItemSoruce(this.DataSource as IEnumerable, this.ExpFilters);
                }

                this.Engine.Fields = this.DataSetFields.ToList();
                this.Engine.Model = this.Model;
                this.Engine.PopulateValue();

                if (!string.IsNullOrEmpty(this.ToggleItem))
                {
                    this.GetTextBoxModel(this.ToggleItem);
                }
                this.GaugePanelProperties = new GaugePanelExpVal();
                this.GaugePanelProperties.AntiAliasing = TryEnum<AntiAliasing>(this.GetEvalString(this.GaugePanelExpProp.AntiAliasing));
                this.GaugePanelProperties.AutoLayout = this.GaugePanelExpProp.AutoLayout;
                this.GaugePanelProperties.ShadowIntensity = double.Parse(this.GetEvalString(this.GaugePanelExpProp.ShadowIntensity));

                this.GaugePanelProperties.GaugeFrame = new FramePropertiesExpVal();
                this.GaugePanelProperties.GaugeFrame.FrameImage = new BaseImageExpVal();
                this.GaugePanelProperties.GaugeFrame.BackFrameStyle = new GaugeStyleExpVal();
                this.GaugePanelProperties.TopImage = new BaseImageExpVal();
               
                if (this.GaugePanelExpProp.Hidden != null)
                {
                    this.GaugePanelProperties.Hidden = bool.Parse(this.GetEvalString(this.GaugePanelExpProp.Hidden));
                    this.Hidden = this.GaugePanelProperties.Hidden;
                }
                else
                {
                    this.Hidden = this.GaugePanelProperties.Hidden;
                }

                if (this.GaugePanelExpProp.GaugeFrame.FrameGlassEffect != null)
                {
                    this.GaugePanelProperties.GaugeFrame.FrameGlassEffect = TryEnum<GlassEffect>(this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.FrameGlassEffect));
                }
                if (this.GaugePanelExpProp.GaugeFrame.FrameImage != null)
                {
                    this.GaugePanelProperties.GaugeFrame.FrameImage.HueColor = this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.FrameImage.HueColor);
                    this.GaugePanelProperties.GaugeFrame.FrameImage.ClipImage = this.GaugePanelExpProp.GaugeFrame.FrameImage.ClipImage;
                    this.GaugePanelProperties.GaugeFrame.FrameImage.Transparency = this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.FrameImage.Transparency);
                    this.GaugePanelProperties.GaugeFrame.FrameImage.MIMEType = this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.FrameImage.MIMEType);
                    this.GaugePanelProperties.GaugeFrame.FrameImage.Source = TryEnum<Source>(this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.FrameImage.Source));
                    this.GaugePanelProperties.GaugeFrame.FrameImage.TransparentColor = this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.FrameImage.TransparentColor);
                    this.GaugePanelProperties.GaugeFrame.FrameImage.Value = this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.FrameImage.Value);
                }
                if (this.GaugePanelExpProp.GaugeFrame.FrameShape != null)
                {
                    this.GaugePanelProperties.GaugeFrame.FrameShape = TryEnum<FrameShape>(this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.FrameShape));
                }
                if (this.GaugePanelExpProp.GaugeFrame.BackFrameStyle != null)
                {
                    if (this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BorderColor != null)
                    {
                        this.GaugePanelProperties.GaugeFrame.BackFrameStyle.BorderColor = this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BorderColor);
                    }
                    this.GaugePanelProperties.GaugeFrame.BackFrameStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BorderStyle));
                    this.GaugePanelProperties.GaugeFrame.BackFrameStyle.BorderWidth = new DOM.Size(this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BorderWidth)).FloatValue;
                    this.GaugePanelProperties.GaugeFrame.BackFrameStyle.BackgroundColor = this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BackgroundColor);
                    this.GaugePanelProperties.GaugeFrame.BackFrameStyle.BackgroundGradientEndcolor = this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BackgroundGradientEndcolor);
                    this.GaugePanelProperties.GaugeFrame.BackFrameStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BackgroundGradientType));
                    this.GaugePanelProperties.GaugeFrame.BackFrameStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.BackFrameStyle.BackgroundHatchType));
                }
                if (this.GaugePanelExpProp.GaugeFrame.FrameStyle != null)
                {
                    this.GaugePanelProperties.GaugeFrame.FrameStyle = TryEnum<FrameStyle>(this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.FrameStyle));
                }
                if (this.GaugePanelExpProp.GaugeFrame.FrameWidth != null)
                {
                    this.GaugePanelProperties.GaugeFrame.FrameWidth = double.Parse(this.GetEvalString(this.GaugePanelExpProp.GaugeFrame.FrameWidth));
                }
                this.GaugePanelProperties.TextAntiAliasingQuality = TryEnum<TextAntiAliasingQuality>(this.GetEvalString(this.GaugePanelExpProp.TextAntiAliasingQuality));
                if (this.GaugePanelExpProp.TopImage != null)
                {
                    this.GaugePanelProperties.TopImage.HueColor = this.GetEvalString(this.GaugePanelExpProp.TopImage.HueColor);
                    this.GaugePanelProperties.TopImage.MIMEType = this.GetEvalString(this.GaugePanelExpProp.TopImage.MIMEType);
                    if (this.GaugePanelExpProp.TopImage.Source != null)
                    {
                        this.GaugePanelProperties.TopImage.Source = TryEnum<Source>(this.GetEvalString(this.GaugePanelExpProp.TopImage.Source));
                    }
                    this.GaugePanelProperties.TopImage.TransparentColor = this.GetEvalString(this.GaugePanelExpProp.TopImage.TransparentColor);
                    this.GaugePanelProperties.TopImage.Value = this.GetEvalString(this.GaugePanelExpProp.TopImage.Value);
                }
                this.GaugePanelProperties.AutoLayout = this.GaugePanelExpProp.AutoLayout;
                if (GaugePanelExpProp.Border != null)
                {
                    this.GaugePanelProperties.Border = new BorderExpval();
                    if (GaugePanelExpProp.Border.Default != null)
                    {
                        this.GaugePanelProperties.Border.Default = new BorderExpvalProperties();
                        this.GaugePanelProperties.Border.Default.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.Default.BorderBrush);
                        this.GaugePanelProperties.Border.Default.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.Default.BorderStyle));
                        if (GaugePanelExpProp.Border.Default.Thickness != null)
                        {
                            this.GaugePanelProperties.Border.Default.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.Default.Thickness)).FloatValue;
                        }
                    }
                    if (GaugePanelExpProp.Border.LeftBorder != null)
                    {
                        this.GaugePanelProperties.Border.LeftBorder = new BorderExpvalProperties();
                        this.GaugePanelProperties.Border.LeftBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.LeftBorder.BorderBrush);
                        this.GaugePanelProperties.Border.Default.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.LeftBorder.BorderStyle));
                        if (GaugePanelExpProp.Border.LeftBorder.Thickness != null)
                        {
                            this.GaugePanelProperties.Border.LeftBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.LeftBorder.Thickness)).FloatValue;
                        }
                    }
                    if (GaugePanelExpProp.Border.TopBorder != null)
                    {
                        this.GaugePanelProperties.Border.TopBorder = new BorderExpvalProperties();
                        this.GaugePanelProperties.Border.TopBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.TopBorder.BorderBrush);
                        this.GaugePanelProperties.Border.TopBorder.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.TopBorder.BorderStyle));
                        if (GaugePanelExpProp.Border.TopBorder.Thickness != null)
                        {
                            this.GaugePanelProperties.Border.TopBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.TopBorder.Thickness)).FloatValue;
                        }
                    }
                    if (GaugePanelExpProp.Border.RightBorder != null)
                    {
                        this.GaugePanelProperties.Border.RightBorder = new BorderExpvalProperties();
                        this.GaugePanelProperties.Border.RightBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.RightBorder.BorderBrush);
                        this.GaugePanelProperties.Border.Default.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.RightBorder.BorderStyle));
                        if (GaugePanelExpProp.Border.RightBorder.Thickness != null)
                        {
                            this.GaugePanelProperties.Border.RightBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.RightBorder.Thickness)).FloatValue;
                        }
                    }
                    if (GaugePanelExpProp.Border.BottomBorder != null)
                    {
                        this.GaugePanelProperties.Border.BottomBorder = new BorderExpvalProperties();
                        this.GaugePanelProperties.Border.BottomBorder.BorderBrush = this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.BottomBorder.BorderBrush);
                        this.GaugePanelProperties.Border.BottomBorder.BorderStyle = TryEnum<BorderStyles>(this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.BottomBorder.BorderStyle));
                        if (GaugePanelExpProp.Border.BottomBorder.Thickness != null)
                        {
                            this.GaugePanelProperties.Border.BottomBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetExpressionKey(GaugePanelExpProp.Border.BottomBorder.Thickness)).FloatValue;
                        }
                    }
                }
                this.GaugePanelProperties.BackgroundColor = this.GetEvalString(this.GaugePanelExpProp.BackgroundColor);
                this.GaugePanelProperties.PageName = this.GetEvalString(this.GaugePanelExpProp.PageName);
                this.GaugePanelProperties.ToolTip = this.GetEvalString(this.GaugePanelExpProp.ToolTip);
                this.GaugePanelProperties.Direction = TryEnum<Direction>(this.GetEvalString(this.GaugePanelExpProp.Direction));
                this.GaugePanelProperties.NumeralLanguage = this.GetEvalString(this.GaugePanelExpProp.NumeralLanguage);
                this.GaugePanelProperties.NumeralVariant = this.GetEvalString(this.GaugePanelExpProp.NumeralVariant);
                this.GaugePanelProperties.Language = this.GetEvalString(this.GaugePanelExpProp.Language);
                this.GaugePanelProperties.Calender = TryEnum<RDL.DOM.Calendar>(this.GetEvalString(this.GaugePanelExpProp.Calender));
                this.GaugePanelProperties.BookMark = this.GetEvalString(this.GaugePanelExpProp.BookMark);
                this.GaugePanelProperties.DocumentMapLabel = this.GetEvalString(this.GaugePanelExpProp.DocumentMapLabel);
                this.DocumentMapLable = this.GaugePanelProperties.DocumentMapLabel;
                if (!string.IsNullOrEmpty(this.DocumentMapLable))
                {
                    this.SetTreeModel();
                }
                this.GaugePanelProperties.ZIndex = int.Parse(this.GetEvalString(this.GaugePanelExpProp.ZIndex));


                this.GaugePanelProperties.GaugeLabels = new List<GaugeLabelExpVal>();
                if (this.GaugePanelExpProp.GaugeLabels != null)
                {
                    foreach (var gaugelabel in this.GaugePanelExpProp.GaugeLabels)
                    {
                        var label = new GaugeLabelExpVal();
                        label.LabelStyle = new GaugeStyleExpVal();
                        label.ActionInfo = new ActionInfoExpVal();

                        if (gaugelabel.LabelStyle != null)
                        {
                            label.LabelStyle.BackgroundColor = this.GetEvalString(gaugelabel.LabelStyle.BackgroundColor);
                            label.LabelStyle.BackgroundGradientEndcolor = this.GetEvalString(gaugelabel.LabelStyle.BackgroundGradientEndcolor);
                            label.LabelStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(gaugelabel.LabelStyle.BackgroundGradientType));
                            label.LabelStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(gaugelabel.LabelStyle.BackgroundHatchType));
                            label.LabelStyle.OffSet = new DOM.Size(this.GetEvalString(gaugelabel.LabelStyle.OffSet)).FloatValue;
                            label.LabelStyle.BorderColor = this.GetEvalString(gaugelabel.LabelStyle.BorderColor);
                            label.LabelStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(gaugelabel.LabelStyle.BorderStyle));
                            label.LabelStyle.BorderWidth = new DOM.Size(this.GetEvalString(gaugelabel.LabelStyle.BorderWidth)).FloatValue;
                            label.LabelStyle.FontFamily = this.GetEvalString(gaugelabel.LabelStyle.FontFamily);
                            label.LabelStyle.FontSize = new DOM.Size(this.GetEvalString(gaugelabel.LabelStyle.FontSize)).FloatValue;
                            label.LabelStyle.FontStyle = TryEnum<RDL.DOM.FontStyle>(this.GetEvalString(gaugelabel.LabelStyle.FontStyle));
                            label.LabelStyle.FontWeight = TryEnum<RDL.DOM.FontWeight>(this.GetEvalString(gaugelabel.LabelStyle.FontWeight));
                        }
                        label.Hidden = gaugelabel.Hidden;
                        label.TextAlign = TryEnum<TextAlign>(this.GetEvalString(gaugelabel.TextAlign));
                        label.TextColor = this.GetEvalString(gaugelabel.TextColor);
                        label.TextDecoration = TryEnum<RDL.DOM.TextDecoration>(this.GetEvalString(gaugelabel.TextDecoration));
                        label.TextShadowOffset = new DOM.Size(this.GetEvalString(gaugelabel.TextShadowOffset)).FloatValue;
                        label.VerticalAlign = TryEnum<VerticalAlign>(this.GetEvalString(gaugelabel.VerticalAlign));
                        label.UseFontPercent = gaugelabel.UseFontPercent;
                        label.ResizeMode = this.GetEvalString(gaugelabel.ResizeMode);
                        label.ToolTip = this.GetEvalString(gaugelabel.ToolTip);
                        label.Text = this.GetEvalString(gaugelabel.Text);
                        label.ZIndex = int.Parse(this.GetEvalString(gaugelabel.ZIndex));
                        label.LabelStyle.BackgroundColor = this.GetEvalString(gaugelabel.LabelStyle.BackgroundColor);
                        label.Top = double.Parse(this.GetEvalString(gaugelabel.Top));
                        label.Left = double.Parse(this.GetEvalString(gaugelabel.Left));
                        label.Height = double.Parse(this.GetEvalString(gaugelabel.Height));
                        label.Width = double.Parse(this.GetEvalString(gaugelabel.Width));
                        label.Angle = double.Parse(this.GetEvalString(gaugelabel.Angle));

                        if (gaugelabel.ActionInfo != null)
                        {
                            label.ActionInfo.BookmarkLink = this.GetEvalString(gaugelabel.ActionInfo.BookmarkLink);
                            label.ActionInfo.Hyperlink = this.GetEvalString(gaugelabel.ActionInfo.Hyperlink);
                            if (gaugelabel.ActionInfo.ReportName != null)
                            {
                                label.ActionInfo.ReportName = this.GetEvalString(gaugelabel.ActionInfo.ReportName);
                            }

                            label.ActionInfo.Parameters = new List<ParameterExpVal>();
                            if (gaugelabel.ActionInfo.Parameters != null)
                            {
                                foreach (var Action in gaugelabel.ActionInfo.Parameters)
                                {
                                    var parameter = new ParameterExpVal();
                                    parameter.Name = this.GetEvalString(Action.Name);
                                    parameter.Omit = this.GetEvalString(Action.Omit);
                                    parameter.Value = this.GetEvalString(Action.Value);
                                    label.ActionInfo.Parameters.Add(parameter);
                                }
                            }
                        }
                        this.GaugePanelProperties.GaugeLabels.Add(label);
                    }
                }

                if (this.GaugePanelExpProp.Indicators != null && this.GaugePanelExpProp.Indicators.Count > 0)
                {
                    this.GaugePanelProperties.Indicators = new List<IndicatorsExpVal>();

                    foreach (var stateIndicator in this.GaugePanelExpProp.Indicators)
                    {
                        IndicatorsExpVal indicators = new IndicatorsExpVal();
                        indicators.Hidden = bool.Parse(this.GetEvalString(stateIndicator.Hidden));
                        indicators.Angle = double.Parse(this.GetEvalString(stateIndicator.Angle));
                        indicators.ToolTip = this.GetEvalString(stateIndicator.ToolTip);
                        indicators.Top = double.Parse(this.GetEvalString(stateIndicator.Top));
                        indicators.Left = double.Parse(this.GetEvalString(stateIndicator.Left));
                        indicators.Height = double.Parse(this.GetEvalString(stateIndicator.Height));
                        indicators.Width = double.Parse(this.GetEvalString(stateIndicator.Width));
                        indicators.ZIndex = int.Parse(this.GetEvalString(stateIndicator.ZIndex));
                        indicators.GaugeIndicatorStyle = TryEnum<GaugeStateIndicatorStyles>(this.GetEvalString(stateIndicator.GaugeIndicatorStyle));
                        indicators.ScaleFactor = double.Parse(this.GetEvalString(stateIndicator.ScaleFactor));
                        indicators.IconSet = TryEnum<StateIndicatorIconsSet>(this.GetEvalString(stateIndicator.IconSet));
                        indicators.TransformationType = TryEnum<TransformationType>(this.GetEvalString(stateIndicator.TransformationType));
                        indicators.FillColor = this.GetEvalString(stateIndicator.FillColor);

                        indicators.MaximumValue = new GaugeInputValueExpVal();
                        indicators.MaximumValue.AddConstant = this.GetEvalString(stateIndicator.MaximumValue.AddConstant);
                        indicators.MaximumValue.DataElementName = this.GetEvalString(stateIndicator.MaximumValue.DataElementName);
                        indicators.MaximumValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(stateIndicator.MaximumValue.DataElementOutput));
                        indicators.MaximumValue.Formula = TryEnum<Formula>(this.GetEvalString(stateIndicator.MaximumValue.Formula));
                        indicators.MaximumValue.MaxPercent = double.Parse(this.GetEvalString(stateIndicator.MaximumValue.MaxPercent));
                        indicators.MaximumValue.MinPercent = double.Parse(this.GetEvalString(stateIndicator.MaximumValue.MinPercent));
                        indicators.MaximumValue.Multiplier = double.Parse(this.GetEvalString(stateIndicator.MaximumValue.Multiplier));
                        indicators.MaximumValue.Value = double.Parse(this.GetEvalString(stateIndicator.MaximumValue.Value));

                        indicators.MinimumValue = new GaugeInputValueExpVal();
                        indicators.MinimumValue.AddConstant = this.GetEvalString(stateIndicator.MinimumValue.AddConstant);
                        indicators.MinimumValue.DataElementName = this.GetEvalString(stateIndicator.MinimumValue.DataElementName);
                        indicators.MinimumValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(stateIndicator.MinimumValue.DataElementOutput));
                        indicators.MinimumValue.Formula = TryEnum<Formula>(this.GetEvalString(stateIndicator.MinimumValue.Formula));
                        indicators.MinimumValue.MaxPercent = double.Parse(this.GetEvalString(stateIndicator.MinimumValue.MaxPercent));
                        indicators.MinimumValue.MinPercent = double.Parse(this.GetEvalString(stateIndicator.MinimumValue.MinPercent));
                        indicators.MinimumValue.Multiplier = double.Parse(this.GetEvalString(stateIndicator.MinimumValue.Multiplier));
                        indicators.MinimumValue.Value = double.Parse(this.GetEvalString(stateIndicator.MinimumValue.Value));

                        indicators.IndicatorData = new GaugeInputValueExpVal();
                        indicators.IndicatorData.AddConstant = this.GetEvalString(stateIndicator.IndicatorData.AddConstant);
                        indicators.IndicatorData.DataElementName = this.GetEvalString(stateIndicator.IndicatorData.DataElementName);
                        indicators.IndicatorData.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(stateIndicator.IndicatorData.DataElementOutput));
                        indicators.IndicatorData.Formula = TryEnum<Formula>(this.GetEvalString(stateIndicator.IndicatorData.Formula));
                        indicators.IndicatorData.MaxPercent = double.Parse(this.GetEvalString(stateIndicator.IndicatorData.MaxPercent));
                        indicators.IndicatorData.MinPercent = double.Parse(this.GetEvalString(stateIndicator.IndicatorData.MinPercent));
                        indicators.IndicatorData.Multiplier = double.Parse(this.GetEvalString(stateIndicator.IndicatorData.Multiplier));
                        indicators.IndicatorData.Value = double.Parse(this.GetEvalString(stateIndicator.IndicatorData.Value));

                        if (stateIndicator.ActionInfo != null)
                        {
                            indicators.ActionInfo = new List<ActionInfoExpVal>();
                            foreach (var Action in stateIndicator.ActionInfo)
                            {
                                ActionInfoExpVal actionInfo = new ActionInfoExpVal();
                                actionInfo.BookmarkLink = this.GetEvalString(Action.BookmarkLink);
                                actionInfo.Hyperlink = this.GetEvalString(Action.Hyperlink);
                                if (!string.IsNullOrEmpty(Action.ReportName))
                                {
                                    actionInfo.ReportName = this.GetEvalString(Action.ReportName);
                                    actionInfo.Parameters = new List<ParameterExpVal>();
                                    foreach (var parameters in Action.Parameters)
                                    {
                                        ParameterExpVal Parameter = new ParameterExpVal();
                                        Parameter.Name = this.GetEvalString(parameters.Name);
                                        Parameter.Omit = this.GetEvalString(parameters.Omit);
                                        Parameter.Value = this.GetEvalString(parameters.Value);
                                        actionInfo.Parameters.Add(Parameter);
                                    }
                                }
                                indicators.ActionInfo.Add(actionInfo);
                            }
                        }

                        if (stateIndicator.IndicatorState != null && stateIndicator.IndicatorState.Count > 0)
                        {
                            indicators.IndicatorState = new List<IndicatorStateExpVal>();
                            foreach (var indicatorState in stateIndicator.IndicatorState)
                            {
                                IndicatorStateExpVal indicatorStateExp = new IndicatorStateExpVal();
                                indicatorStateExp.ResizeMode = TryEnum<Syncfusion.RDL.DOM.ResizeMode>(this.GetEvalString(indicatorState.ResizeMode));
                                indicatorStateExp.FillColor = this.GetEvalString(indicatorState.FillColor);
                                indicatorStateExp.ScaleFactor = double.Parse(this.GetEvalString(indicatorState.ScaleFactor));
                                indicatorStateExp.IndicatorStyle = TryEnum<GaugeStateIndicatorStyles>(this.GetEvalString(indicatorState.IndicatorStyle));

                                indicatorStateExp.StartValue = new GaugeInputValueExpVal();
                                indicatorStateExp.StartValue.AddConstant = this.GetEvalString(indicatorState.StartValue.AddConstant);
                                indicatorStateExp.StartValue.DataElementName = this.GetEvalString(indicatorState.StartValue.DataElementName);
                                indicatorStateExp.StartValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(indicatorState.StartValue.DataElementOutput));
                                indicatorStateExp.StartValue.Formula = TryEnum<Formula>(this.GetEvalString(indicatorState.StartValue.Formula));
                                indicatorStateExp.StartValue.MaxPercent = double.Parse(this.GetEvalString(indicatorState.StartValue.MaxPercent));
                                indicatorStateExp.StartValue.MinPercent = double.Parse(this.GetEvalString(indicatorState.StartValue.MinPercent));
                                indicatorStateExp.StartValue.Multiplier = double.Parse(this.GetEvalString(indicatorState.StartValue.Multiplier));
                                indicatorStateExp.StartValue.Value = double.Parse(this.GetEvalString(indicatorState.StartValue.Value));

                                indicatorStateExp.EndValue = new GaugeInputValueExpVal();
                                indicatorStateExp.EndValue.AddConstant = this.GetEvalString(indicatorState.EndValue.AddConstant);
                                indicatorStateExp.EndValue.DataElementName = this.GetEvalString(indicatorState.EndValue.DataElementName);
                                indicatorStateExp.EndValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(indicatorState.EndValue.DataElementOutput));
                                indicatorStateExp.EndValue.Formula = TryEnum<Formula>(this.GetEvalString(indicatorState.EndValue.Formula));
                                indicatorStateExp.EndValue.MaxPercent = double.Parse(this.GetEvalString(indicatorState.EndValue.MaxPercent));
                                indicatorStateExp.EndValue.MinPercent = double.Parse(this.GetEvalString(indicatorState.EndValue.MinPercent));
                                indicatorStateExp.EndValue.Multiplier = double.Parse(this.GetEvalString(indicatorState.EndValue.Multiplier));
                                indicatorStateExp.EndValue.Value = double.Parse(this.GetEvalString(indicatorState.EndValue.Value));

                                if (indicatorState.StateImage != null)
                                {
                                    indicatorStateExp.HugeColor = this.GetEvalString(indicatorState.HugeColor);
                                    indicatorStateExp.Transparency = double.Parse(this.GetEvalString(indicatorState.Transparency));

                                    indicatorStateExp.StateImage = new BaseImageExpVal();
                                    if (indicatorState.StateImage != null)
                                    {
                                        indicatorStateExp.StateImage.HueColor = this.GetEvalString(indicatorState.StateImage.HueColor);
                                        indicatorStateExp.StateImage.MIMEType = this.GetEvalString(indicatorState.StateImage.MIMEType);
                                        indicatorStateExp.StateImage.Source = TryEnum<Source>(this.GetEvalString(indicatorState.StateImage.Source.ToString()));
                                        indicatorStateExp.StateImage.TransparentColor = this.GetEvalString(indicatorState.StateImage.TransparentColor);
                                        indicatorStateExp.StateImage.Value = this.GetEvalString(indicatorState.StateImage.Value);
                                    }
                                }
                                indicators.IndicatorState.Add(indicatorStateExp);
                            }
                        }
                        this.GaugePanelProperties.Indicators.Add(indicators);
                    }
                }


                if (this.GaugePanelExpProp.LinearGauges != null && this.GaugePanelExpProp.LinearGauges.Count > 0)
                {
                    this.GaugePanelProperties.LinearGauges = new List<GaugePropertiesExpVal>();
                    foreach (var lineargauge in this.GaugePanelExpProp.LinearGauges)
                    {
                        var Lineargauge = new GaugePropertiesExpVal();
                        Lineargauge.GaugeFrame = new FramePropertiesExpVal();
                        Lineargauge.GaugeStyle = new GaugeStyleExpVal();
                        Lineargauge.GaugeFrame.BackFrameStyle = new GaugeStyleExpVal();
                        Lineargauge.GaugeFrame.FrameImage = new BaseImageExpVal();
                        Lineargauge.GaugeTopImage = new BaseImageExpVal();
                        Lineargauge.ActionInfo = new ActionInfoExpVal();
                        Lineargauge.GaugeFrame.FrameProperty = new GaugeStyleExpVal();
                        if (lineargauge.GaugeFrame.FrameShape != null)
                        {
                            Lineargauge.GaugeFrame.FrameShape = TryEnum<FrameShape>(this.GetEvalString(lineargauge.GaugeFrame.FrameShape));
                        }
                        Lineargauge.GaugeFrame.FrameStyle = TryEnum<FrameStyle>(this.GetEvalString(lineargauge.GaugeFrame.FrameStyle));
                        if (lineargauge.GaugeStyle != null)
                        {
                            Lineargauge.GaugeStyle.BackgroundColor = this.GetEvalString(lineargauge.GaugeStyle.BackgroundColor);
                            Lineargauge.GaugeStyle.BackgroundGradientEndcolor = this.GetEvalString(lineargauge.GaugeStyle.BackgroundGradientEndcolor);
                            Lineargauge.GaugeStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(lineargauge.GaugeStyle.BackgroundGradientType));
                            Lineargauge.GaugeStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(lineargauge.GaugeStyle.BackgroundHatchType));
                            Lineargauge.GaugeStyle.BorderColor = this.GetEvalString(lineargauge.GaugeStyle.BorderColor);
                            Lineargauge.GaugeStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(lineargauge.GaugeStyle.BorderStyle));
                            Lineargauge.GaugeStyle.BorderWidth = new DOM.Size(this.GetEvalString(lineargauge.GaugeStyle.BorderWidth)).FloatValue;
                        }
                        if (lineargauge.GaugeFrame.BackFrameStyle != null)
                        {
                            Lineargauge.GaugeFrame.BackFrameStyle.BackgroundColor = this.GetEvalString(lineargauge.GaugeFrame.BackFrameStyle.BackgroundColor);
                            Lineargauge.GaugeFrame.BackFrameStyle.BackgroundGradientEndcolor = this.GetEvalString(lineargauge.GaugeFrame.BackFrameStyle.BackgroundGradientEndcolor);
                            Lineargauge.GaugeFrame.BackFrameStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(lineargauge.GaugeFrame.BackFrameStyle.BackgroundGradientType));
                            Lineargauge.GaugeFrame.BackFrameStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(lineargauge.GaugeFrame.BackFrameStyle.BackgroundHatchType));
                        }
                        if (lineargauge.GaugeFrame.FrameImage != null)
                        {
                            Lineargauge.GaugeFrame.FrameImage.MIMEType = this.GetEvalString(lineargauge.GaugeFrame.FrameImage.MIMEType);
                            Lineargauge.GaugeFrame.FrameImage.HueColor = this.GetEvalString(lineargauge.GaugeFrame.FrameImage.HueColor);
                            Lineargauge.GaugeFrame.FrameImage.Source = TryEnum<Source>(this.GetEvalString(lineargauge.GaugeFrame.FrameImage.Source));
                            Lineargauge.GaugeFrame.FrameImage.Transparency = this.GetEvalString(lineargauge.GaugeFrame.FrameImage.Transparency);
                            Lineargauge.GaugeFrame.FrameImage.TransparentColor = this.GetEvalString(lineargauge.GaugeFrame.FrameImage.TransparentColor);
                            Lineargauge.GaugeFrame.FrameImage.Value = this.GetEvalString(lineargauge.GaugeFrame.FrameImage.Value);
                            Lineargauge.GaugeFrame.FrameImage.ClipImage = lineargauge.GaugeFrame.FrameImage.ClipImage;
                        }
                        Lineargauge.GaugeFrame.FrameWidth = double.Parse(this.GetEvalString(lineargauge.GaugeFrame.FrameWidth));
                        Lineargauge.GaugeFrame.FrameGlassEffect = TryEnum<GlassEffect>(this.GetEvalString(lineargauge.GaugeFrame.FrameGlassEffect));
                        Lineargauge.GaugeFrame.BackFrameStyle.OffSet = new DOM.Size(this.GetEvalString(lineargauge.GaugeFrame.BackFrameStyle.OffSet)).FloatValue;
                        Lineargauge.GaugeFrame.FrameProperty.BackgroundColor = this.GetEvalString(lineargauge.GaugeFrame.FrameProperty.BackgroundColor);

                        Lineargauge.Hidden = lineargauge.Hidden;
                        if (lineargauge.GaugeTopImage != null)
                        {
                            Lineargauge.GaugeTopImage.HueColor = this.GetEvalString(lineargauge.GaugeTopImage.HueColor);
                            Lineargauge.GaugeTopImage.MIMEType = this.GetEvalString(lineargauge.GaugeTopImage.MIMEType);
                            Lineargauge.GaugeTopImage.Source = TryEnum<Source>(this.GetEvalString(lineargauge.GaugeTopImage.Source));
                            Lineargauge.GaugeTopImage.TransparentColor = this.GetEvalString(lineargauge.GaugeTopImage.TransparentColor);
                            Lineargauge.GaugeTopImage.Value = this.GetEvalString(lineargauge.GaugeTopImage.Value);
                        }
                        Lineargauge.ClipContent = lineargauge.ClipContent;
                        Lineargauge.GaugeTooltip = this.GetEvalString(lineargauge.GaugeTooltip);
                        //Lineargauge.AspectRatio =new DOM.Size(this.GetEvalExpressionString(lineargauge.AspectRatio)).in;
                        Lineargauge.GaugeHeight = double.Parse(this.GetEvalString(lineargauge.GaugeHeight));
                        Lineargauge.GaugeWidth = double.Parse(this.GetEvalString(lineargauge.GaugeWidth));
                        Lineargauge.Left = double.Parse(this.GetEvalString(lineargauge.Left));
                        Lineargauge.Top = double.Parse(this.GetEvalString(lineargauge.Top));
                        Lineargauge.Orientation = TryEnum<Orientation>(this.GetEvalString(lineargauge.Orientation));
                        Lineargauge.GaugeWidth = double.Parse(this.GetEvalString(lineargauge.GaugeWidth));
                        Lineargauge.GaugeHeight = double.Parse(this.GetEvalString(lineargauge.GaugeHeight));

                        if (lineargauge.ActionInfo != null)
                        {
                            Lineargauge.ActionInfo.BookmarkLink = this.GetEvalString(lineargauge.ActionInfo.BookmarkLink);
                            Lineargauge.ActionInfo.Hyperlink = this.GetEvalString(lineargauge.ActionInfo.Hyperlink);
                            if (lineargauge.ActionInfo.ReportName != null)
                            {
                                Lineargauge.ActionInfo.ReportName = this.GetEvalString(lineargauge.ActionInfo.ReportName);
                            }
                            if (lineargauge.ActionInfo.Parameters != null)
                            {
                                foreach (var Action in lineargauge.ActionInfo.Parameters)
                                {
                                    ParameterExpVal parameter = new ParameterExpVal();
                                    parameter.Name = this.GetEvalString(Action.Name);
                                    parameter.Omit = this.GetEvalString(Action.Omit);
                                    parameter.Value = this.GetEvalString(Action.Value);
                                    Lineargauge.ActionInfo.Parameters.Add(parameter);
                                }
                            }
                        }

                        Lineargauge.GaugeScales = new List<GaugeScalePropertiesExpVal>();
                        foreach (var gaugescale in lineargauge.GaugeScales)
                        {
                            var Scale = new GaugeScalePropertiesExpVal();
                            Scale.ScaleStyle = new GaugeStyleExpVal();
                            Scale.ScaleLabel = new GaugeLabelExpVal();
                            Scale.ScaleLabel.LabelStyle = new GaugeStyleExpVal();
                            Scale.MajorTickMark = new TickMarksExpVal();
                            Scale.MajorTickMark.TickMarkStyle = new GaugeStyleExpVal();
                            Scale.MajorTickMark.TickMarkImage = new BaseImageExpVal();
                            Scale.MinorTickMark = new TickMarksExpVal();
                            Scale.MinorTickMark.TickMarkStyle = new GaugeStyleExpVal();
                            Scale.MinorTickMark.TickMarkImage = new BaseImageExpVal();
                            Scale.MaximumPin = new ScalePinExpVal();
                            Scale.MaximumPin.PinImage = new BaseImageExpVal();
                            Scale.MaximumPin.PinLabel = new GaugeLabelExpVal();
                            Scale.MaximumPin.PinLabel.LabelStyle = new GaugeStyleExpVal();
                            Scale.MinimumPin = new ScalePinExpVal();
                            Scale.MinimumPin.PinImage = new BaseImageExpVal();
                            Scale.MinimumPin.PinLabel = new GaugeLabelExpVal();
                            Scale.MinimumPin.PinLabel.LabelStyle = new GaugeStyleExpVal();
                            Scale.MaximumValue = new GaugeInputValueExpVal();
                            Scale.MinimumValue = new GaugeInputValueExpVal();
                            Scale.ActionInfo = new ActionInfoExpVal();
                            if (gaugescale.ScaleStyle != null)
                            {
                                Scale.ScaleStyle.BorderColor = this.GetEvalString(gaugescale.ScaleStyle.BorderColor);
                                Scale.ScaleStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(gaugescale.ScaleStyle.BorderStyle));
                                Scale.ScaleStyle.BorderWidth = new DOM.Size(this.GetEvalString(gaugescale.ScaleStyle.BorderWidth)).FloatValue;
                                Scale.ScaleStyle.BackgroundColor = this.GetEvalString(gaugescale.ScaleStyle.BackgroundColor);
                                Scale.ScaleStyle.BackgroundGradientEndcolor = this.GetEvalString(gaugescale.ScaleStyle.BackgroundGradientEndcolor);
                                Scale.ScaleStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(gaugescale.ScaleStyle.BackgroundGradientType));
                                Scale.ScaleStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(gaugescale.ScaleStyle.BackgroundHatchType));
                            }
                            Scale.Hidden = gaugescale.Hidden;
                            Scale.ScaleStyle.OffSet = new DOM.Size(this.GetEvalString(gaugescale.ScaleStyle.OffSet)).FloatValue;
                            Scale.LabelMultiplier = double.Parse(this.GetEvalString(gaugescale.LabelMultiplier));
                            Scale.LogBase = double.Parse(this.GetEvalString(gaugescale.LogBase));
                            Scale.LogrithmicScale = gaugescale.LogrithmicScale;
                            Scale.ReverseDirection = gaugescale.ReverseDirection;
                            Scale.StartMargin = double.Parse(this.GetEvalString(gaugescale.StartMargin));
                            Scale.EndMargin = double.Parse(this.GetEvalString(gaugescale.EndMargin));
                            Scale.Position = TryEnum<Position>(this.GetEvalString(gaugescale.Position));

                            if (gaugescale.ScaleLabel != null)
                            {
                                Scale.ScaleLabel.ScaleDistance = new DOM.Size(this.GetEvalString(gaugescale.ScaleLabel.ScaleDistance)).FloatValue;
                                if (gaugescale.ScaleLabel.LabelStyle != null)
                                {
                                    Scale.ScaleLabel.LabelStyle.FontFamily = this.GetEvalString(gaugescale.ScaleLabel.LabelStyle.FontFamily);
                                    Scale.ScaleLabel.LabelStyle.FontSize = new DOM.Size(this.GetEvalString(gaugescale.ScaleLabel.LabelStyle.FontSize)).FloatValue;
                                    Scale.ScaleLabel.LabelStyle.FontStyle = TryEnum<RDL.DOM.FontStyle>(this.GetEvalString(gaugescale.ScaleLabel.LabelStyle.FontStyle));
                                    Scale.ScaleLabel.LabelStyle.FontWeight = TryEnum<RDL.DOM.FontWeight>(this.GetEvalString(gaugescale.ScaleLabel.LabelStyle.FontWeight));
                                }
                                Scale.ScaleLabel.FontAngle = double.Parse(this.GetEvalString(gaugescale.ScaleLabel.FontAngle));
                                Scale.ScaleLabel.FormatString = this.GetEvalString(gaugescale.ScaleLabel.FormatString);//formatstring
                                Scale.ScaleLabel.Hidden = gaugescale.ScaleLabel.Hidden;
                                Scale.ScaleLabel.LabelInterval = double.Parse(this.GetEvalString(gaugescale.ScaleLabel.LabelInterval));
                                Scale.ScaleLabel.LabelIntervalOffset = double.Parse(this.GetEvalString(gaugescale.ScaleLabel.LabelIntervalOffset));
                                Scale.ScaleLabel.ScalePlacment = TryEnum<Placement>(this.GetEvalString(gaugescale.ScaleLabel.ScalePlacment));
                                Scale.ScaleLabel.RotateLabel = gaugescale.ScaleLabel.RotateLabel;
                                Scale.ScaleLabel.EndLabel = gaugescale.ScaleLabel.ShowEndLabel;
                                Scale.ScaleLabel.TextColor = this.GetEvalString(gaugescale.ScaleLabel.TextColor);//textcolor
                                Scale.ScaleLabel.TextDecoration = TryEnum<RDL.DOM.TextDecoration>(this.GetEvalString(gaugescale.ScaleLabel.TextDecoration));
                                Scale.ScaleLabel.UseFontPercent = gaugescale.ScaleLabel.UseFontPercent;
                            }

                            if (gaugescale.MajorTickMark != null)
                            {
                                if (gaugescale.MajorTickMark.TickMarkStyle != null)
                                {
                                    Scale.MajorTickMark.TickMarkStyle.BorderColor = this.GetEvalString(gaugescale.MajorTickMark.TickMarkStyle.BorderColor);
                                    Scale.MajorTickMark.TickMarkStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(gaugescale.MajorTickMark.TickMarkStyle.BorderStyle));
                                    Scale.MajorTickMark.TickMarkStyle.BorderWidth = new DOM.Size(this.GetEvalString(gaugescale.MajorTickMark.TickMarkStyle.BorderWidth)).FloatValue;
                                }
                                Scale.MajorTickMark.DistanceFromScale = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.DistanceFromScale));
                                Scale.MajorTickMark.EnableGradient = gaugescale.MajorTickMark.EnableGradient;
                                Scale.MajorTickMark.FillColor = this.GetEvalString(gaugescale.MajorTickMark.FillColor);//fillcolor
                                Scale.MajorTickMark.GradientDensity = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.GradientDensity));
                                Scale.MajorTickMark.HideTickMark = gaugescale.MajorTickMark.HideTickMark;
                                Scale.MajorTickMark.Interval = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.Interval));
                                Scale.MajorTickMark.IntervalOffset = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.IntervalOffset));
                                Scale.MajorTickMark.Length = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.Length));
                                Scale.MajorTickMark.TickMarkPlacement = TryEnum<Placement>(this.GetEvalString(gaugescale.MajorTickMark.TickMarkPlacement));
                                Scale.MajorTickMark.TickMarkShape = this.GetEvalString(gaugescale.MajorTickMark.TickMarkShape);
                                if (gaugescale.MajorTickMark.TickMarkImage != null)
                                {
                                    Scale.MajorTickMark.TickMarkImage.MIMEType = this.GetEvalString(gaugescale.MajorTickMark.TickMarkImage.MIMEType);
                                    Scale.MajorTickMark.TickMarkImage.Source = TryEnum<Source>(this.GetEvalString(gaugescale.MajorTickMark.TickMarkImage.Source));
                                    Scale.MajorTickMark.TickMarkImage.TransparentColor = this.GetEvalString(gaugescale.MajorTickMark.TickMarkImage.TransparentColor);
                                    Scale.MajorTickMark.TickMarkImage.Value = this.GetEvalString(gaugescale.MajorTickMark.TickMarkImage.Value);
                                    Scale.MajorTickMark.TickMarkImage.HueColor = this.GetEvalString(gaugescale.MajorTickMark.TickMarkImage.HueColor);
                                }
                                Scale.MajorTickMark.Width = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.Width));
                            }
                            if (gaugescale.MinorTickMark != null)
                            {
                                if (gaugescale.MinorTickMark.TickMarkStyle != null)
                                {
                                    Scale.MinorTickMark.TickMarkStyle.BorderColor = this.GetEvalString(gaugescale.MinorTickMark.TickMarkStyle.BorderColor);
                                    Scale.MinorTickMark.TickMarkStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(gaugescale.MinorTickMark.TickMarkStyle.BorderStyle));
                                    Scale.MinorTickMark.TickMarkStyle.BorderWidth = new DOM.Size(this.GetEvalString(gaugescale.MinorTickMark.TickMarkStyle.BorderWidth)).FloatValue;
                                }
                                Scale.MinorTickMark.DistanceFromScale = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.DistanceFromScale));
                                Scale.MinorTickMark.EnableGradient = gaugescale.MinorTickMark.EnableGradient;
                                Scale.MinorTickMark.FillColor = this.GetEvalString(gaugescale.MinorTickMark.FillColor);//fillcolor
                                Scale.MinorTickMark.GradientDensity = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.GradientDensity));
                                Scale.MinorTickMark.HideTickMark = gaugescale.MinorTickMark.HideTickMark;
                                Scale.MinorTickMark.Interval = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.Interval));
                                Scale.MinorTickMark.IntervalOffset = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.IntervalOffset));
                                Scale.MinorTickMark.Length = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.Length));
                                Scale.MinorTickMark.TickMarkPlacement = TryEnum<Placement>(this.GetEvalString(gaugescale.MinorTickMark.TickMarkPlacement));
                                Scale.MinorTickMark.TickMarkShape = this.GetEvalString(gaugescale.MinorTickMark.TickMarkShape);
                                if (gaugescale.MinorTickMark.TickMarkImage != null)
                                {
                                    Scale.MinorTickMark.TickMarkImage.MIMEType = this.GetEvalString(gaugescale.MinorTickMark.TickMarkImage.MIMEType);
                                    Scale.MinorTickMark.TickMarkImage.Source = TryEnum<Source>(this.GetEvalString(gaugescale.MinorTickMark.TickMarkImage.Source));
                                    Scale.MinorTickMark.TickMarkImage.TransparentColor = this.GetEvalString(gaugescale.MinorTickMark.TickMarkImage.TransparentColor);
                                    Scale.MinorTickMark.TickMarkImage.Value = this.GetEvalString(gaugescale.MinorTickMark.TickMarkImage.Value);
                                    Scale.MinorTickMark.TickMarkImage.HueColor = this.GetEvalString(gaugescale.MinorTickMark.TickMarkImage.HueColor);
                                }
                                Scale.MinorTickMark.Width = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.Width));
                            }
                            Scale.ScaleInterval = double.Parse(this.GetEvalString(gaugescale.ScaleInterval));
                            Scale.ScaleIntervaloffset = double.Parse(this.GetEvalString(gaugescale.ScaleIntervaloffset));

                            if (gaugescale.MaximumPin != null)
                            {
                                Scale.MaximumPin.Location = this.GetEvalString(gaugescale.MaximumPin.Location);
                                Scale.MaximumPin.Enable = gaugescale.MaximumPin.Enable;
                                Scale.MaximumPin.PinLabel.AllowUpsideDown = gaugescale.MaximumPin.PinLabel.AllowUpsideDown;
                                Scale.MaximumPin.PinLabel.ScaleDistance = new DOM.Size(this.GetEvalString(gaugescale.MaximumPin.PinLabel.ScaleDistance)).FloatValue;
                                if (gaugescale.MaximumPin.PinLabel.LabelStyle != null)
                                {
                                    Scale.MaximumPin.PinLabel.LabelStyle.FontSize = new DOM.Size(this.GetEvalString(gaugescale.MaximumPin.PinLabel.LabelStyle.FontSize)).FloatValue;
                                    Scale.MaximumPin.PinLabel.LabelStyle.FontStyle = TryEnum<RDL.DOM.FontStyle>(this.GetEvalString(gaugescale.MaximumPin.PinLabel.LabelStyle.FontStyle));
                                    Scale.MaximumPin.PinLabel.LabelStyle.FontWeight = TryEnum<RDL.DOM.FontWeight>(this.GetEvalString(gaugescale.MaximumPin.PinLabel.LabelStyle.FontWeight));
                                    Scale.MaximumPin.PinLabel.LabelStyle.FontFamily = this.GetEvalString(gaugescale.MaximumPin.PinLabel.LabelStyle.FontFamily);
                                }
                                Scale.MaximumPin.PinLabel.FontAngle = double.Parse(this.GetEvalString(gaugescale.MaximumPin.PinLabel.FontAngle));
                                Scale.MaximumPin.PinLabel.ScalePlacment = TryEnum<Placement>(this.GetEvalString(gaugescale.MaximumPin.PinLabel.ScalePlacment));
                                Scale.MaximumPin.PinLabel.UseFontPercent = gaugescale.MaximumPin.PinLabel.UseFontPercent;
                                Scale.MaximumPin.PinLabel.Text = this.GetEvalString(gaugescale.MaximumPin.PinLabel.Text);
                                Scale.MaximumPin.PinLabel.RotateLabel = gaugescale.MaximumPin.PinLabel.RotateLabel;
                                Scale.MaximumPin.PinLabel.TextDecoration = TryEnum<RDL.DOM.TextDecoration>(this.GetEvalString(gaugescale.MaximumPin.PinLabel.TextDecoration));
                                Scale.MaximumPin.PinLabel.TextColor = this.GetEvalString(gaugescale.MaximumPin.PinLabel.TextColor);
                            }
                            if (gaugescale.MinimumPin != null)
                            {
                                Scale.MinimumPin.Location = this.GetEvalString(gaugescale.MinimumPin.Location);
                                Scale.MinimumPin.Enable = gaugescale.MinimumPin.Enable;
                                Scale.MinimumPin.PinLabel.AllowUpsideDown = gaugescale.MinimumPin.PinLabel.AllowUpsideDown;
                                Scale.MinimumPin.PinLabel.ScaleDistance = new DOM.Size(this.GetEvalString(gaugescale.MinimumPin.PinLabel.ScaleDistance)).FloatValue;
                                if (gaugescale.MinimumPin.PinLabel.LabelStyle != null)
                                {
                                    Scale.MinimumPin.PinLabel.LabelStyle.FontSize = new DOM.Size(this.GetEvalString(gaugescale.MinimumPin.PinLabel.LabelStyle.FontSize)).FloatValue;
                                    Scale.MinimumPin.PinLabel.LabelStyle.FontStyle = TryEnum<RDL.DOM.FontStyle>(this.GetEvalString(gaugescale.MinimumPin.PinLabel.LabelStyle.FontStyle));
                                    Scale.MinimumPin.PinLabel.LabelStyle.FontWeight = TryEnum<RDL.DOM.FontWeight>(this.GetEvalString(gaugescale.MinimumPin.PinLabel.LabelStyle.FontWeight));
                                    Scale.MinimumPin.PinLabel.LabelStyle.FontFamily = this.GetEvalString(gaugescale.MinimumPin.PinLabel.LabelStyle.FontFamily);
                                }
                                Scale.MinimumPin.PinLabel.FontAngle = double.Parse(this.GetEvalString(gaugescale.MinimumPin.PinLabel.FontAngle));
                                Scale.MinimumPin.PinLabel.ScalePlacment = TryEnum<Placement>(this.GetEvalString(gaugescale.MinimumPin.PinLabel.ScalePlacment));
                                Scale.MinimumPin.PinLabel.UseFontPercent = gaugescale.MinimumPin.PinLabel.UseFontPercent;
                                Scale.MinimumPin.PinLabel.Text = this.GetEvalString(gaugescale.MinimumPin.PinLabel.Text);
                                Scale.MinimumPin.PinLabel.RotateLabel = gaugescale.MinimumPin.PinLabel.RotateLabel;
                                Scale.MinimumPin.PinLabel.TextDecoration = TryEnum<RDL.DOM.TextDecoration>(this.GetEvalString(gaugescale.MinimumPin.PinLabel.TextDecoration));
                                Scale.MinimumPin.PinLabel.TextColor = this.GetEvalString(gaugescale.MinimumPin.PinLabel.TextColor);
                            }

                            Scale.MaximumValue.AddConstant = this.GetEvalString(gaugescale.MaximumValue.AddConstant);
                            Scale.MaximumValue.DataElementName = this.GetEvalString(gaugescale.MaximumValue.DataElementName);
                            Scale.MaximumValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(gaugescale.MaximumValue.DataElementOutput));
                            Scale.MaximumValue.Formula = TryEnum<Formula>(this.GetEvalString(gaugescale.MaximumValue.Formula));
                            Scale.MaximumValue.MaxPercent = double.Parse(this.GetEvalString(gaugescale.MaximumValue.MaxPercent));
                            Scale.MaximumValue.MinPercent = double.Parse(this.GetEvalString(gaugescale.MaximumValue.MinPercent));
                            Scale.MaximumValue.Multiplier = double.Parse(this.GetEvalString(gaugescale.MaximumValue.Multiplier));
                            Scale.MaximumValue.Value = double.Parse(this.GetEvalString(gaugescale.MaximumValue.Value));

                            Scale.MinimumValue.AddConstant = this.GetEvalString(gaugescale.MinimumValue.AddConstant);
                            Scale.MinimumValue.DataElementName = this.GetEvalString(gaugescale.MinimumValue.DataElementName);
                            Scale.MinimumValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(gaugescale.MinimumValue.DataElementOutput));
                            Scale.MinimumValue.Formula = TryEnum<Formula>(this.GetEvalString(gaugescale.MinimumValue.Formula));
                            Scale.MinimumValue.MaxPercent = double.Parse(this.GetEvalString(gaugescale.MinimumValue.MaxPercent));
                            Scale.MinimumValue.MinPercent = double.Parse(this.GetEvalString(gaugescale.MinimumValue.MinPercent));
                            Scale.MinimumValue.Multiplier = double.Parse(this.GetEvalString(gaugescale.MinimumValue.Multiplier));
                            Scale.MinimumValue.Value = double.Parse(this.GetEvalString(gaugescale.MinimumValue.Value));
                            Scale.TickMarksonTop = gaugescale.TickMarksonTop;
                            Scale.ToolTip = this.GetEvalString(gaugescale.ToolTip);
                            Scale.ScaleWidth = double.Parse(this.GetEvalString(gaugescale.ScaleWidth));

                            if (gaugescale.ActionInfo != null)
                            {
                                Scale.ActionInfo.BookmarkLink = this.GetEvalString(gaugescale.ActionInfo.BookmarkLink);
                                Scale.ActionInfo.Hyperlink = this.GetEvalString(gaugescale.ActionInfo.Hyperlink);
                                if (gaugescale.ActionInfo.ReportName != null)
                                {
                                    Scale.ActionInfo.ReportName = this.GetEvalString(gaugescale.ActionInfo.ReportName);
                                }

                                Scale.ActionInfo.Parameters = new List<ParameterExpVal>();
                                if (gaugescale.ActionInfo.Parameters != null)
                                {
                                    foreach (var Action in gaugescale.ActionInfo.Parameters)
                                    {
                                        var parameter = new ParameterExpVal();
                                        parameter.Name = this.GetEvalString(Action.Name);
                                        parameter.Omit = this.GetEvalString(Action.Omit);
                                        parameter.Value = this.GetEvalString(Action.Value);
                                        Scale.ActionInfo.Parameters.Add(parameter);
                                    }
                                }
                            }

                            Scale.ScalePointer = new List<PointerExpVal>();
                            if (gaugescale.ScalePointer != null)
                            {
                                foreach (var gaugepointer in gaugescale.ScalePointer)
                                {
                                    var Pointer = new PointerExpVal();
                                    Pointer.PointerImage = new PointerImageExpVal();
                                    Pointer.PointerStyle = new GaugeStyleExpVal();
                                    Pointer.PointerImage.PointerImage = new BaseImageExpVal();
                                    Pointer.LinearPointer = new LinearPointerExpVal();
                                    Pointer.LinearPointer.ThermometerProperty = new GaugeStyleExpVal();
                                    Pointer.Value = new GaugeInputValueExpVal();
                                    Pointer.ActionInfo = new ActionInfoExpVal();

                                    Pointer.PointerStyle.BackgroundColor = this.GetEvalString(gaugepointer.PointerStyle.BackgroundColor);
                                    Pointer.PointerStyle.BackgroundGradientEndcolor = this.GetEvalString(gaugepointer.PointerStyle.BackgroundGradientEndcolor);
                                    Pointer.PointerStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(gaugepointer.PointerStyle.BackgroundGradientType));
                                    Pointer.PointerStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(gaugepointer.PointerStyle.BackgroundHatchType));
                                    Pointer.PointerStyle.BorderColor = this.GetEvalString(gaugepointer.PointerStyle.BorderColor);
                                    Pointer.PointerStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(gaugepointer.PointerStyle.BorderStyle));
                                    Pointer.PointerStyle.BorderWidth = new DOM.Size(this.GetEvalString(gaugepointer.PointerStyle.BorderWidth)).FloatValue;
                                    Pointer.Hidden = gaugepointer.Hidden;
                                    Pointer.PointerImage.PivotX = new DOM.Size(this.GetEvalString(gaugepointer.PointerImage.PivotX)).FloatValue;
                                    Pointer.PointerImage.PivotY = new DOM.Size(this.GetEvalString(gaugepointer.PointerImage.PivotY)).FloatValue;
                                    if (gaugepointer.PointerImage.PointerImage != null)
                                    {
                                        Pointer.PointerImage.PointerImage.HueColor = this.GetEvalString(gaugepointer.PointerImage.PointerImage.HueColor);
                                        Pointer.PointerImage.PointerImage.MIMEType = this.GetEvalString(gaugepointer.PointerImage.PointerImage.MIMEType);
                                        Pointer.PointerImage.PointerImage.Source = TryEnum<Source>(this.GetEvalString(gaugepointer.PointerImage.PointerImage.Source));
                                        Pointer.PointerImage.PointerImage.Transparency = this.GetEvalString(gaugepointer.PointerImage.PointerImage.Transparency);
                                        Pointer.PointerImage.PointerImage.TransparentColor = this.GetEvalString(gaugepointer.PointerImage.PointerImage.TransparentColor);
                                        Pointer.PointerImage.PointerImage.Value = this.GetEvalString(gaugepointer.PointerImage.PointerImage.Value);
                                    }
                                    Pointer.PointerStyle.OffSet = new DOM.Size(this.GetEvalString(gaugepointer.PointerStyle.OffSet)).FloatValue;
                                    Pointer.SnappingInterval = double.Parse(this.GetEvalString(gaugepointer.SnappingInterval));
                                    Pointer.SnappingEnabled = gaugepointer.SnappingEnabled;
                                    Pointer.PointerTooltip = this.GetEvalString(gaugepointer.PointerTooltip);
                                    Pointer.ScaleDistance = double.Parse(this.GetEvalString(gaugepointer.ScaleDistance));
                                    Pointer.Placement = TryEnum<Placement>(this.GetEvalString(gaugepointer.Placement));
                                    Pointer.PointerWidth = double.Parse(this.GetEvalString(gaugepointer.PointerWidth));
                                    Pointer.LinearPointer.PointerType = TryEnum<LinearPointerType>(this.GetEvalString(gaugepointer.LinearPointer.PointerType));
                                    Pointer.BarStart = TryEnum<BarStart>(this.GetEvalString(gaugepointer.BarStart));
                                    Pointer.MarkerLength = double.Parse(this.GetEvalString(gaugepointer.MarkerLength));
                                    Pointer.MarkerStyle = TryEnum<MarkerStyle>(this.GetEvalString(gaugepointer.MarkerStyle));
                                    Pointer.LinearPointer.ThermometerProperty.BackgroundColor = this.GetEvalString(gaugepointer.LinearPointer.ThermometerProperty.BackgroundColor);
                                    Pointer.LinearPointer.ThermometerProperty.BackgroundGradientEndcolor = this.GetEvalString(gaugepointer.LinearPointer.ThermometerProperty.BackgroundGradientEndcolor);
                                    //if (gaugepointer.LinearPointer.ThermometerProperty.BackgroundGradientType!=null)
                                    //{
                                    //Pointer.LinearPointer.ThermometerProperty.BackgroundGradientType = (BackgroundGradientType)Enum.Parse(typeof(BackgroundGradientType), this.GetEvalExpressionString(gaugepointer.LinearPointer.ThermometerProperty.BackgroundGradientType), true);
                                    //}
                                    Pointer.LinearPointer.ThermometerProperty.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(gaugepointer.LinearPointer.ThermometerProperty.BackgroundHatchType));
                                    Pointer.LinearPointer.ThermometerStyle = TryEnum<ThermometerStyle>(this.GetEvalString(gaugepointer.LinearPointer.ThermometerStyle));
                                    Pointer.LinearPointer.BulbOffset = double.Parse(this.GetEvalString(gaugepointer.LinearPointer.BulbOffset));
                                    Pointer.LinearPointer.BulbSize = double.Parse(this.GetEvalString(gaugepointer.LinearPointer.BulbSize));

                                    Pointer.Value.AddConstant = this.GetEvalString(gaugepointer.Value.AddConstant);
                                    Pointer.Value.DataElementName = this.GetEvalString(gaugepointer.Value.DataElementName);
                                    Pointer.Value.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(gaugepointer.Value.DataElementOutput));
                                    Pointer.Value.Formula = TryEnum<Formula>(this.GetEvalString(gaugepointer.Value.Formula));
                                    Pointer.Value.MaxPercent = double.Parse(this.GetEvalString(gaugepointer.Value.MaxPercent));
                                    Pointer.Value.MinPercent = double.Parse(this.GetEvalString(gaugepointer.Value.MinPercent));
                                    Pointer.Value.Multiplier = double.Parse(this.GetEvalString(gaugepointer.Value.Multiplier));
                                    if (gaugepointer.Value.Value != null)
                                    {
                                        Pointer.Value.Value = double.Parse(this.GetEvalString(gaugepointer.Value.Value));
                                    }

                                    if (gaugepointer.ActionInfo != null)
                                    {
                                        Pointer.ActionInfo.BookmarkLink = this.GetEvalString(gaugepointer.ActionInfo.BookmarkLink);
                                        Pointer.ActionInfo.Hyperlink = this.GetEvalString(gaugepointer.ActionInfo.Hyperlink);
                                        Pointer.ActionInfo.ReportName = this.GetEvalString(gaugepointer.ActionInfo.ReportName);

                                        Pointer.ActionInfo.Parameters = new List<ParameterExpVal>();
                                        foreach (var Action in gaugepointer.ActionInfo.Parameters)
                                        {
                                            var parameter = new ParameterExpVal();
                                            parameter.Name = this.GetEvalString(Action.Name);
                                            parameter.Omit = this.GetEvalString(Action.Omit);
                                            parameter.Value = this.GetEvalString(Action.Value);
                                            Pointer.ActionInfo.Parameters.Add(parameter);
                                        }
                                    }
                                    Scale.ScalePointer.Add(Pointer);
                                }
                            }

                            Scale.ScaleRange = new List<ScaleRangeExpVal>();
                            if (gaugescale.ScaleRange != null)
                            {
                                foreach (var scalerange in gaugescale.ScaleRange)
                                {
                                    var Range = new ScaleRangeExpVal();
                                    Range.RangeStyle = new GaugeStyleExpVal();
                                    Range.StartValue = new GaugeInputValueExpVal();
                                    Range.EndValue = new GaugeInputValueExpVal();
                                    if (scalerange.RangeStyle != null)
                                    {
                                        Range.RangeStyle.BackgroundColor = this.GetEvalString(scalerange.RangeStyle.BackgroundColor);
                                        Range.RangeStyle.BackgroundGradientEndcolor = this.GetEvalString(scalerange.RangeStyle.BackgroundGradientEndcolor);
                                        Range.RangeStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(scalerange.RangeStyle.BackgroundGradientType));
                                        Range.RangeStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(scalerange.RangeStyle.BackgroundHatchType));
                                        Range.RangeStyle.BorderColor = this.GetEvalString(scalerange.RangeStyle.BorderColor);
                                        Range.RangeStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(scalerange.RangeStyle.BorderStyle));
                                        Range.RangeStyle.BorderWidth = new DOM.Size(this.GetEvalString(scalerange.RangeStyle.BorderWidth)).FloatValue;
                                    }
                                    Range.Hidden = scalerange.Hidden;
                                    Range.RangeStyle.OffSet = new DOM.Size(this.GetEvalString(scalerange.RangeStyle.OffSet)).FloatValue;
                                    Range.InRangeBarColor = this.GetEvalString(scalerange.InRangeBarColor);
                                    Range.InRangeLabelColor = this.GetEvalString(scalerange.InRangeLabelColor);
                                    Range.InRangeTickmarkColor = this.GetEvalString(scalerange.InRangeTickmarkColor);
                                    Range.ToolTip = this.GetEvalString(scalerange.ToolTip);
                                    Range.EndWidth = double.Parse(this.GetEvalString(scalerange.EndWidth));
                                    Range.RangePlacement = TryEnum<Placement>(this.GetEvalString(scalerange.RangePlacement));
                                    Range.ScaleDistance = double.Parse(this.GetEvalString(scalerange.ScaleDistance));
                                    Range.StartWidth = double.Parse(this.GetEvalString(scalerange.StartWidth));

                                    Range.EndValue.AddConstant = this.GetEvalString(scalerange.EndValue.AddConstant);
                                    Range.EndValue.DataElementName = this.GetEvalString(scalerange.EndValue.DataElementName);
                                    Range.EndValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(scalerange.EndValue.DataElementOutput));
                                    Range.EndValue.Formula = TryEnum<Formula>(this.GetEvalString(scalerange.EndValue.Formula));
                                    Range.EndValue.MaxPercent = double.Parse(this.GetEvalString(scalerange.EndValue.MaxPercent));
                                    Range.EndValue.MinPercent = double.Parse(this.GetEvalString(scalerange.EndValue.MinPercent));
                                    Range.EndValue.Multiplier = double.Parse(this.GetEvalString(scalerange.EndValue.Multiplier));
                                    Range.EndValue.Value = new DOM.Size(this.GetEvalString(scalerange.EndValue.Value)).FloatValue;

                                    Range.StartValue.AddConstant = this.GetEvalString(scalerange.StartValue.AddConstant);
                                    Range.StartValue.DataElementName = this.GetEvalString(scalerange.StartValue.DataElementName);
                                    Range.StartValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(scalerange.StartValue.DataElementOutput));
                                    Range.StartValue.Formula = TryEnum<Formula>(this.GetEvalString(scalerange.StartValue.Formula));
                                    Range.StartValue.MaxPercent = double.Parse(this.GetEvalString(scalerange.StartValue.MaxPercent));
                                    Range.StartValue.MinPercent = double.Parse(this.GetEvalString(scalerange.StartValue.MinPercent));
                                    Range.StartValue.Multiplier = double.Parse(this.GetEvalString(scalerange.StartValue.Multiplier));
                                    Range.StartValue.Value = new DOM.Size(this.GetEvalString(scalerange.StartValue.Value)).FloatValue;

                                    if (scalerange.ActionInfo != null)
                                    {
                                        Range.ActionInfo.BookmarkLink = this.GetEvalString(scalerange.ActionInfo.BookmarkLink);
                                        Range.ActionInfo.Hyperlink = this.GetEvalString(scalerange.ActionInfo.Hyperlink);
                                        Range.ActionInfo.ReportName = this.GetEvalString(scalerange.ActionInfo.ReportName);

                                        Range.ActionInfo.Parameters = new List<ParameterExpVal>();
                                        foreach (var Action in scalerange.ActionInfo.Parameters)
                                        {
                                            ParameterExpVal parameter = new ParameterExpVal();
                                            parameter.Name = this.GetEvalString(Action.Name);
                                            parameter.Omit = this.GetEvalString(Action.Omit);
                                            parameter.Value = this.GetEvalString(Action.Value);
                                            Range.ActionInfo.Parameters.Add(parameter);
                                        }
                                    }
                                    Scale.ScaleRange.Add(Range);
                                }
                            }
                            Lineargauge.GaugeScales.Add(Scale);
                        }
                        this.GaugePanelProperties.LinearGauges.Add(Lineargauge);
                    }
                }


                if (this.GaugePanelExpProp.RadialGauges != null && this.GaugePanelExpProp.RadialGauges.Count > 0)
                {
                    this.GaugePanelProperties.RadialGauges = new List<GaugePropertiesExpVal>();
                    foreach (var radialgauge in this.GaugePanelExpProp.RadialGauges)
                    {
                        var Radialgauge = new GaugePropertiesExpVal();
                        Radialgauge.GaugeFrame = new FramePropertiesExpVal();
                        Radialgauge.GaugeStyle = new GaugeStyleExpVal();
                        Radialgauge.GaugeFrame.BackFrameStyle = new GaugeStyleExpVal();
                        Radialgauge.GaugeFrame.FrameImage = new BaseImageExpVal();
                        Radialgauge.GaugeTopImage = new BaseImageExpVal();
                        Radialgauge.ActionInfo = new ActionInfoExpVal();
                        Radialgauge.GaugeFrame.FrameProperty = new GaugeStyleExpVal();

                        Radialgauge.GaugeFrame.FrameShape = TryEnum<FrameShape>(this.GetEvalString(radialgauge.GaugeFrame.FrameShape));
                        Radialgauge.GaugeFrame.FrameStyle = TryEnum<FrameStyle>(this.GetEvalString(radialgauge.GaugeFrame.FrameStyle));
                        if (radialgauge.GaugeStyle != null)
                        {
                            Radialgauge.GaugeStyle.BackgroundColor = this.GetEvalString(radialgauge.GaugeStyle.BackgroundColor);
                            Radialgauge.GaugeStyle.BackgroundGradientEndcolor = this.GetEvalString(radialgauge.GaugeStyle.BackgroundGradientEndcolor);
                            if (radialgauge.GaugeStyle.BackgroundGradientType != "Default")
                            {
                                Radialgauge.GaugeStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(radialgauge.GaugeStyle.BackgroundGradientType));
                            }
                            Radialgauge.GaugeStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(radialgauge.GaugeStyle.BackgroundHatchType));
                            Radialgauge.GaugeStyle.BorderColor = this.GetEvalString(radialgauge.GaugeStyle.BorderColor);
                            if (radialgauge.GaugeStyle.BorderStyle != null)
                            {
                                Radialgauge.GaugeStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(radialgauge.GaugeStyle.BorderStyle));
                            }
                            Radialgauge.GaugeStyle.BorderWidth = new DOM.Size(this.GetEvalString(radialgauge.GaugeStyle.BorderWidth)).FloatValue;
                        }
                        if (radialgauge.GaugeFrame.BackFrameStyle != null)
                        {
                            Radialgauge.GaugeFrame.BackFrameStyle.BackgroundColor = this.GetEvalString(radialgauge.GaugeFrame.BackFrameStyle.BackgroundColor);
                            Radialgauge.GaugeFrame.BackFrameStyle.BackgroundGradientEndcolor = this.GetEvalString(radialgauge.GaugeFrame.BackFrameStyle.BackgroundGradientEndcolor);
                            if (radialgauge.GaugeFrame.BackFrameStyle.BackgroundGradientType != "Default")
                            {
                                Radialgauge.GaugeFrame.BackFrameStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(radialgauge.GaugeFrame.BackFrameStyle.BackgroundGradientType));
                            }
                            Radialgauge.GaugeFrame.BackFrameStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(radialgauge.GaugeFrame.BackFrameStyle.BackgroundHatchType));
                        }
                        if (radialgauge.GaugeFrame.FrameImage != null)
                        {
                            Radialgauge.GaugeFrame.FrameImage.MIMEType = this.GetEvalString(radialgauge.GaugeFrame.FrameImage.MIMEType);
                            Radialgauge.GaugeFrame.FrameImage.HueColor = this.GetEvalString(radialgauge.GaugeFrame.FrameImage.HueColor);
                            if (radialgauge.GaugeFrame.FrameImage.Source != null)
                            {
                                Radialgauge.GaugeFrame.FrameImage.Source = TryEnum<Source>(this.GetEvalString(radialgauge.GaugeFrame.FrameImage.Source));
                            }
                            Radialgauge.GaugeFrame.FrameImage.Transparency = this.GetEvalString(radialgauge.GaugeFrame.FrameImage.Transparency);
                            Radialgauge.GaugeFrame.FrameImage.TransparentColor = this.GetEvalString(radialgauge.GaugeFrame.FrameImage.TransparentColor);
                            Radialgauge.GaugeFrame.FrameImage.Value = this.GetEvalString(radialgauge.GaugeFrame.FrameImage.Value);
                            Radialgauge.GaugeFrame.FrameImage.ClipImage = radialgauge.GaugeFrame.FrameImage.ClipImage;
                        }
                        Radialgauge.GaugeFrame.FrameWidth = double.Parse(this.GetEvalString(radialgauge.GaugeFrame.FrameWidth));
                        Radialgauge.GaugeFrame.FrameGlassEffect = TryEnum<GlassEffect>(this.GetEvalString(radialgauge.GaugeFrame.FrameGlassEffect));
                        Radialgauge.GaugeFrame.FrameProperty.BackgroundColor = this.GetEvalString(radialgauge.GaugeFrame.FrameProperty.BackgroundColor);

                        Radialgauge.GaugeFrame.BackFrameStyle.OffSet = new DOM.Size(this.GetEvalString(radialgauge.GaugeFrame.BackFrameStyle.OffSet)).FloatValue;
                        Radialgauge.Hidden = radialgauge.Hidden;
                        if (radialgauge.GaugeTopImage != null)
                        {
                            Radialgauge.GaugeTopImage.HueColor = this.GetEvalString(radialgauge.GaugeTopImage.HueColor);
                            Radialgauge.GaugeTopImage.MIMEType = this.GetEvalString(radialgauge.GaugeTopImage.MIMEType);
                            if (radialgauge.GaugeTopImage.Source != null)
                            {
                                Radialgauge.GaugeTopImage.Source = TryEnum<Source>(this.GetEvalString(radialgauge.GaugeTopImage.Source));
                            }
                            Radialgauge.GaugeTopImage.TransparentColor = this.GetEvalString(radialgauge.GaugeTopImage.TransparentColor);
                            Radialgauge.GaugeTopImage.Value = this.GetEvalString(radialgauge.GaugeTopImage.Value);
                        }
                        Radialgauge.ClipContent = radialgauge.ClipContent;
                        Radialgauge.GaugeTooltip = this.GetEvalString(radialgauge.GaugeTooltip);
                        Radialgauge.AspectRatio = int.Parse(this.GetEvalString(radialgauge.AspectRatio));
                        Radialgauge.GaugeHeight = double.Parse(this.GetEvalString(radialgauge.GaugeHeight));
                        Radialgauge.GaugeWidth = double.Parse(this.GetEvalString(radialgauge.GaugeWidth));
                        Radialgauge.Left = double.Parse(this.GetEvalString(radialgauge.Left));
                        Radialgauge.Top = double.Parse(this.GetEvalString(radialgauge.Top));
                        if (radialgauge.Orientation != null)
                        {
                            Radialgauge.Orientation = TryEnum<Orientation>(this.GetEvalString(radialgauge.Orientation));
                        }
                        Radialgauge.GaugeWidth = double.Parse(this.GetEvalString(radialgauge.GaugeWidth));
                        Radialgauge.GaugeHeight = double.Parse(this.GetEvalString(radialgauge.GaugeHeight));

                        if (radialgauge.ActionInfo != null)
                        {
                            Radialgauge.ActionInfo.BookmarkLink = this.GetEvalString(radialgauge.ActionInfo.BookmarkLink);
                            Radialgauge.ActionInfo.Hyperlink = this.GetEvalString(radialgauge.ActionInfo.Hyperlink);
                            if (radialgauge.ActionInfo.ReportName != null)
                            {
                                Radialgauge.ActionInfo.ReportName = this.GetEvalString(radialgauge.ActionInfo.ReportName);
                            }

                            Radialgauge.ActionInfo.Parameters = new List<ParameterExpVal>();
                            if (radialgauge.ActionInfo.Parameters != null)
                            {
                                foreach (var Action in radialgauge.ActionInfo.Parameters)
                                {
                                    var parameter = new ParameterExpVal();
                                    parameter.Name = this.GetEvalString(Action.Name);
                                    parameter.Omit = this.GetEvalString(Action.Omit);
                                    parameter.Value = this.GetEvalString(Action.Value);
                                    Radialgauge.ActionInfo.Parameters.Add(parameter);
                                }
                            }
                        }

                        Radialgauge.GaugeScales = new List<GaugeScalePropertiesExpVal>();
                        if (radialgauge.GaugeScales != null)
                        {
                            foreach (var gaugescale in radialgauge.GaugeScales)
                            {
                                var Scale = new GaugeScalePropertiesExpVal();
                                Scale.ScaleStyle = new GaugeStyleExpVal();
                                Scale.ScaleLabel = new GaugeLabelExpVal();
                                Scale.ScaleLabel.LabelStyle = new GaugeStyleExpVal();
                                Scale.MajorTickMark = new TickMarksExpVal();
                                Scale.MajorTickMark.TickMarkStyle = new GaugeStyleExpVal();
                                Scale.MajorTickMark.TickMarkImage = new BaseImageExpVal();
                                Scale.MinorTickMark = new TickMarksExpVal();
                                Scale.MinorTickMark.TickMarkStyle = new GaugeStyleExpVal();
                                Scale.MinorTickMark.TickMarkImage = new BaseImageExpVal();
                                Scale.MaximumPin = new ScalePinExpVal();
                                Scale.MaximumPin.PinImage = new BaseImageExpVal();
                                Scale.MaximumPin.PinLabel = new GaugeLabelExpVal();
                                Scale.MaximumPin.PinLabel.LabelStyle = new GaugeStyleExpVal();
                                Scale.MinimumPin = new ScalePinExpVal();
                                Scale.MinimumPin.PinImage = new BaseImageExpVal();
                                Scale.MinimumPin.PinLabel = new GaugeLabelExpVal();
                                Scale.MinimumPin.PinLabel.LabelStyle = new GaugeStyleExpVal();
                                Scale.MaximumValue = new GaugeInputValueExpVal();
                                Scale.MinimumValue = new GaugeInputValueExpVal();
                                Scale.ActionInfo = new ActionInfoExpVal();

                                if (gaugescale.ScaleStyle != null)
                                {
                                    Scale.ScaleStyle.BorderColor = this.GetEvalString(gaugescale.ScaleStyle.BorderColor);
                                    if (gaugescale.ScaleStyle.BorderStyle != null)
                                    {
                                        Scale.ScaleStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(gaugescale.ScaleStyle.BorderStyle));
                                    }
                                    Scale.ScaleStyle.BorderWidth = new DOM.Size(this.GetEvalString(gaugescale.ScaleStyle.BorderWidth)).FloatValue;
                                    Scale.ScaleStyle.BackgroundColor = this.GetEvalString(gaugescale.ScaleStyle.BackgroundColor);
                                    Scale.ScaleStyle.BackgroundGradientEndcolor = this.GetEvalString(gaugescale.ScaleStyle.BackgroundGradientEndcolor);
                                    if (gaugescale.ScaleStyle.BackgroundGradientType != null)
                                    {
                                        Scale.ScaleStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(gaugescale.ScaleStyle.BackgroundGradientType));
                                    }
                                    if (gaugescale.ScaleStyle.BackgroundHatchType != null)
                                    {
                                        Scale.ScaleStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(gaugescale.ScaleStyle.BackgroundHatchType));
                                    }
                                }
                                Scale.Hidden = gaugescale.Hidden;
                                Scale.ScaleStyle.OffSet = new DOM.Size(this.GetEvalString(gaugescale.ScaleStyle.OffSet)).FloatValue;
                                Scale.LabelMultiplier = double.Parse(this.GetEvalString(gaugescale.LabelMultiplier));
                                Scale.LogBase = double.Parse(this.GetEvalString(gaugescale.LogBase));
                                Scale.LogrithmicScale = gaugescale.LogrithmicScale;
                                Scale.ReverseDirection = gaugescale.ReverseDirection;
                                Scale.StartMargin = new DOM.Size(this.GetEvalString(gaugescale.StartMargin)).FloatValue;
                                Scale.EndMargin = new DOM.Size(this.GetEvalString(gaugescale.EndMargin)).FloatValue;
                                if (gaugescale.Position != null)
                                {
                                    Scale.Position = TryEnum<Position>(this.GetEvalString(gaugescale.Position));
                                }
                                if (gaugescale.ScaleLabel != null)
                                {
                                    Scale.ScaleLabel.ScaleDistance = new DOM.Size(this.GetEvalString(gaugescale.ScaleLabel.ScaleDistance)).FloatValue;
                                    if (gaugescale.ScaleLabel.LabelStyle != null)
                                    {
                                        Scale.ScaleLabel.LabelStyle.FontFamily = this.GetEvalString(gaugescale.ScaleLabel.LabelStyle.FontFamily);
                                        Scale.ScaleLabel.LabelStyle.FontSize = new DOM.Size(this.GetEvalString(gaugescale.ScaleLabel.LabelStyle.FontSize)).FloatValue;
                                        Scale.ScaleLabel.LabelStyle.FontStyle = TryEnum<RDL.DOM.FontStyle>(this.GetEvalString(gaugescale.ScaleLabel.LabelStyle.FontStyle));
                                        Scale.ScaleLabel.LabelStyle.FontWeight = TryEnum<RDL.DOM.FontWeight>(this.GetEvalString(gaugescale.ScaleLabel.LabelStyle.FontWeight));
                                    }
                                    Scale.ScaleLabel.FontAngle = double.Parse(this.GetEvalString(gaugescale.ScaleLabel.FontAngle));
                                    Scale.ScaleLabel.FormatString = this.GetEvalString(gaugescale.ScaleLabel.FormatString);//formatstring
                                    Scale.ScaleLabel.Hidden = gaugescale.ScaleLabel.Hidden;
                                    Scale.ScaleLabel.LabelInterval = double.Parse(this.GetEvalString(gaugescale.ScaleLabel.LabelInterval));
                                    Scale.ScaleLabel.LabelIntervalOffset = double.Parse(this.GetEvalString(gaugescale.ScaleLabel.LabelIntervalOffset));
                                    Scale.ScaleLabel.ScalePlacment = TryEnum<Placement>(this.GetEvalString(gaugescale.ScaleLabel.ScalePlacment));
                                    Scale.ScaleLabel.RotateLabel = gaugescale.ScaleLabel.RotateLabel;
                                    Scale.ScaleLabel.EndLabel = gaugescale.ScaleLabel.ShowEndLabel;
                                    Scale.ScaleLabel.TextColor = this.GetEvalString(gaugescale.ScaleLabel.TextColor);//textcolor
                                    Scale.ScaleLabel.TextDecoration = TryEnum<RDL.DOM.TextDecoration>(this.GetEvalString(gaugescale.ScaleLabel.TextDecoration));
                                    Scale.ScaleLabel.UseFontPercent = gaugescale.ScaleLabel.UseFontPercent;
                                }

                                if (gaugescale.MajorTickMark.TickMarkStyle != null)
                                {
                                    Scale.MajorTickMark.TickMarkStyle.BorderColor = this.GetEvalString(gaugescale.MajorTickMark.TickMarkStyle.BorderColor);
                                    if (gaugescale.MajorTickMark.TickMarkStyle.BorderStyle != null)
                                    {
                                        Scale.MajorTickMark.TickMarkStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(gaugescale.MajorTickMark.TickMarkStyle.BorderStyle));
                                    }
                                    Scale.MajorTickMark.TickMarkStyle.BorderWidth = new DOM.Size(this.GetEvalString(gaugescale.MajorTickMark.TickMarkStyle.BorderWidth)).FloatValue;
                                }
                                Scale.MajorTickMark.DistanceFromScale = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.DistanceFromScale));
                                Scale.MajorTickMark.EnableGradient = gaugescale.MajorTickMark.EnableGradient;
                                Scale.MajorTickMark.FillColor = this.GetEvalString(gaugescale.MajorTickMark.FillColor);//fillcolor
                                Scale.MajorTickMark.GradientDensity = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.GradientDensity));
                                Scale.MajorTickMark.HideTickMark = gaugescale.MajorTickMark.HideTickMark;
                                Scale.MajorTickMark.Interval = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.Interval));
                                Scale.MajorTickMark.IntervalOffset = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.IntervalOffset));
                                Scale.MajorTickMark.Length = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.Length));
                                Scale.MajorTickMark.TickMarkPlacement = TryEnum<Placement>(this.GetEvalString(gaugescale.MajorTickMark.TickMarkPlacement));
                                Scale.MajorTickMark.TickMarkShape = this.GetEvalString(gaugescale.MajorTickMark.TickMarkShape);
                                if (gaugescale.MajorTickMark.TickMarkImage != null)
                                {
                                    Scale.MajorTickMark.TickMarkImage.MIMEType = this.GetEvalString(gaugescale.MajorTickMark.TickMarkImage.MIMEType);
                                    if (gaugescale.MajorTickMark.TickMarkImage.Source != null)
                                    {
                                        Scale.MajorTickMark.TickMarkImage.Source = TryEnum<Source>(this.GetEvalString(gaugescale.MajorTickMark.TickMarkImage.Source));
                                    }
                                    Scale.MajorTickMark.TickMarkImage.TransparentColor = this.GetEvalString(gaugescale.MajorTickMark.TickMarkImage.TransparentColor);
                                    Scale.MajorTickMark.TickMarkImage.Value = this.GetEvalString(gaugescale.MajorTickMark.TickMarkImage.Value);
                                    Scale.MajorTickMark.TickMarkImage.HueColor = this.GetEvalString(gaugescale.MajorTickMark.TickMarkImage.HueColor);
                                }
                                Scale.MajorTickMark.Width = double.Parse(this.GetEvalString(gaugescale.MajorTickMark.Width));

                                if (gaugescale.MinorTickMark.TickMarkStyle != null)
                                {
                                    Scale.MinorTickMark.TickMarkStyle.BorderColor = this.GetEvalString(gaugescale.MinorTickMark.TickMarkStyle.BorderColor);
                                    if (gaugescale.MinorTickMark.TickMarkStyle.BorderStyle != null)
                                    {
                                        Scale.MinorTickMark.TickMarkStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(gaugescale.MinorTickMark.TickMarkStyle.BorderStyle));
                                    }
                                    Scale.MinorTickMark.TickMarkStyle.BorderWidth = new DOM.Size(this.GetEvalString(gaugescale.MinorTickMark.TickMarkStyle.BorderWidth)).FloatValue;
                                }
                                Scale.MinorTickMark.DistanceFromScale = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.DistanceFromScale));
                                Scale.MinorTickMark.EnableGradient = gaugescale.MinorTickMark.EnableGradient;
                                Scale.MinorTickMark.FillColor = this.GetEvalString(gaugescale.MinorTickMark.FillColor);//fillcolor
                                Scale.MinorTickMark.GradientDensity = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.GradientDensity));
                                Scale.MinorTickMark.HideTickMark = gaugescale.MinorTickMark.HideTickMark;
                                Scale.MinorTickMark.Interval = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.Interval));
                                Scale.MinorTickMark.IntervalOffset = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.IntervalOffset));
                                Scale.MinorTickMark.Length = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.Length));
                                Scale.MinorTickMark.TickMarkPlacement = TryEnum<Placement>(this.GetEvalString(gaugescale.MinorTickMark.TickMarkPlacement));
                                Scale.MinorTickMark.TickMarkShape = this.GetEvalString(gaugescale.MinorTickMark.TickMarkShape);
                                if (gaugescale.MinorTickMark.TickMarkImage != null)
                                {
                                    Scale.MinorTickMark.TickMarkImage.MIMEType = this.GetEvalString(gaugescale.MinorTickMark.TickMarkImage.MIMEType);
                                    if (gaugescale.MinorTickMark.TickMarkImage.Source != null)
                                    {
                                        Scale.MinorTickMark.TickMarkImage.Source = TryEnum<Source>(this.GetEvalString(gaugescale.MinorTickMark.TickMarkImage.Source));
                                    }
                                    Scale.MinorTickMark.TickMarkImage.TransparentColor = this.GetEvalString(gaugescale.MinorTickMark.TickMarkImage.TransparentColor);
                                    Scale.MinorTickMark.TickMarkImage.Value = this.GetEvalString(gaugescale.MinorTickMark.TickMarkImage.Value);
                                    Scale.MinorTickMark.TickMarkImage.HueColor = this.GetEvalString(gaugescale.MinorTickMark.TickMarkImage.HueColor);
                                }
                                Scale.MinorTickMark.Width = double.Parse(this.GetEvalString(gaugescale.MinorTickMark.Width));

                                Scale.ScaleInterval = double.Parse(this.GetEvalString(gaugescale.ScaleInterval));
                                Scale.ScaleIntervaloffset = double.Parse(this.GetEvalString(gaugescale.ScaleIntervaloffset));

                                Scale.MaximumPin.Location = this.GetEvalString(gaugescale.MaximumPin.Location);
                                Scale.MaximumPin.Enable = gaugescale.MaximumPin.Enable;
                                if (gaugescale.MaximumPin.PinLabel != null)
                                {
                                    Scale.MaximumPin.PinLabel.AllowUpsideDown = gaugescale.MaximumPin.PinLabel.AllowUpsideDown;
                                    Scale.MaximumPin.PinLabel.ScaleDistance = new DOM.Size(this.GetEvalString(gaugescale.MaximumPin.PinLabel.ScaleDistance)).FloatValue;
                                    if (gaugescale.MaximumPin.PinLabel.LabelStyle != null)
                                    {
                                        Scale.MaximumPin.PinLabel.LabelStyle.FontSize = new DOM.Size(this.GetEvalString(gaugescale.MaximumPin.PinLabel.LabelStyle.FontSize)).FloatValue;
                                        if (gaugescale.MaximumPin.PinLabel.LabelStyle.FontStyle != null)
                                        {
                                            Scale.MaximumPin.PinLabel.LabelStyle.FontStyle = TryEnum<RDL.DOM.FontStyle>(this.GetEvalString(gaugescale.MaximumPin.PinLabel.LabelStyle.FontStyle));
                                        }
                                        if (gaugescale.MaximumPin.PinLabel.LabelStyle.FontWeight != null)
                                        {
                                            Scale.MaximumPin.PinLabel.LabelStyle.FontWeight = TryEnum<RDL.DOM.FontWeight>(this.GetEvalString(gaugescale.MaximumPin.PinLabel.LabelStyle.FontWeight));
                                        }
                                        Scale.MaximumPin.PinLabel.LabelStyle.FontFamily = this.GetEvalString(gaugescale.MaximumPin.PinLabel.LabelStyle.FontFamily);
                                    }
                                    if (gaugescale.MaximumPin.PinLabel.FontAngle != null)
                                    {
                                        Scale.MaximumPin.PinLabel.FontAngle = double.Parse(this.GetEvalString(gaugescale.MaximumPin.PinLabel.FontAngle));
                                    }
                                    if (gaugescale.MaximumPin.PinLabel.ScalePlacment != null)
                                    {
                                        Scale.MaximumPin.PinLabel.ScalePlacment = TryEnum<Placement>(this.GetEvalString(gaugescale.MaximumPin.PinLabel.ScalePlacment));
                                    }

                                    Scale.MaximumPin.PinLabel.UseFontPercent = gaugescale.MaximumPin.PinLabel.UseFontPercent;

                                    if (gaugescale.MaximumPin.PinLabel.Text != null)
                                    {
                                        Scale.MaximumPin.PinLabel.Text = this.GetEvalString(gaugescale.MaximumPin.PinLabel.Text);
                                    }
                                    Scale.MaximumPin.PinLabel.RotateLabel = gaugescale.MaximumPin.PinLabel.RotateLabel;
                                    if (gaugescale.MaximumPin.PinLabel.TextDecoration != null)
                                    {
                                        Scale.MaximumPin.PinLabel.TextDecoration = TryEnum<RDL.DOM.TextDecoration>(this.GetEvalString(gaugescale.MaximumPin.PinLabel.TextDecoration));
                                    }
                                    if (gaugescale.MaximumPin.PinLabel.TextColor != null)
                                    {
                                        Scale.MaximumPin.PinLabel.TextColor = this.GetEvalString(gaugescale.MaximumPin.PinLabel.TextColor);
                                    }
                                }


                                Scale.MinimumPin.Location = this.GetEvalString(gaugescale.MinimumPin.Location);
                                Scale.MinimumPin.Enable = gaugescale.MinimumPin.Enable;
                                if (gaugescale.MinimumPin.PinLabel != null)
                                {
                                    Scale.MinimumPin.PinLabel.AllowUpsideDown = gaugescale.MinimumPin.PinLabel.AllowUpsideDown;
                                    Scale.MinimumPin.PinLabel.ScaleDistance = new DOM.Size(this.GetEvalString(gaugescale.MinimumPin.PinLabel.ScaleDistance)).FloatValue;
                                    if (gaugescale.MinimumPin.PinLabel.LabelStyle != null)
                                    {
                                        Scale.MinimumPin.PinLabel.LabelStyle.FontSize = new DOM.Size(this.GetEvalString(gaugescale.MinimumPin.PinLabel.LabelStyle.FontSize)).FloatValue;
                                        if (gaugescale.MinimumPin.PinLabel.LabelStyle.FontStyle != null)
                                        {
                                            Scale.MinimumPin.PinLabel.LabelStyle.FontStyle = TryEnum<RDL.DOM.FontStyle>(this.GetEvalString(gaugescale.MinimumPin.PinLabel.LabelStyle.FontStyle));
                                        }
                                        if (gaugescale.MinimumPin.PinLabel.LabelStyle.FontWeight != null)
                                        {
                                            Scale.MinimumPin.PinLabel.LabelStyle.FontWeight = TryEnum<RDL.DOM.FontWeight>(this.GetEvalString(gaugescale.MinimumPin.PinLabel.LabelStyle.FontWeight));
                                        }
                                        Scale.MinimumPin.PinLabel.LabelStyle.FontFamily = this.GetEvalString(gaugescale.MinimumPin.PinLabel.LabelStyle.FontFamily);
                                    }
                                    if (gaugescale.MinimumPin.PinLabel.FontAngle != null)
                                    {
                                        Scale.MinimumPin.PinLabel.FontAngle = double.Parse(this.GetEvalString(gaugescale.MinimumPin.PinLabel.FontAngle));
                                    }
                                    if (gaugescale.MinimumPin.PinLabel.ScalePlacment != null)
                                    {
                                        Scale.MinimumPin.PinLabel.ScalePlacment = TryEnum<Placement>(this.GetEvalString(gaugescale.MinimumPin.PinLabel.ScalePlacment));
                                    }

                                    Scale.MinimumPin.PinLabel.UseFontPercent = gaugescale.MinimumPin.PinLabel.UseFontPercent;

                                    if (gaugescale.MinimumPin.PinLabel.Text != null)
                                    {
                                        Scale.MinimumPin.PinLabel.Text = this.GetEvalString(gaugescale.MinimumPin.PinLabel.Text);
                                    }

                                    Scale.MinimumPin.PinLabel.RotateLabel = gaugescale.MinimumPin.PinLabel.RotateLabel;

                                    if (gaugescale.MinimumPin.PinLabel.TextDecoration != null)
                                    {
                                        Scale.MinimumPin.PinLabel.TextDecoration = TryEnum<RDL.DOM.TextDecoration>(this.GetEvalString(gaugescale.MinimumPin.PinLabel.TextDecoration));
                                    }
                                    if (gaugescale.MinimumPin.PinLabel.TextColor != null)
                                    {
                                        Scale.MinimumPin.PinLabel.TextColor = this.GetEvalString(gaugescale.MinimumPin.PinLabel.TextColor);
                                    }
                                }

                                Scale.MaximumValue.AddConstant = this.GetEvalString(gaugescale.MaximumValue.AddConstant);
                                Scale.MaximumValue.DataElementName = this.GetEvalString(gaugescale.MaximumValue.DataElementName);
                                Scale.MaximumValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(gaugescale.MaximumValue.DataElementOutput));
                                Scale.MaximumValue.Formula = TryEnum<Formula>(this.GetEvalString(gaugescale.MaximumValue.Formula));
                                Scale.MaximumValue.MaxPercent = double.Parse(this.GetEvalString(gaugescale.MaximumValue.MaxPercent));
                                Scale.MaximumValue.MinPercent = double.Parse(this.GetEvalString(gaugescale.MaximumValue.MinPercent));
                                Scale.MaximumValue.Multiplier = double.Parse(this.GetEvalString(gaugescale.MaximumValue.Multiplier));
                                Scale.MaximumValue.Value = double.Parse(this.GetEvalString(gaugescale.MaximumValue.Value));

                                Scale.MinimumValue.AddConstant = this.GetEvalString(gaugescale.MinimumValue.AddConstant);
                                Scale.MinimumValue.DataElementName = this.GetEvalString(gaugescale.MinimumValue.DataElementName);
                                Scale.MinimumValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(gaugescale.MinimumValue.DataElementOutput));
                                Scale.MinimumValue.Formula = TryEnum<Formula>(this.GetEvalString(gaugescale.MinimumValue.Formula));
                                Scale.MinimumValue.MaxPercent = double.Parse(this.GetEvalString(gaugescale.MinimumValue.MaxPercent));
                                Scale.MinimumValue.MinPercent = double.Parse(this.GetEvalString(gaugescale.MinimumValue.MinPercent));
                                Scale.MinimumValue.Multiplier = double.Parse(this.GetEvalString(gaugescale.MinimumValue.Multiplier));
                                Scale.MinimumValue.Value = double.Parse(this.GetEvalString(gaugescale.MinimumValue.Value));

                                Scale.TickMarksonTop = gaugescale.TickMarksonTop;
                                Scale.ToolTip = this.GetEvalString(gaugescale.ToolTip);
                                Scale.ScaleWidth = double.Parse(this.GetEvalString(gaugescale.ScaleWidth));
                                Scale.StartAngle = double.Parse(this.GetEvalString(gaugescale.StartAngle));
                                Scale.SweepAngle = double.Parse(this.GetEvalString(gaugescale.SweepAngle));
                                Scale.Radius = double.Parse(this.GetEvalString(gaugescale.Radius));

                                if (gaugescale.ActionInfo != null)
                                {
                                    Scale.ActionInfo.BookmarkLink = this.GetEvalString(gaugescale.ActionInfo.BookmarkLink);
                                    Scale.ActionInfo.Hyperlink = this.GetEvalString(gaugescale.ActionInfo.Hyperlink);
                                    if (gaugescale.ActionInfo.ReportName != null)
                                    {
                                        Scale.ActionInfo.ReportName = this.GetEvalString(gaugescale.ActionInfo.ReportName);
                                    }

                                    Scale.ActionInfo.Parameters = new List<ParameterExpVal>();
                                    if (gaugescale.ActionInfo.Parameters != null)
                                    {
                                        foreach (var Action in gaugescale.ActionInfo.Parameters)
                                        {
                                            var parameter = new ParameterExpVal();
                                            parameter.Name = this.GetEvalString(Action.Name);
                                            parameter.Omit = this.GetEvalString(Action.Omit);
                                            parameter.Value = this.GetEvalString(Action.Value);
                                            Scale.ActionInfo.Parameters.Add(parameter);
                                        }
                                    }
                                }

                                Scale.ScalePointer = new List<PointerExpVal>();
                                if (gaugescale.ScalePointer != null)
                                {
                                    foreach (var gaugepointer in gaugescale.ScalePointer)
                                    {
                                        var Pointer = new PointerExpVal();
                                        Pointer.PointerImage = new PointerImageExpVal();
                                        Pointer.PointerStyle = new GaugeStyleExpVal();
                                        Pointer.PointerImage.PointerImage = new BaseImageExpVal();
                                        Pointer.RadialPointer = new RadialPointerExpVal();
                                        Pointer.Value = new GaugeInputValueExpVal();
                                        Pointer.RadialPointer.CapProperties = new PointerCapExpVal();
                                        Pointer.RadialPointer.CapProperties.CapImage = new BaseImageExpVal();
                                        Pointer.RadialPointer.CapProperties.CapStyle = new GaugeStyleExpVal();
                                        Pointer.ActionInfo = new ActionInfoExpVal();
                                        if (gaugepointer.PointerStyle != null)
                                        {
                                            Pointer.PointerStyle.BackgroundColor = this.GetEvalString(gaugepointer.PointerStyle.BackgroundColor);
                                            Pointer.PointerStyle.BackgroundGradientEndcolor = this.GetEvalString(gaugepointer.PointerStyle.BackgroundGradientEndcolor);
                                            if (gaugepointer.PointerStyle.BackgroundGradientType != null && gaugepointer.PointerStyle.BackgroundGradientType != "Default")
                                            {
                                                Pointer.PointerStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(gaugepointer.PointerStyle.BackgroundGradientType));
                                            }
                                            Pointer.PointerStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(gaugepointer.PointerStyle.BackgroundHatchType));
                                            Pointer.PointerStyle.BorderColor = this.GetEvalString(gaugepointer.PointerStyle.BorderColor);
                                            if (gaugepointer.PointerStyle.BorderStyle != null)
                                            {
                                                Pointer.PointerStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(gaugepointer.PointerStyle.BorderStyle));
                                            }
                                            Pointer.PointerStyle.BorderWidth = new DOM.Size(this.GetEvalString(gaugepointer.PointerStyle.BorderWidth)).FloatValue;
                                        }
                                        Pointer.Hidden = gaugepointer.Hidden;
                                        if (gaugepointer.PointerImage != null)
                                        {
                                            Pointer.PointerImage.PivotX = new DOM.Size(this.GetEvalString(gaugepointer.PointerImage.PivotX)).FloatValue;
                                            Pointer.PointerImage.PivotY = new DOM.Size(this.GetEvalString(gaugepointer.PointerImage.PivotY)).FloatValue;
                                            Pointer.PointerImage.PointerImage.HueColor = this.GetEvalString(gaugepointer.PointerImage.PointerImage.HueColor);
                                            Pointer.PointerImage.PointerImage.MIMEType = this.GetEvalString(gaugepointer.PointerImage.PointerImage.MIMEType);
                                            if (gaugepointer.PointerImage.PointerImage.Source != null)
                                            {
                                                Pointer.PointerImage.PointerImage.Source = TryEnum<Source>(this.GetEvalString(gaugepointer.PointerImage.PointerImage.Source));
                                            }
                                            Pointer.PointerImage.PointerImage.Transparency = this.GetEvalString(gaugepointer.PointerImage.PointerImage.Transparency);
                                            Pointer.PointerImage.PointerImage.TransparentColor = this.GetEvalString(gaugepointer.PointerImage.PointerImage.TransparentColor);
                                            Pointer.PointerImage.PointerImage.Value = this.GetEvalString(gaugepointer.PointerImage.PointerImage.Value);
                                        }
                                        Pointer.PointerStyle.OffSet = new DOM.Size(this.GetEvalString(gaugepointer.PointerStyle.OffSet)).FloatValue;
                                        Pointer.SnappingInterval = double.Parse(this.GetEvalString(gaugepointer.SnappingInterval));
                                        Pointer.SnappingEnabled = gaugepointer.SnappingEnabled;
                                        Pointer.PointerTooltip = this.GetEvalString(gaugepointer.PointerTooltip);
                                        Pointer.ScaleDistance = double.Parse(this.GetEvalString(gaugepointer.ScaleDistance));
                                        Pointer.Placement = TryEnum<Placement>(this.GetEvalString(gaugepointer.Placement));
                                        Pointer.PointerWidth = double.Parse(this.GetEvalString(gaugepointer.PointerWidth));
                                        if (gaugepointer.LinearPointer != null)
                                        {
                                            Pointer.LinearPointer.PointerType = TryEnum<LinearPointerType>(this.GetEvalString(gaugepointer.LinearPointer.PointerType));
                                        }
                                        Pointer.BarStart = TryEnum<BarStart>(this.GetEvalString(gaugepointer.BarStart));
                                        Pointer.MarkerLength = double.Parse(this.GetEvalString(gaugepointer.MarkerLength));

                                        if (gaugepointer.MarkerStyle != null)
                                        {
                                            Pointer.MarkerStyle = TryEnum<MarkerStyle>(this.GetEvalString(gaugepointer.MarkerStyle));
                                        }
                                        Pointer.Value.AddConstant = this.GetEvalString(gaugepointer.Value.AddConstant);
                                        Pointer.Value.DataElementName = this.GetEvalString(gaugepointer.Value.DataElementName);
                                        Pointer.Value.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(gaugepointer.Value.DataElementOutput));
                                        Pointer.Value.Formula = TryEnum<Formula>(this.GetEvalString(gaugepointer.Value.Formula));
                                        Pointer.Value.MaxPercent = double.Parse(this.GetEvalString(gaugepointer.Value.MaxPercent));
                                        Pointer.Value.MinPercent = double.Parse(this.GetEvalString(gaugepointer.Value.MinPercent));
                                        Pointer.Value.Multiplier = double.Parse(this.GetEvalString(gaugepointer.Value.Multiplier));
                                        Pointer.Value.Value = double.Parse(this.GetEvalString(gaugepointer.Value.Value));

                                        Pointer.RadialPointer.CapProperties.PointerCapStyle = TryEnum<CapStyle>(this.GetEvalString(gaugepointer.RadialPointer.CapProperties.PointerCapStyle));
                                        if (gaugepointer.RadialPointer.CapProperties.CapImage != null)
                                        {
                                            Pointer.RadialPointer.CapProperties.CapImage.HueColor = this.GetEvalString(gaugepointer.RadialPointer.CapProperties.CapImage.HueColor);
                                            Pointer.RadialPointer.CapProperties.CapImage.MIMEType = this.GetEvalString(gaugepointer.RadialPointer.CapProperties.CapImage.MIMEType);
                                            if (gaugepointer.RadialPointer.CapProperties.CapImage.Source != null)
                                            {
                                                Pointer.RadialPointer.CapProperties.CapImage.Source = TryEnum<Source>(this.GetEvalString(gaugepointer.RadialPointer.CapProperties.CapImage.Source));
                                            }
                                            Pointer.RadialPointer.CapProperties.CapImage.TransparentColor = this.GetEvalString(gaugepointer.RadialPointer.CapProperties.CapImage.TransparentColor);
                                            Pointer.RadialPointer.CapProperties.CapImage.Value = this.GetEvalString(gaugepointer.RadialPointer.CapProperties.CapImage.Value);
                                        }
                                        Pointer.RadialPointer.CapProperties.OffsetX = new DOM.Size(this.GetEvalString(gaugepointer.RadialPointer.CapProperties.OffsetX)).FloatValue;
                                        Pointer.RadialPointer.CapProperties.OffsetY = new DOM.Size(this.GetEvalString(gaugepointer.RadialPointer.CapProperties.OffsetY)).FloatValue;
                                        if (gaugepointer.RadialPointer.CapProperties.CapStyle != null)
                                        {
                                            Pointer.RadialPointer.CapProperties.CapStyle.BackgroundColor = this.GetEvalString(gaugepointer.RadialPointer.CapProperties.CapStyle.BackgroundColor);
                                            Pointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientEndcolor = this.GetEvalString(gaugepointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientEndcolor);
                                            if (gaugepointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientType != null && gaugepointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientType != "Default")
                                            {
                                                Pointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(gaugepointer.RadialPointer.CapProperties.CapStyle.BackgroundGradientType));
                                            }
                                            Pointer.RadialPointer.CapProperties.CapStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(gaugepointer.RadialPointer.CapProperties.CapStyle.BackgroundHatchType));
                                        }
                                        Pointer.RadialPointer.CapProperties.Hidden = gaugepointer.RadialPointer.CapProperties.Hidden;
                                        Pointer.RadialPointer.CapProperties.OnTop = gaugepointer.RadialPointer.CapProperties.OnTop;
                                        Pointer.RadialPointer.CapProperties.Reflection = gaugepointer.RadialPointer.CapProperties.Reflection;
                                        Pointer.RadialPointer.CapProperties.PointerCapWidth = double.Parse(this.GetEvalString(gaugepointer.RadialPointer.CapProperties.PointerCapWidth));

                                        Pointer.RadialPointer.NeedleStyle = TryEnum<NeedleStyleGauge>(this.GetEvalString(gaugepointer.RadialPointer.NeedleStyle));
                                        Pointer.RadialPointer.MarkerStyle = TryEnum<MarkerStyle>(this.GetEvalString(gaugepointer.RadialPointer.MarkerStyle));
                                        Pointer.RadialPointer.PointerType = TryEnum<DOM.RadialPointerType>(this.GetEvalString(gaugepointer.RadialPointer.PointerType));

                                        if (gaugepointer.ActionInfo != null)
                                        {
                                            Pointer.ActionInfo.BookmarkLink = this.GetEvalString(gaugepointer.ActionInfo.BookmarkLink);
                                            Pointer.ActionInfo.Hyperlink = this.GetEvalString(gaugepointer.ActionInfo.Hyperlink);
                                            if (gaugepointer.ActionInfo.ReportName != null)
                                            {
                                                Pointer.ActionInfo.ReportName = this.GetEvalString(gaugepointer.ActionInfo.ReportName);
                                            }

                                            Pointer.ActionInfo.Parameters = new List<ParameterExpVal>();
                                            if (gaugepointer.ActionInfo.Parameters != null)
                                            {
                                                foreach (var Action in gaugepointer.ActionInfo.Parameters)
                                                {
                                                    var parameter = new ParameterExpVal();
                                                    parameter.Name = this.GetEvalString(Action.Name);
                                                    parameter.Omit = this.GetEvalString(Action.Omit);
                                                    parameter.Value = this.GetEvalString(Action.Value);
                                                    Pointer.ActionInfo.Parameters.Add(parameter);
                                                }
                                            }
                                        }
                                        Scale.ScalePointer.Add(Pointer);
                                    }
                                }

                                Scale.ScaleRange = new List<ScaleRangeExpVal>();
                                if (gaugescale.ScaleRange != null)
                                {
                                    foreach (var scalerange in gaugescale.ScaleRange)
                                    {
                                        var Range = new ScaleRangeExpVal();
                                        Range.RangeStyle = new GaugeStyleExpVal();
                                        Range.StartValue = new GaugeInputValueExpVal();
                                        Range.EndValue = new GaugeInputValueExpVal();
                                        if (scalerange.RangeStyle != null)
                                        {
                                            Range.RangeStyle.BackgroundColor = this.GetEvalString(scalerange.RangeStyle.BackgroundColor);
                                            Range.RangeStyle.BackgroundGradientEndcolor = this.GetEvalString(scalerange.RangeStyle.BackgroundGradientEndcolor);
                                            if (scalerange.RangeStyle.BackgroundGradientType != null && scalerange.RangeStyle.BackgroundGradientType != "Default")
                                            {
                                                Range.RangeStyle.BackgroundGradientType = TryEnum<BackgroundGradientType>(this.GetEvalString(scalerange.RangeStyle.BackgroundGradientType));
                                            }
                                            Range.RangeStyle.BackgroundHatchType = TryEnum<BackgroundHatchTypes>(this.GetEvalString(scalerange.RangeStyle.BackgroundHatchType));
                                            Range.RangeStyle.BorderColor = this.GetEvalString(scalerange.RangeStyle.BorderColor);
                                            if (scalerange.RangeStyle.BorderStyle != null)
                                            {
                                                Range.RangeStyle.BorderStyle = TryEnum<BorderStyles>(this.GetEvalString(scalerange.RangeStyle.BorderStyle));
                                            }
                                            Range.RangeStyle.BorderWidth = new DOM.Size(this.GetEvalString(scalerange.RangeStyle.BorderWidth)).FloatValue;
                                        }
                                        Range.Hidden = scalerange.Hidden;
                                        Range.RangeStyle.OffSet = new DOM.Size(this.GetEvalString(scalerange.RangeStyle.OffSet)).FloatValue;
                                        Range.InRangeBarColor = this.GetEvalString(scalerange.InRangeBarColor);
                                        Range.InRangeLabelColor = this.GetEvalString(scalerange.InRangeLabelColor);
                                        Range.InRangeTickmarkColor = this.GetEvalString(scalerange.InRangeTickmarkColor);
                                        Range.ToolTip = this.GetEvalString(scalerange.ToolTip);
                                        Range.EndWidth = double.Parse(this.GetEvalString(scalerange.EndWidth));
                                        Range.RangePlacement = TryEnum<Placement>(this.GetEvalString(scalerange.RangePlacement));
                                        Range.ScaleDistance = double.Parse(this.GetEvalString(scalerange.ScaleDistance));
                                        Range.StartWidth = double.Parse(this.GetEvalString(scalerange.StartWidth));

                                        Range.EndValue.AddConstant = this.GetEvalString(scalerange.EndValue.AddConstant);
                                        Range.EndValue.DataElementName = this.GetEvalString(scalerange.EndValue.DataElementName);
                                        Range.EndValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(scalerange.EndValue.DataElementOutput));
                                        Range.EndValue.Formula = TryEnum<Formula>(this.GetEvalString(scalerange.EndValue.Formula));
                                        Range.EndValue.MaxPercent = double.Parse(this.GetEvalString(scalerange.EndValue.MaxPercent));
                                        Range.EndValue.MinPercent = double.Parse(this.GetEvalString(scalerange.EndValue.MinPercent));
                                        Range.EndValue.Multiplier = double.Parse(this.GetEvalString(scalerange.EndValue.Multiplier));
                                        Range.EndValue.Value = new DOM.Size(this.GetEvalString(scalerange.EndValue.Value)).FloatValue;

                                        Range.StartValue.AddConstant = this.GetEvalString(scalerange.StartValue.AddConstant);
                                        Range.StartValue.DataElementName = this.GetEvalString(scalerange.StartValue.DataElementName);
                                        Range.StartValue.DataElementOutput = TryEnum<DataElementOutputs>(this.GetEvalString(scalerange.StartValue.DataElementOutput));
                                        Range.StartValue.Formula = TryEnum<Formula>(this.GetEvalString(scalerange.StartValue.Formula));
                                        Range.StartValue.MaxPercent = double.Parse(this.GetEvalString(scalerange.StartValue.MaxPercent));
                                        Range.StartValue.MinPercent = double.Parse(this.GetEvalString(scalerange.StartValue.MinPercent));
                                        Range.StartValue.Multiplier = double.Parse(this.GetEvalString(scalerange.StartValue.Multiplier));
                                        Range.StartValue.Value = new DOM.Size(this.GetEvalString(scalerange.StartValue.Value)).FloatValue;

                                        if (scalerange.ActionInfo != null)
                                        {
                                            Range.ActionInfo.BookmarkLink = this.GetEvalString(scalerange.ActionInfo.BookmarkLink);
                                            Range.ActionInfo.Hyperlink = this.GetEvalString(scalerange.ActionInfo.Hyperlink);
                                            Range.ActionInfo.ReportName = this.GetEvalString(scalerange.ActionInfo.ReportName);

                                            Range.ActionInfo.Parameters = new List<ParameterExpVal>();
                                            foreach (var Action in scalerange.ActionInfo.Parameters)
                                            {
                                                var parameter = new ParameterExpVal();
                                                parameter.Name = this.GetEvalString(Action.Name);
                                                parameter.Omit = this.GetEvalString(Action.Omit);
                                                parameter.Value = this.GetEvalString(Action.Value);
                                                Range.ActionInfo.Parameters.Add(parameter);
                                            }
                                        }

                                        Scale.ScaleRange.Add(Range);
                                    }
                                }

                                Radialgauge.GaugeScales.Add(Scale);
                            }
                        }

                        this.GaugePanelProperties.RadialGauges.Add(Radialgauge);
                    }
                }

                this.Engine.DisposeEngine();
                this.Engine = null;
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
            node.ModelType = ModelType.GaugeModel;
            node.ReportItemName = this.Name;
            node.IsTablixChild = this.IsTablixChild;
            this.DocumentNodeRefer = node;
            this.Model.MapModel.NodeData.Add(node);
        }

        public override void DisposeEvalObjects()
        {
            this.GaugePanelProperties = null;
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