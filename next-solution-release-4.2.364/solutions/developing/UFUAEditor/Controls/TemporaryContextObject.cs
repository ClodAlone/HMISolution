using System;
using System.Collections;
using UFInterfaces;

namespace UFUAEditor.Controls
{
    internal class TemporaryContextObject : IDisposable
    {
        #region Declarations
        readonly IWorkspace workspace;
        readonly object contextObject;
        readonly IList contextObjects;
        #endregion

        #region Constructors
        public TemporaryContextObject(IWorkspace workspace, object temporaryContext)
        {
            this.workspace = workspace;
            contextObject = workspace.ContextObject;
            contextObjects = workspace.ContextObjects;

            workspace.ContextObject = temporaryContext;
        }

        public TemporaryContextObject(IWorkspace workspace, IList temporaryContext)
        {
            this.workspace = workspace;
            contextObject = workspace.ContextObject;
            contextObjects = workspace.ContextObjects;

            workspace.ContextObjects = temporaryContext;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (contextObject != null)
                workspace.ContextObject = contextObject;
            else if (contextObjects != null)
                workspace.ContextObjects = contextObjects;
            else
                workspace.ContextObject = null;
        }

        #endregion
    }
}
