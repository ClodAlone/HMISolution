using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TempVariablesManager.Helpers
{
    internal enum TypeObject : int
    {
        Variable = 0x1,
        Folder = 0x3,
    }

    internal class GuidHelper
    {
        #region Declarations
        readonly Session session;
        readonly TypeObject type;
        #endregion

        #region Constructors
        public GuidHelper(Session session, TypeObject type)
        {
            this.session = session;
            this.type = type;
        }
        #endregion

        #region Methods
        public bool Contains(Guid guid)
        {
            return Guids.Contains(guid);
        }

        public void EnsureValidGuid(TempVariablesModel.Folder folder)
        {
            if (Guids.Contains(folder.NodeId))
                folder.NodeId = Guid.NewGuid();

            foreach (var subfolder in folder.Folders)
                EnsureValidGuid(subfolder);

            foreach (var variable in folder.Variables)
                EnsureValidGuid(variable);
        }

        public void EnsureValidGuid(TempVariablesModel.Variable variable)
        {
            if (Guids.Contains(variable.NodeId))
                variable.NodeId = Guid.NewGuid();
        }
        #endregion

        #region Properties
        List<Guid> guids;
        public List<Guid> Guids
        {
            get
            {
                if (guids == null)
                {
                    guids = new List<Guid>();
                    if ((type & TypeObject.Variable) == TypeObject.Variable)
                    {
                        guids.AddRange((from p in new XPQuery<TempVariablesModel.Variable>(session, true).AsParallel()
                                        select p.NodeId).ToList());
                    }
                    if ((type & TypeObject.Folder) == TypeObject.Folder)
                    {
                        guids.AddRange((from p in new XPQuery<TempVariablesModel.Folder>(session, true).AsParallel()
                                        select p.NodeId).ToList());
                    }
                }

                return guids;
            }
        }
        #endregion
    }
}
