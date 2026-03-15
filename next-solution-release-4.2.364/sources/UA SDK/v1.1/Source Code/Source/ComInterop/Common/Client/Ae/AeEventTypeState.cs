/* ========================================================================
 * Copyright (c) 2005-2010 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Reciprocal Community Binary License ("RCBL") Version 1.00
 * 
 * Unless explicitly acquired and licensed from Licensor under another 
 * license, the contents of this file are subject to the Reciprocal 
 * Community Binary License ("RCBL") Version 1.00, or subsequent versions 
 * as allowed by the RCBL, and You may not copy or use this file in either 
 * source code or executable form, except in compliance with the terms and 
 * conditions of the RCBL.
 * 
 * All software distributed under the RCBL is provided strictly on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
 * AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RCBL for specific 
 * language governing rights and limitations under the RCBL.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/RCBL/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using Opc.Ua;
using OpcRcw.Ae;

namespace Opc.Ua.Com.Client
{
    /// <summary>
    /// Stores information about an AE event type in the server address space.
    /// </summary>
    internal class AeEventTypeState : BaseObjectTypeState
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AeEventTypeState"/> class.
        /// </summary>
        public AeEventTypeState(EventType eventType, ushort namespaceIndex)
        {
            m_eventType = eventType;

            // create the name for the event type.
            string name = eventType.Description;
            
            if (!String.IsNullOrEmpty(eventType.ConditionName))
            {
                name = eventType.ConditionName;
            }

            if (!name.EndsWith("Type"))
            {
                if (eventType.EventTypeId == OpcRcw.Ae.Constants.CONDITION_EVENT)
                {
                    name += "AlarmType";
                }
                else
                {
                    name += "EventType";
                }
            }

            // the attributes.
            this.NodeId = AeParsedNodeId.Construct(eventType, null, namespaceIndex);
            this.BrowseName = new QualifiedName(name, namespaceIndex);
            this.DisplayName = eventType.Description;
            this.IsAbstract = false;
            this.SuperTypeId = AeParsedNodeId.Construct(eventType.EventTypeMapping, namespaceIndex);

            // add the attributes as properties.
            if (eventType.Attributes != null)
            {
                for (int ii = 0; ii < eventType.Attributes.Count; ii++)
                {
                    string propertyName = eventType.Attributes[ii].Description;

                    if (AeTypeCache.IsKnownName(propertyName, "ACK COMMENT"))
                    {
                        continue;
                    }

                    PropertyState property = new PropertyState(this);

                    property.SymbolicName = propertyName;
                    property.ReferenceTypeId = Opc.Ua.ReferenceTypeIds.HasProperty;
                    property.TypeDefinitionId = Opc.Ua.VariableTypeIds.PropertyType;
                    property.ModellingRuleId = Opc.Ua.ObjectIds.ModellingRule_Optional;
                    property.NodeId = AeParsedNodeId.Construct(eventType, propertyName, namespaceIndex);
                    property.BrowseName = new QualifiedName(propertyName, namespaceIndex);
                    property.DisplayName = property.BrowseName.Name;
                    property.AccessLevel = AccessLevels.None;
                    property.UserAccessLevel = AccessLevels.None;
                    property.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                    property.Historizing = false;

                    bool isArray = false;
                    property.DataType = ComUtils.GetDataTypeId(eventType.Attributes[ii].VarType, out isArray);
                    property.ValueRank = (isArray) ? ValueRanks.OneDimension : ValueRanks.Scalar;

                    this.AddChild(property);
                }
            }
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Gets the event type metadata.
        /// </summary>
        public EventType EventType
        {
            get { return m_eventType; }
        }
        #endregion

        BaseEventState ConstructEvent(ONEVENTSTRUCT e)
        {
            return null;
        }

        #region Private Fields
        private EventType m_eventType;
        #endregion
    }

    /// <summary>
    /// Stores information about an abstract event type that groups AE events in the type hierarchy.
    /// </summary>
    internal class AeEventTypeMappingState : BaseObjectTypeState
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AeEventTypeMappingState"/> class.
        /// </summary>
        public AeEventTypeMappingState(EventTypeMapping eventType, ushort namespaceIndex)
        {
            m_eventType = eventType;

            // create the name for the event type.
            string name = "COMAE" + eventType.ToString();

            // the attributes.
            this.NodeId = AeParsedNodeId.Construct(eventType, namespaceIndex);
            this.BrowseName = new QualifiedName(name, namespaceIndex);
            this.DisplayName = this.BrowseName.Name;
            this.IsAbstract = true;

            // set the supertype.
            switch (eventType)
            {
                case EventTypeMapping.AlarmConditionType: { SuperTypeId = Opc.Ua.ObjectTypeIds.AlarmConditionType; break; }
                case EventTypeMapping.AuditEventType: { SuperTypeId = Opc.Ua.ObjectTypeIds.AuditEventType; break; }
                case EventTypeMapping.BaseEventType: { SuperTypeId = Opc.Ua.ObjectTypeIds.BaseEventType; break; }
                case EventTypeMapping.DeviceFailureEventType: { SuperTypeId = Opc.Ua.ObjectTypeIds.DeviceFailureEventType; break; }
                case EventTypeMapping.DiscreteAlarmType: { SuperTypeId = Opc.Ua.ObjectTypeIds.DiscreteAlarmType; break; }
                case EventTypeMapping.NonExclusiveDeviationAlarmType: { SuperTypeId = Opc.Ua.ObjectTypeIds.NonExclusiveDeviationAlarmType; break; }
                case EventTypeMapping.ExclusiveLevelAlarmType: { SuperTypeId = Opc.Ua.ObjectTypeIds.ExclusiveLevelAlarmType; break; }
                case EventTypeMapping.LimitAlarmType: { SuperTypeId = Opc.Ua.ObjectTypeIds.LimitAlarmType; break; }
                case EventTypeMapping.NonExclusiveLevelAlarmType: { SuperTypeId = Opc.Ua.ObjectTypeIds.NonExclusiveLevelAlarmType; break; }
                case EventTypeMapping.OffNormalAlarmType: { SuperTypeId = Opc.Ua.ObjectTypeIds.OffNormalAlarmType; break; }
                case EventTypeMapping.SystemEventType: { SuperTypeId = Opc.Ua.ObjectTypeIds.SystemEventType; break; }
                case EventTypeMapping.TripAlarmType: { SuperTypeId = Opc.Ua.ObjectTypeIds.TripAlarmType; break; }
                case EventTypeMapping.ConditionClassType: { SuperTypeId = Opc.Ua.ObjectTypeIds.BaseConditionClassType; break; }
            }
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Gets the event type metadata.
        /// </summary>
        public EventTypeMapping EventType
        {
            get { return m_eventType; }
        }
        #endregion

        #region Private Fields
        private EventTypeMapping m_eventType;
        #endregion
    }
}
