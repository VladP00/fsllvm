[<AutoOpen>]
module FSharp.llvm.Params

open LLVMSharp.Interop

let params llfunction =
    let count = LLVM.CountParams(llfunction.llvalue)
    initUnsafe<_, llvalue> (int count) (fun ptr _ ->
        LLVM.GetParams(llfunction.llvalue, ptr)
    )
let param llfunction index =
    LLVM.GetParam (llfunction.llvalue, uint32 index)
    |> fun x -> { llvalue = x }

