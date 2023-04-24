[<Microsoft.FSharp.Core.AutoOpen>]
module internal FSharp.llvm.Utility
open System
open FSharp.NativeInterop
open System.Text
open System.Collections.Generic

let inline ptrToOption ([<InlineIfLambda>] ctor) ptr =
    if (NativePtr.isNullPtr ptr) then ValueNone
    else
        ValueSome (ctor ptr)
let inline isNullPtr ptr = NativePtr.isNullPtr ptr
let inline cast<'t, 'd when 't: unmanaged and 'd: unmanaged> ptr =
    ptr
    |> NativePtr.toNativeInt<'t>
    |> NativePtr.ofNativeInt<'d>
let inline (!*) ptr = NativePtr.read ptr
let inline ( *<-) ptr value = NativePtr.write ptr value
let inline (+!) ptr value = NativePtr.add ptr value
let inline (-!) ptr value = NativePtr.add ptr -value 
let inline pinningAs<'t, 'out, 'returnType when 't: unmanaged and 'out: unmanaged> ([<InlineIfLambda>] f) (v: 't array): 'returnType =
    let count = Array.length v
    use ptr = fixed v
    let ptr = cast<_, 'out> ptr 
    f ptr count
let rec strlen acc = function
    | ptr when NativePtr.read ptr = 0uy -> acc
    | ptr -> strlen (acc + 1) (NativePtr.add ptr 1)
let inline initUnsafe<'t, 'd when 't: unmanaged and 'd: unmanaged> count ([<InlineIfLambda>] f: nativeptr<'t> -> int -> unit) =
    let array = Array.zeroCreate<'d> count
    begin
        use ptr = fixed array
        let ptr = ptr |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<'t>
        
        f ptr count 
    end
    array  
let inline (|>) x ([<InlineIfLambda>] f) = f x 
let inline _of<'t when 't: unmanaged> handle =
    handle
    |> NativePtr.ofNativeInt<'t>
let inline UTF16StringToBytes (str: string) =
    Encoding.UTF8.GetBytes(str)
let inline CSharpStringOfSignedCString (str: nativeptr<sbyte>) =
    let length = strlen 0 (str |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<byte>)
    let span = Span<byte>(str |> NativePtr.toVoidPtr, length)
    Encoding.UTF8.GetString span
let private _dict = Dictionary<string, WeakReference<byte[]>>(HashIdentity.Structural)
let inline private _toUTF8 (str: string) =
    let bytes =
        if _dict.ContainsKey str then
            let v = _dict.[str]
            let mutable target: byte[] = null 
            if v.TryGetTarget &target then
                target
            else
                let b = UTF16StringToBytes str
                _dict.[str] <- WeakReference<byte[]>(b)
                b 
        else
            let b = UTF16StringToBytes str
            _dict.[str] <- WeakReference<byte[]>(b)
            b 
    bytes
let inline withUTF8 ([<InlineIfLambda>] f) (str: string) =
    str
    |> _toUTF8
    |> fun bytes ->
        use ptr = fixed bytes
        ptr
        |> cast<byte, sbyte>
        |> f
let inline withUTF8Lengthed ([<InlineIfLambda>] f) (str: string) =
    str
    |> _toUTF8
    |> fun bytes ->
        use ptr = fixed bytes
        ptr
        |> cast<byte, sbyte>
        |> fun x -> f x bytes.Length

let inline Some x = ValueSome x
let None = ValueNone

[<return: Struct>]
let inline (|Some|_|) (x: _ voption) = if x.IsSome then x else ValueNone
[<return: Struct>]
let inline (|None|_|) (x: _ voption) = if x.IsNone then ValueSome() else ValueNone


