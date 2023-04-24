[<AutoOpen>]
module FSharp.llvm.BasicBlocks

open LLVMSharp.Interop
open Microsoft.FSharp.Core

let basicBlocks llfunction =
    let count = int (LLVM.CountBasicBlocks llfunction.llvalue)
    initUnsafe<_, llbasicblock> count (fun ptr _ ->
        LLVM.GetBasicBlocks(llfunction.llvalue, ptr)
    )

let entryBlock llfunction =
    { llbasicblock = LLVM.GetEntryBasicBlock llfunction.llvalue }

let deleteBlock llblock =
    LLVM.DeleteBasicBlock llblock.llbasicblock

let removeBlock llblock =
    LLVM.RemoveBasicBlockFromParent(llblock.llbasicblock)
 
let moveBlockBefore position basicBlock =
    LLVM.MoveBasicBlockBefore(basicBlock.llbasicblock, position.llbasicblock)

let moveBlockAfter position basicBlock = 
    LLVM.MoveBasicBlockAfter(basicBlock.llbasicblock, position.llbasicblock)

let appendBlock context name llfunction =
    name
    |> withUTF8 (fun name ->
        LLVM.AppendBasicBlockInContext(context.llcontext, llfunction.llvalue, name)
    )
    |> fun x -> { llbasicblock = x }

let insertBlock context name block =
    name
    |> withUTF8 (fun name ->
        LLVM.InsertBasicBlockInContext(context.llcontext, block.llbasicblock, name)
    )
    |> fun x -> { llbasicblock = x }
let blockParent basicBlock =
    { llvalue = LLVM.GetBasicBlockParent(basicBlock.llbasicblock) }

let blockBegin llfunction =
    let first = LLVM.GetFirstBasicBlock(llfunction.llvalue)
    if not (isNullPtr first) then
        Before { llbasicblock = first }
    else At_end llfunction

let blockSucc block =
    let next = LLVM.GetNextBasicBlock block.llbasicblock
    if not (isNullPtr next) then
        Before { llbasicblock = next }
    else At_end { llvalue = LLVM.GetBasicBlockParent(next) }

let blockEnd llfunction =
    let last = LLVM.GetLastBasicBlock llfunction.llvalue
    if not (isNullPtr last) then
        After { llbasicblock = last }
     else
         At_start llfunction

let inline iterBlocks ([<InlineIfLambda>] action) llfunction =
    let rec iter_block_range f i e =
        if i = e then () else
        match i with
        | At_end _ -> invalidOp "Invalid block range"
        | Before bb ->
            f bb
            iter_block_range f (blockSucc bb) e
    in iter_block_range action (blockBegin llfunction) (At_end llfunction)

let inline foldBlocks ([<InlineIfLambda>] folder) initial llfunction =
    let rec fold_block_range f acc i e =
        if i = e then acc else
        match i with
        | At_end _ -> invalidOp "Invalid block range"
        | Before bb ->
            let acc = f bb
            fold_block_range f acc (blockSucc bb) e
    in fold_block_range folder initial (blockBegin llfunction) (At_end llfunction)

let blockTerminator block =
    { llvalue = LLVM.GetBasicBlockTerminator(block.llbasicblock) }

let valueOfBlock block =
    LLVM.BasicBlockAsValue block.llbasicblock
    |> fun x -> { llvalue = x }

let valueIsBlock value = LLVM.ValueIsBasicBlock value.llvalue <> 0
let blockOfValue value = { llbasicblock = LLVM.ValueAsBasicBlock value.llvalue }