////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DriverTcpExampleTag.cs
//
// summary:	Implements the driver TCP example tag class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using DriverCodeBase;
using DriverBaseInterfaces;

namespace DriverTcpExample
{

    /// <summary>   Variable of the DriverTcpExample driver. </summary>
    public sealed class DriverTcpExampleTag : Tag
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        /// <param name="byteoffset" type="uint">   The byteoffset. </param>
        /// <param name="bitoffset" type="uint">    The bitoffset. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverTcpExampleTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverTcpExampleTag(TagDefinition tag)
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
            return DriverTcpExampleDynSettings.TryParse(dynamicSettings);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return dynamic settings. </summary>
        ///
        /// <value> The dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)DriverTcpExampleDynSettings; }
        }

        #endregion

        #region Properties

        /// <summary>   The driver TCP example dynamic settings. </summary>
        readonly DriverTcpExampleDynTagSettings _DriverTcpExampleDynSettings = new DriverTcpExampleDynTagSettings();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Dynamic tag settings property. </summary>
        ///
        /// <value> The driver TCP example dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverTcpExampleDynTagSettings DriverTcpExampleDynSettings
        {
            get { return _DriverTcpExampleDynSettings; }
        }

        #endregion        

    }
}
