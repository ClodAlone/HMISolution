using System;
using Opc.Ua;
using StatDef;

namespace UFUATagLogger
{
    public class UFUATagLogEntity : IStatisticsData
    {
        public String NodeId;
        public String TagName;
        public DataValue Value;
        public BuiltInType builtinType;
        public uint arraySizeOneDimension;

        #region Statistics

        public bool enableStatistics;

        public double? min;
        public double? max;
        public double? totAverage;
        public double? countUpdates;
        public TimeSpan? totalTimeOn;
        public DateTime? lastTotalTimeOn;
        public double? lastDoubleValue;

        public double Min
        {
            get
            {
                if (min == null)
                    return Double.NaN;
                return min.Value;
            }
        }

        public double Max
        {
            get
            {
                if (max == null)
                    return Double.NaN;
                return max.Value;
            }
        }

        public double CountUpdates
        {
            get
            {
                if (countUpdates == null)
                    return Double.NaN;
                return countUpdates.Value;
            }
        }

        public double Average
        {
            get
            {
                if (!totAverage.HasValue || !countUpdates.HasValue || countUpdates == 0)
                    return Double.NaN;
                return totAverage.Value / countUpdates.Value;
            }
        }

        public TimeSpan TotalTimeOn
        {
            get
            {
                if (lastTotalTimeOn.HasValue)
                {
                    if (totalTimeOn != null)
                        return totalTimeOn.Value + (DateTime.UtcNow - lastTotalTimeOn.Value);
                    else
                        return DateTime.UtcNow - lastTotalTimeOn.Value;
                }

                if (totalTimeOn != null)
                    return totalTimeOn.Value;
                else
                    return TimeSpan.Zero;
            }
        }

        public bool IsTotalTimeOnActive 
        { 
            get 
            {
                return lastTotalTimeOn != null;
            }
        }

        public void ResetStatistics()
        {
            lastDoubleValue = countUpdates = min = max = totAverage = null;
            totalTimeOn = null;
            lastTotalTimeOn = null;

            try
            {
                var val = Convert.ToDouble(Value.Value);
                if (val != 0.0)
                    lastTotalTimeOn = DateTime.UtcNow;
            }
            catch
            { }

            OnStatisticUpdated();
        }

        public void UpdateStatistics(DataValue value)
        {
            if (value.Value == null)
                return;

            double val = 0;
            try
            {
                val = Convert.ToDouble(value.Value);
            }
            catch
            {
                return;
            }

            if (lastDoubleValue.HasValue && lastDoubleValue.Value == val)
                return;

            lastDoubleValue = val;
            if (countUpdates == null || countUpdates.Value == 0)
            {
                min = max = totAverage = val;
                countUpdates = 1;
            }
            else
            {
                ++countUpdates;
                totAverage += val;
                if (val < min)
                    min = val;
                if (val > max)
                    max = val;
            }

            if (val != 0.0)
            {
                if (lastTotalTimeOn == null)
                    lastTotalTimeOn = DateTime.UtcNow;
            }
            else
            {
                if (lastTotalTimeOn.HasValue)
                {
                    if (totalTimeOn.HasValue)
                        totalTimeOn += DateTime.UtcNow - lastTotalTimeOn.Value;
                    else
                        totalTimeOn = DateTime.UtcNow - lastTotalTimeOn.Value;

                    lastTotalTimeOn = null;
                }
            }

            OnStatisticUpdated();
        }

        #region Events
        public event EventHandler StatisticUpdated;
        #region OnInUseTag
        /// <summary>
        /// Triggers the DoEvents event.
        /// </summary>
        public virtual void OnStatisticUpdated()
        {
            var e = StatisticUpdated;
            if (e != null)
                e(this, EventArgs.Empty);
        }
        #endregion
        #endregion

        #endregion
    }
}
