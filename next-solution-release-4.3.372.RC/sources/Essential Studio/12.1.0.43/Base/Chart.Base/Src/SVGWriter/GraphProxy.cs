#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Syncfusion.Windows.Forms.Chart
{
    #region GraphProxyEventArgs
    /// <summary>
    /// Represents the data of <see cref="GraphProxyEventHandler"/>.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GraphProxyEventArgs : EventArgs
    {
        #region Members
        private IDictionary m_properties;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public IDictionary Properties
        {
            get
            {
                return m_properties;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphProxyEventArgs"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public GraphProxyEventArgs(IDictionary properties)
        {
            m_properties = properties;
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// Represents the method that handles the <see cref="GraphProxy.GraphInvoke"/> event.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal delegate void GraphProxyEventHandler(object sender, GraphProxyEventArgs e);

    /// <summary>
    /// Represents the proxy of <see cref="Graphics"/> instance.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GraphProxy : RealProxy
    {
        #region Members
        private Graphics m_graph;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when <see cref="Graphics"/> methods is called.
        /// </summary>
        public event GraphProxyEventHandler GraphInvoke;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphProxy"/> class.
        /// </summary>
        /// <param name="g">The g.</param>
        protected GraphProxy(Graphics g)
            : base(typeof(Graphics))
        {
            m_graph = g;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates GraphProxy instance by the specified handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="proxy">The proxy.</param>
        /// <returns>Returns Graphics object.</returns>
        public static Graphics Create(IntPtr handle, out GraphProxy proxy)
        {
            Graphics g = Graphics.FromHwnd(handle);

            proxy = new GraphProxy(g);

            return (Graphics)proxy.GetTransparentProxy();
        }

        /// <summary>
        /// Creates GraphProxy instance by the specified <see cref="Image"/>.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <param name="proxy">The proxy.</param>
        /// <returns>Returns Graphics object.</returns>
        public static Graphics Create(Image img, out GraphProxy proxy)
        {
            Graphics g = Graphics.FromImage(img);

            proxy = new GraphProxy(g);

            return (Graphics)proxy.GetTransparentProxy();
        }

        /// <summary>
        /// When overridden in a derived class, invokes the method that is specified in the provided <see cref="T:System.Runtime.Remoting.Messaging.IMessage"></see> on the remote object that is represented by the current instance.
        /// </summary>
        /// <param name="msg">A <see cref="T:System.Runtime.Remoting.Messaging.IMessage"></see> that contains a <see cref="T:System.Collections.IDictionary"></see> of information about the method call.</param>
        /// <returns>
        /// The message returned by the invoked method, containing the return value and any out or ref parameters.
        /// </returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="Infrastructure"/></PermissionSet>
        public override IMessage Invoke(IMessage msg)
        {
            if (GraphInvoke != null)
            {
                GraphInvoke(this, new GraphProxyEventArgs(msg.Properties));
            }

            return RemotingServices.ExecuteMessage(m_graph, (IMethodCallMessage)msg);
        }
        #endregion
    }
}
