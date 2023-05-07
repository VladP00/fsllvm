[<AutoOpen>]
module FSharp.llvm.Params

open LLVMSharp.Interop
open Microsoft.FSharp.NativeInterop

let params llfunction =
    let count = LLVM.CountParams(llfunction.llvalue)
    initUnsafe<_, llvalue> (int count) (fun ptr _ ->
        LLVM.GetParams(llfunction.llvalue, ptr)
    )
let param llfunction index =
    LLVM.GetParam (llfunction.llvalue, uint32 index)
    |> fun x -> { llvalue = x }

let foldParams (folder) initial llfunction =
    let rec aux acc = function
        | param when isNullPtr param -> acc
        | param ->
            aux (folder acc { llvalue = LLVMValueRef(param |> NativePtr.toNativeInt) }) (LLVM.GetNextParam param)
    in aux initial (LLVM.GetFirstParam llfunction.llvalue )

let iterParams (action) llfunction =
    let rec aux = function
        | param when isNullPtr param -> ()
        | param ->
            action { llvalue = LLVMValueRef(param |> NativePtr.toNativeInt) }
            aux (LLVM.GetNextParam param)
    in aux (LLVM.GetFirstParam llfunction.llvalue )
            