#ifndef __HELPERCLASSES_H__
#define __HELPERCLASSES_H__

#undef PLCH_DLL_DECL
#if defined(PLCH_USE_DLL) && (defined(WIN32) || defined(_WIN32_WCE) || defined(_WIN32_WCE_EMULATION))
	#ifdef _USRDLL
		#define PLCH_DLL_DECL _declspec(dllexport)
	#else
		#define PLCH_DLL_DECL _declspec(dllimport)
	#endif
#else
	#define PLCH_DLL_DECL
#endif

// HIWORD of the ulTypeId
#define TYPECLASS_SIMPLE	0x00000000
#define TYPECLASS_USERDEF	0x00010000
#define TYPECLASS_ARRAY		0x00020000
#define TYPECLASS_POINTER	0x00040000
#define TYPECLASS_ENUM		0x00080000
#define TYPECLASS_PROPERTY	0x00100000
#define TYPECLASS_REFERENCE	0x00200000

// LOWORD of the ulTypeId
typedef enum PlcTypeClassTag
{
	DATATYPE_BOOL,
	DATATYPE_SINT,
	DATATYPE_USINT,
	DATATYPE_BYTE,
	DATATYPE_INT,
	DATATYPE_UINT,
	DATATYPE_WORD,
	DATATYPE_DINT,
	DATATYPE_UDINT,
	DATATYPE_DWORD,
	DATATYPE_REAL,
	DATATYPE_LREAL,
	DATATYPE_TIME,
	DATATYPE_STRING,
	DATATYPE_BITORBYTE,
	DATATYPE_DATE,
	DATATYPE_TOD,
	DATATYPE_DT,
	DATATYPE_REF,
	DATATYPE_VOID,
	DATATYPE_LINT,
	DATATYPE_ULINT,
	DATATYPE_LTIME,
	DATATYPE_WSTRING,
	DATATYPE_LWORD,
	DATATYPE_BIT,
	DATATYPE_MAX
} PlcTypeClass;


class PLCH_DLL_DECL HashContent
{
	public:
		HashContent(void);
		virtual ~HashContent(void);
        virtual void SetKeyName(char *psz);
};

struct HashEntry
{
	char *psz;
	HashContent* phc;
	HashEntry* pheNext;
};

class PLCH_DLL_DECL Hash
/* Hash table with extern collision resolution */
{
	public:
		Hash(int nSizeParam = 37);
		~Hash(void);

		int Map(char *psz);
		HashContent* Add(char *psz, HashContent* phc);
		int Delete(char *pwsz, int bDelete);
		HashContent* Get(char *psz);
		char *Exist(char *psz);

	private:
		int Map(char *psz, RTS_SSIZE nLen);
		int nSize;
		HashEntry** pphe;

		static const int ms_nHashPrime;
};

/* HashContent type for type classes */
class PLCH_DLL_DECL HashVarType : public HashContent
{
	public:
		HashVarType(unsigned long ulType);
		virtual ~HashVarType(void);
		unsigned long GetHashVarType(void);

	private:
		unsigned long m_ulType;
};


class PLCH_DLL_DECL DataTypeHashTable : public Hash
/* Hash table with extern collision resolution */
{
	public:
		DataTypeHashTable(int nSizeParam = 37);
		~DataTypeHashTable(void);
		unsigned long GetType(char *pszType, unsigned long ulSize);
};

#endif	//__HELPERCLASSES_H__

