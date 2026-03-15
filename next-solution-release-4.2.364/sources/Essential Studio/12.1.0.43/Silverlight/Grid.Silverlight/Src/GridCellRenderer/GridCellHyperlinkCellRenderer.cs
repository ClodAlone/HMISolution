#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
#if !WinRT
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Controls.Cells;
namespace Syncfusion.Windows.Controls.Grid
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Syncfusion.WinRT.Controls.Cells;
using Windows.UI.Core;
using Windows.System;
using Windows.UI.Xaml.Documents;
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellHyperlinkModel : GridCellModel<GridCellHyperlinkCellRenderer>
    {
        public GridCellHyperlinkModel()
        {

        }
    }
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellHyperlinkCellRenderer : GridVirtualizingCellRenderer<HyperlinkButton>
    {
        public GridCellHyperlinkCellRenderer()
        {
            this.AllowRecycle = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.IsControlTextShown = false;
            this.SupportsRenderOptimization = false;
            this.IsFocusable = true;
        }

        //protected override UIElement CreateRendererElement(Cells.ArrangeCellArgs aca, GridRenderStyleInfo cellInfo)
        //{
        //    HyperlinkButton hyperlink = new HyperlinkButton();
        //    var font = cellInfo.ReadOnlyFont;
        //    hyperlink.FontFamily = font.FontFamily;
        //    hyperlink.FontSize = font.FontSize;
        //    hyperlink.FontStretch = font.FontStretch;
        //    hyperlink.FontStyle = font.FontStyle;
        //    hyperlink.FontWeight = font.FontWeight;
        //    hyperlink.Background = cellInfo.Background;
        //    if (cellInfo.Binding == null)
        //    {
        //        hyperlink.Content = GetControlText(cellInfo);
        //    }
        //    else
        //    {
        //        hyperlink.DataContext = cellInfo.DataContext;
        //        this.ApplyBinding(hyperlink, HyperlinkButton.ContentProperty, cellInfo.Binding);
        //    }
        //    string url = cellInfo.CellValue2.ToString();
        //    string pattern = @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
        //    if (Regex.IsMatch(url, pattern))
        //    {
        //        hyperlink.NavigateUri = new Uri(url);
        //    }
        //    return hyperlink;
        //    //return base.CreateRendererElement(aca, cellInfo);
        //}

        protected override void OnInitialize()
        {
            base.OnInitialize();
            //this.ControlText = this.GetControlText(this.CurrentStyle);
        }

        public override void OnInitializeContent(HyperlinkButton uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            this.OnUnwireUIElement(uiElement);
            TextBlock textblock = new TextBlock();
            var font = style.ReadOnlyFont;
            textblock.FontFamily = font.FontFamily;
            textblock.FontStyle = font.FontStyle;
            textblock.FontStretch = font.FontStretch;
            textblock.FontStyle = font.FontStyle;
            textblock.FontWeight = font.FontWeight;
            textblock.Foreground = style.Foreground;
            textblock.TextWrapping = style.TextWrapping;
            textblock.HorizontalAlignment = style.HorizontalAlignment;
            textblock.TextAlignment = HorizontalAlignmentToTextAlignment(style.HorizontalAlignment);
#if !WinRT
            textblock.TextDecorations = TextDecorations.Underline;
#else
            Underline ul = new Underline();
            Run r = new Run();
            r.Text = style.CellValue.ToString();
            ul.Inlines.Add(r);
            textblock.Inlines.Add(ul);
#endif
            textblock.Margin = style.TextMargins.ToThickness();
            uiElement.Background = style.Background;
            uiElement.HorizontalAlignment = style.HorizontalAlignment;
            uiElement.HorizontalContentAlignment = style.HorizontalAlignment;
            //Fix for the issue current cell border issue.
            uiElement.Margin = new Thickness(1);
            textblock.Text = GetControlText(style);

            string url = string.Empty;
            if (style.CellValue2 != null)
                url = style.CellValue2.ToString();
            string pattern = @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
            if (Regex.IsMatch(url, pattern))
            {
                uiElement.NavigateUri = new Uri(url);
            }
            else
            {
                uiElement.NavigateUri = null;
            }
            uiElement.Content = textblock;
            this.OnWireUIElement(uiElement);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, HyperlinkButton uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = new Thickness(1);
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
#if !WinRT
                margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
#endif
            }
            uiElement.Padding = margins;
            base.ArrangeUIElement(aca, uiElement, style);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        protected override void OnWireUIElement(HyperlinkButton uiElement)
        {
            uiElement.Click += new RoutedEventHandler(uiElement_Click);
        }

        protected override void OnUnwireUIElement(HyperlinkButton uiElement)
        {
            uiElement.Click -= new RoutedEventHandler(uiElement_Click);
        }

        void uiElement_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton uiElement = sender as HyperlinkButton;
            var currentUIElement = VirtualizingCellsControl.GetArrangeCellArgs(uiElement);
            var cellRowCol = currentUIElement.CellRowColumnIndex;
            //Fix for the issue current cell not set when we click the hyper link cell.
            GridControl.CurrentCell.MoveTo(cellRowCol.RowIndex, cellRowCol.ColumnIndex);
            string url = string.Empty;
            var CurrentStyle = this.GridControl.Model[cellRowCol.RowIndex, cellRowCol.ColumnIndex];
            if (CurrentStyle.CellValue2 != null)
                url = CurrentStyle.CellValue2.ToString();
            if (uiElement.NavigateUri == null && url.Contains("!"))
            {
                string sheetname = url.Substring(0, url.IndexOf('!'));
                string strcell = url.Substring(url.IndexOf('!') + 1);
                int rowIndex = GetRowIndex(strcell);
                int colIndex = ColIndex(strcell);
                CellRequestNavigateEventArgs arg = new CellRequestNavigateEventArgs(sheetname, rowIndex, colIndex, null, CurrentStyle.CellRowColumnIndex);
                this.GridControl.Model.RaiseCellRequestNavigate(arg);
            }
            else
            {
                CellRequestNavigateEventArgs arg = new CellRequestNavigateEventArgs(string.Empty, 0, 0, uiElement.NavigateUri, CurrentStyle.CellRowColumnIndex);
                this.GridControl.Model.RaiseCellRequestNavigate(arg);
            }
        }

        private int GetRowIndex(string s)
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


        protected override string GetControlTextFromEditorCore(HyperlinkButton uiElement)
        {
            return uiElement.Content.ToString();
        }

#if !WinRT
        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            // return false to indicate the CurrentCellUIElement should handle the key
            // and the grid should ignore it.
            bool isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            if (isControlKey)
                return true;

            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (this.GridControl is GridControl)
                        {
                            if (this.GridControl.Model.Options.EnterKeyBehaviour == EnterKeyBehaviour.MouseDown)
                                this.CurrentCell.MoveDown();
                            else
                                this.CurrentCell.MoveRight();
                            CurrentCell.ScrollInView();
                            e.Handled = true;
                            return true;
                        }
                        e.Handled = true;
                        this.CurrentCell.MoveRight();
                        CurrentCell.ScrollInView();
                        return true;
                    }
                    // break; Unreachable code
                case Key.Tab:
                    return true;
                case Key.Home:
                    return true;
            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#else
        public override bool ShouldGridTryToHandlePreviewKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            CoreVirtualKeyStates ctrl = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control);
            CoreVirtualKeyStates shift = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control);
            bool isControlKey = false;
            bool isShiftKey = false;

            if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                isControlKey = true;
            if (shift.HasFlag(CoreVirtualKeyStates.Down))
                isShiftKey = true;

            if (isControlKey)
                return true;

            switch (e.Key)
            {
                case VirtualKey.Enter:
                    {
                        if (this.GridControl is GridControl)
                        {
                            if (this.GridControl.Model.Options.EnterKeyBehaviour == EnterKeyBehaviour.MouseDown)
                                this.CurrentCell.MoveDown();
                            else
                                this.CurrentCell.MoveRight();
                            CurrentCell.ScrollInView();
                            e.Handled = true;
                            return true;
                        }
                        e.Handled = true;
                        this.CurrentCell.MoveRight();
                        CurrentCell.ScrollInView();
                        return true;
                    }
                    break;
                case VirtualKey.Tab:
                    return true;
                case VirtualKey.Home:
                    return true;
            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#endif
    }
}
