using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAEditor.Helpers
{
    internal enum TypeObject : int
    {
        Tag = 0x1,
        Folder = 0x3,
        Prototype = 0x7,
        View = 0x10,
        AlarmDefinition = 0x100,
        AlarmSource = 0x300,
        AlarmArea = 0x700
    }

    internal class GuidHelper
    {
        #region Declarations
        readonly Session session;
        readonly TypeObject type;
        #endregion

        #region Constructors
        public GuidHelper(Session session, TypeObject type)
        {
            this.session = session;
            this.type = type;
        }
        #endregion

        #region Methods
        public bool Contains(Guid guid)
        {
            return Guids.Contains(guid);
        }

        public void EnsureValidGuid(UFUAModel.UFUATagPrototype ufuaPrototype)
        {
            if (Guids.Contains(ufuaPrototype.NodeId))
                ufuaPrototype.NodeId = Guid.NewGuid();

            foreach (var folder in ufuaPrototype.Folders)
                EnsureValidGuid(folder);

            foreach (var tag in ufuaPrototype.Members)
                EnsureValidGuid(tag);
        }

        public void EnsureValidGuid(UFUAModel.UFUAFolder ufuaFolder)
        {
            if (Guids.Contains(ufuaFolder.NodeId))
                ufuaFolder.NodeId = Guid.NewGuid();

            foreach (var folder in ufuaFolder.UFUAFolders)
                EnsureValidGuid(folder);

            foreach (var tag in ufuaFolder.UFUATags)
                EnsureValidGuid(tag);
        }

        public void EnsureValidGuid(UFUAModel.UFUATag ufuaTag)
        {
            if (Guids.Contains(ufuaTag.NodeId))
                ufuaTag.NodeId = Guid.NewGuid();
        }

        public void EnsureValidGuid(UFUAModel.UFUAArea ufuaArea)
        {
            if (Guids.Contains(ufuaArea.NodeId))
                ufuaArea.NodeId = Guid.NewGuid();

            foreach (var source in ufuaArea.UFUAAlarmSources)
                EnsureValidGuid(source);

            foreach (var area in ufuaArea.UFUAAreas)
                EnsureValidGuid(area);
        }

        public void EnsureValidGuid(UFUAModel.UFUAAlarmSource ufuaSource)
        {
            if (Guids.Contains(ufuaSource.NodeId))
                ufuaSource.NodeId = Guid.NewGuid();

            foreach (var alarm in ufuaSource.UFUAAlarmDefinitions)
                EnsureValidGuid(alarm);
        }

        public void EnsureValidGuid(UFUAModel.UFUAAlarmDefinition alarm)
        {
            if (Guids.Contains(alarm.NodeId))
                alarm.NodeId = Guid.NewGuid();
        }
        #endregion

        #region Properties
        List<Guid> guids;
        public List<Guid> Guids
        {
            get
            {
                if (guids == null)
                {
                    guids = new List<Guid>();
                    if ((type & TypeObject.Tag) == TypeObject.Tag)
                    {
                        guids.AddRange((from p in new XPQuery<UFUAModel.UFUATag>(session, true)
                                        select p.NodeId).ToList());
                    }
                    if ((type & TypeObject.Folder) == TypeObject.Folder)
                    {
                        guids.AddRange((from p in new XPQuery<UFUAModel.UFUAFolder>(session, true)
                                        select p.NodeId).ToList());
                    }
                    if ((type & TypeObject.Prototype) == TypeObject.Prototype)
                    {
                        guids.AddRange((from p in new XPQuery<UFUAModel.UFUATagPrototype>(session, true)
                                        select p.NodeId).ToList());
                    }
                    if ((type & TypeObject.View) == TypeObject.View)
                    {
                        guids.AddRange((from p in new XPQuery<UFUAModel.UFUAView>(session, true)
                                        select p.NodeId).ToList());
                    }
                    if ((type & TypeObject.AlarmDefinition) == TypeObject.AlarmDefinition)
                    {
                        guids.AddRange((from p in new XPQuery<UFUAModel.UFUAAlarmDefinition>(session, true)
                                        select p.NodeId).ToList());
                    }
                    if ((type & TypeObject.AlarmSource) == TypeObject.AlarmSource)
                    {
                        guids.AddRange((from p in new XPQuery<UFUAModel.UFUAAlarmSource>(session, true)
                                        select p.NodeId).ToList());
                    }
                    if ((type & TypeObject.AlarmArea) == TypeObject.AlarmArea)
                    {
                        guids.AddRange((from p in new XPQuery<UFUAModel.UFUAArea>(session, true)
                                        select p.NodeId).ToList());
                    }
                }

                return guids;
            }
        }
        #endregion
    }
}
