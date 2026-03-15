 /**
 * <interfacename>CmpRouter</interfacename>
 * <description> 
 *	Interface for the communication router component.
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMPROUTERITF_H_
#define _CMPROUTERITF_H_

#include "CmpStd.h"

 

 




#include "CmpCommunicationLibItf.h"
#include "CmpBlkDrvItf.h"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Max. number of router instances. Allowed value range: 1..16. Normally no configuration is required. 
 *  If there is no specific configuration of main or subnets, the router creates a new router instance 
 *  for each available block driver instance and assigns its main network to the block driver instance. 
 *  If subnets are configured, then the router additionally limits the number of router instances to 
 *  (16 - number of configured subnets). Thanks parallel routing subnets are not necessary anymore.
 * </description>
 */
#define ROUTERKEY_INT_MAX_ROUTERS								"MaxRouters"
#ifndef ROUTERKEY_INT_MAX_ROUTERS_DEFAULT
	#define ROUTERKEY_INT_MAX_ROUTERS_DEFAULT					16
#endif

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Configures the parallel routing feature.
 *  By default there is no communication possible between the router instances. For server implementations
 *  above the router layer (CmpChannelServer or CmpNameServiceServer - typically used in the runtime system) 
 *  this is not necessary at all, because the server implementations listen on all router instances. 
 *  But on client side the implementations (CmpChannelClient or CmpNameServiceClient) are always bound
 *  to the first router instance. This means without the parallel routing feature a communication client
 *  (Gateway, PLCHandler, OPC Server, DataServer, ...) can only communicate to PLCs which are connected to
 *  a main net or subnet of the first local router instance of the client.
 *  Another aspect of parallel routing is the package forwarding. On PLCs all block driver instances are assigned
 *  to a main net of the router instances. But also on the Gateway there are typically at least a few router instances
 *  available. If a PLC or Gateway should forward a received package to a PLC which is connected to another interface, 
 *  there is also the parallel routing feature needed.
 *  Allowed values: 
 *  0: Parallel routing is completely disabled. Should only be used on PLCs without any client functionality or
 *     to limit the local client communication very hard to the first router instance.  
 *  1: Parallel routing is enabled for forwarding received packages to another interface and also local client
 *     implementations can communicate to all networks on all router instances. Is typically be used on PLCs
 *     to allow complex PLC networks. Furthermore this was for a long time also the default for the Gateway.
 *  2 (default): Parallel routing is enabled for the local client to let him communicate to all networks on 
 *     all router instances, but received packages are not forwarded to other interfaces. If there are no
 *     special routing requirements, this default value should be used.
 * </description>
 */
#define ROUTERKEY_INT_PARALLEL_ROUTING							"EnableParallelRouting"
#ifndef ROUTERKEY_INT_PARALLEL_ROUTING_DEFAULT
	#define ROUTERKEY_INT_PARALLEL_ROUTING_DEFAULT				2
#endif

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Enables/disables loop detection for broadcast packets. 
 *  Should only be disabled for debugging purposes.  
 * </description>
 */
#define ROUTERKEY_INT_LOOP_DETECTION							"EnableLoopDetection"
#define ROUTERKEY_INT_LOOP_DETECTION_DEFAULT					1

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *	Name of the router instance (Example: 0.Name=Router1).
 *  There is usually no need to configure this.
 * </description>
 */
#define ROUTERKEY_STRING_INSTANCE_NAME							"Name"
#ifndef ROUTERKEY_STRING_INSTANCE_NAME_DEFAULT_PREFIX
	#define ROUTERKEY_STRING_INSTANCE_NAME_DEFAULT_PREFIX			"Router"
#endif 

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *  Is used to bind a specific block driver instance to a router instance. By default the block driver 
 *  instances are assigned to router instances in the order of its registration at the router. And because
 *  of the routing features, there is usually no need for specific configurations.
 *  On client side (e. g. Gateway) the higher communication layers are linked to the first router instance.
 *  Therefore all devices connected to the network of the first router instance can be addressed without the
 *  the use of parallel routing and appear with a shorter addresses in the network scan. 
 *	If the name is set to "ether x", then the first registered instance of the CmpBlkDrvUdp is assigned.
 *  Example:
 *  0.MainNet=ether x
 *  1.MainNet=BlkDrvTcp
 * </description>
 */
#define ROUTERKEY_STRING_MAINNET								"MainNet"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *  Obsolete. Only available for compatibility reasons. Use implicit or explicit configured main nets instead. 
 *	Allows to configure the number of the router instances' subnets. (Example: 0.NumSubNets=1)
 *  Allowed value range: 1..15.
 *  Thanks parallel routing subnets are not necessary anymore. Must be used on client side (Gateway) only. 
 * </description>
 */
#define ROUTERKEY_INT_NUMSUBNETS								"NumSubnets"
#define ROUTERKEY_INT_NUMSUBNETS_DEFAULT						0

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *  Obsolete. Only available for compatibility reasons. Use implicit or explicit configured main nets instead. 
 *	Prefix for subnet settings. (Example: 0.Subnet.1.Interface=BlkDrvShm)
 * </description>
 */
#define ROUTERKEY_STRING_SUBNET_PREFIX							"Subnet"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *  Obsolete. Only available for compatibility reasons. Use implicit or explicit configured main nets instead. 
 *	Name of the block driver instance, which should be assigned to a specific subnet of the router. 
 *  Important: Of the devices connected to a physical network (e. g. serial line, shared memory channel), 
 *  only one may have the corresponding block driver instance configured as a subnet. So there must be only 
 *  one parent node in a network, which imposes the address on the others. (Example: 0.Subnet.1.Interface=BlkDrvShm)
 *	Note: The CmpBlkDrvTcp or "ether x" cannot be used in a subnet configuration. Furthermore the block drivers
 *  CmpBlkDrvUdp, CmpBlkDrvCanClient and CmpBlkDrvCanServer shall not be used in a subnet configuration. 
 *  If one of these three drivers are used anyway as subnet, the AddressUpdateInterval parameter must also be set
 *  to a meaningful value (e.g. 60) to activate the address update service. This is also true for all main net peers 
 *  in that network.
 * </description>
 */
#define ROUTERKEY_STRING_SUBNET_INTERFACE						"Interface"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description>
 *  Activates the address update service and configures the interval in seconds in which the service is triggered.
 *  The address update service is only necessary, if (exact) one of the devices connected to a physical network 
 *  (e. g. serial line, shared memory channel), have the corresponding block driver instance configured as a subnet.
 *  In this case all devices connected to this line must activate the address update service.
 *  The value 0 not only disables the sending of address notifications and address requests, but also lets the device
 *  completely discard received address update services for this network.
 *  Based on the strong recommendation to use CmpBlkDrvUdp, CmpBlkDrvTcp, CmpBlkDrvCanClient and CmpBlkDrvCanServer
 *  only in main network configurations, the AddressUpdateInterval for all their instances is set to 0, which means
 *  that the address update service is completely disabled by default. For all instances of the other block drivers 
 *  the default value defined below is used. This allows direct use in existing subnet configurations especially 
 *  in combination with older versions of the Gateway or the CODESYS Control runtime system. 
 *  In general, it is not necessary to configure a value that differs from the default. 
 *  Examples: 
 *	0.MainNet.AddressUpdateInterval=60
 *	0.Subnet.1.AddressUpdateInterval=60
 * </description>
 */
#define ROUTERKEY_INT_ADDR_UPDATE_INTERVAL						"AddressUpdateInterval"
#ifndef ROUTERKEY_INT_ADDR_UPDATE_INTERVAL_DEFAULT
	#define ROUTERKEY_INT_ADDR_UPDATE_INTERVAL_DEFAULT				60
#endif


enum tagProtocolId
{
	PID_ADDRESSREQUEST = 1,		
	PID_ADDRESSNOTIFICATION = 2,
	PID_NAMINGREQUEST = 3,
	PID_NAMINGRESPONSE = 4,
	PID_LAYER4 = 64,
};

typedef enum tagRouterPriority
{
	RPRIO_LOW = 0,
	RPRIO_NORMAL = 1,
	RPRIO_HIGH = 2,
	RPRIO_EMERGENCY = 3
} ROUTERPRIORITY;


/* Router package marker */ 
#define ROUTERPKG_MARKER 0xC5

#ifndef NO_PRAGMA_PACK
	#pragma pack(1)
#endif

/* 
 * The header layout of a router package 
 */
typedef struct tagRouterPkgHeader
{
	RTS_UI8 byPkgMarker;
		/* Must be set to ROUTERPKG_MARKER */
	RTS_UI8 byFlag1;
	RTS_UI8 byFlag2;
	RTS_UI8 byServiceId;
	RTS_UI8 byMessageId;
	RTS_UI8 byAddrLength;
} ROUTERPKGHEADER;

/* 
 * The header layout of a router package 2, 
 * containing 2 more bytes for identifying looping broadcasts
 */
typedef struct tagRouterPkgHeader2
{
	RTS_UI8 byPkgMarker;
		/* Must be set to ROUTERPKG_MARKER */
	RTS_UI8 byFlag1;
	RTS_UI8 byFlag2;
	RTS_UI8 byServiceId;
	RTS_UI8 byMessageId;
	RTS_UI8 byAddrLength;
	RTS_UI8 byUid1;
	RTS_UI8 byUid2;
} ROUTERPKGHEADER2;

#ifndef NO_PRAGMA_PACK
	#ifdef USE_PRAGMA_PACK_0
		#pragma pack(0)
	#else
		#pragma pack()
	#endif	
#endif


/* -- Interface exported to block drivers -- */

#define MAX_ROUTERNAME 20
#define MAX_BLOCKSIZE 512
#define MAX_INTERFACE_NAME 20

/**
 * <description> Open a block driver </description>
 * <param name="hInterface" type="IN">Handle to the interface</param>
 * <result>error code</result>
 */
/* static RTS_RESULT CDECL BdOpen(RTS_HANDLE hInterface); */
typedef RTS_RESULT (CDECL * PFBDOPEN) (RTS_HANDLE hInterface);

/**
 * <description> Close a block driver </description>
 * <param name="hInterface" type="IN">Handle to the interface</param>
 * <result>error code</result>
 */
/* static RTS_RESULT CDECL BdClose(RTS_HANDLE hInterface); */
typedef RTS_RESULT (CDECL * PFBDCLOSE) (RTS_HANDLE hInterface);

/**
 * <description> Send data over a block driver. Received reply is sent directly to the router </description>
 * <param name="hInterface" type="IN">Handle to the interface</param>
 * <param name="addrReceiver" type="IN">Logical address of receiver</param>
 * <param name="pduData" type="IN">Pointer to block to sent</param>
 * <result>error code. ERR_NOBUFFER, if the block driver can temporary not send. In this case
 * the higher layers try to send the same block again.</result>
 */
/* static RTS_RESULT CDECL BdSend(RTS_HANDLE hInterface, NETWORKADDRESS addrReceiver, PROTOCOL_DATA_UNIT pduData); */
typedef RTS_RESULT (CDECL * PFBDSEND) (RTS_HANDLE hInterface, NETWORKADDRESS addrReceiver, PROTOCOL_DATA_UNIT pduData);

typedef struct
{
	PFBDSEND pfBDSend;			/* Pointer to the block drivers send method. */
	RTS_HANDLE hInterface;		/* Interface handle within the block driver. This handle is passed to all calls to the block driver */
	int nMaxBlockSize;			/* The maximum size of a block that may be sent over this device. */
	int nNetworkAddressBitSize;	/* Number of bits occupied by an address of this driver */
	int bServiceChannel;		/* If TRUE, this device provides a service channel. */
	NETWORKADDRESS addrDevice;	/* Address of the device within it's subnet (CAN-Node ID, etc.). */
	char szName[MAX_INTERFACE_NAME]; /* human readable name of the device. Must be unique. Could be something like "eth0" or "Ethernet card #1". */
} NETWORKINTERFACEINFO;

typedef struct
{
	PFBDSEND pfBDSend;			/* Pointer to the block drivers send method. */
	RTS_HANDLE hInterface;		/* Interface handle within the block driver. This handle is passed to all calls to the block driver */
	int nMaxBlockSize;			/* The maximum size of a block that may be sent over this device. */
	int nNetworkAddressBitSize;	/* Number of bits occupied by an address of this driver */
	int bServiceChannel;		/* If TRUE, this device provides a service channel. */
	NETWORKADDRESS addrDevice;	/* Address of the device within it's subnet (CAN-Node ID, etc.). */
	char szName[MAX_INTERFACE_NAME]; /* human readable name of the device. Must be unique. Could be something like "eth0" or "Ethernet card #1". */
	RTS_UI16 usType;			/* Type of the adapter, see CmpCommunicationLibItf.h. */
	RTS_UI16 usDummy;			/* Not used at the moment, must be 0 for future compatibility. */
	void *pTypeSpecific;		/* Pointer to the type specific information, must be NULL, if no further information is available. */ 
} NETWORKINTERFACEINFO2;

typedef struct
{	
	PFBDOPEN  pfBDOpen;
	PFBDCLOSE pfBDClose;
} ONDEMANDNETWORKINTERFACE;


#ifdef __cplusplus
extern "C" {
#endif

/** <description>
 *   Called by a block driver to register one of it's devices with the router. Allows also
 *   to provide some block driver type specific information.
 * </description>
 * <param name="pDeviceInfo" type="IN">Describes the device to register </param>
 * <param name="phSubnet" type="OUT">
 *   Is set to the subnet handle that refers to this interface. The
 *   block driver must provide this value in each call to 
 *   RouterHandleData or RouterUnregisterNetworkInterface.
 * </param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterRegisterNetworkInterface2(NETWORKINTERFACEINFO2 *pInterfaceInfo, RTS_HANDLE * phSubnet);
typedef RTS_RESULT (CDECL * PFROUTERREGISTERNETWORKINTERFACE2) (NETWORKINTERFACEINFO2 *pInterfaceInfo, RTS_HANDLE * phSubnet);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERREGISTERNETWORKINTERFACE2_NOTIMPLEMENTED)
	#define USE_RouterRegisterNetworkInterface2
	#define EXT_RouterRegisterNetworkInterface2
	#define GET_RouterRegisterNetworkInterface2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterRegisterNetworkInterface2(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterRegisterNetworkInterface2  FALSE
	#define EXP_RouterRegisterNetworkInterface2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterRegisterNetworkInterface2
	#define EXT_RouterRegisterNetworkInterface2
	#define GET_RouterRegisterNetworkInterface2(fl)  CAL_CMGETAPI( "RouterRegisterNetworkInterface2" ) 
	#define CAL_RouterRegisterNetworkInterface2  RouterRegisterNetworkInterface2
	#define CHK_RouterRegisterNetworkInterface2  TRUE
	#define EXP_RouterRegisterNetworkInterface2  CAL_CMEXPAPI( "RouterRegisterNetworkInterface2" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterRegisterNetworkInterface2
	#define EXT_RouterRegisterNetworkInterface2
	#define GET_RouterRegisterNetworkInterface2(fl)  CAL_CMGETAPI( "RouterRegisterNetworkInterface2" ) 
	#define CAL_RouterRegisterNetworkInterface2  RouterRegisterNetworkInterface2
	#define CHK_RouterRegisterNetworkInterface2  TRUE
	#define EXP_RouterRegisterNetworkInterface2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterRegisterNetworkInterface2", (RTS_UINTPTR)RouterRegisterNetworkInterface2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterRegisterNetworkInterface2
	#define EXT_CmpRouterRouterRegisterNetworkInterface2
	#define GET_CmpRouterRouterRegisterNetworkInterface2  ERR_OK
	#define CAL_CmpRouterRouterRegisterNetworkInterface2 pICmpRouter->IRouterRegisterNetworkInterface2
	#define CHK_CmpRouterRouterRegisterNetworkInterface2 (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterRegisterNetworkInterface2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterRegisterNetworkInterface2
	#define EXT_RouterRegisterNetworkInterface2
	#define GET_RouterRegisterNetworkInterface2(fl)  CAL_CMGETAPI( "RouterRegisterNetworkInterface2" ) 
	#define CAL_RouterRegisterNetworkInterface2 pICmpRouter->IRouterRegisterNetworkInterface2
	#define CHK_RouterRegisterNetworkInterface2 (pICmpRouter != NULL)
	#define EXP_RouterRegisterNetworkInterface2  CAL_CMEXPAPI( "RouterRegisterNetworkInterface2" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterRegisterNetworkInterface2  PFROUTERREGISTERNETWORKINTERFACE2 pfRouterRegisterNetworkInterface2;
	#define EXT_RouterRegisterNetworkInterface2  extern PFROUTERREGISTERNETWORKINTERFACE2 pfRouterRegisterNetworkInterface2;
	#define GET_RouterRegisterNetworkInterface2(fl)  s_pfCMGetAPI2( "RouterRegisterNetworkInterface2", (RTS_VOID_FCTPTR *)&pfRouterRegisterNetworkInterface2, (fl), 0, 0)
	#define CAL_RouterRegisterNetworkInterface2  pfRouterRegisterNetworkInterface2
	#define CHK_RouterRegisterNetworkInterface2  (pfRouterRegisterNetworkInterface2 != NULL)
	#define EXP_RouterRegisterNetworkInterface2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterRegisterNetworkInterface2", (RTS_UINTPTR)RouterRegisterNetworkInterface2, 0, 0) 
#endif




/** <description>
 *   Called by a block driver to register one of it's devices with the router.
 * </description>
 * <param name="pDeviceInfo" type="IN">Describes the device to register </param>
 * <param name="phSubnet" type="OUT">
 *   Is set to the subnet handle that refers to this interface. The
 *   block driver must provide this value in each call to 
 *   RouterHandleData or RouterUnregisterNetworkInterface.
 * </param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterRegisterNetworkInterface(NETWORKINTERFACEINFO *pInterfaceInfo, RTS_HANDLE * phSubnet);
typedef RTS_RESULT (CDECL * PFROUTERREGISTERNETWORKINTERFACE) (NETWORKINTERFACEINFO *pInterfaceInfo, RTS_HANDLE * phSubnet);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERREGISTERNETWORKINTERFACE_NOTIMPLEMENTED)
	#define USE_RouterRegisterNetworkInterface
	#define EXT_RouterRegisterNetworkInterface
	#define GET_RouterRegisterNetworkInterface(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterRegisterNetworkInterface(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterRegisterNetworkInterface  FALSE
	#define EXP_RouterRegisterNetworkInterface  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterRegisterNetworkInterface
	#define EXT_RouterRegisterNetworkInterface
	#define GET_RouterRegisterNetworkInterface(fl)  CAL_CMGETAPI( "RouterRegisterNetworkInterface" ) 
	#define CAL_RouterRegisterNetworkInterface  RouterRegisterNetworkInterface
	#define CHK_RouterRegisterNetworkInterface  TRUE
	#define EXP_RouterRegisterNetworkInterface  CAL_CMEXPAPI( "RouterRegisterNetworkInterface" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterRegisterNetworkInterface
	#define EXT_RouterRegisterNetworkInterface
	#define GET_RouterRegisterNetworkInterface(fl)  CAL_CMGETAPI( "RouterRegisterNetworkInterface" ) 
	#define CAL_RouterRegisterNetworkInterface  RouterRegisterNetworkInterface
	#define CHK_RouterRegisterNetworkInterface  TRUE
	#define EXP_RouterRegisterNetworkInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterRegisterNetworkInterface", (RTS_UINTPTR)RouterRegisterNetworkInterface, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterRegisterNetworkInterface
	#define EXT_CmpRouterRouterRegisterNetworkInterface
	#define GET_CmpRouterRouterRegisterNetworkInterface  ERR_OK
	#define CAL_CmpRouterRouterRegisterNetworkInterface pICmpRouter->IRouterRegisterNetworkInterface
	#define CHK_CmpRouterRouterRegisterNetworkInterface (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterRegisterNetworkInterface  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterRegisterNetworkInterface
	#define EXT_RouterRegisterNetworkInterface
	#define GET_RouterRegisterNetworkInterface(fl)  CAL_CMGETAPI( "RouterRegisterNetworkInterface" ) 
	#define CAL_RouterRegisterNetworkInterface pICmpRouter->IRouterRegisterNetworkInterface
	#define CHK_RouterRegisterNetworkInterface (pICmpRouter != NULL)
	#define EXP_RouterRegisterNetworkInterface  CAL_CMEXPAPI( "RouterRegisterNetworkInterface" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterRegisterNetworkInterface  PFROUTERREGISTERNETWORKINTERFACE pfRouterRegisterNetworkInterface;
	#define EXT_RouterRegisterNetworkInterface  extern PFROUTERREGISTERNETWORKINTERFACE pfRouterRegisterNetworkInterface;
	#define GET_RouterRegisterNetworkInterface(fl)  s_pfCMGetAPI2( "RouterRegisterNetworkInterface", (RTS_VOID_FCTPTR *)&pfRouterRegisterNetworkInterface, (fl), 0, 0)
	#define CAL_RouterRegisterNetworkInterface  pfRouterRegisterNetworkInterface
	#define CHK_RouterRegisterNetworkInterface  (pfRouterRegisterNetworkInterface != NULL)
	#define EXP_RouterRegisterNetworkInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterRegisterNetworkInterface", (RTS_UINTPTR)RouterRegisterNetworkInterface, 0, 0) 
#endif




/** <description>
 *   At the moment pOnDemandInfo IS NOT USED by the CmpRouter!
 * 
 *   Called by an on-demand block driver to register one of it's devices with the router. An on-demand block driver
 *   is a block driver that is able to shutdown/open its connection on demand. Eg. for serial block drivers, so they
 *   can release their hardware interface as long as it isn't needed and open it only, when their is data to be sent.
 *   The block driver is supposed to be initially closed.
 * </description>
 * <param name="pDeviceInfo" type="IN">
 *    Describes the device to register 
 * </param>
 * <param name="pOnDemandInfo" type="IN">
 *    Contains additional functions, that allow for opening and closing of the interface.
 * </param>
 * <param name="phSubnet" type="OUT">
 *   Is set to the subnet handle that refers to this interface. The
 *   block driver must provide this value in each call to 
 *   RouterHandleData or RouterUnregisterNetworkInterface.
 * </param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterRegisterOnDemandNWInterface(NETWORKINTERFACEINFO *pInterfaceInfo, ONDEMANDNETWORKINTERFACE *pOnDemandInfo, RTS_HANDLE * phSubnet);
typedef RTS_RESULT (CDECL * PFROUTERREGISTERONDEMANDNWINTERFACE) (NETWORKINTERFACEINFO *pInterfaceInfo, ONDEMANDNETWORKINTERFACE *pOnDemandInfo, RTS_HANDLE * phSubnet);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERREGISTERONDEMANDNWINTERFACE_NOTIMPLEMENTED)
	#define USE_RouterRegisterOnDemandNWInterface
	#define EXT_RouterRegisterOnDemandNWInterface
	#define GET_RouterRegisterOnDemandNWInterface(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterRegisterOnDemandNWInterface(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterRegisterOnDemandNWInterface  FALSE
	#define EXP_RouterRegisterOnDemandNWInterface  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterRegisterOnDemandNWInterface
	#define EXT_RouterRegisterOnDemandNWInterface
	#define GET_RouterRegisterOnDemandNWInterface(fl)  CAL_CMGETAPI( "RouterRegisterOnDemandNWInterface" ) 
	#define CAL_RouterRegisterOnDemandNWInterface  RouterRegisterOnDemandNWInterface
	#define CHK_RouterRegisterOnDemandNWInterface  TRUE
	#define EXP_RouterRegisterOnDemandNWInterface  CAL_CMEXPAPI( "RouterRegisterOnDemandNWInterface" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterRegisterOnDemandNWInterface
	#define EXT_RouterRegisterOnDemandNWInterface
	#define GET_RouterRegisterOnDemandNWInterface(fl)  CAL_CMGETAPI( "RouterRegisterOnDemandNWInterface" ) 
	#define CAL_RouterRegisterOnDemandNWInterface  RouterRegisterOnDemandNWInterface
	#define CHK_RouterRegisterOnDemandNWInterface  TRUE
	#define EXP_RouterRegisterOnDemandNWInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterRegisterOnDemandNWInterface", (RTS_UINTPTR)RouterRegisterOnDemandNWInterface, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterRegisterOnDemandNWInterface
	#define EXT_CmpRouterRouterRegisterOnDemandNWInterface
	#define GET_CmpRouterRouterRegisterOnDemandNWInterface  ERR_OK
	#define CAL_CmpRouterRouterRegisterOnDemandNWInterface pICmpRouter->IRouterRegisterOnDemandNWInterface
	#define CHK_CmpRouterRouterRegisterOnDemandNWInterface (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterRegisterOnDemandNWInterface  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterRegisterOnDemandNWInterface
	#define EXT_RouterRegisterOnDemandNWInterface
	#define GET_RouterRegisterOnDemandNWInterface(fl)  CAL_CMGETAPI( "RouterRegisterOnDemandNWInterface" ) 
	#define CAL_RouterRegisterOnDemandNWInterface pICmpRouter->IRouterRegisterOnDemandNWInterface
	#define CHK_RouterRegisterOnDemandNWInterface (pICmpRouter != NULL)
	#define EXP_RouterRegisterOnDemandNWInterface  CAL_CMEXPAPI( "RouterRegisterOnDemandNWInterface" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterRegisterOnDemandNWInterface  PFROUTERREGISTERONDEMANDNWINTERFACE pfRouterRegisterOnDemandNWInterface;
	#define EXT_RouterRegisterOnDemandNWInterface  extern PFROUTERREGISTERONDEMANDNWINTERFACE pfRouterRegisterOnDemandNWInterface;
	#define GET_RouterRegisterOnDemandNWInterface(fl)  s_pfCMGetAPI2( "RouterRegisterOnDemandNWInterface", (RTS_VOID_FCTPTR *)&pfRouterRegisterOnDemandNWInterface, (fl), 0, 0)
	#define CAL_RouterRegisterOnDemandNWInterface  pfRouterRegisterOnDemandNWInterface
	#define CHK_RouterRegisterOnDemandNWInterface  (pfRouterRegisterOnDemandNWInterface != NULL)
	#define EXP_RouterRegisterOnDemandNWInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterRegisterOnDemandNWInterface", (RTS_UINTPTR)RouterRegisterOnDemandNWInterface, 0, 0) 
#endif




/** <description>
 *   Called by a block driver to unregister one of it's devices
 * </description>
 * <param name="hSubnet" type="IN">
 *   Subnet handle of the interface, which should be unregistered.
 * </param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterUnregisterNetworkInterface(RTS_HANDLE hSubnet);
typedef RTS_RESULT (CDECL * PFROUTERUNREGISTERNETWORKINTERFACE) (RTS_HANDLE hSubnet);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERUNREGISTERNETWORKINTERFACE_NOTIMPLEMENTED)
	#define USE_RouterUnregisterNetworkInterface
	#define EXT_RouterUnregisterNetworkInterface
	#define GET_RouterUnregisterNetworkInterface(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterUnregisterNetworkInterface(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterUnregisterNetworkInterface  FALSE
	#define EXP_RouterUnregisterNetworkInterface  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterUnregisterNetworkInterface
	#define EXT_RouterUnregisterNetworkInterface
	#define GET_RouterUnregisterNetworkInterface(fl)  CAL_CMGETAPI( "RouterUnregisterNetworkInterface" ) 
	#define CAL_RouterUnregisterNetworkInterface  RouterUnregisterNetworkInterface
	#define CHK_RouterUnregisterNetworkInterface  TRUE
	#define EXP_RouterUnregisterNetworkInterface  CAL_CMEXPAPI( "RouterUnregisterNetworkInterface" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterUnregisterNetworkInterface
	#define EXT_RouterUnregisterNetworkInterface
	#define GET_RouterUnregisterNetworkInterface(fl)  CAL_CMGETAPI( "RouterUnregisterNetworkInterface" ) 
	#define CAL_RouterUnregisterNetworkInterface  RouterUnregisterNetworkInterface
	#define CHK_RouterUnregisterNetworkInterface  TRUE
	#define EXP_RouterUnregisterNetworkInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterUnregisterNetworkInterface", (RTS_UINTPTR)RouterUnregisterNetworkInterface, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterUnregisterNetworkInterface
	#define EXT_CmpRouterRouterUnregisterNetworkInterface
	#define GET_CmpRouterRouterUnregisterNetworkInterface  ERR_OK
	#define CAL_CmpRouterRouterUnregisterNetworkInterface pICmpRouter->IRouterUnregisterNetworkInterface
	#define CHK_CmpRouterRouterUnregisterNetworkInterface (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterUnregisterNetworkInterface  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterUnregisterNetworkInterface
	#define EXT_RouterUnregisterNetworkInterface
	#define GET_RouterUnregisterNetworkInterface(fl)  CAL_CMGETAPI( "RouterUnregisterNetworkInterface" ) 
	#define CAL_RouterUnregisterNetworkInterface pICmpRouter->IRouterUnregisterNetworkInterface
	#define CHK_RouterUnregisterNetworkInterface (pICmpRouter != NULL)
	#define EXP_RouterUnregisterNetworkInterface  CAL_CMEXPAPI( "RouterUnregisterNetworkInterface" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterUnregisterNetworkInterface  PFROUTERUNREGISTERNETWORKINTERFACE pfRouterUnregisterNetworkInterface;
	#define EXT_RouterUnregisterNetworkInterface  extern PFROUTERUNREGISTERNETWORKINTERFACE pfRouterUnregisterNetworkInterface;
	#define GET_RouterUnregisterNetworkInterface(fl)  s_pfCMGetAPI2( "RouterUnregisterNetworkInterface", (RTS_VOID_FCTPTR *)&pfRouterUnregisterNetworkInterface, (fl), 0, 0)
	#define CAL_RouterUnregisterNetworkInterface  pfRouterUnregisterNetworkInterface
	#define CHK_RouterUnregisterNetworkInterface  (pfRouterUnregisterNetworkInterface != NULL)
	#define EXP_RouterUnregisterNetworkInterface  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterUnregisterNetworkInterface", (RTS_UINTPTR)RouterUnregisterNetworkInterface, 0, 0) 
#endif




/** <description>
 *   Called whenever the block driver receives a valid data package 
 * </description>
 * <param name="phSubnet" type="IN">
 *   The subnet id assigned to the receiving device during RouterRegisterDevice
 * </param>
 * <param name="addrSender" type="IN">
 *   The device address of the sender within the subnet
 * </param>
 * <param name="pduData" type="IN">The received data package</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterHandleData(RTS_HANDLE phSubnet, NETWORKADDRESS sender, PROTOCOL_DATA_UNIT pduData, int bIsBroadcast);
typedef RTS_RESULT (CDECL * PFROUTERHANDLEDATA) (RTS_HANDLE phSubnet, NETWORKADDRESS sender, PROTOCOL_DATA_UNIT pduData, int bIsBroadcast);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERHANDLEDATA_NOTIMPLEMENTED)
	#define USE_RouterHandleData
	#define EXT_RouterHandleData
	#define GET_RouterHandleData(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterHandleData(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterHandleData  FALSE
	#define EXP_RouterHandleData  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterHandleData
	#define EXT_RouterHandleData
	#define GET_RouterHandleData(fl)  CAL_CMGETAPI( "RouterHandleData" ) 
	#define CAL_RouterHandleData  RouterHandleData
	#define CHK_RouterHandleData  TRUE
	#define EXP_RouterHandleData  CAL_CMEXPAPI( "RouterHandleData" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterHandleData
	#define EXT_RouterHandleData
	#define GET_RouterHandleData(fl)  CAL_CMGETAPI( "RouterHandleData" ) 
	#define CAL_RouterHandleData  RouterHandleData
	#define CHK_RouterHandleData  TRUE
	#define EXP_RouterHandleData  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterHandleData", (RTS_UINTPTR)RouterHandleData, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterHandleData
	#define EXT_CmpRouterRouterHandleData
	#define GET_CmpRouterRouterHandleData  ERR_OK
	#define CAL_CmpRouterRouterHandleData pICmpRouter->IRouterHandleData
	#define CHK_CmpRouterRouterHandleData (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterHandleData  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterHandleData
	#define EXT_RouterHandleData
	#define GET_RouterHandleData(fl)  CAL_CMGETAPI( "RouterHandleData" ) 
	#define CAL_RouterHandleData pICmpRouter->IRouterHandleData
	#define CHK_RouterHandleData (pICmpRouter != NULL)
	#define EXP_RouterHandleData  CAL_CMEXPAPI( "RouterHandleData" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterHandleData  PFROUTERHANDLEDATA pfRouterHandleData;
	#define EXT_RouterHandleData  extern PFROUTERHANDLEDATA pfRouterHandleData;
	#define GET_RouterHandleData(fl)  s_pfCMGetAPI2( "RouterHandleData", (RTS_VOID_FCTPTR *)&pfRouterHandleData, (fl), 0, 0)
	#define CAL_RouterHandleData  pfRouterHandleData
	#define CHK_RouterHandleData  (pfRouterHandleData != NULL)
	#define EXP_RouterHandleData  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterHandleData", (RTS_UINTPTR)RouterHandleData, 0, 0) 
#endif




/** <description>
 *   Called by a block driver to get the addresses of a data package. 
 * </description>
 * <param name="pduData" type="IN">
 *   Data package, from which the addresses should be read.
 * </param>
 * <param name="pAddrReceiver" type="OUT">
 *   The function returns here the address of the receiver.
 * </param>
 * <param name="pAddrSender" type="OUT">
 *   The function returns here the address of the sender.
 * </param>
 * <param name="piDataOffset" type="OUT">
 *   If not NULL, the function returns here the offset (start) of the pay load. .
 * </param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterGetBlkAddresses(PROTOCOL_DATA_UNIT pduData, PEERADDRESS *pAddrReceiver, PEERADDRESS *pAddrSender, RTS_I32 *piDataOffset);
typedef RTS_RESULT (CDECL * PFROUTERGETBLKADDRESSES) (PROTOCOL_DATA_UNIT pduData, PEERADDRESS *pAddrReceiver, PEERADDRESS *pAddrSender, RTS_I32 *piDataOffset);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETBLKADDRESSES_NOTIMPLEMENTED)
	#define USE_RouterGetBlkAddresses
	#define EXT_RouterGetBlkAddresses
	#define GET_RouterGetBlkAddresses(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterGetBlkAddresses(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterGetBlkAddresses  FALSE
	#define EXP_RouterGetBlkAddresses  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterGetBlkAddresses
	#define EXT_RouterGetBlkAddresses
	#define GET_RouterGetBlkAddresses(fl)  CAL_CMGETAPI( "RouterGetBlkAddresses" ) 
	#define CAL_RouterGetBlkAddresses  RouterGetBlkAddresses
	#define CHK_RouterGetBlkAddresses  TRUE
	#define EXP_RouterGetBlkAddresses  CAL_CMEXPAPI( "RouterGetBlkAddresses" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterGetBlkAddresses
	#define EXT_RouterGetBlkAddresses
	#define GET_RouterGetBlkAddresses(fl)  CAL_CMGETAPI( "RouterGetBlkAddresses" ) 
	#define CAL_RouterGetBlkAddresses  RouterGetBlkAddresses
	#define CHK_RouterGetBlkAddresses  TRUE
	#define EXP_RouterGetBlkAddresses  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetBlkAddresses", (RTS_UINTPTR)RouterGetBlkAddresses, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterGetBlkAddresses
	#define EXT_CmpRouterRouterGetBlkAddresses
	#define GET_CmpRouterRouterGetBlkAddresses  ERR_OK
	#define CAL_CmpRouterRouterGetBlkAddresses pICmpRouter->IRouterGetBlkAddresses
	#define CHK_CmpRouterRouterGetBlkAddresses (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterGetBlkAddresses  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterGetBlkAddresses
	#define EXT_RouterGetBlkAddresses
	#define GET_RouterGetBlkAddresses(fl)  CAL_CMGETAPI( "RouterGetBlkAddresses" ) 
	#define CAL_RouterGetBlkAddresses pICmpRouter->IRouterGetBlkAddresses
	#define CHK_RouterGetBlkAddresses (pICmpRouter != NULL)
	#define EXP_RouterGetBlkAddresses  CAL_CMEXPAPI( "RouterGetBlkAddresses" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterGetBlkAddresses  PFROUTERGETBLKADDRESSES pfRouterGetBlkAddresses;
	#define EXT_RouterGetBlkAddresses  extern PFROUTERGETBLKADDRESSES pfRouterGetBlkAddresses;
	#define GET_RouterGetBlkAddresses(fl)  s_pfCMGetAPI2( "RouterGetBlkAddresses", (RTS_VOID_FCTPTR *)&pfRouterGetBlkAddresses, (fl), 0, 0)
	#define CAL_RouterGetBlkAddresses  pfRouterGetBlkAddresses
	#define CHK_RouterGetBlkAddresses  (pfRouterGetBlkAddresses != NULL)
	#define EXP_RouterGetBlkAddresses  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetBlkAddresses", (RTS_UINTPTR)RouterGetBlkAddresses, 0, 0) 
#endif




/** <description>
 *   Called by a block driver to compare two peer addresses. 
 * </description>
 * <param name="pAddr1" type="IN">
 *   First address to compare
 * </param>
 * <param name="pAddr2" type="IN">
 *   Second address to compare
 * </param>
 * <result>TRUE if the addresses are equal, else FALSE</result>
 */
RTS_BOOL CDECL RouterCompareAddresses(PEERADDRESS *pAddr1, PEERADDRESS *pAddr2);
typedef RTS_BOOL (CDECL * PFROUTERCOMPAREADDRESSES) (PEERADDRESS *pAddr1, PEERADDRESS *pAddr2);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERCOMPAREADDRESSES_NOTIMPLEMENTED)
	#define USE_RouterCompareAddresses
	#define EXT_RouterCompareAddresses
	#define GET_RouterCompareAddresses(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterCompareAddresses(p0,p1)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_RouterCompareAddresses  FALSE
	#define EXP_RouterCompareAddresses  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterCompareAddresses
	#define EXT_RouterCompareAddresses
	#define GET_RouterCompareAddresses(fl)  CAL_CMGETAPI( "RouterCompareAddresses" ) 
	#define CAL_RouterCompareAddresses  RouterCompareAddresses
	#define CHK_RouterCompareAddresses  TRUE
	#define EXP_RouterCompareAddresses  CAL_CMEXPAPI( "RouterCompareAddresses" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterCompareAddresses
	#define EXT_RouterCompareAddresses
	#define GET_RouterCompareAddresses(fl)  CAL_CMGETAPI( "RouterCompareAddresses" ) 
	#define CAL_RouterCompareAddresses  RouterCompareAddresses
	#define CHK_RouterCompareAddresses  TRUE
	#define EXP_RouterCompareAddresses  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterCompareAddresses", (RTS_UINTPTR)RouterCompareAddresses, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterCompareAddresses
	#define EXT_CmpRouterRouterCompareAddresses
	#define GET_CmpRouterRouterCompareAddresses  ERR_OK
	#define CAL_CmpRouterRouterCompareAddresses pICmpRouter->IRouterCompareAddresses
	#define CHK_CmpRouterRouterCompareAddresses (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterCompareAddresses  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterCompareAddresses
	#define EXT_RouterCompareAddresses
	#define GET_RouterCompareAddresses(fl)  CAL_CMGETAPI( "RouterCompareAddresses" ) 
	#define CAL_RouterCompareAddresses pICmpRouter->IRouterCompareAddresses
	#define CHK_RouterCompareAddresses (pICmpRouter != NULL)
	#define EXP_RouterCompareAddresses  CAL_CMEXPAPI( "RouterCompareAddresses" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterCompareAddresses  PFROUTERCOMPAREADDRESSES pfRouterCompareAddresses;
	#define EXT_RouterCompareAddresses  extern PFROUTERCOMPAREADDRESSES pfRouterCompareAddresses;
	#define GET_RouterCompareAddresses(fl)  s_pfCMGetAPI2( "RouterCompareAddresses", (RTS_VOID_FCTPTR *)&pfRouterCompareAddresses, (fl), 0, 0)
	#define CAL_RouterCompareAddresses  pfRouterCompareAddresses
	#define CHK_RouterCompareAddresses  (pfRouterCompareAddresses != NULL)
	#define EXP_RouterCompareAddresses  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterCompareAddresses", (RTS_UINTPTR)RouterCompareAddresses, 0, 0) 
#endif





/* -- General Router Interface -- */

/** <description>
 *   Let the router calculate the CoDeSys peer address of a node. This works only for block drivers, which
 *   have a unique instance, e. g. the BlkDrvTcp. The given network address contains in this case the IP-address
 *	 and the port of the node, in the  block driver specific format.
 *   Additionally this function can be used to get the address of the first router instance of the own 
 *   runtime system.
 * </description>
 * <param name="usBlkDrvType" type="IN">
 *   Type of the block driver, see CmpCommunicationLibItf.h.
 *   If set to RTS_BLK_DRV_TYPE_NONE, the address of the first router instance is returned. In this case
 *   the next two parameters are ignored.
 * </param>
 * <param name="byNetworkAddressBitSize" type="IN">
 *   Length of the specified network address in bits. Must match to the setting in the block driver.
 *   Should be 0, if RTS_BLK_DRV_TYPE_NONE is used to get the own router address.
 * </param>
 * <param name="pNetworkAddr" type="IN">
 *   Network address of the node, for which the peer address should be calculated.
 *   Should be NULL, if RTS_BLK_DRV_TYPE_NONE is used to get the own router address.
 * </param>
 * <param name="pNodeAddr" type="OUT">
 *   The function returns here the calculated node address.
 * </param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterCalculateNodeAddr(RTS_UI16 usBlkDrvType, RTS_UI8 byNetworkAddressBitSize, NETWORKADDRESS *pNetworkAddr, NODEADDRESS *pNodeAddr);
typedef RTS_RESULT (CDECL * PFROUTERCALCULATENODEADDR) (RTS_UI16 usBlkDrvType, RTS_UI8 byNetworkAddressBitSize, NETWORKADDRESS *pNetworkAddr, NODEADDRESS *pNodeAddr);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERCALCULATENODEADDR_NOTIMPLEMENTED)
	#define USE_RouterCalculateNodeAddr
	#define EXT_RouterCalculateNodeAddr
	#define GET_RouterCalculateNodeAddr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterCalculateNodeAddr(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterCalculateNodeAddr  FALSE
	#define EXP_RouterCalculateNodeAddr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterCalculateNodeAddr
	#define EXT_RouterCalculateNodeAddr
	#define GET_RouterCalculateNodeAddr(fl)  CAL_CMGETAPI( "RouterCalculateNodeAddr" ) 
	#define CAL_RouterCalculateNodeAddr  RouterCalculateNodeAddr
	#define CHK_RouterCalculateNodeAddr  TRUE
	#define EXP_RouterCalculateNodeAddr  CAL_CMEXPAPI( "RouterCalculateNodeAddr" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterCalculateNodeAddr
	#define EXT_RouterCalculateNodeAddr
	#define GET_RouterCalculateNodeAddr(fl)  CAL_CMGETAPI( "RouterCalculateNodeAddr" ) 
	#define CAL_RouterCalculateNodeAddr  RouterCalculateNodeAddr
	#define CHK_RouterCalculateNodeAddr  TRUE
	#define EXP_RouterCalculateNodeAddr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterCalculateNodeAddr", (RTS_UINTPTR)RouterCalculateNodeAddr, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterCalculateNodeAddr
	#define EXT_CmpRouterRouterCalculateNodeAddr
	#define GET_CmpRouterRouterCalculateNodeAddr  ERR_OK
	#define CAL_CmpRouterRouterCalculateNodeAddr pICmpRouter->IRouterCalculateNodeAddr
	#define CHK_CmpRouterRouterCalculateNodeAddr (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterCalculateNodeAddr  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterCalculateNodeAddr
	#define EXT_RouterCalculateNodeAddr
	#define GET_RouterCalculateNodeAddr(fl)  CAL_CMGETAPI( "RouterCalculateNodeAddr" ) 
	#define CAL_RouterCalculateNodeAddr pICmpRouter->IRouterCalculateNodeAddr
	#define CHK_RouterCalculateNodeAddr (pICmpRouter != NULL)
	#define EXP_RouterCalculateNodeAddr  CAL_CMEXPAPI( "RouterCalculateNodeAddr" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterCalculateNodeAddr  PFROUTERCALCULATENODEADDR pfRouterCalculateNodeAddr;
	#define EXT_RouterCalculateNodeAddr  extern PFROUTERCALCULATENODEADDR pfRouterCalculateNodeAddr;
	#define GET_RouterCalculateNodeAddr(fl)  s_pfCMGetAPI2( "RouterCalculateNodeAddr", (RTS_VOID_FCTPTR *)&pfRouterCalculateNodeAddr, (fl), 0, 0)
	#define CAL_RouterCalculateNodeAddr  pfRouterCalculateNodeAddr
	#define CHK_RouterCalculateNodeAddr  (pfRouterCalculateNodeAddr != NULL)
	#define EXP_RouterCalculateNodeAddr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterCalculateNodeAddr", (RTS_UINTPTR)RouterCalculateNodeAddr, 0, 0) 
#endif




/**
 * <description>
 *   Get the handle to the router instance with the provided name.
 * </description>
 * <param name="szName" type="IN">
 *	Name of the router. This parameter may be NULL to request a handle to the default router.
 * </param>
 * <param name="phRouter" type="OUT">
 *  If a router exists with the specified name, then the handle of the router is written into this
 *  parameter. Otherwise it is set to RTS_INVALID_HANDLE.
 * </param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterGetInstanceByName(char *szName, RTS_HANDLE *phRouter);
typedef RTS_RESULT (CDECL * PFROUTERGETINSTANCEBYNAME) (char *szName, RTS_HANDLE *phRouter);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETINSTANCEBYNAME_NOTIMPLEMENTED)
	#define USE_RouterGetInstanceByName
	#define EXT_RouterGetInstanceByName
	#define GET_RouterGetInstanceByName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterGetInstanceByName(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterGetInstanceByName  FALSE
	#define EXP_RouterGetInstanceByName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterGetInstanceByName
	#define EXT_RouterGetInstanceByName
	#define GET_RouterGetInstanceByName(fl)  CAL_CMGETAPI( "RouterGetInstanceByName" ) 
	#define CAL_RouterGetInstanceByName  RouterGetInstanceByName
	#define CHK_RouterGetInstanceByName  TRUE
	#define EXP_RouterGetInstanceByName  CAL_CMEXPAPI( "RouterGetInstanceByName" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterGetInstanceByName
	#define EXT_RouterGetInstanceByName
	#define GET_RouterGetInstanceByName(fl)  CAL_CMGETAPI( "RouterGetInstanceByName" ) 
	#define CAL_RouterGetInstanceByName  RouterGetInstanceByName
	#define CHK_RouterGetInstanceByName  TRUE
	#define EXP_RouterGetInstanceByName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetInstanceByName", (RTS_UINTPTR)RouterGetInstanceByName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterGetInstanceByName
	#define EXT_CmpRouterRouterGetInstanceByName
	#define GET_CmpRouterRouterGetInstanceByName  ERR_OK
	#define CAL_CmpRouterRouterGetInstanceByName pICmpRouter->IRouterGetInstanceByName
	#define CHK_CmpRouterRouterGetInstanceByName (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterGetInstanceByName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterGetInstanceByName
	#define EXT_RouterGetInstanceByName
	#define GET_RouterGetInstanceByName(fl)  CAL_CMGETAPI( "RouterGetInstanceByName" ) 
	#define CAL_RouterGetInstanceByName pICmpRouter->IRouterGetInstanceByName
	#define CHK_RouterGetInstanceByName (pICmpRouter != NULL)
	#define EXP_RouterGetInstanceByName  CAL_CMEXPAPI( "RouterGetInstanceByName" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterGetInstanceByName  PFROUTERGETINSTANCEBYNAME pfRouterGetInstanceByName;
	#define EXT_RouterGetInstanceByName  extern PFROUTERGETINSTANCEBYNAME pfRouterGetInstanceByName;
	#define GET_RouterGetInstanceByName(fl)  s_pfCMGetAPI2( "RouterGetInstanceByName", (RTS_VOID_FCTPTR *)&pfRouterGetInstanceByName, (fl), 0, 0)
	#define CAL_RouterGetInstanceByName  pfRouterGetInstanceByName
	#define CHK_RouterGetInstanceByName  (pfRouterGetInstanceByName != NULL)
	#define EXP_RouterGetInstanceByName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetInstanceByName", (RTS_UINTPTR)RouterGetInstanceByName, 0, 0) 
#endif




/**
 * <description>
 *   Get the name of a router specified by handle
 * </description>
 * <param name="hRouter" type="IN">Handle to router</param>
 * <param name="pszName" type="OUT">Pointer to get router name</param>
 * <param name="nMaxLen" type="IN">Maximum buffer length of pszName</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterGetName(RTS_HANDLE hRouter, char *pszName, int nMaxLen);
typedef RTS_RESULT (CDECL * PFROUTERGETNAME) (RTS_HANDLE hRouter, char *pszName, int nMaxLen);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETNAME_NOTIMPLEMENTED)
	#define USE_RouterGetName
	#define EXT_RouterGetName
	#define GET_RouterGetName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterGetName(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterGetName  FALSE
	#define EXP_RouterGetName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterGetName
	#define EXT_RouterGetName
	#define GET_RouterGetName(fl)  CAL_CMGETAPI( "RouterGetName" ) 
	#define CAL_RouterGetName  RouterGetName
	#define CHK_RouterGetName  TRUE
	#define EXP_RouterGetName  CAL_CMEXPAPI( "RouterGetName" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterGetName
	#define EXT_RouterGetName
	#define GET_RouterGetName(fl)  CAL_CMGETAPI( "RouterGetName" ) 
	#define CAL_RouterGetName  RouterGetName
	#define CHK_RouterGetName  TRUE
	#define EXP_RouterGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetName", (RTS_UINTPTR)RouterGetName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterGetName
	#define EXT_CmpRouterRouterGetName
	#define GET_CmpRouterRouterGetName  ERR_OK
	#define CAL_CmpRouterRouterGetName pICmpRouter->IRouterGetName
	#define CHK_CmpRouterRouterGetName (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterGetName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterGetName
	#define EXT_RouterGetName
	#define GET_RouterGetName(fl)  CAL_CMGETAPI( "RouterGetName" ) 
	#define CAL_RouterGetName pICmpRouter->IRouterGetName
	#define CHK_RouterGetName (pICmpRouter != NULL)
	#define EXP_RouterGetName  CAL_CMEXPAPI( "RouterGetName" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterGetName  PFROUTERGETNAME pfRouterGetName;
	#define EXT_RouterGetName  extern PFROUTERGETNAME pfRouterGetName;
	#define GET_RouterGetName(fl)  s_pfCMGetAPI2( "RouterGetName", (RTS_VOID_FCTPTR *)&pfRouterGetName, (fl), 0, 0)
	#define CAL_RouterGetName  pfRouterGetName
	#define CHK_RouterGetName  (pfRouterGetName != NULL)
	#define EXP_RouterGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetName", (RTS_UINTPTR)RouterGetName, 0, 0) 
#endif




/**
 * <description>
 *   Get the max. size of messages, which can be sent by higher layers to this peer address
 * </description>
 * <param name="hRouter" type="IN">Handle to router. If set to RTS_INVALID_HANDLE, the first router instance is used.</param>
 * <param name="addrPeer" type="IN">Address to check</param>
 * <param name="byMaxSize" type="OUT">Max payload of the router message.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterGetMaxMessageSize(RTS_HANDLE hRouter, PEERADDRESS addrPeer, RTS_UI16 *usMaxSize);
typedef RTS_RESULT (CDECL * PFROUTERGETMAXMESSAGESIZE) (RTS_HANDLE hRouter, PEERADDRESS addrPeer, RTS_UI16 *usMaxSize);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETMAXMESSAGESIZE_NOTIMPLEMENTED)
	#define USE_RouterGetMaxMessageSize
	#define EXT_RouterGetMaxMessageSize
	#define GET_RouterGetMaxMessageSize(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterGetMaxMessageSize(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterGetMaxMessageSize  FALSE
	#define EXP_RouterGetMaxMessageSize  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterGetMaxMessageSize
	#define EXT_RouterGetMaxMessageSize
	#define GET_RouterGetMaxMessageSize(fl)  CAL_CMGETAPI( "RouterGetMaxMessageSize" ) 
	#define CAL_RouterGetMaxMessageSize  RouterGetMaxMessageSize
	#define CHK_RouterGetMaxMessageSize  TRUE
	#define EXP_RouterGetMaxMessageSize  CAL_CMEXPAPI( "RouterGetMaxMessageSize" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterGetMaxMessageSize
	#define EXT_RouterGetMaxMessageSize
	#define GET_RouterGetMaxMessageSize(fl)  CAL_CMGETAPI( "RouterGetMaxMessageSize" ) 
	#define CAL_RouterGetMaxMessageSize  RouterGetMaxMessageSize
	#define CHK_RouterGetMaxMessageSize  TRUE
	#define EXP_RouterGetMaxMessageSize  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetMaxMessageSize", (RTS_UINTPTR)RouterGetMaxMessageSize, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterGetMaxMessageSize
	#define EXT_CmpRouterRouterGetMaxMessageSize
	#define GET_CmpRouterRouterGetMaxMessageSize  ERR_OK
	#define CAL_CmpRouterRouterGetMaxMessageSize pICmpRouter->IRouterGetMaxMessageSize
	#define CHK_CmpRouterRouterGetMaxMessageSize (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterGetMaxMessageSize  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterGetMaxMessageSize
	#define EXT_RouterGetMaxMessageSize
	#define GET_RouterGetMaxMessageSize(fl)  CAL_CMGETAPI( "RouterGetMaxMessageSize" ) 
	#define CAL_RouterGetMaxMessageSize pICmpRouter->IRouterGetMaxMessageSize
	#define CHK_RouterGetMaxMessageSize (pICmpRouter != NULL)
	#define EXP_RouterGetMaxMessageSize  CAL_CMEXPAPI( "RouterGetMaxMessageSize" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterGetMaxMessageSize  PFROUTERGETMAXMESSAGESIZE pfRouterGetMaxMessageSize;
	#define EXT_RouterGetMaxMessageSize  extern PFROUTERGETMAXMESSAGESIZE pfRouterGetMaxMessageSize;
	#define GET_RouterGetMaxMessageSize(fl)  s_pfCMGetAPI2( "RouterGetMaxMessageSize", (RTS_VOID_FCTPTR *)&pfRouterGetMaxMessageSize, (fl), 0, 0)
	#define CAL_RouterGetMaxMessageSize  pfRouterGetMaxMessageSize
	#define CHK_RouterGetMaxMessageSize  (pfRouterGetMaxMessageSize != NULL)
	#define EXP_RouterGetMaxMessageSize  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetMaxMessageSize", (RTS_UINTPTR)RouterGetMaxMessageSize, 0, 0) 
#endif




/**
 * <description>
 *   Get the max. size of messages, which can be sent by higher layers to this peer address
 * </description>
 * <param name="hRouter" type="IN">Handle to router</param>
 * <param name="usSumAddrLen" type="IN">Sum of sender and receiver address length</param>
 * <param name="byMaxSize" type="OUT">Max payload of the router message.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterGetMaxMessageSizeByAddressLength(RTS_HANDLE hRouter, RTS_UI16 usSumAddrLen, RTS_UI16 *usMaxSize);
typedef RTS_RESULT (CDECL * PFROUTERGETMAXMESSAGESIZEBYADDRESSLENGTH) (RTS_HANDLE hRouter, RTS_UI16 usSumAddrLen, RTS_UI16 *usMaxSize);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETMAXMESSAGESIZEBYADDRESSLENGTH_NOTIMPLEMENTED)
	#define USE_RouterGetMaxMessageSizeByAddressLength
	#define EXT_RouterGetMaxMessageSizeByAddressLength
	#define GET_RouterGetMaxMessageSizeByAddressLength(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterGetMaxMessageSizeByAddressLength(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterGetMaxMessageSizeByAddressLength  FALSE
	#define EXP_RouterGetMaxMessageSizeByAddressLength  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterGetMaxMessageSizeByAddressLength
	#define EXT_RouterGetMaxMessageSizeByAddressLength
	#define GET_RouterGetMaxMessageSizeByAddressLength(fl)  CAL_CMGETAPI( "RouterGetMaxMessageSizeByAddressLength" ) 
	#define CAL_RouterGetMaxMessageSizeByAddressLength  RouterGetMaxMessageSizeByAddressLength
	#define CHK_RouterGetMaxMessageSizeByAddressLength  TRUE
	#define EXP_RouterGetMaxMessageSizeByAddressLength  CAL_CMEXPAPI( "RouterGetMaxMessageSizeByAddressLength" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterGetMaxMessageSizeByAddressLength
	#define EXT_RouterGetMaxMessageSizeByAddressLength
	#define GET_RouterGetMaxMessageSizeByAddressLength(fl)  CAL_CMGETAPI( "RouterGetMaxMessageSizeByAddressLength" ) 
	#define CAL_RouterGetMaxMessageSizeByAddressLength  RouterGetMaxMessageSizeByAddressLength
	#define CHK_RouterGetMaxMessageSizeByAddressLength  TRUE
	#define EXP_RouterGetMaxMessageSizeByAddressLength  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetMaxMessageSizeByAddressLength", (RTS_UINTPTR)RouterGetMaxMessageSizeByAddressLength, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterGetMaxMessageSizeByAddressLength
	#define EXT_CmpRouterRouterGetMaxMessageSizeByAddressLength
	#define GET_CmpRouterRouterGetMaxMessageSizeByAddressLength  ERR_OK
	#define CAL_CmpRouterRouterGetMaxMessageSizeByAddressLength pICmpRouter->IRouterGetMaxMessageSizeByAddressLength
	#define CHK_CmpRouterRouterGetMaxMessageSizeByAddressLength (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterGetMaxMessageSizeByAddressLength  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterGetMaxMessageSizeByAddressLength
	#define EXT_RouterGetMaxMessageSizeByAddressLength
	#define GET_RouterGetMaxMessageSizeByAddressLength(fl)  CAL_CMGETAPI( "RouterGetMaxMessageSizeByAddressLength" ) 
	#define CAL_RouterGetMaxMessageSizeByAddressLength pICmpRouter->IRouterGetMaxMessageSizeByAddressLength
	#define CHK_RouterGetMaxMessageSizeByAddressLength (pICmpRouter != NULL)
	#define EXP_RouterGetMaxMessageSizeByAddressLength  CAL_CMEXPAPI( "RouterGetMaxMessageSizeByAddressLength" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterGetMaxMessageSizeByAddressLength  PFROUTERGETMAXMESSAGESIZEBYADDRESSLENGTH pfRouterGetMaxMessageSizeByAddressLength;
	#define EXT_RouterGetMaxMessageSizeByAddressLength  extern PFROUTERGETMAXMESSAGESIZEBYADDRESSLENGTH pfRouterGetMaxMessageSizeByAddressLength;
	#define GET_RouterGetMaxMessageSizeByAddressLength(fl)  s_pfCMGetAPI2( "RouterGetMaxMessageSizeByAddressLength", (RTS_VOID_FCTPTR *)&pfRouterGetMaxMessageSizeByAddressLength, (fl), 0, 0)
	#define CAL_RouterGetMaxMessageSizeByAddressLength  pfRouterGetMaxMessageSizeByAddressLength
	#define CHK_RouterGetMaxMessageSizeByAddressLength  (pfRouterGetMaxMessageSizeByAddressLength != NULL)
	#define EXP_RouterGetMaxMessageSizeByAddressLength  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetMaxMessageSizeByAddressLength", (RTS_UINTPTR)RouterGetMaxMessageSizeByAddressLength, 0, 0) 
#endif




/**
 * <description>
 *   Get the sum of the address lengths of the current router and the addrPeer.
 * </description>
 * <param name="hRouter" type="IN">Handle to router. If set to RTS_INVALID_HANDLE, the first router instance is used.</param>
 * <param name="addrPeer" type="IN">Address to check</param>
 * <param name="byMaxSize" type="OUT">Sum of router address and addrPeer length</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterGetMaxAddressSize(RTS_HANDLE hRouter, PEERADDRESS addrPeer, RTS_UI8 *byMaxSize);
typedef RTS_RESULT (CDECL * PFROUTERGETMAXADDRESSSIZE) (RTS_HANDLE hRouter, PEERADDRESS addrPeer, RTS_UI8 *byMaxSize);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETMAXADDRESSSIZE_NOTIMPLEMENTED)
	#define USE_RouterGetMaxAddressSize
	#define EXT_RouterGetMaxAddressSize
	#define GET_RouterGetMaxAddressSize(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterGetMaxAddressSize(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterGetMaxAddressSize  FALSE
	#define EXP_RouterGetMaxAddressSize  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterGetMaxAddressSize
	#define EXT_RouterGetMaxAddressSize
	#define GET_RouterGetMaxAddressSize(fl)  CAL_CMGETAPI( "RouterGetMaxAddressSize" ) 
	#define CAL_RouterGetMaxAddressSize  RouterGetMaxAddressSize
	#define CHK_RouterGetMaxAddressSize  TRUE
	#define EXP_RouterGetMaxAddressSize  CAL_CMEXPAPI( "RouterGetMaxAddressSize" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterGetMaxAddressSize
	#define EXT_RouterGetMaxAddressSize
	#define GET_RouterGetMaxAddressSize(fl)  CAL_CMGETAPI( "RouterGetMaxAddressSize" ) 
	#define CAL_RouterGetMaxAddressSize  RouterGetMaxAddressSize
	#define CHK_RouterGetMaxAddressSize  TRUE
	#define EXP_RouterGetMaxAddressSize  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetMaxAddressSize", (RTS_UINTPTR)RouterGetMaxAddressSize, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterGetMaxAddressSize
	#define EXT_CmpRouterRouterGetMaxAddressSize
	#define GET_CmpRouterRouterGetMaxAddressSize  ERR_OK
	#define CAL_CmpRouterRouterGetMaxAddressSize pICmpRouter->IRouterGetMaxAddressSize
	#define CHK_CmpRouterRouterGetMaxAddressSize (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterGetMaxAddressSize  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterGetMaxAddressSize
	#define EXT_RouterGetMaxAddressSize
	#define GET_RouterGetMaxAddressSize(fl)  CAL_CMGETAPI( "RouterGetMaxAddressSize" ) 
	#define CAL_RouterGetMaxAddressSize pICmpRouter->IRouterGetMaxAddressSize
	#define CHK_RouterGetMaxAddressSize (pICmpRouter != NULL)
	#define EXP_RouterGetMaxAddressSize  CAL_CMEXPAPI( "RouterGetMaxAddressSize" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterGetMaxAddressSize  PFROUTERGETMAXADDRESSSIZE pfRouterGetMaxAddressSize;
	#define EXT_RouterGetMaxAddressSize  extern PFROUTERGETMAXADDRESSSIZE pfRouterGetMaxAddressSize;
	#define GET_RouterGetMaxAddressSize(fl)  s_pfCMGetAPI2( "RouterGetMaxAddressSize", (RTS_VOID_FCTPTR *)&pfRouterGetMaxAddressSize, (fl), 0, 0)
	#define CAL_RouterGetMaxAddressSize  pfRouterGetMaxAddressSize
	#define CHK_RouterGetMaxAddressSize  (pfRouterGetMaxAddressSize != NULL)
	#define EXP_RouterGetMaxAddressSize  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetMaxAddressSize", (RTS_UINTPTR)RouterGetMaxAddressSize, 0, 0) 
#endif




/** <description>
 *   Protocol handlers call this function to send a data package.
 * </description>
 * <param name="hRouter" type="IN">Handle to the router</param>
 * <param name="addrReceiver" type="IN">
 *   Address of the receiver. Relative addresses are allowed as well as absolute ones.
 * </param>
 * <param name="nProtocolId" type="IN">Identifies the protocol handler on the receiving host.</param>
 * <param name="byMessageId" type="IN">typically 0</param>
 * <param name="prio" type="IN">Priority of the message.</param>
 * <param name="pduData" type="IN">The data to be sent.</param>
 * <param name="bUseQueue" type="IN">Defines if the message should be queued, if it can not be send at once.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterSend2(RTS_HANDLE hRouter, PEERADDRESS addrReceiver, int nProtocolId, RTS_UI8 byMessageId, ROUTERPRIORITY prio, PROTOCOL_DATA_UNIT pduData, RTS_BOOL bUseQueue);
typedef RTS_RESULT (CDECL * PFROUTERSEND2) (RTS_HANDLE hRouter, PEERADDRESS addrReceiver, int nProtocolId, RTS_UI8 byMessageId, ROUTERPRIORITY prio, PROTOCOL_DATA_UNIT pduData, RTS_BOOL bUseQueue);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERSEND2_NOTIMPLEMENTED)
	#define USE_RouterSend2
	#define EXT_RouterSend2
	#define GET_RouterSend2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterSend2(p0,p1,p2,p3,p4,p5,p6)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterSend2  FALSE
	#define EXP_RouterSend2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterSend2
	#define EXT_RouterSend2
	#define GET_RouterSend2(fl)  CAL_CMGETAPI( "RouterSend2" ) 
	#define CAL_RouterSend2  RouterSend2
	#define CHK_RouterSend2  TRUE
	#define EXP_RouterSend2  CAL_CMEXPAPI( "RouterSend2" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterSend2
	#define EXT_RouterSend2
	#define GET_RouterSend2(fl)  CAL_CMGETAPI( "RouterSend2" ) 
	#define CAL_RouterSend2  RouterSend2
	#define CHK_RouterSend2  TRUE
	#define EXP_RouterSend2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterSend2", (RTS_UINTPTR)RouterSend2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterSend2
	#define EXT_CmpRouterRouterSend2
	#define GET_CmpRouterRouterSend2  ERR_OK
	#define CAL_CmpRouterRouterSend2 pICmpRouter->IRouterSend2
	#define CHK_CmpRouterRouterSend2 (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterSend2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterSend2
	#define EXT_RouterSend2
	#define GET_RouterSend2(fl)  CAL_CMGETAPI( "RouterSend2" ) 
	#define CAL_RouterSend2 pICmpRouter->IRouterSend2
	#define CHK_RouterSend2 (pICmpRouter != NULL)
	#define EXP_RouterSend2  CAL_CMEXPAPI( "RouterSend2" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterSend2  PFROUTERSEND2 pfRouterSend2;
	#define EXT_RouterSend2  extern PFROUTERSEND2 pfRouterSend2;
	#define GET_RouterSend2(fl)  s_pfCMGetAPI2( "RouterSend2", (RTS_VOID_FCTPTR *)&pfRouterSend2, (fl), 0, 0)
	#define CAL_RouterSend2  pfRouterSend2
	#define CHK_RouterSend2  (pfRouterSend2 != NULL)
	#define EXP_RouterSend2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterSend2", (RTS_UINTPTR)RouterSend2, 0, 0) 
#endif




/** <description>
 *   Protocol handlers call this function to send a data package.
 * </description>
 * <param name="hRouter" type="IN">Handle to the router</param>
 * <param name="addrReceiver" type="IN">
 *   Address of the receiver. Relative addresses are allowed as well as absolute ones.
 * </param>
 * <param name="nProtocolId" type="IN">Identifies the protocol handler on the receiving host.</param>
 * <param name="byMessageId" type="IN">typically 0</param>
 * <param name="prio" type="IN">Priority of the message.</param>
 * <param name="pduData" type="IN">The data to be sent.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterSend(RTS_HANDLE hRouter, PEERADDRESS addrReceiver, int nProtocolId, RTS_UI8 byMessageId, ROUTERPRIORITY prio, PROTOCOL_DATA_UNIT pduData);
typedef RTS_RESULT (CDECL * PFROUTERSEND) (RTS_HANDLE hRouter, PEERADDRESS addrReceiver, int nProtocolId, RTS_UI8 byMessageId, ROUTERPRIORITY prio, PROTOCOL_DATA_UNIT pduData);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERSEND_NOTIMPLEMENTED)
	#define USE_RouterSend
	#define EXT_RouterSend
	#define GET_RouterSend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterSend(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterSend  FALSE
	#define EXP_RouterSend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterSend
	#define EXT_RouterSend
	#define GET_RouterSend(fl)  CAL_CMGETAPI( "RouterSend" ) 
	#define CAL_RouterSend  RouterSend
	#define CHK_RouterSend  TRUE
	#define EXP_RouterSend  CAL_CMEXPAPI( "RouterSend" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterSend
	#define EXT_RouterSend
	#define GET_RouterSend(fl)  CAL_CMGETAPI( "RouterSend" ) 
	#define CAL_RouterSend  RouterSend
	#define CHK_RouterSend  TRUE
	#define EXP_RouterSend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterSend", (RTS_UINTPTR)RouterSend, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterSend
	#define EXT_CmpRouterRouterSend
	#define GET_CmpRouterRouterSend  ERR_OK
	#define CAL_CmpRouterRouterSend pICmpRouter->IRouterSend
	#define CHK_CmpRouterRouterSend (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterSend  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterSend
	#define EXT_RouterSend
	#define GET_RouterSend(fl)  CAL_CMGETAPI( "RouterSend" ) 
	#define CAL_RouterSend pICmpRouter->IRouterSend
	#define CHK_RouterSend (pICmpRouter != NULL)
	#define EXP_RouterSend  CAL_CMEXPAPI( "RouterSend" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterSend  PFROUTERSEND pfRouterSend;
	#define EXT_RouterSend  extern PFROUTERSEND pfRouterSend;
	#define GET_RouterSend(fl)  s_pfCMGetAPI2( "RouterSend", (RTS_VOID_FCTPTR *)&pfRouterSend, (fl), 0, 0)
	#define CAL_RouterSend  pfRouterSend
	#define CHK_RouterSend  (pfRouterSend != NULL)
	#define EXP_RouterSend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterSend", (RTS_UINTPTR)RouterSend, 0, 0) 
#endif




/** <description>
 *   Get the routers node address.
 * </description>
 * <param name="hRouter" typ="IN">Handle to router. If set to RTS_INVALID_HANDLE, the first router instance is used.</param>
 * <param name="pAddrRouter" type="OUT">Is set to the node address of the router</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterGetHostAddress(RTS_HANDLE hRouter, NODEADDRESS *pAddrRouter);
typedef RTS_RESULT (CDECL * PFROUTERGETHOSTADDRESS) (RTS_HANDLE hRouter, NODEADDRESS *pAddrRouter);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETHOSTADDRESS_NOTIMPLEMENTED)
	#define USE_RouterGetHostAddress
	#define EXT_RouterGetHostAddress
	#define GET_RouterGetHostAddress(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterGetHostAddress(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterGetHostAddress  FALSE
	#define EXP_RouterGetHostAddress  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterGetHostAddress
	#define EXT_RouterGetHostAddress
	#define GET_RouterGetHostAddress(fl)  CAL_CMGETAPI( "RouterGetHostAddress" ) 
	#define CAL_RouterGetHostAddress  RouterGetHostAddress
	#define CHK_RouterGetHostAddress  TRUE
	#define EXP_RouterGetHostAddress  CAL_CMEXPAPI( "RouterGetHostAddress" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterGetHostAddress
	#define EXT_RouterGetHostAddress
	#define GET_RouterGetHostAddress(fl)  CAL_CMGETAPI( "RouterGetHostAddress" ) 
	#define CAL_RouterGetHostAddress  RouterGetHostAddress
	#define CHK_RouterGetHostAddress  TRUE
	#define EXP_RouterGetHostAddress  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetHostAddress", (RTS_UINTPTR)RouterGetHostAddress, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterGetHostAddress
	#define EXT_CmpRouterRouterGetHostAddress
	#define GET_CmpRouterRouterGetHostAddress  ERR_OK
	#define CAL_CmpRouterRouterGetHostAddress pICmpRouter->IRouterGetHostAddress
	#define CHK_CmpRouterRouterGetHostAddress (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterGetHostAddress  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterGetHostAddress
	#define EXT_RouterGetHostAddress
	#define GET_RouterGetHostAddress(fl)  CAL_CMGETAPI( "RouterGetHostAddress" ) 
	#define CAL_RouterGetHostAddress pICmpRouter->IRouterGetHostAddress
	#define CHK_RouterGetHostAddress (pICmpRouter != NULL)
	#define EXP_RouterGetHostAddress  CAL_CMEXPAPI( "RouterGetHostAddress" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterGetHostAddress  PFROUTERGETHOSTADDRESS pfRouterGetHostAddress;
	#define EXT_RouterGetHostAddress  extern PFROUTERGETHOSTADDRESS pfRouterGetHostAddress;
	#define GET_RouterGetHostAddress(fl)  s_pfCMGetAPI2( "RouterGetHostAddress", (RTS_VOID_FCTPTR *)&pfRouterGetHostAddress, (fl), 0, 0)
	#define CAL_RouterGetHostAddress  pfRouterGetHostAddress
	#define CHK_RouterGetHostAddress  (pfRouterGetHostAddress != NULL)
	#define EXP_RouterGetHostAddress  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetHostAddress", (RTS_UINTPTR)RouterGetHostAddress, 0, 0) 
#endif




/** <description>
 *   Get the node address of the parent node.
 * </description>
 * <param name="hRouter" typ="IN">Handle to router. If set to RTS_INVALID_HANDLE, the first router instance is used.</param>
 * <param name="pAddrParent" type="OUT">Is set to the node address of the router</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL RouterGetParentAddress(RTS_HANDLE hRouter, NODEADDRESS *pAddrParent);
typedef RTS_RESULT (CDECL * PFROUTERGETPARENTADDRESS) (RTS_HANDLE hRouter, NODEADDRESS *pAddrParent);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETPARENTADDRESS_NOTIMPLEMENTED)
	#define USE_RouterGetParentAddress
	#define EXT_RouterGetParentAddress
	#define GET_RouterGetParentAddress(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterGetParentAddress(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterGetParentAddress  FALSE
	#define EXP_RouterGetParentAddress  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterGetParentAddress
	#define EXT_RouterGetParentAddress
	#define GET_RouterGetParentAddress(fl)  CAL_CMGETAPI( "RouterGetParentAddress" ) 
	#define CAL_RouterGetParentAddress  RouterGetParentAddress
	#define CHK_RouterGetParentAddress  TRUE
	#define EXP_RouterGetParentAddress  CAL_CMEXPAPI( "RouterGetParentAddress" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterGetParentAddress
	#define EXT_RouterGetParentAddress
	#define GET_RouterGetParentAddress(fl)  CAL_CMGETAPI( "RouterGetParentAddress" ) 
	#define CAL_RouterGetParentAddress  RouterGetParentAddress
	#define CHK_RouterGetParentAddress  TRUE
	#define EXP_RouterGetParentAddress  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetParentAddress", (RTS_UINTPTR)RouterGetParentAddress, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterGetParentAddress
	#define EXT_CmpRouterRouterGetParentAddress
	#define GET_CmpRouterRouterGetParentAddress  ERR_OK
	#define CAL_CmpRouterRouterGetParentAddress pICmpRouter->IRouterGetParentAddress
	#define CHK_CmpRouterRouterGetParentAddress (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterGetParentAddress  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterGetParentAddress
	#define EXT_RouterGetParentAddress
	#define GET_RouterGetParentAddress(fl)  CAL_CMGETAPI( "RouterGetParentAddress" ) 
	#define CAL_RouterGetParentAddress pICmpRouter->IRouterGetParentAddress
	#define CHK_RouterGetParentAddress (pICmpRouter != NULL)
	#define EXP_RouterGetParentAddress  CAL_CMEXPAPI( "RouterGetParentAddress" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterGetParentAddress  PFROUTERGETPARENTADDRESS pfRouterGetParentAddress;
	#define EXT_RouterGetParentAddress  extern PFROUTERGETPARENTADDRESS pfRouterGetParentAddress;
	#define GET_RouterGetParentAddress(fl)  s_pfCMGetAPI2( "RouterGetParentAddress", (RTS_VOID_FCTPTR *)&pfRouterGetParentAddress, (fl), 0, 0)
	#define CAL_RouterGetParentAddress  pfRouterGetParentAddress
	#define CHK_RouterGetParentAddress  (pfRouterGetParentAddress != NULL)
	#define EXP_RouterGetParentAddress  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetParentAddress", (RTS_UINTPTR)RouterGetParentAddress, 0, 0) 
#endif




/** <description>
*   Get the block driver type RTS_BLK_DRV_TYPE_xxx of the referred network interface. See CmpCommunicationLibItf.
* </description>
* <param name="hNetworkInterface" typ="IN">Handle of network instance.</param>
* <param name="pbyBlkDrvType" type="OUT">Pointer to return block driver type of the network interface.</param>
* <result>error code</result>
*/
RTS_RESULT CDECL RouterGetBlkDrvType(RTS_HANDLE hNetworkInterface, RTS_UI8 *pbyBlkDrvType);
typedef RTS_RESULT (CDECL * PFROUTERGETBLKDRVTYPE) (RTS_HANDLE hNetworkInterface, RTS_UI8 *pbyBlkDrvType);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETBLKDRVTYPE_NOTIMPLEMENTED)
	#define USE_RouterGetBlkDrvType
	#define EXT_RouterGetBlkDrvType
	#define GET_RouterGetBlkDrvType(fl)  ERR_NOTIMPLEMENTED
	#define CAL_RouterGetBlkDrvType(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_RouterGetBlkDrvType  FALSE
	#define EXP_RouterGetBlkDrvType  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_RouterGetBlkDrvType
	#define EXT_RouterGetBlkDrvType
	#define GET_RouterGetBlkDrvType(fl)  CAL_CMGETAPI( "RouterGetBlkDrvType" ) 
	#define CAL_RouterGetBlkDrvType  RouterGetBlkDrvType
	#define CHK_RouterGetBlkDrvType  TRUE
	#define EXP_RouterGetBlkDrvType  CAL_CMEXPAPI( "RouterGetBlkDrvType" ) 
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_RouterGetBlkDrvType
	#define EXT_RouterGetBlkDrvType
	#define GET_RouterGetBlkDrvType(fl)  CAL_CMGETAPI( "RouterGetBlkDrvType" ) 
	#define CAL_RouterGetBlkDrvType  RouterGetBlkDrvType
	#define CHK_RouterGetBlkDrvType  TRUE
	#define EXP_RouterGetBlkDrvType  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetBlkDrvType", (RTS_UINTPTR)RouterGetBlkDrvType, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterRouterGetBlkDrvType
	#define EXT_CmpRouterRouterGetBlkDrvType
	#define GET_CmpRouterRouterGetBlkDrvType  ERR_OK
	#define CAL_CmpRouterRouterGetBlkDrvType pICmpRouter->IRouterGetBlkDrvType
	#define CHK_CmpRouterRouterGetBlkDrvType (pICmpRouter != NULL)
	#define EXP_CmpRouterRouterGetBlkDrvType  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_RouterGetBlkDrvType
	#define EXT_RouterGetBlkDrvType
	#define GET_RouterGetBlkDrvType(fl)  CAL_CMGETAPI( "RouterGetBlkDrvType" ) 
	#define CAL_RouterGetBlkDrvType pICmpRouter->IRouterGetBlkDrvType
	#define CHK_RouterGetBlkDrvType (pICmpRouter != NULL)
	#define EXP_RouterGetBlkDrvType  CAL_CMEXPAPI( "RouterGetBlkDrvType" ) 
#else /* DYNAMIC_LINK */
	#define USE_RouterGetBlkDrvType  PFROUTERGETBLKDRVTYPE pfRouterGetBlkDrvType;
	#define EXT_RouterGetBlkDrvType  extern PFROUTERGETBLKDRVTYPE pfRouterGetBlkDrvType;
	#define GET_RouterGetBlkDrvType(fl)  s_pfCMGetAPI2( "RouterGetBlkDrvType", (RTS_VOID_FCTPTR *)&pfRouterGetBlkDrvType, (fl), 0, 0)
	#define CAL_RouterGetBlkDrvType  pfRouterGetBlkDrvType
	#define CHK_RouterGetBlkDrvType  (pfRouterGetBlkDrvType != NULL)
	#define EXP_RouterGetBlkDrvType  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"RouterGetBlkDrvType", (RTS_UINTPTR)RouterGetBlkDrvType, 0, 0) 
#endif




/* -- Interface of the router provided for access by iec -- */

/**
 * <description>RTR_AddrComponent</description>
 */
typedef struct tagRTR_AddrComponent
{
	RTS_IEC_BYTE Component[2];
} RTR_AddrComponent;

/**
 * <description>RTR_NodeAddress</description>
 */
typedef struct tagRTR_NodeAddress
{
	RTS_IEC_UDINT nAddrComponentCount;
	RTR_AddrComponent AddrComponents[15];
} RTR_NodeAddress;

/**
 * <description>routergetinstancebyname</description>
 */
typedef struct tagroutergetinstancebyname_struct
{
	RTS_IEC_STRING *pstName;			/* VAR_INPUT */
	RTS_IEC_UDINT *pResult;				/* VAR_INPUT */
	RTS_IEC_BYTE *RouterGetInstanceByName;	/* VAR_OUTPUT */
} routergetinstancebyname_struct;

void CDECL CDECL_EXT routergetinstancebyname(routergetinstancebyname_struct *p);
typedef void (CDECL CDECL_EXT* PFROUTERGETINSTANCEBYNAME_IEC) (routergetinstancebyname_struct *p);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETINSTANCEBYNAME_NOTIMPLEMENTED)
	#define USE_routergetinstancebyname
	#define EXT_routergetinstancebyname
	#define GET_routergetinstancebyname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_routergetinstancebyname(p0) 
	#define CHK_routergetinstancebyname  FALSE
	#define EXP_routergetinstancebyname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_routergetinstancebyname
	#define EXT_routergetinstancebyname
	#define GET_routergetinstancebyname(fl)  CAL_CMGETAPI( "routergetinstancebyname" ) 
	#define CAL_routergetinstancebyname  routergetinstancebyname
	#define CHK_routergetinstancebyname  TRUE
	#define EXP_routergetinstancebyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetinstancebyname", (RTS_UINTPTR)routergetinstancebyname, 1, 0xBA8CBF9D, 0x03051100) 
	#define EXTREF_ITF_routergetinstancebyname {(RTS_VOID_FCTPTR)routergetinstancebyname, "routergetinstancebyname", 0xBA8CBF9D, 0x03051100}
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_routergetinstancebyname
	#define EXT_routergetinstancebyname
	#define GET_routergetinstancebyname(fl)  CAL_CMGETAPI( "routergetinstancebyname" ) 
	#define CAL_routergetinstancebyname  routergetinstancebyname
	#define CHK_routergetinstancebyname  TRUE
	#define EXP_routergetinstancebyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetinstancebyname", (RTS_UINTPTR)routergetinstancebyname, 1, 0xBA8CBF9D, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterroutergetinstancebyname
	#define EXT_CmpRouterroutergetinstancebyname
	#define GET_CmpRouterroutergetinstancebyname  ERR_OK
	#define CAL_CmpRouterroutergetinstancebyname  routergetinstancebyname
	#define CHK_CmpRouterroutergetinstancebyname  TRUE
	#define EXP_CmpRouterroutergetinstancebyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetinstancebyname", (RTS_UINTPTR)routergetinstancebyname, 1, 0xBA8CBF9D, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_routergetinstancebyname
	#define EXT_routergetinstancebyname
	#define GET_routergetinstancebyname(fl)  CAL_CMGETAPI( "routergetinstancebyname" ) 
	#define CAL_routergetinstancebyname  routergetinstancebyname
	#define CHK_routergetinstancebyname  TRUE
	#define EXP_routergetinstancebyname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetinstancebyname", (RTS_UINTPTR)routergetinstancebyname, 1, 0xBA8CBF9D, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_routergetinstancebyname  PFROUTERGETINSTANCEBYNAME_IEC pfroutergetinstancebyname;
	#define EXT_routergetinstancebyname  extern PFROUTERGETINSTANCEBYNAME_IEC pfroutergetinstancebyname;
	#define GET_routergetinstancebyname(fl)  s_pfCMGetAPI2( "routergetinstancebyname", (RTS_VOID_FCTPTR *)&pfroutergetinstancebyname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xBA8CBF9D, 0x03051100)
	#define CAL_routergetinstancebyname  pfroutergetinstancebyname
	#define CHK_routergetinstancebyname  (pfroutergetinstancebyname != NULL)
	#define EXP_routergetinstancebyname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetinstancebyname", (RTS_UINTPTR)routergetinstancebyname, 1, 0xBA8CBF9D, 0x03051100) 
#endif


/**
 * <description>routergetname</description>
 */
typedef struct tagroutergetname_struct
{
	RTS_IEC_BYTE *hRouter;				/* VAR_INPUT */
	RTS_IEC_BYTE *pBuffer;				/* VAR_INPUT */
	RTS_IEC_INT nBufferSize;			/* VAR_INPUT */
	RTS_IEC_UDINT RouterGetName;		/* VAR_OUTPUT */
} routergetname_struct;

void CDECL CDECL_EXT routergetname(routergetname_struct *p);
typedef void (CDECL CDECL_EXT* PFROUTERGETNAME_IEC) (routergetname_struct *p);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETNAME_NOTIMPLEMENTED)
	#define USE_routergetname
	#define EXT_routergetname
	#define GET_routergetname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_routergetname(p0) 
	#define CHK_routergetname  FALSE
	#define EXP_routergetname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_routergetname
	#define EXT_routergetname
	#define GET_routergetname(fl)  CAL_CMGETAPI( "routergetname" ) 
	#define CAL_routergetname  routergetname
	#define CHK_routergetname  TRUE
	#define EXP_routergetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetname", (RTS_UINTPTR)routergetname, 1, 0xE4FB4AAA, 0x03051100) 
	#define EXTREF_ITF_routergetname {(RTS_VOID_FCTPTR)routergetname, "routergetname", 0xE4FB4AAA, 0x03051100}
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_routergetname
	#define EXT_routergetname
	#define GET_routergetname(fl)  CAL_CMGETAPI( "routergetname" ) 
	#define CAL_routergetname  routergetname
	#define CHK_routergetname  TRUE
	#define EXP_routergetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetname", (RTS_UINTPTR)routergetname, 1, 0xE4FB4AAA, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterroutergetname
	#define EXT_CmpRouterroutergetname
	#define GET_CmpRouterroutergetname  ERR_OK
	#define CAL_CmpRouterroutergetname  routergetname
	#define CHK_CmpRouterroutergetname  TRUE
	#define EXP_CmpRouterroutergetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetname", (RTS_UINTPTR)routergetname, 1, 0xE4FB4AAA, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_routergetname
	#define EXT_routergetname
	#define GET_routergetname(fl)  CAL_CMGETAPI( "routergetname" ) 
	#define CAL_routergetname  routergetname
	#define CHK_routergetname  TRUE
	#define EXP_routergetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetname", (RTS_UINTPTR)routergetname, 1, 0xE4FB4AAA, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_routergetname  PFROUTERGETNAME_IEC pfroutergetname;
	#define EXT_routergetname  extern PFROUTERGETNAME_IEC pfroutergetname;
	#define GET_routergetname(fl)  s_pfCMGetAPI2( "routergetname", (RTS_VOID_FCTPTR *)&pfroutergetname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xE4FB4AAA, 0x03051100)
	#define CAL_routergetname  pfroutergetname
	#define CHK_routergetname  (pfroutergetname != NULL)
	#define EXP_routergetname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetname", (RTS_UINTPTR)routergetname, 1, 0xE4FB4AAA, 0x03051100) 
#endif


/**
 * <description>routergethostaddress</description>
 */
typedef struct tagroutergethostaddress_struct
{
	RTS_IEC_BYTE *hRouter;				/* VAR_INPUT */
	RTR_NodeAddress *resAddr;			/* VAR_IN_OUT */
	RTS_IEC_UDINT RouterGetHostAddress;	/* VAR_OUTPUT */
} routergethostaddress_struct;

void CDECL CDECL_EXT routergethostaddress(routergethostaddress_struct *p);
typedef void (CDECL CDECL_EXT* PFROUTERGETHOSTADDRESS_IEC) (routergethostaddress_struct *p);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETHOSTADDRESS_NOTIMPLEMENTED)
	#define USE_routergethostaddress
	#define EXT_routergethostaddress
	#define GET_routergethostaddress(fl)  ERR_NOTIMPLEMENTED
	#define CAL_routergethostaddress(p0) 
	#define CHK_routergethostaddress  FALSE
	#define EXP_routergethostaddress  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_routergethostaddress
	#define EXT_routergethostaddress
	#define GET_routergethostaddress(fl)  CAL_CMGETAPI( "routergethostaddress" ) 
	#define CAL_routergethostaddress  routergethostaddress
	#define CHK_routergethostaddress  TRUE
	#define EXP_routergethostaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergethostaddress", (RTS_UINTPTR)routergethostaddress, 1, 0xC47F4C13, 0x03051100) 
	#define EXTREF_ITF_routergethostaddress {(RTS_VOID_FCTPTR)routergethostaddress, "routergethostaddress", 0xC47F4C13, 0x03051100}
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_routergethostaddress
	#define EXT_routergethostaddress
	#define GET_routergethostaddress(fl)  CAL_CMGETAPI( "routergethostaddress" ) 
	#define CAL_routergethostaddress  routergethostaddress
	#define CHK_routergethostaddress  TRUE
	#define EXP_routergethostaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergethostaddress", (RTS_UINTPTR)routergethostaddress, 1, 0xC47F4C13, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterroutergethostaddress
	#define EXT_CmpRouterroutergethostaddress
	#define GET_CmpRouterroutergethostaddress  ERR_OK
	#define CAL_CmpRouterroutergethostaddress  routergethostaddress
	#define CHK_CmpRouterroutergethostaddress  TRUE
	#define EXP_CmpRouterroutergethostaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergethostaddress", (RTS_UINTPTR)routergethostaddress, 1, 0xC47F4C13, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_routergethostaddress
	#define EXT_routergethostaddress
	#define GET_routergethostaddress(fl)  CAL_CMGETAPI( "routergethostaddress" ) 
	#define CAL_routergethostaddress  routergethostaddress
	#define CHK_routergethostaddress  TRUE
	#define EXP_routergethostaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergethostaddress", (RTS_UINTPTR)routergethostaddress, 1, 0xC47F4C13, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_routergethostaddress  PFROUTERGETHOSTADDRESS_IEC pfroutergethostaddress;
	#define EXT_routergethostaddress  extern PFROUTERGETHOSTADDRESS_IEC pfroutergethostaddress;
	#define GET_routergethostaddress(fl)  s_pfCMGetAPI2( "routergethostaddress", (RTS_VOID_FCTPTR *)&pfroutergethostaddress, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xC47F4C13, 0x03051100)
	#define CAL_routergethostaddress  pfroutergethostaddress
	#define CHK_routergethostaddress  (pfroutergethostaddress != NULL)
	#define EXP_routergethostaddress   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergethostaddress", (RTS_UINTPTR)routergethostaddress, 1, 0xC47F4C13, 0x03051100) 
#endif


/**
 * <description>routergetparentaddress</description>
 */
typedef struct tagroutergetparentaddress_struct
{
	RTS_IEC_BYTE *hRouter;				/* VAR_INPUT */
	RTR_NodeAddress *resAddr;			/* VAR_IN_OUT */
	RTS_IEC_UDINT RouterGetParentAddress;	/* VAR_OUTPUT */
} routergetparentaddress_struct;

void CDECL CDECL_EXT routergetparentaddress(routergetparentaddress_struct *p);
typedef void (CDECL CDECL_EXT* PFROUTERGETPARENTADDRESS_IEC) (routergetparentaddress_struct *p);
#if defined(CMPROUTER_NOTIMPLEMENTED) || defined(ROUTERGETPARENTADDRESS_NOTIMPLEMENTED)
	#define USE_routergetparentaddress
	#define EXT_routergetparentaddress
	#define GET_routergetparentaddress(fl)  ERR_NOTIMPLEMENTED
	#define CAL_routergetparentaddress(p0) 
	#define CHK_routergetparentaddress  FALSE
	#define EXP_routergetparentaddress  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_routergetparentaddress
	#define EXT_routergetparentaddress
	#define GET_routergetparentaddress(fl)  CAL_CMGETAPI( "routergetparentaddress" ) 
	#define CAL_routergetparentaddress  routergetparentaddress
	#define CHK_routergetparentaddress  TRUE
	#define EXP_routergetparentaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetparentaddress", (RTS_UINTPTR)routergetparentaddress, 1, 0xD64907DE, 0x03051100) 
	#define EXTREF_ITF_routergetparentaddress {(RTS_VOID_FCTPTR)routergetparentaddress, "routergetparentaddress", 0xD64907DE, 0x03051100}
#elif defined(MIXED_LINK) && !defined(CMPROUTER_EXTERNAL)
	#define USE_routergetparentaddress
	#define EXT_routergetparentaddress
	#define GET_routergetparentaddress(fl)  CAL_CMGETAPI( "routergetparentaddress" ) 
	#define CAL_routergetparentaddress  routergetparentaddress
	#define CHK_routergetparentaddress  TRUE
	#define EXP_routergetparentaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetparentaddress", (RTS_UINTPTR)routergetparentaddress, 1, 0xD64907DE, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpRouterroutergetparentaddress
	#define EXT_CmpRouterroutergetparentaddress
	#define GET_CmpRouterroutergetparentaddress  ERR_OK
	#define CAL_CmpRouterroutergetparentaddress  routergetparentaddress
	#define CHK_CmpRouterroutergetparentaddress  TRUE
	#define EXP_CmpRouterroutergetparentaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetparentaddress", (RTS_UINTPTR)routergetparentaddress, 1, 0xD64907DE, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_routergetparentaddress
	#define EXT_routergetparentaddress
	#define GET_routergetparentaddress(fl)  CAL_CMGETAPI( "routergetparentaddress" ) 
	#define CAL_routergetparentaddress  routergetparentaddress
	#define CHK_routergetparentaddress  TRUE
	#define EXP_routergetparentaddress  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetparentaddress", (RTS_UINTPTR)routergetparentaddress, 1, 0xD64907DE, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_routergetparentaddress  PFROUTERGETPARENTADDRESS_IEC pfroutergetparentaddress;
	#define EXT_routergetparentaddress  extern PFROUTERGETPARENTADDRESS_IEC pfroutergetparentaddress;
	#define GET_routergetparentaddress(fl)  s_pfCMGetAPI2( "routergetparentaddress", (RTS_VOID_FCTPTR *)&pfroutergetparentaddress, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xD64907DE, 0x03051100)
	#define CAL_routergetparentaddress  pfroutergetparentaddress
	#define CHK_routergetparentaddress  (pfroutergetparentaddress != NULL)
	#define EXP_routergetparentaddress   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"routergetparentaddress", (RTS_UINTPTR)routergetparentaddress, 1, 0xD64907DE, 0x03051100) 
#endif


#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFROUTERREGISTERNETWORKINTERFACE2 IRouterRegisterNetworkInterface2;
 	PFROUTERREGISTERNETWORKINTERFACE IRouterRegisterNetworkInterface;
 	PFROUTERREGISTERONDEMANDNWINTERFACE IRouterRegisterOnDemandNWInterface;
 	PFROUTERUNREGISTERNETWORKINTERFACE IRouterUnregisterNetworkInterface;
 	PFROUTERHANDLEDATA IRouterHandleData;
 	PFROUTERGETBLKADDRESSES IRouterGetBlkAddresses;
 	PFROUTERCOMPAREADDRESSES IRouterCompareAddresses;
 	PFROUTERCALCULATENODEADDR IRouterCalculateNodeAddr;
 	PFROUTERGETINSTANCEBYNAME IRouterGetInstanceByName;
 	PFROUTERGETNAME IRouterGetName;
 	PFROUTERGETMAXMESSAGESIZE IRouterGetMaxMessageSize;
 	PFROUTERGETMAXMESSAGESIZEBYADDRESSLENGTH IRouterGetMaxMessageSizeByAddressLength;
 	PFROUTERGETMAXADDRESSSIZE IRouterGetMaxAddressSize;
 	PFROUTERSEND2 IRouterSend2;
 	PFROUTERSEND IRouterSend;
 	PFROUTERGETHOSTADDRESS IRouterGetHostAddress;
 	PFROUTERGETPARENTADDRESS IRouterGetParentAddress;
 	PFROUTERGETBLKDRVTYPE IRouterGetBlkDrvType;
 } ICmpRouter_C;

#ifdef CPLUSPLUS
class ICmpRouter : public IBase
{
	public:
		virtual RTS_RESULT CDECL IRouterRegisterNetworkInterface2(NETWORKINTERFACEINFO2 *pInterfaceInfo, RTS_HANDLE * phSubnet) =0;
		virtual RTS_RESULT CDECL IRouterRegisterNetworkInterface(NETWORKINTERFACEINFO *pInterfaceInfo, RTS_HANDLE * phSubnet) =0;
		virtual RTS_RESULT CDECL IRouterRegisterOnDemandNWInterface(NETWORKINTERFACEINFO *pInterfaceInfo, ONDEMANDNETWORKINTERFACE *pOnDemandInfo, RTS_HANDLE * phSubnet) =0;
		virtual RTS_RESULT CDECL IRouterUnregisterNetworkInterface(RTS_HANDLE hSubnet) =0;
		virtual RTS_RESULT CDECL IRouterHandleData(RTS_HANDLE phSubnet, NETWORKADDRESS sender, PROTOCOL_DATA_UNIT pduData, int bIsBroadcast) =0;
		virtual RTS_RESULT CDECL IRouterGetBlkAddresses(PROTOCOL_DATA_UNIT pduData, PEERADDRESS *pAddrReceiver, PEERADDRESS *pAddrSender, RTS_I32 *piDataOffset) =0;
		virtual RTS_BOOL CDECL IRouterCompareAddresses(PEERADDRESS *pAddr1, PEERADDRESS *pAddr2) =0;
		virtual RTS_RESULT CDECL IRouterCalculateNodeAddr(RTS_UI16 usBlkDrvType, RTS_UI8 byNetworkAddressBitSize, NETWORKADDRESS *pNetworkAddr, NODEADDRESS *pNodeAddr) =0;
		virtual RTS_RESULT CDECL IRouterGetInstanceByName(char *szName, RTS_HANDLE *phRouter) =0;
		virtual RTS_RESULT CDECL IRouterGetName(RTS_HANDLE hRouter, char *pszName, int nMaxLen) =0;
		virtual RTS_RESULT CDECL IRouterGetMaxMessageSize(RTS_HANDLE hRouter, PEERADDRESS addrPeer, RTS_UI16 *usMaxSize) =0;
		virtual RTS_RESULT CDECL IRouterGetMaxMessageSizeByAddressLength(RTS_HANDLE hRouter, RTS_UI16 usSumAddrLen, RTS_UI16 *usMaxSize) =0;
		virtual RTS_RESULT CDECL IRouterGetMaxAddressSize(RTS_HANDLE hRouter, PEERADDRESS addrPeer, RTS_UI8 *byMaxSize) =0;
		virtual RTS_RESULT CDECL IRouterSend2(RTS_HANDLE hRouter, PEERADDRESS addrReceiver, int nProtocolId, RTS_UI8 byMessageId, ROUTERPRIORITY prio, PROTOCOL_DATA_UNIT pduData, RTS_BOOL bUseQueue) =0;
		virtual RTS_RESULT CDECL IRouterSend(RTS_HANDLE hRouter, PEERADDRESS addrReceiver, int nProtocolId, RTS_UI8 byMessageId, ROUTERPRIORITY prio, PROTOCOL_DATA_UNIT pduData) =0;
		virtual RTS_RESULT CDECL IRouterGetHostAddress(RTS_HANDLE hRouter, NODEADDRESS *pAddrRouter) =0;
		virtual RTS_RESULT CDECL IRouterGetParentAddress(RTS_HANDLE hRouter, NODEADDRESS *pAddrParent) =0;
		virtual RTS_RESULT CDECL IRouterGetBlkDrvType(RTS_HANDLE hNetworkInterface, RTS_UI8 *pbyBlkDrvType) =0;
};
	#ifndef ITF_CmpRouter
		#define ITF_CmpRouter static ICmpRouter *pICmpRouter = NULL;
	#endif
	#define EXTITF_CmpRouter
#else	/*CPLUSPLUS*/
	typedef ICmpRouter_C		ICmpRouter;
	#ifndef ITF_CmpRouter
		#define ITF_CmpRouter
	#endif
	#define EXTITF_CmpRouter
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPROUTERITF_H_*/
