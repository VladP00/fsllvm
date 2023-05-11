[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.StackMap
open System
open System.ComponentModel
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop

[<Literal>]
let stack_walker = "/Users/vladpaun/Documents/Personal/FSharp/fsllvm/lib/libstack_walker.dylib"

[<DllImport(stack_walker)>]
extern byte* get_stackmap_address()

[<Struct>]
type StackMap =
    {
        versionNumber: uint8
        stackSizeRecords: StackSizeRecord[]
        constants: uint64[]
        stackMapRecords: StackMapRecord[]
        
    }

and [<Struct>] StackSizeRecord =
    {
        functionAddress: nativeint
        stackSize: uint64
        recordCount: uint64 
    }

and [<Struct>] StackMapRecord =
    {
        mutable patchPointID: uint64
        mutable instructionOffset: uint32
        mutable flags: uint16 
        mutable locations: Location[]
        mutable liveRegisters: LiveRegister[]
    }
    
and [<Struct>] Location =
    {
        size: uint16
        internals: LocationInternals
    }

and [<Struct>] LocationInternals =
    | Register of dwarfRegisterNumber: uint16
    | Direct of registerNumber: uint16 * offset: int32
    | Indirect of iRegisterNumber: uint16 * iOffset: int32
    | Constant of value: int32
    | LargeConstant of lvalue: uint64

and [<Struct>] LiveRegister =
    {
        registerNumber: uint16
        size: uint8 
    }
    
[<Struct; StructLayout(LayoutKind.Sequential)>]
type location_t = {
    locationType: uint8
    _reserved: uint8
    locationSize: uint16
    registerNumber: uint16
    _reserved_: uint16
    offset: int32 
}

[<Literal>]
let RRegister = 0x1uy

[<Literal>]
let RDirect = 0x2uy

[<Literal>]
let RIndirect = 0x3uy

[<Literal>]
let RConstant = 0x4uy

[<Literal>]
let RConstIndex = 0x5uy

    
let inline private (+!) left right = NativePtr.add left right
let inline private ( *<-) left right = NativePtr.write left right
let inline private (!*) self = NativePtr.read self
let inline cast<'t, 'd when 't: unmanaged and 'd: unmanaged> self =
    self
    |> NativePtr.toNativeInt<'t>
    |> NativePtr.ofNativeInt<'d>
type nativeptr<'t when 't: unmanaged> with
    member inline self.Item
        with get index =
            let ptr = NativePtr.add self index
            NativePtr.toByRef ptr 
        and set index newValue =
            NativePtr.set self index newValue
let readStackMap() =
    let addr = get_stackmap_address()
    let version = !*addr
    let next = addr +! (sizeof<uint8> + sizeof<uint8> + sizeof<uint16>)
    let numbers = cast<_, int> next
    
    let numberOfFunctions = numbers.[0]
    let numberOfConstants = numbers.[1]
    let numberOfRecords = numbers.[2]
    
    let stackMap = {
        versionNumber = version
        stackSizeRecords = Array.zeroCreate numberOfFunctions
        constants = Array.zeroCreate numberOfConstants
        stackMapRecords = Array.zeroCreate numberOfRecords
    }
    
    let next = numbers +! 3
    let stackSizeRecords = cast<_, uint64> next 
    
    for i = 0 to numberOfFunctions - 1 do
        
        stackMap.stackSizeRecords.[i] <- {
            functionAddress = nativeint stackSizeRecords.[i * 3]
            stackSize = stackSizeRecords.[i * 3 + 1]
            ;recordCount = stackSizeRecords.[i * 3 + 2]
        }
    let next = stackSizeRecords +! (3 * numberOfRecords)
    let source = Span(next |> NativePtr.toVoidPtr, numberOfConstants)
    let dest = Span(stackMap.constants)
    
    source.CopyTo dest
    
    let next = next +! numberOfConstants
    let mutable next = cast<_, uint8> next
    for i = 0 to numberOfRecords - 1 do
        let mutable size = 0
        let patchPointID = !*(cast<_, uint64> next)
        let offSet = !*(cast<_, uint32> (next +! 8))
        let flags = !*(cast<_, uint16> (next +! 12))
        let numberOfLocations = int (!*(cast<_, uint16> (next +! 14)))
        
        stackMap.stackMapRecords.[i].patchPointID <- patchPointID
        stackMap.stackMapRecords.[i].flags <- flags
        stackMap.stackMapRecords.[i].instructionOffset <- offSet
        stackMap.stackMapRecords.[i].locations <- Array.zeroCreate numberOfLocations
        next <- next +! 16
        
        let mutable locationPtr = cast<_, location_t> next
        
        for j = 0 to numberOfLocations - 1 do
           let locationSize = locationPtr.[j].locationSize
           let internals = 
               match locationPtr.[j] with
               | { locationType = RRegister; registerNumber = r } -> Register r
               | { locationType = RDirect;  registerNumber = r; offset = o} -> Direct(r, o)
               | { locationType = RIndirect; registerNumber = r; offset = o } -> Indirect(r, o)
               | { locationType = RConstant; offset = c} -> Constant c
               | { locationType = RConstIndex; offset = i} -> LargeConstant (stackMap.constants.[i])
               | _ -> failwith "Unknown location type in stack map"
           size <- size + 12
           stackMap.stackMapRecords.[i].locations.[j] <- { size = locationSize; internals = internals }
        let _next = (cast<_, uint8> (locationPtr +! numberOfLocations)) +! (if size % 8 = 0 then 2 else 6)
        let numberOfLiveRegisters = int (!*(cast<_, uint16> _next))
        stackMap.stackMapRecords.[i].liveRegisters <- Array.zeroCreate numberOfLiveRegisters
        let mutable nextSize = 0
        
        let liveOutsPtr = (_next +! 2)
        for j = 0 to numberOfLiveRegisters - 1 do
            let registerNumber = !* (cast<_, uint16> (liveOutsPtr +! (j * 4)))
            let sizeInBytes = liveOutsPtr.[j * 4 + 3]
            stackMap.stackMapRecords.[i].liveRegisters.[j] <- { size = sizeInBytes; registerNumber = registerNumber }
            
            nextSize <- nextSize + 4
        if nextSize % 8 <> 0 then
            locationPtr <- (locationPtr +! (size + nextSize + 4))
        else
            locationPtr <- (locationPtr +! (size + nextSize))
        next <- cast<_, uint8> locationPtr

    stackMap
    