using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections;
using RedundancyService.Helper;
using Opc.Ua.Utilities;

namespace RedundancyService
{
    [ServiceContract]
    public interface IRedundancyService
    {
        [OperationContract(IsOneWay = true)]
        void ServerActivating(String server);
        [OperationContract(IsOneWay = true)]
        void ServerDeactivating(String server);
        [OperationContract(IsOneWay = true)]
        void ChangedTags(String server, List<ChangedTags> changedTags);
        [OperationContract(IsOneWay = true)]
        void ChangedAlarms(String server, ChangedAlarms changedAlarms);

        [OperationContract]
        void Ping(String server);
        [OperationContract]
        HistorySettings GetHistorySettings();
        [OperationContract]
        uint WriteData(ChangedTags changedTags);
        [OperationContract]
        uint UpdateAlarms(ChangedAlarms changedAlarms);
        [OperationContract]
        ServiceResult CallMethod(CallMethod callMethod);
        [OperationContract]
        Dictionary<NodeId, LiveDataValue> GetAllLiveData();
        [OperationContract]
        Dictionary<NodeId, WrappedAlarmStatusCollection> GetAllAlarmsStatus();
    }

    [DataContract]
    public class HistorySettings
    {
        [DataMember]
        public String HistorianDefaultConnection;
        [DataMember]
        public String EventDefaultConnection;
        [DataMember]
        public Dictionary<NodeId, String> HistorianCustomConnection;
        [DataMember]
        public Dictionary<String, DataConnectionParameters> DataLoggerConnection;
    }

    [DataContract(Name = "T", Namespace = "")]
    public class LiveDataValue
    {
        [DataMember(Name = "V")]
        public WrappedDataValue DataValue;
        #region Statistics
        [DataMember(Name = "S", IsRequired = false)]
        public StatisticsData Statistics;
        #endregion
    }

    [DataContract(Name = "T", Namespace = "")]
    public class ChangedTags
    {
        [DataMember(Name = "N")]
        public String NodeId;
        [DataMember(Name = "V")]
        public WrappedDataValueCollection DataValues;
        #region Statistics
        [DataMember(Name = "S", IsRequired = false)]
        public StatisticsData Statistics;
        #endregion
    }

    #region Statistics
    [DataContract(Name = "ST", Namespace = "")]
    public class StatisticsData
    {
        [DataMember(Name = "Mi", Order = 6, IsRequired = false)]
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
        [DataMember(Name = "Ma", Order = 7, IsRequired = false)]
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
        [DataMember(Name = "TA", Order = 8, IsRequired = false)]
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
        [DataMember(Name = "CU", Order = 9, IsRequired = false)]
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
        [DataMember(Name = "TT", Order = 10, IsRequired = false)]
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
        [DataMember(Name = "LT", Order = 11, IsRequired = false)]
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
    }
    #endregion

    [DataContract(Name = "A", Namespace = "")]
    public class ChangedAlarms
    {
        [DataMember(Name = "N")]
        public String NodeId;
        [DataMember(Name = "S")]
        public WrappedAlarmStatusCollection AlarmsStatus;
    }

    [DataContract]
    public class CallMethod
    {
        [DataMember]
        public NodeId methodNodeId;
        [DataMember]
        public CallMethodRequest methodRequest;
        [DataMember]
        public CallMethodResult methodResult;
    }
}
