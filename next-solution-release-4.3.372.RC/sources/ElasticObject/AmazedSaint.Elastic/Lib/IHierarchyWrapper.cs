using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazedSaint.Elastic.Lib
{
    public interface IHierarchyWrapperProvider<T>
    {
        IEnumerable<KeyValuePair<string, T>> Attributes { get;  }
        bool HasAttribute(string name);
        IEnumerable<T> Elements { get; }
        void SetAttributeValue(string name, object obj);
        object GetAttributeValue(string name);
        T Attribute(string name);
        T Element(string name);
        void AddAttribute(string key, T value);
        void RemoveAttribute(string key);
        void AddElement(T element);
        void RemoveElement(T element);
        object InternalValue { get; set; }
        object InternalContent { get; set; }
        string InternalName { get; set; }
        T InternalParent { get; set; }
        
    }

    public interface IElasticHierarchyWrapper : IHierarchyWrapperProvider<ElasticObject> { }
}
