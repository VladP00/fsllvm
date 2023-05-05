[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.PassManager

open LLVMSharp.Interop

[<Struct>]
type 'a llpassmanager = internal { t: 'a; passmanager: LLVMPassManagerRef }

[<Struct>]
type any =
    | Module
    | Function

[<Struct>]
type ``module`` = Module_
    with static member toAny (self: ``module`` llpassmanager) = {t = Module; passmanager = self.passmanager}

[<Struct>]
type ``function`` = Function_
with static member toAny (self: ``function`` llpassmanager) = {t = Function; passmanager = self.passmanager}



let create(): ``module`` llpassmanager =
    { t = Module_; passmanager = LLVM.CreatePassManager() }
let createFunction ``module``: ``function`` llpassmanager =
    { t = Function_; passmanager = LLVM.CreateFunctionPassManager(``module``.llmodule.CreateModuleProvider()) }
let runModule ``module`` (p: ``module`` llpassmanager) =
    LLVM.RunPassManager(p.passmanager, ``module``.llmodule)
    |> function
        | 0 -> false
        | _ -> true

let initialize (passManager: ``function`` llpassmanager) =
    LLVM.InitializeFunctionPassManager(passManager.passmanager)
    |> function
        | 0 -> false
        | _ -> true

let runFunction ``function`` (p: ``function`` llpassmanager) =
    LLVM.RunFunctionPassManager(p.passmanager, ``function``.llvalue)
    |> function
        | 0 -> false
        | _ -> true

let finalize (p: ``function`` llpassmanager) =
    LLVM.FinalizeFunctionPassManager(p.passmanager)
    |> function
        | 0 -> false
        | _ -> true

let inline dispose p =
    LLVM.DisposePassManager(p.passmanager)
