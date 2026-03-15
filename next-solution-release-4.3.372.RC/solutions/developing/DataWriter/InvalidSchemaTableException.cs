using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataWriter
{
    public enum FaultOperation
    {
        Unknow,
        QuerySchema,
        CreateSchema,
        ChangeSchema,
        RebuildPrimaryKeys
    }

    public class InvalidSchemaTableException : Exception
    {
        #region Declarations
        readonly FaultOperation faultOperation;
        #endregion

        #region Constructors
        public InvalidSchemaTableException(FaultOperation fault, string message) : 
            base(message)
        {
            faultOperation = fault;
        }
        #endregion

        #region Properties
        public FaultOperation FaultOperation
        {
            get
            {
                return faultOperation;
            }
        }
        #endregion
    }
}
