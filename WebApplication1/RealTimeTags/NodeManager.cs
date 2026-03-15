using Interfaces;
using Opc.Ua;
using RealTimeData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeTags
{
    public class NodeManager : IDisposable
    {
        TagEngine tagEngine = new TagEngine();

        HashSet<TagDefinition> listTagDefinitions;
        Dictionary<String, TagDefinition> mapTagDefinitions;

        HashSet<TagUnit> listTagUnits;
        Dictionary<String, TagUnit> mapTagUnits;

        HashSet<Tag> listTags;
        Dictionary<String, Tag> mapTags;
        Dictionary<String, Tag> mapRetentiveTags;

        RetentiveManager retentiveManager;
        Object lockObject = new Object();

        Timer retentiveWriteDelay;
        HashSet<Tag> listWriteRetentive = new HashSet<Tag>();
        int delayWriteRetentive = 5000;

        public event EventHandler ExceptionRaised;

        public NodeManager() 
        { 

        }

        public void Dispose()
        {
            lock(lockObject)
            {
                if (retentiveWriteDelay != null)
                {
                    var eventTimer = new ManualResetEvent(false);
                    retentiveWriteDelay.Dispose(eventTimer);
                    eventTimer.WaitOne();
                }
            }

            retentiveManager?.Dispose();
        }

        public void Init(IEnumerable<Tag> tags, IEnumerable<TagDefinition> tagdefinition, 
            IEnumerable<TagUnit> tagUnits, String retentivePath, String password)
        {
            lock(lockObject)
            {
                listTags = new HashSet<Tag>(tags);
                listTagDefinitions = new HashSet<TagDefinition>(tagdefinition);
                listTagUnits = new HashSet<TagUnit>(tagUnits);

                mapTags = listTags.ToDictionary(x => x.id);
                mapTagDefinitions = listTagDefinitions.ToDictionary(x => x.id);
                mapTagUnits = listTagUnits.ToDictionary(x => x.id);

                mapRetentiveTags = (from t in listTags.AsParallel()
                                    where
                                    mapTagDefinitions.ContainsKey(t.idTagDefinition) &&
                                    mapTagDefinitions[t.idTagDefinition].IsRetentive
                                    select t).ToDictionary(x => x.id);
                if (mapRetentiveTags.Count > 0)
                {
                    retentiveManager = new RetentiveManager(retentivePath, password);

                    try
                    {
                        var listRetentive = retentiveManager.ReadRetentive();

                        lock (lockObject)
                        {
                            var toUpdate = (from tag in listRetentive.AsParallel()
                                            where mapRetentiveTags.ContainsKey(tag.id)
                                            select tag).ToList();
                            foreach (var tag in toUpdate)
                            {
                                mapRetentiveTags[tag.id].DataValue = tag.DataValue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        RaiseException(ex);
                    }
                }

                tagEngine.Init(listTags);

                tagEngine.TagChanged += (o, e) =>
                {
                    var listChanged = e.GetListChanges();

                    UpdateRetentive(listChanged);
                };
            }
        }

        void RaiseException(Exception ex)
        {
            var t = ExceptionRaised;
            if (t == null)
                return;
            t(ex, EventArgs.Empty);
        }

        private void UpdateRetentive(IEnumerable<Tag> listChanged)
        {
            lock (lockObject)
            {
                var retentiveupdate = from c in listChanged.AsParallel()
                                      where mapRetentiveTags.ContainsKey(c.id)
                                      select c;
                listWriteRetentive.UnionWith(retentiveupdate);

                if (retentiveWriteDelay != null)
                {
                    retentiveWriteDelay = new Timer(delegate
                    {
                        var priority = Thread.CurrentThread.Priority;
                        try
                        {
                            var list = new List<Tag>();
                            lock (lockObject)
                            {
                                if (retentiveWriteDelay == null)
                                    return;
                                retentiveWriteDelay.Dispose();
                                retentiveWriteDelay = null;

                                list.AddRange(listWriteRetentive);
                                listWriteRetentive.Clear();
                            }

                            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                            try
                            {
                                retentiveManager.UpdateRetentive(list);
                            }
                            catch(Exception ex)
                            {
                                RaiseException(ex);
                            }
                        }
                        finally
                        {
                            Thread.CurrentThread.Priority = priority;
                        }
                    }, null, delayWriteRetentive, Timeout.Infinite);
                }
            }
        }

        public IEnumerable<Tag> Read(IEnumerable<String> ids, IUser user)
        {
            var listTag = new List<Tag>();
            var mapsValues = tagEngine.Read(ids).ToDictionary(x =>x.id);

            lock (lockObject)
            {
                foreach (var id in ids)
                {
                    if (!mapTags.ContainsKey(id) || !mapTagDefinitions.ContainsKey(id))
                    {
                        listTag.Add(new Tag() { id = id, DataValue = new DataValue(StatusCodes.BadNotFound) });
                    }
                    else
                    {
                        var tagdefinition = mapTagDefinitions[id];
                        if (!tagdefinition.IsReadable || !HasReadableAccess(tagdefinition, user))
                            listTag.Add(new Tag() { id = id, DataValue = new DataValue(StatusCodes.BadNotReadable) });
                        else
                            listTag.Add(new Tag(mapsValues[id]));
                    }
                }
            }

            return listTag;
        }

        public StatusCode Read(String id, int index, IUser user)
        {
            return StatusCodes.BadNotImplemented;
        }

        public StatusCode Write(Tag tags, int index, IUser user)
        {
            return StatusCodes.BadNotImplemented;
        }

        public IEnumerable<StatusCode> Write(IEnumerable<Tag> tags, IUser user)
        {
            var listStatusCodes = new List<StatusCode>();
            var listTag = new List<Tag>();

            lock(lockObject)
            {
                foreach (var tag in tags)
                {
                    if (!mapTags.ContainsKey(tag.id) || !mapTagDefinitions.ContainsKey(tag.id))
                    {
                        listStatusCodes.Add(StatusCodes.BadNotFound);
                    }
                    else
                    {
                        var tagdefinition = mapTagDefinitions[tag.id];
                        if (!tagdefinition.IsWritable || !HasWritableAccess(tagdefinition, user))
                            listStatusCodes.Add(StatusCodes.BadNotWritable);
                        else
                        {
                            var statusCodeConversion = ConvertToType(tag, tagdefinition.TypeInfo);
                            listStatusCodes.Add(statusCodeConversion);
                            if (StatusCode.IsGood(statusCodeConversion))
                                listTags.Add(tag);
                        }
                    }
                }

                tagEngine.Write(listTags);
            }
            
            return listStatusCodes;
        }

        StatusCode ConvertToType(Tag tag, TypeInfo typeInfo)
        {
            if (!TypeInfo.IsNumericType(typeInfo.BuiltInType))
                return StatusCodes.Good;

            return StatusCodes.Good;
        }

        bool HasWritableAccess(TagDefinition tagDefinition, IUser user)
        {
            if (tagDefinition.AccessLevel > user.GetAccessLevel())
                return false;
            if ((tagDefinition.WritableAccessMask & user.GetWritableAccessMask()) == 0)
                return false;

            return true;
        }

        bool HasReadableAccess(TagDefinition tagDefinition, IUser user)
        {
            if (tagDefinition.AccessLevel > user.GetAccessLevel())
                return false;
            if ((tagDefinition.ReadableAccessMask & user.GetReadableAccessMask()) == 0)
                return false;

            return true;
        }

    }
}