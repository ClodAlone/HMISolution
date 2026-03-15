using DevExpress.Xpo;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAModel.Helpers
{
    public class AssociationHelper
    {
        #region Declararions
        readonly Session session;
        readonly List<UFUAModel.UFUAView> views;
        readonly List<UFUAModel.UFUAAlarmDefinition> alarms;
        #endregion

        #region Constructors
        public AssociationHelper(Session session)
        {
            this.session = session;
        }

        public AssociationHelper(List<UFUAModel.UFUAView> views, List<UFUAModel.UFUAAlarmDefinition> alarms)
        {
            this.views = views;
            this.alarms = alarms;
        }
        #endregion

        #region Public Methods
        public void CopyAssociationReferences(List<XPObject> sources, List<XPObject> targets)
        {
            if (sources.Count != targets.Count)
                return;

            for (int ii = 0; ii < sources.Count; ii++)
                CopyAssociationReferences(sources[ii], targets[ii]);
        }

        public void CopyAssociationReferences(XPObject source, XPObject target)
        {
            if (source.GetType() != target.GetType())
                return;

            if (source is UFUAModel.UFUATagPrototype)
                CopyAssociationReferences(source as UFUAModel.UFUATagPrototype, target as UFUAModel.UFUATagPrototype);
            else if (source is UFUAModel.UFUAFolder)
                CopyAssociationReferences(source as UFUAModel.UFUAFolder, target as UFUAModel.UFUAFolder);
            else if (source is UFUAModel.UFUATag)
                CopyAssociationReferences(source as UFUAModel.UFUATag, target as UFUAModel.UFUATag);
        }

        public void CopyAssociationReferences(UFUAModel.UFUATagPrototype source, UFUAModel.UFUATagPrototype target)
        {
            foreach (var sourcefolder in source.Folders)
            {
                foreach (var targetfolder in target.Folders)
                {
                    if (sourcefolder.GetRelativeName() == targetfolder.GetRelativeName())
                    {
                        CopyAssociationReferences(sourcefolder, targetfolder);
                        break;
                    }
                }
            }

            foreach (var sourcetag in source.Members)
            {
                foreach (var targettag in target.Members)
                {
                    if (sourcetag.GetRelativeName() == targettag.GetRelativeName())
                    {
                        CopyAssociationReferences(sourcetag, targettag);
                        break;
                    }
                }
            }
        }

        public void CopyAssociationReferences(UFUAModel.UFUAFolder source, UFUAModel.UFUAFolder target)
        {
            foreach (var sourcefolder in source.UFUAFolders)
            {
                foreach (var targetfolder in target.UFUAFolders)
                {
                    if (sourcefolder.GetRelativeName() == targetfolder.GetRelativeName())
                    {
                        CopyAssociationReferences(sourcefolder, targetfolder);
                        break;
                    }
                }
            }

            foreach (var sourcetag in source.UFUATags)
            {
                foreach (var targettag in target.UFUATags)
                {
                    if (sourcetag.GetRelativeName() == targettag.GetRelativeName())
                    {
                        CopyAssociationReferences(sourcetag, targettag);
                        break;
                    }
                }
            }
        }

        public void CopyAssociationReferences(UFUAModel.UFUATag source, UFUAModel.UFUATag target)
        {
            // copy allarm description reference from source to target
            AssignAlarmDefinitionReference(target);

            // clear view reference inside target
            if (target.UFUAViews.Count > 0)
            {
                while (target.UFUAViews.Count > 0)
                    target.UFUAViews.Remove(target.UFUAViews[0]);
                target.NotifyPropertyChanged("UFUAViews");
            }

            // copy view reference from source to target
            if (source.UFUAViews.Count > 0)
            {
                foreach (var sourcedef in source.UFUAViews)
                {
                    var targedef = FindViewByNodeId(sourcedef.NodeId.ToString());
                    if (targedef != null)
                    {
                        target.UFUAViews.Add(targedef);
                    }
                }
                target.NotifyPropertyChanged("UFUAViews");
            }

            // copy enums reference from source to target
            target.EnumStringsFlat = source.EnumStringsFlat;
        }

        public void ClearAssociationReferences(UFUAModel.UFUATag tag)
        {
            while (tag.UFUAAlarmThresholds.Count > 0)
            {
                var alarm = tag.UFUAAlarmThresholds[0];
                tag.UFUAAlarmThresholds.Remove(alarm);
                alarm.Delete();
            }

            while (tag.UFUAViews.Count > 0)
            {
                var view = tag.UFUAViews[0];
                tag.UFUAViews.Remove(view);
                view.Delete();
            }

            tag.EnumStringsFlat = null;
        }
        #endregion

        #region Private Methods
        void AssignAlarmDefinitionReference(UFUAModel.UFUATag target)
        {
            // copy allarm description reference from source to target
            if (target.UFUAAlarmThresholds.Count > 0)
            {
                foreach (var targetdef in target.UFUAAlarmThresholds)
                {
                    if (!NodeId.IsNull(targetdef.UFUAAlarmDefinitionNodeIdRef))
                    {
                        targetdef.UFUAAlarmDefinitionRef = FindAlarmDefinitionByNodeId(targetdef.UFUAAlarmDefinitionNodeIdRef);
                    }
                }
                target.NotifyPropertyChanged("UFUAAlarmThresholds");
            }

            if (target.SubPrototypeMembers != null && target.SubPrototypeMembers.Count > 0)
            {
                var members = (from c in target.SubPrototypeMembers[0].GetTagMembers().AsParallel()
                               where !c.UseShared.Value
                               select c).ToList();

                foreach (var member in members)
                    AssignAlarmDefinitionReference(member);
            }
        }

        UFUAModel.UFUAView FindViewByNodeId(String nodeId)
        {
            UFUAModel.UFUAView viewFound = null;
            if (session != null)
            {
                viewFound = (from view in new XPQuery<UFUAModel.UFUAView>(session, true).AsParallel()
                             where view.NodeId.ToString() == nodeId
                             select view).FirstOrDefault();
            }
            else if (views != null && views.Count > 0)
            {
                viewFound = (from view in views.AsParallel()
                             where view.NodeId.ToString() == nodeId
                             select view).FirstOrDefault();
            }

            return viewFound;
        }

        internal UFUAModel.UFUAAlarmDefinition FindAlarmDefinitionByNodeId(Guid nodeId)
        {
            UFUAModel.UFUAAlarmDefinition alarmFound = null;
            if (session != null)
            {
                alarmFound = (from definition in new XPQuery<UFUAModel.UFUAAlarmDefinition>(session, true).AsParallel()
                              where definition.NodeId == nodeId
                              select definition).FirstOrDefault();
            }
            else if (alarms != null && alarms.Count > 0)
            {
                alarmFound = (from definition in alarms.AsParallel()
                              where definition.NodeId == nodeId
                              select definition).FirstOrDefault();
            }

            return alarmFound;
        }
        #endregion
    }
}
