[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.MetaData

open System.Text
open LLVMSharp.Interop
open Microsoft.FSharp.NativeInterop
open System

let mdstring context string =
    string |>
    withUTF8Lengthed (fun string length -> 
        LLVM.MDStringInContext(context.llcontext, string, uint32 length)
    )
    |> fun x -> { llvalue = x }

let mdnode context (values: llvalue array) =
    
    values
    |> pinningAs<_, nativeptr<LLVMOpaqueValue>, _> (fun values count ->
        LLVM.MDNodeInContext(context.llcontext, values, uint32 count)
    )
    |> fun x -> { llvalue = x }

let mdnull (context: llcontext) =
    { llvalue = LLVMValueRef(0n) }

let getMDString value =
    let mutable length = 0u
    let s = LLVM.GetMDString(value.llvalue, &&length)
    if length = 0u or isNullPtr s then
        ValueNone
    else
        let s = Span<byte>(s |> NativePtr.toVoidPtr, int length)
        Encoding.UTF8.GetString(s)
        |> ValueSome
    
