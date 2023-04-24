[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Intrinsics

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

