#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows;

#if !WinRT
using System.Windows.Media;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{
    /// <summary>
    /// This class implements a cache of UIElement of the given type parameter T. It is used
    /// by the <see cref="VirtualizingCellRendererBase{T}"/> renderer to recycle UIElement 
    /// elements for cells that were scrolled out of view and delay unloading of UIElements.
    /// This reduces the number of times the UIElement needs to be created or unloaded and
    /// instead only the contents of the UIElement will be reinitialized with cell contents. <para/>
    /// A queue is maintained for each child frame in a <see cref="ScrollControl"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class VirtualizingCellUIElementBin<T> : Dictionary<ScrollControlChildFrame, Queue<WeakReference>>
        where T : UIElement//, new()
    {
        /// <summary>
        /// Gets the <see cref="System.Collections.Generic.Queue{WeakReference}"/> for the specified canvas.
        /// </summary>
        /// <value></value>
        public new Queue<WeakReference> this[ScrollControlChildFrame canvas]
        {
            get
            {
                if (ContainsKey(canvas))
                    return base[canvas];

                Queue<WeakReference> queue = base[canvas] = new Queue<WeakReference>();
                return queue;
            }
        }

        /// <summary>
        /// Dequeues an UIElement from the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <returns></returns>
        public T Dequeue(ScrollControlChildFrame canvas)
        {
            if (!ContainsKey(canvas))
                return default(T);

            Queue<WeakReference> queue = base[canvas];
            if (queue.Count == 0)
                return default(T);

            T el = queue.Dequeue().Target as T;

            if (el == null)
                return default(T);
            if (el.Visibility == Visibility.Collapsed)
                el.Visibility = Visibility.Visible;
            return el;
        }

        //public T DequeueOrCreate(ScrollControlChildFrame canvas, bool allowRecycle)
        //{
        //    T uiElement;
        //    if (allowRecycle)
        //    {
        //        uiElement = Dequeue(canvas);

        //        if (uiElement != null)
        //            return uiElement;
        //    }

        //    uiElement = new T();

        //    // Add control to canvas frame
        //    canvas.Children.Add(uiElement);

        //    return uiElement;
        //}

        /// <summary>
        /// Enqueues the specified UI element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        public void Enqueue(T uiElement)
        {
            ScrollControlChildFrame canvas = VisualTreeHelper.GetParent(uiElement) as ScrollControlChildFrame;
            if (canvas != null)
                this[canvas].Enqueue(new WeakReference(uiElement));
            uiElement.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Unloads all UIElement elements maintained by this cache.
        /// </summary>
        public void Unload()
        {
            foreach (KeyValuePair<ScrollControlChildFrame, Queue<WeakReference>> entry in this)
            {
                foreach (WeakReference wr in entry.Value)
                {
                    UIElement visual = wr.Target as UIElement;
                    if (visual != null)
                        entry.Key.Children.Remove(visual);
                }
            }
        }
    }
}

