 /**
 * <interfacename>CmpMemPool</interfacename>
 * <description>
 *  <p>Interface of the memory pool manager to handle static and dynamic memory blocks</p>
 *
 *  <p>A MemPool has the following structure:</p>
 *  <pre>
 *  
 *  -------------- Pool Control Block                -------------- Block Control Block
 *  |nRefCount   |  Reference Counter                |pBCB        |  Pointer to next BCB
 *  |Blocksize   |  data size of single block        |pPCB        |  Pointer to PCB
 *  |pBlocksInUse|  List of used blocks              ------------
 *  |pBCB        |  List of free blocks              
 *  |...         |
 *  -------------
 * 
 *                       provided
 *                       Memory Area
 *                       .--------------------------.
 *                       | PCB                      |
 *    List of free Blocks|  Blocksize               |      List of used Blocks
 *                       |  pBlocksInUse            |===========.
 *          .=========== |  pBCB                    |           |
 *          |            |......................... |           |
 *          |            | BCB                      | &lt;=========
 *          |            |                          | ---.
 *          |            |......................... |    |
 *          |            | DATA                     |    |
 *          |            |                          |    |
 *          |            |......................... |    |
 *          |            | BCB                      | &lt;--'
 *          |            |                          | ---.
 *          |            |......................... |    |
 *          |            | DATA                     |    |
 *          |            |                          |    |
 *          |            |......................... |    |
 *          |            |                          | &lt;--'
 *          |           ~~~                        ~~~
 *          |
 *          |           ~~~                        ~~~
 *          '=========&gt;  |......................... |
 *                       | BCB                      | ---.
 *                       |                          |    |
 *                       |......................... |    |
 *                       | DATA                     |    |
 *                       |                          |    |
 *                       |......................... | &lt;--'
 *                       | BCB                      | ---.
 *                       |                          |    |
 *                      ~~~                        ~~   ~~~
 * 
 *                      ~~~                        ~~~
 *                       |                          |
 *                       |                          |
 *                       .--------------------------.
 *                       End of Memory Area
 * 
 * 
 * </pre>
 * 
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMPMEMPOOLITF_H_
#define _CMPMEMPOOLITF_H_

#include "CmpStd.h"

 

 




/**
 * <category>Compiler switch</category>
 * <description>
 *	Compiler switches configure the behavior of the memory pool.
 * </description>
 * <element name="MEMPOOL_DISABLE_HEAP_MEMORY">Switch to disable dynamic memory</element>
 * <element name="MEMPOOL_8BYTE_ALIGNED">Force 8 byte alignment of blocks. Note, that this doesn't affect the alignment of the header.</element>
 */

#ifdef RTS_MEMPOOL_VER2
	#error RTS_MEMPOOL_VER2 no longer supported!
#endif


#if !defined(MEMPOOL_8BYTE_ALIGNED) && !defined(RTS_DISABLE_64BIT_SUPPORT)
	#define MEMPOOL_8BYTE_ALIGNED
#endif

#ifndef MEMPOOL_HEADER_ALIGNMENT
	#define MEMPOOL_HEADER_ALIGNMENT 4
#endif
#ifdef MEMPOOL_8BYTE_ALIGNED
	#define MEMPOOL_BLOCK_ALIGNMENT  8
#else
	#define MEMPOOL_BLOCK_ALIGNMENT			sizeof(void*)
#endif

/* Init value to write over a deleted object */
#ifndef MEMPOOL_INIT_VALUE
	#define MEMPOOL_INIT_VALUE		0xAA
#endif


/**
 * <category>Macros</category>
 * <description>Macros to handle static size of a MemPool</description>
 * <element name="MEM_GET_STATIC_LEN" type="IN">Can be use to calculate the gross size of a static MemPool by specifying the structure of one element.
 *  Usage:
 *		#define NUM_OF_STATIC_ELEMENTS		10
 *		typedef struct
 *		{
 *			...
 *		} StructNameOfOneElement;
 *		static RTS_UI8 s_StaticMemPool[MEM_GET_STATIC_LEN(NUM_OF_STATIC_ELEMENTS, StructNameOfOneElement)];
 * </element>
 * <element name="MEM_GET_STATIC_LEN_" type="IN">Can be use to calculate the gross size of a static MemPool by specifying the size of one element.
 *  Usage:
 *		#define NUM_OF_STATIC_ELEMENTS		10
 *		typedef struct
 *		{
 *			...
 *		} StructNameOfOneElement;
 *		static RTS_UI8 s_StaticMemPool[MEM_GET_STATIC_LEN_(NUM_OF_STATIC_ELEMENTS, sizeof(StructNameOfOneElement))];
 * </element>
 */
#define MEM_GET_STATIC_LEN(Num, Struct)				(sizeof(RTS_PCB) + 2 * MEMPOOL_BLOCK_ALIGNMENT + (Num) * ALIGN_SIZE(sizeof(Struct), MEMPOOL_BLOCK_ALIGNMENT) + (Num) * sizeof(RTS_BCB))
#define MEM_GET_STATIC_LEN_(Num, Size)				(sizeof(RTS_PCB) + 2 * MEMPOOL_BLOCK_ALIGNMENT + (Num) * ALIGN_SIZE(Size, MEMPOOL_BLOCK_ALIGNMENT) + (Num) * sizeof(RTS_BCB))
#define MEM_GET_STATIC_LEN2(Num, Size, HeaderType)	(sizeof(HeaderType) + 2 * MEMPOOL_BLOCK_ALIGNMENT + (Num) * ALIGN_SIZE(Size, MEMPOOL_BLOCK_ALIGNMENT) + (Num) * sizeof(RTS_BCB))

/**
 * <category>Static defines</category>
 * <description>Flags for a single MemPool block</description>
 * <element name="MEM_POOL_FLAGS_BLOCK_NONE" type="IN">No flag specified</element>
 * <element name="MEM_POOL_FLAGS_BLOCK_DELETE" type="IN">Block is marked to delete at the next possible moment, if nobody is using the MemPool!</element>
 */
#define MEM_POOL_FLAGS_BLOCK_NONE			0x00000000
#define MEM_POOL_FLAGS_BLOCK_DELETE			0x00000001

#define MEM_POOL_FLAGS_BLOCK_IS_DELETE(pBCB) ((pBCB->flags & MEM_POOL_FLAGS_BLOCK_DELETE) != 0)

/*
 * <SIL2/>
 * <description>Block Control Block</description>
 */
typedef struct RTS_BCBtag
{
	struct RTS_BCBtag *pBCB;	/* Pointer to next free or used block control block */
	struct RTS_PCBtag *pPCB;	/* Pointer to pool control block */
	RTS_UI32 flags;
#if defined(MEMPOOL_8BYTE_ALIGNED)
	RTS_UI32 alignment;
#endif
} RTS_BCB;

/**
 * <category>MemPool types</category>
 * <description>Type of a MemPool</description>
 */

/**
 * <category>MemPool options</category>
 * <description>MemPool options</description>
 * <element name="MEM_POOL_OPTION_STATIC" type="IN">MemPool based on a static array</element>
 * <element name="MEM_POOL_OPTION_DYNAMIC" type="IN">MemPool based on a dynamic allocated heap block</element>
 * <element name="MEM_POOL_OPTION_NOSYNC" type="IN">No synchronization necessary within the MemPool</element>
 * <element name="MEM_POOL_OPTION_SYNC_ON_REQ" type="IN">To save resources, semaphore created only if synchronization is needed</element>
 * <element name="MEM_POOL_OPTION_UNALIGNED" type="IN">Marks the MemPool as unaligned, if the beginning of the aligned pPool is not identical to the pHeapMemory pointer!</element>
 */
#define MEM_POOL_OPTION_STATIC				0x00000001
#define MEM_POOL_OPTION_DYNAMIC				0x00000002
#define MEM_POOL_OPTION_NOSYNC				0x00000010		
#define MEM_POOL_OPTION_SYNC_ON_REQ			0x00000020
#define MEM_POOL_OPTION_UNALIGNED			0x00000100


/**
 * The following macros can be used with a pointer to a PEB, PCB or MemPoolConfiguraion structure.
 * They all have the same ->options member, on which we are operating.
 */
#define MEM_POOL_OPTION_IS_STATIC(pConfiguration)       ((pConfiguration->options & MEM_POOL_OPTION_STATIC) != 0)
#define MEM_POOL_OPTION_IS_DYNAMIC(pConfiguration)      ((pConfiguration->options & MEM_POOL_OPTION_DYNAMIC) != 0)
#define MEM_POOL_OPTION_IS_NOSYNC(pConfiguration)       ((pConfiguration->options & MEM_POOL_OPTION_NOSYNC) != 0)
#define MEM_POOL_OPTION_IS_SYNC_ON_REQ(pConfiguration)  ((pConfiguration->options & MEM_POOL_OPTION_SYNC_ON_REQ) != 0)

#define MEM_POOL_OPTION_SET_STATIC(pConfiguration)       do { pConfiguration->options |= MEM_POOL_OPTION_STATIC;  } while(0)
#define MEM_POOL_OPTION_SET_DYNAMIC(pConfiguration)      do { pConfiguration->options |= MEM_POOL_OPTION_DYNAMIC; } while(0)
#define MEM_POOL_OPTION_SET_NOSYNC(pConfiguration)       do { pConfiguration->options |= MEM_POOL_OPTION_NOSYNC;  } while(0)
#define MEM_POOL_OPTION_SET_SYNC_ON_REQ(pConfiguration)  do { pConfiguration->options |= MEM_POOL_OPTION_SYNC_ON_REQ;  } while(0)

#define MEM_POOL_OPTION_RESET_STATIC(pConfiguration)       do { pConfiguration->options &= ~MEM_POOL_OPTION_STATIC;  } while(0)
#define MEM_POOL_OPTION_RESET_DYNAMIC(pConfiguration)      do { pConfiguration->options &= ~MEM_POOL_OPTION_DYNAMIC; } while(0)
#define MEM_POOL_OPTION_RESET_NOSYNC(pConfiguration)       do { pConfiguration->options &= ~MEM_POOL_OPTION_NOSYNC;  } while(0)
#define MEM_POOL_OPTION_RESET_SYNC_ON_REQ(pConfiguration)  do { pConfiguration->options &= ~MEM_POOL_OPTION_SYNC_ON_REQ;  } while(0)

/**
 * <SIL2/> 
 * <category>Extension block</category>
 * <description>Pool Extension Block</description>
 * <element name="options" type="IN">Flags for internal usage of the MemPool. See the category "MemPool options" for detailed information</element>
 * <element name="pPEB" type="IN">Pointer to the extension block (heap/dynamic) or NULL</element>
 */
typedef struct RTS_PEBtag
{
	RTS_UI32 options;
#if defined(MEMPOOL_8BYTE_ALIGNED) && defined(TRG_64BIT)
	RTS_UI32 alignment;
#endif
	struct RTS_PEBtag *pPEB;
} RTS_PEB;

/**
 * <category>Static defines</category>
 * <description>Delete function pointer definition which is called optional to delete a MemPool block. Can be specified in MemPoolCreate().</description>
 * <param name="pBlock" type="IN">Pointer to the MemPool block</param>
 * <param name="pParam" type="IN">Pointer to a parameter which can be specified in MemPoolPutBlock_LF() as second parameter!</param>
 */
typedef void (CDECL *PF_MEMBLOCK_DELETEFUNCTION)(void *pBlock, void *pParam);

/**
 * <SIL2/>
 * <category>MemPool definition</category>
 * <description>Pool Control Block. This is the header within a MemPool to manage it.
 *	ATTENTION:
 *	- This structure members are managed by the CmpMemPool and so don't modify these entries directly!!!!!
 * </description>
 * <element name="ulBlockSize" type="IN">Block size of each pool block</element>
 * <element name="pfDeleteFunction" type="IN">Delete function pointer which is called optional to delete a MemPool block</element>
 * <element name="nRefCount" type="IN">Reference counter which is incremented with MemPoolAddRef_LF() and is decremented with MemPoolReleaseRef_LF() to prevent, that the chain of used
 *	blocks is not changed during iteration over all elements.
 * </element>
 * <element name="options" type="IN">Options of the MemPool. See the category "MemPool options" for detailed information</element>
 * <element name="lock" type="IN">Is used to hold the specified locking primitive resource</element>
 * <element name="pPEB" type="IN">Pointer to next extension block</element>
 * <element name="pBCB" type="IN">Pointer to free block list</element>
 * <element name="pBlocksInUse" type="IN">Pointer to first block in use resp. the linked chain of used blocks!</element>
 * <element name="pTail" type="IN">Pointer to tail block in chain of used blocks!</element>
 */
typedef struct RTS_PCBtag
{
	RTS_SIZE ulBlockSize;
	PF_MEMBLOCK_DELETEFUNCTION pfDeleteFunction;
	RTS_I32 nRefCount;
	RTS_UI32 options;
	
	union
	{
		RTS_HANDLE	hSem;
		RTS_UINTPTR	mask;
	} lock;

	struct RTS_PEBtag *pPEB;
	struct RTS_BCBtag *pBCB;
	struct RTS_BCBtag *pBlocksInUse;
	struct RTS_BCBtag *pTail;

#if defined(MEMPOOL_8BYTE_ALIGNED) && !defined(TRG_64BIT)
	RTS_UI32 alignment;
#endif
} RTS_PCB;

/**
 * <SIL2/>
 * <category>MemPool iterator</category>
 * <description>Object to iterate of used blocks of a MemPool.
 *	ATTENTION:
 *	- This structure members are managed by the CmpMemPool and so don't modify these entries directly!!!!!
 * </description>
 * <element name="pHead" type="IN">Points to the head element of the chain list</element>
 * <element name="pNext" type="IN">Points to the next element in the chain list</element>
 */
typedef struct
{
	RTS_BCB *pHead;
	RTS_BCB *pNext;
} MemIterator;


/**
 * <category>Static defines</category>
 * <description>Macros to check MemPool contents</description>
 * <element name="MEMPOOL_IS_NOT_EMPTY" type="IN">Macro to check if a MemPool is _not_ empty</element>
 * <element name="MEMPOOL_IS_EMPTY" type="IN">Macro to check if a MemPool is empty</element>
 * <element name="MEMPOOL_HAS_FREE_BLOCKS" type="IN">Macro to check if a MemPool has free block that can be used</element>
 */
#define MEMPOOL_IS_NOT_EMPTY(pPool)		((((RTS_PCB *)pPool)->pBlocksInUse) != NULL)
#define MEMPOOL_IS_EMPTY(pPool)			((((RTS_PCB *)pPool)->pBlocksInUse) == NULL)
#define MEMPOOL_HAS_FREE_BLOCKS(pPool)	(((RTS_PCB *)pPool)->pPEB) != NULL || ((RTS_PCB *)pPool)->pBCB) != NULL)

/**
 * <category>Static defines</category>
 * <description>Macros to iterate over a MemPool</description>
 * <element name="MEM_GET_FIRST" type="IN">Get first block element of a MemPool</element>
 * <element name="MEM_GET_NEXT" type="IN">Get next block element of a MemPool</element>
 * <element name="MEM_GET_DATA" type="IN">Get data pointer of a MemPool block _including_ blocks which are still marked as deleted!</element>
 * <element name="MEM_GET_DATA_SKIP_DELETE" type="IN">Get data pointer of a MemPool block _without_ a block marked as "to delete"!
 *		A block is marked as "to delete", if MemPoolRemoveUsedBlock() was called during somebody else iterated over the pool. And so the block is only marked as "to delete" and should be deleted in the next 
 *		suitable moment.
 *
 *		NOTE:
 *		For blocks which are still marked as deleted, you get a NULL pointer here!</element>
 */
#define MEM_GET_BCB_FROM_BLOCK(pBlock)		((RTS_BCB*)((RTS_UI8*)pBlock - sizeof(RTS_BCB)))
#define MEM_GET_BLOCK_FROM_BCB(pBCB)		(void *)(((RTS_BCB*)pBCB) + 1)

#ifdef MEMPOOL_FUNCTIONS
	static int MEM_GET_FIRST(RTS_HANDLE hPool, MemIterator *pIt)
	{
		RTS_PCB *pPool = (RTS_PCB *)hPool;
		
		pIt->pNext = NULL;
		pIt->pHead = NULL;

		if (hPool != RTS_INVALID_HANDLE && pPool->pBlocksInUse != NULL)
		{
			pIt->pHead = pPool->pBlocksInUse;	
			return 1;
		}
		return 0;
	}
	static int MEM_GET_NEXT(MemIterator *pIt)
	{
		pIt->pNext = (NULL != pIt->pNext) ? pIt->pNext->pBCB : pIt->pHead;

		return pIt->pNext != NULL;
	}
	static void *MEM_GET_DATA(MemIterator *pIt)
	{
		return (pIt->pNext + 1);
	}	
	static void *MEM_GET_DATA_SKIP_DELETE(MemIterator *pIt)
	{
		if (pIt->pNext->flags & MEM_POOL_FLAGS_BLOCK_DELETE)
			return NULL;
		return (pIt->pNext + 1);
	}	
	static void *MEM_GET_NEXT_FROM_DATA(void *pData)
	{
		RTS_BCB *pBCB = ((RTS_BCB*)((unsigned char *)pData - sizeof(RTS_BCB)))->pBCB;
		
		return (NULL != pBCB) ? pBCB + 1 : NULL;
	}
#else
	#define __MEM_GET_ITERATOR_NEXT(pIt)	((MemIterator*)pIt)->pNext
	#define __MEM_GET_ITERATOR_HEAD(pIt)	((MemIterator*)pIt)->pHead

	#define MEM_IS_BLOCK_DELETED(pBlock)	((MEM_GET_BCB_FROM_BLOCK(pBlock)->flags & MEM_POOL_FLAGS_BLOCK_DELETE) != 0)
	#define MEM_GET_FIRST(pPool,pIt)		/*lint -e{506,522} */((__MEM_GET_ITERATOR_HEAD(pIt) = ((RTS_PCB *)pPool)->pBlocksInUse) != NULL ? ((__MEM_GET_ITERATOR_NEXT(pIt) = NULL) != NULL ? 0 : 1) : ((__MEM_GET_ITERATOR_NEXT(pIt) = NULL) != NULL ? 1 : 0))
	#define MEM_GET_NEXT(pIt)				/*lint -e{506} */((__MEM_GET_ITERATOR_NEXT(pIt) == NULL) ? (__MEM_GET_ITERATOR_NEXT(pIt) = __MEM_GET_ITERATOR_HEAD(pIt)) : (__MEM_GET_ITERATOR_NEXT(pIt) = __MEM_GET_ITERATOR_NEXT(pIt)->pBCB))
	#define MEM_GET_DATA(pIt)				( __MEM_GET_ITERATOR_NEXT(pIt) + 1 )
	#define MEM_GET_DATA_SKIP_DELETE(pIt)	( (((RTS_BCB *)__MEM_GET_ITERATOR_NEXT(pIt))->flags & MEM_POOL_FLAGS_BLOCK_DELETE) == 0 ? __MEM_GET_ITERATOR_NEXT(pIt) + 1 : NULL )
	#define MEM_GET_NEXT_FROM_DATA(pData)	/*lint -e{506} */(((RTS_BCB*)((unsigned char *)pData - sizeof(RTS_BCB)))->pBCB == NULL ? NULL : ((RTS_BCB*)((unsigned char *)pData - sizeof(RTS_BCB)))->pBCB + 1)
#endif


/**
 * <category>Static MemPool configuration</category>
 * <description>Structure to configure a static MemPool</description>
 * <element name="ulBlockSize" type="IN">Block size of each pool block in bytes</element>
 * <element name="ulMemSize" type="IN">Size of the static memory in bytes</element>
 * <element name="pMemory" type="IN">Pointer to static memory</element>
 */
typedef struct MemPoolConfigurationStatic_
{
	RTS_SIZE ulBlockSize;
	RTS_SIZE ulMemSize;
	void* pMemory;
} MemPoolConfigurationStatic;

/**
 * <category>Dynamic MemPool configuration</category>
 * <description>Structure to configure a dynamic/heap MemPool</description>
 * <element name="pszComponentName" type="IN">Name of the component, which allocates this MemPool</element>
 * <element name="ulNumBlocks" type="IN">Number of blocks which should be allocated directly at creating this MemPool</element>
 * <element name="ulBlockSize" type="IN">Block size of each pool block in bytes</element>
 */
typedef struct MemPoolConfigurationDynamic_
{
	char *pszComponentName;
	RTS_SIZE ulNumBlocks;
	RTS_SIZE ulBlockSize;
} MemPoolConfigurationDynamic;

/**
 * <category>Generic MemPool configuration</category>
 * <description>Structure to configure a MemPool with MemPoolCreate()</description>
 * <element name="options" type="IN">Options of a MemPool. See category "MemPool options" for detail information</element>
 * <element name="pfDeleteFunction" type="IN">Delete function to release resources within a block. Is called in MemPoolPutBlock() and MemPoolPutBlock_LF() implicitly.</element>
 * <element name="config" type="IN">Union to configure a static or dynamic MemPool. Which one is used is specified in options field.</element>
 */
typedef struct MemPoolConfiguration_
{
	RTS_UI32 options;
	PF_MEMBLOCK_DELETEFUNCTION pfDeleteFunction;

	union
	{
		MemPoolConfigurationStatic stat;
		MemPoolConfigurationDynamic dynamic;
	} config;
} MemPoolConfiguration;


#ifdef __cplusplus
extern "C" {
#endif


/**
 * <description>
 *	Create a new MemPool (dynamic or static)
 * </description>
 * <param name="pConfiguration" type="IN">Pointer to configuration</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the memory pool</result>
 */
RTS_HANDLE CDECL MemPoolCreate(MemPoolConfiguration *pConfiguration, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFMEMPOOLCREATE) (MemPoolConfiguration *pConfiguration, RTS_RESULT *pResult);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLCREATE_NOTIMPLEMENTED)
	#define USE_MemPoolCreate
	#define EXT_MemPoolCreate
	#define GET_MemPoolCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolCreate(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_MemPoolCreate  FALSE
	#define EXP_MemPoolCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolCreate
	#define EXT_MemPoolCreate
	#define GET_MemPoolCreate(fl)  CAL_CMGETAPI( "MemPoolCreate" ) 
	#define CAL_MemPoolCreate  MemPoolCreate
	#define CHK_MemPoolCreate  TRUE
	#define EXP_MemPoolCreate  CAL_CMEXPAPI( "MemPoolCreate" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolCreate
	#define EXT_MemPoolCreate
	#define GET_MemPoolCreate(fl)  CAL_CMGETAPI( "MemPoolCreate" ) 
	#define CAL_MemPoolCreate  MemPoolCreate
	#define CHK_MemPoolCreate  TRUE
	#define EXP_MemPoolCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolCreate", (RTS_UINTPTR)MemPoolCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolCreate
	#define EXT_CmpMemPoolMemPoolCreate
	#define GET_CmpMemPoolMemPoolCreate  ERR_OK
	#define CAL_CmpMemPoolMemPoolCreate pICmpMemPool->IMemPoolCreate
	#define CHK_CmpMemPoolMemPoolCreate (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolCreate
	#define EXT_MemPoolCreate
	#define GET_MemPoolCreate(fl)  CAL_CMGETAPI( "MemPoolCreate" ) 
	#define CAL_MemPoolCreate pICmpMemPool->IMemPoolCreate
	#define CHK_MemPoolCreate (pICmpMemPool != NULL)
	#define EXP_MemPoolCreate  CAL_CMEXPAPI( "MemPoolCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolCreate  PFMEMPOOLCREATE pfMemPoolCreate;
	#define EXT_MemPoolCreate  extern PFMEMPOOLCREATE pfMemPoolCreate;
	#define GET_MemPoolCreate(fl)  s_pfCMGetAPI2( "MemPoolCreate", (RTS_VOID_FCTPTR *)&pfMemPoolCreate, (fl), 0, 0)
	#define CAL_MemPoolCreate  pfMemPoolCreate
	#define CHK_MemPoolCreate  (pfMemPoolCreate != NULL)
	#define EXP_MemPoolCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolCreate", (RTS_UINTPTR)MemPoolCreate, 0, 0) 
#endif




/**
 * <description>
 *	Create a dynamic pool (consists of heap memory)
 * </description>
 * <param name="pszComponentName" type="IN">Component name</param>
 * <param name="ulBlockSize" type="IN">Size of each memory block in the pool</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">MemPool created successfully</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOMEMORY">No memory available</errorcode>
 * <result>Handle to the memory pool</result>
 */
RTS_HANDLE CDECL MemPoolCreateDynamic(char *pszComponentName, RTS_SIZE ulNumBlocks, RTS_SIZE ulBlockSize, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFMEMPOOLCREATEDYNAMIC) (char *pszComponentName, RTS_SIZE ulNumBlocks, RTS_SIZE ulBlockSize, RTS_RESULT *pResult);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLCREATEDYNAMIC_NOTIMPLEMENTED)
	#define USE_MemPoolCreateDynamic
	#define EXT_MemPoolCreateDynamic
	#define GET_MemPoolCreateDynamic(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolCreateDynamic(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_MemPoolCreateDynamic  FALSE
	#define EXP_MemPoolCreateDynamic  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolCreateDynamic
	#define EXT_MemPoolCreateDynamic
	#define GET_MemPoolCreateDynamic(fl)  CAL_CMGETAPI( "MemPoolCreateDynamic" ) 
	#define CAL_MemPoolCreateDynamic  MemPoolCreateDynamic
	#define CHK_MemPoolCreateDynamic  TRUE
	#define EXP_MemPoolCreateDynamic  CAL_CMEXPAPI( "MemPoolCreateDynamic" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolCreateDynamic
	#define EXT_MemPoolCreateDynamic
	#define GET_MemPoolCreateDynamic(fl)  CAL_CMGETAPI( "MemPoolCreateDynamic" ) 
	#define CAL_MemPoolCreateDynamic  MemPoolCreateDynamic
	#define CHK_MemPoolCreateDynamic  TRUE
	#define EXP_MemPoolCreateDynamic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolCreateDynamic", (RTS_UINTPTR)MemPoolCreateDynamic, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolCreateDynamic
	#define EXT_CmpMemPoolMemPoolCreateDynamic
	#define GET_CmpMemPoolMemPoolCreateDynamic  ERR_OK
	#define CAL_CmpMemPoolMemPoolCreateDynamic pICmpMemPool->IMemPoolCreateDynamic
	#define CHK_CmpMemPoolMemPoolCreateDynamic (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolCreateDynamic  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolCreateDynamic
	#define EXT_MemPoolCreateDynamic
	#define GET_MemPoolCreateDynamic(fl)  CAL_CMGETAPI( "MemPoolCreateDynamic" ) 
	#define CAL_MemPoolCreateDynamic pICmpMemPool->IMemPoolCreateDynamic
	#define CHK_MemPoolCreateDynamic (pICmpMemPool != NULL)
	#define EXP_MemPoolCreateDynamic  CAL_CMEXPAPI( "MemPoolCreateDynamic" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolCreateDynamic  PFMEMPOOLCREATEDYNAMIC pfMemPoolCreateDynamic;
	#define EXT_MemPoolCreateDynamic  extern PFMEMPOOLCREATEDYNAMIC pfMemPoolCreateDynamic;
	#define GET_MemPoolCreateDynamic(fl)  s_pfCMGetAPI2( "MemPoolCreateDynamic", (RTS_VOID_FCTPTR *)&pfMemPoolCreateDynamic, (fl), 0, 0)
	#define CAL_MemPoolCreateDynamic  pfMemPoolCreateDynamic
	#define CHK_MemPoolCreateDynamic  (pfMemPoolCreateDynamic != NULL)
	#define EXP_MemPoolCreateDynamic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolCreateDynamic", (RTS_UINTPTR)MemPoolCreateDynamic, 0, 0) 
#endif




/** 
 * <description>
 * <p>Create a memory pool from a static memory buffer.</p>
 * <p>The memory buffer don't has to be aligned in a specific way. Therefore,
 * not all of the memory in the buffer might be used. To get the appropriate
 * additional buffer, the caller is recommended to use the macro
 * MEM_GET_STATIC_LEN(Num, Struct) to get the size of the buffer</p>
 * <p>For example:</p>
 * <pre>
 * typedef struct
 * {
 *     ...
 * } myStruct_s;
 *
 * #define NUM_OF_STATIC_ELEMENTS 0x100
 *
 * RTS_UI8 s_byMyStaticPool[MEM_GET_STATIC_LEN(NUM_OF_STATIC_ELEMENTS, myStruct_s);
 * </pre>
 * </description>
 * <param name="ulBlockSize" type="IN" range="[BLOCKSIZE_ALIGNED1,BLOCKSIZE_ALIGNED2,BLOCKSIZE_ALIGNED4,BLOCKSIZE_ALIGNED8]">Size of each memory block in the pool, misaligned to 1, 2, 4 or 8 bytes</param>
 * <param name="ulMemSize" type="IN" range="[MEMSIZE_INVALID,MEMSIZE_ALIGNED1,MEMSIZE_ALIGNED2,MEMSIZE_ALIGNED4,MEMSIZE_ALIGNED8]">Complete size of the static memory, misaligned to 1, 2, 4 or 8 bytes</param>
 * <param name="pMemory" type="IN" range="[NULL,VALID_MEM]">Pointer to the static memory</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">MemPool created successfully</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">Invalid parameter(e.g. pMemory = NULL)</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NOMEMORY">ulMemSize was not large enough, to hold at least one block</errorcode>
 * <result>Handle to the memory pool</result>
 */
RTS_HANDLE CDECL MemPoolCreateStatic(RTS_SIZE ulBlockSize, RTS_SIZE ulMemSize, void* pMemory, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFMEMPOOLCREATESTATIC) (RTS_SIZE ulBlockSize, RTS_SIZE ulMemSize, void* pMemory, RTS_RESULT *pResult);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLCREATESTATIC_NOTIMPLEMENTED)
	#define USE_MemPoolCreateStatic
	#define EXT_MemPoolCreateStatic
	#define GET_MemPoolCreateStatic(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolCreateStatic(p0,p1,p2,p3)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_MemPoolCreateStatic  FALSE
	#define EXP_MemPoolCreateStatic  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolCreateStatic
	#define EXT_MemPoolCreateStatic
	#define GET_MemPoolCreateStatic(fl)  CAL_CMGETAPI( "MemPoolCreateStatic" ) 
	#define CAL_MemPoolCreateStatic  MemPoolCreateStatic
	#define CHK_MemPoolCreateStatic  TRUE
	#define EXP_MemPoolCreateStatic  CAL_CMEXPAPI( "MemPoolCreateStatic" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolCreateStatic
	#define EXT_MemPoolCreateStatic
	#define GET_MemPoolCreateStatic(fl)  CAL_CMGETAPI( "MemPoolCreateStatic" ) 
	#define CAL_MemPoolCreateStatic  MemPoolCreateStatic
	#define CHK_MemPoolCreateStatic  TRUE
	#define EXP_MemPoolCreateStatic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolCreateStatic", (RTS_UINTPTR)MemPoolCreateStatic, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolCreateStatic
	#define EXT_CmpMemPoolMemPoolCreateStatic
	#define GET_CmpMemPoolMemPoolCreateStatic  ERR_OK
	#define CAL_CmpMemPoolMemPoolCreateStatic pICmpMemPool->IMemPoolCreateStatic
	#define CHK_CmpMemPoolMemPoolCreateStatic (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolCreateStatic  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolCreateStatic
	#define EXT_MemPoolCreateStatic
	#define GET_MemPoolCreateStatic(fl)  CAL_CMGETAPI( "MemPoolCreateStatic" ) 
	#define CAL_MemPoolCreateStatic pICmpMemPool->IMemPoolCreateStatic
	#define CHK_MemPoolCreateStatic (pICmpMemPool != NULL)
	#define EXP_MemPoolCreateStatic  CAL_CMEXPAPI( "MemPoolCreateStatic" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolCreateStatic  PFMEMPOOLCREATESTATIC pfMemPoolCreateStatic;
	#define EXT_MemPoolCreateStatic  extern PFMEMPOOLCREATESTATIC pfMemPoolCreateStatic;
	#define GET_MemPoolCreateStatic(fl)  s_pfCMGetAPI2( "MemPoolCreateStatic", (RTS_VOID_FCTPTR *)&pfMemPoolCreateStatic, (fl), 0, 0)
	#define CAL_MemPoolCreateStatic  pfMemPoolCreateStatic
	#define CHK_MemPoolCreateStatic  (pfMemPoolCreateStatic != NULL)
	#define EXP_MemPoolCreateStatic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolCreateStatic", (RTS_UINTPTR)MemPoolCreateStatic, 0, 0) 
#endif




/**
 * <description>
 *	Extend dynamically an existing pool
 * </description>
 * <param name="hMemPool" type="IN">Handle to the pool</param>
 * <param name="pszComponentName" type="IN">Component name</param>
 * <param name="ulNumBlocks" type="IN">Number of blocks to extend</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">MemPool extended successfully</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">ulNumBlocks was 0</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_INVALID_HANDLE">hMemPool was RTS_INVALID_HANDLE</errorcode>
 */
RTS_RESULT CDECL MemPoolExtendDynamic(RTS_HANDLE hMemPool, char *pszComponentName, RTS_SIZE ulNumBlocks);
typedef RTS_RESULT (CDECL * PFMEMPOOLEXTENDDYNAMIC) (RTS_HANDLE hMemPool, char *pszComponentName, RTS_SIZE ulNumBlocks);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLEXTENDDYNAMIC_NOTIMPLEMENTED)
	#define USE_MemPoolExtendDynamic
	#define EXT_MemPoolExtendDynamic
	#define GET_MemPoolExtendDynamic(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolExtendDynamic(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolExtendDynamic  FALSE
	#define EXP_MemPoolExtendDynamic  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolExtendDynamic
	#define EXT_MemPoolExtendDynamic
	#define GET_MemPoolExtendDynamic(fl)  CAL_CMGETAPI( "MemPoolExtendDynamic" ) 
	#define CAL_MemPoolExtendDynamic  MemPoolExtendDynamic
	#define CHK_MemPoolExtendDynamic  TRUE
	#define EXP_MemPoolExtendDynamic  CAL_CMEXPAPI( "MemPoolExtendDynamic" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolExtendDynamic
	#define EXT_MemPoolExtendDynamic
	#define GET_MemPoolExtendDynamic(fl)  CAL_CMGETAPI( "MemPoolExtendDynamic" ) 
	#define CAL_MemPoolExtendDynamic  MemPoolExtendDynamic
	#define CHK_MemPoolExtendDynamic  TRUE
	#define EXP_MemPoolExtendDynamic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolExtendDynamic", (RTS_UINTPTR)MemPoolExtendDynamic, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolExtendDynamic
	#define EXT_CmpMemPoolMemPoolExtendDynamic
	#define GET_CmpMemPoolMemPoolExtendDynamic  ERR_OK
	#define CAL_CmpMemPoolMemPoolExtendDynamic pICmpMemPool->IMemPoolExtendDynamic
	#define CHK_CmpMemPoolMemPoolExtendDynamic (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolExtendDynamic  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolExtendDynamic
	#define EXT_MemPoolExtendDynamic
	#define GET_MemPoolExtendDynamic(fl)  CAL_CMGETAPI( "MemPoolExtendDynamic" ) 
	#define CAL_MemPoolExtendDynamic pICmpMemPool->IMemPoolExtendDynamic
	#define CHK_MemPoolExtendDynamic (pICmpMemPool != NULL)
	#define EXP_MemPoolExtendDynamic  CAL_CMEXPAPI( "MemPoolExtendDynamic" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolExtendDynamic  PFMEMPOOLEXTENDDYNAMIC pfMemPoolExtendDynamic;
	#define EXT_MemPoolExtendDynamic  extern PFMEMPOOLEXTENDDYNAMIC pfMemPoolExtendDynamic;
	#define GET_MemPoolExtendDynamic(fl)  s_pfCMGetAPI2( "MemPoolExtendDynamic", (RTS_VOID_FCTPTR *)&pfMemPoolExtendDynamic, (fl), 0, 0)
	#define CAL_MemPoolExtendDynamic  pfMemPoolExtendDynamic
	#define CHK_MemPoolExtendDynamic  (pfMemPoolExtendDynamic != NULL)
	#define EXP_MemPoolExtendDynamic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolExtendDynamic", (RTS_UINTPTR)MemPoolExtendDynamic, 0, 0) 
#endif




/**
 * <description>
 *	Extend an existing pool with a static array
 * </description>
 * <param name="hMemPool" type="IN">Handle to the pool</param>
 * <param name="ulMemSize" type="IN">Complete size of the static memory</param>
 * <param name="pMemory" type="IN">Pointer to the static memory</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL MemPoolExtendStatic(RTS_HANDLE hMemPool, RTS_SIZE ulMemSize, void* pMemory);
typedef RTS_RESULT (CDECL * PFMEMPOOLEXTENDSTATIC) (RTS_HANDLE hMemPool, RTS_SIZE ulMemSize, void* pMemory);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLEXTENDSTATIC_NOTIMPLEMENTED)
	#define USE_MemPoolExtendStatic
	#define EXT_MemPoolExtendStatic
	#define GET_MemPoolExtendStatic(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolExtendStatic(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolExtendStatic  FALSE
	#define EXP_MemPoolExtendStatic  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolExtendStatic
	#define EXT_MemPoolExtendStatic
	#define GET_MemPoolExtendStatic(fl)  CAL_CMGETAPI( "MemPoolExtendStatic" ) 
	#define CAL_MemPoolExtendStatic  MemPoolExtendStatic
	#define CHK_MemPoolExtendStatic  TRUE
	#define EXP_MemPoolExtendStatic  CAL_CMEXPAPI( "MemPoolExtendStatic" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolExtendStatic
	#define EXT_MemPoolExtendStatic
	#define GET_MemPoolExtendStatic(fl)  CAL_CMGETAPI( "MemPoolExtendStatic" ) 
	#define CAL_MemPoolExtendStatic  MemPoolExtendStatic
	#define CHK_MemPoolExtendStatic  TRUE
	#define EXP_MemPoolExtendStatic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolExtendStatic", (RTS_UINTPTR)MemPoolExtendStatic, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolExtendStatic
	#define EXT_CmpMemPoolMemPoolExtendStatic
	#define GET_CmpMemPoolMemPoolExtendStatic  ERR_OK
	#define CAL_CmpMemPoolMemPoolExtendStatic pICmpMemPool->IMemPoolExtendStatic
	#define CHK_CmpMemPoolMemPoolExtendStatic (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolExtendStatic  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolExtendStatic
	#define EXT_MemPoolExtendStatic
	#define GET_MemPoolExtendStatic(fl)  CAL_CMGETAPI( "MemPoolExtendStatic" ) 
	#define CAL_MemPoolExtendStatic pICmpMemPool->IMemPoolExtendStatic
	#define CHK_MemPoolExtendStatic (pICmpMemPool != NULL)
	#define EXP_MemPoolExtendStatic  CAL_CMEXPAPI( "MemPoolExtendStatic" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolExtendStatic  PFMEMPOOLEXTENDSTATIC pfMemPoolExtendStatic;
	#define EXT_MemPoolExtendStatic  extern PFMEMPOOLEXTENDSTATIC pfMemPoolExtendStatic;
	#define GET_MemPoolExtendStatic(fl)  s_pfCMGetAPI2( "MemPoolExtendStatic", (RTS_VOID_FCTPTR *)&pfMemPoolExtendStatic, (fl), 0, 0)
	#define CAL_MemPoolExtendStatic  pfMemPoolExtendStatic
	#define CHK_MemPoolExtendStatic  (pfMemPoolExtendStatic != NULL)
	#define EXP_MemPoolExtendStatic  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolExtendStatic", (RTS_UINTPTR)MemPoolExtendStatic, 0, 0) 
#endif




/**
 * <description>
 *	Create the internal sync object for synchronizing the pool.
 * </description>
 * <param name="hMemPool" type="IN">Handle to the pool</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL MemPoolCreateSyncObject(RTS_HANDLE hMemPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLCREATESYNCOBJECT) (RTS_HANDLE hMemPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLCREATESYNCOBJECT_NOTIMPLEMENTED)
	#define USE_MemPoolCreateSyncObject
	#define EXT_MemPoolCreateSyncObject
	#define GET_MemPoolCreateSyncObject(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolCreateSyncObject(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolCreateSyncObject  FALSE
	#define EXP_MemPoolCreateSyncObject  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolCreateSyncObject
	#define EXT_MemPoolCreateSyncObject
	#define GET_MemPoolCreateSyncObject(fl)  CAL_CMGETAPI( "MemPoolCreateSyncObject" ) 
	#define CAL_MemPoolCreateSyncObject  MemPoolCreateSyncObject
	#define CHK_MemPoolCreateSyncObject  TRUE
	#define EXP_MemPoolCreateSyncObject  CAL_CMEXPAPI( "MemPoolCreateSyncObject" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolCreateSyncObject
	#define EXT_MemPoolCreateSyncObject
	#define GET_MemPoolCreateSyncObject(fl)  CAL_CMGETAPI( "MemPoolCreateSyncObject" ) 
	#define CAL_MemPoolCreateSyncObject  MemPoolCreateSyncObject
	#define CHK_MemPoolCreateSyncObject  TRUE
	#define EXP_MemPoolCreateSyncObject  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolCreateSyncObject", (RTS_UINTPTR)MemPoolCreateSyncObject, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolCreateSyncObject
	#define EXT_CmpMemPoolMemPoolCreateSyncObject
	#define GET_CmpMemPoolMemPoolCreateSyncObject  ERR_OK
	#define CAL_CmpMemPoolMemPoolCreateSyncObject pICmpMemPool->IMemPoolCreateSyncObject
	#define CHK_CmpMemPoolMemPoolCreateSyncObject (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolCreateSyncObject  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolCreateSyncObject
	#define EXT_MemPoolCreateSyncObject
	#define GET_MemPoolCreateSyncObject(fl)  CAL_CMGETAPI( "MemPoolCreateSyncObject" ) 
	#define CAL_MemPoolCreateSyncObject pICmpMemPool->IMemPoolCreateSyncObject
	#define CHK_MemPoolCreateSyncObject (pICmpMemPool != NULL)
	#define EXP_MemPoolCreateSyncObject  CAL_CMEXPAPI( "MemPoolCreateSyncObject" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolCreateSyncObject  PFMEMPOOLCREATESYNCOBJECT pfMemPoolCreateSyncObject;
	#define EXT_MemPoolCreateSyncObject  extern PFMEMPOOLCREATESYNCOBJECT pfMemPoolCreateSyncObject;
	#define GET_MemPoolCreateSyncObject(fl)  s_pfCMGetAPI2( "MemPoolCreateSyncObject", (RTS_VOID_FCTPTR *)&pfMemPoolCreateSyncObject, (fl), 0, 0)
	#define CAL_MemPoolCreateSyncObject  pfMemPoolCreateSyncObject
	#define CHK_MemPoolCreateSyncObject  (pfMemPoolCreateSyncObject != NULL)
	#define EXP_MemPoolCreateSyncObject  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolCreateSyncObject", (RTS_UINTPTR)MemPoolCreateSyncObject, 0, 0) 
#endif




/**
 * <description>
 *	Delete the internal sync object for synchronizing the pool.
 * </description>
 * <param name="hMemPool" type="IN">Handle to the pool</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL MemPoolDeleteSyncObject(RTS_HANDLE hMemPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLDELETESYNCOBJECT) (RTS_HANDLE hMemPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLDELETESYNCOBJECT_NOTIMPLEMENTED)
	#define USE_MemPoolDeleteSyncObject
	#define EXT_MemPoolDeleteSyncObject
	#define GET_MemPoolDeleteSyncObject(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolDeleteSyncObject(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolDeleteSyncObject  FALSE
	#define EXP_MemPoolDeleteSyncObject  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolDeleteSyncObject
	#define EXT_MemPoolDeleteSyncObject
	#define GET_MemPoolDeleteSyncObject(fl)  CAL_CMGETAPI( "MemPoolDeleteSyncObject" ) 
	#define CAL_MemPoolDeleteSyncObject  MemPoolDeleteSyncObject
	#define CHK_MemPoolDeleteSyncObject  TRUE
	#define EXP_MemPoolDeleteSyncObject  CAL_CMEXPAPI( "MemPoolDeleteSyncObject" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolDeleteSyncObject
	#define EXT_MemPoolDeleteSyncObject
	#define GET_MemPoolDeleteSyncObject(fl)  CAL_CMGETAPI( "MemPoolDeleteSyncObject" ) 
	#define CAL_MemPoolDeleteSyncObject  MemPoolDeleteSyncObject
	#define CHK_MemPoolDeleteSyncObject  TRUE
	#define EXP_MemPoolDeleteSyncObject  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolDeleteSyncObject", (RTS_UINTPTR)MemPoolDeleteSyncObject, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolDeleteSyncObject
	#define EXT_CmpMemPoolMemPoolDeleteSyncObject
	#define GET_CmpMemPoolMemPoolDeleteSyncObject  ERR_OK
	#define CAL_CmpMemPoolMemPoolDeleteSyncObject pICmpMemPool->IMemPoolDeleteSyncObject
	#define CHK_CmpMemPoolMemPoolDeleteSyncObject (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolDeleteSyncObject  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolDeleteSyncObject
	#define EXT_MemPoolDeleteSyncObject
	#define GET_MemPoolDeleteSyncObject(fl)  CAL_CMGETAPI( "MemPoolDeleteSyncObject" ) 
	#define CAL_MemPoolDeleteSyncObject pICmpMemPool->IMemPoolDeleteSyncObject
	#define CHK_MemPoolDeleteSyncObject (pICmpMemPool != NULL)
	#define EXP_MemPoolDeleteSyncObject  CAL_CMEXPAPI( "MemPoolDeleteSyncObject" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolDeleteSyncObject  PFMEMPOOLDELETESYNCOBJECT pfMemPoolDeleteSyncObject;
	#define EXT_MemPoolDeleteSyncObject  extern PFMEMPOOLDELETESYNCOBJECT pfMemPoolDeleteSyncObject;
	#define GET_MemPoolDeleteSyncObject(fl)  s_pfCMGetAPI2( "MemPoolDeleteSyncObject", (RTS_VOID_FCTPTR *)&pfMemPoolDeleteSyncObject, (fl), 0, 0)
	#define CAL_MemPoolDeleteSyncObject  pfMemPoolDeleteSyncObject
	#define CHK_MemPoolDeleteSyncObject  (pfMemPoolDeleteSyncObject != NULL)
	#define EXP_MemPoolDeleteSyncObject  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolDeleteSyncObject", (RTS_UINTPTR)MemPoolDeleteSyncObject, 0, 0) 
#endif




/**
 * <description>
 *	Delete an existing pool
 * </description>
 * <param name="hMemPool" type="IN">Handle to the pool</param>
 * <param name="pszComponentName" type="IN">Component name</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL MemPoolDelete(RTS_HANDLE hMemPool, char *pszComponentName);
typedef RTS_RESULT (CDECL * PFMEMPOOLDELETE) (RTS_HANDLE hMemPool, char *pszComponentName);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLDELETE_NOTIMPLEMENTED)
	#define USE_MemPoolDelete
	#define EXT_MemPoolDelete
	#define GET_MemPoolDelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolDelete(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolDelete  FALSE
	#define EXP_MemPoolDelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolDelete
	#define EXT_MemPoolDelete
	#define GET_MemPoolDelete(fl)  CAL_CMGETAPI( "MemPoolDelete" ) 
	#define CAL_MemPoolDelete  MemPoolDelete
	#define CHK_MemPoolDelete  TRUE
	#define EXP_MemPoolDelete  CAL_CMEXPAPI( "MemPoolDelete" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolDelete
	#define EXT_MemPoolDelete
	#define GET_MemPoolDelete(fl)  CAL_CMGETAPI( "MemPoolDelete" ) 
	#define CAL_MemPoolDelete  MemPoolDelete
	#define CHK_MemPoolDelete  TRUE
	#define EXP_MemPoolDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolDelete", (RTS_UINTPTR)MemPoolDelete, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolDelete
	#define EXT_CmpMemPoolMemPoolDelete
	#define GET_CmpMemPoolMemPoolDelete  ERR_OK
	#define CAL_CmpMemPoolMemPoolDelete pICmpMemPool->IMemPoolDelete
	#define CHK_CmpMemPoolMemPoolDelete (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolDelete  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolDelete
	#define EXT_MemPoolDelete
	#define GET_MemPoolDelete(fl)  CAL_CMGETAPI( "MemPoolDelete" ) 
	#define CAL_MemPoolDelete pICmpMemPool->IMemPoolDelete
	#define CHK_MemPoolDelete (pICmpMemPool != NULL)
	#define EXP_MemPoolDelete  CAL_CMEXPAPI( "MemPoolDelete" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolDelete  PFMEMPOOLDELETE pfMemPoolDelete;
	#define EXT_MemPoolDelete  extern PFMEMPOOLDELETE pfMemPoolDelete;
	#define GET_MemPoolDelete(fl)  s_pfCMGetAPI2( "MemPoolDelete", (RTS_VOID_FCTPTR *)&pfMemPoolDelete, (fl), 0, 0)
	#define CAL_MemPoolDelete  pfMemPoolDelete
	#define CHK_MemPoolDelete  (pfMemPoolDelete != NULL)
	#define EXP_MemPoolDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolDelete", (RTS_UINTPTR)MemPoolDelete, 0, 0) 
#endif




/**
 * <description>
 *	Cleanup the pool (delete all allocated heap pool objects)
 * </description>
 * <param name="hMemPool" type="IN">Handle to the pool</param>
 * <param name="pszComponentName" type="IN">Component name</param>
 * <param name="bReleaseSemaphore" type="IN">1=Pool semaphore is released, 0=Only cleanup</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL MemPoolCleanup(RTS_HANDLE hMemPool, char *pszComponentName, int bReleaseSemaphore);
typedef RTS_RESULT (CDECL * PFMEMPOOLCLEANUP) (RTS_HANDLE hMemPool, char *pszComponentName, int bReleaseSemaphore);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLCLEANUP_NOTIMPLEMENTED)
	#define USE_MemPoolCleanup
	#define EXT_MemPoolCleanup
	#define GET_MemPoolCleanup(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolCleanup(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolCleanup  FALSE
	#define EXP_MemPoolCleanup  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolCleanup
	#define EXT_MemPoolCleanup
	#define GET_MemPoolCleanup(fl)  CAL_CMGETAPI( "MemPoolCleanup" ) 
	#define CAL_MemPoolCleanup  MemPoolCleanup
	#define CHK_MemPoolCleanup  TRUE
	#define EXP_MemPoolCleanup  CAL_CMEXPAPI( "MemPoolCleanup" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolCleanup
	#define EXT_MemPoolCleanup
	#define GET_MemPoolCleanup(fl)  CAL_CMGETAPI( "MemPoolCleanup" ) 
	#define CAL_MemPoolCleanup  MemPoolCleanup
	#define CHK_MemPoolCleanup  TRUE
	#define EXP_MemPoolCleanup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolCleanup", (RTS_UINTPTR)MemPoolCleanup, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolCleanup
	#define EXT_CmpMemPoolMemPoolCleanup
	#define GET_CmpMemPoolMemPoolCleanup  ERR_OK
	#define CAL_CmpMemPoolMemPoolCleanup pICmpMemPool->IMemPoolCleanup
	#define CHK_CmpMemPoolMemPoolCleanup (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolCleanup  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolCleanup
	#define EXT_MemPoolCleanup
	#define GET_MemPoolCleanup(fl)  CAL_CMGETAPI( "MemPoolCleanup" ) 
	#define CAL_MemPoolCleanup pICmpMemPool->IMemPoolCleanup
	#define CHK_MemPoolCleanup (pICmpMemPool != NULL)
	#define EXP_MemPoolCleanup  CAL_CMEXPAPI( "MemPoolCleanup" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolCleanup  PFMEMPOOLCLEANUP pfMemPoolCleanup;
	#define EXT_MemPoolCleanup  extern PFMEMPOOLCLEANUP pfMemPoolCleanup;
	#define GET_MemPoolCleanup(fl)  s_pfCMGetAPI2( "MemPoolCleanup", (RTS_VOID_FCTPTR *)&pfMemPoolCleanup, (fl), 0, 0)
	#define CAL_MemPoolCleanup  pfMemPoolCleanup
	#define CHK_MemPoolCleanup  (pfMemPoolCleanup != NULL)
	#define EXP_MemPoolCleanup  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolCleanup", (RTS_UINTPTR)MemPoolCleanup, 0, 0) 
#endif






/** 
 * <description>
 * <p>Get one memory block out of the pool.</p>
 * <p>SIL2 Implementation: If pPCB is wrong, an Exception is generated!</p>
 * </description>
 * <param name="hMemPool" type="IN" range="[RTS_INVALID_HANDLE,VALID_MEMPOOLHANDLE,INVALID_MEMPOOLHANDLE]">Handle to the pool</param>
 * <parampseudo name="hMemPool.NumEntries" type="IN" range="[0..NUM_OF_STATIC_ELEMENTS-1,NUM_OF_STATIC_ELEMENTS..INT_MAX]">Number of already allocated entries from the pool</parampseudo>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <parampseudo name="bExceptionGenerated" type="OUT" range="[TRUE,FALSE]">specifies, if an exception was generated or not</parampseudo>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Retrieved free block from MemPool</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_INVALID_HANDLE">No free block found</errorcode> 
 * <result>Pointer to the memory block</result>
 */
void* CDECL MemPoolGetBlock(RTS_HANDLE hMemPool, RTS_RESULT *pResult);
typedef void* (CDECL * PFMEMPOOLGETBLOCK) (RTS_HANDLE hMemPool, RTS_RESULT *pResult);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLGETBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolGetBlock
	#define EXT_MemPoolGetBlock
	#define GET_MemPoolGetBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolGetBlock(p0,p1)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolGetBlock  FALSE
	#define EXP_MemPoolGetBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolGetBlock
	#define EXT_MemPoolGetBlock
	#define GET_MemPoolGetBlock(fl)  CAL_CMGETAPI( "MemPoolGetBlock" ) 
	#define CAL_MemPoolGetBlock  MemPoolGetBlock
	#define CHK_MemPoolGetBlock  TRUE
	#define EXP_MemPoolGetBlock  CAL_CMEXPAPI( "MemPoolGetBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolGetBlock
	#define EXT_MemPoolGetBlock
	#define GET_MemPoolGetBlock(fl)  CAL_CMGETAPI( "MemPoolGetBlock" ) 
	#define CAL_MemPoolGetBlock  MemPoolGetBlock
	#define CHK_MemPoolGetBlock  TRUE
	#define EXP_MemPoolGetBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolGetBlock", (RTS_UINTPTR)MemPoolGetBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolGetBlock
	#define EXT_CmpMemPoolMemPoolGetBlock
	#define GET_CmpMemPoolMemPoolGetBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolGetBlock pICmpMemPool->IMemPoolGetBlock
	#define CHK_CmpMemPoolMemPoolGetBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolGetBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolGetBlock
	#define EXT_MemPoolGetBlock
	#define GET_MemPoolGetBlock(fl)  CAL_CMGETAPI( "MemPoolGetBlock" ) 
	#define CAL_MemPoolGetBlock pICmpMemPool->IMemPoolGetBlock
	#define CHK_MemPoolGetBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolGetBlock  CAL_CMEXPAPI( "MemPoolGetBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolGetBlock  PFMEMPOOLGETBLOCK pfMemPoolGetBlock;
	#define EXT_MemPoolGetBlock  extern PFMEMPOOLGETBLOCK pfMemPoolGetBlock;
	#define GET_MemPoolGetBlock(fl)  s_pfCMGetAPI2( "MemPoolGetBlock", (RTS_VOID_FCTPTR *)&pfMemPoolGetBlock, (fl), 0, 0)
	#define CAL_MemPoolGetBlock  pfMemPoolGetBlock
	#define CHK_MemPoolGetBlock  (pfMemPoolGetBlock != NULL)
	#define EXP_MemPoolGetBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolGetBlock", (RTS_UINTPTR)MemPoolGetBlock, 0, 0) 
#endif




/**
 * <description>
 *	Get one memory block out of the pool
 * </description>
 * <param name="hMemPool" type="IN">Handle to the pool</param>
 * <param name="bDynamic" type="IN">1=Block is created dynamically, if the pool is empty, 0=Only use of static pool memory</param>
 * <param name="pszComponentName" type="IN">Pointer to the component name for dynamic memory allocation</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Pointer to the memory block</result>
 */
void* CDECL MemPoolGetBlock2(RTS_HANDLE hMemPool, int bDynamic, char *pszComponentName, RTS_RESULT *pResult);
typedef void* (CDECL * PFMEMPOOLGETBLOCK2) (RTS_HANDLE hMemPool, int bDynamic, char *pszComponentName, RTS_RESULT *pResult);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLGETBLOCK2_NOTIMPLEMENTED)
	#define USE_MemPoolGetBlock2
	#define EXT_MemPoolGetBlock2
	#define GET_MemPoolGetBlock2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolGetBlock2(p0,p1,p2,p3)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolGetBlock2  FALSE
	#define EXP_MemPoolGetBlock2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolGetBlock2
	#define EXT_MemPoolGetBlock2
	#define GET_MemPoolGetBlock2(fl)  CAL_CMGETAPI( "MemPoolGetBlock2" ) 
	#define CAL_MemPoolGetBlock2  MemPoolGetBlock2
	#define CHK_MemPoolGetBlock2  TRUE
	#define EXP_MemPoolGetBlock2  CAL_CMEXPAPI( "MemPoolGetBlock2" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolGetBlock2
	#define EXT_MemPoolGetBlock2
	#define GET_MemPoolGetBlock2(fl)  CAL_CMGETAPI( "MemPoolGetBlock2" ) 
	#define CAL_MemPoolGetBlock2  MemPoolGetBlock2
	#define CHK_MemPoolGetBlock2  TRUE
	#define EXP_MemPoolGetBlock2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolGetBlock2", (RTS_UINTPTR)MemPoolGetBlock2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolGetBlock2
	#define EXT_CmpMemPoolMemPoolGetBlock2
	#define GET_CmpMemPoolMemPoolGetBlock2  ERR_OK
	#define CAL_CmpMemPoolMemPoolGetBlock2 pICmpMemPool->IMemPoolGetBlock2
	#define CHK_CmpMemPoolMemPoolGetBlock2 (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolGetBlock2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolGetBlock2
	#define EXT_MemPoolGetBlock2
	#define GET_MemPoolGetBlock2(fl)  CAL_CMGETAPI( "MemPoolGetBlock2" ) 
	#define CAL_MemPoolGetBlock2 pICmpMemPool->IMemPoolGetBlock2
	#define CHK_MemPoolGetBlock2 (pICmpMemPool != NULL)
	#define EXP_MemPoolGetBlock2  CAL_CMEXPAPI( "MemPoolGetBlock2" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolGetBlock2  PFMEMPOOLGETBLOCK2 pfMemPoolGetBlock2;
	#define EXT_MemPoolGetBlock2  extern PFMEMPOOLGETBLOCK2 pfMemPoolGetBlock2;
	#define GET_MemPoolGetBlock2(fl)  s_pfCMGetAPI2( "MemPoolGetBlock2", (RTS_VOID_FCTPTR *)&pfMemPoolGetBlock2, (fl), 0, 0)
	#define CAL_MemPoolGetBlock2  pfMemPoolGetBlock2
	#define CHK_MemPoolGetBlock2  (pfMemPoolGetBlock2 != NULL)
	#define EXP_MemPoolGetBlock2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolGetBlock2", (RTS_UINTPTR)MemPoolGetBlock2, 0, 0) 
#endif




/**
 * <description>
 *	Get one the pool control block of a specified pool
 * </description>
 * <param name="hMemPool" type="IN">Handle to the pool</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Pointer to the pool control block</result>
 */
RTS_PCB* CDECL MemPoolGetPCB(RTS_HANDLE hMemPool, RTS_RESULT *pResult);
typedef RTS_PCB* (CDECL * PFMEMPOOLGETPCB) (RTS_HANDLE hMemPool, RTS_RESULT *pResult);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLGETPCB_NOTIMPLEMENTED)
	#define USE_MemPoolGetPCB
	#define EXT_MemPoolGetPCB
	#define GET_MemPoolGetPCB(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolGetPCB(p0,p1)  (RTS_PCB*)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolGetPCB  FALSE
	#define EXP_MemPoolGetPCB  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolGetPCB
	#define EXT_MemPoolGetPCB
	#define GET_MemPoolGetPCB(fl)  CAL_CMGETAPI( "MemPoolGetPCB" ) 
	#define CAL_MemPoolGetPCB  MemPoolGetPCB
	#define CHK_MemPoolGetPCB  TRUE
	#define EXP_MemPoolGetPCB  CAL_CMEXPAPI( "MemPoolGetPCB" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolGetPCB
	#define EXT_MemPoolGetPCB
	#define GET_MemPoolGetPCB(fl)  CAL_CMGETAPI( "MemPoolGetPCB" ) 
	#define CAL_MemPoolGetPCB  MemPoolGetPCB
	#define CHK_MemPoolGetPCB  TRUE
	#define EXP_MemPoolGetPCB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolGetPCB", (RTS_UINTPTR)MemPoolGetPCB, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolGetPCB
	#define EXT_CmpMemPoolMemPoolGetPCB
	#define GET_CmpMemPoolMemPoolGetPCB  ERR_OK
	#define CAL_CmpMemPoolMemPoolGetPCB pICmpMemPool->IMemPoolGetPCB
	#define CHK_CmpMemPoolMemPoolGetPCB (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolGetPCB  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolGetPCB
	#define EXT_MemPoolGetPCB
	#define GET_MemPoolGetPCB(fl)  CAL_CMGETAPI( "MemPoolGetPCB" ) 
	#define CAL_MemPoolGetPCB pICmpMemPool->IMemPoolGetPCB
	#define CHK_MemPoolGetPCB (pICmpMemPool != NULL)
	#define EXP_MemPoolGetPCB  CAL_CMEXPAPI( "MemPoolGetPCB" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolGetPCB  PFMEMPOOLGETPCB pfMemPoolGetPCB;
	#define EXT_MemPoolGetPCB  extern PFMEMPOOLGETPCB pfMemPoolGetPCB;
	#define GET_MemPoolGetPCB(fl)  s_pfCMGetAPI2( "MemPoolGetPCB", (RTS_VOID_FCTPTR *)&pfMemPoolGetPCB, (fl), 0, 0)
	#define CAL_MemPoolGetPCB  pfMemPoolGetPCB
	#define CHK_MemPoolGetPCB  (pfMemPoolGetPCB != NULL)
	#define EXP_MemPoolGetPCB  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolGetPCB", (RTS_UINTPTR)MemPoolGetPCB, 0, 0) 
#endif




/**
 * <description>
 *	Put a memory block back into the pool (release). Now, the block is in the chain list of free blocks again. 
 * </description>
 * <param name="pBlock" type="IN" range="[NULL,VALID_BLOCK,INVALID_BLOCK]">Pointer to the memory block</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successfully added block to MemPool</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Could not add block to MemPool, wrong pBlock</errorcode> 
 */
RTS_RESULT CDECL MemPoolPutBlock(void* pBlock);
typedef RTS_RESULT (CDECL * PFMEMPOOLPUTBLOCK) (void* pBlock);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLPUTBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolPutBlock
	#define EXT_MemPoolPutBlock
	#define GET_MemPoolPutBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolPutBlock(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolPutBlock  FALSE
	#define EXP_MemPoolPutBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolPutBlock
	#define EXT_MemPoolPutBlock
	#define GET_MemPoolPutBlock(fl)  CAL_CMGETAPI( "MemPoolPutBlock" ) 
	#define CAL_MemPoolPutBlock  MemPoolPutBlock
	#define CHK_MemPoolPutBlock  TRUE
	#define EXP_MemPoolPutBlock  CAL_CMEXPAPI( "MemPoolPutBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolPutBlock
	#define EXT_MemPoolPutBlock
	#define GET_MemPoolPutBlock(fl)  CAL_CMGETAPI( "MemPoolPutBlock" ) 
	#define CAL_MemPoolPutBlock  MemPoolPutBlock
	#define CHK_MemPoolPutBlock  TRUE
	#define EXP_MemPoolPutBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolPutBlock", (RTS_UINTPTR)MemPoolPutBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolPutBlock
	#define EXT_CmpMemPoolMemPoolPutBlock
	#define GET_CmpMemPoolMemPoolPutBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolPutBlock pICmpMemPool->IMemPoolPutBlock
	#define CHK_CmpMemPoolMemPoolPutBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolPutBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolPutBlock
	#define EXT_MemPoolPutBlock
	#define GET_MemPoolPutBlock(fl)  CAL_CMGETAPI( "MemPoolPutBlock" ) 
	#define CAL_MemPoolPutBlock pICmpMemPool->IMemPoolPutBlock
	#define CHK_MemPoolPutBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolPutBlock  CAL_CMEXPAPI( "MemPoolPutBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolPutBlock  PFMEMPOOLPUTBLOCK pfMemPoolPutBlock;
	#define EXT_MemPoolPutBlock  extern PFMEMPOOLPUTBLOCK pfMemPoolPutBlock;
	#define GET_MemPoolPutBlock(fl)  s_pfCMGetAPI2( "MemPoolPutBlock", (RTS_VOID_FCTPTR *)&pfMemPoolPutBlock, (fl), 0, 0)
	#define CAL_MemPoolPutBlock  pfMemPoolPutBlock
	#define CHK_MemPoolPutBlock  (pfMemPoolPutBlock != NULL)
	#define EXP_MemPoolPutBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolPutBlock", (RTS_UINTPTR)MemPoolPutBlock, 0, 0) 
#endif




/**
 * <description>
 *	Add used block at the beginning of the pool. Now, the block is in the chain list of used blocks.
 * </description>
 * <param name="pBlock" type="IN" range="[NULL,VALID_BLOCK]">Pointer to the memory block</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successfully added block to MemPool</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Could not add block to MemPool, wrong pBlock</errorcode> 
 */
RTS_RESULT CDECL MemPoolAddUsedBlock(void* pBlock);
typedef RTS_RESULT (CDECL * PFMEMPOOLADDUSEDBLOCK) (void* pBlock);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLADDUSEDBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolAddUsedBlock
	#define EXT_MemPoolAddUsedBlock
	#define GET_MemPoolAddUsedBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolAddUsedBlock(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolAddUsedBlock  FALSE
	#define EXP_MemPoolAddUsedBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolAddUsedBlock
	#define EXT_MemPoolAddUsedBlock
	#define GET_MemPoolAddUsedBlock(fl)  CAL_CMGETAPI( "MemPoolAddUsedBlock" ) 
	#define CAL_MemPoolAddUsedBlock  MemPoolAddUsedBlock
	#define CHK_MemPoolAddUsedBlock  TRUE
	#define EXP_MemPoolAddUsedBlock  CAL_CMEXPAPI( "MemPoolAddUsedBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolAddUsedBlock
	#define EXT_MemPoolAddUsedBlock
	#define GET_MemPoolAddUsedBlock(fl)  CAL_CMGETAPI( "MemPoolAddUsedBlock" ) 
	#define CAL_MemPoolAddUsedBlock  MemPoolAddUsedBlock
	#define CHK_MemPoolAddUsedBlock  TRUE
	#define EXP_MemPoolAddUsedBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolAddUsedBlock", (RTS_UINTPTR)MemPoolAddUsedBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolAddUsedBlock
	#define EXT_CmpMemPoolMemPoolAddUsedBlock
	#define GET_CmpMemPoolMemPoolAddUsedBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolAddUsedBlock pICmpMemPool->IMemPoolAddUsedBlock
	#define CHK_CmpMemPoolMemPoolAddUsedBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolAddUsedBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolAddUsedBlock
	#define EXT_MemPoolAddUsedBlock
	#define GET_MemPoolAddUsedBlock(fl)  CAL_CMGETAPI( "MemPoolAddUsedBlock" ) 
	#define CAL_MemPoolAddUsedBlock pICmpMemPool->IMemPoolAddUsedBlock
	#define CHK_MemPoolAddUsedBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolAddUsedBlock  CAL_CMEXPAPI( "MemPoolAddUsedBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolAddUsedBlock  PFMEMPOOLADDUSEDBLOCK pfMemPoolAddUsedBlock;
	#define EXT_MemPoolAddUsedBlock  extern PFMEMPOOLADDUSEDBLOCK pfMemPoolAddUsedBlock;
	#define GET_MemPoolAddUsedBlock(fl)  s_pfCMGetAPI2( "MemPoolAddUsedBlock", (RTS_VOID_FCTPTR *)&pfMemPoolAddUsedBlock, (fl), 0, 0)
	#define CAL_MemPoolAddUsedBlock  pfMemPoolAddUsedBlock
	#define CHK_MemPoolAddUsedBlock  (pfMemPoolAddUsedBlock != NULL)
	#define EXP_MemPoolAddUsedBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolAddUsedBlock", (RTS_UINTPTR)MemPoolAddUsedBlock, 0, 0) 
#endif




/**
 * <description>
 *	Add used block at the end of the pool. Now, the block is in the chain list of used blocks.
 * </description>
 * <param name="pBlock" type="IN" range="[NULL,VALID_BLOCK]">Pointer to the memory block</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successfully added block to MemPool</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Could not add block to MemPool, wrong pBlock</errorcode> 
 */
RTS_RESULT CDECL MemPoolAppendUsedBlock(void* pBlock);
typedef RTS_RESULT (CDECL * PFMEMPOOLAPPENDUSEDBLOCK) (void* pBlock);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLAPPENDUSEDBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolAppendUsedBlock
	#define EXT_MemPoolAppendUsedBlock
	#define GET_MemPoolAppendUsedBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolAppendUsedBlock(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolAppendUsedBlock  FALSE
	#define EXP_MemPoolAppendUsedBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolAppendUsedBlock
	#define EXT_MemPoolAppendUsedBlock
	#define GET_MemPoolAppendUsedBlock(fl)  CAL_CMGETAPI( "MemPoolAppendUsedBlock" ) 
	#define CAL_MemPoolAppendUsedBlock  MemPoolAppendUsedBlock
	#define CHK_MemPoolAppendUsedBlock  TRUE
	#define EXP_MemPoolAppendUsedBlock  CAL_CMEXPAPI( "MemPoolAppendUsedBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolAppendUsedBlock
	#define EXT_MemPoolAppendUsedBlock
	#define GET_MemPoolAppendUsedBlock(fl)  CAL_CMGETAPI( "MemPoolAppendUsedBlock" ) 
	#define CAL_MemPoolAppendUsedBlock  MemPoolAppendUsedBlock
	#define CHK_MemPoolAppendUsedBlock  TRUE
	#define EXP_MemPoolAppendUsedBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolAppendUsedBlock", (RTS_UINTPTR)MemPoolAppendUsedBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolAppendUsedBlock
	#define EXT_CmpMemPoolMemPoolAppendUsedBlock
	#define GET_CmpMemPoolMemPoolAppendUsedBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolAppendUsedBlock pICmpMemPool->IMemPoolAppendUsedBlock
	#define CHK_CmpMemPoolMemPoolAppendUsedBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolAppendUsedBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolAppendUsedBlock
	#define EXT_MemPoolAppendUsedBlock
	#define GET_MemPoolAppendUsedBlock(fl)  CAL_CMGETAPI( "MemPoolAppendUsedBlock" ) 
	#define CAL_MemPoolAppendUsedBlock pICmpMemPool->IMemPoolAppendUsedBlock
	#define CHK_MemPoolAppendUsedBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolAppendUsedBlock  CAL_CMEXPAPI( "MemPoolAppendUsedBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolAppendUsedBlock  PFMEMPOOLAPPENDUSEDBLOCK pfMemPoolAppendUsedBlock;
	#define EXT_MemPoolAppendUsedBlock  extern PFMEMPOOLAPPENDUSEDBLOCK pfMemPoolAppendUsedBlock;
	#define GET_MemPoolAppendUsedBlock(fl)  s_pfCMGetAPI2( "MemPoolAppendUsedBlock", (RTS_VOID_FCTPTR *)&pfMemPoolAppendUsedBlock, (fl), 0, 0)
	#define CAL_MemPoolAppendUsedBlock  pfMemPoolAppendUsedBlock
	#define CHK_MemPoolAppendUsedBlock  (pfMemPoolAppendUsedBlock != NULL)
	#define EXP_MemPoolAppendUsedBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolAppendUsedBlock", (RTS_UINTPTR)MemPoolAppendUsedBlock, 0, 0) 
#endif




/**
 * <description>
 *	Insert used block right after the specified block or as a head element of an internal used blocks list.
 * </description>
 * <param name="pPrevBlock" type="IN" range="[NULL,VALID_PREVBLOCK,INVALID_PREVBLOCK]">Pointer to the predecessor block</param>
 * <param name="pBlock" type="IN" range="[NULL,VALID_BLOCK]">Pointer to the memory block</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successfully inserted block to MemPool</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Could not insert block to MemPool because pBlock is NULL or pPrevBlock belongs to a different pool than pBlock</errorcode> 
 */
RTS_RESULT CDECL MemPoolInsertUsedBlock(void* pPrevBlock, void* pBlock);
typedef RTS_RESULT (CDECL * PFMEMPOOLINSERTUSEDBLOCK) (void* pPrevBlock, void* pBlock);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLINSERTUSEDBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolInsertUsedBlock
	#define EXT_MemPoolInsertUsedBlock
	#define GET_MemPoolInsertUsedBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolInsertUsedBlock(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolInsertUsedBlock  FALSE
	#define EXP_MemPoolInsertUsedBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolInsertUsedBlock
	#define EXT_MemPoolInsertUsedBlock
	#define GET_MemPoolInsertUsedBlock(fl)  CAL_CMGETAPI( "MemPoolInsertUsedBlock" ) 
	#define CAL_MemPoolInsertUsedBlock  MemPoolInsertUsedBlock
	#define CHK_MemPoolInsertUsedBlock  TRUE
	#define EXP_MemPoolInsertUsedBlock  CAL_CMEXPAPI( "MemPoolInsertUsedBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolInsertUsedBlock
	#define EXT_MemPoolInsertUsedBlock
	#define GET_MemPoolInsertUsedBlock(fl)  CAL_CMGETAPI( "MemPoolInsertUsedBlock" ) 
	#define CAL_MemPoolInsertUsedBlock  MemPoolInsertUsedBlock
	#define CHK_MemPoolInsertUsedBlock  TRUE
	#define EXP_MemPoolInsertUsedBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolInsertUsedBlock", (RTS_UINTPTR)MemPoolInsertUsedBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolInsertUsedBlock
	#define EXT_CmpMemPoolMemPoolInsertUsedBlock
	#define GET_CmpMemPoolMemPoolInsertUsedBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolInsertUsedBlock pICmpMemPool->IMemPoolInsertUsedBlock
	#define CHK_CmpMemPoolMemPoolInsertUsedBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolInsertUsedBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolInsertUsedBlock
	#define EXT_MemPoolInsertUsedBlock
	#define GET_MemPoolInsertUsedBlock(fl)  CAL_CMGETAPI( "MemPoolInsertUsedBlock" ) 
	#define CAL_MemPoolInsertUsedBlock pICmpMemPool->IMemPoolInsertUsedBlock
	#define CHK_MemPoolInsertUsedBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolInsertUsedBlock  CAL_CMEXPAPI( "MemPoolInsertUsedBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolInsertUsedBlock  PFMEMPOOLINSERTUSEDBLOCK pfMemPoolInsertUsedBlock;
	#define EXT_MemPoolInsertUsedBlock  extern PFMEMPOOLINSERTUSEDBLOCK pfMemPoolInsertUsedBlock;
	#define GET_MemPoolInsertUsedBlock(fl)  s_pfCMGetAPI2( "MemPoolInsertUsedBlock", (RTS_VOID_FCTPTR *)&pfMemPoolInsertUsedBlock, (fl), 0, 0)
	#define CAL_MemPoolInsertUsedBlock  pfMemPoolInsertUsedBlock
	#define CHK_MemPoolInsertUsedBlock  (pfMemPoolInsertUsedBlock != NULL)
	#define EXP_MemPoolInsertUsedBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolInsertUsedBlock", (RTS_UINTPTR)MemPoolInsertUsedBlock, 0, 0) 
#endif




/**
 * <description>
 *	Remove used block from the pool. If this function succeeds, the block could not be recognized anymore by any iterator on the pool!
 * </description>
 * <param name="pBlock" type="IN" range="[NULL,VALID_BLOCK,VALID_BLOCK_MID,VALID_BLOCK_LAST]">Pointer to the memory block</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successfully removed block from MemPool</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Could not remove block from MemPool, wrong pBlock</errorcode> 
 */
RTS_RESULT CDECL MemPoolRemoveUsedBlock(void* pBlock);
typedef RTS_RESULT (CDECL * PFMEMPOOLREMOVEUSEDBLOCK) (void* pBlock);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLREMOVEUSEDBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolRemoveUsedBlock
	#define EXT_MemPoolRemoveUsedBlock
	#define GET_MemPoolRemoveUsedBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolRemoveUsedBlock(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolRemoveUsedBlock  FALSE
	#define EXP_MemPoolRemoveUsedBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolRemoveUsedBlock
	#define EXT_MemPoolRemoveUsedBlock
	#define GET_MemPoolRemoveUsedBlock(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlock" ) 
	#define CAL_MemPoolRemoveUsedBlock  MemPoolRemoveUsedBlock
	#define CHK_MemPoolRemoveUsedBlock  TRUE
	#define EXP_MemPoolRemoveUsedBlock  CAL_CMEXPAPI( "MemPoolRemoveUsedBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolRemoveUsedBlock
	#define EXT_MemPoolRemoveUsedBlock
	#define GET_MemPoolRemoveUsedBlock(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlock" ) 
	#define CAL_MemPoolRemoveUsedBlock  MemPoolRemoveUsedBlock
	#define CHK_MemPoolRemoveUsedBlock  TRUE
	#define EXP_MemPoolRemoveUsedBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolRemoveUsedBlock", (RTS_UINTPTR)MemPoolRemoveUsedBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolRemoveUsedBlock
	#define EXT_CmpMemPoolMemPoolRemoveUsedBlock
	#define GET_CmpMemPoolMemPoolRemoveUsedBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolRemoveUsedBlock pICmpMemPool->IMemPoolRemoveUsedBlock
	#define CHK_CmpMemPoolMemPoolRemoveUsedBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolRemoveUsedBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolRemoveUsedBlock
	#define EXT_MemPoolRemoveUsedBlock
	#define GET_MemPoolRemoveUsedBlock(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlock" ) 
	#define CAL_MemPoolRemoveUsedBlock pICmpMemPool->IMemPoolRemoveUsedBlock
	#define CHK_MemPoolRemoveUsedBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolRemoveUsedBlock  CAL_CMEXPAPI( "MemPoolRemoveUsedBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolRemoveUsedBlock  PFMEMPOOLREMOVEUSEDBLOCK pfMemPoolRemoveUsedBlock;
	#define EXT_MemPoolRemoveUsedBlock  extern PFMEMPOOLREMOVEUSEDBLOCK pfMemPoolRemoveUsedBlock;
	#define GET_MemPoolRemoveUsedBlock(fl)  s_pfCMGetAPI2( "MemPoolRemoveUsedBlock", (RTS_VOID_FCTPTR *)&pfMemPoolRemoveUsedBlock, (fl), 0, 0)
	#define CAL_MemPoolRemoveUsedBlock  pfMemPoolRemoveUsedBlock
	#define CHK_MemPoolRemoveUsedBlock  (pfMemPoolRemoveUsedBlock != NULL)
	#define EXP_MemPoolRemoveUsedBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolRemoveUsedBlock", (RTS_UINTPTR)MemPoolRemoveUsedBlock, 0, 0) 
#endif




/**
 * <description>
 *	Add used block the beginning of the specified pool. Now, the block is in the chain list of used blocks.
 * </description>
 * <param name="pBlock" type="IN">Pointer to the memory block</param>
 * <param name="hPool" type="IN">Handle to the pool</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL MemPoolAddUsedBlockToPool(void* pBlock, RTS_HANDLE hPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLADDUSEDBLOCKTOPOOL) (void* pBlock, RTS_HANDLE hPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLADDUSEDBLOCKTOPOOL_NOTIMPLEMENTED)
	#define USE_MemPoolAddUsedBlockToPool
	#define EXT_MemPoolAddUsedBlockToPool
	#define GET_MemPoolAddUsedBlockToPool(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolAddUsedBlockToPool(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolAddUsedBlockToPool  FALSE
	#define EXP_MemPoolAddUsedBlockToPool  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolAddUsedBlockToPool
	#define EXT_MemPoolAddUsedBlockToPool
	#define GET_MemPoolAddUsedBlockToPool(fl)  CAL_CMGETAPI( "MemPoolAddUsedBlockToPool" ) 
	#define CAL_MemPoolAddUsedBlockToPool  MemPoolAddUsedBlockToPool
	#define CHK_MemPoolAddUsedBlockToPool  TRUE
	#define EXP_MemPoolAddUsedBlockToPool  CAL_CMEXPAPI( "MemPoolAddUsedBlockToPool" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolAddUsedBlockToPool
	#define EXT_MemPoolAddUsedBlockToPool
	#define GET_MemPoolAddUsedBlockToPool(fl)  CAL_CMGETAPI( "MemPoolAddUsedBlockToPool" ) 
	#define CAL_MemPoolAddUsedBlockToPool  MemPoolAddUsedBlockToPool
	#define CHK_MemPoolAddUsedBlockToPool  TRUE
	#define EXP_MemPoolAddUsedBlockToPool  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolAddUsedBlockToPool", (RTS_UINTPTR)MemPoolAddUsedBlockToPool, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolAddUsedBlockToPool
	#define EXT_CmpMemPoolMemPoolAddUsedBlockToPool
	#define GET_CmpMemPoolMemPoolAddUsedBlockToPool  ERR_OK
	#define CAL_CmpMemPoolMemPoolAddUsedBlockToPool pICmpMemPool->IMemPoolAddUsedBlockToPool
	#define CHK_CmpMemPoolMemPoolAddUsedBlockToPool (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolAddUsedBlockToPool  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolAddUsedBlockToPool
	#define EXT_MemPoolAddUsedBlockToPool
	#define GET_MemPoolAddUsedBlockToPool(fl)  CAL_CMGETAPI( "MemPoolAddUsedBlockToPool" ) 
	#define CAL_MemPoolAddUsedBlockToPool pICmpMemPool->IMemPoolAddUsedBlockToPool
	#define CHK_MemPoolAddUsedBlockToPool (pICmpMemPool != NULL)
	#define EXP_MemPoolAddUsedBlockToPool  CAL_CMEXPAPI( "MemPoolAddUsedBlockToPool" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolAddUsedBlockToPool  PFMEMPOOLADDUSEDBLOCKTOPOOL pfMemPoolAddUsedBlockToPool;
	#define EXT_MemPoolAddUsedBlockToPool  extern PFMEMPOOLADDUSEDBLOCKTOPOOL pfMemPoolAddUsedBlockToPool;
	#define GET_MemPoolAddUsedBlockToPool(fl)  s_pfCMGetAPI2( "MemPoolAddUsedBlockToPool", (RTS_VOID_FCTPTR *)&pfMemPoolAddUsedBlockToPool, (fl), 0, 0)
	#define CAL_MemPoolAddUsedBlockToPool  pfMemPoolAddUsedBlockToPool
	#define CHK_MemPoolAddUsedBlockToPool  (pfMemPoolAddUsedBlockToPool != NULL)
	#define EXP_MemPoolAddUsedBlockToPool  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolAddUsedBlockToPool", (RTS_UINTPTR)MemPoolAddUsedBlockToPool, 0, 0) 
#endif




/**
 * <description>
 *	Add the block to the end of the used block list of hPool
 * </description>
 * <param name="pBlock" type="IN">Pointer to the memory block</param>
 * <param name="hPool" type="IN">Handle to the pool</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL MemPoolAppendUsedBlockToPool(void* pBlock, RTS_HANDLE hPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLAPPENDUSEDBLOCKTOPOOL) (void* pBlock, RTS_HANDLE hPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLAPPENDUSEDBLOCKTOPOOL_NOTIMPLEMENTED)
	#define USE_MemPoolAppendUsedBlockToPool
	#define EXT_MemPoolAppendUsedBlockToPool
	#define GET_MemPoolAppendUsedBlockToPool(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolAppendUsedBlockToPool(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolAppendUsedBlockToPool  FALSE
	#define EXP_MemPoolAppendUsedBlockToPool  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolAppendUsedBlockToPool
	#define EXT_MemPoolAppendUsedBlockToPool
	#define GET_MemPoolAppendUsedBlockToPool(fl)  CAL_CMGETAPI( "MemPoolAppendUsedBlockToPool" ) 
	#define CAL_MemPoolAppendUsedBlockToPool  MemPoolAppendUsedBlockToPool
	#define CHK_MemPoolAppendUsedBlockToPool  TRUE
	#define EXP_MemPoolAppendUsedBlockToPool  CAL_CMEXPAPI( "MemPoolAppendUsedBlockToPool" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolAppendUsedBlockToPool
	#define EXT_MemPoolAppendUsedBlockToPool
	#define GET_MemPoolAppendUsedBlockToPool(fl)  CAL_CMGETAPI( "MemPoolAppendUsedBlockToPool" ) 
	#define CAL_MemPoolAppendUsedBlockToPool  MemPoolAppendUsedBlockToPool
	#define CHK_MemPoolAppendUsedBlockToPool  TRUE
	#define EXP_MemPoolAppendUsedBlockToPool  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolAppendUsedBlockToPool", (RTS_UINTPTR)MemPoolAppendUsedBlockToPool, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolAppendUsedBlockToPool
	#define EXT_CmpMemPoolMemPoolAppendUsedBlockToPool
	#define GET_CmpMemPoolMemPoolAppendUsedBlockToPool  ERR_OK
	#define CAL_CmpMemPoolMemPoolAppendUsedBlockToPool pICmpMemPool->IMemPoolAppendUsedBlockToPool
	#define CHK_CmpMemPoolMemPoolAppendUsedBlockToPool (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolAppendUsedBlockToPool  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolAppendUsedBlockToPool
	#define EXT_MemPoolAppendUsedBlockToPool
	#define GET_MemPoolAppendUsedBlockToPool(fl)  CAL_CMGETAPI( "MemPoolAppendUsedBlockToPool" ) 
	#define CAL_MemPoolAppendUsedBlockToPool pICmpMemPool->IMemPoolAppendUsedBlockToPool
	#define CHK_MemPoolAppendUsedBlockToPool (pICmpMemPool != NULL)
	#define EXP_MemPoolAppendUsedBlockToPool  CAL_CMEXPAPI( "MemPoolAppendUsedBlockToPool" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolAppendUsedBlockToPool  PFMEMPOOLAPPENDUSEDBLOCKTOPOOL pfMemPoolAppendUsedBlockToPool;
	#define EXT_MemPoolAppendUsedBlockToPool  extern PFMEMPOOLAPPENDUSEDBLOCKTOPOOL pfMemPoolAppendUsedBlockToPool;
	#define GET_MemPoolAppendUsedBlockToPool(fl)  s_pfCMGetAPI2( "MemPoolAppendUsedBlockToPool", (RTS_VOID_FCTPTR *)&pfMemPoolAppendUsedBlockToPool, (fl), 0, 0)
	#define CAL_MemPoolAppendUsedBlockToPool  pfMemPoolAppendUsedBlockToPool
	#define CHK_MemPoolAppendUsedBlockToPool  (pfMemPoolAppendUsedBlockToPool != NULL)
	#define EXP_MemPoolAppendUsedBlockToPool  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolAppendUsedBlockToPool", (RTS_UINTPTR)MemPoolAppendUsedBlockToPool, 0, 0) 
#endif




/**
 * <description>
 *	Remove used block from the specified pool. Now, the block is removed from the chain list of used blocks.
 * </description>
 * <param name="pBlock" type="IN">Pointer to the memory block</param>
 * <param name="hPool" type="IN">Handle to the pool</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successfully removed block from MemPool</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Could not remove block from MemPool, wrong pBlock</errorcode> 
 */
RTS_RESULT CDECL MemPoolRemoveUsedBlockFromPool(void* pBlock, RTS_HANDLE hPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLREMOVEUSEDBLOCKFROMPOOL) (void* pBlock, RTS_HANDLE hPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLREMOVEUSEDBLOCKFROMPOOL_NOTIMPLEMENTED)
	#define USE_MemPoolRemoveUsedBlockFromPool
	#define EXT_MemPoolRemoveUsedBlockFromPool
	#define GET_MemPoolRemoveUsedBlockFromPool(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolRemoveUsedBlockFromPool(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolRemoveUsedBlockFromPool  FALSE
	#define EXP_MemPoolRemoveUsedBlockFromPool  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolRemoveUsedBlockFromPool
	#define EXT_MemPoolRemoveUsedBlockFromPool
	#define GET_MemPoolRemoveUsedBlockFromPool(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlockFromPool" ) 
	#define CAL_MemPoolRemoveUsedBlockFromPool  MemPoolRemoveUsedBlockFromPool
	#define CHK_MemPoolRemoveUsedBlockFromPool  TRUE
	#define EXP_MemPoolRemoveUsedBlockFromPool  CAL_CMEXPAPI( "MemPoolRemoveUsedBlockFromPool" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolRemoveUsedBlockFromPool
	#define EXT_MemPoolRemoveUsedBlockFromPool
	#define GET_MemPoolRemoveUsedBlockFromPool(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlockFromPool" ) 
	#define CAL_MemPoolRemoveUsedBlockFromPool  MemPoolRemoveUsedBlockFromPool
	#define CHK_MemPoolRemoveUsedBlockFromPool  TRUE
	#define EXP_MemPoolRemoveUsedBlockFromPool  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolRemoveUsedBlockFromPool", (RTS_UINTPTR)MemPoolRemoveUsedBlockFromPool, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolRemoveUsedBlockFromPool
	#define EXT_CmpMemPoolMemPoolRemoveUsedBlockFromPool
	#define GET_CmpMemPoolMemPoolRemoveUsedBlockFromPool  ERR_OK
	#define CAL_CmpMemPoolMemPoolRemoveUsedBlockFromPool pICmpMemPool->IMemPoolRemoveUsedBlockFromPool
	#define CHK_CmpMemPoolMemPoolRemoveUsedBlockFromPool (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolRemoveUsedBlockFromPool  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolRemoveUsedBlockFromPool
	#define EXT_MemPoolRemoveUsedBlockFromPool
	#define GET_MemPoolRemoveUsedBlockFromPool(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlockFromPool" ) 
	#define CAL_MemPoolRemoveUsedBlockFromPool pICmpMemPool->IMemPoolRemoveUsedBlockFromPool
	#define CHK_MemPoolRemoveUsedBlockFromPool (pICmpMemPool != NULL)
	#define EXP_MemPoolRemoveUsedBlockFromPool  CAL_CMEXPAPI( "MemPoolRemoveUsedBlockFromPool" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolRemoveUsedBlockFromPool  PFMEMPOOLREMOVEUSEDBLOCKFROMPOOL pfMemPoolRemoveUsedBlockFromPool;
	#define EXT_MemPoolRemoveUsedBlockFromPool  extern PFMEMPOOLREMOVEUSEDBLOCKFROMPOOL pfMemPoolRemoveUsedBlockFromPool;
	#define GET_MemPoolRemoveUsedBlockFromPool(fl)  s_pfCMGetAPI2( "MemPoolRemoveUsedBlockFromPool", (RTS_VOID_FCTPTR *)&pfMemPoolRemoveUsedBlockFromPool, (fl), 0, 0)
	#define CAL_MemPoolRemoveUsedBlockFromPool  pfMemPoolRemoveUsedBlockFromPool
	#define CHK_MemPoolRemoveUsedBlockFromPool  (pfMemPoolRemoveUsedBlockFromPool != NULL)
	#define EXP_MemPoolRemoveUsedBlockFromPool  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolRemoveUsedBlockFromPool", (RTS_UINTPTR)MemPoolRemoveUsedBlockFromPool, 0, 0) 
#endif




/**
 * <description>
 *	Lock the access to a pool to be thread safe.
 * </description>
 * <param name="pBlock" type="IN">Pointer to the memory block</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL MemPoolLockBlock(void* pBlock);
typedef RTS_RESULT (CDECL * PFMEMPOOLLOCKBLOCK) (void* pBlock);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLLOCKBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolLockBlock
	#define EXT_MemPoolLockBlock
	#define GET_MemPoolLockBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolLockBlock(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolLockBlock  FALSE
	#define EXP_MemPoolLockBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolLockBlock
	#define EXT_MemPoolLockBlock
	#define GET_MemPoolLockBlock(fl)  CAL_CMGETAPI( "MemPoolLockBlock" ) 
	#define CAL_MemPoolLockBlock  MemPoolLockBlock
	#define CHK_MemPoolLockBlock  TRUE
	#define EXP_MemPoolLockBlock  CAL_CMEXPAPI( "MemPoolLockBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolLockBlock
	#define EXT_MemPoolLockBlock
	#define GET_MemPoolLockBlock(fl)  CAL_CMGETAPI( "MemPoolLockBlock" ) 
	#define CAL_MemPoolLockBlock  MemPoolLockBlock
	#define CHK_MemPoolLockBlock  TRUE
	#define EXP_MemPoolLockBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolLockBlock", (RTS_UINTPTR)MemPoolLockBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolLockBlock
	#define EXT_CmpMemPoolMemPoolLockBlock
	#define GET_CmpMemPoolMemPoolLockBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolLockBlock pICmpMemPool->IMemPoolLockBlock
	#define CHK_CmpMemPoolMemPoolLockBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolLockBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolLockBlock
	#define EXT_MemPoolLockBlock
	#define GET_MemPoolLockBlock(fl)  CAL_CMGETAPI( "MemPoolLockBlock" ) 
	#define CAL_MemPoolLockBlock pICmpMemPool->IMemPoolLockBlock
	#define CHK_MemPoolLockBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolLockBlock  CAL_CMEXPAPI( "MemPoolLockBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolLockBlock  PFMEMPOOLLOCKBLOCK pfMemPoolLockBlock;
	#define EXT_MemPoolLockBlock  extern PFMEMPOOLLOCKBLOCK pfMemPoolLockBlock;
	#define GET_MemPoolLockBlock(fl)  s_pfCMGetAPI2( "MemPoolLockBlock", (RTS_VOID_FCTPTR *)&pfMemPoolLockBlock, (fl), 0, 0)
	#define CAL_MemPoolLockBlock  pfMemPoolLockBlock
	#define CHK_MemPoolLockBlock  (pfMemPoolLockBlock != NULL)
	#define EXP_MemPoolLockBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolLockBlock", (RTS_UINTPTR)MemPoolLockBlock, 0, 0) 
#endif




/**
 * <description>
 *	Unlock the access to a pool.
 * </description>
 * <param name="pBlock" type="IN">Pointer to the memory block</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL MemPoolUnlockBlock(void* pBlock);
typedef RTS_RESULT (CDECL * PFMEMPOOLUNLOCKBLOCK) (void* pBlock);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLUNLOCKBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolUnlockBlock
	#define EXT_MemPoolUnlockBlock
	#define GET_MemPoolUnlockBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolUnlockBlock(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolUnlockBlock  FALSE
	#define EXP_MemPoolUnlockBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolUnlockBlock
	#define EXT_MemPoolUnlockBlock
	#define GET_MemPoolUnlockBlock(fl)  CAL_CMGETAPI( "MemPoolUnlockBlock" ) 
	#define CAL_MemPoolUnlockBlock  MemPoolUnlockBlock
	#define CHK_MemPoolUnlockBlock  TRUE
	#define EXP_MemPoolUnlockBlock  CAL_CMEXPAPI( "MemPoolUnlockBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolUnlockBlock
	#define EXT_MemPoolUnlockBlock
	#define GET_MemPoolUnlockBlock(fl)  CAL_CMGETAPI( "MemPoolUnlockBlock" ) 
	#define CAL_MemPoolUnlockBlock  MemPoolUnlockBlock
	#define CHK_MemPoolUnlockBlock  TRUE
	#define EXP_MemPoolUnlockBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolUnlockBlock", (RTS_UINTPTR)MemPoolUnlockBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolUnlockBlock
	#define EXT_CmpMemPoolMemPoolUnlockBlock
	#define GET_CmpMemPoolMemPoolUnlockBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolUnlockBlock pICmpMemPool->IMemPoolUnlockBlock
	#define CHK_CmpMemPoolMemPoolUnlockBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolUnlockBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolUnlockBlock
	#define EXT_MemPoolUnlockBlock
	#define GET_MemPoolUnlockBlock(fl)  CAL_CMGETAPI( "MemPoolUnlockBlock" ) 
	#define CAL_MemPoolUnlockBlock pICmpMemPool->IMemPoolUnlockBlock
	#define CHK_MemPoolUnlockBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolUnlockBlock  CAL_CMEXPAPI( "MemPoolUnlockBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolUnlockBlock  PFMEMPOOLUNLOCKBLOCK pfMemPoolUnlockBlock;
	#define EXT_MemPoolUnlockBlock  extern PFMEMPOOLUNLOCKBLOCK pfMemPoolUnlockBlock;
	#define GET_MemPoolUnlockBlock(fl)  s_pfCMGetAPI2( "MemPoolUnlockBlock", (RTS_VOID_FCTPTR *)&pfMemPoolUnlockBlock, (fl), 0, 0)
	#define CAL_MemPoolUnlockBlock  pfMemPoolUnlockBlock
	#define CHK_MemPoolUnlockBlock  (pfMemPoolUnlockBlock != NULL)
	#define EXP_MemPoolUnlockBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolUnlockBlock", (RTS_UINTPTR)MemPoolUnlockBlock, 0, 0) 
#endif




/**
 * <description>
 *	Function to release all blocks within a pool or only the blocks that are marked as "to delete". This behavior can be controlled with the parameter bOnlyDeletedBlocks.
 *  If a delete function was specified in MemPoolCreate(), this function is called at deleting the appropriate block and the delete function gets the pParam parameter!
 * </description>
 * <param name="hMemPool" type="IN" range="[VALID_MEMPOOLHANDLE,NULL]">Handle to the memory pool</param>
 * <param name="bOnlyDeletedBlocks" type="IN" range="[TRUE,FALSE]">TRUE=Release only blocks, which are marked as to delete. FALSE=Release all blocks within a pool</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Blocks in the MemPool are successfully release</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Pool handle is invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PENDING">Not all blocks could be deleted because the pool is still occupied by another iterator!</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">Release the pool could not be done before CH_INIT_SYSTEM. Not relevant for SIL2</errorcode>
 */
RTS_RESULT CDECL MemPoolReleaseBlocks(RTS_HANDLE hMemPool, RTS_BOOL bOnlyDeletedBlocks, void *pParam);
typedef RTS_RESULT (CDECL * PFMEMPOOLRELEASEBLOCKS) (RTS_HANDLE hMemPool, RTS_BOOL bOnlyDeletedBlocks, void *pParam);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLRELEASEBLOCKS_NOTIMPLEMENTED)
	#define USE_MemPoolReleaseBlocks
	#define EXT_MemPoolReleaseBlocks
	#define GET_MemPoolReleaseBlocks(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolReleaseBlocks(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolReleaseBlocks  FALSE
	#define EXP_MemPoolReleaseBlocks  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolReleaseBlocks
	#define EXT_MemPoolReleaseBlocks
	#define GET_MemPoolReleaseBlocks(fl)  CAL_CMGETAPI( "MemPoolReleaseBlocks" ) 
	#define CAL_MemPoolReleaseBlocks  MemPoolReleaseBlocks
	#define CHK_MemPoolReleaseBlocks  TRUE
	#define EXP_MemPoolReleaseBlocks  CAL_CMEXPAPI( "MemPoolReleaseBlocks" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolReleaseBlocks
	#define EXT_MemPoolReleaseBlocks
	#define GET_MemPoolReleaseBlocks(fl)  CAL_CMGETAPI( "MemPoolReleaseBlocks" ) 
	#define CAL_MemPoolReleaseBlocks  MemPoolReleaseBlocks
	#define CHK_MemPoolReleaseBlocks  TRUE
	#define EXP_MemPoolReleaseBlocks  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolReleaseBlocks", (RTS_UINTPTR)MemPoolReleaseBlocks, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolReleaseBlocks
	#define EXT_CmpMemPoolMemPoolReleaseBlocks
	#define GET_CmpMemPoolMemPoolReleaseBlocks  ERR_OK
	#define CAL_CmpMemPoolMemPoolReleaseBlocks pICmpMemPool->IMemPoolReleaseBlocks
	#define CHK_CmpMemPoolMemPoolReleaseBlocks (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolReleaseBlocks  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolReleaseBlocks
	#define EXT_MemPoolReleaseBlocks
	#define GET_MemPoolReleaseBlocks(fl)  CAL_CMGETAPI( "MemPoolReleaseBlocks" ) 
	#define CAL_MemPoolReleaseBlocks pICmpMemPool->IMemPoolReleaseBlocks
	#define CHK_MemPoolReleaseBlocks (pICmpMemPool != NULL)
	#define EXP_MemPoolReleaseBlocks  CAL_CMEXPAPI( "MemPoolReleaseBlocks" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolReleaseBlocks  PFMEMPOOLRELEASEBLOCKS pfMemPoolReleaseBlocks;
	#define EXT_MemPoolReleaseBlocks  extern PFMEMPOOLRELEASEBLOCKS pfMemPoolReleaseBlocks;
	#define GET_MemPoolReleaseBlocks(fl)  s_pfCMGetAPI2( "MemPoolReleaseBlocks", (RTS_VOID_FCTPTR *)&pfMemPoolReleaseBlocks, (fl), 0, 0)
	#define CAL_MemPoolReleaseBlocks  pfMemPoolReleaseBlocks
	#define CHK_MemPoolReleaseBlocks  (pfMemPoolReleaseBlocks != NULL)
	#define EXP_MemPoolReleaseBlocks  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolReleaseBlocks", (RTS_UINTPTR)MemPoolReleaseBlocks, 0, 0) 
#endif




/**
 * <description>
 *	Lock the access to the complete pool. SIL2 Implementation is using INT Locks.
 * </description>
 * <param name="hMemPool" type="IN" range="[VALID_MEMPOOLHANDLE,NULL]">Handle to the memory pool</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Pool was successfully locked</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Parameter is wrong</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Pool could not be locked, SysInt or SysSem had problems</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTIMPLEMENTED">SysInt or SysSem are not implemented</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">Lock cannot be done before CH_INIT_SYSTEM, not relevant for SIL2</errorcode>
 */
RTS_RESULT CDECL MemPoolLock(RTS_HANDLE hMemPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLLOCK) (RTS_HANDLE hMemPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolLock
	#define EXT_MemPoolLock
	#define GET_MemPoolLock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolLock(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolLock  FALSE
	#define EXP_MemPoolLock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolLock
	#define EXT_MemPoolLock
	#define GET_MemPoolLock(fl)  CAL_CMGETAPI( "MemPoolLock" ) 
	#define CAL_MemPoolLock  MemPoolLock
	#define CHK_MemPoolLock  TRUE
	#define EXP_MemPoolLock  CAL_CMEXPAPI( "MemPoolLock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolLock
	#define EXT_MemPoolLock
	#define GET_MemPoolLock(fl)  CAL_CMGETAPI( "MemPoolLock" ) 
	#define CAL_MemPoolLock  MemPoolLock
	#define CHK_MemPoolLock  TRUE
	#define EXP_MemPoolLock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolLock", (RTS_UINTPTR)MemPoolLock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolLock
	#define EXT_CmpMemPoolMemPoolLock
	#define GET_CmpMemPoolMemPoolLock  ERR_OK
	#define CAL_CmpMemPoolMemPoolLock pICmpMemPool->IMemPoolLock
	#define CHK_CmpMemPoolMemPoolLock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolLock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolLock
	#define EXT_MemPoolLock
	#define GET_MemPoolLock(fl)  CAL_CMGETAPI( "MemPoolLock" ) 
	#define CAL_MemPoolLock pICmpMemPool->IMemPoolLock
	#define CHK_MemPoolLock (pICmpMemPool != NULL)
	#define EXP_MemPoolLock  CAL_CMEXPAPI( "MemPoolLock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolLock  PFMEMPOOLLOCK pfMemPoolLock;
	#define EXT_MemPoolLock  extern PFMEMPOOLLOCK pfMemPoolLock;
	#define GET_MemPoolLock(fl)  s_pfCMGetAPI2( "MemPoolLock", (RTS_VOID_FCTPTR *)&pfMemPoolLock, (fl), 0, 0)
	#define CAL_MemPoolLock  pfMemPoolLock
	#define CHK_MemPoolLock  (pfMemPoolLock != NULL)
	#define EXP_MemPoolLock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolLock", (RTS_UINTPTR)MemPoolLock, 0, 0) 
#endif




/**
 * <description>
 *	Unlock the access to the complete pool. SIL2 Implementation is using INT Locks.
 * </description>
 * <param name="hMemPool" type="IN" range="[VALID_MEMPOOLHANDLE,NULL]">Handle to the memory pool</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Pool was successfully unlocked</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Parameter is wrong</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Pool could not be unlocked, SysInt or SysSem had problems</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTIMPLEMENTED">SysInt or SysSem are not implemented</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">Unlock cannot be done before CH_INIT_SYSTEM, not relevant for SIL2</errorcode>
 */
RTS_RESULT CDECL MemPoolUnlock(RTS_HANDLE hMemPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLUNLOCK) (RTS_HANDLE hMemPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLUNLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolUnlock
	#define EXT_MemPoolUnlock
	#define GET_MemPoolUnlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolUnlock(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolUnlock  FALSE
	#define EXP_MemPoolUnlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolUnlock
	#define EXT_MemPoolUnlock
	#define GET_MemPoolUnlock(fl)  CAL_CMGETAPI( "MemPoolUnlock" ) 
	#define CAL_MemPoolUnlock  MemPoolUnlock
	#define CHK_MemPoolUnlock  TRUE
	#define EXP_MemPoolUnlock  CAL_CMEXPAPI( "MemPoolUnlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolUnlock
	#define EXT_MemPoolUnlock
	#define GET_MemPoolUnlock(fl)  CAL_CMGETAPI( "MemPoolUnlock" ) 
	#define CAL_MemPoolUnlock  MemPoolUnlock
	#define CHK_MemPoolUnlock  TRUE
	#define EXP_MemPoolUnlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolUnlock", (RTS_UINTPTR)MemPoolUnlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolUnlock
	#define EXT_CmpMemPoolMemPoolUnlock
	#define GET_CmpMemPoolMemPoolUnlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolUnlock pICmpMemPool->IMemPoolUnlock
	#define CHK_CmpMemPoolMemPoolUnlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolUnlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolUnlock
	#define EXT_MemPoolUnlock
	#define GET_MemPoolUnlock(fl)  CAL_CMGETAPI( "MemPoolUnlock" ) 
	#define CAL_MemPoolUnlock pICmpMemPool->IMemPoolUnlock
	#define CHK_MemPoolUnlock (pICmpMemPool != NULL)
	#define EXP_MemPoolUnlock  CAL_CMEXPAPI( "MemPoolUnlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolUnlock  PFMEMPOOLUNLOCK pfMemPoolUnlock;
	#define EXT_MemPoolUnlock  extern PFMEMPOOLUNLOCK pfMemPoolUnlock;
	#define GET_MemPoolUnlock(fl)  s_pfCMGetAPI2( "MemPoolUnlock", (RTS_VOID_FCTPTR *)&pfMemPoolUnlock, (fl), 0, 0)
	#define CAL_MemPoolUnlock  pfMemPoolUnlock
	#define CHK_MemPoolUnlock  (pfMemPoolUnlock != NULL)
	#define EXP_MemPoolUnlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolUnlock", (RTS_UINTPTR)MemPoolUnlock, 0, 0) 
#endif




/**
 * <description>
 *	Find a block specified by a value, that is stored in the block.
 *  NOTE:
 *	Please use MemPoolLock or MemPoolAddRef_LF before calling this function, because block is returned unsynchronized against the pool!
 * </description>
 * <param name="hMemPool" type="IN">Handle to the memory pool</param>
 * <param name="ulOffset" type="IN">Byte offset of the value in the block to find</param>
 * <param name="ulSize" type="IN">Size in bytes of the value to find in the block</param>
 * <param name="pToFind" type="IN">Pointer to the value to find in the block</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Pointer to the memory block</result>
 */
void * CDECL MemPoolFindBlock(RTS_HANDLE hMemPool, RTS_SIZE ulOffset, RTS_SIZE ulSize, void *pToFind, RTS_RESULT *pResult);
typedef void * (CDECL * PFMEMPOOLFINDBLOCK) (RTS_HANDLE hMemPool, RTS_SIZE ulOffset, RTS_SIZE ulSize, void *pToFind, RTS_RESULT *pResult);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLFINDBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolFindBlock
	#define EXT_MemPoolFindBlock
	#define GET_MemPoolFindBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolFindBlock(p0,p1,p2,p3,p4)  (void *)(RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolFindBlock  FALSE
	#define EXP_MemPoolFindBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolFindBlock
	#define EXT_MemPoolFindBlock
	#define GET_MemPoolFindBlock(fl)  CAL_CMGETAPI( "MemPoolFindBlock" ) 
	#define CAL_MemPoolFindBlock  MemPoolFindBlock
	#define CHK_MemPoolFindBlock  TRUE
	#define EXP_MemPoolFindBlock  CAL_CMEXPAPI( "MemPoolFindBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolFindBlock
	#define EXT_MemPoolFindBlock
	#define GET_MemPoolFindBlock(fl)  CAL_CMGETAPI( "MemPoolFindBlock" ) 
	#define CAL_MemPoolFindBlock  MemPoolFindBlock
	#define CHK_MemPoolFindBlock  TRUE
	#define EXP_MemPoolFindBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolFindBlock", (RTS_UINTPTR)MemPoolFindBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolFindBlock
	#define EXT_CmpMemPoolMemPoolFindBlock
	#define GET_CmpMemPoolMemPoolFindBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolFindBlock pICmpMemPool->IMemPoolFindBlock
	#define CHK_CmpMemPoolMemPoolFindBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolFindBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolFindBlock
	#define EXT_MemPoolFindBlock
	#define GET_MemPoolFindBlock(fl)  CAL_CMGETAPI( "MemPoolFindBlock" ) 
	#define CAL_MemPoolFindBlock pICmpMemPool->IMemPoolFindBlock
	#define CHK_MemPoolFindBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolFindBlock  CAL_CMEXPAPI( "MemPoolFindBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolFindBlock  PFMEMPOOLFINDBLOCK pfMemPoolFindBlock;
	#define EXT_MemPoolFindBlock  extern PFMEMPOOLFINDBLOCK pfMemPoolFindBlock;
	#define GET_MemPoolFindBlock(fl)  s_pfCMGetAPI2( "MemPoolFindBlock", (RTS_VOID_FCTPTR *)&pfMemPoolFindBlock, (fl), 0, 0)
	#define CAL_MemPoolFindBlock  pfMemPoolFindBlock
	#define CHK_MemPoolFindBlock  (pfMemPoolFindBlock != NULL)
	#define EXP_MemPoolFindBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolFindBlock", (RTS_UINTPTR)MemPoolFindBlock, 0, 0) 
#endif




/**
 * <description>
 *	Check a pool memory block, if it is still valid and is not released.
 *	NOTE:
 *	If the check is successful, a lock is done on this pool!!! So you have to unlock this reference at the end of the usage with MemPoolUnlock()!
 * </description>
 * <param name="hMemPool" type="IN">Handle to the memory pool</param>
 * <param name="pBlock" type="IN">Pointer to the memory block</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL MemPoolIsValidBlock(RTS_HANDLE hMemPool, void *pBlock);
typedef RTS_RESULT (CDECL * PFMEMPOOLISVALIDBLOCK) (RTS_HANDLE hMemPool, void *pBlock);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLISVALIDBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolIsValidBlock
	#define EXT_MemPoolIsValidBlock
	#define GET_MemPoolIsValidBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolIsValidBlock(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolIsValidBlock  FALSE
	#define EXP_MemPoolIsValidBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolIsValidBlock
	#define EXT_MemPoolIsValidBlock
	#define GET_MemPoolIsValidBlock(fl)  CAL_CMGETAPI( "MemPoolIsValidBlock" ) 
	#define CAL_MemPoolIsValidBlock  MemPoolIsValidBlock
	#define CHK_MemPoolIsValidBlock  TRUE
	#define EXP_MemPoolIsValidBlock  CAL_CMEXPAPI( "MemPoolIsValidBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolIsValidBlock
	#define EXT_MemPoolIsValidBlock
	#define GET_MemPoolIsValidBlock(fl)  CAL_CMGETAPI( "MemPoolIsValidBlock" ) 
	#define CAL_MemPoolIsValidBlock  MemPoolIsValidBlock
	#define CHK_MemPoolIsValidBlock  TRUE
	#define EXP_MemPoolIsValidBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolIsValidBlock", (RTS_UINTPTR)MemPoolIsValidBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolIsValidBlock
	#define EXT_CmpMemPoolMemPoolIsValidBlock
	#define GET_CmpMemPoolMemPoolIsValidBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolIsValidBlock pICmpMemPool->IMemPoolIsValidBlock
	#define CHK_CmpMemPoolMemPoolIsValidBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolIsValidBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolIsValidBlock
	#define EXT_MemPoolIsValidBlock
	#define GET_MemPoolIsValidBlock(fl)  CAL_CMGETAPI( "MemPoolIsValidBlock" ) 
	#define CAL_MemPoolIsValidBlock pICmpMemPool->IMemPoolIsValidBlock
	#define CHK_MemPoolIsValidBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolIsValidBlock  CAL_CMEXPAPI( "MemPoolIsValidBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolIsValidBlock  PFMEMPOOLISVALIDBLOCK pfMemPoolIsValidBlock;
	#define EXT_MemPoolIsValidBlock  extern PFMEMPOOLISVALIDBLOCK pfMemPoolIsValidBlock;
	#define GET_MemPoolIsValidBlock(fl)  s_pfCMGetAPI2( "MemPoolIsValidBlock", (RTS_VOID_FCTPTR *)&pfMemPoolIsValidBlock, (fl), 0, 0)
	#define CAL_MemPoolIsValidBlock  pfMemPoolIsValidBlock
	#define CHK_MemPoolIsValidBlock  (pfMemPoolIsValidBlock != NULL)
	#define EXP_MemPoolIsValidBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolIsValidBlock", (RTS_UINTPTR)MemPoolIsValidBlock, 0, 0) 
#endif




/**
 * <description>
 *	Get the first memory block out of the pool. Can be used for explicit iteration routines.
 * </description>
 * <param name="hMemPool" type="IN">Handle to the memory pool</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Pointer to first memory block</result>
 */
void* CDECL MemPoolGetFirstBlock(RTS_HANDLE hMemPool, RTS_RESULT *pResult);
typedef void* (CDECL * PFMEMPOOLGETFIRSTBLOCK) (RTS_HANDLE hMemPool, RTS_RESULT *pResult);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLGETFIRSTBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolGetFirstBlock
	#define EXT_MemPoolGetFirstBlock
	#define GET_MemPoolGetFirstBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolGetFirstBlock(p0,p1)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolGetFirstBlock  FALSE
	#define EXP_MemPoolGetFirstBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolGetFirstBlock
	#define EXT_MemPoolGetFirstBlock
	#define GET_MemPoolGetFirstBlock(fl)  CAL_CMGETAPI( "MemPoolGetFirstBlock" ) 
	#define CAL_MemPoolGetFirstBlock  MemPoolGetFirstBlock
	#define CHK_MemPoolGetFirstBlock  TRUE
	#define EXP_MemPoolGetFirstBlock  CAL_CMEXPAPI( "MemPoolGetFirstBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolGetFirstBlock
	#define EXT_MemPoolGetFirstBlock
	#define GET_MemPoolGetFirstBlock(fl)  CAL_CMGETAPI( "MemPoolGetFirstBlock" ) 
	#define CAL_MemPoolGetFirstBlock  MemPoolGetFirstBlock
	#define CHK_MemPoolGetFirstBlock  TRUE
	#define EXP_MemPoolGetFirstBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolGetFirstBlock", (RTS_UINTPTR)MemPoolGetFirstBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolGetFirstBlock
	#define EXT_CmpMemPoolMemPoolGetFirstBlock
	#define GET_CmpMemPoolMemPoolGetFirstBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolGetFirstBlock pICmpMemPool->IMemPoolGetFirstBlock
	#define CHK_CmpMemPoolMemPoolGetFirstBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolGetFirstBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolGetFirstBlock
	#define EXT_MemPoolGetFirstBlock
	#define GET_MemPoolGetFirstBlock(fl)  CAL_CMGETAPI( "MemPoolGetFirstBlock" ) 
	#define CAL_MemPoolGetFirstBlock pICmpMemPool->IMemPoolGetFirstBlock
	#define CHK_MemPoolGetFirstBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolGetFirstBlock  CAL_CMEXPAPI( "MemPoolGetFirstBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolGetFirstBlock  PFMEMPOOLGETFIRSTBLOCK pfMemPoolGetFirstBlock;
	#define EXT_MemPoolGetFirstBlock  extern PFMEMPOOLGETFIRSTBLOCK pfMemPoolGetFirstBlock;
	#define GET_MemPoolGetFirstBlock(fl)  s_pfCMGetAPI2( "MemPoolGetFirstBlock", (RTS_VOID_FCTPTR *)&pfMemPoolGetFirstBlock, (fl), 0, 0)
	#define CAL_MemPoolGetFirstBlock  pfMemPoolGetFirstBlock
	#define CHK_MemPoolGetFirstBlock  (pfMemPoolGetFirstBlock != NULL)
	#define EXP_MemPoolGetFirstBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolGetFirstBlock", (RTS_UINTPTR)MemPoolGetFirstBlock, 0, 0) 
#endif




/**
 * <description>
 *	Get the next memory block out of the pool. Can be used for explicit iteration routines.
 * </description>
 * <param name="hMemPool" type="IN">Handle to the memory pool</param>
 * <param name="pPrevBlock" type="IN">Pointer to previous memory block</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Pointer to next memory block</result>
 */
void* CDECL MemPoolGetNextBlock(RTS_HANDLE hMemPool, void *pPrevBlock, RTS_RESULT *pResult);
typedef void* (CDECL * PFMEMPOOLGETNEXTBLOCK) (RTS_HANDLE hMemPool, void *pPrevBlock, RTS_RESULT *pResult);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLGETNEXTBLOCK_NOTIMPLEMENTED)
	#define USE_MemPoolGetNextBlock
	#define EXT_MemPoolGetNextBlock
	#define GET_MemPoolGetNextBlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolGetNextBlock(p0,p1,p2)  (void*)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolGetNextBlock  FALSE
	#define EXP_MemPoolGetNextBlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolGetNextBlock
	#define EXT_MemPoolGetNextBlock
	#define GET_MemPoolGetNextBlock(fl)  CAL_CMGETAPI( "MemPoolGetNextBlock" ) 
	#define CAL_MemPoolGetNextBlock  MemPoolGetNextBlock
	#define CHK_MemPoolGetNextBlock  TRUE
	#define EXP_MemPoolGetNextBlock  CAL_CMEXPAPI( "MemPoolGetNextBlock" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolGetNextBlock
	#define EXT_MemPoolGetNextBlock
	#define GET_MemPoolGetNextBlock(fl)  CAL_CMGETAPI( "MemPoolGetNextBlock" ) 
	#define CAL_MemPoolGetNextBlock  MemPoolGetNextBlock
	#define CHK_MemPoolGetNextBlock  TRUE
	#define EXP_MemPoolGetNextBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolGetNextBlock", (RTS_UINTPTR)MemPoolGetNextBlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolGetNextBlock
	#define EXT_CmpMemPoolMemPoolGetNextBlock
	#define GET_CmpMemPoolMemPoolGetNextBlock  ERR_OK
	#define CAL_CmpMemPoolMemPoolGetNextBlock pICmpMemPool->IMemPoolGetNextBlock
	#define CHK_CmpMemPoolMemPoolGetNextBlock (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolGetNextBlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolGetNextBlock
	#define EXT_MemPoolGetNextBlock
	#define GET_MemPoolGetNextBlock(fl)  CAL_CMGETAPI( "MemPoolGetNextBlock" ) 
	#define CAL_MemPoolGetNextBlock pICmpMemPool->IMemPoolGetNextBlock
	#define CHK_MemPoolGetNextBlock (pICmpMemPool != NULL)
	#define EXP_MemPoolGetNextBlock  CAL_CMEXPAPI( "MemPoolGetNextBlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolGetNextBlock  PFMEMPOOLGETNEXTBLOCK pfMemPoolGetNextBlock;
	#define EXT_MemPoolGetNextBlock  extern PFMEMPOOLGETNEXTBLOCK pfMemPoolGetNextBlock;
	#define GET_MemPoolGetNextBlock(fl)  s_pfCMGetAPI2( "MemPoolGetNextBlock", (RTS_VOID_FCTPTR *)&pfMemPoolGetNextBlock, (fl), 0, 0)
	#define CAL_MemPoolGetNextBlock  pfMemPoolGetNextBlock
	#define CHK_MemPoolGetNextBlock  (pfMemPoolGetNextBlock != NULL)
	#define EXP_MemPoolGetNextBlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolGetNextBlock", (RTS_UINTPTR)MemPoolGetNextBlock, 0, 0) 
#endif





/*==== MemPool operations based on reference counter on the memory pool to realize nearly locking free iterators (function postfix "_LF") ===========================*/

/**
 * <description>
 *	To have a locking free iteration over a MemPool, you can use this AddRef function to occupy a MemPool!
 *	After calling this function, no element can be deleted out of the list of the MemPool!
 *
 *  NOTES:
 *	1. Please use the appropriate macro MEM_GET_DATA_SKIP_DELETE() to get the data of all blocks, which are not marked as deleted! So you can skip this block and you should 
 *	   iterate to the next blocks.
 *	2. Be aware, that a block cannot be removed from pool (via CAL_MemPoolRemoveUsedBlock_LF() after calling CAL_MemPoolAddRef_LF()!
 *	   You have call CAL_MemPoolReleaseRef_LF() first and then you are able to remove the block or use CAL_MemPoolRemoveUsedBlock() alternatively!
 *
 * </description>
 * <param name="hMemPool" type="IN" range="[VALID_MEMPOOLHANDLE,NULL]">Handle to the memory pool</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Pool was successfully occupied</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Pool handle is invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">Occupying the pool could not be done before CH_INIT_SYSTEM. Not relevant for SIL2</errorcode>
 */
RTS_RESULT CDECL MemPoolAddRef_LF(RTS_HANDLE hMemPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLADDREF_LF) (RTS_HANDLE hMemPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLADDREF_LF_NOTIMPLEMENTED)
	#define USE_MemPoolAddRef_LF
	#define EXT_MemPoolAddRef_LF
	#define GET_MemPoolAddRef_LF(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolAddRef_LF(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolAddRef_LF  FALSE
	#define EXP_MemPoolAddRef_LF  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolAddRef_LF
	#define EXT_MemPoolAddRef_LF
	#define GET_MemPoolAddRef_LF(fl)  CAL_CMGETAPI( "MemPoolAddRef_LF" ) 
	#define CAL_MemPoolAddRef_LF  MemPoolAddRef_LF
	#define CHK_MemPoolAddRef_LF  TRUE
	#define EXP_MemPoolAddRef_LF  CAL_CMEXPAPI( "MemPoolAddRef_LF" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolAddRef_LF
	#define EXT_MemPoolAddRef_LF
	#define GET_MemPoolAddRef_LF(fl)  CAL_CMGETAPI( "MemPoolAddRef_LF" ) 
	#define CAL_MemPoolAddRef_LF  MemPoolAddRef_LF
	#define CHK_MemPoolAddRef_LF  TRUE
	#define EXP_MemPoolAddRef_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolAddRef_LF", (RTS_UINTPTR)MemPoolAddRef_LF, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolAddRef_LF
	#define EXT_CmpMemPoolMemPoolAddRef_LF
	#define GET_CmpMemPoolMemPoolAddRef_LF  ERR_OK
	#define CAL_CmpMemPoolMemPoolAddRef_LF pICmpMemPool->IMemPoolAddRef_LF
	#define CHK_CmpMemPoolMemPoolAddRef_LF (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolAddRef_LF  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolAddRef_LF
	#define EXT_MemPoolAddRef_LF
	#define GET_MemPoolAddRef_LF(fl)  CAL_CMGETAPI( "MemPoolAddRef_LF" ) 
	#define CAL_MemPoolAddRef_LF pICmpMemPool->IMemPoolAddRef_LF
	#define CHK_MemPoolAddRef_LF (pICmpMemPool != NULL)
	#define EXP_MemPoolAddRef_LF  CAL_CMEXPAPI( "MemPoolAddRef_LF" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolAddRef_LF  PFMEMPOOLADDREF_LF pfMemPoolAddRef_LF;
	#define EXT_MemPoolAddRef_LF  extern PFMEMPOOLADDREF_LF pfMemPoolAddRef_LF;
	#define GET_MemPoolAddRef_LF(fl)  s_pfCMGetAPI2( "MemPoolAddRef_LF", (RTS_VOID_FCTPTR *)&pfMemPoolAddRef_LF, (fl), 0, 0)
	#define CAL_MemPoolAddRef_LF  pfMemPoolAddRef_LF
	#define CHK_MemPoolAddRef_LF  (pfMemPoolAddRef_LF != NULL)
	#define EXP_MemPoolAddRef_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolAddRef_LF", (RTS_UINTPTR)MemPoolAddRef_LF, 0, 0) 
#endif




/**
 * <description>
 *	To have a locking free iteration over a MemPool, you can use this release function to free a MemPool, which was occupied previously with MemPoolAddRef_LF()!
 * </description>
 * <param name="hMemPool" type="IN" range="[VALID_MEMPOOLHANDLE,NULL]">Handle to the memory pool</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Pool was successfully freed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Pool handle is invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">Free the pool could not be done before CH_INIT_SYSTEM. Not relevant for SIL2</errorcode>
 */
RTS_RESULT CDECL MemPoolReleaseRef_LF(RTS_HANDLE hMemPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLRELEASEREF_LF) (RTS_HANDLE hMemPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLRELEASEREF_LF_NOTIMPLEMENTED)
	#define USE_MemPoolReleaseRef_LF
	#define EXT_MemPoolReleaseRef_LF
	#define GET_MemPoolReleaseRef_LF(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolReleaseRef_LF(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolReleaseRef_LF  FALSE
	#define EXP_MemPoolReleaseRef_LF  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolReleaseRef_LF
	#define EXT_MemPoolReleaseRef_LF
	#define GET_MemPoolReleaseRef_LF(fl)  CAL_CMGETAPI( "MemPoolReleaseRef_LF" ) 
	#define CAL_MemPoolReleaseRef_LF  MemPoolReleaseRef_LF
	#define CHK_MemPoolReleaseRef_LF  TRUE
	#define EXP_MemPoolReleaseRef_LF  CAL_CMEXPAPI( "MemPoolReleaseRef_LF" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolReleaseRef_LF
	#define EXT_MemPoolReleaseRef_LF
	#define GET_MemPoolReleaseRef_LF(fl)  CAL_CMGETAPI( "MemPoolReleaseRef_LF" ) 
	#define CAL_MemPoolReleaseRef_LF  MemPoolReleaseRef_LF
	#define CHK_MemPoolReleaseRef_LF  TRUE
	#define EXP_MemPoolReleaseRef_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolReleaseRef_LF", (RTS_UINTPTR)MemPoolReleaseRef_LF, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolReleaseRef_LF
	#define EXT_CmpMemPoolMemPoolReleaseRef_LF
	#define GET_CmpMemPoolMemPoolReleaseRef_LF  ERR_OK
	#define CAL_CmpMemPoolMemPoolReleaseRef_LF pICmpMemPool->IMemPoolReleaseRef_LF
	#define CHK_CmpMemPoolMemPoolReleaseRef_LF (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolReleaseRef_LF  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolReleaseRef_LF
	#define EXT_MemPoolReleaseRef_LF
	#define GET_MemPoolReleaseRef_LF(fl)  CAL_CMGETAPI( "MemPoolReleaseRef_LF" ) 
	#define CAL_MemPoolReleaseRef_LF pICmpMemPool->IMemPoolReleaseRef_LF
	#define CHK_MemPoolReleaseRef_LF (pICmpMemPool != NULL)
	#define EXP_MemPoolReleaseRef_LF  CAL_CMEXPAPI( "MemPoolReleaseRef_LF" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolReleaseRef_LF  PFMEMPOOLRELEASEREF_LF pfMemPoolReleaseRef_LF;
	#define EXT_MemPoolReleaseRef_LF  extern PFMEMPOOLRELEASEREF_LF pfMemPoolReleaseRef_LF;
	#define GET_MemPoolReleaseRef_LF(fl)  s_pfCMGetAPI2( "MemPoolReleaseRef_LF", (RTS_VOID_FCTPTR *)&pfMemPoolReleaseRef_LF, (fl), 0, 0)
	#define CAL_MemPoolReleaseRef_LF  pfMemPoolReleaseRef_LF
	#define CHK_MemPoolReleaseRef_LF  (pfMemPoolReleaseRef_LF != NULL)
	#define EXP_MemPoolReleaseRef_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolReleaseRef_LF", (RTS_UINTPTR)MemPoolReleaseRef_LF, 0, 0) 
#endif




/**
 * <description>
 *	Lock the access to the complete pool. SIL2 Implementation is using INT Locks.
 *
 *	NOTE:
 *  - Function can return ERR_NO_CONSISTENCY to signalize, that the reference counter in the pool is >0 and so someone is still in an iteration loop after MemPoolAddRef_LF()! 
 * </description>
 * <param name="hMemPool" type="IN" range="[VALID_MEMPOOLHANDLE,NULL]">Handle to the memory pool</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Pool was successfully locked</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Parameter is wrong</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Pool could not be locked, SysInt or SysSem had problems</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTIMPLEMENTED">SysInt or SysSem are not implemented</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">Lock cannot be done before CH_INIT_SYSTEM, not relevant for SIL2</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_CONSISTENCY">Lock is done, but the reference counter in the pool is >0 and so someone is still in an iteration loop after MemPoolAddRef()!</errorcode>
 */
RTS_RESULT CDECL MemPoolLock_LF(RTS_HANDLE hMemPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLLOCK_LF) (RTS_HANDLE hMemPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLLOCK_LF_NOTIMPLEMENTED)
	#define USE_MemPoolLock_LF
	#define EXT_MemPoolLock_LF
	#define GET_MemPoolLock_LF(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolLock_LF(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolLock_LF  FALSE
	#define EXP_MemPoolLock_LF  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolLock_LF
	#define EXT_MemPoolLock_LF
	#define GET_MemPoolLock_LF(fl)  CAL_CMGETAPI( "MemPoolLock_LF" ) 
	#define CAL_MemPoolLock_LF  MemPoolLock_LF
	#define CHK_MemPoolLock_LF  TRUE
	#define EXP_MemPoolLock_LF  CAL_CMEXPAPI( "MemPoolLock_LF" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolLock_LF
	#define EXT_MemPoolLock_LF
	#define GET_MemPoolLock_LF(fl)  CAL_CMGETAPI( "MemPoolLock_LF" ) 
	#define CAL_MemPoolLock_LF  MemPoolLock_LF
	#define CHK_MemPoolLock_LF  TRUE
	#define EXP_MemPoolLock_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolLock_LF", (RTS_UINTPTR)MemPoolLock_LF, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolLock_LF
	#define EXT_CmpMemPoolMemPoolLock_LF
	#define GET_CmpMemPoolMemPoolLock_LF  ERR_OK
	#define CAL_CmpMemPoolMemPoolLock_LF pICmpMemPool->IMemPoolLock_LF
	#define CHK_CmpMemPoolMemPoolLock_LF (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolLock_LF  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolLock_LF
	#define EXT_MemPoolLock_LF
	#define GET_MemPoolLock_LF(fl)  CAL_CMGETAPI( "MemPoolLock_LF" ) 
	#define CAL_MemPoolLock_LF pICmpMemPool->IMemPoolLock_LF
	#define CHK_MemPoolLock_LF (pICmpMemPool != NULL)
	#define EXP_MemPoolLock_LF  CAL_CMEXPAPI( "MemPoolLock_LF" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolLock_LF  PFMEMPOOLLOCK_LF pfMemPoolLock_LF;
	#define EXT_MemPoolLock_LF  extern PFMEMPOOLLOCK_LF pfMemPoolLock_LF;
	#define GET_MemPoolLock_LF(fl)  s_pfCMGetAPI2( "MemPoolLock_LF", (RTS_VOID_FCTPTR *)&pfMemPoolLock_LF, (fl), 0, 0)
	#define CAL_MemPoolLock_LF  pfMemPoolLock_LF
	#define CHK_MemPoolLock_LF  (pfMemPoolLock_LF != NULL)
	#define EXP_MemPoolLock_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolLock_LF", (RTS_UINTPTR)MemPoolLock_LF, 0, 0) 
#endif




/**
 * <description>
 *	Unlock the access to the complete pool. SIL2 Implementation is using INT Locks.
 * </description>
 * <param name="hMemPool" type="IN" range="[VALID_MEMPOOLHANDLE,NULL]">Handle to the memory pool</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Pool was successfully unlocked</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Parameter is wrong</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Pool could not be unlocked, SysInt or SysSem had problems</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTIMPLEMENTED">SysInt or SysSem are not implemented</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">Unlock cannot be done before CH_INIT_SYSTEM, not relevant for SIL2</errorcode>
 */
RTS_RESULT CDECL MemPoolUnlock_LF(RTS_HANDLE hMemPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLUNLOCK_LF) (RTS_HANDLE hMemPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLUNLOCK_LF_NOTIMPLEMENTED)
	#define USE_MemPoolUnlock_LF
	#define EXT_MemPoolUnlock_LF
	#define GET_MemPoolUnlock_LF(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolUnlock_LF(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolUnlock_LF  FALSE
	#define EXP_MemPoolUnlock_LF  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolUnlock_LF
	#define EXT_MemPoolUnlock_LF
	#define GET_MemPoolUnlock_LF(fl)  CAL_CMGETAPI( "MemPoolUnlock_LF" ) 
	#define CAL_MemPoolUnlock_LF  MemPoolUnlock_LF
	#define CHK_MemPoolUnlock_LF  TRUE
	#define EXP_MemPoolUnlock_LF  CAL_CMEXPAPI( "MemPoolUnlock_LF" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolUnlock_LF
	#define EXT_MemPoolUnlock_LF
	#define GET_MemPoolUnlock_LF(fl)  CAL_CMGETAPI( "MemPoolUnlock_LF" ) 
	#define CAL_MemPoolUnlock_LF  MemPoolUnlock_LF
	#define CHK_MemPoolUnlock_LF  TRUE
	#define EXP_MemPoolUnlock_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolUnlock_LF", (RTS_UINTPTR)MemPoolUnlock_LF, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolUnlock_LF
	#define EXT_CmpMemPoolMemPoolUnlock_LF
	#define GET_CmpMemPoolMemPoolUnlock_LF  ERR_OK
	#define CAL_CmpMemPoolMemPoolUnlock_LF pICmpMemPool->IMemPoolUnlock_LF
	#define CHK_CmpMemPoolMemPoolUnlock_LF (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolUnlock_LF  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolUnlock_LF
	#define EXT_MemPoolUnlock_LF
	#define GET_MemPoolUnlock_LF(fl)  CAL_CMGETAPI( "MemPoolUnlock_LF" ) 
	#define CAL_MemPoolUnlock_LF pICmpMemPool->IMemPoolUnlock_LF
	#define CHK_MemPoolUnlock_LF (pICmpMemPool != NULL)
	#define EXP_MemPoolUnlock_LF  CAL_CMEXPAPI( "MemPoolUnlock_LF" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolUnlock_LF  PFMEMPOOLUNLOCK_LF pfMemPoolUnlock_LF;
	#define EXT_MemPoolUnlock_LF  extern PFMEMPOOLUNLOCK_LF pfMemPoolUnlock_LF;
	#define GET_MemPoolUnlock_LF(fl)  s_pfCMGetAPI2( "MemPoolUnlock_LF", (RTS_VOID_FCTPTR *)&pfMemPoolUnlock_LF, (fl), 0, 0)
	#define CAL_MemPoolUnlock_LF  pfMemPoolUnlock_LF
	#define CHK_MemPoolUnlock_LF  (pfMemPoolUnlock_LF != NULL)
	#define EXP_MemPoolUnlock_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolUnlock_LF", (RTS_UINTPTR)MemPoolUnlock_LF, 0, 0) 
#endif




/**
 * <description>
 *	Remove used block from the pool. If this function succeeds, the block could not be recognized anymore by any iterator on the pool!
 *	NOTE:
 *  Please note the return codes of this function, especially ERR_PENDING! 
 * </description>
 * <param name="pBlock" type="IN" range="[NULL,VALID_BLOCK]">Pointer to the memory block</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successfully removed block from MemPool</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Could not remove block from MemPool, wrong pBlock</errorcode> 
 * <errorcode name="RTS_RESULT" type="ERR_PENDING">Could not remove block from MemPool because the pool is currently under usage by another iterator which used MemPoolAddRef_LF()! 
 *												   But the block is still marked to delete and can be deleted later on by calling CAL_MemPoolReleaseBlocks(hPool, TRUE, pParam)!
 * </errorcode> 
 */
RTS_RESULT CDECL MemPoolRemoveUsedBlock_LF(void* pBlock);
typedef RTS_RESULT (CDECL * PFMEMPOOLREMOVEUSEDBLOCK_LF) (void* pBlock);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLREMOVEUSEDBLOCK_LF_NOTIMPLEMENTED)
	#define USE_MemPoolRemoveUsedBlock_LF
	#define EXT_MemPoolRemoveUsedBlock_LF
	#define GET_MemPoolRemoveUsedBlock_LF(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolRemoveUsedBlock_LF(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolRemoveUsedBlock_LF  FALSE
	#define EXP_MemPoolRemoveUsedBlock_LF  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolRemoveUsedBlock_LF
	#define EXT_MemPoolRemoveUsedBlock_LF
	#define GET_MemPoolRemoveUsedBlock_LF(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlock_LF" ) 
	#define CAL_MemPoolRemoveUsedBlock_LF  MemPoolRemoveUsedBlock_LF
	#define CHK_MemPoolRemoveUsedBlock_LF  TRUE
	#define EXP_MemPoolRemoveUsedBlock_LF  CAL_CMEXPAPI( "MemPoolRemoveUsedBlock_LF" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolRemoveUsedBlock_LF
	#define EXT_MemPoolRemoveUsedBlock_LF
	#define GET_MemPoolRemoveUsedBlock_LF(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlock_LF" ) 
	#define CAL_MemPoolRemoveUsedBlock_LF  MemPoolRemoveUsedBlock_LF
	#define CHK_MemPoolRemoveUsedBlock_LF  TRUE
	#define EXP_MemPoolRemoveUsedBlock_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolRemoveUsedBlock_LF", (RTS_UINTPTR)MemPoolRemoveUsedBlock_LF, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolRemoveUsedBlock_LF
	#define EXT_CmpMemPoolMemPoolRemoveUsedBlock_LF
	#define GET_CmpMemPoolMemPoolRemoveUsedBlock_LF  ERR_OK
	#define CAL_CmpMemPoolMemPoolRemoveUsedBlock_LF pICmpMemPool->IMemPoolRemoveUsedBlock_LF
	#define CHK_CmpMemPoolMemPoolRemoveUsedBlock_LF (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolRemoveUsedBlock_LF  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolRemoveUsedBlock_LF
	#define EXT_MemPoolRemoveUsedBlock_LF
	#define GET_MemPoolRemoveUsedBlock_LF(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlock_LF" ) 
	#define CAL_MemPoolRemoveUsedBlock_LF pICmpMemPool->IMemPoolRemoveUsedBlock_LF
	#define CHK_MemPoolRemoveUsedBlock_LF (pICmpMemPool != NULL)
	#define EXP_MemPoolRemoveUsedBlock_LF  CAL_CMEXPAPI( "MemPoolRemoveUsedBlock_LF" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolRemoveUsedBlock_LF  PFMEMPOOLREMOVEUSEDBLOCK_LF pfMemPoolRemoveUsedBlock_LF;
	#define EXT_MemPoolRemoveUsedBlock_LF  extern PFMEMPOOLREMOVEUSEDBLOCK_LF pfMemPoolRemoveUsedBlock_LF;
	#define GET_MemPoolRemoveUsedBlock_LF(fl)  s_pfCMGetAPI2( "MemPoolRemoveUsedBlock_LF", (RTS_VOID_FCTPTR *)&pfMemPoolRemoveUsedBlock_LF, (fl), 0, 0)
	#define CAL_MemPoolRemoveUsedBlock_LF  pfMemPoolRemoveUsedBlock_LF
	#define CHK_MemPoolRemoveUsedBlock_LF  (pfMemPoolRemoveUsedBlock_LF != NULL)
	#define EXP_MemPoolRemoveUsedBlock_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolRemoveUsedBlock_LF", (RTS_UINTPTR)MemPoolRemoveUsedBlock_LF, 0, 0) 
#endif




/**
 * <description>
 *	Remove used block from the specified pool. Now, the block is removed from the chain list of used blocks.
 * </description>
 * <param name="pBlock" type="IN">Pointer to the memory block</param>
 * <param name="hPool" type="IN">Handle to the pool</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successfully removed block from MemPool</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Could not remove block from MemPool, wrong pBlock</errorcode> 
 * <errorcode name="RTS_RESULT" type="ERR_PENDING">Could not remove block from MemPool because the pool is currently under usage by another iterator which used MemPoolAddRef_LF()! 
 *												   But the block is still marked to delete and can be deleted later on by calling CAL_MemPoolReleaseBlocks(hPool, TRUE, pParam)!
 * </errorcode> 
 */
RTS_RESULT CDECL MemPoolRemoveUsedBlockFromPool_LF(void* pBlock, RTS_HANDLE hPool);
typedef RTS_RESULT (CDECL * PFMEMPOOLREMOVEUSEDBLOCKFROMPOOL_LF) (void* pBlock, RTS_HANDLE hPool);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLREMOVEUSEDBLOCKFROMPOOL_LF_NOTIMPLEMENTED)
	#define USE_MemPoolRemoveUsedBlockFromPool_LF
	#define EXT_MemPoolRemoveUsedBlockFromPool_LF
	#define GET_MemPoolRemoveUsedBlockFromPool_LF(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolRemoveUsedBlockFromPool_LF(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolRemoveUsedBlockFromPool_LF  FALSE
	#define EXP_MemPoolRemoveUsedBlockFromPool_LF  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolRemoveUsedBlockFromPool_LF
	#define EXT_MemPoolRemoveUsedBlockFromPool_LF
	#define GET_MemPoolRemoveUsedBlockFromPool_LF(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlockFromPool_LF" ) 
	#define CAL_MemPoolRemoveUsedBlockFromPool_LF  MemPoolRemoveUsedBlockFromPool_LF
	#define CHK_MemPoolRemoveUsedBlockFromPool_LF  TRUE
	#define EXP_MemPoolRemoveUsedBlockFromPool_LF  CAL_CMEXPAPI( "MemPoolRemoveUsedBlockFromPool_LF" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolRemoveUsedBlockFromPool_LF
	#define EXT_MemPoolRemoveUsedBlockFromPool_LF
	#define GET_MemPoolRemoveUsedBlockFromPool_LF(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlockFromPool_LF" ) 
	#define CAL_MemPoolRemoveUsedBlockFromPool_LF  MemPoolRemoveUsedBlockFromPool_LF
	#define CHK_MemPoolRemoveUsedBlockFromPool_LF  TRUE
	#define EXP_MemPoolRemoveUsedBlockFromPool_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolRemoveUsedBlockFromPool_LF", (RTS_UINTPTR)MemPoolRemoveUsedBlockFromPool_LF, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolRemoveUsedBlockFromPool_LF
	#define EXT_CmpMemPoolMemPoolRemoveUsedBlockFromPool_LF
	#define GET_CmpMemPoolMemPoolRemoveUsedBlockFromPool_LF  ERR_OK
	#define CAL_CmpMemPoolMemPoolRemoveUsedBlockFromPool_LF pICmpMemPool->IMemPoolRemoveUsedBlockFromPool_LF
	#define CHK_CmpMemPoolMemPoolRemoveUsedBlockFromPool_LF (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolRemoveUsedBlockFromPool_LF  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolRemoveUsedBlockFromPool_LF
	#define EXT_MemPoolRemoveUsedBlockFromPool_LF
	#define GET_MemPoolRemoveUsedBlockFromPool_LF(fl)  CAL_CMGETAPI( "MemPoolRemoveUsedBlockFromPool_LF" ) 
	#define CAL_MemPoolRemoveUsedBlockFromPool_LF pICmpMemPool->IMemPoolRemoveUsedBlockFromPool_LF
	#define CHK_MemPoolRemoveUsedBlockFromPool_LF (pICmpMemPool != NULL)
	#define EXP_MemPoolRemoveUsedBlockFromPool_LF  CAL_CMEXPAPI( "MemPoolRemoveUsedBlockFromPool_LF" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolRemoveUsedBlockFromPool_LF  PFMEMPOOLREMOVEUSEDBLOCKFROMPOOL_LF pfMemPoolRemoveUsedBlockFromPool_LF;
	#define EXT_MemPoolRemoveUsedBlockFromPool_LF  extern PFMEMPOOLREMOVEUSEDBLOCKFROMPOOL_LF pfMemPoolRemoveUsedBlockFromPool_LF;
	#define GET_MemPoolRemoveUsedBlockFromPool_LF(fl)  s_pfCMGetAPI2( "MemPoolRemoveUsedBlockFromPool_LF", (RTS_VOID_FCTPTR *)&pfMemPoolRemoveUsedBlockFromPool_LF, (fl), 0, 0)
	#define CAL_MemPoolRemoveUsedBlockFromPool_LF  pfMemPoolRemoveUsedBlockFromPool_LF
	#define CHK_MemPoolRemoveUsedBlockFromPool_LF  (pfMemPoolRemoveUsedBlockFromPool_LF != NULL)
	#define EXP_MemPoolRemoveUsedBlockFromPool_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolRemoveUsedBlockFromPool_LF", (RTS_UINTPTR)MemPoolRemoveUsedBlockFromPool_LF, 0, 0) 
#endif




/**
 * <description>
 *	Put a memory block back into the pool (release). The content of the block is released with the corresponding delete function, which was specified as a parameter in the MemPoolCreate() function.
 * </description>
 * <param name="pBlock" type="IN">Pointer to the memory block</param>
 * <param name="pParam" type="IN">Optional parameter for the delete function. Can be NULL.</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Successfully added block to MemPool</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Could not add block to MemPool, wrong pBlock</errorcode> 
 */
RTS_RESULT CDECL MemPoolPutBlock_LF(void* pBlock, void *pParam);
typedef RTS_RESULT (CDECL * PFMEMPOOLPUTBLOCK_LF) (void* pBlock, void *pParam);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLPUTBLOCK_LF_NOTIMPLEMENTED)
	#define USE_MemPoolPutBlock_LF
	#define EXT_MemPoolPutBlock_LF
	#define GET_MemPoolPutBlock_LF(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolPutBlock_LF(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolPutBlock_LF  FALSE
	#define EXP_MemPoolPutBlock_LF  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolPutBlock_LF
	#define EXT_MemPoolPutBlock_LF
	#define GET_MemPoolPutBlock_LF(fl)  CAL_CMGETAPI( "MemPoolPutBlock_LF" ) 
	#define CAL_MemPoolPutBlock_LF  MemPoolPutBlock_LF
	#define CHK_MemPoolPutBlock_LF  TRUE
	#define EXP_MemPoolPutBlock_LF  CAL_CMEXPAPI( "MemPoolPutBlock_LF" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolPutBlock_LF
	#define EXT_MemPoolPutBlock_LF
	#define GET_MemPoolPutBlock_LF(fl)  CAL_CMGETAPI( "MemPoolPutBlock_LF" ) 
	#define CAL_MemPoolPutBlock_LF  MemPoolPutBlock_LF
	#define CHK_MemPoolPutBlock_LF  TRUE
	#define EXP_MemPoolPutBlock_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolPutBlock_LF", (RTS_UINTPTR)MemPoolPutBlock_LF, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolPutBlock_LF
	#define EXT_CmpMemPoolMemPoolPutBlock_LF
	#define GET_CmpMemPoolMemPoolPutBlock_LF  ERR_OK
	#define CAL_CmpMemPoolMemPoolPutBlock_LF pICmpMemPool->IMemPoolPutBlock_LF
	#define CHK_CmpMemPoolMemPoolPutBlock_LF (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolPutBlock_LF  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolPutBlock_LF
	#define EXT_MemPoolPutBlock_LF
	#define GET_MemPoolPutBlock_LF(fl)  CAL_CMGETAPI( "MemPoolPutBlock_LF" ) 
	#define CAL_MemPoolPutBlock_LF pICmpMemPool->IMemPoolPutBlock_LF
	#define CHK_MemPoolPutBlock_LF (pICmpMemPool != NULL)
	#define EXP_MemPoolPutBlock_LF  CAL_CMEXPAPI( "MemPoolPutBlock_LF" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolPutBlock_LF  PFMEMPOOLPUTBLOCK_LF pfMemPoolPutBlock_LF;
	#define EXT_MemPoolPutBlock_LF  extern PFMEMPOOLPUTBLOCK_LF pfMemPoolPutBlock_LF;
	#define GET_MemPoolPutBlock_LF(fl)  s_pfCMGetAPI2( "MemPoolPutBlock_LF", (RTS_VOID_FCTPTR *)&pfMemPoolPutBlock_LF, (fl), 0, 0)
	#define CAL_MemPoolPutBlock_LF  pfMemPoolPutBlock_LF
	#define CHK_MemPoolPutBlock_LF  (pfMemPoolPutBlock_LF != NULL)
	#define EXP_MemPoolPutBlock_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolPutBlock_LF", (RTS_UINTPTR)MemPoolPutBlock_LF, 0, 0) 
#endif




/**
 * <description>
 *	Check a pool memory block, if it is still valid and is not released.
 *	NOTE:
 *	If the check is successful, a MemPoolAddRef done on this pool!!! So you have to release this reference at the end of the usage with MemPoolReleaseRef_LF()!
 * </description>
 * <param name="hMemPool" type="IN">Handle to the memory pool</param>
 * <param name="pBlock" type="IN">Pointer to the memory block</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Succeeded</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Pool or block handle is invalid</errorcode>
 */
RTS_RESULT CDECL MemPoolIsValidBlock_LF(RTS_HANDLE hMemPool, void *pBlock);
typedef RTS_RESULT (CDECL * PFMEMPOOLISVALIDBLOCK_LF) (RTS_HANDLE hMemPool, void *pBlock);
#if defined(CMPMEMPOOL_NOTIMPLEMENTED) || defined(MEMPOOLISVALIDBLOCK_LF_NOTIMPLEMENTED)
	#define USE_MemPoolIsValidBlock_LF
	#define EXT_MemPoolIsValidBlock_LF
	#define GET_MemPoolIsValidBlock_LF(fl)  ERR_NOTIMPLEMENTED
	#define CAL_MemPoolIsValidBlock_LF(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_MemPoolIsValidBlock_LF  FALSE
	#define EXP_MemPoolIsValidBlock_LF  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_MemPoolIsValidBlock_LF
	#define EXT_MemPoolIsValidBlock_LF
	#define GET_MemPoolIsValidBlock_LF(fl)  CAL_CMGETAPI( "MemPoolIsValidBlock_LF" ) 
	#define CAL_MemPoolIsValidBlock_LF  MemPoolIsValidBlock_LF
	#define CHK_MemPoolIsValidBlock_LF  TRUE
	#define EXP_MemPoolIsValidBlock_LF  CAL_CMEXPAPI( "MemPoolIsValidBlock_LF" ) 
#elif defined(MIXED_LINK) && !defined(CMPMEMPOOL_EXTERNAL)
	#define USE_MemPoolIsValidBlock_LF
	#define EXT_MemPoolIsValidBlock_LF
	#define GET_MemPoolIsValidBlock_LF(fl)  CAL_CMGETAPI( "MemPoolIsValidBlock_LF" ) 
	#define CAL_MemPoolIsValidBlock_LF  MemPoolIsValidBlock_LF
	#define CHK_MemPoolIsValidBlock_LF  TRUE
	#define EXP_MemPoolIsValidBlock_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolIsValidBlock_LF", (RTS_UINTPTR)MemPoolIsValidBlock_LF, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CmpMemPoolMemPoolIsValidBlock_LF
	#define EXT_CmpMemPoolMemPoolIsValidBlock_LF
	#define GET_CmpMemPoolMemPoolIsValidBlock_LF  ERR_OK
	#define CAL_CmpMemPoolMemPoolIsValidBlock_LF pICmpMemPool->IMemPoolIsValidBlock_LF
	#define CHK_CmpMemPoolMemPoolIsValidBlock_LF (pICmpMemPool != NULL)
	#define EXP_CmpMemPoolMemPoolIsValidBlock_LF  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_MemPoolIsValidBlock_LF
	#define EXT_MemPoolIsValidBlock_LF
	#define GET_MemPoolIsValidBlock_LF(fl)  CAL_CMGETAPI( "MemPoolIsValidBlock_LF" ) 
	#define CAL_MemPoolIsValidBlock_LF pICmpMemPool->IMemPoolIsValidBlock_LF
	#define CHK_MemPoolIsValidBlock_LF (pICmpMemPool != NULL)
	#define EXP_MemPoolIsValidBlock_LF  CAL_CMEXPAPI( "MemPoolIsValidBlock_LF" ) 
#else /* DYNAMIC_LINK */
	#define USE_MemPoolIsValidBlock_LF  PFMEMPOOLISVALIDBLOCK_LF pfMemPoolIsValidBlock_LF;
	#define EXT_MemPoolIsValidBlock_LF  extern PFMEMPOOLISVALIDBLOCK_LF pfMemPoolIsValidBlock_LF;
	#define GET_MemPoolIsValidBlock_LF(fl)  s_pfCMGetAPI2( "MemPoolIsValidBlock_LF", (RTS_VOID_FCTPTR *)&pfMemPoolIsValidBlock_LF, (fl), 0, 0)
	#define CAL_MemPoolIsValidBlock_LF  pfMemPoolIsValidBlock_LF
	#define CHK_MemPoolIsValidBlock_LF  (pfMemPoolIsValidBlock_LF != NULL)
	#define EXP_MemPoolIsValidBlock_LF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"MemPoolIsValidBlock_LF", (RTS_UINTPTR)MemPoolIsValidBlock_LF, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFMEMPOOLCREATE IMemPoolCreate;
 	PFMEMPOOLCREATEDYNAMIC IMemPoolCreateDynamic;
 	PFMEMPOOLCREATESTATIC IMemPoolCreateStatic;
 	PFMEMPOOLEXTENDDYNAMIC IMemPoolExtendDynamic;
 	PFMEMPOOLEXTENDSTATIC IMemPoolExtendStatic;
 	PFMEMPOOLCREATESYNCOBJECT IMemPoolCreateSyncObject;
 	PFMEMPOOLDELETESYNCOBJECT IMemPoolDeleteSyncObject;
 	PFMEMPOOLDELETE IMemPoolDelete;
 	PFMEMPOOLCLEANUP IMemPoolCleanup;
 	PFMEMPOOLGETBLOCK IMemPoolGetBlock;
 	PFMEMPOOLGETBLOCK2 IMemPoolGetBlock2;
 	PFMEMPOOLGETPCB IMemPoolGetPCB;
 	PFMEMPOOLPUTBLOCK IMemPoolPutBlock;
 	PFMEMPOOLADDUSEDBLOCK IMemPoolAddUsedBlock;
 	PFMEMPOOLAPPENDUSEDBLOCK IMemPoolAppendUsedBlock;
 	PFMEMPOOLINSERTUSEDBLOCK IMemPoolInsertUsedBlock;
 	PFMEMPOOLREMOVEUSEDBLOCK IMemPoolRemoveUsedBlock;
 	PFMEMPOOLADDUSEDBLOCKTOPOOL IMemPoolAddUsedBlockToPool;
 	PFMEMPOOLAPPENDUSEDBLOCKTOPOOL IMemPoolAppendUsedBlockToPool;
 	PFMEMPOOLREMOVEUSEDBLOCKFROMPOOL IMemPoolRemoveUsedBlockFromPool;
 	PFMEMPOOLLOCKBLOCK IMemPoolLockBlock;
 	PFMEMPOOLUNLOCKBLOCK IMemPoolUnlockBlock;
 	PFMEMPOOLRELEASEBLOCKS IMemPoolReleaseBlocks;
 	PFMEMPOOLLOCK IMemPoolLock;
 	PFMEMPOOLUNLOCK IMemPoolUnlock;
 	PFMEMPOOLFINDBLOCK IMemPoolFindBlock;
 	PFMEMPOOLISVALIDBLOCK IMemPoolIsValidBlock;
 	PFMEMPOOLGETFIRSTBLOCK IMemPoolGetFirstBlock;
 	PFMEMPOOLGETNEXTBLOCK IMemPoolGetNextBlock;
 	PFMEMPOOLADDREF_LF IMemPoolAddRef_LF;
 	PFMEMPOOLRELEASEREF_LF IMemPoolReleaseRef_LF;
 	PFMEMPOOLLOCK_LF IMemPoolLock_LF;
 	PFMEMPOOLUNLOCK_LF IMemPoolUnlock_LF;
 	PFMEMPOOLREMOVEUSEDBLOCK_LF IMemPoolRemoveUsedBlock_LF;
 	PFMEMPOOLREMOVEUSEDBLOCKFROMPOOL_LF IMemPoolRemoveUsedBlockFromPool_LF;
 	PFMEMPOOLPUTBLOCK_LF IMemPoolPutBlock_LF;
 	PFMEMPOOLISVALIDBLOCK_LF IMemPoolIsValidBlock_LF;
 } ICmpMemPool_C;

#ifdef CPLUSPLUS
class ICmpMemPool : public IBase
{
	public:
		virtual RTS_HANDLE CDECL IMemPoolCreate(MemPoolConfiguration *pConfiguration, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IMemPoolCreateDynamic(char *pszComponentName, RTS_SIZE ulNumBlocks, RTS_SIZE ulBlockSize, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL IMemPoolCreateStatic(RTS_SIZE ulBlockSize, RTS_SIZE ulMemSize, void* pMemory, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IMemPoolExtendDynamic(RTS_HANDLE hMemPool, char *pszComponentName, RTS_SIZE ulNumBlocks) =0;
		virtual RTS_RESULT CDECL IMemPoolExtendStatic(RTS_HANDLE hMemPool, RTS_SIZE ulMemSize, void* pMemory) =0;
		virtual RTS_RESULT CDECL IMemPoolCreateSyncObject(RTS_HANDLE hMemPool) =0;
		virtual RTS_RESULT CDECL IMemPoolDeleteSyncObject(RTS_HANDLE hMemPool) =0;
		virtual RTS_RESULT CDECL IMemPoolDelete(RTS_HANDLE hMemPool, char *pszComponentName) =0;
		virtual RTS_RESULT CDECL IMemPoolCleanup(RTS_HANDLE hMemPool, char *pszComponentName, int bReleaseSemaphore) =0;
		virtual void* CDECL IMemPoolGetBlock(RTS_HANDLE hMemPool, RTS_RESULT *pResult) =0;
		virtual void* CDECL IMemPoolGetBlock2(RTS_HANDLE hMemPool, int bDynamic, char *pszComponentName, RTS_RESULT *pResult) =0;
		virtual RTS_PCB* CDECL IMemPoolGetPCB(RTS_HANDLE hMemPool, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IMemPoolPutBlock(void* pBlock) =0;
		virtual RTS_RESULT CDECL IMemPoolAddUsedBlock(void* pBlock) =0;
		virtual RTS_RESULT CDECL IMemPoolAppendUsedBlock(void* pBlock) =0;
		virtual RTS_RESULT CDECL IMemPoolInsertUsedBlock(void* pPrevBlock, void* pBlock) =0;
		virtual RTS_RESULT CDECL IMemPoolRemoveUsedBlock(void* pBlock) =0;
		virtual RTS_RESULT CDECL IMemPoolAddUsedBlockToPool(void* pBlock, RTS_HANDLE hPool) =0;
		virtual RTS_RESULT CDECL IMemPoolAppendUsedBlockToPool(void* pBlock, RTS_HANDLE hPool) =0;
		virtual RTS_RESULT CDECL IMemPoolRemoveUsedBlockFromPool(void* pBlock, RTS_HANDLE hPool) =0;
		virtual RTS_RESULT CDECL IMemPoolLockBlock(void* pBlock) =0;
		virtual RTS_RESULT CDECL IMemPoolUnlockBlock(void* pBlock) =0;
		virtual RTS_RESULT CDECL IMemPoolReleaseBlocks(RTS_HANDLE hMemPool, RTS_BOOL bOnlyDeletedBlocks, void *pParam) =0;
		virtual RTS_RESULT CDECL IMemPoolLock(RTS_HANDLE hMemPool) =0;
		virtual RTS_RESULT CDECL IMemPoolUnlock(RTS_HANDLE hMemPool) =0;
		virtual void * CDECL IMemPoolFindBlock(RTS_HANDLE hMemPool, RTS_SIZE ulOffset, RTS_SIZE ulSize, void *pToFind, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IMemPoolIsValidBlock(RTS_HANDLE hMemPool, void *pBlock) =0;
		virtual void* CDECL IMemPoolGetFirstBlock(RTS_HANDLE hMemPool, RTS_RESULT *pResult) =0;
		virtual void* CDECL IMemPoolGetNextBlock(RTS_HANDLE hMemPool, void *pPrevBlock, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL IMemPoolAddRef_LF(RTS_HANDLE hMemPool) =0;
		virtual RTS_RESULT CDECL IMemPoolReleaseRef_LF(RTS_HANDLE hMemPool) =0;
		virtual RTS_RESULT CDECL IMemPoolLock_LF(RTS_HANDLE hMemPool) =0;
		virtual RTS_RESULT CDECL IMemPoolUnlock_LF(RTS_HANDLE hMemPool) =0;
		virtual RTS_RESULT CDECL IMemPoolRemoveUsedBlock_LF(void* pBlock) =0;
		virtual RTS_RESULT CDECL IMemPoolRemoveUsedBlockFromPool_LF(void* pBlock, RTS_HANDLE hPool) =0;
		virtual RTS_RESULT CDECL IMemPoolPutBlock_LF(void* pBlock, void *pParam) =0;
		virtual RTS_RESULT CDECL IMemPoolIsValidBlock_LF(RTS_HANDLE hMemPool, void *pBlock) =0;
};
	#ifndef ITF_CmpMemPool
		#define ITF_CmpMemPool static ICmpMemPool *pICmpMemPool = NULL;
	#endif
	#define EXTITF_CmpMemPool
#else	/*CPLUSPLUS*/
	typedef ICmpMemPool_C		ICmpMemPool;
	#ifndef ITF_CmpMemPool
		#define ITF_CmpMemPool
	#endif
	#define EXTITF_CmpMemPool
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMPMEMPOOLITF_H_*/
