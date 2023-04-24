[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Uses
open LLVMSharp.Interop
open Utility

let useBegin value =
    let ptr = LLVM.GetFirstUse(value.llvalue)
    if (isNullPtr ptr) then ValueNone
    else ValueSome({ lluse = ptr })
let useSucc ``use`` =
    ptrToOption (fun (ptr: nativeptr<LLVMOpaqueUse>) -> {lluse = ptr}) (LLVM.GetNextUse(``use``.lluse))
let user ``use`` = { llvalue = LLVM.GetUser ``use``.lluse }
let usedValue ``use`` = { llvalue = LLVM.GetUsedValue ``use``.lluse }
let inline iterUses ([<InlineIfLambda>] f) value =
    let rec aux = function
        | ValueNone -> ()
        | ValueSome u ->
            f u
            aux (useSucc u)
    in aux (useBegin value)
let inline foldUses ([<InlineIfLambda>] f) initial value =
    let rec aux state = function
        | ValueNone -> state
        | ValueSome u -> aux (f state u) (useSucc u)
    in aux initial (useBegin value)
let inline foldBackUses ([<InlineIfLambda>] f) value initial =
    let rec aux = function
        | struct(ValueNone, state) -> state
        | struct(ValueSome u, state) -> f u (aux struct((useSucc u), state))
    in aux struct((useBegin value), initial)
