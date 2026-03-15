////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ChangedArgs.cs
//
// summary:	Implements the changed arguments class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;

namespace DriverBaseInterfaces
{
    /// <summary>   Arguments for changed tag. </summary>
    public class ChangedTagArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Construct an istance of this object.
        /// </summary>
        public ChangedTagArgs()
        {
            AllowNullValues = false;
            DisableQualityUpdate = false;
    }

        /// <summary>
        /// Construct an istance of this object.
        /// </summary>
        /// <param name="template"></param>
        public ChangedTagArgs(ChangedTagArgs template)
        {
            driverName = template.driverName;
            NodeId = template.NodeId;
            Status = template.Status;

            if (template.DataValues.Count > 0)
            {
                foreach (var value in template.DataValues)
                    DataValues.Add(new DataValue(value));
            }

            AllowNullValues = template.AllowNullValues;
            DisableQualityUpdate = template.DisableQualityUpdate;
        }
#endregion

#region Members
        /// <summary>   Driver name. </summary>
        public String driverName;
        /// <summary>   Identifier for the node. </summary>
        public NodeId NodeId;
        /// <summary>   The data values. </summary>
        public List<DataValue> DataValues = new List<DataValue>();
        /// <summary>   The operation Status. </summary>
        public uint Status;
        /// <summary>  Used by Movicon to identify is null value is a valid value </summary>
        public bool AllowNullValues;
        /// <summary>  Used by Movicon in order not to update the quality </summary>
        public bool DisableQualityUpdate;
        #endregion

        #region Statistics
        public Double? Min
        {
            get
            {
                return min;
            }
            set
            {
                min = value;
            }
        }
        public Double? Max
        {
            get
            {
                return max;
            }
            set
            {
                max = value;
            }
        }
        public Double? TotAverage
        {
            get
            {
                return totAverage;
            }
            set
            {
                totAverage = value;
            }
        }
        public Double? CountUpdates
        {
            get
            {
                return countUpdates;
            }
            set
            {
                countUpdates = value;
            }
        }
        public TimeSpan? TotalTimeOn
        {
            get
            {
                return totalTimeOn;
            }
            set
            {
                totalTimeOn = value;
            }
        }
        public DateTime? LastTotalTimeOn
        {
            get
            {
                return lastTotalTimeOn;
            }
            set
            {
                lastTotalTimeOn = value;
            }
        }

        double? min;
        double? max;
        double? totAverage;
        double? countUpdates;
        TimeSpan? totalTimeOn;
        DateTime? lastTotalTimeOn;
#endregion
    }
}
