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
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicCellModelCollection : Disposable, ICollection, ICloneable
#if !SILVERLIGHT && !ENABLE_PARTIAL_TRUST
        ,ISerializable
#endif
    {
        GraphicModel graphicModel;
        string defaultKey = "";
        internal Hashtable content = new Hashtable();

        public GraphicModel GraphicModel
        {
            get { return graphicModel; }
        }

        public GridControlBase GridControl
        {
            get
            {
                if (graphicModel != null)
                    return graphicModel.GridControl;
                return null;
            }
        }

        public GraphicCellModelCollection(GraphicModel graphicModel)
        {
            this.graphicModel = graphicModel;
        }
#if !SILVERLIGHT
        protected GraphicCellModelCollection(SerializationInfo info, StreamingContext context)
        {
            content = new Hashtable();
            SerializationInfoEnumerator sie = info.GetEnumerator();
            while (sie.MoveNext())
            {
                if (sie.Name != "DefaultCellType")
                    content[sie.Name] = sie.Value as GraphicCellModelBase;
                else
                    defaultKey = sie.Value as string;
            }
        }

#if !SyncfusionFramework4_0
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
#else
        [SecurityCritical]
#endif
#if !ENABLE_PARTIAL_TRUST

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (content != null)
            {
                foreach (string cellTypeName in content.Keys)
                    info.AddValue(cellTypeName, content[cellTypeName]);
            }
            info.AddValue("DefaultCellType", defaultKey); // GraphicCellModelBase
        }
#endif
#endif

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// The default cell type to be used for cells where a specific cell type could not be loaded.
        /// </summary>
        public string DefaultCellType
        {
            get
            {
                return defaultKey;
            }
            set
            {
                if (!this.ContainsKey(value))
                    throw new ArgumentException(value + " not found in collection.", value);
                defaultKey = value;
            }
        }

        public virtual IEnumerator GetEnumerator()
        {
            return this.content.GetEnumerator();
        }

        public virtual void Remove(string cellTypeName)
        {
            GraphicCellModelBase cellModel = content[cellTypeName] as GraphicCellModelBase;
            if (cellModel != null)
            {
                cellModel.Dispose();
                cellModel.GraphicModel = null;
            }

            // TODO: Reset cellModel.GridModel
            this.content.Remove(cellTypeName);
            if (cellTypeName == defaultKey)
                defaultKey = "";
        }

        public virtual void CopyTo(GraphicCellModelBase[] array, int index)
        {
#if !SILVERLIGHT
            this.content.CopyTo(array, index);
#else
            throw new NotSupportedException();
#endif
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GraphicCellModelBase[])array, index);
        }

        public virtual bool ContainsValue(GraphicCellModelBase cellModel)
        {
            return this.content.ContainsValue(cellModel);
        }

        public virtual bool ContainsKey(string cellTypeName)
        {
            return this.content.ContainsKey(cellTypeName);
        }

        public virtual void Add(string cellTypeName, GraphicCellModelBase cellModel)
        {
            this.content.Add(cellTypeName, cellModel);

            if (String.IsNullOrEmpty(defaultKey))
                defaultKey = cellTypeName;

            cellModel.GraphicModel = this.graphicModel;
        }

        public virtual ICollection Values
        {
            get
            {
                return this.content.Values;
            }
        }



        public virtual object SyncRoot
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


        public virtual bool IsSynchronized
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


        public virtual ICollection Keys
        {
            get
            {
                return this.content.Keys;
            }
        }

        public virtual GraphicCellModelBase this[string cellTypeName]
        {
            set
            {
                if (this.ContainsKey(cellTypeName) && content[cellTypeName] != value)
                    this.Remove(cellTypeName);
                this.Add(cellTypeName, value);
            }
            get
            {
                if (!this.ContainsKey(cellTypeName))
                {
                    try
                    {
                        GraphicQueryCellModelEventArgs qe = new GraphicQueryCellModelEventArgs(graphicModel, cellTypeName);
                        graphicModel.RaiseGraphicQueryCellModel(qe);
                        if (qe.CellModel != null)
                        {
                            if (!this.ContainsKey(qe.CellType))
                                this.Add(qe.CellType, qe.CellModel);
                            return qe.CellModel;
                        }
                        cellTypeName = defaultKey;
                    }
                    finally
                    {
                    }
                }
                else if (String.IsNullOrEmpty(defaultKey))
                    defaultKey = cellTypeName;

                return (GraphicCellModelBase)this.content[cellTypeName];
            }
        }

        public virtual int Count
        {
            get
            {
                return this.content.Count;
            }
        }

        public virtual void Clear()
        {
            // TODO: Reset cellModel.GridModel
            foreach (GraphicCellModelBase cellModel in content.Values)
            {
                if (cellModel != null)
                {
                    cellModel.Dispose();
                    cellModel.GraphicModel = null;
                }
            }
            this.content.Clear();
            this.defaultKey = "";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (content != null)
                {
                    foreach (object obj in content.Values)
                    {
                        GraphicCellModelBase cm = obj as GraphicCellModelBase;
                        if (cm != null)
                            cm.Dispose();
                    }
                    content.Clear();
                }
                this.defaultKey = null;
                if (this.GraphicModel != null)
                    this.graphicModel = null;
            }
            base.Dispose(disposing);
        }
    }

    public sealed class GraphicQueryCellModelEventArgs
    {
        string cellType;
        GraphicCellModelBase cellModel;

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        /// <param name="cellType">The cell type identifier as used in the <see cref="GridStyleInfo.CellType"/> property.</param>
        public GraphicQueryCellModelEventArgs(GraphicModel graphicModel, string cellType)
        {
            this.cellType = cellType;
            this.cellModel = null;
        }


        /// <summary>
        /// The cell type identifier as used in the <see cref="GridStyleInfo.CellType"/> property.
        /// </summary>
        public string CellType
        {
            get
            {
                return cellType;
            }
        }

        /// <summary>
        /// The <see cref="GraphicCellModelBase"/> for the cell type. You should create a new instance
        /// of the specific cell model and save it to this property.
        /// </summary>
        public GraphicCellModelBase CellModel
        {
            get
            {
                return cellModel;
            }
            set
            {
                cellModel = value;
            }
        }
    }
}
