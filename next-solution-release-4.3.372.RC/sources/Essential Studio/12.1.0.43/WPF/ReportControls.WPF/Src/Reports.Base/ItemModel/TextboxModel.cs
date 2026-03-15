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
    internal class TextboxModel
        : ReportItemModeler
    {
        #region members

        ReportItemExp textBoxExpProp;
        List<ParagraphExp> paraExp;

        List<double> paraHeights;

        #endregion

        #region  properties

        public double ActualHeight
        {
            get;
            set;
        }

        public List<ParagraphExpval> ParaExpval
        {
            get;
            set;
        }

        public ReportItemExpval TextBoxProperties
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

        internal List<string> ToggleInfos
        {
            get; 
            set;
        }

        internal bool IsToggled
        {
            get; 
            set;
        }

        internal bool IsFirstToggle
        {
            get; 
            set;
        }

        internal List<ToggleGropInfo> ToggleGroups
        {
            get; 
            set;
        }

        internal bool IsDrillAction
        {
            get; 
            set;
        }

        #endregion

        #region Constructors

        internal TextboxModel()
        {
        }

        internal TextboxModel(ReportItem reportItem, ReportModel pageModel, bool isTablixChild,string dataSetName)
        {
            this.DataSetName = dataSetName;
            this.IsTablixChild = isTablixChild;
            this.ModelType = ModelType.TextBoxModel;
            this.Model = pageModel;
            this.ReportItem = reportItem;
            this.Name = reportItem.Name;
            this.paraHeights = new List<double>();

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

            this.ParseTextBox();
        }

        #endregion

        #region Parsing ReportItem

        private string GetRunExpressionKey(string value,string text)
        {
            if (!string.IsNullOrEmpty(text) && value !=null && value.ToLower().Contains("me.value"))
            {
                if (text.StartsWith("="))
                {
                    value = Regex.Replace(value, "me.value", text.Substring(1), RegexOptions.IgnoreCase);
                }

                else
                {
                    value = Regex.Replace(value, "me.value", text, RegexOptions.IgnoreCase);
                }
            }

            if (string.IsNullOrEmpty(text))
            {
                value = null;
            }

            string key = this.Model.ExpressionEngine.GetExpressionKey(value, this.DataSetName, this.IsTablixChild);
            this.AddDataFields(key);
            return key;
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

        void ParseTextBox()
        {
                TextBox txtBox = this.ReportItem as TextBox;
                this.CanGrow = txtBox.CanGrow;
                this.Intialize(txtBox);
                this.textBoxExpProp = new ReportItemExp();

                if (txtBox.Style != null)
                {
                    RDL.DOM.Style style = txtBox.Style;
                    this.textBoxExpProp.Padding = new ThicknessExp();

                    this.textBoxExpProp.Padding.TopSize = style.PaddingTop;
                    this.textBoxExpProp.Padding.BottomSize = style.PaddingBottom;
                    this.textBoxExpProp.Padding.LeftSize = style.PaddingLeft;
                    this.textBoxExpProp.Padding.RightSize = style.PaddingRight;

                    if (style.PaddingTop != null && style.PaddingTop.IsExpression)
                    {
                        this.textBoxExpProp.Padding.Top = this.GetExpressionKey(style.PaddingTop.size);
                    }
                    if (style.PaddingLeft != null && style.PaddingLeft.IsExpression)
                    {
                        this.textBoxExpProp.Padding.Left = this.GetExpressionKey(style.PaddingLeft.size);
                    }
                    if (style.PaddingBottom != null && style.PaddingBottom.IsExpression)
                    {
                        this.textBoxExpProp.Padding.Bottom = this.GetExpressionKey(style.PaddingBottom.size);
                    }
                    if (style.PaddingRight != null && style.PaddingRight.IsExpression)
                    {
                        this.textBoxExpProp.Padding.Right = this.GetExpressionKey(style.PaddingRight.size);
                    }
                    if (style.WritingMode != null)
                    {
                        this.textBoxExpProp.WrtingMode = this.GetExpressionKey(style.WritingMode);
                    }

                    this.textBoxExpProp.Border = new BorderExp();

                    BorderExpProperties defaultBorder = null;

                    if (style.Border != null)
                    {
                        this.textBoxExpProp.Border.Default = new BorderExpProperties();
                        defaultBorder = this.textBoxExpProp.Border.Default;
                        this.textBoxExpProp.Border.Default.ThicknessSize = style.Border.Width;

                        if (style.Border.Color != null)
                        {
                            this.textBoxExpProp.Border.Default.BorderBrush = this.GetExpressionKey(style.Border.Color);
                        }
                        if (style.Border.Style != null)
                        {
                            this.textBoxExpProp.Border.Default.BorderStyle = this.GetExpressionKey(style.Border.Style);
                        }
                        if (style.Border.Width != null)
                        {
                            this.textBoxExpProp.Border.Default.Thickness = this.GetExpressionKey(style.Border.Width.size);
                        }
                    }

                    if (style.BottomBorder != null)
                    {
                        this.textBoxExpProp.Border.BottomBorder = new BorderExpProperties();
                        var borderExp = this.textBoxExpProp.Border.BottomBorder;

                        if (defaultBorder != null)
                        {
                            borderExp.BorderBrush = defaultBorder.BorderBrush;
                            borderExp.BorderStyle = defaultBorder.BorderStyle;
                            borderExp.Thickness = defaultBorder.Thickness;
                            borderExp.ThicknessSize = defaultBorder.ThicknessSize;
                        }

                        if (style.BottomBorder.Color != null)
                        {
                            this.textBoxExpProp.Border.BottomBorder.BorderBrush =
                                this.GetExpressionKey(style.BottomBorder.Color);
                        }
                        if (style.BottomBorder.Style != null)
                        {
                            this.textBoxExpProp.Border.BottomBorder.BorderStyle =
                                this.GetExpressionKey(style.BottomBorder.Style);
                        }
                        if (style.BottomBorder.Width != null)
                        {
                            borderExp.ThicknessSize = style.BottomBorder.Width;
                            this.textBoxExpProp.Border.BottomBorder.Thickness =
                                this.GetExpressionKey(style.BottomBorder.Width.size);
                        }
                    }
                    if (style.TopBorder != null)
                    {
                        this.textBoxExpProp.Border.TopBorder = new BorderExpProperties();

                        var borderExp = this.textBoxExpProp.Border.TopBorder;

                        if (defaultBorder != null)
                        {
                            borderExp.BorderBrush = defaultBorder.BorderBrush;
                            borderExp.BorderStyle = defaultBorder.BorderStyle;
                            borderExp.Thickness = defaultBorder.Thickness;
                            borderExp.ThicknessSize = defaultBorder.ThicknessSize;
                        }

                        if (style.TopBorder.Color != null)
                        {
                            this.textBoxExpProp.Border.TopBorder.BorderBrush =
                                this.GetExpressionKey(style.TopBorder.Color);
                        }
                        if (style.TopBorder.Style != null)
                        {
                            this.textBoxExpProp.Border.TopBorder.BorderStyle =
                                this.GetExpressionKey(style.TopBorder.Style);
                        }
                        if (style.TopBorder.Width != null)
                        {
                            borderExp.ThicknessSize = style.TopBorder.Width;
                            this.textBoxExpProp.Border.TopBorder.Thickness =
                                this.GetExpressionKey(style.TopBorder.Width.size);
                        }
                    }
                    if (style.LeftBorder != null)
                    {
                        this.textBoxExpProp.Border.LeftBorder = new BorderExpProperties();

                        var borderExp = this.textBoxExpProp.Border.LeftBorder;

                        if (defaultBorder != null)
                        {
                            borderExp.BorderBrush = defaultBorder.BorderBrush;
                            borderExp.BorderStyle = defaultBorder.BorderStyle;
                            borderExp.Thickness = defaultBorder.Thickness;
                            borderExp.ThicknessSize = defaultBorder.ThicknessSize;
                        }

                        if (style.LeftBorder.Color != null)
                        {
                            this.textBoxExpProp.Border.LeftBorder.BorderBrush =
                                this.GetExpressionKey(style.LeftBorder.Color);
                        }
                        if (style.LeftBorder.Style != null)
                        {
                            this.textBoxExpProp.Border.LeftBorder.BorderStyle =
                                this.GetExpressionKey(style.LeftBorder.Style);
                        }
                        if (style.LeftBorder.Width != null)
                        {
                            borderExp.ThicknessSize = style.LeftBorder.Width;
                            this.textBoxExpProp.Border.LeftBorder.Thickness =
                                this.GetExpressionKey(style.LeftBorder.Width.size);
                        }
                    }
                    if (style.RightBorder != null)
                    {
                        this.textBoxExpProp.Border.RightBorder = new BorderExpProperties();

                        var borderExp = this.textBoxExpProp.Border.RightBorder;

                        if (defaultBorder != null)
                        {
                            borderExp.BorderBrush = defaultBorder.BorderBrush;
                            borderExp.BorderStyle = defaultBorder.BorderStyle;
                            borderExp.Thickness = defaultBorder.Thickness;
                            borderExp.ThicknessSize = defaultBorder.ThicknessSize;
                        }

                        if (style.RightBorder.Color != null)
                        {
                            this.textBoxExpProp.Border.RightBorder.BorderBrush =
                                this.GetExpressionKey(style.RightBorder.Color);
                        }
                        if (style.RightBorder.Style != null)
                        {
                            this.textBoxExpProp.Border.RightBorder.BorderStyle =
                                this.GetExpressionKey(style.RightBorder.Style);
                        }
                        if (style.RightBorder.Width != null)
                        {
                            borderExp.ThicknessSize = style.RightBorder.Width;
                            this.textBoxExpProp.Border.RightBorder.Thickness =
                                this.GetExpressionKey(style.RightBorder.Width.size);
                        }
                    }
                    if (style.BackgroundColor != null)
                    {
                        this.textBoxExpProp.BackGroundColor = this.GetExpressionKey(style.BackgroundColor);

                    }
                    if (style.Color != null)
                    {
                        this.textBoxExpProp.Color = this.GetExpressionKey(style.Color);
                    }

                    if (style.VerticalAlign != null)
                    {
                        this.textBoxExpProp.VerticalAlignment = this.GetExpressionKey(style.VerticalAlign);
                    }
                }

                this.textBoxExpProp.CanGrow = txtBox.CanGrow;
                this.textBoxExpProp.CanShrink = txtBox.CanShrink;

                if (txtBox.Visibility != null && txtBox.Visibility.Hidden != null)
                {
                    this.textBoxExpProp.Hidden = this.GetExpressionKey(txtBox.Visibility.Hidden);
                }

                if (txtBox.Visibility != null && txtBox.Visibility.ToggleItem != null)
                {
                    this.ToggleItem = txtBox.Visibility.ToggleItem;
                }

                if (txtBox.DocumentMapLabel != null)
                {
                    this.textBoxExpProp.DocumentMap = this.GetExpressionKey(txtBox.DocumentMapLabel);
                    this.DocumentMapLable = this.textBoxExpProp.DocumentMap;
                }
          
                if (txtBox.ActionInfo != null)
                {
                    if (txtBox.ActionInfo.Actions.Any())
                    {
                        this.textBoxExpProp.TextboxActionInfo = this.GetActionInfo(txtBox.ActionInfo.Actions);
                    }
                }
            this.IsFirstToggle = true;
        }

        TextboxActionInfoExp GetActionInfo(Actions actions)
        {
            TextboxActionInfoExp actionInfoExp = new TextboxActionInfoExp();
            foreach (RDL.DOM.Action Action in actions)
            {
                actionInfoExp.BookmarkLink = this.GetExpressionKey(Action.BookmarkLink);
                actionInfoExp.Hyperlink = this.GetExpressionKey(Action.Hyperlink);

                if (Action.Drillthrough != null)
                {
                    actionInfoExp.ReportName = this.GetExpressionKey(Action.Drillthrough.ReportName);
                    actionInfoExp.Parameters = new List<TextboxParameterExp>();
                    if (Action.Drillthrough.Parameters != null)
                    {
                        foreach (Parameter parameters in Action.Drillthrough.Parameters)
                        {
                            TextboxParameterExp Parameter = new TextboxParameterExp();
                            Parameter.Name = this.GetExpressionKey(parameters.Name);
                            Parameter.Omit = this.GetExpressionKey(parameters.Omit);
                            Parameter.Value = this.GetExpressionKey(parameters.Value);
                            actionInfoExp.Parameters.Add(Parameter);
                        }
                    }
                }
            }
            return actionInfoExp;
        }

        void Intialize(TextBox textbox)
        {
            this.paraExp = new List<ParagraphExp>();

            foreach (Paragraph paragraph in textbox.Paragraphs)
            {
                ParagraphExp block = GetParagraphProperties(paragraph, textbox.Width.PixelValue);
                this.paraExp.Add(block);
            }
        }

        private ParagraphExp GetParagraphProperties(Paragraph paragraph, double desiredWidth)
        {
            ParagraphExp prgh = new ParagraphExp();
            prgh.Runs = new List<TextRunExp>();

            if (paragraph.TextRuns != null && paragraph.TextRuns.Count > 0)
            {
                foreach (TextRun run in paragraph.TextRuns)
                {
                    TextRunExp textRun = GetRun(run);
                    if (textRun != null)
                    {
                        prgh.Runs.Add(textRun);
                    }
                }
            }

            if (paragraph.LeftIndent != null)
            {
                prgh.LeftIndent = this.GetExpressionKey(paragraph.LeftIndent.ToString());
            }

            if (paragraph.RightIndent != null)
            {
                prgh.RightIndent = this.GetExpressionKey(paragraph.RightIndent.ToString());
            }

            if (paragraph.Style != null && paragraph.Style.TextAlign != null)
            {
                prgh.TextAlignment = this.GetExpressionKey(paragraph.Style.TextAlign.ToString());
            }

            prgh.ListLevel = paragraph.ListLevel;
            prgh.ListStyle = paragraph.ListStyle;
            prgh.SpaceAfterSize = paragraph.SpaceAfter;
            prgh.SpaceBeforeSize = paragraph.SpaceBefore;

            if (paragraph.SpaceBefore != null && paragraph.SpaceBefore.IsExpression)
            {
                prgh.SpaceBefore = this.GetExpressionKey(paragraph.SpaceBefore.size);
            }
            if (paragraph.SpaceAfter != null && paragraph.SpaceAfter.IsExpression)
            {
                prgh.SpaceAfter = this.GetExpressionKey(paragraph.SpaceAfter.size);
            }

            return prgh;
        }

        private TextRunExp GetRun(TextRun txtrun)
        {
            TextRunExp textRun = new TextRunExp();
            textRun.Text = this.GetExpressionKey(txtrun.Value);

            if (txtrun.Style != null)
            {
                textRun.Style = new StyleExp();
                textRun.Style.Format = this.GetRunExpressionKey(txtrun.Style.Format,txtrun.Value);
                textRun.Style.Language = this.GetRunExpressionKey(txtrun.Style.Language, txtrun.Value);
                textRun.Style.TextColor = this.GetRunExpressionKey(txtrun.Style.Color, txtrun.Value);
                textRun.Style.Font.FontFamily = this.GetRunExpressionKey(txtrun.Style.FontFamily, txtrun.Value);
                textRun.Style.Font.FontWeight = this.GetRunExpressionKey(txtrun.Style.FontWeight, txtrun.Value);
                textRun.Style.Font.FontStyle = this.GetRunExpressionKey(txtrun.Style.FontStyle, txtrun.Value);

                textRun.Style.Font.FontSizeValue = txtrun.Style.FontSize;

                if (txtrun.Style.TextDecoration != null)
                {
                    textRun.Style.TextDecoration = this.GetRunExpressionKey(txtrun.Style.TextDecoration, txtrun.Value);
                }
                if (txtrun.Style.FontSize != null)
                {
                    textRun.Style.Font.FontSize = this.GetExpressionKey(txtrun.Style.FontSize.size);
                }

            }

            if (txtrun.ActionInfo != null)
            {
                if (txtrun.ActionInfo.Actions.Any())
                {
                    textRun.ActionInfo = this.GetActionInfo(txtrun.ActionInfo.Actions);
                }
            }
        
            return textRun;
        }

        private void AddDataFields(string key)
        {
            if (this.IsTablixChild)
            {
                this.Model.ExpressionEngine.AddTablixDataFields(key, this);
            }
        }

        #endregion

        #region HelperMethods

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
            else if(this.DataSetFields != null && this.DataSource != null)
            {
                ReportingAggEngine engine = new ReportingAggEngine();
                engine.Fields = this.DataSetFields.Where(field=>!(field.IsRecursiveField || field.IsDataSetField)).ToList();

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

        #region overridden methods

        public override void Evaluate()
        {
            this.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();
            this.IntializeEngineValues();
            this.IsToggled = true;
            this.IsDrillAction = false;
            try
            {
                this.TextBoxProperties = new ReportItemExpval();
                this.TextBoxProperties.CanGrow = this.textBoxExpProp.CanGrow;
                this.TextBoxProperties.CanShrink = this.textBoxExpProp.CanShrink;

                if (!string.IsNullOrEmpty(this.ToggleItem))
                {
                    this.GetTextBoxModel(this.ToggleItem);
                }
                if (this.textBoxExpProp.Hidden != null)
                {
                    this.TextBoxProperties.Hidden = bool.Parse(this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Hidden));
                    this.Hidden = this.TextBoxProperties.Hidden;
                }
                else
                {
                    this.Hidden = this.TextBoxProperties.Hidden;
                }

                if (this.textBoxExpProp.WrtingMode != null)
                {
                    this.TextBoxProperties.WritingMode = this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.WrtingMode);
                    this.WritingMode = this.TextBoxProperties.WritingMode;
                }


                if (this.textBoxExpProp.DocumentMap != null)
                {
                    this.TextBoxProperties.DocumentMap = this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.DocumentMap);
                    this.DocumentMapLable = this.TextBoxProperties.DocumentMap;
                    if (!string.IsNullOrEmpty(this.DocumentMapLable) && !this.IsTablixChild)
                    {
                        this.SetTreeModel();
                    }
                }

                if (this.textBoxExpProp.VerticalAlignment != null)
                {
                    this.TextBoxProperties.VerticalAlignment = (VerticalAlign)Enum.Parse(typeof(VerticalAlign), this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.VerticalAlignment), true);
                }

                this.TextBoxProperties.Padding = new ThicknessExpval();

                if (this.textBoxExpProp.Padding != null && this.textBoxExpProp.Padding.TopSize != null)
                {
                    this.TextBoxProperties.Padding.Top = this.textBoxExpProp.Padding.TopSize.PixelValue;
                    if (this.textBoxExpProp.Padding.TopSize.IsExpression)
                    {
                        this.TextBoxProperties.Padding.Top = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Padding.Top)).PixelValue;
                    }
                }

                if (this.textBoxExpProp.Padding != null && this.textBoxExpProp.Padding.LeftSize != null)
                {
                    this.TextBoxProperties.Padding.Left = this.textBoxExpProp.Padding.LeftSize.PixelValue;
                    if (this.textBoxExpProp.Padding.LeftSize.IsExpression)
                    {
                        this.TextBoxProperties.Padding.Left = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Padding.Left)).PixelValue;
                    }
                }

                if (this.textBoxExpProp.Padding != null && this.textBoxExpProp.Padding.BottomSize != null)
                {
                    this.TextBoxProperties.Padding.Bottom = this.textBoxExpProp.Padding.BottomSize.PixelValue;
                    if (this.textBoxExpProp.Padding.BottomSize.IsExpression)
                    {
                        this.TextBoxProperties.Padding.Bottom = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Padding.Bottom)).PixelValue;
                    }
                }

                if (this.textBoxExpProp.Padding != null && this.textBoxExpProp.Padding.RightSize != null)
                {
                    this.TextBoxProperties.Padding.Right = this.textBoxExpProp.Padding.RightSize.PixelValue;
                    if(this.textBoxExpProp.Padding.RightSize.IsExpression)
                    {
                        this.TextBoxProperties.Padding.Right = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Padding.Right)).PixelValue;
                    }
                }

                this.TextBoxProperties.Border = new BorderExpval();

                if (this.textBoxExpProp.Border !=null && this.textBoxExpProp.Border.Default != null)
                {
                    this.TextBoxProperties.Border.Default = new BorderExpvalProperties();

                    if (this.textBoxExpProp.Border.Default.BorderBrush != null)
                    {
                        this.TextBoxProperties.Border.Default.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.Default.BorderBrush);
                    }
                    if (this.textBoxExpProp.Border.Default.BorderStyle != null)
                    {
                        this.TextBoxProperties.Border.Default.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.Default.BorderStyle), true);
                    }
                    if (this.textBoxExpProp.Border.Default.ThicknessSize != null)
                    {
                        this.TextBoxProperties.Border.Default.Thickness = this.textBoxExpProp.Border.Default.ThicknessSize.PixelValue;

                        if (this.textBoxExpProp.Border.Default.ThicknessSize.IsExpression)
                        {
                            this.TextBoxProperties.Border.Default.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.Default.Thickness)).FloatValue;
                        }
                    }
                }

                if (this.textBoxExpProp.Border != null && this.textBoxExpProp.Border.BottomBorder != null)
                {
                    this.TextBoxProperties.Border.BottomBorder = new BorderExpvalProperties();

                    if (this.textBoxExpProp.Border.BottomBorder.BorderBrush != null)
                    {
                        this.TextBoxProperties.Border.BottomBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.BottomBorder.BorderBrush);
                    }
                    if (this.textBoxExpProp.Border.BottomBorder.BorderStyle != null)
                    {
                        this.TextBoxProperties.Border.BottomBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.BottomBorder.BorderStyle), true);
                    }
                    if (this.textBoxExpProp.Border.BottomBorder.ThicknessSize != null)
                    {
                        this.TextBoxProperties.Border.BottomBorder.Thickness = this.textBoxExpProp.Border.BottomBorder.ThicknessSize.PixelValue;

                        if (this.textBoxExpProp.Border.BottomBorder.ThicknessSize.IsExpression)
                        {
                            this.TextBoxProperties.Border.BottomBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.BottomBorder.Thickness)).FloatValue;
                        }
                    }
                }

                if (this.textBoxExpProp.Border != null && this.textBoxExpProp.Border.TopBorder != null)
                {
                    this.TextBoxProperties.Border.TopBorder = new BorderExpvalProperties();

                    if (this.textBoxExpProp.Border.TopBorder.BorderBrush != null)
                    {
                        this.TextBoxProperties.Border.TopBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.TopBorder.BorderBrush);
                    }
                    if (this.textBoxExpProp.Border.TopBorder.BorderStyle != null)
                    {
                        this.TextBoxProperties.Border.TopBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.TopBorder.BorderStyle), true);
                    }
                    if (this.textBoxExpProp.Border.TopBorder.ThicknessSize != null)
                    {
                        this.TextBoxProperties.Border.TopBorder.Thickness = this.textBoxExpProp.Border.TopBorder.ThicknessSize.PixelValue;

                        if (this.textBoxExpProp.Border.TopBorder.ThicknessSize.IsExpression)
                        {
                            this.TextBoxProperties.Border.TopBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.TopBorder.Thickness)).FloatValue;
                        }
                    }
                }

                if (this.textBoxExpProp.Border != null && this.textBoxExpProp.Border.LeftBorder != null)
                {
                    this.TextBoxProperties.Border.LeftBorder = new BorderExpvalProperties();

                    if (this.textBoxExpProp.Border.LeftBorder.BorderBrush != null)
                    {
                        this.TextBoxProperties.Border.LeftBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.LeftBorder.BorderBrush);
                    }
                    if (this.textBoxExpProp.Border.LeftBorder.BorderStyle != null)
                    {
                        this.TextBoxProperties.Border.LeftBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.LeftBorder.BorderStyle), true);
                    }
                    if (this.textBoxExpProp.Border.LeftBorder.ThicknessSize != null)
                    {
                        this.TextBoxProperties.Border.LeftBorder.Thickness = this.textBoxExpProp.Border.LeftBorder.ThicknessSize.PixelValue;

                        if (this.textBoxExpProp.Border.LeftBorder.ThicknessSize.IsExpression)
                        {
                            this.TextBoxProperties.Border.LeftBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.LeftBorder.Thickness)).FloatValue;
                        }
                    }
                }

                if (this.textBoxExpProp.Border != null && this.textBoxExpProp.Border.RightBorder != null)
                {
                    this.TextBoxProperties.Border.RightBorder = new BorderExpvalProperties();

                    if (this.textBoxExpProp.Border.RightBorder.BorderBrush != null)
                    {
                        this.TextBoxProperties.Border.RightBorder.BorderBrush = this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.RightBorder.BorderBrush);
                    }
                    if (this.textBoxExpProp.Border.RightBorder.BorderStyle != null)
                    {
                        this.TextBoxProperties.Border.RightBorder.BorderStyle = (BorderStyles)Enum.Parse(typeof(BorderStyles), this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.RightBorder.BorderStyle), true);
                    }
                    if (this.textBoxExpProp.Border.RightBorder.ThicknessSize != null)
                    {
                        this.TextBoxProperties.Border.RightBorder.Thickness = this.textBoxExpProp.Border.RightBorder.ThicknessSize.PixelValue;

                        if (this.textBoxExpProp.Border.RightBorder.ThicknessSize.IsExpression)
                        {
                            this.TextBoxProperties.Border.RightBorder.Thickness = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Border.RightBorder.Thickness)).FloatValue;
                        }
                    }
                }

                if (this.textBoxExpProp.BackGroundColor != null)
                {
                    this.TextBoxProperties.BackGroundColor = this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.BackGroundColor);
                }

                if (this.textBoxExpProp.Color != null)
                {
                    this.TextBoxProperties.Color = this.Model.ExpressionEngine.GetEvalExpressionString(this.textBoxExpProp.Color);
                }

                this.ParaExpval = new List<ParagraphExpval>();
                foreach (var para in this.paraExp)
                {
                    ParagraphExpval paraVal = new ParagraphExpval();
                    paraVal.Runs = new List<TextRunExpval>();

                    foreach (var run in para.Runs)
                    {
                        TextRunExpval runVal = new TextRunExpval();
                        object value = this.Model.ExpressionEngine.GetEvalExpression(run.Text);
                        runVal.RunText = value;

                        runVal.Style = new StyleExpval();

                        if (run.Style.Font.FontFamily != null)
                        {
                            runVal.Style.Font.FontFamily = this.Model.ExpressionEngine.GetEvalExpressionString(run.Style.Font.FontFamily);
                        }

                        if (run.Style.Font.FontSizeValue != null)
                        {
                            runVal.Style.Font.FontSize = run.Style.Font.FontSizeValue.PixelValue;

                            if (run.Style.Font.FontSizeValue.IsExpression)
                            {
                                runVal.Style.Font.FontSize = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(run.Style.Font.FontSize)).PixelValue;
                            }
                        }

                        if (run.Style.Format != null)
                        {
                            runVal.Style.Format = this.Model.ExpressionEngine.GetEvalExpressionString(run.Style.Format);
                            runVal.Style.Language = this.Model.ExpressionEngine.GetEvalExpressionString(run.Style.Language);
                            runVal.Text = this.Model.ExpressionEngine.GetFormattedText(value, runVal.Style.Format, runVal.Style.Language);
                        }
                        else
                        {
                            if (value != null)
                            {
                                runVal.Text = value.ToString();
                            }
                        }

                        if (run.Style.Font.FontStyle != null)
                        {
                            runVal.Style.Font.FontStyle = (RDL.DOM.FontStyle)Enum.Parse(typeof(RDL.DOM.FontStyle), this.Model.ExpressionEngine.GetEvalExpressionString(run.Style.Font.FontStyle), true);
                        }
                        if (run.Style.Font.FontWeight != null)
                        {
                            runVal.Style.Font.FontWeight = (RDL.DOM.FontWeight)Enum.Parse(typeof(RDL.DOM.FontWeight), this.Model.ExpressionEngine.GetEvalExpressionString(run.Style.Font.FontWeight), true);
                        }
                        if (run.Style.TextDecoration != null)
                        {
                            runVal.Style.TextDecoration = this.Model.ExpressionEngine.GetEvalExpressionString(run.Style.TextDecoration);
                        }

                        runVal.Style.TextColor = this.Model.ExpressionEngine.GetEvalExpressionString(run.Style.TextColor);

                        if (run.ActionInfo != null)
                        {
                            this.IsDrillAction = true;
                            runVal.ActionInfoExpVal = this.GetActionInfoVal(run.ActionInfo);
                        }

                        paraVal.Runs.Add(runVal);
                    }

                    if (para.LeftIndent != null)
                    {
                        if (this.TextBoxProperties.Padding != null )
                        {
                           paraVal.LeftIndent = new DOM.Size( this.Model.ExpressionEngine.GetEvalExpressionString(para.LeftIndent)).PixelValue;
                        }
                    }

                    if (para.RightIndent != null)
                    {
                        if (this.TextBoxProperties.Padding != null )
                        {
                            paraVal.RightIndent = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(para.RightIndent)).PixelValue;
                        }
                    }

                    if (para.SpaceAfterSize != null)
                    {
                        paraVal.SpaceAfter = para.SpaceAfterSize.PixelValue;
                        if (para.SpaceAfterSize.IsExpression)
                        {
                            paraVal.SpaceAfter = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(para.SpaceAfter)).PixelValue;
                        }
                    }

                    if (para.SpaceBeforeSize != null)
                    {
                        paraVal.SpaceBefore = para.SpaceBeforeSize.PixelValue;
                        if (para.SpaceBeforeSize.IsExpression)
                        {
                            paraVal.SpaceBefore = new DOM.Size(this.Model.ExpressionEngine.GetEvalExpressionString(para.SpaceBefore)).PixelValue;
                        }
                    }

                    paraVal.ListLevel = para.ListLevel;
                    paraVal.ListStyle = para.ListStyle;
                    paraVal.TextAlignment = this.Model.ExpressionEngine.GetEvalExpressionString(para.TextAlignment);
                    this.ParaExpval.Add(paraVal);
                }

                if (this.textBoxExpProp.TextboxActionInfo != null)
                {
                    this.TextBoxProperties.TextboxActionInfo = this.GetActionInfoVal(this.textBoxExpProp.TextboxActionInfo);
                }

                base.Evaluate();
            }

            catch (Exception e)
            {
                this.Model.ExceptionDetails.Add("Getting following exception while evaluate the expression in " +this.ReportItem.Name+ " " + e.Message);
            }
            if (this.Model.ExpressionEngine.FieldValues != null)
            {
                this.Model.ExpressionEngine.FieldValues.Clear();
                this.Model.ExpressionEngine.FieldValues = null;
            }
        }

        TextboxActionInfoExpVal GetActionInfoVal(TextboxActionInfoExp actionInfoExp)
        {
            TextboxActionInfoExpVal actionInfoExpVal = new TextboxActionInfoExpVal();
            actionInfoExpVal.BookmarkLink = this.Model.ExpressionEngine.GetEvalExpressionString(actionInfoExp.BookmarkLink);
            actionInfoExpVal.Hyperlink = this.Model.ExpressionEngine.GetEvalExpressionString(actionInfoExp.Hyperlink);
            actionInfoExpVal.ReportName = this.Model.ExpressionEngine.GetEvalExpressionString(actionInfoExp.ReportName);

            if (actionInfoExp.Parameters != null)
            {
                actionInfoExpVal.Parameters = new List<TextboxParameterExpVal>();

                foreach (var parameters in actionInfoExp.Parameters)
                {
                    TextboxParameterExpVal Parameter = new TextboxParameterExpVal();
                    Parameter.Name = this.Model.ExpressionEngine.GetEvalExpressionString(parameters.Name);
                    Parameter.Omit = this.Model.ExpressionEngine.GetEvalExpressionString(parameters.Omit);
                    Parameter.Value = this.Model.ExpressionEngine.GetEvalExpressionString(parameters.Value);
                    actionInfoExpVal.Parameters.Add(Parameter);
                }
            }
            return actionInfoExpVal;
        }

        public override void DisposeReportItemObj()
        {
            this.Model = null;
            this.ReportItem = null;
            this.paraExp = null;
            this.textBoxExpProp = null;
            this.paraHeights = null;
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

        public override void DisposeEvalObjects()
        {
            this.textBoxExpProp.Padding.TopSize = null;
            this.textBoxExpProp.Padding.LeftSize = null;
            this.textBoxExpProp.Padding.BottomSize = null;
            this.textBoxExpProp.Padding.RightSize = null;

            if (this.textBoxExpProp.Border.Default != null)
            {
                this.textBoxExpProp.Border.Default.ThicknessSize = null;
            }

            if (this.textBoxExpProp.Border.BottomBorder != null)
            {
                this.textBoxExpProp.Border.BottomBorder.ThicknessSize = null;
            }

            if (this.textBoxExpProp.Border.TopBorder != null)
            {
                this.textBoxExpProp.Border.TopBorder.ThicknessSize = null;
            }

            if (this.textBoxExpProp.Border.LeftBorder != null)
            {
                this.textBoxExpProp.Border.LeftBorder.ThicknessSize = null;
            }

            if (this.textBoxExpProp.Border.RightBorder != null)
            {
                this.textBoxExpProp.Border.RightBorder.ThicknessSize = null;
            }

            if (this.TextBoxProperties != null)
            {
                this.TextBoxProperties.Padding = null;
                if (this.TextBoxProperties.TextboxActionInfo != null)
                {
                    if (this.TextBoxProperties.TextboxActionInfo.Parameters != null)
                    {
                        this.TextBoxProperties.TextboxActionInfo.Parameters.Clear();
                        this.TextBoxProperties.TextboxActionInfo.Parameters = null;
                    }

                    this.TextBoxProperties.TextboxActionInfo = null;
                }

                this.TextBoxProperties.Border = null;
            }
            this.TextBoxProperties = null;

            if (this.ParaExpval != null)
            {
                foreach (ParagraphExpval para in this.ParaExpval)
                {
                    if (para.Runs != null)
                    {
                        foreach (TextRunExpval run in para.Runs)
                        {
                            if (run.Style != null)
                            {
                                run.Style.Font = null;
                            }

                            run.Style = null;
                        }

                        para.Runs.Clear();
                    }

                    para.Runs = null;
                }
            }
            this.ParaExpval = null;


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
            node.ModelType = ModelType.TextBoxModel;
            node.ReportItemName = this.Name;
            node.IsTablixChild = this.IsTablixChild;
            this.DocumentNodeRefer = node;
            this.Model.MapModel.NodeData.Add(node);
        }

        public override void UpdateSize()
        {
            ActualHeight = 0;
            this.paraHeights.Clear();
            double avilableWidth = this.Width;

            if (this.TextBoxProperties.Padding != null)
            {
                avilableWidth -= this.TextBoxProperties.Padding.Left;
                avilableWidth -= this.TextBoxProperties.Padding.Right;
            }

            foreach (ParagraphExpval paragraph in this.ParaExpval)
            {
                List<LayoutSize> runSizes = new List<LayoutSize>();
                foreach (TextRunExpval run in paragraph.Runs)
                {
                    runSizes.Add(this.GetRunSize(run, avilableWidth, paragraph.Runs.Count>1));
                }

                if (runSizes.Count == 1)
                {
                    this.paraHeights.Add(runSizes[0].Height);
                }

                else
                {
                    this.paraHeights.Add(this.GetParagraphHeight(runSizes, avilableWidth));
                }

                this.paraHeights.Add(paragraph.SpaceAfter + paragraph.SpaceBefore);
            }

            foreach (var height in this.paraHeights.ToList())
            {
                ActualHeight += height;
            }

            if (this.TextBoxProperties.Padding != null)
            {
                ActualHeight += this.TextBoxProperties.Padding.Top;
                ActualHeight += this.TextBoxProperties.Padding.Bottom;
            }

            if (ActualHeight <= 0 || ActualHeight < this.Height)
            {
                this.ActualHeight = this.Height;
            }
        }

        double GetParagraphHeight(List<LayoutSize> textRunSizes, double desiredWidth)
        {
            double lineHeights = 0;
            double size = 0;
            double maxHeight = 0;
            bool reachEnd = false;

            for (int i = 0; i < textRunSizes.Count && !reachEnd; )
            {
                double runSize = textRunSizes[i].Width;
                double runHeight = textRunSizes[i].Height;
                double diffSize = 0;

                if (runSize <= 0 || runHeight <= 0)
                {
                    i++;
                    reachEnd = (i == textRunSizes.Count);
                    lineHeights += 0;
                }
                else if (size + runSize >= desiredWidth && desiredWidth>=0)
                {
                    diffSize = desiredWidth - size;
                    LayoutSize modifiedSize = textRunSizes[i];
                    maxHeight = runHeight > maxHeight ? runHeight : maxHeight;
                    modifiedSize.Width = runSize - diffSize;
                    textRunSizes[i] = modifiedSize;
                    lineHeights += maxHeight;
                    maxHeight = 0;
                    size = 0;
                }
                else
                {
                    i++;
                    maxHeight = runHeight > maxHeight ? runHeight : maxHeight;
                    size += runSize;

                    reachEnd = (i == textRunSizes.Count);

                    if (size == desiredWidth || reachEnd)
                    {
                        lineHeights += maxHeight;
                    }
                }
            }

            return lineHeights;
        }

        LayoutSize GetRunSize(TextRunExpval run,double maxWidth,bool IsMultiRun)
        {
            double height = 0;
            double width = 0;
            string text = run.Text;

            if (string.IsNullOrEmpty(text))
            {
                text = " ";
            }

            text = text.Replace(" ", "-");
#if WINRT
            Windows.UI.Text.FontStyle fontStyle = Windows.UI.Text.FontStyle.Normal;

            if (run.Style.Font.FontStyle == DOM.FontStyle.Italic)
            {
                fontStyle = Windows.UI.Text.FontStyle.Italic;
            }

            Windows.UI.Text.FontWeight fontWeight = Windows.UI.Text.FontWeights.Normal;

            if (run.Style.Font.FontWeight == DOM.FontWeight.Bold)
            {
                fontWeight = Windows.UI.Text.FontWeights.Bold;
            }
            double fontSize = run.Style.Font.FontSize;

            var textBlock = new Windows.UI.Xaml.Controls.TextBlock()
            {
                Text = text,
                TextWrapping = Windows.UI.Xaml.TextWrapping.NoWrap,
                FontSize = fontSize,
                FontFamily = new Windows.UI.Xaml.Media.FontFamily(run.Style.Font.FontFamily),
                FontWeight = fontWeight,
                FontStyle = fontStyle,
                TextAlignment = Windows.UI.Xaml.TextAlignment.Left,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top
            };

            if (!IsMultiRun && maxWidth > 0)
            {
                textBlock.TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap;
                textBlock.MaxWidth = maxWidth;
            }

            Windows.UI.Xaml.Controls.Border parentBorder = new Windows.UI.Xaml.Controls.Border();
            parentBorder.Child = textBlock;
            textBlock.UpdateLayout();
            textBlock.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            parentBorder.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            parentBorder.Arrange(new Windows.Foundation.Rect(0, 0, textBlock.ActualWidth, textBlock.ActualHeight));
            height = textBlock.ActualHeight;
            width = textBlock.ActualWidth;

#elif SILVERLIGHT

            System.Windows.FontStyle fontStyle = FontStyles.Normal;

            if (run.Style.Font.FontStyle == DOM.FontStyle.Italic)
            {
                fontStyle = FontStyles.Italic;
            }

            System.Windows.FontWeight fontWeight = System.Windows.FontWeights.Normal;

            if (run.Style.Font.FontWeight == DOM.FontWeight.Bold)
            {
                fontWeight = System.Windows.FontWeights.Bold;
            }
            double fontSize = run.Style.Font.FontSize;

            var textBlock = new System.Windows.Controls.TextBlock()
            {
                Text = text,
                TextWrapping = TextWrapping.NoWrap,
                FontSize = fontSize,
                FontFamily = new System.Windows.Media.FontFamily(run.Style.Font.FontFamily),
                FontWeight = fontWeight,
                FontStyle = fontStyle,
                TextAlignment = TextAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };


            if (!IsMultiRun && maxWidth > 0)
            {
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.MaxWidth = maxWidth;
            }


            System.Windows.Controls.Border parentBorder = new System.Windows.Controls.Border();
            parentBorder.Child = textBlock;
            textBlock.UpdateLayout();
            textBlock.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            parentBorder.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            parentBorder.Arrange(new Rect(0, 0, textBlock.ActualWidth, textBlock.ActualHeight));
            height = textBlock.ActualHeight;
            width = textBlock.ActualWidth;
#else
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            FlowDirection flowDirection = FlowDirection.LeftToRight;
            System.Windows.FontStyle fontStyle= FontStyles.Normal;

            if (run.Style.Font.FontStyle == DOM.FontStyle.Italic)
            {
                fontStyle = FontStyles.Italic;
            }

            System.Windows.FontWeight fontWeight = System.Windows.FontWeights.Normal;

            if(run.Style.Font.FontWeight == DOM.FontWeight.Bold)
            {
                fontWeight = System.Windows.FontWeights.Bold;
            }

            System.Windows.Media.Typeface typeface  = 
                new System.Windows.Media.Typeface(new System.Windows.Media.FontFamily(run.Style.Font.FontFamily),
                    fontStyle, fontWeight, FontStretches.Normal);

            double fontSize = run.Style.Font.FontSize;
            
            System.Windows.Media.FormattedText formattedText = new System.Windows.Media.FormattedText(text, cultureInfo, flowDirection, typeface, fontSize, null);

            if (!IsMultiRun && maxWidth > 0)
            {
                formattedText.MaxTextWidth = maxWidth;
            }

            formattedText.Trimming = TextTrimming.None;
            height = formattedText.Height;
            width = formattedText.Width;
#endif
            return new LayoutSize(width, height);
        }


        public override IReportItemModeler GetModel()
        {
            TextboxModel itemModel = new TextboxModel();
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
            itemModel.paraExp = this.paraExp;
            itemModel.textBoxExpProp = this.textBoxExpProp;
            itemModel.Hidden = this.Hidden;
            itemModel.paraHeights = paraHeights;
            itemModel.CanGrow = this.CanGrow;
            itemModel.DataSetFields = this.DataSetFields;
            itemModel.ToggleGroups = this.ToggleGroups;

            if (this.IsTablixInnerChild)
            {
                itemModel.FlowLayoutInfo = this.FlowLayoutInfo;
            }

            return itemModel;
        }

        public override void UpdateHeight(double firstPageHeight, LayoutReportItemModel locationInfo, double preferredHeight, ReportItemViewMode itemViewMode)
        {
            if (this.Hidden)
            {
                locationInfo.ActualHeight = 0;
                return;
            }
            locationInfo.ActualHeight = this.ActualHeight;
            Dictionary<int, PageInfo> pageSizes = new Dictionary<int, PageInfo>();
            List<double> pageWidths = itemViewMode == ReportItemViewMode.Normal ? this.PageWidths : this.PrintPageWidths;

            if (itemViewMode != ReportItemViewMode.None)
            {
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

        public override void UpdateWidth(double firstPageWidth, LayoutReportItemModel locationInfo, double preferredWidth, ReportItemViewMode itemViewMode)
        {
            if (this.Hidden)
            {
                locationInfo.ActualWidth = 0;
                return;
            }

            locationInfo.ActualWidth = this.Width;

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
                this.DocumentNodeRefer.PageNo=pageNo+1;
                this.DocumentNodeRefer.TopPos = this.Top;
                this.DocumentNodeRefer.LeftPos = this.Left;
                this.DocumentNodeRefer = null;
            }
        }

        #endregion

        internal void AddToggleItems(string toggleNames)
        {
            if (this.ToggleInfos == null)
            {
                this.ToggleInfos=new List<string>();
            }
            if (this.IsFirstToggle)
            {
                this.ToggleInfos.Clear();
                this.IsFirstToggle = false;
            }
            this.ToggleInfos.Add(toggleNames);
        }
    }
}