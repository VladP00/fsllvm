[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Users
open LLVMSharp.Interop
open Microsoft.FSharp.NativeInterop
open System
let operand value (index: int) =
    { llvalue = LLVM.GetOperand(value.llvalue, uint32 index) }

let operandUse value (index: int) =
    { lluse = LLVM.GetOperandUse(value.llvalue, uint32 index) }

let setOperand value (index: int) operand =
    LLVM.SetOperand(value.llvalue, uint32 index, operand.llvalue)

let numOperands value = LLVM.GetNumOperands(value.llvalue)
let indices value: int array =
    initUnsafe (int (LLVM.GetNumIndices value.llvalue)) (fun (ptrToInt: int nativeptr) count ->
       let tmp = (LLVM.GetIndices(value.llvalue))
       let source = Span<int>(tmp |> NativePtr.toVoidPtr, count)
       let dest = Span<int>(ptrToInt |> NativePtr.toVoidPtr, count)
       
       source.CopyTo dest 
    )