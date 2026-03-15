////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	IStatistics.cs
//
// summary:	Declares the IStatistics interface
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DriverBaseInterfaces
{
    /// <summary>   Interface for statistics. </summary>
    public interface IStatistics
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Allow to know if the statistics are available for this object. </summary>
        ///
        /// <returns>   true if statistics are available; otherwise, false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool IsStatisticsAvailable();

        /// <summary>   Initialize statistics. </summary>
        void StartStatistics();

        /// <summary>   Terminate statistics and free the counters. </summary>
        void TerminateStatistics();

        /// <summary>   Suspend statistics watcher. </summary>
        void SuspendStatistics();

        /// <summary>   Resum statistics watcher. </summary>
        void ResumeStatistics();

        /// <summary>   Reset whole statistic counters to zero. </summary>
        void ResetStatistcs();
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Allow to retrieve a copy of the current statistics dictionary. </summary>
        ///
        /// <param name="totaltimeon" type="out TimeSpan">  [out] out value with the elapsed total time
        ///                                                 with statistics enabled. </param>
        ///
        /// <returns>   An IDictionary. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        IDictionary RetrieveStatisticCounters(out TimeSpan totaltimeon);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the number of statistic values recorded. </summary>
        ///
        /// <returns>   The total counters. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        int GetTotalCounters();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return a list with the name of each statistic counter. </summary>
        ///
        /// <returns>   The list of counters name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        IList GetListOfCountersName();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the value of a counter. </summary>
        ///
        /// <param name="key">  name of a valid counter. </param>
        ///
        /// <returns>   The counter value. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        long GetCounterValue(String key);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the elapsed total time with statistics enabled. </summary>
        ///
        /// <returns>   The total time on. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        TimeSpan GetTotalTimeOn();
    }
}
