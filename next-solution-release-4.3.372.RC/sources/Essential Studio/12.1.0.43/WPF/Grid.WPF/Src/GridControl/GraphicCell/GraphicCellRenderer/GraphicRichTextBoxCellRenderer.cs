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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Input;
using System.Windows;
#if SILVERLIGHT
using Syncfusion.Windows.Controls.Grid.GridUtils;
#endif


namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicRichTextBoxCellModel:GraphicCellModel<GraphicRichTextBoxCellRenderer>
    {
        public GraphicRichTextBoxCellModel()
        {

        }
    }

#if !SILVERLIGHT
    public class GraphicRichTextBoxCellRenderer : GraphicCellRendererBase<RichTextBox>
#else
    public class GraphicRichTextBoxCellRenderer : GraphicCellRendererBase<RichTextBoxAdv>
#endif
    {
        public GraphicRichTextBoxCellRenderer()
        {
            IsEditable = true;
        }

#if !SILVERLIGHT
        protected override RichTextBox CreateUIElement(GraphicStyleInfo cellInfo)
        {
            if (cellInfo.CellValue != null && cellInfo.CellValue is FlowDocument)
            {
                FlowDocument newDocument = cellInfo.CellValue as FlowDocument;
                if (newDocument != null)
                {
                    if (newDocument.Parent != null)
                    {
                        var parentTextBox = newDocument.Parent as RichTextBox;
                        parentTextBox.Document = new FlowDocument();
                    }
                }
                RichTextBox richTextBox = new RichTextBox(newDocument);
                richTextBox.AcceptsTab = true;
                return richTextBox;
            }
            return base.CreateUIElement(cellInfo);
        }
#else
        protected override RichTextBoxAdv CreateUIElement(GraphicStyleInfo cellInfo)
        {
            if (cellInfo.CellValue != null)
            {
                RichTextBoxAdv richTextBox = new RichTextBoxAdv();
                SetRichTextControlValue(cellInfo, richTextBox);
                return richTextBox;
            }
            return base.CreateUIElement(cellInfo);
        }

        public void SetRichTextControlValue(GraphicStyleInfo style, RichTextBoxAdv rtb)
        {
            string xamlstring = string.Empty;
            if (style.CellValue is Paragraph)
            {
                try
                {
                    //Add for the current cell selection issue
                    if (rtb.Blocks.Count == 0)
                    {
                        xamlstring = GridUtility.ConvertParagraphToXaml(style.CellValue as Paragraph);
                        rtb.Blocks.Add(XamlReader.Load(xamlstring) as Block);
                        //style.CellValue = XamlReader.Load(xamlstring) as Paragraph;
                        //this.GridControl.Model[style.RowIndex, style.ColumnIndex].CellValue = style.CellValue;
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Cell value is unidentified");
                }
            }
            else if (style.CellValue is List<Paragraph>)
            {
                try
                {
                    List<Paragraph> Paragraphs = style.CellValue as List<Paragraph>;
                    int Previousblocks = rtb.Blocks.Count;
                    foreach (Paragraph paragraph in Paragraphs)
                    {
                        xamlstring = GridUtility.ConvertParagraphToXaml(paragraph);
                        rtb.Blocks.Add(XamlReader.Load(xamlstring) as Paragraph);
                        xamlstring = string.Empty;
                    }
                    for (int count = 0; count < Previousblocks; count++)
                        rtb.Blocks.RemoveAt(count);
                    style.CellValue = XamlReader.Load(xamlstring) as List<Paragraph>;
                    this.GridControl.Model.GraphicModel[style.CellIndex].CellValue = style.CellValue;
                }
                catch (Exception)
                {
                    throw new Exception("Cell value is unidentified");
                }
            }
            else
            {
                throw new ArgumentNullException("CellValue", "CellValue is not supported. CellValue must be Paragraph or List<Paragraph>");
            }
        }
#endif

#if SILVERLIGHT
        protected override void OnInitializeContent(RichTextBoxAdv element, GraphicStyleInfo style)
        {
            if (style.HasBackground)
                element.Background = style.Background;
            if (style.HasBorderBrush)
                element.BorderBrush = style.BorderBrush;
            if (style.HasBorderThickness)
                element.BorderThickness = style.BorderThickness;
            base.OnInitializeContent(element, style);
        }
#else
        protected override void OnInitializeContent(RichTextBox element, GraphicStyleInfo style)
        {
            if (style.HasBackground)
                element.Background = style.Background;
            if (style.HasBorderBrush)
                element.BorderBrush = style.BorderBrush;
            if (style.HasBorderThickness)
                element.BorderThickness = style.BorderThickness;
            if (style.HasReadOnly)
                element.IsReadOnly = style.ReadOnly;
            if (style.HasHorizontalAlignment)
                element.HorizontalAlignment = style.HorizontalAlignment;
            if (style.HasVerticalAlignment)
                element.VerticalAlignment = style.VerticalAlignment;
            base.OnInitializeContent(element, style);
        }
#endif

#if SILVERLIGHT
        protected override object GetControlValueFromEditor(RichTextBoxAdv uiElement)
        {
            if (uiElement != null)
            {
                if (uiElement.Blocks.Count == 1)
                {
                    return uiElement.Blocks[0] as Paragraph;
                }
                else
                {
                    List<Paragraph> cellvalue = new List<Paragraph>();
                    foreach (var item in uiElement.Blocks)
                    {
                        cellvalue.Add(item as Paragraph);
                    }
                    return cellvalue;
                }
            }
            return base.GetControlValueFromEditor(uiElement);
        }
#else
        protected override object GetControlValueFromEditor(RichTextBox uiElement)
        {
            if (uiElement != null)
            {
                return uiElement.Document;
            }
            return base.GetControlValueFromEditor(uiElement);
        }
#endif

        protected override bool ShouldTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Enter:
                case Key.Up:
                case Key.Down:
                    return true;
#if !SILVERLIGHT
                case Key.LeftShift:
                case Key.RightShift:
                case Key.Delete:
                case Key.Tab:
                    if (this.CurrentUIElement.IsFocused)
                        return true;
                    break;
#else
                case Key.Shift:
                case Key.Delete:
                case Key.Tab:
                    if (FocusManager.GetFocusedElement() as UIElement == this.CurrentUIElement)
                        return true;
                    break;
#endif
                case Key.Escape:
                    break;
                default:
#if !SILVERLIGHT
                    if (Keyboard.Modifiers != ModifierKeys.Control)
                        this.CurrentUIElement.Focus();
#else
#endif
                    break;
            }
            return base.ShouldTryToHandlePreviewKeyDown(e);
        }

#if !SILVERLIGHT
        protected override void UnloadUIElements(int index, RichTextBox uiElement)
        {

            if (uiElement != null)
            {
                uiElement.Document = new FlowDocument();
            }
            base.UnloadUIElements(index, uiElement);
        }
#endif
    }
}
