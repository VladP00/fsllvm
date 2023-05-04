[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Intrinsics

open System.Linq
open LLVMSharp.Interop

let lookupID name =
    
    name |> 
    withUTF8Lengthed (fun name length ->
        LLVM.LookupIntrinsicID(name, unativeint length)
    )

let getIntrinsicDeclaration llmodule id parameterTypes =
    parameterTypes
    |> pinningAs<lltype, _, _>(fun paramTypes count ->
        LLVM.GetIntrinsicDeclaration(llmodule.llmodule, id, paramTypes, unativeint count)
     )
    |> fun x -> { llvalue = x }

let gcRoot llmodule  =
    let context = { llcontext = llmodule.llmodule.Context }
    let id = lookupID "llvm.gcroot"
    let i8 = i8Type context 
    in
        getIntrinsicDeclaration llmodule id [|pointerType (pointerType(i8)); pointerType(i8) |]

let gcWrite llmodule =
    let context = { llcontext = llmodule.llmodule.Context }
    let id = lookupID "llvm.gcwrite"
    let i8 = i8Type context
    in
        getIntrinsicDeclaration llmodule id [|pointerType (i8); pointerType(i8); pointerType(pointerType i8)|]
let gcRead llmodule =
    let context = { llcontext = llmodule.llmodule.Context }
    let id = lookupID "llvm.gcread"
    let i8 = i8Type context
    in
        getIntrinsicDeclaration llmodule id [|pointerType (i8); pointerType(pointerType i8)|]