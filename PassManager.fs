[<Microsoft.FSharp.Core.AutoOpen>]
module FSharp.llvm.PassManager

open System.Runtime.InteropServices
open System.Security
open LLVMSharp.Interop


[<Literal>]
let llvmc = "/Users/vladpaun/Documents/Personal/FSharp/fsllvm/lib/libllvmc.dylib"

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMCreatePassBuilder"); SuppressUnmanagedCodeSecurity>]
extern llpassbuilder passBuilder()

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMCreateModulePassManager"); SuppressUnmanagedCodeSecurity>]
extern llmodulepassmanager modulePassManager()

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMCreateFunctionPassManager"); SuppressUnmanagedCodeSecurity>]
extern llfunctionpassmanager functionPassManager()

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMCreateModuleAnalysisManager"); SuppressUnmanagedCodeSecurity>]
extern llmoduleanalysismanager moduleAnalysisManager()

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMCreateFunctionAnalysisManager"); SuppressUnmanagedCodeSecurity>]
extern llFunctionAnalysisManager functionAnalysisManager()

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMCreateLoopAnalysisManager"); SuppressUnmanagedCodeSecurity>]
extern llLoopAnalysisManager loopAnalysisManager()

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMCreateCGSCCAnalysisManager"); SuppressUnmanagedCodeSecurity>]
extern llCGSCCAnalysisManager CGSCCAnalysisManager()

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMBuildModulePassManager"); SuppressUnmanagedCodeSecurity>]
extern llmodulepassmanager buildModulePassManager(llpassbuilder)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMAddRS4GCPass"); SuppressUnmanagedCodeSecurity>]
extern void addRS4GC(llmodulepassmanager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMAddAlwaysInliner"); SuppressUnmanagedCodeSecurity>]
extern void addAlwaysInline(llmodulepassmanager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMAddMem2RegPass"); SuppressUnmanagedCodeSecurity>]
extern void addMem2Reg(llfunctionpassmanager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMAddInstructionCombining"); SuppressUnmanagedCodeSecurity>]
extern void addInstrComb(llfunctionpassmanager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMAddGVN"); SuppressUnmanagedCodeSecurity>]
extern void addGVN(llfunctionpassmanager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMAddTailRecursionElimination"); SuppressUnmanagedCodeSecurity>]
extern void addTailCallOpt(llfunctionpassmanager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMAddLoopVectorize"); SuppressUnmanagedCodeSecurity>]
extern void addLoopVectorize(llfunctionpassmanager)
    
[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, SetLastError = false, EntryPoint = "LLVMAddFunctionPassToModule"); SuppressUnmanagedCodeSecurity>]
extern void _addFunctionPassToModule(llfunctionpassmanager, llmodulepassmanager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LLVMRunModulePassManager"); SuppressUnmanagedCodeSecurity>]
extern void _runModulePassManager(LLVMModuleRef, llmodulepassmanager, llmoduleanalysismanager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ensureStackTracePrinted"); SuppressUnmanagedCodeSecurity>]
extern void ensureStackTracePrinted()

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LLVMRegisterLoop"); SuppressUnmanagedCodeSecurity>]
extern void registerLoop(llpassbuilder, llLoopAnalysisManager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LLVMRegisterFunction"); SuppressUnmanagedCodeSecurity>]
extern void registerFunction(llpassbuilder, llFunctionAnalysisManager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LLVMRegisterModule"); SuppressUnmanagedCodeSecurity>]
extern void registerModule(llpassbuilder, llmoduleanalysismanager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LLVMRegisterCGSCC"); SuppressUnmanagedCodeSecurity>]
extern void registerCGSCC(llpassbuilder, llCGSCCAnalysisManager)

[<DllImport(llvmc, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LLVMCrossRegisterProxies"); SuppressUnmanagedCodeSecurity>]
extern void _crossRegisterProxies (llpassbuilder pb,
                                         llLoopAnalysisManager lam,
                                         llFunctionAnalysisManager fam,
                                         llCGSCCAnalysisManager cgam,
                                         llmoduleanalysismanager mam)

let runModulePass llmodule passManager analysisManager =
    ensureStackTracePrinted()
    eprintfn "yay"
    _runModulePassManager(llmodule.llmodule, passManager, analysisManager)

let addFunctionPassesToModule fnPas modPas =
    _addFunctionPassToModule(fnPas, modPas)

let crossRegisterProxies struct(lam, fam, cgam, mam) pb = _crossRegisterProxies(pb, lam, fam, cgam, mam)