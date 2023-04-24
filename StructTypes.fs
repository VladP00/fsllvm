[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.StructTypes
open LLVMSharp.Interop
open FSharp.NativeInterop
open Utility
open System
open System.Text
let inline private _structType context types isPacked =
    let count = Array.length types
    let mutable returnType = Unchecked.defaultof<lltype>
    let isPacked = if isPacked then 1 else 0
    
    begin
        use ptr = fixed types
        let ptr = ptr |> NativePtr.toNativeInt<lltype> |> NativePtr.ofNativeInt<nativeptr<LLVMOpaqueType>>
        returnType.lltype <- LLVM.StructTypeInContext(context.llcontext, ptr, uint32 count, isPacked)
    end
    
    returnType
let structType context types = _structType context types false
let packedStructType context types = _structType context types true
let structName structType =
    let ptr = LLVM.GetStructName(structType.lltype)
    if (NativePtr.isNullPtr ptr) then ValueNone
    else
        let ptr = ptr |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<byte>
        
        let length = strlen 0 ptr
        let span = Span<byte>(ptr |> NativePtr.toVoidPtr, length)
        Encoding.UTF8.GetString(span)
        |> ValueSome
let namedStructType context name =
    name
    |> withUTF8 (fun name -> LLVM.StructCreateNamed(context.llcontext, name))
    |> fun opaqueTypePtr -> { lltype = opaqueTypePtr }
let structSetBody structType elementTypes isPacked =
    let isPacked = if isPacked then 1 else 0
    elementTypes
    |> pinningAs<lltype, nativeptr<LLVMOpaqueType>, _> (fun elementTypes count ->
        LLVM.StructSetBody(structType.lltype, elementTypes, uint32 count, isPacked)
    )
let structElementTypes structType: lltype array =
    let elementCount = LLVM.CountStructElementTypes structType.lltype
    initUnsafe (int elementCount) (fun opaqueTypes _ ->
        LLVM.GetStructElementTypes(structType.lltype, opaqueTypes)
    )
let isPacked structType =
    LLVM.IsPackedStruct structType.lltype <> 0

let isOpaque structType =
    LLVM.IsOpaqueStruct (structType.lltype) <> 0

let isLiteral structType =
    LLVM.IsLiteralStruct (structType.lltype) <> 0

