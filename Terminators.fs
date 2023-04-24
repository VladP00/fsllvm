[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Terminators
open FSharp.NativeInterop
open LLVMSharp.Interop
let isTerminator value =
    not (NativePtr.isNullPtr (LLVM.IsATerminatorInst (value.llvalue)))
let successor term index =
    { llbasicblock = LLVM.GetSuccessor(term.llvalue, index) }
let setSuccessor value index block =
    LLVM.SetSuccessor(value.llvalue, index, block.llbasicblock)
let numSuccessors value =
    LLVM.GetNumSuccessors value.llvalue
let successors value =
    let n = numSuccessors value
    Array.init (int n) (fun i -> successor value (uint32 i))
let inline iterSuccessors ([<InlineIfLambda>] f) value =
    let n = numSuccessors value
    let rec loop = function
        | struct(0u, _) -> ()
        | struct(remaining, index) -> f (successor value index); loop struct(remaining - 1u, index + 1u)
    in loop struct(n, 0u)
let inline foldSuccessors ([<InlineIfLambda>] folder) value state =
    let n = numSuccessors value
    let rec loop = function
        | struct(0u, _, state) -> state
        | struct(remaining, index, state) ->
            let element = successor value index 
            loop struct(remaining - 1u, index + 1u, (folder element state))
    in loop struct(n, 0u, state)

