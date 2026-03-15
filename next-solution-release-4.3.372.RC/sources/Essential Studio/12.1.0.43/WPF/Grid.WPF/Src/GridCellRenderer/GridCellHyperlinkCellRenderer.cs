#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Text.RegularExpressions;
    using System.Diagnostics;

    public class GridCellHyperlinkModel : GridCellModel<GridCellHyperlinkCellRenderer>
    {
        public GridCellHyperlinkModel()
        {

        }
    }

    public class GridCellHyperlinkCellRenderer : GridVirtualizingCellRenderer<ContentControl>
    {
        public GridCellHyperlinkCellRenderer()
        {
            this.AllowRecycle = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.IsControlTextShown = true;
            this.SupportsRenderOptimization = false;
        }

        protected override ContentControl CreateUIElement(Cells.ArrangeCellArgs aca, GridRenderStyleInfo cellInfo)
        {
            ContentControl uiElement = new ContentControl();
            Hyperlink hyperlink = new Hyperlink();
            uiElement.Background = cellInfo.Background;
            uiElement.Content = hyperlink;
            return uiElement;
        }        

        public override void CreateRendererElement(ContentControl uiElement, GridRenderStyleInfo style)
        {            
            InitiaLizeContent(uiElement, style);
        }

        protected override string GetControlTextFromEditorCore(ContentControl uiElement)
        {
            return CurrentStyle.CellValue.ToString();
        }

        public override void OnInitializeContent(ContentControl uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            InitiaLizeContent(uiElement, style);
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                uiElement.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = uiElement.Width;
                double offsetY = 0;
                uiElement.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
            }
            else
            {
                uiElement.LayoutTransform = MatrixTransform.Identity;
            }
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, ContentControl uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;

            base.ArrangeUIElement(aca, uiElement, style);
        }

        protected void OnWireUIElement(Hyperlink hyperlink)
        {
            hyperlink.Click += new RoutedEventHandler(hyperlink_Click);
        }

        protected void OnUnwireUIElement(Hyperlink hyperlink)
        {
            hyperlink.Click -= new RoutedEventHandler(hyperlink_Click);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            GridControl.InvalidateCell(CellRowColumnIndex);            
        }

        private void InitiaLizeContent(ContentControl uiElement, GridRenderStyleInfo style)
        {
            if (uiElement.Content == null)
            {
                return;
            }
            Hyperlink hyperlink = uiElement.Content as Hyperlink;
            uiElement.IsTabStop = false;
            KeyboardNavigation.SetIsTabStop(hyperlink, false);
            OnUnwireUIElement(hyperlink);
            Run run = new Run();
            var fontinfo = style.Font;
            run.FontFamily = fontinfo.FontFamily;
            run.FontSize = fontinfo.FontSize;
            run.FontStretch = fontinfo.FontStretch;
            run.FontStyle = fontinfo.FontStyle;
            run.FontWeight = fontinfo.FontWeight;
            run.Text = style.CellValue.ToString();
            hyperlink.Inlines.Clear();
            hyperlink.Inlines.Add(run);
            if (style.CellValue2 != null || style.CellValue != null)
            {
                string url = style.CellValue2 != null ? style.CellValue2.ToString() : (style.CellValue != null ? style.CellValue.ToString() : string.Empty);
                string pattern = @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
                if (Regex.IsMatch(url, pattern))
                {
                    hyperlink.NavigateUri = new Uri(url);
                }
            }
            OnWireUIElement(hyperlink);
            uiElement.Background = style.Background;
            uiElement.Content = hyperlink;
            uiElement.SetValue(TextBox.TextAlignmentProperty, HorizontalAlignmentToTextAlignment(style.HorizontalAlignment));
            VisualContainer.SetWantsMouseInput(hyperlink, true);
            VisualContainer.SetWantsMouseInput(run, true);
            VisualContainer.SetWantsMouseInput(uiElement, true);
        }

        protected override void OnDeactivated()
        {
            GridControl.InvalidateCell(CellRowColumnIndex);
        }

        void hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            if (this.HasCurrentCellState)
            {
                var url = this.CurrentStyle.CellValue2 != null ? this.CurrentStyle.CellValue2.ToString() : string.Empty;
                if (hyperlink.NavigateUri == null && url.Contains("!"))
                {
                    this.GridControl.Focus();
                    string sheetname = url.Substring(0, url.IndexOf('!'));
                    string strcell = url.Substring(url.IndexOf('!') + 1);
                    int rowIndex = RowIndex(strcell);
                    int colIndex = ColIndex(strcell);
                    CellRequestNavigateEventArgs arg = new CellRequestNavigateEventArgs(sheetname, rowIndex, colIndex, GridModel.CellRequestNavigateEvent, this);
                    this.GridControl.Model.RaiseCellRequestNavigate(arg);
                }
                else
                {
                    CellRequestNavigateEventArgs arg = new CellRequestNavigateEventArgs(hyperlink.NavigateUri, this.CurrentStyle.RowIndex, this.CurrentStyle.ColumnIndex, GridModel.CellRequestNavigateEvent, this);
                    this.GridControl.Model.RaiseCellRequestNavigate(arg);
                    if (arg.Handled)
                    {
                        return;
                    }
                    if (hyperlink != null && hyperlink.NavigateUri != null)
                        Process.Start(new ProcessStartInfo(hyperlink.NavigateUri.AbsoluteUri));
                }
            }
        }

        private int RowIndex(string s)
        {
            int row;
            int i = 0;
            while (i < s.Length && char.IsLetter(s[i]))
                i++;
            if (i < s.Length)
            {
                if (int.TryParse(s.Substring(i), out row))
                {
                    return row;
                }
            }
            return -1;
        }

        public int ColIndex(string s)
        {
            int i = 0;
            int k = 0;
            s = s.ToUpper();
            while (i < s.Length && char.IsLetter(s[i]))
            {
                k = (int)(k * 26 + (int)s[i] - (int)('A') + 1);
                i++;
            }
            return k;
        }
    }
}
