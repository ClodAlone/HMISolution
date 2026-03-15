////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DriverSerialExampleTag.cs
//
// summary:	Implements the driver serial example tag class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using DriverCodeBase;
using DriverBaseInterfaces;

namespace DriverSerialExample
{
    /// <summary>   DriverSerialExample's target device memory areas  . </summary>
    public enum FunctionCodes
    {
        /// <summary>   An enum constant representing the coils option. </summary>
        Coils,
        /// <summary>   An enum constant representing the multiple registers option. </summary>
        MultipleRegisters,
    }
    
    /// <summary>   Variable of the DriverSerialExample driver. </summary>
    public sealed class DriverSerialExampleTag : Tag
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        /// <param name="byteoffset" type="uint">   The byteoffset. </param>
        /// <param name="bitoffset" type="uint">    The bitoffset. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverSerialExampleTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverSerialExampleTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Verify if string "dynamicSettings" is correct to initialize a tag. </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return DriverSerialExampleDynSettings.TryParse(dynamicSettings);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return dynamic settings. </summary>
        ///
        /// <value> The dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)DriverSerialExampleDynSettings; }
        }

        #endregion
        
        #region Properties

        /// <summary>   The driver serial example dynamic settings. </summary>
        readonly DriverSerialExampleDynTagSettings _DriverSerialExampleDynSettings = new DriverSerialExampleDynTagSettings();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Dynamic tag settings property. </summary>
        ///
        /// <value> The driver serial example dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverSerialExampleDynTagSettings DriverSerialExampleDynSettings
        {
            get { return _DriverSerialExampleDynSettings; }
        }

        #endregion        

    }
}
