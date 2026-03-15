// ***START***
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;
// ListOfImports

namespace _ExternalNamespace_
{
#if INCLUDE__DesignName_
    // ListOfIdentifiers

    #region BrowseName Declarations
    /// <summary>
    /// Declares all of the BrowseNames used in the Model Design.
    /// </summary>
    public static partial class BrowseNames
    {
        // ListOfBrowseNames
    }
    #endregion
    
    #region Namespace Declarations
    /// <summary>
    /// Defines constants for all namespaces referenced by the model design.
    /// </summary>
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
    #endregion

    // ListOfDataTypes
#endif
}
// ***END***
