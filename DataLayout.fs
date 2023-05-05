[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.DataLayout

open System.Text
open LLVMSharp.Interop

[<Struct>]
type Mangling =
    | ELF
    | GOFF
    | MIPS
    | MACH'O
    | WindowsCOFF
    | WindowsCOFFW
    | XCOFF

[<Struct>]
type Layout =
    | ProgramSpace of pAddressSpace: uint
    | GlobalSpace of gAddressSpace: uint
    | AllocaSpace of aAddressSpace: uint
    | PointerInfo of pointerAddressSpace: uint voption *
                     pSizeInBits: uint64 *
                     pABIAlignment: uint64 *
                     pPreferredAlignment: uint64 voption *
                     index: uint64 voption 
    | IntegerAlignment of integerSize: uint64 *
                          iABIAlignment: uint64 *
                          iPreferredAlignment: uint64 voption
    | FloatAlignment of floatSize: uint64 *
                        fABIAlignment: uint64 *
                        fPreferredAlignment: uint64 voption
    | AggregateAlignment of AABI: uint64 * preferred: uint64 voption
    | FunctionPtrAlignment of isIndependent: bool * FABI: uint64
    | Mangling of mangling: Mangling
    | NonIntegral of addressSpaces: uint list
    | IsLittleEndian of bool 
    static member toString = function
        | ProgramSpace p -> $"P{p}"
        | GlobalSpace g -> $"G{g}"
        | AllocaSpace a -> $"A{a}"
        | PointerInfo(addressSpace, size, abi, pref, index) ->
            let addressSpace = defaultValueArg addressSpace 0u
            let pref = defaultValueArg pref abi
            let index = defaultValueArg index size
            $"p{addressSpace}:{size}:{abi}:{pref}:{index}"
        | IntegerAlignment(size, abi, pref) ->
            let pref = defaultValueArg pref abi
            $"i{size}:{abi}:{pref}"
        | FloatAlignment(size, abi, pref) ->
             let pref = defaultValueArg pref abi
             $"f{size}:{abi}:{pref}"
        | AggregateAlignment(abi, pref) ->
             let pref = defaultValueArg pref abi
             $"a:{abi}:{pref}"
        | FunctionPtrAlignment(true, abi) -> $"Fi{abi}"
        | FunctionPtrAlignment(false, abi) -> $"Fn{abi}"
        | Mangling ELF -> "m:e"
        | Mangling GOFF -> "m:l"
        | Mangling MIPS -> "m:m"
        | Mangling MACH'O -> "m:o"
        | Mangling WindowsCOFF -> "m:x"
        | Mangling WindowsCOFFW -> "m:w"
        | Mangling XCOFF -> "m:a"
        | NonIntegral list ->
            list |> 
            List.fold (fun (builder: StringBuilder) e -> builder.Append ':' |> ignore; builder.Append e) (StringBuilder("ni"))
            |> fun x -> x.ToString()
        | IsLittleEndian true -> "e"
        | IsLittleEndian false -> "E"
let layoutString layout =
    layout
    |> Seq.ofList
    |> Seq.map (Layout.toString)
    |> Seq.reduce (fun left right -> left ^ "-" ^ right)

let setTargetDataLayout layout llmodule =
    layout
    |> layoutString
    |> withUTF8 (fun layout -> LLVM.SetDataLayout(llmodule.llmodule, layout))

let setTargetDataLayoutString layoutString llmodule =
    layoutString
    |> withUTF8 (fun layout -> LLVM.SetDataLayout(llmodule.llmodule, layout))