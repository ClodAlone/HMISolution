 /**
 * <interfacename>CmpBlkDrv</interfacename>
 * <description> 
 *	Block driver interface. This interface could be implemented by different components.
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPBLKDRVITF_H_
#define _CMPBLKDRVITF_H_

#include "CmpStd.h"

 

 





/* Settings for CmpBlkDrvCom */
/* ------------------------- */

/**
 * <category>Static defines</category>
 * <description>
 * #define BLKDVRCOM_EXCLUDE_PROTOCOL_EXTENSIONS
 * The option BLKDVRCOM_EXCLUDE_PROTOCOL_EXTENSIONS can be set to exclude the code for the protocol extensions
 * AutoAddressing and HalfDuplexAutoNegotiate to reduce the amount of needed code and data memory.
 * If this option is set, both extensions can not be used anymore and the corresponding settings are ignored.
 * </description>
 */

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Settings prefix for CmpBlkDrvCom. The number after the prefix describes the configuration instance. 
 *  Example: Configuration for two Instances, one uses ComPort 1, the other ComPort 8.
 *  Com.0.Port=1
 *  Com.1.Port=8
 * </description>
 */
#define BLKDRVCOMKEY_STRING_PREFIX								"Com"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Name of the blockdriver instance.
 *  Example: Com.0.Name=MyCom
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMKEY_STRING_INTERFACE_NAME						"Name"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Serial Port Number.
 *  Example: Com.0.Port=1
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMKEY_INT_COM_PORT								"Port"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Baudrate of serial port.
 *  Example: Com.0.Baudrate=57600
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMKEY_INT_BAUDRATE								"Baudrate"
#define BLKDRVCOMVALUE_INT_BAUDRATE_DEFAULT						SYS_BR_57600

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Parity of serial port.	
 *	SYS_NOPARITY = 0,
 *	SYS_ODDPARITY = 1,
 *	SYS_EVENPARITY = 2
 *  Example: Com.0.Parity=0
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMKEY_INT_PARITY									"Parity"
#define BLKDRVCOMVALUE_INT_PARITY_DEFAULT						SYS_NOPARITY

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Baudrate of serial port.
 *	SYS_ONESTOPBIT = 1,
 *	SYS_ONE5STOPBITS = 2,
 *	SYS_TWOSTOPBITS = 3
 *  Example: Com.0.Stopbit=1
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMKEY_INT_STOPBIT								"Stopbit"
#define BLKDRVCOMVALUE_INT_STOPBIT_DEFAULT						SYS_ONESTOPBIT

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Logical local address of the serial interface. Default value is the number of the COM-Port.
 *  Used for AutoAdressing and DuplexAutoNegotiate features. 
 *  Example: Com.0.LocalAddress=170
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMKEY_INT_LOCAL_ADDRESS							"LocalAddress"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Enables(1) or disables(0) or auto detects(2) the auto addressing feature of the BlkDrvCom. 
 *  The value 0 should not be used anymore for new devices, because this restricts the routing capabilities 
 *  of the device. 
 *  The recommended value 2 (default), let the driver detect the setting of the peer automatically. If both
 *  peers use the value 2, they will both enable the auto addressing feature. Thus in general there is no
 *  need to change this setting.
 *  Example: Com.0.EnableAutoAddressing=2
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMKEY_INT_AUTO_ADDRESS							"EnableAutoAddressing"
#ifndef BLKDRVCOMVALUE_INT_AUTO_ADDRESS_DEFAULT	
	#define BLKDRVCOMVALUE_INT_AUTO_ADDRESS_DEFAULT				2
#endif
/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Enables(1) or disables(0) or auto detects(2) the half-duplex feature of the BlkDrvCom.
 *  If enabled, the block driver manages the communication in a strict half-duplex way.
 *  The role of the master and also the local addresses are automatically negotiated 
 *  between the two peer devices. Because of the half-duplex protocol, the effective data
 *  throughput is somewhat less than without this feature. So this feature is recommended
 *  for media sharing one physical channel for both directions (e. g. RS485) or if a device
 *  has not the capability of full-duplex communication. Only in this cases the parameter
 *  value shold be set to 1.
 *  The recommended value 2 (default), let the driver detect the setting of the peer automatically.
 *  If both peers use the value 2, they will both enable this feature. 
 *  If EnableAutoAddressing and HalfDuplexAutoNegotiate are set to 2 (auto detection), the
 *  two peers will use auto addressing.
 *  Supports only point-to-point communication.
 *  Example: Com.0.HalfDuplexAutoNegotiate=2
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMKEY_INT_HALF_DUPLEX							"HalfDuplexAutoNegotiate"
#ifndef BLKDRVCOMVALUE_INT_HALF_DUPLEX_DEFAULT	
	#define BLKDRVCOMVALUE_INT_HALF_DUPLEX_DEFAULT				2
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  Currently only used to set the RTS_CONTROL_TOGGLE handshake on MS Windows based systems.
 *  This is needed for some externals RS232/RS485 adapters to switch the data direction on 
 *  the (half-duplex) line. 
 *  Example: Com.0.EnableRtsToggleHandshake=0
 *  Only for BlkDrvCom. 
 * </description>
 */
#define BLKDRVCOMKEY_INT_RTS_TOGGLE								"EnableRtsToggleHandshake"
#define BLKDRVCOMVALUE_INT_RTS_TOGGLE_DEFAULT					0
		
/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  Configures the timeout for the serial port in ms. This value is passed as parameter to the
 *  SysComOpen call and adjusts the operating system's timeout configuration of the COM port.
 *  Values different from the default should only be used, if these are really needed, because
 *  they may slow down the communication.
 *  One typical use case are some USB/RS232 adapters in combination with a virtual COM port 
 *  driver on Microsoft Windows operating systems. There are a few versions of the
 *  usbser.sys driver (or vendor specific variants), which seem not be long time stable
 *  when communication runs with a medium or high load. This issue can lead to several 
 *  (very) sporadic symptoms:
 *  - slow communication (gaps for a few seconds)
 *  - shutdown of the Gateway or the CODESYSControl proccess (restart is possible in most cases)
 *  - BSODs or system reboots. 
 *  If this symptoms occur in combination with a USB/RS232 adapter, please check, if there is
 *  an updated driver available. For some drivers exist also some timeout settings in the
 *  Microsoft Windows Device Manager (Control Panel), to adjust the behaviour of the 
 *  virtual COM port.
 *  Last you can try to increase this parameter to workaround the issue. Typical values are in the
 *  range of 0..100 (ms). 
 *  Example: Com.0.PortTimeout=10
 *  Only for BlkDrvCom. 
 * </description>
 */
#define BLKDRVCOMKEY_INT_PORT_TIMEOUT							"PortTimeout"
#define BLKDRVCOMVALUE_INT_PORT_TIMEOUT_DEFAULT					1

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Set the physical mode of the serial interface:
 *  0: RS232
 *  1: RS485 
 *  Only for BlkDrvCom. Currently not used, but reserved for future use.
 * </description>
 */
#define BLKDRVCOMKEY_INT_PHYSICAL_MODE							"PhysicalMode"
#define BLKDRVCOMVALUE_INT_PHYSICAL_MODE_DEFAULT				0



/**
 * <category>Static defines</category>
 * <description>
 *  Maximum number of bytes that are read with one call to SysComRead. Can be set to a bigger 
 *  value on embedded systems, to minimize the number of calls to SysComRead. Example: 256.
 *  Only for BlkDrvCom.
 * </description>
 */
#ifndef BLKDRVCOM_MAX_SER_READSIZE
	#define BLKDRVCOM_MAX_SER_READSIZE 1
#endif



/* Old fashioned settings for CmpBlkDrvCom to be  */
/* Must not mixed with the one above.             */
/* Only supported for backward compatibility.     */
/* Do not use this for new implementations.       */
/* ---------------------------------------------- */

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Serial Port Number. Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMOLDKEY_INT_COM_PORT							"ComPort"
#define BLKDRVCOMOLDVALUE_INT_COM_PORT_DEFAULT					1

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Baudrate of serial port. Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMOLDKEY_INT_BAUDRATE							"Baudrate"
#define BLKDRVCOMOLDVALUE_INT_BAUDRATE_DEFAULT					BLKDRVCOMVALUE_INT_BAUDRATE_DEFAULT

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Name of the serial interface. Default value is "COM[Portnumber]". Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMOLDKEY_STRING_INTERFACE_NAME					"Name"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Logical local address of the serial interface. Default value is the number of the COM-Port. 
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMOLDKEY_INT_LOCAL_ADDRESS						"LocalAddress"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Enable/Disable the auto addressing feature auf the BlkDrvCom. See new setting
 *  format above for details.
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMOLDKEY_INT_AUTO_ADDRESS						"EnableAutoAddressing"
#define BLKDRVCOMOLDVALUE_INT_AUTO_ADDRESS_DEFAULT				BLKDRVCOMVALUE_INT_AUTO_ADDRESS_DEFAULT

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Enable/Disable the ahalf-duplex feature auf the BlkDrvCom. See new setting
 *  format above for details.
 *  Only for BlkDrvCom.
 * </description>
 */
#define BLKDRVCOMOLDKEY_INT_HALF_DUPLEX							"HalfDuplexAutoNegotiate"
#define BLKDRVCOMOLDVALUE_INT_HALF_DUPLEX_DEFAULT				BLKDRVCOMVALUE_INT_HALF_DUPLEX_DEFAULT



/* Settings for CmpBlkDrvShm */
/* ------------------------- */

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	The basename for the used sharedmemory. Only for BlkDrvShm.
 * </description>
 */
#define BLKDRVSHMKEY_STRING_BASENAME							"BaseName"
#define BLKDRVSHMKEY_STRING_BASENAME_DEFAULT					"BlkDrvShm"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Optional setting to force the local address the blockdriver should use. Only for BlkDrvShm.
 * </description>
 */
#define BLKDRVSHMKEY_INT_FORCEDADDRESS						"ForcedAddress"
#define BLKDRVSHMKEY_INT_FORCEDADDRESS_DEFAULT				INT32_C(-1)




/* Settings for CmpBlkDrvUdp */
/* ------------------------- */

/**
 * <category>Static defines</category>
 * <description>
 *  Max number of devices (interfaces), which are used by the CmpBlkDrvUdp. 
 *  New setting BLKDRVUDPKEY_INT_MAX_INTERFACES_DEFAULT should be used instead. 
 * </description>
 */
#ifndef MAX_UDP_DEVICES
	#define MAX_UDP_DEVICES										8
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Defines the maximal number of interfaces (ip addresses), which are used by the CmpBlkDrvUdp. This is also the limitation
 *  for the number of interface configurations.
 *  There are two typical use cases for this setting:
 *	1. Systems on which more than 8 network adapters should be used by this block driver.
 *  Example:
 *	MaxInterfaces=10
 *  2. Define a white list for ip addresses. All other interfaces are ignored by the CmpBlkDrvUdp.
 *	Example:
 *	MaxInterfaces=2
 *  itf.0.ipaddress=192.168.100.1 
 *  itf.1.ipaddress=192.168.200.1
 * </description>
 */
#define BLKDRVUDPKEY_INT_MAX_INTERFACES							"MaxInterfaces"
#ifndef BLKDRVUDPKEY_INT_MAX_INTERFACES_DEFAULT
	#define BLKDRVUDPKEY_INT_MAX_INTERFACES_DEFAULT				MAX_UDP_DEVICES
#endif

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Settings prefix for blockdriver Udp. Has to be used for all settings, which refer only to one interface. 
 *  Example: itf.0.ipaddress=192.168.100.1
 * </description>
 */
#define BLKDRVUDPKEY_STRING_PREFIX								"itf"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	IP address of the interface. Also the network base address can be specified. Only for BlkDrvUdp.
 *  Example: itf.0.ipaddress=192.168.100.1
 * </description>
 */
#define BLKDRVUDPKEY_STRING_IPADDRESS							"ipaddress"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Name of the ethernet UDP interface. Default value is "ether x", whereas x is count from 0. Only for BlkDrvUdp.
 *  Example: itf.0.name=MySpecialInterface
 * </description>
 */
#define BLKDRVUDPKEY_STRING_INTERFACE_NAME						"name"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Network mask of the interface. Setting is only needed, if the networkmask can not be retrieved from the
 *	operating system. Only for BlkDrvUdp.
 *  Example: itf.0.networkmask=255.255.255.0
 * </description>
 */
#define BLKDRVUDPKEY_STRING_NETWORKMASK							"networkmask"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *  If set to a valid IP address, point to point (PPP) communication is enabled for this interface.
 *  That mean, that all broadcasts on this interface are sent to this remote IP address. 
 *  This is useful for interfaces, which does not support broadcast (e. g. PPP over USB). Only for BlkDrvUdp.
 *  Example: itf.0.PPPRemoteAddress=90.0.0.1
 * </description>
 */
#define BLKDRVUDPKEY_STRING_PPPREMOTEADDRESS					"PPPRemoteAddress"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Index of the used UDP port. This value is added to the CmpBlkDrvUdp base port number (1740). If the specified
 *  port could not be opened, no other free port is searched/used and therefore no communication is possible on this
 *  network interface. The allowed range of the setting is 0..3. Only for BlkDrvUdp.
 *  Example: itf.0.portindex=3 
 *  With this setting the port 1743 is used.
 * </description>
 */
#define BLKDRVUDPKEY_INT_PORTINDEX								"portindex"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	If no portindex (see above) is set for a network interface, then the CmpBlkDrvUdp tries to find a free port in the
 *  range 1740..1743 and uses the first free one. The search for a free port starts at 1740 plus the value of this setting
 *  and wraps around, so at any case all four ports will be checked until a free one is found. Usually the setting is used
 *  for devices which run several instances of the CODESYS runtime system (also Gateway, PLCHandler, OPC Server) to get 
 *  independently of the starting sequence always the same port offset for each instance. This setting is evaulated globally
 *  for all adapters, thus there is no need to know the network adapter configuation of the device.
 *  The allowed range of the setting is 0..3. Only for BlkDrvUdp.
 *  Example: DefaultPortIndex=3
 * </description>
 */
#define BLKDRVUDPKEY_INT_DEFAULT_PORTINDEX						"DefaultPortIndex"
#ifndef BLKDRVUDPKEY_INT_DEFAULT_PORTINDEX_DEFAULT
	#define BLKDRVUDPKEY_INT_DEFAULT_PORTINDEX_DEFAULT			0
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Sorts at startup the network adapters to register them in a defined sequenze. This setting can be used
 *  to get more stable addresses over several reboots, if the platform do not have a specific sorting order
 *  for network adapters. Typcially this may used on Microsoft Windows systems, to be independent from the
 *  order of the network adapters, which is reported to change sometimes on Windows updates.
 *  Possible values: 
 *  0: No sort. Keep the system order of the network adapters.
 *  1: sort adapters by ip address. 
 * </description>
 */
#define BLKDRVUDPKEY_INT_SORT_ADAPTERS_AT_STARTUP				"SortAdaptersAtStartup"
#ifndef BLKDRVUDPKEY_INT_SORT_ADAPTERS_AT_STARTUP_DEFAULT
	#define BLKDRVUDPKEY_INT_SORT_ADAPTERS_AT_STARTUP_DEFAULT	0
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Sets the network adapter scan interval in seconds. The network adapter scan checks, if the ip address or
 *  subnet mask of the network adapters has changed or if adapters has been added or removed. If a change is 
 *  detected, the CmpBlkDrvUdp updates its configuration and registers/unregisters the affected instances at
 *  the CmpRouter. 0 means the cyclic call is disabled.
 * </description>
 */
#define BLKDRVUDPKEY_INT_UDPSYSSOCKETUPDATE_CYCLETIME			"UdpSysSocketUpdateCycleTime"
#define BLKDRVUDPKEY_INT_UDPSYSSOCKETUPDATE_CYCLETIME_DEFAULT	5

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *  Global setting for all ethernet interfaces. Is only evaluated, if the subnetmask is set to 0.0.0.0 by
 *  using the setting above. Only for BlkDrvUdp.
 *  By default the broadcasts are sent to the local broadcast address (e. g. to 192.168.1.255), even if the
 *  subnetmask is set to 0.0.0.0 with the obove setting. Thus the setting of the subnet mask affects only
 *  the number of bits, which are taken from the IP address to create the CoDeSys address. With the option
 *  SendGlobalBroadcast=1, the CmpBlkDrvUdp sends global broadcasts (to 255.255.255.255) instead of 
 *  local broadcasts. If the network infrastructure gateway is configured to forward global broadcasts 
 *  also to other networks, then the CoDeSys network scan can also find devices outside the local ip-network. 
 *  In genreal, global broadcasts should be avoided, because at least on multi homed systems (more than one 
 *  IP address) the exact behaviour depends on the operating system. So for example on older Windows versions
 *  (including Windows XP SP3!) this leads typically to "spoofed" packets with a wrong source IP address.
 *  See also Microsoft KB175396.
 *  Example: SendGlobalBroadcast=1
 * </description>
 */
#define BLKDRVUDPKEY_INT_SENDGLOBALBROADCAST					"SendGlobalBroadcast"
#ifndef BLKDRVUDPKEY_INT_SENDGLOBALBROADCAST_DEFAULT
	#define BLKDRVUDPKEY_INT_SENDGLOBALBROADCAST_DEFAULT		0
#endif


/**
 * <category>Static defines</category>
 * <description> Socket bind option for the CmpBlkDrvUdp. </description>
 */
#define UDP_BIND_ADDRESS		0
#define UDP_BIND_INADDR_ANY		1
#define UDP_BIND_TWO_SOCKETS	2

/**
 * <category>Static defines</category>
 * <description>
 * Oldfashioned #defines for BLKDRVUDPKEY_INT_BINDOPTION. For a detailed description see there.
 * #define SOCK_BIND_INADDR_ANY
 * is replaced by
 * #define BLKDRVUDPKEY_INT_BINDOPTION_DEFAULT UDP_BIND_INADDR_ANY
 * and
 * #define SOCK_BIND_TWO_SOCKETS
 * is replaced by
 * #define BLKDRVUDPKEY_INT_BINDOPTION_DEFAULT UDP_BIND_TWO_SOCKETS
 *	
 * The #defines SOCK_BIND_INADDR_ANY and SOCK_BIND_TWO_SOCKETS must not be used together!
 * </description>
 */

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *  Global setting for all ethernet interfaces. Only for BlkDrvUdp.
 *  There are 3 different bind behaviours for the CmpBlkDrvUdp available, which must NOT be combined which each other:
 * 
 *  1. BindOption = UDP_BIND_ADDRESS
 *  One socket is used per interface. This socket is bound to the IP-Address of this interface and we assume,
 *  that we also receive local broadcasts on this socket. This is used for Windows runtime systems.
 *
 *  2. BindOption = UDP_BIND_INADDR_ANY
 *  One socket is used and bound to the IP-Address SOCKET_INADDR_ANY, so we receive also local broadcasts on 
 *  this socket. This works fine, if there is only one interface available. If a device supports more than one 
 *  interface, there is the chance, that the socket layer mix up the incoming frames. To prevent this, see
 *  also the settings for the sort options.
 *  In general the option UDP_BIND_INADDR_ANY is fine for small systems. 
 *
 *  3. BindOption = UDP_BIND_TWO_SOCKETS
 *  Two sockets are used per interface for receiving local broadcast and explicit to the device address sent 
 *  UDP frames. To reach this, one socket is bound to the IP-Address and the other one to the local broadcast 
 *  (multicast) address. Can be used with BSD-sockets (Linux, VxWorks, etc.) 
 * </description>
 */
#define BLKDRVUDPKEY_INT_BINDOPTION							"BindOption"
#ifndef BLKDRVUDPKEY_INT_BINDOPTION_DEFAULT
	#if defined SOCK_BIND_TWO_SOCKETS
		#define BLKDRVUDPKEY_INT_BINDOPTION_DEFAULT			UDP_BIND_TWO_SOCKETS
	#elif defined SOCK_BIND_INADDR_ANY
		#define BLKDRVUDPKEY_INT_BINDOPTION_DEFAULT			UDP_BIND_INADDR_ANY
	#else
		#define BLKDRVUDPKEY_INT_BINDOPTION_DEFAULT			UDP_BIND_ADDRESS
	#endif		
#endif


/**
 * <category>Static defines</category>
 * <description> 
    Options for the CmpBlkDrvUdp to sort packages after receiving or before sending. Sorting for incoming and 
	outgoing packages can be used seperately or in combination.
 * </description>
 */
#define UDP_PACKET_SORT_NONE		0
#define UDP_PACKET_SORT_INCOMING	1
#define UDP_PACKET_SORT_OUTGOING	2

/**
 * <category>Static defines</category>
 * <description>
 * Oldfashioned #defines for BLKDRVUDPKEY_INT_PACKET_SORT_OPTION. For a detailed description see there.
 * #define SOCK_SORT_INCOMING_PACKETS
 * is replaced by
 * #define BLKDRVUDPKEY_INT_PACKET_SORT_OPTION_DEFAULT UDP_PACKET_SORT_INCOMING
 * and
 * #define SOCK_SORT_OUTGOING_PACKETS
 * is replaced by
 * #define BLKDRVUDPKEY_INT_PACKET_SORT_OPTION_DEFAULT UDP_PACKET_SORT_OUTGOING
 * Also the combination of both option is allowed.
 * </description>
 */
  
/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *  Global setting for all ethernet interfaces. Only for BlkDrvUdp.
 *  For some special uses cases, there is the need to assign packages after receiving or before sending to another
 *  ethernet adapter:
 * 
 *  1. SortOption = UDP_PACKET_SORT_NONE
 *  Nothing special is done (default).
 *
 *  2. SortOption = UDP_PACKET_SORT_INCOMING
 * The option UDP_PACKET_SORT_INCOMING can be set in combination with the bind option SOCK_BIND_INADDR_ANY 
 * (see above) to allow also small systems to use more than one ethernet interfaces. By activating this, the
 * CmpBlkDrvUdp tries to assign all incoming packages to the adapter from which it was received. So this can
 * be a workaround, if the socket layer mixes up the incomming packets.
 *
 *  3. SortOption = UDP_PACKET_SORT_OUTGOING
 * Special option for system environments, in which the subnet mask in the settings of the CmpBlkDrvUdp is set 
 * to 0.0.0.0 instead of the one used in the configuration of the network adapter. If this is done, the
 * blockdriver address contains the complete ip-address (e.g. 0000.C0A8.010A instead of 000A) of a node.
 * By default the broadcasts are still sent to the local broadcast address (e. g. to 192.168.1.255). By setting 
 * the option SendGlobalBroadcast=1, the CmpBlkDrvUdp sends global broadcasts (to 255.255.255.255) instead of 
 * the local broadcasts.
 * Doing this has one big drawback, if there is more than one network adapter in the CoDeSys runtime system or 
 * gateway available: The CmpRouter has no chance to check, over which network adapter another node can be reached
 * directly, because of the subnet mask 0.0.0.0 any ip-address seems to be in the local network of each network 
 * adapter. This can lead to packages, which are sent using a "wrong" network adapter.
 * With the static define UDP_PACKET_SORT_OUTGOING, the CmpBlkDrvUdp checks, if the destination ip-address is
 * reachable directly using the network adapter, to which the current socket was bound to, by checking the destination
 * ip-address against the ip-address and the real subnet mask of this adapter. If the destination address is not inside
 * this local network, then the CmpBlkDrvUdp tries to find another socket, which was bound to another adapter,
 * which can address the destination ip-address directly. If there was found such a socket, the sender address of
 * the block is patched and the block is sent directly to the destination using this socket. 
 * ATTENTION: Please use this option (and also the subnet mask 0.0.0.0) very carefully! 
 * All in all this setting overcomes only a few handicaps of setting the subnet mask to 0.0.0.0. You will still
 * have at least the following restrictions: 
 * A. All nodes in the network must have the same (configured) subnet mask. All other nodes are invisible for the
 *    3S protocol. Especially you have the need to tell the setting for the subnet mask also clients like the 
 *    OPC-Server, if you use the ARTI3 interface to connect to a runtime system with the mask 0.0.0.0.
 * B. The CmpBlkDrvUdp is designed for local communication inside the ip-subnet. Even with the option 
 *    SendGlobalBroadcast=1 typically global broadcasts are not forwarded by the network infrastructure gateways 
 *    or routers, thus you will not see nodes outside of your local ip-network in the CoDeSys network scan. 
 *    Furthermore the communication posibilities depend on the currently active configuration of these routing
 *    devices. Please check this for your exact devices very carefully!
 * C. With the option UDP_PACKET_SORT_OUTGOING in combination with the subnet mask 0.0.0.0, the CmpBlkDrvUdp tries 
 *    to reimplement the behavior of the underlaying Tcp/Ip stack. If this does not fit, the routing is still not
 *    possible. 
 * D. The algorithm can only sort blocks on the sender side. If the block is not sent directly to the receiver, but
 *    over further CoDeSys runtime systems or gateways to route the block using the CoDeSys routing mechanism, all 
 *    systems in the middle will behave in the same way as without the option UDP_PACKET_SORT_OUTGOING.     
 * E. Also with the option UDP_PACKET_SORT_OUTGOING, the communication components have no chance to know, over 
 *    which network adapter a block has to be sent, when the destination ip-address is outside the local network 
 *    of all network adapters. Because this routing is done by the network stack on the local and also remote systems
 *    (infrastructure gateways or routers) and there is no common way to get the exact route before sending a package.
 * The option UDP_PACKET_SORT_OUTGOING is activated by default in the GatewayService and also in the PLCHandler/
 * OPC-Server, because this has only an effect, if you set the subnet mask of at least two network adapters to 0.0.0.0. 
 * On PLC-side there is usually no need to activate this option.
 *
 *  4. SortOption = (UDP_PACKET_SORT_INCOMING | UDP_PACKET_SORT_OUTGOING)
 *  Packet soorting is activated in both directions (see above).
 * </description>
 */
#define BLKDRVUDPKEY_INT_PACKET_SORT_OPTION								"SortOption"
#ifndef BLKDRVUDPKEY_INT_PACKET_SORT_OPTION_DEFAULT
	#if defined SOCK_SORT_INCOMING_PACKETS
		#if defined SOCK_SORT_OUTGOING_PACKETS
			#define BLKDRVUDPKEY_INT_PACKET_SORT_OPTION_DEFAULT			(UDP_PACKET_SORT_INCOMING | UDP_PACKET_SORT_OUTGOING)
		#else
			#define BLKDRVUDPKEY_INT_PACKET_SORT_OPTION_DEFAULT			UDP_PACKET_SORT_INCOMING
		#endif		
	#else	
		#if defined SOCK_SORT_OUTGOING_PACKETS
			#define BLKDRVUDPKEY_INT_PACKET_SORT_OPTION_DEFAULT			UDP_PACKET_SORT_OUTGOING
		#else
			#define BLKDRVUDPKEY_INT_PACKET_SORT_OPTION_DEFAULT			UDP_PACKET_SORT_NONE
		#endif
	#endif		
#endif


/* Settings for CmpBlkDrvTcp */
/* ------------------------- */

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Enables/disables (1/0) the client functionality of the CmpBlkDrvTcp. If enabled, the CmpBlkDrvTcp is allowed to 
 *  connect to other systems. Only for BlkDrvTcp.
 * </description>
 */
#define BLKDRVTCPKEY_INT_ENABLE_CLIENT							"EnableClient"
#ifndef BLKDRVTCPKEY_INT_ENABLE_CLIENT_DEFAULT
	#define BLKDRVTCPKEY_INT_ENABLE_CLIENT_DEFAULT				1
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Enables/disables (1/0) the server functionality of the CmpBlkDrvTcp. If enabled, the CmpBlkDrvTcp opens the
 *  the specified port and listens for incoming connection requests. Only for BlkDrvTcp.
 * </description>
 */
#define BLKDRVTCPKEY_INT_ENABLE_SERVER							"EnableServer"
#ifndef BLKDRVTCPKEY_INT_ENABLE_SERVER_DEFAULT
	#define BLKDRVTCPKEY_INT_ENABLE_SERVER_DEFAULT				1
#endif

 /**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	By default the server of the CmpBlkDrvTcp retrieves the number of channels from the CmpChannelServer and allows
 *  (number of channels + 1) incomming connections. This value can be adapted by the setting MaxServerConnections.
 *  Only for BlkDrvTcp.
 * </description>
 */
#define BLKDRVTCPKEY_INT_MAX_SERVER_CONNECTIONS					"MaxServerConnections"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	If activated (1), only localhost communication is allowed. The same can be achieved by the settings
 *	LocalAddress=127.0.0.1 and PeerAddress=127.0.0.1. In opposite to these, LocalAccessOnly is also
 *  setable by a compiler define. Only for BlkDrvTcp.
 * </description>
 */
#define BLKDRVTCPKEY_INT_LOCAL_ACCESS_ONLY						"LocalAccessOnly"
#ifndef BLKDRVTCPKEY_INT_LOCAL_ACCESS_ONLY_DEFAULT
	#define BLKDRVTCPKEY_INT_LOCAL_ACCESS_ONLY_DEFAULT			0
#endif

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Ip address to which the sockets will be bound. If set to a valid ip address, only connection requests received on the
 *  related adapter are accepted by the server. Also the client acts in a similar way and sends the connect request
 *  only on this adapter. Only for BlkDrvTcp.
 * </description>
 */
#define BLKDRVTCPKEY_STRING_LOCALADDRESS						"LocalAddress"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	If set to a valid ip address, the CmpBlkDrvTcp only accepts connection requests from this or connects only to this peer.
 *  Only for BlkDrvTcp.
 * </description>
 */
#define BLKDRVTCPKEY_STRING_PEERADDRESS							"PeerAddress"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Tcp port number on which the block driver Tcp listens for incoming connection requests. Only for BlkDrvTcp.
 * </description>
 */
#define BLKDRVTCPKEY_INT_LISTENPORT								"ListenPort"
#ifndef BLKDRVTCPKEY_INT_LISTENPORT_DEFAULT
	#define BLKDRVTCPKEY_INT_LISTENPORT_DEFAULT					INT32_C(11740)
#endif

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Ip address, which is used for calculating the local CODESYS address. If neither this setting nor the setting LocalAddress
 *  is set to a valid ip address, the ip address of the first found ethernet adapter is used.
 *  In opposite to LocalAddress/PeerAddress, this setting has no influence to the communication. It is only used internally
 *  and for display/logger purposes. 
 *  Only for BlkDrvTcp.
 * </description>
 */
#define BLKDRVTCPKEY_STRING_IPADDRESS							"ipaddress"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	The name for the block driver instance. Only for BlkDrvTcp.
 * </description>
 */
#define BLKDRVTCPKEY_STRING_NAME								"Name"
#define BLKDRVTCPKEY_STRING_NAME_DEFAULT						"BlkDrvTcp"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  Inactivity timeout. If within this time (in seconds) is not a block received or sent,
 *  the TcpConnection will be closed. Only for BlkDrvTcp.
 * </description>
 */
#define BLKDRVTCPKEY_INT_INACTIVITY_TO							"InactivityTimeout"
#define BLKDRVTCPKEY_INT_INACTIVITY_TO_DEFAULT					60


/* Settings for CmpBlkDrvUsb */
/* ------------------------- */

/**
 * <category>Static defines</category>
 * <description>
 * #define BLKDVRUSB_EXCLUDE_PROTOCOL_EXTENSIONS
 * The option BLKDVRUSB_EXCLUDE_PROTOCOL_EXTENSIONS can be set to exclude the code for the protocol extensions
 * AutoAddressing and HalfDuplexAutoNegotiate to reduce the amount of needed code and data memory.
 * If this option is set, both extensions can not be used anymore and the corresponding settings are ignored.
 * </description>
 */

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 * </description>
 */
#define BLKDRVUSBKEY_STRING_DRIVERNAME							"Name"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 * </description>
 */
#define BLKDRVUSBKEY_STRING_DEVICENAME							"Name"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 * </description>
 */
#define BLKDRVUSBKEY_STRING_DEVICEPREFIX						"Dev"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * </description>
 */
#define BLKDRVUSBKEY_INT_VENDORID								"VendorId"
#define BLKDRVUSBKEY_INT_VENDORID_DEF							0x16DE

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * </description>
 */
#define BLKDRVUSBKEY_INT_DEVICEID								"DeviceId"
#define BLKDRVUSBKEY_INT_DEVICEID_DEF							0x0030

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * </description>
 */
#define BLKDRVUSBKEY_INT_INTERFACE								"Interface"
#define BLKDRVUSBKEY_INT_INTERFACE_DEF							0

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * </description>
 */
#define BLKDRVUSBKEY_INT_CONFIG									"Config"
#define BLKDRVUSBKEY_INT_CONFIG_DEF								1

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * </description>
 */
#define BLKDRVUSBKEY_INT_ENDPOINT_IN							"EndpointIn"
#define BLKDRVUSBKEY_INT_ENDPOINT_IN_DEF						0x81

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * </description>
 */
#define BLKDRVUSBKEY_INT_ENDPOINT_OUT							"EndpointOut"
#define BLKDRVUSBKEY_INT_ENDPOINT_OUT_DEF						0x03

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Logical local address of the USB serial interface.  
 *  Only for BlkDrvUSB.
 * </description>
 */
#define BLKDRVUSBKEY_INT_LOCAL_ADDRESS							"LocalAddress"
#define BLKDRVUSBKEY_INT_LOCAL_ADDRESS_DEF						254

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Enables(1) or disables(0) or auto detects(2) the auto addressing feature of the BlkDrvUsb.
 *  The value 0 should not be used anymore for new devices, because this restricts the routing capabilities
 *  of the device.
 *  The recommended value 2 (default), let the driver detect the setting of the peer automatically. If both
 *  peers use the value 2, they will both enable the auto addressing feature. Thus in general there is no
 *  need to change this setting.
 *  Only for BlkDrvUSB.
 * </description>
 */
#define BLKDRVUSBKEY_INT_AUTO_ADDRESS							"EnableAutoAddressing"
#ifndef BLKDRVUSBKEY_INT_AUTO_ADDRESS_DEF
	#define BLKDRVUSBKEY_INT_AUTO_ADDRESS_DEF					BLKDRVCOMVALUE_INT_AUTO_ADDRESS_DEFAULT
#endif

 /**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Enables(1) or disables(0) or auto detects(2) the half-duplex feature of the BlkDrvUsb.
 *  If enabled, the block driver manages the communication in a strict half-duplex way.
 *  The role of the master and also the local addresses are automatically negotiated
 *  between the two peer devices. Because of the half-duplex protocol, the effective data
 *  throughput is somewhat less than without this feature. So this feature is recommended
 *  for media sharing one physical channel for both directions (e. g. RS485) or if a device
 *  has not the capability of full-duplex communication. Only in this cases the parameter
 *  value shold be set to 1.
 *  The recommended value 2 (default), let the driver detect the setting of the peer automatically.
 *  If both peers use the value 2, they will both enable this feature.
 *  If EnableAutoAddressing and HalfDuplexAutoNegotiate are set to 2 (auto detection), the
 *  two peers will use auto addressing.
 *  Supports only point-to-point communication.
 *  Only for BlkDrvUsb.
 * </description>
 */
#define BLKDRVUSBKEY_INT_HALF_DUPLEX							"HalfDuplexAutoNegotiate"
#ifndef BLKDRVUSBKEY_INT_HALF_DUPLEX_DEF
	#define BLKDRVUSBKEY_INT_HALF_DUPLEX_DEF					BLKDRVCOMVALUE_INT_HALF_DUPLEX_DEFAULT
#endif




typedef struct
{
	IBase_C *pBase;
} ICmpBlkDrv_C;

#ifdef CPLUSPLUS
class ICmpBlkDrv : public IBase
{
	public:
};
	#ifndef ITF_CmpBlkDrv
		#define ITF_CmpBlkDrv static ICmpBlkDrv *pICmpBlkDrv = NULL;
	#endif
	#define EXTITF_CmpBlkDrv
#else	/*CPLUSPLUS*/
	typedef ICmpBlkDrv_C		ICmpBlkDrv;
	#ifndef ITF_CmpBlkDrv
		#define ITF_CmpBlkDrv
	#endif
	#define EXTITF_CmpBlkDrv
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPBLKDRVITF_H_*/
