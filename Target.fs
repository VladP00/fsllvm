[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Target

open LLVMSharp.Interop

let getTargetData llmodule =
    LLVM.GetTarget llmodule.llmodule
    |> CSharpStringOfSignedCString
    |> LLVMTargetDataRef.FromStringRepresentation
    |> fun x -> { lltargetdata = x }
    
        

let setTargetData llmodule triple =
    triple
    |> withUTF8 (fun triple -> 
        LLVM.SetTarget(llmodule.llmodule, triple)
    )
