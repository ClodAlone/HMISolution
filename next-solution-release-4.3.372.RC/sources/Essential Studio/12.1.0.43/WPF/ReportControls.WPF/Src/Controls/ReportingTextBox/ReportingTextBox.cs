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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.ItemModel;
using Syncfusion.RDL.Internal;

namespace Syncfusion.RDL.Controls
{
    internal class ReportingTextBox : RichTextBox
    {
        internal TextboxModel txtboxModel
        {
            get;
            set;
        }

        public ReportingTextBox(IReportItemModeler pageContent)
        {
            txtboxModel = pageContent as TextboxModel;

            if (this.txtboxModel.IsTablixInnerChild)
            {
                this.Margin = new Thickness(pageContent.Left, pageContent.Top, 0, 0);
                this.Width = txtboxModel.Width;
                this.Height = pageContent.CanGrow ? txtboxModel.ActualHeight : pageContent.Height; 
            }
            else if (this.txtboxModel.IsTablixChild)
            {
                this.Margin = new Thickness(pageContent.Left, pageContent.Top, 0, 0);
                this.Width = txtboxModel.Width;
                this.Height = pageContent.CanGrow ? txtboxModel.ActualHeight : pageContent.Height;
            }
            else if (pageContent.FlowLayoutInfo != null)
            {
                this.Margin = new Thickness(pageContent.FlowLayoutInfo.ActualLeft, pageContent.FlowLayoutInfo.ActualTop, 0, 0);
                this.Width = pageContent.Width;
                this.Height = pageContent.FlowLayoutInfo.ActualHeight;
            }

            IntializeTextBox();          

#if !SyncfusionFramework3_5
            this.CaretBrush = new SolidColorBrush(Colors.Transparent);
#endif
            this.IsHitTestVisible = false;

            //this.txtboxModel = null;
        }

        private void IntializeTextBox()
        {
            
#if SILVERLIGHT
            this.Blocks.Clear();
#else
            this.Document.Blocks.Clear();
#endif
            foreach (var para in txtboxModel.ParaExpval)
            {
                System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph();

#if !SILVERLIGHT && !WINRT
                paragraph.LineHeight = 0.5;
#endif

                switch (para.TextAlignment)
                {
                    case "Left":
                        paragraph.TextAlignment = System.Windows.TextAlignment.Left;
                        break;
                    case "Right":
                        paragraph.TextAlignment = System.Windows.TextAlignment.Right;
                        break;
                    case "Center":
                        paragraph.TextAlignment = System.Windows.TextAlignment.Center;
                        break;
                    default:
                        paragraph.TextAlignment = System.Windows.TextAlignment.Left;
                        break;
                }

                foreach (var run in para.Runs)
                {
                    System.Windows.Documents.Run txtRun = new System.Windows.Documents.Run();
                    txtRun.Text = run.Text;
                   
                    if (run.Style != null)
                    {
                        txtRun.Foreground = new ReportingBrushConverter().ConvertFromInvariantString(run.Style.TextColor);
                        txtRun.FontFamily = new FontFamily(run.Style.Font.FontFamily);
                        txtRun.FontSize = run.Style.Font.FontSize;

                        if (run.Style.Font.FontStyle != DOM.FontStyle.Default)
                        {
                            txtRun.FontStyle = new ReportingFontStyleConverter().ConvertFromInvariantString(run.Style.Font.FontStyle.ToString());
                        }
                        if (run.Style.Font.FontWeight != DOM.FontWeight.Default)
                        {
                            txtRun.FontWeight = new ReportingFontWeightConverter().ConvertFromInvariantString(run.Style.Font.FontWeight.ToString());
                        }

#if !SILVERLIGHT && !WINRT
                        if (run.Style.TextDecoration != null && run.Style.TextDecoration.ToLower() != "default"
                            && run.Style.TextDecoration.ToLower() != "none")
                        {
                            TextDecorationCollection txtdecr = new TextDecorationCollection();
                            string decoration = run.Style.TextDecoration.ToLower();

                            if (decoration == "underline")
                            {
                                txtdecr.Add(TextDecorations.Underline);
                            }
                            else if (decoration == "baseline")
                            {
                                txtdecr.Add(TextDecorations.Baseline);
                            }
                            else if (decoration == "overline")
                            {
                                txtdecr.Add(TextDecorations.OverLine);
                            }
                            else if (decoration == "strikethrough" || decoration == "linethrough")
                            {
                                txtdecr.Add(TextDecorations.Strikethrough);
                            }

                            txtRun.TextDecorations = txtdecr;
                        }
#endif
                    }
                    
                    paragraph.Inlines.Add(txtRun);
                }
#if !SILVERLIGHT && !WINRT
                paragraph.Padding = new Thickness(para.LeftIndent, para.SpaceBefore, para.RightIndent, para.SpaceAfter);
#endif
#if SILVERLIGHT
                this.Blocks.Add(paragraph);
#else
                this.Document.Blocks.Add(paragraph);
#endif
            }
            if (this.txtboxModel.TextBoxProperties.Border.Default != null &&
                this.txtboxModel.TextBoxProperties.Border.Default.BorderStyle != DOM.BorderStyles.None
                && this.txtboxModel.TextBoxProperties.Border.Default.BorderStyle != DOM.BorderStyles.Default)
            {
                double left;
                double right;
                double top;
                double bottom;
                left = right = top = bottom = this.txtboxModel.TextBoxProperties.Border.Default.Thickness;

                if (this.txtboxModel.TextBoxProperties.Border.LeftBorder != null)
                {
                    left = this.txtboxModel.TextBoxProperties.Border.LeftBorder.Thickness;
                }
                if (this.txtboxModel.TextBoxProperties.Border.RightBorder != null)
                {
                    right = this.txtboxModel.TextBoxProperties.Border.RightBorder.Thickness;
                }
                if (this.txtboxModel.TextBoxProperties.Border.TopBorder != null)
                {
                    top = this.txtboxModel.TextBoxProperties.Border.TopBorder.Thickness;
                }
                if (this.txtboxModel.TextBoxProperties.Border.BottomBorder != null)
                {
                    bottom = this.txtboxModel.TextBoxProperties.Border.BottomBorder.Thickness;
                }

                this.BorderThickness = new Thickness(left, top, right, bottom);
                this.BorderBrush = new ReportingBrushConverter().ConvertFromInvariantString(this.txtboxModel.TextBoxProperties.Border.Default.BorderBrush);
            }
            else
            {
                this.BorderThickness = new Thickness(0);
            }

            this.HorizontalScrollBarVisibility =ScrollBarVisibility.Disabled;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

            this.Background = new ReportingBrushConverter().ConvertFromInvariantString(this.txtboxModel.TextBoxProperties.BackGroundColor);

#if SILVERLIGHT
            this.Padding = new Thickness(this.txtboxModel.TextBoxProperties.Padding.Left, this.txtboxModel.TextBoxProperties.Padding.Top, this.txtboxModel.TextBoxProperties.Padding.Right, this.txtboxModel.TextBoxProperties.Padding.Bottom);
#else
            this.Padding = new Thickness(this.txtboxModel.TextBoxProperties.Padding.Left - 5, this.txtboxModel.TextBoxProperties.Padding.Top, this.txtboxModel.TextBoxProperties.Padding.Right, this.txtboxModel.TextBoxProperties.Padding.Bottom);
#endif
        }
    }
}
