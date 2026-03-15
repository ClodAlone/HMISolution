 /**
 * <interfacename>CMUtils</interfacename>
 * <description>
 *	<p>Interface for utility functions of the component manager</p>
 *	<p>The utility functions can be used by all components, because they are always included
 *	in the component manager.</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMUTILSITF_H_
#define _CMUTILSITF_H_

#include "CmpStd.h"

 

 




#ifndef RTS_WTEXT
	#define	RTS_WTEXT(s)				__RTS_WTEXT(s)
    #if (__STDC_VERSION__ >= 201112L) /* C11 */
        /* UTF-16 character literal. Such literal has type char16_t. */
		#define	__RTS_WTEXT(s)				u ## s
    #else
        /* Wide character literal. Such literal has type wchar_t.
    
        The problem with the L prefix - why the u prefix should be used instead:
    
        The wchar_t type is an implementation-defined wide character type.
        In the Microsoft compiler, it represents a 16-bit wide character
        used to store Unicode encoded as UTF-16LE,
        the native character type on Windows operating systems.
        
        Unlike Windows UTF-16 2-byte wide chars,
        wchar_t on Linux and OS X is 4 bytes UTF-32 (gcc/g++ and XCode).
        On cygwin it is 2 (cygwin uses Windows APIs).
        */
		#define	__RTS_WTEXT(s)				L ## s
    #endif
#endif

#define RTS_WTEXT_EMPTY					RTS_WTEXT("\0")

#ifdef RTS_UNICODE
	#ifndef RTS_CWTEXT
		#define	RTS_CWTEXT(s)			__RTS_CWTEXT(s)
        #if (__STDC_VERSION__ >= 201112L) /* C11 */
            /* UTF-16 character literal. Such literal has type char16_t. */
            #define	__RTS_CWTEXT(s)			u ## s
        #else
             /* Wide character literal. Such literal has type wchar_t.
        
            The problem with the L prefix - why the u prefix should be used instead:
        
            The wchar_t type is an implementation-defined wide character type.
            In the Microsoft compiler, it represents a 16-bit wide character
            used to store Unicode encoded as UTF-16LE,
            the native character type on Windows operating systems.
            
            Unlike Windows UTF-16 2-byte wide chars,
            wchar_t on Linux and OS X is 4 bytes UTF-32 (gcc/g++ and XCode).
            On cygwin it is 2 (cygwin uses Windows APIs).
            */
           #define	__RTS_CWTEXT(s)			L ## s
        #endif
	#endif
#else
	#ifndef RTS_CWTEXT
		#define	RTS_CWTEXT(s) 			s
	#endif
#endif

/**
 * <category>Static defines</category>
 * <description>Static length of a string. Is used for RTS_STRING_CLASS.</description>
 */
#define RTS_STATIC_STRING_LEN					32

/**
 * <category>Copy mode</category>
 * <type>Int</type>
 * <description>Modes to copy strings. Copy byte packed stream to a word stream. HI byte is filled with 0.</description>
 */ 
#define CMUTL_CPY_COMM2BUFFER		1

/**
 * <category>Copy mode</category>
 * <type>Int</type>
 * <description>Modes to copy strings. Copy word stream to a byte packed stream. HI byte is removed.</description>
 */
#define CMUTL_CPY_BUFFER2COMM		2

/**
 * <category>Copy mode</category>
 * <type>Int</type>
 * <description>Modes to copy strings. Copy stream unchanged.</description>
 */
#define CMUTL_CPY_DEFAULT			3

/**
 * <category>Static defines</category>
 * <description>Space delimiter</description>
 */
#define CMUTL_SPACE_DELIMITER		" "

/**
 * <description> 
 * Definition of lock free ring buffer
 * </description>
 */
#define RTS_RINGBUFFER_STRUCT(size) \
struct { \
	RTS_UI32 uiReadPos; \
	RTS_UI32 uiWritePos; \
	RTS_UI8 ucBuf[size]; \
}
/**
 * <description> 
 * Ring buffer initialization
 * </description>
 */
#define RTS_RINGBUFFER_INIT(rbuf) (rbuf.uiReadPos = rbuf.uiWritePos = 0)

/**
 * <description> 
 * Get the length of written data in ring buffer, must be called before reading from buffer!
 * </description>
 */
#define RTS_RINGBUFFER_GET_FILL_SIZE(rbuf) ( (sizeof(rbuf.ucBuf) - rbuf.uiReadPos + rbuf.uiWritePos) % sizeof(rbuf.ucBuf) )

/**
 * <description> 
 * Get the length of available space in ring buffer, must be called before writing into buffer!
 * Always return one byte less than actual space size to avoid collision.
 * </description>
 */
#define RTS_RINGBUFFER_GET_FREE_SIZE(rbuf) ( (sizeof(rbuf.ucBuf) + rbuf.uiReadPos - rbuf.uiWritePos -1) % sizeof(rbuf.ucBuf) )

/**
 * <description> 
 * Read data from ring buffer. 
 * sizeToRead is the actual data length to be read from the buffer!
 * Comparison between filled data length in buffer and the length needs to be read must be done before calling this function!
 * Example:
 * sizeToRead = MIN(RTS_RINGBUFFER_GET_FILL_SIZE(rbuf), usSize);
 * RTS_RINGBUFFER_READ(dest, rbuf, sizeToRead);
 * </description>
 */
#define RTS_RINGBUFFER_READ(dest, rbuf, sizeToRead)\
	do{\
		RTS_UI32 sizeSectionA, sizeSectionB;\
		sizeSectionA = MIN(sizeof(rbuf.ucBuf) - rbuf.uiReadPos, sizeToRead);\
		sizeSectionB = sizeToRead - sizeSectionA;\
		memcpy(dest, &rbuf.ucBuf[rbuf.uiReadPos], sizeSectionA);\
		memcpy(dest + sizeSectionA, &rbuf.ucBuf[0], sizeSectionB);\
		rbuf.uiReadPos = (rbuf.uiReadPos + sizeToRead) % sizeof(rbuf.ucBuf);\
	}while(0)

/**
 * <description>
 * Read single byte of data from ring buffer.
 * </description>
 */
#define RTS_RINGBUFFER_READB(c, rbuf)\
	do{\
		c = rbuf.ucBuf[rbuf.uiReadPos];\
		rbuf.uiReadPos = (rbuf.uiReadPos + 1) % sizeof(rbuf.ucBuf);\
	}while(0)

/**
 * <description> 
 * Write data into ring buffer.
 * sizeToWrite is the actual data length to be written into the buffer!
 * Comparison between free buffer length and the length needs to be written must be done before calling this function!
 * Example:
 * sizeToWrite = MIN(RTS_RINGBUFFER_GET_FREE_SIZE(rbuf), usSize);
 * RTS_RINGBUFFER_WRITE(src, rbuf, sizeToWrite);
 * </description>
 */
#define RTS_RINGBUFFER_WRITE(src, rbuf, sizeToWrite)\
	do{\
		RTS_UI32 sizeSectionA, sizeSectionB;\
		sizeSectionA = MIN(sizeof(rbuf.ucBuf) - rbuf.uiWritePos, sizeToWrite);\
		sizeSectionB = sizeToWrite - sizeSectionA;\
		memcpy(&rbuf.ucBuf[rbuf.uiWritePos], src, sizeSectionA);\
		memcpy(&rbuf.ucBuf[0], src + sizeSectionA, sizeSectionB);\
		rbuf.uiWritePos = (rbuf.uiWritePos + sizeToWrite) % sizeof(rbuf.ucBuf);\
	}while(0)


/**
 * <description>
 * Write single byte of data into ring buffer.
 * </description>
 */
#define RTS_RINGBUFFER_WRITEB(c, rbuf)\
	do{\
		rbuf.ucBuf[rbuf.uiWritePos] = c;\
		rbuf.uiWritePos = (rbuf.uiWritePos + 1) % sizeof(rbuf.ucBuf);\
	}while(0)
		
#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description> 
 *	Called to skip white spaces out of a string.
 * </description>
 * <param name="psz" type="IN">Pointer to the string</param>
 * <result>returns the pointer to the next position in the string with no whitespace</result>
 */
char* CDECL CMUtlSkipWhiteSpace(char* psz);
typedef char* (CDECL * PFCMUTLSKIPWHITESPACE) (char* psz);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSKIPWHITESPACE_NOTIMPLEMENTED)
	#define USE_CMUtlSkipWhiteSpace
	#define EXT_CMUtlSkipWhiteSpace
	#define GET_CMUtlSkipWhiteSpace(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlSkipWhiteSpace(p0)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlSkipWhiteSpace  FALSE
	#define EXP_CMUtlSkipWhiteSpace  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlSkipWhiteSpace
	#define EXT_CMUtlSkipWhiteSpace
	#define GET_CMUtlSkipWhiteSpace(fl)  CAL_CMGETAPI( "CMUtlSkipWhiteSpace" ) 
	#define CAL_CMUtlSkipWhiteSpace  CMUtlSkipWhiteSpace
	#define CHK_CMUtlSkipWhiteSpace  TRUE
	#define EXP_CMUtlSkipWhiteSpace  CAL_CMEXPAPI( "CMUtlSkipWhiteSpace" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlSkipWhiteSpace
	#define EXT_CMUtlSkipWhiteSpace
	#define GET_CMUtlSkipWhiteSpace(fl)  CAL_CMGETAPI( "CMUtlSkipWhiteSpace" ) 
	#define CAL_CMUtlSkipWhiteSpace  CMUtlSkipWhiteSpace
	#define CHK_CMUtlSkipWhiteSpace  TRUE
	#define EXP_CMUtlSkipWhiteSpace  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSkipWhiteSpace", (RTS_UINTPTR)CMUtlSkipWhiteSpace, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlSkipWhiteSpace
	#define EXT_CMUtilsCMUtlSkipWhiteSpace
	#define GET_CMUtilsCMUtlSkipWhiteSpace  ERR_OK
	#define CAL_CMUtilsCMUtlSkipWhiteSpace pICMUtils->ICMUtlSkipWhiteSpace
	#define CHK_CMUtilsCMUtlSkipWhiteSpace (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlSkipWhiteSpace  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlSkipWhiteSpace
	#define EXT_CMUtlSkipWhiteSpace
	#define GET_CMUtlSkipWhiteSpace(fl)  CAL_CMGETAPI( "CMUtlSkipWhiteSpace" ) 
	#define CAL_CMUtlSkipWhiteSpace pICMUtils->ICMUtlSkipWhiteSpace
	#define CHK_CMUtlSkipWhiteSpace (pICMUtils != NULL)
	#define EXP_CMUtlSkipWhiteSpace  CAL_CMEXPAPI( "CMUtlSkipWhiteSpace" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlSkipWhiteSpace  PFCMUTLSKIPWHITESPACE pfCMUtlSkipWhiteSpace;
	#define EXT_CMUtlSkipWhiteSpace  extern PFCMUTLSKIPWHITESPACE pfCMUtlSkipWhiteSpace;
	#define GET_CMUtlSkipWhiteSpace(fl)  s_pfCMGetAPI2( "CMUtlSkipWhiteSpace", (RTS_VOID_FCTPTR *)&pfCMUtlSkipWhiteSpace, (fl), 0, 0)
	#define CAL_CMUtlSkipWhiteSpace  pfCMUtlSkipWhiteSpace
	#define CHK_CMUtlSkipWhiteSpace  (pfCMUtlSkipWhiteSpace != NULL)
	#define EXP_CMUtlSkipWhiteSpace  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSkipWhiteSpace", (RTS_UINTPTR)CMUtlSkipWhiteSpace, 0, 0) 
#endif




/**
 * <description> 
 * Maximum length of the long binary string 
 * </description>
 */
#define MAX_LONG_BIN_STRING_LEN		(sizeof(RTS_I32) * 8 + 1)

/**
 * <description> 
 * Maximum length of the long decimal string 
 * </description>
 */
#define MAX_LONG_DEC_STRING_LEN		11

/**
 * <description> 
 * Maximum length of the long hexadecimal string
 * </description>
 */
#define MAX_LONG_HEX_STRING_LEN		11


#define RTS_JAN1970TOJAN1601_100NS                              RTS_UI64_CONST(116444736000000000)
/**
 * <description> 
 * Convert timestmap [s] based on 1.1.1970T00:00 into [100ns] since 1.1.1601
 * </description>
 */
#define RTS_CONVERT_TIME_TO_100NS_SINCE_1601(timestamp)         (((RTS_UI64)timestamp * 10000000) + RTS_JAN1970TOJAN1601_100NS)

/**
 * <description> 
 * Convert timestamp [100ns] based on 1.1.1601 into [s] since 1.1.1970T00:00
 * </description>
 */
#define RTS_CONVERT_100NS_SINCE_1601_TO_TIME(timestamp100ns)    (((RTS_UI64)timestamp100ns - RTS_JAN1970TOJAN1601_100NS) / 10000000)


/**
 * <description> 
 * <p>Converts a long value to a string. The representation can be specified by the iBase value:</p>
 * <p>2: binary (e.g. "10101010101110111100110011011101"). String length must be at least MAX_LONG_BIN_STRING_LEN bytes!</p>
 * <p>10: decimal (e.g. "2864434397"). String length must be at least MAX_LONG_DEC_STRING_LEN bytes!</p>
 * <p>16: hexadecimal (e.g. "0xAABBCCDD"). String length must be at least MAX_LONG_HEX_STRING_LEN bytes!</p>
 * </description>
 * <param name="lValue" type="IN">Long value to convert</param>
 * <param name="szString" type="OUT">Pointer to the string to get integer value as string</param>
 * <param name="iBase" type="IN">Base for the conversion: See description</param>
 * <result>returns the pointer to the next position in the string with no whitespace</result>
 */
char* CDECL CMUtlLtoa(RTS_I32 lValue, char *pszString, int nMaxLen, int iBase, RTS_RESULT *pResult);
typedef char* (CDECL * PFCMUTLLTOA) (RTS_I32 lValue, char *pszString, int nMaxLen, int iBase, RTS_RESULT *pResult);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLLTOA_NOTIMPLEMENTED)
	#define USE_CMUtlLtoa
	#define EXT_CMUtlLtoa
	#define GET_CMUtlLtoa(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlLtoa(p0,p1,p2,p3,p4)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlLtoa  FALSE
	#define EXP_CMUtlLtoa  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlLtoa
	#define EXT_CMUtlLtoa
	#define GET_CMUtlLtoa(fl)  CAL_CMGETAPI( "CMUtlLtoa" ) 
	#define CAL_CMUtlLtoa  CMUtlLtoa
	#define CHK_CMUtlLtoa  TRUE
	#define EXP_CMUtlLtoa  CAL_CMEXPAPI( "CMUtlLtoa" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlLtoa
	#define EXT_CMUtlLtoa
	#define GET_CMUtlLtoa(fl)  CAL_CMGETAPI( "CMUtlLtoa" ) 
	#define CAL_CMUtlLtoa  CMUtlLtoa
	#define CHK_CMUtlLtoa  TRUE
	#define EXP_CMUtlLtoa  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlLtoa", (RTS_UINTPTR)CMUtlLtoa, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlLtoa
	#define EXT_CMUtilsCMUtlLtoa
	#define GET_CMUtilsCMUtlLtoa  ERR_OK
	#define CAL_CMUtilsCMUtlLtoa pICMUtils->ICMUtlLtoa
	#define CHK_CMUtilsCMUtlLtoa (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlLtoa  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlLtoa
	#define EXT_CMUtlLtoa
	#define GET_CMUtlLtoa(fl)  CAL_CMGETAPI( "CMUtlLtoa" ) 
	#define CAL_CMUtlLtoa pICMUtils->ICMUtlLtoa
	#define CHK_CMUtlLtoa (pICMUtils != NULL)
	#define EXP_CMUtlLtoa  CAL_CMEXPAPI( "CMUtlLtoa" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlLtoa  PFCMUTLLTOA pfCMUtlLtoa;
	#define EXT_CMUtlLtoa  extern PFCMUTLLTOA pfCMUtlLtoa;
	#define GET_CMUtlLtoa(fl)  s_pfCMGetAPI2( "CMUtlLtoa", (RTS_VOID_FCTPTR *)&pfCMUtlLtoa, (fl), 0, 0)
	#define CAL_CMUtlLtoa  pfCMUtlLtoa
	#define CHK_CMUtlLtoa  (pfCMUtlLtoa != NULL)
	#define EXP_CMUtlLtoa  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlLtoa", (RTS_UINTPTR)CMUtlLtoa, 0, 0) 
#endif




/**
 * <description> 
 * Maximum length of the unsigned long binary string   
 * </description>
 */
#define MAX_ULONG_BIN_STRING_LEN	(sizeof(RTS_UI32) * 8 + 1)

/**
 * <description> 
 * Maximum length of the unsigned long decimal string  
 * </description>
 */
#define MAX_ULONG_DEC_STRING_LEN	11

/**
 * <description> 
 * Maximum length of the long unsigned hexadecimal string  
 * </description>
 */
#define MAX_ULONG_HEX_STRING_LEN	11

/**
 * <description> 
 *	Converts a version as string in an integer. The string must have the format "x.x.x.x".
 * </description>
 * <param name="pszVersion" type="IN">Pointer to version as string</param>
 * <param name="pui32Version" type="OUT">Pointer to get the version as integer</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlStringToVersion(char *pszVersion, RTS_UI32 *pui32Version);
typedef RTS_RESULT (CDECL * PFCMUTLSTRINGTOVERSION) (char *pszVersion, RTS_UI32 *pui32Version);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRINGTOVERSION_NOTIMPLEMENTED)
	#define USE_CMUtlStringToVersion
	#define EXT_CMUtlStringToVersion
	#define GET_CMUtlStringToVersion(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStringToVersion(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStringToVersion  FALSE
	#define EXP_CMUtlStringToVersion  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStringToVersion
	#define EXT_CMUtlStringToVersion
	#define GET_CMUtlStringToVersion(fl)  CAL_CMGETAPI( "CMUtlStringToVersion" ) 
	#define CAL_CMUtlStringToVersion  CMUtlStringToVersion
	#define CHK_CMUtlStringToVersion  TRUE
	#define EXP_CMUtlStringToVersion  CAL_CMEXPAPI( "CMUtlStringToVersion" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStringToVersion
	#define EXT_CMUtlStringToVersion
	#define GET_CMUtlStringToVersion(fl)  CAL_CMGETAPI( "CMUtlStringToVersion" ) 
	#define CAL_CMUtlStringToVersion  CMUtlStringToVersion
	#define CHK_CMUtlStringToVersion  TRUE
	#define EXP_CMUtlStringToVersion  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStringToVersion", (RTS_UINTPTR)CMUtlStringToVersion, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStringToVersion
	#define EXT_CMUtilsCMUtlStringToVersion
	#define GET_CMUtilsCMUtlStringToVersion  ERR_OK
	#define CAL_CMUtilsCMUtlStringToVersion pICMUtils->ICMUtlStringToVersion
	#define CHK_CMUtilsCMUtlStringToVersion (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStringToVersion  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStringToVersion
	#define EXT_CMUtlStringToVersion
	#define GET_CMUtlStringToVersion(fl)  CAL_CMGETAPI( "CMUtlStringToVersion" ) 
	#define CAL_CMUtlStringToVersion pICMUtils->ICMUtlStringToVersion
	#define CHK_CMUtlStringToVersion (pICMUtils != NULL)
	#define EXP_CMUtlStringToVersion  CAL_CMEXPAPI( "CMUtlStringToVersion" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStringToVersion  PFCMUTLSTRINGTOVERSION pfCMUtlStringToVersion;
	#define EXT_CMUtlStringToVersion  extern PFCMUTLSTRINGTOVERSION pfCMUtlStringToVersion;
	#define GET_CMUtlStringToVersion(fl)  s_pfCMGetAPI2( "CMUtlStringToVersion", (RTS_VOID_FCTPTR *)&pfCMUtlStringToVersion, (fl), 0, 0)
	#define CAL_CMUtlStringToVersion  pfCMUtlStringToVersion
	#define CHK_CMUtlStringToVersion  (pfCMUtlStringToVersion != NULL)
	#define EXP_CMUtlStringToVersion  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStringToVersion", (RTS_UINTPTR)CMUtlStringToVersion, 0, 0) 
#endif




/**
 * <description> 
 *	Converts a version into a string. The string has the format "x.x.x.x".
 * </description>
 * <param name="ui32Version" type="IN">Version as integer</param>
 * <param name="pszVersion" type="OUT">Pointer to version as string</param>
 * <param name="nMaxLen" type="IN">Max lenght of pszVersion</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlVersionToString(RTS_UI32 ui32Version, char *pszVersion, RTS_SIZE nMaxLen);
typedef RTS_RESULT (CDECL * PFCMUTLVERSIONTOSTRING) (RTS_UI32 ui32Version, char *pszVersion, RTS_SIZE nMaxLen);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLVERSIONTOSTRING_NOTIMPLEMENTED)
	#define USE_CMUtlVersionToString
	#define EXT_CMUtlVersionToString
	#define GET_CMUtlVersionToString(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlVersionToString(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlVersionToString  FALSE
	#define EXP_CMUtlVersionToString  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlVersionToString
	#define EXT_CMUtlVersionToString
	#define GET_CMUtlVersionToString(fl)  CAL_CMGETAPI( "CMUtlVersionToString" ) 
	#define CAL_CMUtlVersionToString  CMUtlVersionToString
	#define CHK_CMUtlVersionToString  TRUE
	#define EXP_CMUtlVersionToString  CAL_CMEXPAPI( "CMUtlVersionToString" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlVersionToString
	#define EXT_CMUtlVersionToString
	#define GET_CMUtlVersionToString(fl)  CAL_CMGETAPI( "CMUtlVersionToString" ) 
	#define CAL_CMUtlVersionToString  CMUtlVersionToString
	#define CHK_CMUtlVersionToString  TRUE
	#define EXP_CMUtlVersionToString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlVersionToString", (RTS_UINTPTR)CMUtlVersionToString, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlVersionToString
	#define EXT_CMUtilsCMUtlVersionToString
	#define GET_CMUtilsCMUtlVersionToString  ERR_OK
	#define CAL_CMUtilsCMUtlVersionToString pICMUtils->ICMUtlVersionToString
	#define CHK_CMUtilsCMUtlVersionToString (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlVersionToString  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlVersionToString
	#define EXT_CMUtlVersionToString
	#define GET_CMUtlVersionToString(fl)  CAL_CMGETAPI( "CMUtlVersionToString" ) 
	#define CAL_CMUtlVersionToString pICMUtils->ICMUtlVersionToString
	#define CHK_CMUtlVersionToString (pICMUtils != NULL)
	#define EXP_CMUtlVersionToString  CAL_CMEXPAPI( "CMUtlVersionToString" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlVersionToString  PFCMUTLVERSIONTOSTRING pfCMUtlVersionToString;
	#define EXT_CMUtlVersionToString  extern PFCMUTLVERSIONTOSTRING pfCMUtlVersionToString;
	#define GET_CMUtlVersionToString(fl)  s_pfCMGetAPI2( "CMUtlVersionToString", (RTS_VOID_FCTPTR *)&pfCMUtlVersionToString, (fl), 0, 0)
	#define CAL_CMUtlVersionToString  pfCMUtlVersionToString
	#define CHK_CMUtlVersionToString  (pfCMUtlVersionToString != NULL)
	#define EXP_CMUtlVersionToString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlVersionToString", (RTS_UINTPTR)CMUtlVersionToString, 0, 0) 
#endif




/**
 * <description> 
 *	Converts a byte array into a string. The NUL-terminated string will contain only hexadecimal numerals (0..9, a..f) without a prefix or a suffix.
 * </description>
 * <param name="pData" type="IN">Byte array to convert</param>
 * <param name="DataSize" type="IN">Size of the byte array</param>
 * <param name="pszDestination" type="OUT">Pointer to the string to store the converted data.</param>
 * <param name="pDestinationSize" type="INOUT">Input: Size of the destination string buffer, output: strlen(pszDestination) + 1</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlByteArrayToString(const RTS_UI8 *pData, RTS_SIZE DataSize, char *pszDestination, RTS_SIZE *pDestinationSize);
typedef RTS_RESULT (CDECL * PFCMUTLBYTEARRAYTOSTRING) (const RTS_UI8 *pData, RTS_SIZE DataSize, char *pszDestination, RTS_SIZE *pDestinationSize);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLBYTEARRAYTOSTRING_NOTIMPLEMENTED)
	#define USE_CMUtlByteArrayToString
	#define EXT_CMUtlByteArrayToString
	#define GET_CMUtlByteArrayToString(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlByteArrayToString(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlByteArrayToString  FALSE
	#define EXP_CMUtlByteArrayToString  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlByteArrayToString
	#define EXT_CMUtlByteArrayToString
	#define GET_CMUtlByteArrayToString(fl)  CAL_CMGETAPI( "CMUtlByteArrayToString" ) 
	#define CAL_CMUtlByteArrayToString  CMUtlByteArrayToString
	#define CHK_CMUtlByteArrayToString  TRUE
	#define EXP_CMUtlByteArrayToString  CAL_CMEXPAPI( "CMUtlByteArrayToString" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlByteArrayToString
	#define EXT_CMUtlByteArrayToString
	#define GET_CMUtlByteArrayToString(fl)  CAL_CMGETAPI( "CMUtlByteArrayToString" ) 
	#define CAL_CMUtlByteArrayToString  CMUtlByteArrayToString
	#define CHK_CMUtlByteArrayToString  TRUE
	#define EXP_CMUtlByteArrayToString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlByteArrayToString", (RTS_UINTPTR)CMUtlByteArrayToString, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlByteArrayToString
	#define EXT_CMUtilsCMUtlByteArrayToString
	#define GET_CMUtilsCMUtlByteArrayToString  ERR_OK
	#define CAL_CMUtilsCMUtlByteArrayToString pICMUtils->ICMUtlByteArrayToString
	#define CHK_CMUtilsCMUtlByteArrayToString (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlByteArrayToString  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlByteArrayToString
	#define EXT_CMUtlByteArrayToString
	#define GET_CMUtlByteArrayToString(fl)  CAL_CMGETAPI( "CMUtlByteArrayToString" ) 
	#define CAL_CMUtlByteArrayToString pICMUtils->ICMUtlByteArrayToString
	#define CHK_CMUtlByteArrayToString (pICMUtils != NULL)
	#define EXP_CMUtlByteArrayToString  CAL_CMEXPAPI( "CMUtlByteArrayToString" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlByteArrayToString  PFCMUTLBYTEARRAYTOSTRING pfCMUtlByteArrayToString;
	#define EXT_CMUtlByteArrayToString  extern PFCMUTLBYTEARRAYTOSTRING pfCMUtlByteArrayToString;
	#define GET_CMUtlByteArrayToString(fl)  s_pfCMGetAPI2( "CMUtlByteArrayToString", (RTS_VOID_FCTPTR *)&pfCMUtlByteArrayToString, (fl), 0, 0)
	#define CAL_CMUtlByteArrayToString  pfCMUtlByteArrayToString
	#define CHK_CMUtlByteArrayToString  (pfCMUtlByteArrayToString != NULL)
	#define EXP_CMUtlByteArrayToString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlByteArrayToString", (RTS_UINTPTR)CMUtlByteArrayToString, 0, 0) 
#endif




/**
 * <description> 
 *	Converts a NUL-terminated string into a byte array. The string must only contain hexadecimal numerals (0..9, a..f, A..F) without a prefix or a suffix.
 * </description>
 * <param name="pszData" type="IN">String to convert</param>
 * <param name="pDestination" type="OUT">Pointer to the buffer to store the converted string</param>
 * <param name="pDestinationSize" type="INOUT">Input: Size of the destination buffer, output: size of the stored byte array</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlStringToByteArray(const char *pszData, RTS_UI8 *pDestination, RTS_SIZE *pDestinationSize);
typedef RTS_RESULT (CDECL * PFCMUTLSTRINGTOBYTEARRAY) (const char *pszData, RTS_UI8 *pDestination, RTS_SIZE *pDestinationSize);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRINGTOBYTEARRAY_NOTIMPLEMENTED)
	#define USE_CMUtlStringToByteArray
	#define EXT_CMUtlStringToByteArray
	#define GET_CMUtlStringToByteArray(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStringToByteArray(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStringToByteArray  FALSE
	#define EXP_CMUtlStringToByteArray  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStringToByteArray
	#define EXT_CMUtlStringToByteArray
	#define GET_CMUtlStringToByteArray(fl)  CAL_CMGETAPI( "CMUtlStringToByteArray" ) 
	#define CAL_CMUtlStringToByteArray  CMUtlStringToByteArray
	#define CHK_CMUtlStringToByteArray  TRUE
	#define EXP_CMUtlStringToByteArray  CAL_CMEXPAPI( "CMUtlStringToByteArray" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStringToByteArray
	#define EXT_CMUtlStringToByteArray
	#define GET_CMUtlStringToByteArray(fl)  CAL_CMGETAPI( "CMUtlStringToByteArray" ) 
	#define CAL_CMUtlStringToByteArray  CMUtlStringToByteArray
	#define CHK_CMUtlStringToByteArray  TRUE
	#define EXP_CMUtlStringToByteArray  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStringToByteArray", (RTS_UINTPTR)CMUtlStringToByteArray, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStringToByteArray
	#define EXT_CMUtilsCMUtlStringToByteArray
	#define GET_CMUtilsCMUtlStringToByteArray  ERR_OK
	#define CAL_CMUtilsCMUtlStringToByteArray pICMUtils->ICMUtlStringToByteArray
	#define CHK_CMUtilsCMUtlStringToByteArray (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStringToByteArray  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStringToByteArray
	#define EXT_CMUtlStringToByteArray
	#define GET_CMUtlStringToByteArray(fl)  CAL_CMGETAPI( "CMUtlStringToByteArray" ) 
	#define CAL_CMUtlStringToByteArray pICMUtils->ICMUtlStringToByteArray
	#define CHK_CMUtlStringToByteArray (pICMUtils != NULL)
	#define EXP_CMUtlStringToByteArray  CAL_CMEXPAPI( "CMUtlStringToByteArray" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStringToByteArray  PFCMUTLSTRINGTOBYTEARRAY pfCMUtlStringToByteArray;
	#define EXT_CMUtlStringToByteArray  extern PFCMUTLSTRINGTOBYTEARRAY pfCMUtlStringToByteArray;
	#define GET_CMUtlStringToByteArray(fl)  s_pfCMGetAPI2( "CMUtlStringToByteArray", (RTS_VOID_FCTPTR *)&pfCMUtlStringToByteArray, (fl), 0, 0)
	#define CAL_CMUtlStringToByteArray  pfCMUtlStringToByteArray
	#define CHK_CMUtlStringToByteArray  (pfCMUtlStringToByteArray != NULL)
	#define EXP_CMUtlStringToByteArray  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStringToByteArray", (RTS_UINTPTR)CMUtlStringToByteArray, 0, 0) 
#endif




/**
 * <description> 
 * <p>Converts an unsigned long value to a string. The representation can be specified by the iBase value:</p>
 * <p>2: binary (e.g. "10101010101110111100110011011101"). String length must be at least MAX_ULONG_BIN_STRING_LEN bytes!</p>
 * <p>10: decimal (e.g. "2864434397"). String length must be at least MAX_ULONG_DEC_STRING_LEN bytes!</p>
 * <p>16: hexadecimal (e.g. "0xAABBCCDD"). String length must be at least MAX_ULONG_HEX_STRING_LEN bytes!</p>	
 * </description>
 * <param name="ulValue" type="IN">Unsigned long value to convert</param>
 * <param name="szString" type="OUT">Pointer to the string to get integer value as string</param>
 * <param name="iBase" type="IN">Base for the conversion: See description</param>
 * <result>returns the pointer to the next position in the string with no whitespace</result>
 */
char* CDECL CMUtlUltoa(RTS_UI32 ulValue, char *pszString, int nMaxLen, int iBase, RTS_RESULT *pResult);
typedef char* (CDECL * PFCMUTLULTOA) (RTS_UI32 ulValue, char *pszString, int nMaxLen, int iBase, RTS_RESULT *pResult);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLULTOA_NOTIMPLEMENTED)
	#define USE_CMUtlUltoa
	#define EXT_CMUtlUltoa
	#define GET_CMUtlUltoa(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlUltoa(p0,p1,p2,p3,p4)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlUltoa  FALSE
	#define EXP_CMUtlUltoa  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlUltoa
	#define EXT_CMUtlUltoa
	#define GET_CMUtlUltoa(fl)  CAL_CMGETAPI( "CMUtlUltoa" ) 
	#define CAL_CMUtlUltoa  CMUtlUltoa
	#define CHK_CMUtlUltoa  TRUE
	#define EXP_CMUtlUltoa  CAL_CMEXPAPI( "CMUtlUltoa" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlUltoa
	#define EXT_CMUtlUltoa
	#define GET_CMUtlUltoa(fl)  CAL_CMGETAPI( "CMUtlUltoa" ) 
	#define CAL_CMUtlUltoa  CMUtlUltoa
	#define CHK_CMUtlUltoa  TRUE
	#define EXP_CMUtlUltoa  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlUltoa", (RTS_UINTPTR)CMUtlUltoa, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlUltoa
	#define EXT_CMUtilsCMUtlUltoa
	#define GET_CMUtilsCMUtlUltoa  ERR_OK
	#define CAL_CMUtilsCMUtlUltoa pICMUtils->ICMUtlUltoa
	#define CHK_CMUtilsCMUtlUltoa (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlUltoa  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlUltoa
	#define EXT_CMUtlUltoa
	#define GET_CMUtlUltoa(fl)  CAL_CMGETAPI( "CMUtlUltoa" ) 
	#define CAL_CMUtlUltoa pICMUtils->ICMUtlUltoa
	#define CHK_CMUtlUltoa (pICMUtils != NULL)
	#define EXP_CMUtlUltoa  CAL_CMEXPAPI( "CMUtlUltoa" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlUltoa  PFCMUTLULTOA pfCMUtlUltoa;
	#define EXT_CMUtlUltoa  extern PFCMUTLULTOA pfCMUtlUltoa;
	#define GET_CMUtlUltoa(fl)  s_pfCMGetAPI2( "CMUtlUltoa", (RTS_VOID_FCTPTR *)&pfCMUtlUltoa, (fl), 0, 0)
	#define CAL_CMUtlUltoa  pfCMUtlUltoa
	#define CHK_CMUtlUltoa  (pfCMUtlUltoa != NULL)
	#define EXP_CMUtlUltoa  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlUltoa", (RTS_UINTPTR)CMUtlUltoa, 0, 0) 
#endif




/**
 * <description> 
 * <p>Reverses content of a string in place</p>
 * </description>
 * <param name="psz" type="INOUT">Pointer to a null-terminated string</param>
 * <result>returns the pointer to the next the same string</result>
 */
char* CDECL CMUtlStrrev(char *psz);
typedef char* (CDECL * PFCMUTLSTRREV) (char *psz);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRREV_NOTIMPLEMENTED)
	#define USE_CMUtlStrrev
	#define EXT_CMUtlStrrev
	#define GET_CMUtlStrrev(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrrev(p0)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrrev  FALSE
	#define EXP_CMUtlStrrev  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrrev
	#define EXT_CMUtlStrrev
	#define GET_CMUtlStrrev(fl)  CAL_CMGETAPI( "CMUtlStrrev" ) 
	#define CAL_CMUtlStrrev  CMUtlStrrev
	#define CHK_CMUtlStrrev  TRUE
	#define EXP_CMUtlStrrev  CAL_CMEXPAPI( "CMUtlStrrev" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrrev
	#define EXT_CMUtlStrrev
	#define GET_CMUtlStrrev(fl)  CAL_CMGETAPI( "CMUtlStrrev" ) 
	#define CAL_CMUtlStrrev  CMUtlStrrev
	#define CHK_CMUtlStrrev  TRUE
	#define EXP_CMUtlStrrev  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrrev", (RTS_UINTPTR)CMUtlStrrev, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrrev
	#define EXT_CMUtilsCMUtlStrrev
	#define GET_CMUtilsCMUtlStrrev  ERR_OK
	#define CAL_CMUtilsCMUtlStrrev pICMUtils->ICMUtlStrrev
	#define CHK_CMUtilsCMUtlStrrev (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrrev  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrrev
	#define EXT_CMUtlStrrev
	#define GET_CMUtlStrrev(fl)  CAL_CMGETAPI( "CMUtlStrrev" ) 
	#define CAL_CMUtlStrrev pICMUtils->ICMUtlStrrev
	#define CHK_CMUtlStrrev (pICMUtils != NULL)
	#define EXP_CMUtlStrrev  CAL_CMEXPAPI( "CMUtlStrrev" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrrev  PFCMUTLSTRREV pfCMUtlStrrev;
	#define EXT_CMUtlStrrev  extern PFCMUTLSTRREV pfCMUtlStrrev;
	#define GET_CMUtlStrrev(fl)  s_pfCMGetAPI2( "CMUtlStrrev", (RTS_VOID_FCTPTR *)&pfCMUtlStrrev, (fl), 0, 0)
	#define CAL_CMUtlStrrev  pfCMUtlStrrev
	#define CHK_CMUtlStrrev  (pfCMUtlStrrev != NULL)
	#define EXP_CMUtlStrrev  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrrev", (RTS_UINTPTR)CMUtlStrrev, 0, 0) 
#endif





/**
 * <description> 
 *  Scan a string for the last occurrence of a character
 * </description>
 * <param name="pszString" type="IN">Pointer to scan</param>
 * <param name="pszBegin" type="IN">Pointer of a position into pszString to start the scan</param>
 * <result>Pointer to the first occurrence in string, or NULL if not found</result>
 */
char* CDECL CMUtlStrRChr(char *pszString, char *pszBegin, char cFind);
typedef char* (CDECL * PFCMUTLSTRRCHR) (char *pszString, char *pszBegin, char cFind);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRRCHR_NOTIMPLEMENTED)
	#define USE_CMUtlStrRChr
	#define EXT_CMUtlStrRChr
	#define GET_CMUtlStrRChr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrRChr(p0,p1,p2)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrRChr  FALSE
	#define EXP_CMUtlStrRChr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrRChr
	#define EXT_CMUtlStrRChr
	#define GET_CMUtlStrRChr(fl)  CAL_CMGETAPI( "CMUtlStrRChr" ) 
	#define CAL_CMUtlStrRChr  CMUtlStrRChr
	#define CHK_CMUtlStrRChr  TRUE
	#define EXP_CMUtlStrRChr  CAL_CMEXPAPI( "CMUtlStrRChr" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrRChr
	#define EXT_CMUtlStrRChr
	#define GET_CMUtlStrRChr(fl)  CAL_CMGETAPI( "CMUtlStrRChr" ) 
	#define CAL_CMUtlStrRChr  CMUtlStrRChr
	#define CHK_CMUtlStrRChr  TRUE
	#define EXP_CMUtlStrRChr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrRChr", (RTS_UINTPTR)CMUtlStrRChr, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrRChr
	#define EXT_CMUtilsCMUtlStrRChr
	#define GET_CMUtilsCMUtlStrRChr  ERR_OK
	#define CAL_CMUtilsCMUtlStrRChr pICMUtils->ICMUtlStrRChr
	#define CHK_CMUtilsCMUtlStrRChr (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrRChr  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrRChr
	#define EXT_CMUtlStrRChr
	#define GET_CMUtlStrRChr(fl)  CAL_CMGETAPI( "CMUtlStrRChr" ) 
	#define CAL_CMUtlStrRChr pICMUtils->ICMUtlStrRChr
	#define CHK_CMUtlStrRChr (pICMUtils != NULL)
	#define EXP_CMUtlStrRChr  CAL_CMEXPAPI( "CMUtlStrRChr" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrRChr  PFCMUTLSTRRCHR pfCMUtlStrRChr;
	#define EXT_CMUtlStrRChr  extern PFCMUTLSTRRCHR pfCMUtlStrRChr;
	#define GET_CMUtlStrRChr(fl)  s_pfCMGetAPI2( "CMUtlStrRChr", (RTS_VOID_FCTPTR *)&pfCMUtlStrRChr, (fl), 0, 0)
	#define CAL_CMUtlStrRChr  pfCMUtlStrRChr
	#define CHK_CMUtlStrRChr  (pfCMUtlStrRChr != NULL)
	#define EXP_CMUtlStrRChr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrRChr", (RTS_UINTPTR)CMUtlStrRChr, 0, 0) 
#endif




/**
 * <description> 
 * <p>Case insensitive string compare.</p>
 * </description>
 * <param name="pszString1" type="IN" range="[NULL,VALID_POINTER_TO_STRING1,VALID_POINTER_TO_STRING2]">Pointer to first string to compare</param>
 * <param name="pszString2" type="IN" range="[NULL,VALID_POINTER_TO_STRING1]">Pointer to second string to compare</param>
 * <result>0 if strings are equal, -1 or 1 if there is a difference between them</result>
 */
int CDECL CMUtlStrICmp(char *pszString1, char *pszString2);
typedef int (CDECL * PFCMUTLSTRICMP) (char *pszString1, char *pszString2);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRICMP_NOTIMPLEMENTED)
	#define USE_CMUtlStrICmp
	#define EXT_CMUtlStrICmp
	#define GET_CMUtlStrICmp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrICmp(p0,p1)  (int)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrICmp  FALSE
	#define EXP_CMUtlStrICmp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrICmp
	#define EXT_CMUtlStrICmp
	#define GET_CMUtlStrICmp(fl)  CAL_CMGETAPI( "CMUtlStrICmp" ) 
	#define CAL_CMUtlStrICmp  CMUtlStrICmp
	#define CHK_CMUtlStrICmp  TRUE
	#define EXP_CMUtlStrICmp  CAL_CMEXPAPI( "CMUtlStrICmp" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrICmp
	#define EXT_CMUtlStrICmp
	#define GET_CMUtlStrICmp(fl)  CAL_CMGETAPI( "CMUtlStrICmp" ) 
	#define CAL_CMUtlStrICmp  CMUtlStrICmp
	#define CHK_CMUtlStrICmp  TRUE
	#define EXP_CMUtlStrICmp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrICmp", (RTS_UINTPTR)CMUtlStrICmp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrICmp
	#define EXT_CMUtilsCMUtlStrICmp
	#define GET_CMUtilsCMUtlStrICmp  ERR_OK
	#define CAL_CMUtilsCMUtlStrICmp pICMUtils->ICMUtlStrICmp
	#define CHK_CMUtilsCMUtlStrICmp (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrICmp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrICmp
	#define EXT_CMUtlStrICmp
	#define GET_CMUtlStrICmp(fl)  CAL_CMGETAPI( "CMUtlStrICmp" ) 
	#define CAL_CMUtlStrICmp pICMUtils->ICMUtlStrICmp
	#define CHK_CMUtlStrICmp (pICMUtils != NULL)
	#define EXP_CMUtlStrICmp  CAL_CMEXPAPI( "CMUtlStrICmp" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrICmp  PFCMUTLSTRICMP pfCMUtlStrICmp;
	#define EXT_CMUtlStrICmp  extern PFCMUTLSTRICMP pfCMUtlStrICmp;
	#define GET_CMUtlStrICmp(fl)  s_pfCMGetAPI2( "CMUtlStrICmp", (RTS_VOID_FCTPTR *)&pfCMUtlStrICmp, (fl), 0, 0)
	#define CAL_CMUtlStrICmp  pfCMUtlStrICmp
	#define CHK_CMUtlStrICmp  (pfCMUtlStrICmp != NULL)
	#define EXP_CMUtlStrICmp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrICmp", (RTS_UINTPTR)CMUtlStrICmp, 0, 0) 
#endif




/**
 * <description> 
 *  Lower case comparison of two strings specified by length.
 * </description>
 * <param name="pszString1" type="IN">Pointer to first string to compare</param>
 * <param name="pszString2" type="IN">Pointer to second string to compare</param>
 * <param name="n" type="IN">Number of characters to compare</param>
 * <result>0= strings are equal, !0=strings unequal</result>
 */
int CDECL CMUtlStrNICmp(char *pszString1, char *pszString2, RTS_SIZE n);
typedef int (CDECL * PFCMUTLSTRNICMP) (char *pszString1, char *pszString2, RTS_SIZE n);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRNICMP_NOTIMPLEMENTED)
	#define USE_CMUtlStrNICmp
	#define EXT_CMUtlStrNICmp
	#define GET_CMUtlStrNICmp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrNICmp(p0,p1,p2)  (int)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrNICmp  FALSE
	#define EXP_CMUtlStrNICmp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrNICmp
	#define EXT_CMUtlStrNICmp
	#define GET_CMUtlStrNICmp(fl)  CAL_CMGETAPI( "CMUtlStrNICmp" ) 
	#define CAL_CMUtlStrNICmp  CMUtlStrNICmp
	#define CHK_CMUtlStrNICmp  TRUE
	#define EXP_CMUtlStrNICmp  CAL_CMEXPAPI( "CMUtlStrNICmp" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrNICmp
	#define EXT_CMUtlStrNICmp
	#define GET_CMUtlStrNICmp(fl)  CAL_CMGETAPI( "CMUtlStrNICmp" ) 
	#define CAL_CMUtlStrNICmp  CMUtlStrNICmp
	#define CHK_CMUtlStrNICmp  TRUE
	#define EXP_CMUtlStrNICmp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrNICmp", (RTS_UINTPTR)CMUtlStrNICmp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrNICmp
	#define EXT_CMUtilsCMUtlStrNICmp
	#define GET_CMUtilsCMUtlStrNICmp  ERR_OK
	#define CAL_CMUtilsCMUtlStrNICmp pICMUtils->ICMUtlStrNICmp
	#define CHK_CMUtilsCMUtlStrNICmp (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrNICmp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrNICmp
	#define EXT_CMUtlStrNICmp
	#define GET_CMUtlStrNICmp(fl)  CAL_CMGETAPI( "CMUtlStrNICmp" ) 
	#define CAL_CMUtlStrNICmp pICMUtils->ICMUtlStrNICmp
	#define CHK_CMUtlStrNICmp (pICMUtils != NULL)
	#define EXP_CMUtlStrNICmp  CAL_CMEXPAPI( "CMUtlStrNICmp" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrNICmp  PFCMUTLSTRNICMP pfCMUtlStrNICmp;
	#define EXT_CMUtlStrNICmp  extern PFCMUTLSTRNICMP pfCMUtlStrNICmp;
	#define GET_CMUtlStrNICmp(fl)  s_pfCMGetAPI2( "CMUtlStrNICmp", (RTS_VOID_FCTPTR *)&pfCMUtlStrNICmp, (fl), 0, 0)
	#define CAL_CMUtlStrNICmp  pfCMUtlStrNICmp
	#define CHK_CMUtlStrNICmp  (pfCMUtlStrNICmp != NULL)
	#define EXP_CMUtlStrNICmp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrNICmp", (RTS_UINTPTR)CMUtlStrNICmp, 0, 0) 
#endif




/**
 * <description> 
 *  Find a substring in another string.
 * </description>
 * <param name="pszString" type="IN">String to search substring in</param>
 * <param name="pszStringToFind" type="IN">Substring to find</param>
  * <result>NULL= substring not found in string, !NULL= pointer to string, where substring was found</result>
 */
char * CDECL CMUtlStrIStr(const char *pszString, const char *pszStringToFind);
typedef char * (CDECL * PFCMUTLSTRISTR) (const char *pszString, const char *pszStringToFind);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRISTR_NOTIMPLEMENTED)
	#define USE_CMUtlStrIStr
	#define EXT_CMUtlStrIStr
	#define GET_CMUtlStrIStr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrIStr(p0,p1)  (char *)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrIStr  FALSE
	#define EXP_CMUtlStrIStr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrIStr
	#define EXT_CMUtlStrIStr
	#define GET_CMUtlStrIStr(fl)  CAL_CMGETAPI( "CMUtlStrIStr" ) 
	#define CAL_CMUtlStrIStr  CMUtlStrIStr
	#define CHK_CMUtlStrIStr  TRUE
	#define EXP_CMUtlStrIStr  CAL_CMEXPAPI( "CMUtlStrIStr" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrIStr
	#define EXT_CMUtlStrIStr
	#define GET_CMUtlStrIStr(fl)  CAL_CMGETAPI( "CMUtlStrIStr" ) 
	#define CAL_CMUtlStrIStr  CMUtlStrIStr
	#define CHK_CMUtlStrIStr  TRUE
	#define EXP_CMUtlStrIStr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrIStr", (RTS_UINTPTR)CMUtlStrIStr, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrIStr
	#define EXT_CMUtilsCMUtlStrIStr
	#define GET_CMUtilsCMUtlStrIStr  ERR_OK
	#define CAL_CMUtilsCMUtlStrIStr pICMUtils->ICMUtlStrIStr
	#define CHK_CMUtilsCMUtlStrIStr (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrIStr  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrIStr
	#define EXT_CMUtlStrIStr
	#define GET_CMUtlStrIStr(fl)  CAL_CMGETAPI( "CMUtlStrIStr" ) 
	#define CAL_CMUtlStrIStr pICMUtils->ICMUtlStrIStr
	#define CHK_CMUtlStrIStr (pICMUtils != NULL)
	#define EXP_CMUtlStrIStr  CAL_CMEXPAPI( "CMUtlStrIStr" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrIStr  PFCMUTLSTRISTR pfCMUtlStrIStr;
	#define EXT_CMUtlStrIStr  extern PFCMUTLSTRISTR pfCMUtlStrIStr;
	#define GET_CMUtlStrIStr(fl)  s_pfCMGetAPI2( "CMUtlStrIStr", (RTS_VOID_FCTPTR *)&pfCMUtlStrIStr, (fl), 0, 0)
	#define CAL_CMUtlStrIStr  pfCMUtlStrIStr
	#define CHK_CMUtlStrIStr  (pfCMUtlStrIStr != NULL)
	#define EXP_CMUtlStrIStr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrIStr", (RTS_UINTPTR)CMUtlStrIStr, 0, 0) 
#endif




/**
 * <description> 
 *  Lower case conversion of a string.
 * </description>
 * <param name="pszString" type="IN">Pointer to conversion string</param>
 * <result>Returns a pointer to the converted string, NULL if conversion failed</result>
 */
char* CDECL CMUtlStrlwr(char *pszString);
typedef char* (CDECL * PFCMUTLSTRLWR) (char *pszString);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRLWR_NOTIMPLEMENTED)
	#define USE_CMUtlStrlwr
	#define EXT_CMUtlStrlwr
	#define GET_CMUtlStrlwr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrlwr(p0)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrlwr  FALSE
	#define EXP_CMUtlStrlwr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrlwr
	#define EXT_CMUtlStrlwr
	#define GET_CMUtlStrlwr(fl)  CAL_CMGETAPI( "CMUtlStrlwr" ) 
	#define CAL_CMUtlStrlwr  CMUtlStrlwr
	#define CHK_CMUtlStrlwr  TRUE
	#define EXP_CMUtlStrlwr  CAL_CMEXPAPI( "CMUtlStrlwr" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrlwr
	#define EXT_CMUtlStrlwr
	#define GET_CMUtlStrlwr(fl)  CAL_CMGETAPI( "CMUtlStrlwr" ) 
	#define CAL_CMUtlStrlwr  CMUtlStrlwr
	#define CHK_CMUtlStrlwr  TRUE
	#define EXP_CMUtlStrlwr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrlwr", (RTS_UINTPTR)CMUtlStrlwr, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrlwr
	#define EXT_CMUtilsCMUtlStrlwr
	#define GET_CMUtilsCMUtlStrlwr  ERR_OK
	#define CAL_CMUtilsCMUtlStrlwr pICMUtils->ICMUtlStrlwr
	#define CHK_CMUtilsCMUtlStrlwr (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrlwr  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrlwr
	#define EXT_CMUtlStrlwr
	#define GET_CMUtlStrlwr(fl)  CAL_CMGETAPI( "CMUtlStrlwr" ) 
	#define CAL_CMUtlStrlwr pICMUtils->ICMUtlStrlwr
	#define CHK_CMUtlStrlwr (pICMUtils != NULL)
	#define EXP_CMUtlStrlwr  CAL_CMEXPAPI( "CMUtlStrlwr" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrlwr  PFCMUTLSTRLWR pfCMUtlStrlwr;
	#define EXT_CMUtlStrlwr  extern PFCMUTLSTRLWR pfCMUtlStrlwr;
	#define GET_CMUtlStrlwr(fl)  s_pfCMGetAPI2( "CMUtlStrlwr", (RTS_VOID_FCTPTR *)&pfCMUtlStrlwr, (fl), 0, 0)
	#define CAL_CMUtlStrlwr  pfCMUtlStrlwr
	#define CHK_CMUtlStrlwr  (pfCMUtlStrlwr != NULL)
	#define EXP_CMUtlStrlwr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrlwr", (RTS_UINTPTR)CMUtlStrlwr, 0, 0) 
#endif




/**
 * <description> 
 *  Upper case conversion of a string.
 * </description>
 * <param name="pszString" type="IN">Pointer to conversion string</param>
 * <result>Returns a pointer to the converted string, NULL if conversion failed</result>
 */
char* CDECL CMUtlStrupr(char *pszString);
typedef char* (CDECL * PFCMUTLSTRUPR) (char *pszString);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRUPR_NOTIMPLEMENTED)
	#define USE_CMUtlStrupr
	#define EXT_CMUtlStrupr
	#define GET_CMUtlStrupr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrupr(p0)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrupr  FALSE
	#define EXP_CMUtlStrupr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrupr
	#define EXT_CMUtlStrupr
	#define GET_CMUtlStrupr(fl)  CAL_CMGETAPI( "CMUtlStrupr" ) 
	#define CAL_CMUtlStrupr  CMUtlStrupr
	#define CHK_CMUtlStrupr  TRUE
	#define EXP_CMUtlStrupr  CAL_CMEXPAPI( "CMUtlStrupr" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrupr
	#define EXT_CMUtlStrupr
	#define GET_CMUtlStrupr(fl)  CAL_CMGETAPI( "CMUtlStrupr" ) 
	#define CAL_CMUtlStrupr  CMUtlStrupr
	#define CHK_CMUtlStrupr  TRUE
	#define EXP_CMUtlStrupr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrupr", (RTS_UINTPTR)CMUtlStrupr, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrupr
	#define EXT_CMUtilsCMUtlStrupr
	#define GET_CMUtilsCMUtlStrupr  ERR_OK
	#define CAL_CMUtilsCMUtlStrupr pICMUtils->ICMUtlStrupr
	#define CHK_CMUtilsCMUtlStrupr (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrupr  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrupr
	#define EXT_CMUtlStrupr
	#define GET_CMUtlStrupr(fl)  CAL_CMGETAPI( "CMUtlStrupr" ) 
	#define CAL_CMUtlStrupr pICMUtils->ICMUtlStrupr
	#define CHK_CMUtlStrupr (pICMUtils != NULL)
	#define EXP_CMUtlStrupr  CAL_CMEXPAPI( "CMUtlStrupr" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrupr  PFCMUTLSTRUPR pfCMUtlStrupr;
	#define EXT_CMUtlStrupr  extern PFCMUTLSTRUPR pfCMUtlStrupr;
	#define GET_CMUtlStrupr(fl)  s_pfCMGetAPI2( "CMUtlStrupr", (RTS_VOID_FCTPTR *)&pfCMUtlStrupr, (fl), 0, 0)
	#define CAL_CMUtlStrupr  pfCMUtlStrupr
	#define CHK_CMUtlStrupr  (pfCMUtlStrupr != NULL)
	#define EXP_CMUtlStrupr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrupr", (RTS_UINTPTR)CMUtlStrupr, 0, 0) 
#endif




/**
 * <description> 
 *  Safe string copy of a string. The length of the destination buffer is checked to avoid memory overwrites.
 * </description>
 * <param name="pszDest" type="IN">Pointer to destination buffer</param>
 * <param name="nDestSize" type="IN">Length of destination buffer</param>
 * <param name="pszSrc" type="IN">Pointer to source buffer. String must be NUL terminated!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlSafeStrCpy(char *pszDest, RTS_SIZE nDestSize, const char *pszSrc);
typedef RTS_RESULT (CDECL * PFCMUTLSAFESTRCPY) (char *pszDest, RTS_SIZE nDestSize, const char *pszSrc);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSAFESTRCPY_NOTIMPLEMENTED)
	#define USE_CMUtlSafeStrCpy
	#define EXT_CMUtlSafeStrCpy
	#define GET_CMUtlSafeStrCpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlSafeStrCpy(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlSafeStrCpy  FALSE
	#define EXP_CMUtlSafeStrCpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlSafeStrCpy
	#define EXT_CMUtlSafeStrCpy
	#define GET_CMUtlSafeStrCpy(fl)  CAL_CMGETAPI( "CMUtlSafeStrCpy" ) 
	#define CAL_CMUtlSafeStrCpy  CMUtlSafeStrCpy
	#define CHK_CMUtlSafeStrCpy  TRUE
	#define EXP_CMUtlSafeStrCpy  CAL_CMEXPAPI( "CMUtlSafeStrCpy" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlSafeStrCpy
	#define EXT_CMUtlSafeStrCpy
	#define GET_CMUtlSafeStrCpy(fl)  CAL_CMGETAPI( "CMUtlSafeStrCpy" ) 
	#define CAL_CMUtlSafeStrCpy  CMUtlSafeStrCpy
	#define CHK_CMUtlSafeStrCpy  TRUE
	#define EXP_CMUtlSafeStrCpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeStrCpy", (RTS_UINTPTR)CMUtlSafeStrCpy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlSafeStrCpy
	#define EXT_CMUtilsCMUtlSafeStrCpy
	#define GET_CMUtilsCMUtlSafeStrCpy  ERR_OK
	#define CAL_CMUtilsCMUtlSafeStrCpy pICMUtils->ICMUtlSafeStrCpy
	#define CHK_CMUtilsCMUtlSafeStrCpy (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlSafeStrCpy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlSafeStrCpy
	#define EXT_CMUtlSafeStrCpy
	#define GET_CMUtlSafeStrCpy(fl)  CAL_CMGETAPI( "CMUtlSafeStrCpy" ) 
	#define CAL_CMUtlSafeStrCpy pICMUtils->ICMUtlSafeStrCpy
	#define CHK_CMUtlSafeStrCpy (pICMUtils != NULL)
	#define EXP_CMUtlSafeStrCpy  CAL_CMEXPAPI( "CMUtlSafeStrCpy" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlSafeStrCpy  PFCMUTLSAFESTRCPY pfCMUtlSafeStrCpy;
	#define EXT_CMUtlSafeStrCpy  extern PFCMUTLSAFESTRCPY pfCMUtlSafeStrCpy;
	#define GET_CMUtlSafeStrCpy(fl)  s_pfCMGetAPI2( "CMUtlSafeStrCpy", (RTS_VOID_FCTPTR *)&pfCMUtlSafeStrCpy, (fl), 0, 0)
	#define CAL_CMUtlSafeStrCpy  pfCMUtlSafeStrCpy
	#define CHK_CMUtlSafeStrCpy  (pfCMUtlSafeStrCpy != NULL)
	#define EXP_CMUtlSafeStrCpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeStrCpy", (RTS_UINTPTR)CMUtlSafeStrCpy, 0, 0) 
#endif




/**
 * <description> 
 * <p>Copy a string from one buffer to another, but limit the size of the
 * destination buffer to nDestSize.</p>
 * </description>
 * <param name="pszDest" type="IN" range="[0,VALID_POINTER_TO_STRINGDEST]">Pointer to destination buffer</param>
 * <param name="nDestSize" type="IN" range="[0,INVALID_DST_SIZE,VALID_DST_SIZE]">Length of destination buffer</param>
 * <param name="pszSrc" type="IN" range="[0,VALID_POINTER_TO_STRINGSRC]">Pointer to source buffer. String must be NUL terminated!</param>
 * <param name="nCopyMode" type="IN" range="[0,CMUTL_CPY_COMM2BUFFER,CMUTL_CPY_BUFFER2COMM,CMUTL_CPY_DEFAULT]">Copy mode. See corresponding category "Copy mode"</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Copied successful!</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_BUFFERSIZE">Size of SrcString is greater than nDestSize!</errorcode> 
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlSafeStrCpy2(char *pszDest, RTS_SIZE nDestSize, const char *pszSrc, int nCopyMode);
typedef RTS_RESULT (CDECL * PFCMUTLSAFESTRCPY2) (char *pszDest, RTS_SIZE nDestSize, const char *pszSrc, int nCopyMode);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSAFESTRCPY2_NOTIMPLEMENTED)
	#define USE_CMUtlSafeStrCpy2
	#define EXT_CMUtlSafeStrCpy2
	#define GET_CMUtlSafeStrCpy2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlSafeStrCpy2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlSafeStrCpy2  FALSE
	#define EXP_CMUtlSafeStrCpy2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlSafeStrCpy2
	#define EXT_CMUtlSafeStrCpy2
	#define GET_CMUtlSafeStrCpy2(fl)  CAL_CMGETAPI( "CMUtlSafeStrCpy2" ) 
	#define CAL_CMUtlSafeStrCpy2  CMUtlSafeStrCpy2
	#define CHK_CMUtlSafeStrCpy2  TRUE
	#define EXP_CMUtlSafeStrCpy2  CAL_CMEXPAPI( "CMUtlSafeStrCpy2" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlSafeStrCpy2
	#define EXT_CMUtlSafeStrCpy2
	#define GET_CMUtlSafeStrCpy2(fl)  CAL_CMGETAPI( "CMUtlSafeStrCpy2" ) 
	#define CAL_CMUtlSafeStrCpy2  CMUtlSafeStrCpy2
	#define CHK_CMUtlSafeStrCpy2  TRUE
	#define EXP_CMUtlSafeStrCpy2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeStrCpy2", (RTS_UINTPTR)CMUtlSafeStrCpy2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlSafeStrCpy2
	#define EXT_CMUtilsCMUtlSafeStrCpy2
	#define GET_CMUtilsCMUtlSafeStrCpy2  ERR_OK
	#define CAL_CMUtilsCMUtlSafeStrCpy2 pICMUtils->ICMUtlSafeStrCpy2
	#define CHK_CMUtilsCMUtlSafeStrCpy2 (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlSafeStrCpy2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlSafeStrCpy2
	#define EXT_CMUtlSafeStrCpy2
	#define GET_CMUtlSafeStrCpy2(fl)  CAL_CMGETAPI( "CMUtlSafeStrCpy2" ) 
	#define CAL_CMUtlSafeStrCpy2 pICMUtils->ICMUtlSafeStrCpy2
	#define CHK_CMUtlSafeStrCpy2 (pICMUtils != NULL)
	#define EXP_CMUtlSafeStrCpy2  CAL_CMEXPAPI( "CMUtlSafeStrCpy2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlSafeStrCpy2  PFCMUTLSAFESTRCPY2 pfCMUtlSafeStrCpy2;
	#define EXT_CMUtlSafeStrCpy2  extern PFCMUTLSAFESTRCPY2 pfCMUtlSafeStrCpy2;
	#define GET_CMUtlSafeStrCpy2(fl)  s_pfCMGetAPI2( "CMUtlSafeStrCpy2", (RTS_VOID_FCTPTR *)&pfCMUtlSafeStrCpy2, (fl), 0, 0)
	#define CAL_CMUtlSafeStrCpy2  pfCMUtlSafeStrCpy2
	#define CHK_CMUtlSafeStrCpy2  (pfCMUtlSafeStrCpy2 != NULL)
	#define EXP_CMUtlSafeStrCpy2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeStrCpy2", (RTS_UINTPTR)CMUtlSafeStrCpy2, 0, 0) 
#endif




/**
 * <description> 
 * <p>Copy a string from one buffer to another specified by the number of characters to copy, but limit the size of the
 * destination buffer to nDestSize.</p>
 * </description>
 * <param name="pszDest" type="IN" range="[RTS_INVALID_HANDLE,NULL,VALID_POINTER_TO_STRINGDEST]">Pointer to destination buffer</param>
 * <param name="nDestSize" type="IN" range="[0,1,RTS_SIZE_MAX]">Length of destination buffer</param>
 * <param name="pszSrc" type="IN" range="[RTS_INVALID_HANDLE,NULL,VALID_POINTER_TO_STRINGSRC]">Pointer to source buffer. String must be NUL terminated!</param>
 * <param name="nBytesCopy" type="IN" range="[0,1,RTS_SIZE_MAX]">Number of bytes to copy</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Copy successful</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_BUFFERSIZE">Destination buffer too short. Only the possible number of characters copied</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlSafeStrNCpy(char *pszDest, RTS_SIZE nDestSize, const char *pszSrc, RTS_SIZE nBytesCopy);
typedef RTS_RESULT (CDECL * PFCMUTLSAFESTRNCPY) (char *pszDest, RTS_SIZE nDestSize, const char *pszSrc, RTS_SIZE nBytesCopy);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSAFESTRNCPY_NOTIMPLEMENTED)
	#define USE_CMUtlSafeStrNCpy
	#define EXT_CMUtlSafeStrNCpy
	#define GET_CMUtlSafeStrNCpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlSafeStrNCpy(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlSafeStrNCpy  FALSE
	#define EXP_CMUtlSafeStrNCpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlSafeStrNCpy
	#define EXT_CMUtlSafeStrNCpy
	#define GET_CMUtlSafeStrNCpy(fl)  CAL_CMGETAPI( "CMUtlSafeStrNCpy" ) 
	#define CAL_CMUtlSafeStrNCpy  CMUtlSafeStrNCpy
	#define CHK_CMUtlSafeStrNCpy  TRUE
	#define EXP_CMUtlSafeStrNCpy  CAL_CMEXPAPI( "CMUtlSafeStrNCpy" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlSafeStrNCpy
	#define EXT_CMUtlSafeStrNCpy
	#define GET_CMUtlSafeStrNCpy(fl)  CAL_CMGETAPI( "CMUtlSafeStrNCpy" ) 
	#define CAL_CMUtlSafeStrNCpy  CMUtlSafeStrNCpy
	#define CHK_CMUtlSafeStrNCpy  TRUE
	#define EXP_CMUtlSafeStrNCpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeStrNCpy", (RTS_UINTPTR)CMUtlSafeStrNCpy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlSafeStrNCpy
	#define EXT_CMUtilsCMUtlSafeStrNCpy
	#define GET_CMUtilsCMUtlSafeStrNCpy  ERR_OK
	#define CAL_CMUtilsCMUtlSafeStrNCpy pICMUtils->ICMUtlSafeStrNCpy
	#define CHK_CMUtilsCMUtlSafeStrNCpy (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlSafeStrNCpy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlSafeStrNCpy
	#define EXT_CMUtlSafeStrNCpy
	#define GET_CMUtlSafeStrNCpy(fl)  CAL_CMGETAPI( "CMUtlSafeStrNCpy" ) 
	#define CAL_CMUtlSafeStrNCpy pICMUtils->ICMUtlSafeStrNCpy
	#define CHK_CMUtlSafeStrNCpy (pICMUtils != NULL)
	#define EXP_CMUtlSafeStrNCpy  CAL_CMEXPAPI( "CMUtlSafeStrNCpy" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlSafeStrNCpy  PFCMUTLSAFESTRNCPY pfCMUtlSafeStrNCpy;
	#define EXT_CMUtlSafeStrNCpy  extern PFCMUTLSAFESTRNCPY pfCMUtlSafeStrNCpy;
	#define GET_CMUtlSafeStrNCpy(fl)  s_pfCMGetAPI2( "CMUtlSafeStrNCpy", (RTS_VOID_FCTPTR *)&pfCMUtlSafeStrNCpy, (fl), 0, 0)
	#define CAL_CMUtlSafeStrNCpy  pfCMUtlSafeStrNCpy
	#define CHK_CMUtlSafeStrNCpy  (pfCMUtlSafeStrNCpy != NULL)
	#define EXP_CMUtlSafeStrNCpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeStrNCpy", (RTS_UINTPTR)CMUtlSafeStrNCpy, 0, 0) 
#endif




/**
 * <description> 
 * <p>Concatenate two strings, but limit the size of the destination buffer to nDestSize.</p>
 * </description>
 * <param name="pszDest" type="IN" range="[RTS_INVALID_HANDLE,NULL,VALID_POINTER_TO_STRINGDEST]">Pointer to destination buffer</param>
 * <param name="nDestSize" type="IN" range="[0,1,RTS_SIZE_MAX]">Length of destination buffer</param>
 * <param name="pszSrc" type="IN" range="[RTS_INVALID_HANDLE,NULL,VALID_POINTER_TO_STRINGSRC]">Pointer to source buffer. String must be NUL terminated!</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Concatenate successful</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_BUFFERSIZE">Destination buffer too short. Only the possible number of characters copied</errorcode>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlSafeStrCat(char *pszDest, RTS_SIZE nDestSize, const char *pszSrc);
typedef RTS_RESULT (CDECL * PFCMUTLSAFESTRCAT) (char *pszDest, RTS_SIZE nDestSize, const char *pszSrc);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSAFESTRCAT_NOTIMPLEMENTED)
	#define USE_CMUtlSafeStrCat
	#define EXT_CMUtlSafeStrCat
	#define GET_CMUtlSafeStrCat(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlSafeStrCat(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlSafeStrCat  FALSE
	#define EXP_CMUtlSafeStrCat  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlSafeStrCat
	#define EXT_CMUtlSafeStrCat
	#define GET_CMUtlSafeStrCat(fl)  CAL_CMGETAPI( "CMUtlSafeStrCat" ) 
	#define CAL_CMUtlSafeStrCat  CMUtlSafeStrCat
	#define CHK_CMUtlSafeStrCat  TRUE
	#define EXP_CMUtlSafeStrCat  CAL_CMEXPAPI( "CMUtlSafeStrCat" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlSafeStrCat
	#define EXT_CMUtlSafeStrCat
	#define GET_CMUtlSafeStrCat(fl)  CAL_CMGETAPI( "CMUtlSafeStrCat" ) 
	#define CAL_CMUtlSafeStrCat  CMUtlSafeStrCat
	#define CHK_CMUtlSafeStrCat  TRUE
	#define EXP_CMUtlSafeStrCat  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeStrCat", (RTS_UINTPTR)CMUtlSafeStrCat, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlSafeStrCat
	#define EXT_CMUtilsCMUtlSafeStrCat
	#define GET_CMUtilsCMUtlSafeStrCat  ERR_OK
	#define CAL_CMUtilsCMUtlSafeStrCat pICMUtils->ICMUtlSafeStrCat
	#define CHK_CMUtilsCMUtlSafeStrCat (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlSafeStrCat  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlSafeStrCat
	#define EXT_CMUtlSafeStrCat
	#define GET_CMUtlSafeStrCat(fl)  CAL_CMGETAPI( "CMUtlSafeStrCat" ) 
	#define CAL_CMUtlSafeStrCat pICMUtils->ICMUtlSafeStrCat
	#define CHK_CMUtlSafeStrCat (pICMUtils != NULL)
	#define EXP_CMUtlSafeStrCat  CAL_CMEXPAPI( "CMUtlSafeStrCat" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlSafeStrCat  PFCMUTLSAFESTRCAT pfCMUtlSafeStrCat;
	#define EXT_CMUtlSafeStrCat  extern PFCMUTLSAFESTRCAT pfCMUtlSafeStrCat;
	#define GET_CMUtlSafeStrCat(fl)  s_pfCMGetAPI2( "CMUtlSafeStrCat", (RTS_VOID_FCTPTR *)&pfCMUtlSafeStrCat, (fl), 0, 0)
	#define CAL_CMUtlSafeStrCat  pfCMUtlSafeStrCat
	#define CHK_CMUtlSafeStrCat  (pfCMUtlSafeStrCat != NULL)
	#define EXP_CMUtlSafeStrCat  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeStrCat", (RTS_UINTPTR)CMUtlSafeStrCat, 0, 0) 
#endif




/**
 * <description>
 * <p>Calculate the string length</p>
 * <p>Is able to handle Null pointer</p>
 * <p>This function works for targets where char has a size of 8 bits as well
 * as for targets where char is 16 bits wide.</p>
 * </description>
 * <param name="str" type="IN" range="[NULL,VALID_POINTER_TO_STRING1]">pointer to string</param>
 * <result>Length of string without trailing \0, 0 if str is Null</result>
 */
RTS_SIZE CDECL CMUtlStrLen(char *str);
typedef RTS_SIZE (CDECL * PFCMUTLSTRLEN) (char *str);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRLEN_NOTIMPLEMENTED)
	#define USE_CMUtlStrLen
	#define EXT_CMUtlStrLen
	#define GET_CMUtlStrLen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrLen(p0)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrLen  FALSE
	#define EXP_CMUtlStrLen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrLen
	#define EXT_CMUtlStrLen
	#define GET_CMUtlStrLen(fl)  CAL_CMGETAPI( "CMUtlStrLen" ) 
	#define CAL_CMUtlStrLen  CMUtlStrLen
	#define CHK_CMUtlStrLen  TRUE
	#define EXP_CMUtlStrLen  CAL_CMEXPAPI( "CMUtlStrLen" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrLen
	#define EXT_CMUtlStrLen
	#define GET_CMUtlStrLen(fl)  CAL_CMGETAPI( "CMUtlStrLen" ) 
	#define CAL_CMUtlStrLen  CMUtlStrLen
	#define CHK_CMUtlStrLen  TRUE
	#define EXP_CMUtlStrLen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrLen", (RTS_UINTPTR)CMUtlStrLen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrLen
	#define EXT_CMUtilsCMUtlStrLen
	#define GET_CMUtilsCMUtlStrLen  ERR_OK
	#define CAL_CMUtilsCMUtlStrLen pICMUtils->ICMUtlStrLen
	#define CHK_CMUtilsCMUtlStrLen (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrLen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrLen
	#define EXT_CMUtlStrLen
	#define GET_CMUtlStrLen(fl)  CAL_CMGETAPI( "CMUtlStrLen" ) 
	#define CAL_CMUtlStrLen pICMUtils->ICMUtlStrLen
	#define CHK_CMUtlStrLen (pICMUtils != NULL)
	#define EXP_CMUtlStrLen  CAL_CMEXPAPI( "CMUtlStrLen" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrLen  PFCMUTLSTRLEN pfCMUtlStrLen;
	#define EXT_CMUtlStrLen  extern PFCMUTLSTRLEN pfCMUtlStrLen;
	#define GET_CMUtlStrLen(fl)  s_pfCMGetAPI2( "CMUtlStrLen", (RTS_VOID_FCTPTR *)&pfCMUtlStrLen, (fl), 0, 0)
	#define CAL_CMUtlStrLen  pfCMUtlStrLen
	#define CHK_CMUtlStrLen  (pfCMUtlStrLen != NULL)
	#define EXP_CMUtlStrLen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrLen", (RTS_UINTPTR)CMUtlStrLen, 0, 0) 
#endif




/**
 * <description>
 * <p>Calculate the string length. This function will stop if no NUL is found in the first nMaxSize bytes of the string.</p>
 * <p>Is able to handle Null pointer</p>
 * <p>This function works for targets where char has a size of 8 bits as well
 * as for targets where char is 16 bits wide.</p>
 * </description>
 * <result>Length of string without trailing \0, 0 if str is Null, nMaxSize if no NUL is found.</result>
 */
RTS_SIZE CDECL CMUtlStrNLen(char *str, RTS_SIZE nMaxSize);
typedef RTS_SIZE (CDECL * PFCMUTLSTRNLEN) (char *str, RTS_SIZE nMaxSize);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRNLEN_NOTIMPLEMENTED)
	#define USE_CMUtlStrNLen
	#define EXT_CMUtlStrNLen
	#define GET_CMUtlStrNLen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrNLen(p0,p1)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrNLen  FALSE
	#define EXP_CMUtlStrNLen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrNLen
	#define EXT_CMUtlStrNLen
	#define GET_CMUtlStrNLen(fl)  CAL_CMGETAPI( "CMUtlStrNLen" ) 
	#define CAL_CMUtlStrNLen  CMUtlStrNLen
	#define CHK_CMUtlStrNLen  TRUE
	#define EXP_CMUtlStrNLen  CAL_CMEXPAPI( "CMUtlStrNLen" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrNLen
	#define EXT_CMUtlStrNLen
	#define GET_CMUtlStrNLen(fl)  CAL_CMGETAPI( "CMUtlStrNLen" ) 
	#define CAL_CMUtlStrNLen  CMUtlStrNLen
	#define CHK_CMUtlStrNLen  TRUE
	#define EXP_CMUtlStrNLen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrNLen", (RTS_UINTPTR)CMUtlStrNLen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrNLen
	#define EXT_CMUtilsCMUtlStrNLen
	#define GET_CMUtilsCMUtlStrNLen  ERR_OK
	#define CAL_CMUtilsCMUtlStrNLen pICMUtils->ICMUtlStrNLen
	#define CHK_CMUtilsCMUtlStrNLen (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrNLen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrNLen
	#define EXT_CMUtlStrNLen
	#define GET_CMUtlStrNLen(fl)  CAL_CMGETAPI( "CMUtlStrNLen" ) 
	#define CAL_CMUtlStrNLen pICMUtils->ICMUtlStrNLen
	#define CHK_CMUtlStrNLen (pICMUtils != NULL)
	#define EXP_CMUtlStrNLen  CAL_CMEXPAPI( "CMUtlStrNLen" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrNLen  PFCMUTLSTRNLEN pfCMUtlStrNLen;
	#define EXT_CMUtlStrNLen  extern PFCMUTLSTRNLEN pfCMUtlStrNLen;
	#define GET_CMUtlStrNLen(fl)  s_pfCMGetAPI2( "CMUtlStrNLen", (RTS_VOID_FCTPTR *)&pfCMUtlStrNLen, (fl), 0, 0)
	#define CAL_CMUtlStrNLen  pfCMUtlStrNLen
	#define CHK_CMUtlStrNLen  (pfCMUtlStrNLen != NULL)
	#define EXP_CMUtlStrNLen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrNLen", (RTS_UINTPTR)CMUtlStrNLen, 0, 0) 
#endif




/**
 * <description> 
 *  Convert a string in lower case. String must be NUL terminated!
 * </description>
 * <param name="psz" type="IN">Pointer to string to convert</param>
 * <result>Pointer to psz with the converted content</result>
 */
char* CDECL CMUtlStrToLower(char* psz);
typedef char* (CDECL * PFCMUTLSTRTOLOWER) (char* psz);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRTOLOWER_NOTIMPLEMENTED)
	#define USE_CMUtlStrToLower
	#define EXT_CMUtlStrToLower
	#define GET_CMUtlStrToLower(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrToLower(p0)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrToLower  FALSE
	#define EXP_CMUtlStrToLower  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrToLower
	#define EXT_CMUtlStrToLower
	#define GET_CMUtlStrToLower(fl)  CAL_CMGETAPI( "CMUtlStrToLower" ) 
	#define CAL_CMUtlStrToLower  CMUtlStrToLower
	#define CHK_CMUtlStrToLower  TRUE
	#define EXP_CMUtlStrToLower  CAL_CMEXPAPI( "CMUtlStrToLower" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrToLower
	#define EXT_CMUtlStrToLower
	#define GET_CMUtlStrToLower(fl)  CAL_CMGETAPI( "CMUtlStrToLower" ) 
	#define CAL_CMUtlStrToLower  CMUtlStrToLower
	#define CHK_CMUtlStrToLower  TRUE
	#define EXP_CMUtlStrToLower  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrToLower", (RTS_UINTPTR)CMUtlStrToLower, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrToLower
	#define EXT_CMUtilsCMUtlStrToLower
	#define GET_CMUtilsCMUtlStrToLower  ERR_OK
	#define CAL_CMUtilsCMUtlStrToLower pICMUtils->ICMUtlStrToLower
	#define CHK_CMUtilsCMUtlStrToLower (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrToLower  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrToLower
	#define EXT_CMUtlStrToLower
	#define GET_CMUtlStrToLower(fl)  CAL_CMGETAPI( "CMUtlStrToLower" ) 
	#define CAL_CMUtlStrToLower pICMUtils->ICMUtlStrToLower
	#define CHK_CMUtlStrToLower (pICMUtils != NULL)
	#define EXP_CMUtlStrToLower  CAL_CMEXPAPI( "CMUtlStrToLower" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrToLower  PFCMUTLSTRTOLOWER pfCMUtlStrToLower;
	#define EXT_CMUtlStrToLower  extern PFCMUTLSTRTOLOWER pfCMUtlStrToLower;
	#define GET_CMUtlStrToLower(fl)  s_pfCMGetAPI2( "CMUtlStrToLower", (RTS_VOID_FCTPTR *)&pfCMUtlStrToLower, (fl), 0, 0)
	#define CAL_CMUtlStrToLower  pfCMUtlStrToLower
	#define CHK_CMUtlStrToLower  (pfCMUtlStrToLower != NULL)
	#define EXP_CMUtlStrToLower  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrToLower", (RTS_UINTPTR)CMUtlStrToLower, 0, 0) 
#endif




/**
 * <description> 
 *  Convert a string in upper case. String must be NUL terminated!
 * </description>
 * <param name="psz" type="IN">Pointer to string to convert</param>
 * <result>Pointer to psz with the converted content</result>
 */
char* CDECL CMUtlStrToUpper(char* psz);
typedef char* (CDECL * PFCMUTLSTRTOUPPER) (char* psz);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRTOUPPER_NOTIMPLEMENTED)
	#define USE_CMUtlStrToUpper
	#define EXT_CMUtlStrToUpper
	#define GET_CMUtlStrToUpper(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrToUpper(p0)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrToUpper  FALSE
	#define EXP_CMUtlStrToUpper  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrToUpper
	#define EXT_CMUtlStrToUpper
	#define GET_CMUtlStrToUpper(fl)  CAL_CMGETAPI( "CMUtlStrToUpper" ) 
	#define CAL_CMUtlStrToUpper  CMUtlStrToUpper
	#define CHK_CMUtlStrToUpper  TRUE
	#define EXP_CMUtlStrToUpper  CAL_CMEXPAPI( "CMUtlStrToUpper" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrToUpper
	#define EXT_CMUtlStrToUpper
	#define GET_CMUtlStrToUpper(fl)  CAL_CMGETAPI( "CMUtlStrToUpper" ) 
	#define CAL_CMUtlStrToUpper  CMUtlStrToUpper
	#define CHK_CMUtlStrToUpper  TRUE
	#define EXP_CMUtlStrToUpper  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrToUpper", (RTS_UINTPTR)CMUtlStrToUpper, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrToUpper
	#define EXT_CMUtilsCMUtlStrToUpper
	#define GET_CMUtilsCMUtlStrToUpper  ERR_OK
	#define CAL_CMUtilsCMUtlStrToUpper pICMUtils->ICMUtlStrToUpper
	#define CHK_CMUtilsCMUtlStrToUpper (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrToUpper  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrToUpper
	#define EXT_CMUtlStrToUpper
	#define GET_CMUtlStrToUpper(fl)  CAL_CMGETAPI( "CMUtlStrToUpper" ) 
	#define CAL_CMUtlStrToUpper pICMUtils->ICMUtlStrToUpper
	#define CHK_CMUtlStrToUpper (pICMUtils != NULL)
	#define EXP_CMUtlStrToUpper  CAL_CMEXPAPI( "CMUtlStrToUpper" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrToUpper  PFCMUTLSTRTOUPPER pfCMUtlStrToUpper;
	#define EXT_CMUtlStrToUpper  extern PFCMUTLSTRTOUPPER pfCMUtlStrToUpper;
	#define GET_CMUtlStrToUpper(fl)  s_pfCMGetAPI2( "CMUtlStrToUpper", (RTS_VOID_FCTPTR *)&pfCMUtlStrToUpper, (fl), 0, 0)
	#define CAL_CMUtlStrToUpper  pfCMUtlStrToUpper
	#define CHK_CMUtlStrToUpper  (pfCMUtlStrToUpper != NULL)
	#define EXP_CMUtlStrToUpper  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrToUpper", (RTS_UINTPTR)CMUtlStrToUpper, 0, 0) 
#endif




/**
 * <description> 
 *  Safe sprintf with argument list.
 *	NOTES:
 *	- If ANSI vsnprintf is not available on the target, vsprintf is used instead and the size of the string can be evaluated only after
 *		writing the complete string! So the memory could be overwritten in this case and so the error code is the only way to detect this situation!
 *	- RTS_ASSERT() is generated, if string buffer is too short!
 * </description>
 * <param name="pszStr" type="IN">Destination buffer</param>
 * <param name="ulStrSize" type="IN">Destination buffer size</param>
 * <param name="pszFormat" type="IN">Format string</param>
 * <param name="pargList" type="IN">Pointer to argument list</param>
 * <result>Error code:
 *	<ul>
 *     <li>ERR_OK: String could be written</li>
 *     <li>ERR_PARAMETER: NULL pointer used as parameter</li>
 *     <li>ERR_BUFFERSIZE: If pszStr is too short to get the string value</li>
 *  </ul>
 * </result>
 */
RTS_RESULT CDECL CMUtlvsnprintf(char *pszStr, RTS_SIZE ulStrSize, const char *pszFormat, va_list *pargList);
typedef RTS_RESULT (CDECL * PFCMUTLVSNPRINTF) (char *pszStr, RTS_SIZE ulStrSize, const char *pszFormat, va_list *pargList);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLVSNPRINTF_NOTIMPLEMENTED)
	#define USE_CMUtlvsnprintf
	#define EXT_CMUtlvsnprintf
	#define GET_CMUtlvsnprintf(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlvsnprintf(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlvsnprintf  FALSE
	#define EXP_CMUtlvsnprintf  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlvsnprintf
	#define EXT_CMUtlvsnprintf
	#define GET_CMUtlvsnprintf(fl)  CAL_CMGETAPI( "CMUtlvsnprintf" ) 
	#define CAL_CMUtlvsnprintf  CMUtlvsnprintf
	#define CHK_CMUtlvsnprintf  TRUE
	#define EXP_CMUtlvsnprintf  CAL_CMEXPAPI( "CMUtlvsnprintf" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlvsnprintf
	#define EXT_CMUtlvsnprintf
	#define GET_CMUtlvsnprintf(fl)  CAL_CMGETAPI( "CMUtlvsnprintf" ) 
	#define CAL_CMUtlvsnprintf  CMUtlvsnprintf
	#define CHK_CMUtlvsnprintf  TRUE
	#define EXP_CMUtlvsnprintf  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlvsnprintf", (RTS_UINTPTR)CMUtlvsnprintf, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlvsnprintf
	#define EXT_CMUtilsCMUtlvsnprintf
	#define GET_CMUtilsCMUtlvsnprintf  ERR_OK
	#define CAL_CMUtilsCMUtlvsnprintf pICMUtils->ICMUtlvsnprintf
	#define CHK_CMUtilsCMUtlvsnprintf (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlvsnprintf  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlvsnprintf
	#define EXT_CMUtlvsnprintf
	#define GET_CMUtlvsnprintf(fl)  CAL_CMGETAPI( "CMUtlvsnprintf" ) 
	#define CAL_CMUtlvsnprintf pICMUtils->ICMUtlvsnprintf
	#define CHK_CMUtlvsnprintf (pICMUtils != NULL)
	#define EXP_CMUtlvsnprintf  CAL_CMEXPAPI( "CMUtlvsnprintf" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlvsnprintf  PFCMUTLVSNPRINTF pfCMUtlvsnprintf;
	#define EXT_CMUtlvsnprintf  extern PFCMUTLVSNPRINTF pfCMUtlvsnprintf;
	#define GET_CMUtlvsnprintf(fl)  s_pfCMGetAPI2( "CMUtlvsnprintf", (RTS_VOID_FCTPTR *)&pfCMUtlvsnprintf, (fl), 0, 0)
	#define CAL_CMUtlvsnprintf  pfCMUtlvsnprintf
	#define CHK_CMUtlvsnprintf  (pfCMUtlvsnprintf != NULL)
	#define EXP_CMUtlvsnprintf  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlvsnprintf", (RTS_UINTPTR)CMUtlvsnprintf, 0, 0) 
#endif




/**
 * <description> 
 *  Safe sprintf with format string and variable argument list.
 *	NOTES:
 *	- If ANSI vsnprintf is not available on the target, vsprintf is used instead and the size of the string can be evaluated only after
 *		writing the complete string! So the memory could be overwritten in this case and so the error code is the only way to detect this situation!
 *	- RTS_ASSERT() is generated, if string buffer is too short!
 * </description>
 * <param name="pszStr" type="IN">Destination buffer</param>
 * <param name="ulStrSize" type="IN">Destination buffer size</param>
 * <param name="pszFormat" type="IN">Format string</param>
 * <param name="..." type="IN">Argument list</param>
 * <result>Error code:
 *	<ul>
 *     <li>ERR_OK: String could be written</li>
 *     <li>ERR_PARAMETER: NULL pointer used as parameter</li> 
 *     <li>ERR_BUFFERSIZE: If pszStr is too short to get the string value</li>
 *  </ul>
 * </result>
 */
RTS_RESULT CDECL CMUtlsnprintf(char *pszStr, RTS_SIZE ulStrSize, const char *pszFormat, ...);
typedef RTS_RESULT (CDECL * PFCMUTLSNPRINTF) (char *pszStr, RTS_SIZE ulStrSize, const char *pszFormat, ...);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSNPRINTF_NOTIMPLEMENTED)
	#define USE_CMUtlsnprintf
	#define EXT_CMUtlsnprintf
	#define GET_CMUtlsnprintf(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlsnprintf  FUNCTION_NOTIMPLEMENTED2 
	#define CHK_CMUtlsnprintf  FALSE
	#define EXP_CMUtlsnprintf  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlsnprintf
	#define EXT_CMUtlsnprintf
	#define GET_CMUtlsnprintf(fl)  CAL_CMGETAPI( "CMUtlsnprintf" ) 
	#define CAL_CMUtlsnprintf  CMUtlsnprintf
	#define CHK_CMUtlsnprintf  TRUE
	#define EXP_CMUtlsnprintf  CAL_CMEXPAPI( "CMUtlsnprintf" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlsnprintf
	#define EXT_CMUtlsnprintf
	#define GET_CMUtlsnprintf(fl)  CAL_CMGETAPI( "CMUtlsnprintf" ) 
	#define CAL_CMUtlsnprintf  CMUtlsnprintf
	#define CHK_CMUtlsnprintf  TRUE
	#define EXP_CMUtlsnprintf  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlsnprintf", (RTS_UINTPTR)CMUtlsnprintf, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlsnprintf
	#define EXT_CMUtilsCMUtlsnprintf
	#define GET_CMUtilsCMUtlsnprintf  ERR_OK
	#define CAL_CMUtilsCMUtlsnprintf pICMUtils->ICMUtlsnprintf
	#define CHK_CMUtilsCMUtlsnprintf (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlsnprintf  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlsnprintf
	#define EXT_CMUtlsnprintf
	#define GET_CMUtlsnprintf(fl)  CAL_CMGETAPI( "CMUtlsnprintf" ) 
	#define CAL_CMUtlsnprintf pICMUtils->ICMUtlsnprintf
	#define CHK_CMUtlsnprintf (pICMUtils != NULL)
	#define EXP_CMUtlsnprintf  CAL_CMEXPAPI( "CMUtlsnprintf" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlsnprintf  PFCMUTLSNPRINTF pfCMUtlsnprintf;
	#define EXT_CMUtlsnprintf  extern PFCMUTLSNPRINTF pfCMUtlsnprintf;
	#define GET_CMUtlsnprintf(fl)  s_pfCMGetAPI2( "CMUtlsnprintf", (RTS_VOID_FCTPTR *)&pfCMUtlsnprintf, (fl), 0, 0)
	#define CAL_CMUtlsnprintf  pfCMUtlsnprintf
	#define CHK_CMUtlsnprintf  (pfCMUtlsnprintf != NULL)
	#define EXP_CMUtlsnprintf  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlsnprintf", (RTS_UINTPTR)CMUtlsnprintf, 0, 0) 
#endif




/**
 * <description> 
 *  Memory copy between two buffers. 
 *  NOTES:
 *  - Caller has to take care about parameter checks. E.g. function does not check for null-pointer or overlapping.
 *  - Function behaves exactly like memcpy from standard c-library
 *  - For some platforms (e.g. ARM), the native memcpy might be problematic when accessing external memory. In these cases this implementation can be used
 * </description>
 * <param name="pDest" type="IN">Pointer to destination buffer</param>
 * <param name="pSrc" type="IN">Pointer to source buffer</param>
 * <param name="ulToCopy" type="IN">Number of chars to copy. NOTE: The char of the TI C28x is 16 bits.</param>
 * <result>Pointer to destination buffer</result>
 */
void* CDECL CMUtlMemCpy(void *pDest, const void *pSrc, RTS_SIZE ulToCopy);
typedef void* (CDECL * PFCMUTLMEMCPY) (void *pDest, const void *pSrc, RTS_SIZE ulToCopy);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLMEMCPY_NOTIMPLEMENTED)
	#define USE_CMUtlMemCpy
	#define EXT_CMUtlMemCpy
	#define GET_CMUtlMemCpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlMemCpy(p0,p1,p2)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlMemCpy  FALSE
	#define EXP_CMUtlMemCpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlMemCpy
	#define EXT_CMUtlMemCpy
	#define GET_CMUtlMemCpy(fl)  CAL_CMGETAPI( "CMUtlMemCpy" ) 
	#define CAL_CMUtlMemCpy  CMUtlMemCpy
	#define CHK_CMUtlMemCpy  TRUE
	#define EXP_CMUtlMemCpy  CAL_CMEXPAPI( "CMUtlMemCpy" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlMemCpy
	#define EXT_CMUtlMemCpy
	#define GET_CMUtlMemCpy(fl)  CAL_CMGETAPI( "CMUtlMemCpy" ) 
	#define CAL_CMUtlMemCpy  CMUtlMemCpy
	#define CHK_CMUtlMemCpy  TRUE
	#define EXP_CMUtlMemCpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlMemCpy", (RTS_UINTPTR)CMUtlMemCpy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlMemCpy
	#define EXT_CMUtilsCMUtlMemCpy
	#define GET_CMUtilsCMUtlMemCpy  ERR_OK
	#define CAL_CMUtilsCMUtlMemCpy pICMUtils->ICMUtlMemCpy
	#define CHK_CMUtilsCMUtlMemCpy (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlMemCpy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlMemCpy
	#define EXT_CMUtlMemCpy
	#define GET_CMUtlMemCpy(fl)  CAL_CMGETAPI( "CMUtlMemCpy" ) 
	#define CAL_CMUtlMemCpy pICMUtils->ICMUtlMemCpy
	#define CHK_CMUtlMemCpy (pICMUtils != NULL)
	#define EXP_CMUtlMemCpy  CAL_CMEXPAPI( "CMUtlMemCpy" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlMemCpy  PFCMUTLMEMCPY pfCMUtlMemCpy;
	#define EXT_CMUtlMemCpy  extern PFCMUTLMEMCPY pfCMUtlMemCpy;
	#define GET_CMUtlMemCpy(fl)  s_pfCMGetAPI2( "CMUtlMemCpy", (RTS_VOID_FCTPTR *)&pfCMUtlMemCpy, (fl), 0, 0)
	#define CAL_CMUtlMemCpy  pfCMUtlMemCpy
	#define CHK_CMUtlMemCpy  (pfCMUtlMemCpy != NULL)
	#define EXP_CMUtlMemCpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlMemCpy", (RTS_UINTPTR)CMUtlMemCpy, 0, 0) 
#endif




/**
 * <description> 
 *  Safe memory copy between two buffers. The length of the destination buffer is checked to avoid memory overwrites.
 * </description>
 * <param name="pDest" type="IN">Pointer to destination buffer</param>
 * <param name="ulDestSize" type="IN">Length of destination buffer</param>
 * <param name="pSrc" type="IN">Pointer to source buffer</param>
 * <param name="ulToCopy" type="IN">Bytes to copy from source to destination buffer</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlSafeMemCpy(void *pDest, RTS_SIZE ulDestSize, const void *pSrc, RTS_SIZE ulToCopy);
typedef RTS_RESULT (CDECL * PFCMUTLSAFEMEMCPY) (void *pDest, RTS_SIZE ulDestSize, const void *pSrc, RTS_SIZE ulToCopy);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSAFEMEMCPY_NOTIMPLEMENTED)
	#define USE_CMUtlSafeMemCpy
	#define EXT_CMUtlSafeMemCpy
	#define GET_CMUtlSafeMemCpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlSafeMemCpy(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlSafeMemCpy  FALSE
	#define EXP_CMUtlSafeMemCpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlSafeMemCpy
	#define EXT_CMUtlSafeMemCpy
	#define GET_CMUtlSafeMemCpy(fl)  CAL_CMGETAPI( "CMUtlSafeMemCpy" ) 
	#define CAL_CMUtlSafeMemCpy  CMUtlSafeMemCpy
	#define CHK_CMUtlSafeMemCpy  TRUE
	#define EXP_CMUtlSafeMemCpy  CAL_CMEXPAPI( "CMUtlSafeMemCpy" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlSafeMemCpy
	#define EXT_CMUtlSafeMemCpy
	#define GET_CMUtlSafeMemCpy(fl)  CAL_CMGETAPI( "CMUtlSafeMemCpy" ) 
	#define CAL_CMUtlSafeMemCpy  CMUtlSafeMemCpy
	#define CHK_CMUtlSafeMemCpy  TRUE
	#define EXP_CMUtlSafeMemCpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeMemCpy", (RTS_UINTPTR)CMUtlSafeMemCpy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlSafeMemCpy
	#define EXT_CMUtilsCMUtlSafeMemCpy
	#define GET_CMUtilsCMUtlSafeMemCpy  ERR_OK
	#define CAL_CMUtilsCMUtlSafeMemCpy pICMUtils->ICMUtlSafeMemCpy
	#define CHK_CMUtilsCMUtlSafeMemCpy (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlSafeMemCpy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlSafeMemCpy
	#define EXT_CMUtlSafeMemCpy
	#define GET_CMUtlSafeMemCpy(fl)  CAL_CMGETAPI( "CMUtlSafeMemCpy" ) 
	#define CAL_CMUtlSafeMemCpy pICMUtils->ICMUtlSafeMemCpy
	#define CHK_CMUtlSafeMemCpy (pICMUtils != NULL)
	#define EXP_CMUtlSafeMemCpy  CAL_CMEXPAPI( "CMUtlSafeMemCpy" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlSafeMemCpy  PFCMUTLSAFEMEMCPY pfCMUtlSafeMemCpy;
	#define EXT_CMUtlSafeMemCpy  extern PFCMUTLSAFEMEMCPY pfCMUtlSafeMemCpy;
	#define GET_CMUtlSafeMemCpy(fl)  s_pfCMGetAPI2( "CMUtlSafeMemCpy", (RTS_VOID_FCTPTR *)&pfCMUtlSafeMemCpy, (fl), 0, 0)
	#define CAL_CMUtlSafeMemCpy  pfCMUtlSafeMemCpy
	#define CHK_CMUtlSafeMemCpy  (pfCMUtlSafeMemCpy != NULL)
	#define EXP_CMUtlSafeMemCpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeMemCpy", (RTS_UINTPTR)CMUtlSafeMemCpy, 0, 0) 
#endif




/**
 * <description> 
 *  Safe memory move between two buffers. The length of the destination buffer is checked to avoid memory overwrites.
 * </description>
 * <param name="pDest" type="IN">Pointer to destination buffer</param>
 * <param name="ulDestSize" type="IN">Length of destination buffer</param>
 * <param name="pSrc" type="IN">Pointer to source buffer</param>
 * <param name="ulToCopy" type="IN">Bytes to copy from source to destination buffer</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlSafeMemMove(void *pDest, RTS_SIZE ulDestSize, const void *pSrc, RTS_SIZE ulToCopy);
typedef RTS_RESULT (CDECL * PFCMUTLSAFEMEMMOVE) (void *pDest, RTS_SIZE ulDestSize, const void *pSrc, RTS_SIZE ulToCopy);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSAFEMEMMOVE_NOTIMPLEMENTED)
	#define USE_CMUtlSafeMemMove
	#define EXT_CMUtlSafeMemMove
	#define GET_CMUtlSafeMemMove(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlSafeMemMove(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlSafeMemMove  FALSE
	#define EXP_CMUtlSafeMemMove  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlSafeMemMove
	#define EXT_CMUtlSafeMemMove
	#define GET_CMUtlSafeMemMove(fl)  CAL_CMGETAPI( "CMUtlSafeMemMove" ) 
	#define CAL_CMUtlSafeMemMove  CMUtlSafeMemMove
	#define CHK_CMUtlSafeMemMove  TRUE
	#define EXP_CMUtlSafeMemMove  CAL_CMEXPAPI( "CMUtlSafeMemMove" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlSafeMemMove
	#define EXT_CMUtlSafeMemMove
	#define GET_CMUtlSafeMemMove(fl)  CAL_CMGETAPI( "CMUtlSafeMemMove" ) 
	#define CAL_CMUtlSafeMemMove  CMUtlSafeMemMove
	#define CHK_CMUtlSafeMemMove  TRUE
	#define EXP_CMUtlSafeMemMove  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeMemMove", (RTS_UINTPTR)CMUtlSafeMemMove, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlSafeMemMove
	#define EXT_CMUtilsCMUtlSafeMemMove
	#define GET_CMUtilsCMUtlSafeMemMove  ERR_OK
	#define CAL_CMUtilsCMUtlSafeMemMove pICMUtils->ICMUtlSafeMemMove
	#define CHK_CMUtilsCMUtlSafeMemMove (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlSafeMemMove  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlSafeMemMove
	#define EXT_CMUtlSafeMemMove
	#define GET_CMUtlSafeMemMove(fl)  CAL_CMGETAPI( "CMUtlSafeMemMove" ) 
	#define CAL_CMUtlSafeMemMove pICMUtils->ICMUtlSafeMemMove
	#define CHK_CMUtlSafeMemMove (pICMUtils != NULL)
	#define EXP_CMUtlSafeMemMove  CAL_CMEXPAPI( "CMUtlSafeMemMove" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlSafeMemMove  PFCMUTLSAFEMEMMOVE pfCMUtlSafeMemMove;
	#define EXT_CMUtlSafeMemMove  extern PFCMUTLSAFEMEMMOVE pfCMUtlSafeMemMove;
	#define GET_CMUtlSafeMemMove(fl)  s_pfCMGetAPI2( "CMUtlSafeMemMove", (RTS_VOID_FCTPTR *)&pfCMUtlSafeMemMove, (fl), 0, 0)
	#define CAL_CMUtlSafeMemMove  pfCMUtlSafeMemMove
	#define CHK_CMUtlSafeMemMove  (pfCMUtlSafeMemMove != NULL)
	#define EXP_CMUtlSafeMemMove  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlSafeMemMove", (RTS_UINTPTR)CMUtlSafeMemMove, 0, 0) 
#endif




/**
 * <description> 
 * Maximum legth of the GUID string
 * </description>
 */
#define MAX_GUID_STRING_LEN		37

/**
 * <description> 
 *  Conversion of a GUID into a string of the format f81d4fae-7dec-11d0-a765-00a0c91e6bf6.  
 * </description>
 * <param name="pguid" type="IN">Pointer to the GUID to convert</param>
 * <param name="pszGUID" type="OUT">Pointer to string buffer for the result</param>
 * <param name="nMaxLen" type="IN">Maximum length of the string buffer. Should be at least MAX_GUID_STRING_LEN</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Conversion was successful</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Pointer pszGUID or pguid is not valid or nMaxLen is to small</errorcode>
 */
RTS_RESULT CDECL CMUtlGUIDToString(const RTS_GUID *pguid, char *pszGUID, RTS_SIZE nMaxLen);
typedef RTS_RESULT (CDECL * PFCMUTLGUIDTOSTRING) (const RTS_GUID *pguid, char *pszGUID, RTS_SIZE nMaxLen);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLGUIDTOSTRING_NOTIMPLEMENTED)
	#define USE_CMUtlGUIDToString
	#define EXT_CMUtlGUIDToString
	#define GET_CMUtlGUIDToString(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlGUIDToString(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlGUIDToString  FALSE
	#define EXP_CMUtlGUIDToString  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlGUIDToString
	#define EXT_CMUtlGUIDToString
	#define GET_CMUtlGUIDToString(fl)  CAL_CMGETAPI( "CMUtlGUIDToString" ) 
	#define CAL_CMUtlGUIDToString  CMUtlGUIDToString
	#define CHK_CMUtlGUIDToString  TRUE
	#define EXP_CMUtlGUIDToString  CAL_CMEXPAPI( "CMUtlGUIDToString" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlGUIDToString
	#define EXT_CMUtlGUIDToString
	#define GET_CMUtlGUIDToString(fl)  CAL_CMGETAPI( "CMUtlGUIDToString" ) 
	#define CAL_CMUtlGUIDToString  CMUtlGUIDToString
	#define CHK_CMUtlGUIDToString  TRUE
	#define EXP_CMUtlGUIDToString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlGUIDToString", (RTS_UINTPTR)CMUtlGUIDToString, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlGUIDToString
	#define EXT_CMUtilsCMUtlGUIDToString
	#define GET_CMUtilsCMUtlGUIDToString  ERR_OK
	#define CAL_CMUtilsCMUtlGUIDToString pICMUtils->ICMUtlGUIDToString
	#define CHK_CMUtilsCMUtlGUIDToString (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlGUIDToString  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlGUIDToString
	#define EXT_CMUtlGUIDToString
	#define GET_CMUtlGUIDToString(fl)  CAL_CMGETAPI( "CMUtlGUIDToString" ) 
	#define CAL_CMUtlGUIDToString pICMUtils->ICMUtlGUIDToString
	#define CHK_CMUtlGUIDToString (pICMUtils != NULL)
	#define EXP_CMUtlGUIDToString  CAL_CMEXPAPI( "CMUtlGUIDToString" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlGUIDToString  PFCMUTLGUIDTOSTRING pfCMUtlGUIDToString;
	#define EXT_CMUtlGUIDToString  extern PFCMUTLGUIDTOSTRING pfCMUtlGUIDToString;
	#define GET_CMUtlGUIDToString(fl)  s_pfCMGetAPI2( "CMUtlGUIDToString", (RTS_VOID_FCTPTR *)&pfCMUtlGUIDToString, (fl), 0, 0)
	#define CAL_CMUtlGUIDToString  pfCMUtlGUIDToString
	#define CHK_CMUtlGUIDToString  (pfCMUtlGUIDToString != NULL)
	#define EXP_CMUtlGUIDToString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlGUIDToString", (RTS_UINTPTR)CMUtlGUIDToString, 0, 0) 
#endif




/**
 * <description>
 *  Conversion of a string into a GUID. Handles the formats
 *   - Platform independent GUID: f81d4fae-7dec-11d0-a765-00a0c91e6bf6 
 *   - Alternative platform dependent legacy formats of older runtime system versions: 
 *       - format previously generated by CMUtlGUIDToString(): {f81d4fae-7dec11d0-a76500a0-c91e6bf6}
 *       - format previously generated by CmpTraceMgr: f81d4fae-11d07dec-a00065a7-f66b1ec9
 *   - Handles GUIDs at start of a string, i.e. characters after the GUID are ignored. 
 * </description>
 * <param name="pszGUID" type="IN">Pointer to string to convert</param>
 * <param name="pguid" type="OUT">Pointer to the GUID for the result</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Conversion was successful</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Pointer pszGUID or pguid is not valid</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_SIZE_MISMATCH">Conversion failed with size mismatch, most likely pszGUID does not contain a valid GUID format</errorcode>
*/
RTS_RESULT CDECL CMUtlStringToGUID(const char *pszGUID, RTS_GUID *pguid);
typedef RTS_RESULT (CDECL * PFCMUTLSTRINGTOGUID) (const char *pszGUID, RTS_GUID *pguid);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRINGTOGUID_NOTIMPLEMENTED)
	#define USE_CMUtlStringToGUID
	#define EXT_CMUtlStringToGUID
	#define GET_CMUtlStringToGUID(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStringToGUID(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStringToGUID  FALSE
	#define EXP_CMUtlStringToGUID  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStringToGUID
	#define EXT_CMUtlStringToGUID
	#define GET_CMUtlStringToGUID(fl)  CAL_CMGETAPI( "CMUtlStringToGUID" ) 
	#define CAL_CMUtlStringToGUID  CMUtlStringToGUID
	#define CHK_CMUtlStringToGUID  TRUE
	#define EXP_CMUtlStringToGUID  CAL_CMEXPAPI( "CMUtlStringToGUID" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStringToGUID
	#define EXT_CMUtlStringToGUID
	#define GET_CMUtlStringToGUID(fl)  CAL_CMGETAPI( "CMUtlStringToGUID" ) 
	#define CAL_CMUtlStringToGUID  CMUtlStringToGUID
	#define CHK_CMUtlStringToGUID  TRUE
	#define EXP_CMUtlStringToGUID  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStringToGUID", (RTS_UINTPTR)CMUtlStringToGUID, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStringToGUID
	#define EXT_CMUtilsCMUtlStringToGUID
	#define GET_CMUtilsCMUtlStringToGUID  ERR_OK
	#define CAL_CMUtilsCMUtlStringToGUID pICMUtils->ICMUtlStringToGUID
	#define CHK_CMUtilsCMUtlStringToGUID (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStringToGUID  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStringToGUID
	#define EXT_CMUtlStringToGUID
	#define GET_CMUtlStringToGUID(fl)  CAL_CMGETAPI( "CMUtlStringToGUID" ) 
	#define CAL_CMUtlStringToGUID pICMUtils->ICMUtlStringToGUID
	#define CHK_CMUtlStringToGUID (pICMUtils != NULL)
	#define EXP_CMUtlStringToGUID  CAL_CMEXPAPI( "CMUtlStringToGUID" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStringToGUID  PFCMUTLSTRINGTOGUID pfCMUtlStringToGUID;
	#define EXT_CMUtlStringToGUID  extern PFCMUTLSTRINGTOGUID pfCMUtlStringToGUID;
	#define GET_CMUtlStringToGUID(fl)  s_pfCMGetAPI2( "CMUtlStringToGUID", (RTS_VOID_FCTPTR *)&pfCMUtlStringToGUID, (fl), 0, 0)
	#define CAL_CMUtlStringToGUID  pfCMUtlStringToGUID
	#define CHK_CMUtlStringToGUID  (pfCMUtlStringToGUID != NULL)
	#define EXP_CMUtlStringToGUID  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStringToGUID", (RTS_UINTPTR)CMUtlStringToGUID, 0, 0) 
#endif





/**
 ***************************************************************************
 * <description> 
 *  String class routines
 * </description>
 ***************************************************************************
 */
/**
 * <category>ComponentNameFlags</category>
 * <description>
 *	<ul>
 *		<li>RTS_STRING_STATIC: Name is stored in a static buffer</li>
 *		<li>RTS_STRING_DYNAMIC: Name is stored in a dynamic buffer</li>
 *		<li>RTS_STRING_ASCII: ASCII string content</li>
 *	</ul>
 * </description>
 */
#define RTS_STRING_STATIC		0x00000001
#define RTS_STRING_DYNAMIC		0x00000002
#define RTS_STRING_ASCII		0x00010000

/**
 * <category>RTS_STRING_CLASS</category>
 * <description>
 *	Can be used to manage strings.
 * </description>
 */
typedef union RTS_STRING_ENTRY_
{
	char *psz;
	char sz[RTS_STATIC_STRING_LEN];
} RTS_STRING_ENTRY;

typedef struct RTS_STRING_CLASS_
{
	RTS_STRING_ENTRY name;
	RTS_SIZE size;
	char *pszComponentName;
	RTS_UI32 ulFlags;
	RTS_UI32 _alignment;
} RTS_STRING_CLASS;

/**
 * <description> 
 *  Macro to get the string content out of a string class.
 * </description>
 * <param name="pString" type="IN">Pointer to string class</param>
 * <param name="default" type="IN">Default string, if string content could not be returned</param>
 * <result>Pointer to content</result>
 */
#define CM_PSTRING_GET_CONTENT(pString, default)		(pString == NULL ? default : ( (((RTS_STRING_CLASS *)pString)->ulFlags & RTS_STRING_DYNAMIC) ? ((RTS_STRING_CLASS *)pString)->name.psz : ((RTS_STRING_CLASS *)pString)->name.sz) )
#define CM_STRING_GET_CONTENT(string)					CM_PSTRING_GET_CONTENT(&string, (RTS_CSTRING*)"")

/**
 * <description> 
 *  Macro to get the size in bytes of the content of a string class.
 * </description>
 * <param name="pString" type="IN">Pointer to string class</param>
 * <result>Size in bytes of the string content</result>
 */
#define CM_PSTRING_GET_SIZE(pString)					(pString == NULL ? 0 : ((RTS_STRING_CLASS *)pString)->size)
#define CM_STRING_GET_SIZE(string)						CM_PSTRING_GET_SIZE(&string)

/**
 * <description> 
 *  Create a string and initialize it with a default string.
 * </description>
 * <param name="pString" type="IN">Pointer to string class</param>
 * <param name="pszComponentName" type="IN">Pointer to the component name, which creates this string</param>
 * <param name="pszInit" type="IN">Pointer to initialization string to copy into content. Can be NULL.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMStringCreate(RTS_STRING_CLASS *pString, char *pszComponentName, char *pszInit);
typedef RTS_RESULT (CDECL * PFCMSTRINGCREATE) (RTS_STRING_CLASS *pString, char *pszComponentName, char *pszInit);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMSTRINGCREATE_NOTIMPLEMENTED)
	#define USE_CMStringCreate
	#define EXT_CMStringCreate
	#define GET_CMStringCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMStringCreate(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMStringCreate  FALSE
	#define EXP_CMStringCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMStringCreate
	#define EXT_CMStringCreate
	#define GET_CMStringCreate(fl)  CAL_CMGETAPI( "CMStringCreate" ) 
	#define CAL_CMStringCreate  CMStringCreate
	#define CHK_CMStringCreate  TRUE
	#define EXP_CMStringCreate  CAL_CMEXPAPI( "CMStringCreate" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMStringCreate
	#define EXT_CMStringCreate
	#define GET_CMStringCreate(fl)  CAL_CMGETAPI( "CMStringCreate" ) 
	#define CAL_CMStringCreate  CMStringCreate
	#define CHK_CMStringCreate  TRUE
	#define EXP_CMStringCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCreate", (RTS_UINTPTR)CMStringCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMStringCreate
	#define EXT_CMUtilsCMStringCreate
	#define GET_CMUtilsCMStringCreate  ERR_OK
	#define CAL_CMUtilsCMStringCreate pICMUtils->ICMStringCreate
	#define CHK_CMUtilsCMStringCreate (pICMUtils != NULL)
	#define EXP_CMUtilsCMStringCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMStringCreate
	#define EXT_CMStringCreate
	#define GET_CMStringCreate(fl)  CAL_CMGETAPI( "CMStringCreate" ) 
	#define CAL_CMStringCreate pICMUtils->ICMStringCreate
	#define CHK_CMStringCreate (pICMUtils != NULL)
	#define EXP_CMStringCreate  CAL_CMEXPAPI( "CMStringCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMStringCreate  PFCMSTRINGCREATE pfCMStringCreate;
	#define EXT_CMStringCreate  extern PFCMSTRINGCREATE pfCMStringCreate;
	#define GET_CMStringCreate(fl)  s_pfCMGetAPI2( "CMStringCreate", (RTS_VOID_FCTPTR *)&pfCMStringCreate, (fl), 0, 0)
	#define CAL_CMStringCreate  pfCMStringCreate
	#define CHK_CMStringCreate  (pfCMStringCreate != NULL)
	#define EXP_CMStringCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCreate", (RTS_UINTPTR)CMStringCreate, 0, 0) 
#endif



RTS_RESULT CDECL CMStringCreate2(RTS_STRING_CLASS *pString, char *pszComponentName, char *pszInit, RTS_BOOL bAllocDynamic);
typedef RTS_RESULT (CDECL * PFCMSTRINGCREATE2) (RTS_STRING_CLASS *pString, char *pszComponentName, char *pszInit, RTS_BOOL bAllocDynamic);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMSTRINGCREATE2_NOTIMPLEMENTED)
	#define USE_CMStringCreate2
	#define EXT_CMStringCreate2
	#define GET_CMStringCreate2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMStringCreate2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMStringCreate2  FALSE
	#define EXP_CMStringCreate2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMStringCreate2
	#define EXT_CMStringCreate2
	#define GET_CMStringCreate2(fl)  CAL_CMGETAPI( "CMStringCreate2" ) 
	#define CAL_CMStringCreate2  CMStringCreate2
	#define CHK_CMStringCreate2  TRUE
	#define EXP_CMStringCreate2  CAL_CMEXPAPI( "CMStringCreate2" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMStringCreate2
	#define EXT_CMStringCreate2
	#define GET_CMStringCreate2(fl)  CAL_CMGETAPI( "CMStringCreate2" ) 
	#define CAL_CMStringCreate2  CMStringCreate2
	#define CHK_CMStringCreate2  TRUE
	#define EXP_CMStringCreate2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCreate2", (RTS_UINTPTR)CMStringCreate2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMStringCreate2
	#define EXT_CMUtilsCMStringCreate2
	#define GET_CMUtilsCMStringCreate2  ERR_OK
	#define CAL_CMUtilsCMStringCreate2 pICMUtils->ICMStringCreate2
	#define CHK_CMUtilsCMStringCreate2 (pICMUtils != NULL)
	#define EXP_CMUtilsCMStringCreate2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMStringCreate2
	#define EXT_CMStringCreate2
	#define GET_CMStringCreate2(fl)  CAL_CMGETAPI( "CMStringCreate2" ) 
	#define CAL_CMStringCreate2 pICMUtils->ICMStringCreate2
	#define CHK_CMStringCreate2 (pICMUtils != NULL)
	#define EXP_CMStringCreate2  CAL_CMEXPAPI( "CMStringCreate2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMStringCreate2  PFCMSTRINGCREATE2 pfCMStringCreate2;
	#define EXT_CMStringCreate2  extern PFCMSTRINGCREATE2 pfCMStringCreate2;
	#define GET_CMStringCreate2(fl)  s_pfCMGetAPI2( "CMStringCreate2", (RTS_VOID_FCTPTR *)&pfCMStringCreate2, (fl), 0, 0)
	#define CAL_CMStringCreate2  pfCMStringCreate2
	#define CHK_CMStringCreate2  (pfCMStringCreate2 != NULL)
	#define EXP_CMStringCreate2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCreate2", (RTS_UINTPTR)CMStringCreate2, 0, 0) 
#endif



RTS_RESULT CDECL CMStringCreate3(RTS_STRING_CLASS *pString, char *pszComponentName, RTS_SIZE nMaxSize, RTS_BOOL bAllocDynamic);
typedef RTS_RESULT (CDECL * PFCMSTRINGCREATE3) (RTS_STRING_CLASS *pString, char *pszComponentName, RTS_SIZE nMaxSize, RTS_BOOL bAllocDynamic);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMSTRINGCREATE3_NOTIMPLEMENTED)
	#define USE_CMStringCreate3
	#define EXT_CMStringCreate3
	#define GET_CMStringCreate3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMStringCreate3(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMStringCreate3  FALSE
	#define EXP_CMStringCreate3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMStringCreate3
	#define EXT_CMStringCreate3
	#define GET_CMStringCreate3(fl)  CAL_CMGETAPI( "CMStringCreate3" ) 
	#define CAL_CMStringCreate3  CMStringCreate3
	#define CHK_CMStringCreate3  TRUE
	#define EXP_CMStringCreate3  CAL_CMEXPAPI( "CMStringCreate3" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMStringCreate3
	#define EXT_CMStringCreate3
	#define GET_CMStringCreate3(fl)  CAL_CMGETAPI( "CMStringCreate3" ) 
	#define CAL_CMStringCreate3  CMStringCreate3
	#define CHK_CMStringCreate3  TRUE
	#define EXP_CMStringCreate3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCreate3", (RTS_UINTPTR)CMStringCreate3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMStringCreate3
	#define EXT_CMUtilsCMStringCreate3
	#define GET_CMUtilsCMStringCreate3  ERR_OK
	#define CAL_CMUtilsCMStringCreate3 pICMUtils->ICMStringCreate3
	#define CHK_CMUtilsCMStringCreate3 (pICMUtils != NULL)
	#define EXP_CMUtilsCMStringCreate3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMStringCreate3
	#define EXT_CMStringCreate3
	#define GET_CMStringCreate3(fl)  CAL_CMGETAPI( "CMStringCreate3" ) 
	#define CAL_CMStringCreate3 pICMUtils->ICMStringCreate3
	#define CHK_CMStringCreate3 (pICMUtils != NULL)
	#define EXP_CMStringCreate3  CAL_CMEXPAPI( "CMStringCreate3" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMStringCreate3  PFCMSTRINGCREATE3 pfCMStringCreate3;
	#define EXT_CMStringCreate3  extern PFCMSTRINGCREATE3 pfCMStringCreate3;
	#define GET_CMStringCreate3(fl)  s_pfCMGetAPI2( "CMStringCreate3", (RTS_VOID_FCTPTR *)&pfCMStringCreate3, (fl), 0, 0)
	#define CAL_CMStringCreate3  pfCMStringCreate3
	#define CHK_CMStringCreate3  (pfCMStringCreate3 != NULL)
	#define EXP_CMStringCreate3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCreate3", (RTS_UINTPTR)CMStringCreate3, 0, 0) 
#endif




/**
 * <description> 
 *  Create a string and initialize it with a default string.
 * </description>
 * <param name="pString" type="IN">Pointer to string class</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMStringDelete(RTS_STRING_CLASS *pString);
typedef RTS_RESULT (CDECL * PFCMSTRINGDELETE) (RTS_STRING_CLASS *pString);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMSTRINGDELETE_NOTIMPLEMENTED)
	#define USE_CMStringDelete
	#define EXT_CMStringDelete
	#define GET_CMStringDelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMStringDelete(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMStringDelete  FALSE
	#define EXP_CMStringDelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMStringDelete
	#define EXT_CMStringDelete
	#define GET_CMStringDelete(fl)  CAL_CMGETAPI( "CMStringDelete" ) 
	#define CAL_CMStringDelete  CMStringDelete
	#define CHK_CMStringDelete  TRUE
	#define EXP_CMStringDelete  CAL_CMEXPAPI( "CMStringDelete" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMStringDelete
	#define EXT_CMStringDelete
	#define GET_CMStringDelete(fl)  CAL_CMGETAPI( "CMStringDelete" ) 
	#define CAL_CMStringDelete  CMStringDelete
	#define CHK_CMStringDelete  TRUE
	#define EXP_CMStringDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringDelete", (RTS_UINTPTR)CMStringDelete, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMStringDelete
	#define EXT_CMUtilsCMStringDelete
	#define GET_CMUtilsCMStringDelete  ERR_OK
	#define CAL_CMUtilsCMStringDelete pICMUtils->ICMStringDelete
	#define CHK_CMUtilsCMStringDelete (pICMUtils != NULL)
	#define EXP_CMUtilsCMStringDelete  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMStringDelete
	#define EXT_CMStringDelete
	#define GET_CMStringDelete(fl)  CAL_CMGETAPI( "CMStringDelete" ) 
	#define CAL_CMStringDelete pICMUtils->ICMStringDelete
	#define CHK_CMStringDelete (pICMUtils != NULL)
	#define EXP_CMStringDelete  CAL_CMEXPAPI( "CMStringDelete" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMStringDelete  PFCMSTRINGDELETE pfCMStringDelete;
	#define EXT_CMStringDelete  extern PFCMSTRINGDELETE pfCMStringDelete;
	#define GET_CMStringDelete(fl)  s_pfCMGetAPI2( "CMStringDelete", (RTS_VOID_FCTPTR *)&pfCMStringDelete, (fl), 0, 0)
	#define CAL_CMStringDelete  pfCMStringDelete
	#define CHK_CMStringDelete  (pfCMStringDelete != NULL)
	#define EXP_CMStringDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringDelete", (RTS_UINTPTR)CMStringDelete, 0, 0) 
#endif




/**
 * <description> 
 *  Extend a string to a specified size.
 * </description>
 * <param name="pString" type="IN">Pointer to string class</param>
 * <param name="size" type="IN">Size to which the content must be extended</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMStringExtend(RTS_STRING_CLASS *pString, RTS_SIZE size);
typedef RTS_RESULT (CDECL * PFCMSTRINGEXTEND) (RTS_STRING_CLASS *pString, RTS_SIZE size);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMSTRINGEXTEND_NOTIMPLEMENTED)
	#define USE_CMStringExtend
	#define EXT_CMStringExtend
	#define GET_CMStringExtend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMStringExtend(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMStringExtend  FALSE
	#define EXP_CMStringExtend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMStringExtend
	#define EXT_CMStringExtend
	#define GET_CMStringExtend(fl)  CAL_CMGETAPI( "CMStringExtend" ) 
	#define CAL_CMStringExtend  CMStringExtend
	#define CHK_CMStringExtend  TRUE
	#define EXP_CMStringExtend  CAL_CMEXPAPI( "CMStringExtend" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMStringExtend
	#define EXT_CMStringExtend
	#define GET_CMStringExtend(fl)  CAL_CMGETAPI( "CMStringExtend" ) 
	#define CAL_CMStringExtend  CMStringExtend
	#define CHK_CMStringExtend  TRUE
	#define EXP_CMStringExtend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringExtend", (RTS_UINTPTR)CMStringExtend, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMStringExtend
	#define EXT_CMUtilsCMStringExtend
	#define GET_CMUtilsCMStringExtend  ERR_OK
	#define CAL_CMUtilsCMStringExtend pICMUtils->ICMStringExtend
	#define CHK_CMUtilsCMStringExtend (pICMUtils != NULL)
	#define EXP_CMUtilsCMStringExtend  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMStringExtend
	#define EXT_CMStringExtend
	#define GET_CMStringExtend(fl)  CAL_CMGETAPI( "CMStringExtend" ) 
	#define CAL_CMStringExtend pICMUtils->ICMStringExtend
	#define CHK_CMStringExtend (pICMUtils != NULL)
	#define EXP_CMStringExtend  CAL_CMEXPAPI( "CMStringExtend" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMStringExtend  PFCMSTRINGEXTEND pfCMStringExtend;
	#define EXT_CMStringExtend  extern PFCMSTRINGEXTEND pfCMStringExtend;
	#define GET_CMStringExtend(fl)  s_pfCMGetAPI2( "CMStringExtend", (RTS_VOID_FCTPTR *)&pfCMStringExtend, (fl), 0, 0)
	#define CAL_CMStringExtend  pfCMStringExtend
	#define CHK_CMStringExtend  (pfCMStringExtend != NULL)
	#define EXP_CMStringExtend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringExtend", (RTS_UINTPTR)CMStringExtend, 0, 0) 
#endif




/**
 * <description> 
 *  Copy a string to the string class.
 * </description>
 * <param name="pString" type="IN">Pointer to string class</param>
 * <param name="pszSrc" type="IN">Pointer to the source string buffer</param>
 * <param name="nSrcLen" type="IN">Length of the source string buffer or number of bytes to copy (_with_ trailing NUL)!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMStringCopy(RTS_STRING_CLASS *pString, char *pszSrc, RTS_SIZE nSrcLen);
typedef RTS_RESULT (CDECL * PFCMSTRINGCOPY) (RTS_STRING_CLASS *pString, char *pszSrc, RTS_SIZE nSrcLen);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMSTRINGCOPY_NOTIMPLEMENTED)
	#define USE_CMStringCopy
	#define EXT_CMStringCopy
	#define GET_CMStringCopy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMStringCopy(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMStringCopy  FALSE
	#define EXP_CMStringCopy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMStringCopy
	#define EXT_CMStringCopy
	#define GET_CMStringCopy(fl)  CAL_CMGETAPI( "CMStringCopy" ) 
	#define CAL_CMStringCopy  CMStringCopy
	#define CHK_CMStringCopy  TRUE
	#define EXP_CMStringCopy  CAL_CMEXPAPI( "CMStringCopy" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMStringCopy
	#define EXT_CMStringCopy
	#define GET_CMStringCopy(fl)  CAL_CMGETAPI( "CMStringCopy" ) 
	#define CAL_CMStringCopy  CMStringCopy
	#define CHK_CMStringCopy  TRUE
	#define EXP_CMStringCopy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCopy", (RTS_UINTPTR)CMStringCopy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMStringCopy
	#define EXT_CMUtilsCMStringCopy
	#define GET_CMUtilsCMStringCopy  ERR_OK
	#define CAL_CMUtilsCMStringCopy pICMUtils->ICMStringCopy
	#define CHK_CMUtilsCMStringCopy (pICMUtils != NULL)
	#define EXP_CMUtilsCMStringCopy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMStringCopy
	#define EXT_CMStringCopy
	#define GET_CMStringCopy(fl)  CAL_CMGETAPI( "CMStringCopy" ) 
	#define CAL_CMStringCopy pICMUtils->ICMStringCopy
	#define CHK_CMStringCopy (pICMUtils != NULL)
	#define EXP_CMStringCopy  CAL_CMEXPAPI( "CMStringCopy" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMStringCopy  PFCMSTRINGCOPY pfCMStringCopy;
	#define EXT_CMStringCopy  extern PFCMSTRINGCOPY pfCMStringCopy;
	#define GET_CMStringCopy(fl)  s_pfCMGetAPI2( "CMStringCopy", (RTS_VOID_FCTPTR *)&pfCMStringCopy, (fl), 0, 0)
	#define CAL_CMStringCopy  pfCMStringCopy
	#define CHK_CMStringCopy  (pfCMStringCopy != NULL)
	#define EXP_CMStringCopy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCopy", (RTS_UINTPTR)CMStringCopy, 0, 0) 
#endif




/**
 * <description> 
 *  Copy a string to the string class.
 * </description>
 * <param name="pString" type="IN">Pointer to string class</param>
 * <param name="pszSrc" type="IN">Pointer to the source string buffer</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMStringCopy2(RTS_STRING_CLASS *pString, char *pszSrc);
typedef RTS_RESULT (CDECL * PFCMSTRINGCOPY2) (RTS_STRING_CLASS *pString, char *pszSrc);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMSTRINGCOPY2_NOTIMPLEMENTED)
	#define USE_CMStringCopy2
	#define EXT_CMStringCopy2
	#define GET_CMStringCopy2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMStringCopy2(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMStringCopy2  FALSE
	#define EXP_CMStringCopy2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMStringCopy2
	#define EXT_CMStringCopy2
	#define GET_CMStringCopy2(fl)  CAL_CMGETAPI( "CMStringCopy2" ) 
	#define CAL_CMStringCopy2  CMStringCopy2
	#define CHK_CMStringCopy2  TRUE
	#define EXP_CMStringCopy2  CAL_CMEXPAPI( "CMStringCopy2" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMStringCopy2
	#define EXT_CMStringCopy2
	#define GET_CMStringCopy2(fl)  CAL_CMGETAPI( "CMStringCopy2" ) 
	#define CAL_CMStringCopy2  CMStringCopy2
	#define CHK_CMStringCopy2  TRUE
	#define EXP_CMStringCopy2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCopy2", (RTS_UINTPTR)CMStringCopy2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMStringCopy2
	#define EXT_CMUtilsCMStringCopy2
	#define GET_CMUtilsCMStringCopy2  ERR_OK
	#define CAL_CMUtilsCMStringCopy2 pICMUtils->ICMStringCopy2
	#define CHK_CMUtilsCMStringCopy2 (pICMUtils != NULL)
	#define EXP_CMUtilsCMStringCopy2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMStringCopy2
	#define EXT_CMStringCopy2
	#define GET_CMStringCopy2(fl)  CAL_CMGETAPI( "CMStringCopy2" ) 
	#define CAL_CMStringCopy2 pICMUtils->ICMStringCopy2
	#define CHK_CMStringCopy2 (pICMUtils != NULL)
	#define EXP_CMStringCopy2  CAL_CMEXPAPI( "CMStringCopy2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMStringCopy2  PFCMSTRINGCOPY2 pfCMStringCopy2;
	#define EXT_CMStringCopy2  extern PFCMSTRINGCOPY2 pfCMStringCopy2;
	#define GET_CMStringCopy2(fl)  s_pfCMGetAPI2( "CMStringCopy2", (RTS_VOID_FCTPTR *)&pfCMStringCopy2, (fl), 0, 0)
	#define CAL_CMStringCopy2  pfCMStringCopy2
	#define CHK_CMStringCopy2  (pfCMStringCopy2 != NULL)
	#define EXP_CMStringCopy2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCopy2", (RTS_UINTPTR)CMStringCopy2, 0, 0) 
#endif




/**
 * <description> 
 *  Concatenate a string to the string class.
 * </description>
 * <param name="pString" type="IN">Pointer to string class</param>
 * <param name="pszToAdd" type="IN">Pointer to source string to add</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMStringCat(RTS_STRING_CLASS *pString, char *pszToAdd);
typedef RTS_RESULT (CDECL * PFCMSTRINGCAT) (RTS_STRING_CLASS *pString, char *pszToAdd);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMSTRINGCAT_NOTIMPLEMENTED)
	#define USE_CMStringCat
	#define EXT_CMStringCat
	#define GET_CMStringCat(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMStringCat(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMStringCat  FALSE
	#define EXP_CMStringCat  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMStringCat
	#define EXT_CMStringCat
	#define GET_CMStringCat(fl)  CAL_CMGETAPI( "CMStringCat" ) 
	#define CAL_CMStringCat  CMStringCat
	#define CHK_CMStringCat  TRUE
	#define EXP_CMStringCat  CAL_CMEXPAPI( "CMStringCat" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMStringCat
	#define EXT_CMStringCat
	#define GET_CMStringCat(fl)  CAL_CMGETAPI( "CMStringCat" ) 
	#define CAL_CMStringCat  CMStringCat
	#define CHK_CMStringCat  TRUE
	#define EXP_CMStringCat  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCat", (RTS_UINTPTR)CMStringCat, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMStringCat
	#define EXT_CMUtilsCMStringCat
	#define GET_CMUtilsCMStringCat  ERR_OK
	#define CAL_CMUtilsCMStringCat pICMUtils->ICMStringCat
	#define CHK_CMUtilsCMStringCat (pICMUtils != NULL)
	#define EXP_CMUtilsCMStringCat  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMStringCat
	#define EXT_CMStringCat
	#define GET_CMStringCat(fl)  CAL_CMGETAPI( "CMStringCat" ) 
	#define CAL_CMStringCat pICMUtils->ICMStringCat
	#define CHK_CMStringCat (pICMUtils != NULL)
	#define EXP_CMStringCat  CAL_CMEXPAPI( "CMStringCat" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMStringCat  PFCMSTRINGCAT pfCMStringCat;
	#define EXT_CMStringCat  extern PFCMSTRINGCAT pfCMStringCat;
	#define GET_CMStringCat(fl)  s_pfCMGetAPI2( "CMStringCat", (RTS_VOID_FCTPTR *)&pfCMStringCat, (fl), 0, 0)
	#define CAL_CMStringCat  pfCMStringCat
	#define CHK_CMStringCat  (pfCMStringCat != NULL)
	#define EXP_CMStringCat  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMStringCat", (RTS_UINTPTR)CMStringCat, 0, 0) 
#endif






/**
 ***************************************************************************
 * <description> 
 *  UNICODE string routines.
 *  16-bit wide Unicode characters encoded as UTF-16LE.
 * </description>
 ***************************************************************************
 */

/**
 * <description> 
 *  Compare two Unicode strings.
 *  16-bit wide Unicode characters encoded as UTF-16LE.
 * </description>
 * <param name="pwsz1" type="IN">Pointer to string 1</param>
 * <param name="pwsz2" type="IN">Pointer to string 2</param>
 * <result>
 *		0: string1 identical to string2
 *		lower 0: string1 less than string2
 *		greater 0: string1 greater than string2
 * </result>
 */
int CDECL CMUtlwstrcmp(const RTS_WCHAR *pwsz1, const RTS_WCHAR *pwsz2);
typedef int (CDECL * PFCMUTLWSTRCMP) (const RTS_WCHAR *pwsz1, const RTS_WCHAR *pwsz2);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWSTRCMP_NOTIMPLEMENTED)
	#define USE_CMUtlwstrcmp
	#define EXT_CMUtlwstrcmp
	#define GET_CMUtlwstrcmp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlwstrcmp(p0,p1)  (int)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlwstrcmp  FALSE
	#define EXP_CMUtlwstrcmp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlwstrcmp
	#define EXT_CMUtlwstrcmp
	#define GET_CMUtlwstrcmp(fl)  CAL_CMGETAPI( "CMUtlwstrcmp" ) 
	#define CAL_CMUtlwstrcmp  CMUtlwstrcmp
	#define CHK_CMUtlwstrcmp  TRUE
	#define EXP_CMUtlwstrcmp  CAL_CMEXPAPI( "CMUtlwstrcmp" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlwstrcmp
	#define EXT_CMUtlwstrcmp
	#define GET_CMUtlwstrcmp(fl)  CAL_CMGETAPI( "CMUtlwstrcmp" ) 
	#define CAL_CMUtlwstrcmp  CMUtlwstrcmp
	#define CHK_CMUtlwstrcmp  TRUE
	#define EXP_CMUtlwstrcmp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrcmp", (RTS_UINTPTR)CMUtlwstrcmp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlwstrcmp
	#define EXT_CMUtilsCMUtlwstrcmp
	#define GET_CMUtilsCMUtlwstrcmp  ERR_OK
	#define CAL_CMUtilsCMUtlwstrcmp pICMUtils->ICMUtlwstrcmp
	#define CHK_CMUtilsCMUtlwstrcmp (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlwstrcmp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlwstrcmp
	#define EXT_CMUtlwstrcmp
	#define GET_CMUtlwstrcmp(fl)  CAL_CMGETAPI( "CMUtlwstrcmp" ) 
	#define CAL_CMUtlwstrcmp pICMUtils->ICMUtlwstrcmp
	#define CHK_CMUtlwstrcmp (pICMUtils != NULL)
	#define EXP_CMUtlwstrcmp  CAL_CMEXPAPI( "CMUtlwstrcmp" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlwstrcmp  PFCMUTLWSTRCMP pfCMUtlwstrcmp;
	#define EXT_CMUtlwstrcmp  extern PFCMUTLWSTRCMP pfCMUtlwstrcmp;
	#define GET_CMUtlwstrcmp(fl)  s_pfCMGetAPI2( "CMUtlwstrcmp", (RTS_VOID_FCTPTR *)&pfCMUtlwstrcmp, (fl), 0, 0)
	#define CAL_CMUtlwstrcmp  pfCMUtlwstrcmp
	#define CHK_CMUtlwstrcmp  (pfCMUtlwstrcmp != NULL)
	#define EXP_CMUtlwstrcmp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrcmp", (RTS_UINTPTR)CMUtlwstrcmp, 0, 0) 
#endif




/**
 * <description> 
 *  Compare two Unicode strings with specified number of characters.
 *  16-bit wide Unicode characters encoded as UTF-16LE.
 * </description>
 * <param name="pwsz1" type="IN">Pointer to string 1</param>
 * <param name="pwsz2" type="IN">Pointer to string 2</param>
 * <param name="nCount" type="IN">Number of characters to compare</param>
 * <result>
 *		0: string1 identical to string2
 *		lower 0: string1 less than string2
 *		greater 0: string1 greater than string2
 * </result>
 */
int CDECL CMUtlwstrncmp(const RTS_WCHAR *pwsz1, const RTS_WCHAR *pwsz2, RTS_SIZE nCount);
typedef int (CDECL * PFCMUTLWSTRNCMP) (const RTS_WCHAR *pwsz1, const RTS_WCHAR *pwsz2, RTS_SIZE nCount);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWSTRNCMP_NOTIMPLEMENTED)
	#define USE_CMUtlwstrncmp
	#define EXT_CMUtlwstrncmp
	#define GET_CMUtlwstrncmp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlwstrncmp(p0,p1,p2)  (int)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlwstrncmp  FALSE
	#define EXP_CMUtlwstrncmp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlwstrncmp
	#define EXT_CMUtlwstrncmp
	#define GET_CMUtlwstrncmp(fl)  CAL_CMGETAPI( "CMUtlwstrncmp" ) 
	#define CAL_CMUtlwstrncmp  CMUtlwstrncmp
	#define CHK_CMUtlwstrncmp  TRUE
	#define EXP_CMUtlwstrncmp  CAL_CMEXPAPI( "CMUtlwstrncmp" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlwstrncmp
	#define EXT_CMUtlwstrncmp
	#define GET_CMUtlwstrncmp(fl)  CAL_CMGETAPI( "CMUtlwstrncmp" ) 
	#define CAL_CMUtlwstrncmp  CMUtlwstrncmp
	#define CHK_CMUtlwstrncmp  TRUE
	#define EXP_CMUtlwstrncmp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrncmp", (RTS_UINTPTR)CMUtlwstrncmp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlwstrncmp
	#define EXT_CMUtilsCMUtlwstrncmp
	#define GET_CMUtilsCMUtlwstrncmp  ERR_OK
	#define CAL_CMUtilsCMUtlwstrncmp pICMUtils->ICMUtlwstrncmp
	#define CHK_CMUtilsCMUtlwstrncmp (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlwstrncmp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlwstrncmp
	#define EXT_CMUtlwstrncmp
	#define GET_CMUtlwstrncmp(fl)  CAL_CMGETAPI( "CMUtlwstrncmp" ) 
	#define CAL_CMUtlwstrncmp pICMUtils->ICMUtlwstrncmp
	#define CHK_CMUtlwstrncmp (pICMUtils != NULL)
	#define EXP_CMUtlwstrncmp  CAL_CMEXPAPI( "CMUtlwstrncmp" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlwstrncmp  PFCMUTLWSTRNCMP pfCMUtlwstrncmp;
	#define EXT_CMUtlwstrncmp  extern PFCMUTLWSTRNCMP pfCMUtlwstrncmp;
	#define GET_CMUtlwstrncmp(fl)  s_pfCMGetAPI2( "CMUtlwstrncmp", (RTS_VOID_FCTPTR *)&pfCMUtlwstrncmp, (fl), 0, 0)
	#define CAL_CMUtlwstrncmp  pfCMUtlwstrncmp
	#define CHK_CMUtlwstrncmp  (pfCMUtlwstrncmp != NULL)
	#define EXP_CMUtlwstrncmp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrncmp", (RTS_UINTPTR)CMUtlwstrncmp, 0, 0) 
#endif




/**
 * <description> 
 *  Copy one Unicode string to another in a safe way.
 *  16-bit wide Unicode characters encoded as UTF-16LE.
 * </description>
 * <param name="pwszDest" type="IN">Destination string</param>
 * <param name="nDestSize" type="IN">Size of the destination buffer in unicode characters (not bytes)!</param>
 * <param name="pwszSrc" type="IN">Source string</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlwstrcpy(RTS_WCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_WCHAR *pwszSrc);
typedef RTS_RESULT (CDECL * PFCMUTLWSTRCPY) (RTS_WCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_WCHAR *pwszSrc);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWSTRCPY_NOTIMPLEMENTED)
	#define USE_CMUtlwstrcpy
	#define EXT_CMUtlwstrcpy
	#define GET_CMUtlwstrcpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlwstrcpy(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlwstrcpy  FALSE
	#define EXP_CMUtlwstrcpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlwstrcpy
	#define EXT_CMUtlwstrcpy
	#define GET_CMUtlwstrcpy(fl)  CAL_CMGETAPI( "CMUtlwstrcpy" ) 
	#define CAL_CMUtlwstrcpy  CMUtlwstrcpy
	#define CHK_CMUtlwstrcpy  TRUE
	#define EXP_CMUtlwstrcpy  CAL_CMEXPAPI( "CMUtlwstrcpy" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlwstrcpy
	#define EXT_CMUtlwstrcpy
	#define GET_CMUtlwstrcpy(fl)  CAL_CMGETAPI( "CMUtlwstrcpy" ) 
	#define CAL_CMUtlwstrcpy  CMUtlwstrcpy
	#define CHK_CMUtlwstrcpy  TRUE
	#define EXP_CMUtlwstrcpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrcpy", (RTS_UINTPTR)CMUtlwstrcpy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlwstrcpy
	#define EXT_CMUtilsCMUtlwstrcpy
	#define GET_CMUtilsCMUtlwstrcpy  ERR_OK
	#define CAL_CMUtilsCMUtlwstrcpy pICMUtils->ICMUtlwstrcpy
	#define CHK_CMUtilsCMUtlwstrcpy (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlwstrcpy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlwstrcpy
	#define EXT_CMUtlwstrcpy
	#define GET_CMUtlwstrcpy(fl)  CAL_CMGETAPI( "CMUtlwstrcpy" ) 
	#define CAL_CMUtlwstrcpy pICMUtils->ICMUtlwstrcpy
	#define CHK_CMUtlwstrcpy (pICMUtils != NULL)
	#define EXP_CMUtlwstrcpy  CAL_CMEXPAPI( "CMUtlwstrcpy" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlwstrcpy  PFCMUTLWSTRCPY pfCMUtlwstrcpy;
	#define EXT_CMUtlwstrcpy  extern PFCMUTLWSTRCPY pfCMUtlwstrcpy;
	#define GET_CMUtlwstrcpy(fl)  s_pfCMGetAPI2( "CMUtlwstrcpy", (RTS_VOID_FCTPTR *)&pfCMUtlwstrcpy, (fl), 0, 0)
	#define CAL_CMUtlwstrcpy  pfCMUtlwstrcpy
	#define CHK_CMUtlwstrcpy  (pfCMUtlwstrcpy != NULL)
	#define EXP_CMUtlwstrcpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrcpy", (RTS_UINTPTR)CMUtlwstrcpy, 0, 0) 
#endif




/**
 * <description> 
 *  Concatenate two Unicode strings in a safe way.
 *  16-bit wide Unicode characters encoded as UTF-16LE.
 * </description>
 * <param name="pwszDest" type="IN">Destination string</param>
 * <param name="nDestSize" type="IN">Size of the destination buffer in Unicode characters (not bytes)!</param>
 * <param name="pwszSrc" type="IN">Source string</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlwstrcat(RTS_WCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_WCHAR *pwszSrc);
typedef RTS_RESULT (CDECL * PFCMUTLWSTRCAT) (RTS_WCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_WCHAR *pwszSrc);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWSTRCAT_NOTIMPLEMENTED)
	#define USE_CMUtlwstrcat
	#define EXT_CMUtlwstrcat
	#define GET_CMUtlwstrcat(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlwstrcat(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlwstrcat  FALSE
	#define EXP_CMUtlwstrcat  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlwstrcat
	#define EXT_CMUtlwstrcat
	#define GET_CMUtlwstrcat(fl)  CAL_CMGETAPI( "CMUtlwstrcat" ) 
	#define CAL_CMUtlwstrcat  CMUtlwstrcat
	#define CHK_CMUtlwstrcat  TRUE
	#define EXP_CMUtlwstrcat  CAL_CMEXPAPI( "CMUtlwstrcat" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlwstrcat
	#define EXT_CMUtlwstrcat
	#define GET_CMUtlwstrcat(fl)  CAL_CMGETAPI( "CMUtlwstrcat" ) 
	#define CAL_CMUtlwstrcat  CMUtlwstrcat
	#define CHK_CMUtlwstrcat  TRUE
	#define EXP_CMUtlwstrcat  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrcat", (RTS_UINTPTR)CMUtlwstrcat, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlwstrcat
	#define EXT_CMUtilsCMUtlwstrcat
	#define GET_CMUtilsCMUtlwstrcat  ERR_OK
	#define CAL_CMUtilsCMUtlwstrcat pICMUtils->ICMUtlwstrcat
	#define CHK_CMUtilsCMUtlwstrcat (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlwstrcat  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlwstrcat
	#define EXT_CMUtlwstrcat
	#define GET_CMUtlwstrcat(fl)  CAL_CMGETAPI( "CMUtlwstrcat" ) 
	#define CAL_CMUtlwstrcat pICMUtils->ICMUtlwstrcat
	#define CHK_CMUtlwstrcat (pICMUtils != NULL)
	#define EXP_CMUtlwstrcat  CAL_CMEXPAPI( "CMUtlwstrcat" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlwstrcat  PFCMUTLWSTRCAT pfCMUtlwstrcat;
	#define EXT_CMUtlwstrcat  extern PFCMUTLWSTRCAT pfCMUtlwstrcat;
	#define GET_CMUtlwstrcat(fl)  s_pfCMGetAPI2( "CMUtlwstrcat", (RTS_VOID_FCTPTR *)&pfCMUtlwstrcat, (fl), 0, 0)
	#define CAL_CMUtlwstrcat  pfCMUtlwstrcat
	#define CHK_CMUtlwstrcat  (pfCMUtlwstrcat != NULL)
	#define EXP_CMUtlwstrcat  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrcat", (RTS_UINTPTR)CMUtlwstrcat, 0, 0) 
#endif




/**
 * <description> 
 *  Copy number of characters from one Unicode string to another in a safe way.
 *  16-bit wide Unicode characters encoded as UTF-16LE.
 * </description>
 * <param name="pwszDest" type="IN">Destination string</param>
 * <param name="nDestSize" type="IN">Size of the destination buffer in Unicode characters (not bytes)!</param>
 * <param name="pwszSrc" type="IN">Source string</param>
 * <param name="n" type="IN">Number of characters to copy (not bytes)!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlwstrncpy(RTS_WCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_WCHAR *pwszSrc, RTS_SIZE n);
typedef RTS_RESULT (CDECL * PFCMUTLWSTRNCPY) (RTS_WCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_WCHAR *pwszSrc, RTS_SIZE n);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWSTRNCPY_NOTIMPLEMENTED)
	#define USE_CMUtlwstrncpy
	#define EXT_CMUtlwstrncpy
	#define GET_CMUtlwstrncpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlwstrncpy(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlwstrncpy  FALSE
	#define EXP_CMUtlwstrncpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlwstrncpy
	#define EXT_CMUtlwstrncpy
	#define GET_CMUtlwstrncpy(fl)  CAL_CMGETAPI( "CMUtlwstrncpy" ) 
	#define CAL_CMUtlwstrncpy  CMUtlwstrncpy
	#define CHK_CMUtlwstrncpy  TRUE
	#define EXP_CMUtlwstrncpy  CAL_CMEXPAPI( "CMUtlwstrncpy" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlwstrncpy
	#define EXT_CMUtlwstrncpy
	#define GET_CMUtlwstrncpy(fl)  CAL_CMGETAPI( "CMUtlwstrncpy" ) 
	#define CAL_CMUtlwstrncpy  CMUtlwstrncpy
	#define CHK_CMUtlwstrncpy  TRUE
	#define EXP_CMUtlwstrncpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrncpy", (RTS_UINTPTR)CMUtlwstrncpy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlwstrncpy
	#define EXT_CMUtilsCMUtlwstrncpy
	#define GET_CMUtilsCMUtlwstrncpy  ERR_OK
	#define CAL_CMUtilsCMUtlwstrncpy pICMUtils->ICMUtlwstrncpy
	#define CHK_CMUtilsCMUtlwstrncpy (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlwstrncpy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlwstrncpy
	#define EXT_CMUtlwstrncpy
	#define GET_CMUtlwstrncpy(fl)  CAL_CMGETAPI( "CMUtlwstrncpy" ) 
	#define CAL_CMUtlwstrncpy pICMUtils->ICMUtlwstrncpy
	#define CHK_CMUtlwstrncpy (pICMUtils != NULL)
	#define EXP_CMUtlwstrncpy  CAL_CMEXPAPI( "CMUtlwstrncpy" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlwstrncpy  PFCMUTLWSTRNCPY pfCMUtlwstrncpy;
	#define EXT_CMUtlwstrncpy  extern PFCMUTLWSTRNCPY pfCMUtlwstrncpy;
	#define GET_CMUtlwstrncpy(fl)  s_pfCMGetAPI2( "CMUtlwstrncpy", (RTS_VOID_FCTPTR *)&pfCMUtlwstrncpy, (fl), 0, 0)
	#define CAL_CMUtlwstrncpy  pfCMUtlwstrncpy
	#define CHK_CMUtlwstrncpy  (pfCMUtlwstrncpy != NULL)
	#define EXP_CMUtlwstrncpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrncpy", (RTS_UINTPTR)CMUtlwstrncpy, 0, 0) 
#endif




/**
 * <description> 
 *  Get the number of 16-bit values in the content of a WString _without_ terminating NULL character.
 * </description>
 * <param name="pwsz" type="IN">
 *     Pointer to string.
 *     16-bit wide Unicode characters encoded as UTF-16LE.
 * </param>
 * <result>Number of 16-bit values in the content of a WString _without_ terminating NULL character</result>
 */
RTS_SIZE CDECL CMUtlwstrlen(const RTS_WCHAR *pwsz);
typedef RTS_SIZE (CDECL * PFCMUTLWSTRLEN) (const RTS_WCHAR *pwsz);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWSTRLEN_NOTIMPLEMENTED)
	#define USE_CMUtlwstrlen
	#define EXT_CMUtlwstrlen
	#define GET_CMUtlwstrlen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlwstrlen(p0)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlwstrlen  FALSE
	#define EXP_CMUtlwstrlen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlwstrlen
	#define EXT_CMUtlwstrlen
	#define GET_CMUtlwstrlen(fl)  CAL_CMGETAPI( "CMUtlwstrlen" ) 
	#define CAL_CMUtlwstrlen  CMUtlwstrlen
	#define CHK_CMUtlwstrlen  TRUE
	#define EXP_CMUtlwstrlen  CAL_CMEXPAPI( "CMUtlwstrlen" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlwstrlen
	#define EXT_CMUtlwstrlen
	#define GET_CMUtlwstrlen(fl)  CAL_CMGETAPI( "CMUtlwstrlen" ) 
	#define CAL_CMUtlwstrlen  CMUtlwstrlen
	#define CHK_CMUtlwstrlen  TRUE
	#define EXP_CMUtlwstrlen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrlen", (RTS_UINTPTR)CMUtlwstrlen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlwstrlen
	#define EXT_CMUtilsCMUtlwstrlen
	#define GET_CMUtilsCMUtlwstrlen  ERR_OK
	#define CAL_CMUtilsCMUtlwstrlen pICMUtils->ICMUtlwstrlen
	#define CHK_CMUtilsCMUtlwstrlen (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlwstrlen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlwstrlen
	#define EXT_CMUtlwstrlen
	#define GET_CMUtlwstrlen(fl)  CAL_CMGETAPI( "CMUtlwstrlen" ) 
	#define CAL_CMUtlwstrlen pICMUtils->ICMUtlwstrlen
	#define CHK_CMUtlwstrlen (pICMUtils != NULL)
	#define EXP_CMUtlwstrlen  CAL_CMEXPAPI( "CMUtlwstrlen" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlwstrlen  PFCMUTLWSTRLEN pfCMUtlwstrlen;
	#define EXT_CMUtlwstrlen  extern PFCMUTLWSTRLEN pfCMUtlwstrlen;
	#define GET_CMUtlwstrlen(fl)  s_pfCMGetAPI2( "CMUtlwstrlen", (RTS_VOID_FCTPTR *)&pfCMUtlwstrlen, (fl), 0, 0)
	#define CAL_CMUtlwstrlen  pfCMUtlwstrlen
	#define CHK_CMUtlwstrlen  (pfCMUtlwstrlen != NULL)
	#define EXP_CMUtlwstrlen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrlen", (RTS_UINTPTR)CMUtlwstrlen, 0, 0) 
#endif




/**
 * <description> 
 *  Convert the characters in a Unicode string to lower case.
 * </description>
 * <param name="pwsz" type="IN">
 *     Pointer to string.
 *     16-bit wide Unicode characters encoded as UTF-16LE.
 * </param>
 * <result>pwsz is returned</result>
 */
RTS_WCHAR* CDECL CMUtlwtolower(RTS_WCHAR* pwsz);
typedef RTS_WCHAR* (CDECL * PFCMUTLWTOLOWER) (RTS_WCHAR* pwsz);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWTOLOWER_NOTIMPLEMENTED)
	#define USE_CMUtlwtolower
	#define EXT_CMUtlwtolower
	#define GET_CMUtlwtolower(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlwtolower(p0)  (RTS_WCHAR*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlwtolower  FALSE
	#define EXP_CMUtlwtolower  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlwtolower
	#define EXT_CMUtlwtolower
	#define GET_CMUtlwtolower(fl)  CAL_CMGETAPI( "CMUtlwtolower" ) 
	#define CAL_CMUtlwtolower  CMUtlwtolower
	#define CHK_CMUtlwtolower  TRUE
	#define EXP_CMUtlwtolower  CAL_CMEXPAPI( "CMUtlwtolower" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlwtolower
	#define EXT_CMUtlwtolower
	#define GET_CMUtlwtolower(fl)  CAL_CMGETAPI( "CMUtlwtolower" ) 
	#define CAL_CMUtlwtolower  CMUtlwtolower
	#define CHK_CMUtlwtolower  TRUE
	#define EXP_CMUtlwtolower  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwtolower", (RTS_UINTPTR)CMUtlwtolower, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlwtolower
	#define EXT_CMUtilsCMUtlwtolower
	#define GET_CMUtilsCMUtlwtolower  ERR_OK
	#define CAL_CMUtilsCMUtlwtolower pICMUtils->ICMUtlwtolower
	#define CHK_CMUtilsCMUtlwtolower (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlwtolower  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlwtolower
	#define EXT_CMUtlwtolower
	#define GET_CMUtlwtolower(fl)  CAL_CMGETAPI( "CMUtlwtolower" ) 
	#define CAL_CMUtlwtolower pICMUtils->ICMUtlwtolower
	#define CHK_CMUtlwtolower (pICMUtils != NULL)
	#define EXP_CMUtlwtolower  CAL_CMEXPAPI( "CMUtlwtolower" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlwtolower  PFCMUTLWTOLOWER pfCMUtlwtolower;
	#define EXT_CMUtlwtolower  extern PFCMUTLWTOLOWER pfCMUtlwtolower;
	#define GET_CMUtlwtolower(fl)  s_pfCMGetAPI2( "CMUtlwtolower", (RTS_VOID_FCTPTR *)&pfCMUtlwtolower, (fl), 0, 0)
	#define CAL_CMUtlwtolower  pfCMUtlwtolower
	#define CHK_CMUtlwtolower  (pfCMUtlwtolower != NULL)
	#define EXP_CMUtlwtolower  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwtolower", (RTS_UINTPTR)CMUtlwtolower, 0, 0) 
#endif




/**
 * <description> 
 *  Convert the characters in a Unicode string to upper case.
 * </description>
 * <param name="pwsz" type="IN">
 *     Pointer to string.
 *     16-bit wide Unicode characters encoded as UTF-16LE.
 * </param>
 * <result>pwsz is returned</result>
 */
RTS_WCHAR* CDECL CMUtlwtoupper(RTS_WCHAR* pwsz);
typedef RTS_WCHAR* (CDECL * PFCMUTLWTOUPPER) (RTS_WCHAR* pwsz);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWTOUPPER_NOTIMPLEMENTED)
	#define USE_CMUtlwtoupper
	#define EXT_CMUtlwtoupper
	#define GET_CMUtlwtoupper(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlwtoupper(p0)  (RTS_WCHAR*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlwtoupper  FALSE
	#define EXP_CMUtlwtoupper  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlwtoupper
	#define EXT_CMUtlwtoupper
	#define GET_CMUtlwtoupper(fl)  CAL_CMGETAPI( "CMUtlwtoupper" ) 
	#define CAL_CMUtlwtoupper  CMUtlwtoupper
	#define CHK_CMUtlwtoupper  TRUE
	#define EXP_CMUtlwtoupper  CAL_CMEXPAPI( "CMUtlwtoupper" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlwtoupper
	#define EXT_CMUtlwtoupper
	#define GET_CMUtlwtoupper(fl)  CAL_CMGETAPI( "CMUtlwtoupper" ) 
	#define CAL_CMUtlwtoupper  CMUtlwtoupper
	#define CHK_CMUtlwtoupper  TRUE
	#define EXP_CMUtlwtoupper  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwtoupper", (RTS_UINTPTR)CMUtlwtoupper, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlwtoupper
	#define EXT_CMUtilsCMUtlwtoupper
	#define GET_CMUtilsCMUtlwtoupper  ERR_OK
	#define CAL_CMUtilsCMUtlwtoupper pICMUtils->ICMUtlwtoupper
	#define CHK_CMUtilsCMUtlwtoupper (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlwtoupper  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlwtoupper
	#define EXT_CMUtlwtoupper
	#define GET_CMUtlwtoupper(fl)  CAL_CMGETAPI( "CMUtlwtoupper" ) 
	#define CAL_CMUtlwtoupper pICMUtils->ICMUtlwtoupper
	#define CHK_CMUtlwtoupper (pICMUtils != NULL)
	#define EXP_CMUtlwtoupper  CAL_CMEXPAPI( "CMUtlwtoupper" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlwtoupper  PFCMUTLWTOUPPER pfCMUtlwtoupper;
	#define EXT_CMUtlwtoupper  extern PFCMUTLWTOUPPER pfCMUtlwtoupper;
	#define GET_CMUtlwtoupper(fl)  s_pfCMGetAPI2( "CMUtlwtoupper", (RTS_VOID_FCTPTR *)&pfCMUtlwtoupper, (fl), 0, 0)
	#define CAL_CMUtlwtoupper  pfCMUtlwtoupper
	#define CHK_CMUtlwtoupper  (pfCMUtlwtoupper != NULL)
	#define EXP_CMUtlwtoupper  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwtoupper", (RTS_UINTPTR)CMUtlwtoupper, 0, 0) 
#endif




/**
 * <description> 
 *  Convert a byte character string to a Unicode character string.
 * </description>
 * <param name="pszSrc" type="IN">Pointer to byte character string</param>
 * <param name="pwszDest" type="IN">
 *     Pointer to Unicode character string.
 *     16-bit wide Unicode characters encoded as UTF-16LE.
 * </param>
 * <param name="nDestSize" type="IN">Size of the destination buffer in Unicode characters (not bytes)!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlStrToW(const char* pszSrc, RTS_WCHAR* pwszDest, RTS_SIZE nDestSize);
typedef RTS_RESULT (CDECL * PFCMUTLSTRTOW) (const char* pszSrc, RTS_WCHAR* pwszDest, RTS_SIZE nDestSize);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRTOW_NOTIMPLEMENTED)
	#define USE_CMUtlStrToW
	#define EXT_CMUtlStrToW
	#define GET_CMUtlStrToW(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrToW(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrToW  FALSE
	#define EXP_CMUtlStrToW  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrToW
	#define EXT_CMUtlStrToW
	#define GET_CMUtlStrToW(fl)  CAL_CMGETAPI( "CMUtlStrToW" ) 
	#define CAL_CMUtlStrToW  CMUtlStrToW
	#define CHK_CMUtlStrToW  TRUE
	#define EXP_CMUtlStrToW  CAL_CMEXPAPI( "CMUtlStrToW" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrToW
	#define EXT_CMUtlStrToW
	#define GET_CMUtlStrToW(fl)  CAL_CMGETAPI( "CMUtlStrToW" ) 
	#define CAL_CMUtlStrToW  CMUtlStrToW
	#define CHK_CMUtlStrToW  TRUE
	#define EXP_CMUtlStrToW  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrToW", (RTS_UINTPTR)CMUtlStrToW, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrToW
	#define EXT_CMUtilsCMUtlStrToW
	#define GET_CMUtilsCMUtlStrToW  ERR_OK
	#define CAL_CMUtilsCMUtlStrToW pICMUtils->ICMUtlStrToW
	#define CHK_CMUtilsCMUtlStrToW (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrToW  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrToW
	#define EXT_CMUtlStrToW
	#define GET_CMUtlStrToW(fl)  CAL_CMGETAPI( "CMUtlStrToW" ) 
	#define CAL_CMUtlStrToW pICMUtils->ICMUtlStrToW
	#define CHK_CMUtlStrToW (pICMUtils != NULL)
	#define EXP_CMUtlStrToW  CAL_CMEXPAPI( "CMUtlStrToW" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrToW  PFCMUTLSTRTOW pfCMUtlStrToW;
	#define EXT_CMUtlStrToW  extern PFCMUTLSTRTOW pfCMUtlStrToW;
	#define GET_CMUtlStrToW(fl)  s_pfCMGetAPI2( "CMUtlStrToW", (RTS_VOID_FCTPTR *)&pfCMUtlStrToW, (fl), 0, 0)
	#define CAL_CMUtlStrToW  pfCMUtlStrToW
	#define CHK_CMUtlStrToW  (pfCMUtlStrToW != NULL)
	#define EXP_CMUtlStrToW  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrToW", (RTS_UINTPTR)CMUtlStrToW, 0, 0) 
#endif




/**
 * <description> 
 *  Convert a Unicode character string to a byte character string .
 * </description>
 * <param name="pwszSrc" type="IN">
 *     Pointer to Unicode character string.
 *     16-bit wide Unicode characters encoded as UTF-16LE.
 * </param>
 * <param name="pszDest" type="IN">Pointer to byte character string</param>
 * <param name="nDestSize" type="IN">Size of the destination buffer in bytes</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlWToStr(const RTS_WCHAR* pwszSrc, char* pszDest, RTS_SIZE nDestSize);
typedef RTS_RESULT (CDECL * PFCMUTLWTOSTR) (const RTS_WCHAR* pwszSrc, char* pszDest, RTS_SIZE nDestSize);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWTOSTR_NOTIMPLEMENTED)
	#define USE_CMUtlWToStr
	#define EXT_CMUtlWToStr
	#define GET_CMUtlWToStr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlWToStr(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlWToStr  FALSE
	#define EXP_CMUtlWToStr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlWToStr
	#define EXT_CMUtlWToStr
	#define GET_CMUtlWToStr(fl)  CAL_CMGETAPI( "CMUtlWToStr" ) 
	#define CAL_CMUtlWToStr  CMUtlWToStr
	#define CHK_CMUtlWToStr  TRUE
	#define EXP_CMUtlWToStr  CAL_CMEXPAPI( "CMUtlWToStr" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlWToStr
	#define EXT_CMUtlWToStr
	#define GET_CMUtlWToStr(fl)  CAL_CMGETAPI( "CMUtlWToStr" ) 
	#define CAL_CMUtlWToStr  CMUtlWToStr
	#define CHK_CMUtlWToStr  TRUE
	#define EXP_CMUtlWToStr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlWToStr", (RTS_UINTPTR)CMUtlWToStr, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlWToStr
	#define EXT_CMUtilsCMUtlWToStr
	#define GET_CMUtilsCMUtlWToStr  ERR_OK
	#define CAL_CMUtilsCMUtlWToStr pICMUtils->ICMUtlWToStr
	#define CHK_CMUtilsCMUtlWToStr (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlWToStr  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlWToStr
	#define EXT_CMUtlWToStr
	#define GET_CMUtlWToStr(fl)  CAL_CMGETAPI( "CMUtlWToStr" ) 
	#define CAL_CMUtlWToStr pICMUtils->ICMUtlWToStr
	#define CHK_CMUtlWToStr (pICMUtils != NULL)
	#define EXP_CMUtlWToStr  CAL_CMEXPAPI( "CMUtlWToStr" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlWToStr  PFCMUTLWTOSTR pfCMUtlWToStr;
	#define EXT_CMUtlWToStr  extern PFCMUTLWTOSTR pfCMUtlWToStr;
	#define GET_CMUtlWToStr(fl)  s_pfCMGetAPI2( "CMUtlWToStr", (RTS_VOID_FCTPTR *)&pfCMUtlWToStr, (fl), 0, 0)
	#define CAL_CMUtlWToStr  pfCMUtlWToStr
	#define CHK_CMUtlWToStr  (pfCMUtlWToStr != NULL)
	#define EXP_CMUtlWToStr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlWToStr", (RTS_UINTPTR)CMUtlWToStr, 0, 0) 
#endif




/**
 * <description> 
 *  Find a character in a Unicode string.
 * </description>
 * <param name="pwsz" type="IN">
 *     Pointer to Unicode string.
 *     16-bit wide Unicode characters encoded as UTF-16LE.
 * </param>
 * <param name="w" type="IN">Character to find</param>
 * <result>Returns the find position or NULL if not found</result>
 */
RTS_WCHAR* CDECL CMUtlwstrchr(const RTS_WCHAR *pwsz, RTS_WCHAR w);
typedef RTS_WCHAR* (CDECL * PFCMUTLWSTRCHR) (const RTS_WCHAR *pwsz, RTS_WCHAR w);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWSTRCHR_NOTIMPLEMENTED)
	#define USE_CMUtlwstrchr
	#define EXT_CMUtlwstrchr
	#define GET_CMUtlwstrchr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlwstrchr(p0,p1)  (RTS_WCHAR*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlwstrchr  FALSE
	#define EXP_CMUtlwstrchr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlwstrchr
	#define EXT_CMUtlwstrchr
	#define GET_CMUtlwstrchr(fl)  CAL_CMGETAPI( "CMUtlwstrchr" ) 
	#define CAL_CMUtlwstrchr  CMUtlwstrchr
	#define CHK_CMUtlwstrchr  TRUE
	#define EXP_CMUtlwstrchr  CAL_CMEXPAPI( "CMUtlwstrchr" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlwstrchr
	#define EXT_CMUtlwstrchr
	#define GET_CMUtlwstrchr(fl)  CAL_CMGETAPI( "CMUtlwstrchr" ) 
	#define CAL_CMUtlwstrchr  CMUtlwstrchr
	#define CHK_CMUtlwstrchr  TRUE
	#define EXP_CMUtlwstrchr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrchr", (RTS_UINTPTR)CMUtlwstrchr, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlwstrchr
	#define EXT_CMUtilsCMUtlwstrchr
	#define GET_CMUtilsCMUtlwstrchr  ERR_OK
	#define CAL_CMUtilsCMUtlwstrchr pICMUtils->ICMUtlwstrchr
	#define CHK_CMUtilsCMUtlwstrchr (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlwstrchr  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlwstrchr
	#define EXT_CMUtlwstrchr
	#define GET_CMUtlwstrchr(fl)  CAL_CMGETAPI( "CMUtlwstrchr" ) 
	#define CAL_CMUtlwstrchr pICMUtils->ICMUtlwstrchr
	#define CHK_CMUtlwstrchr (pICMUtils != NULL)
	#define EXP_CMUtlwstrchr  CAL_CMEXPAPI( "CMUtlwstrchr" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlwstrchr  PFCMUTLWSTRCHR pfCMUtlwstrchr;
	#define EXT_CMUtlwstrchr  extern PFCMUTLWSTRCHR pfCMUtlwstrchr;
	#define GET_CMUtlwstrchr(fl)  s_pfCMGetAPI2( "CMUtlwstrchr", (RTS_VOID_FCTPTR *)&pfCMUtlwstrchr, (fl), 0, 0)
	#define CAL_CMUtlwstrchr  pfCMUtlwstrchr
	#define CHK_CMUtlwstrchr  (pfCMUtlwstrchr != NULL)
	#define EXP_CMUtlwstrchr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlwstrchr", (RTS_UINTPTR)CMUtlwstrchr, 0, 0) 
#endif




/**
 * <description> 
 *  Check all characters in a WSTRING.
 *  
 *  The "CMUtlCheckUtf16Encoding" function iterates over all characters of the passed WSTRING.
 *  For each single UTF-16 character the Unicode code point is determined.
 *  If the 16-bit value of the UTF-16 character is in the range 0xD800 ... 0xDFFF,
 *  it is a so-called surrogate pair consisting of four bytes.
 *  Then the following cases lead to an error:
 *      - The UTF-16 character should be four bytes,
 *        but there aren't at least four unprocessed bytes left in the WSTRING.
 *      - The high surrogate is not in the range [0xD800..0xDBFF].
 *      - The low surrogate is not in the range [0xDC00..0xDFFF].
 * </description>
 * <param name="pwsz" type="IN">
 *     Pointer to WSTRING to be validated.
 *     16-bit wide Unicode characters encoded as UTF-16LE.
 * </param>
 * <param name="siWstrLen" type="IN">Size of read buffer in WCHARs. Set to -1 if unknown.</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">All characters are valid.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Pointer pwsz or siWstrLen is not valid.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_INVALID_ENCODING">Invalid character in WSTRING detected.</errorcode>
 */
RTS_RESULT CDECL CMUtlCheckUtf16Encoding(const RTS_WCHAR *pwsz, RTS_SIZE siWstrLen);
typedef RTS_RESULT (CDECL * PFCMUTLCHECKUTF16ENCODING) (const RTS_WCHAR *pwsz, RTS_SIZE siWstrLen);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCHECKUTF16ENCODING_NOTIMPLEMENTED)
	#define USE_CMUtlCheckUtf16Encoding
	#define EXT_CMUtlCheckUtf16Encoding
	#define GET_CMUtlCheckUtf16Encoding(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlCheckUtf16Encoding(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlCheckUtf16Encoding  FALSE
	#define EXP_CMUtlCheckUtf16Encoding  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlCheckUtf16Encoding
	#define EXT_CMUtlCheckUtf16Encoding
	#define GET_CMUtlCheckUtf16Encoding(fl)  CAL_CMGETAPI( "CMUtlCheckUtf16Encoding" ) 
	#define CAL_CMUtlCheckUtf16Encoding  CMUtlCheckUtf16Encoding
	#define CHK_CMUtlCheckUtf16Encoding  TRUE
	#define EXP_CMUtlCheckUtf16Encoding  CAL_CMEXPAPI( "CMUtlCheckUtf16Encoding" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlCheckUtf16Encoding
	#define EXT_CMUtlCheckUtf16Encoding
	#define GET_CMUtlCheckUtf16Encoding(fl)  CAL_CMGETAPI( "CMUtlCheckUtf16Encoding" ) 
	#define CAL_CMUtlCheckUtf16Encoding  CMUtlCheckUtf16Encoding
	#define CHK_CMUtlCheckUtf16Encoding  TRUE
	#define EXP_CMUtlCheckUtf16Encoding  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlCheckUtf16Encoding", (RTS_UINTPTR)CMUtlCheckUtf16Encoding, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlCheckUtf16Encoding
	#define EXT_CMUtilsCMUtlCheckUtf16Encoding
	#define GET_CMUtilsCMUtlCheckUtf16Encoding  ERR_OK
	#define CAL_CMUtilsCMUtlCheckUtf16Encoding pICMUtils->ICMUtlCheckUtf16Encoding
	#define CHK_CMUtilsCMUtlCheckUtf16Encoding (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlCheckUtf16Encoding  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlCheckUtf16Encoding
	#define EXT_CMUtlCheckUtf16Encoding
	#define GET_CMUtlCheckUtf16Encoding(fl)  CAL_CMGETAPI( "CMUtlCheckUtf16Encoding" ) 
	#define CAL_CMUtlCheckUtf16Encoding pICMUtils->ICMUtlCheckUtf16Encoding
	#define CHK_CMUtlCheckUtf16Encoding (pICMUtils != NULL)
	#define EXP_CMUtlCheckUtf16Encoding  CAL_CMEXPAPI( "CMUtlCheckUtf16Encoding" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlCheckUtf16Encoding  PFCMUTLCHECKUTF16ENCODING pfCMUtlCheckUtf16Encoding;
	#define EXT_CMUtlCheckUtf16Encoding  extern PFCMUTLCHECKUTF16ENCODING pfCMUtlCheckUtf16Encoding;
	#define GET_CMUtlCheckUtf16Encoding(fl)  s_pfCMGetAPI2( "CMUtlCheckUtf16Encoding", (RTS_VOID_FCTPTR *)&pfCMUtlCheckUtf16Encoding, (fl), 0, 0)
	#define CAL_CMUtlCheckUtf16Encoding  pfCMUtlCheckUtf16Encoding
	#define CHK_CMUtlCheckUtf16Encoding  (pfCMUtlCheckUtf16Encoding != NULL)
	#define EXP_CMUtlCheckUtf16Encoding  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlCheckUtf16Encoding", (RTS_UINTPTR)CMUtlCheckUtf16Encoding, 0, 0) 
#endif




/*
 *************************************************************************
 * <description> 
 *  Conversion between UTF-8 and RTS_WSTRING (UTF-16). The conversion
 *  can produce the REPLACEMENT CHARACTER (U+FFFD) for invalid sequences.
 * </description>
 *************************************************************************
 */
 
/**
 * <description>This function converts a UTF-16 encoded WSTRING to a sequence of UTF-8 code points.
 * If a code-point cannot be converted this code-point will be replaced with the Unicode REPLACEMENT 
 * CHARACTER (U+FFFD).</description>
 * <param name="pwsz" type="IN">
 *     Pointer to WSTRING to be converted.
 *     16-bit wide Unicode characters encoded as UTF-16LE.
 * </param>
 * <param name="wstrLen" type="IN">Size of read buffer in WCHARs. Set to -1 if unknown.</param>
 * <param name="pUtf8Str" type="IN">Pointer to buffer where to store the UTF-8 sequence.</param>
 * <param name="bufferSize" type="IN">Buffer size of UTF-8 buffer.</param>
 * <result>On of these error codes: 
 *                                  - ERR_OK: Everything went fine. No errors occurred.
 *                                  - ERR_PARAMETER: The parameter checks failed.
 *                                  - ERR_CONVERSION_INCOMPLETE: The conversion failed because of some code-points. These where replaced with U+FFFD.
 * </result>
 */
 
RTS_RESULT CDECL CMUtlWToUtf8(RTS_WCHAR *pwsz, RTS_SIZE wstrLen, RTS_UI8* pUtf8Str, RTS_SIZE bufferSize);
typedef RTS_RESULT (CDECL * PFCMUTLWTOUTF8) (RTS_WCHAR *pwsz, RTS_SIZE wstrLen, RTS_UI8* pUtf8Str, RTS_SIZE bufferSize);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLWTOUTF8_NOTIMPLEMENTED)
	#define USE_CMUtlWToUtf8
	#define EXT_CMUtlWToUtf8
	#define GET_CMUtlWToUtf8(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlWToUtf8(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlWToUtf8  FALSE
	#define EXP_CMUtlWToUtf8  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlWToUtf8
	#define EXT_CMUtlWToUtf8
	#define GET_CMUtlWToUtf8(fl)  CAL_CMGETAPI( "CMUtlWToUtf8" ) 
	#define CAL_CMUtlWToUtf8  CMUtlWToUtf8
	#define CHK_CMUtlWToUtf8  TRUE
	#define EXP_CMUtlWToUtf8  CAL_CMEXPAPI( "CMUtlWToUtf8" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlWToUtf8
	#define EXT_CMUtlWToUtf8
	#define GET_CMUtlWToUtf8(fl)  CAL_CMGETAPI( "CMUtlWToUtf8" ) 
	#define CAL_CMUtlWToUtf8  CMUtlWToUtf8
	#define CHK_CMUtlWToUtf8  TRUE
	#define EXP_CMUtlWToUtf8  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlWToUtf8", (RTS_UINTPTR)CMUtlWToUtf8, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlWToUtf8
	#define EXT_CMUtilsCMUtlWToUtf8
	#define GET_CMUtilsCMUtlWToUtf8  ERR_OK
	#define CAL_CMUtilsCMUtlWToUtf8 pICMUtils->ICMUtlWToUtf8
	#define CHK_CMUtilsCMUtlWToUtf8 (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlWToUtf8  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlWToUtf8
	#define EXT_CMUtlWToUtf8
	#define GET_CMUtlWToUtf8(fl)  CAL_CMGETAPI( "CMUtlWToUtf8" ) 
	#define CAL_CMUtlWToUtf8 pICMUtils->ICMUtlWToUtf8
	#define CHK_CMUtlWToUtf8 (pICMUtils != NULL)
	#define EXP_CMUtlWToUtf8  CAL_CMEXPAPI( "CMUtlWToUtf8" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlWToUtf8  PFCMUTLWTOUTF8 pfCMUtlWToUtf8;
	#define EXT_CMUtlWToUtf8  extern PFCMUTLWTOUTF8 pfCMUtlWToUtf8;
	#define GET_CMUtlWToUtf8(fl)  s_pfCMGetAPI2( "CMUtlWToUtf8", (RTS_VOID_FCTPTR *)&pfCMUtlWToUtf8, (fl), 0, 0)
	#define CAL_CMUtlWToUtf8  pfCMUtlWToUtf8
	#define CHK_CMUtlWToUtf8  (pfCMUtlWToUtf8 != NULL)
	#define EXP_CMUtlWToUtf8  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlWToUtf8", (RTS_UINTPTR)CMUtlWToUtf8, 0, 0) 
#endif




/**
 * <description>This function converts a UTF-8 string to a UTF-16 encoded WSTRING.
 * The conversion inserts the REPLACEMENT CHARACTER (U+FFFD) if a UTF-8 code-point or invalid.</description>
 * <param name="pUtf8Str" type="IN">Pointer to UTF-8 string to be converted.</param>
 * <param name="pUtf8BufferSize" type="IN">Read buffer size. Set to -1 if unknown.</param>
 * <param name="pwsz" type="IN">
 *     Pointer to WSTRING buffer.
 *     16-bit wide Unicode characters encoded as UTF-16LE.
 * </param>
 * <param name="wstrLen" type="IN">Number of WCHARs in pwsz.</param>
 * <result>On of these error codes: 
 *                                  - ERR_OK: Everything went fine. No errors occurred.
 *                                  - ERR_PARAMETER: The parameter checks failed.
 *                                  - ERR_CONVERSION_INCOMPLETE: The conversion failed because of some code-points. These where replaced with U+FFFD.
 * </result>
 */
RTS_RESULT CDECL CMUtlUtf8ToW(RTS_UI8* pUtf8Str, RTS_SIZE utf8BufferSize, RTS_WCHAR* pwsz, RTS_SIZE wstrLen);
typedef RTS_RESULT (CDECL * PFCMUTLUTF8TOW) (RTS_UI8* pUtf8Str, RTS_SIZE utf8BufferSize, RTS_WCHAR* pwsz, RTS_SIZE wstrLen);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLUTF8TOW_NOTIMPLEMENTED)
	#define USE_CMUtlUtf8ToW
	#define EXT_CMUtlUtf8ToW
	#define GET_CMUtlUtf8ToW(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlUtf8ToW(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlUtf8ToW  FALSE
	#define EXP_CMUtlUtf8ToW  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlUtf8ToW
	#define EXT_CMUtlUtf8ToW
	#define GET_CMUtlUtf8ToW(fl)  CAL_CMGETAPI( "CMUtlUtf8ToW" ) 
	#define CAL_CMUtlUtf8ToW  CMUtlUtf8ToW
	#define CHK_CMUtlUtf8ToW  TRUE
	#define EXP_CMUtlUtf8ToW  CAL_CMEXPAPI( "CMUtlUtf8ToW" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlUtf8ToW
	#define EXT_CMUtlUtf8ToW
	#define GET_CMUtlUtf8ToW(fl)  CAL_CMGETAPI( "CMUtlUtf8ToW" ) 
	#define CAL_CMUtlUtf8ToW  CMUtlUtf8ToW
	#define CHK_CMUtlUtf8ToW  TRUE
	#define EXP_CMUtlUtf8ToW  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlUtf8ToW", (RTS_UINTPTR)CMUtlUtf8ToW, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlUtf8ToW
	#define EXT_CMUtilsCMUtlUtf8ToW
	#define GET_CMUtilsCMUtlUtf8ToW  ERR_OK
	#define CAL_CMUtilsCMUtlUtf8ToW pICMUtils->ICMUtlUtf8ToW
	#define CHK_CMUtilsCMUtlUtf8ToW (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlUtf8ToW  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlUtf8ToW
	#define EXT_CMUtlUtf8ToW
	#define GET_CMUtlUtf8ToW(fl)  CAL_CMGETAPI( "CMUtlUtf8ToW" ) 
	#define CAL_CMUtlUtf8ToW pICMUtils->ICMUtlUtf8ToW
	#define CHK_CMUtlUtf8ToW (pICMUtils != NULL)
	#define EXP_CMUtlUtf8ToW  CAL_CMEXPAPI( "CMUtlUtf8ToW" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlUtf8ToW  PFCMUTLUTF8TOW pfCMUtlUtf8ToW;
	#define EXT_CMUtlUtf8ToW  extern PFCMUTLUTF8TOW pfCMUtlUtf8ToW;
	#define GET_CMUtlUtf8ToW(fl)  s_pfCMGetAPI2( "CMUtlUtf8ToW", (RTS_VOID_FCTPTR *)&pfCMUtlUtf8ToW, (fl), 0, 0)
	#define CAL_CMUtlUtf8ToW  pfCMUtlUtf8ToW
	#define CHK_CMUtlUtf8ToW  (pfCMUtlUtf8ToW != NULL)
	#define EXP_CMUtlUtf8ToW  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlUtf8ToW", (RTS_UINTPTR)CMUtlUtf8ToW, 0, 0) 
#endif




/**
 * <description>This function checks if a UTF-8 string is valid UTF-8.</description>
 * <result>On of these error codes: 
 *                                  - ERR_OK: Everything went fine. No errors occurred.
 *                                  - ERR_PARAMETER: The parameter checks failed.
 *                                  - ERR_INVALID_ENCODING: The UTF-8 string is not valid UTF-8.
 * </result>
 */
RTS_RESULT CDECL CMUtlCheckUtf8Encoding(RTS_UI8 *pUtf8Str, RTS_SIZE utf8BufferSize);
typedef RTS_RESULT (CDECL * PFCMUTLCHECKUTF8ENCODING) (RTS_UI8 *pUtf8Str, RTS_SIZE utf8BufferSize);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCHECKUTF8ENCODING_NOTIMPLEMENTED)
	#define USE_CMUtlCheckUtf8Encoding
	#define EXT_CMUtlCheckUtf8Encoding
	#define GET_CMUtlCheckUtf8Encoding(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlCheckUtf8Encoding(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlCheckUtf8Encoding  FALSE
	#define EXP_CMUtlCheckUtf8Encoding  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlCheckUtf8Encoding
	#define EXT_CMUtlCheckUtf8Encoding
	#define GET_CMUtlCheckUtf8Encoding(fl)  CAL_CMGETAPI( "CMUtlCheckUtf8Encoding" ) 
	#define CAL_CMUtlCheckUtf8Encoding  CMUtlCheckUtf8Encoding
	#define CHK_CMUtlCheckUtf8Encoding  TRUE
	#define EXP_CMUtlCheckUtf8Encoding  CAL_CMEXPAPI( "CMUtlCheckUtf8Encoding" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlCheckUtf8Encoding
	#define EXT_CMUtlCheckUtf8Encoding
	#define GET_CMUtlCheckUtf8Encoding(fl)  CAL_CMGETAPI( "CMUtlCheckUtf8Encoding" ) 
	#define CAL_CMUtlCheckUtf8Encoding  CMUtlCheckUtf8Encoding
	#define CHK_CMUtlCheckUtf8Encoding  TRUE
	#define EXP_CMUtlCheckUtf8Encoding  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlCheckUtf8Encoding", (RTS_UINTPTR)CMUtlCheckUtf8Encoding, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlCheckUtf8Encoding
	#define EXT_CMUtilsCMUtlCheckUtf8Encoding
	#define GET_CMUtilsCMUtlCheckUtf8Encoding  ERR_OK
	#define CAL_CMUtilsCMUtlCheckUtf8Encoding pICMUtils->ICMUtlCheckUtf8Encoding
	#define CHK_CMUtilsCMUtlCheckUtf8Encoding (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlCheckUtf8Encoding  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlCheckUtf8Encoding
	#define EXT_CMUtlCheckUtf8Encoding
	#define GET_CMUtlCheckUtf8Encoding(fl)  CAL_CMGETAPI( "CMUtlCheckUtf8Encoding" ) 
	#define CAL_CMUtlCheckUtf8Encoding pICMUtils->ICMUtlCheckUtf8Encoding
	#define CHK_CMUtlCheckUtf8Encoding (pICMUtils != NULL)
	#define EXP_CMUtlCheckUtf8Encoding  CAL_CMEXPAPI( "CMUtlCheckUtf8Encoding" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlCheckUtf8Encoding  PFCMUTLCHECKUTF8ENCODING pfCMUtlCheckUtf8Encoding;
	#define EXT_CMUtlCheckUtf8Encoding  extern PFCMUTLCHECKUTF8ENCODING pfCMUtlCheckUtf8Encoding;
	#define GET_CMUtlCheckUtf8Encoding(fl)  s_pfCMGetAPI2( "CMUtlCheckUtf8Encoding", (RTS_VOID_FCTPTR *)&pfCMUtlCheckUtf8Encoding, (fl), 0, 0)
	#define CAL_CMUtlCheckUtf8Encoding  pfCMUtlCheckUtf8Encoding
	#define CHK_CMUtlCheckUtf8Encoding  (pfCMUtlCheckUtf8Encoding != NULL)
	#define EXP_CMUtlCheckUtf8Encoding  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlCheckUtf8Encoding", (RTS_UINTPTR)CMUtlCheckUtf8Encoding, 0, 0) 
#endif




/**
 *************************************************************************
 * <description> 
 *  Variable string type routines. Can be UNICODE or C-strings, dependent
 *	of the type definition of RTS_CWCHAR!
 * </description>
 *************************************************************************/

/**
 * <description> 
 *  Compare two RTS_CWCHAR strings
 * </description>
 * <param name="pwsz1" type="IN">Pointer to string 1</param>
 * <param name="pwsz2" type="IN">Pointer to string 2</param>
 * <result>
 *		0: string1 identical to string2
 *		lower 0: string1 less than string2
 *		greater 0: string1 greater than string2
 * </result>
 */
int CDECL CMUtlcwstrcmp(const RTS_CWCHAR *pwsz1, const RTS_CWCHAR *pwsz2);
typedef int (CDECL * PFCMUTLCWSTRCMP) (const RTS_CWCHAR *pwsz1, const RTS_CWCHAR *pwsz2);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCWSTRCMP_NOTIMPLEMENTED)
	#define USE_CMUtlcwstrcmp
	#define EXT_CMUtlcwstrcmp
	#define GET_CMUtlcwstrcmp(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlcwstrcmp(p0,p1)  (int)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlcwstrcmp  FALSE
	#define EXP_CMUtlcwstrcmp  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlcwstrcmp
	#define EXT_CMUtlcwstrcmp
	#define GET_CMUtlcwstrcmp(fl)  CAL_CMGETAPI( "CMUtlcwstrcmp" ) 
	#define CAL_CMUtlcwstrcmp  CMUtlcwstrcmp
	#define CHK_CMUtlcwstrcmp  TRUE
	#define EXP_CMUtlcwstrcmp  CAL_CMEXPAPI( "CMUtlcwstrcmp" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlcwstrcmp
	#define EXT_CMUtlcwstrcmp
	#define GET_CMUtlcwstrcmp(fl)  CAL_CMGETAPI( "CMUtlcwstrcmp" ) 
	#define CAL_CMUtlcwstrcmp  CMUtlcwstrcmp
	#define CHK_CMUtlcwstrcmp  TRUE
	#define EXP_CMUtlcwstrcmp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrcmp", (RTS_UINTPTR)CMUtlcwstrcmp, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlcwstrcmp
	#define EXT_CMUtilsCMUtlcwstrcmp
	#define GET_CMUtilsCMUtlcwstrcmp  ERR_OK
	#define CAL_CMUtilsCMUtlcwstrcmp pICMUtils->ICMUtlcwstrcmp
	#define CHK_CMUtilsCMUtlcwstrcmp (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlcwstrcmp  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlcwstrcmp
	#define EXT_CMUtlcwstrcmp
	#define GET_CMUtlcwstrcmp(fl)  CAL_CMGETAPI( "CMUtlcwstrcmp" ) 
	#define CAL_CMUtlcwstrcmp pICMUtils->ICMUtlcwstrcmp
	#define CHK_CMUtlcwstrcmp (pICMUtils != NULL)
	#define EXP_CMUtlcwstrcmp  CAL_CMEXPAPI( "CMUtlcwstrcmp" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlcwstrcmp  PFCMUTLCWSTRCMP pfCMUtlcwstrcmp;
	#define EXT_CMUtlcwstrcmp  extern PFCMUTLCWSTRCMP pfCMUtlcwstrcmp;
	#define GET_CMUtlcwstrcmp(fl)  s_pfCMGetAPI2( "CMUtlcwstrcmp", (RTS_VOID_FCTPTR *)&pfCMUtlcwstrcmp, (fl), 0, 0)
	#define CAL_CMUtlcwstrcmp  pfCMUtlcwstrcmp
	#define CHK_CMUtlcwstrcmp  (pfCMUtlcwstrcmp != NULL)
	#define EXP_CMUtlcwstrcmp  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrcmp", (RTS_UINTPTR)CMUtlcwstrcmp, 0, 0) 
#endif




/**
 * <description> 
 *  Copy one RTS_CWCHAR string to another in a safe way
 * </description>
 * <param name="pwszDest" type="IN">Destination string</param>
 * <param name="nDestSize" type="IN">Size of the destination buffer in RTS_CWCHAR characters!</param>
 * <param name="pwszSrc" type="IN">Source string</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlcwstrcpy(RTS_CWCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_CWCHAR *pwszSrc);
typedef RTS_RESULT (CDECL * PFCMUTLCWSTRCPY) (RTS_CWCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_CWCHAR *pwszSrc);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCWSTRCPY_NOTIMPLEMENTED)
	#define USE_CMUtlcwstrcpy
	#define EXT_CMUtlcwstrcpy
	#define GET_CMUtlcwstrcpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlcwstrcpy(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlcwstrcpy  FALSE
	#define EXP_CMUtlcwstrcpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlcwstrcpy
	#define EXT_CMUtlcwstrcpy
	#define GET_CMUtlcwstrcpy(fl)  CAL_CMGETAPI( "CMUtlcwstrcpy" ) 
	#define CAL_CMUtlcwstrcpy  CMUtlcwstrcpy
	#define CHK_CMUtlcwstrcpy  TRUE
	#define EXP_CMUtlcwstrcpy  CAL_CMEXPAPI( "CMUtlcwstrcpy" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlcwstrcpy
	#define EXT_CMUtlcwstrcpy
	#define GET_CMUtlcwstrcpy(fl)  CAL_CMGETAPI( "CMUtlcwstrcpy" ) 
	#define CAL_CMUtlcwstrcpy  CMUtlcwstrcpy
	#define CHK_CMUtlcwstrcpy  TRUE
	#define EXP_CMUtlcwstrcpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrcpy", (RTS_UINTPTR)CMUtlcwstrcpy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlcwstrcpy
	#define EXT_CMUtilsCMUtlcwstrcpy
	#define GET_CMUtilsCMUtlcwstrcpy  ERR_OK
	#define CAL_CMUtilsCMUtlcwstrcpy pICMUtils->ICMUtlcwstrcpy
	#define CHK_CMUtilsCMUtlcwstrcpy (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlcwstrcpy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlcwstrcpy
	#define EXT_CMUtlcwstrcpy
	#define GET_CMUtlcwstrcpy(fl)  CAL_CMGETAPI( "CMUtlcwstrcpy" ) 
	#define CAL_CMUtlcwstrcpy pICMUtils->ICMUtlcwstrcpy
	#define CHK_CMUtlcwstrcpy (pICMUtils != NULL)
	#define EXP_CMUtlcwstrcpy  CAL_CMEXPAPI( "CMUtlcwstrcpy" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlcwstrcpy  PFCMUTLCWSTRCPY pfCMUtlcwstrcpy;
	#define EXT_CMUtlcwstrcpy  extern PFCMUTLCWSTRCPY pfCMUtlcwstrcpy;
	#define GET_CMUtlcwstrcpy(fl)  s_pfCMGetAPI2( "CMUtlcwstrcpy", (RTS_VOID_FCTPTR *)&pfCMUtlcwstrcpy, (fl), 0, 0)
	#define CAL_CMUtlcwstrcpy  pfCMUtlcwstrcpy
	#define CHK_CMUtlcwstrcpy  (pfCMUtlcwstrcpy != NULL)
	#define EXP_CMUtlcwstrcpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrcpy", (RTS_UINTPTR)CMUtlcwstrcpy, 0, 0) 
#endif




/**
 * <description> 
 *  Concatenate two RTS_CWCHAR strings in a safe way
 * </description>
 * <param name="pwszDest" type="IN">Destination string</param>
 * <param name="nDestSize" type="IN">Size of the destination buffer in RTS_CWCHAR characters!</param>
 * <param name="pwszSrc" type="IN">Source string</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlcwstrcat(RTS_CWCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_CWCHAR *pwszSrc);
typedef RTS_RESULT (CDECL * PFCMUTLCWSTRCAT) (RTS_CWCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_CWCHAR *pwszSrc);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCWSTRCAT_NOTIMPLEMENTED)
	#define USE_CMUtlcwstrcat
	#define EXT_CMUtlcwstrcat
	#define GET_CMUtlcwstrcat(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlcwstrcat(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlcwstrcat  FALSE
	#define EXP_CMUtlcwstrcat  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlcwstrcat
	#define EXT_CMUtlcwstrcat
	#define GET_CMUtlcwstrcat(fl)  CAL_CMGETAPI( "CMUtlcwstrcat" ) 
	#define CAL_CMUtlcwstrcat  CMUtlcwstrcat
	#define CHK_CMUtlcwstrcat  TRUE
	#define EXP_CMUtlcwstrcat  CAL_CMEXPAPI( "CMUtlcwstrcat" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlcwstrcat
	#define EXT_CMUtlcwstrcat
	#define GET_CMUtlcwstrcat(fl)  CAL_CMGETAPI( "CMUtlcwstrcat" ) 
	#define CAL_CMUtlcwstrcat  CMUtlcwstrcat
	#define CHK_CMUtlcwstrcat  TRUE
	#define EXP_CMUtlcwstrcat  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrcat", (RTS_UINTPTR)CMUtlcwstrcat, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlcwstrcat
	#define EXT_CMUtilsCMUtlcwstrcat
	#define GET_CMUtilsCMUtlcwstrcat  ERR_OK
	#define CAL_CMUtilsCMUtlcwstrcat pICMUtils->ICMUtlcwstrcat
	#define CHK_CMUtilsCMUtlcwstrcat (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlcwstrcat  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlcwstrcat
	#define EXT_CMUtlcwstrcat
	#define GET_CMUtlcwstrcat(fl)  CAL_CMGETAPI( "CMUtlcwstrcat" ) 
	#define CAL_CMUtlcwstrcat pICMUtils->ICMUtlcwstrcat
	#define CHK_CMUtlcwstrcat (pICMUtils != NULL)
	#define EXP_CMUtlcwstrcat  CAL_CMEXPAPI( "CMUtlcwstrcat" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlcwstrcat  PFCMUTLCWSTRCAT pfCMUtlcwstrcat;
	#define EXT_CMUtlcwstrcat  extern PFCMUTLCWSTRCAT pfCMUtlcwstrcat;
	#define GET_CMUtlcwstrcat(fl)  s_pfCMGetAPI2( "CMUtlcwstrcat", (RTS_VOID_FCTPTR *)&pfCMUtlcwstrcat, (fl), 0, 0)
	#define CAL_CMUtlcwstrcat  pfCMUtlcwstrcat
	#define CHK_CMUtlcwstrcat  (pfCMUtlcwstrcat != NULL)
	#define EXP_CMUtlcwstrcat  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrcat", (RTS_UINTPTR)CMUtlcwstrcat, 0, 0) 
#endif




/**
 * <description> 
 *  Copy number of characters from one RTS_CWCHAR string to another in a safe way
 * </description>
 * <param name="pwszDest" type="IN">Destination string</param>
 * <param name="nDestSize" type="IN">Size of the destination buffer in RTS_CWCHAR characters!</param>
 * <param name="pwszSrc" type="IN">Source string</param>
 * <param name="n" type="IN">Number of characters to copy (not bytes)!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlcwstrncpy(RTS_CWCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_CWCHAR *pwszSrc, RTS_SIZE n);
typedef RTS_RESULT (CDECL * PFCMUTLCWSTRNCPY) (RTS_CWCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_CWCHAR *pwszSrc, RTS_SIZE n);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCWSTRNCPY_NOTIMPLEMENTED)
	#define USE_CMUtlcwstrncpy
	#define EXT_CMUtlcwstrncpy
	#define GET_CMUtlcwstrncpy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlcwstrncpy(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlcwstrncpy  FALSE
	#define EXP_CMUtlcwstrncpy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlcwstrncpy
	#define EXT_CMUtlcwstrncpy
	#define GET_CMUtlcwstrncpy(fl)  CAL_CMGETAPI( "CMUtlcwstrncpy" ) 
	#define CAL_CMUtlcwstrncpy  CMUtlcwstrncpy
	#define CHK_CMUtlcwstrncpy  TRUE
	#define EXP_CMUtlcwstrncpy  CAL_CMEXPAPI( "CMUtlcwstrncpy" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlcwstrncpy
	#define EXT_CMUtlcwstrncpy
	#define GET_CMUtlcwstrncpy(fl)  CAL_CMGETAPI( "CMUtlcwstrncpy" ) 
	#define CAL_CMUtlcwstrncpy  CMUtlcwstrncpy
	#define CHK_CMUtlcwstrncpy  TRUE
	#define EXP_CMUtlcwstrncpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrncpy", (RTS_UINTPTR)CMUtlcwstrncpy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlcwstrncpy
	#define EXT_CMUtilsCMUtlcwstrncpy
	#define GET_CMUtilsCMUtlcwstrncpy  ERR_OK
	#define CAL_CMUtilsCMUtlcwstrncpy pICMUtils->ICMUtlcwstrncpy
	#define CHK_CMUtilsCMUtlcwstrncpy (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlcwstrncpy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlcwstrncpy
	#define EXT_CMUtlcwstrncpy
	#define GET_CMUtlcwstrncpy(fl)  CAL_CMGETAPI( "CMUtlcwstrncpy" ) 
	#define CAL_CMUtlcwstrncpy pICMUtils->ICMUtlcwstrncpy
	#define CHK_CMUtlcwstrncpy (pICMUtils != NULL)
	#define EXP_CMUtlcwstrncpy  CAL_CMEXPAPI( "CMUtlcwstrncpy" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlcwstrncpy  PFCMUTLCWSTRNCPY pfCMUtlcwstrncpy;
	#define EXT_CMUtlcwstrncpy  extern PFCMUTLCWSTRNCPY pfCMUtlcwstrncpy;
	#define GET_CMUtlcwstrncpy(fl)  s_pfCMGetAPI2( "CMUtlcwstrncpy", (RTS_VOID_FCTPTR *)&pfCMUtlcwstrncpy, (fl), 0, 0)
	#define CAL_CMUtlcwstrncpy  pfCMUtlcwstrncpy
	#define CHK_CMUtlcwstrncpy  (pfCMUtlcwstrncpy != NULL)
	#define EXP_CMUtlcwstrncpy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrncpy", (RTS_UINTPTR)CMUtlcwstrncpy, 0, 0) 
#endif




/**
 * <description> 
 *  Get the number of RTS_CWCHAR characters in the content of a string _without_ NUL terminating character.
 * </description>
 * <param name="pwsz" type="IN">Pointer to string</param>
 * <result>Number of RTS_CWCHAR characters in the content of a string _without_ NUL terminating character</result>
 */
RTS_SIZE CDECL CMUtlcwstrlen(const RTS_CWCHAR *pwsz);
typedef RTS_SIZE (CDECL * PFCMUTLCWSTRLEN) (const RTS_CWCHAR *pwsz);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCWSTRLEN_NOTIMPLEMENTED)
	#define USE_CMUtlcwstrlen
	#define EXT_CMUtlcwstrlen
	#define GET_CMUtlcwstrlen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlcwstrlen(p0)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlcwstrlen  FALSE
	#define EXP_CMUtlcwstrlen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlcwstrlen
	#define EXT_CMUtlcwstrlen
	#define GET_CMUtlcwstrlen(fl)  CAL_CMGETAPI( "CMUtlcwstrlen" ) 
	#define CAL_CMUtlcwstrlen  CMUtlcwstrlen
	#define CHK_CMUtlcwstrlen  TRUE
	#define EXP_CMUtlcwstrlen  CAL_CMEXPAPI( "CMUtlcwstrlen" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlcwstrlen
	#define EXT_CMUtlcwstrlen
	#define GET_CMUtlcwstrlen(fl)  CAL_CMGETAPI( "CMUtlcwstrlen" ) 
	#define CAL_CMUtlcwstrlen  CMUtlcwstrlen
	#define CHK_CMUtlcwstrlen  TRUE
	#define EXP_CMUtlcwstrlen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrlen", (RTS_UINTPTR)CMUtlcwstrlen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlcwstrlen
	#define EXT_CMUtilsCMUtlcwstrlen
	#define GET_CMUtilsCMUtlcwstrlen  ERR_OK
	#define CAL_CMUtilsCMUtlcwstrlen pICMUtils->ICMUtlcwstrlen
	#define CHK_CMUtilsCMUtlcwstrlen (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlcwstrlen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlcwstrlen
	#define EXT_CMUtlcwstrlen
	#define GET_CMUtlcwstrlen(fl)  CAL_CMGETAPI( "CMUtlcwstrlen" ) 
	#define CAL_CMUtlcwstrlen pICMUtils->ICMUtlcwstrlen
	#define CHK_CMUtlcwstrlen (pICMUtils != NULL)
	#define EXP_CMUtlcwstrlen  CAL_CMEXPAPI( "CMUtlcwstrlen" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlcwstrlen  PFCMUTLCWSTRLEN pfCMUtlcwstrlen;
	#define EXT_CMUtlcwstrlen  extern PFCMUTLCWSTRLEN pfCMUtlcwstrlen;
	#define GET_CMUtlcwstrlen(fl)  s_pfCMGetAPI2( "CMUtlcwstrlen", (RTS_VOID_FCTPTR *)&pfCMUtlcwstrlen, (fl), 0, 0)
	#define CAL_CMUtlcwstrlen  pfCMUtlcwstrlen
	#define CHK_CMUtlcwstrlen  (pfCMUtlcwstrlen != NULL)
	#define EXP_CMUtlcwstrlen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrlen", (RTS_UINTPTR)CMUtlcwstrlen, 0, 0) 
#endif




/**
 * <description> 
 *  Convert the characters in a RTS_CWCHAR string to lower case.
 * </description>
 * <param name="pwsz" type="IN">Pointer to string</param>
 * <result>pwsz is returned</result>
 */
RTS_CWCHAR* CDECL CMUtlcwtolower(RTS_CWCHAR* pwsz);
typedef RTS_CWCHAR* (CDECL * PFCMUTLCWTOLOWER) (RTS_CWCHAR* pwsz);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCWTOLOWER_NOTIMPLEMENTED)
	#define USE_CMUtlcwtolower
	#define EXT_CMUtlcwtolower
	#define GET_CMUtlcwtolower(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlcwtolower(p0)  (RTS_CWCHAR*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlcwtolower  FALSE
	#define EXP_CMUtlcwtolower  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlcwtolower
	#define EXT_CMUtlcwtolower
	#define GET_CMUtlcwtolower(fl)  CAL_CMGETAPI( "CMUtlcwtolower" ) 
	#define CAL_CMUtlcwtolower  CMUtlcwtolower
	#define CHK_CMUtlcwtolower  TRUE
	#define EXP_CMUtlcwtolower  CAL_CMEXPAPI( "CMUtlcwtolower" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlcwtolower
	#define EXT_CMUtlcwtolower
	#define GET_CMUtlcwtolower(fl)  CAL_CMGETAPI( "CMUtlcwtolower" ) 
	#define CAL_CMUtlcwtolower  CMUtlcwtolower
	#define CHK_CMUtlcwtolower  TRUE
	#define EXP_CMUtlcwtolower  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwtolower", (RTS_UINTPTR)CMUtlcwtolower, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlcwtolower
	#define EXT_CMUtilsCMUtlcwtolower
	#define GET_CMUtilsCMUtlcwtolower  ERR_OK
	#define CAL_CMUtilsCMUtlcwtolower pICMUtils->ICMUtlcwtolower
	#define CHK_CMUtilsCMUtlcwtolower (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlcwtolower  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlcwtolower
	#define EXT_CMUtlcwtolower
	#define GET_CMUtlcwtolower(fl)  CAL_CMGETAPI( "CMUtlcwtolower" ) 
	#define CAL_CMUtlcwtolower pICMUtils->ICMUtlcwtolower
	#define CHK_CMUtlcwtolower (pICMUtils != NULL)
	#define EXP_CMUtlcwtolower  CAL_CMEXPAPI( "CMUtlcwtolower" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlcwtolower  PFCMUTLCWTOLOWER pfCMUtlcwtolower;
	#define EXT_CMUtlcwtolower  extern PFCMUTLCWTOLOWER pfCMUtlcwtolower;
	#define GET_CMUtlcwtolower(fl)  s_pfCMGetAPI2( "CMUtlcwtolower", (RTS_VOID_FCTPTR *)&pfCMUtlcwtolower, (fl), 0, 0)
	#define CAL_CMUtlcwtolower  pfCMUtlcwtolower
	#define CHK_CMUtlcwtolower  (pfCMUtlcwtolower != NULL)
	#define EXP_CMUtlcwtolower  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwtolower", (RTS_UINTPTR)CMUtlcwtolower, 0, 0) 
#endif




/**
 * <description> 
 *  Convert the characters in a RTS_CWCHAR string to upper case.
 * </description>
 * <param name="pwsz" type="IN">Pointer to string</param>
 * <result>pwsz is returned</result>
 */
RTS_CWCHAR* CDECL CMUtlcwtoupper(RTS_CWCHAR* pwsz);
typedef RTS_CWCHAR* (CDECL * PFCMUTLCWTOUPPER) (RTS_CWCHAR* pwsz);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCWTOUPPER_NOTIMPLEMENTED)
	#define USE_CMUtlcwtoupper
	#define EXT_CMUtlcwtoupper
	#define GET_CMUtlcwtoupper(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlcwtoupper(p0)  (RTS_CWCHAR*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlcwtoupper  FALSE
	#define EXP_CMUtlcwtoupper  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlcwtoupper
	#define EXT_CMUtlcwtoupper
	#define GET_CMUtlcwtoupper(fl)  CAL_CMGETAPI( "CMUtlcwtoupper" ) 
	#define CAL_CMUtlcwtoupper  CMUtlcwtoupper
	#define CHK_CMUtlcwtoupper  TRUE
	#define EXP_CMUtlcwtoupper  CAL_CMEXPAPI( "CMUtlcwtoupper" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlcwtoupper
	#define EXT_CMUtlcwtoupper
	#define GET_CMUtlcwtoupper(fl)  CAL_CMGETAPI( "CMUtlcwtoupper" ) 
	#define CAL_CMUtlcwtoupper  CMUtlcwtoupper
	#define CHK_CMUtlcwtoupper  TRUE
	#define EXP_CMUtlcwtoupper  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwtoupper", (RTS_UINTPTR)CMUtlcwtoupper, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlcwtoupper
	#define EXT_CMUtilsCMUtlcwtoupper
	#define GET_CMUtilsCMUtlcwtoupper  ERR_OK
	#define CAL_CMUtilsCMUtlcwtoupper pICMUtils->ICMUtlcwtoupper
	#define CHK_CMUtilsCMUtlcwtoupper (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlcwtoupper  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlcwtoupper
	#define EXT_CMUtlcwtoupper
	#define GET_CMUtlcwtoupper(fl)  CAL_CMGETAPI( "CMUtlcwtoupper" ) 
	#define CAL_CMUtlcwtoupper pICMUtils->ICMUtlcwtoupper
	#define CHK_CMUtlcwtoupper (pICMUtils != NULL)
	#define EXP_CMUtlcwtoupper  CAL_CMEXPAPI( "CMUtlcwtoupper" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlcwtoupper  PFCMUTLCWTOUPPER pfCMUtlcwtoupper;
	#define EXT_CMUtlcwtoupper  extern PFCMUTLCWTOUPPER pfCMUtlcwtoupper;
	#define GET_CMUtlcwtoupper(fl)  s_pfCMGetAPI2( "CMUtlcwtoupper", (RTS_VOID_FCTPTR *)&pfCMUtlcwtoupper, (fl), 0, 0)
	#define CAL_CMUtlcwtoupper  pfCMUtlcwtoupper
	#define CHK_CMUtlcwtoupper  (pfCMUtlcwtoupper != NULL)
	#define EXP_CMUtlcwtoupper  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwtoupper", (RTS_UINTPTR)CMUtlcwtoupper, 0, 0) 
#endif




/**
 * <description> 
 *  Convert a byte character string to a RTS_CWCHAR character string.
 * </description>
 * <param name="pszSrc" type="IN">Pointer to byte character string</param>
 * <param name="pwszDest" type="IN">Pointer to RTS_CWCHAR character string</param>
 * <param name="nDestSize" type="IN">Size of the destination buffer in RTS_CWCHAR characters!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlStrToCW(const char* pszSrc, RTS_CWCHAR* pwszDest, RTS_SIZE nDestSize);
typedef RTS_RESULT (CDECL * PFCMUTLSTRTOCW) (const char* pszSrc, RTS_CWCHAR* pwszDest, RTS_SIZE nDestSize);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLSTRTOCW_NOTIMPLEMENTED)
	#define USE_CMUtlStrToCW
	#define EXT_CMUtlStrToCW
	#define GET_CMUtlStrToCW(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlStrToCW(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlStrToCW  FALSE
	#define EXP_CMUtlStrToCW  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlStrToCW
	#define EXT_CMUtlStrToCW
	#define GET_CMUtlStrToCW(fl)  CAL_CMGETAPI( "CMUtlStrToCW" ) 
	#define CAL_CMUtlStrToCW  CMUtlStrToCW
	#define CHK_CMUtlStrToCW  TRUE
	#define EXP_CMUtlStrToCW  CAL_CMEXPAPI( "CMUtlStrToCW" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlStrToCW
	#define EXT_CMUtlStrToCW
	#define GET_CMUtlStrToCW(fl)  CAL_CMGETAPI( "CMUtlStrToCW" ) 
	#define CAL_CMUtlStrToCW  CMUtlStrToCW
	#define CHK_CMUtlStrToCW  TRUE
	#define EXP_CMUtlStrToCW  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrToCW", (RTS_UINTPTR)CMUtlStrToCW, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlStrToCW
	#define EXT_CMUtilsCMUtlStrToCW
	#define GET_CMUtilsCMUtlStrToCW  ERR_OK
	#define CAL_CMUtilsCMUtlStrToCW pICMUtils->ICMUtlStrToCW
	#define CHK_CMUtilsCMUtlStrToCW (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlStrToCW  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlStrToCW
	#define EXT_CMUtlStrToCW
	#define GET_CMUtlStrToCW(fl)  CAL_CMGETAPI( "CMUtlStrToCW" ) 
	#define CAL_CMUtlStrToCW pICMUtils->ICMUtlStrToCW
	#define CHK_CMUtlStrToCW (pICMUtils != NULL)
	#define EXP_CMUtlStrToCW  CAL_CMEXPAPI( "CMUtlStrToCW" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlStrToCW  PFCMUTLSTRTOCW pfCMUtlStrToCW;
	#define EXT_CMUtlStrToCW  extern PFCMUTLSTRTOCW pfCMUtlStrToCW;
	#define GET_CMUtlStrToCW(fl)  s_pfCMGetAPI2( "CMUtlStrToCW", (RTS_VOID_FCTPTR *)&pfCMUtlStrToCW, (fl), 0, 0)
	#define CAL_CMUtlStrToCW  pfCMUtlStrToCW
	#define CHK_CMUtlStrToCW  (pfCMUtlStrToCW != NULL)
	#define EXP_CMUtlStrToCW  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlStrToCW", (RTS_UINTPTR)CMUtlStrToCW, 0, 0) 
#endif




/**
 * <description> 
 *  Convert a RTS_CWCHAR character string to a byte character string .
 * </description>
 * <param name="pwszSrc" type="IN">Pointer to RTS_CWCHAR character string</param>
 * <param name="pszDest" type="IN">Pointer to byte character string</param>
 * <param name="nDestSize" type="IN">Size of the destination buffer in bytes</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlCWToStr(const RTS_CWCHAR* pwszSrc, char* pszDest, RTS_SIZE nDestSize);
typedef RTS_RESULT (CDECL * PFCMUTLCWTOSTR) (const RTS_CWCHAR* pwszSrc, char* pszDest, RTS_SIZE nDestSize);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCWTOSTR_NOTIMPLEMENTED)
	#define USE_CMUtlCWToStr
	#define EXT_CMUtlCWToStr
	#define GET_CMUtlCWToStr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlCWToStr(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlCWToStr  FALSE
	#define EXP_CMUtlCWToStr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlCWToStr
	#define EXT_CMUtlCWToStr
	#define GET_CMUtlCWToStr(fl)  CAL_CMGETAPI( "CMUtlCWToStr" ) 
	#define CAL_CMUtlCWToStr  CMUtlCWToStr
	#define CHK_CMUtlCWToStr  TRUE
	#define EXP_CMUtlCWToStr  CAL_CMEXPAPI( "CMUtlCWToStr" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlCWToStr
	#define EXT_CMUtlCWToStr
	#define GET_CMUtlCWToStr(fl)  CAL_CMGETAPI( "CMUtlCWToStr" ) 
	#define CAL_CMUtlCWToStr  CMUtlCWToStr
	#define CHK_CMUtlCWToStr  TRUE
	#define EXP_CMUtlCWToStr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlCWToStr", (RTS_UINTPTR)CMUtlCWToStr, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlCWToStr
	#define EXT_CMUtilsCMUtlCWToStr
	#define GET_CMUtilsCMUtlCWToStr  ERR_OK
	#define CAL_CMUtilsCMUtlCWToStr pICMUtils->ICMUtlCWToStr
	#define CHK_CMUtilsCMUtlCWToStr (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlCWToStr  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlCWToStr
	#define EXT_CMUtlCWToStr
	#define GET_CMUtlCWToStr(fl)  CAL_CMGETAPI( "CMUtlCWToStr" ) 
	#define CAL_CMUtlCWToStr pICMUtils->ICMUtlCWToStr
	#define CHK_CMUtlCWToStr (pICMUtils != NULL)
	#define EXP_CMUtlCWToStr  CAL_CMEXPAPI( "CMUtlCWToStr" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlCWToStr  PFCMUTLCWTOSTR pfCMUtlCWToStr;
	#define EXT_CMUtlCWToStr  extern PFCMUTLCWTOSTR pfCMUtlCWToStr;
	#define GET_CMUtlCWToStr(fl)  s_pfCMGetAPI2( "CMUtlCWToStr", (RTS_VOID_FCTPTR *)&pfCMUtlCWToStr, (fl), 0, 0)
	#define CAL_CMUtlCWToStr  pfCMUtlCWToStr
	#define CHK_CMUtlCWToStr  (pfCMUtlCWToStr != NULL)
	#define EXP_CMUtlCWToStr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlCWToStr", (RTS_UINTPTR)CMUtlCWToStr, 0, 0) 
#endif




/**
 * <description> 
 *  Find a character in a RTS_CWCHAR string.
 * </description>
 * <param name="pwsz" type="IN">Pointer to RTS_CWCHAR string</param>
 * <param name="w" type="IN">Character to find</param>
 * <result>Returns the find position or NULL if not found</result>
 */
RTS_CWCHAR* CDECL CMUtlcwstrchr(const RTS_CWCHAR *pwsz, RTS_CWCHAR cw);
typedef RTS_CWCHAR* (CDECL * PFCMUTLCWSTRCHR) (const RTS_CWCHAR *pwsz, RTS_CWCHAR cw);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLCWSTRCHR_NOTIMPLEMENTED)
	#define USE_CMUtlcwstrchr
	#define EXT_CMUtlcwstrchr
	#define GET_CMUtlcwstrchr(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlcwstrchr(p0,p1)  (RTS_CWCHAR*)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlcwstrchr  FALSE
	#define EXP_CMUtlcwstrchr  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlcwstrchr
	#define EXT_CMUtlcwstrchr
	#define GET_CMUtlcwstrchr(fl)  CAL_CMGETAPI( "CMUtlcwstrchr" ) 
	#define CAL_CMUtlcwstrchr  CMUtlcwstrchr
	#define CHK_CMUtlcwstrchr  TRUE
	#define EXP_CMUtlcwstrchr  CAL_CMEXPAPI( "CMUtlcwstrchr" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlcwstrchr
	#define EXT_CMUtlcwstrchr
	#define GET_CMUtlcwstrchr(fl)  CAL_CMGETAPI( "CMUtlcwstrchr" ) 
	#define CAL_CMUtlcwstrchr  CMUtlcwstrchr
	#define CHK_CMUtlcwstrchr  TRUE
	#define EXP_CMUtlcwstrchr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrchr", (RTS_UINTPTR)CMUtlcwstrchr, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlcwstrchr
	#define EXT_CMUtilsCMUtlcwstrchr
	#define GET_CMUtilsCMUtlcwstrchr  ERR_OK
	#define CAL_CMUtilsCMUtlcwstrchr pICMUtils->ICMUtlcwstrchr
	#define CHK_CMUtilsCMUtlcwstrchr (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlcwstrchr  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlcwstrchr
	#define EXT_CMUtlcwstrchr
	#define GET_CMUtlcwstrchr(fl)  CAL_CMGETAPI( "CMUtlcwstrchr" ) 
	#define CAL_CMUtlcwstrchr pICMUtils->ICMUtlcwstrchr
	#define CHK_CMUtlcwstrchr (pICMUtils != NULL)
	#define EXP_CMUtlcwstrchr  CAL_CMEXPAPI( "CMUtlcwstrchr" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlcwstrchr  PFCMUTLCWSTRCHR pfCMUtlcwstrchr;
	#define EXT_CMUtlcwstrchr  extern PFCMUTLCWSTRCHR pfCMUtlcwstrchr;
	#define GET_CMUtlcwstrchr(fl)  s_pfCMGetAPI2( "CMUtlcwstrchr", (RTS_VOID_FCTPTR *)&pfCMUtlcwstrchr, (fl), 0, 0)
	#define CAL_CMUtlcwstrchr  pfCMUtlcwstrchr
	#define CHK_CMUtlcwstrchr  (pfCMUtlcwstrchr != NULL)
	#define EXP_CMUtlcwstrchr  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlcwstrchr", (RTS_UINTPTR)CMUtlcwstrchr, 0, 0) 
#endif




/**
 * <description>Tokenizer to help to split the arguments into separated tokens.
 * </description>
 * <param name="pszArguments" type="IN">Pointer to arguments string</param>
 * <param name="paszArgumentList" type="INOUT">Pointer to string array of expected argument tokens.
 *	NOTE: 
 *  Can be NULL and so you can test number of arguments included in pszArguments! In this case you have to specify *pNumArguments = RTS_SIZE_MAX as input value!</param>
 * <param name="pNumArguments" type="INOUT">Poiner number of expected arguments in paArgumentList and returns number of arguments parsed.
 *  NOTE:
 *  If less arguments are specified as contained in pszArguments string, then the last argument string in paszArgumentList contains a pointer to remaining string!</param>
 * <param name="pszDelim" type="IN">Pointer to delimiter string (e.g. CMUTL_SPACE_DELIMITER=" " or ", "). Can be NULL, then we use space (" ") as delimiter!</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successful</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter</errorcode>
 */
RTS_RESULT CDECL CMUtlArgumentTokenizer(RTS_CSTRING* pszArguments, RTS_CSTRING** paszArgumentList, RTS_SIZE *pNumArguments, RTS_CSTRING* pszDelim);
typedef RTS_RESULT (CDECL * PFCMUTLARGUMENTTOKENIZER) (RTS_CSTRING* pszArguments, RTS_CSTRING** paszArgumentList, RTS_SIZE *pNumArguments, RTS_CSTRING* pszDelim);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLARGUMENTTOKENIZER_NOTIMPLEMENTED)
	#define USE_CMUtlArgumentTokenizer
	#define EXT_CMUtlArgumentTokenizer
	#define GET_CMUtlArgumentTokenizer(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlArgumentTokenizer(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlArgumentTokenizer  FALSE
	#define EXP_CMUtlArgumentTokenizer  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlArgumentTokenizer
	#define EXT_CMUtlArgumentTokenizer
	#define GET_CMUtlArgumentTokenizer(fl)  CAL_CMGETAPI( "CMUtlArgumentTokenizer" ) 
	#define CAL_CMUtlArgumentTokenizer  CMUtlArgumentTokenizer
	#define CHK_CMUtlArgumentTokenizer  TRUE
	#define EXP_CMUtlArgumentTokenizer  CAL_CMEXPAPI( "CMUtlArgumentTokenizer" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlArgumentTokenizer
	#define EXT_CMUtlArgumentTokenizer
	#define GET_CMUtlArgumentTokenizer(fl)  CAL_CMGETAPI( "CMUtlArgumentTokenizer" ) 
	#define CAL_CMUtlArgumentTokenizer  CMUtlArgumentTokenizer
	#define CHK_CMUtlArgumentTokenizer  TRUE
	#define EXP_CMUtlArgumentTokenizer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlArgumentTokenizer", (RTS_UINTPTR)CMUtlArgumentTokenizer, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlArgumentTokenizer
	#define EXT_CMUtilsCMUtlArgumentTokenizer
	#define GET_CMUtilsCMUtlArgumentTokenizer  ERR_OK
	#define CAL_CMUtilsCMUtlArgumentTokenizer pICMUtils->ICMUtlArgumentTokenizer
	#define CHK_CMUtilsCMUtlArgumentTokenizer (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlArgumentTokenizer  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlArgumentTokenizer
	#define EXT_CMUtlArgumentTokenizer
	#define GET_CMUtlArgumentTokenizer(fl)  CAL_CMGETAPI( "CMUtlArgumentTokenizer" ) 
	#define CAL_CMUtlArgumentTokenizer pICMUtils->ICMUtlArgumentTokenizer
	#define CHK_CMUtlArgumentTokenizer (pICMUtils != NULL)
	#define EXP_CMUtlArgumentTokenizer  CAL_CMEXPAPI( "CMUtlArgumentTokenizer" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlArgumentTokenizer  PFCMUTLARGUMENTTOKENIZER pfCMUtlArgumentTokenizer;
	#define EXT_CMUtlArgumentTokenizer  extern PFCMUTLARGUMENTTOKENIZER pfCMUtlArgumentTokenizer;
	#define GET_CMUtlArgumentTokenizer(fl)  s_pfCMGetAPI2( "CMUtlArgumentTokenizer", (RTS_VOID_FCTPTR *)&pfCMUtlArgumentTokenizer, (fl), 0, 0)
	#define CAL_CMUtlArgumentTokenizer  pfCMUtlArgumentTokenizer
	#define CHK_CMUtlArgumentTokenizer  (pfCMUtlArgumentTokenizer != NULL)
	#define EXP_CMUtlArgumentTokenizer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlArgumentTokenizer", (RTS_UINTPTR)CMUtlArgumentTokenizer, 0, 0) 
#endif




/**
 * <description> 
 *  Binary search for a RTS_UI32 key in an array. Array can be an array of structure and the structure contains a member with the key.
 * </description>
 * <param name="pui8SearchArray" type="IN">Pointer to search array. Can be an array of a structure</param>
 * <param name="siSearchArraySize" type="IN">Size of the search array in bytes</param>
 * <param name="siSearchElementSize" type="IN">Size of one array element (if it is an array of a structure, this is the structure size)</param>
 * <param name="siSearchElementKeyOffset" type="IN">Offset within an array element to find the key value</param>
 * <param name="siMin" type="IN">Minimal index in the array to start the search</param>
 * <param name="ui32Key" type="IN">Key value to search for</param>
 * <param name="pResult" type="OUT">Pointer to error code:
 *	<ul>
 *     <li>ERR_OK: Key found</li>
 *     <li>ERR_NO_OBJECT: Key not found</li> 
 *  </ul>
 * </param>
 * <result>Returns the find position or RTS_SIZE_MAX if not found</result>
 */
RTS_SIZE CDECL CMUtlBinarySearch_UI32(RTS_UI8 *pui8SearchArray, RTS_SIZE siSearchArraySize, RTS_SIZE siSearchElementSize, RTS_SIZE siSearchElementKeyOffset, RTS_SIZE siMin, RTS_UI32 ui32Key, RTS_RESULT *pResult);
typedef RTS_SIZE (CDECL * PFCMUTLBINARYSEARCH_UI32) (RTS_UI8 *pui8SearchArray, RTS_SIZE siSearchArraySize, RTS_SIZE siSearchElementSize, RTS_SIZE siSearchElementKeyOffset, RTS_SIZE siMin, RTS_UI32 ui32Key, RTS_RESULT *pResult);
#if defined(CMUTILS_NOTIMPLEMENTED) || defined(CMUTLBINARYSEARCH_UI32_NOTIMPLEMENTED)
	#define USE_CMUtlBinarySearch_UI32
	#define EXT_CMUtlBinarySearch_UI32
	#define GET_CMUtlBinarySearch_UI32(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlBinarySearch_UI32(p0,p1,p2,p3,p4,p5,p6)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlBinarySearch_UI32  FALSE
	#define EXP_CMUtlBinarySearch_UI32  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlBinarySearch_UI32
	#define EXT_CMUtlBinarySearch_UI32
	#define GET_CMUtlBinarySearch_UI32(fl)  CAL_CMGETAPI( "CMUtlBinarySearch_UI32" ) 
	#define CAL_CMUtlBinarySearch_UI32  CMUtlBinarySearch_UI32
	#define CHK_CMUtlBinarySearch_UI32  TRUE
	#define EXP_CMUtlBinarySearch_UI32  CAL_CMEXPAPI( "CMUtlBinarySearch_UI32" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILS_EXTERNAL)
	#define USE_CMUtlBinarySearch_UI32
	#define EXT_CMUtlBinarySearch_UI32
	#define GET_CMUtlBinarySearch_UI32(fl)  CAL_CMGETAPI( "CMUtlBinarySearch_UI32" ) 
	#define CAL_CMUtlBinarySearch_UI32  CMUtlBinarySearch_UI32
	#define CHK_CMUtlBinarySearch_UI32  TRUE
	#define EXP_CMUtlBinarySearch_UI32  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlBinarySearch_UI32", (RTS_UINTPTR)CMUtlBinarySearch_UI32, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsCMUtlBinarySearch_UI32
	#define EXT_CMUtilsCMUtlBinarySearch_UI32
	#define GET_CMUtilsCMUtlBinarySearch_UI32  ERR_OK
	#define CAL_CMUtilsCMUtlBinarySearch_UI32 pICMUtils->ICMUtlBinarySearch_UI32
	#define CHK_CMUtilsCMUtlBinarySearch_UI32 (pICMUtils != NULL)
	#define EXP_CMUtilsCMUtlBinarySearch_UI32  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlBinarySearch_UI32
	#define EXT_CMUtlBinarySearch_UI32
	#define GET_CMUtlBinarySearch_UI32(fl)  CAL_CMGETAPI( "CMUtlBinarySearch_UI32" ) 
	#define CAL_CMUtlBinarySearch_UI32 pICMUtils->ICMUtlBinarySearch_UI32
	#define CHK_CMUtlBinarySearch_UI32 (pICMUtils != NULL)
	#define EXP_CMUtlBinarySearch_UI32  CAL_CMEXPAPI( "CMUtlBinarySearch_UI32" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlBinarySearch_UI32  PFCMUTLBINARYSEARCH_UI32 pfCMUtlBinarySearch_UI32;
	#define EXT_CMUtlBinarySearch_UI32  extern PFCMUTLBINARYSEARCH_UI32 pfCMUtlBinarySearch_UI32;
	#define GET_CMUtlBinarySearch_UI32(fl)  s_pfCMGetAPI2( "CMUtlBinarySearch_UI32", (RTS_VOID_FCTPTR *)&pfCMUtlBinarySearch_UI32, (fl), 0, 0)
	#define CAL_CMUtlBinarySearch_UI32  pfCMUtlBinarySearch_UI32
	#define CHK_CMUtlBinarySearch_UI32  (pfCMUtlBinarySearch_UI32 != NULL)
	#define EXP_CMUtlBinarySearch_UI32  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlBinarySearch_UI32", (RTS_UINTPTR)CMUtlBinarySearch_UI32, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFCMUTLSKIPWHITESPACE ICMUtlSkipWhiteSpace;
 	PFCMUTLLTOA ICMUtlLtoa;
 	PFCMUTLSTRINGTOVERSION ICMUtlStringToVersion;
 	PFCMUTLVERSIONTOSTRING ICMUtlVersionToString;
 	PFCMUTLBYTEARRAYTOSTRING ICMUtlByteArrayToString;
 	PFCMUTLSTRINGTOBYTEARRAY ICMUtlStringToByteArray;
 	PFCMUTLULTOA ICMUtlUltoa;
 	PFCMUTLSTRREV ICMUtlStrrev;
 	PFCMUTLSTRRCHR ICMUtlStrRChr;
 	PFCMUTLSTRICMP ICMUtlStrICmp;
 	PFCMUTLSTRNICMP ICMUtlStrNICmp;
 	PFCMUTLSTRISTR ICMUtlStrIStr;
 	PFCMUTLSTRLWR ICMUtlStrlwr;
 	PFCMUTLSTRUPR ICMUtlStrupr;
 	PFCMUTLSAFESTRCPY ICMUtlSafeStrCpy;
 	PFCMUTLSAFESTRCPY2 ICMUtlSafeStrCpy2;
 	PFCMUTLSAFESTRNCPY ICMUtlSafeStrNCpy;
 	PFCMUTLSAFESTRCAT ICMUtlSafeStrCat;
 	PFCMUTLSTRLEN ICMUtlStrLen;
 	PFCMUTLSTRNLEN ICMUtlStrNLen;
 	PFCMUTLSTRTOLOWER ICMUtlStrToLower;
 	PFCMUTLSTRTOUPPER ICMUtlStrToUpper;
 	PFCMUTLVSNPRINTF ICMUtlvsnprintf;
 	PFCMUTLSNPRINTF ICMUtlsnprintf;
 	PFCMUTLMEMCPY ICMUtlMemCpy;
 	PFCMUTLSAFEMEMCPY ICMUtlSafeMemCpy;
 	PFCMUTLSAFEMEMMOVE ICMUtlSafeMemMove;
 	PFCMUTLGUIDTOSTRING ICMUtlGUIDToString;
 	PFCMUTLSTRINGTOGUID ICMUtlStringToGUID;
 	PFCMSTRINGCREATE ICMStringCreate;
 	PFCMSTRINGCREATE2 ICMStringCreate2;
 	PFCMSTRINGCREATE3 ICMStringCreate3;
 	PFCMSTRINGDELETE ICMStringDelete;
 	PFCMSTRINGEXTEND ICMStringExtend;
 	PFCMSTRINGCOPY ICMStringCopy;
 	PFCMSTRINGCOPY2 ICMStringCopy2;
 	PFCMSTRINGCAT ICMStringCat;
 	PFCMUTLWSTRCMP ICMUtlwstrcmp;
 	PFCMUTLWSTRNCMP ICMUtlwstrncmp;
 	PFCMUTLWSTRCPY ICMUtlwstrcpy;
 	PFCMUTLWSTRCAT ICMUtlwstrcat;
 	PFCMUTLWSTRNCPY ICMUtlwstrncpy;
 	PFCMUTLWSTRLEN ICMUtlwstrlen;
 	PFCMUTLWTOLOWER ICMUtlwtolower;
 	PFCMUTLWTOUPPER ICMUtlwtoupper;
 	PFCMUTLSTRTOW ICMUtlStrToW;
 	PFCMUTLWTOSTR ICMUtlWToStr;
 	PFCMUTLWSTRCHR ICMUtlwstrchr;
 	PFCMUTLCHECKUTF16ENCODING ICMUtlCheckUtf16Encoding;
 	PFCMUTLWTOUTF8 ICMUtlWToUtf8;
 	PFCMUTLUTF8TOW ICMUtlUtf8ToW;
 	PFCMUTLCHECKUTF8ENCODING ICMUtlCheckUtf8Encoding;
 	PFCMUTLCWSTRCMP ICMUtlcwstrcmp;
 	PFCMUTLCWSTRCPY ICMUtlcwstrcpy;
 	PFCMUTLCWSTRCAT ICMUtlcwstrcat;
 	PFCMUTLCWSTRNCPY ICMUtlcwstrncpy;
 	PFCMUTLCWSTRLEN ICMUtlcwstrlen;
 	PFCMUTLCWTOLOWER ICMUtlcwtolower;
 	PFCMUTLCWTOUPPER ICMUtlcwtoupper;
 	PFCMUTLSTRTOCW ICMUtlStrToCW;
 	PFCMUTLCWTOSTR ICMUtlCWToStr;
 	PFCMUTLCWSTRCHR ICMUtlcwstrchr;
 	PFCMUTLARGUMENTTOKENIZER ICMUtlArgumentTokenizer;
 	PFCMUTLBINARYSEARCH_UI32 ICMUtlBinarySearch_UI32;
 } ICMUtils_C;

#ifdef CPLUSPLUS
class ICMUtils : public IBase
{
	public:
		virtual char* CDECL ICMUtlSkipWhiteSpace(char* psz) =0;
		virtual char* CDECL ICMUtlLtoa(RTS_I32 lValue, char *pszString, int nMaxLen, int iBase, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ICMUtlStringToVersion(char *pszVersion, RTS_UI32 *pui32Version) =0;
		virtual RTS_RESULT CDECL ICMUtlVersionToString(RTS_UI32 ui32Version, char *pszVersion, RTS_SIZE nMaxLen) =0;
		virtual RTS_RESULT CDECL ICMUtlByteArrayToString(const RTS_UI8 *pData, RTS_SIZE DataSize, char *pszDestination, RTS_SIZE *pDestinationSize) =0;
		virtual RTS_RESULT CDECL ICMUtlStringToByteArray(const char *pszData, RTS_UI8 *pDestination, RTS_SIZE *pDestinationSize) =0;
		virtual char* CDECL ICMUtlUltoa(RTS_UI32 ulValue, char *pszString, int nMaxLen, int iBase, RTS_RESULT *pResult) =0;
		virtual char* CDECL ICMUtlStrrev(char *psz) =0;
		virtual char* CDECL ICMUtlStrRChr(char *pszString, char *pszBegin, char cFind) =0;
		virtual int CDECL ICMUtlStrICmp(char *pszString1, char *pszString2) =0;
		virtual int CDECL ICMUtlStrNICmp(char *pszString1, char *pszString2, RTS_SIZE n) =0;
		virtual char * CDECL ICMUtlStrIStr(const char *pszString, const char *pszStringToFind) =0;
		virtual char* CDECL ICMUtlStrlwr(char *pszString) =0;
		virtual char* CDECL ICMUtlStrupr(char *pszString) =0;
		virtual RTS_RESULT CDECL ICMUtlSafeStrCpy(char *pszDest, RTS_SIZE nDestSize, const char *pszSrc) =0;
		virtual RTS_RESULT CDECL ICMUtlSafeStrCpy2(char *pszDest, RTS_SIZE nDestSize, const char *pszSrc, int nCopyMode) =0;
		virtual RTS_RESULT CDECL ICMUtlSafeStrNCpy(char *pszDest, RTS_SIZE nDestSize, const char *pszSrc, RTS_SIZE nBytesCopy) =0;
		virtual RTS_RESULT CDECL ICMUtlSafeStrCat(char *pszDest, RTS_SIZE nDestSize, const char *pszSrc) =0;
		virtual RTS_SIZE CDECL ICMUtlStrLen(char *str) =0;
		virtual RTS_SIZE CDECL ICMUtlStrNLen(char *str, RTS_SIZE nMaxSize) =0;
		virtual char* CDECL ICMUtlStrToLower(char* psz) =0;
		virtual char* CDECL ICMUtlStrToUpper(char* psz) =0;
		virtual RTS_RESULT CDECL ICMUtlvsnprintf(char *pszStr, RTS_SIZE ulStrSize, const char *pszFormat, va_list *pargList) =0;
		virtual RTS_RESULT CDECL ICMUtlsnprintf(char *pszStr, RTS_SIZE ulStrSize, const char *pszFormat, ...) =0;
		virtual void* CDECL ICMUtlMemCpy(void *pDest, const void *pSrc, RTS_SIZE ulToCopy) =0;
		virtual RTS_RESULT CDECL ICMUtlSafeMemCpy(void *pDest, RTS_SIZE ulDestSize, const void *pSrc, RTS_SIZE ulToCopy) =0;
		virtual RTS_RESULT CDECL ICMUtlSafeMemMove(void *pDest, RTS_SIZE ulDestSize, const void *pSrc, RTS_SIZE ulToCopy) =0;
		virtual RTS_RESULT CDECL ICMUtlGUIDToString(const RTS_GUID *pguid, char *pszGUID, RTS_SIZE nMaxLen) =0;
		virtual RTS_RESULT CDECL ICMUtlStringToGUID(const char *pszGUID, RTS_GUID *pguid) =0;
		virtual RTS_RESULT CDECL ICMStringCreate(RTS_STRING_CLASS *pString, char *pszComponentName, char *pszInit) =0;
		virtual RTS_RESULT CDECL ICMStringCreate2(RTS_STRING_CLASS *pString, char *pszComponentName, char *pszInit, RTS_BOOL bAllocDynamic) =0;
		virtual RTS_RESULT CDECL ICMStringCreate3(RTS_STRING_CLASS *pString, char *pszComponentName, RTS_SIZE nMaxSize, RTS_BOOL bAllocDynamic) =0;
		virtual RTS_RESULT CDECL ICMStringDelete(RTS_STRING_CLASS *pString) =0;
		virtual RTS_RESULT CDECL ICMStringExtend(RTS_STRING_CLASS *pString, RTS_SIZE size) =0;
		virtual RTS_RESULT CDECL ICMStringCopy(RTS_STRING_CLASS *pString, char *pszSrc, RTS_SIZE nSrcLen) =0;
		virtual RTS_RESULT CDECL ICMStringCopy2(RTS_STRING_CLASS *pString, char *pszSrc) =0;
		virtual RTS_RESULT CDECL ICMStringCat(RTS_STRING_CLASS *pString, char *pszToAdd) =0;
		virtual int CDECL ICMUtlwstrcmp(const RTS_WCHAR *pwsz1, const RTS_WCHAR *pwsz2) =0;
		virtual int CDECL ICMUtlwstrncmp(const RTS_WCHAR *pwsz1, const RTS_WCHAR *pwsz2, RTS_SIZE nCount) =0;
		virtual RTS_RESULT CDECL ICMUtlwstrcpy(RTS_WCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_WCHAR *pwszSrc) =0;
		virtual RTS_RESULT CDECL ICMUtlwstrcat(RTS_WCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_WCHAR *pwszSrc) =0;
		virtual RTS_RESULT CDECL ICMUtlwstrncpy(RTS_WCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_WCHAR *pwszSrc, RTS_SIZE n) =0;
		virtual RTS_SIZE CDECL ICMUtlwstrlen(const RTS_WCHAR *pwsz) =0;
		virtual RTS_WCHAR* CDECL ICMUtlwtolower(RTS_WCHAR* pwsz) =0;
		virtual RTS_WCHAR* CDECL ICMUtlwtoupper(RTS_WCHAR* pwsz) =0;
		virtual RTS_RESULT CDECL ICMUtlStrToW(const char* pszSrc, RTS_WCHAR* pwszDest, RTS_SIZE nDestSize) =0;
		virtual RTS_RESULT CDECL ICMUtlWToStr(const RTS_WCHAR* pwszSrc, char* pszDest, RTS_SIZE nDestSize) =0;
		virtual RTS_WCHAR* CDECL ICMUtlwstrchr(const RTS_WCHAR *pwsz, RTS_WCHAR w) =0;
		virtual RTS_RESULT CDECL ICMUtlCheckUtf16Encoding(const RTS_WCHAR *pwsz, RTS_SIZE siWstrLen) =0;
		virtual RTS_RESULT CDECL ICMUtlWToUtf8(RTS_WCHAR *pwsz, RTS_SIZE wstrLen, RTS_UI8* pUtf8Str, RTS_SIZE bufferSize) =0;
		virtual RTS_RESULT CDECL ICMUtlUtf8ToW(RTS_UI8* pUtf8Str, RTS_SIZE utf8BufferSize, RTS_WCHAR* pwsz, RTS_SIZE wstrLen) =0;
		virtual RTS_RESULT CDECL ICMUtlCheckUtf8Encoding(RTS_UI8 *pUtf8Str, RTS_SIZE utf8BufferSize) =0;
		virtual int CDECL ICMUtlcwstrcmp(const RTS_CWCHAR *pwsz1, const RTS_CWCHAR *pwsz2) =0;
		virtual RTS_RESULT CDECL ICMUtlcwstrcpy(RTS_CWCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_CWCHAR *pwszSrc) =0;
		virtual RTS_RESULT CDECL ICMUtlcwstrcat(RTS_CWCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_CWCHAR *pwszSrc) =0;
		virtual RTS_RESULT CDECL ICMUtlcwstrncpy(RTS_CWCHAR *pwszDest, RTS_SIZE nDestSize, const RTS_CWCHAR *pwszSrc, RTS_SIZE n) =0;
		virtual RTS_SIZE CDECL ICMUtlcwstrlen(const RTS_CWCHAR *pwsz) =0;
		virtual RTS_CWCHAR* CDECL ICMUtlcwtolower(RTS_CWCHAR* pwsz) =0;
		virtual RTS_CWCHAR* CDECL ICMUtlcwtoupper(RTS_CWCHAR* pwsz) =0;
		virtual RTS_RESULT CDECL ICMUtlStrToCW(const char* pszSrc, RTS_CWCHAR* pwszDest, RTS_SIZE nDestSize) =0;
		virtual RTS_RESULT CDECL ICMUtlCWToStr(const RTS_CWCHAR* pwszSrc, char* pszDest, RTS_SIZE nDestSize) =0;
		virtual RTS_CWCHAR* CDECL ICMUtlcwstrchr(const RTS_CWCHAR *pwsz, RTS_CWCHAR cw) =0;
		virtual RTS_RESULT CDECL ICMUtlArgumentTokenizer(RTS_CSTRING* pszArguments, RTS_CSTRING** paszArgumentList, RTS_SIZE *pNumArguments, RTS_CSTRING* pszDelim) =0;
		virtual RTS_SIZE CDECL ICMUtlBinarySearch_UI32(RTS_UI8 *pui8SearchArray, RTS_SIZE siSearchArraySize, RTS_SIZE siSearchElementSize, RTS_SIZE siSearchElementKeyOffset, RTS_SIZE siMin, RTS_UI32 ui32Key, RTS_RESULT *pResult) =0;
};
	#ifndef ITF_CMUtils
		#define ITF_CMUtils static ICMUtils *pICMUtils = NULL;
	#endif
	#define EXTITF_CMUtils
#else	/*CPLUSPLUS*/
	typedef ICMUtils_C		ICMUtils;
	#ifndef ITF_CMUtils
		#define ITF_CMUtils
	#endif
	#define EXTITF_CMUtils
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMUTILSITF_H_*/
