[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Constant
open LLVMSharp.Interop
open Microsoft.FSharp.NativeInterop
let isConstant value =
    not (isNullPtr (LLVM.IsAConstant value.llvalue))

let constNull lltype =
    { llvalue = LLVM.ConstNull(lltype.lltype) }

let constAllOnes lltype =
    { llvalue = LLVM.ConstAllOnes(lltype.lltype) }

let constPointerNull lltype =
    { llvalue = LLVM.ConstPointerNull(lltype.lltype) }

let undef lltype = { llvalue = LLVM.GetUndef(lltype.lltype) }
let poison lltype = { llvalue = LLVM.GetPoison(lltype.lltype) }

let ``void``() = { llvalue = LLVMValueRef 0n }
let isVoid self = self.llvalue.Handle = 0n
let isNull llvalue = LLVM.IsNull llvalue.llvalue <> 0
let isUndef llvalue = (LLVM.IsAUndefValue llvalue.llvalue) <> NativePtr.nullPtr
let isPoison llvalue = LLVM.IsPoison llvalue.llvalue <> 0
let constExprOpCode value =
    if (LLVM.IsAConstantExpr (value.llvalue) <> NativePtr.nullPtr) then
        Opcode.unsafeCreate (int32 (LLVM.GetConstOpcode(value.llvalue)))
    else Unchecked.defaultof<Opcode.t>