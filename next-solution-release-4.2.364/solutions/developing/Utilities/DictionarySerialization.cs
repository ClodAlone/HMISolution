using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Utilities
{
    public struct Pair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public Pair(KeyValuePair<TKey, TValue> pair)
        {
            Key = pair.Key;
            Value = pair.Value;
        }

        public static implicit operator Pair<TKey, TValue>(KeyValuePair<TKey, TValue> pair)
        {
            return new Pair<TKey, TValue>(pair);
        }

        public static implicit operator KeyValuePair<TKey, TValue>(Pair<TKey, TValue> pair)
        {
            return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
        }
    }

    public class DictionarySerialization<TKey, TValue>
    {
        private readonly static XmlSerializer serializer = new XmlSerializer(typeof(Pair<TKey, TValue>[]));

        public static void Serialize(Stream stream, IDictionary<TKey, TValue> dictionary)
        {
            Pair<TKey, TValue>[] items = new Pair<TKey, TValue>[dictionary.Count];
            int index = 0;

            foreach(KeyValuePair<TKey, TValue> item in dictionary)
            {
                items[index++] = item;
            }

            serializer.Serialize(stream, items);
        }

        public static void DeserializeInto(Stream stream, IDictionary<TKey, TValue> dictionary)
        {
            Pair<TKey, TValue>[] items = (Pair<TKey, TValue>[])serializer.Deserialize(stream);

            foreach(Pair<TKey, TValue> item in items)
            {
                dictionary.Add(item);
            }
        }
    }
}
