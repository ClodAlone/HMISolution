////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	StatisicTag.cs
//
// summary:	Implements the statisic tag class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace DriverBaseInterfaces
{
    /// <summary>   A statisic tag. </summary>
    public struct StatisicTag
    {
        /// <summary>   The name. </summary>
        public String Name;
        /// <summary>   The description. </summary>
        public String Description;
        /// <summary>   Type of the data. </summary>
        public UFUAModel.DataType DataType;
    }
}
