using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class TagIdentifier
    {
        public object TagReference { get; private set; }
        public TagIdentifier(object tagReference)
        {
            if (tagReference != null)
                TagReference = tagReference;
        }
    }
}
