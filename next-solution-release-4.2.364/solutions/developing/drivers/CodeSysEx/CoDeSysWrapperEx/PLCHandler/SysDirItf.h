 /**
 * <interfacename>SysDir</interfacename>
 * <description> 
 *	<p>The SysDir interface is projected to handle all system dependent directory operations.
 *	If there is no file system on the target, the interface functions ERR_NOTIMPLEMENTED.</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _SYSDIRITF_H_
#define _SYSDIRITF_H_

#include "CmpStd.h"

 

 




#include "CmpItf.h"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *	Windows CE: Create a "Dummy.txt" file in directories.
 *	Default value is 0 (no "Dummy.txt" file in directories).
 * </description>
 */	
#define SYSDIR_WINCE_DUMMYTXT_IN_DIRS					"WinCE.DummyTxtInDirs" 
#ifndef SYSDIR_WINCE_DUMMYTXT_IN_DIRS_DEFAULT
	#define SYSDIR_WINCE_DUMMYTXT_IN_DIRS_DEFAULT				0
#endif

/**
 * <category>Directory file time</category>
 * <element name="ulCreation" type="IN">Creation date of the file</element>
 * <element name="ulLastAccess" type="IN">Date of last access to the file</element>
 * <element name="ulLastModification" type="IN">Date of last modification to the file</element>
 */
typedef struct
{
	RTS_UI32 ulCreation;
	RTS_UI32 ulLastAccess;
	RTS_UI32 ulLastModification;
} DirFileTime;

/**
 * <category>Directory info entry</category>
 * <description>
 *	This structure contains the description of one directory entry
 * </description>
 * <element name="dft" type="IN">Directory dates</element>
 * <element name="ulSize" type="IN">Size of the directory</element>
 * <element name="ulFlags" type="IN">Flag of a directory</element>
 */
typedef struct
{
	DirFileTime dft;
	RTS_SIZE ulSize;
	RTS_UI32 ulFlags;
} DirInfo;

/**
 * <category>Directory info flags</category>
 * <element name="DF_FILE" type="IN">Entry is a file</element>
 * <element name="DF_DIRECTORY" type="IN">Entry is a directory</element>
 */
#define DF_FILE			0x00000000
#define DF_DIRECTORY	0x00000002


/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * Close an open directory
 * :return: Returns the runtime system error code (see CmpErrors_Itf.library)
 */
typedef struct tagsysdirclose_struct
{
	RTS_IEC_HANDLE hDir;				/* VAR_INPUT */	/* Handle to directory opened with SysDirOpen */
	RTS_IEC_RESULT SysDirClose;			/* VAR_OUTPUT */	
} sysdirclose_struct;

void CDECL CDECL_EXT sysdirclose(sysdirclose_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIRCLOSE_IEC) (sysdirclose_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRCLOSE_NOTIMPLEMENTED)
	#define USE_sysdirclose
	#define EXT_sysdirclose
	#define GET_sysdirclose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdirclose(p0) 
	#define CHK_sysdirclose  FALSE
	#define EXP_sysdirclose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdirclose
	#define EXT_sysdirclose
	#define GET_sysdirclose(fl)  CAL_CMGETAPI( "sysdirclose" ) 
	#define CAL_sysdirclose  sysdirclose
	#define CHK_sysdirclose  TRUE
	#define EXP_sysdirclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirclose", (RTS_UINTPTR)sysdirclose, 1, RTSITF_GET_SIGNATURE(0, 0x7985F4ED), 0x03051100) 
	#define EXTREF_ITF_sysdirclose {(RTS_VOID_FCTPTR)sysdirclose, "sysdirclose", RTSITF_GET_SIGNATURE(0, 0x7985F4ED), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdirclose
	#define EXT_sysdirclose
	#define GET_sysdirclose(fl)  CAL_CMGETAPI( "sysdirclose" ) 
	#define CAL_sysdirclose  sysdirclose
	#define CHK_sysdirclose  TRUE
	#define EXP_sysdirclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirclose", (RTS_UINTPTR)sysdirclose, 1, RTSITF_GET_SIGNATURE(0, 0x7985F4ED), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdirclose
	#define EXT_SysDirsysdirclose
	#define GET_SysDirsysdirclose  ERR_OK
	#define CAL_SysDirsysdirclose  sysdirclose
	#define CHK_SysDirsysdirclose  TRUE
	#define EXP_SysDirsysdirclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirclose", (RTS_UINTPTR)sysdirclose, 1, RTSITF_GET_SIGNATURE(0, 0x7985F4ED), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdirclose
	#define EXT_sysdirclose
	#define GET_sysdirclose(fl)  CAL_CMGETAPI( "sysdirclose" ) 
	#define CAL_sysdirclose  sysdirclose
	#define CHK_sysdirclose  TRUE
	#define EXP_sysdirclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirclose", (RTS_UINTPTR)sysdirclose, 1, RTSITF_GET_SIGNATURE(0, 0x7985F4ED), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdirclose  PFSYSDIRCLOSE_IEC pfsysdirclose;
	#define EXT_sysdirclose  extern PFSYSDIRCLOSE_IEC pfsysdirclose;
	#define GET_sysdirclose(fl)  s_pfCMGetAPI2( "sysdirclose", (RTS_VOID_FCTPTR *)&pfsysdirclose, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x7985F4ED), 0x03051100)
	#define CAL_sysdirclose  pfsysdirclose
	#define CHK_sysdirclose  (pfsysdirclose != NULL)
	#define EXP_sysdirclose   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirclose", (RTS_UINTPTR)sysdirclose, 1, RTSITF_GET_SIGNATURE(0, 0x7985F4ED), 0x03051100) 
#endif


/**
 * Copies the contents of szSourceDir to szDestDir.
 * Nonexisting destination path will be created.
 * Depending of the parameters, all subdirectories and its content will be copied too, and existing files will be overwritten.
 * NOTE: Empty direcories will be copied if xRecursive is TRUE.
 * :return: Returns the runtime system error code (see CmpErrors_Itf.library)
 */
typedef struct tagsysdircopy_struct
{
	RTS_IEC_STRING *szDestDir;			/* VAR_INPUT */	/* Name of destination directory */
	RTS_IEC_STRING *szSourceDir;		/* VAR_INPUT */	/* Name of source directory */
	RTS_IEC_BOOL xRecursive;			/* VAR_INPUT */	/* TRUE: all subdirectories and their contents are copied, FALSE: subdirectories are omitted */
	RTS_IEC_BOOL xOverwrite;			/* VAR_INPUT */	/* TRUE: existing files are overwritten, FALSE: existing files are left untouched */
	RTS_IEC_RESULT SysDirCopy;			/* VAR_OUTPUT */	
} sysdircopy_struct;

void CDECL CDECL_EXT sysdircopy(sysdircopy_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIRCOPY_IEC) (sysdircopy_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRCOPY_NOTIMPLEMENTED)
	#define USE_sysdircopy
	#define EXT_sysdircopy
	#define GET_sysdircopy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdircopy(p0) 
	#define CHK_sysdircopy  FALSE
	#define EXP_sysdircopy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdircopy
	#define EXT_sysdircopy
	#define GET_sysdircopy(fl)  CAL_CMGETAPI( "sysdircopy" ) 
	#define CAL_sysdircopy  sysdircopy
	#define CHK_sysdircopy  TRUE
	#define EXP_sysdircopy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircopy", (RTS_UINTPTR)sysdircopy, 1, 0xDCC4B146, 0x03051100) 
	#define EXTREF_ITF_sysdircopy {(RTS_VOID_FCTPTR)sysdircopy, "sysdircopy", 0xDCC4B146, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdircopy
	#define EXT_sysdircopy
	#define GET_sysdircopy(fl)  CAL_CMGETAPI( "sysdircopy" ) 
	#define CAL_sysdircopy  sysdircopy
	#define CHK_sysdircopy  TRUE
	#define EXP_sysdircopy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircopy", (RTS_UINTPTR)sysdircopy, 1, 0xDCC4B146, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdircopy
	#define EXT_SysDirsysdircopy
	#define GET_SysDirsysdircopy  ERR_OK
	#define CAL_SysDirsysdircopy  sysdircopy
	#define CHK_SysDirsysdircopy  TRUE
	#define EXP_SysDirsysdircopy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircopy", (RTS_UINTPTR)sysdircopy, 1, 0xDCC4B146, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdircopy
	#define EXT_sysdircopy
	#define GET_sysdircopy(fl)  CAL_CMGETAPI( "sysdircopy" ) 
	#define CAL_sysdircopy  sysdircopy
	#define CHK_sysdircopy  TRUE
	#define EXP_sysdircopy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircopy", (RTS_UINTPTR)sysdircopy, 1, 0xDCC4B146, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdircopy  PFSYSDIRCOPY_IEC pfsysdircopy;
	#define EXT_sysdircopy  extern PFSYSDIRCOPY_IEC pfsysdircopy;
	#define GET_sysdircopy(fl)  s_pfCMGetAPI2( "sysdircopy", (RTS_VOID_FCTPTR *)&pfsysdircopy, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xDCC4B146, 0x03051100)
	#define CAL_sysdircopy  pfsysdircopy
	#define CHK_sysdircopy  (pfsysdircopy != NULL)
	#define EXP_sysdircopy   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircopy", (RTS_UINTPTR)sysdircopy, 1, 0xDCC4B146, 0x03051100) 
#endif


/**
 * Creates a new directory with the specified name
 * :return: Returns the runtime system error code (see CmpErrors_Itf.library)
 */
typedef struct tagsysdircreate_struct
{
	RTS_IEC_STRING *szDir;				/* VAR_INPUT */	/* Name of the directory */
	RTS_IEC_RESULT SysDirCreate;		/* VAR_OUTPUT */	
} sysdircreate_struct;

void CDECL CDECL_EXT sysdircreate(sysdircreate_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIRCREATE_IEC) (sysdircreate_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRCREATE_NOTIMPLEMENTED)
	#define USE_sysdircreate
	#define EXT_sysdircreate
	#define GET_sysdircreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdircreate(p0) 
	#define CHK_sysdircreate  FALSE
	#define EXP_sysdircreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdircreate
	#define EXT_sysdircreate
	#define GET_sysdircreate(fl)  CAL_CMGETAPI( "sysdircreate" ) 
	#define CAL_sysdircreate  sysdircreate
	#define CHK_sysdircreate  TRUE
	#define EXP_sysdircreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircreate", (RTS_UINTPTR)sysdircreate, 1, RTSITF_GET_SIGNATURE(0, 0xC775A9FA), 0x03051100) 
	#define EXTREF_ITF_sysdircreate {(RTS_VOID_FCTPTR)sysdircreate, "sysdircreate", RTSITF_GET_SIGNATURE(0, 0xC775A9FA), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdircreate
	#define EXT_sysdircreate
	#define GET_sysdircreate(fl)  CAL_CMGETAPI( "sysdircreate" ) 
	#define CAL_sysdircreate  sysdircreate
	#define CHK_sysdircreate  TRUE
	#define EXP_sysdircreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircreate", (RTS_UINTPTR)sysdircreate, 1, RTSITF_GET_SIGNATURE(0, 0xC775A9FA), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdircreate
	#define EXT_SysDirsysdircreate
	#define GET_SysDirsysdircreate  ERR_OK
	#define CAL_SysDirsysdircreate  sysdircreate
	#define CHK_SysDirsysdircreate  TRUE
	#define EXP_SysDirsysdircreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircreate", (RTS_UINTPTR)sysdircreate, 1, RTSITF_GET_SIGNATURE(0, 0xC775A9FA), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdircreate
	#define EXT_sysdircreate
	#define GET_sysdircreate(fl)  CAL_CMGETAPI( "sysdircreate" ) 
	#define CAL_sysdircreate  sysdircreate
	#define CHK_sysdircreate  TRUE
	#define EXP_sysdircreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircreate", (RTS_UINTPTR)sysdircreate, 1, RTSITF_GET_SIGNATURE(0, 0xC775A9FA), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdircreate  PFSYSDIRCREATE_IEC pfsysdircreate;
	#define EXT_sysdircreate  extern PFSYSDIRCREATE_IEC pfsysdircreate;
	#define GET_sysdircreate(fl)  s_pfCMGetAPI2( "sysdircreate", (RTS_VOID_FCTPTR *)&pfsysdircreate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xC775A9FA), 0x03051100)
	#define CAL_sysdircreate  pfsysdircreate
	#define CHK_sysdircreate  (pfsysdircreate != NULL)
	#define EXP_sysdircreate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircreate", (RTS_UINTPTR)sysdircreate, 1, RTSITF_GET_SIGNATURE(0, 0xC775A9FA), 0x03051100) 
#endif


/**
 * Creates a new directory with the specified name.
 * If xRecursive is set, a full path is created, i.e. all missing parent directories are created too.
 * :return: Returns the runtime system error code (see CmpErrors_Itf.library)
 */
typedef struct tagsysdircreate2_struct
{
	RTS_IEC_STRING *szDir;				/* VAR_INPUT */	/* Name of the directory */
	RTS_IEC_BOOL xRecursive;			/* VAR_INPUT */	/* Create a full path */
	RTS_IEC_RESULT SysDirCreate2;		/* VAR_OUTPUT */	
} sysdircreate2_struct;

void CDECL CDECL_EXT sysdircreate2(sysdircreate2_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIRCREATE2_IEC) (sysdircreate2_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRCREATE2_NOTIMPLEMENTED)
	#define USE_sysdircreate2
	#define EXT_sysdircreate2
	#define GET_sysdircreate2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdircreate2(p0) 
	#define CHK_sysdircreate2  FALSE
	#define EXP_sysdircreate2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdircreate2
	#define EXT_sysdircreate2
	#define GET_sysdircreate2(fl)  CAL_CMGETAPI( "sysdircreate2" ) 
	#define CAL_sysdircreate2  sysdircreate2
	#define CHK_sysdircreate2  TRUE
	#define EXP_sysdircreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircreate2", (RTS_UINTPTR)sysdircreate2, 1, 0x03802B43, 0x03051100) 
	#define EXTREF_ITF_sysdircreate2 {(RTS_VOID_FCTPTR)sysdircreate2, "sysdircreate2", 0x03802B43, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdircreate2
	#define EXT_sysdircreate2
	#define GET_sysdircreate2(fl)  CAL_CMGETAPI( "sysdircreate2" ) 
	#define CAL_sysdircreate2  sysdircreate2
	#define CHK_sysdircreate2  TRUE
	#define EXP_sysdircreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircreate2", (RTS_UINTPTR)sysdircreate2, 1, 0x03802B43, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdircreate2
	#define EXT_SysDirsysdircreate2
	#define GET_SysDirsysdircreate2  ERR_OK
	#define CAL_SysDirsysdircreate2  sysdircreate2
	#define CHK_SysDirsysdircreate2  TRUE
	#define EXP_SysDirsysdircreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircreate2", (RTS_UINTPTR)sysdircreate2, 1, 0x03802B43, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdircreate2
	#define EXT_sysdircreate2
	#define GET_sysdircreate2(fl)  CAL_CMGETAPI( "sysdircreate2" ) 
	#define CAL_sysdircreate2  sysdircreate2
	#define CHK_sysdircreate2  TRUE
	#define EXP_sysdircreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircreate2", (RTS_UINTPTR)sysdircreate2, 1, 0x03802B43, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdircreate2  PFSYSDIRCREATE2_IEC pfsysdircreate2;
	#define EXT_sysdircreate2  extern PFSYSDIRCREATE2_IEC pfsysdircreate2;
	#define GET_sysdircreate2(fl)  s_pfCMGetAPI2( "sysdircreate2", (RTS_VOID_FCTPTR *)&pfsysdircreate2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x03802B43, 0x03051100)
	#define CAL_sysdircreate2  pfsysdircreate2
	#define CHK_sysdircreate2  (pfsysdircreate2 != NULL)
	#define EXP_sysdircreate2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdircreate2", (RTS_UINTPTR)sysdircreate2, 1, 0x03802B43, 0x03051100) 
#endif


/**
 * Deletes a directory with the specified name
 * :return: Returns the runtime system error code (see CmpErrors_Itf.library)
 */
typedef struct tagsysdirdelete_struct
{
	RTS_IEC_STRING *szDir;				/* VAR_INPUT */	/* Name of directory */
	RTS_IEC_RESULT SysDirDelete;		/* VAR_OUTPUT */	
} sysdirdelete_struct;

void CDECL CDECL_EXT sysdirdelete(sysdirdelete_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIRDELETE_IEC) (sysdirdelete_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRDELETE_NOTIMPLEMENTED)
	#define USE_sysdirdelete
	#define EXT_sysdirdelete
	#define GET_sysdirdelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdirdelete(p0) 
	#define CHK_sysdirdelete  FALSE
	#define EXP_sysdirdelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdirdelete
	#define EXT_sysdirdelete
	#define GET_sysdirdelete(fl)  CAL_CMGETAPI( "sysdirdelete" ) 
	#define CAL_sysdirdelete  sysdirdelete
	#define CHK_sysdirdelete  TRUE
	#define EXP_sysdirdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirdelete", (RTS_UINTPTR)sysdirdelete, 1, RTSITF_GET_SIGNATURE(0, 0x3E032B8B), 0x03051100) 
	#define EXTREF_ITF_sysdirdelete {(RTS_VOID_FCTPTR)sysdirdelete, "sysdirdelete", RTSITF_GET_SIGNATURE(0, 0x3E032B8B), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdirdelete
	#define EXT_sysdirdelete
	#define GET_sysdirdelete(fl)  CAL_CMGETAPI( "sysdirdelete" ) 
	#define CAL_sysdirdelete  sysdirdelete
	#define CHK_sysdirdelete  TRUE
	#define EXP_sysdirdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirdelete", (RTS_UINTPTR)sysdirdelete, 1, RTSITF_GET_SIGNATURE(0, 0x3E032B8B), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdirdelete
	#define EXT_SysDirsysdirdelete
	#define GET_SysDirsysdirdelete  ERR_OK
	#define CAL_SysDirsysdirdelete  sysdirdelete
	#define CHK_SysDirsysdirdelete  TRUE
	#define EXP_SysDirsysdirdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirdelete", (RTS_UINTPTR)sysdirdelete, 1, RTSITF_GET_SIGNATURE(0, 0x3E032B8B), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdirdelete
	#define EXT_sysdirdelete
	#define GET_sysdirdelete(fl)  CAL_CMGETAPI( "sysdirdelete" ) 
	#define CAL_sysdirdelete  sysdirdelete
	#define CHK_sysdirdelete  TRUE
	#define EXP_sysdirdelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirdelete", (RTS_UINTPTR)sysdirdelete, 1, RTSITF_GET_SIGNATURE(0, 0x3E032B8B), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdirdelete  PFSYSDIRDELETE_IEC pfsysdirdelete;
	#define EXT_sysdirdelete  extern PFSYSDIRDELETE_IEC pfsysdirdelete;
	#define GET_sysdirdelete(fl)  s_pfCMGetAPI2( "sysdirdelete", (RTS_VOID_FCTPTR *)&pfsysdirdelete, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x3E032B8B), 0x03051100)
	#define CAL_sysdirdelete  pfsysdirdelete
	#define CHK_sysdirdelete  (pfsysdirdelete != NULL)
	#define EXP_sysdirdelete   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirdelete", (RTS_UINTPTR)sysdirdelete, 1, RTSITF_GET_SIGNATURE(0, 0x3E032B8B), 0x03051100) 
#endif


/**
 * Deletes a directory with the specified name
 *
 * If xRecursive is set, a whole folder tree can be deleted, i.e. all sub-folders and files are deleted.
 * If xRecursive and xKeepDir are set, the folder is kept but its contents is deleted.
 * :return: Returns the runtime system error code (see CmpErrors_Itf.library)
 */
typedef struct tagsysdirdelete2_struct
{
	RTS_IEC_STRING *szDir;				/* VAR_INPUT */	/* Name of directory */
	RTS_IEC_BOOL xRecursive;			/* VAR_INPUT */	/* Delete a whole folder tree */
	RTS_IEC_BOOL xKeepDir;				/* VAR_INPUT */	/* Keep the starting folder */
	RTS_IEC_RESULT SysDirDelete2;		/* VAR_OUTPUT */	
} sysdirdelete2_struct;

void CDECL CDECL_EXT sysdirdelete2(sysdirdelete2_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIRDELETE2_IEC) (sysdirdelete2_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRDELETE2_NOTIMPLEMENTED)
	#define USE_sysdirdelete2
	#define EXT_sysdirdelete2
	#define GET_sysdirdelete2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdirdelete2(p0) 
	#define CHK_sysdirdelete2  FALSE
	#define EXP_sysdirdelete2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdirdelete2
	#define EXT_sysdirdelete2
	#define GET_sysdirdelete2(fl)  CAL_CMGETAPI( "sysdirdelete2" ) 
	#define CAL_sysdirdelete2  sysdirdelete2
	#define CHK_sysdirdelete2  TRUE
	#define EXP_sysdirdelete2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirdelete2", (RTS_UINTPTR)sysdirdelete2, 1, 0xB8EF3C3D, 0x03051100) 
	#define EXTREF_ITF_sysdirdelete2 {(RTS_VOID_FCTPTR)sysdirdelete2, "sysdirdelete2", 0xB8EF3C3D, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdirdelete2
	#define EXT_sysdirdelete2
	#define GET_sysdirdelete2(fl)  CAL_CMGETAPI( "sysdirdelete2" ) 
	#define CAL_sysdirdelete2  sysdirdelete2
	#define CHK_sysdirdelete2  TRUE
	#define EXP_sysdirdelete2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirdelete2", (RTS_UINTPTR)sysdirdelete2, 1, 0xB8EF3C3D, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdirdelete2
	#define EXT_SysDirsysdirdelete2
	#define GET_SysDirsysdirdelete2  ERR_OK
	#define CAL_SysDirsysdirdelete2  sysdirdelete2
	#define CHK_SysDirsysdirdelete2  TRUE
	#define EXP_SysDirsysdirdelete2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirdelete2", (RTS_UINTPTR)sysdirdelete2, 1, 0xB8EF3C3D, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdirdelete2
	#define EXT_sysdirdelete2
	#define GET_sysdirdelete2(fl)  CAL_CMGETAPI( "sysdirdelete2" ) 
	#define CAL_sysdirdelete2  sysdirdelete2
	#define CHK_sysdirdelete2  TRUE
	#define EXP_sysdirdelete2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirdelete2", (RTS_UINTPTR)sysdirdelete2, 1, 0xB8EF3C3D, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdirdelete2  PFSYSDIRDELETE2_IEC pfsysdirdelete2;
	#define EXT_sysdirdelete2  extern PFSYSDIRDELETE2_IEC pfsysdirdelete2;
	#define GET_sysdirdelete2(fl)  s_pfCMGetAPI2( "sysdirdelete2", (RTS_VOID_FCTPTR *)&pfsysdirdelete2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xB8EF3C3D, 0x03051100)
	#define CAL_sysdirdelete2  pfsysdirdelete2
	#define CHK_sysdirdelete2  (pfsysdirdelete2 != NULL)
	#define EXP_sysdirdelete2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirdelete2", (RTS_UINTPTR)sysdirdelete2, 1, 0xB8EF3C3D, 0x03051100) 
#endif


/**
 * Get current working directory for IEC file access
 * :return: Returns the runtime system error code (see CmpErrors_Itf.library)
 */
typedef struct tagsysdirgetcurrent_struct
{
	RTS_IEC_STRING *szDir;				/* VAR_IN_OUT */	/* OUT_PARAMETER: Name of current directory */
	RTS_IEC_DINT diMaxDirLen;			/* VAR_INPUT */	/* Max length of directory buffer */
	RTS_IEC_RESULT SysDirGetCurrent;	/* VAR_OUTPUT */	
} sysdirgetcurrent_struct;

void CDECL CDECL_EXT sysdirgetcurrent(sysdirgetcurrent_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIRGETCURRENT_IEC) (sysdirgetcurrent_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRGETCURRENT_NOTIMPLEMENTED)
	#define USE_sysdirgetcurrent
	#define EXT_sysdirgetcurrent
	#define GET_sysdirgetcurrent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdirgetcurrent(p0) 
	#define CHK_sysdirgetcurrent  FALSE
	#define EXP_sysdirgetcurrent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdirgetcurrent
	#define EXT_sysdirgetcurrent
	#define GET_sysdirgetcurrent(fl)  CAL_CMGETAPI( "sysdirgetcurrent" ) 
	#define CAL_sysdirgetcurrent  sysdirgetcurrent
	#define CHK_sysdirgetcurrent  TRUE
	#define EXP_sysdirgetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirgetcurrent", (RTS_UINTPTR)sysdirgetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x0376B53C), 0x03051100) 
	#define EXTREF_ITF_sysdirgetcurrent {(RTS_VOID_FCTPTR)sysdirgetcurrent, "sysdirgetcurrent", RTSITF_GET_SIGNATURE(0, 0x0376B53C), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdirgetcurrent
	#define EXT_sysdirgetcurrent
	#define GET_sysdirgetcurrent(fl)  CAL_CMGETAPI( "sysdirgetcurrent" ) 
	#define CAL_sysdirgetcurrent  sysdirgetcurrent
	#define CHK_sysdirgetcurrent  TRUE
	#define EXP_sysdirgetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirgetcurrent", (RTS_UINTPTR)sysdirgetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x0376B53C), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdirgetcurrent
	#define EXT_SysDirsysdirgetcurrent
	#define GET_SysDirsysdirgetcurrent  ERR_OK
	#define CAL_SysDirsysdirgetcurrent  sysdirgetcurrent
	#define CHK_SysDirsysdirgetcurrent  TRUE
	#define EXP_SysDirsysdirgetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirgetcurrent", (RTS_UINTPTR)sysdirgetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x0376B53C), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdirgetcurrent
	#define EXT_sysdirgetcurrent
	#define GET_sysdirgetcurrent(fl)  CAL_CMGETAPI( "sysdirgetcurrent" ) 
	#define CAL_sysdirgetcurrent  sysdirgetcurrent
	#define CHK_sysdirgetcurrent  TRUE
	#define EXP_sysdirgetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirgetcurrent", (RTS_UINTPTR)sysdirgetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x0376B53C), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdirgetcurrent  PFSYSDIRGETCURRENT_IEC pfsysdirgetcurrent;
	#define EXT_sysdirgetcurrent  extern PFSYSDIRGETCURRENT_IEC pfsysdirgetcurrent;
	#define GET_sysdirgetcurrent(fl)  s_pfCMGetAPI2( "sysdirgetcurrent", (RTS_VOID_FCTPTR *)&pfsysdirgetcurrent, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x0376B53C), 0x03051100)
	#define CAL_sysdirgetcurrent  pfsysdirgetcurrent
	#define CHK_sysdirgetcurrent  (pfsysdirgetcurrent != NULL)
	#define EXP_sysdirgetcurrent   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirgetcurrent", (RTS_UINTPTR)sysdirgetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x0376B53C), 0x03051100) 
#endif


/**
 * | Opens a specified directory and returns a handle and the first directory entry.
 * | szDirEntry is expected to provide enough space to write the directory entry to.
 * | In case szDirEntry is 0, the first directory entry may be retrieved with SysDirRead.
 * :return: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysdiropen_struct
{
	RTS_IEC_STRING *szDir;				/* VAR_INPUT */	/* Name of directory.

 .. note:: Empty string ('') is the request for the current working directory. */
	RTS_IEC_STRING *szDirEntry;			/* VAR_INPUT */	/* OUT_PARAMETER, optional: Directory entry as string, may be 0 */
	RTS_IEC_DINT diMaxDirEntry;			/* VAR_INPUT */	/* OUT_PARAMETER, optional: Max number of bytes to write in pszDirEntry */
	DirInfo *pDirInfo;					/* VAR_INPUT */	/* OUT_PARAMETER, optional: Directory information, may be 0 */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* OUT_PARAMETER: Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_HANDLE SysDirOpen;			/* VAR_OUTPUT */	
} sysdiropen_struct;

void CDECL CDECL_EXT sysdiropen(sysdiropen_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIROPEN_IEC) (sysdiropen_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIROPEN_NOTIMPLEMENTED)
	#define USE_sysdiropen
	#define EXT_sysdiropen
	#define GET_sysdiropen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdiropen(p0) 
	#define CHK_sysdiropen  FALSE
	#define EXP_sysdiropen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdiropen
	#define EXT_sysdiropen
	#define GET_sysdiropen(fl)  CAL_CMGETAPI( "sysdiropen" ) 
	#define CAL_sysdiropen  sysdiropen
	#define CHK_sysdiropen  TRUE
	#define EXP_sysdiropen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdiropen", (RTS_UINTPTR)sysdiropen, 1, RTSITF_GET_SIGNATURE(0, 0x05CA15A4), 0x03051100) 
	#define EXTREF_ITF_sysdiropen {(RTS_VOID_FCTPTR)sysdiropen, "sysdiropen", RTSITF_GET_SIGNATURE(0, 0x05CA15A4), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdiropen
	#define EXT_sysdiropen
	#define GET_sysdiropen(fl)  CAL_CMGETAPI( "sysdiropen" ) 
	#define CAL_sysdiropen  sysdiropen
	#define CHK_sysdiropen  TRUE
	#define EXP_sysdiropen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdiropen", (RTS_UINTPTR)sysdiropen, 1, RTSITF_GET_SIGNATURE(0, 0x05CA15A4), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdiropen
	#define EXT_SysDirsysdiropen
	#define GET_SysDirsysdiropen  ERR_OK
	#define CAL_SysDirsysdiropen  sysdiropen
	#define CHK_SysDirsysdiropen  TRUE
	#define EXP_SysDirsysdiropen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdiropen", (RTS_UINTPTR)sysdiropen, 1, RTSITF_GET_SIGNATURE(0, 0x05CA15A4), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdiropen
	#define EXT_sysdiropen
	#define GET_sysdiropen(fl)  CAL_CMGETAPI( "sysdiropen" ) 
	#define CAL_sysdiropen  sysdiropen
	#define CHK_sysdiropen  TRUE
	#define EXP_sysdiropen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdiropen", (RTS_UINTPTR)sysdiropen, 1, RTSITF_GET_SIGNATURE(0, 0x05CA15A4), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdiropen  PFSYSDIROPEN_IEC pfsysdiropen;
	#define EXT_sysdiropen  extern PFSYSDIROPEN_IEC pfsysdiropen;
	#define GET_sysdiropen(fl)  s_pfCMGetAPI2( "sysdiropen", (RTS_VOID_FCTPTR *)&pfsysdiropen, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x05CA15A4), 0x03051100)
	#define CAL_sysdiropen  pfsysdiropen
	#define CHK_sysdiropen  (pfsysdiropen != NULL)
	#define EXP_sysdiropen   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdiropen", (RTS_UINTPTR)sysdiropen, 1, RTSITF_GET_SIGNATURE(0, 0x05CA15A4), 0x03051100) 
#endif


/**
 * Read next directory entry. Writes the entry in pszDirEntry.
 * :return: Returns the runtime system error code (see CmpErrors.library)
 *
 *		+ ERR_OK: Entry could be read
 *		+ ERR_END_OF_OBJECT: If end of directory list was reached
 *		+ ERR_PARAMETER: If one of the parameters is invalid
 *		+ ERR_BUFFERSIZE: If iMaxDirEntry is too short to get the complete directory entry string
 *
 * .. note:: Typically after error ``ERR_BUFFERSIZE``, the directory-handle is set to the next entry, so this entry will be missed!
 */
typedef struct tagsysdirread_struct
{
	RTS_IEC_HANDLE hDir;				/* VAR_INPUT */	/* Handle to directory opened with SysDirOpen */
	RTS_IEC_STRING *szDirEntry;			/* VAR_IN_OUT */	/* OUT_PARAMETER: Directory entry as string */
	RTS_IEC_DINT diMaxDirEntry;			/* VAR_INPUT */	/* OUT_PARAMETER: Max number of bytes to write in pszDirEntry */
	DirInfo *pDirInfo;					/* VAR_INPUT */	/* OUT_PARAMETER: Directory information
 NOTE: Can be 0 (so only directory name is provided in pszDirEntry) */
	RTS_IEC_RESULT SysDirRead;			/* VAR_OUTPUT */	
} sysdirread_struct;

void CDECL CDECL_EXT sysdirread(sysdirread_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIRREAD_IEC) (sysdirread_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRREAD_NOTIMPLEMENTED)
	#define USE_sysdirread
	#define EXT_sysdirread
	#define GET_sysdirread(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdirread(p0) 
	#define CHK_sysdirread  FALSE
	#define EXP_sysdirread  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdirread
	#define EXT_sysdirread
	#define GET_sysdirread(fl)  CAL_CMGETAPI( "sysdirread" ) 
	#define CAL_sysdirread  sysdirread
	#define CHK_sysdirread  TRUE
	#define EXP_sysdirread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirread", (RTS_UINTPTR)sysdirread, 1, RTSITF_GET_SIGNATURE(0, 0xCF257171), 0x03051100) 
	#define EXTREF_ITF_sysdirread {(RTS_VOID_FCTPTR)sysdirread, "sysdirread", RTSITF_GET_SIGNATURE(0, 0xCF257171), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdirread
	#define EXT_sysdirread
	#define GET_sysdirread(fl)  CAL_CMGETAPI( "sysdirread" ) 
	#define CAL_sysdirread  sysdirread
	#define CHK_sysdirread  TRUE
	#define EXP_sysdirread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirread", (RTS_UINTPTR)sysdirread, 1, RTSITF_GET_SIGNATURE(0, 0xCF257171), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdirread
	#define EXT_SysDirsysdirread
	#define GET_SysDirsysdirread  ERR_OK
	#define CAL_SysDirsysdirread  sysdirread
	#define CHK_SysDirsysdirread  TRUE
	#define EXP_SysDirsysdirread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirread", (RTS_UINTPTR)sysdirread, 1, RTSITF_GET_SIGNATURE(0, 0xCF257171), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdirread
	#define EXT_sysdirread
	#define GET_sysdirread(fl)  CAL_CMGETAPI( "sysdirread" ) 
	#define CAL_sysdirread  sysdirread
	#define CHK_sysdirread  TRUE
	#define EXP_sysdirread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirread", (RTS_UINTPTR)sysdirread, 1, RTSITF_GET_SIGNATURE(0, 0xCF257171), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdirread  PFSYSDIRREAD_IEC pfsysdirread;
	#define EXT_sysdirread  extern PFSYSDIRREAD_IEC pfsysdirread;
	#define GET_sysdirread(fl)  s_pfCMGetAPI2( "sysdirread", (RTS_VOID_FCTPTR *)&pfsysdirread, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xCF257171), 0x03051100)
	#define CAL_sysdirread  pfsysdirread
	#define CHK_sysdirread  (pfsysdirread != NULL)
	#define EXP_sysdirread   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirread", (RTS_UINTPTR)sysdirread, 1, RTSITF_GET_SIGNATURE(0, 0xCF257171), 0x03051100) 
#endif


/**
 * Rename directory
 * :return: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysdirrename_struct
{
	RTS_IEC_STRING *szOldDirName;		/* VAR_INPUT */	/* Name of existing directory */
	RTS_IEC_STRING *szNewDirName;		/* VAR_INPUT */	/* Name of new directory */
	RTS_IEC_RESULT SysDirRename;		/* VAR_OUTPUT */	
} sysdirrename_struct;

void CDECL CDECL_EXT sysdirrename(sysdirrename_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIRRENAME_IEC) (sysdirrename_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRRENAME_NOTIMPLEMENTED)
	#define USE_sysdirrename
	#define EXT_sysdirrename
	#define GET_sysdirrename(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdirrename(p0) 
	#define CHK_sysdirrename  FALSE
	#define EXP_sysdirrename  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdirrename
	#define EXT_sysdirrename
	#define GET_sysdirrename(fl)  CAL_CMGETAPI( "sysdirrename" ) 
	#define CAL_sysdirrename  sysdirrename
	#define CHK_sysdirrename  TRUE
	#define EXP_sysdirrename  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirrename", (RTS_UINTPTR)sysdirrename, 1, RTSITF_GET_SIGNATURE(0, 0xA2EF5E53), 0x03051100) 
	#define EXTREF_ITF_sysdirrename {(RTS_VOID_FCTPTR)sysdirrename, "sysdirrename", RTSITF_GET_SIGNATURE(0, 0xA2EF5E53), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdirrename
	#define EXT_sysdirrename
	#define GET_sysdirrename(fl)  CAL_CMGETAPI( "sysdirrename" ) 
	#define CAL_sysdirrename  sysdirrename
	#define CHK_sysdirrename  TRUE
	#define EXP_sysdirrename  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirrename", (RTS_UINTPTR)sysdirrename, 1, RTSITF_GET_SIGNATURE(0, 0xA2EF5E53), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdirrename
	#define EXT_SysDirsysdirrename
	#define GET_SysDirsysdirrename  ERR_OK
	#define CAL_SysDirsysdirrename  sysdirrename
	#define CHK_SysDirsysdirrename  TRUE
	#define EXP_SysDirsysdirrename  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirrename", (RTS_UINTPTR)sysdirrename, 1, RTSITF_GET_SIGNATURE(0, 0xA2EF5E53), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdirrename
	#define EXT_sysdirrename
	#define GET_sysdirrename(fl)  CAL_CMGETAPI( "sysdirrename" ) 
	#define CAL_sysdirrename  sysdirrename
	#define CHK_sysdirrename  TRUE
	#define EXP_sysdirrename  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirrename", (RTS_UINTPTR)sysdirrename, 1, RTSITF_GET_SIGNATURE(0, 0xA2EF5E53), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdirrename  PFSYSDIRRENAME_IEC pfsysdirrename;
	#define EXT_sysdirrename  extern PFSYSDIRRENAME_IEC pfsysdirrename;
	#define GET_sysdirrename(fl)  s_pfCMGetAPI2( "sysdirrename", (RTS_VOID_FCTPTR *)&pfsysdirrename, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xA2EF5E53), 0x03051100)
	#define CAL_sysdirrename  pfsysdirrename
	#define CHK_sysdirrename  (pfsysdirrename != NULL)
	#define EXP_sysdirrename   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirrename", (RTS_UINTPTR)sysdirrename, 1, RTSITF_GET_SIGNATURE(0, 0xA2EF5E53), 0x03051100) 
#endif


/**
 * Set current working directory on the target. In this directory all following file function are operated.
 * This setting applies to file access from IEC only. An empty string resets the working directory to its default.
 * This function should not be used in parallel to any other file operation from IEC. 
 * :return: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysdirsetcurrent_struct
{
	RTS_IEC_STRING *szDir;				/* VAR_INPUT */	/* Name of current directory */
	RTS_IEC_RESULT SysDirSetCurrent;	/* VAR_OUTPUT */	
} sysdirsetcurrent_struct;

void CDECL CDECL_EXT sysdirsetcurrent(sysdirsetcurrent_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSDIRSETCURRENT_IEC) (sysdirsetcurrent_struct *p);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRSETCURRENT_NOTIMPLEMENTED)
	#define USE_sysdirsetcurrent
	#define EXT_sysdirsetcurrent
	#define GET_sysdirsetcurrent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysdirsetcurrent(p0) 
	#define CHK_sysdirsetcurrent  FALSE
	#define EXP_sysdirsetcurrent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysdirsetcurrent
	#define EXT_sysdirsetcurrent
	#define GET_sysdirsetcurrent(fl)  CAL_CMGETAPI( "sysdirsetcurrent" ) 
	#define CAL_sysdirsetcurrent  sysdirsetcurrent
	#define CHK_sysdirsetcurrent  TRUE
	#define EXP_sysdirsetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirsetcurrent", (RTS_UINTPTR)sysdirsetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x92FA36CD), 0x03051100) 
	#define EXTREF_ITF_sysdirsetcurrent {(RTS_VOID_FCTPTR)sysdirsetcurrent, "sysdirsetcurrent", RTSITF_GET_SIGNATURE(0, 0x92FA36CD), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_sysdirsetcurrent
	#define EXT_sysdirsetcurrent
	#define GET_sysdirsetcurrent(fl)  CAL_CMGETAPI( "sysdirsetcurrent" ) 
	#define CAL_sysdirsetcurrent  sysdirsetcurrent
	#define CHK_sysdirsetcurrent  TRUE
	#define EXP_sysdirsetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirsetcurrent", (RTS_UINTPTR)sysdirsetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x92FA36CD), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirsysdirsetcurrent
	#define EXT_SysDirsysdirsetcurrent
	#define GET_SysDirsysdirsetcurrent  ERR_OK
	#define CAL_SysDirsysdirsetcurrent  sysdirsetcurrent
	#define CHK_SysDirsysdirsetcurrent  TRUE
	#define EXP_SysDirsysdirsetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirsetcurrent", (RTS_UINTPTR)sysdirsetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x92FA36CD), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysdirsetcurrent
	#define EXT_sysdirsetcurrent
	#define GET_sysdirsetcurrent(fl)  CAL_CMGETAPI( "sysdirsetcurrent" ) 
	#define CAL_sysdirsetcurrent  sysdirsetcurrent
	#define CHK_sysdirsetcurrent  TRUE
	#define EXP_sysdirsetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirsetcurrent", (RTS_UINTPTR)sysdirsetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x92FA36CD), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysdirsetcurrent  PFSYSDIRSETCURRENT_IEC pfsysdirsetcurrent;
	#define EXT_sysdirsetcurrent  extern PFSYSDIRSETCURRENT_IEC pfsysdirsetcurrent;
	#define GET_sysdirsetcurrent(fl)  s_pfCMGetAPI2( "sysdirsetcurrent", (RTS_VOID_FCTPTR *)&pfsysdirsetcurrent, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x92FA36CD), 0x03051100)
	#define CAL_sysdirsetcurrent  pfsysdirsetcurrent
	#define CHK_sysdirsetcurrent  (pfsysdirsetcurrent != NULL)
	#define EXP_sysdirsetcurrent   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysdirsetcurrent", (RTS_UINTPTR)sysdirsetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x92FA36CD), 0x03051100) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/* Init routines for OS specific modules */
RTS_RESULT CDECL SysDirOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysDirOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <description>Opens a specified directory and returns a handle and the first directory entry.
 * In case pszDirEntry is NULL, the first directory entry may be retrieved with SysDirRead.</description>
 * <param name="pszDir" type="IN">Name of directory.
 * NOTE: Empty string ("") is the request for the current working directory.</param>
 * <param name="pszDirEntry" type="OUT">Optional: Directory entry as string. Can be NULL.</param>
 * <param name="iMaxDirEntry" type="IN">Optional: Max number of bytes to write in pszDirEntry</param>
 * <param name="pDirInfo" type="OUT">Optional: Directory information. Can be NULL.</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Directory can be opened and a directory info can be retrieved</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_FAILED">Directory cannot be opened. An invalid handle is returned.</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_END_OF_OBJECT">Directory can be opened, but _no_ pszDirEntry can be retrieved (directory is empty or end reached)!
 *		NOTE:
 *		A valid handle is returned with this error and so the directory must be closed after usage!
 * </errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OPERATION_DENIED">Directory cannot be opened because access was denied. An invalid handle is returned.</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">One of the parameters is invalid. An invalid handle is returned.</errorcode>
 * <result>RTS_HANDLE on directory</result>
 */
RTS_HANDLE CDECL SysDirOpen(char *pszDir, char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSDIROPEN) (char *pszDir, char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo, RTS_RESULT *pResult);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIROPEN_NOTIMPLEMENTED)
	#define USE_SysDirOpen
	#define EXT_SysDirOpen
	#define GET_SysDirOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirOpen(p0,p1,p2,p3,p4)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysDirOpen  FALSE
	#define EXP_SysDirOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirOpen
	#define EXT_SysDirOpen
	#define GET_SysDirOpen(fl)  CAL_CMGETAPI( "SysDirOpen" ) 
	#define CAL_SysDirOpen  SysDirOpen
	#define CHK_SysDirOpen  TRUE
	#define EXP_SysDirOpen  CAL_CMEXPAPI( "SysDirOpen" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirOpen
	#define EXT_SysDirOpen
	#define GET_SysDirOpen(fl)  CAL_CMGETAPI( "SysDirOpen" ) 
	#define CAL_SysDirOpen  SysDirOpen
	#define CHK_SysDirOpen  TRUE
	#define EXP_SysDirOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirOpen", (RTS_UINTPTR)SysDirOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirOpen
	#define EXT_SysDirSysDirOpen
	#define GET_SysDirSysDirOpen  ERR_OK
	#define CAL_SysDirSysDirOpen  ((ISysDir*)s_pfCMCreateInstance(CLASSID_CSysDir, NULL))->ISysDirOpen
	#define CHK_SysDirSysDirOpen	(s_pfCMCreateInstance != NULL && pISysDir != NULL)
	#define EXP_SysDirSysDirOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirOpen
	#define EXT_SysDirOpen
	#define GET_SysDirOpen(fl)  CAL_CMGETAPI( "SysDirOpen" ) 
	#define CAL_SysDirOpen  ((ISysDir*)s_pfCMCreateInstance(CLASSID_CSysDir, NULL))->ISysDirOpen
	#define CHK_SysDirOpen	(s_pfCMCreateInstance != NULL && pISysDir != NULL)
	#define EXP_SysDirOpen  CAL_CMEXPAPI( "SysDirOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirOpen  PFSYSDIROPEN pfSysDirOpen;
	#define EXT_SysDirOpen  extern PFSYSDIROPEN pfSysDirOpen;
	#define GET_SysDirOpen(fl)  s_pfCMGetAPI2( "SysDirOpen", (RTS_VOID_FCTPTR *)&pfSysDirOpen, (fl), 0, 0)
	#define CAL_SysDirOpen  pfSysDirOpen
	#define CHK_SysDirOpen  (pfSysDirOpen != NULL)
	#define EXP_SysDirOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirOpen", (RTS_UINTPTR)SysDirOpen, 0, 0) 
#endif




/**
 * <description>Opens a specified directory and returns a handle and the first directory entry. No standard path will be added.
 * In case pszDirEntry is NULL, the first directory entry may be retrieved with SysDirRead.
 *	IMPLEMENTATION NOTE:
 *	This interface function is implemented operating system dependent! Optimizations can be done here.</description>
 * <param name="pszDir" type="IN">Name of directory.
 * IMPLEMENTATION NOTE: Empty string ("") is the request for the current working directory.</param>
 * <param name="pszDirEntry" type="OUT">Optional: Directory entry as string. Can be NULL.</param>
 * <param name="iMaxDirEntry" type="IN">Optional: Max number of bytes to write in pszDirEntry</param>
 * <param name="pDirInfo" type="OUT">Optional: Directory information. Can be NULL</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Directory can be opened and a directory info can be retrieved</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_FAILED">Directory cannot be opened. An invalid handle is returned.</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_END_OF_OBJECT">Directory can be opened, but _no_ pszDirEntry can be retrieved (directory is empty or end reached)!
 *		NOTE:
 *		A valid handle is returned with this error and so the directory must be closed after usage!
 * </errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_PARAMETER">One of the parameters is invalid. An invalid handle is returned.</errorcode>
 * <result>RTS_HANDLE on directory</result>
 */
RTS_HANDLE CDECL SysDirOpen_(char *pszDir, char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSDIROPEN_) (char *pszDir, char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo, RTS_RESULT *pResult);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIROPEN__NOTIMPLEMENTED)
	#define USE_SysDirOpen_
	#define EXT_SysDirOpen_
	#define GET_SysDirOpen_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirOpen_(p0,p1,p2,p3,p4)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysDirOpen_  FALSE
	#define EXP_SysDirOpen_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirOpen_
	#define EXT_SysDirOpen_
	#define GET_SysDirOpen_(fl)  CAL_CMGETAPI( "SysDirOpen_" ) 
	#define CAL_SysDirOpen_  SysDirOpen_
	#define CHK_SysDirOpen_  TRUE
	#define EXP_SysDirOpen_  CAL_CMEXPAPI( "SysDirOpen_" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirOpen_
	#define EXT_SysDirOpen_
	#define GET_SysDirOpen_(fl)  CAL_CMGETAPI( "SysDirOpen_" ) 
	#define CAL_SysDirOpen_  SysDirOpen_
	#define CHK_SysDirOpen_  TRUE
	#define EXP_SysDirOpen_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirOpen_", (RTS_UINTPTR)SysDirOpen_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirOpen_
	#define EXT_SysDirSysDirOpen_
	#define GET_SysDirSysDirOpen_  ERR_OK
	#define CAL_SysDirSysDirOpen_  ((ISysDir*)s_pfCMCreateInstance(CLASSID_CSysDir, NULL))->ISysDirOpen_
	#define CHK_SysDirSysDirOpen_	(s_pfCMCreateInstance != NULL && pISysDir != NULL)
	#define EXP_SysDirSysDirOpen_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirOpen_
	#define EXT_SysDirOpen_
	#define GET_SysDirOpen_(fl)  CAL_CMGETAPI( "SysDirOpen_" ) 
	#define CAL_SysDirOpen_  ((ISysDir*)s_pfCMCreateInstance(CLASSID_CSysDir, NULL))->ISysDirOpen_
	#define CHK_SysDirOpen_	(s_pfCMCreateInstance != NULL && pISysDir != NULL)
	#define EXP_SysDirOpen_  CAL_CMEXPAPI( "SysDirOpen_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirOpen_  PFSYSDIROPEN_ pfSysDirOpen_;
	#define EXT_SysDirOpen_  extern PFSYSDIROPEN_ pfSysDirOpen_;
	#define GET_SysDirOpen_(fl)  s_pfCMGetAPI2( "SysDirOpen_", (RTS_VOID_FCTPTR *)&pfSysDirOpen_, (fl), 0, 0)
	#define CAL_SysDirOpen_  pfSysDirOpen_
	#define CHK_SysDirOpen_  (pfSysDirOpen_ != NULL)
	#define EXP_SysDirOpen_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirOpen_", (RTS_UINTPTR)SysDirOpen_, 0, 0) 
#endif




/**
 * <description>Close an open directory</description>
 * <param name="hDir" type="IN">Handle to directory opened with SysDirOpen or with SysDirOpen_</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirClose(RTS_HANDLE hDir);
typedef RTS_RESULT (CDECL * PFSYSDIRCLOSE) (RTS_HANDLE hDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRCLOSE_NOTIMPLEMENTED)
	#define USE_SysDirClose
	#define EXT_SysDirClose
	#define GET_SysDirClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirClose  FALSE
	#define EXP_SysDirClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirClose
	#define EXT_SysDirClose
	#define GET_SysDirClose(fl)  CAL_CMGETAPI( "SysDirClose" ) 
	#define CAL_SysDirClose  SysDirClose
	#define CHK_SysDirClose  TRUE
	#define EXP_SysDirClose  CAL_CMEXPAPI( "SysDirClose" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirClose
	#define EXT_SysDirClose
	#define GET_SysDirClose(fl)  CAL_CMGETAPI( "SysDirClose" ) 
	#define CAL_SysDirClose  SysDirClose
	#define CHK_SysDirClose  TRUE
	#define EXP_SysDirClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirClose", (RTS_UINTPTR)SysDirClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirClose
	#define EXT_SysDirSysDirClose
	#define GET_SysDirSysDirClose  ERR_OK
	#define CAL_SysDirSysDirClose(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysDir*)p0)->ISysDirClose())
	#define CHK_SysDirSysDirClose  TRUE
	#define EXP_SysDirSysDirClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirClose
	#define EXT_SysDirClose
	#define GET_SysDirClose(fl)  CAL_CMGETAPI( "SysDirClose" ) 
	#define CAL_SysDirClose(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysDir*)p0)->ISysDirClose())
	#define CHK_SysDirClose  TRUE
	#define EXP_SysDirClose  CAL_CMEXPAPI( "SysDirClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirClose  PFSYSDIRCLOSE pfSysDirClose;
	#define EXT_SysDirClose  extern PFSYSDIRCLOSE pfSysDirClose;
	#define GET_SysDirClose(fl)  s_pfCMGetAPI2( "SysDirClose", (RTS_VOID_FCTPTR *)&pfSysDirClose, (fl), 0, 0)
	#define CAL_SysDirClose  pfSysDirClose
	#define CHK_SysDirClose  (pfSysDirClose != NULL)
	#define EXP_SysDirClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirClose", (RTS_UINTPTR)SysDirClose, 0, 0) 
#endif




/**
 * <description>Close an open directory.
 *	IMPLEMENTATION NOTE:
 *	This interface function is implemented operating system dependent! Optimizations can be done here.</description>
 * <param name="hDir" type="IN">Handle to directory opened with SysDirOpen</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirClose_(RTS_HANDLE hDir);
typedef RTS_RESULT (CDECL * PFSYSDIRCLOSE_) (RTS_HANDLE hDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRCLOSE__NOTIMPLEMENTED)
	#define USE_SysDirClose_
	#define EXT_SysDirClose_
	#define GET_SysDirClose_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirClose_(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirClose_  FALSE
	#define EXP_SysDirClose_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirClose_
	#define EXT_SysDirClose_
	#define GET_SysDirClose_(fl)  CAL_CMGETAPI( "SysDirClose_" ) 
	#define CAL_SysDirClose_  SysDirClose_
	#define CHK_SysDirClose_  TRUE
	#define EXP_SysDirClose_  CAL_CMEXPAPI( "SysDirClose_" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirClose_
	#define EXT_SysDirClose_
	#define GET_SysDirClose_(fl)  CAL_CMGETAPI( "SysDirClose_" ) 
	#define CAL_SysDirClose_  SysDirClose_
	#define CHK_SysDirClose_  TRUE
	#define EXP_SysDirClose_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirClose_", (RTS_UINTPTR)SysDirClose_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirClose_
	#define EXT_SysDirSysDirClose_
	#define GET_SysDirSysDirClose_  ERR_OK
	#define CAL_SysDirSysDirClose_(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysDir*)p0)->ISysDirClose_())
	#define CHK_SysDirSysDirClose_  TRUE
	#define EXP_SysDirSysDirClose_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirClose_
	#define EXT_SysDirClose_
	#define GET_SysDirClose_(fl)  CAL_CMGETAPI( "SysDirClose_" ) 
	#define CAL_SysDirClose_(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysDir*)p0)->ISysDirClose_())
	#define CHK_SysDirClose_  TRUE
	#define EXP_SysDirClose_  CAL_CMEXPAPI( "SysDirClose_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirClose_  PFSYSDIRCLOSE_ pfSysDirClose_;
	#define EXT_SysDirClose_  extern PFSYSDIRCLOSE_ pfSysDirClose_;
	#define GET_SysDirClose_(fl)  s_pfCMGetAPI2( "SysDirClose_", (RTS_VOID_FCTPTR *)&pfSysDirClose_, (fl), 0, 0)
	#define CAL_SysDirClose_  pfSysDirClose_
	#define CHK_SysDirClose_  (pfSysDirClose_ != NULL)
	#define EXP_SysDirClose_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirClose_", (RTS_UINTPTR)SysDirClose_, 0, 0) 
#endif




/**
 * <description>Read next directory entry. Writes the entry in pszDirEntry.</description>
 * <param name="hDir" type="IN">Handle to directory opened with SysDirOpen</param>
 * <param name="pszDirEntry" type="OUT">Directory entry as string</param>
 * <param name="iMaxDirEntry" type="OUT">Max number of bytes to write in pszDirEntry</param>
 * <param name="pDirInfo" type="OUT">Pointer to directory information.
 *		IMPLEMENTATION NOTE:
 *		Can be NULL (so only directory name is provided in pszDirEntry) </param>
 * <result>
 *   <ul>
 *     <li>ERR_OK: The directory string contains the next directory entry and is referenced by "pszDirEntry", "pDirInfo" references the related directory information string.</li>
 *     <li>ERR_END_OF_OBJECT: If end of directory list was reached, the "pszDirEntry" referenced buffer stays empty.</li>
 *     <li>ERR_PARAMETER: If one of the parameters is invalid, the function returns without updating the directory iterator.</li>
 *     <li>ERR_BUFFERSIZE: If iMaxDirEntry is too short to get the complete directory entry string but not null.
 *			ATTENTION: Typically if this error occurs, the "pszDirEntry" contains the string to the incomplete next entry, the iterator is updated!</li>
 *   </ul>
 * </result>
 */
RTS_RESULT CDECL SysDirRead(RTS_HANDLE hDir, char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo);
typedef RTS_RESULT (CDECL * PFSYSDIRREAD) (RTS_HANDLE hDir, char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRREAD_NOTIMPLEMENTED)
	#define USE_SysDirRead
	#define EXT_SysDirRead
	#define GET_SysDirRead(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirRead(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirRead  FALSE
	#define EXP_SysDirRead  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirRead
	#define EXT_SysDirRead
	#define GET_SysDirRead(fl)  CAL_CMGETAPI( "SysDirRead" ) 
	#define CAL_SysDirRead  SysDirRead
	#define CHK_SysDirRead  TRUE
	#define EXP_SysDirRead  CAL_CMEXPAPI( "SysDirRead" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirRead
	#define EXT_SysDirRead
	#define GET_SysDirRead(fl)  CAL_CMGETAPI( "SysDirRead" ) 
	#define CAL_SysDirRead  SysDirRead
	#define CHK_SysDirRead  TRUE
	#define EXP_SysDirRead  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirRead", (RTS_UINTPTR)SysDirRead, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirRead
	#define EXT_SysDirSysDirRead
	#define GET_SysDirSysDirRead  ERR_OK
	#define CAL_SysDirSysDirRead(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysDir->ISysDirRead(p1,p2,p3) : ((ISysDir*)p0)->ISysDirRead(p1,p2,p3))
	#define CHK_SysDirSysDirRead  (pISysDir != NULL)
	#define EXP_SysDirSysDirRead  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirRead
	#define EXT_SysDirRead
	#define GET_SysDirRead(fl)  CAL_CMGETAPI( "SysDirRead" ) 
	#define CAL_SysDirRead(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysDir->ISysDirRead(p1,p2,p3) : ((ISysDir*)p0)->ISysDirRead(p1,p2,p3))
	#define CHK_SysDirRead  (pISysDir != NULL)
	#define EXP_SysDirRead  CAL_CMEXPAPI( "SysDirRead" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirRead  PFSYSDIRREAD pfSysDirRead;
	#define EXT_SysDirRead  extern PFSYSDIRREAD pfSysDirRead;
	#define GET_SysDirRead(fl)  s_pfCMGetAPI2( "SysDirRead", (RTS_VOID_FCTPTR *)&pfSysDirRead, (fl), 0, 0)
	#define CAL_SysDirRead  pfSysDirRead
	#define CHK_SysDirRead  (pfSysDirRead != NULL)
	#define EXP_SysDirRead  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirRead", (RTS_UINTPTR)SysDirRead, 0, 0) 
#endif




/**
 * <description>Read next directory entry. Writes the entry in pszDirEntry.
 *	IMPLEMENTATION NOTE:
 *	This interface function is implemented operating system dependent! Optimizations can be done here.</description>
 * <param name="hDir" type="IN">Handle to directory opened with SysDirOpen</param>
 * <param name="pszDirEntry" type="OUT">Directory entry as string</param>
 * <param name="iMaxDirEntry" type="OUT">Max number of bytes to write in pszDirEntry</param>
 * <param name="pDirInfo" type="OUT">Pointer to directory information.
 *		IMPLEMENTATION NOTE:
 *		Can be NULL (so only directory name is provided in pszDirEntry) </param>
 * <result>
 *   <ul>
 *     <li>ERR_OK: The directory string contains the next directory entry and is referenced by "pszDirEntry", "pDirInfo" references the related directory information string.</li>
 *     <li>ERR_END_OF_OBJECT: If end of directory list was reached, the "pszDirEntry" referenced buffer stays empty.</li>
 *     <li>ERR_PARAMETER: If one of the parameters is invalid, the function returns without updating the directory iterator.</li>
 *     <li>ERR_BUFFERSIZE: If iMaxDirEntry is too short to get the complete directory entry string but not null.
 *			ATTENTION: Typically if this error occurs, the "pszDirEntry" contains the string to the incomplete next entry, the iterator is updated!</li>
 *   </ul>
 * </result>
 */
RTS_RESULT CDECL SysDirRead_(RTS_HANDLE hDir, char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo);
typedef RTS_RESULT (CDECL * PFSYSDIRREAD_) (RTS_HANDLE hDir, char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRREAD__NOTIMPLEMENTED)
	#define USE_SysDirRead_
	#define EXT_SysDirRead_
	#define GET_SysDirRead_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirRead_(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirRead_  FALSE
	#define EXP_SysDirRead_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirRead_
	#define EXT_SysDirRead_
	#define GET_SysDirRead_(fl)  CAL_CMGETAPI( "SysDirRead_" ) 
	#define CAL_SysDirRead_  SysDirRead_
	#define CHK_SysDirRead_  TRUE
	#define EXP_SysDirRead_  CAL_CMEXPAPI( "SysDirRead_" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirRead_
	#define EXT_SysDirRead_
	#define GET_SysDirRead_(fl)  CAL_CMGETAPI( "SysDirRead_" ) 
	#define CAL_SysDirRead_  SysDirRead_
	#define CHK_SysDirRead_  TRUE
	#define EXP_SysDirRead_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirRead_", (RTS_UINTPTR)SysDirRead_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirRead_
	#define EXT_SysDirSysDirRead_
	#define GET_SysDirSysDirRead_  ERR_OK
	#define CAL_SysDirSysDirRead_(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysDir->ISysDirRead_(p1,p2,p3) : ((ISysDir*)p0)->ISysDirRead_(p1,p2,p3))
	#define CHK_SysDirSysDirRead_  (pISysDir != NULL)
	#define EXP_SysDirSysDirRead_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirRead_
	#define EXT_SysDirRead_
	#define GET_SysDirRead_(fl)  CAL_CMGETAPI( "SysDirRead_" ) 
	#define CAL_SysDirRead_(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysDir->ISysDirRead_(p1,p2,p3) : ((ISysDir*)p0)->ISysDirRead_(p1,p2,p3))
	#define CHK_SysDirRead_  (pISysDir != NULL)
	#define EXP_SysDirRead_  CAL_CMEXPAPI( "SysDirRead_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirRead_  PFSYSDIRREAD_ pfSysDirRead_;
	#define EXT_SysDirRead_  extern PFSYSDIRREAD_ pfSysDirRead_;
	#define GET_SysDirRead_(fl)  s_pfCMGetAPI2( "SysDirRead_", (RTS_VOID_FCTPTR *)&pfSysDirRead_, (fl), 0, 0)
	#define CAL_SysDirRead_  pfSysDirRead_
	#define CHK_SysDirRead_  (pfSysDirRead_ != NULL)
	#define EXP_SysDirRead_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirRead_", (RTS_UINTPTR)SysDirRead_, 0, 0) 
#endif




/**
 * <description>Creates a new directory with the specified name</description>
 * <param name="pszDir" type="IN">Name of directory</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirCreate(char *pszDir);
typedef RTS_RESULT (CDECL * PFSYSDIRCREATE) (char *pszDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRCREATE_NOTIMPLEMENTED)
	#define USE_SysDirCreate
	#define EXT_SysDirCreate
	#define GET_SysDirCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirCreate(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirCreate  FALSE
	#define EXP_SysDirCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirCreate
	#define EXT_SysDirCreate
	#define GET_SysDirCreate(fl)  CAL_CMGETAPI( "SysDirCreate" ) 
	#define CAL_SysDirCreate  SysDirCreate
	#define CHK_SysDirCreate  TRUE
	#define EXP_SysDirCreate  CAL_CMEXPAPI( "SysDirCreate" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirCreate
	#define EXT_SysDirCreate
	#define GET_SysDirCreate(fl)  CAL_CMGETAPI( "SysDirCreate" ) 
	#define CAL_SysDirCreate  SysDirCreate
	#define CHK_SysDirCreate  TRUE
	#define EXP_SysDirCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirCreate", (RTS_UINTPTR)SysDirCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirCreate
	#define EXT_SysDirSysDirCreate
	#define GET_SysDirSysDirCreate  ERR_OK
	#define CAL_SysDirSysDirCreate pISysDir->ISysDirCreate
	#define CHK_SysDirSysDirCreate (pISysDir != NULL)
	#define EXP_SysDirSysDirCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirCreate
	#define EXT_SysDirCreate
	#define GET_SysDirCreate(fl)  CAL_CMGETAPI( "SysDirCreate" ) 
	#define CAL_SysDirCreate pISysDir->ISysDirCreate
	#define CHK_SysDirCreate (pISysDir != NULL)
	#define EXP_SysDirCreate  CAL_CMEXPAPI( "SysDirCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirCreate  PFSYSDIRCREATE pfSysDirCreate;
	#define EXT_SysDirCreate  extern PFSYSDIRCREATE pfSysDirCreate;
	#define GET_SysDirCreate(fl)  s_pfCMGetAPI2( "SysDirCreate", (RTS_VOID_FCTPTR *)&pfSysDirCreate, (fl), 0, 0)
	#define CAL_SysDirCreate  pfSysDirCreate
	#define CHK_SysDirCreate  (pfSysDirCreate != NULL)
	#define EXP_SysDirCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirCreate", (RTS_UINTPTR)SysDirCreate, 0, 0) 
#endif




/**
 * <description>Creates a new directory with the specified name.
 * If bRecursive is set, a full path is created, i.e. all missing parent directories are created too.</description>
 * <param name="pszDir" type="IN">Name of directory</param>
 * <param name="bRecursive" type="IN">Create a full path</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirCreate2(char *pszDir, RTS_BOOL bRecursive);
typedef RTS_RESULT (CDECL * PFSYSDIRCREATE2) (char *pszDir, RTS_BOOL bRecursive);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRCREATE2_NOTIMPLEMENTED)
	#define USE_SysDirCreate2
	#define EXT_SysDirCreate2
	#define GET_SysDirCreate2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirCreate2(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirCreate2  FALSE
	#define EXP_SysDirCreate2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirCreate2
	#define EXT_SysDirCreate2
	#define GET_SysDirCreate2(fl)  CAL_CMGETAPI( "SysDirCreate2" ) 
	#define CAL_SysDirCreate2  SysDirCreate2
	#define CHK_SysDirCreate2  TRUE
	#define EXP_SysDirCreate2  CAL_CMEXPAPI( "SysDirCreate2" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirCreate2
	#define EXT_SysDirCreate2
	#define GET_SysDirCreate2(fl)  CAL_CMGETAPI( "SysDirCreate2" ) 
	#define CAL_SysDirCreate2  SysDirCreate2
	#define CHK_SysDirCreate2  TRUE
	#define EXP_SysDirCreate2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirCreate2", (RTS_UINTPTR)SysDirCreate2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirCreate2
	#define EXT_SysDirSysDirCreate2
	#define GET_SysDirSysDirCreate2  ERR_OK
	#define CAL_SysDirSysDirCreate2 pISysDir->ISysDirCreate2
	#define CHK_SysDirSysDirCreate2 (pISysDir != NULL)
	#define EXP_SysDirSysDirCreate2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirCreate2
	#define EXT_SysDirCreate2
	#define GET_SysDirCreate2(fl)  CAL_CMGETAPI( "SysDirCreate2" ) 
	#define CAL_SysDirCreate2 pISysDir->ISysDirCreate2
	#define CHK_SysDirCreate2 (pISysDir != NULL)
	#define EXP_SysDirCreate2  CAL_CMEXPAPI( "SysDirCreate2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirCreate2  PFSYSDIRCREATE2 pfSysDirCreate2;
	#define EXT_SysDirCreate2  extern PFSYSDIRCREATE2 pfSysDirCreate2;
	#define GET_SysDirCreate2(fl)  s_pfCMGetAPI2( "SysDirCreate2", (RTS_VOID_FCTPTR *)&pfSysDirCreate2, (fl), 0, 0)
	#define CAL_SysDirCreate2  pfSysDirCreate2
	#define CHK_SysDirCreate2  (pfSysDirCreate2 != NULL)
	#define EXP_SysDirCreate2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirCreate2", (RTS_UINTPTR)SysDirCreate2, 0, 0) 
#endif




/**
 * <description>Creates a new directory with the specified name. No standard path will be added.
 *	IMPLEMENTATION NOTE:
 *	This interface function is implemented operating system dependent! Optimizations can be done here.</description>
 * <param name="pszDir" type="IN">Name of directory</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">Directory was created successfully</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_FAILED">Directory couldn't be created for various reasons other than the following two</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NO_OBJECT">The path to the directory to be created does not exist</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_DUPLICATE">The directory already exists</errorcode>
 */
RTS_RESULT CDECL SysDirCreate_(char *pszDir);
typedef RTS_RESULT (CDECL * PFSYSDIRCREATE_) (char *pszDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRCREATE__NOTIMPLEMENTED)
	#define USE_SysDirCreate_
	#define EXT_SysDirCreate_
	#define GET_SysDirCreate_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirCreate_(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirCreate_  FALSE
	#define EXP_SysDirCreate_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirCreate_
	#define EXT_SysDirCreate_
	#define GET_SysDirCreate_(fl)  CAL_CMGETAPI( "SysDirCreate_" ) 
	#define CAL_SysDirCreate_  SysDirCreate_
	#define CHK_SysDirCreate_  TRUE
	#define EXP_SysDirCreate_  CAL_CMEXPAPI( "SysDirCreate_" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirCreate_
	#define EXT_SysDirCreate_
	#define GET_SysDirCreate_(fl)  CAL_CMGETAPI( "SysDirCreate_" ) 
	#define CAL_SysDirCreate_  SysDirCreate_
	#define CHK_SysDirCreate_  TRUE
	#define EXP_SysDirCreate_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirCreate_", (RTS_UINTPTR)SysDirCreate_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirCreate_
	#define EXT_SysDirSysDirCreate_
	#define GET_SysDirSysDirCreate_  ERR_OK
	#define CAL_SysDirSysDirCreate_ pISysDir->ISysDirCreate_
	#define CHK_SysDirSysDirCreate_ (pISysDir != NULL)
	#define EXP_SysDirSysDirCreate_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirCreate_
	#define EXT_SysDirCreate_
	#define GET_SysDirCreate_(fl)  CAL_CMGETAPI( "SysDirCreate_" ) 
	#define CAL_SysDirCreate_ pISysDir->ISysDirCreate_
	#define CHK_SysDirCreate_ (pISysDir != NULL)
	#define EXP_SysDirCreate_  CAL_CMEXPAPI( "SysDirCreate_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirCreate_  PFSYSDIRCREATE_ pfSysDirCreate_;
	#define EXT_SysDirCreate_  extern PFSYSDIRCREATE_ pfSysDirCreate_;
	#define GET_SysDirCreate_(fl)  s_pfCMGetAPI2( "SysDirCreate_", (RTS_VOID_FCTPTR *)&pfSysDirCreate_, (fl), 0, 0)
	#define CAL_SysDirCreate_  pfSysDirCreate_
	#define CHK_SysDirCreate_  (pfSysDirCreate_ != NULL)
	#define EXP_SysDirCreate_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirCreate_", (RTS_UINTPTR)SysDirCreate_, 0, 0) 
#endif




/**
 * <description>Deletes a directory with the specified name</description>
 * <param name="pszDir" type="IN">Name of directory</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirDelete(char *pszDir);
typedef RTS_RESULT (CDECL * PFSYSDIRDELETE) (char *pszDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRDELETE_NOTIMPLEMENTED)
	#define USE_SysDirDelete
	#define EXT_SysDirDelete
	#define GET_SysDirDelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirDelete(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirDelete  FALSE
	#define EXP_SysDirDelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirDelete
	#define EXT_SysDirDelete
	#define GET_SysDirDelete(fl)  CAL_CMGETAPI( "SysDirDelete" ) 
	#define CAL_SysDirDelete  SysDirDelete
	#define CHK_SysDirDelete  TRUE
	#define EXP_SysDirDelete  CAL_CMEXPAPI( "SysDirDelete" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirDelete
	#define EXT_SysDirDelete
	#define GET_SysDirDelete(fl)  CAL_CMGETAPI( "SysDirDelete" ) 
	#define CAL_SysDirDelete  SysDirDelete
	#define CHK_SysDirDelete  TRUE
	#define EXP_SysDirDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirDelete", (RTS_UINTPTR)SysDirDelete, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirDelete
	#define EXT_SysDirSysDirDelete
	#define GET_SysDirSysDirDelete  ERR_OK
	#define CAL_SysDirSysDirDelete pISysDir->ISysDirDelete
	#define CHK_SysDirSysDirDelete (pISysDir != NULL)
	#define EXP_SysDirSysDirDelete  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirDelete
	#define EXT_SysDirDelete
	#define GET_SysDirDelete(fl)  CAL_CMGETAPI( "SysDirDelete" ) 
	#define CAL_SysDirDelete pISysDir->ISysDirDelete
	#define CHK_SysDirDelete (pISysDir != NULL)
	#define EXP_SysDirDelete  CAL_CMEXPAPI( "SysDirDelete" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirDelete  PFSYSDIRDELETE pfSysDirDelete;
	#define EXT_SysDirDelete  extern PFSYSDIRDELETE pfSysDirDelete;
	#define GET_SysDirDelete(fl)  s_pfCMGetAPI2( "SysDirDelete", (RTS_VOID_FCTPTR *)&pfSysDirDelete, (fl), 0, 0)
	#define CAL_SysDirDelete  pfSysDirDelete
	#define CHK_SysDirDelete  (pfSysDirDelete != NULL)
	#define EXP_SysDirDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirDelete", (RTS_UINTPTR)SysDirDelete, 0, 0) 
#endif




/**
 * <description>Deletes a directory with the specified name
 * If bRecursive is set, a whole folder tree can be deleted, i.e. all sub-folders and files are deleted.
 * If bRecursive and bKeepDir are set, the folder is kept but its contents is deleted.</description>
 * <param name="pszDir" type="IN">Name of directory</param>
 * <param name="bRecursive" type="IN">Delete a whole folder tree</param>
 * <param name="bKeepDir" type="IN">Keep the starting folder</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirDelete2(char *pszDir, RTS_BOOL bRecursive, RTS_BOOL bKeepDir);
typedef RTS_RESULT (CDECL * PFSYSDIRDELETE2) (char *pszDir, RTS_BOOL bRecursive, RTS_BOOL bKeepDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRDELETE2_NOTIMPLEMENTED)
	#define USE_SysDirDelete2
	#define EXT_SysDirDelete2
	#define GET_SysDirDelete2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirDelete2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirDelete2  FALSE
	#define EXP_SysDirDelete2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirDelete2
	#define EXT_SysDirDelete2
	#define GET_SysDirDelete2(fl)  CAL_CMGETAPI( "SysDirDelete2" ) 
	#define CAL_SysDirDelete2  SysDirDelete2
	#define CHK_SysDirDelete2  TRUE
	#define EXP_SysDirDelete2  CAL_CMEXPAPI( "SysDirDelete2" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirDelete2
	#define EXT_SysDirDelete2
	#define GET_SysDirDelete2(fl)  CAL_CMGETAPI( "SysDirDelete2" ) 
	#define CAL_SysDirDelete2  SysDirDelete2
	#define CHK_SysDirDelete2  TRUE
	#define EXP_SysDirDelete2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirDelete2", (RTS_UINTPTR)SysDirDelete2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirDelete2
	#define EXT_SysDirSysDirDelete2
	#define GET_SysDirSysDirDelete2  ERR_OK
	#define CAL_SysDirSysDirDelete2 pISysDir->ISysDirDelete2
	#define CHK_SysDirSysDirDelete2 (pISysDir != NULL)
	#define EXP_SysDirSysDirDelete2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirDelete2
	#define EXT_SysDirDelete2
	#define GET_SysDirDelete2(fl)  CAL_CMGETAPI( "SysDirDelete2" ) 
	#define CAL_SysDirDelete2 pISysDir->ISysDirDelete2
	#define CHK_SysDirDelete2 (pISysDir != NULL)
	#define EXP_SysDirDelete2  CAL_CMEXPAPI( "SysDirDelete2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirDelete2  PFSYSDIRDELETE2 pfSysDirDelete2;
	#define EXT_SysDirDelete2  extern PFSYSDIRDELETE2 pfSysDirDelete2;
	#define GET_SysDirDelete2(fl)  s_pfCMGetAPI2( "SysDirDelete2", (RTS_VOID_FCTPTR *)&pfSysDirDelete2, (fl), 0, 0)
	#define CAL_SysDirDelete2  pfSysDirDelete2
	#define CHK_SysDirDelete2  (pfSysDirDelete2 != NULL)
	#define EXP_SysDirDelete2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirDelete2", (RTS_UINTPTR)SysDirDelete2, 0, 0) 
#endif




/**
 * <description>Deletes a directory with the specified name. No standard path will be added.
 *	IMPLEMENTATION NOTE:
 *	This interface function is implemented operating system dependent! Optimizations can be done here.</description>
 * <param name="pszDir" type="IN">Name of directory</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirDelete_(char *pszDir);
typedef RTS_RESULT (CDECL * PFSYSDIRDELETE_) (char *pszDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRDELETE__NOTIMPLEMENTED)
	#define USE_SysDirDelete_
	#define EXT_SysDirDelete_
	#define GET_SysDirDelete_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirDelete_(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirDelete_  FALSE
	#define EXP_SysDirDelete_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirDelete_
	#define EXT_SysDirDelete_
	#define GET_SysDirDelete_(fl)  CAL_CMGETAPI( "SysDirDelete_" ) 
	#define CAL_SysDirDelete_  SysDirDelete_
	#define CHK_SysDirDelete_  TRUE
	#define EXP_SysDirDelete_  CAL_CMEXPAPI( "SysDirDelete_" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirDelete_
	#define EXT_SysDirDelete_
	#define GET_SysDirDelete_(fl)  CAL_CMGETAPI( "SysDirDelete_" ) 
	#define CAL_SysDirDelete_  SysDirDelete_
	#define CHK_SysDirDelete_  TRUE
	#define EXP_SysDirDelete_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirDelete_", (RTS_UINTPTR)SysDirDelete_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirDelete_
	#define EXT_SysDirSysDirDelete_
	#define GET_SysDirSysDirDelete_  ERR_OK
	#define CAL_SysDirSysDirDelete_ pISysDir->ISysDirDelete_
	#define CHK_SysDirSysDirDelete_ (pISysDir != NULL)
	#define EXP_SysDirSysDirDelete_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirDelete_
	#define EXT_SysDirDelete_
	#define GET_SysDirDelete_(fl)  CAL_CMGETAPI( "SysDirDelete_" ) 
	#define CAL_SysDirDelete_ pISysDir->ISysDirDelete_
	#define CHK_SysDirDelete_ (pISysDir != NULL)
	#define EXP_SysDirDelete_  CAL_CMEXPAPI( "SysDirDelete_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirDelete_  PFSYSDIRDELETE_ pfSysDirDelete_;
	#define EXT_SysDirDelete_  extern PFSYSDIRDELETE_ pfSysDirDelete_;
	#define GET_SysDirDelete_(fl)  s_pfCMGetAPI2( "SysDirDelete_", (RTS_VOID_FCTPTR *)&pfSysDirDelete_, (fl), 0, 0)
	#define CAL_SysDirDelete_  pfSysDirDelete_
	#define CHK_SysDirDelete_  (pfSysDirDelete_ != NULL)
	#define EXP_SysDirDelete_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirDelete_", (RTS_UINTPTR)SysDirDelete_, 0, 0) 
#endif




/**
 * <description>Rename directory</description>
 * <param name="pszOldDir" type="IN">Name of existing directory</param>
 * <param name="pszNewDir" type="IN">New name</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirRename(char *pszOldDir, char *pszNewDir);
typedef RTS_RESULT (CDECL * PFSYSDIRRENAME) (char *pszOldDir, char *pszNewDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRRENAME_NOTIMPLEMENTED)
	#define USE_SysDirRename
	#define EXT_SysDirRename
	#define GET_SysDirRename(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirRename(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirRename  FALSE
	#define EXP_SysDirRename  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirRename
	#define EXT_SysDirRename
	#define GET_SysDirRename(fl)  CAL_CMGETAPI( "SysDirRename" ) 
	#define CAL_SysDirRename  SysDirRename
	#define CHK_SysDirRename  TRUE
	#define EXP_SysDirRename  CAL_CMEXPAPI( "SysDirRename" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirRename
	#define EXT_SysDirRename
	#define GET_SysDirRename(fl)  CAL_CMGETAPI( "SysDirRename" ) 
	#define CAL_SysDirRename  SysDirRename
	#define CHK_SysDirRename  TRUE
	#define EXP_SysDirRename  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirRename", (RTS_UINTPTR)SysDirRename, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirRename
	#define EXT_SysDirSysDirRename
	#define GET_SysDirSysDirRename  ERR_OK
	#define CAL_SysDirSysDirRename pISysDir->ISysDirRename
	#define CHK_SysDirSysDirRename (pISysDir != NULL)
	#define EXP_SysDirSysDirRename  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirRename
	#define EXT_SysDirRename
	#define GET_SysDirRename(fl)  CAL_CMGETAPI( "SysDirRename" ) 
	#define CAL_SysDirRename pISysDir->ISysDirRename
	#define CHK_SysDirRename (pISysDir != NULL)
	#define EXP_SysDirRename  CAL_CMEXPAPI( "SysDirRename" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirRename  PFSYSDIRRENAME pfSysDirRename;
	#define EXT_SysDirRename  extern PFSYSDIRRENAME pfSysDirRename;
	#define GET_SysDirRename(fl)  s_pfCMGetAPI2( "SysDirRename", (RTS_VOID_FCTPTR *)&pfSysDirRename, (fl), 0, 0)
	#define CAL_SysDirRename  pfSysDirRename
	#define CHK_SysDirRename  (pfSysDirRename != NULL)
	#define EXP_SysDirRename  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirRename", (RTS_UINTPTR)SysDirRename, 0, 0) 
#endif




/**
 * <description>Rename directory. No standard path will be added.
 *	IMPLEMENTATION NOTE:
 *	This interface function is implemented operating system dependent! Optimizations can be done here.</description>
 * <param name="pszOldDir" type="IN">Name of existing directory</param>
 * <param name="pszNewDir" type="IN">New name</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirRename_(char *pszOldDir, char *pszNewDir);
typedef RTS_RESULT (CDECL * PFSYSDIRRENAME_) (char *pszOldDir, char *pszNewDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRRENAME__NOTIMPLEMENTED)
	#define USE_SysDirRename_
	#define EXT_SysDirRename_
	#define GET_SysDirRename_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirRename_(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirRename_  FALSE
	#define EXP_SysDirRename_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirRename_
	#define EXT_SysDirRename_
	#define GET_SysDirRename_(fl)  CAL_CMGETAPI( "SysDirRename_" ) 
	#define CAL_SysDirRename_  SysDirRename_
	#define CHK_SysDirRename_  TRUE
	#define EXP_SysDirRename_  CAL_CMEXPAPI( "SysDirRename_" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirRename_
	#define EXT_SysDirRename_
	#define GET_SysDirRename_(fl)  CAL_CMGETAPI( "SysDirRename_" ) 
	#define CAL_SysDirRename_  SysDirRename_
	#define CHK_SysDirRename_  TRUE
	#define EXP_SysDirRename_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirRename_", (RTS_UINTPTR)SysDirRename_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirRename_
	#define EXT_SysDirSysDirRename_
	#define GET_SysDirSysDirRename_  ERR_OK
	#define CAL_SysDirSysDirRename_ pISysDir->ISysDirRename_
	#define CHK_SysDirSysDirRename_ (pISysDir != NULL)
	#define EXP_SysDirSysDirRename_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirRename_
	#define EXT_SysDirRename_
	#define GET_SysDirRename_(fl)  CAL_CMGETAPI( "SysDirRename_" ) 
	#define CAL_SysDirRename_ pISysDir->ISysDirRename_
	#define CHK_SysDirRename_ (pISysDir != NULL)
	#define EXP_SysDirRename_  CAL_CMEXPAPI( "SysDirRename_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirRename_  PFSYSDIRRENAME_ pfSysDirRename_;
	#define EXT_SysDirRename_  extern PFSYSDIRRENAME_ pfSysDirRename_;
	#define GET_SysDirRename_(fl)  s_pfCMGetAPI2( "SysDirRename_", (RTS_VOID_FCTPTR *)&pfSysDirRename_, (fl), 0, 0)
	#define CAL_SysDirRename_  pfSysDirRename_
	#define CHK_SysDirRename_  (pfSysDirRename_ != NULL)
	#define EXP_SysDirRename_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirRename_", (RTS_UINTPTR)SysDirRename_, 0, 0) 
#endif




/**
 * <description>Get current working directory. If no FilePath is specified pzDir is empty (default).
 * To obtain the working directory of the underlying file system use SysDirGetCurrent_.
 * </description>
 * <param name="pszDir" type="OUT">Name of current directory</param>
 * <param name="iMaxDirLen" type="IN">Max length of directory buffer</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirGetCurrent(char *pszDir, int iMaxDirLen);
typedef RTS_RESULT (CDECL * PFSYSDIRGETCURRENT) (char *pszDir, int iMaxDirLen);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRGETCURRENT_NOTIMPLEMENTED)
	#define USE_SysDirGetCurrent
	#define EXT_SysDirGetCurrent
	#define GET_SysDirGetCurrent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirGetCurrent(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirGetCurrent  FALSE
	#define EXP_SysDirGetCurrent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirGetCurrent
	#define EXT_SysDirGetCurrent
	#define GET_SysDirGetCurrent(fl)  CAL_CMGETAPI( "SysDirGetCurrent" ) 
	#define CAL_SysDirGetCurrent  SysDirGetCurrent
	#define CHK_SysDirGetCurrent  TRUE
	#define EXP_SysDirGetCurrent  CAL_CMEXPAPI( "SysDirGetCurrent" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirGetCurrent
	#define EXT_SysDirGetCurrent
	#define GET_SysDirGetCurrent(fl)  CAL_CMGETAPI( "SysDirGetCurrent" ) 
	#define CAL_SysDirGetCurrent  SysDirGetCurrent
	#define CHK_SysDirGetCurrent  TRUE
	#define EXP_SysDirGetCurrent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirGetCurrent", (RTS_UINTPTR)SysDirGetCurrent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirGetCurrent
	#define EXT_SysDirSysDirGetCurrent
	#define GET_SysDirSysDirGetCurrent  ERR_OK
	#define CAL_SysDirSysDirGetCurrent pISysDir->ISysDirGetCurrent
	#define CHK_SysDirSysDirGetCurrent (pISysDir != NULL)
	#define EXP_SysDirSysDirGetCurrent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirGetCurrent
	#define EXT_SysDirGetCurrent
	#define GET_SysDirGetCurrent(fl)  CAL_CMGETAPI( "SysDirGetCurrent" ) 
	#define CAL_SysDirGetCurrent pISysDir->ISysDirGetCurrent
	#define CHK_SysDirGetCurrent (pISysDir != NULL)
	#define EXP_SysDirGetCurrent  CAL_CMEXPAPI( "SysDirGetCurrent" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirGetCurrent  PFSYSDIRGETCURRENT pfSysDirGetCurrent;
	#define EXT_SysDirGetCurrent  extern PFSYSDIRGETCURRENT pfSysDirGetCurrent;
	#define GET_SysDirGetCurrent(fl)  s_pfCMGetAPI2( "SysDirGetCurrent", (RTS_VOID_FCTPTR *)&pfSysDirGetCurrent, (fl), 0, 0)
	#define CAL_SysDirGetCurrent  pfSysDirGetCurrent
	#define CHK_SysDirGetCurrent  (pfSysDirGetCurrent != NULL)
	#define EXP_SysDirGetCurrent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirGetCurrent", (RTS_UINTPTR)SysDirGetCurrent, 0, 0) 
#endif




/**
 * <description>Get current working directory of the underlying file system. No standard path will be added.
 *	IMPLEMENTATION NOTE:
 *	This interface function is implemented operating system dependent!</description>
 * <param name="pszDir" type="OUT">Name of current directory</param>
 * <param name="iMaxDirLen" type="IN">Max length of directory buffer</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirGetCurrent_(char *pszDir, int iMaxDirLen);
typedef RTS_RESULT (CDECL * PFSYSDIRGETCURRENT_) (char *pszDir, int iMaxDirLen);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRGETCURRENT__NOTIMPLEMENTED)
	#define USE_SysDirGetCurrent_
	#define EXT_SysDirGetCurrent_
	#define GET_SysDirGetCurrent_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirGetCurrent_(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirGetCurrent_  FALSE
	#define EXP_SysDirGetCurrent_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirGetCurrent_
	#define EXT_SysDirGetCurrent_
	#define GET_SysDirGetCurrent_(fl)  CAL_CMGETAPI( "SysDirGetCurrent_" ) 
	#define CAL_SysDirGetCurrent_  SysDirGetCurrent_
	#define CHK_SysDirGetCurrent_  TRUE
	#define EXP_SysDirGetCurrent_  CAL_CMEXPAPI( "SysDirGetCurrent_" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirGetCurrent_
	#define EXT_SysDirGetCurrent_
	#define GET_SysDirGetCurrent_(fl)  CAL_CMGETAPI( "SysDirGetCurrent_" ) 
	#define CAL_SysDirGetCurrent_  SysDirGetCurrent_
	#define CHK_SysDirGetCurrent_  TRUE
	#define EXP_SysDirGetCurrent_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirGetCurrent_", (RTS_UINTPTR)SysDirGetCurrent_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirGetCurrent_
	#define EXT_SysDirSysDirGetCurrent_
	#define GET_SysDirSysDirGetCurrent_  ERR_OK
	#define CAL_SysDirSysDirGetCurrent_ pISysDir->ISysDirGetCurrent_
	#define CHK_SysDirSysDirGetCurrent_ (pISysDir != NULL)
	#define EXP_SysDirSysDirGetCurrent_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirGetCurrent_
	#define EXT_SysDirGetCurrent_
	#define GET_SysDirGetCurrent_(fl)  CAL_CMGETAPI( "SysDirGetCurrent_" ) 
	#define CAL_SysDirGetCurrent_ pISysDir->ISysDirGetCurrent_
	#define CHK_SysDirGetCurrent_ (pISysDir != NULL)
	#define EXP_SysDirGetCurrent_  CAL_CMEXPAPI( "SysDirGetCurrent_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirGetCurrent_  PFSYSDIRGETCURRENT_ pfSysDirGetCurrent_;
	#define EXT_SysDirGetCurrent_  extern PFSYSDIRGETCURRENT_ pfSysDirGetCurrent_;
	#define GET_SysDirGetCurrent_(fl)  s_pfCMGetAPI2( "SysDirGetCurrent_", (RTS_VOID_FCTPTR *)&pfSysDirGetCurrent_, (fl), 0, 0)
	#define CAL_SysDirGetCurrent_  pfSysDirGetCurrent_
	#define CHK_SysDirGetCurrent_  (pfSysDirGetCurrent_ != NULL)
	#define EXP_SysDirGetCurrent_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirGetCurrent_", (RTS_UINTPTR)SysDirGetCurrent_, 0, 0) 
#endif




/**
 * <description> Former set current working directory.
 * This function has been deprecated. It returns just ERR_NOTIMPLEMENTED.
 * To really change the working directory of the whole runtime system use SysDirSetCurrent_.
 * </description>
 * <param name="pszDir" type="IN">not used</param>
 * <result>ERR_NOTIMPLEMENTED</result>
 */
RTS_RESULT CDECL SysDirSetCurrent(char *pszDir);
typedef RTS_RESULT (CDECL * PFSYSDIRSETCURRENT) (char *pszDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRSETCURRENT_NOTIMPLEMENTED)
	#define USE_SysDirSetCurrent
	#define EXT_SysDirSetCurrent
	#define GET_SysDirSetCurrent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirSetCurrent(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirSetCurrent  FALSE
	#define EXP_SysDirSetCurrent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirSetCurrent
	#define EXT_SysDirSetCurrent
	#define GET_SysDirSetCurrent(fl)  CAL_CMGETAPI( "SysDirSetCurrent" ) 
	#define CAL_SysDirSetCurrent  SysDirSetCurrent
	#define CHK_SysDirSetCurrent  TRUE
	#define EXP_SysDirSetCurrent  CAL_CMEXPAPI( "SysDirSetCurrent" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirSetCurrent
	#define EXT_SysDirSetCurrent
	#define GET_SysDirSetCurrent(fl)  CAL_CMGETAPI( "SysDirSetCurrent" ) 
	#define CAL_SysDirSetCurrent  SysDirSetCurrent
	#define CHK_SysDirSetCurrent  TRUE
	#define EXP_SysDirSetCurrent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirSetCurrent", (RTS_UINTPTR)SysDirSetCurrent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirSetCurrent
	#define EXT_SysDirSysDirSetCurrent
	#define GET_SysDirSysDirSetCurrent  ERR_OK
	#define CAL_SysDirSysDirSetCurrent pISysDir->ISysDirSetCurrent
	#define CHK_SysDirSysDirSetCurrent (pISysDir != NULL)
	#define EXP_SysDirSysDirSetCurrent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirSetCurrent
	#define EXT_SysDirSetCurrent
	#define GET_SysDirSetCurrent(fl)  CAL_CMGETAPI( "SysDirSetCurrent" ) 
	#define CAL_SysDirSetCurrent pISysDir->ISysDirSetCurrent
	#define CHK_SysDirSetCurrent (pISysDir != NULL)
	#define EXP_SysDirSetCurrent  CAL_CMEXPAPI( "SysDirSetCurrent" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirSetCurrent  PFSYSDIRSETCURRENT pfSysDirSetCurrent;
	#define EXT_SysDirSetCurrent  extern PFSYSDIRSETCURRENT pfSysDirSetCurrent;
	#define GET_SysDirSetCurrent(fl)  s_pfCMGetAPI2( "SysDirSetCurrent", (RTS_VOID_FCTPTR *)&pfSysDirSetCurrent, (fl), 0, 0)
	#define CAL_SysDirSetCurrent  pfSysDirSetCurrent
	#define CHK_SysDirSetCurrent  (pfSysDirSetCurrent != NULL)
	#define EXP_SysDirSetCurrent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirSetCurrent", (RTS_UINTPTR)SysDirSetCurrent, 0, 0) 
#endif




/**
 * <description> Set current working directory of the underlying file system of the whole runtime system.
 * No standard path will be added. It is not recommended to use this function!
 * ATTENTION: Just in case it is strongly recommended to save the path with SysDirGetCurrent_ prior to this
 * call and restore the path as soon as possible. Otherwise the whole runtime system may not work any more!
 * ATTENTION: Reset Origin does not affect this setting. The runtime system has to be restarted!
 *	IMPLEMENTATION NOTE:
 *	This interface function is implemented operating system dependent!</description>
 * <param name="pszDir" type="IN">Name of current directory</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysDirSetCurrent_(char *pszDir);
typedef RTS_RESULT (CDECL * PFSYSDIRSETCURRENT_) (char *pszDir);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRSETCURRENT__NOTIMPLEMENTED)
	#define USE_SysDirSetCurrent_
	#define EXT_SysDirSetCurrent_
	#define GET_SysDirSetCurrent_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirSetCurrent_(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirSetCurrent_  FALSE
	#define EXP_SysDirSetCurrent_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirSetCurrent_
	#define EXT_SysDirSetCurrent_
	#define GET_SysDirSetCurrent_(fl)  CAL_CMGETAPI( "SysDirSetCurrent_" ) 
	#define CAL_SysDirSetCurrent_  SysDirSetCurrent_
	#define CHK_SysDirSetCurrent_  TRUE
	#define EXP_SysDirSetCurrent_  CAL_CMEXPAPI( "SysDirSetCurrent_" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirSetCurrent_
	#define EXT_SysDirSetCurrent_
	#define GET_SysDirSetCurrent_(fl)  CAL_CMGETAPI( "SysDirSetCurrent_" ) 
	#define CAL_SysDirSetCurrent_  SysDirSetCurrent_
	#define CHK_SysDirSetCurrent_  TRUE
	#define EXP_SysDirSetCurrent_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirSetCurrent_", (RTS_UINTPTR)SysDirSetCurrent_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirSetCurrent_
	#define EXT_SysDirSysDirSetCurrent_
	#define GET_SysDirSysDirSetCurrent_  ERR_OK
	#define CAL_SysDirSysDirSetCurrent_ pISysDir->ISysDirSetCurrent_
	#define CHK_SysDirSysDirSetCurrent_ (pISysDir != NULL)
	#define EXP_SysDirSysDirSetCurrent_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirSetCurrent_
	#define EXT_SysDirSetCurrent_
	#define GET_SysDirSetCurrent_(fl)  CAL_CMGETAPI( "SysDirSetCurrent_" ) 
	#define CAL_SysDirSetCurrent_ pISysDir->ISysDirSetCurrent_
	#define CHK_SysDirSetCurrent_ (pISysDir != NULL)
	#define EXP_SysDirSetCurrent_  CAL_CMEXPAPI( "SysDirSetCurrent_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirSetCurrent_  PFSYSDIRSETCURRENT_ pfSysDirSetCurrent_;
	#define EXT_SysDirSetCurrent_  extern PFSYSDIRSETCURRENT_ pfSysDirSetCurrent_;
	#define GET_SysDirSetCurrent_(fl)  s_pfCMGetAPI2( "SysDirSetCurrent_", (RTS_VOID_FCTPTR *)&pfSysDirSetCurrent_, (fl), 0, 0)
	#define CAL_SysDirSetCurrent_  pfSysDirSetCurrent_
	#define CHK_SysDirSetCurrent_  (pfSysDirSetCurrent_ != NULL)
	#define EXP_SysDirSetCurrent_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirSetCurrent_", (RTS_UINTPTR)SysDirSetCurrent_, 0, 0) 
#endif




/**
 * <description> Copies the contents of pszSourceDir to pszDestDir.
 * Depending of the parameters, all subdirectories and its content will be copied too, and existing files will be overwritten.
 * Non-existing destination path will be created.
 * NOTE: Empty directories will be copied if bRecursive is TRUE.
 * </description>
 * <param name="pszDestDir" type="IN">Name of destination directory</param>
 * <param name="pszSourceDir" type="IN">Name of source directory</param>
 * <param name="bRecursive" type="IN">TRUE: all subdirectories and their contents are copied, FALSE: subdirectories are omitted</param>
 * <param name="bOverwrite" type="IN">TRUE: existing files are overwritten, FALSE: existing files are left untouched</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OK">The contents of the source directory was successfully copied to the destination directory</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_NO_OBJECT">There were no files to copy, the destination directory is nevertheless created if need be</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_OPERATION_DENIED">Source or destination directory is not accessible</errorcode>
 * <errorcode name="RTS_RESULT pResult" type="ERR_FAILED">System error</errorcode>
 */
RTS_RESULT CDECL SysDirCopy(char *pszDestDir, char *pszSourceDir, RTS_BOOL bRecursive, RTS_BOOL bOverwrite);
typedef RTS_RESULT (CDECL * PFSYSDIRCOPY) (char *pszDestDir, char *pszSourceDir, RTS_BOOL bRecursive, RTS_BOOL bOverwrite);
#if defined(SYSDIR_NOTIMPLEMENTED) || defined(SYSDIRCOPY_NOTIMPLEMENTED)
	#define USE_SysDirCopy
	#define EXT_SysDirCopy
	#define GET_SysDirCopy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysDirCopy(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysDirCopy  FALSE
	#define EXP_SysDirCopy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysDirCopy
	#define EXT_SysDirCopy
	#define GET_SysDirCopy(fl)  CAL_CMGETAPI( "SysDirCopy" ) 
	#define CAL_SysDirCopy  SysDirCopy
	#define CHK_SysDirCopy  TRUE
	#define EXP_SysDirCopy  CAL_CMEXPAPI( "SysDirCopy" ) 
#elif defined(MIXED_LINK) && !defined(SYSDIR_EXTERNAL)
	#define USE_SysDirCopy
	#define EXT_SysDirCopy
	#define GET_SysDirCopy(fl)  CAL_CMGETAPI( "SysDirCopy" ) 
	#define CAL_SysDirCopy  SysDirCopy
	#define CHK_SysDirCopy  TRUE
	#define EXP_SysDirCopy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirCopy", (RTS_UINTPTR)SysDirCopy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysDirSysDirCopy
	#define EXT_SysDirSysDirCopy
	#define GET_SysDirSysDirCopy  ERR_OK
	#define CAL_SysDirSysDirCopy pISysDir->ISysDirCopy
	#define CHK_SysDirSysDirCopy (pISysDir != NULL)
	#define EXP_SysDirSysDirCopy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysDirCopy
	#define EXT_SysDirCopy
	#define GET_SysDirCopy(fl)  CAL_CMGETAPI( "SysDirCopy" ) 
	#define CAL_SysDirCopy pISysDir->ISysDirCopy
	#define CHK_SysDirCopy (pISysDir != NULL)
	#define EXP_SysDirCopy  CAL_CMEXPAPI( "SysDirCopy" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysDirCopy  PFSYSDIRCOPY pfSysDirCopy;
	#define EXT_SysDirCopy  extern PFSYSDIRCOPY pfSysDirCopy;
	#define GET_SysDirCopy(fl)  s_pfCMGetAPI2( "SysDirCopy", (RTS_VOID_FCTPTR *)&pfSysDirCopy, (fl), 0, 0)
	#define CAL_SysDirCopy  pfSysDirCopy
	#define CHK_SysDirCopy  (pfSysDirCopy != NULL)
	#define EXP_SysDirCopy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysDirCopy", (RTS_UINTPTR)SysDirCopy, 0, 0) 
#endif





#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSDIROPEN ISysDirOpen;
 	PFSYSDIROPEN_ ISysDirOpen_;
 	PFSYSDIRCLOSE ISysDirClose;
 	PFSYSDIRCLOSE_ ISysDirClose_;
 	PFSYSDIRREAD ISysDirRead;
 	PFSYSDIRREAD_ ISysDirRead_;
 	PFSYSDIRCREATE ISysDirCreate;
 	PFSYSDIRCREATE2 ISysDirCreate2;
 	PFSYSDIRCREATE_ ISysDirCreate_;
 	PFSYSDIRDELETE ISysDirDelete;
 	PFSYSDIRDELETE2 ISysDirDelete2;
 	PFSYSDIRDELETE_ ISysDirDelete_;
 	PFSYSDIRRENAME ISysDirRename;
 	PFSYSDIRRENAME_ ISysDirRename_;
 	PFSYSDIRGETCURRENT ISysDirGetCurrent;
 	PFSYSDIRGETCURRENT_ ISysDirGetCurrent_;
 	PFSYSDIRSETCURRENT ISysDirSetCurrent;
 	PFSYSDIRSETCURRENT_ ISysDirSetCurrent_;
 	PFSYSDIRCOPY ISysDirCopy;
 } ISysDir_C;

#ifdef CPLUSPLUS
class ISysDir : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISysDirOpen(char *pszDir, char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysDirOpen_(char *pszDir, char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysDirClose(void) =0;
		virtual RTS_RESULT CDECL ISysDirClose_(void) =0;
		virtual RTS_RESULT CDECL ISysDirRead(char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo) =0;
		virtual RTS_RESULT CDECL ISysDirRead_(char *pszDirEntry, int iMaxDirEntry, DirInfo *pDirInfo) =0;
		virtual RTS_RESULT CDECL ISysDirCreate(char *pszDir) =0;
		virtual RTS_RESULT CDECL ISysDirCreate2(char *pszDir, RTS_BOOL bRecursive) =0;
		virtual RTS_RESULT CDECL ISysDirCreate_(char *pszDir) =0;
		virtual RTS_RESULT CDECL ISysDirDelete(char *pszDir) =0;
		virtual RTS_RESULT CDECL ISysDirDelete2(char *pszDir, RTS_BOOL bRecursive, RTS_BOOL bKeepDir) =0;
		virtual RTS_RESULT CDECL ISysDirDelete_(char *pszDir) =0;
		virtual RTS_RESULT CDECL ISysDirRename(char *pszOldDir, char *pszNewDir) =0;
		virtual RTS_RESULT CDECL ISysDirRename_(char *pszOldDir, char *pszNewDir) =0;
		virtual RTS_RESULT CDECL ISysDirGetCurrent(char *pszDir, int iMaxDirLen) =0;
		virtual RTS_RESULT CDECL ISysDirGetCurrent_(char *pszDir, int iMaxDirLen) =0;
		virtual RTS_RESULT CDECL ISysDirSetCurrent(char *pszDir) =0;
		virtual RTS_RESULT CDECL ISysDirSetCurrent_(char *pszDir) =0;
		virtual RTS_RESULT CDECL ISysDirCopy(char *pszDestDir, char *pszSourceDir, RTS_BOOL bRecursive, RTS_BOOL bOverwrite) =0;
};
	#ifndef ITF_SysDir
		#define ITF_SysDir static ISysDir *pISysDir = NULL;
	#endif
	#define EXTITF_SysDir
#else	/*CPLUSPLUS*/
	typedef ISysDir_C		ISysDir;
	#ifndef ITF_SysDir
		#define ITF_SysDir
	#endif
	#define EXTITF_SysDir
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSDIRITF_H_*/
