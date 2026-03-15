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
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicCellRendererBase<T> : IGraphicCellRenderer
        where T : FrameworkElement, new() 
    {
        GraphicCellModelBase cellModel;
        bool canCreateRenderElement = true;
        GridControlBase gridControl;
        UIElement currentUIElement;
        UIElement currentRenderElement;
        bool isEditable;
        GraphicStyleInfo currentStyle;

        public GraphicCellModelBase CellModel
        {
            get { return cellModel; }
        }

        public bool CanCreateRenderElement
        {
            get { return canCreateRenderElement; }
            set { canCreateRenderElement = value; }
        }

        public GridControlBase GridControl
        {
            get
            {
                return gridControl;
            }
            set
            {
                gridControl = value;
            }
        }

        public UIElement CurrentUIElement
        {
            get { return currentUIElement; }
        }


        public UIElement CurrentRenderElement
        {
            get
            {
                if (CanCreateRenderElement)
                    return currentRenderElement;
                else
                    return null;
            }
        }

        public GraphicStyleInfo CurrentStyle
        {
            get { return currentStyle; }
        }

        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; }
        }

        public void RaiseCreated(GraphicCellModelBase cellModel)
        {
            this.cellModel = cellModel;
        }

        #region Arrange

        void IGraphicCellRenderer.Arrange(UIElement uiElements, Rect cellRect, GraphicStyleInfo style)
        {
            OnArrange(uiElements, cellRect, style);
        }

        protected virtual void OnArrange(UIElement uiElement, Rect rect, GraphicStyleInfo style)
        {
            SetBounds(uiElement, rect, false, false);
        }

        #endregion

        #region Prepare and Initialize UIElements

        UIElement IGraphicCellRenderer.PrepareUIElements(GraphicStyleInfo cellInfo, GraphicCellSpanInfo cellSpanInfo)
        {
            UIElement renderElement;
            if (CanCreateRenderElement)
            {
                GraphicCellControl graphicCell = CreateRendererElement(cellInfo, cellSpanInfo);
                UIElement el= CreateUIElement(cellInfo);
                GraphicCellHelper.SetHandleMouseInput(el, true);
                GraphicCellHelper.SetCellSpanInfo(el, cellSpanInfo);
                GraphicCellHelper.SetGraphicCellControl(el, graphicCell);
                graphicCell.Content = el;
                graphicCell.BorderBrush = cellInfo.BorderBrush;
                graphicCell.BorderThickness = cellInfo.BorderThickness;
                graphicCell.Background = cellInfo.Background;
                ((T)el).Height = cellSpanInfo.Height;
                ((T)el).Width = cellSpanInfo.Width;
                OnInitializeContent((T)el, cellInfo);
                renderElement = graphicCell;
            }
            else
            {
                renderElement = CreateUIElement(cellInfo);
                ((T)renderElement).Height = cellSpanInfo.Height;
                ((T)renderElement).Width = cellSpanInfo.Width;
                OnInitializeContent((T)renderElement, cellInfo);
            }
            GraphicCellHelper.SetHandleMouseInput(renderElement, true);
            WireEvents(renderElement);
            return renderElement;
        }

        private GraphicCellControl CreateRendererElement(GraphicStyleInfo cellInfo, GraphicCellSpanInfo cellSpanInfo)
        {
            GraphicCellControl ctrl = new GraphicCellControl(this.gridControl, cellInfo, cellSpanInfo);
            cellInfo.GraphicCellControl = ctrl;
            cellInfo.CellName = cellSpanInfo.Name;
            GraphicCellHelper.SetCellSpanInfo(ctrl, cellSpanInfo);
            GraphicCellHelper.SetStyleInfo(ctrl, cellInfo);
            GraphicCellHelper.SetGraphicCellRenderer(ctrl, this);
            return ctrl;
        }

        protected virtual T CreateUIElement(GraphicStyleInfo cellInfo)
        {
            return new T();
        }

        protected virtual void OnInitializeContent(T element, GraphicStyleInfo style)
        {

        }

        #endregion

        #region SetBounds

        void IGraphicCellRenderer.SetBounds(UIElement el, Rect rect, bool forceMeasure, bool forceArrange)
        {
            SetBounds(el, rect, forceMeasure, forceArrange);
        }

        protected virtual void SetBounds(UIElement el, Rect rect, bool forceMeasure, bool forceArrange)
        {
            if (!forceMeasure)
            {
                Rect elRect = GetBounds(el);
                Visibility visibility = rect.Height > 0 || rect.Width > 0 ? Visibility.Visible : Visibility.Collapsed;
                if (el == null || elRect.IsEmpty || GridUtil.GetSize(elRect) != GridUtil.GetSize(rect))
                    forceMeasure = true;
                // not calling arrange causes issues with nested grids. therefore proceed.
                else if (!forceArrange && !VirtualizingCellsControl.GetHasFocusWithin(el))
                {
                    if (rect == elRect)
                    {
                        if (visibility != el.Visibility)
                            el.Visibility = visibility;
                        return;
                    }
                }
            }

            VisualContainer.SetRenderBounds(el, rect);

            if (!rect.IsEmpty)
            {
                ClipUIElement(el, rect);
                el.SetValue(FrameworkElement.WidthProperty, rect.Width);
                el.SetValue(FrameworkElement.HeightProperty, rect.Height);
                Visibility visibility = rect.Height > 0 || rect.Width > 0 ? Visibility.Visible : Visibility.Collapsed;
                if (visibility != el.Visibility)
                    el.Visibility = visibility;
                if (forceMeasure)
                {
                    InvalidateMeasureRecursive(el);
                    el.Measure(GridUtil.GetSize(rect));
                }
            }
            else
            {
                el.Visibility = Visibility.Collapsed;
                rect = new Rect(0, 0, 0, 0);
            }

            el.Arrange(rect);
        }

        public static Rect GetBounds(UIElement el)
        {
            return VisualContainer.GetRenderBounds(el);
        }

        private void ClipUIElement(UIElement el, Rect rect)
        {
            Rect headerRect = gridControl.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header, GridRangeInfo.Cell(gridControl.Model.HeaderRows, gridControl.Model.HeaderColumns), true, true);
            double clipleft = -3;
            double cliptop = -3;
            bool needclip = false;
            if (headerRect.Left > rect.Left)
            {
                needclip = true;
                clipleft = headerRect.Left - rect.Left;
            }
            if (headerRect.Top > rect.Top)
            {
                needclip = true;
                cliptop = headerRect.Top - rect.Top;
            }
            if (needclip)
            {
                Rect cliprect = new Rect(clipleft, cliptop, rect.Width + 7, rect.Height + 7);
                RectangleGeometry clipGeometry = new RectangleGeometry();
                clipGeometry.Rect = cliprect;
                el.Clip = clipGeometry;
            }
            else
            {
                el.Clip = null;
            }
        }

        void InvalidateMeasureRecursive(UIElement obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                UIElement child = VisualTreeHelper.GetChild(obj, i) as UIElement;
                if (child != null)
                {
                    child.InvalidateMeasure();
#if !SILVERLIGHT
                    if (!child.IsMeasureValid)
                        InvalidateMeasureRecursive(child);
#endif
                }
            }
        }

        #endregion

        #region GetControlValue
        
        protected virtual object GetControlValueFromEditor(T uiElement)
        {
            if (this.CurrentStyle != null)
                return this.CurrentStyle.CellValue;
            return null;
        }

        #endregion

        #region Wire & UnWire Events

        private void WireEvents(UIElement element)
        {
            element.GotFocus += new RoutedEventHandler(element_GotFocus);
            element.LostFocus += new RoutedEventHandler(element_LostFocus);
            if (CanCreateRenderElement)
            {
                GraphicCellControl graphicCell = element as GraphicCellControl;
                WireEvents((T)graphicCell.Content);
            }
            else
            {
                WireEvents((T)element);
            }
        }

        protected virtual void WireEvents(T element)
        {

        }

        private void UnWireEvents(UIElement element)
        {
            element.GotFocus -= new RoutedEventHandler(element_GotFocus);
            element.LostFocus -= new RoutedEventHandler(element_LostFocus);
            if (CanCreateRenderElement)
            {
                GraphicCellControl graphicCell = element as GraphicCellControl;
                UnWireEvents((T)graphicCell.Content);
            }
            else
            {
                UnWireEvents((T)element);
            }
        }

        protected virtual void UnWireEvents(T element)
        {

        }

        void element_LostFocus(object sender, RoutedEventArgs e)
        {
            UIElement ctrl = sender as UIElement;
            if (ctrl != null && CurrentStyle != null)
            {
                this.CurrentStyle.CellValue = GetControlValueFromEditor((T)CurrentUIElement);
            }
            this.currentStyle = null;
            this.currentRenderElement = null;
            this.CellModel.GraphicModel.SetCurrentCellRenderers(null);
        }

        void element_GotFocus(object sender, RoutedEventArgs e)
        {
            UIElement ctrl = sender as UIElement;
            if (ctrl != null)
            {
                GraphicCellSpanInfo span = GraphicCellHelper.GetCellSpanInfo(ctrl);
                if (span != null)
                {
                    this.currentStyle = this.CellModel.GraphicModel[span.CellSpanIndex];
                }
                if (CanCreateRenderElement && ctrl is GraphicCellControl)
                {
                    this.currentUIElement = (ctrl as GraphicCellControl).Content;
                    this.currentRenderElement = ctrl;
                }
                else
                    this.currentUIElement = ctrl;
                this.CellModel.GraphicModel.SetCurrentCellRenderers(this);
            }
        }

        #endregion

        #region UnloadUIElements

        protected virtual void UnloadUIElements(int index, T uiElement)
        {
        }


        void IGraphicCellRenderer.UnloadUIElements(int index, GraphicCellUIElement uiElement)
        {
            foreach (var span in this.gridControl.Model.GraphicModel.SelectedGraphicCells)
            {
                var control = this.gridControl.Model.GraphicModel[span.CellSpanIndex].GraphicCellControl;
                if (canCreateRenderElement && control == (uiElement.UIElement as GraphicCellControl).Content)
                {
                    this.gridControl.Model.GraphicModel.arrangedGraphicCellManager.PostArrangeGraphicCell(index, uiElement);
                    VisualContainer.SetRenderBounds(control, new Rect(-30, -30, 1, 1));
                    control.Arrange(new Rect(-30, -30, 1, 1));
                    return;
                }
                else if (control == uiElement.UIElement as GraphicCellControl)
                {
                    this.gridControl.Model.GraphicModel.arrangedGraphicCellManager.PostArrangeGraphicCell(index, uiElement);
                    VisualContainer.SetRenderBounds(control, new Rect(-30, -30, 1, 1));
                    control.Arrange(new Rect(-30, -30, 1, 1));
                    return;
                }
            }
            if (CurrentStyle != null && CurrentStyle.GraphicCellControl == (uiElement.UIElement as GraphicCellControl))
            {
                UnWireEvents(uiElement.UIElement);
            }
            ScrollControlChildFrame oldCanvas = VisualTreeHelper.GetParent(uiElement.UIElement) as ScrollControlChildFrame;
            if (oldCanvas != null)
                oldCanvas.Children.Remove(uiElement.UIElement);
            if (CanCreateRenderElement)
                UnloadUIElements(index, (T)(uiElement.UIElement as GraphicCellControl).Content);
            else
                UnloadUIElements(index, (T)uiElement.UIElement);
        }
        #endregion

        #region ShouldTryToHandlePreviewKeyDown
        
        protected virtual bool ShouldTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Enter:
                case Key.Up:
                case Key.Down:
                    e.Handled = true;
                    break;
                case Key.Tab:
                    var control = (this.CurrentUIElement as FrameworkElement).FindParentElementOfType<GraphicCellControl>();
                    if (control != null)
                        control.Focus();
                    break;
                default:
                    break;
            }
            return true;
        }

        bool IGraphicCellRenderer.ShouldTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            return ShouldTryToHandlePreviewKeyDown(e);
        }

        #endregion
    }
}
