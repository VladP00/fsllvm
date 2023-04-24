[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Functions

open LLVMSharp.Interop
open Microsoft.FSharp.NativeInterop

let private _entry = "entry"B

let declareFunction name lltype llmodule =
    name
    |> withUTF8 (fun name ->
        let fn = LLVM.GetNamedFunction(llmodule.llmodule, name)
        if not (isNullPtr fn) then
            if LLVM.GlobalGetValueType(fn) <> lltype.lltype then
                { llvalue = LLVM.ConstBitCast(fn, LLVM.PointerType(lltype.lltype, 0u)) }
            else { llvalue = fn }
        else { llvalue = LLVM.AddFunction(llmodule.llmodule, name, lltype.lltype) }
    )
let defineFunction name lltype llmodule =
    name
    |> withUTF8 (fun name ->
        let fn = LLVM.AddFunction(llmodule.llmodule, name, lltype.lltype)
        use entry = fixed _entry
        
        let _ = LLVM.AppendBasicBlockInContext(LLVM.GetTypeContext(lltype.lltype), fn, cast<_, sbyte> entry)
        { llvalue = fn }
    )
    //DEFINE_ITERATORS(function, Function, Module_val, LLVMValueRef, Value_val,
//                 LLVMGetGlobalParent)
let defineFunctionWithParamNames context name returnType paramNames paramTypes llmodule =
    name
    |> withUTF8 (fun name ->
        let ftType = functionType returnType paramTypes
        let fn = LLVM.AddFunction(llmodule.llmodule, name, ftType.lltype)
        let _entry = "entry"B
        use entry = fixed _entry
        let _ = LLVM.AppendBasicBlockInContext(context.llcontext, fn, cast<_, sbyte> entry)
        let fn = { llvalue = fn }
        
        eprintfn "typeof %s = %s" (fn.llvalue.Name) ({ lltype = LLVM.GlobalGetValueType(fn.llvalue) }.lltype.PrintToString())
        
        Array.iteri (fun i name -> setValueName name (param fn i)) paramNames
        eprintfn "typeof %s = %s" (fn.llvalue.Name) ({ lltype = LLVM.GlobalGetValueType(fn.llvalue) }.lltype.PrintToString())
        fn 
    )
    
let deleteFunction llfunction =
    LLVM.DeleteFunction llfunction.llvalue

let functionBegin llmodule =
    let first = LLVM.GetFirstFunction(llmodule.llmodule)
    if not (isNullPtr first) then
        Before { llvalue = first }
    else At_end llmodule
    
let functionSucc llfunction =
    let next = LLVM.GetNextFunction(llfunction.llvalue)
    if not (isNullPtr next) then
        Before { llvalue = next }
    else
        At_end { llmodule = LLVM.GetGlobalParent next }
        
        
let functionEnd llmodule =
    let last = LLVM.GetLastFunction llmodule.llmodule
    if not (isNullPtr last) then
        After { llvalue = last }
    else
        At_start llmodule
let inline iterFunctions ([<InlineIfLambda>] action) llmodule =
    let rec iter_function_range f i e =
        if i = e then () else
        match i with
        | At_end _ -> invalidOp "Invalid function range"
        | Before fn ->
            f fn
            iter_function_range f (functionSucc fn) e
    in iter_function_range action (functionBegin llmodule) (At_end llmodule)
let inline foldFunctions ([<InlineIfLambda>] folder) initial llmodule =
    let rec fold_function_range f acc i e =
        if i = e then acc else
        match i with
        | At_end _ -> invalidOp "Invalid function range"
        | Before fn ->
            let acc = f fn
            fold_function_range f acc (functionSucc fn) e
    in fold_function_range folder initial (functionBegin llmodule) (At_end llmodule)

let isInstrinsic llfunction = LLVM.GetIntrinsicID(llfunction.llvalue) <> 0u
let functionCallConvention llfunction = int (LLVM.GetFunctionCallConv llfunction.llvalue)
let setFunctionCallConvention callingConvention llfunction = LLVM.SetFunctionCallConv(llfunction.llvalue, uint32 callingConvention)
let gc llfunction =
    let strategy =
        LLVM.GetGC(llfunction.llvalue)
    if not (isNullPtr strategy) then
        CSharpStringOfSignedCString strategy
        |> ValueSome
    else ValueNone

let setGC strategy llfunction =
    match strategy with
    | ValueNone -> LLVM.SetGC(llfunction.llvalue, NativePtr.nullPtr)
    | ValueSome strategy -> 
    strategy
    |> withUTF8 (fun strategy -> LLVM.SetGC(llfunction.llvalue, strategy))
        
let lookupFunction name llmodule =
    name
    |> withUTF8 (fun name ->
        let ptr = LLVM.GetNamedFunction(llmodule.llmodule, name)
        if (isNullPtr ptr) then ValueNone
        else ValueSome { llvalue = ptr }
    )

