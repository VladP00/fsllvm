module FSharp.llvm.Fcmp
open LLVMSharp.Interop
[<Struct>]
type t =
    | False
    | Oeq
    | Ogt
    | Oge
    | Olt
    | Ole
    | One
    | Ord
    | Uno
    | Ueq
    | Ugt
    | Uge
    | Ult
    | Ule
    | Une
    | True
    member inline self.llrealpredicate
        with get() =
            match self with
            | False -> LLVMRealPredicate.LLVMRealPredicateFalse
            | Oeq -> LLVMRealPredicate.LLVMRealOEQ
            | Ogt -> LLVMRealPredicate.LLVMRealOGT
            | Oge -> LLVMRealPredicate.LLVMRealOGE
            | Olt -> LLVMRealPredicate.LLVMRealOLT
            | Ole -> LLVMRealPredicate.LLVMRealOLE
            | One -> LLVMRealPredicate.LLVMRealONE
            | Ord -> LLVMRealPredicate.LLVMRealORD
            | Uno -> LLVMRealPredicate.LLVMRealUNO
            | Ueq -> LLVMRealPredicate.LLVMRealUEQ
            | Ugt -> LLVMRealPredicate.LLVMRealUGT
            | Uge -> LLVMRealPredicate.LLVMRealUGE
            | Ult -> LLVMRealPredicate.LLVMRealULT
            | Ule -> LLVMRealPredicate.LLVMRealULE
            | Une -> LLVMRealPredicate.LLVMRealUNE
            | True -> LLVMRealPredicate.LLVMRealPredicateTrue
    
    
   

