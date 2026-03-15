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
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Opc.Ua.Server;

namespace Opc.Ua.Com.Client
{
    /// <summary>
    /// Stores the configuration the data access node manager.
    /// </summary>
    [DataContract(Namespace = Namespaces.ComInterop)]
    public class ComHdaClientConfiguration : ComClientConfiguration
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public ComHdaClientConfiguration()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the object during deserialization.
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private void Initialize()
        {
            m_addCapabilitiesToServerObject = false;
            m_attributeSamplingInterval = 1000;
            m_treatUncertainAsBad = true;
            m_percentDataBad = 0;
            m_percentDataGood = 100;
            m_steppedSlopedExtrapolation = false;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating whether the history server capabilities should be added to the server object.
        /// </summary>
        /// <value>
        /// <c>true</c> if the history server capabilities should be added to the server object; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Order = 1)]
        public bool AddCapabilitiesToServerObject
        {
            get { return m_addCapabilitiesToServerObject; }
            set { m_addCapabilitiesToServerObject = value; }
        }

        /// <summary>
        /// Gets or sets the attribute sampling interval.
        /// </summary>
        /// <value>The attribute sampling interval.</value>
        [DataMember(Order=2)]
        public int AttributeSamplingInterval
        {
            get { return m_attributeSamplingInterval; }
            set { m_attributeSamplingInterval = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HDA server treats uncertain values as bad.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the HDA server treats uncertain values as bad; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Order=3)]
        public bool TreatUncertainAsBad
        {
            get { return m_treatUncertainAsBad; }
            set { m_treatUncertainAsBad = value; }
        }

        /// <summary>
        /// Gets or sets the percent data that is bad before the HDA server treats the entire interval as bad.
        /// </summary>
        /// <value>The percent data bad.</value>
        [DataMember(Order=4)]
        public byte PercentDataBad
        {
            get { return m_percentDataBad; }
            set { m_percentDataBad = value; }
        }
        /// <summary>
        /// Gets or sets the percent data after which the HDA server treats the entire interval as dood.
        /// </summary>
        /// <value>The percent data good.</value>
        [DataMember(Order=5)]
        public byte PercentDataGood
        {
            get { return m_percentDataGood; }
            set { m_percentDataGood = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HDA server sloped extrapolation to calculate end bounds.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the HDA server sloped extrapolation; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Order=6)]
        public bool SteppedSlopedExtrapolation
        {
            get { return m_steppedSlopedExtrapolation; }
            set { m_steppedSlopedExtrapolation = value; }
        }
        #endregion

        #region Private Members
        private bool m_addCapabilitiesToServerObject;
        private int m_attributeSamplingInterval;
        private bool m_treatUncertainAsBad;
        private byte m_percentDataBad;
        private byte m_percentDataGood;
        private bool m_steppedSlopedExtrapolation;
        #endregion
    }
}
