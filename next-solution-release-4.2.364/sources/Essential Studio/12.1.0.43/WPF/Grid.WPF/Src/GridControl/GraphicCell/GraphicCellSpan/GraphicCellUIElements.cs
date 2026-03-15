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
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicCellUIElement
    {
        UIElement uiElement = null;
        IGraphicCellRenderer renderer = null;
        bool isDirty = false;

        public IGraphicCellRenderer Renderer
        {
            get
            {
                return renderer;
            }
            internal set
            {
                renderer = value;
            }
        }

        public UIElement UIElement
        {
            get
            {
                return uiElement;
            }
        }

        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        public GraphicCellUIElement(UIElement uiElement, IGraphicCellRenderer renderer)
        {
            this.uiElement = uiElement;
            this.renderer = renderer;
        }
    }

    public class GraphicCellUIElementsDictionary : Dictionary<int, GraphicCellUIElement>
    {
        public GraphicCellUIElementsDictionary()
        {

        }

        public void Invalidate(int index)
        {
            if (this.ContainsKey(index))
            {
                this[index].IsDirty = true;
            }
        }

        public void InvalidateVisual()
        {
            foreach (KeyValuePair<int,GraphicCellUIElement> item in this)
            {
                item.Value.IsDirty = true;
            }
        }
    }
}
