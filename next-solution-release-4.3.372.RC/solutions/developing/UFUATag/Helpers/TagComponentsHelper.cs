using Opc.Ua;
using System;
using System.Collections.Generic;

namespace UFUAModel.Helpers
{
    public static class TagComponentsHelper
    {
        public struct UFUATagComponents
        {
            public readonly String RelativePath;
            public readonly String HumanReadable;
            public readonly NodeId NodeId;
            public readonly UFUATag TagParent;

            public UFUATagComponents(String relativePath, String humanReadable, NodeId nodeId, UFUAModel.UFUATag tagParent)
            {
                RelativePath = relativePath;
                HumanReadable = humanReadable;
                NodeId = nodeId;
                TagParent = tagParent;
            }
        }

        public static String GetTagPath(UFUATagComponents c)
        {
            return Utilities.TagPathHelper.GetTagPath(c.HumanReadable, c.RelativePath);
        }

        public static UFUATagComponents GetUFUATagComponents(string applicationName, UFUAModel.UFUATag tagFound, IList<UFUAModel.UFUATag> parentTags = null)
        {
            var ns = Utilities.TagPathHelper.GetNS();

            // find relative path and relativa name by scrolling through members
            var relativepath = tagFound.GetRelativePath(ns);

            var humanReadable = string.Format("{0} ({1})", tagFound.GetRelativeName().Replace('/', '\\'), applicationName);
            var relativememberpath = string.Empty;
            UFUAModel.UFUATag tagParent = null;
            if (parentTags != null && parentTags.Count > 0)
            {
                string hr = string.Empty;
                for (int ii = parentTags.Count - 2; ii >= 0; ii--)
                {
                    tagParent = parentTags[ii];
                    if (String.IsNullOrEmpty(relativememberpath))
                    {
                        relativememberpath = tagParent.GetRelativeName();
                        relativepath = tagParent.GetRelativePath(ns, true);
                    }
                    else
                    {
                        relativememberpath = String.Format("{0}/{1}", relativememberpath, tagParent.GetRelativeName());
                        relativepath = String.Format("{0}/{1}", relativepath, tagParent.GetRelativePath(ns, true));
                    }
                    if (string.IsNullOrEmpty(hr))
                        hr = tagParent.Name;
                    else
                        hr = string.Format("{0}\\{1}", hr, tagParent.Name);
                }
                if (!string.IsNullOrEmpty(hr))
                    humanReadable = String.Format("{0}\\{1}", hr, humanReadable);

                tagParent = parentTags[parentTags.Count - 1];

                if (String.IsNullOrEmpty(relativememberpath))
                {
                    relativememberpath = tagFound.GetRelativeNodeId();
                    relativepath = tagFound.GetRelativePath(ns, true);
                }
                else
                {
                    relativememberpath = String.Format("{0}/{1}", relativememberpath, tagFound.GetRelativeNodeId());
                    relativepath = String.Format("{0}/{1}", relativepath, tagFound.GetRelativePath(ns, true));
                }
            }

            // calculate the nodeid to match the selected variable
            Opc.Ua.NodeId nodeid;
            if (tagParent == null)
            {
                nodeid = new Opc.Ua.NodeId(tagFound.NodeId, ns);
            }
            else
            {
                nodeid = new Opc.Ua.NodeId(String.Format("{0}?{1}", tagParent.NodeId, relativememberpath), ns);
                relativepath = String.Format("{0}/{1}", tagParent.GetRelativePath(ns), relativepath);
                humanReadable = String.Format("{0}:{1}", tagParent.Name, humanReadable);
            }
            return new UFUATagComponents(relativepath, humanReadable, nodeid, tagParent);
        }
    }
}
