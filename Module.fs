[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Module

open LLVMSharp.Interop

let createModule context name =
    name
    |> withUTF8 (fun name ->
        
        { llmodule = LLVM.ModuleCreateWithNameInContext(name, context.llcontext) }
    )

let disposeModule llmodule =
    LLVM.DisposeModule(llmodule.llmodule)

let dumpModule llmodule =
    LLVM.DumpModule(llmodule.llmodule)