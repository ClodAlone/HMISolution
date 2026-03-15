using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Constants;

namespace UFUAEditor.RemovedVariables
{
    [DataContract(Name = "RemovedVariablesDocument", Namespace = Namespaces.UriProgea)]
    internal class RemovedVariablesDocument
    {
        #region Persistance
        [DataMember]
        CollectionVariableGuids addressSpaceTags;
        [DataMember]
        CollectionVariableGuids prototypeMembers;

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        { }

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            if (addressSpaceTags != null)
            {
                if (removedNodeIds == null)
                    removedNodeIds = new Dictionary<Guid, String>();
                foreach (var key in addressSpaceTags.Keys)
                    removedNodeIds.Add(addressSpaceTags[key], key);
            }

            if (prototypeMembers != null)
            {
                if (removedNodeIds == null)
                    removedNodeIds = new Dictionary<Guid, String>();
                foreach (var key in prototypeMembers.Keys)
                    removedNodeIds.Add(prototypeMembers[key], key);
            }
        }
        #endregion

        #region Declarations
        Dictionary<Guid, String> removedNodeIds;
        #endregion

        #region Methods
        public void Clear()
        {
            if (addressSpaceTags != null)
                addressSpaceTags.Clear();
            if (prototypeMembers != null)
                prototypeMembers.Clear();
            if (removedNodeIds != null)
                removedNodeIds.Clear();
        }
        #endregion

        #region Properties
        public IDictionary AddressSpaceTags
        {
            get
            {
                if (addressSpaceTags == null)
                    addressSpaceTags = new CollectionVariableGuids();

                return addressSpaceTags;
            }
        }

        public IDictionary PrototypeMembers
        {
            get
            {
                if (prototypeMembers == null)
                    prototypeMembers = new CollectionVariableGuids();

                return prototypeMembers;
            }
        }

        public IDictionary RemovedNodeIds
        {
            get
            {
                if (removedNodeIds == null)
                    removedNodeIds = new Dictionary<Guid, String>();

                return removedNodeIds;
            }
        }
        #endregion
    }
}
