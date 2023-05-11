#include <stdint.h>

const extern struct StackMapHeader __LLVM_StackMaps;

const void* get_stackmap_address(){
    return &__LLVM_StackMaps;
}