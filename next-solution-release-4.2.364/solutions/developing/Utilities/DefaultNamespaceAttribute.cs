using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class DefaultNamespaceAttribute : Attribute
    {
        private string _defaultNamespace;

        public DefaultNamespaceAttribute(string defaultNamespace)
        {
            _defaultNamespace = defaultNamespace;
        }

        public string DefaultNamespace 
        { 
            get 
            { 
                return 
                    _defaultNamespace; 
            }
        }
    }
}
