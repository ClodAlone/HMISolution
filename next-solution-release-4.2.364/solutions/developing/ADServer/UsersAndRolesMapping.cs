using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADServer
{
    public class UsersAndRolesMapping
    {
        #region Properties
        Dictionary<string, UFUserModel.UFRole> nameRoles;
        public Dictionary<string, UFUserModel.UFRole> NameRoles
        {
            get
            {
                if (nameRoles == null)
                    nameRoles = new Dictionary<string, UFUserModel.UFRole>();
                return nameRoles;
            }
        }
        Dictionary<string, UFUserModel.UFUser> nameUsers;
        public Dictionary<string, UFUserModel.UFUser> NameUsers
        {
            get
            {
                if (nameUsers == null)
                    nameUsers = new Dictionary<string, UFUserModel.UFUser>();
                return nameUsers;
            }
        }

        Dictionary<Opc.Ua.NodeId, UFUserModel.UFRole> nodeIdRoles;
        public Dictionary<Opc.Ua.NodeId, UFUserModel.UFRole> NodeIdRoles
        {
            get
            {
                if (nodeIdRoles == null)
                    nodeIdRoles = new Dictionary<Opc.Ua.NodeId, UFUserModel.UFRole>();

                return nodeIdRoles;
            }
        }

        Dictionary<Opc.Ua.NodeId, UFUserModel.UFUser> nodeIdUsers;
        public Dictionary<Opc.Ua.NodeId, UFUserModel.UFUser> NodeIdUsers
        {
            get
            {
                if (nodeIdUsers == null)
                    nodeIdUsers = new Dictionary<Opc.Ua.NodeId, UFUserModel.UFUser>();

                return nodeIdUsers;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (nodeIdRoles == null || nodeIdRoles.Count == 0) &&
                    (nodeIdUsers == null || nodeIdUsers.Count == 0) && 
                    (nameRoles == null || nameRoles.Count == 0) && 
                    (nameUsers == null || nameUsers.Count == 0);
            }
        }
        #endregion
    }
}
