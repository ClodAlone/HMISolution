 /**
 * <interfacename>CmpCrypto</interfacename>
 * <description> 
 *	<p>This interface provides access to different cryptographic algorithms:</p>
 *	<ul>
 *		<li>Symmetric ciphers</li>
 *		<li>Asymmetric ciphers</li>
 *		<li>Message digest algorithms</li>
 *		<li>Hashed message authentication codes (HMAC)</li>
 *		<li>Signature algorithms</li>
 *		<li>Random number generation</li>
 *	</ul>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */

	
	
#ifndef _CMPCRYPTOITF_H_
#define _CMPCRYPTOITF_H_

#include "CmpStd.h"

 

 



#include "CmpCryptoProviderItf.h"

/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>
 * Enumeration: RtsCryptoID
 * </description>
 * <element name="RTSCRYPTOID_AES_128_CBC" type=OUT>AES 128 bit key in CBC mode</element>
 * <element name="RTSCRYPTOID_AES_128_CFB" type=OUT>AES 128 bit key in CFB mode</element>
 * <element name="RTSCRYPTOID_AES_256_CBC" type=OUT>AES 256 bit key in CBC mode</element>
 * <element name="RTSCRYPTOID_DES_CBC" type=OUT>DES 64 bit key in CBC mode</element>
 * <element name="RTSCRYPTOID_AES_256_CTR" type=OUT>AES 256 bit key in CTR mode</element>
 * <element name="RTSCRYPTOID_AES_128_CTR" type=OUT>AES 128 bit key in CTR mode</element>
 * <element name="RTSCRYPTOID_RSA" type=OUT>Plain RSA asymmetric encryption. Not recommended.</element>
 * <element name="RTSCRYPTOID_RSA_OAEP_PADDING" type=OUT>RSA asymmetric encryption with OAEP (SHA1) padding.</element>
 * <element name="RTSCRYPTOID_RSA_PKCS1_V15_PADDING" type=OUT>RSA asymmetric encryption with #PKCS1 v1.5 padding. Not recommended.</element>
 * <element name="RTSCRYPTOID_RSA_OAEP_SHA256_PADDING" type=OUT>RSA asymmetric encryption with OAEP (SHA-256) padding.</element>
 * <element name="RTSCRYPTOID_HASH_MD5" type=OUT>MD5 checksum. Not cryptographically safe.</element>
 * <element name="RTSCRYPTOID_HASH_SHA1" type=OUT>SHA1 checksum. Not cryptographically safe.</element>
 * <element name="RTSCRYPTOID_HASH_SHA224" type=OUT>SHA-224 checksum.</element>
 * <element name="RTSCRYPTOID_HASH_SHA256" type=OUT>SHA-256 checksum.</element>
 * <element name="RTSCRYPTOID_HASH_SHA384" type=OUT>SHA-384 checksum.</element>
 * <element name="RTSCRYPTOID_HASH_SHA512" type=OUT>SHA-512 checksum.</element>
 * <element name="RTSCRYPTOID_HMAC_MD5" type=OUT>HMAC based on MD5. Not cryptographically safe.</element>
 * <element name="RTSCRYPTOID_HMAC_SHA1" type=OUT>HMAC based on SHA1. Not cryptographically safe.</element>
 * <element name="RTSCRYPTOID_HMAC_SHA224" type=OUT>HMAC based on SHA-224.</element>
 * <element name="RTSCRYPTOID_HMAC_SHA256" type=OUT>HMAC based on SHA-256.</element>
 * <element name="RTSCRYPTOID_HMAC_SHA384" type=OUT>HMAC based on SHA-384.</element>
 * <element name="RTSCRYPTOID_HMAC_SHA512" type=OUT>HMAC based on SHA-512.</element>
 * <element name="RTSCRYPTOID_RSA_PKCS1_V15_SHA1" type=OUT>Asymmetric signature based on RSA, SHA1 and #PKCS1 v1.5 padding. Not recommended.</element>
 * <element name="RTSCRYPTOID_RSA_PKCS1_V15_SHA256" type=OUT>Asymmetric signature based on RSA, SHA-256 and #PKCS1 v1.5 padding. Not recommended.</element>
 * <element name="RTSCRYPTOID_RSA_OAEP_SHA1" type=OUT>Not supported.</element>
 * <element name="RTSCRYPTOID_RSA_OAEP_SHA256" type=OUT>Not supported.</element>
 * <element name="RTSCRYPTOID_RSA_PSS_SHA256" type=OUT>Asymmetric signature based on RSA, SHA-256 and PSS padding.</element>
 * <element name="RTSCRYPTOID_KDF_PSHA1" type=OUT>TLS key derivation function based on SHA1. Not recommeded for key generation based on passwords.</element>
 * <element name="RTSCRYPTOID_KDF_PSHA256" type=OUT>TLS key derivation function based on SHA-256. Not recommeded for key generation based on passwords.</element>
 * <element name="RTSCRYPTOID_KDF_SCRYPT" type=OUT>Scrypt key derivation function. Recommeded to expand passwords into cryptographic keys, or to hash passwords.</element>
 * <element name="RTSCRYPTOID_ECDH_X25519" type=OUT>X25519 elliptic curce key exchange protocol</element>
*/
#define RTSCRYPTOID_AES_128_CBC    RTS_IEC_UDINT_C(0x1001)
#define RTSCRYPTOID_AES_128_CFB    RTS_IEC_UDINT_C(0x1002)
#define RTSCRYPTOID_AES_256_CBC    RTS_IEC_UDINT_C(0x1003)
#define RTSCRYPTOID_DES_CBC    RTS_IEC_UDINT_C(0x1004)
#define RTSCRYPTOID_AES_256_CTR    RTS_IEC_UDINT_C(0x1005)
#define RTSCRYPTOID_AES_128_CTR    RTS_IEC_UDINT_C(0x1006)
#define RTSCRYPTOID_RSA    RTS_IEC_UDINT_C(0x2001)
#define RTSCRYPTOID_RSA_OAEP_PADDING    RTS_IEC_UDINT_C(0x2002)
#define RTSCRYPTOID_RSA_PKCS1_V15_PADDING    RTS_IEC_UDINT_C(0x2003)
#define RTSCRYPTOID_RSA_OAEP_SHA256_PADDING    RTS_IEC_UDINT_C(0x2004)
#define RTSCRYPTOID_HASH_MD5    RTS_IEC_UDINT_C(0x3001)
#define RTSCRYPTOID_HASH_SHA1    RTS_IEC_UDINT_C(0x3002)
#define RTSCRYPTOID_HASH_SHA224    RTS_IEC_UDINT_C(0x3003)
#define RTSCRYPTOID_HASH_SHA256    RTS_IEC_UDINT_C(0x3004)
#define RTSCRYPTOID_HASH_SHA384    RTS_IEC_UDINT_C(0x3005)
#define RTSCRYPTOID_HASH_SHA512    RTS_IEC_UDINT_C(0x3006)
#define RTSCRYPTOID_HMAC_MD5    RTS_IEC_UDINT_C(0x4001)
#define RTSCRYPTOID_HMAC_SHA1    RTS_IEC_UDINT_C(0x4002)
#define RTSCRYPTOID_HMAC_SHA224    RTS_IEC_UDINT_C(0x4003)
#define RTSCRYPTOID_HMAC_SHA256    RTS_IEC_UDINT_C(0x4004)
#define RTSCRYPTOID_HMAC_SHA384    RTS_IEC_UDINT_C(0x4005)
#define RTSCRYPTOID_HMAC_SHA512    RTS_IEC_UDINT_C(0x4006)
#define RTSCRYPTOID_RSA_PKCS1_V15_SHA1    RTS_IEC_UDINT_C(0x5001)
#define RTSCRYPTOID_RSA_PKCS1_V15_SHA256    RTS_IEC_UDINT_C(0x5002)
#define RTSCRYPTOID_RSA_OAEP_SHA1    RTS_IEC_UDINT_C(0x5003)
#define RTSCRYPTOID_RSA_OAEP_SHA256    RTS_IEC_UDINT_C(0x5004)
#define RTSCRYPTOID_RSA_PSS_SHA256    RTS_IEC_UDINT_C(0x5005)
#define RTSCRYPTOID_KDF_PSHA1    RTS_IEC_UDINT_C(0x6001)
#define RTSCRYPTOID_KDF_PSHA256    RTS_IEC_UDINT_C(0x6002)
#define RTSCRYPTOID_KDF_SCRYPT    RTS_IEC_UDINT_C(0x6003)
#define RTSCRYPTOID_ECDH_X25519    RTS_IEC_UDINT_C(0x7001)
/* Typed enum definition */
#define RTSCRYPTOID    RTS_IEC_UDINT

/**
 * <description>
 * Enumeration: RtsCryptoKeyType
 * </description>
 * <element name="KeyType_Key" type=OUT>The key is directly available. Stored in a RTS_BYTESTRING. Use this for symmetric keys.</element>
 * <element name="KeyType_CertHandle" type=OUT>The key is identified by a handle to the certificat. Only used for asymmetric keys.</element>
 * <element name="KeyType_CertContent" type=OUT>The key is identified by the certificate context. Not used.</element>
*/
#define KEYTYPE_KEY    RTS_IEC_DINT_C(0x0)
#define KEYTYPE_CERTHANDLE    RTS_IEC_DINT_C(0x1)
#define KEYTYPE_CERTCONTENT    RTS_IEC_DINT_C(0x2)
/* Typed enum definition */
#define RTSCRYPTOKEYTYPE    RTS_IEC_DINT

/**
 * <description>
 * Enumeration: RtsCryptoType
 * </description>
 * <element name="RTSCRYPTOTYPE_SYMMETRIC" type=OUT>Symmetric encryption algorithm</element>
 * <element name="RTSCRYPTOTYPE_ASYMMETRIC" type=OUT>Asymmetric encryption algorithm</element>
 * <element name="RTSCRYPTOTYPE_HASH" type=OUT>Hash algorithm</element>
 * <element name="RTSCRYPTOTYPE_HMAC" type=OUT>Hashed message authentication code algorithm</element>
 * <element name="RTSCRYPTOTYPE_SIGNATURE" type=OUT>Asymmetric signature algorithm.</element>
 * <element name="RTSCRYPTOTYPE_KEYDERIVE" type=OUT>Key derivation algorithm.</element>
 * <element name="RTSCRYPTOTYPE_KEYAGREEMENT" type=OUT>Key agreement algorithm.</element>
*/
#define RTSCRYPTOTYPE_SYMMETRIC    RTS_IEC_UDINT_C(0x1000)
#define RTSCRYPTOTYPE_ASYMMETRIC    RTS_IEC_UDINT_C(0x2000)
#define RTSCRYPTOTYPE_HASH    RTS_IEC_UDINT_C(0x3000)
#define RTSCRYPTOTYPE_HMAC    RTS_IEC_UDINT_C(0x4000)
#define RTSCRYPTOTYPE_SIGNATURE    RTS_IEC_UDINT_C(0x5000)
#define RTSCRYPTOTYPE_KEYDERIVE    RTS_IEC_UDINT_C(0x6000)
#define RTSCRYPTOTYPE_KEYAGREEMENT    RTS_IEC_UDINT_C(0x7000)
/* Typed enum definition */
#define RTSCRYPTOTYPE    RTS_IEC_UDINT

/**
 * <description>
 * This a representation of a byte string.
 * </description>
 * <element name="ui32Len" type=INOUT>Current length of data stored in the buffer.</element>
 * <element name="ui32MaxLen" type=INOUT>The allocated size of the buffer.</element>
 * <element name="pByData" type=INOUT>Pointer to buffer</element>
 * <element name="xDynamic" type=INOUT>Indicates if the data was allocated dynamically</element>
*/
typedef struct tagRtsByteString
{
	RTS_IEC_UDINT ui32Len;
	RTS_IEC_UDINT ui32MaxLen;
	RTS_IEC_BYTE *pByData;
	RTS_IEC_BOOL xDynamic;
} RtsByteString;

/**
 * <description>
 * Storage of a crypto key.
 * </description>
 * <element name="byteString" type=INOUT>Key is stored using the byte string.</element>
 * <element name="hCert" type=INOUT>Key is identified by this handle.</element>
*/
typedef union
{
	RtsByteString byteString;
	RTS_IEC_HANDLE hCert;
} RtsCryptoKeyStorage;

/**
 * <description>
 * A cryptographic key.
 * </description>
 * <element name="keyType" type=INOUT>The type of the key.</element>
 * <element name="key" type=INOUT>Information of the key. Actually only the KeyType_Key is supported.</element>
*/
typedef struct tagRtsCryptoKey
{
	RTS_IEC_DINT keyType;
	RtsCryptoKeyStorage key;
} RtsCryptoKey;

/**
 * <description>
 * Structure: RtsScryptParameter
 * </description>
 * <element name="iterations" type=INOUT>How many iterations are done. Affects CPU and RAM usage.</element>
 * <element name="blockSize" type=INOUT>The block size affects memory an CPU usage.</element>
 * <element name="parallelismFactor" type=INOUT>Setup the number of threads to run in parallel. Should be 1.</element>
*/
typedef struct tagRtsScryptParameter
{
	RTS_IEC_UDINT iterations;
	RTS_IEC_UDINT blockSize;
	RTS_IEC_UDINT parallelismFactor;
} RtsScryptParameter;

/**
 * <description>
 * Structure: RtsKdfParameter
 * </description>
 * <element name="scrypt" type=INOUT></element>
*/
typedef union
{
	RtsScryptParameter scrypt;
} RtsKdfParameter;

#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/

/**
 * <category>SecuritySettings</category>
 * <description>Editable security setting: Minumum asymmetric RSA key length
 * </description>
 */
#define CMPCRYPTO_KEY_INT_MINRSALEN					CMPSETTINGS_CATEGORY_SECURITY ".RSAMinKeyLen"
#define CMPCRYPTO_KEY_INT_MINRSALEN_DESC			"Mininmal key length for RSA keys."

#ifndef CMPCRYPTO_VALUE_MINRSALEN_DEFAULT
	#define CMPCRYPTO_VALUE_MINRSALEN_DEFAULT		2048
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Generation of entropy on linux devices is enabled by default and can be disabled with this setting
 *  Encrypted communication, generating keys and other security features may not work if this function is disabled.
 * </description>
 */
#define SYSCRYPTOKEY_BOOL_LINUX_GENERATEENTROPY					"Linux.GenerateEntropy"
#ifndef SYSCRYPTOKEY_BOOL_LINUX_GENERATEENTROPY_DEFAULT
	#define SYSCRYPTOKEY_BOOL_LINUX_GENERATEENTROPY_DEFAULT		1
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	The minimal amount of entropy can be configured with this setting. Default value will be set to 1024
 *	Depending on the kernel there can be different maximum poolsizes for entropy
 *  When entropy depletes under the minimal entropy specified by this setting entropy is added to ensure encrypted communication
 * </description>
 */
#define SYSCRYPTOKEY_INT_LINUX_MINENTROPY						"Linux.MinimalEntropy"
#ifndef SYSCRYPTOKEY_INT_LINUX_MINENTROPY_DEFAULT
	#define SYSCRYPTOKEY_INT_LINUX_MINENTROPY_DEFAULT			-1
#endif

/**
 * <description>Information of symmetric ciphers.</description>
 * <param name="ui32KeyLen">The length of the key id bytes.</param>
 * <param name="ui32BlockSize">Block size of the cipher in bytes.</param>
 * <param name="ui32InitVLen">Initialization vector size in bytes.</param>
 */
typedef struct
{
	RTS_UI32	ui32KeyLen;
	RTS_UI32	ui32BlockSize;
	RTS_UI32	ui32InitVLen;
}RtsSymmetricCipherInfo;

/**
 * <description>Information of message digests.</description>
 * <param name="ui32DigestLen">The length generated message digest in bytes..</param>
 */
typedef struct
{
	RTS_UI32	ui32DigestLen;
}RtsDigestInfo;


/**
 * <description>Information of a specific cryptographic algorithm.</description>
 * <param name="ui32CryptoType">Type of the algorithm. See: Crypto Types</param>
 * <param name="ui32CryptoId">ID of the algorithm.</param>
 * <param name="pszCryptoName">Name of the algorithm.</param>
 * <param name="info">algorithm specific information.</param>
 */
typedef struct
{
	RTS_UI32	ui32CryptoType;
	RTS_UI32	ui32CryptoId;
	const char*		pszCryptoName;
	union
	{
		RtsSymmetricCipherInfo symmetric;
		RtsDigestInfo digest;
		RtsDigestInfo HMAC;

	}info;
}RtsCryptoInfo;


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Get a handle to the algorithm using a specific ID</description>
 * <param name="ui32CryptoID" type="IN">ID of the algorithm</param>
 * <param name="pResult" type="IN">Error code: Result of the operation. Can be NULL.</param>
 * <result>Handle to the cryptographic algorithm</result>
 */
RTS_HANDLE CDECL CryptoGetAlgorithmById(RTS_UI32 ui32CryptoID, RTS_RESULT* pResult);
typedef RTS_HANDLE (CDECL * PFCRYPTOGETALGORITHMBYID) (RTS_UI32 ui32CryptoID, RTS_RESULT* pResult);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGETALGORITHMBYID_NOTIMPLEMENTED)
	#define USE_CryptoGetAlgorithmById
	#define EXT_CryptoGetAlgorithmById
	#define GET_CryptoGetAlgorithmById(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoGetAlgorithmById(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CryptoGetAlgorithmById  FALSE
	#define EXP_CryptoGetAlgorithmById  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoGetAlgorithmById
	#define EXT_CryptoGetAlgorithmById
	#define GET_CryptoGetAlgorithmById(fl)  CAL_CMGETAPI( "CryptoGetAlgorithmById" ) 
	#define CAL_CryptoGetAlgorithmById  CryptoGetAlgorithmById
	#define CHK_CryptoGetAlgorithmById  TRUE
	#define EXP_CryptoGetAlgorithmById  CAL_CMEXPAPI( "CryptoGetAlgorithmById" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoGetAlgorithmById
	#define EXT_CryptoGetAlgorithmById
	#define GET_CryptoGetAlgorithmById(fl)  CAL_CMGETAPI( "CryptoGetAlgorithmById" ) 
	#define CAL_CryptoGetAlgorithmById  CryptoGetAlgorithmById
	#define CHK_CryptoGetAlgorithmById  TRUE
	#define EXP_CryptoGetAlgorithmById  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGetAlgorithmById", (RTS_UINTPTR)CryptoGetAlgorithmById, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoGetAlgorithmById
	#define EXT_CmpCryptoCryptoGetAlgorithmById
	#define GET_CmpCryptoCryptoGetAlgorithmById  ERR_OK
	#define CAL_CmpCryptoCryptoGetAlgorithmById pICmpCrypto->ICryptoGetAlgorithmById
	#define CHK_CmpCryptoCryptoGetAlgorithmById (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoGetAlgorithmById  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoGetAlgorithmById
	#define EXT_CryptoGetAlgorithmById
	#define GET_CryptoGetAlgorithmById(fl)  CAL_CMGETAPI( "CryptoGetAlgorithmById" ) 
	#define CAL_CryptoGetAlgorithmById pICmpCrypto->ICryptoGetAlgorithmById
	#define CHK_CryptoGetAlgorithmById (pICmpCrypto != NULL)
	#define EXP_CryptoGetAlgorithmById  CAL_CMEXPAPI( "CryptoGetAlgorithmById" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoGetAlgorithmById  PFCRYPTOGETALGORITHMBYID pfCryptoGetAlgorithmById;
	#define EXT_CryptoGetAlgorithmById  extern PFCRYPTOGETALGORITHMBYID pfCryptoGetAlgorithmById;
	#define GET_CryptoGetAlgorithmById(fl)  s_pfCMGetAPI2( "CryptoGetAlgorithmById", (RTS_VOID_FCTPTR *)&pfCryptoGetAlgorithmById, (fl), 0, 0)
	#define CAL_CryptoGetAlgorithmById  pfCryptoGetAlgorithmById
	#define CHK_CryptoGetAlgorithmById  (pfCryptoGetAlgorithmById != NULL)
	#define EXP_CryptoGetAlgorithmById  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGetAlgorithmById", (RTS_UINTPTR)CryptoGetAlgorithmById, 0, 0) 
#endif




/**
 * <description>Get the first algorithm matching the type given. Use this with the 
 * GetNext function to iterate over algorithms of a specific type.</description>
 * <param name="ui32CryptoType" type="IN">Type of the algorithm. See: Crypto Types</param>
 * <param name="pszName" type="OUT">Name of the algorithm. Can be NULL.</param>
 * <param name="i32MaxNameLen" type="OUT">Maximum length of the name buffer</param>
 * <param name="pui32CryptoID" type="OUT">ID of the algorithm.</param>
 * <param name="pResult" type="IN">Error code: Result of the operation. Can be NULL.</param>
 * <result>Handle to the cryptographic algorithm</result>
 */
RTS_HANDLE CDECL CryptoGetFirstAlgorithm(RTS_UI32 ui32CryptoType, char *pszName, RTS_I32 i32MaxNameLen, RTS_UI32 *pui32CryptoID, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFCRYPTOGETFIRSTALGORITHM) (RTS_UI32 ui32CryptoType, char *pszName, RTS_I32 i32MaxNameLen, RTS_UI32 *pui32CryptoID, RTS_RESULT *pResult);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGETFIRSTALGORITHM_NOTIMPLEMENTED)
	#define USE_CryptoGetFirstAlgorithm
	#define EXT_CryptoGetFirstAlgorithm
	#define GET_CryptoGetFirstAlgorithm(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoGetFirstAlgorithm(p0,p1,p2,p3,p4)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CryptoGetFirstAlgorithm  FALSE
	#define EXP_CryptoGetFirstAlgorithm  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoGetFirstAlgorithm
	#define EXT_CryptoGetFirstAlgorithm
	#define GET_CryptoGetFirstAlgorithm(fl)  CAL_CMGETAPI( "CryptoGetFirstAlgorithm" ) 
	#define CAL_CryptoGetFirstAlgorithm  CryptoGetFirstAlgorithm
	#define CHK_CryptoGetFirstAlgorithm  TRUE
	#define EXP_CryptoGetFirstAlgorithm  CAL_CMEXPAPI( "CryptoGetFirstAlgorithm" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoGetFirstAlgorithm
	#define EXT_CryptoGetFirstAlgorithm
	#define GET_CryptoGetFirstAlgorithm(fl)  CAL_CMGETAPI( "CryptoGetFirstAlgorithm" ) 
	#define CAL_CryptoGetFirstAlgorithm  CryptoGetFirstAlgorithm
	#define CHK_CryptoGetFirstAlgorithm  TRUE
	#define EXP_CryptoGetFirstAlgorithm  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGetFirstAlgorithm", (RTS_UINTPTR)CryptoGetFirstAlgorithm, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoGetFirstAlgorithm
	#define EXT_CmpCryptoCryptoGetFirstAlgorithm
	#define GET_CmpCryptoCryptoGetFirstAlgorithm  ERR_OK
	#define CAL_CmpCryptoCryptoGetFirstAlgorithm pICmpCrypto->ICryptoGetFirstAlgorithm
	#define CHK_CmpCryptoCryptoGetFirstAlgorithm (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoGetFirstAlgorithm  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoGetFirstAlgorithm
	#define EXT_CryptoGetFirstAlgorithm
	#define GET_CryptoGetFirstAlgorithm(fl)  CAL_CMGETAPI( "CryptoGetFirstAlgorithm" ) 
	#define CAL_CryptoGetFirstAlgorithm pICmpCrypto->ICryptoGetFirstAlgorithm
	#define CHK_CryptoGetFirstAlgorithm (pICmpCrypto != NULL)
	#define EXP_CryptoGetFirstAlgorithm  CAL_CMEXPAPI( "CryptoGetFirstAlgorithm" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoGetFirstAlgorithm  PFCRYPTOGETFIRSTALGORITHM pfCryptoGetFirstAlgorithm;
	#define EXT_CryptoGetFirstAlgorithm  extern PFCRYPTOGETFIRSTALGORITHM pfCryptoGetFirstAlgorithm;
	#define GET_CryptoGetFirstAlgorithm(fl)  s_pfCMGetAPI2( "CryptoGetFirstAlgorithm", (RTS_VOID_FCTPTR *)&pfCryptoGetFirstAlgorithm, (fl), 0, 0)
	#define CAL_CryptoGetFirstAlgorithm  pfCryptoGetFirstAlgorithm
	#define CHK_CryptoGetFirstAlgorithm  (pfCryptoGetFirstAlgorithm != NULL)
	#define EXP_CryptoGetFirstAlgorithm  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGetFirstAlgorithm", (RTS_UINTPTR)CryptoGetFirstAlgorithm, 0, 0) 
#endif




/**
 * <description>Get the next algorithm matching the type given.</description>
 * <param name="hCrypto" type="IN">Handle to the last algorithm.</param>
 * <param name="ui32CryptoType" type="IN">Handle to the last algorithm.</param>
 * <param name="pszName" type="OUT">Name of the algorithm. Can be NULL.</param>
 * <param name="i32MaxNameLen" type="OUT">Maximum length of the name buffer</param>
 * <param name="pui32CryptoID" type="OUT">ID of the algorithm.</param>
 * <param name="pResult" type="IN">Error code: Result of the operation. Can be NULL.</param>
 * <result>Handle to the cryptographic algorithm</result>
 */
RTS_HANDLE CDECL CryptoGetNextAlgorithm(RTS_HANDLE hCrypto, RTS_UI32 ui32CryptoType, char *pszName, RTS_I32 i32MaxNameLen, RTS_UI32 *pui32CryptoID, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFCRYPTOGETNEXTALGORITHM) (RTS_HANDLE hCrypto, RTS_UI32 ui32CryptoType, char *pszName, RTS_I32 i32MaxNameLen, RTS_UI32 *pui32CryptoID, RTS_RESULT *pResult);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGETNEXTALGORITHM_NOTIMPLEMENTED)
	#define USE_CryptoGetNextAlgorithm
	#define EXT_CryptoGetNextAlgorithm
	#define GET_CryptoGetNextAlgorithm(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoGetNextAlgorithm(p0,p1,p2,p3,p4,p5)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CryptoGetNextAlgorithm  FALSE
	#define EXP_CryptoGetNextAlgorithm  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoGetNextAlgorithm
	#define EXT_CryptoGetNextAlgorithm
	#define GET_CryptoGetNextAlgorithm(fl)  CAL_CMGETAPI( "CryptoGetNextAlgorithm" ) 
	#define CAL_CryptoGetNextAlgorithm  CryptoGetNextAlgorithm
	#define CHK_CryptoGetNextAlgorithm  TRUE
	#define EXP_CryptoGetNextAlgorithm  CAL_CMEXPAPI( "CryptoGetNextAlgorithm" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoGetNextAlgorithm
	#define EXT_CryptoGetNextAlgorithm
	#define GET_CryptoGetNextAlgorithm(fl)  CAL_CMGETAPI( "CryptoGetNextAlgorithm" ) 
	#define CAL_CryptoGetNextAlgorithm  CryptoGetNextAlgorithm
	#define CHK_CryptoGetNextAlgorithm  TRUE
	#define EXP_CryptoGetNextAlgorithm  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGetNextAlgorithm", (RTS_UINTPTR)CryptoGetNextAlgorithm, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoGetNextAlgorithm
	#define EXT_CmpCryptoCryptoGetNextAlgorithm
	#define GET_CmpCryptoCryptoGetNextAlgorithm  ERR_OK
	#define CAL_CmpCryptoCryptoGetNextAlgorithm pICmpCrypto->ICryptoGetNextAlgorithm
	#define CHK_CmpCryptoCryptoGetNextAlgorithm (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoGetNextAlgorithm  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoGetNextAlgorithm
	#define EXT_CryptoGetNextAlgorithm
	#define GET_CryptoGetNextAlgorithm(fl)  CAL_CMGETAPI( "CryptoGetNextAlgorithm" ) 
	#define CAL_CryptoGetNextAlgorithm pICmpCrypto->ICryptoGetNextAlgorithm
	#define CHK_CryptoGetNextAlgorithm (pICmpCrypto != NULL)
	#define EXP_CryptoGetNextAlgorithm  CAL_CMEXPAPI( "CryptoGetNextAlgorithm" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoGetNextAlgorithm  PFCRYPTOGETNEXTALGORITHM pfCryptoGetNextAlgorithm;
	#define EXT_CryptoGetNextAlgorithm  extern PFCRYPTOGETNEXTALGORITHM pfCryptoGetNextAlgorithm;
	#define GET_CryptoGetNextAlgorithm(fl)  s_pfCMGetAPI2( "CryptoGetNextAlgorithm", (RTS_VOID_FCTPTR *)&pfCryptoGetNextAlgorithm, (fl), 0, 0)
	#define CAL_CryptoGetNextAlgorithm  pfCryptoGetNextAlgorithm
	#define CHK_CryptoGetNextAlgorithm  (pfCryptoGetNextAlgorithm != NULL)
	#define EXP_CryptoGetNextAlgorithm  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGetNextAlgorithm", (RTS_UINTPTR)CryptoGetNextAlgorithm, 0, 0) 
#endif




/**
 * <description>Get information of the algorithm. Don't free or modify the CryptoInfo.</description>
 * <param name="hCrypto" type="IN">Handle to the algorithm.</param>
 * <param name="pResult" type="IN">Error code: Result of the operation. Can be NULL.</param>
 * <result>Pointer to the information structure.</result>
 */
RtsCryptoInfo* CDECL CryptoGetAlgorithmInfo(RTS_HANDLE hCrypto, RTS_RESULT* pCryptoInfo);
typedef RtsCryptoInfo* (CDECL * PFCRYPTOGETALGORITHMINFO) (RTS_HANDLE hCrypto, RTS_RESULT* pCryptoInfo);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGETALGORITHMINFO_NOTIMPLEMENTED)
	#define USE_CryptoGetAlgorithmInfo
	#define EXT_CryptoGetAlgorithmInfo
	#define GET_CryptoGetAlgorithmInfo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoGetAlgorithmInfo(p0,p1)  (RtsCryptoInfo*)ERR_NOTIMPLEMENTED
	#define CHK_CryptoGetAlgorithmInfo  FALSE
	#define EXP_CryptoGetAlgorithmInfo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoGetAlgorithmInfo
	#define EXT_CryptoGetAlgorithmInfo
	#define GET_CryptoGetAlgorithmInfo(fl)  CAL_CMGETAPI( "CryptoGetAlgorithmInfo" ) 
	#define CAL_CryptoGetAlgorithmInfo  CryptoGetAlgorithmInfo
	#define CHK_CryptoGetAlgorithmInfo  TRUE
	#define EXP_CryptoGetAlgorithmInfo  CAL_CMEXPAPI( "CryptoGetAlgorithmInfo" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoGetAlgorithmInfo
	#define EXT_CryptoGetAlgorithmInfo
	#define GET_CryptoGetAlgorithmInfo(fl)  CAL_CMGETAPI( "CryptoGetAlgorithmInfo" ) 
	#define CAL_CryptoGetAlgorithmInfo  CryptoGetAlgorithmInfo
	#define CHK_CryptoGetAlgorithmInfo  TRUE
	#define EXP_CryptoGetAlgorithmInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGetAlgorithmInfo", (RTS_UINTPTR)CryptoGetAlgorithmInfo, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoGetAlgorithmInfo
	#define EXT_CmpCryptoCryptoGetAlgorithmInfo
	#define GET_CmpCryptoCryptoGetAlgorithmInfo  ERR_OK
	#define CAL_CmpCryptoCryptoGetAlgorithmInfo pICmpCrypto->ICryptoGetAlgorithmInfo
	#define CHK_CmpCryptoCryptoGetAlgorithmInfo (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoGetAlgorithmInfo  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoGetAlgorithmInfo
	#define EXT_CryptoGetAlgorithmInfo
	#define GET_CryptoGetAlgorithmInfo(fl)  CAL_CMGETAPI( "CryptoGetAlgorithmInfo" ) 
	#define CAL_CryptoGetAlgorithmInfo pICmpCrypto->ICryptoGetAlgorithmInfo
	#define CHK_CryptoGetAlgorithmInfo (pICmpCrypto != NULL)
	#define EXP_CryptoGetAlgorithmInfo  CAL_CMEXPAPI( "CryptoGetAlgorithmInfo" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoGetAlgorithmInfo  PFCRYPTOGETALGORITHMINFO pfCryptoGetAlgorithmInfo;
	#define EXT_CryptoGetAlgorithmInfo  extern PFCRYPTOGETALGORITHMINFO pfCryptoGetAlgorithmInfo;
	#define GET_CryptoGetAlgorithmInfo(fl)  s_pfCMGetAPI2( "CryptoGetAlgorithmInfo", (RTS_VOID_FCTPTR *)&pfCryptoGetAlgorithmInfo, (fl), 0, 0)
	#define CAL_CryptoGetAlgorithmInfo  pfCryptoGetAlgorithmInfo
	#define CHK_CryptoGetAlgorithmInfo  (pfCryptoGetAlgorithmInfo != NULL)
	#define EXP_CryptoGetAlgorithmInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGetAlgorithmInfo", (RTS_UINTPTR)CryptoGetAlgorithmInfo, 0, 0) 
#endif




/**
 * <description>Perform a symmetric encryption using the algorithm handle.</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pPlainText" type="IN">Data to be encrypted</param>
 * <param name="key" type="IN">Key to encrypt the data. Has to be a KeyType_Key key.</param>
 * <param name="pInitVector" type="IN">Initialization vector of the encryption</param>
 * <param name="bEnablePadding" type="IN">Enables the padding of the algorithm. If padding is not enabled the plain text length has to match a multiple of the cipher block length</param>
 * <param name="pCipherText" type="OUT">Encrypted data.</param>
 * <result>Error code: Result of the operation</result>
 */
RTS_RESULT CDECL CryptoSymmetricEncrypt(RTS_HANDLE hAlgo, RtsByteString* pPlainText, RtsCryptoKey key, RtsByteString* pInitVector, RTS_BOOL bEnablePadding, RtsByteString* pCipherText);
typedef RTS_RESULT (CDECL * PFCRYPTOSYMMETRICENCRYPT) (RTS_HANDLE hAlgo, RtsByteString* pPlainText, RtsCryptoKey key, RtsByteString* pInitVector, RTS_BOOL bEnablePadding, RtsByteString* pCipherText);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOSYMMETRICENCRYPT_NOTIMPLEMENTED)
	#define USE_CryptoSymmetricEncrypt
	#define EXT_CryptoSymmetricEncrypt
	#define GET_CryptoSymmetricEncrypt(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoSymmetricEncrypt(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoSymmetricEncrypt  FALSE
	#define EXP_CryptoSymmetricEncrypt  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoSymmetricEncrypt
	#define EXT_CryptoSymmetricEncrypt
	#define GET_CryptoSymmetricEncrypt(fl)  CAL_CMGETAPI( "CryptoSymmetricEncrypt" ) 
	#define CAL_CryptoSymmetricEncrypt  CryptoSymmetricEncrypt
	#define CHK_CryptoSymmetricEncrypt  TRUE
	#define EXP_CryptoSymmetricEncrypt  CAL_CMEXPAPI( "CryptoSymmetricEncrypt" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoSymmetricEncrypt
	#define EXT_CryptoSymmetricEncrypt
	#define GET_CryptoSymmetricEncrypt(fl)  CAL_CMGETAPI( "CryptoSymmetricEncrypt" ) 
	#define CAL_CryptoSymmetricEncrypt  CryptoSymmetricEncrypt
	#define CHK_CryptoSymmetricEncrypt  TRUE
	#define EXP_CryptoSymmetricEncrypt  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoSymmetricEncrypt", (RTS_UINTPTR)CryptoSymmetricEncrypt, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoSymmetricEncrypt
	#define EXT_CmpCryptoCryptoSymmetricEncrypt
	#define GET_CmpCryptoCryptoSymmetricEncrypt  ERR_OK
	#define CAL_CmpCryptoCryptoSymmetricEncrypt pICmpCrypto->ICryptoSymmetricEncrypt
	#define CHK_CmpCryptoCryptoSymmetricEncrypt (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoSymmetricEncrypt  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoSymmetricEncrypt
	#define EXT_CryptoSymmetricEncrypt
	#define GET_CryptoSymmetricEncrypt(fl)  CAL_CMGETAPI( "CryptoSymmetricEncrypt" ) 
	#define CAL_CryptoSymmetricEncrypt pICmpCrypto->ICryptoSymmetricEncrypt
	#define CHK_CryptoSymmetricEncrypt (pICmpCrypto != NULL)
	#define EXP_CryptoSymmetricEncrypt  CAL_CMEXPAPI( "CryptoSymmetricEncrypt" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoSymmetricEncrypt  PFCRYPTOSYMMETRICENCRYPT pfCryptoSymmetricEncrypt;
	#define EXT_CryptoSymmetricEncrypt  extern PFCRYPTOSYMMETRICENCRYPT pfCryptoSymmetricEncrypt;
	#define GET_CryptoSymmetricEncrypt(fl)  s_pfCMGetAPI2( "CryptoSymmetricEncrypt", (RTS_VOID_FCTPTR *)&pfCryptoSymmetricEncrypt, (fl), 0, 0)
	#define CAL_CryptoSymmetricEncrypt  pfCryptoSymmetricEncrypt
	#define CHK_CryptoSymmetricEncrypt  (pfCryptoSymmetricEncrypt != NULL)
	#define EXP_CryptoSymmetricEncrypt  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoSymmetricEncrypt", (RTS_UINTPTR)CryptoSymmetricEncrypt, 0, 0) 
#endif




/**
 * <description>Perform a symmetric decryption using the algorithm handle.</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pCipherText" type="IN">Data to be decrypted</param>
 * <param name="key" type="IN">Key to decrypt the data. Has to be a KeyType_Key key.</param>
 * <param name="pInitVector" type="IN">Initialization vector of the decrypt.</param>
 * <param name="bEnablePadding" type="IN">Enables the padding of the algorithm. If padding is not enabled the plain text length has to match a multiple of the cipher block length</param>
 * <param name="pCipherText" type="OUT">Decrypted data.</param>
 * <result>Error code: Result of the operation</result>
 */
RTS_RESULT CDECL CryptoSymmetricDecrypt(RTS_HANDLE hAlgo, RtsByteString* pCipherText, RtsCryptoKey key, RtsByteString* pInitVector, RTS_BOOL bEnablePadding, RtsByteString* pPlainText);
typedef RTS_RESULT (CDECL * PFCRYPTOSYMMETRICDECRYPT) (RTS_HANDLE hAlgo, RtsByteString* pCipherText, RtsCryptoKey key, RtsByteString* pInitVector, RTS_BOOL bEnablePadding, RtsByteString* pPlainText);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOSYMMETRICDECRYPT_NOTIMPLEMENTED)
	#define USE_CryptoSymmetricDecrypt
	#define EXT_CryptoSymmetricDecrypt
	#define GET_CryptoSymmetricDecrypt(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoSymmetricDecrypt(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoSymmetricDecrypt  FALSE
	#define EXP_CryptoSymmetricDecrypt  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoSymmetricDecrypt
	#define EXT_CryptoSymmetricDecrypt
	#define GET_CryptoSymmetricDecrypt(fl)  CAL_CMGETAPI( "CryptoSymmetricDecrypt" ) 
	#define CAL_CryptoSymmetricDecrypt  CryptoSymmetricDecrypt
	#define CHK_CryptoSymmetricDecrypt  TRUE
	#define EXP_CryptoSymmetricDecrypt  CAL_CMEXPAPI( "CryptoSymmetricDecrypt" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoSymmetricDecrypt
	#define EXT_CryptoSymmetricDecrypt
	#define GET_CryptoSymmetricDecrypt(fl)  CAL_CMGETAPI( "CryptoSymmetricDecrypt" ) 
	#define CAL_CryptoSymmetricDecrypt  CryptoSymmetricDecrypt
	#define CHK_CryptoSymmetricDecrypt  TRUE
	#define EXP_CryptoSymmetricDecrypt  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoSymmetricDecrypt", (RTS_UINTPTR)CryptoSymmetricDecrypt, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoSymmetricDecrypt
	#define EXT_CmpCryptoCryptoSymmetricDecrypt
	#define GET_CmpCryptoCryptoSymmetricDecrypt  ERR_OK
	#define CAL_CmpCryptoCryptoSymmetricDecrypt pICmpCrypto->ICryptoSymmetricDecrypt
	#define CHK_CmpCryptoCryptoSymmetricDecrypt (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoSymmetricDecrypt  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoSymmetricDecrypt
	#define EXT_CryptoSymmetricDecrypt
	#define GET_CryptoSymmetricDecrypt(fl)  CAL_CMGETAPI( "CryptoSymmetricDecrypt" ) 
	#define CAL_CryptoSymmetricDecrypt pICmpCrypto->ICryptoSymmetricDecrypt
	#define CHK_CryptoSymmetricDecrypt (pICmpCrypto != NULL)
	#define EXP_CryptoSymmetricDecrypt  CAL_CMEXPAPI( "CryptoSymmetricDecrypt" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoSymmetricDecrypt  PFCRYPTOSYMMETRICDECRYPT pfCryptoSymmetricDecrypt;
	#define EXT_CryptoSymmetricDecrypt  extern PFCRYPTOSYMMETRICDECRYPT pfCryptoSymmetricDecrypt;
	#define GET_CryptoSymmetricDecrypt(fl)  s_pfCMGetAPI2( "CryptoSymmetricDecrypt", (RTS_VOID_FCTPTR *)&pfCryptoSymmetricDecrypt, (fl), 0, 0)
	#define CAL_CryptoSymmetricDecrypt  pfCryptoSymmetricDecrypt
	#define CHK_CryptoSymmetricDecrypt  (pfCryptoSymmetricDecrypt != NULL)
	#define EXP_CryptoSymmetricDecrypt  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoSymmetricDecrypt", (RTS_UINTPTR)CryptoSymmetricDecrypt, 0, 0) 
#endif




/**
 * <description>Perform an asymmetric encryption using the algorithm handle.</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pPlainText" type="IN">Data to be encrypted</param>
 * <param name="publicKey" type="IN">Public key used to encrypt the data. Use X509CertGetPublicKey to get the public key form an certificate.</param>
 * <param name="pCipherText" type="OUT">Encrypted data.</param>
 * <result>Error code: Result of the operation</result>
 */
RTS_RESULT CDECL CryptoAsymmetricEncrypt(RTS_HANDLE hAlgo, RtsByteString* pPlainText, RtsCryptoKey publicKey, RtsByteString* pCipherText);
typedef RTS_RESULT (CDECL * PFCRYPTOASYMMETRICENCRYPT) (RTS_HANDLE hAlgo, RtsByteString* pPlainText, RtsCryptoKey publicKey, RtsByteString* pCipherText);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOASYMMETRICENCRYPT_NOTIMPLEMENTED)
	#define USE_CryptoAsymmetricEncrypt
	#define EXT_CryptoAsymmetricEncrypt
	#define GET_CryptoAsymmetricEncrypt(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoAsymmetricEncrypt(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoAsymmetricEncrypt  FALSE
	#define EXP_CryptoAsymmetricEncrypt  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoAsymmetricEncrypt
	#define EXT_CryptoAsymmetricEncrypt
	#define GET_CryptoAsymmetricEncrypt(fl)  CAL_CMGETAPI( "CryptoAsymmetricEncrypt" ) 
	#define CAL_CryptoAsymmetricEncrypt  CryptoAsymmetricEncrypt
	#define CHK_CryptoAsymmetricEncrypt  TRUE
	#define EXP_CryptoAsymmetricEncrypt  CAL_CMEXPAPI( "CryptoAsymmetricEncrypt" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoAsymmetricEncrypt
	#define EXT_CryptoAsymmetricEncrypt
	#define GET_CryptoAsymmetricEncrypt(fl)  CAL_CMGETAPI( "CryptoAsymmetricEncrypt" ) 
	#define CAL_CryptoAsymmetricEncrypt  CryptoAsymmetricEncrypt
	#define CHK_CryptoAsymmetricEncrypt  TRUE
	#define EXP_CryptoAsymmetricEncrypt  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoAsymmetricEncrypt", (RTS_UINTPTR)CryptoAsymmetricEncrypt, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoAsymmetricEncrypt
	#define EXT_CmpCryptoCryptoAsymmetricEncrypt
	#define GET_CmpCryptoCryptoAsymmetricEncrypt  ERR_OK
	#define CAL_CmpCryptoCryptoAsymmetricEncrypt pICmpCrypto->ICryptoAsymmetricEncrypt
	#define CHK_CmpCryptoCryptoAsymmetricEncrypt (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoAsymmetricEncrypt  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoAsymmetricEncrypt
	#define EXT_CryptoAsymmetricEncrypt
	#define GET_CryptoAsymmetricEncrypt(fl)  CAL_CMGETAPI( "CryptoAsymmetricEncrypt" ) 
	#define CAL_CryptoAsymmetricEncrypt pICmpCrypto->ICryptoAsymmetricEncrypt
	#define CHK_CryptoAsymmetricEncrypt (pICmpCrypto != NULL)
	#define EXP_CryptoAsymmetricEncrypt  CAL_CMEXPAPI( "CryptoAsymmetricEncrypt" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoAsymmetricEncrypt  PFCRYPTOASYMMETRICENCRYPT pfCryptoAsymmetricEncrypt;
	#define EXT_CryptoAsymmetricEncrypt  extern PFCRYPTOASYMMETRICENCRYPT pfCryptoAsymmetricEncrypt;
	#define GET_CryptoAsymmetricEncrypt(fl)  s_pfCMGetAPI2( "CryptoAsymmetricEncrypt", (RTS_VOID_FCTPTR *)&pfCryptoAsymmetricEncrypt, (fl), 0, 0)
	#define CAL_CryptoAsymmetricEncrypt  pfCryptoAsymmetricEncrypt
	#define CHK_CryptoAsymmetricEncrypt  (pfCryptoAsymmetricEncrypt != NULL)
	#define EXP_CryptoAsymmetricEncrypt  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoAsymmetricEncrypt", (RTS_UINTPTR)CryptoAsymmetricEncrypt, 0, 0) 
#endif




/**
 * <description>Perform an asymmetric decryption using the algorithm handle.</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pCiperText" type="IN">Data to be decrypted</param>
 * <param name="privateKey" type="IN">Private key used to decrypt the data. Use X509CertGetPrivateKey to get the private key from an certificate.</param>
 * <param name="pCipherText" type="OUT">Decrypted data.</param>
 * <result>Error code: Result of the operation</result>
 */
RTS_RESULT CDECL CryptoAsymmetricDecrypt(RTS_HANDLE hAlgo, RtsByteString* pCipherText, RtsCryptoKey privateKey, RtsByteString* pPlainText);
typedef RTS_RESULT (CDECL * PFCRYPTOASYMMETRICDECRYPT) (RTS_HANDLE hAlgo, RtsByteString* pCipherText, RtsCryptoKey privateKey, RtsByteString* pPlainText);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOASYMMETRICDECRYPT_NOTIMPLEMENTED)
	#define USE_CryptoAsymmetricDecrypt
	#define EXT_CryptoAsymmetricDecrypt
	#define GET_CryptoAsymmetricDecrypt(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoAsymmetricDecrypt(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoAsymmetricDecrypt  FALSE
	#define EXP_CryptoAsymmetricDecrypt  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoAsymmetricDecrypt
	#define EXT_CryptoAsymmetricDecrypt
	#define GET_CryptoAsymmetricDecrypt(fl)  CAL_CMGETAPI( "CryptoAsymmetricDecrypt" ) 
	#define CAL_CryptoAsymmetricDecrypt  CryptoAsymmetricDecrypt
	#define CHK_CryptoAsymmetricDecrypt  TRUE
	#define EXP_CryptoAsymmetricDecrypt  CAL_CMEXPAPI( "CryptoAsymmetricDecrypt" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoAsymmetricDecrypt
	#define EXT_CryptoAsymmetricDecrypt
	#define GET_CryptoAsymmetricDecrypt(fl)  CAL_CMGETAPI( "CryptoAsymmetricDecrypt" ) 
	#define CAL_CryptoAsymmetricDecrypt  CryptoAsymmetricDecrypt
	#define CHK_CryptoAsymmetricDecrypt  TRUE
	#define EXP_CryptoAsymmetricDecrypt  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoAsymmetricDecrypt", (RTS_UINTPTR)CryptoAsymmetricDecrypt, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoAsymmetricDecrypt
	#define EXT_CmpCryptoCryptoAsymmetricDecrypt
	#define GET_CmpCryptoCryptoAsymmetricDecrypt  ERR_OK
	#define CAL_CmpCryptoCryptoAsymmetricDecrypt pICmpCrypto->ICryptoAsymmetricDecrypt
	#define CHK_CmpCryptoCryptoAsymmetricDecrypt (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoAsymmetricDecrypt  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoAsymmetricDecrypt
	#define EXT_CryptoAsymmetricDecrypt
	#define GET_CryptoAsymmetricDecrypt(fl)  CAL_CMGETAPI( "CryptoAsymmetricDecrypt" ) 
	#define CAL_CryptoAsymmetricDecrypt pICmpCrypto->ICryptoAsymmetricDecrypt
	#define CHK_CryptoAsymmetricDecrypt (pICmpCrypto != NULL)
	#define EXP_CryptoAsymmetricDecrypt  CAL_CMEXPAPI( "CryptoAsymmetricDecrypt" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoAsymmetricDecrypt  PFCRYPTOASYMMETRICDECRYPT pfCryptoAsymmetricDecrypt;
	#define EXT_CryptoAsymmetricDecrypt  extern PFCRYPTOASYMMETRICDECRYPT pfCryptoAsymmetricDecrypt;
	#define GET_CryptoAsymmetricDecrypt(fl)  s_pfCMGetAPI2( "CryptoAsymmetricDecrypt", (RTS_VOID_FCTPTR *)&pfCryptoAsymmetricDecrypt, (fl), 0, 0)
	#define CAL_CryptoAsymmetricDecrypt  pfCryptoAsymmetricDecrypt
	#define CHK_CryptoAsymmetricDecrypt  (pfCryptoAsymmetricDecrypt != NULL)
	#define EXP_CryptoAsymmetricDecrypt  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoAsymmetricDecrypt", (RTS_UINTPTR)CryptoAsymmetricDecrypt, 0, 0) 
#endif




/**
 * <description>Get the private key size in bits.</description>
 * <param name="priavteKey" type="IN">Asymmetric key of interest. Use X509CertGetPublicKey or X509CertGetPrivateKey to get the asymmetric key.</param>
 * <param name="pResult" type="OUT">Error code: Operation Result</param>
 * <result>Size of the private key in bits</result>
 */
RTS_UI32 CDECL CryptoGetAsymmetricKeyLength(RtsCryptoKey asymmetricKey, RTS_RESULT* pResult);
typedef RTS_UI32 (CDECL * PFCRYPTOGETASYMMETRICKEYLENGTH) (RtsCryptoKey asymmetricKey, RTS_RESULT* pResult);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGETASYMMETRICKEYLENGTH_NOTIMPLEMENTED)
	#define USE_CryptoGetAsymmetricKeyLength
	#define EXT_CryptoGetAsymmetricKeyLength
	#define GET_CryptoGetAsymmetricKeyLength(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoGetAsymmetricKeyLength(p0,p1)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_CryptoGetAsymmetricKeyLength  FALSE
	#define EXP_CryptoGetAsymmetricKeyLength  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoGetAsymmetricKeyLength
	#define EXT_CryptoGetAsymmetricKeyLength
	#define GET_CryptoGetAsymmetricKeyLength(fl)  CAL_CMGETAPI( "CryptoGetAsymmetricKeyLength" ) 
	#define CAL_CryptoGetAsymmetricKeyLength  CryptoGetAsymmetricKeyLength
	#define CHK_CryptoGetAsymmetricKeyLength  TRUE
	#define EXP_CryptoGetAsymmetricKeyLength  CAL_CMEXPAPI( "CryptoGetAsymmetricKeyLength" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoGetAsymmetricKeyLength
	#define EXT_CryptoGetAsymmetricKeyLength
	#define GET_CryptoGetAsymmetricKeyLength(fl)  CAL_CMGETAPI( "CryptoGetAsymmetricKeyLength" ) 
	#define CAL_CryptoGetAsymmetricKeyLength  CryptoGetAsymmetricKeyLength
	#define CHK_CryptoGetAsymmetricKeyLength  TRUE
	#define EXP_CryptoGetAsymmetricKeyLength  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGetAsymmetricKeyLength", (RTS_UINTPTR)CryptoGetAsymmetricKeyLength, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoGetAsymmetricKeyLength
	#define EXT_CmpCryptoCryptoGetAsymmetricKeyLength
	#define GET_CmpCryptoCryptoGetAsymmetricKeyLength  ERR_OK
	#define CAL_CmpCryptoCryptoGetAsymmetricKeyLength pICmpCrypto->ICryptoGetAsymmetricKeyLength
	#define CHK_CmpCryptoCryptoGetAsymmetricKeyLength (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoGetAsymmetricKeyLength  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoGetAsymmetricKeyLength
	#define EXT_CryptoGetAsymmetricKeyLength
	#define GET_CryptoGetAsymmetricKeyLength(fl)  CAL_CMGETAPI( "CryptoGetAsymmetricKeyLength" ) 
	#define CAL_CryptoGetAsymmetricKeyLength pICmpCrypto->ICryptoGetAsymmetricKeyLength
	#define CHK_CryptoGetAsymmetricKeyLength (pICmpCrypto != NULL)
	#define EXP_CryptoGetAsymmetricKeyLength  CAL_CMEXPAPI( "CryptoGetAsymmetricKeyLength" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoGetAsymmetricKeyLength  PFCRYPTOGETASYMMETRICKEYLENGTH pfCryptoGetAsymmetricKeyLength;
	#define EXT_CryptoGetAsymmetricKeyLength  extern PFCRYPTOGETASYMMETRICKEYLENGTH pfCryptoGetAsymmetricKeyLength;
	#define GET_CryptoGetAsymmetricKeyLength(fl)  s_pfCMGetAPI2( "CryptoGetAsymmetricKeyLength", (RTS_VOID_FCTPTR *)&pfCryptoGetAsymmetricKeyLength, (fl), 0, 0)
	#define CAL_CryptoGetAsymmetricKeyLength  pfCryptoGetAsymmetricKeyLength
	#define CHK_CryptoGetAsymmetricKeyLength  (pfCryptoGetAsymmetricKeyLength != NULL)
	#define EXP_CryptoGetAsymmetricKeyLength  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGetAsymmetricKeyLength", (RTS_UINTPTR)CryptoGetAsymmetricKeyLength, 0, 0) 
#endif




/**
 * <description>Generate an asymmetric key pair that can be used by the specified algorithm.</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="ui32KeyLen" type="IN">Length of the key in bits. If 0, the configured minimal key length is used. May be omitted on ECC keys.</param>
 * <param name="pKeyPair" type="OUT">The generated key pair. Can be used for public or private key operations.</param>
 * <param name="pPublicKey" type="OUT">The corresponding public key. May be used for any suitable operation.</param>
 * <result>Operation result.
 *		- ERR_OK: KeyPair generated.
 *		- ERR_INVALID_HANDLE: hAlgo was not a valid handle.
 *		- ERR_PARAMETER: pKeyPair was NULL
 *		- ERR_SIZE_MISMATCH: ui32KeyLen did not meet the minimal required key length.
 *		- ERR_NOT_SUPPORTED: The algorithm passed with hAlgo does not support key generation.
 *		- ERR_FAILED: Some internal error happened.
 * </result>
 */	
RTS_RESULT CDECL CryptoGenerateAsymmetricKeyPair(RTS_IEC_HANDLE hAlgo, RTS_IEC_UDINT ui32KeyLen, RtsCryptoKey *pKeyPair, RTS_BOOL bPrivateKeyExportable);
typedef RTS_RESULT (CDECL * PFCRYPTOGENERATEASYMMETRICKEYPAIR) (RTS_IEC_HANDLE hAlgo, RTS_IEC_UDINT ui32KeyLen, RtsCryptoKey *pKeyPair, RTS_BOOL bPrivateKeyExportable);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGENERATEASYMMETRICKEYPAIR_NOTIMPLEMENTED)
	#define USE_CryptoGenerateAsymmetricKeyPair
	#define EXT_CryptoGenerateAsymmetricKeyPair
	#define GET_CryptoGenerateAsymmetricKeyPair(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoGenerateAsymmetricKeyPair(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoGenerateAsymmetricKeyPair  FALSE
	#define EXP_CryptoGenerateAsymmetricKeyPair  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoGenerateAsymmetricKeyPair
	#define EXT_CryptoGenerateAsymmetricKeyPair
	#define GET_CryptoGenerateAsymmetricKeyPair(fl)  CAL_CMGETAPI( "CryptoGenerateAsymmetricKeyPair" ) 
	#define CAL_CryptoGenerateAsymmetricKeyPair  CryptoGenerateAsymmetricKeyPair
	#define CHK_CryptoGenerateAsymmetricKeyPair  TRUE
	#define EXP_CryptoGenerateAsymmetricKeyPair  CAL_CMEXPAPI( "CryptoGenerateAsymmetricKeyPair" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoGenerateAsymmetricKeyPair
	#define EXT_CryptoGenerateAsymmetricKeyPair
	#define GET_CryptoGenerateAsymmetricKeyPair(fl)  CAL_CMGETAPI( "CryptoGenerateAsymmetricKeyPair" ) 
	#define CAL_CryptoGenerateAsymmetricKeyPair  CryptoGenerateAsymmetricKeyPair
	#define CHK_CryptoGenerateAsymmetricKeyPair  TRUE
	#define EXP_CryptoGenerateAsymmetricKeyPair  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGenerateAsymmetricKeyPair", (RTS_UINTPTR)CryptoGenerateAsymmetricKeyPair, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoGenerateAsymmetricKeyPair
	#define EXT_CmpCryptoCryptoGenerateAsymmetricKeyPair
	#define GET_CmpCryptoCryptoGenerateAsymmetricKeyPair  ERR_OK
	#define CAL_CmpCryptoCryptoGenerateAsymmetricKeyPair pICmpCrypto->ICryptoGenerateAsymmetricKeyPair
	#define CHK_CmpCryptoCryptoGenerateAsymmetricKeyPair (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoGenerateAsymmetricKeyPair  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoGenerateAsymmetricKeyPair
	#define EXT_CryptoGenerateAsymmetricKeyPair
	#define GET_CryptoGenerateAsymmetricKeyPair(fl)  CAL_CMGETAPI( "CryptoGenerateAsymmetricKeyPair" ) 
	#define CAL_CryptoGenerateAsymmetricKeyPair pICmpCrypto->ICryptoGenerateAsymmetricKeyPair
	#define CHK_CryptoGenerateAsymmetricKeyPair (pICmpCrypto != NULL)
	#define EXP_CryptoGenerateAsymmetricKeyPair  CAL_CMEXPAPI( "CryptoGenerateAsymmetricKeyPair" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoGenerateAsymmetricKeyPair  PFCRYPTOGENERATEASYMMETRICKEYPAIR pfCryptoGenerateAsymmetricKeyPair;
	#define EXT_CryptoGenerateAsymmetricKeyPair  extern PFCRYPTOGENERATEASYMMETRICKEYPAIR pfCryptoGenerateAsymmetricKeyPair;
	#define GET_CryptoGenerateAsymmetricKeyPair(fl)  s_pfCMGetAPI2( "CryptoGenerateAsymmetricKeyPair", (RTS_VOID_FCTPTR *)&pfCryptoGenerateAsymmetricKeyPair, (fl), 0, 0)
	#define CAL_CryptoGenerateAsymmetricKeyPair  pfCryptoGenerateAsymmetricKeyPair
	#define CHK_CryptoGenerateAsymmetricKeyPair  (pfCryptoGenerateAsymmetricKeyPair != NULL)
	#define EXP_CryptoGenerateAsymmetricKeyPair  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGenerateAsymmetricKeyPair", (RTS_UINTPTR)CryptoGenerateAsymmetricKeyPair, 0, 0) 
#endif




/**
 * <description>This function can be used to export asymmetric keys.</description>
 * <param name="key" type="IN">The key to be exported.</param>
 * <param name="bBase64" type="IN">Export the key in BASE64 encoding. Otherwise ASN.1 will be used.</param>
 * <param name="bPrivateKey" type="IN">TRUE: Export the private key. FALSE: Export the public key.</param>
 * <param name="pData" type="OUT">Pointer to the buffer where to store the key. Note: 
 *		If pData->pByData == NULL, the needed length to store the key wil be returned in ui32Len;
 *		If pData->pByData != NULL, the data will be written. A Maximum of ui32MaxLen will be written. The written length is stored within ui32Len.</param>
 * <result>Operation result. 
 *		- ERR_OK: Key exported successfully or length returned.
 *		- ERR_PARAMETER: key was invalid or pData was NULL.
 *		- ERR_NOT_SUPPORTED: It is not allowed to export this private key.</result>
 *		- ERR_FAILED: The key was not exported.
 * </result>
 */
RTS_RESULT CDECL CryptoExportAsymmetricKey(RtsCryptoKey key, RTS_BOOL bBase64, RTS_BOOL bPrivateKey, RtsByteString *pData);
typedef RTS_RESULT (CDECL * PFCRYPTOEXPORTASYMMETRICKEY) (RtsCryptoKey key, RTS_BOOL bBase64, RTS_BOOL bPrivateKey, RtsByteString *pData);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOEXPORTASYMMETRICKEY_NOTIMPLEMENTED)
	#define USE_CryptoExportAsymmetricKey
	#define EXT_CryptoExportAsymmetricKey
	#define GET_CryptoExportAsymmetricKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoExportAsymmetricKey(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoExportAsymmetricKey  FALSE
	#define EXP_CryptoExportAsymmetricKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoExportAsymmetricKey
	#define EXT_CryptoExportAsymmetricKey
	#define GET_CryptoExportAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoExportAsymmetricKey" ) 
	#define CAL_CryptoExportAsymmetricKey  CryptoExportAsymmetricKey
	#define CHK_CryptoExportAsymmetricKey  TRUE
	#define EXP_CryptoExportAsymmetricKey  CAL_CMEXPAPI( "CryptoExportAsymmetricKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoExportAsymmetricKey
	#define EXT_CryptoExportAsymmetricKey
	#define GET_CryptoExportAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoExportAsymmetricKey" ) 
	#define CAL_CryptoExportAsymmetricKey  CryptoExportAsymmetricKey
	#define CHK_CryptoExportAsymmetricKey  TRUE
	#define EXP_CryptoExportAsymmetricKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoExportAsymmetricKey", (RTS_UINTPTR)CryptoExportAsymmetricKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoExportAsymmetricKey
	#define EXT_CmpCryptoCryptoExportAsymmetricKey
	#define GET_CmpCryptoCryptoExportAsymmetricKey  ERR_OK
	#define CAL_CmpCryptoCryptoExportAsymmetricKey pICmpCrypto->ICryptoExportAsymmetricKey
	#define CHK_CmpCryptoCryptoExportAsymmetricKey (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoExportAsymmetricKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoExportAsymmetricKey
	#define EXT_CryptoExportAsymmetricKey
	#define GET_CryptoExportAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoExportAsymmetricKey" ) 
	#define CAL_CryptoExportAsymmetricKey pICmpCrypto->ICryptoExportAsymmetricKey
	#define CHK_CryptoExportAsymmetricKey (pICmpCrypto != NULL)
	#define EXP_CryptoExportAsymmetricKey  CAL_CMEXPAPI( "CryptoExportAsymmetricKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoExportAsymmetricKey  PFCRYPTOEXPORTASYMMETRICKEY pfCryptoExportAsymmetricKey;
	#define EXT_CryptoExportAsymmetricKey  extern PFCRYPTOEXPORTASYMMETRICKEY pfCryptoExportAsymmetricKey;
	#define GET_CryptoExportAsymmetricKey(fl)  s_pfCMGetAPI2( "CryptoExportAsymmetricKey", (RTS_VOID_FCTPTR *)&pfCryptoExportAsymmetricKey, (fl), 0, 0)
	#define CAL_CryptoExportAsymmetricKey  pfCryptoExportAsymmetricKey
	#define CHK_CryptoExportAsymmetricKey  (pfCryptoExportAsymmetricKey != NULL)
	#define EXP_CryptoExportAsymmetricKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoExportAsymmetricKey", (RTS_UINTPTR)CryptoExportAsymmetricKey, 0, 0) 
#endif




/**
 * <description>Exports the public key of a key pair in raw format. This means that no additional 
 *  data like ASN.1 or BASE64 encoding is generated. Not all key pairs will support this.</description>
 * <param name="key" type="IN">The key pair or public key to export in raw format.</param>
 * <param name="bPrivateKey" type="IN">TRUE: Export the private key. FALSE: Export the public key.</param>
 * <param name="pKeyData" type="INOUT">The bytestring to store the raw encoded key. If the bytestring is empty 
 *  (pByData == NULL, ui32MaxLen > 0, ui32Len == 0) the needed size will be returned. </param>
 * <result>Operation result.
 *		- ERR_OK: Public key was exported.
 *		- ERR_INVALID_STATE: The component is not in a valid state to do this operation.
 *		- ERR_PARAMETER: The key or the pKeyData was not valid.
 *		- ERR_NOT_SUPPORTED: The given key does not support raw export.
 *		- ERR_SIZE_MISMATCH: The given bytestring is too small.
 * </result>
 */	
RTS_RESULT CDECL CryptoExportRawAsymmetricKey(RtsCryptoKey key, RTS_BOOL bPrivateKey, RtsByteString *pKeyData);
typedef RTS_RESULT (CDECL * PFCRYPTOEXPORTRAWASYMMETRICKEY) (RtsCryptoKey key, RTS_BOOL bPrivateKey, RtsByteString *pKeyData);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOEXPORTRAWASYMMETRICKEY_NOTIMPLEMENTED)
	#define USE_CryptoExportRawAsymmetricKey
	#define EXT_CryptoExportRawAsymmetricKey
	#define GET_CryptoExportRawAsymmetricKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoExportRawAsymmetricKey(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoExportRawAsymmetricKey  FALSE
	#define EXP_CryptoExportRawAsymmetricKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoExportRawAsymmetricKey
	#define EXT_CryptoExportRawAsymmetricKey
	#define GET_CryptoExportRawAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoExportRawAsymmetricKey" ) 
	#define CAL_CryptoExportRawAsymmetricKey  CryptoExportRawAsymmetricKey
	#define CHK_CryptoExportRawAsymmetricKey  TRUE
	#define EXP_CryptoExportRawAsymmetricKey  CAL_CMEXPAPI( "CryptoExportRawAsymmetricKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoExportRawAsymmetricKey
	#define EXT_CryptoExportRawAsymmetricKey
	#define GET_CryptoExportRawAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoExportRawAsymmetricKey" ) 
	#define CAL_CryptoExportRawAsymmetricKey  CryptoExportRawAsymmetricKey
	#define CHK_CryptoExportRawAsymmetricKey  TRUE
	#define EXP_CryptoExportRawAsymmetricKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoExportRawAsymmetricKey", (RTS_UINTPTR)CryptoExportRawAsymmetricKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoExportRawAsymmetricKey
	#define EXT_CmpCryptoCryptoExportRawAsymmetricKey
	#define GET_CmpCryptoCryptoExportRawAsymmetricKey  ERR_OK
	#define CAL_CmpCryptoCryptoExportRawAsymmetricKey pICmpCrypto->ICryptoExportRawAsymmetricKey
	#define CHK_CmpCryptoCryptoExportRawAsymmetricKey (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoExportRawAsymmetricKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoExportRawAsymmetricKey
	#define EXT_CryptoExportRawAsymmetricKey
	#define GET_CryptoExportRawAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoExportRawAsymmetricKey" ) 
	#define CAL_CryptoExportRawAsymmetricKey pICmpCrypto->ICryptoExportRawAsymmetricKey
	#define CHK_CryptoExportRawAsymmetricKey (pICmpCrypto != NULL)
	#define EXP_CryptoExportRawAsymmetricKey  CAL_CMEXPAPI( "CryptoExportRawAsymmetricKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoExportRawAsymmetricKey  PFCRYPTOEXPORTRAWASYMMETRICKEY pfCryptoExportRawAsymmetricKey;
	#define EXT_CryptoExportRawAsymmetricKey  extern PFCRYPTOEXPORTRAWASYMMETRICKEY pfCryptoExportRawAsymmetricKey;
	#define GET_CryptoExportRawAsymmetricKey(fl)  s_pfCMGetAPI2( "CryptoExportRawAsymmetricKey", (RTS_VOID_FCTPTR *)&pfCryptoExportRawAsymmetricKey, (fl), 0, 0)
	#define CAL_CryptoExportRawAsymmetricKey  pfCryptoExportRawAsymmetricKey
	#define CHK_CryptoExportRawAsymmetricKey  (pfCryptoExportRawAsymmetricKey != NULL)
	#define EXP_CryptoExportRawAsymmetricKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoExportRawAsymmetricKey", (RTS_UINTPTR)CryptoExportRawAsymmetricKey, 0, 0) 
#endif




/**
 * <description>This function can be used to import asymmetric keys.</description>
 * <param name="data" type="IN">The data to be imported.</param>
 * <param name="bBase64" type="IN">TRUE: Import the key in BASE64 encoding. FALSE: Import the key in ASN.1 encoding.</param>
 * <param name="bPrivateKey" type="IN">TRUE: Import a private key. FALSE:  Import a public key.</param>
 * <param name="pKey" type="IN">The key generated from the imported data.</param>
 * <result>Operation result.
 *		- ERR_OK: The key was imported successfully.
 *		- ERR_PARAMETER: pKey was NULL or data was invalid.
 * 		- ERR_FAILED: The key was not imported successfully.
 * </result>
 */
RTS_RESULT CDECL CryptoImportAsymmetricKey(RtsByteString data, RTS_BOOL bBase64, RTS_BOOL bPrivateKey, RtsCryptoKey *pKey);
typedef RTS_RESULT (CDECL * PFCRYPTOIMPORTASYMMETRICKEY) (RtsByteString data, RTS_BOOL bBase64, RTS_BOOL bPrivateKey, RtsCryptoKey *pKey);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOIMPORTASYMMETRICKEY_NOTIMPLEMENTED)
	#define USE_CryptoImportAsymmetricKey
	#define EXT_CryptoImportAsymmetricKey
	#define GET_CryptoImportAsymmetricKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoImportAsymmetricKey(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoImportAsymmetricKey  FALSE
	#define EXP_CryptoImportAsymmetricKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoImportAsymmetricKey
	#define EXT_CryptoImportAsymmetricKey
	#define GET_CryptoImportAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoImportAsymmetricKey" ) 
	#define CAL_CryptoImportAsymmetricKey  CryptoImportAsymmetricKey
	#define CHK_CryptoImportAsymmetricKey  TRUE
	#define EXP_CryptoImportAsymmetricKey  CAL_CMEXPAPI( "CryptoImportAsymmetricKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoImportAsymmetricKey
	#define EXT_CryptoImportAsymmetricKey
	#define GET_CryptoImportAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoImportAsymmetricKey" ) 
	#define CAL_CryptoImportAsymmetricKey  CryptoImportAsymmetricKey
	#define CHK_CryptoImportAsymmetricKey  TRUE
	#define EXP_CryptoImportAsymmetricKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoImportAsymmetricKey", (RTS_UINTPTR)CryptoImportAsymmetricKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoImportAsymmetricKey
	#define EXT_CmpCryptoCryptoImportAsymmetricKey
	#define GET_CmpCryptoCryptoImportAsymmetricKey  ERR_OK
	#define CAL_CmpCryptoCryptoImportAsymmetricKey pICmpCrypto->ICryptoImportAsymmetricKey
	#define CHK_CmpCryptoCryptoImportAsymmetricKey (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoImportAsymmetricKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoImportAsymmetricKey
	#define EXT_CryptoImportAsymmetricKey
	#define GET_CryptoImportAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoImportAsymmetricKey" ) 
	#define CAL_CryptoImportAsymmetricKey pICmpCrypto->ICryptoImportAsymmetricKey
	#define CHK_CryptoImportAsymmetricKey (pICmpCrypto != NULL)
	#define EXP_CryptoImportAsymmetricKey  CAL_CMEXPAPI( "CryptoImportAsymmetricKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoImportAsymmetricKey  PFCRYPTOIMPORTASYMMETRICKEY pfCryptoImportAsymmetricKey;
	#define EXT_CryptoImportAsymmetricKey  extern PFCRYPTOIMPORTASYMMETRICKEY pfCryptoImportAsymmetricKey;
	#define GET_CryptoImportAsymmetricKey(fl)  s_pfCMGetAPI2( "CryptoImportAsymmetricKey", (RTS_VOID_FCTPTR *)&pfCryptoImportAsymmetricKey, (fl), 0, 0)
	#define CAL_CryptoImportAsymmetricKey  pfCryptoImportAsymmetricKey
	#define CHK_CryptoImportAsymmetricKey  (pfCryptoImportAsymmetricKey != NULL)
	#define EXP_CryptoImportAsymmetricKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoImportAsymmetricKey", (RTS_UINTPTR)CryptoImportAsymmetricKey, 0, 0) 
#endif




/**
 * <description>Imports a asymmetric key from raw format. This means that no additional 
 *  data like ASN.1 or BASE64 encoding is needed. Not all asymmetric algorithms can import this format.</description>
 * <param name="hAlgo" type="IN">The desired algorithm the key is needed for. Must be one of the following key types:
 *  RTSCRYPTOTYPE_ASYMMETRIC, RTSCRYPTOTYPE_SIGNATURE, RTSCRYPTOTYPE_KEYAGREEMENT</param>
 * <param name="keyData" type="IN">A bytestring containting the raw data.</param>
 * <param name="bPrivateKey" type="IN">TRUE: Import a private key. FALSE:  Import a public key.</param>
 * <param name="pKey" type="INOUT">A pointer to the key for storage of the imported key.</param>
 * <result>Operation result.
 *		- ERR_OK: The key was imported.
 *		- ERR_FAILED: Storage of the key failed.
 *		- ERR_INVALID_STATE: The component is not in a valid state to do this operation.
 *		- RTS_INVALID_HANDLE: The hAlgo was not one of the needed types.
 *		- ERR_PARAMETER: keyData is not valid or pKey was NULL.
 *		- ERR_NOT_SUPPORTED: The given key does not support raw import.
 * </result>
 */	
RTS_RESULT CDECL CryptoImportRawAsymmetricKey(RTS_IEC_HANDLE hAlgo, RtsByteString keyData, RTS_BOOL bPrivateKey, RtsCryptoKey *pKey);
typedef RTS_RESULT (CDECL * PFCRYPTOIMPORTRAWASYMMETRICKEY) (RTS_IEC_HANDLE hAlgo, RtsByteString keyData, RTS_BOOL bPrivateKey, RtsCryptoKey *pKey);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOIMPORTRAWASYMMETRICKEY_NOTIMPLEMENTED)
	#define USE_CryptoImportRawAsymmetricKey
	#define EXT_CryptoImportRawAsymmetricKey
	#define GET_CryptoImportRawAsymmetricKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoImportRawAsymmetricKey(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoImportRawAsymmetricKey  FALSE
	#define EXP_CryptoImportRawAsymmetricKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoImportRawAsymmetricKey
	#define EXT_CryptoImportRawAsymmetricKey
	#define GET_CryptoImportRawAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoImportRawAsymmetricKey" ) 
	#define CAL_CryptoImportRawAsymmetricKey  CryptoImportRawAsymmetricKey
	#define CHK_CryptoImportRawAsymmetricKey  TRUE
	#define EXP_CryptoImportRawAsymmetricKey  CAL_CMEXPAPI( "CryptoImportRawAsymmetricKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoImportRawAsymmetricKey
	#define EXT_CryptoImportRawAsymmetricKey
	#define GET_CryptoImportRawAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoImportRawAsymmetricKey" ) 
	#define CAL_CryptoImportRawAsymmetricKey  CryptoImportRawAsymmetricKey
	#define CHK_CryptoImportRawAsymmetricKey  TRUE
	#define EXP_CryptoImportRawAsymmetricKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoImportRawAsymmetricKey", (RTS_UINTPTR)CryptoImportRawAsymmetricKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoImportRawAsymmetricKey
	#define EXT_CmpCryptoCryptoImportRawAsymmetricKey
	#define GET_CmpCryptoCryptoImportRawAsymmetricKey  ERR_OK
	#define CAL_CmpCryptoCryptoImportRawAsymmetricKey pICmpCrypto->ICryptoImportRawAsymmetricKey
	#define CHK_CmpCryptoCryptoImportRawAsymmetricKey (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoImportRawAsymmetricKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoImportRawAsymmetricKey
	#define EXT_CryptoImportRawAsymmetricKey
	#define GET_CryptoImportRawAsymmetricKey(fl)  CAL_CMGETAPI( "CryptoImportRawAsymmetricKey" ) 
	#define CAL_CryptoImportRawAsymmetricKey pICmpCrypto->ICryptoImportRawAsymmetricKey
	#define CHK_CryptoImportRawAsymmetricKey (pICmpCrypto != NULL)
	#define EXP_CryptoImportRawAsymmetricKey  CAL_CMEXPAPI( "CryptoImportRawAsymmetricKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoImportRawAsymmetricKey  PFCRYPTOIMPORTRAWASYMMETRICKEY pfCryptoImportRawAsymmetricKey;
	#define EXT_CryptoImportRawAsymmetricKey  extern PFCRYPTOIMPORTRAWASYMMETRICKEY pfCryptoImportRawAsymmetricKey;
	#define GET_CryptoImportRawAsymmetricKey(fl)  s_pfCMGetAPI2( "CryptoImportRawAsymmetricKey", (RTS_VOID_FCTPTR *)&pfCryptoImportRawAsymmetricKey, (fl), 0, 0)
	#define CAL_CryptoImportRawAsymmetricKey  pfCryptoImportRawAsymmetricKey
	#define CHK_CryptoImportRawAsymmetricKey  (pfCryptoImportRawAsymmetricKey != NULL)
	#define EXP_CryptoImportRawAsymmetricKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoImportRawAsymmetricKey", (RTS_UINTPTR)CryptoImportRawAsymmetricKey, 0, 0) 
#endif






/**
 * <description>This function can be used to store a private key that was imported or generated within the components internal data store.</description>
 * <param name="key" type="IN">The private key stored.</param>
 * <param name="pKeyIdentifier" type="OUT">A bytestring where the key identifier is stored. This identifier can be used to access 
 *	the key later. The identifier does not contain any sensitive data.</param>
 * <result>Operation result.
 *		- ERR_OK: The key was stored successfully.
 *		- ERR_PARAMETER: key was not valid or pKeyIdentifier is invalid.
 * 		- ERR_NOT_SUPPORTED: The key is not a private key that can be stored.
 *		- ERR_FAILED: Some internal error happened.
 * </result>
 */
RTS_RESULT CDECL CryptoStorePrivateKey(RtsCryptoKey key, RtsByteString *pKeyIdentifier);
typedef RTS_RESULT (CDECL * PFCRYPTOSTOREPRIVATEKEY) (RtsCryptoKey key, RtsByteString *pKeyIdentifier);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOSTOREPRIVATEKEY_NOTIMPLEMENTED)
	#define USE_CryptoStorePrivateKey
	#define EXT_CryptoStorePrivateKey
	#define GET_CryptoStorePrivateKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoStorePrivateKey(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoStorePrivateKey  FALSE
	#define EXP_CryptoStorePrivateKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoStorePrivateKey
	#define EXT_CryptoStorePrivateKey
	#define GET_CryptoStorePrivateKey(fl)  CAL_CMGETAPI( "CryptoStorePrivateKey" ) 
	#define CAL_CryptoStorePrivateKey  CryptoStorePrivateKey
	#define CHK_CryptoStorePrivateKey  TRUE
	#define EXP_CryptoStorePrivateKey  CAL_CMEXPAPI( "CryptoStorePrivateKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoStorePrivateKey
	#define EXT_CryptoStorePrivateKey
	#define GET_CryptoStorePrivateKey(fl)  CAL_CMGETAPI( "CryptoStorePrivateKey" ) 
	#define CAL_CryptoStorePrivateKey  CryptoStorePrivateKey
	#define CHK_CryptoStorePrivateKey  TRUE
	#define EXP_CryptoStorePrivateKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoStorePrivateKey", (RTS_UINTPTR)CryptoStorePrivateKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoStorePrivateKey
	#define EXT_CmpCryptoCryptoStorePrivateKey
	#define GET_CmpCryptoCryptoStorePrivateKey  ERR_OK
	#define CAL_CmpCryptoCryptoStorePrivateKey pICmpCrypto->ICryptoStorePrivateKey
	#define CHK_CmpCryptoCryptoStorePrivateKey (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoStorePrivateKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoStorePrivateKey
	#define EXT_CryptoStorePrivateKey
	#define GET_CryptoStorePrivateKey(fl)  CAL_CMGETAPI( "CryptoStorePrivateKey" ) 
	#define CAL_CryptoStorePrivateKey pICmpCrypto->ICryptoStorePrivateKey
	#define CHK_CryptoStorePrivateKey (pICmpCrypto != NULL)
	#define EXP_CryptoStorePrivateKey  CAL_CMEXPAPI( "CryptoStorePrivateKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoStorePrivateKey  PFCRYPTOSTOREPRIVATEKEY pfCryptoStorePrivateKey;
	#define EXT_CryptoStorePrivateKey  extern PFCRYPTOSTOREPRIVATEKEY pfCryptoStorePrivateKey;
	#define GET_CryptoStorePrivateKey(fl)  s_pfCMGetAPI2( "CryptoStorePrivateKey", (RTS_VOID_FCTPTR *)&pfCryptoStorePrivateKey, (fl), 0, 0)
	#define CAL_CryptoStorePrivateKey  pfCryptoStorePrivateKey
	#define CHK_CryptoStorePrivateKey  (pfCryptoStorePrivateKey != NULL)
	#define EXP_CryptoStorePrivateKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoStorePrivateKey", (RTS_UINTPTR)CryptoStorePrivateKey, 0, 0) 
#endif




/**
 * <description>This function can be used to load a private key from the components internal data store.</description>
 * <param name="keyIdentifier" type="IN">The identifier of the key got from CryptoStorePrivateKey.</param>
 * <param name="pKey" type="OUT">The private key retrieved from the internal storage.</param>
 * <result>Operation result.
 *		- ERR_OK: The key was stored successfully.
 *		- ERR_PARAMETER: keyIdentifier was not valid or pKey == NULL.
 * 		- ERR_NO_OBJECT: No key matching the identifier is available.
 *		- ERR_FAILED: Some internal error happened.
 * </result>
 */
RTS_RESULT CDECL CryptoLoadPrivateKey(RtsByteString keyIdentifier, RtsCryptoKey *pKey);
typedef RTS_RESULT (CDECL * PFCRYPTOLOADPRIVATEKEY) (RtsByteString keyIdentifier, RtsCryptoKey *pKey);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOLOADPRIVATEKEY_NOTIMPLEMENTED)
	#define USE_CryptoLoadPrivateKey
	#define EXT_CryptoLoadPrivateKey
	#define GET_CryptoLoadPrivateKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoLoadPrivateKey(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoLoadPrivateKey  FALSE
	#define EXP_CryptoLoadPrivateKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoLoadPrivateKey
	#define EXT_CryptoLoadPrivateKey
	#define GET_CryptoLoadPrivateKey(fl)  CAL_CMGETAPI( "CryptoLoadPrivateKey" ) 
	#define CAL_CryptoLoadPrivateKey  CryptoLoadPrivateKey
	#define CHK_CryptoLoadPrivateKey  TRUE
	#define EXP_CryptoLoadPrivateKey  CAL_CMEXPAPI( "CryptoLoadPrivateKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoLoadPrivateKey
	#define EXT_CryptoLoadPrivateKey
	#define GET_CryptoLoadPrivateKey(fl)  CAL_CMGETAPI( "CryptoLoadPrivateKey" ) 
	#define CAL_CryptoLoadPrivateKey  CryptoLoadPrivateKey
	#define CHK_CryptoLoadPrivateKey  TRUE
	#define EXP_CryptoLoadPrivateKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoLoadPrivateKey", (RTS_UINTPTR)CryptoLoadPrivateKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoLoadPrivateKey
	#define EXT_CmpCryptoCryptoLoadPrivateKey
	#define GET_CmpCryptoCryptoLoadPrivateKey  ERR_OK
	#define CAL_CmpCryptoCryptoLoadPrivateKey pICmpCrypto->ICryptoLoadPrivateKey
	#define CHK_CmpCryptoCryptoLoadPrivateKey (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoLoadPrivateKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoLoadPrivateKey
	#define EXT_CryptoLoadPrivateKey
	#define GET_CryptoLoadPrivateKey(fl)  CAL_CMGETAPI( "CryptoLoadPrivateKey" ) 
	#define CAL_CryptoLoadPrivateKey pICmpCrypto->ICryptoLoadPrivateKey
	#define CHK_CryptoLoadPrivateKey (pICmpCrypto != NULL)
	#define EXP_CryptoLoadPrivateKey  CAL_CMEXPAPI( "CryptoLoadPrivateKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoLoadPrivateKey  PFCRYPTOLOADPRIVATEKEY pfCryptoLoadPrivateKey;
	#define EXT_CryptoLoadPrivateKey  extern PFCRYPTOLOADPRIVATEKEY pfCryptoLoadPrivateKey;
	#define GET_CryptoLoadPrivateKey(fl)  s_pfCMGetAPI2( "CryptoLoadPrivateKey", (RTS_VOID_FCTPTR *)&pfCryptoLoadPrivateKey, (fl), 0, 0)
	#define CAL_CryptoLoadPrivateKey  pfCryptoLoadPrivateKey
	#define CHK_CryptoLoadPrivateKey  (pfCryptoLoadPrivateKey != NULL)
	#define EXP_CryptoLoadPrivateKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoLoadPrivateKey", (RTS_UINTPTR)CryptoLoadPrivateKey, 0, 0) 
#endif




/**
 * <description>This function can be used to delete a private key from the components internal data store.</description>
 * <param name="keyIdentifier" type="IN">The identifier of the key got from CryptoStorePrivateKey.</param>
 * <result>Operation result.
 *		- ERR_OK: The key was stored successfully.
 *		- ERR_PARAMETER: keyIdentifier was not valid or pKey == NULL.
 * 		- ERR_NO_OBJECT: No key matching the identifier is available.
 *		- ERR_FAILED: Some internal error happened.
 * </result>
 */
RTS_RESULT CDECL CryptoDeletePrivateKey(RtsByteString keyIdentifier);
typedef RTS_RESULT (CDECL * PFCRYPTODELETEPRIVATEKEY) (RtsByteString keyIdentifier);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTODELETEPRIVATEKEY_NOTIMPLEMENTED)
	#define USE_CryptoDeletePrivateKey
	#define EXT_CryptoDeletePrivateKey
	#define GET_CryptoDeletePrivateKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoDeletePrivateKey(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoDeletePrivateKey  FALSE
	#define EXP_CryptoDeletePrivateKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoDeletePrivateKey
	#define EXT_CryptoDeletePrivateKey
	#define GET_CryptoDeletePrivateKey(fl)  CAL_CMGETAPI( "CryptoDeletePrivateKey" ) 
	#define CAL_CryptoDeletePrivateKey  CryptoDeletePrivateKey
	#define CHK_CryptoDeletePrivateKey  TRUE
	#define EXP_CryptoDeletePrivateKey  CAL_CMEXPAPI( "CryptoDeletePrivateKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoDeletePrivateKey
	#define EXT_CryptoDeletePrivateKey
	#define GET_CryptoDeletePrivateKey(fl)  CAL_CMGETAPI( "CryptoDeletePrivateKey" ) 
	#define CAL_CryptoDeletePrivateKey  CryptoDeletePrivateKey
	#define CHK_CryptoDeletePrivateKey  TRUE
	#define EXP_CryptoDeletePrivateKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoDeletePrivateKey", (RTS_UINTPTR)CryptoDeletePrivateKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoDeletePrivateKey
	#define EXT_CmpCryptoCryptoDeletePrivateKey
	#define GET_CmpCryptoCryptoDeletePrivateKey  ERR_OK
	#define CAL_CmpCryptoCryptoDeletePrivateKey pICmpCrypto->ICryptoDeletePrivateKey
	#define CHK_CmpCryptoCryptoDeletePrivateKey (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoDeletePrivateKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoDeletePrivateKey
	#define EXT_CryptoDeletePrivateKey
	#define GET_CryptoDeletePrivateKey(fl)  CAL_CMGETAPI( "CryptoDeletePrivateKey" ) 
	#define CAL_CryptoDeletePrivateKey pICmpCrypto->ICryptoDeletePrivateKey
	#define CHK_CryptoDeletePrivateKey (pICmpCrypto != NULL)
	#define EXP_CryptoDeletePrivateKey  CAL_CMEXPAPI( "CryptoDeletePrivateKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoDeletePrivateKey  PFCRYPTODELETEPRIVATEKEY pfCryptoDeletePrivateKey;
	#define EXT_CryptoDeletePrivateKey  extern PFCRYPTODELETEPRIVATEKEY pfCryptoDeletePrivateKey;
	#define GET_CryptoDeletePrivateKey(fl)  s_pfCMGetAPI2( "CryptoDeletePrivateKey", (RTS_VOID_FCTPTR *)&pfCryptoDeletePrivateKey, (fl), 0, 0)
	#define CAL_CryptoDeletePrivateKey  pfCryptoDeletePrivateKey
	#define CHK_CryptoDeletePrivateKey  (pfCryptoDeletePrivateKey != NULL)
	#define EXP_CryptoDeletePrivateKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoDeletePrivateKey", (RTS_UINTPTR)CryptoDeletePrivateKey, 0, 0) 
#endif




/**
 * <description>Calculate the message digest of the given data.</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pData" type="IN">Data to calculate the message digest.</param>
 * <param name="pHash" type="OUT">Calculated message digest.</param>
 * <result>Result of the operation</result>
 */
RTS_RESULT CDECL CryptoGenerateHash(RTS_HANDLE hAlgo, RtsByteString* pData, RtsByteString* pHash);
typedef RTS_RESULT (CDECL * PFCRYPTOGENERATEHASH) (RTS_HANDLE hAlgo, RtsByteString* pData, RtsByteString* pHash);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGENERATEHASH_NOTIMPLEMENTED)
	#define USE_CryptoGenerateHash
	#define EXT_CryptoGenerateHash
	#define GET_CryptoGenerateHash(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoGenerateHash(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoGenerateHash  FALSE
	#define EXP_CryptoGenerateHash  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoGenerateHash
	#define EXT_CryptoGenerateHash
	#define GET_CryptoGenerateHash(fl)  CAL_CMGETAPI( "CryptoGenerateHash" ) 
	#define CAL_CryptoGenerateHash  CryptoGenerateHash
	#define CHK_CryptoGenerateHash  TRUE
	#define EXP_CryptoGenerateHash  CAL_CMEXPAPI( "CryptoGenerateHash" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoGenerateHash
	#define EXT_CryptoGenerateHash
	#define GET_CryptoGenerateHash(fl)  CAL_CMGETAPI( "CryptoGenerateHash" ) 
	#define CAL_CryptoGenerateHash  CryptoGenerateHash
	#define CHK_CryptoGenerateHash  TRUE
	#define EXP_CryptoGenerateHash  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGenerateHash", (RTS_UINTPTR)CryptoGenerateHash, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoGenerateHash
	#define EXT_CmpCryptoCryptoGenerateHash
	#define GET_CmpCryptoCryptoGenerateHash  ERR_OK
	#define CAL_CmpCryptoCryptoGenerateHash pICmpCrypto->ICryptoGenerateHash
	#define CHK_CmpCryptoCryptoGenerateHash (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoGenerateHash  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoGenerateHash
	#define EXT_CryptoGenerateHash
	#define GET_CryptoGenerateHash(fl)  CAL_CMGETAPI( "CryptoGenerateHash" ) 
	#define CAL_CryptoGenerateHash pICmpCrypto->ICryptoGenerateHash
	#define CHK_CryptoGenerateHash (pICmpCrypto != NULL)
	#define EXP_CryptoGenerateHash  CAL_CMEXPAPI( "CryptoGenerateHash" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoGenerateHash  PFCRYPTOGENERATEHASH pfCryptoGenerateHash;
	#define EXT_CryptoGenerateHash  extern PFCRYPTOGENERATEHASH pfCryptoGenerateHash;
	#define GET_CryptoGenerateHash(fl)  s_pfCMGetAPI2( "CryptoGenerateHash", (RTS_VOID_FCTPTR *)&pfCryptoGenerateHash, (fl), 0, 0)
	#define CAL_CryptoGenerateHash  pfCryptoGenerateHash
	#define CHK_CryptoGenerateHash  (pfCryptoGenerateHash != NULL)
	#define EXP_CryptoGenerateHash  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGenerateHash", (RTS_UINTPTR)CryptoGenerateHash, 0, 0) 
#endif




/**
 * <description>This function derives a new key from an existing secret (key) and an corresponding salt. 
 * The functions generates keys of the requested size which is given in pDerivedKey->ui32MaxLen. If setup 
 * properly this function provides a secure way to store passwords within the file system. By now two
 * classes of algorithms can be use:
 *	1. P_SHA(1, 256): This algorithm can be used to derive session keys from some kind of secret data. 
 *		If this algorithm is used the pSpecificParameters is not evaluated an should be NULL. This algorithm is quite fast.
 *	2. SCRYPT: This algorithm can be used to generate strong keys from weak passwords, or to store passwords 
 *		in a secure way within the file system. This algorithm makes use of the pSpecificParameters and requires this parameter.
 * 		Depending on the configuration given in pSpecificParameters this algorithm is time and memory consuming.
 *		For details see RFC-7914.
 * </description>
 * <param name="hAlgo" type="IN">Handle of the used algorithm. Should be one of RTSCRYPTOID_KDF_* algorithms.</param>
 * <param name="pKey" type="IN">The key that should be derived. The function reads pKey->ui32Len bytes of the given buffer.</param>
 * <param name="pSalt" type="IN">The SALT that should be used to derive the key. The function reads pSalt->ui32Len bytes of the given buffer.</param>
 * <param name="pSpecificParameters" type="IN">Some additional algorithm specific parameters. By now only used for scrypt.</param>
 * <param name="pDerivedKey" type="IN">Destination where to store the derived key. The function generates a key with the size of pDerivedKey->ui32MaxLen.
 *	 The size of the derived key will be stored in pDerivedKey->ui32Len.</param>
 * <result>Operation result.
 *		- ERR_OK: The key was derived successfully.
 *		- ERR_INVALID_HANDLE: hAlgo was not a valid handle to derive keys.
 *		- ERR_PARAMETER: One of the other parameters was invalid.
 * 		- ERR_FAILED: some internal error happened.
 * </result>
 */
RTS_RESULT CDECL CryptoDeriveKey(RTS_HANDLE hAlgo, RtsByteString *pKey, RtsByteString *pSalt, RtsKdfParameter* pSpecificParameters, RtsByteString *pDerivedKey);
typedef RTS_RESULT (CDECL * PFCRYPTODERIVEKEY) (RTS_HANDLE hAlgo, RtsByteString *pKey, RtsByteString *pSalt, RtsKdfParameter* pSpecificParameters, RtsByteString *pDerivedKey);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTODERIVEKEY_NOTIMPLEMENTED)
	#define USE_CryptoDeriveKey
	#define EXT_CryptoDeriveKey
	#define GET_CryptoDeriveKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoDeriveKey(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoDeriveKey  FALSE
	#define EXP_CryptoDeriveKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoDeriveKey
	#define EXT_CryptoDeriveKey
	#define GET_CryptoDeriveKey(fl)  CAL_CMGETAPI( "CryptoDeriveKey" ) 
	#define CAL_CryptoDeriveKey  CryptoDeriveKey
	#define CHK_CryptoDeriveKey  TRUE
	#define EXP_CryptoDeriveKey  CAL_CMEXPAPI( "CryptoDeriveKey" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoDeriveKey
	#define EXT_CryptoDeriveKey
	#define GET_CryptoDeriveKey(fl)  CAL_CMGETAPI( "CryptoDeriveKey" ) 
	#define CAL_CryptoDeriveKey  CryptoDeriveKey
	#define CHK_CryptoDeriveKey  TRUE
	#define EXP_CryptoDeriveKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoDeriveKey", (RTS_UINTPTR)CryptoDeriveKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoDeriveKey
	#define EXT_CmpCryptoCryptoDeriveKey
	#define GET_CmpCryptoCryptoDeriveKey  ERR_OK
	#define CAL_CmpCryptoCryptoDeriveKey pICmpCrypto->ICryptoDeriveKey
	#define CHK_CmpCryptoCryptoDeriveKey (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoDeriveKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoDeriveKey
	#define EXT_CryptoDeriveKey
	#define GET_CryptoDeriveKey(fl)  CAL_CMGETAPI( "CryptoDeriveKey" ) 
	#define CAL_CryptoDeriveKey pICmpCrypto->ICryptoDeriveKey
	#define CHK_CryptoDeriveKey (pICmpCrypto != NULL)
	#define EXP_CryptoDeriveKey  CAL_CMEXPAPI( "CryptoDeriveKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoDeriveKey  PFCRYPTODERIVEKEY pfCryptoDeriveKey;
	#define EXT_CryptoDeriveKey  extern PFCRYPTODERIVEKEY pfCryptoDeriveKey;
	#define GET_CryptoDeriveKey(fl)  s_pfCMGetAPI2( "CryptoDeriveKey", (RTS_VOID_FCTPTR *)&pfCryptoDeriveKey, (fl), 0, 0)
	#define CAL_CryptoDeriveKey  pfCryptoDeriveKey
	#define CHK_CryptoDeriveKey  (pfCryptoDeriveKey != NULL)
	#define EXP_CryptoDeriveKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoDeriveKey", (RTS_UINTPTR)CryptoDeriveKey, 0, 0) 
#endif




/**
 * <description>This function can be used to run asymmetric key agreement protocols like Diffie-Hellman 
 *  (DH) or elliptic curve Diffie-Hellman (ECDH). These procols can be used to agree on secret keys via an
 *  insecure channel. The algorithm can not guarantee authenticity of the communication partner.
 *  Therefore, a man in the middle attack can not be detected or prevented during execution of the protocol. The strength of 
 *  the new key depends on the chosen algorithm. It is recommended to derive a session key from this secret key.
 * </description>
 * <param name="hAlgo" type="IN">Handle of the used algorithm. Should be one of the RTSCRYPTOTYPE_KEYAGREEMENT algorithms.</param>
 * <param name="privateKey" type="IN">The local private key to run the operation.</param>
 * <param name="peerPublicKey" type="IN">The public key of the communication partner.</param>
 * <param name="pSharedSecret" type="IN">The calculated shared secret key.</param>
 * <result>Operation result:
 *		- ERR_OK: Key agreement was successful.
 *		- ERR_INVALID_STATE: The component is not in a valid state to do this operation.
 *		- RTS_INVALID_HANDLE: The hAlgo was not of type RTSCRYPTOTYPE_KEYAGREEMENT.
 *		- ERR_PARAMETER: The given keys where invalid or pSharedSecret was NULL.
 *		- ERR_SIZE_MISMATCH: The given bytestring is too small.
 *		- ERR_FAILED: The operation failed of some internal error.
 *		- ERR_NOT_SUPPORTED: The given algorithm or key does not support this operation.
 * </result>
 */
RTS_RESULT CDECL CryptoKeyAgreement(RTS_HANDLE hAlgo, RtsCryptoKey privateKey, RtsCryptoKey peerPublicKey, RtsByteString *pSharedSecret);
typedef RTS_RESULT (CDECL * PFCRYPTOKEYAGREEMENT) (RTS_HANDLE hAlgo, RtsCryptoKey privateKey, RtsCryptoKey peerPublicKey, RtsByteString *pSharedSecret);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOKEYAGREEMENT_NOTIMPLEMENTED)
	#define USE_CryptoKeyAgreement
	#define EXT_CryptoKeyAgreement
	#define GET_CryptoKeyAgreement(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoKeyAgreement(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoKeyAgreement  FALSE
	#define EXP_CryptoKeyAgreement  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoKeyAgreement
	#define EXT_CryptoKeyAgreement
	#define GET_CryptoKeyAgreement(fl)  CAL_CMGETAPI( "CryptoKeyAgreement" ) 
	#define CAL_CryptoKeyAgreement  CryptoKeyAgreement
	#define CHK_CryptoKeyAgreement  TRUE
	#define EXP_CryptoKeyAgreement  CAL_CMEXPAPI( "CryptoKeyAgreement" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoKeyAgreement
	#define EXT_CryptoKeyAgreement
	#define GET_CryptoKeyAgreement(fl)  CAL_CMGETAPI( "CryptoKeyAgreement" ) 
	#define CAL_CryptoKeyAgreement  CryptoKeyAgreement
	#define CHK_CryptoKeyAgreement  TRUE
	#define EXP_CryptoKeyAgreement  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoKeyAgreement", (RTS_UINTPTR)CryptoKeyAgreement, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoKeyAgreement
	#define EXT_CmpCryptoCryptoKeyAgreement
	#define GET_CmpCryptoCryptoKeyAgreement  ERR_OK
	#define CAL_CmpCryptoCryptoKeyAgreement pICmpCrypto->ICryptoKeyAgreement
	#define CHK_CmpCryptoCryptoKeyAgreement (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoKeyAgreement  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoKeyAgreement
	#define EXT_CryptoKeyAgreement
	#define GET_CryptoKeyAgreement(fl)  CAL_CMGETAPI( "CryptoKeyAgreement" ) 
	#define CAL_CryptoKeyAgreement pICmpCrypto->ICryptoKeyAgreement
	#define CHK_CryptoKeyAgreement (pICmpCrypto != NULL)
	#define EXP_CryptoKeyAgreement  CAL_CMEXPAPI( "CryptoKeyAgreement" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoKeyAgreement  PFCRYPTOKEYAGREEMENT pfCryptoKeyAgreement;
	#define EXT_CryptoKeyAgreement  extern PFCRYPTOKEYAGREEMENT pfCryptoKeyAgreement;
	#define GET_CryptoKeyAgreement(fl)  s_pfCMGetAPI2( "CryptoKeyAgreement", (RTS_VOID_FCTPTR *)&pfCryptoKeyAgreement, (fl), 0, 0)
	#define CAL_CryptoKeyAgreement  pfCryptoKeyAgreement
	#define CHK_CryptoKeyAgreement  (pfCryptoKeyAgreement != NULL)
	#define EXP_CryptoKeyAgreement  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoKeyAgreement", (RTS_UINTPTR)CryptoKeyAgreement, 0, 0) 
#endif






/**
 * <description>Generate a hashed message authentication code (HMAC) of the given data and key.</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pData" type="IN">Data to calculate the HMAC.</param>
 * <param name="key" type="IN">Key to be used for the HMAC calculation</param>
 * <param name="pSignature" type="OUT">Calculated HMAC</param>
 * <result>Error code Result of the operation</result>
 **/
RTS_RESULT CDECL CryptoHMACSign(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey key, RtsByteString* pSignature);
typedef RTS_RESULT (CDECL * PFCRYPTOHMACSIGN) (RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey key, RtsByteString* pSignature);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOHMACSIGN_NOTIMPLEMENTED)
	#define USE_CryptoHMACSign
	#define EXT_CryptoHMACSign
	#define GET_CryptoHMACSign(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoHMACSign(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoHMACSign  FALSE
	#define EXP_CryptoHMACSign  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoHMACSign
	#define EXT_CryptoHMACSign
	#define GET_CryptoHMACSign(fl)  CAL_CMGETAPI( "CryptoHMACSign" ) 
	#define CAL_CryptoHMACSign  CryptoHMACSign
	#define CHK_CryptoHMACSign  TRUE
	#define EXP_CryptoHMACSign  CAL_CMEXPAPI( "CryptoHMACSign" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoHMACSign
	#define EXT_CryptoHMACSign
	#define GET_CryptoHMACSign(fl)  CAL_CMGETAPI( "CryptoHMACSign" ) 
	#define CAL_CryptoHMACSign  CryptoHMACSign
	#define CHK_CryptoHMACSign  TRUE
	#define EXP_CryptoHMACSign  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoHMACSign", (RTS_UINTPTR)CryptoHMACSign, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoHMACSign
	#define EXT_CmpCryptoCryptoHMACSign
	#define GET_CmpCryptoCryptoHMACSign  ERR_OK
	#define CAL_CmpCryptoCryptoHMACSign pICmpCrypto->ICryptoHMACSign
	#define CHK_CmpCryptoCryptoHMACSign (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoHMACSign  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoHMACSign
	#define EXT_CryptoHMACSign
	#define GET_CryptoHMACSign(fl)  CAL_CMGETAPI( "CryptoHMACSign" ) 
	#define CAL_CryptoHMACSign pICmpCrypto->ICryptoHMACSign
	#define CHK_CryptoHMACSign (pICmpCrypto != NULL)
	#define EXP_CryptoHMACSign  CAL_CMEXPAPI( "CryptoHMACSign" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoHMACSign  PFCRYPTOHMACSIGN pfCryptoHMACSign;
	#define EXT_CryptoHMACSign  extern PFCRYPTOHMACSIGN pfCryptoHMACSign;
	#define GET_CryptoHMACSign(fl)  s_pfCMGetAPI2( "CryptoHMACSign", (RTS_VOID_FCTPTR *)&pfCryptoHMACSign, (fl), 0, 0)
	#define CAL_CryptoHMACSign  pfCryptoHMACSign
	#define CHK_CryptoHMACSign  (pfCryptoHMACSign != NULL)
	#define EXP_CryptoHMACSign  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoHMACSign", (RTS_UINTPTR)CryptoHMACSign, 0, 0) 
#endif




/**
 * <description>Verify a received hashed message authentication code (HMAC).</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pData" type="IN">Data to calculate the HMAC.</param>
 * <param name="key" type="IN">Key to be used for the HMAC calculation</param>
 * <param name="pSignature" type="IN">Received HMAC</param>
 * <result>Error Code Function returns ERR_OK if the HMAC is valid.</result>
 **/
RTS_RESULT CDECL CryptoHMACVerify(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey key, RtsByteString* pSignature);
typedef RTS_RESULT (CDECL * PFCRYPTOHMACVERIFY) (RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey key, RtsByteString* pSignature);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOHMACVERIFY_NOTIMPLEMENTED)
	#define USE_CryptoHMACVerify
	#define EXT_CryptoHMACVerify
	#define GET_CryptoHMACVerify(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoHMACVerify(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoHMACVerify  FALSE
	#define EXP_CryptoHMACVerify  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoHMACVerify
	#define EXT_CryptoHMACVerify
	#define GET_CryptoHMACVerify(fl)  CAL_CMGETAPI( "CryptoHMACVerify" ) 
	#define CAL_CryptoHMACVerify  CryptoHMACVerify
	#define CHK_CryptoHMACVerify  TRUE
	#define EXP_CryptoHMACVerify  CAL_CMEXPAPI( "CryptoHMACVerify" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoHMACVerify
	#define EXT_CryptoHMACVerify
	#define GET_CryptoHMACVerify(fl)  CAL_CMGETAPI( "CryptoHMACVerify" ) 
	#define CAL_CryptoHMACVerify  CryptoHMACVerify
	#define CHK_CryptoHMACVerify  TRUE
	#define EXP_CryptoHMACVerify  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoHMACVerify", (RTS_UINTPTR)CryptoHMACVerify, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoHMACVerify
	#define EXT_CmpCryptoCryptoHMACVerify
	#define GET_CmpCryptoCryptoHMACVerify  ERR_OK
	#define CAL_CmpCryptoCryptoHMACVerify pICmpCrypto->ICryptoHMACVerify
	#define CHK_CmpCryptoCryptoHMACVerify (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoHMACVerify  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoHMACVerify
	#define EXT_CryptoHMACVerify
	#define GET_CryptoHMACVerify(fl)  CAL_CMGETAPI( "CryptoHMACVerify" ) 
	#define CAL_CryptoHMACVerify pICmpCrypto->ICryptoHMACVerify
	#define CHK_CryptoHMACVerify (pICmpCrypto != NULL)
	#define EXP_CryptoHMACVerify  CAL_CMEXPAPI( "CryptoHMACVerify" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoHMACVerify  PFCRYPTOHMACVERIFY pfCryptoHMACVerify;
	#define EXT_CryptoHMACVerify  extern PFCRYPTOHMACVERIFY pfCryptoHMACVerify;
	#define GET_CryptoHMACVerify(fl)  s_pfCMGetAPI2( "CryptoHMACVerify", (RTS_VOID_FCTPTR *)&pfCryptoHMACVerify, (fl), 0, 0)
	#define CAL_CryptoHMACVerify  pfCryptoHMACVerify
	#define CHK_CryptoHMACVerify  (pfCryptoHMACVerify != NULL)
	#define EXP_CryptoHMACVerify  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoHMACVerify", (RTS_UINTPTR)CryptoHMACVerify, 0, 0) 
#endif




/**
 * <description>Sign the data using a specific message digest and the private key.</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pData" type="IN">Data to be signed.</param>
 * <param name="privateKey" type="IN">Key to be used signing. Has to be a private key. Use X509CertGetPrivateKey to get the private key.</param>
 * <param name="pSignature" type="IN">Calculated signature of the data.</param>
 * <result>Error code: Result of the operation.</result>
 **/
RTS_RESULT CDECL CryptoSignatureGenerate(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey privateKey, RtsByteString* pSignature);
typedef RTS_RESULT (CDECL * PFCRYPTOSIGNATUREGENERATE) (RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey privateKey, RtsByteString* pSignature);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOSIGNATUREGENERATE_NOTIMPLEMENTED)
	#define USE_CryptoSignatureGenerate
	#define EXT_CryptoSignatureGenerate
	#define GET_CryptoSignatureGenerate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoSignatureGenerate(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoSignatureGenerate  FALSE
	#define EXP_CryptoSignatureGenerate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoSignatureGenerate
	#define EXT_CryptoSignatureGenerate
	#define GET_CryptoSignatureGenerate(fl)  CAL_CMGETAPI( "CryptoSignatureGenerate" ) 
	#define CAL_CryptoSignatureGenerate  CryptoSignatureGenerate
	#define CHK_CryptoSignatureGenerate  TRUE
	#define EXP_CryptoSignatureGenerate  CAL_CMEXPAPI( "CryptoSignatureGenerate" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoSignatureGenerate
	#define EXT_CryptoSignatureGenerate
	#define GET_CryptoSignatureGenerate(fl)  CAL_CMGETAPI( "CryptoSignatureGenerate" ) 
	#define CAL_CryptoSignatureGenerate  CryptoSignatureGenerate
	#define CHK_CryptoSignatureGenerate  TRUE
	#define EXP_CryptoSignatureGenerate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoSignatureGenerate", (RTS_UINTPTR)CryptoSignatureGenerate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoSignatureGenerate
	#define EXT_CmpCryptoCryptoSignatureGenerate
	#define GET_CmpCryptoCryptoSignatureGenerate  ERR_OK
	#define CAL_CmpCryptoCryptoSignatureGenerate pICmpCrypto->ICryptoSignatureGenerate
	#define CHK_CmpCryptoCryptoSignatureGenerate (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoSignatureGenerate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoSignatureGenerate
	#define EXT_CryptoSignatureGenerate
	#define GET_CryptoSignatureGenerate(fl)  CAL_CMGETAPI( "CryptoSignatureGenerate" ) 
	#define CAL_CryptoSignatureGenerate pICmpCrypto->ICryptoSignatureGenerate
	#define CHK_CryptoSignatureGenerate (pICmpCrypto != NULL)
	#define EXP_CryptoSignatureGenerate  CAL_CMEXPAPI( "CryptoSignatureGenerate" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoSignatureGenerate  PFCRYPTOSIGNATUREGENERATE pfCryptoSignatureGenerate;
	#define EXT_CryptoSignatureGenerate  extern PFCRYPTOSIGNATUREGENERATE pfCryptoSignatureGenerate;
	#define GET_CryptoSignatureGenerate(fl)  s_pfCMGetAPI2( "CryptoSignatureGenerate", (RTS_VOID_FCTPTR *)&pfCryptoSignatureGenerate, (fl), 0, 0)
	#define CAL_CryptoSignatureGenerate  pfCryptoSignatureGenerate
	#define CHK_CryptoSignatureGenerate  (pfCryptoSignatureGenerate != NULL)
	#define EXP_CryptoSignatureGenerate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoSignatureGenerate", (RTS_UINTPTR)CryptoSignatureGenerate, 0, 0) 
#endif




/**
 * <description>Verify a received signature</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pData" type="IN">Data to verify.</param>
 * <param name="publicKey" type="IN">Public key of the sender. X509CertGetPublicKey to get the key.</param>
 * <param name="pSignature" type="IN">Received signature.</param>
 * <result>Error code: Function returns ERR_OK if the signature is valid.</result>
 **/
RTS_RESULT CDECL CryptoSignatureVerify(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey publicKey, RtsByteString* pSignature);
typedef RTS_RESULT (CDECL * PFCRYPTOSIGNATUREVERIFY) (RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey publicKey, RtsByteString* pSignature);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOSIGNATUREVERIFY_NOTIMPLEMENTED)
	#define USE_CryptoSignatureVerify
	#define EXT_CryptoSignatureVerify
	#define GET_CryptoSignatureVerify(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoSignatureVerify(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoSignatureVerify  FALSE
	#define EXP_CryptoSignatureVerify  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoSignatureVerify
	#define EXT_CryptoSignatureVerify
	#define GET_CryptoSignatureVerify(fl)  CAL_CMGETAPI( "CryptoSignatureVerify" ) 
	#define CAL_CryptoSignatureVerify  CryptoSignatureVerify
	#define CHK_CryptoSignatureVerify  TRUE
	#define EXP_CryptoSignatureVerify  CAL_CMEXPAPI( "CryptoSignatureVerify" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoSignatureVerify
	#define EXT_CryptoSignatureVerify
	#define GET_CryptoSignatureVerify(fl)  CAL_CMGETAPI( "CryptoSignatureVerify" ) 
	#define CAL_CryptoSignatureVerify  CryptoSignatureVerify
	#define CHK_CryptoSignatureVerify  TRUE
	#define EXP_CryptoSignatureVerify  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoSignatureVerify", (RTS_UINTPTR)CryptoSignatureVerify, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoSignatureVerify
	#define EXT_CmpCryptoCryptoSignatureVerify
	#define GET_CmpCryptoCryptoSignatureVerify  ERR_OK
	#define CAL_CmpCryptoCryptoSignatureVerify pICmpCrypto->ICryptoSignatureVerify
	#define CHK_CmpCryptoCryptoSignatureVerify (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoSignatureVerify  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoSignatureVerify
	#define EXT_CryptoSignatureVerify
	#define GET_CryptoSignatureVerify(fl)  CAL_CMGETAPI( "CryptoSignatureVerify" ) 
	#define CAL_CryptoSignatureVerify pICmpCrypto->ICryptoSignatureVerify
	#define CHK_CryptoSignatureVerify (pICmpCrypto != NULL)
	#define EXP_CryptoSignatureVerify  CAL_CMEXPAPI( "CryptoSignatureVerify" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoSignatureVerify  PFCRYPTOSIGNATUREVERIFY pfCryptoSignatureVerify;
	#define EXT_CryptoSignatureVerify  extern PFCRYPTOSIGNATUREVERIFY pfCryptoSignatureVerify;
	#define GET_CryptoSignatureVerify(fl)  s_pfCMGetAPI2( "CryptoSignatureVerify", (RTS_VOID_FCTPTR *)&pfCryptoSignatureVerify, (fl), 0, 0)
	#define CAL_CryptoSignatureVerify  pfCryptoSignatureVerify
	#define CHK_CryptoSignatureVerify  (pfCryptoSignatureVerify != NULL)
	#define EXP_CryptoSignatureVerify  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoSignatureVerify", (RTS_UINTPTR)CryptoSignatureVerify, 0, 0) 
#endif




/**
 * <description>Generate a number of cryptographic strong bytes.</description>
 * <param name="ui32NumOfRandomBytes" type="IN">Number of bytes needed.</param>
 * <param name="pRandom" type="IN">Buffer to store the bytes.</param>
 * <result>Error code: Function returns ERR_OK if the bytes could be generated</result>
 **/
RTS_RESULT CDECL CryptoGenerateRandomNumber(RTS_UI32 ui32NumOfRandomBytes, RtsByteString* pRandom);
typedef RTS_RESULT (CDECL * PFCRYPTOGENERATERANDOMNUMBER) (RTS_UI32 ui32NumOfRandomBytes, RtsByteString* pRandom);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGENERATERANDOMNUMBER_NOTIMPLEMENTED)
	#define USE_CryptoGenerateRandomNumber
	#define EXT_CryptoGenerateRandomNumber
	#define GET_CryptoGenerateRandomNumber(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoGenerateRandomNumber(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoGenerateRandomNumber  FALSE
	#define EXP_CryptoGenerateRandomNumber  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoGenerateRandomNumber
	#define EXT_CryptoGenerateRandomNumber
	#define GET_CryptoGenerateRandomNumber(fl)  CAL_CMGETAPI( "CryptoGenerateRandomNumber" ) 
	#define CAL_CryptoGenerateRandomNumber  CryptoGenerateRandomNumber
	#define CHK_CryptoGenerateRandomNumber  TRUE
	#define EXP_CryptoGenerateRandomNumber  CAL_CMEXPAPI( "CryptoGenerateRandomNumber" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoGenerateRandomNumber
	#define EXT_CryptoGenerateRandomNumber
	#define GET_CryptoGenerateRandomNumber(fl)  CAL_CMGETAPI( "CryptoGenerateRandomNumber" ) 
	#define CAL_CryptoGenerateRandomNumber  CryptoGenerateRandomNumber
	#define CHK_CryptoGenerateRandomNumber  TRUE
	#define EXP_CryptoGenerateRandomNumber  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGenerateRandomNumber", (RTS_UINTPTR)CryptoGenerateRandomNumber, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoGenerateRandomNumber
	#define EXT_CmpCryptoCryptoGenerateRandomNumber
	#define GET_CmpCryptoCryptoGenerateRandomNumber  ERR_OK
	#define CAL_CmpCryptoCryptoGenerateRandomNumber pICmpCrypto->ICryptoGenerateRandomNumber
	#define CHK_CmpCryptoCryptoGenerateRandomNumber (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoGenerateRandomNumber  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoGenerateRandomNumber
	#define EXT_CryptoGenerateRandomNumber
	#define GET_CryptoGenerateRandomNumber(fl)  CAL_CMGETAPI( "CryptoGenerateRandomNumber" ) 
	#define CAL_CryptoGenerateRandomNumber pICmpCrypto->ICryptoGenerateRandomNumber
	#define CHK_CryptoGenerateRandomNumber (pICmpCrypto != NULL)
	#define EXP_CryptoGenerateRandomNumber  CAL_CMEXPAPI( "CryptoGenerateRandomNumber" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoGenerateRandomNumber  PFCRYPTOGENERATERANDOMNUMBER pfCryptoGenerateRandomNumber;
	#define EXT_CryptoGenerateRandomNumber  extern PFCRYPTOGENERATERANDOMNUMBER pfCryptoGenerateRandomNumber;
	#define GET_CryptoGenerateRandomNumber(fl)  s_pfCMGetAPI2( "CryptoGenerateRandomNumber", (RTS_VOID_FCTPTR *)&pfCryptoGenerateRandomNumber, (fl), 0, 0)
	#define CAL_CryptoGenerateRandomNumber  pfCryptoGenerateRandomNumber
	#define CHK_CryptoGenerateRandomNumber  (pfCryptoGenerateRandomNumber != NULL)
	#define EXP_CryptoGenerateRandomNumber  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoGenerateRandomNumber", (RTS_UINTPTR)CryptoGenerateRandomNumber, 0, 0) 
#endif




/**
 * <description>Initialize a cryptographic key.</description>
 * <param name="pKey" type="IN">Pointer to the key to be initialized.</param>
 * <param name="key" type="IN">Content of the key.</param>
 * <param name="type" type="IN">Type of the key.</param>
 * <result>Operation result.</result>
 **/
RTS_RESULT CDECL CryptoKeyInit(RtsCryptoKey* pKey, RtsCryptoKeyStorage key, RTSCRYPTOKEYTYPE keyType);
typedef RTS_RESULT (CDECL * PFCRYPTOKEYINIT) (RtsCryptoKey* pKey, RtsCryptoKeyStorage key, RTSCRYPTOKEYTYPE keyType);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOKEYINIT_NOTIMPLEMENTED)
	#define USE_CryptoKeyInit
	#define EXT_CryptoKeyInit
	#define GET_CryptoKeyInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoKeyInit(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoKeyInit  FALSE
	#define EXP_CryptoKeyInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoKeyInit
	#define EXT_CryptoKeyInit
	#define GET_CryptoKeyInit(fl)  CAL_CMGETAPI( "CryptoKeyInit" ) 
	#define CAL_CryptoKeyInit  CryptoKeyInit
	#define CHK_CryptoKeyInit  TRUE
	#define EXP_CryptoKeyInit  CAL_CMEXPAPI( "CryptoKeyInit" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoKeyInit
	#define EXT_CryptoKeyInit
	#define GET_CryptoKeyInit(fl)  CAL_CMGETAPI( "CryptoKeyInit" ) 
	#define CAL_CryptoKeyInit  CryptoKeyInit
	#define CHK_CryptoKeyInit  TRUE
	#define EXP_CryptoKeyInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoKeyInit", (RTS_UINTPTR)CryptoKeyInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoKeyInit
	#define EXT_CmpCryptoCryptoKeyInit
	#define GET_CmpCryptoCryptoKeyInit  ERR_OK
	#define CAL_CmpCryptoCryptoKeyInit pICmpCrypto->ICryptoKeyInit
	#define CHK_CmpCryptoCryptoKeyInit (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoKeyInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoKeyInit
	#define EXT_CryptoKeyInit
	#define GET_CryptoKeyInit(fl)  CAL_CMGETAPI( "CryptoKeyInit" ) 
	#define CAL_CryptoKeyInit pICmpCrypto->ICryptoKeyInit
	#define CHK_CryptoKeyInit (pICmpCrypto != NULL)
	#define EXP_CryptoKeyInit  CAL_CMEXPAPI( "CryptoKeyInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoKeyInit  PFCRYPTOKEYINIT pfCryptoKeyInit;
	#define EXT_CryptoKeyInit  extern PFCRYPTOKEYINIT pfCryptoKeyInit;
	#define GET_CryptoKeyInit(fl)  s_pfCMGetAPI2( "CryptoKeyInit", (RTS_VOID_FCTPTR *)&pfCryptoKeyInit, (fl), 0, 0)
	#define CAL_CryptoKeyInit  pfCryptoKeyInit
	#define CHK_CryptoKeyInit  (pfCryptoKeyInit != NULL)
	#define EXP_CryptoKeyInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoKeyInit", (RTS_UINTPTR)CryptoKeyInit, 0, 0) 
#endif




/**
 * <description>Clean up a cryptographic key.</description>
 * <param name="pKey" type="IN">Key to be clean up.</param>
 * <result>Operation result.</result>
 **/
RTS_RESULT CDECL CryptoKeyExit(RtsCryptoKey* pKey);
typedef RTS_RESULT (CDECL * PFCRYPTOKEYEXIT) (RtsCryptoKey* pKey);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOKEYEXIT_NOTIMPLEMENTED)
	#define USE_CryptoKeyExit
	#define EXT_CryptoKeyExit
	#define GET_CryptoKeyExit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoKeyExit(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoKeyExit  FALSE
	#define EXP_CryptoKeyExit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoKeyExit
	#define EXT_CryptoKeyExit
	#define GET_CryptoKeyExit(fl)  CAL_CMGETAPI( "CryptoKeyExit" ) 
	#define CAL_CryptoKeyExit  CryptoKeyExit
	#define CHK_CryptoKeyExit  TRUE
	#define EXP_CryptoKeyExit  CAL_CMEXPAPI( "CryptoKeyExit" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoKeyExit
	#define EXT_CryptoKeyExit
	#define GET_CryptoKeyExit(fl)  CAL_CMGETAPI( "CryptoKeyExit" ) 
	#define CAL_CryptoKeyExit  CryptoKeyExit
	#define CHK_CryptoKeyExit  TRUE
	#define EXP_CryptoKeyExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoKeyExit", (RTS_UINTPTR)CryptoKeyExit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoKeyExit
	#define EXT_CmpCryptoCryptoKeyExit
	#define GET_CmpCryptoCryptoKeyExit  ERR_OK
	#define CAL_CmpCryptoCryptoKeyExit pICmpCrypto->ICryptoKeyExit
	#define CHK_CmpCryptoCryptoKeyExit (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoKeyExit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoKeyExit
	#define EXT_CryptoKeyExit
	#define GET_CryptoKeyExit(fl)  CAL_CMGETAPI( "CryptoKeyExit" ) 
	#define CAL_CryptoKeyExit pICmpCrypto->ICryptoKeyExit
	#define CHK_CryptoKeyExit (pICmpCrypto != NULL)
	#define EXP_CryptoKeyExit  CAL_CMEXPAPI( "CryptoKeyExit" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoKeyExit  PFCRYPTOKEYEXIT pfCryptoKeyExit;
	#define EXT_CryptoKeyExit  extern PFCRYPTOKEYEXIT pfCryptoKeyExit;
	#define GET_CryptoKeyExit(fl)  s_pfCMGetAPI2( "CryptoKeyExit", (RTS_VOID_FCTPTR *)&pfCryptoKeyExit, (fl), 0, 0)
	#define CAL_CryptoKeyExit  pfCryptoKeyExit
	#define CHK_CryptoKeyExit  (pfCryptoKeyExit != NULL)
	#define EXP_CryptoKeyExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoKeyExit", (RTS_UINTPTR)CryptoKeyExit, 0, 0) 
#endif




/**
 * <description>Initialize a RtsByteString string.</description>
 * <param name="pByteString" type="IN">Pointer to the RtsByteString to be initialized.</param>
 * <result>Operation result.</result>
 **/
RTS_RESULT CDECL CryptoRtsByteStringInit(RtsByteString* pByteString);
typedef RTS_RESULT (CDECL * PFCRYPTORTSBYTESTRINGINIT) (RtsByteString* pByteString);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTORTSBYTESTRINGINIT_NOTIMPLEMENTED)
	#define USE_CryptoRtsByteStringInit
	#define EXT_CryptoRtsByteStringInit
	#define GET_CryptoRtsByteStringInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoRtsByteStringInit(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoRtsByteStringInit  FALSE
	#define EXP_CryptoRtsByteStringInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoRtsByteStringInit
	#define EXT_CryptoRtsByteStringInit
	#define GET_CryptoRtsByteStringInit(fl)  CAL_CMGETAPI( "CryptoRtsByteStringInit" ) 
	#define CAL_CryptoRtsByteStringInit  CryptoRtsByteStringInit
	#define CHK_CryptoRtsByteStringInit  TRUE
	#define EXP_CryptoRtsByteStringInit  CAL_CMEXPAPI( "CryptoRtsByteStringInit" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoRtsByteStringInit
	#define EXT_CryptoRtsByteStringInit
	#define GET_CryptoRtsByteStringInit(fl)  CAL_CMGETAPI( "CryptoRtsByteStringInit" ) 
	#define CAL_CryptoRtsByteStringInit  CryptoRtsByteStringInit
	#define CHK_CryptoRtsByteStringInit  TRUE
	#define EXP_CryptoRtsByteStringInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRtsByteStringInit", (RTS_UINTPTR)CryptoRtsByteStringInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoRtsByteStringInit
	#define EXT_CmpCryptoCryptoRtsByteStringInit
	#define GET_CmpCryptoCryptoRtsByteStringInit  ERR_OK
	#define CAL_CmpCryptoCryptoRtsByteStringInit pICmpCrypto->ICryptoRtsByteStringInit
	#define CHK_CmpCryptoCryptoRtsByteStringInit (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoRtsByteStringInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoRtsByteStringInit
	#define EXT_CryptoRtsByteStringInit
	#define GET_CryptoRtsByteStringInit(fl)  CAL_CMGETAPI( "CryptoRtsByteStringInit" ) 
	#define CAL_CryptoRtsByteStringInit pICmpCrypto->ICryptoRtsByteStringInit
	#define CHK_CryptoRtsByteStringInit (pICmpCrypto != NULL)
	#define EXP_CryptoRtsByteStringInit  CAL_CMEXPAPI( "CryptoRtsByteStringInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoRtsByteStringInit  PFCRYPTORTSBYTESTRINGINIT pfCryptoRtsByteStringInit;
	#define EXT_CryptoRtsByteStringInit  extern PFCRYPTORTSBYTESTRINGINIT pfCryptoRtsByteStringInit;
	#define GET_CryptoRtsByteStringInit(fl)  s_pfCMGetAPI2( "CryptoRtsByteStringInit", (RTS_VOID_FCTPTR *)&pfCryptoRtsByteStringInit, (fl), 0, 0)
	#define CAL_CryptoRtsByteStringInit  pfCryptoRtsByteStringInit
	#define CHK_CryptoRtsByteStringInit  (pfCryptoRtsByteStringInit != NULL)
	#define EXP_CryptoRtsByteStringInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRtsByteStringInit", (RTS_UINTPTR)CryptoRtsByteStringInit, 0, 0) 
#endif




/**
 * <description>Initialize a byte string.</description>
 * <param name="pByteString" type="IN">Pointer to the RtsByteString to be initialized.</param>
 * <param name="ui32BufferSize" type="IN">Buffersize needed for this byte string.</param>
 * <param name="pBuffer" type="IN">Pointer to a optional static buffer. If NULL is passed the function will allocated ui32BufferSize bytes from the heap.</param>
 * <result>Operation result.</result>
 **/
RTS_RESULT CDECL CryptoRtsByteStringInit2(RtsByteString* pByteString, RTS_UI32 ui32BufferSize, RTS_UI8* pBuffer);
typedef RTS_RESULT (CDECL * PFCRYPTORTSBYTESTRINGINIT2) (RtsByteString* pByteString, RTS_UI32 ui32BufferSize, RTS_UI8* pBuffer);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTORTSBYTESTRINGINIT2_NOTIMPLEMENTED)
	#define USE_CryptoRtsByteStringInit2
	#define EXT_CryptoRtsByteStringInit2
	#define GET_CryptoRtsByteStringInit2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoRtsByteStringInit2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoRtsByteStringInit2  FALSE
	#define EXP_CryptoRtsByteStringInit2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoRtsByteStringInit2
	#define EXT_CryptoRtsByteStringInit2
	#define GET_CryptoRtsByteStringInit2(fl)  CAL_CMGETAPI( "CryptoRtsByteStringInit2" ) 
	#define CAL_CryptoRtsByteStringInit2  CryptoRtsByteStringInit2
	#define CHK_CryptoRtsByteStringInit2  TRUE
	#define EXP_CryptoRtsByteStringInit2  CAL_CMEXPAPI( "CryptoRtsByteStringInit2" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoRtsByteStringInit2
	#define EXT_CryptoRtsByteStringInit2
	#define GET_CryptoRtsByteStringInit2(fl)  CAL_CMGETAPI( "CryptoRtsByteStringInit2" ) 
	#define CAL_CryptoRtsByteStringInit2  CryptoRtsByteStringInit2
	#define CHK_CryptoRtsByteStringInit2  TRUE
	#define EXP_CryptoRtsByteStringInit2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRtsByteStringInit2", (RTS_UINTPTR)CryptoRtsByteStringInit2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoRtsByteStringInit2
	#define EXT_CmpCryptoCryptoRtsByteStringInit2
	#define GET_CmpCryptoCryptoRtsByteStringInit2  ERR_OK
	#define CAL_CmpCryptoCryptoRtsByteStringInit2 pICmpCrypto->ICryptoRtsByteStringInit2
	#define CHK_CmpCryptoCryptoRtsByteStringInit2 (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoRtsByteStringInit2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoRtsByteStringInit2
	#define EXT_CryptoRtsByteStringInit2
	#define GET_CryptoRtsByteStringInit2(fl)  CAL_CMGETAPI( "CryptoRtsByteStringInit2" ) 
	#define CAL_CryptoRtsByteStringInit2 pICmpCrypto->ICryptoRtsByteStringInit2
	#define CHK_CryptoRtsByteStringInit2 (pICmpCrypto != NULL)
	#define EXP_CryptoRtsByteStringInit2  CAL_CMEXPAPI( "CryptoRtsByteStringInit2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoRtsByteStringInit2  PFCRYPTORTSBYTESTRINGINIT2 pfCryptoRtsByteStringInit2;
	#define EXT_CryptoRtsByteStringInit2  extern PFCRYPTORTSBYTESTRINGINIT2 pfCryptoRtsByteStringInit2;
	#define GET_CryptoRtsByteStringInit2(fl)  s_pfCMGetAPI2( "CryptoRtsByteStringInit2", (RTS_VOID_FCTPTR *)&pfCryptoRtsByteStringInit2, (fl), 0, 0)
	#define CAL_CryptoRtsByteStringInit2  pfCryptoRtsByteStringInit2
	#define CHK_CryptoRtsByteStringInit2  (pfCryptoRtsByteStringInit2 != NULL)
	#define EXP_CryptoRtsByteStringInit2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRtsByteStringInit2", (RTS_UINTPTR)CryptoRtsByteStringInit2, 0, 0) 
#endif




/**
 * <description>Initialize a byte string.</description>
 * <param name="pByteString" type="IN">Pointer to the RtsByteString to be initialized.</param>
 * <param name="ui32BufferSize" type="IN">Buffersize needed for this byte string.</param>
 * <param name="pBuffer" type="IN">Pointer to a optional static buffer. If NULL is passed the function will allocated ui32BufferSize bytes from the heap.</param>
 * <param name="bEmptyBuffer" type="IN">Used for static buffers. If FALSE pByteString->ui32Lenght will be set to ui32BufferSize. Must be TRUE if pBuffer == NULL</param>
 * <result>Operation result.</result>
 **/
RTS_RESULT CDECL CryptoRtsByteStringInit3(RtsByteString* pByteString, RTS_UI32 ui32BufferSize, RTS_UI8* pBuffer, RTS_BOOL bEmptyBuffer);
typedef RTS_RESULT (CDECL * PFCRYPTORTSBYTESTRINGINIT3) (RtsByteString* pByteString, RTS_UI32 ui32BufferSize, RTS_UI8* pBuffer, RTS_BOOL bEmptyBuffer);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTORTSBYTESTRINGINIT3_NOTIMPLEMENTED)
	#define USE_CryptoRtsByteStringInit3
	#define EXT_CryptoRtsByteStringInit3
	#define GET_CryptoRtsByteStringInit3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoRtsByteStringInit3(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoRtsByteStringInit3  FALSE
	#define EXP_CryptoRtsByteStringInit3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoRtsByteStringInit3
	#define EXT_CryptoRtsByteStringInit3
	#define GET_CryptoRtsByteStringInit3(fl)  CAL_CMGETAPI( "CryptoRtsByteStringInit3" ) 
	#define CAL_CryptoRtsByteStringInit3  CryptoRtsByteStringInit3
	#define CHK_CryptoRtsByteStringInit3  TRUE
	#define EXP_CryptoRtsByteStringInit3  CAL_CMEXPAPI( "CryptoRtsByteStringInit3" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoRtsByteStringInit3
	#define EXT_CryptoRtsByteStringInit3
	#define GET_CryptoRtsByteStringInit3(fl)  CAL_CMGETAPI( "CryptoRtsByteStringInit3" ) 
	#define CAL_CryptoRtsByteStringInit3  CryptoRtsByteStringInit3
	#define CHK_CryptoRtsByteStringInit3  TRUE
	#define EXP_CryptoRtsByteStringInit3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRtsByteStringInit3", (RTS_UINTPTR)CryptoRtsByteStringInit3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoRtsByteStringInit3
	#define EXT_CmpCryptoCryptoRtsByteStringInit3
	#define GET_CmpCryptoCryptoRtsByteStringInit3  ERR_OK
	#define CAL_CmpCryptoCryptoRtsByteStringInit3 pICmpCrypto->ICryptoRtsByteStringInit3
	#define CHK_CmpCryptoCryptoRtsByteStringInit3 (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoRtsByteStringInit3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoRtsByteStringInit3
	#define EXT_CryptoRtsByteStringInit3
	#define GET_CryptoRtsByteStringInit3(fl)  CAL_CMGETAPI( "CryptoRtsByteStringInit3" ) 
	#define CAL_CryptoRtsByteStringInit3 pICmpCrypto->ICryptoRtsByteStringInit3
	#define CHK_CryptoRtsByteStringInit3 (pICmpCrypto != NULL)
	#define EXP_CryptoRtsByteStringInit3  CAL_CMEXPAPI( "CryptoRtsByteStringInit3" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoRtsByteStringInit3  PFCRYPTORTSBYTESTRINGINIT3 pfCryptoRtsByteStringInit3;
	#define EXT_CryptoRtsByteStringInit3  extern PFCRYPTORTSBYTESTRINGINIT3 pfCryptoRtsByteStringInit3;
	#define GET_CryptoRtsByteStringInit3(fl)  s_pfCMGetAPI2( "CryptoRtsByteStringInit3", (RTS_VOID_FCTPTR *)&pfCryptoRtsByteStringInit3, (fl), 0, 0)
	#define CAL_CryptoRtsByteStringInit3  pfCryptoRtsByteStringInit3
	#define CHK_CryptoRtsByteStringInit3  (pfCryptoRtsByteStringInit3 != NULL)
	#define EXP_CryptoRtsByteStringInit3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRtsByteStringInit3", (RTS_UINTPTR)CryptoRtsByteStringInit3, 0, 0) 
#endif




/**
 * <description>Clean up a byte string.</description>
 * <param name="pByteString" type="IN">Byte string to be clean up.</param>
 * <result>Operation result.</result>
 **/
RTS_RESULT CDECL CryptoRtsByteStringExit(RtsByteString* pByteString);
typedef RTS_RESULT (CDECL * PFCRYPTORTSBYTESTRINGEXIT) (RtsByteString* pByteString);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTORTSBYTESTRINGEXIT_NOTIMPLEMENTED)
	#define USE_CryptoRtsByteStringExit
	#define EXT_CryptoRtsByteStringExit
	#define GET_CryptoRtsByteStringExit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoRtsByteStringExit(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoRtsByteStringExit  FALSE
	#define EXP_CryptoRtsByteStringExit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoRtsByteStringExit
	#define EXT_CryptoRtsByteStringExit
	#define GET_CryptoRtsByteStringExit(fl)  CAL_CMGETAPI( "CryptoRtsByteStringExit" ) 
	#define CAL_CryptoRtsByteStringExit  CryptoRtsByteStringExit
	#define CHK_CryptoRtsByteStringExit  TRUE
	#define EXP_CryptoRtsByteStringExit  CAL_CMEXPAPI( "CryptoRtsByteStringExit" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoRtsByteStringExit
	#define EXT_CryptoRtsByteStringExit
	#define GET_CryptoRtsByteStringExit(fl)  CAL_CMGETAPI( "CryptoRtsByteStringExit" ) 
	#define CAL_CryptoRtsByteStringExit  CryptoRtsByteStringExit
	#define CHK_CryptoRtsByteStringExit  TRUE
	#define EXP_CryptoRtsByteStringExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRtsByteStringExit", (RTS_UINTPTR)CryptoRtsByteStringExit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoRtsByteStringExit
	#define EXT_CmpCryptoCryptoRtsByteStringExit
	#define GET_CmpCryptoCryptoRtsByteStringExit  ERR_OK
	#define CAL_CmpCryptoCryptoRtsByteStringExit pICmpCrypto->ICryptoRtsByteStringExit
	#define CHK_CmpCryptoCryptoRtsByteStringExit (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoRtsByteStringExit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoRtsByteStringExit
	#define EXT_CryptoRtsByteStringExit
	#define GET_CryptoRtsByteStringExit(fl)  CAL_CMGETAPI( "CryptoRtsByteStringExit" ) 
	#define CAL_CryptoRtsByteStringExit pICmpCrypto->ICryptoRtsByteStringExit
	#define CHK_CryptoRtsByteStringExit (pICmpCrypto != NULL)
	#define EXP_CryptoRtsByteStringExit  CAL_CMEXPAPI( "CryptoRtsByteStringExit" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoRtsByteStringExit  PFCRYPTORTSBYTESTRINGEXIT pfCryptoRtsByteStringExit;
	#define EXT_CryptoRtsByteStringExit  extern PFCRYPTORTSBYTESTRINGEXIT pfCryptoRtsByteStringExit;
	#define GET_CryptoRtsByteStringExit(fl)  s_pfCMGetAPI2( "CryptoRtsByteStringExit", (RTS_VOID_FCTPTR *)&pfCryptoRtsByteStringExit, (fl), 0, 0)
	#define CAL_CryptoRtsByteStringExit  pfCryptoRtsByteStringExit
	#define CHK_CryptoRtsByteStringExit  (pfCryptoRtsByteStringExit != NULL)
	#define EXP_CryptoRtsByteStringExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRtsByteStringExit", (RTS_UINTPTR)CryptoRtsByteStringExit, 0, 0) 
#endif




/**
 * <description>Compare two byte strings.</description>
 * <param name="pBS1" type="IN">Byte string 1 to compare.</param>
 * <param name="pBS2" type="IN">Byte string 2 to compare.</param>
 * <result>Operation result like memcmp.</result>
 **/
RTS_I32 CDECL CryptoRtsByteStringCompare(RtsByteString * pBS1, RtsByteString * pBS2);
typedef RTS_I32 (CDECL * PFCRYPTORTSBYTESTRINGCOMPARE) (RtsByteString * pBS1, RtsByteString * pBS2);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTORTSBYTESTRINGCOMPARE_NOTIMPLEMENTED)
	#define USE_CryptoRtsByteStringCompare
	#define EXT_CryptoRtsByteStringCompare
	#define GET_CryptoRtsByteStringCompare(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoRtsByteStringCompare(p0,p1)  (RTS_I32)ERR_NOTIMPLEMENTED
	#define CHK_CryptoRtsByteStringCompare  FALSE
	#define EXP_CryptoRtsByteStringCompare  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoRtsByteStringCompare
	#define EXT_CryptoRtsByteStringCompare
	#define GET_CryptoRtsByteStringCompare(fl)  CAL_CMGETAPI( "CryptoRtsByteStringCompare" ) 
	#define CAL_CryptoRtsByteStringCompare  CryptoRtsByteStringCompare
	#define CHK_CryptoRtsByteStringCompare  TRUE
	#define EXP_CryptoRtsByteStringCompare  CAL_CMEXPAPI( "CryptoRtsByteStringCompare" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoRtsByteStringCompare
	#define EXT_CryptoRtsByteStringCompare
	#define GET_CryptoRtsByteStringCompare(fl)  CAL_CMGETAPI( "CryptoRtsByteStringCompare" ) 
	#define CAL_CryptoRtsByteStringCompare  CryptoRtsByteStringCompare
	#define CHK_CryptoRtsByteStringCompare  TRUE
	#define EXP_CryptoRtsByteStringCompare  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRtsByteStringCompare", (RTS_UINTPTR)CryptoRtsByteStringCompare, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoRtsByteStringCompare
	#define EXT_CmpCryptoCryptoRtsByteStringCompare
	#define GET_CmpCryptoCryptoRtsByteStringCompare  ERR_OK
	#define CAL_CmpCryptoCryptoRtsByteStringCompare pICmpCrypto->ICryptoRtsByteStringCompare
	#define CHK_CmpCryptoCryptoRtsByteStringCompare (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoRtsByteStringCompare  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoRtsByteStringCompare
	#define EXT_CryptoRtsByteStringCompare
	#define GET_CryptoRtsByteStringCompare(fl)  CAL_CMGETAPI( "CryptoRtsByteStringCompare" ) 
	#define CAL_CryptoRtsByteStringCompare pICmpCrypto->ICryptoRtsByteStringCompare
	#define CHK_CryptoRtsByteStringCompare (pICmpCrypto != NULL)
	#define EXP_CryptoRtsByteStringCompare  CAL_CMEXPAPI( "CryptoRtsByteStringCompare" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoRtsByteStringCompare  PFCRYPTORTSBYTESTRINGCOMPARE pfCryptoRtsByteStringCompare;
	#define EXT_CryptoRtsByteStringCompare  extern PFCRYPTORTSBYTESTRINGCOMPARE pfCryptoRtsByteStringCompare;
	#define GET_CryptoRtsByteStringCompare(fl)  s_pfCMGetAPI2( "CryptoRtsByteStringCompare", (RTS_VOID_FCTPTR *)&pfCryptoRtsByteStringCompare, (fl), 0, 0)
	#define CAL_CryptoRtsByteStringCompare  pfCryptoRtsByteStringCompare
	#define CHK_CryptoRtsByteStringCompare  (pfCryptoRtsByteStringCompare != NULL)
	#define EXP_CryptoRtsByteStringCompare  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRtsByteStringCompare", (RTS_UINTPTR)CryptoRtsByteStringCompare, 0, 0) 
#endif




/**
 * <description>Converts an binary blob into an BASE64 encoded block.
 * 	Note: The size of the BASE64 output must be at least CEIL(inputSize / 3) * 4 bytes.
 * </description>
 * <param name="input" type="IN">Input to be encoded in BASE64</param>
 * <param name="base64Output" type="IN">Buffer to store the BASE64 data</param>
 * <result>Error Code:
 *		- ERR_OK: Encoding successful.
 *		- ERR_PARAMETER: Invalid parameter given.
 *		- ERR_BUFFERSIZE: The base64Output was too small.
 * </result>
 **/
RTS_RESULT CDECL CryptoBase64Encode(RtsByteString *input, RtsByteString* base64Output);
typedef RTS_RESULT (CDECL * PFCRYPTOBASE64ENCODE) (RtsByteString *input, RtsByteString* base64Output);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOBASE64ENCODE_NOTIMPLEMENTED)
	#define USE_CryptoBase64Encode
	#define EXT_CryptoBase64Encode
	#define GET_CryptoBase64Encode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoBase64Encode(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoBase64Encode  FALSE
	#define EXP_CryptoBase64Encode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoBase64Encode
	#define EXT_CryptoBase64Encode
	#define GET_CryptoBase64Encode(fl)  CAL_CMGETAPI( "CryptoBase64Encode" ) 
	#define CAL_CryptoBase64Encode  CryptoBase64Encode
	#define CHK_CryptoBase64Encode  TRUE
	#define EXP_CryptoBase64Encode  CAL_CMEXPAPI( "CryptoBase64Encode" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoBase64Encode
	#define EXT_CryptoBase64Encode
	#define GET_CryptoBase64Encode(fl)  CAL_CMGETAPI( "CryptoBase64Encode" ) 
	#define CAL_CryptoBase64Encode  CryptoBase64Encode
	#define CHK_CryptoBase64Encode  TRUE
	#define EXP_CryptoBase64Encode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoBase64Encode", (RTS_UINTPTR)CryptoBase64Encode, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoBase64Encode
	#define EXT_CmpCryptoCryptoBase64Encode
	#define GET_CmpCryptoCryptoBase64Encode  ERR_OK
	#define CAL_CmpCryptoCryptoBase64Encode pICmpCrypto->ICryptoBase64Encode
	#define CHK_CmpCryptoCryptoBase64Encode (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoBase64Encode  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoBase64Encode
	#define EXT_CryptoBase64Encode
	#define GET_CryptoBase64Encode(fl)  CAL_CMGETAPI( "CryptoBase64Encode" ) 
	#define CAL_CryptoBase64Encode pICmpCrypto->ICryptoBase64Encode
	#define CHK_CryptoBase64Encode (pICmpCrypto != NULL)
	#define EXP_CryptoBase64Encode  CAL_CMEXPAPI( "CryptoBase64Encode" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoBase64Encode  PFCRYPTOBASE64ENCODE pfCryptoBase64Encode;
	#define EXT_CryptoBase64Encode  extern PFCRYPTOBASE64ENCODE pfCryptoBase64Encode;
	#define GET_CryptoBase64Encode(fl)  s_pfCMGetAPI2( "CryptoBase64Encode", (RTS_VOID_FCTPTR *)&pfCryptoBase64Encode, (fl), 0, 0)
	#define CAL_CryptoBase64Encode  pfCryptoBase64Encode
	#define CHK_CryptoBase64Encode  (pfCryptoBase64Encode != NULL)
	#define EXP_CryptoBase64Encode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoBase64Encode", (RTS_UINTPTR)CryptoBase64Encode, 0, 0) 
#endif




/**
 * <description>Decodes an BASE64 block into an binary blob.
 * 	Note: The size of the output must be at least CEIL(base64InputSize / 4) * 3 bytes.
 * </description>
 * <param name="base64Input" type="IN">Input of BASE64 encoded data.</param>
 * <param name="output" type="IN">Buffer to store the decoded data</param>
 * <result>Error Code:
 *		- ERR_OK: Decoding successful.
 *		- ERR_PARAMETER: Invalid parameter given.
 *		- ERR_BUFFERSIZE: The base64Output was too small.
 * </result>
 **/
RTS_RESULT CDECL CryptoBase64Decode(RtsByteString *base64Input, RtsByteString *output);
typedef RTS_RESULT (CDECL * PFCRYPTOBASE64DECODE) (RtsByteString *base64Input, RtsByteString *output);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOBASE64DECODE_NOTIMPLEMENTED)
	#define USE_CryptoBase64Decode
	#define EXT_CryptoBase64Decode
	#define GET_CryptoBase64Decode(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoBase64Decode(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoBase64Decode  FALSE
	#define EXP_CryptoBase64Decode  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoBase64Decode
	#define EXT_CryptoBase64Decode
	#define GET_CryptoBase64Decode(fl)  CAL_CMGETAPI( "CryptoBase64Decode" ) 
	#define CAL_CryptoBase64Decode  CryptoBase64Decode
	#define CHK_CryptoBase64Decode  TRUE
	#define EXP_CryptoBase64Decode  CAL_CMEXPAPI( "CryptoBase64Decode" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoBase64Decode
	#define EXT_CryptoBase64Decode
	#define GET_CryptoBase64Decode(fl)  CAL_CMGETAPI( "CryptoBase64Decode" ) 
	#define CAL_CryptoBase64Decode  CryptoBase64Decode
	#define CHK_CryptoBase64Decode  TRUE
	#define EXP_CryptoBase64Decode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoBase64Decode", (RTS_UINTPTR)CryptoBase64Decode, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoBase64Decode
	#define EXT_CmpCryptoCryptoBase64Decode
	#define GET_CmpCryptoCryptoBase64Decode  ERR_OK
	#define CAL_CmpCryptoCryptoBase64Decode pICmpCrypto->ICryptoBase64Decode
	#define CHK_CmpCryptoCryptoBase64Decode (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoBase64Decode  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoBase64Decode
	#define EXT_CryptoBase64Decode
	#define GET_CryptoBase64Decode(fl)  CAL_CMGETAPI( "CryptoBase64Decode" ) 
	#define CAL_CryptoBase64Decode pICmpCrypto->ICryptoBase64Decode
	#define CHK_CryptoBase64Decode (pICmpCrypto != NULL)
	#define EXP_CryptoBase64Decode  CAL_CMEXPAPI( "CryptoBase64Decode" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoBase64Decode  PFCRYPTOBASE64DECODE pfCryptoBase64Decode;
	#define EXT_CryptoBase64Decode  extern PFCRYPTOBASE64DECODE pfCryptoBase64Decode;
	#define GET_CryptoBase64Decode(fl)  s_pfCMGetAPI2( "CryptoBase64Decode", (RTS_VOID_FCTPTR *)&pfCryptoBase64Decode, (fl), 0, 0)
	#define CAL_CryptoBase64Decode  pfCryptoBase64Decode
	#define CHK_CryptoBase64Decode  (pfCryptoBase64Decode != NULL)
	#define EXP_CryptoBase64Decode  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoBase64Decode", (RTS_UINTPTR)CryptoBase64Decode, 0, 0) 
#endif





/**
 * <description>Use this interface to register a cryptographic backend. A backend provides access to
 *	different cryptographic capabilities of the device itself.</description>
 * <param name="hBackend" type="IN">Handle to the backend.</param>
 * <param name="component" type="IN">The component that implements this backend.</param>
 * <param name="backendProperties" type="IN">Properties of the cryptographic backend. See Backend Properties of CmpCryptoBackendItf</param>
 * <result>
 *		- ERR_OK: Registration successful.
 *		- ERR_FAILED: Registration no successful.
 * </result>
 **/
RTS_RESULT CDECL CryptoRegisterProvider(CryptoProvider *pProvider);
typedef RTS_RESULT (CDECL * PFCRYPTOREGISTERPROVIDER) (CryptoProvider *pProvider);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOREGISTERPROVIDER_NOTIMPLEMENTED)
	#define USE_CryptoRegisterProvider
	#define EXT_CryptoRegisterProvider
	#define GET_CryptoRegisterProvider(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CryptoRegisterProvider(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CryptoRegisterProvider  FALSE
	#define EXP_CryptoRegisterProvider  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CryptoRegisterProvider
	#define EXT_CryptoRegisterProvider
	#define GET_CryptoRegisterProvider(fl)  CAL_CMGETAPI( "CryptoRegisterProvider" ) 
	#define CAL_CryptoRegisterProvider  CryptoRegisterProvider
	#define CHK_CryptoRegisterProvider  TRUE
	#define EXP_CryptoRegisterProvider  CAL_CMEXPAPI( "CryptoRegisterProvider" ) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_CryptoRegisterProvider
	#define EXT_CryptoRegisterProvider
	#define GET_CryptoRegisterProvider(fl)  CAL_CMGETAPI( "CryptoRegisterProvider" ) 
	#define CAL_CryptoRegisterProvider  CryptoRegisterProvider
	#define CHK_CryptoRegisterProvider  TRUE
	#define EXP_CryptoRegisterProvider  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRegisterProvider", (RTS_UINTPTR)CryptoRegisterProvider, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptoCryptoRegisterProvider
	#define EXT_CmpCryptoCryptoRegisterProvider
	#define GET_CmpCryptoCryptoRegisterProvider  ERR_OK
	#define CAL_CmpCryptoCryptoRegisterProvider pICmpCrypto->ICryptoRegisterProvider
	#define CHK_CmpCryptoCryptoRegisterProvider (pICmpCrypto != NULL)
	#define EXP_CmpCryptoCryptoRegisterProvider  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CryptoRegisterProvider
	#define EXT_CryptoRegisterProvider
	#define GET_CryptoRegisterProvider(fl)  CAL_CMGETAPI( "CryptoRegisterProvider" ) 
	#define CAL_CryptoRegisterProvider pICmpCrypto->ICryptoRegisterProvider
	#define CHK_CryptoRegisterProvider (pICmpCrypto != NULL)
	#define EXP_CryptoRegisterProvider  CAL_CMEXPAPI( "CryptoRegisterProvider" ) 
#else /* DYNAMIC_LINK */
	#define USE_CryptoRegisterProvider  PFCRYPTOREGISTERPROVIDER pfCryptoRegisterProvider;
	#define EXT_CryptoRegisterProvider  extern PFCRYPTOREGISTERPROVIDER pfCryptoRegisterProvider;
	#define GET_CryptoRegisterProvider(fl)  s_pfCMGetAPI2( "CryptoRegisterProvider", (RTS_VOID_FCTPTR *)&pfCryptoRegisterProvider, (fl), 0, 0)
	#define CAL_CryptoRegisterProvider  pfCryptoRegisterProvider
	#define CHK_CryptoRegisterProvider  (pfCryptoRegisterProvider != NULL)
	#define EXP_CryptoRegisterProvider  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CryptoRegisterProvider", (RTS_UINTPTR)CryptoRegisterProvider, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFCRYPTOGETALGORITHMBYID ICryptoGetAlgorithmById;
 	PFCRYPTOGETFIRSTALGORITHM ICryptoGetFirstAlgorithm;
 	PFCRYPTOGETNEXTALGORITHM ICryptoGetNextAlgorithm;
 	PFCRYPTOGETALGORITHMINFO ICryptoGetAlgorithmInfo;
 	PFCRYPTOSYMMETRICENCRYPT ICryptoSymmetricEncrypt;
 	PFCRYPTOSYMMETRICDECRYPT ICryptoSymmetricDecrypt;
 	PFCRYPTOASYMMETRICENCRYPT ICryptoAsymmetricEncrypt;
 	PFCRYPTOASYMMETRICDECRYPT ICryptoAsymmetricDecrypt;
 	PFCRYPTOGETASYMMETRICKEYLENGTH ICryptoGetAsymmetricKeyLength;
 	PFCRYPTOGENERATEASYMMETRICKEYPAIR ICryptoGenerateAsymmetricKeyPair;
 	PFCRYPTOEXPORTASYMMETRICKEY ICryptoExportAsymmetricKey;
 	PFCRYPTOEXPORTRAWASYMMETRICKEY ICryptoExportRawAsymmetricKey;
 	PFCRYPTOIMPORTASYMMETRICKEY ICryptoImportAsymmetricKey;
 	PFCRYPTOIMPORTRAWASYMMETRICKEY ICryptoImportRawAsymmetricKey;
 	PFCRYPTOSTOREPRIVATEKEY ICryptoStorePrivateKey;
 	PFCRYPTOLOADPRIVATEKEY ICryptoLoadPrivateKey;
 	PFCRYPTODELETEPRIVATEKEY ICryptoDeletePrivateKey;
 	PFCRYPTOGENERATEHASH ICryptoGenerateHash;
 	PFCRYPTODERIVEKEY ICryptoDeriveKey;
 	PFCRYPTOKEYAGREEMENT ICryptoKeyAgreement;
 	PFCRYPTOHMACSIGN ICryptoHMACSign;
 	PFCRYPTOHMACVERIFY ICryptoHMACVerify;
 	PFCRYPTOSIGNATUREGENERATE ICryptoSignatureGenerate;
 	PFCRYPTOSIGNATUREVERIFY ICryptoSignatureVerify;
 	PFCRYPTOGENERATERANDOMNUMBER ICryptoGenerateRandomNumber;
 	PFCRYPTOKEYINIT ICryptoKeyInit;
 	PFCRYPTOKEYEXIT ICryptoKeyExit;
 	PFCRYPTORTSBYTESTRINGINIT ICryptoRtsByteStringInit;
 	PFCRYPTORTSBYTESTRINGINIT2 ICryptoRtsByteStringInit2;
 	PFCRYPTORTSBYTESTRINGINIT3 ICryptoRtsByteStringInit3;
 	PFCRYPTORTSBYTESTRINGEXIT ICryptoRtsByteStringExit;
 	PFCRYPTORTSBYTESTRINGCOMPARE ICryptoRtsByteStringCompare;
 	PFCRYPTOBASE64ENCODE ICryptoBase64Encode;
 	PFCRYPTOBASE64DECODE ICryptoBase64Decode;
 	PFCRYPTOREGISTERPROVIDER ICryptoRegisterProvider;
 } ICmpCrypto_C;

#ifdef CPLUSPLUS
class ICmpCrypto : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ICryptoGetAlgorithmById(RTS_UI32 ui32CryptoID, RTS_RESULT* pResult) =0;
		virtual RTS_HANDLE CDECL ICryptoGetFirstAlgorithm(RTS_UI32 ui32CryptoType, char *pszName, RTS_I32 i32MaxNameLen, RTS_UI32 *pui32CryptoID, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ICryptoGetNextAlgorithm(RTS_HANDLE hCrypto, RTS_UI32 ui32CryptoType, char *pszName, RTS_I32 i32MaxNameLen, RTS_UI32 *pui32CryptoID, RTS_RESULT *pResult) =0;
		virtual RtsCryptoInfo* CDECL ICryptoGetAlgorithmInfo(RTS_HANDLE hCrypto, RTS_RESULT* pCryptoInfo) =0;
		virtual RTS_RESULT CDECL ICryptoSymmetricEncrypt(RTS_HANDLE hAlgo, RtsByteString* pPlainText, RtsCryptoKey key, RtsByteString* pInitVector, RTS_BOOL bEnablePadding, RtsByteString* pCipherText) =0;
		virtual RTS_RESULT CDECL ICryptoSymmetricDecrypt(RTS_HANDLE hAlgo, RtsByteString* pCipherText, RtsCryptoKey key, RtsByteString* pInitVector, RTS_BOOL bEnablePadding, RtsByteString* pPlainText) =0;
		virtual RTS_RESULT CDECL ICryptoAsymmetricEncrypt(RTS_HANDLE hAlgo, RtsByteString* pPlainText, RtsCryptoKey publicKey, RtsByteString* pCipherText) =0;
		virtual RTS_RESULT CDECL ICryptoAsymmetricDecrypt(RTS_HANDLE hAlgo, RtsByteString* pCipherText, RtsCryptoKey privateKey, RtsByteString* pPlainText) =0;
		virtual RTS_UI32 CDECL ICryptoGetAsymmetricKeyLength(RtsCryptoKey asymmetricKey, RTS_RESULT* pResult) =0;
		virtual RTS_RESULT CDECL ICryptoGenerateAsymmetricKeyPair(RTS_IEC_HANDLE hAlgo, RTS_IEC_UDINT ui32KeyLen, RtsCryptoKey *pKeyPair, RTS_BOOL bPrivateKeyExportable) =0;
		virtual RTS_RESULT CDECL ICryptoExportAsymmetricKey(RtsCryptoKey key, RTS_BOOL bBase64, RTS_BOOL bPrivateKey, RtsByteString *pData) =0;
		virtual RTS_RESULT CDECL ICryptoExportRawAsymmetricKey(RtsCryptoKey key, RTS_BOOL bPrivateKey, RtsByteString *pKeyData) =0;
		virtual RTS_RESULT CDECL ICryptoImportAsymmetricKey(RtsByteString data, RTS_BOOL bBase64, RTS_BOOL bPrivateKey, RtsCryptoKey *pKey) =0;
		virtual RTS_RESULT CDECL ICryptoImportRawAsymmetricKey(RTS_IEC_HANDLE hAlgo, RtsByteString keyData, RTS_BOOL bPrivateKey, RtsCryptoKey *pKey) =0;
		virtual RTS_RESULT CDECL ICryptoStorePrivateKey(RtsCryptoKey key, RtsByteString *pKeyIdentifier) =0;
		virtual RTS_RESULT CDECL ICryptoLoadPrivateKey(RtsByteString keyIdentifier, RtsCryptoKey *pKey) =0;
		virtual RTS_RESULT CDECL ICryptoDeletePrivateKey(RtsByteString keyIdentifier) =0;
		virtual RTS_RESULT CDECL ICryptoGenerateHash(RTS_HANDLE hAlgo, RtsByteString* pData, RtsByteString* pHash) =0;
		virtual RTS_RESULT CDECL ICryptoDeriveKey(RTS_HANDLE hAlgo, RtsByteString *pKey, RtsByteString *pSalt, RtsKdfParameter* pSpecificParameters, RtsByteString *pDerivedKey) =0;
		virtual RTS_RESULT CDECL ICryptoKeyAgreement(RTS_HANDLE hAlgo, RtsCryptoKey privateKey, RtsCryptoKey peerPublicKey, RtsByteString *pSharedSecret) =0;
		virtual RTS_RESULT CDECL ICryptoHMACSign(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey key, RtsByteString* pSignature) =0;
		virtual RTS_RESULT CDECL ICryptoHMACVerify(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey key, RtsByteString* pSignature) =0;
		virtual RTS_RESULT CDECL ICryptoSignatureGenerate(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey privateKey, RtsByteString* pSignature) =0;
		virtual RTS_RESULT CDECL ICryptoSignatureVerify(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey publicKey, RtsByteString* pSignature) =0;
		virtual RTS_RESULT CDECL ICryptoGenerateRandomNumber(RTS_UI32 ui32NumOfRandomBytes, RtsByteString* pRandom) =0;
		virtual RTS_RESULT CDECL ICryptoKeyInit(RtsCryptoKey* pKey, RtsCryptoKeyStorage key, RTSCRYPTOKEYTYPE keyType) =0;
		virtual RTS_RESULT CDECL ICryptoKeyExit(RtsCryptoKey* pKey) =0;
		virtual RTS_RESULT CDECL ICryptoRtsByteStringInit(RtsByteString* pByteString) =0;
		virtual RTS_RESULT CDECL ICryptoRtsByteStringInit2(RtsByteString* pByteString, RTS_UI32 ui32BufferSize, RTS_UI8* pBuffer) =0;
		virtual RTS_RESULT CDECL ICryptoRtsByteStringInit3(RtsByteString* pByteString, RTS_UI32 ui32BufferSize, RTS_UI8* pBuffer, RTS_BOOL bEmptyBuffer) =0;
		virtual RTS_RESULT CDECL ICryptoRtsByteStringExit(RtsByteString* pByteString) =0;
		virtual RTS_I32 CDECL ICryptoRtsByteStringCompare(RtsByteString * pBS1, RtsByteString * pBS2) =0;
		virtual RTS_RESULT CDECL ICryptoBase64Encode(RtsByteString *input, RtsByteString* base64Output) =0;
		virtual RTS_RESULT CDECL ICryptoBase64Decode(RtsByteString *base64Input, RtsByteString *output) =0;
		virtual RTS_RESULT CDECL ICryptoRegisterProvider(CryptoProvider *pProvider) =0;
};
	#ifndef ITF_CmpCrypto
		#define ITF_CmpCrypto static ICmpCrypto *pICmpCrypto = NULL;
	#endif
	#define EXTITF_CmpCrypto
#else	/*CPLUSPLUS*/
	typedef ICmpCrypto_C		ICmpCrypto;
	#ifndef ITF_CmpCrypto
		#define ITF_CmpCrypto
	#endif
	#define EXTITF_CmpCrypto
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPCRYPTOITF_H_*/
