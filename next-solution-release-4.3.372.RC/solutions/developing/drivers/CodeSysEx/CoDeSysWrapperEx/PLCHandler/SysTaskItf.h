 /**
 * <interfacename>SysTask</interfacename>
 * <description> 
 *  <p>The SysTask interface is projected to handle all system dependent task operations.
 *  This interface can only be implemented typically on targets with an operating system.</p>
 *
 *  <p>All task that are created by this interface must have the same prototype.
 *  Here are examples for implementing a cyclic task and example how to create and delete a task.</p>
 *  
 *  <pre>
 *       1.  TaskFrame
 *      
 *           - Cyclic task that sets itself into sleep for cyclic execution:
 *      
 *                   int iTaskParameter = 123;
 *                   RTS_RESULT Result;
 *                   RTS_HANDLE hTask = CAL_SysTaskCreate("TestTask", TestTask, &amp;iTaskParameter, TASKPRIO_HIGH_BASE, 0, 0, NULL, &amp;Result);
 *                   CAL_SysTaskResume(hTask);
 *      
 *	        	        void TestTask(SYS_TASK_PARAM *ptp)
 *	        	        {
 *	        	        	RTS_HANDLE hTask = ptp-&gt;hTask;
 *	        	        	void *pParameter = (void *)ptp-&gt;pParam;
 *           
 *	        	        	CAL_SysTaskEnter(hTask);
 *	        	        	while (!ptp-&gt;bExit)
 *	        	        	{
 *	        	        		// TODO: Task code
 *	        	        		CAL_SysTaskWaitSleep(hTask, 100);
 *	        	        	}
 *	        	        	CAL_SysTaskLeave(hTask);
 *	        	        	CAL_SysTaskEnd(hTask, 0);
 *	        	        }
 *      
 *           - Cyclic task that uses the cyclic mechanism of the operating system for cyclic execution:
 *      
 *                   int iTaskParameter = 123;
 *                   RTS_RESULT Result;
 *                   RTS_HANDLE hTask = CAL_SysTaskCreate("TestTask2", TestTask2, &amp;iTaskParameter, TASKPRIO_HIGH_BASE, 1000, 0, NULL, &amp;Result);
 *                   CAL_SysTaskResume(hTask);
 *           
 *                   void TestTask2(SYS_TASK_PARAM *ptp)
 *                   {
 *                       RTS_HANDLE hTask = ptp-&gt;hTask;
 *                       void *pParameter = (void *)ptp-&gt;pParam;
 *           
 *                       CAL_SysTaskEnter(hTask);
 *                       while (!ptp-&gt;bExit)
 *                       {
 *                           // TODO: Task code
 *                           CAL_SysTaskWaitInterval(hTask);
 *                       }
 *                       CAL_SysTaskLeave(hTask);
 *                       CAL_SysTaskEnd(hTask, 0);
 *                   }
 *      
 *       2.  Create and delete a task
 *      
 *           To create and delete a task you should use the following sequences 
 *      
 *           - Creating a task:
 *             There are 2 different interface functions to create a task:
 *       
 *             a) This function creates a typical task. See interface descriptions for details:
 *      
 *                     RTS_HANDLE CDECL SysTaskCreate(char* pszTaskName, PFSYS_TASK_FUNCTION pFunction, void *pParam, RTS_UI32 ulPriority, RTS_UI32 ulInterval, RTS_UI32 ulStackSize, PFTASKEXCEPTIONHANDLER pExceptionHandler, RTS_RESULT *pResult);
 *                
 *             b) This function creates a task, which is automatically added to a so called task group. See interface descriptions for details:
 *      
 *                     RTS_HANDLE CDECL SysTaskCreate2(char* pszTaskName, char *pszTaskGroup, PFSYS_TASK_FUNCTION pFunction, void *pParam, RTS_UI32 ulPriority, RTS_UI32 ulInterval, RTS_UI32 ulStackSize, PFTASKEXCEPTIONHANDLER pExceptionHandler, RTS_RESULT *pResult);
 *       
 *           - Deleting a task:
 *      
 *             There are 2 ways to delete a task:
 *       
 *             a) Implicitly by the task itself:
 *      
 *                Here the task will release itself after calling SysTaskSetExit(). The task must be specified with the flag SYSTASK_FF_AUTORELEASEONEXIT by calling SysTaskAutoReleaseOnExit():
 *      
 *                Example:
 *                       
 *                   o Create the task:
 *      
 *                     RTS_HANDLE hTask = CAL_SysTaskCreate("MyTask", MyTaskFrame, NULL, TASKPRIO_NORMAL_BASE, 0, 0, NULL, NULL);
 *                     CAL_SysTaskAutoReleaseOnExit(hTask);
 *       
 *                   o Release the task:
 *      
 *                     CAL_SysTaskSetExit(hTask);
 *       
 *             b) Explicitly by the creator of a task:
 *      
 *                Example:
 *      
 *                     CAL_SysTaskSetExit(hTask);
 *                     CAL_SysTaskExit(hTask, 10000);
 *      
 *                The timeout should be the double of the cycle time of the task, if it is a cyclic task.
 *  </pre> 
 *
 * </description>
 *  
 * <copyright>
 * Copyright CODESYS Development GmbH
 * </copyright>
 */


	
	
#ifndef _SYSTASKITF_H_
#define _SYSTASKITF_H_

#include "CmpStd.h"

 

 




#ifndef CDECL
    #define CDECL
#endif

#include "SysExceptItf.h"
#include "SysTimeItf.h"
#include "CMUtilsHashItf.h"

/**
 * <category>Static defines</category>
 * <description>Maximum length of a task name</description>
 */
#ifndef SYSTASK_MAX_NAME_LEN
    #define SYSTASK_MAX_NAME_LEN    20
#endif

/**
 * <category>Static defines</category>
 * <description>Number of static tasks</description>
 */
#ifndef SYSTASK_NUM_OF_STATIC_TASKS
    #define SYSTASK_NUM_OF_STATIC_TASKS 20
#endif

/**
 * <category>Static defines</category>
 * <description>Size of the hash table used to access thread local storage</description>
 */
#ifndef SYSTASK_THREAD_LOCAL_STORAGE_HASH_TABLE_SIZE
    #define SYSTASK_THREAD_LOCAL_STORAGE_HASH_TABLE_SIZE 17
#endif

/**
 * <category>Static defines</category>
 * <description>Use atomic bit access for status changes on multi-core targets! Only in this case the bit field remains consistent.</description>
 */
#if !defined(SYSCPUMULTICORE_NOTIMPLEMENTED)
    #define SYSTASK_USE_ATOMIC_BITACCESS
#endif

/**
 * <category>Task status definitions</category>
 * <description>Actual status of a task</description>
 */
#define TS_NOT_CREATED      0x0000
#define TS_VALID            0x0001
#define TS_ACTIVE           0x0002
#define TS_SUSPENDED        0x0004
#define TS_DELETED          0x0008
#define TS_EXCEPTION        0x0010
#define TS_CONTEXT          0x0020

/**
 * <category>Task status bits</category>
 * <description></description>
 */
#define TF_VALID            0
#define TF_ACTIVE           1
#define TF_SUSPENDED_       2
#define TF_DELETED          3
#define TF_EXCEPTION        4
#define TF_CONTEXT          5
#if !defined(TF_SUSPENDED)
    #define TF_SUSPENDED    TF_SUSPENDED_
#endif

/**
 * <category>Task status macros</category>
 */
#define SysTaskIsValid(pTask)               (pTask->iState & TS_VALID)
#define SysTaskIsActive(pTask)              (pTask->iState & TS_ACTIVE)
#define SysTaskIsSuspended(pTask)           (pTask->iState & TS_SUSPENDED)
#define SysTaskIsDeleted(pTask)             (pTask->iState & TS_DELETED)
#define SysTaskIsException(pTask)           (pTask->iState & TS_EXCEPTION)
#define SysTaskHasContext(pTask)            (pTask->iState & TS_CONTEXT)

#if defined(SYSCPUHANDLING_NOTIMPLEMENTED) || defined(SYSCPUTESTANDSETBIT_NOTIMPLEMENTED) || !defined(SYSTASK_USE_ATOMIC_BITACCESS)
    #define SysTaskSetValid(pTask)          (pTask->iState |= TS_VALID)
    #define SysTaskSetInvalid(pTask)        (pTask->iState &= ~TS_VALID)

    #define SysTaskSetActive(pTask)         (pTask->iState |= TS_ACTIVE)
    #define SysTaskSetInactive(pTask)       (pTask->iState &= ~TS_ACTIVE)

    #define SysTaskSetSuspended(pTask)      (pTask->iState |= TS_SUSPENDED)
    #define SysTaskSetResumed(pTask)        (pTask->iState &= ~TS_SUSPENDED)

    #define SysTaskSetDeleted(pTask)        (pTask->iState |= TS_DELETED)

    #define SysTaskSetException(pTask)      (pTask->iState |= TS_EXCEPTION)
    #define SysTaskResetException(pTask)    (pTask->iState &= ~TS_EXCEPTION)

    #define SysTaskSetContext(pTask)        (pTask->iState |= TS_CONTEXT)
    #define SysTaskSetNoContext(pTask)      (pTask->iState &= ~TS_CONTEXT)
#else
    #define SysTaskSetValid(pTask)          {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_VALID, 1);\
                                                else\
                                                pTask->iState |= TS_VALID;\
                                            }
    #define SysTaskSetInvalid(pTask)        {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_VALID, 0);\
                                                else\
                                                    pTask->iState &= ~TS_VALID;\
                                            }

    #define SysTaskSetActive(pTask)         {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_ACTIVE, 1);\
                                                else\
                                                    pTask->iState |= TS_ACTIVE;\
                                            }
    #define SysTaskSetInactive(pTask)       {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_ACTIVE, 0);\
                                                else\
                                                    pTask->iState &= ~TS_ACTIVE;\
                                            }

    #define SysTaskSetSuspended(pTask)      {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_SUSPENDED_, 1);\
                                                else\
                                                    pTask->iState |= TS_SUSPENDED;\
                                            }
    #define SysTaskSetResumed(pTask)        {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_SUSPENDED_, 0);\
                                                else\
                                                    pTask->iState &= ~TS_SUSPENDED;\
                                            }

    #define SysTaskSetDeleted(pTask)        {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_DELETED, 1);\
                                                else\
                                                    pTask->iState |= TS_DELETED;\
                                            }

    #define SysTaskSetException(pTask)      {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_EXCEPTION, 1);\
                                                else\
                                                    pTask->iState |= TS_EXCEPTION;\
                                            }
    #define SysTaskResetException(pTask)    {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_EXCEPTION, 0);\
                                                else\
                                                    pTask->iState &= ~TS_EXCEPTION;\
                                            }
    #define SysTaskSetContext(pTask)      {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_CONTEXT, 1);\
                                                else\
                                                    pTask->iState |= TS_CONTEXT;\
                                            }
    #define SysTaskSetNoContext(pTask)        {\
                                                if (CHK_SysCpuTestAndSetBit)\
                                                    CAL_SysCpuTestAndSetBit(&pTask->iState, sizeof(pTask->iState), TF_CONTEXT, 0);\
                                                else\
                                                    pTask->iState &= ~TS_CONTEXT;\
                                            }

#endif


/**
 * <category>Priority ranges</category>
 * <description>
 *  Task priorities can be set between 0..255.
 *  Each task priority is assigned to one of the following range:
 *  <ul>
 *      <li>TASKPRIO_SYSTEM: Runtime system management tasks, like scheduler</li>
 *      <li>TASKPRIO_REALTIME: Real time tasks. Reserved for IEC-tasks</li>
 *      <li>TASKPRIO_HIGH: For tasks right below real time tasks. Could be used e.g. for higher priority communication tasks</li>
 *      <li>TASKPRIO_ABOVENORMAL: For tasks below high tasks</li>
 *      <li>TASKPRIO_NORMAL: Normal tasks, like communication server tasks</li>
 *      <li>TASKPRIO_BELOWNORMAL: For tasks below normal tasks</li>
 *      <li>TASKPRIO_LOW: For tasks with low priority</li>
 *      <li>TASKPRIO_LOWEST: For tasks with lowest priority</li>
 *      <li>TASKPRIO_IDLE: For background or idle operations</li>
 *  </ul>
 * </description>
 */
#define TASKPRIO_RANGE              255
#define TASKPRIO_SEGMENT            32

#define TASKPRIO_MAX                0

/* Task segment 1 */
#define TASKPRIO_SYSTEM_BASE        0
#define TASKPRIO_SYSTEM_END         31

/* Task segment 2 */
#define TASKPRIO_REALTIME_BASE      32
#define TASKPRIO_REALTIME_END       63

/* Task segment 3 */
#define TASKPRIO_HIGH_BASE          64
#define TASKPRIO_HIGH_END           95

/* Task segment 4 */
#define TASKPRIO_ABOVENORMAL_BASE   96
#define TASKPRIO_ABOVENORMAL_END    127

/* Task segment 5 */
#define TASKPRIO_NORMAL_BASE        128
#define TASKPRIO_NORMAL_END         159

/* Task segment 6 */
#define TASKPRIO_BELOWNORMAL_BASE   160
#define TASKPRIO_BELOWNORMAL_END    191

/* Task segment 7 */
#define TASKPRIO_LOW_BASE           192
#define TASKPRIO_LOW_END            223

/* Task segment 8 */
#define TASKPRIO_LOWEST_BASE        224
#define TASKPRIO_LOWEST_END         255

#define TASKPRIO_IDLE               255
#define TASKPRIO_MIN                255


/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  Mapping task priorities:
 *  Settings to shift task priority ranges to the appropriate area. This is done via calling the function SysTaskGetConfiguredPriority(). 
 *  The virtual priorities can be mapped to real operating system priorities with the following settings.
 *
 *  The mapping is done this way (pseudo code):
 *
 *  <pre>
 *      if (taskPriority >= OSPriority.XXX.Base &amp;&&amp; TaskPriority <= OSPriority.XXX.Base)
 *      {
 *          if (taskPriority > (OSPriority.XXX.End - OSPriority.XXX.Base))
 *                  return OSPriority.XXX.End;
 *              return OSPriority.XXX.Base + taskPriority;
 *          return taskPriority;
 *      }
 *  </pre>
 * </description>
 */
#define SYSTASKKEY_INT_PRIORITY_SYSTEM_BASE             "OSPriority.System.Base"
#define SYSTASKVALUE_INT_PRIORITY_SYSTEM_BASE_DEFAULT   TASKPRIO_SYSTEM_BASE
#define SYSTASKKEY_INT_PRIORITY_SYSTEM_END              "OSPriority.System.End"
#define SYSTASKVALUE_INT_PRIORITY_SYSTEM_END_DEFAULT    TASKPRIO_SYSTEM_END

#define SYSTASKKEY_INT_PRIORITY_REALTIME_BASE           "OSPriority.Realtime.Base"
#define SYSTASKVALUE_INT_PRIORITY_REALTIME_BASE_DEFAULT TASKPRIO_REALTIME_BASE
#define SYSTASKKEY_INT_PRIORITY_REALTIME_END            "OSPriority.Realtime.End"
#define SYSTASKVALUE_INT_PRIORITY_REALTIME_END_DEFAULT  TASKPRIO_REALTIME_END

#define SYSTASKKEY_INT_PRIORITY_HIGH_BASE               "OSPriority.High.Base"
#define SYSTASKVALUE_INT_PRIORITY_HIGH_BASE_DEFAULT     TASKPRIO_HIGH_BASE
#define SYSTASKKEY_INT_PRIORITY_HIGH_END                "OSPriority.High.End"
#define SYSTASKVALUE_INT_PRIORITY_HIGH_END_DEFAULT      TASKPRIO_HIGH_END

#define SYSTASKKEY_INT_PRIORITY_ABOVENORMAL_BASE            "OSPriority.AboveNormal.Base"
#define SYSTASKVALUE_INT_PRIORITY_ABOVENORMAL_BASE_DEFAULT  TASKPRIO_ABOVENORMAL_BASE
#define SYSTASKKEY_INT_PRIORITY_ABOVENORMAL_END             "OSPriority.AboveNormal.End"
#define SYSTASKVALUE_INT_PRIORITY_ABOVENORMAL_END_DEFAULT   TASKPRIO_ABOVENORMAL_END

#define SYSTASKKEY_INT_PRIORITY_NORMAL_BASE             "OSPriority.Normal.Base"
#define SYSTASKVALUE_INT_PRIORITY_NORMAL_BASE_DEFAULT   TASKPRIO_NORMAL_BASE
#define SYSTASKKEY_INT_PRIORITY_NORMAL_END              "OSPriority.Normal.End"
#define SYSTASKVALUE_INT_PRIORITY_NORMAL_END_DEFAULT    TASKPRIO_NORMAL_END

#define SYSTASKKEY_INT_PRIORITY_BELOWNORMAL_BASE            "OSPriority.BelowNormal.Base"
#define SYSTASKVALUE_INT_PRIORITY_BELOWNORMAL_BASE_DEFAULT  TASKPRIO_BELOWNORMAL_BASE
#define SYSTASKKEY_INT_PRIORITY_BELOWNORMAL_END             "OSPriority.BelowNormal.End"
#define SYSTASKVALUE_INT_PRIORITY_BELOWNORMAL_END_DEFAULT   TASKPRIO_BELOWNORMAL_END

#define SYSTASKKEY_INT_PRIORITY_LOW_BASE                "OSPriority.Low.Base"
#define SYSTASKVALUE_INT_PRIORITY_LOW_BASE_DEFAULT      TASKPRIO_LOW_BASE
#define SYSTASKKEY_INT_PRIORITY_LOW_END                 "OSPriority.Low.End"
#define SYSTASKVALUE_INT_PRIORITY_LOW_END_DEFAULT       TASKPRIO_LOW_END

#define SYSTASKKEY_INT_PRIORITY_LOWEST_BASE             "OSPriority.Lowest.Base"
#define SYSTASKVALUE_INT_PRIORITY_LOWEST_BASE_DEFAULT   TASKPRIO_LOWEST_BASE
#define SYSTASKKEY_INT_PRIORITY_LOWEST_END              "OSPriority.Lowest.End"
#define SYSTASKVALUE_INT_PRIORITY_LOWEST_END_DEFAULT    TASKPRIO_LOWEST_END

#define SYSTASKKEY_INT_VXWORKS_CPU                  "VxWorks.CPU"
#define SYSTASKVALUE_INT_VXWORKS_CPU_DEFAULT        0

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 * ATTENTION!!! Only intended for very special debug purposes!
 * Use this setting only, if this was requested by the 3S team!
 * Use this setting only in non-productive test environments!
 * When this is set, all task exceptions are untreated and the first exception will directly crash the runtime system!!!* 
 * </description>
 */
#define SYSTASKKEY_INT_WIN32_DEBUG_DISABLE_EXCEPTIONHANDLER             "Win32.DebuggingOnlyDisableExceptionHandler"
#ifndef SYSTASKKEY_INT_WIN32_DEBUG_DISABLE_EXCEPTIONHANDLER_DEFAULT
    #define SYSTASKKEY_INT_WIN32_DEBUG_DISABLE_EXCEPTIONHANDLER_DEFAULT     0
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  Setting to specify the OS specify task stack-size. For smaller embedded systems with tasks, this setting is necessary.
 *  NOTE:
 *  Actually this setting is only supported by Linux/QNX platforms!
 * </description>
 */
#define SYSTASKKEY_INT_TASK_STACKSIZE                                   "StackSize"
#ifndef SYSTASKVALUE_INT_TASK_STACKSIZE_DEFAULT
    #define SYSTASKVALUE_INT_TASK_STACKSIZE_DEFAULT                     0
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  By default CODESYS doesn't change the clock period of the QNX system tick. To reduce the jitter of
 *  the tasks, or to support "distributed clocks", one might want to have a higher resolution of the
 *  timer. With this setting you can adjust the clock period at CODESYS startup.
 *  The period is defined in nanoseconds and is passed as is to the QNX neutrino function ClockPeriod().
 * </description>
 */
#define SYSTASKKEY_INT_QNX_CLOCKPERIOD                          "QNX.ClockPeriod"
#ifndef SYSTASKVALUE_INT_QNX_CLOCKPERIOD_DEFAULT
#define SYSTASKVALUE_INT_QNX_CLOCKPERIOD_DEFAULT            0
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  By default CODESYS aligns tasks to the QNX system tick to reduce task jitter and avoid lost IEC task cycles.
 *  These would occur as the QNX system tick may not be set to exact values, depending on the underlying hardware.
 *  When using "distributed clocks", this behavior is undesired as it leads to high deviations in the resulting
 *  DC sync window. With this setting you can modify this behavior so that the scheduler will not make any
 *  adjustments to a task's wakeup time and leave the exact tick up to the system. Please note that you should
 *  additionally decrease the QNX system tick for "distributed clocks".
 *  0: Default behavior, tasks are aligned to QNX system tick; recommended to minimize task jitter with low system tick rates
 *  1: Modified behavior, tasks are not aligned to QNX system tick; recommended when using "distributed clocks"
 * </description>
 */
#define SYSTASKKEY_INT_QNX_DISABLETICKBASEDSCHEDULER            "QNX.DisableTickBasedScheduler"
#ifndef SYSTASKVALUE_INT_QNX_DISABLETICKBASEDSCHEDULER
#define SYSTASKVALUE_INT_QNX_DISABLETICKBASEDSCHEDULER_DEFAULT  0
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  1:  If the computed starting time of the next task cycle is in the past this cycle will be skipped. The task will wait until the next starting time in the future.
 *  0(DEFAULT): If the computed starting time of the next task cycle is in the past the task will be scheduled immediately. 
 *  NOTE:
 *  Actually this setting is only supported by Linux/QNX platforms!
 * </description>
 */
#define SYSTASKKEY_INT_LINUX_SKIPLOSTCYCLES                     "Linux.SkipLostCycles"
#ifndef SYSTASKVALUE_INT_LINUX_SKIPLOSTCYCLES_DEFAULT
    #define SYSTASKVALUE_INT_LINUX_SKIPLOSTCYCLES_DEFAULT       0
#endif

/**
 * <category>Settings</category>
 * <type>Int</type>
 * <description>
 *  The maximum OS Task Priority that can be assigned to a runtime thread (The thread with priority TASKPRIO_SYSTEM_BASE).
 *  Valid values are 49 - operating system dependent maximum priority for scheduling policy SCHED_FIFO. 
 *  NOTE:
 *  Actually this setting is only supported by Linux/QNX platforms!
 * </description>
 */
#define SYSTASKKEY_INT_LINUX_MAXOSPRIORITY                     "Linux.MaxOSPriority"
#ifndef SYSTASKVALUE_INT_LINUX_MAXOSPRIORITY_DEFAULT
 #ifdef QNX
    #define SYSTASKVALUE_INT_LINUX_MAXOSPRIORITY_DEFAULT       58
 #else
    #define SYSTASKVALUE_INT_LINUX_MAXOSPRIORITY_DEFAULT       88
 #endif
#endif

RTS_RESULT CDECL TaskExceptionHandler(RTS_HANDLE hTask, RTS_UI32 ulException, RegContext Context);
typedef RTS_RESULT (CDECL *PFSYS_TASK_EXCEPTIONHANDLER)(RTS_HANDLE hTask, RTS_UI32 ulException, RegContext Context);
#define PFTASKEXCEPTIONHANDLER      PFSYS_TASK_EXCEPTIONHANDLER


/**
 * <category>Event parameter</category>
 * <element name="pSysTaskInfo" type="IN">Pointer to task info structure</element>
im */
typedef struct
{
    struct tagSYS_TASK_INFO *pSysTaskInfo;
} EVTPARAM_SysTask;
#define EVTPARAMID_SysTask                  0x0001
#define EVTVERSION_SysTask                  0x0001

/**
 * <category>Event parameter</category>
 * <element name="pSysTaskInfo" type="IN">Pointer to task info structure</element>
 * <element name="ui32PriorityOriginal" type="IN">Original task priority (virtual RTS task priority)</element>
 * <element name="ui32PriorityMapped" type="OUT">Mapped task priority (virtual RTS task priority)</element>
 * <element name="Result" type="OUT">Error code:
 *                   ERR_OK: Priority is mapped
 *                   Other:  Original mapping is used
 * </element>
 */
typedef struct
{
    struct tagSYS_TASK_INFO *pSysTaskInfo;
    RTS_UI32 ui32PriorityOriginal;
    RTS_UI32 ui32PriorityMapped;
    RTS_RESULT Result;
} EVTPARAM_SysTask_GetPriority;
#define EVTPARAMID_SysTask_GetPriority          0x0002
#define EVTVERSION_SysTask_GetPriority          0x0001

/**
 * <category>Events</category>
 * <description>Event is sent, if a new task was create</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysTask</param>
 */
#define EVT_TaskCreate                  MAKE_EVENTID(EVTCLASS_INFO, 1)

/**
 * <category>Events</category>
 * <description>Event is sent, before a task will be deleted</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysTask</param>
 */
#define EVT_TaskDelete                  MAKE_EVENTID(EVTCLASS_INFO, 2)

/**
 * <category>Events</category>
 * <description>Event is sent, whenever the task interval is set (used for ECAT DC)</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysTask</param>
 */
#define EVT_TaskSetInterval             MAKE_EVENTID(EVTCLASS_INFO, 3)
#define EVTPARAMID_SysTaskSetInterval       0x0002
#define EVTVERSION_SysTaskSetInterval       0x0001

/**
 * <category>Events</category>
 * <description>Event is sent for each task to map virtual RTS task priority</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysTask_GetPriority</param>
 */
#define EVT_TaskGetPriority             MAKE_EVENTID(EVTCLASS_INFO, 4)


/**
 * <category>Events</category>
 * <description>Event is sent when a task leaves. This event is called from the tasks context.</description>
 * <param name="pEventParam" type="IN">EVTPARAM_SysTask</param>
 */
#define EVT_TaskLeave                   MAKE_EVENTID(EVTCLASS_INFO, 5)


/**
 * <category>Event parameter</category>
 * <element name="pSysTaskSetInterval" type="IN">Pointer to set task interval information</element>
 */
typedef struct
{
    struct tagsystasksetinterval_struct* pSysTaskSetInterval;
} EVTPARAM_SysTaskSetInterval;

/**
 * <category>Feature flags</category>
 * <element name="pSysTaskInfo" type="IN">Pointer to task info structure</element>
 */
#define SYSTASK_FF_NONE                     0x00000000
#define SYSTASK_FF_AUTORELEASEONEXIT        0x00000001
#define SYSTASK_FF_NONSYSTASK               0x00000002

struct tagSYS_TASK_PARAM;
typedef void CDECL SYS_TASK_FUNCTION(struct tagSYS_TASK_PARAM *);
typedef void (CDECL *PFSYS_TASK_FUNCTION)(struct tagSYS_TASK_PARAM *);

/**
 * <description>SYS_TASK_PARAM</description>
 */
typedef struct tagSYS_TASK_PARAM
{
    RTS_IEC_HANDLE hTask;       /* <element name="hTask" type="IN">SysTask handle of the task</element> */
    RTS_IEC_DINT bExit;     /* <element name="bExit" type="IN">bExit flag to exit the task loop</element> */
    RTS_IEC_UDINT ulInterval;       /* <element name="ulInterval" type="IN">Specified interval of the task</element> */
    void *pParam;       /* <element name="pParam" type="IN">User specific parameter for the task</element> */
} SYS_TASK_PARAM;

/**
 * <description>SYS_TASK_INFO</description>
 */
typedef struct tagSYS_TASK_INFO
{
    RTS_IEC_HANDLE uiOSHandle;      /* <element name="uiOSHandle" type="IN">Operating system handle of a task</element> */
    RTS_IEC_DINT iState;            /* <element name="iState" type="IN">Actual state of the task. See category "Task status definitions" for details.</element> */
    RTS_IEC_DINT iOldState;         /* <element name="iOldState" type="IN">Previous state of the task</element> */
    SYS_TASK_PARAM TP;              /* <element name="TP" type="IN">Task parameter, with which the task was created</element> */
    RTS_IEC_UDINT ulCycleTime;      /* <element name="ulCycleTime" type="IN">Last cycle time</element> */
    RTS_IEC_UDINT ulCycleStart;     /* <element name="ulCycleStart" type="IN">Start time of the cycle</element> */
    RTS_IEC_UDINT ulPriority;       /* <element name="ulPriority" type="IN">RTS priority of the task (not OS priority)</element> */
    RTS_IEC_UDINT ulOSPriority;     /* <element name="ulOSPriority" type="IN">Operating system priority of the task</element> */
    RTS_IEC_UDINT ulInterval;       /* <element name="ulInterval" type="IN">Specified interval for the task</element> */
    RTS_IEC_UDINT ulStackSize;      /* <element name="ulStackSize" type="IN">Specified stack size for the task</element> */
    PFSYS_TASK_FUNCTION pFunction;      /* <element name="pFunction" type="IN">Function pointer of the task frame</element> */
    RTS_IEC_STRING szName[SYSTASK_MAX_NAME_LEN];        /* <element name="szName" type="IN">Name of the task</element> */
    PFSYS_TASK_EXCEPTIONHANDLER pExceptionHandler;      /* <element name="pExceptionHandler" type="IN">Specified exception handler of the task</element> */
    RegContext Context;     /* <element name="Context" type="IN">Current context of the task, if the task is suspended</element> */
    RTS_IEC_BYTE *pCppInstance;     /* <element name="pCppInstance" type="IN">Internal usage for C++</element> */
    RTS_IEC_BYTE *pOSSpecific;      /* <element name="pOSSpecific" type="IN">Optional pointer to operating system specific stuff</element> */
    RTS_IEC_DWORD ulFeature;        /* <element name="ulFeature" type="IN">Features flags of the task. See category "FeatureFlags" for details.</element> */
    SEHContext *pSEHContextHead;    /* <element name="pSEHContextHead" type="IN">Points to the head of the registered exception frames</element> */
    RTS_IEC_STRING *pszLongName;    /* <element name="pszLongName" type="IN">Long name of the task</element> */
    RTS_IEC_HANDLE hTaskGroup;      /* <element name="hTaskGroup" type="IN">Optional task group handle</element> */
#ifndef CMUTILSHASH_NOTIMPLEMENTED
    CMUtlHashTable threadLocalStorage; /* <element name="threadLocalStorage" type="IN">Hash table used to access thread local storage</element> */
    RTS_HANDLE hNonSysTaskHandle;   /* <element name="hNonSysTaskHandle" type="IN">Handle used to store non SysTask thread IDs.</element>*/
    CMUtlHashEntry *apHashTableBuf[SYSTASK_THREAD_LOCAL_STORAGE_HASH_TABLE_SIZE]; /* <element name="apHashTableBuf" type="IN">Static buffer for the hash table used to access thread local storage</element> */
#endif
} SYS_TASK_INFO;

/** EXTERN LIB SECTION BEGIN **/
/*  Comments are ignored for m4 compiler so restructured text can be used.  */

#ifdef __cplusplus
extern "C" {
#endif

/**
 * <description>The creator of a task can call this function to release the task object, if the task ends is execution. So the task is responsible itself to delete this object.
 *  NOTE: The task object must not be used from outside the task after calling this function!</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskautoreleaseonexit_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_RESULT SysTaskAutoReleaseOnExit;    /* VAR_OUTPUT */    
} systaskautoreleaseonexit_struct;

void CDECL CDECL_EXT systaskautoreleaseonexit(systaskautoreleaseonexit_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKAUTORELEASEONEXIT_IEC) (systaskautoreleaseonexit_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKAUTORELEASEONEXIT_NOTIMPLEMENTED)
	#define USE_systaskautoreleaseonexit
	#define EXT_systaskautoreleaseonexit
	#define GET_systaskautoreleaseonexit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskautoreleaseonexit(p0) 
	#define CHK_systaskautoreleaseonexit  FALSE
	#define EXP_systaskautoreleaseonexit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskautoreleaseonexit
	#define EXT_systaskautoreleaseonexit
	#define GET_systaskautoreleaseonexit(fl)  CAL_CMGETAPI( "systaskautoreleaseonexit" ) 
	#define CAL_systaskautoreleaseonexit  systaskautoreleaseonexit
	#define CHK_systaskautoreleaseonexit  TRUE
	#define EXP_systaskautoreleaseonexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskautoreleaseonexit", (RTS_UINTPTR)systaskautoreleaseonexit, 1, 0x3A77FA5D, 0x03051100) 
	#define EXTREF_ITF_systaskautoreleaseonexit {(RTS_VOID_FCTPTR)systaskautoreleaseonexit, "systaskautoreleaseonexit", 0x3A77FA5D, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskautoreleaseonexit
	#define EXT_systaskautoreleaseonexit
	#define GET_systaskautoreleaseonexit(fl)  CAL_CMGETAPI( "systaskautoreleaseonexit" ) 
	#define CAL_systaskautoreleaseonexit  systaskautoreleaseonexit
	#define CHK_systaskautoreleaseonexit  TRUE
	#define EXP_systaskautoreleaseonexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskautoreleaseonexit", (RTS_UINTPTR)systaskautoreleaseonexit, 1, 0x3A77FA5D, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskautoreleaseonexit
	#define EXT_SysTasksystaskautoreleaseonexit
	#define GET_SysTasksystaskautoreleaseonexit  ERR_OK
	#define CAL_SysTasksystaskautoreleaseonexit  systaskautoreleaseonexit
	#define CHK_SysTasksystaskautoreleaseonexit  TRUE
	#define EXP_SysTasksystaskautoreleaseonexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskautoreleaseonexit", (RTS_UINTPTR)systaskautoreleaseonexit, 1, 0x3A77FA5D, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskautoreleaseonexit
	#define EXT_systaskautoreleaseonexit
	#define GET_systaskautoreleaseonexit(fl)  CAL_CMGETAPI( "systaskautoreleaseonexit" ) 
	#define CAL_systaskautoreleaseonexit  systaskautoreleaseonexit
	#define CHK_systaskautoreleaseonexit  TRUE
	#define EXP_systaskautoreleaseonexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskautoreleaseonexit", (RTS_UINTPTR)systaskautoreleaseonexit, 1, 0x3A77FA5D, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskautoreleaseonexit  PFSYSTASKAUTORELEASEONEXIT_IEC pfsystaskautoreleaseonexit;
	#define EXT_systaskautoreleaseonexit  extern PFSYSTASKAUTORELEASEONEXIT_IEC pfsystaskautoreleaseonexit;
	#define GET_systaskautoreleaseonexit(fl)  s_pfCMGetAPI2( "systaskautoreleaseonexit", (RTS_VOID_FCTPTR *)&pfsystaskautoreleaseonexit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0x3A77FA5D, 0x03051100)
	#define CAL_systaskautoreleaseonexit  pfsystaskautoreleaseonexit
	#define CHK_systaskautoreleaseonexit  (pfsystaskautoreleaseonexit != NULL)
	#define EXP_systaskautoreleaseonexit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskautoreleaseonexit", (RTS_UINTPTR)systaskautoreleaseonexit, 1, 0x3A77FA5D, 0x03051100) 
#endif


/**
 * <description>NOT IMPLEMENTED YET! Function to investigate the stack task. </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskcheckstack_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_UDINT *pulMaxDepth;         /* VAR_INPUT */ /* <param name="pulMaxDepth" type="OUT">Maximum stack depth</param> */
    RTS_IEC_RESULT SysTaskCheckStack;   /* VAR_OUTPUT */    
} systaskcheckstack_struct;

void CDECL CDECL_EXT systaskcheckstack(systaskcheckstack_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKCHECKSTACK_IEC) (systaskcheckstack_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKCHECKSTACK_NOTIMPLEMENTED)
	#define USE_systaskcheckstack
	#define EXT_systaskcheckstack
	#define GET_systaskcheckstack(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskcheckstack(p0) 
	#define CHK_systaskcheckstack  FALSE
	#define EXP_systaskcheckstack  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskcheckstack
	#define EXT_systaskcheckstack
	#define GET_systaskcheckstack(fl)  CAL_CMGETAPI( "systaskcheckstack" ) 
	#define CAL_systaskcheckstack  systaskcheckstack
	#define CHK_systaskcheckstack  TRUE
	#define EXP_systaskcheckstack  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcheckstack", (RTS_UINTPTR)systaskcheckstack, 1, RTSITF_GET_SIGNATURE(0, 0xF64957C9), 0x03051100) 
	#define EXTREF_ITF_systaskcheckstack {(RTS_VOID_FCTPTR)systaskcheckstack, "systaskcheckstack", RTSITF_GET_SIGNATURE(0, 0xF64957C9), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskcheckstack
	#define EXT_systaskcheckstack
	#define GET_systaskcheckstack(fl)  CAL_CMGETAPI( "systaskcheckstack" ) 
	#define CAL_systaskcheckstack  systaskcheckstack
	#define CHK_systaskcheckstack  TRUE
	#define EXP_systaskcheckstack  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcheckstack", (RTS_UINTPTR)systaskcheckstack, 1, RTSITF_GET_SIGNATURE(0, 0xF64957C9), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskcheckstack
	#define EXT_SysTasksystaskcheckstack
	#define GET_SysTasksystaskcheckstack  ERR_OK
	#define CAL_SysTasksystaskcheckstack  systaskcheckstack
	#define CHK_SysTasksystaskcheckstack  TRUE
	#define EXP_SysTasksystaskcheckstack  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcheckstack", (RTS_UINTPTR)systaskcheckstack, 1, RTSITF_GET_SIGNATURE(0, 0xF64957C9), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskcheckstack
	#define EXT_systaskcheckstack
	#define GET_systaskcheckstack(fl)  CAL_CMGETAPI( "systaskcheckstack" ) 
	#define CAL_systaskcheckstack  systaskcheckstack
	#define CHK_systaskcheckstack  TRUE
	#define EXP_systaskcheckstack  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcheckstack", (RTS_UINTPTR)systaskcheckstack, 1, RTSITF_GET_SIGNATURE(0, 0xF64957C9), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskcheckstack  PFSYSTASKCHECKSTACK_IEC pfsystaskcheckstack;
	#define EXT_systaskcheckstack  extern PFSYSTASKCHECKSTACK_IEC pfsystaskcheckstack;
	#define GET_systaskcheckstack(fl)  s_pfCMGetAPI2( "systaskcheckstack", (RTS_VOID_FCTPTR *)&pfsystaskcheckstack, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xF64957C9), 0x03051100)
	#define CAL_systaskcheckstack  pfsystaskcheckstack
	#define CHK_systaskcheckstack  (pfsystaskcheckstack != NULL)
	#define EXP_systaskcheckstack   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcheckstack", (RTS_UINTPTR)systaskcheckstack, 1, RTSITF_GET_SIGNATURE(0, 0xF64957C9), 0x03051100) 
#endif


/**
 * <description>Is called to create a task in _run_ mode.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskcreate_struct
{
    RTS_IEC_STRING *pszTaskName;        /* VAR_INPUT */ /* <param name="pszTaskName" type="IN">The name of the task</param> */
    PFSYS_TASK_FUNCTION pFunction;      /* VAR_INPUT */ /* <param name="pFunction" type="IN">Function which implements the task: void func (SYS_TASK_PARAM/// pParam*)</param> */
    SYS_TASK_PARAM *pParam;             /* VAR_INPUT */ /* <param name="pParam" type="IN">Pointer to the argument which is passed on entry, see SYS_TASK_PARAM->pParam</param> */
    RTS_IEC_UDINT ulPriority;           /* VAR_INPUT */ /* <param name="ulPriority" type="IN">Priority of the task. Can be 0..255</param> */
    RTS_IEC_UDINT ulInterval;           /* VAR_INPUT */ /* <param name="ulInterval" type="IN">Interval in microseconds</param> */
    RTS_IEC_UDINT ulStackSize;          /* VAR_INPUT */ /* <param name="ulStackSize" type="IN">Stack size of task in bytes. 0=Default</param> */
    PFSYS_TASK_EXCEPTIONHANDLER pExceptionHandler;  /* VAR_INPUT */ /* <param name="pExceptionHandler" type="IN">Function pointer to exception handler that is called after an exception has occurred in the task</param> */
    RTS_IEC_HANDLE *phTaskHandle;       /* VAR_INPUT */ /* <param name="phTaskHandle" type="OUT">Handle to the created task</param> */
    RTS_IEC_RESULT SysTaskCreate;       /* VAR_OUTPUT */    
} systaskcreate_struct;

void CDECL CDECL_EXT systaskcreate(systaskcreate_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKCREATE_IEC) (systaskcreate_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKCREATE_NOTIMPLEMENTED)
	#define USE_systaskcreate
	#define EXT_systaskcreate
	#define GET_systaskcreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskcreate(p0) 
	#define CHK_systaskcreate  FALSE
	#define EXP_systaskcreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskcreate
	#define EXT_systaskcreate
	#define GET_systaskcreate(fl)  CAL_CMGETAPI( "systaskcreate" ) 
	#define CAL_systaskcreate  systaskcreate
	#define CHK_systaskcreate  TRUE
	#define EXP_systaskcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcreate", (RTS_UINTPTR)systaskcreate, 1, RTSITF_GET_SIGNATURE(0, 0x6A30BC52), 0x03051100) 
	#define EXTREF_ITF_systaskcreate {(RTS_VOID_FCTPTR)systaskcreate, "systaskcreate", RTSITF_GET_SIGNATURE(0, 0x6A30BC52), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskcreate
	#define EXT_systaskcreate
	#define GET_systaskcreate(fl)  CAL_CMGETAPI( "systaskcreate" ) 
	#define CAL_systaskcreate  systaskcreate
	#define CHK_systaskcreate  TRUE
	#define EXP_systaskcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcreate", (RTS_UINTPTR)systaskcreate, 1, RTSITF_GET_SIGNATURE(0, 0x6A30BC52), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskcreate
	#define EXT_SysTasksystaskcreate
	#define GET_SysTasksystaskcreate  ERR_OK
	#define CAL_SysTasksystaskcreate  systaskcreate
	#define CHK_SysTasksystaskcreate  TRUE
	#define EXP_SysTasksystaskcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcreate", (RTS_UINTPTR)systaskcreate, 1, RTSITF_GET_SIGNATURE(0, 0x6A30BC52), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskcreate
	#define EXT_systaskcreate
	#define GET_systaskcreate(fl)  CAL_CMGETAPI( "systaskcreate" ) 
	#define CAL_systaskcreate  systaskcreate
	#define CHK_systaskcreate  TRUE
	#define EXP_systaskcreate  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcreate", (RTS_UINTPTR)systaskcreate, 1, RTSITF_GET_SIGNATURE(0, 0x6A30BC52), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskcreate  PFSYSTASKCREATE_IEC pfsystaskcreate;
	#define EXT_systaskcreate  extern PFSYSTASKCREATE_IEC pfsystaskcreate;
	#define GET_systaskcreate(fl)  s_pfCMGetAPI2( "systaskcreate", (RTS_VOID_FCTPTR *)&pfsystaskcreate, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x6A30BC52), 0x03051100)
	#define CAL_systaskcreate  pfsystaskcreate
	#define CHK_systaskcreate  (pfsystaskcreate != NULL)
	#define EXP_systaskcreate   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcreate", (RTS_UINTPTR)systaskcreate, 1, RTSITF_GET_SIGNATURE(0, 0x6A30BC52), 0x03051100) 
#endif


/**
 * <description>Is called to create a task in _suspended_ mode. SysTaskResume must be called afterwards!</description>
 * <result><p>RESULT: Handle to the created task</p></result>
 */
typedef struct tagsystaskcreate2_struct
{
    RTS_IEC_STRING *pszTaskName;        /* VAR_INPUT */ /* <param name="pszTaskName" type="IN">The name of the task</param> */
    PFSYS_TASK_FUNCTION pFunction;      /* VAR_INPUT */ /* <param name="pFunction" type="IN">Function which implements the task: void func (SYS_TASK_PARAM/// pParam*)</param> */
    SYS_TASK_PARAM *pParam;             /* VAR_INPUT */ /* <param name="pParam" type="IN">Pointer to the argument which is passed on entry, see SYS_TASK_PARAM->pParam</param> */
    RTS_IEC_UDINT ulPriority;           /* VAR_INPUT */ /* <param name="ulPriority" type="IN">Priority of the task. Can be 0..255</param> */
    RTS_IEC_UDINT ulInterval;           /* VAR_INPUT */ /* <param name="ulInterval" type="IN">Interval in microseconds</param> */
    RTS_IEC_UDINT ulStackSize;          /* VAR_INPUT */ /* <param name="ulStackSize" type="IN">Stack size of task in bytes. 0=Default</param> */
    PFSYS_TASK_EXCEPTIONHANDLER pExceptionHandler;  /* VAR_INPUT */ /* <param name="pExceptionHandler" type="IN">Function pointer to exception handler that is called after an exception has occurred in the task</param> */
    RTS_IEC_RESULT *pResult;            /* VAR_INPUT */ /* <param name="pResult" type="OUT">Pointer to runtime system error code (see CmpErrors.library)</param> */
    RTS_IEC_HANDLE SysTaskCreate2;      /* VAR_OUTPUT */    
} systaskcreate2_struct;

void CDECL CDECL_EXT systaskcreate2(systaskcreate2_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKCREATE2_IEC) (systaskcreate2_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKCREATE2_NOTIMPLEMENTED)
	#define USE_systaskcreate2
	#define EXT_systaskcreate2
	#define GET_systaskcreate2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskcreate2(p0) 
	#define CHK_systaskcreate2  FALSE
	#define EXP_systaskcreate2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskcreate2
	#define EXT_systaskcreate2
	#define GET_systaskcreate2(fl)  CAL_CMGETAPI( "systaskcreate2" ) 
	#define CAL_systaskcreate2  systaskcreate2
	#define CHK_systaskcreate2  TRUE
	#define EXP_systaskcreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcreate2", (RTS_UINTPTR)systaskcreate2, 1, RTSITF_GET_SIGNATURE(0, 0x94FB2C1D), 0x03051100) 
	#define EXTREF_ITF_systaskcreate2 {(RTS_VOID_FCTPTR)systaskcreate2, "systaskcreate2", RTSITF_GET_SIGNATURE(0, 0x94FB2C1D), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskcreate2
	#define EXT_systaskcreate2
	#define GET_systaskcreate2(fl)  CAL_CMGETAPI( "systaskcreate2" ) 
	#define CAL_systaskcreate2  systaskcreate2
	#define CHK_systaskcreate2  TRUE
	#define EXP_systaskcreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcreate2", (RTS_UINTPTR)systaskcreate2, 1, RTSITF_GET_SIGNATURE(0, 0x94FB2C1D), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskcreate2
	#define EXT_SysTasksystaskcreate2
	#define GET_SysTasksystaskcreate2  ERR_OK
	#define CAL_SysTasksystaskcreate2  systaskcreate2
	#define CHK_SysTasksystaskcreate2  TRUE
	#define EXP_SysTasksystaskcreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcreate2", (RTS_UINTPTR)systaskcreate2, 1, RTSITF_GET_SIGNATURE(0, 0x94FB2C1D), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskcreate2
	#define EXT_systaskcreate2
	#define GET_systaskcreate2(fl)  CAL_CMGETAPI( "systaskcreate2" ) 
	#define CAL_systaskcreate2  systaskcreate2
	#define CHK_systaskcreate2  TRUE
	#define EXP_systaskcreate2  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcreate2", (RTS_UINTPTR)systaskcreate2, 1, RTSITF_GET_SIGNATURE(0, 0x94FB2C1D), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskcreate2  PFSYSTASKCREATE2_IEC pfsystaskcreate2;
	#define EXT_systaskcreate2  extern PFSYSTASKCREATE2_IEC pfsystaskcreate2;
	#define GET_systaskcreate2(fl)  s_pfCMGetAPI2( "systaskcreate2", (RTS_VOID_FCTPTR *)&pfsystaskcreate2, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x94FB2C1D), 0x03051100)
	#define CAL_systaskcreate2  pfsystaskcreate2
	#define CHK_systaskcreate2  (pfsystaskcreate2 != NULL)
	#define EXP_systaskcreate2   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskcreate2", (RTS_UINTPTR)systaskcreate2, 1, RTSITF_GET_SIGNATURE(0, 0x94FB2C1D), 0x03051100) 
#endif


/**
 * <description>Is called to destroy the given task.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskdestroy_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_RESULT SysTaskDestroy;      /* VAR_OUTPUT */    
} systaskdestroy_struct;

void CDECL CDECL_EXT systaskdestroy(systaskdestroy_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKDESTROY_IEC) (systaskdestroy_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKDESTROY_NOTIMPLEMENTED)
	#define USE_systaskdestroy
	#define EXT_systaskdestroy
	#define GET_systaskdestroy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskdestroy(p0) 
	#define CHK_systaskdestroy  FALSE
	#define EXP_systaskdestroy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskdestroy
	#define EXT_systaskdestroy
	#define GET_systaskdestroy(fl)  CAL_CMGETAPI( "systaskdestroy" ) 
	#define CAL_systaskdestroy  systaskdestroy
	#define CHK_systaskdestroy  TRUE
	#define EXP_systaskdestroy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskdestroy", (RTS_UINTPTR)systaskdestroy, 1, RTSITF_GET_SIGNATURE(0, 0x23988B85), 0x03051100) 
	#define EXTREF_ITF_systaskdestroy {(RTS_VOID_FCTPTR)systaskdestroy, "systaskdestroy", RTSITF_GET_SIGNATURE(0, 0x23988B85), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskdestroy
	#define EXT_systaskdestroy
	#define GET_systaskdestroy(fl)  CAL_CMGETAPI( "systaskdestroy" ) 
	#define CAL_systaskdestroy  systaskdestroy
	#define CHK_systaskdestroy  TRUE
	#define EXP_systaskdestroy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskdestroy", (RTS_UINTPTR)systaskdestroy, 1, RTSITF_GET_SIGNATURE(0, 0x23988B85), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskdestroy
	#define EXT_SysTasksystaskdestroy
	#define GET_SysTasksystaskdestroy  ERR_OK
	#define CAL_SysTasksystaskdestroy  systaskdestroy
	#define CHK_SysTasksystaskdestroy  TRUE
	#define EXP_SysTasksystaskdestroy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskdestroy", (RTS_UINTPTR)systaskdestroy, 1, RTSITF_GET_SIGNATURE(0, 0x23988B85), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskdestroy
	#define EXT_systaskdestroy
	#define GET_systaskdestroy(fl)  CAL_CMGETAPI( "systaskdestroy" ) 
	#define CAL_systaskdestroy  systaskdestroy
	#define CHK_systaskdestroy  TRUE
	#define EXP_systaskdestroy  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskdestroy", (RTS_UINTPTR)systaskdestroy, 1, RTSITF_GET_SIGNATURE(0, 0x23988B85), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskdestroy  PFSYSTASKDESTROY_IEC pfsystaskdestroy;
	#define EXT_systaskdestroy  extern PFSYSTASKDESTROY_IEC pfsystaskdestroy;
	#define GET_systaskdestroy(fl)  s_pfCMGetAPI2( "systaskdestroy", (RTS_VOID_FCTPTR *)&pfsystaskdestroy, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x23988B85), 0x03051100)
	#define CAL_systaskdestroy  pfsystaskdestroy
	#define CHK_systaskdestroy  (pfsystaskdestroy != NULL)
	#define EXP_systaskdestroy   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskdestroy", (RTS_UINTPTR)systaskdestroy, 1, RTSITF_GET_SIGNATURE(0, 0x23988B85), 0x03051100) 
#endif


/**
 * <description>Is called from the task itself, that ends its execution.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskend_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_UDINT ulExitCode;           /* VAR_INPUT */ /* <param name="ulExitCode" type="IN">Exit code of the task</param> */
    RTS_IEC_RESULT SysTaskEnd;          /* VAR_OUTPUT */    
} systaskend_struct;

void CDECL CDECL_EXT systaskend(systaskend_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKEND_IEC) (systaskend_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKEND_NOTIMPLEMENTED)
	#define USE_systaskend
	#define EXT_systaskend
	#define GET_systaskend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskend(p0) 
	#define CHK_systaskend  FALSE
	#define EXP_systaskend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskend
	#define EXT_systaskend
	#define GET_systaskend(fl)  CAL_CMGETAPI( "systaskend" ) 
	#define CAL_systaskend  systaskend
	#define CHK_systaskend  TRUE
	#define EXP_systaskend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskend", (RTS_UINTPTR)systaskend, 1, RTSITF_GET_SIGNATURE(0, 0xE402950D), 0x03051100) 
	#define EXTREF_ITF_systaskend {(RTS_VOID_FCTPTR)systaskend, "systaskend", RTSITF_GET_SIGNATURE(0, 0xE402950D), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskend
	#define EXT_systaskend
	#define GET_systaskend(fl)  CAL_CMGETAPI( "systaskend" ) 
	#define CAL_systaskend  systaskend
	#define CHK_systaskend  TRUE
	#define EXP_systaskend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskend", (RTS_UINTPTR)systaskend, 1, RTSITF_GET_SIGNATURE(0, 0xE402950D), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskend
	#define EXT_SysTasksystaskend
	#define GET_SysTasksystaskend  ERR_OK
	#define CAL_SysTasksystaskend  systaskend
	#define CHK_SysTasksystaskend  TRUE
	#define EXP_SysTasksystaskend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskend", (RTS_UINTPTR)systaskend, 1, RTSITF_GET_SIGNATURE(0, 0xE402950D), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskend
	#define EXT_systaskend
	#define GET_systaskend(fl)  CAL_CMGETAPI( "systaskend" ) 
	#define CAL_systaskend  systaskend
	#define CHK_systaskend  TRUE
	#define EXP_systaskend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskend", (RTS_UINTPTR)systaskend, 1, RTSITF_GET_SIGNATURE(0, 0xE402950D), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskend  PFSYSTASKEND_IEC pfsystaskend;
	#define EXT_systaskend  extern PFSYSTASKEND_IEC pfsystaskend;
	#define GET_systaskend(fl)  s_pfCMGetAPI2( "systaskend", (RTS_VOID_FCTPTR *)&pfsystaskend, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xE402950D), 0x03051100)
	#define CAL_systaskend  pfsystaskend
	#define CHK_systaskend  (pfsystaskend != NULL)
	#define EXP_systaskend   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskend", (RTS_UINTPTR)systaskend, 1, RTSITF_GET_SIGNATURE(0, 0xE402950D), 0x03051100) 
#endif


/**
 * <description>This function is called to mark entering the while loop.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskenter_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_RESULT SysTaskEnter;        /* VAR_OUTPUT */    
} systaskenter_struct;

void CDECL CDECL_EXT systaskenter(systaskenter_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKENTER_IEC) (systaskenter_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKENTER_NOTIMPLEMENTED)
	#define USE_systaskenter
	#define EXT_systaskenter
	#define GET_systaskenter(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskenter(p0) 
	#define CHK_systaskenter  FALSE
	#define EXP_systaskenter  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskenter
	#define EXT_systaskenter
	#define GET_systaskenter(fl)  CAL_CMGETAPI( "systaskenter" ) 
	#define CAL_systaskenter  systaskenter
	#define CHK_systaskenter  TRUE
	#define EXP_systaskenter  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskenter", (RTS_UINTPTR)systaskenter, 1, RTSITF_GET_SIGNATURE(0, 0x91B321BC), 0x03051100) 
	#define EXTREF_ITF_systaskenter {(RTS_VOID_FCTPTR)systaskenter, "systaskenter", RTSITF_GET_SIGNATURE(0, 0x91B321BC), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskenter
	#define EXT_systaskenter
	#define GET_systaskenter(fl)  CAL_CMGETAPI( "systaskenter" ) 
	#define CAL_systaskenter  systaskenter
	#define CHK_systaskenter  TRUE
	#define EXP_systaskenter  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskenter", (RTS_UINTPTR)systaskenter, 1, RTSITF_GET_SIGNATURE(0, 0x91B321BC), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskenter
	#define EXT_SysTasksystaskenter
	#define GET_SysTasksystaskenter  ERR_OK
	#define CAL_SysTasksystaskenter  systaskenter
	#define CHK_SysTasksystaskenter  TRUE
	#define EXP_SysTasksystaskenter  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskenter", (RTS_UINTPTR)systaskenter, 1, RTSITF_GET_SIGNATURE(0, 0x91B321BC), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskenter
	#define EXT_systaskenter
	#define GET_systaskenter(fl)  CAL_CMGETAPI( "systaskenter" ) 
	#define CAL_systaskenter  systaskenter
	#define CHK_systaskenter  TRUE
	#define EXP_systaskenter  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskenter", (RTS_UINTPTR)systaskenter, 1, RTSITF_GET_SIGNATURE(0, 0x91B321BC), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskenter  PFSYSTASKENTER_IEC pfsystaskenter;
	#define EXT_systaskenter  extern PFSYSTASKENTER_IEC pfsystaskenter;
	#define GET_systaskenter(fl)  s_pfCMGetAPI2( "systaskenter", (RTS_VOID_FCTPTR *)&pfsystaskenter, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x91B321BC), 0x03051100)
	#define CAL_systaskenter  pfsystaskenter
	#define CHK_systaskenter  (pfsystaskenter != NULL)
	#define EXP_systaskenter   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskenter", (RTS_UINTPTR)systaskenter, 1, RTSITF_GET_SIGNATURE(0, 0x91B321BC), 0x03051100) 
#endif


/**
 * <description>Tries to exit the given task gracefully. If the task doesn't answer in the specified timeout, then the task will be deleted hard!</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskexit_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_UDINT ulTimeoutMs;          /* VAR_INPUT */ /* <param name="ulTimeoutMs" type="IN">Timeout in milliseconds</param> */
    RTS_IEC_RESULT SysTaskExit;         /* VAR_OUTPUT */    
} systaskexit_struct;

void CDECL CDECL_EXT systaskexit(systaskexit_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKEXIT_IEC) (systaskexit_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKEXIT_NOTIMPLEMENTED)
	#define USE_systaskexit
	#define EXT_systaskexit
	#define GET_systaskexit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskexit(p0) 
	#define CHK_systaskexit  FALSE
	#define EXP_systaskexit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskexit
	#define EXT_systaskexit
	#define GET_systaskexit(fl)  CAL_CMGETAPI( "systaskexit" ) 
	#define CAL_systaskexit  systaskexit
	#define CHK_systaskexit  TRUE
	#define EXP_systaskexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskexit", (RTS_UINTPTR)systaskexit, 1, RTSITF_GET_SIGNATURE(0, 0x643B6FFF), 0x03051100) 
	#define EXTREF_ITF_systaskexit {(RTS_VOID_FCTPTR)systaskexit, "systaskexit", RTSITF_GET_SIGNATURE(0, 0x643B6FFF), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskexit
	#define EXT_systaskexit
	#define GET_systaskexit(fl)  CAL_CMGETAPI( "systaskexit" ) 
	#define CAL_systaskexit  systaskexit
	#define CHK_systaskexit  TRUE
	#define EXP_systaskexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskexit", (RTS_UINTPTR)systaskexit, 1, RTSITF_GET_SIGNATURE(0, 0x643B6FFF), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskexit
	#define EXT_SysTasksystaskexit
	#define GET_SysTasksystaskexit  ERR_OK
	#define CAL_SysTasksystaskexit  systaskexit
	#define CHK_SysTasksystaskexit  TRUE
	#define EXP_SysTasksystaskexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskexit", (RTS_UINTPTR)systaskexit, 1, RTSITF_GET_SIGNATURE(0, 0x643B6FFF), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskexit
	#define EXT_systaskexit
	#define GET_systaskexit(fl)  CAL_CMGETAPI( "systaskexit" ) 
	#define CAL_systaskexit  systaskexit
	#define CHK_systaskexit  TRUE
	#define EXP_systaskexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskexit", (RTS_UINTPTR)systaskexit, 1, RTSITF_GET_SIGNATURE(0, 0x643B6FFF), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskexit  PFSYSTASKEXIT_IEC pfsystaskexit;
	#define EXT_systaskexit  extern PFSYSTASKEXIT_IEC pfsystaskexit;
	#define GET_systaskexit(fl)  s_pfCMGetAPI2( "systaskexit", (RTS_VOID_FCTPTR *)&pfsystaskexit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x643B6FFF), 0x03051100)
	#define CAL_systaskexit  pfsystaskexit
	#define CHK_systaskexit  (pfsystaskexit != NULL)
	#define EXP_systaskexit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskexit", (RTS_UINTPTR)systaskexit, 1, RTSITF_GET_SIGNATURE(0, 0x643B6FFF), 0x03051100) 
#endif


/**
 * <description>Call the corresponding exception handler of the task.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskgenerateexception_struct
{
    RTS_IEC_HANDLE ulTaskOSHandle;      /* VAR_INPUT */ /* <param name="ulTaskOSHandle" type="IN">Operating system handle of the task</param> */
    RTS_IEC_UDINT ulException;          /* VAR_INPUT */ /* <param name="ulException" type="IN">Runtime system standard exception</param> */
    RegContext Context;                 /* VAR_INPUT */ /* <param name="Context" type="IN">Context to detect the code location where the exception occurred</param> */
    RTS_IEC_RESULT SysTaskGenerateException;    /* VAR_OUTPUT */    
} systaskgenerateexception_struct;

void CDECL CDECL_EXT systaskgenerateexception(systaskgenerateexception_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGENERATEEXCEPTION_IEC) (systaskgenerateexception_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGENERATEEXCEPTION_NOTIMPLEMENTED)
	#define USE_systaskgenerateexception
	#define EXT_systaskgenerateexception
	#define GET_systaskgenerateexception(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgenerateexception(p0) 
	#define CHK_systaskgenerateexception  FALSE
	#define EXP_systaskgenerateexception  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgenerateexception
	#define EXT_systaskgenerateexception
	#define GET_systaskgenerateexception(fl)  CAL_CMGETAPI( "systaskgenerateexception" ) 
	#define CAL_systaskgenerateexception  systaskgenerateexception
	#define CHK_systaskgenerateexception  TRUE
	#define EXP_systaskgenerateexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgenerateexception", (RTS_UINTPTR)systaskgenerateexception, 1, RTSITF_GET_SIGNATURE(0, 0x38E403E6), 0x03051100) 
	#define EXTREF_ITF_systaskgenerateexception {(RTS_VOID_FCTPTR)systaskgenerateexception, "systaskgenerateexception", RTSITF_GET_SIGNATURE(0, 0x38E403E6), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgenerateexception
	#define EXT_systaskgenerateexception
	#define GET_systaskgenerateexception(fl)  CAL_CMGETAPI( "systaskgenerateexception" ) 
	#define CAL_systaskgenerateexception  systaskgenerateexception
	#define CHK_systaskgenerateexception  TRUE
	#define EXP_systaskgenerateexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgenerateexception", (RTS_UINTPTR)systaskgenerateexception, 1, RTSITF_GET_SIGNATURE(0, 0x38E403E6), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgenerateexception
	#define EXT_SysTasksystaskgenerateexception
	#define GET_SysTasksystaskgenerateexception  ERR_OK
	#define CAL_SysTasksystaskgenerateexception  systaskgenerateexception
	#define CHK_SysTasksystaskgenerateexception  TRUE
	#define EXP_SysTasksystaskgenerateexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgenerateexception", (RTS_UINTPTR)systaskgenerateexception, 1, RTSITF_GET_SIGNATURE(0, 0x38E403E6), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgenerateexception
	#define EXT_systaskgenerateexception
	#define GET_systaskgenerateexception(fl)  CAL_CMGETAPI( "systaskgenerateexception" ) 
	#define CAL_systaskgenerateexception  systaskgenerateexception
	#define CHK_systaskgenerateexception  TRUE
	#define EXP_systaskgenerateexception  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgenerateexception", (RTS_UINTPTR)systaskgenerateexception, 1, RTSITF_GET_SIGNATURE(0, 0x38E403E6), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgenerateexception  PFSYSTASKGENERATEEXCEPTION_IEC pfsystaskgenerateexception;
	#define EXT_systaskgenerateexception  extern PFSYSTASKGENERATEEXCEPTION_IEC pfsystaskgenerateexception;
	#define GET_systaskgenerateexception(fl)  s_pfCMGetAPI2( "systaskgenerateexception", (RTS_VOID_FCTPTR *)&pfsystaskgenerateexception, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x38E403E6), 0x03051100)
	#define CAL_systaskgenerateexception  pfsystaskgenerateexception
	#define CHK_systaskgenerateexception  (pfsystaskgenerateexception != NULL)
	#define EXP_systaskgenerateexception   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgenerateexception", (RTS_UINTPTR)systaskgenerateexception, 1, RTSITF_GET_SIGNATURE(0, 0x38E403E6), 0x03051100) 
#endif


/**
 * <description>Get the current register context of the task. Task must be in suspended mode!</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskgetcontext_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RegContext *pContext;               /* VAR_INPUT */ /* <param name="pContext" type="OUT">Pointer to context</param> */
    RTS_IEC_RESULT SysTaskGetContext;   /* VAR_OUTPUT */    
} systaskgetcontext_struct;

void CDECL CDECL_EXT systaskgetcontext(systaskgetcontext_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGETCONTEXT_IEC) (systaskgetcontext_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETCONTEXT_NOTIMPLEMENTED)
	#define USE_systaskgetcontext
	#define EXT_systaskgetcontext
	#define GET_systaskgetcontext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgetcontext(p0) 
	#define CHK_systaskgetcontext  FALSE
	#define EXP_systaskgetcontext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgetcontext
	#define EXT_systaskgetcontext
	#define GET_systaskgetcontext(fl)  CAL_CMGETAPI( "systaskgetcontext" ) 
	#define CAL_systaskgetcontext  systaskgetcontext
	#define CHK_systaskgetcontext  TRUE
	#define EXP_systaskgetcontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcontext", (RTS_UINTPTR)systaskgetcontext, 1, RTSITF_GET_SIGNATURE(0, 0x57FAA367), 0x03051100) 
	#define EXTREF_ITF_systaskgetcontext {(RTS_VOID_FCTPTR)systaskgetcontext, "systaskgetcontext", RTSITF_GET_SIGNATURE(0, 0x57FAA367), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgetcontext
	#define EXT_systaskgetcontext
	#define GET_systaskgetcontext(fl)  CAL_CMGETAPI( "systaskgetcontext" ) 
	#define CAL_systaskgetcontext  systaskgetcontext
	#define CHK_systaskgetcontext  TRUE
	#define EXP_systaskgetcontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcontext", (RTS_UINTPTR)systaskgetcontext, 1, RTSITF_GET_SIGNATURE(0, 0x57FAA367), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgetcontext
	#define EXT_SysTasksystaskgetcontext
	#define GET_SysTasksystaskgetcontext  ERR_OK
	#define CAL_SysTasksystaskgetcontext  systaskgetcontext
	#define CHK_SysTasksystaskgetcontext  TRUE
	#define EXP_SysTasksystaskgetcontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcontext", (RTS_UINTPTR)systaskgetcontext, 1, RTSITF_GET_SIGNATURE(0, 0x57FAA367), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgetcontext
	#define EXT_systaskgetcontext
	#define GET_systaskgetcontext(fl)  CAL_CMGETAPI( "systaskgetcontext" ) 
	#define CAL_systaskgetcontext  systaskgetcontext
	#define CHK_systaskgetcontext  TRUE
	#define EXP_systaskgetcontext  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcontext", (RTS_UINTPTR)systaskgetcontext, 1, RTSITF_GET_SIGNATURE(0, 0x57FAA367), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgetcontext  PFSYSTASKGETCONTEXT_IEC pfsystaskgetcontext;
	#define EXT_systaskgetcontext  extern PFSYSTASKGETCONTEXT_IEC pfsystaskgetcontext;
	#define GET_systaskgetcontext(fl)  s_pfCMGetAPI2( "systaskgetcontext", (RTS_VOID_FCTPTR *)&pfsystaskgetcontext, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x57FAA367), 0x03051100)
	#define CAL_systaskgetcontext  pfsystaskgetcontext
	#define CHK_systaskgetcontext  (pfsystaskgetcontext != NULL)
	#define EXP_systaskgetcontext   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcontext", (RTS_UINTPTR)systaskgetcontext, 1, RTSITF_GET_SIGNATURE(0, 0x57FAA367), 0x03051100) 
#endif


/**
 * <description>Returns the handle of the current running task.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskgetcurrent_struct
{
    RTS_IEC_HANDLE *phTask;             /* VAR_INPUT */ /* <param name="phTask" type="OUT">Pointer to task handle</param> */
    RTS_IEC_RESULT SysTaskGetCurrent;   /* VAR_OUTPUT */    
} systaskgetcurrent_struct;

void CDECL CDECL_EXT systaskgetcurrent(systaskgetcurrent_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGETCURRENT_IEC) (systaskgetcurrent_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETCURRENT_NOTIMPLEMENTED)
	#define USE_systaskgetcurrent
	#define EXT_systaskgetcurrent
	#define GET_systaskgetcurrent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgetcurrent(p0) 
	#define CHK_systaskgetcurrent  FALSE
	#define EXP_systaskgetcurrent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgetcurrent
	#define EXT_systaskgetcurrent
	#define GET_systaskgetcurrent(fl)  CAL_CMGETAPI( "systaskgetcurrent" ) 
	#define CAL_systaskgetcurrent  systaskgetcurrent
	#define CHK_systaskgetcurrent  TRUE
	#define EXP_systaskgetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcurrent", (RTS_UINTPTR)systaskgetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x79AE7BFF), 0x03051100) 
	#define EXTREF_ITF_systaskgetcurrent {(RTS_VOID_FCTPTR)systaskgetcurrent, "systaskgetcurrent", RTSITF_GET_SIGNATURE(0, 0x79AE7BFF), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgetcurrent
	#define EXT_systaskgetcurrent
	#define GET_systaskgetcurrent(fl)  CAL_CMGETAPI( "systaskgetcurrent" ) 
	#define CAL_systaskgetcurrent  systaskgetcurrent
	#define CHK_systaskgetcurrent  TRUE
	#define EXP_systaskgetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcurrent", (RTS_UINTPTR)systaskgetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x79AE7BFF), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgetcurrent
	#define EXT_SysTasksystaskgetcurrent
	#define GET_SysTasksystaskgetcurrent  ERR_OK
	#define CAL_SysTasksystaskgetcurrent  systaskgetcurrent
	#define CHK_SysTasksystaskgetcurrent  TRUE
	#define EXP_SysTasksystaskgetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcurrent", (RTS_UINTPTR)systaskgetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x79AE7BFF), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgetcurrent
	#define EXT_systaskgetcurrent
	#define GET_systaskgetcurrent(fl)  CAL_CMGETAPI( "systaskgetcurrent" ) 
	#define CAL_systaskgetcurrent  systaskgetcurrent
	#define CHK_systaskgetcurrent  TRUE
	#define EXP_systaskgetcurrent  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcurrent", (RTS_UINTPTR)systaskgetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x79AE7BFF), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgetcurrent  PFSYSTASKGETCURRENT_IEC pfsystaskgetcurrent;
	#define EXT_systaskgetcurrent  extern PFSYSTASKGETCURRENT_IEC pfsystaskgetcurrent;
	#define GET_systaskgetcurrent(fl)  s_pfCMGetAPI2( "systaskgetcurrent", (RTS_VOID_FCTPTR *)&pfsystaskgetcurrent, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x79AE7BFF), 0x03051100)
	#define CAL_systaskgetcurrent  pfsystaskgetcurrent
	#define CHK_systaskgetcurrent  (pfsystaskgetcurrent != NULL)
	#define EXP_systaskgetcurrent   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcurrent", (RTS_UINTPTR)systaskgetcurrent, 1, RTSITF_GET_SIGNATURE(0, 0x79AE7BFF), 0x03051100) 
#endif


/**
 * <description>Returns the operating system handle of the current running task.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskgetcurrentoshandle_struct
{
    RTS_IEC_HANDLE *puiTaskOSHandle;    /* VAR_INPUT */ /* <param name="puiTaskOSHandle" type="OUT">Pointer to operating system task handle</param> */
    RTS_IEC_RESULT SysTaskGetCurrentOSHandle;   /* VAR_OUTPUT */    
} systaskgetcurrentoshandle_struct;

void CDECL CDECL_EXT systaskgetcurrentoshandle(systaskgetcurrentoshandle_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGETCURRENTOSHANDLE_IEC) (systaskgetcurrentoshandle_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETCURRENTOSHANDLE_NOTIMPLEMENTED)
	#define USE_systaskgetcurrentoshandle
	#define EXT_systaskgetcurrentoshandle
	#define GET_systaskgetcurrentoshandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgetcurrentoshandle(p0) 
	#define CHK_systaskgetcurrentoshandle  FALSE
	#define EXP_systaskgetcurrentoshandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgetcurrentoshandle
	#define EXT_systaskgetcurrentoshandle
	#define GET_systaskgetcurrentoshandle(fl)  CAL_CMGETAPI( "systaskgetcurrentoshandle" ) 
	#define CAL_systaskgetcurrentoshandle  systaskgetcurrentoshandle
	#define CHK_systaskgetcurrentoshandle  TRUE
	#define EXP_systaskgetcurrentoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcurrentoshandle", (RTS_UINTPTR)systaskgetcurrentoshandle, 1, RTSITF_GET_SIGNATURE(0, 0x9A7C0E47), 0x03051100) 
	#define EXTREF_ITF_systaskgetcurrentoshandle {(RTS_VOID_FCTPTR)systaskgetcurrentoshandle, "systaskgetcurrentoshandle", RTSITF_GET_SIGNATURE(0, 0x9A7C0E47), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgetcurrentoshandle
	#define EXT_systaskgetcurrentoshandle
	#define GET_systaskgetcurrentoshandle(fl)  CAL_CMGETAPI( "systaskgetcurrentoshandle" ) 
	#define CAL_systaskgetcurrentoshandle  systaskgetcurrentoshandle
	#define CHK_systaskgetcurrentoshandle  TRUE
	#define EXP_systaskgetcurrentoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcurrentoshandle", (RTS_UINTPTR)systaskgetcurrentoshandle, 1, RTSITF_GET_SIGNATURE(0, 0x9A7C0E47), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgetcurrentoshandle
	#define EXT_SysTasksystaskgetcurrentoshandle
	#define GET_SysTasksystaskgetcurrentoshandle  ERR_OK
	#define CAL_SysTasksystaskgetcurrentoshandle  systaskgetcurrentoshandle
	#define CHK_SysTasksystaskgetcurrentoshandle  TRUE
	#define EXP_SysTasksystaskgetcurrentoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcurrentoshandle", (RTS_UINTPTR)systaskgetcurrentoshandle, 1, RTSITF_GET_SIGNATURE(0, 0x9A7C0E47), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgetcurrentoshandle
	#define EXT_systaskgetcurrentoshandle
	#define GET_systaskgetcurrentoshandle(fl)  CAL_CMGETAPI( "systaskgetcurrentoshandle" ) 
	#define CAL_systaskgetcurrentoshandle  systaskgetcurrentoshandle
	#define CHK_systaskgetcurrentoshandle  TRUE
	#define EXP_systaskgetcurrentoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcurrentoshandle", (RTS_UINTPTR)systaskgetcurrentoshandle, 1, RTSITF_GET_SIGNATURE(0, 0x9A7C0E47), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgetcurrentoshandle  PFSYSTASKGETCURRENTOSHANDLE_IEC pfsystaskgetcurrentoshandle;
	#define EXT_systaskgetcurrentoshandle  extern PFSYSTASKGETCURRENTOSHANDLE_IEC pfsystaskgetcurrentoshandle;
	#define GET_systaskgetcurrentoshandle(fl)  s_pfCMGetAPI2( "systaskgetcurrentoshandle", (RTS_VOID_FCTPTR *)&pfsystaskgetcurrentoshandle, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x9A7C0E47), 0x03051100)
	#define CAL_systaskgetcurrentoshandle  pfsystaskgetcurrentoshandle
	#define CHK_systaskgetcurrentoshandle  (pfsystaskgetcurrentoshandle != NULL)
	#define EXP_systaskgetcurrentoshandle   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetcurrentoshandle", (RTS_UINTPTR)systaskgetcurrentoshandle, 1, RTSITF_GET_SIGNATURE(0, 0x9A7C0E47), 0x03051100) 
#endif


/**
 * <description>Returns the task info of the specified task.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskgetinfo_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    SYS_TASK_INFO **ppInfo;             /* VAR_INPUT */ /* <param name="ppInfo" type="OUT">Pointer to pointer to get task info structure</param> */
    RTS_IEC_RESULT SysTaskGetInfo;      /* VAR_OUTPUT */    
} systaskgetinfo_struct;

void CDECL CDECL_EXT systaskgetinfo(systaskgetinfo_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGETINFO_IEC) (systaskgetinfo_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETINFO_NOTIMPLEMENTED)
	#define USE_systaskgetinfo
	#define EXT_systaskgetinfo
	#define GET_systaskgetinfo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgetinfo(p0) 
	#define CHK_systaskgetinfo  FALSE
	#define EXP_systaskgetinfo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgetinfo
	#define EXT_systaskgetinfo
	#define GET_systaskgetinfo(fl)  CAL_CMGETAPI( "systaskgetinfo" ) 
	#define CAL_systaskgetinfo  systaskgetinfo
	#define CHK_systaskgetinfo  TRUE
	#define EXP_systaskgetinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetinfo", (RTS_UINTPTR)systaskgetinfo, 1, RTSITF_GET_SIGNATURE(0, 0xB44E2B3F), 0x03051100) 
	#define EXTREF_ITF_systaskgetinfo {(RTS_VOID_FCTPTR)systaskgetinfo, "systaskgetinfo", RTSITF_GET_SIGNATURE(0, 0xB44E2B3F), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgetinfo
	#define EXT_systaskgetinfo
	#define GET_systaskgetinfo(fl)  CAL_CMGETAPI( "systaskgetinfo" ) 
	#define CAL_systaskgetinfo  systaskgetinfo
	#define CHK_systaskgetinfo  TRUE
	#define EXP_systaskgetinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetinfo", (RTS_UINTPTR)systaskgetinfo, 1, RTSITF_GET_SIGNATURE(0, 0xB44E2B3F), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgetinfo
	#define EXT_SysTasksystaskgetinfo
	#define GET_SysTasksystaskgetinfo  ERR_OK
	#define CAL_SysTasksystaskgetinfo  systaskgetinfo
	#define CHK_SysTasksystaskgetinfo  TRUE
	#define EXP_SysTasksystaskgetinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetinfo", (RTS_UINTPTR)systaskgetinfo, 1, RTSITF_GET_SIGNATURE(0, 0xB44E2B3F), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgetinfo
	#define EXT_systaskgetinfo
	#define GET_systaskgetinfo(fl)  CAL_CMGETAPI( "systaskgetinfo" ) 
	#define CAL_systaskgetinfo  systaskgetinfo
	#define CHK_systaskgetinfo  TRUE
	#define EXP_systaskgetinfo  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetinfo", (RTS_UINTPTR)systaskgetinfo, 1, RTSITF_GET_SIGNATURE(0, 0xB44E2B3F), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgetinfo  PFSYSTASKGETINFO_IEC pfsystaskgetinfo;
	#define EXT_systaskgetinfo  extern PFSYSTASKGETINFO_IEC pfsystaskgetinfo;
	#define GET_systaskgetinfo(fl)  s_pfCMGetAPI2( "systaskgetinfo", (RTS_VOID_FCTPTR *)&pfsystaskgetinfo, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB44E2B3F), 0x03051100)
	#define CAL_systaskgetinfo  pfsystaskgetinfo
	#define CHK_systaskgetinfo  (pfsystaskgetinfo != NULL)
	#define EXP_systaskgetinfo   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetinfo", (RTS_UINTPTR)systaskgetinfo, 1, RTSITF_GET_SIGNATURE(0, 0xB44E2B3F), 0x03051100) 
#endif


/**
 * <description>Get the actual interval of a cyclic task. If the specified task is no cyclic task, the function return an error.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskgetinterval_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to the task</param> */
    RTS_IEC_UDINT *pulInterval;         /* VAR_IN_OUT */    /* <param name="pulInterval" type="OUT">Interval of the task in microseconds!</param> */
    RTS_IEC_RESULT SysTaskGetInterval;  /* VAR_OUTPUT */    
} systaskgetinterval_struct;

void CDECL CDECL_EXT systaskgetinterval(systaskgetinterval_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGETINTERVAL_IEC) (systaskgetinterval_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETINTERVAL_NOTIMPLEMENTED)
	#define USE_systaskgetinterval
	#define EXT_systaskgetinterval
	#define GET_systaskgetinterval(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgetinterval(p0) 
	#define CHK_systaskgetinterval  FALSE
	#define EXP_systaskgetinterval  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgetinterval
	#define EXT_systaskgetinterval
	#define GET_systaskgetinterval(fl)  CAL_CMGETAPI( "systaskgetinterval" ) 
	#define CAL_systaskgetinterval  systaskgetinterval
	#define CHK_systaskgetinterval  TRUE
	#define EXP_systaskgetinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetinterval", (RTS_UINTPTR)systaskgetinterval, 1, RTSITF_GET_SIGNATURE(0, 0x4CCFF91B), 0x03051100) 
	#define EXTREF_ITF_systaskgetinterval {(RTS_VOID_FCTPTR)systaskgetinterval, "systaskgetinterval", RTSITF_GET_SIGNATURE(0, 0x4CCFF91B), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgetinterval
	#define EXT_systaskgetinterval
	#define GET_systaskgetinterval(fl)  CAL_CMGETAPI( "systaskgetinterval" ) 
	#define CAL_systaskgetinterval  systaskgetinterval
	#define CHK_systaskgetinterval  TRUE
	#define EXP_systaskgetinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetinterval", (RTS_UINTPTR)systaskgetinterval, 1, RTSITF_GET_SIGNATURE(0, 0x4CCFF91B), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgetinterval
	#define EXT_SysTasksystaskgetinterval
	#define GET_SysTasksystaskgetinterval  ERR_OK
	#define CAL_SysTasksystaskgetinterval  systaskgetinterval
	#define CHK_SysTasksystaskgetinterval  TRUE
	#define EXP_SysTasksystaskgetinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetinterval", (RTS_UINTPTR)systaskgetinterval, 1, RTSITF_GET_SIGNATURE(0, 0x4CCFF91B), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgetinterval
	#define EXT_systaskgetinterval
	#define GET_systaskgetinterval(fl)  CAL_CMGETAPI( "systaskgetinterval" ) 
	#define CAL_systaskgetinterval  systaskgetinterval
	#define CHK_systaskgetinterval  TRUE
	#define EXP_systaskgetinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetinterval", (RTS_UINTPTR)systaskgetinterval, 1, RTSITF_GET_SIGNATURE(0, 0x4CCFF91B), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgetinterval  PFSYSTASKGETINTERVAL_IEC pfsystaskgetinterval;
	#define EXT_systaskgetinterval  extern PFSYSTASKGETINTERVAL_IEC pfsystaskgetinterval;
	#define GET_systaskgetinterval(fl)  s_pfCMGetAPI2( "systaskgetinterval", (RTS_VOID_FCTPTR *)&pfsystaskgetinterval, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x4CCFF91B), 0x03051100)
	#define CAL_systaskgetinterval  pfsystaskgetinterval
	#define CHK_systaskgetinterval  (pfsystaskgetinterval != NULL)
	#define EXP_systaskgetinterval   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetinterval", (RTS_UINTPTR)systaskgetinterval, 1, RTSITF_GET_SIGNATURE(0, 0x4CCFF91B), 0x03051100) 
#endif


/**
 * <description>Returns the task name of the specified task.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskgetname_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_RESULT *pResult;            /* VAR_INPUT */ /* <param name="pResult" type="OUT">Pointer to runtime system error code (see CmpErrors.library)</param> */
    RTS_IEC_STRING *SysTaskGetName;     /* VAR_OUTPUT */    
} systaskgetname_struct;

void CDECL CDECL_EXT systaskgetname(systaskgetname_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGETNAME_IEC) (systaskgetname_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETNAME_NOTIMPLEMENTED)
	#define USE_systaskgetname
	#define EXT_systaskgetname
	#define GET_systaskgetname(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgetname(p0) 
	#define CHK_systaskgetname  FALSE
	#define EXP_systaskgetname  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgetname
	#define EXT_systaskgetname
	#define GET_systaskgetname(fl)  CAL_CMGETAPI( "systaskgetname" ) 
	#define CAL_systaskgetname  systaskgetname
	#define CHK_systaskgetname  TRUE
	#define EXP_systaskgetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetname", (RTS_UINTPTR)systaskgetname, 1, 0xC7690C59, 0x03051100) 
	#define EXTREF_ITF_systaskgetname {(RTS_VOID_FCTPTR)systaskgetname, "systaskgetname", 0xC7690C59, 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgetname
	#define EXT_systaskgetname
	#define GET_systaskgetname(fl)  CAL_CMGETAPI( "systaskgetname" ) 
	#define CAL_systaskgetname  systaskgetname
	#define CHK_systaskgetname  TRUE
	#define EXP_systaskgetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetname", (RTS_UINTPTR)systaskgetname, 1, 0xC7690C59, 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgetname
	#define EXT_SysTasksystaskgetname
	#define GET_SysTasksystaskgetname  ERR_OK
	#define CAL_SysTasksystaskgetname  systaskgetname
	#define CHK_SysTasksystaskgetname  TRUE
	#define EXP_SysTasksystaskgetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetname", (RTS_UINTPTR)systaskgetname, 1, 0xC7690C59, 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgetname
	#define EXT_systaskgetname
	#define GET_systaskgetname(fl)  CAL_CMGETAPI( "systaskgetname" ) 
	#define CAL_systaskgetname  systaskgetname
	#define CHK_systaskgetname  TRUE
	#define EXP_systaskgetname  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetname", (RTS_UINTPTR)systaskgetname, 1, 0xC7690C59, 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgetname  PFSYSTASKGETNAME_IEC pfsystaskgetname;
	#define EXT_systaskgetname  extern PFSYSTASKGETNAME_IEC pfsystaskgetname;
	#define GET_systaskgetname(fl)  s_pfCMGetAPI2( "systaskgetname", (RTS_VOID_FCTPTR *)&pfsystaskgetname, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, 0xC7690C59, 0x03051100)
	#define CAL_systaskgetname  pfsystaskgetname
	#define CHK_systaskgetname  (pfsystaskgetname != NULL)
	#define EXP_systaskgetname   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetname", (RTS_UINTPTR)systaskgetname, 1, 0xC7690C59, 0x03051100) 
#endif


/**
 * <description>Function to get the operating system specific handle.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskgetoshandle_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_HANDLE SysTaskGetOSHandle;  /* VAR_OUTPUT */    
} systaskgetoshandle_struct;

void CDECL CDECL_EXT systaskgetoshandle(systaskgetoshandle_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGETOSHANDLE_IEC) (systaskgetoshandle_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETOSHANDLE_NOTIMPLEMENTED)
	#define USE_systaskgetoshandle
	#define EXT_systaskgetoshandle
	#define GET_systaskgetoshandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgetoshandle(p0) 
	#define CHK_systaskgetoshandle  FALSE
	#define EXP_systaskgetoshandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgetoshandle
	#define EXT_systaskgetoshandle
	#define GET_systaskgetoshandle(fl)  CAL_CMGETAPI( "systaskgetoshandle" ) 
	#define CAL_systaskgetoshandle  systaskgetoshandle
	#define CHK_systaskgetoshandle  TRUE
	#define EXP_systaskgetoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetoshandle", (RTS_UINTPTR)systaskgetoshandle, 1, RTSITF_GET_SIGNATURE(0, 0xD55E19B5), 0x03051100) 
	#define EXTREF_ITF_systaskgetoshandle {(RTS_VOID_FCTPTR)systaskgetoshandle, "systaskgetoshandle", RTSITF_GET_SIGNATURE(0, 0xD55E19B5), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgetoshandle
	#define EXT_systaskgetoshandle
	#define GET_systaskgetoshandle(fl)  CAL_CMGETAPI( "systaskgetoshandle" ) 
	#define CAL_systaskgetoshandle  systaskgetoshandle
	#define CHK_systaskgetoshandle  TRUE
	#define EXP_systaskgetoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetoshandle", (RTS_UINTPTR)systaskgetoshandle, 1, RTSITF_GET_SIGNATURE(0, 0xD55E19B5), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgetoshandle
	#define EXT_SysTasksystaskgetoshandle
	#define GET_SysTasksystaskgetoshandle  ERR_OK
	#define CAL_SysTasksystaskgetoshandle  systaskgetoshandle
	#define CHK_SysTasksystaskgetoshandle  TRUE
	#define EXP_SysTasksystaskgetoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetoshandle", (RTS_UINTPTR)systaskgetoshandle, 1, RTSITF_GET_SIGNATURE(0, 0xD55E19B5), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgetoshandle
	#define EXT_systaskgetoshandle
	#define GET_systaskgetoshandle(fl)  CAL_CMGETAPI( "systaskgetoshandle" ) 
	#define CAL_systaskgetoshandle  systaskgetoshandle
	#define CHK_systaskgetoshandle  TRUE
	#define EXP_systaskgetoshandle  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetoshandle", (RTS_UINTPTR)systaskgetoshandle, 1, RTSITF_GET_SIGNATURE(0, 0xD55E19B5), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgetoshandle  PFSYSTASKGETOSHANDLE_IEC pfsystaskgetoshandle;
	#define EXT_systaskgetoshandle  extern PFSYSTASKGETOSHANDLE_IEC pfsystaskgetoshandle;
	#define GET_systaskgetoshandle(fl)  s_pfCMGetAPI2( "systaskgetoshandle", (RTS_VOID_FCTPTR *)&pfsystaskgetoshandle, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xD55E19B5), 0x03051100)
	#define CAL_systaskgetoshandle  pfsystaskgetoshandle
	#define CHK_systaskgetoshandle  (pfsystaskgetoshandle != NULL)
	#define EXP_systaskgetoshandle   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetoshandle", (RTS_UINTPTR)systaskgetoshandle, 1, RTSITF_GET_SIGNATURE(0, 0xD55E19B5), 0x03051100) 
#endif


/**
 * <description>Returns the operating system priority of the given task.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskgetospriority_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_UDINT *pulOSPriority;       /* VAR_INPUT */ /* <param name="pulOSPriority" type="OUT">Pointer to get operating system priority</param> */
    RTS_IEC_RESULT SysTaskGetOSPriority;    /* VAR_OUTPUT */    
} systaskgetospriority_struct;

void CDECL CDECL_EXT systaskgetospriority(systaskgetospriority_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGETOSPRIORITY_IEC) (systaskgetospriority_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETOSPRIORITY_NOTIMPLEMENTED)
	#define USE_systaskgetospriority
	#define EXT_systaskgetospriority
	#define GET_systaskgetospriority(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgetospriority(p0) 
	#define CHK_systaskgetospriority  FALSE
	#define EXP_systaskgetospriority  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgetospriority
	#define EXT_systaskgetospriority
	#define GET_systaskgetospriority(fl)  CAL_CMGETAPI( "systaskgetospriority" ) 
	#define CAL_systaskgetospriority  systaskgetospriority
	#define CHK_systaskgetospriority  TRUE
	#define EXP_systaskgetospriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetospriority", (RTS_UINTPTR)systaskgetospriority, 1, RTSITF_GET_SIGNATURE(0, 0x21E9C566), 0x03051100) 
	#define EXTREF_ITF_systaskgetospriority {(RTS_VOID_FCTPTR)systaskgetospriority, "systaskgetospriority", RTSITF_GET_SIGNATURE(0, 0x21E9C566), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgetospriority
	#define EXT_systaskgetospriority
	#define GET_systaskgetospriority(fl)  CAL_CMGETAPI( "systaskgetospriority" ) 
	#define CAL_systaskgetospriority  systaskgetospriority
	#define CHK_systaskgetospriority  TRUE
	#define EXP_systaskgetospriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetospriority", (RTS_UINTPTR)systaskgetospriority, 1, RTSITF_GET_SIGNATURE(0, 0x21E9C566), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgetospriority
	#define EXT_SysTasksystaskgetospriority
	#define GET_SysTasksystaskgetospriority  ERR_OK
	#define CAL_SysTasksystaskgetospriority  systaskgetospriority
	#define CHK_SysTasksystaskgetospriority  TRUE
	#define EXP_SysTasksystaskgetospriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetospriority", (RTS_UINTPTR)systaskgetospriority, 1, RTSITF_GET_SIGNATURE(0, 0x21E9C566), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgetospriority
	#define EXT_systaskgetospriority
	#define GET_systaskgetospriority(fl)  CAL_CMGETAPI( "systaskgetospriority" ) 
	#define CAL_systaskgetospriority  systaskgetospriority
	#define CHK_systaskgetospriority  TRUE
	#define EXP_systaskgetospriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetospriority", (RTS_UINTPTR)systaskgetospriority, 1, RTSITF_GET_SIGNATURE(0, 0x21E9C566), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgetospriority  PFSYSTASKGETOSPRIORITY_IEC pfsystaskgetospriority;
	#define EXT_systaskgetospriority  extern PFSYSTASKGETOSPRIORITY_IEC pfsystaskgetospriority;
	#define GET_systaskgetospriority(fl)  s_pfCMGetAPI2( "systaskgetospriority", (RTS_VOID_FCTPTR *)&pfsystaskgetospriority, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x21E9C566), 0x03051100)
	#define CAL_systaskgetospriority  pfsystaskgetospriority
	#define CHK_systaskgetospriority  (pfsystaskgetospriority != NULL)
	#define EXP_systaskgetospriority   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetospriority", (RTS_UINTPTR)systaskgetospriority, 1, RTSITF_GET_SIGNATURE(0, 0x21E9C566), 0x03051100) 
#endif


/**
 * <description>Get the runtime system priority of the given task.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskgetpriority_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" TYPE="IN">Handle to the task</param> */
    RTS_IEC_UDINT *pulPriority;         /* VAR_INPUT */ /* <param name="pulPriority" type="OUT">Pointer to get priority</param> */
    RTS_IEC_RESULT SysTaskGetPriority;  /* VAR_OUTPUT */    
} systaskgetpriority_struct;

void CDECL CDECL_EXT systaskgetpriority(systaskgetpriority_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGETPRIORITY_IEC) (systaskgetpriority_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETPRIORITY_NOTIMPLEMENTED)
	#define USE_systaskgetpriority
	#define EXT_systaskgetpriority
	#define GET_systaskgetpriority(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgetpriority(p0) 
	#define CHK_systaskgetpriority  FALSE
	#define EXP_systaskgetpriority  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgetpriority
	#define EXT_systaskgetpriority
	#define GET_systaskgetpriority(fl)  CAL_CMGETAPI( "systaskgetpriority" ) 
	#define CAL_systaskgetpriority  systaskgetpriority
	#define CHK_systaskgetpriority  TRUE
	#define EXP_systaskgetpriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetpriority", (RTS_UINTPTR)systaskgetpriority, 1, RTSITF_GET_SIGNATURE(0, 0xB576F235), 0x03051100) 
	#define EXTREF_ITF_systaskgetpriority {(RTS_VOID_FCTPTR)systaskgetpriority, "systaskgetpriority", RTSITF_GET_SIGNATURE(0, 0xB576F235), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgetpriority
	#define EXT_systaskgetpriority
	#define GET_systaskgetpriority(fl)  CAL_CMGETAPI( "systaskgetpriority" ) 
	#define CAL_systaskgetpriority  systaskgetpriority
	#define CHK_systaskgetpriority  TRUE
	#define EXP_systaskgetpriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetpriority", (RTS_UINTPTR)systaskgetpriority, 1, RTSITF_GET_SIGNATURE(0, 0xB576F235), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgetpriority
	#define EXT_SysTasksystaskgetpriority
	#define GET_SysTasksystaskgetpriority  ERR_OK
	#define CAL_SysTasksystaskgetpriority  systaskgetpriority
	#define CHK_SysTasksystaskgetpriority  TRUE
	#define EXP_SysTasksystaskgetpriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetpriority", (RTS_UINTPTR)systaskgetpriority, 1, RTSITF_GET_SIGNATURE(0, 0xB576F235), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgetpriority
	#define EXT_systaskgetpriority
	#define GET_systaskgetpriority(fl)  CAL_CMGETAPI( "systaskgetpriority" ) 
	#define CAL_systaskgetpriority  systaskgetpriority
	#define CHK_systaskgetpriority  TRUE
	#define EXP_systaskgetpriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetpriority", (RTS_UINTPTR)systaskgetpriority, 1, RTSITF_GET_SIGNATURE(0, 0xB576F235), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgetpriority  PFSYSTASKGETPRIORITY_IEC pfsystaskgetpriority;
	#define EXT_systaskgetpriority  extern PFSYSTASKGETPRIORITY_IEC pfsystaskgetpriority;
	#define GET_systaskgetpriority(fl)  s_pfCMGetAPI2( "systaskgetpriority", (RTS_VOID_FCTPTR *)&pfsystaskgetpriority, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB576F235), 0x03051100)
	#define CAL_systaskgetpriority  pfsystaskgetpriority
	#define CHK_systaskgetpriority  (pfsystaskgetpriority != NULL)
	#define EXP_systaskgetpriority   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskgetpriority", (RTS_UINTPTR)systaskgetpriority, 1, RTSITF_GET_SIGNATURE(0, 0xB576F235), 0x03051100) 
#endif


/**
 * <description>Is used to wait for exit of the specified join task.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskjoin_struct
{
    RTS_IEC_HANDLE hTaskToJoin;         /* VAR_INPUT */ /* <param name="hTaskToJoin" type="IN">Task to join</param> */
    RTS_IEC_UDINT ulTimeoutMs;          /* VAR_INPUT */ /* <param name="ulTimeoutMs" type="IN">Timeout in milliseconds to wait for join task</param> */
    RTS_IEC_RESULT SysTaskJoin;         /* VAR_OUTPUT */    
} systaskjoin_struct;

void CDECL CDECL_EXT systaskjoin(systaskjoin_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKJOIN_IEC) (systaskjoin_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKJOIN_NOTIMPLEMENTED)
	#define USE_systaskjoin
	#define EXT_systaskjoin
	#define GET_systaskjoin(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskjoin(p0) 
	#define CHK_systaskjoin  FALSE
	#define EXP_systaskjoin  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskjoin
	#define EXT_systaskjoin
	#define GET_systaskjoin(fl)  CAL_CMGETAPI( "systaskjoin" ) 
	#define CAL_systaskjoin  systaskjoin
	#define CHK_systaskjoin  TRUE
	#define EXP_systaskjoin  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskjoin", (RTS_UINTPTR)systaskjoin, 1, RTSITF_GET_SIGNATURE(0, 0x379A5B0D), 0x03051100) 
	#define EXTREF_ITF_systaskjoin {(RTS_VOID_FCTPTR)systaskjoin, "systaskjoin", RTSITF_GET_SIGNATURE(0, 0x379A5B0D), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskjoin
	#define EXT_systaskjoin
	#define GET_systaskjoin(fl)  CAL_CMGETAPI( "systaskjoin" ) 
	#define CAL_systaskjoin  systaskjoin
	#define CHK_systaskjoin  TRUE
	#define EXP_systaskjoin  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskjoin", (RTS_UINTPTR)systaskjoin, 1, RTSITF_GET_SIGNATURE(0, 0x379A5B0D), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskjoin
	#define EXT_SysTasksystaskjoin
	#define GET_SysTasksystaskjoin  ERR_OK
	#define CAL_SysTasksystaskjoin  systaskjoin
	#define CHK_SysTasksystaskjoin  TRUE
	#define EXP_SysTasksystaskjoin  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskjoin", (RTS_UINTPTR)systaskjoin, 1, RTSITF_GET_SIGNATURE(0, 0x379A5B0D), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskjoin
	#define EXT_systaskjoin
	#define GET_systaskjoin(fl)  CAL_CMGETAPI( "systaskjoin" ) 
	#define CAL_systaskjoin  systaskjoin
	#define CHK_systaskjoin  TRUE
	#define EXP_systaskjoin  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskjoin", (RTS_UINTPTR)systaskjoin, 1, RTSITF_GET_SIGNATURE(0, 0x379A5B0D), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskjoin  PFSYSTASKJOIN_IEC pfsystaskjoin;
	#define EXT_systaskjoin  extern PFSYSTASKJOIN_IEC pfsystaskjoin;
	#define GET_systaskjoin(fl)  s_pfCMGetAPI2( "systaskjoin", (RTS_VOID_FCTPTR *)&pfsystaskjoin, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x379A5B0D), 0x03051100)
	#define CAL_systaskjoin  pfsystaskjoin
	#define CHK_systaskjoin  (pfsystaskjoin != NULL)
	#define EXP_systaskjoin   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskjoin", (RTS_UINTPTR)systaskjoin, 1, RTSITF_GET_SIGNATURE(0, 0x379A5B0D), 0x03051100) 
#endif


/**
 * <description>This function is called to mark leaving the while loop.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskleave_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_RESULT SysTaskLeave;        /* VAR_OUTPUT */    
} systaskleave_struct;

void CDECL CDECL_EXT systaskleave(systaskleave_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKLEAVE_IEC) (systaskleave_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKLEAVE_NOTIMPLEMENTED)
	#define USE_systaskleave
	#define EXT_systaskleave
	#define GET_systaskleave(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskleave(p0) 
	#define CHK_systaskleave  FALSE
	#define EXP_systaskleave  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskleave
	#define EXT_systaskleave
	#define GET_systaskleave(fl)  CAL_CMGETAPI( "systaskleave" ) 
	#define CAL_systaskleave  systaskleave
	#define CHK_systaskleave  TRUE
	#define EXP_systaskleave  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskleave", (RTS_UINTPTR)systaskleave, 1, RTSITF_GET_SIGNATURE(0, 0xE095052C), 0x03051100) 
	#define EXTREF_ITF_systaskleave {(RTS_VOID_FCTPTR)systaskleave, "systaskleave", RTSITF_GET_SIGNATURE(0, 0xE095052C), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskleave
	#define EXT_systaskleave
	#define GET_systaskleave(fl)  CAL_CMGETAPI( "systaskleave" ) 
	#define CAL_systaskleave  systaskleave
	#define CHK_systaskleave  TRUE
	#define EXP_systaskleave  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskleave", (RTS_UINTPTR)systaskleave, 1, RTSITF_GET_SIGNATURE(0, 0xE095052C), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskleave
	#define EXT_SysTasksystaskleave
	#define GET_SysTasksystaskleave  ERR_OK
	#define CAL_SysTasksystaskleave  systaskleave
	#define CHK_SysTasksystaskleave  TRUE
	#define EXP_SysTasksystaskleave  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskleave", (RTS_UINTPTR)systaskleave, 1, RTSITF_GET_SIGNATURE(0, 0xE095052C), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskleave
	#define EXT_systaskleave
	#define GET_systaskleave(fl)  CAL_CMGETAPI( "systaskleave" ) 
	#define CAL_systaskleave  systaskleave
	#define CHK_systaskleave  TRUE
	#define EXP_systaskleave  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskleave", (RTS_UINTPTR)systaskleave, 1, RTSITF_GET_SIGNATURE(0, 0xE095052C), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskleave  PFSYSTASKLEAVE_IEC pfsystaskleave;
	#define EXT_systaskleave  extern PFSYSTASKLEAVE_IEC pfsystaskleave;
	#define GET_systaskleave(fl)  s_pfCMGetAPI2( "systaskleave", (RTS_VOID_FCTPTR *)&pfsystaskleave, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xE095052C), 0x03051100)
	#define CAL_systaskleave  pfsystaskleave
	#define CHK_systaskleave  (pfsystaskleave != NULL)
	#define EXP_systaskleave   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskleave", (RTS_UINTPTR)systaskleave, 1, RTSITF_GET_SIGNATURE(0, 0xE095052C), 0x03051100) 
#endif


/**
 * <description>Is called to resume the given task, if the task was suspended.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskresume_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_RESULT SysTaskResume;       /* VAR_OUTPUT */    
} systaskresume_struct;

void CDECL CDECL_EXT systaskresume(systaskresume_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKRESUME_IEC) (systaskresume_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKRESUME_NOTIMPLEMENTED)
	#define USE_systaskresume
	#define EXT_systaskresume
	#define GET_systaskresume(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskresume(p0) 
	#define CHK_systaskresume  FALSE
	#define EXP_systaskresume  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskresume
	#define EXT_systaskresume
	#define GET_systaskresume(fl)  CAL_CMGETAPI( "systaskresume" ) 
	#define CAL_systaskresume  systaskresume
	#define CHK_systaskresume  TRUE
	#define EXP_systaskresume  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskresume", (RTS_UINTPTR)systaskresume, 1, RTSITF_GET_SIGNATURE(0, 0x3C4C4CF9), 0x03051100) 
	#define EXTREF_ITF_systaskresume {(RTS_VOID_FCTPTR)systaskresume, "systaskresume", RTSITF_GET_SIGNATURE(0, 0x3C4C4CF9), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskresume
	#define EXT_systaskresume
	#define GET_systaskresume(fl)  CAL_CMGETAPI( "systaskresume" ) 
	#define CAL_systaskresume  systaskresume
	#define CHK_systaskresume  TRUE
	#define EXP_systaskresume  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskresume", (RTS_UINTPTR)systaskresume, 1, RTSITF_GET_SIGNATURE(0, 0x3C4C4CF9), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskresume
	#define EXT_SysTasksystaskresume
	#define GET_SysTasksystaskresume  ERR_OK
	#define CAL_SysTasksystaskresume  systaskresume
	#define CHK_SysTasksystaskresume  TRUE
	#define EXP_SysTasksystaskresume  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskresume", (RTS_UINTPTR)systaskresume, 1, RTSITF_GET_SIGNATURE(0, 0x3C4C4CF9), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskresume
	#define EXT_systaskresume
	#define GET_systaskresume(fl)  CAL_CMGETAPI( "systaskresume" ) 
	#define CAL_systaskresume  systaskresume
	#define CHK_systaskresume  TRUE
	#define EXP_systaskresume  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskresume", (RTS_UINTPTR)systaskresume, 1, RTSITF_GET_SIGNATURE(0, 0x3C4C4CF9), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskresume  PFSYSTASKRESUME_IEC pfsystaskresume;
	#define EXT_systaskresume  extern PFSYSTASKRESUME_IEC pfsystaskresume;
	#define GET_systaskresume(fl)  s_pfCMGetAPI2( "systaskresume", (RTS_VOID_FCTPTR *)&pfsystaskresume, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x3C4C4CF9), 0x03051100)
	#define CAL_systaskresume  pfsystaskresume
	#define CHK_systaskresume  (pfsystaskresume != NULL)
	#define EXP_systaskresume   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskresume", (RTS_UINTPTR)systaskresume, 1, RTSITF_GET_SIGNATURE(0, 0x3C4C4CF9), 0x03051100) 
#endif


/**
 * <description>Set the exit flag of the specified task. On the next cycle, the task will exit.</description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystasksetexit_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_RESULT SysTaskSetExit;      /* VAR_OUTPUT */    
} systasksetexit_struct;

void CDECL CDECL_EXT systasksetexit(systasksetexit_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKSETEXIT_IEC) (systasksetexit_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKSETEXIT_NOTIMPLEMENTED)
	#define USE_systasksetexit
	#define EXT_systasksetexit
	#define GET_systasksetexit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systasksetexit(p0) 
	#define CHK_systasksetexit  FALSE
	#define EXP_systasksetexit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systasksetexit
	#define EXT_systasksetexit
	#define GET_systasksetexit(fl)  CAL_CMGETAPI( "systasksetexit" ) 
	#define CAL_systasksetexit  systasksetexit
	#define CHK_systasksetexit  TRUE
	#define EXP_systasksetexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetexit", (RTS_UINTPTR)systasksetexit, 1, RTSITF_GET_SIGNATURE(0, 0x137693C1), 0x03051100) 
	#define EXTREF_ITF_systasksetexit {(RTS_VOID_FCTPTR)systasksetexit, "systasksetexit", RTSITF_GET_SIGNATURE(0, 0x137693C1), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systasksetexit
	#define EXT_systasksetexit
	#define GET_systasksetexit(fl)  CAL_CMGETAPI( "systasksetexit" ) 
	#define CAL_systasksetexit  systasksetexit
	#define CHK_systasksetexit  TRUE
	#define EXP_systasksetexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetexit", (RTS_UINTPTR)systasksetexit, 1, RTSITF_GET_SIGNATURE(0, 0x137693C1), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystasksetexit
	#define EXT_SysTasksystasksetexit
	#define GET_SysTasksystasksetexit  ERR_OK
	#define CAL_SysTasksystasksetexit  systasksetexit
	#define CHK_SysTasksystasksetexit  TRUE
	#define EXP_SysTasksystasksetexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetexit", (RTS_UINTPTR)systasksetexit, 1, RTSITF_GET_SIGNATURE(0, 0x137693C1), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systasksetexit
	#define EXT_systasksetexit
	#define GET_systasksetexit(fl)  CAL_CMGETAPI( "systasksetexit" ) 
	#define CAL_systasksetexit  systasksetexit
	#define CHK_systasksetexit  TRUE
	#define EXP_systasksetexit  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetexit", (RTS_UINTPTR)systasksetexit, 1, RTSITF_GET_SIGNATURE(0, 0x137693C1), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systasksetexit  PFSYSTASKSETEXIT_IEC pfsystasksetexit;
	#define EXT_systasksetexit  extern PFSYSTASKSETEXIT_IEC pfsystasksetexit;
	#define GET_systasksetexit(fl)  s_pfCMGetAPI2( "systasksetexit", (RTS_VOID_FCTPTR *)&pfsystasksetexit, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x137693C1), 0x03051100)
	#define CAL_systasksetexit  pfsystasksetexit
	#define CHK_systasksetexit  (pfsystasksetexit != NULL)
	#define EXP_systasksetexit   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetexit", (RTS_UINTPTR)systasksetexit, 1, RTSITF_GET_SIGNATURE(0, 0x137693C1), 0x03051100) 
#endif


/**
 * <description>
 *  Set the actual interval of a cyclic task. If the specified task is no cyclic task, the function return an error.
 *  This interface can be used to synchronize a task to another task or to events.
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystasksetinterval_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to the task</param> */
    RTS_IEC_UDINT ulInterval;           /* VAR_INPUT */ /* <param name="ulInterval" type="IN">New interval of the task in microseconds!</param> */
    RTS_IEC_RESULT SysTaskSetInterval;  /* VAR_OUTPUT */    
} systasksetinterval_struct;

void CDECL CDECL_EXT systasksetinterval(systasksetinterval_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKSETINTERVAL_IEC) (systasksetinterval_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKSETINTERVAL_NOTIMPLEMENTED)
	#define USE_systasksetinterval
	#define EXT_systasksetinterval
	#define GET_systasksetinterval(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systasksetinterval(p0) 
	#define CHK_systasksetinterval  FALSE
	#define EXP_systasksetinterval  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systasksetinterval
	#define EXT_systasksetinterval
	#define GET_systasksetinterval(fl)  CAL_CMGETAPI( "systasksetinterval" ) 
	#define CAL_systasksetinterval  systasksetinterval
	#define CHK_systasksetinterval  TRUE
	#define EXP_systasksetinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetinterval", (RTS_UINTPTR)systasksetinterval, 1, RTSITF_GET_SIGNATURE(0, 0xB855D3DE), 0x03051100) 
	#define EXTREF_ITF_systasksetinterval {(RTS_VOID_FCTPTR)systasksetinterval, "systasksetinterval", RTSITF_GET_SIGNATURE(0, 0xB855D3DE), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systasksetinterval
	#define EXT_systasksetinterval
	#define GET_systasksetinterval(fl)  CAL_CMGETAPI( "systasksetinterval" ) 
	#define CAL_systasksetinterval  systasksetinterval
	#define CHK_systasksetinterval  TRUE
	#define EXP_systasksetinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetinterval", (RTS_UINTPTR)systasksetinterval, 1, RTSITF_GET_SIGNATURE(0, 0xB855D3DE), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystasksetinterval
	#define EXT_SysTasksystasksetinterval
	#define GET_SysTasksystasksetinterval  ERR_OK
	#define CAL_SysTasksystasksetinterval  systasksetinterval
	#define CHK_SysTasksystasksetinterval  TRUE
	#define EXP_SysTasksystasksetinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetinterval", (RTS_UINTPTR)systasksetinterval, 1, RTSITF_GET_SIGNATURE(0, 0xB855D3DE), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systasksetinterval
	#define EXT_systasksetinterval
	#define GET_systasksetinterval(fl)  CAL_CMGETAPI( "systasksetinterval" ) 
	#define CAL_systasksetinterval  systasksetinterval
	#define CHK_systasksetinterval  TRUE
	#define EXP_systasksetinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetinterval", (RTS_UINTPTR)systasksetinterval, 1, RTSITF_GET_SIGNATURE(0, 0xB855D3DE), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systasksetinterval  PFSYSTASKSETINTERVAL_IEC pfsystasksetinterval;
	#define EXT_systasksetinterval  extern PFSYSTASKSETINTERVAL_IEC pfsystasksetinterval;
	#define GET_systasksetinterval(fl)  s_pfCMGetAPI2( "systasksetinterval", (RTS_VOID_FCTPTR *)&pfsystasksetinterval, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0xB855D3DE), 0x03051100)
	#define CAL_systasksetinterval  pfsystasksetinterval
	#define CHK_systasksetinterval  (pfsystasksetinterval != NULL)
	#define EXP_systasksetinterval   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetinterval", (RTS_UINTPTR)systasksetinterval, 1, RTSITF_GET_SIGNATURE(0, 0xB855D3DE), 0x03051100) 
#endif


/**
 * <description>
 *  Set the priority of the given task. 
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystasksetpriority_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_UDINT ulPriority;           /* VAR_INPUT */ /* <param name="ulPriority" type="IN">Task priority to set. Is the virtual priority between 0(highest)..255(lowest) and will be mapped to OS priority</param> */
    RTS_IEC_RESULT SysTaskSetPriority;  /* VAR_OUTPUT */    
} systasksetpriority_struct;

void CDECL CDECL_EXT systasksetpriority(systasksetpriority_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKSETPRIORITY_IEC) (systasksetpriority_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKSETPRIORITY_NOTIMPLEMENTED)
	#define USE_systasksetpriority
	#define EXT_systasksetpriority
	#define GET_systasksetpriority(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systasksetpriority(p0) 
	#define CHK_systasksetpriority  FALSE
	#define EXP_systasksetpriority  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systasksetpriority
	#define EXT_systasksetpriority
	#define GET_systasksetpriority(fl)  CAL_CMGETAPI( "systasksetpriority" ) 
	#define CAL_systasksetpriority  systasksetpriority
	#define CHK_systasksetpriority  TRUE
	#define EXP_systasksetpriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetpriority", (RTS_UINTPTR)systasksetpriority, 1, RTSITF_GET_SIGNATURE(0, 0x421AD898), 0x03051100) 
	#define EXTREF_ITF_systasksetpriority {(RTS_VOID_FCTPTR)systasksetpriority, "systasksetpriority", RTSITF_GET_SIGNATURE(0, 0x421AD898), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systasksetpriority
	#define EXT_systasksetpriority
	#define GET_systasksetpriority(fl)  CAL_CMGETAPI( "systasksetpriority" ) 
	#define CAL_systasksetpriority  systasksetpriority
	#define CHK_systasksetpriority  TRUE
	#define EXP_systasksetpriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetpriority", (RTS_UINTPTR)systasksetpriority, 1, RTSITF_GET_SIGNATURE(0, 0x421AD898), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystasksetpriority
	#define EXT_SysTasksystasksetpriority
	#define GET_SysTasksystasksetpriority  ERR_OK
	#define CAL_SysTasksystasksetpriority  systasksetpriority
	#define CHK_SysTasksystasksetpriority  TRUE
	#define EXP_SysTasksystasksetpriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetpriority", (RTS_UINTPTR)systasksetpriority, 1, RTSITF_GET_SIGNATURE(0, 0x421AD898), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systasksetpriority
	#define EXT_systasksetpriority
	#define GET_systasksetpriority(fl)  CAL_CMGETAPI( "systasksetpriority" ) 
	#define CAL_systasksetpriority  systasksetpriority
	#define CHK_systasksetpriority  TRUE
	#define EXP_systasksetpriority  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetpriority", (RTS_UINTPTR)systasksetpriority, 1, RTSITF_GET_SIGNATURE(0, 0x421AD898), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systasksetpriority  PFSYSTASKSETPRIORITY_IEC pfsystasksetpriority;
	#define EXT_systasksetpriority  extern PFSYSTASKSETPRIORITY_IEC pfsystasksetpriority;
	#define GET_systasksetpriority(fl)  s_pfCMGetAPI2( "systasksetpriority", (RTS_VOID_FCTPTR *)&pfsystasksetpriority, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x421AD898), 0x03051100)
	#define CAL_systasksetpriority  pfsystasksetpriority
	#define CHK_systasksetpriority  (pfsystasksetpriority != NULL)
	#define EXP_systasksetpriority   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksetpriority", (RTS_UINTPTR)systasksetpriority, 1, RTSITF_GET_SIGNATURE(0, 0x421AD898), 0x03051100) 
#endif


/**
 * <description>
 *  Is called to suspend the given task. 
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystasksuspend_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_RESULT SysTaskSuspend;      /* VAR_OUTPUT */    
} systasksuspend_struct;

void CDECL CDECL_EXT systasksuspend(systasksuspend_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKSUSPEND_IEC) (systasksuspend_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKSUSPEND_NOTIMPLEMENTED)
	#define USE_systasksuspend
	#define EXT_systasksuspend
	#define GET_systasksuspend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systasksuspend(p0) 
	#define CHK_systasksuspend  FALSE
	#define EXP_systasksuspend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systasksuspend
	#define EXT_systasksuspend
	#define GET_systasksuspend(fl)  CAL_CMGETAPI( "systasksuspend" ) 
	#define CAL_systasksuspend  systasksuspend
	#define CHK_systasksuspend  TRUE
	#define EXP_systasksuspend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksuspend", (RTS_UINTPTR)systasksuspend, 1, RTSITF_GET_SIGNATURE(0, 0x44E9CD9B), 0x03051100) 
	#define EXTREF_ITF_systasksuspend {(RTS_VOID_FCTPTR)systasksuspend, "systasksuspend", RTSITF_GET_SIGNATURE(0, 0x44E9CD9B), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systasksuspend
	#define EXT_systasksuspend
	#define GET_systasksuspend(fl)  CAL_CMGETAPI( "systasksuspend" ) 
	#define CAL_systasksuspend  systasksuspend
	#define CHK_systasksuspend  TRUE
	#define EXP_systasksuspend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksuspend", (RTS_UINTPTR)systasksuspend, 1, RTSITF_GET_SIGNATURE(0, 0x44E9CD9B), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystasksuspend
	#define EXT_SysTasksystasksuspend
	#define GET_SysTasksystasksuspend  ERR_OK
	#define CAL_SysTasksystasksuspend  systasksuspend
	#define CHK_SysTasksystasksuspend  TRUE
	#define EXP_SysTasksystasksuspend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksuspend", (RTS_UINTPTR)systasksuspend, 1, RTSITF_GET_SIGNATURE(0, 0x44E9CD9B), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systasksuspend
	#define EXT_systasksuspend
	#define GET_systasksuspend(fl)  CAL_CMGETAPI( "systasksuspend" ) 
	#define CAL_systasksuspend  systasksuspend
	#define CHK_systasksuspend  TRUE
	#define EXP_systasksuspend  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksuspend", (RTS_UINTPTR)systasksuspend, 1, RTSITF_GET_SIGNATURE(0, 0x44E9CD9B), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systasksuspend  PFSYSTASKSUSPEND_IEC pfsystasksuspend;
	#define EXT_systasksuspend  extern PFSYSTASKSUSPEND_IEC pfsystasksuspend;
	#define GET_systasksuspend(fl)  s_pfCMGetAPI2( "systasksuspend", (RTS_VOID_FCTPTR *)&pfsystasksuspend, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x44E9CD9B), 0x03051100)
	#define CAL_systasksuspend  pfsystasksuspend
	#define CHK_systasksuspend  (pfsystasksuspend != NULL)
	#define EXP_systasksuspend   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systasksuspend", (RTS_UINTPTR)systasksuspend, 1, RTSITF_GET_SIGNATURE(0, 0x44E9CD9B), 0x03051100) 
#endif


/**
 * <description>
 *  Wait to the next interval to be activated, if OS supports cyclic task.
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskwaitinterval_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_RESULT SysTaskWaitInterval; /* VAR_OUTPUT */    
} systaskwaitinterval_struct;

void CDECL CDECL_EXT systaskwaitinterval(systaskwaitinterval_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKWAITINTERVAL_IEC) (systaskwaitinterval_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKWAITINTERVAL_NOTIMPLEMENTED)
	#define USE_systaskwaitinterval
	#define EXT_systaskwaitinterval
	#define GET_systaskwaitinterval(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskwaitinterval(p0) 
	#define CHK_systaskwaitinterval  FALSE
	#define EXP_systaskwaitinterval  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskwaitinterval
	#define EXT_systaskwaitinterval
	#define GET_systaskwaitinterval(fl)  CAL_CMGETAPI( "systaskwaitinterval" ) 
	#define CAL_systaskwaitinterval  systaskwaitinterval
	#define CHK_systaskwaitinterval  TRUE
	#define EXP_systaskwaitinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitinterval", (RTS_UINTPTR)systaskwaitinterval, 1, RTSITF_GET_SIGNATURE(0, 0x4FA22EBF), 0x03051100) 
	#define EXTREF_ITF_systaskwaitinterval {(RTS_VOID_FCTPTR)systaskwaitinterval, "systaskwaitinterval", RTSITF_GET_SIGNATURE(0, 0x4FA22EBF), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskwaitinterval
	#define EXT_systaskwaitinterval
	#define GET_systaskwaitinterval(fl)  CAL_CMGETAPI( "systaskwaitinterval" ) 
	#define CAL_systaskwaitinterval  systaskwaitinterval
	#define CHK_systaskwaitinterval  TRUE
	#define EXP_systaskwaitinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitinterval", (RTS_UINTPTR)systaskwaitinterval, 1, RTSITF_GET_SIGNATURE(0, 0x4FA22EBF), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskwaitinterval
	#define EXT_SysTasksystaskwaitinterval
	#define GET_SysTasksystaskwaitinterval  ERR_OK
	#define CAL_SysTasksystaskwaitinterval  systaskwaitinterval
	#define CHK_SysTasksystaskwaitinterval  TRUE
	#define EXP_SysTasksystaskwaitinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitinterval", (RTS_UINTPTR)systaskwaitinterval, 1, RTSITF_GET_SIGNATURE(0, 0x4FA22EBF), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskwaitinterval
	#define EXT_systaskwaitinterval
	#define GET_systaskwaitinterval(fl)  CAL_CMGETAPI( "systaskwaitinterval" ) 
	#define CAL_systaskwaitinterval  systaskwaitinterval
	#define CHK_systaskwaitinterval  TRUE
	#define EXP_systaskwaitinterval  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitinterval", (RTS_UINTPTR)systaskwaitinterval, 1, RTSITF_GET_SIGNATURE(0, 0x4FA22EBF), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskwaitinterval  PFSYSTASKWAITINTERVAL_IEC pfsystaskwaitinterval;
	#define EXT_systaskwaitinterval  extern PFSYSTASKWAITINTERVAL_IEC pfsystaskwaitinterval;
	#define GET_systaskwaitinterval(fl)  s_pfCMGetAPI2( "systaskwaitinterval", (RTS_VOID_FCTPTR *)&pfsystaskwaitinterval, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x4FA22EBF), 0x03051100)
	#define CAL_systaskwaitinterval  pfsystaskwaitinterval
	#define CHK_systaskwaitinterval  (pfsystaskwaitinterval != NULL)
	#define EXP_systaskwaitinterval   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitinterval", (RTS_UINTPTR)systaskwaitinterval, 1, RTSITF_GET_SIGNATURE(0, 0x4FA22EBF), 0x03051100) 
#endif


/**
 * <description>S
 * This function serves to pause the processing of a running task for a time interval specified in milliseconds.
 * Processing will be continued after the time interval elapsed. 
 * </description>
 * <result><p>RESULT: Returns the runtime system error code (see CmpErrors.library).</p></result>
 */
typedef struct tagsystaskwaitsleep_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_UDINT ulMilliSeconds;       /* VAR_INPUT */ /* <param name="ulMilliSeconds" type="IN">Time in milliseconds to sleep</param> */
    RTS_IEC_RESULT SysTaskWaitSleep;    /* VAR_OUTPUT */    
} systaskwaitsleep_struct;

void CDECL CDECL_EXT systaskwaitsleep(systaskwaitsleep_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKWAITSLEEP_IEC) (systaskwaitsleep_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKWAITSLEEP_NOTIMPLEMENTED)
	#define USE_systaskwaitsleep
	#define EXT_systaskwaitsleep
	#define GET_systaskwaitsleep(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskwaitsleep(p0) 
	#define CHK_systaskwaitsleep  FALSE
	#define EXP_systaskwaitsleep  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskwaitsleep
	#define EXT_systaskwaitsleep
	#define GET_systaskwaitsleep(fl)  CAL_CMGETAPI( "systaskwaitsleep" ) 
	#define CAL_systaskwaitsleep  systaskwaitsleep
	#define CHK_systaskwaitsleep  TRUE
	#define EXP_systaskwaitsleep  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitsleep", (RTS_UINTPTR)systaskwaitsleep, 1, RTSITF_GET_SIGNATURE(0, 0x1A10CA63), 0x03051100) 
	#define EXTREF_ITF_systaskwaitsleep {(RTS_VOID_FCTPTR)systaskwaitsleep, "systaskwaitsleep", RTSITF_GET_SIGNATURE(0, 0x1A10CA63), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskwaitsleep
	#define EXT_systaskwaitsleep
	#define GET_systaskwaitsleep(fl)  CAL_CMGETAPI( "systaskwaitsleep" ) 
	#define CAL_systaskwaitsleep  systaskwaitsleep
	#define CHK_systaskwaitsleep  TRUE
	#define EXP_systaskwaitsleep  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitsleep", (RTS_UINTPTR)systaskwaitsleep, 1, RTSITF_GET_SIGNATURE(0, 0x1A10CA63), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskwaitsleep
	#define EXT_SysTasksystaskwaitsleep
	#define GET_SysTasksystaskwaitsleep  ERR_OK
	#define CAL_SysTasksystaskwaitsleep  systaskwaitsleep
	#define CHK_SysTasksystaskwaitsleep  TRUE
	#define EXP_SysTasksystaskwaitsleep  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitsleep", (RTS_UINTPTR)systaskwaitsleep, 1, RTSITF_GET_SIGNATURE(0, 0x1A10CA63), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskwaitsleep
	#define EXT_systaskwaitsleep
	#define GET_systaskwaitsleep(fl)  CAL_CMGETAPI( "systaskwaitsleep" ) 
	#define CAL_systaskwaitsleep  systaskwaitsleep
	#define CHK_systaskwaitsleep  TRUE
	#define EXP_systaskwaitsleep  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitsleep", (RTS_UINTPTR)systaskwaitsleep, 1, RTSITF_GET_SIGNATURE(0, 0x1A10CA63), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskwaitsleep  PFSYSTASKWAITSLEEP_IEC pfsystaskwaitsleep;
	#define EXT_systaskwaitsleep  extern PFSYSTASKWAITSLEEP_IEC pfsystaskwaitsleep;
	#define GET_systaskwaitsleep(fl)  s_pfCMGetAPI2( "systaskwaitsleep", (RTS_VOID_FCTPTR *)&pfsystaskwaitsleep, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x1A10CA63), 0x03051100)
	#define CAL_systaskwaitsleep  pfsystaskwaitsleep
	#define CHK_systaskwaitsleep  (pfsystaskwaitsleep != NULL)
	#define EXP_systaskwaitsleep   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitsleep", (RTS_UINTPTR)systaskwaitsleep, 1, RTSITF_GET_SIGNATURE(0, 0x1A10CA63), 0x03051100) 
#endif


/**
 * <description>
 * This function serves to pause the processing of a running task for a time interval specified in microseconds.
 * The function is not implemented on every platform. 
 * </description>
 * <result>error code</result>
 */
typedef struct tagsystaskwaitsleepus_struct
{
    RTS_IEC_HANDLE hTask;               /* VAR_INPUT */ /* <param name="hTask" type="IN">Handle to task</param> */
    RTS_IEC_ULINT *ptSleepUs;           /* VAR_IN_OUT */    /* <param name="ptSleepUs" type="IN">Time in microseconds to sleep</param> */
    RTS_IEC_RESULT SysTaskWaitSleepUs;  /* VAR_OUTPUT */    
} systaskwaitsleepus_struct;

void CDECL CDECL_EXT systaskwaitsleepus(systaskwaitsleepus_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKWAITSLEEPUS_IEC) (systaskwaitsleepus_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKWAITSLEEPUS_NOTIMPLEMENTED)
	#define USE_systaskwaitsleepus
	#define EXT_systaskwaitsleepus
	#define GET_systaskwaitsleepus(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskwaitsleepus(p0) 
	#define CHK_systaskwaitsleepus  FALSE
	#define EXP_systaskwaitsleepus  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskwaitsleepus
	#define EXT_systaskwaitsleepus
	#define GET_systaskwaitsleepus(fl)  CAL_CMGETAPI( "systaskwaitsleepus" ) 
	#define CAL_systaskwaitsleepus  systaskwaitsleepus
	#define CHK_systaskwaitsleepus  TRUE
	#define EXP_systaskwaitsleepus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitsleepus", (RTS_UINTPTR)systaskwaitsleepus, 1, RTSITF_GET_SIGNATURE(0, 0x3D82CA22), 0x03051100) 
	#define EXTREF_ITF_systaskwaitsleepus {(RTS_VOID_FCTPTR)systaskwaitsleepus, "systaskwaitsleepus", RTSITF_GET_SIGNATURE(0, 0x3D82CA22), 0x03051100}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskwaitsleepus
	#define EXT_systaskwaitsleepus
	#define GET_systaskwaitsleepus(fl)  CAL_CMGETAPI( "systaskwaitsleepus" ) 
	#define CAL_systaskwaitsleepus  systaskwaitsleepus
	#define CHK_systaskwaitsleepus  TRUE
	#define EXP_systaskwaitsleepus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitsleepus", (RTS_UINTPTR)systaskwaitsleepus, 1, RTSITF_GET_SIGNATURE(0, 0x3D82CA22), 0x03051100) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskwaitsleepus
	#define EXT_SysTasksystaskwaitsleepus
	#define GET_SysTasksystaskwaitsleepus  ERR_OK
	#define CAL_SysTasksystaskwaitsleepus  systaskwaitsleepus
	#define CHK_SysTasksystaskwaitsleepus  TRUE
	#define EXP_SysTasksystaskwaitsleepus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitsleepus", (RTS_UINTPTR)systaskwaitsleepus, 1, RTSITF_GET_SIGNATURE(0, 0x3D82CA22), 0x03051100) 
#elif defined(CPLUSPLUS)
	#define USE_systaskwaitsleepus
	#define EXT_systaskwaitsleepus
	#define GET_systaskwaitsleepus(fl)  CAL_CMGETAPI( "systaskwaitsleepus" ) 
	#define CAL_systaskwaitsleepus  systaskwaitsleepus
	#define CHK_systaskwaitsleepus  TRUE
	#define EXP_systaskwaitsleepus  s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitsleepus", (RTS_UINTPTR)systaskwaitsleepus, 1, RTSITF_GET_SIGNATURE(0, 0x3D82CA22), 0x03051100) 
#else /* DYNAMIC_LINK */
	#define USE_systaskwaitsleepus  PFSYSTASKWAITSLEEPUS_IEC pfsystaskwaitsleepus;
	#define EXT_systaskwaitsleepus  extern PFSYSTASKWAITSLEEPUS_IEC pfsystaskwaitsleepus;
	#define GET_systaskwaitsleepus(fl)  s_pfCMGetAPI2( "systaskwaitsleepus", (RTS_VOID_FCTPTR *)&pfsystaskwaitsleepus, (fl) | CM_IMPORT_EXTERNAL_LIB_FUNCTION, RTSITF_GET_SIGNATURE(0, 0x3D82CA22), 0x03051100)
	#define CAL_systaskwaitsleepus  pfsystaskwaitsleepus
	#define CHK_systaskwaitsleepus  (pfsystaskwaitsleepus != NULL)
	#define EXP_systaskwaitsleepus   s_pfCMRegisterAPI2( (const CMP_EXT_FUNCTION_REF*)"systaskwaitsleepus", (RTS_UINTPTR)systaskwaitsleepus, 1, RTSITF_GET_SIGNATURE(0, 0x3D82CA22), 0x03051100) 
#endif


#ifdef __cplusplus
}
#endif

/** EXTERN LIB SECTION END **/


#ifdef __cplusplus
extern "C" {
#endif

/* Initialization routines for OS specific modules */
RTS_RESULT CDECL SysTaskOSInit(INIT_STRUCT *pInit);
RTS_RESULT CDECL SysTaskOSHookFunction(RTS_UI32 ulHook, RTS_UINTPTR ulParam1, RTS_UINTPTR ulParam2);

/* Functions to be optionally implemented and registered by the OS specific implementation of SysTask. */
/* Get the OS task handle from pTask->uiOSHandle. Only needed, if pTask->uiOSHandle is not the OS task handle itself. */
RTS_HANDLE CDECL SysTaskOSGetOSHandleFromTaskInfo(SYS_TASK_INFO *pTask);
typedef RTS_HANDLE (CDECL *PFSYSTASKOSGETOSHANDLEFROMTASKINFO)(SYS_TASK_INFO *pTask);

/* Helper functions to register optional OS specific functions of SysTask. */
/* Register SysTaskOSGetOSHandleFromTaskInfo */ 
RTS_RESULT CDECL SysTaskRegisterGetOSHandleFromTaskInfo(PFSYSTASKOSGETOSHANDLEFROMTASKINFO pfSysTaskOSGetOSHandleFromTaskInfo);


/**
 * <description>Get the first task</description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="pResult" type="ERR_OK">Task found</errorcode>
 * <errorcode name="pResult" type="ERR_NO_OBJECT">No more task available</errorcode>
 * <errorcode name="pResult" type="ERR_PARAMETER">Invalid parameters</errorcode>
 * <errorcode name="pResult" type="ERR_NOTINITIALIZED">The task list is not initialized</errorcode>
 * <result>Handle to the task or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL SysTaskGetFirst(RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSTASKGETFIRST) (RTS_RESULT *pResult);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETFIRST_NOTIMPLEMENTED)
	#define USE_SysTaskGetFirst
	#define EXT_SysTaskGetFirst
	#define GET_SysTaskGetFirst(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetFirst(p0)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysTaskGetFirst  FALSE
	#define EXP_SysTaskGetFirst  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetFirst
	#define EXT_SysTaskGetFirst
	#define GET_SysTaskGetFirst(fl)  CAL_CMGETAPI( "SysTaskGetFirst" ) 
	#define CAL_SysTaskGetFirst  SysTaskGetFirst
	#define CHK_SysTaskGetFirst  TRUE
	#define EXP_SysTaskGetFirst  CAL_CMEXPAPI( "SysTaskGetFirst" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetFirst
	#define EXT_SysTaskGetFirst
	#define GET_SysTaskGetFirst(fl)  CAL_CMGETAPI( "SysTaskGetFirst" ) 
	#define CAL_SysTaskGetFirst  SysTaskGetFirst
	#define CHK_SysTaskGetFirst  TRUE
	#define EXP_SysTaskGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetFirst", (RTS_UINTPTR)SysTaskGetFirst, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetFirst
	#define EXT_SysTaskSysTaskGetFirst
	#define GET_SysTaskSysTaskGetFirst  ERR_OK
	#define CAL_SysTaskSysTaskGetFirst pISysTask->ISysTaskGetFirst
	#define CHK_SysTaskSysTaskGetFirst (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetFirst  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetFirst
	#define EXT_SysTaskGetFirst
	#define GET_SysTaskGetFirst(fl)  CAL_CMGETAPI( "SysTaskGetFirst" ) 
	#define CAL_SysTaskGetFirst pISysTask->ISysTaskGetFirst
	#define CHK_SysTaskGetFirst (pISysTask != NULL)
	#define EXP_SysTaskGetFirst  CAL_CMEXPAPI( "SysTaskGetFirst" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetFirst  PFSYSTASKGETFIRST pfSysTaskGetFirst;
	#define EXT_SysTaskGetFirst  extern PFSYSTASKGETFIRST pfSysTaskGetFirst;
	#define GET_SysTaskGetFirst(fl)  s_pfCMGetAPI2( "SysTaskGetFirst", (RTS_VOID_FCTPTR *)&pfSysTaskGetFirst, (fl), 0, 0)
	#define CAL_SysTaskGetFirst  pfSysTaskGetFirst
	#define CHK_SysTaskGetFirst  (pfSysTaskGetFirst != NULL)
	#define EXP_SysTaskGetFirst  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetFirst", (RTS_UINTPTR)SysTaskGetFirst, 0, 0) 
#endif




/**
 * <description>Get the next task</description>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="pResult" type="ERR_OK">Task found</errorcode>
 * <errorcode name="pResult" type="ERR_NO_OBJECT">No more task available</errorcode>
 * <errorcode name="pResult" type="ERR_PARAMETER">Invalid parameters</errorcode>
 * <errorcode name="pResult" type="ERR_NOTINITIALIZED">The task list is not initialized</errorcode>
 * <result>Handle to the task or RTS_INVALID_HANDLE if failed</result>
 */
RTS_HANDLE CDECL SysTaskGetNext(RTS_HANDLE hPrevTask, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSTASKGETNEXT) (RTS_HANDLE hPrevTask, RTS_RESULT *pResult);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETNEXT_NOTIMPLEMENTED)
	#define USE_SysTaskGetNext
	#define EXT_SysTaskGetNext
	#define GET_SysTaskGetNext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetNext(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysTaskGetNext  FALSE
	#define EXP_SysTaskGetNext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetNext
	#define EXT_SysTaskGetNext
	#define GET_SysTaskGetNext(fl)  CAL_CMGETAPI( "SysTaskGetNext" ) 
	#define CAL_SysTaskGetNext  SysTaskGetNext
	#define CHK_SysTaskGetNext  TRUE
	#define EXP_SysTaskGetNext  CAL_CMEXPAPI( "SysTaskGetNext" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetNext
	#define EXT_SysTaskGetNext
	#define GET_SysTaskGetNext(fl)  CAL_CMGETAPI( "SysTaskGetNext" ) 
	#define CAL_SysTaskGetNext  SysTaskGetNext
	#define CHK_SysTaskGetNext  TRUE
	#define EXP_SysTaskGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetNext", (RTS_UINTPTR)SysTaskGetNext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetNext
	#define EXT_SysTaskSysTaskGetNext
	#define GET_SysTaskSysTaskGetNext  ERR_OK
	#define CAL_SysTaskSysTaskGetNext pISysTask->ISysTaskGetNext
	#define CHK_SysTaskSysTaskGetNext (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetNext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetNext
	#define EXT_SysTaskGetNext
	#define GET_SysTaskGetNext(fl)  CAL_CMGETAPI( "SysTaskGetNext" ) 
	#define CAL_SysTaskGetNext pISysTask->ISysTaskGetNext
	#define CHK_SysTaskGetNext (pISysTask != NULL)
	#define EXP_SysTaskGetNext  CAL_CMEXPAPI( "SysTaskGetNext" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetNext  PFSYSTASKGETNEXT pfSysTaskGetNext;
	#define EXT_SysTaskGetNext  extern PFSYSTASKGETNEXT pfSysTaskGetNext;
	#define GET_SysTaskGetNext(fl)  s_pfCMGetAPI2( "SysTaskGetNext", (RTS_VOID_FCTPTR *)&pfSysTaskGetNext, (fl), 0, 0)
	#define CAL_SysTaskGetNext  pfSysTaskGetNext
	#define CHK_SysTaskGetNext  (pfSysTaskGetNext != NULL)
	#define EXP_SysTaskGetNext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetNext", (RTS_UINTPTR)SysTaskGetNext, 0, 0) 
#endif




/**
 * <description>
 *  Is called to create a task in suspended mode.
 *  IMPLEMENTATION NOTE:
 *  The task must be created _suspended_! After creating a task, the task must be resumed manually to run the task.
 *  If the operating system does not support creating tasks in suspended mode, a task frame must be setup
 *  in the SysTaskOS implementation, where the first call does a SysTaskSuspend() on itself.
 * </description>
 * <param name="pszTaskName" type="IN">The name of the task</param>
 * <param name="pFunction" type="IN">Function which implements the task</param>
 * <param name="pParam" type="IN">Pointer to the argument which is passed on entry</param>
 * <param name="ulPriority" type="IN">Priority of the task</param>
 * <param name="ulInterval" type="IN">Interval in microseconds</param>
 * <param name="ulStackSize" type="IN">Stack size of task in bytes. 0=Default</param>
 * <param name="pExceptionHandler" type="IN">Function pointer to exception handler that is called after an exception has occurred in the task</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <result>Handle to the created task</result>
 */
RTS_HANDLE CDECL SysTaskCreate(char* pszTaskName, PFSYS_TASK_FUNCTION pFunction, void *pParam, RTS_UI32 ulPriority, RTS_UI32 ulInterval, RTS_UI32 ulStackSize, PFTASKEXCEPTIONHANDLER pExceptionHandler, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSTASKCREATE) (char* pszTaskName, PFSYS_TASK_FUNCTION pFunction, void *pParam, RTS_UI32 ulPriority, RTS_UI32 ulInterval, RTS_UI32 ulStackSize, PFTASKEXCEPTIONHANDLER pExceptionHandler, RTS_RESULT *pResult);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKCREATE_NOTIMPLEMENTED)
	#define USE_SysTaskCreate
	#define EXT_SysTaskCreate
	#define GET_SysTaskCreate(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskCreate(p0,p1,p2,p3,p4,p5,p6,p7)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysTaskCreate  FALSE
	#define EXP_SysTaskCreate  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskCreate
	#define EXT_SysTaskCreate
	#define GET_SysTaskCreate(fl)  CAL_CMGETAPI( "SysTaskCreate" ) 
	#define CAL_SysTaskCreate  SysTaskCreate
	#define CHK_SysTaskCreate  TRUE
	#define EXP_SysTaskCreate  CAL_CMEXPAPI( "SysTaskCreate" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskCreate
	#define EXT_SysTaskCreate
	#define GET_SysTaskCreate(fl)  CAL_CMGETAPI( "SysTaskCreate" ) 
	#define CAL_SysTaskCreate  SysTaskCreate
	#define CHK_SysTaskCreate  TRUE
	#define EXP_SysTaskCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskCreate", (RTS_UINTPTR)SysTaskCreate, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskCreate
	#define EXT_SysTaskSysTaskCreate
	#define GET_SysTaskSysTaskCreate  ERR_OK
	#define CAL_SysTaskSysTaskCreate  ((ISysTask*)s_pfCMCreateInstance(CLASSID_CSysTask, NULL))->ISysTaskCreate
	#define CHK_SysTaskSysTaskCreate	(s_pfCMCreateInstance != NULL && pISysTask != NULL)
	#define EXP_SysTaskSysTaskCreate  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskCreate
	#define EXT_SysTaskCreate
	#define GET_SysTaskCreate(fl)  CAL_CMGETAPI( "SysTaskCreate" ) 
	#define CAL_SysTaskCreate  ((ISysTask*)s_pfCMCreateInstance(CLASSID_CSysTask, NULL))->ISysTaskCreate
	#define CHK_SysTaskCreate	(s_pfCMCreateInstance != NULL && pISysTask != NULL)
	#define EXP_SysTaskCreate  CAL_CMEXPAPI( "SysTaskCreate" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskCreate  PFSYSTASKCREATE pfSysTaskCreate;
	#define EXT_SysTaskCreate  extern PFSYSTASKCREATE pfSysTaskCreate;
	#define GET_SysTaskCreate(fl)  s_pfCMGetAPI2( "SysTaskCreate", (RTS_VOID_FCTPTR *)&pfSysTaskCreate, (fl), 0, 0)
	#define CAL_SysTaskCreate  pfSysTaskCreate
	#define CHK_SysTaskCreate  (pfSysTaskCreate != NULL)
	#define EXP_SysTaskCreate  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskCreate", (RTS_UINTPTR)SysTaskCreate, 0, 0) 
#endif




/**
 * <description>
 *  This call extends SysTaskCreate (s.a.) with the name of the task group in order to add this task to the group.
 * </description>
 * <param name="pszTaskGroup" type="IN">Name of the task group</param>
 */
RTS_HANDLE CDECL SysTaskCreate2(char* pszTaskName, char *pszTaskGroup, PFSYS_TASK_FUNCTION pFunction, void *pParam, RTS_UI32 ulPriority, RTS_UI32 ulInterval, RTS_UI32 ulStackSize, PFTASKEXCEPTIONHANDLER pExceptionHandler, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSTASKCREATE2) (char* pszTaskName, char *pszTaskGroup, PFSYS_TASK_FUNCTION pFunction, void *pParam, RTS_UI32 ulPriority, RTS_UI32 ulInterval, RTS_UI32 ulStackSize, PFTASKEXCEPTIONHANDLER pExceptionHandler, RTS_RESULT *pResult);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKCREATE2_NOTIMPLEMENTED)
	#define USE_SysTaskCreate2
	#define EXT_SysTaskCreate2
	#define GET_SysTaskCreate2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskCreate2(p0,p1,p2,p3,p4,p5,p6,p7,p8)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysTaskCreate2  FALSE
	#define EXP_SysTaskCreate2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskCreate2
	#define EXT_SysTaskCreate2
	#define GET_SysTaskCreate2(fl)  CAL_CMGETAPI( "SysTaskCreate2" ) 
	#define CAL_SysTaskCreate2  SysTaskCreate2
	#define CHK_SysTaskCreate2  TRUE
	#define EXP_SysTaskCreate2  CAL_CMEXPAPI( "SysTaskCreate2" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskCreate2
	#define EXT_SysTaskCreate2
	#define GET_SysTaskCreate2(fl)  CAL_CMGETAPI( "SysTaskCreate2" ) 
	#define CAL_SysTaskCreate2  SysTaskCreate2
	#define CHK_SysTaskCreate2  TRUE
	#define EXP_SysTaskCreate2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskCreate2", (RTS_UINTPTR)SysTaskCreate2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskCreate2
	#define EXT_SysTaskSysTaskCreate2
	#define GET_SysTaskSysTaskCreate2  ERR_OK
	#define CAL_SysTaskSysTaskCreate2 pISysTask->ISysTaskCreate2
	#define CHK_SysTaskSysTaskCreate2 (pISysTask != NULL)
	#define EXP_SysTaskSysTaskCreate2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskCreate2
	#define EXT_SysTaskCreate2
	#define GET_SysTaskCreate2(fl)  CAL_CMGETAPI( "SysTaskCreate2" ) 
	#define CAL_SysTaskCreate2 pISysTask->ISysTaskCreate2
	#define CHK_SysTaskCreate2 (pISysTask != NULL)
	#define EXP_SysTaskCreate2  CAL_CMEXPAPI( "SysTaskCreate2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskCreate2  PFSYSTASKCREATE2 pfSysTaskCreate2;
	#define EXT_SysTaskCreate2  extern PFSYSTASKCREATE2 pfSysTaskCreate2;
	#define GET_SysTaskCreate2(fl)  s_pfCMGetAPI2( "SysTaskCreate2", (RTS_VOID_FCTPTR *)&pfSysTaskCreate2, (fl), 0, 0)
	#define CAL_SysTaskCreate2  pfSysTaskCreate2
	#define CHK_SysTaskCreate2  (pfSysTaskCreate2 != NULL)
	#define EXP_SysTaskCreate2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskCreate2", (RTS_UINTPTR)SysTaskCreate2, 0, 0) 
#endif




/**
 * <description>
 *  Tries exit the specified task gracefully. If the task doesn't come to its end within the timeout, then the task will be killed hard!
 * IMPLEMENTATION NOTE:
 * Use TaskTryExit(RTS_HANDLE hTask, RTS_UI32 ulTimeoutMs) first to handle the standard way to release a task gracefully.
 * If this function returned ERR_FAILED, the task doesn't comes to its end within the timeout. In this case, the task must be killed hard!
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <param name="ulTimeoutMs" type="IN">Timeout in milliseconds. Defined values RTS_TIMEOUT_INFINITE, RTS_TIMEOUT_DEFAULT and RTS_TIMEOUT_NO_WAIT can be used here.</param>
 * <result>Error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">Task exit gracefully</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">Handle to task is invalid</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">Task does not exit and could not be deleted hard! After this, only a reboot of
 *                                               the controller can heal this situation!</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_TIMEOUT">Task does not exit within the timeout, but could be deleted hard. This is only a hint for the caller, but no serious error.</errorcode>
 */
RTS_RESULT CDECL SysTaskExit(RTS_HANDLE hTask, RTS_UI32 ulTimeoutMs);
typedef RTS_RESULT (CDECL * PFSYSTASKEXIT) (RTS_HANDLE hTask, RTS_UI32 ulTimeoutMs);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKEXIT_NOTIMPLEMENTED)
	#define USE_SysTaskExit
	#define EXT_SysTaskExit
	#define GET_SysTaskExit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskExit(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskExit  FALSE
	#define EXP_SysTaskExit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskExit
	#define EXT_SysTaskExit
	#define GET_SysTaskExit(fl)  CAL_CMGETAPI( "SysTaskExit" ) 
	#define CAL_SysTaskExit  SysTaskExit
	#define CHK_SysTaskExit  TRUE
	#define EXP_SysTaskExit  CAL_CMEXPAPI( "SysTaskExit" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskExit
	#define EXT_SysTaskExit
	#define GET_SysTaskExit(fl)  CAL_CMGETAPI( "SysTaskExit" ) 
	#define CAL_SysTaskExit  SysTaskExit
	#define CHK_SysTaskExit  TRUE
	#define EXP_SysTaskExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskExit", (RTS_UINTPTR)SysTaskExit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskExit
	#define EXT_SysTaskSysTaskExit
	#define GET_SysTaskSysTaskExit  ERR_OK
	#define CAL_SysTaskSysTaskExit(p0,p1) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysTask*)p0)->ISysTaskExit(p1))
	#define CHK_SysTaskSysTaskExit  TRUE
	#define EXP_SysTaskSysTaskExit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskExit
	#define EXT_SysTaskExit
	#define GET_SysTaskExit(fl)  CAL_CMGETAPI( "SysTaskExit" ) 
	#define CAL_SysTaskExit(p0,p1) (((RTS_HANDLE)p0 == NULL || (RTS_HANDLE)p0 == RTS_INVALID_HANDLE) ? ERR_PARAMETER : ((ISysTask*)p0)->ISysTaskExit(p1))
	#define CHK_SysTaskExit  TRUE
	#define EXP_SysTaskExit  CAL_CMEXPAPI( "SysTaskExit" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskExit  PFSYSTASKEXIT pfSysTaskExit;
	#define EXT_SysTaskExit  extern PFSYSTASKEXIT pfSysTaskExit;
	#define GET_SysTaskExit(fl)  s_pfCMGetAPI2( "SysTaskExit", (RTS_VOID_FCTPTR *)&pfSysTaskExit, (fl), 0, 0)
	#define CAL_SysTaskExit  pfSysTaskExit
	#define CHK_SysTaskExit  (pfSysTaskExit != NULL)
	#define EXP_SysTaskExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskExit", (RTS_UINTPTR)SysTaskExit, 0, 0) 
#endif




/**
 * <description>
 *  Set the exit flag of the specified task. On the next cycle, the task will exit.
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskSetExit(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKSETEXIT) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKSETEXIT_NOTIMPLEMENTED)
	#define USE_SysTaskSetExit
	#define EXT_SysTaskSetExit
	#define GET_SysTaskSetExit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskSetExit(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskSetExit  FALSE
	#define EXP_SysTaskSetExit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskSetExit
	#define EXT_SysTaskSetExit
	#define GET_SysTaskSetExit(fl)  CAL_CMGETAPI( "SysTaskSetExit" ) 
	#define CAL_SysTaskSetExit  SysTaskSetExit
	#define CHK_SysTaskSetExit  TRUE
	#define EXP_SysTaskSetExit  CAL_CMEXPAPI( "SysTaskSetExit" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskSetExit
	#define EXT_SysTaskSetExit
	#define GET_SysTaskSetExit(fl)  CAL_CMGETAPI( "SysTaskSetExit" ) 
	#define CAL_SysTaskSetExit  SysTaskSetExit
	#define CHK_SysTaskSetExit  TRUE
	#define EXP_SysTaskSetExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskSetExit", (RTS_UINTPTR)SysTaskSetExit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskSetExit
	#define EXT_SysTaskSysTaskSetExit
	#define GET_SysTaskSysTaskSetExit  ERR_OK
	#define CAL_SysTaskSysTaskSetExit(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskSetExit() : ((ISysTask*)p0)->ISysTaskSetExit())
	#define CHK_SysTaskSysTaskSetExit  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskSetExit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskSetExit
	#define EXT_SysTaskSetExit
	#define GET_SysTaskSetExit(fl)  CAL_CMGETAPI( "SysTaskSetExit" ) 
	#define CAL_SysTaskSetExit(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskSetExit() : ((ISysTask*)p0)->ISysTaskSetExit())
	#define CHK_SysTaskSetExit  (pISysTask != NULL)
	#define EXP_SysTaskSetExit  CAL_CMEXPAPI( "SysTaskSetExit" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskSetExit  PFSYSTASKSETEXIT pfSysTaskSetExit;
	#define EXT_SysTaskSetExit  extern PFSYSTASKSETEXIT pfSysTaskSetExit;
	#define GET_SysTaskSetExit(fl)  s_pfCMGetAPI2( "SysTaskSetExit", (RTS_VOID_FCTPTR *)&pfSysTaskSetExit, (fl), 0, 0)
	#define CAL_SysTaskSetExit  pfSysTaskSetExit
	#define CHK_SysTaskSetExit  (pfSysTaskSetExit != NULL)
	#define EXP_SysTaskSetExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskSetExit", (RTS_UINTPTR)SysTaskSetExit, 0, 0) 
#endif




/**
 * <description>
 *  The creator of a task can call this function to release the task object, if the task ends is execution. So the task is responsible itself to delete this object.
 *  NOTE: The task object must not be used from outside the task after calling this function!
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskAutoReleaseOnExit(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKAUTORELEASEONEXIT) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKAUTORELEASEONEXIT_NOTIMPLEMENTED)
	#define USE_SysTaskAutoReleaseOnExit
	#define EXT_SysTaskAutoReleaseOnExit
	#define GET_SysTaskAutoReleaseOnExit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskAutoReleaseOnExit(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskAutoReleaseOnExit  FALSE
	#define EXP_SysTaskAutoReleaseOnExit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskAutoReleaseOnExit
	#define EXT_SysTaskAutoReleaseOnExit
	#define GET_SysTaskAutoReleaseOnExit(fl)  CAL_CMGETAPI( "SysTaskAutoReleaseOnExit" ) 
	#define CAL_SysTaskAutoReleaseOnExit  SysTaskAutoReleaseOnExit
	#define CHK_SysTaskAutoReleaseOnExit  TRUE
	#define EXP_SysTaskAutoReleaseOnExit  CAL_CMEXPAPI( "SysTaskAutoReleaseOnExit" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskAutoReleaseOnExit
	#define EXT_SysTaskAutoReleaseOnExit
	#define GET_SysTaskAutoReleaseOnExit(fl)  CAL_CMGETAPI( "SysTaskAutoReleaseOnExit" ) 
	#define CAL_SysTaskAutoReleaseOnExit  SysTaskAutoReleaseOnExit
	#define CHK_SysTaskAutoReleaseOnExit  TRUE
	#define EXP_SysTaskAutoReleaseOnExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskAutoReleaseOnExit", (RTS_UINTPTR)SysTaskAutoReleaseOnExit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskAutoReleaseOnExit
	#define EXT_SysTaskSysTaskAutoReleaseOnExit
	#define GET_SysTaskSysTaskAutoReleaseOnExit  ERR_OK
	#define CAL_SysTaskSysTaskAutoReleaseOnExit(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskAutoReleaseOnExit() : ((ISysTask*)p0)->ISysTaskAutoReleaseOnExit())
	#define CHK_SysTaskSysTaskAutoReleaseOnExit  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskAutoReleaseOnExit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskAutoReleaseOnExit
	#define EXT_SysTaskAutoReleaseOnExit
	#define GET_SysTaskAutoReleaseOnExit(fl)  CAL_CMGETAPI( "SysTaskAutoReleaseOnExit" ) 
	#define CAL_SysTaskAutoReleaseOnExit(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskAutoReleaseOnExit() : ((ISysTask*)p0)->ISysTaskAutoReleaseOnExit())
	#define CHK_SysTaskAutoReleaseOnExit  (pISysTask != NULL)
	#define EXP_SysTaskAutoReleaseOnExit  CAL_CMEXPAPI( "SysTaskAutoReleaseOnExit" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskAutoReleaseOnExit  PFSYSTASKAUTORELEASEONEXIT pfSysTaskAutoReleaseOnExit;
	#define EXT_SysTaskAutoReleaseOnExit  extern PFSYSTASKAUTORELEASEONEXIT pfSysTaskAutoReleaseOnExit;
	#define GET_SysTaskAutoReleaseOnExit(fl)  s_pfCMGetAPI2( "SysTaskAutoReleaseOnExit", (RTS_VOID_FCTPTR *)&pfSysTaskAutoReleaseOnExit, (fl), 0, 0)
	#define CAL_SysTaskAutoReleaseOnExit  pfSysTaskAutoReleaseOnExit
	#define CHK_SysTaskAutoReleaseOnExit  (pfSysTaskAutoReleaseOnExit != NULL)
	#define EXP_SysTaskAutoReleaseOnExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskAutoReleaseOnExit", (RTS_UINTPTR)SysTaskAutoReleaseOnExit, 0, 0) 
#endif




/**
 * <description>
 *  Remove the flag set by SysTaskAutoReleaseOnExit().
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskRemoveAutoReleaseOnExit(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKREMOVEAUTORELEASEONEXIT) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKREMOVEAUTORELEASEONEXIT_NOTIMPLEMENTED)
	#define USE_SysTaskRemoveAutoReleaseOnExit
	#define EXT_SysTaskRemoveAutoReleaseOnExit
	#define GET_SysTaskRemoveAutoReleaseOnExit(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskRemoveAutoReleaseOnExit(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskRemoveAutoReleaseOnExit  FALSE
	#define EXP_SysTaskRemoveAutoReleaseOnExit  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskRemoveAutoReleaseOnExit
	#define EXT_SysTaskRemoveAutoReleaseOnExit
	#define GET_SysTaskRemoveAutoReleaseOnExit(fl)  CAL_CMGETAPI( "SysTaskRemoveAutoReleaseOnExit" ) 
	#define CAL_SysTaskRemoveAutoReleaseOnExit  SysTaskRemoveAutoReleaseOnExit
	#define CHK_SysTaskRemoveAutoReleaseOnExit  TRUE
	#define EXP_SysTaskRemoveAutoReleaseOnExit  CAL_CMEXPAPI( "SysTaskRemoveAutoReleaseOnExit" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskRemoveAutoReleaseOnExit
	#define EXT_SysTaskRemoveAutoReleaseOnExit
	#define GET_SysTaskRemoveAutoReleaseOnExit(fl)  CAL_CMGETAPI( "SysTaskRemoveAutoReleaseOnExit" ) 
	#define CAL_SysTaskRemoveAutoReleaseOnExit  SysTaskRemoveAutoReleaseOnExit
	#define CHK_SysTaskRemoveAutoReleaseOnExit  TRUE
	#define EXP_SysTaskRemoveAutoReleaseOnExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskRemoveAutoReleaseOnExit", (RTS_UINTPTR)SysTaskRemoveAutoReleaseOnExit, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskRemoveAutoReleaseOnExit
	#define EXT_SysTaskSysTaskRemoveAutoReleaseOnExit
	#define GET_SysTaskSysTaskRemoveAutoReleaseOnExit  ERR_OK
	#define CAL_SysTaskSysTaskRemoveAutoReleaseOnExit(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskRemoveAutoReleaseOnExit() : ((ISysTask*)p0)->ISysTaskRemoveAutoReleaseOnExit())
	#define CHK_SysTaskSysTaskRemoveAutoReleaseOnExit  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskRemoveAutoReleaseOnExit  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskRemoveAutoReleaseOnExit
	#define EXT_SysTaskRemoveAutoReleaseOnExit
	#define GET_SysTaskRemoveAutoReleaseOnExit(fl)  CAL_CMGETAPI( "SysTaskRemoveAutoReleaseOnExit" ) 
	#define CAL_SysTaskRemoveAutoReleaseOnExit(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskRemoveAutoReleaseOnExit() : ((ISysTask*)p0)->ISysTaskRemoveAutoReleaseOnExit())
	#define CHK_SysTaskRemoveAutoReleaseOnExit  (pISysTask != NULL)
	#define EXP_SysTaskRemoveAutoReleaseOnExit  CAL_CMEXPAPI( "SysTaskRemoveAutoReleaseOnExit" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskRemoveAutoReleaseOnExit  PFSYSTASKREMOVEAUTORELEASEONEXIT pfSysTaskRemoveAutoReleaseOnExit;
	#define EXT_SysTaskRemoveAutoReleaseOnExit  extern PFSYSTASKREMOVEAUTORELEASEONEXIT pfSysTaskRemoveAutoReleaseOnExit;
	#define GET_SysTaskRemoveAutoReleaseOnExit(fl)  s_pfCMGetAPI2( "SysTaskRemoveAutoReleaseOnExit", (RTS_VOID_FCTPTR *)&pfSysTaskRemoveAutoReleaseOnExit, (fl), 0, 0)
	#define CAL_SysTaskRemoveAutoReleaseOnExit  pfSysTaskRemoveAutoReleaseOnExit
	#define CHK_SysTaskRemoveAutoReleaseOnExit  (pfSysTaskRemoveAutoReleaseOnExit != NULL)
	#define EXP_SysTaskRemoveAutoReleaseOnExit  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskRemoveAutoReleaseOnExit", (RTS_UINTPTR)SysTaskRemoveAutoReleaseOnExit, 0, 0) 
#endif




/**
 * <description>
 *  Is called to destroy the given task. 
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskDestroy(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKDESTROY) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKDESTROY_NOTIMPLEMENTED)
	#define USE_SysTaskDestroy
	#define EXT_SysTaskDestroy
	#define GET_SysTaskDestroy(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskDestroy(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskDestroy  FALSE
	#define EXP_SysTaskDestroy  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskDestroy
	#define EXT_SysTaskDestroy
	#define GET_SysTaskDestroy(fl)  CAL_CMGETAPI( "SysTaskDestroy" ) 
	#define CAL_SysTaskDestroy  SysTaskDestroy
	#define CHK_SysTaskDestroy  TRUE
	#define EXP_SysTaskDestroy  CAL_CMEXPAPI( "SysTaskDestroy" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskDestroy
	#define EXT_SysTaskDestroy
	#define GET_SysTaskDestroy(fl)  CAL_CMGETAPI( "SysTaskDestroy" ) 
	#define CAL_SysTaskDestroy  SysTaskDestroy
	#define CHK_SysTaskDestroy  TRUE
	#define EXP_SysTaskDestroy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskDestroy", (RTS_UINTPTR)SysTaskDestroy, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskDestroy
	#define EXT_SysTaskSysTaskDestroy
	#define GET_SysTaskSysTaskDestroy  ERR_OK
	#define CAL_SysTaskSysTaskDestroy(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskDestroy() : ((ISysTask*)p0)->ISysTaskDestroy())
	#define CHK_SysTaskSysTaskDestroy  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskDestroy  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskDestroy
	#define EXT_SysTaskDestroy
	#define GET_SysTaskDestroy(fl)  CAL_CMGETAPI( "SysTaskDestroy" ) 
	#define CAL_SysTaskDestroy(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskDestroy() : ((ISysTask*)p0)->ISysTaskDestroy())
	#define CHK_SysTaskDestroy  (pISysTask != NULL)
	#define EXP_SysTaskDestroy  CAL_CMEXPAPI( "SysTaskDestroy" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskDestroy  PFSYSTASKDESTROY pfSysTaskDestroy;
	#define EXT_SysTaskDestroy  extern PFSYSTASKDESTROY pfSysTaskDestroy;
	#define GET_SysTaskDestroy(fl)  s_pfCMGetAPI2( "SysTaskDestroy", (RTS_VOID_FCTPTR *)&pfSysTaskDestroy, (fl), 0, 0)
	#define CAL_SysTaskDestroy  pfSysTaskDestroy
	#define CHK_SysTaskDestroy  (pfSysTaskDestroy != NULL)
	#define EXP_SysTaskDestroy  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskDestroy", (RTS_UINTPTR)SysTaskDestroy, 0, 0) 
#endif




/**
 * <description>
 *  Is called to suspend the given task. 
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskSuspend(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKSUSPEND) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKSUSPEND_NOTIMPLEMENTED)
	#define USE_SysTaskSuspend
	#define EXT_SysTaskSuspend
	#define GET_SysTaskSuspend(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskSuspend(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskSuspend  FALSE
	#define EXP_SysTaskSuspend  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskSuspend
	#define EXT_SysTaskSuspend
	#define GET_SysTaskSuspend(fl)  CAL_CMGETAPI( "SysTaskSuspend" ) 
	#define CAL_SysTaskSuspend  SysTaskSuspend
	#define CHK_SysTaskSuspend  TRUE
	#define EXP_SysTaskSuspend  CAL_CMEXPAPI( "SysTaskSuspend" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskSuspend
	#define EXT_SysTaskSuspend
	#define GET_SysTaskSuspend(fl)  CAL_CMGETAPI( "SysTaskSuspend" ) 
	#define CAL_SysTaskSuspend  SysTaskSuspend
	#define CHK_SysTaskSuspend  TRUE
	#define EXP_SysTaskSuspend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskSuspend", (RTS_UINTPTR)SysTaskSuspend, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskSuspend
	#define EXT_SysTaskSysTaskSuspend
	#define GET_SysTaskSysTaskSuspend  ERR_OK
	#define CAL_SysTaskSysTaskSuspend(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskSuspend() : ((ISysTask*)p0)->ISysTaskSuspend())
	#define CHK_SysTaskSysTaskSuspend  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskSuspend  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskSuspend
	#define EXT_SysTaskSuspend
	#define GET_SysTaskSuspend(fl)  CAL_CMGETAPI( "SysTaskSuspend" ) 
	#define CAL_SysTaskSuspend(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskSuspend() : ((ISysTask*)p0)->ISysTaskSuspend())
	#define CHK_SysTaskSuspend  (pISysTask != NULL)
	#define EXP_SysTaskSuspend  CAL_CMEXPAPI( "SysTaskSuspend" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskSuspend  PFSYSTASKSUSPEND pfSysTaskSuspend;
	#define EXT_SysTaskSuspend  extern PFSYSTASKSUSPEND pfSysTaskSuspend;
	#define GET_SysTaskSuspend(fl)  s_pfCMGetAPI2( "SysTaskSuspend", (RTS_VOID_FCTPTR *)&pfSysTaskSuspend, (fl), 0, 0)
	#define CAL_SysTaskSuspend  pfSysTaskSuspend
	#define CHK_SysTaskSuspend  (pfSysTaskSuspend != NULL)
	#define EXP_SysTaskSuspend  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskSuspend", (RTS_UINTPTR)SysTaskSuspend, 0, 0) 
#endif




/**
 * <description>
 *  Is called to resume the given task. 
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskResume(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKRESUME) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKRESUME_NOTIMPLEMENTED)
	#define USE_SysTaskResume
	#define EXT_SysTaskResume
	#define GET_SysTaskResume(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskResume(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskResume  FALSE
	#define EXP_SysTaskResume  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskResume
	#define EXT_SysTaskResume
	#define GET_SysTaskResume(fl)  CAL_CMGETAPI( "SysTaskResume" ) 
	#define CAL_SysTaskResume  SysTaskResume
	#define CHK_SysTaskResume  TRUE
	#define EXP_SysTaskResume  CAL_CMEXPAPI( "SysTaskResume" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskResume
	#define EXT_SysTaskResume
	#define GET_SysTaskResume(fl)  CAL_CMGETAPI( "SysTaskResume" ) 
	#define CAL_SysTaskResume  SysTaskResume
	#define CHK_SysTaskResume  TRUE
	#define EXP_SysTaskResume  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskResume", (RTS_UINTPTR)SysTaskResume, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskResume
	#define EXT_SysTaskSysTaskResume
	#define GET_SysTaskSysTaskResume  ERR_OK
	#define CAL_SysTaskSysTaskResume(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskResume() : ((ISysTask*)p0)->ISysTaskResume())
	#define CHK_SysTaskSysTaskResume  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskResume  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskResume
	#define EXT_SysTaskResume
	#define GET_SysTaskResume(fl)  CAL_CMGETAPI( "SysTaskResume" ) 
	#define CAL_SysTaskResume(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskResume() : ((ISysTask*)p0)->ISysTaskResume())
	#define CHK_SysTaskResume  (pISysTask != NULL)
	#define EXP_SysTaskResume  CAL_CMEXPAPI( "SysTaskResume" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskResume  PFSYSTASKRESUME pfSysTaskResume;
	#define EXT_SysTaskResume  extern PFSYSTASKRESUME pfSysTaskResume;
	#define GET_SysTaskResume(fl)  s_pfCMGetAPI2( "SysTaskResume", (RTS_VOID_FCTPTR *)&pfSysTaskResume, (fl), 0, 0)
	#define CAL_SysTaskResume  pfSysTaskResume
	#define CHK_SysTaskResume  (pfSysTaskResume != NULL)
	#define EXP_SysTaskResume  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskResume", (RTS_UINTPTR)SysTaskResume, 0, 0) 
#endif




/**
 * <description>
 *  Is called from the task, that end its execution. 
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <param name="ulExitCode" type="IN">Exit code</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskEnd(RTS_HANDLE hTask, RTS_UINTPTR ulExitCode);
typedef RTS_RESULT (CDECL * PFSYSTASKEND) (RTS_HANDLE hTask, RTS_UINTPTR ulExitCode);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKEND_NOTIMPLEMENTED)
	#define USE_SysTaskEnd
	#define EXT_SysTaskEnd
	#define GET_SysTaskEnd(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskEnd(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskEnd  FALSE
	#define EXP_SysTaskEnd  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskEnd
	#define EXT_SysTaskEnd
	#define GET_SysTaskEnd(fl)  CAL_CMGETAPI( "SysTaskEnd" ) 
	#define CAL_SysTaskEnd  SysTaskEnd
	#define CHK_SysTaskEnd  TRUE
	#define EXP_SysTaskEnd  CAL_CMEXPAPI( "SysTaskEnd" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskEnd
	#define EXT_SysTaskEnd
	#define GET_SysTaskEnd(fl)  CAL_CMGETAPI( "SysTaskEnd" ) 
	#define CAL_SysTaskEnd  SysTaskEnd
	#define CHK_SysTaskEnd  TRUE
	#define EXP_SysTaskEnd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskEnd", (RTS_UINTPTR)SysTaskEnd, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskEnd
	#define EXT_SysTaskSysTaskEnd
	#define GET_SysTaskSysTaskEnd  ERR_OK
	#define CAL_SysTaskSysTaskEnd(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskEnd(p1) : ((ISysTask*)p0)->ISysTaskEnd(p1))
	#define CHK_SysTaskSysTaskEnd  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskEnd  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskEnd
	#define EXT_SysTaskEnd
	#define GET_SysTaskEnd(fl)  CAL_CMGETAPI( "SysTaskEnd" ) 
	#define CAL_SysTaskEnd(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskEnd(p1) : ((ISysTask*)p0)->ISysTaskEnd(p1))
	#define CHK_SysTaskEnd  (pISysTask != NULL)
	#define EXP_SysTaskEnd  CAL_CMEXPAPI( "SysTaskEnd" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskEnd  PFSYSTASKEND pfSysTaskEnd;
	#define EXT_SysTaskEnd  extern PFSYSTASKEND pfSysTaskEnd;
	#define GET_SysTaskEnd(fl)  s_pfCMGetAPI2( "SysTaskEnd", (RTS_VOID_FCTPTR *)&pfSysTaskEnd, (fl), 0, 0)
	#define CAL_SysTaskEnd  pfSysTaskEnd
	#define CHK_SysTaskEnd  (pfSysTaskEnd != NULL)
	#define EXP_SysTaskEnd  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskEnd", (RTS_UINTPTR)SysTaskEnd, 0, 0) 
#endif




/**
 * <description>
 *  Prevents execution of the current task and addresses the OS scheduler to resume
 *  execution after the given time (ms). 
 *  IMPLEMENTATION NOTE:
 *  - Don't use hTask in the implementation! If you must have the task handle, get it with SysTaskGetCurrent!
 * </description>
 * <param name="hTask" type="IN">Obsolete: Should be removed in future versions! Handle to task. Can be RTS_INVALID_HANDLE.</param>
 * <param name="ulMilliSeconds" type="IN">Time in milliseconds to sleep</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskWaitSleep(RTS_HANDLE hTask, RTS_UI32 ulMilliSeconds);
typedef RTS_RESULT (CDECL * PFSYSTASKWAITSLEEP) (RTS_HANDLE hTask, RTS_UI32 ulMilliSeconds);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKWAITSLEEP_NOTIMPLEMENTED)
	#define USE_SysTaskWaitSleep
	#define EXT_SysTaskWaitSleep
	#define GET_SysTaskWaitSleep(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskWaitSleep(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskWaitSleep  FALSE
	#define EXP_SysTaskWaitSleep  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskWaitSleep
	#define EXT_SysTaskWaitSleep
	#define GET_SysTaskWaitSleep(fl)  CAL_CMGETAPI( "SysTaskWaitSleep" ) 
	#define CAL_SysTaskWaitSleep  SysTaskWaitSleep
	#define CHK_SysTaskWaitSleep  TRUE
	#define EXP_SysTaskWaitSleep  CAL_CMEXPAPI( "SysTaskWaitSleep" ) 
	#define EXTREF_ITF_SysTaskWaitSleep {(RTS_VOID_FCTPTR)SysTaskWaitSleep, "SysTaskWaitSleep", 0, 0}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskWaitSleep
	#define EXT_SysTaskWaitSleep
	#define GET_SysTaskWaitSleep(fl)  CAL_CMGETAPI( "SysTaskWaitSleep" ) 
	#define CAL_SysTaskWaitSleep  SysTaskWaitSleep
	#define CHK_SysTaskWaitSleep  TRUE
	#define EXP_SysTaskWaitSleep  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskWaitSleep", (RTS_UINTPTR)SysTaskWaitSleep, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskWaitSleep
	#define EXT_SysTaskSysTaskWaitSleep
	#define GET_SysTaskSysTaskWaitSleep  ERR_OK
	#define CAL_SysTaskSysTaskWaitSleep  SysTaskWaitSleep
	#define CHK_SysTaskSysTaskWaitSleep  TRUE
	#define EXP_SysTaskSysTaskWaitSleep  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskWaitSleep
	#define EXT_SysTaskWaitSleep
	#define GET_SysTaskWaitSleep(fl)  CAL_CMGETAPI( "SysTaskWaitSleep" ) 
	#define CAL_SysTaskWaitSleep  SysTaskWaitSleep
	#define CHK_SysTaskWaitSleep  TRUE
	#define EXP_SysTaskWaitSleep  CAL_CMEXPAPI( "SysTaskWaitSleep" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskWaitSleep  PFSYSTASKWAITSLEEP pfSysTaskWaitSleep;
	#define EXT_SysTaskWaitSleep  extern PFSYSTASKWAITSLEEP pfSysTaskWaitSleep;
	#define GET_SysTaskWaitSleep(fl)  s_pfCMGetAPI2( "SysTaskWaitSleep", (RTS_VOID_FCTPTR *)&pfSysTaskWaitSleep, (fl), 0,0)
	#define CAL_SysTaskWaitSleep  pfSysTaskWaitSleep
	#define CHK_SysTaskWaitSleep  (pfSysTaskWaitSleep != NULL)
	#define EXP_SysTaskWaitSleep  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskWaitSleep", (RTS_UINTPTR)SysTaskWaitSleep, 0, 0) 
#endif


/**
 * <description>
 *  Prevents execution of the current task and addresses the OS scheduler to resume
 *  execution after the given time (us). 
 *  IMPLEMENTATION NOTE:
 *  - Don't use hTask in the implementation! If you must have the task handle, get it with SysTaskGetCurrent!
 * </description>
 * <param name="hTask" type="IN">Obsolete: Should be removed in future versions! Handle to task. Can be RTS_INVALID_HANDLE.</param>
 * <param name="ptSleepUs" type="IN">Time in microseconds to sleep</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskWaitSleepUs(RTS_HANDLE hTask, RTS_SYSTIME *ptSleepUs);
typedef RTS_RESULT (CDECL * PFSYSTASKWAITSLEEPUS) (RTS_HANDLE hTask, RTS_SYSTIME *ptSleepUs);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKWAITSLEEPUS_NOTIMPLEMENTED)
	#define USE_SysTaskWaitSleepUs
	#define EXT_SysTaskWaitSleepUs
	#define GET_SysTaskWaitSleepUs(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskWaitSleepUs(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskWaitSleepUs  FALSE
	#define EXP_SysTaskWaitSleepUs  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskWaitSleepUs
	#define EXT_SysTaskWaitSleepUs
	#define GET_SysTaskWaitSleepUs(fl)  CAL_CMGETAPI( "SysTaskWaitSleepUs" ) 
	#define CAL_SysTaskWaitSleepUs  SysTaskWaitSleepUs
	#define CHK_SysTaskWaitSleepUs  TRUE
	#define EXP_SysTaskWaitSleepUs  CAL_CMEXPAPI( "SysTaskWaitSleepUs" ) 
	#define EXTREF_ITF_SysTaskWaitSleepUs {(RTS_VOID_FCTPTR)SysTaskWaitSleepUs, "SysTaskWaitSleepUs", 0, 0}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskWaitSleepUs
	#define EXT_SysTaskWaitSleepUs
	#define GET_SysTaskWaitSleepUs(fl)  CAL_CMGETAPI( "SysTaskWaitSleepUs" ) 
	#define CAL_SysTaskWaitSleepUs  SysTaskWaitSleepUs
	#define CHK_SysTaskWaitSleepUs  TRUE
	#define EXP_SysTaskWaitSleepUs  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskWaitSleepUs", (RTS_UINTPTR)SysTaskWaitSleepUs, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskWaitSleepUs
	#define EXT_SysTaskSysTaskWaitSleepUs
	#define GET_SysTaskSysTaskWaitSleepUs  ERR_OK
	#define CAL_SysTaskSysTaskWaitSleepUs  SysTaskWaitSleepUs
	#define CHK_SysTaskSysTaskWaitSleepUs  TRUE
	#define EXP_SysTaskSysTaskWaitSleepUs  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskWaitSleepUs
	#define EXT_SysTaskWaitSleepUs
	#define GET_SysTaskWaitSleepUs(fl)  CAL_CMGETAPI( "SysTaskWaitSleepUs" ) 
	#define CAL_SysTaskWaitSleepUs  SysTaskWaitSleepUs
	#define CHK_SysTaskWaitSleepUs  TRUE
	#define EXP_SysTaskWaitSleepUs  CAL_CMEXPAPI( "SysTaskWaitSleepUs" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskWaitSleepUs  PFSYSTASKWAITSLEEPUS pfSysTaskWaitSleepUs;
	#define EXT_SysTaskWaitSleepUs  extern PFSYSTASKWAITSLEEPUS pfSysTaskWaitSleepUs;
	#define GET_SysTaskWaitSleepUs(fl)  s_pfCMGetAPI2( "SysTaskWaitSleepUs", (RTS_VOID_FCTPTR *)&pfSysTaskWaitSleepUs, (fl), 0,0)
	#define CAL_SysTaskWaitSleepUs  pfSysTaskWaitSleepUs
	#define CHK_SysTaskWaitSleepUs  (pfSysTaskWaitSleepUs != NULL)
	#define EXP_SysTaskWaitSleepUs  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskWaitSleepUs", (RTS_UINTPTR)SysTaskWaitSleepUs, 0, 0) 
#endif


/**
 * <description>
 *  Wait to the next interval to be activated, if OS supports cyclic task.
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskWaitInterval(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKWAITINTERVAL) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKWAITINTERVAL_NOTIMPLEMENTED)
	#define USE_SysTaskWaitInterval
	#define EXT_SysTaskWaitInterval
	#define GET_SysTaskWaitInterval(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskWaitInterval(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskWaitInterval  FALSE
	#define EXP_SysTaskWaitInterval  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskWaitInterval
	#define EXT_SysTaskWaitInterval
	#define GET_SysTaskWaitInterval(fl)  CAL_CMGETAPI( "SysTaskWaitInterval" ) 
	#define CAL_SysTaskWaitInterval  SysTaskWaitInterval
	#define CHK_SysTaskWaitInterval  TRUE
	#define EXP_SysTaskWaitInterval  CAL_CMEXPAPI( "SysTaskWaitInterval" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskWaitInterval
	#define EXT_SysTaskWaitInterval
	#define GET_SysTaskWaitInterval(fl)  CAL_CMGETAPI( "SysTaskWaitInterval" ) 
	#define CAL_SysTaskWaitInterval  SysTaskWaitInterval
	#define CHK_SysTaskWaitInterval  TRUE
	#define EXP_SysTaskWaitInterval  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskWaitInterval", (RTS_UINTPTR)SysTaskWaitInterval, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskWaitInterval
	#define EXT_SysTaskSysTaskWaitInterval
	#define GET_SysTaskSysTaskWaitInterval  ERR_OK
	#define CAL_SysTaskSysTaskWaitInterval(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskWaitInterval() : ((ISysTask*)p0)->ISysTaskWaitInterval())
	#define CHK_SysTaskSysTaskWaitInterval  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskWaitInterval  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskWaitInterval
	#define EXT_SysTaskWaitInterval
	#define GET_SysTaskWaitInterval(fl)  CAL_CMGETAPI( "SysTaskWaitInterval" ) 
	#define CAL_SysTaskWaitInterval(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskWaitInterval() : ((ISysTask*)p0)->ISysTaskWaitInterval())
	#define CHK_SysTaskWaitInterval  (pISysTask != NULL)
	#define EXP_SysTaskWaitInterval  CAL_CMEXPAPI( "SysTaskWaitInterval" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskWaitInterval  PFSYSTASKWAITINTERVAL pfSysTaskWaitInterval;
	#define EXT_SysTaskWaitInterval  extern PFSYSTASKWAITINTERVAL pfSysTaskWaitInterval;
	#define GET_SysTaskWaitInterval(fl)  s_pfCMGetAPI2( "SysTaskWaitInterval", (RTS_VOID_FCTPTR *)&pfSysTaskWaitInterval, (fl), 0, 0)
	#define CAL_SysTaskWaitInterval  pfSysTaskWaitInterval
	#define CHK_SysTaskWaitInterval  (pfSysTaskWaitInterval != NULL)
	#define EXP_SysTaskWaitInterval  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskWaitInterval", (RTS_UINTPTR)SysTaskWaitInterval, 0, 0) 
#endif




/**
 * <description>
 *  Is used to wait for exit of the specified join task. 
 * </description>
 * <param name="hTaskToJoin" type="IN">Task to join</param>
 * <param name="ulTimeoutMs" type="IN">Timeout in milliseconds to wait for join task</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskJoin(RTS_HANDLE hTaskToJoin, RTS_UI32 ulTimeoutMs);
typedef RTS_RESULT (CDECL * PFSYSTASKJOIN) (RTS_HANDLE hTaskToJoin, RTS_UI32 ulTimeoutMs);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKJOIN_NOTIMPLEMENTED)
	#define USE_SysTaskJoin
	#define EXT_SysTaskJoin
	#define GET_SysTaskJoin(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskJoin(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskJoin  FALSE
	#define EXP_SysTaskJoin  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskJoin
	#define EXT_SysTaskJoin
	#define GET_SysTaskJoin(fl)  CAL_CMGETAPI( "SysTaskJoin" ) 
	#define CAL_SysTaskJoin  SysTaskJoin
	#define CHK_SysTaskJoin  TRUE
	#define EXP_SysTaskJoin  CAL_CMEXPAPI( "SysTaskJoin" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskJoin
	#define EXT_SysTaskJoin
	#define GET_SysTaskJoin(fl)  CAL_CMGETAPI( "SysTaskJoin" ) 
	#define CAL_SysTaskJoin  SysTaskJoin
	#define CHK_SysTaskJoin  TRUE
	#define EXP_SysTaskJoin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskJoin", (RTS_UINTPTR)SysTaskJoin, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskJoin
	#define EXT_SysTaskSysTaskJoin
	#define GET_SysTaskSysTaskJoin  ERR_OK
	#define CAL_SysTaskSysTaskJoin pISysTask->ISysTaskJoin
	#define CHK_SysTaskSysTaskJoin (pISysTask != NULL)
	#define EXP_SysTaskSysTaskJoin  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskJoin
	#define EXT_SysTaskJoin
	#define GET_SysTaskJoin(fl)  CAL_CMGETAPI( "SysTaskJoin" ) 
	#define CAL_SysTaskJoin pISysTask->ISysTaskJoin
	#define CHK_SysTaskJoin (pISysTask != NULL)
	#define EXP_SysTaskJoin  CAL_CMEXPAPI( "SysTaskJoin" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskJoin  PFSYSTASKJOIN pfSysTaskJoin;
	#define EXT_SysTaskJoin  extern PFSYSTASKJOIN pfSysTaskJoin;
	#define GET_SysTaskJoin(fl)  s_pfCMGetAPI2( "SysTaskJoin", (RTS_VOID_FCTPTR *)&pfSysTaskJoin, (fl), 0, 0)
	#define CAL_SysTaskJoin  pfSysTaskJoin
	#define CHK_SysTaskJoin  (pfSysTaskJoin != NULL)
	#define EXP_SysTaskJoin  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskJoin", (RTS_UINTPTR)SysTaskJoin, 0, 0) 
#endif




/**
 * <description>
 *  Returns the task handle of the current running task. 
 * </description>
 * <param name="phTask" type="OUT">Pointer to task handle</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskGetCurrent(RTS_HANDLE *phTask);
typedef RTS_RESULT (CDECL * PFSYSTASKGETCURRENT) (RTS_HANDLE *phTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETCURRENT_NOTIMPLEMENTED)
	#define USE_SysTaskGetCurrent
	#define EXT_SysTaskGetCurrent
	#define GET_SysTaskGetCurrent(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetCurrent(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetCurrent  FALSE
	#define EXP_SysTaskGetCurrent  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetCurrent
	#define EXT_SysTaskGetCurrent
	#define GET_SysTaskGetCurrent(fl)  CAL_CMGETAPI( "SysTaskGetCurrent" ) 
	#define CAL_SysTaskGetCurrent  SysTaskGetCurrent
	#define CHK_SysTaskGetCurrent  TRUE
	#define EXP_SysTaskGetCurrent  CAL_CMEXPAPI( "SysTaskGetCurrent" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetCurrent
	#define EXT_SysTaskGetCurrent
	#define GET_SysTaskGetCurrent(fl)  CAL_CMGETAPI( "SysTaskGetCurrent" ) 
	#define CAL_SysTaskGetCurrent  SysTaskGetCurrent
	#define CHK_SysTaskGetCurrent  TRUE
	#define EXP_SysTaskGetCurrent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetCurrent", (RTS_UINTPTR)SysTaskGetCurrent, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetCurrent
	#define EXT_SysTaskSysTaskGetCurrent
	#define GET_SysTaskSysTaskGetCurrent  ERR_OK
	#define CAL_SysTaskSysTaskGetCurrent pISysTask->ISysTaskGetCurrent
	#define CHK_SysTaskSysTaskGetCurrent (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetCurrent  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetCurrent
	#define EXT_SysTaskGetCurrent
	#define GET_SysTaskGetCurrent(fl)  CAL_CMGETAPI( "SysTaskGetCurrent" ) 
	#define CAL_SysTaskGetCurrent pISysTask->ISysTaskGetCurrent
	#define CHK_SysTaskGetCurrent (pISysTask != NULL)
	#define EXP_SysTaskGetCurrent  CAL_CMEXPAPI( "SysTaskGetCurrent" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetCurrent  PFSYSTASKGETCURRENT pfSysTaskGetCurrent;
	#define EXT_SysTaskGetCurrent  extern PFSYSTASKGETCURRENT pfSysTaskGetCurrent;
	#define GET_SysTaskGetCurrent(fl)  s_pfCMGetAPI2( "SysTaskGetCurrent", (RTS_VOID_FCTPTR *)&pfSysTaskGetCurrent, (fl), 0, 0)
	#define CAL_SysTaskGetCurrent  pfSysTaskGetCurrent
	#define CHK_SysTaskGetCurrent  (pfSysTaskGetCurrent != NULL)
	#define EXP_SysTaskGetCurrent  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetCurrent", (RTS_UINTPTR)SysTaskGetCurrent, 0, 0) 
#endif




/**
 * <description>
 *  Returns the operating system handle of the current running task. 
 * </description>
 * <param name="puiTaskOSHandle" type="OUT">Pointer to operating system task handle</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskGetCurrentOSHandle(RTS_HANDLE *puiTaskOSHandle);
typedef RTS_RESULT (CDECL * PFSYSTASKGETCURRENTOSHANDLE) (RTS_HANDLE *puiTaskOSHandle);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETCURRENTOSHANDLE_NOTIMPLEMENTED)
	#define USE_SysTaskGetCurrentOSHandle
	#define EXT_SysTaskGetCurrentOSHandle
	#define GET_SysTaskGetCurrentOSHandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetCurrentOSHandle(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetCurrentOSHandle  FALSE
	#define EXP_SysTaskGetCurrentOSHandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetCurrentOSHandle
	#define EXT_SysTaskGetCurrentOSHandle
	#define GET_SysTaskGetCurrentOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetCurrentOSHandle" ) 
	#define CAL_SysTaskGetCurrentOSHandle  SysTaskGetCurrentOSHandle
	#define CHK_SysTaskGetCurrentOSHandle  TRUE
	#define EXP_SysTaskGetCurrentOSHandle  CAL_CMEXPAPI( "SysTaskGetCurrentOSHandle" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetCurrentOSHandle
	#define EXT_SysTaskGetCurrentOSHandle
	#define GET_SysTaskGetCurrentOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetCurrentOSHandle" ) 
	#define CAL_SysTaskGetCurrentOSHandle  SysTaskGetCurrentOSHandle
	#define CHK_SysTaskGetCurrentOSHandle  TRUE
	#define EXP_SysTaskGetCurrentOSHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetCurrentOSHandle", (RTS_UINTPTR)SysTaskGetCurrentOSHandle, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetCurrentOSHandle
	#define EXT_SysTaskSysTaskGetCurrentOSHandle
	#define GET_SysTaskSysTaskGetCurrentOSHandle  ERR_OK
	#define CAL_SysTaskSysTaskGetCurrentOSHandle pISysTask->ISysTaskGetCurrentOSHandle
	#define CHK_SysTaskSysTaskGetCurrentOSHandle (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetCurrentOSHandle  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetCurrentOSHandle
	#define EXT_SysTaskGetCurrentOSHandle
	#define GET_SysTaskGetCurrentOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetCurrentOSHandle" ) 
	#define CAL_SysTaskGetCurrentOSHandle pISysTask->ISysTaskGetCurrentOSHandle
	#define CHK_SysTaskGetCurrentOSHandle (pISysTask != NULL)
	#define EXP_SysTaskGetCurrentOSHandle  CAL_CMEXPAPI( "SysTaskGetCurrentOSHandle" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetCurrentOSHandle  PFSYSTASKGETCURRENTOSHANDLE pfSysTaskGetCurrentOSHandle;
	#define EXT_SysTaskGetCurrentOSHandle  extern PFSYSTASKGETCURRENTOSHANDLE pfSysTaskGetCurrentOSHandle;
	#define GET_SysTaskGetCurrentOSHandle(fl)  s_pfCMGetAPI2( "SysTaskGetCurrentOSHandle", (RTS_VOID_FCTPTR *)&pfSysTaskGetCurrentOSHandle, (fl), 0, 0)
	#define CAL_SysTaskGetCurrentOSHandle  pfSysTaskGetCurrentOSHandle
	#define CHK_SysTaskGetCurrentOSHandle  (pfSysTaskGetCurrentOSHandle != NULL)
	#define EXP_SysTaskGetCurrentOSHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetCurrentOSHandle", (RTS_UINTPTR)SysTaskGetCurrentOSHandle, 0, 0) 
#endif




/**
 * <description>
 *  Set the priority of the given task. 
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <param name="ulPriority" type="IN">Task priority to set. Is the virtual priority between 0..255 and will be mapped to OS priority</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskSetPriority(RTS_HANDLE hTask, RTS_UI32 ulPriority);
typedef RTS_RESULT (CDECL * PFSYSTASKSETPRIORITY) (RTS_HANDLE hTask, RTS_UI32 ulPriority);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKSETPRIORITY_NOTIMPLEMENTED)
	#define USE_SysTaskSetPriority
	#define EXT_SysTaskSetPriority
	#define GET_SysTaskSetPriority(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskSetPriority(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskSetPriority  FALSE
	#define EXP_SysTaskSetPriority  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskSetPriority
	#define EXT_SysTaskSetPriority
	#define GET_SysTaskSetPriority(fl)  CAL_CMGETAPI( "SysTaskSetPriority" ) 
	#define CAL_SysTaskSetPriority  SysTaskSetPriority
	#define CHK_SysTaskSetPriority  TRUE
	#define EXP_SysTaskSetPriority  CAL_CMEXPAPI( "SysTaskSetPriority" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskSetPriority
	#define EXT_SysTaskSetPriority
	#define GET_SysTaskSetPriority(fl)  CAL_CMGETAPI( "SysTaskSetPriority" ) 
	#define CAL_SysTaskSetPriority  SysTaskSetPriority
	#define CHK_SysTaskSetPriority  TRUE
	#define EXP_SysTaskSetPriority  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskSetPriority", (RTS_UINTPTR)SysTaskSetPriority, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskSetPriority
	#define EXT_SysTaskSysTaskSetPriority
	#define GET_SysTaskSysTaskSetPriority  ERR_OK
	#define CAL_SysTaskSysTaskSetPriority(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskSetPriority(p1) : ((ISysTask*)p0)->ISysTaskSetPriority(p1))
	#define CHK_SysTaskSysTaskSetPriority  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskSetPriority  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskSetPriority
	#define EXT_SysTaskSetPriority
	#define GET_SysTaskSetPriority(fl)  CAL_CMGETAPI( "SysTaskSetPriority" ) 
	#define CAL_SysTaskSetPriority(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskSetPriority(p1) : ((ISysTask*)p0)->ISysTaskSetPriority(p1))
	#define CHK_SysTaskSetPriority  (pISysTask != NULL)
	#define EXP_SysTaskSetPriority  CAL_CMEXPAPI( "SysTaskSetPriority" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskSetPriority  PFSYSTASKSETPRIORITY pfSysTaskSetPriority;
	#define EXT_SysTaskSetPriority  extern PFSYSTASKSETPRIORITY pfSysTaskSetPriority;
	#define GET_SysTaskSetPriority(fl)  s_pfCMGetAPI2( "SysTaskSetPriority", (RTS_VOID_FCTPTR *)&pfSysTaskSetPriority, (fl), 0, 0)
	#define CAL_SysTaskSetPriority  pfSysTaskSetPriority
	#define CHK_SysTaskSetPriority  (pfSysTaskSetPriority != NULL)
	#define EXP_SysTaskSetPriority  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskSetPriority", (RTS_UINTPTR)SysTaskSetPriority, 0, 0) 
#endif




/**
 * <description>
 *  Get the runtime system priority of the given task. 
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <param name="pulPriority" type="OUT">Pointer to get priority</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskGetPriority(RTS_HANDLE hTask, RTS_UI32 *pulPriority);
typedef RTS_RESULT (CDECL * PFSYSTASKGETPRIORITY) (RTS_HANDLE hTask, RTS_UI32 *pulPriority);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETPRIORITY_NOTIMPLEMENTED)
	#define USE_SysTaskGetPriority
	#define EXT_SysTaskGetPriority
	#define GET_SysTaskGetPriority(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetPriority(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetPriority  FALSE
	#define EXP_SysTaskGetPriority  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetPriority
	#define EXT_SysTaskGetPriority
	#define GET_SysTaskGetPriority(fl)  CAL_CMGETAPI( "SysTaskGetPriority" ) 
	#define CAL_SysTaskGetPriority  SysTaskGetPriority
	#define CHK_SysTaskGetPriority  TRUE
	#define EXP_SysTaskGetPriority  CAL_CMEXPAPI( "SysTaskGetPriority" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetPriority
	#define EXT_SysTaskGetPriority
	#define GET_SysTaskGetPriority(fl)  CAL_CMGETAPI( "SysTaskGetPriority" ) 
	#define CAL_SysTaskGetPriority  SysTaskGetPriority
	#define CHK_SysTaskGetPriority  TRUE
	#define EXP_SysTaskGetPriority  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetPriority", (RTS_UINTPTR)SysTaskGetPriority, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetPriority
	#define EXT_SysTaskSysTaskGetPriority
	#define GET_SysTaskSysTaskGetPriority  ERR_OK
	#define CAL_SysTaskSysTaskGetPriority(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetPriority(p1) : ((ISysTask*)p0)->ISysTaskGetPriority(p1))
	#define CHK_SysTaskSysTaskGetPriority  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetPriority  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetPriority
	#define EXT_SysTaskGetPriority
	#define GET_SysTaskGetPriority(fl)  CAL_CMGETAPI( "SysTaskGetPriority" ) 
	#define CAL_SysTaskGetPriority(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetPriority(p1) : ((ISysTask*)p0)->ISysTaskGetPriority(p1))
	#define CHK_SysTaskGetPriority  (pISysTask != NULL)
	#define EXP_SysTaskGetPriority  CAL_CMEXPAPI( "SysTaskGetPriority" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetPriority  PFSYSTASKGETPRIORITY pfSysTaskGetPriority;
	#define EXT_SysTaskGetPriority  extern PFSYSTASKGETPRIORITY pfSysTaskGetPriority;
	#define GET_SysTaskGetPriority(fl)  s_pfCMGetAPI2( "SysTaskGetPriority", (RTS_VOID_FCTPTR *)&pfSysTaskGetPriority, (fl), 0, 0)
	#define CAL_SysTaskGetPriority  pfSysTaskGetPriority
	#define CHK_SysTaskGetPriority  (pfSysTaskGetPriority != NULL)
	#define EXP_SysTaskGetPriority  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetPriority", (RTS_UINTPTR)SysTaskGetPriority, 0, 0) 
#endif




/**
 * <description>
 *  Returns the operating system priority of the given task.
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <param name="pulOSPriority" type="OUT">Pointer to get operating system priority</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskGetOSPriority(RTS_HANDLE hTask, RTS_UI32 *pulOSPriority);
typedef RTS_RESULT (CDECL * PFSYSTASKGETOSPRIORITY) (RTS_HANDLE hTask, RTS_UI32 *pulOSPriority);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETOSPRIORITY_NOTIMPLEMENTED)
	#define USE_SysTaskGetOSPriority
	#define EXT_SysTaskGetOSPriority
	#define GET_SysTaskGetOSPriority(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetOSPriority(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetOSPriority  FALSE
	#define EXP_SysTaskGetOSPriority  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetOSPriority
	#define EXT_SysTaskGetOSPriority
	#define GET_SysTaskGetOSPriority(fl)  CAL_CMGETAPI( "SysTaskGetOSPriority" ) 
	#define CAL_SysTaskGetOSPriority  SysTaskGetOSPriority
	#define CHK_SysTaskGetOSPriority  TRUE
	#define EXP_SysTaskGetOSPriority  CAL_CMEXPAPI( "SysTaskGetOSPriority" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetOSPriority
	#define EXT_SysTaskGetOSPriority
	#define GET_SysTaskGetOSPriority(fl)  CAL_CMGETAPI( "SysTaskGetOSPriority" ) 
	#define CAL_SysTaskGetOSPriority  SysTaskGetOSPriority
	#define CHK_SysTaskGetOSPriority  TRUE
	#define EXP_SysTaskGetOSPriority  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetOSPriority", (RTS_UINTPTR)SysTaskGetOSPriority, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetOSPriority
	#define EXT_SysTaskSysTaskGetOSPriority
	#define GET_SysTaskSysTaskGetOSPriority  ERR_OK
	#define CAL_SysTaskSysTaskGetOSPriority(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetOSPriority(p1) : ((ISysTask*)p0)->ISysTaskGetOSPriority(p1))
	#define CHK_SysTaskSysTaskGetOSPriority  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetOSPriority  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetOSPriority
	#define EXT_SysTaskGetOSPriority
	#define GET_SysTaskGetOSPriority(fl)  CAL_CMGETAPI( "SysTaskGetOSPriority" ) 
	#define CAL_SysTaskGetOSPriority(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetOSPriority(p1) : ((ISysTask*)p0)->ISysTaskGetOSPriority(p1))
	#define CHK_SysTaskGetOSPriority  (pISysTask != NULL)
	#define EXP_SysTaskGetOSPriority  CAL_CMEXPAPI( "SysTaskGetOSPriority" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetOSPriority  PFSYSTASKGETOSPRIORITY pfSysTaskGetOSPriority;
	#define EXT_SysTaskGetOSPriority  extern PFSYSTASKGETOSPRIORITY pfSysTaskGetOSPriority;
	#define GET_SysTaskGetOSPriority(fl)  s_pfCMGetAPI2( "SysTaskGetOSPriority", (RTS_VOID_FCTPTR *)&pfSysTaskGetOSPriority, (fl), 0, 0)
	#define CAL_SysTaskGetOSPriority  pfSysTaskGetOSPriority
	#define CHK_SysTaskGetOSPriority  (pfSysTaskGetOSPriority != NULL)
	#define EXP_SysTaskGetOSPriority  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetOSPriority", (RTS_UINTPTR)SysTaskGetOSPriority, 0, 0) 
#endif




/**
 * <description>
 *  Returns the task info of the specified task.
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <param name="ppInfo" type="OUT">Pointer to pointer to get task info structure</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskGetInfo(RTS_HANDLE hTask, SYS_TASK_INFO **ppInfo);
typedef RTS_RESULT (CDECL * PFSYSTASKGETINFO) (RTS_HANDLE hTask, SYS_TASK_INFO **ppInfo);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETINFO_NOTIMPLEMENTED)
	#define USE_SysTaskGetInfo
	#define EXT_SysTaskGetInfo
	#define GET_SysTaskGetInfo(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetInfo(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetInfo  FALSE
	#define EXP_SysTaskGetInfo  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetInfo
	#define EXT_SysTaskGetInfo
	#define GET_SysTaskGetInfo(fl)  CAL_CMGETAPI( "SysTaskGetInfo" ) 
	#define CAL_SysTaskGetInfo  SysTaskGetInfo
	#define CHK_SysTaskGetInfo  TRUE
	#define EXP_SysTaskGetInfo  CAL_CMEXPAPI( "SysTaskGetInfo" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetInfo
	#define EXT_SysTaskGetInfo
	#define GET_SysTaskGetInfo(fl)  CAL_CMGETAPI( "SysTaskGetInfo" ) 
	#define CAL_SysTaskGetInfo  SysTaskGetInfo
	#define CHK_SysTaskGetInfo  TRUE
	#define EXP_SysTaskGetInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetInfo", (RTS_UINTPTR)SysTaskGetInfo, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetInfo
	#define EXT_SysTaskSysTaskGetInfo
	#define GET_SysTaskSysTaskGetInfo  ERR_OK
	#define CAL_SysTaskSysTaskGetInfo(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetInfo(p1) : ((ISysTask*)p0)->ISysTaskGetInfo(p1))
	#define CHK_SysTaskSysTaskGetInfo  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetInfo  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetInfo
	#define EXT_SysTaskGetInfo
	#define GET_SysTaskGetInfo(fl)  CAL_CMGETAPI( "SysTaskGetInfo" ) 
	#define CAL_SysTaskGetInfo(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetInfo(p1) : ((ISysTask*)p0)->ISysTaskGetInfo(p1))
	#define CHK_SysTaskGetInfo  (pISysTask != NULL)
	#define EXP_SysTaskGetInfo  CAL_CMEXPAPI( "SysTaskGetInfo" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetInfo  PFSYSTASKGETINFO pfSysTaskGetInfo;
	#define EXT_SysTaskGetInfo  extern PFSYSTASKGETINFO pfSysTaskGetInfo;
	#define GET_SysTaskGetInfo(fl)  s_pfCMGetAPI2( "SysTaskGetInfo", (RTS_VOID_FCTPTR *)&pfSysTaskGetInfo, (fl), 0, 0)
	#define CAL_SysTaskGetInfo  pfSysTaskGetInfo
	#define CHK_SysTaskGetInfo  (pfSysTaskGetInfo != NULL)
	#define EXP_SysTaskGetInfo  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetInfo", (RTS_UINTPTR)SysTaskGetInfo, 0, 0) 
#endif




/**
 * <description>
 *  Returns the task name of the specified task.
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <param name="pResult" type="OUT">Pointer to error code</param>
 * <errorcode name="pResult" type="ERR_OK">Task name found</errorcode>
 * <errorcode name="pResult" type="ERR_PARAMETER">Invalid parameters</errorcode>
 * <errorcode name="pResult" type="ERR_FAILED">Task name not found</errorcode>
 * <result>pointer to task name</result>
 */
char* CDECL SysTaskGetName(RTS_HANDLE hTask, RTS_RESULT *pResult);
typedef char* (CDECL * PFSYSTASKGETNAME) (RTS_HANDLE hTask, RTS_RESULT *pResult);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETNAME_NOTIMPLEMENTED)
	#define USE_SysTaskGetName
	#define EXT_SysTaskGetName
	#define GET_SysTaskGetName(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetName(p0,p1)  (char*)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetName  FALSE
	#define EXP_SysTaskGetName  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetName
	#define EXT_SysTaskGetName
	#define GET_SysTaskGetName(fl)  CAL_CMGETAPI( "SysTaskGetName" ) 
	#define CAL_SysTaskGetName  SysTaskGetName
	#define CHK_SysTaskGetName  TRUE
	#define EXP_SysTaskGetName  CAL_CMEXPAPI( "SysTaskGetName" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetName
	#define EXT_SysTaskGetName
	#define GET_SysTaskGetName(fl)  CAL_CMGETAPI( "SysTaskGetName" ) 
	#define CAL_SysTaskGetName  SysTaskGetName
	#define CHK_SysTaskGetName  TRUE
	#define EXP_SysTaskGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetName", (RTS_UINTPTR)SysTaskGetName, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetName
	#define EXT_SysTaskSysTaskGetName
	#define GET_SysTaskSysTaskGetName  ERR_OK
	#define CAL_SysTaskSysTaskGetName(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetName(p1) : ((ISysTask*)p0)->ISysTaskGetName(p1))
	#define CHK_SysTaskSysTaskGetName  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetName  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetName
	#define EXT_SysTaskGetName
	#define GET_SysTaskGetName(fl)  CAL_CMGETAPI( "SysTaskGetName" ) 
	#define CAL_SysTaskGetName(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetName(p1) : ((ISysTask*)p0)->ISysTaskGetName(p1))
	#define CHK_SysTaskGetName  (pISysTask != NULL)
	#define EXP_SysTaskGetName  CAL_CMEXPAPI( "SysTaskGetName" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetName  PFSYSTASKGETNAME pfSysTaskGetName;
	#define EXT_SysTaskGetName  extern PFSYSTASKGETNAME pfSysTaskGetName;
	#define GET_SysTaskGetName(fl)  s_pfCMGetAPI2( "SysTaskGetName", (RTS_VOID_FCTPTR *)&pfSysTaskGetName, (fl), 0, 0)
	#define CAL_SysTaskGetName  pfSysTaskGetName
	#define CHK_SysTaskGetName  (pfSysTaskGetName != NULL)
	#define EXP_SysTaskGetName  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetName", (RTS_UINTPTR)SysTaskGetName, 0, 0) 
#endif




/**
 * <description>
 *  This function is called to mark entering the while loop.
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskEnter(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKENTER) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKENTER_NOTIMPLEMENTED)
	#define USE_SysTaskEnter
	#define EXT_SysTaskEnter
	#define GET_SysTaskEnter(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskEnter(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskEnter  FALSE
	#define EXP_SysTaskEnter  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskEnter
	#define EXT_SysTaskEnter
	#define GET_SysTaskEnter(fl)  CAL_CMGETAPI( "SysTaskEnter" ) 
	#define CAL_SysTaskEnter  SysTaskEnter
	#define CHK_SysTaskEnter  TRUE
	#define EXP_SysTaskEnter  CAL_CMEXPAPI( "SysTaskEnter" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskEnter
	#define EXT_SysTaskEnter
	#define GET_SysTaskEnter(fl)  CAL_CMGETAPI( "SysTaskEnter" ) 
	#define CAL_SysTaskEnter  SysTaskEnter
	#define CHK_SysTaskEnter  TRUE
	#define EXP_SysTaskEnter  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskEnter", (RTS_UINTPTR)SysTaskEnter, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskEnter
	#define EXT_SysTaskSysTaskEnter
	#define GET_SysTaskSysTaskEnter  ERR_OK
	#define CAL_SysTaskSysTaskEnter(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskEnter() : ((ISysTask*)p0)->ISysTaskEnter())
	#define CHK_SysTaskSysTaskEnter  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskEnter  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskEnter
	#define EXT_SysTaskEnter
	#define GET_SysTaskEnter(fl)  CAL_CMGETAPI( "SysTaskEnter" ) 
	#define CAL_SysTaskEnter(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskEnter() : ((ISysTask*)p0)->ISysTaskEnter())
	#define CHK_SysTaskEnter  (pISysTask != NULL)
	#define EXP_SysTaskEnter  CAL_CMEXPAPI( "SysTaskEnter" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskEnter  PFSYSTASKENTER pfSysTaskEnter;
	#define EXT_SysTaskEnter  extern PFSYSTASKENTER pfSysTaskEnter;
	#define GET_SysTaskEnter(fl)  s_pfCMGetAPI2( "SysTaskEnter", (RTS_VOID_FCTPTR *)&pfSysTaskEnter, (fl), 0, 0)
	#define CAL_SysTaskEnter  pfSysTaskEnter
	#define CHK_SysTaskEnter  (pfSysTaskEnter != NULL)
	#define EXP_SysTaskEnter  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskEnter", (RTS_UINTPTR)SysTaskEnter, 0, 0) 
#endif




/**
 * <description>
 *  This function is called to mark leaving the while loop. 
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskLeave(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKLEAVE) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKLEAVE_NOTIMPLEMENTED)
	#define USE_SysTaskLeave
	#define EXT_SysTaskLeave
	#define GET_SysTaskLeave(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskLeave(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskLeave  FALSE
	#define EXP_SysTaskLeave  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskLeave
	#define EXT_SysTaskLeave
	#define GET_SysTaskLeave(fl)  CAL_CMGETAPI( "SysTaskLeave" ) 
	#define CAL_SysTaskLeave  SysTaskLeave
	#define CHK_SysTaskLeave  TRUE
	#define EXP_SysTaskLeave  CAL_CMEXPAPI( "SysTaskLeave" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskLeave
	#define EXT_SysTaskLeave
	#define GET_SysTaskLeave(fl)  CAL_CMGETAPI( "SysTaskLeave" ) 
	#define CAL_SysTaskLeave  SysTaskLeave
	#define CHK_SysTaskLeave  TRUE
	#define EXP_SysTaskLeave  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskLeave", (RTS_UINTPTR)SysTaskLeave, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskLeave
	#define EXT_SysTaskSysTaskLeave
	#define GET_SysTaskSysTaskLeave  ERR_OK
	#define CAL_SysTaskSysTaskLeave(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskLeave() : ((ISysTask*)p0)->ISysTaskLeave())
	#define CHK_SysTaskSysTaskLeave  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskLeave  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskLeave
	#define EXT_SysTaskLeave
	#define GET_SysTaskLeave(fl)  CAL_CMGETAPI( "SysTaskLeave" ) 
	#define CAL_SysTaskLeave(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskLeave() : ((ISysTask*)p0)->ISysTaskLeave())
	#define CHK_SysTaskLeave  (pISysTask != NULL)
	#define EXP_SysTaskLeave  CAL_CMEXPAPI( "SysTaskLeave" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskLeave  PFSYSTASKLEAVE pfSysTaskLeave;
	#define EXT_SysTaskLeave  extern PFSYSTASKLEAVE pfSysTaskLeave;
	#define GET_SysTaskLeave(fl)  s_pfCMGetAPI2( "SysTaskLeave", (RTS_VOID_FCTPTR *)&pfSysTaskLeave, (fl), 0, 0)
	#define CAL_SysTaskLeave  pfSysTaskLeave
	#define CHK_SysTaskLeave  (pfSysTaskLeave != NULL)
	#define EXP_SysTaskLeave  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskLeave", (RTS_UINTPTR)SysTaskLeave, 0, 0) 
#endif




/**
 * <description>
 *  Get the current register context of the task. Task must be in suspended mode! 
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <param name="pContext" type="OUT">Pointer to context</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskGetContext(RTS_HANDLE hTask, RegContext *pContext);
typedef RTS_RESULT (CDECL * PFSYSTASKGETCONTEXT) (RTS_HANDLE hTask, RegContext *pContext);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETCONTEXT_NOTIMPLEMENTED)
	#define USE_SysTaskGetContext
	#define EXT_SysTaskGetContext
	#define GET_SysTaskGetContext(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetContext(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetContext  FALSE
	#define EXP_SysTaskGetContext  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetContext
	#define EXT_SysTaskGetContext
	#define GET_SysTaskGetContext(fl)  CAL_CMGETAPI( "SysTaskGetContext" ) 
	#define CAL_SysTaskGetContext  SysTaskGetContext
	#define CHK_SysTaskGetContext  TRUE
	#define EXP_SysTaskGetContext  CAL_CMEXPAPI( "SysTaskGetContext" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetContext
	#define EXT_SysTaskGetContext
	#define GET_SysTaskGetContext(fl)  CAL_CMGETAPI( "SysTaskGetContext" ) 
	#define CAL_SysTaskGetContext  SysTaskGetContext
	#define CHK_SysTaskGetContext  TRUE
	#define EXP_SysTaskGetContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetContext", (RTS_UINTPTR)SysTaskGetContext, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetContext
	#define EXT_SysTaskSysTaskGetContext
	#define GET_SysTaskSysTaskGetContext  ERR_OK
	#define CAL_SysTaskSysTaskGetContext(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetContext(p1) : ((ISysTask*)p0)->ISysTaskGetContext(p1))
	#define CHK_SysTaskSysTaskGetContext  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetContext  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetContext
	#define EXT_SysTaskGetContext
	#define GET_SysTaskGetContext(fl)  CAL_CMGETAPI( "SysTaskGetContext" ) 
	#define CAL_SysTaskGetContext(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetContext(p1) : ((ISysTask*)p0)->ISysTaskGetContext(p1))
	#define CHK_SysTaskGetContext  (pISysTask != NULL)
	#define EXP_SysTaskGetContext  CAL_CMEXPAPI( "SysTaskGetContext" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetContext  PFSYSTASKGETCONTEXT pfSysTaskGetContext;
	#define EXT_SysTaskGetContext  extern PFSYSTASKGETCONTEXT pfSysTaskGetContext;
	#define GET_SysTaskGetContext(fl)  s_pfCMGetAPI2( "SysTaskGetContext", (RTS_VOID_FCTPTR *)&pfSysTaskGetContext, (fl), 0, 0)
	#define CAL_SysTaskGetContext  pfSysTaskGetContext
	#define CHK_SysTaskGetContext  (pfSysTaskGetContext != NULL)
	#define EXP_SysTaskGetContext  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetContext", (RTS_UINTPTR)SysTaskGetContext, 0, 0) 
#endif




/**
 * <description>
 *  Call the corresponding exception handler of the task. 
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <param name="ulException" type="IN">Rts standard exception</param>
 * <param name="Context" type="IN">Context to detect the code location where the exception occurred</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskGenerateException(RTS_HANDLE uiTaskOSHandle, RTS_UI32 ulException, RegContext Context);
typedef RTS_RESULT (CDECL * PFSYSTASKGENERATEEXCEPTION) (RTS_HANDLE uiTaskOSHandle, RTS_UI32 ulException, RegContext Context);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGENERATEEXCEPTION_NOTIMPLEMENTED)
	#define USE_SysTaskGenerateException
	#define EXT_SysTaskGenerateException
	#define GET_SysTaskGenerateException(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGenerateException(p0,p1,p2)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGenerateException  FALSE
	#define EXP_SysTaskGenerateException  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGenerateException
	#define EXT_SysTaskGenerateException
	#define GET_SysTaskGenerateException(fl)  CAL_CMGETAPI( "SysTaskGenerateException" ) 
	#define CAL_SysTaskGenerateException  SysTaskGenerateException
	#define CHK_SysTaskGenerateException  TRUE
	#define EXP_SysTaskGenerateException  CAL_CMEXPAPI( "SysTaskGenerateException" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGenerateException
	#define EXT_SysTaskGenerateException
	#define GET_SysTaskGenerateException(fl)  CAL_CMGETAPI( "SysTaskGenerateException" ) 
	#define CAL_SysTaskGenerateException  SysTaskGenerateException
	#define CHK_SysTaskGenerateException  TRUE
	#define EXP_SysTaskGenerateException  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGenerateException", (RTS_UINTPTR)SysTaskGenerateException, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGenerateException
	#define EXT_SysTaskSysTaskGenerateException
	#define GET_SysTaskSysTaskGenerateException  ERR_OK
	#define CAL_SysTaskSysTaskGenerateException pISysTask->ISysTaskGenerateException
	#define CHK_SysTaskSysTaskGenerateException (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGenerateException  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGenerateException
	#define EXT_SysTaskGenerateException
	#define GET_SysTaskGenerateException(fl)  CAL_CMGETAPI( "SysTaskGenerateException" ) 
	#define CAL_SysTaskGenerateException pISysTask->ISysTaskGenerateException
	#define CHK_SysTaskGenerateException (pISysTask != NULL)
	#define EXP_SysTaskGenerateException  CAL_CMEXPAPI( "SysTaskGenerateException" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGenerateException  PFSYSTASKGENERATEEXCEPTION pfSysTaskGenerateException;
	#define EXT_SysTaskGenerateException  extern PFSYSTASKGENERATEEXCEPTION pfSysTaskGenerateException;
	#define GET_SysTaskGenerateException(fl)  s_pfCMGetAPI2( "SysTaskGenerateException", (RTS_VOID_FCTPTR *)&pfSysTaskGenerateException, (fl), 0, 0)
	#define CAL_SysTaskGenerateException  pfSysTaskGenerateException
	#define CHK_SysTaskGenerateException  (pfSysTaskGenerateException != NULL)
	#define EXP_SysTaskGenerateException  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGenerateException", (RTS_UINTPTR)SysTaskGenerateException, 0, 0) 
#endif




/**
 * <description>
 *  Function to investigate the stack task.
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <param name="pulMaxDepth" type="OUT">Maximum stack depth</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskCheckStack(RTS_HANDLE hTask, RTS_UI32 *pulMaxDepth);
typedef RTS_RESULT (CDECL * PFSYSTASKCHECKSTACK) (RTS_HANDLE hTask, RTS_UI32 *pulMaxDepth);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKCHECKSTACK_NOTIMPLEMENTED)
	#define USE_SysTaskCheckStack
	#define EXT_SysTaskCheckStack
	#define GET_SysTaskCheckStack(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskCheckStack(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskCheckStack  FALSE
	#define EXP_SysTaskCheckStack  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskCheckStack
	#define EXT_SysTaskCheckStack
	#define GET_SysTaskCheckStack(fl)  CAL_CMGETAPI( "SysTaskCheckStack" ) 
	#define CAL_SysTaskCheckStack  SysTaskCheckStack
	#define CHK_SysTaskCheckStack  TRUE
	#define EXP_SysTaskCheckStack  CAL_CMEXPAPI( "SysTaskCheckStack" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskCheckStack
	#define EXT_SysTaskCheckStack
	#define GET_SysTaskCheckStack(fl)  CAL_CMGETAPI( "SysTaskCheckStack" ) 
	#define CAL_SysTaskCheckStack  SysTaskCheckStack
	#define CHK_SysTaskCheckStack  TRUE
	#define EXP_SysTaskCheckStack  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskCheckStack", (RTS_UINTPTR)SysTaskCheckStack, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskCheckStack
	#define EXT_SysTaskSysTaskCheckStack
	#define GET_SysTaskSysTaskCheckStack  ERR_OK
	#define CAL_SysTaskSysTaskCheckStack(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskCheckStack(p1) : ((ISysTask*)p0)->ISysTaskCheckStack(p1))
	#define CHK_SysTaskSysTaskCheckStack  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskCheckStack  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskCheckStack
	#define EXT_SysTaskCheckStack
	#define GET_SysTaskCheckStack(fl)  CAL_CMGETAPI( "SysTaskCheckStack" ) 
	#define CAL_SysTaskCheckStack(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskCheckStack(p1) : ((ISysTask*)p0)->ISysTaskCheckStack(p1))
	#define CHK_SysTaskCheckStack  (pISysTask != NULL)
	#define EXP_SysTaskCheckStack  CAL_CMEXPAPI( "SysTaskCheckStack" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskCheckStack  PFSYSTASKCHECKSTACK pfSysTaskCheckStack;
	#define EXT_SysTaskCheckStack  extern PFSYSTASKCHECKSTACK pfSysTaskCheckStack;
	#define GET_SysTaskCheckStack(fl)  s_pfCMGetAPI2( "SysTaskCheckStack", (RTS_VOID_FCTPTR *)&pfSysTaskCheckStack, (fl), 0, 0)
	#define CAL_SysTaskCheckStack  pfSysTaskCheckStack
	#define CHK_SysTaskCheckStack  (pfSysTaskCheckStack != NULL)
	#define EXP_SysTaskCheckStack  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskCheckStack", (RTS_UINTPTR)SysTaskCheckStack, 0, 0) 
#endif




/**
 * <description>
 *  Function to get the operating system specific handle.
 * </description>
 * <param name="hTask" type="IN">Handle to task</param>
 * <result>error code</result>
 */
RTS_HANDLE CDECL SysTaskGetOSHandle(RTS_HANDLE hTask);
typedef RTS_HANDLE (CDECL * PFSYSTASKGETOSHANDLE) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETOSHANDLE_NOTIMPLEMENTED)
	#define USE_SysTaskGetOSHandle
	#define EXT_SysTaskGetOSHandle
	#define GET_SysTaskGetOSHandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetOSHandle(p0)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysTaskGetOSHandle  FALSE
	#define EXP_SysTaskGetOSHandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetOSHandle
	#define EXT_SysTaskGetOSHandle
	#define GET_SysTaskGetOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetOSHandle" ) 
	#define CAL_SysTaskGetOSHandle  SysTaskGetOSHandle
	#define CHK_SysTaskGetOSHandle  TRUE
	#define EXP_SysTaskGetOSHandle  CAL_CMEXPAPI( "SysTaskGetOSHandle" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetOSHandle
	#define EXT_SysTaskGetOSHandle
	#define GET_SysTaskGetOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetOSHandle" ) 
	#define CAL_SysTaskGetOSHandle  SysTaskGetOSHandle
	#define CHK_SysTaskGetOSHandle  TRUE
	#define EXP_SysTaskGetOSHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetOSHandle", (RTS_UINTPTR)SysTaskGetOSHandle, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetOSHandle
	#define EXT_SysTaskSysTaskGetOSHandle
	#define GET_SysTaskSysTaskGetOSHandle  ERR_OK
	#define CAL_SysTaskSysTaskGetOSHandle(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetOSHandle() : ((ISysTask*)p0)->ISysTaskGetOSHandle())
	#define CHK_SysTaskSysTaskGetOSHandle  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetOSHandle  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetOSHandle
	#define EXT_SysTaskGetOSHandle
	#define GET_SysTaskGetOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetOSHandle" ) 
	#define CAL_SysTaskGetOSHandle(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetOSHandle() : ((ISysTask*)p0)->ISysTaskGetOSHandle())
	#define CHK_SysTaskGetOSHandle  (pISysTask != NULL)
	#define EXP_SysTaskGetOSHandle  CAL_CMEXPAPI( "SysTaskGetOSHandle" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetOSHandle  PFSYSTASKGETOSHANDLE pfSysTaskGetOSHandle;
	#define EXT_SysTaskGetOSHandle  extern PFSYSTASKGETOSHANDLE pfSysTaskGetOSHandle;
	#define GET_SysTaskGetOSHandle(fl)  s_pfCMGetAPI2( "SysTaskGetOSHandle", (RTS_VOID_FCTPTR *)&pfSysTaskGetOSHandle, (fl), 0, 0)
	#define CAL_SysTaskGetOSHandle  pfSysTaskGetOSHandle
	#define CHK_SysTaskGetOSHandle  (pfSysTaskGetOSHandle != NULL)
	#define EXP_SysTaskGetOSHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetOSHandle", (RTS_UINTPTR)SysTaskGetOSHandle, 0, 0) 
#endif




/**
 * <description>
 * OBSOLETE FUNCTION: Use SysTaskGetHandleByOSHandle() and SysTaskGetInfo() instead!
 *  Function to get the task handle specified by the operating system task handle.
 * </description>
 * <param name="uiTaskOSHandle" type="IN">Operating system task handleHandle to task</param>
 * <result>Pointer to SYS_TASK_INFO</result>
 */
SYS_TASK_INFO * CDECL SysTaskGetByOSHandle(RTS_HANDLE uiTaskOSHandle);
typedef SYS_TASK_INFO * (CDECL * PFSYSTASKGETBYOSHANDLE) (RTS_HANDLE uiTaskOSHandle);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETBYOSHANDLE_NOTIMPLEMENTED)
	#define USE_SysTaskGetByOSHandle
	#define EXT_SysTaskGetByOSHandle
	#define GET_SysTaskGetByOSHandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetByOSHandle(p0)  (SYS_TASK_INFO *)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetByOSHandle  FALSE
	#define EXP_SysTaskGetByOSHandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetByOSHandle
	#define EXT_SysTaskGetByOSHandle
	#define GET_SysTaskGetByOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetByOSHandle" ) 
	#define CAL_SysTaskGetByOSHandle  SysTaskGetByOSHandle
	#define CHK_SysTaskGetByOSHandle  TRUE
	#define EXP_SysTaskGetByOSHandle  CAL_CMEXPAPI( "SysTaskGetByOSHandle" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetByOSHandle
	#define EXT_SysTaskGetByOSHandle
	#define GET_SysTaskGetByOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetByOSHandle" ) 
	#define CAL_SysTaskGetByOSHandle  SysTaskGetByOSHandle
	#define CHK_SysTaskGetByOSHandle  TRUE
	#define EXP_SysTaskGetByOSHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetByOSHandle", (RTS_UINTPTR)SysTaskGetByOSHandle, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetByOSHandle
	#define EXT_SysTaskSysTaskGetByOSHandle
	#define GET_SysTaskSysTaskGetByOSHandle  ERR_OK
	#define CAL_SysTaskSysTaskGetByOSHandle pISysTask->ISysTaskGetByOSHandle
	#define CHK_SysTaskSysTaskGetByOSHandle (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetByOSHandle  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetByOSHandle
	#define EXT_SysTaskGetByOSHandle
	#define GET_SysTaskGetByOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetByOSHandle" ) 
	#define CAL_SysTaskGetByOSHandle pISysTask->ISysTaskGetByOSHandle
	#define CHK_SysTaskGetByOSHandle (pISysTask != NULL)
	#define EXP_SysTaskGetByOSHandle  CAL_CMEXPAPI( "SysTaskGetByOSHandle" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetByOSHandle  PFSYSTASKGETBYOSHANDLE pfSysTaskGetByOSHandle;
	#define EXT_SysTaskGetByOSHandle  extern PFSYSTASKGETBYOSHANDLE pfSysTaskGetByOSHandle;
	#define GET_SysTaskGetByOSHandle(fl)  s_pfCMGetAPI2( "SysTaskGetByOSHandle", (RTS_VOID_FCTPTR *)&pfSysTaskGetByOSHandle, (fl), 0, 0)
	#define CAL_SysTaskGetByOSHandle  pfSysTaskGetByOSHandle
	#define CHK_SysTaskGetByOSHandle  (pfSysTaskGetByOSHandle != NULL)
	#define EXP_SysTaskGetByOSHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetByOSHandle", (RTS_UINTPTR)SysTaskGetByOSHandle, 0, 0) 
#endif




/**
 * <description>
 *  Function to get the task handle specified by the operating system task handle.
 * </description>
 * <param name="uiTaskOSHandle" type="IN">Operating system task handleHandle to task</param>
 * <param name="pResult" type="OUT">Pointer to error code.
 *      ERR_OK: Ok
 *      ERR_FAILED: Task not found
 * <result>Handle to the specified task</result>
 */
RTS_HANDLE CDECL SysTaskGetHandleByOSHandle(RTS_HANDLE uiTaskOSHandle, RTS_RESULT *pResult);
typedef RTS_HANDLE (CDECL * PFSYSTASKGETHANDLEBYOSHANDLE) (RTS_HANDLE uiTaskOSHandle, RTS_RESULT *pResult);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETHANDLEBYOSHANDLE_NOTIMPLEMENTED)
	#define USE_SysTaskGetHandleByOSHandle
	#define EXT_SysTaskGetHandleByOSHandle
	#define GET_SysTaskGetHandleByOSHandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetHandleByOSHandle(p0,p1)  (RTS_HANDLE)RTS_INVALID_HANDLE
	#define CHK_SysTaskGetHandleByOSHandle  FALSE
	#define EXP_SysTaskGetHandleByOSHandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetHandleByOSHandle
	#define EXT_SysTaskGetHandleByOSHandle
	#define GET_SysTaskGetHandleByOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetHandleByOSHandle" ) 
	#define CAL_SysTaskGetHandleByOSHandle  SysTaskGetHandleByOSHandle
	#define CHK_SysTaskGetHandleByOSHandle  TRUE
	#define EXP_SysTaskGetHandleByOSHandle  CAL_CMEXPAPI( "SysTaskGetHandleByOSHandle" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetHandleByOSHandle
	#define EXT_SysTaskGetHandleByOSHandle
	#define GET_SysTaskGetHandleByOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetHandleByOSHandle" ) 
	#define CAL_SysTaskGetHandleByOSHandle  SysTaskGetHandleByOSHandle
	#define CHK_SysTaskGetHandleByOSHandle  TRUE
	#define EXP_SysTaskGetHandleByOSHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetHandleByOSHandle", (RTS_UINTPTR)SysTaskGetHandleByOSHandle, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetHandleByOSHandle
	#define EXT_SysTaskSysTaskGetHandleByOSHandle
	#define GET_SysTaskSysTaskGetHandleByOSHandle  ERR_OK
	#define CAL_SysTaskSysTaskGetHandleByOSHandle pISysTask->ISysTaskGetHandleByOSHandle
	#define CHK_SysTaskSysTaskGetHandleByOSHandle (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetHandleByOSHandle  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetHandleByOSHandle
	#define EXT_SysTaskGetHandleByOSHandle
	#define GET_SysTaskGetHandleByOSHandle(fl)  CAL_CMGETAPI( "SysTaskGetHandleByOSHandle" ) 
	#define CAL_SysTaskGetHandleByOSHandle pISysTask->ISysTaskGetHandleByOSHandle
	#define CHK_SysTaskGetHandleByOSHandle (pISysTask != NULL)
	#define EXP_SysTaskGetHandleByOSHandle  CAL_CMEXPAPI( "SysTaskGetHandleByOSHandle" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetHandleByOSHandle  PFSYSTASKGETHANDLEBYOSHANDLE pfSysTaskGetHandleByOSHandle;
	#define EXT_SysTaskGetHandleByOSHandle  extern PFSYSTASKGETHANDLEBYOSHANDLE pfSysTaskGetHandleByOSHandle;
	#define GET_SysTaskGetHandleByOSHandle(fl)  s_pfCMGetAPI2( "SysTaskGetHandleByOSHandle", (RTS_VOID_FCTPTR *)&pfSysTaskGetHandleByOSHandle, (fl), 0, 0)
	#define CAL_SysTaskGetHandleByOSHandle  pfSysTaskGetHandleByOSHandle
	#define CHK_SysTaskGetHandleByOSHandle  (pfSysTaskGetHandleByOSHandle != NULL)
	#define EXP_SysTaskGetHandleByOSHandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetHandleByOSHandle", (RTS_UINTPTR)SysTaskGetHandleByOSHandle, 0, 0) 
#endif




/**
 * <description>
 *  Function to get the configured priority out of the settings. See category "Settings" | "Mapping task priorities" for details.
 * </description>
 * <param name="ulPriority" type="IN">Task priority</param>
 * <param name="pResult" type="OUT">Pointer to error code.
 *      ERR_OK: Priority is mapped
 *      ERR_FAILED: Original priority is returned, because no mapping is configured
 * </param>
 * <result>Configured priority</result>
 */
RTS_UI32 CDECL SysTaskGetConfiguredPriority(RTS_UI32 ulPriority, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFSYSTASKGETCONFIGUREDPRIORITY) (RTS_UI32 ulPriority, RTS_RESULT *pResult);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETCONFIGUREDPRIORITY_NOTIMPLEMENTED)
	#define USE_SysTaskGetConfiguredPriority
	#define EXT_SysTaskGetConfiguredPriority
	#define GET_SysTaskGetConfiguredPriority(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetConfiguredPriority(p0,p1)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetConfiguredPriority  FALSE
	#define EXP_SysTaskGetConfiguredPriority  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetConfiguredPriority
	#define EXT_SysTaskGetConfiguredPriority
	#define GET_SysTaskGetConfiguredPriority(fl)  CAL_CMGETAPI( "SysTaskGetConfiguredPriority" ) 
	#define CAL_SysTaskGetConfiguredPriority  SysTaskGetConfiguredPriority
	#define CHK_SysTaskGetConfiguredPriority  TRUE
	#define EXP_SysTaskGetConfiguredPriority  CAL_CMEXPAPI( "SysTaskGetConfiguredPriority" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetConfiguredPriority
	#define EXT_SysTaskGetConfiguredPriority
	#define GET_SysTaskGetConfiguredPriority(fl)  CAL_CMGETAPI( "SysTaskGetConfiguredPriority" ) 
	#define CAL_SysTaskGetConfiguredPriority  SysTaskGetConfiguredPriority
	#define CHK_SysTaskGetConfiguredPriority  TRUE
	#define EXP_SysTaskGetConfiguredPriority  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetConfiguredPriority", (RTS_UINTPTR)SysTaskGetConfiguredPriority, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetConfiguredPriority
	#define EXT_SysTaskSysTaskGetConfiguredPriority
	#define GET_SysTaskSysTaskGetConfiguredPriority  ERR_OK
	#define CAL_SysTaskSysTaskGetConfiguredPriority pISysTask->ISysTaskGetConfiguredPriority
	#define CHK_SysTaskSysTaskGetConfiguredPriority (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetConfiguredPriority  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetConfiguredPriority
	#define EXT_SysTaskGetConfiguredPriority
	#define GET_SysTaskGetConfiguredPriority(fl)  CAL_CMGETAPI( "SysTaskGetConfiguredPriority" ) 
	#define CAL_SysTaskGetConfiguredPriority pISysTask->ISysTaskGetConfiguredPriority
	#define CHK_SysTaskGetConfiguredPriority (pISysTask != NULL)
	#define EXP_SysTaskGetConfiguredPriority  CAL_CMEXPAPI( "SysTaskGetConfiguredPriority" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetConfiguredPriority  PFSYSTASKGETCONFIGUREDPRIORITY pfSysTaskGetConfiguredPriority;
	#define EXT_SysTaskGetConfiguredPriority  extern PFSYSTASKGETCONFIGUREDPRIORITY pfSysTaskGetConfiguredPriority;
	#define GET_SysTaskGetConfiguredPriority(fl)  s_pfCMGetAPI2( "SysTaskGetConfiguredPriority", (RTS_VOID_FCTPTR *)&pfSysTaskGetConfiguredPriority, (fl), 0, 0)
	#define CAL_SysTaskGetConfiguredPriority  pfSysTaskGetConfiguredPriority
	#define CHK_SysTaskGetConfiguredPriority  (pfSysTaskGetConfiguredPriority != NULL)
	#define EXP_SysTaskGetConfiguredPriority  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetConfiguredPriority", (RTS_UINTPTR)SysTaskGetConfiguredPriority, 0, 0) 
#endif




/**
 * <description>
 *  Function to get the configured priority of a specified task out of the settings. See category "Settings" | "Mapping task priorities" for details.
 * </description>
 * <param name="hTask" type="IN">Handle to the task</param>
 * <param name="ulPriority" type="IN">Task priority</param>
 * <param name="pResult" type="OUT">Pointer to error code.
 *      ERR_OK: Priority is mapped
 *      ERR_FAILED: Original priority is returned, because no mapping is configured
 * </param>
 * <result>Configured priority</result>
 */
RTS_UI32 CDECL SysTaskGetConfiguredPriority2(RTS_HANDLE hTask, RTS_UI32 ulPriority, RTS_RESULT *pResult);
typedef RTS_UI32 (CDECL * PFSYSTASKGETCONFIGUREDPRIORITY2) (RTS_HANDLE hTask, RTS_UI32 ulPriority, RTS_RESULT *pResult);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETCONFIGUREDPRIORITY2_NOTIMPLEMENTED)
	#define USE_SysTaskGetConfiguredPriority2
	#define EXT_SysTaskGetConfiguredPriority2
	#define GET_SysTaskGetConfiguredPriority2(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetConfiguredPriority2(p0,p1,p2)  (RTS_UI32)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetConfiguredPriority2  FALSE
	#define EXP_SysTaskGetConfiguredPriority2  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetConfiguredPriority2
	#define EXT_SysTaskGetConfiguredPriority2
	#define GET_SysTaskGetConfiguredPriority2(fl)  CAL_CMGETAPI( "SysTaskGetConfiguredPriority2" ) 
	#define CAL_SysTaskGetConfiguredPriority2  SysTaskGetConfiguredPriority2
	#define CHK_SysTaskGetConfiguredPriority2  TRUE
	#define EXP_SysTaskGetConfiguredPriority2  CAL_CMEXPAPI( "SysTaskGetConfiguredPriority2" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetConfiguredPriority2
	#define EXT_SysTaskGetConfiguredPriority2
	#define GET_SysTaskGetConfiguredPriority2(fl)  CAL_CMGETAPI( "SysTaskGetConfiguredPriority2" ) 
	#define CAL_SysTaskGetConfiguredPriority2  SysTaskGetConfiguredPriority2
	#define CHK_SysTaskGetConfiguredPriority2  TRUE
	#define EXP_SysTaskGetConfiguredPriority2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetConfiguredPriority2", (RTS_UINTPTR)SysTaskGetConfiguredPriority2, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetConfiguredPriority2
	#define EXT_SysTaskSysTaskGetConfiguredPriority2
	#define GET_SysTaskSysTaskGetConfiguredPriority2  ERR_OK
	#define CAL_SysTaskSysTaskGetConfiguredPriority2(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetConfiguredPriority2(p1,p2) : ((ISysTask*)p0)->ISysTaskGetConfiguredPriority2(p1,p2))
	#define CHK_SysTaskSysTaskGetConfiguredPriority2  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetConfiguredPriority2  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetConfiguredPriority2
	#define EXT_SysTaskGetConfiguredPriority2
	#define GET_SysTaskGetConfiguredPriority2(fl)  CAL_CMGETAPI( "SysTaskGetConfiguredPriority2" ) 
	#define CAL_SysTaskGetConfiguredPriority2(p0,p1,p2)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetConfiguredPriority2(p1,p2) : ((ISysTask*)p0)->ISysTaskGetConfiguredPriority2(p1,p2))
	#define CHK_SysTaskGetConfiguredPriority2  (pISysTask != NULL)
	#define EXP_SysTaskGetConfiguredPriority2  CAL_CMEXPAPI( "SysTaskGetConfiguredPriority2" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetConfiguredPriority2  PFSYSTASKGETCONFIGUREDPRIORITY2 pfSysTaskGetConfiguredPriority2;
	#define EXT_SysTaskGetConfiguredPriority2  extern PFSYSTASKGETCONFIGUREDPRIORITY2 pfSysTaskGetConfiguredPriority2;
	#define GET_SysTaskGetConfiguredPriority2(fl)  s_pfCMGetAPI2( "SysTaskGetConfiguredPriority2", (RTS_VOID_FCTPTR *)&pfSysTaskGetConfiguredPriority2, (fl), 0, 0)
	#define CAL_SysTaskGetConfiguredPriority2  pfSysTaskGetConfiguredPriority2
	#define CHK_SysTaskGetConfiguredPriority2  (pfSysTaskGetConfiguredPriority2 != NULL)
	#define EXP_SysTaskGetConfiguredPriority2  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetConfiguredPriority2", (RTS_UINTPTR)SysTaskGetConfiguredPriority2, 0, 0) 
#endif




/**
 * <description>
 *  Get the actual interval of a cyclic task. If the specified task is no cyclic task, the function return an error.
 * </description>
 * <param name="hTask" type="IN">Handle to the task</param>
 * <param name="pulInterval" type="OUT">Pointer to the interval in microseconds!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskGetInterval(RTS_HANDLE hTask, RTS_UI32 *pulInterval);
typedef RTS_RESULT (CDECL * PFSYSTASKGETINTERVAL) (RTS_HANDLE hTask, RTS_UI32 *pulInterval);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETINTERVAL_NOTIMPLEMENTED)
	#define USE_SysTaskGetInterval
	#define EXT_SysTaskGetInterval
	#define GET_SysTaskGetInterval(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetInterval(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetInterval  FALSE
	#define EXP_SysTaskGetInterval  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetInterval
	#define EXT_SysTaskGetInterval
	#define GET_SysTaskGetInterval(fl)  CAL_CMGETAPI( "SysTaskGetInterval" ) 
	#define CAL_SysTaskGetInterval  SysTaskGetInterval
	#define CHK_SysTaskGetInterval  TRUE
	#define EXP_SysTaskGetInterval  CAL_CMEXPAPI( "SysTaskGetInterval" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetInterval
	#define EXT_SysTaskGetInterval
	#define GET_SysTaskGetInterval(fl)  CAL_CMGETAPI( "SysTaskGetInterval" ) 
	#define CAL_SysTaskGetInterval  SysTaskGetInterval
	#define CHK_SysTaskGetInterval  TRUE
	#define EXP_SysTaskGetInterval  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetInterval", (RTS_UINTPTR)SysTaskGetInterval, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetInterval
	#define EXT_SysTaskSysTaskGetInterval
	#define GET_SysTaskSysTaskGetInterval  ERR_OK
	#define CAL_SysTaskSysTaskGetInterval(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetInterval(p1) : ((ISysTask*)p0)->ISysTaskGetInterval(p1))
	#define CHK_SysTaskSysTaskGetInterval  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetInterval  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetInterval
	#define EXT_SysTaskGetInterval
	#define GET_SysTaskGetInterval(fl)  CAL_CMGETAPI( "SysTaskGetInterval" ) 
	#define CAL_SysTaskGetInterval(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskGetInterval(p1) : ((ISysTask*)p0)->ISysTaskGetInterval(p1))
	#define CHK_SysTaskGetInterval  (pISysTask != NULL)
	#define EXP_SysTaskGetInterval  CAL_CMEXPAPI( "SysTaskGetInterval" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetInterval  PFSYSTASKGETINTERVAL pfSysTaskGetInterval;
	#define EXT_SysTaskGetInterval  extern PFSYSTASKGETINTERVAL pfSysTaskGetInterval;
	#define GET_SysTaskGetInterval(fl)  s_pfCMGetAPI2( "SysTaskGetInterval", (RTS_VOID_FCTPTR *)&pfSysTaskGetInterval, (fl), 0, 0)
	#define CAL_SysTaskGetInterval  pfSysTaskGetInterval
	#define CHK_SysTaskGetInterval  (pfSysTaskGetInterval != NULL)
	#define EXP_SysTaskGetInterval  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetInterval", (RTS_UINTPTR)SysTaskGetInterval, 0, 0) 
#endif




/**
 * <description>
 *  Set the actual interval of a cyclic task. If the specified task is no cyclic task, the function return an error.
 *  This interface can be used to synchronize a task to another task or to events.
 * </description>
 * <param name="hTask" type="IN">Handle to the task</param>
 * <param name="ulInterval" type="IN">New interval in microseconds!</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskSetInterval(RTS_HANDLE hTask, RTS_UI32 ulInterval);
typedef RTS_RESULT (CDECL * PFSYSTASKSETINTERVAL) (RTS_HANDLE hTask, RTS_UI32 ulInterval);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKSETINTERVAL_NOTIMPLEMENTED)
	#define USE_SysTaskSetInterval
	#define EXT_SysTaskSetInterval
	#define GET_SysTaskSetInterval(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskSetInterval(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskSetInterval  FALSE
	#define EXP_SysTaskSetInterval  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskSetInterval
	#define EXT_SysTaskSetInterval
	#define GET_SysTaskSetInterval(fl)  CAL_CMGETAPI( "SysTaskSetInterval" ) 
	#define CAL_SysTaskSetInterval  SysTaskSetInterval
	#define CHK_SysTaskSetInterval  TRUE
	#define EXP_SysTaskSetInterval  CAL_CMEXPAPI( "SysTaskSetInterval" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskSetInterval
	#define EXT_SysTaskSetInterval
	#define GET_SysTaskSetInterval(fl)  CAL_CMGETAPI( "SysTaskSetInterval" ) 
	#define CAL_SysTaskSetInterval  SysTaskSetInterval
	#define CHK_SysTaskSetInterval  TRUE
	#define EXP_SysTaskSetInterval  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskSetInterval", (RTS_UINTPTR)SysTaskSetInterval, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskSetInterval
	#define EXT_SysTaskSysTaskSetInterval
	#define GET_SysTaskSysTaskSetInterval  ERR_OK
	#define CAL_SysTaskSysTaskSetInterval(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskSetInterval(p1) : ((ISysTask*)p0)->ISysTaskSetInterval(p1))
	#define CHK_SysTaskSysTaskSetInterval  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskSetInterval  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskSetInterval
	#define EXT_SysTaskSetInterval
	#define GET_SysTaskSetInterval(fl)  CAL_CMGETAPI( "SysTaskSetInterval" ) 
	#define CAL_SysTaskSetInterval(p0,p1)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskSetInterval(p1) : ((ISysTask*)p0)->ISysTaskSetInterval(p1))
	#define CHK_SysTaskSetInterval  (pISysTask != NULL)
	#define EXP_SysTaskSetInterval  CAL_CMEXPAPI( "SysTaskSetInterval" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskSetInterval  PFSYSTASKSETINTERVAL pfSysTaskSetInterval;
	#define EXT_SysTaskSetInterval  extern PFSYSTASKSETINTERVAL pfSysTaskSetInterval;
	#define GET_SysTaskSetInterval(fl)  s_pfCMGetAPI2( "SysTaskSetInterval", (RTS_VOID_FCTPTR *)&pfSysTaskSetInterval, (fl), 0, 0)
	#define CAL_SysTaskSetInterval  pfSysTaskSetInterval
	#define CHK_SysTaskSetInterval  (pfSysTaskSetInterval != NULL)
	#define EXP_SysTaskSetInterval  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskSetInterval", (RTS_UINTPTR)SysTaskSetInterval, 0, 0) 
#endif





/**
 * <description>Create a new key to access a specific thread local storage type</description>
 * <param name="phKey" type="IN">New key to access the local storage.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskCreateThreadLocalStorageKey(RTS_HANDLE *phKey);
typedef RTS_RESULT (CDECL * PFSYSTASKCREATETHREADLOCALSTORAGEKEY) (RTS_HANDLE *phKey);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKCREATETHREADLOCALSTORAGEKEY_NOTIMPLEMENTED)
	#define USE_SysTaskCreateThreadLocalStorageKey
	#define EXT_SysTaskCreateThreadLocalStorageKey
	#define GET_SysTaskCreateThreadLocalStorageKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskCreateThreadLocalStorageKey(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskCreateThreadLocalStorageKey  FALSE
	#define EXP_SysTaskCreateThreadLocalStorageKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskCreateThreadLocalStorageKey
	#define EXT_SysTaskCreateThreadLocalStorageKey
	#define GET_SysTaskCreateThreadLocalStorageKey(fl)  CAL_CMGETAPI( "SysTaskCreateThreadLocalStorageKey" ) 
	#define CAL_SysTaskCreateThreadLocalStorageKey  SysTaskCreateThreadLocalStorageKey
	#define CHK_SysTaskCreateThreadLocalStorageKey  TRUE
	#define EXP_SysTaskCreateThreadLocalStorageKey  CAL_CMEXPAPI( "SysTaskCreateThreadLocalStorageKey" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskCreateThreadLocalStorageKey
	#define EXT_SysTaskCreateThreadLocalStorageKey
	#define GET_SysTaskCreateThreadLocalStorageKey(fl)  CAL_CMGETAPI( "SysTaskCreateThreadLocalStorageKey" ) 
	#define CAL_SysTaskCreateThreadLocalStorageKey  SysTaskCreateThreadLocalStorageKey
	#define CHK_SysTaskCreateThreadLocalStorageKey  TRUE
	#define EXP_SysTaskCreateThreadLocalStorageKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskCreateThreadLocalStorageKey", (RTS_UINTPTR)SysTaskCreateThreadLocalStorageKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskCreateThreadLocalStorageKey
	#define EXT_SysTaskSysTaskCreateThreadLocalStorageKey
	#define GET_SysTaskSysTaskCreateThreadLocalStorageKey  ERR_OK
	#define CAL_SysTaskSysTaskCreateThreadLocalStorageKey pISysTask->ISysTaskCreateThreadLocalStorageKey
	#define CHK_SysTaskSysTaskCreateThreadLocalStorageKey (pISysTask != NULL)
	#define EXP_SysTaskSysTaskCreateThreadLocalStorageKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskCreateThreadLocalStorageKey
	#define EXT_SysTaskCreateThreadLocalStorageKey
	#define GET_SysTaskCreateThreadLocalStorageKey(fl)  CAL_CMGETAPI( "SysTaskCreateThreadLocalStorageKey" ) 
	#define CAL_SysTaskCreateThreadLocalStorageKey pISysTask->ISysTaskCreateThreadLocalStorageKey
	#define CHK_SysTaskCreateThreadLocalStorageKey (pISysTask != NULL)
	#define EXP_SysTaskCreateThreadLocalStorageKey  CAL_CMEXPAPI( "SysTaskCreateThreadLocalStorageKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskCreateThreadLocalStorageKey  PFSYSTASKCREATETHREADLOCALSTORAGEKEY pfSysTaskCreateThreadLocalStorageKey;
	#define EXT_SysTaskCreateThreadLocalStorageKey  extern PFSYSTASKCREATETHREADLOCALSTORAGEKEY pfSysTaskCreateThreadLocalStorageKey;
	#define GET_SysTaskCreateThreadLocalStorageKey(fl)  s_pfCMGetAPI2( "SysTaskCreateThreadLocalStorageKey", (RTS_VOID_FCTPTR *)&pfSysTaskCreateThreadLocalStorageKey, (fl), 0, 0)
	#define CAL_SysTaskCreateThreadLocalStorageKey  pfSysTaskCreateThreadLocalStorageKey
	#define CHK_SysTaskCreateThreadLocalStorageKey  (pfSysTaskCreateThreadLocalStorageKey != NULL)
	#define EXP_SysTaskCreateThreadLocalStorageKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskCreateThreadLocalStorageKey", (RTS_UINTPTR)SysTaskCreateThreadLocalStorageKey, 0, 0) 
#endif




/**
 * <description>Delete a key used to access thread local storage. All entries are removed from the threads.</description>
 * <param name="phKey" type="IN">Key to remove.</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskDeleteThreadLocalStorageKey(RTS_HANDLE *phKey);
typedef RTS_RESULT (CDECL * PFSYSTASKDELETETHREADLOCALSTORAGEKEY) (RTS_HANDLE *phKey);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKDELETETHREADLOCALSTORAGEKEY_NOTIMPLEMENTED)
	#define USE_SysTaskDeleteThreadLocalStorageKey
	#define EXT_SysTaskDeleteThreadLocalStorageKey
	#define GET_SysTaskDeleteThreadLocalStorageKey(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskDeleteThreadLocalStorageKey(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskDeleteThreadLocalStorageKey  FALSE
	#define EXP_SysTaskDeleteThreadLocalStorageKey  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskDeleteThreadLocalStorageKey
	#define EXT_SysTaskDeleteThreadLocalStorageKey
	#define GET_SysTaskDeleteThreadLocalStorageKey(fl)  CAL_CMGETAPI( "SysTaskDeleteThreadLocalStorageKey" ) 
	#define CAL_SysTaskDeleteThreadLocalStorageKey  SysTaskDeleteThreadLocalStorageKey
	#define CHK_SysTaskDeleteThreadLocalStorageKey  TRUE
	#define EXP_SysTaskDeleteThreadLocalStorageKey  CAL_CMEXPAPI( "SysTaskDeleteThreadLocalStorageKey" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskDeleteThreadLocalStorageKey
	#define EXT_SysTaskDeleteThreadLocalStorageKey
	#define GET_SysTaskDeleteThreadLocalStorageKey(fl)  CAL_CMGETAPI( "SysTaskDeleteThreadLocalStorageKey" ) 
	#define CAL_SysTaskDeleteThreadLocalStorageKey  SysTaskDeleteThreadLocalStorageKey
	#define CHK_SysTaskDeleteThreadLocalStorageKey  TRUE
	#define EXP_SysTaskDeleteThreadLocalStorageKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskDeleteThreadLocalStorageKey", (RTS_UINTPTR)SysTaskDeleteThreadLocalStorageKey, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskDeleteThreadLocalStorageKey
	#define EXT_SysTaskSysTaskDeleteThreadLocalStorageKey
	#define GET_SysTaskSysTaskDeleteThreadLocalStorageKey  ERR_OK
	#define CAL_SysTaskSysTaskDeleteThreadLocalStorageKey pISysTask->ISysTaskDeleteThreadLocalStorageKey
	#define CHK_SysTaskSysTaskDeleteThreadLocalStorageKey (pISysTask != NULL)
	#define EXP_SysTaskSysTaskDeleteThreadLocalStorageKey  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskDeleteThreadLocalStorageKey
	#define EXT_SysTaskDeleteThreadLocalStorageKey
	#define GET_SysTaskDeleteThreadLocalStorageKey(fl)  CAL_CMGETAPI( "SysTaskDeleteThreadLocalStorageKey" ) 
	#define CAL_SysTaskDeleteThreadLocalStorageKey pISysTask->ISysTaskDeleteThreadLocalStorageKey
	#define CHK_SysTaskDeleteThreadLocalStorageKey (pISysTask != NULL)
	#define EXP_SysTaskDeleteThreadLocalStorageKey  CAL_CMEXPAPI( "SysTaskDeleteThreadLocalStorageKey" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskDeleteThreadLocalStorageKey  PFSYSTASKDELETETHREADLOCALSTORAGEKEY pfSysTaskDeleteThreadLocalStorageKey;
	#define EXT_SysTaskDeleteThreadLocalStorageKey  extern PFSYSTASKDELETETHREADLOCALSTORAGEKEY pfSysTaskDeleteThreadLocalStorageKey;
	#define GET_SysTaskDeleteThreadLocalStorageKey(fl)  s_pfCMGetAPI2( "SysTaskDeleteThreadLocalStorageKey", (RTS_VOID_FCTPTR *)&pfSysTaskDeleteThreadLocalStorageKey, (fl), 0, 0)
	#define CAL_SysTaskDeleteThreadLocalStorageKey  pfSysTaskDeleteThreadLocalStorageKey
	#define CHK_SysTaskDeleteThreadLocalStorageKey  (pfSysTaskDeleteThreadLocalStorageKey != NULL)
	#define EXP_SysTaskDeleteThreadLocalStorageKey  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskDeleteThreadLocalStorageKey", (RTS_UINTPTR)SysTaskDeleteThreadLocalStorageKey, 0, 0) 
#endif




/**
 * <description>Add a value to the current thread. Note: the caller is responsible to clean up the data if the key is removed.</description>
 * <param name="phKey" type="IN">Key created with SysTaskCreateThreadLocalStorageKey</param>
 * <result>error code</result>
 */
RTS_RESULT CDECL SysTaskSetThreadLocalStorage(RTS_HANDLE hKey, void *pData);
typedef RTS_RESULT (CDECL * PFSYSTASKSETTHREADLOCALSTORAGE) (RTS_HANDLE hKey, void *pData);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKSETTHREADLOCALSTORAGE_NOTIMPLEMENTED)
	#define USE_SysTaskSetThreadLocalStorage
	#define EXT_SysTaskSetThreadLocalStorage
	#define GET_SysTaskSetThreadLocalStorage(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskSetThreadLocalStorage(p0,p1)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskSetThreadLocalStorage  FALSE
	#define EXP_SysTaskSetThreadLocalStorage  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskSetThreadLocalStorage
	#define EXT_SysTaskSetThreadLocalStorage
	#define GET_SysTaskSetThreadLocalStorage(fl)  CAL_CMGETAPI( "SysTaskSetThreadLocalStorage" ) 
	#define CAL_SysTaskSetThreadLocalStorage  SysTaskSetThreadLocalStorage
	#define CHK_SysTaskSetThreadLocalStorage  TRUE
	#define EXP_SysTaskSetThreadLocalStorage  CAL_CMEXPAPI( "SysTaskSetThreadLocalStorage" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskSetThreadLocalStorage
	#define EXT_SysTaskSetThreadLocalStorage
	#define GET_SysTaskSetThreadLocalStorage(fl)  CAL_CMGETAPI( "SysTaskSetThreadLocalStorage" ) 
	#define CAL_SysTaskSetThreadLocalStorage  SysTaskSetThreadLocalStorage
	#define CHK_SysTaskSetThreadLocalStorage  TRUE
	#define EXP_SysTaskSetThreadLocalStorage  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskSetThreadLocalStorage", (RTS_UINTPTR)SysTaskSetThreadLocalStorage, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskSetThreadLocalStorage
	#define EXT_SysTaskSysTaskSetThreadLocalStorage
	#define GET_SysTaskSysTaskSetThreadLocalStorage  ERR_OK
	#define CAL_SysTaskSysTaskSetThreadLocalStorage pISysTask->ISysTaskSetThreadLocalStorage
	#define CHK_SysTaskSysTaskSetThreadLocalStorage (pISysTask != NULL)
	#define EXP_SysTaskSysTaskSetThreadLocalStorage  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskSetThreadLocalStorage
	#define EXT_SysTaskSetThreadLocalStorage
	#define GET_SysTaskSetThreadLocalStorage(fl)  CAL_CMGETAPI( "SysTaskSetThreadLocalStorage" ) 
	#define CAL_SysTaskSetThreadLocalStorage pISysTask->ISysTaskSetThreadLocalStorage
	#define CHK_SysTaskSetThreadLocalStorage (pISysTask != NULL)
	#define EXP_SysTaskSetThreadLocalStorage  CAL_CMEXPAPI( "SysTaskSetThreadLocalStorage" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskSetThreadLocalStorage  PFSYSTASKSETTHREADLOCALSTORAGE pfSysTaskSetThreadLocalStorage;
	#define EXT_SysTaskSetThreadLocalStorage  extern PFSYSTASKSETTHREADLOCALSTORAGE pfSysTaskSetThreadLocalStorage;
	#define GET_SysTaskSetThreadLocalStorage(fl)  s_pfCMGetAPI2( "SysTaskSetThreadLocalStorage", (RTS_VOID_FCTPTR *)&pfSysTaskSetThreadLocalStorage, (fl), 0, 0)
	#define CAL_SysTaskSetThreadLocalStorage  pfSysTaskSetThreadLocalStorage
	#define CHK_SysTaskSetThreadLocalStorage  (pfSysTaskSetThreadLocalStorage != NULL)
	#define EXP_SysTaskSetThreadLocalStorage  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskSetThreadLocalStorage", (RTS_UINTPTR)SysTaskSetThreadLocalStorage, 0, 0) 
#endif




/**
 * <description>Get the value stored for that key within the threads local storage.</description>
 * <param name="phKey" type="IN">Key created with SysTaskCreateThreadLocalStorageKey</param>
 * <result>error code</result>
 */
void * CDECL SysTaskGetThreadLocalStorage(RTS_HANDLE hKey, RTS_RESULT *pResult);
typedef void * (CDECL * PFSYSTASKGETTHREADLOCALSTORAGE) (RTS_HANDLE hKey, RTS_RESULT *pResult);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETTHREADLOCALSTORAGE_NOTIMPLEMENTED)
	#define USE_SysTaskGetThreadLocalStorage
	#define EXT_SysTaskGetThreadLocalStorage
	#define GET_SysTaskGetThreadLocalStorage(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskGetThreadLocalStorage(p0,p1)  (void *)(RTS_SIZE)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskGetThreadLocalStorage  FALSE
	#define EXP_SysTaskGetThreadLocalStorage  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskGetThreadLocalStorage
	#define EXT_SysTaskGetThreadLocalStorage
	#define GET_SysTaskGetThreadLocalStorage(fl)  CAL_CMGETAPI( "SysTaskGetThreadLocalStorage" ) 
	#define CAL_SysTaskGetThreadLocalStorage  SysTaskGetThreadLocalStorage
	#define CHK_SysTaskGetThreadLocalStorage  TRUE
	#define EXP_SysTaskGetThreadLocalStorage  CAL_CMEXPAPI( "SysTaskGetThreadLocalStorage" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskGetThreadLocalStorage
	#define EXT_SysTaskGetThreadLocalStorage
	#define GET_SysTaskGetThreadLocalStorage(fl)  CAL_CMGETAPI( "SysTaskGetThreadLocalStorage" ) 
	#define CAL_SysTaskGetThreadLocalStorage  SysTaskGetThreadLocalStorage
	#define CHK_SysTaskGetThreadLocalStorage  TRUE
	#define EXP_SysTaskGetThreadLocalStorage  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetThreadLocalStorage", (RTS_UINTPTR)SysTaskGetThreadLocalStorage, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskGetThreadLocalStorage
	#define EXT_SysTaskSysTaskGetThreadLocalStorage
	#define GET_SysTaskSysTaskGetThreadLocalStorage  ERR_OK
	#define CAL_SysTaskSysTaskGetThreadLocalStorage pISysTask->ISysTaskGetThreadLocalStorage
	#define CHK_SysTaskSysTaskGetThreadLocalStorage (pISysTask != NULL)
	#define EXP_SysTaskSysTaskGetThreadLocalStorage  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskGetThreadLocalStorage
	#define EXT_SysTaskGetThreadLocalStorage
	#define GET_SysTaskGetThreadLocalStorage(fl)  CAL_CMGETAPI( "SysTaskGetThreadLocalStorage" ) 
	#define CAL_SysTaskGetThreadLocalStorage pISysTask->ISysTaskGetThreadLocalStorage
	#define CHK_SysTaskGetThreadLocalStorage (pISysTask != NULL)
	#define EXP_SysTaskGetThreadLocalStorage  CAL_CMEXPAPI( "SysTaskGetThreadLocalStorage" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskGetThreadLocalStorage  PFSYSTASKGETTHREADLOCALSTORAGE pfSysTaskGetThreadLocalStorage;
	#define EXT_SysTaskGetThreadLocalStorage  extern PFSYSTASKGETTHREADLOCALSTORAGE pfSysTaskGetThreadLocalStorage;
	#define GET_SysTaskGetThreadLocalStorage(fl)  s_pfCMGetAPI2( "SysTaskGetThreadLocalStorage", (RTS_VOID_FCTPTR *)&pfSysTaskGetThreadLocalStorage, (fl), 0, 0)
	#define CAL_SysTaskGetThreadLocalStorage  pfSysTaskGetThreadLocalStorage
	#define CHK_SysTaskGetThreadLocalStorage  (pfSysTaskGetThreadLocalStorage != NULL)
	#define EXP_SysTaskGetThreadLocalStorage  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskGetThreadLocalStorage", (RTS_UINTPTR)SysTaskGetThreadLocalStorage, 0, 0) 
#endif




/**
 * <description>
 *	Check a task handle, if it is still valid and is not released, and lock the task.
 *	If the check is successful, you have to unlock the task at the end of the usage with SysTaskUnlock().
 *	NOTE:
 *	In the background the complete task pool of the runtime system is locked. This prevents for example the creation of a new task. The lock therefore should be held very short!
 * </description>
 * <param name="hTask" type="IN">Handle to the task</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">hTask is valid and could be locked</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">hTask is an invalid handle</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_FAILED">hTask could not be found</errorcode>
 */
RTS_RESULT CDECL SysTaskLock(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKLOCK) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKLOCK_NOTIMPLEMENTED)
	#define USE_SysTaskLock
	#define EXT_SysTaskLock
	#define GET_SysTaskLock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskLock(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskLock  FALSE
	#define EXP_SysTaskLock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskLock
	#define EXT_SysTaskLock
	#define GET_SysTaskLock(fl)  CAL_CMGETAPI( "SysTaskLock" ) 
	#define CAL_SysTaskLock  SysTaskLock
	#define CHK_SysTaskLock  TRUE
	#define EXP_SysTaskLock  CAL_CMEXPAPI( "SysTaskLock" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskLock
	#define EXT_SysTaskLock
	#define GET_SysTaskLock(fl)  CAL_CMGETAPI( "SysTaskLock" ) 
	#define CAL_SysTaskLock  SysTaskLock
	#define CHK_SysTaskLock  TRUE
	#define EXP_SysTaskLock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskLock", (RTS_UINTPTR)SysTaskLock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskLock
	#define EXT_SysTaskSysTaskLock
	#define GET_SysTaskSysTaskLock  ERR_OK
	#define CAL_SysTaskSysTaskLock(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskLock() : ((ISysTask*)p0)->ISysTaskLock())
	#define CHK_SysTaskSysTaskLock  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskLock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskLock
	#define EXT_SysTaskLock
	#define GET_SysTaskLock(fl)  CAL_CMGETAPI( "SysTaskLock" ) 
	#define CAL_SysTaskLock(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskLock() : ((ISysTask*)p0)->ISysTaskLock())
	#define CHK_SysTaskLock  (pISysTask != NULL)
	#define EXP_SysTaskLock  CAL_CMEXPAPI( "SysTaskLock" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskLock  PFSYSTASKLOCK pfSysTaskLock;
	#define EXT_SysTaskLock  extern PFSYSTASKLOCK pfSysTaskLock;
	#define GET_SysTaskLock(fl)  s_pfCMGetAPI2( "SysTaskLock", (RTS_VOID_FCTPTR *)&pfSysTaskLock, (fl), 0, 0)
	#define CAL_SysTaskLock  pfSysTaskLock
	#define CHK_SysTaskLock  (pfSysTaskLock != NULL)
	#define EXP_SysTaskLock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskLock", (RTS_UINTPTR)SysTaskLock, 0, 0) 
#endif




/**
 * <description>
 *	Unlock the task previously locked by SysTaskLock.
 * </description>
 * <param name="hTask" type="IN">Handle to the task</param>
 * <result>error code</result>
 * <errorcode name="RTS_RESULT" type="ERR_OK">hTask is valid and could unlocked</errorcode>
 * <errorcode name="RTS_RESULT" type="ERR_PARAMETER">hTask is an invalid handle</errorcode>
 */
RTS_RESULT CDECL SysTaskUnlock(RTS_HANDLE hTask);
typedef RTS_RESULT (CDECL * PFSYSTASKUNLOCK) (RTS_HANDLE hTask);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKUNLOCK_NOTIMPLEMENTED)
	#define USE_SysTaskUnlock
	#define EXT_SysTaskUnlock
	#define GET_SysTaskUnlock(fl)  ERR_NOTIMPLEMENTED
	#define CAL_SysTaskUnlock(p0)  (RTS_RESULT)ERR_NOTIMPLEMENTED
	#define CHK_SysTaskUnlock  FALSE
	#define EXP_SysTaskUnlock  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_SysTaskUnlock
	#define EXT_SysTaskUnlock
	#define GET_SysTaskUnlock(fl)  CAL_CMGETAPI( "SysTaskUnlock" ) 
	#define CAL_SysTaskUnlock  SysTaskUnlock
	#define CHK_SysTaskUnlock  TRUE
	#define EXP_SysTaskUnlock  CAL_CMEXPAPI( "SysTaskUnlock" ) 
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_SysTaskUnlock
	#define EXT_SysTaskUnlock
	#define GET_SysTaskUnlock(fl)  CAL_CMGETAPI( "SysTaskUnlock" ) 
	#define CAL_SysTaskUnlock  SysTaskUnlock
	#define CHK_SysTaskUnlock  TRUE
	#define EXP_SysTaskUnlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskUnlock", (RTS_UINTPTR)SysTaskUnlock, 0, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTaskSysTaskUnlock
	#define EXT_SysTaskSysTaskUnlock
	#define GET_SysTaskSysTaskUnlock  ERR_OK
	#define CAL_SysTaskSysTaskUnlock(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskUnlock() : ((ISysTask*)p0)->ISysTaskUnlock())
	#define CHK_SysTaskSysTaskUnlock  (pISysTask != NULL)
	#define EXP_SysTaskSysTaskUnlock  ERR_OK
#elif defined(CPLUSPLUS)
	#define USE_SysTaskUnlock
	#define EXT_SysTaskUnlock
	#define GET_SysTaskUnlock(fl)  CAL_CMGETAPI( "SysTaskUnlock" ) 
	#define CAL_SysTaskUnlock(p0)		(p0 == RTS_INVALID_HANDLE || p0 == NULL ? pISysTask->ISysTaskUnlock() : ((ISysTask*)p0)->ISysTaskUnlock())
	#define CHK_SysTaskUnlock  (pISysTask != NULL)
	#define EXP_SysTaskUnlock  CAL_CMEXPAPI( "SysTaskUnlock" ) 
#else /* DYNAMIC_LINK */
	#define USE_SysTaskUnlock  PFSYSTASKUNLOCK pfSysTaskUnlock;
	#define EXT_SysTaskUnlock  extern PFSYSTASKUNLOCK pfSysTaskUnlock;
	#define GET_SysTaskUnlock(fl)  s_pfCMGetAPI2( "SysTaskUnlock", (RTS_VOID_FCTPTR *)&pfSysTaskUnlock, (fl), 0, 0)
	#define CAL_SysTaskUnlock  pfSysTaskUnlock
	#define CHK_SysTaskUnlock  (pfSysTaskUnlock != NULL)
	#define EXP_SysTaskUnlock  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"SysTaskUnlock", (RTS_UINTPTR)SysTaskUnlock, 0, 0) 
#endif




/* Obsolete function. Can be removed in the future */
typedef struct
{
    RTS_HANDLE hTask;
    RTS_HANDLE out;
}systaskgettaskhandle_struct ;
void CDECL CDECL_EXT systaskgettaskhandle(systaskgettaskhandle_struct *p);
typedef void (CDECL CDECL_EXT* PFSYSTASKGETTASKHANDLE_IEC) (systaskgettaskhandle_struct *p);
#if defined(SYSTASK_NOTIMPLEMENTED) || defined(SYSTASKGETTASKHANDLE_NOTIMPLEMENTED)
	#define USE_systaskgettaskhandle
	#define EXT_systaskgettaskhandle
	#define GET_systaskgettaskhandle(fl)  ERR_NOTIMPLEMENTED
	#define CAL_systaskgettaskhandle(p0) 
	#define CHK_systaskgettaskhandle  FALSE
	#define EXP_systaskgettaskhandle  ERR_OK
#elif defined(STATIC_LINK)
	#define USE_systaskgettaskhandle
	#define EXT_systaskgettaskhandle
	#define GET_systaskgettaskhandle(fl)  CAL_CMGETAPI( "systaskgettaskhandle" ) 
	#define CAL_systaskgettaskhandle  systaskgettaskhandle
	#define CHK_systaskgettaskhandle  TRUE
	#define EXP_systaskgettaskhandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"systaskgettaskhandle", (RTS_UINTPTR)systaskgettaskhandle, 1, 0) 
	#define EXTREF_ITF_systaskgettaskhandle {(RTS_VOID_FCTPTR)systaskgettaskhandle, "systaskgettaskhandle", 0, 0}
#elif defined(MIXED_LINK) && !defined(SYSTASK_EXTERNAL)
	#define USE_systaskgettaskhandle
	#define EXT_systaskgettaskhandle
	#define GET_systaskgettaskhandle(fl)  CAL_CMGETAPI( "systaskgettaskhandle" ) 
	#define CAL_systaskgettaskhandle  systaskgettaskhandle
	#define CHK_systaskgettaskhandle  TRUE
	#define EXP_systaskgettaskhandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"systaskgettaskhandle", (RTS_UINTPTR)systaskgettaskhandle, 1, 0) 
#elif defined(CPLUSPLUS_ONLY)
	#define USE_SysTasksystaskgettaskhandle
	#define EXT_SysTasksystaskgettaskhandle
	#define GET_SysTasksystaskgettaskhandle  ERR_OK
	#define CAL_SysTasksystaskgettaskhandle  systaskgettaskhandle
	#define CHK_SysTasksystaskgettaskhandle  TRUE
	#define EXP_SysTasksystaskgettaskhandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"systaskgettaskhandle", (RTS_UINTPTR)systaskgettaskhandle, 1, 0) 
#elif defined(CPLUSPLUS)
	#define USE_systaskgettaskhandle
	#define EXT_systaskgettaskhandle
	#define GET_systaskgettaskhandle(fl)  CAL_CMGETAPI( "systaskgettaskhandle" ) 
	#define CAL_systaskgettaskhandle  systaskgettaskhandle
	#define CHK_systaskgettaskhandle  TRUE
	#define EXP_systaskgettaskhandle  s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"systaskgettaskhandle", (RTS_UINTPTR)systaskgettaskhandle, 1, 0) 
#else /* DYNAMIC_LINK */
	#define USE_systaskgettaskhandle  PFSYSTASKGETTASKHANDLE_IEC pfsystaskgettaskhandle;
	#define EXT_systaskgettaskhandle  extern PFSYSTASKGETTASKHANDLE_IEC pfsystaskgettaskhandle;
	#define GET_systaskgettaskhandle(fl)  s_pfCMGetAPI2( "systaskgettaskhandle", (RTS_VOID_FCTPTR *)&pfsystaskgettaskhandle, (fl), 0, 0)
	#define CAL_systaskgettaskhandle  pfsystaskgettaskhandle
	#define CHK_systaskgettaskhandle  (pfsystaskgettaskhandle != NULL)
	#define EXP_systaskgettaskhandle   s_pfCMRegisterAPI( (const CMP_EXT_FUNCTION_REF*)"systaskgettaskhandle", (RTS_UINTPTR)systaskgettaskhandle, 1, 0) 
#endif



#ifdef __cplusplus
}
#endif



typedef struct
{
	IBase_C *pBase;
	PFSYSTASKGETFIRST ISysTaskGetFirst;
 	PFSYSTASKGETNEXT ISysTaskGetNext;
 	PFSYSTASKCREATE ISysTaskCreate;
 	PFSYSTASKCREATE2 ISysTaskCreate2;
 	PFSYSTASKEXIT ISysTaskExit;
 	PFSYSTASKSETEXIT ISysTaskSetExit;
 	PFSYSTASKAUTORELEASEONEXIT ISysTaskAutoReleaseOnExit;
 	PFSYSTASKREMOVEAUTORELEASEONEXIT ISysTaskRemoveAutoReleaseOnExit;
 	PFSYSTASKDESTROY ISysTaskDestroy;
 	PFSYSTASKSUSPEND ISysTaskSuspend;
 	PFSYSTASKRESUME ISysTaskResume;
 	PFSYSTASKEND ISysTaskEnd;
 	PFSYSTASKWAITINTERVAL ISysTaskWaitInterval;
 	PFSYSTASKJOIN ISysTaskJoin;
 	PFSYSTASKGETCURRENT ISysTaskGetCurrent;
 	PFSYSTASKGETCURRENTOSHANDLE ISysTaskGetCurrentOSHandle;
 	PFSYSTASKSETPRIORITY ISysTaskSetPriority;
 	PFSYSTASKGETPRIORITY ISysTaskGetPriority;
 	PFSYSTASKGETOSPRIORITY ISysTaskGetOSPriority;
 	PFSYSTASKGETINFO ISysTaskGetInfo;
 	PFSYSTASKGETNAME ISysTaskGetName;
 	PFSYSTASKENTER ISysTaskEnter;
 	PFSYSTASKLEAVE ISysTaskLeave;
 	PFSYSTASKGETCONTEXT ISysTaskGetContext;
 	PFSYSTASKGENERATEEXCEPTION ISysTaskGenerateException;
 	PFSYSTASKCHECKSTACK ISysTaskCheckStack;
 	PFSYSTASKGETOSHANDLE ISysTaskGetOSHandle;
 	PFSYSTASKGETBYOSHANDLE ISysTaskGetByOSHandle;
 	PFSYSTASKGETHANDLEBYOSHANDLE ISysTaskGetHandleByOSHandle;
 	PFSYSTASKGETCONFIGUREDPRIORITY ISysTaskGetConfiguredPriority;
 	PFSYSTASKGETCONFIGUREDPRIORITY2 ISysTaskGetConfiguredPriority2;
 	PFSYSTASKGETINTERVAL ISysTaskGetInterval;
 	PFSYSTASKSETINTERVAL ISysTaskSetInterval;
 	PFSYSTASKCREATETHREADLOCALSTORAGEKEY ISysTaskCreateThreadLocalStorageKey;
 	PFSYSTASKDELETETHREADLOCALSTORAGEKEY ISysTaskDeleteThreadLocalStorageKey;
 	PFSYSTASKSETTHREADLOCALSTORAGE ISysTaskSetThreadLocalStorage;
 	PFSYSTASKGETTHREADLOCALSTORAGE ISysTaskGetThreadLocalStorage;
 	PFSYSTASKLOCK ISysTaskLock;
 	PFSYSTASKUNLOCK ISysTaskUnlock;
 } ISysTask_C;

#ifdef CPLUSPLUS
class ISysTask : public IBase
{
	public:
		virtual RTS_HANDLE CDECL ISysTaskGetFirst(RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysTaskGetNext(RTS_HANDLE hPrevTask, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysTaskCreate(char* pszTaskName, PFSYS_TASK_FUNCTION pFunction, void *pParam, RTS_UI32 ulPriority, RTS_UI32 ulInterval, RTS_UI32 ulStackSize, PFTASKEXCEPTIONHANDLER pExceptionHandler, RTS_RESULT *pResult) =0;
		virtual RTS_HANDLE CDECL ISysTaskCreate2(char* pszTaskName, char *pszTaskGroup, PFSYS_TASK_FUNCTION pFunction, void *pParam, RTS_UI32 ulPriority, RTS_UI32 ulInterval, RTS_UI32 ulStackSize, PFTASKEXCEPTIONHANDLER pExceptionHandler, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysTaskExit(RTS_UI32 ulTimeoutMs) =0;
		virtual RTS_RESULT CDECL ISysTaskSetExit(void) =0;
		virtual RTS_RESULT CDECL ISysTaskAutoReleaseOnExit(void) =0;
		virtual RTS_RESULT CDECL ISysTaskRemoveAutoReleaseOnExit(void) =0;
		virtual RTS_RESULT CDECL ISysTaskDestroy(void) =0;
		virtual RTS_RESULT CDECL ISysTaskSuspend(void) =0;
		virtual RTS_RESULT CDECL ISysTaskResume(void) =0;
		virtual RTS_RESULT CDECL ISysTaskEnd(RTS_UINTPTR ulExitCode) =0;
		virtual RTS_RESULT CDECL ISysTaskWaitInterval(void) =0;
		virtual RTS_RESULT CDECL ISysTaskJoin(RTS_HANDLE hTaskToJoin, RTS_UI32 ulTimeoutMs) =0;
		virtual RTS_RESULT CDECL ISysTaskGetCurrent(RTS_HANDLE *phTask) =0;
		virtual RTS_RESULT CDECL ISysTaskGetCurrentOSHandle(RTS_HANDLE *puiTaskOSHandle) =0;
		virtual RTS_RESULT CDECL ISysTaskSetPriority(RTS_UI32 ulPriority) =0;
		virtual RTS_RESULT CDECL ISysTaskGetPriority(RTS_UI32 *pulPriority) =0;
		virtual RTS_RESULT CDECL ISysTaskGetOSPriority(RTS_UI32 *pulOSPriority) =0;
		virtual RTS_RESULT CDECL ISysTaskGetInfo(SYS_TASK_INFO **ppInfo) =0;
		virtual char* CDECL ISysTaskGetName(RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysTaskEnter(void) =0;
		virtual RTS_RESULT CDECL ISysTaskLeave(void) =0;
		virtual RTS_RESULT CDECL ISysTaskGetContext(RegContext *pContext) =0;
		virtual RTS_RESULT CDECL ISysTaskGenerateException(RTS_HANDLE uiTaskOSHandle, RTS_UI32 ulException, RegContext Context) =0;
		virtual RTS_RESULT CDECL ISysTaskCheckStack(RTS_UI32 *pulMaxDepth) =0;
		virtual RTS_HANDLE CDECL ISysTaskGetOSHandle(void) =0;
		virtual SYS_TASK_INFO * CDECL ISysTaskGetByOSHandle(RTS_HANDLE uiTaskOSHandle) =0;
		virtual RTS_HANDLE CDECL ISysTaskGetHandleByOSHandle(RTS_HANDLE uiTaskOSHandle, RTS_RESULT *pResult) =0;
		virtual RTS_UI32 CDECL ISysTaskGetConfiguredPriority(RTS_UI32 ulPriority, RTS_RESULT *pResult) =0;
		virtual RTS_UI32 CDECL ISysTaskGetConfiguredPriority2(RTS_UI32 ulPriority, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysTaskGetInterval(RTS_UI32 *pulInterval) =0;
		virtual RTS_RESULT CDECL ISysTaskSetInterval(RTS_UI32 ulInterval) =0;
		virtual RTS_RESULT CDECL ISysTaskCreateThreadLocalStorageKey(RTS_HANDLE *phKey) =0;
		virtual RTS_RESULT CDECL ISysTaskDeleteThreadLocalStorageKey(RTS_HANDLE *phKey) =0;
		virtual RTS_RESULT CDECL ISysTaskSetThreadLocalStorage(RTS_HANDLE hKey, void *pData) =0;
		virtual void * CDECL ISysTaskGetThreadLocalStorage(RTS_HANDLE hKey, RTS_RESULT *pResult) =0;
		virtual RTS_RESULT CDECL ISysTaskLock(void) =0;
		virtual RTS_RESULT CDECL ISysTaskUnlock(void) =0;
};
	#ifndef ITF_SysTask
		#define ITF_SysTask static ISysTask *pISysTask = NULL;
	#endif
	#define EXTITF_SysTask
#else	/*CPLUSPLUS*/
	typedef ISysTask_C		ISysTask;
	#ifndef ITF_SysTask
		#define ITF_SysTask
	#endif
	#define EXTITF_SysTask
#endif

#ifdef CPLUSPLUS_ONLY
  #undef CPLUSPLUS_ONLY
#endif

#endif /*_SYSTASKITF_H_*/
