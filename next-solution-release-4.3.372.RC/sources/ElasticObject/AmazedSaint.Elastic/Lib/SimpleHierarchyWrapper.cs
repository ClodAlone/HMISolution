using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazedSaint.Elastic.Lib
{
    /// <summary>
    /// A concrete hierarchy wrapper
    /// </summary>
    public class SimpleHierarchyWrapper : IElasticHierarchyWrapper
    {
        private Dictionary<string, ElasticObject> attributes = new Dictionary<string, ElasticObject>();
        private Dictionary<string, List<ElasticObject>> elements = new Dictionary<string, List<ElasticObject>>();

        #region IElasticHierarchyWrapper<ElasticObject> Members

        public IEnumerable<KeyValuePair<string, ElasticObject>> Attributes
        {
            get { return attributes; }
        }

        public bool HasAttribute(string name)
        {
            return attributes.ContainsKey(name);
        }

        public ElasticObject Attribute(string name)
        {
            if (HasAttribute(name))
                return attributes[name];
            else
                return null;
        }

        public ElasticObject Element(string name)
        {
           return Elements.FirstOrDefault(item => item.InternalName == name);
        }

        public IEnumerable<ElasticObject> Elements
        {
            get
            {
                var result = from list in elements
                             from item in list.Value
                             select item;
                return result;
            }
        }

        public void AddAttribute(string key, ElasticObject value)
        {
            attributes.Add(key, value);
        }

        public void RemoveAttribute(string key)
        {
            attributes.Remove(key);
        }

        public void AddElement(ElasticObject element)
        {
            if (!elements.ContainsKey(element.InternalName))
            {
                elements[element.InternalName] = new List<ElasticObject>();
            }
            elements[element.InternalName].Add(element);
        }

        public void RemoveElement(ElasticObject element)
        {
            if (elements.ContainsKey(element.InternalName))
            {
                if (elements[element.InternalName].Contains(element))
                    elements[element.InternalName].Remove(element);
            }
        }

        private object internalContent;
        public object InternalContent
        {
            get
            {
                return internalContent;
            }
            set
            {
                internalContent = value;
            }
        }

        private object internalValue;
        public object InternalValue
        {
            get
            {
                return internalValue;
            }
            set
            {
                internalValue = value;
            }
        }

        private string internalName;
        public string InternalName
        {
            get
            {
                return internalName;
            }
            set
            {
                internalName = value;
            }
        }

        ElasticObject internalParent;
        public ElasticObject InternalParent
        {
            get
            {
                return internalParent;
            }
            set
            {
                internalParent = value;
            }
        }

        public void SetAttributeValue(string name, object obj)
        {
            attributes[name].InternalValue = obj;
        }

        public  object GetAttributeValue(string name)
        {
            return attributes[name].InternalValue;
        }


        #endregion

        
    }
}
