module FSharp.llvm.Icmp
open LLVMSharp.Interop
[<Struct>]
type t =
    | Eq
    | Neq
    | Ugt
    | Uge
    | Ult
    | Ule
    | Sgt
    | Sge
    | Slt
    | Sle
    member inline self.llintpredicate
        with get() =
            match self with
            | Eq -> LLVMIntPredicate.LLVMIntEQ
            | Neq -> LLVMIntPredicate.LLVMIntNE
            | Ugt -> LLVMIntPredicate.LLVMIntUGT
            | Uge -> LLVMIntPredicate.LLVMIntUGE
            | Ult -> LLVMIntPredicate.LLVMIntULT
            | Ule -> LLVMIntPredicate.LLVMIntULE
            | Sgt -> LLVMIntPredicate.LLVMIntSGT
            | Sge -> LLVMIntPredicate.LLVMIntSGE
            | Slt -> LLVMIntPredicate.LLVMIntSLT
            | Sle -> LLVMIntPredicate.LLVMIntSLE

