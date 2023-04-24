[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.GlobalVariables

open LLVMSharp.Interop

let declareGlobal lltype name llmodule =
    name
    |> withUTF8 (fun name ->
        let llmodule = llmodule.llmodule
        let lltype = lltype.lltype 
        let globalVar = LLVM.GetNamedGlobal(llmodule, name)
        if not (isNullPtr globalVar) then
            if LLVM.GlobalGetValueType(globalVar) <> lltype then 
                { llvalue = LLVM.ConstBitCast(globalVar, LLVM.PointerType(lltype, 0u)) }
            else { llvalue = globalVar }
        else
            { llvalue =
                LLVM.AddGlobal(llmodule, lltype, name) }

    )
let declareQualifedGlobal lltype name addressSpace llmodule =
    name
    |> withUTF8 (fun name ->
        let llmodule = llmodule.llmodule
        let lltype = lltype.lltype 
        let globalVar = LLVM.GetNamedGlobal(llmodule, name)
        if not (isNullPtr globalVar) then
            if LLVM.GlobalGetValueType(globalVar) <> lltype then 
                { llvalue = LLVM.ConstBitCast(globalVar, LLVM.PointerType(lltype, addressSpace)) }
            else { llvalue = globalVar }
        else
            { llvalue =
                LLVM.AddGlobalInAddressSpace(llmodule, lltype, name, addressSpace) }

    )