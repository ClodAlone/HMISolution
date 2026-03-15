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
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
#if WINRT_USING
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls;
using System.Windows.Media; 
using PointerRoutedEventArgs = System.Windows.Input.MouseEventArgs;
using TappedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif
using ScrollViewer = Syncfusion.UI.Xaml.Diagram.Controls.ScrollViewer;

namespace Syncfusion.UI.Xaml.Diagram.Stencil
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public sealed partial class Symbol : ContentControl
    {

        #region PrivateVariables
        internal Stencil _mStencil;
        ScrollViewer _mCurrentScrollviewer = null;
        #endregion

        public Symbol()
        {
            this.DefaultStyleKey = typeof(Symbol);
#if TOUCH
            this.ManipulationMode = ManipulationModes.All;
#endif
#if WINRT
            this.PointerPressed += Symbol_PointerPressed;
            this.PointerMoved += Symbol_PointerMoved;
            this.PointerReleased += Symbol_PointerReleased;
            this.Tapped += Symbol_Tapped;
#else
            this.MouseLeftButtonDown += Symbol_PointerPressed;
            this.MouseMove += Symbol_PointerMoved;
            this.MouseLeftButtonUp += Symbol_PointerReleased; 
#endif
        }

        void Symbol_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }
        private void DoApplyTemplate()
        {
            _mStencil = this.FindVisualParent<Stencil>();
            base.OnApplyTemplate();
        }

        private bool _mCaptured = false;
        private ScrollViewer page = null;
        private void setScrollViewer(ScrollViewer scroll)
        {
            if (page != scroll)
            {
                if (page != null)
                {
                    page._mSharedData.ClearDragDropPreview();
                }
                page = scroll;
            }
        }

        void Symbol_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _mCaptured = false;
            this.ReleasePointerCapture(e);
            this.GoToState(true, "Normal");
            if (_mStencil != null)
            {
                _mStencil.SelectedSymbol = null;
            }
            DragObject<ISymbol> clone = DiagramDragDrop<ISymbol>.GetData();
            if (clone != null)
            {
                DiagramDragDrop<ISymbol>.DoDrag(null);
                UIElement uiElement = null;
                var pointerOver = uiElement.FindElementsInHostCoordinates(e.GetCurrentPoint(null).Position);
                foreach (var element in pointerOver)
                {
                    if (element is ScrollViewer)
                    {
                        setScrollViewer(element as ScrollViewer);
                        SymbolDroppedEvent evt = page._mSharedData.EventAggregator.GetEvent<SymbolDroppedEvent>();
                        SymbolDropArgs args = new SymbolDropArgs(clone.Source, e);
                        evt.Publish(args);
                        setScrollViewer(null);
                        return;
                    }
                }
            }
        }

        void GoToState(bool useTransitions, string name)
        {
            VisualStateManager.GoToState(this, name, useTransitions);
        }


        TranslateTransform tt = new TranslateTransform();
        private void Symbol_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_mCaptured)
            {
                UIElement uiElement = null;
                var pointerOver = uiElement.FindElementsInHostCoordinates(e.GetCurrentPoint(null).Position);
                foreach (var element in pointerOver)
                {
                    if (element is ScrollViewer)
                    {
                        setScrollViewer(element as ScrollViewer);
                        SymbolDroppingEventArgs args = new SymbolDroppingEventArgs() { SymbolDropMode = SymbolDropMode.Drop };
                        (page._mSharedData.Graph as SfDiagramWrapper).OnSymbolDroppingEvent(args);
                        if (args.SymbolDropMode == SymbolDropMode.Drop)
                        {
                            DragObject<ISymbol> clone = DiagramDragDrop<ISymbol>.GetData();
                            if (clone != null)
                            {
                                SymbolDropArgs _sargs=new SymbolDropArgs(clone.Source,e);
                                IInternalNode node = page._mSharedData.Page.AddNewNode(_sargs);
                                if (node.View== null)
                                {
                                    page._mSharedData.VirtualizingController.Realize(node);
                                }
                                if (node.View != null)
                                {
                                    (node.View as Node).SetInitialLocation(_sargs.Position.Value);
                                    ReleaseCapture(e);
                                    (node.View as UIElement).CapturePointer(e);
                                    (node.View as Node).Manual_PointerPressed(e);
                                }

                                return;
                            }
                        }
                        else if (args.SymbolDropMode == SymbolDropMode.Cancel)
                        {
                            ReleaseCapture(e);
                            return;
                        }
                        
                        if (_mCurrentScrollviewer == null)
                        {
                            _mCurrentScrollviewer = page;
                        }
                        if (_mStencil.Constraints.Contains(StencilConstraints.ShowPreview))
                        {
                            if (_mCurrentScrollviewer != page)
                            {
                                _mCurrentScrollviewer._mSharedData._mPreview.Content = null;
                                _mCurrentScrollviewer = page;
                            }
                            page._mSharedData.SetDragDropPreview(_mStencil.SymbolPreview);

                            tt = new TranslateTransform();
                            tt.X = e.GetCurrentPoint(page).Position.X;
                            tt.Y = e.GetCurrentPoint(page).Position.Y;
                            (page._mSharedData._mPreview as ContentPresenter).RenderTransform = tt;
                        } 
                        return;
                    }
                }
            }
            setScrollViewer(null);
        }

        private void ReleaseCapture(PointerRoutedEventArgs e)
        {
            this.ReleasePointerCapture(e);
            setScrollViewer(null);
            this.GoToState(true, "Normal");
            _mCaptured = false;
        }

        void Symbol_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!_mCaptured && this.CapturePointer(e))
            {
                _mCaptured = true;
                this.GoToState(true, "Pressed");
                if (_mStencil != null)
                {
                    _mStencil.SelectedSymbol = this;
                }
                ISymbol clone = (this.DataContext as ISymbol).Clone();
                DragObject<ISymbol> dragObject = new DragObject<ISymbol>(clone);
                DiagramDragDrop<ISymbol>.DoDrag(dragObject);
            }            
        }
    }
}
