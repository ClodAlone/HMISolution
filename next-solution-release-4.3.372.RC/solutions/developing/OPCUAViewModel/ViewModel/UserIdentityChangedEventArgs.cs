using System;
using Opc.Ua;

namespace OPCUAViewModel
{
    /// <summary>
    /// Provides data for the OPCUAViewModel.UserIdentityChanged event.
    /// </summary>
    public class UserIdentityChangedEventArgs : EventArgs
    {
        readonly IUserIdentity userIdentity;

        /// <summary>
        /// Initializes a new instance of the OPCUAViewModel.UserIdentityChangedEventArgs class.
        /// </summary>
        /// <param name="userIdentity">
        /// Identity of the active user.
        /// </param>
        public UserIdentityChangedEventArgs(IUserIdentity userIdentity)
        {
            this.userIdentity = userIdentity;
        }

        /// <summary>
        /// Identity of the active user.
        /// </summary>
        public IUserIdentity UserIdentity { get { return userIdentity; } }
    }
}
