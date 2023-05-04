    [<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Instructions

open LLVMSharp.Interop
open Microsoft.FSharp.NativeInterop

let instrParent instruction =
    { llbasicblock = LLVM.GetInstructionParent(instruction.llvalue) }

let firstInstruction basicBlock =
    match LLVM.GetFirstInstruction(basicBlock.llbasicblock) with
    | NonNull p -> ValueSome ({ llvalue = p })
    | _ -> ValueNone
    
let lastInstruction basicBlock =
    match LLVM.GetLastInstruction(basicBlock.llbasicblock) with
    | NonNull p -> ValueSome ({ llvalue = p })
    | _ -> ValueNone
    

let hasMetadata instruction =
    LLVM.HasMetadata (instruction.llvalue) <> 0

let metadata instruction (kind: llmdkind) =
    LLVM.GetMetadata(instruction.llvalue, uint32 kind)
    |> ptrToOption (fun ptr -> { llvalue = ptr })

let setMetadata instruction (kind: llmdkind) metadata =
    LLVM.SetMetadata(instruction.llvalue, uint32 kind, metadata.llvalue)

let clearMetadata instruction (kind: llmdkind) =
    LLVM.SetMetadata(instruction.llvalue, uint32 kind, NativePtr.nullPtr)
    
