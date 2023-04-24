module FSharp.llvm.Opcode

open System.Reflection

[<Struct>]
type t = 
    | Invalid
    (*
    Not an instruction

    *)
    | Ret
    (*
    Terminator Instructions

    *)
    | Br
    | Switch
    | IndirectBr
    | Invoke
    | Invalid2
    | Unreachable
    | Add
    (*
    Standard Binary Operators

    *)
    | FAdd
    | Sub
    | FSub
    | Mul
    | FMul
    | UDiv
    | SDiv
    | FDiv
    | URem
    | SRem
    | FRem
    | Shl
    (*
    Logical Operators

    *)
    | LShr
    | AShr
    | And
    | Or
    | Xor
    | Alloca
    (*
    Memory Operators

    *)
    | Load
    | Store
    | GetElementPtr
    | Trunc
    (*
    Cast Operators

    *)
    | ZExt
    | SExt
    | FPToUI
    | FPToSI
    | UIToFP
    | SIToFP
    | FPTrunc
    | FPExt
    | PtrToInt
    | IntToPtr
    | BitCast
    | ICmp
    (*
    Other Operators

    *)
    | FCmp
    | PHI
    | Call
    | Select
    | UserOp1
    | UserOp2
    | VAArg
    | ExtractElement
    | InsertElement
    | ShuffleVector
    | ExtractValue
    | InsertValue
    | Fence
    | AtomicCmpXchg
    | AtomicRMW
    | Resume
    | LandingPad
    | AddrSpaceCast
    | CleanupRet
    | CatchRet
    | CatchPad
    | CleanupPad
    | CatchSwitch
    | FNeg
    | CallBr
    | Freeze
let private ``.ctor`` =
    typeof<t>.GetConstructor(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance, [|typeof<int32>; typeof<bool>|])
let unsafeCreate (tag: int32): t =
    let case = ``.ctor``.Invoke [|tag; true|]
    unbox case 