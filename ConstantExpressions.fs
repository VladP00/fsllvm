[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.ConstantExpressions

open System.Runtime.InteropServices
open System.Security
open LLVMSharp.Interop



[<DllImport("libLLVM", EntryPoint = "LLVMAlignOf", CallingConvention = CallingConvention.Cdecl, SetLastError = false); SuppressUnmanagedCodeSecurity>]
extern llvalue alignOf(lltype t)
[<DllImport("libLLVM", EntryPoint = "LLVMSizeOf", CallingConvention = CallingConvention.Cdecl, SetLastError = false); SuppressUnmanagedCodeSecurity>]
extern llvalue sizeOf(lltype t);

[<DllImport("libLLVM", EntryPoint = "LLVMConstNeg", CallingConvention = CallingConvention.Cdecl, SetLastError = false); SuppressUnmanagedCodeSecurity>]
extern llvalue constNeg(llvalue v);


