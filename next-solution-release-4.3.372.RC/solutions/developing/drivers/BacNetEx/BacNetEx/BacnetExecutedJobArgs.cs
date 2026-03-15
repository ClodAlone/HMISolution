using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;

namespace BACnet
{
    public class BACnetExecutedJobArgs : ExecutedJobArgs
    {
        #region Constructors
        public BACnetExecutedJobArgs()
            : base()
        {
            _IsRead = false;
            _IsCovDataFromDifferentJob = false;
        }
        #endregion

        #region Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   States if the job has been executed for reading data or must be process unsolicited data. </summary>
        ///
        /// <value> True or False. </value>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool _IsRead = false;
        public bool IsRead
        {
            get { return _IsRead; }
            set { _IsRead = value; }
        }

        bool _IsCovDataFromDifferentJob = false;
        public bool IsCovDataFromDifferentJob
        {
            get { return _IsCovDataFromDifferentJob; }
            set { _IsCovDataFromDifferentJob = value; }
        }
        #endregion
    }
}
