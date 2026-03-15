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
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */

	
	
#ifndef _CMPCRYPTOITF_H_
#define _CMPCRYPTOITF_H_

#include "CmpStd.h"

 

 




/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>Enum: RtsCryptoID</description>
 */

/* --- Symmetric chipers --- */
#define RTSCRYPTOID_AES_128_CBC				RTS_IEC_UDINT_C(0x1001)
#define RTSCRYPTOID_AES_128_CFB				RTS_IEC_UDINT_C(0x1002)	
#define RTSCRYPTOID_AES_256_CBC				RTS_IEC_UDINT_C(0x1003)	
#define RTSCRYPTOID_DES_CBC					RTS_IEC_UDINT_C(0x1004)	
#define RTSCRYPTOID_AES_256_CTR				RTS_IEC_UDINT_C(0x1005)

/* --- Asymmetric Cipers --- */	
#define RTSCRYPTOID_RSA						RTS_IEC_UDINT_C(0x2001)
#define RTSCRYPTOID_RSA_OAEP_PADDING		RTS_IEC_UDINT_C(0x2002)	
#define RTSCRYPTOID_RSA_PKCS1_V15_PADDING   RTS_IEC_UDINT_C(0x2003)	

/* --- Hash functions --- */
#define RTSCRYPTOID_HASH_MD5				RTS_IEC_UDINT_C(0x3001)	
#define RTSCRYPTOID_HASH_SHA1				RTS_IEC_UDINT_C(0x3002)	
#define RTSCRYPTOID_HASH_SHA224				RTS_IEC_UDINT_C(0x3003)	
#define RTSCRYPTOID_HASH_SHA256				RTS_IEC_UDINT_C(0x3004)	
#define RTSCRYPTOID_HASH_SHA384				RTS_IEC_UDINT_C(0x3005)	
#define RTSCRYPTOID_HASH_SHA512				RTS_IEC_UDINT_C(0x3006)	

/* --- Signature Functions --- */
/* HMAC Functions */
#define RTSCRYPTOID_HMAC_MD5				RTS_IEC_UDINT_C(0x4001)
#define RTSCRYPTOID_HMAC_SHA1				RTS_IEC_UDINT_C(0x4002)	
#define RTSCRYPTOID_HMAC_SHA224				RTS_IEC_UDINT_C(0x4003)	
#define RTSCRYPTOID_HMAC_SHA256				RTS_IEC_UDINT_C(0x4004)	
#define RTSCRYPTOID_HMAC_SHA384				RTS_IEC_UDINT_C(0x4005)	
#define RTSCRYPTOID_HMAC_SHA512				RTS_IEC_UDINT_C(0x4006)	

/* RSA Functions */
#define RTSCRYPTOID_RSA_OAEP_SHA1			RTS_IEC_UDINT_C(0x5003)
#define RTSCRYPTOID_RSA_OAEP_SHA256			RTS_IEC_UDINT_C(0x5004)	
#define RTSCRYPTOID_RSA_PKCS1_V15_SHA1		RTS_IEC_UDINT_C(0x5001)	
#define RTSCRYPTOID_RSA_PKCS1_V15_SHA256    RTS_IEC_UDINT_C(0x5002)	

/* Typed enum definition */
#define RTSCRYPTOID							RTS_IEC_UDINT

/**
 * <description>Enum: RtsCryptoKeyType</description>
 */
#define KEYTYPE_KEY    RTS_IEC_DINT_C(0x0)	/* The key is directly available. Stored in a RTS_BYTESTRING. */
#define KEYTYPE_CERTHANDLE    RTS_IEC_DINT_C(0x1)	/* The key is identified by a handle to the certificat. Only used for asymmetric keys. */
#define KEYTYPE_CERTCONTENT    RTS_IEC_DINT_C(0x2)	/* The key is identified by the certificate context. Only used for asymmetric keys. */
/* Typed enum definition */
#define RTSCRYPTOKEYTYPE    RTS_IEC_DINT

/**
 * <description>Enum: RtsCryptoType</description>
 */
#define RTSCRYPTOTYPE_SYMMETRIC    RTS_IEC_UDINT_C(0x1000)	
#define RTSCRYPTOTYPE_ASYMMETRIC    RTS_IEC_UDINT_C(0x2000)	
#define RTSCRYPTOTYPE_HASH    RTS_IEC_UDINT_C(0x3000)	
#define RTSCRYPTOTYPE_HMAC    RTS_IEC_UDINT_C(0x4000)	
#define RTSCRYPTOTYPE_SIGNATURE    RTS_IEC_UDINT_C(0x5000)	
/* Typed enum definition */
#define RTSCRYPTOTYPE    RTS_IEC_UDINT

/**
 * This a representation of a byte string.
 */
typedef struct tagRtsByteString
{
	RTS_IEC_UDINT ui32Len;		/* Current lenght of data stored in the buffer. */
	RTS_IEC_UDINT ui32MaxLen;		/* The allocated size of the buffer. */
	RTS_IEC_BYTE *pByData;		/* Pointer to buffer */
	RTS_IEC_BOOL xDynamic;		/* Indicates if the data was allocated dynamically */
} RtsByteString;

/**
 * Storage of a crypto key.
 */
typedef union
{
	RtsByteString byteString;		/* Key is stored using the byte string. */
	RTS_IEC_HANDLE hCert;		/* Key is identified by this handle. */
} RtsCryptoKeyStorage;

/**
 * A cryptographic key.
 */
typedef struct tagRtsCryptoKey
{
	RTS_IEC_DINT keyType;		/* The type of the key. */
	RtsCryptoKeyStorage key;		/* Information of the key. Acturally only the KeyType_Key is supported. */
} RtsCryptoKey;

/**
 * Perform a asymmetric decryption using the algorithm handle.
 * :return: Result of the operation
 */
typedef struct tagcryptoasymmetricdecrypt_struct
{
	RTS_IEC_HANDLE hAlgo;				/* VAR_INPUT */	/* Handle to the algorithm. */
	RtsByteString *pCipherText;			/* VAR_INPUT */	/* Data to be decrypted */
	RtsCryptoKey privateKey;			/* VAR_INPUT */	/* Private key used to decrypt the data. */
	RtsByteString *pPlainText;			/* VAR_INPUT */	/* Decrypted data. */
	RTS_IEC_RESULT CryptoAsymmetricDecrypt;	/* VAR_OUTPUT */	
} cryptoasymmetricdecrypt_struct;

void CDECL CDECL_EXT cryptoasymmetricdecrypt(cryptoasymmetricdecrypt_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOASYMMETRICDECRYPT_IEC) (cryptoasymmetricdecrypt_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOASYMMETRICDECRYPT_NOTIMPLEMENTED)
	#define USE_cryptoasymmetricdecrypt
	#define EXT_cryptoasymmetricdecrypt
	#define GET_cryptoasymmetricdecrypt(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptoasymmetricdecrypt(p0) 
	#define CHK_cryptoasymmetricdecrypt  FALSE
	#define EXP_cryptoasymmetricdecrypt  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptoasymmetricdecrypt
	#define EXT_cryptoasymmetricdecrypt
	#define GET_cryptoasymmetricdecrypt(fl)  CAL_CMGETAPI( "cryptoasymmetricdecrypt" ) 
	#define CAL_cryptoasymmetricdecrypt  cryptoasymmetricdecrypt
	#define CHK_cryptoasymmetricdecrypt  TRUE
	#define EXP_cryptoasymmetricdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptoasymmetricdecrypt", (RTS_UINTPTR)cryptoasymmetricdecrypt, 1, 0x126B86CC, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptoasymmetricdecrypt
	#define EXT_cryptoasymmetricdecrypt
	#define GET_cryptoasymmetricdecrypt(fl)  CAL_CMGETAPI( "cryptoasymmetricdecrypt" ) 
	#define CAL_cryptoasymmetricdecrypt  cryptoasymmetricdecrypt
	#define CHK_cryptoasymmetricdecrypt  TRUE
	#define EXP_cryptoasymmetricdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptoasymmetricdecrypt", (RTS_UINTPTR)cryptoasymmetricdecrypt, 1, 0x126B86CC, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptoasymmetricdecrypt
	#define EXT_CmpCryptocryptoasymmetricdecrypt
	#define GET_CmpCryptocryptoasymmetricdecrypt  ERR_OK
	#define CAL_CmpCryptocryptoasymmetricdecrypt  cryptoasymmetricdecrypt
	#define CHK_CmpCryptocryptoasymmetricdecrypt  TRUE
	#define EXP_CmpCryptocryptoasymmetricdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptoasymmetricdecrypt", (RTS_UINTPTR)cryptoasymmetricdecrypt, 1, 0x126B86CC, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptoasymmetricdecrypt
	#define EXT_cryptoasymmetricdecrypt
	#define GET_cryptoasymmetricdecrypt(fl)  CAL_CMGETAPI( "cryptoasymmetricdecrypt" ) 
	#define CAL_cryptoasymmetricdecrypt  cryptoasymmetricdecrypt
	#define CHK_cryptoasymmetricdecrypt  TRUE
	#define EXP_cryptoasymmetricdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptoasymmetricdecrypt", (RTS_UINTPTR)cryptoasymmetricdecrypt, 1, 0x126B86CC, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptoasymmetricdecrypt  PFCRYPTOASYMMETRICDECRYPT_IEC pfcryptoasymmetricdecrypt;
	#define EXT_cryptoasymmetricdecrypt  extern PFCRYPTOASYMMETRICDECRYPT_IEC pfcryptoasymmetricdecrypt;
	#define GET_cryptoasymmetricdecrypt(fl)  s_pfCMGetAPI2( "cryptoasymmetricdecrypt", (RTS_VOID_FCTPTR *)&pfcryptoasymmetricdecrypt, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x126B86CC, 0x03050B00)
	#define CAL_cryptoasymmetricdecrypt  pfcryptoasymmetricdecrypt
	#define CHK_cryptoasymmetricdecrypt  (pfcryptoasymmetricdecrypt != NULL)
	#define EXP_cryptoasymmetricdecrypt   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptoasymmetricdecrypt", (RTS_UINTPTR)cryptoasymmetricdecrypt, 1, 0x126B86CC, 0x03050B00) 
#endif


/**
 * Perform a asymmetric encryption using the algorithm handle.
 * :return: Result of the operation
 */
typedef struct tagcryptoasymmetricencrypt_struct
{
	RTS_IEC_HANDLE hAlgo;				/* VAR_INPUT */	/* Handle to the algorithm. */
	RtsByteString *pPlainText;			/* VAR_INPUT */	/* Data to be encrypted */
	RtsCryptoKey publicKey;				/* VAR_INPUT */	/* Public key used to encrypt the data. */
	RtsByteString *pCipherText;			/* VAR_INPUT */	/* Encrypted data. */
	RTS_IEC_RESULT CryptoAsymmetricEncrypt;	/* VAR_OUTPUT */	
} cryptoasymmetricencrypt_struct;

void CDECL CDECL_EXT cryptoasymmetricencrypt(cryptoasymmetricencrypt_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOASYMMETRICENCRYPT_IEC) (cryptoasymmetricencrypt_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOASYMMETRICENCRYPT_NOTIMPLEMENTED)
	#define USE_cryptoasymmetricencrypt
	#define EXT_cryptoasymmetricencrypt
	#define GET_cryptoasymmetricencrypt(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptoasymmetricencrypt(p0) 
	#define CHK_cryptoasymmetricencrypt  FALSE
	#define EXP_cryptoasymmetricencrypt  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptoasymmetricencrypt
	#define EXT_cryptoasymmetricencrypt
	#define GET_cryptoasymmetricencrypt(fl)  CAL_CMGETAPI( "cryptoasymmetricencrypt" ) 
	#define CAL_cryptoasymmetricencrypt  cryptoasymmetricencrypt
	#define CHK_cryptoasymmetricencrypt  TRUE
	#define EXP_cryptoasymmetricencrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptoasymmetricencrypt", (RTS_UINTPTR)cryptoasymmetricencrypt, 1, 0xC14613D0, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptoasymmetricencrypt
	#define EXT_cryptoasymmetricencrypt
	#define GET_cryptoasymmetricencrypt(fl)  CAL_CMGETAPI( "cryptoasymmetricencrypt" ) 
	#define CAL_cryptoasymmetricencrypt  cryptoasymmetricencrypt
	#define CHK_cryptoasymmetricencrypt  TRUE
	#define EXP_cryptoasymmetricencrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptoasymmetricencrypt", (RTS_UINTPTR)cryptoasymmetricencrypt, 1, 0xC14613D0, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptoasymmetricencrypt
	#define EXT_CmpCryptocryptoasymmetricencrypt
	#define GET_CmpCryptocryptoasymmetricencrypt  ERR_OK
	#define CAL_CmpCryptocryptoasymmetricencrypt  cryptoasymmetricencrypt
	#define CHK_CmpCryptocryptoasymmetricencrypt  TRUE
	#define EXP_CmpCryptocryptoasymmetricencrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptoasymmetricencrypt", (RTS_UINTPTR)cryptoasymmetricencrypt, 1, 0xC14613D0, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptoasymmetricencrypt
	#define EXT_cryptoasymmetricencrypt
	#define GET_cryptoasymmetricencrypt(fl)  CAL_CMGETAPI( "cryptoasymmetricencrypt" ) 
	#define CAL_cryptoasymmetricencrypt  cryptoasymmetricencrypt
	#define CHK_cryptoasymmetricencrypt  TRUE
	#define EXP_cryptoasymmetricencrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptoasymmetricencrypt", (RTS_UINTPTR)cryptoasymmetricencrypt, 1, 0xC14613D0, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptoasymmetricencrypt  PFCRYPTOASYMMETRICENCRYPT_IEC pfcryptoasymmetricencrypt;
	#define EXT_cryptoasymmetricencrypt  extern PFCRYPTOASYMMETRICENCRYPT_IEC pfcryptoasymmetricencrypt;
	#define GET_cryptoasymmetricencrypt(fl)  s_pfCMGetAPI2( "cryptoasymmetricencrypt", (RTS_VOID_FCTPTR *)&pfcryptoasymmetricencrypt, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xC14613D0, 0x03050B00)
	#define CAL_cryptoasymmetricencrypt  pfcryptoasymmetricencrypt
	#define CHK_cryptoasymmetricencrypt  (pfcryptoasymmetricencrypt != NULL)
	#define EXP_cryptoasymmetricencrypt   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptoasymmetricencrypt", (RTS_UINTPTR)cryptoasymmetricencrypt, 1, 0xC14613D0, 0x03050B00) 
#endif


/**
 * Calculate the message digest of the given data.
 * :return: Result of the operation
 */
typedef struct tagcryptogeneratehash_struct
{
	RTS_IEC_HANDLE hAlgo;				/* VAR_INPUT */	/* Handle to the algorithm. */
	RtsByteString *pData;				/* VAR_INPUT */	/* Data to calculate the message digest. */
	RtsByteString *pHash;				/* VAR_INPUT */	/* Calculated message digest. */
	RTS_IEC_RESULT CryptoGenerateHash;	/* VAR_OUTPUT */	
} cryptogeneratehash_struct;

void CDECL CDECL_EXT cryptogeneratehash(cryptogeneratehash_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOGENERATEHASH_IEC) (cryptogeneratehash_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGENERATEHASH_NOTIMPLEMENTED)
	#define USE_cryptogeneratehash
	#define EXT_cryptogeneratehash
	#define GET_cryptogeneratehash(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptogeneratehash(p0) 
	#define CHK_cryptogeneratehash  FALSE
	#define EXP_cryptogeneratehash  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptogeneratehash
	#define EXT_cryptogeneratehash
	#define GET_cryptogeneratehash(fl)  CAL_CMGETAPI( "cryptogeneratehash" ) 
	#define CAL_cryptogeneratehash  cryptogeneratehash
	#define CHK_cryptogeneratehash  TRUE
	#define EXP_cryptogeneratehash  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogeneratehash", (RTS_UINTPTR)cryptogeneratehash, 1, 0x34E1A812, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptogeneratehash
	#define EXT_cryptogeneratehash
	#define GET_cryptogeneratehash(fl)  CAL_CMGETAPI( "cryptogeneratehash" ) 
	#define CAL_cryptogeneratehash  cryptogeneratehash
	#define CHK_cryptogeneratehash  TRUE
	#define EXP_cryptogeneratehash  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogeneratehash", (RTS_UINTPTR)cryptogeneratehash, 1, 0x34E1A812, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptogeneratehash
	#define EXT_CmpCryptocryptogeneratehash
	#define GET_CmpCryptocryptogeneratehash  ERR_OK
	#define CAL_CmpCryptocryptogeneratehash  cryptogeneratehash
	#define CHK_CmpCryptocryptogeneratehash  TRUE
	#define EXP_CmpCryptocryptogeneratehash  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogeneratehash", (RTS_UINTPTR)cryptogeneratehash, 1, 0x34E1A812, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptogeneratehash
	#define EXT_cryptogeneratehash
	#define GET_cryptogeneratehash(fl)  CAL_CMGETAPI( "cryptogeneratehash" ) 
	#define CAL_cryptogeneratehash  cryptogeneratehash
	#define CHK_cryptogeneratehash  TRUE
	#define EXP_cryptogeneratehash  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogeneratehash", (RTS_UINTPTR)cryptogeneratehash, 1, 0x34E1A812, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptogeneratehash  PFCRYPTOGENERATEHASH_IEC pfcryptogeneratehash;
	#define EXT_cryptogeneratehash  extern PFCRYPTOGENERATEHASH_IEC pfcryptogeneratehash;
	#define GET_cryptogeneratehash(fl)  s_pfCMGetAPI2( "cryptogeneratehash", (RTS_VOID_FCTPTR *)&pfcryptogeneratehash, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x34E1A812, 0x03050B00)
	#define CAL_cryptogeneratehash  pfcryptogeneratehash
	#define CHK_cryptogeneratehash  (pfcryptogeneratehash != NULL)
	#define EXP_cryptogeneratehash   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogeneratehash", (RTS_UINTPTR)cryptogeneratehash, 1, 0x34E1A812, 0x03050B00) 
#endif


/**
 * Generate a number of cryptographic strong bytes.
 * :return: Function returns ERR_OK if the bytes could be generated
 */
typedef struct tagcryptogeneraterandomnumber_struct
{
	RTS_IEC_UDINT ui32NumOfRandomBytes;	/* VAR_INPUT */	/* Number of bytes needed. */
	RtsByteString *pRandom;				/* VAR_INPUT */	/* Buffer to store the bytes. */
	RTS_IEC_RESULT CryptoGenerateRandomNumber;	/* VAR_OUTPUT */	
} cryptogeneraterandomnumber_struct;

void CDECL CDECL_EXT cryptogeneraterandomnumber(cryptogeneraterandomnumber_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOGENERATERANDOMNUMBER_IEC) (cryptogeneraterandomnumber_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGENERATERANDOMNUMBER_NOTIMPLEMENTED)
	#define USE_cryptogeneraterandomnumber
	#define EXT_cryptogeneraterandomnumber
	#define GET_cryptogeneraterandomnumber(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptogeneraterandomnumber(p0) 
	#define CHK_cryptogeneraterandomnumber  FALSE
	#define EXP_cryptogeneraterandomnumber  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptogeneraterandomnumber
	#define EXT_cryptogeneraterandomnumber
	#define GET_cryptogeneraterandomnumber(fl)  CAL_CMGETAPI( "cryptogeneraterandomnumber" ) 
	#define CAL_cryptogeneraterandomnumber  cryptogeneraterandomnumber
	#define CHK_cryptogeneraterandomnumber  TRUE
	#define EXP_cryptogeneraterandomnumber  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogeneraterandomnumber", (RTS_UINTPTR)cryptogeneraterandomnumber, 1, 0xF9FC5437, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptogeneraterandomnumber
	#define EXT_cryptogeneraterandomnumber
	#define GET_cryptogeneraterandomnumber(fl)  CAL_CMGETAPI( "cryptogeneraterandomnumber" ) 
	#define CAL_cryptogeneraterandomnumber  cryptogeneraterandomnumber
	#define CHK_cryptogeneraterandomnumber  TRUE
	#define EXP_cryptogeneraterandomnumber  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogeneraterandomnumber", (RTS_UINTPTR)cryptogeneraterandomnumber, 1, 0xF9FC5437, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptogeneraterandomnumber
	#define EXT_CmpCryptocryptogeneraterandomnumber
	#define GET_CmpCryptocryptogeneraterandomnumber  ERR_OK
	#define CAL_CmpCryptocryptogeneraterandomnumber  cryptogeneraterandomnumber
	#define CHK_CmpCryptocryptogeneraterandomnumber  TRUE
	#define EXP_CmpCryptocryptogeneraterandomnumber  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogeneraterandomnumber", (RTS_UINTPTR)cryptogeneraterandomnumber, 1, 0xF9FC5437, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptogeneraterandomnumber
	#define EXT_cryptogeneraterandomnumber
	#define GET_cryptogeneraterandomnumber(fl)  CAL_CMGETAPI( "cryptogeneraterandomnumber" ) 
	#define CAL_cryptogeneraterandomnumber  cryptogeneraterandomnumber
	#define CHK_cryptogeneraterandomnumber  TRUE
	#define EXP_cryptogeneraterandomnumber  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogeneraterandomnumber", (RTS_UINTPTR)cryptogeneraterandomnumber, 1, 0xF9FC5437, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptogeneraterandomnumber  PFCRYPTOGENERATERANDOMNUMBER_IEC pfcryptogeneraterandomnumber;
	#define EXT_cryptogeneraterandomnumber  extern PFCRYPTOGENERATERANDOMNUMBER_IEC pfcryptogeneraterandomnumber;
	#define GET_cryptogeneraterandomnumber(fl)  s_pfCMGetAPI2( "cryptogeneraterandomnumber", (RTS_VOID_FCTPTR *)&pfcryptogeneraterandomnumber, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xF9FC5437, 0x03050B00)
	#define CAL_cryptogeneraterandomnumber  pfcryptogeneraterandomnumber
	#define CHK_cryptogeneraterandomnumber  (pfcryptogeneraterandomnumber != NULL)
	#define EXP_cryptogeneraterandomnumber   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogeneraterandomnumber", (RTS_UINTPTR)cryptogeneraterandomnumber, 1, 0xF9FC5437, 0x03050B00) 
#endif


/**
 * Get a handle to the algorithm using a specific ID
 * :return: Handle to the crypto algorithm
 */
typedef struct tagcryptogetalgorithmbyid_struct
{
	RTS_IEC_UDINT ui32CryptoID;			/* VAR_INPUT, Enum: RTSCRYPTOID */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Result of the operation. Can be NULL. */
	RTS_IEC_HANDLE CryptoGetAlgorithmById;	/* VAR_OUTPUT */	
} cryptogetalgorithmbyid_struct;

void CDECL CDECL_EXT cryptogetalgorithmbyid(cryptogetalgorithmbyid_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOGETALGORITHMBYID_IEC) (cryptogetalgorithmbyid_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGETALGORITHMBYID_NOTIMPLEMENTED)
	#define USE_cryptogetalgorithmbyid
	#define EXT_cryptogetalgorithmbyid
	#define GET_cryptogetalgorithmbyid(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptogetalgorithmbyid(p0) 
	#define CHK_cryptogetalgorithmbyid  FALSE
	#define EXP_cryptogetalgorithmbyid  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptogetalgorithmbyid
	#define EXT_cryptogetalgorithmbyid
	#define GET_cryptogetalgorithmbyid(fl)  CAL_CMGETAPI( "cryptogetalgorithmbyid" ) 
	#define CAL_cryptogetalgorithmbyid  cryptogetalgorithmbyid
	#define CHK_cryptogetalgorithmbyid  TRUE
	#define EXP_cryptogetalgorithmbyid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetalgorithmbyid", (RTS_UINTPTR)cryptogetalgorithmbyid, 1, 0x3D0776D3, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptogetalgorithmbyid
	#define EXT_cryptogetalgorithmbyid
	#define GET_cryptogetalgorithmbyid(fl)  CAL_CMGETAPI( "cryptogetalgorithmbyid" ) 
	#define CAL_cryptogetalgorithmbyid  cryptogetalgorithmbyid
	#define CHK_cryptogetalgorithmbyid  TRUE
	#define EXP_cryptogetalgorithmbyid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetalgorithmbyid", (RTS_UINTPTR)cryptogetalgorithmbyid, 1, 0x3D0776D3, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptogetalgorithmbyid
	#define EXT_CmpCryptocryptogetalgorithmbyid
	#define GET_CmpCryptocryptogetalgorithmbyid  ERR_OK
	#define CAL_CmpCryptocryptogetalgorithmbyid  cryptogetalgorithmbyid
	#define CHK_CmpCryptocryptogetalgorithmbyid  TRUE
	#define EXP_CmpCryptocryptogetalgorithmbyid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetalgorithmbyid", (RTS_UINTPTR)cryptogetalgorithmbyid, 1, 0x3D0776D3, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptogetalgorithmbyid
	#define EXT_cryptogetalgorithmbyid
	#define GET_cryptogetalgorithmbyid(fl)  CAL_CMGETAPI( "cryptogetalgorithmbyid" ) 
	#define CAL_cryptogetalgorithmbyid  cryptogetalgorithmbyid
	#define CHK_cryptogetalgorithmbyid  TRUE
	#define EXP_cryptogetalgorithmbyid  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetalgorithmbyid", (RTS_UINTPTR)cryptogetalgorithmbyid, 1, 0x3D0776D3, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptogetalgorithmbyid  PFCRYPTOGETALGORITHMBYID_IEC pfcryptogetalgorithmbyid;
	#define EXT_cryptogetalgorithmbyid  extern PFCRYPTOGETALGORITHMBYID_IEC pfcryptogetalgorithmbyid;
	#define GET_cryptogetalgorithmbyid(fl)  s_pfCMGetAPI2( "cryptogetalgorithmbyid", (RTS_VOID_FCTPTR *)&pfcryptogetalgorithmbyid, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x3D0776D3, 0x03050B00)
	#define CAL_cryptogetalgorithmbyid  pfcryptogetalgorithmbyid
	#define CHK_cryptogetalgorithmbyid  (pfcryptogetalgorithmbyid != NULL)
	#define EXP_cryptogetalgorithmbyid   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetalgorithmbyid", (RTS_UINTPTR)cryptogetalgorithmbyid, 1, 0x3D0776D3, 0x03050B00) 
#endif


/**
 * Get the asymmetric key size in bits.
 * :return: Size of the asymmetric key in bits
 */
typedef struct tagcryptogetasymmetrickeylength_struct
{
	RtsCryptoKey asymmetricKey;			/* VAR_INPUT */	/* Asymmetric key of intrest. */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Operation Result */
	RTS_IEC_UDINT CryptoGetAsymmetricKeyLength;	/* VAR_OUTPUT */	
} cryptogetasymmetrickeylength_struct;

void CDECL CDECL_EXT cryptogetasymmetrickeylength(cryptogetasymmetrickeylength_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOGETASYMMETRICKEYLENGTH_IEC) (cryptogetasymmetrickeylength_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGETASYMMETRICKEYLENGTH_NOTIMPLEMENTED)
	#define USE_cryptogetasymmetrickeylength
	#define EXT_cryptogetasymmetrickeylength
	#define GET_cryptogetasymmetrickeylength(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptogetasymmetrickeylength(p0) 
	#define CHK_cryptogetasymmetrickeylength  FALSE
	#define EXP_cryptogetasymmetrickeylength  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptogetasymmetrickeylength
	#define EXT_cryptogetasymmetrickeylength
	#define GET_cryptogetasymmetrickeylength(fl)  CAL_CMGETAPI( "cryptogetasymmetrickeylength" ) 
	#define CAL_cryptogetasymmetrickeylength  cryptogetasymmetrickeylength
	#define CHK_cryptogetasymmetrickeylength  TRUE
	#define EXP_cryptogetasymmetrickeylength  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetasymmetrickeylength", (RTS_UINTPTR)cryptogetasymmetrickeylength, 1, 0, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptogetasymmetrickeylength
	#define EXT_cryptogetasymmetrickeylength
	#define GET_cryptogetasymmetrickeylength(fl)  CAL_CMGETAPI( "cryptogetasymmetrickeylength" ) 
	#define CAL_cryptogetasymmetrickeylength  cryptogetasymmetrickeylength
	#define CHK_cryptogetasymmetrickeylength  TRUE
	#define EXP_cryptogetasymmetrickeylength  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetasymmetrickeylength", (RTS_UINTPTR)cryptogetasymmetrickeylength, 1, 0, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptogetasymmetrickeylength
	#define EXT_CmpCryptocryptogetasymmetrickeylength
	#define GET_CmpCryptocryptogetasymmetrickeylength  ERR_OK
	#define CAL_CmpCryptocryptogetasymmetrickeylength  cryptogetasymmetrickeylength
	#define CHK_CmpCryptocryptogetasymmetrickeylength  TRUE
	#define EXP_CmpCryptocryptogetasymmetrickeylength  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetasymmetrickeylength", (RTS_UINTPTR)cryptogetasymmetrickeylength, 1, 0, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptogetasymmetrickeylength
	#define EXT_cryptogetasymmetrickeylength
	#define GET_cryptogetasymmetrickeylength(fl)  CAL_CMGETAPI( "cryptogetasymmetrickeylength" ) 
	#define CAL_cryptogetasymmetrickeylength  cryptogetasymmetrickeylength
	#define CHK_cryptogetasymmetrickeylength  TRUE
	#define EXP_cryptogetasymmetrickeylength  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetasymmetrickeylength", (RTS_UINTPTR)cryptogetasymmetrickeylength, 1, 0, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptogetasymmetrickeylength  PFCRYPTOGETASYMMETRICKEYLENGTH_IEC pfcryptogetasymmetrickeylength;
	#define EXT_cryptogetasymmetrickeylength  extern PFCRYPTOGETASYMMETRICKEYLENGTH_IEC pfcryptogetasymmetrickeylength;
	#define GET_cryptogetasymmetrickeylength(fl)  s_pfCMGetAPI2( "cryptogetasymmetrickeylength", (RTS_VOID_FCTPTR *)&pfcryptogetasymmetrickeylength, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0, 0x03050B00)
	#define CAL_cryptogetasymmetrickeylength  pfcryptogetasymmetrickeylength
	#define CHK_cryptogetasymmetrickeylength  (pfcryptogetasymmetrickeylength != NULL)
	#define EXP_cryptogetasymmetrickeylength   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetasymmetrickeylength", (RTS_UINTPTR)cryptogetasymmetrickeylength, 1, 0, 0x03050B00) 
#endif


/**
 * Get the first algorthm matching the type given. Use this with the GetNext function to iterate over algorthmes of a specific type.
 * :return: Handle to the crypto algorithm
 */
typedef struct tagcryptogetfirstalgorithm_struct
{
	RTS_IEC_UDINT ui32CryptoType;		/* VAR_INPUT, Enum: RTSCRYPTOTYPE */
	RTS_IEC_STRING *pszName;			/* VAR_INPUT */	/* Name of the algorthm. Can be NULL. */
	RTS_IEC_DINT i32MaxNameLen;			/* VAR_INPUT */	/* Maximum length of the name buffer */
	RTS_IEC_UDINT *pui32CryptoID;		/* VAR_INPUT */	/* ID of the algorithm. */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Result of the operation. Can be NULL. */
	RTS_IEC_HANDLE CryptoGetFirstAlgorithm;	/* VAR_OUTPUT */	
} cryptogetfirstalgorithm_struct;

void CDECL CDECL_EXT cryptogetfirstalgorithm(cryptogetfirstalgorithm_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOGETFIRSTALGORITHM_IEC) (cryptogetfirstalgorithm_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGETFIRSTALGORITHM_NOTIMPLEMENTED)
	#define USE_cryptogetfirstalgorithm
	#define EXT_cryptogetfirstalgorithm
	#define GET_cryptogetfirstalgorithm(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptogetfirstalgorithm(p0) 
	#define CHK_cryptogetfirstalgorithm  FALSE
	#define EXP_cryptogetfirstalgorithm  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptogetfirstalgorithm
	#define EXT_cryptogetfirstalgorithm
	#define GET_cryptogetfirstalgorithm(fl)  CAL_CMGETAPI( "cryptogetfirstalgorithm" ) 
	#define CAL_cryptogetfirstalgorithm  cryptogetfirstalgorithm
	#define CHK_cryptogetfirstalgorithm  TRUE
	#define EXP_cryptogetfirstalgorithm  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetfirstalgorithm", (RTS_UINTPTR)cryptogetfirstalgorithm, 1, 0xC8A4A345, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptogetfirstalgorithm
	#define EXT_cryptogetfirstalgorithm
	#define GET_cryptogetfirstalgorithm(fl)  CAL_CMGETAPI( "cryptogetfirstalgorithm" ) 
	#define CAL_cryptogetfirstalgorithm  cryptogetfirstalgorithm
	#define CHK_cryptogetfirstalgorithm  TRUE
	#define EXP_cryptogetfirstalgorithm  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetfirstalgorithm", (RTS_UINTPTR)cryptogetfirstalgorithm, 1, 0xC8A4A345, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptogetfirstalgorithm
	#define EXT_CmpCryptocryptogetfirstalgorithm
	#define GET_CmpCryptocryptogetfirstalgorithm  ERR_OK
	#define CAL_CmpCryptocryptogetfirstalgorithm  cryptogetfirstalgorithm
	#define CHK_CmpCryptocryptogetfirstalgorithm  TRUE
	#define EXP_CmpCryptocryptogetfirstalgorithm  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetfirstalgorithm", (RTS_UINTPTR)cryptogetfirstalgorithm, 1, 0xC8A4A345, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptogetfirstalgorithm
	#define EXT_cryptogetfirstalgorithm
	#define GET_cryptogetfirstalgorithm(fl)  CAL_CMGETAPI( "cryptogetfirstalgorithm" ) 
	#define CAL_cryptogetfirstalgorithm  cryptogetfirstalgorithm
	#define CHK_cryptogetfirstalgorithm  TRUE
	#define EXP_cryptogetfirstalgorithm  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetfirstalgorithm", (RTS_UINTPTR)cryptogetfirstalgorithm, 1, 0xC8A4A345, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptogetfirstalgorithm  PFCRYPTOGETFIRSTALGORITHM_IEC pfcryptogetfirstalgorithm;
	#define EXT_cryptogetfirstalgorithm  extern PFCRYPTOGETFIRSTALGORITHM_IEC pfcryptogetfirstalgorithm;
	#define GET_cryptogetfirstalgorithm(fl)  s_pfCMGetAPI2( "cryptogetfirstalgorithm", (RTS_VOID_FCTPTR *)&pfcryptogetfirstalgorithm, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xC8A4A345, 0x03050B00)
	#define CAL_cryptogetfirstalgorithm  pfcryptogetfirstalgorithm
	#define CHK_cryptogetfirstalgorithm  (pfcryptogetfirstalgorithm != NULL)
	#define EXP_cryptogetfirstalgorithm   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetfirstalgorithm", (RTS_UINTPTR)cryptogetfirstalgorithm, 1, 0xC8A4A345, 0x03050B00) 
#endif


/**
 * Get the next algorthm matching the type given.
 * :return:  Handle to the crypto algorithm
 */
typedef struct tagcryptogetnextalgorithm_struct
{
	RTS_IEC_HANDLE hCrypto;				/* VAR_INPUT */	/* Handle to the last algorithem. */
	RTS_IEC_UDINT ui32CryptoType;		/* VAR_INPUT, Enum: RTSCRYPTOTYPE */
	RTS_IEC_STRING *pszName;			/* VAR_INPUT */	/* Name of the agorithm. Can be NULL. */
	RTS_IEC_DINT i32MaxNameLen;			/* VAR_INPUT */	/* Maximum length of the name buffer */
	RTS_IEC_UDINT *pui32CryptoID;		/* VAR_INPUT */	/* ID of the agorithm. */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Result of the operation. Can be NULL. */
	RTS_IEC_HANDLE CryptoGetNextAlgorithm;	/* VAR_OUTPUT */	
} cryptogetnextalgorithm_struct;

void CDECL CDECL_EXT cryptogetnextalgorithm(cryptogetnextalgorithm_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOGETNEXTALGORITHM_IEC) (cryptogetnextalgorithm_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOGETNEXTALGORITHM_NOTIMPLEMENTED)
	#define USE_cryptogetnextalgorithm
	#define EXT_cryptogetnextalgorithm
	#define GET_cryptogetnextalgorithm(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptogetnextalgorithm(p0) 
	#define CHK_cryptogetnextalgorithm  FALSE
	#define EXP_cryptogetnextalgorithm  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptogetnextalgorithm
	#define EXT_cryptogetnextalgorithm
	#define GET_cryptogetnextalgorithm(fl)  CAL_CMGETAPI( "cryptogetnextalgorithm" ) 
	#define CAL_cryptogetnextalgorithm  cryptogetnextalgorithm
	#define CHK_cryptogetnextalgorithm  TRUE
	#define EXP_cryptogetnextalgorithm  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetnextalgorithm", (RTS_UINTPTR)cryptogetnextalgorithm, 1, 0x79EC29C8, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptogetnextalgorithm
	#define EXT_cryptogetnextalgorithm
	#define GET_cryptogetnextalgorithm(fl)  CAL_CMGETAPI( "cryptogetnextalgorithm" ) 
	#define CAL_cryptogetnextalgorithm  cryptogetnextalgorithm
	#define CHK_cryptogetnextalgorithm  TRUE
	#define EXP_cryptogetnextalgorithm  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetnextalgorithm", (RTS_UINTPTR)cryptogetnextalgorithm, 1, 0x79EC29C8, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptogetnextalgorithm
	#define EXT_CmpCryptocryptogetnextalgorithm
	#define GET_CmpCryptocryptogetnextalgorithm  ERR_OK
	#define CAL_CmpCryptocryptogetnextalgorithm  cryptogetnextalgorithm
	#define CHK_CmpCryptocryptogetnextalgorithm  TRUE
	#define EXP_CmpCryptocryptogetnextalgorithm  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetnextalgorithm", (RTS_UINTPTR)cryptogetnextalgorithm, 1, 0x79EC29C8, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptogetnextalgorithm
	#define EXT_cryptogetnextalgorithm
	#define GET_cryptogetnextalgorithm(fl)  CAL_CMGETAPI( "cryptogetnextalgorithm" ) 
	#define CAL_cryptogetnextalgorithm  cryptogetnextalgorithm
	#define CHK_cryptogetnextalgorithm  TRUE
	#define EXP_cryptogetnextalgorithm  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetnextalgorithm", (RTS_UINTPTR)cryptogetnextalgorithm, 1, 0x79EC29C8, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptogetnextalgorithm  PFCRYPTOGETNEXTALGORITHM_IEC pfcryptogetnextalgorithm;
	#define EXT_cryptogetnextalgorithm  extern PFCRYPTOGETNEXTALGORITHM_IEC pfcryptogetnextalgorithm;
	#define GET_cryptogetnextalgorithm(fl)  s_pfCMGetAPI2( "cryptogetnextalgorithm", (RTS_VOID_FCTPTR *)&pfcryptogetnextalgorithm, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x79EC29C8, 0x03050B00)
	#define CAL_cryptogetnextalgorithm  pfcryptogetnextalgorithm
	#define CHK_cryptogetnextalgorithm  (pfcryptogetnextalgorithm != NULL)
	#define EXP_cryptogetnextalgorithm   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptogetnextalgorithm", (RTS_UINTPTR)cryptogetnextalgorithm, 1, 0x79EC29C8, 0x03050B00) 
#endif


/**
 *Generate a hashed message authentication code (HMAC) OF the given data AND key.
 * :return: Result of the operation
 */
typedef struct tagcryptohmacsign_struct
{
	RTS_IEC_HANDLE hAlgo;				/* VAR_INPUT */	/* Handle to the algorithm. */
	RtsByteString *pData;				/* VAR_INPUT */	/* Data to calculate the HMAC. */
	RtsCryptoKey key;					/* VAR_INPUT */	/* Key to be used for the HMAC calcuation */
	RtsByteString *pSignature;			/* VAR_INPUT */	/* Calculated HMAC */
	RTS_IEC_RESULT CryptoHMACSign;		/* VAR_OUTPUT */	
} cryptohmacsign_struct;

void CDECL CDECL_EXT cryptohmacsign(cryptohmacsign_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOHMACSIGN_IEC) (cryptohmacsign_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOHMACSIGN_NOTIMPLEMENTED)
	#define USE_cryptohmacsign
	#define EXT_cryptohmacsign
	#define GET_cryptohmacsign(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptohmacsign(p0) 
	#define CHK_cryptohmacsign  FALSE
	#define EXP_cryptohmacsign  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptohmacsign
	#define EXT_cryptohmacsign
	#define GET_cryptohmacsign(fl)  CAL_CMGETAPI( "cryptohmacsign" ) 
	#define CAL_cryptohmacsign  cryptohmacsign
	#define CHK_cryptohmacsign  TRUE
	#define EXP_cryptohmacsign  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptohmacsign", (RTS_UINTPTR)cryptohmacsign, 1, 0x931A9470, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptohmacsign
	#define EXT_cryptohmacsign
	#define GET_cryptohmacsign(fl)  CAL_CMGETAPI( "cryptohmacsign" ) 
	#define CAL_cryptohmacsign  cryptohmacsign
	#define CHK_cryptohmacsign  TRUE
	#define EXP_cryptohmacsign  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptohmacsign", (RTS_UINTPTR)cryptohmacsign, 1, 0x931A9470, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptohmacsign
	#define EXT_CmpCryptocryptohmacsign
	#define GET_CmpCryptocryptohmacsign  ERR_OK
	#define CAL_CmpCryptocryptohmacsign  cryptohmacsign
	#define CHK_CmpCryptocryptohmacsign  TRUE
	#define EXP_CmpCryptocryptohmacsign  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptohmacsign", (RTS_UINTPTR)cryptohmacsign, 1, 0x931A9470, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptohmacsign
	#define EXT_cryptohmacsign
	#define GET_cryptohmacsign(fl)  CAL_CMGETAPI( "cryptohmacsign" ) 
	#define CAL_cryptohmacsign  cryptohmacsign
	#define CHK_cryptohmacsign  TRUE
	#define EXP_cryptohmacsign  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptohmacsign", (RTS_UINTPTR)cryptohmacsign, 1, 0x931A9470, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptohmacsign  PFCRYPTOHMACSIGN_IEC pfcryptohmacsign;
	#define EXT_cryptohmacsign  extern PFCRYPTOHMACSIGN_IEC pfcryptohmacsign;
	#define GET_cryptohmacsign(fl)  s_pfCMGetAPI2( "cryptohmacsign", (RTS_VOID_FCTPTR *)&pfcryptohmacsign, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x931A9470, 0x03050B00)
	#define CAL_cryptohmacsign  pfcryptohmacsign
	#define CHK_cryptohmacsign  (pfcryptohmacsign != NULL)
	#define EXP_cryptohmacsign   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptohmacsign", (RTS_UINTPTR)cryptohmacsign, 1, 0x931A9470, 0x03050B00) 
#endif


/**
 * Verify a recieved hashed message authentication code (HMAC).
 * :return: Function returns ERR_OK if the HMAC is valid.
 */
typedef struct tagcryptohmacverify_struct
{
	RTS_IEC_HANDLE hAlgo;				/* VAR_INPUT */	/* Handle to the algorithm. */
	RtsByteString *pData;				/* VAR_INPUT */	/* Data to calculate the HMAC. */
	RtsCryptoKey key;					/* VAR_INPUT */	/* Key to be used for the HMAC calcuation */
	RtsByteString *pSignature;			/* VAR_INPUT */	/* Recieved HMAC */
	RTS_IEC_RESULT CryptoHMACVerify;	/* VAR_OUTPUT */	
} cryptohmacverify_struct;

void CDECL CDECL_EXT cryptohmacverify(cryptohmacverify_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOHMACVERIFY_IEC) (cryptohmacverify_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOHMACVERIFY_NOTIMPLEMENTED)
	#define USE_cryptohmacverify
	#define EXT_cryptohmacverify
	#define GET_cryptohmacverify(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptohmacverify(p0) 
	#define CHK_cryptohmacverify  FALSE
	#define EXP_cryptohmacverify  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptohmacverify
	#define EXT_cryptohmacverify
	#define GET_cryptohmacverify(fl)  CAL_CMGETAPI( "cryptohmacverify" ) 
	#define CAL_cryptohmacverify  cryptohmacverify
	#define CHK_cryptohmacverify  TRUE
	#define EXP_cryptohmacverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptohmacverify", (RTS_UINTPTR)cryptohmacverify, 1, 0x49EEB4A9, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptohmacverify
	#define EXT_cryptohmacverify
	#define GET_cryptohmacverify(fl)  CAL_CMGETAPI( "cryptohmacverify" ) 
	#define CAL_cryptohmacverify  cryptohmacverify
	#define CHK_cryptohmacverify  TRUE
	#define EXP_cryptohmacverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptohmacverify", (RTS_UINTPTR)cryptohmacverify, 1, 0x49EEB4A9, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptohmacverify
	#define EXT_CmpCryptocryptohmacverify
	#define GET_CmpCryptocryptohmacverify  ERR_OK
	#define CAL_CmpCryptocryptohmacverify  cryptohmacverify
	#define CHK_CmpCryptocryptohmacverify  TRUE
	#define EXP_CmpCryptocryptohmacverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptohmacverify", (RTS_UINTPTR)cryptohmacverify, 1, 0x49EEB4A9, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptohmacverify
	#define EXT_cryptohmacverify
	#define GET_cryptohmacverify(fl)  CAL_CMGETAPI( "cryptohmacverify" ) 
	#define CAL_cryptohmacverify  cryptohmacverify
	#define CHK_cryptohmacverify  TRUE
	#define EXP_cryptohmacverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptohmacverify", (RTS_UINTPTR)cryptohmacverify, 1, 0x49EEB4A9, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptohmacverify  PFCRYPTOHMACVERIFY_IEC pfcryptohmacverify;
	#define EXT_cryptohmacverify  extern PFCRYPTOHMACVERIFY_IEC pfcryptohmacverify;
	#define GET_cryptohmacverify(fl)  s_pfCMGetAPI2( "cryptohmacverify", (RTS_VOID_FCTPTR *)&pfcryptohmacverify, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x49EEB4A9, 0x03050B00)
	#define CAL_cryptohmacverify  pfcryptohmacverify
	#define CHK_cryptohmacverify  (pfcryptohmacverify != NULL)
	#define EXP_cryptohmacverify   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptohmacverify", (RTS_UINTPTR)cryptohmacverify, 1, 0x49EEB4A9, 0x03050B00) 
#endif


/**
 * <description>cryptokeyexit</description>
 */
typedef struct tagcryptokeyexit_struct
{
	RtsCryptoKey *pKey;					/* VAR_INPUT */	
	RTS_IEC_RESULT CryptoKeyExit;		/* VAR_OUTPUT */	
} cryptokeyexit_struct;

void CDECL CDECL_EXT cryptokeyexit(cryptokeyexit_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOKEYEXIT_IEC) (cryptokeyexit_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOKEYEXIT_NOTIMPLEMENTED)
	#define USE_cryptokeyexit
	#define EXT_cryptokeyexit
	#define GET_cryptokeyexit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptokeyexit(p0) 
	#define CHK_cryptokeyexit  FALSE
	#define EXP_cryptokeyexit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptokeyexit
	#define EXT_cryptokeyexit
	#define GET_cryptokeyexit(fl)  CAL_CMGETAPI( "cryptokeyexit" ) 
	#define CAL_cryptokeyexit  cryptokeyexit
	#define CHK_cryptokeyexit  TRUE
	#define EXP_cryptokeyexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptokeyexit", (RTS_UINTPTR)cryptokeyexit, 1, 0xD71ED5F3, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptokeyexit
	#define EXT_cryptokeyexit
	#define GET_cryptokeyexit(fl)  CAL_CMGETAPI( "cryptokeyexit" ) 
	#define CAL_cryptokeyexit  cryptokeyexit
	#define CHK_cryptokeyexit  TRUE
	#define EXP_cryptokeyexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptokeyexit", (RTS_UINTPTR)cryptokeyexit, 1, 0xD71ED5F3, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptokeyexit
	#define EXT_CmpCryptocryptokeyexit
	#define GET_CmpCryptocryptokeyexit  ERR_OK
	#define CAL_CmpCryptocryptokeyexit  cryptokeyexit
	#define CHK_CmpCryptocryptokeyexit  TRUE
	#define EXP_CmpCryptocryptokeyexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptokeyexit", (RTS_UINTPTR)cryptokeyexit, 1, 0xD71ED5F3, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptokeyexit
	#define EXT_cryptokeyexit
	#define GET_cryptokeyexit(fl)  CAL_CMGETAPI( "cryptokeyexit" ) 
	#define CAL_cryptokeyexit  cryptokeyexit
	#define CHK_cryptokeyexit  TRUE
	#define EXP_cryptokeyexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptokeyexit", (RTS_UINTPTR)cryptokeyexit, 1, 0xD71ED5F3, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptokeyexit  PFCRYPTOKEYEXIT_IEC pfcryptokeyexit;
	#define EXT_cryptokeyexit  extern PFCRYPTOKEYEXIT_IEC pfcryptokeyexit;
	#define GET_cryptokeyexit(fl)  s_pfCMGetAPI2( "cryptokeyexit", (RTS_VOID_FCTPTR *)&pfcryptokeyexit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xD71ED5F3, 0x03050B00)
	#define CAL_cryptokeyexit  pfcryptokeyexit
	#define CHK_cryptokeyexit  (pfcryptokeyexit != NULL)
	#define EXP_cryptokeyexit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptokeyexit", (RTS_UINTPTR)cryptokeyexit, 1, 0xD71ED5F3, 0x03050B00) 
#endif


/**
 * <description>cryptokeyinit</description>
 */
typedef struct tagcryptokeyinit_struct
{
	RtsCryptoKey *pKey;					/* VAR_INPUT */	
	RtsCryptoKeyStorage key;			/* VAR_INPUT */	
	RTS_IEC_DINT keyType;				/* VAR_INPUT, Enum: RTSCRYPTOKEYTYPE */
	RTS_IEC_RESULT CryptoKeyInit;		/* VAR_OUTPUT */	
} cryptokeyinit_struct;

void CDECL CDECL_EXT cryptokeyinit(cryptokeyinit_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOKEYINIT_IEC) (cryptokeyinit_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOKEYINIT_NOTIMPLEMENTED)
	#define USE_cryptokeyinit
	#define EXT_cryptokeyinit
	#define GET_cryptokeyinit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptokeyinit(p0) 
	#define CHK_cryptokeyinit  FALSE
	#define EXP_cryptokeyinit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptokeyinit
	#define EXT_cryptokeyinit
	#define GET_cryptokeyinit(fl)  CAL_CMGETAPI( "cryptokeyinit" ) 
	#define CAL_cryptokeyinit  cryptokeyinit
	#define CHK_cryptokeyinit  TRUE
	#define EXP_cryptokeyinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptokeyinit", (RTS_UINTPTR)cryptokeyinit, 1, 0xE8E228B3, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptokeyinit
	#define EXT_cryptokeyinit
	#define GET_cryptokeyinit(fl)  CAL_CMGETAPI( "cryptokeyinit" ) 
	#define CAL_cryptokeyinit  cryptokeyinit
	#define CHK_cryptokeyinit  TRUE
	#define EXP_cryptokeyinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptokeyinit", (RTS_UINTPTR)cryptokeyinit, 1, 0xE8E228B3, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptokeyinit
	#define EXT_CmpCryptocryptokeyinit
	#define GET_CmpCryptocryptokeyinit  ERR_OK
	#define CAL_CmpCryptocryptokeyinit  cryptokeyinit
	#define CHK_CmpCryptocryptokeyinit  TRUE
	#define EXP_CmpCryptocryptokeyinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptokeyinit", (RTS_UINTPTR)cryptokeyinit, 1, 0xE8E228B3, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptokeyinit
	#define EXT_cryptokeyinit
	#define GET_cryptokeyinit(fl)  CAL_CMGETAPI( "cryptokeyinit" ) 
	#define CAL_cryptokeyinit  cryptokeyinit
	#define CHK_cryptokeyinit  TRUE
	#define EXP_cryptokeyinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptokeyinit", (RTS_UINTPTR)cryptokeyinit, 1, 0xE8E228B3, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptokeyinit  PFCRYPTOKEYINIT_IEC pfcryptokeyinit;
	#define EXT_cryptokeyinit  extern PFCRYPTOKEYINIT_IEC pfcryptokeyinit;
	#define GET_cryptokeyinit(fl)  s_pfCMGetAPI2( "cryptokeyinit", (RTS_VOID_FCTPTR *)&pfcryptokeyinit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xE8E228B3, 0x03050B00)
	#define CAL_cryptokeyinit  pfcryptokeyinit
	#define CHK_cryptokeyinit  (pfcryptokeyinit != NULL)
	#define EXP_cryptokeyinit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptokeyinit", (RTS_UINTPTR)cryptokeyinit, 1, 0xE8E228B3, 0x03050B00) 
#endif


/**
 * <description>cryptortsbytestringexit</description>
 */
typedef struct tagcryptortsbytestringexit_struct
{
	RtsByteString *pByteString;			/* VAR_INPUT */	
	RTS_IEC_RESULT CryptoRtsByteStringExit;	/* VAR_OUTPUT */	
} cryptortsbytestringexit_struct;

void CDECL CDECL_EXT cryptortsbytestringexit(cryptortsbytestringexit_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTORTSBYTESTRINGEXIT_IEC) (cryptortsbytestringexit_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTORTSBYTESTRINGEXIT_NOTIMPLEMENTED)
	#define USE_cryptortsbytestringexit
	#define EXT_cryptortsbytestringexit
	#define GET_cryptortsbytestringexit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptortsbytestringexit(p0) 
	#define CHK_cryptortsbytestringexit  FALSE
	#define EXP_cryptortsbytestringexit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptortsbytestringexit
	#define EXT_cryptortsbytestringexit
	#define GET_cryptortsbytestringexit(fl)  CAL_CMGETAPI( "cryptortsbytestringexit" ) 
	#define CAL_cryptortsbytestringexit  cryptortsbytestringexit
	#define CHK_cryptortsbytestringexit  TRUE
	#define EXP_cryptortsbytestringexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptortsbytestringexit", (RTS_UINTPTR)cryptortsbytestringexit, 1, 0x16B297C5, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptortsbytestringexit
	#define EXT_cryptortsbytestringexit
	#define GET_cryptortsbytestringexit(fl)  CAL_CMGETAPI( "cryptortsbytestringexit" ) 
	#define CAL_cryptortsbytestringexit  cryptortsbytestringexit
	#define CHK_cryptortsbytestringexit  TRUE
	#define EXP_cryptortsbytestringexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptortsbytestringexit", (RTS_UINTPTR)cryptortsbytestringexit, 1, 0x16B297C5, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptortsbytestringexit
	#define EXT_CmpCryptocryptortsbytestringexit
	#define GET_CmpCryptocryptortsbytestringexit  ERR_OK
	#define CAL_CmpCryptocryptortsbytestringexit  cryptortsbytestringexit
	#define CHK_CmpCryptocryptortsbytestringexit  TRUE
	#define EXP_CmpCryptocryptortsbytestringexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptortsbytestringexit", (RTS_UINTPTR)cryptortsbytestringexit, 1, 0x16B297C5, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptortsbytestringexit
	#define EXT_cryptortsbytestringexit
	#define GET_cryptortsbytestringexit(fl)  CAL_CMGETAPI( "cryptortsbytestringexit" ) 
	#define CAL_cryptortsbytestringexit  cryptortsbytestringexit
	#define CHK_cryptortsbytestringexit  TRUE
	#define EXP_cryptortsbytestringexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptortsbytestringexit", (RTS_UINTPTR)cryptortsbytestringexit, 1, 0x16B297C5, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptortsbytestringexit  PFCRYPTORTSBYTESTRINGEXIT_IEC pfcryptortsbytestringexit;
	#define EXT_cryptortsbytestringexit  extern PFCRYPTORTSBYTESTRINGEXIT_IEC pfcryptortsbytestringexit;
	#define GET_cryptortsbytestringexit(fl)  s_pfCMGetAPI2( "cryptortsbytestringexit", (RTS_VOID_FCTPTR *)&pfcryptortsbytestringexit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x16B297C5, 0x03050B00)
	#define CAL_cryptortsbytestringexit  pfcryptortsbytestringexit
	#define CHK_cryptortsbytestringexit  (pfcryptortsbytestringexit != NULL)
	#define EXP_cryptortsbytestringexit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptortsbytestringexit", (RTS_UINTPTR)cryptortsbytestringexit, 1, 0x16B297C5, 0x03050B00) 
#endif


/**
 * <description>cryptortsbytestringinit</description>
 */
typedef struct tagcryptortsbytestringinit_struct
{
	RtsByteString *pByteString;			/* VAR_INPUT */	
	RTS_IEC_RESULT CryptoRtsByteStringInit;	/* VAR_OUTPUT */	
} cryptortsbytestringinit_struct;

void CDECL CDECL_EXT cryptortsbytestringinit(cryptortsbytestringinit_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTORTSBYTESTRINGINIT_IEC) (cryptortsbytestringinit_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTORTSBYTESTRINGINIT_NOTIMPLEMENTED)
	#define USE_cryptortsbytestringinit
	#define EXT_cryptortsbytestringinit
	#define GET_cryptortsbytestringinit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptortsbytestringinit(p0) 
	#define CHK_cryptortsbytestringinit  FALSE
	#define EXP_cryptortsbytestringinit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptortsbytestringinit
	#define EXT_cryptortsbytestringinit
	#define GET_cryptortsbytestringinit(fl)  CAL_CMGETAPI( "cryptortsbytestringinit" ) 
	#define CAL_cryptortsbytestringinit  cryptortsbytestringinit
	#define CHK_cryptortsbytestringinit  TRUE
	#define EXP_cryptortsbytestringinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptortsbytestringinit", (RTS_UINTPTR)cryptortsbytestringinit, 1, 0xC1B4C2F9, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptortsbytestringinit
	#define EXT_cryptortsbytestringinit
	#define GET_cryptortsbytestringinit(fl)  CAL_CMGETAPI( "cryptortsbytestringinit" ) 
	#define CAL_cryptortsbytestringinit  cryptortsbytestringinit
	#define CHK_cryptortsbytestringinit  TRUE
	#define EXP_cryptortsbytestringinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptortsbytestringinit", (RTS_UINTPTR)cryptortsbytestringinit, 1, 0xC1B4C2F9, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptortsbytestringinit
	#define EXT_CmpCryptocryptortsbytestringinit
	#define GET_CmpCryptocryptortsbytestringinit  ERR_OK
	#define CAL_CmpCryptocryptortsbytestringinit  cryptortsbytestringinit
	#define CHK_CmpCryptocryptortsbytestringinit  TRUE
	#define EXP_CmpCryptocryptortsbytestringinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptortsbytestringinit", (RTS_UINTPTR)cryptortsbytestringinit, 1, 0xC1B4C2F9, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptortsbytestringinit
	#define EXT_cryptortsbytestringinit
	#define GET_cryptortsbytestringinit(fl)  CAL_CMGETAPI( "cryptortsbytestringinit" ) 
	#define CAL_cryptortsbytestringinit  cryptortsbytestringinit
	#define CHK_cryptortsbytestringinit  TRUE
	#define EXP_cryptortsbytestringinit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptortsbytestringinit", (RTS_UINTPTR)cryptortsbytestringinit, 1, 0xC1B4C2F9, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptortsbytestringinit  PFCRYPTORTSBYTESTRINGINIT_IEC pfcryptortsbytestringinit;
	#define EXT_cryptortsbytestringinit  extern PFCRYPTORTSBYTESTRINGINIT_IEC pfcryptortsbytestringinit;
	#define GET_cryptortsbytestringinit(fl)  s_pfCMGetAPI2( "cryptortsbytestringinit", (RTS_VOID_FCTPTR *)&pfcryptortsbytestringinit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xC1B4C2F9, 0x03050B00)
	#define CAL_cryptortsbytestringinit  pfcryptortsbytestringinit
	#define CHK_cryptortsbytestringinit  (pfcryptortsbytestringinit != NULL)
	#define EXP_cryptortsbytestringinit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptortsbytestringinit", (RTS_UINTPTR)cryptortsbytestringinit, 1, 0xC1B4C2F9, 0x03050B00) 
#endif


/**
 * Sign the data using a specific message digest and the private key.
 * :return: Result of the operation.
 */
typedef struct tagcryptosignaturegenerate_struct
{
	RTS_IEC_HANDLE hAlgo;				/* VAR_INPUT */	/* Handle to the algorithm. */
	RtsByteString *pData;				/* VAR_INPUT */	/* Data to be signed. */
	RtsCryptoKey privateKey;			/* VAR_INPUT */	/* Key to be used signing. Has to be a private key. */
	RtsByteString *pSignature;			/* VAR_INPUT */	/* Calculated signature of the data. */
	RTS_IEC_RESULT CryptoSignatureGenerate;	/* VAR_OUTPUT */	
} cryptosignaturegenerate_struct;

void CDECL CDECL_EXT cryptosignaturegenerate(cryptosignaturegenerate_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOSIGNATUREGENERATE_IEC) (cryptosignaturegenerate_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOSIGNATUREGENERATE_NOTIMPLEMENTED)
	#define USE_cryptosignaturegenerate
	#define EXT_cryptosignaturegenerate
	#define GET_cryptosignaturegenerate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptosignaturegenerate(p0) 
	#define CHK_cryptosignaturegenerate  FALSE
	#define EXP_cryptosignaturegenerate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptosignaturegenerate
	#define EXT_cryptosignaturegenerate
	#define GET_cryptosignaturegenerate(fl)  CAL_CMGETAPI( "cryptosignaturegenerate" ) 
	#define CAL_cryptosignaturegenerate  cryptosignaturegenerate
	#define CHK_cryptosignaturegenerate  TRUE
	#define EXP_cryptosignaturegenerate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosignaturegenerate", (RTS_UINTPTR)cryptosignaturegenerate, 1, 0x0C2FBF80, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptosignaturegenerate
	#define EXT_cryptosignaturegenerate
	#define GET_cryptosignaturegenerate(fl)  CAL_CMGETAPI( "cryptosignaturegenerate" ) 
	#define CAL_cryptosignaturegenerate  cryptosignaturegenerate
	#define CHK_cryptosignaturegenerate  TRUE
	#define EXP_cryptosignaturegenerate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosignaturegenerate", (RTS_UINTPTR)cryptosignaturegenerate, 1, 0x0C2FBF80, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptosignaturegenerate
	#define EXT_CmpCryptocryptosignaturegenerate
	#define GET_CmpCryptocryptosignaturegenerate  ERR_OK
	#define CAL_CmpCryptocryptosignaturegenerate  cryptosignaturegenerate
	#define CHK_CmpCryptocryptosignaturegenerate  TRUE
	#define EXP_CmpCryptocryptosignaturegenerate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosignaturegenerate", (RTS_UINTPTR)cryptosignaturegenerate, 1, 0x0C2FBF80, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptosignaturegenerate
	#define EXT_cryptosignaturegenerate
	#define GET_cryptosignaturegenerate(fl)  CAL_CMGETAPI( "cryptosignaturegenerate" ) 
	#define CAL_cryptosignaturegenerate  cryptosignaturegenerate
	#define CHK_cryptosignaturegenerate  TRUE
	#define EXP_cryptosignaturegenerate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosignaturegenerate", (RTS_UINTPTR)cryptosignaturegenerate, 1, 0x0C2FBF80, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptosignaturegenerate  PFCRYPTOSIGNATUREGENERATE_IEC pfcryptosignaturegenerate;
	#define EXT_cryptosignaturegenerate  extern PFCRYPTOSIGNATUREGENERATE_IEC pfcryptosignaturegenerate;
	#define GET_cryptosignaturegenerate(fl)  s_pfCMGetAPI2( "cryptosignaturegenerate", (RTS_VOID_FCTPTR *)&pfcryptosignaturegenerate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x0C2FBF80, 0x03050B00)
	#define CAL_cryptosignaturegenerate  pfcryptosignaturegenerate
	#define CHK_cryptosignaturegenerate  (pfcryptosignaturegenerate != NULL)
	#define EXP_cryptosignaturegenerate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosignaturegenerate", (RTS_UINTPTR)cryptosignaturegenerate, 1, 0x0C2FBF80, 0x03050B00) 
#endif


/**
 * Verify a recieved signature
 * :return: unction returns ERR_OK if the signature is valid.
 */
typedef struct tagcryptosignatureverify_struct
{
	RTS_IEC_HANDLE hAlgo;				/* VAR_INPUT */	/* Handle to the algorithm. */
	RtsByteString *pData;				/* VAR_INPUT */	/* Data to verify. */
	RtsCryptoKey publicKey;				/* VAR_INPUT */	/* Public key of the sender. */
	RtsByteString *pSignature;			/* VAR_INPUT */	/* Received signature. */
	RTS_IEC_RESULT CryptoSignatureVerify;	/* VAR_OUTPUT */	
} cryptosignatureverify_struct;

void CDECL CDECL_EXT cryptosignatureverify(cryptosignatureverify_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOSIGNATUREVERIFY_IEC) (cryptosignatureverify_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOSIGNATUREVERIFY_NOTIMPLEMENTED)
	#define USE_cryptosignatureverify
	#define EXT_cryptosignatureverify
	#define GET_cryptosignatureverify(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptosignatureverify(p0) 
	#define CHK_cryptosignatureverify  FALSE
	#define EXP_cryptosignatureverify  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptosignatureverify
	#define EXT_cryptosignatureverify
	#define GET_cryptosignatureverify(fl)  CAL_CMGETAPI( "cryptosignatureverify" ) 
	#define CAL_cryptosignatureverify  cryptosignatureverify
	#define CHK_cryptosignatureverify  TRUE
	#define EXP_cryptosignatureverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosignatureverify", (RTS_UINTPTR)cryptosignatureverify, 1, 0x3F425BB9, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptosignatureverify
	#define EXT_cryptosignatureverify
	#define GET_cryptosignatureverify(fl)  CAL_CMGETAPI( "cryptosignatureverify" ) 
	#define CAL_cryptosignatureverify  cryptosignatureverify
	#define CHK_cryptosignatureverify  TRUE
	#define EXP_cryptosignatureverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosignatureverify", (RTS_UINTPTR)cryptosignatureverify, 1, 0x3F425BB9, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptosignatureverify
	#define EXT_CmpCryptocryptosignatureverify
	#define GET_CmpCryptocryptosignatureverify  ERR_OK
	#define CAL_CmpCryptocryptosignatureverify  cryptosignatureverify
	#define CHK_CmpCryptocryptosignatureverify  TRUE
	#define EXP_CmpCryptocryptosignatureverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosignatureverify", (RTS_UINTPTR)cryptosignatureverify, 1, 0x3F425BB9, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptosignatureverify
	#define EXT_cryptosignatureverify
	#define GET_cryptosignatureverify(fl)  CAL_CMGETAPI( "cryptosignatureverify" ) 
	#define CAL_cryptosignatureverify  cryptosignatureverify
	#define CHK_cryptosignatureverify  TRUE
	#define EXP_cryptosignatureverify  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosignatureverify", (RTS_UINTPTR)cryptosignatureverify, 1, 0x3F425BB9, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptosignatureverify  PFCRYPTOSIGNATUREVERIFY_IEC pfcryptosignatureverify;
	#define EXT_cryptosignatureverify  extern PFCRYPTOSIGNATUREVERIFY_IEC pfcryptosignatureverify;
	#define GET_cryptosignatureverify(fl)  s_pfCMGetAPI2( "cryptosignatureverify", (RTS_VOID_FCTPTR *)&pfcryptosignatureverify, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x3F425BB9, 0x03050B00)
	#define CAL_cryptosignatureverify  pfcryptosignatureverify
	#define CHK_cryptosignatureverify  (pfcryptosignatureverify != NULL)
	#define EXP_cryptosignatureverify   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosignatureverify", (RTS_UINTPTR)cryptosignatureverify, 1, 0x3F425BB9, 0x03050B00) 
#endif


/**
 * Perform a symmetric decryption using the algorithm handle.
 * :return: Result of the operation
 */
typedef struct tagcryptosymmetricdecrypt_struct
{
	RTS_IEC_HANDLE hAlgo;				/* VAR_INPUT */	/* Handle to the algorithm. */
	RtsByteString *pCipherText;			/* VAR_INPUT */	/* Data to be decrypted */
	RtsCryptoKey key;					/* VAR_INPUT */	/* Key to decrypt the data. Has to be a KeyType_Key key. */
	RtsByteString *pInitVector;			/* VAR_INPUT */	/* Init vector of the decrypt. */
	RTS_IEC_BOOL xEnablePadding;		/* VAR_INPUT */	/* Enables padding. If this is not enabled the ciphertext length has to match a multiple of the block length. */
	RtsByteString *pPlainText;			/* VAR_INPUT */	/* Decrypted data. */
	RTS_IEC_RESULT CryptoSymmetricDecrypt;	/* VAR_OUTPUT */	
} cryptosymmetricdecrypt_struct;

void CDECL CDECL_EXT cryptosymmetricdecrypt(cryptosymmetricdecrypt_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOSYMMETRICDECRYPT_IEC) (cryptosymmetricdecrypt_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOSYMMETRICDECRYPT_NOTIMPLEMENTED)
	#define USE_cryptosymmetricdecrypt
	#define EXT_cryptosymmetricdecrypt
	#define GET_cryptosymmetricdecrypt(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptosymmetricdecrypt(p0) 
	#define CHK_cryptosymmetricdecrypt  FALSE
	#define EXP_cryptosymmetricdecrypt  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptosymmetricdecrypt
	#define EXT_cryptosymmetricdecrypt
	#define GET_cryptosymmetricdecrypt(fl)  CAL_CMGETAPI( "cryptosymmetricdecrypt" ) 
	#define CAL_cryptosymmetricdecrypt  cryptosymmetricdecrypt
	#define CHK_cryptosymmetricdecrypt  TRUE
	#define EXP_cryptosymmetricdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosymmetricdecrypt", (RTS_UINTPTR)cryptosymmetricdecrypt, 1, 0xF3963545, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptosymmetricdecrypt
	#define EXT_cryptosymmetricdecrypt
	#define GET_cryptosymmetricdecrypt(fl)  CAL_CMGETAPI( "cryptosymmetricdecrypt" ) 
	#define CAL_cryptosymmetricdecrypt  cryptosymmetricdecrypt
	#define CHK_cryptosymmetricdecrypt  TRUE
	#define EXP_cryptosymmetricdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosymmetricdecrypt", (RTS_UINTPTR)cryptosymmetricdecrypt, 1, 0xF3963545, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptosymmetricdecrypt
	#define EXT_CmpCryptocryptosymmetricdecrypt
	#define GET_CmpCryptocryptosymmetricdecrypt  ERR_OK
	#define CAL_CmpCryptocryptosymmetricdecrypt  cryptosymmetricdecrypt
	#define CHK_CmpCryptocryptosymmetricdecrypt  TRUE
	#define EXP_CmpCryptocryptosymmetricdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosymmetricdecrypt", (RTS_UINTPTR)cryptosymmetricdecrypt, 1, 0xF3963545, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptosymmetricdecrypt
	#define EXT_cryptosymmetricdecrypt
	#define GET_cryptosymmetricdecrypt(fl)  CAL_CMGETAPI( "cryptosymmetricdecrypt" ) 
	#define CAL_cryptosymmetricdecrypt  cryptosymmetricdecrypt
	#define CHK_cryptosymmetricdecrypt  TRUE
	#define EXP_cryptosymmetricdecrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosymmetricdecrypt", (RTS_UINTPTR)cryptosymmetricdecrypt, 1, 0xF3963545, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptosymmetricdecrypt  PFCRYPTOSYMMETRICDECRYPT_IEC pfcryptosymmetricdecrypt;
	#define EXT_cryptosymmetricdecrypt  extern PFCRYPTOSYMMETRICDECRYPT_IEC pfcryptosymmetricdecrypt;
	#define GET_cryptosymmetricdecrypt(fl)  s_pfCMGetAPI2( "cryptosymmetricdecrypt", (RTS_VOID_FCTPTR *)&pfcryptosymmetricdecrypt, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xF3963545, 0x03050B00)
	#define CAL_cryptosymmetricdecrypt  pfcryptosymmetricdecrypt
	#define CHK_cryptosymmetricdecrypt  (pfcryptosymmetricdecrypt != NULL)
	#define EXP_cryptosymmetricdecrypt   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosymmetricdecrypt", (RTS_UINTPTR)cryptosymmetricdecrypt, 1, 0xF3963545, 0x03050B00) 
#endif


/**
 * Perform a symmetric encryption using the algorithm handle.
 * :return: Result of the operation
 */
typedef struct tagcryptosymmetricencrypt_struct
{
	RTS_IEC_HANDLE hAlgo;				/* VAR_INPUT */	/* Handle to the algorithm. */
	RtsByteString *pPlainText;			/* VAR_INPUT */	/* Data to be encrypted */
	RtsCryptoKey key;					/* VAR_INPUT */	/* Key to encrypt the data. Has to be a KeyType_Key key. */
	RtsByteString *pInitVector;			/* VAR_INPUT */	/* Init vector of the ecryption */
	RTS_IEC_BOOL xEnablePadding;		/* VAR_INPUT */	/* Enables padding. If this is not enabled the plaintext length has to match a multiple of the block length. */
	RtsByteString *pCipherText;			/* VAR_INPUT */	/* Encrypted data. */
	RTS_IEC_RESULT CryptoSymmetricEncrypt;	/* VAR_OUTPUT */	
} cryptosymmetricencrypt_struct;

void CDECL CDECL_EXT cryptosymmetricencrypt(cryptosymmetricencrypt_struct *p);
typedef void (CDECL CDECL_EXT* PFCRYPTOSYMMETRICENCRYPT_IEC) (cryptosymmetricencrypt_struct *p);
#if defined(CMPCRYPTO_NOTIMPLEMENTED) || defined(CRYPTOSYMMETRICENCRYPT_NOTIMPLEMENTED)
	#define USE_cryptosymmetricencrypt
	#define EXT_cryptosymmetricencrypt
	#define GET_cryptosymmetricencrypt(fl)  ERR_NOTIMPLEMENTED
	#define CAL_cryptosymmetricencrypt(p0) 
	#define CHK_cryptosymmetricencrypt  FALSE
	#define EXP_cryptosymmetricencrypt  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_cryptosymmetricencrypt
	#define EXT_cryptosymmetricencrypt
	#define GET_cryptosymmetricencrypt(fl)  CAL_CMGETAPI( "cryptosymmetricencrypt" ) 
	#define CAL_cryptosymmetricencrypt  cryptosymmetricencrypt
	#define CHK_cryptosymmetricencrypt  TRUE
	#define EXP_cryptosymmetricencrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosymmetricencrypt", (RTS_UINTPTR)cryptosymmetricencrypt, 1, 0x8D659084, 0x03050B00) 
#elif defined(MIXED_LINK) && !defined(CMPCRYPTO_EXTERNAL)
	#define USE_cryptosymmetricencrypt
	#define EXT_cryptosymmetricencrypt
	#define GET_cryptosymmetricencrypt(fl)  CAL_CMGETAPI( "cryptosymmetricencrypt" ) 
	#define CAL_cryptosymmetricencrypt  cryptosymmetricencrypt
	#define CHK_cryptosymmetricencrypt  TRUE
	#define EXP_cryptosymmetricencrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosymmetricencrypt", (RTS_UINTPTR)cryptosymmetricencrypt, 1, 0x8D659084, 0x03050B00) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpCryptocryptosymmetricencrypt
	#define EXT_CmpCryptocryptosymmetricencrypt
	#define GET_CmpCryptocryptosymmetricencrypt  ERR_OK
	#define CAL_CmpCryptocryptosymmetricencrypt  cryptosymmetricencrypt
	#define CHK_CmpCryptocryptosymmetricencrypt  TRUE
	#define EXP_CmpCryptocryptosymmetricencrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosymmetricencrypt", (RTS_UINTPTR)cryptosymmetricencrypt, 1, 0x8D659084, 0x03050B00) 
#elif defined(CPLUSPLUS)
	#define USE_cryptosymmetricencrypt
	#define EXT_cryptosymmetricencrypt
	#define GET_cryptosymmetricencrypt(fl)  CAL_CMGETAPI( "cryptosymmetricencrypt" ) 
	#define CAL_cryptosymmetricencrypt  cryptosymmetricencrypt
	#define CHK_cryptosymmetricencrypt  TRUE
	#define EXP_cryptosymmetricencrypt  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosymmetricencrypt", (RTS_UINTPTR)cryptosymmetricencrypt, 1, 0x8D659084, 0x03050B00) 
#else /* DYNAMIC_LINK */
	#define USE_cryptosymmetricencrypt  PFCRYPTOSYMMETRICENCRYPT_IEC pfcryptosymmetricencrypt;
	#define EXT_cryptosymmetricencrypt  extern PFCRYPTOSYMMETRICENCRYPT_IEC pfcryptosymmetricencrypt;
	#define GET_cryptosymmetricencrypt(fl)  s_pfCMGetAPI2( "cryptosymmetricencrypt", (RTS_VOID_FCTPTR *)&pfcryptosymmetricencrypt, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x8D659084, 0x03050B00)
	#define CAL_cryptosymmetricencrypt  pfcryptosymmetricencrypt
	#define CHK_cryptosymmetricencrypt  (pfcryptosymmetricencrypt != NULL)
	#define EXP_cryptosymmetricencrypt   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"cryptosymmetricencrypt", (RTS_UINTPTR)cryptosymmetricencrypt, 1, 0x8D659084, 0x03050B00) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/

/**
 * <description>Information of symmetric ciphers.</description>
 * <param name="ui32KeyLen">The length of the key id bytes.</param>
 * <param name="ui32BlockSize">Block size of the cipher in bytes.</param>
 * <param name="ui32InitVLen">Init vector size in bytes.</param>
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
 * <description>Information of a specific crypto algorithm.</description>
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
 * <result>Handle to the crypto algorithm</result>
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
 * GetNext function to iterate over algorithmes of a specific type.</description>
 * <param name="ui32CryptoType" type="IN">Type of the algorithm. See: Crypto Types</param>
 * <param name="pszName" type="OUT">Name of the algorithm. Can be NULL.</param>
 * <param name="i32MaxNameLen" type="OUT">Maximum length of the name buffer</param>
 * <param name="pui32CryptoID" type="OUT">ID of the algorithm.</param>
 * <param name="pResult" type="IN">Error code: Result of the operation. Can be NULL.</param>
 * <result>Handle to the crypto algorithm</result>
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
 * <result>Handle to the crypto algorithm</result>
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
 * <param name="pInitVector" type="IN">Init vector of the ecryption</param>
 * <param name="bEnablePadding" type="IN">Enables the padding of the algorithm. If padding is not enabled the plaintext length has to match a multiple of the cipher block length</param>
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
 * <param name="pInitVector" type="IN">Init vector of the decrypt.</param>
 * <param name="bEnablePadding" type="IN">Enables the padding of the algorithm. If padding is not enabled the plaintext length has to match a multiple of the cipher block length</param>
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
 * <param name="priavteKey" type="IN">Asymmetric key of intrest. Use X509CertGetPublicKey or X509CertGetPrivateKey to get the asymmetric key.</param>
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
 * <description>Generate a hashed message authentication code (HMAC) of the given data and key.</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pData" type="IN">Data to calculate the HMAC.</param>
 * <param name="key" type="IN">Key to be used for the HMAC calcuation</param>
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
 * <description>Verify a recieved hashed message authentication code (HMAC).</description>
 * <param name="hAlgo" type="IN">Handle to the algorithm.</param>
 * <param name="pData" type="IN">Data to calculate the HMAC.</param>
 * <param name="key" type="IN">Key to be used for the HMAC calcuation</param>
 * <param name="pSignature" type="IN">Recieved HMAC</param>
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
 * <description>Initialize a crypto key.</description>
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
 * <description>Clean up a crypto key.</description>
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
 * <description>Initialize a byte string.</description>
 * <param name="pByteString" type="IN">Pointer to the key to be initialized.</param>
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
 * <param name="pByteString" type="IN">Pointer to the key to be initialized.</param>
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
 	PFCRYPTOGENERATEHASH ICryptoGenerateHash;
 	PFCRYPTOHMACSIGN ICryptoHMACSign;
 	PFCRYPTOHMACVERIFY ICryptoHMACVerify;
 	PFCRYPTOSIGNATUREGENERATE ICryptoSignatureGenerate;
 	PFCRYPTOSIGNATUREVERIFY ICryptoSignatureVerify;
 	PFCRYPTOGENERATERANDOMNUMBER ICryptoGenerateRandomNumber;
 	PFCRYPTOKEYINIT ICryptoKeyInit;
 	PFCRYPTOKEYEXIT ICryptoKeyExit;
 	PFCRYPTORTSBYTESTRINGINIT ICryptoRtsByteStringInit;
 	PFCRYPTORTSBYTESTRINGINIT2 ICryptoRtsByteStringInit2;
 	PFCRYPTORTSBYTESTRINGEXIT ICryptoRtsByteStringExit;
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
		virtual RTS_RESULT CDECL ICryptoGenerateHash(RTS_HANDLE hAlgo, RtsByteString* pData, RtsByteString* pHash) =0;
		virtual RTS_RESULT CDECL ICryptoHMACSign(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey key, RtsByteString* pSignature) =0;
		virtual RTS_RESULT CDECL ICryptoHMACVerify(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey key, RtsByteString* pSignature) =0;
		virtual RTS_RESULT CDECL ICryptoSignatureGenerate(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey privateKey, RtsByteString* pSignature) =0;
		virtual RTS_RESULT CDECL ICryptoSignatureVerify(RTS_HANDLE hAlgo, RtsByteString* pData, RtsCryptoKey publicKey, RtsByteString* pSignature) =0;
		virtual RTS_RESULT CDECL ICryptoGenerateRandomNumber(RTS_UI32 ui32NumOfRandomBytes, RtsByteString* pRandom) =0;
		virtual RTS_RESULT CDECL ICryptoKeyInit(RtsCryptoKey* pKey, RtsCryptoKeyStorage key, RTSCRYPTOKEYTYPE keyType) =0;
		virtual RTS_RESULT CDECL ICryptoKeyExit(RtsCryptoKey* pKey) =0;
		virtual RTS_RESULT CDECL ICryptoRtsByteStringInit(RtsByteString* pByteString) =0;
		virtual RTS_RESULT CDECL ICryptoRtsByteStringInit2(RtsByteString* pByteString, RTS_UI32 ui32BufferSize, RTS_UI8* pBuffer) =0;
		virtual RTS_RESULT CDECL ICryptoRtsByteStringExit(RtsByteString* pByteString) =0;
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
