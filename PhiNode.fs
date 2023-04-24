[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.PhiNode
open FSharpx.Collections
open Utility
open LLVMSharp.Interop
let addIncoming struct(value, basicBlock) phiNode =
    
    let mutable value = value.llvalue.Handle |> _of<LLVMOpaqueValue>
    let mutable basicBlock = basicBlock.llbasicblock.Handle |> _of<LLVMOpaqueBasicBlock>
    
    LLVM.AddIncoming(phiNode.llvalue, &&value, &&basicBlock, uint32 1)
let incoming phiNode =
    
    let phiNode = phiNode.llvalue
    let array = ResizeArray()
    for i = int (LLVM.CountIncoming phiNode) downto 1 do
        let i = uint32 i
        let incomingValue = LLVM.GetIncomingValue(phiNode, i - 1u)
        let incomingBlock = LLVM.GetIncomingBlock(phiNode, i - 1u)
        
        array.Add (struct({ llvalue = incomingValue }, { llbasicblock = incomingBlock }))
    array
    |> ResizeArray.toList

