#ifndef __PLCCOMDIRECT_H__
#define __PLCCOMDIRECT_H__

#include "CmpPLCHandlerDep.h"
#include "PLCConfig.h"
#include "PLCComBase.h"

USEEXTERN_STMT

typedef void* HVARLIST;

/********** Structure definitions *********/
typedef enum tagDirectTypeClass
{
	DTYPE_BOOL,
	DTYPE_INT,
	DTYPE_BYTE,
	DTYPE_WORD,
	DTYPE_DINT,
	DTYPE_DWORD,
	DTYPE_REAL,
	DTYPE_TIME,
	DTYPE_STRING,
	DTYPE_ARRAY,
	DTYPE_ENUM,
	DTYPE_USERDEF,
	DTYPE_BITORBYTE,
	DTYPE_POINTER,
	DTYPE_SINT,
	DTYPE_USINT,
	DTYPE_UINT,
	DTYPE_UDINT,
	DTYPE_DATE,
	DTYPE_TOD,
	DTYPE_DT,
	DTYPE_VOID,
	DTYPE_LREAL,
	DTYPE_REF,
	DTYPE_SUBRANGE, /* this class is only used for load and save
				   	  in all other cases a subrange type behaves like the basetype! */
	DTYPE_LBITORBYTE,
	DTYPE_LINT,
	DTYPE_ULINT,
	DTYPE_LWORD,
	DTYPE_NONE,
	DTYPE_BIT,
	DMAXTYPES
} DirectTypeClass;

typedef struct DirectSymbolDescTag
{
	char *pszName;
	char *pszType;
	DirectTypeClass tcType;
	unsigned short uRefId;
	unsigned long ulOffset;
	unsigned long ulSize;
	char  szAccess[2];
} DirectSymbolDesc;

typedef struct ReadValueTag
{
	unsigned long ulTimeStamp;
	unsigned long ulSize;
	void *pData;
} DirectVarValue;

typedef struct
{
	HVARLIST VarListHandle;
	unsigned long ulNumOfSymbols;
	PlcVarValue **ppPlcVarValue;
}DirectVarList;


#define ERR_OK			    0x0000
#define ERR_FAILED		    0x0001  /* General error - to be used only for internal errors */
#define ERR_NOTIMPLEMENTED  0x000C  /* The function is not implemented */

#ifdef __cplusplus
extern "C" {
#endif

/********** Prototypes of all functions, which are exported by the RTS *********/

/*	Get a list of all available symbols */
/*	Returnvalues: OK: 1, Failed: 0 */
DLL_DECL short SymGetAllSymbols(DirectSymbolDesc **ppSymbolList, unsigned long *pulSymbols);
typedef short (*PFSYMGETALLSYMBOLS)(DirectSymbolDesc **, unsigned long *);

/*	Define a variable list for reading and writing specified by name list */
/*  Returns the handle of the varlist or NULL on failure */
DLL_DECL HVARLIST SymDefineVarList(char **ppszSymbolList, unsigned long ulSymbols);
typedef HVARLIST (*PFSYMDEFINEVARLIST)(char **, unsigned long);

/*	Delete a specified VarList */
/*	Returnvalues: OK: 1, Failed: 0 */
DLL_DECL short SymDeleteVarList(HVARLIST hVarList);
typedef short (*PFSYMDELETEVARLIST)(HVARLIST);

/*	Get the value list of defined VarList */
/*  Returns a pointer to the value list of the VarList or NULL on failure */
DLL_DECL DirectVarValue *SymGetValueList(HVARLIST hVarList);
typedef DirectVarValue* (*PFSYMGETVALUELIST)(HVARLIST);

/*	Read values in ValueList of symbols defined by VarList */
/*	Returnvalues: OK: 1, Failed: 0 */
DLL_DECL short SymReadVarList(HVARLIST hVarList);
typedef short (*PFSYMREADVARLIST)(HVARLIST);

/*	Write values of symbols defined by VarList with specified data in ValueList */
/*	Returnvalues: OK: 1, Failed: 0 */
DLL_DECL short SymWriteVarList(HVARLIST hVarList);
typedef short (*PFSYMWRITEVARLIST)(HVARLIST);

/*	Set all IEC tasks of the application to stop. */
/*  There is no need to evaluate the return value, this will always be 1. */
DLL_DECL int RtsStopAllTasks(void);
typedef int (*PFRTSSTOPALLTASKS)(void);

/*	Set all IEC tasks of the application to running. */
DLL_DECL void RtsStartAllTasks(void);
typedef void (*PFRTSSTARTALLTASKS)(void);

/*	Reset the program dependant on the following modes: */
/*  Reset modes: 0 = WARM, 1 = COLD, 2 = HARD (Origin) */
DLL_DECL void RtsProgramReset(unsigned char byResetMode);
typedef void (*PFRTSPROGRAMRESET)(unsigned char);

/*	Get the status of the PLC, calls internally CtrlControllerStatus() */
/*  Returns the status of the PLC: 0 = RUN, ... */
DLL_DECL unsigned char RtsGetPLCState(void);
typedef unsigned char (*PFRTSGETPLCSTATE)(void);

/*	Get the status of the PLC */
/*  Returns the status of the PLC: 0 = RUN, ... */
/*  Should only be used from the PLCHandler */
DLL_DECL unsigned char CtrlControllerStatus(void);
typedef unsigned char (*PFCTRLCONTROLLERSTATUS)(void);

/*	Get the ID of the currently loaded program. */
/*  Returns the ProjectID */
DLL_DECL long CtrlGetIdentity(void);
typedef long (*PFCTRLGETIDENTITY)(void);

/*	Get the ProjectId if the currently loaded symbol database */
/*  If the target setting "" is used, then this ID can be different from */
/*  CtrlGetIdentity(), after an OnlineChange that only change the */
/*  ProjectId, but not the content of the symbolfile */
DLL_DECL unsigned long SymGetIdentity(void);
typedef unsigned long (*PFSYMGETIDENTITY)(void);

/* Enter synchronization of the symbol interface */
DLL_DECL short SymEnterAccess(void);
typedef short (*PFSYMENTERACCESS)(void);

/* Enter synchronization of the symbol interface */
DLL_DECL short SymLeaveAccess(void);
typedef short (*PFSYMLEAVEACCESS)(void);

#ifdef __cplusplus
}
#endif


class CPLCComDirect : public CPLCComBase
{
	public:
		// Construction with specific PLC and Device configuration 
		CPLCComDirect(PlcConfig *pConfig, PlcDeviceDesc *pDevice, RTS_HANDLE hLogger = RTS_INVALID_HANDLE);

		virtual ~CPLCComDirect(void);

		virtual long Open(void);
		virtual long Close(void);

		virtual long Login(void);
		virtual long Logout(void);

		virtual long CheckProjectIds(void);
		virtual long GetPlcProjectName(char *pszProject, unsigned long ulLen);
		virtual long GetPlcStatus(PLC_STATUS *pPlcStatus = NULL);
		
		virtual long LoadSymbols(void);
		virtual long DeleteSymbols(void);
		virtual long GetSymbols(PlcSymbolDesc **ppSymbolList, unsigned long *pulNumOfSymbols);

		virtual HVARLIST DefineVarList(char **ppszSymbols, unsigned long ulNumOfSymbols, int bDataChange, unsigned long ulFlags, long *plResult);
		virtual long DeleteVarList(HVARLIST hVarList, int bDeleteLocalListOnly);

		virtual long SendVarList(HVARLIST hVarList, RTS_HANDLE hEvent, RTS_HANDLE hSem, int *pbSync = NULL, int bGetChangedValues = 0, unsigned long *pulVarFlags = NULL);
		virtual long GetVarList(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);
		virtual long GetValues(HVARLIST hVarList, PlcVarValue ***pppValues, unsigned long *pulNumOfValues);

		virtual long WriteVarList(HVARLIST hVarList, unsigned long ulNumOfSymbols, unsigned char **ppValues, unsigned long *pulValueSizes);
		virtual long WriteVars(char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned char** ppValues, unsigned long *pulValueSizes);

		virtual long SendService(unsigned char *pbySend, unsigned long ulSendSize, unsigned char **ppbyRecv, unsigned long *pulRecvSize);

		virtual long GetDevInfo(DeviceInfo3 **ppDeviceInfo);
		virtual long GetPrjInfo(ProjectInfo **ppPrjInfo);

	private:
		void Init(void);
		long LoadGatewayFcts(void);

		RTS_HANDLE					m_hPlcLib;
		PFSYMGETALLSYMBOLS			m_pfSymGetAllSymbols;
		PFSYMDEFINEVARLIST			m_pfSymDefineVarList;
		PFSYMDELETEVARLIST			m_pfSymDeleteVarList;
		PFSYMGETVALUELIST			m_pfSymGetValueList;
		PFSYMREADVARLIST			m_pfSymReadVarList;
		PFSYMWRITEVARLIST			m_pfSymWriteVarList;
		PFCTRLCONTROLLERSTATUS		m_pfCtrlControllerStatus;
		PFCTRLGETIDENTITY			m_pfCtrlGetIdentity;
		PFSYMGETIDENTITY			m_pfSymGetIdentity;
		PFSYMENTERACCESS			m_pfSymEnterAccess;
		PFSYMLEAVEACCESS			m_pfSymLeaveAccess;
};


#endif	//__PLCCOMDIRECT_H__
