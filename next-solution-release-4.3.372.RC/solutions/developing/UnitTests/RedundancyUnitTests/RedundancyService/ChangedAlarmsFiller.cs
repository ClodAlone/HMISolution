using RedundancyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyUnitTests.ActiveServerManager
{
    public class ChangedAlarmsFiller
    {
        #region Declarations
        readonly List<ChangedAlarms> changedAlarms;
        readonly int alarmssCounter;
        readonly int valuesCounter;
        #endregion

        #region Constructors
        ChangedAlarmsFiller()
        {
            changedAlarms = new List<ChangedAlarms>(alarmssCounter);
        }

        public ChangedAlarmsFiller(int alarmsCounter, int valuesCounter) : 
            this()
        {
            this.alarmssCounter = alarmsCounter;
            this.valuesCounter = valuesCounter;

            FillChangedAlarms();
        }
        #endregion

        #region Methods
        void FillChangedAlarms()
        {
            for (int ii = 0; ii < alarmssCounter; ii++)
            {
                var values = new WrappedAlarmStatusCollection(valuesCounter);
                for (int cc = 0; cc < valuesCounter; cc++)
                    values.Add(new UFUAAlarm.AlarmStatus());
                changedAlarms.Add(new ChangedAlarms() { NodeId = new Opc.Ua.NodeId(Guid.NewGuid()).ToString(), AlarmsStatus = values });
            }
        }
        #endregion

        #region Properties
        public List<ChangedAlarms> ChangedAlarms
        {
            get
            {
                return changedAlarms;
            }
        }

        public int MaxAlarms
        {
            get
            {
                return alarmssCounter;
            }
        }

        public int MaxValues
        {
            get
            {
                return valuesCounter;
            }
        }
        #endregion
    }
}
