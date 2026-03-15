using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DataValidation
{
    public class DynamicRow
    {
        #region Declarations
        readonly bool isValid;
        readonly dynamic dataRow;
        #endregion

        #region Constructors
        public DynamicRow(dynamic dataRow, bool isValid)
        {
            this.dataRow = dataRow;
            this.isValid = isValid;
        }
        #endregion

        #region Properties
        public bool IsValid
        {
            get
            {
                return isValid;
            }
        }
        public dynamic DataRow
        {
            get
            {
                return dataRow;
            }
        }
        #endregion
    }
}
