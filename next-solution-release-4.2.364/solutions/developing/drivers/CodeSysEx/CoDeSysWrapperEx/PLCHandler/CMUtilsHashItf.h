 /**
 * <interfacename>CMUtilsHash</interfacename>
 * <description>
 *	Interface for the implementation of a hash table with a linked list of hash entries to handle collisions.
 *
 *	Usage:
 *		 1. Creating a hash table using of static memory [recommended]:
 *
 *			 CMUtlHashTable s_hashTable;
 *			 #define MY_NUM_OF_HASHENTRIES	10
 *			 RTS_UI8 s_hashTableBuffer[MEMPOOLHASH_GET_HASHTABLE_LEN( MY_NUM_OF_HASHENTRIES )];
 *
 *			 // Create hash table
 *			 RTS_RESULT result = CAL_CMUtlHashCreate2(&amp;s_hashTable, MY_NUM_OF_HASHENTRIES, s_hashTableBuffer, COMPONENT_NAME);
 *
 *		 2. Creating a hash table using of dynamic memory:
 *
 *			 CMUtlHashTable s_hashTable;
 *			 #define MY_NUM_OF_HASHENTRIES	10
 *
 *			 // Create hash table
 *			 RTS_RESULT result = CAL_CMUtlHashCreate2(&amp;s_hashTable, MY_NUM_OF_HASHENTRIES, s_hashTableBuffer, COMPONENT_NAME);
 *
 * 
 *		 // Delete hash table
 *		 CAL_CMUtlHashDelete(&amp;s_hashTable);
 *
 *		 // Insert hash entry to the hash table
 *		 void *pBlock = ...; // Pointer to memory block to insert
 *		 result = CAL_CMUtlHashInsert(&amp;s_hashTable, "MyKey", strlen("MyKey") + 1, FALSE, pBlock);
 * 
 *		 // Insert new static hash entry to the hash table, which is filled with the parameters
 *		 static CMUtlHashEntry s_newEntry;
 *		 result = CAL_CMUtlHashInsert2(&amp;s_hashTable, &amp;s_newEntry, "MyKey", strlen("MyKey") + 1, FALSE, pBlock);
 *
 *		 // Remove hash entry specified by key
 *		 CAL_CMUtlHashRemove(&amp;s_hashTable, &amp;s_newEntry);	
 *
 *		 // Remove hash entry specified by entry
 *		 CAL_CMUtlHashRemove2(&amp;s_hashTable, "MyKey", strlen("MyKey") + 1);	
 *
 *		 // Search a hash entry
 *		 RTS_RESULT result;
 *		 CMUtlHashEntry *pEntry = CAL_CMUtlHashSearch(&amp;s_hashTable, "MyKey", strlen("MyKey") + 1, &amp;result);
 *		 void *pBlock = pEntry->pData;				
 *
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _CMUTILSHASHITF_H_
#define _CMUTILSHASHITF_H_

#include "CmpStd.h"

 

 





/**
 * <category>Hash flag handling macros</category>
 * <element name="CMUTLHASH_SET_FLAG_KEY">Macro to set hash key type flags</element>
 * <element name="CMUTLHASH_GET_FLAG_KEY">Macro to get hash key type flags</element>
 */
#define CMUTLHASH_SET_FLAG_KEY(flags, keytype)	(flags = ((flags & 0xFFFFFF00) | keytype))
#define CMUTLHASH_GET_FLAG_KEY(flags)			(flags & 0x000000FF)

/**
 * <category>Hash entry flags</category>
 * <element name="CMUTLHASH_FLAG_KEY_INTEGER">Key type flag to specify a hash key as an integer</element>
 * <element name="CMUTLHASH_FLAG_KEY_STRING">Key type flag to specify a hash key as a string</element>
 * <element name="CMUTLHASH_FLAG_ALLOC_KEY">Flag to specify that the key is allocated dynamic from heap</element>
 * <element name="CMUTLHASH_FLAG_ALLOC_ENTRY">Flag to specify that the hash entry is allocated dynamic from heap</element>
 */
#define CMUTLHASH_FLAG_KEY_INTEGER				0x00000001
#define CMUTLHASH_FLAG_KEY_STRING				0x00000002
#define CMUTLHASH_FLAG_KEY_ARBITRARY			0x00000004

#define CMUTLHASH_FLAG_ALLOC_KEY				0x00000100
#define CMUTLHASH_FLAG_ALLOC_ENTRY				0x00000200

/**
 * <category>Hash table flags</category>
 * <element name="CMUTLHASHTABLE_FLAG_ALLOC_TABLE">INTERNAL USAGE: Table is dynamically allocated from heap</element>
 * <element name="CMUTLHASHTABLE_FLAG_INTERRUPT_SAFE">Can be set by the caller after creating the hash table to specify, that the hash table should be interrupt safe/used from an interrupt handler!</element>
 * <element name="CMUTLHASHTABLE_FLAG_NO_SYNC">Can be set to specify, that the hash table should not create a synchronization object. In this case the hash table should only be called from a single task context!</element>
 */
#define CMUTLHASHTABLE_FLAG_ALLOC_TABLE			0x00000001
#define CMUTLHASHTABLE_FLAG_INTERRUPT_SAFE		0x00000002
#define CMUTLHASHTABLE_FLAG_NO_SYNC				0x00000004
#define CMUTLHASHTABLE_FLAG_CALLBACK			0x00000008


/**
 * <category>Hash key</category>
 * <description>
 *	Key value of a hash entry. Can be an integer or string key.
 * </description>
 * <element name="keyLen" type="IN">Key length in bytes</element>
 * <element name="integer" type="IN">Integer key is stored</element>
 * <element name="pString" type="IN">Pointer to string key</element>
 */
typedef struct CMUtlHaskKey_
{
	RTS_SIZE keyLen;
	union
	{
		RTS_SIZE integer;
		RTS_UI8 *pString;
		void *pArbitrary;
	} type;
} CMUtlHaskKey;

/**
 * <category>Hash entry</category>
 * <description>
 *	Single hash entry of the hash table.
 * </description>
 * <element name="pData" type="IN">Pointer to data to store in hash entry</element>
 * <element name="key" type="IN">Corresponding unique key</element>
 * <element name="pNext" type="IN">Pointer to next element in linked list of the same hash entry (hash table collisions)</element>
 * <element name="flags" type="IN">Flags of the hash entry. See category "Hash entry flags" for details.</element>
 * <element name="_alignmentDummy" type="IN">Don't use. 8 byte alignment dummy.</element>
 */
typedef struct CMUtlHashEntry_
{
	void *pData;
	CMUtlHaskKey key;
	struct CMUtlHashEntry_ *pNext;
	RTS_UI32 flags;
	RTS_UI32 _alignmentDummy;
} CMUtlHashEntry;


/**
 * <description>Callback function to calculate the hash of an arbitrary data structure.</description>
 * <param name="pKey" type="IN">Pointer the data strucutre used as key.</param>
 * <param name="keySize" type="IN">Size of the data stucture.</param>
 * <result>Hash of the data structure.</result>
 */
typedef RTS_SIZE(CDECL *PFHASHCALLBACK)(void *pKey, RTS_SIZE keySize);

/**
 * <description>Callback function to compare two different hash keys.</description>
 * <param name="pKeyA" type="IN">Pointer to the first data strucutre used as key.</param>
 * <param name="keyASize" type="IN">Size of the first data structure.</param>* <param name="pKeyA" type="IN">Pointer to the first data strucutre used as key.</param>
 * <param name="pKeyB" type="IN">Pointer to the second data strucutre used as key.</param>
 * <param name="keyBSize" type="IN">Size of the second data structure.</param>
 * <result>TRUE if the keys are equal, FALSE otherwise.</result>
 */
typedef RTS_BOOL(CDECL *PFCOMPARECALLBACK)(void *pKeyA, RTS_SIZE keyASize, void *pKeyB, RTS_SIZE keyBSize);

/**
 * <category>Hash table</category>
 * <description>
 *	All entries of the hash table object are created and initialized in the CMUtlHashCreate() call!
 * </description>
 * <element name="hSync" type="IN">Handle to a synchronization object</element>
 * <element name="ppTable" type="IN">Pointer to the hash table buffer. Length must be (sNumHashEntries * sizeof(RTS_UI8 *))</element>
 * <element name="sNumHashEntries" type="IN">Length of the hash table respectively the number of elements that can be stored in the hash table without any collision</element>
 * <element name="pszComponentName" type="IN">Pointer to component name of the caller (which is generated typically in the macro COMPONENT_NAME)</element>
 * <element name="flagsTable" type="IN">Flags of the hash table. See category "Hash table flags" for details.</element>
 */
typedef struct CMUtlHashTable_
{
	RTS_HANDLE hSync;
	CMUtlHashEntry **ppTable;
	RTS_SIZE sNumHashEntries;
	char *pszComponentName; 
	RTS_UI32 flagsTable;
	PFHASHCALLBACK hashCallback;
	PFCOMPARECALLBACK compareCallback;
} CMUtlHashTable;


#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>
 *  Function to create a hash table.
 * </description>
 * <param name="pHashTable" type="INOUT">Pointer to a hash table object, which is filled by this function call!</param>
 * <param name="sNumHashEntries" type="IN">Length of the hash table respectively the number of elements that can be stored in the hash table without any collision.
 *		NOTE:
 *		Hash table is allocated on heap via SysMemAllocData() within this function. If you would like to provide a static memory for the hash table. Use CMUtlHashCreate2() instead!
 *      The number of available hash entries should be a prime number to get a nice spreading.
 * </param>
 * <param name="pszComponentName" type="IN">Pointer to component name of the caller (which is generated typically in the macro COMPONENT_NAME)</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Hash table could be created</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid pointer or sHashTableLen is 0</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_NOMEMORY">No memory to create the hash table</errorcode> 
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlHashCreate(CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, char *pszComponentName);
typedef RTS_RESULT (CDECL * PFCMUTLHASHCREATE) (CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, char *pszComponentName);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHCREATE_NOTIMPLEMENTED)
	#define USE_CMUtlHashCreate
	#define EXT_CMUtlHashCreate
	#define GET_CMUtlHashCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashCreate(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashCreate  FALSE
	#define EXP_CMUtlHashCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashCreate
	#define EXT_CMUtlHashCreate
	#define GET_CMUtlHashCreate(fl)  CAL_CMGETAPI( "CMUtlHashCreate" ) 
	#define CAL_CMUtlHashCreate  CMUtlHashCreate
	#define CHK_CMUtlHashCreate  TRUE
	#define EXP_CMUtlHashCreate  CAL_CMEXPAPI( "CMUtlHashCreate" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashCreate
	#define EXT_CMUtlHashCreate
	#define GET_CMUtlHashCreate(fl)  CAL_CMGETAPI( "CMUtlHashCreate" ) 
	#define CAL_CMUtlHashCreate  CMUtlHashCreate
	#define CHK_CMUtlHashCreate  TRUE
	#define EXP_CMUtlHashCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashCreate", (RTS_UINTPTR)CMUtlHashCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashCreate
	#define EXT_CMUtilsHashCMUtlHashCreate
	#define GET_CMUtilsHashCMUtlHashCreate  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashCreate pICMUtilsHash->ICMUtlHashCreate
	#define CHK_CMUtilsHashCMUtlHashCreate (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashCreate
	#define EXT_CMUtlHashCreate
	#define GET_CMUtlHashCreate(fl)  CAL_CMGETAPI( "CMUtlHashCreate" ) 
	#define CAL_CMUtlHashCreate pICMUtilsHash->ICMUtlHashCreate
	#define CHK_CMUtlHashCreate (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashCreate  CAL_CMEXPAPI( "CMUtlHashCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashCreate  PFCMUTLHASHCREATE pfCMUtlHashCreate;
	#define EXT_CMUtlHashCreate  extern PFCMUTLHASHCREATE pfCMUtlHashCreate;
	#define GET_CMUtlHashCreate(fl)  s_pfCMGetAPI2( "CMUtlHashCreate", (RTS_VOID_FCTPTR *)&pfCMUtlHashCreate, (fl), 0, 0)
	#define CAL_CMUtlHashCreate  pfCMUtlHashCreate
	#define CHK_CMUtlHashCreate  (pfCMUtlHashCreate != NULL)
	#define EXP_CMUtlHashCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashCreate", (RTS_UINTPTR)CMUtlHashCreate, 0, 0) 
#endif




/**
 * <description>
 *  Function to create a hash table with a specified hash buffer.
 * </description>
 * <param name="pHashTable" type="INOUT">Pointer to a hash table object, which is filled by this function</param>
 * <param name="sNumHashEntries" type="IN">Length of the hash table respectively the number of elements that can be stored in the hash table without any collision</param>
 * <param name="pHashTableBuffer" type="IN">Pointer to the hash table (can be static memory). Length must be (sNumHashEntries * sizeof(RTS_UI8 *))!.
 *		NOTE:
 *		If pHashTableBuffer = NULL. the hash table will be allocated on heap via SysMemAllocData() within this function.
 *      The number of available hash entries should be a prime number to get a nice spreading.
 * </param>
 * <param name="pszComponentName" type="IN">Pointer to component name of the caller (which is generated typically in the macro COMPONENT_NAME)</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Hash table could be created</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid pointer or sHashTableLen is 0</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_NOMEMORY">No memory to create the hash table</errorcode> 
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlHashCreate2(CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, RTS_UI8 *pHashTableBuffer, char *pszComponentName);
typedef RTS_RESULT (CDECL * PFCMUTLHASHCREATE2) (CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, RTS_UI8 *pHashTableBuffer, char *pszComponentName);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHCREATE2_NOTIMPLEMENTED)
	#define USE_CMUtlHashCreate2
	#define EXT_CMUtlHashCreate2
	#define GET_CMUtlHashCreate2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashCreate2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashCreate2  FALSE
	#define EXP_CMUtlHashCreate2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashCreate2
	#define EXT_CMUtlHashCreate2
	#define GET_CMUtlHashCreate2(fl)  CAL_CMGETAPI( "CMUtlHashCreate2" ) 
	#define CAL_CMUtlHashCreate2  CMUtlHashCreate2
	#define CHK_CMUtlHashCreate2  TRUE
	#define EXP_CMUtlHashCreate2  CAL_CMEXPAPI( "CMUtlHashCreate2" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashCreate2
	#define EXT_CMUtlHashCreate2
	#define GET_CMUtlHashCreate2(fl)  CAL_CMGETAPI( "CMUtlHashCreate2" ) 
	#define CAL_CMUtlHashCreate2  CMUtlHashCreate2
	#define CHK_CMUtlHashCreate2  TRUE
	#define EXP_CMUtlHashCreate2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashCreate2", (RTS_UINTPTR)CMUtlHashCreate2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashCreate2
	#define EXT_CMUtilsHashCMUtlHashCreate2
	#define GET_CMUtilsHashCMUtlHashCreate2  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashCreate2 pICMUtilsHash->ICMUtlHashCreate2
	#define CHK_CMUtilsHashCMUtlHashCreate2 (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashCreate2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashCreate2
	#define EXT_CMUtlHashCreate2
	#define GET_CMUtlHashCreate2(fl)  CAL_CMGETAPI( "CMUtlHashCreate2" ) 
	#define CAL_CMUtlHashCreate2 pICMUtilsHash->ICMUtlHashCreate2
	#define CHK_CMUtlHashCreate2 (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashCreate2  CAL_CMEXPAPI( "CMUtlHashCreate2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashCreate2  PFCMUTLHASHCREATE2 pfCMUtlHashCreate2;
	#define EXT_CMUtlHashCreate2  extern PFCMUTLHASHCREATE2 pfCMUtlHashCreate2;
	#define GET_CMUtlHashCreate2(fl)  s_pfCMGetAPI2( "CMUtlHashCreate2", (RTS_VOID_FCTPTR *)&pfCMUtlHashCreate2, (fl), 0, 0)
	#define CAL_CMUtlHashCreate2  pfCMUtlHashCreate2
	#define CHK_CMUtlHashCreate2  (pfCMUtlHashCreate2 != NULL)
	#define EXP_CMUtlHashCreate2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashCreate2", (RTS_UINTPTR)CMUtlHashCreate2, 0, 0) 
#endif




/**
 * <description>
 *  Function to create a hash table with a specified hash buffer.
 * </description>
 * <param name="pHashTable" type="INOUT">Pointer to a hash table object, which is filled by this function</param>
 * <param name="sNumHashEntries" type="IN">Length of the hash table respectively the number of elements that can be stored in the hash table without any collision</param>
 * <param name="pHashTableBuffer" type="IN">Pointer to the hash table (can be static memory). Length must be (sNumHashEntries * sizeof(RTS_UI8 *))!.
 *		NOTE:
 *		If pHashTableBuffer = NULL. the hash table will be allocated on heap via SysMemAllocData() within this function.
 *      The number of available hash entries should be a prime number to get a nice spreading.
 * </param>
 * <param name="ui32Flags" type="IN">Flags how to create the hash table. See: Hash table flags</param>
 * <param name="pszComponentName" type="IN">Pointer to component name of the caller (which is generated typically in the macro COMPONENT_NAME)</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Hash table could be created</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid pointer or sHashTableLen is 0</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_NOMEMORY">No memory to create the hash table</errorcode> 
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlHashCreate3(CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, RTS_UI8 *pHashTableBuffer, RTS_UI32 ui32Flags, char *pszComponentName);
typedef RTS_RESULT (CDECL * PFCMUTLHASHCREATE3) (CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, RTS_UI8 *pHashTableBuffer, RTS_UI32 ui32Flags, char *pszComponentName);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHCREATE3_NOTIMPLEMENTED)
	#define USE_CMUtlHashCreate3
	#define EXT_CMUtlHashCreate3
	#define GET_CMUtlHashCreate3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashCreate3(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashCreate3  FALSE
	#define EXP_CMUtlHashCreate3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashCreate3
	#define EXT_CMUtlHashCreate3
	#define GET_CMUtlHashCreate3(fl)  CAL_CMGETAPI( "CMUtlHashCreate3" ) 
	#define CAL_CMUtlHashCreate3  CMUtlHashCreate3
	#define CHK_CMUtlHashCreate3  TRUE
	#define EXP_CMUtlHashCreate3  CAL_CMEXPAPI( "CMUtlHashCreate3" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashCreate3
	#define EXT_CMUtlHashCreate3
	#define GET_CMUtlHashCreate3(fl)  CAL_CMGETAPI( "CMUtlHashCreate3" ) 
	#define CAL_CMUtlHashCreate3  CMUtlHashCreate3
	#define CHK_CMUtlHashCreate3  TRUE
	#define EXP_CMUtlHashCreate3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashCreate3", (RTS_UINTPTR)CMUtlHashCreate3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashCreate3
	#define EXT_CMUtilsHashCMUtlHashCreate3
	#define GET_CMUtilsHashCMUtlHashCreate3  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashCreate3 pICMUtilsHash->ICMUtlHashCreate3
	#define CHK_CMUtilsHashCMUtlHashCreate3 (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashCreate3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashCreate3
	#define EXT_CMUtlHashCreate3
	#define GET_CMUtlHashCreate3(fl)  CAL_CMGETAPI( "CMUtlHashCreate3" ) 
	#define CAL_CMUtlHashCreate3 pICMUtilsHash->ICMUtlHashCreate3
	#define CHK_CMUtlHashCreate3 (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashCreate3  CAL_CMEXPAPI( "CMUtlHashCreate3" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashCreate3  PFCMUTLHASHCREATE3 pfCMUtlHashCreate3;
	#define EXT_CMUtlHashCreate3  extern PFCMUTLHASHCREATE3 pfCMUtlHashCreate3;
	#define GET_CMUtlHashCreate3(fl)  s_pfCMGetAPI2( "CMUtlHashCreate3", (RTS_VOID_FCTPTR *)&pfCMUtlHashCreate3, (fl), 0, 0)
	#define CAL_CMUtlHashCreate3  pfCMUtlHashCreate3
	#define CHK_CMUtlHashCreate3  (pfCMUtlHashCreate3 != NULL)
	#define EXP_CMUtlHashCreate3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashCreate3", (RTS_UINTPTR)CMUtlHashCreate3, 0, 0) 
#endif




/**
 * <description>
 *  Function to create a hash table with a specified hash buffer.
 * </description>
 * <param name="pHashTable" type="INOUT">Pointer to a hash table object, which is filled by this function</param>
 * <param name="sNumHashEntries" type="IN">Length of the hash table respectively the number of elements that can be stored in the hash table without any collision</param>
 * <param name="pHashTableBuffer" type="IN">Pointer to the hash table (can be static memory). Length must be (sNumHashEntries * sizeof(RTS_UI8 *))!.
 *		NOTE:
 *		If pHashTableBuffer = NULL. the hash table will be allocated on heap via SysMemAllocData() within this function.
 *      The number of available hash entries should be a prime number to get a nice spreading.
 * </param>
 * <param name="ui32Flags" type="IN">Flags how to create the hash table. See: Hash table flags</param>
 * <param name="hashcallback" type="IN">Callbackfunction used to calculate the hash of an arbitrary data structure. This function is used instead of the integrated hash function.</param>
 * <param name="comparecallback" type="IN">Callbackfunction used to compare two keys for equality. This function is used instead of the integrated compare function.</param>
 * <param name="pszComponentName" type="IN">Pointer to component name of the caller (which is generated typically in the macro COMPONENT_NAME)</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Hash table could be created</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid pointer or sHashTableLen is 0</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_NOMEMORY">No memory to create the hash table</errorcode> 
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlHashCreate4(CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, RTS_UI8 *pHashTableBuffer, RTS_UI32 ui32Flags, PFHASHCALLBACK hashcallback, PFCOMPARECALLBACK comparecallback, char *pszComponentName);
typedef RTS_RESULT (CDECL * PFCMUTLHASHCREATE4) (CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, RTS_UI8 *pHashTableBuffer, RTS_UI32 ui32Flags, PFHASHCALLBACK hashcallback, PFCOMPARECALLBACK comparecallback, char *pszComponentName);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHCREATE4_NOTIMPLEMENTED)
	#define USE_CMUtlHashCreate4
	#define EXT_CMUtlHashCreate4
	#define GET_CMUtlHashCreate4(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashCreate4(p0,p1,p2,p3,p4,p5,p6)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashCreate4  FALSE
	#define EXP_CMUtlHashCreate4  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashCreate4
	#define EXT_CMUtlHashCreate4
	#define GET_CMUtlHashCreate4(fl)  CAL_CMGETAPI( "CMUtlHashCreate4" ) 
	#define CAL_CMUtlHashCreate4  CMUtlHashCreate4
	#define CHK_CMUtlHashCreate4  TRUE
	#define EXP_CMUtlHashCreate4  CAL_CMEXPAPI( "CMUtlHashCreate4" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashCreate4
	#define EXT_CMUtlHashCreate4
	#define GET_CMUtlHashCreate4(fl)  CAL_CMGETAPI( "CMUtlHashCreate4" ) 
	#define CAL_CMUtlHashCreate4  CMUtlHashCreate4
	#define CHK_CMUtlHashCreate4  TRUE
	#define EXP_CMUtlHashCreate4  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashCreate4", (RTS_UINTPTR)CMUtlHashCreate4, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashCreate4
	#define EXT_CMUtilsHashCMUtlHashCreate4
	#define GET_CMUtilsHashCMUtlHashCreate4  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashCreate4 pICMUtilsHash->ICMUtlHashCreate4
	#define CHK_CMUtilsHashCMUtlHashCreate4 (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashCreate4  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashCreate4
	#define EXT_CMUtlHashCreate4
	#define GET_CMUtlHashCreate4(fl)  CAL_CMGETAPI( "CMUtlHashCreate4" ) 
	#define CAL_CMUtlHashCreate4 pICMUtilsHash->ICMUtlHashCreate4
	#define CHK_CMUtlHashCreate4 (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashCreate4  CAL_CMEXPAPI( "CMUtlHashCreate4" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashCreate4  PFCMUTLHASHCREATE4 pfCMUtlHashCreate4;
	#define EXT_CMUtlHashCreate4  extern PFCMUTLHASHCREATE4 pfCMUtlHashCreate4;
	#define GET_CMUtlHashCreate4(fl)  s_pfCMGetAPI2( "CMUtlHashCreate4", (RTS_VOID_FCTPTR *)&pfCMUtlHashCreate4, (fl), 0, 0)
	#define CAL_CMUtlHashCreate4  pfCMUtlHashCreate4
	#define CHK_CMUtlHashCreate4  (pfCMUtlHashCreate4 != NULL)
	#define EXP_CMUtlHashCreate4  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashCreate4", (RTS_UINTPTR)CMUtlHashCreate4, 0, 0) 
#endif




/**
 * <description>
 *  Function to delete a hash table.
 * </description>
 * <param name="pHashTable" type="IN">Pointer to a hash table filled by CMUtlHashCreate() or CMUtlHashCreate2()</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Hash table could be deleted</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid hash table pointer </errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Hash table could not be deleted</errorcode> 
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlHashDelete(CMUtlHashTable *pHashTable);
typedef RTS_RESULT (CDECL * PFCMUTLHASHDELETE) (CMUtlHashTable *pHashTable);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHDELETE_NOTIMPLEMENTED)
	#define USE_CMUtlHashDelete
	#define EXT_CMUtlHashDelete
	#define GET_CMUtlHashDelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashDelete(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashDelete  FALSE
	#define EXP_CMUtlHashDelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashDelete
	#define EXT_CMUtlHashDelete
	#define GET_CMUtlHashDelete(fl)  CAL_CMGETAPI( "CMUtlHashDelete" ) 
	#define CAL_CMUtlHashDelete  CMUtlHashDelete
	#define CHK_CMUtlHashDelete  TRUE
	#define EXP_CMUtlHashDelete  CAL_CMEXPAPI( "CMUtlHashDelete" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashDelete
	#define EXT_CMUtlHashDelete
	#define GET_CMUtlHashDelete(fl)  CAL_CMGETAPI( "CMUtlHashDelete" ) 
	#define CAL_CMUtlHashDelete  CMUtlHashDelete
	#define CHK_CMUtlHashDelete  TRUE
	#define EXP_CMUtlHashDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashDelete", (RTS_UINTPTR)CMUtlHashDelete, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashDelete
	#define EXT_CMUtilsHashCMUtlHashDelete
	#define GET_CMUtilsHashCMUtlHashDelete  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashDelete pICMUtilsHash->ICMUtlHashDelete
	#define CHK_CMUtilsHashCMUtlHashDelete (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashDelete  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashDelete
	#define EXT_CMUtlHashDelete
	#define GET_CMUtlHashDelete(fl)  CAL_CMGETAPI( "CMUtlHashDelete" ) 
	#define CAL_CMUtlHashDelete pICMUtilsHash->ICMUtlHashDelete
	#define CHK_CMUtlHashDelete (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashDelete  CAL_CMEXPAPI( "CMUtlHashDelete" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashDelete  PFCMUTLHASHDELETE pfCMUtlHashDelete;
	#define EXT_CMUtlHashDelete  extern PFCMUTLHASHDELETE pfCMUtlHashDelete;
	#define GET_CMUtlHashDelete(fl)  s_pfCMGetAPI2( "CMUtlHashDelete", (RTS_VOID_FCTPTR *)&pfCMUtlHashDelete, (fl), 0, 0)
	#define CAL_CMUtlHashDelete  pfCMUtlHashDelete
	#define CHK_CMUtlHashDelete  (pfCMUtlHashDelete != NULL)
	#define EXP_CMUtlHashDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashDelete", (RTS_UINTPTR)CMUtlHashDelete, 0, 0) 
#endif




/**
 * <description>
 *  Function to insert a new hash entry. New hash entry (CMUtlHashEntry) is allocated from heap via SysMemAllocData().
 *  Note: If the size of the key is equal or lower the register size of the architecture the key is directly used as hash.
 * </description>
 * <param name="pHashTable" type="IN">Pointer to a hash table filled by CMUtlHashCreate() or CMUtlHashCreate2()</param>
 * <param name="pKey" type="IN">Corresponding unique key. Can be an integer or string key.</param>
 * <param name="keyLen" type="IN">Key length in bytes</param>
 * <param name="bCopyKey" type="IN">TRUE=Key is copied. If the size of the key is bigger than the register size of the architecture 
		then the key is copied. FALSE=Only a reference to the key is hold and the value is not copied at all!
		Please take care: For integral values or addresses that should be used as keys typically TRUE has to be passed</param>
 * <param name="pData" type="IN">Pointer to data to store in hash entry</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Hash entry could be inserted</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid hash table pointer</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_NOMEMORY">Hash entry failed to insert because of insufficient memory</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Hash entry failed to insert</errorcode> 
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlHashInsert(CMUtlHashTable *pHashTable, RTS_UI8 *pKey, RTS_SIZE keyLen, RTS_BOOL bCopyKey, void *pData);
typedef RTS_RESULT (CDECL * PFCMUTLHASHINSERT) (CMUtlHashTable *pHashTable, RTS_UI8 *pKey, RTS_SIZE keyLen, RTS_BOOL bCopyKey, void *pData);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHINSERT_NOTIMPLEMENTED)
	#define USE_CMUtlHashInsert
	#define EXT_CMUtlHashInsert
	#define GET_CMUtlHashInsert(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashInsert(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashInsert  FALSE
	#define EXP_CMUtlHashInsert  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashInsert
	#define EXT_CMUtlHashInsert
	#define GET_CMUtlHashInsert(fl)  CAL_CMGETAPI( "CMUtlHashInsert" ) 
	#define CAL_CMUtlHashInsert  CMUtlHashInsert
	#define CHK_CMUtlHashInsert  TRUE
	#define EXP_CMUtlHashInsert  CAL_CMEXPAPI( "CMUtlHashInsert" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashInsert
	#define EXT_CMUtlHashInsert
	#define GET_CMUtlHashInsert(fl)  CAL_CMGETAPI( "CMUtlHashInsert" ) 
	#define CAL_CMUtlHashInsert  CMUtlHashInsert
	#define CHK_CMUtlHashInsert  TRUE
	#define EXP_CMUtlHashInsert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashInsert", (RTS_UINTPTR)CMUtlHashInsert, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashInsert
	#define EXT_CMUtilsHashCMUtlHashInsert
	#define GET_CMUtilsHashCMUtlHashInsert  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashInsert pICMUtilsHash->ICMUtlHashInsert
	#define CHK_CMUtilsHashCMUtlHashInsert (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashInsert  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashInsert
	#define EXT_CMUtlHashInsert
	#define GET_CMUtlHashInsert(fl)  CAL_CMGETAPI( "CMUtlHashInsert" ) 
	#define CAL_CMUtlHashInsert pICMUtilsHash->ICMUtlHashInsert
	#define CHK_CMUtlHashInsert (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashInsert  CAL_CMEXPAPI( "CMUtlHashInsert" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashInsert  PFCMUTLHASHINSERT pfCMUtlHashInsert;
	#define EXT_CMUtlHashInsert  extern PFCMUTLHASHINSERT pfCMUtlHashInsert;
	#define GET_CMUtlHashInsert(fl)  s_pfCMGetAPI2( "CMUtlHashInsert", (RTS_VOID_FCTPTR *)&pfCMUtlHashInsert, (fl), 0, 0)
	#define CAL_CMUtlHashInsert  pfCMUtlHashInsert
	#define CHK_CMUtlHashInsert  (pfCMUtlHashInsert != NULL)
	#define EXP_CMUtlHashInsert  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashInsert", (RTS_UINTPTR)CMUtlHashInsert, 0, 0) 
#endif




/**
 * <description>
 *  Function to insert a new hash entry.
 *  Note: If the size of the key is equal or lower the register size of the architecture the key is directly used as hash.
 * </description>
 * <param name="pHashTable" type="IN">Pointer to a hash table filled by CMUtlHashCreate() or CMUtlHashCreate2()</param>
 * <param name="pNewEntry" type="IN">Pointer to a new the hash entry memory object to be added in the hash table. Can be static memory. 
 *		If the pointer is NULL, a new hash entry (CMUtlHashEntry) is allocated from heap via SysMemAllocData().</param>
 * <param name="pKey" type="IN">Corresponding unique key. Can be an integer or string key.</param>
 * <param name="keyLen" type="IN">Key length in bytes</param>
 * <param name="bCopyKey" type="IN">TRUE=Key is copied. If the size of the key is bigger than the register size of the architecture 
		then the key is copied. FALSE=Only a reference to the key is hold and the value is not copied at all!
		Please take care: For integral values or addresses that should be used as keys typically TRUE has to be passed</param>
 * <param name="pData" type="IN">Pointer to data to store in hash entry</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Hash entry could be inserted</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid hash table pointer</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_NOMEMORY">Hash entry failed to insert because of insufficient memory</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">Hash entry failed to insert</errorcode> 
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlHashInsert2(CMUtlHashTable *pHashTable, CMUtlHashEntry *pNewEntry, RTS_UI8 *pKey, RTS_SIZE keyLen, RTS_BOOL bCopyKey, void *pData);
typedef RTS_RESULT (CDECL * PFCMUTLHASHINSERT2) (CMUtlHashTable *pHashTable, CMUtlHashEntry *pNewEntry, RTS_UI8 *pKey, RTS_SIZE keyLen, RTS_BOOL bCopyKey, void *pData);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHINSERT2_NOTIMPLEMENTED)
	#define USE_CMUtlHashInsert2
	#define EXT_CMUtlHashInsert2
	#define GET_CMUtlHashInsert2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashInsert2(p0,p1,p2,p3,p4,p5)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashInsert2  FALSE
	#define EXP_CMUtlHashInsert2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashInsert2
	#define EXT_CMUtlHashInsert2
	#define GET_CMUtlHashInsert2(fl)  CAL_CMGETAPI( "CMUtlHashInsert2" ) 
	#define CAL_CMUtlHashInsert2  CMUtlHashInsert2
	#define CHK_CMUtlHashInsert2  TRUE
	#define EXP_CMUtlHashInsert2  CAL_CMEXPAPI( "CMUtlHashInsert2" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashInsert2
	#define EXT_CMUtlHashInsert2
	#define GET_CMUtlHashInsert2(fl)  CAL_CMGETAPI( "CMUtlHashInsert2" ) 
	#define CAL_CMUtlHashInsert2  CMUtlHashInsert2
	#define CHK_CMUtlHashInsert2  TRUE
	#define EXP_CMUtlHashInsert2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashInsert2", (RTS_UINTPTR)CMUtlHashInsert2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashInsert2
	#define EXT_CMUtilsHashCMUtlHashInsert2
	#define GET_CMUtilsHashCMUtlHashInsert2  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashInsert2 pICMUtilsHash->ICMUtlHashInsert2
	#define CHK_CMUtilsHashCMUtlHashInsert2 (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashInsert2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashInsert2
	#define EXT_CMUtlHashInsert2
	#define GET_CMUtlHashInsert2(fl)  CAL_CMGETAPI( "CMUtlHashInsert2" ) 
	#define CAL_CMUtlHashInsert2 pICMUtilsHash->ICMUtlHashInsert2
	#define CHK_CMUtlHashInsert2 (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashInsert2  CAL_CMEXPAPI( "CMUtlHashInsert2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashInsert2  PFCMUTLHASHINSERT2 pfCMUtlHashInsert2;
	#define EXT_CMUtlHashInsert2  extern PFCMUTLHASHINSERT2 pfCMUtlHashInsert2;
	#define GET_CMUtlHashInsert2(fl)  s_pfCMGetAPI2( "CMUtlHashInsert2", (RTS_VOID_FCTPTR *)&pfCMUtlHashInsert2, (fl), 0, 0)
	#define CAL_CMUtlHashInsert2  pfCMUtlHashInsert2
	#define CHK_CMUtlHashInsert2  (pfCMUtlHashInsert2 != NULL)
	#define EXP_CMUtlHashInsert2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashInsert2", (RTS_UINTPTR)CMUtlHashInsert2, 0, 0) 
#endif




/**
 * <description>
 *  Function to remove a hash entry specified by its entry pointer.
 * </description>
 * <param name="pHashTable" type="IN">Pointer to a hash table filled by CMUtlHashCreate() or CMUtlHashCreate2()</param>
 * <param name="pEntry" type="IN">Pointer to the hash entry retrieved by CMUtlHashSearch</param> 
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Hash entry could be removed</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid hash table pointer or hash entry</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_OBJECT">Unknown hash entry</errorcode> 
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlHashRemove(CMUtlHashTable *pHashTable, CMUtlHashEntry *pEntry);
typedef RTS_RESULT (CDECL * PFCMUTLHASHREMOVE) (CMUtlHashTable *pHashTable, CMUtlHashEntry *pEntry);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHREMOVE_NOTIMPLEMENTED)
	#define USE_CMUtlHashRemove
	#define EXT_CMUtlHashRemove
	#define GET_CMUtlHashRemove(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashRemove(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashRemove  FALSE
	#define EXP_CMUtlHashRemove  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashRemove
	#define EXT_CMUtlHashRemove
	#define GET_CMUtlHashRemove(fl)  CAL_CMGETAPI( "CMUtlHashRemove" ) 
	#define CAL_CMUtlHashRemove  CMUtlHashRemove
	#define CHK_CMUtlHashRemove  TRUE
	#define EXP_CMUtlHashRemove  CAL_CMEXPAPI( "CMUtlHashRemove" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashRemove
	#define EXT_CMUtlHashRemove
	#define GET_CMUtlHashRemove(fl)  CAL_CMGETAPI( "CMUtlHashRemove" ) 
	#define CAL_CMUtlHashRemove  CMUtlHashRemove
	#define CHK_CMUtlHashRemove  TRUE
	#define EXP_CMUtlHashRemove  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashRemove", (RTS_UINTPTR)CMUtlHashRemove, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashRemove
	#define EXT_CMUtilsHashCMUtlHashRemove
	#define GET_CMUtilsHashCMUtlHashRemove  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashRemove pICMUtilsHash->ICMUtlHashRemove
	#define CHK_CMUtilsHashCMUtlHashRemove (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashRemove  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashRemove
	#define EXT_CMUtlHashRemove
	#define GET_CMUtlHashRemove(fl)  CAL_CMGETAPI( "CMUtlHashRemove" ) 
	#define CAL_CMUtlHashRemove pICMUtilsHash->ICMUtlHashRemove
	#define CHK_CMUtlHashRemove (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashRemove  CAL_CMEXPAPI( "CMUtlHashRemove" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashRemove  PFCMUTLHASHREMOVE pfCMUtlHashRemove;
	#define EXT_CMUtlHashRemove  extern PFCMUTLHASHREMOVE pfCMUtlHashRemove;
	#define GET_CMUtlHashRemove(fl)  s_pfCMGetAPI2( "CMUtlHashRemove", (RTS_VOID_FCTPTR *)&pfCMUtlHashRemove, (fl), 0, 0)
	#define CAL_CMUtlHashRemove  pfCMUtlHashRemove
	#define CHK_CMUtlHashRemove  (pfCMUtlHashRemove != NULL)
	#define EXP_CMUtlHashRemove  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashRemove", (RTS_UINTPTR)CMUtlHashRemove, 0, 0) 
#endif




/**
 * <description>
 *  Function to remove a hash entry specified by its key.
 * </description>
 * <param name="pHashTable" type="IN">Pointer to a hash table filled by CMUtlHashCreate() or CMUtlHashCreate2()</param>
 * <param name="pKey" type="IN">Corresponding unique key. Can be an integer or string key.</param>
 * <param name="keyLen" type="IN">Key length in bytes</param>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Hash entry could be removed</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid hash table pointer or hash entry</errorcode> 
 * <errorcode name="RTS_RESULT Result" type="ERR_NO_OBJECT">Unknown hash entry</errorcode> 
 * <result>error code</result>
 */
RTS_RESULT CDECL CMUtlHashRemove2(CMUtlHashTable *pHashTable, RTS_UI8 *pKey, RTS_SIZE keyLen);
typedef RTS_RESULT (CDECL * PFCMUTLHASHREMOVE2) (CMUtlHashTable *pHashTable, RTS_UI8 *pKey, RTS_SIZE keyLen);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHREMOVE2_NOTIMPLEMENTED)
	#define USE_CMUtlHashRemove2
	#define EXT_CMUtlHashRemove2
	#define GET_CMUtlHashRemove2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashRemove2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashRemove2  FALSE
	#define EXP_CMUtlHashRemove2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashRemove2
	#define EXT_CMUtlHashRemove2
	#define GET_CMUtlHashRemove2(fl)  CAL_CMGETAPI( "CMUtlHashRemove2" ) 
	#define CAL_CMUtlHashRemove2  CMUtlHashRemove2
	#define CHK_CMUtlHashRemove2  TRUE
	#define EXP_CMUtlHashRemove2  CAL_CMEXPAPI( "CMUtlHashRemove2" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashRemove2
	#define EXT_CMUtlHashRemove2
	#define GET_CMUtlHashRemove2(fl)  CAL_CMGETAPI( "CMUtlHashRemove2" ) 
	#define CAL_CMUtlHashRemove2  CMUtlHashRemove2
	#define CHK_CMUtlHashRemove2  TRUE
	#define EXP_CMUtlHashRemove2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashRemove2", (RTS_UINTPTR)CMUtlHashRemove2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashRemove2
	#define EXT_CMUtilsHashCMUtlHashRemove2
	#define GET_CMUtilsHashCMUtlHashRemove2  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashRemove2 pICMUtilsHash->ICMUtlHashRemove2
	#define CHK_CMUtilsHashCMUtlHashRemove2 (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashRemove2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashRemove2
	#define EXT_CMUtlHashRemove2
	#define GET_CMUtlHashRemove2(fl)  CAL_CMGETAPI( "CMUtlHashRemove2" ) 
	#define CAL_CMUtlHashRemove2 pICMUtilsHash->ICMUtlHashRemove2
	#define CHK_CMUtlHashRemove2 (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashRemove2  CAL_CMEXPAPI( "CMUtlHashRemove2" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashRemove2  PFCMUTLHASHREMOVE2 pfCMUtlHashRemove2;
	#define EXT_CMUtlHashRemove2  extern PFCMUTLHASHREMOVE2 pfCMUtlHashRemove2;
	#define GET_CMUtlHashRemove2(fl)  s_pfCMGetAPI2( "CMUtlHashRemove2", (RTS_VOID_FCTPTR *)&pfCMUtlHashRemove2, (fl), 0, 0)
	#define CAL_CMUtlHashRemove2  pfCMUtlHashRemove2
	#define CHK_CMUtlHashRemove2  (pfCMUtlHashRemove2 != NULL)
	#define EXP_CMUtlHashRemove2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashRemove2", (RTS_UINTPTR)CMUtlHashRemove2, 0, 0) 
#endif




/**
 * <description>
 *  Function to search for an entry in the hash table. 
 *  Note: If the size of the key is equal or lower the register size of the architecture the key is directly used as hash.
 * </description>
 * <param name="pHashTable" type="IN">Pointer to a hash table object, which is filled by this function</param>
 * <param name="pKey" type="IN">Pointer to the key value to search for the entry</param> 
 * <param name="keyLen" type="IN">Length of the key. If the key is a string, additionally the NUL termination character must be _included_ in the key length!</param> 
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Hash entry could be found</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid hash table pointer or pointer to key</errorcode> 
 * <errorcode name="RTS_RESULT pResult" type="ERR_NO_OBJECT">No hash entry matching to the key found</errorcode>
 * <result>Pointer to the hash entry or NULL if not found</result>
 */
CMUtlHashEntry * CDECL CMUtlHashSearch(CMUtlHashTable *pHashTable, RTS_UI8 *pKey, RTS_SIZE keyLen, RTS_RESULT *pResult);
typedef CMUtlHashEntry * (CDECL * PFCMUTLHASHSEARCH) (CMUtlHashTable *pHashTable, RTS_UI8 *pKey, RTS_SIZE keyLen, RTS_RESULT *pResult);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHSEARCH_NOTIMPLEMENTED)
	#define USE_CMUtlHashSearch
	#define EXT_CMUtlHashSearch
	#define GET_CMUtlHashSearch(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashSearch(p0,p1,p2,p3)  (CMUtlHashEntry *)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashSearch  FALSE
	#define EXP_CMUtlHashSearch  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashSearch
	#define EXT_CMUtlHashSearch
	#define GET_CMUtlHashSearch(fl)  CAL_CMGETAPI( "CMUtlHashSearch" ) 
	#define CAL_CMUtlHashSearch  CMUtlHashSearch
	#define CHK_CMUtlHashSearch  TRUE
	#define EXP_CMUtlHashSearch  CAL_CMEXPAPI( "CMUtlHashSearch" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashSearch
	#define EXT_CMUtlHashSearch
	#define GET_CMUtlHashSearch(fl)  CAL_CMGETAPI( "CMUtlHashSearch" ) 
	#define CAL_CMUtlHashSearch  CMUtlHashSearch
	#define CHK_CMUtlHashSearch  TRUE
	#define EXP_CMUtlHashSearch  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashSearch", (RTS_UINTPTR)CMUtlHashSearch, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashSearch
	#define EXT_CMUtilsHashCMUtlHashSearch
	#define GET_CMUtilsHashCMUtlHashSearch  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashSearch pICMUtilsHash->ICMUtlHashSearch
	#define CHK_CMUtilsHashCMUtlHashSearch (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashSearch  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashSearch
	#define EXT_CMUtlHashSearch
	#define GET_CMUtlHashSearch(fl)  CAL_CMGETAPI( "CMUtlHashSearch" ) 
	#define CAL_CMUtlHashSearch pICMUtilsHash->ICMUtlHashSearch
	#define CHK_CMUtlHashSearch (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashSearch  CAL_CMEXPAPI( "CMUtlHashSearch" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashSearch  PFCMUTLHASHSEARCH pfCMUtlHashSearch;
	#define EXT_CMUtlHashSearch  extern PFCMUTLHASHSEARCH pfCMUtlHashSearch;
	#define GET_CMUtlHashSearch(fl)  s_pfCMGetAPI2( "CMUtlHashSearch", (RTS_VOID_FCTPTR *)&pfCMUtlHashSearch, (fl), 0, 0)
	#define CAL_CMUtlHashSearch  pfCMUtlHashSearch
	#define CHK_CMUtlHashSearch  (pfCMUtlHashSearch != NULL)
	#define EXP_CMUtlHashSearch  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashSearch", (RTS_UINTPTR)CMUtlHashSearch, 0, 0) 
#endif




/**
 * <description>
 *  This callback function can be implemented by components iterating over the content of a hash table using <see>CMUtlHashIterate</see>.
 * </description>
 * <param name="pEntry" type="IN">The current entry of the iteration</param>
 * <param name="cbUserData" type="IN">The user provided value that was passed to CMUtlHashIterate</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">The iteration should continue if there are more entries.</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_FAILED">The iteration will be stopped and not proceed with the remaining entries after the callback returned.</errorcode> 
 */
typedef RTS_RESULT (CDECL *PF_HASHENTRY_CALLBACK)(CMUtlHashEntry *pEntry, RTS_UINTPTR cbUserData);

/**
 * <description>
 *  Function to iterate over all entries in the hash table. 
 * </description>
 * <param name="pHashTable" type="IN">Pointer to a hash table object, which will be iterated over by this function</param>
 * <param name="pfcbHashEntry" type="IN">Callback function that will be called for every entry of the hash-table.</param> 
 * <param name="cbUserData" type="IN">User provided value that will transparently be passed to all calls of pfCallback</param> 
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT Result" type="ERR_OK">Hash entry could be found</errorcode>
 * <errorcode name="RTS_RESULT Result" type="ERR_PARAMETER">Invalid hash table or callback function</errorcode> 
 */
RTS_RESULT CDECL CMUtlHashIterate(CMUtlHashTable *pHashTable, PF_HASHENTRY_CALLBACK pfcbHashEntry, RTS_UINTPTR cbUserData);
typedef RTS_RESULT (CDECL * PFCMUTLHASHITERATE) (CMUtlHashTable *pHashTable, PF_HASHENTRY_CALLBACK pfcbHashEntry, RTS_UINTPTR cbUserData);
#if defined(CMUTILSHASH_NOTIMPLEMENTED) || defined(CMUTLHASHITERATE_NOTIMPLEMENTED)
	#define USE_CMUtlHashIterate
	#define EXT_CMUtlHashIterate
	#define GET_CMUtlHashIterate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_CMUtlHashIterate(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_CMUtlHashIterate  FALSE
	#define EXP_CMUtlHashIterate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_CMUtlHashIterate
	#define EXT_CMUtlHashIterate
	#define GET_CMUtlHashIterate(fl)  CAL_CMGETAPI( "CMUtlHashIterate" ) 
	#define CAL_CMUtlHashIterate  CMUtlHashIterate
	#define CHK_CMUtlHashIterate  TRUE
	#define EXP_CMUtlHashIterate  CAL_CMEXPAPI( "CMUtlHashIterate" ) 
#elif defined(MIXED_LINK) && !defined(CMUTILSHASH_EXTERNAL)
	#define USE_CMUtlHashIterate
	#define EXT_CMUtlHashIterate
	#define GET_CMUtlHashIterate(fl)  CAL_CMGETAPI( "CMUtlHashIterate" ) 
	#define CAL_CMUtlHashIterate  CMUtlHashIterate
	#define CHK_CMUtlHashIterate  TRUE
	#define EXP_CMUtlHashIterate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashIterate", (RTS_UINTPTR)CMUtlHashIterate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_CMUtilsHashCMUtlHashIterate
	#define EXT_CMUtilsHashCMUtlHashIterate
	#define GET_CMUtilsHashCMUtlHashIterate  ERR_OK
	#define CAL_CMUtilsHashCMUtlHashIterate pICMUtilsHash->ICMUtlHashIterate
	#define CHK_CMUtilsHashCMUtlHashIterate (pICMUtilsHash != NULL)
	#define EXP_CMUtilsHashCMUtlHashIterate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_CMUtlHashIterate
	#define EXT_CMUtlHashIterate
	#define GET_CMUtlHashIterate(fl)  CAL_CMGETAPI( "CMUtlHashIterate" ) 
	#define CAL_CMUtlHashIterate pICMUtilsHash->ICMUtlHashIterate
	#define CHK_CMUtlHashIterate (pICMUtilsHash != NULL)
	#define EXP_CMUtlHashIterate  CAL_CMEXPAPI( "CMUtlHashIterate" ) 
#else /* DYNAMIC_LINK */
	#define USE_CMUtlHashIterate  PFCMUTLHASHITERATE pfCMUtlHashIterate;
	#define EXT_CMUtlHashIterate  extern PFCMUTLHASHITERATE pfCMUtlHashIterate;
	#define GET_CMUtlHashIterate(fl)  s_pfCMGetAPI2( "CMUtlHashIterate", (RTS_VOID_FCTPTR *)&pfCMUtlHashIterate, (fl), 0, 0)
	#define CAL_CMUtlHashIterate  pfCMUtlHashIterate
	#define CHK_CMUtlHashIterate  (pfCMUtlHashIterate != NULL)
	#define EXP_CMUtlHashIterate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"CMUtlHashIterate", (RTS_UINTPTR)CMUtlHashIterate, 0, 0) 
#endif





#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFCMUTLHASHCREATE ICMUtlHashCreate;
 	PFCMUTLHASHCREATE2 ICMUtlHashCreate2;
 	PFCMUTLHASHCREATE3 ICMUtlHashCreate3;
 	PFCMUTLHASHCREATE4 ICMUtlHashCreate4;
 	PFCMUTLHASHDELETE ICMUtlHashDelete;
 	PFCMUTLHASHINSERT ICMUtlHashInsert;
 	PFCMUTLHASHINSERT2 ICMUtlHashInsert2;
 	PFCMUTLHASHREMOVE ICMUtlHashRemove;
 	PFCMUTLHASHREMOVE2 ICMUtlHashRemove2;
 	PFCMUTLHASHSEARCH ICMUtlHashSearch;
 	PFCMUTLHASHITERATE ICMUtlHashIterate;
 } ICMUtilsHash_C;

#ifdef CPLUSPLUS
class ICMUtilsHash : public IBase
{
	public:
		virtual RTS_RESULT CDECL ICMUtlHashCreate(CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, char *pszComponentName) =0;
		virtual RTS_RESULT CDECL ICMUtlHashCreate2(CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, RTS_UI8 *pHashTableBuffer, char *pszComponentName) =0;
		virtual RTS_RESULT CDECL ICMUtlHashCreate3(CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, RTS_UI8 *pHashTableBuffer, RTS_UI32 ui32Flags, char *pszComponentName) =0;
		virtual RTS_RESULT CDECL ICMUtlHashCreate4(CMUtlHashTable *pHashTable, RTS_SIZE sNumHashEntries, RTS_UI8 *pHashTableBuffer, RTS_UI32 ui32Flags, PFHASHCALLBACK hashcallback, PFCOMPARECALLBACK comparecallback, char *pszComponentName) =0;
		virtual RTS_RESULT CDECL ICMUtlHashDelete(CMUtlHashTable *pHashTable) =0;
		virtual RTS_RESULT CDECL ICMUtlHashInsert(CMUtlHashTable *pHashTable, RTS_UI8 *pKey, RTS_SIZE keyLen, RTS_BOOL bCopyKey, void *pData) =0;
		virtual RTS_RESULT CDECL ICMUtlHashInsert2(CMUtlHashTable *pHashTable, CMUtlHashEntry *pNewEntry, RTS_UI8 *pKey, RTS_SIZE keyLen, RTS_BOOL bCopyKey, void *pData) =0;
		virtual RTS_RESULT CDECL ICMUtlHashRemove(CMUtlHashTable *pHashTable, CMUtlHashEntry *pEntry) =0;
		virtual RTS_RESULT CDECL ICMUtlHashRemove2(CMUtlHashTable *pHashTable, RTS_UI8 *pKey, RTS_SIZE keyLen) =0;
		virtual CMUtlHashEntry * CDECL ICMUtlHashSearch(CMUtlHashTable *pHashTable, RTS_UI8 *pKey, RTS_SIZE keyLen, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ICMUtlHashIterate(CMUtlHashTable *pHashTable, PF_HASHENTRY_CALLBACK pfcbHashEntry, RTS_UINTPTR cbUserData) =0;
};
	#ifndef ITF_CMUtilsHash
		#define ITF_CMUtilsHash static ICMUtilsHash *pICMUtilsHash = NULL;
	#endif
	#define EXTITF_CMUtilsHash
#else	/*CPLUSPLUS*/
	typedef ICMUtilsHash_C		ICMUtilsHash;
	#ifndef ITF_CMUtilsHash
		#define ITF_CMUtilsHash
	#endif
	#define EXTITF_CMUtilsHash
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_CMUTILSHASHITF_H_*/
