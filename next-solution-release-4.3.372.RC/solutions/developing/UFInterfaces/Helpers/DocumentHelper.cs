using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.ComponentService;

namespace DocumentManager.ComponentService.Helpers
{
    public static class DocumentHelper
    {
        public static IDocument GetRootParent(IDocument parent, bool traverse)
        {
            if (!traverse && parent.IsRoot)
                return parent;

            var p = parent;
            var memParent = p;
            while (p != null)
            {
                p = p.Parent;
                if (p == null || (!traverse && p.IsRoot))
                {
                    if (p != null)
                        memParent = p;
                    break;
                }
                memParent = p;
            }
            p = memParent;
            return p;
        }
        public static IDocument GetRootParent(IDocument parent, int backSteps)
        {
            if(backSteps <= 0)
                return null;

            var p = parent;
            if (!p.IsRoot)
                p = p.Parent;

            while (p != null)
            {
                p = p.Parent;
                backSteps--;
                if (p == null || backSteps == 0)
                {
                    break;
                }
            }
            return p;
        }

        public static IDocument GetChild(IDocument parent, string childName)
        {
            var p = parent;
            IDocument child = null;
            IDocument ret = null;
            if (!p.IsRoot)
                p = p.Parent;

            var split = childName.Split('.');
            for(int i = 0; i <= split.Length - 1; i++)
            {
                child = (from c in p.Childs where c.Title == split[i] select c).FirstOrDefault();
                if (child != null)
                {
                    p = ret = child;
                }
                else
                    break;
            }
            return ret;
        }
    }
}
