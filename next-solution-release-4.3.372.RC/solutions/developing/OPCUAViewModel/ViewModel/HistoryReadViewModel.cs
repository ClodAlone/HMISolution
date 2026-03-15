using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using System.Windows.Input;
using LinqStatistics;
#if !WINDOWS_UWP
using log4net;
#endif

namespace OPCUAViewModel
{
    public class HistoryReadViewModel : ViewModelBase
    {
        public enum ReadTypeDefinition
        {
            Raw,
            Modified,
            AtTime,
            Processed
        }

#region Members

        SessionViewModel sessionViewModel;

        NodeId nodeId;
        HistoryReadResult result;
        int index;
#if !WINDOWS_UWP
#if !NET_STANDARD
        static readonly ILog log = LogManager.GetLogger(Properties.Resource.HistoryLog);
#else
        static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resource.HistoryLog);
#endif
#endif
#endregion

#region Constructor

        public HistoryReadViewModel(SessionViewModel s, NodeId node)
        {
            sessionViewModel = s;
            nodeId = node;

            UseServerCapabilitiesDefaults = 
                UseMaxReturnValues = true;
            MaxReturnValues = 25;
        }

#endregion

#region Properties
        String _lastMessage;
        public String LastMessage
        {
            get
            {
                return _lastMessage;
            }
            set
            {
                if (value == _lastMessage)
                    return;

                _lastMessage = value;
                OnPropertyChanged("LastMessage");
            }
        }

        public bool ReturnBounds { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTime ViewportStartTime { get; set; }
        public DateTime ViewportEndTime { get; set; }
        public DateTime SelectedStartTime { get; set; }
        public DateTime SelectedEndTime { get; set; }

        public uint MaxReturnValues { get; set; }
        public uint ResampleInterval { get; set; }

        public bool UseStartTime { get; set; }
        public bool UseEndTime { get; set; }
        public bool UseMaxReturnValues { get; set; }

        public ReadTypeDefinition ReadType { get; set; }
        public String Aggregate { get; set; }

        public bool UseServerCapabilitiesDefaults { get; set; }


        public double StatisticMax { private get; set; }
        public double StatisticMin { private get; set; }
        public double StatisticAverage { private get; set; }
        public double StatisticMedian { private get; set; }
        public double StatisticVariance { private get; set; }
        public double StatisticStandardDeviation { private get; set; }
        public double StatisticPopulationVariance { private get; set; }
        public double StatisticPopulationStandardDeviation { private get; set; }
        public double StatisticRange { private get; set; }

        public DataValueCollection DemoValues
        {
            get
            {
                Random random = new Random();
                DataValueCollection ret = new DataValueCollection();
                for (int i = 0; i < 100; ++i )
                    ret.Add(new DataValue() 
                            {
                                Value = i * random.NextDouble(),
                                StatusCode = StatusCodes.Good,
                                SourceTimestamp = DateTime.Now.AddDays(i)
                            }
                        );
                return ret;
            }
        }

        public DataValueCollection Values
        {
            get
            {
                if (result == null)
                    return null;

                HistoryData results = ExtensionObject.ToEncodeable(result.HistoryData) as HistoryData;

                if (results == null)
                    return null;

                // Dirty the commands registered with CommandManager,
                // such as our Save command, so that they are queried
                // to see if they can execute now.
#if !WINDOWS_UWP && !NET_STANDARD
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
                if (results.DataValues != null)
                {
                    foreach (var value in results.DataValues)
                    {
                        value.SourceTimestamp = value.SourceTimestamp.ToLocalTime();
                        value.ServerTimestamp = value.SourceTimestamp.ToLocalTime();
                    }
                }

                return results.DataValues;
            }
        }

#endregion

#region Commands

        RelayCommand _getResults;
        public ICommand GetResults
        {
            get
            {
                if (_getResults == null)
                {
                    _getResults = new RelayCommand(
                            param => 
                            { 
                                index = 0;
#if !WINDOWS_UWP && !NET_STANDARD
                                PromoteIdleExecution(System.Windows.Threading.DispatcherPriority.Invalid);
#else
                                PromoteIdleExecution(100);
#endif
                            },
                        param => sessionViewModel.Connected && (result == null || result.ContinuationPoint == null) && !IsBusy
                        );
                }
                return _getResults;
            }
        }

        RelayCommand _getNextResults;
        public ICommand GetNextResults
        {
            get
            {
                if (_getNextResults == null)
                {
                    _getNextResults = new RelayCommand(
                        param => 
                        {
#if !WINDOWS_UWP && !NET_STANDARD
                            PromoteIdleExecution(System.Windows.Threading.DispatcherPriority.Invalid);
#else
                            PromoteIdleExecution(100);
#endif
                        },
                        param => sessionViewModel.Connected && result != null && result.ContinuationPoint != null && !IsBusy
                        );
                }
                return _getNextResults;
            }
        }

        RelayCommand _stopResults;
        public ICommand StopResults
        {
            get
            {
                if (_stopResults == null)
                {
                    _stopResults = new RelayCommand(
                        param =>
                        {
                            ReleaseContinuationPoints();
                        },
                        param => sessionViewModel.Connected && result != null && result.ContinuationPoint != null && !IsBusy
                        );
                }
                return _stopResults;
            }
        }

#endregion

#region Methods
        public DataValueCollection GetHistoryValues(int timeZoneOffset)
        {
            if (result == null)
                return null;

            HistoryData results = ExtensionObject.ToEncodeable(result.HistoryData) as HistoryData;

            if (results == null)
                return null;

            if (results.DataValues != null)
            {
                foreach (var value in results.DataValues)
                {
                    value.SourceTimestamp = value.SourceTimestamp.ToUniversalTime().AddMinutes(timeZoneOffset);
                    value.ServerTimestamp = value.ServerTimestamp.ToUniversalTime().AddMinutes(timeZoneOffset);
                }
            }

            return results.DataValues;
        }

        protected override void IdleExecution()
        {
            try
            {
                LastMessage = String.Empty;
                Read();
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                LastMessage = message;
#if !WINDOWS_UWP
                log.Error(message);
#endif
            }
        }

        private void Read()
        {
            switch (ReadType)
            {
                case ReadTypeDefinition.Raw:
                    {
                        ReadRaw(false);
                        break;
                    }

                case ReadTypeDefinition.Modified:
                    {
                        ReadRaw(true);
                        break;
                    }

                case ReadTypeDefinition.AtTime:
                    {
                        ReadAtTime();
                        break;
                    }

                case ReadTypeDefinition.Processed:
                    {
                        ReadProcessed();
                        break;
                    }
            }

            try
            {
                var statDouble = (from c in Values
                                  select (double)c.Value);
                StatisticMax = statDouble.Max();
                StatisticMin = statDouble.Min();
                StatisticAverage = statDouble.Average();
                StatisticMedian = statDouble.Median();
                StatisticVariance = statDouble.Variance();
                StatisticStandardDeviation = statDouble.StandardDeviation();
                StatisticPopulationVariance = statDouble.VarianceP();
                StatisticPopulationStandardDeviation = statDouble.StandardDeviationP();
                StatisticRange = statDouble.Range();
            }
            catch (Exception ex)
            {
                StatisticMax = Double.NaN;
                StatisticMin = Double.NaN;
                StatisticAverage = Double.NaN;
                StatisticMedian = Double.NaN;
                StatisticVariance = Double.NaN;
                StatisticStandardDeviation = Double.NaN;
                StatisticPopulationVariance = Double.NaN;
                StatisticPopulationStandardDeviation = Double.NaN;
                StatisticRange = Double.NaN;
            }

            OnPropertyChanged("Values");
            OnPropertyChanged("StatisticMax");
            OnPropertyChanged("StatisticMin");
            OnPropertyChanged("StatisticAverage");
            OnPropertyChanged("StatisticMedian");
            OnPropertyChanged("StatisticVariance");
            OnPropertyChanged("StatisticStandardDeviation");
            OnPropertyChanged("StatisticPopulationVariance");
            OnPropertyChanged("StatisticPopulationStandardDeviation");
            OnPropertyChanged("StatisticRange");
    }


    private void ReleaseContinuationPoints()
        {
            ReadRawModifiedDetails details = new ReadRawModifiedDetails();

            HistoryReadValueId nodeToRead = new HistoryReadValueId();
            nodeToRead.NodeId = nodeId;

            if (result != null)
            {
                nodeToRead.ContinuationPoint = result.ContinuationPoint;
            }

            HistoryReadValueIdCollection nodesToRead = new HistoryReadValueIdCollection();
            nodesToRead.Add(nodeToRead);

            HistoryReadResultCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            sessionViewModel.Session.HistoryRead(
                null,
                new ExtensionObject(details),
                TimestampsToReturn.Source,
                true,
                nodesToRead,
                out results,
                out diagnosticInfos);

            Session.ValidateResponse(results, nodesToRead);
            Session.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            result = null;
        }

        private DateTime ReadFirstDate()
        {
            ReadRawModifiedDetails details = new ReadRawModifiedDetails();
            details.StartTime = new DateTime(1970, 1, 1);
            details.EndTime = DateTime.UtcNow.AddDays(1);
            details.IsReadModified = false;
            details.NumValuesPerNode = 1;
            details.ReturnBounds = false;

            HistoryReadValueId nodeToRead = new HistoryReadValueId();
            nodeToRead.NodeId = nodeId;

            HistoryReadValueIdCollection nodesToRead = new HistoryReadValueIdCollection();
            nodesToRead.Add(nodeToRead);

            HistoryReadResultCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            sessionViewModel.Session.HistoryRead(
                null,
                new ExtensionObject(details),
                TimestampsToReturn.Source,
                false,
                nodesToRead,
                out results,
                out diagnosticInfos);

            Session.ValidateResponse(results, nodesToRead);
            Session.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            if (StatusCode.IsBad(results[0].StatusCode))
            {
                return DateTime.MinValue;
            }

            HistoryData data = ExtensionObject.ToEncodeable(results[0].HistoryData) as HistoryData;

            if (results == null)
            {
                return DateTime.MinValue;
            }

            DateTime startTime = data.DataValues[0].SourceTimestamp;

            if (results[0].ContinuationPoint != null)
            {
                nodeToRead.ContinuationPoint = results[0].ContinuationPoint;

                sessionViewModel.Session.HistoryRead(
                    null,
                    new ExtensionObject(details),
                    TimestampsToReturn.Source,
                    true,
                    nodesToRead,
                    out results,
                    out diagnosticInfos);

                Session.ValidateResponse(results, nodesToRead);
                Session.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);
            }

            return startTime;
        }

        private void ReadRaw(bool isReadModified)
        {
            ReadRawModifiedDetails details = new ReadRawModifiedDetails();
            try
            {
                var firstDate = ReadFirstDate();// .ToLocalTime();
                details.StartTime = firstDate.AddMilliseconds(-1);
                if (!UseStartTime)
                {
                    StartTime = details.StartTime.ToLocalTime();
                    OnPropertyChanged("StartTime");
                }
            }
            catch (Exception)
            {
                details.StartTime = new DateTime(2000, 1, 1);
            }
            // details.StartTime = DateTime.MinValue;
            //details.EndTime = DateTime.MinValue;
            details.EndTime = DateTime.UtcNow.AddDays(1);

            details.IsReadModified = isReadModified;
            details.NumValuesPerNode = 0;
            details.ReturnBounds = ReturnBounds;

            if (UseStartTime)
            {
                details.StartTime = StartTime.ToUniversalTime();
            }

            if (UseEndTime)
            {
                details.EndTime = EndTime.ToUniversalTime();
            }

            if (UseMaxReturnValues)
            {
                details.NumValuesPerNode = MaxReturnValues;
            }

            HistoryReadValueId nodeToRead = new HistoryReadValueId();
            nodeToRead.NodeId = nodeId;

            if (result != null)
            {
                nodeToRead.ContinuationPoint = result.ContinuationPoint;
            }

            HistoryReadValueIdCollection nodesToRead = new HistoryReadValueIdCollection();
            nodesToRead.Add(nodeToRead);

            HistoryReadResultCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            sessionViewModel.Session.HistoryRead(
                null,
                new ExtensionObject(details),
                TimestampsToReturn.Source,
                false,
                nodesToRead,
                out results,
                out diagnosticInfos);

            Session.ValidateResponse(results, nodesToRead);
            Session.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            if (StatusCode.IsBad(results[0].StatusCode))
            {
                throw new ServiceResultException(results[0].StatusCode);
            }

            result = results[0];
        }

        private void ReadAtTime()
        {
        }

        private void ReadProcessed()
        {
            ReadProcessedDetails details = new ReadProcessedDetails();
            details.AggregateConfiguration.UseServerCapabilitiesDefaults = UseServerCapabilitiesDefaults;
            details.StartTime = StartTime.ToUniversalTime();
            details.EndTime = EndTime.ToUniversalTime();
            details.ProcessingInterval = (double)ResampleInterval;

            NodeId aggregateId = null;

            switch(Aggregate)
            {
                case BrowseNames.AggregateFunction_Interpolative: { aggregateId = ObjectIds.AggregateFunction_Interpolative; break; }
                case BrowseNames.AggregateFunction_TimeAverage: { aggregateId = ObjectIds.AggregateFunction_TimeAverage; break; }
                case BrowseNames.AggregateFunction_Average: { aggregateId = ObjectIds.AggregateFunction_Average; break; }
                case BrowseNames.AggregateFunction_Count: { aggregateId = ObjectIds.AggregateFunction_Count; break; }
                case BrowseNames.AggregateFunction_Maximum: { aggregateId = ObjectIds.AggregateFunction_Maximum; break; }
                case BrowseNames.AggregateFunction_Minimum: { aggregateId = ObjectIds.AggregateFunction_Minimum; break; }
                case BrowseNames.AggregateFunction_Total: { aggregateId = ObjectIds.AggregateFunction_Total; break; }
            }

            details.AggregateType.Add(aggregateId);
                        
            HistoryReadValueId nodeToRead = new HistoryReadValueId();
            nodeToRead.NodeId = nodeId;

            if (result != null)
            {
                nodeToRead.ContinuationPoint = result.ContinuationPoint;
            }

            HistoryReadValueIdCollection nodesToRead = new HistoryReadValueIdCollection();
            nodesToRead.Add(nodeToRead);

            HistoryReadResultCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            sessionViewModel.Session.HistoryRead(
                null,
                new ExtensionObject(details),
                TimestampsToReturn.Source,
                false,
                nodesToRead,
                out results,
                out diagnosticInfos);

            Session.ValidateResponse(results, nodesToRead);
            Session.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            if (StatusCode.IsBad(results[0].StatusCode))
            {
                throw new ServiceResultException(results[0].StatusCode);
            }

            result = results[0];
        }

#endregion
    }
}
