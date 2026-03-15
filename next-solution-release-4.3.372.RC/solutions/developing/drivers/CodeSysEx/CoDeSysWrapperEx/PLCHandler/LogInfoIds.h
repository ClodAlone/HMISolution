/**
 *  <desription>
 *		Log info IDs for all components.
 *		Text will be resolved in another xml-file.
 *	</desription>
*/

#ifndef _LOGINFOIDS_H_
#define _LOGINFOIDS_H_

/**
 *  <desription>CmpMgr</desription>
 */
#define LOGID_CmpMgr_ImportFailed					0x00000001	/** <classid>LOG_ERROR</classid>	<severity>PLC shutdown</severity> */
#define LOGID_CmpMgr_BasicChecksFailed				0x00000002	/** <classid>LOG_ERROR</classid>	<severity>PLC shutdown</severity> */
#define LOGID_CmpMgr_ConfigurationNotFound			0x00000003	/** <classid>LOG_ERROR</classid>	<severity>PLC shutdown</severity> */
#define LOGID_CmpMgr_Name							0x00000004	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Copyright						0x00000005	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_BuildDate						0x00000006	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitSystemComponent			0x00000007	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitSystemHook					0x00000008	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitSystemHookDone				0x00000009	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_LoadComponents					0x0000000A	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_LoadComponentDone				0x0000000B	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_LoadComponentFailed			0x0000000C	/** <classid>LOG_ERROR</classid>	<severity>Component inoperative</severity> */
#define LOGID_CmpMgr_LoadComponentsDone				0x0000000D	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_CallEntryFunctions				0x0000000E	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_CallEntryFunctionDone			0x0000000F	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_CallEntryFunctionFailed		0x00000010	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_CallEntryFunctionsDone			0x00000011	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_CallExportFunctions			0x00000012	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_CallExportFunctionFailed		0x00000013	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_CallExportFunctionsDone		0x00000014	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_CallImportFunctions			0x00000015	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_CallImportFunctionFailed		0x00000016	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_CallImportFunctionsDone		0x00000017	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitHook						0x00000018	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitHookDone					0x00000019	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Init2Hook						0x0000001A	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Init2HookDone					0x0000001B	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Init3Hook						0x0000001C	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Init3HookDone					0x0000001D	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitTasksHook					0x0000001E	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitTasksHookDone				0x0000001F	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitCommHook					0x00000020	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitCommHookDone				0x00000021	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Ready							0x00000022	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define	LOGID_CmpMgr_Shutdown						0x00000023	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define	LOGID_CmpMgr_ShutdownDone					0x00000024	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_ExitHook						0x00000025	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_ExitHookDone					0x00000026	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Exit2Hook						0x00000027	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Exit2HookDone					0x00000028	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Exit3Hook						0x00000029	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Exit3HookDone					0x0000002A	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_ExitTasksHook					0x0000002B	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_ExitTasksHookDone				0x0000002C	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_ExitCommHook					0x0000002D	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_ExitCommHookDone				0x0000002E	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Init201Hook					0x0000002F	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_Init201HookDone				0x00000030	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitComponent					0x00000031	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_ExitComponent					0x00000032	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_SysTargetSignatureMismatch		0x00000033	/** <classid>LOG_ERROR</classid>	<severity>PLC shutdown</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_DSP		   	0x00000034	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_RTS_I8		0x00000035	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_I16			0x00000036	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_I32			0x00000037	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_UI8			0x00000038	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_UI16			0x00000039	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_UI32			0x0000003A	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_RESULT		0x0000003B	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_HANDLE		0x0000003C	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_WCHAR		0x0000003D	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_SWAP16u		0x0000003E	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_SWAP16s		0x0000003F	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_SWAP32u		0x00000040	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_SWAP32s		0x00000041	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_3SLicense_Missing				0x00000042	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_UINTPTR		0x00000043	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_SIZE			0x00000044	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_InitFailed_SafeMode			0x00000045	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_InitFailed_ObjectManager		0x00000046	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_FormatSpecifier	0x00000047	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_ExtLibAlignment	0x00000048	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_I8			0x00000049	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_InitSystemTasksHook			0x0000004A	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_InitSystemTasksHookDone		0x0000004B	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_ExitSystemTasksHookDone		0x0000004C	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_PackMode		0x0000004D	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_InitComponentFailed			0x0000004E	/** <classid>LOG_ERROR</classid>	<severity>Component inoperative</severity> */
#define LOGID_CmpMgr_UnloadFailedComponent			0x0000004F	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_GUID			0x00000050	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_GUID_STRING	0x00000051	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpMgr_ExitShutdownHook				0x00000052	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_ExitShutdownHookDone			0x00000053	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpMgr_BasicChecksFailed_Atomic		0x00000054	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */


/**
 *  <desription>CmpApp</desription>
 */
#define LOGID_CmpApp_ApplicationNotFound			0x00000001	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_ApplicationLoaded				0x00000002	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_ApplicationCodeId				0x00000003	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_ApplicationDataId				0x00000004	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_OpenBootprojectFailed			0x00000005	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_BootprojectLoadedApp			0x00000006	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_BootprojectRetainsMatched		0x00000007	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_BootprojectRetainsMismatch		0x00000008	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_BootprojectLoadFailed			0x00000009	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_BootprojectStart				0x0000000A	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_BootprojectStartFailed			0x0000000B	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_ExtRef_Unresolved				0x0000000C	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_InvalidExternalReferenceArea	0x0000000D	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_BootprojectCRCMismatch			0x0000000E	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_BootprojectCreateCRCFailed		0x0000000F	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_NoAreaMemory					0x00000010	/** <classid>LOG_ERROR</classid>		<severity>No area memory for application</severity> */
#define LOGID_CmpApp_LoadRetainDataFailed			0x00000011	/** <classid>LOG_ERROR</classid>		<severity>Loading retain data failed</severity> */
#define LOGID_CmpApp_BootprojectNoRetains			0x00000012	/** <classid>LOG_INFO</classid>			<severity>No retains because of RetainType=None</severity> */
#define LOGID_CmpApp_Exception						0x00000013	/** <classid>LOG_EXCEPTION</classid>	<severity>Application can not be executed</severity> */
#define LOGID_CmpApp_ExtRef_VersionMismatch			0x00000014	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_ExtRef_SignatureMismatch		0x00000015	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_BootprojectNoRetainArea		0x00000016	/** <classid>LOG_INFO</classid>			<severity>Application has no retain areas</severity> */
#define LOGID_CmpApp_BootprojectCreateFailed		0x00000017	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_OnlineChangeDenied				0x00000018	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_BootprojectLoadDenied			0x00000019	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_BootprojectStartDenied			0x00000020	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_StartDenied					0x00000021	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_StopDenied						0x00000022	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_BootprojectRetainsMismatchExc	0x00000023	/** <classid>LOG_ERROR</classid>		<severity>Application loaded, but in exception state because of retain mismatch</severity> */
/*#define LOGID_CmpApp_AsyncServiceFailed				0x00000024*/	/** <classid>LOG_EXCEPTION</classid>	<severity>Application service failed</severity> */
#define LOGID_CmpApp_BootprojectTargetMismatch		0x00000025	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_BootprojectStartFailedException	0x00000026	/** <classid>LOG_ERROR</classid>	<severity>Application operation failed</severity> */
#define LOGID_CmpApp_BootprojectNoStart				0x00000027	/** <classid>LOG_INFO</classid>	<severity>Application not started (is denied in this state)</severity> */
#define LOGID_CmpApp_BootprojectRestoreNoFile		0x00000028	/** <classid>LOG_EXCEPTION</classid>	<severity>Application operation failed</severity> */
#define LOGID_CmpApp_BootprojectRestoreFailed		0x00000029	/** <classid>LOG_EXCEPTION</classid>	<severity>Application operation failed</severity> */
#define LOGID_CmpApp_DeleteDenied					0x0000002A	/** <classid>LOG_WARNING</classid>		<severity>none</severity> */
#define LOGID_CmpApp_ServiceNotSupported			0x0000002B	/** <classid>LOG_ERROR</classid>		<severity>Online service not supported</severity> */
#define LOGID_CmpApp_FlashEraseSuccessfull			0x0000002C	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_FlashEraseFailed				0x0000002D	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpApp_ParentBootprojectFailed		0x0000002E	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpApp_ParentBootprojectMismatch		0x0000002F	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpApp_BootprojectFormat				0x00000030	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpApp_RetainsInitialized				0x00000031	/** <classid>LOG_INFO/LOG_WARNING</classid>		<severity>none</severity> */
#define LOGID_CmpApp_DownloadDenied					0x00000032	/** <classid>LOG_ERROR</classid>		<severity>Operation denied</severity> */
#define LOGID_CmpApp_OnlineChangeDenied2			0x00000033	/** <classid>LOG_ERROR</classid>		<severity>Operation denied</severity> */
#define LOGID_CmpApp_ResetDenied					0x00000034	/** <classid>LOG_ERROR</classid>		<severity>Operation denied</severity> */
#define LOGID_CmpApp_CodeMeterDeviceMissing			0x00000035	/** <classid>LOG_ERROR</classid>		<severity>Operation denied</severity> */
#define LOGID_CmpApp_RelocCode_Failed				0x00000036	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpApp_CodeInit_Failed				0x00000037	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpApp_GlobalInitCode_Failed			0x00000038	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpApp_GlobalExitCode_Failed			0x00000039	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpApp_DownloadPOUCode_Failed			0x0000003A	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpApp_OC1ConcurrentCode_Failed		0x0000003B	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpApp_OC2RepeatableCode_Failed		0x0000003C	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpApp_DownloadBPActive_Failed		0x0000003D	/** <classid>LOG_ERROR</classid>		<severity>Application failed</severity> */
#define LOGID_CmpApp_CodeMeterComponentMissing		0x0000003E	/** <classid>LOG_ERROR</classid>		<severity>Operation denied</severity> */
#define LOGID_CmpApp_OnlineChangeCodeLongExecution	0x0000003F	/** <classid>LOG_WARNING</classid>		<severity>none</severity> */
#define LOGID_CmpApp_DynModWrite_Failed				0x00000040	/** <classid>LOG_ERROR</classid>		<severity>Application failed</severity> */
#define LOGID_CmpApp_DynModLoad_Failed				0x00000041	/** <classid>LOG_ERROR</classid>		<severity>Application failed</severity> */
#define LOGID_CmpApp_Init_Exception					0x00000042	/** <classid>LOG_EXCEPTION</classid>	<severity>Exception in Application init</severity> */
#define LOGID_CmpApp_SourcePosition					0x00000043	/** <classid>LOG_EXCEPTION</classid>	<severity>Source position of an exception</severity> */
#define LOGID_CmpApp_DownloadFailed					0x00000044	/** <classid>LOG_ERROR</classid>		<severity>Application download failed</severity> */
#define LOGID_CmpApp_OnlineChange_NoGap				0x00000045	/** <classid>LOG_EXCEPTION</classid>	<severity>Application OnlineChange failed</severity> */
#define LOGID_CmpApp_CIM_UnloadFailed				0x00000046  /** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_InitServiceHandlerDenied		0x00000047	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpApp_OpenBootprojectCRCFailed		0x00000048	/** <classid>LOG_ERROR</classid>		<severity>Application operation failed</severity> */
#define LOGID_CmpApp_UnalignedArea					0x00000049	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpApp_IncorrectSessionInfo			0x0000004A	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpApp_InsufficientLicense			0x0000004B	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpApp_IncompatibleLicense			0x0000004C	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpApp_AllocRetainCRCAreaFailed		0x0000004D	/** <classid>LOG_WARNING</classid>	    <severity>none</severity> */

/**
 * <description>CmpAppEmbedded</description>
 */
#define LOGID_CmpAppEmbedded_ApplicationCodeId				0x00000001	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_ApplicationDataId				0x00000002	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_ApplicationLoaded				0x00000003	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_ApplicationNotFound			0x00000004	/** <classid>LOG_ERROR</classid>	<severity>Application operation failed</severity> */
#define LOGID_CmpAppEmbedded_BootprojectLoadDenied			0x00000005	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_BootprojectLoadedApp			0x00000006	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_BootprojectNoStart				0x00000007	/** <classid>LOG_INFO</classid>	<severity>Application not started (is denied in this state)</severity> */
#define LOGID_CmpAppEmbedded_BootprojectRetainsMismatchExc	0x00000008	/** <classid>LOG_ERROR</classid>	<severity>Application loaded, but in exception state because of retain mismatch</severity> */
#define LOGID_CmpAppEmbedded_BootprojectStart				0x00000009	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_BootprojectStartDenied			0x0000000A	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_BootprojectStartFailed			0x0000000B	/** <classid>LOG_ERROR</classid>	<severity>Application operation failed</severity> */
#define LOGID_CmpAppEmbedded_BootprojectStartFailedException		0x0000000C	/** <classid>LOG_ERROR</classid>	<severity>Application operation failed</severity> */
#define LOGID_CmpAppEmbedded_Exception						0x0000000D	/** <classid>LOG_EXCEPTION</classid>	<severity>Application can not be executed</severity> */
#define LOGID_CmpAppEmbedded_OnlineChangeDenied				0x0000000E	/** <classid>LOG_ERROR</classid>	<severity>Application operation failed</severity> */
#define LOGID_CmpAppEmbedded_ServiceNotSupported			0x0000000F	/** <classid>LOG_ERROR</classid>		<severity>Online service not supported</severity> */
#define LOGID_CmpAppEmbedded_StartDenied					0x00000010	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_StopDenied						0x00000011	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_RunInFlash						0x00000012	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_ApplicationCodeCRC				0x00000013	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_ApplicationHeader				0x00000014	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_DeviceType						0x00000015	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_DeviceID						0x00000016	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_DeviceVersion					0x00000017	/** <classid>LOG_ERROR</classid>		<severity>none</severity> */
#define LOGID_CmpAppEmbedded_RelocCode_Failed				0x00000018	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpAppEmbedded_CodeInit_Failed				0x00000019	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpAppEmbedded_GlobalInitCode_Failed			0x0000001A	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpAppEmbedded_GlobalExitCode_Failed			0x0000001B	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpAppEmbedded_DownloadPOUCode_Failed			0x0000001C	/** <classid>LOG_EXCEPTION</classid>	<severity>Application failed</severity> */
#define LOGID_CmpAppEmbedded_DeleteBootprojectDenied		0x0000001D	/** <classid>LOG_WARNING</classid>		<severity>none</severity> */


/**
 *  <desription>CmpAppBP</desription>
 */
#define LOGID_CmpAppBP_CreateBPPoolFailed			0x00000001	/** <classid>LOG_ERROR</classid>	<severity>No breakpoint pool available</severity> */
#define LOGID_CmpAppBP_CreateFlowPoolFailed			0x00000002	/** <classid>LOG_ERROR</classid>	<severity>No flow pool available</severity> */
#define LOGID_CmpAppBP_CreateFlowSrvcPoolFailed		0x00000003	/** <classid>LOG_ERROR</classid>	<severity>No flow service pool available</severity> */
#define LOGID_CmpAppBP_SetBPDenied					0x00000004	/** <classid>LOG_ERROR</classid>	<severity>Set BP denied</severity> */
#define LOGID_CmpAppBP_EmptyHit						0x00000005	/** <classid>LOG_ERROR</classid>	<severity>Breakpoint was hitten, but could not be found</severity> */
#define LOGID_CmpAppBP_NoTask						0x00000006	/** <classid>LOG_ERROR</classid>	<severity>Current task could not be determined</severity> */
#define LOGID_CmpAppBP_RecursiveCall				0x00000007	/** <classid>LOG_INFO</classid>		<severity>Breakpoint was already reached in same context</severity> */

/**
 *  <desription>CmpAppForce</desription>
 */
#define LOGID_CmpAppForce_CreateForcePoolFailed		0x00000001	/** <classid>LOG_ERROR</classid>	<severity>No force pool available</severity> */


/**
 *  <desription>CmpBlkDrvCom</desription>
 */
#define LOGID_CmpBlkDrvCom_OpenComFailed			0x00000001	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvCom_ExceptionOccurred		0x00000002	/** <classid>LOG_EXCEPTION</classid>	<severity>Restart communication server</severity> */
#define LOGID_CmpBlkDrvCom_CRCError					0x00000003	/** <classid>LOG_WARNING</classid>		<severity>Low communication performance, block has to be repeated</severity> */
#define LOGID_CmpBlkDrvCom_ComPortOpened			0x00000004	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpBlkDrvCom_CloseComPort				0x00000005	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpBlkDrvCom_ReregisterPort			0x00000006	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpBlkDrvCom_BufferExceeded			0x00000007	/** <classid>LOG_ERROR</classid>		<severity>Communication error</severity> */
#define LOGID_CmpBlkDrvCom_BlockLenError			0x00000008	/** <classid>LOG_WARNING</classid>		<severity>Low communication performace, block has to be repeated</severity> */
#define LOGID_CmpBlkDrvCom_DuplicateHashCharacter	0x00000009	/** <classid>LOG_WARNING</classid>		<severity>Low communication performace, block has to be repeated</severity> */
#define LOGID_CmpBlkDrvCom_EndDelimiterWithoutStart	0x0000000A	/** <classid>LOG_WARNING</classid>		<severity>Low communication performace, block has to be repeated</severity> */
#define LOGID_CmpBlkDrvCom_ReadFailed				0x0000000B	/** <classid>LOG_ERROR</classid>		<severity>Communication error</severity> */
#define LOGID_CmpBlkDrvCom_WriteFailed				0x0000000C	/** <classid>LOG_ERROR</classid>		<severity>Communication error</severity> */
#define LOGID_CmpBlkDrvCom_SwitchNoneToHdx			0x0000000D	/** <classid>LOG_DEBUG</classid>		<severity>protocol type changed</severity> */
#define LOGID_CmpBlkDrvCom_SwitchNoneToAutoAddr		0x0000000E	/** <classid>LOG_DEBUG</classid>		<severity>protocol type changed</severity> */
#define LOGID_CmpBlkDrvCom_SwitchAutoAddrToHdx		0x0000000F	/** <classid>LOG_DEBUG</classid>		<severity>protocol type changed</severity> */
#define LOGID_CmpBlkDrvCom_SwitchHdxToAutoAddr		0x00000010	/** <classid>LOG_DEBUG</classid>		<severity>protocol type changed</severity> */
#define LOGID_CmpBlkDrvCom_ProtocolAndAddr			0x00000011	/** <classid>LOG_DEBUG</classid>		<severity>none</severity> */

/**
 *  <desription>CmpBlkDrvDirectCall</desription>
 */
#define LOGID_CmpBlkDrvDirectCall_ExceptionOccurred		0x00000001	/** <classid>LOG_EXCEPTION</classid>	<severity>Restart communication server</severity> */
#define LOGID_CmpBlkDrvDirectCall_RegisterDriverFailed	0x00000002	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvDirectCall_EventCreateFailed		0x00000003	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvDirectCall_TaskCreateFailed		0x00000004	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvDirectCall_EventWaitFailed		0x00000005	/** <classid>LOG_WARNING</classid>		<severity>Low communication performance</severity> */ 

/**
 *  <desription>CmpBlkDrvUsb</desription>
 */
#define LOGID_CmpBlkDrvUsb_OpenComFailed			0x00000001	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvUsb_ExceptionOccurred		0x00000002	/** <classid>LOG_EXCEPTION</classid>	<severity>Restart communication server</severity> */
#define LOGID_CmpBlkDrvUsb_CRCError					0x00000003	/** <classid>LOG_WARNING</classid>		<severity>Low communication performance, block has to be repeated</severity> */
#define LOGID_CmpBlkDrvUsb_ComPortOpened			0x00000004	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpBlkDrvUsb_CloseComPort				0x00000005	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpBlkDrvUsb_ReregisterPort			0x00000006	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpBlkDrvUsb_BufferExceeded			0x00000007	/** <classid>LOG_ERROR</classid>		<severity>Communication error</severity> */
#define LOGID_CmpBlkDrvUsb_BlockLenError			0x00000008	/** <classid>LOG_WARNING</classid>		<severity>Low communication performace, block has to be repeated</severity> */
#define LOGID_CmpBlkDrvUsb_DuplicateHashCharacter	0x00000009	/** <classid>LOG_WARNING</classid>		<severity>Low communication performace, block has to be repeated</severity> */
#define LOGID_CmpBlkDrvUsb_EndDelimiterWithoutStart	0x0000000A	/** <classid>LOG_WARNING</classid>		<severity>Low communication performace, block has to be repeated</severity> */
#define LOGID_CmpBlkDrvUsb_ReadFailed				0x0000000B	/** <classid>LOG_ERROR</classid>		<severity>Communication error</severity> */
#define LOGID_CmpBlkDrvUsb_WriteFailed				0x0000000C	/** <classid>LOG_ERROR</classid>		<severity>Communication error</severity> */
#define LOGID_CmpBlkDrvUsb_SwitchNoneToHdx			0x0000000D	/** <classid>LOG_DEBUG</classid>		<severity>protocol type changed</severity> */
#define LOGID_CmpBlkDrvUsb_SwitchNoneToAutoAddr		0x0000000E	/** <classid>LOG_DEBUG</classid>		<severity>protocol type changed</severity> */
#define LOGID_CmpBlkDrvUsb_SwitchAutoAddrToHdx		0x0000000F	/** <classid>LOG_DEBUG</classid>		<severity>protocol type changed</severity> */
#define LOGID_CmpBlkDrvUsb_SwitchHdxToAutoAddr		0x00000010	/** <classid>LOG_DEBUG</classid>		<severity>protocol type changed</severity> */
#define LOGID_CmpBlkDrvUsb_ProtocolAndAddr			0x00000011	/** <classid>LOG_DEBUG</classid>		<severity>none</severity> */

/**
 *  <desription>CmpBlkDrvUdp</desription>
 */
#define LOGID_CmpBlkDrvUdp_ExceptionOccurred		0x00000001	/** <classid>LOG_EXCEPTION</classid>	<severity>Restart communication server</severity> */
#define LOGID_CmpBlkDrvUdp_InvalidIpAddress			0x00000002	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvUdp_MaxInterfacesExceed		0x00000003	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvUdp_InvalidNetworkMask		0x00000004	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvUdp_InvalidPortIndex			0x00000005	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvUdp_CheckIpAddress			0x00000006	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpBlkDrvUdp_NoReceivePort			0x00000007	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvUdp_CreateSocketFailed		0x00000008	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvUdp_SetPPPRemoteAddress		0x00000009	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpBlkDrvUdp_InvalidPPPRemoteAddr		0x0000000A	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvUdp_SelectFailed				0x0000000B	/** <classid>LOG_WARNING</classid>		<severity>Low communication performance</severity> */
#define LOGID_CmpBlkDrvUdp_NumInterfaces			0x0000000C	/** <classid>LOG_INFO</classid>			<severity>none</severity> */

/**
 *  <desription>CmpBlkDrvTcp</desription>
 */
#define LOGID_CmpBlkDrvTcp_ExceptionOccurred		0x00000001	/** <classid>LOG_EXCEPTION</classid>	<severity>Restart communication server</severity> */
#define LOGID_CmpBlkDrvTcp_InvalidIpAddress			0x00000002	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvTcp_CreateListenSocketFailed	0x00000003	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_ConfigListenSocketFailed	0x00000004	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_RegisterDriverFailed		0x00000005	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_IpAddress				0x00000006	/** <classid>LOG_INFO</classid>			<severity>none</severity> */
#define LOGID_CmpBlkDrvTcp_CouldNotSetTcpKeeplive	0x00000007	/** <classid>LOG_WARNING</classid>		<severity>Low communication performance</severity> */
#define LOGID_CmpBlkDrvTcp_CouldNotSetTcpNoDelay	0x00000008	/** <classid>LOG_WARNING</classid>		<severity>Low communication performance</severity> */
#define LOGID_CmpBlkDrvTcp_CouldNotSetNonBlocking	0x00000009	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvTcp_CouldNotAllocateMemory	0x0000000A	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvTcp_CreateClientSocketFailed	0x0000000B	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_ConfigClientSocketFailed	0x0000000C	/** <classid>LOG_ERROR</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_ConnectCallFailed		0x0000000D	/** <classid>LOG_DEBUG</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_ConnectFailed			0x0000000E	/** <classid>LOG_DEBUG</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_SendFailed				0x0000000F	/** <classid>LOG_DEBUG</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_SocketClosed				0x00000010	/** <classid>LOG_DEBUG</classid>		<severity>none</severity> */ 
#define LOGID_CmpBlkDrvTcp_SocketClosedByTO			0x00000011	/** <classid>LOG_DEBUG</classid>		<severity>none</severity> */ 
#define LOGID_CmpBlkDrvTcp_Connected				0x00000012	/** <classid>LOG_DEBUG</classid>		<severity>none</severity> */ 
#define LOGID_CmpBlkDrvTcp_IOCtrlFailed				0x00000013	/** <classid>LOG_DEBUG</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_RecvFailed				0x00000014	/** <classid>LOG_DEBUG</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_MagicIdMismatch			0x00000015	/** <classid>LOG_DEBUG</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_Accepted					0x00000016	/** <classid>LOG_DEBUG</classid>		<severity>none</severity> */ 
#define LOGID_CmpBlkDrvTcp_SelectFailed				0x00000017	/** <classid>LOG_WARNING</classid>		<severity>Low communication performance</severity> */
#define LOGID_CmpBlkDrvTcp_RecvLenInvalid			0x00000018	/** <classid>LOG_DEBUG</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_InvalidLocalIpAddress	0x00000019	/** <classid>LOG_ERROR</classid>		<severity>Wrong communication setup</severity> */
#define LOGID_CmpBlkDrvTcp_InvalidPeerIpAddress		0x0000001A	/** <classid>LOG_ERROR</classid>		<severity>Wrong communication setup</severity> */
#define LOGID_CmpBlkDrvTcp_ServerDisabled			0x0000001B	/** <classid>LOG_INFO</classid>			<severity>none</severity> */ 
#define LOGID_CmpBlkDrvTcp_ClientDisabled			0x0000001C	/** <classid>LOG_INFO</classid>			<severity>none</severity> */ 
#define LOGID_CmpBlkDrvTcp_LocalIpAddress			0x0000001D	/** <classid>LOG_INFO</classid>			<severity>none</severity> */ 
#define LOGID_CmpBlkDrvTcp_PeerIpAddress			0x0000001E	/** <classid>LOG_INFO</classid>			<severity>none</severity> */ 
#define LOGID_CmpBlkDrvTcp_TryToConnect				0x0000001F	/** <classid>LOG_DEBUG</classid>		<severity>none</severity> */
#define LOGID_CmpBlkDrvTcp_ConnectSuppressed		0x00000020	/** <classid>LOG_DEBUG</classid>		<severity>none</severity> */
#define LOGID_CmpBlkDrvTcp_ConnectionClosedByPeer	0x00000021	/** <classid>LOG_DEBUG</classid>		<severity>none</severity> */
#define LOGID_CmpBlkDrvTcp_ServerLimitReached		0x00000022	/** <classid>LOG_DEBUG</classid>		<severity>none</severity> */
#define LOGID_CmpBlkDrvTcp_RecvHeaderFailed			0x00000023	/** <classid>LOG_DEBUG</classid>		<severity>No communication possible</severity> */ 
#define LOGID_CmpBlkDrvTcp_RecvAddrCorrectionFailed	0x00000024	/** <classid>LOG_DEBUG</classid>		<severity>No communication possible</severity> */ 

/**
 *  <desription>CmpBlkDrvShm</desription>
 */
#define LOGID_CmpBlkDrvShm_NameTooLong				0x00000001	/** <classid>LOG_ERROR</classid>	<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvShm_RegisteringFailed		0x00000002	/** <classid>LOG_ERROR</classid>	<severity>No communication possible</severity> */
#define LOGID_CmpBlkDrvShm_CreateCommThreadFailed	0x00000003	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBlkDrvShm_ExceptionOccurred		0x00000004	/** <classid>LOG_EXCEPTION</classid>	<severity>Restart communication server</severity> */
#define LOGID_CmpBlkDrvShm_InvalidReceiveBlock		0x00000005	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBlkDrvShm_CreateEventFailed		0x00000006	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBlkDrvShm_AllocChannelFailed		0x00000007	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBlkDrvShm_AllocBroadcastFailed		0x00000008	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBlkDrvShm_OutputLocalAddres		0x00000009	/** <classid>LOG_INFO</classid>	<severity>none</severity> */

/**
 *  <desription>CmpBlkDrvCanClient</desription>
 */
#define LOGID_CmpBlkDrvCanClient_NotEnoughMem		0x00000001	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBlkDrvCanClient_DriverNotStarted	0x00000002	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBlkDrvCanClient_CanOpened		0x00000003	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpBlkDrvCanClient_CanOpenFailed		0x00000004	/** <classid>LOG_INFO</classid>	<severity>none</severity> */


/**
 *  <desription>CmpChannelMgr</desription>
 */
#define LOGID_CmpChannelMgr_NetworkClient			0x00000001	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpChannelMgr_NetworkServer			0x00000002	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpChannelMgr_NoClientOrServer		0x00000003	/** <classid>LOG_ERROR</classid>	<severity>No communication possible</severity> */
#define LOGID_CmpChannelMgr_CustomChannelTimeout	0x00000005	/** <classid>LOG_INFO</classid>	<severity>none</severity> */


/**
 *  <desription>CmpChannelServer</desription>
 */
#define LOGID_CmpChannelServer_ChannelCreated		0x00000001	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpChannelServer_ChannelClosed		0x00000002	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpChannelServer_RequestReceived		0x00000003	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpChannelServer_ReplySent			0x00000004	/** <classid>LOG_INFO</classid>		<severity>none</severity> */


 /**
 *  <desription>CmpNameServiceServer</desription>
 */
#define LOGID_CmpNameServiceServer_UnexpectedPackageType		0x00000001	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpNameServiceServer_NodeNameTruncated			0x00000002	/** <classid>LOG_WARNING</classid>	<severity>Possibly no communication</severity> */
#define LOGID_CmpNameServiceServer_DeviceVenoderNameTruncated	0x00000003	/** <classid>LOG_WARNING</classid>	<severity>Truncated names displayed</severity> */
#define LOGID_CmpNameServiceServer_SerialNumberTruncated		0x00000004	/** <classid>LOG_WARNING</classid>	<severity>Truncated serial number displayed</severity> */
#define LOGID_CmpNameServiceServer_OemDataSkipped				0x00000005	/** <classid>LOG_ERROR</classid>	<severity>OEM data are not sent</severity> */


/**
 *  <desription>CmpIoMgr</desription>
 */
#define LOGID_CmpIoMgr_UpdateConfigurationFailed		0x00000001	/** <classid>LOG_ERROR</classid>	<severity>No IO data exchange possible via this driver</severity> */
#define LOGID_CmpIoMgr_UpdateMappingFailed				0x00000002	/** <classid>LOG_ERROR</classid>	<severity>No IO data exchange possible via this driver</severity> */
#define LOGID_CmpIoMgr_DuplicateConfigApplication		0x00000003	/** <classid>LOG_ERROR</classid>	<severity>No IO data exchange possible on this application</severity> */
#define LOGID_CmpIoMgr_CreatingWatchdogFailed			0x00000004	/** <classid>LOG_ERROR</classid>	<severity>No IO data exchange possible on this application</severity> */
#define LOGID_CmpIoMgr_IoConfigParamMismatch			0x00000005	/** <classid>LOG_ERROR</classid>	<severity>No restore of IO-config parameter values possible</severity> */
#define LOGID_CmpIoMgr_IoConfigParamLineNoBuffer		0x00000006	/** <classid>LOG_ERROR</classid>	<severity>Restore of IO-config parameter values may be lost</severity> */
#define LOGID_CmpIoMgr_IoConfigParamNotPossible			0x00000007	/** <classid>LOG_ERROR</classid>	<severity>Restore of IO-config parameter values was not possible</severity> */
#define LOGID_CmpIoMgr_WatchdogTriggerFailed			0x00000008	/** <classid>LOG_ERROR</classid>	<severity>Watchdog trigger failed</severity> */


/**
 *  <desription>CmpMonitor</desription>
 */
#define LOGID_CmpMonitor_DataSizeError				0x00000001	/** <classid>LOG_ERROR</classid>	<severity>No answer from target</severity> */
#define LOGID_CmpMonitor_WriteDenied				0x00000002	/** <classid>LOG_ERROR</classid>	<severity>Writing values denied</severity> */
#define LOGID_CmpMonitor_ForceDenied				0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Forcing values denied</severity> */
#define LOGID_CmpMonitor_MemoryOverwrite            0x00000004  /** <classid>LOG_ERROR</classid>	<severity>Memory overwritten while executing the byte-code</severity> */

/**
 *  <desription>CmpIecTask</desription>
 */
#define LOGID_CmpIecTask_NoApplication				0x00000001	/** <classid>LOG_ERROR</classid>	<severity>No task of this application is created</severity> */
#define LOGID_CmpIecTask_CreateTaskFailed			0x00000002	/** <classid>LOG_ERROR</classid>	<severity>One task of this application is not created</severity> */
#define LOGID_CmpIecTask_Exception					0x00000003	/** <classid>LOG_EXCEPTION</classid>	<severity>This task can not be executed</severity> */
#define LOGID_CmpIecTask_Exception_StopTimeout		0x00000004	/** <classid>LOG_EXCEPTION</classid>	<severity>Timeout occurred to switch in stop mode</severity> */
#define LOGID_CmpIecTask_SupervisorOperationRegisterFailed		0x00000005	/** <classid>LOG_ERROR</classid>	<severity>Supervisor operation register failed</severity> */
#define LOGID_CmpIecTask_MultiCoreLicenseMissing	0x00000006	/** <classid>LOG_WARNING</classid>	<severity>For this multi-core configuration, license is missing!</severity> */
#define LOGID_CmpIecTask_MultiCoreDemoModeExpired	0x00000007	/** <classid>LOG_EXCEPTION</classid>	<severity>Application remains in exception state, because demo mode has expired!</severity> */

/**
 *  <desription>CmpRouter</desription>
 */
#define LOGID_CmpRouter_Address						0x00000001	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpRouter_AddressTypeInvalid			0x00000002	/** <classid>LOG_WARNING</classid>	<severity>none</severity> */
#define LOGID_CmpRouter_NoMemory					0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Router instance could not be created</severity> */
#define LOGID_CmpRouter_RegisterInterface			0x00000004	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpRouter_UnregisterInterface			0x00000005	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpRouter_ParallelRoutingDisabled		0x00000006	/** <classid>LOG_WARNING</classid>	<severity>none</severity> */
#define LOGID_CmpRouter_MultipleParentNodes			0x00000007	/** <classid>LOG_WARNING</classid>	<severity>none</severity> */
#define LOGID_CmpRouter_MainnetNotFound				0x00000008	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpRouter_SubnetNotFound				0x00000009	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpRouter_UnregisterInvalidNetwork	0x0000000A	/** <classid>LOG_WARNING</classid>	<severity>none</severity> */
#define LOGID_CmpRouter_InvalidAddrRegistered		0x0000000B	/** <classid>LOG_ERROR</classid>	<severity>Some network addreseses may not be reachable</severity> */
#define LOGID_CmpRouter_TcpInSubNet					0x0000000C	/** <classid>LOG_WARNING</classid>	<severity>Setting is ignored</severity> */
#define LOGID_CmpRouter_NoInstanceFound				0x0000000D	/** <classid>LOG_WARNING</classid>	<severity>Driver instance is not used for communication</severity> */
#define LOGID_CmpRouter_TooMuchSubnets				0x0000000E	/** <classid>LOG_WARNING</classid>	<severity>Setting is ignored</severity> */
#define LOGID_CmpRouter_TooMuchRouters				0x0000000F	/** <classid>LOG_INFO</classid>		<severity>Router configuration was automatically adapted</severity> */
#define LOGID_CmpRouter_NoMemoryForSubnets			0x00000010	/** <classid>LOG_ERROR</classid>	<severity>Subnet for router instance could not be created</severity> */
#define LOGID_CmpRouter_NoMemoryForQueueHeaders		0x00000011	/** <classid>LOG_ERROR</classid>	<severity>Queueheaders could not be allocated</severity> */
#define LOGID_CmpRouter_InitFailed					0x00000012	/** <classid>LOG_ERROR</classid>	<severity>Router could not be initialized</severity> */
#define LOGID_CmpRouter_RouterInstances				0x00000013	/** <classid>LOG_INFO</classid>		<severity>Number of router instances</severity> */

/**
 *  <desription>CmpSchedule</desription>
 */
#define LOGID_CmpSchedule_TimerResolutionTooLess	0x00000001	/** <classid>LOG_ERROR</classid>	<severity>Scheduler cannot operate</severity> */
#define LOGID_CmpSchedule_TimesliceConfigError		0x00000002	/** <classid>LOG_ERROR</classid>	<severity>Timeslicing is not operating</severity> */
#define LOGID_CmpSchedule_Exception					0x00000003	/** <classid>LOG_EXCEPTION</classid>	<severity>Scheduler cannot operate</severity> */
#define LOGID_CmpSchedule_StartTimerFailed			0x00000004	/** <classid>LOG_ERROR</classid>	<severity>Scheduler operates with a task</severity> */
#define LOGID_CmpSchedule_CreateTimerFailed			0x00000005	/** <classid>LOG_ERROR</classid>	<severity>Scheduler operates with a task</severity> */
#define LOGID_CmpSchedule_TimerNotAvailable			0x00000006	/** <classid>LOG_ERROR</classid>	<severity>Scheduler operates with a task</severity> */
#define LOGID_CmpSchedule_TimerNotAvailable			0x00000006	/** <classid>LOG_ERROR</classid>	<severity>Scheduler operates with a task</severity> */

#define LOGID_CmpSchedule_TaskResume				0x00000007	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpSchedule_TaskSuspend				0x00000008	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpSchedule_TaskEnd					0x00000009	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpSchedule_TaskStart					0x0000000A	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpSchedule_TimesliceBegin			0x0000000B	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpSchedule_TimesliceEnd				0x0000000C	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpSchedule_ScheduleTick				0x0000000D	/** <classid>LOG_INFO</classid>	<severity>none</severity> */

#define LOGID_CmpSchedule_Watchdog_Exception		0x0000000E	/** <classid>LOG_EXCEPTION</classid>	<severity>Task cannot operate</severity> */
#define LOGID_CmpSchedule_Watchdog_Exception2		0x0000000F	/** <classid>LOG_EXCEPTION</classid>	<severity>Task cannot operate</severity> */
#define LOGID_CmpSchedule_Watchdog_Exception3		0x00000010	/** <classid>LOG_EXCEPTION</classid>	<severity>Task cannot operate</severity> */
#define LOGID_CmpSchedule_Watchdog_Exception4		0x00000011	/** <classid>LOG_EXCEPTION</classid>	<severity>Task cannot operate</severity> */

#define LOGID_CmpSchedule_TaskDelete_Failed			0x00000012	/** <classid>LOG_ERROR</classid>	<severity>Task cannot operate</severity> */
#define LOGID_CmpSchedule_BindTaskToCoreFailed		0x00000013	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpSchedule_SupervisorOperationRegisterFailed		0x00000014	/** <classid>LOG_ERROR</classid>	<severity>Component not supervised</severity> */
#define LOGID_CmpSchedule_BindTaskToGroupFailed		0x00000015	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */

#define LOGID_CmpSchedule_ProcessorloadWatchdog_Exception	0x00000016	/** <classid>LOG_EXCEPTION</classid>	<severity>none</severity> */
#define LOGID_CmpSchedule_CreateTaskGroupFailed				0x00000017	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpSchedule_TaskGroupBindingReconfigured		0x00000018	/** <classid>LOG_WARNING/classid>		<severity>none</severity> */

#define LOGID_CmpSchedule_CreateTaskGapEventFailed			0x00000019	/** <classid>LOG_ERROR</classid>	<severity>Scheduler uses fallback for task gap handling</severity> */

/**
 *  <desription>CmpSrv</desription>
 */
#define LOGID_CmpSrv_InvalidContentPointer			0x00000001	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpSrv_AsyncServiceFailed				0x00000002	/** <classid>LOG_EXCEPTION</classid>	<severity>Async service execution caused an error.</severity> */


/**
 *  <desription>CmpTargetVisu</desription>
 */
#define LOGID_CmpTargetVisu_CreateTargetVisuPoolFailed		0x00000001	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateCallbackPoolFailed		0x00000002	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateSemaphoreFailed			0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_InstancesLeft					0x00000004	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_InvalidExternId					0x00000005	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateVisuThreadFailed			0x00000006	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_Exception						0x00000007	/** <classid>LOG_EXCEPTION</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_ThreadNotYetShudown				0x00000008	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_ThreadFinished					0x00000009	/** <classid>LOG_INFO</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateBackbufferFailed			0x0000000A	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateFontBackbufferFailed		0x0000000B	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateBackgroundBufferFailed	0x0000000C	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_WindowCallbackFailed			0x0000000D	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_GetInstanceHandleFailed			0x0000000E	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateWindowFailed				0x0000000F	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_RegisterCyclicCallbackFailed	0x00000010	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateBitmapBufferFailed		0x00000011	/** <classid>LOG_WARNING</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_GetClientIdFailed				0x00000012	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_InitializeWindowFailed			0x00000013	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_MissingResources				0x00000014	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateDefaultItemsFailed		0x00000015	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_NoMoreMemory					0x00000016	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_SetClipRectangleFailed			0x00000017	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_UnClipRectangleFailed			0x00000018	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_DelayedExecutionLocked			0x00000019	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateEmbeddedTargetVisuFailed	0x0000001A	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_GesturesInitFailed				0x0000001B	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_TouchHandlingInitFailed			0x0000001C	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_CreateWindowAccessSemFailed		0x0000001D	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */
#define LOGID_CmpTargetVisu_OverlayNotSupported				0x0000001E	/** <classid>LOG_ERROR</classid>		<severity>Service cannot be handled</severity> */


/**
 *  <desription>CmpVisuServer</desription>
 */
#define LOGID_CmpVisuServer_GetPaintServiceOldVersion		0x00000001	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuServer_NoMemoryForAdditionalData		0x00000002	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuServer_UnexpectedBintagUtilError		0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuServer_GetPaintServiceUnexpectedSize	0x00000004	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuServer_MissingCommandBuffer			0x00000005	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuServer_StoringClientFailed				0x00000006	/** <classid>LOG_WARNING</classid>	<severity>notification only</severity> */
#define LOGID_CmpVisuServer_ContinuationMgrAddEntryFailed	0x00000007	/** <classid>LOG_WARNING</classid>	<severity>notification only</severity> */
#define LOGID_CmpVisuServer_ContinuationMgrInitFailed		0x00000008	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuServer_ContinuationMgrExitFailed		0x00000009	/** <classid>LOG_WARNING</classid>	<severity>notification only</severity> */

/**
 *  <desription>CmpBitmapPool</desription>
 */
#define LOGID_CmpBitmapPool_BitmapPoolFailed			0x00000001	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_InvalidParameter			0x00000002	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_NoSyFile				0x00000003	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_NoFilePath				0x00000004	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_NameTooLong				0x00000005	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_ContentfileNotFound			0x00000006	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_ContentfileExists			0x00000007	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_BasepathnameTooLong			0x00000008	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_BitmapPoolNotSaved			0x00000009	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_ContentfileInvalidLine		0x0000000A	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_ContentfileLongLine			0x0000000B	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_NoMemory				0x0000000C	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_ContentfileRemoved			0x0000000D	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_NoSysTimeRtc				0x0000000E	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpBitmapPool_NoSysDir				0x0000000F	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */

/**
 *  <desription>CmpDynamicText</desription>
 */
#define LOGID_CmpDynamicText_CreateMemPoolFailed			0x00000001	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_CmpDynamicText_GetMemBlockFailed				0x00000002	/** <classid>LOG_ERROR</classid>	<severity>none</severity> */

/**
 *  <desription>CmpWebServer</desription>
 */
#define LOGID_CmpWebServer_CreateTaskFailed				0x00000001	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServer_SendServiceFailed			0x00000002	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServer_InvalidCommAddress			0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServer_InitializeFailed				0x00000004	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServer_StartConnectionFailed		0x00000005	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServer_OpeningChannelFailed			0x00000006	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServer_CloseChannelFailed			0x00000007	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServer_ReadStatusFailed				0x00000008	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServer_LicenseFailed				0x00000009	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServer_CreateMemPoolFailed			0x0000000A	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServer_InvalidStartupType			0x0000000B	/** <classid>LOG_WARNING</classid>	<severity>none</severity> */
#define LOGID_CmpWebServer_WebserverStarted				0x0000000C	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpWebServer_WebserverStopped				0x0000000D	/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_CmpWebServer_UnbalancedStartStopCalls		0x0000000E	/** <classid>LOG_WARNING</classid>		<severity>none</severity> */

/**
 * <desription>CmpWebServerHandlerV3</desription>
 */
#define LOGID_CmpWebServerHandlerV3_InvalidCommAddress		0x00000001	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServerHandlerV3_LicenseFailed			0x00000002	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServerHandlerV3_SendServiceFailed		0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServerHandlerV3_CheckService			0x00000004	/** <classid>LOG_DEBUG</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpWebServerHandlerV3_StateShutdownRTS		0x00000005	/** <classid>LOG_INFO</classid>		<severity>notification only</severity> */


/**
 *  <desription>CmpVisuHandler</desription>
 */
#define LOGID_CmpVisuHandler_GetPaintServiceOldVersion	0x00000001	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandler_VisuInfoTupleNotCreated	0x00000002	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandler_InvalidVisuLibraries		0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandler_UnknownApplication			0x00000004	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandler_InvalidCreationInfo		0x00000005	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandler_InvalidExternId			0x00000006	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandler_LTypesNotSupported			0x00000007	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandler_PrintfWNotSupported		0x00000008	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandler_VisuInfoTupleNotFound		0x00000009	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandler_PrintfOverflow				0x0000000A	/** <classid>LOG_ERROR</classid>	<severity>Critical Error</severity> */
#define LOGID_CmpVisuHandler_InpuEventDropped			0x0000000B	/** <classid>LOG_WARNING</classid>	<severity>Service cannot be handled</severity> */


/**
 *  <desription>CmpVisuHandlerRemote</desription>
 */
#define LOGID_CmpVisuHandlerRemote_NoDestinationAddress			0x00000001	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_StartConnectionFailed		0x00000002	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_InvalidCommBufferSize		0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_InvalidPaintBufferSize		0x00000004	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_OpeningChannelFailed			0x00000005	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_SendUploadImagePoolFailed	0x00000006	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_NoVisuAppName				0x00000007	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_CreateTargetVisuFailed		0x00000008	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_CreateTaskFailed				0x00000009	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_BuildSrvRegisterClientFailed	0x0000000A	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_CreateCommEventFailed		0x0000000B	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_SendRegisterClientFailed		0x0000000C	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_ParseRegisterClientResFailed	0x0000000D	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_BuildSrvIsRegisteredFailed	0x0000000E	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_SendIsRegisteredFailed		0x0000000F	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_ParseIsRegisteredResFailed	0x00000010	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_BuildSrvGetPaintDataFailed	0x00000011	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_SendGetPaintDataFailed		0x00000012	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_ParseGetPaintDataResFailed	0x00000013	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_DebugMessage					0x00000014	/** <classid>LOG_DEBUG</classid>	<severity>none</severity> */
#define LOGID_CmpVisuHandlerRemote_StartingVisuClient			0x00000015	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpVisuHandlerRemote_StaticBufferTooSmall			0x00000016	/** <classid>LOG_WARNING</classid>	<severity>warning</severity> */
#define LOGID_CmpVisuHandlerRemote_AllocatingBufferFailed		0x00000017	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_PaintBufferTooSmall			0x00000018	/** <classid>LOG_WARNING</classid>	<severity>warning</severity> */
#define LOGID_CmpVisuHandlerRemote_CreateSemaphoreFailed		0x00000019	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_TargetvisuClosed				0x0000001A	/** <classid>LOG_INFO</classid>	<severity>none</severity> */
#define LOGID_CmpVisuHandlerRemote_CloseChannelFailed			0x0000001B	/** <classid>LOG_ERROR</classid>	<severity>Service cannot be handled</severity> */
#define LOGID_CmpVisuHandlerRemote_InvalidReconnectTime			0x0000001C	/** <classid>LOG_WARNING</classid>	<severity>warning</severity> */
#define LOGID_CmpVisuHandlerRemote_StartReconnect				0x0000001D	/** <classid>LOG_WARNING</classid>	<severity>warning</severity> */
#define LOGID_CmpVisuHandlerRemote_InvalidCommAddress			0x0000001E	/** <classid>LOG_WARNING</classid>	<severity>warning</severity> */
#define LOGID_CmpVisuHandlerRemote_NoStartVisu						0x0000001F	/** <classid>LOG_ERROR</classid>	<severity>Visu client will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_BuildSrvWriteImagePoolFailed		0x00000020	/** <classid>LOG_ERROR</classid>	<severity>Images will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_SendWriteImagePoolFailed			0x00000021	/** <classid>LOG_WARNING</classid>	<severity>Images will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_ParseSrvWriteImagePoolFailed		0x00000022	/** <classid>LOG_WARNING</classid>	<severity>Images will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_FileUploadFailedCouldNotOpen		0x00000023	/** <classid>LOG_ERROR</classid>	<severity>A file could not be uploaded</severity> */	
#define LOGID_CmpVisuHandlerRemote_UploadImagePoolFailed			0x00000024	/** <classid>LOG_WARNING</classid>	<severity>Images will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_ReadingImagePoolFailed			0x00000025	/** <classid>LOG_WARNING</classid>	<severity>Images will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_DeleteRemoteImagePoolFailed		0x00000026	/** <classid>LOG_WARNING</classid>	<severity>Obsolete file on the plc not deleted</severity> */	
#define LOGID_CmpVisuHandlerRemote_UploadingSingleImageFailed		0x00000027	/** <classid>LOG_WARNING</classid>	<severity>Image will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_SrvWriteImagePoolNotImplemented	0x00000028	/** <classid>LOG_WARNING</classid>	<severity>Images will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_BuildSrvPostEventFailed			0x00000029	/** <classid>LOG_ERROR</classid>	<severity>Visu client might not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_SendPostEventFailed				0x0000002A	/** <classid>LOG_ERROR</classid>	<severity>Visu client might not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_ParsePostEventResponseFailed		0x0000002B	/** <classid>LOG_ERROR</classid>	<severity>Visu client might not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_SrvPostEventNotImplemented		0x0000002C	/** <classid>LOG_WARNING</classid>	<severity>Paintbugs might occur</severity> */	
#define LOGID_CmpVisuHandlerRemote_DeviceLoginFailed				0x0000002D	/** <classid>LOG_ERROR</classid>	<severity>Visu client might not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_DeviceLoginIgnoreNoUserMgr		0x0000002E	/** <classid>LOG_WARNING</classid>	<severity>Visu client might not work correctly</severity> */	
#define LOGID_CmpVisuHandlerRemote_DeviceLoginSendFailed			0x0000002F	/** <classid>LOG_WARNING</classid>	<severity>Visu client might not work correctly</severity> */	
#define LOGID_CmpVisuHandlerRemote_DeviceLoginFailedInvalidUser		0x00000030	/** <classid>LOG_WARNING</classid>	<severity>Visu client might not work correctly</severity> */	
#define LOGID_CmpVisuHandlerRemote_DeviceLoginFailedInvalidPassword	0x00000031	/** <classid>LOG_WARNING</classid>	<severity>Visu client might not work correctly</severity> */	
#define LOGID_CmpVisuHandlerRemote_DeviceLoginFailedUnexpected		0x00000032	/** <classid>LOG_WARNING</classid>	<severity>Visu client might not work correctly</severity> */	
#define LOGID_CmpVisuHandlerRemote_DeviceLoginFailedNoAnswer		0x00000033	/** <classid>LOG_WARNING</classid>	<severity>Visu client might not work correctly</severity> */	
#define LOGID_CmpVisuHandlerRemote_LoginRequired					0x00000034	/** <classid>LOG_WARNING</classid>	<severity>Visu client might not work correctly</severity> */	
#define LOGID_CmpVisuHandlerRemote_VisuNotYetRegisteredInfo			0x00000035	/** <classid>LOG_INFO</classid>		<severity/> */	
#define LOGID_CmpVisuHandlerRemote_StartupConnectionFailed			0x00000036	/** <classid>LOG_ERROR</classid>	<severity>Visu client will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_ResolvingAddressFailed			0x00000037	/** <classid>LOG_ERROR</classid>	<severity>Visu client will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_NSClientNotImplemented			0x00000038	/** <classid>LOG_ERROR</classid>	<severity>Visu client will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_ScanningNetwork					0x00000039	/** <classid>LOG_INFO</classid>		<severity/> */	
#define LOGID_CmpVisuHandlerRemote_InvalidNetworkScanWaitTime		0x0000003A	/** <classid>LOG_WARNING</classid>	<severity>warning</severity> */
#define LOGID_CmpVisuHandlerRemote_ScanningNetwork_NothingFound		0x0000003B	/** <classid>LOG_ERROR</classid>	<severity>Visu client will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_ScanningNetwork_Ambigous			0x0000003C	/** <classid>LOG_ERROR</classid>	<severity>Visu client will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_StaticAddressConnection			0x0000003D	/** <classid>LOG_INFO</classid>		<severity/> */	
#define LOGID_CmpVisuHandlerRemote_EventCreationFailed				0x0000003E	/** <classid>LOG_WARNING</classid>	<severity>Visu client might not work correctly</severity> */	
#define LOGID_CmpVisuHandlerRemote_TcpResolutionFailed				0x0000003F	/** <classid>LOG_ERROR</classid>	<severity>Visu client might not work correctly</severity> */	
#define LOGID_CmpVisuHandlerRemote_TcpInvalidAddrConfigured			0x00000040	/** <classid>LOG_ERROR</classid>	<severity>Visu client will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_TcpComponentsMissing				0x00000041	/** <classid>LOG_ERROR</classid>	<severity>Visu client will not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_TcpInvalidPortNr					0x00000042	/** <classid>LOG_ERROR</classid>	<severity>Visu client might not work</severity> */	
#define LOGID_CmpVisuHandlerRemote_TcpResolving						0x00000043	/** <classid>LOG_INFO</classid>		<severity/> */
#define LOGID_CmpVisuHandlerRemote_ImageReloadFailed				0x00000044	/** <classid>LOG_WARNING</classid>	<severity>Dynamic image will flicker</severity> */
#define LOGID_CmpVisuHandlerRemote_DeviceNoAccessRights				0x00000045	/** <classid>LOG_DEBUG</classid>	<severity>No access rights. Requires username and credentials</severity> */
#define LOGID_CmpVisuHandlerRemote_DeviceTooManyRetries				0x00000046	/** <classid>LOG_WARNING</classid>	<severity>Login failed due to many wrong username/password attempts</severity> */



/**
 *  <desription>CmpRetain</desription>
 */
#define LOGID_CmpRetain_RetainNotStored					0x00000001	/** <classid>LOG_WARNING</classid>	<severity>Retains don't work</severity> */
#define LOGID_CmpRetain_RetainReinit					0x00000002	/** <classid>LOG_WARNING</classid>	<severity>Retains might be lost</severity> */
#define LOGID_CmpRetain_NoMemory						0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Failed to open retain memory</severity> */


/**
 *  <desription>CmpUserMgr</desription>
 */
#define LOGID_CmpUserMgr_AuthKeyLenAdapted				0x00000001	/** <classid>LOG_WARNING</classid>	<severity>Configured key length replaced by allowed minimum value</severity> */


/**
 *  <desription>CmpHalconIntegration</desription>
 */
#define LOGID_CmpHalconIntegration_Failed				0x00000001	/** <classid>LOG_ERROR</classid>	<severity>HALCON library call failed</severity> */


/**
 *  <desription>CmpTraceMgr</desription>
 */
#define LOGID_CmpTraceMgr_CreatingEventsFailed			0x00000001	/** <classid>LOG_ERROR</classid>	<severity>Necessary events could not be created</severity> */
#define LOGID_CmpTraceMgr_RestoreGuid				0x00000002	/** <classid>LOG_ERROR</classid>	<severity>Data Guid Mismatch when restoring a trace packet.</severity> */
#define LOGID_CmpTraceMgr_RestoreApp				0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Application not found on restoring a trace packet.</severity> */
#define LOGID_CmpTraceMgr_StoreEmptyName			0x00000004	/** <classid>LOG_ERROR</classid>	<severity>Packet could not be stored, because it has no name.</severity> */
#define LOGID_CmpTraceMgr_RestoreOnBoot				0x00000005	/** <classid>LOG_ERROR</classid>	<severity>Failed to restore trace packet at boot time.</severity> */
#define LOGID_CmpTraceMgr_Restored				0x00000006	/** <classid>LOG_INFO</classid>		<severity>Packet successfully restored.</severity> */


/**
 *  <desription>CmpDynamicText</desription>
 */
#define LOGID_CmpDynamicText_TextFileNotFound			0x00000001	/** <classid>LOG_ERROR</classid>	<severity>A file containing text lists was not found</severity> */
#define LOGID_CmpDynamicText_CmpXMLParser_NotAvailable	0x00000002	/** <classid>LOG_ERROR</classid>	<severity>XML parser not available</severity> */
#define LOGID_CmpDynamicText_CreateSemaphoreFailed		0x00000003	/** <classid>LOG_ERROR</classid>	<severity>Semaphore for synchronization could not be allocated</severity> */


/**
 *  <desription>CmpFileTransfer</desription>
 */
/*#define LOGID_CmpFileTransfer_AsyncServiceFailed		0x00000001*//** <classid>LOG_EXCEPTION</classid>	<severity>Async file transfer service failed</severity> */
#define LOGID_CmpFileTransfer_DownloadDenied			0x00000002	/** <classid>LOG_ERROR | LOG_USER_NOTIFY</classid>	<severity>File download denied</severity> */
#define LOGID_CmpFileTransfer_UploadDenied				0x00000003	/** <classid>LOG_ERROR | LOG_USER_NOTIFY</classid>	<severity>File upload denied</severity> */
#define LOGID_CmpFileTransfer_FileDeleteDenied			0x00000004	/** <classid>LOG_ERROR | LOG_USER_NOTIFY</classid>	<severity>File delete denied</severity> */
#define LOGID_CmpFileTransfer_FileRenameDenied			0x00000005	/** <classid>LOG_ERROR | LOG_USER_NOTIFY</classid>	<severity>File rename denied</severity> */
#define LOGID_CmpFileTransfer_DirCreateDenied			0x00000006	/** <classid>LOG_ERROR | LOG_USER_NOTIFY</classid>	<severity>Directory create denied</severity> */
#define LOGID_CmpFileTransfer_DirDeleteDenied			0x00000007	/** <classid>LOG_ERROR | LOG_USER_NOTIFY</classid>	<severity>Directory delete denied</severity> */
#define LOGID_CmpFileTransfer_DirRenameDenied			0x00000008	/** <classid>LOG_ERROR | LOG_USER_NOTIFY</classid>	<severity>Directroy rename denied</severity> */


/**
 *  <desription>CmpPlcShell</desription>
 */
#define LOGID_CmpPlcShell_RegisterEventFailed			0x00000001	/** <classid>LOG_EXCEPTION</classid>	<severity>Register of the Command Handler Event failed</severity> */


/**
 *  <desription>CmpDevice</desription>
 */
#define LOGID_CmpDevice_SettingOperationModeDenied		0x00000001	/** <classid>LOG_ERROR | LOG_USER_NOTIFY</classid>	<severity>Setting new Operation mode denied</severity> */
#define LOGID_CmpDevice_OperationalModeBootAppMismatch	0x00000002	/** <classid>LOG_ERROR</classid>	<severity>Bootapplication denied to start</severity> */
#define LOGID_CmpDevice_ResetOriginDeviceDenied			0x00000003	/** <classid>LOG_ERROR | LOG_USER_NOTIFY</classid>	<severity>Reset origin device denied</severity> */
#define LOGID_CmpDevice_SessionLoginCheckFailed			0x00000004	/** <classid>LOG_ERROR </classid>	<severity>No communication possible</severity> */
#define LOGID_CmpDevice_OperationDisabled				0x00000005	/** <classid>LOG_ERROR </classid>	<severity>Operation is disabled</severity> */
#define LOGID_CmpDevice_NoValidSession					0x00000006	/** <classid>LOG_ERROR </classid>	<severity>No communication possible</severity> */
#define LOGID_CmpDevice_SessionNotAvailableAnymore		0x00000007	/** <classid>LOG_ERROR </classid>	<severity>No communication possible</severity> */
#define LOGID_CmpDevice_SessionTimeout					0x00000008	/** <classid>LOG_ERROR </classid>	<severity>No communication possible</severity> */

/**
 *  <desription>SysFile</desription>
 */
#define LOGID_SysFile_BackupRestoreInactive				0x00000001	/** <classid>LOG_INFO</classid>	<severity>Backup/restore feature should be activated</severity> */
#define LOGID_SysFile_BackupRestoreOff					0x00000002	/** <classid>LOG_INFO</classid>	<severity>Backup/restore feature cannot be used</severity> */

/**
 *  <desription>SysEthernet</desription>
 */
#define LOGID_SysEthernet_NoCmpBinTagUtil				0x00000001	/** <classid>LOG_ERROR</classid>	<severity>No online service possible</severity> */
#define LOGID_SysEthernet_NoCmpSrv						0x00000002	/** <classid>LOG_ERROR</classid>	<severity>No online service possible</severity> */

/**
 *  <desription>SysInternalLibDefault</desription>
 */
#define LOGID_SysInternalLibDefault_UserHandledException	0x00000001	/** <classid>LOG_ERROR</classid>	<severity>User handled Exception</severity> */

/**
 *	<description>SysNativeControl</description>
 */
#define LOGID_SysNativeControl_InvalidCast		0

/**
 *  <desription>SysWindowNative</desription>
 */
#define	LOGID_SysWindowNative_NoTimerTaskDueToMissingSysTask		0
#define	LOGID_SysWindowNative_TimerTaskCreationFailed				1
#define LOGID_SysWindowNative_MethodNotImplemented					2
#define LOGID_SysWindowNative_InvalidRectangleDropped 				3
#define LOGID_SysWindowNative_PrintingNotSupported	 				4
#define LOGID_SysWindowNative_InputDroppedQueueFull					5
#define LOGID_SysWindowNative_InputCreateSemaphoreFailed			6
#define LOGID_SysWindowNative_InputCreateEventFailed				7
#define LOGID_SysWindowNative_NoSemaphoreUsed						8
#define LOGID_SysWindowNative_NoEventUsed							9
#define LOGID_SysWindowNative_WindowMgmtCreateSemaphoreFailed		10
#define LOGID_SysWindowNative_CreateEditControlError				11
#define LOGID_SysWindowNative_CreateEditControlPaintItemError		12
#define LOGID_SysWindowNative_AllocDynamicMemoryError				13
#define LOGID_SysWindowNative_TopLevelWindowPositionIgnored			14
#define LOGID_SysWindowNative_OnlyOneTopLevelWindowPossible			15


/**
 *  <desription>SysGraphicFramebuffer</desription>
 */
#define	LOGID_SysGraphicFramebuffer_SimulationCreateWindowFailed				1
#define	LOGID_SysGraphicFramebuffer_SimulationCreateTaskFailed					2				
#define	LOGID_SysGraphicFramebuffer_SimulationRegisterWindowClassFailed			3
#define	LOGID_SysGraphicFramebuffer_FramebufferNotReady							4
#define	LOGID_SysGraphicFramebuffer_BPPMismatch									5
#define	LOGID_SysGraphicFramebuffer_CreateHWDeviceContextFailed					6
#define	LOGID_SysGraphicFramebuffer_FeatureNotImplemented						7
#define	LOGID_SysGraphicFramebuffer_DebugMessage								8
#define	LOGID_SysGraphicFramebuffer_LoadImageFailed								9
#define	LOGID_SysGraphicFramebuffer_AllocatingFontArrayFailed					10
#define	LOGID_SysGraphicFramebuffer_AllocatingDeviceContextFailed				11
#define	LOGID_SysGraphicFramebuffer_AllocatingPaintItemFailed					12
#define	LOGID_SysGraphicFramebuffer_SimulationAllocatingFramebufferFailed		13
#define	LOGID_SysGraphicFramebuffer_AllocatingDecviceContextVirtualVideoMemoryFailed		14
#define	LOGID_SysGraphicFramebuffer_LoadImageFailed_NoFileImplementation		15
#define	LOGID_SysGraphicFramebuffer_LoadImageFailed_AllocMemoryFailed			16
#define	LOGID_SysGraphicFramebuffer_LoadImageFailed_SysFileFailed				17
#define	LOGID_SysGraphicFramebuffer_AllocationFailed							18
#define	LOGID_SysGraphicFramebuffer_AllocatingImageFilterArrayFailed			19
#define	LOGID_SysGraphicFramebuffer_LoadImageFailed_NotSupportedFile			20
#define	LOGID_SysGraphicFramebuffer_AllocatingPaletteFailed						21
#define	LOGID_SysGraphicFramebuffer_OpenFramebufferFailed						22
#define	LOGID_SysGraphicFramebuffer_NoSemaphores								23
#define	LOGID_SysGraphicFramebuffer_CreateSemaphoreFailed						24
#define	LOGID_SysGraphicFramebuffer_SimulationInputsNotImplemented				25

 /**
 *  <desription>SysFramebuffer</desription>
 */
#define LOGID_SysFramebuffer_SimulationCreateTaskFailed							1
#define LOGID_SysFramebuffer_FramebufferTaskGetMessageException					2

/**
 *  <desription>SysGraphicLight</desription>
 */
#define	LOGID_SysGraphicLight_SimulationCreateWindowFailed						1
#define	LOGID_SysGraphicLight_SimulationCreateTaskFailed						2				
#define	LOGID_SysGraphicLight_SimulationRegisterWindowClassFailed				3


/**
 *  <desription>CmpIecVarAccess</desription>
 */
#define LOGID_CmpIecVarAccess_InvalidOptimizedVariableRequest					1			/** <classid>LOG_ERROR</classid> */
#define LOGID_CmpIecVarAccess_NoMemoryParseOptimizedVariableRequest				2			/** <classid>LOG_ERROR</classid> */
#define LOGID_CmpIecVarAccess_OptimizedVariableRequestNested					3			/** <classid>LOG_ERROR</classid> */
#define LOGID_CmpIecVarAccess_ParentApplicationNotFound							4			/** <classid>LOG_ERROR</classid> */


/**
 *  <desription>CmpEventMgr</desription>
 */
#define LOGID_CmpEventMgr_InvalidEventHandle									1			/** <classid>LOG_ERROR</classid> */
#define LOGID_CmpEventMgr_CallbackIecExeption									2			/** <classid>LOG_ERROR</classid> */


/**
 *  <desription>CmpSIL2</desription>
 */
#define LOGID_CmpSIL2_LoadBootprojectFailed										1			/** <classid>LOG_ERROR</classid>	<severity>Loading Bootproject failed</severity> */
#define LOGID_CmpSIL2_BootprojectExtRefsFailed									2			/** <classid>LOG_ERROR</classid>	<severity>Bootproject contains prohibited references to external functions</severity> */
#define LOGID_CmpSIL2_BootprojectOutOfMemoryFailed								3			/** <classid>LOG_ERROR</classid>	<severity>Not enough memory for application</severity> */


/**
 *  <desription>SysSocket</desription>
 */
#define LOGID_SysSocketEmbedded_CouldNotCreateSocket							1			/** <classid>LOG_WARNING</classid>	<severity>Could not create socket. Out of memory.</severity> */
#define LOGID_SysSocket_DuplicateIP												2			/** <classid>LOG_WARNING</classid>	<severity>Duplicate IP detected. Skipping second one.</severity> */
#define LOGID_SysSocket_AddIpFailed												3			/** <classid>LOG_ERROR</classid>	<severity>IP could not be added to adapter</severity> */
#define LOGID_SysSocket_RemoveIpFailed											4			/** <classid>LOG_ERROR</classid>	<severity>IP could not be removed from adapter</severity> */
#define LOGID_SysSocket_SetDefaultGWFailed										5			/** <classid>LOG_ERROR</classid>	<severity>Default gateway could not be set</severity>*/
#define LOGID_SysSocket_RemoveDefaultGWFailed									6			/** <classid>LOG_ERROR</classid>	<severity>Default gateway could not be removed</severity>*/
#define LOGID_SysSocket_RestoredDefault											7			/** <classid>LOG_INFO</classid>		<severity>Restored the old default gateway</severity>*/

/**
 *  <desription>SIL2PSP_RM4x</desription>
 */
#define LOGID_SIL2PSP_RM4x_BootprojectNotLoaded									1			/** <classid>LOG_INFO</classid>	<severity>Bootproject not loaded</severity> */

 /**
 *  <desription>SysCpuMultiCore</desription>
 */
#define LOGID_SysCpuMultiCore_BindCurrentProcessToCoreFailed					1			/** <classid>LOG_ERROR</classid>	<severity>none/severity> */
#define LOGID_SysCpuMultiCore_ConfigTaskGroupFailed								2			/** <classid>LOG_ERROR</classid>	<severity>none/severity> */
#define LOGID_SysCpuMultiCore_LicenseMissing									3			/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_SysCpuMultiCore_MultiCoreInfo										4			/** <classid>LOG_INFO</classid>		<severity>none</severity> */
#define LOGID_SysCpuMultiCore_MultiCoreConfError								5			/** <classid>LOG_ERROR</classid>	<severity>none</severity> */
#define LOGID_SysCpuMultiCore_MultiCoreConfWarning								6			/** <classid>LOG_WARNING</classid>	<severity>none</severity> */


 /**
 *  <desription>SysTask</desription>
 */
#define LOGID_SysTask_BindTaskToGroupFailed										1			/** <classid>LOG_ERROR</classid>	<severity>none/severity> */


/**
*	<description>CmpCANFoxDrv</description>
*/
#define LOGID_CmpCANFoxDrv_HardwareRegistered									0
#define LOGID_CmpCANFoxDrv_HardwareNotRegistered								1
#define LOGID_CmpCANFoxDrv_WrongDriverVersion									2
#define LOGID_CmpCANFoxDrv_WrongFirmwareVersion									3
#define LOGID_CmpCANFoxDrv_NotReadyForOperation									4
#define LOGID_CmpCANFoxDrv_MaximalNumberOfDevicesExceeded						5
#define LOGID_CmpCANFoxDrv_NoHardwareFound										6

/**
*	<description>CmpIxxatCANDrv</description>
*/
#define LOGID_CmpIxxatCANDrv_HardwareRegistered									0
#define LOGID_CmpIxxatCANDrv_HardwareNotRegistered								1
#define LOGID_CmpIxxatCANDrv_NoHardwareFound									2

/**
*	<description>CmpKvaserCANDrv</description>
*/
#define LOGID_CmpKvaserCANDrv_HardwareRegistered								0
#define LOGID_CmpKvaserCANDrv_HardwareNotRegistered								1
#define LOGID_CmpKvaserCANDrv_NoHardwareFound									2

/**
*	<description>CmpWebSocketClient</description>
*/
#define LOGID_CmpWebSocketClient_HostnameTooLong								0
#define LOGID_CmpWebSocketClient_HostnameUnavailable							1
#define LOGID_CmpWebSocketClient_UnexpectedSchema								2
#define LOGID_CmpWebSocketClient_PortTooLong									3
#define LOGID_CmpWebSocketClient_PortNumberTooHigh								4
#define LOGID_CmpWebSocketClient_HTTPError										5
#define LOGID_CmpWebSocketClient_ConnectViaProxyError							6
#endif	/*_LOGINFOIDS_H_*/
