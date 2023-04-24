[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Values
open LLVMSharp.Interop
open Utility

let llvmTypeOf value = { lltype = LLVM.TypeOf value.llvalue }
let classifyValue value =
    let inline is x = not (isNullPtr x)
    if value.llvalue.Handle = 0n then ValueKind.NullValue
    else
        let value = value.llvalue
        match () with
        | _ when is (LLVM.IsABlockAddress value) -> ValueKind.BlockAddress
        | _ when is (LLVM.IsAConstantAggregateZero value) -> ValueKind.ConstantAggregateZero
        | _ when is (LLVM.IsAConstantArray value) -> ValueKind.ConstantArray
        | _ when is (LLVM.IsAConstantDataVector value) -> ValueKind.ConstantDataVector
        | _ when is (LLVM.IsAConstantExpr value) -> ValueKind.ConstantExpr
        | _ when is (LLVM.IsAConstantFP value) -> ValueKind.ConstantFP
        | _ when is (LLVM.IsAConstantInt value) -> ValueKind.ConstantInt
        | _ when is (LLVM.IsAConstantPointerNull value) -> ValueKind.ConstantPointerNull
        | _ when is (LLVM.IsAConstantStruct value) -> ValueKind.ConstantStruct
        | _ when is (LLVM.IsAConstantVector value) -> ValueKind.ConstantVector
        | _ when is (LLVM.IsAInstruction value) -> ValueKind.Instruction ( LLVM.GetInstructionOpcode value |> int32 |> Opcode.unsafeCreate)
        | _ when is (LLVM.IsAFunction value) -> ValueKind.Function
        | _ when is (LLVM.IsAGlobalAlias value) -> ValueKind.GlobalAlias
        | _ when is (LLVM.IsAGlobalIFunc value) -> ValueKind.GlobalIFunc
        | _ when is (LLVM.IsAGlobalVariable value) -> ValueKind.GlobalVariable
        | _ when is (LLVM.IsAArgument value) -> ValueKind.Argument
        | _ when is (LLVM.IsABasicBlock value) -> ValueKind.BasicBlock
        | _ when is (LLVM.IsAInlineAsm value) -> ValueKind.InlineAsm
        | _ when is (LLVM.IsAMDNode value) -> ValueKind.MDNode
        | _ when is (LLVM.IsAMDString value) -> ValueKind.MDString
        | _ when is (LLVM.IsAUndefValue value) -> ValueKind.UndefValue
        | _ when is (LLVM.IsAPoisonValue value) -> ValueKind.PoisonValue
        | _ -> failwith "Unknown value class"

let valueName value =
    let ptr = LLVM.GetValueName(value.llvalue)
    CSharpStringOfSignedCString ptr 
let setValueName name value =
    name
    |> withUTF8 (fun name -> LLVM.SetValueName(value.llvalue, name))
let dumpValue value =
    LLVM.DumpValue(value.llvalue)
let stringOfLLValue value =
    let cstring =
        value.llvalue 
        |> LLVM.PrintValueToString
    let csString = CSharpStringOfSignedCString cstring
    LLVM.DisposeMessage cstring
    csString
let replaceAllUsesWith old ``new`` =
    LLVM.ReplaceAllUsesWith(old.llvalue, ``new``.llvalue)
