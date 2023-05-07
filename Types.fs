[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.Types
open LLVMSharp.Interop
open Microsoft.FSharp.NativeInterop
[<Struct>]
type llvalue = internal { mutable llvalue: LLVMValueRef }
[<Struct>]
type lltype = internal { mutable lltype: LLVMTypeRef }
[<Struct>]
type llcontext = internal { mutable llcontext: LLVMContextRef }
[<Struct>]
type llbuilder = internal { mutable llbuilder: LLVMBuilderRef }
[<Struct>]
type llmodule = internal { mutable llmodule: LLVMModuleRef }
[<Struct>]
type llbasicblock = internal { mutable llbasicblock: LLVMBasicBlockRef }
[<Struct>]
type lltargetdata = internal { mutable lltargetdata: LLVMTargetDataRef }
[<Struct>]
type lluse = internal { mutable lluse: LLVMUseRef }
[<Struct>]
type llpassbuilder = { mutable address: nativeint }
[<Struct>]
type llfunctionpassmanager = { mutable address: nativeint }
[<Struct>]
type llmodulepassmanager = { mutable address: nativeint }
[<Struct>]
type llmoduleanalysismanager = { mutable address: nativeint }
[<Struct>]
type llLoopAnalysisManager = { mutable address: nativeint }
[<Struct>]
type llCGSCCAnalysisManager = { mutable address: nativeint }
[<Struct>]
type llFunctionAnalysisManager = { mutable address: nativeint }

[<Struct>]
type llpos<'a, 'b> =
    | At_end of first: 'a
    | Before of 'b
[<Struct>]
type llrev_pos<'a, 'b> =
    | At_start of first: 'a
    | After of 'b
    
type llmdkind = LLVMMetadataKind


type llrealpredicate = Fcmp.t
type llintpredicate = Icmp.t
type lldllstorageclass = DLLStorageClass.t
type llvaluekind = ValueKind.t
type llopcode = Opcode.t 

let test() =
    printfn "sizeof<llvalue>: %d" sizeof<llvalue>
    ()