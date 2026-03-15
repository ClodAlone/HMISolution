using Opc.Ua;
using System.Collections.Generic;

namespace OPCUAViewModel.Services
{
    public interface IOPCSessionService
    {
        /// <summary>
        /// Write single value using monitoredItemViewModel session, if exists
        /// </summary>
        /// <param name="value">Value to write</param>
        /// <param name="monitoredItemViewModel"></param>
        /// <param name="lockObject">object used to lock writing session</param>
        /// <param name="indexArray"></param>
        /// <param name="indexBit"></param>
        /// <returns></returns>
        bool WriteValue(object value,
            MonitoredItemViewModel monitoredItemViewModel,
            object lockObject,
            int indexArray,
            int indexBit);

        /// <summary>
        /// Read values from monitoredItemViewModels sessions, once for single session.
        /// Items with null monitoredItem or session are not allowed
        /// </summary>
        /// <param name="monitoredItemViewModels"></param>
        /// <returns></returns>
        IList<MonitoredItemValue> ReadValues(IList<MonitoredItemViewModel> monitoredItemViewModels);

        /// <summary>
        /// Write all values splitting monitoredItemViewModels by sessions, once for single session.
        /// Items with null monitoredItem or session are not allowed
        /// </summary>
        /// <param name="monitoredItemViewModels"></param>
        /// <param name="values"></param>
        void WriteValues(IList<MonitoredItemViewModel> monitoredItemViewModels,
            IList<object> values); 

        /// <summary>
        /// Write single value synchronously using direct method, if exists
        /// </summary>
        /// <param name="tag">NodeId of the tag to write to</param>
        /// <param name="value">Value to write</param>
        /// <param name="monitoredItemViewModel"></param>
        /// <returns></returns>
        bool WriteSynchValue(NodeId tag, object value, MonitoredItemViewModel monitoredItemViewModel);
    }
}