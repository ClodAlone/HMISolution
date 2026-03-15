#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Syncfusion.Documentation;
using System.Collections.Generic;


namespace Syncfusion.Windows.Forms.Chart
{
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    class ChartSetter : IEnumerable
    {
        #region Contants
        private const string DEF_VALUE_ATTR = "value";
        #endregion

        #region Members
        private PropertyDescriptor m_propertyDescriptor = null;
        private object m_value;
        public ArrayList m_children = new ArrayList();
        private ChartTemplateSet m_type = ChartTemplateSet.Simple;
        public List<ArrayList> m_serPoints = new List<ArrayList>();
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is empty value.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is empty value; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool IsEmptyValue
        {
            get
            {
                return null == m_value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty children.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is empty children; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool IsEmptyChildren
        {
            get
            {
                bool isEmpty = true;

                foreach (ChartSetter setter in m_children)
                {
                    if (!setter.IsEmpty)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                return isEmpty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return IsEmptyValue ? IsEmptyChildren : false;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        [Browsable(false)]
        public ChartTemplateSet Type
        {
            get
            {
                return m_type;
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        [Browsable(false)]
        public string PropertyName
        {
            get
            {
                return m_propertyDescriptor.Name;
            }
        }

        /// <summary>
        /// Gets the property descriptor.
        /// </summary>
        /// <value>The property descriptor.</value>
        public PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return m_propertyDescriptor;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public object Value
        {
            get
            {
                return m_value;
            }

            set
            {
                m_value = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Chart.ChartSetter"/> at the specified index.
        /// </summary>
        /// <value>The ChartSetter indexer.</value>
        public ChartSetter this[int index]
        {
            get
            {
                return m_children[index] as ChartSetter;
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Chart.ChartSetter"/> with the specified property.
        /// </summary>
        /// <value>The ChartSetter indexer.</value>
        public ChartSetter this[string property]
        {
            get
            {
                return GetSetter(property);
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        [Browsable(false)]
        public int Count
        {
            get
            {
                return m_children.Count;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSetter"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="descriptor">The descriptor.</param>
        public ChartSetter(ChartTemplateSet type, PropertyDescriptor descriptor)
        {
            m_type = type;
            m_propertyDescriptor = descriptor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSetter"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="value">The value.</param>
        public ChartSetter(ChartTemplateSet type, PropertyDescriptor descriptor, object value)
        {
            m_type = type;
            m_propertyDescriptor = descriptor;
            m_value = value;
        }
        #endregion

        #region Public methods
        
        /// <summary>
        /// Adds the specified setter.
        /// </summary>
        /// <param name="setter">The setter.</param>
        public void Add(ChartSetter setter)
        {
            m_children.Add(setter);
        }

        /// <summary>
        /// Removes the specified setter.
        /// </summary>
        /// <param name="setter">The setter.</param>
        public void Remove(ChartSetter setter)
        {
            m_children.Remove(setter);
        }

        /// <summary>
        /// Applies the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Apply(object owner)
        {
            switch (m_type)
            {
                case ChartTemplateSet.Simple:
                    if (!IsEmpty)
                    {
                        m_propertyDescriptor.SetValue(owner, m_value);
                    }

                    break;

                case ChartTemplateSet.SimpleBehavior: 
                    if (!IsEmpty)
                    {
                        m_propertyDescriptor.SetValue(owner, m_value);
                    }

                    break;

                case ChartTemplateSet.Content:
                    object contentObj = m_propertyDescriptor.GetValue(owner);

                    foreach (ChartSetter setter in m_children)
                    {
                        setter.Apply(contentObj);
                    }

                    break;

                case ChartTemplateSet.ContentBehavior: 

                    object contentObjNew = m_propertyDescriptor.GetValue(owner);
                    foreach (ChartSetter setter in m_children)
                    {
                        setter.Apply(contentObjNew);
                    }

                    break;

                case ChartTemplateSet.Collection:
                    {
                        IEnumerable list = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in list)
                            {
                                setter.Apply(child);
                            }
                        }

                        break;
                    }

                case ChartTemplateSet.CollectionBehavior: 
                    {
                        IEnumerable list = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in list)
                            {
                                setter.Apply(child);
                            }
                        }

                        break;
                    }


                case ChartTemplateSet.SimpleAndCollection:
                    {
                        if (!IsEmpty && (m_value != null))
                        {
                            m_propertyDescriptor.SetValue(owner, m_value);
                        }

                        IEnumerable list = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in list)
                            {
                                setter.Apply(child);
                            }
                        }

                        break;
                    }

                case ChartTemplateSet.SimpleAndCollectionBehavior:
                    {
                        if (!IsEmpty && (m_value != null))
                        {
                            m_propertyDescriptor.SetValue(owner, m_value);
                        }

                        IEnumerable list = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in list)
                            {
                                setter.Apply(child);
                            }
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// Resets the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Reset(object owner)
        {
            switch (m_type)
            {
                case ChartTemplateSet.Simple:
                    m_propertyDescriptor.ResetValue(owner);
                    break;

                case ChartTemplateSet.SimpleBehavior: 
                    m_propertyDescriptor.ResetValue(owner);
                    break;

                case ChartTemplateSet.Content:
                    object contentObj = m_propertyDescriptor.GetValue(owner);

                    foreach (ChartSetter setter in m_children)
                    {
                        setter.Reset(contentObj);
                    }

                    break;

                case ChartTemplateSet.ContentBehavior:
                    object contentObjNew = m_propertyDescriptor.GetValue(owner);

                    foreach (ChartSetter setter in m_children)
                    {
                        setter.Reset(contentObjNew);
                    }

                    break;

                case ChartTemplateSet.Collection:
                    {
                        IEnumerable list = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in list)
                            {
                                setter.Reset(child);
                            }
                        }

                        break;
                    }

                case ChartTemplateSet.CollectionBehavior:
                    {
                        IEnumerable list = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in list)
                            {
                                setter.Reset(child);
                            }
                        }

                        break;
                    }

                case ChartTemplateSet.SimpleAndCollection:
                    {
                        m_propertyDescriptor.ResetValue(owner);

                        IEnumerable list = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in list)
                            {
                                setter.Reset(child);
                            }
                        }

                        break;
                    }

                case ChartTemplateSet.SimpleAndCollectionBehavior:
                    {
                        m_propertyDescriptor.ResetValue(owner);

                        IEnumerable list = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in list)
                            {
                                setter.Reset(child);
                            }
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// Reads the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Read(XmlElement element)
        {
            if (element.HasAttribute(DEF_VALUE_ATTR))
            {
                TypeConverter converter = m_propertyDescriptor.Converter;

                if (converter.CanConvertFrom(typeof(string)))
                {
                    string attr = element.GetAttribute(DEF_VALUE_ATTR);
                    m_value = converter.ConvertFromString(null, CultureInfo.InvariantCulture, attr);
                }
            }

            foreach (XmlElement elem in element.ChildNodes)
            {
                ChartSetter setter = GetSetter(elem.Name);

                if (setter != null)
                {
                    setter.Read(elem);
                }
            }
        }

        /// <summary>
        /// Writes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Write(XmlElement element)
        {
            if (!IsEmptyValue)
            {
                TypeConverter converter = m_propertyDescriptor.Converter;

                if (converter.CanConvertTo(typeof(string)))
                {
                    element.SetAttribute(DEF_VALUE_ATTR, converter.ConvertToString(null, CultureInfo.InvariantCulture, m_value));
                }
            }
            if (element.Name == "PointsCollection")
            {
                foreach(ArrayList point in m_serPoints)
                {
                    XmlElement elemParent = element.OwnerDocument.CreateElement("ChartPoint") as XmlElement;
                    foreach (ChartSetter setter in point)
                    {
                        XmlElement elemChild = element.OwnerDocument.CreateElement(setter.PropertyName) as XmlElement;
                        if (!setter.IsEmpty)
                        {
                            setter.Write(elemChild);
                            elemParent.AppendChild(elemChild);                          
                        }                       
                    }                  
                    element.AppendChild(elemParent);
                }               
            }
            else
            {
                foreach (ChartSetter setter in m_children)
                {
                    if (!setter.IsEmpty)
                    {
                        XmlElement elem = element.OwnerDocument.CreateElement(setter.PropertyName) as XmlElement;

                        setter.Write(elem);
                        element.AppendChild(elem);
                    }
                }
            }
        }

        /// <summary>
        /// Scans the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Scan(object owner)
        {
            switch (m_type)
            {
                case ChartTemplateSet.Simple:
                    m_value = m_propertyDescriptor.GetValue(owner);
                    break;

                case ChartTemplateSet.SimpleBehavior: 
                    m_value = m_propertyDescriptor.GetValue(owner);
                    break;

                case ChartTemplateSet.Content:
                    {
                        object contentObj = m_propertyDescriptor.GetValue(owner);

                        foreach (ChartSetter setter in m_children)
                        {
                            setter.Scan(contentObj);
                        }
                    }

                    break;

                case ChartTemplateSet.ContentBehavior:
                    {
                        object contentObj = m_propertyDescriptor.GetValue(owner);

                        foreach (ChartSetter setter in m_children)
                        {
                            setter.Scan(contentObj);
                        }
                    }

                    break;

                case ChartTemplateSet.Collection:
                    IEnumerable list = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                    foreach (ChartSetter setter in m_children)
                    {
                        foreach (object child in list)
                        {
                            setter.Scan(child);
                            break;
                        }
                    }

                    break;

                case ChartTemplateSet.CollectionBehavior:
                    {
                        IEnumerable listNew = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in listNew)
                            {
                                setter.Scan(child);
                                break;
                            }
                        }

                    }
                    break;

                case ChartTemplateSet.SimpleAndCollection:
                    IEnumerable list2 = m_propertyDescriptor.GetValue(owner) as IEnumerable;
                    if (m_propertyDescriptor.PropertyType.Name == "ChartPointIndexer")
                    {                     
                        foreach (object child in list2)
                        {
                        foreach(ArrayList pValues in m_serPoints)
                        {
                            ChartSetter setterXValue = pValues[1] as ChartSetter;
                            if (setterXValue.Value == null)
                            {
                                foreach (ChartSetter setter in pValues)
                                {
                                    setter.Scan(child);
                                }
                                break;
                            }                                                   
                        }
                        }                             
                    }              
                    else
                    {
                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in list2)
                            {
                                setter.Scan(child);
                                break;
                            }
                        }
                    }

                    break;

                case ChartTemplateSet.SimpleAndCollectionBehavior:
                    {
                        IEnumerable list2New = m_propertyDescriptor.GetValue(owner) as IEnumerable;

                        foreach (ChartSetter setter in m_children)
                        {
                            foreach (object child in list2New)
                            {
                                setter.Scan(child);
                                break;
                            }
                        }
                    }
                    break;

            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("[prop={0},value={1}]", m_propertyDescriptor.Name, m_value);
        }
        #endregion

        #region Impelmentation

        /// <summary>
        /// Should the serialize value.
        /// </summary>
        /// <returns>True if the element should serialize otherwise False.</returns>
        private bool ShouldSerializeValue()
        {
            return !IsEmptyValue;
        }

        /// <summary>
        /// Resets the value.
        /// </summary>
        private void ResetValue()
        {
            m_value = null;
        }

        /// <summary>
        /// Gets the setter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Returns ChartSetter.</returns>
        private ChartSetter GetSetter(string propertyName)
        {
            ChartSetter result = null;

            foreach (ChartSetter setter in m_children)
            {
                if (setter.PropertyName == propertyName)
                {
                    result = setter;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_children.GetEnumerator();
        }
        #endregion
    }
}
