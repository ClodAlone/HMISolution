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
    }
}
