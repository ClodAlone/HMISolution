////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ComunicationStateArgs.cs
//
// summary:	Implements the system event arguments class
////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverBaseInterfaces
{
    public enum ComunicationState 
    {
        Fault,
        Suspend,
        Running
    };

    /// <summary>   Additional information for Drivers state events. </summary>
    public class ComunicationStateArgs : EventArgs
    {
        /// <summary>   Last State. </summary>
        public ComunicationState NewState ;
        /// <summary>   Previous Status. </summary>
        public ComunicationState OldState ;
    }
}
