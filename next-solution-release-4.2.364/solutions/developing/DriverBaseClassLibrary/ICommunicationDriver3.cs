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
    public interface ICommunicationDriver3 : ICommunicationDriver2
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initialises this object. </summary>
        ///
        /// <param name="strSettingPath" type="String"> Full pathname of the setting file. </param>
        /// <param name="xpoConnectionString" type="String"> Xpo Connection String for historiacal data. </param>
        /// <param name="isProtected" type="String"> true if the project is protected. </param>
        /// <param name="protectionCode" type="String"> unique indentifier to check. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Init(String strSettingPath, String xpoConnectionString, bool isProtected, Guid protectionCode);
    }
}