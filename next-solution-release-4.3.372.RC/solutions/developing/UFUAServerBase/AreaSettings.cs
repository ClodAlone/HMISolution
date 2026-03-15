using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Text;

namespace UFUAServerBase
{
    internal class AreaSettings
    {
        #region Declarations
        readonly string areaName;
        #endregion

        #region Constructors
        public AreaSettings(string name)
        {
            areaName = name;
        }
        #endregion

        #region Properties
        public NodeId AlarmsNumEnabledStateNodeId { get; set; }
        public NodeId AlarmsNumActiveOnStateNodeId { get; set; }
        public NodeId AlarmsNumActiveOffStateNodeId { get; set; }
        public NodeId AlarmsNumActiveOnOffStateNodeId { get; set; }
        public NodeId AlarmsNumShelvedStateNodeId { get; set; }
        public NodeId AlarmsNumNotAckStateNodeId { get; set; }
        public NodeId MessagesNumEnabledStateNodeId { get; set; }
        public NodeId MessagesNumActiveOnStateNodeId { get; set; }
        public NodeId MessagesNumShelvedStateNodeId { get; set; }

        public string AreaName
        {
            get
            {
                return areaName;
            }
        }
        #endregion

        #region Methods
        public List<NodeId> GetStateNodeIds()
        {
            var nodeIds = new List<NodeId>();
            if (!NodeId.IsNull(AlarmsNumEnabledStateNodeId))
                nodeIds.Add(AlarmsNumEnabledStateNodeId);
            if (!NodeId.IsNull(AlarmsNumActiveOnStateNodeId))
                nodeIds.Add(AlarmsNumActiveOnStateNodeId);
            if (!NodeId.IsNull(AlarmsNumActiveOffStateNodeId))
                nodeIds.Add(AlarmsNumActiveOffStateNodeId);
            if (!NodeId.IsNull(AlarmsNumActiveOnOffStateNodeId))
                nodeIds.Add(AlarmsNumActiveOnOffStateNodeId);
            if (!NodeId.IsNull(AlarmsNumShelvedStateNodeId))
                nodeIds.Add(AlarmsNumShelvedStateNodeId);
            if (!NodeId.IsNull(AlarmsNumNotAckStateNodeId))
                nodeIds.Add(AlarmsNumNotAckStateNodeId);
            if (!NodeId.IsNull(MessagesNumEnabledStateNodeId))
                nodeIds.Add(MessagesNumEnabledStateNodeId);
            if (!NodeId.IsNull(MessagesNumActiveOnStateNodeId))
                nodeIds.Add(MessagesNumActiveOnStateNodeId);
            if (!NodeId.IsNull(MessagesNumShelvedStateNodeId))
                nodeIds.Add(MessagesNumShelvedStateNodeId);
            return nodeIds;
        }
        #endregion
    }
}
