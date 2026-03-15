/*****************************************************************************
*
*	Copyright:		© 3S - Smart Software Solutions GmbH, Kempten
*	Module: 		PLCHandlerPrivate
*	Purpose:		Contains all private definitions for plchandler class.
*
******************************************************************************/

#ifndef __PLCHANDLERPRIVATE_H__
#define __PLCHANDLERPRIVATE_H__

#include "CmpPLCHandlerDep.h"

class CPLCHandlerCallback;
class CPLCHandler;

/* States for Async services */
#define ASYNC_SERV_FREE			0
#define ASYNC_SERV_TO_SEND		1
#define ASYNC_SERV_WORK			2
#define ASYNC_SERV_READY		3

#define MAX_ASYNC_COMMANDS		32

typedef struct AsyncServiceTag
{
	int iInvokeId;
	int	iState;
	CPLCHandlerCallback *pCallback;
	long lResult;
	unsigned long ulSendSize;
	unsigned long ulRecvSize;	
	unsigned char *pbySend;
	unsigned char *pbyRecv;
} AsyncServiceInfotyp;

// Cyclic list flags
#define FCVL_VALID			0x00000001
#define FCVL_ENABLED		0x00000002

#define IS_CYCLIST_VALID(pCycList)		(((CycVarList*)pCycList)->ulFlags & FCVL_VALID)
#define CYCLIST_SET_VALID(pCycList)		(((CycVarList*)pCycList)->ulFlags |= FCVL_VALID)
#define CYCLIST_SET_INVALID(pCycList)	(((CycVarList*)pCycList)->ulFlags &= ~FCVL_VALID)

#define IS_CYCLIST_ENABLED(pCycList)	(((CycVarList*)pCycList)->ulFlags & FCVL_ENABLED)
#define CYCLIST_SET_ENABLE(pCycList)	(((CycVarList*)pCycList)->ulFlags |= FCVL_ENABLED)
#define CYCLIST_SET_DISABLE(pCycList)	(((CycVarList*)pCycList)->ulFlags &= ~FCVL_ENABLED)


class CycVarList
{
	public:
		CycVarList(CPLCHandler *pPlcHandler);
		~CycVarList(void);

		int AddRef(void);
		int Release(void);
		int EnterVarAccess(void);
		int LeaveVarAccess(void);

		int iRefCount;
		unsigned long ulFlags;
		void *m_pPlcHandler;
		unsigned long ulUpdateRate;
		RTS_HANDLE hUpdateThread;
		char bUpdateActive;
		HVARLIST hVarList;
		PlcVarValue **ppValues;
		unsigned long ulNumOfValues;
		char **ppszSymbolList;
		RTS_HANDLE hsemVarAccess;
		unsigned long ulOperatingRate;
		RTS_HANDLE hRecvEvent;
		CPLCHandlerCallback *pUpdateReadyCallback;
		CPLCHandlerCallback *pDataChangeCallback;
		RTS_HANDLE hUpdateEvent;
		PlcVarValue **ppOldValues;
		PlcVarValue **ppChangedValues;
		unsigned long ulNumOfChangedValues;
		unsigned long *pulChangedValuesIndex;
		unsigned long *pulValueLength;
		unsigned long *pulVarFlags;
};

class SyncVarList
{
public:
	SyncVarList(CPLCHandler *pPlcHandler, char **ppszSymbols, unsigned long ulNumOfSymbols, unsigned long ulFlags);
	~SyncVarList(void);

	void *m_pPlcHandler;
	unsigned long m_ulNumOfSymbols;
	unsigned long m_ulFlags;
	char **m_ppszSymbolList;
	CStringAllocator* m_pTypeStringAllocater;
	HVARLIST m_hVarList;
	int m_bValid;
};

typedef struct UpdatethreadWaitExitTag
{
	RTS_HANDLE hThread;					// Thread wait for exit
	unsigned long ulSetExitTimestamp;	// Timestamp of SysTaskSetExit() call
	unsigned long ulExitTimeout;		// Timeout to wait before the task is killed
} UpdatethreadWaitExittyp;

#endif	/* __PLCHANDLERPRIVATE_H__ */
