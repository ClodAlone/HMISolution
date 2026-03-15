//--------------------------------------------------------------------------
// <copyright file="RasDeviceType.cs" company="Jeff Winn">
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      GNU Library General Public License (LGPL) v2.1 which can be found
//      in the License.rtf at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace DotRas
{
    using System;

    /// <summary>
    /// Defines the remote access service (RAS) device types.
    /// </summary>
    public static class RasDeviceType
    {
        /// <summary>
        /// A modem accessed through a COM port.
        /// </summary>
        public const string Modem = "modem";

        /// <summary>
        /// An ISDN card with a corresponding NDISWAN driver installed.
        /// </summary>
        public const string Isdn = "isdn";

        /// <summary>
        /// An X.25 card with a corresponding NDISWAN driver installed.
        /// </summary>
        public const string X25 = "x25";

        /// <summary>
        /// A virtual private network connection.
        /// </summary>
        public const string Vpn = "vpn";

        /// <summary>
        /// A packet assembler/disassembler.
        /// </summary>
        public const string Pad = "pad";

        /// <summary>
        /// Generic device type.
        /// </summary>
        public const string Generic = "GENERIC";

        /// <summary>
        /// Direct serial connection through a serial port.
        /// </summary>
        public const string Serial = "SERIAL";

        /// <summary>
        /// Frame Relay.
        /// </summary>
        public const string FrameRelay = "FRAMERELAY";

        /// <summary>
        /// Asynchronous Transfer Mode (ATM).
        /// </summary>
        public const string Atm = "ATM";

        /// <summary>
        /// Sonet device type.
        /// </summary>
        public const string Sonet = "SONET";

        /// <summary>
        /// Switched 56K access.
        /// </summary>
        public const string SW56 = "SW56";

        /// <summary>
        /// An Infrared Data Association (IrDA) compliant device.
        /// </summary>
        public const string Irda = "IRDA";

        /// <summary>
        /// Direct parallel connection through a parallel port.
        /// </summary>
        public const string Parallel = "PARALLEL";

#if (WINXP || WIN2K8 || WIN7)
        /// <summary>
        /// Point-to-Point Protocol over Ethernet.
        /// </summary>
        public const string PPPoE = "PPPoE";
#endif
    }
}