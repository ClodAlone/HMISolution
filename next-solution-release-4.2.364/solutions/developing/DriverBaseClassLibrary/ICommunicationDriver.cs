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
    public interface ICommunicationDriver
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Check if the driver is enabled. </summary>
        ///
        /// <param name="strSettingPath" type="String"> Full pathname of the setting file. </param>
        ///
        /// <returns>   true if it enabled, false if it disabled. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool IsEnabled(String strSettingPath);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initialises this object. </summary>
        ///
        /// <param name="strSettingPath" type="String"> Full pathname of the setting file. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Init(String strSettingPath);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Prepares this object for use. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Startup();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Suspends this object. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Suspend();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Terminates this object. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool Terminate();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds the dynamics. </summary>
        ///
        /// <param name="tags" type="IList<TagDefinition>"> The tags. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool AddDynamics(IList<TagDefinition> tags);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   In use dynamics. </summary>
        ///
        /// <param name="tags" type="IList<TagDefinition>"> The tags. </param>
        /// <param name="bInUse" type="bool">               true to in use. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool InUseDynamics(IList<TagDefinition> tags, bool bInUse);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the write tag action. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        /// <param name="value" type="ref object">  [in,out] The value. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint OnWriteTag(TagDefinition tag, ref object value, ref StatusCode statusCode, ref DateTime timestamp);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the method call action. </summary>
        ///
        /// <param name="tag" type="TagDefinition">             The tag. </param>
        /// <param name="inputArguments" type="IList<Object>">  The input arguments. </param>
        /// <param name="outputArguments" type="IList<Object>"> The output arguments. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint OnMethodCall(TagDefinition tag, IList<Object> inputArguments, IList<Object> outputArguments);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the write values action. </summary>
        ///
        /// <param name="startingAddress" type="String">    The starting address. </param>
        /// <param name="dataValues" type="IList<object>">  The data values. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint OnWriteValues(String startingAddress, IList<object> dataValues);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the read values action. </summary>
        ///
        /// <param name="startingAddress" type="String">    The starting address. </param>
        /// <param name="dataValues" type="IList<object>">  The data values. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint OnReadValues(String startingAddress, IList<object> dataValues);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the resume diagnostic action. </summary>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint OnResumeDiagnostic();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the suspend diagnostic action. </summary>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint OnSuspendDiagnostic();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the reset diagnostic action. </summary>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint OnResetDiagnostic();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets driver name. </summary>
        ///
        /// <returns>   The driver name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        string GetDriverName();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets internal name. </summary>
        ///
        /// <returns>   The internal name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        string GetInternalName();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets driver diagram variable. </summary>
        ///
        /// <returns>   The driver diagram variable. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        List<StatisicTag> GetDriverDiagVar();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets channel diagram variable. </summary>
        ///
        /// <returns>   The channel diagram variable. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        List<StatisicTag> GetChannelDiagVar();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets station diagram variable. </summary>
        ///
        /// <returns>   The station diagram variable. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        List<StatisicTag> GetStationDiagVar();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets diagnostic folder. </summary>
        ///
        /// <param name="RootDriversGuid" type="string">    Unique identifier for the root drivers. </param>
        /// <param name="NamespaceIndex" type="ushort">     Zero-based index of the namespace. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint SetDiagnosticFolder(string RootDriversGuid, ushort NamespaceIndex);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets channels names. </summary>
        ///
        /// <returns>   The channels names. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        List<string> GetChannelsNames();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets stations names. </summary>
        ///
        /// <returns>   The stations names. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        List<string> GetStationsNames();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets nodeid to observe. </summary>
        ///
        /// <returns>   The nodeid to observe. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        List<NodeId> GetObservingNodes();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates nodeid values of observed nodeid. </summary>
        ///
        /// <returns>   Updates nodeid values of observed nodeid. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdateObservedTag(NodeId node, DataValue value);

        /// <summary>   Event queue for all listeners interested in Starting events. </summary>
        event EventHandler Starting;
        /// <summary>   Event queue for all listeners interested in Started events. </summary>
        event EventHandler Started;

        /// <summary>   Event queue for all listeners interested in Suspending events. </summary>
        event EventHandler Suspending;
        /// <summary>   Event queue for all listeners interested in Suspended events. </summary>
        event EventHandler Suspended;

        /// <summary>   Event queue for all listeners interested in Terminating events. </summary>
        event EventHandler Terminating;
        /// <summary>   Event queue for all listeners interested in Terminated events. </summary>
        event EventHandler Terminated;

        /// <summary>   Event queue for all listeners interested in TagChanging events. </summary>
        event EventHandler<ChangedTagArgs> TagChanging;
        /// <summary>   Event queue for all listeners interested in TagChanged events. </summary>
        event EventHandler<ChangedTagArgs> TagChanged;

        /// <summary>   Event queue for all listeners interested in system events. </summary>
        event EventHandler<SystemEventArgs> SystemEvent;
        /// <summary>   Event queue for all listeners interested in audi events. </summary>
        event EventHandler<AuditEventArgs> AudiEvent;

        /// <summary>   Event queue for all listeners interested in TagPrototypeQuery events. </summary>
        event EventHandler<TagPrototypeArgs> TagPrototypeQuery;
    }
}