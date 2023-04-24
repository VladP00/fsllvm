[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.IntegerTypes
open LLVMSharp.Interop
let i1Type context = { lltype = LLVM.Int1TypeInContext(context.llcontext) }
let i8Type context = { lltype = LLVM.Int8TypeInContext(context.llcontext) }
let i16Type context = { lltype = LLVM.Int16TypeInContext(context.llcontext) }
let i32Type context = { lltype = LLVM.Int32TypeInContext(context.llcontext) }
let i64Type context = { lltype = LLVM.Int64TypeInContext(context.llcontext) }
let i128Type context = { lltype = LLVM.Int128TypeInContext(context.llcontext) }
let intPtrType context targetData = { lltype = LLVM.IntPtrTypeInContext (context.llcontext, targetData.lltargetdata) }
let integerBitWidth lltype = LLVM.GetIntTypeWidth (lltype.lltype)
let integerType context bitWidth = { lltype = LLVM.IntTypeInContext(context.llcontext, bitWidth) }
