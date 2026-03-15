#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Represents tasks that make up the project
    /// </summary>
    [System.Serializable()]
    public class Tasks
    {
        #region Fields
        private Task m_rootTask;
        private List<Task> m_children = new List<Task>();
        #endregion

        #region Initiliazer
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Tasks(){ }
        #endregion

        #region Properties
        [XmlIgnore()]
        public Task RootTask
        {
            get
            {
                return this.m_rootTask;
            }
            set
            {
                this.m_rootTask = value;
            }
        }

        [XmlElement("Task")]
        public List<Task> Children
        {
            get
            {
                return this.m_children;
            }
            set
            {
                this.m_children = value;
            }
        }
        #endregion
    }
}
