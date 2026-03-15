using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

/* Written by NyaRuRu */
namespace DynamicShader {
  namespace D3DX {
    using Native;
    using Win32;

    using D3DXHANDLE = IntPtr;
    using HRESULT = Int32;
    using ID3DXInclude = IntPtr;

    [Flags]
    public enum CompilerOptions {
      None = D3DXSHADER.NONE,
      Debug = D3DXSHADER.DEBUG,
      SkipValidation = D3DXSHADER.SKIPVALIDATION,
      SkipOptimization = D3DXSHADER.SKIPOPTIMIZATION,
      PackMatrixRowMajor = D3DXSHADER.PACKMATRIX_ROWMAJOR,
      PackMatrixColumnMajor = D3DXSHADER.PACKMATRIX_COLUMNMAJOR,
      PartialPrecision = D3DXSHADER.PARTIALPRECISION,
      ForceVertexShaderSoftwareNoOptimizations = D3DXSHADER.FORCE_VS_SOFTWARE_NOOPT,
      ForcePixelShaderSoftwareNoOptimizations = D3DXSHADER.FORCE_PS_SOFTWARE_NOOPT,
      NoPreShader = D3DXSHADER.NO_PRESHADER,
      AvoidFlowControl = D3DXSHADER.AVOID_FLOW_CONTROL,
      PreferFlowControl = D3DXSHADER.PREFER_FLOW_CONTROL,
      [Obsolete]
      NotCloneable = 0x800,
      EnableBackwardsCompatibility = D3DXSHADER.ENABLE_BACKWARDS_COMPATIBILITY,
      IEEEStrictness = D3DXSHADER.IEEE_STRICTNESS,
      UseLegacy_D3DX9_31_DLL = D3DXSHADER.USE_LEGACY_D3DX9_31_DLL,
      OptimizationLevel0 = D3DXSHADER.OPTIMIZATION_LEVEL0,
      OptimizationLevel1 = D3DXSHADER.OPTIMIZATION_LEVEL1,
      OptimizationLevel2 = D3DXSHADER.OPTIMIZATION_LEVEL2,
      OptimizationLevel3 = D3DXSHADER.OPTIMIZATION_LEVEL3,
    }

    public enum RegisterSet {
      Bool = D3DXREGISTER_SET.D3DXRS_BOOL,
      Int4 = D3DXREGISTER_SET.D3DXRS_INT4,
      Float4 = D3DXREGISTER_SET.D3DXRS_FLOAT4,
      Sampler = D3DXREGISTER_SET.D3DXRS_SAMPLER,
    }

    public enum ParameterClass {
      Scalar = D3DXPARAMETER_CLASS.D3DXPC_SCALAR,
      Vector = D3DXPARAMETER_CLASS.D3DXPC_VECTOR,
      MatrixRows = D3DXPARAMETER_CLASS.D3DXPC_MATRIX_ROWS,
      MatrixColumns = D3DXPARAMETER_CLASS.D3DXPC_MATRIX_COLUMNS,
      Object = D3DXPARAMETER_CLASS.D3DXPC_OBJECT,
      Struct = D3DXPARAMETER_CLASS.D3DXPC_STRUCT,
    }

    public enum ParameterType {
      Void = D3DXPARAMETER_TYPE.D3DXPT_VOID,
      Bool = D3DXPARAMETER_TYPE.D3DXPT_BOOL,
      Int = D3DXPARAMETER_TYPE.D3DXPT_INT,
      Float = D3DXPARAMETER_TYPE.D3DXPT_FLOAT,
      String = D3DXPARAMETER_TYPE.D3DXPT_STRING,
      Texture = D3DXPARAMETER_TYPE.D3DXPT_TEXTURE,
      Texture1D = D3DXPARAMETER_TYPE.D3DXPT_TEXTURE1D,
      Texture2D = D3DXPARAMETER_TYPE.D3DXPT_TEXTURE2D,
      Texture3D = D3DXPARAMETER_TYPE.D3DXPT_TEXTURE3D,
      TextureCube = D3DXPARAMETER_TYPE.D3DXPT_TEXTURECUBE,
      Sampler = D3DXPARAMETER_TYPE.D3DXPT_SAMPLER,
      Sampler1D = D3DXPARAMETER_TYPE.D3DXPT_SAMPLER1D,
      Sampler2D = D3DXPARAMETER_TYPE.D3DXPT_SAMPLER2D,
      Sampler3D = D3DXPARAMETER_TYPE.D3DXPT_SAMPLER3D,
      SamplerCube = D3DXPARAMETER_TYPE.D3DXPT_SAMPLERCUBE,
      PixelShader = D3DXPARAMETER_TYPE.D3DXPT_PIXELSHADER,
      VertexShader = D3DXPARAMETER_TYPE.D3DXPT_VERTEXSHADER,
      PixelFragment = D3DXPARAMETER_TYPE.D3DXPT_PIXELFRAGMENT,
      VertexFragment = D3DXPARAMETER_TYPE.D3DXPT_VERTEXFRAGMENT,
      Unsupported = D3DXPARAMETER_TYPE.D3DXPT_UNSUPPORTED,
    }

    [Flags]
    public enum ParameterFlags {
      Shared = D3DX_PARAMETER.SHARED,
      Literal = D3DX_PARAMETER.LITERAL,
      Annotation = D3DX_PARAMETER.ANNOTATION,
    }

    public sealed partial class ParameterInfo {
      public string Name { get; internal set; }
      public string Semantic { get; internal set; }
      public ParameterClass Class { get; internal set; }
      public ParameterType Type { get; internal set; }
      public int Rows { get; internal set; }
      public int Columns { get; internal set; }
      public int Elements { get; internal set; }
      public string[] Annotations { get; internal set; }
      public int StructMembers { get; internal set; }
      public ParameterFlags Flags { get; internal set; }
      public int Bytes { get; internal set; }
    }

    public sealed partial class ConstantInfo {
      public string Name { get; internal set; }
      public RegisterSet RegisterSet { get; internal set; }
      public int RegisterIndex { get; internal set; }
      public int RegisterCount { get; internal set; }

      public ParameterClass Class { get; internal set; }
      public ParameterType Type { get; internal set; }

      public int Rows { get; internal set; }
      public int Columns { get; internal set; }
      public int Elements { get; internal set; }
      public int StructMembers { get; internal set; }

      public int Bytes { get; internal set; }
      public byte[] DefaultValue { get; internal set; }
    }

    public sealed partial class CompiledShader {
      public bool CompileSucceeded { get { return Data != null; } }
      public int ConstantTableVersion { get; internal set; }
      public string ConstantTableCreator { get; internal set; }
      public byte[] Data { get; internal set; }
      public string ErrorMessage { get; internal set; }
      public ConstantInfo[][] TopLevelConstants { get; internal set; }
    }

    public static partial class D3DXUtil {
      [DebuggerStepThroughAttribute]
      private static void AssertNonNullArgument<T>(T argument, string argumentName) {
        if (argument == null) throw new ArgumentNullException(argumentName);
      }
      internal static string ReadAsAnsiString(this IntPtr ptr) {
        return ptr == IntPtr.Zero
                   ? string.Empty
                   : Marshal.PtrToStringAnsi(ptr);
      }
      internal static byte[] AsByteArray(this ID3DXBuffer buffer) {
        AssertNonNullArgument(buffer, "buffer");
        var buf = new byte[(int)buffer.GetBufferSize()];
        Marshal.Copy(buffer.GetBufferPointer(), buf, 0, (int)buffer.GetBufferSize());
        return buf;
      }
      internal static string AsAnsiString(this ID3DXBuffer buffer) {
        AssertNonNullArgument(buffer, "buffer");
        unsafe {
          var ptr = (sbyte*)buffer.GetBufferPointer();
          var size = (int)buffer.GetBufferSize();
          if (ptr[size - 1] == 0) {
            // skip the last null char
            size = Math.Max(checked(size - 1), 0);
          }
          return new string(ptr, 0, size);
        }
      }
      internal static RegisterSet ToRegisterSet(this D3DXREGISTER_SET src) {
        return (RegisterSet)src;
      }
      internal static D3DXREGISTER_SET ToD3DXREGISTER_SET(this RegisterSet src) {
        return (D3DXREGISTER_SET)src;
      }
      internal static ParameterClass ToParameterClass(this D3DXPARAMETER_CLASS src) {
        return (ParameterClass)src;
      }
      internal static D3DXPARAMETER_CLASS ToD3DXPARAMETER_CLASS(this ParameterClass src) {
        return (D3DXPARAMETER_CLASS)src;
      }
      internal static ParameterType ToParameterType(this D3DXPARAMETER_TYPE src) {
        return (ParameterType)src;
      }
      internal static D3DXPARAMETER_TYPE ToD3DXPARAMETER_TYPE(this ParameterType src) {
        return (D3DXPARAMETER_TYPE)src;
      }
      internal static ParameterFlags ToParameterFlags(this D3DX_PARAMETER src) {
        return (ParameterFlags)src;
      }
      internal static D3DX_PARAMETER ToD3DX_PARAMETER(this ParameterFlags src) {
        return (D3DX_PARAMETER)src;
      }
      internal static CompilerOptions ToCompilerOptions(this D3DXSHADER option) {
        return (CompilerOptions)option;
      }
      internal static D3DXSHADER ToD3DXSHADER(this CompilerOptions option) {
        return (D3DXSHADER)option;
      }
      internal static ConstantInfo ToConstantInfo(this D3DXCONSTANT_DESC desc) {
        var info = new ConstantInfo() {
          Name = desc.Name.ReadAsAnsiString(),
          RegisterSet = desc.RegisterSet.ToRegisterSet(),
          RegisterIndex = (int)desc.RegisterIndex,
          RegisterCount = (int)desc.RegisterCount,

          Class = desc.Class.ToParameterClass(),
          Type = desc.Type.ToParameterType(),

          Rows = (int)desc.Rows,
          Columns = (int)desc.Columns,
          Elements = (int)desc.Elements,

          StructMembers = (int)desc.StructMembers,

          Bytes = (int)desc.Bytes,
        };

        if (desc.DefaultValue != IntPtr.Zero) {
          info.DefaultValue = new byte[desc.Bytes];
          Marshal.Copy(desc.DefaultValue, info.DefaultValue, 0, (int)desc.Bytes);
        }

        return info;
      }
      internal static D3DXEFFECT_DESC? GetDesc(this ID3DXEffectCompiler compiler) {
        AssertNonNullArgument(compiler, "compiler");

        var desc = default(D3DXEFFECT_DESC);
        return Succeeded(compiler.GetDesc(out desc)) ? (D3DXEFFECT_DESC?)desc : null;
      }
      internal static D3DXFUNCTION_DESC? GetFunctionDesc(this ID3DXEffectCompiler compiler, D3DXHANDLE functionHandle) {
        AssertNonNullArgument(compiler, "compiler");

        var desc = default(D3DXFUNCTION_DESC);
        return Succeeded(compiler.GetFunctionDesc(functionHandle, out desc)) ? (D3DXFUNCTION_DESC?)desc : null;
      }
      internal static D3DXPARAMETER_DESC? GetParameterDesc(this ID3DXEffectCompiler compiler, D3DXHANDLE parameterHandle) {
        AssertNonNullArgument(compiler, "compiler");

        var desc = default(D3DXPARAMETER_DESC);
        return Succeeded(compiler.GetParameterDesc(parameterHandle, out desc)) ? (D3DXPARAMETER_DESC?)desc : null;
      }
      internal static D3DXCONSTANTTABLE_DESC? GetDesc(this ID3DXConstantTable contantTable) {
        AssertNonNullArgument(contantTable, "contantTable");

        var desc = default(D3DXCONSTANTTABLE_DESC);
        return Succeeded(contantTable.GetDesc(out desc)) ? (D3DXCONSTANTTABLE_DESC?)desc : null;
      }
      internal static IEnumerable<D3DXHANDLE> GetTopLevelConstantHandles(this ID3DXConstantTable contantTable) {
        AssertNonNullArgument(contantTable, "contantTable");

        var desc_ = contantTable.GetDesc();
        if (!desc_.HasValue) {
          return Enumerable.Empty<D3DXHANDLE>();
        }
        var desc = desc_.Value;
        return Enumerable.Range(0, (int)desc.Constants)
                         .Select(index => contantTable.GetConstant(IntPtr.Zero, (uint)index));
      }
      internal static IEnumerable<D3DXCONSTANT_DESC> GetConstantDesc(this ID3DXConstantTable contantTable, D3DXHANDLE constantHandle) {
        AssertNonNullArgument(contantTable, "contantTable");

        uint count = 0;
        if (Succeeded(contantTable.GetConstantDesc(constantHandle, null, ref count))) {
          var array = new D3DXCONSTANT_DESC[count];
          if (Succeeded(contantTable.GetConstantDesc(constantHandle, array, ref count))) {
            return array.Take((int)count);
          }
        }
        return Enumerable.Empty<D3DXCONSTANT_DESC>();
      }

      internal static ParameterInfo GetParameter(this ID3DXEffectCompiler compiler, IntPtr targetHandle, int paramIndex) {
        AssertNonNullArgument(compiler, "compiler");

        var paramHandle = compiler.GetParameter(targetHandle, (uint)paramIndex);
        var paramDesc_ = compiler.GetParameterDesc(paramHandle);
        if (!paramDesc_.HasValue) {
          return null;
        }

        var paramDesc = paramDesc_.Value;
        var annotations = from annotationIndex in Enumerable.Range(0, (int)paramDesc.Annotations)
                          let annotationHandle = compiler.GetAnnotation(paramHandle, (uint)annotationIndex)
                          let annotation = annotationHandle == IntPtr.Zero ? string.Empty : Marshal.PtrToStringAnsi(annotationHandle)
                          select annotation;

        return new ParameterInfo() {
          Annotations = annotations.ToArray(),
          Bytes = (int)paramDesc.Bytes,
          Class = paramDesc.Class.ToParameterClass(),
          Columns = (int)paramDesc.Columns,
          Elements = (int)paramDesc.Elements,
          Flags = paramDesc.Flags.ToParameterFlags(),
          Name = paramDesc.Name.ReadAsAnsiString(),
          Rows = (int)paramDesc.Rows,
          Semantic = paramDesc.Semantic.ReadAsAnsiString(),
          StructMembers = (int)paramDesc.StructMembers,
          Type = paramDesc.Type.ToParameterType(),
        };
      }

      internal static CompiledShader CompileShader(this ID3DXEffectCompiler compiler, string function, string profile, CompilerOptions option) {
        AssertNonNullArgument(compiler, "compiler");
        return CompileShader(compiler as ID3DXEffectCompiler_custom, function, profile, option);
      }

      internal static CompiledShader CompileShader(this ID3DXEffectCompiler_custom compiler, string function, string profile, CompilerOptions option) {
        AssertNonNullArgument(compiler, "compiler");

        var buffer = default(ID3DXBuffer);
        var msg = default(ID3DXBuffer);
        var constantTable = default(ID3DXConstantTable);

        try {
          var ret = new CompiledShader();
          if (Succeeded(compiler.CompileShader(function, profile, option.ToD3DXSHADER(), out buffer, out msg, out constantTable))) {
            var constantDesc = constantTable.GetDesc();
            if (constantDesc.HasValue) {
              var constantsList =
                  from handle in constantTable.GetTopLevelConstantHandles()
                  let constatns = from desc in constantTable.GetConstantDesc(handle)
                                  select desc.ToConstantInfo()
                  select constatns.ToArray();
              ret.TopLevelConstants = constantsList.ToArray();
              ret.ConstantTableCreator = constantDesc.Value.Creator.ReadAsAnsiString();
              ret.ConstantTableVersion = (int)constantDesc.Value.Version;
            }
            ret.Data = buffer.AsByteArray();
          }
          ret.ErrorMessage = msg != null ? msg.AsAnsiString() : string.Empty;

          return ret;
        } finally {
          if (buffer != null) {
            Marshal.ReleaseComObject(buffer);
            buffer = null;
          }
          if (msg != null) {
            Marshal.ReleaseComObject(msg);
            msg = null;
          }
          if (constantTable != null) {
            Marshal.ReleaseComObject(constantTable);
            constantTable = null;
          }
        }
      }

      static internal bool Succeeded(HRESULT hr) {
        return hr >= 0;
      }
    }

    public partial class CompiledEffect {
      public readonly string D3DXDllName;
      public readonly string ErrorMessage;
      public readonly string SourceCode;
      public readonly CompilerOptions CompileFlags;
      public readonly bool CompileSucceeded;
      public readonly string Creator;
      public readonly string[] Functions;
      public readonly ParameterInfo[] TopLevelParameters;

      private const int MinD3DX9Version = 32; // DirectX SDK December 2006
      private const int MaxD3DX9Version = 39; // DirectX SDK August 2008
      private const string EffectCompilerFuncName = "D3DXCreateEffectCompiler";

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
      internal delegate HRESULT D3DXCreateEffectCompilerProc(
              [MarshalAs(UnmanagedType.LPStr)]
                string pSrcData,
              uint SrcDataLen,
              [In]
                D3DXMACRO[] defines, // last element should be null+null
              ID3DXInclude include,
              D3DXSHADER flags,
              out ID3DXEffectCompiler effectCompiler,
              out ID3DXBuffer parseErrors
          );

      private bool CreateEffectCompiler(
          string source,
          CompilerOptions options,
          out ID3DXEffectCompiler effectCompiler,
          out string errorMessage
      ) {
        effectCompiler = null;
        errorMessage = string.Empty;
        return false;
      }

      private CompiledEffect(string source, CompilerOptions options) {
        D3DXDllName = string.Empty;
        for (int ver = MaxD3DX9Version; ver >= MinD3DX9Version; --ver) {
          var dllname = "d3dx9_" + Convert.ToString(ver) + ".dll";
          using (var handle = UnmanagedLibrary.LoadLibrary(dllname)) {
            if (handle != null) {
              D3DXDllName = dllname;
              break;
            }
          }
        }

        this.SourceCode = source;
        this.CompileFlags = options;
        this.CompileSucceeded = false;

        if (D3DXDllName == string.Empty) {
          this.ErrorMessage = "D3DX DLL not found";
          return;
        }

        using (var handle = UnmanagedLibrary.LoadLibrary(D3DXDllName)) {
          if (handle == null) {
            this.ErrorMessage = string.Format("LoadLibrary(\"{0}\") failed", D3DXDllName);
            return;
          }

          var errorbuf = default(ID3DXBuffer);
          var compiler = default(ID3DXEffectCompiler);
          try {
            var len = Encoding.ASCII.GetByteCount(this.SourceCode);

            var D3DXCreateEffectCompiler = handle.GetUnmanagedFunction<D3DXCreateEffectCompilerProc>(EffectCompilerFuncName);
            if (D3DXCreateEffectCompiler == null) {
              this.ErrorMessage = string.Format("GetProcAddress(module, \"{0}\") failed", EffectCompilerFuncName);
              return;
            }

            var hr = D3DXCreateEffectCompiler(this.SourceCode, (uint)len, null, IntPtr.Zero, options.ToD3DXSHADER(), out compiler, out errorbuf);
            this.ErrorMessage = errorbuf != null ? errorbuf.AsAnsiString()
                                                 : string.Empty;
            if (!D3DXUtil.Succeeded(hr)) {
              return;
            }

            this.CompileSucceeded = true;

            var effectDesc_ = compiler.GetDesc();
            if (!effectDesc_.HasValue) {
              return;
            }

            var effectDesc = effectDesc_.Value;
            var funcsions = from funcIndex in Enumerable.Range(0, (int)effectDesc.Functions)
                            let funcHandle = compiler.GetFunction((uint)funcIndex)
                            let funcDesc = compiler.GetFunctionDesc(funcHandle)
                            // funcDesc.Annotations are always empty, so we ignore them
                            select funcDesc.HasValue ? funcDesc.Value.Name.ReadAsAnsiString() : string.Empty;

            var topLevelParams = from paramIndex in Enumerable.Range(0, (int)effectDesc.Parameters)
                                 select compiler.GetParameter(IntPtr.Zero, paramIndex);

            this.Creator = effectDesc.Creator.ReadAsAnsiString();
            this.Functions = funcsions.ToArray();
            this.TopLevelParameters = topLevelParams.ToArray();

            return;
          } finally {
            if (errorbuf != null) {
              Marshal.ReleaseComObject(errorbuf);
              errorbuf = null;
            }
            if (compiler != null) {
              Marshal.ReleaseComObject(compiler);
              compiler = null;
            }
          }
        }
      }

      public CompiledShader CompileShader(string funcname, string profile, CompilerOptions options) {
        if (D3DXDllName == string.Empty) {
          return new CompiledShader { ErrorMessage = "D3DX DLL not found" };
        }

        using (var handle = UnmanagedLibrary.LoadLibrary(D3DXDllName)) {
          if (handle == null) {
            return new CompiledShader { ErrorMessage = string.Format("LoadLibrary(\"{0}\") failed", D3DXDllName) };
          }

          var errorbuf = default(ID3DXBuffer);
          var _compiler = default(ID3DXEffectCompiler);
          try {
            var len = Encoding.ASCII.GetByteCount(this.SourceCode);

            var D3DXCreateEffectCompiler = handle.GetUnmanagedFunction<D3DXCreateEffectCompilerProc>(EffectCompilerFuncName);
            if (D3DXCreateEffectCompiler == null) {
              return new CompiledShader { ErrorMessage = string.Format("GetProcAddress(module, \"{0}\") failed", EffectCompilerFuncName) };
            }

            var hr = D3DXCreateEffectCompiler(this.SourceCode, (uint)len, null, IntPtr.Zero, options.ToD3DXSHADER(), out _compiler, out errorbuf);
            if (!D3DXUtil.Succeeded(hr)) {
              return new CompiledShader {
                ErrorMessage = errorbuf != null ? errorbuf.AsAnsiString() : string.Empty
              };
            }
            return _compiler.CompileShader(funcname, profile, options);
          } finally {
            if (errorbuf != null) {
              Marshal.ReleaseComObject(errorbuf);
              errorbuf = null;
            }
            if (_compiler != null) {
              Marshal.ReleaseComObject(_compiler);
              _compiler = null;
            }
          }
        }
      }

      public static CompiledEffect FromString(string source, CompilerOptions options) {
        return new CompiledEffect(source, options);
      }
    }
  }

  namespace D3DX.Native {
    using D3DXHANDLE = IntPtr;
    using HRESULT = Int32;
    using IDirect3DDevice9 = IntPtr;
    using IDirect3DTexture9 = IntPtr;
    using LPCSTR = IntPtr;

    internal struct D3DXEFFECT_DESC {
      public LPCSTR Creator;                     // Creator string
      public uint Parameters;                    // Number of parameters
      public uint Techniques;                    // Number of techniques
      public uint Functions;                     // Number of function entrypoints
    }

    internal enum D3DXREGISTER_SET {
      D3DXRS_BOOL,
      D3DXRS_INT4,
      D3DXRS_FLOAT4,
      D3DXRS_SAMPLER,
    }

    internal enum D3DXPARAMETER_CLASS {
      D3DXPC_SCALAR,
      D3DXPC_VECTOR,
      D3DXPC_MATRIX_ROWS,
      D3DXPC_MATRIX_COLUMNS,
      D3DXPC_OBJECT,
      D3DXPC_STRUCT,
    }

    internal enum D3DXPARAMETER_TYPE {
      D3DXPT_VOID,
      D3DXPT_BOOL,
      D3DXPT_INT,
      D3DXPT_FLOAT,
      D3DXPT_STRING,
      D3DXPT_TEXTURE,
      D3DXPT_TEXTURE1D,
      D3DXPT_TEXTURE2D,
      D3DXPT_TEXTURE3D,
      D3DXPT_TEXTURECUBE,
      D3DXPT_SAMPLER,
      D3DXPT_SAMPLER1D,
      D3DXPT_SAMPLER2D,
      D3DXPT_SAMPLER3D,
      D3DXPT_SAMPLERCUBE,
      D3DXPT_PIXELSHADER,
      D3DXPT_VERTEXSHADER,
      D3DXPT_PIXELFRAGMENT,
      D3DXPT_VERTEXFRAGMENT,
      D3DXPT_UNSUPPORTED,
    }

    [Flags]
    internal enum D3DX_PARAMETER {
      SHARED = (1 << 0),
      LITERAL = (1 << 1),
      ANNOTATION = (1 << 2),
    }

    internal struct D3DXPARAMETER_DESC {
      public LPCSTR Name;                        // Parameter name
      public LPCSTR Semantic;                    // Parameter semantic
      public D3DXPARAMETER_CLASS Class;          // Class
      public D3DXPARAMETER_TYPE Type;            // Component type
      public uint Rows;                          // Number of rows
      public uint Columns;                       // Number of columns
      public uint Elements;                      // Number of array elements
      public uint Annotations;                   // Number of annotations
      public uint StructMembers;                 // Number of structure member sub-parameters
      public D3DX_PARAMETER Flags;               // D3DX_PARAMETER_* flags
      public uint Bytes;                         // Parameter size, in bytes
    }

    internal struct D3DXTECHNIQUE_DESC {
      public LPCSTR Name;                        // Technique name
      public uint Passes;                        // Number of passes
      public uint Annotations;                   // Number of annotations
    }

    internal struct D3DXPASS_DESC {
      public LPCSTR Name;                        // Pass name
      public uint Annotations;                   // Number of annotations
      public IntPtr VertexShaderFunction;        // Vertex shader function
      public IntPtr PixelShaderFunction;         // Pixel shader function
    }

    internal struct D3DXFUNCTION_DESC {
      public LPCSTR Name;                        // Function name
      public uint Annotations;                   // Number of annotations
    }

    public struct D3DXVector4 {
      public float X;
      public float Y;
      public float Z;
      public float W;
    }

    public struct D3DXMatrix16 {
      public float M11;
      public float M12;
      public float M13;
      public float M14;
      public float M21;
      public float M22;
      public float M23;
      public float M24;
      public float M31;
      public float M32;
      public float M33;
      public float M34;
      public float M41;
      public float M42;
      public float M43;
      public float M44;
    }
    internal struct D3DXCONSTANTTABLE_DESC {
      public LPCSTR Creator;                     // Creator string
      public uint Version;                       // Shader version
      public uint Constants;                     // Number of constants
    }
    internal struct D3DXCONSTANT_DESC {
      public LPCSTR Name;                        // Lft name
      public D3DXREGISTER_SET RegisterSet;       // Register set
      public uint RegisterIndex;                 // Register index
      public uint RegisterCount;                 // Number of registers occupied

      public D3DXPARAMETER_CLASS Class;          // Class
      public D3DXPARAMETER_TYPE Type;            // Component type

      public uint Rows;                          // Number of rows
      public uint Columns;                       // Number of columns
      public uint Elements;                      // Number of array elements
      public uint StructMembers;                 // Number of structure member sub-parameters

      public uint Bytes;                         // Data size, in bytes
      public IntPtr DefaultValue;                // Pointer to default value
    }

    [Flags]
    internal enum D3DXSHADER {
      NONE = 0,
      DEBUG = (1 << 0),
      SKIPVALIDATION = (1 << 1),
      SKIPOPTIMIZATION = (1 << 2),
      PACKMATRIX_ROWMAJOR = (1 << 3),
      PACKMATRIX_COLUMNMAJOR = (1 << 4),
      PARTIALPRECISION = (1 << 5),
      FORCE_VS_SOFTWARE_NOOPT = (1 << 6),
      FORCE_PS_SOFTWARE_NOOPT = (1 << 7),
      NO_PRESHADER = (1 << 8),
      AVOID_FLOW_CONTROL = (1 << 9),
      PREFER_FLOW_CONTROL = (1 << 10),
      ENABLE_BACKWARDS_COMPATIBILITY = (1 << 12),
      IEEE_STRICTNESS = (1 << 13),
      USE_LEGACY_D3DX9_31_DLL = (1 << 16),
      OPTIMIZATION_LEVEL0 = (1 << 14),
      OPTIMIZATION_LEVEL1 = 0,
      OPTIMIZATION_LEVEL2 = ((1 << 14) | (1 << 15)),
      OPTIMIZATION_LEVEL3 = (1 << 15),
    }

    public struct D3DXMACRO {
      public LPCSTR Name;
      public LPCSTR Definition;
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("8ba5fb08-5195-40e2-ac58-0d989c3a0102")]
    internal interface ID3DXBuffer {
      [PreserveSig]
      IntPtr GetBufferPointer();
      [PreserveSig]
      uint GetBufferSize();
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("EFC5557E-6265-4613-8A94-43857889EB36")]
    internal interface IDirect3DVertexShader9 {
      [PreserveSig]
      HRESULT GetDevice(out IDirect3DDevice9 device);
      [PreserveSig]
      HRESULT GetFunction(IntPtr buffer, out uint size);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("6D3BDBDC-5B02-4415-B852-CE5E8BCCB289")]
    internal interface IDirect3DPixelShader9 {
      [PreserveSig]
      HRESULT GetDevice(out IDirect3DDevice9 device);
      [PreserveSig]
      HRESULT GetFunction(IntPtr buffer, out uint size);
    }

    [ComImport]
    [Guid("51b8a949-1a31-47e6-bea0-4b30db53f1e0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID3DXEffectCompiler {
      //[PreserveSig] HRESULT QueryInterface;
      //[PreserveSig] HRESULT AddRef;
      //[PreserveSig] HRESULT Release;

      [PreserveSig]
      HRESULT GetDesc(out D3DXEFFECT_DESC desc);
      [PreserveSig]
      HRESULT GetParameterDesc(D3DXHANDLE parameterHandle, out D3DXPARAMETER_DESC desc);
      [PreserveSig]
      HRESULT GetTechniqueDesc(D3DXHANDLE techniqueHandle, out D3DXTECHNIQUE_DESC desc);
      [PreserveSig]
      HRESULT GetPassDesc(D3DXHANDLE passHandle, out D3DXPASS_DESC desc);
      [PreserveSig]
      HRESULT GetFunctionDesc(D3DXHANDLE functionHandle, out D3DXFUNCTION_DESC desc);

      // Handle operations

      [PreserveSig]
      D3DXHANDLE GetParameter(D3DXHANDLE parameterHandle, uint index);
      [PreserveSig]
      D3DXHANDLE GetParameterByName(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      D3DXHANDLE GetParameterBySemantic(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPStr)]string semantic);
      [PreserveSig]
      D3DXHANDLE GetParameterElement(D3DXHANDLE parameterHandle, uint index);
      [PreserveSig]
      D3DXHANDLE GetTechnique(uint index);
      [PreserveSig]
      D3DXHANDLE GetTechniqueByName([MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      D3DXHANDLE GetPass(D3DXHANDLE techniqueHandle, uint index);
      [PreserveSig]
      D3DXHANDLE GetPassByName(D3DXHANDLE techniqueHandle, [MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      D3DXHANDLE GetFunction(uint index);
      [PreserveSig]
      D3DXHANDLE GetFunctionByName([MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      D3DXHANDLE GetAnnotation(D3DXHANDLE objectHandle, uint index);
      [PreserveSig]
      D3DXHANDLE GetAnnotationByName(D3DXHANDLE objectHandle, [MarshalAs(UnmanagedType.LPStr)]string name);

      [PreserveSig]
      HRESULT SetValue(D3DXHANDLE parameterHandle, IntPtr data, uint bytes);
      [PreserveSig]
      HRESULT GetValue(D3DXHANDLE parameterHandle, IntPtr data, uint bytes);
      [PreserveSig]
      HRESULT SetBool(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.Bool)]bool b);
      [PreserveSig]
      HRESULT GetBool(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.Bool)]out bool b);
      [PreserveSig]
      HRESULT SetBoolArray(D3DXHANDLE parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Bool, SizeParamIndex = 2)]bool[] b, uint count);
      [PreserveSig]
      HRESULT GetBoolArray(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Bool, SizeParamIndex = 2)]out bool[] b, uint count);
      [PreserveSig]
      HRESULT SetInt(D3DXHANDLE parameterHandle, int b);
      [PreserveSig]
      HRESULT GetInt(D3DXHANDLE parameterHandle, out int b);
      [PreserveSig]
      HRESULT SetIntArray(D3DXHANDLE parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]int[] n, uint count);
      [PreserveSig]
      HRESULT GetIntArray(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out int[] n, uint count);
      [PreserveSig]
      HRESULT SetFloat(D3DXHANDLE parameterHandle, float b);
      [PreserveSig]
      HRESULT GetFloat(D3DXHANDLE parameterHandle, out float b);
      [PreserveSig]
      HRESULT SetFloatArray(D3DXHANDLE parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]float[] n, uint count);
      [PreserveSig]
      HRESULT GetFloatArray(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out float[] n, uint count);
      [PreserveSig]
      HRESULT SetVector(D3DXHANDLE parameterHandle, [In]ref D3DXVector4 b);
      [PreserveSig]
      HRESULT GetVector(D3DXHANDLE parameterHandle, out D3DXVector4 b);
      [PreserveSig]
      HRESULT SetVectorArray(D3DXHANDLE parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]D3DXVector4[] n, uint count);
      [PreserveSig]
      HRESULT GetVectorArray(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out D3DXVector4[] n, uint count);
      [PreserveSig]
      HRESULT SetMatrix(D3DXHANDLE parameterHandle, [In]ref D3DXMatrix16 b);
      [PreserveSig]
      HRESULT GetMatrix(D3DXHANDLE parameterHandle, out D3DXMatrix16 b);
      [PreserveSig]
      HRESULT SetMatrixArray(D3DXHANDLE parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]D3DXMatrix16[] n, uint count);
      [PreserveSig]
      HRESULT GetMatrixArray(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out D3DXMatrix16[] n, uint count);
      [PreserveSig]
      HRESULT SetMatrixPointerArray(D3DXHANDLE parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]IntPtr[] n, uint count);
      [PreserveSig]
      HRESULT GetMatrixPointerArray(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out IntPtr[] n, uint count);
      [PreserveSig]
      HRESULT SetMatrixTranspose(D3DXHANDLE parameterHandle, [In]ref D3DXMatrix16 b);
      [PreserveSig]
      HRESULT GetMatrixTranspose(D3DXHANDLE parameterHandle, out D3DXMatrix16 b);
      [PreserveSig]
      HRESULT SetMatrixTransposeArray(D3DXHANDLE parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]D3DXMatrix16[] n, uint count);
      [PreserveSig]
      HRESULT GetMatrixTransposeArray(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out D3DXMatrix16[] n, uint count);
      [PreserveSig]
      HRESULT SetMatrixTransposePointerArray(D3DXHANDLE parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]IntPtr[] n, uint count);
      [PreserveSig]
      HRESULT GetMatrixTransposePointerArray(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out IntPtr[] n, uint count);
      [PreserveSig]
      HRESULT SetString(D3DXHANDLE parameterHandle, [In][MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      HRESULT GetString(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.LPStr)]out string name);
      [PreserveSig]
      HRESULT SetTexture(D3DXHANDLE parameterHandle, IDirect3DTexture9 texture);
      [PreserveSig]
      HRESULT GetTexture(D3DXHANDLE parameterHandle, out IDirect3DTexture9 texture);
      [PreserveSig]
      HRESULT GetPixelShader(D3DXHANDLE parameterHandle, out IDirect3DPixelShader9 pixelShader);
      [PreserveSig]
      HRESULT GetVertexShader(D3DXHANDLE parameterHandle, out IDirect3DVertexShader9 vertexShader);

      HRESULT SetArrayRange(D3DXHANDLE parameterHandle, uint start, uint end);

      //// ID3DXBaseEffect

      //// Parameter sharing, specialization, and information
      [PreserveSig]
      HRESULT SetLiteral(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.Bool)]bool literal);
      [PreserveSig]
      HRESULT GetLiteral(D3DXHANDLE parameterHandle, [MarshalAs(UnmanagedType.Bool)]out bool literal);

      //// Compilation
      [PreserveSig]
      HRESULT CompileEffect(D3DXSHADER flags, out ID3DXBuffer effect, out ID3DXBuffer errorMsgs);
      [PreserveSig]
      HRESULT CompileShader(D3DXHANDLE functionHandle, [MarshalAs(UnmanagedType.LPStr)]string target, D3DXSHADER flags, out ID3DXBuffer shader, out ID3DXBuffer errorMsgs, out ID3DXConstantTable constantTable);
    }

    [ComImport]
    [Guid("51b8a949-1a31-47e6-bea0-4b30db53f1e0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID3DXEffectCompiler_custom {
      //[PreserveSig] HRESULT QueryInterface;
      //[PreserveSig] HRESULT AddRef;
      //[PreserveSig] HRESULT Release;

      [PreserveSig]
      HRESULT GetDesc(out D3DXEFFECT_DESC desc);
      [PreserveSig]
      HRESULT GetParameterDesc(D3DXHANDLE parameterHandle, out D3DXPARAMETER_DESC desc);
      [PreserveSig]
      HRESULT GetTechniqueDesc(D3DXHANDLE techniqueHandle, out D3DXTECHNIQUE_DESC desc);
      [PreserveSig]
      HRESULT GetPassDesc(D3DXHANDLE passHandle, out D3DXPASS_DESC desc);
      [PreserveSig]
      HRESULT GetFunctionDesc(D3DXHANDLE functionHandle, out D3DXFUNCTION_DESC desc);

      // Handle operations

      [PreserveSig]
      D3DXHANDLE GetParameter([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, uint index);
      [PreserveSig]
      D3DXHANDLE GetParameterByName([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      D3DXHANDLE GetParameterBySemantic([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPStr)]string semantic);
      [PreserveSig]
      D3DXHANDLE GetParameterElement([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, uint index);
      [PreserveSig]
      D3DXHANDLE GetTechnique(uint index);
      [PreserveSig]
      D3DXHANDLE GetTechniqueByName([MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      D3DXHANDLE GetPass([MarshalAs(UnmanagedType.LPStr)] string techniqueHandle, uint index);
      [PreserveSig]
      D3DXHANDLE GetPassByName([MarshalAs(UnmanagedType.LPStr)] string techniqueHandle, [MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      D3DXHANDLE GetFunction(uint index);
      [PreserveSig]
      D3DXHANDLE GetFunctionByName([MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      D3DXHANDLE GetAnnotation(D3DXHANDLE objectHandle, uint index);
      [PreserveSig]
      D3DXHANDLE GetAnnotationByName(D3DXHANDLE objectHandle, [MarshalAs(UnmanagedType.LPStr)]string name);

      [PreserveSig]
      HRESULT SetValue([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, IntPtr data, uint bytes);
      [PreserveSig]
      HRESULT GetValue([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, IntPtr data, uint bytes);
      [PreserveSig]
      HRESULT SetBool([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.Bool)]bool b);
      [PreserveSig]
      HRESULT GetBool([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.Bool)]out bool b);
      [PreserveSig]
      HRESULT SetBoolArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Bool, SizeParamIndex = 2)]bool[] b, uint count);
      [PreserveSig]
      HRESULT GetBoolArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Bool, SizeParamIndex = 2)]out bool[] b, uint count);
      [PreserveSig]
      HRESULT SetInt([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, int b);
      [PreserveSig]
      HRESULT GetInt([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, out int b);
      [PreserveSig]
      HRESULT SetIntArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]int[] n, uint count);
      [PreserveSig]
      HRESULT GetIntArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out int[] n, uint count);
      [PreserveSig]
      HRESULT SetFloat([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, float b);
      [PreserveSig]
      HRESULT GetFloat([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, out float b);
      [PreserveSig]
      HRESULT SetFloatArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]float[] n, uint count);
      [PreserveSig]
      HRESULT GetFloatArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out float[] n, uint count);
      [PreserveSig]
      HRESULT SetVector([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In]ref D3DXVector4 b);
      [PreserveSig]
      HRESULT GetVector([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, out D3DXVector4 b);
      [PreserveSig]
      HRESULT SetVectorArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]D3DXVector4[] n, uint count);
      [PreserveSig]
      HRESULT GetVectorArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out D3DXVector4[] n, uint count);
      [PreserveSig]
      HRESULT SetMatrix([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In]ref D3DXMatrix16 b);
      [PreserveSig]
      HRESULT GetMatrix([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, out D3DXMatrix16 b);
      [PreserveSig]
      HRESULT SetMatrixArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]D3DXMatrix16[] n, uint count);
      [PreserveSig]
      HRESULT GetMatrixArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out D3DXMatrix16[] n, uint count);
      [PreserveSig]
      HRESULT SetMatrixPointerArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]IntPtr[] n, uint count);
      [PreserveSig]
      HRESULT GetMatrixPointerArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out IntPtr[] n, uint count);
      [PreserveSig]
      HRESULT SetMatrixTranspose([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In]ref D3DXMatrix16 b);
      [PreserveSig]
      HRESULT GetMatrixTranspose([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, out D3DXMatrix16 b);
      [PreserveSig]
      HRESULT SetMatrixTransposeArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]D3DXMatrix16[] n, uint count);
      [PreserveSig]
      HRESULT GetMatrixTransposeArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out D3DXMatrix16[] n, uint count);
      [PreserveSig]
      HRESULT SetMatrixTransposePointerArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]IntPtr[] n, uint count);
      [PreserveSig]
      HRESULT GetMatrixTransposePointerArray([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]out IntPtr[] n, uint count);
      [PreserveSig]
      HRESULT SetString([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [In][MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      HRESULT GetString([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.LPStr)]out string name);
      [PreserveSig]
      HRESULT SetTexture([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, IDirect3DTexture9 texture);
      [PreserveSig]
      HRESULT GetTexture([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, out IDirect3DTexture9 texture);
      [PreserveSig]
      HRESULT GetPixelShader([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, out IDirect3DPixelShader9 pixelShader);
      [PreserveSig]
      HRESULT GetVertexShader([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, out IDirect3DVertexShader9 vertexShader);

      [PreserveSig]
      HRESULT SetArrayRange([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, uint start, uint end);

      //// ID3DXBaseEffect

      //// Parameter sharing, specialization, and information
      [PreserveSig]
      HRESULT SetLiteral([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.Bool)]bool literal);
      [PreserveSig]
      HRESULT GetLiteral([MarshalAs(UnmanagedType.LPStr)] string parameterHandle, [MarshalAs(UnmanagedType.Bool)]out bool literal);

      //// Compilation
      [PreserveSig]
      HRESULT CompileEffect(D3DXSHADER flags, out ID3DXBuffer effect, out ID3DXBuffer errorMsgs);
      [PreserveSig]
      HRESULT CompileShader([MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)]string target, D3DXSHADER flags, out ID3DXBuffer shader, out ID3DXBuffer errorMsgs, out ID3DXConstantTable constantTable);
    }


    [ComImport]
    [Guid("ab3c758f-093e-4356-b762-4db18f1b3a01")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface ID3DXConstantTable {
      //[PreserveSig] HRESULT QueryInterface;
      //[PreserveSig] HRESULT AddRef;
      //[PreserveSig] HRESULT Release;

      // Buffer
      [PreserveSig]
      IntPtr GetBufferPointer();
      [PreserveSig]
      uint GetBufferSize();

      // Descs
      [PreserveSig]
      HRESULT GetDesc(out D3DXCONSTANTTABLE_DESC desc);
      [PreserveSig]
      HRESULT GetConstantDesc(D3DXHANDLE constant, [In][Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Bool, SizeParamIndex = 2)] D3DXCONSTANT_DESC[] desc, [In][Out]ref uint count);
      [PreserveSig]
      uint GetSamplerIndex(D3DXHANDLE constant);

      // Handle operations
      [PreserveSig]
      D3DXHANDLE GetConstant(D3DXHANDLE constant, uint index);
      [PreserveSig]
      D3DXHANDLE GetConstantByName(D3DXHANDLE constant, [Out][MarshalAs(UnmanagedType.LPStr)]string name);
      [PreserveSig]
      D3DXHANDLE GetConstantElement(D3DXHANDLE constant, uint index);

      // Set Constants
      [PreserveSig]
      HRESULT SetDefaults(IntPtr device);
      [PreserveSig]
      HRESULT SetValue(IntPtr device, D3DXHANDLE constant, IntPtr data, uint bytes);
      [PreserveSig]
      HRESULT SetBool(IntPtr device, D3DXHANDLE constant, [MarshalAs(UnmanagedType.Bool)]bool b);
      [PreserveSig]
      HRESULT SetBoolArray(IntPtr device, D3DXHANDLE constant, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Bool, SizeParamIndex = 2)] bool[] array, uint count);
      [PreserveSig]
      HRESULT SetInt(IntPtr device, D3DXHANDLE constant, int n);
      [PreserveSig]
      HRESULT SetIntArray(IntPtr device, D3DXHANDLE constant, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 2)] int[] array, uint count);
      [PreserveSig]
      HRESULT SetFloat(IntPtr device, D3DXHANDLE constant, float f);
      [PreserveSig]
      HRESULT SetFloatArray(IntPtr device, D3DXHANDLE constant, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4, SizeParamIndex = 2)] float[] array, uint count);
      [PreserveSig]
      HRESULT SetVector(IntPtr device, D3DXHANDLE constant, [In] ref D3DXVector4 f);
      [PreserveSig]
      HRESULT SetVectorArray(IntPtr device, D3DXHANDLE constant, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeParamIndex = 2)] D3DXVector4[] array, uint count);
      [PreserveSig]
      HRESULT SetMatrix(IntPtr device, D3DXHANDLE constant, [In] ref D3DXMatrix16 matrix);
      [PreserveSig]
      HRESULT SetMatrixArray(IntPtr device, D3DXHANDLE constant, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] D3DXMatrix16[] array, uint count);
      [PreserveSig]
      HRESULT SetMatrixPointerArray(IntPtr device, D3DXHANDLE constant, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeParamIndex = 2)] D3DXMatrix16*[] array, uint count);
      [PreserveSig]
      HRESULT SetMatrixTranspose(IntPtr device, D3DXHANDLE constant, [In] ref D3DXMatrix16 matrix);
      [PreserveSig]
      HRESULT SetMatrixTransposeArray(IntPtr device, D3DXHANDLE constant, [In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] D3DXMatrix16[] array, uint count);
      [PreserveSig]
      HRESULT SetMatrixTransposePointerArray(IntPtr device, D3DXHANDLE constant, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeParamIndex = 2)] D3DXMatrix16*[] array, uint count);
    }
  }

  namespace Win32 {
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;
    using Microsoft.Win32.SafeHandles;

    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    internal sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid {
      private SafeLibraryHandle() : base(true) { }

      protected override bool ReleaseHandle() {
        return NativeMethods.FreeLibrary(handle);
      }
    }

    internal static class NativeMethods {
      const string Kernel32DLL = "kernel32";

      [DllImport(Kernel32DLL, EntryPoint = "LoadLibraryW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
      public static extern SafeLibraryHandle LoadLibrary(string fileName);

      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [DllImport(Kernel32DLL, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool FreeLibrary(IntPtr hModule);

      [DllImport(Kernel32DLL, CharSet = CharSet.Ansi, ExactSpelling = true)]
      public static extern IntPtr GetProcAddress(SafeLibraryHandle hModule, string procname);
    }


    /// <summary>
    /// Utility class to wrap an unmanaged DLL and be responsible for freeing it.
    /// </summary>
    /// <remarks>This is a managed wrapper over the native LoadLibrary, GetProcAddress, and
    /// FreeLibrary calls.
    /// original: http://blogs.msdn.com/jmstall/archive/2007/01/06/Typesafe-GetProcAddress.aspx
    /// </remarks>
    public sealed class UnmanagedLibrary : IDisposable {
      private UnmanagedLibrary(SafeLibraryHandle handle) {
        if (handle.IsInvalid) {
          throw new Exception("module handle is invalid");
        }
        m_hLibrary = handle;
      }

      /// <summary>
      /// Constructor to load a dll and be responible for freeing it.
      /// </summary>
      /// <param name="fileName">full path name of dll to load</param>
      /// <exception cref="System.IO.FileNotFoundException">if fileName can't be found</exception>
      /// <remarks>Throws exceptions on failure. Most common failure would be file-not-found, or
      /// that the file is not a  loadable image.</remarks>
      public static UnmanagedLibrary LoadLibrary(string fileName) {
        SafeLibraryHandle handle = NativeMethods.LoadLibrary(fileName);
        int hr = Marshal.GetHRForLastWin32Error();
        if (!handle.IsInvalid) {
          return new UnmanagedLibrary(handle);
        }
        return null;
      }

      /// <summary>
      /// Dynamically lookup a function in the dll via kernel32!GetProcAddress.
      /// </summary>
      /// <param name="functionName">raw name of the function in the export table.</param>
      /// <returns>null if function is not found. Else a delegate to the unmanaged function.
      /// </returns>
      /// <remarks>GetProcAddress results are valid as long as the dll is not yet unloaded. This
      /// is very very dangerous to use since you need to ensure that the dll is not unloaded
      /// until after you're done with any objects implemented by the dll. For example, if you
      /// get a delegate that then gets an IUnknown implemented by this dll,
      /// you can not dispose this library until that IUnknown is collected. Else, you may free
      /// the library and then the CLR may call release on that IUnknown and it will crash.</remarks>
      public TDelegate GetUnmanagedFunction<TDelegate>(string functionName) where TDelegate : class {
        IntPtr p = NativeMethods.GetProcAddress(m_hLibrary, functionName);

        // Failure is a common case, especially for adaptive code.
        if (p == IntPtr.Zero) {
          return null;
        }
        Delegate function = Marshal.GetDelegateForFunctionPointer(p, typeof(TDelegate));

        // Ideally, we'd just make the constraint on TDelegate be
        // System.Delegate, but compiler error CS0702 (constrained can't be System.Delegate)
        // prevents that. So we make the constraint system.object and do the cast from object-->TDelegate.
        object o = function;

        return (TDelegate)o;
      }

      #region IDisposable Members
      /// <summary>
      /// Call FreeLibrary on the unmanaged dll. All function pointers
      /// handed out from this class become invalid after this.
      /// </summary>
      /// <remarks>This is very dangerous because it suddenly invalidate
      /// everything retrieved from this dll. This includes any functions
      /// handed out via GetProcAddress, and potentially any objects returned
      /// from those functions (which may have an implemention in the
      /// dll).
      /// </remarks>
      public void Dispose() {
        if (!m_hLibrary.IsClosed) {
          m_hLibrary.Close();
        }
      }

      // Unmanaged resource. CLR will ensure SafeHandles get freed, without requiring a finalizer on this class.
      readonly SafeLibraryHandle m_hLibrary;

      #endregion
    } // UnmanagedLibrary
  }
}