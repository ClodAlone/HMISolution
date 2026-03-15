using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedundancyService.Helper
{
    public class ChangedAlarmsHelper : ChangedHelper<ChangedAlarms>
    {
        #region Declarations
        readonly List<ChangedAlarms> changedAlarms;
        #endregion

        #region Constructors
        public ChangedAlarmsHelper(IList<ChangedAlarms> changedAlarms)
        {
            this.changedAlarms = new List<ChangedAlarms>(changedAlarms.Count);
            this.changedAlarms.AddRange(changedAlarms);
        }

        public ChangedAlarmsHelper(ChangedAlarms changedAlarms) : 
            this(new List<ChangedAlarms>() { changedAlarms })
        { }
        #endregion

        #region Overrides
        protected override int GetElementsCount()
        {
            return changedAlarms.Count;
        }
        protected override int GetValuesCount()
        {
            return changedAlarms[skipElements].AlarmsStatus.Count;
        }

        public override List<ChangedAlarms> GetChangedElementsBlock()
        {
            var tags = new List<ChangedAlarms>();

            if (changedAlarms.Count > 0)
            {
                UpdateCounters();

                var list = changedAlarms.Skip(skipElements).Take(takeElements).ToList();
                if (list.Count == 1)
                {
                    var values = list[0].AlarmsStatus.Skip(skipValues).Take(takeValues).ToList();
                    tags.Add(new ChangedAlarms() { NodeId = list[0].NodeId, AlarmsStatus = new WrappedAlarmStatusCollection(values) });
                }
                else if (list.Count > 1)
                {
                    tags.AddRange(list);
                }
            }

            return tags;
        }
        #endregion
    }
}
