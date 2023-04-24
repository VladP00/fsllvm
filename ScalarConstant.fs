[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.ScalarConstant

open LLVMSharp.Interop

let constInt lltype int isSigned = { llvalue = LLVM.ConstInt(lltype.lltype, int, if isSigned then 1 else 0) }
let constIntOfString lltype string radix =
    string
    |> withUTF8 (fun string ->
        LLVM.ConstIntOfString(lltype.lltype, string, radix)
    )
    |> fun x -> { llvalue = x }

let constFloat lltype value = { llvalue = LLVM.ConstReal(lltype.lltype, value) }
let constFloatOfString lltype string =
    string
    |> withUTF8 (fun string ->
        LLVM.ConstRealOfString(lltype.lltype, string)
    )
    |> fun x -> { llvalue = x }

