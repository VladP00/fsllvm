[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.FunctionTypes
open LLVMSharp.Interop
open FSharp.NativeInterop
let inline private _functionType returnType (paramTypes: lltype array) isVarArg =
    let count = Array.length paramTypes
    let mutable returnValue = { lltype = LLVMTypeRef(handle = 0n) }
    let isVarArg = if isVarArg then 1 else 0
    
    begin 
        use paramTypes = fixed paramTypes
        let paramTypes = paramTypes |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<nativeptr<LLVMOpaqueType>>
        returnValue.lltype <- LLVM.FunctionType(returnType.lltype, paramTypes, uint32 count, isVarArg)
        
    end
    eprintfn "%A" (returnValue.lltype.PrintToString())
    returnValue
let functionType returnType paramTypes =
    _functionType returnType paramTypes false
let varArgFunctionType returnType paramTypes =
    _functionType returnType paramTypes true
let isVarArg functionType =
    LLVM.IsFunctionVarArg(functionType.lltype) <> 0
let returnType functionType =
    { lltype = LLVM.GetReturnType(functionType.lltype) }
let paramTypes functionType =
    let count = LLVM.CountParamTypes(functionType.lltype)
    let array = Array.zeroCreate<lltype> (int count)
    begin 
        use ptr = fixed array
        let ptr = ptr |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<nativeptr<LLVMOpaqueType>>
        LLVM.GetParamTypes(functionType.lltype, ptr)
    end 
    array