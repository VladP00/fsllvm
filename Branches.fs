[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Branches
open LLVMSharp.Interop
[<Struct>]
type Branch =
    | Conditional of first: llvalue * second: llbasicblock * third: llbasicblock
    | Unconditional of llbasicblock
    
let isConditional value =
    (LLVM.IsConditional (value.llvalue)) <> 0

let condition value = { llvalue = LLVM.GetCondition value.llvalue }
let setCondition value condition =
    LLVM.SetCondition(value.llvalue, condition.llvalue)
let getBranch llv =
    if classifyValue llv <> ValueKind.Instruction Opcode.Br then
        ValueNone
    else if isConditional llv then
        ValueSome (Conditional (condition llv, successor llv 0u, successor llv 1u))
    else
        ValueSome (Unconditional (successor llv 0u))