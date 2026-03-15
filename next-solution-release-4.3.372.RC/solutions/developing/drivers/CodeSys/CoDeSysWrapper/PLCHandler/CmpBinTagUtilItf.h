 /**
 * <interfacename>CmpBinTagUtil</interfacename>
 * <description> 
 *	<p>Interface for the binary tag utility.</p>
 * </description>
 *
 * <copyright>
 * Copyright (c) 2017-2018 CODESYS GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.
 * </copyright>
 */


	
	
#ifndef _CMPBINTAGUTILITF_H_
#define _CMPBINTAGUTILITF_H_

#include "CmpStd.h"

 

 




/**
 * <category>Static defines</category>
 * <description>Maximum tag length that can be expected. Can be used to check, if the response fits into the communication buffer.</description>
 */
#define BTU_MAX_TAG_LEN		12

/**
 * <description>
 *   This struct defines an alignment property. 
 *   The meaning of the members is
 *   "Align to an address for which the following equation holds:
 *   address % sModulus = sRemainder"
 *   Eg.: 
 *   (4,0) aligns to a 4 byte boundary
 *   (4,1) aligns to a 4 byte boundary + 1 (1,5,9,13,...)
 *   (2,0) aligns to an equal address
 *   (2,1) aligns to an odd address
 *   (1,0) aligns to any address
 * </description>
 */    
typedef struct
{
	unsigned short usModulus;
	unsigned short usRemainder;
} BTAG_ALIGNMENT;

static BTAG_ALIGNMENT const align_40 = {4,0};
static BTAG_ALIGNMENT const align_42 = {4,2};

#ifndef BTAG_MAX_NESTED_TAGS
	#define BTAG_MAX_NESTED_TAGS 10
#endif

#ifndef BTAG_MAX_STATIC_FILESTREAM_BUFFER
	#define BTAG_MAX_STATIC_FILESTREAM_BUFFER		(NETSERVER_BUFFERSIZE / (NETSERVER_MAXCHANNELS * 2))
#endif

typedef struct
{
	RTS_UI16 usHeaderTag;
	RTS_UI16 usHeaderLength;
	RTS_UI16 usServiceGroup;
	RTS_UI16 usService;
	RTS_UI32 ulSessionID;
	RTS_UI32 ulServiceLength;
} HEADER_TAG;

typedef struct
{
	RTS_UI16 usHeaderTag;
	RTS_UI16 usHeaderLength;
	RTS_UI16 usServiceGroup;
	RTS_UI16 usService;
	RTS_UI32 ulSessionID;
	RTS_UI32 ulServiceLength;
	RTS_UI16 usCustomerId;
	RTS_UI16 usReserved; /* Must be 0x0000 */
} HEADER_TAG_EXT;


#ifdef RTS_SIXTEENBITBYTES
	#define sizeof_HEADER_TAG_EXT	sizeof(HEADER_TAG_EXT) * 2
#endif


typedef struct _BTAG_WriterTagInfo
{
	RTS_UI32 ulTagId;
	RTS_UI32 ulSizePos; /* Index of the length field */
	RTS_UI32 ulSize; /* Current content size of the tag */
	RTS_UI8 ucSizeLength; /* Will contain values from 1 to 8 */
	RTS_I8 bIsDataTag;
}BTAG_WRITERTAGINFO;

typedef struct _BTAG_ReaderTagInfo
{
	RTS_UI32 ulTagId;
	RTS_UI32 ulSize;		/* Size of the tag */
	RTS_UI32 ulContentPos; /* Index of the tag content*/
}BTAG_READERTAGINFO;


#define BINTAGTYPE_STREAM		1
#define BINTAGTYPE_FILE			2
#define BINTAGTYPE_FILESTREAM	3

typedef void (CDECL *PFUPDATECRC)(RTS_UI8 *pbyData, RTS_UI32 ulDataLen, void *pParameter);

/**
 * <description>
 *   This struct holds the internal state of a writer and should be 
 *   treated as opaque by an application.
 *   Do not alter!
 * </description>
 */
typedef struct
{
	RTS_UI8 *pBuffer;
	RTS_HANDLE hFile;
	RTS_UI32 ulBufferSize;
	RTS_UI32 ulPos;

	BTAG_WRITERTAGINFO tagStack[BTAG_MAX_NESTED_TAGS];

	int nStackPos; /* Points to the index of the current tag. Initially set to -1 (toplevel, no current tag).
	                   Must not exceed BTAG_MAX_NESTED_TAGS-1 */
	
	int iType;
	RTS_UI32 ulEndServicePos;
	int bSwapHeader;
	int	bBufferOverflow;
	PFUPDATECRC pfUpdateCRC;
	void *pParameterUpdateCRC;
}BINTAGWRITER;

typedef struct
{
	RTS_UI32 ulPos;

	BTAG_WRITERTAGINFO tagStack[BTAG_MAX_NESTED_TAGS];
	int nStackPos; /* Points to the index of the current tag. Initially set to -1 (toplevel, no current tag).
	                   Must not exceed BTAG_MAX_NESTED_TAGS-1 */
	int	bBufferOverflow;
}BINTAGSAVEPOINT;

/**
 * <description>
 *   This struct holds the internal state of a reader and should be 
 *   treated as opaque by an application.
 *   Do not alter!
 * </description>
 */
typedef struct
{
	RTS_UI8 *pBuffer;
	RTS_UI32 ulBufferSize;
	RTS_HANDLE hFile;

	RTS_UI32 ulPos;

	BTAG_READERTAGINFO tagStack[BTAG_MAX_NESTED_TAGS];
	int nStackPos; /* Points to the index of the current tag. Initially set to -1 (toplevel, no current tag).
	                   Must not exceed BTAG_MAX_NESTED_TAGS-1 */
	int iType;
	RTS_UI32 ulStartServicePos;
}BINTAGREADER;

typedef struct 
{
	RTS_UI32 ulPos;

	BTAG_READERTAGINFO tagStack[BTAG_MAX_NESTED_TAGS];
	int nStackPos; /* Points to the index of the current tag. Initially set to -1 (toplevel, no current tag).
	                   Must not exceed BTAG_MAX_NESTED_TAGS-1 */
}BINTAGREADERSAVEPOINT;

/* Enumeration of element types */
enum
{
	BTAG_ET_STARTTAG = 0,
	BTAG_ET_ENDTAG = 1,
	BTAG_ET_EOF = 2
};


#ifdef __cplusplus
extern "C" {
#endif
 

/**
 * <description>
 *   Swap the header of a service. 
 * </description>
 * <param name="pHeader" type="INOUT">
 *   Pass in a HEADER_TAG_EXT struct that should be swapped.
 * </param>
 * <param name="bSwap" type="IN">
 *   Determines, if the header should be swapped (1) or not (0).
 * </param>
 */
RTS_RESULT CDECL BTagSwapHeader(HEADER_TAG_EXT* pHeader, RTS_I32 bSwap);
typedef RTS_RESULT (CDECL * PFBTAGSWAPHEADER) (HEADER_TAG_EXT* pHeader, RTS_I32 bSwap);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGSWAPHEADER_NOTIMPLEMENTED)
	#define USE_BTagSwapHeader
	#define EXT_BTagSwapHeader
	#define GET_BTagSwapHeader(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagSwapHeader(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagSwapHeader  FALSE
	#define EXP_BTagSwapHeader  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagSwapHeader
	#define EXT_BTagSwapHeader
	#define GET_BTagSwapHeader(fl)  CAL_CMGETAPI( "BTagSwapHeader" ) 
	#define CAL_BTagSwapHeader  BTagSwapHeader
	#define CHK_BTagSwapHeader  TRUE
	#define EXP_BTagSwapHeader  CAL_CMEXPAPI( "BTagSwapHeader" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagSwapHeader
	#define EXT_BTagSwapHeader
	#define GET_BTagSwapHeader(fl)  CAL_CMGETAPI( "BTagSwapHeader" ) 
	#define CAL_BTagSwapHeader  BTagSwapHeader
	#define CHK_BTagSwapHeader  TRUE
	#define EXP_BTagSwapHeader  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagSwapHeader", (RTS_UINTPTR)BTagSwapHeader, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagSwapHeader
	#define EXT_CmpBinTagUtilBTagSwapHeader
	#define GET_CmpBinTagUtilBTagSwapHeader  ERR_OK
	#define CAL_CmpBinTagUtilBTagSwapHeader pICmpBinTagUtil->IBTagSwapHeader
	#define CHK_CmpBinTagUtilBTagSwapHeader (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagSwapHeader  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagSwapHeader
	#define EXT_BTagSwapHeader
	#define GET_BTagSwapHeader(fl)  CAL_CMGETAPI( "BTagSwapHeader" ) 
	#define CAL_BTagSwapHeader pICmpBinTagUtil->IBTagSwapHeader
	#define CHK_BTagSwapHeader (pICmpBinTagUtil != NULL)
	#define EXP_BTagSwapHeader  CAL_CMEXPAPI( "BTagSwapHeader" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagSwapHeader  PFBTAGSWAPHEADER pfBTagSwapHeader;
	#define EXT_BTagSwapHeader  extern PFBTAGSWAPHEADER pfBTagSwapHeader;
	#define GET_BTagSwapHeader(fl)  s_pfCMGetAPI2( "BTagSwapHeader", (RTS_VOID_FCTPTR *)&pfBTagSwapHeader, (fl), 0, 0)
	#define CAL_BTagSwapHeader  pfBTagSwapHeader
	#define CHK_BTagSwapHeader  (pfBTagSwapHeader != NULL)
	#define EXP_BTagSwapHeader  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagSwapHeader", (RTS_UINTPTR)BTagSwapHeader, 0, 0) 
#endif





/* --------- The writer interface ---------- */

/**
 * <description>
 *   Initialize a writer. 
 * </description>
 * <param name="pWriter" type="INOUT">
 *   Pass in a BINTAGWRITER struct that will be initialized to a empty writer.
 * </param>
 * <param name="pBuffer" type="IN">
 *   The buffer that the writer will write to. 
 *   The buffer should not be altered until the writer has finished.
 * </param>
 * <param name="ulBufferSize" type="IN">
 *   The size of the buffer. The writer will fail if a write operation would exceed the buffer.
 * </param>
 * <param name="bSwapHeader" type="IN">
 *    Clients have to set this flag, if the addressed server has a different byte order. 
 *    Must be always FALSE for server implementations or if the client has the same byte order as the server. 
 * </param>
 */
RTS_RESULT CDECL BTagWriterInit2(BINTAGWRITER *pWriter, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize, RTS_I32 bSwapHeader);
typedef RTS_RESULT (CDECL * PFBTAGWRITERINIT2) (BINTAGWRITER *pWriter, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize, RTS_I32 bSwapHeader);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERINIT2_NOTIMPLEMENTED)
	#define USE_BTagWriterInit2
	#define EXT_BTagWriterInit2
	#define GET_BTagWriterInit2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterInit2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterInit2  FALSE
	#define EXP_BTagWriterInit2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterInit2
	#define EXT_BTagWriterInit2
	#define GET_BTagWriterInit2(fl)  CAL_CMGETAPI( "BTagWriterInit2" ) 
	#define CAL_BTagWriterInit2  BTagWriterInit2
	#define CHK_BTagWriterInit2  TRUE
	#define EXP_BTagWriterInit2  CAL_CMEXPAPI( "BTagWriterInit2" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterInit2
	#define EXT_BTagWriterInit2
	#define GET_BTagWriterInit2(fl)  CAL_CMGETAPI( "BTagWriterInit2" ) 
	#define CAL_BTagWriterInit2  BTagWriterInit2
	#define CHK_BTagWriterInit2  TRUE
	#define EXP_BTagWriterInit2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterInit2", (RTS_UINTPTR)BTagWriterInit2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterInit2
	#define EXT_CmpBinTagUtilBTagWriterInit2
	#define GET_CmpBinTagUtilBTagWriterInit2  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterInit2 pICmpBinTagUtil->IBTagWriterInit2
	#define CHK_CmpBinTagUtilBTagWriterInit2 (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterInit2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterInit2
	#define EXT_BTagWriterInit2
	#define GET_BTagWriterInit2(fl)  CAL_CMGETAPI( "BTagWriterInit2" ) 
	#define CAL_BTagWriterInit2 pICmpBinTagUtil->IBTagWriterInit2
	#define CHK_BTagWriterInit2 (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterInit2  CAL_CMEXPAPI( "BTagWriterInit2" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterInit2  PFBTAGWRITERINIT2 pfBTagWriterInit2;
	#define EXT_BTagWriterInit2  extern PFBTAGWRITERINIT2 pfBTagWriterInit2;
	#define GET_BTagWriterInit2(fl)  s_pfCMGetAPI2( "BTagWriterInit2", (RTS_VOID_FCTPTR *)&pfBTagWriterInit2, (fl), 0, 0)
	#define CAL_BTagWriterInit2  pfBTagWriterInit2
	#define CHK_BTagWriterInit2  (pfBTagWriterInit2 != NULL)
	#define EXP_BTagWriterInit2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterInit2", (RTS_UINTPTR)BTagWriterInit2, 0, 0) 
#endif




/**
 * <description>
 *   Initialize a writer. Can be used by servers or if the client and the server have the same byte order.
 * </description>
 * <param name="pWriter" type="INOUT">
 *   Pass in a BINTAGWRITER struct that will be initialized to a empty writer.
 * </param>
 * <param name="pBuffer" type="IN">
 *   The buffer that the writer will write to. 
 *   The buffer should not be altered until the writer has finished.
 * </param>
 * <param name="ulBufferSize" type="IN">
 *   The size of the buffer. The writer will fail if a write operation would exceed the buffer.
 * </param>
 */
RTS_RESULT CDECL BTagWriterInit(BINTAGWRITER *pWriter, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize);
typedef RTS_RESULT (CDECL * PFBTAGWRITERINIT) (BINTAGWRITER *pWriter, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERINIT_NOTIMPLEMENTED)
	#define USE_BTagWriterInit
	#define EXT_BTagWriterInit
	#define GET_BTagWriterInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterInit(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterInit  FALSE
	#define EXP_BTagWriterInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterInit
	#define EXT_BTagWriterInit
	#define GET_BTagWriterInit(fl)  CAL_CMGETAPI( "BTagWriterInit" ) 
	#define CAL_BTagWriterInit  BTagWriterInit
	#define CHK_BTagWriterInit  TRUE
	#define EXP_BTagWriterInit  CAL_CMEXPAPI( "BTagWriterInit" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterInit
	#define EXT_BTagWriterInit
	#define GET_BTagWriterInit(fl)  CAL_CMGETAPI( "BTagWriterInit" ) 
	#define CAL_BTagWriterInit  BTagWriterInit
	#define CHK_BTagWriterInit  TRUE
	#define EXP_BTagWriterInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterInit", (RTS_UINTPTR)BTagWriterInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterInit
	#define EXT_CmpBinTagUtilBTagWriterInit
	#define GET_CmpBinTagUtilBTagWriterInit  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterInit pICmpBinTagUtil->IBTagWriterInit
	#define CHK_CmpBinTagUtilBTagWriterInit (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterInit
	#define EXT_BTagWriterInit
	#define GET_BTagWriterInit(fl)  CAL_CMGETAPI( "BTagWriterInit" ) 
	#define CAL_BTagWriterInit pICmpBinTagUtil->IBTagWriterInit
	#define CHK_BTagWriterInit (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterInit  CAL_CMEXPAPI( "BTagWriterInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterInit  PFBTAGWRITERINIT pfBTagWriterInit;
	#define EXT_BTagWriterInit  extern PFBTAGWRITERINIT pfBTagWriterInit;
	#define GET_BTagWriterInit(fl)  s_pfCMGetAPI2( "BTagWriterInit", (RTS_VOID_FCTPTR *)&pfBTagWriterInit, (fl), 0, 0)
	#define CAL_BTagWriterInit  pfBTagWriterInit
	#define CHK_BTagWriterInit  (pfBTagWriterInit != NULL)
	#define EXP_BTagWriterInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterInit", (RTS_UINTPTR)BTagWriterInit, 0, 0) 
#endif




/**
 * <description>
 *   Start a new service. Must be called after BTagWriterInit.
 * </description>
 * <param name="pWriter" type="INOUT">
 *   The writer.
 * </param>
 * <param name="ulSessionID" type="IN">
 *   SessionID of the current session
 * </param>
 * <param name="usHeaderTag" type="IN">
 *   HeaderTag to identify the protocol handler.
 * </param>
 * <param name="ulServiceGroup" type="IN">
 *   ulServiceGroup = HIGHWORD: usCustomerId, LOWWORD: usServiceGroup;
 * </param>
 * <param name="usService" type="IN">
 *   Service
 * </param>
 */
RTS_RESULT CDECL BTagWriterStartService(BINTAGWRITER *pWriter, RTS_UI32 ulSessionID, RTS_UI16 usHeaderTag, RTS_UI32 ulServiceGroup, RTS_UI16 usService);
typedef RTS_RESULT (CDECL * PFBTAGWRITERSTARTSERVICE) (BINTAGWRITER *pWriter, RTS_UI32 ulSessionID, RTS_UI16 usHeaderTag, RTS_UI32 ulServiceGroup, RTS_UI16 usService);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERSTARTSERVICE_NOTIMPLEMENTED)
	#define USE_BTagWriterStartService
	#define EXT_BTagWriterStartService
	#define GET_BTagWriterStartService(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterStartService(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterStartService  FALSE
	#define EXP_BTagWriterStartService  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterStartService
	#define EXT_BTagWriterStartService
	#define GET_BTagWriterStartService(fl)  CAL_CMGETAPI( "BTagWriterStartService" ) 
	#define CAL_BTagWriterStartService  BTagWriterStartService
	#define CHK_BTagWriterStartService  TRUE
	#define EXP_BTagWriterStartService  CAL_CMEXPAPI( "BTagWriterStartService" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterStartService
	#define EXT_BTagWriterStartService
	#define GET_BTagWriterStartService(fl)  CAL_CMGETAPI( "BTagWriterStartService" ) 
	#define CAL_BTagWriterStartService  BTagWriterStartService
	#define CHK_BTagWriterStartService  TRUE
	#define EXP_BTagWriterStartService  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterStartService", (RTS_UINTPTR)BTagWriterStartService, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterStartService
	#define EXT_CmpBinTagUtilBTagWriterStartService
	#define GET_CmpBinTagUtilBTagWriterStartService  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterStartService pICmpBinTagUtil->IBTagWriterStartService
	#define CHK_CmpBinTagUtilBTagWriterStartService (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterStartService  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterStartService
	#define EXT_BTagWriterStartService
	#define GET_BTagWriterStartService(fl)  CAL_CMGETAPI( "BTagWriterStartService" ) 
	#define CAL_BTagWriterStartService pICmpBinTagUtil->IBTagWriterStartService
	#define CHK_BTagWriterStartService (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterStartService  CAL_CMEXPAPI( "BTagWriterStartService" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterStartService  PFBTAGWRITERSTARTSERVICE pfBTagWriterStartService;
	#define EXT_BTagWriterStartService  extern PFBTAGWRITERSTARTSERVICE pfBTagWriterStartService;
	#define GET_BTagWriterStartService(fl)  s_pfCMGetAPI2( "BTagWriterStartService", (RTS_VOID_FCTPTR *)&pfBTagWriterStartService, (fl), 0, 0)
	#define CAL_BTagWriterStartService  pfBTagWriterStartService
	#define CHK_BTagWriterStartService  (pfBTagWriterStartService != NULL)
	#define EXP_BTagWriterStartService  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterStartService", (RTS_UINTPTR)BTagWriterStartService, 0, 0) 
#endif




/**
 * <description>
 *   Finishes the service.
 *   It returns the buffer passed in to BTagWriterInit in ppBuffer and the number
 *   of bytes written to the buffer in pulSize;
 * </description>
 */
RTS_RESULT CDECL BTagWriterFinishService(BINTAGWRITER *pWriter, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize);
typedef RTS_RESULT (CDECL * PFBTAGWRITERFINISHSERVICE) (BINTAGWRITER *pWriter, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERFINISHSERVICE_NOTIMPLEMENTED)
	#define USE_BTagWriterFinishService
	#define EXT_BTagWriterFinishService
	#define GET_BTagWriterFinishService(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterFinishService(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterFinishService  FALSE
	#define EXP_BTagWriterFinishService  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterFinishService
	#define EXT_BTagWriterFinishService
	#define GET_BTagWriterFinishService(fl)  CAL_CMGETAPI( "BTagWriterFinishService" ) 
	#define CAL_BTagWriterFinishService  BTagWriterFinishService
	#define CHK_BTagWriterFinishService  TRUE
	#define EXP_BTagWriterFinishService  CAL_CMEXPAPI( "BTagWriterFinishService" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterFinishService
	#define EXT_BTagWriterFinishService
	#define GET_BTagWriterFinishService(fl)  CAL_CMGETAPI( "BTagWriterFinishService" ) 
	#define CAL_BTagWriterFinishService  BTagWriterFinishService
	#define CHK_BTagWriterFinishService  TRUE
	#define EXP_BTagWriterFinishService  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFinishService", (RTS_UINTPTR)BTagWriterFinishService, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterFinishService
	#define EXT_CmpBinTagUtilBTagWriterFinishService
	#define GET_CmpBinTagUtilBTagWriterFinishService  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterFinishService pICmpBinTagUtil->IBTagWriterFinishService
	#define CHK_CmpBinTagUtilBTagWriterFinishService (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterFinishService  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterFinishService
	#define EXT_BTagWriterFinishService
	#define GET_BTagWriterFinishService(fl)  CAL_CMGETAPI( "BTagWriterFinishService" ) 
	#define CAL_BTagWriterFinishService pICmpBinTagUtil->IBTagWriterFinishService
	#define CHK_BTagWriterFinishService (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterFinishService  CAL_CMEXPAPI( "BTagWriterFinishService" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterFinishService  PFBTAGWRITERFINISHSERVICE pfBTagWriterFinishService;
	#define EXT_BTagWriterFinishService  extern PFBTAGWRITERFINISHSERVICE pfBTagWriterFinishService;
	#define GET_BTagWriterFinishService(fl)  s_pfCMGetAPI2( "BTagWriterFinishService", (RTS_VOID_FCTPTR *)&pfBTagWriterFinishService, (fl), 0, 0)
	#define CAL_BTagWriterFinishService  pfBTagWriterFinishService
	#define CHK_BTagWriterFinishService  (pfBTagWriterFinishService != NULL)
	#define EXP_BTagWriterFinishService  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFinishService", (RTS_UINTPTR)BTagWriterFinishService, 0, 0) 
#endif




/**
 * <description>
 *   Start a new tag. A tag cannot be started within a data tag. Data tags are determined
 *   by the id of the tag: If bit 7 (that is the highest bit of the least significant byte of the tagid)
 *   is set then the tag contains only subtags, otherwise it contains no subtags but only content.
 *   Every call to BTagWriterStartTag must be matched with a call to BTagWriterEndTag.
 *   All subtags of a tag must be closed before the tag itself may be closed.
 * </description>
 * <param name="pWriter" type="INOUT">
 *   The writer must have been initialized with a call to BTagWriterInit.
 * </param>
 * <param name="ulTagId" type="IN">
 *   The id of the tag.
 * </param>
 * <param name="contentAlignment" type="IN">
 *   How the content should be aligned within this tag. If the tag is not a data tag 
 *   then contentAlignment must always be (4,0).
 *   In any case in this version of the data format the nModulus member must always be 4
 *   since all alignment is done in respect to 4 byte boundaries.
 * </param>
 * <param name="ulMinLengthSize" type="IN">
 *   
 * </param>
 */
RTS_RESULT CDECL BTagWriterStartTag(BINTAGWRITER *pWriter, RTS_UI32 ulTagId, BTAG_ALIGNMENT contentAlignment, RTS_UI32 ulMinLengthSize);
typedef RTS_RESULT (CDECL * PFBTAGWRITERSTARTTAG) (BINTAGWRITER *pWriter, RTS_UI32 ulTagId, BTAG_ALIGNMENT contentAlignment, RTS_UI32 ulMinLengthSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERSTARTTAG_NOTIMPLEMENTED)
	#define USE_BTagWriterStartTag
	#define EXT_BTagWriterStartTag
	#define GET_BTagWriterStartTag(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterStartTag(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterStartTag  FALSE
	#define EXP_BTagWriterStartTag  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterStartTag
	#define EXT_BTagWriterStartTag
	#define GET_BTagWriterStartTag(fl)  CAL_CMGETAPI( "BTagWriterStartTag" ) 
	#define CAL_BTagWriterStartTag  BTagWriterStartTag
	#define CHK_BTagWriterStartTag  TRUE
	#define EXP_BTagWriterStartTag  CAL_CMEXPAPI( "BTagWriterStartTag" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterStartTag
	#define EXT_BTagWriterStartTag
	#define GET_BTagWriterStartTag(fl)  CAL_CMGETAPI( "BTagWriterStartTag" ) 
	#define CAL_BTagWriterStartTag  BTagWriterStartTag
	#define CHK_BTagWriterStartTag  TRUE
	#define EXP_BTagWriterStartTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterStartTag", (RTS_UINTPTR)BTagWriterStartTag, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterStartTag
	#define EXT_CmpBinTagUtilBTagWriterStartTag
	#define GET_CmpBinTagUtilBTagWriterStartTag  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterStartTag pICmpBinTagUtil->IBTagWriterStartTag
	#define CHK_CmpBinTagUtilBTagWriterStartTag (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterStartTag  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterStartTag
	#define EXT_BTagWriterStartTag
	#define GET_BTagWriterStartTag(fl)  CAL_CMGETAPI( "BTagWriterStartTag" ) 
	#define CAL_BTagWriterStartTag pICmpBinTagUtil->IBTagWriterStartTag
	#define CHK_BTagWriterStartTag (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterStartTag  CAL_CMEXPAPI( "BTagWriterStartTag" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterStartTag  PFBTAGWRITERSTARTTAG pfBTagWriterStartTag;
	#define EXT_BTagWriterStartTag  extern PFBTAGWRITERSTARTTAG pfBTagWriterStartTag;
	#define GET_BTagWriterStartTag(fl)  s_pfCMGetAPI2( "BTagWriterStartTag", (RTS_VOID_FCTPTR *)&pfBTagWriterStartTag, (fl), 0, 0)
	#define CAL_BTagWriterStartTag  pfBTagWriterStartTag
	#define CHK_BTagWriterStartTag  (pfBTagWriterStartTag != NULL)
	#define EXP_BTagWriterStartTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterStartTag", (RTS_UINTPTR)BTagWriterStartTag, 0, 0) 
#endif




/**
 * <description>
 *   Append a C-string to the current tag. The string is written including
 *   the trailing end of string char (NUL). The current tag must be a data tag.
 * </description>
 * <param name="pWriter" type="IN">Pointer to bintag writer</param>
 * <param name="pszString" type="IN">Pointer to NUL terminated string to append</param>
 * <result>TRUE=succeeded, FALSE=failed</result>
 */
RTS_RESULT CDECL BTagWriterAppendString(BINTAGWRITER *pWriter, const char *pszString);
typedef RTS_RESULT (CDECL * PFBTAGWRITERAPPENDSTRING) (BINTAGWRITER *pWriter, const char *pszString);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERAPPENDSTRING_NOTIMPLEMENTED)
	#define USE_BTagWriterAppendString
	#define EXT_BTagWriterAppendString
	#define GET_BTagWriterAppendString(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterAppendString(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterAppendString  FALSE
	#define EXP_BTagWriterAppendString  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterAppendString
	#define EXT_BTagWriterAppendString
	#define GET_BTagWriterAppendString(fl)  CAL_CMGETAPI( "BTagWriterAppendString" ) 
	#define CAL_BTagWriterAppendString  BTagWriterAppendString
	#define CHK_BTagWriterAppendString  TRUE
	#define EXP_BTagWriterAppendString  CAL_CMEXPAPI( "BTagWriterAppendString" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterAppendString
	#define EXT_BTagWriterAppendString
	#define GET_BTagWriterAppendString(fl)  CAL_CMGETAPI( "BTagWriterAppendString" ) 
	#define CAL_BTagWriterAppendString  BTagWriterAppendString
	#define CHK_BTagWriterAppendString  TRUE
	#define EXP_BTagWriterAppendString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendString", (RTS_UINTPTR)BTagWriterAppendString, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterAppendString
	#define EXT_CmpBinTagUtilBTagWriterAppendString
	#define GET_CmpBinTagUtilBTagWriterAppendString  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterAppendString pICmpBinTagUtil->IBTagWriterAppendString
	#define CHK_CmpBinTagUtilBTagWriterAppendString (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterAppendString  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterAppendString
	#define EXT_BTagWriterAppendString
	#define GET_BTagWriterAppendString(fl)  CAL_CMGETAPI( "BTagWriterAppendString" ) 
	#define CAL_BTagWriterAppendString pICmpBinTagUtil->IBTagWriterAppendString
	#define CHK_BTagWriterAppendString (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterAppendString  CAL_CMEXPAPI( "BTagWriterAppendString" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterAppendString  PFBTAGWRITERAPPENDSTRING pfBTagWriterAppendString;
	#define EXT_BTagWriterAppendString  extern PFBTAGWRITERAPPENDSTRING pfBTagWriterAppendString;
	#define GET_BTagWriterAppendString(fl)  s_pfCMGetAPI2( "BTagWriterAppendString", (RTS_VOID_FCTPTR *)&pfBTagWriterAppendString, (fl), 0, 0)
	#define CAL_BTagWriterAppendString  pfBTagWriterAppendString
	#define CHK_BTagWriterAppendString  (pfBTagWriterAppendString != NULL)
	#define EXP_BTagWriterAppendString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendString", (RTS_UINTPTR)BTagWriterAppendString, 0, 0) 
#endif




/**
 * <description>
 *   Append a UTF-16 (widechar) string to the current tag. The string is written including
 *   the trailing end of string char (0x0000). The current tag must be a data tag.
 * </description>
 */
RTS_RESULT CDECL BTagWriterAppendWString(BINTAGWRITER *pWriter, const RTS_WCHAR *wszString);
typedef RTS_RESULT (CDECL * PFBTAGWRITERAPPENDWSTRING) (BINTAGWRITER *pWriter, const RTS_WCHAR *wszString);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERAPPENDWSTRING_NOTIMPLEMENTED)
	#define USE_BTagWriterAppendWString
	#define EXT_BTagWriterAppendWString
	#define GET_BTagWriterAppendWString(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterAppendWString(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterAppendWString  FALSE
	#define EXP_BTagWriterAppendWString  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterAppendWString
	#define EXT_BTagWriterAppendWString
	#define GET_BTagWriterAppendWString(fl)  CAL_CMGETAPI( "BTagWriterAppendWString" ) 
	#define CAL_BTagWriterAppendWString  BTagWriterAppendWString
	#define CHK_BTagWriterAppendWString  TRUE
	#define EXP_BTagWriterAppendWString  CAL_CMEXPAPI( "BTagWriterAppendWString" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterAppendWString
	#define EXT_BTagWriterAppendWString
	#define GET_BTagWriterAppendWString(fl)  CAL_CMGETAPI( "BTagWriterAppendWString" ) 
	#define CAL_BTagWriterAppendWString  BTagWriterAppendWString
	#define CHK_BTagWriterAppendWString  TRUE
	#define EXP_BTagWriterAppendWString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendWString", (RTS_UINTPTR)BTagWriterAppendWString, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterAppendWString
	#define EXT_CmpBinTagUtilBTagWriterAppendWString
	#define GET_CmpBinTagUtilBTagWriterAppendWString  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterAppendWString pICmpBinTagUtil->IBTagWriterAppendWString
	#define CHK_CmpBinTagUtilBTagWriterAppendWString (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterAppendWString  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterAppendWString
	#define EXT_BTagWriterAppendWString
	#define GET_BTagWriterAppendWString(fl)  CAL_CMGETAPI( "BTagWriterAppendWString" ) 
	#define CAL_BTagWriterAppendWString pICmpBinTagUtil->IBTagWriterAppendWString
	#define CHK_BTagWriterAppendWString (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterAppendWString  CAL_CMEXPAPI( "BTagWriterAppendWString" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterAppendWString  PFBTAGWRITERAPPENDWSTRING pfBTagWriterAppendWString;
	#define EXT_BTagWriterAppendWString  extern PFBTAGWRITERAPPENDWSTRING pfBTagWriterAppendWString;
	#define GET_BTagWriterAppendWString(fl)  s_pfCMGetAPI2( "BTagWriterAppendWString", (RTS_VOID_FCTPTR *)&pfBTagWriterAppendWString, (fl), 0, 0)
	#define CAL_BTagWriterAppendWString  pfBTagWriterAppendWString
	#define CHK_BTagWriterAppendWString  (pfBTagWriterAppendWString != NULL)
	#define EXP_BTagWriterAppendWString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendWString", (RTS_UINTPTR)BTagWriterAppendWString, 0, 0) 
#endif




/**
 * <description>
 *   Append ulSize bytes from the buffer pBlob to the current tag. 
 *   The current tag must be a data tag.
 * </description>
 */
RTS_RESULT CDECL BTagWriterAppendBlob(BINTAGWRITER *pWriter, const RTS_UI8 *pBlob, RTS_UI32 ulSize);
typedef RTS_RESULT (CDECL * PFBTAGWRITERAPPENDBLOB) (BINTAGWRITER *pWriter, const RTS_UI8 *pBlob, RTS_UI32 ulSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERAPPENDBLOB_NOTIMPLEMENTED)
	#define USE_BTagWriterAppendBlob
	#define EXT_BTagWriterAppendBlob
	#define GET_BTagWriterAppendBlob(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterAppendBlob(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterAppendBlob  FALSE
	#define EXP_BTagWriterAppendBlob  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterAppendBlob
	#define EXT_BTagWriterAppendBlob
	#define GET_BTagWriterAppendBlob(fl)  CAL_CMGETAPI( "BTagWriterAppendBlob" ) 
	#define CAL_BTagWriterAppendBlob  BTagWriterAppendBlob
	#define CHK_BTagWriterAppendBlob  TRUE
	#define EXP_BTagWriterAppendBlob  CAL_CMEXPAPI( "BTagWriterAppendBlob" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterAppendBlob
	#define EXT_BTagWriterAppendBlob
	#define GET_BTagWriterAppendBlob(fl)  CAL_CMGETAPI( "BTagWriterAppendBlob" ) 
	#define CAL_BTagWriterAppendBlob  BTagWriterAppendBlob
	#define CHK_BTagWriterAppendBlob  TRUE
	#define EXP_BTagWriterAppendBlob  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendBlob", (RTS_UINTPTR)BTagWriterAppendBlob, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterAppendBlob
	#define EXT_CmpBinTagUtilBTagWriterAppendBlob
	#define GET_CmpBinTagUtilBTagWriterAppendBlob  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterAppendBlob pICmpBinTagUtil->IBTagWriterAppendBlob
	#define CHK_CmpBinTagUtilBTagWriterAppendBlob (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterAppendBlob  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterAppendBlob
	#define EXT_BTagWriterAppendBlob
	#define GET_BTagWriterAppendBlob(fl)  CAL_CMGETAPI( "BTagWriterAppendBlob" ) 
	#define CAL_BTagWriterAppendBlob pICmpBinTagUtil->IBTagWriterAppendBlob
	#define CHK_BTagWriterAppendBlob (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterAppendBlob  CAL_CMEXPAPI( "BTagWriterAppendBlob" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterAppendBlob  PFBTAGWRITERAPPENDBLOB pfBTagWriterAppendBlob;
	#define EXT_BTagWriterAppendBlob  extern PFBTAGWRITERAPPENDBLOB pfBTagWriterAppendBlob;
	#define GET_BTagWriterAppendBlob(fl)  s_pfCMGetAPI2( "BTagWriterAppendBlob", (RTS_VOID_FCTPTR *)&pfBTagWriterAppendBlob, (fl), 0, 0)
	#define CAL_BTagWriterAppendBlob  pfBTagWriterAppendBlob
	#define CHK_BTagWriterAppendBlob  (pfBTagWriterAppendBlob != NULL)
	#define EXP_BTagWriterAppendBlob  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendBlob", (RTS_UINTPTR)BTagWriterAppendBlob, 0, 0) 
#endif




/**
 * <description>
 *  Returns a pointer to the current content position and forwards the write position for ulSize bytes.
 *   This function provides a buffer of length ulSize within the content of the current tag, in effect giving
 *   the caller random access to this buffer.
 *   This function is especially usefull, if that buffer has to be passed to another function, that fills it.
 *   (see example).
 *   ATTENTION: The returned pointer is valid only until the next operation on the bintagwriter is executed.
 *   After that the pointer must not be used any more! Do not store that pointer permanently.
 *   The current tag must be a data tag.
 * </description>
 * <param name="ppBuffer" type="OUT">Is set to point to the buffer, if the function succeeds</param>
 * <pre>
 *   // Store a number as hexadecimal string in a bintag writer
 *   unsigned char *pBuffer;
 *   unsigned char **ppBuffer;
 *   int nNumber = 1234;
 *	 *ppBuffer = pBuffer;
 *   if (ERR_OK == CAL_BTagWriterAppendRaw(pWriter, ppBuffer, 5)) // 4 digits + an additional byte for a trailing zero
 *     sprintf(pBuffer, "%04x", nNumber);
 *   pBuffer = NULL; // Avoid accidentally writing on that buffer again
 * </pre>
 */
RTS_RESULT CDECL BTagWriterAppendRaw(BINTAGWRITER *pWriter, RTS_UI8 **ppBuffer, RTS_UI32 ulSize);
typedef RTS_RESULT (CDECL * PFBTAGWRITERAPPENDRAW) (BINTAGWRITER *pWriter, RTS_UI8 **ppBuffer, RTS_UI32 ulSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERAPPENDRAW_NOTIMPLEMENTED)
	#define USE_BTagWriterAppendRaw
	#define EXT_BTagWriterAppendRaw
	#define GET_BTagWriterAppendRaw(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterAppendRaw(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterAppendRaw  FALSE
	#define EXP_BTagWriterAppendRaw  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterAppendRaw
	#define EXT_BTagWriterAppendRaw
	#define GET_BTagWriterAppendRaw(fl)  CAL_CMGETAPI( "BTagWriterAppendRaw" ) 
	#define CAL_BTagWriterAppendRaw  BTagWriterAppendRaw
	#define CHK_BTagWriterAppendRaw  TRUE
	#define EXP_BTagWriterAppendRaw  CAL_CMEXPAPI( "BTagWriterAppendRaw" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterAppendRaw
	#define EXT_BTagWriterAppendRaw
	#define GET_BTagWriterAppendRaw(fl)  CAL_CMGETAPI( "BTagWriterAppendRaw" ) 
	#define CAL_BTagWriterAppendRaw  BTagWriterAppendRaw
	#define CHK_BTagWriterAppendRaw  TRUE
	#define EXP_BTagWriterAppendRaw  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendRaw", (RTS_UINTPTR)BTagWriterAppendRaw, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterAppendRaw
	#define EXT_CmpBinTagUtilBTagWriterAppendRaw
	#define GET_CmpBinTagUtilBTagWriterAppendRaw  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterAppendRaw pICmpBinTagUtil->IBTagWriterAppendRaw
	#define CHK_CmpBinTagUtilBTagWriterAppendRaw (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterAppendRaw  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterAppendRaw
	#define EXT_BTagWriterAppendRaw
	#define GET_BTagWriterAppendRaw(fl)  CAL_CMGETAPI( "BTagWriterAppendRaw" ) 
	#define CAL_BTagWriterAppendRaw pICmpBinTagUtil->IBTagWriterAppendRaw
	#define CHK_BTagWriterAppendRaw (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterAppendRaw  CAL_CMEXPAPI( "BTagWriterAppendRaw" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterAppendRaw  PFBTAGWRITERAPPENDRAW pfBTagWriterAppendRaw;
	#define EXT_BTagWriterAppendRaw  extern PFBTAGWRITERAPPENDRAW pfBTagWriterAppendRaw;
	#define GET_BTagWriterAppendRaw(fl)  s_pfCMGetAPI2( "BTagWriterAppendRaw", (RTS_VOID_FCTPTR *)&pfBTagWriterAppendRaw, (fl), 0, 0)
	#define CAL_BTagWriterAppendRaw  pfBTagWriterAppendRaw
	#define CHK_BTagWriterAppendRaw  (pfBTagWriterAppendRaw != NULL)
	#define EXP_BTagWriterAppendRaw  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendRaw", (RTS_UINTPTR)BTagWriterAppendRaw, 0, 0) 
#endif




/**
 * <description>
 *   Add byFillByte to the content until the current content ends 
 *   on a position that satifies the desired alignment.
 *   Eg. a tag must always be closed on a (4,0)-alignment, so after adding a variable length
 *   string one should call:
 *   <pre>
 *     BTAG_ALIGNMENT align = {4,0};
 *     ...
 *     BTagWriterAppendFillBytes(pWriter, 0, align);
 *     BTagWriterEndTag(pWriter, TAG_ID);
 *   </pre>
 *   If the alignment property is already fullfilled nothing will be appended.
 * </description>
 * <param name="pWriter" type="IN"></param>
 * <param name="byFillByte" type="IN">
 *   The byte to be appended until alignment is achieved.
 * </param>
 * <param name="alignment" type="IN">
 *   The desired alignment of the next writer position.
 * </param>
 */
RTS_RESULT CDECL BTagWriterAppendFillBytes(BINTAGWRITER *pWriter, RTS_UI8 byFillByte, BTAG_ALIGNMENT alignment);
typedef RTS_RESULT (CDECL * PFBTAGWRITERAPPENDFILLBYTES) (BINTAGWRITER *pWriter, RTS_UI8 byFillByte, BTAG_ALIGNMENT alignment);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERAPPENDFILLBYTES_NOTIMPLEMENTED)
	#define USE_BTagWriterAppendFillBytes
	#define EXT_BTagWriterAppendFillBytes
	#define GET_BTagWriterAppendFillBytes(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterAppendFillBytes(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterAppendFillBytes  FALSE
	#define EXP_BTagWriterAppendFillBytes  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterAppendFillBytes
	#define EXT_BTagWriterAppendFillBytes
	#define GET_BTagWriterAppendFillBytes(fl)  CAL_CMGETAPI( "BTagWriterAppendFillBytes" ) 
	#define CAL_BTagWriterAppendFillBytes  BTagWriterAppendFillBytes
	#define CHK_BTagWriterAppendFillBytes  TRUE
	#define EXP_BTagWriterAppendFillBytes  CAL_CMEXPAPI( "BTagWriterAppendFillBytes" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterAppendFillBytes
	#define EXT_BTagWriterAppendFillBytes
	#define GET_BTagWriterAppendFillBytes(fl)  CAL_CMGETAPI( "BTagWriterAppendFillBytes" ) 
	#define CAL_BTagWriterAppendFillBytes  BTagWriterAppendFillBytes
	#define CHK_BTagWriterAppendFillBytes  TRUE
	#define EXP_BTagWriterAppendFillBytes  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendFillBytes", (RTS_UINTPTR)BTagWriterAppendFillBytes, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterAppendFillBytes
	#define EXT_CmpBinTagUtilBTagWriterAppendFillBytes
	#define GET_CmpBinTagUtilBTagWriterAppendFillBytes  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterAppendFillBytes pICmpBinTagUtil->IBTagWriterAppendFillBytes
	#define CHK_CmpBinTagUtilBTagWriterAppendFillBytes (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterAppendFillBytes  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterAppendFillBytes
	#define EXT_BTagWriterAppendFillBytes
	#define GET_BTagWriterAppendFillBytes(fl)  CAL_CMGETAPI( "BTagWriterAppendFillBytes" ) 
	#define CAL_BTagWriterAppendFillBytes pICmpBinTagUtil->IBTagWriterAppendFillBytes
	#define CHK_BTagWriterAppendFillBytes (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterAppendFillBytes  CAL_CMEXPAPI( "BTagWriterAppendFillBytes" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterAppendFillBytes  PFBTAGWRITERAPPENDFILLBYTES pfBTagWriterAppendFillBytes;
	#define EXT_BTagWriterAppendFillBytes  extern PFBTAGWRITERAPPENDFILLBYTES pfBTagWriterAppendFillBytes;
	#define GET_BTagWriterAppendFillBytes(fl)  s_pfCMGetAPI2( "BTagWriterAppendFillBytes", (RTS_VOID_FCTPTR *)&pfBTagWriterAppendFillBytes, (fl), 0, 0)
	#define CAL_BTagWriterAppendFillBytes  pfBTagWriterAppendFillBytes
	#define CHK_BTagWriterAppendFillBytes  (pfBTagWriterAppendFillBytes != NULL)
	#define EXP_BTagWriterAppendFillBytes  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendFillBytes", (RTS_UINTPTR)BTagWriterAppendFillBytes, 0, 0) 
#endif




/**
 * <description>
 *   Append byFillByte ulSize times.
 * </description>
 */
RTS_RESULT CDECL BTagWriterAppendDummyBytes(BINTAGWRITER *pWriter, RTS_UI8 byFillByte, RTS_UI32 ulSize);
typedef RTS_RESULT (CDECL * PFBTAGWRITERAPPENDDUMMYBYTES) (BINTAGWRITER *pWriter, RTS_UI8 byFillByte, RTS_UI32 ulSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERAPPENDDUMMYBYTES_NOTIMPLEMENTED)
	#define USE_BTagWriterAppendDummyBytes
	#define EXT_BTagWriterAppendDummyBytes
	#define GET_BTagWriterAppendDummyBytes(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterAppendDummyBytes(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterAppendDummyBytes  FALSE
	#define EXP_BTagWriterAppendDummyBytes  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterAppendDummyBytes
	#define EXT_BTagWriterAppendDummyBytes
	#define GET_BTagWriterAppendDummyBytes(fl)  CAL_CMGETAPI( "BTagWriterAppendDummyBytes" ) 
	#define CAL_BTagWriterAppendDummyBytes  BTagWriterAppendDummyBytes
	#define CHK_BTagWriterAppendDummyBytes  TRUE
	#define EXP_BTagWriterAppendDummyBytes  CAL_CMEXPAPI( "BTagWriterAppendDummyBytes" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterAppendDummyBytes
	#define EXT_BTagWriterAppendDummyBytes
	#define GET_BTagWriterAppendDummyBytes(fl)  CAL_CMGETAPI( "BTagWriterAppendDummyBytes" ) 
	#define CAL_BTagWriterAppendDummyBytes  BTagWriterAppendDummyBytes
	#define CHK_BTagWriterAppendDummyBytes  TRUE
	#define EXP_BTagWriterAppendDummyBytes  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendDummyBytes", (RTS_UINTPTR)BTagWriterAppendDummyBytes, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterAppendDummyBytes
	#define EXT_CmpBinTagUtilBTagWriterAppendDummyBytes
	#define GET_CmpBinTagUtilBTagWriterAppendDummyBytes  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterAppendDummyBytes pICmpBinTagUtil->IBTagWriterAppendDummyBytes
	#define CHK_CmpBinTagUtilBTagWriterAppendDummyBytes (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterAppendDummyBytes  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterAppendDummyBytes
	#define EXT_BTagWriterAppendDummyBytes
	#define GET_BTagWriterAppendDummyBytes(fl)  CAL_CMGETAPI( "BTagWriterAppendDummyBytes" ) 
	#define CAL_BTagWriterAppendDummyBytes pICmpBinTagUtil->IBTagWriterAppendDummyBytes
	#define CHK_BTagWriterAppendDummyBytes (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterAppendDummyBytes  CAL_CMEXPAPI( "BTagWriterAppendDummyBytes" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterAppendDummyBytes  PFBTAGWRITERAPPENDDUMMYBYTES pfBTagWriterAppendDummyBytes;
	#define EXT_BTagWriterAppendDummyBytes  extern PFBTAGWRITERAPPENDDUMMYBYTES pfBTagWriterAppendDummyBytes;
	#define GET_BTagWriterAppendDummyBytes(fl)  s_pfCMGetAPI2( "BTagWriterAppendDummyBytes", (RTS_VOID_FCTPTR *)&pfBTagWriterAppendDummyBytes, (fl), 0, 0)
	#define CAL_BTagWriterAppendDummyBytes  pfBTagWriterAppendDummyBytes
	#define CHK_BTagWriterAppendDummyBytes  (pfBTagWriterAppendDummyBytes != NULL)
	#define EXP_BTagWriterAppendDummyBytes  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterAppendDummyBytes", (RTS_UINTPTR)BTagWriterAppendDummyBytes, 0, 0) 
#endif




/**
 * <description>
 *   Close the current tag. The current tag must have been started with the given tag id.
 *   ulTagId is used only as an additional check that all tags are closed in the right order.
 *   Since the only element that follows a tag must be a tag again and a tag must always start
 *   on a 4 byte boundary the complete size of a tag (header + content) must be dividable by 4.
 *   Taking a contentalignment of (4,x) then "contentsize == 4 - x (MOD 4) " must hold.
 *   (eg. for (4,1): contentsize % 4 == 3)
 *   If this condition does not hold, the function will fail.
 * </description>
 */
RTS_RESULT CDECL BTagWriterEndTag(BINTAGWRITER *pWriter, RTS_UI32 ulTagId);
typedef RTS_RESULT (CDECL * PFBTAGWRITERENDTAG) (BINTAGWRITER *pWriter, RTS_UI32 ulTagId);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERENDTAG_NOTIMPLEMENTED)
	#define USE_BTagWriterEndTag
	#define EXT_BTagWriterEndTag
	#define GET_BTagWriterEndTag(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterEndTag(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterEndTag  FALSE
	#define EXP_BTagWriterEndTag  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterEndTag
	#define EXT_BTagWriterEndTag
	#define GET_BTagWriterEndTag(fl)  CAL_CMGETAPI( "BTagWriterEndTag" ) 
	#define CAL_BTagWriterEndTag  BTagWriterEndTag
	#define CHK_BTagWriterEndTag  TRUE
	#define EXP_BTagWriterEndTag  CAL_CMEXPAPI( "BTagWriterEndTag" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterEndTag
	#define EXT_BTagWriterEndTag
	#define GET_BTagWriterEndTag(fl)  CAL_CMGETAPI( "BTagWriterEndTag" ) 
	#define CAL_BTagWriterEndTag  BTagWriterEndTag
	#define CHK_BTagWriterEndTag  TRUE
	#define EXP_BTagWriterEndTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterEndTag", (RTS_UINTPTR)BTagWriterEndTag, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterEndTag
	#define EXT_CmpBinTagUtilBTagWriterEndTag
	#define GET_CmpBinTagUtilBTagWriterEndTag  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterEndTag pICmpBinTagUtil->IBTagWriterEndTag
	#define CHK_CmpBinTagUtilBTagWriterEndTag (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterEndTag  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterEndTag
	#define EXT_BTagWriterEndTag
	#define GET_BTagWriterEndTag(fl)  CAL_CMGETAPI( "BTagWriterEndTag" ) 
	#define CAL_BTagWriterEndTag pICmpBinTagUtil->IBTagWriterEndTag
	#define CHK_BTagWriterEndTag (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterEndTag  CAL_CMEXPAPI( "BTagWriterEndTag" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterEndTag  PFBTAGWRITERENDTAG pfBTagWriterEndTag;
	#define EXT_BTagWriterEndTag  extern PFBTAGWRITERENDTAG pfBTagWriterEndTag;
	#define GET_BTagWriterEndTag(fl)  s_pfCMGetAPI2( "BTagWriterEndTag", (RTS_VOID_FCTPTR *)&pfBTagWriterEndTag, (fl), 0, 0)
	#define CAL_BTagWriterEndTag  pfBTagWriterEndTag
	#define CHK_BTagWriterEndTag  (pfBTagWriterEndTag != NULL)
	#define EXP_BTagWriterEndTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterEndTag", (RTS_UINTPTR)BTagWriterEndTag, 0, 0) 
#endif




/**
 * <description>
 *   Replaces the current writer buffer with a new one. All content in the current buffer
 *   is copied to the new buffer. This also means that the size of the new buffer has to be
 *   at least the current position of the writer. 
 *   The purpose of this function is to allow for extension of the buffer if an operation
 *   returned a buffer overflow. Then the caller may allocate a new buffer, call 
 *   BTagWriterSwitchBuffer and retry the failed operation.
 * </description>
 * <param name="pWriter" type="IN"></param>
 * <param name="pNewBuffer" type="IN">
 *   The buffer that is to replace the original buffer.
 * </param>
 * <param name="ulNewSize" type="IN">
 *   Size of the new buffer. Must be greater or at least equal to the current postion
 *   of the writer.
 * </param>
 * <param name="ppOldBuffer" type="OUT">
 *   Is set to the previous buffer.
 * </param>
 */
RTS_RESULT CDECL BTagWriterSwitchBuffer(BINTAGWRITER *pWriter, RTS_UI8 *pNewBuffer, RTS_UI32 ulNewSize, RTS_UI8 **ppOldBuffer);
typedef RTS_RESULT (CDECL * PFBTAGWRITERSWITCHBUFFER) (BINTAGWRITER *pWriter, RTS_UI8 *pNewBuffer, RTS_UI32 ulNewSize, RTS_UI8 **ppOldBuffer);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERSWITCHBUFFER_NOTIMPLEMENTED)
	#define USE_BTagWriterSwitchBuffer
	#define EXT_BTagWriterSwitchBuffer
	#define GET_BTagWriterSwitchBuffer(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterSwitchBuffer(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterSwitchBuffer  FALSE
	#define EXP_BTagWriterSwitchBuffer  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterSwitchBuffer
	#define EXT_BTagWriterSwitchBuffer
	#define GET_BTagWriterSwitchBuffer(fl)  CAL_CMGETAPI( "BTagWriterSwitchBuffer" ) 
	#define CAL_BTagWriterSwitchBuffer  BTagWriterSwitchBuffer
	#define CHK_BTagWriterSwitchBuffer  TRUE
	#define EXP_BTagWriterSwitchBuffer  CAL_CMEXPAPI( "BTagWriterSwitchBuffer" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterSwitchBuffer
	#define EXT_BTagWriterSwitchBuffer
	#define GET_BTagWriterSwitchBuffer(fl)  CAL_CMGETAPI( "BTagWriterSwitchBuffer" ) 
	#define CAL_BTagWriterSwitchBuffer  BTagWriterSwitchBuffer
	#define CHK_BTagWriterSwitchBuffer  TRUE
	#define EXP_BTagWriterSwitchBuffer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterSwitchBuffer", (RTS_UINTPTR)BTagWriterSwitchBuffer, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterSwitchBuffer
	#define EXT_CmpBinTagUtilBTagWriterSwitchBuffer
	#define GET_CmpBinTagUtilBTagWriterSwitchBuffer  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterSwitchBuffer pICmpBinTagUtil->IBTagWriterSwitchBuffer
	#define CHK_CmpBinTagUtilBTagWriterSwitchBuffer (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterSwitchBuffer  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterSwitchBuffer
	#define EXT_BTagWriterSwitchBuffer
	#define GET_BTagWriterSwitchBuffer(fl)  CAL_CMGETAPI( "BTagWriterSwitchBuffer" ) 
	#define CAL_BTagWriterSwitchBuffer pICmpBinTagUtil->IBTagWriterSwitchBuffer
	#define CHK_BTagWriterSwitchBuffer (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterSwitchBuffer  CAL_CMEXPAPI( "BTagWriterSwitchBuffer" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterSwitchBuffer  PFBTAGWRITERSWITCHBUFFER pfBTagWriterSwitchBuffer;
	#define EXT_BTagWriterSwitchBuffer  extern PFBTAGWRITERSWITCHBUFFER pfBTagWriterSwitchBuffer;
	#define GET_BTagWriterSwitchBuffer(fl)  s_pfCMGetAPI2( "BTagWriterSwitchBuffer", (RTS_VOID_FCTPTR *)&pfBTagWriterSwitchBuffer, (fl), 0, 0)
	#define CAL_BTagWriterSwitchBuffer  pfBTagWriterSwitchBuffer
	#define CHK_BTagWriterSwitchBuffer  (pfBTagWriterSwitchBuffer != NULL)
	#define EXP_BTagWriterSwitchBuffer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterSwitchBuffer", (RTS_UINTPTR)BTagWriterSwitchBuffer, 0, 0) 
#endif




/**
 * <description>
 *   Save the current state of the writer. A later call to BTagWriterRestoreSavepoint will
 *   reset the writer and its buffer to that state. Any number of savepoints may be created.
 *   The caller must use multiple savepoints in a stack-like fashion: If savepoints are
 *   created in the order 1,2,3,4 then they must be restored in opposite order only. Not 
 *   every savepoint must be restored - but if one is restored all savepoints created after
 *   that one MUST NOT BE USED any more.
 *   Examples: ("sX"="create savepoint X", "rX"="Restore savepoint X", 
 *			    "(x,y,z)"="stack of valid savepoints")
 *   The following sequences are valid:
 *   () s1 (1) s2 (1,2) s3 (1,2,3) r2 (1) r1 ()
 *   () s1 (1) s2 (1,2) s3 (1,2,3) r2 (1) s4 (1,4) r4 (1)  
 *   whereas this one is invalid:
 *   () s1 (1) s2 (1,2) s3 (1,2,3) r2 (1) r3 [3 is not valid at this point]
 * 
 *   The writer does not track the stack of savepoints, therefore it cannot
 *   detect errors in the restore order. It's the responsibility of the application
 *   to ensure the correct restore order. Failure in doing so will lead to an 
 *   inconsistent state and may well corrupt the whole document.
 *   Besides the restore order savepoints are independent of each other. The application
 *   may delete/reuse a savepoint anytime it doesn't need it anymore.
 * 
 *   Savepoints cannot be used to transfer the state of a writer to a second writer.
 *   Any attempt in doing so will lead to undefined behaviour.
 * </description>
 * <param name="pWriter" type="IN"></param>
 * <param name="pSavePoint" type="OUT">
 *   Will receive all information needed to restore the current state of the writer.
 * </param>
 */ 
RTS_RESULT CDECL BTagWriterCreateSavepoint(BINTAGWRITER *pWriter, BINTAGSAVEPOINT *pSavepoint);
typedef RTS_RESULT (CDECL * PFBTAGWRITERCREATESAVEPOINT) (BINTAGWRITER *pWriter, BINTAGSAVEPOINT *pSavepoint);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERCREATESAVEPOINT_NOTIMPLEMENTED)
	#define USE_BTagWriterCreateSavepoint
	#define EXT_BTagWriterCreateSavepoint
	#define GET_BTagWriterCreateSavepoint(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterCreateSavepoint(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterCreateSavepoint  FALSE
	#define EXP_BTagWriterCreateSavepoint  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterCreateSavepoint
	#define EXT_BTagWriterCreateSavepoint
	#define GET_BTagWriterCreateSavepoint(fl)  CAL_CMGETAPI( "BTagWriterCreateSavepoint" ) 
	#define CAL_BTagWriterCreateSavepoint  BTagWriterCreateSavepoint
	#define CHK_BTagWriterCreateSavepoint  TRUE
	#define EXP_BTagWriterCreateSavepoint  CAL_CMEXPAPI( "BTagWriterCreateSavepoint" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterCreateSavepoint
	#define EXT_BTagWriterCreateSavepoint
	#define GET_BTagWriterCreateSavepoint(fl)  CAL_CMGETAPI( "BTagWriterCreateSavepoint" ) 
	#define CAL_BTagWriterCreateSavepoint  BTagWriterCreateSavepoint
	#define CHK_BTagWriterCreateSavepoint  TRUE
	#define EXP_BTagWriterCreateSavepoint  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterCreateSavepoint", (RTS_UINTPTR)BTagWriterCreateSavepoint, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterCreateSavepoint
	#define EXT_CmpBinTagUtilBTagWriterCreateSavepoint
	#define GET_CmpBinTagUtilBTagWriterCreateSavepoint  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterCreateSavepoint pICmpBinTagUtil->IBTagWriterCreateSavepoint
	#define CHK_CmpBinTagUtilBTagWriterCreateSavepoint (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterCreateSavepoint  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterCreateSavepoint
	#define EXT_BTagWriterCreateSavepoint
	#define GET_BTagWriterCreateSavepoint(fl)  CAL_CMGETAPI( "BTagWriterCreateSavepoint" ) 
	#define CAL_BTagWriterCreateSavepoint pICmpBinTagUtil->IBTagWriterCreateSavepoint
	#define CHK_BTagWriterCreateSavepoint (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterCreateSavepoint  CAL_CMEXPAPI( "BTagWriterCreateSavepoint" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterCreateSavepoint  PFBTAGWRITERCREATESAVEPOINT pfBTagWriterCreateSavepoint;
	#define EXT_BTagWriterCreateSavepoint  extern PFBTAGWRITERCREATESAVEPOINT pfBTagWriterCreateSavepoint;
	#define GET_BTagWriterCreateSavepoint(fl)  s_pfCMGetAPI2( "BTagWriterCreateSavepoint", (RTS_VOID_FCTPTR *)&pfBTagWriterCreateSavepoint, (fl), 0, 0)
	#define CAL_BTagWriterCreateSavepoint  pfBTagWriterCreateSavepoint
	#define CHK_BTagWriterCreateSavepoint  (pfBTagWriterCreateSavepoint != NULL)
	#define EXP_BTagWriterCreateSavepoint  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterCreateSavepoint", (RTS_UINTPTR)BTagWriterCreateSavepoint, 0, 0) 
#endif




/**
 * <description>
 *   Restore the state of the writer to one previously saved using 
 *   BTagWriterCreateSavepoint. See there for a detailed description on how to use
 *   these functions.
 * </description>
 * <param name="pWriter" type="IN">
 *   Must be the same writer that the savepoint was created on.
 * </param>
 * <param name="pSavepoint" type="IN">
 *   A previously created savepoint.
 * </param>
 */
RTS_RESULT CDECL BTagWriterRestoreSavepoint(BINTAGWRITER *pWriter, BINTAGSAVEPOINT *pSavepoint);
typedef RTS_RESULT (CDECL * PFBTAGWRITERRESTORESAVEPOINT) (BINTAGWRITER *pWriter, BINTAGSAVEPOINT *pSavepoint);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERRESTORESAVEPOINT_NOTIMPLEMENTED)
	#define USE_BTagWriterRestoreSavepoint
	#define EXT_BTagWriterRestoreSavepoint
	#define GET_BTagWriterRestoreSavepoint(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterRestoreSavepoint(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterRestoreSavepoint  FALSE
	#define EXP_BTagWriterRestoreSavepoint  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterRestoreSavepoint
	#define EXT_BTagWriterRestoreSavepoint
	#define GET_BTagWriterRestoreSavepoint(fl)  CAL_CMGETAPI( "BTagWriterRestoreSavepoint" ) 
	#define CAL_BTagWriterRestoreSavepoint  BTagWriterRestoreSavepoint
	#define CHK_BTagWriterRestoreSavepoint  TRUE
	#define EXP_BTagWriterRestoreSavepoint  CAL_CMEXPAPI( "BTagWriterRestoreSavepoint" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterRestoreSavepoint
	#define EXT_BTagWriterRestoreSavepoint
	#define GET_BTagWriterRestoreSavepoint(fl)  CAL_CMGETAPI( "BTagWriterRestoreSavepoint" ) 
	#define CAL_BTagWriterRestoreSavepoint  BTagWriterRestoreSavepoint
	#define CHK_BTagWriterRestoreSavepoint  TRUE
	#define EXP_BTagWriterRestoreSavepoint  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterRestoreSavepoint", (RTS_UINTPTR)BTagWriterRestoreSavepoint, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterRestoreSavepoint
	#define EXT_CmpBinTagUtilBTagWriterRestoreSavepoint
	#define GET_CmpBinTagUtilBTagWriterRestoreSavepoint  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterRestoreSavepoint pICmpBinTagUtil->IBTagWriterRestoreSavepoint
	#define CHK_CmpBinTagUtilBTagWriterRestoreSavepoint (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterRestoreSavepoint  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterRestoreSavepoint
	#define EXT_BTagWriterRestoreSavepoint
	#define GET_BTagWriterRestoreSavepoint(fl)  CAL_CMGETAPI( "BTagWriterRestoreSavepoint" ) 
	#define CAL_BTagWriterRestoreSavepoint pICmpBinTagUtil->IBTagWriterRestoreSavepoint
	#define CHK_BTagWriterRestoreSavepoint (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterRestoreSavepoint  CAL_CMEXPAPI( "BTagWriterRestoreSavepoint" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterRestoreSavepoint  PFBTAGWRITERRESTORESAVEPOINT pfBTagWriterRestoreSavepoint;
	#define EXT_BTagWriterRestoreSavepoint  extern PFBTAGWRITERRESTORESAVEPOINT pfBTagWriterRestoreSavepoint;
	#define GET_BTagWriterRestoreSavepoint(fl)  s_pfCMGetAPI2( "BTagWriterRestoreSavepoint", (RTS_VOID_FCTPTR *)&pfBTagWriterRestoreSavepoint, (fl), 0, 0)
	#define CAL_BTagWriterRestoreSavepoint  pfBTagWriterRestoreSavepoint
	#define CHK_BTagWriterRestoreSavepoint  (pfBTagWriterRestoreSavepoint != NULL)
	#define EXP_BTagWriterRestoreSavepoint  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterRestoreSavepoint", (RTS_UINTPTR)BTagWriterRestoreSavepoint, 0, 0) 
#endif




/**
 * <description>
 *   Reads the available bytes in writer buffer that can be used for data.
 * </description>
 */
RTS_UI32 CDECL BTagWriterGetAvailableBuffer(BINTAGWRITER *pWriter);
typedef RTS_UI32 (CDECL * PFBTAGWRITERGETAVAILABLEBUFFER) (BINTAGWRITER *pWriter);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERGETAVAILABLEBUFFER_NOTIMPLEMENTED)
	#define USE_BTagWriterGetAvailableBuffer
	#define EXT_BTagWriterGetAvailableBuffer
	#define GET_BTagWriterGetAvailableBuffer(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterGetAvailableBuffer(p0)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterGetAvailableBuffer  FALSE
	#define EXP_BTagWriterGetAvailableBuffer  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterGetAvailableBuffer
	#define EXT_BTagWriterGetAvailableBuffer
	#define GET_BTagWriterGetAvailableBuffer(fl)  CAL_CMGETAPI( "BTagWriterGetAvailableBuffer" ) 
	#define CAL_BTagWriterGetAvailableBuffer  BTagWriterGetAvailableBuffer
	#define CHK_BTagWriterGetAvailableBuffer  TRUE
	#define EXP_BTagWriterGetAvailableBuffer  CAL_CMEXPAPI( "BTagWriterGetAvailableBuffer" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterGetAvailableBuffer
	#define EXT_BTagWriterGetAvailableBuffer
	#define GET_BTagWriterGetAvailableBuffer(fl)  CAL_CMGETAPI( "BTagWriterGetAvailableBuffer" ) 
	#define CAL_BTagWriterGetAvailableBuffer  BTagWriterGetAvailableBuffer
	#define CHK_BTagWriterGetAvailableBuffer  TRUE
	#define EXP_BTagWriterGetAvailableBuffer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterGetAvailableBuffer", (RTS_UINTPTR)BTagWriterGetAvailableBuffer, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterGetAvailableBuffer
	#define EXT_CmpBinTagUtilBTagWriterGetAvailableBuffer
	#define GET_CmpBinTagUtilBTagWriterGetAvailableBuffer  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterGetAvailableBuffer pICmpBinTagUtil->IBTagWriterGetAvailableBuffer
	#define CHK_CmpBinTagUtilBTagWriterGetAvailableBuffer (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterGetAvailableBuffer  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterGetAvailableBuffer
	#define EXT_BTagWriterGetAvailableBuffer
	#define GET_BTagWriterGetAvailableBuffer(fl)  CAL_CMGETAPI( "BTagWriterGetAvailableBuffer" ) 
	#define CAL_BTagWriterGetAvailableBuffer pICmpBinTagUtil->IBTagWriterGetAvailableBuffer
	#define CHK_BTagWriterGetAvailableBuffer (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterGetAvailableBuffer  CAL_CMEXPAPI( "BTagWriterGetAvailableBuffer" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterGetAvailableBuffer  PFBTAGWRITERGETAVAILABLEBUFFER pfBTagWriterGetAvailableBuffer;
	#define EXT_BTagWriterGetAvailableBuffer  extern PFBTAGWRITERGETAVAILABLEBUFFER pfBTagWriterGetAvailableBuffer;
	#define GET_BTagWriterGetAvailableBuffer(fl)  s_pfCMGetAPI2( "BTagWriterGetAvailableBuffer", (RTS_VOID_FCTPTR *)&pfBTagWriterGetAvailableBuffer, (fl), 0, 0)
	#define CAL_BTagWriterGetAvailableBuffer  pfBTagWriterGetAvailableBuffer
	#define CHK_BTagWriterGetAvailableBuffer  (pfBTagWriterGetAvailableBuffer != NULL)
	#define EXP_BTagWriterGetAvailableBuffer  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterGetAvailableBuffer", (RTS_UINTPTR)BTagWriterGetAvailableBuffer, 0, 0) 
#endif




/**
 * <description>
 *   Finishes the writer. If not all tags have been closed properly this function will fail.
 *   Otherwise it returns the buffer passed in to BTagWriterInit in ppBuffer and the number
 *   of bytes written to the buffer in pulSize;
 * </description>
 */
RTS_RESULT CDECL BTagWriterFinish(BINTAGWRITER *pWriter, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize);
typedef RTS_RESULT (CDECL * PFBTAGWRITERFINISH) (BINTAGWRITER *pWriter, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERFINISH_NOTIMPLEMENTED)
	#define USE_BTagWriterFinish
	#define EXT_BTagWriterFinish
	#define GET_BTagWriterFinish(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterFinish(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterFinish  FALSE
	#define EXP_BTagWriterFinish  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterFinish
	#define EXT_BTagWriterFinish
	#define GET_BTagWriterFinish(fl)  CAL_CMGETAPI( "BTagWriterFinish" ) 
	#define CAL_BTagWriterFinish  BTagWriterFinish
	#define CHK_BTagWriterFinish  TRUE
	#define EXP_BTagWriterFinish  CAL_CMEXPAPI( "BTagWriterFinish" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterFinish
	#define EXT_BTagWriterFinish
	#define GET_BTagWriterFinish(fl)  CAL_CMGETAPI( "BTagWriterFinish" ) 
	#define CAL_BTagWriterFinish  BTagWriterFinish
	#define CHK_BTagWriterFinish  TRUE
	#define EXP_BTagWriterFinish  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFinish", (RTS_UINTPTR)BTagWriterFinish, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterFinish
	#define EXT_CmpBinTagUtilBTagWriterFinish
	#define GET_CmpBinTagUtilBTagWriterFinish  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterFinish pICmpBinTagUtil->IBTagWriterFinish
	#define CHK_CmpBinTagUtilBTagWriterFinish (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterFinish  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterFinish
	#define EXT_BTagWriterFinish
	#define GET_BTagWriterFinish(fl)  CAL_CMGETAPI( "BTagWriterFinish" ) 
	#define CAL_BTagWriterFinish pICmpBinTagUtil->IBTagWriterFinish
	#define CHK_BTagWriterFinish (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterFinish  CAL_CMEXPAPI( "BTagWriterFinish" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterFinish  PFBTAGWRITERFINISH pfBTagWriterFinish;
	#define EXT_BTagWriterFinish  extern PFBTAGWRITERFINISH pfBTagWriterFinish;
	#define GET_BTagWriterFinish(fl)  s_pfCMGetAPI2( "BTagWriterFinish", (RTS_VOID_FCTPTR *)&pfBTagWriterFinish, (fl), 0, 0)
	#define CAL_BTagWriterFinish  pfBTagWriterFinish
	#define CHK_BTagWriterFinish  (pfBTagWriterFinish != NULL)
	#define EXP_BTagWriterFinish  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFinish", (RTS_UINTPTR)BTagWriterFinish, 0, 0) 
#endif




/**
 * <description>Write a single tag</description>
 * <param name="pWriter" type="IN">Pointer to the writer stream</param>
 * <param name="ulTag" type="IN">Tag to write</param>
 * <param name="align" type="IN">Alignment of the tag</param>
 * <param name="pContent" type="IN">Pointer to the tag data</param>
 * <param name="ulSize" type="IN">Size in bytes of the tag data</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL BTagWriteSingleTag(BINTAGWRITER *pWriter, RTS_UI32 ulTag, BTAG_ALIGNMENT align, RTS_I32 bFillBytes, void *pContent, RTS_UI32 ulSize);
typedef RTS_RESULT (CDECL * PFBTAGWRITESINGLETAG) (BINTAGWRITER *pWriter, RTS_UI32 ulTag, BTAG_ALIGNMENT align, RTS_I32 bFillBytes, void *pContent, RTS_UI32 ulSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITESINGLETAG_NOTIMPLEMENTED)
	#define USE_BTagWriteSingleTag
	#define EXT_BTagWriteSingleTag
	#define GET_BTagWriteSingleTag(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriteSingleTag(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriteSingleTag  FALSE
	#define EXP_BTagWriteSingleTag  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriteSingleTag
	#define EXT_BTagWriteSingleTag
	#define GET_BTagWriteSingleTag(fl)  CAL_CMGETAPI( "BTagWriteSingleTag" ) 
	#define CAL_BTagWriteSingleTag  BTagWriteSingleTag
	#define CHK_BTagWriteSingleTag  TRUE
	#define EXP_BTagWriteSingleTag  CAL_CMEXPAPI( "BTagWriteSingleTag" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriteSingleTag
	#define EXT_BTagWriteSingleTag
	#define GET_BTagWriteSingleTag(fl)  CAL_CMGETAPI( "BTagWriteSingleTag" ) 
	#define CAL_BTagWriteSingleTag  BTagWriteSingleTag
	#define CHK_BTagWriteSingleTag  TRUE
	#define EXP_BTagWriteSingleTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriteSingleTag", (RTS_UINTPTR)BTagWriteSingleTag, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriteSingleTag
	#define EXT_CmpBinTagUtilBTagWriteSingleTag
	#define GET_CmpBinTagUtilBTagWriteSingleTag  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriteSingleTag pICmpBinTagUtil->IBTagWriteSingleTag
	#define CHK_CmpBinTagUtilBTagWriteSingleTag (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriteSingleTag  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriteSingleTag
	#define EXT_BTagWriteSingleTag
	#define GET_BTagWriteSingleTag(fl)  CAL_CMGETAPI( "BTagWriteSingleTag" ) 
	#define CAL_BTagWriteSingleTag pICmpBinTagUtil->IBTagWriteSingleTag
	#define CHK_BTagWriteSingleTag (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriteSingleTag  CAL_CMEXPAPI( "BTagWriteSingleTag" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriteSingleTag  PFBTAGWRITESINGLETAG pfBTagWriteSingleTag;
	#define EXT_BTagWriteSingleTag  extern PFBTAGWRITESINGLETAG pfBTagWriteSingleTag;
	#define GET_BTagWriteSingleTag(fl)  s_pfCMGetAPI2( "BTagWriteSingleTag", (RTS_VOID_FCTPTR *)&pfBTagWriteSingleTag, (fl), 0, 0)
	#define CAL_BTagWriteSingleTag  pfBTagWriteSingleTag
	#define CHK_BTagWriteSingleTag  (pfBTagWriteSingleTag != NULL)
	#define EXP_BTagWriteSingleTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriteSingleTag", (RTS_UINTPTR)BTagWriteSingleTag, 0, 0) 
#endif




/**
 * <description>Write a single tag with more than one content</description>
 * <param name="pWriter" type="IN">Pointer to the writer stream</param>
 * <param name="ulTag" type="IN">Tag to write</param>
 * <param name="align" type="IN">Alignment of the tag</param>
 * <param name="pContent" type="IN">Pointer to the tag data</param>
 * <param name="ulSize" type="IN">Size in bytes of the tag data</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL BTagWriteSingleTag2(BINTAGWRITER *pWriter, RTS_UI32 ulTag, BTAG_ALIGNMENT align, void **ppContentList, RTS_UI32 *paulSize);
typedef RTS_RESULT (CDECL * PFBTAGWRITESINGLETAG2) (BINTAGWRITER *pWriter, RTS_UI32 ulTag, BTAG_ALIGNMENT align, void **ppContentList, RTS_UI32 *paulSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITESINGLETAG2_NOTIMPLEMENTED)
	#define USE_BTagWriteSingleTag2
	#define EXT_BTagWriteSingleTag2
	#define GET_BTagWriteSingleTag2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriteSingleTag2(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriteSingleTag2  FALSE
	#define EXP_BTagWriteSingleTag2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriteSingleTag2
	#define EXT_BTagWriteSingleTag2
	#define GET_BTagWriteSingleTag2(fl)  CAL_CMGETAPI( "BTagWriteSingleTag2" ) 
	#define CAL_BTagWriteSingleTag2  BTagWriteSingleTag2
	#define CHK_BTagWriteSingleTag2  TRUE
	#define EXP_BTagWriteSingleTag2  CAL_CMEXPAPI( "BTagWriteSingleTag2" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriteSingleTag2
	#define EXT_BTagWriteSingleTag2
	#define GET_BTagWriteSingleTag2(fl)  CAL_CMGETAPI( "BTagWriteSingleTag2" ) 
	#define CAL_BTagWriteSingleTag2  BTagWriteSingleTag2
	#define CHK_BTagWriteSingleTag2  TRUE
	#define EXP_BTagWriteSingleTag2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriteSingleTag2", (RTS_UINTPTR)BTagWriteSingleTag2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriteSingleTag2
	#define EXT_CmpBinTagUtilBTagWriteSingleTag2
	#define GET_CmpBinTagUtilBTagWriteSingleTag2  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriteSingleTag2 pICmpBinTagUtil->IBTagWriteSingleTag2
	#define CHK_CmpBinTagUtilBTagWriteSingleTag2 (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriteSingleTag2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriteSingleTag2
	#define EXT_BTagWriteSingleTag2
	#define GET_BTagWriteSingleTag2(fl)  CAL_CMGETAPI( "BTagWriteSingleTag2" ) 
	#define CAL_BTagWriteSingleTag2 pICmpBinTagUtil->IBTagWriteSingleTag2
	#define CHK_BTagWriteSingleTag2 (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriteSingleTag2  CAL_CMEXPAPI( "BTagWriteSingleTag2" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriteSingleTag2  PFBTAGWRITESINGLETAG2 pfBTagWriteSingleTag2;
	#define EXT_BTagWriteSingleTag2  extern PFBTAGWRITESINGLETAG2 pfBTagWriteSingleTag2;
	#define GET_BTagWriteSingleTag2(fl)  s_pfCMGetAPI2( "BTagWriteSingleTag2", (RTS_VOID_FCTPTR *)&pfBTagWriteSingleTag2, (fl), 0, 0)
	#define CAL_BTagWriteSingleTag2  pfBTagWriteSingleTag2
	#define CHK_BTagWriteSingleTag2  (pfBTagWriteSingleTag2 != NULL)
	#define EXP_BTagWriteSingleTag2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriteSingleTag2", (RTS_UINTPTR)BTagWriteSingleTag2, 0, 0) 
#endif




/* --------- The reader interface ---------- */
/**
 * <description>
 *   Initialize a reader.
 * </description>
 * <param name="pReader" type="INOUT">
 *   Pass in a BINTAGREADER structure that will be initialized to an empty reader.
 * </param>
 * <param name="pBuffer" type="IN">
 *   The buffer that contains the bintag structure to be read. Do not alter the buffer
 *   until the reader isn't used any more.
 * </param>
 * <param name="ulBufferSize" type="IN">
 *   The size of pBuffer. It is expected that the whole buffer is in the bintag structure,
 *   possibly containing multiple toplevel tags. 
 * </param>
 */
RTS_RESULT CDECL BTagReaderInit(BINTAGREADER *pReader, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize);
typedef RTS_RESULT (CDECL * PFBTAGREADERINIT) (BINTAGREADER *pReader, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERINIT_NOTIMPLEMENTED)
	#define USE_BTagReaderInit
	#define EXT_BTagReaderInit
	#define GET_BTagReaderInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderInit(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderInit  FALSE
	#define EXP_BTagReaderInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderInit
	#define EXT_BTagReaderInit
	#define GET_BTagReaderInit(fl)  CAL_CMGETAPI( "BTagReaderInit" ) 
	#define CAL_BTagReaderInit  BTagReaderInit
	#define CHK_BTagReaderInit  TRUE
	#define EXP_BTagReaderInit  CAL_CMEXPAPI( "BTagReaderInit" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderInit
	#define EXT_BTagReaderInit
	#define GET_BTagReaderInit(fl)  CAL_CMGETAPI( "BTagReaderInit" ) 
	#define CAL_BTagReaderInit  BTagReaderInit
	#define CHK_BTagReaderInit  TRUE
	#define EXP_BTagReaderInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderInit", (RTS_UINTPTR)BTagReaderInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderInit
	#define EXT_CmpBinTagUtilBTagReaderInit
	#define GET_CmpBinTagUtilBTagReaderInit  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderInit pICmpBinTagUtil->IBTagReaderInit
	#define CHK_CmpBinTagUtilBTagReaderInit (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderInit
	#define EXT_BTagReaderInit
	#define GET_BTagReaderInit(fl)  CAL_CMGETAPI( "BTagReaderInit" ) 
	#define CAL_BTagReaderInit pICmpBinTagUtil->IBTagReaderInit
	#define CHK_BTagReaderInit (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderInit  CAL_CMEXPAPI( "BTagReaderInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderInit  PFBTAGREADERINIT pfBTagReaderInit;
	#define EXT_BTagReaderInit  extern PFBTAGREADERINIT pfBTagReaderInit;
	#define GET_BTagReaderInit(fl)  s_pfCMGetAPI2( "BTagReaderInit", (RTS_VOID_FCTPTR *)&pfBTagReaderInit, (fl), 0, 0)
	#define CAL_BTagReaderInit  pfBTagReaderInit
	#define CHK_BTagReaderInit  (pfBTagReaderInit != NULL)
	#define EXP_BTagReaderInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderInit", (RTS_UINTPTR)BTagReaderInit, 0, 0) 
#endif




/**
 * <description>
 *   Peek at the type of the next element as it would be returned by a call to BTagReaderMoveNext,
 *   but do not actually move to it (ie. the readers state will not be changed).
 * </description>
 * <param name="pReader" type="IN">
 *   An active reader.
 * </param>
 * <param name="pnElementType" type="OUT">
 *   Will be set to the type of the current element:
 *   <ul>
 *     <li>BTAG_ET_STARTTAG: Reader entered a new tag.</li>
 *     <li>BTAG_ET_ENDTAG: End of the current tag reached. New active tag is the surrounding tag</li>
 *     <li>BTAG_ET_EOF: End of the buffer reached. No more content/tags can be read</li>
 *   </ul>
 * </param>
 */
RTS_RESULT CDECL BTagReaderPeekNext(BINTAGREADER *pReader);
typedef RTS_RESULT (CDECL * PFBTAGREADERPEEKNEXT) (BINTAGREADER *pReader);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERPEEKNEXT_NOTIMPLEMENTED)
	#define USE_BTagReaderPeekNext
	#define EXT_BTagReaderPeekNext
	#define GET_BTagReaderPeekNext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderPeekNext(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderPeekNext  FALSE
	#define EXP_BTagReaderPeekNext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderPeekNext
	#define EXT_BTagReaderPeekNext
	#define GET_BTagReaderPeekNext(fl)  CAL_CMGETAPI( "BTagReaderPeekNext" ) 
	#define CAL_BTagReaderPeekNext  BTagReaderPeekNext
	#define CHK_BTagReaderPeekNext  TRUE
	#define EXP_BTagReaderPeekNext  CAL_CMEXPAPI( "BTagReaderPeekNext" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderPeekNext
	#define EXT_BTagReaderPeekNext
	#define GET_BTagReaderPeekNext(fl)  CAL_CMGETAPI( "BTagReaderPeekNext" ) 
	#define CAL_BTagReaderPeekNext  BTagReaderPeekNext
	#define CHK_BTagReaderPeekNext  TRUE
	#define EXP_BTagReaderPeekNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderPeekNext", (RTS_UINTPTR)BTagReaderPeekNext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderPeekNext
	#define EXT_CmpBinTagUtilBTagReaderPeekNext
	#define GET_CmpBinTagUtilBTagReaderPeekNext  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderPeekNext pICmpBinTagUtil->IBTagReaderPeekNext
	#define CHK_CmpBinTagUtilBTagReaderPeekNext (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderPeekNext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderPeekNext
	#define EXT_BTagReaderPeekNext
	#define GET_BTagReaderPeekNext(fl)  CAL_CMGETAPI( "BTagReaderPeekNext" ) 
	#define CAL_BTagReaderPeekNext pICmpBinTagUtil->IBTagReaderPeekNext
	#define CHK_BTagReaderPeekNext (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderPeekNext  CAL_CMEXPAPI( "BTagReaderPeekNext" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderPeekNext  PFBTAGREADERPEEKNEXT pfBTagReaderPeekNext;
	#define EXT_BTagReaderPeekNext  extern PFBTAGREADERPEEKNEXT pfBTagReaderPeekNext;
	#define GET_BTagReaderPeekNext(fl)  s_pfCMGetAPI2( "BTagReaderPeekNext", (RTS_VOID_FCTPTR *)&pfBTagReaderPeekNext, (fl), 0, 0)
	#define CAL_BTagReaderPeekNext  pfBTagReaderPeekNext
	#define CHK_BTagReaderPeekNext  (pfBTagReaderPeekNext != NULL)
	#define EXP_BTagReaderPeekNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderPeekNext", (RTS_UINTPTR)BTagReaderPeekNext, 0, 0) 
#endif




/**
 * <description>
 *   Let the reader move on to the next element.
 * </description>
 * <param name="pReader" type="IN">
 *   An active reader.
 * </param>
 * <param name="pnElementType" type="OUT">
 *   Will be set to the type of the current element:
 *   <ul>
 *     <li>BTAG_ET_STARTTAG: Reader entered a new tag.</li>
 *     <li>BTAG_ET_ENDTAG: End of the current tag reached. New active tag is the surrounding tag</li>
 *     <li>BTAG_ET_EOF: End of the buffer reached. No more content/tags can be read</li>
 *   </ul>
 * </param>
 */
RTS_RESULT CDECL BTagReaderMoveNext(BINTAGREADER *pReader, RTS_I32 *pnElementType);
typedef RTS_RESULT (CDECL * PFBTAGREADERMOVENEXT) (BINTAGREADER *pReader, RTS_I32 *pnElementType);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERMOVENEXT_NOTIMPLEMENTED)
	#define USE_BTagReaderMoveNext
	#define EXT_BTagReaderMoveNext
	#define GET_BTagReaderMoveNext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderMoveNext(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderMoveNext  FALSE
	#define EXP_BTagReaderMoveNext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderMoveNext
	#define EXT_BTagReaderMoveNext
	#define GET_BTagReaderMoveNext(fl)  CAL_CMGETAPI( "BTagReaderMoveNext" ) 
	#define CAL_BTagReaderMoveNext  BTagReaderMoveNext
	#define CHK_BTagReaderMoveNext  TRUE
	#define EXP_BTagReaderMoveNext  CAL_CMEXPAPI( "BTagReaderMoveNext" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderMoveNext
	#define EXT_BTagReaderMoveNext
	#define GET_BTagReaderMoveNext(fl)  CAL_CMGETAPI( "BTagReaderMoveNext" ) 
	#define CAL_BTagReaderMoveNext  BTagReaderMoveNext
	#define CHK_BTagReaderMoveNext  TRUE
	#define EXP_BTagReaderMoveNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderMoveNext", (RTS_UINTPTR)BTagReaderMoveNext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderMoveNext
	#define EXT_CmpBinTagUtilBTagReaderMoveNext
	#define GET_CmpBinTagUtilBTagReaderMoveNext  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderMoveNext pICmpBinTagUtil->IBTagReaderMoveNext
	#define CHK_CmpBinTagUtilBTagReaderMoveNext (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderMoveNext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderMoveNext
	#define EXT_BTagReaderMoveNext
	#define GET_BTagReaderMoveNext(fl)  CAL_CMGETAPI( "BTagReaderMoveNext" ) 
	#define CAL_BTagReaderMoveNext pICmpBinTagUtil->IBTagReaderMoveNext
	#define CHK_BTagReaderMoveNext (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderMoveNext  CAL_CMEXPAPI( "BTagReaderMoveNext" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderMoveNext  PFBTAGREADERMOVENEXT pfBTagReaderMoveNext;
	#define EXT_BTagReaderMoveNext  extern PFBTAGREADERMOVENEXT pfBTagReaderMoveNext;
	#define GET_BTagReaderMoveNext(fl)  s_pfCMGetAPI2( "BTagReaderMoveNext", (RTS_VOID_FCTPTR *)&pfBTagReaderMoveNext, (fl), 0, 0)
	#define CAL_BTagReaderMoveNext  pfBTagReaderMoveNext
	#define CHK_BTagReaderMoveNext  (pfBTagReaderMoveNext != NULL)
	#define EXP_BTagReaderMoveNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderMoveNext", (RTS_UINTPTR)BTagReaderMoveNext, 0, 0) 
#endif




/** 
 * <description>
 *   Jump to the end of the current tag. The next element that will be read by BTagReaderMoveNext
 *   will be the ENDTAG of the current tag.
 * </description>
 * <param name="pReader">
 * </param>
 */
RTS_RESULT CDECL BTagReaderSkipContent(BINTAGREADER *pReader);
typedef RTS_RESULT (CDECL * PFBTAGREADERSKIPCONTENT) (BINTAGREADER *pReader);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERSKIPCONTENT_NOTIMPLEMENTED)
	#define USE_BTagReaderSkipContent
	#define EXT_BTagReaderSkipContent
	#define GET_BTagReaderSkipContent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderSkipContent(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderSkipContent  FALSE
	#define EXP_BTagReaderSkipContent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderSkipContent
	#define EXT_BTagReaderSkipContent
	#define GET_BTagReaderSkipContent(fl)  CAL_CMGETAPI( "BTagReaderSkipContent" ) 
	#define CAL_BTagReaderSkipContent  BTagReaderSkipContent
	#define CHK_BTagReaderSkipContent  TRUE
	#define EXP_BTagReaderSkipContent  CAL_CMEXPAPI( "BTagReaderSkipContent" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderSkipContent
	#define EXT_BTagReaderSkipContent
	#define GET_BTagReaderSkipContent(fl)  CAL_CMGETAPI( "BTagReaderSkipContent" ) 
	#define CAL_BTagReaderSkipContent  BTagReaderSkipContent
	#define CHK_BTagReaderSkipContent  TRUE
	#define EXP_BTagReaderSkipContent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderSkipContent", (RTS_UINTPTR)BTagReaderSkipContent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderSkipContent
	#define EXT_CmpBinTagUtilBTagReaderSkipContent
	#define GET_CmpBinTagUtilBTagReaderSkipContent  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderSkipContent pICmpBinTagUtil->IBTagReaderSkipContent
	#define CHK_CmpBinTagUtilBTagReaderSkipContent (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderSkipContent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderSkipContent
	#define EXT_BTagReaderSkipContent
	#define GET_BTagReaderSkipContent(fl)  CAL_CMGETAPI( "BTagReaderSkipContent" ) 
	#define CAL_BTagReaderSkipContent pICmpBinTagUtil->IBTagReaderSkipContent
	#define CHK_BTagReaderSkipContent (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderSkipContent  CAL_CMEXPAPI( "BTagReaderSkipContent" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderSkipContent  PFBTAGREADERSKIPCONTENT pfBTagReaderSkipContent;
	#define EXT_BTagReaderSkipContent  extern PFBTAGREADERSKIPCONTENT pfBTagReaderSkipContent;
	#define GET_BTagReaderSkipContent(fl)  s_pfCMGetAPI2( "BTagReaderSkipContent", (RTS_VOID_FCTPTR *)&pfBTagReaderSkipContent, (fl), 0, 0)
	#define CAL_BTagReaderSkipContent  pfBTagReaderSkipContent
	#define CHK_BTagReaderSkipContent  (pfBTagReaderSkipContent != NULL)
	#define EXP_BTagReaderSkipContent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderSkipContent", (RTS_UINTPTR)BTagReaderSkipContent, 0, 0) 
#endif




/**
 * <description>
 *   Get the id of the current tag. Will fail if the reader is positioned at the toplevel.
 * </description>
 * <param name="pulTagId" type="OUT">
 *   Will be set to the id of the current tag.
 * </param>
 */
RTS_RESULT CDECL BTagReaderGetTagId(BINTAGREADER *pReader, RTS_UI32 *pulTagId);
typedef RTS_RESULT (CDECL * PFBTAGREADERGETTAGID) (BINTAGREADER *pReader, RTS_UI32 *pulTagId);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERGETTAGID_NOTIMPLEMENTED)
	#define USE_BTagReaderGetTagId
	#define EXT_BTagReaderGetTagId
	#define GET_BTagReaderGetTagId(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderGetTagId(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderGetTagId  FALSE
	#define EXP_BTagReaderGetTagId  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderGetTagId
	#define EXT_BTagReaderGetTagId
	#define GET_BTagReaderGetTagId(fl)  CAL_CMGETAPI( "BTagReaderGetTagId" ) 
	#define CAL_BTagReaderGetTagId  BTagReaderGetTagId
	#define CHK_BTagReaderGetTagId  TRUE
	#define EXP_BTagReaderGetTagId  CAL_CMEXPAPI( "BTagReaderGetTagId" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderGetTagId
	#define EXT_BTagReaderGetTagId
	#define GET_BTagReaderGetTagId(fl)  CAL_CMGETAPI( "BTagReaderGetTagId" ) 
	#define CAL_BTagReaderGetTagId  BTagReaderGetTagId
	#define CHK_BTagReaderGetTagId  TRUE
	#define EXP_BTagReaderGetTagId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetTagId", (RTS_UINTPTR)BTagReaderGetTagId, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderGetTagId
	#define EXT_CmpBinTagUtilBTagReaderGetTagId
	#define GET_CmpBinTagUtilBTagReaderGetTagId  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderGetTagId pICmpBinTagUtil->IBTagReaderGetTagId
	#define CHK_CmpBinTagUtilBTagReaderGetTagId (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderGetTagId  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderGetTagId
	#define EXT_BTagReaderGetTagId
	#define GET_BTagReaderGetTagId(fl)  CAL_CMGETAPI( "BTagReaderGetTagId" ) 
	#define CAL_BTagReaderGetTagId pICmpBinTagUtil->IBTagReaderGetTagId
	#define CHK_BTagReaderGetTagId (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderGetTagId  CAL_CMEXPAPI( "BTagReaderGetTagId" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderGetTagId  PFBTAGREADERGETTAGID pfBTagReaderGetTagId;
	#define EXT_BTagReaderGetTagId  extern PFBTAGREADERGETTAGID pfBTagReaderGetTagId;
	#define GET_BTagReaderGetTagId(fl)  s_pfCMGetAPI2( "BTagReaderGetTagId", (RTS_VOID_FCTPTR *)&pfBTagReaderGetTagId, (fl), 0, 0)
	#define CAL_BTagReaderGetTagId  pfBTagReaderGetTagId
	#define CHK_BTagReaderGetTagId  (pfBTagReaderGetTagId != NULL)
	#define EXP_BTagReaderGetTagId  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetTagId", (RTS_UINTPTR)BTagReaderGetTagId, 0, 0) 
#endif




/**
 * <description>
 *   Get the tag length of the current tag. Will fail if the reader is positioned at the toplevel.
 * </description>
 * <param name="pulTagLen" type="OUT">
 *   Will be set to the length of the current tag.
 * </param>
 */
RTS_RESULT CDECL BTagReaderGetTagLen(BINTAGREADER *pReader, RTS_UI32 *pulTagLen);
typedef RTS_RESULT (CDECL * PFBTAGREADERGETTAGLEN) (BINTAGREADER *pReader, RTS_UI32 *pulTagLen);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERGETTAGLEN_NOTIMPLEMENTED)
	#define USE_BTagReaderGetTagLen
	#define EXT_BTagReaderGetTagLen
	#define GET_BTagReaderGetTagLen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderGetTagLen(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderGetTagLen  FALSE
	#define EXP_BTagReaderGetTagLen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderGetTagLen
	#define EXT_BTagReaderGetTagLen
	#define GET_BTagReaderGetTagLen(fl)  CAL_CMGETAPI( "BTagReaderGetTagLen" ) 
	#define CAL_BTagReaderGetTagLen  BTagReaderGetTagLen
	#define CHK_BTagReaderGetTagLen  TRUE
	#define EXP_BTagReaderGetTagLen  CAL_CMEXPAPI( "BTagReaderGetTagLen" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderGetTagLen
	#define EXT_BTagReaderGetTagLen
	#define GET_BTagReaderGetTagLen(fl)  CAL_CMGETAPI( "BTagReaderGetTagLen" ) 
	#define CAL_BTagReaderGetTagLen  BTagReaderGetTagLen
	#define CHK_BTagReaderGetTagLen  TRUE
	#define EXP_BTagReaderGetTagLen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetTagLen", (RTS_UINTPTR)BTagReaderGetTagLen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderGetTagLen
	#define EXT_CmpBinTagUtilBTagReaderGetTagLen
	#define GET_CmpBinTagUtilBTagReaderGetTagLen  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderGetTagLen pICmpBinTagUtil->IBTagReaderGetTagLen
	#define CHK_CmpBinTagUtilBTagReaderGetTagLen (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderGetTagLen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderGetTagLen
	#define EXT_BTagReaderGetTagLen
	#define GET_BTagReaderGetTagLen(fl)  CAL_CMGETAPI( "BTagReaderGetTagLen" ) 
	#define CAL_BTagReaderGetTagLen pICmpBinTagUtil->IBTagReaderGetTagLen
	#define CHK_BTagReaderGetTagLen (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderGetTagLen  CAL_CMEXPAPI( "BTagReaderGetTagLen" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderGetTagLen  PFBTAGREADERGETTAGLEN pfBTagReaderGetTagLen;
	#define EXT_BTagReaderGetTagLen  extern PFBTAGREADERGETTAGLEN pfBTagReaderGetTagLen;
	#define GET_BTagReaderGetTagLen(fl)  s_pfCMGetAPI2( "BTagReaderGetTagLen", (RTS_VOID_FCTPTR *)&pfBTagReaderGetTagLen, (fl), 0, 0)
	#define CAL_BTagReaderGetTagLen  pfBTagReaderGetTagLen
	#define CHK_BTagReaderGetTagLen  (pfBTagReaderGetTagLen != NULL)
	#define EXP_BTagReaderGetTagLen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetTagLen", (RTS_UINTPTR)BTagReaderGetTagLen, 0, 0) 
#endif




/**
 * <description>
 *   Check if the current tag is a data tag (ie. contains raw content) or does contain
 *   subtags. Will return an error if the reader is not positioned on a tag (ie. is at the 
 *   toplevel). However, in that case pbIsData will be set to FALSE.
 * </description>
 * <param name="pReader" type="IN">
 * </param>
 * <param name="pbIsDataTag" type="IN">
 *   Is set to TRUE, if the current tag is a data tag, FALSE otherwise.
 * </param>
 */
RTS_RESULT CDECL BTagReaderIsDataTag(BINTAGREADER *pReader, RTS_I32 *pbIsDataTag);
typedef RTS_RESULT (CDECL * PFBTAGREADERISDATATAG) (BINTAGREADER *pReader, RTS_I32 *pbIsDataTag);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERISDATATAG_NOTIMPLEMENTED)
	#define USE_BTagReaderIsDataTag
	#define EXT_BTagReaderIsDataTag
	#define GET_BTagReaderIsDataTag(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderIsDataTag(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderIsDataTag  FALSE
	#define EXP_BTagReaderIsDataTag  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderIsDataTag
	#define EXT_BTagReaderIsDataTag
	#define GET_BTagReaderIsDataTag(fl)  CAL_CMGETAPI( "BTagReaderIsDataTag" ) 
	#define CAL_BTagReaderIsDataTag  BTagReaderIsDataTag
	#define CHK_BTagReaderIsDataTag  TRUE
	#define EXP_BTagReaderIsDataTag  CAL_CMEXPAPI( "BTagReaderIsDataTag" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderIsDataTag
	#define EXT_BTagReaderIsDataTag
	#define GET_BTagReaderIsDataTag(fl)  CAL_CMGETAPI( "BTagReaderIsDataTag" ) 
	#define CAL_BTagReaderIsDataTag  BTagReaderIsDataTag
	#define CHK_BTagReaderIsDataTag  TRUE
	#define EXP_BTagReaderIsDataTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderIsDataTag", (RTS_UINTPTR)BTagReaderIsDataTag, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderIsDataTag
	#define EXT_CmpBinTagUtilBTagReaderIsDataTag
	#define GET_CmpBinTagUtilBTagReaderIsDataTag  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderIsDataTag pICmpBinTagUtil->IBTagReaderIsDataTag
	#define CHK_CmpBinTagUtilBTagReaderIsDataTag (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderIsDataTag  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderIsDataTag
	#define EXT_BTagReaderIsDataTag
	#define GET_BTagReaderIsDataTag(fl)  CAL_CMGETAPI( "BTagReaderIsDataTag" ) 
	#define CAL_BTagReaderIsDataTag pICmpBinTagUtil->IBTagReaderIsDataTag
	#define CHK_BTagReaderIsDataTag (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderIsDataTag  CAL_CMEXPAPI( "BTagReaderIsDataTag" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderIsDataTag  PFBTAGREADERISDATATAG pfBTagReaderIsDataTag;
	#define EXT_BTagReaderIsDataTag  extern PFBTAGREADERISDATATAG pfBTagReaderIsDataTag;
	#define GET_BTagReaderIsDataTag(fl)  s_pfCMGetAPI2( "BTagReaderIsDataTag", (RTS_VOID_FCTPTR *)&pfBTagReaderIsDataTag, (fl), 0, 0)
	#define CAL_BTagReaderIsDataTag  pfBTagReaderIsDataTag
	#define CHK_BTagReaderIsDataTag  (pfBTagReaderIsDataTag != NULL)
	#define EXP_BTagReaderIsDataTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderIsDataTag", (RTS_UINTPTR)BTagReaderIsDataTag, 0, 0) 
#endif




/**
 * <description>
 *   Get a pointer on the content of the current tag. 
 * </description>
 * <param name="pReader" type="IN">
 * </param>
 * <param name="ppBuffer" type="OUT">
 *   Will be set to point at the content of the current tag.
 * </param>
 * <param name="pulSize" type="OUT">
 *   Will contain the size of the content.
 * </param>
 */
RTS_RESULT CDECL BTagReaderGetComplexContent(BINTAGREADER *pReader, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize);
typedef RTS_RESULT (CDECL * PFBTAGREADERGETCOMPLEXCONTENT) (BINTAGREADER *pReader, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERGETCOMPLEXCONTENT_NOTIMPLEMENTED)
	#define USE_BTagReaderGetComplexContent
	#define EXT_BTagReaderGetComplexContent
	#define GET_BTagReaderGetComplexContent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderGetComplexContent(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderGetComplexContent  FALSE
	#define EXP_BTagReaderGetComplexContent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderGetComplexContent
	#define EXT_BTagReaderGetComplexContent
	#define GET_BTagReaderGetComplexContent(fl)  CAL_CMGETAPI( "BTagReaderGetComplexContent" ) 
	#define CAL_BTagReaderGetComplexContent  BTagReaderGetComplexContent
	#define CHK_BTagReaderGetComplexContent  TRUE
	#define EXP_BTagReaderGetComplexContent  CAL_CMEXPAPI( "BTagReaderGetComplexContent" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderGetComplexContent
	#define EXT_BTagReaderGetComplexContent
	#define GET_BTagReaderGetComplexContent(fl)  CAL_CMGETAPI( "BTagReaderGetComplexContent" ) 
	#define CAL_BTagReaderGetComplexContent  BTagReaderGetComplexContent
	#define CHK_BTagReaderGetComplexContent  TRUE
	#define EXP_BTagReaderGetComplexContent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetComplexContent", (RTS_UINTPTR)BTagReaderGetComplexContent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderGetComplexContent
	#define EXT_CmpBinTagUtilBTagReaderGetComplexContent
	#define GET_CmpBinTagUtilBTagReaderGetComplexContent  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderGetComplexContent pICmpBinTagUtil->IBTagReaderGetComplexContent
	#define CHK_CmpBinTagUtilBTagReaderGetComplexContent (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderGetComplexContent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderGetComplexContent
	#define EXT_BTagReaderGetComplexContent
	#define GET_BTagReaderGetComplexContent(fl)  CAL_CMGETAPI( "BTagReaderGetComplexContent" ) 
	#define CAL_BTagReaderGetComplexContent pICmpBinTagUtil->IBTagReaderGetComplexContent
	#define CHK_BTagReaderGetComplexContent (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderGetComplexContent  CAL_CMEXPAPI( "BTagReaderGetComplexContent" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderGetComplexContent  PFBTAGREADERGETCOMPLEXCONTENT pfBTagReaderGetComplexContent;
	#define EXT_BTagReaderGetComplexContent  extern PFBTAGREADERGETCOMPLEXCONTENT pfBTagReaderGetComplexContent;
	#define GET_BTagReaderGetComplexContent(fl)  s_pfCMGetAPI2( "BTagReaderGetComplexContent", (RTS_VOID_FCTPTR *)&pfBTagReaderGetComplexContent, (fl), 0, 0)
	#define CAL_BTagReaderGetComplexContent  pfBTagReaderGetComplexContent
	#define CHK_BTagReaderGetComplexContent  (pfBTagReaderGetComplexContent != NULL)
	#define EXP_BTagReaderGetComplexContent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetComplexContent", (RTS_UINTPTR)BTagReaderGetComplexContent, 0, 0) 
#endif




/**
 * <description>
 *   Get a pointer on the content of the current tag. 
 *   Will fail if the reader is on the toplevel
 *   (no current tag) or the current tag is not a data tag.
 * </description>
 * <param name="pReader" type="IN">
 * </param>
 * <param name="ppBuffer" type="OUT">
 *   Will be set to point at the content of the current tag.
 * </param>
 * <param name="pulSize" type="OUT">
 *   Will contain the size of the content.
 * </param>
 */
RTS_RESULT CDECL BTagReaderGetContent(BINTAGREADER *pReader, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize);
typedef RTS_RESULT (CDECL * PFBTAGREADERGETCONTENT) (BINTAGREADER *pReader, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERGETCONTENT_NOTIMPLEMENTED)
	#define USE_BTagReaderGetContent
	#define EXT_BTagReaderGetContent
	#define GET_BTagReaderGetContent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderGetContent(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderGetContent  FALSE
	#define EXP_BTagReaderGetContent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderGetContent
	#define EXT_BTagReaderGetContent
	#define GET_BTagReaderGetContent(fl)  CAL_CMGETAPI( "BTagReaderGetContent" ) 
	#define CAL_BTagReaderGetContent  BTagReaderGetContent
	#define CHK_BTagReaderGetContent  TRUE
	#define EXP_BTagReaderGetContent  CAL_CMEXPAPI( "BTagReaderGetContent" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderGetContent
	#define EXT_BTagReaderGetContent
	#define GET_BTagReaderGetContent(fl)  CAL_CMGETAPI( "BTagReaderGetContent" ) 
	#define CAL_BTagReaderGetContent  BTagReaderGetContent
	#define CHK_BTagReaderGetContent  TRUE
	#define EXP_BTagReaderGetContent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetContent", (RTS_UINTPTR)BTagReaderGetContent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderGetContent
	#define EXT_CmpBinTagUtilBTagReaderGetContent
	#define GET_CmpBinTagUtilBTagReaderGetContent  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderGetContent pICmpBinTagUtil->IBTagReaderGetContent
	#define CHK_CmpBinTagUtilBTagReaderGetContent (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderGetContent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderGetContent
	#define EXT_BTagReaderGetContent
	#define GET_BTagReaderGetContent(fl)  CAL_CMGETAPI( "BTagReaderGetContent" ) 
	#define CAL_BTagReaderGetContent pICmpBinTagUtil->IBTagReaderGetContent
	#define CHK_BTagReaderGetContent (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderGetContent  CAL_CMEXPAPI( "BTagReaderGetContent" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderGetContent  PFBTAGREADERGETCONTENT pfBTagReaderGetContent;
	#define EXT_BTagReaderGetContent  extern PFBTAGREADERGETCONTENT pfBTagReaderGetContent;
	#define GET_BTagReaderGetContent(fl)  s_pfCMGetAPI2( "BTagReaderGetContent", (RTS_VOID_FCTPTR *)&pfBTagReaderGetContent, (fl), 0, 0)
	#define CAL_BTagReaderGetContent  pfBTagReaderGetContent
	#define CHK_BTagReaderGetContent  (pfBTagReaderGetContent != NULL)
	#define EXP_BTagReaderGetContent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetContent", (RTS_UINTPTR)BTagReaderGetContent, 0, 0) 
#endif




/**
 * <description>
 *   Like BTagReaderGetContent but treats the content as a string. If a '\0' is found
 *   before the end of the tag, pulSize is set to the length up to (and including) the zero.
 *   If bAddEndOfString is set, then '\0' is written at the last byte, if '\0' is not found. 
 * </description>
 * <note>
 *   If the length of the tag is 0 then no '\0' can be written into the buffer without overwriting
 *   other content. In that case a reference to a static empty string is returned. 
 * </note>
 * <param name="pReader" type="IN">
 * </param>
 * <param name="ppString" type="OUT">
 *   Will be set to the start of the string. Will not necessarily point into the readers buffer.
 *   (see note).
 * </param>
 * <param name="pulSize" type="IN">
 *   The length of the string INCLUDING the trailing '\0',
 *   ie. "strlen(ppString)+1".
 * </param>
 * <param name="bAddEndOfString" type="IN">
 *   Force a trailing zero byte to be added, if it doesn't exist within the bounds of the tag.
 *   Will overwrite the last char of the string.
 *   Note: This option will alter the content of the buffer!
 * </param>
 */
RTS_RESULT CDECL BTagReaderGetString(BINTAGREADER *pReader, char **ppString, RTS_UI32 *pulSize, RTS_I32 bAddEndOfString);
typedef RTS_RESULT (CDECL * PFBTAGREADERGETSTRING) (BINTAGREADER *pReader, char **ppString, RTS_UI32 *pulSize, RTS_I32 bAddEndOfString);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERGETSTRING_NOTIMPLEMENTED)
	#define USE_BTagReaderGetString
	#define EXT_BTagReaderGetString
	#define GET_BTagReaderGetString(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderGetString(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderGetString  FALSE
	#define EXP_BTagReaderGetString  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderGetString
	#define EXT_BTagReaderGetString
	#define GET_BTagReaderGetString(fl)  CAL_CMGETAPI( "BTagReaderGetString" ) 
	#define CAL_BTagReaderGetString  BTagReaderGetString
	#define CHK_BTagReaderGetString  TRUE
	#define EXP_BTagReaderGetString  CAL_CMEXPAPI( "BTagReaderGetString" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderGetString
	#define EXT_BTagReaderGetString
	#define GET_BTagReaderGetString(fl)  CAL_CMGETAPI( "BTagReaderGetString" ) 
	#define CAL_BTagReaderGetString  BTagReaderGetString
	#define CHK_BTagReaderGetString  TRUE
	#define EXP_BTagReaderGetString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetString", (RTS_UINTPTR)BTagReaderGetString, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderGetString
	#define EXT_CmpBinTagUtilBTagReaderGetString
	#define GET_CmpBinTagUtilBTagReaderGetString  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderGetString pICmpBinTagUtil->IBTagReaderGetString
	#define CHK_CmpBinTagUtilBTagReaderGetString (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderGetString  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderGetString
	#define EXT_BTagReaderGetString
	#define GET_BTagReaderGetString(fl)  CAL_CMGETAPI( "BTagReaderGetString" ) 
	#define CAL_BTagReaderGetString pICmpBinTagUtil->IBTagReaderGetString
	#define CHK_BTagReaderGetString (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderGetString  CAL_CMEXPAPI( "BTagReaderGetString" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderGetString  PFBTAGREADERGETSTRING pfBTagReaderGetString;
	#define EXT_BTagReaderGetString  extern PFBTAGREADERGETSTRING pfBTagReaderGetString;
	#define GET_BTagReaderGetString(fl)  s_pfCMGetAPI2( "BTagReaderGetString", (RTS_VOID_FCTPTR *)&pfBTagReaderGetString, (fl), 0, 0)
	#define CAL_BTagReaderGetString  pfBTagReaderGetString
	#define CHK_BTagReaderGetString  (pfBTagReaderGetString != NULL)
	#define EXP_BTagReaderGetString  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetString", (RTS_UINTPTR)BTagReaderGetString, 0, 0) 
#endif




/**
 * <description>
 *   Save the current state of the reader. A later call to BTagReaderRestoreSavepoint will
 *   reset the reader and its buffer to that state. Any number of savepoints may be created.
 *   The caller must use multiple savepoints in a stack-like fashion: If savepoints are
 *   created in the order 1,2,3,4 then they must be restored in opposite order only. Not 
 *   every savepoint must be restored - but if one is restored all savepoints created after
 *   that one MUST NOT BE USED any more.
 *   Examples: ("sX"="create savepoint X", "rX"="Restore savepoint X", 
 *			    "(x,y,z)"="stack of valid savepoints")
 *   The following sequences are valid:
 *   () s1 (1) s2 (1,2) s3 (1,2,3) r2 (1) r1 ()
 *   () s1 (1) s2 (1,2) s3 (1,2,3) r2 (1) s4 (1,4) r4 (1)  
 *   whereas this one is invalid:
 *   () s1 (1) s2 (1,2) s3 (1,2,3) r2 (1) r3 [3 is not valid at this point]
 * 
 *   The reader does not track the stack of savepoints, therefore it cannot
 *   detect errors in the restore order. It's the responsibility of the application
 *   to ensure the correct restore order. Failure in doing so will lead to an 
 *   inconsistent state and may well corrupt the whole document.
 *   Besides the restore order savepoints are independent of each other. The application
 *   may delete/reuse a savepoint anytime it doesn't need it anymore.
 * 
 *   Savepoints cannot be used to transfer the state of a reader to a second reader.
 *   Any attempt in doing so will lead to undefined behaviour.
 * </description>
 * <param name="pReader" type="IN"></param>
 * <param name="pSavePoint" type="OUT">
 *   Will receive all information needed to restore the current state of the reader.
 * </param>
 */ 
RTS_RESULT CDECL BTagReaderCreateSavepoint(BINTAGREADER *pReader, BINTAGREADERSAVEPOINT *pSavepoint);
typedef RTS_RESULT (CDECL * PFBTAGREADERCREATESAVEPOINT) (BINTAGREADER *pReader, BINTAGREADERSAVEPOINT *pSavepoint);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERCREATESAVEPOINT_NOTIMPLEMENTED)
	#define USE_BTagReaderCreateSavepoint
	#define EXT_BTagReaderCreateSavepoint
	#define GET_BTagReaderCreateSavepoint(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderCreateSavepoint(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderCreateSavepoint  FALSE
	#define EXP_BTagReaderCreateSavepoint  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderCreateSavepoint
	#define EXT_BTagReaderCreateSavepoint
	#define GET_BTagReaderCreateSavepoint(fl)  CAL_CMGETAPI( "BTagReaderCreateSavepoint" ) 
	#define CAL_BTagReaderCreateSavepoint  BTagReaderCreateSavepoint
	#define CHK_BTagReaderCreateSavepoint  TRUE
	#define EXP_BTagReaderCreateSavepoint  CAL_CMEXPAPI( "BTagReaderCreateSavepoint" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderCreateSavepoint
	#define EXT_BTagReaderCreateSavepoint
	#define GET_BTagReaderCreateSavepoint(fl)  CAL_CMGETAPI( "BTagReaderCreateSavepoint" ) 
	#define CAL_BTagReaderCreateSavepoint  BTagReaderCreateSavepoint
	#define CHK_BTagReaderCreateSavepoint  TRUE
	#define EXP_BTagReaderCreateSavepoint  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderCreateSavepoint", (RTS_UINTPTR)BTagReaderCreateSavepoint, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderCreateSavepoint
	#define EXT_CmpBinTagUtilBTagReaderCreateSavepoint
	#define GET_CmpBinTagUtilBTagReaderCreateSavepoint  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderCreateSavepoint pICmpBinTagUtil->IBTagReaderCreateSavepoint
	#define CHK_CmpBinTagUtilBTagReaderCreateSavepoint (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderCreateSavepoint  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderCreateSavepoint
	#define EXT_BTagReaderCreateSavepoint
	#define GET_BTagReaderCreateSavepoint(fl)  CAL_CMGETAPI( "BTagReaderCreateSavepoint" ) 
	#define CAL_BTagReaderCreateSavepoint pICmpBinTagUtil->IBTagReaderCreateSavepoint
	#define CHK_BTagReaderCreateSavepoint (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderCreateSavepoint  CAL_CMEXPAPI( "BTagReaderCreateSavepoint" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderCreateSavepoint  PFBTAGREADERCREATESAVEPOINT pfBTagReaderCreateSavepoint;
	#define EXT_BTagReaderCreateSavepoint  extern PFBTAGREADERCREATESAVEPOINT pfBTagReaderCreateSavepoint;
	#define GET_BTagReaderCreateSavepoint(fl)  s_pfCMGetAPI2( "BTagReaderCreateSavepoint", (RTS_VOID_FCTPTR *)&pfBTagReaderCreateSavepoint, (fl), 0, 0)
	#define CAL_BTagReaderCreateSavepoint  pfBTagReaderCreateSavepoint
	#define CHK_BTagReaderCreateSavepoint  (pfBTagReaderCreateSavepoint != NULL)
	#define EXP_BTagReaderCreateSavepoint  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderCreateSavepoint", (RTS_UINTPTR)BTagReaderCreateSavepoint, 0, 0) 
#endif




/**
 * <description>
 *   Restore the state of the reader to one previously saved using 
 *   BTagReaderCreateSavepoint. See there for a detailed description on how to use
 *   these functions.
 * </description>
 * <param name="pReader" type="IN">
 *   Must be the same reader that the savepoint was created on.
 * </param>
 * <param name="pSavepoint" type="IN">
 *   A previously created savepoint.
 * </param>
 */
RTS_RESULT CDECL BTagReaderRestoreSavepoint(BINTAGREADER *pReader, BINTAGREADERSAVEPOINT *pSavepoint);
typedef RTS_RESULT (CDECL * PFBTAGREADERRESTORESAVEPOINT) (BINTAGREADER *pReader, BINTAGREADERSAVEPOINT *pSavepoint);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERRESTORESAVEPOINT_NOTIMPLEMENTED)
	#define USE_BTagReaderRestoreSavepoint
	#define EXT_BTagReaderRestoreSavepoint
	#define GET_BTagReaderRestoreSavepoint(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderRestoreSavepoint(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderRestoreSavepoint  FALSE
	#define EXP_BTagReaderRestoreSavepoint  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderRestoreSavepoint
	#define EXT_BTagReaderRestoreSavepoint
	#define GET_BTagReaderRestoreSavepoint(fl)  CAL_CMGETAPI( "BTagReaderRestoreSavepoint" ) 
	#define CAL_BTagReaderRestoreSavepoint  BTagReaderRestoreSavepoint
	#define CHK_BTagReaderRestoreSavepoint  TRUE
	#define EXP_BTagReaderRestoreSavepoint  CAL_CMEXPAPI( "BTagReaderRestoreSavepoint" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderRestoreSavepoint
	#define EXT_BTagReaderRestoreSavepoint
	#define GET_BTagReaderRestoreSavepoint(fl)  CAL_CMGETAPI( "BTagReaderRestoreSavepoint" ) 
	#define CAL_BTagReaderRestoreSavepoint  BTagReaderRestoreSavepoint
	#define CHK_BTagReaderRestoreSavepoint  TRUE
	#define EXP_BTagReaderRestoreSavepoint  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderRestoreSavepoint", (RTS_UINTPTR)BTagReaderRestoreSavepoint, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderRestoreSavepoint
	#define EXT_CmpBinTagUtilBTagReaderRestoreSavepoint
	#define GET_CmpBinTagUtilBTagReaderRestoreSavepoint  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderRestoreSavepoint pICmpBinTagUtil->IBTagReaderRestoreSavepoint
	#define CHK_CmpBinTagUtilBTagReaderRestoreSavepoint (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderRestoreSavepoint  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderRestoreSavepoint
	#define EXT_BTagReaderRestoreSavepoint
	#define GET_BTagReaderRestoreSavepoint(fl)  CAL_CMGETAPI( "BTagReaderRestoreSavepoint" ) 
	#define CAL_BTagReaderRestoreSavepoint pICmpBinTagUtil->IBTagReaderRestoreSavepoint
	#define CHK_BTagReaderRestoreSavepoint (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderRestoreSavepoint  CAL_CMEXPAPI( "BTagReaderRestoreSavepoint" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderRestoreSavepoint  PFBTAGREADERRESTORESAVEPOINT pfBTagReaderRestoreSavepoint;
	#define EXT_BTagReaderRestoreSavepoint  extern PFBTAGREADERRESTORESAVEPOINT pfBTagReaderRestoreSavepoint;
	#define GET_BTagReaderRestoreSavepoint(fl)  s_pfCMGetAPI2( "BTagReaderRestoreSavepoint", (RTS_VOID_FCTPTR *)&pfBTagReaderRestoreSavepoint, (fl), 0, 0)
	#define CAL_BTagReaderRestoreSavepoint  pfBTagReaderRestoreSavepoint
	#define CHK_BTagReaderRestoreSavepoint  (pfBTagReaderRestoreSavepoint != NULL)
	#define EXP_BTagReaderRestoreSavepoint  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderRestoreSavepoint", (RTS_UINTPTR)BTagReaderRestoreSavepoint, 0, 0) 
#endif




RTS_RESULT CDECL BTagWriterFileInit(BINTAGWRITER *pWriter, RTS_HANDLE hFile);
typedef RTS_RESULT (CDECL * PFBTAGWRITERFILEINIT) (BINTAGWRITER *pWriter, RTS_HANDLE hFile);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERFILEINIT_NOTIMPLEMENTED)
	#define USE_BTagWriterFileInit
	#define EXT_BTagWriterFileInit
	#define GET_BTagWriterFileInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterFileInit(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterFileInit  FALSE
	#define EXP_BTagWriterFileInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterFileInit
	#define EXT_BTagWriterFileInit
	#define GET_BTagWriterFileInit(fl)  CAL_CMGETAPI( "BTagWriterFileInit" ) 
	#define CAL_BTagWriterFileInit  BTagWriterFileInit
	#define CHK_BTagWriterFileInit  TRUE
	#define EXP_BTagWriterFileInit  CAL_CMEXPAPI( "BTagWriterFileInit" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterFileInit
	#define EXT_BTagWriterFileInit
	#define GET_BTagWriterFileInit(fl)  CAL_CMGETAPI( "BTagWriterFileInit" ) 
	#define CAL_BTagWriterFileInit  BTagWriterFileInit
	#define CHK_BTagWriterFileInit  TRUE
	#define EXP_BTagWriterFileInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFileInit", (RTS_UINTPTR)BTagWriterFileInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterFileInit
	#define EXT_CmpBinTagUtilBTagWriterFileInit
	#define GET_CmpBinTagUtilBTagWriterFileInit  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterFileInit pICmpBinTagUtil->IBTagWriterFileInit
	#define CHK_CmpBinTagUtilBTagWriterFileInit (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterFileInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterFileInit
	#define EXT_BTagWriterFileInit
	#define GET_BTagWriterFileInit(fl)  CAL_CMGETAPI( "BTagWriterFileInit" ) 
	#define CAL_BTagWriterFileInit pICmpBinTagUtil->IBTagWriterFileInit
	#define CHK_BTagWriterFileInit (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterFileInit  CAL_CMEXPAPI( "BTagWriterFileInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterFileInit  PFBTAGWRITERFILEINIT pfBTagWriterFileInit;
	#define EXT_BTagWriterFileInit  extern PFBTAGWRITERFILEINIT pfBTagWriterFileInit;
	#define GET_BTagWriterFileInit(fl)  s_pfCMGetAPI2( "BTagWriterFileInit", (RTS_VOID_FCTPTR *)&pfBTagWriterFileInit, (fl), 0, 0)
	#define CAL_BTagWriterFileInit  pfBTagWriterFileInit
	#define CHK_BTagWriterFileInit  (pfBTagWriterFileInit != NULL)
	#define EXP_BTagWriterFileInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFileInit", (RTS_UINTPTR)BTagWriterFileInit, 0, 0) 
#endif



RTS_RESULT CDECL BTagWriterFileInit2(BINTAGWRITER *pWriter, char *pszFileName);
typedef RTS_RESULT (CDECL * PFBTAGWRITERFILEINIT2) (BINTAGWRITER *pWriter, char *pszFileName);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERFILEINIT2_NOTIMPLEMENTED)
	#define USE_BTagWriterFileInit2
	#define EXT_BTagWriterFileInit2
	#define GET_BTagWriterFileInit2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterFileInit2(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterFileInit2  FALSE
	#define EXP_BTagWriterFileInit2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterFileInit2
	#define EXT_BTagWriterFileInit2
	#define GET_BTagWriterFileInit2(fl)  CAL_CMGETAPI( "BTagWriterFileInit2" ) 
	#define CAL_BTagWriterFileInit2  BTagWriterFileInit2
	#define CHK_BTagWriterFileInit2  TRUE
	#define EXP_BTagWriterFileInit2  CAL_CMEXPAPI( "BTagWriterFileInit2" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterFileInit2
	#define EXT_BTagWriterFileInit2
	#define GET_BTagWriterFileInit2(fl)  CAL_CMGETAPI( "BTagWriterFileInit2" ) 
	#define CAL_BTagWriterFileInit2  BTagWriterFileInit2
	#define CHK_BTagWriterFileInit2  TRUE
	#define EXP_BTagWriterFileInit2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFileInit2", (RTS_UINTPTR)BTagWriterFileInit2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterFileInit2
	#define EXT_CmpBinTagUtilBTagWriterFileInit2
	#define GET_CmpBinTagUtilBTagWriterFileInit2  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterFileInit2 pICmpBinTagUtil->IBTagWriterFileInit2
	#define CHK_CmpBinTagUtilBTagWriterFileInit2 (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterFileInit2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterFileInit2
	#define EXT_BTagWriterFileInit2
	#define GET_BTagWriterFileInit2(fl)  CAL_CMGETAPI( "BTagWriterFileInit2" ) 
	#define CAL_BTagWriterFileInit2 pICmpBinTagUtil->IBTagWriterFileInit2
	#define CHK_BTagWriterFileInit2 (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterFileInit2  CAL_CMEXPAPI( "BTagWriterFileInit2" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterFileInit2  PFBTAGWRITERFILEINIT2 pfBTagWriterFileInit2;
	#define EXT_BTagWriterFileInit2  extern PFBTAGWRITERFILEINIT2 pfBTagWriterFileInit2;
	#define GET_BTagWriterFileInit2(fl)  s_pfCMGetAPI2( "BTagWriterFileInit2", (RTS_VOID_FCTPTR *)&pfBTagWriterFileInit2, (fl), 0, 0)
	#define CAL_BTagWriterFileInit2  pfBTagWriterFileInit2
	#define CHK_BTagWriterFileInit2  (pfBTagWriterFileInit2 != NULL)
	#define EXP_BTagWriterFileInit2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFileInit2", (RTS_UINTPTR)BTagWriterFileInit2, 0, 0) 
#endif



RTS_RESULT CDECL BTagWriterFileStreamInit(BINTAGWRITER *pWriter, RTS_HANDLE hFile);
typedef RTS_RESULT (CDECL * PFBTAGWRITERFILESTREAMINIT) (BINTAGWRITER *pWriter, RTS_HANDLE hFile);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERFILESTREAMINIT_NOTIMPLEMENTED)
	#define USE_BTagWriterFileStreamInit
	#define EXT_BTagWriterFileStreamInit
	#define GET_BTagWriterFileStreamInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterFileStreamInit(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterFileStreamInit  FALSE
	#define EXP_BTagWriterFileStreamInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterFileStreamInit
	#define EXT_BTagWriterFileStreamInit
	#define GET_BTagWriterFileStreamInit(fl)  CAL_CMGETAPI( "BTagWriterFileStreamInit" ) 
	#define CAL_BTagWriterFileStreamInit  BTagWriterFileStreamInit
	#define CHK_BTagWriterFileStreamInit  TRUE
	#define EXP_BTagWriterFileStreamInit  CAL_CMEXPAPI( "BTagWriterFileStreamInit" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterFileStreamInit
	#define EXT_BTagWriterFileStreamInit
	#define GET_BTagWriterFileStreamInit(fl)  CAL_CMGETAPI( "BTagWriterFileStreamInit" ) 
	#define CAL_BTagWriterFileStreamInit  BTagWriterFileStreamInit
	#define CHK_BTagWriterFileStreamInit  TRUE
	#define EXP_BTagWriterFileStreamInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFileStreamInit", (RTS_UINTPTR)BTagWriterFileStreamInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterFileStreamInit
	#define EXT_CmpBinTagUtilBTagWriterFileStreamInit
	#define GET_CmpBinTagUtilBTagWriterFileStreamInit  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterFileStreamInit pICmpBinTagUtil->IBTagWriterFileStreamInit
	#define CHK_CmpBinTagUtilBTagWriterFileStreamInit (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterFileStreamInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterFileStreamInit
	#define EXT_BTagWriterFileStreamInit
	#define GET_BTagWriterFileStreamInit(fl)  CAL_CMGETAPI( "BTagWriterFileStreamInit" ) 
	#define CAL_BTagWriterFileStreamInit pICmpBinTagUtil->IBTagWriterFileStreamInit
	#define CHK_BTagWriterFileStreamInit (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterFileStreamInit  CAL_CMEXPAPI( "BTagWriterFileStreamInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterFileStreamInit  PFBTAGWRITERFILESTREAMINIT pfBTagWriterFileStreamInit;
	#define EXT_BTagWriterFileStreamInit  extern PFBTAGWRITERFILESTREAMINIT pfBTagWriterFileStreamInit;
	#define GET_BTagWriterFileStreamInit(fl)  s_pfCMGetAPI2( "BTagWriterFileStreamInit", (RTS_VOID_FCTPTR *)&pfBTagWriterFileStreamInit, (fl), 0, 0)
	#define CAL_BTagWriterFileStreamInit  pfBTagWriterFileStreamInit
	#define CHK_BTagWriterFileStreamInit  (pfBTagWriterFileStreamInit != NULL)
	#define EXP_BTagWriterFileStreamInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFileStreamInit", (RTS_UINTPTR)BTagWriterFileStreamInit, 0, 0) 
#endif



RTS_RESULT CDECL BTagWriterFileStreamInit2(BINTAGWRITER *pWriter, RTS_HANDLE hFile, PFUPDATECRC pfUpdateCRC, void *pParameterUpdateCRC);
typedef RTS_RESULT (CDECL * PFBTAGWRITERFILESTREAMINIT2) (BINTAGWRITER *pWriter, RTS_HANDLE hFile, PFUPDATECRC pfUpdateCRC, void *pParameterUpdateCRC);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGWRITERFILESTREAMINIT2_NOTIMPLEMENTED)
	#define USE_BTagWriterFileStreamInit2
	#define EXT_BTagWriterFileStreamInit2
	#define GET_BTagWriterFileStreamInit2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagWriterFileStreamInit2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagWriterFileStreamInit2  FALSE
	#define EXP_BTagWriterFileStreamInit2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagWriterFileStreamInit2
	#define EXT_BTagWriterFileStreamInit2
	#define GET_BTagWriterFileStreamInit2(fl)  CAL_CMGETAPI( "BTagWriterFileStreamInit2" ) 
	#define CAL_BTagWriterFileStreamInit2  BTagWriterFileStreamInit2
	#define CHK_BTagWriterFileStreamInit2  TRUE
	#define EXP_BTagWriterFileStreamInit2  CAL_CMEXPAPI( "BTagWriterFileStreamInit2" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagWriterFileStreamInit2
	#define EXT_BTagWriterFileStreamInit2
	#define GET_BTagWriterFileStreamInit2(fl)  CAL_CMGETAPI( "BTagWriterFileStreamInit2" ) 
	#define CAL_BTagWriterFileStreamInit2  BTagWriterFileStreamInit2
	#define CHK_BTagWriterFileStreamInit2  TRUE
	#define EXP_BTagWriterFileStreamInit2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFileStreamInit2", (RTS_UINTPTR)BTagWriterFileStreamInit2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagWriterFileStreamInit2
	#define EXT_CmpBinTagUtilBTagWriterFileStreamInit2
	#define GET_CmpBinTagUtilBTagWriterFileStreamInit2  ERR_OK
	#define CAL_CmpBinTagUtilBTagWriterFileStreamInit2 pICmpBinTagUtil->IBTagWriterFileStreamInit2
	#define CHK_CmpBinTagUtilBTagWriterFileStreamInit2 (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagWriterFileStreamInit2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagWriterFileStreamInit2
	#define EXT_BTagWriterFileStreamInit2
	#define GET_BTagWriterFileStreamInit2(fl)  CAL_CMGETAPI( "BTagWriterFileStreamInit2" ) 
	#define CAL_BTagWriterFileStreamInit2 pICmpBinTagUtil->IBTagWriterFileStreamInit2
	#define CHK_BTagWriterFileStreamInit2 (pICmpBinTagUtil != NULL)
	#define EXP_BTagWriterFileStreamInit2  CAL_CMEXPAPI( "BTagWriterFileStreamInit2" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagWriterFileStreamInit2  PFBTAGWRITERFILESTREAMINIT2 pfBTagWriterFileStreamInit2;
	#define EXT_BTagWriterFileStreamInit2  extern PFBTAGWRITERFILESTREAMINIT2 pfBTagWriterFileStreamInit2;
	#define GET_BTagWriterFileStreamInit2(fl)  s_pfCMGetAPI2( "BTagWriterFileStreamInit2", (RTS_VOID_FCTPTR *)&pfBTagWriterFileStreamInit2, (fl), 0, 0)
	#define CAL_BTagWriterFileStreamInit2  pfBTagWriterFileStreamInit2
	#define CHK_BTagWriterFileStreamInit2  (pfBTagWriterFileStreamInit2 != NULL)
	#define EXP_BTagWriterFileStreamInit2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagWriterFileStreamInit2", (RTS_UINTPTR)BTagWriterFileStreamInit2, 0, 0) 
#endif



RTS_RESULT CDECL BTagReaderFileInit(BINTAGREADER *pReader, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize, RTS_HANDLE hFile);
typedef RTS_RESULT (CDECL * PFBTAGREADERFILEINIT) (BINTAGREADER *pReader, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize, RTS_HANDLE hFile);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERFILEINIT_NOTIMPLEMENTED)
	#define USE_BTagReaderFileInit
	#define EXT_BTagReaderFileInit
	#define GET_BTagReaderFileInit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderFileInit(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderFileInit  FALSE
	#define EXP_BTagReaderFileInit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderFileInit
	#define EXT_BTagReaderFileInit
	#define GET_BTagReaderFileInit(fl)  CAL_CMGETAPI( "BTagReaderFileInit" ) 
	#define CAL_BTagReaderFileInit  BTagReaderFileInit
	#define CHK_BTagReaderFileInit  TRUE
	#define EXP_BTagReaderFileInit  CAL_CMEXPAPI( "BTagReaderFileInit" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderFileInit
	#define EXT_BTagReaderFileInit
	#define GET_BTagReaderFileInit(fl)  CAL_CMGETAPI( "BTagReaderFileInit" ) 
	#define CAL_BTagReaderFileInit  BTagReaderFileInit
	#define CHK_BTagReaderFileInit  TRUE
	#define EXP_BTagReaderFileInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderFileInit", (RTS_UINTPTR)BTagReaderFileInit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderFileInit
	#define EXT_CmpBinTagUtilBTagReaderFileInit
	#define GET_CmpBinTagUtilBTagReaderFileInit  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderFileInit pICmpBinTagUtil->IBTagReaderFileInit
	#define CHK_CmpBinTagUtilBTagReaderFileInit (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderFileInit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderFileInit
	#define EXT_BTagReaderFileInit
	#define GET_BTagReaderFileInit(fl)  CAL_CMGETAPI( "BTagReaderFileInit" ) 
	#define CAL_BTagReaderFileInit pICmpBinTagUtil->IBTagReaderFileInit
	#define CHK_BTagReaderFileInit (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderFileInit  CAL_CMEXPAPI( "BTagReaderFileInit" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderFileInit  PFBTAGREADERFILEINIT pfBTagReaderFileInit;
	#define EXT_BTagReaderFileInit  extern PFBTAGREADERFILEINIT pfBTagReaderFileInit;
	#define GET_BTagReaderFileInit(fl)  s_pfCMGetAPI2( "BTagReaderFileInit", (RTS_VOID_FCTPTR *)&pfBTagReaderFileInit, (fl), 0, 0)
	#define CAL_BTagReaderFileInit  pfBTagReaderFileInit
	#define CHK_BTagReaderFileInit  (pfBTagReaderFileInit != NULL)
	#define EXP_BTagReaderFileInit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderFileInit", (RTS_UINTPTR)BTagReaderFileInit, 0, 0) 
#endif




/**
 * <description>Get the first tag out of a stream</description>
 * <param name="pReader" type="IN">Pointer to the reader stream</param>
 * <param name="pulToplevelTag" type="OUT">Returns the toplevel tag. -1, if no complex tag</param>
 * <param name="pulTag" type="OUT">Returns the tag</param>
 * <param name="pulSize" type="OUT">Size of the tag</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Pointer to the content</result>
 */
void* CDECL BTagReaderGetFirstTag(BINTAGREADER *pReader, RTS_UI32 *pulToplevelTag, RTS_UI32 *pulTag, RTS_UI32 *pulSize, RTS_RESULT *pResult);
typedef void* (CDECL * PFBTAGREADERGETFIRSTTAG) (BINTAGREADER *pReader, RTS_UI32 *pulToplevelTag, RTS_UI32 *pulTag, RTS_UI32 *pulSize, RTS_RESULT *pResult);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERGETFIRSTTAG_NOTIMPLEMENTED)
	#define USE_BTagReaderGetFirstTag
	#define EXT_BTagReaderGetFirstTag
	#define GET_BTagReaderGetFirstTag(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderGetFirstTag(p0,p1,p2,p3,p4)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderGetFirstTag  FALSE
	#define EXP_BTagReaderGetFirstTag  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderGetFirstTag
	#define EXT_BTagReaderGetFirstTag
	#define GET_BTagReaderGetFirstTag(fl)  CAL_CMGETAPI( "BTagReaderGetFirstTag" ) 
	#define CAL_BTagReaderGetFirstTag  BTagReaderGetFirstTag
	#define CHK_BTagReaderGetFirstTag  TRUE
	#define EXP_BTagReaderGetFirstTag  CAL_CMEXPAPI( "BTagReaderGetFirstTag" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderGetFirstTag
	#define EXT_BTagReaderGetFirstTag
	#define GET_BTagReaderGetFirstTag(fl)  CAL_CMGETAPI( "BTagReaderGetFirstTag" ) 
	#define CAL_BTagReaderGetFirstTag  BTagReaderGetFirstTag
	#define CHK_BTagReaderGetFirstTag  TRUE
	#define EXP_BTagReaderGetFirstTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetFirstTag", (RTS_UINTPTR)BTagReaderGetFirstTag, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderGetFirstTag
	#define EXT_CmpBinTagUtilBTagReaderGetFirstTag
	#define GET_CmpBinTagUtilBTagReaderGetFirstTag  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderGetFirstTag pICmpBinTagUtil->IBTagReaderGetFirstTag
	#define CHK_CmpBinTagUtilBTagReaderGetFirstTag (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderGetFirstTag  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderGetFirstTag
	#define EXT_BTagReaderGetFirstTag
	#define GET_BTagReaderGetFirstTag(fl)  CAL_CMGETAPI( "BTagReaderGetFirstTag" ) 
	#define CAL_BTagReaderGetFirstTag pICmpBinTagUtil->IBTagReaderGetFirstTag
	#define CHK_BTagReaderGetFirstTag (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderGetFirstTag  CAL_CMEXPAPI( "BTagReaderGetFirstTag" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderGetFirstTag  PFBTAGREADERGETFIRSTTAG pfBTagReaderGetFirstTag;
	#define EXT_BTagReaderGetFirstTag  extern PFBTAGREADERGETFIRSTTAG pfBTagReaderGetFirstTag;
	#define GET_BTagReaderGetFirstTag(fl)  s_pfCMGetAPI2( "BTagReaderGetFirstTag", (RTS_VOID_FCTPTR *)&pfBTagReaderGetFirstTag, (fl), 0, 0)
	#define CAL_BTagReaderGetFirstTag  pfBTagReaderGetFirstTag
	#define CHK_BTagReaderGetFirstTag  (pfBTagReaderGetFirstTag != NULL)
	#define EXP_BTagReaderGetFirstTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetFirstTag", (RTS_UINTPTR)BTagReaderGetFirstTag, 0, 0) 
#endif




/**
 * <description>Get the first tag out of a stream</description>
 * <param name="pReader" type="IN">Pointer to the reader stream</param>
 * <param name="pulToplevelTag" type="OUT">Returns the toplevel tag. -1, if no complex tag</param>
 * <param name="pulTag" type="OUT">Returns the tag</param>
 * <param name="pulSize" type="OUT">Size of the tag</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Pointer to the content</result>
 */
void* CDECL BTagReaderGetNextTag(BINTAGREADER *pReader, RTS_UI32 *pulToplevelTag, RTS_UI32 *pulTag, RTS_UI32 *pulSize, RTS_RESULT *pResult);
typedef void* (CDECL * PFBTAGREADERGETNEXTTAG) (BINTAGREADER *pReader, RTS_UI32 *pulToplevelTag, RTS_UI32 *pulTag, RTS_UI32 *pulSize, RTS_RESULT *pResult);
#if defined(CMPBINTAGUTIL_NOTIMPLEMENTED) || defined(BTAGREADERGETNEXTTAG_NOTIMPLEMENTED)
	#define USE_BTagReaderGetNextTag
	#define EXT_BTagReaderGetNextTag
	#define GET_BTagReaderGetNextTag(fl)  ERR_NOTIMPLEMENTED
	#define CAL_BTagReaderGetNextTag(p0,p1,p2,p3,p4)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_BTagReaderGetNextTag  FALSE
	#define EXP_BTagReaderGetNextTag  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_BTagReaderGetNextTag
	#define EXT_BTagReaderGetNextTag
	#define GET_BTagReaderGetNextTag(fl)  CAL_CMGETAPI( "BTagReaderGetNextTag" ) 
	#define CAL_BTagReaderGetNextTag  BTagReaderGetNextTag
	#define CHK_BTagReaderGetNextTag  TRUE
	#define EXP_BTagReaderGetNextTag  CAL_CMEXPAPI( "BTagReaderGetNextTag" ) 
#elif defined(MIXED_LINK) && !defined(CMPBINTAGUTIL_EXTERNAL)
	#define USE_BTagReaderGetNextTag
	#define EXT_BTagReaderGetNextTag
	#define GET_BTagReaderGetNextTag(fl)  CAL_CMGETAPI( "BTagReaderGetNextTag" ) 
	#define CAL_BTagReaderGetNextTag  BTagReaderGetNextTag
	#define CHK_BTagReaderGetNextTag  TRUE
	#define EXP_BTagReaderGetNextTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetNextTag", (RTS_UINTPTR)BTagReaderGetNextTag, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpBinTagUtilBTagReaderGetNextTag
	#define EXT_CmpBinTagUtilBTagReaderGetNextTag
	#define GET_CmpBinTagUtilBTagReaderGetNextTag  ERR_OK
	#define CAL_CmpBinTagUtilBTagReaderGetNextTag pICmpBinTagUtil->IBTagReaderGetNextTag
	#define CHK_CmpBinTagUtilBTagReaderGetNextTag (pICmpBinTagUtil != NULL)
	#define EXP_CmpBinTagUtilBTagReaderGetNextTag  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_BTagReaderGetNextTag
	#define EXT_BTagReaderGetNextTag
	#define GET_BTagReaderGetNextTag(fl)  CAL_CMGETAPI( "BTagReaderGetNextTag" ) 
	#define CAL_BTagReaderGetNextTag pICmpBinTagUtil->IBTagReaderGetNextTag
	#define CHK_BTagReaderGetNextTag (pICmpBinTagUtil != NULL)
	#define EXP_BTagReaderGetNextTag  CAL_CMEXPAPI( "BTagReaderGetNextTag" ) 
#else /* DYNAMIC_LINK */
	#define USE_BTagReaderGetNextTag  PFBTAGREADERGETNEXTTAG pfBTagReaderGetNextTag;
	#define EXT_BTagReaderGetNextTag  extern PFBTAGREADERGETNEXTTAG pfBTagReaderGetNextTag;
	#define GET_BTagReaderGetNextTag(fl)  s_pfCMGetAPI2( "BTagReaderGetNextTag", (RTS_VOID_FCTPTR *)&pfBTagReaderGetNextTag, (fl), 0, 0)
	#define CAL_BTagReaderGetNextTag  pfBTagReaderGetNextTag
	#define CHK_BTagReaderGetNextTag  (pfBTagReaderGetNextTag != NULL)
	#define EXP_BTagReaderGetNextTag  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"BTagReaderGetNextTag", (RTS_UINTPTR)BTagReaderGetNextTag, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif




typedef struct
{
	IBase_C *pBase;
	PFBTAGSWAPHEADER IBTagSwapHeader;
 	PFBTAGWRITERINIT2 IBTagWriterInit2;
 	PFBTAGWRITERINIT IBTagWriterInit;
 	PFBTAGWRITERSTARTSERVICE IBTagWriterStartService;
 	PFBTAGWRITERFINISHSERVICE IBTagWriterFinishService;
 	PFBTAGWRITERSTARTTAG IBTagWriterStartTag;
 	PFBTAGWRITERAPPENDSTRING IBTagWriterAppendString;
 	PFBTAGWRITERAPPENDWSTRING IBTagWriterAppendWString;
 	PFBTAGWRITERAPPENDBLOB IBTagWriterAppendBlob;
 	PFBTAGWRITERAPPENDRAW IBTagWriterAppendRaw;
 	PFBTAGWRITERAPPENDFILLBYTES IBTagWriterAppendFillBytes;
 	PFBTAGWRITERAPPENDDUMMYBYTES IBTagWriterAppendDummyBytes;
 	PFBTAGWRITERENDTAG IBTagWriterEndTag;
 	PFBTAGWRITERSWITCHBUFFER IBTagWriterSwitchBuffer;
 	PFBTAGWRITERCREATESAVEPOINT IBTagWriterCreateSavepoint;
 	PFBTAGWRITERRESTORESAVEPOINT IBTagWriterRestoreSavepoint;
 	PFBTAGWRITERGETAVAILABLEBUFFER IBTagWriterGetAvailableBuffer;
 	PFBTAGWRITERFINISH IBTagWriterFinish;
 	PFBTAGWRITESINGLETAG IBTagWriteSingleTag;
 	PFBTAGWRITESINGLETAG2 IBTagWriteSingleTag2;
 	PFBTAGREADERINIT IBTagReaderInit;
 	PFBTAGREADERPEEKNEXT IBTagReaderPeekNext;
 	PFBTAGREADERMOVENEXT IBTagReaderMoveNext;
 	PFBTAGREADERSKIPCONTENT IBTagReaderSkipContent;
 	PFBTAGREADERGETTAGID IBTagReaderGetTagId;
 	PFBTAGREADERGETTAGLEN IBTagReaderGetTagLen;
 	PFBTAGREADERISDATATAG IBTagReaderIsDataTag;
 	PFBTAGREADERGETCOMPLEXCONTENT IBTagReaderGetComplexContent;
 	PFBTAGREADERGETCONTENT IBTagReaderGetContent;
 	PFBTAGREADERGETSTRING IBTagReaderGetString;
 	PFBTAGREADERCREATESAVEPOINT IBTagReaderCreateSavepoint;
 	PFBTAGREADERRESTORESAVEPOINT IBTagReaderRestoreSavepoint;
 	PFBTAGWRITERFILEINIT IBTagWriterFileInit;
 	PFBTAGWRITERFILEINIT2 IBTagWriterFileInit2;
 	PFBTAGWRITERFILESTREAMINIT IBTagWriterFileStreamInit;
 	PFBTAGWRITERFILESTREAMINIT2 IBTagWriterFileStreamInit2;
 	PFBTAGREADERFILEINIT IBTagReaderFileInit;
 	PFBTAGREADERGETFIRSTTAG IBTagReaderGetFirstTag;
 	PFBTAGREADERGETNEXTTAG IBTagReaderGetNextTag;
 } ICmpBinTagUtil_C;

#ifdef CPLUSPLUS
class ICmpBinTagUtil : public IBase
{
	public:
		virtual RTS_RESULT CDECL IBTagSwapHeader(HEADER_TAG_EXT* pHeader, RTS_I32 bSwap) =0;
		virtual RTS_RESULT CDECL IBTagWriterInit2(BINTAGWRITER *pWriter, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize, RTS_I32 bSwapHeader) =0;
		virtual RTS_RESULT CDECL IBTagWriterInit(BINTAGWRITER *pWriter, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize) =0;
		virtual RTS_RESULT CDECL IBTagWriterStartService(BINTAGWRITER *pWriter, RTS_UI32 ulSessionID, RTS_UI16 usHeaderTag, RTS_UI32 ulServiceGroup, RTS_UI16 usService) =0;
		virtual RTS_RESULT CDECL IBTagWriterFinishService(BINTAGWRITER *pWriter, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize) =0;
		virtual RTS_RESULT CDECL IBTagWriterStartTag(BINTAGWRITER *pWriter, RTS_UI32 ulTagId, BTAG_ALIGNMENT contentAlignment, RTS_UI32 ulMinLengthSize) =0;
		virtual RTS_RESULT CDECL IBTagWriterAppendString(BINTAGWRITER *pWriter, const char *pszString) =0;
		virtual RTS_RESULT CDECL IBTagWriterAppendWString(BINTAGWRITER *pWriter, const RTS_WCHAR *wszString) =0;
		virtual RTS_RESULT CDECL IBTagWriterAppendBlob(BINTAGWRITER *pWriter, const RTS_UI8 *pBlob, RTS_UI32 ulSize) =0;
		virtual RTS_RESULT CDECL IBTagWriterAppendRaw(BINTAGWRITER *pWriter, RTS_UI8 **ppBuffer, RTS_UI32 ulSize) =0;
		virtual RTS_RESULT CDECL IBTagWriterAppendFillBytes(BINTAGWRITER *pWriter, RTS_UI8 byFillByte, BTAG_ALIGNMENT alignment) =0;
		virtual RTS_RESULT CDECL IBTagWriterAppendDummyBytes(BINTAGWRITER *pWriter, RTS_UI8 byFillByte, RTS_UI32 ulSize) =0;
		virtual RTS_RESULT CDECL IBTagWriterEndTag(BINTAGWRITER *pWriter, RTS_UI32 ulTagId) =0;
		virtual RTS_RESULT CDECL IBTagWriterSwitchBuffer(BINTAGWRITER *pWriter, RTS_UI8 *pNewBuffer, RTS_UI32 ulNewSize, RTS_UI8 **ppOldBuffer) =0;
		virtual RTS_RESULT CDECL IBTagWriterCreateSavepoint(BINTAGWRITER *pWriter, BINTAGSAVEPOINT *pSavepoint) =0;
		virtual RTS_RESULT CDECL IBTagWriterRestoreSavepoint(BINTAGWRITER *pWriter, BINTAGSAVEPOINT *pSavepoint) =0;
		virtual RTS_UI32 CDECL IBTagWriterGetAvailableBuffer(BINTAGWRITER *pWriter) =0;
		virtual RTS_RESULT CDECL IBTagWriterFinish(BINTAGWRITER *pWriter, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize) =0;
		virtual RTS_RESULT CDECL IBTagWriteSingleTag(BINTAGWRITER *pWriter, RTS_UI32 ulTag, BTAG_ALIGNMENT align, RTS_I32 bFillBytes, void *pContent, RTS_UI32 ulSize) =0;
		virtual RTS_RESULT CDECL IBTagWriteSingleTag2(BINTAGWRITER *pWriter, RTS_UI32 ulTag, BTAG_ALIGNMENT align, void **ppContentList, RTS_UI32 *paulSize) =0;
		virtual RTS_RESULT CDECL IBTagReaderInit(BINTAGREADER *pReader, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize) =0;
		virtual RTS_RESULT CDECL IBTagReaderPeekNext(BINTAGREADER *pReader) =0;
		virtual RTS_RESULT CDECL IBTagReaderMoveNext(BINTAGREADER *pReader, RTS_I32 *pnElementType) =0;
		virtual RTS_RESULT CDECL IBTagReaderSkipContent(BINTAGREADER *pReader) =0;
		virtual RTS_RESULT CDECL IBTagReaderGetTagId(BINTAGREADER *pReader, RTS_UI32 *pulTagId) =0;
		virtual RTS_RESULT CDECL IBTagReaderGetTagLen(BINTAGREADER *pReader, RTS_UI32 *pulTagLen) =0;
		virtual RTS_RESULT CDECL IBTagReaderIsDataTag(BINTAGREADER *pReader, RTS_I32 *pbIsDataTag) =0;
		virtual RTS_RESULT CDECL IBTagReaderGetComplexContent(BINTAGREADER *pReader, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize) =0;
		virtual RTS_RESULT CDECL IBTagReaderGetContent(BINTAGREADER *pReader, RTS_UI8 **ppBuffer, RTS_UI32 *pulSize) =0;
		virtual RTS_RESULT CDECL IBTagReaderGetString(BINTAGREADER *pReader, char **ppString, RTS_UI32 *pulSize, RTS_I32 bAddEndOfString) =0;
		virtual RTS_RESULT CDECL IBTagReaderCreateSavepoint(BINTAGREADER *pReader, BINTAGREADERSAVEPOINT *pSavepoint) =0;
		virtual RTS_RESULT CDECL IBTagReaderRestoreSavepoint(BINTAGREADER *pReader, BINTAGREADERSAVEPOINT *pSavepoint) =0;
		virtual RTS_RESULT CDECL IBTagWriterFileInit(BINTAGWRITER *pWriter, RTS_HANDLE hFile) =0;
		virtual RTS_RESULT CDECL IBTagWriterFileInit2(BINTAGWRITER *pWriter, char *pszFileName) =0;
		virtual RTS_RESULT CDECL IBTagWriterFileStreamInit(BINTAGWRITER *pWriter, RTS_HANDLE hFile) =0;
		virtual RTS_RESULT CDECL IBTagWriterFileStreamInit2(BINTAGWRITER *pWriter, RTS_HANDLE hFile, PFUPDATECRC pfUpdateCRC, void *pParameterUpdateCRC) =0;
		virtual RTS_RESULT CDECL IBTagReaderFileInit(BINTAGREADER *pReader, RTS_UI8 *pBuffer, RTS_UI32 ulBufferSize, RTS_HANDLE hFile) =0;
		virtual void* CDECL IBTagReaderGetFirstTag(BINTAGREADER *pReader, RTS_UI32 *pulToplevelTag, RTS_UI32 *pulTag, RTS_UI32 *pulSize, RTS_RESULT *pResult) =0;
		virtual void* CDECL IBTagReaderGetNextTag(BINTAGREADER *pReader, RTS_UI32 *pulToplevelTag, RTS_UI32 *pulTag, RTS_UI32 *pulSize, RTS_RESULT *pResult) =0;
};
	#ifndef ITF_CmpBinTagUtil
		#define ITF_CmpBinTagUtil static ICmpBinTagUtil *pICmpBinTagUtil = NULL;
	#endif
	#define EXTITF_CmpBinTagUtil
#else	/*CPLUSPLUS*/
	typedef ICmpBinTagUtil_C		ICmpBinTagUtil;
	#ifndef ITF_CmpBinTagUtil
		#define ITF_CmpBinTagUtil
	#endif
	#define EXTITF_CmpBinTagUtil
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPBINTAGUTILITF_H_*/
