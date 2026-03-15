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
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Stencil;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input; 
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#else
using System.Windows.Input;
using PointerRoutedEventArgs = System.Windows.Input.MouseEventArgs;
using TappedRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    internal sealed class AddedEvent<T> : Event<T>
    {
    }
    internal sealed class AddingEvent<T> : Event<T>
    {

    }
    internal sealed class DeletedEvent<T> : Event<T>
    {
    }

    internal sealed class BoundsChangedEvent : Event<ChangeArgs<Rect>>
    {
    }

    internal sealed class PageBoundsChangedEvent : Event<ChangeArgs<Rect>>
    {
    }

    internal sealed class ViewportChangedEvent : Event<ChangeArgs<Rect>>
    {
    }

    //bool:true - Measure page
    //bool:false - Arrange page
    internal sealed class MeasuringArrangingEvent : Event<bool>
    {
    }

    internal sealed class SelectionRectangleChangedEvent : Event<Rect>
    {
    }

    internal sealed class SelectedEvent<T> : Event<SelectionArgs<T>>
    {
    }

    internal sealed class UnSelectedEvent<T> : Event<SelectionArgs<T>>
    {
    }
    
    internal sealed class QuickViewAddedEvent : Event<IDiagramElement>
    {
        
    }

    internal sealed class DragStartingEvent : Event<ManipulationDragStartingArgs>
    {
    }

    internal sealed class NodeDragStartingEvent : Event<PointerRoutedEventArgs>
    {
    }

    internal sealed class DragEvent : Event<ManipulationDragEventArgs>
    {
    }

    internal sealed class NodeDragEvent : Event<NodeDragDeltaEventArgs>
    {
    }

    internal sealed class SymbolDroppedEvent : Event<SymbolDropArgs>
    {
    }

     internal sealed class DrawStartedEvent : Event<DrawParameter>
    {
    }

    internal sealed class DrawingCompletedEvent : Event<DrawParameter>
    {
    }

    sealed class CollectionArgs<T>
    {
        public T Element { get; private set; }

        public ElementType ElementType { get; private set; }

        public SourceType SourceType { get; private set; }

        public ItemSource ItemSource { get; private set; }

        public object Sender { get; private set; }

        public CollectionArgs(object sender, T element, ElementType elementType, SourceType srcType, ItemSource source)
        {
            Sender = sender;
            Element = element;
            ElementType = elementType;
            SourceType = srcType;
            ItemSource = source;
        }
    }

    sealed class ChangeArgs<TArgs>
    {
        public TArgs OldValue { get; private set; }

        public TArgs NewValue { get; private set; }

        public object Sender { get; private set; }

        public ChangeArgs(object sender, TArgs oldValue, TArgs newValue)
        {
            Sender = sender;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    sealed class PropChangedEventArgs<T>
    {
        public object OldValue { get; private set; }

        public object NewValue { get; private set; }

        public object Sender { get; private set; }

        public string PropertyName { get; private set; }
        public PropChangedEventArgs(object sender, object oldValue, object newValue, string name)
        {
            Sender = sender;
            OldValue = oldValue;
            NewValue = newValue;
            PropertyName = name;
        }
    }
    internal sealed class SelectionArgs<T>
    {
        public bool ClearSelection { get; private set; }

        public T Source { get; private set; }

        public SelectionArgs(T source, bool clearSelection)
        {
            Source = source;
            ClearSelection = clearSelection;
        }
    }

    public interface IDrawParameter
    {
        DrawingTool Tool { get; }
        Point? Point { get; }
        object Node { get; }
        object Port { get; }
        MouseEventArgs PressedEventArgs { get; }
        NullSourceTarget NullSourceTarget { get; }
    }

    [Flags]
    public enum NullSourceTarget
    {
        None = 0,
        SelectionAsSource = 1,
        CloneSourceAsTarget = 1 << 1,
        Default = None
    }

    public class DrawParameter : IDrawParameter
    {
        public DrawingTool Tool { get; private set; }
        public Point? Point { get; private set; }
        public object Node { get; private set; }
        public object Port { get; private set; }
        public PointerRoutedEventArgs PressedEventArgs { get; private set; }
        public NullSourceTarget NullSourceTarget { get; private set; }

        public DrawParameter()
        {
        }

        public DrawParameter(DrawingTool tool,
            PointerRoutedEventArgs args, 
            Point? point = null,
            object node = null, 
            object port = null,
            NullSourceTarget nullSourceTarget = NullSourceTarget.Default)
        {
            Tool = tool;
            Point = point;
            Node = node;
            Port = port;
            PressedEventArgs = args;
            NullSourceTarget = nullSourceTarget;
        }
    }

    internal sealed class SymbolDropArgs
    {
        public ISymbol Symbol { get; private set; }

        public PointerRoutedEventArgs PointerArgs { get; private set; } 

        public Point? Position { get; set; }

        public SymbolDropArgs(ISymbol symbol,PointerRoutedEventArgs args)
        {
            Symbol = symbol;
            PointerArgs = args; 
        }
    }

    //internal sealed class DrawArgs
    //{
    //    public DrawingTool Tool { get; private set; }
    //    public Point? Point { get; private set; }
    //    public object Node { get; private set; }
    //    public object Port { get; private set; }
    //    public PointerRoutedEventArgs PressedEventArgs { get; private set; } 

    //    public DrawArgs(DrawingTool tool, Point? point, object node, object port,PointerRoutedEventArgs args)
    //    {
    //        Tool = tool;
    //        Point = point;
    //        Node = node;
    //        Port = port;
    //        PressedEventArgs = args;  
    //    }
    //}
}
