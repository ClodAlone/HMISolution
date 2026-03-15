using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace SmartTagsHelper.EditableAdapter
{
    public class Memento<T>
    {
        Dictionary<PropertyInfo, object> storedProperties = new Dictionary<PropertyInfo, object>();

        public Memento(T originator)
        {
            var type = originator.GetType();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                         .Where(p => p.CanRead && p.CanWrite);

            foreach (var property in propertyInfos)
            {
                this.storedProperties[property] = property.GetValue(originator, null);
            }
        }

        public void Restore(T originator)
        {
            foreach (var pair in this.storedProperties)
            {
                pair.Key.SetValue(originator, pair.Value, null);
            }
        }
    }
}
