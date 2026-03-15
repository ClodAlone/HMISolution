/*****************************************************************************
*
*	Copyright:		(c) 3S - Smart Software Solutions GmbH, Kempten
*	Module: 		PLCHandlerDefines.h
*	Purpose:		Contains all global PLCHandler definitions.
*
******************************************************************************/

#ifndef __PLCHANDLERDEFINES_H__
#define __PLCHANDLERDEFINES_H__


/* Error codes */
#define RESULT_FAILED								-1
#define RESULT_OK									0
#define RESULT_PLC_NOT_CONNECTED					1
#define RESULT_PLC_LOGIN_FAILED						2
#define RESULT_PLC_NO_CYCLIC_LIST_DEFINED			3
#define RESULT_PLCHANDLER_INACTIVE					4
#define RESULT_LOADING_SYMBOLS_FAILED				5
#define RESULT_ITF_NOT_SUPPORTED					6
#define RESULT_COMM_FATAL							7
#define RESULT_NO_CONFIGURATION						8
#define RESULT_INVALID_PARAMETER					9
#define RESULT_ITF_FAILED							10
#define RESULT_NOT_SUPPORTED						11
#define RESULT_EXCEPTION							12
#define RESULT_TIMEOUT								13
#define RESULT_STILL_CONNECTED						14
#define RESULT_RECONNECTTHREAD_STILL_ACTIVE			15
#define RESULT_PLC_NOT_CONNECTED_SYMBOLS_LOADED		16
#define RESULT_NO_UPDATE							17
#define RESULT_OCX_CONVERSION_FAILED				18
#define RESULT_TARGETID_MISMATCH					19
#define RESULT_NO_OBJECT							20
#define RESULT_COMPONENTS_NOT_LOADED				21
#define RESULT_BUSY									22
#define RESULT_DISABLED								23
#define RESULT_PLC_FAILED							24
#define RESULT_INVALID_SYMBOL						25
#define RESULT_BUFFER_TOO_SMALL						26
#define RESULT_NO_PROJECT							27
#define RESULT_FILE_ERROR							28
#define RESULT_RETAIN_MISMATCH						29
#define RESULT_NO_ACCESS_RIGHTS						30
#define RESULT_DUPLICATE_PLC_NAME					31
#define RESULT_SIZE_MISMATCH						32
#define RESULT_LIST_NO_WRITE_ACCESS					33
#define RESULT_CONSISTENT_ACCESS_TIMEOUT			34
#define RESULT_SYNC_CONSISTENT_ACCESS_DENIED		35
#define RESULT_INVALID_ASCII_STRING					36
#define RESULT_INVALID_STRING_LENGTH				37
#define RESULT_OUTOFMEMORY							38
#define RESULT_NO_FILE								39
#define RESULT_APPLICATION_NOT_IN_STOP				40
#define RESULT_APPLICATION_NOT_IN_RUN				41
#define RESULT_OPERATIONMODE_NOT_IN_DEBUG			42
#define RESULT_BACKUP_RESTORE_NOT_SUPPORTED			43
#define RESULT_PLC_INCONSISTENT_STATE				44
#define RESULT_PLC_INCOMPATIBLE						45
#define RESULT_PLC_VERSION_INCOMPATIBLE				46
#define RESULT_RETAIN_ERROR							47
#define RESULT_APPLICATIONS_LOAD_ERROR				48
#define RESULT_APPLICATIONS_START_ERROR				49
#define RESULT_FILETRANSFER_ERROR					50
#define RESULT_OPERATION_DENIED						51
#define RESULT_FORCES_ACTIVE						52
#define RESULT_META_VERSION_MISMATCH				53

/* Certificate validation error codes */
#define RESULT_CERT_OK									RESULT_OK

/* illegal error (for uninitialized values, to avoid ERR_CERT_OK) */
#define RESULT_CERT_UNABLE_TO_GET_ISSUER_CERT			ERR_CERT_UNABLE_TO_GET_ISSUER_CERT
#define RESULT_CERT_UNABLE_TO_GET_CRL					ERR_CERT_UNABLE_TO_GET_CRL
#define RESULT_CERT_UNABLE_TO_DECRYPT_CERT_SIGNATURE	ERR_CERT_UNABLE_TO_DECRYPT_CERT_SIGNATURE
#define RESULT_CERT_UNABLE_TO_DECRYPT_CRL_SIGNATURE		ERR_CERT_UNABLE_TO_DECRYPT_CRL_SIGNATURE
#define RESULT_CERT_UNABLE_TO_DECODE_ISSUER_PUBLIC_KEY	ERR_CERT_UNABLE_TO_DECODE_ISSUER_PUBLIC_KEY
#define RESULT_CERT_SIGNATURE_FAILURE					ERR_CERT_SIGNATURE_FAILURE
#define RESULT_CERT_CRL_SIGNATURE_FAILURE				ERR_CERT_CRL_SIGNATURE_FAILURE
#define RESULT_CERT_NOT_YET_VALID						ERR_CERT_NOT_YET_VALID
#define RESULT_CERT_HAS_EXPIRED							ERR_CERT_HAS_EXPIRED
#define RESULT_CERT_CRL_NOT_YET_VALID					ERR_CERT_CRL_NOT_YET_VALID
#define RESULT_CERT_CRL_HAS_EXPIRED						ERR_CERT_CRL_HAS_EXPIRED
#define RESULT_CERT_ERROR_IN_CERT_NOT_BEFORE_FIELD		ERR_CERT_ERROR_IN_CERT_NOT_BEFORE_FIELD
#define RESULT_CERT_ERROR_IN_CERT_NOT_AFTER_FIELD		ERR_CERT_ERROR_IN_CERT_NOT_AFTER_FIELD
#define RESULT_CERT_ERROR_IN_CRL_LAST_UPDATE_FIELD		ERR_CERT_ERROR_IN_CRL_LAST_UPDATE_FIELD
#define RESULT_CERT_ERROR_IN_CRL_NEXT_UPDATE_FIELD		ERR_CERT_ERROR_IN_CRL_NEXT_UPDATE_FIELD
#define RESULT_CERT_OUT_OF_MEM							ERR_CERT_OUT_OF_MEM
#define RESULT_CERT_DEPTH_ZERO_SELF_SIGNED_CERT			ERR_CERT_DEPTH_ZERO_SELF_SIGNED_CERT
#define RESULT_CERT_SELF_SIGNED_CERT_IN_CHAIN			ERR_CERT_SELF_SIGNED_CERT_IN_CHAIN
#define RESULT_CERT_UNABLE_TO_GET_ISSUER_CERT_LOCALLY	ERR_CERT_UNABLE_TO_GET_ISSUER_CERT_LOCALLY
#define RESULT_CERT_UNABLE_TO_VERIFY_LEAF_SIGNATURE		ERR_CERT_UNABLE_TO_VERIFY_LEAF_SIGNATURE
#define RESULT_CERT_CHAIN_TOO_LONG						ERR_CERT_CHAIN_TOO_LONG
#define RESULT_CERT_REVOKED								ERR_CERT_REVOKED
#define RESULT_CERT_INVALID_CA							ERR_CERT_INVALID_CA
#define RESULT_CERT_PATH_LENGTH_EXCEEDED				ERR_CERT_PATH_LENGTH_EXCEEDED
#define RESULT_CERT_INVALID_PURPOSE						ERR_CERT_INVALID_PURPOSE
#define RESULT_CERT_UNTRUSTED							ERR_CERT_UNTRUSTED
#define RESULT_CERT_REJECTED							ERR_CERT_REJECTED

/* These are 'informational' when looking for issuer cert */
#define RESULT_CERT_SUBJECT_ISSUER_MISMATCH				ERR_CERT_SUBJECT_ISSUER_MISMATCH
#define RESULT_CERT_AKID_SKID_MISMATCH					ERR_CERT_AKID_SKID_MISMATCH
#define RESULT_CERT_AKID_ISSUER_SERIAL_MISMATCH			ERR_CERT_AKID_ISSUER_SERIAL_MISMATCH
#define RESULT_CERT_KEYUSAGE_NO_CERTSIGN				ERR_CERT_KEYUSAGE_NO_CERTSIGN
#define RESULT_CERT_UNABLE_TO_GET_CRL_ISSUER			ERR_CERT_UNABLE_TO_GET_CRL_ISSUER
#define RESULT_CERT_UNHANDLED_CRITICAL_EXTENSION		ERR_CERT_UNHANDLED_CRITICAL_EXTENSION
#define RESULT_CERT_KEYUSAGE_NO_CRL_SIGN				ERR_CERT_KEYUSAGE_NO_CRL_SIGN
#define RESULT_CERT_UNHANDLED_CRITICAL_CRL_EXTENSION	ERR_CERT_UNHANDLED_CRITICAL_CRL_EXTENSION
#define RESULT_CERT_INVALID_NON_CA						ERR_CERT_INVALID_NON_CA
#define RESULT_CERT_PROXY_PATH_LENGTH_EXCEEDED			ERR_CERT_PROXY_PATH_LENGTH_EXCEEDED
#define RESULT_CERT_KEYUSAGE_NO_DIGITAL_SIGNATURE		ERR_CERT_KEYUSAGE_NO_DIGITAL_SIGNATURE
#define RESULT_CERT_PROXY_CERTIFICATES_NOT_ALLOWED		ERR_CERT_PROXY_CERTIFICATES_NOT_ALLOWED
#define RESULT_CERT_INVALID_EXTENSION					ERR_CERT_INVALID_EXTENSION
#define RESULT_CERT_INVALID_POLICY_EXTENSION			ERR_CERT_INVALID_POLICY_EXTENSION
#define RESULT_CERT_NO_EXPLICIT_POLICY					ERR_CERT_NO_EXPLICIT_POLICY
#define RESULT_CERT_DIFFERENT_CRL_SCOPE					ERR_CERT_DIFFERENT_CRL_SCOPE
#define RESULT_CERT_UNSUPPORTED_EXTENSION_FEATURE		ERR_CERT_UNSUPPORTED_EXTENSION_FEATURE
#define RESULT_CERT_UNNESTED_RESOURCE					ERR_CERT_UNNESTED_RESOURCE
#define RESULT_CERT_PERMITTED_VIOLATION					ERR_CERT_PERMITTED_VIOLATION
#define RESULT_CERT_EXCLUDED_VIOLATION					ERR_CERT_EXCLUDED_VIOLATION
#define RESULT_CERT_SUBTREE_MINMAX						ERR_CERT_SUBTREE_MINMAX
#define RESULT_CERT_UNSUPPORTED_CONSTRAINT_TYPE			ERR_CERT_UNSUPPORTED_CONSTRAINT_TYPE
#define RESULT_CERT_UNSUPPORTED_CONSTRAINT_SYNTAX		ERR_CERT_UNSUPPORTED_CONSTRAINT_SYNTAX
#define RESULT_CERT_UNSUPPORTED_NAME_SYNTAX				ERR_CERT_UNSUPPORTED_NAME_SYNTAX
#define RESULT_CERT_CRL_PATH_VALIDATION_ERROR			ERR_CERT_CRL_PATH_VALIDATION_ERROR

/* The application is not happy */
#define RESULT_CERT_APPLICATION_VERIFICATION			ERR_CERT_APPLICATION_VERIFICATION
#define RESULT_CERT_MISSING								ERR_CERT_MISSING
#define RESULT_CERT_UNKNOWN_ERROR						ERR_CERT_UNKNOWN_ERROR

/* PLCHandler states */
typedef enum
{
	STATE_TERMINATE = -1,
	STATE_PLC_NOT_CONNECTED = 0,
	STATE_PLC_CONNECTED,
	STATE_NO_SYMBOLS,
	STATE_SYMBOLS_LOADED,
	STATE_RUNNING,
	STATE_DISCONNECT,
	STATE_NO_CONFIGURATION,
	STATE_PLC_NOT_CONNECTED_SYMBOLS_LOADED
} PLCHANDLER_STATE;

/* obsolete, use NodeInfotyp2 instead */
typedef struct NodeInfoTag
{
	long lResult;						/* all other elements of the structure are only valid if lResult=RESULT_OK */
	int	bComplete;						/* if set, this is the last call of this callback */
	char *pszNodeAddr;
	char *pszParentAddr;				/* address of the parent, if there is one */
	RTS_WCHAR *pwszNodeName;
	RTS_WCHAR *pwszDeviceName;
	RTS_WCHAR *pwszVendorName;
	unsigned long ulTargetType;
	unsigned long ulTargetId;
	unsigned long ulTargetVersion;
	unsigned short wMaxChannels;		/* Number of channels supported by the PLC */
}NodeInfotyp;

typedef struct NodeInfoTag2
{
	long lResult;					/* All other elements of the structure are only valid if lResult=RESULT_OK */
	int	bComplete;					/* If set, this is the last call of this callback */
	char *pszNodeAddr;				/* Address of answering node */
	char *pszParentAddr;			/* Address of the parent, if there is one */
	RTS_WCHAR *pwszNodeName;		/* Node name */
	RTS_WCHAR *pwszDeviceName;		/* Device name */
	RTS_WCHAR *pwszVendorName;		/* Vendor name, may be an empty string */
	RTS_UI32 ulTargetType;			/* Target type, e. g. SYSTARGET_TYPE_PROGRAMMABLE */
	RTS_UI32 ulTargetId;			/* Target id, consists of SYSTARGET_VENDOR_ID and SYSTARGET_DEVICE_ID */
	RTS_UI32 ulTargetVersion;		/* (OEM) version */
	RTS_UI16 wMaxChannels;			/* Number of communication channels supported by the PLC */
	RTS_UI16 wReserved;				/* Reserved, currently set to 0 */
	RTS_CSTRING	*pszSerialNumber;	/* Serial number, may be an empty string */
	RTS_UI8	*pbyOemData;			/* OEM data, may be NULL */
	RTS_UI32 ulOemDataLength;		/* Length of OEM specific data */
	RTS_UI32 ulFlags;				/* See name service package flags NSPF_ - unused flags have to be set to 0 */
	RTS_UI8	byIntelByteOrder;		/* Byte order: 0="Motorola", 1="Intel", 255=Unknown, see defines GWCLIENT_NI_BYTEORDER_xxx */
	RTS_UI8	byReserved;				/* Reserved, currently set to 0 */
	RTS_UI16 wRtsBlkDrvType;		/* Type of block driver instance on runtime on which this node info was sent, e.g. RTS_BLK_DRV_TYPE_UDP */
	RTS_UI16 wGwBlkDrvType;			/* Type of block driver instance on Gateway/PLCHandler(ARTI3) on which this node info was received, e.g. RTS_BLK_DRV_TYPE_UDP */
	RTS_UI16 wGwRtsNsVersion;		/* Name service version used between runtime system and Gateway/PLCHandler(ARTI3) */
	RTS_UI32 ulGwClientGwVersion;	/* Name service version used between GwClient and Gateway (only for interface GATEWAY3)*/
}NodeInfotyp2;


/* obsolete, use DeviceInfo2 instead */
typedef struct DeviceInfoTag
{
	char*			pszNodeAddress;
	RTS_WCHAR*		pwszNodeName;
	RTS_WCHAR*		pwszTargetName;
	RTS_WCHAR*		pwszTargetVendorName;
	char*			pszTargetVersion;
	unsigned long	ulTargetId;
	unsigned long	ulTargetType;
	unsigned long	ulBufferSize;
	unsigned long	DeviceSessionId;
	unsigned long	bMotorola;
}DeviceInfo;

typedef DeviceInfo DeviceInfoStruct;

/* Target ident flags
* These flags are used to describe device features within ulTargetIdentFlags.
*/
#define TIF_NONE						NSPF_NONE						/* empty flag field */
#define TIF_ENCRYPTED_COMM_SUPPORTED	NSPF_ENCRYPTED_COMM_SUPPORTED	/* device supports encrypted online communication */
#define TIF_ENCRYPTED_COMM_REQUIRED		NSPF_ENCRYPTED_COMM_REQUIRED	/* device requires encrypted online communication */

typedef struct IpInfoTag
{
	char szIp[16];
	char szMask[16];
}IpInfo;

typedef struct DeviceInfoTag2
{
	char*			pszNodeAddress;				/* V3 CODESYS address */
	RTS_WCHAR*		pwszNodeName;				/* Name of the device */
	RTS_WCHAR*		pwszTargetName;				/* Name of the target type */
	RTS_WCHAR*		pwszTargetVendorName;		/* Name of the vendor */
	char*			pszTargetVersion;			/* Version of the device */
	unsigned long	ulTargetId;					/* Target id of the device */
	unsigned long	ulTargetType;				/* Target type of the device */
	unsigned long	ulBufferSize;				/* Communication buffer size of the device. No service must exceed this size! */	
	unsigned long	DeviceSessionId;			/* Session id of the current connection */
	unsigned long	bMotorola;					/* Byte order of the device */
	char*			pszTargetSerialNumber;		/* Serial number of the device */
	char*			pszTargetCoreVersion;		/* Version of the basic CODESYS runtime system */
	unsigned long	ulTargetIdentFlags;			/* Special features of the device, e. g. communication encryption */
	unsigned short	usTargetChannels;			/* Max. number of clients, which are supported at the same time */
	unsigned short	usNumOfIpAddresses;			/* Number of entries in the IP address list */
	IpInfo*			pIpAddressList;				/* List with IP addresses and subnet masks of the device */
}DeviceInfo2;

typedef DeviceInfo2 DeviceInfoStruct2;

typedef struct ProjectInfoTag
{
	unsigned long	ulProjectId;
	unsigned long	ulTimestampUtc; // UTC time in seconds since 1.1.1970 00:00:00
	char*			pszProject;
	char*			pszTitle;
	char*			pszVersion;
	char*			pszAuthor;
	char*			pszDescription;
}ProjectInfo;

typedef ProjectInfo ProjectInfoStruct;


/* obsolete, use ApplicationInfo2 instead */
typedef struct ApplicationInfoTag
{
	unsigned long	ulLastChanges; // UTC time in seconds since 1.1.1970 00:00:00
	char*			pszProject;
	char*			pszVersion;
	char*			pszAuthor;
	char*			pszDescription;
	char*			pszProfile;
}ApplicationInfo;

typedef ApplicationInfo ApplicationInfoStruct;


typedef struct ApplicationInfoTag2
{
	unsigned long	ulLastChanges; // UTC time in seconds since 1.1.1970 00:00:00
	char*			pszProject;
	char*			pszVersion;
	char*			pszAuthor;
	char*			pszDescription;
	char*			pszProfile;
	RTS_GUID		CodeGuid;
	RTS_GUID		DataGuid;
}ApplicationInfo2;

typedef ApplicationInfo2 ApplicationInfoStruct2;


typedef struct tagEXT_RS232_PARAMS
{
	unsigned long ulSize;	/* Size of this structure */
	char *pszParity;		/* Parity:  "No" or "Yes" */
	int nStopBits;			/* Number of Stopbits */
}
EXT_RS232_PARAMStyp;


/* Timeout definitions */
#define PLCHANDLER_USE_DEFAULT				0
#define PLCHANDLER_TIMEOUT_INFINITE			((unsigned long)-1)
#define PLCHANDLER_USE_WAITTIME_ONLY		((unsigned long)-2)


/* Variable list flags for CycDefineVarlist() / SyncDefineVarList() */
#define VARLIST_FLAG_NONE							0x00000000
#define VARLIST_FLAG_WRITE_ACCESS					0x00000001		/* Have to be set, if variable list should (also) used by SyncWriteVarListToPlc(). */
#define VARLIST_FLAG_CONSISTENT_BACKGROUND_READ		0x00000002		/* On PLC access is done in a background loop without blocking the IEC tasks. */
#define VARLIST_FLAG_CONSISTENT_SYNCHRONIZED_READ	0x00000004		/* On PLC access is synchronized with IEC tasks; may have an impact to realtime behavior. */
#define VARLIST_FLAG_CONSISTENT_SYNCHRONIZED_WRITE	0x00000008		/* On PLC access is synchronized with IEC tasks; may have an impact to realtime behavior. */


/* Device operation modes for GetDeviceOperationMode() / SetDeviceOperationMode() */
typedef enum
{
	DEVICE_OPMODE_UNKNOWN = 0,
	DEVICE_OPMODE_DEBUG = 1,
	DEVICE_OPMODE_LOCKED = 2,
	DEVICE_OPMODE_OPERATIONAL = 3
} DEVICE_OPERATION_MODE;

#endif	/*__PLCHANDLERDEFINES_H__*/
