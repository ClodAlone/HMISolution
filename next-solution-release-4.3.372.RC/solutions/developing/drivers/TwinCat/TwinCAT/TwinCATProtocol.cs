////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	IEC60870_5_104Protocol.cs
//
// summary:	Implements the driver IEC60870_5_104 protocol class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace TwinCAT
{
    /// <summary>   Communication protocol of IEC60870_5_104 driver. </summary>
    public class TwinCATProtocol
    {
        #region const
        public const int MAX_DATA_BYTES = 2048;
        #endregion

        #region methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return maximum memory size of TwinCAT objects basing on Movicon data type.</summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static uint GetMaxJobSize(UFUAModel.DataType varType)
        {
            if (varType == UFUAModel.DataType.Boolean)
                return MAX_DATA_BYTES * 8;
            else
                return MAX_DATA_BYTES;
        }

        public static bool IsValidStringSize(uint size)
        {
            return (size<=0 || MAX_DATA_BYTES > size);
        }

        #endregion
    }
}
