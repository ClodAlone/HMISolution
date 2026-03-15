////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ICommunicationDriver.cs
//
// summary:	Declares the ICommunicationDriver interface
////////////////////////////////////////////////////////////////////////////////////////////////////

using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBaseInterfaces
{
    /// <summary>   Interface for communication driver. </summary>
    public interface ICommunicationDriver2 : ICommunicationDriver
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initialises this object. </summary>
        ///
        /// <param name="strSettingPath" type="String"> Full pathname of the setting file. </param>
        /// <param name="isProtected" type="String"> true if the project is protected. </param>
        /// <param name="protectionCode" type="String"> unique indentifier to check. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Init(String strSettingPath, bool isProtected, Guid protectionCode);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets driver state. </summary>
        ///
        /// <returns>   The internal driver state. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ComunicationState GetDriverState();

        /// <summary>   Event queue for all listeners interested in change of drivers state. </summary>
        event EventHandler<ComunicationStateArgs> StateChanged;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the write tag action. </summary>
        ///
        /// <param name="tag"> the tag </param>
        /// <param name="value"> the value </param>
        /// <param name="statusCode"> result of write operation </param>
        /// <param name="timestamp"> date/time of write operation </param>
        /// <param name="ignoreWriteAsync"> force driver to ignore ignoreWriteAsync paramter </param>
        /// <param name="forceSynchWrite"> force driver to write sinchronously</param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint OnWriteTag(TagDefinition tag, ref object value, ref StatusCode statusCode, ref DateTime timestamp, bool ignoreWriteAsync = false, bool forceSynchWrite = false);
    }
}