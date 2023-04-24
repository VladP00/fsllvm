[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.RealTypes
open LLVMSharp.Interop

let floatType context = { lltype = LLVM.FloatTypeInContext(context.llcontext) }
let doubleType context = { lltype = LLVM.DoubleTypeInContext(context.llcontext) }
let x86FP80Type context = { lltype = LLVM.X86FP80TypeInContext(context.llcontext) }
let FP128Type context = { lltype = LLVM.FP128TypeInContext(context.llcontext) }
let PPCFP128Type context = { lltype = LLVM.PPCFP128TypeInContext(context.llcontext) }
