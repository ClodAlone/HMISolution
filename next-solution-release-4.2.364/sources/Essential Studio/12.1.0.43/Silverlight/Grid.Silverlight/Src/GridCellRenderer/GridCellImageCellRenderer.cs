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
using System.IO;

#if!WinRT
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Grid
{
#else
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;
using Windows.System;
namespace Syncfusion.WinRT.Controls.Grid
{
#endif
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellImageCellModel : GridCellModel<GridCellImageCellRenderer>
    {
        public GridCellImageCellModel()
        {
        }
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellImageCellRenderer : GridVirtualizingCellRenderer<ContentControl>
    {
        public GridCellImageCellRenderer()
        {
            this.AllowRecycle = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.IsControlTextShown = true;
            this.SupportsRenderOptimization = true;
            this.AllowTransparentBackground = false;
        }

        protected override UIElement CreateRendererElement(Cells.ArrangeCellArgs aca, GridRenderStyleInfo cellInfo)
        {
            ContentControl contentControl = new ContentControl();
            Image image = new Image();
            BitmapImage bitImg = cellInfo.CellValue as BitmapImage;
            image.Source = bitImg;
            image.Stretch = Stretch.Fill;
#if!WinRT
            image.Stretch = cellInfo.ImageCell.Stretch;
#else
            image.Height = aca.CellClipRect.Height;
            image.Width = aca.CellClipRect.Width;
#endif
            Thickness margins = cellInfo.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);
            contentControl.Padding = margins;
            contentControl.HorizontalContentAlignment = cellInfo.HorizontalAlignment;
            contentControl.VerticalContentAlignment = cellInfo.VerticalAlignment;
            contentControl.Content = image;
            return contentControl;
        }

        public override void OnInitializeContent(ContentControl uiElement, GridRenderStyleInfo style)
        {
            Image image = new Image();
            BitmapImage bitImg = style.CellValue as BitmapImage;
            image.Source = bitImg;
            image.Stretch = Stretch.Fill;
#if!WinRT
            image.Stretch = style.ImageCell.Stretch;
#else
            //image.Height = 
            //image.Width = aca.CellClipRect.Width;       
#endif
            //Thickness margins = style.TextMargins.ToThickness();
            //margins.Left = Math.Max(0, margins.Left - 2);
            //margins.Right = Math.Max(0, margins.Right - 2);
            //uiElement.Padding = margins;
            uiElement.HorizontalContentAlignment = style.HorizontalAlignment;
            uiElement.VerticalContentAlignment = style.VerticalAlignment;
            uiElement.Content = image;
        }
#if WinRT
        public override bool ShouldGridTryToHandlePreviewKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            bool isControlKey=false;
            CoreVirtualKeyStates ctrl = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control);
            if (ctrl == CoreVirtualKeyStates.Down)
                isControlKey = true;

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
#else
        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {            
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
#endif
        protected override string GetControlTextFromEditorCore(ContentControl uiElement)
        {
            return uiElement.Content.ToString();
        }

        //protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, ContentControl uiElement, GridRenderStyleInfo style)
        //{
        //    //aca.CellClipRect = new Rect(aca.CellClipRect.Top, aca.CellClipRect.Left, aca.CellClipRect.Width + 400, aca.CellClipRect.Height + 400);
        //    Rect r = aca.CellClipRect;
        //    aca.CellRect = new Rect(aca.CellRect.Left, aca.CellRect.Top, aca.CellRect.Width + 400, aca.CellRect.Height + 400);
        //    base.ArrangeUIElement(aca, uiElement, style);
        //}
    }
}
