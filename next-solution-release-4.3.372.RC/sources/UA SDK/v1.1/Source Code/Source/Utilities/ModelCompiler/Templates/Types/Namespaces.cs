// ***START***
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;
// ListOfImports

namespace _Namespace_
{
    /// <summary>
    /// Defines constants for all namespace referenced by the model design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]     
    public static partial class Namespaces
    {
        // ListOfNamespaceUris
        
        /// <summary>
		/// Returns a namespace table with all of the URIs defined.
		/// </summary>
        /// <remarks>
        /// This table is was used to create any relative paths in the model design.
        /// </remarks>
		public static NamespaceTable GetNamespaceTable()
		{
			FieldInfo[] fields = typeof(Namespaces).GetFields(BindingFlags.Public | BindingFlags.Static);
            
            NamespaceTable namespaceTable = new NamespaceTable();

			foreach (FieldInfo field in fields)
			{
                string namespaceUri = (string)field.GetValue(typeof(Namespaces));

                if (namespaceTable.GetIndex(namespaceUri) == -1)
                {
				    namespaceTable.Append(namespaceUri);
                }
			}

			return namespaceTable;
		}
    }
}
// ***END***
