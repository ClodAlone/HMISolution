 /**
 * <interfacename>CmpXMLParser</interfacename>
 * <description> 
 *	Interface of the XML-Parser.
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPXMLPARSERITF_H_
#define _CMPXMLPARSERITF_H_

#include "CmpStd.h"

 

 




#include "CmpStd.h"

/* Hook definitions (0..10000 reserved for core components) */

typedef RTS_CWCHAR XML_Char;
typedef RTS_CWCHAR XML_LChar;

/**
 * <category>XML flags</category>
 * <description>
 *	Flags for the XML events
 * </description>
 *	<element name="RTS_XML_EVENTFLAG_ASCII" type="IN">ASCII strings</element>
 *	<element name="RTS_XML_EVENTFLAG_UNICODE" type="IN">Unicode strings</element>
 */
#define RTS_XML_EVENTFLAG_ASCII			0x00000001
#define RTS_XML_EVENTFLAG_UNICODE		0x00000002


/** EXTERN LIB SECTION BEGIN **/

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <category>Event parameter</category>
 * <element name="hXML" type="IN">Handle to XML parser</element>
 * <element name="ulFlags" type="IN">Flags. See corresponding category</element>
 * <element name="pUserData" type="IN">Pointer to user data</element>
 * <element name="pcwName" type="IN">Pointer to tag name</element>
 */
typedef struct tagEVTPARAM_CmpXMLEnd
{
	RTS_IEC_HANDLE hXMLParser;		
	RTS_IEC_UDINT ulFlags;		
	RTS_IEC_BYTE *pUserData;		
	RTS_IEC_CWCHAR *pcwName;		
} EVTPARAM_CmpXMLEnd;

/**
 * <category>Event parameter</category>
 * <element name="hXML" type="IN">Handle to XML parser</element>
 * <element name="ulFlags" type="IN">Flags. See corresponding category</element>
 * <element name="pUserData" type="IN">Pointer to user data</element>
 * <element name="pcwName" type="IN">Pointer to tag name</element>
 * <element name="ppcwAttributes" type="IN">Pointer to list of attributes (NULL terminated list)</element>
 */
typedef struct tagEVTPARAM_CmpXMLData
{
	RTS_IEC_HANDLE hXMLParser;		
	RTS_IEC_UDINT ulFlags;		
	RTS_IEC_BYTE *pUserData;		
	RTS_IEC_CWCHAR *pcwValue;		
	RTS_IEC_DINT nValueLen;		
} EVTPARAM_CmpXMLData;

/**
 * <category>Event parameter</category>
 * <element name="hXML" type="IN">Handle to XML parser</element>
 * <element name="ulFlags" type="IN">Flags. See corresponding category</element>
 * <element name="pUserData" type="IN">Pointer to user data</element>
 * <element name="pcwName" type="IN">Pointer to tag name</element>
 */
typedef struct tagEVTPARAM_CmpXMLStart
{
	RTS_IEC_HANDLE hXMLParser;		
	RTS_IEC_UDINT ulFlags;		
	RTS_IEC_BYTE *pUserData;		
	RTS_IEC_CWCHAR *pcwName;		
	RTS_IEC_CWCHAR **ppcwAttributes;		
} EVTPARAM_CmpXMLStart;

/**
 * <description>createxmlparser2</description>
 */
typedef struct tagcreatexmlparser2_struct
{
	RTS_IEC_STRING szXMLFileName[81];	/* VAR_INPUT */	
	RTS_IEC_STRING szEncoding[81];		/* VAR_INPUT */	
	RTS_IEC_RESULT *Result;				/* VAR_IN_OUT */	
	RTS_IEC_HANDLE CreateXMLParser2;	/* VAR_OUTPUT */	
} createxmlparser2_struct;

void CDECL CDECL_EXT createxmlparser2(createxmlparser2_struct *p);
typedef void (CDECL CDECL_EXT* PFCREATEXMLPARSER2_IEC) (createxmlparser2_struct *p);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(CREATEXMLPARSER2_NOTIMPLEMENTED)
	#define USE_createxmlparser2
	#define EXT_createxmlparser2
	#define GET_createxmlparser2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_createxmlparser2(p0) 
	#define CHK_createxmlparser2  FALSE
	#define EXP_createxmlparser2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_createxmlparser2
	#define EXT_createxmlparser2
	#define GET_createxmlparser2(fl)  CAL_CMGETAPI( "createxmlparser2" ) 
	#define CAL_createxmlparser2  createxmlparser2
	#define CHK_createxmlparser2  TRUE
	#define EXP_createxmlparser2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"createxmlparser2", (RTS_UINTPTR)createxmlparser2, 1, 0xAC05CC34, 0x03050500) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_createxmlparser2
	#define EXT_createxmlparser2
	#define GET_createxmlparser2(fl)  CAL_CMGETAPI( "createxmlparser2" ) 
	#define CAL_createxmlparser2  createxmlparser2
	#define CHK_createxmlparser2  TRUE
	#define EXP_createxmlparser2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"createxmlparser2", (RTS_UINTPTR)createxmlparser2, 1, 0xAC05CC34, 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParsercreatexmlparser2
	#define EXT_CmpXMLParsercreatexmlparser2
	#define GET_CmpXMLParsercreatexmlparser2  ERR_OK
	#define CAL_CmpXMLParsercreatexmlparser2  createxmlparser2
	#define CHK_CmpXMLParsercreatexmlparser2  TRUE
	#define EXP_CmpXMLParsercreatexmlparser2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"createxmlparser2", (RTS_UINTPTR)createxmlparser2, 1, 0xAC05CC34, 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_createxmlparser2
	#define EXT_createxmlparser2
	#define GET_createxmlparser2(fl)  CAL_CMGETAPI( "createxmlparser2" ) 
	#define CAL_createxmlparser2  createxmlparser2
	#define CHK_createxmlparser2  TRUE
	#define EXP_createxmlparser2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"createxmlparser2", (RTS_UINTPTR)createxmlparser2, 1, 0xAC05CC34, 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_createxmlparser2  PFCREATEXMLPARSER2_IEC pfcreatexmlparser2;
	#define EXT_createxmlparser2  extern PFCREATEXMLPARSER2_IEC pfcreatexmlparser2;
	#define GET_createxmlparser2(fl)  s_pfCMGetAPI2( "createxmlparser2", (RTS_VOID_FCTPTR *)&pfcreatexmlparser2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xAC05CC34, 0x03050500)
	#define CAL_createxmlparser2  pfcreatexmlparser2
	#define CHK_createxmlparser2  (pfcreatexmlparser2 != NULL)
	#define EXP_createxmlparser2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"createxmlparser2", (RTS_UINTPTR)createxmlparser2, 1, 0xAC05CC34, 0x03050500) 
#endif


/**
 * <description>freexmlparser</description>
 */
typedef struct tagfreexmlparser_struct
{
	RTS_IEC_HANDLE hParser;				/* VAR_INPUT */	
	RTS_IEC_RESULT FreeXMLParser;		/* VAR_OUTPUT */	
} freexmlparser_struct;

void CDECL CDECL_EXT freexmlparser(freexmlparser_struct *p);
typedef void (CDECL CDECL_EXT* PFFREEXMLPARSER_IEC) (freexmlparser_struct *p);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(FREEXMLPARSER_NOTIMPLEMENTED)
	#define USE_freexmlparser
	#define EXT_freexmlparser
	#define GET_freexmlparser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_freexmlparser(p0) 
	#define CHK_freexmlparser  FALSE
	#define EXP_freexmlparser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_freexmlparser
	#define EXT_freexmlparser
	#define GET_freexmlparser(fl)  CAL_CMGETAPI( "freexmlparser" ) 
	#define CAL_freexmlparser  freexmlparser
	#define CHK_freexmlparser  TRUE
	#define EXP_freexmlparser  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"freexmlparser", (RTS_UINTPTR)freexmlparser, 1, 0xF9BB08C3, 0x03050500) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_freexmlparser
	#define EXT_freexmlparser
	#define GET_freexmlparser(fl)  CAL_CMGETAPI( "freexmlparser" ) 
	#define CAL_freexmlparser  freexmlparser
	#define CHK_freexmlparser  TRUE
	#define EXP_freexmlparser  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"freexmlparser", (RTS_UINTPTR)freexmlparser, 1, 0xF9BB08C3, 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParserfreexmlparser
	#define EXT_CmpXMLParserfreexmlparser
	#define GET_CmpXMLParserfreexmlparser  ERR_OK
	#define CAL_CmpXMLParserfreexmlparser  freexmlparser
	#define CHK_CmpXMLParserfreexmlparser  TRUE
	#define EXP_CmpXMLParserfreexmlparser  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"freexmlparser", (RTS_UINTPTR)freexmlparser, 1, 0xF9BB08C3, 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_freexmlparser
	#define EXT_freexmlparser
	#define GET_freexmlparser(fl)  CAL_CMGETAPI( "freexmlparser" ) 
	#define CAL_freexmlparser  freexmlparser
	#define CHK_freexmlparser  TRUE
	#define EXP_freexmlparser  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"freexmlparser", (RTS_UINTPTR)freexmlparser, 1, 0xF9BB08C3, 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_freexmlparser  PFFREEXMLPARSER_IEC pffreexmlparser;
	#define EXT_freexmlparser  extern PFFREEXMLPARSER_IEC pffreexmlparser;
	#define GET_freexmlparser(fl)  s_pfCMGetAPI2( "freexmlparser", (RTS_VOID_FCTPTR *)&pffreexmlparser, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xF9BB08C3, 0x03050500)
	#define CAL_freexmlparser  pffreexmlparser
	#define CHK_freexmlparser  (pffreexmlparser != NULL)
	#define EXP_freexmlparser   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"freexmlparser", (RTS_UINTPTR)freexmlparser, 1, 0xF9BB08C3, 0x03050500) 
#endif


/**
 * <description>parsexml2</description>
 */
typedef struct tagparsexml2_struct
{
	RTS_IEC_HANDLE hParser;				/* VAR_INPUT */	
	RTS_IEC_BOOL bIsFinal;				/* VAR_INPUT */	
	RTS_IEC_RESULT ParseXML2;			/* VAR_OUTPUT */	
} parsexml2_struct;

void CDECL CDECL_EXT parsexml2(parsexml2_struct *p);
typedef void (CDECL CDECL_EXT* PFPARSEXML2_IEC) (parsexml2_struct *p);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(PARSEXML2_NOTIMPLEMENTED)
	#define USE_parsexml2
	#define EXT_parsexml2
	#define GET_parsexml2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_parsexml2(p0) 
	#define CHK_parsexml2  FALSE
	#define EXP_parsexml2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_parsexml2
	#define EXT_parsexml2
	#define GET_parsexml2(fl)  CAL_CMGETAPI( "parsexml2" ) 
	#define CAL_parsexml2  parsexml2
	#define CHK_parsexml2  TRUE
	#define EXP_parsexml2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"parsexml2", (RTS_UINTPTR)parsexml2, 1, 0xC0F3799A, 0x03050500) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_parsexml2
	#define EXT_parsexml2
	#define GET_parsexml2(fl)  CAL_CMGETAPI( "parsexml2" ) 
	#define CAL_parsexml2  parsexml2
	#define CHK_parsexml2  TRUE
	#define EXP_parsexml2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"parsexml2", (RTS_UINTPTR)parsexml2, 1, 0xC0F3799A, 0x03050500) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParserparsexml2
	#define EXT_CmpXMLParserparsexml2
	#define GET_CmpXMLParserparsexml2  ERR_OK
	#define CAL_CmpXMLParserparsexml2  parsexml2
	#define CHK_CmpXMLParserparsexml2  TRUE
	#define EXP_CmpXMLParserparsexml2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"parsexml2", (RTS_UINTPTR)parsexml2, 1, 0xC0F3799A, 0x03050500) 
#elif defined(CPLUSPLUS)
	#define USE_parsexml2
	#define EXT_parsexml2
	#define GET_parsexml2(fl)  CAL_CMGETAPI( "parsexml2" ) 
	#define CAL_parsexml2  parsexml2
	#define CHK_parsexml2  TRUE
	#define EXP_parsexml2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"parsexml2", (RTS_UINTPTR)parsexml2, 1, 0xC0F3799A, 0x03050500) 
#else /* DYNAMIC_LINK */
	#define USE_parsexml2  PFPARSEXML2_IEC pfparsexml2;
	#define EXT_parsexml2  extern PFPARSEXML2_IEC pfparsexml2;
	#define GET_parsexml2(fl)  s_pfCMGetAPI2( "parsexml2", (RTS_VOID_FCTPTR *)&pfparsexml2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xC0F3799A, 0x03050500)
	#define CAL_parsexml2  pfparsexml2
	#define CHK_parsexml2  (pfparsexml2 != NULL)
	#define EXP_parsexml2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"parsexml2", (RTS_UINTPTR)parsexml2, 1, 0xC0F3799A, 0x03050500) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#define EVTPARAMID_CmpXMLStart			0x0001
#define EVTVERSION_CmpXMLStart			0x0001

#define EVTPARAMID_CmpXMLData			0x0002
#define EVTVERSION_CmpXMLData			0x0001

#define EVTPARAMID_CmpXMLEnd			0x0003
#define EVTVERSION_CmpXMLEnd			0x0001


/**
 * <category>Events</category>
 * <description>Event is sent, if a new XML tag is detected</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpXMLStart</param>
 */
#define EVT_XMLStart					MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Events</category>
 * <description>Event is sent, if the content value of a XML tag is detected</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpXMLData</param>
 */
#define EVT_XMLData						MAKE_EVENTID(EVTCLASS_INFO, 2)

/**
 * <category>Events</category>
 * <description>Event is sent, if the end of a XML tag is detected</description>
 * <param name="pEventParam" type="IN">EVTPARAM_CmpXMLEnd</param>
 */
#define EVT_XMLEnd						MAKE_EVENTID(EVTCLASS_INFO, 3)


/**
 * <category>Encoding</category>
 * <description>Encoding to parse XML file</description>
 */
#define RTS_XML_ENCODING_UTF8			"UTF-8"
#define RTS_XML_ENCODING_UTF16			"UTF-16"


#ifdef __cplusplus
extern "C" {
#endif

/* atts is array of name/value pairs, terminated by 0;
   names and values are 0 terminated. */
typedef void (*PF_XML_StartElementHandler)(void *userData, XML_Char *name, XML_Char **atts);

typedef void (*PF_XML_EndElementHandler)(void *userData, XML_Char *name);

/* s is not 0 terminated. */
typedef void (*PF_XML_CharacterDataHandler)(void *userData, XML_Char *s, int len);

/**
 * <description> Creates a XML parser </description>
 * <param name="pcwEncoding" type="IN">Optional encoding. See category "Encoding".</param>
 * <result>Handle to XML parser instance</result>
 */
RTS_HANDLE CDECL CreateXMLParser(const XML_Char *pcwEncoding);
typedef RTS_HANDLE (CDECL * PFCREATEXMLPARSER) (const XML_Char *pcwEncoding);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(CREATEXMLPARSER_NOTIMPLEMENTED)
	#define USE_CreateXMLParser
	#define EXT_CreateXMLParser
	#define GET_CreateXMLParser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CreateXMLParser(p0)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CreateXMLParser  FALSE
	#define EXP_CreateXMLParser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CreateXMLParser
	#define EXT_CreateXMLParser
	#define GET_CreateXMLParser(fl)  CAL_CMGETAPI( "CreateXMLParser" ) 
	#define CAL_CreateXMLParser  CreateXMLParser
	#define CHK_CreateXMLParser  TRUE
	#define EXP_CreateXMLParser  CAL_CMEXPAPI( "CreateXMLParser" ) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_CreateXMLParser
	#define EXT_CreateXMLParser
	#define GET_CreateXMLParser(fl)  CAL_CMGETAPI( "CreateXMLParser" ) 
	#define CAL_CreateXMLParser  CreateXMLParser
	#define CHK_CreateXMLParser  TRUE
	#define EXP_CreateXMLParser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CreateXMLParser", (RTS_UINTPTR)CreateXMLParser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParserCreateXMLParser
	#define EXT_CmpXMLParserCreateXMLParser
	#define GET_CmpXMLParserCreateXMLParser  ERR_OK
	#define CAL_CmpXMLParserCreateXMLParser pICmpXMLParser->ICreateXMLParser
	#define CHK_CmpXMLParserCreateXMLParser (pICmpXMLParser != NULL)
	#define EXP_CmpXMLParserCreateXMLParser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CreateXMLParser
	#define EXT_CreateXMLParser
	#define GET_CreateXMLParser(fl)  CAL_CMGETAPI( "CreateXMLParser" ) 
	#define CAL_CreateXMLParser pICmpXMLParser->ICreateXMLParser
	#define CHK_CreateXMLParser (pICmpXMLParser != NULL)
	#define EXP_CreateXMLParser  CAL_CMEXPAPI( "CreateXMLParser" ) 
#else /* DYNAMIC_LINK */
	#define USE_CreateXMLParser  PFCREATEXMLPARSER pfCreateXMLParser;
	#define EXT_CreateXMLParser  extern PFCREATEXMLPARSER pfCreateXMLParser;
	#define GET_CreateXMLParser(fl)  s_pfCMGetAPI2( "CreateXMLParser", (RTS_VOID_FCTPTR *)&pfCreateXMLParser, (fl), 0, 0)
	#define CAL_CreateXMLParser  pfCreateXMLParser
	#define CHK_CreateXMLParser  (pfCreateXMLParser != NULL)
	#define EXP_CreateXMLParser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CreateXMLParser", (RTS_UINTPTR)CreateXMLParser, 0, 0) 
#endif




/**
 * <description> Creates a XML parser </description>
 * <param name="pszXMLFileName" type="IN">Name of the XML file</param>
 * <param name="pcwEncoding" type="IN">Optional encoding. See category "Encoding".</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to XML parser instance</result>
 */
RTS_HANDLE CDECL CreateXMLParser2(char *pszXMLFileName, const XML_Char *pcwEncoding, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFCREATEXMLPARSER2) (char *pszXMLFileName, const XML_Char *pcwEncoding, RTS_RESULT *pResult);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(CREATEXMLPARSER2_NOTIMPLEMENTED)
	#define USE_CreateXMLParser2
	#define EXT_CreateXMLParser2
	#define GET_CreateXMLParser2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CreateXMLParser2(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CreateXMLParser2  FALSE
	#define EXP_CreateXMLParser2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CreateXMLParser2
	#define EXT_CreateXMLParser2
	#define GET_CreateXMLParser2(fl)  CAL_CMGETAPI( "CreateXMLParser2" ) 
	#define CAL_CreateXMLParser2  CreateXMLParser2
	#define CHK_CreateXMLParser2  TRUE
	#define EXP_CreateXMLParser2  CAL_CMEXPAPI( "CreateXMLParser2" ) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_CreateXMLParser2
	#define EXT_CreateXMLParser2
	#define GET_CreateXMLParser2(fl)  CAL_CMGETAPI( "CreateXMLParser2" ) 
	#define CAL_CreateXMLParser2  CreateXMLParser2
	#define CHK_CreateXMLParser2  TRUE
	#define EXP_CreateXMLParser2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CreateXMLParser2", (RTS_UINTPTR)CreateXMLParser2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParserCreateXMLParser2
	#define EXT_CmpXMLParserCreateXMLParser2
	#define GET_CmpXMLParserCreateXMLParser2  ERR_OK
	#define CAL_CmpXMLParserCreateXMLParser2 pICmpXMLParser->ICreateXMLParser2
	#define CHK_CmpXMLParserCreateXMLParser2 (pICmpXMLParser != NULL)
	#define EXP_CmpXMLParserCreateXMLParser2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CreateXMLParser2
	#define EXT_CreateXMLParser2
	#define GET_CreateXMLParser2(fl)  CAL_CMGETAPI( "CreateXMLParser2" ) 
	#define CAL_CreateXMLParser2 pICmpXMLParser->ICreateXMLParser2
	#define CHK_CreateXMLParser2 (pICmpXMLParser != NULL)
	#define EXP_CreateXMLParser2  CAL_CMEXPAPI( "CreateXMLParser2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CreateXMLParser2  PFCREATEXMLPARSER2 pfCreateXMLParser2;
	#define EXT_CreateXMLParser2  extern PFCREATEXMLPARSER2 pfCreateXMLParser2;
	#define GET_CreateXMLParser2(fl)  s_pfCMGetAPI2( "CreateXMLParser2", (RTS_VOID_FCTPTR *)&pfCreateXMLParser2, (fl), 0, 0)
	#define CAL_CreateXMLParser2  pfCreateXMLParser2
	#define CHK_CreateXMLParser2  (pfCreateXMLParser2 != NULL)
	#define EXP_CreateXMLParser2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CreateXMLParser2", (RTS_UINTPTR)CreateXMLParser2, 0, 0) 
#endif




/**
* <description> Creates a XML parser </description>
* <param name="pszXMLFileName" type="IN">Name of the XML file</param>
* <param name="pcwEncoding" type="IN">Optional encoding. See category "Encoding".</param>
* <param name="pResult" type="OUT">Pointer to error code</param>
* <param name="UserData" type="OUT">User data is returned by each parser callback of this instance.</param>
* <result>Handle to XML parser instance</result>
*/
RTS_HANDLE CDECL CreateXMLParser3(char *pszXMLFileName, const XML_Char *pcwEncoding, RTS_RESULT *pResult, void *UserData);
typedef RTS_HANDLE (CDECL * PFCREATEXMLPARSER3) (char *pszXMLFileName, const XML_Char *pcwEncoding, RTS_RESULT *pResult, void *UserData);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(CREATEXMLPARSER3_NOTIMPLEMENTED)
	#define USE_CreateXMLParser3
	#define EXT_CreateXMLParser3
	#define GET_CreateXMLParser3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CreateXMLParser3(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_CreateXMLParser3  FALSE
	#define EXP_CreateXMLParser3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CreateXMLParser3
	#define EXT_CreateXMLParser3
	#define GET_CreateXMLParser3(fl)  CAL_CMGETAPI( "CreateXMLParser3" ) 
	#define CAL_CreateXMLParser3  CreateXMLParser3
	#define CHK_CreateXMLParser3  TRUE
	#define EXP_CreateXMLParser3  CAL_CMEXPAPI( "CreateXMLParser3" ) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_CreateXMLParser3
	#define EXT_CreateXMLParser3
	#define GET_CreateXMLParser3(fl)  CAL_CMGETAPI( "CreateXMLParser3" ) 
	#define CAL_CreateXMLParser3  CreateXMLParser3
	#define CHK_CreateXMLParser3  TRUE
	#define EXP_CreateXMLParser3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CreateXMLParser3", (RTS_UINTPTR)CreateXMLParser3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParserCreateXMLParser3
	#define EXT_CmpXMLParserCreateXMLParser3
	#define GET_CmpXMLParserCreateXMLParser3  ERR_OK
	#define CAL_CmpXMLParserCreateXMLParser3 pICmpXMLParser->ICreateXMLParser3
	#define CHK_CmpXMLParserCreateXMLParser3 (pICmpXMLParser != NULL)
	#define EXP_CmpXMLParserCreateXMLParser3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CreateXMLParser3
	#define EXT_CreateXMLParser3
	#define GET_CreateXMLParser3(fl)  CAL_CMGETAPI( "CreateXMLParser3" ) 
	#define CAL_CreateXMLParser3 pICmpXMLParser->ICreateXMLParser3
	#define CHK_CreateXMLParser3 (pICmpXMLParser != NULL)
	#define EXP_CreateXMLParser3  CAL_CMEXPAPI( "CreateXMLParser3" ) 
#else /* DYNAMIC_LINK */
	#define USE_CreateXMLParser3  PFCREATEXMLPARSER3 pfCreateXMLParser3;
	#define EXT_CreateXMLParser3  extern PFCREATEXMLPARSER3 pfCreateXMLParser3;
	#define GET_CreateXMLParser3(fl)  s_pfCMGetAPI2( "CreateXMLParser3", (RTS_VOID_FCTPTR *)&pfCreateXMLParser3, (fl), 0, 0)
	#define CAL_CreateXMLParser3  pfCreateXMLParser3
	#define CHK_CreateXMLParser3  (pfCreateXMLParser3 != NULL)
	#define EXP_CreateXMLParser3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CreateXMLParser3", (RTS_UINTPTR)CreateXMLParser3, 0, 0) 
#endif




/**
 * <description> Frees the XML parser </description>
 * <param name="hParser" type="IN">Handle of the XML parser instance</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL FreeXMLParser(RTS_HANDLE hParser);
typedef RTS_RESULT (CDECL * PFFREEXMLPARSER) (RTS_HANDLE hParser);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(FREEXMLPARSER_NOTIMPLEMENTED)
	#define USE_FreeXMLParser
	#define EXT_FreeXMLParser
	#define GET_FreeXMLParser(fl)  ERR_NOTIMPLEMENTED
	#define CAL_FreeXMLParser(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_FreeXMLParser  FALSE
	#define EXP_FreeXMLParser  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_FreeXMLParser
	#define EXT_FreeXMLParser
	#define GET_FreeXMLParser(fl)  CAL_CMGETAPI( "FreeXMLParser" ) 
	#define CAL_FreeXMLParser  FreeXMLParser
	#define CHK_FreeXMLParser  TRUE
	#define EXP_FreeXMLParser  CAL_CMEXPAPI( "FreeXMLParser" ) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_FreeXMLParser
	#define EXT_FreeXMLParser
	#define GET_FreeXMLParser(fl)  CAL_CMGETAPI( "FreeXMLParser" ) 
	#define CAL_FreeXMLParser  FreeXMLParser
	#define CHK_FreeXMLParser  TRUE
	#define EXP_FreeXMLParser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"FreeXMLParser", (RTS_UINTPTR)FreeXMLParser, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParserFreeXMLParser
	#define EXT_CmpXMLParserFreeXMLParser
	#define GET_CmpXMLParserFreeXMLParser  ERR_OK
	#define CAL_CmpXMLParserFreeXMLParser pICmpXMLParser->IFreeXMLParser
	#define CHK_CmpXMLParserFreeXMLParser (pICmpXMLParser != NULL)
	#define EXP_CmpXMLParserFreeXMLParser  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_FreeXMLParser
	#define EXT_FreeXMLParser
	#define GET_FreeXMLParser(fl)  CAL_CMGETAPI( "FreeXMLParser" ) 
	#define CAL_FreeXMLParser pICmpXMLParser->IFreeXMLParser
	#define CHK_FreeXMLParser (pICmpXMLParser != NULL)
	#define EXP_FreeXMLParser  CAL_CMEXPAPI( "FreeXMLParser" ) 
#else /* DYNAMIC_LINK */
	#define USE_FreeXMLParser  PFFREEXMLPARSER pfFreeXMLParser;
	#define EXT_FreeXMLParser  extern PFFREEXMLPARSER pfFreeXMLParser;
	#define GET_FreeXMLParser(fl)  s_pfCMGetAPI2( "FreeXMLParser", (RTS_VOID_FCTPTR *)&pfFreeXMLParser, (fl), 0, 0)
	#define CAL_FreeXMLParser  pfFreeXMLParser
	#define CHK_FreeXMLParser  (pfFreeXMLParser != NULL)
	#define EXP_FreeXMLParser  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"FreeXMLParser", (RTS_UINTPTR)FreeXMLParser, 0, 0) 
#endif




/**
 * <description> Sets the XML element handler </description>
 * <param name="hParser" type="IN">Handle of the XML parser instance</param>
 * <param name="start" type="IN">Callback function for the StartElementHandler. If this is not set, the events above are sent instead!</param>
 * <param name="end" type="IN">Callback function for the EndElementHandler. If this is not set, the events above are sent instead!</param>
 * <result>ERR_OK</result>
 */
RTS_RESULT CDECL SetXMLElementHandler(RTS_HANDLE hParser, PF_XML_StartElementHandler start, PF_XML_EndElementHandler end);
typedef RTS_RESULT (CDECL * PFSETXMLELEMENTHANDLER) (RTS_HANDLE hParser, PF_XML_StartElementHandler start, PF_XML_EndElementHandler end);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(SETXMLELEMENTHANDLER_NOTIMPLEMENTED)
	#define USE_SetXMLElementHandler
	#define EXT_SetXMLElementHandler
	#define GET_SetXMLElementHandler(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SetXMLElementHandler(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SetXMLElementHandler  FALSE
	#define EXP_SetXMLElementHandler  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SetXMLElementHandler
	#define EXT_SetXMLElementHandler
	#define GET_SetXMLElementHandler(fl)  CAL_CMGETAPI( "SetXMLElementHandler" ) 
	#define CAL_SetXMLElementHandler  SetXMLElementHandler
	#define CHK_SetXMLElementHandler  TRUE
	#define EXP_SetXMLElementHandler  CAL_CMEXPAPI( "SetXMLElementHandler" ) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_SetXMLElementHandler
	#define EXT_SetXMLElementHandler
	#define GET_SetXMLElementHandler(fl)  CAL_CMGETAPI( "SetXMLElementHandler" ) 
	#define CAL_SetXMLElementHandler  SetXMLElementHandler
	#define CHK_SetXMLElementHandler  TRUE
	#define EXP_SetXMLElementHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SetXMLElementHandler", (RTS_UINTPTR)SetXMLElementHandler, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParserSetXMLElementHandler
	#define EXT_CmpXMLParserSetXMLElementHandler
	#define GET_CmpXMLParserSetXMLElementHandler  ERR_OK
	#define CAL_CmpXMLParserSetXMLElementHandler pICmpXMLParser->ISetXMLElementHandler
	#define CHK_CmpXMLParserSetXMLElementHandler (pICmpXMLParser != NULL)
	#define EXP_CmpXMLParserSetXMLElementHandler  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SetXMLElementHandler
	#define EXT_SetXMLElementHandler
	#define GET_SetXMLElementHandler(fl)  CAL_CMGETAPI( "SetXMLElementHandler" ) 
	#define CAL_SetXMLElementHandler pICmpXMLParser->ISetXMLElementHandler
	#define CHK_SetXMLElementHandler (pICmpXMLParser != NULL)
	#define EXP_SetXMLElementHandler  CAL_CMEXPAPI( "SetXMLElementHandler" ) 
#else /* DYNAMIC_LINK */
	#define USE_SetXMLElementHandler  PFSETXMLELEMENTHANDLER pfSetXMLElementHandler;
	#define EXT_SetXMLElementHandler  extern PFSETXMLELEMENTHANDLER pfSetXMLElementHandler;
	#define GET_SetXMLElementHandler(fl)  s_pfCMGetAPI2( "SetXMLElementHandler", (RTS_VOID_FCTPTR *)&pfSetXMLElementHandler, (fl), 0, 0)
	#define CAL_SetXMLElementHandler  pfSetXMLElementHandler
	#define CHK_SetXMLElementHandler  (pfSetXMLElementHandler != NULL)
	#define EXP_SetXMLElementHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SetXMLElementHandler", (RTS_UINTPTR)SetXMLElementHandler, 0, 0) 
#endif




/**
 * <description> Sets the XML data handler </description>
 * <param name="hParser" type="IN">Handle of the XML parser instance</param>
 * <param name="handler" type="IN">Callback function for the CharacterDataHandler. If this is not set, the events above are sent instead!</param>
 * <result>ERR_OK</result>
 */
RTS_RESULT CDECL SetXMLCharacterDataHandler(RTS_HANDLE hParser, PF_XML_CharacterDataHandler handler);
typedef RTS_RESULT (CDECL * PFSETXMLCHARACTERDATAHANDLER) (RTS_HANDLE hParser, PF_XML_CharacterDataHandler handler);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(SETXMLCHARACTERDATAHANDLER_NOTIMPLEMENTED)
	#define USE_SetXMLCharacterDataHandler
	#define EXT_SetXMLCharacterDataHandler
	#define GET_SetXMLCharacterDataHandler(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SetXMLCharacterDataHandler(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SetXMLCharacterDataHandler  FALSE
	#define EXP_SetXMLCharacterDataHandler  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SetXMLCharacterDataHandler
	#define EXT_SetXMLCharacterDataHandler
	#define GET_SetXMLCharacterDataHandler(fl)  CAL_CMGETAPI( "SetXMLCharacterDataHandler" ) 
	#define CAL_SetXMLCharacterDataHandler  SetXMLCharacterDataHandler
	#define CHK_SetXMLCharacterDataHandler  TRUE
	#define EXP_SetXMLCharacterDataHandler  CAL_CMEXPAPI( "SetXMLCharacterDataHandler" ) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_SetXMLCharacterDataHandler
	#define EXT_SetXMLCharacterDataHandler
	#define GET_SetXMLCharacterDataHandler(fl)  CAL_CMGETAPI( "SetXMLCharacterDataHandler" ) 
	#define CAL_SetXMLCharacterDataHandler  SetXMLCharacterDataHandler
	#define CHK_SetXMLCharacterDataHandler  TRUE
	#define EXP_SetXMLCharacterDataHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SetXMLCharacterDataHandler", (RTS_UINTPTR)SetXMLCharacterDataHandler, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParserSetXMLCharacterDataHandler
	#define EXT_CmpXMLParserSetXMLCharacterDataHandler
	#define GET_CmpXMLParserSetXMLCharacterDataHandler  ERR_OK
	#define CAL_CmpXMLParserSetXMLCharacterDataHandler pICmpXMLParser->ISetXMLCharacterDataHandler
	#define CHK_CmpXMLParserSetXMLCharacterDataHandler (pICmpXMLParser != NULL)
	#define EXP_CmpXMLParserSetXMLCharacterDataHandler  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SetXMLCharacterDataHandler
	#define EXT_SetXMLCharacterDataHandler
	#define GET_SetXMLCharacterDataHandler(fl)  CAL_CMGETAPI( "SetXMLCharacterDataHandler" ) 
	#define CAL_SetXMLCharacterDataHandler pICmpXMLParser->ISetXMLCharacterDataHandler
	#define CHK_SetXMLCharacterDataHandler (pICmpXMLParser != NULL)
	#define EXP_SetXMLCharacterDataHandler  CAL_CMEXPAPI( "SetXMLCharacterDataHandler" ) 
#else /* DYNAMIC_LINK */
	#define USE_SetXMLCharacterDataHandler  PFSETXMLCHARACTERDATAHANDLER pfSetXMLCharacterDataHandler;
	#define EXT_SetXMLCharacterDataHandler  extern PFSETXMLCHARACTERDATAHANDLER pfSetXMLCharacterDataHandler;
	#define GET_SetXMLCharacterDataHandler(fl)  s_pfCMGetAPI2( "SetXMLCharacterDataHandler", (RTS_VOID_FCTPTR *)&pfSetXMLCharacterDataHandler, (fl), 0, 0)
	#define CAL_SetXMLCharacterDataHandler  pfSetXMLCharacterDataHandler
	#define CHK_SetXMLCharacterDataHandler  (pfSetXMLCharacterDataHandler != NULL)
	#define EXP_SetXMLCharacterDataHandler  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SetXMLCharacterDataHandler", (RTS_UINTPTR)SetXMLCharacterDataHandler, 0, 0) 
#endif




/**
 * <description> Starts the parsing process </description>
 * <param name="hParser" type="IN">Handle of the XML parser instance</param>
 * <param name="s" type="IN">Pointer to buffer that contains the XML file</param>
 * <param name="len" type="IN">Length of the XML file</param>
 * <param name="isFinal" type="IN"></param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL ParseXML(RTS_HANDLE hParser, const char *s, RTS_SIZE len, RTS_BOOL isFinal);
typedef RTS_RESULT (CDECL * PFPARSEXML) (RTS_HANDLE hParser, const char *s, RTS_SIZE len, RTS_BOOL isFinal);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(PARSEXML_NOTIMPLEMENTED)
	#define USE_ParseXML
	#define EXT_ParseXML
	#define GET_ParseXML(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ParseXML(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ParseXML  FALSE
	#define EXP_ParseXML  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ParseXML
	#define EXT_ParseXML
	#define GET_ParseXML(fl)  CAL_CMGETAPI( "ParseXML" ) 
	#define CAL_ParseXML  ParseXML
	#define CHK_ParseXML  TRUE
	#define EXP_ParseXML  CAL_CMEXPAPI( "ParseXML" ) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_ParseXML
	#define EXT_ParseXML
	#define GET_ParseXML(fl)  CAL_CMGETAPI( "ParseXML" ) 
	#define CAL_ParseXML  ParseXML
	#define CHK_ParseXML  TRUE
	#define EXP_ParseXML  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ParseXML", (RTS_UINTPTR)ParseXML, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParserParseXML
	#define EXT_CmpXMLParserParseXML
	#define GET_CmpXMLParserParseXML  ERR_OK
	#define CAL_CmpXMLParserParseXML pICmpXMLParser->IParseXML
	#define CHK_CmpXMLParserParseXML (pICmpXMLParser != NULL)
	#define EXP_CmpXMLParserParseXML  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ParseXML
	#define EXT_ParseXML
	#define GET_ParseXML(fl)  CAL_CMGETAPI( "ParseXML" ) 
	#define CAL_ParseXML pICmpXMLParser->IParseXML
	#define CHK_ParseXML (pICmpXMLParser != NULL)
	#define EXP_ParseXML  CAL_CMEXPAPI( "ParseXML" ) 
#else /* DYNAMIC_LINK */
	#define USE_ParseXML  PFPARSEXML pfParseXML;
	#define EXT_ParseXML  extern PFPARSEXML pfParseXML;
	#define GET_ParseXML(fl)  s_pfCMGetAPI2( "ParseXML", (RTS_VOID_FCTPTR *)&pfParseXML, (fl), 0, 0)
	#define CAL_ParseXML  pfParseXML
	#define CHK_ParseXML  (pfParseXML != NULL)
	#define EXP_ParseXML  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ParseXML", (RTS_UINTPTR)ParseXML, 0, 0) 
#endif




/**
 * <description> Starts the parsing process </description>
 * <param name="hParser" type="IN">Handle of the XML parser instance</param>
 * <param name="isFinal" type="IN"></param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL ParseXML2(RTS_HANDLE hParser, RTS_BOOL isFinal);
typedef RTS_RESULT (CDECL * PFPARSEXML2) (RTS_HANDLE hParser, RTS_BOOL isFinal);
#if defined(CMPXMLPARSER_NOTIMPLEMENTED) || defined(PARSEXML2_NOTIMPLEMENTED)
	#define USE_ParseXML2
	#define EXT_ParseXML2
	#define GET_ParseXML2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_ParseXML2(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_ParseXML2  FALSE
	#define EXP_ParseXML2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_ParseXML2
	#define EXT_ParseXML2
	#define GET_ParseXML2(fl)  CAL_CMGETAPI( "ParseXML2" ) 
	#define CAL_ParseXML2  ParseXML2
	#define CHK_ParseXML2  TRUE
	#define EXP_ParseXML2  CAL_CMEXPAPI( "ParseXML2" ) 
#elif defined(MIXED_LINK) && !defined(CMPXMLPARSER_EXTERNAL)
	#define USE_ParseXML2
	#define EXT_ParseXML2
	#define GET_ParseXML2(fl)  CAL_CMGETAPI( "ParseXML2" ) 
	#define CAL_ParseXML2  ParseXML2
	#define CHK_ParseXML2  TRUE
	#define EXP_ParseXML2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ParseXML2", (RTS_UINTPTR)ParseXML2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpXMLParserParseXML2
	#define EXT_CmpXMLParserParseXML2
	#define GET_CmpXMLParserParseXML2  ERR_OK
	#define CAL_CmpXMLParserParseXML2 pICmpXMLParser->IParseXML2
	#define CHK_CmpXMLParserParseXML2 (pICmpXMLParser != NULL)
	#define EXP_CmpXMLParserParseXML2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_ParseXML2
	#define EXT_ParseXML2
	#define GET_ParseXML2(fl)  CAL_CMGETAPI( "ParseXML2" ) 
	#define CAL_ParseXML2 pICmpXMLParser->IParseXML2
	#define CHK_ParseXML2 (pICmpXMLParser != NULL)
	#define EXP_ParseXML2  CAL_CMEXPAPI( "ParseXML2" ) 
#else /* DYNAMIC_LINK */
	#define USE_ParseXML2  PFPARSEXML2 pfParseXML2;
	#define EXT_ParseXML2  extern PFPARSEXML2 pfParseXML2;
	#define GET_ParseXML2(fl)  s_pfCMGetAPI2( "ParseXML2", (RTS_VOID_FCTPTR *)&pfParseXML2, (fl), 0, 0)
	#define CAL_ParseXML2  pfParseXML2
	#define CHK_ParseXML2  (pfParseXML2 != NULL)
	#define EXP_ParseXML2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"ParseXML2", (RTS_UINTPTR)ParseXML2, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFCREATEXMLPARSER ICreateXMLParser;
 	PFCREATEXMLPARSER2 ICreateXMLParser2;
 	PFCREATEXMLPARSER3 ICreateXMLParser3;
 	PFFREEXMLPARSER IFreeXMLParser;
 	PFSETXMLELEMENTHANDLER ISetXMLElementHandler;
 	PFSETXMLCHARACTERDATAHANDLER ISetXMLCharacterDataHandler;
 	PFPARSEXML IParseXML;
 	PFPARSEXML2 IParseXML2;
 } ICmpXMLParser_C;

#ifdef CPLUSPLUS
class ICmpXMLParser : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ICreateXMLParser(const XML_Char *pcwEncoding) =0;
		virtual RTS_HANDLE CDECL ICreateXMLParser2(char *pszXMLFileName, const XML_Char *pcwEncoding, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ICreateXMLParser3(char *pszXMLFileName, const XML_Char *pcwEncoding, RTS_RESULT *pResult, void *UserData) =0;
		virtual RTS_RESULT CDECL IFreeXMLParser(RTS_HANDLE hParser) =0;
		virtual RTS_RESULT CDECL ISetXMLElementHandler(RTS_HANDLE hParser, PF_XML_StartElementHandler start, PF_XML_EndElementHandler end) =0;
		virtual RTS_RESULT CDECL ISetXMLCharacterDataHandler(RTS_HANDLE hParser, PF_XML_CharacterDataHandler handler) =0;
		virtual RTS_RESULT CDECL IParseXML(RTS_HANDLE hParser, const char *s, RTS_SIZE len, RTS_BOOL isFinal) =0;
		virtual RTS_RESULT CDECL IParseXML2(RTS_HANDLE hParser, RTS_BOOL isFinal) =0;
};
	#ifndef ITF_CmpXMLParser
		#define ITF_CmpXMLParser static ICmpXMLParser *pICmpXMLParser = NULL;
	#endif
	#define EXTITF_CmpXMLParser
#else	/*CPLUSPLUS*/
	typedef ICmpXMLParser_C		ICmpXMLParser;
	#ifndef ITF_CmpXMLParser
		#define ITF_CmpXMLParser
	#endif
	#define EXTITF_CmpXMLParser
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPXMLPARSERITF_H_*/
