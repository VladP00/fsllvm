module FSharp.llvm.DLLStorageClass
open LLVMSharp.Interop
    [<Struct>]
    type t =
        | Default
        | DLLImport
        | DLLExport
        member inline self.lldllstorageclass
            with get() =
                match self with
                | Default -> LLVMDLLStorageClass.LLVMDefaultStorageClass
                | DLLImport -> LLVMDLLStorageClass.LLVMDLLImportStorageClass
                | DLLExport -> LLVMDLLStorageClass.LLVMDLLExportStorageClass

