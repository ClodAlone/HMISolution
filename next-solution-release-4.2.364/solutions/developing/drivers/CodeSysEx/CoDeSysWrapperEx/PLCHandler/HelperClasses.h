#ifndef __HELPERCLASSES_H__
#define __HELPERCLASSES_H__

#undef PLCH_DLL_DECL
#if defined(PLCH_USE_DLL) && (defined(WIN32) || defined(_WIN32_WCE) || defined(_WIN32_WCE_EMULATION))
	#ifdef _USRDLL
		#define PLCH_DLL_DECL _declspec(dllexport)
	#else
		#define PLCH_DLL_DECL _declspec(dllimport)
	#endif
#elif (defined(LINUX) || defined(QNX)) && defined(_USRDLL)
	#define PLCH_DLL_DECL __attribute__ ((visibility ("default")))
#else
	#define PLCH_DLL_DECL
#endif

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

