  /**
 * <interfacename>CmpTLS</interfacename>
 * <description> 
 *	<p>Interface for TLS encrypted communication over sockets and encrypted CODESYS communication</p>
 *	Limitations:
 *	- Component should only be used to support TLS within the web-server. Validation of the peer certificate
 *	is not available. So there is no secure way to establish a secure client connection.
 *	- The functions for secure CODESYS communication are only prototypes. They don't have to be used.
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */
 
 
	
	
#ifndef _CMPTLSITF_H_
#define _CMPTLSITF_H_

#include "CmpStd.h"

 

 



#include "CmpSrvItf.h"
#include "CmpCryptoItf.h"

/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Enumeration: CmpTlsMethod</description>
 */
#define CMPTLSMETHOD_SSLV2_CLIENT    RTS_IEC_INT_C(0x0)	/* Not Available */
#define CMPTLSMETHOD_SSLV3_CLIENT    RTS_IEC_INT_C(0x1)	/* SSL v3 Client */
#define CMPTLSMETHOD_SSLV23_CLIENT    RTS_IEC_INT_C(0x2)	/* SSL Client accepting all versions but SSL v2 */
#define CMPTLSMETHOD_TLSV1_CLIENT    RTS_IEC_INT_C(0x3)	/* TLS 1.1 Client */
#define CMPTLSMETHOD_TLSV2_CLIENT    RTS_IEC_INT_C(0x4)	/* TLS 1.2 Client (Default for CmpTlsCreateContext) */
#define CMPTLSMETHOD_SSLV2_SERVER    RTS_IEC_INT_C(0x5)	/* Not Available */
#define CMPTLSMETHOD_SSLV3_SERVER    RTS_IEC_INT_C(0x6)	/* SSL v3 Server */
#define CMPTLSMETHOD_SSLV23_SERVER    RTS_IEC_INT_C(0x7)	/* SSL Server accepting all versions but SSL v2 */
#define CMPTLSMETHOD_TLSV1_SERVER    RTS_IEC_INT_C(0x8)	/* TLS 1.1 Server */
#define CMPTLSMETHOD_TLSV2_SERVER    RTS_IEC_INT_C(0x9)	/* TLS 1.2 Server */
#define CMPTLSMETHOD_TLSV3_CLIENT    RTS_IEC_INT_C(0xA)	/* TLS 1.3 Client */
#define CMPTLSMETHOD_TLSV3_SERVER    RTS_IEC_INT_C(0xB)	/* TLS 1.3 Server */
/* Typed enum definition */
#define CMPTLSMETHOD    RTS_IEC_INT

#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/

/* Availabe SSL / TLS versions */
#define TLS_METHOD_SSLv2_CLIENT		CMPTLSMETHOD_SSLV2_CLIENT		/* Not compiled */
#define TLS_METHOD_SSLv3_CLIENT		CMPTLSMETHOD_SSLV3_CLIENT
#define TLS_METHOD_SSLv23_CLIENT	CMPTLSMETHOD_SSLV23_CLIENT
#define TLS_METHOD_TLSv1_CLIENT		CMPTLSMETHOD_TLSV1_CLIENT
#define TLS_METHOD_TLSv2_CLIENT		CMPTLSMETHOD_TLSV2_CLIENT
#define TLS_METHOD_SSLv2_SERVER		CMPTLSMETHOD_SSLV2_SERVER		/* Not compiled */
#define TLS_METHOD_SSLv3_SERVER		CMPTLSMETHOD_SSLV3_SERVER
#define TLS_METHOD_SSLv23_SERVER	CMPTLSMETHOD_SSLV23_SERVER
#define TLS_METHOD_TLSv1_SERVER		CMPTLSMETHOD_TLSV1_SERVER
#define TLS_METHOD_TLSv2_SERVER		CMPTLSMETHOD_TLSV2_SERVER
#define TLS_METHOD_LAST_INDEX		0x0A


/* Verification Methods */
#define TLS_VERIFY_NONE					0x01
#define TLS_VERIFY_PEER					0x02
#define TLS_VERIFY_FAIL_IF_NO_PEER_CERT	0x04
#define TLS_VERIFY_CLIENT_ONCE			0x08

/* Versions */
#define TLS_FIRST_RELEASE_MAJOR		1
#define TLS_FIRST_RELEASE_MINOR		0

#define TLS_HIGHEST_SUPPORTED_MAJOR 1
#define TLS_HIGHEST_SUPPORTED_MINOR 0

/* Maximum sizes of TLS records */
#define TLS_MAX_RECORD_SIZE			0x4000
#define TLS_MAX_HEADER_SIZE			0x0158


/**
 * <category>Memory settings</category>
 * <description>Maximum number of allowed TLS connections</description>
 */
#ifndef TLS_MAX_CONNECTION_COUNT
	#define TLS_MAX_CONNECTION_COUNT					100
#endif

/**
 * <category>Memory settings</category>
 * <description>Maximum number of allowed TLS contexts</description>
 */
#ifndef TLS_MAX_CONTEXT_COUNT
	#define TLS_MAX_CONTEXT_COUNT					10
#endif

/**
 * <type>String</type>
 * <description>Obsolete: use placeholder instead (see SysFileItf.h)
 * Path to the base directory of the PKI infrastructure. The path should contain a trailing slash.
 * This setting is only available until a component for managing certificates is available. Will be removed in future!
 * </description>
 */
#define TLSKEY_STR_CERT_STORE_BASE_DIR				"CertStoreBase"
#ifndef TLSVALUE_STR_CERT_STORE_BASE_DIR_DEFAULT
	#define TLSVALUE_STR_CERT_STORE_BASE_DIR_DEFAULT	"./PKI/"
#endif

/* Folder structure inside the PKI infrastructure. Don't change these defines to avoid future compatibility issues.*/
#define TLS_CERT_STORE_CERT_DIR		"cert/"
#define TLS_CERT_STORE_PRIVATE_DIR	"private/"
#define TLS_CERT_STORE_TURESTED_DIR	"trusted/"
#define TLS_CERT_STORE_CRL_DIR		"crl/"


/* These settings are used as name space settings. 
 * Use Namespace.Setting in the configuration file to set them.
 * If you create a context using the function TlsCreateContext, use the
 * Name space used in the configuration file for your context. 
 */

/**
 * <category>Context settings</category>
 * <type>Int</type>
 * <description>Protocol version of TLS or SSL connection. Can be set per context.
 * Select out of the following values:
 *	- SSL v2: not available
 *	- SSLv3: client = 1, server = 6
 *	- Auto selection from SSLv3 to TLS1.2: client = 2, server = 7
 *	- TLSv1.1: client = 3, server = 8
 *	- TLSv1.2: client = 4, server = 9 (default)
</description>
 */
#define TLSKEY_INT_CTX_METHOD							"Method"
#ifndef TLSVALUE_INT_CTX_METHOD_DEFAULT
	#define TLSVALUE_INT_CTX_METHOD_DEFAULT				TLS_METHOD_TLSv2_SERVER
#endif

/**
 * <category>Context settings</category>
 * <type>String</type>
 * <description>Certificate for a context. Value is the filename of the 
 * certificate inside the PKI/cert folder. Supported file types are BASE64 
 * encoding an DER encoding. See the manual for support in generating a private key
 * and the corresponding certificate. (Default = server.cer)</description>
 */
#define TLSKEY_STR_CTX_CERT_FILE						"Cert"
#ifndef TLSVALUE_STR_CTX_CERT_FILE_DEFAULT
	#define TLSVALUE_STR_CTX_CERT_FILE_DEFAULT			"server.cer"
#endif

/**
 * <category>Context settings</category>
 * <type>String</type>
 * <description>Private key of the certificate configured with the "Cert" setting.
 * Value is the filename of the private key inside the PKI/private folder.
 * Supported are BASE64 and DER encodings. (Default = sercer.key)</description>
 */
#define TLSKEY_STR_CTX_PRIVATE_KEY_FILE					"PrivateKey"
#ifndef TLSVALUE_STR_CTX_PRIVATE_KEY_FILE_DEFAULT
	#define TLSVALUE_STR_CTX_PRIVATE_KEY_FILE_DEFAULT	"server.key"
#endif

/**
 * <category>Context settings</category>
 * <type>Int</type>
 * <description>How the verification process at connection establishment is managed: 
 * Behaves different on client or server side:
 * - TLS_VERIFY_NONE (1): The peer side certificate is not checked against the local CAs. Default for server side.
 * - TLS_VERIFY_PEER (2): The peer side certificate is checked against the local CAs. Default on client side
 * - TLS_VERIFY_FAIL_IF_NO_PEER_CERT (4): Fail the verification of the peer send no cert. Server only. Add value to TLS_VERIFY_PEER
 * - TLS_VERIFY_CLIENT_ONCE (8): Verify the client on each renegotiation. Server only. Add value to TLS_VERIFY_PEER.
 * </description>
 */
#define TLSKEY_INT_CTX_VERIFY_MODE						"VerifyMode"

/**
 * <category>Context settings</category>
 * <type>String</type>
 * <description>A list of available cipher suites for the TLS Context.
 * See notation for cipher selection at: http://www.openssl.org/docs/apps/ciphers.html
 * </description>
 */
#define TLSKEY_STR_CTX_CIPHER_LIST						"CipherList"
#ifndef TLSVALUE_STR_CTX_CIPHER_LIST_DEFAULT
	#define TLSVALUE_STR_CTX_CIPHER_LIST_DEFAULT		"HIGH+ECDH:HIGH+DH"
#endif

/**
 * <category>Context settings</category>
 * <type>String</type>
 * <description>Path to a file in pem-format containing the Diffie-Hellman parameters
 * used for DH key exchange. These parameters are needed for secure Diffie-Hellman key exchange.
 * These parameters may be generated on a separate fast workstation. The generation will take a while.
 * Use the OpenSSL toolkit to generate these parameters. See the manual. The Value
 * is the filename of these parameters in the PKI/private folder.
 * </description>
 */
#define TLSKEY_STR_CTX_DH_PARAMS						"DHParams"
#ifndef TLSVALUE_STR_CTX_DH_PARAMS_DEFAULT
	#define TLSVALUE_STR_CTX_DH_PARAMS_DEFAULT			"dhparams.pem"
#endif

 
#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Creates a TLS context, needed for creating TLS connections. One context may be used for many TLS 
 * connections. A Context contains all the application specific information of the connection. New connections are 
 * initialized using a context.
 * ATTENTION: This function is deprecated. Use TlsCreateContext2 or TlsCreateContext3 instead.</description>
 * <param name="pszNamespace" type="IN">Name space of the context. Settings of the context are read from the configuration file.</param>
 * <param name="pResult" type="OUT">Result of this operation.</param>
 * <result>Handle to the created TLS context.</result>
 */
RTS_HANDLE CDECL TlsCreateContext(char* pszNamespace, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFTLSCREATECONTEXT) (char* pszNamespace, RTS_RESULT* pResult);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSCREATECONTEXT_NOTIMPLEMENTED)
	#define USE_TlsCreateContext
	#define EXT_TlsCreateContext
	#define GET_TlsCreateContext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsCreateContext(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_TlsCreateContext  FALSE
	#define EXP_TlsCreateContext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsCreateContext
	#define EXT_TlsCreateContext
	#define GET_TlsCreateContext(fl)  CAL_CMGETAPI( "TlsCreateContext" ) 
	#define CAL_TlsCreateContext  TlsCreateContext
	#define CHK_TlsCreateContext  TRUE
	#define EXP_TlsCreateContext  CAL_CMEXPAPI( "TlsCreateContext" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsCreateContext
	#define EXT_TlsCreateContext
	#define GET_TlsCreateContext(fl)  CAL_CMGETAPI( "TlsCreateContext" ) 
	#define CAL_TlsCreateContext  TlsCreateContext
	#define CHK_TlsCreateContext  TRUE
	#define EXP_TlsCreateContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsCreateContext", (RTS_UINTPTR)TlsCreateContext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsCreateContext
	#define EXT_CmpTlsTlsCreateContext
	#define GET_CmpTlsTlsCreateContext  ERR_OK
	#define CAL_CmpTlsTlsCreateContext pICmpTls->ITlsCreateContext
	#define CHK_CmpTlsTlsCreateContext (pICmpTls != NULL)
	#define EXP_CmpTlsTlsCreateContext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsCreateContext
	#define EXT_TlsCreateContext
	#define GET_TlsCreateContext(fl)  CAL_CMGETAPI( "TlsCreateContext" ) 
	#define CAL_TlsCreateContext pICmpTls->ITlsCreateContext
	#define CHK_TlsCreateContext (pICmpTls != NULL)
	#define EXP_TlsCreateContext  CAL_CMEXPAPI( "TlsCreateContext" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsCreateContext  PFTLSCREATECONTEXT pfTlsCreateContext;
	#define EXT_TlsCreateContext  extern PFTLSCREATECONTEXT pfTlsCreateContext;
	#define GET_TlsCreateContext(fl)  s_pfCMGetAPI2( "TlsCreateContext", (RTS_VOID_FCTPTR *)&pfTlsCreateContext, (fl), 0, 0)
	#define CAL_TlsCreateContext  pfTlsCreateContext
	#define CHK_TlsCreateContext  (pfTlsCreateContext != NULL)
	#define EXP_TlsCreateContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsCreateContext", (RTS_UINTPTR)TlsCreateContext, 0, 0) 
#endif




/**
 * <description>Creates a TLS context, needed for creating TLS connections. One context may be used for many TLS 
 * connections. A Context contains all the application specific information of the connection. New connections are 
 * initialized using a context.</description>
 * <param name="hCert" type="IN">Certificate used by the TLS connection. If a client connection should be established the certificate is optional.</param>
 * <param name="ui32TlsMethod" type="IN">Mode of the TLS connections. See: TLS_METHOD_... macros</param>
 * <param name="szCipherList" type="IN">Cipher selection of the TLS connection. See CipherList setting for details. If NULL the default of the setting will be used.</param>
 * <param name="i32VerifyMode" type="IN">Verify mode of the TLS connection. See: TLS_VERIFY... macros. Default server: NONE; Default client: VERIFY_PEER.</param>
 * <param name="pResult" type="IN">Result of the operation.</param>
 * <result>Handle to the created TLS context.</result>
 */
RTS_HANDLE CDECL TlsCreateContext2(RTS_HANDLE hCert, RTS_UI32 ui32TlsMethod, char* szCipherList, RTS_I32 i32VerifyMode,  RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFTLSCREATECONTEXT2) (RTS_HANDLE hCert, RTS_UI32 ui32TlsMethod, char* szCipherList, RTS_I32 i32VerifyMode,  RTS_RESULT* pResult);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSCREATECONTEXT2_NOTIMPLEMENTED)
	#define USE_TlsCreateContext2
	#define EXT_TlsCreateContext2
	#define GET_TlsCreateContext2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsCreateContext2(p0,p1,p2,p3,p4)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_TlsCreateContext2  FALSE
	#define EXP_TlsCreateContext2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsCreateContext2
	#define EXT_TlsCreateContext2
	#define GET_TlsCreateContext2(fl)  CAL_CMGETAPI( "TlsCreateContext2" ) 
	#define CAL_TlsCreateContext2  TlsCreateContext2
	#define CHK_TlsCreateContext2  TRUE
	#define EXP_TlsCreateContext2  CAL_CMEXPAPI( "TlsCreateContext2" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsCreateContext2
	#define EXT_TlsCreateContext2
	#define GET_TlsCreateContext2(fl)  CAL_CMGETAPI( "TlsCreateContext2" ) 
	#define CAL_TlsCreateContext2  TlsCreateContext2
	#define CHK_TlsCreateContext2  TRUE
	#define EXP_TlsCreateContext2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsCreateContext2", (RTS_UINTPTR)TlsCreateContext2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsCreateContext2
	#define EXT_CmpTlsTlsCreateContext2
	#define GET_CmpTlsTlsCreateContext2  ERR_OK
	#define CAL_CmpTlsTlsCreateContext2 pICmpTls->ITlsCreateContext2
	#define CHK_CmpTlsTlsCreateContext2 (pICmpTls != NULL)
	#define EXP_CmpTlsTlsCreateContext2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsCreateContext2
	#define EXT_TlsCreateContext2
	#define GET_TlsCreateContext2(fl)  CAL_CMGETAPI( "TlsCreateContext2" ) 
	#define CAL_TlsCreateContext2 pICmpTls->ITlsCreateContext2
	#define CHK_TlsCreateContext2 (pICmpTls != NULL)
	#define EXP_TlsCreateContext2  CAL_CMEXPAPI( "TlsCreateContext2" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsCreateContext2  PFTLSCREATECONTEXT2 pfTlsCreateContext2;
	#define EXT_TlsCreateContext2  extern PFTLSCREATECONTEXT2 pfTlsCreateContext2;
	#define GET_TlsCreateContext2(fl)  s_pfCMGetAPI2( "TlsCreateContext2", (RTS_VOID_FCTPTR *)&pfTlsCreateContext2, (fl), 0, 0)
	#define CAL_TlsCreateContext2  pfTlsCreateContext2
	#define CHK_TlsCreateContext2  (pfTlsCreateContext2 != NULL)
	#define EXP_TlsCreateContext2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsCreateContext2", (RTS_UINTPTR)TlsCreateContext2, 0, 0) 
#endif





/**
 * <description>This callback is called after the peers certificate has been validated. You can use this callback to do some additional verification.
 * Return ERR_CERT_OK to signal the verification is was OK. Return any other error code to cause the TLS connection to fail.</description>
 * <param name="hCert" type="IN">Certificate sent by the peer. This needs to be verified.</param>
 * <param name="verifyResult" type="IN">The verify result of the X509CertStore.</param>
 * <param name="hConnection" type="IN">The handle of the TLS connection.</param>
 * <param name="pUserParameter" type="IN">Parameter given in TlsCreatecontext3</param>
 * <param name="pResult" type="IN">Result of the operation.</param>
 * <result>Handle to the created TLS context.</result>
 */
typedef RTS_RESULT(CDECL *PFVERIFYCERTIFICATE)(RTS_HANDLE hCert, RTS_RESULT verifyResult, void* pUserParameter);

/**
 * <description>Creates a TLS context, needed for creating TLS connections. One context may be used for many TLS 
 * connections. A Context contains all the application specific information of the connection. New connections are 
 * initialized using a context.</description>
 * <param name="hCert" type="IN">Certificate used by the TLS connection. If a client connection should be established the certificate is optional.</param>
 * <param name="ui32TlsMethod" type="IN">Mode of the TLS connections. See: TLS_METHOD_... macros</param>
 * <param name="szCipherList" type="IN">Cipher selection of the TLS connection. See CipherList setting for details. If NULL the default of the setting will be used.</param>
 * <param name="i32VerifyMode" type="IN">Verify mode of the TLS connection. See: TLS_VERIFY... macros. Default server: NONE; Default client: VERIFY_PEER.</param>
 * <param name="pfVerifyCallback" type="IN">Callback function to verify the certificate. This callback is called after the certificate has been pre-validated by the certificate store.</param>
 * <param name="pUserParameter" type="IN">A parameter that is passed to the pfVerifyCallback.</param>
 * <param name="pResult" type="IN">Result of the operation.</param>
 * <result>Handle to the created TLS context.</result>
 */
RTS_HANDLE CDECL TlsCreateContext3(RTS_HANDLE hCert, RTS_UI32 ui32TlsMethod, char* szCipherList, RTS_I32 i32VerifyMode, PFVERIFYCERTIFICATE pfVerifyCallback, void* pUserParameter, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFTLSCREATECONTEXT3) (RTS_HANDLE hCert, RTS_UI32 ui32TlsMethod, char* szCipherList, RTS_I32 i32VerifyMode, PFVERIFYCERTIFICATE pfVerifyCallback, void* pUserParameter, RTS_RESULT* pResult);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSCREATECONTEXT3_NOTIMPLEMENTED)
	#define USE_TlsCreateContext3
	#define EXT_TlsCreateContext3
	#define GET_TlsCreateContext3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsCreateContext3(p0,p1,p2,p3,p4,p5,p6)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_TlsCreateContext3  FALSE
	#define EXP_TlsCreateContext3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsCreateContext3
	#define EXT_TlsCreateContext3
	#define GET_TlsCreateContext3(fl)  CAL_CMGETAPI( "TlsCreateContext3" ) 
	#define CAL_TlsCreateContext3  TlsCreateContext3
	#define CHK_TlsCreateContext3  TRUE
	#define EXP_TlsCreateContext3  CAL_CMEXPAPI( "TlsCreateContext3" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsCreateContext3
	#define EXT_TlsCreateContext3
	#define GET_TlsCreateContext3(fl)  CAL_CMGETAPI( "TlsCreateContext3" ) 
	#define CAL_TlsCreateContext3  TlsCreateContext3
	#define CHK_TlsCreateContext3  TRUE
	#define EXP_TlsCreateContext3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsCreateContext3", (RTS_UINTPTR)TlsCreateContext3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsCreateContext3
	#define EXT_CmpTlsTlsCreateContext3
	#define GET_CmpTlsTlsCreateContext3  ERR_OK
	#define CAL_CmpTlsTlsCreateContext3 pICmpTls->ITlsCreateContext3
	#define CHK_CmpTlsTlsCreateContext3 (pICmpTls != NULL)
	#define EXP_CmpTlsTlsCreateContext3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsCreateContext3
	#define EXT_TlsCreateContext3
	#define GET_TlsCreateContext3(fl)  CAL_CMGETAPI( "TlsCreateContext3" ) 
	#define CAL_TlsCreateContext3 pICmpTls->ITlsCreateContext3
	#define CHK_TlsCreateContext3 (pICmpTls != NULL)
	#define EXP_TlsCreateContext3  CAL_CMEXPAPI( "TlsCreateContext3" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsCreateContext3  PFTLSCREATECONTEXT3 pfTlsCreateContext3;
	#define EXT_TlsCreateContext3  extern PFTLSCREATECONTEXT3 pfTlsCreateContext3;
	#define GET_TlsCreateContext3(fl)  s_pfCMGetAPI2( "TlsCreateContext3", (RTS_VOID_FCTPTR *)&pfTlsCreateContext3, (fl), 0, 0)
	#define CAL_TlsCreateContext3  pfTlsCreateContext3
	#define CHK_TlsCreateContext3  (pfTlsCreateContext3 != NULL)
	#define EXP_TlsCreateContext3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsCreateContext3", (RTS_UINTPTR)TlsCreateContext3, 0, 0) 
#endif




/**
 * <description>Delete the TLS context created using TLSCreateContext</description>
 * <param name="hTlsCtx" type="IN">Handle to the context, that should be deleted</param>
 * <result>Result of this operation.</result>
 */
 RTS_RESULT CDECL TlsFreeContext(RTS_HANDLE hTlsContext);
typedef RTS_RESULT (CDECL * PFTLSFREECONTEXT) (RTS_HANDLE hTlsContext);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSFREECONTEXT_NOTIMPLEMENTED)
	#define USE_TlsFreeContext
	#define EXT_TlsFreeContext
	#define GET_TlsFreeContext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsFreeContext(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_TlsFreeContext  FALSE
	#define EXP_TlsFreeContext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsFreeContext
	#define EXT_TlsFreeContext
	#define GET_TlsFreeContext(fl)  CAL_CMGETAPI( "TlsFreeContext" ) 
	#define CAL_TlsFreeContext  TlsFreeContext
	#define CHK_TlsFreeContext  TRUE
	#define EXP_TlsFreeContext  CAL_CMEXPAPI( "TlsFreeContext" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsFreeContext
	#define EXT_TlsFreeContext
	#define GET_TlsFreeContext(fl)  CAL_CMGETAPI( "TlsFreeContext" ) 
	#define CAL_TlsFreeContext  TlsFreeContext
	#define CHK_TlsFreeContext  TRUE
	#define EXP_TlsFreeContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsFreeContext", (RTS_UINTPTR)TlsFreeContext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsFreeContext
	#define EXT_CmpTlsTlsFreeContext
	#define GET_CmpTlsTlsFreeContext  ERR_OK
	#define CAL_CmpTlsTlsFreeContext pICmpTls->ITlsFreeContext
	#define CHK_CmpTlsTlsFreeContext (pICmpTls != NULL)
	#define EXP_CmpTlsTlsFreeContext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsFreeContext
	#define EXT_TlsFreeContext
	#define GET_TlsFreeContext(fl)  CAL_CMGETAPI( "TlsFreeContext" ) 
	#define CAL_TlsFreeContext pICmpTls->ITlsFreeContext
	#define CHK_TlsFreeContext (pICmpTls != NULL)
	#define EXP_TlsFreeContext  CAL_CMEXPAPI( "TlsFreeContext" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsFreeContext  PFTLSFREECONTEXT pfTlsFreeContext;
	#define EXT_TlsFreeContext  extern PFTLSFREECONTEXT pfTlsFreeContext;
	#define GET_TlsFreeContext(fl)  s_pfCMGetAPI2( "TlsFreeContext", (RTS_VOID_FCTPTR *)&pfTlsFreeContext, (fl), 0, 0)
	#define CAL_TlsFreeContext  pfTlsFreeContext
	#define CHK_TlsFreeContext  (pfTlsFreeContext != NULL)
	#define EXP_TlsFreeContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsFreeContext", (RTS_UINTPTR)TlsFreeContext, 0, 0) 
#endif



 
 /**
 * <description>Create a TLS connection around any existing transport media. There are two buffers needed:
 * a send and a receive buffer. Put the data received from the peer to the Rx buffer. The data written by the TLS implementation 
 * will be put to the Tx buffer. This buffer has to be sent to the peer after calling the connect, accept or write functions. Maybe
 * after calling the read function the buffer has to be sent too. This depends on internal TLS protocol stuff (change of 
 * session keys etc) which may occur at any time. This will be indicated through the corresponding returned flag.
 * The TLS connection will be established explicit by calling TSLConnect, or on the server side by calling the Accept function.
 * If the handshaking hasn't finished the read and write functions will internally finish the handshaking until the connection is
 * established. The implementation will start writing at the beginning at each call. If the function indicated that the
 * the data must be sent the next call will overwrite the data in the Tx buffer. So the caller must handle the data before calling
 * the next TLS function. The buffers given here contain raw TLS protocol data. This data has to be transferred to the peer using
 * a reliable way. The packages have to arrive completely and in the right order. Otherwise TLS won't work. 
 * To read or to write the corresponding plain text data use the TlsRead and TlsWrite functions.
 * Handling is done as follows:
 *    Received data: The bytestrings pointer will point to the beginning of the data.
 *                   The bytestrings max length will hold the buffers size
 *                   The bytestrings length will hold the current available amount of data.
 *                   The function ResetRx will not change any of the byte string. Only the internal read position will be set to the beginning of the buffer
 *
 *    Transmit data: The bytestrings pointer will point to the of the empty writing buffer.
 *                   The bytestrings max length will hold the buffer size
 *                   The bytestrings length will contain the amount of data written by the TLS implementation.
 *                   The function ResetTx will reset the internal write position. The length of the byte string will be set to 0.
 *                   To make the handling of the Tx buffer easier the pDataWritten will be set to 1. This indicates that the TLS implementation has written some data.
 * </description>
 * <param name="hSocket" type="IN">The socket handle, used for the TLC connection. The socket should be TCP type and
 * be connected to it's peer.</param>
 * <param name="hTlsCtx" type="IN">Handle to the TLS configuration context. Create by TlsCreateContext or TlsCreateContext2</param>
 * <param name="pRxBuffer" type="IN">Pointer to the buffer containing the data received from the peer.</param>
 * <param name="pTxBuffer" type="IN">Pointer to the buffer containing the data to be sent to the peer.</param>
 * <param name="pResult" type="OUT">Result of this operation. Possible values:
 *		- ERR_OK: The TLS connection was created successful.
 *		- ERR_NOMEMORY: The TLS connection could not be created. No memory available
 *		- ERR_INVALID_HANDLE: The TLS connection could not be created. The given context handle is invalid.
 *      - ERR_PARAMETER: Some parameters where inconsistent or wrong.</param>
 * <result>The handle to the created TLS connection, or RTS_INVALID_HANDLE, if operation failed.</result>
 */
RTS_HANDLE CDECL TlsBufferOpen(RTS_HANDLE hContext, RtsByteString* pRxBuffer, RtsByteString* pTxBuffer, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFTLSBUFFEROPEN) (RTS_HANDLE hContext, RtsByteString* pRxBuffer, RtsByteString* pTxBuffer, RTS_RESULT* pResult);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSBUFFEROPEN_NOTIMPLEMENTED)
	#define USE_TlsBufferOpen
	#define EXT_TlsBufferOpen
	#define GET_TlsBufferOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsBufferOpen(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_TlsBufferOpen  FALSE
	#define EXP_TlsBufferOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsBufferOpen
	#define EXT_TlsBufferOpen
	#define GET_TlsBufferOpen(fl)  CAL_CMGETAPI( "TlsBufferOpen" ) 
	#define CAL_TlsBufferOpen  TlsBufferOpen
	#define CHK_TlsBufferOpen  TRUE
	#define EXP_TlsBufferOpen  CAL_CMEXPAPI( "TlsBufferOpen" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsBufferOpen
	#define EXT_TlsBufferOpen
	#define GET_TlsBufferOpen(fl)  CAL_CMGETAPI( "TlsBufferOpen" ) 
	#define CAL_TlsBufferOpen  TlsBufferOpen
	#define CHK_TlsBufferOpen  TRUE
	#define EXP_TlsBufferOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsBufferOpen", (RTS_UINTPTR)TlsBufferOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsBufferOpen
	#define EXT_CmpTlsTlsBufferOpen
	#define GET_CmpTlsTlsBufferOpen  ERR_OK
	#define CAL_CmpTlsTlsBufferOpen pICmpTls->ITlsBufferOpen
	#define CHK_CmpTlsTlsBufferOpen (pICmpTls != NULL)
	#define EXP_CmpTlsTlsBufferOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsBufferOpen
	#define EXT_TlsBufferOpen
	#define GET_TlsBufferOpen(fl)  CAL_CMGETAPI( "TlsBufferOpen" ) 
	#define CAL_TlsBufferOpen pICmpTls->ITlsBufferOpen
	#define CHK_TlsBufferOpen (pICmpTls != NULL)
	#define EXP_TlsBufferOpen  CAL_CMEXPAPI( "TlsBufferOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsBufferOpen  PFTLSBUFFEROPEN pfTlsBufferOpen;
	#define EXT_TlsBufferOpen  extern PFTLSBUFFEROPEN pfTlsBufferOpen;
	#define GET_TlsBufferOpen(fl)  s_pfCMGetAPI2( "TlsBufferOpen", (RTS_VOID_FCTPTR *)&pfTlsBufferOpen, (fl), 0, 0)
	#define CAL_TlsBufferOpen  pfTlsBufferOpen
	#define CHK_TlsBufferOpen  (pfTlsBufferOpen != NULL)
	#define EXP_TlsBufferOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsBufferOpen", (RTS_UINTPTR)TlsBufferOpen, 0, 0) 
#endif




/**
 * <description>This function is used to indicate that the Rx buffer contains new TLS data. Call this function is new data have arrived. All data of the 
 * buffer has to be consumed by the TLS implementation before overwriting the existing data and calling this function. Call TlsRead until the corresponding 
 * error code is returned. If not all data is consumed the TLS stream will be broken and the data can't be decrypted anymore. The Rx buffer will not be changed.
 * </description>
 * <param name="hTlsConn" type="IN">Handle to the TLS connection. Must have been created with TlsBufferOpen</param>
 */
RTS_RESULT CDECL TlsBufferDataReveived(RTS_HANDLE hTlsConn);
typedef RTS_RESULT (CDECL * PFTLSBUFFERDATAREVEIVED) (RTS_HANDLE hTlsConn);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSBUFFERDATAREVEIVED_NOTIMPLEMENTED)
	#define USE_TlsBufferDataReveived
	#define EXT_TlsBufferDataReveived
	#define GET_TlsBufferDataReveived(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsBufferDataReveived(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_TlsBufferDataReveived  FALSE
	#define EXP_TlsBufferDataReveived  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsBufferDataReveived
	#define EXT_TlsBufferDataReveived
	#define GET_TlsBufferDataReveived(fl)  CAL_CMGETAPI( "TlsBufferDataReveived" ) 
	#define CAL_TlsBufferDataReveived  TlsBufferDataReveived
	#define CHK_TlsBufferDataReveived  TRUE
	#define EXP_TlsBufferDataReveived  CAL_CMEXPAPI( "TlsBufferDataReveived" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsBufferDataReveived
	#define EXT_TlsBufferDataReveived
	#define GET_TlsBufferDataReveived(fl)  CAL_CMGETAPI( "TlsBufferDataReveived" ) 
	#define CAL_TlsBufferDataReveived  TlsBufferDataReveived
	#define CHK_TlsBufferDataReveived  TRUE
	#define EXP_TlsBufferDataReveived  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsBufferDataReveived", (RTS_UINTPTR)TlsBufferDataReveived, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsBufferDataReveived
	#define EXT_CmpTlsTlsBufferDataReveived
	#define GET_CmpTlsTlsBufferDataReveived  ERR_OK
	#define CAL_CmpTlsTlsBufferDataReveived pICmpTls->ITlsBufferDataReveived
	#define CHK_CmpTlsTlsBufferDataReveived (pICmpTls != NULL)
	#define EXP_CmpTlsTlsBufferDataReveived  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsBufferDataReveived
	#define EXT_TlsBufferDataReveived
	#define GET_TlsBufferDataReveived(fl)  CAL_CMGETAPI( "TlsBufferDataReveived" ) 
	#define CAL_TlsBufferDataReveived pICmpTls->ITlsBufferDataReveived
	#define CHK_TlsBufferDataReveived (pICmpTls != NULL)
	#define EXP_TlsBufferDataReveived  CAL_CMEXPAPI( "TlsBufferDataReveived" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsBufferDataReveived  PFTLSBUFFERDATAREVEIVED pfTlsBufferDataReveived;
	#define EXT_TlsBufferDataReveived  extern PFTLSBUFFERDATAREVEIVED pfTlsBufferDataReveived;
	#define GET_TlsBufferDataReveived(fl)  s_pfCMGetAPI2( "TlsBufferDataReveived", (RTS_VOID_FCTPTR *)&pfTlsBufferDataReveived, (fl), 0, 0)
	#define CAL_TlsBufferDataReveived  pfTlsBufferDataReveived
	#define CHK_TlsBufferDataReveived  (pfTlsBufferDataReveived != NULL)
	#define EXP_TlsBufferDataReveived  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsBufferDataReveived", (RTS_UINTPTR)TlsBufferDataReveived, 0, 0) 
#endif




/**
 * <description>This function is used to indicate that the data written by the TLS implementation has been sent to the peer. If this function
 * is called the TLS implementation will start writing it's output at the beginning of the Tx buffer again. Call this function if the data of 
 * the Tx buffer has been sent. The length parameter of the Tx buffer will be set to 0.</description>
 * <param name="hTlsConn" type="IN">Handle to the TLS connection. Must have been created with TlsBufferOpen</param>
 */
RTS_RESULT CDECL TlsBufferDataSent(RTS_HANDLE hTlsConn);
typedef RTS_RESULT (CDECL * PFTLSBUFFERDATASENT) (RTS_HANDLE hTlsConn);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSBUFFERDATASENT_NOTIMPLEMENTED)
	#define USE_TlsBufferDataSent
	#define EXT_TlsBufferDataSent
	#define GET_TlsBufferDataSent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsBufferDataSent(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_TlsBufferDataSent  FALSE
	#define EXP_TlsBufferDataSent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsBufferDataSent
	#define EXT_TlsBufferDataSent
	#define GET_TlsBufferDataSent(fl)  CAL_CMGETAPI( "TlsBufferDataSent" ) 
	#define CAL_TlsBufferDataSent  TlsBufferDataSent
	#define CHK_TlsBufferDataSent  TRUE
	#define EXP_TlsBufferDataSent  CAL_CMEXPAPI( "TlsBufferDataSent" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsBufferDataSent
	#define EXT_TlsBufferDataSent
	#define GET_TlsBufferDataSent(fl)  CAL_CMGETAPI( "TlsBufferDataSent" ) 
	#define CAL_TlsBufferDataSent  TlsBufferDataSent
	#define CHK_TlsBufferDataSent  TRUE
	#define EXP_TlsBufferDataSent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsBufferDataSent", (RTS_UINTPTR)TlsBufferDataSent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsBufferDataSent
	#define EXT_CmpTlsTlsBufferDataSent
	#define GET_CmpTlsTlsBufferDataSent  ERR_OK
	#define CAL_CmpTlsTlsBufferDataSent pICmpTls->ITlsBufferDataSent
	#define CHK_CmpTlsTlsBufferDataSent (pICmpTls != NULL)
	#define EXP_CmpTlsTlsBufferDataSent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsBufferDataSent
	#define EXT_TlsBufferDataSent
	#define GET_TlsBufferDataSent(fl)  CAL_CMGETAPI( "TlsBufferDataSent" ) 
	#define CAL_TlsBufferDataSent pICmpTls->ITlsBufferDataSent
	#define CHK_TlsBufferDataSent (pICmpTls != NULL)
	#define EXP_TlsBufferDataSent  CAL_CMEXPAPI( "TlsBufferDataSent" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsBufferDataSent  PFTLSBUFFERDATASENT pfTlsBufferDataSent;
	#define EXT_TlsBufferDataSent  extern PFTLSBUFFERDATASENT pfTlsBufferDataSent;
	#define GET_TlsBufferDataSent(fl)  s_pfCMGetAPI2( "TlsBufferDataSent", (RTS_VOID_FCTPTR *)&pfTlsBufferDataSent, (fl), 0, 0)
	#define CAL_TlsBufferDataSent  pfTlsBufferDataSent
	#define CHK_TlsBufferDataSent  (pfTlsBufferDataSent != NULL)
	#define EXP_TlsBufferDataSent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsBufferDataSent", (RTS_UINTPTR)TlsBufferDataSent, 0, 0) 
#endif




/**
 * <description>This function is called to determine if the tx buffer contains data which should be sent to the peer.</description>
 * <param name="hTlsConn" type="IN">Handle to the TLS connection. Must have been created with TlsBufferOpen</param>
 */
RTS_BOOL CDECL TlsBufferDataToSendAvailable(RTS_HANDLE hTlsConn);
typedef RTS_BOOL (CDECL * PFTLSBUFFERDATATOSENDAVAILABLE) (RTS_HANDLE hTlsConn);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSBUFFERDATATOSENDAVAILABLE_NOTIMPLEMENTED)
	#define USE_TlsBufferDataToSendAvailable
	#define EXT_TlsBufferDataToSendAvailable
	#define GET_TlsBufferDataToSendAvailable(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsBufferDataToSendAvailable(p0)  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_TlsBufferDataToSendAvailable  FALSE
	#define EXP_TlsBufferDataToSendAvailable  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsBufferDataToSendAvailable
	#define EXT_TlsBufferDataToSendAvailable
	#define GET_TlsBufferDataToSendAvailable(fl)  CAL_CMGETAPI( "TlsBufferDataToSendAvailable" ) 
	#define CAL_TlsBufferDataToSendAvailable  TlsBufferDataToSendAvailable
	#define CHK_TlsBufferDataToSendAvailable  TRUE
	#define EXP_TlsBufferDataToSendAvailable  CAL_CMEXPAPI( "TlsBufferDataToSendAvailable" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsBufferDataToSendAvailable
	#define EXT_TlsBufferDataToSendAvailable
	#define GET_TlsBufferDataToSendAvailable(fl)  CAL_CMGETAPI( "TlsBufferDataToSendAvailable" ) 
	#define CAL_TlsBufferDataToSendAvailable  TlsBufferDataToSendAvailable
	#define CHK_TlsBufferDataToSendAvailable  TRUE
	#define EXP_TlsBufferDataToSendAvailable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsBufferDataToSendAvailable", (RTS_UINTPTR)TlsBufferDataToSendAvailable, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsBufferDataToSendAvailable
	#define EXT_CmpTlsTlsBufferDataToSendAvailable
	#define GET_CmpTlsTlsBufferDataToSendAvailable  ERR_OK
	#define CAL_CmpTlsTlsBufferDataToSendAvailable pICmpTls->ITlsBufferDataToSendAvailable
	#define CHK_CmpTlsTlsBufferDataToSendAvailable (pICmpTls != NULL)
	#define EXP_CmpTlsTlsBufferDataToSendAvailable  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsBufferDataToSendAvailable
	#define EXT_TlsBufferDataToSendAvailable
	#define GET_TlsBufferDataToSendAvailable(fl)  CAL_CMGETAPI( "TlsBufferDataToSendAvailable" ) 
	#define CAL_TlsBufferDataToSendAvailable pICmpTls->ITlsBufferDataToSendAvailable
	#define CHK_TlsBufferDataToSendAvailable (pICmpTls != NULL)
	#define EXP_TlsBufferDataToSendAvailable  CAL_CMEXPAPI( "TlsBufferDataToSendAvailable" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsBufferDataToSendAvailable  PFTLSBUFFERDATATOSENDAVAILABLE pfTlsBufferDataToSendAvailable;
	#define EXT_TlsBufferDataToSendAvailable  extern PFTLSBUFFERDATATOSENDAVAILABLE pfTlsBufferDataToSendAvailable;
	#define GET_TlsBufferDataToSendAvailable(fl)  s_pfCMGetAPI2( "TlsBufferDataToSendAvailable", (RTS_VOID_FCTPTR *)&pfTlsBufferDataToSendAvailable, (fl), 0, 0)
	#define CAL_TlsBufferDataToSendAvailable  pfTlsBufferDataToSendAvailable
	#define CHK_TlsBufferDataToSendAvailable  (pfTlsBufferDataToSendAvailable != NULL)
	#define EXP_TlsBufferDataToSendAvailable  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsBufferDataToSendAvailable", (RTS_UINTPTR)TlsBufferDataToSendAvailable, 0, 0) 
#endif




/**
 * <description>This function returns the number of written TLS packets since the last call of TlsBufferDataSent.</description>
 * <param name="hTlsConn" type="IN">Handle to the TLS connection. Must have been created with TlsBufferOpen</param>
 */
RTS_UI32 CDECL TlsBufferNumOfWrittenBlocks(RTS_HANDLE hTlsConn);
typedef RTS_UI32 (CDECL * PFTLSBUFFERNUMOFWRITTENBLOCKS) (RTS_HANDLE hTlsConn);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSBUFFERNUMOFWRITTENBLOCKS_NOTIMPLEMENTED)
	#define USE_TlsBufferNumOfWrittenBlocks
	#define EXT_TlsBufferNumOfWrittenBlocks
	#define GET_TlsBufferNumOfWrittenBlocks(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsBufferNumOfWrittenBlocks(p0)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_TlsBufferNumOfWrittenBlocks  FALSE
	#define EXP_TlsBufferNumOfWrittenBlocks  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsBufferNumOfWrittenBlocks
	#define EXT_TlsBufferNumOfWrittenBlocks
	#define GET_TlsBufferNumOfWrittenBlocks(fl)  CAL_CMGETAPI( "TlsBufferNumOfWrittenBlocks" ) 
	#define CAL_TlsBufferNumOfWrittenBlocks  TlsBufferNumOfWrittenBlocks
	#define CHK_TlsBufferNumOfWrittenBlocks  TRUE
	#define EXP_TlsBufferNumOfWrittenBlocks  CAL_CMEXPAPI( "TlsBufferNumOfWrittenBlocks" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsBufferNumOfWrittenBlocks
	#define EXT_TlsBufferNumOfWrittenBlocks
	#define GET_TlsBufferNumOfWrittenBlocks(fl)  CAL_CMGETAPI( "TlsBufferNumOfWrittenBlocks" ) 
	#define CAL_TlsBufferNumOfWrittenBlocks  TlsBufferNumOfWrittenBlocks
	#define CHK_TlsBufferNumOfWrittenBlocks  TRUE
	#define EXP_TlsBufferNumOfWrittenBlocks  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsBufferNumOfWrittenBlocks", (RTS_UINTPTR)TlsBufferNumOfWrittenBlocks, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsBufferNumOfWrittenBlocks
	#define EXT_CmpTlsTlsBufferNumOfWrittenBlocks
	#define GET_CmpTlsTlsBufferNumOfWrittenBlocks  ERR_OK
	#define CAL_CmpTlsTlsBufferNumOfWrittenBlocks pICmpTls->ITlsBufferNumOfWrittenBlocks
	#define CHK_CmpTlsTlsBufferNumOfWrittenBlocks (pICmpTls != NULL)
	#define EXP_CmpTlsTlsBufferNumOfWrittenBlocks  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsBufferNumOfWrittenBlocks
	#define EXT_TlsBufferNumOfWrittenBlocks
	#define GET_TlsBufferNumOfWrittenBlocks(fl)  CAL_CMGETAPI( "TlsBufferNumOfWrittenBlocks" ) 
	#define CAL_TlsBufferNumOfWrittenBlocks pICmpTls->ITlsBufferNumOfWrittenBlocks
	#define CHK_TlsBufferNumOfWrittenBlocks (pICmpTls != NULL)
	#define EXP_TlsBufferNumOfWrittenBlocks  CAL_CMEXPAPI( "TlsBufferNumOfWrittenBlocks" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsBufferNumOfWrittenBlocks  PFTLSBUFFERNUMOFWRITTENBLOCKS pfTlsBufferNumOfWrittenBlocks;
	#define EXT_TlsBufferNumOfWrittenBlocks  extern PFTLSBUFFERNUMOFWRITTENBLOCKS pfTlsBufferNumOfWrittenBlocks;
	#define GET_TlsBufferNumOfWrittenBlocks(fl)  s_pfCMGetAPI2( "TlsBufferNumOfWrittenBlocks", (RTS_VOID_FCTPTR *)&pfTlsBufferNumOfWrittenBlocks, (fl), 0, 0)
	#define CAL_TlsBufferNumOfWrittenBlocks  pfTlsBufferNumOfWrittenBlocks
	#define CHK_TlsBufferNumOfWrittenBlocks  (pfTlsBufferNumOfWrittenBlocks != NULL)
	#define EXP_TlsBufferNumOfWrittenBlocks  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsBufferNumOfWrittenBlocks", (RTS_UINTPTR)TlsBufferNumOfWrittenBlocks, 0, 0) 
#endif





/**
 * <description>Starts handshaking on the server side. The client has to initiate the handshaking.
 * This is the first function call on the server side after establishing the underlaying connection.
 * Data has to be received first. This function will is likely to read and write data.</description>
 * <param name="hTlsConn" type="IN">Handle of the TLS connection, created with one of the TLSxxxOpen functions</param>
 * <result>Result of this operation. May be one of these values:
 *		- ERR_OK: No error. Everything worked as expected.
 *		- ERR_NOTINITIALIZED: The TLS component was not initialized properly.
 *		- ERR_TLS_CONNECTION_CLOSED: The TLS connection is closed. Use TlsShutdown to shutdown locally.
 *      - ERR_TLS_WANT_READ: The operation needs to read data from the IO system. There is no data available.
 *      - ERR_TLS_WANT_WRITE: The operation needs to write data to the IO system. This is not possible.
 *		- ERR_TLS_RETRY_OPERATION: Something went wrong. Try the same function call again
 *		- ERR_TLS_IO_SYSTEM: An error in the underlaying IO system.
 *		- ERR_TLS_INTERNAL: An internal Error of the TLS Component
 *		- ERR_FAILED: Unknown error.</result>
 */
RTS_RESULT CDECL TlsAccept(RTS_HANDLE hTlsConn);
typedef RTS_RESULT (CDECL * PFTLSACCEPT) (RTS_HANDLE hTlsConn);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSACCEPT_NOTIMPLEMENTED)
	#define USE_TlsAccept
	#define EXT_TlsAccept
	#define GET_TlsAccept(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsAccept(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_TlsAccept  FALSE
	#define EXP_TlsAccept  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsAccept
	#define EXT_TlsAccept
	#define GET_TlsAccept(fl)  CAL_CMGETAPI( "TlsAccept" ) 
	#define CAL_TlsAccept  TlsAccept
	#define CHK_TlsAccept  TRUE
	#define EXP_TlsAccept  CAL_CMEXPAPI( "TlsAccept" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsAccept
	#define EXT_TlsAccept
	#define GET_TlsAccept(fl)  CAL_CMGETAPI( "TlsAccept" ) 
	#define CAL_TlsAccept  TlsAccept
	#define CHK_TlsAccept  TRUE
	#define EXP_TlsAccept  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsAccept", (RTS_UINTPTR)TlsAccept, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsAccept
	#define EXT_CmpTlsTlsAccept
	#define GET_CmpTlsTlsAccept  ERR_OK
	#define CAL_CmpTlsTlsAccept pICmpTls->ITlsAccept
	#define CHK_CmpTlsTlsAccept (pICmpTls != NULL)
	#define EXP_CmpTlsTlsAccept  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsAccept
	#define EXT_TlsAccept
	#define GET_TlsAccept(fl)  CAL_CMGETAPI( "TlsAccept" ) 
	#define CAL_TlsAccept pICmpTls->ITlsAccept
	#define CHK_TlsAccept (pICmpTls != NULL)
	#define EXP_TlsAccept  CAL_CMEXPAPI( "TlsAccept" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsAccept  PFTLSACCEPT pfTlsAccept;
	#define EXT_TlsAccept  extern PFTLSACCEPT pfTlsAccept;
	#define GET_TlsAccept(fl)  s_pfCMGetAPI2( "TlsAccept", (RTS_VOID_FCTPTR *)&pfTlsAccept, (fl), 0, 0)
	#define CAL_TlsAccept  pfTlsAccept
	#define CHK_TlsAccept  (pfTlsAccept != NULL)
	#define EXP_TlsAccept  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsAccept", (RTS_UINTPTR)TlsAccept, 0, 0) 
#endif




/**
 * <description>Starts handshaking on the client side. This data is likely to read and write data</description>
 * <param name="hTlsConn" type="IN">Handle of the TLS connection, created with one of the TLSxxxOpen functions</param>
 * <result>Result of this operation. May be one of these values:
 *		- ERR_OK: No error. Everything worked as expected.
 *		- ERR_NOTINITIALIZED: The TLS component was not initialized properly.
 *		- ERR_TLS_CONNECTION_CLOSED: The TLS connection is closed. Use TlsShutdown to shutdown locally.
 *      - ERR_TLS_WANT_READ: The operation needs to read data from the IO system. There is no data available.
 *      - ERR_TLS_WANT_WRITE: The operation needs to write data to the IO system. This is not possible.
 *		- ERR_TLS_RETRY_OPERATION: Something went wrong. Try the same function call again
 *		- ERR_TLS_IO_SYSTEM: An error in the underlaying IO system.
 *		- ERR_TLS_INTERNAL: An internal Error of the TLS Component
 *		- ERR_FAILED: Unknown error.</result>
 */
RTS_RESULT CDECL TlsConnect(RTS_HANDLE hTlsConn);
typedef RTS_RESULT (CDECL * PFTLSCONNECT) (RTS_HANDLE hTlsConn);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSCONNECT_NOTIMPLEMENTED)
	#define USE_TlsConnect
	#define EXT_TlsConnect
	#define GET_TlsConnect(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsConnect(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_TlsConnect  FALSE
	#define EXP_TlsConnect  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsConnect
	#define EXT_TlsConnect
	#define GET_TlsConnect(fl)  CAL_CMGETAPI( "TlsConnect" ) 
	#define CAL_TlsConnect  TlsConnect
	#define CHK_TlsConnect  TRUE
	#define EXP_TlsConnect  CAL_CMEXPAPI( "TlsConnect" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsConnect
	#define EXT_TlsConnect
	#define GET_TlsConnect(fl)  CAL_CMGETAPI( "TlsConnect" ) 
	#define CAL_TlsConnect  TlsConnect
	#define CHK_TlsConnect  TRUE
	#define EXP_TlsConnect  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsConnect", (RTS_UINTPTR)TlsConnect, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsConnect
	#define EXT_CmpTlsTlsConnect
	#define GET_CmpTlsTlsConnect  ERR_OK
	#define CAL_CmpTlsTlsConnect pICmpTls->ITlsConnect
	#define CHK_CmpTlsTlsConnect (pICmpTls != NULL)
	#define EXP_CmpTlsTlsConnect  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsConnect
	#define EXT_TlsConnect
	#define GET_TlsConnect(fl)  CAL_CMGETAPI( "TlsConnect" ) 
	#define CAL_TlsConnect pICmpTls->ITlsConnect
	#define CHK_TlsConnect (pICmpTls != NULL)
	#define EXP_TlsConnect  CAL_CMEXPAPI( "TlsConnect" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsConnect  PFTLSCONNECT pfTlsConnect;
	#define EXT_TlsConnect  extern PFTLSCONNECT pfTlsConnect;
	#define GET_TlsConnect(fl)  s_pfCMGetAPI2( "TlsConnect", (RTS_VOID_FCTPTR *)&pfTlsConnect, (fl), 0, 0)
	#define CAL_TlsConnect  pfTlsConnect
	#define CHK_TlsConnect  (pfTlsConnect != NULL)
	#define EXP_TlsConnect  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsConnect", (RTS_UINTPTR)TlsConnect, 0, 0) 
#endif


 

/**
 * <description>Receive data from the encrypted channel. If the connection is not established yet, this will be done
 * transparent in the background. Normally only data is of the encrypted channel is consumed. In some cases (Handshaking has
 * not finished or renegotiation of the session keys) this function will also write data. If the underlaying IO doesn't write
 * automatically the need to send data will be indicated using the corresponding error code.</description>
 * <param name="hTlsConn" type="IN">Handle of the TLS connection, created with one of the TLSxxxOpen functions</param>
 * <param name="pbyData" type="IN">Pointer to a data buffer, where the received data should be written to.</param>
 * <param name="lDataSize" type="IN">Size of pbyData. Maximum number of bytes, that could be received from the TLS connection.</param>
 * <param name="pResult" type="OUT">Result of this operation. May be one of these values:
 *		- ERR_OK: No error. Everything worked as expected.
 *		- ERR_NOTINITIALIZED: The TLS component was not initialized properly.
 *		- ERR_TLS_CONNECTION_CLOSED: The TLS connection is closed. Use TlsShutdown to shutdown locally.
 *      - ERR_TLS_WANT_READ: The operation needs to read data from the IO system. There is no data available.
 *      - ERR_TLS_WANT_WRITE: The operation needs to write data to the IO system. This is not possible. 
 *		- ERR_TLS_RETRY_OPERATION: Something went wrong. Try the same function call again
 *		- ERR_TLS_IO_SYSTEM: An error in the underlaying IO system.
 *		- ERR_TLS_INTERNAL: An internal Error of the TLS Component
 *		- ERR_FAILED: Unknown error.</param>
 * <result>Number of bytes received. 0 if an error occurred.</result>
 */
RTS_SSIZE CDECL TlsRead(RTS_HANDLE hTlsConn, RTS_UI8* pbyBuffer, RTS_SSIZE ulBufferSize, RTS_RESULT* pResult);
typedef RTS_SSIZE (CDECL * PFTLSREAD) (RTS_HANDLE hTlsConn, RTS_UI8* pbyBuffer, RTS_SSIZE ulBufferSize, RTS_RESULT* pResult);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSREAD_NOTIMPLEMENTED)
	#define USE_TlsRead
	#define EXT_TlsRead
	#define GET_TlsRead(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsRead(p0,p1,p2,p3)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_TlsRead  FALSE
	#define EXP_TlsRead  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsRead
	#define EXT_TlsRead
	#define GET_TlsRead(fl)  CAL_CMGETAPI( "TlsRead" ) 
	#define CAL_TlsRead  TlsRead
	#define CHK_TlsRead  TRUE
	#define EXP_TlsRead  CAL_CMEXPAPI( "TlsRead" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsRead
	#define EXT_TlsRead
	#define GET_TlsRead(fl)  CAL_CMGETAPI( "TlsRead" ) 
	#define CAL_TlsRead  TlsRead
	#define CHK_TlsRead  TRUE
	#define EXP_TlsRead  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsRead", (RTS_UINTPTR)TlsRead, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsRead
	#define EXT_CmpTlsTlsRead
	#define GET_CmpTlsTlsRead  ERR_OK
	#define CAL_CmpTlsTlsRead pICmpTls->ITlsRead
	#define CHK_CmpTlsTlsRead (pICmpTls != NULL)
	#define EXP_CmpTlsTlsRead  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsRead
	#define EXT_TlsRead
	#define GET_TlsRead(fl)  CAL_CMGETAPI( "TlsRead" ) 
	#define CAL_TlsRead pICmpTls->ITlsRead
	#define CHK_TlsRead (pICmpTls != NULL)
	#define EXP_TlsRead  CAL_CMEXPAPI( "TlsRead" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsRead  PFTLSREAD pfTlsRead;
	#define EXT_TlsRead  extern PFTLSREAD pfTlsRead;
	#define GET_TlsRead(fl)  s_pfCMGetAPI2( "TlsRead", (RTS_VOID_FCTPTR *)&pfTlsRead, (fl), 0, 0)
	#define CAL_TlsRead  pfTlsRead
	#define CHK_TlsRead  (pfTlsRead != NULL)
	#define EXP_TlsRead  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsRead", (RTS_UINTPTR)TlsRead, 0, 0) 
#endif



 
/**
 * <description>Send data over the encrypted channel. If the connection is not established yet, this will be done
 * transparent in the background. Normally only data is written to the underlaying IO. In some cases data have to be read
 * (Handshaking has not finished or renegotiation of the session keys). If not data is for reading is available this will
 * be indicated through the corresponding error code.</description>
 * <param name="hTlsConn" type="IN">Handle of the TLS connection, created with one of the TLSxxxOpen functions</param>
 * <param name="pbyData" type="IN">Pointer to data, that should be sent over the TLS connection</param>
 * <param name="lDataSize" type="IN">Size of pbyData. Number of bytes that should be sent over the TLS connection</param>
 * <param name="pResult" type="OUT">Result of this operation. May be one of these values:
 *		- ERR_OK: No error. Everything worked as expected.
 *		- ERR_NOTINITIALIZED: The TLS component was not initialized properly.
 *		- ERR_TLS_CONNECTION_CLOSED: The TLS connection is closed. Use TlsShutdown to shutdown locally.
 *      - ERR_TLS_WANT_READ: The operation needs to read data from the IO system. There is no data available.
 *      - ERR_TLS_WANT_WRITE: The operation needs to write data to the IO system. This is not possible. 
 *		- ERR_TLS_RETRY_OPERATION: Something went wrong. Try the same function call again
 *		- ERR_TLS_IO_SYSTEM: An error in the underlaying IO system.
 *		- ERR_TLS_INTERNAL: An internal Error of the TLS Component
 *		- ERR_FAILED: Unknown error.</param>
 * <result>Number of bytes sent. 0 if an error occurred.</result>
 */
RTS_SSIZE CDECL TlsWrite(RTS_HANDLE hTlsConn, RTS_UI8* pbyData, RTS_SSIZE ulDataSize, RTS_RESULT* pResult);
typedef RTS_SSIZE (CDECL * PFTLSWRITE) (RTS_HANDLE hTlsConn, RTS_UI8* pbyData, RTS_SSIZE ulDataSize, RTS_RESULT* pResult);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSWRITE_NOTIMPLEMENTED)
	#define USE_TlsWrite
	#define EXT_TlsWrite
	#define GET_TlsWrite(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsWrite(p0,p1,p2,p3)  (RTS_SSIZE)ERR_NOTIMPLEMENTED
	#define CHK_TlsWrite  FALSE
	#define EXP_TlsWrite  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsWrite
	#define EXT_TlsWrite
	#define GET_TlsWrite(fl)  CAL_CMGETAPI( "TlsWrite" ) 
	#define CAL_TlsWrite  TlsWrite
	#define CHK_TlsWrite  TRUE
	#define EXP_TlsWrite  CAL_CMEXPAPI( "TlsWrite" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsWrite
	#define EXT_TlsWrite
	#define GET_TlsWrite(fl)  CAL_CMGETAPI( "TlsWrite" ) 
	#define CAL_TlsWrite  TlsWrite
	#define CHK_TlsWrite  TRUE
	#define EXP_TlsWrite  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsWrite", (RTS_UINTPTR)TlsWrite, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsWrite
	#define EXT_CmpTlsTlsWrite
	#define GET_CmpTlsTlsWrite  ERR_OK
	#define CAL_CmpTlsTlsWrite pICmpTls->ITlsWrite
	#define CHK_CmpTlsTlsWrite (pICmpTls != NULL)
	#define EXP_CmpTlsTlsWrite  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsWrite
	#define EXT_TlsWrite
	#define GET_TlsWrite(fl)  CAL_CMGETAPI( "TlsWrite" ) 
	#define CAL_TlsWrite pICmpTls->ITlsWrite
	#define CHK_TlsWrite (pICmpTls != NULL)
	#define EXP_TlsWrite  CAL_CMEXPAPI( "TlsWrite" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsWrite  PFTLSWRITE pfTlsWrite;
	#define EXT_TlsWrite  extern PFTLSWRITE pfTlsWrite;
	#define GET_TlsWrite(fl)  s_pfCMGetAPI2( "TlsWrite", (RTS_VOID_FCTPTR *)&pfTlsWrite, (fl), 0, 0)
	#define CAL_TlsWrite  pfTlsWrite
	#define CHK_TlsWrite  (pfTlsWrite != NULL)
	#define EXP_TlsWrite  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsWrite", (RTS_UINTPTR)TlsWrite, 0, 0) 
#endif




/**
 * <description>Shutdown the TLS connection. There are two ways to shutdown the TLS connection:
 * bidirectional: The first call of TlsShutdown will send the shutdown message. The operation will return ERR_TLS_RETRY_OPERATION.
 *             Then the function has to be called again. If the acknowledge of the peer was received ERR_OK will be returned. If the
 *             acknowledge was not received yet the function will return ERR_TLS_WANT_READ.
 *  unidirectional: The first call of TlsShutdown will send the shutdown message. The operation will return ERR_TLS_RETRY_OPERATION.
               Close the IO channel and clean up. Don't wait for the acknowledge of the peer.
 * </description>
 * <param name="hTlsConn" type="IN">Handle of the TLS connection, created with one of the TLSxxxOpen functions</param>
 * <result>Result of this operation. May be one of these values:
 *		- ERR_OK: No error. Everything worked as expected.
 *		- ERR_NOTINITIALIZED: The TLS component was not initialized properly.
 *		- ERR_TLS_CONNECTION_CLOSED: The TLS connection is closed.
 *      - ERR_TLS_WANT_READ: The operation needs to read data from the IO system. There is no data available.
 *      - ERR_TLS_WANT_WRITE: The operation needs to write data to the IO system. This is not possible. 
 *		- ERR_TLS_RETRY_OPERATION: Something went wrong. Try the same function call again
 *		- ERR_TLS_IO_SYSTEM: An error in the underlaying IO system.
 *		- ERR_TLS_INTERNAL: An internal Error of the TLS Component
 *		- ERR_FAILED: Unknown error.</result>
 */
RTS_RESULT CDECL TlsShutdown(RTS_HANDLE hTlsConn);
typedef RTS_RESULT (CDECL * PFTLSSHUTDOWN) (RTS_HANDLE hTlsConn);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSSHUTDOWN_NOTIMPLEMENTED)
	#define USE_TlsShutdown
	#define EXT_TlsShutdown
	#define GET_TlsShutdown(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsShutdown(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_TlsShutdown  FALSE
	#define EXP_TlsShutdown  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsShutdown
	#define EXT_TlsShutdown
	#define GET_TlsShutdown(fl)  CAL_CMGETAPI( "TlsShutdown" ) 
	#define CAL_TlsShutdown  TlsShutdown
	#define CHK_TlsShutdown  TRUE
	#define EXP_TlsShutdown  CAL_CMEXPAPI( "TlsShutdown" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsShutdown
	#define EXT_TlsShutdown
	#define GET_TlsShutdown(fl)  CAL_CMGETAPI( "TlsShutdown" ) 
	#define CAL_TlsShutdown  TlsShutdown
	#define CHK_TlsShutdown  TRUE
	#define EXP_TlsShutdown  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsShutdown", (RTS_UINTPTR)TlsShutdown, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsShutdown
	#define EXT_CmpTlsTlsShutdown
	#define GET_CmpTlsTlsShutdown  ERR_OK
	#define CAL_CmpTlsTlsShutdown pICmpTls->ITlsShutdown
	#define CHK_CmpTlsTlsShutdown (pICmpTls != NULL)
	#define EXP_CmpTlsTlsShutdown  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsShutdown
	#define EXT_TlsShutdown
	#define GET_TlsShutdown(fl)  CAL_CMGETAPI( "TlsShutdown" ) 
	#define CAL_TlsShutdown pICmpTls->ITlsShutdown
	#define CHK_TlsShutdown (pICmpTls != NULL)
	#define EXP_TlsShutdown  CAL_CMEXPAPI( "TlsShutdown" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsShutdown  PFTLSSHUTDOWN pfTlsShutdown;
	#define EXT_TlsShutdown  extern PFTLSSHUTDOWN pfTlsShutdown;
	#define GET_TlsShutdown(fl)  s_pfCMGetAPI2( "TlsShutdown", (RTS_VOID_FCTPTR *)&pfTlsShutdown, (fl), 0, 0)
	#define CAL_TlsShutdown  pfTlsShutdown
	#define CHK_TlsShutdown  (pfTlsShutdown != NULL)
	#define EXP_TlsShutdown  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsShutdown", (RTS_UINTPTR)TlsShutdown, 0, 0) 
#endif




/**
 * <description>Close the TLS connection. All data will be cleaned up. The handle used for creating the connection will
 * be closed too. No shutdown is done here. This is allowed by the TLS protocol.</description>
 * <param name="hTlsConn" type="IN">Handle of the TLS connection, created with one of the TLSxxxOpen functions</param>
 * <result>Result of this operation. May be one of these values:
 *		- ERR_OK: No error. Everything worked as expected.
 *		- ERR_FAILED: Unknown error.</result>
 */
RTS_RESULT CDECL TlsClose(RTS_HANDLE hTlsConn);
typedef RTS_RESULT (CDECL * PFTLSCLOSE) (RTS_HANDLE hTlsConn);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSCLOSE_NOTIMPLEMENTED)
	#define USE_TlsClose
	#define EXT_TlsClose
	#define GET_TlsClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_TlsClose  FALSE
	#define EXP_TlsClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsClose
	#define EXT_TlsClose
	#define GET_TlsClose(fl)  CAL_CMGETAPI( "TlsClose" ) 
	#define CAL_TlsClose  TlsClose
	#define CHK_TlsClose  TRUE
	#define EXP_TlsClose  CAL_CMEXPAPI( "TlsClose" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsClose
	#define EXT_TlsClose
	#define GET_TlsClose(fl)  CAL_CMGETAPI( "TlsClose" ) 
	#define CAL_TlsClose  TlsClose
	#define CHK_TlsClose  TRUE
	#define EXP_TlsClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsClose", (RTS_UINTPTR)TlsClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsClose
	#define EXT_CmpTlsTlsClose
	#define GET_CmpTlsTlsClose  ERR_OK
	#define CAL_CmpTlsTlsClose pICmpTls->ITlsClose
	#define CHK_CmpTlsTlsClose (pICmpTls != NULL)
	#define EXP_CmpTlsTlsClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsClose
	#define EXT_TlsClose
	#define GET_TlsClose(fl)  CAL_CMGETAPI( "TlsClose" ) 
	#define CAL_TlsClose pICmpTls->ITlsClose
	#define CHK_TlsClose (pICmpTls != NULL)
	#define EXP_TlsClose  CAL_CMEXPAPI( "TlsClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsClose  PFTLSCLOSE pfTlsClose;
	#define EXT_TlsClose  extern PFTLSCLOSE pfTlsClose;
	#define GET_TlsClose(fl)  s_pfCMGetAPI2( "TlsClose", (RTS_VOID_FCTPTR *)&pfTlsClose, (fl), 0, 0)
	#define CAL_TlsClose  pfTlsClose
	#define CHK_TlsClose  (pfTlsClose != NULL)
	#define EXP_TlsClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsClose", (RTS_UINTPTR)TlsClose, 0, 0) 
#endif




/*
 * <description>Get the certificate of the peer connected to.</description>
 * <param name="hTlsConn" type="IN">Handle of the TLS connection, created with one of the TLSWrap functions</param>
 * <param name="pResult" type="OUT">Result of this operation.</param>
 * <result>A pointer to the certificate received from the peer. NULL, if no certificate could be received. 
 * Check pResult for more information</result>
 */

 /**
  * <description>Add entropy to the PRNG (Pseudo Random Number Generator)of the underlaying TLS implementation.</description>
  * <param name="pui8Buffer" type="IN">Pointer to the data containing the random value</param>
  * <param name="i32BufferSize" type="IN">Size of the input buffer</param>
  * <param name="r64Entropy" type="IN">An estimate of how much randomness is contained in pui8Buffer, in bytes,
  and should be a number between zero and i32BufferSize.
  "Randomness" is the term used for "entropy" in OpenSSL.
  Details about sources of randomness and how to estimate their randomness can be found in the literature; for example [NIST SP 800-90B].</param>
  * <result>TRUE: If the channel has be secured already. FALSE: If the channel has net been secured already, or is not a valid channelID</result>
  */
void CDECL TlsRandAdd(const RTS_UI8* pui8Buffer, RTS_I32 i32BufferSize, RTS_REAL64 r64Entropy);
typedef void (CDECL * PFTLSRANDADD) (const RTS_UI8* pui8Buffer, RTS_I32 i32BufferSize, RTS_REAL64 r64Entropy);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSRANDADD_NOTIMPLEMENTED)
	#define USE_TlsRandAdd
	#define EXT_TlsRandAdd
	#define GET_TlsRandAdd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsRandAdd(p0,p1,p2)  
	#define CHK_TlsRandAdd  FALSE
	#define EXP_TlsRandAdd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsRandAdd
	#define EXT_TlsRandAdd
	#define GET_TlsRandAdd(fl)  CAL_CMGETAPI( "TlsRandAdd" ) 
	#define CAL_TlsRandAdd  TlsRandAdd
	#define CHK_TlsRandAdd  TRUE
	#define EXP_TlsRandAdd  CAL_CMEXPAPI( "TlsRandAdd" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsRandAdd
	#define EXT_TlsRandAdd
	#define GET_TlsRandAdd(fl)  CAL_CMGETAPI( "TlsRandAdd" ) 
	#define CAL_TlsRandAdd  TlsRandAdd
	#define CHK_TlsRandAdd  TRUE
	#define EXP_TlsRandAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsRandAdd", (RTS_UINTPTR)TlsRandAdd, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsRandAdd
	#define EXT_CmpTlsTlsRandAdd
	#define GET_CmpTlsTlsRandAdd  ERR_OK
	#define CAL_CmpTlsTlsRandAdd pICmpTls->ITlsRandAdd
	#define CHK_CmpTlsTlsRandAdd (pICmpTls != NULL)
	#define EXP_CmpTlsTlsRandAdd  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsRandAdd
	#define EXT_TlsRandAdd
	#define GET_TlsRandAdd(fl)  CAL_CMGETAPI( "TlsRandAdd" ) 
	#define CAL_TlsRandAdd pICmpTls->ITlsRandAdd
	#define CHK_TlsRandAdd (pICmpTls != NULL)
	#define EXP_TlsRandAdd  CAL_CMEXPAPI( "TlsRandAdd" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsRandAdd  PFTLSRANDADD pfTlsRandAdd;
	#define EXT_TlsRandAdd  extern PFTLSRANDADD pfTlsRandAdd;
	#define GET_TlsRandAdd(fl)  s_pfCMGetAPI2( "TlsRandAdd", (RTS_VOID_FCTPTR *)&pfTlsRandAdd, (fl), 0, 0)
	#define CAL_TlsRandAdd  pfTlsRandAdd
	#define CHK_TlsRandAdd  (pfTlsRandAdd != NULL)
	#define EXP_TlsRandAdd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsRandAdd", (RTS_UINTPTR)TlsRandAdd, 0, 0) 
#endif




 /**
  * <description>Check if the PRNG has enough entropy to work properly</description>
  * <param name="ui32ChannelId" type="IN"></param>
  * <result>TRUE: If the channel has be secured already. FALSE: If the channel has net been secured already, or is not a valid channelID</result>
  */
RTS_BOOL CDECL TlsRandCheckStatus(void);
typedef RTS_BOOL (CDECL * PFTLSRANDCHECKSTATUS) (void);
#if defined(CMPTLS_NOTIMPLEMENTED) || defined(TLSRANDCHECKSTATUS_NOTIMPLEMENTED)
	#define USE_TlsRandCheckStatus
	#define EXT_TlsRandCheckStatus
	#define GET_TlsRandCheckStatus(fl)  ERR_NOTIMPLEMENTED
	#define CAL_TlsRandCheckStatus()  (RTS_BOOL)ERR_NOTIMPLEMENTED
	#define CHK_TlsRandCheckStatus  FALSE
	#define EXP_TlsRandCheckStatus  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_TlsRandCheckStatus
	#define EXT_TlsRandCheckStatus
	#define GET_TlsRandCheckStatus(fl)  CAL_CMGETAPI( "TlsRandCheckStatus" ) 
	#define CAL_TlsRandCheckStatus  TlsRandCheckStatus
	#define CHK_TlsRandCheckStatus  TRUE
	#define EXP_TlsRandCheckStatus  CAL_CMEXPAPI( "TlsRandCheckStatus" ) 
#elif defined(MIXED_LINK) && !defined(CMPTLS_EXTERNAL)
	#define USE_TlsRandCheckStatus
	#define EXT_TlsRandCheckStatus
	#define GET_TlsRandCheckStatus(fl)  CAL_CMGETAPI( "TlsRandCheckStatus" ) 
	#define CAL_TlsRandCheckStatus  TlsRandCheckStatus
	#define CHK_TlsRandCheckStatus  TRUE
	#define EXP_TlsRandCheckStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsRandCheckStatus", (RTS_UINTPTR)TlsRandCheckStatus, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpTlsTlsRandCheckStatus
	#define EXT_CmpTlsTlsRandCheckStatus
	#define GET_CmpTlsTlsRandCheckStatus  ERR_OK
	#define CAL_CmpTlsTlsRandCheckStatus pICmpTls->ITlsRandCheckStatus
	#define CHK_CmpTlsTlsRandCheckStatus (pICmpTls != NULL)
	#define EXP_CmpTlsTlsRandCheckStatus  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_TlsRandCheckStatus
	#define EXT_TlsRandCheckStatus
	#define GET_TlsRandCheckStatus(fl)  CAL_CMGETAPI( "TlsRandCheckStatus" ) 
	#define CAL_TlsRandCheckStatus pICmpTls->ITlsRandCheckStatus
	#define CHK_TlsRandCheckStatus (pICmpTls != NULL)
	#define EXP_TlsRandCheckStatus  CAL_CMEXPAPI( "TlsRandCheckStatus" ) 
#else /* DYNAMIC_LINK */
	#define USE_TlsRandCheckStatus  PFTLSRANDCHECKSTATUS pfTlsRandCheckStatus;
	#define EXT_TlsRandCheckStatus  extern PFTLSRANDCHECKSTATUS pfTlsRandCheckStatus;
	#define GET_TlsRandCheckStatus(fl)  s_pfCMGetAPI2( "TlsRandCheckStatus", (RTS_VOID_FCTPTR *)&pfTlsRandCheckStatus, (fl), 0, 0)
	#define CAL_TlsRandCheckStatus  pfTlsRandCheckStatus
	#define CHK_TlsRandCheckStatus  (pfTlsRandCheckStatus != NULL)
	#define EXP_TlsRandCheckStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"TlsRandCheckStatus", (RTS_UINTPTR)TlsRandCheckStatus, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif




typedef struct
{
	IBase_C *pBase;
	PFTLSCREATECONTEXT ITlsCreateContext;
 	PFTLSCREATECONTEXT2 ITlsCreateContext2;
 	PFTLSCREATECONTEXT3 ITlsCreateContext3;
 	PFTLSFREECONTEXT ITlsFreeContext;
 	PFTLSBUFFEROPEN ITlsBufferOpen;
 	PFTLSBUFFERDATAREVEIVED ITlsBufferDataReveived;
 	PFTLSBUFFERDATASENT ITlsBufferDataSent;
 	PFTLSBUFFERDATATOSENDAVAILABLE ITlsBufferDataToSendAvailable;
 	PFTLSBUFFERNUMOFWRITTENBLOCKS ITlsBufferNumOfWrittenBlocks;
 	PFTLSACCEPT ITlsAccept;
 	PFTLSCONNECT ITlsConnect;
 	PFTLSREAD ITlsRead;
 	PFTLSWRITE ITlsWrite;
 	PFTLSSHUTDOWN ITlsShutdown;
 	PFTLSCLOSE ITlsClose;
 	PFTLSRANDADD ITlsRandAdd;
 	PFTLSRANDCHECKSTATUS ITlsRandCheckStatus;
 } ICmpTls_C;

#ifdef CPLUSPLUS
class ICmpTls : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ITlsCreateContext(char* pszNamespace, RTS_RESULT* pResult) =0;
		virtual RTS_HANDLE CDECL ITlsCreateContext2(RTS_HANDLE hCert, RTS_UI32 ui32TlsMethod, char* szCipherList, RTS_I32 i32VerifyMode, RTS_RESULT* pResult) =0;
		virtual RTS_HANDLE CDECL ITlsCreateContext3(RTS_HANDLE hCert, RTS_UI32 ui32TlsMethod, char* szCipherList, RTS_I32 i32VerifyMode, PFVERIFYCERTIFICATE pfVerifyCallback, void* pUserParameter, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL ITlsFreeContext(RTS_HANDLE hTlsContext) =0;
		virtual RTS_HANDLE CDECL ITlsBufferOpen(RTS_HANDLE hContext, RtsByteString* pRxBuffer, RtsByteString* pTxBuffer, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL ITlsBufferDataReveived(RTS_HANDLE hTlsConn) =0;
		virtual RTS_RESULT CDECL ITlsBufferDataSent(RTS_HANDLE hTlsConn) =0;
		virtual RTS_BOOL CDECL ITlsBufferDataToSendAvailable(RTS_HANDLE hTlsConn) =0;
		virtual RTS_UI32 CDECL ITlsBufferNumOfWrittenBlocks(RTS_HANDLE hTlsConn) =0;
		virtual RTS_RESULT CDECL ITlsAccept(RTS_HANDLE hTlsConn) =0;
		virtual RTS_RESULT CDECL ITlsConnect(RTS_HANDLE hTlsConn) =0;
		virtual RTS_SSIZE CDECL ITlsRead(RTS_HANDLE hTlsConn, RTS_UI8* pbyBuffer, RTS_SSIZE ulBufferSize, RTS_RESULT* pResult) =0;
		virtual RTS_SSIZE CDECL ITlsWrite(RTS_HANDLE hTlsConn, RTS_UI8* pbyData, RTS_SSIZE ulDataSize, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL ITlsShutdown(RTS_HANDLE hTlsConn) =0;
		virtual RTS_RESULT CDECL ITlsClose(RTS_HANDLE hTlsConn) =0;
		virtual void CDECL ITlsRandAdd(const RTS_UI8* pui8Buffer, RTS_I32 i32BufferSize, RTS_REAL64 r64Entropy) =0;
		virtual RTS_BOOL CDECL ITlsRandCheckStatus(void) =0;
};
	#ifndef ITF_CmpTls
		#define ITF_CmpTls static ICmpTls *pICmpTls = NULL;
	#endif
	#define EXTITF_CmpTls
#else	/*CPLUSPLUS*/
	typedef ICmpTls_C		ICmpTls;
	#ifndef ITF_CmpTls
		#define ITF_CmpTls
	#endif
	#define EXTITF_CmpTls
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPTLSITF_H_*/
