#ifndef __XMLSYMBOLPARSER_H__
#define __XMLSYMBOLPARSER_H__

#include "CmpIecVarAccessItf.h"

#define X_READ_ERROR				   -1
#define X_READ_START					0
#define X_READ_SYMBOLCONFIGURATION		1
#define X_READ_HEADER					2
#define X_READ_TYPELIST					3
#define X_READ_NODELIST					4
#define X_READ_TYPE_SIMPLE				5
#define X_READ_TYPE_USERDEF				6
#define X_READ_TYPE_ARRAY				7
#define X_READ_USERDEF_ELEMENT			8
#define X_READ_ARRAY_DIM				9
#define X_READ_NODE					   10
#define X_READ_PROJECTINFO			   11
#define X_READ_UNKNOWN				   12
#define X_READ_SUCCESSFULL_FINISCHED   99


#define MAX_NODENAME_PREFIX_LEN	255

typedef struct
{
	char*                   pszTypeName;
	char*					pszTypeClass;
	char*					pszIecName;
	char*					pszBaseType;
	TypeDescAsUnion*		ptd3;
} XMLTYPENODE;

typedef struct
{
	char*                   pszTypeName;
	char*					pszIecName;
	unsigned long			ulElemOffset;
	unsigned long			ulExtraInfo;
} XMLUSERDEFELEM;

typedef struct
{
	char*                   pszName;
	char*					pszType;
	char*					pszMappedAddr;
	char					pszAccess[2];
	long					lArea;
	unsigned long			ulOffset;
	unsigned long			ulExtraInfo;
} XMLSYMBOLNODE;


class CXmlSymbolParser
{
	public:
		CXmlSymbolParser(/*[In]*/char *pszXmlSymbolFile, /*[In]*/int bGetMonitoringAddresses, /*[In]*/int bDontExpandSimpleTypeArrays = 0, /*[In]*/int bDontExpandComplexTypeArrays = 0);
		virtual ~CXmlSymbolParser(void);

		virtual long LoadSymbolsFromFile(/*[Out]*/PlcSymbolDesc **ppSymbolList, /*[Out]*/unsigned long *pulNumOfSymbols, /*[Out]*/char **ppszApplication, /*[Out]*/RTS_GUID **ppDataGuid = NULL, 
										/*[Out]*/char ***pppszMappedAddr = NULL, /*[Out]*/unsigned long *pulCacheSize = 0);
		virtual long DeleteSymbols(void);

	private:
		void HandleUnknownTag(RTS_CWCHAR *tag);
		void TagStartCallbackMethod(RTS_CWCHAR *el, RTS_CWCHAR **attr);
		void TagEndCallbackMethod(RTS_CWCHAR *el);
		void TagCallbackMethod(RTS_CWCHAR* data, int n);
		long ReadSimpleType(RTS_CWCHAR **ppAttributes);
		long ReadSimpleTypeDone(void);
		long ReadUserdefType(RTS_CWCHAR **ppAttributes);
		long ReadUserdefTypeElement(RTS_CWCHAR **ppAttributes);
		long ReadUserdefTypeDone(void);
		long ReadArrayType(RTS_CWCHAR **ppAttributes);
		long ReadArrayTypeDim(RTS_CWCHAR **ppAttributes);
		long ReadArrayTypeDone(void);
		long ReadSymbolNode(RTS_CWCHAR **ppAttributes);
		long ReadSymbolNodeDone(void);
		long ReadProjectInfo(RTS_CWCHAR **ppAttributes);
		long FindInsertPosition(char *pszSymbolname);

		int						m_iReadState;
		int						m_iLastState;
		int						m_iUnknownRecursionCount;
		RTS_CWCHAR				m_unknownTag[255];
		int						m_iNodeNameParseDepth;
		int						m_iNodeNameDepth;
		char					m_pszLeafNodePrefix[MAX_NODENAME_PREFIX_LEN];
		char					*m_pszXmlSymbolFile;
		int						m_bDontExpandSimpleTypeArrays;
		int						m_bDontExpandComplexTypeArrays;
		int						m_bGetMonitoringAddresses;
		int						m_nTypeNodes;
		int						m_nMaxTypeNodes;
		XMLTYPENODE				*m_pTypeNodes;
		int						m_nSymbolNodes;
		int						m_nMaxSymbolNodes;
		XMLSYMBOLNODE			*m_pSymbolNodes;
		int						m_nTempStructElements;
		int						m_nMaxTempStructElements;
		XMLUSERDEFELEM				*m_pTempStructElements;
		unsigned long			m_ulSymbols;
		unsigned long			m_ulMaxSymbols;
		PlcSymbolDesc			*m_pSymbols;
		char					*m_pszApplication;
		char					**m_ppszMappedAddr;
		unsigned long			*m_pulIndex;
		DataTypeHashTable		*m_pHashTableDataTypes;
		RTS_GUID				m_DataGuid;

		friend void TagStartCallback(void *UserData, RTS_CWCHAR *el, RTS_CWCHAR **attr);
		friend void TagEndCallback(void *UserData, RTS_CWCHAR *el);
		friend void TagCallback(void *UserData, RTS_CWCHAR* data, int n);
};

#endif	//__XMLSYMBOLPARSER_H__

