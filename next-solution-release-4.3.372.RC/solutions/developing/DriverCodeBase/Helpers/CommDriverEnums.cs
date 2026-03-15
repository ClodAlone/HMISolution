////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Helpers\CommDriverEnums.cs
//
// summary:	Implements the communications driver enums class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DriverCodeBase.Converters;

namespace DriverCodeBase.Enumerators
{
    #region Enumerators

    /// <summary>   Values that represent DataType. </summary>
    public enum DataType
    {
        /// <summary>   An enum constant representing the undefined option. </summary>
        Undefined = -1,
        /// <summary>   An enum constant representing the byte option. </summary>
        SByte,
        /// <summary>   An enum constant representing the int 16 option. </summary>
        Int16,
        /// <summary>   An enum constant representing the int 32 option. </summary>
        Int32,
        /// <summary>   An enum constant representing the int 64 option. </summary>
        Int64,
        /// <summary>   An enum constant representing the float option. </summary>
        Float,
        /// <summary>   An enum constant representing the double option. </summary>
        Double,
        /// <summary>   An enum constant representing the string option. </summary>
        String
    }

    /// <summary>   modes to access the tags. </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum LinkType
    {
        /// <summary>   An enum constant representing the input option. </summary>
        Input,
        /// <summary>   An enum constant representing the input output option. </summary>
        InputOutput,
        /// <summary>   An enum constant representing the exception output option. </summary>
        ExceptionOutput,
        /// <summary>   An enum constant representing the unconditional output option. </summary>
        UnconditionalOutput
    }
    
    /// <summary>   how to aggregate job. </summary>
    public enum JobAggregationType
    { 
        /// <summary>   An enum constant representing the job aggreg impossible option. </summary>
        JobAggregImpossible,
        /// <summary>   An enum constant representing the job aggreg forward option. </summary>
        JobAggregForward,
        /// <summary>   An enum constant representing the job aggreg backward option. </summary>
        JobAggregBackward,
        /// <summary>   An enum constant representing the job aggreg fits option. </summary>
        JobAggregFits
    }

    /// <summary>   Values that represent CommJobState. </summary>
    public enum CommJobState
    {
        /// <summary>   An enum constant representing the polling in error option. </summary>
        PollingInError,
        /// <summary>   An enum constant representing the polling not in use option. </summary>
        PollingNotInUse,
        /// <summary>   An enum constant representing the polling in use option. </summary>
        PollingInUse,
        /// <summary>   An enum constant representing the polling now option. </summary>
        PollingNow
    }

    /// <summary>   Values that represent DriverSynchroOperation. </summary>
    public enum DriverSynchroOperation : int
    {
        /// <summary>   An enum constant representing the read synchro option. </summary>
        ReadSynchro,
        /// <summary>   An enum constant representing the write synchro option. </summary>
        WriteSynchro
    }
    /// <summary>   Values that represent DriverMethods. </summary>
    public enum DriverMethods : int
    {
        
    }
    /// <summary>   Common error codes for serial drivers. </summary>
    public enum DriverErrorCodes : int
    { 
        /// <summary>   An enum constant representing the error no error option. </summary>
        ErrorNoError,
        /// <summary>   An enum constant representing the error time out option. </summary>
        ErrorTimeOut,
        /// <summary>   An enum constant representing the error frame error option. </summary>
        ErrorFrameError,
        /// <summary>   An enum constant representing the error overrun error option. </summary>
        ErrorOverrunError,
        /// <summary>   An enum constant representing the error receive over error option. </summary>
        ErrorRXOverError,
        /// <summary>   An enum constant representing the error receive parity error option. </summary>
        ErrorRXParityError,
        /// <summary>   An enum constant representing the error transmit full error option. </summary>
        ErrorTXFullError,
        /// <summary>   An enum constant representing the error parsing answer option. </summary>
        ErrorParsingAnswer
    }

    /// <summary>   Bits of the state/command variable of a station. </summary>
    public enum StationVariableBits : ushort
    {
        StationErrorState,
        StationActiveCommand
    }
    /// <summary>   Bits of the state/command variable of a channel. </summary>
    public enum ChannelVariableBits : ushort
    {
        ChannelUnconnected,
        PrimaryHostErrorState,
        BackupHostErrorState,
        ConnectedHost,
        SwitchServer
    }
    /// <summary>   Bits of the state/command variable of a driver. </summary>
    public enum DriverVariableBits : ushort
    {
        ChangeTagsSettings,
        ErrorLoadTagsSettings,
    }
    #endregion

}
