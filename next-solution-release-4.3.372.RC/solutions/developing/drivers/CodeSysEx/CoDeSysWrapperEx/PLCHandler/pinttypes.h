#ifndef __PINTTYPES_H__
#define __PINTTYPES_H__


#define PRI_I8		"d"
#define PRI_I16		"d"

#define PRI_UI8		"u"
#define PRI_UI16	"u"

#define PRI_X8		"x"
#define PRI_X16		"x"

#define PRI_HANDLE	"p"
#define PRI_RESULT	"x"

#define PRI_TIME	"x"
#define PRI_INT		"i"
#define PRI_BOOL	"u"

#define PRI_REAL32	"f"
#define PRI_REAL64	"f"

#if RTS_REGISTER_WIDTH == 16
#define PRI_I32		"ld"
#define PRI_I64		"lld"

#define PRI_UI32	"lu"
#define PRI_UI64	"llu"

#define PRI_X32		"lx"
#define PRI_X64		"llx"
#elif RTS_REGISTER_WIDTH == 32
#define PRI_I32		"d"
#define PRI_I64		"lld"

#define PRI_UI32	"u"
#define PRI_UI64	"llu"

#define PRI_X32		"x"
#define PRI_X64		"llx"
#elif RTS_REGISTER_WIDTH == 64
#define PRI_I32		"d"
#define PRI_UI32	"u"
#if SIZEOF_LONG < 8
 #define PRI_I64	"lld"
 #define PRI_UI64	"llu"
#else
 #define PRI_I64	"ld"
 #define PRI_UI64	"lu"
#endif
#define PRI_X32		"x"
#define PRI_X64		"x"
#endif

#if RTS_REGISTER_WIDTH == 16 || RTS_REGISTER_WIDTH == 32
	#define PRI_UINTPTR	PRI_UI32
	#define PRI_INTPTR	PRI_I32
	#define PRI_SIZE	PRI_UI32
	#define PRI_SSIZE	PRI_I32
	#define PRI_PTRDIFF	PRI_INTPTR
#elif RTS_REGISTER_WIDTH == 64
	#define PRI_UINTPTR	PRI_UI64
	#define PRI_INTPTR	PRI_I64
	#define PRI_SIZE	PRI_UI64
	#define PRI_SSIZE	PRI_I64
	#define PRI_PTRDIFF	PRI_INTPTR
#else
	#error "register width not supported."
#endif

#define SCN_I8		PRI_I8
#define SCN_I16		PRI_I16
#define SCN_I32		PRI_I32
#define SCN_I64		PRI_I64

#define SCN_UI8		PRI_UI8
#define SCN_UI16	PRI_UI16
#define SCN_UI32	PRI_UI32
#define SCN_UI64	PRI_UI64

#define SCN_X8		PRI_X8
#define SCN_X16		PRI_X16
#define SCN_X32		PRI_X32
#define SCN_X64		PRI_X64

#define SCN_HANDLE	PRI_HANDLE
#define SCN_RESULT	PRI_RESULT

#define SCN_TIME	PRI_TIME
#define SCN_INT		PRI_INT
#define SCN_BOOL	PRI_BOOL
#define SCN_SIZE	PRI_SIZE

#define SCN_REAL32	PRI_REAL32
#define SCN_REAL64	PRI_REAL64

#define SCN_UINTPTR	PRI_UINTPTR
#define SCN_INTPTR	PRI_INTPTR
#define SCN_SIZE	PRI_SIZE
#define SCN_PTRDIFF	PRI_PTRDIFF


#endif	/*__PINTTYPES_H__*/
