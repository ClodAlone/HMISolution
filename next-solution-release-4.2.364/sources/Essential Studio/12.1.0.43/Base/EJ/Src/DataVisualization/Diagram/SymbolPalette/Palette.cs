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
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.DataVisualization.Models.Controls
{
    public class Palette
    {
        #region Members
        private Collection _items;
        private string _strName;
        private bool _bExpanded;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="Palette"/> class.
        /// </summary>
        /// <param name="paletteName">The palette Name.</param>
        public Palette(string paletteName)
        {
            _items = new Collection();
            _strName = paletteName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Palette"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public Palette(Palette src)
        {
            this._items = src._items;
            this.Capacity = src.Capacity;
            this._strName = src._strName;
        }

         
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Palette"/> is expanded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if expanded; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("expanded")]
        [DefaultValue(false)]
        public bool Expanded
        {
            get
            {
                return _bExpanded;
            }
            set
            {
                if (value != _bExpanded)
                    _bExpanded = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the palette.
        /// </summary>
        /// <value>
        /// The palette name.
        /// </value>
        [JsonProperty("name")]
        [DefaultValue("")]
        public string Name
        {
            get
            {
                return _strName;
            }
            set
            {
                if (value != _strName)
                    _strName = value;
            }
        }

        /// <summary>
        /// Gets the palette children.
        /// </summary>
        [JsonProperty("items")]
        public Collection Items
        {
            get
            {
                return _items;
            }
        }

        /// <summary>
        /// Gets the palette child count.
        /// </summary>
        public int ChildCount
        {
            get
            {
                return this._items.Count;
            }
        }

        /// <summary>
        /// Gets or sets the capacity of palette.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity
        {
            get
            {
                return this._items.Members.Capacity;
            }
            set
            {
                this._items.Members.Capacity = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Appends the node to the palette collection.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AppendChild(NodeBase node)
        {
            if (node != null && !this._items.Contains(node))
            {
                this._items.Add(node);
            }
        }
        /// <summary>
        /// Appends the connector to the palette collection.
        /// </summary>
        /// <param name="connector">The connector.</param>
        public void AppendChild(Connector connector)
        {
            if (connector != null && !this._items.Contains(connector))
            {
                this._items.Add(connector);
            }
        }

        /// <summary>
        /// Removes the specified child.
        /// </summary>
        /// <param name="node">The node.</param>
        public void RemoveChild(NodeBase node)
        {
            if (this._items.Contains(node))
            {
                this._items.Remove(node);
            }
        }

        /// <summary>
        /// Removes the specified child.
        /// </summary>
        /// <param name="connector">The connector.</param>
        public void RemoveChild(Connector connector)
        {
            if (this._items.Contains(connector))
            {
                this._items.Remove(connector);
            }
        }

        /// <summary>
        /// Removes the child at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveChild(int index)
        {
            if (index >= 0 && index <= this._items.Count - 1)
            {
                this._items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clears the palette children.
        /// </summary>
        public void Clear()
        {
            this._items.Clear();
        }

        /// <summary>
        /// Determines whether the palette contains the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   <c>true</c> if the palette contains the specified node; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(NodeBase node)
        {
            return this._items.Contains(node);
        }

        /// <summary>
        /// Determines whether the palette contains the specified connector.
        /// </summary>
        /// <param name="connector">The connector.</param>
        /// <returns>
        ///   <c>true</c> if palette contains the specified connector; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Connector connector)
        {
            return this._items.Contains(connector);
        }

        /// <summary>
        /// Gets the child at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The child.</returns>
        public object GetChild(int index)
        {
            if (index >= 0 && index <= this._items.Count - 1)
            {
                return this._items[index];
            }
            return null;
        }

         
        
        #endregion

       
        public object Clone()
        {
            return new Palette(this);
        }

        
    } 
}

