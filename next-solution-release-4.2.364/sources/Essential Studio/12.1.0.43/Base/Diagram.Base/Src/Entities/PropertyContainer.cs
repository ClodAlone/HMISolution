#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A style is an object that encapsulates one or more properties
    /// in a property container.
    /// </summary>
    /// <remarks>
    /// Style objects provide a wrapper for a collection of properties in
    /// a property container. Getting and setting properties through a style
    /// object is easier than doing it through the IPropertyContainer interface
    /// because the style object provides type-safe wrappers for each property.
    /// The properties exposed through a style object are also browsable in
    /// the property grid.
    /// </remarks>
    public abstract class PropertyContainer
        : IServiceReferenceHolder,
          IServiceReferenceProvider,
          IPropertyObserver,
          IPropertyContainer,
          ICloneable,
          ISerializable
    {
        #region Class members
        private EventSink m_eventSink;
        private IServiceReferenceProvider m_provider;
        private HistoryManager m_mgrHistoryManager;
        protected IPropertyObserver m_propertyObserver;
        private IPropertyContainer m_owner;

        /// <summary>
        /// Container measure units.
        /// </summary>
        private MeasureUnits m_units;

        /// <summary>
        /// Indicates whether node's units will change on its container's MeasureUnits changing.
        /// </summary>
        private bool m_bInheritContainerUnits;
        #endregion

        #region Class initialize/finalize members
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyContainer"/> class.
        /// </summary>
        public PropertyContainer()
        {
            m_units = MeasureUnits.Pixel;
            m_bInheritContainerUnits = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyContainer"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public PropertyContainer(PropertyContainer src)
        {
            m_units = src.m_units;
            m_bInheritContainerUnits = src.m_bInheritContainerUnits;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyContainer"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected PropertyContainer(SerializationInfo info, StreamingContext context)
        {
            m_bInheritContainerUnits = info.GetBoolean("inheritContainerUnits");
            m_units = (MeasureUnits)info.GetValue("measureUnits", typeof(MeasureUnits));
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the property observer.
        /// </summary>
        /// <value>The property observer.</value>
        protected IPropertyObserver PropertyObserber
        {
            get
            {
                return m_propertyObserver;
            }
        }

        /// <summary>
        /// Gets or sets the measure unit.
        /// </summary>
        /// <value>The measure unit.</value>
        [Browsable(true)]
        [DefaultValue(MeasureUnits.Pixel)]
        [Category("Behavior")]
        [Description("Logical unit of measurement.")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual MeasureUnits MeasureUnit
        {
            get 
            { 
                return m_units; 
            }
            set
            {
                if (m_units != value)
                {
                    OnMeasureUnitsChanging(m_units, value);

                    m_units = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether current instance inherit container measure units.
        /// </summary>
        /// <value>
        /// <c>true</c> if current instance inherit container measure units; otherwise, <c>false</c>.
        /// </value>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Indicates whether node update its measure units when node's container will change its measure units.")]
        public virtual bool InheritContainerMeasureUnits
        {
            get 
            { 
                return m_bInheritContainerUnits; 
            }
            set
            {
                if (m_bInheritContainerUnits != value && OnPropertyChanging(DPN.InheritContainerMeasureUnits, value))
                {
                    //// make history entry
                    RecordPropertyChanged(DPN.InheritContainerMeasureUnits);
                    //// set new value
                    m_bInheritContainerUnits = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.InheritContainerMeasureUnits);
                }
            }
        }

        /// <summary>
        /// Gets the history manager service.
        /// </summary>
        /// <value>The history manager service.</value>
        protected HistoryManager HistoryService
        {
            get { return m_mgrHistoryManager; }
        }
        #endregion

        #region IServiceReferenceHolder Members
        /// <summary>
        /// Updates the service references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public virtual void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            if (provider == null)
            {
                m_mgrHistoryManager = null;
                m_propertyObserver = null;
                m_owner = null;
                m_eventSink = null;
            }
            else
            {
                m_mgrHistoryManager = (HistoryManager)provider.ProvideServiceReference(typeof(HistoryManager).TypeHandle);
                m_propertyObserver = (IPropertyObserver)provider.ProvideServiceReference(typeof(IPropertyObserver).TypeHandle);
                m_owner = (IPropertyContainer)provider.ProvideServiceReference(typeof(IPropertyContainer).TypeHandle);
                m_eventSink = (EventSink)provider.ProvideServiceReference(typeof(EventSink).TypeHandle);
            }

            m_provider = provider;
        }
        #endregion

        #region IServiceReferenceProvider Members
        /// <summary>
        /// Get the service reference from provider.
        /// </summary>
        /// <param name="typeHandle">The type handle.</param>
        /// <returns>The object.</returns>
        public virtual object ProvideServiceReference(RuntimeTypeHandle typeHandle)
        {
            object objToReturn = null;

            if (m_provider != null)
            {
                objToReturn = m_provider.ProvideServiceReference(typeHandle);
            }

            return objToReturn;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Records the property changed.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        protected virtual void RecordPropertyChanged(string strPropertyName)
        {
            if (m_mgrHistoryManager != null)
            {
                m_mgrHistoryManager.RecordPropertyChanged((IPropertyContainer)m_propertyObserver, this.FullContainerName, strPropertyName);
            }
        }
        #endregion

        #region IPropertyObserver Members
        /// <summary>
        /// Called when property changing.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>true, if property is changing.</returns>
        protected virtual bool OnPropertyChanging(string strPropertyName, object newValue)
        {
            bool bSuccess = true;

            if (m_propertyObserver != null)
            {
                bSuccess = m_propertyObserver.OnPropertyChanging(this.FullContainerName, strPropertyName, newValue);
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string strPropertyName)
        {
            if (m_propertyObserver != null)
            {
                m_propertyObserver.OnPropertyChanged(this.FullContainerName, strPropertyName);
            }
        }
        #endregion

        #region IPropertyContainerr Members
        /// <summary>
        /// Gets the full name of the container.
        /// </summary>
        /// <value>The full name of the container.</value>
        [Browsable(false)]
        public string FullContainerName
        {
            get
            {
                string strContainerToReturn = GetPropertyContainerName();

                if (m_owner != null)
                {
                    string strContainerName = m_owner.FullContainerName;

                    if (strContainerName != string.Empty)
                    {
                        strContainerToReturn = strContainerName + "." + strContainerToReturn;
                    }
                }

                return strContainerToReturn;
            }
        }

        /// <summary>
        /// Gets the container of the property by name.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <returns>The property container.</returns>
        public virtual object GetPropertyContainerByName(string strPropertyName)
        {
            return null;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Called when measure units changing.
        /// </summary>
        /// <param name="from">The old value.</param>
        /// <param name="to">The new value.</param>
        protected virtual void OnMeasureUnitsChanging(MeasureUnits from, MeasureUnits to)
        { 
        }

        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>The property container name.</returns>
        protected abstract string GetPropertyContainerName();

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("inheritContainerUnits", m_bInheritContainerUnits);
            info.AddValue("measureUnits", m_units);
        }
        #endregion

        #region ISerializable Members
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }
        #endregion

        #region ICLoneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public abstract object Clone();
        #endregion

        #region IPropertyObserver Members

        bool IPropertyObserver.OnPropertyChanging(string strPropertyContainerName, string strPropertyName, object newValue)
        {
            bool bSuccess = true;

            if (m_eventSink != null)
            {
                string strPropName = strPropertyName;

                if (strPropertyContainerName != string.Empty)
                {
                    strPropName = strPropertyContainerName + "." + strPropertyName;
                }

                bSuccess = m_eventSink.RaisePropertyChangingEvent(this, strPropName, newValue);
            }

            return bSuccess;
        }

        void IPropertyObserver.OnPropertyChanged(string strPropertyContainerName, string strPropertyName)
        {
            if (m_eventSink != null)
            {
                string strPropName = strPropertyName;

                if (strPropertyContainerName != string.Empty)
                {
                    strPropName = strPropertyContainerName + "." + strPropertyName;
                }

                m_eventSink.RaisePropertyChangedEvent(this, strPropName);
            }
        }

        #endregion
    }
}
