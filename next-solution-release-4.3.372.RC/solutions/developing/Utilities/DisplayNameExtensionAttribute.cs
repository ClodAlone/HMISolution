using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class DisplayNameExtension : Attribute
    {
        public DisplayNameExtension()
        {
        }

        public override string ToString()
        {
            return "Extended";
        }
    }
}
