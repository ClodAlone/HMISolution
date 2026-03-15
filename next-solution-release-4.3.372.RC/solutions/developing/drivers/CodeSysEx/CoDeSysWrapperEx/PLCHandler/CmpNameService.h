#ifndef _CMPNAMESERVICE_H_
#define _CMPNAMESERVICE_H_

#include "CmpRouterItf.h"

/* Name service versions */
#define NSVERSION_MAJOR			0x04 
#define NSVERSION_MINOR			0x00
#define NSVERSION				((NSVERSION_MAJOR<<8)|NSVERSION_MINOR)
#define GET_NSVERSION_MAJOR(v)	((v>>8)&0xFF)
#define GET_NSVERSION_MINOR(v)	(v&0xFF)

#define NSVERSION_FIRST_IMPLEMENTATION			0x0101
#define NSVERSION_WITH_DEVICEID					0x0102 /* introduced with V3.1.0.0 */
#define NSVERSION_WITH_DEVICEID_SWAPPED			0x0201 
#define NSVERSION_WITH_ADDRDIFFERENCE			0x0103 /* introduced with V3.2.1.10 */
#define NSVERSION_WITH_ADDRDIFFERENCE_SWAPPED	0x0301 
#define NSVERSION_BANNED_MIN					0x0104
#define NSVERSION_BANNED_MAX					0x03FF
#define NSVERSION_WITH_FLAGS_AND_SN				0x0400 /* introduced with V3.5.10.0 */
/* ATTENTION: Because of swapping issues in implementations <V3.5.10.0, the following versions have to be considered as the same: 
NSVERSION_WITH_DEVICEID_SWAPPED: V2.1       --> NSVERSION_WITH_DEVICEID: V1.2
NSVERSION_WITH_ADDRDIFFERENCE_SWAPPED V3.1  --> NSVERSION_WITH_ADDRDIFFERENCE V1.3
And additionally V1.4..V1.255, V2.x and V3.x must not be used for futere extensions!
*/

/* Max. string and data lengths for the name service paket */
#define NS_MAX_NODE_NAME_LEN					50	/* wide-chars strings including NULL */
#define NS_MAX_DEVICE_AND_VENDOR_NAME_LEN		100	/* 2 wide-chars including 2 x NULL */
#define NS_MAX_SERIAL_NUMBER_AND_OEM_DATA_LEN	66  /* Bytes */  

#define NS_EXPECTED_PACKET_LEN					444 /* 512 Bytes block size - 68 Byte router header including 2*15 address componentes */

/* Name service package types*/
#define NSPT_RESOLVENAME     0xC201 /* let only answer nodes with a matching node name */
#define NSPT_RESOLVEADDR     0xC202	/* to resolve all nodes in the network send a broadcast with a RESOLVEADDR request */
#define NSPT_RESOLVEGATEWAY	 0xC203 /* reserved - currently not used */
#define NSPT_IDENTIFICATION  0xC280	/* used in answer package and for self identification requests */

/* -- Packages used by service to service communication -- */

#ifndef NO_PRAGMA_PACK
	#pragma pack(1)
#endif

typedef struct
{
	RTS_UI16 wPkgType; 
	RTS_UI16 wVersion;
	RTS_UI32 dwRequestID;
}NAMESERVICEPKG;

typedef struct
{
	NAMESERVICEPKG header;
	RTS_WCHAR wszName[1];
	  /* Name of the searched for node (zero terminated) */
}NSRESOLVENAMEPKG;

typedef struct
{
	NAMESERVICEPKG header;
}NSRESOLVEADDRPKG;

/* This struct describes a node in the sense of the naming service for version
   NSVERSION_FIRST_IMPLEMENTATION (1.1).
   It contains three dynamic length fields, only one of these can be
   listed in this struct definition. 
   The dynamic fields are (in this order):
   - addrParent (wParentAddrSize components) followed by
   - node name (zero terminated wide-char string) followed by
   - type description in NODEINFOPACKED2 aka device name (zero terminated wide-char string)
   If wParentAddrSize is zero (i.e. node has no parent) then the node name
   starts immediatly at position offsetof(addrParent).
*/
typedef struct 
{
	RTS_UI32		 ulType;			/* in NODEINFOPACKED2 aka ulTargetId */
	unsigned short   wMaxChannels;
#ifdef RTS_SIXTEENBITBYTES
	unsigned short	 bIntelByteOrder;
#else
	unsigned char    bIntelByteOrder;	/* Byte order: 0="Motorola" else "Intel" */
	unsigned char    byDummy;
#endif
	unsigned short   wParentAddrSize;	/* Number of components in addrParent */

	unsigned short   wNameLength;		/* in NODEINFOPACKED2 aka wNodeNameLength. Length of the name string w/o trailing "0" */
	unsigned short   wTypeLength;		/* in NODEINFOPACKED2 aka wDeviceNameLength. Length of the type string w/o trailing "0" */

	ADDRESSCOMPONENT addrParent[1];
} NODEINFOPACKED;

/* This struct describes a node in the sense of the naming service for versions
   NSVERSION_WITH_DEVICEID (1.2) and NSVERSION_WITH_ADDRDIFFERENCE (1.3).
   It contains four dynamic length fields, only one of these can be
   listed in this struct definition. 
   The dynamic fields are (in this order):
   - addrParent (wParentAddrSize components) followed by
   - node name (zero terminated wide-char string) followed by
   - device name (zero terminated wide-char string) followed by
   - vendor name (zero terminated wide-char string)
   If wParentAddrSize is zero (i.e. node has no parent) then the node name
   starts immediatly at position offsetof(addrParent).
*/
typedef struct 
{
	unsigned short wMaxChannels;
#ifdef RTS_SIXTEENBITBYTES
	unsigned short bIntelByteOrderAndAddrDiff;	/* Byte order: 0="Motorola" else "Intel" */
#else
	unsigned char  bIntelByteOrder;				/* Byte order: 0="Motorola" else "Intel" */
	unsigned char  byAddrDifference;			/* Difference of number of addr components between parent and child*/
#endif
	unsigned short wParentAddrSize;				/* Number of components in addrParent */

	unsigned short wNodeNameLength;				/* Length of the name string w/o trailing "0" */
	unsigned short wDeviceNameLength;			/* Length of the type string w/o trailing "0" */
	unsigned short wVendorNameLength;			/* Length of the type string w/o trailing "0" */

	RTS_UI32 ulTargetType;
	RTS_UI32 ulTargetId;
	RTS_UI32 ulTargetVersion;

	ADDRESSCOMPONENT addrParent[1];
} NODEINFOPACKED2;

/* This struct describes a node in the sense of the naming service for version
   NSVERSION_WITH_FLAGS_AND_SN (4.0).
   It contains six dynamic length fields, only one of these can be 
   listed in this struct definition.
   The dynamic fields are (in this order):
   - addrParent (wParentAddrSize components) followed by
   - node name (zero terminated wide-char string) followed by
   - device name (zero terminated wide-char string) followed by
   - vendor name (zero terminated wide-char string) followed by
   - serial number (zero terminated char string) followed by
   - OEM specific data (byte array)
   If wParentAddrSize is zero (i.e. node has no parent) then the node name
   starts immediatly at position offsetof(addrParent).
   This struct is limited to 512 Bytes including nameservice and router package header:
    68 Byte Router header (including 60 Bytes for sender- and receiveraddr. with 15 address components each)
     8 Byte Name service package header (NAMESERVICEPKG)
     6 Byte MaxChannels, ByteOrder, AddrDifference and ParentAddrSize
     6 Byte NameLengths
    12 Byte TargetType/Id/Version
    16 Byte Flags, Serialnumber/OemDataLength, BlkDrvType and Reserved 
    30 Byte ParentAddress (max. 15 address components)
   100 Byte Node name (max. 50 wide-chars including NULL)
   200 Byte Device name and vendor name (max. 100 wide-chars including 2x NULL)
    66 Byte Serial number and OEM specific data (e.g. 64 char serial number + NULL + 1 Byte OEM data)
   --------
   512 Byte
   ========
*/
typedef struct
{
	RTS_UI16 wMaxChannels;					/* Number of communciation channels */
#ifdef RTS_SIXTEENBITBYTES
	RTS_UI16 bIntelByteOrderAndAddrDiff;	/* see bIntelByteOrder and byAddrDifference below */
#else
	RTS_UI8  bIntelByteOrder;				/* Byte order: 0="Motorola" else "Intel" */
	RTS_UI8  byAddrDifference;				/* Difference of number of addr components between parent and child */
#endif
	RTS_UI16 wParentAddrSize;				/* Number of components in addrParent [0..15] */

	RTS_UI16 wNodeNameLength;				/* Length of node name w/o NULL [0..49] */
	RTS_UI16 wDeviceNameLength;				/* Length of device name w/o NULL [0..98]; wDeviceNameLength + wVendorNameLength <= 98 */
	RTS_UI16 wVendorNameLength;				/* Length of vendor name w/o NULL [0..98]; wDeviceNameLength + wVendorNameLength <= 98 */

	RTS_UI32 ulTargetType;					/* target type, e. g. SYSTARGET_TYPE_PROGRAMMABLE */
	RTS_UI32 ulTargetId;					/* target id, consists of SYSTARGET_VENDOR_ID and SYSTARGET_DEVICE_ID */
	RTS_UI32 ulTargetVersion;				/* (OEM) version */

	RTS_UI32 ulFlags;						/* See name service package flags NSPF_ - unused flags have to be set to 0 */
#ifdef RTS_SIXTEENBITBYTES
	RTS_UI16 wSerialNumberAndOemLen;		/* see bySerialNumberLength and byOemDataLength below */
	RTS_UI16 wBlkDrvTypeAndByte;			/* see byBlkDrvType and byByte below */
#else
	RTS_UI8	 bySerialNumberLength;			/* Length of serial number w/o NULL [0..64]; bySerialNumberLength + byOemDataLength <= 65 */
	RTS_UI8	 byOemDataLength;				/* Length of OEM specific data [0..65]; bySerialNumberLength + byOemDataLength <= 65 */
	RTS_UI8  byBlkDrvType;					/* type of block driver instance on which request was received, e.g. RTS_BLK_DRV_TYPE_UDP  */
	RTS_UI8  byByte;						/* Reserved, must be set to 0 */
#endif
	RTS_UI32 ulDWord1;						/* Reserved, must be set to 0 */
	RTS_UI32 ulDWord2;						/* Reserved, must be set to 0 */

	ADDRESSCOMPONENT addrParent[1];			/* addrParent, node name, device name, vendor name, serial number and OEM specific data */
} NODEINFOPACKED3;


/* This struct is the answer to any of the previous requests
   to the name service.
*/
typedef struct
{
	NAMESERVICEPKG header;
	NODEINFOPACKED ni;
}NSIDENTIFICATIONPKG;

typedef struct
{
	NAMESERVICEPKG header;
	NODEINFOPACKED2 ni;
}NSIDENTIFICATIONPKG2;

typedef struct
{
	NAMESERVICEPKG header;
	NODEINFOPACKED3 ni;
}NSIDENTIFICATIONPKG3;


#ifndef NO_PRAGMA_PACK
	#ifdef USE_PRAGMA_PACK_0
		#pragma pack(0)
	#else
		#pragma pack()
	#endif	
#endif

/* Check wether nSize is the minimum size of a nameserver package */
#define CHECKNSPACKAGESIZE(nSize) ((nSize) >= sizeof(NAMESERVICEPKG))

/* Get the package type of a nameservice package. 
   The data parameter must be an (unsigned char *).
   Use CHECKNSPACKAGESIZE before using this macro to ensure
   the package contains enough data.
*/
#define NSPACKAGETYPE(data) SWAP16u(((NAMESERVICEPKG *)(data))->wPkgType)

#ifdef RTS_SIXTEENBITBYTES

	#define sizeof_NAMESERVICEPKG	sizeof(NAMESERVICEPKG) * 2
	#define sizeof_NODEINFOPACKED	sizeof(NODEINFOPACKED) * 2
	#define sizeof_NODEINFOPACKED2	sizeof(NODEINFOPACKED2) * 2
	#define sizeof_NODEINFOPACKED3	sizeof(NODEINFOPACKED3) * 2

	/* GET/SET elements of NODEINFOPACKED */
	#define GET_NODEINFO_INTELBO(nsidentpkg)					(unsigned char)(((NODEINFOPACKED*)nsidentpkg)->bIntelByteOrder & 0x00FF)
	#define SET_NODEINFO_INTELBO(nsidentpkg,bIntelBO)			((NODEINFOPACKED*)nsidentpkg)->bIntelByteOrder = (unsigned short)((((NODEINFOPACKED*)nsidentpkg)->bIntelByteOrder & 0xFF00) | (bIntelBO & 0x00FF))

	/* GET/SET elements of NODEINFOPACKED2 */
	#define GET_NODEINFO2_INTELBO(nsidentpkg)					(unsigned short)(((NODEINFOPACKED2*)nsidentpkg)->bIntelByteOrderAndAddrDiff & 0x00FF)
	#define GET_NODEINFO2_ADDRDIFF(nsidentpkg)					(unsigned short)((((NODEINFOPACKED2*)nsidentpkg)->bIntelByteOrderAndAddrDiff >> 8) & 0x00FF)			   
	#define SET_NODEINFO2_INTELBO(nsidentpkg,bIntelBO)			((NODEINFOPACKED2*)nsidentpkg)->bIntelByteOrderAndAddrDiff = (unsigned short)((((NODEINFOPACKED2*)nsidentpkg)->bIntelByteOrderAndAddrDiff & 0xFF00) | (bIntelBO & 0x00FF))														
	#define SET_NODEINFO2_ADDRDIFF(nsidentpkg,byAddrDifference) ((NODEINFOPACKED2*)nsidentpkg)->bIntelByteOrderAndAddrDiff = (unsigned short)((((NODEINFOPACKED2*)nsidentpkg)->bIntelByteOrderAndAddrDiff & 0x00FF) | ((byAddrDifference << 8) & 0xFF00))															
	#define SET_NODEINFO2_ADDR_PARENT(nsidentpkg,nPos,nAddrParent) *(unsigned short*)(((NODEINFOPACKED2*)nsidentpkg)->addrParent) = (unsigned short)(((nAddrParent)[0] & 0x00FF) | (((nAddrParent)[1] << 8) & 0xFF00))

	/* GET/SET elements of NODEINFOPACKED3 */
	#define GET_NODEINFO3_INTELBO(nsidentpkg)					(unsigned short)(((NODEINFOPACKED3*)nsidentpkg)->bIntelByteOrderAndAddrDiff & 0x00FF)
	#define GET_NODEINFO3_ADDRDIFF(nsidentpkg)					(unsigned short)((((NODEINFOPACKED3*)nsidentpkg)->bIntelByteOrderAndAddrDiff >> 8) & 0x00FF)			   
	#define SET_NODEINFO3_INTELBO(nsidentpkg,bIntelBO)			((NODEINFOPACKED3*)nsidentpkg)->bIntelByteOrderAndAddrDiff = (unsigned short)((((NODEINFOPACKED3*)nsidentpkg)->bIntelByteOrderAndAddrDiff & 0xFF00) | (bIntelBO & 0x00FF))														
	#define SET_NODEINFO3_ADDRDIFF(nsidentpkg,byAddrDifference) ((NODEINFOPACKED3*)nsidentpkg)->bIntelByteOrderAndAddrDiff = (unsigned short)((((NODEINFOPACKED3*)nsidentpkg)->bIntelByteOrderAndAddrDiff & 0x00FF) | ((byAddrDifference << 8) & 0xFF00))															
	#define GET_NODEINFO3_SERIALLEN(nsidentpkg)					(unsigned short)(((NODEINFOPACKED3*)nsidentpkg)->wSerialNumberAndOemLen & 0x00FF)
	#define GET_NODEINFO3_OEMDATALEN(nsidentpkg)				(unsigned short)((((NODEINFOPACKED3*)nsidentpkg)->wSerialNumberAndOemLen >> 8) & 0x00FF)			   
	#define SET_NODEINFO3_SERIALLEN(nsidentpkg,bySnLen)			((NODEINFOPACKED3*)nsidentpkg)->wSerialNumberAndOemLen = (unsigned short)((((NODEINFOPACKED3*)nsidentpkg)->wSerialNumberAndOemLen & 0xFF00) | (bySnLen & 0x00FF))				
	#define SET_NODEINFO3_OEMDATALEN(nsidentpkg,byOemLen)		((NODEINFOPACKED3*)nsidentpkg)->wSerialNumberAndOemLen = (unsigned short)((((NODEINFOPACKED3*)nsidentpkg)->wSerialNumberAndOemLen & 0x00FF) | ((byOemLen << 8) & 0xFF00))
	#define GET_NODEINFO3_BLKDRVTYPE(nsidentpkg)				(unsigned short)(((NODEINFOPACKED3*)nsidentpkg)->wBlkDrvTypeAndByte & 0x00FF)
	#define GET_NODEINFO3_RESBYTE(nsidentpkg)					(unsigned short)((((NODEINFOPACKED3*)nsidentpkg)->wBlkDrvTypeAndByte >> 8) & 0x00FF)			   
	#define SET_NODEINFO3_BLKDRVTYPE(nsidentpkg,byBlkDrv)		((NODEINFOPACKED3*)nsidentpkg)->wBlkDrvTypeAndByte = (unsigned short)((((NODEINFOPACKED3*)nsidentpkg)->wBlkDrvTypeAndByte & 0xFF00) | (byBlkDrv & 0x00FF))					
	#define SET_NODEINFO3_RESBYTE(nsidentpkg,byResByte)			((NODEINFOPACKED3*)nsidentpkg)->wBlkDrvTypeAndByte = (unsigned short)((((NODEINFOPACKED3*)nsidentpkg)->wBlkDrvTypeAndByte & 0x00FF) | ((byResByte << 8) & 0xFF00))	
	#define SET_NODEINFO3_ADDR_PARENT(nsidentpkg,nPos,nAddrParent) *(unsigned short*)(((NODEINFOPACKED3*)nsidentpkg)->addrParent) = (unsigned short)(((nAddrParent)[0] & 0x00FF) | (((nAddrParent)[1] << 8) & 0xFF00))

#else

	/* GET/SET elements of NODEINFOPACKED */
	#define GET_NODEINFO_INTELBO(nsidentpkg)					(unsigned char)((NODEINFOPACKED*)nsidentpkg)->bIntelByteOrder
	#define SET_NODEINFO_INTELBO(nsidentpkg,bIntelBO)			((NODEINFOPACKED*)nsidentpkg)->bIntelByteOrder = (unsigned char) bIntelBO

	/* GET/SET elements of NODEINFOPACKED2 */
	#define GET_NODEINFO2_INTELBO(nsidentpkg)					(unsigned char)((NODEINFOPACKED2*)nsidentpkg)->bIntelByteOrder
	#define GET_NODEINFO2_ADDRDIFF(nsidentpkg)					(unsigned char) ((NODEINFOPACKED2*)nsidentpkg)->byAddrDifference
	#define SET_NODEINFO2_INTELBO(nsidentpkg,bIntelBO)			((NODEINFOPACKED2*)nsidentpkg)->bIntelByteOrder = (unsigned char) bIntelBO
	#define SET_NODEINFO2_ADDRDIFF(nsidentpkg,byAddrDiff)		((NODEINFOPACKED2*)nsidentpkg)->byAddrDifference = (unsigned char) byAddrDiff

	/* GET/SET elements of NODEINFOPACKED3 */
	#define GET_NODEINFO3_INTELBO(nsidentpkg)					(unsigned char)((NODEINFOPACKED3*)nsidentpkg)->bIntelByteOrder
	#define GET_NODEINFO3_ADDRDIFF(nsidentpkg)					(unsigned char) ((NODEINFOPACKED3*)nsidentpkg)->byAddrDifference
	#define SET_NODEINFO3_INTELBO(nsidentpkg,bIntelBO)			((NODEINFOPACKED3*)nsidentpkg)->bIntelByteOrder = (unsigned char) bIntelBO
	#define SET_NODEINFO3_ADDRDIFF(nsidentpkg,byAddrDiff)		((NODEINFOPACKED3*)nsidentpkg)->byAddrDifference = (unsigned char) byAddrDiff
	#define GET_NODEINFO3_SERIALLEN(nsidentpkg)					(unsigned char)((NODEINFOPACKED3*)nsidentpkg)->bySerialNumberLength
	#define GET_NODEINFO3_OEMDATALEN(nsidentpkg)				(unsigned char) ((NODEINFOPACKED3*)nsidentpkg)->byOemDataLength
	#define SET_NODEINFO3_SERIALLEN(nsidentpkg,bySnLen)			((NODEINFOPACKED3*)nsidentpkg)->bySerialNumberLength = (unsigned char) bySnLen
	#define SET_NODEINFO3_OEMDATALEN(nsidentpkg,byOemLen)		((NODEINFOPACKED3*)nsidentpkg)->byOemDataLength = (unsigned char) byOemLen
	#define GET_NODEINFO3_BLKDRVTYPE(nsidentpkg)				(unsigned char)((NODEINFOPACKED3*)nsidentpkg)->byBlkDrvType
	#define GET_NODEINFO3_RESBYTE(nsidentpkg)					(unsigned char) ((NODEINFOPACKED3*)nsidentpkg)->byByte
	#define SET_NODEINFO3_BLKDRVTYPE(nsidentpkg,byBlkDrv)		((NODEINFOPACKED3*)nsidentpkg)->byBlkDrvType = (unsigned char) byBlkDrv
	#define SET_NODEINFO3_RESBYTE(nsidentpkg,byResByte)			((NODEINFOPACKED3*)nsidentpkg)->byByte = (unsigned char) byResByte

#endif /* RTS_SIXTEENBITBYTES */

#endif /* _CMPNAMESERVICE_H_ */
