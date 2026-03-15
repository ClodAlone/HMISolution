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
#if WINRT_USING
using System.Threading.Tasks; 
#endif

namespace Syncfusion.UI.Xaml.Diagram.Utility
{
    static class DiagramDragDrop<T>
    {
        private static DragObject<T> Data { get; set; }

        public static void DoDrag(DragObject<T> data)
        {
            Data = data;
        }

        public static DragObject<T> GetData()
        {
            return Data;
        }
    }

    public class DragObject<T>
    {
        public T Source { get; set; }

        public DragObject(T clone)
        {
            Source = clone;
        }
    }
}
