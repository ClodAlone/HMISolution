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
using Syncfusion.Windows.ComponentModel;
using System.Collections;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicCellRendererCollection : Disposable, ICollection
    {
        string cachedKey = "";
        IGraphicCellRenderer cachedRenderer = null;
        internal Hashtable content = new Hashtable();
        GraphicModel graphicModel;

        public GraphicCellRendererCollection(GraphicModel graphicmodel)
        {
            this.graphicModel = graphicmodel;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
                if (this.cachedRenderer != null)
                {
                    this.cachedRenderer = null;
                }
                this.content = null;
                this.graphicModel = null;
            }
            base.Dispose(disposing);
        }

        public virtual void Clear()
        {
            //foreach (object obj in content.Values)
            //{
            //    IGraphicCellRenderer control = obj as IGraphicCellRenderer;
            //}
            this.content.Clear();
            cachedKey = "";
            cachedRenderer = null;
        }

        public void CopyTo(Array array, int index)
        {
#if !SILVERLIGHT
            this.content.CopyTo(array, index);
#else
            throw new NotSupportedException();
#endif
        }

        public int Count
        {
            get { return this.content.Count; }
        }

        public bool IsSynchronized
        {
            get 
            { 
#if !SILVERLIGHT
                return this.content.IsSynchronized;
#else
                return false;
#endif
            }
        }

        public object SyncRoot
        {
            get 
            {
#if !SILVERLIGHT
                return this.content.SyncRoot;
#else
                return null;
#endif
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.content.GetEnumerator();
        }

        public virtual bool ContainsKey(string key)
        {
            return this.content.ContainsKey(key);
        }

        public virtual void Add(string key, IGraphicCellRenderer grid)
        {
            this.content.Add(key, grid);
        }

        public virtual void Remove(string key)
        {
            if (ContainsKey(key))
            {
                IGraphicCellRenderer control = content[key] as IGraphicCellRenderer;
                //if (control != null && control.CellModel != null)
                //    control.Dispose();
                this.content.Remove(key);
            }
            cachedKey = "";
            cachedRenderer = null;
        }

        public virtual IGraphicCellRenderer this[string key]
        {
            set
            {
                if (this.ContainsKey(key) && content[key] != value)
                    this.Remove(key);
                this.Add(key, value);
            }
            get
            {
                if (key == cachedKey)
                    return cachedRenderer;

                cachedKey = key;
                if (!this.ContainsKey(key))
                {
                    cachedKey = key;
                    GraphicCellModelBase cellModel = graphicModel.CellModels[key];
                    cachedRenderer = cellModel.CreateRenderer();
                    cachedRenderer.GridControl = graphicModel.GridControl;
                    this.Add(cachedKey, cachedRenderer);
                }
                else
                    cachedRenderer = (IGraphicCellRenderer)content[key];
                return cachedRenderer;
            }
        }
    }
}
