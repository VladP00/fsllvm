#import <llvm/Pass.h>
#import <llvm/Passes/PassBuilder.h>
#import <llvm/Transforms/Scalar/RewriteStatepointsForGC.h>
#import <llvm/Transforms/Utils/Mem2Reg.h>
#import <llvm/Transforms/AggressiveInstCombine/AggressiveInstCombine.h>
#import <llvm/Transforms/IPO/AlwaysInliner.h>
#import <llvm/Transforms/Scalar/GVN.h>
#import <llvm/Transforms/Scalar/GVNExpression.h>
#import <llvm/Transforms/Scalar/SimplifyCFG.h>
#import <llvm/Transforms/Scalar/TailRecursionElimination.h>
#import <llvm/Transforms/Vectorize/LoopVectorize.h>
#import <signal.h>
#import <llvm/Support/PrettyStackTrace.h>
#import <execinfo.h>
#import <unistd.h>


using namespace llvm;



#define LLVM_CREATE_P(TYPE) \
struct LLVMOpaque##TYPE;    \
typedef struct LLVMOpaque##TYPE * LLVM##TYPE##Ref; \
extern "C" LLVM##TYPE##Ref LLVMCreate##TYPE(){ \
    auto self = (llvm::TYPE *) calloc(1, sizeof(llvm::TYPE));                             \
    *self = (llvm::TYPE ());                                                         \
    return (LLVM##TYPE##Ref) self;                                                   \
}\

LLVM_CREATE_P(PassBuilder);
LLVM_CREATE_P(ModulePassManager);
LLVM_CREATE_P(FunctionPassManager);
LLVM_CREATE_P(CGSCCPassManager);
LLVM_CREATE_P(LoopAnalysisManager);
LLVM_CREATE_P(FunctionAnalysisManager);
LLVM_CREATE_P(CGSCCAnalysisManager);
LLVM_CREATE_P(ModuleAnalysisManager);

// --- Pass Builder Stuff --- //
#define LLVM_REGISTER_PB(TYPE) \
extern "C" void LLVMRegister##TYPE(LLVMPassBuilderRef ref, LLVM##TYPE##AnalysisManagerRef other) {\
    llvm::PassBuilder* self = (llvm::PassBuilder*) ref; \
    TYPE##AnalysisManager* me = (TYPE##AnalysisManager*) other; \
    self->register##TYPE##Analyses(*me); \
}

LLVM_REGISTER_PB(Loop)
LLVM_REGISTER_PB(Function)
LLVM_REGISTER_PB(CGSCC)
LLVM_REGISTER_PB(Module)

extern "C" LLVMModulePassManagerRef LLVMBuildModulePassManager(LLVMPassBuilderRef ref){
    auto self = (PassBuilder*) ref;
    auto mod = self->buildPerModuleDefaultPipeline(OptimizationLevel::O0);
    auto box = (ModulePassManager*) calloc(1, sizeof(ModulePassManager));
    *box = std::move(mod);
    return (LLVMModulePassManagerRef) box;
}

// --- Module Passes --- //

extern "C" void LLVMAddRS4GCPass(LLVMModulePassManagerRef ref){
    auto* self = (llvm::ModulePassManager*) ref;
    self->addPass(RewriteStatepointsForGC());
}

extern "C" void LLVMAddAlwaysInliner(LLVMModulePassManagerRef ref){
    auto* self = (llvm::ModulePassManager*) ref;
    self->addPass(AlwaysInlinerPass());
}

// --- Function Passes --- //

extern "C" void LLVMAddMem2RegPass(LLVMFunctionPassManagerRef ref){
    auto* self = (llvm::FunctionPassManager*) ref;
    self->addPass(PromotePass());
}

extern "C" void LLVMAddInstructionCombining(LLVMFunctionPassManagerRef ref){
    auto* self = (llvm::FunctionPassManager*) ref;
    self->addPass(AggressiveInstCombinePass());
}

extern "C" void LLVMAddGVN(LLVMFunctionPassManagerRef ref){
    auto* self = (llvm::FunctionPassManager*) ref;
    self->addPass(GVNPass());
}

extern "C" void LLVMAddCFGSimplification(LLVMFunctionPassManagerRef ref){
    auto* self = (llvm::FunctionPassManager*) ref;
    self->addPass(SimplifyCFGPass());
}

extern "C" void LLVMAddTailRecursionElimination(LLVMFunctionPassManagerRef ref){
    auto* self = (llvm::FunctionPassManager*) ref;
    self->addPass(TailCallElimPass());
}

extern "C" void LLVMAddLoopVectorize(LLVMFunctionPassManagerRef ref){
    auto* self = (llvm::FunctionPassManager*) ref;
    self->addPass(LoopVectorizePass());
}

// --- Utilities --- //

extern "C" void LLVMAddFunctionPassToModule(LLVMFunctionPassManagerRef function, LLVMModulePassManagerRef theModule){
    auto* fun = (llvm::FunctionPassManager*) function;
    auto* mod = (llvm::ModulePassManager*) theModule;

    mod->addPass(llvm::createModuleToFunctionPassAdaptor(std::move(*fun)));
}

extern "C" void segfault_sigaction(int signal, siginfo_t *si, void *arg)
{
    static volatile unsigned char stack_trace_buffer[65536] = { 0 };

    backtrace((void **) stack_trace_buffer, 65536 / 8);
    size_t actual_size = 0;
    for(size_t i = 0; i < 65536 / 8; i++){
        void* addr = ((void**) stack_trace_buffer)[i];
        actual_size++;
        if(addr == nullptr){
            break;
        }
    }
    backtrace_symbols_fd((void* const*) stack_trace_buffer, (int)actual_size, 2);
    exit(0);
}

extern "C" void LLVMCrossRegisterProxies(LLVMPassBuilderRef pb,
                                         LLVMLoopAnalysisManagerRef lam,
                                         LLVMFunctionAnalysisManagerRef fam,
                                         LLVMCGSCCAnalysisManagerRef cgam,
                                         LLVMModuleAnalysisManagerRef mam){
    auto self = (PassBuilder*) pb;
    auto l = (LoopAnalysisManager*) lam;
    auto f = (FunctionAnalysisManager*) fam;
    auto cg = (CGSCCAnalysisManager*) cgam;
    auto m = (ModuleAnalysisManager*) mam;

    self->crossRegisterProxies(*l, *f, *cg, *m);
}

extern "C" void ensureStackTracePrinted(){
    struct sigaction sa = { 0 };
    sa.sa_flags = SA_SIGINFO;
    sa.sa_sigaction = segfault_sigaction;
    sigemptyset(&sa.sa_mask);
    sigaction(SIGSEGV, &sa, NULL);
}

extern "C" void LLVMRunModulePassManager(LLVMModuleRef llmodule, LLVMModulePassManagerRef ref, LLVMModuleAnalysisManagerRef analysis){
    auto* self = (llvm::ModulePassManager*) ref;
    auto* theModule = (llvm::Module*) llmodule;
    auto* analyzer = (llvm::ModuleAnalysisManager*) analysis;

    try{
        self->run(*theModule, *analyzer);
    }catch(std::exception& e){
        std::fprintf(stderr, "%s\n", e.what());
        exit(1);
    }
}