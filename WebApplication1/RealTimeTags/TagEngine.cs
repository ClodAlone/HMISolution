using RealTimeData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeTags
{
    internal class TagEngine
    {
        Object lockObject = new Object();

        Dictionary<String, Tag> realTimeTags;
        HashSet<Tag> listTags;

        internal event EventHandler<TagChangedArg> TagChanging;
        internal event EventHandler<TagChangedArg> TagChanged;

        internal void Init(IEnumerable<Tag> tags)
        {
            lock(lockObject)
            {
                if (realTimeTags != null)
                    throw new Exception("Init has already been invoked");
                listTags = new HashSet<Tag>(tags);
                realTimeTags = new Dictionary<String, Tag>(listTags.Count);
            }
        }

        void UpdateDictionary(IEnumerable<Tag> tags)
        {
            lock(lockObject)
            {
                var list = (from t in tags.AsParallel() 
                           where !realTimeTags.ContainsKey(t.id) select t);
                foreach (var tag in list)
                    realTimeTags.Add(tag.id, tag);
            }
        }

        void UpdateDictionary(IEnumerable<String> tags)
        {
            lock (lockObject)
            {
                var list = (from t in tags.AsParallel()
                            where !realTimeTags.ContainsKey(t)
                            select t);
                foreach (var id in list)
                    realTimeTags.Add(id, new Tag());
            }
        }

        internal void Add(IEnumerable<Tag> tags)
        {
            UpdateDictionary(tags);
        }

        internal void Remove(IEnumerable<String> tags)
        {
            lock (lockObject)
            {
                foreach (var id in tags)
                {
                    if (realTimeTags.ContainsKey(id))
                        realTimeTags.Remove(id);
                }
            }
        }

        internal void Write(IEnumerable<Tag> tags)
        {
            UpdateDictionary(tags);

            lock (lockObject)
            {
                var list = new List<Tag>(tags);
                list.RemoveAll(tag => !realTimeTags.ContainsKey(tag.id) || tag.DataValue == realTimeTags[tag.id].DataValue);

                var t1 = TagChanging;
                if (t1 != null)
                {
                    var arg = new TagChangedArg(list);
                    t1(this, arg);
                    list.RemoveAll(tag => !arg.IsGood(tag));
                }

                list.AsParallel().ForAll(tag => realTimeTags[tag.id].DataValue = tag.DataValue);

                var t2 = TagChanged;
                if (t2 != null)
                {
                    var arg = new TagChangedArg(list);
                    t2(this, arg);
                }
            }
        }

        internal IEnumerable<Tag> Read(IEnumerable<String> ids)
        {
            UpdateDictionary(ids);

            lock (lockObject)
                return (from t in realTimeTags.Keys.AsParallel() where ids.Contains(t) select realTimeTags[t]);
        }
    }
}
