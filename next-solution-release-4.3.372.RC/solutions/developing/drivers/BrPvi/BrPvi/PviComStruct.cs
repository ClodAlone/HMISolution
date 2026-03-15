using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BrPvi
{
    public class PviComStruct
    {
        #region Structure definitions
        /// <exclude/>
        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct T_RESPONSE_INFO
        {
            /// <exclude/>
            public UInt32 LinkID; // Link object identifier
            /// <exclude/>
            public UInt32 nMode; // Request/response/event mode
            /// <exclude/>
            public UInt32 nType; // Type of access or event
            /// <exclude/>
            public UInt32 ErrCode; // != 0 -> Error state
            /// <exclude/>
            public UInt32 Status; // response/event status
        }
        #endregion
    }
}
