#ifndef __CMPERRORS_H__
#define __CMPERRORS_H__


/* Common error codes 
  (range: 0x0000 - 0x00FF) 
*/
#define ERR_OK					0x0000
#define ERR_FAILED				0x0001  /* General error - to be used only for internal errors */
#define ERR_PARAMETER			0x0002
#define ERR_NOTINITIALIZED		0x0003  /* Function cannot be executed, since component has not been initialized yet. It may work later, though */
#define ERR_VERSION				0x0004  /* Version conflict */
#define ERR_TIMEOUT				0x0005  /* Operation timed out */
#define ERR_NOBUFFER			0x0006  /* Insufficient memory to carry out the request */
#define ERR_PENDING				0x000A  /* For async-calls: call not complete, yet */
#define ERR_NUMPENDING			0x000B  /* To many pending calls. Try later */
#define ERR_NOTIMPLEMENTED		0x000C  /* The function is not implemented */
#define ERR_INVALIDID			0x000D  /* No object with the provided id found */
#define ERR_OVERFLOW			0x000E  /* Integer overflow */
#define ERR_BUFFERSIZE			0x000F	/* The size of a buffer is to small or invalid */
#define ERR_NO_OBJECT			0x0010	/* No object with this specified name available */
#define ERR_NOMEMORY			0x0011	/* No heap memory available */
#define ERR_NO_MEMORY			ERR_NOMEMORY
#define ERR_DUPLICATE			0x0012	/* An object with the same name is still available */
#define ERR_MEMORY_OVERWRITE	0x0013	/* Heap memory was written out of bounds! Memory overwrite error */
#define ERR_INVALID_HANDLE		0x0014	/* Invalid handle to an object */
#define ERR_END_OF_OBJECT		0x0015	/* End of object reached */
#define ERR_NO_CHANGE			0x0016	/* No changes done */
#define ERR_INVALID_INTERFACE	0x0017	/* Invalid or unknown interface */
#define ERR_NOT_SUPPORTED		0x0018	/* Functionality not supported */
#define ERR_NO_ACCESS_RIGHTS	0x0019	/* No access rights for this operation */
#define ERR_OUT_OF_LIMITS		0x001A	/* Specified limits of a resource exceeded */
#define ERR_ENTRIES_REMAINING	0x001B	/* Remaining entries that could not be transmitted because of buffer limitation */
#define ERR_INVALID_SESSION_ID	0x001C	/* Invalid online sessionid */
#define ERR_EXCEPTION			0x001D	/* Exception occurred */
#define ERR_SIGNATURE_MISMATCH	0x001E	/* Signature mismatch of an api function */
#define ERR_VERSION_MISMATCH	0x001F	/* Version mismatch */
#define ERR_TYPE_MISMATCH		0x0020	/* Type mismatch */
#define ERR_ID_MISMATCH			0x0021	/* ID mismatch */
#define ERR_NO_CONSISTENCY		0x0022	/* Consistency error */
#define ERR_NO_COMM_CYCLE		0x0023	/* No COMM_CYCLE needed */
#define ERR_DONT_SUSPEND_TASK	0x0024	/* Do not suspend task after an exception */
#define ERR_MEMORY_LOCK_FAILED	0x0025	/* Memory cannot be locked in this operation */
#define ERR_LICENSE_MISSING		0x0026	/* License missing for the runtime */
#define ERR_OPERATION_DENIED	0x0027	/* Operation denied */
#define ERR_DEVICE				0x0028	/* Device error */
#define ERR_DISK_FULL			0x0029	/* Disk full */
#define ERR_CRC_FAILED			0x0030
#define ERR_MEDIA_ERASE			0x0031
#define ERR_FILE_ERROR			0x0032	/* File error. e.g. cannot open a file for writing because it could be write protected */
#define ERR_NO_RETAIN_MEMORY	0x0033	/* No retain memory available */
#define ERR_OUT_OF_LIMITS_MIN	0x0034	/* Specified minimum-limit of a resource exceeded */
#define ERR_OUT_OF_LIMITS_MAX	0x0035	/* Specified maximum-limit of a resource exceeded */
#define ERR_SIZE_MISMATCH		0x0036	/* Size mismatch */
#define ERR_CALL_AGAIN			0x0037	/* Operation is not yet finished, call function again to proceed */
#define ERR_NOTHING_TO_DO		0x0038	/* Operation has nothing to do. No execution. */
#define ERR_SECURITY_CHECKS_FAILED 0x0039	/* Some security checks have failed. This is a generic error code to report this error over public channels. In this case the error code doesn't provide a detailed cause for the error. */
#define ERR_INVALID_SEQUENCE	0x003A
#define ERR_INVALID_REFERENCE	0x003B	/* Dereferencing an IEC reference in IecVarAccess failed due to invalid destination address, e. G. NULL. */
#define ERR_CONVERSION_INCOMPLETE 0x003C /* Conversion of string encodings was not lossless. */
#define ERR_TOO_MANY_RETRIES	0x003D	/* Too many retries of an action */
#define ERR_ALIGNMENT			0x003E /* There is an unaligned access on memory detected. */
#define ERR_CHANGE_PASSWORD		0x003F /* The password has to be changed */
#define ERR_REAUTHENTICATE		0x0040 /* The user has to reauthenticate oneself */
#define ERR_RELOGIN				0x0041 /* The user has to relogin */

/* General network errors 
  (range: 0x0100 - 0x010F)
*/
#define ERR_NET_FAILED			0x0100  /* General failure of a network call */
#define ERR_NET_NOTCONNECTED	0x0101	/* Connection not established or closed */
#define ERR_NET_MSGSIZE			0x0102  /* Message exceeds the maximum message size */
#define ERR_NET_NOROUTE			0x0103  /* No route found */
#define ERR_NET_TIMEOUT			0x0104  /* Timeout of the network call */
#define ERR_NET_PKGINVALID		0x0105  /* A network (Layer 3) package is invalid */
#define ERR_NET_INVALIDADDR     0x0106  /* Invalid address */

/* Network service errors (issued by the server of a network service)
   (range: 0x0120 - 0x012F)
*/
#define ERR_SVC_PKGINVALID			0x0120  /* The received package is not compliant with the service specification */
#define ERR_SVC_INVALIDSERVICE		0x0121  /* The requested service does not exist */
#define ERR_SVC_VERSION_MISMATCH	0x0122  /* The requested service version does not exist */

/* Network client errors (issued by the client of a network service)
   (range: 0x0140 - 0x014F)
*/
#define ERR_CLNT_PKGINVALID			0x0140	/* The received package is not compliant with the service specification */

/* Channel server (PLC) errors 
  (range: 0x0180 - 0x019F)
*/
#define ERR_CHS_CHINVALID	  0x0181 /* Channel Id invalid */
#define ERR_CHS_TIMEOUT		  0x0182 /* Channel closed due to a timeout */
#define ERR_CHS_PROTOCOL	  0x0183 /* Protocol error */
#define ERR_CHS_CHECKSUM	  0x0184 /* Inrecoverable transmission error (checksum) etc. */
#define ERR_CHS_MODEHOST	  0x0185 /* The server application tried to send a message although the channel is in receive mode */
#define ERR_CHS_MODEPEER	  0x0186 /* Incoming message although the channel is in send mode. */
#define ERR_CHS_NUMCHANNELS	  0x0187 /* No more channels available. */
#define ERR_CHS_CLOSED		  0x0188 /* Channel closed by peer */
#define ERR_CHS_MESSAGESIZE   0x0189 /* Message exceeds the commbuffer size */
#define ERR_CHS_REQNOTHANDLED 0x018A /* The server application didn't properly handle an incoming request */
#define ERR_CHS_SHUTDOWN	  0x018B /* The channel is closed due a runtime system shutdown */
#define ERR_CHS_ENCRYPTION_NOT_READY_YET	0x018C /* The CmpSecureChannel is not ready for encrypted communication. Try again later */

#define ERR_CHS_NOTSERVER     0x0190 /* This node is not a server node */

/* Channel client (eg. the gateway) errors 
  (range: 0x01A0 - 0x01BF)
*/
#define ERR_CHC_CHINVALID	0x01A1 /* Channel Id invalid */
#define ERR_CHC_TIMEOUT		0x01A2 /* Channel closed or connect failed due to a timeout */
#define ERR_CHC_PROTOCOL	0x01A3 /* Protocol error */
#define ERR_CHC_CHECKSUM	0x01A4 /* Inrecoverable transmission error (checksum) etc. */
#define ERR_CHC_MODEHOST	0x01A5 /* The client application tried to send a message although the channel is in receive mode */
#define ERR_CHC_MODEPEER	0x01A6 /* Incoming message although the channel is in send mode. */
#define ERR_CHC_NUMCHANNELS	0x01A7 /* No more channels available. */
#define ERR_CHC_CLOSED		0x01A8 /* Channel closed by peer */
#define ERR_CHC_MESSAGESIZE 0x01A9 /* Message exceeds the commbuffer size */
#define ERR_CHC_NOCHBUFFER  0x01AA /* Not enough memory to provide the required communication buffer */


/* Gateway errors (Client-Gateway communication)
   (range: 0x01C0 - 0x01D0)
*/
#define ERR_GW_INVALIDPKG				0x01C0  /* Invalid pkg */
#define ERR_GW_PROTOCOL					0x01C1  /* Protocol error */
#define ERR_GW_VERSION					0x01C2  /* Clients protocol version cannot be handled */
#define ERR_GW_NOAUTHMODULE				0x01C3  /* The authentication modules provided by the client are not available on the gateway */
#define ERR_GW_UNKNOWNCMD				0x01C4  /* Command or command group not known by the gateway */
#define ERR_GW_REQCANCELED				0x01C5  /* The request has been canceled by a cancelrequest message */
#define ERR_GW_REQUNKNOWN				0x01C6  /* A reference to an other request is unknown (eg. a cancelRequest refering to a request that has already been answered. */
#define ERR_GW_CANCELFAILED				0x01C7  /* The referenced request cannot be canceled (certain requests may not be canceled once they are initiated) */
#define ERR_GW_INVALIDCONN				0x01C8  /* The connection handle is invalid. */
#define ERR_GW_NUMCONNECTIONS			0x01C9  /* GWClient/driver can't handle another connection. */
#define ERR_GW_PARAMETER_NOT_SUPPORTED	0x01CA  /* Parameter values are not supported by the negotiated protocol version between GwClient and Gateway (old Gateway). */
#define ERR_GW_COMMAND_NOT_SUPPORTED	0x01CB  /* Command is not supported by the negotiated protocol version between GwClient and Gateway (old Gateway). */
#define ERR_GW_CLIENT_TIMEOUT			0x01CC  /* GwClient has not received any data within the timeout */
#define ERR_GW_TIMEOUT					0x01CD  /* Gateway has not received any data within the timeout */

/* Socket errors 
  (range: 0x0200 - 0x02FF)  
*/
#define ERR_SOCK_NOTINITIALIZED			0x0201
#define ERR_SOCK_NOTSOCKET				0x0202  /* The provided socket handle is invalid */
#define ERR_SOCK_AFUNSUPPORTED			0x0203  /* The address family is not supported */
#define ERR_SOCK_PROTOUNSUPPORTED		0x0204  /* Protocol is not supported */
#define ERR_SOCK_NOBUFFER				0x0205  /* Not enough buffer to handle the request */
#define ERR_SOCK_WOULDBLOCK				0x0206  /* Socket is in nonblocking mode but this call would block */
#define ERR_SOCK_ADDRINUSE				0x0207  /* The provided address is already in use */
#define ERR_SOCK_ADDRNOTAVAILABLE		0x0208  /* The provided address is not available on this computer */
#define ERR_SOCK_CONNREFUSED			0x0209  /* Connection has been refused by the remote host */
#define ERR_SOCK_TIMEDOUT				0x020A  /* Operation timed out */
#define ERR_SOCK_HOSTNOTFOUND			0x020B  /* The host has not been found */
#define ERR_SOCK_HOSTUNREACHABLE		0x020C  /* Host is unreachable */
#define ERR_SOCK_ISCONNECTED			0x020D  /* Socket is already connected */
#define ERR_SOCK_NOTCONNECTED			0x020E  /* The socket is not connected */
#define ERR_SOCK_SHUTDOWN				0x020F  /* Shutdown has been called on the socket */
#define ERR_SOCK_MSGSIZE				0x0210  /* For sockets of type DGRAM. The package to send exceeds the maximum package size */
#define ERR_SOCK_CLOSED					0x0211  /* Socket has been gracefully closed. No more send/receives allowed */

/* L7 Protocol errors 
  (range: 0x0300 - 0x03FF)
*/
#define ERR_L7_TAG_MISSING				0x0300	/* Tag missing in online communication buffer */
#define ERR_L7_UNKNOWNCMDGROUP			0x0301  /* Unknown command group */
#define ERR_L7_UNKNOWNCMD				0x0302	/* Unknown command (within a valid command group) */
#define ERR_L7_INCOMPLETE				0x0303	/* Level 7 service incomplete */
#define ERR_L7_PROTOCOLMISMATCH			0x0304	/* Received protocol does not match to the expected protocol on this channel */

/* Errors of the BinTagUtils library
   (range: 0x0400-0x040F)
*/
#define ERR_BTAG_TAGOVERLAP				0x0400 /* A nested tag reaches beyond the end of the surrounding tag */
#define ERR_BTAG_BUFFEROVERRUN			0x0401 /* A tag reaches beyond the end of the buffer */
#define ERR_BTAG_MAXNESTINGDEPTH		0x0402 /* The number of nested tags exceeds the maximum nesting depth */
#define ERR_BTAG_NOTAG					0x0403 /* No current tag - reader/writer is at the toplevel. */
#define ERR_BTAG_NODATATAG				0x0404 /* The current tag is not a data tag */
#define ERR_BTAG_NOCONTAINERTAG			0x0405 /* The operation is allowed only on a container tag not on a data tag */
#define ERR_BTAG_NOTTOPLEVEL			0x0406 /* Operation only allowed on toplevel (not inside a tag) */
#define ERR_BTAG_ALIGNMENT				0x0407 /* Invalid alignment */

/* Errors of the Application component
   (range: 0x0500-0x05FF)
*/
#define ERR_APP_EXISTS					0x0500 /* Application with same name exists */
#define ERR_APP_NOT_EXIST				0x0501 /* Application not available */
#define ERR_APP_INVALID_SESSIONID		0x0502 /* Invalid sessionid */
#define ERR_APP_INVALID_DATAID			0x0503 /* Invalid dataid of application. Data don't match! */
#define ERR_APP_NOT_LOADED				0x0504 /* Application not loaded */
#define ERR_APP_NO_SESSIONID			0x0505 /* no sessionid specified */
#define ERR_APP_UNRESOLVED_REFERENCE	0x0506 /* unresolved external function */
#define ERR_APP_SIGNATURE_MISMATCH		0x0507 /* signature mismatch */
#define ERR_APP_LOAD_RETAIN_DATA_FAILED	0x0508 /* loading retain data failed */
#define ERR_APP_VERSION_MISMATCH		0x0509 /* Version mismatch of an external lib function */
#define ERR_APP_RETAIN_MISMATCH			0x050A /* Retain mismatch of the retain area */
#define ERR_APP_SINGLECYCLE_ON_BREAKPOINT	0x050B /* Ask, if single cycle should be done on breakpoint */
#define ERR_APP_CREATE_BOOTPROJECT_FAILED	0x050C /* Bootproject could not be created */
#define ERR_APP_BP_CONDITION_TOO_COMPLEX  0x50D   /* Breakpoint condition too complex */
#define ERR_APP_BP_BREAKPOINT_NOT_SET     0x50E   /* Breakpoint could not be set */
#define ERR_APP_BP_INTERP_CODE_TOO_COMPLEX	0x50F /* Interpreter code for Breakpoint is too complex */
#define ERR_APP_CREATE_BOOTPROJECT_FAILED_DISK_FULL	0x0510 /* Bootproject could not be created, disk full */
#define ERR_APP_LOADING_CINTEGRATION_MODULES	0x0511 /* Loading any of the C-Integration modules failed */
#define ERR_APP_DONT_INIT_AREA			0x0512 /* Don't initialize area */
#define ERR_APP_MAPPED_AREA_NOT_FOUND	0x0513 /* A data segment has been mapped to an area that could not be found */
#define ERR_APP_SIGNATURE_MISSING		0x0514 /* The digital signature of the download is missing. This is requried by the runtime configuration */
#define ERR_APP_DECRYPTION_FAILED		0x0515 /* The decrytion of the application code failed */
#define ERR_APP_ENCRYPTION_MISSING		0x0516 /* The application code is not encrypted. This is required by the runtime */
#define ERR_APP_SIGNATURE_AND_ENCRYPTION_MISSING 0x0517 /* The application code is not signed and encrypted */
#define ERR_APP_BP_BREAKPOINT_NOT_DELETED   0x0518   /* Breakpoint could not be deleted */

/* TLS errors
 * (range: 0x0600 - 0x06FF)
 */

#define ERR_TLS_INTERNAL					0x0600		/* Internal Error of TLS component */
#define ERR_TLS_RAND_NOT_INIT				0x0601		/* Random number generator was not seeded with enough entropy */
/* IO operations */
#define ERR_TLS_CONNECTION_CLOSED			0x0610		/* The TLS connection has been closed. */
#define ERR_TLS_RETRY_OPERATION				0x0611		/* The same TLS operation should be called again */
#define ERR_TLS_IO_SYSTEM					0x0612		/* The underlaying transport caused a problem */
#define ERR_TLS_WANT_READ					0x0613		/* The finish this operation data have to be read. The data was not available. Receive TLS protocol data to finish this operation */
#define ERR_TLS_WANT_WRITE					0x0614		/* To finish this operation data has to be written. The buffer doesn't allow that. Send existing data to allow writing new data */

/* Certificate errors*/
#define ERR_TLS_CERT_INVALID				0x0620
#define ERR_TLS_CERT_SELFSIGNED				0x0621
#define ERR_TLS_CERT_EXPIRED				0x0622

/*
 * X509 Cert errors range(0x700 - 0x7FF)
 */

/* Certificate validation error codes */
#define ERR_CERT_OK										ERR_OK

/* illegal error (for uninitialized values, to avoid ERR_CERT_OK) */
#define ERR_CERT_UNABLE_TO_GET_ISSUER_CERT            	0x701
#define ERR_CERT_UNABLE_TO_GET_CRL                    	0x702
#define ERR_CERT_UNABLE_TO_DECRYPT_CERT_SIGNATURE     	0x703
#define ERR_CERT_UNABLE_TO_DECRYPT_CRL_SIGNATURE      	0x704
#define ERR_CERT_UNABLE_TO_DECODE_ISSUER_PUBLIC_KEY   	0x705
#define ERR_CERT_SIGNATURE_FAILURE                		0x706
#define ERR_CERT_CRL_SIGNATURE_FAILURE                  0x707
#define ERR_CERT_NOT_YET_VALID                     		0x708
#define ERR_CERT_HAS_EXPIRED                       		0x709
#define ERR_CERT_CRL_NOT_YET_VALID                      0x70a
#define ERR_CERT_CRL_HAS_EXPIRED                        0x70b
#define ERR_CERT_ERROR_IN_CERT_NOT_BEFORE_FIELD         0x70c
#define ERR_CERT_ERROR_IN_CERT_NOT_AFTER_FIELD          0x70d
#define ERR_CERT_ERROR_IN_CRL_LAST_UPDATE_FIELD         0x70e
#define ERR_CERT_ERROR_IN_CRL_NEXT_UPDATE_FIELD         0x70f
#define ERR_CERT_OUT_OF_MEM                             0x710
#define ERR_CERT_DEPTH_ZERO_SELF_SIGNED_CERT            0x711
#define ERR_CERT_SELF_SIGNED_CERT_IN_CHAIN              0x712
#define ERR_CERT_UNABLE_TO_GET_ISSUER_CERT_LOCALLY      0x713
#define ERR_CERT_UNABLE_TO_VERIFY_LEAF_SIGNATURE        0x714
#define ERR_CERT_CHAIN_TOO_LONG                    		0x715
#define ERR_CERT_REVOKED                           		0x716
#define ERR_CERT_INVALID_CA                             0x717
#define ERR_CERT_PATH_LENGTH_EXCEEDED                   0x718
#define ERR_CERT_INVALID_PURPOSE                        0x719
#define ERR_CERT_UNTRUSTED                         		0x71a
#define ERR_CERT_REJECTED                          		0x71b

/* These are 'informational' when looking for issuer cert */
#define ERR_CERT_SUBJECT_ISSUER_MISMATCH                0x71c
#define ERR_CERT_AKID_SKID_MISMATCH                     0x71d
#define ERR_CERT_AKID_ISSUER_SERIAL_MISMATCH            0x71e
#define ERR_CERT_KEYUSAGE_NO_CERTSIGN                   0x71f
#define ERR_CERT_UNABLE_TO_GET_CRL_ISSUER               0x720
#define ERR_CERT_UNHANDLED_CRITICAL_EXTENSION           0x721
#define ERR_CERT_KEYUSAGE_NO_CRL_SIGN                   0x722
#define ERR_CERT_UNHANDLED_CRITICAL_CRL_EXTENSION       0x723
#define ERR_CERT_INVALID_NON_CA                         0x724
#define ERR_CERT_PROXY_PATH_LENGTH_EXCEEDED             0x725
#define ERR_CERT_KEYUSAGE_NO_DIGITAL_SIGNATURE          0x726
#define ERR_CERT_PROXY_CERTIFICATES_NOT_ALLOWED         0x727
#define ERR_CERT_INVALID_EXTENSION                      0x728
#define ERR_CERT_INVALID_POLICY_EXTENSION               0x729
#define ERR_CERT_NO_EXPLICIT_POLICY                     0x72a
#define ERR_CERT_DIFFERENT_CRL_SCOPE                    0x72b
#define ERR_CERT_UNSUPPORTED_EXTENSION_FEATURE          0x72c
#define ERR_CERT_UNNESTED_RESOURCE                      0x72d
#define ERR_CERT_PERMITTED_VIOLATION                    0x72e
#define ERR_CERT_EXCLUDED_VIOLATION                     0x72f
#define ERR_CERT_SUBTREE_MINMAX                         0x730
#define ERR_CERT_UNSUPPORTED_CONSTRAINT_TYPE            0x731
#define ERR_CERT_UNSUPPORTED_CONSTRAINT_SYNTAX          0x732
#define ERR_CERT_UNSUPPORTED_NAME_SYNTAX                0x733
#define ERR_CERT_CRL_PATH_VALIDATION_ERROR            	0x734

/* The application is not happy */
#define ERR_CERT_APPLICATION_VERIFICATION				0x735

#define ERR_CERT_MISSING								0x736
#define ERR_CERT_UNKNOWN_ERROR							0x737

/* Monitoring errors - 
	For use in the monitoring reply packages only. Not to be mixed with the
	standard ERR_xxx result values.
	These errors values must not exceed a single byte (ie. <= 0xFF)
*/
#define MONITORING_ERR_OK					   0x00
#define	MONITORING_ERR_INVALID_AREA            0x01	
#define	MONITORING_ERR_UNSUPPORTED_DATATYPE    0x02
#define MONITORING_ERR_SIZE_0				   0x03
#define MONITORING_ERR_NOBP					   0x04
#define MONITORING_ERR_INVALID_POINTER		   0x05
#define MONITORING_ERR_NESTING_DEPTH		   0x06
#define MONITORING_ERR_NO_CONSISTENCY		   0x07
#define MONITORING_ERR_BUFFERSIZE_EXCEEDED	   0x08
#define MONITORING_ERR_INVALID_INDEX		   0x09
#define MONITORING_ERR_UNSUPPORTED_OPERATOR	   0x0A
#define MONITORING_ERR_OPERATION_DENIED		   0x0B
#define MONITORING_ERR_EXCEPTION               0x0C  /* An exception occurred while executing the monitoring expression      */
#define MONITORING_ERR_STACK_OVERFLOW          0x0D  /* The monitoring stack was not large enough to evaluate the expression */
#define MONITORING_ERR_STACK_UNDERFLOW         0x0E  /* The expression caused an underflow of the monitoring stack           */
#define MONITORING_ERR_IOPARAM                 0x0F  /* Accessing an I/O parameter failed                                    */
#define MONITORING_ERR_DIVISIONBYZERO          0x10  /* A division by zero occured                                           */
#define MONITORING_ERR_INVALIDBRANCHOFFSET     0x11  /* The branch offset is too large in magnitude                          */
#define MONITORING_ERR_NOAPP                   0x12  /* No application context has been given                                */
#define MONITORING_ERR_WRONGFUNCTION           0x13  /* The break point is not in the current function                       */
#define MONITORING_ERR_FORCE_FAILED			   0x14  /* Adding a forced valule failed										 */
#define MONITORING_ERR_UNKNOWN				   0xff


/* FileTransfer errors */
#define ERR_FT_FILE_SIZE_MISMATCH				0x0800	/* File size don't match */
#define ERR_FT_CRC_MISMATCH						0x0801	/* Crc of file don't match */

/* IecVarAccess errors */
#define ERR_IECVARACC_UNKNOWN_NODE_TYPE			0x0900	/* Unknown symbolic node type */
#define ERR_IECVARACC_NO_CHILD_NODE				0x0901	/* No child not available */
#define ERR_IECVARACC_CONFIG_CHANGED			0x0902	/* Symbolconfiguration has changed */
#define ERR_IECVARACC_SYNC_CONSISTENCY_DENIED	0x0903	/* AF_ALLOW_SYMBOLIC_VARIABLE_ACCESS_IN_SYNC_WITH_IEC_TASK is not set for all applications on the device*/

/* OEM specific error code range */
#define IsOEMErrorCode(r)						((r & 0x8000) != 0)
#define ERR_OEM_START							0x8000	/* OEM error range start */
#define ERR_OEM_END								0xFFFF	/* OEM error range end */

#endif	/*__CMPERRORS_H__*/
