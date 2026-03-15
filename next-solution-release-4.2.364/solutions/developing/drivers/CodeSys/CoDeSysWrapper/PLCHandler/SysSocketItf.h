 /**
 * <interfacename>SysSocket</interfacename>
 * <description> 
 *	<p>The SysSocket interface is projected to handle access to ethernet socket layer.
 *	TCP, UDP and RAW sockets can be used.</p>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _SYSSOCKETITF_H_
#define _SYSSOCKETITF_H_

#include "CmpStd.h"

 

 




#ifndef CDECL
	#define CDECL
#endif

/**
 * <category>AddressFamily</category>
 * <description>Socket family definitions</description>
 */
#define	SOCKET_AF_UNSPEC		0			/* unspecified */
#define	SOCKET_AF_LOCAL			1			/* local to host (pipes, portals) */
#define	SOCKET_AF_UNIX			SOCKET_AF_LOCAL	/* backward compatibility */
#define	SOCKET_AF_INET			2			/* internetwork: UDP, TCP, etc. */
#define	SOCKET_AF_IMPLINK		3			/* arpanet imp addresses */
#define	SOCKET_AF_PUP			4			/* pup protocols: e.g. BSP */
#define	SOCKET_AF_CHAOS			5			/* mit CHAOS protocols */
#define	SOCKET_AF_NS			6			/* XEROX NS protocols */
#define	SOCKET_AF_ISO			7			/* ISO protocols */
#define	SOCKET_AF_OSI			SOCKET_AF_ISO
#define	SOCKET_AF_ECMA			8			/* european computer manufacturers */
#define	SOCKET_AF_DATAKIT		9			/* datakit protocols */
#define	SOCKET_AF_CCITT			10			/* CCITT protocols, X.25 etc */
#define	SOCKET_AF_SNA			11			/* IBM SNA */
#define SOCKET_AF_DECnet		12			/* DECnet */
#define SOCKET_AF_DLI			13			/* DEC Direct data link interface */
#define SOCKET_AF_LAT			14			/* LAT */
#define	SOCKET_AF_HYLINK		15			/* NSC Hyperchannel */
#define	SOCKET_AF_APPLETALK		16			/* Apple Talk */
#define	SOCKET_AF_ROUTE			17			/* Internal Routing Protocol */
#define	SOCKET_AF_LINK			18			/* Link layer interface */
#define	SOCKET_pseudo_AF_XTP	19			/* eXpress Transfer Protocol (no AF) */
#define	SOCKET_AF_COIP			20			/* connection-oriented IP, aka ST II */
#define	SOCKET_AF_CNT			21			/* Computer Network Technology */
#define SOCKET_pseudo_AF_RTIP	22			/* Help Identify RTIP packets */
#define	SOCKET_AF_IPX			23			/* Novell Internet Protocol */
#define	SOCKET_AF_SIP			24			/* Simple Internet Protocol */
#define SOCKET_pseudo_AF_PIP	25			/* Help Identify PIP packets */
#define	SOCKET_AF_MAX			26
#define	SOCKET_AF_INET_BSD		100			/* BSD-specific INET af */
#define	SOCKET_AF_INET_STREAMS	101			/* STREAMS-specific INET af */

/**
 * <category>Socket protocols</category>
 * <description>Socket protocols for SysSockGetOption()/SysSockSetOption()</description>
 */
#define SOCKET_IPPROTO_IP                0		/* IP level */
#define SOCKET_IPPROTO_ICMP              1		/* control message protocol */
#define SOCKET_IPPROTO_IGMP              2		/* group management protocol */
#define SOCKET_IPPROTO_GGP               3		/* gateway^2 (deprecated) */
#define SOCKET_IPPROTO_TCP               6		/* tcp */
#define SOCKET_IPPROTO_PUP              12		/* pup */
#define SOCKET_IPPROTO_UDP              17		/* user datagram protocol */
#define SOCKET_IPPROTO_IDP              22		/* xns idp */
#define SOCKET_IPPROTO_ND               77		/* UNOFFICIAL net disk proto */
#define SOCKET_IPPROTO_TLS             254		/* UNOFFICIAL TCP / TLS protocol */
#define SOCKET_IPPROTO_RAW             255		/* raw IP packet */
#define SOCKET_IPPROTO_MAX             256

/**
 * <category>Socket level</category>
 * <description>Level number for SysSockGetOption()/SysSockSetOption() to apply to socket itself</description>
 */
#define	SOCKET_SOL                     0xffff	/* level for socket itself in the same numbering space as IPPROTO_TCP, IPPROTO_IP, etc. */

/**
 * <category>Socket options</category>
 * <description>Socket options for SysSockGetOption()/SysSockSetOption()</description>
 */
#define	SOCKET_SO_DEBUG			0x0001		/* turn on debugging info recording */
#define	SOCKET_SO_ACCEPTCONN	0x0002		/* socket has had listen() */
#define	SOCKET_SO_REUSEADDR		0x0004		/* allow local address reuse */
#define	SOCKET_SO_KEEPALIVE		0x0008		/* keep connections alive. See category "TCP keepalve options" for extended parameters for this option. */
#define	SOCKET_SO_DONTROUTE		0x0010		/* just use interface addresses */
#define	SOCKET_SO_BROADCAST		0x0020		/* permit sending of broadcast msgs */
#define	SOCKET_SO_USELOOPBACK	0x0040		/* bypass hardware when possible */
#define	SOCKET_SO_LINGER		0x0080		/* linger on close if data present. See category "Linger on close options" for extended parameters for this option. */
#define	SOCKET_SO_OOBINLINE		0x0100		/* leave received OOB data in line */
#define	SOCKET_SO_REUSEPORT		0x0200		/* allow local address & port reuse */
#define SOCKET_SO_SNDBUF		0x1001		/* send buffer size */
#define SOCKET_SO_RCVBUF		0x1002		/* receive buffer size */
#define SOCKET_SO_SNDLOWAT		0x1003		/* send low-water mark */
#define SOCKET_SO_RCVLOWAT		0x1004		/* receive low-water mark */
#define SOCKET_SO_SNDTIMEO		0x1005		/* send timeout */
#define SOCKET_SO_RCVTIMEO		0x1006		/* receive timeout */
#define	SOCKET_SO_ERROR			0x1007		/* get error status and clear */
#define	SOCKET_SO_TYPE			0x1008		/* get socket type */
#define SOCKET_SO_PROTOTYPE		0x1009		/* get/set protocol type */
#define SOCKET_SO_HOSTNAME		0x2001		/* get/set the hostname for TLS connections. Needed for SNI support */

/**
 * <category>Socket TCP options</category>
 * <description>Socket options for SysSockGetOption()/SysSockSetOption(). Only to be used for SOCKET_IPPROTO_TCP.</description>
 */
#define	SOCKET_TCP_NODELAY		0x0001		/* don't delay send to coalesce packets */
#define	SOCKET_TCP_MAXSEG		0x0002		/* set maximum segment size */

/**
 * <category>IP multicast options</category>
 * <description>Socket options for SysSockGetOption()/SysSockSetOption(). Only to be used for SOCKET_IPPROTO_IP.</description>
 */
#define SOCKET_IP_MULTICAST_IF		0x0009		/* IP multicast interface */
#define	SOCKET_SO_MULTICAST_IF		SOCKET_IP_MULTICAST_IF	/* backward compatibility */
#define SOCKET_IP_MULTICAST_TTL 	0x000A		/* IP multicast TTL (hop limit) */
#define SOCKET_IP_MULTICAST_LOOP	0x000B		/* IP multicast loopback */
#define SOCKET_IP_ADD_MEMBERSHIP	0x000C		/* Add an IP group membership. See category "IP multicast options" for extended parameters for this option. */
#define SOCKET_IP_DROP_MEMBERSHIP	0x000D		/* Drop an IP group membership. See category "IP multicast options" for extended parameters for this option. */
#define SOCKET_IP_DONTFRAGMENT		0x000E		/* Indicates that data should not be fragmented regardless of the local MTU. Valid only for message oriented protocols (UDP etc). */

/**
 * <category>Socket types</category>
 * <description>Different socket types</description>
 */
#define SOCKET_STREAM			1			/* stream socket */
#define SOCKET_DGRAM			2			/* datagram socket */
#define SOCKET_RAW				3			/* raw-protocol interface */
#define SOCKET_RDM				4			/* reliably-delivered message */
#define SOCKET_SEQPACKET		5			/* sequenced packet stream */

/**
 * <category>Socket class handling</category>
 * <description>Definitions of bits in internet address integers.
 *	On subnets, the decomposition of addresses to host and net parts
 *	is done according to subnet mask, not the masks here.</description>
 */
#define SOCKET_IN_CLASSA(i)            (((RTS_UI32)(i) & UINT32_C(0x80000000)) == 0)
#define SOCKET_IN_CLASSA_NET           UINT32_C(0xff000000)
#define SOCKET_IN_CLASSA_NSHIFT        24
#define SOCKET_IN_CLASSA_HOST          UINT32_C(0x00ffffff)
#define SOCKET_IN_CLASSA_MAX           128

#define SOCKET_IN_CLASSB(i)            (((RTS_UI32)(i) & UINT32_C(0xc0000000)) == UINT32_C(0x80000000))
#define SOCKET_IN_CLASSB_NET           UINT32_C(0xffff0000)
#define SOCKET_IN_CLASSB_NSHIFT        16
#define SOCKET_IN_CLASSB_HOST          UINT32_C(0x0000ffff)
#define SOCKET_IN_CLASSB_MAX           65536

#define SOCKET_IN_CLASSC(i)            (((RTS_UI32)(i) & UINT32_C(0xe0000000)) == UINT32_C(0xc0000000))
#define SOCKET_IN_CLASSC_NET           UINT32_C(0xffffff00)
#define SOCKET_IN_CLASSC_NSHIFT        8
#define SOCKET_IN_CLASSC_HOST          UINT32_C(0x000000ff)

#define SOCKET_INADDR_ANY              UINT32_C(0x00000000)
#define SOCKET_INADDR_LOOPBACK         UINT32_C(0x7f000001)
#define SOCKET_INADDR_BROADCAST        UINT32_C(0xffffffff)
#define SOCKET_INADDR_NONE             UINT32_C(0xffffffff)

/**
 * <category>Ioctl commands</category>
 * <description>Control commands to set sockets to blocking or non-blocking</description>
 */
#define SOCKET_FIONREAD					1		/* use to determine the amount of data pending in the network's input buffer */
#define SOCKET_FIONBIO					2		/* set socket to non-blocking; argument must be non zero */

/**
 * <category>TCP flags</category>
 * <description></description>
 */
#define SOCKET_MSG_NONE        0x00
#define SOCKET_MSG_OOB         0x01				/* process out-of-band data */
#define SOCKET_MSG_PEEK        0x02				/* peek at incoming message */
#define SOCKET_MSG_DONTROUTE   0x04				/* send without using routing tables */
#define SOCKET_MSG_DONTWAIT	   0x08				/* send TCP frame directly without blocking */

/**
 * <category>Shutdown flags</category>
 * <description>Flags to specify, which operations are no longer be allowed</description>
 */
#define SOCKET_SD_RECEIVE	0x00
#define SOCKET_SD_SEND		0x01
#define SOCKET_SD_BOTH		0x02


/**
 * <category>INADDR</category>
 * <description>Numeric IP-Address union to access different parts of the IP-address. IP-Address in S_addr should be in ethernet byte order</description>
 * <element name="s_b1" type="IN">1 byte of IP-address</element>
 * <element name="s_b2" type="IN">2 byte of IP-address</element>
 * <element name="s_b3" type="IN">3 byte of IP-address</element>
 * <element name="s_b4" type="IN">4 byte of IP-address</element>
 * <element name="s_w1" type="IN">Loword of IP-address</element>
 * <element name="s_w2" type="IN">Hiword of IP-address</element>
 * <element name="S_addr" type="IN">Dword of IP-address</element>
 */
typedef struct
{
	union
	{
        struct
		{
			RTS_IEC_BYTE s_b1	;
			RTS_IEC_BYTE s_b2	;
			RTS_IEC_BYTE s_b3	;
			RTS_IEC_BYTE s_b4	; 
		} S_un_b;
        struct
		{
			RTS_IEC_WORD s_w1	;
			RTS_IEC_WORD s_w2	;
		} S_un_w;
        RTS_IEC_UDINT S_addr	;
    } S_un;
} INADDR;

/**
 * <category>SOCKADDRESS</category>
 * <description>The SOCKADDRESS structure is used to define a socket address</description>
 * <element name="sin_family" type="IN">Address family</element>
 * <element name="sin_port" type="IN">Port number</element>
 * <element name="sin_addr" type="IN">Ip address</element>
 * <element name="sin_zero" type="IN">Zero bytes</element>
 */
typedef struct
{
	RTS_IEC_INT		sin_family		;
	RTS_IEC_UINT	sin_port		;
	INADDR			sin_addr		;	
	RTS_IEC_BYTE	sin_zero[8]		;
} SOCKADDRESS;

/**
 * <category>TCP keepalive options</category>
 * <description>
 *	Parameters for the socket option SOCKET_SO_KEEPALIVE.
 *	NOTE:
 *	If one of the parameters is not supported, the result of SysSockSetOption() is ERR_NOT_SUPPORTED. In this case, the corresponding result of the option contains the error result.
 * </description>
 * <element name="bOn" type="IN">1=Enable keepalive, 0=Disable</element>
 * <element name="probes" type="IN">the number of unacknowledged probes to send before considering the connection dead and notifying the application layer.
 *								NOTE: Is not supported by every platform!</element>
 * <element name="probesResult" type="OUT">Error code for the probes parameter. Returns ERR_NOT_SUPPORTED, if option is not available on the target</element>
 * <element name="timeout" type="IN">specifies the timeout in milliseconds with no activity until the first keep-alive packet is sent</element>
 * <element name="timeoutResult" type="OUT">Error code for the timeout parameter. Returns ERR_NOT_SUPPORTED, if option is not available on the target</element>
 * <element name="interval" type="IN">specifies the interval in milliseconds between when successive keep-alive packets are sent if no acknowledgement is received</element>
 * <element name="intervalResult" type="OUT">Error code for the interval parameter. Returns ERR_NOT_SUPPORTED, if option is not available on the target</element>
 */
typedef struct RTS_SOCKET_SO_VALUE_TCP_KEEPALIVE_T
{
	RTS_I32 bOn;
	RTS_UI32 probes;
	RTS_RESULT probesResult;
	RTS_UI32 timeout;
	RTS_RESULT timeoutResult;
	RTS_UI32 interval;
	RTS_RESULT intervalResult;
} RTS_SOCKET_SO_VALUE_TCP_KEEPALIVE;

/**
 * <category>Linger on close options</category>
 * <description>
 *	Parameters for the socket option SOCKET_SO_LINGER.
 * </description>
 * <element name="l_onoff" type="IN">Specifies whether a socket should remain open for a specified amount of time 
 *                                   after a closesocket function call to enable queued data to be sent. 0=Socket will not remain open.</element>
 * <element name="l_linger" type="IN">The linger time in seconds. This member specifies how long to remain open after a closesocket function call to enable queued data to be sent.</element>
 */
 typedef struct
{
    RTS_IEC_WORD l_onoff;
    RTS_IEC_WORD l_linger;
} SOCKOPT_LINGER;

/**
 * <category>IP multicast options</category>
 * <description>
 *	Parameters for the socket options SOCKET_IP_ADD_MEMBERSHIP and SOCKET_IP_DROP_MEMBERSHIP to join/leave 
 *  the socket to/from the supplied multicast group on the specified interface.
 * </description>
 * <element name="imr_multiaddr" type="IN">The address of the IPv4 multicast group.</element>
 * <element name="imr_interface" type="IN">The local address of the interface on which the multicast group should be joined or dropped.</element>
 */
typedef struct RTS_SOCKET_SO_VALUE_IP_MREQ_T
{
	INADDR imr_multiaddr;
	INADDR imr_interface;
} RTS_SOCKET_SO_VALUE_IP_MREQ;

#define SOCKET_FD_SETSIZE				64

typedef struct
{
	RTS_IEC_UDINT	fd_count						;	/* how many are SET? */
	RTS_HANDLE		fd_array[SOCKET_FD_SETSIZE]		;	/* an array of SOCKETs */
} SOCKET_FD_SET;

/* Compatibility Macros */
#define SOCKET_FD_INIT(fd, set)  CAL_SysSockFdInit(fd, set)
#define SOCKET_FD_ISSET(fd, set) CAL_SysSockFdIsset(fd, set)
#define SOCKET_FD_ZERO(set)      CAL_SysSockFdZero(set)
#define SOCKET_ZERO_FD(set)      CAL_SysSockFdZero(set)

typedef struct
{
	RTS_IEC_DINT tv_sec		;	/* seconds */
	RTS_IEC_DINT tv_usec	;	/* and microseconds */
} SOCKET_TIMEVAL; 

typedef struct
{
        char    * h_name;        /* official name of host */
        char    ** h_aliases;    /* alias list */
        RTS_IEC_INT	h_addrtype;      /* host address type */
        RTS_IEC_INT	h_length;        /* length of address */
        char    ** h_addr_list;  /* list of addresses, each in ethernet byte order */
#define h_addr  h_addr_list[0]   /* address, for backward compat */
} SOCK_HOSTENT;


/**
 * <category>Adapter types</category>
 * <description></description>
 */
#define SOCK_AIT_ETHERNET			1	/* generic ethernet adapter - default, if nothing else matches*/
#define SOCK_AIT_LOCALHOST			2	/* localhost adapter */
#define SOCK_AIT_PPP				3	/* ppp adapter - broadcasts are not supported */
#define SOCK_AIT_WIRELESS			4	/* wireless (e. g. WLAN) network adapter */

/**
 * <category>Adapter information flags</category>
 * <description></description>
 */
#define SOCK_AIF_NONE					UINT32_C(0x00000000)	/* no flag set */
#define SOCK_AIF_INACTIVE				UINT32_C(0x00000001)	/* interface not ready for ip communication (no driver, no ip address, no link, ...) */
#define SOCK_AIF_ALIAS_IP				UINT32_C(0x00000002)	/* further ip (aka alias address) of adapter */
#define SOCK_AIF_DYNAMIC_IP				UINT32_C(0x00000004)	/* dynamic ip address, provided by DHCP, BOOTP, etc. Cannot be changed by SysSockSetIpAddressAndNetMask() */
#define SOCK_AIF_FIX_IP					UINT32_C(0x00000008)	/* fix (e. g. hard coded) ip address. Cannot be changed by SysSockSetIpAddressAndNetMask() */
#define SOCK_AIF_IP_CHANGE_ALLOWED		UINT32_C(0x00000010)	/* adapter is white listed to allow address changes. Ip address can be likely changed by SysSockSetIpAddressAndNetMask() */
#define SOCK_AIF_GATEWAY_INFO_VALID		UINT32_C(0x00000020)	/* DefaultGateway address is valid. Must not be set, if no Gateway information is provided by the system. */
#define SOCK_AIF_INFO_VALID				UINT32_C(0x00008000)	/* Adapter is valid - only valid adapters are returned by SysSockGetFirstAdapterInfo() or SysSockGetNextAdapterInfo() */

/**
 * <category>Static defines for adapter information </category>
 * <description></description>
 */
#define ADAPTER_INFO_STRUCT_VERSION				1	/* version number of structure, to be increased on changes */
#define ADAPTER_INFO_MAC_ADDR_LENGTH			6	/* byte length of MAC address; fix value */
#define ADAPTER_INFO_NAME_LEN				  256	/* max. length of adapter name */ 
#define ADAPTER_INFO_DESC_LEN				  256	/* max. length of adapter description */

/**
 * <category>SysSocket adapter information struct</category>
 * <description>Structure to describe a network adapter on SysSocket level</description>
 * <element name="ui32StructSize" type="OUT">Size of the structure SOCK_ADAPTER_INFO</element>
 * <element name="ui32Version" type="OUT">Version number of the structure</element>
 * <element name="ui32Index" type="OUT">Index number of this adapter. Value is constant until shutdown</element>
 * <element name="ui32Reserved" type="OUT">Reserved for internal use and to align structure</element>
 * <element name="pwszName" type="OUT">Name of the network interface</element>
 * <element name="pwszDescription" type="OUT">Description text for the network interface; may be NULL</element>
 * <element name="aui8Mac" type="OUT">MAC ID (hardware address)</element>
 * <element name="ui16Type" type="OUT">Adapter type, see category "Adapter types"</element>
 * <element name="ui32Flags" type="OUT">Flags to characterize the adapter, see category "Adapter information flags"</element>
 * <element name="IpAddr" type="OUT">Ip address in network byte order</element>
 * <element name="NetMask" type="OUT">Subnet mask in network byte order</element>
 * <element name="DefaultGateway" type="OUT">Ip address of default gateway in network byte order; only valid if SOCK_AIF_GATEWAY_INFO_VALID is set in dwFlags</element>
*/
typedef struct tagSOCK_ADAPTER_INFO
{
	RTS_UI32 ui32StructSize;
	RTS_UI32 ui32Version;
	RTS_UI32 ui32Index;
	RTS_UI32 ui32Reserved;
	RTS_WCHAR *pwszName;
	RTS_WCHAR *pwszDescription;
	RTS_UI8 aui8Mac[ADAPTER_INFO_MAC_ADDR_LENGTH];
	RTS_UI16 ui16Type;
	RTS_UI32 ui32Flags;
	INADDR IpAddr;
	INADDR NetMask;
	INADDR DefaultGateway;
} SOCK_ADAPTER_INFO;

/**
 * <category>Udp reply</category>
 * <description>Udp reply information</description>
 * <element name="ulSourceAddress" type="IN">Sender IP-Address</element>
 * <element name="szSourceAddress" type="IN">Sender IP-Address as string</element>
 * <element name="iRecv" type="IN">Number of reveived bytes</element>
 * <element name="usRecvPort" type="IN">Received port, in host byteorder!</element>
 */
typedef struct UDP_REPLYtag
{
	RTS_IEC_DWORD ulSourceAddress;
	RTS_IEC_STRING szSourceAddress[32];
	RTS_IEC_XINT iRecv;
	RTS_IEC_WORD usRecvPort;
} UDP_REPLY;

/* For compatibility with IEC library! */
#define UDP_REPLY2	UDP_REPLY

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Settings prefix for network adapter list. Has to be used for all list settings.
 *  The index number, separated by dots, have to be increased by one for each configured adapter name.
 *  Adapter names can be configured by an ASCII name oder by an unicode name. If for a index number
 *  was found a "NameUnicode" setting, the "Name" setting is ignored.
 *  Example: Adapter.0.Name=Lan1
 *           Adapter.1.Name=Lan2
 *           Adapter.2.NameUnicode="L\00a\00n\003\00"
 * </description>
 */
#define SYSSOCKET_ADAPTER_PREFIX								"Adapter"

/**
 * <category>Settings</category>
 * <type>WString</type>
. * <description>Setting for network. adapter list. Adapter name in Unicode.
 .*	Example: Adapter.0.NameUnicode="L\00a\00n\001\00"
 * </description>
 */
#define SYSSOCKETKEY_WSTRING_NAME_UNICODE						"NameUnicode"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Setting for network adapter list. Adapter name in ASCII.
 *  Only evaluted, if no "NameUnicode" setting for the same adapter index exists.
 *	Example: Adapter.0.Name="Lan1"
 * </description>
 */
#define SYSSOCKETKEY_STRING_NAME								"Name"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Setting for network adapter list to specify, if it is allowed to set the  
 *  ip address and subnet mask for this adapter. If enabled (1) the ip address and subnet mask
 *  can be set by SysSockSetIpAddressAndNetMask(). 
 *	Example: Adapter.0.Name="Lan1"
 *           Adapter.0.EnableSetIpAndMask=1    
 * </description>
 */
#define SYSSOCKETKEY_INT_ENABLE_SETIPANDMASK					"EnableSetIpAndMask"
#define SYSSOCKETVALUE_INT_ENABLE_SETIPANDMASK_DEFAULT			0

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Setting to specify the socket interface name to be used by the runtime.
 * Only for QNX. The default is "tsec0". </description>
 */
#define SYSSOCKETKEY_STRING_QNX_ITFNAME							"QNX.ItfName"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Setting to specify the IP address of the target. Both settings
 *  ipaddress and subnetmask have to be configured. The combination ipaddress=0.0.0.0
 *  and subnetmask=0.0.0.0 can be used to remove the IP address from the adapter. 
 *  With this configuration is no IP based communication possible, until an IP address is set
 *  e. g. by calling SysSockSetIpAddressAndNetMask().
 *	Example: 
 *  Adapter.0.Name="Lan1"
 *  Adapter.0.ipaddress=192.168.2.1
 *  Adapter.0.subnetmask=255.255.255.0
 *  Adapter.0.gateway=192.168.2.2
 *  Runtime systems based on SysSocketEmbedded use a simplyfied configuration, 
 *  as these support only a single network adapter: 
 *	ipaddress=192.168.2.1
 *  subnetmask=255.255.255.1
 *  Note: On some operating systems it is neccessary to store this in a non-volatile way to
 *        provide the IP address and the subnet mask for the next boot to the operating system,
 *        before the runtime systems is started. In such cases the system must be booted twice 
 *        after the setting was added or changed. The first boot is needed to read the settings
 *        by the runtime system and to store the information for the operating system. The
 *        second boot activates the setting during the operating system's startup. 
 *  OS-specific information:  
 * 	On Microsoft Windows based runtime systems the following Windows registry key will be written
 *  to disable the IP autoconfiguartion feature for the referenced adapter:
 *  IPAutoconfigurationEnabled = 0 (DWORD)
 *  The key is located here:
 *  HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\Tcpip\Parameters\Interfaces\<Guid>\
 *  Whereas <Guid> is replaced by the guid, identifying the configured network adapter.
 * </description>
 */
#define SYSSOCKETKEY_STRING_IPADDRESS							"ipaddress"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Setting to specify the subnet mask of the target. See setting ipaddress for details.</description>
 */
#define SYSSOCKETKEY_STRING_SUBNETMASK							"subnetmask"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Setting to specify the default gateway of the target. See setting ipaddress for details.</description>
 */
#define SYSSOCKETKEY_STRING_GATEWAY							    "gateway"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>Setting to specify the send IP address (destination) of the syslog messages.
 *	NOTE:
 *	"255.255.255.255" is the broadcast address to send the syslog message to all network members!
 * </description>
 */
#define SYSSOCKETKEY_STRING_SYSLOG_IPADDRESS					"sysLog.IPAddress"
#ifndef SYSSOCKETVALUE_STRING_SYSLOG_IPADDRESS_DEFAULT
	#define SYSSOCKETVALUE_STRING_SYSLOG_IPADDRESS_DEFAULT		"127.0.0.1"
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Setting to specify the UDP send port of the syslog messages</description>
 */
#define SYSSOCKETKEY_INT_SYSLOG_PORT							"sysLog.Port"
#ifndef SYSSOCKETVALUE_INT_SYSLOG_PORT_DEFAULT
	#define SYSSOCKETVALUE_INT_SYSLOG_PORT_DEFAULT				514
#endif

/**
 * <category>Compiler switch</category>
 * <description>
 *	Compiler switches to enable/disable single features in the component.
 * </description>
 * <element name="SYSSOCKET_DISABLE_SYSLOG">
 *	Switch to disable the feature completely, that all logger entries are tranmitted via sysLog messages. 
 *  The syslog messages are based on standard RFC5424 (http://tools.ietf.org/html/rfc5424).
 *	A syslog message is typically sent via UDP to "localhost" and on port 514. Both parameters can be specified (see corresponding settings).
 *	NOTE:
 *	To activate the sysLog feature, you have to specify the corresponding backend at the assigned logger, e.g.:
 *		[CmpLog]
 *		Logger.0.Backend.0.ClassId=0x00000135
 * </element>
 */

/**
 * <category>Static defines</category>
 * <description>Number of static allocated blocks in adapter information pool.</description>
 */
#ifndef SYSSOCKET_NUM_OF_STATIC_ADAPTER_INFOS
  #define SYSSOCKET_NUM_OF_STATIC_ADAPTER_INFOS 1
#endif


/**
 * <category>Static defines</category>
 * <description>Only for SysSocketEmbedded: Set Transmit MTU size.</description>
 */
#ifndef SOCKET_MTU_SIZE
  #define SOCKET_MTU_SIZE 1500
#endif

/**
 * <category>Static defines</category>
 * <description>Only for SysSocketEmbedded: Set Receive Buffer Size.</description>
 */
#ifndef SOCKET_BUFFER_SIZE
  #define SOCKET_BUFFER_SIZE (SOCKET_MTU_SIZE*3)
#endif

/**
 * <category>Static defines</category>
 * <description>Only for SysSocketEmbedded: Number of supported sockets (one is enough for communication).</description>
 */
#ifndef SYSSOCKET_NUM_OF_STATIC_SOCKETS
  #define SYSSOCKET_NUM_OF_STATIC_SOCKETS 1
#endif

/**
 * <category>Static defines</category>
 * <description>Only for SysSocketEmbedded: Number of ARP entries in our cache.</description>
 */
#ifndef SYSSOCKET_NUM_OF_STATIC_ARP_ENTRIES
  #define SYSSOCKET_NUM_OF_STATIC_ARP_ENTRIES 5
#endif

/**
 * <category>Static defines</category>
 * <description>Only for SysSocketEmbedded: Default IP Address (can be overwritten by setting).</description>
 */
#ifndef SYSSOCKET_DEFAULT_IP
  #define SYSSOCKET_DEFAULT_IP UINT32_C(0xc0a80102) /* 192.168.1.2 */
#endif

/**
 * <category>Static defines</category>
 * <description>Only for SysSocketEmbedded: Default Subnet Mask (can be overwritten by setting).</description>
 */
#ifndef SYSSOCKET_DEFAULT_SUBNET
  #define SYSSOCKET_DEFAULT_SUBNET UINT32_C(0xFFFFFF00) /* 255.255.255.0 */
#endif

/**
 * <category>Event parameter</category>
 * <element name="pwszAdapterName" type="IN">Name of the network interface; may be NULL or an empty string, if only one adapter is available</element>
 * <element name="pIpAddr" type="IN">Ip address to set in network byte order.</element>
 * <element name="pNetMask" type="IN">Subnet mask to set in network byte order.</element>
 * <element name="rResult" type="INOUT">Result</element>
 *	<ul>
 *		<li>ERR_PARAMETER: At least one of the passed pointers is NULL.</li>
 *		<li>ERR_OPERATION_DENIED: Adapter is not white listed to allow address changes or the operation was denied by the event EVT_SysSocket_BeforeSetIpAndMask.</li>
 *		<li>ERR_NOTHING_TO_DO: Only to be returned by EVT_SysSocket_BeforeSetIpAndMask (see there).</li>
 *		<li>ERR_NO_OBJECT: Adapter with the specified name does not exist.</li>
 *		<li>ERR_NO_CHANGE: Adapter have a fix (not changeable) or dynamic (DHCP) ip address.</li>
 *		<li>ERR_FAILED: Ip adress/subnet mask could not be set.</li>
 *		<li>ERR_OK: Ip adress/subnet mask was set successfully.</li>
 *	</ul>
*/
typedef struct
{
	RTS_WCHAR* pwszAdapterName; 
	INADDR* pIpAddr;
	INADDR* pNetMask;
	RTS_RESULT rResult;
} EVTPARAM_SysSocket_SetIpAndMask;
#define EVTPARAMID_SysSocket_SetIpAndMask	0x0001
#define EVTVERSION_SysSocket_SetIpAndMask	0x0001

/**
 * <category>Event parameter</category>
 * <element name="pwszAdapterName" type="IN">Name of the network interface; may be NULL or an empty string, if only one adapter is available</element>
 * <element name="pGatewayAddr" type="IN">Gateway address to set in network byte order.</element>
 * <element name="rResult" type="INOUT">Result</element>
 *	<ul>
 *		<li>ERR_PARAMETER: At least one of the passed pointers is NULL.</li>
 *		<li>ERR_OPERATION_DENIED: Adapter is not white listed to allow address changes or the operation was denied by the event EVT_SysSocket_BeforeSetIpAndMask.</li>
 *		<li>ERR_NOTHING_TO_DO: Only to be returned by EVT_SysSocket_BeforeSetIpAndMask (see there).</li>
 *		<li>ERR_NO_OBJECT: Adapter with the specified name does not exist.</li>
 *		<li>ERR_NO_CHANGE: Adapter have a fix (not changeable) or dynamic (DHCP) gateway address.</li>
 *		<li>ERR_FAILED: Gateway address could not be set.</li>
 *		<li>ERR_OK: Gateway address was set successfully.</li>
 *	</ul>
*/
typedef struct
{
	RTS_WCHAR* pwszAdapterName; 
	INADDR* pGatewayAddr;
	RTS_RESULT rResult;
} EVTPARAM_SysSocket_SetGateway;
#define EVTPARAMID_SysSocket_SetGateway	0x0002
#define EVTVERSION_SysSocket_SetGateway	0x0001

/**
 * <category>Events</category>
 * <description>Event is sent in function SysSockSetIpAddressAndNetMask() before the 
 * ip address and network mask is set in the SysSocket OS layer. The following results
 * are provided to the event:
 * ERR_OPERATION_DENIED: Adapter is not white listed to allow address changes.
 * ERR_OK: Adapter is in the white listes for address changes.
 * The result codes ERR_OPERATION_DENIED and ERR_OK can be changed to the opposite 
 * to overwrite the adapter list settings. Furthermore it is possible to implement
 * the functionality to set the ip address and network mask completely in this event.
 * In this case ERR_NOTHING_TO_DO have to be returned to signal this to the internal
 * implementation. Then the internal implementation is skipped and 
 * SysSockSetIpAddressAndNetMask() will return ERR_OK.
 * The event should only be used by OEMs an not by the application.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpDevice_InteractiveLogin</param>
 */
#define EVT_SysSocket_BeforeSetIpAndMask			MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Events</category>
 * <description>Event is sent in function SysSockSetIpAddressAndNetMask() after the 
 * ip address and network mask was set in the SysSocket OS layer. The event is called
 * in any case regardless, if the operation was successful or not. Typically this
 * is used to store the new ip configuration for example in a system bootup file 
 * to keep it also after a reboot.
 * The event should only be used by OEMs an not by the application.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpDevice_InteractiveLogin</param>
 */
#define EVT_SysSocket_AfterSetIpAndMask				MAKE_EVENTID(EVTCLASS_INFO, 2)

/**
 * <category>Events</category>
 * <description>Event is sent in function SysSockSetGateway() before the 
 * gateway address is set in the SysSocket OS layer. The following results
 * are provided to the event:
 * ERR_OPERATION_DENIED: Adapter is not white listed to allow address changes.
 * ERR_OK: Adapter is in the white lists for address changes. The result codes 
 * ERR_OPERATION_DENIED and ERR_OK can be changed to the opposite to overwrite 
 * the adapter list settings. Furthermore it is possible to implement the 
 * functionality to set the gateway address completely in this event. In this 
 * case ERR_NOTHING_TO_DO have to be returned to signal this to the internal
 * implementation. Then the internal implementation is skipped and 
 * SysSockSetGateway() will return ERR_OK.
 * The event should only be used by OEMs an not by the application.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpDevice_InteractiveLogin</param>
 */
#define EVT_SysSocket_BeforeSetGateway			    MAKE_EVENTID(EVTCLASS_INFO, 3)

/**
 * <category>Events</category>
 * <description>Event is sent in function SysSockSetGateway() after the gateway
 * address was set in the SysSocket OS layer. The event is called in any case 
 * regardless, if the operation was successful or not. Typically this is used to
 * store the new gateway configuration for example in a system bootup file to keep 
 * it also after a reboot.
 * The event should only be used by OEMs an not by the application.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpDevice_InteractiveLogin</param>
 */
#define EVT_SysSocket_AfterSetGateway				MAKE_EVENTID(EVTCLASS_INFO, 4)


/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * Structure to describe a network adapter on SysSocket level
 */
typedef struct tagSOCK_ADAPTER_INFORMATION
{
	RTS_IEC_UDINT udiStructSize;		/* Size of the structure SOCK_ADAPTER_INFORMATION returned from external implementation */
	RTS_IEC_UDINT udiVersion;		/* Version number of the structure */
	RTS_IEC_UDINT udiIndex;		/* Index number of this adapter. Value is constant until shutdown */
	RTS_IEC_UDINT udiReserved;		/* Reserved for internal use */
	RTS_IEC_WSTRING wsName[256];		/* Name of the network interface */
	RTS_IEC_WSTRING wsDescription[256];		/* Description text for the network interface; may be empty */
	RTS_IEC_BYTE abyMac[ADAPTER_INFO_MAC_ADDR_LENGTH - 1 + 1];		/* MAC ID (hardware address) */
	RTS_IEC_WORD wType;		/* Adapter type, see "Adapter types" contants ``SOCK_AIT_xxx`` in |GVL| */
	RTS_IEC_DWORD dwFlags;		/* Flags to characterize the adapter, see "Adapter information flags" constants ``SOCK_AIF_xxx`` in |GVL| */
	INADDR IpAddr;		/* Ip address in network byte order */
	INADDR NetMask;		/* Subnet mask in network byte order */
	INADDR DefaultGateway;		/* Ip address of default gateway in network byte order; only valid if |SOCK_AIF_GATEWAY_INFO_VALID| is set in dwFlags in |GVL| */
} SOCK_ADAPTER_INFORMATION;

/**
 * | Accept the next incoming TCP connection.
 * | This functions calls the function accept of the operating system, which can accept a request to connect
 *   to a socket. A new descriptor (handle) for the socket is returned. The original socket is reset to the
 *   "listening" state (see SysSockListen).
 *
 * :return: Handle to the new accepted socket or RTS_INVALID_HANDLE if failed.
 */
typedef struct tagsyssockaccept_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address of the client, who is connected */
	RTS_IEC_DINT *pdiSockAddrSize;		/* VAR_INPUT */	/* Pointer to size of socket address structure */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_HANDLE SysSockAccept;		/* VAR_OUTPUT */	
} syssockaccept_struct;

void CDECL CDECL_EXT syssockaccept(syssockaccept_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKACCEPT_IEC) (syssockaccept_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKACCEPT_NOTIMPLEMENTED)
	#define USE_syssockaccept
	#define EXT_syssockaccept
	#define GET_syssockaccept(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockaccept(p0) 
	#define CHK_syssockaccept  FALSE
	#define EXP_syssockaccept  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockaccept
	#define EXT_syssockaccept
	#define GET_syssockaccept(fl)  CAL_CMGETAPI( "syssockaccept" ) 
	#define CAL_syssockaccept  syssockaccept
	#define CHK_syssockaccept  TRUE
	#define EXP_syssockaccept  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockaccept", (RTS_UINTPTR)syssockaccept, 1, RTSITF_GET_SIGNATURE(0, 0xCA6B3076), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockaccept
	#define EXT_syssockaccept
	#define GET_syssockaccept(fl)  CAL_CMGETAPI( "syssockaccept" ) 
	#define CAL_syssockaccept  syssockaccept
	#define CHK_syssockaccept  TRUE
	#define EXP_syssockaccept  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockaccept", (RTS_UINTPTR)syssockaccept, 1, RTSITF_GET_SIGNATURE(0, 0xCA6B3076), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockaccept
	#define EXT_SysSocketsyssockaccept
	#define GET_SysSocketsyssockaccept  ERR_OK
	#define CAL_SysSocketsyssockaccept  syssockaccept
	#define CHK_SysSocketsyssockaccept  TRUE
	#define EXP_SysSocketsyssockaccept  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockaccept", (RTS_UINTPTR)syssockaccept, 1, RTSITF_GET_SIGNATURE(0, 0xCA6B3076), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockaccept
	#define EXT_syssockaccept
	#define GET_syssockaccept(fl)  CAL_CMGETAPI( "syssockaccept" ) 
	#define CAL_syssockaccept  syssockaccept
	#define CHK_syssockaccept  TRUE
	#define EXP_syssockaccept  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockaccept", (RTS_UINTPTR)syssockaccept, 1, RTSITF_GET_SIGNATURE(0, 0xCA6B3076), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockaccept  PFSYSSOCKACCEPT_IEC pfsyssockaccept;
	#define EXT_syssockaccept  extern PFSYSSOCKACCEPT_IEC pfsyssockaccept;
	#define GET_syssockaccept(fl)  s_pfCMGetAPI2( "syssockaccept", (RTS_VOID_FCTPTR *)&pfsyssockaccept, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xCA6B3076), 0x03050E00)
	#define CAL_syssockaccept  pfsyssockaccept
	#define CHK_syssockaccept  (pfsyssockaccept != NULL)
	#define EXP_syssockaccept   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockaccept", (RTS_UINTPTR)syssockaccept, 1, RTSITF_GET_SIGNATURE(0, 0xCA6B3076), 0x03050E00) 
#endif


/**
 * | Bind a socket to a socket address and port number.
 * | This functions calls the function bind of the operating system, which will assign a local address
 *   to a socket that has already been allocated to an address by |SysSockCreate|, but not yet fixed uniquely.
 *   This is usually done previous to a call to functions like |SysSockListen| or |SysSockAccept|.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockbind_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Spcket address */
	RTS_IEC_DINT diSockAddrSize;		/* VAR_INPUT */	/* Size of the socket address structure */
	RTS_IEC_RESULT SysSockBind;			/* VAR_OUTPUT */	
} syssockbind_struct;

void CDECL CDECL_EXT syssockbind(syssockbind_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKBIND_IEC) (syssockbind_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKBIND_NOTIMPLEMENTED)
	#define USE_syssockbind
	#define EXT_syssockbind
	#define GET_syssockbind(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockbind(p0) 
	#define CHK_syssockbind  FALSE
	#define EXP_syssockbind  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockbind
	#define EXT_syssockbind
	#define GET_syssockbind(fl)  CAL_CMGETAPI( "syssockbind" ) 
	#define CAL_syssockbind  syssockbind
	#define CHK_syssockbind  TRUE
	#define EXP_syssockbind  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockbind", (RTS_UINTPTR)syssockbind, 1, RTSITF_GET_SIGNATURE(0, 0xCF23D531), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockbind
	#define EXT_syssockbind
	#define GET_syssockbind(fl)  CAL_CMGETAPI( "syssockbind" ) 
	#define CAL_syssockbind  syssockbind
	#define CHK_syssockbind  TRUE
	#define EXP_syssockbind  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockbind", (RTS_UINTPTR)syssockbind, 1, RTSITF_GET_SIGNATURE(0, 0xCF23D531), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockbind
	#define EXT_SysSocketsyssockbind
	#define GET_SysSocketsyssockbind  ERR_OK
	#define CAL_SysSocketsyssockbind  syssockbind
	#define CHK_SysSocketsyssockbind  TRUE
	#define EXP_SysSocketsyssockbind  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockbind", (RTS_UINTPTR)syssockbind, 1, RTSITF_GET_SIGNATURE(0, 0xCF23D531), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockbind
	#define EXT_syssockbind
	#define GET_syssockbind(fl)  CAL_CMGETAPI( "syssockbind" ) 
	#define CAL_syssockbind  syssockbind
	#define CHK_syssockbind  TRUE
	#define EXP_syssockbind  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockbind", (RTS_UINTPTR)syssockbind, 1, RTSITF_GET_SIGNATURE(0, 0xCF23D531), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockbind  PFSYSSOCKBIND_IEC pfsyssockbind;
	#define EXT_syssockbind  extern PFSYSSOCKBIND_IEC pfsyssockbind;
	#define GET_syssockbind(fl)  s_pfCMGetAPI2( "syssockbind", (RTS_VOID_FCTPTR *)&pfsyssockbind, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xCF23D531), 0x03050E00)
	#define CAL_syssockbind  pfsyssockbind
	#define CHK_syssockbind  (pfsyssockbind != NULL)
	#define EXP_syssockbind   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockbind", (RTS_UINTPTR)syssockbind, 1, RTSITF_GET_SIGNATURE(0, 0xCF23D531), 0x03050E00) 
#endif


/**
 *  | Close a socket.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockclose_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_RESULT SysSockClose;		/* VAR_OUTPUT */	
} syssockclose_struct;

void CDECL CDECL_EXT syssockclose(syssockclose_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKCLOSE_IEC) (syssockclose_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKCLOSE_NOTIMPLEMENTED)
	#define USE_syssockclose
	#define EXT_syssockclose
	#define GET_syssockclose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockclose(p0) 
	#define CHK_syssockclose  FALSE
	#define EXP_syssockclose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockclose
	#define EXT_syssockclose
	#define GET_syssockclose(fl)  CAL_CMGETAPI( "syssockclose" ) 
	#define CAL_syssockclose  syssockclose
	#define CHK_syssockclose  TRUE
	#define EXP_syssockclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockclose", (RTS_UINTPTR)syssockclose, 1, RTSITF_GET_SIGNATURE(0, 0xF72471AA), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockclose
	#define EXT_syssockclose
	#define GET_syssockclose(fl)  CAL_CMGETAPI( "syssockclose" ) 
	#define CAL_syssockclose  syssockclose
	#define CHK_syssockclose  TRUE
	#define EXP_syssockclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockclose", (RTS_UINTPTR)syssockclose, 1, RTSITF_GET_SIGNATURE(0, 0xF72471AA), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockclose
	#define EXT_SysSocketsyssockclose
	#define GET_SysSocketsyssockclose  ERR_OK
	#define CAL_SysSocketsyssockclose  syssockclose
	#define CHK_SysSocketsyssockclose  TRUE
	#define EXP_SysSocketsyssockclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockclose", (RTS_UINTPTR)syssockclose, 1, RTSITF_GET_SIGNATURE(0, 0xF72471AA), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockclose
	#define EXT_syssockclose
	#define GET_syssockclose(fl)  CAL_CMGETAPI( "syssockclose" ) 
	#define CAL_syssockclose  syssockclose
	#define CHK_syssockclose  TRUE
	#define EXP_syssockclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockclose", (RTS_UINTPTR)syssockclose, 1, RTSITF_GET_SIGNATURE(0, 0xF72471AA), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockclose  PFSYSSOCKCLOSE_IEC pfsyssockclose;
	#define EXT_syssockclose  extern PFSYSSOCKCLOSE_IEC pfsyssockclose;
	#define GET_syssockclose(fl)  s_pfCMGetAPI2( "syssockclose", (RTS_VOID_FCTPTR *)&pfsyssockclose, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xF72471AA), 0x03050E00)
	#define CAL_syssockclose  pfsyssockclose
	#define CHK_syssockclose  (pfsyssockclose != NULL)
	#define EXP_syssockclose   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockclose", (RTS_UINTPTR)syssockclose, 1, RTSITF_GET_SIGNATURE(0, 0xF72471AA), 0x03050E00) 
#endif


/**
 * | Close a UDP socket. Handle must be retrieved by |SysSockCreateUdp| !
 * | This function calls the function closesocket of the operating system to close a socket
 *   working with the user datagram protocol UDP.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockcloseudp_struct
{
	RTS_IEC_HANDLE hSocketUdp;			/* VAR_INPUT */	/* Handle to the UDP socket. Must be opened with |SysSockCreateUdp| ! */
	RTS_IEC_RESULT SysSockCloseUdp;		/* VAR_OUTPUT */	
} syssockcloseudp_struct;

void CDECL CDECL_EXT syssockcloseudp(syssockcloseudp_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKCLOSEUDP_IEC) (syssockcloseudp_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKCLOSEUDP_NOTIMPLEMENTED)
	#define USE_syssockcloseudp
	#define EXT_syssockcloseudp
	#define GET_syssockcloseudp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockcloseudp(p0) 
	#define CHK_syssockcloseudp  FALSE
	#define EXP_syssockcloseudp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockcloseudp
	#define EXT_syssockcloseudp
	#define GET_syssockcloseudp(fl)  CAL_CMGETAPI( "syssockcloseudp" ) 
	#define CAL_syssockcloseudp  syssockcloseudp
	#define CHK_syssockcloseudp  TRUE
	#define EXP_syssockcloseudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcloseudp", (RTS_UINTPTR)syssockcloseudp, 1, RTSITF_GET_SIGNATURE(0, 0x6978E3CC), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockcloseudp
	#define EXT_syssockcloseudp
	#define GET_syssockcloseudp(fl)  CAL_CMGETAPI( "syssockcloseudp" ) 
	#define CAL_syssockcloseudp  syssockcloseudp
	#define CHK_syssockcloseudp  TRUE
	#define EXP_syssockcloseudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcloseudp", (RTS_UINTPTR)syssockcloseudp, 1, RTSITF_GET_SIGNATURE(0, 0x6978E3CC), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockcloseudp
	#define EXT_SysSocketsyssockcloseudp
	#define GET_SysSocketsyssockcloseudp  ERR_OK
	#define CAL_SysSocketsyssockcloseudp  syssockcloseudp
	#define CHK_SysSocketsyssockcloseudp  TRUE
	#define EXP_SysSocketsyssockcloseudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcloseudp", (RTS_UINTPTR)syssockcloseudp, 1, RTSITF_GET_SIGNATURE(0, 0x6978E3CC), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockcloseudp
	#define EXT_syssockcloseudp
	#define GET_syssockcloseudp(fl)  CAL_CMGETAPI( "syssockcloseudp" ) 
	#define CAL_syssockcloseudp  syssockcloseudp
	#define CHK_syssockcloseudp  TRUE
	#define EXP_syssockcloseudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcloseudp", (RTS_UINTPTR)syssockcloseudp, 1, RTSITF_GET_SIGNATURE(0, 0x6978E3CC), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockcloseudp  PFSYSSOCKCLOSEUDP_IEC pfsyssockcloseudp;
	#define EXT_syssockcloseudp  extern PFSYSSOCKCLOSEUDP_IEC pfsyssockcloseudp;
	#define GET_syssockcloseudp(fl)  s_pfCMGetAPI2( "syssockcloseudp", (RTS_VOID_FCTPTR *)&pfsyssockcloseudp, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x6978E3CC), 0x03050E00)
	#define CAL_syssockcloseudp  pfsyssockcloseudp
	#define CHK_syssockcloseudp  (pfsyssockcloseudp != NULL)
	#define EXP_syssockcloseudp   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcloseudp", (RTS_UINTPTR)syssockcloseudp, 1, RTSITF_GET_SIGNATURE(0, 0x6978E3CC), 0x03050E00) 
#endif


/**
 * | Connect as a client to a TCP server.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockconnect_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address of the server to connect to */
	RTS_IEC_DINT diSockAddrSize;		/* VAR_INPUT */	/* Size of socket address structure */
	RTS_IEC_RESULT SysSockConnect;		/* VAR_OUTPUT */	
} syssockconnect_struct;

void CDECL CDECL_EXT syssockconnect(syssockconnect_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKCONNECT_IEC) (syssockconnect_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKCONNECT_NOTIMPLEMENTED)
	#define USE_syssockconnect
	#define EXT_syssockconnect
	#define GET_syssockconnect(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockconnect(p0) 
	#define CHK_syssockconnect  FALSE
	#define EXP_syssockconnect  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockconnect
	#define EXT_syssockconnect
	#define GET_syssockconnect(fl)  CAL_CMGETAPI( "syssockconnect" ) 
	#define CAL_syssockconnect  syssockconnect
	#define CHK_syssockconnect  TRUE
	#define EXP_syssockconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockconnect", (RTS_UINTPTR)syssockconnect, 1, RTSITF_GET_SIGNATURE(0, 0x765FD3AB), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockconnect
	#define EXT_syssockconnect
	#define GET_syssockconnect(fl)  CAL_CMGETAPI( "syssockconnect" ) 
	#define CAL_syssockconnect  syssockconnect
	#define CHK_syssockconnect  TRUE
	#define EXP_syssockconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockconnect", (RTS_UINTPTR)syssockconnect, 1, RTSITF_GET_SIGNATURE(0, 0x765FD3AB), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockconnect
	#define EXT_SysSocketsyssockconnect
	#define GET_SysSocketsyssockconnect  ERR_OK
	#define CAL_SysSocketsyssockconnect  syssockconnect
	#define CHK_SysSocketsyssockconnect  TRUE
	#define EXP_SysSocketsyssockconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockconnect", (RTS_UINTPTR)syssockconnect, 1, RTSITF_GET_SIGNATURE(0, 0x765FD3AB), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockconnect
	#define EXT_syssockconnect
	#define GET_syssockconnect(fl)  CAL_CMGETAPI( "syssockconnect" ) 
	#define CAL_syssockconnect  syssockconnect
	#define CHK_syssockconnect  TRUE
	#define EXP_syssockconnect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockconnect", (RTS_UINTPTR)syssockconnect, 1, RTSITF_GET_SIGNATURE(0, 0x765FD3AB), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockconnect  PFSYSSOCKCONNECT_IEC pfsyssockconnect;
	#define EXT_syssockconnect  extern PFSYSSOCKCONNECT_IEC pfsyssockconnect;
	#define GET_syssockconnect(fl)  s_pfCMGetAPI2( "syssockconnect", (RTS_VOID_FCTPTR *)&pfsyssockconnect, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x765FD3AB), 0x03050E00)
	#define CAL_syssockconnect  pfsyssockconnect
	#define CHK_syssockconnect  (pfsyssockconnect != NULL)
	#define EXP_syssockconnect   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockconnect", (RTS_UINTPTR)syssockconnect, 1, RTSITF_GET_SIGNATURE(0, 0x765FD3AB), 0x03050E00) 
#endif


/**
 * Create a new socket and return the socket handle.
 * :return: Handle to the new socket. The return is a descriptor (handle) of the new socket,
 * which is required as input parameter by other library functions like |SysSockBind|, |SysSockConnect| etc.
 */
typedef struct tagsyssockcreate_struct
{
	RTS_IEC_INT iAddressFamily;			/* VAR_INPUT */	/* Socket address family */
	RTS_IEC_DINT diType;				/* VAR_INPUT */	/* Socket type */
	RTS_IEC_DINT diProtocol;			/* VAR_INPUT */	/* Socket protocol */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_HANDLE SysSockCreate;		/* VAR_OUTPUT */	
} syssockcreate_struct;

void CDECL CDECL_EXT syssockcreate(syssockcreate_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKCREATE_IEC) (syssockcreate_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKCREATE_NOTIMPLEMENTED)
	#define USE_syssockcreate
	#define EXT_syssockcreate
	#define GET_syssockcreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockcreate(p0) 
	#define CHK_syssockcreate  FALSE
	#define EXP_syssockcreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockcreate
	#define EXT_syssockcreate
	#define GET_syssockcreate(fl)  CAL_CMGETAPI( "syssockcreate" ) 
	#define CAL_syssockcreate  syssockcreate
	#define CHK_syssockcreate  TRUE
	#define EXP_syssockcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcreate", (RTS_UINTPTR)syssockcreate, 1, RTSITF_GET_SIGNATURE(0, 0xE8829CB3), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockcreate
	#define EXT_syssockcreate
	#define GET_syssockcreate(fl)  CAL_CMGETAPI( "syssockcreate" ) 
	#define CAL_syssockcreate  syssockcreate
	#define CHK_syssockcreate  TRUE
	#define EXP_syssockcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcreate", (RTS_UINTPTR)syssockcreate, 1, RTSITF_GET_SIGNATURE(0, 0xE8829CB3), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockcreate
	#define EXT_SysSocketsyssockcreate
	#define GET_SysSocketsyssockcreate  ERR_OK
	#define CAL_SysSocketsyssockcreate  syssockcreate
	#define CHK_SysSocketsyssockcreate  TRUE
	#define EXP_SysSocketsyssockcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcreate", (RTS_UINTPTR)syssockcreate, 1, RTSITF_GET_SIGNATURE(0, 0xE8829CB3), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockcreate
	#define EXT_syssockcreate
	#define GET_syssockcreate(fl)  CAL_CMGETAPI( "syssockcreate" ) 
	#define CAL_syssockcreate  syssockcreate
	#define CHK_syssockcreate  TRUE
	#define EXP_syssockcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcreate", (RTS_UINTPTR)syssockcreate, 1, RTSITF_GET_SIGNATURE(0, 0xE8829CB3), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockcreate  PFSYSSOCKCREATE_IEC pfsyssockcreate;
	#define EXT_syssockcreate  extern PFSYSSOCKCREATE_IEC pfsyssockcreate;
	#define GET_syssockcreate(fl)  s_pfCMGetAPI2( "syssockcreate", (RTS_VOID_FCTPTR *)&pfsyssockcreate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xE8829CB3), 0x03050E00)
	#define CAL_syssockcreate  pfsyssockcreate
	#define CHK_syssockcreate  (pfsyssockcreate != NULL)
	#define EXP_syssockcreate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcreate", (RTS_UINTPTR)syssockcreate, 1, RTSITF_GET_SIGNATURE(0, 0xE8829CB3), 0x03050E00) 
#endif


/**
 * | Higher level function, to create a complete UDP socket.
 * | The socket will make use of the user data protocol UDP.
 * :return: Handle to the UDP socket
 */
typedef struct tagsyssockcreateudp_struct
{
	RTS_IEC_DINT diSendPort;			/* VAR_INPUT */	/* Port number to send (host byte order) */
	RTS_IEC_DINT diRecvPort;			/* VAR_INPUT */	/* Port number to receive (host byte order) */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_HANDLE SysSockCreateUdp;	/* VAR_OUTPUT */	
} syssockcreateudp_struct;

void CDECL CDECL_EXT syssockcreateudp(syssockcreateudp_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKCREATEUDP_IEC) (syssockcreateudp_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKCREATEUDP_NOTIMPLEMENTED)
	#define USE_syssockcreateudp
	#define EXT_syssockcreateudp
	#define GET_syssockcreateudp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockcreateudp(p0) 
	#define CHK_syssockcreateudp  FALSE
	#define EXP_syssockcreateudp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockcreateudp
	#define EXT_syssockcreateudp
	#define GET_syssockcreateudp(fl)  CAL_CMGETAPI( "syssockcreateudp" ) 
	#define CAL_syssockcreateudp  syssockcreateudp
	#define CHK_syssockcreateudp  TRUE
	#define EXP_syssockcreateudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcreateudp", (RTS_UINTPTR)syssockcreateudp, 1, RTSITF_GET_SIGNATURE(0, 0x4A894324), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockcreateudp
	#define EXT_syssockcreateudp
	#define GET_syssockcreateudp(fl)  CAL_CMGETAPI( "syssockcreateudp" ) 
	#define CAL_syssockcreateudp  syssockcreateudp
	#define CHK_syssockcreateudp  TRUE
	#define EXP_syssockcreateudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcreateudp", (RTS_UINTPTR)syssockcreateudp, 1, RTSITF_GET_SIGNATURE(0, 0x4A894324), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockcreateudp
	#define EXT_SysSocketsyssockcreateudp
	#define GET_SysSocketsyssockcreateudp  ERR_OK
	#define CAL_SysSocketsyssockcreateudp  syssockcreateudp
	#define CHK_SysSocketsyssockcreateudp  TRUE
	#define EXP_SysSocketsyssockcreateudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcreateudp", (RTS_UINTPTR)syssockcreateudp, 1, RTSITF_GET_SIGNATURE(0, 0x4A894324), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockcreateudp
	#define EXT_syssockcreateudp
	#define GET_syssockcreateudp(fl)  CAL_CMGETAPI( "syssockcreateudp" ) 
	#define CAL_syssockcreateudp  syssockcreateudp
	#define CHK_syssockcreateudp  TRUE
	#define EXP_syssockcreateudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcreateudp", (RTS_UINTPTR)syssockcreateudp, 1, RTSITF_GET_SIGNATURE(0, 0x4A894324), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockcreateudp  PFSYSSOCKCREATEUDP_IEC pfsyssockcreateudp;
	#define EXT_syssockcreateudp  extern PFSYSSOCKCREATEUDP_IEC pfsyssockcreateudp;
	#define GET_syssockcreateudp(fl)  s_pfCMGetAPI2( "syssockcreateudp", (RTS_VOID_FCTPTR *)&pfsyssockcreateudp, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x4A894324), 0x03050E00)
	#define CAL_syssockcreateudp  pfsyssockcreateudp
	#define CHK_syssockcreateudp  (pfsyssockcreateudp != NULL)
	#define EXP_syssockcreateudp   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockcreateudp", (RTS_UINTPTR)syssockcreateudp, 1, RTSITF_GET_SIGNATURE(0, 0x4A894324), 0x03050E00) 
#endif


/**
 * | Add a socket to a socket set.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockfdinit_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Socket to add */
	SOCKET_FD_SET *pfs;					/* VAR_INPUT */	/* Socket Set */
	RTS_IEC_RESULT SysSockFdInit;		/* VAR_OUTPUT */	
} syssockfdinit_struct;

void CDECL CDECL_EXT syssockfdinit(syssockfdinit_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKFDINIT_IEC) (syssockfdinit_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKFDINIT_NOTIMPLEMENTED)
	#define USE_syssockfdinit
	#define EXT_syssockfdinit
	#define GET_syssockfdinit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockfdinit(p0) 
	#define CHK_syssockfdinit  FALSE
	#define EXP_syssockfdinit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockfdinit
	#define EXT_syssockfdinit
	#define GET_syssockfdinit(fl)  CAL_CMGETAPI( "syssockfdinit" ) 
	#define CAL_syssockfdinit  syssockfdinit
	#define CHK_syssockfdinit  TRUE
	#define EXP_syssockfdinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdinit", (RTS_UINTPTR)syssockfdinit, 1, 0x09C72D7D, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockfdinit
	#define EXT_syssockfdinit
	#define GET_syssockfdinit(fl)  CAL_CMGETAPI( "syssockfdinit" ) 
	#define CAL_syssockfdinit  syssockfdinit
	#define CHK_syssockfdinit  TRUE
	#define EXP_syssockfdinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdinit", (RTS_UINTPTR)syssockfdinit, 1, 0x09C72D7D, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockfdinit
	#define EXT_SysSocketsyssockfdinit
	#define GET_SysSocketsyssockfdinit  ERR_OK
	#define CAL_SysSocketsyssockfdinit  syssockfdinit
	#define CHK_SysSocketsyssockfdinit  TRUE
	#define EXP_SysSocketsyssockfdinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdinit", (RTS_UINTPTR)syssockfdinit, 1, 0x09C72D7D, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockfdinit
	#define EXT_syssockfdinit
	#define GET_syssockfdinit(fl)  CAL_CMGETAPI( "syssockfdinit" ) 
	#define CAL_syssockfdinit  syssockfdinit
	#define CHK_syssockfdinit  TRUE
	#define EXP_syssockfdinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdinit", (RTS_UINTPTR)syssockfdinit, 1, 0x09C72D7D, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockfdinit  PFSYSSOCKFDINIT_IEC pfsyssockfdinit;
	#define EXT_syssockfdinit  extern PFSYSSOCKFDINIT_IEC pfsyssockfdinit;
	#define GET_syssockfdinit(fl)  s_pfCMGetAPI2( "syssockfdinit", (RTS_VOID_FCTPTR *)&pfsyssockfdinit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x09C72D7D, 0x03050E00)
	#define CAL_syssockfdinit  pfsyssockfdinit
	#define CHK_syssockfdinit  (pfsyssockfdinit != NULL)
	#define EXP_syssockfdinit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdinit", (RTS_UINTPTR)syssockfdinit, 1, 0x09C72D7D, 0x03050E00) 
#endif


/**
 * | Check if a socket is inside of a set.
 * :return: TRUE if it is inside the set, FALSE if not.
 */
typedef struct tagsyssockfdisset_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Socket to check */
	SOCKET_FD_SET *pfs;					/* VAR_INPUT */	/* Socket Set */
	RTS_IEC_BOOL SysSockFdIsset;		/* VAR_OUTPUT */	
} syssockfdisset_struct;

void CDECL CDECL_EXT syssockfdisset(syssockfdisset_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKFDISSET_IEC) (syssockfdisset_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKFDISSET_NOTIMPLEMENTED)
	#define USE_syssockfdisset
	#define EXT_syssockfdisset
	#define GET_syssockfdisset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockfdisset(p0) 
	#define CHK_syssockfdisset  FALSE
	#define EXP_syssockfdisset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockfdisset
	#define EXT_syssockfdisset
	#define GET_syssockfdisset(fl)  CAL_CMGETAPI( "syssockfdisset" ) 
	#define CAL_syssockfdisset  syssockfdisset
	#define CHK_syssockfdisset  TRUE
	#define EXP_syssockfdisset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdisset", (RTS_UINTPTR)syssockfdisset, 1, 0x25248CA6, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockfdisset
	#define EXT_syssockfdisset
	#define GET_syssockfdisset(fl)  CAL_CMGETAPI( "syssockfdisset" ) 
	#define CAL_syssockfdisset  syssockfdisset
	#define CHK_syssockfdisset  TRUE
	#define EXP_syssockfdisset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdisset", (RTS_UINTPTR)syssockfdisset, 1, 0x25248CA6, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockfdisset
	#define EXT_SysSocketsyssockfdisset
	#define GET_SysSocketsyssockfdisset  ERR_OK
	#define CAL_SysSocketsyssockfdisset  syssockfdisset
	#define CHK_SysSocketsyssockfdisset  TRUE
	#define EXP_SysSocketsyssockfdisset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdisset", (RTS_UINTPTR)syssockfdisset, 1, 0x25248CA6, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockfdisset
	#define EXT_syssockfdisset
	#define GET_syssockfdisset(fl)  CAL_CMGETAPI( "syssockfdisset" ) 
	#define CAL_syssockfdisset  syssockfdisset
	#define CHK_syssockfdisset  TRUE
	#define EXP_syssockfdisset  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdisset", (RTS_UINTPTR)syssockfdisset, 1, 0x25248CA6, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockfdisset  PFSYSSOCKFDISSET_IEC pfsyssockfdisset;
	#define EXT_syssockfdisset  extern PFSYSSOCKFDISSET_IEC pfsyssockfdisset;
	#define GET_syssockfdisset(fl)  s_pfCMGetAPI2( "syssockfdisset", (RTS_VOID_FCTPTR *)&pfsyssockfdisset, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x25248CA6, 0x03050E00)
	#define CAL_syssockfdisset  pfsyssockfdisset
	#define CHK_syssockfdisset  (pfsyssockfdisset != NULL)
	#define EXP_syssockfdisset   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdisset", (RTS_UINTPTR)syssockfdisset, 1, 0x25248CA6, 0x03050E00) 
#endif


/**
 * | Clear a Socket set.
 * :return: Returns the runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockfdzero_struct
{
	SOCKET_FD_SET *pfs;					/* VAR_INPUT */	/* Socket Set */
	RTS_IEC_RESULT SysSockFdZero;		/* VAR_OUTPUT */	
} syssockfdzero_struct;

void CDECL CDECL_EXT syssockfdzero(syssockfdzero_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKFDZERO_IEC) (syssockfdzero_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKFDZERO_NOTIMPLEMENTED)
	#define USE_syssockfdzero
	#define EXT_syssockfdzero
	#define GET_syssockfdzero(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockfdzero(p0) 
	#define CHK_syssockfdzero  FALSE
	#define EXP_syssockfdzero  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockfdzero
	#define EXT_syssockfdzero
	#define GET_syssockfdzero(fl)  CAL_CMGETAPI( "syssockfdzero" ) 
	#define CAL_syssockfdzero  syssockfdzero
	#define CHK_syssockfdzero  TRUE
	#define EXP_syssockfdzero  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdzero", (RTS_UINTPTR)syssockfdzero, 1, 0xD6D9FDA1, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockfdzero
	#define EXT_syssockfdzero
	#define GET_syssockfdzero(fl)  CAL_CMGETAPI( "syssockfdzero" ) 
	#define CAL_syssockfdzero  syssockfdzero
	#define CHK_syssockfdzero  TRUE
	#define EXP_syssockfdzero  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdzero", (RTS_UINTPTR)syssockfdzero, 1, 0xD6D9FDA1, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockfdzero
	#define EXT_SysSocketsyssockfdzero
	#define GET_SysSocketsyssockfdzero  ERR_OK
	#define CAL_SysSocketsyssockfdzero  syssockfdzero
	#define CHK_SysSocketsyssockfdzero  TRUE
	#define EXP_SysSocketsyssockfdzero  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdzero", (RTS_UINTPTR)syssockfdzero, 1, 0xD6D9FDA1, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockfdzero
	#define EXT_syssockfdzero
	#define GET_syssockfdzero(fl)  CAL_CMGETAPI( "syssockfdzero" ) 
	#define CAL_syssockfdzero  syssockfdzero
	#define CHK_syssockfdzero  TRUE
	#define EXP_syssockfdzero  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdzero", (RTS_UINTPTR)syssockfdzero, 1, 0xD6D9FDA1, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockfdzero  PFSYSSOCKFDZERO_IEC pfsyssockfdzero;
	#define EXT_syssockfdzero  extern PFSYSSOCKFDZERO_IEC pfsyssockfdzero;
	#define GET_syssockfdzero(fl)  s_pfCMGetAPI2( "syssockfdzero", (RTS_VOID_FCTPTR *)&pfsyssockfdzero, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xD6D9FDA1, 0x03050E00)
	#define CAL_syssockfdzero  pfsyssockfdzero
	#define CHK_syssockfdzero  (pfsyssockfdzero != NULL)
	#define EXP_syssockfdzero   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockfdzero", (RTS_UINTPTR)syssockfdzero, 1, 0xD6D9FDA1, 0x03050E00) 
#endif


/**
 * | Get adapter information struct of the first network adapter.
 * | Note: It may took a while until a change of the adapter configuration is reflected here.
 * :return: Handle to be passed to SysSockGetNextAdapter() to retrieve the next adapter information.
 */
typedef struct tagsyssockgetfirstadapterinfo_struct
{
	SOCK_ADAPTER_INFORMATION *pAdapterInfo;	/* VAR_INPUT */	/* Network adapter information structure */
	RTS_IEC_UXINT *puxiAdapterInfoSize;	/* VAR_INPUT */	/* Size in bytes of |SOCK_ADAPTER_INFORMATION|; returns structure size from external implementation */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_HANDLE SysSockGetFirstAdapterInfo;	/* VAR_OUTPUT */	
} syssockgetfirstadapterinfo_struct;

void CDECL CDECL_EXT syssockgetfirstadapterinfo(syssockgetfirstadapterinfo_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKGETFIRSTADAPTERINFO_IEC) (syssockgetfirstadapterinfo_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETFIRSTADAPTERINFO_NOTIMPLEMENTED)
	#define USE_syssockgetfirstadapterinfo
	#define EXT_syssockgetfirstadapterinfo
	#define GET_syssockgetfirstadapterinfo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockgetfirstadapterinfo(p0) 
	#define CHK_syssockgetfirstadapterinfo  FALSE
	#define EXP_syssockgetfirstadapterinfo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockgetfirstadapterinfo
	#define EXT_syssockgetfirstadapterinfo
	#define GET_syssockgetfirstadapterinfo(fl)  CAL_CMGETAPI( "syssockgetfirstadapterinfo" ) 
	#define CAL_syssockgetfirstadapterinfo  syssockgetfirstadapterinfo
	#define CHK_syssockgetfirstadapterinfo  TRUE
	#define EXP_syssockgetfirstadapterinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetfirstadapterinfo", (RTS_UINTPTR)syssockgetfirstadapterinfo, 1, RTSITF_GET_SIGNATURE(0x0FE33BC1, 0xF08189B5), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockgetfirstadapterinfo
	#define EXT_syssockgetfirstadapterinfo
	#define GET_syssockgetfirstadapterinfo(fl)  CAL_CMGETAPI( "syssockgetfirstadapterinfo" ) 
	#define CAL_syssockgetfirstadapterinfo  syssockgetfirstadapterinfo
	#define CHK_syssockgetfirstadapterinfo  TRUE
	#define EXP_syssockgetfirstadapterinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetfirstadapterinfo", (RTS_UINTPTR)syssockgetfirstadapterinfo, 1, RTSITF_GET_SIGNATURE(0x0FE33BC1, 0xF08189B5), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockgetfirstadapterinfo
	#define EXT_SysSocketsyssockgetfirstadapterinfo
	#define GET_SysSocketsyssockgetfirstadapterinfo  ERR_OK
	#define CAL_SysSocketsyssockgetfirstadapterinfo  syssockgetfirstadapterinfo
	#define CHK_SysSocketsyssockgetfirstadapterinfo  TRUE
	#define EXP_SysSocketsyssockgetfirstadapterinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetfirstadapterinfo", (RTS_UINTPTR)syssockgetfirstadapterinfo, 1, RTSITF_GET_SIGNATURE(0x0FE33BC1, 0xF08189B5), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockgetfirstadapterinfo
	#define EXT_syssockgetfirstadapterinfo
	#define GET_syssockgetfirstadapterinfo(fl)  CAL_CMGETAPI( "syssockgetfirstadapterinfo" ) 
	#define CAL_syssockgetfirstadapterinfo  syssockgetfirstadapterinfo
	#define CHK_syssockgetfirstadapterinfo  TRUE
	#define EXP_syssockgetfirstadapterinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetfirstadapterinfo", (RTS_UINTPTR)syssockgetfirstadapterinfo, 1, RTSITF_GET_SIGNATURE(0x0FE33BC1, 0xF08189B5), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockgetfirstadapterinfo  PFSYSSOCKGETFIRSTADAPTERINFO_IEC pfsyssockgetfirstadapterinfo;
	#define EXT_syssockgetfirstadapterinfo  extern PFSYSSOCKGETFIRSTADAPTERINFO_IEC pfsyssockgetfirstadapterinfo;
	#define GET_syssockgetfirstadapterinfo(fl)  s_pfCMGetAPI2( "syssockgetfirstadapterinfo", (RTS_VOID_FCTPTR *)&pfsyssockgetfirstadapterinfo, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x0FE33BC1, 0xF08189B5), 0x03050E00)
	#define CAL_syssockgetfirstadapterinfo  pfsyssockgetfirstadapterinfo
	#define CHK_syssockgetfirstadapterinfo  (pfsyssockgetfirstadapterinfo != NULL)
	#define EXP_syssockgetfirstadapterinfo   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetfirstadapterinfo", (RTS_UINTPTR)syssockgetfirstadapterinfo, 1, RTSITF_GET_SIGNATURE(0x0FE33BC1, 0xF08189B5), 0x03050E00) 
#endif


/**
 * | Get host description specified by host name.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockgethostbyname_struct
{
	RTS_IEC_STRING *szHostName;			/* VAR_INPUT */	/* Reference to host name */
	SOCK_HOSTENT *pHost;				/* VAR_INPUT */	/* Pointer to host description */
	RTS_IEC_RESULT SysSockGetHostByName;	/* VAR_OUTPUT */	
} syssockgethostbyname_struct;

void CDECL CDECL_EXT syssockgethostbyname(syssockgethostbyname_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKGETHOSTBYNAME_IEC) (syssockgethostbyname_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETHOSTBYNAME_NOTIMPLEMENTED)
	#define USE_syssockgethostbyname
	#define EXT_syssockgethostbyname
	#define GET_syssockgethostbyname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockgethostbyname(p0) 
	#define CHK_syssockgethostbyname  FALSE
	#define EXP_syssockgethostbyname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockgethostbyname
	#define EXT_syssockgethostbyname
	#define GET_syssockgethostbyname(fl)  CAL_CMGETAPI( "syssockgethostbyname" ) 
	#define CAL_syssockgethostbyname  syssockgethostbyname
	#define CHK_syssockgethostbyname  TRUE
	#define EXP_syssockgethostbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgethostbyname", (RTS_UINTPTR)syssockgethostbyname, 1, RTSITF_GET_SIGNATURE(0, 0x05667F90), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockgethostbyname
	#define EXT_syssockgethostbyname
	#define GET_syssockgethostbyname(fl)  CAL_CMGETAPI( "syssockgethostbyname" ) 
	#define CAL_syssockgethostbyname  syssockgethostbyname
	#define CHK_syssockgethostbyname  TRUE
	#define EXP_syssockgethostbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgethostbyname", (RTS_UINTPTR)syssockgethostbyname, 1, RTSITF_GET_SIGNATURE(0, 0x05667F90), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockgethostbyname
	#define EXT_SysSocketsyssockgethostbyname
	#define GET_SysSocketsyssockgethostbyname  ERR_OK
	#define CAL_SysSocketsyssockgethostbyname  syssockgethostbyname
	#define CHK_SysSocketsyssockgethostbyname  TRUE
	#define EXP_SysSocketsyssockgethostbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgethostbyname", (RTS_UINTPTR)syssockgethostbyname, 1, RTSITF_GET_SIGNATURE(0, 0x05667F90), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockgethostbyname
	#define EXT_syssockgethostbyname
	#define GET_syssockgethostbyname(fl)  CAL_CMGETAPI( "syssockgethostbyname" ) 
	#define CAL_syssockgethostbyname  syssockgethostbyname
	#define CHK_syssockgethostbyname  TRUE
	#define EXP_syssockgethostbyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgethostbyname", (RTS_UINTPTR)syssockgethostbyname, 1, RTSITF_GET_SIGNATURE(0, 0x05667F90), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockgethostbyname  PFSYSSOCKGETHOSTBYNAME_IEC pfsyssockgethostbyname;
	#define EXT_syssockgethostbyname  extern PFSYSSOCKGETHOSTBYNAME_IEC pfsyssockgethostbyname;
	#define GET_syssockgethostbyname(fl)  s_pfCMGetAPI2( "syssockgethostbyname", (RTS_VOID_FCTPTR *)&pfsyssockgethostbyname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x05667F90), 0x03050E00)
	#define CAL_syssockgethostbyname  pfsyssockgethostbyname
	#define CHK_syssockgethostbyname  (pfsyssockgethostbyname != NULL)
	#define EXP_syssockgethostbyname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgethostbyname", (RTS_UINTPTR)syssockgethostbyname, 1, RTSITF_GET_SIGNATURE(0, 0x05667F90), 0x03050E00) 
#endif


/**
 * | Get host name of the target.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockgethostname_struct
{
	RTS_IEC_STRING *szHostName;			/* VAR_INPUT */	/* Reference to get host name */
	RTS_IEC_DINT diNameLen;				/* VAR_INPUT */	/* Maximum length of hostname */
	RTS_IEC_RESULT SysSockGetHostName;	/* VAR_OUTPUT */	
} syssockgethostname_struct;

void CDECL CDECL_EXT syssockgethostname(syssockgethostname_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKGETHOSTNAME_IEC) (syssockgethostname_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETHOSTNAME_NOTIMPLEMENTED)
	#define USE_syssockgethostname
	#define EXT_syssockgethostname
	#define GET_syssockgethostname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockgethostname(p0) 
	#define CHK_syssockgethostname  FALSE
	#define EXP_syssockgethostname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockgethostname
	#define EXT_syssockgethostname
	#define GET_syssockgethostname(fl)  CAL_CMGETAPI( "syssockgethostname" ) 
	#define CAL_syssockgethostname  syssockgethostname
	#define CHK_syssockgethostname  TRUE
	#define EXP_syssockgethostname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgethostname", (RTS_UINTPTR)syssockgethostname, 1, RTSITF_GET_SIGNATURE(0, 0x68CAC03B), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockgethostname
	#define EXT_syssockgethostname
	#define GET_syssockgethostname(fl)  CAL_CMGETAPI( "syssockgethostname" ) 
	#define CAL_syssockgethostname  syssockgethostname
	#define CHK_syssockgethostname  TRUE
	#define EXP_syssockgethostname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgethostname", (RTS_UINTPTR)syssockgethostname, 1, RTSITF_GET_SIGNATURE(0, 0x68CAC03B), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockgethostname
	#define EXT_SysSocketsyssockgethostname
	#define GET_SysSocketsyssockgethostname  ERR_OK
	#define CAL_SysSocketsyssockgethostname  syssockgethostname
	#define CHK_SysSocketsyssockgethostname  TRUE
	#define EXP_SysSocketsyssockgethostname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgethostname", (RTS_UINTPTR)syssockgethostname, 1, RTSITF_GET_SIGNATURE(0, 0x68CAC03B), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockgethostname
	#define EXT_syssockgethostname
	#define GET_syssockgethostname(fl)  CAL_CMGETAPI( "syssockgethostname" ) 
	#define CAL_syssockgethostname  syssockgethostname
	#define CHK_syssockgethostname  TRUE
	#define EXP_syssockgethostname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgethostname", (RTS_UINTPTR)syssockgethostname, 1, RTSITF_GET_SIGNATURE(0, 0x68CAC03B), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockgethostname  PFSYSSOCKGETHOSTNAME_IEC pfsyssockgethostname;
	#define EXT_syssockgethostname  extern PFSYSSOCKGETHOSTNAME_IEC pfsyssockgethostname;
	#define GET_syssockgethostname(fl)  s_pfCMGetAPI2( "syssockgethostname", (RTS_VOID_FCTPTR *)&pfsyssockgethostname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x68CAC03B), 0x03050E00)
	#define CAL_syssockgethostname  pfsyssockgethostname
	#define CHK_syssockgethostname  (pfsyssockgethostname != NULL)
	#define EXP_syssockgethostname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgethostname", (RTS_UINTPTR)syssockgethostname, 1, RTSITF_GET_SIGNATURE(0, 0x68CAC03B), 0x03050E00) 
#endif


/**
 * | Get adapter information struct of the next network adapter from the adapter list.
 * | Note: It may took a while until a change of the adapter configuration is reflected here.
 * :return: Handle to be passed to SysSockGetNextAdapter() to retrieve the next adapter information.
 */
typedef struct tagsyssockgetnextadapterinfo_struct
{
	RTS_IEC_HANDLE hPrevAdapter;		/* VAR_INPUT */	/* Handle returned by SysSockGetFirstAdapter() or by previous call of SysSockGetNextAdapter() */
	SOCK_ADAPTER_INFORMATION *pAdapterInfo;	/* VAR_INPUT */	/* Network adapter information structure */
	RTS_IEC_UXINT *puxiAdapterInfoSize;	/* VAR_INPUT */	/* Size in bytes of SOCK_ADAPTER_INFORMATION; returns structure size from external implementation */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_HANDLE SysSockGetNextAdapterInfo;	/* VAR_OUTPUT */	
} syssockgetnextadapterinfo_struct;

void CDECL CDECL_EXT syssockgetnextadapterinfo(syssockgetnextadapterinfo_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKGETNEXTADAPTERINFO_IEC) (syssockgetnextadapterinfo_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETNEXTADAPTERINFO_NOTIMPLEMENTED)
	#define USE_syssockgetnextadapterinfo
	#define EXT_syssockgetnextadapterinfo
	#define GET_syssockgetnextadapterinfo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockgetnextadapterinfo(p0) 
	#define CHK_syssockgetnextadapterinfo  FALSE
	#define EXP_syssockgetnextadapterinfo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockgetnextadapterinfo
	#define EXT_syssockgetnextadapterinfo
	#define GET_syssockgetnextadapterinfo(fl)  CAL_CMGETAPI( "syssockgetnextadapterinfo" ) 
	#define CAL_syssockgetnextadapterinfo  syssockgetnextadapterinfo
	#define CHK_syssockgetnextadapterinfo  TRUE
	#define EXP_syssockgetnextadapterinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetnextadapterinfo", (RTS_UINTPTR)syssockgetnextadapterinfo, 1, RTSITF_GET_SIGNATURE(0x1BE45D9D, 0x8EB7E5E3), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockgetnextadapterinfo
	#define EXT_syssockgetnextadapterinfo
	#define GET_syssockgetnextadapterinfo(fl)  CAL_CMGETAPI( "syssockgetnextadapterinfo" ) 
	#define CAL_syssockgetnextadapterinfo  syssockgetnextadapterinfo
	#define CHK_syssockgetnextadapterinfo  TRUE
	#define EXP_syssockgetnextadapterinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetnextadapterinfo", (RTS_UINTPTR)syssockgetnextadapterinfo, 1, RTSITF_GET_SIGNATURE(0x1BE45D9D, 0x8EB7E5E3), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockgetnextadapterinfo
	#define EXT_SysSocketsyssockgetnextadapterinfo
	#define GET_SysSocketsyssockgetnextadapterinfo  ERR_OK
	#define CAL_SysSocketsyssockgetnextadapterinfo  syssockgetnextadapterinfo
	#define CHK_SysSocketsyssockgetnextadapterinfo  TRUE
	#define EXP_SysSocketsyssockgetnextadapterinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetnextadapterinfo", (RTS_UINTPTR)syssockgetnextadapterinfo, 1, RTSITF_GET_SIGNATURE(0x1BE45D9D, 0x8EB7E5E3), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockgetnextadapterinfo
	#define EXT_syssockgetnextadapterinfo
	#define GET_syssockgetnextadapterinfo(fl)  CAL_CMGETAPI( "syssockgetnextadapterinfo" ) 
	#define CAL_syssockgetnextadapterinfo  syssockgetnextadapterinfo
	#define CHK_syssockgetnextadapterinfo  TRUE
	#define EXP_syssockgetnextadapterinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetnextadapterinfo", (RTS_UINTPTR)syssockgetnextadapterinfo, 1, RTSITF_GET_SIGNATURE(0x1BE45D9D, 0x8EB7E5E3), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockgetnextadapterinfo  PFSYSSOCKGETNEXTADAPTERINFO_IEC pfsyssockgetnextadapterinfo;
	#define EXT_syssockgetnextadapterinfo  extern PFSYSSOCKGETNEXTADAPTERINFO_IEC pfsyssockgetnextadapterinfo;
	#define GET_syssockgetnextadapterinfo(fl)  s_pfCMGetAPI2( "syssockgetnextadapterinfo", (RTS_VOID_FCTPTR *)&pfsyssockgetnextadapterinfo, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0x1BE45D9D, 0x8EB7E5E3), 0x03050E00)
	#define CAL_syssockgetnextadapterinfo  pfsyssockgetnextadapterinfo
	#define CHK_syssockgetnextadapterinfo  (pfsyssockgetnextadapterinfo != NULL)
	#define EXP_syssockgetnextadapterinfo   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetnextadapterinfo", (RTS_UINTPTR)syssockgetnextadapterinfo, 1, RTSITF_GET_SIGNATURE(0x1BE45D9D, 0x8EB7E5E3), 0x03050E00) 
#endif


/**
 * | Set options of a specified socket.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockgetoption_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_DINT diLevel;				/* VAR_INPUT */	/* Level of the socket */
	RTS_IEC_DINT diOption;				/* VAR_INPUT */	/* Socket option command */
	RTS_IEC_DINT *pdiOptionValue;		/* VAR_INPUT */	/* Pointer to the option value */
	RTS_IEC_DINT *pdiOptionLen;			/* VAR_INPUT */	/* Lenght of option value */
	RTS_IEC_RESULT SysSockGetOption;	/* VAR_OUTPUT */	
} syssockgetoption_struct;

void CDECL CDECL_EXT syssockgetoption(syssockgetoption_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKGETOPTION_IEC) (syssockgetoption_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETOPTION_NOTIMPLEMENTED)
	#define USE_syssockgetoption
	#define EXT_syssockgetoption
	#define GET_syssockgetoption(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockgetoption(p0) 
	#define CHK_syssockgetoption  FALSE
	#define EXP_syssockgetoption  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockgetoption
	#define EXT_syssockgetoption
	#define GET_syssockgetoption(fl)  CAL_CMGETAPI( "syssockgetoption" ) 
	#define CAL_syssockgetoption  syssockgetoption
	#define CHK_syssockgetoption  TRUE
	#define EXP_syssockgetoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetoption", (RTS_UINTPTR)syssockgetoption, 1, RTSITF_GET_SIGNATURE(0, 0xB7ABC5B8), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockgetoption
	#define EXT_syssockgetoption
	#define GET_syssockgetoption(fl)  CAL_CMGETAPI( "syssockgetoption" ) 
	#define CAL_syssockgetoption  syssockgetoption
	#define CHK_syssockgetoption  TRUE
	#define EXP_syssockgetoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetoption", (RTS_UINTPTR)syssockgetoption, 1, RTSITF_GET_SIGNATURE(0, 0xB7ABC5B8), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockgetoption
	#define EXT_SysSocketsyssockgetoption
	#define GET_SysSocketsyssockgetoption  ERR_OK
	#define CAL_SysSocketsyssockgetoption  syssockgetoption
	#define CHK_SysSocketsyssockgetoption  TRUE
	#define EXP_SysSocketsyssockgetoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetoption", (RTS_UINTPTR)syssockgetoption, 1, RTSITF_GET_SIGNATURE(0, 0xB7ABC5B8), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockgetoption
	#define EXT_syssockgetoption
	#define GET_syssockgetoption(fl)  CAL_CMGETAPI( "syssockgetoption" ) 
	#define CAL_syssockgetoption  syssockgetoption
	#define CHK_syssockgetoption  TRUE
	#define EXP_syssockgetoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetoption", (RTS_UINTPTR)syssockgetoption, 1, RTSITF_GET_SIGNATURE(0, 0xB7ABC5B8), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockgetoption  PFSYSSOCKGETOPTION_IEC pfsyssockgetoption;
	#define EXT_syssockgetoption  extern PFSYSSOCKGETOPTION_IEC pfsyssockgetoption;
	#define GET_syssockgetoption(fl)  s_pfCMGetAPI2( "syssockgetoption", (RTS_VOID_FCTPTR *)&pfsyssockgetoption, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB7ABC5B8), 0x03050E00)
	#define CAL_syssockgetoption  pfsyssockgetoption
	#define CHK_syssockgetoption  (pfsyssockgetoption != NULL)
	#define EXP_syssockgetoption   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetoption", (RTS_UINTPTR)syssockgetoption, 1, RTSITF_GET_SIGNATURE(0, 0xB7ABC5B8), 0x03050E00) 
#endif


/**
 * | Get operating system handle of the UDP socket.
 * :return: Operating system handle
 */
typedef struct tagsyssockgetoshandle_struct
{
	RTS_IEC_HANDLE hSocketUdp;			/* VAR_INPUT */	/* Handle to the UDP socket */
	RTS_IEC_HANDLE SysSockGetOSHandle;	/* VAR_OUTPUT */	
} syssockgetoshandle_struct;

void CDECL CDECL_EXT syssockgetoshandle(syssockgetoshandle_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKGETOSHANDLE_IEC) (syssockgetoshandle_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETOSHANDLE_NOTIMPLEMENTED)
	#define USE_syssockgetoshandle
	#define EXT_syssockgetoshandle
	#define GET_syssockgetoshandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockgetoshandle(p0) 
	#define CHK_syssockgetoshandle  FALSE
	#define EXP_syssockgetoshandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockgetoshandle
	#define EXT_syssockgetoshandle
	#define GET_syssockgetoshandle(fl)  CAL_CMGETAPI( "syssockgetoshandle" ) 
	#define CAL_syssockgetoshandle  syssockgetoshandle
	#define CHK_syssockgetoshandle  TRUE
	#define EXP_syssockgetoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetoshandle", (RTS_UINTPTR)syssockgetoshandle, 1, RTSITF_GET_SIGNATURE(0, 0x784811EB), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockgetoshandle
	#define EXT_syssockgetoshandle
	#define GET_syssockgetoshandle(fl)  CAL_CMGETAPI( "syssockgetoshandle" ) 
	#define CAL_syssockgetoshandle  syssockgetoshandle
	#define CHK_syssockgetoshandle  TRUE
	#define EXP_syssockgetoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetoshandle", (RTS_UINTPTR)syssockgetoshandle, 1, RTSITF_GET_SIGNATURE(0, 0x784811EB), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockgetoshandle
	#define EXT_SysSocketsyssockgetoshandle
	#define GET_SysSocketsyssockgetoshandle  ERR_OK
	#define CAL_SysSocketsyssockgetoshandle  syssockgetoshandle
	#define CHK_SysSocketsyssockgetoshandle  TRUE
	#define EXP_SysSocketsyssockgetoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetoshandle", (RTS_UINTPTR)syssockgetoshandle, 1, RTSITF_GET_SIGNATURE(0, 0x784811EB), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockgetoshandle
	#define EXT_syssockgetoshandle
	#define GET_syssockgetoshandle(fl)  CAL_CMGETAPI( "syssockgetoshandle" ) 
	#define CAL_syssockgetoshandle  syssockgetoshandle
	#define CHK_syssockgetoshandle  TRUE
	#define EXP_syssockgetoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetoshandle", (RTS_UINTPTR)syssockgetoshandle, 1, RTSITF_GET_SIGNATURE(0, 0x784811EB), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockgetoshandle  PFSYSSOCKGETOSHANDLE_IEC pfsyssockgetoshandle;
	#define EXT_syssockgetoshandle  extern PFSYSSOCKGETOSHANDLE_IEC pfsyssockgetoshandle;
	#define GET_syssockgetoshandle(fl)  s_pfCMGetAPI2( "syssockgetoshandle", (RTS_VOID_FCTPTR *)&pfsyssockgetoshandle, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x784811EB), 0x03050E00)
	#define CAL_syssockgetoshandle  pfsyssockgetoshandle
	#define CHK_syssockgetoshandle  (pfsyssockgetoshandle != NULL)
	#define EXP_syssockgetoshandle   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetoshandle", (RTS_UINTPTR)syssockgetoshandle, 1, RTSITF_GET_SIGNATURE(0, 0x784811EB), 0x03050E00) 
#endif


/**
 * | Returns the socket address of the peer to which a socket is connected.
 * | The SysSockGetPeerName function can be used only on a connected socket. 
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockgetpeername_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address of the peer */
	RTS_IEC_DINT *pdiSockAddrSize;		/* VAR_INPUT */	/* Pointer to size of socket address structure */
	RTS_IEC_RESULT SysSockGetPeerName;	/* VAR_OUTPUT */	
} syssockgetpeername_struct;

void CDECL CDECL_EXT syssockgetpeername(syssockgetpeername_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKGETPEERNAME_IEC) (syssockgetpeername_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETPEERNAME_NOTIMPLEMENTED)
	#define USE_syssockgetpeername
	#define EXT_syssockgetpeername
	#define GET_syssockgetpeername(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockgetpeername(p0) 
	#define CHK_syssockgetpeername  FALSE
	#define EXP_syssockgetpeername  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockgetpeername
	#define EXT_syssockgetpeername
	#define GET_syssockgetpeername(fl)  CAL_CMGETAPI( "syssockgetpeername" ) 
	#define CAL_syssockgetpeername  syssockgetpeername
	#define CHK_syssockgetpeername  TRUE
	#define EXP_syssockgetpeername  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetpeername", (RTS_UINTPTR)syssockgetpeername, 1, 0xE0A45F4A, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockgetpeername
	#define EXT_syssockgetpeername
	#define GET_syssockgetpeername(fl)  CAL_CMGETAPI( "syssockgetpeername" ) 
	#define CAL_syssockgetpeername  syssockgetpeername
	#define CHK_syssockgetpeername  TRUE
	#define EXP_syssockgetpeername  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetpeername", (RTS_UINTPTR)syssockgetpeername, 1, 0xE0A45F4A, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockgetpeername
	#define EXT_SysSocketsyssockgetpeername
	#define GET_SysSocketsyssockgetpeername  ERR_OK
	#define CAL_SysSocketsyssockgetpeername  syssockgetpeername
	#define CHK_SysSocketsyssockgetpeername  TRUE
	#define EXP_SysSocketsyssockgetpeername  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetpeername", (RTS_UINTPTR)syssockgetpeername, 1, 0xE0A45F4A, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockgetpeername
	#define EXT_syssockgetpeername
	#define GET_syssockgetpeername(fl)  CAL_CMGETAPI( "syssockgetpeername" ) 
	#define CAL_syssockgetpeername  syssockgetpeername
	#define CHK_syssockgetpeername  TRUE
	#define EXP_syssockgetpeername  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetpeername", (RTS_UINTPTR)syssockgetpeername, 1, 0xE0A45F4A, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockgetpeername  PFSYSSOCKGETPEERNAME_IEC pfsyssockgetpeername;
	#define EXT_syssockgetpeername  extern PFSYSSOCKGETPEERNAME_IEC pfsyssockgetpeername;
	#define GET_syssockgetpeername(fl)  s_pfCMGetAPI2( "syssockgetpeername", (RTS_VOID_FCTPTR *)&pfsyssockgetpeername, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xE0A45F4A, 0x03050E00)
	#define CAL_syssockgetpeername  pfsyssockgetpeername
	#define CHK_syssockgetpeername  (pfsyssockgetpeername != NULL)
	#define EXP_syssockgetpeername   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetpeername", (RTS_UINTPTR)syssockgetpeername, 1, 0xE0A45F4A, 0x03050E00) 
#endif


/**
 * Check actual received data on the UDP socket.
 * :return: Number of bytes actual available in the socket
 */
typedef struct tagsyssockgetrecvsizeudp_struct
{
	RTS_IEC_HANDLE hSocketUdp;			/* VAR_INPUT */	/* Handle to the UDP socket */
	RTS_IEC_DINT diTimeout;				/* VAR_INPUT */	/* Timeout to wait for received data.

 - -1 = Infinite wait
 -  0 = no wait */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSockGetRecvSizeUdp;	/* VAR_OUTPUT */	
} syssockgetrecvsizeudp_struct;

void CDECL CDECL_EXT syssockgetrecvsizeudp(syssockgetrecvsizeudp_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKGETRECVSIZEUDP_IEC) (syssockgetrecvsizeudp_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETRECVSIZEUDP_NOTIMPLEMENTED)
	#define USE_syssockgetrecvsizeudp
	#define EXT_syssockgetrecvsizeudp
	#define GET_syssockgetrecvsizeudp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockgetrecvsizeudp(p0) 
	#define CHK_syssockgetrecvsizeudp  FALSE
	#define EXP_syssockgetrecvsizeudp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockgetrecvsizeudp
	#define EXT_syssockgetrecvsizeudp
	#define GET_syssockgetrecvsizeudp(fl)  CAL_CMGETAPI( "syssockgetrecvsizeudp" ) 
	#define CAL_syssockgetrecvsizeudp  syssockgetrecvsizeudp
	#define CHK_syssockgetrecvsizeudp  TRUE
	#define EXP_syssockgetrecvsizeudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetrecvsizeudp", (RTS_UINTPTR)syssockgetrecvsizeudp, 1, RTSITF_GET_SIGNATURE(0, 0xDBEEF58B), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockgetrecvsizeudp
	#define EXT_syssockgetrecvsizeudp
	#define GET_syssockgetrecvsizeudp(fl)  CAL_CMGETAPI( "syssockgetrecvsizeudp" ) 
	#define CAL_syssockgetrecvsizeudp  syssockgetrecvsizeudp
	#define CHK_syssockgetrecvsizeudp  TRUE
	#define EXP_syssockgetrecvsizeudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetrecvsizeudp", (RTS_UINTPTR)syssockgetrecvsizeudp, 1, RTSITF_GET_SIGNATURE(0, 0xDBEEF58B), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockgetrecvsizeudp
	#define EXT_SysSocketsyssockgetrecvsizeudp
	#define GET_SysSocketsyssockgetrecvsizeudp  ERR_OK
	#define CAL_SysSocketsyssockgetrecvsizeudp  syssockgetrecvsizeudp
	#define CHK_SysSocketsyssockgetrecvsizeudp  TRUE
	#define EXP_SysSocketsyssockgetrecvsizeudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetrecvsizeudp", (RTS_UINTPTR)syssockgetrecvsizeudp, 1, RTSITF_GET_SIGNATURE(0, 0xDBEEF58B), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockgetrecvsizeudp
	#define EXT_syssockgetrecvsizeudp
	#define GET_syssockgetrecvsizeudp(fl)  CAL_CMGETAPI( "syssockgetrecvsizeudp" ) 
	#define CAL_syssockgetrecvsizeudp  syssockgetrecvsizeudp
	#define CHK_syssockgetrecvsizeudp  TRUE
	#define EXP_syssockgetrecvsizeudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetrecvsizeudp", (RTS_UINTPTR)syssockgetrecvsizeudp, 1, RTSITF_GET_SIGNATURE(0, 0xDBEEF58B), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockgetrecvsizeudp  PFSYSSOCKGETRECVSIZEUDP_IEC pfsyssockgetrecvsizeudp;
	#define EXT_syssockgetrecvsizeudp  extern PFSYSSOCKGETRECVSIZEUDP_IEC pfsyssockgetrecvsizeudp;
	#define GET_syssockgetrecvsizeudp(fl)  s_pfCMGetAPI2( "syssockgetrecvsizeudp", (RTS_VOID_FCTPTR *)&pfsyssockgetrecvsizeudp, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xDBEEF58B), 0x03050E00)
	#define CAL_syssockgetrecvsizeudp  pfsyssockgetrecvsizeudp
	#define CHK_syssockgetrecvsizeudp  (pfsyssockgetrecvsizeudp != NULL)
	#define EXP_syssockgetrecvsizeudp   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetrecvsizeudp", (RTS_UINTPTR)syssockgetrecvsizeudp, 1, RTSITF_GET_SIGNATURE(0, 0xDBEEF58B), 0x03050E00) 
#endif


/**
 * | Returns the socket address of the local socket.
 * | Usally this function is called to retrieve the local socket address on multihomed hosts or to get 
 *   the local port number in client implemntations. If the socket is neither connected nor locally bound to an address,
 *   the result of the function and the value stored in the object pointed to by pSockAddress is unspecified. 
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockgetsockname_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address of the local socket */
	RTS_IEC_DINT *pdiSockAddrSize;		/* VAR_INPUT */	/* Pointer to size of socket address structure */
	RTS_IEC_RESULT SysSockGetSockName;	/* VAR_OUTPUT */	
} syssockgetsockname_struct;

void CDECL CDECL_EXT syssockgetsockname(syssockgetsockname_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKGETSOCKNAME_IEC) (syssockgetsockname_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETSOCKNAME_NOTIMPLEMENTED)
	#define USE_syssockgetsockname
	#define EXT_syssockgetsockname
	#define GET_syssockgetsockname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockgetsockname(p0) 
	#define CHK_syssockgetsockname  FALSE
	#define EXP_syssockgetsockname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockgetsockname
	#define EXT_syssockgetsockname
	#define GET_syssockgetsockname(fl)  CAL_CMGETAPI( "syssockgetsockname" ) 
	#define CAL_syssockgetsockname  syssockgetsockname
	#define CHK_syssockgetsockname  TRUE
	#define EXP_syssockgetsockname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetsockname", (RTS_UINTPTR)syssockgetsockname, 1, 0x7EE86135, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockgetsockname
	#define EXT_syssockgetsockname
	#define GET_syssockgetsockname(fl)  CAL_CMGETAPI( "syssockgetsockname" ) 
	#define CAL_syssockgetsockname  syssockgetsockname
	#define CHK_syssockgetsockname  TRUE
	#define EXP_syssockgetsockname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetsockname", (RTS_UINTPTR)syssockgetsockname, 1, 0x7EE86135, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockgetsockname
	#define EXT_SysSocketsyssockgetsockname
	#define GET_SysSocketsyssockgetsockname  ERR_OK
	#define CAL_SysSocketsyssockgetsockname  syssockgetsockname
	#define CHK_SysSocketsyssockgetsockname  TRUE
	#define EXP_SysSocketsyssockgetsockname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetsockname", (RTS_UINTPTR)syssockgetsockname, 1, 0x7EE86135, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockgetsockname
	#define EXT_syssockgetsockname
	#define GET_syssockgetsockname(fl)  CAL_CMGETAPI( "syssockgetsockname" ) 
	#define CAL_syssockgetsockname  syssockgetsockname
	#define CHK_syssockgetsockname  TRUE
	#define EXP_syssockgetsockname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetsockname", (RTS_UINTPTR)syssockgetsockname, 1, 0x7EE86135, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockgetsockname  PFSYSSOCKGETSOCKNAME_IEC pfsyssockgetsockname;
	#define EXT_syssockgetsockname  extern PFSYSSOCKGETSOCKNAME_IEC pfsyssockgetsockname;
	#define GET_syssockgetsockname(fl)  s_pfCMGetAPI2( "syssockgetsockname", (RTS_VOID_FCTPTR *)&pfsyssockgetsockname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x7EE86135, 0x03050E00)
	#define CAL_syssockgetsockname  pfsyssockgetsockname
	#define CHK_syssockgetsockname  (pfsyssockgetsockname != NULL)
	#define EXP_syssockgetsockname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetsockname", (RTS_UINTPTR)syssockgetsockname, 1, 0x7EE86135, 0x03050E00) 
#endif


/**
 * | Get subnetmask of a specified IP address adapter.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockgetsubnetmask_struct
{
	RTS_IEC_STRING *szIPAddress;		/* VAR_INPUT */	/* IP address of the communication partner as string */
	RTS_IEC_STRING *szSubnetMask;		/* VAR_INPUT */	/* Subnet mask as string */
	RTS_IEC_DINT diMaxSugnetMask;		/* VAR_INPUT */	/* Maximum length of the subnet mask string */
	RTS_IEC_RESULT SysSockGetSubnetMask;	/* VAR_OUTPUT */	
} syssockgetsubnetmask_struct;

void CDECL CDECL_EXT syssockgetsubnetmask(syssockgetsubnetmask_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKGETSUBNETMASK_IEC) (syssockgetsubnetmask_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETSUBNETMASK_NOTIMPLEMENTED)
	#define USE_syssockgetsubnetmask
	#define EXT_syssockgetsubnetmask
	#define GET_syssockgetsubnetmask(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockgetsubnetmask(p0) 
	#define CHK_syssockgetsubnetmask  FALSE
	#define EXP_syssockgetsubnetmask  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockgetsubnetmask
	#define EXT_syssockgetsubnetmask
	#define GET_syssockgetsubnetmask(fl)  CAL_CMGETAPI( "syssockgetsubnetmask" ) 
	#define CAL_syssockgetsubnetmask  syssockgetsubnetmask
	#define CHK_syssockgetsubnetmask  TRUE
	#define EXP_syssockgetsubnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetsubnetmask", (RTS_UINTPTR)syssockgetsubnetmask, 1, RTSITF_GET_SIGNATURE(0, 0x972C29B9), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockgetsubnetmask
	#define EXT_syssockgetsubnetmask
	#define GET_syssockgetsubnetmask(fl)  CAL_CMGETAPI( "syssockgetsubnetmask" ) 
	#define CAL_syssockgetsubnetmask  syssockgetsubnetmask
	#define CHK_syssockgetsubnetmask  TRUE
	#define EXP_syssockgetsubnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetsubnetmask", (RTS_UINTPTR)syssockgetsubnetmask, 1, RTSITF_GET_SIGNATURE(0, 0x972C29B9), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockgetsubnetmask
	#define EXT_SysSocketsyssockgetsubnetmask
	#define GET_SysSocketsyssockgetsubnetmask  ERR_OK
	#define CAL_SysSocketsyssockgetsubnetmask  syssockgetsubnetmask
	#define CHK_SysSocketsyssockgetsubnetmask  TRUE
	#define EXP_SysSocketsyssockgetsubnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetsubnetmask", (RTS_UINTPTR)syssockgetsubnetmask, 1, RTSITF_GET_SIGNATURE(0, 0x972C29B9), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockgetsubnetmask
	#define EXT_syssockgetsubnetmask
	#define GET_syssockgetsubnetmask(fl)  CAL_CMGETAPI( "syssockgetsubnetmask" ) 
	#define CAL_syssockgetsubnetmask  syssockgetsubnetmask
	#define CHK_syssockgetsubnetmask  TRUE
	#define EXP_syssockgetsubnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetsubnetmask", (RTS_UINTPTR)syssockgetsubnetmask, 1, RTSITF_GET_SIGNATURE(0, 0x972C29B9), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockgetsubnetmask  PFSYSSOCKGETSUBNETMASK_IEC pfsyssockgetsubnetmask;
	#define EXT_syssockgetsubnetmask  extern PFSYSSOCKGETSUBNETMASK_IEC pfsyssockgetsubnetmask;
	#define GET_syssockgetsubnetmask(fl)  s_pfCMGetAPI2( "syssockgetsubnetmask", (RTS_VOID_FCTPTR *)&pfsyssockgetsubnetmask, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x972C29B9), 0x03050E00)
	#define CAL_syssockgetsubnetmask  pfsyssockgetsubnetmask
	#define CHK_syssockgetsubnetmask  (pfsyssockgetsubnetmask != NULL)
	#define EXP_syssockgetsubnetmask   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockgetsubnetmask", (RTS_UINTPTR)syssockgetsubnetmask, 1, RTSITF_GET_SIGNATURE(0, 0x972C29B9), 0x03050E00) 
#endif


/**
 * | Converts a value of type UDINT from host byte order to the order of the TCP/IP network.
 * :return: Converted UDINT value
 */
typedef struct tagsyssockhtonl_struct
{
	RTS_IEC_UDINT ulHost;				/* VAR_INPUT */	/* Host value */
	RTS_IEC_UDINT SysSockHtonl;			/* VAR_OUTPUT */	
} syssockhtonl_struct;

void CDECL CDECL_EXT syssockhtonl(syssockhtonl_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKHTONL_IEC) (syssockhtonl_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKHTONL_NOTIMPLEMENTED)
	#define USE_syssockhtonl
	#define EXT_syssockhtonl
	#define GET_syssockhtonl(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockhtonl(p0) 
	#define CHK_syssockhtonl  FALSE
	#define EXP_syssockhtonl  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockhtonl
	#define EXT_syssockhtonl
	#define GET_syssockhtonl(fl)  CAL_CMGETAPI( "syssockhtonl" ) 
	#define CAL_syssockhtonl  syssockhtonl
	#define CHK_syssockhtonl  TRUE
	#define EXP_syssockhtonl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockhtonl", (RTS_UINTPTR)syssockhtonl, 1, RTSITF_GET_SIGNATURE(0, 0x72F6021A), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockhtonl
	#define EXT_syssockhtonl
	#define GET_syssockhtonl(fl)  CAL_CMGETAPI( "syssockhtonl" ) 
	#define CAL_syssockhtonl  syssockhtonl
	#define CHK_syssockhtonl  TRUE
	#define EXP_syssockhtonl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockhtonl", (RTS_UINTPTR)syssockhtonl, 1, RTSITF_GET_SIGNATURE(0, 0x72F6021A), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockhtonl
	#define EXT_SysSocketsyssockhtonl
	#define GET_SysSocketsyssockhtonl  ERR_OK
	#define CAL_SysSocketsyssockhtonl  syssockhtonl
	#define CHK_SysSocketsyssockhtonl  TRUE
	#define EXP_SysSocketsyssockhtonl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockhtonl", (RTS_UINTPTR)syssockhtonl, 1, RTSITF_GET_SIGNATURE(0, 0x72F6021A), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockhtonl
	#define EXT_syssockhtonl
	#define GET_syssockhtonl(fl)  CAL_CMGETAPI( "syssockhtonl" ) 
	#define CAL_syssockhtonl  syssockhtonl
	#define CHK_syssockhtonl  TRUE
	#define EXP_syssockhtonl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockhtonl", (RTS_UINTPTR)syssockhtonl, 1, RTSITF_GET_SIGNATURE(0, 0x72F6021A), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockhtonl  PFSYSSOCKHTONL_IEC pfsyssockhtonl;
	#define EXT_syssockhtonl  extern PFSYSSOCKHTONL_IEC pfsyssockhtonl;
	#define GET_syssockhtonl(fl)  s_pfCMGetAPI2( "syssockhtonl", (RTS_VOID_FCTPTR *)&pfsyssockhtonl, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x72F6021A), 0x03050E00)
	#define CAL_syssockhtonl  pfsyssockhtonl
	#define CHK_syssockhtonl  (pfsyssockhtonl != NULL)
	#define EXP_syssockhtonl   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockhtonl", (RTS_UINTPTR)syssockhtonl, 1, RTSITF_GET_SIGNATURE(0, 0x72F6021A), 0x03050E00) 
#endif


/**
 * | Converts a value of type short from order of the TCP/IP network to the host byte order.
 * :return: Converted WORD value
 */
typedef struct tagsyssockhtons_struct
{
	RTS_IEC_WORD usHost;				/* VAR_INPUT */	/* Ethernet value */
	RTS_IEC_WORD SysSockHtons;			/* VAR_OUTPUT */	
} syssockhtons_struct;

void CDECL CDECL_EXT syssockhtons(syssockhtons_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKHTONS_IEC) (syssockhtons_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKHTONS_NOTIMPLEMENTED)
	#define USE_syssockhtons
	#define EXT_syssockhtons
	#define GET_syssockhtons(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockhtons(p0) 
	#define CHK_syssockhtons  FALSE
	#define EXP_syssockhtons  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockhtons
	#define EXT_syssockhtons
	#define GET_syssockhtons(fl)  CAL_CMGETAPI( "syssockhtons" ) 
	#define CAL_syssockhtons  syssockhtons
	#define CHK_syssockhtons  TRUE
	#define EXP_syssockhtons  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockhtons", (RTS_UINTPTR)syssockhtons, 1, RTSITF_GET_SIGNATURE(0, 0xD258956A), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockhtons
	#define EXT_syssockhtons
	#define GET_syssockhtons(fl)  CAL_CMGETAPI( "syssockhtons" ) 
	#define CAL_syssockhtons  syssockhtons
	#define CHK_syssockhtons  TRUE
	#define EXP_syssockhtons  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockhtons", (RTS_UINTPTR)syssockhtons, 1, RTSITF_GET_SIGNATURE(0, 0xD258956A), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockhtons
	#define EXT_SysSocketsyssockhtons
	#define GET_SysSocketsyssockhtons  ERR_OK
	#define CAL_SysSocketsyssockhtons  syssockhtons
	#define CHK_SysSocketsyssockhtons  TRUE
	#define EXP_SysSocketsyssockhtons  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockhtons", (RTS_UINTPTR)syssockhtons, 1, RTSITF_GET_SIGNATURE(0, 0xD258956A), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockhtons
	#define EXT_syssockhtons
	#define GET_syssockhtons(fl)  CAL_CMGETAPI( "syssockhtons" ) 
	#define CAL_syssockhtons  syssockhtons
	#define CHK_syssockhtons  TRUE
	#define EXP_syssockhtons  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockhtons", (RTS_UINTPTR)syssockhtons, 1, RTSITF_GET_SIGNATURE(0, 0xD258956A), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockhtons  PFSYSSOCKHTONS_IEC pfsyssockhtons;
	#define EXT_syssockhtons  extern PFSYSSOCKHTONS_IEC pfsyssockhtons;
	#define GET_syssockhtons(fl)  s_pfCMGetAPI2( "syssockhtons", (RTS_VOID_FCTPTR *)&pfsyssockhtons, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xD258956A), 0x03050E00)
	#define CAL_syssockhtons  pfsyssockhtons
	#define CHK_syssockhtons  (pfsyssockhtons != NULL)
	#define EXP_syssockhtons   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockhtons", (RTS_UINTPTR)syssockhtons, 1, RTSITF_GET_SIGNATURE(0, 0xD258956A), 0x03050E00) 
#endif


/**
 * | Convert an IP address string into an IP address.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockinetaddr_struct
{
	RTS_IEC_STRING *szIPAddress;		/* VAR_INPUT */	/* Pointer to get IP address string (must be at least 16 bytes long) */
	RTS_IEC_UDINT *pInAddr;				/* VAR_INPUT */	/* Pointer to IP address description */
	RTS_IEC_RESULT SysSockInetAddr;		/* VAR_OUTPUT */	
} syssockinetaddr_struct;

void CDECL CDECL_EXT syssockinetaddr(syssockinetaddr_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKINETADDR_IEC) (syssockinetaddr_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKINETADDR_NOTIMPLEMENTED)
	#define USE_syssockinetaddr
	#define EXT_syssockinetaddr
	#define GET_syssockinetaddr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockinetaddr(p0) 
	#define CHK_syssockinetaddr  FALSE
	#define EXP_syssockinetaddr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockinetaddr
	#define EXT_syssockinetaddr
	#define GET_syssockinetaddr(fl)  CAL_CMGETAPI( "syssockinetaddr" ) 
	#define CAL_syssockinetaddr  syssockinetaddr
	#define CHK_syssockinetaddr  TRUE
	#define EXP_syssockinetaddr  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockinetaddr", (RTS_UINTPTR)syssockinetaddr, 1, RTSITF_GET_SIGNATURE(0, 0xB8198B8F), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockinetaddr
	#define EXT_syssockinetaddr
	#define GET_syssockinetaddr(fl)  CAL_CMGETAPI( "syssockinetaddr" ) 
	#define CAL_syssockinetaddr  syssockinetaddr
	#define CHK_syssockinetaddr  TRUE
	#define EXP_syssockinetaddr  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockinetaddr", (RTS_UINTPTR)syssockinetaddr, 1, RTSITF_GET_SIGNATURE(0, 0xB8198B8F), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockinetaddr
	#define EXT_SysSocketsyssockinetaddr
	#define GET_SysSocketsyssockinetaddr  ERR_OK
	#define CAL_SysSocketsyssockinetaddr  syssockinetaddr
	#define CHK_SysSocketsyssockinetaddr  TRUE
	#define EXP_SysSocketsyssockinetaddr  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockinetaddr", (RTS_UINTPTR)syssockinetaddr, 1, RTSITF_GET_SIGNATURE(0, 0xB8198B8F), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockinetaddr
	#define EXT_syssockinetaddr
	#define GET_syssockinetaddr(fl)  CAL_CMGETAPI( "syssockinetaddr" ) 
	#define CAL_syssockinetaddr  syssockinetaddr
	#define CHK_syssockinetaddr  TRUE
	#define EXP_syssockinetaddr  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockinetaddr", (RTS_UINTPTR)syssockinetaddr, 1, RTSITF_GET_SIGNATURE(0, 0xB8198B8F), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockinetaddr  PFSYSSOCKINETADDR_IEC pfsyssockinetaddr;
	#define EXT_syssockinetaddr  extern PFSYSSOCKINETADDR_IEC pfsyssockinetaddr;
	#define GET_syssockinetaddr(fl)  s_pfCMGetAPI2( "syssockinetaddr", (RTS_VOID_FCTPTR *)&pfsyssockinetaddr, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB8198B8F), 0x03050E00)
	#define CAL_syssockinetaddr  pfsyssockinetaddr
	#define CHK_syssockinetaddr  (pfsyssockinetaddr != NULL)
	#define EXP_syssockinetaddr   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockinetaddr", (RTS_UINTPTR)syssockinetaddr, 1, RTSITF_GET_SIGNATURE(0, 0xB8198B8F), 0x03050E00) 
#endif


/**
 * | Convert IP address to a string.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockinetntoa_struct
{
	INADDR *pInAddr;					/* VAR_INPUT */	/* Pointer to IP address description */
	RTS_IEC_STRING *szIPADDR;			/* VAR_INPUT */	/* Pointer to get IP address string (must be at least 16 bytes long) */
	RTS_IEC_DINT diIPAddrSize;			/* VAR_INPUT */	/* Maximum length of szIPAddr */
	RTS_IEC_RESULT SysSockInetNtoa;		/* VAR_OUTPUT */	
} syssockinetntoa_struct;

void CDECL CDECL_EXT syssockinetntoa(syssockinetntoa_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKINETNTOA_IEC) (syssockinetntoa_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKINETNTOA_NOTIMPLEMENTED)
	#define USE_syssockinetntoa
	#define EXT_syssockinetntoa
	#define GET_syssockinetntoa(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockinetntoa(p0) 
	#define CHK_syssockinetntoa  FALSE
	#define EXP_syssockinetntoa  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockinetntoa
	#define EXT_syssockinetntoa
	#define GET_syssockinetntoa(fl)  CAL_CMGETAPI( "syssockinetntoa" ) 
	#define CAL_syssockinetntoa  syssockinetntoa
	#define CHK_syssockinetntoa  TRUE
	#define EXP_syssockinetntoa  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockinetntoa", (RTS_UINTPTR)syssockinetntoa, 1, RTSITF_GET_SIGNATURE(0, 0x5CE722B2), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockinetntoa
	#define EXT_syssockinetntoa
	#define GET_syssockinetntoa(fl)  CAL_CMGETAPI( "syssockinetntoa" ) 
	#define CAL_syssockinetntoa  syssockinetntoa
	#define CHK_syssockinetntoa  TRUE
	#define EXP_syssockinetntoa  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockinetntoa", (RTS_UINTPTR)syssockinetntoa, 1, RTSITF_GET_SIGNATURE(0, 0x5CE722B2), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockinetntoa
	#define EXT_SysSocketsyssockinetntoa
	#define GET_SysSocketsyssockinetntoa  ERR_OK
	#define CAL_SysSocketsyssockinetntoa  syssockinetntoa
	#define CHK_SysSocketsyssockinetntoa  TRUE
	#define EXP_SysSocketsyssockinetntoa  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockinetntoa", (RTS_UINTPTR)syssockinetntoa, 1, RTSITF_GET_SIGNATURE(0, 0x5CE722B2), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockinetntoa
	#define EXT_syssockinetntoa
	#define GET_syssockinetntoa(fl)  CAL_CMGETAPI( "syssockinetntoa" ) 
	#define CAL_syssockinetntoa  syssockinetntoa
	#define CHK_syssockinetntoa  TRUE
	#define EXP_syssockinetntoa  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockinetntoa", (RTS_UINTPTR)syssockinetntoa, 1, RTSITF_GET_SIGNATURE(0, 0x5CE722B2), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockinetntoa  PFSYSSOCKINETNTOA_IEC pfsyssockinetntoa;
	#define EXT_syssockinetntoa  extern PFSYSSOCKINETNTOA_IEC pfsyssockinetntoa;
	#define GET_syssockinetntoa(fl)  s_pfCMGetAPI2( "syssockinetntoa", (RTS_VOID_FCTPTR *)&pfsyssockinetntoa, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x5CE722B2), 0x03050E00)
	#define CAL_syssockinetntoa  pfsyssockinetntoa
	#define CHK_syssockinetntoa  (pfsyssockinetntoa != NULL)
	#define EXP_syssockinetntoa   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockinetntoa", (RTS_UINTPTR)syssockinetntoa, 1, RTSITF_GET_SIGNATURE(0, 0x5CE722B2), 0x03050E00) 
#endif


/**
 * | Io-control of a socket.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockioctl_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_DINT diCommand;				/* VAR_INPUT */	/* Io-control command */
	RTS_IEC_DINT *pdiParameter;			/* VAR_INPUT */	/* Parameter value of the command */
	RTS_IEC_RESULT SysSockIoctl;		/* VAR_OUTPUT */	
} syssockioctl_struct;

void CDECL CDECL_EXT syssockioctl(syssockioctl_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKIOCTL_IEC) (syssockioctl_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKIOCTL_NOTIMPLEMENTED)
	#define USE_syssockioctl
	#define EXT_syssockioctl
	#define GET_syssockioctl(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockioctl(p0) 
	#define CHK_syssockioctl  FALSE
	#define EXP_syssockioctl  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockioctl
	#define EXT_syssockioctl
	#define GET_syssockioctl(fl)  CAL_CMGETAPI( "syssockioctl" ) 
	#define CAL_syssockioctl  syssockioctl
	#define CHK_syssockioctl  TRUE
	#define EXP_syssockioctl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockioctl", (RTS_UINTPTR)syssockioctl, 1, RTSITF_GET_SIGNATURE(0, 0x408480FB), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockioctl
	#define EXT_syssockioctl
	#define GET_syssockioctl(fl)  CAL_CMGETAPI( "syssockioctl" ) 
	#define CAL_syssockioctl  syssockioctl
	#define CHK_syssockioctl  TRUE
	#define EXP_syssockioctl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockioctl", (RTS_UINTPTR)syssockioctl, 1, RTSITF_GET_SIGNATURE(0, 0x408480FB), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockioctl
	#define EXT_SysSocketsyssockioctl
	#define GET_SysSocketsyssockioctl  ERR_OK
	#define CAL_SysSocketsyssockioctl  syssockioctl
	#define CHK_SysSocketsyssockioctl  TRUE
	#define EXP_SysSocketsyssockioctl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockioctl", (RTS_UINTPTR)syssockioctl, 1, RTSITF_GET_SIGNATURE(0, 0x408480FB), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockioctl
	#define EXT_syssockioctl
	#define GET_syssockioctl(fl)  CAL_CMGETAPI( "syssockioctl" ) 
	#define CAL_syssockioctl  syssockioctl
	#define CHK_syssockioctl  TRUE
	#define EXP_syssockioctl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockioctl", (RTS_UINTPTR)syssockioctl, 1, RTSITF_GET_SIGNATURE(0, 0x408480FB), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockioctl  PFSYSSOCKIOCTL_IEC pfsyssockioctl;
	#define EXT_syssockioctl  extern PFSYSSOCKIOCTL_IEC pfsyssockioctl;
	#define GET_syssockioctl(fl)  s_pfCMGetAPI2( "syssockioctl", (RTS_VOID_FCTPTR *)&pfsyssockioctl, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x408480FB), 0x03050E00)
	#define CAL_syssockioctl  pfsyssockioctl
	#define CHK_syssockioctl  (pfsyssockioctl != NULL)
	#define EXP_syssockioctl   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockioctl", (RTS_UINTPTR)syssockioctl, 1, RTSITF_GET_SIGNATURE(0, 0x408480FB), 0x03050E00) 
#endif


/**
 * | Listen on a TCP server socket for new connection.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssocklisten_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_DINT diMaxConnections;		/* VAR_INPUT */	/* Maximum number of connections allowed */
	RTS_IEC_RESULT SysSockListen;		/* VAR_OUTPUT */	
} syssocklisten_struct;

void CDECL CDECL_EXT syssocklisten(syssocklisten_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKLISTEN_IEC) (syssocklisten_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKLISTEN_NOTIMPLEMENTED)
	#define USE_syssocklisten
	#define EXT_syssocklisten
	#define GET_syssocklisten(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssocklisten(p0) 
	#define CHK_syssocklisten  FALSE
	#define EXP_syssocklisten  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssocklisten
	#define EXT_syssocklisten
	#define GET_syssocklisten(fl)  CAL_CMGETAPI( "syssocklisten" ) 
	#define CAL_syssocklisten  syssocklisten
	#define CHK_syssocklisten  TRUE
	#define EXP_syssocklisten  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocklisten", (RTS_UINTPTR)syssocklisten, 1, RTSITF_GET_SIGNATURE(0, 0x30FE27C1), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssocklisten
	#define EXT_syssocklisten
	#define GET_syssocklisten(fl)  CAL_CMGETAPI( "syssocklisten" ) 
	#define CAL_syssocklisten  syssocklisten
	#define CHK_syssocklisten  TRUE
	#define EXP_syssocklisten  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocklisten", (RTS_UINTPTR)syssocklisten, 1, RTSITF_GET_SIGNATURE(0, 0x30FE27C1), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssocklisten
	#define EXT_SysSocketsyssocklisten
	#define GET_SysSocketsyssocklisten  ERR_OK
	#define CAL_SysSocketsyssocklisten  syssocklisten
	#define CHK_SysSocketsyssocklisten  TRUE
	#define EXP_SysSocketsyssocklisten  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocklisten", (RTS_UINTPTR)syssocklisten, 1, RTSITF_GET_SIGNATURE(0, 0x30FE27C1), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssocklisten
	#define EXT_syssocklisten
	#define GET_syssocklisten(fl)  CAL_CMGETAPI( "syssocklisten" ) 
	#define CAL_syssocklisten  syssocklisten
	#define CHK_syssocklisten  TRUE
	#define EXP_syssocklisten  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocklisten", (RTS_UINTPTR)syssocklisten, 1, RTSITF_GET_SIGNATURE(0, 0x30FE27C1), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssocklisten  PFSYSSOCKLISTEN_IEC pfsyssocklisten;
	#define EXT_syssocklisten  extern PFSYSSOCKLISTEN_IEC pfsyssocklisten;
	#define GET_syssocklisten(fl)  s_pfCMGetAPI2( "syssocklisten", (RTS_VOID_FCTPTR *)&pfsyssocklisten, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x30FE27C1), 0x03050E00)
	#define CAL_syssocklisten  pfsyssocklisten
	#define CHK_syssocklisten  (pfsyssocklisten != NULL)
	#define EXP_syssocklisten   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocklisten", (RTS_UINTPTR)syssocklisten, 1, RTSITF_GET_SIGNATURE(0, 0x30FE27C1), 0x03050E00) 
#endif


/**
 * | Convert a UDINT value from ethernet byte order into host format.
 * :return: Converted UDINT value
 */
typedef struct tagsyssockntohl_struct
{
	RTS_IEC_UDINT ulNet;				/* VAR_INPUT */	/* Ethernet value */
	RTS_IEC_UDINT SysSockNtohl;			/* VAR_OUTPUT */	
} syssockntohl_struct;

void CDECL CDECL_EXT syssockntohl(syssockntohl_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKNTOHL_IEC) (syssockntohl_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKNTOHL_NOTIMPLEMENTED)
	#define USE_syssockntohl
	#define EXT_syssockntohl
	#define GET_syssockntohl(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockntohl(p0) 
	#define CHK_syssockntohl  FALSE
	#define EXP_syssockntohl  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockntohl
	#define EXT_syssockntohl
	#define GET_syssockntohl(fl)  CAL_CMGETAPI( "syssockntohl" ) 
	#define CAL_syssockntohl  syssockntohl
	#define CHK_syssockntohl  TRUE
	#define EXP_syssockntohl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockntohl", (RTS_UINTPTR)syssockntohl, 1, RTSITF_GET_SIGNATURE(0, 0x05B31DF1), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockntohl
	#define EXT_syssockntohl
	#define GET_syssockntohl(fl)  CAL_CMGETAPI( "syssockntohl" ) 
	#define CAL_syssockntohl  syssockntohl
	#define CHK_syssockntohl  TRUE
	#define EXP_syssockntohl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockntohl", (RTS_UINTPTR)syssockntohl, 1, RTSITF_GET_SIGNATURE(0, 0x05B31DF1), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockntohl
	#define EXT_SysSocketsyssockntohl
	#define GET_SysSocketsyssockntohl  ERR_OK
	#define CAL_SysSocketsyssockntohl  syssockntohl
	#define CHK_SysSocketsyssockntohl  TRUE
	#define EXP_SysSocketsyssockntohl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockntohl", (RTS_UINTPTR)syssockntohl, 1, RTSITF_GET_SIGNATURE(0, 0x05B31DF1), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockntohl
	#define EXT_syssockntohl
	#define GET_syssockntohl(fl)  CAL_CMGETAPI( "syssockntohl" ) 
	#define CAL_syssockntohl  syssockntohl
	#define CHK_syssockntohl  TRUE
	#define EXP_syssockntohl  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockntohl", (RTS_UINTPTR)syssockntohl, 1, RTSITF_GET_SIGNATURE(0, 0x05B31DF1), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockntohl  PFSYSSOCKNTOHL_IEC pfsyssockntohl;
	#define EXT_syssockntohl  extern PFSYSSOCKNTOHL_IEC pfsyssockntohl;
	#define GET_syssockntohl(fl)  s_pfCMGetAPI2( "syssockntohl", (RTS_VOID_FCTPTR *)&pfsyssockntohl, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x05B31DF1), 0x03050E00)
	#define CAL_syssockntohl  pfsyssockntohl
	#define CHK_syssockntohl  (pfsyssockntohl != NULL)
	#define EXP_syssockntohl   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockntohl", (RTS_UINTPTR)syssockntohl, 1, RTSITF_GET_SIGNATURE(0, 0x05B31DF1), 0x03050E00) 
#endif


/**
 * | Convert a WORD value from ethernet byte order into host format.
 * :return:Converted WORD value
 */
typedef struct tagsyssockntohs_struct
{
	RTS_IEC_WORD usNet;					/* VAR_INPUT */	/* Ethernet value */
	RTS_IEC_WORD SysSockNtohs;			/* VAR_OUTPUT */	
} syssockntohs_struct;

void CDECL CDECL_EXT syssockntohs(syssockntohs_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKNTOHS_IEC) (syssockntohs_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKNTOHS_NOTIMPLEMENTED)
	#define USE_syssockntohs
	#define EXT_syssockntohs
	#define GET_syssockntohs(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockntohs(p0) 
	#define CHK_syssockntohs  FALSE
	#define EXP_syssockntohs  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockntohs
	#define EXT_syssockntohs
	#define GET_syssockntohs(fl)  CAL_CMGETAPI( "syssockntohs" ) 
	#define CAL_syssockntohs  syssockntohs
	#define CHK_syssockntohs  TRUE
	#define EXP_syssockntohs  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockntohs", (RTS_UINTPTR)syssockntohs, 1, RTSITF_GET_SIGNATURE(0, 0x100E0417), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockntohs
	#define EXT_syssockntohs
	#define GET_syssockntohs(fl)  CAL_CMGETAPI( "syssockntohs" ) 
	#define CAL_syssockntohs  syssockntohs
	#define CHK_syssockntohs  TRUE
	#define EXP_syssockntohs  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockntohs", (RTS_UINTPTR)syssockntohs, 1, RTSITF_GET_SIGNATURE(0, 0x100E0417), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockntohs
	#define EXT_SysSocketsyssockntohs
	#define GET_SysSocketsyssockntohs  ERR_OK
	#define CAL_SysSocketsyssockntohs  syssockntohs
	#define CHK_SysSocketsyssockntohs  TRUE
	#define EXP_SysSocketsyssockntohs  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockntohs", (RTS_UINTPTR)syssockntohs, 1, RTSITF_GET_SIGNATURE(0, 0x100E0417), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockntohs
	#define EXT_syssockntohs
	#define GET_syssockntohs(fl)  CAL_CMGETAPI( "syssockntohs" ) 
	#define CAL_syssockntohs  syssockntohs
	#define CHK_syssockntohs  TRUE
	#define EXP_syssockntohs  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockntohs", (RTS_UINTPTR)syssockntohs, 1, RTSITF_GET_SIGNATURE(0, 0x100E0417), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockntohs  PFSYSSOCKNTOHS_IEC pfsyssockntohs;
	#define EXT_syssockntohs  extern PFSYSSOCKNTOHS_IEC pfsyssockntohs;
	#define GET_syssockntohs(fl)  s_pfCMGetAPI2( "syssockntohs", (RTS_VOID_FCTPTR *)&pfsyssockntohs, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x100E0417), 0x03050E00)
	#define CAL_syssockntohs  pfsyssockntohs
	#define CHK_syssockntohs  (pfsyssockntohs != NULL)
	#define EXP_syssockntohs   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockntohs", (RTS_UINTPTR)syssockntohs, 1, RTSITF_GET_SIGNATURE(0, 0x100E0417), 0x03050E00) 
#endif


/**
 * | Check the availability of the communication partner with a ping request.
 * :return: Runtime system error code (see CmpErrors.library): 
 *		| ERR_OK:            Partner available
 *		| ERR_FAILED:        Partner cannot be reached
 *		| All other results: Ping could not be sent because of other errors, so we don't know, if the partner is available.
 */
typedef struct tagsyssockping_struct
{
	RTS_IEC_STRING *szIPAddress;		/* VAR_INPUT */	/* IP address of the communication partner as string */
	RTS_IEC_UDINT ulTimeout;			/* VAR_INPUT */	/* Timeout in milliseconds to wait until reply */
	RTS_IEC_UDINT *pulReplyTime;		/* VAR_INPUT */	/* Pointer to get average reply time of the ping request in milliseconds */
	RTS_IEC_RESULT SysSockPing;			/* VAR_OUTPUT */	
} syssockping_struct;

void CDECL CDECL_EXT syssockping(syssockping_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKPING_IEC) (syssockping_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKPING_NOTIMPLEMENTED)
	#define USE_syssockping
	#define EXT_syssockping
	#define GET_syssockping(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockping(p0) 
	#define CHK_syssockping  FALSE
	#define EXP_syssockping  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockping
	#define EXT_syssockping
	#define GET_syssockping(fl)  CAL_CMGETAPI( "syssockping" ) 
	#define CAL_syssockping  syssockping
	#define CHK_syssockping  TRUE
	#define EXP_syssockping  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockping", (RTS_UINTPTR)syssockping, 1, RTSITF_GET_SIGNATURE(0, 0xB3610A39), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockping
	#define EXT_syssockping
	#define GET_syssockping(fl)  CAL_CMGETAPI( "syssockping" ) 
	#define CAL_syssockping  syssockping
	#define CHK_syssockping  TRUE
	#define EXP_syssockping  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockping", (RTS_UINTPTR)syssockping, 1, RTSITF_GET_SIGNATURE(0, 0xB3610A39), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockping
	#define EXT_SysSocketsyssockping
	#define GET_SysSocketsyssockping  ERR_OK
	#define CAL_SysSocketsyssockping  syssockping
	#define CHK_SysSocketsyssockping  TRUE
	#define EXP_SysSocketsyssockping  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockping", (RTS_UINTPTR)syssockping, 1, RTSITF_GET_SIGNATURE(0, 0xB3610A39), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockping
	#define EXT_syssockping
	#define GET_syssockping(fl)  CAL_CMGETAPI( "syssockping" ) 
	#define CAL_syssockping  syssockping
	#define CHK_syssockping  TRUE
	#define EXP_syssockping  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockping", (RTS_UINTPTR)syssockping, 1, RTSITF_GET_SIGNATURE(0, 0xB3610A39), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockping  PFSYSSOCKPING_IEC pfsyssockping;
	#define EXT_syssockping  extern PFSYSSOCKPING_IEC pfsyssockping;
	#define GET_syssockping(fl)  s_pfCMGetAPI2( "syssockping", (RTS_VOID_FCTPTR *)&pfsyssockping, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB3610A39), 0x03050E00)
	#define CAL_syssockping  pfsyssockping
	#define CHK_syssockping  (pfsyssockping != NULL)
	#define EXP_syssockping   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockping", (RTS_UINTPTR)syssockping, 1, RTSITF_GET_SIGNATURE(0, 0xB3610A39), 0x03050E00) 
#endif


/**
 * | Receive data from a TCP socket.
 * :return: Number of bytes received. 0 if failed.
 */
typedef struct tagsyssockrecv_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Buffer to read data from the socket */
	RTS_IEC_XINT diBufferSize;			/* VAR_INPUT */	/* Maximum length of the buffer */
	RTS_IEC_DINT diFlags;				/* VAR_INPUT */	/* | The flags parameter can be used to influence the behavior of the 
	  function beyond the options specified for the associated socket. The semantics of this function 
	  are determined by the socket options and the flags parameter. The latter is constructed by using the
	  bitwise OR operator with any of the SOCKET_MSG values. See category TCP flags. */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSockRecv;			/* VAR_OUTPUT */	
} syssockrecv_struct;

void CDECL CDECL_EXT syssockrecv(syssockrecv_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKRECV_IEC) (syssockrecv_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKRECV_NOTIMPLEMENTED)
	#define USE_syssockrecv
	#define EXT_syssockrecv
	#define GET_syssockrecv(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockrecv(p0) 
	#define CHK_syssockrecv  FALSE
	#define EXP_syssockrecv  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockrecv
	#define EXT_syssockrecv
	#define GET_syssockrecv(fl)  CAL_CMGETAPI( "syssockrecv" ) 
	#define CAL_syssockrecv  syssockrecv
	#define CHK_syssockrecv  TRUE
	#define EXP_syssockrecv  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecv", (RTS_UINTPTR)syssockrecv, 1, RTSITF_GET_SIGNATURE(0, 0x887FBA6B), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockrecv
	#define EXT_syssockrecv
	#define GET_syssockrecv(fl)  CAL_CMGETAPI( "syssockrecv" ) 
	#define CAL_syssockrecv  syssockrecv
	#define CHK_syssockrecv  TRUE
	#define EXP_syssockrecv  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecv", (RTS_UINTPTR)syssockrecv, 1, RTSITF_GET_SIGNATURE(0, 0x887FBA6B), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockrecv
	#define EXT_SysSocketsyssockrecv
	#define GET_SysSocketsyssockrecv  ERR_OK
	#define CAL_SysSocketsyssockrecv  syssockrecv
	#define CHK_SysSocketsyssockrecv  TRUE
	#define EXP_SysSocketsyssockrecv  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecv", (RTS_UINTPTR)syssockrecv, 1, RTSITF_GET_SIGNATURE(0, 0x887FBA6B), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockrecv
	#define EXT_syssockrecv
	#define GET_syssockrecv(fl)  CAL_CMGETAPI( "syssockrecv" ) 
	#define CAL_syssockrecv  syssockrecv
	#define CHK_syssockrecv  TRUE
	#define EXP_syssockrecv  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecv", (RTS_UINTPTR)syssockrecv, 1, RTSITF_GET_SIGNATURE(0, 0x887FBA6B), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockrecv  PFSYSSOCKRECV_IEC pfsyssockrecv;
	#define EXT_syssockrecv  extern PFSYSSOCKRECV_IEC pfsyssockrecv;
	#define GET_syssockrecv(fl)  s_pfCMGetAPI2( "syssockrecv", (RTS_VOID_FCTPTR *)&pfsyssockrecv, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x887FBA6B), 0x03050E00)
	#define CAL_syssockrecv  pfsyssockrecv
	#define CHK_syssockrecv  (pfsyssockrecv != NULL)
	#define EXP_syssockrecv   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecv", (RTS_UINTPTR)syssockrecv, 1, RTSITF_GET_SIGNATURE(0, 0x887FBA6B), 0x03050E00) 
#endif


/**
 * | Receive a message from a connectionless socket (UDP).
 * :return: Number of bytes received
 */
typedef struct tagsyssockrecvfrom_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Buffer to read data from the socket */
	RTS_IEC_XINT diBufferSize;			/* VAR_INPUT */	/* Maximum length of the buffer */
	RTS_IEC_DINT diFlags;				/* VAR_INPUT */	/* | The flags parameter can be used to influence the behavior of the 
	  function beyond the options specified for the associated socket. The semantics of this function 
	  are determined by the socket options and the flags parameter. The latter is constructed by using the
	  bitwise OR operator with any of the SOCKET_MSG values. */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address and port to receive data from */
	RTS_IEC_DINT diSockAddrSize;		/* VAR_INPUT */	/* Size of socket address structure */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSockRecvFrom;		/* VAR_OUTPUT */	
} syssockrecvfrom_struct;

void CDECL CDECL_EXT syssockrecvfrom(syssockrecvfrom_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKRECVFROM_IEC) (syssockrecvfrom_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKRECVFROM_NOTIMPLEMENTED)
	#define USE_syssockrecvfrom
	#define EXT_syssockrecvfrom
	#define GET_syssockrecvfrom(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockrecvfrom(p0) 
	#define CHK_syssockrecvfrom  FALSE
	#define EXP_syssockrecvfrom  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockrecvfrom
	#define EXT_syssockrecvfrom
	#define GET_syssockrecvfrom(fl)  CAL_CMGETAPI( "syssockrecvfrom" ) 
	#define CAL_syssockrecvfrom  syssockrecvfrom
	#define CHK_syssockrecvfrom  TRUE
	#define EXP_syssockrecvfrom  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfrom", (RTS_UINTPTR)syssockrecvfrom, 1, RTSITF_GET_SIGNATURE(0, 0x26DF0362), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockrecvfrom
	#define EXT_syssockrecvfrom
	#define GET_syssockrecvfrom(fl)  CAL_CMGETAPI( "syssockrecvfrom" ) 
	#define CAL_syssockrecvfrom  syssockrecvfrom
	#define CHK_syssockrecvfrom  TRUE
	#define EXP_syssockrecvfrom  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfrom", (RTS_UINTPTR)syssockrecvfrom, 1, RTSITF_GET_SIGNATURE(0, 0x26DF0362), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockrecvfrom
	#define EXT_SysSocketsyssockrecvfrom
	#define GET_SysSocketsyssockrecvfrom  ERR_OK
	#define CAL_SysSocketsyssockrecvfrom  syssockrecvfrom
	#define CHK_SysSocketsyssockrecvfrom  TRUE
	#define EXP_SysSocketsyssockrecvfrom  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfrom", (RTS_UINTPTR)syssockrecvfrom, 1, RTSITF_GET_SIGNATURE(0, 0x26DF0362), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockrecvfrom
	#define EXT_syssockrecvfrom
	#define GET_syssockrecvfrom(fl)  CAL_CMGETAPI( "syssockrecvfrom" ) 
	#define CAL_syssockrecvfrom  syssockrecvfrom
	#define CHK_syssockrecvfrom  TRUE
	#define EXP_syssockrecvfrom  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfrom", (RTS_UINTPTR)syssockrecvfrom, 1, RTSITF_GET_SIGNATURE(0, 0x26DF0362), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockrecvfrom  PFSYSSOCKRECVFROM_IEC pfsyssockrecvfrom;
	#define EXT_syssockrecvfrom  extern PFSYSSOCKRECVFROM_IEC pfsyssockrecvfrom;
	#define GET_syssockrecvfrom(fl)  s_pfCMGetAPI2( "syssockrecvfrom", (RTS_VOID_FCTPTR *)&pfsyssockrecvfrom, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x26DF0362), 0x03050E00)
	#define CAL_syssockrecvfrom  pfsyssockrecvfrom
	#define CHK_syssockrecvfrom  (pfsyssockrecvfrom != NULL)
	#define EXP_syssockrecvfrom   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfrom", (RTS_UINTPTR)syssockrecvfrom, 1, RTSITF_GET_SIGNATURE(0, 0x26DF0362), 0x03050E00) 
#endif


/**
 * | Receive a paket from a UDP socket.
 * :return: Number of bytes received
 */
typedef struct tagsyssockrecvfromudp_struct
{
	RTS_IEC_HANDLE hSocketUdp;			/* VAR_INPUT */	/* Handle to the UDP socket */
	RTS_IEC_BYTE *pbyData;				/* VAR_INPUT */	/* Pointer to data to receive */
	RTS_IEC_XINT diDataSize;			/* VAR_INPUT */	/* Size of data to receive */
	UDP_REPLY *pReply;					/* VAR_INPUT */	/* Description of the client that has sent this paket */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSockRecvFromUdp;	/* VAR_OUTPUT */	
} syssockrecvfromudp_struct;

void CDECL CDECL_EXT syssockrecvfromudp(syssockrecvfromudp_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKRECVFROMUDP_IEC) (syssockrecvfromudp_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKRECVFROMUDP_NOTIMPLEMENTED)
	#define USE_syssockrecvfromudp
	#define EXT_syssockrecvfromudp
	#define GET_syssockrecvfromudp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockrecvfromudp(p0) 
	#define CHK_syssockrecvfromudp  FALSE
	#define EXP_syssockrecvfromudp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockrecvfromudp
	#define EXT_syssockrecvfromudp
	#define GET_syssockrecvfromudp(fl)  CAL_CMGETAPI( "syssockrecvfromudp" ) 
	#define CAL_syssockrecvfromudp  syssockrecvfromudp
	#define CHK_syssockrecvfromudp  TRUE
	#define EXP_syssockrecvfromudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfromudp", (RTS_UINTPTR)syssockrecvfromudp, 1, RTSITF_GET_SIGNATURE(0, 0x86A63EF8), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockrecvfromudp
	#define EXT_syssockrecvfromudp
	#define GET_syssockrecvfromudp(fl)  CAL_CMGETAPI( "syssockrecvfromudp" ) 
	#define CAL_syssockrecvfromudp  syssockrecvfromudp
	#define CHK_syssockrecvfromudp  TRUE
	#define EXP_syssockrecvfromudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfromudp", (RTS_UINTPTR)syssockrecvfromudp, 1, RTSITF_GET_SIGNATURE(0, 0x86A63EF8), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockrecvfromudp
	#define EXT_SysSocketsyssockrecvfromudp
	#define GET_SysSocketsyssockrecvfromudp  ERR_OK
	#define CAL_SysSocketsyssockrecvfromudp  syssockrecvfromudp
	#define CHK_SysSocketsyssockrecvfromudp  TRUE
	#define EXP_SysSocketsyssockrecvfromudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfromudp", (RTS_UINTPTR)syssockrecvfromudp, 1, RTSITF_GET_SIGNATURE(0, 0x86A63EF8), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockrecvfromudp
	#define EXT_syssockrecvfromudp
	#define GET_syssockrecvfromudp(fl)  CAL_CMGETAPI( "syssockrecvfromudp" ) 
	#define CAL_syssockrecvfromudp  syssockrecvfromudp
	#define CHK_syssockrecvfromudp  TRUE
	#define EXP_syssockrecvfromudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfromudp", (RTS_UINTPTR)syssockrecvfromudp, 1, RTSITF_GET_SIGNATURE(0, 0x86A63EF8), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockrecvfromudp  PFSYSSOCKRECVFROMUDP_IEC pfsyssockrecvfromudp;
	#define EXT_syssockrecvfromudp  extern PFSYSSOCKRECVFROMUDP_IEC pfsyssockrecvfromudp;
	#define GET_syssockrecvfromudp(fl)  s_pfCMGetAPI2( "syssockrecvfromudp", (RTS_VOID_FCTPTR *)&pfsyssockrecvfromudp, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x86A63EF8), 0x03050E00)
	#define CAL_syssockrecvfromudp  pfsyssockrecvfromudp
	#define CHK_syssockrecvfromudp  (pfsyssockrecvfromudp != NULL)
	#define EXP_syssockrecvfromudp   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfromudp", (RTS_UINTPTR)syssockrecvfromudp, 1, RTSITF_GET_SIGNATURE(0, 0x86A63EF8), 0x03050E00) 
#endif


/**
 * | Receive a paket from a UDP socket.
 * :return: Number of bytes received
 */
typedef struct tagsyssockrecvfromudp2_struct
{
	RTS_IEC_HANDLE hSocketUdp;			/* VAR_INPUT */	/* Handle to the UDP socket */
	RTS_IEC_BYTE *pbyData;				/* VAR_INPUT */	/* Pointer to data to receive */
	RTS_IEC_XINT diDataSize;			/* VAR_INPUT */	/* Size of data to receive */
	UDP_REPLY2 *pReply;					/* VAR_INPUT */	/* Description of the client that has sent this paket */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSockRecvFromUdp2;	/* VAR_OUTPUT */	
} syssockrecvfromudp2_struct;

void CDECL CDECL_EXT syssockrecvfromudp2(syssockrecvfromudp2_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKRECVFROMUDP2_IEC) (syssockrecvfromudp2_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKRECVFROMUDP2_NOTIMPLEMENTED)
	#define USE_syssockrecvfromudp2
	#define EXT_syssockrecvfromudp2
	#define GET_syssockrecvfromudp2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockrecvfromudp2(p0) 
	#define CHK_syssockrecvfromudp2  FALSE
	#define EXP_syssockrecvfromudp2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockrecvfromudp2
	#define EXT_syssockrecvfromudp2
	#define GET_syssockrecvfromudp2(fl)  CAL_CMGETAPI( "syssockrecvfromudp2" ) 
	#define CAL_syssockrecvfromudp2  syssockrecvfromudp2
	#define CHK_syssockrecvfromudp2  TRUE
	#define EXP_syssockrecvfromudp2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfromudp2", (RTS_UINTPTR)syssockrecvfromudp2, 1, RTSITF_GET_SIGNATURE(0, 0xBA3470D3), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockrecvfromudp2
	#define EXT_syssockrecvfromudp2
	#define GET_syssockrecvfromudp2(fl)  CAL_CMGETAPI( "syssockrecvfromudp2" ) 
	#define CAL_syssockrecvfromudp2  syssockrecvfromudp2
	#define CHK_syssockrecvfromudp2  TRUE
	#define EXP_syssockrecvfromudp2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfromudp2", (RTS_UINTPTR)syssockrecvfromudp2, 1, RTSITF_GET_SIGNATURE(0, 0xBA3470D3), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockrecvfromudp2
	#define EXT_SysSocketsyssockrecvfromudp2
	#define GET_SysSocketsyssockrecvfromudp2  ERR_OK
	#define CAL_SysSocketsyssockrecvfromudp2  syssockrecvfromudp2
	#define CHK_SysSocketsyssockrecvfromudp2  TRUE
	#define EXP_SysSocketsyssockrecvfromudp2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfromudp2", (RTS_UINTPTR)syssockrecvfromudp2, 1, RTSITF_GET_SIGNATURE(0, 0xBA3470D3), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockrecvfromudp2
	#define EXT_syssockrecvfromudp2
	#define GET_syssockrecvfromudp2(fl)  CAL_CMGETAPI( "syssockrecvfromudp2" ) 
	#define CAL_syssockrecvfromudp2  syssockrecvfromudp2
	#define CHK_syssockrecvfromudp2  TRUE
	#define EXP_syssockrecvfromudp2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfromudp2", (RTS_UINTPTR)syssockrecvfromudp2, 1, RTSITF_GET_SIGNATURE(0, 0xBA3470D3), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockrecvfromudp2  PFSYSSOCKRECVFROMUDP2_IEC pfsyssockrecvfromudp2;
	#define EXT_syssockrecvfromudp2  extern PFSYSSOCKRECVFROMUDP2_IEC pfsyssockrecvfromudp2;
	#define GET_syssockrecvfromudp2(fl)  s_pfCMGetAPI2( "syssockrecvfromudp2", (RTS_VOID_FCTPTR *)&pfsyssockrecvfromudp2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xBA3470D3), 0x03050E00)
	#define CAL_syssockrecvfromudp2  pfsyssockrecvfromudp2
	#define CHK_syssockrecvfromudp2  (pfsyssockrecvfromudp2 != NULL)
	#define EXP_syssockrecvfromudp2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockrecvfromudp2", (RTS_UINTPTR)syssockrecvfromudp2, 1, RTSITF_GET_SIGNATURE(0, 0xBA3470D3), 0x03050E00) 
#endif


/**
 * | Check a number of sockets for activity.
 * :return: Runtime system error code (see CmpErrors.library):
 *		| ERR_OK
 *		| ERR_SOCK_TIMEDOUT, if timeout expired
 */
typedef struct tagsyssockselect_struct
{
	RTS_IEC_DINT diWidth;				/* VAR_INPUT */	/* Number of sockets in the |SOCKET_FD_SET| structure, so |SOCKET_FD_SETSIZE| must be used here. */
	SOCKET_FD_SET *pfdRead;				/* VAR_INPUT */	/* Optional pointer to the structure |SOCKET_FD_SET| defining the socket set
 to be checked for reading. It is also possible to set this parameter to 0. */
	SOCKET_FD_SET *pfdWrite;			/* VAR_INPUT */	/* Optional pointer to the structure |SOCKET_FD_SET| defining the socket set
 to be checked for writing. It is also possible to set this parameter to 0. */
	SOCKET_FD_SET *pfdExcept;			/* VAR_INPUT */	/* Optional pointer to structure |SOCKET_FD_SET| that defines the socket set
 the error state has to be checked. It is also possible to set this parameter to 0. */
	SOCKET_TIMEVAL *ptvTimeout;			/* VAR_INPUT */	/* Pointer to maximum timespan which the function SysSockSelect waits for a response:
 	| ptvTimeout=NULL:	Infinite wait
 	| ptvTimeout->tv_sec=-1, ptvTimeout->tv_usec=-1:	Infinite wait
 	| ptvTimeout->tv_sec=0, ptvTimeout->tv_usec=0:	No wait */
	RTS_IEC_DINT *pdiReady;				/* VAR_INPUT */	/* Number of sockets that are ready for IO */
	RTS_IEC_RESULT SysSockSelect;		/* VAR_OUTPUT */	
} syssockselect_struct;

void CDECL CDECL_EXT syssockselect(syssockselect_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKSELECT_IEC) (syssockselect_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSELECT_NOTIMPLEMENTED)
	#define USE_syssockselect
	#define EXT_syssockselect
	#define GET_syssockselect(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockselect(p0) 
	#define CHK_syssockselect  FALSE
	#define EXP_syssockselect  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockselect
	#define EXT_syssockselect
	#define GET_syssockselect(fl)  CAL_CMGETAPI( "syssockselect" ) 
	#define CAL_syssockselect  syssockselect
	#define CHK_syssockselect  TRUE
	#define EXP_syssockselect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockselect", (RTS_UINTPTR)syssockselect, 1, RTSITF_GET_SIGNATURE(0, 0x59125CA7), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockselect
	#define EXT_syssockselect
	#define GET_syssockselect(fl)  CAL_CMGETAPI( "syssockselect" ) 
	#define CAL_syssockselect  syssockselect
	#define CHK_syssockselect  TRUE
	#define EXP_syssockselect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockselect", (RTS_UINTPTR)syssockselect, 1, RTSITF_GET_SIGNATURE(0, 0x59125CA7), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockselect
	#define EXT_SysSocketsyssockselect
	#define GET_SysSocketsyssockselect  ERR_OK
	#define CAL_SysSocketsyssockselect  syssockselect
	#define CHK_SysSocketsyssockselect  TRUE
	#define EXP_SysSocketsyssockselect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockselect", (RTS_UINTPTR)syssockselect, 1, RTSITF_GET_SIGNATURE(0, 0x59125CA7), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockselect
	#define EXT_syssockselect
	#define GET_syssockselect(fl)  CAL_CMGETAPI( "syssockselect" ) 
	#define CAL_syssockselect  syssockselect
	#define CHK_syssockselect  TRUE
	#define EXP_syssockselect  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockselect", (RTS_UINTPTR)syssockselect, 1, RTSITF_GET_SIGNATURE(0, 0x59125CA7), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockselect  PFSYSSOCKSELECT_IEC pfsyssockselect;
	#define EXT_syssockselect  extern PFSYSSOCKSELECT_IEC pfsyssockselect;
	#define GET_syssockselect(fl)  s_pfCMGetAPI2( "syssockselect", (RTS_VOID_FCTPTR *)&pfsyssockselect, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x59125CA7), 0x03050E00)
	#define CAL_syssockselect  pfsyssockselect
	#define CHK_syssockselect  (pfsyssockselect != NULL)
	#define EXP_syssockselect   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockselect", (RTS_UINTPTR)syssockselect, 1, RTSITF_GET_SIGNATURE(0, 0x59125CA7), 0x03050E00) 
#endif


/**
 * | Sent data to a TCP socket.
 * :return: Number of sent bytes. 0 if failed.
 */
typedef struct tagsyssocksend_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Buffer with data to sent */
	RTS_IEC_XINT diBufferSize;			/* VAR_INPUT */	/* Maximum length of the buffer */
	RTS_IEC_DINT diFlags;				/* VAR_INPUT */	/* | The flags parameter can be used to influence the behavior of the 
	  function beyond the options specified for the associated socket. The semantics of this function 
	  are determined by the socket options and the flags parameter. The latter is constructed by using the
	  bitwise OR operator with any of the SOCKET_MSG values. */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSockSend;			/* VAR_OUTPUT */	
} syssocksend_struct;

void CDECL CDECL_EXT syssocksend(syssocksend_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKSEND_IEC) (syssocksend_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSEND_NOTIMPLEMENTED)
	#define USE_syssocksend
	#define EXT_syssocksend
	#define GET_syssocksend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssocksend(p0) 
	#define CHK_syssocksend  FALSE
	#define EXP_syssocksend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssocksend
	#define EXT_syssocksend
	#define GET_syssocksend(fl)  CAL_CMGETAPI( "syssocksend" ) 
	#define CAL_syssocksend  syssocksend
	#define CHK_syssocksend  TRUE
	#define EXP_syssocksend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksend", (RTS_UINTPTR)syssocksend, 1, RTSITF_GET_SIGNATURE(0, 0xA73C5F51), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssocksend
	#define EXT_syssocksend
	#define GET_syssocksend(fl)  CAL_CMGETAPI( "syssocksend" ) 
	#define CAL_syssocksend  syssocksend
	#define CHK_syssocksend  TRUE
	#define EXP_syssocksend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksend", (RTS_UINTPTR)syssocksend, 1, RTSITF_GET_SIGNATURE(0, 0xA73C5F51), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssocksend
	#define EXT_SysSocketsyssocksend
	#define GET_SysSocketsyssocksend  ERR_OK
	#define CAL_SysSocketsyssocksend  syssocksend
	#define CHK_SysSocketsyssocksend  TRUE
	#define EXP_SysSocketsyssocksend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksend", (RTS_UINTPTR)syssocksend, 1, RTSITF_GET_SIGNATURE(0, 0xA73C5F51), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssocksend
	#define EXT_syssocksend
	#define GET_syssocksend(fl)  CAL_CMGETAPI( "syssocksend" ) 
	#define CAL_syssocksend  syssocksend
	#define CHK_syssocksend  TRUE
	#define EXP_syssocksend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksend", (RTS_UINTPTR)syssocksend, 1, RTSITF_GET_SIGNATURE(0, 0xA73C5F51), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssocksend  PFSYSSOCKSEND_IEC pfsyssocksend;
	#define EXT_syssocksend  extern PFSYSSOCKSEND_IEC pfsyssocksend;
	#define GET_syssocksend(fl)  s_pfCMGetAPI2( "syssocksend", (RTS_VOID_FCTPTR *)&pfsyssocksend, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xA73C5F51), 0x03050E00)
	#define CAL_syssocksend  pfsyssocksend
	#define CHK_syssocksend  (pfsyssocksend != NULL)
	#define EXP_syssocksend   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksend", (RTS_UINTPTR)syssocksend, 1, RTSITF_GET_SIGNATURE(0, 0xA73C5F51), 0x03050E00) 
#endif


/**
 * | Send a message over a connectionless socket (UDP).
 * :return: Number of bytes received.
 */
typedef struct tagsyssocksendto_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Buffer with send data */
	RTS_IEC_XINT diBufferSize;			/* VAR_INPUT */	/* Length of data to send. If diBufferSize = 0 ERR_PARAMETER is returned */
	RTS_IEC_DINT diFlags;				/* VAR_INPUT */	/* | The flags parameter can be used to influence the behavior of the 
	  function beyond the options specified for the associated socket. The semantics of this function 
	  are determined by the socket options and the flags parameter. The latter is constructed by using the
	  bitwise OR operator with any of the SOCKET_MSG values. */
	SOCKADDRESS *pSockAddr;				/* VAR_INPUT */	/* Socket address and port to sent data to */
	RTS_IEC_DINT diSockAddrSize;		/* VAR_INPUT */	/* Size of socket address structure */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSockSendTo;			/* VAR_OUTPUT */	
} syssocksendto_struct;

void CDECL CDECL_EXT syssocksendto(syssocksendto_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKSENDTO_IEC) (syssocksendto_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSENDTO_NOTIMPLEMENTED)
	#define USE_syssocksendto
	#define EXT_syssocksendto
	#define GET_syssocksendto(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssocksendto(p0) 
	#define CHK_syssocksendto  FALSE
	#define EXP_syssocksendto  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssocksendto
	#define EXT_syssocksendto
	#define GET_syssocksendto(fl)  CAL_CMGETAPI( "syssocksendto" ) 
	#define CAL_syssocksendto  syssocksendto
	#define CHK_syssocksendto  TRUE
	#define EXP_syssocksendto  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksendto", (RTS_UINTPTR)syssocksendto, 1, RTSITF_GET_SIGNATURE(0, 0xEE37CAFA), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssocksendto
	#define EXT_syssocksendto
	#define GET_syssocksendto(fl)  CAL_CMGETAPI( "syssocksendto" ) 
	#define CAL_syssocksendto  syssocksendto
	#define CHK_syssocksendto  TRUE
	#define EXP_syssocksendto  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksendto", (RTS_UINTPTR)syssocksendto, 1, RTSITF_GET_SIGNATURE(0, 0xEE37CAFA), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssocksendto
	#define EXT_SysSocketsyssocksendto
	#define GET_SysSocketsyssocksendto  ERR_OK
	#define CAL_SysSocketsyssocksendto  syssocksendto
	#define CHK_SysSocketsyssocksendto  TRUE
	#define EXP_SysSocketsyssocksendto  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksendto", (RTS_UINTPTR)syssocksendto, 1, RTSITF_GET_SIGNATURE(0, 0xEE37CAFA), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssocksendto
	#define EXT_syssocksendto
	#define GET_syssocksendto(fl)  CAL_CMGETAPI( "syssocksendto" ) 
	#define CAL_syssocksendto  syssocksendto
	#define CHK_syssocksendto  TRUE
	#define EXP_syssocksendto  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksendto", (RTS_UINTPTR)syssocksendto, 1, RTSITF_GET_SIGNATURE(0, 0xEE37CAFA), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssocksendto  PFSYSSOCKSENDTO_IEC pfsyssocksendto;
	#define EXT_syssocksendto  extern PFSYSSOCKSENDTO_IEC pfsyssocksendto;
	#define GET_syssocksendto(fl)  s_pfCMGetAPI2( "syssocksendto", (RTS_VOID_FCTPTR *)&pfsyssocksendto, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xEE37CAFA), 0x03050E00)
	#define CAL_syssocksendto  pfsyssocksendto
	#define CHK_syssocksendto  (pfsyssocksendto != NULL)
	#define EXP_syssocksendto   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksendto", (RTS_UINTPTR)syssocksendto, 1, RTSITF_GET_SIGNATURE(0, 0xEE37CAFA), 0x03050E00) 
#endif


/**
 * | Send a paket to a UDP socket.
 * :return: Number of bytes sent
 */
typedef struct tagsyssocksendtoudp_struct
{
	RTS_IEC_HANDLE hSocketUdp;			/* VAR_INPUT */	/* Handle to the UDP socket */
	RTS_IEC_DINT diPort;				/* VAR_INPUT */	/* Port number ot send data to */
	RTS_IEC_STRING *szDestAddress;		/* VAR_INPUT */	/* Destination IP address ot send data to */
	RTS_IEC_BYTE *pbyData;				/* VAR_INPUT */	/* Pointer to data to send */
	RTS_IEC_XINT diDataSize;			/* VAR_INPUT */	/* Size of data to send. If diDataSize = 0 ERR_PARAMETER is returned. */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XINT SysSockSendToUdp;		/* VAR_OUTPUT */	
} syssocksendtoudp_struct;

void CDECL CDECL_EXT syssocksendtoudp(syssocksendtoudp_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKSENDTOUDP_IEC) (syssocksendtoudp_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSENDTOUDP_NOTIMPLEMENTED)
	#define USE_syssocksendtoudp
	#define EXT_syssocksendtoudp
	#define GET_syssocksendtoudp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssocksendtoudp(p0) 
	#define CHK_syssocksendtoudp  FALSE
	#define EXP_syssocksendtoudp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssocksendtoudp
	#define EXT_syssocksendtoudp
	#define GET_syssocksendtoudp(fl)  CAL_CMGETAPI( "syssocksendtoudp" ) 
	#define CAL_syssocksendtoudp  syssocksendtoudp
	#define CHK_syssocksendtoudp  TRUE
	#define EXP_syssocksendtoudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksendtoudp", (RTS_UINTPTR)syssocksendtoudp, 1, RTSITF_GET_SIGNATURE(0, 0x81DCE32E), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssocksendtoudp
	#define EXT_syssocksendtoudp
	#define GET_syssocksendtoudp(fl)  CAL_CMGETAPI( "syssocksendtoudp" ) 
	#define CAL_syssocksendtoudp  syssocksendtoudp
	#define CHK_syssocksendtoudp  TRUE
	#define EXP_syssocksendtoudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksendtoudp", (RTS_UINTPTR)syssocksendtoudp, 1, RTSITF_GET_SIGNATURE(0, 0x81DCE32E), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssocksendtoudp
	#define EXT_SysSocketsyssocksendtoudp
	#define GET_SysSocketsyssocksendtoudp  ERR_OK
	#define CAL_SysSocketsyssocksendtoudp  syssocksendtoudp
	#define CHK_SysSocketsyssocksendtoudp  TRUE
	#define EXP_SysSocketsyssocksendtoudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksendtoudp", (RTS_UINTPTR)syssocksendtoudp, 1, RTSITF_GET_SIGNATURE(0, 0x81DCE32E), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssocksendtoudp
	#define EXT_syssocksendtoudp
	#define GET_syssocksendtoudp(fl)  CAL_CMGETAPI( "syssocksendtoudp" ) 
	#define CAL_syssocksendtoudp  syssocksendtoudp
	#define CHK_syssocksendtoudp  TRUE
	#define EXP_syssocksendtoudp  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksendtoudp", (RTS_UINTPTR)syssocksendtoudp, 1, RTSITF_GET_SIGNATURE(0, 0x81DCE32E), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssocksendtoudp  PFSYSSOCKSENDTOUDP_IEC pfsyssocksendtoudp;
	#define EXT_syssocksendtoudp  extern PFSYSSOCKSENDTOUDP_IEC pfsyssocksendtoudp;
	#define GET_syssocksendtoudp(fl)  s_pfCMGetAPI2( "syssocksendtoudp", (RTS_VOID_FCTPTR *)&pfsyssocksendtoudp, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x81DCE32E), 0x03050E00)
	#define CAL_syssocksendtoudp  pfsyssocksendtoudp
	#define CHK_syssocksendtoudp  (pfsyssocksendtoudp != NULL)
	#define EXP_syssocksendtoudp   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksendtoudp", (RTS_UINTPTR)syssocksendtoudp, 1, RTSITF_GET_SIGNATURE(0, 0x81DCE32E), 0x03050E00) 
#endif


/**
 * Set default gateway address an adapter. 
 * It depends on the device, whether the new gateway address is reset during reboot or
 * if it is retained. In general the caller should consider this as volatile. The 
 * gateway address = 0.0.0.0 can be used to remove the gateway address from the adapter. 
 * After this there is not routing to other subnets possible.
 * 
 * .. note::
 *     The default gateway can only be changed if the adapter containing
 *     the current default gateway is whitelisted in the CODESYSControl.cfg. This function
 *     will not change the default gateway of any other adapter.
 *
 * :return: Returns the runtime system error code (see CmpErrors.library):
 *     - ERR_PARAMETER: At least one of the passed pointers is NULL.
 *	    - ERR_OPERATION_DENIED: Adapter is not in white list or the operation was denied by the event EVT_SysSocket_BeforeSetGateway.
 *	    - ERR_NO_OBJECT: Adapter with the specified name does not exist.
 *	    - ERR_NO_CHANGE: There is a default gateway set on an adapter that is not whitelisted.
 *	    - ERR_FAILED: Default gateway could not be set.
 *	    - ERR_OK: Default gateway was set successfully.
 */
typedef struct tagsyssocksetdefaultgateway_struct
{
	RTS_IEC_WSTRING *wsAdapterName;		/* VAR_INPUT */	/* Adapter name provided by SysSockGetFirstAdapter() / SysSockGetNextAdapter() */
	INADDR *GatewayAddr;				/* VAR_INPUT */	/* Ip address of the gateway to set in network byte order. */
	RTS_IEC_RESULT SysSockSetDefaultGateway;	/* VAR_OUTPUT */	
} syssocksetdefaultgateway_struct;

void CDECL CDECL_EXT syssocksetdefaultgateway(syssocksetdefaultgateway_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKSETDEFAULTGATEWAY_IEC) (syssocksetdefaultgateway_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSETDEFAULTGATEWAY_NOTIMPLEMENTED)
	#define USE_syssocksetdefaultgateway
	#define EXT_syssocksetdefaultgateway
	#define GET_syssocksetdefaultgateway(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssocksetdefaultgateway(p0) 
	#define CHK_syssocksetdefaultgateway  FALSE
	#define EXP_syssocksetdefaultgateway  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssocksetdefaultgateway
	#define EXT_syssocksetdefaultgateway
	#define GET_syssocksetdefaultgateway(fl)  CAL_CMGETAPI( "syssocksetdefaultgateway" ) 
	#define CAL_syssocksetdefaultgateway  syssocksetdefaultgateway
	#define CHK_syssocksetdefaultgateway  TRUE
	#define EXP_syssocksetdefaultgateway  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetdefaultgateway", (RTS_UINTPTR)syssocksetdefaultgateway, 1, 0xB87B1303, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssocksetdefaultgateway
	#define EXT_syssocksetdefaultgateway
	#define GET_syssocksetdefaultgateway(fl)  CAL_CMGETAPI( "syssocksetdefaultgateway" ) 
	#define CAL_syssocksetdefaultgateway  syssocksetdefaultgateway
	#define CHK_syssocksetdefaultgateway  TRUE
	#define EXP_syssocksetdefaultgateway  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetdefaultgateway", (RTS_UINTPTR)syssocksetdefaultgateway, 1, 0xB87B1303, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssocksetdefaultgateway
	#define EXT_SysSocketsyssocksetdefaultgateway
	#define GET_SysSocketsyssocksetdefaultgateway  ERR_OK
	#define CAL_SysSocketsyssocksetdefaultgateway  syssocksetdefaultgateway
	#define CHK_SysSocketsyssocksetdefaultgateway  TRUE
	#define EXP_SysSocketsyssocksetdefaultgateway  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetdefaultgateway", (RTS_UINTPTR)syssocksetdefaultgateway, 1, 0xB87B1303, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssocksetdefaultgateway
	#define EXT_syssocksetdefaultgateway
	#define GET_syssocksetdefaultgateway(fl)  CAL_CMGETAPI( "syssocksetdefaultgateway" ) 
	#define CAL_syssocksetdefaultgateway  syssocksetdefaultgateway
	#define CHK_syssocksetdefaultgateway  TRUE
	#define EXP_syssocksetdefaultgateway  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetdefaultgateway", (RTS_UINTPTR)syssocksetdefaultgateway, 1, 0xB87B1303, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssocksetdefaultgateway  PFSYSSOCKSETDEFAULTGATEWAY_IEC pfsyssocksetdefaultgateway;
	#define EXT_syssocksetdefaultgateway  extern PFSYSSOCKSETDEFAULTGATEWAY_IEC pfsyssocksetdefaultgateway;
	#define GET_syssocksetdefaultgateway(fl)  s_pfCMGetAPI2( "syssocksetdefaultgateway", (RTS_VOID_FCTPTR *)&pfsyssocksetdefaultgateway, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xB87B1303, 0x03050E00)
	#define CAL_syssocksetdefaultgateway  pfsyssocksetdefaultgateway
	#define CHK_syssocksetdefaultgateway  (pfsyssocksetdefaultgateway != NULL)
	#define EXP_syssocksetdefaultgateway   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetdefaultgateway", (RTS_UINTPTR)syssocksetdefaultgateway, 1, 0xB87B1303, 0x03050E00) 
#endif


/**
 * | Set IP address of the specified ethernet device. Is not available on all platforms! 
 * | Use SysSockSetIpAddressAndNetMask() instead.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssocksetipaddress_struct
{
	RTS_IEC_STRING *szCard;				/* VAR_INPUT */	/* Name of the ethernet card (aka adapter name) */
	RTS_IEC_STRING *szIPAddress;		/* VAR_INPUT */	/* IP address to set as string */
	RTS_IEC_RESULT SysSockSetIPAddress;	/* VAR_OUTPUT */	
} syssocksetipaddress_struct;

void CDECL CDECL_EXT syssocksetipaddress(syssocksetipaddress_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKSETIPADDRESS_IEC) (syssocksetipaddress_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSETIPADDRESS_NOTIMPLEMENTED)
	#define USE_syssocksetipaddress
	#define EXT_syssocksetipaddress
	#define GET_syssocksetipaddress(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssocksetipaddress(p0) 
	#define CHK_syssocksetipaddress  FALSE
	#define EXP_syssocksetipaddress  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssocksetipaddress
	#define EXT_syssocksetipaddress
	#define GET_syssocksetipaddress(fl)  CAL_CMGETAPI( "syssocksetipaddress" ) 
	#define CAL_syssocksetipaddress  syssocksetipaddress
	#define CHK_syssocksetipaddress  TRUE
	#define EXP_syssocksetipaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetipaddress", (RTS_UINTPTR)syssocksetipaddress, 1, RTSITF_GET_SIGNATURE(0, 0x955CE64B), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssocksetipaddress
	#define EXT_syssocksetipaddress
	#define GET_syssocksetipaddress(fl)  CAL_CMGETAPI( "syssocksetipaddress" ) 
	#define CAL_syssocksetipaddress  syssocksetipaddress
	#define CHK_syssocksetipaddress  TRUE
	#define EXP_syssocksetipaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetipaddress", (RTS_UINTPTR)syssocksetipaddress, 1, RTSITF_GET_SIGNATURE(0, 0x955CE64B), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssocksetipaddress
	#define EXT_SysSocketsyssocksetipaddress
	#define GET_SysSocketsyssocksetipaddress  ERR_OK
	#define CAL_SysSocketsyssocksetipaddress  syssocksetipaddress
	#define CHK_SysSocketsyssocksetipaddress  TRUE
	#define EXP_SysSocketsyssocksetipaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetipaddress", (RTS_UINTPTR)syssocksetipaddress, 1, RTSITF_GET_SIGNATURE(0, 0x955CE64B), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssocksetipaddress
	#define EXT_syssocksetipaddress
	#define GET_syssocksetipaddress(fl)  CAL_CMGETAPI( "syssocksetipaddress" ) 
	#define CAL_syssocksetipaddress  syssocksetipaddress
	#define CHK_syssocksetipaddress  TRUE
	#define EXP_syssocksetipaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetipaddress", (RTS_UINTPTR)syssocksetipaddress, 1, RTSITF_GET_SIGNATURE(0, 0x955CE64B), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssocksetipaddress  PFSYSSOCKSETIPADDRESS_IEC pfsyssocksetipaddress;
	#define EXT_syssocksetipaddress  extern PFSYSSOCKSETIPADDRESS_IEC pfsyssocksetipaddress;
	#define GET_syssocksetipaddress(fl)  s_pfCMGetAPI2( "syssocksetipaddress", (RTS_VOID_FCTPTR *)&pfsyssocksetipaddress, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x955CE64B), 0x03050E00)
	#define CAL_syssocksetipaddress  pfsyssocksetipaddress
	#define CHK_syssocksetipaddress  (pfsyssocksetipaddress != NULL)
	#define EXP_syssocksetipaddress   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetipaddress", (RTS_UINTPTR)syssocksetipaddress, 1, RTSITF_GET_SIGNATURE(0, 0x955CE64B), 0x03050E00) 
#endif


/**
 * | Set IP address and subnet mask of an adapter. 
 * | It depends on the device, whether the new ip address and subnet mask is reset during reboot or
 *   if it is retained. In general the caller should consider these as volatile. The combination 
 *   IP address = 0.0.0.0 and subnet mask = 0.0.0.0 can be used to remove the IP address from the adapter.
 *   After this there is no IP based communication possible anymore, until a new IP address is set.
 *   Replaces the functions SysSockSetIPAddress() and SysSockSetSubnetMask().
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssocksetipaddressandnetmask_struct
{
	RTS_IEC_WSTRING *wsAdapterName;		/* VAR_INPUT */	/* Adapter name provided by SysSockGetFirstAdapter() / SysSockGetNextAdapter() */
	INADDR *IpAddr;						/* VAR_INPUT */	/* Ip address to set in network byte order */
	INADDR *NetMask;					/* VAR_INPUT */	/* Subnet mask to set in network byte order */
	RTS_IEC_RESULT SysSockSetIpAddressAndNetMask;	/* VAR_OUTPUT */	
} syssocksetipaddressandnetmask_struct;

void CDECL CDECL_EXT syssocksetipaddressandnetmask(syssocksetipaddressandnetmask_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKSETIPADDRESSANDNETMASK_IEC) (syssocksetipaddressandnetmask_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSETIPADDRESSANDNETMASK_NOTIMPLEMENTED)
	#define USE_syssocksetipaddressandnetmask
	#define EXT_syssocksetipaddressandnetmask
	#define GET_syssocksetipaddressandnetmask(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssocksetipaddressandnetmask(p0) 
	#define CHK_syssocksetipaddressandnetmask  FALSE
	#define EXP_syssocksetipaddressandnetmask  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssocksetipaddressandnetmask
	#define EXT_syssocksetipaddressandnetmask
	#define GET_syssocksetipaddressandnetmask(fl)  CAL_CMGETAPI( "syssocksetipaddressandnetmask" ) 
	#define CAL_syssocksetipaddressandnetmask  syssocksetipaddressandnetmask
	#define CHK_syssocksetipaddressandnetmask  TRUE
	#define EXP_syssocksetipaddressandnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetipaddressandnetmask", (RTS_UINTPTR)syssocksetipaddressandnetmask, 1, 0x01D16C55, 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssocksetipaddressandnetmask
	#define EXT_syssocksetipaddressandnetmask
	#define GET_syssocksetipaddressandnetmask(fl)  CAL_CMGETAPI( "syssocksetipaddressandnetmask" ) 
	#define CAL_syssocksetipaddressandnetmask  syssocksetipaddressandnetmask
	#define CHK_syssocksetipaddressandnetmask  TRUE
	#define EXP_syssocksetipaddressandnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetipaddressandnetmask", (RTS_UINTPTR)syssocksetipaddressandnetmask, 1, 0x01D16C55, 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssocksetipaddressandnetmask
	#define EXT_SysSocketsyssocksetipaddressandnetmask
	#define GET_SysSocketsyssocksetipaddressandnetmask  ERR_OK
	#define CAL_SysSocketsyssocksetipaddressandnetmask  syssocksetipaddressandnetmask
	#define CHK_SysSocketsyssocksetipaddressandnetmask  TRUE
	#define EXP_SysSocketsyssocksetipaddressandnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetipaddressandnetmask", (RTS_UINTPTR)syssocksetipaddressandnetmask, 1, 0x01D16C55, 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssocksetipaddressandnetmask
	#define EXT_syssocksetipaddressandnetmask
	#define GET_syssocksetipaddressandnetmask(fl)  CAL_CMGETAPI( "syssocksetipaddressandnetmask" ) 
	#define CAL_syssocksetipaddressandnetmask  syssocksetipaddressandnetmask
	#define CHK_syssocksetipaddressandnetmask  TRUE
	#define EXP_syssocksetipaddressandnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetipaddressandnetmask", (RTS_UINTPTR)syssocksetipaddressandnetmask, 1, 0x01D16C55, 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssocksetipaddressandnetmask  PFSYSSOCKSETIPADDRESSANDNETMASK_IEC pfsyssocksetipaddressandnetmask;
	#define EXT_syssocksetipaddressandnetmask  extern PFSYSSOCKSETIPADDRESSANDNETMASK_IEC pfsyssocksetipaddressandnetmask;
	#define GET_syssocksetipaddressandnetmask(fl)  s_pfCMGetAPI2( "syssocksetipaddressandnetmask", (RTS_VOID_FCTPTR *)&pfsyssocksetipaddressandnetmask, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x01D16C55, 0x03050E00)
	#define CAL_syssocksetipaddressandnetmask  pfsyssocksetipaddressandnetmask
	#define CHK_syssocksetipaddressandnetmask  (pfsyssocksetipaddressandnetmask != NULL)
	#define EXP_syssocksetipaddressandnetmask   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetipaddressandnetmask", (RTS_UINTPTR)syssocksetipaddressandnetmask, 1, 0x01D16C55, 0x03050E00) 
#endif


/**
 * | Set options of a specified socket.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssocksetoption_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_DINT diLevel;				/* VAR_INPUT */	/* Level of the socket */
	RTS_IEC_DINT diOption;				/* VAR_INPUT */	/* Socket option command */
	RTS_IEC_DINT *pdiOptionValue;		/* VAR_INPUT */	/* Pointer to the option value */
	RTS_IEC_DINT diOptionLen;			/* VAR_INPUT */	/* Lenght of option value */
	RTS_IEC_RESULT SysSockSetOption;	/* VAR_OUTPUT */	
} syssocksetoption_struct;

void CDECL CDECL_EXT syssocksetoption(syssocksetoption_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKSETOPTION_IEC) (syssocksetoption_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSETOPTION_NOTIMPLEMENTED)
	#define USE_syssocksetoption
	#define EXT_syssocksetoption
	#define GET_syssocksetoption(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssocksetoption(p0) 
	#define CHK_syssocksetoption  FALSE
	#define EXP_syssocksetoption  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssocksetoption
	#define EXT_syssocksetoption
	#define GET_syssocksetoption(fl)  CAL_CMGETAPI( "syssocksetoption" ) 
	#define CAL_syssocksetoption  syssocksetoption
	#define CHK_syssocksetoption  TRUE
	#define EXP_syssocksetoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetoption", (RTS_UINTPTR)syssocksetoption, 1, RTSITF_GET_SIGNATURE(0, 0xFBCD9B23), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssocksetoption
	#define EXT_syssocksetoption
	#define GET_syssocksetoption(fl)  CAL_CMGETAPI( "syssocksetoption" ) 
	#define CAL_syssocksetoption  syssocksetoption
	#define CHK_syssocksetoption  TRUE
	#define EXP_syssocksetoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetoption", (RTS_UINTPTR)syssocksetoption, 1, RTSITF_GET_SIGNATURE(0, 0xFBCD9B23), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssocksetoption
	#define EXT_SysSocketsyssocksetoption
	#define GET_SysSocketsyssocksetoption  ERR_OK
	#define CAL_SysSocketsyssocksetoption  syssocksetoption
	#define CHK_SysSocketsyssocksetoption  TRUE
	#define EXP_SysSocketsyssocksetoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetoption", (RTS_UINTPTR)syssocksetoption, 1, RTSITF_GET_SIGNATURE(0, 0xFBCD9B23), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssocksetoption
	#define EXT_syssocksetoption
	#define GET_syssocksetoption(fl)  CAL_CMGETAPI( "syssocksetoption" ) 
	#define CAL_syssocksetoption  syssocksetoption
	#define CHK_syssocksetoption  TRUE
	#define EXP_syssocksetoption  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetoption", (RTS_UINTPTR)syssocksetoption, 1, RTSITF_GET_SIGNATURE(0, 0xFBCD9B23), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssocksetoption  PFSYSSOCKSETOPTION_IEC pfsyssocksetoption;
	#define EXT_syssocksetoption  extern PFSYSSOCKSETOPTION_IEC pfsyssocksetoption;
	#define GET_syssocksetoption(fl)  s_pfCMGetAPI2( "syssocksetoption", (RTS_VOID_FCTPTR *)&pfsyssocksetoption, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xFBCD9B23), 0x03050E00)
	#define CAL_syssocksetoption  pfsyssocksetoption
	#define CHK_syssocksetoption  (pfsyssocksetoption != NULL)
	#define EXP_syssocksetoption   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetoption", (RTS_UINTPTR)syssocksetoption, 1, RTSITF_GET_SIGNATURE(0, 0xFBCD9B23), 0x03050E00) 
#endif


/**
 * | Set subnetmask of an adapter, specified by IP address. Is not available on all platforms!
 * | Use SysSockSetIpAddressAndNetMask() instead.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssocksetsubnetmask_struct
{
	RTS_IEC_STRING *szIPAddress;		/* VAR_INPUT */	/* IP address of the communication partner as string */
	RTS_IEC_STRING *szSubnetMask;		/* VAR_INPUT */	/* Subnet mask as string */
	RTS_IEC_RESULT SysSockSetSubnetMask;	/* VAR_OUTPUT */	
} syssocksetsubnetmask_struct;

void CDECL CDECL_EXT syssocksetsubnetmask(syssocksetsubnetmask_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKSETSUBNETMASK_IEC) (syssocksetsubnetmask_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSETSUBNETMASK_NOTIMPLEMENTED)
	#define USE_syssocksetsubnetmask
	#define EXT_syssocksetsubnetmask
	#define GET_syssocksetsubnetmask(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssocksetsubnetmask(p0) 
	#define CHK_syssocksetsubnetmask  FALSE
	#define EXP_syssocksetsubnetmask  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssocksetsubnetmask
	#define EXT_syssocksetsubnetmask
	#define GET_syssocksetsubnetmask(fl)  CAL_CMGETAPI( "syssocksetsubnetmask" ) 
	#define CAL_syssocksetsubnetmask  syssocksetsubnetmask
	#define CHK_syssocksetsubnetmask  TRUE
	#define EXP_syssocksetsubnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetsubnetmask", (RTS_UINTPTR)syssocksetsubnetmask, 1, RTSITF_GET_SIGNATURE(0, 0x0185BC67), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssocksetsubnetmask
	#define EXT_syssocksetsubnetmask
	#define GET_syssocksetsubnetmask(fl)  CAL_CMGETAPI( "syssocksetsubnetmask" ) 
	#define CAL_syssocksetsubnetmask  syssocksetsubnetmask
	#define CHK_syssocksetsubnetmask  TRUE
	#define EXP_syssocksetsubnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetsubnetmask", (RTS_UINTPTR)syssocksetsubnetmask, 1, RTSITF_GET_SIGNATURE(0, 0x0185BC67), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssocksetsubnetmask
	#define EXT_SysSocketsyssocksetsubnetmask
	#define GET_SysSocketsyssocksetsubnetmask  ERR_OK
	#define CAL_SysSocketsyssocksetsubnetmask  syssocksetsubnetmask
	#define CHK_SysSocketsyssocksetsubnetmask  TRUE
	#define EXP_SysSocketsyssocksetsubnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetsubnetmask", (RTS_UINTPTR)syssocksetsubnetmask, 1, RTSITF_GET_SIGNATURE(0, 0x0185BC67), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssocksetsubnetmask
	#define EXT_syssocksetsubnetmask
	#define GET_syssocksetsubnetmask(fl)  CAL_CMGETAPI( "syssocksetsubnetmask" ) 
	#define CAL_syssocksetsubnetmask  syssocksetsubnetmask
	#define CHK_syssocksetsubnetmask  TRUE
	#define EXP_syssocksetsubnetmask  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetsubnetmask", (RTS_UINTPTR)syssocksetsubnetmask, 1, RTSITF_GET_SIGNATURE(0, 0x0185BC67), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssocksetsubnetmask  PFSYSSOCKSETSUBNETMASK_IEC pfsyssocksetsubnetmask;
	#define EXT_syssocksetsubnetmask  extern PFSYSSOCKSETSUBNETMASK_IEC pfsyssocksetsubnetmask;
	#define GET_syssocksetsubnetmask(fl)  s_pfCMGetAPI2( "syssocksetsubnetmask", (RTS_VOID_FCTPTR *)&pfsyssocksetsubnetmask, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x0185BC67), 0x03050E00)
	#define CAL_syssocksetsubnetmask  pfsyssocksetsubnetmask
	#define CHK_syssocksetsubnetmask  (pfsyssocksetsubnetmask != NULL)
	#define EXP_syssocksetsubnetmask   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssocksetsubnetmask", (RTS_UINTPTR)syssocksetsubnetmask, 1, RTSITF_GET_SIGNATURE(0, 0x0185BC67), 0x03050E00) 
#endif


/**
 * | Shutdown a socket.
 * :return: Runtime system error code (see CmpErrors.library).
 */
typedef struct tagsyssockshutdown_struct
{
	RTS_IEC_HANDLE hSocket;				/* VAR_INPUT */	/* Handle to the socket */
	RTS_IEC_DINT diHow;					/* VAR_INPUT */	/* Specifies, which operations are no longer be allowed. See shutdown flags. */
	RTS_IEC_RESULT SysSockShutdown;		/* VAR_OUTPUT */	
} syssockshutdown_struct;

void CDECL CDECL_EXT syssockshutdown(syssockshutdown_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSSOCKSHUTDOWN_IEC) (syssockshutdown_struct *p);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSHUTDOWN_NOTIMPLEMENTED)
	#define USE_syssockshutdown
	#define EXT_syssockshutdown
	#define GET_syssockshutdown(fl)  ERR_NOTIMPLEMENTED
	#define CAL_syssockshutdown(p0) 
	#define CHK_syssockshutdown  FALSE
	#define EXP_syssockshutdown  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_syssockshutdown
	#define EXT_syssockshutdown
	#define GET_syssockshutdown(fl)  CAL_CMGETAPI( "syssockshutdown" ) 
	#define CAL_syssockshutdown  syssockshutdown
	#define CHK_syssockshutdown  TRUE
	#define EXP_syssockshutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockshutdown", (RTS_UINTPTR)syssockshutdown, 1, RTSITF_GET_SIGNATURE(0, 0xCB0E612E), 0x03050E00) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_syssockshutdown
	#define EXT_syssockshutdown
	#define GET_syssockshutdown(fl)  CAL_CMGETAPI( "syssockshutdown" ) 
	#define CAL_syssockshutdown  syssockshutdown
	#define CHK_syssockshutdown  TRUE
	#define EXP_syssockshutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockshutdown", (RTS_UINTPTR)syssockshutdown, 1, RTSITF_GET_SIGNATURE(0, 0xCB0E612E), 0x03050E00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketsyssockshutdown
	#define EXT_SysSocketsyssockshutdown
	#define GET_SysSocketsyssockshutdown  ERR_OK
	#define CAL_SysSocketsyssockshutdown  syssockshutdown
	#define CHK_SysSocketsyssockshutdown  TRUE
	#define EXP_SysSocketsyssockshutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockshutdown", (RTS_UINTPTR)syssockshutdown, 1, RTSITF_GET_SIGNATURE(0, 0xCB0E612E), 0x03050E00) 
#elif defined(CPLUSPLUS)
	#define USE_syssockshutdown
	#define EXT_syssockshutdown
	#define GET_syssockshutdown(fl)  CAL_CMGETAPI( "syssockshutdown" ) 
	#define CAL_syssockshutdown  syssockshutdown
	#define CHK_syssockshutdown  TRUE
	#define EXP_syssockshutdown  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockshutdown", (RTS_UINTPTR)syssockshutdown, 1, RTSITF_GET_SIGNATURE(0, 0xCB0E612E), 0x03050E00) 
#else /* DYNAMIC_LINK */
	#define USE_syssockshutdown  PFSYSSOCKSHUTDOWN_IEC pfsyssockshutdown;
	#define EXT_syssockshutdown  extern PFSYSSOCKSHUTDOWN_IEC pfsyssockshutdown;
	#define GET_syssockshutdown(fl)  s_pfCMGetAPI2( "syssockshutdown", (RTS_VOID_FCTPTR *)&pfsyssockshutdown, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xCB0E612E), 0x03050E00)
	#define CAL_syssockshutdown  pfsyssockshutdown
	#define CHK_syssockshutdown  (pfsyssockshutdown != NULL)
	#define EXP_syssockshutdown   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"syssockshutdown", (RTS_UINTPTR)syssockshutdown, 1, RTSITF_GET_SIGNATURE(0, 0xCB0E612E), 0x03050E00) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <category>Internal flags for adapter management</category>
 * <description></description>
 */
#define SOCK_IFLAG_AF_INETFOUND				0x00000001	/* Adapter information for AF_INET already found - to be used internally in SysSocketOS implementation */
#define SOCK_IFLAG_IN_PROGRESS				0x00000002	/* Adapter information is currently checked for update - to be used internally in SysSocketOS implementation */
#define SOCK_IFLAG_FREE_STRINGS_ON_EXIT		0x00000004	/* On shutdown SysSocket.c checks this flag and if set it frees all referenced strings by calling SysMemFree() */
#define SOCK_IFLAG_FREE_HOSSPECIFIC_ON_EXIT	0x00000008	/* On shutdown SysSocket.c checks this flag and if set it frees hOsSpecific by calling SysMemFree() */

/**
 * <category>Internal SysSocket adapter information struct</category>
 * <description>Internal structure to describe a network adapter on SysSocket level</description>
 * <element name="hOsSpecific">Only to be used by the OS specific part to store local data</element>
 * <element name="ui32InternalFlags">Internal used flags, see category "Internal flags for adapter management"</element>
 * <element name="Adapter">Public structure SOCK_ADAPTER_INFO</element>
*/
typedef struct tagSOCK_ADAPTER_INFO_INT
{
	RTS_HANDLE hOsSpecific;
	RTS_UI32 ui32InternalFlags;
	SOCK_ADAPTER_INFO Adapter;
} SOCK_ADAPTER_INFO_INT;

/**
 * Helper functions to fill the adapter information, only to be called by OS specific implementation of SysSocket.
 */
/* Get a new adapter internal adapter information block. */ 
SOCK_ADAPTER_INFO_INT* CDECL SysSockGetAdapterInfoBlockInternal(RTS_RESULT *pResult);

/* Append a new filled adapter information block to the adapter list. */ 
RTS_RESULT CDECL SysSockAppendAdapterInfoBlockInternal(SOCK_ADAPTER_INFO_INT* pAdapter);

/* Get the first adapter. Is used to check/change the content of the adapter information. */ 
SOCK_ADAPTER_INFO_INT* CDECL SysSockGetFirstAdapterInfoInternal(RTS_RESULT *pResult);

/* Get the next adapter. Is used to check/change the content of the adapter information. */ 
SOCK_ADAPTER_INFO_INT* CDECL SysSockGetNextAdapterInfoInternal(SOCK_ADAPTER_INFO_INT *pPrevAdapter, RTS_RESULT *pResult);

/**
 * Functions to be implemented by the OS specific implementation of SysSocket.
 */
/* SysSockOSUpdateNetworkAdapterInfo() is called on startup (typically during the CH_INIT2 hook) 
   and each time the adapter information is requested by the runtime system (e. g. by a call of 
   SysSockGetFirstAdapterInfo()). It is mandatory to fill the adapter information completly
   during the first call to have a valid SysSocket adapter list afterwards.
   
   There are several ways to keep this list updated:
   1. On small systems with a static configuration, which cannot be changed anymore after startup, all 
      further calls can directly return without touching the adapter information anymore.
   
   2. If the adapter information is easy and fast to read from the system, then on each call of 
      SysSockOSUpdateNetworkAdapterInfo() the implementation can do a check or update of the adapter information.
	  Be carefull, the function is usually called by different tasks and a reentrant implementation 
	  is required. Instead of using intlocks or semaphores, SysCpuTestAndSetBit() on a flag should 
	  be used to skip the update when this is already executed in the context of another task.

   3. For ethernet stacks which provide a notification/callback/event for changes of the ip configuration 
	  (e. g. add/remove adapter or ip address, change of ip adress/subnet mask, change of link state), the
	  adapter information shall be updated in the first call (init hook) and when a change is signaled.
	  
   4. It is also possible to update the SysSocket adapter information later on cyclically in the context 
      of the HookFunction() in CH_COMM_CYCLE. In this case the check/update should be executed every
	  about 5 seconds or less.   

   General implementation notes:
	- Do not touch SOCK_ADAPTER_INFO_INT.Adapter.ui32StructSize, SOCK_ADAPTER_INFO_INT.Adapter.ui32Version
	  and SOCK_ADAPTER_INFO_INT.Adapter.ui32Index, because these are filled by SysSocket.c.
	- After calling SysSockAppendAdapterInfoBlockInternal(), do not change SOCK_ADAPTER_INFO_INT.Adapter.pwszName 
	  or SOCK_ADAPTER_INFO_INT.Adapter.pwszDescription anymore.
	- If the name of an adapter changes, add a new adapter to the SysSocket adapter list.
	- All adapters remain until shutdown in the SysSocket list (with removed flag SOCK_AIF_INFO_VALID),
	  even if an adapter is removed from the system. Adapters without SOCK_AIF_INFO_VALID are hidden for the 
	  caller of SysSockGetFirstAdapterInfo() or SysSockGetNextAdapterInfo().
	- If a removed adapter is added later on again, the old list entry can be recycled (set SOCK_AIF_INFO_VALID again).
	- Set the internal flag SOCK_IFLAG_FREE_STRINGS_ON_EXIT only, if SysSocket.c should free 
	  SOCK_ADAPTER_INFO_INT.Adapter.pwszName and SOCK_ADAPTER_INFO_INT.Adapter.pwszDescription.
	- SOCK_ADAPTER_INFO_INT.hOsSpecific can be used to store local data for the adapter. This is not 
	  evaluated/touched by SysSocket.c 
	- Inactive adapters, which are not ready for ip communication (e. g. no ip, no link) should be treated as 
	  valid ones, but marked with the flag SOCK_AIF_INACTIVE in SOCK_ADAPTER_INFO_INT.Adapter.ui32Flags.
	- Both SOCK_ADAPTER_INFO_INT.ui32InternalFlags and SOCK_ADAPTER_INFO_INT.Adapter.ui32Flags must only be
	  changed on bit level and not written as complete RTS_UI32 to keep all other flags.
	- The flag SOCK_AIF_IP_CHANGE_ALLOWED is handled by the generic part of SysSocket implemention (SysSocket.c).
	- See SysSocketItf.m4/h for more information regarding SOCK_ADAPTER_INFO and SOCK_ADAPTER_INFO_INT. 
*/
RTS_RESULT CDECL SysSockOSUpdateNetworkAdapterInfo(void);

/* Set ip address and subnet mask of an adapter. Is called by SysSockSetIpAddressAndNetMask() after reading
   the adapter list settings and sending the event EVT_SysSocket_BeforeSetIpAndMask. After return 
   SysSockSetIpAddressAndNetMask() sends the event EVT_SysSocket_AfterSetIpAndMask. 
   See declaration of SysSockSetIpAddressAndNetMask() for more information (result codes etc.). 
   The new ip address and subnet mask may or may not be retained over a reboot of the device. 
   In general the caller should consider these as volatile. 
*/ 
RTS_RESULT CDECL SysSockOSSetIpAddressAndNetMask(RTS_WCHAR *pwszAdapterName, INADDR* pIpAddr, INADDR* pNetMask);

/* Set gateway address and subnet mask of an adapter. Is called by SysSockSetGateway() after reading
   the adapter list settings and sending the event EVT_SysSocket_BeforeSetGateway. After return
   SysSockSetGateway() sends the event EVT_SysSocket_AfterSetGateway. 
   See declaration of SysSockSetGateway() for more information (result codes etc.). 
   The new gateway address and subnet mask may or may not be retained over a reboot of the device. 
   In general the caller should consider these as volatile. 
*/ 
RTS_RESULT CDECL SysSockOSSetDefaultGateway(RTS_WCHAR *pwszAdapterName, INADDR* pGatewayAddr);

/* Init routines for OS specific modules */
RTS_RESULT CDECL SysSockOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysSockOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);


/*---- TCP/UDP-IP protocol routines -----------------------------------------*/
/**
 * <description>
 *	Create a new socket and return the socket handle. In case of an error, RTS_INVALID_HANDLE is returned.
 * </description>
 * <param name="iAddressFamily" type="IN">Socket address family</param>
 * <param name="iType" type="IN">Socket type</param>
 * <param name="iProtocol" type="IN">Socket protocol</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the socket</result>
 */
RTS_HANDLE CDECL SysSockCreate(int iAddressFamily, int iType, int iProtocol, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSSOCKCREATE) (int iAddressFamily, int iType, int iProtocol, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKCREATE_NOTIMPLEMENTED)
	#define USE_SysSockCreate
	#define EXT_SysSockCreate
	#define GET_SysSockCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockCreate(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysSockCreate  FALSE
	#define EXP_SysSockCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockCreate
	#define EXT_SysSockCreate
	#define GET_SysSockCreate(fl)  CAL_CMGETAPI( "SysSockCreate" ) 
	#define CAL_SysSockCreate  SysSockCreate
	#define CHK_SysSockCreate  TRUE
	#define EXP_SysSockCreate  CAL_CMEXPAPI( "SysSockCreate" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockCreate
	#define EXT_SysSockCreate
	#define GET_SysSockCreate(fl)  CAL_CMGETAPI( "SysSockCreate" ) 
	#define CAL_SysSockCreate  SysSockCreate
	#define CHK_SysSockCreate  TRUE
	#define EXP_SysSockCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockCreate", (RTS_UINTPTR)SysSockCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockCreate
	#define EXT_SysSocketSysSockCreate
	#define GET_SysSocketSysSockCreate  ERR_OK
	#define CAL_SysSocketSysSockCreate  ((ISysSocket*)s_pfCMCreateInstance(CLASSID_CSysSocket, NULL))->ISysSockCreate
	#define CHK_SysSocketSysSockCreate	(s_pfCMCreateInstance != NULL && pISysSocket != NULL)
	#define EXP_SysSocketSysSockCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockCreate
	#define EXT_SysSockCreate
	#define GET_SysSockCreate(fl)  CAL_CMGETAPI( "SysSockCreate" ) 
	#define CAL_SysSockCreate  ((ISysSocket*)s_pfCMCreateInstance(CLASSID_CSysSocket, NULL))->ISysSockCreate
	#define CHK_SysSockCreate	(s_pfCMCreateInstance != NULL && pISysSocket != NULL)
	#define EXP_SysSockCreate  CAL_CMEXPAPI( "SysSockCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockCreate  PFSYSSOCKCREATE pfSysSockCreate;
	#define EXT_SysSockCreate  extern PFSYSSOCKCREATE pfSysSockCreate;
	#define GET_SysSockCreate(fl)  s_pfCMGetAPI2( "SysSockCreate", (RTS_VOID_FCTPTR *)&pfSysSockCreate, (fl), 0, 0)
	#define CAL_SysSockCreate  pfSysSockCreate
	#define CHK_SysSockCreate  (pfSysSockCreate != NULL)
	#define EXP_SysSockCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockCreate", (RTS_UINTPTR)SysSockCreate, 0, 0) 
#endif




/**
 * <description>
 *	Close a socket.
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockClose(RTS_HANDLE hSocket);
typedef RTS_RESULT (CDECL * PFSYSSOCKCLOSE) (RTS_HANDLE hSocket);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKCLOSE_NOTIMPLEMENTED)
	#define USE_SysSockClose
	#define EXT_SysSockClose
	#define GET_SysSockClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockClose  FALSE
	#define EXP_SysSockClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockClose
	#define EXT_SysSockClose
	#define GET_SysSockClose(fl)  CAL_CMGETAPI( "SysSockClose" ) 
	#define CAL_SysSockClose  SysSockClose
	#define CHK_SysSockClose  TRUE
	#define EXP_SysSockClose  CAL_CMEXPAPI( "SysSockClose" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockClose
	#define EXT_SysSockClose
	#define GET_SysSockClose(fl)  CAL_CMGETAPI( "SysSockClose" ) 
	#define CAL_SysSockClose  SysSockClose
	#define CHK_SysSockClose  TRUE
	#define EXP_SysSockClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockClose", (RTS_UINTPTR)SysSockClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockClose
	#define EXT_SysSocketSysSockClose
	#define GET_SysSocketSysSockClose  ERR_OK
	#define CAL_SysSocketSysSockClose(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysSocket*)p0)->ISysSockClose())
	#define CHK_SysSocketSysSockClose  TRUE
	#define EXP_SysSocketSysSockClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockClose
	#define EXT_SysSockClose
	#define GET_SysSockClose(fl)  CAL_CMGETAPI( "SysSockClose" ) 
	#define CAL_SysSockClose(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysSocket*)p0)->ISysSockClose())
	#define CHK_SysSockClose  TRUE
	#define EXP_SysSockClose  CAL_CMEXPAPI( "SysSockClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockClose  PFSYSSOCKCLOSE pfSysSockClose;
	#define EXT_SysSockClose  extern PFSYSSOCKCLOSE pfSysSockClose;
	#define GET_SysSockClose(fl)  s_pfCMGetAPI2( "SysSockClose", (RTS_VOID_FCTPTR *)&pfSysSockClose, (fl), 0, 0)
	#define CAL_SysSockClose  pfSysSockClose
	#define CHK_SysSockClose  (pfSysSockClose != NULL)
	#define EXP_SysSockClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockClose", (RTS_UINTPTR)SysSockClose, 0, 0) 
#endif




/**
 * <description>
 *	Set options of a specified socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="iLevel" type="IN">Level of the socket</param>
 * <param name="iOption" type="IN">Socket option command</param>
 * <param name="pcOptionValue" type="IN">Pointer to the option value</param>
 * <param name="iOptionLen" type="IN">Lenght of option value</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockSetOption(RTS_HANDLE hSocket, int iLevel, int iOption, char *pcOptionValue, int iOptionLen);
typedef RTS_RESULT (CDECL * PFSYSSOCKSETOPTION) (RTS_HANDLE hSocket, int iLevel, int iOption, char *pcOptionValue, int iOptionLen);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSETOPTION_NOTIMPLEMENTED)
	#define USE_SysSockSetOption
	#define EXT_SysSockSetOption
	#define GET_SysSockSetOption(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockSetOption(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockSetOption  FALSE
	#define EXP_SysSockSetOption  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockSetOption
	#define EXT_SysSockSetOption
	#define GET_SysSockSetOption(fl)  CAL_CMGETAPI( "SysSockSetOption" ) 
	#define CAL_SysSockSetOption  SysSockSetOption
	#define CHK_SysSockSetOption  TRUE
	#define EXP_SysSockSetOption  CAL_CMEXPAPI( "SysSockSetOption" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockSetOption
	#define EXT_SysSockSetOption
	#define GET_SysSockSetOption(fl)  CAL_CMGETAPI( "SysSockSetOption" ) 
	#define CAL_SysSockSetOption  SysSockSetOption
	#define CHK_SysSockSetOption  TRUE
	#define EXP_SysSockSetOption  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSetOption", (RTS_UINTPTR)SysSockSetOption, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockSetOption
	#define EXT_SysSocketSysSockSetOption
	#define GET_SysSocketSysSockSetOption  ERR_OK
	#define CAL_SysSocketSysSockSetOption(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockSetOption(p1,p2,p3,p4) : ((ISysSocket*)p0)->ISysSockSetOption(p1,p2,p3,p4))
	#define CHK_SysSocketSysSockSetOption  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockSetOption  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockSetOption
	#define EXT_SysSockSetOption
	#define GET_SysSockSetOption(fl)  CAL_CMGETAPI( "SysSockSetOption" ) 
	#define CAL_SysSockSetOption(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockSetOption(p1,p2,p3,p4) : ((ISysSocket*)p0)->ISysSockSetOption(p1,p2,p3,p4))
	#define CHK_SysSockSetOption  (pISysSocket != NULL)
	#define EXP_SysSockSetOption  CAL_CMEXPAPI( "SysSockSetOption" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockSetOption  PFSYSSOCKSETOPTION pfSysSockSetOption;
	#define EXT_SysSockSetOption  extern PFSYSSOCKSETOPTION pfSysSockSetOption;
	#define GET_SysSockSetOption(fl)  s_pfCMGetAPI2( "SysSockSetOption", (RTS_VOID_FCTPTR *)&pfSysSockSetOption, (fl), 0, 0)
	#define CAL_SysSockSetOption  pfSysSockSetOption
	#define CHK_SysSockSetOption  (pfSysSockSetOption != NULL)
	#define EXP_SysSockSetOption  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSetOption", (RTS_UINTPTR)SysSockSetOption, 0, 0) 
#endif




/**
 * <description>
 *	Get options of a specified socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="iLevel" type="IN">Level of the socket</param>
 * <param name="iOption" type="IN">Socket option command</param>
 * <param name="pcOptionValue" type="OUT">Pointer to get the option value</param>
 * <param name="piOptionLen" type="OUT">Pointer to the option length. Real length is returned</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockGetOption(RTS_HANDLE hSocket, int iLevel, int iOption, char *pcOptionValue, int *piOptionLen);
typedef RTS_RESULT (CDECL * PFSYSSOCKGETOPTION) (RTS_HANDLE hSocket, int iLevel, int iOption, char *pcOptionValue, int *piOptionLen);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETOPTION_NOTIMPLEMENTED)
	#define USE_SysSockGetOption
	#define EXT_SysSockGetOption
	#define GET_SysSockGetOption(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockGetOption(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockGetOption  FALSE
	#define EXP_SysSockGetOption  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockGetOption
	#define EXT_SysSockGetOption
	#define GET_SysSockGetOption(fl)  CAL_CMGETAPI( "SysSockGetOption" ) 
	#define CAL_SysSockGetOption  SysSockGetOption
	#define CHK_SysSockGetOption  TRUE
	#define EXP_SysSockGetOption  CAL_CMEXPAPI( "SysSockGetOption" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockGetOption
	#define EXT_SysSockGetOption
	#define GET_SysSockGetOption(fl)  CAL_CMGETAPI( "SysSockGetOption" ) 
	#define CAL_SysSockGetOption  SysSockGetOption
	#define CHK_SysSockGetOption  TRUE
	#define EXP_SysSockGetOption  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetOption", (RTS_UINTPTR)SysSockGetOption, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockGetOption
	#define EXT_SysSocketSysSockGetOption
	#define GET_SysSocketSysSockGetOption  ERR_OK
	#define CAL_SysSocketSysSockGetOption(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockGetOption(p1,p2,p3,p4) : ((ISysSocket*)p0)->ISysSockGetOption(p1,p2,p3,p4))
	#define CHK_SysSocketSysSockGetOption  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockGetOption  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockGetOption
	#define EXT_SysSockGetOption
	#define GET_SysSockGetOption(fl)  CAL_CMGETAPI( "SysSockGetOption" ) 
	#define CAL_SysSockGetOption(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockGetOption(p1,p2,p3,p4) : ((ISysSocket*)p0)->ISysSockGetOption(p1,p2,p3,p4))
	#define CHK_SysSockGetOption  (pISysSocket != NULL)
	#define EXP_SysSockGetOption  CAL_CMEXPAPI( "SysSockGetOption" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockGetOption  PFSYSSOCKGETOPTION pfSysSockGetOption;
	#define EXT_SysSockGetOption  extern PFSYSSOCKGETOPTION pfSysSockGetOption;
	#define GET_SysSockGetOption(fl)  s_pfCMGetAPI2( "SysSockGetOption", (RTS_VOID_FCTPTR *)&pfSysSockGetOption, (fl), 0, 0)
	#define CAL_SysSockGetOption  pfSysSockGetOption
	#define CHK_SysSockGetOption  (pfSysSockGetOption != NULL)
	#define EXP_SysSockGetOption  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetOption", (RTS_UINTPTR)SysSockGetOption, 0, 0) 
#endif




/**
 * <description>
 *	Bind a socket to a socket address and port number
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pSockAddr" type="IN">Spcket address</param>
 * <param name="iSockAddrSize" type="IN">Size of the socket address structure</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockBind(RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, int iSockAddrSize);
typedef RTS_RESULT (CDECL * PFSYSSOCKBIND) (RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, int iSockAddrSize);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKBIND_NOTIMPLEMENTED)
	#define USE_SysSockBind
	#define EXT_SysSockBind
	#define GET_SysSockBind(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockBind(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockBind  FALSE
	#define EXP_SysSockBind  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockBind
	#define EXT_SysSockBind
	#define GET_SysSockBind(fl)  CAL_CMGETAPI( "SysSockBind" ) 
	#define CAL_SysSockBind  SysSockBind
	#define CHK_SysSockBind  TRUE
	#define EXP_SysSockBind  CAL_CMEXPAPI( "SysSockBind" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockBind
	#define EXT_SysSockBind
	#define GET_SysSockBind(fl)  CAL_CMGETAPI( "SysSockBind" ) 
	#define CAL_SysSockBind  SysSockBind
	#define CHK_SysSockBind  TRUE
	#define EXP_SysSockBind  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockBind", (RTS_UINTPTR)SysSockBind, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockBind
	#define EXT_SysSocketSysSockBind
	#define GET_SysSocketSysSockBind  ERR_OK
	#define CAL_SysSocketSysSockBind(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockBind(p1,p2) : ((ISysSocket*)p0)->ISysSockBind(p1,p2))
	#define CHK_SysSocketSysSockBind  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockBind  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockBind
	#define EXT_SysSockBind
	#define GET_SysSockBind(fl)  CAL_CMGETAPI( "SysSockBind" ) 
	#define CAL_SysSockBind(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockBind(p1,p2) : ((ISysSocket*)p0)->ISysSockBind(p1,p2))
	#define CHK_SysSockBind  (pISysSocket != NULL)
	#define EXP_SysSockBind  CAL_CMEXPAPI( "SysSockBind" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockBind  PFSYSSOCKBIND pfSysSockBind;
	#define EXT_SysSockBind  extern PFSYSSOCKBIND pfSysSockBind;
	#define GET_SysSockBind(fl)  s_pfCMGetAPI2( "SysSockBind", (RTS_VOID_FCTPTR *)&pfSysSockBind, (fl), 0, 0)
	#define CAL_SysSockBind  pfSysSockBind
	#define CHK_SysSockBind  (pfSysSockBind != NULL)
	#define EXP_SysSockBind  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockBind", (RTS_UINTPTR)SysSockBind, 0, 0) 
#endif




/**
 * <description>
 *	Get host name of the target
 * </description>
 * <param name="pszHostName" type="OUT">Pointer to get host name</param>
 * <param name="iNameLength" type="IN">Maximum length of hostname</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockGetHostName(char *pszHostName, int iNameLength);
typedef RTS_RESULT (CDECL * PFSYSSOCKGETHOSTNAME) (char *pszHostName, int iNameLength);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETHOSTNAME_NOTIMPLEMENTED)
	#define USE_SysSockGetHostName
	#define EXT_SysSockGetHostName
	#define GET_SysSockGetHostName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockGetHostName(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockGetHostName  FALSE
	#define EXP_SysSockGetHostName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockGetHostName
	#define EXT_SysSockGetHostName
	#define GET_SysSockGetHostName(fl)  CAL_CMGETAPI( "SysSockGetHostName" ) 
	#define CAL_SysSockGetHostName  SysSockGetHostName
	#define CHK_SysSockGetHostName  TRUE
	#define EXP_SysSockGetHostName  CAL_CMEXPAPI( "SysSockGetHostName" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockGetHostName
	#define EXT_SysSockGetHostName
	#define GET_SysSockGetHostName(fl)  CAL_CMGETAPI( "SysSockGetHostName" ) 
	#define CAL_SysSockGetHostName  SysSockGetHostName
	#define CHK_SysSockGetHostName  TRUE
	#define EXP_SysSockGetHostName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetHostName", (RTS_UINTPTR)SysSockGetHostName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockGetHostName
	#define EXT_SysSocketSysSockGetHostName
	#define GET_SysSocketSysSockGetHostName  ERR_OK
	#define CAL_SysSocketSysSockGetHostName pISysSocket->ISysSockGetHostName
	#define CHK_SysSocketSysSockGetHostName (pISysSocket != NULL)
	#define EXP_SysSocketSysSockGetHostName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockGetHostName
	#define EXT_SysSockGetHostName
	#define GET_SysSockGetHostName(fl)  CAL_CMGETAPI( "SysSockGetHostName" ) 
	#define CAL_SysSockGetHostName pISysSocket->ISysSockGetHostName
	#define CHK_SysSockGetHostName (pISysSocket != NULL)
	#define EXP_SysSockGetHostName  CAL_CMEXPAPI( "SysSockGetHostName" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockGetHostName  PFSYSSOCKGETHOSTNAME pfSysSockGetHostName;
	#define EXT_SysSockGetHostName  extern PFSYSSOCKGETHOSTNAME pfSysSockGetHostName;
	#define GET_SysSockGetHostName(fl)  s_pfCMGetAPI2( "SysSockGetHostName", (RTS_VOID_FCTPTR *)&pfSysSockGetHostName, (fl), 0, 0)
	#define CAL_SysSockGetHostName  pfSysSockGetHostName
	#define CHK_SysSockGetHostName  (pfSysSockGetHostName != NULL)
	#define EXP_SysSockGetHostName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetHostName", (RTS_UINTPTR)SysSockGetHostName, 0, 0) 
#endif




/**
 * <description>
 *	Get host description specified by host name
 * </description>
 * <param name="pszHostName" type="IN">Pointer to host name</param>
 * <param name="pHost" type="OUT">Pointer to host description</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockGetHostByName(char *pszHostName, SOCK_HOSTENT *pHost);
typedef RTS_RESULT (CDECL * PFSYSSOCKGETHOSTBYNAME) (char *pszHostName, SOCK_HOSTENT *pHost);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETHOSTBYNAME_NOTIMPLEMENTED)
	#define USE_SysSockGetHostByName
	#define EXT_SysSockGetHostByName
	#define GET_SysSockGetHostByName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockGetHostByName(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockGetHostByName  FALSE
	#define EXP_SysSockGetHostByName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockGetHostByName
	#define EXT_SysSockGetHostByName
	#define GET_SysSockGetHostByName(fl)  CAL_CMGETAPI( "SysSockGetHostByName" ) 
	#define CAL_SysSockGetHostByName  SysSockGetHostByName
	#define CHK_SysSockGetHostByName  TRUE
	#define EXP_SysSockGetHostByName  CAL_CMEXPAPI( "SysSockGetHostByName" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockGetHostByName
	#define EXT_SysSockGetHostByName
	#define GET_SysSockGetHostByName(fl)  CAL_CMGETAPI( "SysSockGetHostByName" ) 
	#define CAL_SysSockGetHostByName  SysSockGetHostByName
	#define CHK_SysSockGetHostByName  TRUE
	#define EXP_SysSockGetHostByName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetHostByName", (RTS_UINTPTR)SysSockGetHostByName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockGetHostByName
	#define EXT_SysSocketSysSockGetHostByName
	#define GET_SysSocketSysSockGetHostByName  ERR_OK
	#define CAL_SysSocketSysSockGetHostByName pISysSocket->ISysSockGetHostByName
	#define CHK_SysSocketSysSockGetHostByName (pISysSocket != NULL)
	#define EXP_SysSocketSysSockGetHostByName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockGetHostByName
	#define EXT_SysSockGetHostByName
	#define GET_SysSockGetHostByName(fl)  CAL_CMGETAPI( "SysSockGetHostByName" ) 
	#define CAL_SysSockGetHostByName pISysSocket->ISysSockGetHostByName
	#define CHK_SysSockGetHostByName (pISysSocket != NULL)
	#define EXP_SysSockGetHostByName  CAL_CMEXPAPI( "SysSockGetHostByName" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockGetHostByName  PFSYSSOCKGETHOSTBYNAME pfSysSockGetHostByName;
	#define EXT_SysSockGetHostByName  extern PFSYSSOCKGETHOSTBYNAME pfSysSockGetHostByName;
	#define GET_SysSockGetHostByName(fl)  s_pfCMGetAPI2( "SysSockGetHostByName", (RTS_VOID_FCTPTR *)&pfSysSockGetHostByName, (fl), 0, 0)
	#define CAL_SysSockGetHostByName  pfSysSockGetHostByName
	#define CHK_SysSockGetHostByName  (pfSysSockGetHostByName != NULL)
	#define EXP_SysSockGetHostByName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetHostByName", (RTS_UINTPTR)SysSockGetHostByName, 0, 0) 
#endif




/**
 * <description>
 *	Convert IP address to a string	  
 * </description>
 * <param name="pInAddr" type="IN">Pointer to IP address description</param>
 * <param name="pszIPAddr" type="OUT">Pointer to get IP address string (must be at least 16 bytes long)</param>
 * <param name="iIPAddrSize" type="IN">Maximum length of pszIPAddr</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockInetNtoa(INADDR *pInAddr, char *pszIPAddr, int iIPAddrSize);
typedef RTS_RESULT (CDECL * PFSYSSOCKINETNTOA) (INADDR *pInAddr, char *pszIPAddr, int iIPAddrSize);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKINETNTOA_NOTIMPLEMENTED)
	#define USE_SysSockInetNtoa
	#define EXT_SysSockInetNtoa
	#define GET_SysSockInetNtoa(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockInetNtoa(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockInetNtoa  FALSE
	#define EXP_SysSockInetNtoa  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockInetNtoa
	#define EXT_SysSockInetNtoa
	#define GET_SysSockInetNtoa(fl)  CAL_CMGETAPI( "SysSockInetNtoa" ) 
	#define CAL_SysSockInetNtoa  SysSockInetNtoa
	#define CHK_SysSockInetNtoa  TRUE
	#define EXP_SysSockInetNtoa  CAL_CMEXPAPI( "SysSockInetNtoa" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockInetNtoa
	#define EXT_SysSockInetNtoa
	#define GET_SysSockInetNtoa(fl)  CAL_CMGETAPI( "SysSockInetNtoa" ) 
	#define CAL_SysSockInetNtoa  SysSockInetNtoa
	#define CHK_SysSockInetNtoa  TRUE
	#define EXP_SysSockInetNtoa  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockInetNtoa", (RTS_UINTPTR)SysSockInetNtoa, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockInetNtoa
	#define EXT_SysSocketSysSockInetNtoa
	#define GET_SysSocketSysSockInetNtoa  ERR_OK
	#define CAL_SysSocketSysSockInetNtoa pISysSocket->ISysSockInetNtoa
	#define CHK_SysSocketSysSockInetNtoa (pISysSocket != NULL)
	#define EXP_SysSocketSysSockInetNtoa  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockInetNtoa
	#define EXT_SysSockInetNtoa
	#define GET_SysSockInetNtoa(fl)  CAL_CMGETAPI( "SysSockInetNtoa" ) 
	#define CAL_SysSockInetNtoa pISysSocket->ISysSockInetNtoa
	#define CHK_SysSockInetNtoa (pISysSocket != NULL)
	#define EXP_SysSockInetNtoa  CAL_CMEXPAPI( "SysSockInetNtoa" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockInetNtoa  PFSYSSOCKINETNTOA pfSysSockInetNtoa;
	#define EXT_SysSockInetNtoa  extern PFSYSSOCKINETNTOA pfSysSockInetNtoa;
	#define GET_SysSockInetNtoa(fl)  s_pfCMGetAPI2( "SysSockInetNtoa", (RTS_VOID_FCTPTR *)&pfSysSockInetNtoa, (fl), 0, 0)
	#define CAL_SysSockInetNtoa  pfSysSockInetNtoa
	#define CHK_SysSockInetNtoa  (pfSysSockInetNtoa != NULL)
	#define EXP_SysSockInetNtoa  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockInetNtoa", (RTS_UINTPTR)SysSockInetNtoa, 0, 0) 
#endif




/**
 * <description>
 *	Convert an IP address string into an IP address
 * </description>
 * <param name="pszIPAddr" type="IN">Pointer to get IP address string (must be at least 16 bytes long)</param>
 * <param name="pInAddr" type="OUT">Pointer to IP address description (in ethernet byte order)</param>
 * <result>Error code:
 *	<ul>
 *		<li>ERR_PARAMETER: if pszIPAddress=NULL or pInAddr=NULL</li>
 *		<li>ERR_OK: IP-address could be converted
 *			IMPLEMENTATION NOTE:
 *			If pszIPAddress="255.255.255.255", the error code must be ERR_OK with *pInAddr=0xFFFFFFFF (SOCKET_INADDR_BROADCAST).</li>
 *		<li>ERR_FAILED: IP-address invalid or empty
 *			IMPLEMENTATION NOTE:
 *			If pszIPAddress="", the error code must be ERR_FAILED with *pInAddr=0xFFFFFFFF (SOCKET_INADDR_NONE).</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL SysSockInetAddr(char *pszIPAddress, RTS_UI32 *pInAddr);
typedef RTS_RESULT (CDECL * PFSYSSOCKINETADDR) (char *pszIPAddress, RTS_UI32 *pInAddr);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKINETADDR_NOTIMPLEMENTED)
	#define USE_SysSockInetAddr
	#define EXT_SysSockInetAddr
	#define GET_SysSockInetAddr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockInetAddr(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockInetAddr  FALSE
	#define EXP_SysSockInetAddr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockInetAddr
	#define EXT_SysSockInetAddr
	#define GET_SysSockInetAddr(fl)  CAL_CMGETAPI( "SysSockInetAddr" ) 
	#define CAL_SysSockInetAddr  SysSockInetAddr
	#define CHK_SysSockInetAddr  TRUE
	#define EXP_SysSockInetAddr  CAL_CMEXPAPI( "SysSockInetAddr" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockInetAddr
	#define EXT_SysSockInetAddr
	#define GET_SysSockInetAddr(fl)  CAL_CMGETAPI( "SysSockInetAddr" ) 
	#define CAL_SysSockInetAddr  SysSockInetAddr
	#define CHK_SysSockInetAddr  TRUE
	#define EXP_SysSockInetAddr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockInetAddr", (RTS_UINTPTR)SysSockInetAddr, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockInetAddr
	#define EXT_SysSocketSysSockInetAddr
	#define GET_SysSocketSysSockInetAddr  ERR_OK
	#define CAL_SysSocketSysSockInetAddr pISysSocket->ISysSockInetAddr
	#define CHK_SysSocketSysSockInetAddr (pISysSocket != NULL)
	#define EXP_SysSocketSysSockInetAddr  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockInetAddr
	#define EXT_SysSockInetAddr
	#define GET_SysSockInetAddr(fl)  CAL_CMGETAPI( "SysSockInetAddr" ) 
	#define CAL_SysSockInetAddr pISysSocket->ISysSockInetAddr
	#define CHK_SysSockInetAddr (pISysSocket != NULL)
	#define EXP_SysSockInetAddr  CAL_CMEXPAPI( "SysSockInetAddr" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockInetAddr  PFSYSSOCKINETADDR pfSysSockInetAddr;
	#define EXT_SysSockInetAddr  extern PFSYSSOCKINETADDR pfSysSockInetAddr;
	#define GET_SysSockInetAddr(fl)  s_pfCMGetAPI2( "SysSockInetAddr", (RTS_VOID_FCTPTR *)&pfSysSockInetAddr, (fl), 0, 0)
	#define CAL_SysSockInetAddr  pfSysSockInetAddr
	#define CHK_SysSockInetAddr  (pfSysSockInetAddr != NULL)
	#define EXP_SysSockInetAddr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockInetAddr", (RTS_UINTPTR)SysSockInetAddr, 0, 0) 
#endif




/**
 * <description>
 *	Convert a host unsigned short value into the ethernet byte order
 * </description>
 * <param name="usHost" type="IN">Host unsigned short value</param>
 * <result>Returns the converted unsigned short value</result>
 */
unsigned short CDECL SysSockHtons(unsigned short usHost);
typedef unsigned short (CDECL * PFSYSSOCKHTONS) (unsigned short usHost);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKHTONS_NOTIMPLEMENTED)
	#define USE_SysSockHtons
	#define EXT_SysSockHtons
	#define GET_SysSockHtons(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockHtons(p0)  (unsigned short)ERR_NOTIMPLEMENTED
	#define CHK_SysSockHtons  FALSE
	#define EXP_SysSockHtons  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockHtons
	#define EXT_SysSockHtons
	#define GET_SysSockHtons(fl)  CAL_CMGETAPI( "SysSockHtons" ) 
	#define CAL_SysSockHtons  SysSockHtons
	#define CHK_SysSockHtons  TRUE
	#define EXP_SysSockHtons  CAL_CMEXPAPI( "SysSockHtons" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockHtons
	#define EXT_SysSockHtons
	#define GET_SysSockHtons(fl)  CAL_CMGETAPI( "SysSockHtons" ) 
	#define CAL_SysSockHtons  SysSockHtons
	#define CHK_SysSockHtons  TRUE
	#define EXP_SysSockHtons  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockHtons", (RTS_UINTPTR)SysSockHtons, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockHtons
	#define EXT_SysSocketSysSockHtons
	#define GET_SysSocketSysSockHtons  ERR_OK
	#define CAL_SysSocketSysSockHtons pISysSocket->ISysSockHtons
	#define CHK_SysSocketSysSockHtons (pISysSocket != NULL)
	#define EXP_SysSocketSysSockHtons  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockHtons
	#define EXT_SysSockHtons
	#define GET_SysSockHtons(fl)  CAL_CMGETAPI( "SysSockHtons" ) 
	#define CAL_SysSockHtons pISysSocket->ISysSockHtons
	#define CHK_SysSockHtons (pISysSocket != NULL)
	#define EXP_SysSockHtons  CAL_CMEXPAPI( "SysSockHtons" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockHtons  PFSYSSOCKHTONS pfSysSockHtons;
	#define EXT_SysSockHtons  extern PFSYSSOCKHTONS pfSysSockHtons;
	#define GET_SysSockHtons(fl)  s_pfCMGetAPI2( "SysSockHtons", (RTS_VOID_FCTPTR *)&pfSysSockHtons, (fl), 0, 0)
	#define CAL_SysSockHtons  pfSysSockHtons
	#define CHK_SysSockHtons  (pfSysSockHtons != NULL)
	#define EXP_SysSockHtons  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockHtons", (RTS_UINTPTR)SysSockHtons, 0, 0) 
#endif




/**
 * <description>
 *	Convert a host unsigned long value into the ethernet byte order
 * </description>
 * <param name="usHost" type="IN">Host unsigned long value</param>
 * <result>Returns the converted unsigned long value</result>
 */
RTS_UI32 CDECL SysSockHtonl(RTS_UI32 ulHost);
typedef RTS_UI32 (CDECL * PFSYSSOCKHTONL) (RTS_UI32 ulHost);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKHTONL_NOTIMPLEMENTED)
	#define USE_SysSockHtonl
	#define EXT_SysSockHtonl
	#define GET_SysSockHtonl(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockHtonl(p0)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SysSockHtonl  FALSE
	#define EXP_SysSockHtonl  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockHtonl
	#define EXT_SysSockHtonl
	#define GET_SysSockHtonl(fl)  CAL_CMGETAPI( "SysSockHtonl" ) 
	#define CAL_SysSockHtonl  SysSockHtonl
	#define CHK_SysSockHtonl  TRUE
	#define EXP_SysSockHtonl  CAL_CMEXPAPI( "SysSockHtonl" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockHtonl
	#define EXT_SysSockHtonl
	#define GET_SysSockHtonl(fl)  CAL_CMGETAPI( "SysSockHtonl" ) 
	#define CAL_SysSockHtonl  SysSockHtonl
	#define CHK_SysSockHtonl  TRUE
	#define EXP_SysSockHtonl  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockHtonl", (RTS_UINTPTR)SysSockHtonl, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockHtonl
	#define EXT_SysSocketSysSockHtonl
	#define GET_SysSocketSysSockHtonl  ERR_OK
	#define CAL_SysSocketSysSockHtonl pISysSocket->ISysSockHtonl
	#define CHK_SysSocketSysSockHtonl (pISysSocket != NULL)
	#define EXP_SysSocketSysSockHtonl  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockHtonl
	#define EXT_SysSockHtonl
	#define GET_SysSockHtonl(fl)  CAL_CMGETAPI( "SysSockHtonl" ) 
	#define CAL_SysSockHtonl pISysSocket->ISysSockHtonl
	#define CHK_SysSockHtonl (pISysSocket != NULL)
	#define EXP_SysSockHtonl  CAL_CMEXPAPI( "SysSockHtonl" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockHtonl  PFSYSSOCKHTONL pfSysSockHtonl;
	#define EXT_SysSockHtonl  extern PFSYSSOCKHTONL pfSysSockHtonl;
	#define GET_SysSockHtonl(fl)  s_pfCMGetAPI2( "SysSockHtonl", (RTS_VOID_FCTPTR *)&pfSysSockHtonl, (fl), 0, 0)
	#define CAL_SysSockHtonl  pfSysSockHtonl
	#define CHK_SysSockHtonl  (pfSysSockHtonl != NULL)
	#define EXP_SysSockHtonl  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockHtonl", (RTS_UINTPTR)SysSockHtonl, 0, 0) 
#endif




/**
 * <description>
 *	Convert a unsigned short value from ethernet byte order into host format
 * </description>
 * <param name="usNet" type="IN">Ethernet unsigned short value</param>
 * <result>Returns the converted unsigned short value</result>
 */
unsigned short CDECL SysSockNtohs(unsigned short usNet);
typedef unsigned short (CDECL * PFSYSSOCKNTOHS) (unsigned short usNet);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKNTOHS_NOTIMPLEMENTED)
	#define USE_SysSockNtohs
	#define EXT_SysSockNtohs
	#define GET_SysSockNtohs(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockNtohs(p0)  (unsigned short)ERR_NOTIMPLEMENTED
	#define CHK_SysSockNtohs  FALSE
	#define EXP_SysSockNtohs  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockNtohs
	#define EXT_SysSockNtohs
	#define GET_SysSockNtohs(fl)  CAL_CMGETAPI( "SysSockNtohs" ) 
	#define CAL_SysSockNtohs  SysSockNtohs
	#define CHK_SysSockNtohs  TRUE
	#define EXP_SysSockNtohs  CAL_CMEXPAPI( "SysSockNtohs" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockNtohs
	#define EXT_SysSockNtohs
	#define GET_SysSockNtohs(fl)  CAL_CMGETAPI( "SysSockNtohs" ) 
	#define CAL_SysSockNtohs  SysSockNtohs
	#define CHK_SysSockNtohs  TRUE
	#define EXP_SysSockNtohs  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockNtohs", (RTS_UINTPTR)SysSockNtohs, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockNtohs
	#define EXT_SysSocketSysSockNtohs
	#define GET_SysSocketSysSockNtohs  ERR_OK
	#define CAL_SysSocketSysSockNtohs pISysSocket->ISysSockNtohs
	#define CHK_SysSocketSysSockNtohs (pISysSocket != NULL)
	#define EXP_SysSocketSysSockNtohs  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockNtohs
	#define EXT_SysSockNtohs
	#define GET_SysSockNtohs(fl)  CAL_CMGETAPI( "SysSockNtohs" ) 
	#define CAL_SysSockNtohs pISysSocket->ISysSockNtohs
	#define CHK_SysSockNtohs (pISysSocket != NULL)
	#define EXP_SysSockNtohs  CAL_CMEXPAPI( "SysSockNtohs" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockNtohs  PFSYSSOCKNTOHS pfSysSockNtohs;
	#define EXT_SysSockNtohs  extern PFSYSSOCKNTOHS pfSysSockNtohs;
	#define GET_SysSockNtohs(fl)  s_pfCMGetAPI2( "SysSockNtohs", (RTS_VOID_FCTPTR *)&pfSysSockNtohs, (fl), 0, 0)
	#define CAL_SysSockNtohs  pfSysSockNtohs
	#define CHK_SysSockNtohs  (pfSysSockNtohs != NULL)
	#define EXP_SysSockNtohs  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockNtohs", (RTS_UINTPTR)SysSockNtohs, 0, 0) 
#endif




/**
 * <description>
 *	Convert a unsigned long value from ethernet byte order into host format
 * </description>
 * <param name="usNet" type="IN">Ethernet unsigned long value</param>
 * <result>Returns the converted unsigned long value</result>
 */
RTS_UI32 CDECL SysSockNtohl(RTS_UI32 ulNet);
typedef RTS_UI32 (CDECL * PFSYSSOCKNTOHL) (RTS_UI32 ulNet);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKNTOHL_NOTIMPLEMENTED)
	#define USE_SysSockNtohl
	#define EXT_SysSockNtohl
	#define GET_SysSockNtohl(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockNtohl(p0)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SysSockNtohl  FALSE
	#define EXP_SysSockNtohl  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockNtohl
	#define EXT_SysSockNtohl
	#define GET_SysSockNtohl(fl)  CAL_CMGETAPI( "SysSockNtohl" ) 
	#define CAL_SysSockNtohl  SysSockNtohl
	#define CHK_SysSockNtohl  TRUE
	#define EXP_SysSockNtohl  CAL_CMEXPAPI( "SysSockNtohl" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockNtohl
	#define EXT_SysSockNtohl
	#define GET_SysSockNtohl(fl)  CAL_CMGETAPI( "SysSockNtohl" ) 
	#define CAL_SysSockNtohl  SysSockNtohl
	#define CHK_SysSockNtohl  TRUE
	#define EXP_SysSockNtohl  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockNtohl", (RTS_UINTPTR)SysSockNtohl, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockNtohl
	#define EXT_SysSocketSysSockNtohl
	#define GET_SysSocketSysSockNtohl  ERR_OK
	#define CAL_SysSocketSysSockNtohl pISysSocket->ISysSockNtohl
	#define CHK_SysSocketSysSockNtohl (pISysSocket != NULL)
	#define EXP_SysSocketSysSockNtohl  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockNtohl
	#define EXT_SysSockNtohl
	#define GET_SysSockNtohl(fl)  CAL_CMGETAPI( "SysSockNtohl" ) 
	#define CAL_SysSockNtohl pISysSocket->ISysSockNtohl
	#define CHK_SysSockNtohl (pISysSocket != NULL)
	#define EXP_SysSockNtohl  CAL_CMEXPAPI( "SysSockNtohl" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockNtohl  PFSYSSOCKNTOHL pfSysSockNtohl;
	#define EXT_SysSockNtohl  extern PFSYSSOCKNTOHL pfSysSockNtohl;
	#define GET_SysSockNtohl(fl)  s_pfCMGetAPI2( "SysSockNtohl", (RTS_VOID_FCTPTR *)&pfSysSockNtohl, (fl), 0, 0)
	#define CAL_SysSockNtohl  pfSysSockNtohl
	#define CHK_SysSockNtohl  (pfSysSockNtohl != NULL)
	#define EXP_SysSockNtohl  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockNtohl", (RTS_UINTPTR)SysSockNtohl, 0, 0) 
#endif




/**
 * <description>
 *	Listen on a TCP server socket for new connection
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="iMaxConnections" type="IN">Maximum number of connections allowed</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockListen(RTS_HANDLE hSocket, int iMaxConnections);
typedef RTS_RESULT (CDECL * PFSYSSOCKLISTEN) (RTS_HANDLE hSocket, int iMaxConnections);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKLISTEN_NOTIMPLEMENTED)
	#define USE_SysSockListen
	#define EXT_SysSockListen
	#define GET_SysSockListen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockListen(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockListen  FALSE
	#define EXP_SysSockListen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockListen
	#define EXT_SysSockListen
	#define GET_SysSockListen(fl)  CAL_CMGETAPI( "SysSockListen" ) 
	#define CAL_SysSockListen  SysSockListen
	#define CHK_SysSockListen  TRUE
	#define EXP_SysSockListen  CAL_CMEXPAPI( "SysSockListen" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockListen
	#define EXT_SysSockListen
	#define GET_SysSockListen(fl)  CAL_CMGETAPI( "SysSockListen" ) 
	#define CAL_SysSockListen  SysSockListen
	#define CHK_SysSockListen  TRUE
	#define EXP_SysSockListen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockListen", (RTS_UINTPTR)SysSockListen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockListen
	#define EXT_SysSocketSysSockListen
	#define GET_SysSocketSysSockListen  ERR_OK
	#define CAL_SysSocketSysSockListen(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockListen(p1) : ((ISysSocket*)p0)->ISysSockListen(p1))
	#define CHK_SysSocketSysSockListen  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockListen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockListen
	#define EXT_SysSockListen
	#define GET_SysSockListen(fl)  CAL_CMGETAPI( "SysSockListen" ) 
	#define CAL_SysSockListen(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockListen(p1) : ((ISysSocket*)p0)->ISysSockListen(p1))
	#define CHK_SysSockListen  (pISysSocket != NULL)
	#define EXP_SysSockListen  CAL_CMEXPAPI( "SysSockListen" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockListen  PFSYSSOCKLISTEN pfSysSockListen;
	#define EXT_SysSockListen  extern PFSYSSOCKLISTEN pfSysSockListen;
	#define GET_SysSockListen(fl)  s_pfCMGetAPI2( "SysSockListen", (RTS_VOID_FCTPTR *)&pfSysSockListen, (fl), 0, 0)
	#define CAL_SysSockListen  pfSysSockListen
	#define CHK_SysSockListen  (pfSysSockListen != NULL)
	#define EXP_SysSockListen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockListen", (RTS_UINTPTR)SysSockListen, 0, 0) 
#endif




/**
 * <description>
 *	Accept the next incoming TCP connection. Returns the socket for the newly created
 *	connection or RTS_INVALID_HANDLE if failed.
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pSockAddr" type="OUT">Socket address of the client, who is connected</param>
 * <param name="piSockAddrSize" type="INOUT">Pointer to size of socket address structure</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the new accepted socket</result>
 */
RTS_HANDLE CDECL SysSockAccept(RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, int *piSockAddrSize, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSSOCKACCEPT) (RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, int *piSockAddrSize, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKACCEPT_NOTIMPLEMENTED)
	#define USE_SysSockAccept
	#define EXT_SysSockAccept
	#define GET_SysSockAccept(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockAccept(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysSockAccept  FALSE
	#define EXP_SysSockAccept  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockAccept
	#define EXT_SysSockAccept
	#define GET_SysSockAccept(fl)  CAL_CMGETAPI( "SysSockAccept" ) 
	#define CAL_SysSockAccept  SysSockAccept
	#define CHK_SysSockAccept  TRUE
	#define EXP_SysSockAccept  CAL_CMEXPAPI( "SysSockAccept" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockAccept
	#define EXT_SysSockAccept
	#define GET_SysSockAccept(fl)  CAL_CMGETAPI( "SysSockAccept" ) 
	#define CAL_SysSockAccept  SysSockAccept
	#define CHK_SysSockAccept  TRUE
	#define EXP_SysSockAccept  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockAccept", (RTS_UINTPTR)SysSockAccept, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockAccept
	#define EXT_SysSocketSysSockAccept
	#define GET_SysSocketSysSockAccept  ERR_OK
	#define CAL_SysSocketSysSockAccept(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockAccept(p1,p2,p3) : ((ISysSocket*)p0)->ISysSockAccept(p1,p2,p3))
	#define CHK_SysSocketSysSockAccept  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockAccept  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockAccept
	#define EXT_SysSockAccept
	#define GET_SysSockAccept(fl)  CAL_CMGETAPI( "SysSockAccept" ) 
	#define CAL_SysSockAccept(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockAccept(p1,p2,p3) : ((ISysSocket*)p0)->ISysSockAccept(p1,p2,p3))
	#define CHK_SysSockAccept  (pISysSocket != NULL)
	#define EXP_SysSockAccept  CAL_CMEXPAPI( "SysSockAccept" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockAccept  PFSYSSOCKACCEPT pfSysSockAccept;
	#define EXT_SysSockAccept  extern PFSYSSOCKACCEPT pfSysSockAccept;
	#define GET_SysSockAccept(fl)  s_pfCMGetAPI2( "SysSockAccept", (RTS_VOID_FCTPTR *)&pfSysSockAccept, (fl), 0, 0)
	#define CAL_SysSockAccept  pfSysSockAccept
	#define CHK_SysSockAccept  (pfSysSockAccept != NULL)
	#define EXP_SysSockAccept  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockAccept", (RTS_UINTPTR)SysSockAccept, 0, 0) 
#endif




/**
 * <description>
 *	Set IP address of the specified ethernet device. Is not available on all platforms!
 *  Use SysSockSetIpAddressAndNetMask() instead.
 * </description>
 * <param name="pszCard" type="IN">Name of the ethernet card</param>
 * <param name="pszIPAddress" type="IN">IP address to set as string</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockSetIPAddress(char *pszCard, char *pszIPAddress);
typedef RTS_RESULT (CDECL * PFSYSSOCKSETIPADDRESS) (char *pszCard, char *pszIPAddress);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSETIPADDRESS_NOTIMPLEMENTED)
	#define USE_SysSockSetIPAddress
	#define EXT_SysSockSetIPAddress
	#define GET_SysSockSetIPAddress(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockSetIPAddress(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockSetIPAddress  FALSE
	#define EXP_SysSockSetIPAddress  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockSetIPAddress
	#define EXT_SysSockSetIPAddress
	#define GET_SysSockSetIPAddress(fl)  CAL_CMGETAPI( "SysSockSetIPAddress" ) 
	#define CAL_SysSockSetIPAddress  SysSockSetIPAddress
	#define CHK_SysSockSetIPAddress  TRUE
	#define EXP_SysSockSetIPAddress  CAL_CMEXPAPI( "SysSockSetIPAddress" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockSetIPAddress
	#define EXT_SysSockSetIPAddress
	#define GET_SysSockSetIPAddress(fl)  CAL_CMGETAPI( "SysSockSetIPAddress" ) 
	#define CAL_SysSockSetIPAddress  SysSockSetIPAddress
	#define CHK_SysSockSetIPAddress  TRUE
	#define EXP_SysSockSetIPAddress  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSetIPAddress", (RTS_UINTPTR)SysSockSetIPAddress, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockSetIPAddress
	#define EXT_SysSocketSysSockSetIPAddress
	#define GET_SysSocketSysSockSetIPAddress  ERR_OK
	#define CAL_SysSocketSysSockSetIPAddress pISysSocket->ISysSockSetIPAddress
	#define CHK_SysSocketSysSockSetIPAddress (pISysSocket != NULL)
	#define EXP_SysSocketSysSockSetIPAddress  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockSetIPAddress
	#define EXT_SysSockSetIPAddress
	#define GET_SysSockSetIPAddress(fl)  CAL_CMGETAPI( "SysSockSetIPAddress" ) 
	#define CAL_SysSockSetIPAddress pISysSocket->ISysSockSetIPAddress
	#define CHK_SysSockSetIPAddress (pISysSocket != NULL)
	#define EXP_SysSockSetIPAddress  CAL_CMEXPAPI( "SysSockSetIPAddress" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockSetIPAddress  PFSYSSOCKSETIPADDRESS pfSysSockSetIPAddress;
	#define EXT_SysSockSetIPAddress  extern PFSYSSOCKSETIPADDRESS pfSysSockSetIPAddress;
	#define GET_SysSockSetIPAddress(fl)  s_pfCMGetAPI2( "SysSockSetIPAddress", (RTS_VOID_FCTPTR *)&pfSysSockSetIPAddress, (fl), 0, 0)
	#define CAL_SysSockSetIPAddress  pfSysSockSetIPAddress
	#define CHK_SysSockSetIPAddress  (pfSysSockSetIPAddress != NULL)
	#define EXP_SysSockSetIPAddress  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSetIPAddress", (RTS_UINTPTR)SysSockSetIPAddress, 0, 0) 
#endif




/**
 * <description>
 *	Connect as a client to a TCP server
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pSockAddr" type="IN">Socket address of the server to connect to</param>
 * <param name="iSockAddrSize" type="IN">Size of socket address structure</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockConnect(RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, int iSockAddrSize);
typedef RTS_RESULT (CDECL * PFSYSSOCKCONNECT) (RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, int iSockAddrSize);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKCONNECT_NOTIMPLEMENTED)
	#define USE_SysSockConnect
	#define EXT_SysSockConnect
	#define GET_SysSockConnect(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockConnect(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockConnect  FALSE
	#define EXP_SysSockConnect  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockConnect
	#define EXT_SysSockConnect
	#define GET_SysSockConnect(fl)  CAL_CMGETAPI( "SysSockConnect" ) 
	#define CAL_SysSockConnect  SysSockConnect
	#define CHK_SysSockConnect  TRUE
	#define EXP_SysSockConnect  CAL_CMEXPAPI( "SysSockConnect" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockConnect
	#define EXT_SysSockConnect
	#define GET_SysSockConnect(fl)  CAL_CMGETAPI( "SysSockConnect" ) 
	#define CAL_SysSockConnect  SysSockConnect
	#define CHK_SysSockConnect  TRUE
	#define EXP_SysSockConnect  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockConnect", (RTS_UINTPTR)SysSockConnect, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockConnect
	#define EXT_SysSocketSysSockConnect
	#define GET_SysSocketSysSockConnect  ERR_OK
	#define CAL_SysSocketSysSockConnect(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockConnect(p1,p2) : ((ISysSocket*)p0)->ISysSockConnect(p1,p2))
	#define CHK_SysSocketSysSockConnect  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockConnect  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockConnect
	#define EXT_SysSockConnect
	#define GET_SysSockConnect(fl)  CAL_CMGETAPI( "SysSockConnect" ) 
	#define CAL_SysSockConnect(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockConnect(p1,p2) : ((ISysSocket*)p0)->ISysSockConnect(p1,p2))
	#define CHK_SysSockConnect  (pISysSocket != NULL)
	#define EXP_SysSockConnect  CAL_CMEXPAPI( "SysSockConnect" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockConnect  PFSYSSOCKCONNECT pfSysSockConnect;
	#define EXT_SysSockConnect  extern PFSYSSOCKCONNECT pfSysSockConnect;
	#define GET_SysSockConnect(fl)  s_pfCMGetAPI2( "SysSockConnect", (RTS_VOID_FCTPTR *)&pfSysSockConnect, (fl), 0, 0)
	#define CAL_SysSockConnect  pfSysSockConnect
	#define CHK_SysSockConnect  (pfSysSockConnect != NULL)
	#define EXP_SysSockConnect  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockConnect", (RTS_UINTPTR)SysSockConnect, 0, 0) 
#endif




/**
 * <description>
 *	Check a number of sockets for activity	
 * </description>
 * <param name="iWidth" type="IN">Number of sockets in the SOCKET_FD_SET structure, so SOCKET_FD_SETSIZE must be used here.</param>
 * <param name="fdRead" type="IN">Read socket</param>
 * <param name="fdWrite" type="IN">Write socket</param>
 * <param name="fdExcept" type="IN">Exception socket</param>
 * <param name="ptvTimeout" type="IN">Pointer to specify the timeout of the operation.
 *	ptvTimeout=NULL:	Infinite wait
 *	ptvTimeout->tv_sec=-1, ptvTimeout->tv_usec=-1:	Infinite wait
 *	ptvTimeout->tv_sec=0, ptvTimeout->tv_usec=0:	No wait</param>
 * <param name="pnReady" type="OUT">Number of sockets that are ready for IO</param>
 * <result>ERR_OK or ERR_SOCK_TIMEDOUT, if timeout expired</result>
 */
RTS_RESULT CDECL SysSockSelect(int iWidth, SOCKET_FD_SET *fdRead, SOCKET_FD_SET *fdWrite, SOCKET_FD_SET *fdExcept, SOCKET_TIMEVAL *ptvTimeout, int *pnReady);
typedef RTS_RESULT (CDECL * PFSYSSOCKSELECT) (int iWidth, SOCKET_FD_SET *fdRead, SOCKET_FD_SET *fdWrite, SOCKET_FD_SET *fdExcept, SOCKET_TIMEVAL *ptvTimeout, int *pnReady);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSELECT_NOTIMPLEMENTED)
	#define USE_SysSockSelect
	#define EXT_SysSockSelect
	#define GET_SysSockSelect(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockSelect(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockSelect  FALSE
	#define EXP_SysSockSelect  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockSelect
	#define EXT_SysSockSelect
	#define GET_SysSockSelect(fl)  CAL_CMGETAPI( "SysSockSelect" ) 
	#define CAL_SysSockSelect  SysSockSelect
	#define CHK_SysSockSelect  TRUE
	#define EXP_SysSockSelect  CAL_CMEXPAPI( "SysSockSelect" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockSelect
	#define EXT_SysSockSelect
	#define GET_SysSockSelect(fl)  CAL_CMGETAPI( "SysSockSelect" ) 
	#define CAL_SysSockSelect  SysSockSelect
	#define CHK_SysSockSelect  TRUE
	#define EXP_SysSockSelect  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSelect", (RTS_UINTPTR)SysSockSelect, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockSelect
	#define EXT_SysSocketSysSockSelect
	#define GET_SysSocketSysSockSelect  ERR_OK
	#define CAL_SysSocketSysSockSelect pISysSocket->ISysSockSelect
	#define CHK_SysSocketSysSockSelect (pISysSocket != NULL)
	#define EXP_SysSocketSysSockSelect  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockSelect
	#define EXT_SysSockSelect
	#define GET_SysSockSelect(fl)  CAL_CMGETAPI( "SysSockSelect" ) 
	#define CAL_SysSockSelect pISysSocket->ISysSockSelect
	#define CHK_SysSockSelect (pISysSocket != NULL)
	#define EXP_SysSockSelect  CAL_CMEXPAPI( "SysSockSelect" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockSelect  PFSYSSOCKSELECT pfSysSockSelect;
	#define EXT_SysSockSelect  extern PFSYSSOCKSELECT pfSysSockSelect;
	#define GET_SysSockSelect(fl)  s_pfCMGetAPI2( "SysSockSelect", (RTS_VOID_FCTPTR *)&pfSysSockSelect, (fl), 0, 0)
	#define CAL_SysSockSelect  pfSysSockSelect
	#define CHK_SysSockSelect  (pfSysSockSelect != NULL)
	#define EXP_SysSockSelect  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSelect", (RTS_UINTPTR)SysSockSelect, 0, 0) 
#endif




/**
 * <description>
 *	Shutdown a socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="iHow" type="IN">Specified, which operations are no longer be allowed. See category shutdown flags</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockShutdown(RTS_HANDLE hSocket, int iHow);
typedef RTS_RESULT (CDECL * PFSYSSOCKSHUTDOWN) (RTS_HANDLE hSocket, int iHow);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSHUTDOWN_NOTIMPLEMENTED)
	#define USE_SysSockShutdown
	#define EXT_SysSockShutdown
	#define GET_SysSockShutdown(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockShutdown(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockShutdown  FALSE
	#define EXP_SysSockShutdown  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockShutdown
	#define EXT_SysSockShutdown
	#define GET_SysSockShutdown(fl)  CAL_CMGETAPI( "SysSockShutdown" ) 
	#define CAL_SysSockShutdown  SysSockShutdown
	#define CHK_SysSockShutdown  TRUE
	#define EXP_SysSockShutdown  CAL_CMEXPAPI( "SysSockShutdown" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockShutdown
	#define EXT_SysSockShutdown
	#define GET_SysSockShutdown(fl)  CAL_CMGETAPI( "SysSockShutdown" ) 
	#define CAL_SysSockShutdown  SysSockShutdown
	#define CHK_SysSockShutdown  TRUE
	#define EXP_SysSockShutdown  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockShutdown", (RTS_UINTPTR)SysSockShutdown, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockShutdown
	#define EXT_SysSocketSysSockShutdown
	#define GET_SysSocketSysSockShutdown  ERR_OK
	#define CAL_SysSocketSysSockShutdown(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockShutdown(p1) : ((ISysSocket*)p0)->ISysSockShutdown(p1))
	#define CHK_SysSocketSysSockShutdown  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockShutdown  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockShutdown
	#define EXT_SysSockShutdown
	#define GET_SysSockShutdown(fl)  CAL_CMGETAPI( "SysSockShutdown" ) 
	#define CAL_SysSockShutdown(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockShutdown(p1) : ((ISysSocket*)p0)->ISysSockShutdown(p1))
	#define CHK_SysSockShutdown  (pISysSocket != NULL)
	#define EXP_SysSockShutdown  CAL_CMEXPAPI( "SysSockShutdown" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockShutdown  PFSYSSOCKSHUTDOWN pfSysSockShutdown;
	#define EXT_SysSockShutdown  extern PFSYSSOCKSHUTDOWN pfSysSockShutdown;
	#define GET_SysSockShutdown(fl)  s_pfCMGetAPI2( "SysSockShutdown", (RTS_VOID_FCTPTR *)&pfSysSockShutdown, (fl), 0, 0)
	#define CAL_SysSockShutdown  pfSysSockShutdown
	#define CHK_SysSockShutdown  (pfSysSockShutdown != NULL)
	#define EXP_SysSockShutdown  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockShutdown", (RTS_UINTPTR)SysSockShutdown, 0, 0) 
#endif




/**
 * <description>
 *	Io-control of a socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="iCommand" type="IN">Io-control command</param>
 * <param name="piParameter" type="INOUT">Parameter value of the command</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockIoctl(RTS_HANDLE hSocket, int iCommand, int *piParameter);
typedef RTS_RESULT (CDECL * PFSYSSOCKIOCTL) (RTS_HANDLE hSocket, int iCommand, int *piParameter);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKIOCTL_NOTIMPLEMENTED)
	#define USE_SysSockIoctl
	#define EXT_SysSockIoctl
	#define GET_SysSockIoctl(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockIoctl(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockIoctl  FALSE
	#define EXP_SysSockIoctl  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockIoctl
	#define EXT_SysSockIoctl
	#define GET_SysSockIoctl(fl)  CAL_CMGETAPI( "SysSockIoctl" ) 
	#define CAL_SysSockIoctl  SysSockIoctl
	#define CHK_SysSockIoctl  TRUE
	#define EXP_SysSockIoctl  CAL_CMEXPAPI( "SysSockIoctl" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockIoctl
	#define EXT_SysSockIoctl
	#define GET_SysSockIoctl(fl)  CAL_CMGETAPI( "SysSockIoctl" ) 
	#define CAL_SysSockIoctl  SysSockIoctl
	#define CHK_SysSockIoctl  TRUE
	#define EXP_SysSockIoctl  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockIoctl", (RTS_UINTPTR)SysSockIoctl, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockIoctl
	#define EXT_SysSocketSysSockIoctl
	#define GET_SysSocketSysSockIoctl  ERR_OK
	#define CAL_SysSocketSysSockIoctl(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockIoctl(p1,p2) : ((ISysSocket*)p0)->ISysSockIoctl(p1,p2))
	#define CHK_SysSocketSysSockIoctl  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockIoctl  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockIoctl
	#define EXT_SysSockIoctl
	#define GET_SysSockIoctl(fl)  CAL_CMGETAPI( "SysSockIoctl" ) 
	#define CAL_SysSockIoctl(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockIoctl(p1,p2) : ((ISysSocket*)p0)->ISysSockIoctl(p1,p2))
	#define CHK_SysSockIoctl  (pISysSocket != NULL)
	#define EXP_SysSockIoctl  CAL_CMEXPAPI( "SysSockIoctl" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockIoctl  PFSYSSOCKIOCTL pfSysSockIoctl;
	#define EXT_SysSockIoctl  extern PFSYSSOCKIOCTL pfSysSockIoctl;
	#define GET_SysSockIoctl(fl)  s_pfCMGetAPI2( "SysSockIoctl", (RTS_VOID_FCTPTR *)&pfSysSockIoctl, (fl), 0, 0)
	#define CAL_SysSockIoctl  pfSysSockIoctl
	#define CHK_SysSockIoctl  (pfSysSockIoctl != NULL)
	#define EXP_SysSockIoctl  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockIoctl", (RTS_UINTPTR)SysSockIoctl, 0, 0) 
#endif




/* ------------ TCP specific ------------ */

/**
 * <description>
 *	Receive data from a TCP socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pbyBuffer" type="OUT">Buffer to read data from the socket</param>
 * <param name="iBufferSize" type="IN">Maximum length of the buffer</param>
 * <param name="iFlags" type="IN">The flags parameter can be used to influence the behavior of the 
 *	function beyond the options specified for the associated socket. The semantics of this function 
 *	are determined by the socket options and the flags parameter. The latter is constructed by using the
 *	bitwise OR operator with any of the SOCKET_MSG values. See category TCP flags.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns number of bytes received. 0 if failed.</result>
 */
RTS_SSIZE CDECL SysSockRecv(RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCKRECV) (RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKRECV_NOTIMPLEMENTED)
	#define USE_SysSockRecv
	#define EXT_SysSockRecv
	#define GET_SysSockRecv(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockRecv(p0,p1,p2,p3,p4)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSockRecv  FALSE
	#define EXP_SysSockRecv  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockRecv
	#define EXT_SysSockRecv
	#define GET_SysSockRecv(fl)  CAL_CMGETAPI( "SysSockRecv" ) 
	#define CAL_SysSockRecv  SysSockRecv
	#define CHK_SysSockRecv  TRUE
	#define EXP_SysSockRecv  CAL_CMEXPAPI( "SysSockRecv" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockRecv
	#define EXT_SysSockRecv
	#define GET_SysSockRecv(fl)  CAL_CMGETAPI( "SysSockRecv" ) 
	#define CAL_SysSockRecv  SysSockRecv
	#define CHK_SysSockRecv  TRUE
	#define EXP_SysSockRecv  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockRecv", (RTS_UINTPTR)SysSockRecv, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockRecv
	#define EXT_SysSocketSysSockRecv
	#define GET_SysSocketSysSockRecv  ERR_OK
	#define CAL_SysSocketSysSockRecv(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockRecv(p1,p2,p3,p4) : ((ISysSocket*)p0)->ISysSockRecv(p1,p2,p3,p4))
	#define CHK_SysSocketSysSockRecv  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockRecv  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockRecv
	#define EXT_SysSockRecv
	#define GET_SysSockRecv(fl)  CAL_CMGETAPI( "SysSockRecv" ) 
	#define CAL_SysSockRecv(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockRecv(p1,p2,p3,p4) : ((ISysSocket*)p0)->ISysSockRecv(p1,p2,p3,p4))
	#define CHK_SysSockRecv  (pISysSocket != NULL)
	#define EXP_SysSockRecv  CAL_CMEXPAPI( "SysSockRecv" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockRecv  PFSYSSOCKRECV pfSysSockRecv;
	#define EXT_SysSockRecv  extern PFSYSSOCKRECV pfSysSockRecv;
	#define GET_SysSockRecv(fl)  s_pfCMGetAPI2( "SysSockRecv", (RTS_VOID_FCTPTR *)&pfSysSockRecv, (fl), 0, 0)
	#define CAL_SysSockRecv  pfSysSockRecv
	#define CHK_SysSockRecv  (pfSysSockRecv != NULL)
	#define EXP_SysSockRecv  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockRecv", (RTS_UINTPTR)SysSockRecv, 0, 0) 
#endif




/**
 * <description>
 *	Sent data to a TCP socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pbyBuffer" type="IN">Buffer with data to sent</param>
 * <param name="iBufferSize" type="IN">Maximum length of the buffer</param>
 * <param name="iFlags" type="IN">The flags parameter can be used to influence the behavior of the 
 *	function beyond the options specified for the associated socket. The semantics of this function 
 *	are determined by the socket options and the flags parameter. The latter is constructed by using the
 *	bitwise OR operator with any of the SOCKET_MSG values.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns number of sent bytes. 0 if failed.</result>
 */
RTS_SSIZE CDECL SysSockSend(RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCKSEND) (RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSEND_NOTIMPLEMENTED)
	#define USE_SysSockSend
	#define EXT_SysSockSend
	#define GET_SysSockSend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockSend(p0,p1,p2,p3,p4)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSockSend  FALSE
	#define EXP_SysSockSend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockSend
	#define EXT_SysSockSend
	#define GET_SysSockSend(fl)  CAL_CMGETAPI( "SysSockSend" ) 
	#define CAL_SysSockSend  SysSockSend
	#define CHK_SysSockSend  TRUE
	#define EXP_SysSockSend  CAL_CMEXPAPI( "SysSockSend" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockSend
	#define EXT_SysSockSend
	#define GET_SysSockSend(fl)  CAL_CMGETAPI( "SysSockSend" ) 
	#define CAL_SysSockSend  SysSockSend
	#define CHK_SysSockSend  TRUE
	#define EXP_SysSockSend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSend", (RTS_UINTPTR)SysSockSend, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockSend
	#define EXT_SysSocketSysSockSend
	#define GET_SysSocketSysSockSend  ERR_OK
	#define CAL_SysSocketSysSockSend(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockSend(p1,p2,p3,p4) : ((ISysSocket*)p0)->ISysSockSend(p1,p2,p3,p4))
	#define CHK_SysSocketSysSockSend  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockSend  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockSend
	#define EXT_SysSockSend
	#define GET_SysSockSend(fl)  CAL_CMGETAPI( "SysSockSend" ) 
	#define CAL_SysSockSend(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockSend(p1,p2,p3,p4) : ((ISysSocket*)p0)->ISysSockSend(p1,p2,p3,p4))
	#define CHK_SysSockSend  (pISysSocket != NULL)
	#define EXP_SysSockSend  CAL_CMEXPAPI( "SysSockSend" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockSend  PFSYSSOCKSEND pfSysSockSend;
	#define EXT_SysSockSend  extern PFSYSSOCKSEND pfSysSockSend;
	#define GET_SysSockSend(fl)  s_pfCMGetAPI2( "SysSockSend", (RTS_VOID_FCTPTR *)&pfSysSockSend, (fl), 0, 0)
	#define CAL_SysSockSend  pfSysSockSend
	#define CHK_SysSockSend  (pfSysSockSend != NULL)
	#define EXP_SysSockSend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSend", (RTS_UINTPTR)SysSockSend, 0, 0) 
#endif




/* ------------ UDP specific ------------ */

/**
 * <description>
 *	Receive a message from a connectionless socket (UDP)
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pbyBuffer" type="OUT">Buffer to read data from the socket</param>
 * <param name="iBufferSize" type="IN">Maximum length of the buffer</param>
 * <param name="iFlags" type="IN">The flags parameter can be used to influence the behavior of the 
 *	function beyond the options specified for the associated socket. The semantics of this function 
 *	are determined by the socket options and the flags parameter. The latter is constructed by using the
 *	bitwise OR operator with any of the SOCKET_MSG values.</param>
 * <param name="pSockAddr" type="IN">Socket address and port to receive data from</param>
 * <param name="iSockAddrSize" type="IN">Size of socket address structure</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns number of bytes received</result>
 */
RTS_SSIZE CDECL SysSockRecvFrom(RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, SOCKADDRESS *pSockAddr, int iSockAddrSize, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCKRECVFROM) (RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, SOCKADDRESS *pSockAddr, int iSockAddrSize, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKRECVFROM_NOTIMPLEMENTED)
	#define USE_SysSockRecvFrom
	#define EXT_SysSockRecvFrom
	#define GET_SysSockRecvFrom(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockRecvFrom(p0,p1,p2,p3,p4,p5,p6)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSockRecvFrom  FALSE
	#define EXP_SysSockRecvFrom  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockRecvFrom
	#define EXT_SysSockRecvFrom
	#define GET_SysSockRecvFrom(fl)  CAL_CMGETAPI( "SysSockRecvFrom" ) 
	#define CAL_SysSockRecvFrom  SysSockRecvFrom
	#define CHK_SysSockRecvFrom  TRUE
	#define EXP_SysSockRecvFrom  CAL_CMEXPAPI( "SysSockRecvFrom" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockRecvFrom
	#define EXT_SysSockRecvFrom
	#define GET_SysSockRecvFrom(fl)  CAL_CMGETAPI( "SysSockRecvFrom" ) 
	#define CAL_SysSockRecvFrom  SysSockRecvFrom
	#define CHK_SysSockRecvFrom  TRUE
	#define EXP_SysSockRecvFrom  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockRecvFrom", (RTS_UINTPTR)SysSockRecvFrom, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockRecvFrom
	#define EXT_SysSocketSysSockRecvFrom
	#define GET_SysSocketSysSockRecvFrom  ERR_OK
	#define CAL_SysSocketSysSockRecvFrom(p0,p1,p2,p3,p4,p5,p6)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockRecvFrom(p1,p2,p3,p4,p5,p6) : ((ISysSocket*)p0)->ISysSockRecvFrom(p1,p2,p3,p4,p5,p6))
	#define CHK_SysSocketSysSockRecvFrom  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockRecvFrom  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockRecvFrom
	#define EXT_SysSockRecvFrom
	#define GET_SysSockRecvFrom(fl)  CAL_CMGETAPI( "SysSockRecvFrom" ) 
	#define CAL_SysSockRecvFrom(p0,p1,p2,p3,p4,p5,p6)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockRecvFrom(p1,p2,p3,p4,p5,p6) : ((ISysSocket*)p0)->ISysSockRecvFrom(p1,p2,p3,p4,p5,p6))
	#define CHK_SysSockRecvFrom  (pISysSocket != NULL)
	#define EXP_SysSockRecvFrom  CAL_CMEXPAPI( "SysSockRecvFrom" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockRecvFrom  PFSYSSOCKRECVFROM pfSysSockRecvFrom;
	#define EXT_SysSockRecvFrom  extern PFSYSSOCKRECVFROM pfSysSockRecvFrom;
	#define GET_SysSockRecvFrom(fl)  s_pfCMGetAPI2( "SysSockRecvFrom", (RTS_VOID_FCTPTR *)&pfSysSockRecvFrom, (fl), 0, 0)
	#define CAL_SysSockRecvFrom  pfSysSockRecvFrom
	#define CHK_SysSockRecvFrom  (pfSysSockRecvFrom != NULL)
	#define EXP_SysSockRecvFrom  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockRecvFrom", (RTS_UINTPTR)SysSockRecvFrom, 0, 0) 
#endif




/**
 * <description>
 *	Send a message over a connectionless socket (UDP)
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pbyBuffer" type="IN">Buffer with send data</param>
 * <param name="iBufferSize" type="IN">Length of data to send. If iBufferSize is 0 ERR_PARAMETER is returned.</param>
 * <param name="iFlags" type="IN">The flags parameter can be used to influence the behavior of the 
 *	function beyond the options specified for the associated socket. The semantics of this function 
 *	are determined by the socket options and the flags parameter. The latter is constructed by using the
 *	bitwise OR operator with any of the SOCKET_MSG values.</param>
 * <param name="pSockAddr" type="IN">Socket address and port to sent data to</param>
 * <param name="iSockAddrSize" type="IN">Size of socket address structure</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Returns number of bytes received</result>
 */
RTS_SSIZE CDECL SysSockSendTo(RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, SOCKADDRESS *pSockAddr, int iSockAddrSize, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCKSENDTO) (RTS_HANDLE hSocket, char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, SOCKADDRESS *pSockAddr, int iSockAddrSize, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSENDTO_NOTIMPLEMENTED)
	#define USE_SysSockSendTo
	#define EXT_SysSockSendTo
	#define GET_SysSockSendTo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockSendTo(p0,p1,p2,p3,p4,p5,p6)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSockSendTo  FALSE
	#define EXP_SysSockSendTo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockSendTo
	#define EXT_SysSockSendTo
	#define GET_SysSockSendTo(fl)  CAL_CMGETAPI( "SysSockSendTo" ) 
	#define CAL_SysSockSendTo  SysSockSendTo
	#define CHK_SysSockSendTo  TRUE
	#define EXP_SysSockSendTo  CAL_CMEXPAPI( "SysSockSendTo" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockSendTo
	#define EXT_SysSockSendTo
	#define GET_SysSockSendTo(fl)  CAL_CMGETAPI( "SysSockSendTo" ) 
	#define CAL_SysSockSendTo  SysSockSendTo
	#define CHK_SysSockSendTo  TRUE
	#define EXP_SysSockSendTo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSendTo", (RTS_UINTPTR)SysSockSendTo, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockSendTo
	#define EXT_SysSocketSysSockSendTo
	#define GET_SysSocketSysSockSendTo  ERR_OK
	#define CAL_SysSocketSysSockSendTo(p0,p1,p2,p3,p4,p5,p6)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockSendTo(p1,p2,p3,p4,p5,p6) : ((ISysSocket*)p0)->ISysSockSendTo(p1,p2,p3,p4,p5,p6))
	#define CHK_SysSocketSysSockSendTo  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockSendTo  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockSendTo
	#define EXT_SysSockSendTo
	#define GET_SysSockSendTo(fl)  CAL_CMGETAPI( "SysSockSendTo" ) 
	#define CAL_SysSockSendTo(p0,p1,p2,p3,p4,p5,p6)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockSendTo(p1,p2,p3,p4,p5,p6) : ((ISysSocket*)p0)->ISysSockSendTo(p1,p2,p3,p4,p5,p6))
	#define CHK_SysSockSendTo  (pISysSocket != NULL)
	#define EXP_SysSockSendTo  CAL_CMEXPAPI( "SysSockSendTo" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockSendTo  PFSYSSOCKSENDTO pfSysSockSendTo;
	#define EXT_SysSockSendTo  extern PFSYSSOCKSENDTO pfSysSockSendTo;
	#define GET_SysSockSendTo(fl)  s_pfCMGetAPI2( "SysSockSendTo", (RTS_VOID_FCTPTR *)&pfSysSockSendTo, (fl), 0, 0)
	#define CAL_SysSockSendTo  pfSysSockSendTo
	#define CHK_SysSockSendTo  (pfSysSockSendTo != NULL)
	#define EXP_SysSockSendTo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSendTo", (RTS_UINTPTR)SysSockSendTo, 0, 0) 
#endif





/**
 * <description>
 *	Higher level function, to create a complete UDP socket
 * </description>
 * <param name="iSendPort" type="IN">Port number to send (host byte order)</param>
 * <param name="iRecvPort" type="IN">Port number to receive (host byte order)</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the UDP socket</result>
 */
RTS_HANDLE CDECL SysSockCreateUdp(int iSendPort, int iRecvPort, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSSOCKCREATEUDP) (int iSendPort, int iRecvPort, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKCREATEUDP_NOTIMPLEMENTED)
	#define USE_SysSockCreateUdp
	#define EXT_SysSockCreateUdp
	#define GET_SysSockCreateUdp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockCreateUdp(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysSockCreateUdp  FALSE
	#define EXP_SysSockCreateUdp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockCreateUdp
	#define EXT_SysSockCreateUdp
	#define GET_SysSockCreateUdp(fl)  CAL_CMGETAPI( "SysSockCreateUdp" ) 
	#define CAL_SysSockCreateUdp  SysSockCreateUdp
	#define CHK_SysSockCreateUdp  TRUE
	#define EXP_SysSockCreateUdp  CAL_CMEXPAPI( "SysSockCreateUdp" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockCreateUdp
	#define EXT_SysSockCreateUdp
	#define GET_SysSockCreateUdp(fl)  CAL_CMGETAPI( "SysSockCreateUdp" ) 
	#define CAL_SysSockCreateUdp  SysSockCreateUdp
	#define CHK_SysSockCreateUdp  TRUE
	#define EXP_SysSockCreateUdp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockCreateUdp", (RTS_UINTPTR)SysSockCreateUdp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockCreateUdp
	#define EXT_SysSocketSysSockCreateUdp
	#define GET_SysSocketSysSockCreateUdp  ERR_OK
	#define CAL_SysSocketSysSockCreateUdp  ((ISysSocket*)s_pfCMCreateInstance(CLASSID_CSysSocket, NULL))->ISysSockCreateUdp
	#define CHK_SysSocketSysSockCreateUdp	(s_pfCMCreateInstance != NULL && pISysSocket != NULL)
	#define EXP_SysSocketSysSockCreateUdp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockCreateUdp
	#define EXT_SysSockCreateUdp
	#define GET_SysSockCreateUdp(fl)  CAL_CMGETAPI( "SysSockCreateUdp" ) 
	#define CAL_SysSockCreateUdp  ((ISysSocket*)s_pfCMCreateInstance(CLASSID_CSysSocket, NULL))->ISysSockCreateUdp
	#define CHK_SysSockCreateUdp	(s_pfCMCreateInstance != NULL && pISysSocket != NULL)
	#define EXP_SysSockCreateUdp  CAL_CMEXPAPI( "SysSockCreateUdp" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockCreateUdp  PFSYSSOCKCREATEUDP pfSysSockCreateUdp;
	#define EXT_SysSockCreateUdp  extern PFSYSSOCKCREATEUDP pfSysSockCreateUdp;
	#define GET_SysSockCreateUdp(fl)  s_pfCMGetAPI2( "SysSockCreateUdp", (RTS_VOID_FCTPTR *)&pfSysSockCreateUdp, (fl), 0, 0)
	#define CAL_SysSockCreateUdp  pfSysSockCreateUdp
	#define CHK_SysSockCreateUdp  (pfSysSockCreateUdp != NULL)
	#define EXP_SysSockCreateUdp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockCreateUdp", (RTS_UINTPTR)SysSockCreateUdp, 0, 0) 
#endif




/**
 * <description>
 *	Close a UDP socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the UDP socket. Must be opened with SysSockCreateUdp!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockCloseUdp(RTS_HANDLE hSocket);
typedef RTS_RESULT (CDECL * PFSYSSOCKCLOSEUDP) (RTS_HANDLE hSocket);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKCLOSEUDP_NOTIMPLEMENTED)
	#define USE_SysSockCloseUdp
	#define EXT_SysSockCloseUdp
	#define GET_SysSockCloseUdp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockCloseUdp(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockCloseUdp  FALSE
	#define EXP_SysSockCloseUdp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockCloseUdp
	#define EXT_SysSockCloseUdp
	#define GET_SysSockCloseUdp(fl)  CAL_CMGETAPI( "SysSockCloseUdp" ) 
	#define CAL_SysSockCloseUdp  SysSockCloseUdp
	#define CHK_SysSockCloseUdp  TRUE
	#define EXP_SysSockCloseUdp  CAL_CMEXPAPI( "SysSockCloseUdp" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockCloseUdp
	#define EXT_SysSockCloseUdp
	#define GET_SysSockCloseUdp(fl)  CAL_CMGETAPI( "SysSockCloseUdp" ) 
	#define CAL_SysSockCloseUdp  SysSockCloseUdp
	#define CHK_SysSockCloseUdp  TRUE
	#define EXP_SysSockCloseUdp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockCloseUdp", (RTS_UINTPTR)SysSockCloseUdp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockCloseUdp
	#define EXT_SysSocketSysSockCloseUdp
	#define GET_SysSocketSysSockCloseUdp  ERR_OK
	#define CAL_SysSocketSysSockCloseUdp(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockCloseUdp() : ((ISysSocket*)p0)->ISysSockCloseUdp())
	#define CHK_SysSocketSysSockCloseUdp  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockCloseUdp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockCloseUdp
	#define EXT_SysSockCloseUdp
	#define GET_SysSockCloseUdp(fl)  CAL_CMGETAPI( "SysSockCloseUdp" ) 
	#define CAL_SysSockCloseUdp(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockCloseUdp() : ((ISysSocket*)p0)->ISysSockCloseUdp())
	#define CHK_SysSockCloseUdp  (pISysSocket != NULL)
	#define EXP_SysSockCloseUdp  CAL_CMEXPAPI( "SysSockCloseUdp" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockCloseUdp  PFSYSSOCKCLOSEUDP pfSysSockCloseUdp;
	#define EXT_SysSockCloseUdp  extern PFSYSSOCKCLOSEUDP pfSysSockCloseUdp;
	#define GET_SysSockCloseUdp(fl)  s_pfCMGetAPI2( "SysSockCloseUdp", (RTS_VOID_FCTPTR *)&pfSysSockCloseUdp, (fl), 0, 0)
	#define CAL_SysSockCloseUdp  pfSysSockCloseUdp
	#define CHK_SysSockCloseUdp  (pfSysSockCloseUdp != NULL)
	#define EXP_SysSockCloseUdp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockCloseUdp", (RTS_UINTPTR)SysSockCloseUdp, 0, 0) 
#endif




/**
 * <description>
 *	Send a paket to a UDP socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the UDP socket</param>
 * <param name="iPort" type="IN">Port number to send in host byteorder!</param>
 * <param name="pszDestAddress" type="IN">Destination IP address ot send data to</param>
 * <param name="pbyData" type="IN">Pointer to data to send</param>
 * <param name="iDataSize" type="IN">Size of data to send. If iDataSize is 0 ERR_PARAMETER is returned.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Number of bytes sent</result>
 */
RTS_SSIZE CDECL SysSockSendToUdp(RTS_HANDLE hSocket, int iPort, char *pszDestAddress, unsigned char *pbyData, RTS_SSIZE iDataSize, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCKSENDTOUDP) (RTS_HANDLE hSocket, int iPort, char *pszDestAddress, unsigned char *pbyData, RTS_SSIZE iDataSize, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSENDTOUDP_NOTIMPLEMENTED)
	#define USE_SysSockSendToUdp
	#define EXT_SysSockSendToUdp
	#define GET_SysSockSendToUdp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockSendToUdp(p0,p1,p2,p3,p4,p5)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSockSendToUdp  FALSE
	#define EXP_SysSockSendToUdp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockSendToUdp
	#define EXT_SysSockSendToUdp
	#define GET_SysSockSendToUdp(fl)  CAL_CMGETAPI( "SysSockSendToUdp" ) 
	#define CAL_SysSockSendToUdp  SysSockSendToUdp
	#define CHK_SysSockSendToUdp  TRUE
	#define EXP_SysSockSendToUdp  CAL_CMEXPAPI( "SysSockSendToUdp" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockSendToUdp
	#define EXT_SysSockSendToUdp
	#define GET_SysSockSendToUdp(fl)  CAL_CMGETAPI( "SysSockSendToUdp" ) 
	#define CAL_SysSockSendToUdp  SysSockSendToUdp
	#define CHK_SysSockSendToUdp  TRUE
	#define EXP_SysSockSendToUdp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSendToUdp", (RTS_UINTPTR)SysSockSendToUdp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockSendToUdp
	#define EXT_SysSocketSysSockSendToUdp
	#define GET_SysSocketSysSockSendToUdp  ERR_OK
	#define CAL_SysSocketSysSockSendToUdp(p0,p1,p2,p3,p4,p5)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockSendToUdp(p1,p2,p3,p4,p5) : ((ISysSocket*)p0)->ISysSockSendToUdp(p1,p2,p3,p4,p5))
	#define CHK_SysSocketSysSockSendToUdp  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockSendToUdp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockSendToUdp
	#define EXT_SysSockSendToUdp
	#define GET_SysSockSendToUdp(fl)  CAL_CMGETAPI( "SysSockSendToUdp" ) 
	#define CAL_SysSockSendToUdp(p0,p1,p2,p3,p4,p5)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockSendToUdp(p1,p2,p3,p4,p5) : ((ISysSocket*)p0)->ISysSockSendToUdp(p1,p2,p3,p4,p5))
	#define CHK_SysSockSendToUdp  (pISysSocket != NULL)
	#define EXP_SysSockSendToUdp  CAL_CMEXPAPI( "SysSockSendToUdp" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockSendToUdp  PFSYSSOCKSENDTOUDP pfSysSockSendToUdp;
	#define EXT_SysSockSendToUdp  extern PFSYSSOCKSENDTOUDP pfSysSockSendToUdp;
	#define GET_SysSockSendToUdp(fl)  s_pfCMGetAPI2( "SysSockSendToUdp", (RTS_VOID_FCTPTR *)&pfSysSockSendToUdp, (fl), 0, 0)
	#define CAL_SysSockSendToUdp  pfSysSockSendToUdp
	#define CHK_SysSockSendToUdp  (pfSysSockSendToUdp != NULL)
	#define EXP_SysSockSendToUdp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSendToUdp", (RTS_UINTPTR)SysSockSendToUdp, 0, 0) 
#endif




/**
 * <description>
 *	Receive a paket from a UDP socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the UDP socket</param>
 * <param name="pbyData" type="OUT">Pointer to data to receive</param>
 * <param name="iDataSize" type="IN">Size of data to receive</param>
 * <param name="pReply" type="OUT">Description of the client that has sent this packet. See category "Udp reply".</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Number of bytes received</result>
 */
RTS_SSIZE CDECL SysSockRecvFromUdp(RTS_HANDLE hSocket, unsigned char *pbyData, RTS_SSIZE iDataSize, UDP_REPLY *pReply, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCKRECVFROMUDP) (RTS_HANDLE hSocket, unsigned char *pbyData, RTS_SSIZE iDataSize, UDP_REPLY *pReply, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKRECVFROMUDP_NOTIMPLEMENTED)
	#define USE_SysSockRecvFromUdp
	#define EXT_SysSockRecvFromUdp
	#define GET_SysSockRecvFromUdp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockRecvFromUdp(p0,p1,p2,p3,p4)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSockRecvFromUdp  FALSE
	#define EXP_SysSockRecvFromUdp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockRecvFromUdp
	#define EXT_SysSockRecvFromUdp
	#define GET_SysSockRecvFromUdp(fl)  CAL_CMGETAPI( "SysSockRecvFromUdp" ) 
	#define CAL_SysSockRecvFromUdp  SysSockRecvFromUdp
	#define CHK_SysSockRecvFromUdp  TRUE
	#define EXP_SysSockRecvFromUdp  CAL_CMEXPAPI( "SysSockRecvFromUdp" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockRecvFromUdp
	#define EXT_SysSockRecvFromUdp
	#define GET_SysSockRecvFromUdp(fl)  CAL_CMGETAPI( "SysSockRecvFromUdp" ) 
	#define CAL_SysSockRecvFromUdp  SysSockRecvFromUdp
	#define CHK_SysSockRecvFromUdp  TRUE
	#define EXP_SysSockRecvFromUdp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockRecvFromUdp", (RTS_UINTPTR)SysSockRecvFromUdp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockRecvFromUdp
	#define EXT_SysSocketSysSockRecvFromUdp
	#define GET_SysSocketSysSockRecvFromUdp  ERR_OK
	#define CAL_SysSocketSysSockRecvFromUdp(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockRecvFromUdp(p1,p2,p3,p4) : ((ISysSocket*)p0)->ISysSockRecvFromUdp(p1,p2,p3,p4))
	#define CHK_SysSocketSysSockRecvFromUdp  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockRecvFromUdp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockRecvFromUdp
	#define EXT_SysSockRecvFromUdp
	#define GET_SysSockRecvFromUdp(fl)  CAL_CMGETAPI( "SysSockRecvFromUdp" ) 
	#define CAL_SysSockRecvFromUdp(p0,p1,p2,p3,p4)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockRecvFromUdp(p1,p2,p3,p4) : ((ISysSocket*)p0)->ISysSockRecvFromUdp(p1,p2,p3,p4))
	#define CHK_SysSockRecvFromUdp  (pISysSocket != NULL)
	#define EXP_SysSockRecvFromUdp  CAL_CMEXPAPI( "SysSockRecvFromUdp" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockRecvFromUdp  PFSYSSOCKRECVFROMUDP pfSysSockRecvFromUdp;
	#define EXT_SysSockRecvFromUdp  extern PFSYSSOCKRECVFROMUDP pfSysSockRecvFromUdp;
	#define GET_SysSockRecvFromUdp(fl)  s_pfCMGetAPI2( "SysSockRecvFromUdp", (RTS_VOID_FCTPTR *)&pfSysSockRecvFromUdp, (fl), 0, 0)
	#define CAL_SysSockRecvFromUdp  pfSysSockRecvFromUdp
	#define CHK_SysSockRecvFromUdp  (pfSysSockRecvFromUdp != NULL)
	#define EXP_SysSockRecvFromUdp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockRecvFromUdp", (RTS_UINTPTR)SysSockRecvFromUdp, 0, 0) 
#endif




/**
 * <description>
 *	Check actual received data on the UDP socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the UDP socket</param>
 * <param name="iTimeout" type="IN">Timeout to wait for received data. -1=Infinite wait, 0=no wait</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Number of bytes actual available in the socket</result>
 */
RTS_SSIZE CDECL SysSockGetRecvSizeUdp(RTS_HANDLE hSocket, int iTimeout, RTS_RESULT *pResult);
typedef RTS_SSIZE (CDECL * PFSYSSOCKGETRECVSIZEUDP) (RTS_HANDLE hSocket, int iTimeout, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETRECVSIZEUDP_NOTIMPLEMENTED)
	#define USE_SysSockGetRecvSizeUdp
	#define EXT_SysSockGetRecvSizeUdp
	#define GET_SysSockGetRecvSizeUdp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockGetRecvSizeUdp(p0,p1,p2)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysSockGetRecvSizeUdp  FALSE
	#define EXP_SysSockGetRecvSizeUdp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockGetRecvSizeUdp
	#define EXT_SysSockGetRecvSizeUdp
	#define GET_SysSockGetRecvSizeUdp(fl)  CAL_CMGETAPI( "SysSockGetRecvSizeUdp" ) 
	#define CAL_SysSockGetRecvSizeUdp  SysSockGetRecvSizeUdp
	#define CHK_SysSockGetRecvSizeUdp  TRUE
	#define EXP_SysSockGetRecvSizeUdp  CAL_CMEXPAPI( "SysSockGetRecvSizeUdp" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockGetRecvSizeUdp
	#define EXT_SysSockGetRecvSizeUdp
	#define GET_SysSockGetRecvSizeUdp(fl)  CAL_CMGETAPI( "SysSockGetRecvSizeUdp" ) 
	#define CAL_SysSockGetRecvSizeUdp  SysSockGetRecvSizeUdp
	#define CHK_SysSockGetRecvSizeUdp  TRUE
	#define EXP_SysSockGetRecvSizeUdp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetRecvSizeUdp", (RTS_UINTPTR)SysSockGetRecvSizeUdp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockGetRecvSizeUdp
	#define EXT_SysSocketSysSockGetRecvSizeUdp
	#define GET_SysSocketSysSockGetRecvSizeUdp  ERR_OK
	#define CAL_SysSocketSysSockGetRecvSizeUdp(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockGetRecvSizeUdp(p1,p2) : ((ISysSocket*)p0)->ISysSockGetRecvSizeUdp(p1,p2))
	#define CHK_SysSocketSysSockGetRecvSizeUdp  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockGetRecvSizeUdp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockGetRecvSizeUdp
	#define EXT_SysSockGetRecvSizeUdp
	#define GET_SysSockGetRecvSizeUdp(fl)  CAL_CMGETAPI( "SysSockGetRecvSizeUdp" ) 
	#define CAL_SysSockGetRecvSizeUdp(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockGetRecvSizeUdp(p1,p2) : ((ISysSocket*)p0)->ISysSockGetRecvSizeUdp(p1,p2))
	#define CHK_SysSockGetRecvSizeUdp  (pISysSocket != NULL)
	#define EXP_SysSockGetRecvSizeUdp  CAL_CMEXPAPI( "SysSockGetRecvSizeUdp" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockGetRecvSizeUdp  PFSYSSOCKGETRECVSIZEUDP pfSysSockGetRecvSizeUdp;
	#define EXT_SysSockGetRecvSizeUdp  extern PFSYSSOCKGETRECVSIZEUDP pfSysSockGetRecvSizeUdp;
	#define GET_SysSockGetRecvSizeUdp(fl)  s_pfCMGetAPI2( "SysSockGetRecvSizeUdp", (RTS_VOID_FCTPTR *)&pfSysSockGetRecvSizeUdp, (fl), 0, 0)
	#define CAL_SysSockGetRecvSizeUdp  pfSysSockGetRecvSizeUdp
	#define CHK_SysSockGetRecvSizeUdp  (pfSysSockGetRecvSizeUdp != NULL)
	#define EXP_SysSockGetRecvSizeUdp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetRecvSizeUdp", (RTS_UINTPTR)SysSockGetRecvSizeUdp, 0, 0) 
#endif




/**
 * <description>
 *	Get operating system handle of the UDP socket
 * </description>
 * <param name="hSocket" type="IN">Handle to the UDP socket</param>
 * <result>Operating system handle</result>
 */
RTS_HANDLE CDECL SysSockGetOSHandle(RTS_HANDLE hSocket);
typedef RTS_HANDLE (CDECL * PFSYSSOCKGETOSHANDLE) (RTS_HANDLE hSocket);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETOSHANDLE_NOTIMPLEMENTED)
	#define USE_SysSockGetOSHandle
	#define EXT_SysSockGetOSHandle
	#define GET_SysSockGetOSHandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockGetOSHandle(p0)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysSockGetOSHandle  FALSE
	#define EXP_SysSockGetOSHandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockGetOSHandle
	#define EXT_SysSockGetOSHandle
	#define GET_SysSockGetOSHandle(fl)  CAL_CMGETAPI( "SysSockGetOSHandle" ) 
	#define CAL_SysSockGetOSHandle  SysSockGetOSHandle
	#define CHK_SysSockGetOSHandle  TRUE
	#define EXP_SysSockGetOSHandle  CAL_CMEXPAPI( "SysSockGetOSHandle" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockGetOSHandle
	#define EXT_SysSockGetOSHandle
	#define GET_SysSockGetOSHandle(fl)  CAL_CMGETAPI( "SysSockGetOSHandle" ) 
	#define CAL_SysSockGetOSHandle  SysSockGetOSHandle
	#define CHK_SysSockGetOSHandle  TRUE
	#define EXP_SysSockGetOSHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetOSHandle", (RTS_UINTPTR)SysSockGetOSHandle, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockGetOSHandle
	#define EXT_SysSocketSysSockGetOSHandle
	#define GET_SysSocketSysSockGetOSHandle  ERR_OK
	#define CAL_SysSocketSysSockGetOSHandle(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockGetOSHandle() : ((ISysSocket*)p0)->ISysSockGetOSHandle())
	#define CHK_SysSocketSysSockGetOSHandle  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockGetOSHandle  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockGetOSHandle
	#define EXT_SysSockGetOSHandle
	#define GET_SysSockGetOSHandle(fl)  CAL_CMGETAPI( "SysSockGetOSHandle" ) 
	#define CAL_SysSockGetOSHandle(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockGetOSHandle() : ((ISysSocket*)p0)->ISysSockGetOSHandle())
	#define CHK_SysSockGetOSHandle  (pISysSocket != NULL)
	#define EXP_SysSockGetOSHandle  CAL_CMEXPAPI( "SysSockGetOSHandle" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockGetOSHandle  PFSYSSOCKGETOSHANDLE pfSysSockGetOSHandle;
	#define EXT_SysSockGetOSHandle  extern PFSYSSOCKGETOSHANDLE pfSysSockGetOSHandle;
	#define GET_SysSockGetOSHandle(fl)  s_pfCMGetAPI2( "SysSockGetOSHandle", (RTS_VOID_FCTPTR *)&pfSysSockGetOSHandle, (fl), 0, 0)
	#define CAL_SysSockGetOSHandle  pfSysSockGetOSHandle
	#define CHK_SysSockGetOSHandle  (pfSysSockGetOSHandle != NULL)
	#define EXP_SysSockGetOSHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetOSHandle", (RTS_UINTPTR)SysSockGetOSHandle, 0, 0) 
#endif




/**
 * <description>
 *	Check the availability of the communication partner with a ping request
 * </description>
 * <param name="pszIPAddress" type="IN">IP address of the communication partner as string</param>
 * <param name="ulTimeout" type="IN">Timeout in milliseconds to wait until reply</param>
 * <param name="pulReplyTime" type="OUT">Pointer to get the reply time of the ping request in milliseconds or NULL</param>
 * <result>
 *   ERR_OK: Partner available, ERR_TIMEOUT: Partner could not be reached during the specified timeout.
 *   All other results: Ping could not be sent because of other errors, so we don't know, if the partner is available.
 * </result>
 */
RTS_RESULT CDECL SysSockPing(char *pszIPAddress, RTS_UI32 ulTimeout, RTS_UI32 *pulReplyTime);
typedef RTS_RESULT (CDECL * PFSYSSOCKPING) (char *pszIPAddress, RTS_UI32 ulTimeout, RTS_UI32 *pulReplyTime);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKPING_NOTIMPLEMENTED)
	#define USE_SysSockPing
	#define EXT_SysSockPing
	#define GET_SysSockPing(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockPing(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockPing  FALSE
	#define EXP_SysSockPing  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockPing
	#define EXT_SysSockPing
	#define GET_SysSockPing(fl)  CAL_CMGETAPI( "SysSockPing" ) 
	#define CAL_SysSockPing  SysSockPing
	#define CHK_SysSockPing  TRUE
	#define EXP_SysSockPing  CAL_CMEXPAPI( "SysSockPing" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockPing
	#define EXT_SysSockPing
	#define GET_SysSockPing(fl)  CAL_CMGETAPI( "SysSockPing" ) 
	#define CAL_SysSockPing  SysSockPing
	#define CHK_SysSockPing  TRUE
	#define EXP_SysSockPing  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockPing", (RTS_UINTPTR)SysSockPing, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockPing
	#define EXT_SysSocketSysSockPing
	#define GET_SysSocketSysSockPing  ERR_OK
	#define CAL_SysSocketSysSockPing pISysSocket->ISysSockPing
	#define CHK_SysSocketSysSockPing (pISysSocket != NULL)
	#define EXP_SysSocketSysSockPing  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockPing
	#define EXT_SysSockPing
	#define GET_SysSockPing(fl)  CAL_CMGETAPI( "SysSockPing" ) 
	#define CAL_SysSockPing pISysSocket->ISysSockPing
	#define CHK_SysSockPing (pISysSocket != NULL)
	#define EXP_SysSockPing  CAL_CMEXPAPI( "SysSockPing" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockPing  PFSYSSOCKPING pfSysSockPing;
	#define EXT_SysSockPing  extern PFSYSSOCKPING pfSysSockPing;
	#define GET_SysSockPing(fl)  s_pfCMGetAPI2( "SysSockPing", (RTS_VOID_FCTPTR *)&pfSysSockPing, (fl), 0, 0)
	#define CAL_SysSockPing  pfSysSockPing
	#define CHK_SysSockPing  (pfSysSockPing != NULL)
	#define EXP_SysSockPing  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockPing", (RTS_UINTPTR)SysSockPing, 0, 0) 
#endif




/**
 * <description>
 *	Set subnetmask of a specified IP address adapter. Is not available on all platforms!
 *  Use SysSockSetIpAddressAndNetMask() instead.
 * </description>
 * <param name="pszIPAddress" type="IN">IP address of the communication partner as string</param>
 * <param name="pszSubnetMask" type="IN">Subnet mask as string</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockSetSubnetMask(char *pszIPAddress, char *pszSubnetMask);
typedef RTS_RESULT (CDECL * PFSYSSOCKSETSUBNETMASK) (char *pszIPAddress, char *pszSubnetMask);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSETSUBNETMASK_NOTIMPLEMENTED)
	#define USE_SysSockSetSubnetMask
	#define EXT_SysSockSetSubnetMask
	#define GET_SysSockSetSubnetMask(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockSetSubnetMask(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockSetSubnetMask  FALSE
	#define EXP_SysSockSetSubnetMask  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockSetSubnetMask
	#define EXT_SysSockSetSubnetMask
	#define GET_SysSockSetSubnetMask(fl)  CAL_CMGETAPI( "SysSockSetSubnetMask" ) 
	#define CAL_SysSockSetSubnetMask  SysSockSetSubnetMask
	#define CHK_SysSockSetSubnetMask  TRUE
	#define EXP_SysSockSetSubnetMask  CAL_CMEXPAPI( "SysSockSetSubnetMask" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockSetSubnetMask
	#define EXT_SysSockSetSubnetMask
	#define GET_SysSockSetSubnetMask(fl)  CAL_CMGETAPI( "SysSockSetSubnetMask" ) 
	#define CAL_SysSockSetSubnetMask  SysSockSetSubnetMask
	#define CHK_SysSockSetSubnetMask  TRUE
	#define EXP_SysSockSetSubnetMask  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSetSubnetMask", (RTS_UINTPTR)SysSockSetSubnetMask, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockSetSubnetMask
	#define EXT_SysSocketSysSockSetSubnetMask
	#define GET_SysSocketSysSockSetSubnetMask  ERR_OK
	#define CAL_SysSocketSysSockSetSubnetMask pISysSocket->ISysSockSetSubnetMask
	#define CHK_SysSocketSysSockSetSubnetMask (pISysSocket != NULL)
	#define EXP_SysSocketSysSockSetSubnetMask  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockSetSubnetMask
	#define EXT_SysSockSetSubnetMask
	#define GET_SysSockSetSubnetMask(fl)  CAL_CMGETAPI( "SysSockSetSubnetMask" ) 
	#define CAL_SysSockSetSubnetMask pISysSocket->ISysSockSetSubnetMask
	#define CHK_SysSockSetSubnetMask (pISysSocket != NULL)
	#define EXP_SysSockSetSubnetMask  CAL_CMEXPAPI( "SysSockSetSubnetMask" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockSetSubnetMask  PFSYSSOCKSETSUBNETMASK pfSysSockSetSubnetMask;
	#define EXT_SysSockSetSubnetMask  extern PFSYSSOCKSETSUBNETMASK pfSysSockSetSubnetMask;
	#define GET_SysSockSetSubnetMask(fl)  s_pfCMGetAPI2( "SysSockSetSubnetMask", (RTS_VOID_FCTPTR *)&pfSysSockSetSubnetMask, (fl), 0, 0)
	#define CAL_SysSockSetSubnetMask  pfSysSockSetSubnetMask
	#define CHK_SysSockSetSubnetMask  (pfSysSockSetSubnetMask != NULL)
	#define EXP_SysSockSetSubnetMask  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSetSubnetMask", (RTS_UINTPTR)SysSockSetSubnetMask, 0, 0) 
#endif




/**
 * <description>
 *	Get subnetmask of a specified IP address adapter
 * </description>
 * <param name="pszIPAddress" type="IN">IP address of the communication partner as string</param>
 * <param name="pszSubnetMask" type="OUT">Subnet mask as string</param>
 * <param name="iMaxSubnetMask" type="IN">Maximum length of the subnet mask string</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockGetSubnetMask(char *pszIPAddress, char *pszSubnetMask, int iMaxSubnetMask);
typedef RTS_RESULT (CDECL * PFSYSSOCKGETSUBNETMASK) (char *pszIPAddress, char *pszSubnetMask, int iMaxSubnetMask);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETSUBNETMASK_NOTIMPLEMENTED)
	#define USE_SysSockGetSubnetMask
	#define EXT_SysSockGetSubnetMask
	#define GET_SysSockGetSubnetMask(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockGetSubnetMask(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockGetSubnetMask  FALSE
	#define EXP_SysSockGetSubnetMask  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockGetSubnetMask
	#define EXT_SysSockGetSubnetMask
	#define GET_SysSockGetSubnetMask(fl)  CAL_CMGETAPI( "SysSockGetSubnetMask" ) 
	#define CAL_SysSockGetSubnetMask  SysSockGetSubnetMask
	#define CHK_SysSockGetSubnetMask  TRUE
	#define EXP_SysSockGetSubnetMask  CAL_CMEXPAPI( "SysSockGetSubnetMask" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockGetSubnetMask
	#define EXT_SysSockGetSubnetMask
	#define GET_SysSockGetSubnetMask(fl)  CAL_CMGETAPI( "SysSockGetSubnetMask" ) 
	#define CAL_SysSockGetSubnetMask  SysSockGetSubnetMask
	#define CHK_SysSockGetSubnetMask  TRUE
	#define EXP_SysSockGetSubnetMask  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetSubnetMask", (RTS_UINTPTR)SysSockGetSubnetMask, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockGetSubnetMask
	#define EXT_SysSocketSysSockGetSubnetMask
	#define GET_SysSocketSysSockGetSubnetMask  ERR_OK
	#define CAL_SysSocketSysSockGetSubnetMask pISysSocket->ISysSockGetSubnetMask
	#define CHK_SysSocketSysSockGetSubnetMask (pISysSocket != NULL)
	#define EXP_SysSocketSysSockGetSubnetMask  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockGetSubnetMask
	#define EXT_SysSockGetSubnetMask
	#define GET_SysSockGetSubnetMask(fl)  CAL_CMGETAPI( "SysSockGetSubnetMask" ) 
	#define CAL_SysSockGetSubnetMask pISysSocket->ISysSockGetSubnetMask
	#define CHK_SysSockGetSubnetMask (pISysSocket != NULL)
	#define EXP_SysSockGetSubnetMask  CAL_CMEXPAPI( "SysSockGetSubnetMask" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockGetSubnetMask  PFSYSSOCKGETSUBNETMASK pfSysSockGetSubnetMask;
	#define EXT_SysSockGetSubnetMask  extern PFSYSSOCKGETSUBNETMASK pfSysSockGetSubnetMask;
	#define GET_SysSockGetSubnetMask(fl)  s_pfCMGetAPI2( "SysSockGetSubnetMask", (RTS_VOID_FCTPTR *)&pfSysSockGetSubnetMask, (fl), 0, 0)
	#define CAL_SysSockGetSubnetMask  pfSysSockGetSubnetMask
	#define CHK_SysSockGetSubnetMask  (pfSysSockGetSubnetMask != NULL)
	#define EXP_SysSockGetSubnetMask  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetSubnetMask", (RTS_UINTPTR)SysSockGetSubnetMask, 0, 0) 
#endif




/**
 * <description>
 *	Check if a socket is inside of a set.
 * </description>
 * <param name="hSocket" type="IN">Socket to check. Can be RTS_INVALID_HANDLE to check for an invalid filled set.</param>
 * <param name="pfs" type="IN">Socket Set</param>
 * <result>TRUE if the specified socket is inside the set, FALSE if not</result>
 */
RTS_BOOL CDECL SysSockFdIsset(RTS_HANDLE hSocket, SOCKET_FD_SET *pfs);
typedef RTS_BOOL (CDECL * PFSYSSOCKFDISSET) (RTS_HANDLE hSocket, SOCKET_FD_SET *pfs);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKFDISSET_NOTIMPLEMENTED)
	#define USE_SysSockFdIsset
	#define EXT_SysSockFdIsset
	#define GET_SysSockFdIsset(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockFdIsset(p0,p1)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_SysSockFdIsset  FALSE
	#define EXP_SysSockFdIsset  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockFdIsset
	#define EXT_SysSockFdIsset
	#define GET_SysSockFdIsset(fl)  CAL_CMGETAPI( "SysSockFdIsset" ) 
	#define CAL_SysSockFdIsset  SysSockFdIsset
	#define CHK_SysSockFdIsset  TRUE
	#define EXP_SysSockFdIsset  CAL_CMEXPAPI( "SysSockFdIsset" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockFdIsset
	#define EXT_SysSockFdIsset
	#define GET_SysSockFdIsset(fl)  CAL_CMGETAPI( "SysSockFdIsset" ) 
	#define CAL_SysSockFdIsset  SysSockFdIsset
	#define CHK_SysSockFdIsset  TRUE
	#define EXP_SysSockFdIsset  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockFdIsset", (RTS_UINTPTR)SysSockFdIsset, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockFdIsset
	#define EXT_SysSocketSysSockFdIsset
	#define GET_SysSocketSysSockFdIsset  ERR_OK
	#define CAL_SysSocketSysSockFdIsset(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockFdIsset(p1) : ((ISysSocket*)p0)->ISysSockFdIsset(p1))
	#define CHK_SysSocketSysSockFdIsset  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockFdIsset  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockFdIsset
	#define EXT_SysSockFdIsset
	#define GET_SysSockFdIsset(fl)  CAL_CMGETAPI( "SysSockFdIsset" ) 
	#define CAL_SysSockFdIsset(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockFdIsset(p1) : ((ISysSocket*)p0)->ISysSockFdIsset(p1))
	#define CHK_SysSockFdIsset  (pISysSocket != NULL)
	#define EXP_SysSockFdIsset  CAL_CMEXPAPI( "SysSockFdIsset" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockFdIsset  PFSYSSOCKFDISSET pfSysSockFdIsset;
	#define EXT_SysSockFdIsset  extern PFSYSSOCKFDISSET pfSysSockFdIsset;
	#define GET_SysSockFdIsset(fl)  s_pfCMGetAPI2( "SysSockFdIsset", (RTS_VOID_FCTPTR *)&pfSysSockFdIsset, (fl), 0, 0)
	#define CAL_SysSockFdIsset  pfSysSockFdIsset
	#define CHK_SysSockFdIsset  (pfSysSockFdIsset != NULL)
	#define EXP_SysSockFdIsset  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockFdIsset", (RTS_UINTPTR)SysSockFdIsset, 0, 0) 
#endif




/**
 * <description>
 *	Add a socket to a socket set.
 * </description>
 * <param name="hSocket" type="IN">Socket to add.</param>
 * <param name="pfs" type="IN">Socket Set</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockFdInit(RTS_HANDLE hSocket, SOCKET_FD_SET *pfs);
typedef RTS_RESULT (CDECL * PFSYSSOCKFDINIT) (RTS_HANDLE hSocket, SOCKET_FD_SET *pfs);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKFDINIT_NOTIMPLEMENTED)
	#define USE_SysSockFdInit
	#define EXT_SysSockFdInit
	#define GET_SysSockFdInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockFdInit(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockFdInit  FALSE
	#define EXP_SysSockFdInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockFdInit
	#define EXT_SysSockFdInit
	#define GET_SysSockFdInit(fl)  CAL_CMGETAPI( "SysSockFdInit" ) 
	#define CAL_SysSockFdInit  SysSockFdInit
	#define CHK_SysSockFdInit  TRUE
	#define EXP_SysSockFdInit  CAL_CMEXPAPI( "SysSockFdInit" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockFdInit
	#define EXT_SysSockFdInit
	#define GET_SysSockFdInit(fl)  CAL_CMGETAPI( "SysSockFdInit" ) 
	#define CAL_SysSockFdInit  SysSockFdInit
	#define CHK_SysSockFdInit  TRUE
	#define EXP_SysSockFdInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockFdInit", (RTS_UINTPTR)SysSockFdInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockFdInit
	#define EXT_SysSocketSysSockFdInit
	#define GET_SysSocketSysSockFdInit  ERR_OK
	#define CAL_SysSocketSysSockFdInit(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockFdInit(p1) : ((ISysSocket*)p0)->ISysSockFdInit(p1))
	#define CHK_SysSocketSysSockFdInit  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockFdInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockFdInit
	#define EXT_SysSockFdInit
	#define GET_SysSockFdInit(fl)  CAL_CMGETAPI( "SysSockFdInit" ) 
	#define CAL_SysSockFdInit(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockFdInit(p1) : ((ISysSocket*)p0)->ISysSockFdInit(p1))
	#define CHK_SysSockFdInit  (pISysSocket != NULL)
	#define EXP_SysSockFdInit  CAL_CMEXPAPI( "SysSockFdInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockFdInit  PFSYSSOCKFDINIT pfSysSockFdInit;
	#define EXT_SysSockFdInit  extern PFSYSSOCKFDINIT pfSysSockFdInit;
	#define GET_SysSockFdInit(fl)  s_pfCMGetAPI2( "SysSockFdInit", (RTS_VOID_FCTPTR *)&pfSysSockFdInit, (fl), 0, 0)
	#define CAL_SysSockFdInit  pfSysSockFdInit
	#define CHK_SysSockFdInit  (pfSysSockFdInit != NULL)
	#define EXP_SysSockFdInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockFdInit", (RTS_UINTPTR)SysSockFdInit, 0, 0) 
#endif




/**
 * <description>
 *	Clear a Socket set.
 * </description>
 * <param name="pfs" type="IN">Socket Set</param>
 */
void CDECL SysSockFdZero(SOCKET_FD_SET *pfs);
typedef void (CDECL * PFSYSSOCKFDZERO) (SOCKET_FD_SET *pfs);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKFDZERO_NOTIMPLEMENTED)
	#define USE_SysSockFdZero
	#define EXT_SysSockFdZero
	#define GET_SysSockFdZero(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockFdZero(p0)  
	#define CHK_SysSockFdZero  FALSE
	#define EXP_SysSockFdZero  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockFdZero
	#define EXT_SysSockFdZero
	#define GET_SysSockFdZero(fl)  CAL_CMGETAPI( "SysSockFdZero" ) 
	#define CAL_SysSockFdZero  SysSockFdZero
	#define CHK_SysSockFdZero  TRUE
	#define EXP_SysSockFdZero  CAL_CMEXPAPI( "SysSockFdZero" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockFdZero
	#define EXT_SysSockFdZero
	#define GET_SysSockFdZero(fl)  CAL_CMGETAPI( "SysSockFdZero" ) 
	#define CAL_SysSockFdZero  SysSockFdZero
	#define CHK_SysSockFdZero  TRUE
	#define EXP_SysSockFdZero  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockFdZero", (RTS_UINTPTR)SysSockFdZero, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockFdZero
	#define EXT_SysSocketSysSockFdZero
	#define GET_SysSocketSysSockFdZero  ERR_OK
	#define CAL_SysSocketSysSockFdZero pISysSocket->ISysSockFdZero
	#define CHK_SysSocketSysSockFdZero (pISysSocket != NULL)
	#define EXP_SysSocketSysSockFdZero  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockFdZero
	#define EXT_SysSockFdZero
	#define GET_SysSockFdZero(fl)  CAL_CMGETAPI( "SysSockFdZero" ) 
	#define CAL_SysSockFdZero pISysSocket->ISysSockFdZero
	#define CHK_SysSockFdZero (pISysSocket != NULL)
	#define EXP_SysSockFdZero  CAL_CMEXPAPI( "SysSockFdZero" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockFdZero  PFSYSSOCKFDZERO pfSysSockFdZero;
	#define EXT_SysSockFdZero  extern PFSYSSOCKFDZERO pfSysSockFdZero;
	#define GET_SysSockFdZero(fl)  s_pfCMGetAPI2( "SysSockFdZero", (RTS_VOID_FCTPTR *)&pfSysSockFdZero, (fl), 0, 0)
	#define CAL_SysSockFdZero  pfSysSockFdZero
	#define CHK_SysSockFdZero  (pfSysSockFdZero != NULL)
	#define EXP_SysSockFdZero  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockFdZero", (RTS_UINTPTR)SysSockFdZero, 0, 0) 
#endif




/**
 * <description>
 *	Get adapter information struct of the first network adapter.
 * </description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Information struct of first network adapter</result>
 */
SOCK_ADAPTER_INFO* CDECL SysSockGetFirstAdapterInfo(RTS_RESULT *pResult);
typedef SOCK_ADAPTER_INFO* (CDECL * PFSYSSOCKGETFIRSTADAPTERINFO) (RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETFIRSTADAPTERINFO_NOTIMPLEMENTED)
	#define USE_SysSockGetFirstAdapterInfo
	#define EXT_SysSockGetFirstAdapterInfo
	#define GET_SysSockGetFirstAdapterInfo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockGetFirstAdapterInfo(p0)  (SOCK_ADAPTER_INFO*)ERR_NOTIMPLEMENTED
	#define CHK_SysSockGetFirstAdapterInfo  FALSE
	#define EXP_SysSockGetFirstAdapterInfo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockGetFirstAdapterInfo
	#define EXT_SysSockGetFirstAdapterInfo
	#define GET_SysSockGetFirstAdapterInfo(fl)  CAL_CMGETAPI( "SysSockGetFirstAdapterInfo" ) 
	#define CAL_SysSockGetFirstAdapterInfo  SysSockGetFirstAdapterInfo
	#define CHK_SysSockGetFirstAdapterInfo  TRUE
	#define EXP_SysSockGetFirstAdapterInfo  CAL_CMEXPAPI( "SysSockGetFirstAdapterInfo" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockGetFirstAdapterInfo
	#define EXT_SysSockGetFirstAdapterInfo
	#define GET_SysSockGetFirstAdapterInfo(fl)  CAL_CMGETAPI( "SysSockGetFirstAdapterInfo" ) 
	#define CAL_SysSockGetFirstAdapterInfo  SysSockGetFirstAdapterInfo
	#define CHK_SysSockGetFirstAdapterInfo  TRUE
	#define EXP_SysSockGetFirstAdapterInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetFirstAdapterInfo", (RTS_UINTPTR)SysSockGetFirstAdapterInfo, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockGetFirstAdapterInfo
	#define EXT_SysSocketSysSockGetFirstAdapterInfo
	#define GET_SysSocketSysSockGetFirstAdapterInfo  ERR_OK
	#define CAL_SysSocketSysSockGetFirstAdapterInfo pISysSocket->ISysSockGetFirstAdapterInfo
	#define CHK_SysSocketSysSockGetFirstAdapterInfo (pISysSocket != NULL)
	#define EXP_SysSocketSysSockGetFirstAdapterInfo  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockGetFirstAdapterInfo
	#define EXT_SysSockGetFirstAdapterInfo
	#define GET_SysSockGetFirstAdapterInfo(fl)  CAL_CMGETAPI( "SysSockGetFirstAdapterInfo" ) 
	#define CAL_SysSockGetFirstAdapterInfo pISysSocket->ISysSockGetFirstAdapterInfo
	#define CHK_SysSockGetFirstAdapterInfo (pISysSocket != NULL)
	#define EXP_SysSockGetFirstAdapterInfo  CAL_CMEXPAPI( "SysSockGetFirstAdapterInfo" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockGetFirstAdapterInfo  PFSYSSOCKGETFIRSTADAPTERINFO pfSysSockGetFirstAdapterInfo;
	#define EXT_SysSockGetFirstAdapterInfo  extern PFSYSSOCKGETFIRSTADAPTERINFO pfSysSockGetFirstAdapterInfo;
	#define GET_SysSockGetFirstAdapterInfo(fl)  s_pfCMGetAPI2( "SysSockGetFirstAdapterInfo", (RTS_VOID_FCTPTR *)&pfSysSockGetFirstAdapterInfo, (fl), 0, 0)
	#define CAL_SysSockGetFirstAdapterInfo  pfSysSockGetFirstAdapterInfo
	#define CHK_SysSockGetFirstAdapterInfo  (pfSysSockGetFirstAdapterInfo != NULL)
	#define EXP_SysSockGetFirstAdapterInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetFirstAdapterInfo", (RTS_UINTPTR)SysSockGetFirstAdapterInfo, 0, 0) 
#endif




/**
 * <description>
 *	Get adapter information struct of the next network adapter.
 * </description>
 * <param name="pPrevAdapter" type="OUT">Pointer to adapter information struct of previous adapter.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Information struct of current network adapter</result>
 */
SOCK_ADAPTER_INFO* CDECL SysSockGetNextAdapterInfo(SOCK_ADAPTER_INFO *pPrevAdapter, RTS_RESULT *pResult);
typedef SOCK_ADAPTER_INFO* (CDECL * PFSYSSOCKGETNEXTADAPTERINFO) (SOCK_ADAPTER_INFO *pPrevAdapter, RTS_RESULT *pResult);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETNEXTADAPTERINFO_NOTIMPLEMENTED)
	#define USE_SysSockGetNextAdapterInfo
	#define EXT_SysSockGetNextAdapterInfo
	#define GET_SysSockGetNextAdapterInfo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockGetNextAdapterInfo(p0,p1)  (SOCK_ADAPTER_INFO*)ERR_NOTIMPLEMENTED
	#define CHK_SysSockGetNextAdapterInfo  FALSE
	#define EXP_SysSockGetNextAdapterInfo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockGetNextAdapterInfo
	#define EXT_SysSockGetNextAdapterInfo
	#define GET_SysSockGetNextAdapterInfo(fl)  CAL_CMGETAPI( "SysSockGetNextAdapterInfo" ) 
	#define CAL_SysSockGetNextAdapterInfo  SysSockGetNextAdapterInfo
	#define CHK_SysSockGetNextAdapterInfo  TRUE
	#define EXP_SysSockGetNextAdapterInfo  CAL_CMEXPAPI( "SysSockGetNextAdapterInfo" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockGetNextAdapterInfo
	#define EXT_SysSockGetNextAdapterInfo
	#define GET_SysSockGetNextAdapterInfo(fl)  CAL_CMGETAPI( "SysSockGetNextAdapterInfo" ) 
	#define CAL_SysSockGetNextAdapterInfo  SysSockGetNextAdapterInfo
	#define CHK_SysSockGetNextAdapterInfo  TRUE
	#define EXP_SysSockGetNextAdapterInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetNextAdapterInfo", (RTS_UINTPTR)SysSockGetNextAdapterInfo, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockGetNextAdapterInfo
	#define EXT_SysSocketSysSockGetNextAdapterInfo
	#define GET_SysSocketSysSockGetNextAdapterInfo  ERR_OK
	#define CAL_SysSocketSysSockGetNextAdapterInfo pISysSocket->ISysSockGetNextAdapterInfo
	#define CHK_SysSocketSysSockGetNextAdapterInfo (pISysSocket != NULL)
	#define EXP_SysSocketSysSockGetNextAdapterInfo  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockGetNextAdapterInfo
	#define EXT_SysSockGetNextAdapterInfo
	#define GET_SysSockGetNextAdapterInfo(fl)  CAL_CMGETAPI( "SysSockGetNextAdapterInfo" ) 
	#define CAL_SysSockGetNextAdapterInfo pISysSocket->ISysSockGetNextAdapterInfo
	#define CHK_SysSockGetNextAdapterInfo (pISysSocket != NULL)
	#define EXP_SysSockGetNextAdapterInfo  CAL_CMEXPAPI( "SysSockGetNextAdapterInfo" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockGetNextAdapterInfo  PFSYSSOCKGETNEXTADAPTERINFO pfSysSockGetNextAdapterInfo;
	#define EXT_SysSockGetNextAdapterInfo  extern PFSYSSOCKGETNEXTADAPTERINFO pfSysSockGetNextAdapterInfo;
	#define GET_SysSockGetNextAdapterInfo(fl)  s_pfCMGetAPI2( "SysSockGetNextAdapterInfo", (RTS_VOID_FCTPTR *)&pfSysSockGetNextAdapterInfo, (fl), 0, 0)
	#define CAL_SysSockGetNextAdapterInfo  pfSysSockGetNextAdapterInfo
	#define CHK_SysSockGetNextAdapterInfo  (pfSysSockGetNextAdapterInfo != NULL)
	#define EXP_SysSockGetNextAdapterInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetNextAdapterInfo", (RTS_UINTPTR)SysSockGetNextAdapterInfo, 0, 0) 
#endif




/**
 * <description>
 *	Set IP address and subnet mask of an adapter. 
 *  It depends on the device, whether the new ip address and subnet mask is reset during reboot or
 *  if it is retained. In general the caller should consider these as volatile. The combination 
 *  IP address = 0.0.0.0 and subnet mask = 0.0.0.0 can be used to remove the IP address from the adapter.
 *  After this there is no IP based communication possible anymore, until a new IP address is set.
 *  Replaces the functions SysSockSetIPAddress() and SysSockSetSubnetMask().
 * </description>
 * <param name="pwszAdapterName" type="IN">Adapter name provided by SysSockGetFirstAdapter() / SysSockGetNextAdapter()</param>
 * <param name="pIpAddr" type="IN">Ip address to set in network byte order.</param>
 * <param name="pNetMask" type="IN">Subnet mask to set in network byte order.</param>
 * <result>Error code:
 *	<ul>
 *		<li>ERR_PARAMETER: At least one of the passed pointers is NULL.</li>
 *		<li>ERR_OPERATION_DENIED: Adapter is not in white list or the operation was denied by the event EVT_SysSocket_BeforeSetIpAndMask.</li>
 *		<li>ERR_NO_OBJECT: Adapter with the specified name does not exist.</li>
 *		<li>ERR_NO_CHANGE: Adapter have a fix (not changeable) or dynamic (DHCP) ip address.</li>
 *		<li>ERR_FAILED: Ip adress/subnet mask could not be set.</li>
 *		<li>ERR_OK: Ip adress/subnet mask was set successfully.</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL SysSockSetIpAddressAndNetMask(RTS_WCHAR *pwszAdapterName, INADDR *pIpAddr, INADDR *pNetMask);
typedef RTS_RESULT (CDECL * PFSYSSOCKSETIPADDRESSANDNETMASK) (RTS_WCHAR *pwszAdapterName, INADDR *pIpAddr, INADDR *pNetMask);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSETIPADDRESSANDNETMASK_NOTIMPLEMENTED)
	#define USE_SysSockSetIpAddressAndNetMask
	#define EXT_SysSockSetIpAddressAndNetMask
	#define GET_SysSockSetIpAddressAndNetMask(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockSetIpAddressAndNetMask(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockSetIpAddressAndNetMask  FALSE
	#define EXP_SysSockSetIpAddressAndNetMask  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockSetIpAddressAndNetMask
	#define EXT_SysSockSetIpAddressAndNetMask
	#define GET_SysSockSetIpAddressAndNetMask(fl)  CAL_CMGETAPI( "SysSockSetIpAddressAndNetMask" ) 
	#define CAL_SysSockSetIpAddressAndNetMask  SysSockSetIpAddressAndNetMask
	#define CHK_SysSockSetIpAddressAndNetMask  TRUE
	#define EXP_SysSockSetIpAddressAndNetMask  CAL_CMEXPAPI( "SysSockSetIpAddressAndNetMask" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockSetIpAddressAndNetMask
	#define EXT_SysSockSetIpAddressAndNetMask
	#define GET_SysSockSetIpAddressAndNetMask(fl)  CAL_CMGETAPI( "SysSockSetIpAddressAndNetMask" ) 
	#define CAL_SysSockSetIpAddressAndNetMask  SysSockSetIpAddressAndNetMask
	#define CHK_SysSockSetIpAddressAndNetMask  TRUE
	#define EXP_SysSockSetIpAddressAndNetMask  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSetIpAddressAndNetMask", (RTS_UINTPTR)SysSockSetIpAddressAndNetMask, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockSetIpAddressAndNetMask
	#define EXT_SysSocketSysSockSetIpAddressAndNetMask
	#define GET_SysSocketSysSockSetIpAddressAndNetMask  ERR_OK
	#define CAL_SysSocketSysSockSetIpAddressAndNetMask pISysSocket->ISysSockSetIpAddressAndNetMask
	#define CHK_SysSocketSysSockSetIpAddressAndNetMask (pISysSocket != NULL)
	#define EXP_SysSocketSysSockSetIpAddressAndNetMask  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockSetIpAddressAndNetMask
	#define EXT_SysSockSetIpAddressAndNetMask
	#define GET_SysSockSetIpAddressAndNetMask(fl)  CAL_CMGETAPI( "SysSockSetIpAddressAndNetMask" ) 
	#define CAL_SysSockSetIpAddressAndNetMask pISysSocket->ISysSockSetIpAddressAndNetMask
	#define CHK_SysSockSetIpAddressAndNetMask (pISysSocket != NULL)
	#define EXP_SysSockSetIpAddressAndNetMask  CAL_CMEXPAPI( "SysSockSetIpAddressAndNetMask" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockSetIpAddressAndNetMask  PFSYSSOCKSETIPADDRESSANDNETMASK pfSysSockSetIpAddressAndNetMask;
	#define EXT_SysSockSetIpAddressAndNetMask  extern PFSYSSOCKSETIPADDRESSANDNETMASK pfSysSockSetIpAddressAndNetMask;
	#define GET_SysSockSetIpAddressAndNetMask(fl)  s_pfCMGetAPI2( "SysSockSetIpAddressAndNetMask", (RTS_VOID_FCTPTR *)&pfSysSockSetIpAddressAndNetMask, (fl), 0, 0)
	#define CAL_SysSockSetIpAddressAndNetMask  pfSysSockSetIpAddressAndNetMask
	#define CHK_SysSockSetIpAddressAndNetMask  (pfSysSockSetIpAddressAndNetMask != NULL)
	#define EXP_SysSockSetIpAddressAndNetMask  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSetIpAddressAndNetMask", (RTS_UINTPTR)SysSockSetIpAddressAndNetMask, 0, 0) 
#endif




/**
 * <description>
 *	Set default gateway address an adapter. 
 *  It depends on the device, whether the new gateway address is reset during reboot or
 *  if it is retained. In general the caller should consider this as volatile. The 
 *  gateway address = 0.0.0.0 can be used to remove the gateway address from the adapter. 
 *  After this there is not routing to other subnets possible.
 *
 *  Attention: The default gateway can only be changed if the adapter containing
 *   the current default gateway is whitelisted in the CODESYSControl.cfg. This function
 *   will not change the default gateway of any other adapter.
 * </description>
 * <param name="pwszAdapterName" type="IN">Adapter name provided by SysSockGetFirstAdapter() / SysSockGetNextAdapter()</param>
 * <param name="pGatewayAddr" type="IN">Ip address of the gateway to set in network byte order.</param>
 * <result>Error code:
 *	<ul>
 *		<li>ERR_PARAMETER: At least one of the passed pointers is NULL.</li>
 *		<li>ERR_OPERATION_DENIED: Adapter is not in white list or the operation was denied by the event EVT_SysSocket_BeforeSetGateway.</li>
 *		<li>ERR_NO_OBJECT: Adapter with the specified name does not exist.</li>
 *		<li>ERR_NO_CHANGE: There is a default gateway set on an adapter that is not whitelisted.</li>
 *		<li>ERR_FAILED: Default gateway could not be set.</li>
 *		<li>ERR_OK: Default gateway was set successfully.</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL SysSockSetDefaultGateway(RTS_WCHAR *pwszAdapterName, INADDR *pGatewayAddr);
typedef RTS_RESULT (CDECL * PFSYSSOCKSETDEFAULTGATEWAY) (RTS_WCHAR *pwszAdapterName, INADDR *pGatewayAddr);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKSETDEFAULTGATEWAY_NOTIMPLEMENTED)
	#define USE_SysSockSetDefaultGateway
	#define EXT_SysSockSetDefaultGateway
	#define GET_SysSockSetDefaultGateway(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockSetDefaultGateway(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockSetDefaultGateway  FALSE
	#define EXP_SysSockSetDefaultGateway  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockSetDefaultGateway
	#define EXT_SysSockSetDefaultGateway
	#define GET_SysSockSetDefaultGateway(fl)  CAL_CMGETAPI( "SysSockSetDefaultGateway" ) 
	#define CAL_SysSockSetDefaultGateway  SysSockSetDefaultGateway
	#define CHK_SysSockSetDefaultGateway  TRUE
	#define EXP_SysSockSetDefaultGateway  CAL_CMEXPAPI( "SysSockSetDefaultGateway" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockSetDefaultGateway
	#define EXT_SysSockSetDefaultGateway
	#define GET_SysSockSetDefaultGateway(fl)  CAL_CMGETAPI( "SysSockSetDefaultGateway" ) 
	#define CAL_SysSockSetDefaultGateway  SysSockSetDefaultGateway
	#define CHK_SysSockSetDefaultGateway  TRUE
	#define EXP_SysSockSetDefaultGateway  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSetDefaultGateway", (RTS_UINTPTR)SysSockSetDefaultGateway, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockSetDefaultGateway
	#define EXT_SysSocketSysSockSetDefaultGateway
	#define GET_SysSocketSysSockSetDefaultGateway  ERR_OK
	#define CAL_SysSocketSysSockSetDefaultGateway pISysSocket->ISysSockSetDefaultGateway
	#define CHK_SysSocketSysSockSetDefaultGateway (pISysSocket != NULL)
	#define EXP_SysSocketSysSockSetDefaultGateway  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockSetDefaultGateway
	#define EXT_SysSockSetDefaultGateway
	#define GET_SysSockSetDefaultGateway(fl)  CAL_CMGETAPI( "SysSockSetDefaultGateway" ) 
	#define CAL_SysSockSetDefaultGateway pISysSocket->ISysSockSetDefaultGateway
	#define CHK_SysSockSetDefaultGateway (pISysSocket != NULL)
	#define EXP_SysSockSetDefaultGateway  CAL_CMEXPAPI( "SysSockSetDefaultGateway" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockSetDefaultGateway  PFSYSSOCKSETDEFAULTGATEWAY pfSysSockSetDefaultGateway;
	#define EXT_SysSockSetDefaultGateway  extern PFSYSSOCKSETDEFAULTGATEWAY pfSysSockSetDefaultGateway;
	#define GET_SysSockSetDefaultGateway(fl)  s_pfCMGetAPI2( "SysSockSetDefaultGateway", (RTS_VOID_FCTPTR *)&pfSysSockSetDefaultGateway, (fl), 0, 0)
	#define CAL_SysSockSetDefaultGateway  pfSysSockSetDefaultGateway
	#define CHK_SysSockSetDefaultGateway  (pfSysSockSetDefaultGateway != NULL)
	#define EXP_SysSockSetDefaultGateway  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockSetDefaultGateway", (RTS_UINTPTR)SysSockSetDefaultGateway, 0, 0) 
#endif




/**
 * <description>
 *	Returns the socket address of the peer to which a socket is connected.
 *  The SysSockGetPeerName function can be used only on a connected socket. 
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pSockAddr" type="OUT">Socket address of the peer</param>
 * <param name="piSockAddrSize" type="INOUT">Pointer to size of socket address structure</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockGetPeerName(RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize);
typedef RTS_RESULT (CDECL * PFSYSSOCKGETPEERNAME) (RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETPEERNAME_NOTIMPLEMENTED)
	#define USE_SysSockGetPeerName
	#define EXT_SysSockGetPeerName
	#define GET_SysSockGetPeerName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockGetPeerName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockGetPeerName  FALSE
	#define EXP_SysSockGetPeerName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockGetPeerName
	#define EXT_SysSockGetPeerName
	#define GET_SysSockGetPeerName(fl)  CAL_CMGETAPI( "SysSockGetPeerName" ) 
	#define CAL_SysSockGetPeerName  SysSockGetPeerName
	#define CHK_SysSockGetPeerName  TRUE
	#define EXP_SysSockGetPeerName  CAL_CMEXPAPI( "SysSockGetPeerName" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockGetPeerName
	#define EXT_SysSockGetPeerName
	#define GET_SysSockGetPeerName(fl)  CAL_CMGETAPI( "SysSockGetPeerName" ) 
	#define CAL_SysSockGetPeerName  SysSockGetPeerName
	#define CHK_SysSockGetPeerName  TRUE
	#define EXP_SysSockGetPeerName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetPeerName", (RTS_UINTPTR)SysSockGetPeerName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockGetPeerName
	#define EXT_SysSocketSysSockGetPeerName
	#define GET_SysSocketSysSockGetPeerName  ERR_OK
	#define CAL_SysSocketSysSockGetPeerName(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockGetPeerName(p1,p2) : ((ISysSocket*)p0)->ISysSockGetPeerName(p1,p2))
	#define CHK_SysSocketSysSockGetPeerName  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockGetPeerName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockGetPeerName
	#define EXT_SysSockGetPeerName
	#define GET_SysSockGetPeerName(fl)  CAL_CMGETAPI( "SysSockGetPeerName" ) 
	#define CAL_SysSockGetPeerName(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockGetPeerName(p1,p2) : ((ISysSocket*)p0)->ISysSockGetPeerName(p1,p2))
	#define CHK_SysSockGetPeerName  (pISysSocket != NULL)
	#define EXP_SysSockGetPeerName  CAL_CMEXPAPI( "SysSockGetPeerName" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockGetPeerName  PFSYSSOCKGETPEERNAME pfSysSockGetPeerName;
	#define EXT_SysSockGetPeerName  extern PFSYSSOCKGETPEERNAME pfSysSockGetPeerName;
	#define GET_SysSockGetPeerName(fl)  s_pfCMGetAPI2( "SysSockGetPeerName", (RTS_VOID_FCTPTR *)&pfSysSockGetPeerName, (fl), 0, 0)
	#define CAL_SysSockGetPeerName  pfSysSockGetPeerName
	#define CHK_SysSockGetPeerName  (pfSysSockGetPeerName != NULL)
	#define EXP_SysSockGetPeerName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetPeerName", (RTS_UINTPTR)SysSockGetPeerName, 0, 0) 
#endif




/**
 * <description>
 *	Returns the socket address of the local socket.
 *  Usally this function is called to retrieve the local socket address on multihomed hosts or to get 
 *  the local port number in client implemntations. If the socket is neither connected nor locally bound to an address,
 *  the result of the function and the value stored in the object pointed to by pSockAddress is unspecified.
 * </description>
 * <param name="hSocket" type="IN">Handle to the socket</param>
 * <param name="pSockAddr" type="OUT">Socket address of the local socket</param>
 * <param name="piSockAddrSize" type="INOUT">Pointer to size of socket address structure</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysSockGetSockName(RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize);
typedef RTS_RESULT (CDECL * PFSYSSOCKGETSOCKNAME) (RTS_HANDLE hSocket, SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize);
#if defined(SYSSOCKET_NOTIMPLEMENTED) || defined(SYSSOCKGETSOCKNAME_NOTIMPLEMENTED)
	#define USE_SysSockGetSockName
	#define EXT_SysSockGetSockName
	#define GET_SysSockGetSockName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysSockGetSockName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysSockGetSockName  FALSE
	#define EXP_SysSockGetSockName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysSockGetSockName
	#define EXT_SysSockGetSockName
	#define GET_SysSockGetSockName(fl)  CAL_CMGETAPI( "SysSockGetSockName" ) 
	#define CAL_SysSockGetSockName  SysSockGetSockName
	#define CHK_SysSockGetSockName  TRUE
	#define EXP_SysSockGetSockName  CAL_CMEXPAPI( "SysSockGetSockName" ) 
#elif defined(MIXED_LINK) && !defined(SYSSOCKET_EXTERNAL)
	#define USE_SysSockGetSockName
	#define EXT_SysSockGetSockName
	#define GET_SysSockGetSockName(fl)  CAL_CMGETAPI( "SysSockGetSockName" ) 
	#define CAL_SysSockGetSockName  SysSockGetSockName
	#define CHK_SysSockGetSockName  TRUE
	#define EXP_SysSockGetSockName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetSockName", (RTS_UINTPTR)SysSockGetSockName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysSocketSysSockGetSockName
	#define EXT_SysSocketSysSockGetSockName
	#define GET_SysSocketSysSockGetSockName  ERR_OK
	#define CAL_SysSocketSysSockGetSockName(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockGetSockName(p1,p2) : ((ISysSocket*)p0)->ISysSockGetSockName(p1,p2))
	#define CHK_SysSocketSysSockGetSockName  (pISysSocket != NULL)
	#define EXP_SysSocketSysSockGetSockName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysSockGetSockName
	#define EXT_SysSockGetSockName
	#define GET_SysSockGetSockName(fl)  CAL_CMGETAPI( "SysSockGetSockName" ) 
	#define CAL_SysSockGetSockName(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysSocket->ISysSockGetSockName(p1,p2) : ((ISysSocket*)p0)->ISysSockGetSockName(p1,p2))
	#define CHK_SysSockGetSockName  (pISysSocket != NULL)
	#define EXP_SysSockGetSockName  CAL_CMEXPAPI( "SysSockGetSockName" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysSockGetSockName  PFSYSSOCKGETSOCKNAME pfSysSockGetSockName;
	#define EXT_SysSockGetSockName  extern PFSYSSOCKGETSOCKNAME pfSysSockGetSockName;
	#define GET_SysSockGetSockName(fl)  s_pfCMGetAPI2( "SysSockGetSockName", (RTS_VOID_FCTPTR *)&pfSysSockGetSockName, (fl), 0, 0)
	#define CAL_SysSockGetSockName  pfSysSockGetSockName
	#define CHK_SysSockGetSockName  (pfSysSockGetSockName != NULL)
	#define EXP_SysSockGetSockName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysSockGetSockName", (RTS_UINTPTR)SysSockGetSockName, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSSOCKCREATE ISysSockCreate;
 	PFSYSSOCKCLOSE ISysSockClose;
 	PFSYSSOCKSETOPTION ISysSockSetOption;
 	PFSYSSOCKGETOPTION ISysSockGetOption;
 	PFSYSSOCKBIND ISysSockBind;
 	PFSYSSOCKGETHOSTNAME ISysSockGetHostName;
 	PFSYSSOCKGETHOSTBYNAME ISysSockGetHostByName;
 	PFSYSSOCKINETNTOA ISysSockInetNtoa;
 	PFSYSSOCKINETADDR ISysSockInetAddr;
 	PFSYSSOCKHTONS ISysSockHtons;
 	PFSYSSOCKHTONL ISysSockHtonl;
 	PFSYSSOCKNTOHS ISysSockNtohs;
 	PFSYSSOCKNTOHL ISysSockNtohl;
 	PFSYSSOCKLISTEN ISysSockListen;
 	PFSYSSOCKACCEPT ISysSockAccept;
 	PFSYSSOCKSETIPADDRESS ISysSockSetIPAddress;
 	PFSYSSOCKCONNECT ISysSockConnect;
 	PFSYSSOCKSELECT ISysSockSelect;
 	PFSYSSOCKSHUTDOWN ISysSockShutdown;
 	PFSYSSOCKIOCTL ISysSockIoctl;
 	PFSYSSOCKRECV ISysSockRecv;
 	PFSYSSOCKSEND ISysSockSend;
 	PFSYSSOCKRECVFROM ISysSockRecvFrom;
 	PFSYSSOCKSENDTO ISysSockSendTo;
 	PFSYSSOCKCREATEUDP ISysSockCreateUdp;
 	PFSYSSOCKCLOSEUDP ISysSockCloseUdp;
 	PFSYSSOCKSENDTOUDP ISysSockSendToUdp;
 	PFSYSSOCKRECVFROMUDP ISysSockRecvFromUdp;
 	PFSYSSOCKGETRECVSIZEUDP ISysSockGetRecvSizeUdp;
 	PFSYSSOCKGETOSHANDLE ISysSockGetOSHandle;
 	PFSYSSOCKPING ISysSockPing;
 	PFSYSSOCKSETSUBNETMASK ISysSockSetSubnetMask;
 	PFSYSSOCKGETSUBNETMASK ISysSockGetSubnetMask;
 	PFSYSSOCKFDISSET ISysSockFdIsset;
 	PFSYSSOCKFDINIT ISysSockFdInit;
 	PFSYSSOCKFDZERO ISysSockFdZero;
 	PFSYSSOCKGETFIRSTADAPTERINFO ISysSockGetFirstAdapterInfo;
 	PFSYSSOCKGETNEXTADAPTERINFO ISysSockGetNextAdapterInfo;
 	PFSYSSOCKSETIPADDRESSANDNETMASK ISysSockSetIpAddressAndNetMask;
 	PFSYSSOCKSETDEFAULTGATEWAY ISysSockSetDefaultGateway;
 	PFSYSSOCKGETPEERNAME ISysSockGetPeerName;
 	PFSYSSOCKGETSOCKNAME ISysSockGetSockName;
 } ISysSocket_C;

#ifdef CPLUSPLUS
class ISysSocket : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISysSockCreate(int iAddressFamily, int iType, int iProtocol, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysSockClose(void) =0;
		virtual RTS_RESULT CDECL ISysSockSetOption(int iLevel, int iOption, char *pcOptionValue, int iOptionLen) =0;
		virtual RTS_RESULT CDECL ISysSockGetOption(int iLevel, int iOption, char *pcOptionValue, int *piOptionLen) =0;
		virtual RTS_RESULT CDECL ISysSockBind(SOCKADDRESS *pSockAddr, int iSockAddrSize) =0;
		virtual RTS_RESULT CDECL ISysSockGetHostName(char *pszHostName, int iNameLength) =0;
		virtual RTS_RESULT CDECL ISysSockGetHostByName(char *pszHostName, SOCK_HOSTENT *pHost) =0;
		virtual RTS_RESULT CDECL ISysSockInetNtoa(INADDR *pInAddr, char *pszIPAddr, int iIPAddrSize) =0;
		virtual RTS_RESULT CDECL ISysSockInetAddr(char *pszIPAddress, RTS_UI32 *pInAddr) =0;
		virtual unsigned short CDECL ISysSockHtons(unsigned short usHost) =0;
		virtual RTS_UI32 CDECL ISysSockHtonl(RTS_UI32 ulHost) =0;
		virtual unsigned short CDECL ISysSockNtohs(unsigned short usNet) =0;
		virtual RTS_UI32 CDECL ISysSockNtohl(RTS_UI32 ulNet) =0;
		virtual RTS_RESULT CDECL ISysSockListen(int iMaxConnections) =0;
		virtual RTS_HANDLE CDECL ISysSockAccept(SOCKADDRESS *pSockAddr, int *piSockAddrSize, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysSockSetIPAddress(char *pszCard, char *pszIPAddress) =0;
		virtual RTS_RESULT CDECL ISysSockConnect(SOCKADDRESS *pSockAddr, int iSockAddrSize) =0;
		virtual RTS_RESULT CDECL ISysSockSelect(int iWidth, SOCKET_FD_SET *fdRead, SOCKET_FD_SET *fdWrite, SOCKET_FD_SET *fdExcept, SOCKET_TIMEVAL *ptvTimeout, int *pnReady) =0;
		virtual RTS_RESULT CDECL ISysSockShutdown(int iHow) =0;
		virtual RTS_RESULT CDECL ISysSockIoctl(int iCommand, int *piParameter) =0;
		virtual RTS_SSIZE CDECL ISysSockRecv(char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, RTS_RESULT *pResult) =0;
		virtual RTS_SSIZE CDECL ISysSockSend(char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, RTS_RESULT *pResult) =0;
		virtual RTS_SSIZE CDECL ISysSockRecvFrom(char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, SOCKADDRESS *pSockAddr, int iSockAddrSize, RTS_RESULT *pResult) =0;
		virtual RTS_SSIZE CDECL ISysSockSendTo(char *pbyBuffer, RTS_SSIZE iBufferSize, int iFlags, SOCKADDRESS *pSockAddr, int iSockAddrSize, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysSockCreateUdp(int iSendPort, int iRecvPort, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysSockCloseUdp(void) =0;
		virtual RTS_SSIZE CDECL ISysSockSendToUdp(int iPort, char *pszDestAddress, unsigned char *pbyData, RTS_SSIZE iDataSize, RTS_RESULT *pResult) =0;
		virtual RTS_SSIZE CDECL ISysSockRecvFromUdp(unsigned char *pbyData, RTS_SSIZE iDataSize, UDP_REPLY *pReply, RTS_RESULT *pResult) =0;
		virtual RTS_SSIZE CDECL ISysSockGetRecvSizeUdp(int iTimeout, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysSockGetOSHandle(void) =0;
		virtual RTS_RESULT CDECL ISysSockPing(char *pszIPAddress, RTS_UI32 ulTimeout, RTS_UI32 *pulReplyTime) =0;
		virtual RTS_RESULT CDECL ISysSockSetSubnetMask(char *pszIPAddress, char *pszSubnetMask) =0;
		virtual RTS_RESULT CDECL ISysSockGetSubnetMask(char *pszIPAddress, char *pszSubnetMask, int iMaxSubnetMask) =0;
		virtual RTS_BOOL CDECL ISysSockFdIsset(SOCKET_FD_SET *pfs) =0;
		virtual RTS_RESULT CDECL ISysSockFdInit(SOCKET_FD_SET *pfs) =0;
		virtual void CDECL ISysSockFdZero(SOCKET_FD_SET *pfs) =0;
		virtual SOCK_ADAPTER_INFO* CDECL ISysSockGetFirstAdapterInfo(RTS_RESULT *pResult) =0;
		virtual SOCK_ADAPTER_INFO* CDECL ISysSockGetNextAdapterInfo(SOCK_ADAPTER_INFO *pPrevAdapter, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysSockSetIpAddressAndNetMask(RTS_WCHAR *pwszAdapterName, INADDR *pIpAddr, INADDR *pNetMask) =0;
		virtual RTS_RESULT CDECL ISysSockSetDefaultGateway(RTS_WCHAR *pwszAdapterName, INADDR *pGatewayAddr) =0;
		virtual RTS_RESULT CDECL ISysSockGetPeerName(SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize) =0;
		virtual RTS_RESULT CDECL ISysSockGetSockName(SOCKADDRESS *pSockAddr, RTS_I32 *piSockAddrSize) =0;
};
	#ifndef ITF_SysSocket
		#define ITF_SysSocket static ISysSocket *pISysSocket = NULL;
	#endif
	#define EXTITF_SysSocket
#else	/*CPLUSPLUS*/
	typedef ISysSocket_C		ISysSocket;
	#ifndef ITF_SysSocket
		#define ITF_SysSocket
	#endif
	#define EXTITF_SysSocket
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSSOCKETITF_H_*/
