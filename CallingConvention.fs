[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.CallingConvention

open LLVMSharp.Interop

let swiftCC = int LLVMCallConv.LLVMSwiftCallConv
let cdecl = int LLVMCallConv.LLVMCCallConv
let fastCC = int LLVMCallConv.LLVMFastCallConv
let swiftTailCC = 20
let tailCC = 18