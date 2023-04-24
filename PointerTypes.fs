[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.PointerTypes
open LLVMSharp.Interop
open Utility

let subTypes lltype: lltype[] =
    let numberOfContained = LLVM.GetNumContainedTypes(lltype.lltype)
    initUnsafe (int numberOfContained) (fun opaqueTypes _ ->
        LLVM.GetSubtypes(lltype.lltype, opaqueTypes)
    )
let arrayType elementType count =
    { lltype = LLVM.ArrayType(elementType.lltype, uint32 count) }
let pointerType pointeeType =
    { lltype = LLVM.PointerType(pointeeType.lltype, 0u) }
let qualifiedPointerType pointeeType addressSpace =
    { lltype = LLVM.PointerType(pointeeType.lltype, uint32 addressSpace) }
let vectorType primitiveType count =
    { lltype = LLVM.VectorType(primitiveType.lltype, uint32 count) }
let elementType lltype =
    { lltype = LLVM.GetElementType(lltype.lltype) }
let arrayLength arrayType =
    int (LLVM.GetArrayLength(arrayType.lltype))
let addressSpace pointerType =
    int (LLVM.GetPointerAddressSpace(pointerType.lltype))
let vectorSize vectorType =
    int (LLVM.GetVectorSize(vectorType.lltype))

