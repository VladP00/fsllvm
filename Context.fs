[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Context

open LLVMSharp.Interop

let createContext() =
    { llcontext = LLVM.ContextCreate() }

let disposeContext context =
    LLVM.ContextDispose(context.llcontext)
    
let globalContext() =
    { llcontext = LLVM.GetGlobalContext() }
    
let mdkindID context name =
    let id =
        name
        |> withUTF8Lengthed (fun name length -> 
            LLVM.GetMDKindIDInContext(context.llcontext, name, uint32 length)
        )
    enum<llmdkind> (int32 id)
    

