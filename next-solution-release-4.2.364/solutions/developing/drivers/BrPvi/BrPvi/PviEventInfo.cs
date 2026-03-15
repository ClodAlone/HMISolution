using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrPvi
{
    public class PviEventInfo : Object
    {
        #region Constructors
        public PviEventInfo(PviComStruct.T_RESPONSE_INFO responseInfo)
        {
            LinkID = responseInfo.LinkID;
            nMode = responseInfo.nMode;
            nType = responseInfo.nType;
            ErrCode = responseInfo.ErrCode;
            Status = responseInfo.Status;
        }

        public PviEventInfo()
        {
            LinkID = 0;
            nMode = 0;
            nType = 0;
            ErrCode = 0;
            Status = 0;
        }
        #endregion

        #region data fields
        public UInt32 LinkID = 0; // Link object identifier
        public UInt32 nMode = 0; // Request/response/event mode
        public UInt32 nType = 0; // Type of access or event
        public UInt32 ErrCode = 0; // != 0 -> Error state
        public UInt32 Status = 0; // response/event status
        #endregion
    }
}
