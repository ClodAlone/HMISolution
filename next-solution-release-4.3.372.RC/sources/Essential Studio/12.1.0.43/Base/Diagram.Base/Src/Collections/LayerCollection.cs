#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A collection of <see cref="Syncfusion.Windows.Forms.Diagram.Layer"/> objects.
    /// </summary>
    [Serializable]
    [Description("Collection of layers.")]
    [DefaultProperty("Item")]
    public class LayerCollection
        : CollectionEx
    {
        #region Class contants
        /// <summary>
        /// Default new layer item name.
        /// </summary>
        protected const string c_strLAYER = "Layer";
        #endregion

        #region Class nested classes
        /// <summary>
        /// Implement enumerator for <see cref="Syncfusion.Windows.Forms.Diagram.LayerCollection"/> object.
        /// </summary>
        private sealed class LayerEnumerator
            : IEnumerator
        {
            #region Class members
            private LayerCollection m_collection;
            private int m_nIndex;
            private int m_nCollectionMembers;
            #endregion

            #region Class initialize/finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="LayerEnumerator"/> class.
            /// </summary>
            /// <param name="layers">The layers.</param>
            internal LayerEnumerator(LayerCollection layers)
            {
                if (layers == null)
                    throw new ArgumentNullException("layers");

                m_collection = layers;
                m_nIndex = -1;
                m_nCollectionMembers = m_collection.Count;
            }
            #endregion

            #region IEnumerator
            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <value></value>
            /// <returns>The current element in the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.-or- The collection was modified after the enumerator was created.</exception>
            public object Current
            {
                get { return m_collection[m_nIndex]; }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                if (m_collection.Count != m_nCollectionMembers)
                    throw new InvalidOperationException("collection was modified");

                return ++m_nIndex < m_collection.Count;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                m_nIndex = -1;
            }
            #endregion
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LayerCollection"/> class.
        /// </summary>
        public LayerCollection()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public LayerCollection(object owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerCollection"/> class.
        /// </summary>
        /// <param name="src">The source instance.</param>
        public LayerCollection(LayerCollection src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerCollection"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        public LayerCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the collection container.
        /// </summary>
        /// <value>The container.</value>
        public object Container
        {
            get { return this.Owner; }
            set { this.Owner = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Diagram.Layer"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <value>The layer at the specified index.</value>
        public Layer this[int index]
        {
            get { return this.Members[index] as Layer; }
            set { Set(index, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Diagram.Layer"/> with the specified layer name.
        /// </summary>
        /// <param name="strLayerName">The layer name.</param>
        /// <value>The layer with the specified name.</value>
        public Layer this[string strLayerName]
        {
            get 
            { 
                return FindLayerByName(strLayerName); 
            }
            set
            {
                int nLayerIndex = -1;
                Layer layerTemp;

                // find layer to modify
                for (int nCounter = 0, nLength = 0; nCounter < nLength; nCounter++)
                {
                    layerTemp = this.Members[nCounter] as Layer;

                    if (layerTemp.Name == strLayerName)
                    {
                        nLayerIndex = nCounter;
                        break;
                    }
                }

                Set(nLayerIndex, value);
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Adds the specified layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>The value.</returns>
        public int Add(Layer layer)
        {
            return AddValue(layer);
        }

        /// <summary>
        /// Gets the layer index in current collection.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>The index.</returns>
        public int IndexOf(Layer layer)
        {
            return this.Members.IndexOf(layer);
        }

        /// <summary>
        /// Inserts the layer by specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="layer">The layer.</param>
        public void Insert(int index, Layer layer)
        {
            InsertValue(index, layer);
        }

        /// <summary>
        /// Removes the specified layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        public void Remove(Layer layer)
        {
            RemoveValue(layer);
        }

        /// <summary>
        /// Determines whether collection contains the specified layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>
        /// <c>true</c> if collection contains the specified layer; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Layer layer)
        {
            return this.Members.Contains(layer);
        }

        /// <summary>
        /// Copy all collection members to array.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Layer[] layers, int index)
        {
            this.Members.CopyTo(layers, index);
        }

        /// <summary>
        /// Determines whether collections contains the layer with specified name.
        /// </summary>
        /// <param name="strLayerName">Name of the layer.</param>
        /// <returns>
        /// <c>true</c> if collection contains the layer with specified name; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string strLayerName)
        {
            return FindLayerByName(strLayerName) != null;
        }

        /// <summary>
        /// Finds the layer that contain given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The layer at the given node.</returns>
        public Layer FindNodeLayer(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            Layer layerToReturn = null;

            foreach (Layer layerCur in this.Members)
            {
                if (layerCur.Contains(node))
                {
                    layerToReturn = layerCur;
                    break;
                }
            }

            return layerToReturn;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates the collection enumerator.
        /// </summary>
        /// <returns>Created enumerator</returns>
        public override IEnumerator GetEnumerator()
        {
            return new LayerEnumerator(this);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new LayerCollection(this);
        }

        /// <summary>
        /// Raises the <see cref="E:Changing"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected override void OnChanging(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.ChangeType == CollectionExChangeType.Insert ||
                evtArgs.ChangeType == CollectionExChangeType.Set)
            {
                Layer newLayer = evtArgs.Element as Layer;

                if (newLayer != null)
                {
                    string layerName = newLayer.Name;
                    
                    // Make sure the name of the layer is unique.
                    if (GenerateUniqueName(newLayer, ref layerName))
                    {
                        newLayer.Name = layerName;
                    }
                }
            }

            base.OnChanging(evtArgs);
        }

        /// <summary>
        /// Validates given value.
        /// </summary>
        /// <param name="value">value to validate</param>
        /// <exception cref="System.InvalidCastException"/>
        protected override void OnValidate(object value)
        {
            if (!(value is Layer))
                throw new InvalidCastException("value");
        }

        /// <summary>
        /// Validates given values.
        /// </summary>
        /// <param name="values">values to validate</param>
        /// <exception cref="System.InvalidCastException"/>
        protected override void OnValidate(ICollection values)
        {
            // Get collection enumerator
            IEnumerator enumerator = values.GetEnumerator();
            
            // Iterate through collection members checking their types
            while (enumerator.MoveNext())
            {
                if (!(enumerator.Current is Layer))
                    throw new InvalidCastException("value");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ChangesComplete"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected override void OnChangesComplete(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.ChangeType != CollectionExChangeType.Clear && evtArgs.ChangeType != CollectionExChangeType.Remove)
            {
                IEnumerator enumerator = evtArgs.Elements.GetEnumerator();
                Layer layerTemp;

                while (enumerator.MoveNext())
                {
                    layerTemp = enumerator.Current as Layer;

                    if (layerTemp != null)
                    {
                        if (this.Container == null && layerTemp.Container != null)
                            this.Container = layerTemp.Container;
                        else
                        {
                            ILayerContainer layerContainer = this.Container as ILayerContainer;

                            if (layerContainer != null)
                            {
                                layerTemp.Container = layerContainer;
                            }
                        }
                    }
                }
            }

            base.OnChangesComplete(evtArgs);
        }

        /// <summary>
        /// Raises ChangesComplete event.
        /// </summary>
        /// <param name="evtArgs">event args</param>
        protected override void RaiseChangesCompleteEvent(CollectionExEventArgs evtArgs)
        {
            if (!this.QuietMode && this.EventSink != null)
            {
                DocumentEventSink eventSink = this.EventSink as DocumentEventSink;

                if (eventSink != null)
                {
                    eventSink.RaiseLayersChangedEvent(evtArgs);
                }
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Generates the unique name for given layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <returns>true, if generate unique name otherwise, false.</returns>
        public bool GenerateUniqueName(Layer layer, ref string layerName)
        {
            bool bChange = (layerName == string.Empty);

            // create regex only once
            string strRegex = @"([0-9]*$)";
            Regex regex = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            layerName = (bChange) ? layer.GetType().Name : layerName;
            bChange = (layerName != layer.Name);
            string[] curLayerName = SplitLayerName(layerName, regex);

            List<Int64> indexes = new List<Int64>();
            bool bContain = false;

            // collect all indexes for new name
            foreach (Layer item in this)
            {
                if (item == layer)
                    continue;

                string[] name = SplitLayerName(item.Name, regex);

                if (name[0] == curLayerName[0])
                {
                    int nIndex = (name[1] != string.Empty) ? int.Parse(name[1]) : 0;
                    indexes.Add(nIndex);
                }

                if (!bContain)
                    bContain = (item.Name == layerName);
            }

            if (bContain)
            {
                Int64 nIndex = HandlesHitTesting.GetSkippedIndexes(indexes.ToArray(), 1)[0];
                layerName = curLayerName[0] + nIndex.ToString();
                bChange = true;
            }

            return bChange;
        }

        /// <summary>
        /// Splits the name of the layer.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="regex">The regex.</param>
        /// <returns>The splitted layer name.</returns>
        protected string[] SplitLayerName(string layerName, Regex regex)
        {
            string[] nameToReturn = new string[] { layerName, string.Empty };

            if (regex.IsMatch(layerName))
            {
                string[] strsName = regex.Split(layerName);
                Array.Copy(strsName, 0, nameToReturn, 0, 2);
            }

            return nameToReturn;
        }

        /// <summary>
        /// Finds the layer by name.
        /// </summary>
        /// <param name="strLayerName">Name of the layer.</param>
        /// <returns>The layer with the specified name.</returns>
        public Layer FindLayerByName(string strLayerName)
        {
            Layer layerToReturn = null;

            // iterate through layers' collection
            // looking for layer with the name specified
            foreach (Layer layer in this.Members)
            {
                if (layer.Name == strLayerName)
                {
                    layerToReturn = layer;
                    break;
                }
            }

            return layerToReturn;
        }
        #endregion
    }
}