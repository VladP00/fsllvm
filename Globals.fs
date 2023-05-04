[<AutoOpen>]
module FSharp.llvm.Globals

open LLVMSharp.Interop

let declareGlobal lltype name llmodule =
    name
    |> withUTF8 (fun name ->
        let globalVar = LLVM.GetNamedGlobal(llmodule.llmodule, name)
        if not (isNullPtr globalVar) then
            if LLVM.GlobalGetValueType(globalVar) <> lltype.lltype then
                { llvalue = LLVM.ConstBitCast(globalVar, LLVM.PointerType(lltype.lltype, 0u)) }
            else { llvalue = globalVar }
        else
            { llvalue = LLVM.AddGlobal(llmodule.llmodule, lltype.lltype, name) } 
    )