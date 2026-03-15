#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if WinRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
    /// <summary>
    /// This class implements a cache of UIElement of the given type parameter T. It is used
    /// by the <see cref="GridVirtualizingCellRendererBase{T}"/> renderer to recycle UIElement 
    /// elements for cells that were scrolled out of view and delay unloading of UIElements.
    /// This reduces the number of times the UIElement needs to be created or unloaded and
    /// instead only the contents of the UIElement will be reinitialized with cell contents. <para/>
    /// </summary>
    /// <typeparam name="T">Type of the Element</typeparam>
    [ClassReference(IsReviewed = false)]
    public class VirtualizingCellUIElementBin<T> : Dictionary<GridCellRendererBase, Queue<WeakReference>>
        where T : UIElement
    {
        #region Property

        /// <summary>
        /// Gets the <see cref="System.Collections.Generic.Queue{WeakReference}"/> for the specified Renderer.
        /// </summary>
        /// <value></value>
        public new Queue<WeakReference> this[GridCellRendererBase renderer]
        {
            get
            {
                if (ContainsKey(renderer))
                    return base[renderer];

                Queue<WeakReference> queue = base[renderer] = new Queue<WeakReference>();
                return queue;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Enqueues the specified UI element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        public void Enqueue(GridCellRendererBase renderer, T uiElement)
        {
            this[renderer].Enqueue(new WeakReference(uiElement));
            uiElement.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Dequeues an UIElement from the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <returns></returns>
        public T Dequeue(GridCellRendererBase renderer)
        {
            if (!ContainsKey(renderer))
                return default(T);

            Queue<WeakReference> queue = base[renderer];
            if (queue.Count == 0)
                return default(T);

            var el = queue.Dequeue().Target as T;

            if (el == null)
                return default(T);
            
            el.Visibility = Visibility.Visible;
            
            return el;
        }

        #endregion
    }
}
