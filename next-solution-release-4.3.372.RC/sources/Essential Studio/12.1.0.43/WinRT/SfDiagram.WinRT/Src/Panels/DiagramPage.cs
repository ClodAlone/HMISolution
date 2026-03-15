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
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes; 
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes; 
#endif
using Syncfusion.UI.Xaml.Diagram.Utility;
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using ScrollViewer = Syncfusion.UI.Xaml.Diagram.Controls.ScrollViewer;

namespace Syncfusion.UI.Xaml.Diagram.Panels
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public sealed class DiagramPage : Panel
    {
        internal SharedData SharedData;
        private MeasuringArrangingEvent _mMeasureArrangeEvent;

        internal void SetSharedData(SharedData data)
        {
            SharedData = data;
            SymbolDroppedEvent evt = SharedData.EventAggregator.GetEvent<SymbolDroppedEvent>();
            _mMeasureArrangeEvent = SharedData.EventAggregator.GetEvent<MeasuringArrangingEvent>();
            evt.Subscribe(DropSymbol);

            Rectangle selectionRectangle = new Rectangle()
                {
                    Stroke = new SolidColorBrush(new Color() { R = 51, G = 153, B = 255, A = 255 }),
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(new Color() { R = 51, G = 153, B = 255, A = 19 }),
                    Width = 0,
                    Height = 0,
                };
            Canvas.SetZIndex(selectionRectangle, 10000);
            this.Children.Add(selectionRectangle);
            SharedData.selectionRectangle = selectionRectangle;
            SharedData.Page = this;
        }

    
        internal void DropSymbol(SymbolDropArgs args)
        {
            AddNewNode(args);
        }

        internal IInternalNode AddNewNode(SymbolDropArgs args)
        {
            Point position = args.PointerArgs.GetCurrentPoint(this).Position;

            if (args.Position == null)
            {
                args.Position = position;
            }

            IGraphInternal graph = SharedData.Graph;

            graph.InternalSelectedItems.ClearSelection();
            if (SharedData.Graph.InternalNodes != null)
            {
                object node = graph.GetNewItem(ElementType.Node, SharedData.Graph.InternalNodes.ItemType);
                IInternalNode internalNode = graph.InternalNodes.GetNewWrapper(node, true);
                graph.InternalNodes.Add(internalNode, ItemSource.Stencil);
                //var internalNode = (this as IGraphInternal).GetNodeWrapper(node, true);
                //InternalNodes.Add(internalNode);
                //if (Nodes is ICollection<TNode> && !(Nodes as ICollection<TNode>).IsReadOnly)
                {
                    //(Nodes as ICollection<TNode>).Add((TNode)(object)node);
                }
                internalNode.OffsetX = args.Position.Value.X;
                internalNode.OffsetY = args.Position.Value.Y;
                internalNode.Content = args.Symbol.Symbol;
                internalNode.ContentTemplate = args.Symbol.SymbolTemplate;
                //internalNode.UnitWidth = SharedData._mPreview.Width;
                //internalNode.UnitHeight = SharedData._mPreview.Height;
                internalNode.IsSelected = true;
                return internalNode;
            }
            return null;
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            _mMeasureArrangeEvent.Publish(true);
            List<IGroup> gr = new List<IGroup>();
            List<IConnector> con = new List<IConnector>();

            foreach (UIElement child in Children)
            {
                if (child is IGroup)
                {
                    gr.Add(child as IGroup);
                }
                else if (child is INode)
                {
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
                else if (child is IConnector)
                {
                    con.Add(child as IConnector);
                }
                else
                {
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));                    
                }
            }

            foreach (UIElement child in gr)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            foreach (UIElement child in con)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _mMeasureArrangeEvent.Publish(false);

            List<IGroup> gr = new List<IGroup>();
            List<IConnector> con = new List<IConnector>();

            foreach (UIElement child in Children)
            {
                if (child is IGroup)
                {
                    gr.Add(child as IGroup);
                }
                else if (child is INode)
                {
                    child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
                }
                else if (child is IConnector)
                {
                    con.Add(child as IConnector);
                }
                else
                {
                    child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));                    
                }
            }

            foreach (UIElement child in gr)
            {
                child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
            }

            con.Sort((a, b) =>
                {
                    return a.ZIndex - b.ZIndex;
                });
            foreach (UIElement child in con)
            {
                child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
            }
            return finalSize;
        }
    }
}
