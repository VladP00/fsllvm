module FSharp.llvm.ValueKind
[<Struct>]
type t =
    | NullValue
    | Argument
    | BasicBlock
    | InlineAsm
    | MDNode
    | MDString
    | BlockAddress
    | ConstantAggregateZero
    | ConstantArray
    | ConstantDataArray
    | ConstantDataVector
    | ConstantExpr
    | ConstantFP
    | ConstantInt
    | ConstantPointerNull
    | ConstantStruct
    | ConstantVector
    | Function
    | GlobalAlias
    | GlobalIFunc
    | GlobalVariable
    | UndefValue
    | PoisonValue
    | Instruction of Opcode.t
 

