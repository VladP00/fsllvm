[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.OtherTypes
open LLVMSharp.Interop
open Utility
open FSharp.NativeInterop

let voidType context = {lltype = LLVM.VoidTypeInContext context.llcontext}
let labelType context = { lltype = LLVM.LabelTypeInContext context.llcontext }
let typeByName llmodule name =
    name 
    |> withUTF8 (fun name -> 
        let t = LLVM.GetTypeByName(llmodule.llmodule, name)
        if (NativePtr.isNullPtr t) then ValueNone
        else ValueSome { lltype = t }
    )

