using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Server;

namespace ServerScriptManager
{
    [Serializable]
    public class ServerScriptManager
    {
        #region ctor
        public ServerScriptManager(BaseInstanceState i, StandardServer s, ServerSystemContext sc,
            Dictionary<String, BaseInstanceState> pn, CustomNodeManager2 nm)
        {
            instance = i;
            server = s;
            serverSystemContext = sc;
            predefinedNodes = pn;
            nodeManager = nm;
        }
        private ServerScriptManager()
        {
            
        }
         
        #endregion

        #region Properties
        public bool ForceWrite { get; set; }

        public BaseInstanceState Instance
        {
            get
            {
                return instance;
            }
        }

        public StandardServer Server
        {
            get
            {
                return server;
            }
        }

        public Object Value
        {
            get
            {
                ValidateValue();
                var variable = instance as BaseVariableState;

                return variable.Value;
            }
            set
            {
                ValidateValue();
                var variable = instance as BaseVariableState;

                var v = Opc.Ua.TypeInfo.Cast(value, TypeInfo.GetBuiltInType(variable.DataType));
                if (!ForceWrite && variable.Value != null && variable.Value.Equals(v))
                    return;

                if (variable.OnWriteValue != null)
                {
                    var statusCode = variable.StatusCode;
                    var timeStamp = DateTime.UtcNow;
                    var result = variable.OnWriteValue(
                        serverSystemContext,
                        variable,
                        NumericRange.Empty,
                        null,
                        ref v,
                        ref statusCode,
                        ref timeStamp);

                    if (ServiceResult.IsBad(result))
                    {
                        throw new ServiceResultException(result);
                    }

                    variable.Value = v;
                    variable.StatusCode = statusCode;
                    variable.Timestamp = timeStamp;

                    instance.ClearChangeMasks(serverSystemContext, true);

                }
                else
                {
                    variable.Value = v;
                    instance.ClearChangeMasks(serverSystemContext, true);
                }
            }
        }

        public DateTime Timestamp
        {
            get
            {
                ValidateValue();
                var variable = instance as BaseVariableState;

                return variable.Timestamp;
            }
            set
            {
                ValidateValue();
                var variable = instance as BaseVariableState;

                variable.Timestamp = value;
                instance.ClearChangeMasks(serverSystemContext, true);
            }
        }

        public StatusCode StatusCode
        {
            get
            {
                ValidateValue();
                var variable = instance as BaseVariableState;

                return variable.StatusCode;
            }
            set
            {
                ValidateValue();
                var variable = instance as BaseVariableState;

                variable.StatusCode = value;
                instance.ClearChangeMasks(serverSystemContext, true);
            }
        }

        Dictionary<String, ServerScriptManager> mapTagManagers;

        public ServerScriptManager GetTag(String name)
        {
            if (predefinedNodes == null)
                return null;
            name = name.Replace(':', '\\');
            lock (serverSystemContext)
            {
                if (mapTagManagers == null)
                    mapTagManagers = new Dictionary<String, ServerScriptManager>();
                if (mapTagManagers.ContainsKey(name))
                    return mapTagManagers[name];
            }
            BaseInstanceState found = null;
            if (!predefinedNodes.ContainsKey(name) || !(predefinedNodes[name] is BaseInstanceState))
            {
                var nodeid = NodeId.Parse(name);

                found = nodeManager.Find(nodeid) as BaseInstanceState;
                if (found == null)
                    return null;
            }
            else
                found = predefinedNodes[name];

            OnInUseTag(found);
            var scriptManager = new ServerScriptManager(found, server, serverSystemContext, predefinedNodes, nodeManager);
            lock(serverSystemContext)
            {
                mapTagManagers.Add(name, scriptManager);
                return mapTagManagers[name];
            }
        }
        #endregion

        #region Methods
        void ValidateValue()
        {
            if (serverSystemContext == null || instance == null || !(instance is BaseVariableState))
                throw new ArgumentException("Instance Variable not initialized");
        }
        #endregion

        #region members
        readonly BaseInstanceState instance;
        readonly StandardServer server;
        readonly CustomNodeManager2 nodeManager;
        readonly ServerSystemContext serverSystemContext;
        readonly Dictionary<String, BaseInstanceState> predefinedNodes;
        #endregion

        #region Events
        public event EventHandler Init;
        #region OnInit
        /// <summary>
        /// Triggers the Init event.
        /// </summary>
        public virtual void OnInit(EventArgs ea)
        {
            var e = Init;
            if (e != null)
                e(this, ea);
        }
        #endregion

        public event EventHandler Terminate;
        #region OnTerminate
        /// <summary>
        /// Triggers the Terminate event.
        /// </summary>
        public virtual void OnTerminate(EventArgs ea)
        {
            var e = Terminate;
            if (e != null)
                e(this, ea);
        }
        #endregion

        public event EventHandler DoEvents;
        #region OnDoEvents
        /// <summary>
        /// Triggers the DoEvents event.
        /// </summary>
        public virtual void OnDoEvents(EventArgs ea)
        {
            var e = DoEvents;
            if (e != null)
                e(this, ea);
        }
        #endregion

        public event EventHandler InUseTag;
        #region OnInUseTag
        /// <summary>
        /// Triggers the DoEvents event.
        /// </summary>
        public virtual void OnInUseTag(NodeState node)
        {
            var e = InUseTag;
            if (e != null)
                e(node, EventArgs.Empty);
        }
        #endregion
        #endregion
    }
}
