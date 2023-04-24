[<Microsoft.FSharp.Core.RequireQualifiedAccess>]
module FSharp.llvm.ABIInfo

open System.Runtime.InteropServices
open System.Security

[<Struct>]
type private 'value SmallDAT5 when 'value: unmanaged =
    {
        mutable values: struct('value * 'value * 'value * 'value * 'value)
        mutable isPresent: byte 
    }
    static member set (dat: _ byref, index, newValue) =
        dat.isPresent <- dat.isPresent ||| (1uy <<< index)
        let struct(first, second, third, fourth, fifth) = dat.values
        match index with
        | 0 -> struct(newValue, second, third, fourth, fifth)

[<Literal>]
let abi_info = "/Users/vladpaun/Documents/Personal/FSharp/fsllvm/lib/libabi_info.dylib"

type size_t = unativeint

[<DllImport(abi_info, EntryPoint = "get_pointer_size", CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
extern size_t private get_pointer_size()

[<DllImport(abi_info, CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
extern size_t private get_function_pointer_size()

[<DllImport(abi_info, CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
extern size_t private get_alignment_of_pointer()

[<DllImport(abi_info, CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
extern size_t private get_alignment_of_function_pointer()

[<DllImport(abi_info, CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
extern size_t private get_alignment_of_integer(uint32 width)

[<DllImport(abi_info, CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
extern size_t private get_alignment_of_fp(uint32 width)

[<DllImport(abi_info, CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
extern size_t private get_long_double_size()

let inline private makeGetter ([<InlineIfLambda>] f) =
    let mutable pointerSize = 0un
    let mutable didSetPointer = false
    fun() ->
        if not didSetPointer then
            pointerSize <- f()
            didSetPointer <- true
        pointerSize
let pointerSize = makeGetter get_pointer_size
let functionPointerSize = makeGetter get_function_pointer_size
let pointerAlignment = makeGetter get_alignment_of_pointer
let functionPointerAlignment = makeGetter get_alignment_of_function_pointer
let longDoubleSize = makeGetter get_long_double_size
let alignmentOfInteger width = get_alignment_of_integer width
let alignmentOfFloatingPoint width = get_alignment_of_fp width 

        
    