using System;

namespace UFUAServerBase.Model
{
    internal class AreaState : Opc.Ua.FolderState
    {
        #region Constructors
        /// <summary>
        /// Initializes the instance with its defalt attribute values.
        /// </summary>
        public AreaState(Opc.Ua.NodeState parent) : base(parent)
        {
        }
        #endregion
    }
}
