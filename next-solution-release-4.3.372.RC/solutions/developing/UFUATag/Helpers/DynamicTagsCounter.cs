using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAModel.Helpers
{
    public static class DynamicTagsCounter
    {
        #region Methods
        public static uint GetDynamicTagsCounter(Session session, bool bArrayOneSize, IList<UFUAModel.UFUATag> dynamicTags = null)
        {
            uint totalCount = 0;
            var tags = dynamicTags;
            if (tags == null)
                tags = GetAllDynamicTags(session);
            foreach (var tag in tags)
                totalCount += GetMemberCount(session, tag, bArrayOneSize);
            return totalCount;
        }

        public static uint GetDynamicTagsCounter(Session session, UFUAModel.UFUAFolder folder, bool bArrayOneSize)
        {
            uint totalCount = 0;
            var tags = GetAllDynamicTags(session, folder);
            foreach (var tag in tags)
                totalCount += GetMemberCount(session, tag, bArrayOneSize);
            return totalCount;
        }

        public static uint GetDynamicTagsCounter(Session session, UFUAModel.UFUATag tag, bool bArrayOneSize)
        {
            uint totalCount = 0;
            if (IsDynamicTag(tag))
                totalCount += GetMemberCount(session, tag, bArrayOneSize);
            return totalCount;
        }

        static List<UFUAModel.UFUATag> GetAllDynamicTags(Session session)
        {
            return (from tag in new XPQuery<UFUAModel.UFUATag>(session, true).AsParallel()
                    where IsDynamicTag(tag)
                    select tag).ToList();
        }

        static List<UFUAModel.UFUATag> GetAllDynamicTags(Session session, UFUAFolder folder)
        {
            return (from tag in folder.GetTagMembers().AsParallel()
                    where IsDynamicTag(tag)
                    select tag).ToList();
        }

        static bool IsDynamicTag(UFUAModel.UFUATag tag)
        {
            return !tag.ExcludeDynamicSettings && !String.IsNullOrEmpty(tag.DynamicSettings) && (tag.IsSubPrototypeMember && !tag.UseShared.Value || !tag.IsPrototypeMember);
        }

        static uint GetMemberCount(Session session, UFUAModel.UFUAFolder folder, bool bArrayOneSize)
        {
            uint totalCount = 0;
            foreach (var t in folder.UFUATags)
                totalCount += GetMemberCount(session, t, bArrayOneSize);
            foreach (var f in folder.UFUAFolders)
                totalCount += GetMemberCount(session, f, bArrayOneSize);

            return totalCount;
        }

        static uint GetMemberCount(Session session, UFUAModel.UFUATag tag, bool bArrayOneSize)
        {
            if (!String.IsNullOrEmpty(tag.PrototypeName))
            {
                var prototypeName = tag.PrototypeName;
                var typedeflist = (from p in new XPQuery<UFUAModel.UFUATagPrototype>(session, true).AsParallel()
                                    where p.Name == prototypeName && p.UFUATagOwner == null
                                    select p).ToList();
                if (typedeflist.Count > 0)
                {
                    uint totalCount = 0;
                    foreach (var t in typedeflist[0].Members)
                        totalCount += GetMemberCount(session, t, bArrayOneSize);
                    foreach (var f in typedeflist[0].Folders)
                        totalCount += GetMemberCount(session, f, bArrayOneSize);

                    return totalCount;
                }
            }

            if (!bArrayOneSize && tag.ArrayDimension > 0)
                return tag.ArrayDimension;
            return 1;
        }
        #endregion
    }
}
