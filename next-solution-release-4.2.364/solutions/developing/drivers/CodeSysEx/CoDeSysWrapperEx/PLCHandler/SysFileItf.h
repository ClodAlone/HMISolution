 /**
 * <interfacename>SysFile</interfacename>
 * <description> 
 *	<p>The SysFile interface is projected to get access to the files of a file system.
 *	The file system can be on hard-disk, a flash/flash disk, a RAM disk or what ever. The only
 *	requirement is, that the file system is non volatile!</p>
 *  <p>Please read following notes if using the SysFileFlash with our SysFlash:
 *	This component needs a global
 *	define of the file table FILE_MAP. It has to be declared in sysdefines.h. Here is an example
 *	with the necessary initializations:
 *	#define FILE1_SIZE	0x4000
 *	#define FILE2_SIZE	0x2000
 *	#define FILE3_SIZE	0x2000
 *	#define FILE4_SIZE	0x38000
 *	#define FILE5_SIZE	0x40000
 *	#define SYSFILEFLASH_MAX_SIZE			(FILE1_SIZE + FILE2_SIZE + FILE3_SIZE + FILE4_SIZE + FILE5_SIZE)
 *	#define FILE_MAP	static FILE_DESC m_FileSystem[] = \
 *				{ \
 *					*	Name		Offset											MaxSize				read index	write index * \
 *					{"file1.txt",	0x0,											FILE1_SIZE,			0xFFFFFFFF, 0xFFFFFFFF}, \
 *					{"app.crc",		FILE1_SIZE,										FILE2_SIZE,			0xFFFFFFFF, 0xFFFFFFFF}, \
 *					{"file2.txt",	FILE1_SIZE+FILE2_SIZE,							FILE3_SIZE,			0xFFFFFFFF, 0xFFFFFFFF}, \
 *					{"file4.txt",	FILE1_SIZE+FILE2_SIZE+FILE3_SIZE,				FILE4_SIZE,			0xFFFFFFFF, 0xFFFFFFFF}, \
 *					{"app.app",		FILE1_SIZE+FILE2_SIZE+FILE3_SIZE+FILE4_SIZE,	FILE5_SIZE,			0xFFFFFFFF, 0xFFFFFFFF}, \
 *				};
 *	Please note that the offsets of the files have to correspond with sector borders of the flash. One file should be
 *	stored in one sector.</p>
 * </description>
 *
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _SYSFILEITF_H_
#define _SYSFILEITF_H_

#include "CmpStd.h"

 

 




#include "CmpStd.h"

#ifndef CDECL
	#define CDECL
#endif

#if defined(WIN32) && defined(_WIN32_WCE)
	#define itoa		_itoa
#endif

/**
 * <category>Compiler switch</category>
 * <description>
 * Compiler switches to enable/disable single features in the component.
 * </description>
 * <element name="SYSFILE_DISABLE_FILE_CACHE">For each transmitted file, the CRC and size is stored in a central configuration file (cache). This can be disabled by this compiler switch.
 * <element name="SYSFILE_NOPLCLOGICMIGRATION">At startup SysFile.PlcLogicPrefix=0 checks for no boot application and than creates the new folder structure with SysFile.PlcLogicPrefix=1. This can be disabled by this compiler switch.
 * </element>
 */ 

/**
 * <category>Static defines</category>
 * <description>Maximum length of a file path</description>
 */
#ifndef MAX_PATH_LEN
	#define MAX_PATH_LEN							255
#endif
#ifndef RTS_MAX_PATH_LEN
	#define RTS_MAX_PATH_LEN						MAX_PATH_LEN
#endif

/**
 * <category>Static defines</category>
 * <description>Size for temporary buffer e.g. to calculate CRC. If size is larger than 256, buffer is allocated from heap!</description>
 */
#ifndef SYSFILE_TEMP_BUFFER_SIZE
     #define SYSFILE_TEMP_BUFFER_SIZE				256
#endif 

/**
 * <category>Static defines</category>
 * <description>File name prefix to make a file invisible e.g. for the file transfer dialog. This prevents the corresponding file to be overwritten/read/compromised from outside.
 * </description>
 */
#ifndef SYSFILE_INVISIBLE_FILENAME_PREFIX
	#define SYSFILE_INVISIBLE_FILENAME_PREFIX		"."
#endif

/**
 * <category>Static defines</category>
 * <description>This is the name for a section which contains the name, CRC and size of a file. Is used as a cache for these values.
 *	Improves for example the performance of the file transfer. SYSFILE_DISABLE_FILE_CACHE disables this feature.
 *	NOTE:
 *	It is recommended to use a file reference in the configuration, to move these entries into a separate cfg-file. e.g.:
 *	[CmpSettings]
 *	FileReference.0=SysFileMap.cfg, SysFileMap
 * </description>
 */
#ifndef SYSFILE_MAP_SECTION_NAME
	#define SYSFILE_MAP_SECTION_NAME				"SysFileMap"
#endif

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description><p>Setting to configure the standard path for file access, for all filenames with no specified path or for directory access.
 *	Several file paths could be entered, if they were indexed, e.g.:</p>
 *	<p>FilePath=D:/Temp/Default
 *	FilePath.1=./Boot, *.app, *.ap_, *.ret, *.frc
 *	FilePath.2=D:/Temp/Wild-cards, *.txw</p>
 *	<p>In this case, files with special extensions could be separated into several directories. File
 *	extensions can be specified with the wild-cards '*'.</p>
 *	<p>If a file extension is not configured, the standard path (FilePath= or FilePath.1=) is used.</p>
 *	NOTE: Indexing starts with .1
 * </description>
 */
#define SYSFILE_KEY_STRING_GET_FILEPATH						"FilePath"
#ifndef SYSFILE_VALUE_STRING_GET_FILEPATH_DEFAULT
	#define SYSFILE_VALUE_STRING_GET_FILEPATH_DEFAULT		""
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Setting to put the PLC into a sandbox for file access, i.e. file access is only allowed inside the configured paths.
 *	If an absolute file path is requested, which is not a configured path, or a directory traversal path an error is returned at this operation (ERR_OPERATION_DENIED).
 *	NOTE: This setting controls all file access from C, from IEC and online (file access from C as far as SysFile operations are concerned).
 * </description>
 */
#define SYSFILE_KEY_INT_FORCE_FILEPATH						"ForceFilePath"
#ifndef SYSFILE_VALUE_INT_FORCE_FILEPATH_DEFAULT
	#define SYSFILE_VALUE_INT_FORCE_FILEPATH_DEFAULT		0
#endif


/**
 * <category>Settings</category>
 * <type>String</type>
 * <description><p>Setting to configure the standard IEC path for file access, for all filenames with no specified path or for directory access.
 *	NOTE: THIS IS USED ONLY FOR ALL FILE ACCESS ROUTINES OUT OF THE IEC PROGRAM!
 *	Several file paths could be entered, if they were indexed, e.g.:</p>
 *	<p>IecFilePath=D:/Temp/IEC
 *	IecFilePath.1=./Boot, *.app, *.ap_, *.ret, *.frc
 *	IecFilePath.2=D:/Temp/Wild-cards, *.txw</p>
 *	<p>In this case, files with special extensions could be separated into several directories. File
 *	extensions can be specified with the wild-cards '*'.</p>
 *	<p>If a file extension is not configured, the standard path (IecFilePath= or IecFilePath.1=) is used.</p>
 *	NOTE: Indexing starts with .1
 *	</description>
 */
#define SYSFILE_KEY_STRING_GET_IEC_FILEPATH					"IecFilePath"
#ifndef SYSFILE_VALUE_STRING_GET_IEC_FILEPATH_DEFAULT
	#define SYSFILE_VALUE_STRING_GET_IEC_FILEPATH_DEFAULT	""
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Setting to put the PLC into a sandbox for file access from IEC, i.e. file access is only allowed inside the configured paths.
 *	If an absolute file path is requested, which is not a configured path, or a directory traversal path an error is returned at this operation (ERR_OPERATION_DENIED).
 *	NOTE: This setting controls all file access from IEC but ForceFilePath (if set) dominates this setting.
 * </description>
 */
#define SYSFILE_KEY_INT_FORCE_IECFILEPATH					"ForceIecFilePath"
#ifndef SYSFILE_VALUE_INT_FORCE_IECFILEPATH_DEFAULT
	#define SYSFILE_VALUE_INT_FORCE_IECFILEPATH_DEFAULT		1
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Setting to put the PLC into a sandbox for online file access, i.e. file access is only allowed inside the configured paths.
 *	If an absolute file path is requested, which is not a configured path, or a directory traversal path an error is returned at this operation (ERR_OPERATION_DENIED).
 *	NOTE: This setting controls all online file access but ForceFilePath (if set) dominates this setting.
 * </description>
 */
#define SYSFILE_KEY_INT_FORCE_ONLINEFILEPATH					"ForceOnlineFilePath"
#ifndef SYSFILE_VALUE_INT_FORCE_ONLINEFILEPATH_DEFAULT
	#define SYSFILE_VALUE_INT_FORCE_ONLINEFILEPATH_DEFAULT		1
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>Setting to deny online access to all configuration files.
 *	If a configuration file is requested, an error is returned at this operation (ERR_OPERATION_DENIED).
 *	NOTE: This setting is independent of any Force settings.
 * </description>
 */
#define SYSFILE_KEY_INT_DENYONLINEACCESSCFGFILE					"DenyOnlineAccessCfgFile"
#ifndef SYSFILE_VALUE_INT_DENYONLINEACCESSCFGFILE_DEFAULT
	#define SYSFILE_VALUE_INT_DENYONLINEACCESSCFGFILE_DEFAULT		1
#endif


/**
 * <category>Settings</category>
 * <type>String</type>
 * <description><p>Setting to get the placeholder file path for file access. A placeholder is defined like a relative path, but with special placeholder marks, e.g.:
 *		$visu$/mainwindows.bmp
 * The setting has the following structure:
 *		PlaceholderFilePath=[directory, relative or absolute], [placeholder]
 * Example:
 *		PlaceholderFilePath=visu, $visu$
 *	Several file paths could be entered, if they were indexed, e.g.:</p>
 *	<p>PlaceholderFilePath=PlcLogic, $PlcLogic$
 *	PlaceholderFilePath.1=visu, $visu$</p>
 *	PlaceholderFilePath.2=D:\Temp, $Temp$</p>
 *	NOTE: Indexing starts with .1
 *	</description>
 */
#define SYSFILE_KEY_STRING_GET_PLACEHOLDER_FILEPATH					"PlaceholderFilePath"
#ifndef SYSFILE_VALUE_STRING_GET_PLACEHOLDER_FILEPATH_DEFAULT
	#define SYSFILE_VALUE_STRING_GET_PLACEHOLDER_FILEPATH_DEFAULT	""
#endif

/**
 * <category>Static defines</category>
 * <description>
 *  File placeholder attributes
 * </description>
 */
#define SYSFILE_PLACEHOLDER_ATTRIBUTE_NONE				0x00
#define SYSFILE_PLACEHOLDER_ATTRIBUTE_VIEW				0x01
#define SYSFILE_PLACEHOLDER_ATTRIBUTE_VOLATILE			0x02
#define SYSFILE_PLACEHOLDER_ATTRIBUTE_CREATE_ON_WRITE	0x04
#define SYSFILE_PLACEHOLDER_ATTRIBUTE_TEMPORARY			0x08
#define SYSFILE_PLACEHOLDER_ATTRIBUTE_VALID_MASK		0x0F

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description><p>Setting to make a placeholder visible in the file transfer window.
 * Example:
 *		PlaceholderFilePath.View=1</p>
 *	</description>
 */
#define SYSFILE_KEY_EXT_VIEW										".View"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description><p>Setting to make all placeholders visible in the file transfer window.
 * Example:
 *		PlaceholderFilePath.All.View=1</p>
 *	</description>
 */
#define SYSFILE_KEY_INT_PLACEHOLDER_FILEPATH_ALL_VIEW				"PlaceholderFilePath.All.View"
#ifndef SYSFILE_VALUE_INT_PLACEHOLDER_FILEPATH_ALL_VIEW_DEFAULT
	#define SYSFILE_VALUE_INT_PLACEHOLDER_FILEPATH_ALL_VIEW_DEFAULT	0
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description><p>Setting to mark a placeholder as volatile, i.e. the specified folder is not created automatically at startup and the placeholder is visible in the file transfer windows only when the folder exists.
 * This behavior is important for hot plug devices like USB sticks. 
 * Example:
 *		PlaceholderFilePath.2=/mnt/USBDevice, $USBDev$
 *		PlaceholderFilePath.2.Volatile=1</p>
 *	</description>
 */
#define SYSFILE_KEY_EXT_VOLATILE									".Volatile"

/**
 * <category>Settings</category>
 * <type>String</type>
 * <description><p>Setting to connect a placeholder virtually with a parent placeholder, i.e. in the file transfer window this placeholder is reachable via its virtual parent placeholder.
 * Example:
 *		PlaceholderFilePath.1=/temp/oem, $oem$
 *		PlaceholderFilePath.1.VParent=$PlcLogic$
 *	The oem placeholder is virtually placed under $PlcLogic$ but physically at /temp/oem.</p>
 *	</description>
 */
#define SYSFILE_KEY_EXT_VPARENT										".VParent"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description><p>Setting to mark a placeholder so that the specified folder is created when a file is to be written in that folder, the  folder is not created at startup, the placeholder is visible in the filetransfer windows only when the folder exists.
 * Example:
 *		PlaceholderFilePath.2=/Temp, $Temp$
 *		PlaceholderFilePath.2.CreateOnWrite=1</p>
 *	</description>
 */
#define SYSFILE_KEY_EXT_CREATEONWRITE								".CreateOnWrite"

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description><p>Setting to mark a placeholder as temporary, that is, the  folder is not created at startup, the placeholder is visible in the filetransfer windows only when the folder exists.
 * This is for folders with absolute paths created and deleted via online services, especially when ForceOnlineFilePath is set, which is default.
 * Example:
 *		PlaceholderFilePath.2=/Temp, $Temp$
 *		PlaceholderFilePath.2.Temporary=1</p>
 *	</description>
 */
#define SYSFILE_KEY_EXT_TEMPORARY									".Temporary"

/**
 * <category>Static defines</category>
 * <description>
 * Setting to configure the PLC logic placeholder prefix that is used now by all application related files.
 * -1 - old behavior: flat file structure (default)
 *  0 - old behavior: flat file structure, but Reset Origin Device will change that to 1
 *  1 - new folder structure:
 *    $PlcLogic$
 *    + &lt;application&gt;
 *    + $trend$
 *    + $alarm$
 *    + $ac_persistence$
 *    + $visu$
 * </description>
 */
#define SYSFILE_KEY_INT_PLCLOGIC_PREFIX					"PlcLogicPrefix"
#ifndef SYSFILE_VALUE_INT_PLCLOGIC_PREFIX_DEFAULT
	#define SYSFILE_VALUE_INT_PLCLOGIC_PREFIX_DEFAULT	-1
#endif


/**
 * <category>File placeholder</category>
 * <description>Defines to specify the visualization folder that is requested by every file transfer/file access on visualization files. And we specify the placeholder for there files, for which
 *	the destination folder can be specified with the setting "PlaceholderFilePath" (see details above).
 * </description>
 */
#ifndef SYSFILE_VISU_PLACEHOLDER
	#define SYSFILE_VISU_PLACEHOLDER			"$visu$"
#endif
#ifndef SYSFILE_VISU_FOLDER
	#define SYSFILE_VISU_FOLDER					"visu/"
#endif

/**
 * <category>File placeholder</category>
 * <description>Defines to specify the storage location for certificate related files.
 *	Storage for import/export of certificates: $cert$.
 *	Storage for certificates and keys: $.pki$. Leading . to make tha folder in Filetransfer dialog invisible.
 *	The destination folder can be specified with the setting "PlaceholderFilePath" (see details above).
 * </description>
 */
 #ifndef SYSFILE_CERT_PLACEHOLDER
	#define SYSFILE_CERT_PLACEHOLDER		    "$cert$"
#endif
#ifndef SYSFILE_PKI_PLACEHOLDER
	#define SYSFILE_PKI_PLACEHOLDER			    "$.pki$"
#endif

/**
 * <category>File placeholder</category>
 * <description>
 * Defines to specify the PLC logic folder that is requested by application and device related file transfers. 
 * The destination folder can be specified with the setting "PlaceholderFilePath" (see details above).
 * </description>
 */
#ifndef SYSFILE_PLC_LOGIC_PLACEHOLDER
	#define SYSFILE_PLC_LOGIC_PLACEHOLDER		"$PlcLogic$"
#endif
#ifndef SYSFILE_PLC_LOGIC_FOLDER
	#define SYSFILE_PLC_LOGIC_FOLDER			"PlcLogic/"
#endif

/**
 * <category>File placeholder</category>
 * <description>
 * Placeholder defines for grouping trend database files (application related files):
 * The destination folders can be specified with the setting "PlaceholderFilePath" (see details above).
 * If not configured resolving depends on "PlcLogicPrefix" (see above).
 * </description>
 */
#ifndef SYSFILE_TREND_PLACEHOLDER
	#define SYSFILE_TREND_PLACEHOLDER			"$trend$"
#endif
#ifndef SYSFILE_TREND_FOLDER
	#define SYSFILE_TREND_FOLDER				"trend/"
#endif

/**
 * <category>File placeholder</category>
 * <description>
 * Placeholder defines for grouping alarm database files (application related files):
 * The destination folders can be specified with the setting "PlaceholderFilePath" (see details above).
 * If not configured resolving depends on "PlcLogicPrefix" (see above).
 * </description>
 */
#ifndef SYSFILE_ALARMS_PLACEHOLDER
	#define SYSFILE_ALARMS_PLACEHOLDER			"$alarms$"
#endif
#ifndef SYSFILE_ALARMS_FOLDER
	#define SYSFILE_ALARMS_FOLDER				"alarms/"
#endif

/**
 * <category>File placeholder</category>
 * <description>
 * Placeholder defines for grouping ac persistence files (application related files):
 * The destination folders can be specified with the setting "PlaceholderFilePath" (see details above).
 * If not configured resolving depends on "PlcLogicPrefix" (see above).
 * </description>
 */
#ifndef SYSFILE_AC_PERSISTENCE_PLACEHOLDER
	#define SYSFILE_AC_PERSISTENCE_PLACEHOLDER	"$ac_persistence$"
#endif
#ifndef SYSFILE_AC_PERSISTENCE_FOLDER
	#define SYSFILE_AC_PERSISTENCE_FOLDER		"ac_persistence/"
#endif

/**
 * <category>File placeholder</category>
 * <description>
 * Placeholder defines for backup operations:
 * The destination folders can be specified with the setting "PlaceholderFilePath" (see details above).
 * </description>
 */
#ifndef SYSFILE_BACKUP_PLACEHOLDER
	#define SYSFILE_BACKUP_PLACEHOLDER			"$TBF$"
#endif
#ifndef SYSFILE_BACKUP_FOLDER
	#define SYSFILE_BACKUP_FOLDER				"TBF/"
#endif

/**
 * <category>File placeholder</category>
 * <description>
 * Placeholder defines for CNC files:
 * The destination folders can be specified with the setting "PlaceholderFilePath" (see details above).
 * </description>
 */
#ifndef SYSFILE_CNC_PLACEHOLDER
	#define SYSFILE_CNC_PLACEHOLDER			    "$_cnc$"
#endif
#ifndef SYSFILE_CNC_FOLDER
	#define SYSFILE_CNC_FOLDER				    "_cnc/"
#endif

/**
 * <category>File placeholder</category>
 * <description>
 * Placeholder to define folder for all logger files.
 * </description>
 */
#define SYSFILE_LOGFILES_PLACEHOLDER		    "$logfiles$"
#ifndef SYSFILE_LOGFILES_FOLDER
	#define SYSFILE_LOGFILES_FOLDER			    ""
#endif

/**
 * <category>File placeholder</category>
 * <description>
 * Placeholder to define the folder for the project archive.
 * The destination folder can be specified with the setting "PlaceholderFilePath" (see details above).
 * </description>
 */
#ifndef SYSFILE_PROJECTARCHIVE_PLACEHOLDER
	#define SYSFILE_PROJECTARCHIVE_PLACEHOLDER			"$projectarchive$"
#endif
#ifndef SYSFILE_PROJECTARCHIVE_FOLDER
	#define SYSFILE_PROJECTARCHIVE_FOLDER				SYSFILE_PLC_LOGIC_PLACEHOLDER
#endif


/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * | Access mode
 * | File modes to open a file.
 *
 * .. note::
 *    For all ``*_PLUS`` modes be aware, that after reading from a file, writing can only be done after a call
 *    to |SysFileGetPos| or |SysFileSetPos|! If you call |SysFileWrite| right after |SysFileRead|,
 *    the file pointer could be on an invalid position!
 *
 *    Correct example::
 *
 * 	     SysFileRead();
 * 	     SysFileGetPos();
 * 	     SysFileWrite();
 */
#define RTS_ACCESS_MODE_AM_READ    RTS_IEC_UDINT_C(0x0)	/* Open an existing file with Read access. If file does not exist, Open fails */
#define RTS_ACCESS_MODE_AM_WRITE    RTS_IEC_UDINT_C(0x1)	/* Create new file with Write access. If file does exist, content is discarded */
#define RTS_ACCESS_MODE_AM_APPEND    RTS_IEC_UDINT_C(0x2)	/* Open an existing file with Append (only write) access. If file does not exist, Open fails */
#define RTS_ACCESS_MODE_AM_READ_PLUS    RTS_IEC_UDINT_C(0x3)	/* Open an existing file with Read/Write access. If file does not exist, Open fails */
#define RTS_ACCESS_MODE_AM_WRITE_PLUS    RTS_IEC_UDINT_C(0x4)	/* Create new file with Read/Write access. If file does exist, content is discarded */
#define RTS_ACCESS_MODE_AM_APPEND_PLUS    RTS_IEC_UDINT_C(0x5)	/* Open an existing file with Append (read/write) access. If file does not exist, Open creates a new file */
/* Typed enum definition */
#define RTS_ACCESS_MODE    RTS_IEC_UDINT

/**
 * | File status
 * | Actual file status of the specified file.
 */
#define SYS_FILE_STATUS_FS_OK    RTS_IEC_INT_C(0x0)	/* File could be opened */
#define SYS_FILE_STATUS_FS_NO_FILE    RTS_IEC_INT_C(0x1)	/* No file available */
#define SYS_FILE_STATUS_FS_ILLEGAL_POS    RTS_IEC_INT_C(0x2)	/* Illegal position in the file */
#define SYS_FILE_STATUS_FS_FULL    RTS_IEC_INT_C(0x3)	/* No more space on the file system */
#define SYS_FILE_STATUS_FS_EOF    RTS_IEC_INT_C(0x4)	/* End of file reached */
/* Typed enum definition */
#define SYS_FILE_STATUS    RTS_IEC_INT

/**
 * | File TIME
 * | Timestamps of the specified file.
 */
typedef struct tagSYS_FILETIME
{
	RTS_IEC_UDINT tCreation;		/* Creation time of the file */
	RTS_IEC_UDINT tLastAccess;		/* Time of as access to the file */
	RTS_IEC_UDINT tLastModification;		/* Time of last modification */
} SYS_FILETIME;

/**
 * Close a file specified by handle
 * :return: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysfileclose_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_RESULT SysFileClose;		/* VAR_OUTPUT */	
} sysfileclose_struct;

void CDECL CDECL_EXT sysfileclose(sysfileclose_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILECLOSE_IEC) (sysfileclose_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILECLOSE_NOTIMPLEMENTED)
	#define USE_sysfileclose
	#define EXT_sysfileclose
	#define GET_sysfileclose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfileclose(p0) 
	#define CHK_sysfileclose  FALSE
	#define EXP_sysfileclose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfileclose
	#define EXT_sysfileclose
	#define GET_sysfileclose(fl)  CAL_CMGETAPI( "sysfileclose" ) 
	#define CAL_sysfileclose  sysfileclose
	#define CHK_sysfileclose  TRUE
	#define EXP_sysfileclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileclose", (RTS_UINTPTR)sysfileclose, 1, RTSITF_GET_SIGNATURE(0, 0xC19E28BA), 0x03051100) 
	#define EXTREF_ITF_sysfileclose {(RTS_VOID_FCTPTR)sysfileclose, "sysfileclose", RTSITF_GET_SIGNATURE(0, 0xC19E28BA), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfileclose
	#define EXT_sysfileclose
	#define GET_sysfileclose(fl)  CAL_CMGETAPI( "sysfileclose" ) 
	#define CAL_sysfileclose  sysfileclose
	#define CHK_sysfileclose  TRUE
	#define EXP_sysfileclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileclose", (RTS_UINTPTR)sysfileclose, 1, RTSITF_GET_SIGNATURE(0, 0xC19E28BA), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfileclose
	#define EXT_SysFilesysfileclose
	#define GET_SysFilesysfileclose  ERR_OK
	#define CAL_SysFilesysfileclose  sysfileclose
	#define CHK_SysFilesysfileclose  TRUE
	#define EXP_SysFilesysfileclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileclose", (RTS_UINTPTR)sysfileclose, 1, RTSITF_GET_SIGNATURE(0, 0xC19E28BA), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfileclose
	#define EXT_sysfileclose
	#define GET_sysfileclose(fl)  CAL_CMGETAPI( "sysfileclose" ) 
	#define CAL_sysfileclose  sysfileclose
	#define CHK_sysfileclose  TRUE
	#define EXP_sysfileclose  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileclose", (RTS_UINTPTR)sysfileclose, 1, RTSITF_GET_SIGNATURE(0, 0xC19E28BA), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfileclose  PFSYSFILECLOSE_IEC pfsysfileclose;
	#define EXT_sysfileclose  extern PFSYSFILECLOSE_IEC pfsysfileclose;
	#define GET_sysfileclose(fl)  s_pfCMGetAPI2( "sysfileclose", (RTS_VOID_FCTPTR *)&pfsysfileclose, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xC19E28BA), 0x03051100)
	#define CAL_sysfileclose  pfsysfileclose
	#define CHK_sysfileclose  (pfsysfileclose != NULL)
	#define EXP_sysfileclose   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileclose", (RTS_UINTPTR)sysfileclose, 1, RTSITF_GET_SIGNATURE(0, 0xC19E28BA), 0x03051100) 
#endif


/**
 * | Copy one file to another.
 * | A standard path will be added to the filename, if no path is specified.
 * :return: The runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysfilecopy_struct
{
	RTS_IEC_STRING *szDestFileName;		/* VAR_INPUT */	/* Destination file name. File name can contain an absolute or relative path to the file.
 Path entries must be separated with a Slash (/)  and not with a Backslash (\\)! */
	RTS_IEC_STRING *szSourceFileName;	/* VAR_INPUT */	/* Source file name. File name can contain an absolute or relative path to the file.											
 Path entries must be separated with a Slash (/)  and not with a Backslash (\\)! */
	RTS_IEC_XWORD *pulCopied;			/* VAR_INPUT */	/* Number of bytes copied */
	RTS_IEC_RESULT SysFileCopy;			/* VAR_OUTPUT */	
} sysfilecopy_struct;

void CDECL CDECL_EXT sysfilecopy(sysfilecopy_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILECOPY_IEC) (sysfilecopy_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILECOPY_NOTIMPLEMENTED)
	#define USE_sysfilecopy
	#define EXT_sysfilecopy
	#define GET_sysfilecopy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilecopy(p0) 
	#define CHK_sysfilecopy  FALSE
	#define EXP_sysfilecopy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilecopy
	#define EXT_sysfilecopy
	#define GET_sysfilecopy(fl)  CAL_CMGETAPI( "sysfilecopy" ) 
	#define CAL_sysfilecopy  sysfilecopy
	#define CHK_sysfilecopy  TRUE
	#define EXP_sysfilecopy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilecopy", (RTS_UINTPTR)sysfilecopy, 1, RTSITF_GET_SIGNATURE(0, 0xDB491610), 0x03051100) 
	#define EXTREF_ITF_sysfilecopy {(RTS_VOID_FCTPTR)sysfilecopy, "sysfilecopy", RTSITF_GET_SIGNATURE(0, 0xDB491610), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilecopy
	#define EXT_sysfilecopy
	#define GET_sysfilecopy(fl)  CAL_CMGETAPI( "sysfilecopy" ) 
	#define CAL_sysfilecopy  sysfilecopy
	#define CHK_sysfilecopy  TRUE
	#define EXP_sysfilecopy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilecopy", (RTS_UINTPTR)sysfilecopy, 1, RTSITF_GET_SIGNATURE(0, 0xDB491610), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilecopy
	#define EXT_SysFilesysfilecopy
	#define GET_SysFilesysfilecopy  ERR_OK
	#define CAL_SysFilesysfilecopy  sysfilecopy
	#define CHK_SysFilesysfilecopy  TRUE
	#define EXP_SysFilesysfilecopy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilecopy", (RTS_UINTPTR)sysfilecopy, 1, RTSITF_GET_SIGNATURE(0, 0xDB491610), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilecopy
	#define EXT_sysfilecopy
	#define GET_sysfilecopy(fl)  CAL_CMGETAPI( "sysfilecopy" ) 
	#define CAL_sysfilecopy  sysfilecopy
	#define CHK_sysfilecopy  TRUE
	#define EXP_sysfilecopy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilecopy", (RTS_UINTPTR)sysfilecopy, 1, RTSITF_GET_SIGNATURE(0, 0xDB491610), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilecopy  PFSYSFILECOPY_IEC pfsysfilecopy;
	#define EXT_sysfilecopy  extern PFSYSFILECOPY_IEC pfsysfilecopy;
	#define GET_sysfilecopy(fl)  s_pfCMGetAPI2( "sysfilecopy", (RTS_VOID_FCTPTR *)&pfsysfilecopy, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xDB491610), 0x03051100)
	#define CAL_sysfilecopy  pfsysfilecopy
	#define CHK_sysfilecopy  (pfsysfilecopy != NULL)
	#define EXP_sysfilecopy   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilecopy", (RTS_UINTPTR)sysfilecopy, 1, RTSITF_GET_SIGNATURE(0, 0xDB491610), 0x03051100) 
#endif


/**
 * | Delete the file specified by name.
 * | A standard path will be added in the runtime system to the filename, if no path is specified.
 * :return: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysfiledelete_struct
{
	RTS_IEC_STRING *szFileName;			/* VAR_INPUT */	/* File name. File name can contain an absolute or relative path to the file.
 Path entries must be separated with a Slash (/)  and not with a Backslash (\\)! */
	RTS_IEC_RESULT SysFileDelete;		/* VAR_OUTPUT */	
} sysfiledelete_struct;

void CDECL CDECL_EXT sysfiledelete(sysfiledelete_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEDELETE_IEC) (sysfiledelete_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEDELETE_NOTIMPLEMENTED)
	#define USE_sysfiledelete
	#define EXT_sysfiledelete
	#define GET_sysfiledelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfiledelete(p0) 
	#define CHK_sysfiledelete  FALSE
	#define EXP_sysfiledelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfiledelete
	#define EXT_sysfiledelete
	#define GET_sysfiledelete(fl)  CAL_CMGETAPI( "sysfiledelete" ) 
	#define CAL_sysfiledelete  sysfiledelete
	#define CHK_sysfiledelete  TRUE
	#define EXP_sysfiledelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiledelete", (RTS_UINTPTR)sysfiledelete, 1, RTSITF_GET_SIGNATURE(0, 0xA977F762), 0x03051100) 
	#define EXTREF_ITF_sysfiledelete {(RTS_VOID_FCTPTR)sysfiledelete, "sysfiledelete", RTSITF_GET_SIGNATURE(0, 0xA977F762), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfiledelete
	#define EXT_sysfiledelete
	#define GET_sysfiledelete(fl)  CAL_CMGETAPI( "sysfiledelete" ) 
	#define CAL_sysfiledelete  sysfiledelete
	#define CHK_sysfiledelete  TRUE
	#define EXP_sysfiledelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiledelete", (RTS_UINTPTR)sysfiledelete, 1, RTSITF_GET_SIGNATURE(0, 0xA977F762), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfiledelete
	#define EXT_SysFilesysfiledelete
	#define GET_SysFilesysfiledelete  ERR_OK
	#define CAL_SysFilesysfiledelete  sysfiledelete
	#define CHK_SysFilesysfiledelete  TRUE
	#define EXP_SysFilesysfiledelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiledelete", (RTS_UINTPTR)sysfiledelete, 1, RTSITF_GET_SIGNATURE(0, 0xA977F762), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfiledelete
	#define EXT_sysfiledelete
	#define GET_sysfiledelete(fl)  CAL_CMGETAPI( "sysfiledelete" ) 
	#define CAL_sysfiledelete  sysfiledelete
	#define CHK_sysfiledelete  TRUE
	#define EXP_sysfiledelete  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiledelete", (RTS_UINTPTR)sysfiledelete, 1, RTSITF_GET_SIGNATURE(0, 0xA977F762), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfiledelete  PFSYSFILEDELETE_IEC pfsysfiledelete;
	#define EXT_sysfiledelete  extern PFSYSFILEDELETE_IEC pfsysfiledelete;
	#define GET_sysfiledelete(fl)  s_pfCMGetAPI2( "sysfiledelete", (RTS_VOID_FCTPTR *)&pfsysfiledelete, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xA977F762), 0x03051100)
	#define CAL_sysfiledelete  pfsysfiledelete
	#define CHK_sysfiledelete  (pfsysfiledelete != NULL)
	#define EXP_sysfiledelete   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiledelete", (RTS_UINTPTR)sysfiledelete, 1, RTSITF_GET_SIGNATURE(0, 0xA977F762), 0x03051100) 
#endif


/**
 * Delete the file specified by handle
 * :return: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysfiledeletebyhandle_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_RESULT SysFileDeleteByHandle;	/* VAR_OUTPUT */	
} sysfiledeletebyhandle_struct;

void CDECL CDECL_EXT sysfiledeletebyhandle(sysfiledeletebyhandle_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEDELETEBYHANDLE_IEC) (sysfiledeletebyhandle_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEDELETEBYHANDLE_NOTIMPLEMENTED)
	#define USE_sysfiledeletebyhandle
	#define EXT_sysfiledeletebyhandle
	#define GET_sysfiledeletebyhandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfiledeletebyhandle(p0) 
	#define CHK_sysfiledeletebyhandle  FALSE
	#define EXP_sysfiledeletebyhandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfiledeletebyhandle
	#define EXT_sysfiledeletebyhandle
	#define GET_sysfiledeletebyhandle(fl)  CAL_CMGETAPI( "sysfiledeletebyhandle" ) 
	#define CAL_sysfiledeletebyhandle  sysfiledeletebyhandle
	#define CHK_sysfiledeletebyhandle  TRUE
	#define EXP_sysfiledeletebyhandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiledeletebyhandle", (RTS_UINTPTR)sysfiledeletebyhandle, 1, RTSITF_GET_SIGNATURE(0, 0xC812562E), 0x03051100) 
	#define EXTREF_ITF_sysfiledeletebyhandle {(RTS_VOID_FCTPTR)sysfiledeletebyhandle, "sysfiledeletebyhandle", RTSITF_GET_SIGNATURE(0, 0xC812562E), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfiledeletebyhandle
	#define EXT_sysfiledeletebyhandle
	#define GET_sysfiledeletebyhandle(fl)  CAL_CMGETAPI( "sysfiledeletebyhandle" ) 
	#define CAL_sysfiledeletebyhandle  sysfiledeletebyhandle
	#define CHK_sysfiledeletebyhandle  TRUE
	#define EXP_sysfiledeletebyhandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiledeletebyhandle", (RTS_UINTPTR)sysfiledeletebyhandle, 1, RTSITF_GET_SIGNATURE(0, 0xC812562E), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfiledeletebyhandle
	#define EXT_SysFilesysfiledeletebyhandle
	#define GET_SysFilesysfiledeletebyhandle  ERR_OK
	#define CAL_SysFilesysfiledeletebyhandle  sysfiledeletebyhandle
	#define CHK_SysFilesysfiledeletebyhandle  TRUE
	#define EXP_SysFilesysfiledeletebyhandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiledeletebyhandle", (RTS_UINTPTR)sysfiledeletebyhandle, 1, RTSITF_GET_SIGNATURE(0, 0xC812562E), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfiledeletebyhandle
	#define EXT_sysfiledeletebyhandle
	#define GET_sysfiledeletebyhandle(fl)  CAL_CMGETAPI( "sysfiledeletebyhandle" ) 
	#define CAL_sysfiledeletebyhandle  sysfiledeletebyhandle
	#define CHK_sysfiledeletebyhandle  TRUE
	#define EXP_sysfiledeletebyhandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiledeletebyhandle", (RTS_UINTPTR)sysfiledeletebyhandle, 1, RTSITF_GET_SIGNATURE(0, 0xC812562E), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfiledeletebyhandle  PFSYSFILEDELETEBYHANDLE_IEC pfsysfiledeletebyhandle;
	#define EXT_sysfiledeletebyhandle  extern PFSYSFILEDELETEBYHANDLE_IEC pfsysfiledeletebyhandle;
	#define GET_sysfiledeletebyhandle(fl)  s_pfCMGetAPI2( "sysfiledeletebyhandle", (RTS_VOID_FCTPTR *)&pfsysfiledeletebyhandle, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xC812562E), 0x03051100)
	#define CAL_sysfiledeletebyhandle  pfsysfiledeletebyhandle
	#define CHK_sysfiledeletebyhandle  (pfsysfiledeletebyhandle != NULL)
	#define EXP_sysfiledeletebyhandle   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiledeletebyhandle", (RTS_UINTPTR)sysfiledeletebyhandle, 1, RTSITF_GET_SIGNATURE(0, 0xC812562E), 0x03051100) 
#endif


/**
 *  Check, if end of file is reached
 *
 *  .. note::
 * 		End of file is only checked after a read operation with SysFileRead! But after a SysFileWrite or SysFileSetPos call, the function
 * 		returns ERR_FAILED (no end of file)!
 * :return: Returns the runtime system error code (see CmpErrors.library):
 *	+ ERR_OK: End of file reached at reading beyond the end of the file
 *	+ ERR_FAILED: No end of file reached
 *	+ ERR_PARAMETER: hFile is invalid
 */
typedef struct tagsysfileeof_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_RESULT SysFileEOF;			/* VAR_OUTPUT */	
} sysfileeof_struct;

void CDECL CDECL_EXT sysfileeof(sysfileeof_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEEOF_IEC) (sysfileeof_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEEOF_NOTIMPLEMENTED)
	#define USE_sysfileeof
	#define EXT_sysfileeof
	#define GET_sysfileeof(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfileeof(p0) 
	#define CHK_sysfileeof  FALSE
	#define EXP_sysfileeof  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfileeof
	#define EXT_sysfileeof
	#define GET_sysfileeof(fl)  CAL_CMGETAPI( "sysfileeof" ) 
	#define CAL_sysfileeof  sysfileeof
	#define CHK_sysfileeof  TRUE
	#define EXP_sysfileeof  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileeof", (RTS_UINTPTR)sysfileeof, 1, RTSITF_GET_SIGNATURE(0, 0x30A3EB85), 0x03051100) 
	#define EXTREF_ITF_sysfileeof {(RTS_VOID_FCTPTR)sysfileeof, "sysfileeof", RTSITF_GET_SIGNATURE(0, 0x30A3EB85), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfileeof
	#define EXT_sysfileeof
	#define GET_sysfileeof(fl)  CAL_CMGETAPI( "sysfileeof" ) 
	#define CAL_sysfileeof  sysfileeof
	#define CHK_sysfileeof  TRUE
	#define EXP_sysfileeof  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileeof", (RTS_UINTPTR)sysfileeof, 1, RTSITF_GET_SIGNATURE(0, 0x30A3EB85), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfileeof
	#define EXT_SysFilesysfileeof
	#define GET_SysFilesysfileeof  ERR_OK
	#define CAL_SysFilesysfileeof  sysfileeof
	#define CHK_SysFilesysfileeof  TRUE
	#define EXP_SysFilesysfileeof  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileeof", (RTS_UINTPTR)sysfileeof, 1, RTSITF_GET_SIGNATURE(0, 0x30A3EB85), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfileeof
	#define EXT_sysfileeof
	#define GET_sysfileeof(fl)  CAL_CMGETAPI( "sysfileeof" ) 
	#define CAL_sysfileeof  sysfileeof
	#define CHK_sysfileeof  TRUE
	#define EXP_sysfileeof  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileeof", (RTS_UINTPTR)sysfileeof, 1, RTSITF_GET_SIGNATURE(0, 0x30A3EB85), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfileeof  PFSYSFILEEOF_IEC pfsysfileeof;
	#define EXT_sysfileeof  extern PFSYSFILEEOF_IEC pfsysfileeof;
	#define GET_sysfileeof(fl)  s_pfCMGetAPI2( "sysfileeof", (RTS_VOID_FCTPTR *)&pfsysfileeof, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x30A3EB85), 0x03051100)
	#define CAL_sysfileeof  pfsysfileeof
	#define CHK_sysfileeof  (pfsysfileeof != NULL)
	#define EXP_sysfileeof   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileeof", (RTS_UINTPTR)sysfileeof, 1, RTSITF_GET_SIGNATURE(0, 0x30A3EB85), 0x03051100) 
#endif


/**
 * Flush the file cache and write into the file
 * :return: Returns the runtime system error code (see CmpErrors_Itfs.library):
 *
 * - ERR_OK: Succeeded flushing the file
 * - ERR_FAILED: Error occurred during file flush
 * - ERR_NOTIMPLEMENTED: File flush is not implemented
 * - ERR_NOT_SUPPORTED: File flush not available on the target
 */
typedef struct tagsysfileflush_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_RESULT SysFileFlush;		/* VAR_OUTPUT */	
} sysfileflush_struct;

void CDECL CDECL_EXT sysfileflush(sysfileflush_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEFLUSH_IEC) (sysfileflush_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEFLUSH_NOTIMPLEMENTED)
	#define USE_sysfileflush
	#define EXT_sysfileflush
	#define GET_sysfileflush(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfileflush(p0) 
	#define CHK_sysfileflush  FALSE
	#define EXP_sysfileflush  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfileflush
	#define EXT_sysfileflush
	#define GET_sysfileflush(fl)  CAL_CMGETAPI( "sysfileflush" ) 
	#define CAL_sysfileflush  sysfileflush
	#define CHK_sysfileflush  TRUE
	#define EXP_sysfileflush  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileflush", (RTS_UINTPTR)sysfileflush, 1, 0x32985005, 0x03051100) 
	#define EXTREF_ITF_sysfileflush {(RTS_VOID_FCTPTR)sysfileflush, "sysfileflush", 0x32985005, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfileflush
	#define EXT_sysfileflush
	#define GET_sysfileflush(fl)  CAL_CMGETAPI( "sysfileflush" ) 
	#define CAL_sysfileflush  sysfileflush
	#define CHK_sysfileflush  TRUE
	#define EXP_sysfileflush  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileflush", (RTS_UINTPTR)sysfileflush, 1, 0x32985005, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfileflush
	#define EXT_SysFilesysfileflush
	#define GET_SysFilesysfileflush  ERR_OK
	#define CAL_SysFilesysfileflush  sysfileflush
	#define CHK_SysFilesysfileflush  TRUE
	#define EXP_SysFilesysfileflush  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileflush", (RTS_UINTPTR)sysfileflush, 1, 0x32985005, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfileflush
	#define EXT_sysfileflush
	#define GET_sysfileflush(fl)  CAL_CMGETAPI( "sysfileflush" ) 
	#define CAL_sysfileflush  sysfileflush
	#define CHK_sysfileflush  TRUE
	#define EXP_sysfileflush  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileflush", (RTS_UINTPTR)sysfileflush, 1, 0x32985005, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfileflush  PFSYSFILEFLUSH_IEC pfsysfileflush;
	#define EXT_sysfileflush  extern PFSYSFILEFLUSH_IEC pfsysfileflush;
	#define GET_sysfileflush(fl)  s_pfCMGetAPI2( "sysfileflush", (RTS_VOID_FCTPTR *)&pfsysfileflush, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x32985005, 0x03051100)
	#define CAL_sysfileflush  pfsysfileflush
	#define CHK_sysfileflush  (pfsysfileflush != NULL)
	#define EXP_sysfileflush   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileflush", (RTS_UINTPTR)sysfileflush, 1, 0x32985005, 0x03051100) 
#endif


/**
 * Get the file name from file specified by handle
 * :return: File name of the specified file
 */
typedef struct tagsysfilegetname_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_STRING *SysFileGetName;		/* VAR_OUTPUT */	
} sysfilegetname_struct;

void CDECL CDECL_EXT sysfilegetname(sysfilegetname_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEGETNAME_IEC) (sysfilegetname_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETNAME_NOTIMPLEMENTED)
	#define USE_sysfilegetname
	#define EXT_sysfilegetname
	#define GET_sysfilegetname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilegetname(p0) 
	#define CHK_sysfilegetname  FALSE
	#define EXP_sysfilegetname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilegetname
	#define EXT_sysfilegetname
	#define GET_sysfilegetname(fl)  CAL_CMGETAPI( "sysfilegetname" ) 
	#define CAL_sysfilegetname  sysfilegetname
	#define CHK_sysfilegetname  TRUE
	#define EXP_sysfilegetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetname", (RTS_UINTPTR)sysfilegetname, 1, RTSITF_GET_SIGNATURE(0, 0x4AEE0669), 0x03051100) 
	#define EXTREF_ITF_sysfilegetname {(RTS_VOID_FCTPTR)sysfilegetname, "sysfilegetname", RTSITF_GET_SIGNATURE(0, 0x4AEE0669), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilegetname
	#define EXT_sysfilegetname
	#define GET_sysfilegetname(fl)  CAL_CMGETAPI( "sysfilegetname" ) 
	#define CAL_sysfilegetname  sysfilegetname
	#define CHK_sysfilegetname  TRUE
	#define EXP_sysfilegetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetname", (RTS_UINTPTR)sysfilegetname, 1, RTSITF_GET_SIGNATURE(0, 0x4AEE0669), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilegetname
	#define EXT_SysFilesysfilegetname
	#define GET_SysFilesysfilegetname  ERR_OK
	#define CAL_SysFilesysfilegetname  sysfilegetname
	#define CHK_SysFilesysfilegetname  TRUE
	#define EXP_SysFilesysfilegetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetname", (RTS_UINTPTR)sysfilegetname, 1, RTSITF_GET_SIGNATURE(0, 0x4AEE0669), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilegetname
	#define EXT_sysfilegetname
	#define GET_sysfilegetname(fl)  CAL_CMGETAPI( "sysfilegetname" ) 
	#define CAL_sysfilegetname  sysfilegetname
	#define CHK_sysfilegetname  TRUE
	#define EXP_sysfilegetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetname", (RTS_UINTPTR)sysfilegetname, 1, RTSITF_GET_SIGNATURE(0, 0x4AEE0669), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilegetname  PFSYSFILEGETNAME_IEC pfsysfilegetname;
	#define EXT_sysfilegetname  extern PFSYSFILEGETNAME_IEC pfsysfilegetname;
	#define GET_sysfilegetname(fl)  s_pfCMGetAPI2( "sysfilegetname", (RTS_VOID_FCTPTR *)&pfsysfilegetname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x4AEE0669), 0x03051100)
	#define CAL_sysfilegetname  pfsysfilegetname
	#define CHK_sysfilegetname  (pfsysfilegetname != NULL)
	#define EXP_sysfilegetname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetname", (RTS_UINTPTR)sysfilegetname, 1, RTSITF_GET_SIGNATURE(0, 0x4AEE0669), 0x03051100) 
#endif


/**
 * Get the file name from file specified by handle
 * :return: File name of the specified file
 */
typedef struct tagsysfilegetname2_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_STRING *SysFileGetName2;	/* VAR_OUTPUT */	
} sysfilegetname2_struct;

void CDECL CDECL_EXT sysfilegetname2(sysfilegetname2_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEGETNAME2_IEC) (sysfilegetname2_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETNAME2_NOTIMPLEMENTED)
	#define USE_sysfilegetname2
	#define EXT_sysfilegetname2
	#define GET_sysfilegetname2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilegetname2(p0) 
	#define CHK_sysfilegetname2  FALSE
	#define EXP_sysfilegetname2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilegetname2
	#define EXT_sysfilegetname2
	#define GET_sysfilegetname2(fl)  CAL_CMGETAPI( "sysfilegetname2" ) 
	#define CAL_sysfilegetname2  sysfilegetname2
	#define CHK_sysfilegetname2  TRUE
	#define EXP_sysfilegetname2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetname2", (RTS_UINTPTR)sysfilegetname2, 1, RTSITF_GET_SIGNATURE(0, 0x2856F375), 0x03051100) 
	#define EXTREF_ITF_sysfilegetname2 {(RTS_VOID_FCTPTR)sysfilegetname2, "sysfilegetname2", RTSITF_GET_SIGNATURE(0, 0x2856F375), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilegetname2
	#define EXT_sysfilegetname2
	#define GET_sysfilegetname2(fl)  CAL_CMGETAPI( "sysfilegetname2" ) 
	#define CAL_sysfilegetname2  sysfilegetname2
	#define CHK_sysfilegetname2  TRUE
	#define EXP_sysfilegetname2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetname2", (RTS_UINTPTR)sysfilegetname2, 1, RTSITF_GET_SIGNATURE(0, 0x2856F375), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilegetname2
	#define EXT_SysFilesysfilegetname2
	#define GET_SysFilesysfilegetname2  ERR_OK
	#define CAL_SysFilesysfilegetname2  sysfilegetname2
	#define CHK_SysFilesysfilegetname2  TRUE
	#define EXP_SysFilesysfilegetname2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetname2", (RTS_UINTPTR)sysfilegetname2, 1, RTSITF_GET_SIGNATURE(0, 0x2856F375), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilegetname2
	#define EXT_sysfilegetname2
	#define GET_sysfilegetname2(fl)  CAL_CMGETAPI( "sysfilegetname2" ) 
	#define CAL_sysfilegetname2  sysfilegetname2
	#define CHK_sysfilegetname2  TRUE
	#define EXP_sysfilegetname2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetname2", (RTS_UINTPTR)sysfilegetname2, 1, RTSITF_GET_SIGNATURE(0, 0x2856F375), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilegetname2  PFSYSFILEGETNAME2_IEC pfsysfilegetname2;
	#define EXT_sysfilegetname2  extern PFSYSFILEGETNAME2_IEC pfsysfilegetname2;
	#define GET_sysfilegetname2(fl)  s_pfCMGetAPI2( "sysfilegetname2", (RTS_VOID_FCTPTR *)&pfsysfilegetname2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x2856F375), 0x03051100)
	#define CAL_sysfilegetname2  pfsysfilegetname2
	#define CHK_sysfilegetname2  (pfsysfilegetname2 != NULL)
	#define EXP_sysfilegetname2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetname2", (RTS_UINTPTR)sysfilegetname2, 1, RTSITF_GET_SIGNATURE(0, 0x2856F375), 0x03051100) 
#endif


/**
 * | Get the path of this file.
 * | If a path is specified in the filename, the path will be extracted from the filename.
 * | If no path is specified in the filename, the standard path for this file extension type will be returned.
 * :return: The runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysfilegetpath_struct
{
	RTS_IEC_STRING *szFileName;			/* VAR_INPUT */	/* File name. Can contain an absolute or relative path */
	RTS_IEC_STRING *szPath;				/* VAR_IN_OUT */	/* Path for this file */
	RTS_IEC_DINT diMaxLen;				/* VAR_INPUT */	/* Maximum size in bytes of path length */
	RTS_IEC_RESULT SysFileGetPath;		/* VAR_OUTPUT */	
} sysfilegetpath_struct;

void CDECL CDECL_EXT sysfilegetpath(sysfilegetpath_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEGETPATH_IEC) (sysfilegetpath_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETPATH_NOTIMPLEMENTED)
	#define USE_sysfilegetpath
	#define EXT_sysfilegetpath
	#define GET_sysfilegetpath(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilegetpath(p0) 
	#define CHK_sysfilegetpath  FALSE
	#define EXP_sysfilegetpath  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilegetpath
	#define EXT_sysfilegetpath
	#define GET_sysfilegetpath(fl)  CAL_CMGETAPI( "sysfilegetpath" ) 
	#define CAL_sysfilegetpath  sysfilegetpath
	#define CHK_sysfilegetpath  TRUE
	#define EXP_sysfilegetpath  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetpath", (RTS_UINTPTR)sysfilegetpath, 1, 0xE8836F87, 0x03051100) 
	#define EXTREF_ITF_sysfilegetpath {(RTS_VOID_FCTPTR)sysfilegetpath, "sysfilegetpath", 0xE8836F87, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilegetpath
	#define EXT_sysfilegetpath
	#define GET_sysfilegetpath(fl)  CAL_CMGETAPI( "sysfilegetpath" ) 
	#define CAL_sysfilegetpath  sysfilegetpath
	#define CHK_sysfilegetpath  TRUE
	#define EXP_sysfilegetpath  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetpath", (RTS_UINTPTR)sysfilegetpath, 1, 0xE8836F87, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilegetpath
	#define EXT_SysFilesysfilegetpath
	#define GET_SysFilesysfilegetpath  ERR_OK
	#define CAL_SysFilesysfilegetpath  sysfilegetpath
	#define CHK_SysFilesysfilegetpath  TRUE
	#define EXP_SysFilesysfilegetpath  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetpath", (RTS_UINTPTR)sysfilegetpath, 1, 0xE8836F87, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilegetpath
	#define EXT_sysfilegetpath
	#define GET_sysfilegetpath(fl)  CAL_CMGETAPI( "sysfilegetpath" ) 
	#define CAL_sysfilegetpath  sysfilegetpath
	#define CHK_sysfilegetpath  TRUE
	#define EXP_sysfilegetpath  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetpath", (RTS_UINTPTR)sysfilegetpath, 1, 0xE8836F87, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilegetpath  PFSYSFILEGETPATH_IEC pfsysfilegetpath;
	#define EXT_sysfilegetpath  extern PFSYSFILEGETPATH_IEC pfsysfilegetpath;
	#define GET_sysfilegetpath(fl)  s_pfCMGetAPI2( "sysfilegetpath", (RTS_VOID_FCTPTR *)&pfsysfilegetpath, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xE8836F87, 0x03051100)
	#define CAL_sysfilegetpath  pfsysfilegetpath
	#define CHK_sysfilegetpath  (pfsysfilegetpath != NULL)
	#define EXP_sysfilegetpath   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetpath", (RTS_UINTPTR)sysfilegetpath, 1, 0xE8836F87, 0x03051100) 
#endif


/**
 * Get actual file pointer position
 * :return: The runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysfilegetpos_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_XWORD *pulPos;				/* VAR_INPUT */	/* Pointer to get actual position of the file pointer from the beginning of the file */
	RTS_IEC_RESULT SysFileGetPos;		/* VAR_OUTPUT */	
} sysfilegetpos_struct;

void CDECL CDECL_EXT sysfilegetpos(sysfilegetpos_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEGETPOS_IEC) (sysfilegetpos_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETPOS_NOTIMPLEMENTED)
	#define USE_sysfilegetpos
	#define EXT_sysfilegetpos
	#define GET_sysfilegetpos(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilegetpos(p0) 
	#define CHK_sysfilegetpos  FALSE
	#define EXP_sysfilegetpos  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilegetpos
	#define EXT_sysfilegetpos
	#define GET_sysfilegetpos(fl)  CAL_CMGETAPI( "sysfilegetpos" ) 
	#define CAL_sysfilegetpos  sysfilegetpos
	#define CHK_sysfilegetpos  TRUE
	#define EXP_sysfilegetpos  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetpos", (RTS_UINTPTR)sysfilegetpos, 1, RTSITF_GET_SIGNATURE(0, 0x4F0954A2), 0x03051100) 
	#define EXTREF_ITF_sysfilegetpos {(RTS_VOID_FCTPTR)sysfilegetpos, "sysfilegetpos", RTSITF_GET_SIGNATURE(0, 0x4F0954A2), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilegetpos
	#define EXT_sysfilegetpos
	#define GET_sysfilegetpos(fl)  CAL_CMGETAPI( "sysfilegetpos" ) 
	#define CAL_sysfilegetpos  sysfilegetpos
	#define CHK_sysfilegetpos  TRUE
	#define EXP_sysfilegetpos  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetpos", (RTS_UINTPTR)sysfilegetpos, 1, RTSITF_GET_SIGNATURE(0, 0x4F0954A2), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilegetpos
	#define EXT_SysFilesysfilegetpos
	#define GET_SysFilesysfilegetpos  ERR_OK
	#define CAL_SysFilesysfilegetpos  sysfilegetpos
	#define CHK_SysFilesysfilegetpos  TRUE
	#define EXP_SysFilesysfilegetpos  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetpos", (RTS_UINTPTR)sysfilegetpos, 1, RTSITF_GET_SIGNATURE(0, 0x4F0954A2), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilegetpos
	#define EXT_sysfilegetpos
	#define GET_sysfilegetpos(fl)  CAL_CMGETAPI( "sysfilegetpos" ) 
	#define CAL_sysfilegetpos  sysfilegetpos
	#define CHK_sysfilegetpos  TRUE
	#define EXP_sysfilegetpos  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetpos", (RTS_UINTPTR)sysfilegetpos, 1, RTSITF_GET_SIGNATURE(0, 0x4F0954A2), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilegetpos  PFSYSFILEGETPOS_IEC pfsysfilegetpos;
	#define EXT_sysfilegetpos  extern PFSYSFILEGETPOS_IEC pfsysfilegetpos;
	#define GET_sysfilegetpos(fl)  s_pfCMGetAPI2( "sysfilegetpos", (RTS_VOID_FCTPTR *)&pfsysfilegetpos, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x4F0954A2), 0x03051100)
	#define CAL_sysfilegetpos  pfsysfilegetpos
	#define CHK_sysfilegetpos  (pfsysfilegetpos != NULL)
	#define EXP_sysfilegetpos   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetpos", (RTS_UINTPTR)sysfilegetpos, 1, RTSITF_GET_SIGNATURE(0, 0x4F0954A2), 0x03051100) 
#endif


/**
 * | Get file size of the file specified by name.
 * | A standard path will be added to the filename, if no path is specified.
 * :return: Size of the file in bytes
 */
typedef struct tagsysfilegetsize_struct
{
	RTS_IEC_STRING *szFileName;			/* VAR_INPUT */	/* File name. File name can contain an absolute or relative path to the file.
 Path entries must be separated with a Slash (/) and not with a Backslash (\\)! */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library):

 - ERR_OK: Successful
 - ERR_NO_OBJECT: File not available
 - ERR_FAILED: Failed to get file size */
	RTS_IEC_XWORD SysFileGetSize;		/* VAR_OUTPUT */	
} sysfilegetsize_struct;

void CDECL CDECL_EXT sysfilegetsize(sysfilegetsize_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEGETSIZE_IEC) (sysfilegetsize_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETSIZE_NOTIMPLEMENTED)
	#define USE_sysfilegetsize
	#define EXT_sysfilegetsize
	#define GET_sysfilegetsize(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilegetsize(p0) 
	#define CHK_sysfilegetsize  FALSE
	#define EXP_sysfilegetsize  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilegetsize
	#define EXT_sysfilegetsize
	#define GET_sysfilegetsize(fl)  CAL_CMGETAPI( "sysfilegetsize" ) 
	#define CAL_sysfilegetsize  sysfilegetsize
	#define CHK_sysfilegetsize  TRUE
	#define EXP_sysfilegetsize  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetsize", (RTS_UINTPTR)sysfilegetsize, 1, RTSITF_GET_SIGNATURE(0, 0x0A16C4DB), 0x03051100) 
	#define EXTREF_ITF_sysfilegetsize {(RTS_VOID_FCTPTR)sysfilegetsize, "sysfilegetsize", RTSITF_GET_SIGNATURE(0, 0x0A16C4DB), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilegetsize
	#define EXT_sysfilegetsize
	#define GET_sysfilegetsize(fl)  CAL_CMGETAPI( "sysfilegetsize" ) 
	#define CAL_sysfilegetsize  sysfilegetsize
	#define CHK_sysfilegetsize  TRUE
	#define EXP_sysfilegetsize  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetsize", (RTS_UINTPTR)sysfilegetsize, 1, RTSITF_GET_SIGNATURE(0, 0x0A16C4DB), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilegetsize
	#define EXT_SysFilesysfilegetsize
	#define GET_SysFilesysfilegetsize  ERR_OK
	#define CAL_SysFilesysfilegetsize  sysfilegetsize
	#define CHK_SysFilesysfilegetsize  TRUE
	#define EXP_SysFilesysfilegetsize  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetsize", (RTS_UINTPTR)sysfilegetsize, 1, RTSITF_GET_SIGNATURE(0, 0x0A16C4DB), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilegetsize
	#define EXT_sysfilegetsize
	#define GET_sysfilegetsize(fl)  CAL_CMGETAPI( "sysfilegetsize" ) 
	#define CAL_sysfilegetsize  sysfilegetsize
	#define CHK_sysfilegetsize  TRUE
	#define EXP_sysfilegetsize  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetsize", (RTS_UINTPTR)sysfilegetsize, 1, RTSITF_GET_SIGNATURE(0, 0x0A16C4DB), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilegetsize  PFSYSFILEGETSIZE_IEC pfsysfilegetsize;
	#define EXT_sysfilegetsize  extern PFSYSFILEGETSIZE_IEC pfsysfilegetsize;
	#define GET_sysfilegetsize(fl)  s_pfCMGetAPI2( "sysfilegetsize", (RTS_VOID_FCTPTR *)&pfsysfilegetsize, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x0A16C4DB), 0x03051100)
	#define CAL_sysfilegetsize  pfsysfilegetsize
	#define CHK_sysfilegetsize  (pfsysfilegetsize != NULL)
	#define EXP_sysfilegetsize   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetsize", (RTS_UINTPTR)sysfilegetsize, 1, RTSITF_GET_SIGNATURE(0, 0x0A16C4DB), 0x03051100) 
#endif


/**
 * Get file size of the file specified by handle
 * :return: Size of the file in bytes
 */
typedef struct tagsysfilegetsizebyhandle_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XWORD SysFileGetSizeByHandle;	/* VAR_OUTPUT */	
} sysfilegetsizebyhandle_struct;

void CDECL CDECL_EXT sysfilegetsizebyhandle(sysfilegetsizebyhandle_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEGETSIZEBYHANDLE_IEC) (sysfilegetsizebyhandle_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETSIZEBYHANDLE_NOTIMPLEMENTED)
	#define USE_sysfilegetsizebyhandle
	#define EXT_sysfilegetsizebyhandle
	#define GET_sysfilegetsizebyhandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilegetsizebyhandle(p0) 
	#define CHK_sysfilegetsizebyhandle  FALSE
	#define EXP_sysfilegetsizebyhandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilegetsizebyhandle
	#define EXT_sysfilegetsizebyhandle
	#define GET_sysfilegetsizebyhandle(fl)  CAL_CMGETAPI( "sysfilegetsizebyhandle" ) 
	#define CAL_sysfilegetsizebyhandle  sysfilegetsizebyhandle
	#define CHK_sysfilegetsizebyhandle  TRUE
	#define EXP_sysfilegetsizebyhandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetsizebyhandle", (RTS_UINTPTR)sysfilegetsizebyhandle, 1, RTSITF_GET_SIGNATURE(0, 0x847C756B), 0x03051100) 
	#define EXTREF_ITF_sysfilegetsizebyhandle {(RTS_VOID_FCTPTR)sysfilegetsizebyhandle, "sysfilegetsizebyhandle", RTSITF_GET_SIGNATURE(0, 0x847C756B), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilegetsizebyhandle
	#define EXT_sysfilegetsizebyhandle
	#define GET_sysfilegetsizebyhandle(fl)  CAL_CMGETAPI( "sysfilegetsizebyhandle" ) 
	#define CAL_sysfilegetsizebyhandle  sysfilegetsizebyhandle
	#define CHK_sysfilegetsizebyhandle  TRUE
	#define EXP_sysfilegetsizebyhandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetsizebyhandle", (RTS_UINTPTR)sysfilegetsizebyhandle, 1, RTSITF_GET_SIGNATURE(0, 0x847C756B), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilegetsizebyhandle
	#define EXT_SysFilesysfilegetsizebyhandle
	#define GET_SysFilesysfilegetsizebyhandle  ERR_OK
	#define CAL_SysFilesysfilegetsizebyhandle  sysfilegetsizebyhandle
	#define CHK_SysFilesysfilegetsizebyhandle  TRUE
	#define EXP_SysFilesysfilegetsizebyhandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetsizebyhandle", (RTS_UINTPTR)sysfilegetsizebyhandle, 1, RTSITF_GET_SIGNATURE(0, 0x847C756B), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilegetsizebyhandle
	#define EXT_sysfilegetsizebyhandle
	#define GET_sysfilegetsizebyhandle(fl)  CAL_CMGETAPI( "sysfilegetsizebyhandle" ) 
	#define CAL_sysfilegetsizebyhandle  sysfilegetsizebyhandle
	#define CHK_sysfilegetsizebyhandle  TRUE
	#define EXP_sysfilegetsizebyhandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetsizebyhandle", (RTS_UINTPTR)sysfilegetsizebyhandle, 1, RTSITF_GET_SIGNATURE(0, 0x847C756B), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilegetsizebyhandle  PFSYSFILEGETSIZEBYHANDLE_IEC pfsysfilegetsizebyhandle;
	#define EXT_sysfilegetsizebyhandle  extern PFSYSFILEGETSIZEBYHANDLE_IEC pfsysfilegetsizebyhandle;
	#define GET_sysfilegetsizebyhandle(fl)  s_pfCMGetAPI2( "sysfilegetsizebyhandle", (RTS_VOID_FCTPTR *)&pfsysfilegetsizebyhandle, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x847C756B), 0x03051100)
	#define CAL_sysfilegetsizebyhandle  pfsysfilegetsizebyhandle
	#define CHK_sysfilegetsizebyhandle  (pfsysfilegetsizebyhandle != NULL)
	#define EXP_sysfilegetsizebyhandle   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetsizebyhandle", (RTS_UINTPTR)sysfilegetsizebyhandle, 1, RTSITF_GET_SIGNATURE(0, 0x847C756B), 0x03051100) 
#endif


/**
 * Get the file status
 * :return: File status. See category file status
 */
typedef struct tagsysfilegetstatus_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_INT SysFileGetStatus;		/* VAR_OUTPUT, Enumeration: SYS_FILE_STATUS */
} sysfilegetstatus_struct;

void CDECL CDECL_EXT sysfilegetstatus(sysfilegetstatus_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEGETSTATUS_IEC) (sysfilegetstatus_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETSTATUS_NOTIMPLEMENTED)
	#define USE_sysfilegetstatus
	#define EXT_sysfilegetstatus
	#define GET_sysfilegetstatus(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilegetstatus(p0) 
	#define CHK_sysfilegetstatus  FALSE
	#define EXP_sysfilegetstatus  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilegetstatus
	#define EXT_sysfilegetstatus
	#define GET_sysfilegetstatus(fl)  CAL_CMGETAPI( "sysfilegetstatus" ) 
	#define CAL_sysfilegetstatus  sysfilegetstatus
	#define CHK_sysfilegetstatus  TRUE
	#define EXP_sysfilegetstatus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetstatus", (RTS_UINTPTR)sysfilegetstatus, 1, RTSITF_GET_SIGNATURE(0, 0x31FB098C), 0x03051100) 
	#define EXTREF_ITF_sysfilegetstatus {(RTS_VOID_FCTPTR)sysfilegetstatus, "sysfilegetstatus", RTSITF_GET_SIGNATURE(0, 0x31FB098C), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilegetstatus
	#define EXT_sysfilegetstatus
	#define GET_sysfilegetstatus(fl)  CAL_CMGETAPI( "sysfilegetstatus" ) 
	#define CAL_sysfilegetstatus  sysfilegetstatus
	#define CHK_sysfilegetstatus  TRUE
	#define EXP_sysfilegetstatus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetstatus", (RTS_UINTPTR)sysfilegetstatus, 1, RTSITF_GET_SIGNATURE(0, 0x31FB098C), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilegetstatus
	#define EXT_SysFilesysfilegetstatus
	#define GET_SysFilesysfilegetstatus  ERR_OK
	#define CAL_SysFilesysfilegetstatus  sysfilegetstatus
	#define CHK_SysFilesysfilegetstatus  TRUE
	#define EXP_SysFilesysfilegetstatus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetstatus", (RTS_UINTPTR)sysfilegetstatus, 1, RTSITF_GET_SIGNATURE(0, 0x31FB098C), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilegetstatus
	#define EXT_sysfilegetstatus
	#define GET_sysfilegetstatus(fl)  CAL_CMGETAPI( "sysfilegetstatus" ) 
	#define CAL_sysfilegetstatus  sysfilegetstatus
	#define CHK_sysfilegetstatus  TRUE
	#define EXP_sysfilegetstatus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetstatus", (RTS_UINTPTR)sysfilegetstatus, 1, RTSITF_GET_SIGNATURE(0, 0x31FB098C), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilegetstatus  PFSYSFILEGETSTATUS_IEC pfsysfilegetstatus;
	#define EXT_sysfilegetstatus  extern PFSYSFILEGETSTATUS_IEC pfsysfilegetstatus;
	#define GET_sysfilegetstatus(fl)  s_pfCMGetAPI2( "sysfilegetstatus", (RTS_VOID_FCTPTR *)&pfsysfilegetstatus, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x31FB098C), 0x03051100)
	#define CAL_sysfilegetstatus  pfsysfilegetstatus
	#define CHK_sysfilegetstatus  (pfsysfilegetstatus != NULL)
	#define EXP_sysfilegetstatus   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetstatus", (RTS_UINTPTR)sysfilegetstatus, 1, RTSITF_GET_SIGNATURE(0, 0x31FB098C), 0x03051100) 
#endif


/**
 * Get the file status
 * :return: File status. See category file status
 */
typedef struct tagsysfilegetstatus2_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_INT SysFileGetStatus2;		/* VAR_OUTPUT, Enumeration: SYS_FILE_STATUS */
} sysfilegetstatus2_struct;

void CDECL CDECL_EXT sysfilegetstatus2(sysfilegetstatus2_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEGETSTATUS2_IEC) (sysfilegetstatus2_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETSTATUS2_NOTIMPLEMENTED)
	#define USE_sysfilegetstatus2
	#define EXT_sysfilegetstatus2
	#define GET_sysfilegetstatus2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilegetstatus2(p0) 
	#define CHK_sysfilegetstatus2  FALSE
	#define EXP_sysfilegetstatus2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilegetstatus2
	#define EXT_sysfilegetstatus2
	#define GET_sysfilegetstatus2(fl)  CAL_CMGETAPI( "sysfilegetstatus2" ) 
	#define CAL_sysfilegetstatus2  sysfilegetstatus2
	#define CHK_sysfilegetstatus2  TRUE
	#define EXP_sysfilegetstatus2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetstatus2", (RTS_UINTPTR)sysfilegetstatus2, 1, RTSITF_GET_SIGNATURE(0, 0x59D8FC78), 0x03051100) 
	#define EXTREF_ITF_sysfilegetstatus2 {(RTS_VOID_FCTPTR)sysfilegetstatus2, "sysfilegetstatus2", RTSITF_GET_SIGNATURE(0, 0x59D8FC78), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilegetstatus2
	#define EXT_sysfilegetstatus2
	#define GET_sysfilegetstatus2(fl)  CAL_CMGETAPI( "sysfilegetstatus2" ) 
	#define CAL_sysfilegetstatus2  sysfilegetstatus2
	#define CHK_sysfilegetstatus2  TRUE
	#define EXP_sysfilegetstatus2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetstatus2", (RTS_UINTPTR)sysfilegetstatus2, 1, RTSITF_GET_SIGNATURE(0, 0x59D8FC78), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilegetstatus2
	#define EXT_SysFilesysfilegetstatus2
	#define GET_SysFilesysfilegetstatus2  ERR_OK
	#define CAL_SysFilesysfilegetstatus2  sysfilegetstatus2
	#define CHK_SysFilesysfilegetstatus2  TRUE
	#define EXP_SysFilesysfilegetstatus2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetstatus2", (RTS_UINTPTR)sysfilegetstatus2, 1, RTSITF_GET_SIGNATURE(0, 0x59D8FC78), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilegetstatus2
	#define EXT_sysfilegetstatus2
	#define GET_sysfilegetstatus2(fl)  CAL_CMGETAPI( "sysfilegetstatus2" ) 
	#define CAL_sysfilegetstatus2  sysfilegetstatus2
	#define CHK_sysfilegetstatus2  TRUE
	#define EXP_sysfilegetstatus2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetstatus2", (RTS_UINTPTR)sysfilegetstatus2, 1, RTSITF_GET_SIGNATURE(0, 0x59D8FC78), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilegetstatus2  PFSYSFILEGETSTATUS2_IEC pfsysfilegetstatus2;
	#define EXT_sysfilegetstatus2  extern PFSYSFILEGETSTATUS2_IEC pfsysfilegetstatus2;
	#define GET_sysfilegetstatus2(fl)  s_pfCMGetAPI2( "sysfilegetstatus2", (RTS_VOID_FCTPTR *)&pfsysfilegetstatus2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x59D8FC78), 0x03051100)
	#define CAL_sysfilegetstatus2  pfsysfilegetstatus2
	#define CHK_sysfilegetstatus2  (pfsysfilegetstatus2 != NULL)
	#define EXP_sysfilegetstatus2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegetstatus2", (RTS_UINTPTR)sysfilegetstatus2, 1, RTSITF_GET_SIGNATURE(0, 0x59D8FC78), 0x03051100) 
#endif


/**
 * | Get file time of the specified file.
 * | A standard path will be added to the filename, if no path is specified.
 * :return: The runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysfilegettime_struct
{
	RTS_IEC_STRING *szFileName;			/* VAR_INPUT */	/* File name. File name can contain an absolute or relative path to the file.	
 Path entries must be separated with a Slash (/)  and not with a Backslash (\\)! */
	SYS_FILETIME *ptFileTime;			/* VAR_INPUT */	/* Pointer to get the file time results. */
	RTS_IEC_RESULT SysFileGetTime;		/* VAR_OUTPUT */	
} sysfilegettime_struct;

void CDECL CDECL_EXT sysfilegettime(sysfilegettime_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEGETTIME_IEC) (sysfilegettime_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETTIME_NOTIMPLEMENTED)
	#define USE_sysfilegettime
	#define EXT_sysfilegettime
	#define GET_sysfilegettime(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilegettime(p0) 
	#define CHK_sysfilegettime  FALSE
	#define EXP_sysfilegettime  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilegettime
	#define EXT_sysfilegettime
	#define GET_sysfilegettime(fl)  CAL_CMGETAPI( "sysfilegettime" ) 
	#define CAL_sysfilegettime  sysfilegettime
	#define CHK_sysfilegettime  TRUE
	#define EXP_sysfilegettime  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegettime", (RTS_UINTPTR)sysfilegettime, 1, RTSITF_GET_SIGNATURE(0, 0xB2AF6BDD), 0x03051100) 
	#define EXTREF_ITF_sysfilegettime {(RTS_VOID_FCTPTR)sysfilegettime, "sysfilegettime", RTSITF_GET_SIGNATURE(0, 0xB2AF6BDD), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilegettime
	#define EXT_sysfilegettime
	#define GET_sysfilegettime(fl)  CAL_CMGETAPI( "sysfilegettime" ) 
	#define CAL_sysfilegettime  sysfilegettime
	#define CHK_sysfilegettime  TRUE
	#define EXP_sysfilegettime  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegettime", (RTS_UINTPTR)sysfilegettime, 1, RTSITF_GET_SIGNATURE(0, 0xB2AF6BDD), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilegettime
	#define EXT_SysFilesysfilegettime
	#define GET_SysFilesysfilegettime  ERR_OK
	#define CAL_SysFilesysfilegettime  sysfilegettime
	#define CHK_SysFilesysfilegettime  TRUE
	#define EXP_SysFilesysfilegettime  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegettime", (RTS_UINTPTR)sysfilegettime, 1, RTSITF_GET_SIGNATURE(0, 0xB2AF6BDD), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilegettime
	#define EXT_sysfilegettime
	#define GET_sysfilegettime(fl)  CAL_CMGETAPI( "sysfilegettime" ) 
	#define CAL_sysfilegettime  sysfilegettime
	#define CHK_sysfilegettime  TRUE
	#define EXP_sysfilegettime  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegettime", (RTS_UINTPTR)sysfilegettime, 1, RTSITF_GET_SIGNATURE(0, 0xB2AF6BDD), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilegettime  PFSYSFILEGETTIME_IEC pfsysfilegettime;
	#define EXT_sysfilegettime  extern PFSYSFILEGETTIME_IEC pfsysfilegettime;
	#define GET_sysfilegettime(fl)  s_pfCMGetAPI2( "sysfilegettime", (RTS_VOID_FCTPTR *)&pfsysfilegettime, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB2AF6BDD), 0x03051100)
	#define CAL_sysfilegettime  pfsysfilegettime
	#define CHK_sysfilegettime  (pfsysfilegettime != NULL)
	#define EXP_sysfilegettime   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilegettime", (RTS_UINTPTR)sysfilegettime, 1, RTSITF_GET_SIGNATURE(0, 0xB2AF6BDD), 0x03051100) 
#endif


/**
 * | Open or create file. A standard path will be added to the filename, if no path is specified in the file name.
 * | If a file extension is specified in the settings, this path will be used (see category settings).
 *
 * .. note:: File name can contain an absolute or relative path to the file. Path entries
 *           must be separated with a Slash (/)  and not with a Backslash (\\)!
 *
 * :return: Handle to the file or RTS_INVALID_HANDLE if failed
 */
typedef struct tagsysfileopen_struct
{
	RTS_IEC_STRING *szFile;				/* VAR_INPUT */	/* File name. File name can contain an absolute or relative path to the file.	
 Path entries must be separated with a Slash (/)  and not with a Backslash (\\)! */
	RTS_IEC_UDINT am;					/* VAR_INPUT, Enumeration: ACCESS_MODE */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_HANDLE SysFileOpen;			/* VAR_OUTPUT */	
} sysfileopen_struct;

void CDECL CDECL_EXT sysfileopen(sysfileopen_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEOPEN_IEC) (sysfileopen_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEOPEN_NOTIMPLEMENTED)
	#define USE_sysfileopen
	#define EXT_sysfileopen
	#define GET_sysfileopen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfileopen(p0) 
	#define CHK_sysfileopen  FALSE
	#define EXP_sysfileopen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfileopen
	#define EXT_sysfileopen
	#define GET_sysfileopen(fl)  CAL_CMGETAPI( "sysfileopen" ) 
	#define CAL_sysfileopen  sysfileopen
	#define CHK_sysfileopen  TRUE
	#define EXP_sysfileopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileopen", (RTS_UINTPTR)sysfileopen, 1, RTSITF_GET_SIGNATURE(0, 0xFE31259F), 0x03051100) 
	#define EXTREF_ITF_sysfileopen {(RTS_VOID_FCTPTR)sysfileopen, "sysfileopen", RTSITF_GET_SIGNATURE(0, 0xFE31259F), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfileopen
	#define EXT_sysfileopen
	#define GET_sysfileopen(fl)  CAL_CMGETAPI( "sysfileopen" ) 
	#define CAL_sysfileopen  sysfileopen
	#define CHK_sysfileopen  TRUE
	#define EXP_sysfileopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileopen", (RTS_UINTPTR)sysfileopen, 1, RTSITF_GET_SIGNATURE(0, 0xFE31259F), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfileopen
	#define EXT_SysFilesysfileopen
	#define GET_SysFilesysfileopen  ERR_OK
	#define CAL_SysFilesysfileopen  sysfileopen
	#define CHK_SysFilesysfileopen  TRUE
	#define EXP_SysFilesysfileopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileopen", (RTS_UINTPTR)sysfileopen, 1, RTSITF_GET_SIGNATURE(0, 0xFE31259F), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfileopen
	#define EXT_sysfileopen
	#define GET_sysfileopen(fl)  CAL_CMGETAPI( "sysfileopen" ) 
	#define CAL_sysfileopen  sysfileopen
	#define CHK_sysfileopen  TRUE
	#define EXP_sysfileopen  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileopen", (RTS_UINTPTR)sysfileopen, 1, RTSITF_GET_SIGNATURE(0, 0xFE31259F), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfileopen  PFSYSFILEOPEN_IEC pfsysfileopen;
	#define EXT_sysfileopen  extern PFSYSFILEOPEN_IEC pfsysfileopen;
	#define GET_sysfileopen(fl)  s_pfCMGetAPI2( "sysfileopen", (RTS_VOID_FCTPTR *)&pfsysfileopen, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xFE31259F), 0x03051100)
	#define CAL_sysfileopen  pfsysfileopen
	#define CHK_sysfileopen  (pfsysfileopen != NULL)
	#define EXP_sysfileopen   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileopen", (RTS_UINTPTR)sysfileopen, 1, RTSITF_GET_SIGNATURE(0, 0xFE31259F), 0x03051100) 
#endif


/**
 * Read number of bytes out of the file
 * :return: Number of bytes read from file. 0=if failed
 */
typedef struct tagsysfileread_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Pointer to buffer for read data */
	RTS_IEC_XWORD ulSize;				/* VAR_INPUT */	/* Number of bytes to read from file. Must be less or equal the buffer size! */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XWORD SysFileRead;			/* VAR_OUTPUT */	
} sysfileread_struct;

void CDECL CDECL_EXT sysfileread(sysfileread_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEREAD_IEC) (sysfileread_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEREAD_NOTIMPLEMENTED)
	#define USE_sysfileread
	#define EXT_sysfileread
	#define GET_sysfileread(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfileread(p0) 
	#define CHK_sysfileread  FALSE
	#define EXP_sysfileread  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfileread
	#define EXT_sysfileread
	#define GET_sysfileread(fl)  CAL_CMGETAPI( "sysfileread" ) 
	#define CAL_sysfileread  sysfileread
	#define CHK_sysfileread  TRUE
	#define EXP_sysfileread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileread", (RTS_UINTPTR)sysfileread, 1, RTSITF_GET_SIGNATURE(0, 0xD962D4F9), 0x03051100) 
	#define EXTREF_ITF_sysfileread {(RTS_VOID_FCTPTR)sysfileread, "sysfileread", RTSITF_GET_SIGNATURE(0, 0xD962D4F9), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfileread
	#define EXT_sysfileread
	#define GET_sysfileread(fl)  CAL_CMGETAPI( "sysfileread" ) 
	#define CAL_sysfileread  sysfileread
	#define CHK_sysfileread  TRUE
	#define EXP_sysfileread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileread", (RTS_UINTPTR)sysfileread, 1, RTSITF_GET_SIGNATURE(0, 0xD962D4F9), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfileread
	#define EXT_SysFilesysfileread
	#define GET_SysFilesysfileread  ERR_OK
	#define CAL_SysFilesysfileread  sysfileread
	#define CHK_SysFilesysfileread  TRUE
	#define EXP_SysFilesysfileread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileread", (RTS_UINTPTR)sysfileread, 1, RTSITF_GET_SIGNATURE(0, 0xD962D4F9), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfileread
	#define EXT_sysfileread
	#define GET_sysfileread(fl)  CAL_CMGETAPI( "sysfileread" ) 
	#define CAL_sysfileread  sysfileread
	#define CHK_sysfileread  TRUE
	#define EXP_sysfileread  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileread", (RTS_UINTPTR)sysfileread, 1, RTSITF_GET_SIGNATURE(0, 0xD962D4F9), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfileread  PFSYSFILEREAD_IEC pfsysfileread;
	#define EXT_sysfileread  extern PFSYSFILEREAD_IEC pfsysfileread;
	#define GET_sysfileread(fl)  s_pfCMGetAPI2( "sysfileread", (RTS_VOID_FCTPTR *)&pfsysfileread, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xD962D4F9), 0x03051100)
	#define CAL_sysfileread  pfsysfileread
	#define CHK_sysfileread  (pfsysfileread != NULL)
	#define EXP_sysfileread   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfileread", (RTS_UINTPTR)sysfileread, 1, RTSITF_GET_SIGNATURE(0, 0xD962D4F9), 0x03051100) 
#endif


/**
 * | Rename the file.
 * | A standard path will be added to the filename, if no path is specified.
 * :return: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysfilerename_struct
{
	RTS_IEC_STRING *szOldFileName;		/* VAR_INPUT */	/* Old file name. File name can contain an absolute or relative path to the file.
 Path entries must be separated with a Slash (/)  and not with a Backslash (\\)! */
	RTS_IEC_STRING *szNewFileName;		/* VAR_INPUT */	/* New file name. File name can contain an absolute or relative path to the file.
 Path entries must be separated with a Slash (/)  and not with a Backslash (\\)! */
	RTS_IEC_RESULT SysFileRename;		/* VAR_OUTPUT */	
} sysfilerename_struct;

void CDECL CDECL_EXT sysfilerename(sysfilerename_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILERENAME_IEC) (sysfilerename_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILERENAME_NOTIMPLEMENTED)
	#define USE_sysfilerename
	#define EXT_sysfilerename
	#define GET_sysfilerename(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilerename(p0) 
	#define CHK_sysfilerename  FALSE
	#define EXP_sysfilerename  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilerename
	#define EXT_sysfilerename
	#define GET_sysfilerename(fl)  CAL_CMGETAPI( "sysfilerename" ) 
	#define CAL_sysfilerename  sysfilerename
	#define CHK_sysfilerename  TRUE
	#define EXP_sysfilerename  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilerename", (RTS_UINTPTR)sysfilerename, 1, RTSITF_GET_SIGNATURE(0, 0xF3B9A110), 0x03051100) 
	#define EXTREF_ITF_sysfilerename {(RTS_VOID_FCTPTR)sysfilerename, "sysfilerename", RTSITF_GET_SIGNATURE(0, 0xF3B9A110), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilerename
	#define EXT_sysfilerename
	#define GET_sysfilerename(fl)  CAL_CMGETAPI( "sysfilerename" ) 
	#define CAL_sysfilerename  sysfilerename
	#define CHK_sysfilerename  TRUE
	#define EXP_sysfilerename  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilerename", (RTS_UINTPTR)sysfilerename, 1, RTSITF_GET_SIGNATURE(0, 0xF3B9A110), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilerename
	#define EXT_SysFilesysfilerename
	#define GET_SysFilesysfilerename  ERR_OK
	#define CAL_SysFilesysfilerename  sysfilerename
	#define CHK_SysFilesysfilerename  TRUE
	#define EXP_SysFilesysfilerename  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilerename", (RTS_UINTPTR)sysfilerename, 1, RTSITF_GET_SIGNATURE(0, 0xF3B9A110), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilerename
	#define EXT_sysfilerename
	#define GET_sysfilerename(fl)  CAL_CMGETAPI( "sysfilerename" ) 
	#define CAL_sysfilerename  sysfilerename
	#define CHK_sysfilerename  TRUE
	#define EXP_sysfilerename  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilerename", (RTS_UINTPTR)sysfilerename, 1, RTSITF_GET_SIGNATURE(0, 0xF3B9A110), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilerename  PFSYSFILERENAME_IEC pfsysfilerename;
	#define EXT_sysfilerename  extern PFSYSFILERENAME_IEC pfsysfilerename;
	#define GET_sysfilerename(fl)  s_pfCMGetAPI2( "sysfilerename", (RTS_VOID_FCTPTR *)&pfsysfilerename, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xF3B9A110), 0x03051100)
	#define CAL_sysfilerename  pfsysfilerename
	#define CHK_sysfilerename  (pfsysfilerename != NULL)
	#define EXP_sysfilerename   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilerename", (RTS_UINTPTR)sysfilerename, 1, RTSITF_GET_SIGNATURE(0, 0xF3B9A110), 0x03051100) 
#endif


/**
 * Set the file pointer to the specified position
 * :return: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysfilesetpos_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_XWORD ulOffset;				/* VAR_INPUT */	/* Offset to set from the beginning of the file */
	RTS_IEC_RESULT SysFileSetPos;		/* VAR_OUTPUT */	
} sysfilesetpos_struct;

void CDECL CDECL_EXT sysfilesetpos(sysfilesetpos_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILESETPOS_IEC) (sysfilesetpos_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILESETPOS_NOTIMPLEMENTED)
	#define USE_sysfilesetpos
	#define EXT_sysfilesetpos
	#define GET_sysfilesetpos(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilesetpos(p0) 
	#define CHK_sysfilesetpos  FALSE
	#define EXP_sysfilesetpos  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilesetpos
	#define EXT_sysfilesetpos
	#define GET_sysfilesetpos(fl)  CAL_CMGETAPI( "sysfilesetpos" ) 
	#define CAL_sysfilesetpos  sysfilesetpos
	#define CHK_sysfilesetpos  TRUE
	#define EXP_sysfilesetpos  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilesetpos", (RTS_UINTPTR)sysfilesetpos, 1, RTSITF_GET_SIGNATURE(0, 0xFC7A70CE), 0x03051100) 
	#define EXTREF_ITF_sysfilesetpos {(RTS_VOID_FCTPTR)sysfilesetpos, "sysfilesetpos", RTSITF_GET_SIGNATURE(0, 0xFC7A70CE), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilesetpos
	#define EXT_sysfilesetpos
	#define GET_sysfilesetpos(fl)  CAL_CMGETAPI( "sysfilesetpos" ) 
	#define CAL_sysfilesetpos  sysfilesetpos
	#define CHK_sysfilesetpos  TRUE
	#define EXP_sysfilesetpos  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilesetpos", (RTS_UINTPTR)sysfilesetpos, 1, RTSITF_GET_SIGNATURE(0, 0xFC7A70CE), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilesetpos
	#define EXT_SysFilesysfilesetpos
	#define GET_SysFilesysfilesetpos  ERR_OK
	#define CAL_SysFilesysfilesetpos  sysfilesetpos
	#define CHK_SysFilesysfilesetpos  TRUE
	#define EXP_SysFilesysfilesetpos  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilesetpos", (RTS_UINTPTR)sysfilesetpos, 1, RTSITF_GET_SIGNATURE(0, 0xFC7A70CE), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilesetpos
	#define EXT_sysfilesetpos
	#define GET_sysfilesetpos(fl)  CAL_CMGETAPI( "sysfilesetpos" ) 
	#define CAL_sysfilesetpos  sysfilesetpos
	#define CHK_sysfilesetpos  TRUE
	#define EXP_sysfilesetpos  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilesetpos", (RTS_UINTPTR)sysfilesetpos, 1, RTSITF_GET_SIGNATURE(0, 0xFC7A70CE), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilesetpos  PFSYSFILESETPOS_IEC pfsysfilesetpos;
	#define EXT_sysfilesetpos  extern PFSYSFILESETPOS_IEC pfsysfilesetpos;
	#define GET_sysfilesetpos(fl)  s_pfCMGetAPI2( "sysfilesetpos", (RTS_VOID_FCTPTR *)&pfsysfilesetpos, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xFC7A70CE), 0x03051100)
	#define CAL_sysfilesetpos  pfsysfilesetpos
	#define CHK_sysfilesetpos  (pfsysfilesetpos != NULL)
	#define EXP_sysfilesetpos   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilesetpos", (RTS_UINTPTR)sysfilesetpos, 1, RTSITF_GET_SIGNATURE(0, 0xFC7A70CE), 0x03051100) 
#endif


/**
 * Set a new filesize. May be larger or smaller than current size.
 * :return: Returns the runtime system error code (see CmpErrors.library)
 */
typedef struct tagsysfiletruncate_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_XWORD ulSizeNew;			/* VAR_INPUT */	/* Size to set. */
	RTS_IEC_RESULT SysFileTruncate;		/* VAR_OUTPUT */	
} sysfiletruncate_struct;

void CDECL CDECL_EXT sysfiletruncate(sysfiletruncate_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILETRUNCATE_IEC) (sysfiletruncate_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILETRUNCATE_NOTIMPLEMENTED)
	#define USE_sysfiletruncate
	#define EXT_sysfiletruncate
	#define GET_sysfiletruncate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfiletruncate(p0) 
	#define CHK_sysfiletruncate  FALSE
	#define EXP_sysfiletruncate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfiletruncate
	#define EXT_sysfiletruncate
	#define GET_sysfiletruncate(fl)  CAL_CMGETAPI( "sysfiletruncate" ) 
	#define CAL_sysfiletruncate  sysfiletruncate
	#define CHK_sysfiletruncate  TRUE
	#define EXP_sysfiletruncate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiletruncate", (RTS_UINTPTR)sysfiletruncate, 1, RTSITF_GET_SIGNATURE(0, 0xF2C7CF6C), 0x03051100) 
	#define EXTREF_ITF_sysfiletruncate {(RTS_VOID_FCTPTR)sysfiletruncate, "sysfiletruncate", RTSITF_GET_SIGNATURE(0, 0xF2C7CF6C), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfiletruncate
	#define EXT_sysfiletruncate
	#define GET_sysfiletruncate(fl)  CAL_CMGETAPI( "sysfiletruncate" ) 
	#define CAL_sysfiletruncate  sysfiletruncate
	#define CHK_sysfiletruncate  TRUE
	#define EXP_sysfiletruncate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiletruncate", (RTS_UINTPTR)sysfiletruncate, 1, RTSITF_GET_SIGNATURE(0, 0xF2C7CF6C), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfiletruncate
	#define EXT_SysFilesysfiletruncate
	#define GET_SysFilesysfiletruncate  ERR_OK
	#define CAL_SysFilesysfiletruncate  sysfiletruncate
	#define CHK_SysFilesysfiletruncate  TRUE
	#define EXP_SysFilesysfiletruncate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiletruncate", (RTS_UINTPTR)sysfiletruncate, 1, RTSITF_GET_SIGNATURE(0, 0xF2C7CF6C), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfiletruncate
	#define EXT_sysfiletruncate
	#define GET_sysfiletruncate(fl)  CAL_CMGETAPI( "sysfiletruncate" ) 
	#define CAL_sysfiletruncate  sysfiletruncate
	#define CHK_sysfiletruncate  TRUE
	#define EXP_sysfiletruncate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiletruncate", (RTS_UINTPTR)sysfiletruncate, 1, RTSITF_GET_SIGNATURE(0, 0xF2C7CF6C), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfiletruncate  PFSYSFILETRUNCATE_IEC pfsysfiletruncate;
	#define EXT_sysfiletruncate  extern PFSYSFILETRUNCATE_IEC pfsysfiletruncate;
	#define GET_sysfiletruncate(fl)  s_pfCMGetAPI2( "sysfiletruncate", (RTS_VOID_FCTPTR *)&pfsysfiletruncate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xF2C7CF6C), 0x03051100)
	#define CAL_sysfiletruncate  pfsysfiletruncate
	#define CHK_sysfiletruncate  (pfsysfiletruncate != NULL)
	#define EXP_sysfiletruncate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfiletruncate", (RTS_UINTPTR)sysfiletruncate, 1, RTSITF_GET_SIGNATURE(0, 0xF2C7CF6C), 0x03051100) 
#endif


/**
 * Write number of bytes to the file. File must be opened with |AM_WRITE| or |AM_APPEND|.
 * :return: Number of bytes written to the file. 0=if failed
 */
typedef struct tagsysfilewrite_struct
{
	RTS_IEC_HANDLE hFile;				/* VAR_INPUT */	/* Handle of the file */
	RTS_IEC_BYTE *pbyBuffer;			/* VAR_INPUT */	/* Pointer to buffer with data to write to file */
	RTS_IEC_XWORD ulSize;				/* VAR_INPUT */	/* Number of bytes to write in the file. Must be less or equal the buffer size! */
	RTS_IEC_RESULT *pResult;			/* VAR_INPUT */	/* Pointer to runtime system error code (see CmpErrors.library) */
	RTS_IEC_XWORD SysFileWrite;			/* VAR_OUTPUT */	
} sysfilewrite_struct;

void CDECL CDECL_EXT sysfilewrite(sysfilewrite_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSFILEWRITE_IEC) (sysfilewrite_struct *p);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEWRITE_NOTIMPLEMENTED)
	#define USE_sysfilewrite
	#define EXT_sysfilewrite
	#define GET_sysfilewrite(fl)  ERR_NOTIMPLEMENTED
	#define CAL_sysfilewrite(p0) 
	#define CHK_sysfilewrite  FALSE
	#define EXP_sysfilewrite  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_sysfilewrite
	#define EXT_sysfilewrite
	#define GET_sysfilewrite(fl)  CAL_CMGETAPI( "sysfilewrite" ) 
	#define CAL_sysfilewrite  sysfilewrite
	#define CHK_sysfilewrite  TRUE
	#define EXP_sysfilewrite  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilewrite", (RTS_UINTPTR)sysfilewrite, 1, RTSITF_GET_SIGNATURE(0, 0x6AE210A7), 0x03051100) 
	#define EXTREF_ITF_sysfilewrite {(RTS_VOID_FCTPTR)sysfilewrite, "sysfilewrite", RTSITF_GET_SIGNATURE(0, 0x6AE210A7), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_sysfilewrite
	#define EXT_sysfilewrite
	#define GET_sysfilewrite(fl)  CAL_CMGETAPI( "sysfilewrite" ) 
	#define CAL_sysfilewrite  sysfilewrite
	#define CHK_sysfilewrite  TRUE
	#define EXP_sysfilewrite  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilewrite", (RTS_UINTPTR)sysfilewrite, 1, RTSITF_GET_SIGNATURE(0, 0x6AE210A7), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFilesysfilewrite
	#define EXT_SysFilesysfilewrite
	#define GET_SysFilesysfilewrite  ERR_OK
	#define CAL_SysFilesysfilewrite  sysfilewrite
	#define CHK_SysFilesysfilewrite  TRUE
	#define EXP_SysFilesysfilewrite  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilewrite", (RTS_UINTPTR)sysfilewrite, 1, RTSITF_GET_SIGNATURE(0, 0x6AE210A7), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_sysfilewrite
	#define EXT_sysfilewrite
	#define GET_sysfilewrite(fl)  CAL_CMGETAPI( "sysfilewrite" ) 
	#define CAL_sysfilewrite  sysfilewrite
	#define CHK_sysfilewrite  TRUE
	#define EXP_sysfilewrite  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilewrite", (RTS_UINTPTR)sysfilewrite, 1, RTSITF_GET_SIGNATURE(0, 0x6AE210A7), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_sysfilewrite  PFSYSFILEWRITE_IEC pfsysfilewrite;
	#define EXT_sysfilewrite  extern PFSYSFILEWRITE_IEC pfsysfilewrite;
	#define GET_sysfilewrite(fl)  s_pfCMGetAPI2( "sysfilewrite", (RTS_VOID_FCTPTR *)&pfsysfilewrite, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x6AE210A7), 0x03051100)
	#define CAL_sysfilewrite  pfsysfilewrite
	#define CHK_sysfilewrite  (pfsysfilewrite != NULL)
	#define EXP_sysfilewrite   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"sysfilewrite", (RTS_UINTPTR)sysfilewrite, 1, RTSITF_GET_SIGNATURE(0, 0x6AE210A7), 0x03051100) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


/**
 * <category>Path options</category>
 * <description>
 * Separation of directory or file for SysFileGetPath2, SysFileGetFullPath2, SysFileGetIecPath2
 * </description>
 */
#define SYSFILE_PATH_OPTION_UNKNOWN 0
#define SYSFILE_PATH_OPTION_FILE 1
#define SYSFILE_PATH_OPTION_DIRECTORY 2
#define SYSFILE_PATH_OPTION_FILE_KEEP_NAME 3


/**
 * <category>Event parameter</category>
 * <element name="hFile" type="IN">Handle of the file</element>
 * <element name="szPath" type="IN">Full path name of the file</element>
 * <element name="am " type="IN">Access mode for open, see RTS_ACCESS_MODE</element>
 */
typedef struct
{
	RTS_HANDLE hFile;
	char *pszPath;
	RTS_ACCESS_MODE am;
} EVTPARAM_SysFileOpen;
#define EVTPARAMID_SysFileOpen						0x0001
#define EVTVERSION_SysFileOpen						0x0001

/**
 * <category>Event parameter</category>
 * <element name="hFile" type="IN">Handle of the file</element>
 */
typedef struct
{
	RTS_HANDLE hFile;
} EVTPARAM_SysFileClose;
#define EVTPARAMID_SysFileClose						0x0002
#define EVTVERSION_SysFileClose						0x0001

/**
 * <category>Event parameter</category>
 * <element name="szFileName" type="OUT">name of founded file</element>
 * <element name="byIndex" type="IN/OUT">index of file in file mapping list</element>
 */
 #define MAX_FILENAME 32
typedef struct
{
	char * pbyExecute;
  int * pintIndex;
}EVTPARAM_SysFileFlashGetFileMapIndex;
#define EVTPARAMID_SysFileFlashGetFileMapIndex					0x0003
#define EVTVERSION_SysFileFlashGetFileMapIndex					0x0001


/**
 * <category>Events</category>
 * <description>Event is sent when a file is opened. It is sent directly after the file was opened.
 * Note: The event is set only with SysFile, it is not set with SysFileFlash.
 * Note: Event set after CH_INIT2. There is no notification of opening a file before this hook.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysFileOpen</param>
 */
#define EVT_SysFileOpen					MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Events</category>
 * <description>Event is sent when a file is closed. It is send directly before the file is closed (file handle still valid).
 * Note: The event is set only with SysFile, it is not set with SysFileFlash.
 * Note: Event set after CH_INIT2. There is no notification of opening a file before this hook.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysFileClose</param>
 */
#define EVT_SysFileClose					MAKE_EVENTID(EVTCLASS_INFO, 2)

/**
 * <category>Events</category>
 * <description>The event is sent if the function GetFileMapIndex is called
 *	NOTE:
 *	This event is fired for every call and every loop call of count of files
 * </description>
 * <param name="pEventParam" type="IN/OUT">EVTPARAM_SysFileFlashGetFileMapIndex</param>
 */
#define EVT_SysFileFlashGetFileMapIndex					MAKE_EVENTID(EVTCLASS_INFO, 3)

/**
 * <category>File info</category>
 * <description>
 *	<element name="pszName" type="IN">Name of the file</element>
 *	<element name="am" type="IN">Access mode. See corresponding category</element>
 *	<element name="status" type="IN">File status. See corresponding category</element>
 *	<element name="hOSFile" type="IN">Operating system file handle</element>
 * </description>
 */
typedef struct SYS_FILE_INFO_
{
	char szName[MAX_PATH_LEN];
	RTS_ACCESS_MODE am;
	SYS_FILE_STATUS status;
	RTS_HANDLE hOSFile;
} SYS_FILE_INFO;


/* Backward compatibility */
#define ACCESS_MODE_AM_READ				RTS_ACCESS_MODE_AM_READ
#define ACCESS_MODE_AM_WRITE			RTS_ACCESS_MODE_AM_WRITE
#define ACCESS_MODE_AM_APPEND			RTS_ACCESS_MODE_AM_APPEND
#define ACCESS_MODE_AM_READ_PLUS		RTS_ACCESS_MODE_AM_READ_PLUS
#define ACCESS_MODE_AM_WRITE_PLUS		RTS_ACCESS_MODE_AM_WRITE_PLUS
#define ACCESS_MODE_AM_APPEND_PLUS		RTS_ACCESS_MODE_AM_APPEND_PLUS
#define AM_READ							ACCESS_MODE_AM_READ
#define AM_WRITE						ACCESS_MODE_AM_WRITE
#define AM_APPEND						ACCESS_MODE_AM_APPEND
#define AM_READ_PLUS					ACCESS_MODE_AM_READ_PLUS
#define AM_WRITE_PLUS					ACCESS_MODE_AM_WRITE_PLUS
#define AM_APPEND_PLUS					ACCESS_MODE_AM_APPEND_PLUS

#define FS_OK							SYS_FILE_STATUS_FS_OK
#define FS_NO_FILE						SYS_FILE_STATUS_FS_NO_FILE
#define FS_ILLEGALPOS					SYS_FILE_STATUS_FS_ILLEGALPOS
#define FS_FULL							SYS_FILE_STATUS_FS_FULL

/**
 * <category>Options</category>
 * <description>
 * Flags for SysFileGetNextConfiguredPath2
 *	<element name="SYSFILE_BIT_VIEWALL" type="IN">Overrides the setting PlaceholderFilePath.x.View to view all placeholders.</element>
 *	<element name="SYSFILE_BIT_VIRTUAL" type="IN">In case the placeholder has a virtual parent return this instead of the resolved path.</element>
 * </description>
 */
#define SYSFILE_BIT_VIEWALL 0x01
#define SYSFILE_BIT_VIRTUAL 0x02

#ifdef __cplusplus
extern "C" {
#endif

/* Init routines for OS specific modules */
RTS_RESULT CDECL SysFileOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysFileOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/**
 * <description>
 *	<p>Open or create file. A standard path will be added to the filename, if no path is specified in the
 *	file name.</p>
 *	<p>If a file extension is specified in the settings, this path will be used (see category settings).</p>
 *	<p>IMPLEMENTATION NOTE: File name can contain an absolute or relative path to the file. Path entries
 *	must be separated with a '/' and not with a '\"!</p>
 * </description>
 * <param name="pszFile" type="IN">File name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <param name="am" type="IN"><p>Requested access mode to the file:</p>
 *	<ul>
 *		<li>AM_READ: If file does not exist, an error is returned. If the file exists, the file 
 *					will be opened</li>
 *		<li>AM_WRITE: If file does not exist, a new file will be created. If the file exists,
 *					it will be overwritten!</li>
 *		<li>AM_APPEND: If the file does not exist, an error is returned. If the file exists, the file 
 *					will be opened</li>
 *	</ul>
 * </param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">File could be opened</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">Filename is empty or file not available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_OPERATION_DENIED">File access is denied (out of configured path)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FILE_ERROR">File access is denied (file is write protected)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Error occurred during write operation</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No heap memory to finish operation</errorcode>
 * <result>Handle to the file or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL SysFileOpen(char *pszFile, RTS_ACCESS_MODE am, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSFILEOPEN) (char *pszFile, RTS_ACCESS_MODE am, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEOPEN_NOTIMPLEMENTED)
	#define USE_SysFileOpen
	#define EXT_SysFileOpen
	#define GET_SysFileOpen(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileOpen(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysFileOpen  FALSE
	#define EXP_SysFileOpen  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileOpen
	#define EXT_SysFileOpen
	#define GET_SysFileOpen(fl)  CAL_CMGETAPI( "SysFileOpen" ) 
	#define CAL_SysFileOpen  SysFileOpen
	#define CHK_SysFileOpen  TRUE
	#define EXP_SysFileOpen  CAL_CMEXPAPI( "SysFileOpen" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileOpen
	#define EXT_SysFileOpen
	#define GET_SysFileOpen(fl)  CAL_CMGETAPI( "SysFileOpen" ) 
	#define CAL_SysFileOpen  SysFileOpen
	#define CHK_SysFileOpen  TRUE
	#define EXP_SysFileOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileOpen", (RTS_UINTPTR)SysFileOpen, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileOpen
	#define EXT_SysFileSysFileOpen
	#define GET_SysFileSysFileOpen  ERR_OK
	#define CAL_SysFileSysFileOpen  ((ISysFile*)s_pfCMCreateInstance(CLASSID_CSysFile, NULL))->ISysFileOpen
	#define CHK_SysFileSysFileOpen	(s_pfCMCreateInstance != NULL && pISysFile != NULL)
	#define EXP_SysFileSysFileOpen  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileOpen
	#define EXT_SysFileOpen
	#define GET_SysFileOpen(fl)  CAL_CMGETAPI( "SysFileOpen" ) 
	#define CAL_SysFileOpen  ((ISysFile*)s_pfCMCreateInstance(CLASSID_CSysFile, NULL))->ISysFileOpen
	#define CHK_SysFileOpen	(s_pfCMCreateInstance != NULL && pISysFile != NULL)
	#define EXP_SysFileOpen  CAL_CMEXPAPI( "SysFileOpen" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileOpen  PFSYSFILEOPEN pfSysFileOpen;
	#define EXT_SysFileOpen  extern PFSYSFILEOPEN pfSysFileOpen;
	#define GET_SysFileOpen(fl)  s_pfCMGetAPI2( "SysFileOpen", (RTS_VOID_FCTPTR *)&pfSysFileOpen, (fl), 0, 0)
	#define CAL_SysFileOpen  pfSysFileOpen
	#define CHK_SysFileOpen  (pfSysFileOpen != NULL)
	#define EXP_SysFileOpen  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileOpen", (RTS_UINTPTR)SysFileOpen, 0, 0) 
#endif




/**
 * <description>Platform specific implementation:
 *	Open or create a file. File will be opened with no standard path. The file name will be used as it is.
 *
 *	IMPLEMENTATION NOTE:
 *	The platform specific implementation should be POSIX compatible, but the behavior depends on the operating system
 *  and/or the used file system!
 * </description>
 * <param name="pszFile" type="IN">File name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <param name="am" type="IN"><p>Requested access mode to the file:</p>
 *	<ul>
 *		<li>AM_READ: If file does not exist, an error is returned. If the file exists, the file 
 *					will be opened</li>
 *		<li>AM_WRITE: If file does not exist, a new file will be created. If the file exists,
 *					it will be overwritten!</li>
 *		<li>AM_APPEND: If the file does not exist, an error is returned. If the file exists, the file 
 *					will be opened</li>
 *	</ul>
 * </param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">File could be opened</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">Filename is empty or file not available</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_OPERATION_DENIED">File access is denied: out of configured path</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FILE_ERROR">File access is denied: file is write protected</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Error occurred during write operation</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">No heap memory to finish operation</errorcode>
 * <result>Handle to the file or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL SysFileOpen_(char *pszFile, RTS_ACCESS_MODE am, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSFILEOPEN_) (char *pszFile, RTS_ACCESS_MODE am, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEOPEN__NOTIMPLEMENTED)
	#define USE_SysFileOpen_
	#define EXT_SysFileOpen_
	#define GET_SysFileOpen_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileOpen_(p0,p1,p2)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysFileOpen_  FALSE
	#define EXP_SysFileOpen_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileOpen_
	#define EXT_SysFileOpen_
	#define GET_SysFileOpen_(fl)  CAL_CMGETAPI( "SysFileOpen_" ) 
	#define CAL_SysFileOpen_  SysFileOpen_
	#define CHK_SysFileOpen_  TRUE
	#define EXP_SysFileOpen_  CAL_CMEXPAPI( "SysFileOpen_" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileOpen_
	#define EXT_SysFileOpen_
	#define GET_SysFileOpen_(fl)  CAL_CMGETAPI( "SysFileOpen_" ) 
	#define CAL_SysFileOpen_  SysFileOpen_
	#define CHK_SysFileOpen_  TRUE
	#define EXP_SysFileOpen_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileOpen_", (RTS_UINTPTR)SysFileOpen_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileOpen_
	#define EXT_SysFileSysFileOpen_
	#define GET_SysFileSysFileOpen_  ERR_OK
	#define CAL_SysFileSysFileOpen_ pISysFile->ISysFileOpen_
	#define CHK_SysFileSysFileOpen_ (pISysFile != NULL)
	#define EXP_SysFileSysFileOpen_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileOpen_
	#define EXT_SysFileOpen_
	#define GET_SysFileOpen_(fl)  CAL_CMGETAPI( "SysFileOpen_" ) 
	#define CAL_SysFileOpen_ pISysFile->ISysFileOpen_
	#define CHK_SysFileOpen_ (pISysFile != NULL)
	#define EXP_SysFileOpen_  CAL_CMEXPAPI( "SysFileOpen_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileOpen_  PFSYSFILEOPEN_ pfSysFileOpen_;
	#define EXT_SysFileOpen_  extern PFSYSFILEOPEN_ pfSysFileOpen_;
	#define GET_SysFileOpen_(fl)  s_pfCMGetAPI2( "SysFileOpen_", (RTS_VOID_FCTPTR *)&pfSysFileOpen_, (fl), 0, 0)
	#define CAL_SysFileOpen_  pfSysFileOpen_
	#define CHK_SysFileOpen_  (pfSysFileOpen_ != NULL)
	#define EXP_SysFileOpen_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileOpen_", (RTS_UINTPTR)SysFileOpen_, 0, 0) 
#endif




/**
 * <description>Close a file specified by handle</description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileClose(RTS_HANDLE hFile);
typedef RTS_RESULT (CDECL * PFSYSFILECLOSE) (RTS_HANDLE hFile);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILECLOSE_NOTIMPLEMENTED)
	#define USE_SysFileClose
	#define EXT_SysFileClose
	#define GET_SysFileClose(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileClose(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileClose  FALSE
	#define EXP_SysFileClose  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileClose
	#define EXT_SysFileClose
	#define GET_SysFileClose(fl)  CAL_CMGETAPI( "SysFileClose" ) 
	#define CAL_SysFileClose  SysFileClose
	#define CHK_SysFileClose  TRUE
	#define EXP_SysFileClose  CAL_CMEXPAPI( "SysFileClose" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileClose
	#define EXT_SysFileClose
	#define GET_SysFileClose(fl)  CAL_CMGETAPI( "SysFileClose" ) 
	#define CAL_SysFileClose  SysFileClose
	#define CHK_SysFileClose  TRUE
	#define EXP_SysFileClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileClose", (RTS_UINTPTR)SysFileClose, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileClose
	#define EXT_SysFileSysFileClose
	#define GET_SysFileSysFileClose  ERR_OK
	#define CAL_SysFileSysFileClose(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysFile*)p0)->ISysFileClose())
	#define CHK_SysFileSysFileClose  TRUE
	#define EXP_SysFileSysFileClose  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileClose
	#define EXT_SysFileClose
	#define GET_SysFileClose(fl)  CAL_CMGETAPI( "SysFileClose" ) 
	#define CAL_SysFileClose(p0) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysFile*)p0)->ISysFileClose())
	#define CHK_SysFileClose  TRUE
	#define EXP_SysFileClose  CAL_CMEXPAPI( "SysFileClose" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileClose  PFSYSFILECLOSE pfSysFileClose;
	#define EXT_SysFileClose  extern PFSYSFILECLOSE pfSysFileClose;
	#define GET_SysFileClose(fl)  s_pfCMGetAPI2( "SysFileClose", (RTS_VOID_FCTPTR *)&pfSysFileClose, (fl), 0, 0)
	#define CAL_SysFileClose  pfSysFileClose
	#define CHK_SysFileClose  (pfSysFileClose != NULL)
	#define EXP_SysFileClose  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileClose", (RTS_UINTPTR)SysFileClose, 0, 0) 
#endif




/**
 * <description>Close a file specified by handle. System specific version of the function</description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileClose_(RTS_HANDLE hFile);
typedef RTS_RESULT (CDECL * PFSYSFILECLOSE_) (RTS_HANDLE hFile);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILECLOSE__NOTIMPLEMENTED)
	#define USE_SysFileClose_
	#define EXT_SysFileClose_
	#define GET_SysFileClose_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileClose_(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileClose_  FALSE
	#define EXP_SysFileClose_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileClose_
	#define EXT_SysFileClose_
	#define GET_SysFileClose_(fl)  CAL_CMGETAPI( "SysFileClose_" ) 
	#define CAL_SysFileClose_  SysFileClose_
	#define CHK_SysFileClose_  TRUE
	#define EXP_SysFileClose_  CAL_CMEXPAPI( "SysFileClose_" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileClose_
	#define EXT_SysFileClose_
	#define GET_SysFileClose_(fl)  CAL_CMGETAPI( "SysFileClose_" ) 
	#define CAL_SysFileClose_  SysFileClose_
	#define CHK_SysFileClose_  TRUE
	#define EXP_SysFileClose_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileClose_", (RTS_UINTPTR)SysFileClose_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileClose_
	#define EXT_SysFileSysFileClose_
	#define GET_SysFileSysFileClose_  ERR_OK
	#define CAL_SysFileSysFileClose_ pISysFile->ISysFileClose_
	#define CHK_SysFileSysFileClose_ (pISysFile != NULL)
	#define EXP_SysFileSysFileClose_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileClose_
	#define EXT_SysFileClose_
	#define GET_SysFileClose_(fl)  CAL_CMGETAPI( "SysFileClose_" ) 
	#define CAL_SysFileClose_ pISysFile->ISysFileClose_
	#define CHK_SysFileClose_ (pISysFile != NULL)
	#define EXP_SysFileClose_  CAL_CMEXPAPI( "SysFileClose_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileClose_  PFSYSFILECLOSE_ pfSysFileClose_;
	#define EXT_SysFileClose_  extern PFSYSFILECLOSE_ pfSysFileClose_;
	#define GET_SysFileClose_(fl)  s_pfCMGetAPI2( "SysFileClose_", (RTS_VOID_FCTPTR *)&pfSysFileClose_, (fl), 0, 0)
	#define CAL_SysFileClose_  pfSysFileClose_
	#define CHK_SysFileClose_  (pfSysFileClose_ != NULL)
	#define EXP_SysFileClose_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileClose_", (RTS_UINTPTR)SysFileClose_, 0, 0) 
#endif




/**
 * <description>Get the file status</description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <param name="pResult" type="IN">Pointer to error code</param>
 * <result>File status. See category file status</result>
 */
SYS_FILE_STATUS CDECL SysFileGetStatus(RTS_HANDLE hFile, RTS_RESULT *pResult);
typedef SYS_FILE_STATUS (CDECL * PFSYSFILEGETSTATUS) (RTS_HANDLE hFile, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETSTATUS_NOTIMPLEMENTED)
	#define USE_SysFileGetStatus
	#define EXT_SysFileGetStatus
	#define GET_SysFileGetStatus(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetStatus(p0,p1)  (SYS_FILE_STATUS)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetStatus  FALSE
	#define EXP_SysFileGetStatus  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetStatus
	#define EXT_SysFileGetStatus
	#define GET_SysFileGetStatus(fl)  CAL_CMGETAPI( "SysFileGetStatus" ) 
	#define CAL_SysFileGetStatus  SysFileGetStatus
	#define CHK_SysFileGetStatus  TRUE
	#define EXP_SysFileGetStatus  CAL_CMEXPAPI( "SysFileGetStatus" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetStatus
	#define EXT_SysFileGetStatus
	#define GET_SysFileGetStatus(fl)  CAL_CMGETAPI( "SysFileGetStatus" ) 
	#define CAL_SysFileGetStatus  SysFileGetStatus
	#define CHK_SysFileGetStatus  TRUE
	#define EXP_SysFileGetStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetStatus", (RTS_UINTPTR)SysFileGetStatus, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetStatus
	#define EXT_SysFileSysFileGetStatus
	#define GET_SysFileSysFileGetStatus  ERR_OK
	#define CAL_SysFileSysFileGetStatus(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileGetStatus(p1) : ((ISysFile*)p0)->ISysFileGetStatus(p1))
	#define CHK_SysFileSysFileGetStatus  (pISysFile != NULL)
	#define EXP_SysFileSysFileGetStatus  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetStatus
	#define EXT_SysFileGetStatus
	#define GET_SysFileGetStatus(fl)  CAL_CMGETAPI( "SysFileGetStatus" ) 
	#define CAL_SysFileGetStatus(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileGetStatus(p1) : ((ISysFile*)p0)->ISysFileGetStatus(p1))
	#define CHK_SysFileGetStatus  (pISysFile != NULL)
	#define EXP_SysFileGetStatus  CAL_CMEXPAPI( "SysFileGetStatus" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetStatus  PFSYSFILEGETSTATUS pfSysFileGetStatus;
	#define EXT_SysFileGetStatus  extern PFSYSFILEGETSTATUS pfSysFileGetStatus;
	#define GET_SysFileGetStatus(fl)  s_pfCMGetAPI2( "SysFileGetStatus", (RTS_VOID_FCTPTR *)&pfSysFileGetStatus, (fl), 0, 0)
	#define CAL_SysFileGetStatus  pfSysFileGetStatus
	#define CHK_SysFileGetStatus  (pfSysFileGetStatus != NULL)
	#define EXP_SysFileGetStatus  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetStatus", (RTS_UINTPTR)SysFileGetStatus, 0, 0) 
#endif




/**
 * <description>Get the file name from file specified by handle</description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <param name="pResult" type="IN">Pointer to error code</param>
 * <result>File name</result>
 */
char* CDECL SysFileGetName(RTS_HANDLE hFile, RTS_RESULT *pResult);
typedef char* (CDECL * PFSYSFILEGETNAME) (RTS_HANDLE hFile, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETNAME_NOTIMPLEMENTED)
	#define USE_SysFileGetName
	#define EXT_SysFileGetName
	#define GET_SysFileGetName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetName(p0,p1)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetName  FALSE
	#define EXP_SysFileGetName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetName
	#define EXT_SysFileGetName
	#define GET_SysFileGetName(fl)  CAL_CMGETAPI( "SysFileGetName" ) 
	#define CAL_SysFileGetName  SysFileGetName
	#define CHK_SysFileGetName  TRUE
	#define EXP_SysFileGetName  CAL_CMEXPAPI( "SysFileGetName" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetName
	#define EXT_SysFileGetName
	#define GET_SysFileGetName(fl)  CAL_CMGETAPI( "SysFileGetName" ) 
	#define CAL_SysFileGetName  SysFileGetName
	#define CHK_SysFileGetName  TRUE
	#define EXP_SysFileGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetName", (RTS_UINTPTR)SysFileGetName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetName
	#define EXT_SysFileSysFileGetName
	#define GET_SysFileSysFileGetName  ERR_OK
	#define CAL_SysFileSysFileGetName(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileGetName(p1) : ((ISysFile*)p0)->ISysFileGetName(p1))
	#define CHK_SysFileSysFileGetName  (pISysFile != NULL)
	#define EXP_SysFileSysFileGetName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetName
	#define EXT_SysFileGetName
	#define GET_SysFileGetName(fl)  CAL_CMGETAPI( "SysFileGetName" ) 
	#define CAL_SysFileGetName(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileGetName(p1) : ((ISysFile*)p0)->ISysFileGetName(p1))
	#define CHK_SysFileGetName  (pISysFile != NULL)
	#define EXP_SysFileGetName  CAL_CMEXPAPI( "SysFileGetName" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetName  PFSYSFILEGETNAME pfSysFileGetName;
	#define EXT_SysFileGetName  extern PFSYSFILEGETNAME pfSysFileGetName;
	#define GET_SysFileGetName(fl)  s_pfCMGetAPI2( "SysFileGetName", (RTS_VOID_FCTPTR *)&pfSysFileGetName, (fl), 0, 0)
	#define CAL_SysFileGetName  pfSysFileGetName
	#define CHK_SysFileGetName  (pfSysFileGetName != NULL)
	#define EXP_SysFileGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetName", (RTS_UINTPTR)SysFileGetName, 0, 0) 
#endif




/**
 * <description>
 *	Read number of bytes out of the file.
 *  Independent of the size, that was read (even 0), the function returns ERR_OK.
 *  Only when there was an internal error when accessing the file, it will return ERR_FAILED.
 * </description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <param name="pbyBuffer" type="OUT">Pointer to buffer for read data</param>
 * <param name="uiSize" type="IN">Number of bytes to read from file. Must be less or equal the buffer size!</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Buffer could be read</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Internal error when reading the file</errorcode>
 * <result>Number of bytes read from file. 0=if failed</result>
 */
RTS_SIZE CDECL SysFileRead(RTS_HANDLE hFile, unsigned char *pbyBuffer, RTS_SIZE uiSize, RTS_RESULT *pResult);
typedef RTS_SIZE (CDECL * PFSYSFILEREAD) (RTS_HANDLE hFile, unsigned char *pbyBuffer, RTS_SIZE uiSize, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEREAD_NOTIMPLEMENTED)
	#define USE_SysFileRead
	#define EXT_SysFileRead
	#define GET_SysFileRead(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileRead(p0,p1,p2,p3)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysFileRead  FALSE
	#define EXP_SysFileRead  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileRead
	#define EXT_SysFileRead
	#define GET_SysFileRead(fl)  CAL_CMGETAPI( "SysFileRead" ) 
	#define CAL_SysFileRead  SysFileRead
	#define CHK_SysFileRead  TRUE
	#define EXP_SysFileRead  CAL_CMEXPAPI( "SysFileRead" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileRead
	#define EXT_SysFileRead
	#define GET_SysFileRead(fl)  CAL_CMGETAPI( "SysFileRead" ) 
	#define CAL_SysFileRead  SysFileRead
	#define CHK_SysFileRead  TRUE
	#define EXP_SysFileRead  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileRead", (RTS_UINTPTR)SysFileRead, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileRead
	#define EXT_SysFileSysFileRead
	#define GET_SysFileSysFileRead  ERR_OK
	#define CAL_SysFileSysFileRead(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileRead(p1,p2,p3) : ((ISysFile*)p0)->ISysFileRead(p1,p2,p3))
	#define CHK_SysFileSysFileRead  (pISysFile != NULL)
	#define EXP_SysFileSysFileRead  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileRead
	#define EXT_SysFileRead
	#define GET_SysFileRead(fl)  CAL_CMGETAPI( "SysFileRead" ) 
	#define CAL_SysFileRead(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileRead(p1,p2,p3) : ((ISysFile*)p0)->ISysFileRead(p1,p2,p3))
	#define CHK_SysFileRead  (pISysFile != NULL)
	#define EXP_SysFileRead  CAL_CMEXPAPI( "SysFileRead" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileRead  PFSYSFILEREAD pfSysFileRead;
	#define EXT_SysFileRead  extern PFSYSFILEREAD pfSysFileRead;
	#define GET_SysFileRead(fl)  s_pfCMGetAPI2( "SysFileRead", (RTS_VOID_FCTPTR *)&pfSysFileRead, (fl), 0, 0)
	#define CAL_SysFileRead  pfSysFileRead
	#define CHK_SysFileRead  (pfSysFileRead != NULL)
	#define EXP_SysFileRead  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileRead", (RTS_UINTPTR)SysFileRead, 0, 0) 
#endif




/**
 * <description>
 *	Write number of bytes to the file. File must be opened with AM_WRITE or AM_APPEND.
 * </description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <param name="pbyBuffer" type="IN">Pointer to buffer with data to write to file</param>
 * <param name="uiSize" type="IN">Number of bytes to write in the file. Must be less or equal the buffer size!</param>
 * <param name="pResult" type="OUT">Pointer to error code </param>
 * <errorcode name="RTS_RESULT" type="ERR_OK">File could be written</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter (handle invalid, buffer pointer NULL or buffer size 0)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Error occurred during writing</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DISK_FULL">Disk is full so writing failed</errorcode>
 * <result>Number of bytes written to the file. 0=if failed</result>
 */
RTS_SIZE CDECL SysFileWrite(RTS_HANDLE hFile, unsigned char *pbyBuffer, RTS_SIZE uiSize, RTS_RESULT *pResult);
typedef RTS_SIZE (CDECL * PFSYSFILEWRITE) (RTS_HANDLE hFile, unsigned char *pbyBuffer, RTS_SIZE uiSize, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEWRITE_NOTIMPLEMENTED)
	#define USE_SysFileWrite
	#define EXT_SysFileWrite
	#define GET_SysFileWrite(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileWrite(p0,p1,p2,p3)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysFileWrite  FALSE
	#define EXP_SysFileWrite  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileWrite
	#define EXT_SysFileWrite
	#define GET_SysFileWrite(fl)  CAL_CMGETAPI( "SysFileWrite" ) 
	#define CAL_SysFileWrite  SysFileWrite
	#define CHK_SysFileWrite  TRUE
	#define EXP_SysFileWrite  CAL_CMEXPAPI( "SysFileWrite" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileWrite
	#define EXT_SysFileWrite
	#define GET_SysFileWrite(fl)  CAL_CMGETAPI( "SysFileWrite" ) 
	#define CAL_SysFileWrite  SysFileWrite
	#define CHK_SysFileWrite  TRUE
	#define EXP_SysFileWrite  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileWrite", (RTS_UINTPTR)SysFileWrite, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileWrite
	#define EXT_SysFileSysFileWrite
	#define GET_SysFileSysFileWrite  ERR_OK
	#define CAL_SysFileSysFileWrite(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileWrite(p1,p2,p3) : ((ISysFile*)p0)->ISysFileWrite(p1,p2,p3))
	#define CHK_SysFileSysFileWrite  (pISysFile != NULL)
	#define EXP_SysFileSysFileWrite  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileWrite
	#define EXT_SysFileWrite
	#define GET_SysFileWrite(fl)  CAL_CMGETAPI( "SysFileWrite" ) 
	#define CAL_SysFileWrite(p0,p1,p2,p3)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileWrite(p1,p2,p3) : ((ISysFile*)p0)->ISysFileWrite(p1,p2,p3))
	#define CHK_SysFileWrite  (pISysFile != NULL)
	#define EXP_SysFileWrite  CAL_CMEXPAPI( "SysFileWrite" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileWrite  PFSYSFILEWRITE pfSysFileWrite;
	#define EXT_SysFileWrite  extern PFSYSFILEWRITE pfSysFileWrite;
	#define GET_SysFileWrite(fl)  s_pfCMGetAPI2( "SysFileWrite", (RTS_VOID_FCTPTR *)&pfSysFileWrite, (fl), 0, 0)
	#define CAL_SysFileWrite  pfSysFileWrite
	#define CHK_SysFileWrite  (pfSysFileWrite != NULL)
	#define EXP_SysFileWrite  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileWrite", (RTS_UINTPTR)SysFileWrite, 0, 0) 
#endif




/**
 * <description>
 *	Delete the file specified by name.
 *	A standard path will be added to the filename, if no path is specified.
 * </description>
 * <param name="pszFile" type="IN">File name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileDelete(char *pszFile);
typedef RTS_RESULT (CDECL * PFSYSFILEDELETE) (char *pszFile);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEDELETE_NOTIMPLEMENTED)
	#define USE_SysFileDelete
	#define EXT_SysFileDelete
	#define GET_SysFileDelete(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileDelete(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileDelete  FALSE
	#define EXP_SysFileDelete  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileDelete
	#define EXT_SysFileDelete
	#define GET_SysFileDelete(fl)  CAL_CMGETAPI( "SysFileDelete" ) 
	#define CAL_SysFileDelete  SysFileDelete
	#define CHK_SysFileDelete  TRUE
	#define EXP_SysFileDelete  CAL_CMEXPAPI( "SysFileDelete" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileDelete
	#define EXT_SysFileDelete
	#define GET_SysFileDelete(fl)  CAL_CMGETAPI( "SysFileDelete" ) 
	#define CAL_SysFileDelete  SysFileDelete
	#define CHK_SysFileDelete  TRUE
	#define EXP_SysFileDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileDelete", (RTS_UINTPTR)SysFileDelete, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileDelete
	#define EXT_SysFileSysFileDelete
	#define GET_SysFileSysFileDelete  ERR_OK
	#define CAL_SysFileSysFileDelete pISysFile->ISysFileDelete
	#define CHK_SysFileSysFileDelete (pISysFile != NULL)
	#define EXP_SysFileSysFileDelete  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileDelete
	#define EXT_SysFileDelete
	#define GET_SysFileDelete(fl)  CAL_CMGETAPI( "SysFileDelete" ) 
	#define CAL_SysFileDelete pISysFile->ISysFileDelete
	#define CHK_SysFileDelete (pISysFile != NULL)
	#define EXP_SysFileDelete  CAL_CMEXPAPI( "SysFileDelete" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileDelete  PFSYSFILEDELETE pfSysFileDelete;
	#define EXT_SysFileDelete  extern PFSYSFILEDELETE pfSysFileDelete;
	#define GET_SysFileDelete(fl)  s_pfCMGetAPI2( "SysFileDelete", (RTS_VOID_FCTPTR *)&pfSysFileDelete, (fl), 0, 0)
	#define CAL_SysFileDelete  pfSysFileDelete
	#define CHK_SysFileDelete  (pfSysFileDelete != NULL)
	#define EXP_SysFileDelete  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileDelete", (RTS_UINTPTR)SysFileDelete, 0, 0) 
#endif




/**
 * <description>
 *	Delete the file specified by name. Filename will be used with no standard path.
 * </description>
 * <param name="pszFile" type="IN">File name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileDelete_(char *pszFile);
typedef RTS_RESULT (CDECL * PFSYSFILEDELETE_) (char *pszFile);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEDELETE__NOTIMPLEMENTED)
	#define USE_SysFileDelete_
	#define EXT_SysFileDelete_
	#define GET_SysFileDelete_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileDelete_(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileDelete_  FALSE
	#define EXP_SysFileDelete_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileDelete_
	#define EXT_SysFileDelete_
	#define GET_SysFileDelete_(fl)  CAL_CMGETAPI( "SysFileDelete_" ) 
	#define CAL_SysFileDelete_  SysFileDelete_
	#define CHK_SysFileDelete_  TRUE
	#define EXP_SysFileDelete_  CAL_CMEXPAPI( "SysFileDelete_" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileDelete_
	#define EXT_SysFileDelete_
	#define GET_SysFileDelete_(fl)  CAL_CMGETAPI( "SysFileDelete_" ) 
	#define CAL_SysFileDelete_  SysFileDelete_
	#define CHK_SysFileDelete_  TRUE
	#define EXP_SysFileDelete_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileDelete_", (RTS_UINTPTR)SysFileDelete_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileDelete_
	#define EXT_SysFileSysFileDelete_
	#define GET_SysFileSysFileDelete_  ERR_OK
	#define CAL_SysFileSysFileDelete_ pISysFile->ISysFileDelete_
	#define CHK_SysFileSysFileDelete_ (pISysFile != NULL)
	#define EXP_SysFileSysFileDelete_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileDelete_
	#define EXT_SysFileDelete_
	#define GET_SysFileDelete_(fl)  CAL_CMGETAPI( "SysFileDelete_" ) 
	#define CAL_SysFileDelete_ pISysFile->ISysFileDelete_
	#define CHK_SysFileDelete_ (pISysFile != NULL)
	#define EXP_SysFileDelete_  CAL_CMEXPAPI( "SysFileDelete_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileDelete_  PFSYSFILEDELETE_ pfSysFileDelete_;
	#define EXT_SysFileDelete_  extern PFSYSFILEDELETE_ pfSysFileDelete_;
	#define GET_SysFileDelete_(fl)  s_pfCMGetAPI2( "SysFileDelete_", (RTS_VOID_FCTPTR *)&pfSysFileDelete_, (fl), 0, 0)
	#define CAL_SysFileDelete_  pfSysFileDelete_
	#define CHK_SysFileDelete_  (pfSysFileDelete_ != NULL)
	#define EXP_SysFileDelete_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileDelete_", (RTS_UINTPTR)SysFileDelete_, 0, 0) 
#endif




/**
 * <description>
 *	Delete the file specified by handle
 * </description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileDeleteByHandle(RTS_HANDLE hFile);
typedef RTS_RESULT (CDECL * PFSYSFILEDELETEBYHANDLE) (RTS_HANDLE hFile);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEDELETEBYHANDLE_NOTIMPLEMENTED)
	#define USE_SysFileDeleteByHandle
	#define EXT_SysFileDeleteByHandle
	#define GET_SysFileDeleteByHandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileDeleteByHandle(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileDeleteByHandle  FALSE
	#define EXP_SysFileDeleteByHandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileDeleteByHandle
	#define EXT_SysFileDeleteByHandle
	#define GET_SysFileDeleteByHandle(fl)  CAL_CMGETAPI( "SysFileDeleteByHandle" ) 
	#define CAL_SysFileDeleteByHandle  SysFileDeleteByHandle
	#define CHK_SysFileDeleteByHandle  TRUE
	#define EXP_SysFileDeleteByHandle  CAL_CMEXPAPI( "SysFileDeleteByHandle" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileDeleteByHandle
	#define EXT_SysFileDeleteByHandle
	#define GET_SysFileDeleteByHandle(fl)  CAL_CMGETAPI( "SysFileDeleteByHandle" ) 
	#define CAL_SysFileDeleteByHandle  SysFileDeleteByHandle
	#define CHK_SysFileDeleteByHandle  TRUE
	#define EXP_SysFileDeleteByHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileDeleteByHandle", (RTS_UINTPTR)SysFileDeleteByHandle, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileDeleteByHandle
	#define EXT_SysFileSysFileDeleteByHandle
	#define GET_SysFileSysFileDeleteByHandle  ERR_OK
	#define CAL_SysFileSysFileDeleteByHandle(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileDeleteByHandle() : ((ISysFile*)p0)->ISysFileDeleteByHandle())
	#define CHK_SysFileSysFileDeleteByHandle  (pISysFile != NULL)
	#define EXP_SysFileSysFileDeleteByHandle  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileDeleteByHandle
	#define EXT_SysFileDeleteByHandle
	#define GET_SysFileDeleteByHandle(fl)  CAL_CMGETAPI( "SysFileDeleteByHandle" ) 
	#define CAL_SysFileDeleteByHandle(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileDeleteByHandle() : ((ISysFile*)p0)->ISysFileDeleteByHandle())
	#define CHK_SysFileDeleteByHandle  (pISysFile != NULL)
	#define EXP_SysFileDeleteByHandle  CAL_CMEXPAPI( "SysFileDeleteByHandle" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileDeleteByHandle  PFSYSFILEDELETEBYHANDLE pfSysFileDeleteByHandle;
	#define EXT_SysFileDeleteByHandle  extern PFSYSFILEDELETEBYHANDLE pfSysFileDeleteByHandle;
	#define GET_SysFileDeleteByHandle(fl)  s_pfCMGetAPI2( "SysFileDeleteByHandle", (RTS_VOID_FCTPTR *)&pfSysFileDeleteByHandle, (fl), 0, 0)
	#define CAL_SysFileDeleteByHandle  pfSysFileDeleteByHandle
	#define CHK_SysFileDeleteByHandle  (pfSysFileDeleteByHandle != NULL)
	#define EXP_SysFileDeleteByHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileDeleteByHandle", (RTS_UINTPTR)SysFileDeleteByHandle, 0, 0) 
#endif




/**
 * <description>
 *	Rename the file.
 *	A standard path will be added to the filename, if no path is specified.
 * </description>
 * <param name="pszOldFileName" type="IN">Old file name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <param name="pszNewFileName" type="IN">New file name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">File could be successfully renamed</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter (pointer NULL or length 0)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NO_OBJECT">Failed because old file does not exist</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_DUPLICATE">Failed because new file already exists</errorcode>
 */
RTS_RESULT CDECL SysFileRename(char *pszOldFileName, char *pszNewFileName);
typedef RTS_RESULT (CDECL * PFSYSFILERENAME) (char *pszOldFileName, char *pszNewFileName);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILERENAME_NOTIMPLEMENTED)
	#define USE_SysFileRename
	#define EXT_SysFileRename
	#define GET_SysFileRename(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileRename(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileRename  FALSE
	#define EXP_SysFileRename  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileRename
	#define EXT_SysFileRename
	#define GET_SysFileRename(fl)  CAL_CMGETAPI( "SysFileRename" ) 
	#define CAL_SysFileRename  SysFileRename
	#define CHK_SysFileRename  TRUE
	#define EXP_SysFileRename  CAL_CMEXPAPI( "SysFileRename" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileRename
	#define EXT_SysFileRename
	#define GET_SysFileRename(fl)  CAL_CMGETAPI( "SysFileRename" ) 
	#define CAL_SysFileRename  SysFileRename
	#define CHK_SysFileRename  TRUE
	#define EXP_SysFileRename  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileRename", (RTS_UINTPTR)SysFileRename, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileRename
	#define EXT_SysFileSysFileRename
	#define GET_SysFileSysFileRename  ERR_OK
	#define CAL_SysFileSysFileRename pISysFile->ISysFileRename
	#define CHK_SysFileSysFileRename (pISysFile != NULL)
	#define EXP_SysFileSysFileRename  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileRename
	#define EXT_SysFileRename
	#define GET_SysFileRename(fl)  CAL_CMGETAPI( "SysFileRename" ) 
	#define CAL_SysFileRename pISysFile->ISysFileRename
	#define CHK_SysFileRename (pISysFile != NULL)
	#define EXP_SysFileRename  CAL_CMEXPAPI( "SysFileRename" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileRename  PFSYSFILERENAME pfSysFileRename;
	#define EXT_SysFileRename  extern PFSYSFILERENAME pfSysFileRename;
	#define GET_SysFileRename(fl)  s_pfCMGetAPI2( "SysFileRename", (RTS_VOID_FCTPTR *)&pfSysFileRename, (fl), 0, 0)
	#define CAL_SysFileRename  pfSysFileRename
	#define CHK_SysFileRename  (pfSysFileRename != NULL)
	#define EXP_SysFileRename  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileRename", (RTS_UINTPTR)SysFileRename, 0, 0) 
#endif




/**
 * <description>
 *	Rename the file. File will be renamed with no standard path.
 * </description>
 * <param name="pszOldFileName" type="IN">Old file name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <param name="pszNewFileName" type="IN">New file name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileRename_(char *pszOldFileName, char *pszNewFileName);
typedef RTS_RESULT (CDECL * PFSYSFILERENAME_) (char *pszOldFileName, char *pszNewFileName);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILERENAME__NOTIMPLEMENTED)
	#define USE_SysFileRename_
	#define EXT_SysFileRename_
	#define GET_SysFileRename_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileRename_(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileRename_  FALSE
	#define EXP_SysFileRename_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileRename_
	#define EXT_SysFileRename_
	#define GET_SysFileRename_(fl)  CAL_CMGETAPI( "SysFileRename_" ) 
	#define CAL_SysFileRename_  SysFileRename_
	#define CHK_SysFileRename_  TRUE
	#define EXP_SysFileRename_  CAL_CMEXPAPI( "SysFileRename_" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileRename_
	#define EXT_SysFileRename_
	#define GET_SysFileRename_(fl)  CAL_CMGETAPI( "SysFileRename_" ) 
	#define CAL_SysFileRename_  SysFileRename_
	#define CHK_SysFileRename_  TRUE
	#define EXP_SysFileRename_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileRename_", (RTS_UINTPTR)SysFileRename_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileRename_
	#define EXT_SysFileSysFileRename_
	#define GET_SysFileSysFileRename_  ERR_OK
	#define CAL_SysFileSysFileRename_ pISysFile->ISysFileRename_
	#define CHK_SysFileSysFileRename_ (pISysFile != NULL)
	#define EXP_SysFileSysFileRename_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileRename_
	#define EXT_SysFileRename_
	#define GET_SysFileRename_(fl)  CAL_CMGETAPI( "SysFileRename_" ) 
	#define CAL_SysFileRename_ pISysFile->ISysFileRename_
	#define CHK_SysFileRename_ (pISysFile != NULL)
	#define EXP_SysFileRename_  CAL_CMEXPAPI( "SysFileRename_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileRename_  PFSYSFILERENAME_ pfSysFileRename_;
	#define EXT_SysFileRename_  extern PFSYSFILERENAME_ pfSysFileRename_;
	#define GET_SysFileRename_(fl)  s_pfCMGetAPI2( "SysFileRename_", (RTS_VOID_FCTPTR *)&pfSysFileRename_, (fl), 0, 0)
	#define CAL_SysFileRename_  pfSysFileRename_
	#define CHK_SysFileRename_  (pfSysFileRename_ != NULL)
	#define EXP_SysFileRename_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileRename_", (RTS_UINTPTR)SysFileRename_, 0, 0) 
#endif




/**
 * <description>
 *	Set the file pointer to the specified position
 * </description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <param name="uiOffset" type="IN">Offset to set from the beginning of the file</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileSetPos(RTS_HANDLE hFile, RTS_SIZE uiOffset);
typedef RTS_RESULT (CDECL * PFSYSFILESETPOS) (RTS_HANDLE hFile, RTS_SIZE uiOffset);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILESETPOS_NOTIMPLEMENTED)
	#define USE_SysFileSetPos
	#define EXT_SysFileSetPos
	#define GET_SysFileSetPos(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileSetPos(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileSetPos  FALSE
	#define EXP_SysFileSetPos  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileSetPos
	#define EXT_SysFileSetPos
	#define GET_SysFileSetPos(fl)  CAL_CMGETAPI( "SysFileSetPos" ) 
	#define CAL_SysFileSetPos  SysFileSetPos
	#define CHK_SysFileSetPos  TRUE
	#define EXP_SysFileSetPos  CAL_CMEXPAPI( "SysFileSetPos" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileSetPos
	#define EXT_SysFileSetPos
	#define GET_SysFileSetPos(fl)  CAL_CMGETAPI( "SysFileSetPos" ) 
	#define CAL_SysFileSetPos  SysFileSetPos
	#define CHK_SysFileSetPos  TRUE
	#define EXP_SysFileSetPos  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileSetPos", (RTS_UINTPTR)SysFileSetPos, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileSetPos
	#define EXT_SysFileSysFileSetPos
	#define GET_SysFileSysFileSetPos  ERR_OK
	#define CAL_SysFileSysFileSetPos(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileSetPos(p1) : ((ISysFile*)p0)->ISysFileSetPos(p1))
	#define CHK_SysFileSysFileSetPos  (pISysFile != NULL)
	#define EXP_SysFileSysFileSetPos  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileSetPos
	#define EXT_SysFileSetPos
	#define GET_SysFileSetPos(fl)  CAL_CMGETAPI( "SysFileSetPos" ) 
	#define CAL_SysFileSetPos(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileSetPos(p1) : ((ISysFile*)p0)->ISysFileSetPos(p1))
	#define CHK_SysFileSetPos  (pISysFile != NULL)
	#define EXP_SysFileSetPos  CAL_CMEXPAPI( "SysFileSetPos" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileSetPos  PFSYSFILESETPOS pfSysFileSetPos;
	#define EXT_SysFileSetPos  extern PFSYSFILESETPOS pfSysFileSetPos;
	#define GET_SysFileSetPos(fl)  s_pfCMGetAPI2( "SysFileSetPos", (RTS_VOID_FCTPTR *)&pfSysFileSetPos, (fl), 0, 0)
	#define CAL_SysFileSetPos  pfSysFileSetPos
	#define CHK_SysFileSetPos  (pfSysFileSetPos != NULL)
	#define EXP_SysFileSetPos  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileSetPos", (RTS_UINTPTR)SysFileSetPos, 0, 0) 
#endif




/**
 * <description>
 *	Get actual file pointer position
 * </description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <param name="puiPos" type="OUT">Pointer to get actual position of the file pointer from
 *	the beginning of the file</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileGetPos(RTS_HANDLE hFile, RTS_SIZE *puiPos);
typedef RTS_RESULT (CDECL * PFSYSFILEGETPOS) (RTS_HANDLE hFile, RTS_SIZE *puiPos);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETPOS_NOTIMPLEMENTED)
	#define USE_SysFileGetPos
	#define EXT_SysFileGetPos
	#define GET_SysFileGetPos(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetPos(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetPos  FALSE
	#define EXP_SysFileGetPos  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetPos
	#define EXT_SysFileGetPos
	#define GET_SysFileGetPos(fl)  CAL_CMGETAPI( "SysFileGetPos" ) 
	#define CAL_SysFileGetPos  SysFileGetPos
	#define CHK_SysFileGetPos  TRUE
	#define EXP_SysFileGetPos  CAL_CMEXPAPI( "SysFileGetPos" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetPos
	#define EXT_SysFileGetPos
	#define GET_SysFileGetPos(fl)  CAL_CMGETAPI( "SysFileGetPos" ) 
	#define CAL_SysFileGetPos  SysFileGetPos
	#define CHK_SysFileGetPos  TRUE
	#define EXP_SysFileGetPos  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetPos", (RTS_UINTPTR)SysFileGetPos, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetPos
	#define EXT_SysFileSysFileGetPos
	#define GET_SysFileSysFileGetPos  ERR_OK
	#define CAL_SysFileSysFileGetPos(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileGetPos(p1) : ((ISysFile*)p0)->ISysFileGetPos(p1))
	#define CHK_SysFileSysFileGetPos  (pISysFile != NULL)
	#define EXP_SysFileSysFileGetPos  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetPos
	#define EXT_SysFileGetPos
	#define GET_SysFileGetPos(fl)  CAL_CMGETAPI( "SysFileGetPos" ) 
	#define CAL_SysFileGetPos(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileGetPos(p1) : ((ISysFile*)p0)->ISysFileGetPos(p1))
	#define CHK_SysFileGetPos  (pISysFile != NULL)
	#define EXP_SysFileGetPos  CAL_CMEXPAPI( "SysFileGetPos" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetPos  PFSYSFILEGETPOS pfSysFileGetPos;
	#define EXT_SysFileGetPos  extern PFSYSFILEGETPOS pfSysFileGetPos;
	#define GET_SysFileGetPos(fl)  s_pfCMGetAPI2( "SysFileGetPos", (RTS_VOID_FCTPTR *)&pfSysFileGetPos, (fl), 0, 0)
	#define CAL_SysFileGetPos  pfSysFileGetPos
	#define CHK_SysFileGetPos  (pfSysFileGetPos != NULL)
	#define EXP_SysFileGetPos  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetPos", (RTS_UINTPTR)SysFileGetPos, 0, 0) 
#endif




/**
 * <description>
 *	Get file size of the file specified by name.
 *	A standard path will be added to the filename, if no path is specified.
 *  File names can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\\'!
 *
 *  If the result is != ERR_OK, the size is always set to 0.
 * </description>
 * <param name="pszFile" type="IN">File name</param>
 * <param name="pResult" type="OUT">error code</param>
 * <errorcode name="pResult" type="ERR_OK">file exists and size could be retrieved</errorcode>
 * <errorcode name="pResult" type="ERR_NO_OBJECT">path pointed to a directory, or file does not exist.</errorcode>
 * <errorcode name="pResult" type="ERR_FAILED">Filesystem could not be read.</errorcode>
 * <result>Size of the file in bytes</result>
 */
RTS_SIZE CDECL SysFileGetSize(char *pszFile, RTS_RESULT *pResult);
typedef RTS_SIZE (CDECL * PFSYSFILEGETSIZE) (char *pszFile, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETSIZE_NOTIMPLEMENTED)
	#define USE_SysFileGetSize
	#define EXT_SysFileGetSize
	#define GET_SysFileGetSize(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetSize(p0,p1)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetSize  FALSE
	#define EXP_SysFileGetSize  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetSize
	#define EXT_SysFileGetSize
	#define GET_SysFileGetSize(fl)  CAL_CMGETAPI( "SysFileGetSize" ) 
	#define CAL_SysFileGetSize  SysFileGetSize
	#define CHK_SysFileGetSize  TRUE
	#define EXP_SysFileGetSize  CAL_CMEXPAPI( "SysFileGetSize" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetSize
	#define EXT_SysFileGetSize
	#define GET_SysFileGetSize(fl)  CAL_CMGETAPI( "SysFileGetSize" ) 
	#define CAL_SysFileGetSize  SysFileGetSize
	#define CHK_SysFileGetSize  TRUE
	#define EXP_SysFileGetSize  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetSize", (RTS_UINTPTR)SysFileGetSize, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetSize
	#define EXT_SysFileSysFileGetSize
	#define GET_SysFileSysFileGetSize  ERR_OK
	#define CAL_SysFileSysFileGetSize pISysFile->ISysFileGetSize
	#define CHK_SysFileSysFileGetSize (pISysFile != NULL)
	#define EXP_SysFileSysFileGetSize  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetSize
	#define EXT_SysFileGetSize
	#define GET_SysFileGetSize(fl)  CAL_CMGETAPI( "SysFileGetSize" ) 
	#define CAL_SysFileGetSize pISysFile->ISysFileGetSize
	#define CHK_SysFileGetSize (pISysFile != NULL)
	#define EXP_SysFileGetSize  CAL_CMEXPAPI( "SysFileGetSize" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetSize  PFSYSFILEGETSIZE pfSysFileGetSize;
	#define EXT_SysFileGetSize  extern PFSYSFILEGETSIZE pfSysFileGetSize;
	#define GET_SysFileGetSize(fl)  s_pfCMGetAPI2( "SysFileGetSize", (RTS_VOID_FCTPTR *)&pfSysFileGetSize, (fl), 0, 0)
	#define CAL_SysFileGetSize  pfSysFileGetSize
	#define CHK_SysFileGetSize  (pfSysFileGetSize != NULL)
	#define EXP_SysFileGetSize  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetSize", (RTS_UINTPTR)SysFileGetSize, 0, 0) 
#endif




/**
 * <description>
 *	Get file size of the file specified by name, No standard path will be added to the file name.
 * </description>
 * <param name="pszFile" type="IN">File name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Size of the file in bytes</result>
 */
RTS_SIZE CDECL SysFileGetSize_(char *pszFile, RTS_RESULT *pResult);
typedef RTS_SIZE (CDECL * PFSYSFILEGETSIZE_) (char *pszFile, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETSIZE__NOTIMPLEMENTED)
	#define USE_SysFileGetSize_
	#define EXT_SysFileGetSize_
	#define GET_SysFileGetSize_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetSize_(p0,p1)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetSize_  FALSE
	#define EXP_SysFileGetSize_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetSize_
	#define EXT_SysFileGetSize_
	#define GET_SysFileGetSize_(fl)  CAL_CMGETAPI( "SysFileGetSize_" ) 
	#define CAL_SysFileGetSize_  SysFileGetSize_
	#define CHK_SysFileGetSize_  TRUE
	#define EXP_SysFileGetSize_  CAL_CMEXPAPI( "SysFileGetSize_" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetSize_
	#define EXT_SysFileGetSize_
	#define GET_SysFileGetSize_(fl)  CAL_CMGETAPI( "SysFileGetSize_" ) 
	#define CAL_SysFileGetSize_  SysFileGetSize_
	#define CHK_SysFileGetSize_  TRUE
	#define EXP_SysFileGetSize_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetSize_", (RTS_UINTPTR)SysFileGetSize_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetSize_
	#define EXT_SysFileSysFileGetSize_
	#define GET_SysFileSysFileGetSize_  ERR_OK
	#define CAL_SysFileSysFileGetSize_ pISysFile->ISysFileGetSize_
	#define CHK_SysFileSysFileGetSize_ (pISysFile != NULL)
	#define EXP_SysFileSysFileGetSize_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetSize_
	#define EXT_SysFileGetSize_
	#define GET_SysFileGetSize_(fl)  CAL_CMGETAPI( "SysFileGetSize_" ) 
	#define CAL_SysFileGetSize_ pISysFile->ISysFileGetSize_
	#define CHK_SysFileGetSize_ (pISysFile != NULL)
	#define EXP_SysFileGetSize_  CAL_CMEXPAPI( "SysFileGetSize_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetSize_  PFSYSFILEGETSIZE_ pfSysFileGetSize_;
	#define EXT_SysFileGetSize_  extern PFSYSFILEGETSIZE_ pfSysFileGetSize_;
	#define GET_SysFileGetSize_(fl)  s_pfCMGetAPI2( "SysFileGetSize_", (RTS_VOID_FCTPTR *)&pfSysFileGetSize_, (fl), 0, 0)
	#define CAL_SysFileGetSize_  pfSysFileGetSize_
	#define CHK_SysFileGetSize_  (pfSysFileGetSize_ != NULL)
	#define EXP_SysFileGetSize_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetSize_", (RTS_UINTPTR)SysFileGetSize_, 0, 0) 
#endif




/**
 * <description>
 *	Get file size of the actual opened file
 * </description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Size of the file in bytes</result>
 */
RTS_SIZE CDECL SysFileGetSizeByHandle(RTS_HANDLE hFile, RTS_RESULT *pResult);
typedef RTS_SIZE (CDECL * PFSYSFILEGETSIZEBYHANDLE) (RTS_HANDLE hFile, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETSIZEBYHANDLE_NOTIMPLEMENTED)
	#define USE_SysFileGetSizeByHandle
	#define EXT_SysFileGetSizeByHandle
	#define GET_SysFileGetSizeByHandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetSizeByHandle(p0,p1)  (RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetSizeByHandle  FALSE
	#define EXP_SysFileGetSizeByHandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetSizeByHandle
	#define EXT_SysFileGetSizeByHandle
	#define GET_SysFileGetSizeByHandle(fl)  CAL_CMGETAPI( "SysFileGetSizeByHandle" ) 
	#define CAL_SysFileGetSizeByHandle  SysFileGetSizeByHandle
	#define CHK_SysFileGetSizeByHandle  TRUE
	#define EXP_SysFileGetSizeByHandle  CAL_CMEXPAPI( "SysFileGetSizeByHandle" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetSizeByHandle
	#define EXT_SysFileGetSizeByHandle
	#define GET_SysFileGetSizeByHandle(fl)  CAL_CMGETAPI( "SysFileGetSizeByHandle" ) 
	#define CAL_SysFileGetSizeByHandle  SysFileGetSizeByHandle
	#define CHK_SysFileGetSizeByHandle  TRUE
	#define EXP_SysFileGetSizeByHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetSizeByHandle", (RTS_UINTPTR)SysFileGetSizeByHandle, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetSizeByHandle
	#define EXT_SysFileSysFileGetSizeByHandle
	#define GET_SysFileSysFileGetSizeByHandle  ERR_OK
	#define CAL_SysFileSysFileGetSizeByHandle(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileGetSizeByHandle(p1) : ((ISysFile*)p0)->ISysFileGetSizeByHandle(p1))
	#define CHK_SysFileSysFileGetSizeByHandle  (pISysFile != NULL)
	#define EXP_SysFileSysFileGetSizeByHandle  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetSizeByHandle
	#define EXT_SysFileGetSizeByHandle
	#define GET_SysFileGetSizeByHandle(fl)  CAL_CMGETAPI( "SysFileGetSizeByHandle" ) 
	#define CAL_SysFileGetSizeByHandle(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileGetSizeByHandle(p1) : ((ISysFile*)p0)->ISysFileGetSizeByHandle(p1))
	#define CHK_SysFileGetSizeByHandle  (pISysFile != NULL)
	#define EXP_SysFileGetSizeByHandle  CAL_CMEXPAPI( "SysFileGetSizeByHandle" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetSizeByHandle  PFSYSFILEGETSIZEBYHANDLE pfSysFileGetSizeByHandle;
	#define EXT_SysFileGetSizeByHandle  extern PFSYSFILEGETSIZEBYHANDLE pfSysFileGetSizeByHandle;
	#define GET_SysFileGetSizeByHandle(fl)  s_pfCMGetAPI2( "SysFileGetSizeByHandle", (RTS_VOID_FCTPTR *)&pfSysFileGetSizeByHandle, (fl), 0, 0)
	#define CAL_SysFileGetSizeByHandle  pfSysFileGetSizeByHandle
	#define CHK_SysFileGetSizeByHandle  (pfSysFileGetSizeByHandle != NULL)
	#define EXP_SysFileGetSizeByHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetSizeByHandle", (RTS_UINTPTR)SysFileGetSizeByHandle, 0, 0) 
#endif




/**
 * <description>
 *	Get file time of the specified file.
 *	A standard path will be added to the filename, if no path is specified.
 * </description>
 * <param name="pszFile" type="IN">File name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <param name="pftFileTime" type="OUT">Pointer to get the file time results.
 *	IMPLEMENTATION NOTE: All time values must be local time!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileGetTime(char *pszFile, SYS_FILETIME *pftFileTime);
typedef RTS_RESULT (CDECL * PFSYSFILEGETTIME) (char *pszFile, SYS_FILETIME *pftFileTime);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETTIME_NOTIMPLEMENTED)
	#define USE_SysFileGetTime
	#define EXT_SysFileGetTime
	#define GET_SysFileGetTime(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetTime(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetTime  FALSE
	#define EXP_SysFileGetTime  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetTime
	#define EXT_SysFileGetTime
	#define GET_SysFileGetTime(fl)  CAL_CMGETAPI( "SysFileGetTime" ) 
	#define CAL_SysFileGetTime  SysFileGetTime
	#define CHK_SysFileGetTime  TRUE
	#define EXP_SysFileGetTime  CAL_CMEXPAPI( "SysFileGetTime" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetTime
	#define EXT_SysFileGetTime
	#define GET_SysFileGetTime(fl)  CAL_CMGETAPI( "SysFileGetTime" ) 
	#define CAL_SysFileGetTime  SysFileGetTime
	#define CHK_SysFileGetTime  TRUE
	#define EXP_SysFileGetTime  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetTime", (RTS_UINTPTR)SysFileGetTime, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetTime
	#define EXT_SysFileSysFileGetTime
	#define GET_SysFileSysFileGetTime  ERR_OK
	#define CAL_SysFileSysFileGetTime pISysFile->ISysFileGetTime
	#define CHK_SysFileSysFileGetTime (pISysFile != NULL)
	#define EXP_SysFileSysFileGetTime  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetTime
	#define EXT_SysFileGetTime
	#define GET_SysFileGetTime(fl)  CAL_CMGETAPI( "SysFileGetTime" ) 
	#define CAL_SysFileGetTime pISysFile->ISysFileGetTime
	#define CHK_SysFileGetTime (pISysFile != NULL)
	#define EXP_SysFileGetTime  CAL_CMEXPAPI( "SysFileGetTime" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetTime  PFSYSFILEGETTIME pfSysFileGetTime;
	#define EXT_SysFileGetTime  extern PFSYSFILEGETTIME pfSysFileGetTime;
	#define GET_SysFileGetTime(fl)  s_pfCMGetAPI2( "SysFileGetTime", (RTS_VOID_FCTPTR *)&pfSysFileGetTime, (fl), 0, 0)
	#define CAL_SysFileGetTime  pfSysFileGetTime
	#define CHK_SysFileGetTime  (pfSysFileGetTime != NULL)
	#define EXP_SysFileGetTime  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetTime", (RTS_UINTPTR)SysFileGetTime, 0, 0) 
#endif




/**
 * <description>
 *	Get file time of the file specified by name. No standard path will be added.
 * </description>
 * <param name="pszFile" type="IN">File name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <param name="pftFileTime" type="OUT">Pointer to get the file time results.
 *	IMPLEMENTATION NOTE: All time values must be local time!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileGetTime_(char *pszFile, SYS_FILETIME *pftFileTime);
typedef RTS_RESULT (CDECL * PFSYSFILEGETTIME_) (char *pszFile, SYS_FILETIME *pftFileTime);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETTIME__NOTIMPLEMENTED)
	#define USE_SysFileGetTime_
	#define EXT_SysFileGetTime_
	#define GET_SysFileGetTime_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetTime_(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetTime_  FALSE
	#define EXP_SysFileGetTime_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetTime_
	#define EXT_SysFileGetTime_
	#define GET_SysFileGetTime_(fl)  CAL_CMGETAPI( "SysFileGetTime_" ) 
	#define CAL_SysFileGetTime_  SysFileGetTime_
	#define CHK_SysFileGetTime_  TRUE
	#define EXP_SysFileGetTime_  CAL_CMEXPAPI( "SysFileGetTime_" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetTime_
	#define EXT_SysFileGetTime_
	#define GET_SysFileGetTime_(fl)  CAL_CMGETAPI( "SysFileGetTime_" ) 
	#define CAL_SysFileGetTime_  SysFileGetTime_
	#define CHK_SysFileGetTime_  TRUE
	#define EXP_SysFileGetTime_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetTime_", (RTS_UINTPTR)SysFileGetTime_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetTime_
	#define EXT_SysFileSysFileGetTime_
	#define GET_SysFileSysFileGetTime_  ERR_OK
	#define CAL_SysFileSysFileGetTime_ pISysFile->ISysFileGetTime_
	#define CHK_SysFileSysFileGetTime_ (pISysFile != NULL)
	#define EXP_SysFileSysFileGetTime_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetTime_
	#define EXT_SysFileGetTime_
	#define GET_SysFileGetTime_(fl)  CAL_CMGETAPI( "SysFileGetTime_" ) 
	#define CAL_SysFileGetTime_ pISysFile->ISysFileGetTime_
	#define CHK_SysFileGetTime_ (pISysFile != NULL)
	#define EXP_SysFileGetTime_  CAL_CMEXPAPI( "SysFileGetTime_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetTime_  PFSYSFILEGETTIME_ pfSysFileGetTime_;
	#define EXT_SysFileGetTime_  extern PFSYSFILEGETTIME_ pfSysFileGetTime_;
	#define GET_SysFileGetTime_(fl)  s_pfCMGetAPI2( "SysFileGetTime_", (RTS_VOID_FCTPTR *)&pfSysFileGetTime_, (fl), 0, 0)
	#define CAL_SysFileGetTime_  pfSysFileGetTime_
	#define CHK_SysFileGetTime_  (pfSysFileGetTime_ != NULL)
	#define EXP_SysFileGetTime_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetTime_", (RTS_UINTPTR)SysFileGetTime_, 0, 0) 
#endif




/**

 * <description>
 *	Check, if end of file is reached at reading from the file.
 *	IMPLEMENTATION NOTE:
 *	End of file is only checked after a read operation with SysFileRead! But after a SysFileWrite or SysFileSetPos call, the function
 *	returns ERR_FAILED (no end of file)!
 * </description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <result>Error code:
 *	<ul>
 *		<li>ERR_OK: End of file reached at reading beyond the end of the file</li>
 *		<li>ERR_FAILED: No end of file reached</li>
 *		<li>ERR_PARAMETER: hFile is invalid</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL SysFileEOF(RTS_HANDLE hFile);
typedef RTS_RESULT (CDECL * PFSYSFILEEOF) (RTS_HANDLE hFile);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEEOF_NOTIMPLEMENTED)
	#define USE_SysFileEOF
	#define EXT_SysFileEOF
	#define GET_SysFileEOF(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileEOF(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileEOF  FALSE
	#define EXP_SysFileEOF  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileEOF
	#define EXT_SysFileEOF
	#define GET_SysFileEOF(fl)  CAL_CMGETAPI( "SysFileEOF" ) 
	#define CAL_SysFileEOF  SysFileEOF
	#define CHK_SysFileEOF  TRUE
	#define EXP_SysFileEOF  CAL_CMEXPAPI( "SysFileEOF" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileEOF
	#define EXT_SysFileEOF
	#define GET_SysFileEOF(fl)  CAL_CMGETAPI( "SysFileEOF" ) 
	#define CAL_SysFileEOF  SysFileEOF
	#define CHK_SysFileEOF  TRUE
	#define EXP_SysFileEOF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileEOF", (RTS_UINTPTR)SysFileEOF, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileEOF
	#define EXT_SysFileSysFileEOF
	#define GET_SysFileSysFileEOF  ERR_OK
	#define CAL_SysFileSysFileEOF(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileEOF() : ((ISysFile*)p0)->ISysFileEOF())
	#define CHK_SysFileSysFileEOF  (pISysFile != NULL)
	#define EXP_SysFileSysFileEOF  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileEOF
	#define EXT_SysFileEOF
	#define GET_SysFileEOF(fl)  CAL_CMGETAPI( "SysFileEOF" ) 
	#define CAL_SysFileEOF(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileEOF() : ((ISysFile*)p0)->ISysFileEOF())
	#define CHK_SysFileEOF  (pISysFile != NULL)
	#define EXP_SysFileEOF  CAL_CMEXPAPI( "SysFileEOF" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileEOF  PFSYSFILEEOF pfSysFileEOF;
	#define EXT_SysFileEOF  extern PFSYSFILEEOF pfSysFileEOF;
	#define GET_SysFileEOF(fl)  s_pfCMGetAPI2( "SysFileEOF", (RTS_VOID_FCTPTR *)&pfSysFileEOF, (fl), 0, 0)
	#define CAL_SysFileEOF  pfSysFileEOF
	#define CHK_SysFileEOF  (pfSysFileEOF != NULL)
	#define EXP_SysFileEOF  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileEOF", (RTS_UINTPTR)SysFileEOF, 0, 0) 
#endif




/**
 * <description>
 *	Copy one file to another.
 *	A standard path will be added to the filename, if no path is specified.
 *  IMPLEMENTATION NOTE:
 *	If the destination file exists, the file will be overwritten and the function succeeded.
 * </description>
 * <param name="pszDestFileName" type="IN">Destination file name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <param name="pszSourceFileName" type="IN">Source file name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <param name="puiCopied" type="OUT">Number of bytes copied</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileCopy(char *pszDestFileName, char *pszSourceFileName, RTS_SIZE *puiCopied);
typedef RTS_RESULT (CDECL * PFSYSFILECOPY) (char *pszDestFileName, char *pszSourceFileName, RTS_SIZE *puiCopied);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILECOPY_NOTIMPLEMENTED)
	#define USE_SysFileCopy
	#define EXT_SysFileCopy
	#define GET_SysFileCopy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileCopy(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileCopy  FALSE
	#define EXP_SysFileCopy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileCopy
	#define EXT_SysFileCopy
	#define GET_SysFileCopy(fl)  CAL_CMGETAPI( "SysFileCopy" ) 
	#define CAL_SysFileCopy  SysFileCopy
	#define CHK_SysFileCopy  TRUE
	#define EXP_SysFileCopy  CAL_CMEXPAPI( "SysFileCopy" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileCopy
	#define EXT_SysFileCopy
	#define GET_SysFileCopy(fl)  CAL_CMGETAPI( "SysFileCopy" ) 
	#define CAL_SysFileCopy  SysFileCopy
	#define CHK_SysFileCopy  TRUE
	#define EXP_SysFileCopy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileCopy", (RTS_UINTPTR)SysFileCopy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileCopy
	#define EXT_SysFileSysFileCopy
	#define GET_SysFileSysFileCopy  ERR_OK
	#define CAL_SysFileSysFileCopy pISysFile->ISysFileCopy
	#define CHK_SysFileSysFileCopy (pISysFile != NULL)
	#define EXP_SysFileSysFileCopy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileCopy
	#define EXT_SysFileCopy
	#define GET_SysFileCopy(fl)  CAL_CMGETAPI( "SysFileCopy" ) 
	#define CAL_SysFileCopy pISysFile->ISysFileCopy
	#define CHK_SysFileCopy (pISysFile != NULL)
	#define EXP_SysFileCopy  CAL_CMEXPAPI( "SysFileCopy" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileCopy  PFSYSFILECOPY pfSysFileCopy;
	#define EXT_SysFileCopy  extern PFSYSFILECOPY pfSysFileCopy;
	#define GET_SysFileCopy(fl)  s_pfCMGetAPI2( "SysFileCopy", (RTS_VOID_FCTPTR *)&pfSysFileCopy, (fl), 0, 0)
	#define CAL_SysFileCopy  pfSysFileCopy
	#define CHK_SysFileCopy  (pfSysFileCopy != NULL)
	#define EXP_SysFileCopy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileCopy", (RTS_UINTPTR)SysFileCopy, 0, 0) 
#endif




/**
 * <description>
 *	Copy one file in another. No standard path will be added to filename.
 * </description>
 * <param name="pszFile" type="IN">File name. File name can contain an absolute or relative path to the 
 *	file. Path entries must be separated with a '/' and not with a '\"!</param>
 * <param name="pftFileTime" type="OUT">Pointer to get the file time results</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileCopy_(char *pszDestFileName, char *pszSourceFileName, RTS_SIZE *puiCopied);
typedef RTS_RESULT (CDECL * PFSYSFILECOPY_) (char *pszDestFileName, char *pszSourceFileName, RTS_SIZE *puiCopied);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILECOPY__NOTIMPLEMENTED)
	#define USE_SysFileCopy_
	#define EXT_SysFileCopy_
	#define GET_SysFileCopy_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileCopy_(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileCopy_  FALSE
	#define EXP_SysFileCopy_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileCopy_
	#define EXT_SysFileCopy_
	#define GET_SysFileCopy_(fl)  CAL_CMGETAPI( "SysFileCopy_" ) 
	#define CAL_SysFileCopy_  SysFileCopy_
	#define CHK_SysFileCopy_  TRUE
	#define EXP_SysFileCopy_  CAL_CMEXPAPI( "SysFileCopy_" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileCopy_
	#define EXT_SysFileCopy_
	#define GET_SysFileCopy_(fl)  CAL_CMGETAPI( "SysFileCopy_" ) 
	#define CAL_SysFileCopy_  SysFileCopy_
	#define CHK_SysFileCopy_  TRUE
	#define EXP_SysFileCopy_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileCopy_", (RTS_UINTPTR)SysFileCopy_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileCopy_
	#define EXT_SysFileSysFileCopy_
	#define GET_SysFileSysFileCopy_  ERR_OK
	#define CAL_SysFileSysFileCopy_ pISysFile->ISysFileCopy_
	#define CHK_SysFileSysFileCopy_ (pISysFile != NULL)
	#define EXP_SysFileSysFileCopy_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileCopy_
	#define EXT_SysFileCopy_
	#define GET_SysFileCopy_(fl)  CAL_CMGETAPI( "SysFileCopy_" ) 
	#define CAL_SysFileCopy_ pISysFile->ISysFileCopy_
	#define CHK_SysFileCopy_ (pISysFile != NULL)
	#define EXP_SysFileCopy_  CAL_CMEXPAPI( "SysFileCopy_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileCopy_  PFSYSFILECOPY_ pfSysFileCopy_;
	#define EXT_SysFileCopy_  extern PFSYSFILECOPY_ pfSysFileCopy_;
	#define GET_SysFileCopy_(fl)  s_pfCMGetAPI2( "SysFileCopy_", (RTS_VOID_FCTPTR *)&pfSysFileCopy_, (fl), 0, 0)
	#define CAL_SysFileCopy_  pfSysFileCopy_
	#define CHK_SysFileCopy_  (pfSysFileCopy_ != NULL)
	#define EXP_SysFileCopy_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileCopy_", (RTS_UINTPTR)SysFileCopy_, 0, 0) 
#endif




/**
 * <description>
 *	Get the path of this file. If a path is specified in the filename, the path will be extracted from the filename.
 *	If no path is specified in the filename, the standard path for this file extension type will be returned.
 * </description>
 * <param name="pszFileName" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="pszPath" type="OUT">Path for this file</param>
 * <param name="iMaxLen" type="IN">Maximum size in bytes of path length</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Resolved file path is correctly returned</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter (pointer NULL or length 0)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">Path buffer too short</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_OPERATION_DENIED">Access to this file path not allowed! The following file or directory access must be denied!</errorcode>
 */
RTS_RESULT CDECL SysFileGetPath(char *pszFileName, char *pszPath, RTS_SIZE iMaxLen);
typedef RTS_RESULT (CDECL * PFSYSFILEGETPATH) (char *pszFileName, char *pszPath, RTS_SIZE iMaxLen);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETPATH_NOTIMPLEMENTED)
	#define USE_SysFileGetPath
	#define EXT_SysFileGetPath
	#define GET_SysFileGetPath(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetPath(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetPath  FALSE
	#define EXP_SysFileGetPath  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetPath
	#define EXT_SysFileGetPath
	#define GET_SysFileGetPath(fl)  CAL_CMGETAPI( "SysFileGetPath" ) 
	#define CAL_SysFileGetPath  SysFileGetPath
	#define CHK_SysFileGetPath  TRUE
	#define EXP_SysFileGetPath  CAL_CMEXPAPI( "SysFileGetPath" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetPath
	#define EXT_SysFileGetPath
	#define GET_SysFileGetPath(fl)  CAL_CMGETAPI( "SysFileGetPath" ) 
	#define CAL_SysFileGetPath  SysFileGetPath
	#define CHK_SysFileGetPath  TRUE
	#define EXP_SysFileGetPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetPath", (RTS_UINTPTR)SysFileGetPath, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetPath
	#define EXT_SysFileSysFileGetPath
	#define GET_SysFileSysFileGetPath  ERR_OK
	#define CAL_SysFileSysFileGetPath pISysFile->ISysFileGetPath
	#define CHK_SysFileSysFileGetPath (pISysFile != NULL)
	#define EXP_SysFileSysFileGetPath  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetPath
	#define EXT_SysFileGetPath
	#define GET_SysFileGetPath(fl)  CAL_CMGETAPI( "SysFileGetPath" ) 
	#define CAL_SysFileGetPath pISysFile->ISysFileGetPath
	#define CHK_SysFileGetPath (pISysFile != NULL)
	#define EXP_SysFileGetPath  CAL_CMEXPAPI( "SysFileGetPath" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetPath  PFSYSFILEGETPATH pfSysFileGetPath;
	#define EXT_SysFileGetPath  extern PFSYSFILEGETPATH pfSysFileGetPath;
	#define GET_SysFileGetPath(fl)  s_pfCMGetAPI2( "SysFileGetPath", (RTS_VOID_FCTPTR *)&pfSysFileGetPath, (fl), 0, 0)
	#define CAL_SysFileGetPath  pfSysFileGetPath
	#define CHK_SysFileGetPath  (pfSysFileGetPath != NULL)
	#define EXP_SysFileGetPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetPath", (RTS_UINTPTR)SysFileGetPath, 0, 0) 
#endif




/**
 * <description>
 *	Get the path of this file. If a path is specified in the filename, the path will be extracted from the filename.
 *	If no path is specified in the filename, the standard path for this file extension type will be returned.
 * </description>
 * <param name="pszFileName" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="nOption" type="IN">Separation of directory or file (see Path options)</param>
 * <param name="pszPath" type="OUT">Path for this file</param>
 * <param name="iMaxLen" type="IN">Maximum size in bytes of path length</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Resolved file path is correctly returned</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter (pointer NULL or length 0)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">Path buffer too short</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_OPERATION_DENIED">Access to this file path not allowed! The following file or directory access must be denied!</errorcode>
 */
RTS_RESULT CDECL SysFileGetPath2(char *pszFileName, RTS_I32 nOption, char *pszPath, RTS_SIZE iMaxLen);
typedef RTS_RESULT (CDECL * PFSYSFILEGETPATH2) (char *pszFileName, RTS_I32 nOption, char *pszPath, RTS_SIZE iMaxLen);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETPATH2_NOTIMPLEMENTED)
	#define USE_SysFileGetPath2
	#define EXT_SysFileGetPath2
	#define GET_SysFileGetPath2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetPath2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetPath2  FALSE
	#define EXP_SysFileGetPath2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetPath2
	#define EXT_SysFileGetPath2
	#define GET_SysFileGetPath2(fl)  CAL_CMGETAPI( "SysFileGetPath2" ) 
	#define CAL_SysFileGetPath2  SysFileGetPath2
	#define CHK_SysFileGetPath2  TRUE
	#define EXP_SysFileGetPath2  CAL_CMEXPAPI( "SysFileGetPath2" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetPath2
	#define EXT_SysFileGetPath2
	#define GET_SysFileGetPath2(fl)  CAL_CMGETAPI( "SysFileGetPath2" ) 
	#define CAL_SysFileGetPath2  SysFileGetPath2
	#define CHK_SysFileGetPath2  TRUE
	#define EXP_SysFileGetPath2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetPath2", (RTS_UINTPTR)SysFileGetPath2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetPath2
	#define EXT_SysFileSysFileGetPath2
	#define GET_SysFileSysFileGetPath2  ERR_OK
	#define CAL_SysFileSysFileGetPath2 pISysFile->ISysFileGetPath2
	#define CHK_SysFileSysFileGetPath2 (pISysFile != NULL)
	#define EXP_SysFileSysFileGetPath2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetPath2
	#define EXT_SysFileGetPath2
	#define GET_SysFileGetPath2(fl)  CAL_CMGETAPI( "SysFileGetPath2" ) 
	#define CAL_SysFileGetPath2 pISysFile->ISysFileGetPath2
	#define CHK_SysFileGetPath2 (pISysFile != NULL)
	#define EXP_SysFileGetPath2  CAL_CMEXPAPI( "SysFileGetPath2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetPath2  PFSYSFILEGETPATH2 pfSysFileGetPath2;
	#define EXT_SysFileGetPath2  extern PFSYSFILEGETPATH2 pfSysFileGetPath2;
	#define GET_SysFileGetPath2(fl)  s_pfCMGetAPI2( "SysFileGetPath2", (RTS_VOID_FCTPTR *)&pfSysFileGetPath2, (fl), 0, 0)
	#define CAL_SysFileGetPath2  pfSysFileGetPath2
	#define CHK_SysFileGetPath2  (pfSysFileGetPath2 != NULL)
	#define EXP_SysFileGetPath2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetPath2", (RTS_UINTPTR)SysFileGetPath2, 0, 0) 
#endif




/**
 * <description>
 *	Get the path of this file including the filename. If no path is specified in pszFileName, the filename is returned in pszPath.
 * </description>
 * <param name="pszFileName" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="pszPath" type="OUT">Path for this file including the filename</param>
 * <param name="iMaxLen" type="IN">Maximum size in bytes of path length</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Resolved file path including the filename is correctly returned</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter (pointer NULL or length 0)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">Path buffer too short</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_OPERATION_DENIED">Access to this file path not allowed! The following file or directory access must be denied!</errorcode>
 */
RTS_RESULT CDECL SysFileGetFullPath(char *pszFileName, char *pszPath, RTS_SIZE iMaxLen);
typedef RTS_RESULT (CDECL * PFSYSFILEGETFULLPATH) (char *pszFileName, char *pszPath, RTS_SIZE iMaxLen);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETFULLPATH_NOTIMPLEMENTED)
	#define USE_SysFileGetFullPath
	#define EXT_SysFileGetFullPath
	#define GET_SysFileGetFullPath(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetFullPath(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetFullPath  FALSE
	#define EXP_SysFileGetFullPath  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetFullPath
	#define EXT_SysFileGetFullPath
	#define GET_SysFileGetFullPath(fl)  CAL_CMGETAPI( "SysFileGetFullPath" ) 
	#define CAL_SysFileGetFullPath  SysFileGetFullPath
	#define CHK_SysFileGetFullPath  TRUE
	#define EXP_SysFileGetFullPath  CAL_CMEXPAPI( "SysFileGetFullPath" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetFullPath
	#define EXT_SysFileGetFullPath
	#define GET_SysFileGetFullPath(fl)  CAL_CMGETAPI( "SysFileGetFullPath" ) 
	#define CAL_SysFileGetFullPath  SysFileGetFullPath
	#define CHK_SysFileGetFullPath  TRUE
	#define EXP_SysFileGetFullPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetFullPath", (RTS_UINTPTR)SysFileGetFullPath, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetFullPath
	#define EXT_SysFileSysFileGetFullPath
	#define GET_SysFileSysFileGetFullPath  ERR_OK
	#define CAL_SysFileSysFileGetFullPath pISysFile->ISysFileGetFullPath
	#define CHK_SysFileSysFileGetFullPath (pISysFile != NULL)
	#define EXP_SysFileSysFileGetFullPath  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetFullPath
	#define EXT_SysFileGetFullPath
	#define GET_SysFileGetFullPath(fl)  CAL_CMGETAPI( "SysFileGetFullPath" ) 
	#define CAL_SysFileGetFullPath pISysFile->ISysFileGetFullPath
	#define CHK_SysFileGetFullPath (pISysFile != NULL)
	#define EXP_SysFileGetFullPath  CAL_CMEXPAPI( "SysFileGetFullPath" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetFullPath  PFSYSFILEGETFULLPATH pfSysFileGetFullPath;
	#define EXT_SysFileGetFullPath  extern PFSYSFILEGETFULLPATH pfSysFileGetFullPath;
	#define GET_SysFileGetFullPath(fl)  s_pfCMGetAPI2( "SysFileGetFullPath", (RTS_VOID_FCTPTR *)&pfSysFileGetFullPath, (fl), 0, 0)
	#define CAL_SysFileGetFullPath  pfSysFileGetFullPath
	#define CHK_SysFileGetFullPath  (pfSysFileGetFullPath != NULL)
	#define EXP_SysFileGetFullPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetFullPath", (RTS_UINTPTR)SysFileGetFullPath, 0, 0) 
#endif




/**
 * <description>
 *	Get the path of this file including the filename. If no path is specified in pszFileName, the filename is returned in pszPath.
 * </description>
 * <param name="pszFileName" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="nOption" type="IN">Separation of directory or file (see Path options)</param>
 * <param name="pszPath" type="OUT">Path for this file including the filename</param>
 * <param name="iMaxLen" type="IN">Maximum size in bytes of path length</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Resolved file path including the filename is correctly returned</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter (pointer NULL or length 0)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_BUFFERSIZE">Path buffer too short</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_OPERATION_DENIED">Access to this file path not allowed! The following file or directory access must be denied!</errorcode>
 */
RTS_RESULT CDECL SysFileGetFullPath2(char *pszFileName, RTS_I32 nOption, char *pszPath, RTS_SIZE iMaxLen);
typedef RTS_RESULT (CDECL * PFSYSFILEGETFULLPATH2) (char *pszFileName, RTS_I32 nOption, char *pszPath, RTS_SIZE iMaxLen);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETFULLPATH2_NOTIMPLEMENTED)
	#define USE_SysFileGetFullPath2
	#define EXT_SysFileGetFullPath2
	#define GET_SysFileGetFullPath2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetFullPath2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetFullPath2  FALSE
	#define EXP_SysFileGetFullPath2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetFullPath2
	#define EXT_SysFileGetFullPath2
	#define GET_SysFileGetFullPath2(fl)  CAL_CMGETAPI( "SysFileGetFullPath2" ) 
	#define CAL_SysFileGetFullPath2  SysFileGetFullPath2
	#define CHK_SysFileGetFullPath2  TRUE
	#define EXP_SysFileGetFullPath2  CAL_CMEXPAPI( "SysFileGetFullPath2" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetFullPath2
	#define EXT_SysFileGetFullPath2
	#define GET_SysFileGetFullPath2(fl)  CAL_CMGETAPI( "SysFileGetFullPath2" ) 
	#define CAL_SysFileGetFullPath2  SysFileGetFullPath2
	#define CHK_SysFileGetFullPath2  TRUE
	#define EXP_SysFileGetFullPath2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetFullPath2", (RTS_UINTPTR)SysFileGetFullPath2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetFullPath2
	#define EXT_SysFileSysFileGetFullPath2
	#define GET_SysFileSysFileGetFullPath2  ERR_OK
	#define CAL_SysFileSysFileGetFullPath2 pISysFile->ISysFileGetFullPath2
	#define CHK_SysFileSysFileGetFullPath2 (pISysFile != NULL)
	#define EXP_SysFileSysFileGetFullPath2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetFullPath2
	#define EXT_SysFileGetFullPath2
	#define GET_SysFileGetFullPath2(fl)  CAL_CMGETAPI( "SysFileGetFullPath2" ) 
	#define CAL_SysFileGetFullPath2 pISysFile->ISysFileGetFullPath2
	#define CHK_SysFileGetFullPath2 (pISysFile != NULL)
	#define EXP_SysFileGetFullPath2  CAL_CMEXPAPI( "SysFileGetFullPath2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetFullPath2  PFSYSFILEGETFULLPATH2 pfSysFileGetFullPath2;
	#define EXT_SysFileGetFullPath2  extern PFSYSFILEGETFULLPATH2 pfSysFileGetFullPath2;
	#define GET_SysFileGetFullPath2(fl)  s_pfCMGetAPI2( "SysFileGetFullPath2", (RTS_VOID_FCTPTR *)&pfSysFileGetFullPath2, (fl), 0, 0)
	#define CAL_SysFileGetFullPath2  pfSysFileGetFullPath2
	#define CHK_SysFileGetFullPath2  (pfSysFileGetFullPath2 != NULL)
	#define EXP_SysFileGetFullPath2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetFullPath2", (RTS_UINTPTR)SysFileGetFullPath2, 0, 0) 
#endif




/**
 * <description>
 *	Get the path of this file for IEC applications. If a path is specified in the filename, the path will be extracted from the filename.
 *	If no path is specified in the filename, the standard path for this file extension type will be returned.
 * </description>
 * <param name="pszFileName" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="pszPath" type="OUT">Path for this file</param>
 * <param name="iMaxLen" type="IN">Maximum size in bytes of path length</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileGetIecPath(char *pszFileName, char *pszPath, RTS_SIZE iMaxLen);
typedef RTS_RESULT (CDECL * PFSYSFILEGETIECPATH) (char *pszFileName, char *pszPath, RTS_SIZE iMaxLen);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETIECPATH_NOTIMPLEMENTED)
	#define USE_SysFileGetIecPath
	#define EXT_SysFileGetIecPath
	#define GET_SysFileGetIecPath(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetIecPath(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetIecPath  FALSE
	#define EXP_SysFileGetIecPath  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetIecPath
	#define EXT_SysFileGetIecPath
	#define GET_SysFileGetIecPath(fl)  CAL_CMGETAPI( "SysFileGetIecPath" ) 
	#define CAL_SysFileGetIecPath  SysFileGetIecPath
	#define CHK_SysFileGetIecPath  TRUE
	#define EXP_SysFileGetIecPath  CAL_CMEXPAPI( "SysFileGetIecPath" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetIecPath
	#define EXT_SysFileGetIecPath
	#define GET_SysFileGetIecPath(fl)  CAL_CMGETAPI( "SysFileGetIecPath" ) 
	#define CAL_SysFileGetIecPath  SysFileGetIecPath
	#define CHK_SysFileGetIecPath  TRUE
	#define EXP_SysFileGetIecPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetIecPath", (RTS_UINTPTR)SysFileGetIecPath, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetIecPath
	#define EXT_SysFileSysFileGetIecPath
	#define GET_SysFileSysFileGetIecPath  ERR_OK
	#define CAL_SysFileSysFileGetIecPath pISysFile->ISysFileGetIecPath
	#define CHK_SysFileSysFileGetIecPath (pISysFile != NULL)
	#define EXP_SysFileSysFileGetIecPath  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetIecPath
	#define EXT_SysFileGetIecPath
	#define GET_SysFileGetIecPath(fl)  CAL_CMGETAPI( "SysFileGetIecPath" ) 
	#define CAL_SysFileGetIecPath pISysFile->ISysFileGetIecPath
	#define CHK_SysFileGetIecPath (pISysFile != NULL)
	#define EXP_SysFileGetIecPath  CAL_CMEXPAPI( "SysFileGetIecPath" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetIecPath  PFSYSFILEGETIECPATH pfSysFileGetIecPath;
	#define EXT_SysFileGetIecPath  extern PFSYSFILEGETIECPATH pfSysFileGetIecPath;
	#define GET_SysFileGetIecPath(fl)  s_pfCMGetAPI2( "SysFileGetIecPath", (RTS_VOID_FCTPTR *)&pfSysFileGetIecPath, (fl), 0, 0)
	#define CAL_SysFileGetIecPath  pfSysFileGetIecPath
	#define CHK_SysFileGetIecPath  (pfSysFileGetIecPath != NULL)
	#define EXP_SysFileGetIecPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetIecPath", (RTS_UINTPTR)SysFileGetIecPath, 0, 0) 
#endif




/**
 * <description>
 *	Get the path of this file for IEC applications. If a path is specified in the filename, the path will be extracted from the filename.
 *	If no path is specified in the filename, the standard path for this file extension type will be returned.
 * </description>
 * <param name="pszFileName" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="nOption" type="IN">Separation of directory or file (see Path options)</param>
 * <param name="pszPath" type="OUT">Path for this file</param>
 * <param name="iMaxLen" type="IN">Maximum size in bytes of path length</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileGetIecPath2(char *pszFileName, RTS_I32 nOption, char *pszPath, RTS_SIZE iMaxLen);
typedef RTS_RESULT (CDECL * PFSYSFILEGETIECPATH2) (char *pszFileName, RTS_I32 nOption, char *pszPath, RTS_SIZE iMaxLen);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETIECPATH2_NOTIMPLEMENTED)
	#define USE_SysFileGetIecPath2
	#define EXT_SysFileGetIecPath2
	#define GET_SysFileGetIecPath2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetIecPath2(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetIecPath2  FALSE
	#define EXP_SysFileGetIecPath2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetIecPath2
	#define EXT_SysFileGetIecPath2
	#define GET_SysFileGetIecPath2(fl)  CAL_CMGETAPI( "SysFileGetIecPath2" ) 
	#define CAL_SysFileGetIecPath2  SysFileGetIecPath2
	#define CHK_SysFileGetIecPath2  TRUE
	#define EXP_SysFileGetIecPath2  CAL_CMEXPAPI( "SysFileGetIecPath2" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetIecPath2
	#define EXT_SysFileGetIecPath2
	#define GET_SysFileGetIecPath2(fl)  CAL_CMGETAPI( "SysFileGetIecPath2" ) 
	#define CAL_SysFileGetIecPath2  SysFileGetIecPath2
	#define CHK_SysFileGetIecPath2  TRUE
	#define EXP_SysFileGetIecPath2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetIecPath2", (RTS_UINTPTR)SysFileGetIecPath2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetIecPath2
	#define EXT_SysFileSysFileGetIecPath2
	#define GET_SysFileSysFileGetIecPath2  ERR_OK
	#define CAL_SysFileSysFileGetIecPath2 pISysFile->ISysFileGetIecPath2
	#define CHK_SysFileSysFileGetIecPath2 (pISysFile != NULL)
	#define EXP_SysFileSysFileGetIecPath2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetIecPath2
	#define EXT_SysFileGetIecPath2
	#define GET_SysFileGetIecPath2(fl)  CAL_CMGETAPI( "SysFileGetIecPath2" ) 
	#define CAL_SysFileGetIecPath2 pISysFile->ISysFileGetIecPath2
	#define CHK_SysFileGetIecPath2 (pISysFile != NULL)
	#define EXP_SysFileGetIecPath2  CAL_CMEXPAPI( "SysFileGetIecPath2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetIecPath2  PFSYSFILEGETIECPATH2 pfSysFileGetIecPath2;
	#define EXT_SysFileGetIecPath2  extern PFSYSFILEGETIECPATH2 pfSysFileGetIecPath2;
	#define GET_SysFileGetIecPath2(fl)  s_pfCMGetAPI2( "SysFileGetIecPath2", (RTS_VOID_FCTPTR *)&pfSysFileGetIecPath2, (fl), 0, 0)
	#define CAL_SysFileGetIecPath2  pfSysFileGetIecPath2
	#define CHK_SysFileGetIecPath2  (pfSysFileGetIecPath2 != NULL)
	#define EXP_SysFileGetIecPath2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetIecPath2", (RTS_UINTPTR)SysFileGetIecPath2, 0, 0) 
#endif




/**
 * <description>
 *	Generate the CRC32 of a file. Can be used to check file integrity.
 *	ATTENTION: Only for backward compatibility! CRC is implemented not independent from buffer size!
 * </description>
 * <param name="pszFile" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="ulSize" type="IN">Size to calculate checksum. 0 if real size of file should be used [default]</param>
 * <param name="pulCRC" type="OUT">Pointer to get CRC32 result</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileGenerateCRC(char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC);
typedef RTS_RESULT (CDECL * PFSYSFILEGENERATECRC) (char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGENERATECRC_NOTIMPLEMENTED)
	#define USE_SysFileGenerateCRC
	#define EXT_SysFileGenerateCRC
	#define GET_SysFileGenerateCRC(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGenerateCRC(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGenerateCRC  FALSE
	#define EXP_SysFileGenerateCRC  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGenerateCRC
	#define EXT_SysFileGenerateCRC
	#define GET_SysFileGenerateCRC(fl)  CAL_CMGETAPI( "SysFileGenerateCRC" ) 
	#define CAL_SysFileGenerateCRC  SysFileGenerateCRC
	#define CHK_SysFileGenerateCRC  TRUE
	#define EXP_SysFileGenerateCRC  CAL_CMEXPAPI( "SysFileGenerateCRC" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGenerateCRC
	#define EXT_SysFileGenerateCRC
	#define GET_SysFileGenerateCRC(fl)  CAL_CMGETAPI( "SysFileGenerateCRC" ) 
	#define CAL_SysFileGenerateCRC  SysFileGenerateCRC
	#define CHK_SysFileGenerateCRC  TRUE
	#define EXP_SysFileGenerateCRC  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGenerateCRC", (RTS_UINTPTR)SysFileGenerateCRC, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGenerateCRC
	#define EXT_SysFileSysFileGenerateCRC
	#define GET_SysFileSysFileGenerateCRC  ERR_OK
	#define CAL_SysFileSysFileGenerateCRC pISysFile->ISysFileGenerateCRC
	#define CHK_SysFileSysFileGenerateCRC (pISysFile != NULL)
	#define EXP_SysFileSysFileGenerateCRC  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGenerateCRC
	#define EXT_SysFileGenerateCRC
	#define GET_SysFileGenerateCRC(fl)  CAL_CMGETAPI( "SysFileGenerateCRC" ) 
	#define CAL_SysFileGenerateCRC pISysFile->ISysFileGenerateCRC
	#define CHK_SysFileGenerateCRC (pISysFile != NULL)
	#define EXP_SysFileGenerateCRC  CAL_CMEXPAPI( "SysFileGenerateCRC" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGenerateCRC  PFSYSFILEGENERATECRC pfSysFileGenerateCRC;
	#define EXT_SysFileGenerateCRC  extern PFSYSFILEGENERATECRC pfSysFileGenerateCRC;
	#define GET_SysFileGenerateCRC(fl)  s_pfCMGetAPI2( "SysFileGenerateCRC", (RTS_VOID_FCTPTR *)&pfSysFileGenerateCRC, (fl), 0, 0)
	#define CAL_SysFileGenerateCRC  pfSysFileGenerateCRC
	#define CHK_SysFileGenerateCRC  (pfSysFileGenerateCRC != NULL)
	#define EXP_SysFileGenerateCRC  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGenerateCRC", (RTS_UINTPTR)SysFileGenerateCRC, 0, 0) 
#endif




/**
 * <description>
 *	Generate the CRC32 of a file. Can be used to check file integrity.
 * </description>
 * <param name="pszFile" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="ulSize" type="IN">Size to calculate checksum. 0 if real size of file should be used [default]</param>
 * <param name="pulCRC" type="OUT">Pointer to get CRC32 result</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileGenerateCRC2(char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC);
typedef RTS_RESULT (CDECL * PFSYSFILEGENERATECRC2) (char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGENERATECRC2_NOTIMPLEMENTED)
	#define USE_SysFileGenerateCRC2
	#define EXT_SysFileGenerateCRC2
	#define GET_SysFileGenerateCRC2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGenerateCRC2(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGenerateCRC2  FALSE
	#define EXP_SysFileGenerateCRC2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGenerateCRC2
	#define EXT_SysFileGenerateCRC2
	#define GET_SysFileGenerateCRC2(fl)  CAL_CMGETAPI( "SysFileGenerateCRC2" ) 
	#define CAL_SysFileGenerateCRC2  SysFileGenerateCRC2
	#define CHK_SysFileGenerateCRC2  TRUE
	#define EXP_SysFileGenerateCRC2  CAL_CMEXPAPI( "SysFileGenerateCRC2" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGenerateCRC2
	#define EXT_SysFileGenerateCRC2
	#define GET_SysFileGenerateCRC2(fl)  CAL_CMGETAPI( "SysFileGenerateCRC2" ) 
	#define CAL_SysFileGenerateCRC2  SysFileGenerateCRC2
	#define CHK_SysFileGenerateCRC2  TRUE
	#define EXP_SysFileGenerateCRC2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGenerateCRC2", (RTS_UINTPTR)SysFileGenerateCRC2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGenerateCRC2
	#define EXT_SysFileSysFileGenerateCRC2
	#define GET_SysFileSysFileGenerateCRC2  ERR_OK
	#define CAL_SysFileSysFileGenerateCRC2 pISysFile->ISysFileGenerateCRC2
	#define CHK_SysFileSysFileGenerateCRC2 (pISysFile != NULL)
	#define EXP_SysFileSysFileGenerateCRC2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGenerateCRC2
	#define EXT_SysFileGenerateCRC2
	#define GET_SysFileGenerateCRC2(fl)  CAL_CMGETAPI( "SysFileGenerateCRC2" ) 
	#define CAL_SysFileGenerateCRC2 pISysFile->ISysFileGenerateCRC2
	#define CHK_SysFileGenerateCRC2 (pISysFile != NULL)
	#define EXP_SysFileGenerateCRC2  CAL_CMEXPAPI( "SysFileGenerateCRC2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGenerateCRC2  PFSYSFILEGENERATECRC2 pfSysFileGenerateCRC2;
	#define EXT_SysFileGenerateCRC2  extern PFSYSFILEGENERATECRC2 pfSysFileGenerateCRC2;
	#define GET_SysFileGenerateCRC2(fl)  s_pfCMGetAPI2( "SysFileGenerateCRC2", (RTS_VOID_FCTPTR *)&pfSysFileGenerateCRC2, (fl), 0, 0)
	#define CAL_SysFileGenerateCRC2  pfSysFileGenerateCRC2
	#define CHK_SysFileGenerateCRC2  (pfSysFileGenerateCRC2 != NULL)
	#define EXP_SysFileGenerateCRC2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGenerateCRC2", (RTS_UINTPTR)SysFileGenerateCRC2, 0, 0) 
#endif




/**
 * <description>
 *	Generate the CRC32 of a file. Can be used to check file integrity.
 * </description>
 * <param name="pszFile" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="ulSize" type="IN">Size to calculate checksum. 0 if real size of file should be used [default]</param>
 * <param name="pulCRC" type="OUT">Pointer to get CRC32 result</param>
 * <param name="bUseCache" type="IN">Flag whether to use the SysFileMap cache. In case there is no cache this flag is ignored.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileGenerateCRC3(char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC, RTS_BOOL bUseCache);
typedef RTS_RESULT (CDECL * PFSYSFILEGENERATECRC3) (char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC, RTS_BOOL bUseCache);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGENERATECRC3_NOTIMPLEMENTED)
	#define USE_SysFileGenerateCRC3
	#define EXT_SysFileGenerateCRC3
	#define GET_SysFileGenerateCRC3(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGenerateCRC3(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGenerateCRC3  FALSE
	#define EXP_SysFileGenerateCRC3  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGenerateCRC3
	#define EXT_SysFileGenerateCRC3
	#define GET_SysFileGenerateCRC3(fl)  CAL_CMGETAPI( "SysFileGenerateCRC3" ) 
	#define CAL_SysFileGenerateCRC3  SysFileGenerateCRC3
	#define CHK_SysFileGenerateCRC3  TRUE
	#define EXP_SysFileGenerateCRC3  CAL_CMEXPAPI( "SysFileGenerateCRC3" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGenerateCRC3
	#define EXT_SysFileGenerateCRC3
	#define GET_SysFileGenerateCRC3(fl)  CAL_CMGETAPI( "SysFileGenerateCRC3" ) 
	#define CAL_SysFileGenerateCRC3  SysFileGenerateCRC3
	#define CHK_SysFileGenerateCRC3  TRUE
	#define EXP_SysFileGenerateCRC3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGenerateCRC3", (RTS_UINTPTR)SysFileGenerateCRC3, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGenerateCRC3
	#define EXT_SysFileSysFileGenerateCRC3
	#define GET_SysFileSysFileGenerateCRC3  ERR_OK
	#define CAL_SysFileSysFileGenerateCRC3 pISysFile->ISysFileGenerateCRC3
	#define CHK_SysFileSysFileGenerateCRC3 (pISysFile != NULL)
	#define EXP_SysFileSysFileGenerateCRC3  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGenerateCRC3
	#define EXT_SysFileGenerateCRC3
	#define GET_SysFileGenerateCRC3(fl)  CAL_CMGETAPI( "SysFileGenerateCRC3" ) 
	#define CAL_SysFileGenerateCRC3 pISysFile->ISysFileGenerateCRC3
	#define CHK_SysFileGenerateCRC3 (pISysFile != NULL)
	#define EXP_SysFileGenerateCRC3  CAL_CMEXPAPI( "SysFileGenerateCRC3" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGenerateCRC3  PFSYSFILEGENERATECRC3 pfSysFileGenerateCRC3;
	#define EXT_SysFileGenerateCRC3  extern PFSYSFILEGENERATECRC3 pfSysFileGenerateCRC3;
	#define GET_SysFileGenerateCRC3(fl)  s_pfCMGetAPI2( "SysFileGenerateCRC3", (RTS_VOID_FCTPTR *)&pfSysFileGenerateCRC3, (fl), 0, 0)
	#define CAL_SysFileGenerateCRC3  pfSysFileGenerateCRC3
	#define CHK_SysFileGenerateCRC3  (pfSysFileGenerateCRC3 != NULL)
	#define EXP_SysFileGenerateCRC3  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGenerateCRC3", (RTS_UINTPTR)SysFileGenerateCRC3, 0, 0) 
#endif




/**
 * <description>
 *	Generate the CRC32 of a file. Can be used to check file integrity. No standard path will be added.
 * </description>
 * <param name="pszFile" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="ulSize" type="IN">Size to calculate checksum. 0 if real size of file should be used [default]</param>
 * <param name="pulCRC" type="OUT">Pointer to get CRC32 result</param>
 * <param name="bUseCache" type="IN">Flag whether to use the SysFileMap cache. In case there is no cache this flag is ignored.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileGenerateCRC3_(char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC, RTS_BOOL bUseCache);
typedef RTS_RESULT (CDECL * PFSYSFILEGENERATECRC3_) (char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC, RTS_BOOL bUseCache);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGENERATECRC3__NOTIMPLEMENTED)
	#define USE_SysFileGenerateCRC3_
	#define EXT_SysFileGenerateCRC3_
	#define GET_SysFileGenerateCRC3_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGenerateCRC3_(p0,p1,p2,p3)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGenerateCRC3_  FALSE
	#define EXP_SysFileGenerateCRC3_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGenerateCRC3_
	#define EXT_SysFileGenerateCRC3_
	#define GET_SysFileGenerateCRC3_(fl)  CAL_CMGETAPI( "SysFileGenerateCRC3_" ) 
	#define CAL_SysFileGenerateCRC3_  SysFileGenerateCRC3_
	#define CHK_SysFileGenerateCRC3_  TRUE
	#define EXP_SysFileGenerateCRC3_  CAL_CMEXPAPI( "SysFileGenerateCRC3_" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGenerateCRC3_
	#define EXT_SysFileGenerateCRC3_
	#define GET_SysFileGenerateCRC3_(fl)  CAL_CMGETAPI( "SysFileGenerateCRC3_" ) 
	#define CAL_SysFileGenerateCRC3_  SysFileGenerateCRC3_
	#define CHK_SysFileGenerateCRC3_  TRUE
	#define EXP_SysFileGenerateCRC3_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGenerateCRC3_", (RTS_UINTPTR)SysFileGenerateCRC3_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGenerateCRC3_
	#define EXT_SysFileSysFileGenerateCRC3_
	#define GET_SysFileSysFileGenerateCRC3_  ERR_OK
	#define CAL_SysFileSysFileGenerateCRC3_ pISysFile->ISysFileGenerateCRC3_
	#define CHK_SysFileSysFileGenerateCRC3_ (pISysFile != NULL)
	#define EXP_SysFileSysFileGenerateCRC3_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGenerateCRC3_
	#define EXT_SysFileGenerateCRC3_
	#define GET_SysFileGenerateCRC3_(fl)  CAL_CMGETAPI( "SysFileGenerateCRC3_" ) 
	#define CAL_SysFileGenerateCRC3_ pISysFile->ISysFileGenerateCRC3_
	#define CHK_SysFileGenerateCRC3_ (pISysFile != NULL)
	#define EXP_SysFileGenerateCRC3_  CAL_CMEXPAPI( "SysFileGenerateCRC3_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGenerateCRC3_  PFSYSFILEGENERATECRC3_ pfSysFileGenerateCRC3_;
	#define EXT_SysFileGenerateCRC3_  extern PFSYSFILEGENERATECRC3_ pfSysFileGenerateCRC3_;
	#define GET_SysFileGenerateCRC3_(fl)  s_pfCMGetAPI2( "SysFileGenerateCRC3_", (RTS_VOID_FCTPTR *)&pfSysFileGenerateCRC3_, (fl), 0, 0)
	#define CAL_SysFileGenerateCRC3_  pfSysFileGenerateCRC3_
	#define CHK_SysFileGenerateCRC3_  (pfSysFileGenerateCRC3_ != NULL)
	#define EXP_SysFileGenerateCRC3_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGenerateCRC3_", (RTS_UINTPTR)SysFileGenerateCRC3_, 0, 0) 
#endif




/**
 * <description>
 *	Generate the CRC32 of a file. Can be used to check file integrity.
 *	IMPLEMENTATION NOTE:
 *	This interface function is implemented operating system dependent! Optimizations can be done here.
 * </description>
 * <param name="pszFile" type="IN">File name. Can contain an absolute or relative path</param>
 * <param name="ulSize" type="IN">Size to calculate checksum. 0 if real size of file should be used [default]</param>
 * <param name="pulCRC" type="OUT">Pointer to get CRC32 result</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysFileGenerateCRC2_(char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC);
typedef RTS_RESULT (CDECL * PFSYSFILEGENERATECRC2_) (char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGENERATECRC2__NOTIMPLEMENTED)
	#define USE_SysFileGenerateCRC2_
	#define EXT_SysFileGenerateCRC2_
	#define GET_SysFileGenerateCRC2_(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGenerateCRC2_(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGenerateCRC2_  FALSE
	#define EXP_SysFileGenerateCRC2_  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGenerateCRC2_
	#define EXT_SysFileGenerateCRC2_
	#define GET_SysFileGenerateCRC2_(fl)  CAL_CMGETAPI( "SysFileGenerateCRC2_" ) 
	#define CAL_SysFileGenerateCRC2_  SysFileGenerateCRC2_
	#define CHK_SysFileGenerateCRC2_  TRUE
	#define EXP_SysFileGenerateCRC2_  CAL_CMEXPAPI( "SysFileGenerateCRC2_" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGenerateCRC2_
	#define EXT_SysFileGenerateCRC2_
	#define GET_SysFileGenerateCRC2_(fl)  CAL_CMGETAPI( "SysFileGenerateCRC2_" ) 
	#define CAL_SysFileGenerateCRC2_  SysFileGenerateCRC2_
	#define CHK_SysFileGenerateCRC2_  TRUE
	#define EXP_SysFileGenerateCRC2_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGenerateCRC2_", (RTS_UINTPTR)SysFileGenerateCRC2_, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGenerateCRC2_
	#define EXT_SysFileSysFileGenerateCRC2_
	#define GET_SysFileSysFileGenerateCRC2_  ERR_OK
	#define CAL_SysFileSysFileGenerateCRC2_ pISysFile->ISysFileGenerateCRC2_
	#define CHK_SysFileSysFileGenerateCRC2_ (pISysFile != NULL)
	#define EXP_SysFileSysFileGenerateCRC2_  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGenerateCRC2_
	#define EXT_SysFileGenerateCRC2_
	#define GET_SysFileGenerateCRC2_(fl)  CAL_CMGETAPI( "SysFileGenerateCRC2_" ) 
	#define CAL_SysFileGenerateCRC2_ pISysFile->ISysFileGenerateCRC2_
	#define CHK_SysFileGenerateCRC2_ (pISysFile != NULL)
	#define EXP_SysFileGenerateCRC2_  CAL_CMEXPAPI( "SysFileGenerateCRC2_" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGenerateCRC2_  PFSYSFILEGENERATECRC2_ pfSysFileGenerateCRC2_;
	#define EXT_SysFileGenerateCRC2_  extern PFSYSFILEGENERATECRC2_ pfSysFileGenerateCRC2_;
	#define GET_SysFileGenerateCRC2_(fl)  s_pfCMGetAPI2( "SysFileGenerateCRC2_", (RTS_VOID_FCTPTR *)&pfSysFileGenerateCRC2_, (fl), 0, 0)
	#define CAL_SysFileGenerateCRC2_  pfSysFileGenerateCRC2_
	#define CHK_SysFileGenerateCRC2_  (pfSysFileGenerateCRC2_ != NULL)
	#define EXP_SysFileGenerateCRC2_  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGenerateCRC2_", (RTS_UINTPTR)SysFileGenerateCRC2_, 0, 0) 
#endif




/**
 * <description>
 *	Flush the file cache and write into the file.
 * </description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <result>Error code:
 *	<ul>
 *		<li>ERR_OK: Succeeded flushing the file</li>
 *		<li>ERR_FAILED: Error occurred during file flush</li>
 *		<li>ERR_NOTIMPLEMENTED: File flush is not implemented</li>
 *		<li>ERR_NOT_SUPPORTED: File flush not available on the target</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL SysFileFlush(RTS_HANDLE hFile);
typedef RTS_RESULT (CDECL * PFSYSFILEFLUSH) (RTS_HANDLE hFile);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEFLUSH_NOTIMPLEMENTED)
	#define USE_SysFileFlush
	#define EXT_SysFileFlush
	#define GET_SysFileFlush(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileFlush(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileFlush  FALSE
	#define EXP_SysFileFlush  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileFlush
	#define EXT_SysFileFlush
	#define GET_SysFileFlush(fl)  CAL_CMGETAPI( "SysFileFlush" ) 
	#define CAL_SysFileFlush  SysFileFlush
	#define CHK_SysFileFlush  TRUE
	#define EXP_SysFileFlush  CAL_CMEXPAPI( "SysFileFlush" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileFlush
	#define EXT_SysFileFlush
	#define GET_SysFileFlush(fl)  CAL_CMGETAPI( "SysFileFlush" ) 
	#define CAL_SysFileFlush  SysFileFlush
	#define CHK_SysFileFlush  TRUE
	#define EXP_SysFileFlush  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileFlush", (RTS_UINTPTR)SysFileFlush, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileFlush
	#define EXT_SysFileSysFileFlush
	#define GET_SysFileSysFileFlush  ERR_OK
	#define CAL_SysFileSysFileFlush(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileFlush() : ((ISysFile*)p0)->ISysFileFlush())
	#define CHK_SysFileSysFileFlush  (pISysFile != NULL)
	#define EXP_SysFileSysFileFlush  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileFlush
	#define EXT_SysFileFlush
	#define GET_SysFileFlush(fl)  CAL_CMGETAPI( "SysFileFlush" ) 
	#define CAL_SysFileFlush(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileFlush() : ((ISysFile*)p0)->ISysFileFlush())
	#define CHK_SysFileFlush  (pISysFile != NULL)
	#define EXP_SysFileFlush  CAL_CMEXPAPI( "SysFileFlush" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileFlush  PFSYSFILEFLUSH pfSysFileFlush;
	#define EXT_SysFileFlush  extern PFSYSFILEFLUSH pfSysFileFlush;
	#define GET_SysFileFlush(fl)  s_pfCMGetAPI2( "SysFileFlush", (RTS_VOID_FCTPTR *)&pfSysFileFlush, (fl), 0, 0)
	#define CAL_SysFileFlush  pfSysFileFlush
	#define CHK_SysFileFlush  (pfSysFileFlush != NULL)
	#define EXP_SysFileFlush  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileFlush", (RTS_UINTPTR)SysFileFlush, 0, 0) 
#endif




/**
 * <description>
 *	Interface function to check, if the filename has a prefix that marked that this file must be invisible outside the runtime system (e.g. in CmpFileTransfer).
 * </description>
 * <param name="pszFileName" type="IN">HPointer to the filename to check</param>
 * <result>Error code:
 *	<ul>
 *		<li>ERR_OK: Invisible file! Must be invisible outside the runtime system (e.g. in CmpFileTransfer)</li>
 *		<li>ERR_FAILED: No invisible file</li>
 *		<li>ERR_PARAMETER: Invalid parameter pszFileName</li>
 *	</ul>
 * </result>
 */
RTS_RESULT CDECL SysFileIsInvisible(char *pszFileName);
typedef RTS_RESULT (CDECL * PFSYSFILEISINVISIBLE) (char *pszFileName);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEISINVISIBLE_NOTIMPLEMENTED)
	#define USE_SysFileIsInvisible
	#define EXT_SysFileIsInvisible
	#define GET_SysFileIsInvisible(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileIsInvisible(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileIsInvisible  FALSE
	#define EXP_SysFileIsInvisible  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileIsInvisible
	#define EXT_SysFileIsInvisible
	#define GET_SysFileIsInvisible(fl)  CAL_CMGETAPI( "SysFileIsInvisible" ) 
	#define CAL_SysFileIsInvisible  SysFileIsInvisible
	#define CHK_SysFileIsInvisible  TRUE
	#define EXP_SysFileIsInvisible  CAL_CMEXPAPI( "SysFileIsInvisible" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileIsInvisible
	#define EXT_SysFileIsInvisible
	#define GET_SysFileIsInvisible(fl)  CAL_CMGETAPI( "SysFileIsInvisible" ) 
	#define CAL_SysFileIsInvisible  SysFileIsInvisible
	#define CHK_SysFileIsInvisible  TRUE
	#define EXP_SysFileIsInvisible  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileIsInvisible", (RTS_UINTPTR)SysFileIsInvisible, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileIsInvisible
	#define EXT_SysFileSysFileIsInvisible
	#define GET_SysFileSysFileIsInvisible  ERR_OK
	#define CAL_SysFileSysFileIsInvisible pISysFile->ISysFileIsInvisible
	#define CHK_SysFileSysFileIsInvisible (pISysFile != NULL)
	#define EXP_SysFileSysFileIsInvisible  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileIsInvisible
	#define EXT_SysFileIsInvisible
	#define GET_SysFileIsInvisible(fl)  CAL_CMGETAPI( "SysFileIsInvisible" ) 
	#define CAL_SysFileIsInvisible pISysFile->ISysFileIsInvisible
	#define CHK_SysFileIsInvisible (pISysFile != NULL)
	#define EXP_SysFileIsInvisible  CAL_CMEXPAPI( "SysFileIsInvisible" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileIsInvisible  PFSYSFILEISINVISIBLE pfSysFileIsInvisible;
	#define EXT_SysFileIsInvisible  extern PFSYSFILEISINVISIBLE pfSysFileIsInvisible;
	#define GET_SysFileIsInvisible(fl)  s_pfCMGetAPI2( "SysFileIsInvisible", (RTS_VOID_FCTPTR *)&pfSysFileIsInvisible, (fl), 0, 0)
	#define CAL_SysFileIsInvisible  pfSysFileIsInvisible
	#define CHK_SysFileIsInvisible  (pfSysFileIsInvisible != NULL)
	#define EXP_SysFileIsInvisible  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileIsInvisible", (RTS_UINTPTR)SysFileIsInvisible, 0, 0) 
#endif




/**
 * <description>
 *	Set the size of the actual opened file.
 *  Used for increasing/decreasing the size of a file.
 *  The current file offset (file pointer) is not changed. 
 * </description>
 * <param name="hFile" type="IN">Handle to the file</param>
 * <param name="sSizeNew" type="IN">New size of the file, type: RTS_SIZE </param>
 * <result>Error code.</result>
 */
RTS_RESULT CDECL SysFileTruncate(RTS_HANDLE hFile, RTS_SIZE sSizeNew);
typedef RTS_RESULT (CDECL * PFSYSFILETRUNCATE) (RTS_HANDLE hFile, RTS_SIZE sSizeNew);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILETRUNCATE_NOTIMPLEMENTED)
	#define USE_SysFileTruncate
	#define EXT_SysFileTruncate
	#define GET_SysFileTruncate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileTruncate(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileTruncate  FALSE
	#define EXP_SysFileTruncate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileTruncate
	#define EXT_SysFileTruncate
	#define GET_SysFileTruncate(fl)  CAL_CMGETAPI( "SysFileTruncate" ) 
	#define CAL_SysFileTruncate  SysFileTruncate
	#define CHK_SysFileTruncate  TRUE
	#define EXP_SysFileTruncate  CAL_CMEXPAPI( "SysFileTruncate" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileTruncate
	#define EXT_SysFileTruncate
	#define GET_SysFileTruncate(fl)  CAL_CMGETAPI( "SysFileTruncate" ) 
	#define CAL_SysFileTruncate  SysFileTruncate
	#define CHK_SysFileTruncate  TRUE
	#define EXP_SysFileTruncate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileTruncate", (RTS_UINTPTR)SysFileTruncate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileTruncate
	#define EXT_SysFileSysFileTruncate
	#define GET_SysFileSysFileTruncate  ERR_OK
	#define CAL_SysFileSysFileTruncate(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileTruncate(p1) : ((ISysFile*)p0)->ISysFileTruncate(p1))
	#define CHK_SysFileSysFileTruncate  (pISysFile != NULL)
	#define EXP_SysFileSysFileTruncate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileTruncate
	#define EXT_SysFileTruncate
	#define GET_SysFileTruncate(fl)  CAL_CMGETAPI( "SysFileTruncate" ) 
	#define CAL_SysFileTruncate(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysFile->ISysFileTruncate(p1) : ((ISysFile*)p0)->ISysFileTruncate(p1))
	#define CHK_SysFileTruncate  (pISysFile != NULL)
	#define EXP_SysFileTruncate  CAL_CMEXPAPI( "SysFileTruncate" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileTruncate  PFSYSFILETRUNCATE pfSysFileTruncate;
	#define EXT_SysFileTruncate  extern PFSYSFILETRUNCATE pfSysFileTruncate;
	#define GET_SysFileTruncate(fl)  s_pfCMGetAPI2( "SysFileTruncate", (RTS_VOID_FCTPTR *)&pfSysFileTruncate, (fl), 0, 0)
	#define CAL_SysFileTruncate  pfSysFileTruncate
	#define CHK_SysFileTruncate  (pfSysFileTruncate != NULL)
	#define EXP_SysFileTruncate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileTruncate", (RTS_UINTPTR)SysFileTruncate, 0, 0) 
#endif




/**
 * <description>
 * Returns the first configured path and a handle for further processing.
 * </description>
 * <param name="pszFilePath" type="OUT">File path</param>
 * <param name="nMaxFilePathLen" type="IN">Max size of pszFilePath</param>
 * <param name="pszPlaceholder" type="OUT">Name of placeholder</param>
 * <param name="nMaxPlaceholderLen" type="IN">Max size of pszPlaceholder</param>
 * <param name="pResult" type="OUT">Error code</param>
 * <result>Handle</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">A placeholder and its path are successfully found and returned</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter (pointer NULL or length 0)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_END_OF_OBJECT">No entries found. The referenced buffers of pszFilePath and pszPlaceholder are left untouched.</errorcode>
 */
RTS_HANDLE CDECL SysFileGetFirstConfiguredPath(char *pszFilePath, RTS_I32 nMaxFilePathLen, char *pszPlaceholder, RTS_I32 nMaxPlaceholderLen, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSFILEGETFIRSTCONFIGUREDPATH) (char *pszFilePath, RTS_I32 nMaxFilePathLen, char *pszPlaceholder, RTS_I32 nMaxPlaceholderLen, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETFIRSTCONFIGUREDPATH_NOTIMPLEMENTED)
	#define USE_SysFileGetFirstConfiguredPath
	#define EXT_SysFileGetFirstConfiguredPath
	#define GET_SysFileGetFirstConfiguredPath(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetFirstConfiguredPath(p0,p1,p2,p3,p4)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysFileGetFirstConfiguredPath  FALSE
	#define EXP_SysFileGetFirstConfiguredPath  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetFirstConfiguredPath
	#define EXT_SysFileGetFirstConfiguredPath
	#define GET_SysFileGetFirstConfiguredPath(fl)  CAL_CMGETAPI( "SysFileGetFirstConfiguredPath" ) 
	#define CAL_SysFileGetFirstConfiguredPath  SysFileGetFirstConfiguredPath
	#define CHK_SysFileGetFirstConfiguredPath  TRUE
	#define EXP_SysFileGetFirstConfiguredPath  CAL_CMEXPAPI( "SysFileGetFirstConfiguredPath" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetFirstConfiguredPath
	#define EXT_SysFileGetFirstConfiguredPath
	#define GET_SysFileGetFirstConfiguredPath(fl)  CAL_CMGETAPI( "SysFileGetFirstConfiguredPath" ) 
	#define CAL_SysFileGetFirstConfiguredPath  SysFileGetFirstConfiguredPath
	#define CHK_SysFileGetFirstConfiguredPath  TRUE
	#define EXP_SysFileGetFirstConfiguredPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetFirstConfiguredPath", (RTS_UINTPTR)SysFileGetFirstConfiguredPath, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetFirstConfiguredPath
	#define EXT_SysFileSysFileGetFirstConfiguredPath
	#define GET_SysFileSysFileGetFirstConfiguredPath  ERR_OK
	#define CAL_SysFileSysFileGetFirstConfiguredPath pISysFile->ISysFileGetFirstConfiguredPath
	#define CHK_SysFileSysFileGetFirstConfiguredPath (pISysFile != NULL)
	#define EXP_SysFileSysFileGetFirstConfiguredPath  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetFirstConfiguredPath
	#define EXT_SysFileGetFirstConfiguredPath
	#define GET_SysFileGetFirstConfiguredPath(fl)  CAL_CMGETAPI( "SysFileGetFirstConfiguredPath" ) 
	#define CAL_SysFileGetFirstConfiguredPath pISysFile->ISysFileGetFirstConfiguredPath
	#define CHK_SysFileGetFirstConfiguredPath (pISysFile != NULL)
	#define EXP_SysFileGetFirstConfiguredPath  CAL_CMEXPAPI( "SysFileGetFirstConfiguredPath" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetFirstConfiguredPath  PFSYSFILEGETFIRSTCONFIGUREDPATH pfSysFileGetFirstConfiguredPath;
	#define EXT_SysFileGetFirstConfiguredPath  extern PFSYSFILEGETFIRSTCONFIGUREDPATH pfSysFileGetFirstConfiguredPath;
	#define GET_SysFileGetFirstConfiguredPath(fl)  s_pfCMGetAPI2( "SysFileGetFirstConfiguredPath", (RTS_VOID_FCTPTR *)&pfSysFileGetFirstConfiguredPath, (fl), 0, 0)
	#define CAL_SysFileGetFirstConfiguredPath  pfSysFileGetFirstConfiguredPath
	#define CHK_SysFileGetFirstConfiguredPath  (pfSysFileGetFirstConfiguredPath != NULL)
	#define EXP_SysFileGetFirstConfiguredPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetFirstConfiguredPath", (RTS_UINTPTR)SysFileGetFirstConfiguredPath, 0, 0) 
#endif




/**
 * <description>
 * Returns the next configured path and a handle for further processing.
 * </description>
 * <param name="hPrevPath" type="IN">Handle from previous call</param>
 * <param name="pszFilePath" type="OUT">File path</param>
 * <param name="nMaxFilePathLen" type="IN">Max size of pszFilePath</param>
 * <param name="pszPlaceholder" type="OUT">Name of placeholder</param>
 * <param name="nMaxPlaceholderLen" type="IN">Max size of pszPlaceholder</param>
 * <param name="pResult" type="OUT">Error code</param>
 * <result>Handle</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">A placeholder and its path are successfully found and returned.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter (pointer NULL or length 0)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_END_OF_OBJECT">No more entries found. The referenced buffers of pszFilePath and pszPlaceholder are left untouched.</errorcode>
 */
RTS_HANDLE CDECL SysFileGetNextConfiguredPath(RTS_HANDLE hPrevPath, char *pszFilePath, RTS_I32 nMaxFilePathLen, char *pszPlaceholder, RTS_I32 nMaxPlaceholderLen, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSFILEGETNEXTCONFIGUREDPATH) (RTS_HANDLE hPrevPath, char *pszFilePath, RTS_I32 nMaxFilePathLen, char *pszPlaceholder, RTS_I32 nMaxPlaceholderLen, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETNEXTCONFIGUREDPATH_NOTIMPLEMENTED)
	#define USE_SysFileGetNextConfiguredPath
	#define EXT_SysFileGetNextConfiguredPath
	#define GET_SysFileGetNextConfiguredPath(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetNextConfiguredPath(p0,p1,p2,p3,p4,p5)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysFileGetNextConfiguredPath  FALSE
	#define EXP_SysFileGetNextConfiguredPath  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetNextConfiguredPath
	#define EXT_SysFileGetNextConfiguredPath
	#define GET_SysFileGetNextConfiguredPath(fl)  CAL_CMGETAPI( "SysFileGetNextConfiguredPath" ) 
	#define CAL_SysFileGetNextConfiguredPath  SysFileGetNextConfiguredPath
	#define CHK_SysFileGetNextConfiguredPath  TRUE
	#define EXP_SysFileGetNextConfiguredPath  CAL_CMEXPAPI( "SysFileGetNextConfiguredPath" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetNextConfiguredPath
	#define EXT_SysFileGetNextConfiguredPath
	#define GET_SysFileGetNextConfiguredPath(fl)  CAL_CMGETAPI( "SysFileGetNextConfiguredPath" ) 
	#define CAL_SysFileGetNextConfiguredPath  SysFileGetNextConfiguredPath
	#define CHK_SysFileGetNextConfiguredPath  TRUE
	#define EXP_SysFileGetNextConfiguredPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetNextConfiguredPath", (RTS_UINTPTR)SysFileGetNextConfiguredPath, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetNextConfiguredPath
	#define EXT_SysFileSysFileGetNextConfiguredPath
	#define GET_SysFileSysFileGetNextConfiguredPath  ERR_OK
	#define CAL_SysFileSysFileGetNextConfiguredPath pISysFile->ISysFileGetNextConfiguredPath
	#define CHK_SysFileSysFileGetNextConfiguredPath (pISysFile != NULL)
	#define EXP_SysFileSysFileGetNextConfiguredPath  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetNextConfiguredPath
	#define EXT_SysFileGetNextConfiguredPath
	#define GET_SysFileGetNextConfiguredPath(fl)  CAL_CMGETAPI( "SysFileGetNextConfiguredPath" ) 
	#define CAL_SysFileGetNextConfiguredPath pISysFile->ISysFileGetNextConfiguredPath
	#define CHK_SysFileGetNextConfiguredPath (pISysFile != NULL)
	#define EXP_SysFileGetNextConfiguredPath  CAL_CMEXPAPI( "SysFileGetNextConfiguredPath" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetNextConfiguredPath  PFSYSFILEGETNEXTCONFIGUREDPATH pfSysFileGetNextConfiguredPath;
	#define EXT_SysFileGetNextConfiguredPath  extern PFSYSFILEGETNEXTCONFIGUREDPATH pfSysFileGetNextConfiguredPath;
	#define GET_SysFileGetNextConfiguredPath(fl)  s_pfCMGetAPI2( "SysFileGetNextConfiguredPath", (RTS_VOID_FCTPTR *)&pfSysFileGetNextConfiguredPath, (fl), 0, 0)
	#define CAL_SysFileGetNextConfiguredPath  pfSysFileGetNextConfiguredPath
	#define CHK_SysFileGetNextConfiguredPath  (pfSysFileGetNextConfiguredPath != NULL)
	#define EXP_SysFileGetNextConfiguredPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetNextConfiguredPath", (RTS_UINTPTR)SysFileGetNextConfiguredPath, 0, 0) 
#endif




/**
 * <description>
 * Returns the next configured path and a handle for further processing.
 * </description>
 * <param name="hPrevPath" type="IN">Handle from previous call</param>
 * <param name="pszFilePath" type="OUT">File path</param>
 * <param name="nMaxFilePathLen" type="IN">Max size of pszFilePath</param>
 * <param name="pszPlaceholder" type="OUT">Name of placeholder</param>
 * <param name="nMaxPlaceholderLen" type="IN">Max size of pszPlaceholder</param>
 * <param name="nFlags" type="IN">See Flags for SysFileGetNextConfiguredPath2</param>
 * <param name="pResult" type="OUT">Error code</param>
 * <result>Handle</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">A placeholder and its path are successfully found and returned.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Invalid parameter (pointer NULL or length 0)</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_END_OF_OBJECT">No more entries found. The referenced buffers of pszFilePath and pszPlaceholder are left untouched.</errorcode>
 */
RTS_HANDLE CDECL SysFileGetNextConfiguredPath2(RTS_HANDLE hPrevPath, char *pszFilePath, RTS_I32 nMaxFilePathLen, char *pszPlaceholder, RTS_I32 nMaxPlaceholderLen, RTS_UI32 nFlags, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSFILEGETNEXTCONFIGUREDPATH2) (RTS_HANDLE hPrevPath, char *pszFilePath, RTS_I32 nMaxFilePathLen, char *pszPlaceholder, RTS_I32 nMaxPlaceholderLen, RTS_UI32 nFlags, RTS_RESULT *pResult);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETNEXTCONFIGUREDPATH2_NOTIMPLEMENTED)
	#define USE_SysFileGetNextConfiguredPath2
	#define EXT_SysFileGetNextConfiguredPath2
	#define GET_SysFileGetNextConfiguredPath2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetNextConfiguredPath2(p0,p1,p2,p3,p4,p5,p6)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysFileGetNextConfiguredPath2  FALSE
	#define EXP_SysFileGetNextConfiguredPath2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetNextConfiguredPath2
	#define EXT_SysFileGetNextConfiguredPath2
	#define GET_SysFileGetNextConfiguredPath2(fl)  CAL_CMGETAPI( "SysFileGetNextConfiguredPath2" ) 
	#define CAL_SysFileGetNextConfiguredPath2  SysFileGetNextConfiguredPath2
	#define CHK_SysFileGetNextConfiguredPath2  TRUE
	#define EXP_SysFileGetNextConfiguredPath2  CAL_CMEXPAPI( "SysFileGetNextConfiguredPath2" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetNextConfiguredPath2
	#define EXT_SysFileGetNextConfiguredPath2
	#define GET_SysFileGetNextConfiguredPath2(fl)  CAL_CMGETAPI( "SysFileGetNextConfiguredPath2" ) 
	#define CAL_SysFileGetNextConfiguredPath2  SysFileGetNextConfiguredPath2
	#define CHK_SysFileGetNextConfiguredPath2  TRUE
	#define EXP_SysFileGetNextConfiguredPath2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetNextConfiguredPath2", (RTS_UINTPTR)SysFileGetNextConfiguredPath2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetNextConfiguredPath2
	#define EXT_SysFileSysFileGetNextConfiguredPath2
	#define GET_SysFileSysFileGetNextConfiguredPath2  ERR_OK
	#define CAL_SysFileSysFileGetNextConfiguredPath2 pISysFile->ISysFileGetNextConfiguredPath2
	#define CHK_SysFileSysFileGetNextConfiguredPath2 (pISysFile != NULL)
	#define EXP_SysFileSysFileGetNextConfiguredPath2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetNextConfiguredPath2
	#define EXT_SysFileGetNextConfiguredPath2
	#define GET_SysFileGetNextConfiguredPath2(fl)  CAL_CMGETAPI( "SysFileGetNextConfiguredPath2" ) 
	#define CAL_SysFileGetNextConfiguredPath2 pISysFile->ISysFileGetNextConfiguredPath2
	#define CHK_SysFileGetNextConfiguredPath2 (pISysFile != NULL)
	#define EXP_SysFileGetNextConfiguredPath2  CAL_CMEXPAPI( "SysFileGetNextConfiguredPath2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetNextConfiguredPath2  PFSYSFILEGETNEXTCONFIGUREDPATH2 pfSysFileGetNextConfiguredPath2;
	#define EXT_SysFileGetNextConfiguredPath2  extern PFSYSFILEGETNEXTCONFIGUREDPATH2 pfSysFileGetNextConfiguredPath2;
	#define GET_SysFileGetNextConfiguredPath2(fl)  s_pfCMGetAPI2( "SysFileGetNextConfiguredPath2", (RTS_VOID_FCTPTR *)&pfSysFileGetNextConfiguredPath2, (fl), 0, 0)
	#define CAL_SysFileGetNextConfiguredPath2  pfSysFileGetNextConfiguredPath2
	#define CHK_SysFileGetNextConfiguredPath2  (pfSysFileGetNextConfiguredPath2 != NULL)
	#define EXP_SysFileGetNextConfiguredPath2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetNextConfiguredPath2", (RTS_UINTPTR)SysFileGetNextConfiguredPath2, 0, 0) 
#endif




/**
 * <description>
 * Register a file placeholder.
 * NOTE:
 * A placeholder must be documented with the category "File placeholder".
 * </description>
 * <param name="pszPlaceholder" type="IN">Placeholder name. Must be surrounded by a "$". Example: "$MyPlaceholder$"</param>
 * <param name="pszFolder" type="IN">Default folder of the placeholder</param> 
 * <param name="attribute" type="IN">Attribute. See "File placeholder attributes"</param>  
 * <result>Error code</result>
 */
RTS_RESULT CDECL SysFileRegisterPlaceholder(char* pszPlaceholder, char* pszFolder, RTS_UI8 attribute);
typedef RTS_RESULT (CDECL * PFSYSFILEREGISTERPLACEHOLDER) (char* pszPlaceholder, char* pszFolder, RTS_UI8 attribute);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEREGISTERPLACEHOLDER_NOTIMPLEMENTED)
	#define USE_SysFileRegisterPlaceholder
	#define EXT_SysFileRegisterPlaceholder
	#define GET_SysFileRegisterPlaceholder(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileRegisterPlaceholder(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileRegisterPlaceholder  FALSE
	#define EXP_SysFileRegisterPlaceholder  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileRegisterPlaceholder
	#define EXT_SysFileRegisterPlaceholder
	#define GET_SysFileRegisterPlaceholder(fl)  CAL_CMGETAPI( "SysFileRegisterPlaceholder" ) 
	#define CAL_SysFileRegisterPlaceholder  SysFileRegisterPlaceholder
	#define CHK_SysFileRegisterPlaceholder  TRUE
	#define EXP_SysFileRegisterPlaceholder  CAL_CMEXPAPI( "SysFileRegisterPlaceholder" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileRegisterPlaceholder
	#define EXT_SysFileRegisterPlaceholder
	#define GET_SysFileRegisterPlaceholder(fl)  CAL_CMGETAPI( "SysFileRegisterPlaceholder" ) 
	#define CAL_SysFileRegisterPlaceholder  SysFileRegisterPlaceholder
	#define CHK_SysFileRegisterPlaceholder  TRUE
	#define EXP_SysFileRegisterPlaceholder  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileRegisterPlaceholder", (RTS_UINTPTR)SysFileRegisterPlaceholder, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileRegisterPlaceholder
	#define EXT_SysFileSysFileRegisterPlaceholder
	#define GET_SysFileSysFileRegisterPlaceholder  ERR_OK
	#define CAL_SysFileSysFileRegisterPlaceholder pISysFile->ISysFileRegisterPlaceholder
	#define CHK_SysFileSysFileRegisterPlaceholder (pISysFile != NULL)
	#define EXP_SysFileSysFileRegisterPlaceholder  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileRegisterPlaceholder
	#define EXT_SysFileRegisterPlaceholder
	#define GET_SysFileRegisterPlaceholder(fl)  CAL_CMGETAPI( "SysFileRegisterPlaceholder" ) 
	#define CAL_SysFileRegisterPlaceholder pISysFile->ISysFileRegisterPlaceholder
	#define CHK_SysFileRegisterPlaceholder (pISysFile != NULL)
	#define EXP_SysFileRegisterPlaceholder  CAL_CMEXPAPI( "SysFileRegisterPlaceholder" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileRegisterPlaceholder  PFSYSFILEREGISTERPLACEHOLDER pfSysFileRegisterPlaceholder;
	#define EXT_SysFileRegisterPlaceholder  extern PFSYSFILEREGISTERPLACEHOLDER pfSysFileRegisterPlaceholder;
	#define GET_SysFileRegisterPlaceholder(fl)  s_pfCMGetAPI2( "SysFileRegisterPlaceholder", (RTS_VOID_FCTPTR *)&pfSysFileRegisterPlaceholder, (fl), 0, 0)
	#define CAL_SysFileRegisterPlaceholder  pfSysFileRegisterPlaceholder
	#define CHK_SysFileRegisterPlaceholder  (pfSysFileRegisterPlaceholder != NULL)
	#define EXP_SysFileRegisterPlaceholder  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileRegisterPlaceholder", (RTS_UINTPTR)SysFileRegisterPlaceholder, 0, 0) 
#endif




/**
 * <description>
 * Sets the standard path for all IEC file operations (IEC working directory). Reset with empty path.
 * </description>
 * <param name="pszPath" type="IN">Standard IEC file path</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL SysFileSetIecPath(char *pszPath);
typedef RTS_RESULT (CDECL * PFSYSFILESETIECPATH) (char *pszPath);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILESETIECPATH_NOTIMPLEMENTED)
	#define USE_SysFileSetIecPath
	#define EXT_SysFileSetIecPath
	#define GET_SysFileSetIecPath(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileSetIecPath(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileSetIecPath  FALSE
	#define EXP_SysFileSetIecPath  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileSetIecPath
	#define EXT_SysFileSetIecPath
	#define GET_SysFileSetIecPath(fl)  CAL_CMGETAPI( "SysFileSetIecPath" ) 
	#define CAL_SysFileSetIecPath  SysFileSetIecPath
	#define CHK_SysFileSetIecPath  TRUE
	#define EXP_SysFileSetIecPath  CAL_CMEXPAPI( "SysFileSetIecPath" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileSetIecPath
	#define EXT_SysFileSetIecPath
	#define GET_SysFileSetIecPath(fl)  CAL_CMGETAPI( "SysFileSetIecPath" ) 
	#define CAL_SysFileSetIecPath  SysFileSetIecPath
	#define CHK_SysFileSetIecPath  TRUE
	#define EXP_SysFileSetIecPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileSetIecPath", (RTS_UINTPTR)SysFileSetIecPath, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileSetIecPath
	#define EXT_SysFileSysFileSetIecPath
	#define GET_SysFileSysFileSetIecPath  ERR_OK
	#define CAL_SysFileSysFileSetIecPath pISysFile->ISysFileSetIecPath
	#define CHK_SysFileSysFileSetIecPath (pISysFile != NULL)
	#define EXP_SysFileSysFileSetIecPath  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileSetIecPath
	#define EXT_SysFileSetIecPath
	#define GET_SysFileSetIecPath(fl)  CAL_CMGETAPI( "SysFileSetIecPath" ) 
	#define CAL_SysFileSetIecPath pISysFile->ISysFileSetIecPath
	#define CHK_SysFileSetIecPath (pISysFile != NULL)
	#define EXP_SysFileSetIecPath  CAL_CMEXPAPI( "SysFileSetIecPath" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileSetIecPath  PFSYSFILESETIECPATH pfSysFileSetIecPath;
	#define EXT_SysFileSetIecPath  extern PFSYSFILESETIECPATH pfSysFileSetIecPath;
	#define GET_SysFileSetIecPath(fl)  s_pfCMGetAPI2( "SysFileSetIecPath", (RTS_VOID_FCTPTR *)&pfSysFileSetIecPath, (fl), 0, 0)
	#define CAL_SysFileSetIecPath  pfSysFileSetIecPath
	#define CHK_SysFileSetIecPath  (pfSysFileSetIecPath != NULL)
	#define EXP_SysFileSetIecPath  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileSetIecPath", (RTS_UINTPTR)SysFileSetIecPath, 0, 0) 
#endif




/**
 * <description>
 * Returns the PLC logic prefix.
 * </description>
 * <result>PLC logic prefix</result>
 */
char * CDECL SysFileGetPlcLogicPrefix(void);
typedef char * (CDECL * PFSYSFILEGETPLCLOGICPREFIX) (void);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEGETPLCLOGICPREFIX_NOTIMPLEMENTED)
	#define USE_SysFileGetPlcLogicPrefix
	#define EXT_SysFileGetPlcLogicPrefix
	#define GET_SysFileGetPlcLogicPrefix(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileGetPlcLogicPrefix()  (char *)ERR_NOTIMPLEMENTED
	#define CHK_SysFileGetPlcLogicPrefix  FALSE
	#define EXP_SysFileGetPlcLogicPrefix  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileGetPlcLogicPrefix
	#define EXT_SysFileGetPlcLogicPrefix
	#define GET_SysFileGetPlcLogicPrefix(fl)  CAL_CMGETAPI( "SysFileGetPlcLogicPrefix" ) 
	#define CAL_SysFileGetPlcLogicPrefix  SysFileGetPlcLogicPrefix
	#define CHK_SysFileGetPlcLogicPrefix  TRUE
	#define EXP_SysFileGetPlcLogicPrefix  CAL_CMEXPAPI( "SysFileGetPlcLogicPrefix" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileGetPlcLogicPrefix
	#define EXT_SysFileGetPlcLogicPrefix
	#define GET_SysFileGetPlcLogicPrefix(fl)  CAL_CMGETAPI( "SysFileGetPlcLogicPrefix" ) 
	#define CAL_SysFileGetPlcLogicPrefix  SysFileGetPlcLogicPrefix
	#define CHK_SysFileGetPlcLogicPrefix  TRUE
	#define EXP_SysFileGetPlcLogicPrefix  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetPlcLogicPrefix", (RTS_UINTPTR)SysFileGetPlcLogicPrefix, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileGetPlcLogicPrefix
	#define EXT_SysFileSysFileGetPlcLogicPrefix
	#define GET_SysFileSysFileGetPlcLogicPrefix  ERR_OK
	#define CAL_SysFileSysFileGetPlcLogicPrefix pISysFile->ISysFileGetPlcLogicPrefix
	#define CHK_SysFileSysFileGetPlcLogicPrefix (pISysFile != NULL)
	#define EXP_SysFileSysFileGetPlcLogicPrefix  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileGetPlcLogicPrefix
	#define EXT_SysFileGetPlcLogicPrefix
	#define GET_SysFileGetPlcLogicPrefix(fl)  CAL_CMGETAPI( "SysFileGetPlcLogicPrefix" ) 
	#define CAL_SysFileGetPlcLogicPrefix pISysFile->ISysFileGetPlcLogicPrefix
	#define CHK_SysFileGetPlcLogicPrefix (pISysFile != NULL)
	#define EXP_SysFileGetPlcLogicPrefix  CAL_CMEXPAPI( "SysFileGetPlcLogicPrefix" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileGetPlcLogicPrefix  PFSYSFILEGETPLCLOGICPREFIX pfSysFileGetPlcLogicPrefix;
	#define EXT_SysFileGetPlcLogicPrefix  extern PFSYSFILEGETPLCLOGICPREFIX pfSysFileGetPlcLogicPrefix;
	#define GET_SysFileGetPlcLogicPrefix(fl)  s_pfCMGetAPI2( "SysFileGetPlcLogicPrefix", (RTS_VOID_FCTPTR *)&pfSysFileGetPlcLogicPrefix, (fl), 0, 0)
	#define CAL_SysFileGetPlcLogicPrefix  pfSysFileGetPlcLogicPrefix
	#define CHK_SysFileGetPlcLogicPrefix  (pfSysFileGetPlcLogicPrefix != NULL)
	#define EXP_SysFileGetPlcLogicPrefix  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileGetPlcLogicPrefix", (RTS_UINTPTR)SysFileGetPlcLogicPrefix, 0, 0) 
#endif




/**
 * <description>
 * Prepends the PLC logic prefix to name and ext in the resulting pathOut, makes sure the extension is there at the cost of the filename.
 * </description>
 * <param name="pszPathOut" type="OUT">Resulting file path</param>
 * <param name="ulPathOutSize" type="IN">Size of resulting file path</param>
 * <param name="pszSubDir" type="IN">Name of subdirectory (optional, only used when PLC logic prefix is set)</param>
 * <param name="pszName" type="IN">Name of file</param>
 * <param name="pszExt" type="IN">Extension of file</param>
 * <result>Error code</result>
 */
RTS_RESULT CDECL SysFilePlcLogicPrefix(char *pszPathOut, RTS_SIZE ulPathOutSize, char *pszSubDir, char *pszName, const char *pszExt);
typedef RTS_RESULT (CDECL * PFSYSFILEPLCLOGICPREFIX) (char *pszPathOut, RTS_SIZE ulPathOutSize, char *pszSubDir, char *pszName, const char *pszExt);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEPLCLOGICPREFIX_NOTIMPLEMENTED)
	#define USE_SysFilePlcLogicPrefix
	#define EXT_SysFilePlcLogicPrefix
	#define GET_SysFilePlcLogicPrefix(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFilePlcLogicPrefix(p0,p1,p2,p3,p4)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFilePlcLogicPrefix  FALSE
	#define EXP_SysFilePlcLogicPrefix  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFilePlcLogicPrefix
	#define EXT_SysFilePlcLogicPrefix
	#define GET_SysFilePlcLogicPrefix(fl)  CAL_CMGETAPI( "SysFilePlcLogicPrefix" ) 
	#define CAL_SysFilePlcLogicPrefix  SysFilePlcLogicPrefix
	#define CHK_SysFilePlcLogicPrefix  TRUE
	#define EXP_SysFilePlcLogicPrefix  CAL_CMEXPAPI( "SysFilePlcLogicPrefix" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFilePlcLogicPrefix
	#define EXT_SysFilePlcLogicPrefix
	#define GET_SysFilePlcLogicPrefix(fl)  CAL_CMGETAPI( "SysFilePlcLogicPrefix" ) 
	#define CAL_SysFilePlcLogicPrefix  SysFilePlcLogicPrefix
	#define CHK_SysFilePlcLogicPrefix  TRUE
	#define EXP_SysFilePlcLogicPrefix  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFilePlcLogicPrefix", (RTS_UINTPTR)SysFilePlcLogicPrefix, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFilePlcLogicPrefix
	#define EXT_SysFileSysFilePlcLogicPrefix
	#define GET_SysFileSysFilePlcLogicPrefix  ERR_OK
	#define CAL_SysFileSysFilePlcLogicPrefix pISysFile->ISysFilePlcLogicPrefix
	#define CHK_SysFileSysFilePlcLogicPrefix (pISysFile != NULL)
	#define EXP_SysFileSysFilePlcLogicPrefix  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFilePlcLogicPrefix
	#define EXT_SysFilePlcLogicPrefix
	#define GET_SysFilePlcLogicPrefix(fl)  CAL_CMGETAPI( "SysFilePlcLogicPrefix" ) 
	#define CAL_SysFilePlcLogicPrefix pISysFile->ISysFilePlcLogicPrefix
	#define CHK_SysFilePlcLogicPrefix (pISysFile != NULL)
	#define EXP_SysFilePlcLogicPrefix  CAL_CMEXPAPI( "SysFilePlcLogicPrefix" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFilePlcLogicPrefix  PFSYSFILEPLCLOGICPREFIX pfSysFilePlcLogicPrefix;
	#define EXT_SysFilePlcLogicPrefix  extern PFSYSFILEPLCLOGICPREFIX pfSysFilePlcLogicPrefix;
	#define GET_SysFilePlcLogicPrefix(fl)  s_pfCMGetAPI2( "SysFilePlcLogicPrefix", (RTS_VOID_FCTPTR *)&pfSysFilePlcLogicPrefix, (fl), 0, 0)
	#define CAL_SysFilePlcLogicPrefix  pfSysFilePlcLogicPrefix
	#define CHK_SysFilePlcLogicPrefix  (pfSysFilePlcLogicPrefix != NULL)
	#define EXP_SysFilePlcLogicPrefix  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFilePlcLogicPrefix", (RTS_UINTPTR)SysFilePlcLogicPrefix, 0, 0) 
#endif




/**
 * <description>
 * Check whether a given file and/or path is blacklisted
 * On a blacklisted file or path it is intended to deny all operations
 * NOTE: SysFile uses this blacklist to deny file access from IEC.
 * NOTE: Other components may use this blacklist too (currently CmpFileTransfer).
 * </description>
 * <param name="pszPath" type="IN">File and/or path to check</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">The checked file/path is blacklisted</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">The checked file/path not blacklisted.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">The file/path to check is empty or the pointer is NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">List is not initialized</errorcode>
 */
RTS_RESULT CDECL SysFileIsBlacklisted(char *pszPath);
typedef RTS_RESULT (CDECL * PFSYSFILEISBLACKLISTED) (char *pszPath);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEISBLACKLISTED_NOTIMPLEMENTED)
	#define USE_SysFileIsBlacklisted
	#define EXT_SysFileIsBlacklisted
	#define GET_SysFileIsBlacklisted(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileIsBlacklisted(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileIsBlacklisted  FALSE
	#define EXP_SysFileIsBlacklisted  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileIsBlacklisted
	#define EXT_SysFileIsBlacklisted
	#define GET_SysFileIsBlacklisted(fl)  CAL_CMGETAPI( "SysFileIsBlacklisted" ) 
	#define CAL_SysFileIsBlacklisted  SysFileIsBlacklisted
	#define CHK_SysFileIsBlacklisted  TRUE
	#define EXP_SysFileIsBlacklisted  CAL_CMEXPAPI( "SysFileIsBlacklisted" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileIsBlacklisted
	#define EXT_SysFileIsBlacklisted
	#define GET_SysFileIsBlacklisted(fl)  CAL_CMGETAPI( "SysFileIsBlacklisted" ) 
	#define CAL_SysFileIsBlacklisted  SysFileIsBlacklisted
	#define CHK_SysFileIsBlacklisted  TRUE
	#define EXP_SysFileIsBlacklisted  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileIsBlacklisted", (RTS_UINTPTR)SysFileIsBlacklisted, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileIsBlacklisted
	#define EXT_SysFileSysFileIsBlacklisted
	#define GET_SysFileSysFileIsBlacklisted  ERR_OK
	#define CAL_SysFileSysFileIsBlacklisted pISysFile->ISysFileIsBlacklisted
	#define CHK_SysFileSysFileIsBlacklisted (pISysFile != NULL)
	#define EXP_SysFileSysFileIsBlacklisted  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileIsBlacklisted
	#define EXT_SysFileIsBlacklisted
	#define GET_SysFileIsBlacklisted(fl)  CAL_CMGETAPI( "SysFileIsBlacklisted" ) 
	#define CAL_SysFileIsBlacklisted pISysFile->ISysFileIsBlacklisted
	#define CHK_SysFileIsBlacklisted (pISysFile != NULL)
	#define EXP_SysFileIsBlacklisted  CAL_CMEXPAPI( "SysFileIsBlacklisted" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileIsBlacklisted  PFSYSFILEISBLACKLISTED pfSysFileIsBlacklisted;
	#define EXT_SysFileIsBlacklisted  extern PFSYSFILEISBLACKLISTED pfSysFileIsBlacklisted;
	#define GET_SysFileIsBlacklisted(fl)  s_pfCMGetAPI2( "SysFileIsBlacklisted", (RTS_VOID_FCTPTR *)&pfSysFileIsBlacklisted, (fl), 0, 0)
	#define CAL_SysFileIsBlacklisted  pfSysFileIsBlacklisted
	#define CHK_SysFileIsBlacklisted  (pfSysFileIsBlacklisted != NULL)
	#define EXP_SysFileIsBlacklisted  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileIsBlacklisted", (RTS_UINTPTR)SysFileIsBlacklisted, 0, 0) 
#endif




/**
 * <description>
 * Add a given file and/or path to the blacklist
 * NOTE: SysFile uses this blacklist to deny file access from IEC.
 * NOTE: Other components may use this blacklist too (currently CmpFileTransfer).
 * </description>
 * <param name="pszPath" type="IN">File and/or path to add</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">The file/path is successfully added to the list.</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOMEMORY">Not enough memory to add the file/path</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOBUFFER">Copy failure, file/path is to long</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">The file/path to add is empty or the pointer is NULL</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_NOTINITIALIZED">List is not initialized</errorcode>
 */
RTS_RESULT CDECL SysFileAddToBlacklist(char *pszPath);
typedef RTS_RESULT (CDECL * PFSYSFILEADDTOBLACKLIST) (char *pszPath);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEADDTOBLACKLIST_NOTIMPLEMENTED)
	#define USE_SysFileAddToBlacklist
	#define EXT_SysFileAddToBlacklist
	#define GET_SysFileAddToBlacklist(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileAddToBlacklist(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileAddToBlacklist  FALSE
	#define EXP_SysFileAddToBlacklist  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileAddToBlacklist
	#define EXT_SysFileAddToBlacklist
	#define GET_SysFileAddToBlacklist(fl)  CAL_CMGETAPI( "SysFileAddToBlacklist" ) 
	#define CAL_SysFileAddToBlacklist  SysFileAddToBlacklist
	#define CHK_SysFileAddToBlacklist  TRUE
	#define EXP_SysFileAddToBlacklist  CAL_CMEXPAPI( "SysFileAddToBlacklist" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileAddToBlacklist
	#define EXT_SysFileAddToBlacklist
	#define GET_SysFileAddToBlacklist(fl)  CAL_CMGETAPI( "SysFileAddToBlacklist" ) 
	#define CAL_SysFileAddToBlacklist  SysFileAddToBlacklist
	#define CHK_SysFileAddToBlacklist  TRUE
	#define EXP_SysFileAddToBlacklist  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileAddToBlacklist", (RTS_UINTPTR)SysFileAddToBlacklist, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileAddToBlacklist
	#define EXT_SysFileSysFileAddToBlacklist
	#define GET_SysFileSysFileAddToBlacklist  ERR_OK
	#define CAL_SysFileSysFileAddToBlacklist pISysFile->ISysFileAddToBlacklist
	#define CHK_SysFileSysFileAddToBlacklist (pISysFile != NULL)
	#define EXP_SysFileSysFileAddToBlacklist  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileAddToBlacklist
	#define EXT_SysFileAddToBlacklist
	#define GET_SysFileAddToBlacklist(fl)  CAL_CMGETAPI( "SysFileAddToBlacklist" ) 
	#define CAL_SysFileAddToBlacklist pISysFile->ISysFileAddToBlacklist
	#define CHK_SysFileAddToBlacklist (pISysFile != NULL)
	#define EXP_SysFileAddToBlacklist  CAL_CMEXPAPI( "SysFileAddToBlacklist" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileAddToBlacklist  PFSYSFILEADDTOBLACKLIST pfSysFileAddToBlacklist;
	#define EXT_SysFileAddToBlacklist  extern PFSYSFILEADDTOBLACKLIST pfSysFileAddToBlacklist;
	#define GET_SysFileAddToBlacklist(fl)  s_pfCMGetAPI2( "SysFileAddToBlacklist", (RTS_VOID_FCTPTR *)&pfSysFileAddToBlacklist, (fl), 0, 0)
	#define CAL_SysFileAddToBlacklist  pfSysFileAddToBlacklist
	#define CHK_SysFileAddToBlacklist  (pfSysFileAddToBlacklist != NULL)
	#define EXP_SysFileAddToBlacklist  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileAddToBlacklist", (RTS_UINTPTR)SysFileAddToBlacklist, 0, 0) 
#endif




/**
 * <description>
 * Check whether online access of a given file/path is denied
 *	If ForceOnlineFilePath (s.a) is set, which is default, this check works as follows:
 *	If an absolute file path is requested, which is not a configured path, or a directory traversal path ERR_OK is returned.
 * </description>
 * <param name="pszPath" type="IN">File path</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">The file path is not accessible</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">The file path is accessible</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">The file path is empty or the pointer is NULL</errorcode>
 */
RTS_RESULT CDECL SysFileIsOnlineAccessDenied(char *pszPath);
typedef RTS_RESULT (CDECL * PFSYSFILEISONLINEACCESSDENIED) (char *pszPath);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEISONLINEACCESSDENIED_NOTIMPLEMENTED)
	#define USE_SysFileIsOnlineAccessDenied
	#define EXT_SysFileIsOnlineAccessDenied
	#define GET_SysFileIsOnlineAccessDenied(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileIsOnlineAccessDenied(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileIsOnlineAccessDenied  FALSE
	#define EXP_SysFileIsOnlineAccessDenied  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileIsOnlineAccessDenied
	#define EXT_SysFileIsOnlineAccessDenied
	#define GET_SysFileIsOnlineAccessDenied(fl)  CAL_CMGETAPI( "SysFileIsOnlineAccessDenied" ) 
	#define CAL_SysFileIsOnlineAccessDenied  SysFileIsOnlineAccessDenied
	#define CHK_SysFileIsOnlineAccessDenied  TRUE
	#define EXP_SysFileIsOnlineAccessDenied  CAL_CMEXPAPI( "SysFileIsOnlineAccessDenied" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileIsOnlineAccessDenied
	#define EXT_SysFileIsOnlineAccessDenied
	#define GET_SysFileIsOnlineAccessDenied(fl)  CAL_CMGETAPI( "SysFileIsOnlineAccessDenied" ) 
	#define CAL_SysFileIsOnlineAccessDenied  SysFileIsOnlineAccessDenied
	#define CHK_SysFileIsOnlineAccessDenied  TRUE
	#define EXP_SysFileIsOnlineAccessDenied  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileIsOnlineAccessDenied", (RTS_UINTPTR)SysFileIsOnlineAccessDenied, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileIsOnlineAccessDenied
	#define EXT_SysFileSysFileIsOnlineAccessDenied
	#define GET_SysFileSysFileIsOnlineAccessDenied  ERR_OK
	#define CAL_SysFileSysFileIsOnlineAccessDenied pISysFile->ISysFileIsOnlineAccessDenied
	#define CHK_SysFileSysFileIsOnlineAccessDenied (pISysFile != NULL)
	#define EXP_SysFileSysFileIsOnlineAccessDenied  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileIsOnlineAccessDenied
	#define EXT_SysFileIsOnlineAccessDenied
	#define GET_SysFileIsOnlineAccessDenied(fl)  CAL_CMGETAPI( "SysFileIsOnlineAccessDenied" ) 
	#define CAL_SysFileIsOnlineAccessDenied pISysFile->ISysFileIsOnlineAccessDenied
	#define CHK_SysFileIsOnlineAccessDenied (pISysFile != NULL)
	#define EXP_SysFileIsOnlineAccessDenied  CAL_CMEXPAPI( "SysFileIsOnlineAccessDenied" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileIsOnlineAccessDenied  PFSYSFILEISONLINEACCESSDENIED pfSysFileIsOnlineAccessDenied;
	#define EXT_SysFileIsOnlineAccessDenied  extern PFSYSFILEISONLINEACCESSDENIED pfSysFileIsOnlineAccessDenied;
	#define GET_SysFileIsOnlineAccessDenied(fl)  s_pfCMGetAPI2( "SysFileIsOnlineAccessDenied", (RTS_VOID_FCTPTR *)&pfSysFileIsOnlineAccessDenied, (fl), 0, 0)
	#define CAL_SysFileIsOnlineAccessDenied  pfSysFileIsOnlineAccessDenied
	#define CHK_SysFileIsOnlineAccessDenied  (pfSysFileIsOnlineAccessDenied != NULL)
	#define EXP_SysFileIsOnlineAccessDenied  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileIsOnlineAccessDenied", (RTS_UINTPTR)SysFileIsOnlineAccessDenied, 0, 0) 
#endif




/**
 * <description>
 * Check whether online modify access of a given folder path is denied
 *	If ForceOnlineFilePath (s.a) is set, which is default, this check works as follows:
 *	If an absolute folder path is requested, which is directly a configured placeholder path, but w/o .Temporary attribute (s.a.) ERR_OK is returned.
 * </description>
 * <param name="pszDir" type="IN">Folder path</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">The folder path is not accessible</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">The folder path is accessible</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">The folder path is empty or the pointer is NULL</errorcode>
 */
RTS_RESULT CDECL SysFileIsOnlineModifyAccessDenied(char *pszDir);
typedef RTS_RESULT (CDECL * PFSYSFILEISONLINEMODIFYACCESSDENIED) (char *pszDir);
#if defined(SYSFILE_NOTIMPLEMENTED) || defined(SYSFILEISONLINEMODIFYACCESSDENIED_NOTIMPLEMENTED)
	#define USE_SysFileIsOnlineModifyAccessDenied
	#define EXT_SysFileIsOnlineModifyAccessDenied
	#define GET_SysFileIsOnlineModifyAccessDenied(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysFileIsOnlineModifyAccessDenied(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysFileIsOnlineModifyAccessDenied  FALSE
	#define EXP_SysFileIsOnlineModifyAccessDenied  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysFileIsOnlineModifyAccessDenied
	#define EXT_SysFileIsOnlineModifyAccessDenied
	#define GET_SysFileIsOnlineModifyAccessDenied(fl)  CAL_CMGETAPI( "SysFileIsOnlineModifyAccessDenied" ) 
	#define CAL_SysFileIsOnlineModifyAccessDenied  SysFileIsOnlineModifyAccessDenied
	#define CHK_SysFileIsOnlineModifyAccessDenied  TRUE
	#define EXP_SysFileIsOnlineModifyAccessDenied  CAL_CMEXPAPI( "SysFileIsOnlineModifyAccessDenied" ) 
#elif defined(MIXED_LINK) && !defined(SYSFILE_EXTERNAL)
	#define USE_SysFileIsOnlineModifyAccessDenied
	#define EXT_SysFileIsOnlineModifyAccessDenied
	#define GET_SysFileIsOnlineModifyAccessDenied(fl)  CAL_CMGETAPI( "SysFileIsOnlineModifyAccessDenied" ) 
	#define CAL_SysFileIsOnlineModifyAccessDenied  SysFileIsOnlineModifyAccessDenied
	#define CHK_SysFileIsOnlineModifyAccessDenied  TRUE
	#define EXP_SysFileIsOnlineModifyAccessDenied  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileIsOnlineModifyAccessDenied", (RTS_UINTPTR)SysFileIsOnlineModifyAccessDenied, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysFileSysFileIsOnlineModifyAccessDenied
	#define EXT_SysFileSysFileIsOnlineModifyAccessDenied
	#define GET_SysFileSysFileIsOnlineModifyAccessDenied  ERR_OK
	#define CAL_SysFileSysFileIsOnlineModifyAccessDenied pISysFile->ISysFileIsOnlineModifyAccessDenied
	#define CHK_SysFileSysFileIsOnlineModifyAccessDenied (pISysFile != NULL)
	#define EXP_SysFileSysFileIsOnlineModifyAccessDenied  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysFileIsOnlineModifyAccessDenied
	#define EXT_SysFileIsOnlineModifyAccessDenied
	#define GET_SysFileIsOnlineModifyAccessDenied(fl)  CAL_CMGETAPI( "SysFileIsOnlineModifyAccessDenied" ) 
	#define CAL_SysFileIsOnlineModifyAccessDenied pISysFile->ISysFileIsOnlineModifyAccessDenied
	#define CHK_SysFileIsOnlineModifyAccessDenied (pISysFile != NULL)
	#define EXP_SysFileIsOnlineModifyAccessDenied  CAL_CMEXPAPI( "SysFileIsOnlineModifyAccessDenied" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysFileIsOnlineModifyAccessDenied  PFSYSFILEISONLINEMODIFYACCESSDENIED pfSysFileIsOnlineModifyAccessDenied;
	#define EXT_SysFileIsOnlineModifyAccessDenied  extern PFSYSFILEISONLINEMODIFYACCESSDENIED pfSysFileIsOnlineModifyAccessDenied;
	#define GET_SysFileIsOnlineModifyAccessDenied(fl)  s_pfCMGetAPI2( "SysFileIsOnlineModifyAccessDenied", (RTS_VOID_FCTPTR *)&pfSysFileIsOnlineModifyAccessDenied, (fl), 0, 0)
	#define CAL_SysFileIsOnlineModifyAccessDenied  pfSysFileIsOnlineModifyAccessDenied
	#define CHK_SysFileIsOnlineModifyAccessDenied  (pfSysFileIsOnlineModifyAccessDenied != NULL)
	#define EXP_SysFileIsOnlineModifyAccessDenied  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysFileIsOnlineModifyAccessDenied", (RTS_UINTPTR)SysFileIsOnlineModifyAccessDenied, 0, 0) 
#endif




#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSFILEOPEN ISysFileOpen;
 	PFSYSFILEOPEN_ ISysFileOpen_;
 	PFSYSFILECLOSE ISysFileClose;
 	PFSYSFILECLOSE_ ISysFileClose_;
 	PFSYSFILEGETSTATUS ISysFileGetStatus;
 	PFSYSFILEGETNAME ISysFileGetName;
 	PFSYSFILEREAD ISysFileRead;
 	PFSYSFILEWRITE ISysFileWrite;
 	PFSYSFILEDELETE ISysFileDelete;
 	PFSYSFILEDELETE_ ISysFileDelete_;
 	PFSYSFILEDELETEBYHANDLE ISysFileDeleteByHandle;
 	PFSYSFILERENAME ISysFileRename;
 	PFSYSFILERENAME_ ISysFileRename_;
 	PFSYSFILESETPOS ISysFileSetPos;
 	PFSYSFILEGETPOS ISysFileGetPos;
 	PFSYSFILEGETSIZE ISysFileGetSize;
 	PFSYSFILEGETSIZE_ ISysFileGetSize_;
 	PFSYSFILEGETSIZEBYHANDLE ISysFileGetSizeByHandle;
 	PFSYSFILEGETTIME ISysFileGetTime;
 	PFSYSFILEGETTIME_ ISysFileGetTime_;
 	PFSYSFILEEOF ISysFileEOF;
 	PFSYSFILECOPY ISysFileCopy;
 	PFSYSFILECOPY_ ISysFileCopy_;
 	PFSYSFILEGETPATH ISysFileGetPath;
 	PFSYSFILEGETPATH2 ISysFileGetPath2;
 	PFSYSFILEGETFULLPATH ISysFileGetFullPath;
 	PFSYSFILEGETFULLPATH2 ISysFileGetFullPath2;
 	PFSYSFILEGETIECPATH ISysFileGetIecPath;
 	PFSYSFILEGETIECPATH2 ISysFileGetIecPath2;
 	PFSYSFILEGENERATECRC ISysFileGenerateCRC;
 	PFSYSFILEGENERATECRC2 ISysFileGenerateCRC2;
 	PFSYSFILEGENERATECRC3 ISysFileGenerateCRC3;
 	PFSYSFILEGENERATECRC3_ ISysFileGenerateCRC3_;
 	PFSYSFILEGENERATECRC2_ ISysFileGenerateCRC2_;
 	PFSYSFILEFLUSH ISysFileFlush;
 	PFSYSFILEISINVISIBLE ISysFileIsInvisible;
 	PFSYSFILETRUNCATE ISysFileTruncate;
 	PFSYSFILEGETFIRSTCONFIGUREDPATH ISysFileGetFirstConfiguredPath;
 	PFSYSFILEGETNEXTCONFIGUREDPATH ISysFileGetNextConfiguredPath;
 	PFSYSFILEGETNEXTCONFIGUREDPATH2 ISysFileGetNextConfiguredPath2;
 	PFSYSFILEREGISTERPLACEHOLDER ISysFileRegisterPlaceholder;
 	PFSYSFILESETIECPATH ISysFileSetIecPath;
 	PFSYSFILEGETPLCLOGICPREFIX ISysFileGetPlcLogicPrefix;
 	PFSYSFILEPLCLOGICPREFIX ISysFilePlcLogicPrefix;
 	PFSYSFILEISBLACKLISTED ISysFileIsBlacklisted;
 	PFSYSFILEADDTOBLACKLIST ISysFileAddToBlacklist;
 	PFSYSFILEISONLINEACCESSDENIED ISysFileIsOnlineAccessDenied;
 	PFSYSFILEISONLINEMODIFYACCESSDENIED ISysFileIsOnlineModifyAccessDenied;
 } ISysFile_C;

#ifdef CPLUSPLUS
class ISysFile : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISysFileOpen(char *pszFile, RTS_ACCESS_MODE am, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysFileOpen_(char *pszFile, RTS_ACCESS_MODE am, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysFileClose(void) =0;
		virtual RTS_RESULT CDECL ISysFileClose_(RTS_HANDLE hFile) =0;
		virtual SYS_FILE_STATUS CDECL ISysFileGetStatus(RTS_RESULT *pResult) =0;
		virtual char* CDECL ISysFileGetName(RTS_RESULT *pResult) =0;
		virtual RTS_SIZE CDECL ISysFileRead(unsigned char *pbyBuffer, RTS_SIZE uiSize, RTS_RESULT *pResult) =0;
		virtual RTS_SIZE CDECL ISysFileWrite(unsigned char *pbyBuffer, RTS_SIZE uiSize, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysFileDelete(char *pszFile) =0;
		virtual RTS_RESULT CDECL ISysFileDelete_(char *pszFile) =0;
		virtual RTS_RESULT CDECL ISysFileDeleteByHandle(void) =0;
		virtual RTS_RESULT CDECL ISysFileRename(char *pszOldFileName, char *pszNewFileName) =0;
		virtual RTS_RESULT CDECL ISysFileRename_(char *pszOldFileName, char *pszNewFileName) =0;
		virtual RTS_RESULT CDECL ISysFileSetPos(RTS_SIZE uiOffset) =0;
		virtual RTS_RESULT CDECL ISysFileGetPos(RTS_SIZE *puiPos) =0;
		virtual RTS_SIZE CDECL ISysFileGetSize(char *pszFile, RTS_RESULT *pResult) =0;
		virtual RTS_SIZE CDECL ISysFileGetSize_(char *pszFile, RTS_RESULT *pResult) =0;
		virtual RTS_SIZE CDECL ISysFileGetSizeByHandle(RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysFileGetTime(char *pszFile, SYS_FILETIME *pftFileTime) =0;
		virtual RTS_RESULT CDECL ISysFileGetTime_(char *pszFile, SYS_FILETIME *pftFileTime) =0;
		virtual RTS_RESULT CDECL ISysFileEOF(void) =0;
		virtual RTS_RESULT CDECL ISysFileCopy(char *pszDestFileName, char *pszSourceFileName, RTS_SIZE *puiCopied) =0;
		virtual RTS_RESULT CDECL ISysFileCopy_(char *pszDestFileName, char *pszSourceFileName, RTS_SIZE *puiCopied) =0;
		virtual RTS_RESULT CDECL ISysFileGetPath(char *pszFileName, char *pszPath, RTS_SIZE iMaxLen) =0;
		virtual RTS_RESULT CDECL ISysFileGetPath2(char *pszFileName, RTS_I32 nOption, char *pszPath, RTS_SIZE iMaxLen) =0;
		virtual RTS_RESULT CDECL ISysFileGetFullPath(char *pszFileName, char *pszPath, RTS_SIZE iMaxLen) =0;
		virtual RTS_RESULT CDECL ISysFileGetFullPath2(char *pszFileName, RTS_I32 nOption, char *pszPath, RTS_SIZE iMaxLen) =0;
		virtual RTS_RESULT CDECL ISysFileGetIecPath(char *pszFileName, char *pszPath, RTS_SIZE iMaxLen) =0;
		virtual RTS_RESULT CDECL ISysFileGetIecPath2(char *pszFileName, RTS_I32 nOption, char *pszPath, RTS_SIZE iMaxLen) =0;
		virtual RTS_RESULT CDECL ISysFileGenerateCRC(char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC) =0;
		virtual RTS_RESULT CDECL ISysFileGenerateCRC2(char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC) =0;
		virtual RTS_RESULT CDECL ISysFileGenerateCRC3(char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC, RTS_BOOL bUseCache) =0;
		virtual RTS_RESULT CDECL ISysFileGenerateCRC3_(char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC, RTS_BOOL bUseCache) =0;
		virtual RTS_RESULT CDECL ISysFileGenerateCRC2_(char *pszFile, RTS_SIZE ulSize, RTS_UI32 *pulCRC) =0;
		virtual RTS_RESULT CDECL ISysFileFlush(void) =0;
		virtual RTS_RESULT CDECL ISysFileIsInvisible(char *pszFileName) =0;
		virtual RTS_RESULT CDECL ISysFileTruncate(RTS_SIZE sSizeNew) =0;
		virtual RTS_HANDLE CDECL ISysFileGetFirstConfiguredPath(char *pszFilePath, RTS_I32 nMaxFilePathLen, char *pszPlaceholder, RTS_I32 nMaxPlaceholderLen, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysFileGetNextConfiguredPath(RTS_HANDLE hPrevPath, char *pszFilePath, RTS_I32 nMaxFilePathLen, char *pszPlaceholder, RTS_I32 nMaxPlaceholderLen, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysFileGetNextConfiguredPath2(RTS_HANDLE hPrevPath, char *pszFilePath, RTS_I32 nMaxFilePathLen, char *pszPlaceholder, RTS_I32 nMaxPlaceholderLen, RTS_UI32 nFlags, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysFileRegisterPlaceholder(char* pszPlaceholder, char* pszFolder, RTS_UI8 attribute) =0;
		virtual RTS_RESULT CDECL ISysFileSetIecPath(char *pszPath) =0;
		virtual char * CDECL ISysFileGetPlcLogicPrefix(void) =0;
		virtual RTS_RESULT CDECL ISysFilePlcLogicPrefix(char *pszPathOut, RTS_SIZE ulPathOutSize, char *pszSubDir, char *pszName, const char *pszExt) =0;
		virtual RTS_RESULT CDECL ISysFileIsBlacklisted(char *pszPath) =0;
		virtual RTS_RESULT CDECL ISysFileAddToBlacklist(char *pszPath) =0;
		virtual RTS_RESULT CDECL ISysFileIsOnlineAccessDenied(char *pszPath) =0;
		virtual RTS_RESULT CDECL ISysFileIsOnlineModifyAccessDenied(char *pszDir) =0;
};
	#ifndef ITF_SysFile
		#define ITF_SysFile static ISysFile *pISysFile = NULL;
	#endif
	#define EXTITF_SysFile
#else	/*CPLUSPLUS*/
	typedef ISysFile_C		ISysFile;
	#ifndef ITF_SysFile
		#define ITF_SysFile
	#endif
	#define EXTITF_SysFile
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSFILEITF_H_*/
