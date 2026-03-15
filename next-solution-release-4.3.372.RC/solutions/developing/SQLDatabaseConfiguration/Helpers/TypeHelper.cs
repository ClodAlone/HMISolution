using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabaseConfiguration.Helpers
{
    /// <summary>
    /// TypeHelper provides static methods that operate on Types.
    /// </summary>
    public class TypeHelper
    {
        /// <summary>
        /// Determines whether the specified type is numeric.
        /// </summary>
        /// <param name="type">The type to be examined.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is numeric; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(Type type)
        {
            return type == typeof(double) ||
                    type == typeof(float) ||
                    type == typeof(decimal) ||
                    type == typeof(Int64) ||
                    type == typeof(Int32) ||
                    type == typeof(Int16) ||
                    type == typeof(UInt64) ||
                    type == typeof(UInt32) ||
                    type == typeof(UInt16) ||
                    type == typeof(byte) ||
                    type == typeof(sbyte);
        }
    }
}
