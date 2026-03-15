using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using System.Windows.Input;

namespace OPCUAViewModel
{
    public class HistoryEventReadViewModel : ViewModelBase
    {
        #region Members

        SessionViewModel sessionViewModel;

        NodeId nodeId; // aread nodeid
        HistoryReadResult result;
        FilterDeclaration filter = new FilterDeclaration();
        int index;

        #endregion

        #region Constructor

        public HistoryEventReadViewModel(SessionViewModel s, NodeId node)
        {
            sessionViewModel = s;
            nodeId = node;

            UseMaxReturnValues = true;
            MaxReturnValues = 25;
        }

        #endregion

        #region Properties

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

        public String Aggregate { get; set; }

        public HistoryEventFieldListCollection Values
        {
            get
            {
                if (result == null)
                    return null;

                HistoryEvent results = ExtensionObject.ToEncodeable(result.HistoryData) as HistoryEvent;

                if (results == null)
                    return null;

                // Dirty the commands registered with CommandManager,
                // such as our Save command, so that they are queried
                // to see if they can execute now.
#if !WINDOWS_UWP && !NET_STANDARD
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
                return results.Events;
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

        protected override void IdleExecution()
        {
            try
            {
                Read();
            }
            catch (Exception
#if !WINDOWS_UWP && !NET_STANDARD
            exception
#endif
            )
            {
#if !WINDOWS_UWP && !NET_STANDARD
                System.Windows.MessageBox.Show(exception.Message);
#endif
            }
        }

        private void Read()
        {
            ReadValues();
            OnPropertyChanged("Values");
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
                TimestampsToReturn.Neither,
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
            // read the time of the first event in the archive.
            ReadEventDetails details = new ReadEventDetails();
            details.StartTime = new DateTime(1970, 1, 1);
            details.EndTime = DateTime.MinValue;
            details.NumValuesPerNode = 1;
            details.Filter = new EventFilter();
            details.Filter.AddSelectClause(Opc.Ua.ObjectTypeIds.BaseEventType, Opc.Ua.BrowseNames.Time);

            HistoryReadValueId nodeToRead = new HistoryReadValueId();
            nodeToRead.NodeId = nodeId;

            HistoryReadValueIdCollection nodesToRead = new HistoryReadValueIdCollection();
            nodesToRead.Add(nodeToRead);

            HistoryReadResultCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            sessionViewModel.Session.HistoryRead(
                null,
                new ExtensionObject(details),
                TimestampsToReturn.Neither,
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

            HistoryEvent data = ExtensionObject.ToEncodeable(results[0].HistoryData) as HistoryEvent;

            if (results == null)
            {
                return DateTime.MinValue;
            }

            // check if an event found.
            if (data == null || data.Events.Count == 0 || data.Events[0].EventFields.Count == 0)
            {
                throw new ServiceResultException(StatusCodes.BadNoDataAvailable);
            }

            // get the event time.
            DateTime? eventTime = data.Events[0].EventFields[0].Value as DateTime?;

            if (eventTime == null)
            {
                throw new ServiceResultException(StatusCodes.BadTypeMismatch);
            }

            // return time as UTC value.
            return eventTime.Value;
        }

        private void ReadValues()
        {
            ReadEventDetails details = new ReadEventDetails();
            try
            {
                details.StartTime = ReadFirstDate().ToLocalTime();
            }
            catch (Exception)
            {
                details.StartTime = new DateTime(2000, 1, 1);
            }
            // details.StartTime = DateTime.MinValue;
            details.EndTime = DateTime.MinValue;
            details.NumValuesPerNode = 0;
            details.Filter = filter.GetFilter();

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
                TimestampsToReturn.Neither,
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
