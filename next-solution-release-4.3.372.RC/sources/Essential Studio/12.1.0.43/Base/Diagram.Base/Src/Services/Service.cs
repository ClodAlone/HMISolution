#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Service class.
    /// </summary>
    public abstract class Service
    {
        #region Class members
        private ServiceStatus m_serviceStatus;
        private int m_nRequests;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the service status state.
        /// </summary>
        /// <value>The service status.</value>
        public ServiceStatus ServiceStatus
        {
            get { return m_serviceStatus; }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Occurs when service is stopped.
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Occurs when service is started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Occurs when service is paused.
        /// </summary>
        public event EventHandler Paused;

        /// <summary>
        /// Occurs when service is resumed.
        /// </summary>
        public event EventHandler Resumed;
        #endregion

        #region Class utility methods
        /// <summary>
        /// Starts this service.
        /// </summary>
        public void Start()
        {
            if (m_serviceStatus == ServiceStatus.Stopped)
            {
                m_serviceStatus = Diagram.ServiceStatus.Started;

                OnStart();
            }
        }

        /// <summary>
        /// Stops this service.
        /// </summary>
        public void Stop()
        {
            if (m_serviceStatus != Diagram.ServiceStatus.Stopped)
            {
                m_serviceStatus = Diagram.ServiceStatus.Stopped;

                // reset requests
                m_nRequests = 0;

                OnStop();
            }
        }

        /// <summary>
        /// Resumes this service.
        /// </summary>
        public void Resume()
        {
            if (m_serviceStatus != ServiceStatus.Stopped)
            {
                --m_nRequests;

                if (m_nRequests == 0)
                {
                    m_serviceStatus = Diagram.ServiceStatus.Resumed;

                    OnResume();
                }
            }
        }

        /// <summary>
        /// Pauses this service.
        /// </summary>
        public void Pause()
        {
            if (m_serviceStatus != ServiceStatus.Stopped)
            {
                m_serviceStatus = Diagram.ServiceStatus.Paused;

                m_nRequests++;

                OnPause();
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Called when service is stoped.
        /// </summary>
        protected virtual void OnStop()
        {
            if (Stopped != null)
                Stopped(this, new EventArgs());
        }

        /// <summary>
        /// Called when service is started.
        /// </summary>
        protected virtual void OnStart()
        {
            if (Started != null)
                Started(this, new EventArgs());
        }

        /// <summary>
        /// Called when service is resumed.
        /// </summary>
        protected virtual void OnResume()
        {
            if (Resumed != null)
                Resumed(this, new EventArgs());
        }

        /// <summary>
        /// Called when service is paused.
        /// </summary>
        protected virtual void OnPause()
        {
            if (Paused != null)
                Paused(this, new EventArgs());
        }
        #endregion
    }
}
