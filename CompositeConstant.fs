[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.CompositeConstant

open System.Text
open LLVMSharp.Interop

let inline private _constUTF8String context (string: string) nullTerminate =
    string
    |> withUTF8Lengthed (fun utf8string length ->
    

        LLVM.ConstStringInContext(context.llcontext, utf8string, uint32 length, if nullTerminate then 0 else 1)
    )

let constUTF8String context string = _constUTF8String context string false
let constUTF8Stringz context string = _constUTF8String context string true
let constArray elementType elements =
    elements
    |> pinningAs<lltype, _, _> (fun ptr count ->
                  LLVM.ConstArray(elementType.lltype, ptr, uint32 count)
    )
    |> fun x -> { llvalue = x }

let inline private _constStruct context elements isPacked =
   
    elements
    |> pinningAs<llvalue, _, _> (fun ptr count ->
        LLVM.ConstStructInContext(context.llcontext, ptr, uint32 count, if isPacked then 1 else 0)
    )
    |> fun x -> { llvalue = x }

let constStruct context elements = _constStruct context elements false
let constPackedStruct context elements = _constStruct context elements true
let constNamedStruct structType elements =
    elements
    |> pinningAs<llvalue, _, _> (fun ptr count ->
        LLVM.ConstNamedStruct(structType.lltype, ptr, uint32 count)
    )
    |> fun x -> { llvalue = x }

