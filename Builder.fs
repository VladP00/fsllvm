[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Builder
open LLVMSharp.Interop
open FSharp.NativeInterop
open System
open Microsoft.FSharp.Core
let builder context =
    { llbuilder = LLVM.CreateBuilderInContext(context.llcontext) }
let positionBuilder position builder =
    match position with
    | At_end basicBlock -> LLVM.PositionBuilderAtEnd(builder.llbuilder, basicBlock.llbasicblock)
    | Before value -> LLVM.PositionBuilderBefore(builder.llbuilder, value.llvalue)
let builderAt context position =
    let b = builder context in
    positionBuilder position b
    b 
let buildAlloca lltype (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildAlloca(lltype.lltype, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildAlloca(lltype.lltype) }
let buildArrayAlloca lltype size (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildArrayAlloca(lltype.lltype, size.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildArrayAlloca(lltype.lltype, size.llvalue) }
let buildStore value pointer builder =
    { llvalue = builder.llbuilder.BuildStore(value.llvalue, pointer.llvalue) }
let buildTrunc value lltype (name: string voption) builder =
    let name = ValueOption.defaultValue "" name
    { llvalue = builder.llbuilder.BuildTrunc(value.llvalue, lltype.lltype, name) }
let buildZExt value lltype (name: string voption) builder =
    let name = ValueOption.defaultValue "" name
    { llvalue = builder.llbuilder.BuildZExt(value.llvalue, lltype.lltype, name) }
let buildFPToUI value lltype (name: string voption) builder =
    let name = ValueOption.defaultValue "" name
    { llvalue = builder.llbuilder.BuildFPToUI(value.llvalue, lltype.lltype, name) }
let buildFPToSI value lltype (name: string voption) builder =
    let name = ValueOption.defaultValue "" name
    { llvalue = builder.llbuilder.BuildFPToSI(value.llvalue, lltype.lltype, name) }
let buildUIToFP value lltype (name: string voption) builder =
    let name = ValueOption.defaultValue "" name
    { llvalue = builder.llbuilder.BuildUIToFP(value.llvalue, lltype.lltype, name) }
let buildSIToFP value lltype (name: string voption) builder =
    let name = ValueOption.defaultValue "" name
    { llvalue = builder.llbuilder.BuildSIToFP(value.llvalue, lltype.lltype, name) }
let buildFPTrunc value lltype (name: string voption) builder =
    let name = ValueOption.defaultValue "" name
    { llvalue = builder.llbuilder.BuildFPTrunc(value.llvalue, lltype.lltype, name) }
let buildFPExt value lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildFPExt(value.llvalue, lltype.lltype, name) }
let buildPtrToInt value lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildPtrToInt(value.llvalue, lltype.lltype, name) }
let buildIntToPtr value lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildIntToPtr(value.llvalue, lltype.lltype, name) }
let buildBitCast value lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildBitCast(value.llvalue, lltype.lltype, name) }
let buildZExtOrBitCast value lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildZExtOrBitCast(value.llvalue, lltype.lltype, name) }
let buildSExtOrBitCast value lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildSExtOrBitCast(value.llvalue, lltype.lltype, name) }
let buildTruncOrBitCast value lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildTruncOrBitCast(value.llvalue, lltype.lltype, name) }
let buildPointerCast value lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildPointerCast(value.llvalue, lltype.lltype, name) }
let buildIntCast value lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildIntCast(value.llvalue, lltype.lltype, name) }
let buildFPCast value lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildFPCast(value.llvalue, lltype.lltype, name) }
let buildPhi list (name: string voption) builder =
    match list with
    | [] -> Unchecked.defaultof<llvalue>
    | (struct(expr, _) :: tail) when expr.llvalue.Handle = 0n -> Unchecked.defaultof<llvalue>
    | (head :: _) as incoming  -> 
    let name = defaultValueArg name ""
    let struct(firstValue: llvalue, _) = head
    let phiNode =
        name |>
        withUTF8 (fun name ->
            let t = LLVM.TypeOf(firstValue.llvalue) in
   
            let k = { lltype  = t }
            if k.lltype.Kind <> LLVMTypeKind.LLVMVoidTypeKind then 
                LLVM.BuildPhi(builder.llbuilder, t, name)
            else NativePtr.nullPtr
       ) 
    if not (NativePtr.isNullPtr phiNode) then
        incoming
       
        |> List.iter (fun struct(value, basicBlock) ->
                let mutable value = value.llvalue.Handle |> _of<LLVMOpaqueValue>
                let mutable basicBlock = basicBlock.llbasicblock.Handle |> _of<LLVMOpaqueBasicBlock>
                
                LLVM.AddIncoming(phiNode, &&value, &&basicBlock, 1u)
            )
        
        { llvalue = phiNode }
    else { llvalue = LLVMValueRef 0n }
let buildCall functionType functionValue (parameters: llvalue[]) (name: string voption) builder =
    let name = defaultValueArg name "tmp"
    let length = parameters.Length
    let mutable returnValue = Unchecked.defaultof<llvalue>
    let p = parameters
 
    begin
        use parameters = fixed parameters
        let parameters = parameters |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<nativeptr<LLVMOpaqueValue>>
        name
        |> withUTF8 (fun name ->
            eprintfn "Objective reached"
            if p.Length = 0 then 
                returnValue <- { llvalue = LLVM.BuildCall2(builder.llbuilder, functionType.lltype, functionValue.llvalue, NativePtr.nullPtr, uint32 p.Length, name) }
            else
                returnValue <- { llvalue = LLVM.BuildCall2(builder.llbuilder, functionType.lltype, functionValue.llvalue, parameters, uint32 p.Length, name) }
            )
    end
    
    
    returnValue

let buildEmptyPhi lltype (name: string voption) builder =
    let name = defaultValueArg name ""
    { llvalue = builder.llbuilder.BuildPhi(lltype.lltype, name) }

let buildGEP ty pointer (indices: lltype[]) (name: string voption) builder =
    let length = indices.Length
    use indices = fixed indices
    let indices =
        indices
        |> NativePtr.toVoidPtr
    let indices = ReadOnlySpan<LLVMValueRef>(indices, length)
    match name with
    | ValueSome name -> { llvalue = builder.llbuilder.BuildGEP2(ty.lltype, pointer.llvalue, indices, name) }
    | ValueNone ->  { llvalue = builder.llbuilder.BuildGEP2(ty.lltype, pointer.llvalue, indices, "") }
let buildInBoundsGEP ty pointer (indices: lltype[]) (name: string voption) builder =
    let length = indices.Length
    use indices = fixed indices
    let indices =
        indices
        |> NativePtr.toVoidPtr
    let indices = ReadOnlySpan<LLVMValueRef>(indices, length)
    match name with
    | ValueSome name -> { llvalue = builder.llbuilder.BuildInBoundsGEP2(ty.lltype, pointer.llvalue, indices, name) }
    | ValueNone ->  { llvalue = builder.llbuilder.BuildInBoundsGEP2(ty.lltype, pointer.llvalue, indices, "") }
let buildAdd lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildAdd(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildAdd(lhs.llvalue, rhs.llvalue) }
let buildNSWAdd lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildNSWAdd(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildNSWAdd(lhs.llvalue, rhs.llvalue) }
let buildNUWAdd lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildNUWAdd(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildNUWAdd(lhs.llvalue, rhs.llvalue) }
let buildSub lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildSub(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildSub(lhs.llvalue, rhs.llvalue) }
let buildNSWSub lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildNSWSub(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildNSWSub(lhs.llvalue, rhs.llvalue) }
let buildNUWSub lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildNUWSub(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildNUWSub(lhs.llvalue, rhs.llvalue) }
let buildMul lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildMul(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildMul(lhs.llvalue, rhs.llvalue) }
let buildNSWMul lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildNSWMul(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildNSWMul(lhs.llvalue, rhs.llvalue) }
let buildNUWMul lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildNUWMul(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildNUWMul(lhs.llvalue, rhs.llvalue) }
let buildUDiv lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildUDiv(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildUDiv(lhs.llvalue, rhs.llvalue) }

let buildSDiv lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildSDiv(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildSDiv(lhs.llvalue, rhs.llvalue) }
let buildExactSDiv lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildExactSDiv(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildExactSDiv(lhs.llvalue, rhs.llvalue) }
let buildURem lhs rhs (name: string voption) builder =
     match name with
     | ValueSome name ->
         { llvalue = builder.llbuilder.BuildURem(lhs.llvalue, rhs.llvalue, name) }
     | ValueNone ->
         { llvalue = builder.llbuilder.BuildURem(lhs.llvalue, rhs.llvalue) }
let buildSRem lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildSRem(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildSRem(lhs.llvalue, rhs.llvalue) }
let buildNeg op (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildNeg(op.llvalue, name) }
    | ValueNone -> { llvalue = builder.llbuilder.BuildNeg(op.llvalue) }
let buildNSWNeg op (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildNSWNeg(op.llvalue, name) }
    | ValueNone -> { llvalue = builder.llbuilder.BuildNSWNeg(op.llvalue) }
let buildNUWNeg op (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildNUWNeg(op.llvalue, name) }
    | ValueNone -> { llvalue = builder.llbuilder.BuildNUWNeg(op.llvalue) }
let buildFNeg op (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildFNeg(op.llvalue, name) }
    | ValueNone -> { llvalue = builder.llbuilder.BuildFNeg(op.llvalue) }
let buildFAdd lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildFAdd(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildFAdd(lhs.llvalue, rhs.llvalue) }
let buildExtractElement vector index (name: string voption) builder =
    match name with
    | ValueSome name -> { llvalue = builder.llbuilder.BuildExtractElement(vector.llvalue, index.llvalue, name) }
    | ValueNone -> { llvalue = builder.llbuilder.BuildExtractElement(vector.llvalue, index.llvalue) }
let buildInsertElement vector element index (name: string voption) builder =
    match name with
    | ValueSome name -> { llvalue = builder.llbuilder.BuildInsertElement(vector.llvalue, element.llvalue, index.llvalue, name) }
    | ValueNone -> { llvalue = builder.llbuilder.BuildInsertElement(vector.llvalue, element.llvalue, index.llvalue) }
let buildShuffleVector lhs rhs mask (name: string voption) builder =
    match name with
    | ValueSome name -> { llvalue = builder.llbuilder.BuildShuffleVector(lhs.llvalue, rhs.llvalue, mask.llvalue, name) }
    | ValueNone -> { llvalue = builder.llbuilder.BuildShuffleVector(lhs.llvalue, rhs.llvalue, mask.llvalue) }
let buildFSub lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildFSub(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildFSub(lhs.llvalue, rhs.llvalue) }
let buildFMul lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildFMul(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildFMul(lhs.llvalue, rhs.llvalue) }
let buildFDiv lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildFDiv(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildFDiv(lhs.llvalue, rhs.llvalue) }
let buildFRem lhs rhs (name: string voption) builder =
     match name with
     | ValueSome name ->
         { llvalue = builder.llbuilder.BuildFRem(lhs.llvalue, rhs.llvalue, name) }
     | ValueNone ->
         { llvalue = builder.llbuilder.BuildFRem(lhs.llvalue, rhs.llvalue) }
let buildFCmp (predicate: llrealpredicate) lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildFCmp(predicate.llrealpredicate, lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildFCmp(predicate.llrealpredicate, lhs.llvalue, rhs.llvalue) }
let buildICmp (predicate: llintpredicate) lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildICmp(predicate.llintpredicate, lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildICmp(predicate.llintpredicate, lhs.llvalue, rhs.llvalue) }
let buildShl lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildShl(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildShl(lhs.llvalue, rhs.llvalue) }
let buildLShr lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildLShr(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildLShr(lhs.llvalue, rhs.llvalue) }
let buildAShr lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildAShr(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildAShr(lhs.llvalue, rhs.llvalue) }
let buildAnd lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildAnd(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildAnd(lhs.llvalue, rhs.llvalue) }
let buildOr lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildOr(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildOr(lhs.llvalue, rhs.llvalue) }
let buildXor lhs rhs (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildXor(lhs.llvalue, rhs.llvalue, name) }
    | ValueNone ->
        { llvalue = builder.llbuilder.BuildXor(lhs.llvalue, rhs.llvalue) }
let buildNot op (name: string voption) builder =
    match name with
    | ValueSome name ->
        { llvalue = builder.llbuilder.BuildNot(op.llvalue, name) }
    | ValueNone -> { llvalue = builder.llbuilder.BuildNot(op.llvalue) }

let buildCondBr condition ifTrueBlock ifFalseBlock builder =
    { llvalue = LLVM.BuildCondBr(builder.llbuilder, condition.llvalue, ifTrueBlock.llbasicblock, ifFalseBlock.llbasicblock) }
let buildBr basicBlock builder =
    { llvalue = LLVM.BuildBr(builder.llbuilder, basicBlock.llbasicblock) }
    
let buildRet value builder =
    match value with
    | ValueSome value ->
        { llvalue = LLVM.BuildRet(builder.llbuilder, value.llvalue) }
    | ValueNone ->
        { llvalue = LLVM.BuildRetVoid(builder.llbuilder) }

let buildCallB fn args name builder =
    
    args |> pinningAs<llvalue, _, _> (fun ptr count ->
        match name with
        | ValueSome name ->
        name
        |> withUTF8 (fun name -> 
            { llvalue = LLVM.BuildCall2(builder.llbuilder, (llvmTypeOf fn).lltype, fn.llvalue, ptr, uint32 count, name) }
        )
        | ValueNone ->
            { llvalue = LLVM.BuildCall2(builder.llbuilder, (llvmTypeOf fn).lltype, fn.llvalue, ptr, uint32 count, NativePtr.nullPtr) }
    )
let buildNoop llmodule builder =
    (*let id = lookupID "llvm.donothing"
    
    let fn = getIntrinsicDeclaration llmodule id [||]
    builder |> buildCall (llvmTypeOf fn) fn [||] ValueNone*)
    Unchecked.defaultof<llvalue>