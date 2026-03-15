using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UFInterfaces;
using Opc.Ua;
using ViewModelLib;

namespace OPCUAViewModel
{
    public class UseOPCUAEntityReference : IDisposable
    {
        #region Declarations
        OPCUAEntityReference entityReference;
        IEntityReference subscriber;
        #endregion

        #region Constructors
        private UseOPCUAEntityReference() { }

        public UseOPCUAEntityReference(String sessionName, OPCUAEntityReference reference, IEntityReference s)
        {
            entityReference = reference;
            subscriber = s;
            if (entityReference == null)
                throw new ArgumentNullException("OPCUAEntityReference");
            if (subscriber == null)
                throw new ArgumentNullException("IEntityReference");
            entityReference.Resolve(sessionName);
            entityReference.SetInUse(subscriber, true);
        }
        #endregion

        #region IDisposable Members

        ~UseOPCUAEntityReference()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                GC.SuppressFinalize(this);

            entityReference.SetInUse(subscriber, false);
        }

        public void Dispose()
        {
            Dispose(false);
        }

        #endregion
    }
}
