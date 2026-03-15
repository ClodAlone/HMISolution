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

namespace Syncfusion.Windows.Controls.Grid
{
    public class ArrangedGraphicCellManager : IDisposable
    {
        private GraphicCellUIElementsDictionary aliveGraphicCellUIElements = new GraphicCellUIElementsDictionary();
        private GraphicCellUIElementsDictionary unloadGraphicCellUIElements;

        internal void PrepareArrangeGraphicCell()
        {
            unloadGraphicCellUIElements = this.aliveGraphicCellUIElements;
            aliveGraphicCellUIElements = new GraphicCellUIElementsDictionary();
        }

        internal GraphicCellUIElement PreArrangeGraphicCell(int index)
        {
            GraphicCellUIElement cellUIElements;
            if (unloadGraphicCellUIElements != null && unloadGraphicCellUIElements.TryGetValue(index, out cellUIElements))
            {
                unloadGraphicCellUIElements.Remove(index);
                return cellUIElements;
            }
            return null;
        }

        internal void PostArrangeGraphicCell(int index, GraphicCellUIElement graphicCellUIElement)
        {
            if (!aliveGraphicCellUIElements.ContainsKey(index))
                aliveGraphicCellUIElements.Add(index, graphicCellUIElement);
        }

        internal void ConcludeArrange()
        {
            foreach (KeyValuePair<int, GraphicCellUIElement> entry in unloadGraphicCellUIElements)
            {
                entry.Value.Renderer.UnloadUIElements(entry.Key, entry.Value);
            }
            unloadGraphicCellUIElements.Clear();
            unloadGraphicCellUIElements = null;
        }

        public void Invalidate(int index)
        {
            if (aliveGraphicCellUIElements != null)
                aliveGraphicCellUIElements.Invalidate(index);
        }

        public void InvalidateVisual()
        {
            if (aliveGraphicCellUIElements != null)
            {
                aliveGraphicCellUIElements.InvalidateVisual();
            }
        }

        public void Dispose()
        {
            if (this.aliveGraphicCellUIElements != null)
            {
                this.aliveGraphicCellUIElements.Clear();
                this.aliveGraphicCellUIElements = null;
            }
            if (this.unloadGraphicCellUIElements != null)
            {
                this.unloadGraphicCellUIElements.Clear();
                this.unloadGraphicCellUIElements = null;
            }
        }
    }
}
